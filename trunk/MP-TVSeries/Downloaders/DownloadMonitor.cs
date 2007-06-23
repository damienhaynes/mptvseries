using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

namespace WindowPlugins.GUITVSeries.Download
{
    enum MonitorAction
    {
        Full,
        List_Add,
        List_Remove
    }

    class CMonitorParameters
    {
        public MonitorAction m_action = MonitorAction.Full;
        public List<PathPair> m_files = null;

        public CMonitorParameters()
        {
        }

        public CMonitorParameters(MonitorAction action, List<PathPair> files)
        {
            m_action = action;
            m_files = files;
        }
    };

    class Monitor
    {
        private Watcher m_watcherUpdater = null;
        private List<CMonitorParameters> m_ActionQueue = new List<CMonitorParameters>();
        private bool m_moverWorking = false;
        private List<DBEpisode> m_DownloadingEpisodes = new List<DBEpisode>();
        private static Monitor s_Monitor = new Monitor();

        private TimerCallback m_timerDelegate = null;
        private System.Threading.Timer m_scanTimer = null;

        public Monitor()
        {
            List<String> watchedFolders = new List<String>();
            watchedFolders.Add(DBOption.GetOptions(DBOption.cNewsLeecherDownloadPath));

            m_watcherUpdater = new Watcher(watchedFolders);
            m_watcherUpdater.WatcherProgress += new Watcher.WatcherProgressHandler(m_watcherUpdater_WatcherProgress);
            m_watcherUpdater.StartFolderWatch();

            SQLCondition setcondition = new SQLCondition();
            setcondition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cDownloadPending, 1, SQLConditionType.Equal);
            m_DownloadingEpisodes = DBEpisode.Get(setcondition);

            // full scan of the download folders, then monitor new files 
            m_ActionQueue.Add(new CMonitorParameters());

            // timer check every seconds
            m_timerDelegate = new TimerCallback(Clock);
            m_scanTimer = new System.Threading.Timer(m_timerDelegate, null, 1000, 1000);
        }

        public static void AddPendingDownload(DBEpisode episode)
        {
            // check if already pending
            if (episode[DBOnlineEpisode.cDownloadPending] != 1)
            {
                episode[DBOnlineEpisode.cDownloadPending] = 1;
                episode.Commit();
                s_Monitor.m_DownloadingEpisodes.Add(episode);
            } 
        }

