#region GNU license
// MP-TVSeries - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2007
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#endregion
#if inclDownloaders
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using WindowPlugins.GUITVSeries.Feedback;

namespace WindowPlugins.GUITVSeries.Download
{
    class MyLevenshtein
    {
        public static double Match(string s, string t)
        {
            int nDistance = MediaPortal.Util.Levenshtein.Match(s.ToLower(), t.ToLower());
            return (Math.Max(s.Length, t.Length) - nDistance) / (double)Math.Max(s.Length, t.Length);
        }
    }

    enum MonitorAction
    {
        Full,
        List_Add,
        List_Remove
    }

    class CMonitorParameters
    {
        public MonitorAction m_action = MonitorAction.Full;
        public List<MonitorPathPair> m_files = null;

        public CMonitorParameters()
        {
        }

        public CMonitorParameters(MonitorAction action, List<MonitorPathPair> files)
        {
            m_action = action;
            m_files = files;
        }
    };

    class MonitorPathPair : PathPair
    {
        public String m_sTargetFileName = String.Empty;
        public DBEpisode m_episodeBestMatch = null;
        public MonitorPathPair(String sMatch, String sFull) : base(sMatch, sFull) {}
        public MonitorPathPair(String sMatch, String sFull, String sTargetFileName, DBEpisode episodeBestMatch) : base(sMatch, sFull)
        {
            m_sTargetFileName = sTargetFileName;
            m_episodeBestMatch = episodeBestMatch;
        }
    };

    class Monitor
    {
        private Watcher m_watcherUpdater = null;
        private List<CMonitorParameters> m_ActionQueue = new List<CMonitorParameters>();
        private bool m_moverWorking = false;
        private List<DBEpisode> m_DownloadingEpisodes = new List<DBEpisode>();
        private static Monitor s_Monitor;

        private TimerCallback m_timerDelegate = null;
        private System.Threading.Timer m_scanTimer = null;
        Interface m_feedback = null;

        public static void Start(Interface feedback)
        {
            if (s_Monitor == null)
                s_Monitor = new Monitor(feedback);
        }

        public Monitor(Interface feedback)
        {
            m_feedback = feedback;
            List<String> watchedFolders = new List<String>();
            watchedFolders.Add(DBOption.GetOptions(DBOption.cNewsLeecherDownloadPath));
            if (watchedFolders.Count > 0 && watchedFolders[0] != string.Empty) //path not set
            {

                m_watcherUpdater = new Watcher(watchedFolders);
                m_watcherUpdater.WatcherProgress += new Watcher.WatcherProgressHandler(m_watcherUpdater_WatcherProgress);
                m_watcherUpdater.StartFolderWatch();

                SQLCondition setcondition = new SQLCondition();
                setcondition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cDownloadPending, 1, SQLConditionType.Equal);
                m_DownloadingEpisodes = DBEpisode.Get(setcondition);

                // full scan of the download folders, then monitor new files 
                m_ActionQueue.Add(new CMonitorParameters());

                // timer check every 10 seconds
                m_timerDelegate = new TimerCallback(Clock);
                m_scanTimer = new System.Threading.Timer(m_timerDelegate, null, 10000, 10000);
            }
        }

        public static void AddPendingDownload(List<String> sSubjectNames, DBEpisode episode)
        {
            // check if already pending
            if (episode[DBOnlineEpisode.cDownloadPending] != 1)
            {
                episode[DBOnlineEpisode.cDownloadPending] = 1;
                String sAll = String.Empty;
                foreach (String s in sSubjectNames)
                    sAll += s + "|||";
                episode[DBOnlineEpisode.cDownloadExpectedNames] = sAll;
                episode.Commit();
                s_Monitor.m_DownloadingEpisodes.Add(episode);
            } 
        }

        void m_watcherUpdater_WatcherProgress(int nProgress, List<WatcherItem> modifiedFilesList)
        {
            List<MonitorPathPair> filesAdded = new List<MonitorPathPair>();
            List<MonitorPathPair> filesRemoved = new List<MonitorPathPair>();

            // go over the modified files list once in a while & update
            foreach (WatcherItem item in modifiedFilesList)
            {
                switch (item.m_type)
                {
                    case WatcherItemType.Added:
                        filesAdded.Add(new MonitorPathPair(item.m_sParsedFileName, item.m_sFullPathFileName));
                        break;

                    case WatcherItemType.Deleted:
                        filesRemoved.Add(new MonitorPathPair(item.m_sParsedFileName, item.m_sFullPathFileName));
                        break;
                }
            }

            // with out list of files, start the parsing process
            if (filesAdded.Count > 0)
            {
                // queue it
                lock (m_ActionQueue)
                {
                    m_ActionQueue.Add(new CMonitorParameters(MonitorAction.List_Add, filesAdded));
                }
            }

            if (filesRemoved.Count > 0)
            {
                // queue it
                lock (m_ActionQueue)
                {
                    m_ActionQueue.Add(new CMonitorParameters(MonitorAction.List_Remove, filesRemoved));
                }
            }
        }

