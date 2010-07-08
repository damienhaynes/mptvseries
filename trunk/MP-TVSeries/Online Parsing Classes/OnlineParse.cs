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
using System.IO;
using System.Text;
using System.Threading;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using WindowPlugins.GUITVSeries.Feedback;
using Action = MediaPortal.GUI.Library.Action;

namespace WindowPlugins.GUITVSeries
{

    public enum ParsingAction
    {
        NoExactMatch,

        LocalScan,
        List_Add,
        List_Remove,
        MediaInfo,
        IdentifyNewSeries,
        IdentifyNewEpisodes,

        GetOnlineUpdates,
        UpdateSeries,
        UpdateEpisodes,
        UpdateEpisodeCounts,
        UpdateUserRatings,
        UpdateBanners,
        UpdateFanart,

        GetNewBanners,
        GetNewFanArt,
        UpdateEpisodeThumbNails,
        UpdateUserFavourites,

        UpdateRecentlyAdded,
        BroadcastRecentlyAdded
    }

    public class ParsingProgress
    {
        public ParsingAction CurrentAction { get; set; }
        public string CurrentProgress { get; set; }
        public int CurrentItem { get; set; }
        public int TotalItems { get; set; }
        public DBTable Details { get; set; }
        public string Picture { get; set; }

        public ParsingProgress(ParsingAction currentAction, int totalitems)
            : this(currentAction, null, -1, totalitems) { }
        public ParsingProgress(ParsingAction currentAction, string currentItem, int currentProgress, int totalItems) 
            : this(currentAction, currentItem, currentProgress, totalItems, null, null){}
        public ParsingProgress(ParsingAction currentAction, string currentItem, int currentProgress, int totalItems, DBTable details, string picture)
        {
            this.CurrentAction = currentAction;
            this.CurrentProgress = currentItem;
            this.CurrentItem = currentProgress;
            this.TotalItems = totalItems;
            this.Details = details;
            this.Picture = picture;
        }
    }

    class CParsingParameters
    {
        private static List<ParsingAction> FirstLocalScanActions = new List<ParsingAction> { 
            ParsingAction.LocalScan, 
            ParsingAction.MediaInfo,            
            ParsingAction.IdentifyNewSeries, 
            ParsingAction.IdentifyNewEpisodes,
            ParsingAction.UpdateEpisodeCounts
        };

        private static List<ParsingAction> OnlineRefreshActions = new List<ParsingAction> { 
            ParsingAction.GetOnlineUpdates, 
            ParsingAction.UpdateSeries, 
            ParsingAction.UpdateEpisodes, 
            ParsingAction.UpdateUserRatings, 
            ParsingAction.UpdateBanners, 
            ParsingAction.UpdateFanart, 
            ParsingAction.GetNewBanners, 
            ParsingAction.GetNewFanArt, 
            ParsingAction.UpdateEpisodeThumbNails, 
            ParsingAction.UpdateUserFavourites,
			ParsingAction.UpdateEpisodeCounts,
            ParsingAction.BroadcastRecentlyAdded,
            ParsingAction.UpdateRecentlyAdded
		};

        public List<ParsingAction> m_actions = new List<ParsingAction>();

        public bool m_bLocalScan = true;
        public bool m_bUpdateScan = true;
        public List<PathPair> m_files = null;

        public List<DBValue> m_series = null;
        public List<DBValue> m_episodes = null;

        public UserInputResults m_userInputResult = null;
        public Feedback.IEpisodeMatchingFeedback UserEpisodeMatcher = null;

        public CParsingParameters(bool bScanNew, bool bUpdateExisting)
        {
            m_bLocalScan = bScanNew;
            m_bUpdateScan = bUpdateExisting;

            if (m_bLocalScan)
                m_actions.AddRange(FirstLocalScanActions);

            if (m_bUpdateScan)
                m_actions.AddRange(OnlineRefreshActions);
        }

        public CParsingParameters(ParsingAction action, List<PathPair> files, bool bScanNew, bool bUpdateExisting)
        {
            m_bLocalScan = bScanNew;
            m_bUpdateScan = bUpdateExisting;

            m_actions.Add(action);
            if (action == ParsingAction.List_Add) {
                m_actions.Add(ParsingAction.MediaInfo);                
                m_actions.Add(ParsingAction.IdentifyNewSeries);
                m_actions.Add(ParsingAction.IdentifyNewEpisodes);
                m_actions.Add(ParsingAction.UpdateEpisodeCounts);
                m_actions.Add(ParsingAction.BroadcastRecentlyAdded);
            }
            
            if (action == ParsingAction.List_Remove)
            { 
                m_actions.Add(ParsingAction.UpdateEpisodeCounts); 
            }
            
            m_files = files;

            if (m_bLocalScan)
                m_actions.AddRange(FirstLocalScanActions);
            if (m_bUpdateScan)
                m_actions.AddRange(OnlineRefreshActions);
        }

        public CParsingParameters(IEnumerable<ParsingAction> actions, List<DBValue> series, List<DBValue> episodes)
        {
            m_actions.AddRange(actions);
            m_episodes = episodes;
            m_series = series;
        }

        public CParsingParameters(bool bScanNew, bool bUpdateExisting, UserInputResults UserInputResult, Feedback.IEpisodeMatchingFeedback UserEpisodeMatcher)
            : this(bScanNew, bUpdateExisting)
        {
            this.m_userInputResult = UserInputResult;
            this.UserEpisodeMatcher = UserEpisodeMatcher;
        }

    };

    class OnlineParsing
    {
        public bool onlineUpdateNeeded = false;
        public bool wasOnlineUpdate = false;
        public BackgroundWorker m_worker = new BackgroundWorker();
        IFeedback m_feedback = null;

        bool m_bDataUpdated = false;
        bool m_bFullSeriesRetrieval = false;
        bool m_bNoExactMatch = false;       //if set to true then the user will be always prompted to choose the series
        CParsingParameters m_params = null;
        DateTime m_LastOnlineMirrorUpdate = DateTime.MinValue;

        public static bool IsMainOnlineParseComplete = true; // not including extra background threads

        int RETRY_INTERVAL = 1000;
        int RETRY_MULTIPLIER = 2;
        int MAX_TIMEOUT = 120000;

        public delegate void OnlineParsingProgressHandler(int nProgress, ParsingProgress Progress);
        public delegate void OnlineParsingCompletedHandler(bool bDataUpdated);

        /// <summary>
        /// This will be triggered once all the SeriesAndEpisodeInfo has been parsed completely.
        /// </summary>
        public event OnlineParsingProgressHandler OnlineParsingProgress;
        public event OnlineParsingCompletedHandler OnlineParsingCompleted;

        public OnlineParsing(IFeedback feedback)
        {
            m_feedback = feedback;
            m_worker.WorkerReportsProgress = true;
            m_worker.WorkerSupportsCancellation = true;
            m_worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            m_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            m_worker.DoWork += new DoWorkEventHandler(worker_DoWork);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Run an Online update when needed
            if (onlineUpdateNeeded && !wasOnlineUpdate) {
                MPTVSeriesLog.Write("Worker completed, online update needed is set to true, needs online update!", MPTVSeriesLog.LogLevel.Debug);
                onlineUpdateNeeded = false;                
                Start(new CParsingParameters(false, true));
            }
            else {
                if (OnlineParsingCompleted != null) // only if any subscribers exist
                {
                    this.OnlineParsingCompleted.Invoke(m_bDataUpdated);
                }
            }           
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (OnlineParsingProgress != null) // only if any subscribers exist
                OnlineParsingProgress.Invoke(e.ProgressPercentage, e.UserState as ParsingProgress);
        }

        public bool IsWorking
        {
            get
            {
                return m_worker.IsBusy;
            }
        }

        public bool Start(CParsingParameters param)
        {
            wasOnlineUpdate=false;

            // check if we are doing any online refresh actions
            // we dont want to double up actions later            
            if (param.m_actions.Contains(ParsingAction.GetOnlineUpdates)) {
                wasOnlineUpdate = true;
            }

            if (!m_worker.IsBusy)
            {
                m_worker.RunWorkerAsync(param);
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
            m_worker.CancelAsync();
        }

        public void UpdateOnlineMirror()
        {
            TimeSpan tsUpdate = DateTime.Now - m_LastOnlineMirrorUpdate;
            if ((int)tsUpdate.TotalMinutes > 15)
            {
                m_LastOnlineMirrorUpdate = DateTime.Now;
                // Re-Initialize Mirrors in case they have changed or are down
                DBOnlineMirror.Init();

                // Try again, possibly resuming from standby and network interface is not available yet           
                int iSleepInterval = RETRY_INTERVAL;
                while (!DBOnlineMirror.IsMirrorsAvailable && (TVSeriesPlugin.IsResumeFromStandby || !TVSeriesPlugin.IsNetworkAvailable))
                {
                    if (iSleepInterval > MAX_TIMEOUT)
                    {
                        MPTVSeriesLog.Write("Aborting connection retries, maximum timeout has expired");
                        break;
                    }
                    MPTVSeriesLog.Write(string.Format("Retrying connection to mirror in {0} seconds", iSleepInterval / 1000));
                    Thread.Sleep(iSleepInterval);
                    DBOnlineMirror.Init();
                    iSleepInterval *= RETRY_MULTIPLIER;
                }
            }
        }

        public void worker_DoWork(object sender, DoWorkEventArgs e)
        {            
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            
            m_params = e.Argument as CParsingParameters;
            m_bFullSeriesRetrieval = DBOption.GetOptions(DBOption.cFullSeriesRetrieval);
            m_bNoExactMatch = false;
            m_worker.ReportProgress(0);

            IsMainOnlineParseComplete = false;

            TVSeriesPlugin.IsResumeFromStandby = false;

            UpdateOnlineMirror();

            BackgroundWorker tMediaInfo = null;
            BackgroundWorker tEpisodeCounts = null;
            BackgroundWorker tUserRatings = null;      
   
            Online_Parsing_Classes.GetUpdates updates = null;
            foreach (ParsingAction action in m_params.m_actions) {
                MPTVSeriesLog.Write("Begin Parsing action: ", action.ToString(), MPTVSeriesLog.LogLevel.Debug);
                if (m_worker.CancellationPending)
                    break;

                switch (action) {
                    case ParsingAction.List_Remove:
                        ParseActionRemove(m_params.m_files);
                        break;

                    case ParsingAction.List_Add:
                        ParseActionAdd(m_params.m_files);
                        break;

                    case ParsingAction.NoExactMatch:
                        this.m_bNoExactMatch = true;
                        break;

                    case ParsingAction.LocalScan:
                        ParseActionLocalScan(m_params.m_bLocalScan, m_params.m_bUpdateScan, m_params.m_userInputResult == null ? null : m_params.m_userInputResult.ParseResults);
                        break;

                    case ParsingAction.MediaInfo:
                        //Threaded MediaInfo.dll parsing of new files - goes straight to next task
                        if (!DBOption.GetOptions(DBOption.cDisableMediaInfo))
                        {
                            tMediaInfo = new BackgroundWorker();
                            MediaInfoParse(tMediaInfo);
                        }
                        break;

                    case ParsingAction.IdentifyNewSeries:
                        if (DBOption.GetOptions(DBOption.cOnlineParseEnabled) == 1)
                            UpdateOnlineMirror();
                        if (DBOnlineMirror.IsMirrorsAvailable)
                        {
                            GetSeries(m_worker, m_bNoExactMatch, m_params.m_userInputResult == null ? null : m_params.m_userInputResult.UserChosenSeries);
                            UpdateSeries(true, null); // todo: ask orderoption
                        }
                        break;

                    case ParsingAction.IdentifyNewEpisodes:
                        if (DBOption.GetOptions(DBOption.cOnlineParseEnabled) == 1)
                            UpdateOnlineMirror();
                        if (DBOnlineMirror.IsMirrorsAvailable)
                        {
                            GetEpisodes(m_params.m_bUpdateScan, m_bFullSeriesRetrieval);                            
                        }
                        break;

                    case ParsingAction.GetOnlineUpdates:
                        if (DBOption.GetOptions(DBOption.cOnlineParseEnabled) == 1)
                            UpdateOnlineMirror();
                        if (DBOnlineMirror.IsMirrorsAvailable)
                        {
                            updates = GetOnlineUpdates();
                        }
                        break;

                    case ParsingAction.UpdateSeries:
                        if (DBOption.GetOptions(DBOption.cOnlineParseEnabled) == 1)
                            UpdateOnlineMirror();
                        if (DBOnlineMirror.IsMirrorsAvailable)
                        {
                            if (updates != null)
                                UpdateSeries(false, updates.UpdatedSeries);
                            if (m_params.m_series != null)
                                UpdateSeries(false, m_params.m_series);
                        }
                        break;

                    case ParsingAction.UpdateEpisodes:
                        if (DBOption.GetOptions(DBOption.cOnlineParseEnabled) == 1)
                            UpdateOnlineMirror();
                        if (DBOnlineMirror.IsMirrorsAvailable)
                        {
                            if (updates != null)
                                UpdateEpisodes(updates.UpdatedEpisodes);
                            if (m_params.m_episodes != null)
                            {
                                UpdateEpisodes(m_params.m_episodes);                               
                            }
                        }
                        break;

                    case ParsingAction.UpdateBanners:
                        if (DBOption.GetOptions(DBOption.cOnlineParseEnabled) == 1)
                            UpdateOnlineMirror();
                        if (DBOnlineMirror.IsMirrorsAvailable && updates != null)
                            UpdateBanners(false, updates.UpdatedBanners);
                        break;

                    case ParsingAction.UpdateFanart:
                        if (DBOption.GetOptions(DBOption.cOnlineParseEnabled) == 1)
                            UpdateOnlineMirror();
                        if (DBOnlineMirror.IsMirrorsAvailable && updates != null && !DBOption.GetOptions(DBOption.cAutoUpdateAllFanart))
                            UpdateFanart(false, updates.UpdatedFanart);
                        break;

                    case ParsingAction.GetNewBanners:
                        if (DBOption.GetOptions(DBOption.cOnlineParseEnabled) == 1)
                            UpdateOnlineMirror();
                        if (DBOnlineMirror.IsMirrorsAvailable)
                            UpdateBanners(true, null);// update new series for banners
                        break;

                    case ParsingAction.GetNewFanArt:
                        if (DBOption.GetOptions(DBOption.cOnlineParseEnabled) == 1)
                            UpdateOnlineMirror();
                        if (DBOnlineMirror.IsMirrorsAvailable)
                            UpdateFanart(true, null);// updates ALL series for fanart - todo: only scan for series with missing fanart since we got new ones from thetvdb.com above...
                                                     //                                       mirror how banners does it.
                        break;

                    case ParsingAction.UpdateEpisodeThumbNails:
                        if (DBOption.GetOptions(DBOption.cOnlineParseEnabled) == 1)
                            UpdateOnlineMirror();
                        if (DBOnlineMirror.IsMirrorsAvailable)
                            UpdateEpisodeThumbNails();
                        break;

                    case ParsingAction.UpdateUserRatings:
                        if (DBOption.GetOptions(DBOption.cOnlineParseEnabled) == 1)
                            UpdateOnlineMirror();
                        if (DBOnlineMirror.IsMirrorsAvailable)
                            tUserRatings = new BackgroundWorker();
                            UpdateUserRatings(tUserRatings);
                        break;

                    case ParsingAction.UpdateUserFavourites:
                        if (DBOption.GetOptions(DBOption.cOnlineParseEnabled) == 1)
                            UpdateOnlineMirror();
                        if (DBOnlineMirror.IsMirrorsAvailable)
                            UpdateUserFavourites();
                        break;

                    case ParsingAction.UpdateEpisodeCounts:
                        //Threaded processing of episode counts - goes straight to next task
                        tEpisodeCounts = new BackgroundWorker();
                        UpdateEpisodeCounts(tEpisodeCounts);
                        break;

                    case ParsingAction.BroadcastRecentlyAdded:
                        // Broadcast Recently Added to Infoservice plugin
                        if (!Settings.isConfig) {
                            BroadcastRecentlyAdded();
                        }
                        break;

                    case ParsingAction.UpdateRecentlyAdded:
                        // set hasnewepisodes field on series record
                        UpdateRecentlyAdded();
                        break;
                }
            }

            IsMainOnlineParseComplete = true;

            // SLEEP UNTIL ALL WORKER THREADS ARE DONE - avoids user thinking scan it's completed
            do
            {
                Thread.Sleep(1000);
            }
            while ((tMediaInfo != null && tMediaInfo.IsBusy) || (tEpisodeCounts != null && tEpisodeCounts.IsBusy) || (tUserRatings != null && tUserRatings.IsBusy));
            
            // lets save the updateTimestamp
            if (updates != null && updates.OnlineTimeStamp > 0)
                DBOption.SetOptions(DBOption.cUpdateTimeStamp, updates.OnlineTimeStamp);

            Online_Parsing_Classes.OnlineAPI.ClearBuffer();

            // and we are done, the backgroundworker is going to notify so
            MPTVSeriesLog.Write("***************************************************************************");
            MPTVSeriesLog.Write("*******************            Completed           ************************");
            MPTVSeriesLog.Write("***************************************************************************");
        }

        private void BroadcastRecentlyAdded()
        {            
            MPTVSeriesLog.Write("Getting list of Recently Added episodes from Database");

            // Calculate date for querying database
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            dt = dt.Subtract(new TimeSpan(7,0,0,0,0));
            string date = dt.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");

            // Get a list of the most recently added episodes in the database
            SQLCondition conditions = new SQLCondition();
            conditions.Add(new DBEpisode(), DBEpisode.cFileDateCreated, date, SQLConditionType.GreaterEqualThan);
            conditions.AddOrderItem(DBEpisode.Q(DBEpisode.cFileDateCreated), SQLCondition.orderType.Descending);
            List<DBEpisode> episodes = DBEpisode.Get(conditions, false);

            if (episodes != null)
            {
                MPTVSeriesLog.Write("Sending most Recently Added episodes to InfoService plugin");

                // Infoservice only supports 3 most recent episodes
                if (episodes.Count > 3) episodes.RemoveRange(3, episodes.Count - 3);                
                
                episodes.Reverse();

                foreach (DBEpisode episode in episodes)
                {
                    // Get Episode Details and send to InfoService plugin
                    DBSeries series = Helper.getCorrespondingSeries(episode[DBEpisode.cSeriesID]);
                    if (series != null)
                    {
                        string episodeTitle = episode[DBEpisode.cEpisodeName];
                        string seasonIdx = episode[DBEpisode.cSeasonIndex];
                        string episodeIdx = episode[DBEpisode.cEpisodeIndex];
                        string seriesTitle = series.ToString();
                        string thumb = ImageAllocator.GetSeriesPosterAsFilename(series);
                        string fanart = Fanart.getFanart(episode[DBEpisode.cSeriesID]).FanartFilename;
                        string sendTitle = string.Format("{0}/{1}/{2}/{3}", seriesTitle, seasonIdx, episodeIdx, episodeTitle);

                        string[] episodeDetails = new string[] { "Series", sendTitle, thumb, fanart };
                        MPTVSeriesLog.Write(string.Format("InfoService: {0}, {1}, {2}, {3}", episodeDetails[0], episodeDetails[1], episodeDetails[2], episodeDetails[3]));

                        // Send message to InfoService plugin
                        GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_USER, 16000, 9811, 0, 0, 0, episodeDetails);
                        GUIGraphicsContext.SendMessage(msg);
                        GUIWindowManager.Process();
                    }
                }
            }
        }

