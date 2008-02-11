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


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using System.IO;

namespace WindowPlugins.GUITVSeries
{
    enum WatcherItemType
    {
        Added,
        Deleted
    }

    class WatcherItem
    {
        public String m_sFullPathFileName;
        public String m_sParsedFileName;
        public WatcherItemType m_type;

        public WatcherItem(FileSystemWatcher watcher, RenamedEventArgs e, bool bOldName)
        {
            if (bOldName)
            {
                m_sFullPathFileName = e.OldFullPath;
                m_sParsedFileName = m_sFullPathFileName.Substring(watcher.Path.Length).TrimStart('\\');
                m_type = WatcherItemType.Deleted;
                MPTVSeriesLog.Write("File monitor: " + m_sParsedFileName + " " + m_type);
            }
            else
            {
                m_sFullPathFileName = e.FullPath;
                m_sParsedFileName = m_sFullPathFileName.Substring(watcher.Path.Length).TrimStart('\\');
                m_type = WatcherItemType.Added;
                MPTVSeriesLog.Write("File monitor: " + m_sParsedFileName + " " + m_type);
            }
        }

        public WatcherItem(FileSystemWatcher watcher, FileSystemEventArgs e)
        {
            m_sFullPathFileName = e.FullPath;
            m_sParsedFileName = m_sFullPathFileName.Substring(watcher.Path.Length).TrimStart('\\');
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Deleted:
                    m_type = WatcherItemType.Deleted;
                    break;

                default:
                    m_type = WatcherItemType.Added;
                    break;
            }
            MPTVSeriesLog.Write("File monitor: " + m_sParsedFileName + " " + m_type);
        }
    };

    class Watcher
    {
        public BackgroundWorker worker = new BackgroundWorker();
        List<String> m_WatchedFolders;
        List<System.IO.FileSystemWatcher> m_watchersList = new List<System.IO.FileSystemWatcher>();
        List<WatcherItem> m_modifiedFilesList = new List<WatcherItem>();
        bool progressUpdateRequired = false;
        public delegate void WatcherProgressHandler(int nProgress, List<WatcherItem> modifiedFilesList);

        /// <summary>
        /// This will be triggered once all the SeriesAndEpisodeInfo has been parsed completely.
        /// </summary>
        public event WatcherProgressHandler WatcherProgress;

        public Watcher(List<String> WatchedFolders)
        {
            m_WatchedFolders = WatchedFolders;
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.DoWork += new DoWorkEventHandler(workerWatcher_DoWork);
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (WatcherProgress != null) // only if any subscribers exist
                WatcherProgress.Invoke(e.ProgressPercentage, e.UserState as List<WatcherItem>);
        }

        public void StartFolderWatch()
        {
            // start the thread that is going to handle the addition in the db when files change
            worker.RunWorkerAsync();
        }

        void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            // rename: delete the old, add the new
            lock (m_modifiedFilesList)
            {
                String sOldExtention = System.IO.Path.GetExtension(e.OldFullPath);
                if (MediaPortal.Util.Utils.VideoExtensions.IndexOf(sOldExtention) != -1)
                    m_modifiedFilesList.Add(new WatcherItem(sender as FileSystemWatcher, e, true));
                String sNewExtention = System.IO.Path.GetExtension(e.FullPath);
                if (MediaPortal.Util.Utils.VideoExtensions.IndexOf(sNewExtention) != -1)
                    m_modifiedFilesList.Add(new WatcherItem(sender as FileSystemWatcher, e, false));
            }
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            // a file has changed! created, not created, whatever. Just add it to our list. We only process this list once in a while
            lock (m_modifiedFilesList)
            {
                foreach (WatcherItem item in m_modifiedFilesList)
                {
                    if (item.m_sFullPathFileName == e.FullPath)
                        return;
                }

                m_modifiedFilesList.Add(new WatcherItem(sender as FileSystemWatcher, e));
                progressUpdateRequired = true; // signal the worker thread (not using events because we don't want to react immediatly but only every couple of seconds at the most
            }
        }

        void setUpWatches()
        {
            if (m_watchersList.Count > 0) return; // this can only run once
            // ok let's see ... go through all enable import folders, and add a watchfolder on it
            foreach (String sWatchedFolder in m_WatchedFolders)
            {
                if (Directory.Exists(sWatchedFolder))
                {
                    // one watcher for each extension type
                    foreach (String extention in MediaPortal.Util.Utils.VideoExtensions)
                    {
                        FileSystemWatcher watcher = new FileSystemWatcher();
                        watcher.Filter = "*" + extention;
                        watcher.Path = sWatchedFolder;
                        watcher.IncludeSubdirectories = true;
                        watcher.NotifyFilter = NotifyFilters.FileName; 
                        // Inker, I don't think lastwrite is such as good idea if you have your download/recording dir monitored
                        // only check for lastwrite .. I believe that's the only thing we're interested in
                        watcher.Changed += new FileSystemEventHandler(watcher_Changed);
                        watcher.Created += new FileSystemEventHandler(watcher_Changed);
                        watcher.Deleted += new FileSystemEventHandler(watcher_Changed);
                        watcher.Renamed += new RenamedEventHandler(watcher_Renamed);
                        watcher.EnableRaisingEvents = true;
                        m_watchersList.Add(watcher);
                    }
                }
            }
        }

        void signalActionRequired()
        {
            try
            {
                List<WatcherItem> outList = new List<WatcherItem>();
                lock (m_modifiedFilesList)
                {
                    // go over the modified files list once in a while & update
                    outList.AddRange(m_modifiedFilesList);
                    m_modifiedFilesList.Clear();
                    if (outList.Count > 0)
                        worker.ReportProgress(0, outList);
                }
                outList = null;
            }
            catch (Exception exp)
            {
                MPTVSeriesLog.Write("Exception happened in workerWatcher_DoWork: " + exp.Message);
            }
        }

        void workerWatcher_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            setUpWatches();
            while (!worker.CancellationPending)
            {
                if (progressUpdateRequired)
                {
                    signalActionRequired();
                }
                // wait
                Thread.Sleep(3000); // every 3 seconds do a quick check if we need to do something
            }
        }
    };

    enum ParsingAction
    {
        Full,
        List_Add,
        List_Remove,
        LocalScanNoExactMatch,
    }

    class CParsingParameters
    {
        public ParsingAction m_action = ParsingAction.Full;
        public bool m_bLocalScan = true;
        public bool m_bUpdateScan = true;
        public List<PathPair> m_files = null;

        public CParsingParameters(bool bScanNew, bool bUpdateExisting)
        {
            m_bLocalScan = bScanNew;
            m_bUpdateScan = bUpdateExisting;
        }

        public CParsingParameters(ParsingAction action, List<PathPair> files)
        {
            m_action = action;
            m_files = files;
            m_bLocalScan = true;
            m_bUpdateScan = false;
        }
    };

    class OnlineParsing
    {
        public BackgroundWorker worker = new BackgroundWorker();
        Feedback.Interface m_feedback = null;

        bool m_bDataUpdated = false;
        bool m_bFullSeriesRetrieval = false;
        bool m_bReparseNeeded = false;
        bool m_bNoExactMatch = false;
        CParsingParameters m_params = null;

        public delegate void OnlineParsingProgressHandler(int nProgress);
        public delegate void OnlineParsingCompletedHandler(bool bDataUpdated);

        /// <summary>
        /// This will be triggered once all the SeriesAndEpisodeInfo has been parsed completely.
        /// </summary>
        public event OnlineParsingProgressHandler OnlineParsingProgress;
        public event OnlineParsingCompletedHandler OnlineParsingCompleted;

        public OnlineParsing(Feedback.Interface feedback)
        {
            m_feedback = feedback;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (OnlineParsingCompleted != null) // only if any subscribers exist
            {
                this.OnlineParsingCompleted.Invoke(m_bDataUpdated);
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (OnlineParsingProgress != null) // only if any subscribers exist
                OnlineParsingProgress.Invoke(e.ProgressPercentage);
        }

        public bool IsWorking
        {
            get { return worker.IsBusy; }
        }

        public bool Start(CParsingParameters param)
        {
            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync(param);
                return true;
            }
            return false;
        }

        public bool LocalScan
        {
            get { if (m_params != null) return m_params.m_bLocalScan; else return false; }
        }

        public bool UpdateScan
        {
            get { if (m_params != null) return m_params.m_bUpdateScan; else return false; }
        }

        public void Cancel()
        {
            worker.CancelAsync();
            //            m_bAbort = true;
        }

        public void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            m_params = e.Argument as CParsingParameters;
            m_bFullSeriesRetrieval = DBOption.GetOptions(DBOption.cFullSeriesRetrieval);
            m_bNoExactMatch = false;
            worker.ReportProgress(0);

            switch (m_params.m_action)
            {
                case ParsingAction.List_Remove:
                    MPTVSeriesLog.Write("***************************************************************************");
                    MPTVSeriesLog.Write("*******************    List_Remove Starting     ***************************");
                    MPTVSeriesLog.Write("***************************************************************************");
                    // should we remove deleted files?
                    if (!DBOption.GetOptions(DBOption.cDontClearMissingLocalFiles))
                    {
                        List<DBOnlineSeries> relatedSeries = new List<DBOnlineSeries>();
                        List<DBSeason> relatedSeasons = new List<DBSeason>();

                        foreach (PathPair pair in m_params.m_files)
                        {
                            if(!LocalParse.isOnRemovable(pair.m_sFull_FileName))
                            {
                                DBEpisode episode = new DBEpisode(pair.m_sFull_FileName);
                                
                                // already in?
                                bool bSeasonFound = false;
                                foreach (DBSeason season in relatedSeasons)
                                    if (season[DBSeason.cSeriesID] == episode[DBEpisode.cSeriesID] && season[DBSeason.cIndex] == episode[DBEpisode.cSeasonIndex])
                                    {
                                        bSeasonFound = true;
                                        break;
                                    }
                                if (!bSeasonFound)
                                    relatedSeasons.Add(new DBSeason(episode[DBEpisode.cSeriesID], episode[DBEpisode.cSeasonIndex]));

                                bool bSeriesFound = false;
                                foreach (DBOnlineSeries series in relatedSeries)
                                    if (series[DBOnlineSeries.cID] == episode[DBEpisode.cSeriesID])
                                    {
                                        bSeriesFound = true;
                                        break;
                                    }
                                if (!bSeriesFound)
                                    relatedSeries.Add(new DBOnlineSeries(episode[DBEpisode.cSeriesID]));

                                SQLCondition condition = new SQLCondition();
                                condition.Add(new DBEpisode(), DBEpisode.cFilename, pair.m_sFull_FileName, SQLConditionType.Equal);
                                DBEpisode.Clear(condition);
                            }
                        }

                        // now go over the touched seasons & series
                        foreach (DBSeason season in relatedSeasons)
                        {
                            if (DBEpisode.Get(season[DBSeason.cSeriesID], season[DBSeason.cIndex], false).Count > 0)
                            {
                                season[DBSeason.cHasLocalFilesTemp] = true;
                                season[DBSeason.cHasEpisodes] = true;
                            }
                            else
                                season[DBSeason.cHasLocalFilesTemp] = false;

                            DBEpisode episode = DBEpisode.GetFirstUnwatched(season[DBSeason.cSeriesID], season[DBSeason.cIndex]);
                            if (episode != null)
                                season[DBSeason.cUnwatchedItems] = true;
                            else
                                season[DBSeason.cUnwatchedItems] = false;

                            season.Commit();
                        }

                        foreach (DBOnlineSeries series in relatedSeries)
                        {
                            if (DBEpisode.Get((int)series[DBOnlineSeries.cID], false).Count > 0)
                                series[DBOnlineSeries.cHasLocalFilesTemp] = true;
                            else
                                series[DBOnlineSeries.cHasLocalFilesTemp] = false;

                            DBEpisode episode = DBEpisode.GetFirstUnwatched(series[DBSeries.cID]);
                            if (episode != null)
                                series[DBOnlineSeries.cUnwatchedItems] = true;
                            else
                                series[DBOnlineSeries.cUnwatchedItems] = false;

                            series.Commit();
                        }

                        // and copy the HasLocalFileTemp value into the real one
                        DBSeries.GlobalSet(DBOnlineSeries.cHasLocalFiles, DBOnlineSeries.cHasLocalFilesTemp);
                        DBSeason.GlobalSet(DBSeason.cHasLocalFiles, DBSeason.cHasLocalFilesTemp);
                        // and we are done, the backgroundworker is going to notify so
                        MPTVSeriesLog.Write("***************************************************************************");
                        MPTVSeriesLog.Write("*******************          Completed          ***************************");
                        MPTVSeriesLog.Write("***************************************************************************");
                    }
                    return;

                case ParsingAction.List_Add:
                    MPTVSeriesLog.Write("***************************************************************************");
                    MPTVSeriesLog.Write("*******************       List_Add Starting     ***************************");
                    MPTVSeriesLog.Write("***************************************************************************");
                    ParseLocal(m_params.m_files);
                    break;

                case ParsingAction.LocalScanNoExactMatch:
                    MPTVSeriesLog.Write("***************************************************************************");
                    MPTVSeriesLog.Write(String.Format("******************* LocalScanNoExactMatch Starting {0} - {1}   ***************************", m_params.m_bLocalScan, m_params.m_bUpdateScan));
                    MPTVSeriesLog.Write("***************************************************************************");
                    m_bNoExactMatch = true;
                    goto case ParsingAction.Full;

                case ParsingAction.Full:
                    MPTVSeriesLog.Write("***************************************************************************");
                    MPTVSeriesLog.Write(String.Format("******************* Full Starting {0} - {1}   ***************************", m_params.m_bLocalScan, m_params.m_bUpdateScan));
                    MPTVSeriesLog.Write("***************************************************************************");
                    if (m_params.m_bLocalScan)
                    {
                        // mark all files in the db as not processed (to figure out which ones we'll have to remove after the import)
                        DBEpisode.GlobalSet(DBEpisode.cImportProcessed, 2);
                        // also clear all season & series for local files
                        DBSeries.GlobalSet(DBOnlineSeries.cHasLocalFilesTemp, false);
                        DBSeason.GlobalSet(DBSeason.cHasLocalFilesTemp, false);

                        List<String> listFolders = new List<string>();
                        DBImportPath[] importPathes = DBImportPath.GetAll();
                        if (importPathes != null)
                        {
                            foreach (DBImportPath importPath in importPathes)
                            {
                                if (importPath[DBImportPath.cEnabled] != 0)
                                {
                                    listFolders.Add(importPath[DBImportPath.cPath]);
                                }
                            }
                        }

                        ParseLocal(Filelister.GetFiles(listFolders));
                        
                        // now, remove all episodes still processed = 0, the weren't find in the scan
                        if (!DBOption.GetOptions(DBOption.cDontClearMissingLocalFiles))
                        {
                            SQLCondition condition = new SQLCondition();
                            condition.Add(new DBEpisode(), DBEpisode.cImportProcessed, 2, SQLConditionType.Equal);
                            condition.Add(new DBEpisode(), DBEpisode.cIsOnRemovable, false, SQLConditionType.Equal);
                            DBEpisode.Clear(condition);
                        }
                        // and copy the HasLocalFileTemp value into the real one
                        DBSeries.GlobalSet(DBOnlineSeries.cHasLocalFiles, DBOnlineSeries.cHasLocalFilesTemp);
                        DBSeason.GlobalSet(DBSeason.cHasLocalFiles, DBSeason.cHasLocalFilesTemp);
                    } 
                    break;
            }

            // now on with online parsing
            if (DBOption.GetOptions(DBOption.cOnlineParseEnabled) == 1)
            {
                int counter = 0;
                m_bReparseNeeded = true;
                while (m_bReparseNeeded && counter < 4) // limit the max number of loops
                {
                    m_bReparseNeeded = false;

                    GetSeries();

                    GetEpisodes();

                    UpdateSeries(true);
                    if (m_params.m_bUpdateScan)
                    {
                        worker.ReportProgress(45);
                        // now do an regular update (refresh, with timestamp) on all the series
                        UpdateSeries(false);
                    }
                    else
                    {
                        worker.ReportProgress(50);
                    }

                    // now update the episodes
                    UpdateEpisodes(true);

                    if (m_params.m_bUpdateScan)
                    {
                        // now refresh existing episodes
                        UpdateEpisodes(false);
                    }
                    counter++;
                }
                // we really only need to do this once
                // update new series for banners
                UpdateBanners(true);
                if (m_params.m_bUpdateScan)
                {
                    // refresh existing banners
                    UpdateBanners(false);
                }

                UpdateEpisodeThumbNails();
            }
            // and we are done, the backgroundworker is going to notify so
            MPTVSeriesLog.Write("***************************************************************************");
            MPTVSeriesLog.Write("*******************          Completed          ***************************");
            MPTVSeriesLog.Write("***************************************************************************");
        }

        /// <summary>
        /// sets:
        ///  - DBEpisode.cImportProcessed to 1
        ///  - DBSeason.cHasLocalFilesTemp to true
        ///  - DBSeries.cHasLocalFilesTemp to true
        /// </summary>
        /// <param name="filenames"></param>
        void UpdateStatus(List<string> filenames)
        {
            if (filenames.Count == 0) return;
            SQLCondition cond = new SQLCondition();
            
            // for huge libraries a stringBuilder is much better than adding every filename seperatly to the condition (and for small ones it hardly matters)
            // sqllite expression tree can only be 1000 deep though, to be safe we do 500 at once and no more
            List<SQLCondition> importProcessedConds = new List<SQLCondition>();
            try
            {
                StringBuilder condBuilder = new StringBuilder();
                string field = DBEpisode.Q(DBEpisode.cFilename);
                int count = 0;
                foreach (string file in filenames)
                {
                    if (condBuilder.Length > 0)
                        condBuilder.Append(" or ");
                    condBuilder.Append(field).Append(" = '").Append(file.Replace("'", "''")).Append('\'');
                    if (count++ >= 500)
                    {
                        cond.AddCustom(condBuilder.ToString());
                        importProcessedConds.Add(cond);
                        cond = new SQLCondition();
                        condBuilder.Remove(0, condBuilder.Length);
                        count = 0;
                    }
                }
                if (count > 0)
                {
                    cond.AddCustom(condBuilder.ToString());
                    importProcessedConds.Add(cond);
                }
            }
            catch ( Exception ex)
            {
                MPTVSeriesLog.Write(ex.Message);
                return;
            }

            foreach(SQLCondition condition in importProcessedConds)
                DBEpisode.GlobalSet(DBEpisode.cImportProcessed, 1, condition);

            SQLCondition condSeason = new SQLCondition();
            condSeason.AddCustom(" exists( select " + DBEpisode.Q(DBEpisode.cFilename) + " from " + DBEpisode.cTableName
                            + " where " + DBEpisode.cSeriesID + " = " + DBSeason.Q(DBSeason.cSeriesID) + " and "
                            + DBEpisode.cSeasonIndex + " = " + DBSeason.Q(DBSeason.cIndex) + " and " + DBEpisode.Q(DBEpisode.cImportProcessed) + " = 1 "   + ")");
            DBSeason.GlobalSet(DBSeason.cHasLocalFilesTemp, true, condSeason);

            SQLCondition condSeries = new SQLCondition();
            condSeries.AddCustom(" exists( select " + DBEpisode.Q(DBEpisode.cFilename) + " from " + DBEpisode.cTableName
                            + " where " + DBEpisode.cSeriesID + " = " + DBOnlineSeries.Q(DBOnlineSeries.cID) +
                            " and " + DBEpisode.Q(DBEpisode.cImportProcessed) + " = 1 " + ")");
            DBSeries.GlobalSet(DBOnlineSeries.cHasLocalFilesTemp, true, condSeries);
        }

        void ParseLocal(List<PathPair> files)
        {
            List<parseResult> parsedFiles = LocalParse.Parse(files, false);
            
            // don't process those already in DB
            List<string> dbEps = new List<string>();
            SQLite.NET.SQLiteResultSet results = DBTVSeries.Execute("select episodefilename from local_episodes");
            if (results.Rows.Count > 0)
            {
                for (int index = 0; index < results.Rows.Count; index++)
                {
                    dbEps.Add(results.Rows[index].fields[0]);
                }
            }
            List<string> updateStatusEps = new List<string>();
            for (int i = 0; i < parsedFiles.Count; i++)
            {
                if (dbEps.Contains(parsedFiles[i].full_filename))
                {
                    updateStatusEps.Add(parsedFiles[i].full_filename);
                    parsedFiles.RemoveAt(i);
                    i--;
                }
            }

            UpdateStatus(updateStatusEps);
            MPTVSeriesLog.Write(parsedFiles.Count.ToString() + " found that sucessfully parsed and are not already in DB");
            int nSeason = 0;
            List<DBSeries> relatedSeries = new List<DBSeries>();
            List<DBSeason> relatedSeasons = new List<DBSeason>();

            foreach (parseResult progress in parsedFiles)
            {
                if (worker.CancellationPending)
                    return;
                if (progress.success)
                {
                    DBSeries series = null;
                    if (progress.parser.Matches.ContainsKey(DBOnlineEpisode.cFirstAired))
                    {
                        // series first
                        series = new DBSeries(progress.parser.Matches[DBSeries.cParsedName].ToLower());
                        series[DBOnlineSeries.cHasLocalFilesTemp] = 1;
                        // not much to do here except commiting the series
                        series.Commit();
                    }
                    else
                    {
                        nSeason = Convert.ToInt32(progress.parser.Matches[DBEpisode.cSeasonIndex]);

                        // ok, we are sure it's valid now
                        // series first
                        series = new DBSeries(progress.parser.Matches[DBSeries.cParsedName].ToLower());
                        series[DBOnlineSeries.cHasLocalFilesTemp] = 1;
                        // not much to do here except commiting the series
                        series.Commit();

                        // season now
                        DBSeason season = new DBSeason(series[DBSeries.cID], nSeason);
                        season[DBSeason.cHasLocalFilesTemp] = true;
                        season[DBSeason.cHasEpisodes] = true;
                        season.Commit();

                        // already in?
                        bool bSeasonFound = false;
                        foreach (DBSeason seasonLoop in relatedSeasons)
                            if (seasonLoop[DBSeason.cSeriesID] == series[DBOnlineSeries.cID] && seasonLoop[DBSeason.cIndex] == nSeason)
                            {
                                bSeasonFound = true;
                                break;
                            }
                        if (!bSeasonFound)
                            relatedSeasons.Add(season);

                        bool bSeriesFound = false;
                        foreach (DBSeries seriesLoop in relatedSeries)
                            if (seriesLoop[DBOnlineSeries.cID] == series[DBOnlineSeries.cID])
                            {
                                bSeriesFound = true;
                                break;
                            }
                        if (!bSeriesFound)
                            relatedSeries.Add(series);

                    }
                    
                    // then episode
                    DBEpisode episode = new DBEpisode(progress.full_filename);
                    bool bNewFile = false;
                    if (episode[DBEpisode.cImportProcessed] != 2)
                    {
                        m_bDataUpdated = true;
                        bNewFile = true;
                    }

                    episode[DBEpisode.cImportProcessed] = 1;
                    episode[DBEpisode.cSeriesID] = series[DBSeries.cID];
                    if (progress.parser.Matches.ContainsKey(DBEpisode.cEpisodeIndex2))
                    {
                        episode[DBEpisode.cEpisodeIndex2] = progress.parser.Matches[DBEpisode.cEpisodeIndex2];
                        episode[DBEpisode.cCompositeID2] = episode[DBEpisode.cSeriesID] + "_" + nSeason + "x" + episode[DBEpisode.cEpisodeIndex2];
                    }

                    episode[DBEpisode.cAvailableSubtitles] = episode.checkHasSubtitles();
                    
                    foreach (KeyValuePair<string, string> match in progress.parser.Matches)
                    {
                        if (match.Key != DBSeries.cParsedName)
                        {
                            episode.AddColumn(match.Key, new DBField(DBField.cTypeString));
                            if (bNewFile || (episode[match.Key] != null && episode[match.Key] != match.Value))
                                episode[match.Key] = match.Value;
                        }
                    }
                    episode.Commit();
                }
            }

            // now go over the touched seasons & series
            foreach (DBSeason season in relatedSeasons)
            {
                DBEpisode episode = DBEpisode.GetFirstUnwatched(season[DBSeason.cSeriesID], season[DBSeason.cIndex]);
                if (episode != null)
                    season[DBSeason.cUnwatchedItems] = true;
                else
                    season[DBSeason.cUnwatchedItems] = false;

                season.Commit();
            }

            foreach (DBSeries series in relatedSeries)
            {
                DBEpisode episode = DBEpisode.GetFirstUnwatched(series[DBSeries.cID]);
                if (episode != null)
                    series[DBOnlineSeries.cUnwatchedItems] = true;
                else
                    series[DBOnlineSeries.cUnwatchedItems] = false;

                series.Commit();
            }
        }

        void GetSeries()
        {
            MPTVSeriesLog.Write(bigLogMessage("Identifying Unknown Series Online"));

            SQLCondition condition = null;
            if (m_params.m_bUpdateScan)
            {
                // mark existing online data as "old", needs a refresh
                condition = new SQLCondition();
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cOnlineDataImported, 2, SQLConditionType.Equal);
                DBTable.GlobalSet(new DBOnlineSeries(), DBOnlineSeries.cOnlineDataImported, 1, condition);

                // mark existing banners as "old", needs a refresh too
                condition = new SQLCondition();
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cBannersDownloaded, 2, SQLConditionType.Equal);
                DBTable.GlobalSet(new DBOnlineSeries(), DBOnlineSeries.cBannersDownloaded, 1, condition);
            }

            condition = new SQLCondition();
            // all series that don't have an onlineID ( < 0) and not marked as ignored
            condition.Add(new DBSeries(), DBSeries.cID, 0, SQLConditionType.LessThan);
            condition.Add(new DBSeries(), DBSeries.cScanIgnore, 0, SQLConditionType.Equal);

            int nIndex = 0;
            List<DBSeries> seriesList = DBSeries.Get(condition, false, false);
            if(seriesList.Count > 0)
                MPTVSeriesLog.Write(string.Format("Found {0} Unknown Series, attempting to identify them now...", seriesList.Count));
            else
                MPTVSeriesLog.Write("All Series are already identified.");

            foreach (DBSeries series in seriesList) {
                if (worker.CancellationPending)
                    return;

                worker.ReportProgress(10 + (10 * nIndex / seriesList.Count));
                nIndex++;

                String sSeriesNameToSearch = series[DBSeries.cParsedName];
                DBOnlineSeries UserChosenSeries = SearchForSeries(sSeriesNameToSearch);

                if (UserChosenSeries != null) // make sure selection was not cancelled
                    {
                    // set the ID on the current series with the one from the chosen one
                    // we need to update all depending items - seasons & episodes
                    List<DBSeason> seasons = DBSeason.Get(series[DBSeries.cID]);
                    foreach (DBSeason season in seasons)
                        season.ChangeSeriesID(UserChosenSeries[DBSeries.cID]);

                    SQLCondition setcondition = new SQLCondition();
                    setcondition.Add(new DBSeason(), DBSeason.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                    DBSeason.Clear(setcondition);

                    setcondition = new SQLCondition();
                    setcondition.Add(new DBEpisode(), DBEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                    List<DBEpisode> episodes = DBEpisode.Get(setcondition, false);
                    foreach (DBEpisode episode in episodes)
                        episode.ChangeSeriesID(UserChosenSeries[DBSeries.cID]);

                    setcondition = new SQLCondition();
                    setcondition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                    DBOnlineEpisode.Clear(setcondition);

                    int nSeriesID = series[DBSeries.cID];

                    // Support different Episode orders (dvd/aired/absolute, etc)
                    try {
                        series[DBOnlineSeries.cEpisodeOrders] = UserChosenSeries[DBOnlineSeries.cEpisodeOrders];
                        List<string> episodeOrders = new List<string>(UserChosenSeries[DBOnlineSeries.cEpisodeOrders].ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
                        if (episodeOrders.Count > 1) {
                            MPTVSeriesLog.Write(string.Format("\"{0}\" supports {1} different ordering options, asking user...", UserChosenSeries.ToString(), episodeOrders.Count));
                            // let the user choose
                            string helpText = "Some series expose several ways in which they are ordered, for instance a DVD-release may differ from the original Air schedule." + Environment.NewLine +
                                              "Note that your file numbering must match the option you choose here." + Environment.NewLine +
                                              "Choose the default \"Aired\" option unless you have a specific reason not to!";

                            List<Feedback.CItem> Choices = new List<Feedback.CItem>();
                            foreach (string orderOption in episodeOrders)
                                Choices.Add(new Feedback.CItem(orderOption, helpText, orderOption));

                            Feedback.ChooseFromSelectionDescriptor descriptor = new Feedback.ChooseFromSelectionDescriptor();
                            descriptor.m_sTitle = "Multiple ordering Options detected";
                            descriptor.m_sItemToMatchLabel = "The following Series supports multiple Order Options:";
                            descriptor.m_sItemToMatch = series[DBOnlineSeries.cPrettyName];
                            descriptor.m_sListLabel = "Please choose the desired Option:";
                            descriptor.m_List = Choices;
                            descriptor.m_useRadioToSelect = true;
                            descriptor.m_allowAlter = false;

                            Feedback.CItem selectedOrdering = null;
                            Feedback.ReturnCode result = m_feedback.ChooseFromSelection(descriptor, out selectedOrdering);
                            if (result == WindowPlugins.GUITVSeries.Feedback.ReturnCode.OK) {
                                series[DBOnlineSeries.cChoseEpisodeOrder] = (string)selectedOrdering.m_Tag;
                            }
                        }

                    }
                    catch (Exception) {

                    }
                    // End support for ordering

                    series.ChangeSeriesID(UserChosenSeries[DBSeries.cID]);
                    series.Commit();

                    setcondition = new SQLCondition();
                    setcondition.Add(new DBOnlineSeries(), DBOnlineSeries.cID, nSeriesID, SQLConditionType.Equal);
                    DBOnlineSeries.Clear(setcondition);

                    // only keep one local dbseries marked as non dupe
                    setcondition = new SQLCondition();
                    setcondition.Add(new DBOnlineSeries(), DBOnlineSeries.cID, UserChosenSeries[DBSeries.cID], SQLConditionType.Equal);
                    List<DBSeries> seriesDupeSetList = DBSeries.Get(setcondition);
                    bool bFirst = true;
                    foreach (DBSeries seriesDupeSet in seriesDupeSetList) {
                        if (bFirst) {
                            seriesDupeSet[DBSeries.cDuplicateLocalName] = 0;
                            seriesDupeSet.Commit();
                            bFirst = false;
                        } else {
                            seriesDupeSet[DBSeries.cDuplicateLocalName] = 1;
                            seriesDupeSet.Commit();
                        }
                    }
                }
                
            }
        }

        public DBOnlineSeries SearchForSeries(string seriesName) {
            string nameToSearch = seriesName;
            
            while (true) {
                // query online db for possible matches
                GetSeries GetSeriesParser = new GetSeries(nameToSearch);

                // try to find an exact match in our results, if found, return
                if (DBOption.GetOptions(DBOption.cAutoChooseSeries) == 1)
                {
                    foreach (DBOnlineSeries onlineSeries in GetSeriesParser.Results)
                    {
                        if (!Helper.String.IsNullOrEmpty(onlineSeries[DBOnlineSeries.cStatus]) &&
                           (onlineSeries[DBOnlineSeries.cPrettyName].ToString().Trim().Equals(nameToSearch.Trim().ToLower(), StringComparison.InvariantCultureIgnoreCase) || onlineSeries["SortName"].ToString().Trim().Equals(nameToSearch.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                        {
                            MPTVSeriesLog.Write(string.Format("\"{0}\" was automatically matched to \"{1}\" (SeriesID: {2}), there were a total of {3} matches returned from the Online Database", nameToSearch, onlineSeries.ToString(), onlineSeries[DBOnlineSeries.cID], GetSeriesParser.Results.Count));
                            return onlineSeries;
                        }
                    }
                }
                MPTVSeriesLog.Write(string.Format("Found {0} possible matches for \"{1}\"", GetSeriesParser.Results.Count, nameToSearch));

                // User has three choices:
                // 1) Pick a series from the list
                // 2) Simply skip
                // 3) Skip and never ask for this series again

                List<Feedback.CItem> Choices = new List<Feedback.CItem>();
                foreach (DBOnlineSeries onlineSeries in GetSeriesParser.Results)
                    Choices.Add(new Feedback.CItem(onlineSeries[DBOnlineSeries.cPrettyName], "First Aired: " + onlineSeries["FirstAired"] + "\r\nOverview:\r\n" + onlineSeries[DBOnlineSeries.cSummary], onlineSeries));

                if (Choices.Count == 0)
                    Choices.Add(new Feedback.CItem("No Match Found, try to enter another name for the show", String.Empty, null));

                Feedback.ChooseFromSelectionDescriptor descriptor = new Feedback.ChooseFromSelectionDescriptor();
                descriptor.m_sTitle = "Unable to find matching series";
                descriptor.m_sItemToMatchLabel = "Local series:";
                descriptor.m_sItemToMatch = nameToSearch;
                descriptor.m_sListLabel = "Choose the correct series from this list:";
                descriptor.m_List = Choices;
                descriptor.m_sbtnCancelLabel = "Skip this time";
                descriptor.m_sbtnIgnoreLabel = "Skip/Never ask again";

                bool bKeepTrying = true;
                while (bKeepTrying)
                {
                    Feedback.CItem Selected = null;
                    Feedback.ReturnCode result = m_feedback.ChooseFromSelection(descriptor, out Selected);
                    switch (result)
                    {
                        case Feedback.ReturnCode.Cancel:
                            MPTVSeriesLog.Write("User cancelled Series Selection");
                            return null;

                        case Feedback.ReturnCode.Ignore:
                            MPTVSeriesLog.Write("User chose to Ignore \"" + nameToSearch + "\" in the future");
                            nameToSearch = null;
                            DBSeries series = new DBSeries(seriesName);
                            series[DBSeries.cScanIgnore] = 1; // means it will be skipped in the future
                            series[DBSeries.cHidden] = true;
                            series.Commit();
                            return null;

                        case Feedback.ReturnCode.OK:
                            DBOnlineSeries selectedSeries = Selected.m_Tag as DBOnlineSeries;
                            if (nameToSearch != Selected.m_sName || selectedSeries == null)
                            {
                                nameToSearch = Selected.m_sName;
                                bKeepTrying = false;
                            }
                            else
                            {
                                MPTVSeriesLog.Write(string.Format("User matched \"{0}\" to \"{1}\" (SeriesID: {2})", nameToSearch, selectedSeries.ToString(), selectedSeries[DBOnlineSeries.cID]));
                                return selectedSeries;
                            }
                            break;

                        case Feedback.ReturnCode.NotReady:
                            {
                                // plugin's not loaded (yet?) so wait and ask again later
                                Thread.Sleep(2000);
                            }
                            break;
                    }
                }
                if (!bKeepTrying) MPTVSeriesLog.Write("User typed a new Search term: \"" + nameToSearch + "\"");
            }
        }

        void GetEpisodes()
        {
            long nGetEpisodesTimeStamp = 0;

            MPTVSeriesLog.Write(bigLogMessage("Identifying unknown Episodes Online"));
            SQLCondition condition = null;
            if (m_params.m_bUpdateScan)
            {
                // mark existing online data as "old", needs a refresh
                condition = new SQLCondition();
                condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cOnlineDataImported, 2, SQLConditionType.Equal);
                DBEpisode.GlobalSet(DBOnlineEpisode.cOnlineDataImported, 1, condition);
            }

            condition = new SQLCondition();
            // all series that have an onlineID ( != 0)
            condition.Add(new DBSeries(), DBSeries.cID, 0, SQLConditionType.GreaterThan);
            condition.Add(new DBSeries(), DBSeries.cScanIgnore, 0, SQLConditionType.Equal);
            condition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);

            List<DBSeries> seriesList = DBSeries.Get(condition, false, false);
            int nIndex = 0;

            if (seriesList.Count == 0)
                MPTVSeriesLog.Write("All Episodes are already identified");

            if (m_bFullSeriesRetrieval && m_params.m_bUpdateScan)
                MPTVSeriesLog.Write("Mode: Get all Episodes of Series");

            foreach (DBSeries series in seriesList)
            {
                if (worker.CancellationPending)
                    return;

                worker.ReportProgress(20 + (20 * nIndex / seriesList.Count));
                nIndex++;

                if (m_bFullSeriesRetrieval && m_params.m_bUpdateScan)
                {
                    MPTVSeriesLog.Write(string.Format("Looking for all the Episodes of \"{0}\"", series.ToString()));
                    SQLCondition conditions = new SQLCondition();
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cID, 0, SQLConditionType.Equal);
                    List<DBEpisode> episodesList = DBEpisode.Get(conditions, false);
                    // if we have unidentified episodes, let's retrieve the full list
                    if (episodesList.Count > 0)
                        nGetEpisodesTimeStamp = 0;
                    else
                        nGetEpisodesTimeStamp = series[DBOnlineSeries.cGetEpisodesTimeStamp];

                    GetEpisodes episodesParser = new GetEpisodes(series[DBSeries.cID], nGetEpisodesTimeStamp);
                    if (episodesParser.Results.Count > 0)
                    {
                        MPTVSeriesLog.Write(string.Format("Found {0} Episodes for \"{1}\"", episodesParser.Results.Count, series.ToString()));
                        // add all online episodes in the local db
                        foreach (DBOnlineEpisode onlineEpisode in episodesParser.Results)
                        {
                            // season if not there yet
                            DBSeason season = new DBSeason(series[DBSeries.cID], onlineEpisode[DBOnlineEpisode.cSeasonIndex]);
                            season[DBSeason.cHasEpisodes] = true;
                            DBSeason existing = DBSeason.getRaw(series[DBSeries.cID], onlineEpisode[DBOnlineEpisode.cSeasonIndex]);
                            if (existing != null)
                            {
                                season[DBSeason.cHasLocalFiles] = existing[DBSeason.cHasLocalFilesTemp];
                                season[DBSeason.cHasLocalFilesTemp] = existing[DBSeason.cHasLocalFilesTemp];
                            }
                            season.Commit();

                            DBOnlineEpisode newOnlineEpisode = new DBOnlineEpisode(series[DBSeries.cID], onlineEpisode[DBOnlineEpisode.cSeasonIndex], onlineEpisode[DBOnlineEpisode.cEpisodeIndex]);
                            newOnlineEpisode[DBOnlineEpisode.cID] = onlineEpisode[DBOnlineEpisode.cID];
                            if (newOnlineEpisode[DBOnlineEpisode.cEpisodeName].ToString().Length == 0)
                                newOnlineEpisode[DBOnlineEpisode.cEpisodeName] = onlineEpisode[DBOnlineEpisode.cEpisodeName];
                            newOnlineEpisode.Commit();
                        }
                    }

                    if (episodesList.Count == 0)
                    {
                        series[DBOnlineSeries.cGetEpisodesTimeStamp] = episodesParser.ServerTimeStamp;
                        series.Commit();
                    }
                }
                else
                {
                    // if just retrieving info for existing files, for each series we have an ID of, build the list of episodes without ids;
                    // if there are less than 5 episodes in the list, do them individually (saves server bandwidth)))
                    // otherwise retrieve the full list
                    SQLCondition conditions = new SQLCondition();
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cID, 0, SQLConditionType.Equal);
                    List<DBEpisode> episodesList = DBEpisode.Get(conditions, false);
                    if (episodesList.Count < 5)
                    {
                        if(episodesList.Count > 0)
                            MPTVSeriesLog.Write(string.Format("\"{0}\" has few ({1}) unidentified Episodes, doing them individually", series.ToString(), episodesList.Count));
                        foreach (DBEpisode episode in episodesList)
                        {

                            MPTVSeriesLog.Write("Looking for the single episode \"" + episode.ToString() + "\"");
                            GetEpisodes episodesParser = null;
                            if (episode[DBEpisode.cSeasonIndex] >= 0 && episode[DBEpisode.cEpisodeIndex] >= 0)
                            {
                                episodesParser = new GetEpisodes(series[DBSeries.cID], episode[DBEpisode.cSeasonIndex], episode[DBEpisode.cEpisodeIndex]);
                                if (episode[DBEpisode.cEpisodeIndex2] > 0)
                                {
                                    MPTVSeriesLog.Write("Episode is a double Episode, now looking for the second Episode");
                                    episodesParser.Work(series[DBSeries.cID], episode[DBEpisode.cSeasonIndex], episode[DBEpisode.cEpisodeIndex2], 0, default(DateTime));
                                }
                            }
                            else
                                episodesParser = new GetEpisodes(series[DBSeries.cID], DateTime.Parse(episode[DBOnlineEpisode.cFirstAired]));

                            if (episodesParser.Results.Count > 0)
                            {

                                DBOnlineEpisode onlineEpisode = episodesParser.Results[0];
                                // season update for online data
                                DBSeason season = new DBSeason(series[DBSeries.cID], onlineEpisode[DBOnlineEpisode.cSeasonIndex]);
                                season[DBSeason.cHasEpisodes] = true;
                                DBSeason existing = DBSeason.getRaw(series[DBSeries.cID], onlineEpisode[DBOnlineEpisode.cSeasonIndex]);
                                if (existing != null)
                                {
                                    season[DBSeason.cHasLocalFiles] = existing[DBSeason.cHasLocalFilesTemp];
                                    season[DBSeason.cHasLocalFilesTemp] = existing[DBSeason.cHasLocalFilesTemp];
                                }
                                season.Commit();

                                // ugly cleanup of onlineepisodes in case it was matched by firstaired
                                // this should really be handled differently
                                if (episode[DBEpisode.cSeasonIndex] < 0 && episode[DBEpisode.cEpisodeIndex] < 0)
                                {
                                    SQLCondition cleanup = new SQLCondition();

                                    cleanup.Add(new DBOnlineEpisode(), DBOnlineEpisode.cCompositeID, episode[DBOnlineEpisode.cFirstAired], SQLConditionType.Like);
                                }

                                // end cleanup

                                episode[DBEpisode.cSeasonIndex] = onlineEpisode[DBOnlineEpisode.cSeasonIndex];
                                episode[DBEpisode.cEpisodeIndex] = onlineEpisode[DBOnlineEpisode.cEpisodeIndex];
                                episode.onlineEpisode[DBOnlineEpisode.cEpisodeIndex] = onlineEpisode[DBOnlineEpisode.cEpisodeIndex];
                                episode.onlineEpisode[DBOnlineEpisode.cSeasonIndex] = onlineEpisode[DBOnlineEpisode.cSeasonIndex];
                                episode[DBOnlineEpisode.cID] = onlineEpisode[DBOnlineEpisode.cID];
                                if (episode[DBOnlineEpisode.cEpisodeName].ToString().Length == 0)
                                    episode[DBOnlineEpisode.cEpisodeName] = onlineEpisode[DBOnlineEpisode.cEpisodeName];

                                MPTVSeriesLog.Write(string.Format("\"{0}\" identified", episode.ToString()));
                                episode.Commit();

                                if (episodesParser.Results.Count > 1)
                                {
                                    // we matched a double episode, add it too
                                    DBOnlineEpisode newOnlineEpisode = new DBOnlineEpisode(series[DBSeries.cID], episodesParser.Results[1][DBOnlineEpisode.cSeasonIndex], episodesParser.Results[1][DBOnlineEpisode.cEpisodeIndex]);
                                    newOnlineEpisode[DBOnlineEpisode.cID] = episodesParser.Results[1][DBOnlineEpisode.cID];
                                    if (newOnlineEpisode[DBOnlineEpisode.cEpisodeName].ToString().Length == 0)
                                        newOnlineEpisode[DBOnlineEpisode.cEpisodeName] = onlineEpisode[DBOnlineEpisode.cEpisodeName];
                                    MPTVSeriesLog.Write("Second Episode also identified...");
                                    newOnlineEpisode.Commit();
                                }

                            }
                            else MPTVSeriesLog.Write("This episode could not be identified (Possible reasons: Wrong filename/OnlineDatabase does not have these episodes)");
                        }
                    }
                    else
                    {
                        MPTVSeriesLog.Write(string.Format("\"{0}\" has many ({1}) unidentified Episodes, using batch mode", series.ToString(), episodesList.Count));
                        // no need to do single matches for many episodes, it's more efficient to do it all at once
                        GetEpisodes episodesParser = new GetEpisodes(series[DBSeries.cID], 0);
                        if (episodesParser.Results.Count > 0)
                        {
                            MPTVSeriesLog.Write(string.Format("Found {0} Episodes for \"{1}\", matching them up with local Episodes now", episodesParser.Results.Count, series.ToString()));
                            // look for the episodes for that series, and compare / update the values
                            foreach (DBOnlineEpisode onlineEpisode in episodesParser.Results)
                            {
                                foreach (DBEpisode localEpisode in episodesList)
                                {
                                    if (((int)localEpisode[DBEpisode.cSeasonIndex] == (int)onlineEpisode[DBOnlineEpisode.cSeasonIndex] &&
                                        (int)localEpisode[DBEpisode.cEpisodeIndex] == (int)onlineEpisode[DBOnlineEpisode.cEpisodeIndex]) ||
                                        ((int)localEpisode[DBEpisode.cSeasonIndex] == (int)onlineEpisode[DBOnlineEpisode.cSeasonIndex] &&
                                        (int)localEpisode[DBEpisode.cEpisodeIndex2] == (int)onlineEpisode[DBOnlineEpisode.cEpisodeIndex]))
                                    {
                                        // season update for online data
                                        DBSeason season = new DBSeason(series[DBSeries.cID], onlineEpisode[DBOnlineEpisode.cSeasonIndex]);
                                        season[DBSeason.cHasEpisodes] = true;
                                        season.Commit();

                                        // update data
                                        localEpisode[DBOnlineEpisode.cID] = onlineEpisode[DBOnlineEpisode.cID];
                                        if (localEpisode[DBOnlineEpisode.cEpisodeName].ToString().Length == 0)
                                            localEpisode[DBOnlineEpisode.cEpisodeName] = onlineEpisode[DBOnlineEpisode.cEpisodeName];
                                        MPTVSeriesLog.Write("\"" + localEpisode.ToString() + "\" identified");
                                        localEpisode.Commit();
                                        // remove the localEpisode from the local list (we found it, it's updated, it's faster this way)
                                        episodesList.Remove(localEpisode);
                                        break;
                                    }
                                }
                            }
                        }
                        else MPTVSeriesLog.Write("None of these episodes could be identified (Possible reasons: Wrong filename/OnlineDatabase does not have these episodes)");
                    }
                }
            }
        }

        public void UpdateSeries(bool bUpdateNewSeries)
        {
            long nUpdateSeriesTimeStamp = 0;
            // now retrieve the info about the series
            SQLCondition condition = new SQLCondition();
            // all series that have an onlineID ( != 0)
            condition.Add(new DBOnlineSeries(), DBOnlineSeries.cID, 0, SQLConditionType.GreaterThan);
            condition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);
            if (bUpdateNewSeries)
            {
                MPTVSeriesLog.Write(bigLogMessage("Retrieving Metadata for new Series"));
                // and that never had data imported from the online DB
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cOnlineDataImported, 0, SQLConditionType.Equal);
                // in that case, don't use the lasttime of import
                nUpdateSeriesTimeStamp = 0;
            }
            else
            {
                MPTVSeriesLog.Write(bigLogMessage("Updating Metadata for existing Series"));
                // and that already had data imported from the online DB (but not the new ones, that are set to 2)
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cOnlineDataImported, 1, SQLConditionType.Equal);
                nUpdateSeriesTimeStamp = (long)DBOption.GetOptions(DBOption.cUpdateSeriesTimeStamp);
            }
            List<DBSeries> SeriesList = DBSeries.Get(condition, false, false);

            if (SeriesList.Count > 0)
            {
                // generate a comma separated list of all the series ID
                String sSeriesIDs = generateIDList(SeriesList, DBSeries.cID);

                // use the last known timestamp from when we updated the series
                MPTVSeriesLog.Write(string.Format("{0} metadata of {1} Series", (bUpdateNewSeries ? "Retrieving" : "Looking for updated"), SeriesList.Count));
                UpdateSeries UpdateSeriesParser = new UpdateSeries(sSeriesIDs, nUpdateSeriesTimeStamp);
                if (UpdateSeriesParser.Results.Count == 0)
                    MPTVSeriesLog.Write(string.Format("No {0} found", (bUpdateNewSeries ? "metadata" : "updates")));

                foreach (DBOnlineSeries updatedSeries in UpdateSeriesParser.Results)
                {
                    m_bDataUpdated = true;
                    if (worker.CancellationPending)
                        return;

                    MPTVSeriesLog.Write(string.Format("Metadata {0} for \"{1}\"", (bUpdateNewSeries ? "retrieved" : "updated"), updatedSeries.ToString()));
                    // find the corresponding series in our list
                    foreach (DBSeries localSeries in SeriesList)
                    {
                        if (localSeries[DBSeries.cID] == updatedSeries[DBSeries.cID])
                        {
                            // go over all the fields, (and update only those which haven't been modified by the user - will do that later)
                            foreach (String key in updatedSeries.FieldNames)
                            {
                                switch (key)
                                {
                                    // do not overwrite current series local settings with the one from the online series (baaaad design??)
                                    case DBSeries.cParsedName: // this field shouldn't be required here since updatedSeries is an Onlineseries and not a localseries??
                                    case DBOnlineSeries.cHasLocalFiles:
                                    case DBOnlineSeries.cHasLocalFilesTemp:
                                    case DBOnlineSeries.cIsFavourite:
                                    case DBOnlineSeries.cEpisodeOrders:
                                    case DBOnlineSeries.cChoseEpisodeOrder:

                                    case DBOnlineSeries.cBannerFileNames: // banners get handled differently (later on)
                                    case DBOnlineSeries.cCurrentBannerFileName:
                                        break;

                                    default:
                                        localSeries.AddColumn(key, new DBField(DBField.cTypeString));
                                        localSeries[key] = updatedSeries[key];
                                        break;
                                }
                            }
                            // data import completed; set to 2 (data up to date)
                            localSeries[DBOnlineSeries.cOnlineDataImported] = 2;

                            if(localSeries[DBOnlineSeries.cHasLocalFilesTemp])
                                localSeries[DBOnlineSeries.cHasLocalFiles] = 1;
                            localSeries.Commit();
                            //                        SeriesList.Remove(localSeries);
                        }
                    }
                }

                // now process incorrect IDs if any
                foreach (int nIncorrectID in UpdateSeriesParser.BadIds)
                {
                    m_bDataUpdated = true;
                    if (worker.CancellationPending)
                        return;

                    m_bReparseNeeded = true;
                    // find the corresponding series in our list
                    foreach (DBSeries localSeries in SeriesList)
                    {
                        if (localSeries[DBSeries.cID] == nIncorrectID)
                        {
                            MPTVSeriesLog.Write("Incorrect SeriesID found! ID=" + nIncorrectID + " for local series '" + localSeries[DBSeries.cParsedName] + "'");
                            m_bDataUpdated = true;
                            // reset the seriesID of this series
                            localSeries[DBSeries.cID] = 0;
                            localSeries[DBOnlineSeries.cOnlineDataImported] = 0;
                            localSeries.Commit();
                            //                        SeriesList.Remove(localSeries);
                        }
                    }
                }

                // update timestamp with the last one we know about
                if (!bUpdateNewSeries)
                    DBOption.SetOptions(DBOption.cUpdateSeriesTimeStamp, UpdateSeriesParser.ServerTimeStamp);
            }
            else MPTVSeriesLog.Write("Nothing to do...");
        }

        public void UpdateEpisodeThumbNails()
        {
            if (DBOption.GetOptions(DBOption.cGetEpisodeSnapshots) == true)
            {
                MPTVSeriesLog.Write(bigLogMessage("Checking for EpisodeThumbnails..."));
                // get a list of all the episodes with thumbnailUrl
                SQLCondition condition = new SQLCondition();
                condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cEpisodeThumbnailUrl, "", SQLConditionType.NotEqual);
                condition.AddOrderItem(DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID), SQLCondition.orderType.Ascending);
                List<DBEpisode> episodes = DBEpisode.Get(condition);
                DBSeries tmpSeries = null; 
                foreach (DBEpisode episode in episodes)
                {
                    String sThumbNailFilename = episode[DBOnlineEpisode.cEpisodeThumbnailFilename];
                    string basePath = Settings.GetPath(Settings.Path.banners);
                    string completePath = Helper.PathCombine(basePath, sThumbNailFilename);
                    if (!File.Exists(completePath))
                    {
                        // we need the pretty name to figure out the folder to store to
                        try
                        {
                            if (null == tmpSeries || tmpSeries[DBSeries.cID] != episode[DBEpisode.cSeriesID])
                            {
                                tmpSeries = DBSeries.Get(episode[DBOnlineEpisode.cSeriesID], false);
                            }
                            string seriesFolder = tmpSeries[DBOnlineSeries.cPrettyName];
                            foreach (char c in System.IO.Path.GetInvalidFileNameChars()) seriesFolder = seriesFolder.Replace(c, '_');
                            sThumbNailFilename = Helper.PathCombine(seriesFolder, @"Episodes\" + episode[DBOnlineEpisode.cSeasonIndex] + "x" + episode[DBOnlineEpisode.cEpisodeIndex] + ".jpg");
                            completePath = Helper.PathCombine(basePath, sThumbNailFilename);

                            if (!File.Exists(completePath))
                            {
                                DBSeries s = Helper.getCorrespondingSeries(episode[DBOnlineEpisode.cSeriesID]);
                                MPTVSeriesLog.Write(string.Format("New EpisodeImage found for \"{0}\": {1}", episode.ToString(), episode[DBOnlineEpisode.cEpisodeThumbnailUrl]));
                                System.Net.WebClient webClient = new System.Net.WebClient();
                                webClient.Headers.Add("user-agent", Settings.UserAgent);
                                try
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(completePath));
                                    webClient.DownloadFile(DBOnlineMirror.Banners + "/" + episode[DBOnlineEpisode.cEpisodeThumbnailUrl], completePath);
                                }
                                catch (System.Net.WebException)
                                {
                                    MPTVSeriesLog.Write("Banner download failed (" + episode[DBOnlineEpisode.cEpisodeThumbnailUrl] + ")");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MPTVSeriesLog.Write("There was a problem getting the episode image: " + ex.Message);
                        }
                        episode[DBOnlineEpisode.cEpisodeThumbnailFilename] = sThumbNailFilename;
                        episode.Commit();
                    }
                }
            }

        }

        public void UpdateBanners(bool bUpdateNewSeries)
        {
            SQLCondition condition = new SQLCondition();
            // all series that have an onlineID ( > 0)
            condition.Add(new DBOnlineSeries(), DBSeries.cID, 0, SQLConditionType.GreaterThan);
            condition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);
            if (bUpdateNewSeries)
            {
                MPTVSeriesLog.Write(bigLogMessage("Checking for banners for series without any banners"));
                // and that never had data imported from the online DB
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cBannersDownloaded, 0, SQLConditionType.Equal);
            }
            else
            {
                MPTVSeriesLog.Write(bigLogMessage("Checking for new banners"));                
                // and that already had data imported from the online DB
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cBannersDownloaded, 1, SQLConditionType.Equal);
            }

            List<DBSeries> seriesList = DBSeries.Get(condition, false, false);
            int nIndex = 0;
            if (seriesList.Count == 0)
            {
                if (bUpdateNewSeries) MPTVSeriesLog.Write("All Series appear to have banners already");
                else MPTVSeriesLog.Write("Nothing to do");
            }
            else MPTVSeriesLog.Write("Looking for banners on " + seriesList.Count + " Series");

            // TODO: new way to make only one webrequest
            //String sSeriesIDs = generateIDList(seriesList, DBSeries.cID);
            //GetBanner bannerParser = new GetBanner(sSeriesIDs, bUpdateNewSeries ? 0 : (long)DBOption.GetOptions(DBOption.cUpdateBannersTimeStamp));
            // end new way:

            foreach (DBSeries series in seriesList)
            {
                if (worker.CancellationPending)
                    return;

                worker.ReportProgress(50 + (bUpdateNewSeries ? 0 : 10) + (10 * nIndex / seriesList.Count));
                nIndex++;

                // disable for new way
                if (bUpdateNewSeries)
                    MPTVSeriesLog.Write("Downloading banners for \"" + series.ToString() + "\"");
                else
                    MPTVSeriesLog.Write("Refreshing banners for \"" + series.ToString() +"\"");

                // check if the files are still there - if not, redownload
                bool bMissingBanners = false;
                foreach (String filename in series.BannerList)
                {
                    if (!System.IO.File.Exists(filename)) 
                    {
                        bMissingBanners = true;
                        break;
                    }
                }

                GetBanner bannerParser = new GetBanner((int)series[DBSeries.cID], (bUpdateNewSeries | bMissingBanners) ? 0 : (long)series[DBOnlineSeries.cUpdateBannersTimeStamp], series[DBOnlineSeries.cPrettyName]);
                // end disable for new way

                String sLastTextBanner = String.Empty;
                String sLastGraphicalBanner = String.Empty;

                seriesBannersMap seriesBanners = Helper.getElementFromList<seriesBannersMap, string>(series[DBSeries.cID], "seriesID", 0, bannerParser.seriesBanners);
                if (seriesBanners != null)  // oops!
                {
                    bool hasOwnLang = false;
                    foreach (BannerSeries bannerSeries in seriesBanners.seriesBanners)
                    {
                        if (series[DBOnlineSeries.cBannerFileNames].ToString().IndexOf(bannerSeries.sBannerFileName) == -1)
                        {
                            m_bDataUpdated = true;
                            MPTVSeriesLog.Write("New banner found for \"" + series.ToString() + "\" : " + bannerSeries.sOnlineBannerPath);
                            if (series[DBOnlineSeries.cBannerFileNames].ToString().Trim().Length == 0)
                            {
                                series[DBOnlineSeries.cBannerFileNames] += bannerSeries.sBannerFileName;
                            }
                            else
                            {
                                series[DBOnlineSeries.cBannerFileNames] += "|" + bannerSeries.sBannerFileName;
                            }
                        }
                        // prefer graphical
                        if (bannerSeries.sBannerLang == ZsoriParser.SelLanguageAsString)
                        {
                            if (bannerSeries.bGraphical)
                                sLastGraphicalBanner = bannerSeries.sBannerFileName;
                            else
                                sLastTextBanner = bannerSeries.sBannerFileName;
                            hasOwnLang = true;
                        }
                        else if(!hasOwnLang)
                        {
                            if (bannerSeries.bGraphical)
                                sLastGraphicalBanner = bannerSeries.sBannerFileName;
                            else
                                sLastTextBanner = bannerSeries.sBannerFileName;
                        }
                    }

                    if (series[DBOnlineSeries.cCurrentBannerFileName].ToString().Trim().Length == 0)
                    {
                        // use the last banner as the current one (if any graphical found)
                        // otherwise use the first available
                        if (sLastGraphicalBanner.Length > 0)
                            series[DBOnlineSeries.cCurrentBannerFileName] = sLastGraphicalBanner;
                        else
                            series[DBOnlineSeries.cCurrentBannerFileName] = sLastTextBanner;
                    }

                    series[DBOnlineSeries.cBannersDownloaded] = 2;
                    series.Commit();
                    string lastSeasonBanner = string.Empty;
                    hasOwnLang = false;
                    foreach (BannerSeason bannerSeason in seriesBanners.seasonBanners)
                    {
                        DBSeason season = new DBSeason(series[DBSeries.cID], bannerSeason.nIndex);
                        if (season[DBSeason.cBannerFileNames].ToString().IndexOf(bannerSeason.sBannerFileName) == -1)
                        {
                            m_bDataUpdated = true;
                            if (season[DBSeason.cBannerFileNames].ToString().Length == 0)
                            {
                                season[DBSeason.cBannerFileNames] += bannerSeason.sBannerFileName;
                            }
                            else
                            {
                                season[DBSeason.cBannerFileNames] += "|" + bannerSeason.sBannerFileName;
                                MPTVSeriesLog.Write("New banner found for \"" + series.ToString() + "\"Season " + season[DBSeason.cIndex] + ": " + bannerSeason.sOnlineBannerPath);
                            }
                        }
     
                        if (bannerSeason.sBannerLang == ZsoriParser.SelLanguageAsString)
                        {
                            lastSeasonBanner = bannerSeason.sBannerFileName;
                            hasOwnLang = true;
                        }
                        else if(!hasOwnLang)
                        {
                            lastSeasonBanner = bannerSeason.sBannerFileName;
                        }
                        // use the last banner as the current one
                        if (season[DBSeason.cCurrentBannerFileName].ToString().Trim().Length == 0)
                            season[DBSeason.cCurrentBannerFileName] = lastSeasonBanner;
                        season.Commit();
                    }
                    
                    if (!bUpdateNewSeries)
                    {
                        // disable for new way
                        series[DBOnlineSeries.cUpdateBannersTimeStamp] = bannerParser.ServerTimeStamp;
                        series.Commit();
                    }
                    if (!bUpdateNewSeries) DBOption.SetOptions(DBOption.cUpdateBannersTimeStamp, bannerParser.ServerTimeStamp);
                }
            }
        }

        public void UpdateEpisodes(bool bUpdateNewEpisodes)
        {
            SQLCondition condition = new SQLCondition();
            // all series that have an onlineID ( != 0)
            condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cID, 0, SQLConditionType.NotEqual);
            condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cID, -1, SQLConditionType.NotEqual);

            if (bUpdateNewEpisodes)
            {
                MPTVSeriesLog.Write(bigLogMessage("Retrieving Metadata for new Episodes"));
                // and that never had data imported from the online DB
                condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cOnlineDataImported, 0, SQLConditionType.Equal);
                
                // scan all the episodes that don't have an episode thumbnail url - and mark them as no OnlineDataImported to retrieve clean data from the base
                SQLCondition conditionNoEpisodeSnapshot = new SQLCondition();
                conditionNoEpisodeSnapshot.Add(new DBOnlineEpisode(), DBOnlineEpisode.cEpisodeThumbnailUrl, "init", SQLConditionType.Equal);
                conditionNoEpisodeSnapshot.Add(new DBOnlineEpisode(), DBOnlineEpisode.cFirstAired, DateTime.Now.ToString("yyyy-MM-dd"), SQLConditionType.LessThan);
                DBTable.GlobalSet(new DBOnlineEpisode(), DBOnlineEpisode.cOnlineDataImported, 0, conditionNoEpisodeSnapshot);
            }
            else
            {
                MPTVSeriesLog.Write(bigLogMessage("Updating Metadata for existing Episodes"));
                // and that already had data imported from the online DB
                condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cOnlineDataImported, 1, SQLConditionType.Equal);
            }

            List<DBEpisode> episodeList = DBEpisode.Get(condition, false);
            int nTotalEpisodeCount = episodeList.Count;
            int nIndex = 0;

            long nUpdateEpisodesTimeStamp = 0;
            // in that case, don't use the lasttime of import
            if (!bUpdateNewEpisodes)
                nUpdateEpisodesTimeStamp = (long)DBOption.GetOptions(DBOption.cUpdateEpisodesTimeStamp);
            long nReturnedUpdateEpisodesTimeStamp = 0;
            if (episodeList.Count == 0)
                MPTVSeriesLog.Write("Nothing to do...");
            while (episodeList.Count > 0)
            {
                if (worker.CancellationPending)
                    return;

                worker.ReportProgress(70 + (bUpdateNewEpisodes ? 0 : 15) + (15 * nIndex / nTotalEpisodeCount));

                Dictionary<int, DBEpisode> IDToEpisodesMap = new Dictionary<int, DBEpisode>();
                int nCount = 0;
                // call update with batches of 500 ids max - otherwise the server fails to generate a big enough xml chunk
                while (nCount < 500 && episodeList.Count > 0)
                {
                    DBEpisode episode = episodeList[0];
                    episodeList.RemoveAt(0);
                    if (!IDToEpisodesMap.ContainsKey(episode[DBOnlineEpisode.cID]))
                        IDToEpisodesMap.Add(episode[DBOnlineEpisode.cID], episode);

                    nCount++;
                    nIndex++;
                }

                // generate a comma separated list of all the series ID
                String sEpisodeIDs = String.Empty;
                foreach (KeyValuePair<int, DBEpisode> pair in IDToEpisodesMap)
                {
                    if (Helper.String.IsNullOrEmpty(sEpisodeIDs))
                        sEpisodeIDs += pair.Value[DBOnlineEpisode.cID];
                    else
                        sEpisodeIDs += "," + pair.Value[DBOnlineEpisode.cID];
                }
                
                // use the last known timestamp from when we updated the episode
                MPTVSeriesLog.Write(string.Format("Updating {0} episodes, {1} left", IDToEpisodesMap.Count, episodeList.Count));
                UpdateEpisodes updateEpisodesParser = new UpdateEpisodes(sEpisodeIDs, nUpdateEpisodesTimeStamp);
                nReturnedUpdateEpisodesTimeStamp = updateEpisodesParser.ServerTimeStamp;
                if (updateEpisodesParser.Results.Count == 0)
                    MPTVSeriesLog.Write(string.Format("No {0} found", (bUpdateNewEpisodes ? "metadata" : "updates")));
                foreach (DBOnlineEpisode onlineEpisode in updateEpisodesParser.Results)
                {
                    m_bDataUpdated = true;
                    // find the corresponding series in our list
                    DBEpisode localEpisode = IDToEpisodesMap[onlineEpisode[DBOnlineEpisode.cID]];
                    MPTVSeriesLog.Write(string.Format("{0} metadata for \"{1}\"", (bUpdateNewEpisodes ? "Retrieved" : "Updating"), localEpisode.ToString()));
//                    bool isSecondOfDoubleEpisode = localEpisode.m_fields[DBEpisode.cSeriesID].Value.ToString().Length == 0;
                    if (localEpisode != null)
                    {
                        // go over all the fields, (and update only those which haven't been modified by the user - will do that later)
                        foreach (String key in onlineEpisode.FieldNames)
                        {
                            switch (key)
                            {
                                case DBOnlineEpisode.cCompositeID:
                                case DBEpisode.cSeriesID:
                                case DBOnlineEpisode.cWatched:
                                case DBOnlineEpisode.cDownloadPending:
                                case DBOnlineEpisode.cDownloadExpectedNames:
                                    // do nothing here, those information are local only
                                    break;
                                
                                case DBOnlineEpisode.cSeasonIndex:
                                case DBOnlineEpisode.cEpisodeIndex:
                                    break; // those must not get overwritten from what they were set to by getEpisodes (because of different order options)

                                default:
                                    localEpisode.onlineEpisode.AddColumn(key, new DBField(DBField.cTypeString));
                                    localEpisode[key] = onlineEpisode[key];
                                    break;
                            }
                        }
                        
//                         if (isSecondOfDoubleEpisode) // dont commit the local ep if so
//                             localEpisode.onlineEpisode.Commit();
//                         else
//                         {
                            localEpisode[DBOnlineEpisode.cOnlineDataImported] = 2;
                            localEpisode.Commit();
//                        }
                    }
                    else
                    {
                        // hopefully the server will NEVER return an ID I didn't asked for!
                    }
                }

                // now process incorrect IDs if any
                foreach (int nIncorrectID in updateEpisodesParser.BadIds)
                {
                    m_bDataUpdated = true;
                    m_bReparseNeeded = true;
                    MPTVSeriesLog.Write("Incorrect EpisodeID found! ID=" + nIncorrectID + " for episode '" + IDToEpisodesMap[nIncorrectID][DBOnlineEpisode.cCompositeID] + "'");
                    // reset the seriesID of this series
                    IDToEpisodesMap[nIncorrectID][DBOnlineEpisode.cID] = 0;
                    IDToEpisodesMap[nIncorrectID][DBOnlineEpisode.cOnlineDataImported] = 0;
                    IDToEpisodesMap[nIncorrectID].Commit();
                }
            }

            // save last episodes timestamp
            if (!bUpdateNewEpisodes && nReturnedUpdateEpisodesTimeStamp != 0)
                DBOption.SetOptions(DBOption.cUpdateEpisodesTimeStamp, nReturnedUpdateEpisodesTimeStamp);
        }

        static string generateIDList<T>(List<T> entities, string fieldname) where T:DBTable
        {
            // generate a comma separated list of all the ids
            String sSeriesIDs = String.Empty;
            if (entities.Count > 0)
            {
                foreach (DBTable entity in entities)
                {
                    if (sSeriesIDs.Length > 0)
                        sSeriesIDs += ",";
                    sSeriesIDs += entity[fieldname];
                }
            }
            return sSeriesIDs;
        }

        string bigLogMessage(string msg)
        {
            return string.Format("***********     {0}     ***********", msg);
        }
    }
}