        public void Clock(Object stateInfo)
        {
            if (!m_moverWorking)
            {
                // extract the current param, so that we don't call movefiles (which can invoke other threads) while the actionQueue is locked - that could cause a deadlock
                CMonitorParameters currentParam = null;
                lock (m_ActionQueue)
                {
                    if (m_ActionQueue.Count > 0)
                    {
                        currentParam = m_ActionQueue[0];
                        m_ActionQueue.RemoveAt(0);
                    }
                }
                if (currentParam != null)
                {
                    m_moverWorking = true;
                    MoveFiles(currentParam);
                    m_moverWorking = false;
                }
            }
        }

        public void MoveFiles(CMonitorParameters param)
        {
            switch (param.m_action)
            {
                case MonitorAction.Full:
                {
                    // full scan of the folder(s)
                    List<String> watchedFolders = new List<String>();
                    watchedFolders.Add(DBOption.GetOptions(DBOption.cNewsLeecherDownloadPath));
                    List<PathPair> watchedFiles_simple = Filelister.GetFiles(watchedFolders);
                    List<MonitorPathPair> watchedFiles = new List<MonitorPathPair>();
                    foreach (PathPair pair in watchedFiles_simple)
                        watchedFiles.Add(new MonitorPathPair(pair.m_sMatch_FileName, pair.m_sFull_FileName));
                    TryAndMoveFile(watchedFiles);
                }
                break;

            case MonitorAction.List_Add:
                {
                    TryAndMoveFile(param.m_files);
                }
                break;

            case MonitorAction.List_Remove:
                {
                    // nothing to do really
                }
                break;

            }
        }

        private static int EpisodeMatchesCompare(KeyValuePair<double, DBEpisode> x, KeyValuePair<double, DBEpisode> y)
        {
            return y.Key.CompareTo(x.Key);
        }