        private void UpdateRecentlyAdded()
        {
            // Clear setting for all series
            DBSeries.GlobalSet(DBOnlineSeries.cHasNewEpisodes, "0");

            // Calculate date for querying database
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            dt = dt.Subtract(new TimeSpan(7, 0, 0, 0, 0));
            string date = dt.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");
           
            SQLCondition conditions = new SQLCondition();
            conditions.Add(new DBEpisode(), DBEpisode.cFileDateCreated, date, SQLConditionType.GreaterEqualThan);            
            List<DBEpisode> episodes = DBEpisode.Get(conditions, false);
            
            // set series 'HasNewEpisodes' field if it contains new episodes
            foreach (DBEpisode episode in episodes)
            {
                DBSeries series = Helper.getCorrespondingSeries(episode[DBEpisode.cSeriesID]);
                if (series != null)
                {
                    series[DBOnlineSeries.cHasNewEpisodes] = "1";
                    series.Commit();
                }
            }
        }

        #region ParseActions
        private void ParseActionRemove(List<PathPair> files) {
            string initialMsg = "*******************        Remove Run Starting      ***********************";
            MPTVSeriesLog.Write(prettyStars(initialMsg.Length));
            MPTVSeriesLog.Write(initialMsg);
            MPTVSeriesLog.Write(prettyStars(initialMsg.Length));
			
            List<DBOnlineSeries> relatedSeries = new List<DBOnlineSeries>();
            List<DBSeason> relatedSeasons = new List<DBSeason>();

            foreach (PathPair pair in files) 
            {               
                DBEpisode episode = new DBEpisode(pair.m_sFull_FileName, true);

                // already in?
                bool bSeasonFound = false;
                foreach (DBSeason season in relatedSeasons)
                {
                    if (season[DBSeason.cSeriesID] == episode[DBEpisode.cSeriesID] && season[DBSeason.cIndex] == episode[DBEpisode.cSeasonIndex])
                    {
                        bSeasonFound = true;
                        break;
                    }
                }
                if (!bSeasonFound)
                    relatedSeasons.Add(new DBSeason(episode[DBEpisode.cSeriesID], episode[DBEpisode.cSeasonIndex]));

                bool bSeriesFound = false;
                foreach (DBOnlineSeries series in relatedSeries)
                {
                    if (series[DBOnlineSeries.cID] == episode[DBEpisode.cSeriesID])
                    {
                        bSeriesFound = true;
                        break;
                    }
                }
                if (!bSeriesFound)
                    relatedSeries.Add(new DBOnlineSeries(episode[DBEpisode.cSeriesID]));

                SQLCondition condition = new SQLCondition();
                condition.Add(new DBEpisode(), DBEpisode.cFilename, pair.m_sFull_FileName, SQLConditionType.Equal);
                
                if (!LocalParse.isOnRemovable(pair.m_sFull_FileName) && !LocalParse.needToKeepReference(pair.m_sFull_FileName)) /*!DBOption.GetOptions(DBOption.cImport_DontClearMissingLocalFiles)*/
                {
                    DBEpisode.Clear(condition);
                    m_bDataUpdated = true;
                }
                else 
                {
                    DBEpisode.GlobalSet(DBEpisode.cImportProcessed, 2, condition);
                    DBEpisode.GlobalSet(DBEpisode.cIsAvailable, 0, condition);
                    m_bDataUpdated = true;
                }               
            }

            // now go over the touched seasons & series
            bool seriesHasLocalFilesTemp = false;
            bool seasonHasLocalFilesTemp = false;
            foreach (DBSeason season in relatedSeasons) {
                foreach (DBEpisode episode1 in DBEpisode.Get(season[DBSeason.cSeriesID], season[DBSeason.cIndex], false)) {
                    if (episode1[DBEpisode.cImportProcessed] == 1) {
                        seasonHasLocalFilesTemp = true;
                        break;
                    }
                }

                season[DBSeason.cHasLocalFilesTemp] = seasonHasLocalFilesTemp;

                if (DBEpisode.Get(season[DBSeason.cSeriesID], season[DBSeason.cIndex], false).Count > 0) {
                    //season[DBSeason.cHasLocalFilesTemp] = true;
                    season[DBSeason.cHasEpisodes] = true;
                }
                else {
                    //season[DBSeason.cHasLocalFilesTemp] = false;
                    season[DBSeason.cHasEpisodes] = false;
                }

                DBEpisode episode = DBEpisode.GetFirstUnwatched(season[DBSeason.cSeriesID], season[DBSeason.cIndex]);
                if (episode != null)
                    season[DBSeason.cUnwatchedItems] = true;
                else
                    season[DBSeason.cUnwatchedItems] = false;

                season.Commit();
            }

            foreach (DBOnlineSeries series in relatedSeries) {
                	// Just bug fixing :)
					foreach (DBEpisode episode1 in DBEpisode.Get(series[DBOnlineSeries.cID], false)) {
                    if (episode1[DBEpisode.cImportProcessed] == 1) {
                        seriesHasLocalFilesTemp = true;
                        break;
                    }
                }

                series[DBOnlineSeries.cHasLocalFilesTemp] = seriesHasLocalFilesTemp;

                //if (DBEpisode.Get((int)series[DBOnlineSeries.cID], false).Count > 0)
                //    series[DBOnlineSeries.cHasLocalFilesTemp] = true;
                //else
                //    series[DBOnlineSeries.cHasLocalFilesTemp] = false;

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
        }

        private void ParseActionAdd(List<PathPair> files) {
            string initialMsg = "*******************       Add Run Starting     ***************************";
            MPTVSeriesLog.Write(prettyStars(initialMsg.Length));
            MPTVSeriesLog.Write(initialMsg);
            MPTVSeriesLog.Write(prettyStars(initialMsg.Length));
            ParseLocal(files);

            // GUI refresh after update actions
            // and copy the HasLocalFileTemp value into the real one
            DBSeries.GlobalSet(DBOnlineSeries.cHasLocalFiles, DBOnlineSeries.cHasLocalFilesTemp);
            DBSeason.GlobalSet(DBSeason.cHasLocalFiles, DBSeason.cHasLocalFilesTemp);
        }

        private void ParseActionLocalScan(bool localScan, bool updateScan, IList<parseResult> UserModifiedParsedResults)
        {
            if (UserModifiedParsedResults == null) // only if we dont get the results from the user
            {
                string initialMsg = String.Format("******************* Full Run Starting (LocalScan: {0} -  UpdateScan: {1})   ***************************", localScan, updateScan);
                if (m_bNoExactMatch)
                {
                    initialMsg = String.Format("******************* NoExactMatch Run Starting (LocalScan: {0} -  UpdateScan: {1})   ***************************", localScan, updateScan);
                }

                MPTVSeriesLog.Write(prettyStars(initialMsg.Length));
                MPTVSeriesLog.Write(initialMsg);
                MPTVSeriesLog.Write(prettyStars(initialMsg.Length));
            }
            if (localScan) {
                // mark all files in the db as not processed (to figure out which ones we'll have to remove after the import)
                DBEpisode.GlobalSet(DBEpisode.cImportProcessed, 2);                
                DBEpisode.GlobalSet(DBEpisode.cIsAvailable, 0);

                // also clear all season & series for local files
                DBSeries.GlobalSet(DBOnlineSeries.cHasLocalFilesTemp, false);
                DBSeason.GlobalSet(DBSeason.cHasLocalFilesTemp, false);

                List<String> listFolders = new List<string>();
                DBImportPath[] importPathes = DBImportPath.GetAll();
                if (importPathes != null) {
                    foreach (DBImportPath importPath in importPathes) {
                        if (importPath[DBImportPath.cEnabled] != 0) {
                            listFolders.Add(importPath[DBImportPath.cPath]);
                        }
                    }
                }

                //ParseLocal(UserModifiedParsedResults == null ? Filelister.GetFiles(listFolders) : null, UserModifiedParsedResults);
                ParseLocal(Filelister.GetFiles(listFolders), UserModifiedParsedResults);              

                // now, remove all episodes still processed = 0, the weren't find in the scan
                SQLCondition condition = new SQLCondition();
                condition.Add(new DBEpisode(), DBEpisode.cImportProcessed, 2, SQLConditionType.Equal);
                condition.Add(new DBEpisode(), DBEpisode.cIsOnRemovable, false, SQLConditionType.Equal);

                foreach (DBEpisode localepisode in DBEpisode.Get(condition))
                {
                    if (!LocalParse.needToKeepReference(localepisode[DBEpisode.cFilename]))
                    {
                        DBEpisode.Clear(new SQLCondition(new DBEpisode(), DBEpisode.cFilename, localepisode[DBEpisode.cFilename], SQLConditionType.Equal));
                    }
                }

                // moved global set out of foreach, clear will delete episode which need to be deleted, this global set will update
                // all other episodes left in db that weren't found in the scan to be marked as not available
                // other way to do this is to have this lines in else condition of foreach loop:
                // localepisode[DBEpisode.cIsAvailable] = false;
                // localepisode.Commit;
                // but my understanding is that executing one "big" query is quicker than executing loads of "smaller" ones
                // added this because DBEpisode.Get fills the condition with default ones..
                condition = new SQLCondition();
                condition.Add(new DBEpisode(), DBEpisode.cImportProcessed, 2, SQLConditionType.Equal);
                condition.Add(new DBEpisode(), DBEpisode.cIsOnRemovable, false, SQLConditionType.Equal);
                DBEpisode.GlobalSet(DBEpisode.cIsAvailable, false, condition);

                // and copy the HasLocalFileTemp value into the real one
                DBSeries.GlobalSet(DBOnlineSeries.cHasLocalFiles, DBOnlineSeries.cHasLocalFilesTemp);
                DBSeason.GlobalSet(DBSeason.cHasLocalFiles, DBSeason.cHasLocalFilesTemp);
            }
        }

        private void GetSeries(BackgroundWorker worker, bool bNoExactMatch, Dictionary<string, UserInputResultSeriesActionPair> preChosenSeriesPairs)
        {
            MPTVSeriesLog.Write(bigLogMessage("Identifying Unknown Series Online"), MPTVSeriesLog.LogLevel.Debug);

            SQLCondition condition = null;

            condition = new SQLCondition();
            // all series that don't have an onlineID ( < 0) and not marked as ignored
            condition.Add(new DBSeries(), DBSeries.cID, 0, SQLConditionType.LessThan);
            condition.Add(new DBSeries(), DBSeries.cScanIgnore, 0, SQLConditionType.Equal);

            int nIndex = 0;
            List<DBSeries> seriesList = DBSeries.Get(condition, false, false);
            if (seriesList.Count > 0)            
            {
                // Run Online update when needed
                onlineUpdateNeeded = true;
                MPTVSeriesLog.Write(string.Format("Found {0} unknown Series, attempting to identify them now", seriesList.Count), MPTVSeriesLog.LogLevel.Debug);
            }
            else
                MPTVSeriesLog.Write("All Series are already identified", MPTVSeriesLog.LogLevel.Debug);

            foreach (DBSeries series in seriesList) {
                if (worker.CancellationPending)
                    return;
                
                String sSeriesNameToSearch = series[DBSeries.cParsedName];
                worker.ReportProgress(0, new ParsingProgress(ParsingAction.IdentifyNewSeries, sSeriesNameToSearch, ++nIndex, seriesList.Count, series, null));
                DBOnlineSeries UserChosenSeries = null;
                UserInputResultSeriesActionPair sap = null;

                if (preChosenSeriesPairs != null)
                    preChosenSeriesPairs.TryGetValue(sSeriesNameToSearch, out sap);

                if (preChosenSeriesPairs == null)
                {
                    UserChosenSeries = SearchForSeries(sSeriesNameToSearch, bNoExactMatch, m_feedback);
                }
                else if (sap != null && sap.RequestedAction == UserInputResults.SeriesAction.Approve)
                {
                    MPTVSeriesLog.Write("User has approved \"" + sSeriesNameToSearch + "\" as being: " + sap.ChosenSeries.ToString(), MPTVSeriesLog.LogLevel.Debug);
                    UserChosenSeries = sap.ChosenSeries;
                }
                else if (sap != null && sap.RequestedAction == UserInputResults.SeriesAction.IgnoreAlways)
                {
                    // duplicate code from SearchForSeries - should be changed
                    MPTVSeriesLog.Write("User has Ignored \"" + sSeriesNameToSearch + "\" in the future", MPTVSeriesLog.LogLevel.Debug);
                    DBSeries ignoreSeries = new DBSeries(sSeriesNameToSearch);
                    series[DBSeries.cScanIgnore] = 1; // means it will be skipped in the future
                    series[DBSeries.cHidden] = true;
                    series.Commit();
                }

                if (UserChosenSeries != null) // make sure selection was not cancelled
                {
                    worker.ReportProgress(0, new ParsingProgress(ParsingAction.IdentifyNewSeries, UserChosenSeries[DBOnlineSeries.cPrettyName], nIndex, seriesList.Count, series, null));
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
            // that is done
            worker.ReportProgress(0, new ParsingProgress(ParsingAction.IdentifyNewSeries, seriesList.Count));
        }

        private Online_Parsing_Classes.GetUpdates GetOnlineUpdates()
        {
            // let's get the time we last updated from the local options
            long lastUpdateTimeStamp = DBOption.GetOptions(DBOption.cUpdateTimeStamp);
            double curTimeStamp = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            double sinceLastUpdate = curTimeStamp - lastUpdateTimeStamp;

            MPTVSeriesLog.Write(bigLogMessage("Processing Updates from online DB"));

            Online_Parsing_Classes.OnlineAPI.UpdateType uType = WindowPlugins.GUITVSeries.Online_Parsing_Classes.OnlineAPI.UpdateType.all;
            if (sinceLastUpdate < 3600 * 24)
                uType = WindowPlugins.GUITVSeries.Online_Parsing_Classes.OnlineAPI.UpdateType.day;
            else if (sinceLastUpdate < 3600 * 24 * 7)
                uType = WindowPlugins.GUITVSeries.Online_Parsing_Classes.OnlineAPI.UpdateType.week;
            else if (sinceLastUpdate < 3600 * 24 * 30)
                uType = WindowPlugins.GUITVSeries.Online_Parsing_Classes.OnlineAPI.UpdateType.month;

            Online_Parsing_Classes.GetUpdates GU = new WindowPlugins.GUITVSeries.Online_Parsing_Classes.GetUpdates(uType);

            return GU;
        }

        private void UpdateSeries(bool bUpdateNewSeries, List<DBValue> seriesUpdated)
        {
            // now retrieve the info about the series
            SQLCondition condition = new SQLCondition();
            // all series that have an onlineID ( != 0)
            condition.Add(new DBOnlineSeries(), DBOnlineSeries.cID, 0, SQLConditionType.GreaterThan);
            condition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);
            if (bUpdateNewSeries) {
                MPTVSeriesLog.Write(bigLogMessage("Retrieving Metadata for new Series"));
                // and that never had data imported from the online DB
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cOnlineDataImported, 0, SQLConditionType.Equal);
            } else {
                MPTVSeriesLog.Write(bigLogMessage("Updating Metadata for existing Series"));
                // and that already had data imported from the online DB (but not the new ones, that are set to 1) ??
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cOnlineDataImported, 2, SQLConditionType.Equal);
            }
            List<DBSeries> SeriesList = DBSeries.Get(condition, false, false);

            if (!bUpdateNewSeries && SeriesList.Count > 0) {
                // let's check which of these we have any interest in
                for (int i = 0; i < SeriesList.Count; i++) {
                    if (!seriesUpdated.Contains(SeriesList[i][DBSeries.cID])) {
                        SeriesList.RemoveAt(i);
                        i--;
                    }
                }
            }

            if (SeriesList.Count == 0) {
                MPTVSeriesLog.Write("Nothing to do");
                return;
            }

            MPTVSeriesLog.Write(string.Format("{0} metadata of {1} Series", (bUpdateNewSeries ? "Retrieving" : "Looking for updated"), SeriesList.Count), MPTVSeriesLog.LogLevel.Debug);

            UpdateSeries UpdateSeriesParser = new UpdateSeries(generateIDListOfString(SeriesList, DBSeries.cID));
            
            int nIndex = 0;
            foreach (DBOnlineSeries updatedSeries in UpdateSeriesParser.ResultsLazy) {
                m_bDataUpdated = true;
                if (m_worker.CancellationPending)
                    return;
                               
                MPTVSeriesLog.Write(string.Format("Metadata {0} for \"{1}\"", (bUpdateNewSeries ? "retrieved" : "updated"), updatedSeries.ToString()), MPTVSeriesLog.LogLevel.Debug);
                // find the corresponding series in our list
                foreach (DBSeries localSeries in SeriesList) {
                    if (localSeries[DBSeries.cID] == updatedSeries[DBSeries.cID]) {
                        m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateSeries, updatedSeries[DBOnlineSeries.cPrettyName], ++nIndex, SeriesList.Count, updatedSeries, null));
                        // go over all the fields, (and update only those which haven't been modified by the user - will do that later)
                        foreach (String key in updatedSeries.FieldNames) {
                            switch (key) {
                                // do not overwrite current series local settings with the one from the online series (baaaad design??)
                                case DBSeries.cParsedName: // this field shouldn't be required here since updatedSeries is an Onlineseries and not a localseries??
                                case DBOnlineSeries.cHasLocalFiles:
                                case DBOnlineSeries.cHasLocalFilesTemp:
                                case DBOnlineSeries.cEpisodesUnWatched:
                                case DBOnlineSeries.cEpisodeCount:
                                case DBOnlineSeries.cIsFavourite:
                                case DBOnlineSeries.cChoseEpisodeOrder:

                                case DBOnlineSeries.cBannerFileNames: // banners get handled differently (later on)
                                case DBOnlineSeries.cPosterFileNames:
                                case DBOnlineSeries.cCurrentBannerFileName:
                                case DBOnlineSeries.cCurrentPosterFileName:
                                case DBOnlineSeries.cMyRating:
                                case DBOnlineSeries.cViewTags:
                                    break;
                                case DBOnlineSeries.cEpisodeOrders:
                                    if (bUpdateNewSeries)
                                        goto default;
                                    break;
                                default:
                                    if (!key.EndsWith(DBTable.cUserEditPostFix))
                                    {
                                        localSeries.AddColumn(key, new DBField(DBField.cTypeString));
                                        localSeries[key] = updatedSeries[key];
                                    }
                                    break;
                            }
                        }

                        // diff. order options
                        if (bUpdateNewSeries)
                            determineOrderOption(localSeries);

                        // data import completed; set to 2 (data up to date)
                        localSeries[DBOnlineSeries.cOnlineDataImported] = 2;

                        if (localSeries[DBOnlineSeries.cHasLocalFilesTemp])
                            localSeries[DBOnlineSeries.cHasLocalFiles] = 1;
                        
                        localSeries.Commit();
                        
                        // UPDATE CACHE to fix getting the series named as the parsed name instead of the online pretty name!
                        if(bUpdateNewSeries) cache.addChangeSeries(localSeries);
                    }
                }
            }
            if (nIndex == 0)
            {
                MPTVSeriesLog.Write(string.Format("No {0} found", (bUpdateNewSeries ? "metadata" : "updates")));
                m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateSeries, 0));
            }
            m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateSeries,  UpdateSeriesParser.Results.Count));
        }

        private void GetEpisodes(bool bUpdateScan, bool bFullSeriesRetrieval)
        {
            MPTVSeriesLog.Write(bigLogMessage("Get Episodes"));
            SQLCondition condition = null;
            if (bUpdateScan) {
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

            if (bFullSeriesRetrieval && bUpdateScan)
                MPTVSeriesLog.Write("Mode: Get all Episodes of Series");

            int epCount = 0;
            int nIndex = 0;
            foreach (DBSeries series in seriesList) {
                List<DBEpisode> episodesList = null;

                // lets get the list of unidentified episodes
                SQLCondition conditions = new SQLCondition();
                conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cID, 0, SQLConditionType.Equal);
                episodesList = DBEpisode.Get(conditions, false);

                epCount += episodesList.Count;
                
                if (bFullSeriesRetrieval || episodesList.Count > 0) {
                    GetEpisodes episodesParser = new GetEpisodes((string)series[DBSeries.cID]);
                    m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.IdentifyNewEpisodes, series[DBOnlineSeries.cPrettyName], ++nIndex, seriesList.Count, series, null));                    
                    if (episodesParser.Results.Count > 0) {
                        MPTVSeriesLog.Write(string.Format("Found {0} episodes online for \"{1}\"", episodesParser.Results.Count.ToString().PadLeft(3, '0'), series.ToString()));
                        // look for the episodes for that series, and compare / update the values
                        if (m_params.UserEpisodeMatcher == null) // auto mode
                            matchOnlineToLocalEpisodes(series, episodesList, episodesParser);
                        else // user mode
                            m_params.UserEpisodeMatcher.MatchEpisodesForSeries(series, episodesList, episodesParser.Results);
                    } else
                        MPTVSeriesLog.Write(string.Format("No episodes could be identified online for {0}, check that the online database has these episodes", series.ToString()));

                    if (bFullSeriesRetrieval) {
                        // add all online episodes in the local db
                        System.Globalization.NumberFormatInfo provider = new System.Globalization.NumberFormatInfo();
                        provider.NumberDecimalSeparator = ".";
                        foreach (DBOnlineEpisode onlineEpisode in episodesParser.Results) {
                            // only add episodes that have seaon/ep set in dvd-ordering mode
                            float onlineEp = -1;
                            float.TryParse(onlineEpisode["DVD_episodenumber"], System.Globalization.NumberStyles.AllowDecimalPoint, provider, out onlineEp);
                            float onlineSeason = -1;
                            float.TryParse(onlineEpisode["DVD_season"], System.Globalization.NumberStyles.AllowDecimalPoint, provider, out onlineSeason);
                            if (series[DBOnlineSeries.cChoseEpisodeOrder] == "DVD") {
                                onlineEpisode[DBOnlineEpisode.cSeasonIndex] = onlineSeason;
                                onlineEpisode[DBOnlineEpisode.cEpisodeIndex] = onlineEp;
                            }

                            if (!(series[DBOnlineSeries.cChoseEpisodeOrder] == "DVD" && (onlineEp == -1 || onlineSeason == -1))) {
                                // season if not there yet
                                DBSeason season = new DBSeason(series[DBSeries.cID], onlineEpisode[DBOnlineEpisode.cSeasonIndex]);
                                season[DBSeason.cHasEpisodes] = true;
                                DBSeason existing = DBSeason.getRaw(series[DBSeries.cID], onlineEpisode[DBOnlineEpisode.cSeasonIndex]);
                                if (existing != null) {
                                    season[DBSeason.cHasLocalFiles] = existing[DBSeason.cHasLocalFilesTemp];
                                    season[DBSeason.cHasLocalFilesTemp] = existing[DBSeason.cHasLocalFilesTemp];
                                }
                                season.Commit();

                                DBOnlineEpisode newOnlineEpisode = new DBOnlineEpisode(series[DBSeries.cID], onlineEpisode[DBOnlineEpisode.cSeasonIndex], onlineEpisode[DBOnlineEpisode.cEpisodeIndex]);
                                newOnlineEpisode[DBOnlineEpisode.cID] = onlineEpisode[DBOnlineEpisode.cID];
                                if (newOnlineEpisode[DBOnlineEpisode.cEpisodeName].ToString().Length == 0)
                                    newOnlineEpisode[DBOnlineEpisode.cEpisodeName] = onlineEpisode[DBOnlineEpisode.cEpisodeName];
                                foreach (String key in onlineEpisode.FieldNames) {
                                    switch (key) {
                                        case DBOnlineEpisode.cCompositeID:
                                        case DBEpisode.cSeriesID:
                                        case DBOnlineEpisode.cWatched:
                                        case DBOnlineEpisode.cHidden:
                                        case DBOnlineEpisode.cDownloadPending:
                                        case DBOnlineEpisode.cDownloadExpectedNames:
                                        case DBOnlineEpisode.cMyRating:
                                        case DBOnlineEpisode.cEpisodeThumbnailFilename:
                                            // do nothing here, those information are local only
                                            break;

                                        case DBOnlineEpisode.cSeasonIndex:
                                        case DBOnlineEpisode.cEpisodeIndex:
                                            break; // those must not get overwritten from what they were set to by getEpisodes (because of different order options)

                                        default:
                                            if (!key.EndsWith(DBTable.cUserEditPostFix))
                                            {
                                                newOnlineEpisode.AddColumn(key, new DBField(DBField.cTypeString));
                                                newOnlineEpisode[key] = onlineEpisode[key];
                                            }
                                            break;
                                    }
                                }
                                newOnlineEpisode.Commit();
                            }
                        }
                    }
                }
            }
            if (epCount == 0)
                MPTVSeriesLog.Write("No new episodes identified");            
            else {

                // get the result from user matching if needed and commit them
                List<KeyValuePair<DBSeries, List<KeyValuePair<DBEpisode, DBOnlineEpisode>>>> result;
                // this will block here
                if (m_params.UserEpisodeMatcher != null && m_params.UserEpisodeMatcher.GetResult(out result) == ReturnCode.OK)
                {
                    foreach (var match in result)
                        foreach (var episodePair in match.Value)
                        {
                            if(episodePair.Value != null && episodePair.Key != null)
                                commitOnlineToLocalEpisisodeMatch(match.Key, episodePair.Key, episodePair.Value);
                        }
                }

                // Online update when needed
                if (!bUpdateScan)
                    onlineUpdateNeeded = true;
            }
            m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.IdentifyNewEpisodes, epCount));            
        }

        private void UpdateEpisodes(List<DBValue> episodesUpdated)
        {
            MPTVSeriesLog.Write(bigLogMessage("Updating Metadata for existing Episodes"));

            // let's check which series/episodes we have locally
            SQLCondition cond = new SQLCondition(new DBOnlineEpisode(), DBOnlineEpisode.cID, 0, SQLConditionType.GreaterThan);
            cond.AddOrderItem(DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID), SQLCondition.orderType.Ascending);

            List<DBEpisode> episodesInDB = DBEpisode.Get(cond);

            if (episodesUpdated == null || episodesUpdated.Count == 0)
                episodesInDB.Clear();

            // let's check which of these we have any interest in
            for (int i = 0; i < episodesInDB.Count; i++)
                if (!episodesUpdated.Contains(episodesInDB[i][DBOnlineEpisode.cID]))
                    episodesInDB.RemoveAt(i--);

            if (episodesInDB.Count == 0) {
                m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateEpisodes, 0));            
                MPTVSeriesLog.Write("Nothing to do");
                return;
            }

            // let's updated those we are interested in
            // for the remaining ones get the <lang>.xml
            MPTVSeriesLog.Write(episodesInDB.Count.ToString() + " episodes need updating", MPTVSeriesLog.LogLevel.Debug);
            int seriesID = 0;
            for (int i = 0; i < episodesInDB.Count; i++) {
                if (seriesID != episodesInDB[i][DBEpisode.cSeriesID]) {
                    seriesID = episodesInDB[i][DBEpisode.cSeriesID];
                    
                    // Filter out episodes and parse only the ones in the current series
                    List<DBEpisode> eps = new List<DBEpisode>(episodesInDB.Count);
					for (int j = 0; j < episodesInDB.Count; j++) {
						if (episodesInDB[j][DBEpisode.cSeriesID] == seriesID)
							eps.Add(episodesInDB[j]);
					}
                    
					DBSeries series = Helper.getCorrespondingSeries(seriesID);
                    m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateEpisodes, null, eps.Count, episodesInDB.Count, series, null));
					if (series != null) {
						matchOnlineToLocalEpisodes(series, eps, new GetEpisodes(seriesID.ToString()));
					}					
                }
            }
            m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateEpisodes, episodesInDB.Count));
        }

        private void UpdateBanners(bool bUpdateNewSeries, List<DBValue> updatedSeries)
        {
            SQLCondition condition = new SQLCondition();
            // all series that have an onlineID ( > 0)
            condition.Add(new DBOnlineSeries(), DBSeries.cID, 0, SQLConditionType.GreaterThan);
            condition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);
            if (bUpdateNewSeries) {
                if (!DBOption.GetOptions(DBOption.cAutoDownloadMissingArtwork))
                    return;

                MPTVSeriesLog.Write(bigLogMessage("Checking for missing artwork"));
                // and that never had data imported from the online DB
                condition.beginGroup();
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cBannerFileNames, string.Empty, SQLConditionType.Equal);
                condition.nextIsOr = true;
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cPosterFileNames, string.Empty, SQLConditionType.Equal);
                condition.nextIsOr = true;
                condition.AddCustom(" exists (select * from season where season.seriesID = online_series.id and season.bannerfilenames = '')");
                condition.nextIsOr = false;
                condition.endGroup();
            } else {
                MPTVSeriesLog.Write(bigLogMessage("Checking for new artwork"));
                // and that already had data imported from the online DB
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cBannerFileNames, string.Empty, SQLConditionType.NotEqual);
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cPosterFileNames, string.Empty, SQLConditionType.NotEqual);
            }

            List<DBSeries> seriesList = DBSeries.Get(condition, false, false);
            if (!bUpdateNewSeries && seriesList.Count > 0) {
                // let's check which of these we have any interest in
                for (int i = 0; i < seriesList.Count; i++)
                    if (!updatedSeries.Contains(seriesList[i][DBSeries.cID])) {
                        seriesList.RemoveAt(i--);
                    }
            }

            int nIndex = 0;
            if (seriesList.Count == 0) {
                if (bUpdateNewSeries)
                    MPTVSeriesLog.Write("All Series appear to have artwork already");
                else
                    MPTVSeriesLog.Write("Nothing to do");
                m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateBanners, 0));
            } else
                MPTVSeriesLog.Write("Looking for artwork on " + seriesList.Count + " Series");

            foreach (DBSeries series in seriesList) {
                if (m_worker.CancellationPending)
                    return;
                nIndex++;
                MPTVSeriesLog.Write((bUpdateNewSeries ? "Downloading" : "Refreshing") + " artwork for \"" + series.ToString() + "\"", MPTVSeriesLog.LogLevel.Debug);
                m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateBanners, series[DBOnlineSeries.cPrettyName], nIndex, seriesList.Count, series, null));

                GetBanner bannerParser = new GetBanner((string)series[DBSeries.cID]);
                bannerParser.BannerDownloadDone += new newArtWorkDownloadDoneHandler(name =>
                {
                    if (!string.IsNullOrEmpty(name))
                        m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateBanners, string.Format("{0} - {1}", series[DBOnlineSeries.cPrettyName], name), nIndex, seriesList.Count, series, name));
                });
                bannerParser.DownloadBanners(Online_Parsing_Classes.OnlineAPI.SelLanguageAsString);

                String sLastTextBanner = String.Empty;
                String sLastGraphicalBanner = String.Empty;
                String sLastPoster = String.Empty;

                String sHighestRatedSeriesPoster = String.Empty;
                String sHighestRatedSeriesBanner = String.Empty;

                // Cleanup available Banners to choose from                
                string sBanners = series[DBOnlineSeries.cBannerFileNames].ToString();
                string sPosters = series[DBOnlineSeries.cPosterFileNames].ToString();

                String[] splitBanners = sBanners.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                String[] splitPosters = sPosters.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                string sAvailableBanners = "";
                string sAvailablePosters = "";
                foreach (String filename in splitBanners) {
                    if (sAvailableBanners.Trim().Length == 0) {
                        sAvailableBanners += filename;
                    } else
                        sAvailableBanners += "|" + filename;
                }
                series[DBOnlineSeries.cBannerFileNames] = sAvailableBanners;

                foreach (String filename in splitPosters) {
                    if (sAvailablePosters.Trim().Length == 0) {
                        sAvailablePosters += filename;
                    } else
                        sAvailablePosters += "|" + filename;
                }
                series[DBOnlineSeries.cPosterFileNames] = sAvailablePosters;
                
                seriesBannersMap seriesArtwork = Helper.getElementFromList<seriesBannersMap, string>(series[DBSeries.cID], "seriesID", 0, bannerParser.seriesBannersMap);
                if (seriesArtwork != null)  // oops!
                {
                    bool hasOwnLang = false;
                    foreach (BannerSeries bannerSeries in seriesArtwork.seriesBanners) {
                        if (!series[DBOnlineSeries.cBannerFileNames].ToString().Contains(bannerSeries.sBannerFileName)) {
                            m_bDataUpdated = true;
                            // note: we also log in the progress recieved event
                            MPTVSeriesLog.Write("New series banner found for \"" + series.ToString() + "\" : " + bannerSeries.sOnlineBannerPath, MPTVSeriesLog.LogLevel.Debug);
                            if (series[DBOnlineSeries.cBannerFileNames].ToString().Trim().Length == 0) {
                                series[DBOnlineSeries.cBannerFileNames] += bannerSeries.sBannerFileName;
                            } else {
                                series[DBOnlineSeries.cBannerFileNames] += "|" + bannerSeries.sBannerFileName;
                            }
                        }
                        // Prefer the highest rated, localized, graphical banner
                        if (bannerSeries.sBannerLang == Online_Parsing_Classes.OnlineAPI.SelLanguageAsString) {
                            if (bannerSeries.bGraphical) {
                                if (bannerSeries.bHighestRated)
                                    sHighestRatedSeriesBanner = bannerSeries.sBannerFileName;
                                sLastGraphicalBanner = bannerSeries.sBannerFileName;
                            } else
                                sLastTextBanner = bannerSeries.sBannerFileName;

                            hasOwnLang = true;
                        } else if (!hasOwnLang) {
                            if (bannerSeries.bGraphical)
                                sLastGraphicalBanner = bannerSeries.sBannerFileName;
                            else
                                sLastTextBanner = bannerSeries.sBannerFileName;
                        }
                    }

                    // Don't override user selection of banner
                    if (series[DBOnlineSeries.cCurrentBannerFileName].ToString().Trim().Length == 0) {
                        // Use highest rated banner if one found                                                                  
                        if (sHighestRatedSeriesBanner.Length > 0)
                            series[DBOnlineSeries.cCurrentBannerFileName] = sHighestRatedSeriesBanner;
                        // Use the last banner as the current one (if any graphical found)
                        else if (sLastGraphicalBanner.Length > 0)
                            series[DBOnlineSeries.cCurrentBannerFileName] = sLastGraphicalBanner;
                        else // otherwise use the first available 
                            series[DBOnlineSeries.cCurrentBannerFileName] = sLastTextBanner;
                    }

                    foreach (PosterSeries posterSeries in seriesArtwork.seriesPosters) {
                        if (!series[DBOnlineSeries.cPosterFileNames].ToString().Contains(posterSeries.sPosterFileName)) {
                            m_bDataUpdated = true;
                            MPTVSeriesLog.Write("New series poster found for \"" + series.ToString() + "\" : " + posterSeries.sOnlinePosterPath, MPTVSeriesLog.LogLevel.Debug);
                            if (series[DBOnlineSeries.cPosterFileNames].ToString().Trim().Length == 0) {
                                series[DBOnlineSeries.cPosterFileNames] += posterSeries.sPosterFileName;
                            } else {
                                series[DBOnlineSeries.cPosterFileNames] += "|" + posterSeries.sPosterFileName;
                            }
                        }
                        // Prefer the highest rated localized poster
                        // Jan 4th 09 - Currently theTVDB does not support localized posters but does have a field for language defined
                        // Perhaps this will be added at a later date, handle this just incase
                        if (posterSeries.sPosterLang == Online_Parsing_Classes.OnlineAPI.SelLanguageAsString) {
                            if (posterSeries.bHighestRated)
                                sHighestRatedSeriesPoster = posterSeries.sPosterFileName;

                            sLastPoster = posterSeries.sPosterFileName;
                            hasOwnLang = true;
                        } else if (!hasOwnLang)
                            sLastPoster = posterSeries.sPosterFileName;
                    }

                    // Don't override user selection of poster
                    if (series[DBOnlineSeries.cCurrentPosterFileName].ToString().Trim().Length == 0) {
                        // Use highest rated poster if one found                                                                  
                        if (sHighestRatedSeriesPoster.Length > 0)
                            series[DBOnlineSeries.cCurrentPosterFileName] = sHighestRatedSeriesPoster;
                        else {
                            if (sLastPoster.Length > 0)
                                series[DBOnlineSeries.cCurrentPosterFileName] = sLastPoster;
                            else {
                                if (seriesArtwork.seriesPosters.Count > 0)
                                    series[DBOnlineSeries.cCurrentPosterFileName] = seriesArtwork.seriesPosters[0].sPosterFileName;
                            }
                        }
                    }

                    series.Commit();

                    hasOwnLang = false;
                    foreach (BannerSeason bannerSeason in seriesArtwork.seasonBanners) {
                        string lastSeasonBanner = string.Empty;
                        string sHighestRatedSeasonBanner = String.Empty;

                        DBSeason season = new DBSeason(series[DBSeries.cID], Int32.Parse(bannerSeason.sSeason));
                        if (season[DBSeason.cBannerFileNames].ToString().IndexOf(bannerSeason.sBannerFileName) == -1) {
                            m_bDataUpdated = true;
                            if (season[DBSeason.cBannerFileNames].ToString().Length == 0) {
                                season[DBSeason.cBannerFileNames] += bannerSeason.sBannerFileName;
                            } else {
                                season[DBSeason.cBannerFileNames] += "|" + bannerSeason.sBannerFileName;
                                MPTVSeriesLog.Write("New season banner found for \"" + series.ToString() + "\" Season " + season[DBSeason.cIndex] + ": " + bannerSeason.sOnlineBannerPath, MPTVSeriesLog.LogLevel.Debug);
                            }
                        }

                        if (bannerSeason.sBannerLang == Online_Parsing_Classes.OnlineAPI.SelLanguageAsString) {
                            if (bannerSeason.bHighestRated)
                                sHighestRatedSeasonBanner = bannerSeason.sBannerFileName;
                            lastSeasonBanner = bannerSeason.sBannerFileName;
                            hasOwnLang = true;
                        } else if (!hasOwnLang) {
                            lastSeasonBanner = bannerSeason.sBannerFileName;
                        }

                        // Check if Currently set season banner exists, its possible that the file path has changed.
                        bool bBannerExists = File.Exists(Helper.PathCombine(Settings.GetPath(Settings.Path.banners),
                                                         season[DBSeason.cCurrentBannerFileName].ToString()));

                        // Prefer highest rated banner as current
                        if (season[DBSeason.cCurrentBannerFileName].ToString().Trim().Length == 0 || !bBannerExists) {
                            if (sHighestRatedSeasonBanner.Length > 0) {
                                season[DBSeason.cCurrentBannerFileName] = sHighestRatedSeasonBanner;
                            } else
                                season[DBSeason.cCurrentBannerFileName] = lastSeasonBanner;
                        }
                        season.Commit();
                    }
                }
            }
            m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateBanners, seriesList.Count));
        }

        private void UpdateFanart(bool bUpdateNewSeries, List<DBValue> updatedSeries)
        {
            if (!DBOption.GetOptions(DBOption.cAutoDownloadFanart))
                return;

            if (bUpdateNewSeries && !DBOption.GetOptions(DBOption.cAutoUpdateAllFanart)) return;

            SQLCondition condition = new SQLCondition();
            condition.Add(new DBSeries(), DBSeries.cID, 0, SQLConditionType.GreaterThan);
            condition.Add(new DBSeries(), DBSeries.cScanIgnore, 0, SQLConditionType.Equal);
            condition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);

            List<DBSeries> seriesList = DBSeries.Get(condition, false, false);
            if (!bUpdateNewSeries && seriesList.Count > 0)
            {
                MPTVSeriesLog.Write(bigLogMessage("Checking for new Fanart"));
                // let's check which of these we have any interest in
                for (int i = 0; i < seriesList.Count; i++)
                    if (!updatedSeries.Contains(seriesList[i][DBSeries.cID]))
                    {
                        seriesList.RemoveAt(i--);
                    }
            }
            else
            {
                MPTVSeriesLog.Write(bigLogMessage("Checking for all Fanart"));
            }
            int nIndex = 0;
            foreach (DBSeries series in seriesList)
            {
                MPTVSeriesLog.Write("Retrieving Fanart for: " + Helper.getCorrespondingSeries(series[DBSeries.cID]), MPTVSeriesLog.LogLevel.Debug);
                m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateFanart, series[DBOnlineSeries.cPrettyName], ++nIndex, seriesList.Count, series, null));
                try {
                    GetFanart gf = new GetFanart(series[DBSeries.cID]);
                    foreach (DBFanart f in gf.Fanart)
                        f.Commit();

                    // Get List of Fanarts to auto download
                    DBFanart fanart = new DBFanart();
                    List<DBFanart> fanarts = fanart.FanartsToDownload(series[DBSeries.cID]);

                    // Download Fanart
                    foreach (DBFanart toDownload in fanarts) {
                        string onlineFilename = toDownload[DBFanart.cBannerPath];
                        string localFilename = onlineFilename.Replace("/", @"\");

                        MPTVSeriesLog.Write(string.Format("New Fanart found for \"{0}\": {1}", Helper.getCorrespondingSeries(series[DBSeries.cID]), onlineFilename), MPTVSeriesLog.LogLevel.Debug);
                        string result = Online_Parsing_Classes.OnlineAPI.DownloadBanner(onlineFilename, Settings.Path.fanart, localFilename);

                        if (result != null) {
                            // Update Fanart DB
                            toDownload[DBFanart.cLocalPath] = localFilename;
                            toDownload.Commit();

                            m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateFanart, string.Format("{0} - {1}", series[DBOnlineSeries.cPrettyName], Helper.PathCombine(Settings.GetPath(Settings.Path.fanart), localFilename)), nIndex, seriesList.Count, series, result));
                        }
                    }
                } catch (Exception ex) {
                    MPTVSeriesLog.Write("Failed to update Fanart: " + ex.Message);
                }
            }
            m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateFanart, seriesList.Count));
        }

        public void UpdateUserRatings(BackgroundWorker tUserRatings)
        {
            MPTVSeriesLog.Write(bigLogMessage("Get User Ratings"), MPTVSeriesLog.LogLevel.Debug);
            List<DBOnlineSeries> seriesList = DBOnlineSeries.getAllSeries();

            tUserRatings.DoWork += new DoWorkEventHandler(asyncUserRatings);
            tUserRatings.RunWorkerCompleted += new RunWorkerCompletedEventHandler(asyncUserRatingsCompleted);
            tUserRatings.RunWorkerAsync(seriesList);
        }

        void asyncUserRatingsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MPTVSeriesLog.Write("*****************   User Ratings Updated in Database    *******************");
        }

        void asyncUserRatings(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;

            string sAccountID = DBOption.GetOptions(DBOption.cOnlineUserID);            

            if (!String.IsNullOrEmpty(sAccountID))
            {
                List<DBOnlineSeries> seriesList = (List<DBOnlineSeries>)e.Argument;

                int nIndex = 0;
                if (DBOption.GetOptions(DBOption.cAutoUpdateEpisodeRatings)) // i.e. update Series AND Underlying Episodes
                {
                    bool MarkWatched = DBOption.GetOptions(DBOption.cMarkRatedEpisodeAsWatched);
                    
                    foreach (DBOnlineSeries series in seriesList)
                    {
                        MPTVSeriesLog.Write("Retrieving user ratings for series: " + Helper.getCorrespondingSeries((int)series[DBOnlineSeries.cID]), MPTVSeriesLog.LogLevel.Debug);
                        GetUserRatings userRatings = new GetUserRatings(series[DBOnlineSeries.cID], sAccountID);
                        
                        // Update Progress
                        m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateUserRatings, series[DBOnlineSeries.cPrettyName], ++nIndex, seriesList.Count, series, null));

                        // Set Series Ratings
                        // We should also update Community Rating as theTVDB Updates API doesnt take into consideration
                        // Series/Episodes that have rating changes.
                        series[DBOnlineSeries.cMyRating] = userRatings.SeriesUserRating;
                        // Dont clear site rating if user rating does not exist
                        if (!String.IsNullOrEmpty(userRatings.SeriesCommunityRating)) {
                            series[DBOnlineSeries.cRating] = userRatings.SeriesCommunityRating;
                        }
                        series.Commit();

                        SQLCondition condition = new SQLCondition();
                        condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series[DBOnlineSeries.cID], SQLConditionType.Equal);
                        List<DBEpisode> episodes = DBEpisode.Get(condition);

                        // Set Episode Ratings
                        foreach (DBEpisode episode in episodes)
                        {
                            if (userRatings.EpisodeUserRatings.ContainsKey(episode[DBOnlineEpisode.cID])) {
                                episode[DBOnlineEpisode.cMyRating] = userRatings.EpisodeUserRatings[episode[DBOnlineEpisode.cID]];

                                // If user has rated episode then mark as watched
                                if (MarkWatched)
                                    episode[DBOnlineEpisode.cWatched] = true;

                                episode.Commit();
                            }                            
                            if (userRatings.EpisodeCommunityRatings.ContainsKey(episode[DBOnlineEpisode.cID])) {
                                episode[DBOnlineEpisode.cRating] = userRatings.EpisodeCommunityRatings[episode[DBOnlineEpisode.cID]];
                                episode.Commit();
                            }

                            // Is this better?
                            /*if (userRatings.EpisodeRatings.ContainsKey(episode[DBOnlineEpisode.cID])) {
                                episode[DBOnlineEpisode.cMyRating] = userRatings.EpisodeRatings[episode[DBOnlineEpisode.cID]].UserRating;
                                episode[DBOnlineEpisode.cRating] = userRatings.EpisodeRatings[episode[DBOnlineEpisode.cID]].CommunityRating;
                                
                                if (MarkWatched)
                                    episode[DBOnlineEpisode.cWatched] = true;
    
                                episode.Commit();
                            }*/

                        }
                    }
                }
                else //update Series only, not Episodes -- workaround for not being able to pull up all series/episode ratings at once from theTVDB.com; saves time.
                {
                    GetUserRatings userRatings = new GetUserRatings(null, sAccountID);
                    foreach (DBOnlineSeries series in seriesList)
                    {
                        // Update Progress
                        m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateUserRatings, series[DBOnlineSeries.cPrettyName], ++nIndex, seriesList.Count, series, null));

                        if (userRatings.AllSeriesUserRatings.ContainsKey(series[DBOnlineSeries.cID]))
                        {
                            MPTVSeriesLog.Write("User ratings retrieved for series: " + Helper.getCorrespondingSeries((int)series[DBOnlineSeries.cID]));
                            series[DBOnlineSeries.cMyRating] = userRatings.AllSeriesUserRatings[series[DBOnlineSeries.cID]];
                            series.Commit();
                        }
                        if (userRatings.AllSeriesCommunityRatings.ContainsKey(series[DBOnlineSeries.cID])) {
                            MPTVSeriesLog.Write("User ratings retrieved for series: " + Helper.getCorrespondingSeries((int)series[DBOnlineSeries.cID]));
                            // Dont clear site rating if user rating does not exist
                            if (!String.IsNullOrEmpty(userRatings.AllSeriesCommunityRatings[series[DBOnlineSeries.cID]])) {
                                MPTVSeriesLog.Write("User ratings retrieved for series: " + Helper.getCorrespondingSeries((int)series[DBOnlineSeries.cID]));
                                series[DBOnlineSeries.cRating] = userRatings.AllSeriesCommunityRatings[series[DBOnlineSeries.cID]];
                                series.Commit();
                            }                            
                        }
                    }
                }
                m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateUserRatings, seriesList.Count));
            }            
        }

        public void UpdateUserFavourites()
        {
            string sAccountID = DBOption.GetOptions(DBOption.cOnlineUserID);
            int nIndex = 0;

            if (!String.IsNullOrEmpty(sAccountID)) {
                MPTVSeriesLog.Write(bigLogMessage("Get User Favourites"), MPTVSeriesLog.LogLevel.Debug);

                GetUserFavourites userFavourites = new GetUserFavourites(sAccountID);

                SQLCondition conditions = new SQLCondition();
                List<DBSeries> seriesList = DBSeries.Get(conditions);
                
                foreach (DBSeries series in seriesList) {
                    // Update Progress
                    m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateUserFavourites, series[DBOnlineSeries.cPrettyName], ++nIndex, seriesList.Count, series, null));

                    if (userFavourites.Series.Contains(series[DBOnlineSeries.cID])) {
                        MPTVSeriesLog.Write("Retrieved favourite series: " + Helper.getCorrespondingSeries((int)series[DBOnlineSeries.cID]), MPTVSeriesLog.LogLevel.Debug);
                        series[DBOnlineSeries.cViewTags] = Helper.GetSeriesViewTags(series, true, DBView.cTranslateTokenOnlineFavourite);
                        series.Commit();
                    } else {
                        series[DBOnlineSeries.cViewTags] = Helper.GetSeriesViewTags(series, false, DBView.cTranslateTokenOnlineFavourite);
                        series.Commit();
                    }
                }
                m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateUserFavourites, seriesList.Count));
            }            
        }

        public void UpdateEpisodeThumbNails()
        {
            if (DBOption.GetOptions(DBOption.cGetEpisodeSnapshots) == true) {
                MPTVSeriesLog.Write(bigLogMessage("Checking for Episode Thumbnails"), MPTVSeriesLog.LogLevel.Debug);

                // get a list of all the episodes with thumbnailUrl
                //SQLCondition condition = new SQLCondition();                
                //condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cEpisodeThumbnailUrl, "", SQLConditionType.NotEqual);                
                //condition.AddOrderItem(DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID), SQLCondition.orderType.Ascending);
                //List<DBEpisode> episodes = DBEpisode.Get(condition);

                // Get all online episodes that have a image but not yet downloaded                
                string query = string.Empty;
                if (Settings.isConfig)
                    // Be more thorough in configuration, user may have deleted thumbs locally
                    query = "select * from online_episodes where ThumbURL != '' order by SeriesID asc";
                else
                    query = "select * from online_episodes where ThumbURL != '' and thumbFilename = '' order by SeriesID asc";

                List<DBEpisode> episodes = DBEpisode.Get(query);

                DBSeries tmpSeries = null;
                int nIndex = 0;
                foreach (DBEpisode episode in episodes) {
                    String sThumbNailFilename = episode[DBOnlineEpisode.cEpisodeThumbnailFilename];
                    string basePath = Settings.GetPath(Settings.Path.banners);
                    string completePath = Helper.PathCombine(basePath, sThumbNailFilename);
                    
                    // we need the pretty name to figure out the folder to store to
                    try {
                        if (null == tmpSeries || tmpSeries[DBSeries.cID] != episode[DBEpisode.cSeriesID])
                            tmpSeries = Helper.getCorrespondingSeries(episode[DBOnlineEpisode.cSeriesID]);

                        if (tmpSeries != null) {
                            // Create different filename for different orders, this will ensure that correct thumbnail is viewed                            
                            string orderIdentifier = ".jpg";
                            string order = tmpSeries[DBOnlineSeries.cChoseEpisodeOrder];

                            if (String.IsNullOrEmpty(order) || order == "Aired")
                                orderIdentifier = ".jpg";
                            else
                                orderIdentifier = "_" + tmpSeries[DBOnlineSeries.cChoseEpisodeOrder] + ".jpg";

                            string seriesFolder = Helper.cleanLocalPath(tmpSeries.ToString());
                            
                            sThumbNailFilename = Helper.PathCombine(seriesFolder, @"Episodes\" + episode[DBOnlineEpisode.cSeasonIndex] + "x" + episode[DBOnlineEpisode.cEpisodeIndex] + orderIdentifier);
                            completePath = Helper.PathCombine(basePath, sThumbNailFilename);

                            if (!File.Exists(completePath)) {
                                MPTVSeriesLog.Write(string.Format("New Episode Image found for \"{0}\": {1}", episode.ToString(), episode[DBOnlineEpisode.cEpisodeThumbnailUrl]), MPTVSeriesLog.LogLevel.Debug);
                                System.Net.WebClient webClient = new System.Net.WebClient();
                                webClient.Headers.Add("user-agent", Settings.UserAgent);
								//webClient.Headers.Add("referer", "http://thetvdb.com/");
                                string url = DBOnlineMirror.Banners + episode[DBOnlineEpisode.cEpisodeThumbnailUrl];
                                try {
                                    Directory.CreateDirectory(Path.GetDirectoryName(completePath));
                                    // Determine if a thumbnail
                                    if (!url.Contains(".jpg")) {
                                        MPTVSeriesLog.Write("Episode Thumbnail location is incorrect: " + url, MPTVSeriesLog.LogLevel.Normal);
                                        episode[DBOnlineEpisode.cEpisodeThumbnailUrl] = "";
                                        episode[DBOnlineEpisode.cEpisodeThumbnailFilename] = "";
                                    } else {
										MPTVSeriesLog.Write("Downloading new Image from: " + url, MPTVSeriesLog.LogLevel.Debug);
                                        webClient.DownloadFile(url, completePath);

                                        m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateEpisodeThumbNails, episode.ToString(), ++nIndex, episodes.Count, episode, completePath));
                                    }
                                    episode.Commit();
                                } catch (System.Net.WebException) {
                                    MPTVSeriesLog.Write("Episode Thumbnail download failed ( " + url + " )");
                                    sThumbNailFilename = "";
                                    // try to delete file if it exists on disk. maybe download was cut short. Re-download next time
                                    try {
                                        System.IO.File.Delete(completePath);
                                    } catch {
                                    }
                                }
                            }
                        }
                    } catch (Exception ex) {
                        MPTVSeriesLog.Write(string.Format("There was a problem getting the episode image: {0}", ex.Message));
                        sThumbNailFilename = "";
                    }
                    episode[DBOnlineEpisode.cEpisodeThumbnailFilename] = sThumbNailFilename;
                    episode.Commit();
                }
                m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateEpisodeThumbNails, episodes.Count));
            }
        }

        void UpdateEpisodeCounts(BackgroundWorker tEpisodeCounts)
        {
            tEpisodeCounts.WorkerReportsProgress = true;

            SQLCondition condEmpty = new SQLCondition();
            List<DBSeries> series = DBSeries.Get(condEmpty);

            if (series.Count > 0)
            {
                tEpisodeCounts.DoWork += new DoWorkEventHandler(asyncEpisodeCounts);
                tEpisodeCounts.RunWorkerCompleted += new RunWorkerCompletedEventHandler(asyncEpisodeCountsCompleted);
                tEpisodeCounts.ProgressChanged += new ProgressChangedEventHandler((s, e) =>
                {
                    if (m_worker.IsBusy)
                    {
                        object[] userState = e.UserState as object[];
                        m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateEpisodeCounts, (userState[0] as DBSeries).ToString(), (int)userState[1], series.Count));
                    }
                });
                tEpisodeCounts.RunWorkerAsync(series);
            }
        }

        void asyncEpisodeCountsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (m_worker.IsBusy)
                m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateEpisodeCounts, (int)e.Result));
            
            MPTVSeriesLog.Write("Update of Episode Counts complete", MPTVSeriesLog.LogLevel.Debug);
        }

        void asyncEpisodeCounts(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;
            List<DBSeries> allSeries = (List<DBSeries>)e.Argument;
            BackgroundWorker worker = sender as BackgroundWorker;
            int nIndex = 1;
            foreach (DBSeries series in allSeries)
            {
                worker.ReportProgress(0, new object[] { series, nIndex++ });
                DBSeries.UpdateEpisodeCounts(series);
            }
            e.Result = allSeries.Count;
        }

        void MediaInfoParse(BackgroundWorker tMediaInfo)
        {
            tMediaInfo.WorkerReportsProgress = true;

            SQLCondition cond = new SQLCondition();
            cond.Add(new DBEpisode(), DBEpisode.cFilename, "", SQLConditionType.NotEqual);
            cond.Add(new DBEpisode(), DBEpisode.cVideoWidth, "0", SQLConditionType.Equal);            
            // Playtime decrements by one every failed attempt(0,-1,-2,..,-5), dont attempt future scans if done more than Maximum attempts
            cond.Add(new DBEpisode(), "localPlaytime", (DBEpisode.MAX_MEDIAINFO_RETRIES*-1), SQLConditionType.GreaterThan); 
            List<DBEpisode> episodes = new List<DBEpisode>();
            // get all the episodes
            episodes = DBEpisode.Get(cond, false);
            
            if (episodes.Count > 0)
            {                
                tMediaInfo.DoWork += new DoWorkEventHandler(asyncReadResolutions);
                tMediaInfo.RunWorkerCompleted += new RunWorkerCompletedEventHandler(asyncReadResolutionsCompleted);
                tMediaInfo.ProgressChanged += new ProgressChangedEventHandler((s, e) =>
                {
                    if (m_worker.IsBusy) { // cannot report progress of finished worker - mepo dies
                        object[] userState = e.UserState as object[];
                        m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.MediaInfo, (userState[0] as DBEpisode)[DBEpisode.cFilenameWOPath], (int)userState[1], episodes.Count));
                    }
                });
                tMediaInfo.RunWorkerAsync(episodes);

            }
            else 
                MPTVSeriesLog.Write("All episodes already contain MediaInfo");
        }

        void asyncReadResolutionsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (m_worker.IsBusy) { // cannot report progress of finished worker - mepo dies
                m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.MediaInfo, (int)e.Result));
            }
            MPTVSeriesLog.Write("Update of MediaInfo complete (processed " + e.Result.ToString() + " files)", MPTVSeriesLog.LogLevel.Debug);
        }

        void asyncReadResolutions(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;
            List<DBEpisode> episodes = (List<DBEpisode>)e.Argument;
            BackgroundWorker w = sender as BackgroundWorker;
            int nIndex = 1;
            foreach (DBEpisode ep in episodes)
            {
                w.ReportProgress(0, new object[] { ep, nIndex++ });
                ep.ReadMediaInfo();
            }
            e.Result = episodes.Count;
        }

        #endregion

        #region SeriesHelpers
        public static DBOnlineSeries SearchForSeries(string seriesName, bool bNoExactMatch, IFeedback feedback)
        {
            string nameToSearch = seriesName;

            while (true) {
                // query online db for possible matches
                GetSeries GetSeriesParser = new GetSeries(nameToSearch);

                // try to find an exact match in our results, if found, return
                //if (DBOption.GetOptions(DBOption.cAutoChooseSeries) == 1) {
                //    foreach (DBOnlineSeries onlineSeries in GetSeriesParser.Results) {
                //        if (!bNoExactMatch && !String.IsNullOrEmpty(onlineSeries[DBOnlineSeries.cPrettyName]) &&
                //           (onlineSeries[DBOnlineSeries.cPrettyName].ToString().Trim().Equals(nameToSearch.Trim().ToLower(), StringComparison.InvariantCultureIgnoreCase))) {
                //            MPTVSeriesLog.Write(string.Format("\"{0}\" was automatically matched to \"{1}\" (SeriesID: {2}), there were a total of {3} matches returned from the Online Database", nameToSearch, onlineSeries.ToString(), onlineSeries[DBOnlineSeries.cID], GetSeriesParser.Results.Count));
                //            return onlineSeries;
                //        }
                //    }
                //}
                if (GetSeriesParser.PerfectMatch != null)
                {
                    MPTVSeriesLog.Write(string.Format("\"{0}\" was automatically matched to \"{1}\" (SeriesID: {2}), there were a total of {3} matches returned from the Online Database", nameToSearch, GetSeriesParser.PerfectMatch.ToString(), GetSeriesParser.PerfectMatch[DBOnlineSeries.cID], GetSeriesParser.Results.Count));
                    return GetSeriesParser.PerfectMatch;
                }

                MPTVSeriesLog.Write(string.Format("Found {0} possible matches for \"{1}\"", GetSeriesParser.Results.Count, nameToSearch));

                // User has four choices:
                // 1) Pick a series from the list
                // 2) Simply skip
                // 3) Skip and never ask for this series again
                // 4) Manually Search

                List<CItem> Choices = new List<CItem>();
                Dictionary<int, DBOnlineSeries> uniqueSeriesIds = new Dictionary<int, DBOnlineSeries>();
                foreach (DBOnlineSeries onlineSeries in GetSeriesParser.Results) // make them unique (each seriesID) - if possible in users lang
                {
                    if (!uniqueSeriesIds.ContainsKey(onlineSeries[DBOnlineSeries.cID]))
                        uniqueSeriesIds.Add(onlineSeries[DBOnlineSeries.cID], onlineSeries);
                    else if (onlineSeries["language"] == Online_Parsing_Classes.OnlineAPI.SelLanguageAsString)
                        uniqueSeriesIds[onlineSeries[DBOnlineSeries.cID]] = onlineSeries;
                }
                foreach (KeyValuePair<int, DBOnlineSeries> onlineSeries in uniqueSeriesIds) {
                    Choices.Add(new CItem(onlineSeries.Value[DBOnlineSeries.cPrettyName],
                        "SeriesID: " + onlineSeries.Value[DBOnlineSeries.cID] + Environment.NewLine +
                        onlineSeries.Value[DBOnlineSeries.cSummary],
                        onlineSeries.Value));
                }

                if (Choices.Count == 0) {
                    Choices.Add(new CItem("No Match Found, Enter Manual Search...", String.Empty, null));
                } else
                    if (!Settings.isConfig)
                        Choices.Add(new CItem("Manual Search...", String.Empty, null));

                ChooseFromSelectionDescriptor descriptor = new ChooseFromSelectionDescriptor();
                descriptor.m_sTitle = "Unable to find matching series";
                descriptor.m_sItemToMatchLabel = "Local series:";
                descriptor.m_sItemToMatch = nameToSearch;
                descriptor.m_sListLabel = "Choose the correct series from this list:";
                descriptor.m_List = Choices;
                descriptor.m_sbtnCancelLabel = "&Skip";
                descriptor.m_sbtnIgnoreLabel = "Skip &Always";

                bool bKeepTrying = true;
                while (bKeepTrying) {
                    CItem Selected = null;
                    ReturnCode result = feedback.ChooseFromSelection(descriptor, out Selected);
                    switch (result) {
                        case ReturnCode.Cancel:
                            MPTVSeriesLog.Write("User cancelled Series Selection");
                            return null;

                        case ReturnCode.Ignore:
                            MPTVSeriesLog.Write("User chose to Ignore \"" + nameToSearch + "\" in the future");
                            nameToSearch = null;
                            DBSeries series = new DBSeries(seriesName);
                            series[DBSeries.cScanIgnore] = 1; // means it will be skipped in the future
                            series[DBSeries.cHidden] = true;
                            series.Commit();
                            return null;

                        case ReturnCode.OK:
                            DBOnlineSeries selectedSeries = Selected.m_Tag as DBOnlineSeries;

                            // Show the Virtual Keyboard to manual enter in name to search                            
                            if (selectedSeries == null && !Settings.isConfig) {
                                GetStringFromUserDescriptor Keyboard = new GetStringFromUserDescriptor();
								Keyboard.KeyboardStyle = (GetStringFromUserDescriptor.KeyboardStyles)(int)DBOption.GetOptions(DBOption.cKeyboardStyle);
								Keyboard.Text = nameToSearch;

								if (feedback.GetStringFromUser(Keyboard, out nameToSearch) == ReturnCode.OK) {
                                    // Search again using manually entered name
                                    bKeepTrying = false;
                                } else {
                                    MPTVSeriesLog.Write("User cancelled Series Selection");
                                    return null;
                                }
                            } else if (nameToSearch != Selected.m_sName || selectedSeries == null) {
                                nameToSearch = Selected.m_sName;
                                bKeepTrying = false;
                            } else {
                                MPTVSeriesLog.Write(string.Format("\"{0}\" was manually matched to \"{1}\" (SeriesID: {2})", nameToSearch, selectedSeries.ToString(), selectedSeries[DBOnlineSeries.cID]));
                                return selectedSeries;
                            }
                            break;

                        case ReturnCode.NotReady: {
                                // plugin's not loaded (yet?) so wait and ask again later
                                Thread.Sleep(2000);
                            }
                            break;
                    }
                }
                if (!bKeepTrying)
                    MPTVSeriesLog.Write("User typed a new Search term: \"" + nameToSearch + "\"", MPTVSeriesLog.LogLevel.Debug);
            }
        }

        private void determineOrderOption(DBSeries series)
        {
            try {
                if (series[DBOnlineSeries.cChoseEpisodeOrder] == null)
                {
                    List<string> episodeOrders = new List<string>(series[DBOnlineSeries.cEpisodeOrders].ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
                    if (episodeOrders.Count > 1 && (DBOption.GetOptions(DBOption.cAutoChooseOrder) == 0))
                    {
                        MPTVSeriesLog.Write(string.Format("\"{0}\" supports {1} different ordering options, asking user...", series.ToString(), episodeOrders.Count), MPTVSeriesLog.LogLevel.Debug);
                        // let the user choose
                        string helpText = "Some series expose several ways in which they are ordered, for instance a DVD-release may differ from the original Air schedule." + Environment.NewLine +
                                          "Note that your file numbering must match the option you choose here." + Environment.NewLine +
                                          "Choose the default \"Aired\" option unless you have a specific reason not to!";

                        List<CItem> Choices = new List<CItem>();
                        foreach (string orderOption in episodeOrders)
                            Choices.Add(new CItem(orderOption, helpText, orderOption));

                        ChooseFromSelectionDescriptor descriptor = new ChooseFromSelectionDescriptor();
                        descriptor.m_sTitle = "Multiple ordering Options detected";
                        descriptor.m_sItemToMatchLabel = "The following Series supports multiple Order Options:";
                        descriptor.m_sItemToMatch = series[DBOnlineSeries.cPrettyName];
                        descriptor.m_sListLabel = "Please choose the desired Option:";
                        descriptor.m_List = Choices;
                        descriptor.m_useRadioToSelect = true;
                        descriptor.m_allowAlter = false;

                        CItem selectedOrdering = null;
                        ReturnCode result = m_feedback.ChooseFromSelection(descriptor, out selectedOrdering);
                        if (result == ReturnCode.OK)
                        {
                            series[DBOnlineSeries.cChoseEpisodeOrder] = (string)selectedOrdering.m_Tag;
                            MPTVSeriesLog.Write(string.Format("{0} order option chosen for series \"{1}\"", (string)selectedOrdering.m_Tag, series.ToString()), MPTVSeriesLog.LogLevel.Normal);
                        }
                    }
                    else
                    {
                        if (series[DBOnlineSeries.cEpisodeOrders] != "")
                            series[DBOnlineSeries.cChoseEpisodeOrder] = "Aired";
                        MPTVSeriesLog.Write(string.Format("Aired order option chosen for series \"{0}\"", series.ToString()), MPTVSeriesLog.LogLevel.Normal);
                    }
                }

            } catch (Exception) {

            }
            // End support for ordering
        }

        #endregion

        #region EpisodeHelpers
        private static void matchOnlineToLocalEpisodes(DBSeries series, List<DBEpisode> episodesList, GetEpisodes episodesParser)
        {
            if (episodesList == null || episodesList.Count == 0)
                return;

            foreach (DBEpisode localEpisode in episodesList) {
                bool bMatchFound = false;
                foreach (DBOnlineEpisode onlineEpisode in episodesParser.Results) {
                    if ((int)localEpisode[DBEpisode.cSeriesID] == (int)onlineEpisode[DBOnlineEpisode.cSeriesID]) {
                        if (matchOnlineToLocalEpisode(series, localEpisode, onlineEpisode, null, true) == 0) 
                        {
                            commitOnlineToLocalEpisisodeMatch(series, localEpisode, onlineEpisode);
                            bMatchFound = true;
                            break;
                        }
                    }
                }
                if (!bMatchFound)
                    MPTVSeriesLog.Write(localEpisode[DBEpisode.cFilename].ToString() + " could not be matched online, check that the online database has this episode.");
            }
        }


        private static void commitOnlineToLocalEpisisodeMatch(DBSeries series, DBEpisode localEpisode, DBOnlineEpisode onlineEpisode)
        {
            // season update for online data
            DBSeason season = new DBSeason(series[DBSeries.cID], onlineEpisode[DBOnlineEpisode.cSeasonIndex]);
            season[DBSeason.cHasEpisodes] = true;
            season.Commit();

            // update data
            localEpisode[DBOnlineEpisode.cID] = onlineEpisode[DBOnlineEpisode.cID];
            if (localEpisode[DBOnlineEpisode.cEpisodeName].ToString().Length == 0)
                localEpisode[DBOnlineEpisode.cEpisodeName] = onlineEpisode[DBOnlineEpisode.cEpisodeName];

            // go over all the fields, (and update only those which haven't been modified by the user - will do that later)
            foreach (String key in onlineEpisode.FieldNames)
            {
                switch (key)
                {
                    case DBOnlineEpisode.cCompositeID:
                    case DBEpisode.cSeriesID:
                    case DBOnlineEpisode.cWatched:
                    case DBOnlineEpisode.cHidden:
                    case DBOnlineEpisode.cDownloadPending:
                    case DBOnlineEpisode.cDownloadExpectedNames:
                    case DBOnlineEpisode.cMyRating:
                        // do nothing here, those information are local only
                        break;

                    case DBOnlineEpisode.cSeasonIndex:
                    case DBOnlineEpisode.cEpisodeIndex:
                        break; // those must not get overwritten from what they were set to by getEpisodes (because of different order options)

                    case DBOnlineEpisode.cEpisodeThumbnailFilename:
                        // Dont reset as Thumbnail update may not be called in some situations
                        break;

                    default:
                        localEpisode.onlineEpisode.AddColumn(key, new DBField(DBField.cTypeString));
                        localEpisode[key] = onlineEpisode[key];
                        break;
                }
            }
            MPTVSeriesLog.Write("ccid l: " + localEpisode[DBEpisode.cCompositeID].ToString(), MPTVSeriesLog.LogLevel.Debug);
            MPTVSeriesLog.Write("ccid o: " + onlineEpisode[DBOnlineEpisode.cCompositeID].ToString(), MPTVSeriesLog.LogLevel.Debug);
            MPTVSeriesLog.Write("ccid on: " + localEpisode.onlineEpisode[DBOnlineEpisode.cCompositeID].ToString(), MPTVSeriesLog.LogLevel.Debug);
            localEpisode[DBOnlineEpisode.cOnlineDataImported] = 1;
            MPTVSeriesLog.Write("\"" + localEpisode.ToString() + "\" identified");
            localEpisode.Commit();            
        }


        public static int matchOnlineToLocalEpisode(DBSeries series, DBEpisode localEpisode, DBOnlineEpisode onlineEpisode, string orderingOption, bool changedOnline)
        {
            // TODO: Enable this for any possible field in localepisode (from parsing)
            // just look for a corresponding field in onlineepisode, and depending on the type do a numerical perfect check, or a fuzzy text match
            // also recognize dates
            // also, if  the orderingoptions string should not be passed in here, we should try all fields (perhaps with a certain hardcoded order) and sum up the confidences (or rather errormsg)
            // for instance we could auto detect dvd-ordered local eps, because in aired only the 1x01 will pass, but in dvd both the 1x01 and the title will pass
            // this will also enable fields such as onlineEpisodeIds to be read out from the filename (or possibly in the future nfo files) and be 100% auto matched

            // lastly, this should never overwrite the online-ep ids, instead we should overwrite the local ep ids (but not here) to always the same as the onlineep ids (aired ordering)
            // we can get different ordering on the fly inside the plugin this way

            try
            {
                orderingOption = orderingOption == null ? (string)series[DBOnlineSeries.cChoseEpisodeOrder] : orderingOption;
                switch (orderingOption)
                {
                    case "":
                    case "Aired":
                        int iEpIndex2 = 0;
                        if ((int)localEpisode[DBEpisode.cEpisodeIndex2] == 0)
                        {
                            // Don't want to match local episodes with no EpisodeIndex2 with an online episode index of zero
                            iEpIndex2 = -1;
                        }

                        if ((int)localEpisode[DBEpisode.cSeasonIndex] == (int)onlineEpisode[DBOnlineEpisode.cSeasonIndex] &&
                                ((int)localEpisode[DBEpisode.cEpisodeIndex] == (int)onlineEpisode[DBOnlineEpisode.cEpisodeIndex] ||
                                iEpIndex2 == (int)onlineEpisode[DBOnlineEpisode.cEpisodeIndex]))
                        {
                            return 0;
                        }
                        else return int.MaxValue;

                    case "DVD":
                        System.Globalization.NumberFormatInfo provider = new System.Globalization.NumberFormatInfo();
                        provider.NumberDecimalSeparator = ".";
                        int localSeason = (int)localEpisode[DBEpisode.cSeasonIndex];
                        float onlineSeasonTemp;
                        int onlineSeason = -1;
                        if (float.TryParse(onlineEpisode["DVD_season"], System.Globalization.NumberStyles.AllowDecimalPoint, provider, out onlineSeasonTemp))
                            onlineSeason = (int)onlineSeasonTemp;

                        int localEp = (int)localEpisode[DBEpisode.cEpisodeIndex];
                        int localEp2 = (int)localEpisode[DBEpisode.cEpisodeIndex2];

                        if (String.IsNullOrEmpty(localEpisode[DBEpisode.cEpisodeIndex2]))
                            localEp2 = 0;
                        float onlineEp = -1;


                        if (onlineSeason != -1 && float.TryParse(onlineEpisode["DVD_episodenumber"], System.Globalization.NumberStyles.AllowDecimalPoint, provider, out onlineEp))
                        {
                            //MPTVSeriesLog.Write(string.Format("Series {0} , localEp {1} localEp2 {2} onlineEp {3}", onlineSeason, localEp, localEp2, onlineEp));
                            /*if (!String.IsNullOrEmpty(onlineEpisode["DVD_season"]) && !String.IsNullOrEmpty(onlineEpisode["DVD_season"]) &&
                                (localSeason == onlineSeason && ((int)localEp == (int)onlineEp || (int)localEp2 == -1 ? false : (int)localEp2 == (int)onlineEp)))
                            */

                            //if(localEp == (int)onlineEp)
                            string localstring;
                            double localcomp;
                            localstring = (localEp.ToString() + "." + localEp2.ToString());
                            localcomp = Convert.ToDouble(localstring, provider);
                            if (!String.IsNullOrEmpty(onlineEpisode["DVD_season"]) && !String.IsNullOrEmpty(onlineEpisode["DVD_episodenumber"]) && (localSeason == onlineSeason && (localcomp == onlineEp || localEp == (int)onlineEp)))
                            {
                                /*check that the vital parts exist DVD_season and DVD_episodenumber, then check to see if we have a match either for the full
                                 possible online format of X.Y via the use of localcomp and some string combinations, or through the default style of X.0 
                                 via integer comparison*/
                                // overwrite onlineEps season/ep #
                                if (changedOnline)
                                {
                                    onlineEpisode[DBOnlineEpisode.cSeasonIndex] = (int)localEpisode[DBEpisode.cSeasonIndex];
                                    if (localcomp == onlineEp)
                                    {
                                        MPTVSeriesLog.Write(string.Format("Episode {0} matched to episode {1}", localEp, onlineEp), MPTVSeriesLog.LogLevel.Debug);
                                        onlineEpisode[DBEpisode.cEpisodeIndex] = localcomp;
                                    }
                                    else if (localEp == (int)onlineEp)
                                    {
                                        onlineEpisode[DBEpisode.cEpisodeIndex] = localEp;
                                    }
                                }
                                return 0;
                            }
                            else
                            {
                                MPTVSeriesLog.Write(string.Format("File does not match current parse Series: {0} Episode: {1} : Online Episode: {2}", localSeason, localcomp, onlineEp), MPTVSeriesLog.LogLevel.Debug);
                                return int.MaxValue;
                            }

                        }
                        break;
                    case "Absolute":

                        System.Globalization.NumberFormatInfo provided = new System.Globalization.NumberFormatInfo();
                        float onlineabs = -1;
                        float onlineabsTemp;
                        if (float.TryParse(onlineEpisode["absolute_number"], System.Globalization.NumberStyles.AllowDecimalPoint, provided, out onlineabsTemp))
                            onlineabs = onlineabsTemp;
                        MPTVSeriesLog.Write(string.Format("Absolute number: {0}", onlineabs), MPTVSeriesLog.LogLevel.Debug);
                        if (onlineabs != -1)
                        {
                            double localabs = -1;
                            if ((int)localEpisode[DBEpisode.cSeasonIndex] == 0)
                            {
                                /*Now we have to figure out whether we are at ep 100 or more*/
                                localabs = Convert.ToDouble(localEpisode[DBEpisode.cSeasonIndex].ToString() + localEpisode[DBEpisode.cEpisodeIndex].ToString());
                            }
                            else if ((int)localEpisode[DBEpisode.cSeasonIndex] >= 1 && (int)localEpisode[DBEpisode.cEpisodeIndex] < 10 /*&& String.IsNullOrEmpty(localEpisode[DBEpisode.cEpisodeIndex2])*/)
                            {/* Any episode X0[0-9] should be combined in this manner */
                                localabs = Convert.ToDouble(localEpisode[DBEpisode.cSeasonIndex].ToString() + "0" + localEpisode[DBEpisode.cEpisodeIndex].ToString());
                            }
                            else if ((int)localEpisode[DBEpisode.cSeasonIndex] >= 1 && (int)localEpisode[DBEpisode.cEpisodeIndex] >= 10 /*&& String.IsNullOrEmpty(localEpisode[DBEpisode.cEpisodeIndex2])*/)
                            {/* All other episodes should fall into this category */
                                localabs = Convert.ToDouble(localEpisode[DBEpisode.cSeasonIndex].ToString() + localEpisode[DBEpisode.cEpisodeIndex].ToString());
                            }

                            float.TryParse(onlineEpisode["absolute_number"], System.Globalization.NumberStyles.AllowDecimalPoint, provided, out onlineabs);
                            if (localabs == onlineabs)
                            {
                                if (changedOnline)
                                {
                                    localEpisode[DBEpisode.cSeasonIndex] = 1;
                                    localEpisode[DBEpisode.cEpisodeIndex] = (int)onlineabs;
                                }
                                MPTVSeriesLog.Write(string.Format("Matched Absolute Ep {0} to local ep {1}x{2}", onlineabs, series, localEpisode), MPTVSeriesLog.LogLevel.Debug);
                                return 0;
                            }
                            else
                            {
                                MPTVSeriesLog.Write(string.Format("Failed to Match local ep {1}x{2} to Absolute ep {0}", onlineabs, series, localEpisode), MPTVSeriesLog.LogLevel.Debug);
                                return int.MaxValue;
                            }
                        }
                        break;
                    case "Title":
                        int fuzzyness = 3;
                        string localTitle = localEpisode[DBEpisode.cEpisodeName];
                        string onlineTitle = onlineEpisode[DBOnlineEpisode.cEpisodeName];
                        if (string.IsNullOrEmpty(localTitle) || string.IsNullOrEmpty(onlineTitle)) return int.MaxValue;
                        double maxDistance = Math.Min(localTitle.Length, onlineTitle.Length) * 0.1 + fuzzyness;
                        int isDistance = MediaPortal.Util.Levenshtein.Match(localTitle.ToLower(), onlineTitle.ToLower());
                        if (isDistance > maxDistance) return int.MaxValue; // we dont return
                        else return isDistance;
                }          
            }
            catch (Exception ex)
            {
                // was getting strange results from API where online ep was greater than Int32!
                MPTVSeriesLog.Write("Error: Exception generated whilst matching local episode [{0}x{1}] to online episode [{2}x{3}]: {4}", 
                    localEpisode[DBEpisode.cSeasonIndex], localEpisode[DBEpisode.cEpisodeIndex],
                    onlineEpisode[DBOnlineEpisode.cSeasonIndex], onlineEpisode[DBOnlineEpisode.cEpisodeIndex],
                    ex.Message);                
            }
            return int.MaxValue;
        }
        #endregion

        #region ParseLocal
        /// <summary>
        /// sets:
        ///  - DBEpisode.cImportProcessed to 1
        ///  - DBSeason.cHasLocalFilesTemp to true
        ///  - DBSeries.cHasLocalFilesTemp to true
        /// </summary>
        /// <param name="filenames"></param>
        private static void UpdateStatus(List<string> filenames)
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

            foreach (SQLCondition condition in importProcessedConds)
            {
                DBEpisode.GlobalSet(DBEpisode.cImportProcessed, 1, condition);                
                DBEpisode.GlobalSet(DBEpisode.cIsAvailable, 1, condition);
            }

            SQLCondition condSeason = new SQLCondition();
            condSeason.AddCustom(" exists( select " + DBEpisode.Q(DBEpisode.cFilename) + " from " + DBEpisode.cTableName
                            + " where " + DBEpisode.cSeriesID + " = " + DBSeason.Q(DBSeason.cSeriesID) + " and "
                            + DBEpisode.cSeasonIndex + " = " + DBSeason.Q(DBSeason.cIndex) + " and " + DBEpisode.Q(DBEpisode.cImportProcessed) + " = 1 "   + ")");
            DBSeason.GlobalSet(DBSeason.cHasLocalFilesTemp, true, condSeason); //takes forever

            SQLCondition condSeries = new SQLCondition();
            condSeries.AddCustom(" exists( select " + DBEpisode.Q(DBEpisode.cFilename) + " from " + DBEpisode.cTableName
                            + " where " + DBEpisode.cSeriesID + " = " + DBOnlineSeries.Q(DBOnlineSeries.cID) +
                            " and " + DBEpisode.Q(DBEpisode.cImportProcessed) + " = 1 " + ")");
            DBSeries.GlobalSet(DBOnlineSeries.cHasLocalFilesTemp, true, condSeries); //takes about 1/4 of the time of above
        }

        /// <summary>
        /// Removes entries from haystack that are already in db
        /// </summary>
        /// <param name="haystack"></param>
        /// <returns>Returns a list of Files Removed</returns>
        public static List<string> RemoveFilesInDB(ref IList<parseResult> haystack)
        {
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
            for (int i = 0; i < haystack.Count; i++)
            {
                if (dbEps.Contains(haystack[i].full_filename))
                {
                    updateStatusEps.Add(haystack[i].full_filename);
                    haystack.RemoveAt(i);
                    i--;
                }
            }
            return updateStatusEps;
        }

        private void ParseLocal(List<PathPair> files)
        {
            ParseLocal(files, null);
        }
        private void ParseLocal(List<PathPair> files, IList<parseResult> UserModifiedParsedResults)
        {
            bool resultsFromUser = UserModifiedParsedResults != null;
            MPTVSeriesLog.Write(bigLogMessage("Gathering Local Information"));
            // dont parse if we get results from user (already done)
            //IList<parseResult> parsedFiles = resultsFromUser ? UserModifiedParsedResults : LocalParse.Parse(files, false);            
            IList<parseResult> parsedFiles = LocalParse.Parse(files, true);

            // get a list of files that are on disk and still in database
            // also get a list of new files for import returned in parsedFiles
            var updateStatusEps = RemoveFilesInDB(ref parsedFiles);
            // update the importprocessed state for files we previously set as not processed
            UpdateStatus(updateStatusEps);

            // if we have user modified parse results, use these instead
            // user may have manually input for series/season/episodes.
            if (resultsFromUser)
                parsedFiles = UserModifiedParsedResults;

            MPTVSeriesLog.Write("Adding " + parsedFiles.Count.ToString() + " new file(s) to Database");
            
            int nSeason = 0;
            List<DBSeries> relatedSeries = new List<DBSeries>();
            List<DBSeason> relatedSeasons = new List<DBSeason>();

            int nIndex = 0;
            foreach (parseResult progress in parsedFiles)
            {
                if (m_worker.CancellationPending)
                    return;
                if (progress.success)
                {
                    DBSeries series = null;
                    DBSeason season = null;
                    if (progress.parser.Matches.ContainsKey(DBOnlineEpisode.cFirstAired))
                    {
                        // series first
                        series = new DBSeries(progress.parser.Matches[DBSeries.cParsedName]);
                        series[DBOnlineSeries.cHasLocalFilesTemp] = 1;
                        // not much to do here except commiting the series
                        series.Commit();
                    }
                    else
                    {
                        nSeason = Convert.ToInt32(progress.parser.Matches[DBEpisode.cSeasonIndex]);

                        // ok, we are sure it's valid now
                        // series first
                        series = new DBSeries(progress.parser.Matches[DBSeries.cParsedName]);
                        series[DBOnlineSeries.cHasLocalFilesTemp] = 1;
                        // not much to do here except commiting the series
                        series.Commit();

                        // season now
                        season = new DBSeason(series[DBSeries.cID], nSeason);
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
                    DBEpisode episode = new DBEpisode(progress.full_filename, true);
                    bool bNewFile = false;
                    if (episode[DBEpisode.cImportProcessed] != 2) {
                        m_bDataUpdated = true;
                        bNewFile = true;
                    }

                    episode[DBEpisode.cImportProcessed] = 1;                    
                    episode[DBEpisode.cIsAvailable] = 1;
                    
                    episode[DBEpisode.cSeriesID] = series[DBSeries.cID];
                    if (progress.parser.Matches.ContainsKey(DBEpisode.cEpisodeIndex2))
                    {
                        episode[DBEpisode.cEpisodeIndex2] = progress.parser.Matches[DBEpisode.cEpisodeIndex2];
                        episode[DBEpisode.cCompositeID2] = episode[DBEpisode.cSeriesID] + "_" + nSeason + "x" + episode[DBEpisode.cEpisodeIndex2];

                        if (bNewFile) {
                            //if it's a new file but the DBOnlineEpisode already has the online ID is set, then ensure that the episode is not hidden
                            DBOnlineEpisode doubleEp = new DBOnlineEpisode(episode[DBEpisode.cSeriesID], nSeason, episode[DBEpisode.cEpisodeIndex2]);
                            if (doubleEp[DBOnlineEpisode.cID] > 0 && doubleEp[DBOnlineEpisode.cHidden] == 1) {
                                doubleEp[DBOnlineEpisode.cHidden] = 0;
                                doubleEp.Commit();
                            }
                        }
                    }

                    //episode[DBEpisode.cAvailableSubtitles] = episode.checkHasSubtitles();
                    
                    foreach (KeyValuePair<string, string> match in progress.parser.Matches)
                    {
                        if (match.Key != DBSeries.cParsedName)
                        {
                            episode.AddColumn(match.Key, new DBField(DBField.cTypeString));
                            if (bNewFile || (episode[match.Key] != null && episode[match.Key] != match.Value))
                                episode[match.Key] = match.Value;
                        }
                    }

                    //if it's a new file but the DBOnlineEpisode already has the online ID is set, then ensure that the episode is not hidden
                    if (bNewFile && episode[DBOnlineEpisode.cID] > 0) {
                        episode[DBOnlineEpisode.cHidden] = 0;
                    }

                    // newly added should have watched to false
                    episode[DBOnlineEpisode.cWatched] = false;

                    episode.Commit();

                    // reloads the episode, in order to get a proper link on the onlineEpisode, and set the PendingDownload to 0 
                    episode = new DBEpisode(progress.full_filename, false);
                    episode[DBOnlineEpisode.cDownloadPending] = 0;
                    episode.Commit();
                }

                if(++nIndex % 25 == 0)
                    m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.LocalScan, progress.match_filename, nIndex, parsedFiles.Count));
            }

            //now make DBOnlineEpisodes for each unmatched double episode
            //fetch all the episodes that have a CompositeID2 with no matching DBOnlineEpisode
            SQLCondition cond = new SQLCondition(new DBEpisode(), DBEpisode.cCompositeID2, " ", SQLConditionType.NotEqual);
            cond.AddCustom(DBOnlineEpisode.cTableName + "." + DBOnlineEpisode.cCompositeID + " is null");
            List<DBEpisode> missingDoubleEpisodes = DBEpisode.Get(cond, false, true);
            //now foreach episode make a DBOnlineEpisode
            foreach (DBEpisode episode in missingDoubleEpisodes) {
                DBOnlineEpisode onlineEpisode = new DBOnlineEpisode(episode[DBEpisode.cSeriesID], episode[DBEpisode.cSeasonIndex], episode[DBEpisode.cEpisodeIndex2]);
                onlineEpisode.Commit();
            }

            // now go over the touched seasons & series
            foreach (DBSeason season in relatedSeasons)
            {
                DBEpisode episode = DBEpisode.GetFirstUnwatched(season[DBSeason.cSeriesID], season[DBSeason.cIndex]);
                if (episode != null)
                    season[DBSeason.cUnwatchedItems] = true;
                else
                    season[DBSeason.cUnwatchedItems] = false;

                season[DBSeason.cHidden] = 0;
                season.Commit();
            }

            foreach (DBSeries series in relatedSeries)
            {
                DBEpisode episode = DBEpisode.GetFirstUnwatched(series[DBSeries.cID]);
                if (episode != null)
                    series[DBOnlineSeries.cUnwatchedItems] = true;
                else
                    series[DBOnlineSeries.cUnwatchedItems] = false;

                series[DBSeries.cHidden] = 0;
                series.Commit();
            }

            m_worker.ReportProgress(0, new ParsingProgress(ParsingAction.LocalScan, parsedFiles.Count));
        }
        #endregion

        #region HelperFunctions
        static List<string> generateIDListOfString<T>(List<T> entities, string fieldname) where T : DBTable
        {
            // generate a comma separated list of all the ids
            List<String> sSeriesIDs = new List<String>(entities.Count);
            if (entities.Count > 0)
                foreach (DBTable entity in entities)
                    sSeriesIDs.Add(entity[fieldname].ToString());
            return sSeriesIDs;
        }
        #endregion

        #region MessageFormatters
        static string bigLogMessage(string msg)
        {
            return string.Format("***************     {0}     ***************", msg);
        }

        static string prettyStars(int length)
        {
            StringBuilder b = new StringBuilder(length);
            for (int i = 0; i < length; i++) b.Append('*');
            return b.ToString();
        }
        #endregion
    }

    class testing
    {
        static List<string> generateIDListOfString<T>(List<T> entities, string fieldname) where T : DBTable
        {
            // generate a comma separated list of all the ids
            List<String> sSeriesIDs = new List<String>(entities.Count);
            if (entities.Count > 0)
                foreach (DBTable entity in entities)
                    sSeriesIDs.Add(entity[fieldname].ToString());
            return sSeriesIDs;
        }

        static string prettyStars(int length)
        {
            StringBuilder b = new StringBuilder(length);
            for (int i = 0; i < length; i++)
                b.Append('*');
            return b.ToString();
        }

        static string bigLogMessage(string msg)
        {
            return string.Format("***************     {0}     ***************", msg);
        }

    }
}