        void m_watcherUpdater_WatcherProgress(int nProgress, List<WatcherItem> modifiedFilesList)
        {
            List<PathPair> filesAdded = new List<PathPair>();
            List<PathPair> filesRemoved = new List<PathPair>();

            // go over the modified files list once in a while & update
            foreach (WatcherItem item in modifiedFilesList)
            {
                switch (item.m_type)
                {
                    case WatcherItemType.Added:
                        filesAdded.Add(new PathPair(item.m_sParsedFileName, item.m_sFullPathFileName));
                        break;

                    case WatcherItemType.Deleted:
                        filesRemoved.Add(new PathPair(item.m_sParsedFileName, item.m_sFullPathFileName));
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
                lock (m_ActionQueue)
                {
                    if (m_ActionQueue.Count > 0)
                    {
                        m_moverWorking = true;
                        MoveFiles(m_ActionQueue[0]);
                        m_ActionQueue.RemoveAt(0);
                        m_moverWorking = false;
                    }
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

                    TryAndMoveFile(Filelister.GetFiles(watchedFolders));
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

        public void TryAndMoveFile(List<PathPair> files)
        {
            List<parseResult> parseResults = LocalParse.Parse(files);
            foreach (parseResult result in parseResults)
            {
                if (result.success)
                {
                    DBEpisode episodeBestMatch = null;
                    DBOnlineSeries seriesBestMatch = null;
                    int nBestDistance = 10000;

                    // first, figure out if the detected file is part of the pending downloads
                    foreach (DBEpisode episode in m_DownloadingEpisodes)
                    {
                        if (Convert.ToInt32(result.parser.Matches[DBEpisode.cSeasonIndex]) == episode[DBEpisode.cSeasonIndex] && Convert.ToInt32(result.parser.Matches[DBEpisode.cEpisodeIndex]) == episode[DBEpisode.cEpisodeIndex])
                        {
                            // episode & season index matches, let's deal with the series name
                            DBOnlineSeries series = new DBOnlineSeries(episode[DBEpisode.cSeriesID]);
                            int nDistance = Levenshtein.CalcEditDistance(result.parser.Matches[DBSeries.cParsedName], series[DBOnlineSeries.cPrettyName]);
                            if (nDistance < nBestDistance)
                            {
                                nBestDistance = nDistance;
                                episodeBestMatch = episode;
                                seriesBestMatch = series;
                            }
                        }
                    }

                    if (episodeBestMatch != null)
                    {
                        // we are just hoping here that the match is actually a good one!
                        bool bMoveable = true;
                        // will check if the file is locked for writing, and if it is, we'll keep it in a list for further tries
                        try
                        {
                            FileStream fileStream = new FileStream(result.full_filename, FileMode.Append);
                            fileStream.Close();
                        }
                        catch (IOException e)
                        {
                            bMoveable = false;
                        }

                        if (bMoveable)
                        {
                            // ok, file isn't locked, so try to figure out where it should go
                            String sTargetFolder = String.Empty;

                            // to figure out where to move the file, first try to see if there is a season folder already present
                            SQLCondition conditions = new SQLCondition();
                            conditions.Add(new DBEpisode(), DBEpisode.cSeasonIndex, episodeBestMatch[DBEpisode.cSeasonIndex], SQLConditionType.Equal);
                            conditions.Add(new DBEpisode(), DBEpisode.cSeriesID, episodeBestMatch[DBEpisode.cSeriesID], SQLConditionType.Equal);
                            List<DBEpisode> SeriesEpisodes = DBEpisode.Get(conditions);

                            if (SeriesEpisodes.Count > 0)
                                sTargetFolder = System.IO.Path.GetDirectoryName(SeriesEpisodes[0][DBEpisode.cFilename]);
                            else
                            {
                                // ok, default back to just the series folder
                                conditions = new SQLCondition();
                                conditions.Add(new DBEpisode(), DBEpisode.cSeriesID, episodeBestMatch[DBEpisode.cSeriesID], SQLConditionType.Equal);
                                SeriesEpisodes = DBEpisode.Get(conditions);

                                if (SeriesEpisodes.Count > 0)
                                    sTargetFolder = System.IO.Path.GetDirectoryName(SeriesEpisodes[0][DBEpisode.cFilename]);
                                else
                                {
                                    // nothing! well in that case, create a folder named as the series, at the root of the first monitored disk
                                    DBImportPath[] importPathes = DBImportPath.GetAll();
                                    if (importPathes.Length > 0)
                                        sTargetFolder = importPathes[0][DBImportPath.cPath] + "\\" + episodeBestMatch[DBOnlineSeries.cPrettyName];
                                }
                            }

                            if (sTargetFolder != String.Empty)
                            {
                                // let's move the file, "beautifulizing" it
                                try
                                {
                                    String sOutputFile = String.Format("{0} - {1}x{2:D2} - {3}{4}", seriesBestMatch[DBOnlineSeries.cPrettyName], (int)episodeBestMatch[DBEpisode.cSeasonIndex], (int)episodeBestMatch[DBEpisode.cEpisodeIndex], episodeBestMatch[DBOnlineEpisode.cEpisodeName], System.IO.Path.GetExtension(result.full_filename)); 
                                    sOutputFile = sOutputFile.Replace(":", "");
                                    sOutputFile = sOutputFile.Replace("?", "");
                                    sOutputFile = sOutputFile.Replace("\\", "");
                                    sOutputFile = sOutputFile.Replace("*", "");
                                    sOutputFile = sOutputFile.Replace("<", "");
                                    sOutputFile = sOutputFile.Replace(">", "");
                                    sOutputFile = sOutputFile.Replace("|", "");
                                    sOutputFile = sOutputFile.Replace("/", "");

                                    System.IO.File.Move(result.full_filename, sTargetFolder + "\\" + sOutputFile);
                                    // if file isn't there anymore, consider success
                                    if (System.IO.File.Exists(result.full_filename))
                                        bMoveable = false;
                                }
                                catch (Exception)
                                {
                                    bMoveable = false;
                                }
                            }
                        }


                        if (!bMoveable)
                        {
                            // move failed for some reason - reinsert this file in the loop
                            List<PathPair> path = new List<PathPair>();
                            path.Add(new PathPair(result.match_filename, result.full_filename));
                            m_ActionQueue.Add(new CMonitorParameters(MonitorAction.List_Add, path));
                        }
                    }
                }
            }
        }
    }
}