        public void TryAndMoveFile(List<MonitorPathPair> files)
        {
            foreach (MonitorPathPair pair in files)
            {
                List<PathPair> currentFile = new List<PathPair>();
                currentFile.Add(new PathPair(pair.m_sMatch_FileName, pair.m_sFull_FileName));
                parseResult result = LocalParse.Parse(currentFile)[0];
                if (result.success)
                {
                    if (pair.m_sTargetFileName == String.Empty) 
                    {
                        result.match_filename.ToLower();
                        List<KeyValuePair<double, DBEpisode>> episodeMatches = new List<KeyValuePair<double, DBEpisode>>();

                        // first, figure out if the detected file is part of the pending downloads
                        foreach (DBEpisode episode in m_DownloadingEpisodes)
                        {
                            // use nfo name if available
                            double fOverallMatchValue = 0;
                            int nTotalChecks = 2;

                            if (episode[DBOnlineEpisode.cDownloadExpectedNames] != String.Empty)
                            {
                                // this has a lot of weight. If we find a good match it's probably it
                                nTotalChecks+=4;
                                double fLocalMatchValue = 0;
                                String sFileToMatch = System.IO.Path.GetFileNameWithoutExtension(result.match_filename);
                                String sPathToMatch = String.Empty;
                                if (result.match_filename.IndexOf(sFileToMatch) > 0)
                                    sPathToMatch = result.match_filename.Substring(0, result.match_filename.IndexOf(sFileToMatch) - 1);
                                String RegExp = @"([^|]+)\|\|\|";
                                Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                MatchCollection matches = Engine.Matches(episode[DBOnlineEpisode.cDownloadExpectedNames]);
                                foreach (Match match in matches)
                                {
                                    double fTempMatchValue = MyLevenshtein.Match(sFileToMatch, match.Groups[1].Value);
                                    if (fTempMatchValue > fLocalMatchValue)
                                    {
                                        fLocalMatchValue = fTempMatchValue;
                                    }
                                    // also work on the path as it's often more related than the filename
                                    fTempMatchValue = MyLevenshtein.Match(sPathToMatch, match.Groups[1].Value);
                                    if (fTempMatchValue > fLocalMatchValue)
                                    {
                                        fLocalMatchValue = fTempMatchValue;
                                    }
                                }

                                fOverallMatchValue += 4*fLocalMatchValue;
                            }

                            // otherwise try to parse the name and pray
                            if (Convert.ToInt32(result.parser.Matches[DBEpisode.cSeasonIndex]) == episode[DBEpisode.cSeasonIndex] && Convert.ToInt32(result.parser.Matches[DBEpisode.cEpisodeIndex]) == episode[DBEpisode.cEpisodeIndex])
                            {
                                double fLocalMatchValue = 0;
                                // episode & season index matches, let's deal with the series name
                                DBOnlineSeries series = new DBOnlineSeries(episode[DBEpisode.cSeriesID]);
                                fLocalMatchValue = MyLevenshtein.Match(result.parser.Matches[DBSeries.cParsedName], ((String)series[DBOnlineSeries.cPrettyName]));
                                fOverallMatchValue += fLocalMatchValue;

                                // and also do a simple find: if we can find parts of the pretty name somewhere in the filename, consider it a bonus
                                String RegExp = @"([^ ]+)";
                                Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                MatchCollection matches = Engine.Matches(series[DBOnlineSeries.cPrettyName]);
                                if (matches.Count != 0)
                                {
                                    foreach (Match match in matches)
                                    {
                                        if (result.match_filename.Contains(match.Groups[1].Value.ToLower()))
                                            fOverallMatchValue += 1 / (double)matches.Count;
                                    }
                                }
                            }

                            // final verdict:
                            fOverallMatchValue /= nTotalChecks;
                            episodeMatches.Add(new KeyValuePair<double, DBEpisode>(fOverallMatchValue, episode));
                        }

                        episodeMatches.Sort(EpisodeMatchesCompare);
                        if (episodeMatches.Count == 0)
                            return;

                        if (episodeMatches[0].Key < 0.6)
                        {
                            // we didn't get any proper match. I assume in this case we need to ask the user
                            // now, sort & take the first one as our best result
                            List<CItem> Choices = new List<CItem>();
                            foreach (KeyValuePair<double, DBEpisode> match in episodeMatches)
                            {
                                // online episode has to exist here
                                Choices.Add(new CItem(match.Value.onlineEpisode.CompleteTitle, String.Empty, match.Value));
                            }

                            ChooseFromSelectionDescriptor descriptor = new ChooseFromSelectionDescriptor();
                            descriptor.m_sTitle = "What is this file?";
                            descriptor.m_sItemToMatchLabel = "filename:";
                            descriptor.m_sItemToMatch = System.IO.Path.GetFileNameWithoutExtension(result.match_filename);
                            descriptor.m_sListLabel = "Matching file to episode:";
                            descriptor.m_List = Choices;
                            descriptor.m_sbtnIgnoreLabel = String.Empty;

                            bool bReady = false;
                            while (!bReady)
                            {
                                CItem Selected = null;
                                ReturnCode resultFeedback = m_feedback.ChooseFromSelection(descriptor, out Selected);
                                switch (resultFeedback)
                                {
                                    case ReturnCode.NotReady:
                                        {
                                            // we'll wait until the plugin is loaded - we don't want to show up unrequested popups outside the tvseries pages
                                            Thread.Sleep(5000);
                                        }
                                        break;

                                    case ReturnCode.OK:
                                        {
                                            pair.m_episodeBestMatch = Selected.m_Tag as DBEpisode;
                                            bReady = true;
                                        }
                                        break;

                                    default:
                                        {
                                            // exit too if cancelled
                                            pair.m_episodeBestMatch = null;
                                            bReady = true;
                                        }
                                        break;
                                }
                            }
                        }
                        else
                        {
                            pair.m_episodeBestMatch = episodeMatches[0].Value;
                        }

                        // arbitrary value to trigger a manual selection. More than 60% success is likely to be the good one
                        if (pair.m_episodeBestMatch != null)
                        {
                            String sTargetFolder = String.Empty;
                            DBOnlineSeries seriesBestMatch = new DBOnlineSeries(pair.m_episodeBestMatch[DBEpisode.cSeriesID]);

                            // ok, file isn't locked, so try to figure out where it should go

                            // to figure out where to move the file, first try to see if there is a season folder already present
                            SQLCondition conditions = new SQLCondition();
                            conditions.Add(new DBEpisode(), DBEpisode.cSeasonIndex, pair.m_episodeBestMatch[DBEpisode.cSeasonIndex], SQLConditionType.Equal);
                            conditions.Add(new DBEpisode(), DBEpisode.cSeriesID, pair.m_episodeBestMatch[DBEpisode.cSeriesID], SQLConditionType.Equal);
                            List<DBEpisode> SeriesEpisodes = DBEpisode.Get(conditions);

                            if (SeriesEpisodes.Count > 0)
                                sTargetFolder = System.IO.Path.GetDirectoryName(SeriesEpisodes[0][DBEpisode.cFilename]);
                            else
                            {
                                // ok, default back to just the series folder
                                conditions = new SQLCondition();
                                conditions.Add(new DBEpisode(), DBEpisode.cSeriesID, pair.m_episodeBestMatch[DBEpisode.cSeriesID], SQLConditionType.Equal);
                                SeriesEpisodes = DBEpisode.Get(conditions);

                                if (SeriesEpisodes.Count > 0)
                                    sTargetFolder = System.IO.Path.GetDirectoryName(SeriesEpisodes[0][DBEpisode.cFilename]);
                                else
                                {
                                    // nothing! well in that case, create a folder named as the series, at the root of the first monitored disk
                                    DBImportPath[] importPathes = DBImportPath.GetAll();
                                    if (importPathes.Length > 0)
                                        sTargetFolder = importPathes[0][DBImportPath.cPath] + "\\" + seriesBestMatch[DBOnlineSeries.cPrettyName];
                                }
                            }

                            if (sTargetFolder != String.Empty)
                            {
                                String sOutputFile;
                                if (DBOption.GetOptions(DBOption.cDownloadMonitor_RenameFiles) == 1)
                                {
                                    // "beautifulizing" the filename
                                    sOutputFile = String.Format("{0} - {1}x{2:D2} - {3}{4}", seriesBestMatch[DBOnlineSeries.cPrettyName], (int)pair.m_episodeBestMatch[DBEpisode.cSeasonIndex], (int)pair.m_episodeBestMatch[DBEpisode.cEpisodeIndex], pair.m_episodeBestMatch[DBOnlineEpisode.cEpisodeName], System.IO.Path.GetExtension(result.full_filename));
                                    sOutputFile = sOutputFile.Replace(":", "");
                                    sOutputFile = sOutputFile.Replace("?", "");
                                    sOutputFile = sOutputFile.Replace("\\", "");
                                    sOutputFile = sOutputFile.Replace("*", "");
                                    sOutputFile = sOutputFile.Replace("<", "");
                                    sOutputFile = sOutputFile.Replace(">", "");
                                    sOutputFile = sOutputFile.Replace("|", "");
                                    sOutputFile = sOutputFile.Replace("/", "");
                                }
                                else
                                    sOutputFile = System.IO.Path.GetFileName(result.full_filename);

                                pair.m_sTargetFileName = sTargetFolder + "\\" + sOutputFile;
                            }
                        }
                    }

                    if (pair.m_sTargetFileName != String.Empty)
                    {
                        bool bMoveable = true;
                        // will check if the file is locked for writing, and if it is, we'll keep it in a list for further tries
                        try
                        {
                            if (System.IO.File.Exists(result.full_filename))
                            {
                                FileStream stream = System.IO.File.OpenWrite(result.full_filename);
                                stream.Close();
                            }
                        }
                        catch
                        {
                            bMoveable = false;
                        }

                        bool bMoved = false;
                        if (bMoveable)
                        {
                            // let's move the file
                            try
                            {
                                System.IO.File.Move(result.full_filename, pair.m_sTargetFileName);
                                // if file isn't there anymore, consider success
                                if (!System.IO.File.Exists(result.full_filename))
                                    bMoved = true;
                            }
                            catch {}
                        }

                        if (!bMoved)
                        {
                            // move failed for some reason - reinsert this file in the loop
                            List<MonitorPathPair> path = new List<MonitorPathPair>();
                            path.Add(pair);
                            m_ActionQueue.Add(new CMonitorParameters(MonitorAction.List_Add, path));
                        }
                        else
                        {
                            // file has been moved - remove from our list, clears the pending download flag
                            m_DownloadingEpisodes.Remove(pair.m_episodeBestMatch);
                            pair.m_episodeBestMatch[DBOnlineEpisode.cDownloadPending] = 0;
                            pair.m_episodeBestMatch.Commit();
                        }
                    }
                }
            }
        }
    }
}
#endif