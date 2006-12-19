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
        Feedback.Interface m_feedback = null;
        List<System.IO.FileSystemWatcher> m_watchersList = new List<System.IO.FileSystemWatcher>();
        List<WatcherItem> m_modifiedFilesList = new List<WatcherItem>();

        public delegate void WatcherProgressHandler(int nProgress, List<WatcherItem> modifiedFilesList);

        /// <summary>
        /// This will be triggered once all the SeriesAndEpisodeInfo has been parsed completely.
        /// </summary>
        public event WatcherProgressHandler WatcherProgress;

        public Watcher(Feedback.Interface feedback)
        {
            m_feedback = feedback;
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
            }
        }

        void workerWatcher_DoWork(object sender, DoWorkEventArgs e)
        {
            DBImportPath[] importPaths = DBImportPath.GetAll();
            if (importPaths != null)
            {
                // ok let's see ... go through all enable import folders, and add a watchfolder on it
                foreach (DBImportPath importPath in DBImportPath.GetAll())
                {
                    if (importPath[DBImportPath.cEnabled] != 0)
                    {
                        // one watcher for each extension type
                        foreach (String extention in MediaPortal.Util.Utils.VideoExtensions)
                        {
                            FileSystemWatcher watcher = new FileSystemWatcher();
                            watcher.Filter = "*" + extention;
                            watcher.Path = importPath[DBImportPath.cPath];
                            watcher.IncludeSubdirectories = true;
                            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName; // only check for lastwrite .. I believe that's the only thing we're interested in
                            watcher.Changed += new FileSystemEventHandler(watcher_Changed);
                            watcher.Created += new FileSystemEventHandler(watcher_Changed);
                            watcher.Deleted += new FileSystemEventHandler(watcher_Changed);
                            watcher.Renamed += new RenamedEventHandler(watcher_Renamed);
                            watcher.EnableRaisingEvents = true;
                            m_watchersList.Add(watcher);
                        }
                    }
                }

                while (!worker.CancellationPending)
                {
                    try
                    {
                        List<WatcherItem> outList = new List<WatcherItem>();
                        lock (m_modifiedFilesList)
                        {
                            // go over the modified files list once in a while & update
                            while (m_modifiedFilesList.Count > 0)
                            {
                                outList.Add(m_modifiedFilesList[0]);
                                m_modifiedFilesList.RemoveAt(0);
                            }

                            if (outList.Count > 0)
                                worker.ReportProgress(0, outList);
                        }
                    }
                    catch (Exception exp)
                    {
                        MPTVSeriesLog.Write("Exception happened in workerWatcher_DoWork: " + exp.Message);
                    }

                    // wait
                    Thread.Sleep(5000);
                }
            }
        }
    };

    enum ParsingAction
    {
        Full,
        List_Add,
        List_Remove
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

        public void worker_RemoveDoWork(object sender, DoWorkEventArgs e)
        {
        }

        public void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            m_params = e.Argument as CParsingParameters;
            m_bFullSeriesRetrieval = DBOption.GetOptions(DBOption.cFullSeriesRetrieval);
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
                            DBEpisode episode = new DBEpisode(pair.sFull_FileName);
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
                            condition.Add(new DBEpisode(), DBEpisode.cFilename, pair.sFull_FileName, SQLConditionType.Equal);
                            DBEpisode.Clear(condition);
                        }

                        // now go over the touched seasons & series
                        foreach (DBSeason season in relatedSeasons)
                        {
                            if (DBEpisode.Get(season[DBSeason.cSeriesID], season[DBSeason.cIndex], true, false).Count > 0)
                            {
                                season[DBSeason.cHasLocalFilesTemp] = true;
                                season[DBSeason.cHasEpisodes] = true;
                            }
                            else
                                season[DBSeason.cHasLocalFilesTemp] = false;
                            season.Commit();
                        }

                        foreach (DBOnlineSeries series in relatedSeries)
                        {
                            if (DBEpisode.Get(series[DBOnlineSeries.cID], true, false).Count > 0)
                                series[DBOnlineSeries.cHasLocalFilesTemp] = true;
                            else
                                series[DBOnlineSeries.cHasLocalFilesTemp] = false;
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

                        ParseLocal(Filelister.GetFiles());

                        // now, remove all episodes still processed = 0, the weren't find in the scan
                        if (!DBOption.GetOptions(DBOption.cDontClearMissingLocalFiles))
                        {
                            SQLCondition condition = new SQLCondition();
                            condition.Add(new DBEpisode(), DBEpisode.cImportProcessed, 2, SQLConditionType.Equal);
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
                m_bReparseNeeded = true;
                while (m_bReparseNeeded)
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

                    // update new series for banners
                    UpdateBanners(true);
                    if (m_params.m_bUpdateScan)
                    {
                        // refresh existing banners
                        UpdateBanners(false);
                    }

                    // now update the episodes
                    UpdateEpisodes(true);

                    if (m_params.m_bUpdateScan)
                    {
                        // now refresh existing episodes
                        UpdateEpisodes(false);
                    }
                }
            }

            // and we are done, the backgroundworker is going to notify so
            MPTVSeriesLog.Write("***************************************************************************");
            MPTVSeriesLog.Write("*******************          Completed          ***************************");
            MPTVSeriesLog.Write("***************************************************************************");
        }

        void ParseLocal(List<PathPair> files)
        {
            foreach (parseResult progress in LocalParse.Parse(files))
            {
                if (worker.CancellationPending)
                    return;

                if (progress.success)
                {
                    int nEpisode = Convert.ToInt32(progress.parser.Matches[DBEpisode.cEpisodeIndex]);
                    int nSeason = Convert.ToInt32(progress.parser.Matches[DBEpisode.cSeasonIndex]);

                    // ok, we are sure it's valid now
                    // series first
                    DBSeries series = new DBSeries(progress.parser.Matches[DBSeries.cParsedName].ToLower());
                    series[DBOnlineSeries.cHasLocalFilesTemp] = 1;
                    // not much to do here except commiting the series
                    series.Commit();

                    // season now
                    DBSeason season = new DBSeason(series[DBSeries.cID], nSeason);
                    season[DBSeason.cHasLocalFilesTemp] = true;
                    season[DBSeason.cHasEpisodes] = true;
                    season.Commit();

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

                    // check if a subtitle is present alongside the file
                    String sDirectory = System.IO.Path.GetDirectoryName(progress.full_filename);
                    String sFileNameNoExt = System.IO.Path.GetFileNameWithoutExtension(progress.full_filename);
                    if (System.IO.File.Exists(sDirectory + @"\" + sFileNameNoExt + ".srt") || System.IO.File.Exists(sDirectory + @"\" + sFileNameNoExt + ".sub"))
                        episode[DBEpisode.cAvailableSubtitles] = true;
                    else
                        episode[DBEpisode.cAvailableSubtitles] = false;
                    
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
        }

        void GetSeries()
        {
            MPTVSeriesLog.Write("*********  GetSeries - unknown series  *********");

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
            List<DBSeries> seriesList = DBSeries.Get(condition);
            MPTVSeriesLog.Write("Found " + seriesList.Count + " Series without an online ID, looking for them");

            foreach (DBSeries series in seriesList)
            {
                if (worker.CancellationPending)
                    return;

                worker.ReportProgress(10 + (10 * nIndex / seriesList.Count));
                nIndex++;

                bool bDone = false;
                String sSeriesNameToSearch = series[DBSeries.cParsedName];
                while (!bDone)
                {
                    GetSeries GetSeriesParser = new GetSeries(sSeriesNameToSearch);

                    MPTVSeriesLog.Write("Found " + GetSeriesParser.Results.Count + " matching results for " + sSeriesNameToSearch);
                    // find out if our name is found in multiple results
                    int SubStringCount = 0;
                    int ExactMatchCount = 0;
                    foreach (DBOnlineSeries onlineSeries in GetSeriesParser.Results)
                    {
                        if (onlineSeries[DBOnlineSeries.cPrettyName].ToString().ToLower().IndexOf(sSeriesNameToSearch.ToLower()) != -1)
                            SubStringCount++;
                        if (onlineSeries[DBOnlineSeries.cPrettyName].ToString().ToLower() == sSeriesNameToSearch.ToLower())
                            ExactMatchCount++;
                    }

                    DBOnlineSeries UserChosenSeries = null;
                    if (GetSeriesParser.Results.Count > 0)
                        UserChosenSeries = GetSeriesParser.Results[0];

                    if (ExactMatchCount != 1 || (SubStringCount != 1 && DBOption.GetOptions(DBOption.cAutoChooseSeries) == 0))
                    {
                        // User has three choices:
                        // 1) Pick a series from the list
                        // 2) Simply skip
                        // 3) Skip and never ask for this series again

                        List<Feedback.CItem> Choices = new List<Feedback.CItem>();
                        foreach (DBOnlineSeries onlineSeries in GetSeriesParser.Results)
                            Choices.Add(new Feedback.CItem(onlineSeries[DBOnlineSeries.cPrettyName], "First Aired: " + onlineSeries["FirstAired"] + "\r\nOverview:\r\n" + onlineSeries[DBOnlineSeries.cSummary], onlineSeries));

                        if (Choices.Count == 0)
                            Choices.Add(new Feedback.CItem("No Match Found, try to enter another name for the show", String.Empty, null));

                        Feedback.CDescriptor descriptor = new Feedback.CDescriptor();
                        descriptor.m_sTitle = "Unable to find matching series";
                        descriptor.m_sItemToMatchLabel = "Local series:";
                        descriptor.m_sItemToMatch = sSeriesNameToSearch;
                        descriptor.m_sListLabel = "Choose the correct series from this list:";
                        descriptor.m_List = Choices;
                        descriptor.m_sbtnCancelLabel = "Skip this time";
                        descriptor.m_sbtnIgnoreLabel = "Skip/Never ask again";

                        Feedback.CItem Selected = null;
                        Feedback.ReturnCode result = m_feedback.ChooseFromSelection(descriptor, out Selected);
                        switch (result)
                        {
                            case Feedback.ReturnCode.Cancel:
                                UserChosenSeries = null;
                                bDone = true;
                                break;

                            case Feedback.ReturnCode.Ignore:
                                UserChosenSeries = null;
                                series[DBSeries.cScanIgnore] = 1; // means it will be skipped in the future
                                series[DBSeries.cHidden] = true;
                                series.Commit();
                                bDone = true;
                                break;

                            case Feedback.ReturnCode.OK:
                                DBOnlineSeries selectedSeries = Selected.m_Tag as DBOnlineSeries;
                                if (sSeriesNameToSearch != Selected.m_sName)
                                {
                                    sSeriesNameToSearch = Selected.m_sName;
                                    UserChosenSeries = null;
                                }
                                else
                                {
                                    UserChosenSeries = selectedSeries;
                                    bDone = true;
                                }
                                break;
                        }
                    }
                    else
                        bDone = true;

                    if (UserChosenSeries != null) // make sure selection was not cancelled
                    {
                        // set the ID on the current series with the one from the chosen one
                        // we need to update all depending items - seasons & episodes
                        List<DBSeason> seasons = DBSeason.Get(series[DBSeries.cID], false, false, false);
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
                        foreach (DBSeries seriesDupeSet in seriesDupeSetList)
                        {
                            if (bFirst)
                            {
                                seriesDupeSet[DBSeries.cDuplicateLocalName] = 0;
                                seriesDupeSet.Commit();
                                bFirst = false;
                            }
                            else
                            {
                                seriesDupeSet[DBSeries.cDuplicateLocalName] = 1;
                                seriesDupeSet.Commit();
                            }
                        }
                    }
                }
            }
        }

        void GetEpisodes()
        {
            long nGetEpisodesTimeStamp = 0;

            MPTVSeriesLog.Write("*********  Starting GetEpisodes  *********");
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

            List<DBSeries> seriesList = DBSeries.Get(condition);
            int nIndex = 0;

            foreach (DBSeries series in seriesList)
            {
                if (worker.CancellationPending)
                    return;

                worker.ReportProgress(20 + (20 * nIndex / seriesList.Count));
                nIndex++;

                if (m_bFullSeriesRetrieval && m_params.m_bUpdateScan)
                {
                    MPTVSeriesLog.Write("Looking for all the episodes of " + series[DBSeries.cParsedName]);
                    SQLCondition conditions = new SQLCondition();
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cID, 0, SQLConditionType.Equal);
                    List<DBEpisode> episodesList = DBEpisode.Get(conditions, true);
                    // if we have unidentified episodes, let's retrieve the full list
                    if (episodesList.Count > 0)
                        nGetEpisodesTimeStamp = 0;
                    else
                        nGetEpisodesTimeStamp = series[DBOnlineSeries.cGetEpisodesTimeStamp];

                    GetEpisodes episodesParser = new GetEpisodes(series[DBSeries.cID], nGetEpisodesTimeStamp);
                    if (episodesParser.Results.Count > 0)
                    {
                        MPTVSeriesLog.Write("Found " + episodesParser.Results.Count + " episodes for " + series[DBSeries.cParsedName]);
                        // add all online episodes in the local db
                        foreach (DBOnlineEpisode onlineEpisode in episodesParser.Results)
                        {
                            // season if not there yet
                            DBSeason season = new DBSeason(series[DBSeries.cID], onlineEpisode[DBOnlineEpisode.cSeasonIndex]);
                            season[DBSeason.cHasEpisodes] = true;
                            season.Commit();

                            DBOnlineEpisode newOnlineEpisode = new DBOnlineEpisode(series[DBSeries.cID], onlineEpisode[DBOnlineEpisode.cSeasonIndex], onlineEpisode[DBOnlineEpisode.cEpisodeIndex]);
                            newOnlineEpisode[DBOnlineEpisode.cID] = onlineEpisode[DBOnlineEpisode.cID];
                            if (newOnlineEpisode[DBOnlineEpisode.cEpisodeName] == "")
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
                    List<DBEpisode> episodesList = DBEpisode.Get(conditions, true);
                    if (episodesList.Count < 5)
                    {
                        foreach (DBEpisode episode in episodesList)
                        {
                            MPTVSeriesLog.Write("Looking for the single episode " + episode[DBOnlineEpisode.cCompositeID]);
                            GetEpisodes episodesParser = new GetEpisodes(series[DBSeries.cID], episode[DBEpisode.cSeasonIndex], episode[DBEpisode.cEpisodeIndex]);

                            if (episodesParser.Results.Count > 0)
                            {
                                // season update for online data
                                DBSeason season = new DBSeason(series[DBSeries.cID], episode[DBOnlineEpisode.cSeasonIndex]);
                                season[DBSeason.cHasEpisodes] = true;
                                season.Commit();

                                DBOnlineEpisode onlineEpisode = episodesParser.Results[0];
                                MPTVSeriesLog.Write("Found episodeID for " + episode[DBOnlineEpisode.cCompositeID]);
                                episode[DBOnlineEpisode.cID] = onlineEpisode[DBOnlineEpisode.cID];
                                if (episode[DBOnlineEpisode.cEpisodeName] == "")
                                    episode[DBOnlineEpisode.cEpisodeName] = onlineEpisode[DBOnlineEpisode.cEpisodeName];
                                episode.Commit();
                            }
                        }
                    }
                    else
                    {
                        // no need to do single matches for many episodes, it's more efficient to do it all at once
                        GetEpisodes episodesParser = new GetEpisodes(series[DBSeries.cID], 0);
                        if (episodesParser.Results.Count > 0)
                        {
                            MPTVSeriesLog.Write("Found " + episodesParser.Results.Count + " episodes for " + series[DBSeries.cParsedName]);
                            // look for the episodes for that series, and compare / update the values
                            foreach (DBOnlineEpisode onlineEpisode in episodesParser.Results)
                            {
                                foreach (DBEpisode localEpisode in episodesList)
                                {
                                    if ((int)localEpisode[DBEpisode.cSeasonIndex] == (int)onlineEpisode[DBOnlineEpisode.cSeasonIndex] &&
                                        (int)localEpisode[DBEpisode.cEpisodeIndex] == (int)onlineEpisode[DBOnlineEpisode.cEpisodeIndex])
                                    {
                                        // season update for online data
                                        DBSeason season = new DBSeason(series[DBSeries.cID], onlineEpisode[DBOnlineEpisode.cSeasonIndex]);
                                        season[DBSeason.cHasEpisodes] = true;
                                        season.Commit();

                                        MPTVSeriesLog.Write(localEpisode[DBOnlineEpisode.cCompositeID] + " identified");
                                        // update data
                                        localEpisode[DBOnlineEpisode.cID] = onlineEpisode[DBOnlineEpisode.cID];
                                        if (localEpisode[DBOnlineEpisode.cEpisodeName] == "")
                                            localEpisode[DBOnlineEpisode.cEpisodeName] = onlineEpisode[DBOnlineEpisode.cEpisodeName];
                                        localEpisode.Commit();
                                        // remove the localEpisode from the local list (we found it, it's updated, it's faster this way)
                                        episodesList.Remove(localEpisode);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        void UpdateSeries(bool bUpdateNewSeries)
        {
            long nUpdateSeriesTimeStamp = 0;
            // now retrieve the info about the series
            SQLCondition condition = new SQLCondition();
            // all series that have an onlineID ( != 0)
            condition.Add(new DBOnlineSeries(), DBOnlineSeries.cID, 0, SQLConditionType.GreaterThan);
            condition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);
            if (bUpdateNewSeries)
            {
                MPTVSeriesLog.Write("*********  UpdateSeries - retrieve unknown series  *********");
                // and that never had data imported from the online DB
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cOnlineDataImported, 0, SQLConditionType.Equal);
                // in that case, don't use the lasttime of import
                nUpdateSeriesTimeStamp = 0;
            }
            else
            {
                MPTVSeriesLog.Write("*********  UpdateSeries - refresh series  *********");
                // and that already had data imported from the online DB (but not the new ones, that are set to 2)
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cOnlineDataImported, 1, SQLConditionType.Equal);
                nUpdateSeriesTimeStamp = (long)DBOption.GetOptions(DBOption.cUpdateSeriesTimeStamp);
            }
            List<DBSeries> SeriesList = DBSeries.Get(condition);

            if (SeriesList.Count > 0)
            {
                // generate a comma separated list of all the series ID
                String sSeriesIDs = String.Empty;
                foreach (DBSeries series in SeriesList)
                {
                    if (sSeriesIDs == String.Empty)
                        sSeriesIDs += series[DBSeries.cID];
                    else
                        sSeriesIDs += "," + series[DBSeries.cID];
                }

                // use the last known timestamp from when we updated the series
                MPTVSeriesLog.Write("Updating " + SeriesList.Count + " Series");
                UpdateSeries UpdateSeriesParser = new UpdateSeries(sSeriesIDs, nUpdateSeriesTimeStamp);

                foreach (DBOnlineSeries updatedSeries in UpdateSeriesParser.Results)
                {
                    m_bDataUpdated = true;
                    if (worker.CancellationPending)
                        return;

                    MPTVSeriesLog.Write("Updating data for " + updatedSeries[DBOnlineSeries.cPrettyName]);
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
                                    case DBSeries.cParsedName:
                                    case DBOnlineSeries.cHasLocalFiles:
                                        break;

                                    default:
                                        localSeries.AddColumn(key, new DBField(DBField.cTypeString));
                                        localSeries[key] = updatedSeries[key];
                                        break;
                                }
                            }
                            // data import completed; set to 2 (data up to date)
                            localSeries[DBOnlineSeries.cOnlineDataImported] = 2;
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
        }

        void UpdateBanners(bool bUpdateNewSeries)
        {
            SQLCondition condition = new SQLCondition();
            // all series that have an onlineID ( > 0)
            condition.Add(new DBOnlineSeries(), DBSeries.cID, 0, SQLConditionType.GreaterThan);
            condition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);
            if (bUpdateNewSeries)
            {
                MPTVSeriesLog.Write("*********  UpdateBanners - retrieve banners for new series  *********");
                // and that never had data imported from the online DB
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cBannersDownloaded, 0, SQLConditionType.Equal);
            }
            else
            {
                MPTVSeriesLog.Write("*********  UpdateBanners - refresh banners for new series  *********");
                // and that already had data imported from the online DB
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cBannersDownloaded, 1, SQLConditionType.Equal);
            }

            List<DBSeries> seriesList = DBSeries.Get(condition);
            int nIndex = 0;
            MPTVSeriesLog.Write("Looking for banners on " + seriesList.Count + " Series");

            foreach (DBSeries series in seriesList)
            {
                if (worker.CancellationPending)
                    return;

                worker.ReportProgress(50 + (bUpdateNewSeries ? 0 : 10) + (10 * nIndex / seriesList.Count));
                nIndex++;

                if (bUpdateNewSeries)
                    MPTVSeriesLog.Write("Downloading banners for " + series[DBSeries.cParsedName]);
                else
                    MPTVSeriesLog.Write("Refreshing banners for " + series[DBSeries.cParsedName]);

                GetBanner GetBannerParser = new GetBanner(series[DBSeries.cID], bUpdateNewSeries?0:(long)series[DBOnlineSeries.cUpdateBannersTimeStamp]);

                String sLastTextBanner = String.Empty;
                String sLastGraphicalBanner = String.Empty;

                foreach (BannerSeries bannerSeries in GetBannerParser.bannerSeriesList)
                {
                    if (series[DBOnlineSeries.cBannerFileNames].ToString().IndexOf(bannerSeries.sBannerFileName) == -1)
                    {
                        m_bDataUpdated = true;
                        MPTVSeriesLog.Write("New banner found for " + series[DBSeries.cParsedName] + " : " + bannerSeries.sOnlineBannerPath);
                        if (series[DBOnlineSeries.cBannerFileNames] == String.Empty)
                            series[DBOnlineSeries.cBannerFileNames] += bannerSeries.sBannerFileName;
                        else
                        {
                            series[DBOnlineSeries.cBannerFileNames] += "|" + bannerSeries.sBannerFileName;
                        }
                    }
                    // prefer graphical
                    if (bannerSeries.bGraphical)
                        sLastGraphicalBanner = bannerSeries.sBannerFileName;
                    else
                        sLastTextBanner = bannerSeries.sBannerFileName;
                }

                if (series[DBOnlineSeries.cCurrentBannerFileName] == "")
                {
                    // use the last banner as the current one (if any graphical found)
                    // otherwise use the first available
                    if (sLastGraphicalBanner != String.Empty)
                        series[DBOnlineSeries.cCurrentBannerFileName] = sLastGraphicalBanner;
                    else
                        series[DBOnlineSeries.cCurrentBannerFileName] = sLastTextBanner;
                }

                series[DBOnlineSeries.cBannersDownloaded] = 2;
                series.Commit();

                foreach (BannerSeason bannerSeason in GetBannerParser.bannerSeasonList)
                {
                    DBSeason season = new DBSeason(series[DBSeries.cID], bannerSeason.nIndex);
                    if (season[DBSeason.cBannerFileNames].ToString().IndexOf(bannerSeason.sBannerFileName) == -1)
                    {
                        m_bDataUpdated = true;
                        if (season[DBSeason.cBannerFileNames] == String.Empty)
                        {
                            season[DBSeason.cBannerFileNames] += bannerSeason.sBannerFileName;
                        }
                        else
                        {
                            season[DBSeason.cBannerFileNames] += "|" + bannerSeason.sBannerFileName;
                            MPTVSeriesLog.Write("New banner found for " + season[DBSeason.cID] + " : " + bannerSeason.sOnlineBannerPath);
                        }
                    }
                    // use the last banner as the current one
                    season[DBSeason.cCurrentBannerFileName] = bannerSeason.sBannerFileName;
                    season.Commit();
                }

                if (!bUpdateNewSeries)
                {
                    series[DBOnlineSeries.cUpdateBannersTimeStamp] = GetBannerParser.ServerTimeStamp;
                    series.Commit();
                }
            }
        }

        void UpdateEpisodes(bool bUpdateNewEpisodes)
        {
            SQLCondition condition = new SQLCondition();
            // all series that have an onlineID ( != 0)
            condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cID, 0, SQLConditionType.NotEqual);
            condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cID, -1, SQLConditionType.NotEqual);

            if (bUpdateNewEpisodes)
            {
                MPTVSeriesLog.Write("*********  UpdateEpisodes - retrieve unknown episodes  *********");
                // and that never had data imported from the online DB
                condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cOnlineDataImported, 0, SQLConditionType.Equal);
            }
            else
            {
                MPTVSeriesLog.Write("*********  UpdateEpisodes - refresh episodes  *********");
                // and that already had data imported from the online DB
                condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cOnlineDataImported, 1, SQLConditionType.Equal);
            }

            List<DBEpisode> episodeList = DBEpisode.Get(condition, true);
            int nTotalEpisodeCount = episodeList.Count;
            int nIndex = 0;

            long nUpdateEpisodesTimeStamp = 0;
            // in that case, don't use the lasttime of import
            if (!bUpdateNewEpisodes)
                nUpdateEpisodesTimeStamp = (long)DBOption.GetOptions(DBOption.cUpdateEpisodesTimeStamp);
            long nReturnedUpdateEpisodesTimeStamp = 0;
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
                    if (sEpisodeIDs == String.Empty)
                        sEpisodeIDs += pair.Value[DBOnlineEpisode.cID];
                    else
                        sEpisodeIDs += "," + pair.Value[DBOnlineEpisode.cID];
                }

                // use the last known timestamp from when we updated the series
                MPTVSeriesLog.Write("Updating " + IDToEpisodesMap.Count + " Episodes, " + episodeList.Count + " left");
                UpdateEpisodes updateEpisodesParser = new UpdateEpisodes(sEpisodeIDs, nUpdateEpisodesTimeStamp);
                nReturnedUpdateEpisodesTimeStamp = updateEpisodesParser.ServerTimeStamp;
                foreach (DBOnlineEpisode onlineEpisode in updateEpisodesParser.Results)
                {
                    m_bDataUpdated = true;
                    // find the corresponding series in our list
                    DBEpisode localEpisode = IDToEpisodesMap[onlineEpisode[DBOnlineEpisode.cID]];
                    MPTVSeriesLog.Write("Updating data for " + localEpisode[DBEpisode.cCompositeID]);

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
                                    // do nothing here, those information are local only
                                    break;

                                default:
                                    localEpisode.onlineEpisode.AddColumn(key, new DBField(DBField.cTypeString));
                                    localEpisode[key] = onlineEpisode[key];
                                    break;
                            }
                        }
                        // update to date info
                        localEpisode[DBOnlineEpisode.cOnlineDataImported] = 2;
                        localEpisode.Commit();
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
    }
}
