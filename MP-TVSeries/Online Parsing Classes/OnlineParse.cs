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
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using TraktAPI.DataStructures;
using WindowPlugins.GUITVSeries.Feedback;
using WindowPlugins.GUITVSeries.Online_Parsing_Classes;

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
        CheckArtwork,

        GetOnlineUpdates,
        UpdateSeries,
        UpdateEpisodes,
        CleanupEpisodes,
        UpdateEpisodeCounts,
        UpdateCommunityRatings,
        UpdateUserRatings,
        UpdateBanners,
        UpdateFanart,

        GetNewBanners,
        GetNewFanArt,
        GetNewActors,
        UpdateEpisodeThumbNails,
        UpdateUserFavourites,

        UpdateRecentlyAdded
    }

    public class ParsingProgress
    {
        public ParsingAction CurrentAction { get; set; }
        public string CurrentProgress { get; set; }
        public int CurrentItem { get; set; }
        public int TotalItems { get; set; }
        public DBTable Details { get; set; }
        public string Picture { get; set; }

        public ParsingProgress(ParsingAction aCurrentAction, int aTotalItems)
            : this(aCurrentAction, null, -1, aTotalItems) { }

        public ParsingProgress(ParsingAction aCurrentAction, string aCurrentItem, int aCurrentProgress, int aTotalItems) 
            : this(aCurrentAction, aCurrentItem, aCurrentProgress, aTotalItems, null, null) { }

        public ParsingProgress(ParsingAction aCurrentAction, string aCurrentItem, int aCurrentProgress, int aTotalItems, DBTable aDetails, string aPicture)
        {
            this.CurrentAction = aCurrentAction;
            this.CurrentProgress = aCurrentItem;
            this.CurrentItem = aCurrentProgress;
            this.TotalItems = aTotalItems;
            this.Details = aDetails;
            this.Picture = aPicture;
        }
    }

    public class CParsingParameters
    {
        private static List<ParsingAction> mFirstLocalScanActions = new List<ParsingAction> { 
            ParsingAction.LocalScan, 
            ParsingAction.MediaInfo,
            ParsingAction.IdentifyNewSeries, 
            ParsingAction.IdentifyNewEpisodes,
            ParsingAction.UpdateEpisodeCounts,
            ParsingAction.CheckArtwork
        };

        private static List<ParsingAction> mOnlineRefreshActions = new List<ParsingAction> { 
            ParsingAction.GetOnlineUpdates, 
            ParsingAction.UpdateSeries, 
            ParsingAction.UpdateEpisodes, 
            ParsingAction.UpdateUserRatings, 
            ParsingAction.UpdateBanners, 
            ParsingAction.UpdateFanart, 
            ParsingAction.GetNewBanners, 
            ParsingAction.GetNewFanArt,
            ParsingAction.GetNewActors,
            ParsingAction.CleanupEpisodes,
            ParsingAction.UpdateCommunityRatings,
            ParsingAction.UpdateEpisodeThumbNails,
            ParsingAction.UpdateUserFavourites,
            ParsingAction.UpdateRecentlyAdded,
            ParsingAction.UpdateEpisodeCounts
		};

        public List<ParsingAction> Actions = new List<ParsingAction>();

        public bool IsLocalScan = true;
        public bool IsUpdateScan = true;
        public List<PathPair> Files = null;

        public List<int> Series = null;
        public List<DBValue> Episodes = null;

        public UserInputResults UserInputResult = null;
        public IEpisodeMatchingFeedback UserEpisodeMatcher = null;

        public CParsingParameters(bool aScanNew, bool aUpdateExisting)
        {
            IsLocalScan = aScanNew;
            IsUpdateScan = aUpdateExisting;

            if (IsLocalScan)
                Actions.AddRange(mFirstLocalScanActions);

            if (IsUpdateScan)
                Actions.AddRange(mOnlineRefreshActions);
        }

        public CParsingParameters(ParsingAction aAction, List<PathPair> aFiles, bool aScanNew, bool aUpdateExisting)
        {
            IsLocalScan = aScanNew;
            IsUpdateScan = aUpdateExisting;

            Actions.Add(aAction);
            if (aAction == ParsingAction.List_Add) {
                Actions.Add(ParsingAction.MediaInfo);
                Actions.Add(ParsingAction.IdentifyNewSeries);
                Actions.Add(ParsingAction.IdentifyNewEpisodes);
                Actions.Add(ParsingAction.UpdateEpisodeCounts);                
            }
            
            if (aAction == ParsingAction.List_Remove)
            { 
                Actions.Add(ParsingAction.UpdateEpisodeCounts); 
            }
            
            Files = aFiles;

            if (IsLocalScan)
                Actions.AddRange(mFirstLocalScanActions);
            if (IsUpdateScan)
                Actions.AddRange(mOnlineRefreshActions);
        }

        public CParsingParameters(IEnumerable<ParsingAction> aActions, List<int> aSeries, List<DBValue> aEpisodes)
        {
            Actions.AddRange(aActions);
            Episodes = aEpisodes;
            Series = aSeries;
        }

        public CParsingParameters(bool aScanNew, bool aUpdateExisting, UserInputResults aUserInputResult, IEpisodeMatchingFeedback aUserEpisodeMatcher)
            : this(aScanNew, aUpdateExisting)
        {
            this.UserInputResult = aUserInputResult;
            this.UserEpisodeMatcher = aUserEpisodeMatcher;
        }
    };

    public class OnlineParsing
    {
        public bool OnlineUpdateNeeded = false;
        public bool WasOnlineUpdate = false;
        public BackgroundWorker Worker = new BackgroundWorker();

        public static bool DataUpdated = false;
        public static bool IsMainOnlineParseComplete = true; // not including extra background threads

        public delegate void OnlineParsingProgressHandler(int nProgress, ParsingProgress Progress);
        public delegate void OnlineParsingCompletedHandler(bool newLocalFiles);

        /// <summary>
        /// This will be triggered once all the SeriesAndEpisodeInfo has been parsed completely.
        /// </summary>        
        public static event OnlineParsingCompletedHandler OnlineParsingCompleted;
        public static event OnlineParsingProgressHandler OnlineParsingProgress;

        private const int RETRY_INTERVAL = 1000;
        private const int RETRY_MULTIPLIER = 2;
        private const int MAX_TIMEOUT = 120000;

        private bool mNewLocalFiles = false;        
        private bool mFullSeriesRetrieval = false;
        private bool mNoExactMatch = false;       // if set to true then the user will be always prompted to choose the series
        private readonly IFeedback mFeedback = null;
        private CParsingParameters mParsingParameters = null;
        private DateTime mLastOnlineMirrorUpdate = DateTime.MinValue;
        private readonly Dictionary<string, List<DBOnlineEpisode>> mOnlineEpisodes = new Dictionary<string, List<DBOnlineEpisode>>();

        public OnlineParsing(IFeedback feedback)
        {
            mFeedback = feedback;
            Worker.WorkerReportsProgress = true;
            Worker.WorkerSupportsCancellation = true;
            Worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            Worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
        }

        void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Run an Online update when needed
            if (OnlineUpdateNeeded && !WasOnlineUpdate)
            {
                MPTVSeriesLog.Write("Import Worker completed, online update is required.", MPTVSeriesLog.LogLevel.Normal);
                OnlineUpdateNeeded = false;
                TVSeriesPlugin.m_LastUpdateScan = DateTime.Now;
                Start(new CParsingParameters(false, true));
            }
            else
            {
                if (OnlineParsingCompleted != null) // only if any subscribers exist
                {
                    OnlineParsingCompleted.Invoke(mNewLocalFiles);
                }
                mNewLocalFiles = false;
            }
        }

        void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (OnlineParsingProgress != null) // only if any subscribers exist
                OnlineParsingProgress.Invoke(e.ProgressPercentage, e.UserState as ParsingProgress);
        }

        public bool IsWorking
        {
            get
            {
                return Worker.IsBusy;
            }
        }

        public bool Start(CParsingParameters param)
        {
            WasOnlineUpdate=false;

            // check if we are doing any online refresh actions
            // we dont want to double up actions later            
            if (param.Actions.Contains(ParsingAction.GetOnlineUpdates)) {
                WasOnlineUpdate = true;
            }

            if (!Worker.IsBusy)
            {
                Worker.RunWorkerAsync(param);
                return true;
            }
            return false;
        }

        public bool LocalScan
        {
            get { if (mParsingParameters != null) return mParsingParameters.IsLocalScan; else return false; }
        }

        public bool UpdateScan
        {
            get { if (mParsingParameters != null) return mParsingParameters.IsUpdateScan; else return false; }
        }

        public void Cancel()
        {
            Worker.CancelAsync();
        }

        public void Worker_DoWork(object sender, DoWorkEventArgs e)
        {            
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            
            mParsingParameters = e.Argument as CParsingParameters;
            mFullSeriesRetrieval = DBOption.GetOptions(DBOption.cFullSeriesRetrieval);
            mNoExactMatch = false;
            Worker.ReportProgress(0);

            IsMainOnlineParseComplete = false;

            TVSeriesPlugin.IsResumeFromStandby = false;

            BackgroundWorker lMediaInfoWorker = null;
            BackgroundWorker lEpisodeCounterWorker = null;
            BackgroundWorker lUserRatingsWorker = null;
            BackgroundWorker lTraktCommunityRatingsWorker = null;

            // remove first update episode count as this is added twice in some Import Actions
            // we could do distinct() but that removes last
            if (mParsingParameters.Actions.Count(pa => pa == ParsingAction.UpdateEpisodeCounts) == 2)
                mParsingParameters.Actions.Remove(ParsingAction.UpdateEpisodeCounts);

            // we may need to clear the cache to identify new episodes
            // but update check and identify are done in two seperate import actions (online/local)
            if (mParsingParameters.Actions.Contains(ParsingAction.GetOnlineUpdates) && mParsingParameters.Actions.Contains(ParsingAction.IdentifyNewEpisodes))
            {
                mParsingParameters.Actions.Remove(ParsingAction.GetOnlineUpdates);
                mParsingParameters.Actions.Insert(mParsingParameters.Actions.IndexOf(ParsingAction.IdentifyNewEpisodes), ParsingAction.GetOnlineUpdates);
            }
                
            GetUpdates lUpdates = null;
            foreach (ParsingAction action in mParsingParameters.Actions)
            {
                MPTVSeriesLog.Write("Begin Parsing action: ", action.ToString(), MPTVSeriesLog.LogLevel.Debug);
                if (Worker.CancellationPending)
                    break;

                switch (action) {
                    case ParsingAction.List_Remove:
                        ParseActionRemove(mParsingParameters.Files);
                        break;

                    case ParsingAction.List_Add:
                        ParseActionAdd(mParsingParameters.Files);
                        break;

                    case ParsingAction.NoExactMatch:
                        mNoExactMatch = true;
                        break;

                    case ParsingAction.LocalScan:
                        ParseActionLocalScan(mParsingParameters.IsLocalScan, mParsingParameters.IsUpdateScan, mParsingParameters.UserInputResult?.ParseResults);
                        break;

                    case ParsingAction.MediaInfo:
                        // Multi-threaded MediaInfo parsing of new files - goes straight to next task
                        if ( !DBOption.GetOptions( DBOption.cDisableMediaInfo ) )
                        {
                            // disable only in configuration
                            if ( !Settings.IsConfig || ( Settings.IsConfig && !DBOption.GetOptions( DBOption.cDisableMediaInfoInConfigImports ) ) )
                            {
                                lMediaInfoWorker = new BackgroundWorker();
                                MediaInfoParse( lMediaInfoWorker );
                            }
                        }
                        break;

                    case ParsingAction.IdentifyNewSeries:
                        GetSeries(Worker, mNoExactMatch, mParsingParameters.UserInputResult?.UserChosenSeries);
                        UpdateSeries(true, null); // todo: ask orderoption
                        break;

                    case ParsingAction.IdentifyNewEpisodes:
                        GetEpisodes(mParsingParameters.IsUpdateScan, mFullSeriesRetrieval); 
                        // at this point we have identified any new episodes
                        // signal the facade to be reloaded.
                        // TODO: smart way to report progress and expose as property to skins
                        Worker.ReportProgress(30);
                        break;
                    
                    case ParsingAction.CheckArtwork:
                        if (DBOption.GetOptions(DBOption.cCheckArtwork) == 1)
                            this.CheckBanners();

                        break;

                    case ParsingAction.CleanupEpisodes:
                        if (DBOption.GetOptions(DBOption.cCleanOnlineEpisodes) == 1)
                            this.CleanEpisodes();

                        break;

                    case ParsingAction.GetOnlineUpdates:
                        lUpdates = GetOnlineUpdates();
                        break;

                    case ParsingAction.UpdateSeries:
                        if (lUpdates != null)
                            UpdateSeries(false, lUpdates.UpdatedSeries);

                        if (mParsingParameters.Series != null)
                            UpdateSeries(false, mParsingParameters.Series);

                        break;

                    case ParsingAction.UpdateEpisodes:
                        if (lUpdates != null)
                            UpdateEpisodes(lUpdates.UpdatedSeries);

                        if (mParsingParameters.Episodes != null)
                        {
                            UpdateEpisodes(mParsingParameters.Series);
                        }
                        break;

                    case ParsingAction.UpdateBanners:
                        if (lUpdates != null)
                            UpdateBanners(false, lUpdates.UpdatedSeries);

                        break;

                    case ParsingAction.UpdateFanart:
                        if (lUpdates != null && !DBOption.GetOptions(DBOption.cAutoUpdateAllFanart))
                            UpdateFanart(true, lUpdates.UpdatedSeries);

                        break;

                    case ParsingAction.GetNewBanners:
                        // update new series for banners
                        UpdateBanners(true, null);
                        break;

                    case ParsingAction.GetNewFanArt:
                        UpdateFanart(false, null);
                        break;

                    case ParsingAction.GetNewActors:
                        UpdateActors(false, null);
                        break;

                    case ParsingAction.UpdateEpisodeThumbNails:
                        UpdateEpisodeThumbNails();
                        break;

                    case ParsingAction.UpdateCommunityRatings:
                        if (DBOption.GetOptions(DBOption.cTraktCommunityRatings) == 1)
                        {
                            lTraktCommunityRatingsWorker = new BackgroundWorker();
                            UpdateTraktCommunityRatings(lTraktCommunityRatingsWorker, mParsingParameters.Series, mParsingParameters.Episodes);
                        }
                        break;

                    case ParsingAction.UpdateUserRatings:
                        lUserRatingsWorker = new BackgroundWorker();
                        UpdateUserRatings(lUserRatingsWorker);
                        break;

                    case ParsingAction.UpdateUserFavourites:
                        UpdateUserFavourites();
                        break;

                    case ParsingAction.UpdateEpisodeCounts:
                        //Threaded processing of episode counts - goes straight to next task
                        lEpisodeCounterWorker = new BackgroundWorker();
                        UpdateEpisodeCounts(lEpisodeCounterWorker);
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
                Thread.Sleep(1000);
            }
            while ((lMediaInfoWorker != null && lMediaInfoWorker.IsBusy) || 
                   (lEpisodeCounterWorker != null && lEpisodeCounterWorker.IsBusy) ||
                   (lUserRatingsWorker != null && lUserRatingsWorker.IsBusy) || 
                   (lTraktCommunityRatingsWorker != null && lTraktCommunityRatingsWorker.IsBusy));
            
            // save the last time we requested an update
            if (lUpdates != null)
                DBOption.SetOptions(DBOption.cUpdateTimeStamp, lUpdates.OnlineTimeStamp);            

            // and we are done, the backgroundworker is going to notify so
            MPTVSeriesLog.Write("***************************************************************************");
            MPTVSeriesLog.Write("*******************            Completed           ************************");
            MPTVSeriesLog.Write("***************************************************************************");
        }

        private void UpdateRecentlyAdded()
        {
            // no need to update fields if we wont use them
            if (DBOption.GetOptions(DBOption.cNewEpisodeThumbType) == (int)NewEpisodeIndicatorType.none ||
                DBOption.GetOptions(DBOption.cNewEpisodeThumbType) == (int)NewEpisodeIndicatorType.unwatched) return;

            MPTVSeriesLog.Write(BigLogMessage("Updating Recently Added"));

            // Clear setting for all series
            DBSeries.GlobalSet(DBOnlineSeries.cHasNewEpisodes, (DBValue)"0");

            // Calculate date for querying database
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            dt = dt.Subtract(new TimeSpan(DBOption.GetOptions(DBOption.cNewEpisodeRecentDays), 0, 0, 0, 0));
            string date = dt.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");
           
            var conditions = new SQLCondition();
            conditions.Add(new DBEpisode(), DBEpisode.cFileDateCreated, date, SQLConditionType.GreaterEqualThan);            
            List<DBEpisode> episodes = DBEpisode.Get(conditions, false);
            
            // Get unique series list
            var seriesIdList = episodes.Select(e => e[DBOnlineEpisode.cSeriesID].ToString()).Distinct().ToList();

            // Set series 'HasNewEpisodes' field if it contains new episodes
            int i = 0;
            foreach (var seriesId in seriesIdList)
            {
                DBSeries series = Helper.GetCorrespondingSeries(int.Parse(seriesId));
                if (series != null)
                {
                    MPTVSeriesLog.Write("progress received: UpdateRecentlyAdded [{0}/{1}] {2}", ++i, seriesIdList.Count, series.ToString());
                    series[DBOnlineSeries.cHasNewEpisodes] = "1";
                    series.Commit();
                }
            }
        }

        #region ParseActions
        private void ParseActionRemove(List<PathPair> files) {
            string initialMsg = "*******************        Remove Run Starting      ***********************";
            MPTVSeriesLog.Write(PrettyStars(initialMsg.Length));
            MPTVSeriesLog.Write(initialMsg);
            MPTVSeriesLog.Write(PrettyStars(initialMsg.Length));
			
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
                
                // clear the file if we know for certain it doesnt exist anymore
                // if removable drive or network path is offline then we dont remove
                string importPath = LocalParse.getImportPath(pair.m_sFull_FileName);
                if (string.IsNullOrEmpty(importPath) || (Directory.Exists(importPath) && !File.Exists(pair.m_sFull_FileName) && string.Equals(DeviceManager.GetVolumeLabel(importPath), DeviceManager.GetVolumeLabel(pair.m_sFull_FileName))))
                {
                    MPTVSeriesLog.Write("Episode is marked for removal from database, file: {0}", pair.m_sFull_FileName);
                    DBEpisode.Clear(condition);
                    DataUpdated = true;
                }
                else 
                {
                    DBEpisode.GlobalSet(DBEpisode.cImportProcessed, 2, condition);
                    DBEpisode.GlobalSet(DBEpisode.cIsAvailable, 0, condition);
                    DataUpdated = true;
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
            MPTVSeriesLog.Write(PrettyStars(initialMsg.Length));
            MPTVSeriesLog.Write(initialMsg);
            MPTVSeriesLog.Write(PrettyStars(initialMsg.Length));
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
                if (mNoExactMatch)
                {
                    initialMsg = String.Format("******************* NoExactMatch Run Starting (LocalScan: {0} -  UpdateScan: {1})   ***************************", localScan, updateScan);
                }

                MPTVSeriesLog.Write(PrettyStars(initialMsg.Length));
                MPTVSeriesLog.Write(initialMsg);
                MPTVSeriesLog.Write(PrettyStars(initialMsg.Length));
            }
            if (localScan) {
                // mark all files in the db as not processed (to figure out which ones we'll have to remove after the import)
                DBEpisode.GlobalSet(DBEpisode.cImportProcessed, 2);                
                DBEpisode.GlobalSet(DBEpisode.cIsAvailable, 2); // dont set to 0 so we dont filter out eps in view during localscan

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

                // remove references to files that we know for certain dont exist anymore
                // if import path is not available then could be offline
                List<DBEpisode> localepisodes = DBEpisode.Get(condition);
                foreach (DBImportPath path in DBImportPath.GetAll())
                {
                    string importPath = path[DBImportPath.cPath];
                    if (path[DBImportPath.cEnabled] && !Directory.Exists(importPath)) //  && !LocalParse.needToKeepReference(importPath)
                    {
                        MPTVSeriesLog.Write("Import path '{0}' is not available, ignoring database maintenance on this path until available.", importPath);
                        localepisodes.RemoveAll(ep => ep[DBEpisode.cFilename].ToString().Contains(importPath));
                    }
                    // check if same importpath has different volume labels and keep them as well
                    // this can occur if you have multiple DVDs or USB disks that share the same drive letter
                    // but each removable disk have a unique volume label (volume serial should be used but we dont store that)
                    if (path[DBImportPath.cEnabled] && Directory.Exists(importPath) && !string.IsNullOrEmpty(DeviceManager.GetVolumeLabel(importPath)))
                    {
                        localepisodes.RemoveAll(ep => ep[DBEpisode.cFilename].ToString().Contains(importPath) &&
                                                      !string.Equals(ep[DBEpisode.cVolumeLabel].ToString(), DeviceManager.GetVolumeLabel(importPath)));
                    }
                }

                // clear local database references for files that dont exist anymore
                foreach (DBEpisode localepisode in localepisodes)
                {
                    MPTVSeriesLog.Write("Episode is marked for removal from database, file: '{0}', volume label: '{1}'", localepisode[DBEpisode.cFilename], localepisode[DBEpisode.cVolumeLabel]);
                    DBEpisode.Clear(new SQLCondition(new DBEpisode(), DBEpisode.cFilename, localepisode[DBEpisode.cFilename], SQLConditionType.Equal));                 
                }

                // mark all remaining episodes in database as not available                
                condition = new SQLCondition();
                condition.Add(new DBEpisode(), DBEpisode.cImportProcessed, 2, SQLConditionType.Equal);                
                DBEpisode.GlobalSet(DBEpisode.cIsAvailable, false, condition);

                // and copy the HasLocalFileTemp value into the real one
                DBSeries.GlobalSet(DBOnlineSeries.cHasLocalFiles, DBOnlineSeries.cHasLocalFilesTemp);
                DBSeason.GlobalSet(DBSeason.cHasLocalFiles, DBSeason.cHasLocalFilesTemp);
            }
        }

        private void GetSeries(BackgroundWorker aWorker, bool aNoExactMatch, Dictionary<string, UserInputResultSeriesActionPair> aPreChosenSeriesPairs)
        {
            MPTVSeriesLog.Write(BigLogMessage("Identifying Unknown Series Online"), MPTVSeriesLog.LogLevel.Debug);

            var lCondition = new SQLCondition();
            // all series that don't have an onlineID ( < 0) and not marked as ignored
            lCondition.Add(new DBSeries(), DBSeries.cID, 0, SQLConditionType.LessThan);
            lCondition.Add(new DBSeries(), DBSeries.cScanIgnore, 0, SQLConditionType.Equal);

            int lIndex = 0;
            List<DBSeries> lSeries = DBSeries.Get(lCondition, false, false);
            if (lSeries.Count > 0)            
            {
                // Run Online update when needed
                OnlineUpdateNeeded = true;
                MPTVSeriesLog.Write(string.Format("Found {0} unknown Series, attempting to identify them now", lSeries.Count), MPTVSeriesLog.LogLevel.Debug);
            }
            else
                MPTVSeriesLog.Write("All Series are already identified", MPTVSeriesLog.LogLevel.Debug);

            foreach (DBSeries series in lSeries) {
                if (aWorker.CancellationPending)
                    return;
                
                string lSeriesNameToSearch = series[DBSeries.cParsedName];                
                DBOnlineSeries lUserChosenSeries = null;
                UserInputResultSeriesActionPair lSap = null;

                if (aPreChosenSeriesPairs != null)
                    aPreChosenSeriesPairs.TryGetValue(lSeriesNameToSearch, out lSap);

                if (aPreChosenSeriesPairs == null)
                {
                    lUserChosenSeries = SearchForSeries(lSeriesNameToSearch, aNoExactMatch, mFeedback);
                }
                else if (lSap != null && lSap.RequestedAction == UserInputResults.SeriesAction.Approve)
                {
                    MPTVSeriesLog.Write("User has approved \"" + lSeriesNameToSearch + "\" as being: " + lSap.ChosenSeries.ToString(), MPTVSeriesLog.LogLevel.Debug);
                    lUserChosenSeries = lSap.ChosenSeries;
                }
                else if (lSap != null && lSap.RequestedAction == UserInputResults.SeriesAction.IgnoreAlways)
                {
                    // duplicate code from SearchForSeries - should be changed
                    MPTVSeriesLog.Write("User has Ignored \"" + lSeriesNameToSearch + "\" in the future", MPTVSeriesLog.LogLevel.Debug);
                    
                    var lIgnoreSeries = new DBSeries(lSeriesNameToSearch);
                    series[DBSeries.cScanIgnore] = 1; // means it will be skipped in the future
                    series[DBSeries.cHidden] = true;
                    series.Commit();
                }

                string lSeriesName = lUserChosenSeries != null ? lUserChosenSeries[DBOnlineSeries.cPrettyName].ToString() : lSeriesNameToSearch;
                aWorker.ReportProgress(0, new ParsingProgress(ParsingAction.IdentifyNewSeries, lSeriesName, ++lIndex, lSeries.Count, series, null));

                if (lUserChosenSeries != null) // make sure selection was not cancelled
                {                    
                    // set the ID on the current series with the one from the chosen one
                    // we need to update all depending items - seasons & episodes
                    List<DBSeason> lSeasons = DBSeason.Get(series[DBSeries.cID]);
                    foreach (DBSeason season in lSeasons)
                        season.ChangeSeriesID(lUserChosenSeries[DBSeries.cID]);

                    var lSetCondition = new SQLCondition();
                    lSetCondition.Add(new DBSeason(), DBSeason.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                    DBSeason.Clear(lSetCondition);

                    lSetCondition = new SQLCondition();
                    lSetCondition.Add(new DBEpisode(), DBEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                    List<DBEpisode> lEpisodes = DBEpisode.Get(lSetCondition, false);
                    foreach (DBEpisode episode in lEpisodes)
                        episode.ChangeSeriesID(lUserChosenSeries[DBSeries.cID]);

                    lSetCondition = new SQLCondition();
                    lSetCondition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                    DBOnlineEpisode.Clear(lSetCondition);

                    int lSeriesID = series[DBSeries.cID];

                    series.ChangeSeriesID(lUserChosenSeries[DBSeries.cID]);
                    series.Commit();

                    lSetCondition = new SQLCondition();
                    lSetCondition.Add(new DBOnlineSeries(), DBOnlineSeries.cID, lSeriesID, SQLConditionType.Equal);
                    DBOnlineSeries.Clear(lSetCondition);

                    // only keep one local dbseries marked as non dupe
                    lSetCondition = new SQLCondition();
                    lSetCondition.Add(new DBOnlineSeries(), DBOnlineSeries.cID, lUserChosenSeries[DBSeries.cID], SQLConditionType.Equal);
                    List<DBSeries> lSeriesDuplicates = DBSeries.Get(lSetCondition);
                    bool lFirstDuplicate = true;
                    foreach (DBSeries seriesDuplicate in lSeriesDuplicates)
                    {
                        if (lFirstDuplicate)
                        {
                            seriesDuplicate[DBSeries.cDuplicateLocalName] = 0;
                            seriesDuplicate.Commit();
                            lFirstDuplicate = false;
                        }
                        else
                        {
                            seriesDuplicate[DBSeries.cDuplicateLocalName] = 1;
                            seriesDuplicate.Commit();
                        }
                    }
                }               
            }
            // that is done
            aWorker.ReportProgress(0, new ParsingProgress(ParsingAction.IdentifyNewSeries, lSeries.Count));
        }

        private GetUpdates GetOnlineUpdates()
        {
            MPTVSeriesLog.Write(BigLogMessage("Processing updates from online database"));

            // let's get the time we last updated from the local options
            long lLastUpdateEpoch = DBOption.GetOptions(DBOption.cUpdateTimeStamp);
            long lCurrentEpoch = DateTime.UtcNow.ToEpoch();
            long lDaysSinceLastUpdate = (lCurrentEpoch - lLastUpdateEpoch) / 60 / 60 / 24;

            var lUpdates = new GetUpdates(lDaysSinceLastUpdate, lCurrentEpoch);
            if ( lUpdates.UpdatedSeries == null ) return null;

            MPTVSeriesLog.Write($"Found {lUpdates.UpdatedSeries?.Count} online tv series updated since last update");

            #region remove cache files that need updating

            string lCacheDir = Path.Combine(Settings.GetPath(Settings.Path.config), @"Cache\Tmdb");

            if (Directory.Exists(lCacheDir))
            {
                // get series cache directories
                var lSeriesDirs = new DirectoryInfo(lCacheDir).GetDirectories().Select(d => d.Name).ToList();

                // check if updated series exists in cache
                foreach (int series in lUpdates.UpdatedSeries.Where(s => lSeriesDirs.Contains(s.ToString())))
                {
                    MPTVSeriesLog.Write($"Deleting cache directory for series with ID: {series}");

                    string lDirectory = Path.Combine(lCacheDir, series.ToString());

                    try
                    {
                        Directory.Delete(lDirectory, true);
                    }
                    catch (Exception ex)
                    {
                        MPTVSeriesLog.Write($"Failed to delete 'lDirectory' cache directory, Exception: {ex.Message}");
                        continue;
                    }

                    DBSeries lDBSeries = Helper.GetCorrespondingSeries(series);
                    MPTVSeriesLog.Write($"Cache cleared for series: { lDBSeries?.ToString() ?? series.ToString() }");
                }
            }

            #endregion

            MPTVSeriesLog.Write(BigLogMessage("Finished processing updates from online database"));

            return lUpdates;
        }

        private void UpdateSeries(bool aUpdateNewSeries, List<int> aSeriesUpdated)
        {
            // now retrieve the info about the series
            var lCondition = new SQLCondition();
            // all series that have an onlineID ( != 0)
            lCondition.Add(new DBOnlineSeries(), DBOnlineSeries.cID, 0, SQLConditionType.GreaterThan);
            lCondition.Add(new DBSeries(), DBSeries.cScanIgnore, 0, SQLConditionType.Equal);
            lCondition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);

            if (aUpdateNewSeries)
            {
                MPTVSeriesLog.Write(BigLogMessage("Retrieving Metadata for new Series"));
                // and that never had data imported from the online DB
                lCondition.Add(new DBOnlineSeries(), DBOnlineSeries.cOnlineDataImported, 0, SQLConditionType.Equal);
            } 
            else
            {
                MPTVSeriesLog.Write(BigLogMessage("Updating Metadata for existing Series"));
                // and that already had data imported from the online DB (but not the new ones, that are set to 1) ??
                lCondition.Add(new DBOnlineSeries(), DBOnlineSeries.cOnlineDataImported, 2, SQLConditionType.Equal);
            }
            List<DBSeries> lSeriesList = DBSeries.Get(lCondition, false, false);

            if (!aUpdateNewSeries && lSeriesList.Count > 0)
            {
                // let's check which of these we have any interest in
                lSeriesList.RemoveAll(s => !aSeriesUpdated.Contains(s[DBSeries.cID]));
            }

            if (lSeriesList.Count == 0)
            {
                MPTVSeriesLog.Write("Nothing to do");
                return;
            }

            MPTVSeriesLog.Write($"{ (aUpdateNewSeries ? "Retrieving" : "Looking for updated") } metadata of {lSeriesList.Count} series", MPTVSeriesLog.LogLevel.Debug);

            var lUpdateSeriesParser = new UpdateSeries(GenerateIDListOfString(lSeriesList, DBSeries.cID));
            
            int lIndex = 0;
            bool lTraktCommunityRatings = DBOption.GetOptions(DBOption.cTraktCommunityRatings) && Helper.IsTraktAvailableAndEnabled;

            foreach (DBOnlineSeries updatedSeries in lUpdateSeriesParser.ResultsLazy)
            {
                DataUpdated = true;
                if (Worker.CancellationPending)
                    return;
                               
                MPTVSeriesLog.Write(string.Format("Metadata {0} for \"{1}\"", (aUpdateNewSeries ? "retrieved" : "updated"), updatedSeries.ToString()), MPTVSeriesLog.LogLevel.Debug);

                // find the corresponding series in our list
                foreach (DBSeries localSeries in lSeriesList)
                {
                    if (localSeries[DBSeries.cID] == updatedSeries[DBSeries.cID])
                    {
                        Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateSeries, updatedSeries[DBOnlineSeries.cPrettyName], ++lIndex, lSeriesList.Count, updatedSeries, null));
                        // go over all the fields, (and update only those which haven't been modified by the user - will do that later)
                        foreach (String key in updatedSeries.FieldNames)
                        {
                            switch (key)
                            {
                                // do not overwrite current series local settings with the one from the online series
                                case DBSeries.cParsedName: // this field shouldn't be required here since updatedSeries is an Onlineseries and not a localseries??
                                case DBOnlineSeries.cHasLocalFiles:
                                case DBOnlineSeries.cHasLocalFilesTemp:
                                case DBOnlineSeries.cEpisodesUnWatched:
                                case DBOnlineSeries.cEpisodeCount:
                                case DBOnlineSeries.cIsFavourite:
                                case DBOnlineSeries.cChosenEpisodeOrder:
                                case DBOnlineSeries.cEpisodeSortOrder:
                                case DBOnlineSeries.cBannerFileNames: // banners get handled differently (later on)
                                case DBOnlineSeries.cPosterFileNames:
                                case DBOnlineSeries.cCurrentBannerFileName:
                                case DBOnlineSeries.cCurrentPosterFileName:
                                case DBOnlineSeries.cMyRating:
                                case DBOnlineSeries.cMyRatingAt:
                                case DBOnlineSeries.cViewTags:
                                case DBOnlineSeries.cHasNewEpisodes: //gets cleared and updated at end of scan
                                case DBOnlineSeries.cTraktIgnore:
                                case DBOnlineSeries.cOriginalName:
                                case DBOnlineSeries.cTraktID:
                                case DBOnlineSeries.cSlug:
                                //case DBOnlineSeries.cTmdbId:
                                case DBOnlineSeries.cArtworkChooserProvider:
                                    break;

                                // dont update community ratings if we get from trakt
                                case DBOnlineSeries.cRating:
                                case DBOnlineSeries.cRatingCount:
                                    if (lTraktCommunityRatings)
                                        break;
                                    goto default;
                                    
                                case DBOnlineSeries.cEpisodeOrders:
                                    if (aUpdateNewSeries)
                                        goto default;
                                    break;

                                default:
                                    if (!key.EndsWith(DBTable.cUserEditPostFix))
                                    {
                                        localSeries.AddColumn(key, new DBField(DBField.cTypeString));
                                        localSeries[key] = updatedSeries[key].ToString().RemapHighOrderChars();
                                    }
                                    break;
                            }
                        }

                        // diff. order options
                        if (aUpdateNewSeries)
                            SetEpisodeOrderForSeries(localSeries);

                        // data import completed; set to 2 (data up to date)
                        localSeries[DBOnlineSeries.cOnlineDataImported] = 2;

                        if (localSeries[DBOnlineSeries.cHasLocalFilesTemp])
                            localSeries[DBOnlineSeries.cHasLocalFiles] = 1;
                        
                        localSeries.Commit();
                        
                        // UPDATE CACHE to fix getting the series named as the parsed name instead of the online pretty name!
                        if(aUpdateNewSeries) cache.addChangeSeries(localSeries);
                    }
                }
            }
            if (lIndex == 0)
            {
                MPTVSeriesLog.Write(string.Format("No {0} found", (aUpdateNewSeries ? "metadata" : "updates")));
                Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateSeries, 0));
            }
            Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateSeries,  lUpdateSeriesParser.Results.Count));
        }

        private void GetEpisodes(bool aUpdateScan, bool aFullSeriesRetrieval)
        {
            MPTVSeriesLog.Write(BigLogMessage("Get Episodes"));
            SQLCondition lCondition = null;
            if (aUpdateScan) 
            {
                // mark existing online data as "old", needs a refresh
                lCondition = new SQLCondition();
                lCondition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cOnlineDataImported, 2, SQLConditionType.Equal);
                DBEpisode.GlobalSet(DBOnlineEpisode.cOnlineDataImported, 1, lCondition);
            }

            lCondition = new SQLCondition();
            // all series that have an onlineID ( != 0)
            lCondition.Add(new DBSeries(), DBSeries.cID, 0, SQLConditionType.GreaterThan);
            lCondition.Add(new DBSeries(), DBSeries.cScanIgnore, 0, SQLConditionType.Equal);
            lCondition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);

            List<DBSeries> lSeries = DBSeries.Get(lCondition, false, false);

            if (aFullSeriesRetrieval && aUpdateScan)
                MPTVSeriesLog.Write("Mode: Get all Episodes of Series");

            int epCount = 0;
            int nIndex = 0;
            bool traktCommunityRatings = DBOption.GetOptions(DBOption.cTraktCommunityRatings) && Helper.IsTraktAvailableAndEnabled;
            
            // clear reference to existing online episodes
            mOnlineEpisodes.Clear();

            foreach (DBSeries series in lSeries) 
            {
                List<DBEpisode> episodesList = null;

                // lets get the list of unidentified episodes
                SQLCondition conditions = new SQLCondition();               
                conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cID, 0, SQLConditionType.Equal);
                conditions.Add(new DBEpisode(), DBEpisode.cFilename, string.Empty, SQLConditionType.NotEqual);
                episodesList = DBEpisode.Get(conditions, false);

                // we may already have all the online episode references
                // but need to update the composite id's of new episodes added
                if (episodesList.Count == 0 && !(string.IsNullOrEmpty(series[DBOnlineSeries.cChosenEpisodeOrder]) || series[DBOnlineSeries.cChosenEpisodeOrder] == "Aired"))
                {
                    conditions = new SQLCondition();
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                    conditions.Add(new DBEpisode(), DBEpisode.cCompositeUpdated, 0, SQLConditionType.Equal);
                    episodesList = DBEpisode.Get(conditions, false);
                }

                epCount += episodesList.Count;
                                
                if (aFullSeriesRetrieval || episodesList.Count > 0) 
                {
                    string[] lSeasons = series[DBOnlineSeries.cOnlineSeasonsAvailable].ToString().Split(',');

                    var lEpisodesParser = new GetEpisodes((string)series[DBSeries.cID], Array.ConvertAll(lSeasons, s => int.Parse(s)));

                    Worker.ReportProgress(0, new ParsingProgress(ParsingAction.IdentifyNewEpisodes, series[DBOnlineSeries.cPrettyName], ++nIndex, lSeries.Count, series, null));
                    if (lEpisodesParser.Results.Count > 0) 
                    {
                        MPTVSeriesLog.Write(string.Format("Found {0} episodes online for \"{1}\"", lEpisodesParser.Results.Count.ToString().PadLeft(3, '0'), series.ToString()), MPTVSeriesLog.LogLevel.Debug);
                        // look for the episodes for that series, and compare / update the values
                        if (mParsingParameters.UserEpisodeMatcher == null) // auto mode
                            MatchOnlineToLocalEpisodes(series, episodesList, lEpisodesParser);
                        else // user mode
                            mParsingParameters.UserEpisodeMatcher.MatchEpisodesForSeries(series, episodesList, lEpisodesParser.Results);

                        // add online episode for cleanup task
                        mOnlineEpisodes.Add(series[DBOnlineSeries.cID], lEpisodesParser.Results);
                    } 
                    else
                        MPTVSeriesLog.Write(string.Format("No episodes could be identified online for {0}, check that the online database has these episodes", series.ToString()));
                    
                    if (aFullSeriesRetrieval) 
                    {
                        // add all online episodes to the database                 
                        foreach (DBOnlineEpisode onlineEpisode in lEpisodesParser.Results) 
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
                            foreach (String key in onlineEpisode.FieldNames) 
                            {
                                switch (key) 
                                {
                                    case DBOnlineEpisode.cCompositeID:
                                    case DBEpisode.cSeriesID:
                                    case DBOnlineEpisode.cWatched:
                                    case DBOnlineEpisode.cLastWatchedDate:
                                    case DBOnlineEpisode.cFirstWatchedDate:
                                    case DBOnlineEpisode.cPlayCount:
                                    case DBOnlineEpisode.cHidden:
                                    case DBOnlineEpisode.cMyRating:
                                    case DBOnlineEpisode.cMyRatingAt:
                                    case DBOnlineEpisode.cEpisodeThumbnailFilename:
                                    case DBOnlineEpisode.cEpisodeThumbnailSource:
                                    case DBOnlineEpisode.cTMDbEpisodeThumbnailUrl:
                                        // do nothing here, this information is local only
                                        break;

                                    // don't update community ratings if we get from trakt
                                    case DBOnlineEpisode.cRating:
                                    case DBOnlineEpisode.cRatingCount:
                                        if (traktCommunityRatings)
                                            break;
                                        goto default;

                                    case DBOnlineEpisode.cSeasonIndex:
                                    case DBOnlineEpisode.cEpisodeIndex:
                                        break; // those must not get overwritten from what they were set to by getEpisodes (because of different order options)

                                    default:
                                        if (!key.EndsWith(DBTable.cUserEditPostFix))
                                        {
                                            newOnlineEpisode.AddColumn(key, new DBField(DBField.cTypeString));
                                            newOnlineEpisode[key] = onlineEpisode[key].ToString().RemapHighOrderChars();
                                        }
                                        break;
                                }
                            }
                            newOnlineEpisode.Commit();                        
                        }
                    }
                }
            }
            DBEpisode.GlobalSet(DBEpisode.cCompositeUpdated, 1);

            if (epCount == 0)
                MPTVSeriesLog.Write("No new episodes identified");            
            else 
            {

                // get the result from user matching if needed and commit them
                List<KeyValuePair<DBSeries, List<KeyValuePair<DBEpisode, DBOnlineEpisode>>>> result;
                // this will block here
                if (mParsingParameters.UserEpisodeMatcher != null && mParsingParameters.UserEpisodeMatcher.GetResult(out result) == ReturnCode.OK)
                {
                    foreach (var match in result)
                        foreach (var episodePair in match.Value)
                        {
                            if(episodePair.Value != null && episodePair.Key != null)
                                commitOnlineToLocalEpisodeMatch(match.Key, episodePair.Key, episodePair.Value);
                        }
                }

                // Online update when needed
                if (!aUpdateScan)
                    OnlineUpdateNeeded = true;
            }
            Worker.ReportProgress(0, new ParsingProgress(ParsingAction.IdentifyNewEpisodes, epCount));
        }

        private void UpdateEpisodes(List<int> aUpdatedSeries)
        {
            MPTVSeriesLog.Write(BigLogMessage("Updating Metadata for episodes in database"));

            // let's check which series/episodes we have locally
            var lCondition = new SQLCondition(new DBOnlineEpisode(), DBOnlineEpisode.cID, 0, SQLConditionType.GreaterThan);
            lCondition.AddOrderItem(DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID), SQLCondition.orderType.Ascending);

            List<DBEpisode> lEpisodesInDB = DBEpisode.Get(lCondition);

            if (aUpdatedSeries == null || aUpdatedSeries.Count == 0)
                lEpisodesInDB.Clear();

            // let's check which of these we have any interest in
            // i.e. remove any local episodes where the series ID does not exist in online updates
            lEpisodesInDB.RemoveAll(e => !aUpdatedSeries.Contains(e[DBOnlineEpisode.cSeriesID]));

            // TODO: lets filter the episode list even more e.g. only the latest airing season (we don't want to update a series with 1000 of episodes!)

            if (lEpisodesInDB.Count == 0) {
                Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateEpisodes, 0));            
                MPTVSeriesLog.Write("Nothing to do");
                return;
            }

            // let's update those we are interested in
            MPTVSeriesLog.Write($"Found {lEpisodesInDB.Count} episodes need updating");

            int i = 0;
            var lDistinctSeriesIds = (from s in lEpisodesInDB select s[DBEpisode.cSeriesID].ToString()).Distinct().ToList();
            foreach (string seriesid in lDistinctSeriesIds)
            {
                var lEpisodes = lEpisodesInDB.Where(e => e[DBEpisode.cSeriesID] == seriesid).ToList();
                DBSeries lSeries = Helper.GetCorrespondingSeries(int.Parse(seriesid));
                if (lSeries != null)
                {
                    Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateEpisodes, lSeries.ToString() + " [" + lEpisodes.Count.ToString() + " episodes]", ++i, lDistinctSeriesIds.Count, lSeries, null));

                    string[] lSeasons = lSeries[DBOnlineSeries.cOnlineSeasonsAvailable].ToString().Split(',');

                    MatchOnlineToLocalEpisodes(lSeries, lEpisodes, new GetEpisodes(seriesid, Array.ConvertAll(lSeasons, s => int.Parse(s))));
                }
            }
            
            Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateEpisodes, lEpisodesInDB.Count));
        }

        private void UpdateBanners(bool aUpdateNewSeries, List<int> aUpdatedSeries)
        {
            // no updates to process
            if (!aUpdateNewSeries && aUpdatedSeries == null)
                return;

            var lCondition = new SQLCondition();
            // all series that have an onlineID ( > 0)
            lCondition.Add(new DBOnlineSeries(), DBSeries.cID, 0, SQLConditionType.GreaterThan);
            lCondition.Add(new DBSeries(), DBSeries.cScanIgnore, 0, SQLConditionType.Equal);
            lCondition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);
            if (aUpdateNewSeries)
            {
                if (!DBOption.GetOptions(DBOption.cAutoDownloadMissingArtwork))
                    return;

                MPTVSeriesLog.Write(BigLogMessage("Checking for missing artwork"));
                // and that never had data imported from the online DB
                lCondition.beginGroup();
                lCondition.Add(new DBOnlineSeries(), DBOnlineSeries.cBannerFileNames, string.Empty, SQLConditionType.Equal);
                lCondition.nextIsOr = true;
                lCondition.Add(new DBOnlineSeries(), DBOnlineSeries.cPosterFileNames, string.Empty, SQLConditionType.Equal);
                lCondition.nextIsOr = true;
                lCondition.AddCustom(" exists (select * from season where season.seriesID = online_series.id and season.bannerfilenames = '')");
                lCondition.nextIsOr = false;
                lCondition.endGroup();
            }
            else
            {
                MPTVSeriesLog.Write(BigLogMessage("Checking for new artwork"));
                // and that already had data imported from the online DB
                lCondition.Add(new DBOnlineSeries(), DBOnlineSeries.cBannerFileNames, string.Empty, SQLConditionType.NotEqual);
                lCondition.Add(new DBOnlineSeries(), DBOnlineSeries.cPosterFileNames, string.Empty, SQLConditionType.NotEqual);
            }

            List<DBSeries> lSeries = DBSeries.Get(lCondition, false, false);
            if (!aUpdateNewSeries && lSeries.Count > 0)
            {
                // let's check which of these we have any interest in
                lSeries.RemoveAll(s => !aUpdatedSeries.Contains(s[DBOnlineSeries.cID]));
            }

            int lIndex = 0;
            if (lSeries.Count == 0)
            {
                if (aUpdateNewSeries)
                    MPTVSeriesLog.Write("All series appear to have artwork already", MPTVSeriesLog.LogLevel.Debug);
                else
                    MPTVSeriesLog.Write("Nothing to do");

                Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateBanners, 0));
            }
            else
                MPTVSeriesLog.Write("Looking for artwork on " + lSeries.Count + " series", MPTVSeriesLog.LogLevel.Debug);

            foreach (DBSeries series in lSeries)
            {
                if (Worker.CancellationPending)
                    return;

                MPTVSeriesLog.Write((aUpdateNewSeries ? "Downloading" : "Refreshing") + " artwork for \"" + series.ToString() + "\"", MPTVSeriesLog.LogLevel.Debug);

                lIndex++;                
                Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateBanners, series[DBOnlineSeries.cPrettyName], lIndex, lSeries.Count, series, null));

                // get list of available banners online
                var lBannerParser = new GetBanner(int.Parse(series[DBSeries.cID]));

                // download banners
                lBannerParser.BannerDownloadDone += new NewArtWorkDownloadDoneHandler(name =>
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        Worker.ReportProgress(0, new ParsingProgress
                                                    (
                                                        ParsingAction.UpdateBanners, 
                                                        $"{series[DBOnlineSeries.cPrettyName]} - {name}",
                                                        lIndex, 
                                                        lSeries.Count, 
                                                        series,
                                                        name
                                                    )
                                              );
                    }
                });
                
                // Other language for the Series?
                if (DBOption.GetOptions(DBOption.cOverrideLanguage))
                {
                    //Get the prefered language for the Series.
                    lBannerParser.DownloadBanners(OnlineAPI.GetLanguageOverride(series[DBOnlineSeries.cID]));
                }
                else
                {
                    lBannerParser.DownloadBanners(OnlineAPI.SelectedLanguage);
                }

                // update database with available banners
                SeriesBannersMap lSeriesArtwork = Helper.GetElementFromList<SeriesBannersMap, string>(series[DBSeries.cID], "SeriesID", 0, lBannerParser.SeriesBannersMap);
                if (lSeriesArtwork == null) continue;

                #region Series WideBanners
                foreach (WideBannerSeries seriesBanner in lSeriesArtwork.SeriesWideBanners)
                {
                    // if we have a new banner then add it the list of available ones, possibly a higher rated artwork became available
                    if (!series[DBOnlineSeries.cBannerFileNames].ToString().Contains(seriesBanner.FileName))
                    {
                        DataUpdated = true;                            
                        MPTVSeriesLog.Write("New series widebanner found for \"" + series.ToString() + "\" : " + seriesBanner.OnlinePath, MPTVSeriesLog.LogLevel.Debug);
                        if (series[DBOnlineSeries.cBannerFileNames].ToString().Trim().Length == 0)
                        {
                            series[DBOnlineSeries.cBannerFileNames] += seriesBanner.FileName;
                        }
                        else
                        {
                            series[DBOnlineSeries.cBannerFileNames] += "|" + seriesBanner.FileName;
                        }
                    }
                }

                // Check if current set series widebanner exists
                string lCurrentArtwork = Helper.PathCombine(Settings.GetPath(Settings.Path.banners), series[DBOnlineSeries.cCurrentBannerFileName].ToString());
                bool lCurrentArtworkExists = File.Exists(lCurrentArtwork);

                // if the current artwork exists, but not in the list of available artwork then add it
                if (lCurrentArtworkExists && !series.BannerList.Exists(b => b.Equals(lCurrentArtwork)))
                {
                    var lImageList = new List<string>(series.BannerList);
                    lImageList.Add(lCurrentArtwork);
                    series.BannerList = lImageList;
                    lImageList = null;
                }

                // Don't override user selection of banner
                if ((series[DBOnlineSeries.cCurrentBannerFileName].ToString().Trim().Length == 0 || !lCurrentArtworkExists) && lSeriesArtwork.SeriesWideBanners.Count > 0)
                {
                    // our list of banners are already sorted by local language and rating so choose first one
                    series[DBOnlineSeries.cCurrentBannerFileName] = lSeriesArtwork.SeriesWideBanners[0].FileName;
                }
                #endregion

                #region Series Posters
                foreach (PosterSeries seriesPoster in lSeriesArtwork.SeriesPosters)
                {
                    if (!series[DBOnlineSeries.cPosterFileNames].ToString().Contains(seriesPoster.FileName))
                    {
                        DataUpdated = true;
                        MPTVSeriesLog.Write("New series poster found for \"" + series.ToString() + "\" : " + seriesPoster.OnlinePath, MPTVSeriesLog.LogLevel.Debug);
                        if (series[DBOnlineSeries.cPosterFileNames].ToString().Trim().Length == 0)
                        {
                            series[DBOnlineSeries.cPosterFileNames] += seriesPoster.FileName;
                        }
                        else
                        {
                            series[DBOnlineSeries.cPosterFileNames] += "|" + seriesPoster.FileName;
                        }
                    }
                }

                // Check if current set series poster exists
                lCurrentArtwork = Helper.PathCombine(Settings.GetPath(Settings.Path.banners), series[DBOnlineSeries.cCurrentPosterFileName].ToString());
                lCurrentArtworkExists = File.Exists(lCurrentArtwork);

                // if the current artwork exists, but not in the list of available artwork then add it
                if (lCurrentArtworkExists && !series.PosterList.Exists(b => b.Equals(lCurrentArtwork)))
                {

                    var lImageList = new List<string>(series.PosterList);
                    lImageList.Add(lCurrentArtwork);
                    series.PosterList = lImageList;
                    lImageList = null;
                }

                // Don't override user selection of poster
                if ((series[DBOnlineSeries.cCurrentPosterFileName].ToString().Trim().Length == 0 || !lCurrentArtworkExists) && lSeriesArtwork.SeriesPosters.Count > 0)
                {
                    series[DBOnlineSeries.cCurrentPosterFileName] = lSeriesArtwork.SeriesPosters[0].FileName;
                }
                #endregion

                // commit series record
                series.Commit();

                #region Season Posters
                // update the season artwork for series
                List<DBSeason> lLocalSeasons = DBSeason.Get(new SQLCondition(new DBSeason(), DBSeason.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal), false);

                foreach (DBSeason season in lLocalSeasons)
                {
                    // get season posters from map for the current season
                    var lSeasonPosters = new List<PosterSeason>(lSeriesArtwork.SeasonPosters);
                    lSeasonPosters.RemoveAll(s => s.SeasonIndex != season[DBSeason.cIndex]);

                    // add to existing range of season posters in database
                    foreach (PosterSeason seasonPoster in lSeasonPosters)
                    {
                        if (!season[DBSeason.cBannerFileNames].ToString().Contains(seasonPoster.FileName))
                        {
                            DataUpdated = true;
                            MPTVSeriesLog.Write("New season poster found for \"" + series.ToString() + "\" Season " + season[DBSeason.cIndex] + ": " + seasonPoster.OnlinePath, MPTVSeriesLog.LogLevel.Debug);
                            if (season[DBSeason.cBannerFileNames].ToString().Length == 0)
                            {
                                season[DBSeason.cBannerFileNames] += seasonPoster.FileName;
                            }
                            else
                            {
                                season[DBSeason.cBannerFileNames] += "|" + seasonPoster.FileName;
                            }
                        }
                    }

                    // Check if current set season poster exists
                    lCurrentArtwork = Helper.PathCombine(Settings.GetPath(Settings.Path.banners), season[DBSeason.cCurrentBannerFileName].ToString());
                    lCurrentArtworkExists = File.Exists(lCurrentArtwork);

                    // if the current artwork exists, but not in the list of available artwork then add it
                    if (lCurrentArtworkExists && !season.BannerList.Exists(b => b.Equals(lCurrentArtwork)))
                    {
                        var lImageList = new List<string>(season.BannerList);
                        lImageList.Add(lCurrentArtwork);
                        season.BannerList = lImageList;
                        lImageList = null;
                    }

                    // Don't override user selection of season poster
                    if ((season[DBSeason.cCurrentBannerFileName].ToString().Trim().Length == 0 || !lCurrentArtworkExists) && lSeasonPosters.Count > 0)
                    {
                        season[DBSeason.cCurrentBannerFileName] = lSeasonPosters[0].FileName;
                    }

                    // commit season record
                    season.Commit();
                }
                #endregion
            }
            Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateBanners, lSeries.Count));
        }

        /// <summary>
        /// Removes any episodes from the online_episodes table that no longer exists online
        /// </summary>
        private void CleanEpisodes()
        {
            MPTVSeriesLog.Write(BigLogMessage("Cleaning Online Episode References"));

            int i = 0;
            foreach (var series in mOnlineEpisodes.Keys)
            {
                var seriesObj = Helper.GetCorrespondingSeries(int.Parse(series));
                string seriesName = seriesObj == null ? series : seriesObj.ToString();

                Worker.ReportProgress(0, new ParsingProgress(ParsingAction.CleanupEpisodes, seriesName, ++i, mOnlineEpisodes.Keys.Count));

                // get the current online episodes in database
                var episodes = DBEpisode.Get(int.Parse(series), false);
                if (episodes == null) continue;

                bool cleanEpisodeZero = DBOption.GetOptions(DBOption.cCleanOnlineEpisodeZero);

                var epsRemovedInSeason = new Dictionary<int, int>();
                foreach (var episode in episodes)
                {
                    var expectedEpisodes = mOnlineEpisodes[series];
                    
                    int episodeIdx = episode[DBOnlineEpisode.cEpisodeIndex];
                    int seasonIdx = episode[DBOnlineEpisode.cSeasonIndex];

                    // check if episode is in the expected list of episodes
                    // also check if episode is valid i.e. Idx > 0
                    if (!expectedEpisodes.Any(e => (e[DBOnlineEpisode.cEpisodeIndex] == episodeIdx &&
                                                   e[DBOnlineEpisode.cSeasonIndex] == seasonIdx)) ||
                                                   (cleanEpisodeZero && episodeIdx == 0))
                    {
                        string message = string.Format("{0} - Removing episode {1}x{2}, episode no longer exists online or is invalid", seriesName, seasonIdx, episodeIdx);
                        Worker.ReportProgress(0, new ParsingProgress(ParsingAction.CleanupEpisodes, message, i, mOnlineEpisodes.Keys.Count));

                        // delete local and online references in database
                        episode.DeleteLocalEpisode();
                        episode.DeleteOnlineEpisode();

                        // keep a counter of episodes we remove per season 
                        // if amount removed equals total eps in season then we can remove season
                        int season = episode[DBOnlineEpisode.cSeasonIndex];
                        if (epsRemovedInSeason.ContainsKey(season))
                        {
                            epsRemovedInSeason[season] = epsRemovedInSeason[season] + 1;
                        }
                        else
                        {
                            epsRemovedInSeason.Add(season, 1);
                        }

                        // check if we should clear the season as well
                        int episodesInSeason = episodes.Where(e => e[DBOnlineEpisode.cSeasonIndex] == season).Count();
                        if (episodesInSeason == epsRemovedInSeason[season])
                        {
                            message = string.Format("{0} - Removing season {1}, season no longer has any associated episodes", seriesName, seasonIdx);
                            Worker.ReportProgress(0, new ParsingProgress(ParsingAction.CleanupEpisodes, message, i, mOnlineEpisodes.Keys.Count));

                            var conditions = new SQLCondition();
                            conditions.Add(new DBSeason(), DBSeason.cSeriesID, series, SQLConditionType.Equal);
                            conditions.Add(new DBSeason(), DBSeason.cIndex, episode[DBOnlineEpisode.cSeasonIndex], SQLConditionType.Equal);
                            DBSeason.Clear(conditions);
                        }
                    }
                }
            }
            Worker.ReportProgress(100, new ParsingProgress(ParsingAction.CleanupEpisodes, mOnlineEpisodes.Keys.Count));
        }

        /// <summary>
        /// Check Banners & Posters for existence and corruption. Remove non-existent or corrupt banners from database (to re-download on UpdateBanners)
        /// </summary>
        private void CheckBanners()
        {
            MPTVSeriesLog.Write(BigLogMessage("Verifying ArtWork"));
            var lCondition = new SQLCondition();

            // all series that have an onlineID ( > 0)
            lCondition.Add(new DBOnlineSeries(), DBSeries.cID, 0, SQLConditionType.GreaterThan);
            lCondition.Add(new DBSeries(), DBSeries.cScanIgnore, 0, SQLConditionType.Equal);
            lCondition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);
            
            // and that already had data imported from the online DB
            lCondition.beginGroup();
            lCondition.Add(new DBOnlineSeries(), DBOnlineSeries.cBannerFileNames, string.Empty, SQLConditionType.NotEqual);
            lCondition.nextIsOr = true;
            lCondition.Add(new DBOnlineSeries(), DBOnlineSeries.cPosterFileNames, string.Empty, SQLConditionType.NotEqual);
            lCondition.nextIsOr = false;
            lCondition.endGroup();

            List<DBSeries> lSeries = DBSeries.Get(lCondition, false, false);
            if (lSeries.Count <= 0)
            {
                MPTVSeriesLog.Write("No Artwork found to verify");
                return;
            }

            int lCounter = 0;
            foreach (var series in lSeries)
            {
                MPTVSeriesLog.Write("progress received: VerifyArtwork [{0}/{1}] {2}", ++lCounter, lSeries.Count, series.ToString());
                series.BannerList = series.BannerList.Where(this.CheckArtwork).ToList();
                series.PosterList = series.PosterList.Where(this.CheckArtwork).ToList();
                series.Commit();
            }
        }

        /// <summary>
        /// Check Artwork for existence and corruption
        /// </summary>
        /// <param name="aPath">Path to file</param>
        /// <returns>OK, not OK</returns>
        private bool CheckArtwork(string aPath)
        {
            if (File.Exists(aPath))
            {
                System.Drawing.Image lImage = ImageAllocator.LoadImageFastFromFile(aPath);
                if (lImage == null)
                {
                    MPTVSeriesLog.Write("Removing corrupt artwork from database. Artwork={0}", aPath);
                    return false;
                }
            }
            else
            {
                MPTVSeriesLog.Write("Removing missing artwork from database. Artwork={0}", aPath);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets Actors From Online
        /// </summary>
        /// <param name="updatesOnly">set to true when processing online updates</param>
        /// <param name="updatedSeries">list of series that have had updates online, set to null if updatesOnly is false</param>
        private void UpdateActors(bool updatesOnly, List<DBValue> updatedSeries)
        {
            // exit if we dont want to automatically download fanart
            if (!DBOption.GetOptions(DBOption.cAutoDownloadActors))
                return;

            // get all series in database
            SQLCondition condition = new SQLCondition();
            condition.Add(new DBSeries(), DBSeries.cID, 0, SQLConditionType.GreaterThan);
            condition.Add(new DBSeries(), DBSeries.cScanIgnore, 0, SQLConditionType.Equal);
            condition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);
            List<DBSeries> seriesList = DBSeries.Get(condition, false, false);

            // process updates on series we have locally
            // remove series not of interest
            if (updatesOnly)
            {
                MPTVSeriesLog.Write(BigLogMessage("Processing Actors from Online Updates"));
                seriesList.RemoveAll(s => !updatedSeries.Contains(s[DBSeries.cID]));
            }

            // updating fanart for all series
            if (!updatesOnly)
            {
                MPTVSeriesLog.Write(BigLogMessage("Downloading new and missing Actors for Series"));

                List<int> seriesids = DBActor.GetSeriesWithActors();
                seriesList.RemoveAll(s => seriesids.Contains(s[DBSeries.cID]));
            }

            int nIndex = 0;
            foreach (DBSeries series in seriesList)
            {
                Worker.ReportProgress(0, new ParsingProgress(ParsingAction.GetNewActors, series.ToString(), ++nIndex, seriesList.Count, series, null));

                try
                {
                    // get available banners and add to database
                    GetActors ga = new GetActors(series[DBSeries.cID]);
                    foreach (DBActor a in ga.Actors)
                    {
                        a.Commit();

                        string remoteThumb = a.ImageRemotePath;
                        if (string.IsNullOrEmpty(remoteThumb)) continue;

                        string localThumb = a.Image;
                        if (string.IsNullOrEmpty(localThumb)) continue;

                        if (Helper.DownloadFile(remoteThumb, localThumb))
                        {
                            // notify that thumbnail image has been downloaded
                            a.ThumbnailImage = localThumb;
                            a.NotifyPropertyChanged("ThumbnailImage");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MPTVSeriesLog.Write("Failed to update Actor: " + ex.Message);
                }
            }
            Worker.ReportProgress(0, new ParsingProgress(ParsingAction.GetNewActors, seriesList.Count));
        }

        /// <summary>
        /// Gets Fanart From Online
        /// </summary>
        /// <param name="aUpdatesOnly">set to true when processing online updates</param>
        /// <param name="aUpdatedSeries">list of series that have had updates online, set to null if updatesOnly is false</param>
        private void UpdateFanart(bool aUpdatesOnly, List<int> aUpdatedSeries)
        {
            // exit if we dont want to automatically download fanart
            if (!DBOption.GetOptions(DBOption.cAutoDownloadFanart))
                return;

            // get all series in database
            var lCondition = new SQLCondition();
            lCondition.Add(new DBSeries(), DBSeries.cID, 0, SQLConditionType.GreaterThan);
            lCondition.Add(new DBSeries(), DBSeries.cScanIgnore, 0, SQLConditionType.Equal);
            lCondition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);
            List<DBSeries> lSeriesList = DBSeries.Get(lCondition, false, false);
            
            // process updates on series we have locally
            // remove series not of interest
            if (aUpdatesOnly)
            {
                MPTVSeriesLog.Write(BigLogMessage("Processing Fanart from Online Updates"));
                lSeriesList.RemoveAll(s => !aUpdatedSeries.Contains(s[DBSeries.cID])); 
            }

            // updating fanart for all series
            if (!aUpdatesOnly)
            {
                MPTVSeriesLog.Write(BigLogMessage("Downloading new and missing Fanart for Series"));

                // just process the ones with missing fanart
                if (!DBOption.GetOptions(DBOption.cAutoUpdateAllFanart))
                {
                    List<int> lSeriesIds = DBFanart.GetSeriesWithFanart();
                    lSeriesList.RemoveAll(s => lSeriesIds.Contains(s[DBSeries.cID]));
                }
            }

            int lIndex = 0;
            int lConsecutiveDownloadErrors = 0;
            if (!int.TryParse(DBOption.GetOptions(DBOption.cMaxConsecutiveDownloadErrors).ToString(), out int lMaxConsecutiveDownloadErrors))
            {
                lMaxConsecutiveDownloadErrors = 3;
            }

            foreach (DBSeries series in lSeriesList)
            {
                Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateFanart, series.ToString(), ++lIndex, lSeriesList.Count, series, null));

                try
                {
                    // get available banners and add to database
                    var lFanartsToAdd = new GetFanart(series[DBSeries.cID]);
                    foreach (DBFanart f in lFanartsToAdd.Fanart)
                    {
                        f.Commit();
                    }

                    // get list of fanarts to auto download
                    var lDbFanart = new DBFanart();
                    List<DBFanart> lFanarts = lDbFanart.FanartsToDownload(series[DBSeries.cID]);

                    // download fanart
                    foreach (DBFanart lFanart in lFanarts)
                    {
                        if (lConsecutiveDownloadErrors >= lMaxConsecutiveDownloadErrors)
                        {
                            MPTVSeriesLog.Write("Too many consecutive download errors. Aborting.");
                            return;
                        }
                        
                        // lets put fanart in expected location fanart\original\<seriesID>*.jpg
                        string lLocalFilename = Fanart.GetLocalPath( lFanart );

                        string lResult = OnlineAPI.DownloadBanner( lFanart[DBFanart.cBannerPath], Settings.Path.fanart, lLocalFilename );
                        if (lResult != null)
                        {
                            // commit to database
                            lFanart[DBFanart.cLocalPath] = lLocalFilename;
                            lFanart.Commit();

                            Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateFanart, string.Format("{0} - {1}", series.ToString(), Helper.PathCombine(Settings.GetPath(Settings.Path.fanart), lLocalFilename) ), lIndex, lSeriesList.Count, series, lResult));
                            lConsecutiveDownloadErrors = 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MPTVSeriesLog.Write("Failed to update Fanart. " + ex.Message);
                    lConsecutiveDownloadErrors++;
                }
            }
            Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateFanart, lSeriesList.Count));
        }

        void UpdateTraktCommunityRatings(BackgroundWorker aTraktCommunityRatingWorker, List<int> aSeriesIds, List<DBValue> aEpisodeIds)
        {
            // check if trakt is installed and enabled
            if (!Helper.IsTraktAvailableAndEnabled)
            {
                MPTVSeriesLog.Write("Aborting Trakt community ratings as requirements not met, be sure trakt is installed");
                return;
            }

            MPTVSeriesLog.Write(BigLogMessage("Updating Trakt Community Ratings"), MPTVSeriesLog.LogLevel.Normal);

            var lSeries = new List<DBSeries>();
            bool lForceUpdate = false;

            if (aSeriesIds == null && aEpisodeIds == null)
            {
                // get all series
                var lCondition = new SQLCondition();
                lCondition.Add(new DBSeries(), DBSeries.cID, 0, SQLConditionType.GreaterThan);
                lCondition.Add(new DBSeries(), DBSeries.cScanIgnore, 0, SQLConditionType.Equal);
                lCondition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);
                lSeries = DBSeries.Get(lCondition, false, false);
            }
            else
            {
                lForceUpdate = true;

                // if there are no series ids, then we must be updating a season / episode
                if (aSeriesIds != null && aSeriesIds.Count > 0)
                {
                    // update only select series - typically only a single series
                    aSeriesIds.ForEach(id => lSeries.Add(DBSeries.Get(id, false)));
                }
                else if (aEpisodeIds != null & aEpisodeIds.Count > 0)
                {
                    // get the series being updated from from the first episode id
                    List<DBEpisode> episodes = DBEpisode.Get("SELECT * FROM online_episodes WHERE EpisodeID = " + aEpisodeIds.First());
                    lSeries.Add(DBSeries.Get(episodes.First()[DBOnlineEpisode.cSeriesID], false));
                }
            }

            aTraktCommunityRatingWorker.DoWork += new DoWorkEventHandler(ASyncTraktCommunityRatings);
            aTraktCommunityRatingWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ASyncTraktCommunityRatingsCompleted);

            // parse a tuple into the worker method where T1=The list of series and T2=Force Update
            // Force update if only updating selected series
            aTraktCommunityRatingWorker.RunWorkerAsync(Tuple.Create(lSeries, lForceUpdate));
        }

        void ASyncTraktCommunityRatingsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MPTVSeriesLog.Write(BigLogMessage("Trakt Community Ratings Updated in Database"), MPTVSeriesLog.LogLevel.Debug);
        }

        void ASyncTraktCommunityRatings(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            Thread.CurrentThread.Name = "Ratings";

            var lTupleParams = e.Argument as Tuple<List<DBSeries>, bool>;
            List<DBSeries> lSeriesList = lTupleParams.Item1;
            bool lForceUpdate = lTupleParams.Item2;

            // get list of updated shows from trakt
            DateTime lDateLastUpdated = DateTime.MinValue;
            string lLastUpdated = DBOption.GetOptions(DBOption.cTraktLastDateUpdated);
            var lRecentSeries = new HashSet<string>();
            IEnumerable<TraktShowUpdate> lUpdatedShows = null;

            // ensure we set the TraktAPI client ID if we are using the configuration tool
            // dont't need to worry about this when running inside MP as it will be initialised by
            // the trakt plugin - we could register an ID for the tvseries plugin
            if (Settings.IsConfig)
            {
                TraktAPI.TraktAPI.ClientId = "49e6907e6221d3c7e866f9d4d890c6755590cf4aa92163e8490a17753b905e57";
                using (MediaPortal.Profile.Settings aXmlreader = new MediaPortal.Profile.MPSettings())
                {
                    // this should not be required but for some reason trakt is complaining.
                    TraktAPI.TraktAPI.UserAccessToken = aXmlreader.GetValueAsString("Trakt", "UserAccessToken", "");
                }
            }
 
            #region Recently Updated Shows
            if (!string.IsNullOrEmpty(lLastUpdated) && !lForceUpdate)
            {
                if (DateTime.TryParse(lLastUpdated, out lDateLastUpdated))
                {
                    int lPage = 1;
                    int lMaxPageSize = 1000;

                    MPTVSeriesLog.Write(string.Format("Requesting list of recently updated series from trakt.tv, Page = '{0}', Last Update Time = '{1}'", lPage, lLastUpdated), MPTVSeriesLog.LogLevel.Normal);
                    TraktShowsUpdated lUpdatedShowsResult = TraktAPI.TraktAPI.GetRecentlyUpdatedShows(lDateLastUpdated.ToUniversalTime().ToString("yyyy-MM-dd"), lPage, lMaxPageSize);
                    if (lUpdatedShowsResult != null)
                    { 
                        lUpdatedShows = lUpdatedShowsResult.Shows;
                    }
                    
                    while (lUpdatedShowsResult != null && lUpdatedShowsResult.Shows.Count() == lMaxPageSize)
                    {
                        MPTVSeriesLog.Write(string.Format("Requesting list of recently updated series from trakt.tv, Page = '{0}'", ++lPage), MPTVSeriesLog.LogLevel.Normal);
                        lUpdatedShowsResult = TraktAPI.TraktAPI.GetRecentlyUpdatedShows(lDateLastUpdated.ToUniversalTime().ToString("yyyy-MM-dd"), lPage, lMaxPageSize);
                        if (lUpdatedShowsResult != null)
                        {
                            lUpdatedShows = lUpdatedShows.Union(lUpdatedShowsResult.Shows);
                        }
                    }
                }

                #region Forced Updates
                // if there has been some new episodes recently added to the database (since last trakt update)
                // force an update on the series such that the ratings are up to date
                List<DBEpisode> lRecentlyAddedEpisodes = DBEpisode.GetMostRecent(lDateLastUpdated);
                lRecentlyAddedEpisodes.GroupBy(ep => ep[DBOnlineEpisode.cSeriesID]).ToList().ForEach(s => lRecentSeries.Add(s.Key));
                #endregion
            }
            #endregion

            int lIndex = 0;
            foreach (DBSeries series in lSeriesList)
            {
                // Update Progress
                Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateCommunityRatings, series[DBOnlineSeries.cPrettyName], ++lIndex, lSeriesList.Count, series, null));

                // get the trakt id for the series
                // if not found do a lookup and store it for next time
                string lTmdbId = series[DBOnlineSeries.cTmdbId];
                string lTraktId = series[DBOnlineSeries.cTraktID];
                string lSeriesName = series[DBOnlineSeries.cPrettyName];

                #region Trakt ID Lookup
                if (string.IsNullOrEmpty(lTraktId))
                {
                    MPTVSeriesLog.Write($"Searching for series Trakt ID, Title = '{lSeriesName}', TMDb ID = '{lTmdbId ?? "<empty>"}'", MPTVSeriesLog.LogLevel.Debug);

                    if (string.IsNullOrEmpty(lTmdbId))
                        continue;

                    // search by tvdb id
                    IEnumerable<TraktSearchResult> lSearchResults = TraktAPI.TraktAPI.SearchById("tmdb", lTmdbId, "show");

                    // if there is more than one result it could be an episode or season with the same id
                    // get the first show result from the list
                    if (lSearchResults == null || lSearchResults.Count() == 0)
                    {
                        MPTVSeriesLog.Write($"Aborting community series rating, failed to retrieve Trakt ID. Title = '{lSeriesName}', TMDb ID = '{lTmdbId}'"); 
                        continue;
                    }

                    // match up series with our search result
                    TraktSearchResult lTraktSeries = lSearchResults.FirstOrDefault(r => r.Type == "show");
                    if (lTraktSeries == null || lTraktSeries.Show.Ids.Tmdb != int.Parse(lTmdbId))
                    {
                        MPTVSeriesLog.Write($"Aborting community series rating, failed to retrieve Trakt ID. Title = '{lSeriesName}', TMDb ID = '{lTmdbId}'"); 
                        continue;
                    }

                    // store the id for later
                    lTraktId = lTraktSeries.Show.Ids.Trakt.ToString();
                    Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateCommunityRatings, $"{lSeriesName} - Trakt Lookup Complete, ID = {lTraktId}", lIndex, lSeriesList.Count, series, null));

                    series[DBOnlineSeries.cTraktID] = lTraktId;
                    series.Commit();
                }
                #endregion
                
                #region Series Community Ratings

                #region Update Check
                // only update ratings for series if it is a series that has recently been updated on trakt.
                // we don't want to update every series and underlying episode every sync!!!
                // skip update check on a series if it was recently added
                if (lUpdatedShows != null && !lRecentSeries.Contains(lTmdbId))
                {
                    // if the series hasn't been updated recently continue on to next
                    TraktShowUpdate lUpdatedShow = lUpdatedShows.FirstOrDefault(u => u.Show.Ids.Trakt.ToString() == lTraktId);

                    if (lUpdatedShow == null)
                    {
                        MPTVSeriesLog.Write($"Skipping community ratings update for series, the series has not been found in the update list. Title = '{lSeriesName}', TMDb ID = '{lTmdbId}', Trakt ID = '{lTraktId}'", MPTVSeriesLog.LogLevel.Debug);
                        continue;
                    }
                    else
                    {
                        // check that the show updated_at is newer than last time we did an update
                        if (DateTime.TryParse(lUpdatedShow.UpdatedAt, out DateTime lDateShowUpdated))
                        {
                            if (lDateShowUpdated.ToUniversalTime() < lDateLastUpdated.ToUniversalTime())
                            {
                                MPTVSeriesLog.Write($"Skipping community ratings update for series, the series has not been updated recently. Title = '{lSeriesName}', TMDb ID = '{lTmdbId}', Trakt ID = '{lTraktId}', Local Update Time = '{lDateLastUpdated}', Online Update Time = '{DateTime.Parse(lUpdatedShow.UpdatedAt)}'", MPTVSeriesLog.LogLevel.Debug);
                                continue;
                            }
                        }
                    }
                }
                #endregion

                // get series ratings from trakt
                MPTVSeriesLog.Write($"Requesting series ratings from trakt.tv. Title = '{lSeriesName}', TMDb ID = '{lTmdbId}', Trakt ID = '{lTraktId}'", MPTVSeriesLog.LogLevel.Debug);

                TraktRating lSeriesRatings = TraktAPI.TraktAPI.GetShowRatings(lTraktId);
                if (lSeriesRatings == null)
                {
                    MPTVSeriesLog.Write($"Failed to get series ratings from trakt.tv. Title = '{lSeriesName}', TMDb ID = '{lTmdbId}', Trakt ID = '{lTraktId}'");
                    continue;
                }

                // update database with community rating and votes - if it has changed
                if (series[DBOnlineSeries.cRatingCount] != lSeriesRatings.Votes)
                {
                    string lRating = Math.Round(lSeriesRatings.Rating ?? 0.0, 2).ToString();

                    Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateCommunityRatings, $"{lSeriesName} - Rating = {lRating}, Votes = {lSeriesRatings.Votes}", lIndex, lSeriesList.Count, series, null));

                    series[DBOnlineSeries.cRating] = lRating;
                    series[DBOnlineSeries.cRatingCount] = lSeriesRatings.Votes;
                    series.Commit();
                }
                #endregion

                #region Season and Episode Community Ratings
                // to get the episode ratings we could call the GetEpisodeRatings method on each episode but that's too much work
                // if we call the summary method for seasons, we can get each underlying episode
                // if we also request the 'episodes' extended parameter. We also need to get 'full' data to get the ratings
                // as an added bonus we can also get season overviews and ratings which are not provided by theTVDb API
                MPTVSeriesLog.Write($"Requesting season information for series from trakt.tv. Title = '{lSeriesName}', TMDb ID = '{lTmdbId}', Trakt ID = '{lTraktId}'", MPTVSeriesLog.LogLevel.Debug);
                IEnumerable<TraktSeasonSummary> lTraktSeasons = TraktAPI.TraktAPI.GetShowSeasons(lTraktId, "episodes,full");
                if (lTraktSeasons == null)
                {
                    MPTVSeriesLog.Write($"Failed to get season information for series from trakt.tv. Title = '{lSeriesName}', TMDb ID = '{lTmdbId}', Trakt ID = '{lTraktId}'");
                    continue;
                }

                #region Seasons
                // get seasons from local database for current series
                var lConditions = new SQLCondition(new DBSeason(), DBSeason.cSeriesID, lTmdbId, SQLConditionType.Equal);
                List<DBSeason> lSeasons = DBSeason.Get(lConditions, false);

                foreach (DBSeason season in lSeasons)
                {
                    TraktSeasonSummary lTraktSeason = lTraktSeasons.FirstOrDefault(s => s.Number == season[DBSeason.cIndex]);
                    if (lTraktSeason == null) continue;

                    // update database with community rating and votes - if it has changed
                    if (season[DBSeason.cRatingCount] != lTraktSeason.Votes)
                    {
                        string lRating = Math.Round(lTraktSeason.Rating ?? 0.0, 2).ToString();

                        Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateCommunityRatings, $"{lSeriesName} - Season = {lTraktSeason.Number}, Rating = {lRating}, Votes = {lTraktSeason.Votes}", lIndex, lSeriesList.Count, series, null));

                        season[DBOnlineEpisode.cRating] = lRating;
                        season[DBOnlineEpisode.cRatingCount] = lTraktSeason.Votes;
                        season.Commit();
                    }

                    // bonus: update the season summary while we are here
                    if (!string.IsNullOrEmpty(lTraktSeason.Overview) && lTraktSeason.Overview != season[DBSeason.cSummary])
                    {
                        season[DBSeason.cSummary] = lTraktSeason.Overview;
                        season.Commit();
                    }
                }

                #endregion

                #region Episodes
                // get episodes from local database for current series
                lConditions = new SQLCondition(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, lTmdbId, SQLConditionType.Equal);
                lConditions.AddOrderItem(DBOnlineEpisode.Q(DBOnlineEpisode.cID), SQLCondition.orderType.Ascending);

                List<DBEpisode> lEpisodes = DBEpisode.Get(lConditions, false);
                if (lEpisodes == null) continue;

                foreach (DBEpisode episode in lEpisodes)
                {
                    TraktSeasonSummary lTraktSeason = lTraktSeasons.FirstOrDefault(s => s.Number == episode[DBOnlineEpisode.cSeasonIndex]);
                    if (lTraktSeason == null) continue;

                    TraktEpisodeSummary lTraktEpisode = lTraktSeason.Episodes.FirstOrDefault(ep => ep.Number == episode[DBOnlineEpisode.cEpisodeIndex]);
                    if (lTraktEpisode == null) continue;

                    // update database with community rating and votes - if it has changed
                    if (episode[DBOnlineEpisode.cRatingCount] != lTraktEpisode.Votes)
                    {
                        string lRating = Math.Round(lTraktEpisode.Rating ?? 0.0, 2).ToString();

                        Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateCommunityRatings, $"{lSeriesName} - Episode = {lTraktSeason.Number}x{lTraktEpisode.Number}, Rating = {lRating}, Votes = {lTraktEpisode.Votes}", lIndex, lSeriesList.Count, series, null));
                        
                        episode[DBOnlineEpisode.cRating] = lRating;
                        episode[DBOnlineEpisode.cRatingCount] = lTraktEpisode.Votes;
                        episode.Commit();
                    }
                }
                #endregion

                #endregion
            }

            // store last time updated
            if (!lForceUpdate)
            {
                DBOption.SetOptions(DBOption.cTraktLastDateUpdated, DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            }
            Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateCommunityRatings, lSeriesList.Count));
        }

        void UpdateUserRatings(BackgroundWorker aUserRatingsWorker)
        {
            MPTVSeriesLog.Write(BigLogMessage("Updating User Ratings"), MPTVSeriesLog.LogLevel.Normal);
            
            var lCondition = new SQLCondition();
            lCondition.Add(new DBSeries(), DBSeries.cID, 0, SQLConditionType.GreaterThan);
            lCondition.Add(new DBSeries(), DBSeries.cScanIgnore, 0, SQLConditionType.Equal);
            lCondition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);
            List<DBSeries> lSeries = DBSeries.Get(lCondition, false, false);

            aUserRatingsWorker.DoWork += new DoWorkEventHandler(ASyncUserRatings);
            aUserRatingsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ASyncUserRatingsCompleted);
            aUserRatingsWorker.RunWorkerAsync(lSeries);
        }

        void ASyncUserRatingsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MPTVSeriesLog.Write(BigLogMessage("User Ratings Updated in Database"), MPTVSeriesLog.LogLevel.Debug);
        }

        void ASyncUserRatings(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            string sAccountID = DBOption.GetOptions(DBOption.cOnlineUserID);
            bool traktCommunityRatings = DBOption.GetOptions(DBOption.cTraktCommunityRatings) && Helper.IsTraktAvailableAndEnabled;

            if (!String.IsNullOrEmpty(sAccountID))
            {
                List<DBSeries> seriesList = (List<DBSeries>)e.Argument;

                int nIndex = 0;
                if (DBOption.GetOptions(DBOption.cAutoUpdateEpisodeRatings)) // i.e. update Series AND Underlying Episodes
                {
                    bool markWatched = DBOption.GetOptions(DBOption.cMarkRatedEpisodeAsWatched);
                    
                    // get all episodes in database
                    List<DBEpisode> episodes = DBEpisode.Get(new SQLCondition());

                    foreach (DBSeries series in seriesList)
                    {
                        MPTVSeriesLog.Write("Retrieving user ratings for series: " + series[DBOnlineSeries.cPrettyName], MPTVSeriesLog.LogLevel.Debug);
                        GetUserRatings userRatings = new GetUserRatings(series[DBOnlineSeries.cID], sAccountID);
                        
                        // Update Progress
                        Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateUserRatings, series[DBOnlineSeries.cPrettyName], ++nIndex, seriesList.Count, series, null));

                        // Set Series Ratings
                        // We should also update Community Rating as theTVDB Updates API doesnt take into consideration
                        // Series/Episodes that have rating changes.
                        if (!String.IsNullOrEmpty(userRatings.SeriesUserRating))
                        {
                            series[DBOnlineSeries.cMyRating] = userRatings.SeriesUserRating;
                            series[DBOnlineSeries.cMyRatingAt] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        }

                        // Dont clear site rating if user rating does not exist
                        // also dont update if we get community ratings from trakt
                        if (!traktCommunityRatings && !String.IsNullOrEmpty(userRatings.SeriesCommunityRating))
                            series[DBOnlineSeries.cRating] = userRatings.SeriesCommunityRating;
                        
                        series.Commit();

                        // Set Episode Ratings
                        foreach (var episode in episodes.Where(ep => ep[DBEpisode.cSeriesID] == series[DBSeries.cID]))
                        {
                            if (userRatings.EpisodeRatings.ContainsKey(episode[DBOnlineEpisode.cID]))
                            {
                                episode[DBOnlineEpisode.cMyRating] = userRatings.EpisodeRatings[episode[DBOnlineEpisode.cID]].UserRating;
                                episode[DBOnlineEpisode.cMyRatingAt] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                // dont update community rating if we get from trakt.tv
                                if (!traktCommunityRatings)
                                {
                                    episode[DBOnlineEpisode.cRating] = userRatings.EpisodeRatings[episode[DBOnlineEpisode.cID]].CommunityRating;
                                }

                                if (markWatched)
                                {
                                    episode[DBOnlineEpisode.cWatched] = true;
                                    if (episode[DBOnlineEpisode.cPlayCount] == 0)
                                        episode[DBOnlineEpisode.cPlayCount] = 1;
                                    if (string.IsNullOrEmpty(episode[DBOnlineEpisode.cLastWatchedDate]))
                                        episode[DBOnlineEpisode.cLastWatchedDate] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    if (string.IsNullOrEmpty(episode[DBOnlineEpisode.cFirstWatchedDate]))
                                        episode[DBOnlineEpisode.cFirstWatchedDate] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                }

                                episode.Commit();
                            }
                        }
                    }
                }
                else // update Series only, not Episodes
                {
                    MPTVSeriesLog.Write("Retrieving user ratings for all series", MPTVSeriesLog.LogLevel.Debug);
                    GetUserRatings userRatings = new GetUserRatings(null, sAccountID);
                    foreach (DBSeries series in seriesList)
                    {
                        // Update Progress
                        Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateUserRatings, series[DBOnlineSeries.cPrettyName], ++nIndex, seriesList.Count, series, null));

                        if (userRatings.SeriesUserRatings.ContainsKey(series[DBSeries.cID]))
                        {
                            if (!String.IsNullOrEmpty(userRatings.SeriesUserRatings[series[DBSeries.cID]]))
                            {
                                series[DBOnlineSeries.cMyRating] = userRatings.SeriesUserRatings[series[DBSeries.cID]];
                                series[DBOnlineSeries.cMyRatingAt] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                series.Commit();
                            }
                        }

                        if (userRatings.SeriesCommunityRatings.ContainsKey(series[DBSeries.cID]))
                        {
                            // dont update community rating if we get from trakt.tv
                            if (!traktCommunityRatings && !String.IsNullOrEmpty(userRatings.SeriesCommunityRatings[series[DBSeries.cID]]))
                            {
                                series[DBOnlineSeries.cRating] = userRatings.SeriesCommunityRatings[series[DBSeries.cID]];
                                series.Commit();
                            }                            
                        }
                    }
                }
                Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateUserRatings, seriesList.Count));
            }            
        }

        public void UpdateUserFavourites()
        {
            string sAccountID = DBOption.GetOptions(DBOption.cOnlineUserID);
            int nIndex = 0;

            if (!String.IsNullOrEmpty(sAccountID)) {
                MPTVSeriesLog.Write(BigLogMessage("Updating User Favourites"), MPTVSeriesLog.LogLevel.Normal);

                GetUserFavourites userFavourites = new GetUserFavourites(sAccountID);

                SQLCondition condition = new SQLCondition();
                condition.Add(new DBSeries(), DBSeries.cID, 0, SQLConditionType.GreaterThan);
                condition.Add(new DBSeries(), DBSeries.cScanIgnore, 0, SQLConditionType.Equal);
                condition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);
                List<DBSeries> seriesList = DBSeries.Get(condition, false, false);

                foreach (DBSeries series in seriesList) {
                    // Update Progress
                    Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateUserFavourites, series[DBOnlineSeries.cPrettyName], ++nIndex, seriesList.Count, series, null));

                    if (userFavourites.Series.Contains(series[DBOnlineSeries.cID])) {
                        MPTVSeriesLog.Write("Retrieved favourite series: " + Helper.GetCorrespondingSeries((int)series[DBOnlineSeries.cID]), MPTVSeriesLog.LogLevel.Debug);
                        series[DBOnlineSeries.cViewTags] = Helper.GetSeriesViewTags(series, true, DBView.cTranslateTokenOnlineFavourite);
                        series.Commit();
                    } else {
                        series[DBOnlineSeries.cViewTags] = Helper.GetSeriesViewTags(series, false, DBView.cTranslateTokenOnlineFavourite);
                        series.Commit();
                    }
                }
                Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateUserFavourites, seriesList.Count));
            }            
        }

        public void UpdateEpisodeThumbNails()
        {
            if (DBOption.GetOptions(DBOption.cGetEpisodeSnapshots) == true) 
            {
                MPTVSeriesLog.Write(BigLogMessage("Updating Episode Thumbnails"), MPTVSeriesLog.LogLevel.Normal);

                // Get all online episodes that have an image but not yet downloaded                
                string lQuery;

                if (Settings.IsConfig)
                {
                    // Be more thorough in configuration, user may have deleted thumbs locally
                    lQuery = "select * from online_episodes where ThumbURL != '' order by SeriesID asc";
                }
                else
                {
                    lQuery = "select * from online_episodes where ThumbURL != '' and thumbFilename = '' order by SeriesID asc";
                }

                DBSeries lSeries = null;
                List<DBEpisode> lEpisodes = DBEpisode.Get(lQuery);
                var lEpisodesThumbsForDownload = new List<DBEpisode>();

                #region Check for thumbs to download
                foreach (DBEpisode episode in lEpisodes)
                {
                    if (lSeries == null || lSeries[DBSeries.cID] != episode[DBEpisode.cSeriesID])
                        lSeries = Helper.GetCorrespondingSeries(episode[DBOnlineEpisode.cSeriesID]);

                    if (lSeries == null) continue;

                    string lThumbFilename = episode.BuildEpisodeThumbFilename();
                    string lCompletePath = Helper.PathCombine(Settings.GetPath(Settings.Path.banners), lThumbFilename);

                    // if it doesn't exist download it
                    if (!File.Exists(lCompletePath))
                    {
                        lEpisodesThumbsForDownload.Add(episode);
                    }
                    else
                    {
                        // if we already have the file check db entry
                        if (!episode[DBOnlineEpisode.cEpisodeThumbnailFilename].ToString().Equals(lThumbFilename))
                        {
                            episode[DBOnlineEpisode.cEpisodeThumbnailFilename] = lThumbFilename;
                            episode.Commit();
                        }
                    }
                }
                #endregion

                #region Download Thumbs
                int lIndex = 0;

                foreach (DBEpisode episode in lEpisodesThumbsForDownload)
                {
                    if (lSeries == null || lSeries[DBSeries.cID] != episode[DBEpisode.cSeriesID])
                        lSeries = Helper.GetCorrespondingSeries(episode[DBOnlineEpisode.cSeriesID]);

                    if (lSeries == null) continue;

                    string lThumbFilename = episode.BuildEpisodeThumbFilename();
                    string lCompletePath = Helper.PathCombine(Settings.GetPath(Settings.Path.banners), lThumbFilename);
                    string lUrl = OnlineAPI.GetTMDbBasePath() + episode[DBOnlineEpisode.cEpisodeThumbnailUrl];

                    MPTVSeriesLog.Write($"New Episode Image found for \"{episode.ToString()}\": {episode[DBOnlineEpisode.cEpisodeThumbnailUrl]}", MPTVSeriesLog.LogLevel.Debug);
                    
                    var lWebClient = new WebClient();
                    lWebClient.Headers.Add("user-agent", Settings.UserAgent);

                    // .NET 4.0: Use TLS v1.2. Many download sources no longer support the older and now insecure TLS v1.0/1.1 and SSL v3.
                    ServicePointManager.SecurityProtocol = ( SecurityProtocolType )0xc00;

                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(lCompletePath));
                        
                        if (!lUrl.Contains(".jpg"))
                        {
                            MPTVSeriesLog.Write($"Episode thumbnail '{lUrl}' is incorrect");
                            episode[DBOnlineEpisode.cEpisodeThumbnailUrl] = "";
                            episode[DBOnlineEpisode.cEpisodeThumbnailFilename] = "";
                        }
                        else
                        {
                            MPTVSeriesLog.Write("Downloading new episode image: " + lUrl, MPTVSeriesLog.LogLevel.Debug);
                            lWebClient.DownloadFile(lUrl, lCompletePath);

                            Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateEpisodeThumbNails, episode.ToString(), ++lIndex, lEpisodesThumbsForDownload.Count, episode, lCompletePath));
                        }
                    }
                    catch (WebException)
                    {
                        MPTVSeriesLog.Write("Episode thumbnail download failed ( " + lUrl + " )");
                        lThumbFilename = "";

                        // try to delete file if it exists on disk. maybe download was cut short. Re-download next time
                        try { File.Delete(lCompletePath); } catch { }
                    }
                    episode[DBOnlineEpisode.cEpisodeThumbnailFilename] = lThumbFilename;
                    episode.Commit();
                }
                #endregion

                Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateEpisodeThumbNails, lEpisodesThumbsForDownload.Count));
            }
        }

        void UpdateEpisodeCounts(BackgroundWorker aEpisodeCounterWorker)
        {
            MPTVSeriesLog.Write(BigLogMessage("Updating Episode Counts"));

            aEpisodeCounterWorker.WorkerReportsProgress = true;

            var lCondition = new SQLCondition();
            lCondition.Add(new DBSeries(), DBSeries.cID, 0, SQLConditionType.GreaterThan);            
            lCondition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);
            List<DBSeries> lSeries = DBSeries.Get(lCondition, false, false);            

            if (lSeries.Count > 0)
            {
                aEpisodeCounterWorker.DoWork += new DoWorkEventHandler(ASyncEpisodeCounts);
                aEpisodeCounterWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ASyncEpisodeCountsCompleted);
                aEpisodeCounterWorker.ProgressChanged += new ProgressChangedEventHandler((s, e) =>
                {
                    if (Worker.IsBusy)
                    {
                        object[] lUserState = e.UserState as object[];
                        Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateEpisodeCounts, (lUserState[0] as DBSeries).ToString(), (int)lUserState[1], lSeries.Count));
                    }
                });
                aEpisodeCounterWorker.RunWorkerAsync(lSeries);
            }
        }

        void ASyncEpisodeCountsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Worker.IsBusy)
                Worker.ReportProgress(0, new ParsingProgress(ParsingAction.UpdateEpisodeCounts, (int)e.Result));
            
            MPTVSeriesLog.Write("Update of Episode Counts complete", MPTVSeriesLog.LogLevel.Debug);
        }

        void ASyncEpisodeCounts(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            var lSeries = (List<DBSeries>)e.Argument;
            var lWorker = sender as BackgroundWorker;

            int lIndex = 1;
            Dictionary<string, List<EpisodeCounter>> lEpisodesForCount = DBSeries.GetEpisodesForCount();

            foreach (DBSeries series in lSeries)
            {
                lWorker.ReportProgress(0, new object[] { series, lIndex++ });
                DBSeries.UpdateEpisodeCounts(series, lEpisodesForCount);
            }
            e.Result = lSeries.Count;
        }

        void MediaInfoParse(BackgroundWorker aMediaInfoWorker)
        {
            aMediaInfoWorker.WorkerReportsProgress = true;

            var lCondition = new SQLCondition();
            lCondition.Add(new DBEpisode(), DBEpisode.cFilename, "", SQLConditionType.NotEqual);
            lCondition.Add(new DBEpisode(), DBEpisode.cVideoWidth, "0", SQLConditionType.Equal);

            // Playtime decrements by one every failed attempt(0,-1,-2,..,-5), dont attempt future scans if done more than Maximum attempts
            lCondition.beginGroup();
            lCondition.Add(new DBEpisode(), DBEpisode.cLocalPlaytime, (DBEpisode.MAX_MEDIAINFO_RETRIES*-1), SQLConditionType.GreaterThan);
            lCondition.nextIsOr = true;
            lCondition.AddCustom( DBEpisode.cTableName + "." + DBEpisode.cLocalPlaytime + " IS NULL");
            lCondition.endGroup();

            var lEpisodes = new List<DBEpisode>();
            // get all the episodes
            lEpisodes = DBEpisode.Get(lCondition, false);

            if (lEpisodes.Count > 0)
            {
                aMediaInfoWorker.DoWork += new DoWorkEventHandler(ASyncReadResolutions);
                aMediaInfoWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ASyncReadResolutionsCompleted);
                aMediaInfoWorker.ProgressChanged += new ProgressChangedEventHandler((s, e) =>
                {
                    if (Worker.IsBusy)
                    { // cannot report progress of finished worker - mepo dies
                        object[] userState = e.UserState as object[];
                        Worker.ReportProgress(0, new ParsingProgress(ParsingAction.MediaInfo, (userState[0] as DBEpisode)[DBEpisode.cFilenameWOPath], (int)userState[1], lEpisodes.Count));
                    }
                });
                aMediaInfoWorker.RunWorkerAsync(lEpisodes);

            }
            else
            {
                MPTVSeriesLog.Write("All episodes already contain MediaInfo");
            }
        }

        void ASyncReadResolutionsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Worker.IsBusy) { // cannot report progress of finished worker - mepo dies
                Worker.ReportProgress(0, new ParsingProgress(ParsingAction.MediaInfo, (int)e.Result));
            }
            MPTVSeriesLog.Write($"Update of MediaInfo complete, processed {e.Result} files", MPTVSeriesLog.LogLevel.Debug);
        }

        void ASyncReadResolutions(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            var lEpisodes = (List<DBEpisode>)e.Argument;
            var lWorker = sender as BackgroundWorker;

            int lIndex = 1;
            foreach (DBEpisode ep in lEpisodes)
            {
                lWorker.ReportProgress(0, new object[] { ep, lIndex++ });
                ep.ReadMediaInfo();
            }
            e.Result = lEpisodes.Count;
        }

        #endregion

        #region SeriesHelpers
        public static DBOnlineSeries SearchForSeries(string seriesName, bool bNoExactMatch, IFeedback feedback)
        {
            string SelLang = string.Empty;
            string nameToSearch = seriesName;

            while (true) 
            {
                // query online db for possible matches
                GetSeries GetSeriesParser = new GetSeries(nameToSearch);

                // try to find an exact match in our results, if found, return               
                if (GetSeriesParser.PerfectMatch != null && !bNoExactMatch)
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
                    // Other language for the Series?
                    if (DBOption.GetOptions(DBOption.cOverrideLanguage))
                    {
                        //Get the prefered language for the Series.
                        SelLang = Online_Parsing_Classes.OnlineAPI.GetLanguageOverride(onlineSeries[DBOnlineSeries.cID]);
                    }
                    else
                    {
                        SelLang = Online_Parsing_Classes.OnlineAPI.SelectedLanguage;
                    }

                    if (!uniqueSeriesIds.ContainsKey(onlineSeries[DBOnlineSeries.cID]))
                        uniqueSeriesIds.Add(onlineSeries[DBOnlineSeries.cID], onlineSeries);
                    else if (onlineSeries["language"] == SelLang)
                        uniqueSeriesIds[onlineSeries[DBOnlineSeries.cID]] = onlineSeries;
                }
                foreach (KeyValuePair<int, DBOnlineSeries> onlineSeries in uniqueSeriesIds)
                {
                    Choices.Add(new CItem(onlineSeries.Value[DBOnlineSeries.cPrettyName],
                        "SeriesID: " + onlineSeries.Value[DBOnlineSeries.cID] + Environment.NewLine +
                        onlineSeries.Value[DBOnlineSeries.cSummary],
                        onlineSeries.Value));
                }

                if (Choices.Count == 0) 
                    Choices.Add(new CItem(Translation.CFS_No_Match_Manual_Search, String.Empty, null));

                ChooseFromSelectionDescriptor descriptor = new ChooseFromSelectionDescriptor();                
                descriptor.m_sItemToMatchLabel = Translation.CFS_Local_Series;
                descriptor.m_sItemToMatch = nameToSearch;
                descriptor.m_List = Choices;

                bool bKeepTrying = true;
                while (bKeepTrying) {
                    CItem Selected = null;
                    ReturnCode result = feedback.ChooseFromSelection(descriptor, out Selected);
                    switch (result) {
                        case ReturnCode.Cancel:
                            MPTVSeriesLog.Write("User cancelled Series Selection");
                            return null;

                        case ReturnCode.Ignore:
                            MPTVSeriesLog.Write("User chose to Ignore \"" + nameToSearch + "\" in the future, setting Hidden=True and ScanIgnore=True");
                            nameToSearch = null;
                            DBSeries series = new DBSeries(seriesName);
                            series[DBSeries.cScanIgnore] = 1; // means it will be skipped in the future
                            series[DBSeries.cHidden] = true;
                            series.Commit();
                            return null;

                        case ReturnCode.OK:
                            DBOnlineSeries selectedSeries = Selected.m_Tag as DBOnlineSeries;

                            // Show the Virtual Keyboard to manual enter in name to search                            
                            if (selectedSeries == null && !Settings.IsConfig)
                            {
                                GetStringFromUserDescriptor Keyboard = new GetStringFromUserDescriptor();
                                Keyboard.Text = nameToSearch;

                                if (feedback.GetStringFromUser(Keyboard, out nameToSearch) == ReturnCode.OK)
                                {
                                    // Search again using manually entered name
                                    bKeepTrying = false;
                                }
                                else
                                {
                                    MPTVSeriesLog.Write("User cancelled Series Selection");
                                    return null;
                                }
                            }
                            else if (nameToSearch != Selected.m_sName || selectedSeries == null)
                            {
                                nameToSearch = Selected.m_sName;
                                bKeepTrying = false;
                            }
                            else
                            {
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

        private void SetEpisodeOrderForSeries(DBSeries series)
        {
            try 
            {
                // prompt to choose episode order only in GUI.
                // episode orders can be set from import wizard in configuration
                if (string.IsNullOrEmpty(series[DBOnlineSeries.cChosenEpisodeOrder]) && !Settings.IsConfig)
                {
                    List<string> episodeOrders = new List<string>(series[DBOnlineSeries.cEpisodeOrders].ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
                    if (episodeOrders.Count > 1 && !DBOption.GetOptions(DBOption.cAutoChooseOrder))
                    {
                        MPTVSeriesLog.Write(string.Format("\"{0}\" supports {1} different ordering options, asking user...", series.ToString(), episodeOrders.Count), MPTVSeriesLog.LogLevel.Debug);
                        // let the user choose
                        string helpText = "Some series expose several ways in which they are ordered, for instance a DVD-release may differ from the original Air schedule." + Environment.NewLine +
                                          "Note that your file numbering must match the option you choose here." + Environment.NewLine +
                                          "Choose the default \"Aired\" option unless you have a specific reason not to!";

                        List<CItem> Choices = new List<CItem>();
                        foreach (string orderOption in episodeOrders)
                            Choices.Add(new CItem(orderOption, helpText, orderOption));
                        Choices.Add(new CItem("Title", helpText, "Title"));

                        ChooseFromSelectionDescriptor descriptor = new ChooseFromSelectionDescriptor();
                        descriptor.m_sTitle = "Multiple ordering Options detected";
                        descriptor.m_sItemToMatchLabel = Translation.ChangeOnlineMatchOrder + ":";
                        descriptor.m_sItemToMatch = series[DBOnlineSeries.cPrettyName];
                        descriptor.m_sListLabel = "Please choose the desired Option:";
                        descriptor.m_List = Choices;
                        descriptor.m_useRadioToSelect = true;
                        descriptor.m_allowAlter = false;
                        descriptor.m_sbtnSkipLabel = String.Empty;
                        descriptor.m_sbtnIgnoreLabel = String.Empty;

                        CItem selectedOrdering = null;
                        ReturnCode result = mFeedback.ChooseFromSelection(descriptor, out selectedOrdering);
                        if (result == ReturnCode.OK)
                        {
                            series[DBOnlineSeries.cChosenEpisodeOrder] = (string)selectedOrdering.m_Tag;
                            // set default episode sort order to match chosen episode order
                            series[DBOnlineSeries.cEpisodeSortOrder] = (string)selectedOrdering.m_Tag == "DVD" ? "DVD" : "Aired";
                            MPTVSeriesLog.Write(string.Format("{0} order option chosen for series \"{1}\"", (string)selectedOrdering.m_Tag, series.ToString()), MPTVSeriesLog.LogLevel.Normal);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(series[DBOnlineSeries.cEpisodeOrders]))
                        {
                            series[DBOnlineSeries.cChosenEpisodeOrder] = "Aired";
                            series[DBOnlineSeries.cEpisodeSortOrder] = "Aired";
                        }
                        MPTVSeriesLog.Write(string.Format("Aired order option chosen for series \"{0}\"", series.ToString()), MPTVSeriesLog.LogLevel.Normal);
                    }
                }
            } 
            catch (Exception e) 
            {
                MPTVSeriesLog.Write("Error determining episode order for series: {0}, {1}", series.ToString(), e.Message);
            }            
        }

        #endregion

        #region EpisodeHelpers
        private static void MatchOnlineToLocalEpisodes(DBSeries series, List<DBEpisode> episodesList, GetEpisodes episodesParser)
        {
            matchOnlineToLocalEpisodes(series, episodesList, episodesParser, null);
        }

        public static void matchOnlineToLocalEpisodes(DBSeries series, List<DBEpisode> episodesList, GetEpisodes episodesParser, string orderOption)
        {
            if (episodesList == null || episodesList.Count == 0)
                return;

            foreach (DBEpisode localEpisode in episodesList) {
                bool bMatchFound = false;
                foreach (DBOnlineEpisode onlineEpisode in episodesParser.Results) {
                    if ((int)localEpisode[DBEpisode.cSeriesID] == (int)onlineEpisode[DBOnlineEpisode.cSeriesID]) {
                        if (matchOnlineToLocalEpisode(series, localEpisode, onlineEpisode, orderOption) == 0)
                        {
                            #region change local episode ids to match online ids
                            bool isSecondPart = false;
                            // check if its a double episode
                            if (!string.IsNullOrEmpty(localEpisode[DBEpisode.cCompositeID2]))
                            {
                                // check if its the second part of a double episode
                                if (localEpisode[DBEpisode.cEpisodeIndex] == localEpisode[DBEpisode.cEpisodeIndex2])
                                    isSecondPart = true;
                            }
                            // change composite id's of local episode
                            if (!string.IsNullOrEmpty(localEpisode[DBEpisode.cFilename]))
                                localEpisode.ChangeIndexes(onlineEpisode[DBOnlineEpisode.cSeasonIndex], onlineEpisode[DBOnlineEpisode.cEpisodeIndex], isSecondPart);
                            #endregion

                            // commit updated data
                            commitOnlineToLocalEpisodeMatch(series, localEpisode, onlineEpisode);
                            bMatchFound = true;
                            break;
                        }
                    }
                }
                // if a local filename failed to find a match, log parsing result
                if (!bMatchFound && !string.IsNullOrEmpty(localEpisode[DBEpisode.cFilename]))
                {
                    string filename = localEpisode[DBEpisode.cFilename];
                    string seriesName = series[DBSeries.cParsedName];
                    string seasonIdx = localEpisode[DBEpisode.cSeasonIndex];
                    string episodeIdx = localEpisode[DBEpisode.cEpisodeIndex];
                    string episodeIdx2 = localEpisode[DBEpisode.cEpisodeIndex2];
                    string episodeId = string.IsNullOrEmpty(episodeIdx2) || episodeIdx2 == "0" ? episodeIdx : string.Concat(episodeIdx, "-", episodeIdx2);

                    MPTVSeriesLog.Write("The following file could not be matched online: ");
                    MPTVSeriesLog.Write("FileName: " + localEpisode[DBEpisode.cFilename]);
                    MPTVSeriesLog.Write("Parsed Series Name: " + seriesName);
                    MPTVSeriesLog.Write("Parsed Season Index: Season " + seasonIdx);
                    MPTVSeriesLog.Write("Parsed Episode Index: Episode " + episodeId);
                    MPTVSeriesLog.Write("Confirm the entry exists in the online database: http://thetvdb.com/");
                }
            }
        }

        private static void commitOnlineToLocalEpisodeMatch(DBSeries series, DBEpisode localEpisode, DBOnlineEpisode onlineEpisode)
        {
            // season update for online data
            var season = new DBSeason(series[DBSeries.cID], onlineEpisode[DBOnlineEpisode.cSeasonIndex]);
            // TODO: TMDb has online details such as overview and name which could be added here
            season[DBSeason.cHasEpisodes] = true;
            season.Commit();

            // update data
            localEpisode[DBOnlineEpisode.cID] = onlineEpisode[DBOnlineEpisode.cID];
            if (localEpisode[DBOnlineEpisode.cEpisodeName].ToString().Length == 0)
                localEpisode[DBOnlineEpisode.cEpisodeName] = onlineEpisode[DBOnlineEpisode.cEpisodeName];

            bool traktCommunityRatings = DBOption.GetOptions(DBOption.cTraktCommunityRatings) && Helper.IsTraktAvailableAndEnabled;
            
            // go over all the fields, (and update only those which haven't been modified by the user - will do that later)
            foreach (String key in onlineEpisode.FieldNames)
            {
                switch (key)
                {
                    case DBOnlineEpisode.cCompositeID:
                    case DBEpisode.cSeriesID:
                    case DBOnlineEpisode.cWatched:
                    case DBOnlineEpisode.cFirstWatchedDate:
                    case DBOnlineEpisode.cLastWatchedDate:
                    case DBOnlineEpisode.cPlayCount:
                    case DBOnlineEpisode.cHidden:
                    case DBOnlineEpisode.cMyRating:
                    case DBOnlineEpisode.cMyRatingAt:
                        // do nothing here, those information are local only
                        break;

                    // dont update community ratings if we get from trakt
                    case DBOnlineEpisode.cRating:
                    case DBOnlineEpisode.cRatingCount:
                        if (traktCommunityRatings)
                            break;
                        goto default;

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

            localEpisode[DBOnlineEpisode.cOnlineDataImported] = 1;
            MPTVSeriesLog.Write("\"" + localEpisode.ToString() + "\" identified", MPTVSeriesLog.LogLevel.Debug);
            localEpisode.Commit();            
        }

        public static int matchOnlineToLocalEpisode(DBSeries series, DBEpisode localEpisode, DBOnlineEpisode onlineEpisode, string orderingOption)
        {
            // TODO: Enable this for any possible field in local episode (from parsing)
            // just look for a corresponding field in onlineepisode, and depending on the type do a numerical perfect check, or a fuzzy text match
            // also recognize dates
            // also, if  the orderingoptions string should not be passed in here, we should try all fields (perhaps with a certain hardcoded order) and sum up the confidences (or rather errormsg)
            // for instance we could auto detect dvd-ordered local eps, because in aired only the 1x01 will pass, but in dvd both the 1x01 and the title will pass
            // this will also enable fields such as onlineEpisodeIds to be read out from the filename (or possibly in the future nfo files) and be 100% auto matched

            // lastly, this should never overwrite the online-ep ids, instead we should overwrite the local ep ids (but not here) to always the same as the onlineep ids (aired ordering)
            // we can get different ordering on the fly inside the plugin this way

            try
            {
                orderingOption = orderingOption == null ? (string)series[DBOnlineSeries.cChosenEpisodeOrder] : orderingOption;
                switch (orderingOption)
                {
                    case "":
                    case "Aired":
                        int iEpIndex2 = (int)localEpisode[DBEpisode.cEpisodeIndex2];
                        if (iEpIndex2 == 0)
                        {
                            // Don't want to match local episodes with no EpisodeIndex2 with an online episode index of zero
                            iEpIndex2 = -1;
                        }
                        
                        // Check whether we are parsing a Air Date vs Episode/Season Index
                        if (localEpisode[DBEpisode.cEpisodeIndex] < 0 && !string.IsNullOrEmpty(localEpisode[DBOnlineEpisode.cFirstAired]))
                        {
                            string airDate = string.Empty;
                            try { airDate = DateTime.Parse(localEpisode[DBOnlineEpisode.cFirstAired]).ToString("yyyy-MM-dd"); }
                            catch { return int.MaxValue; }

                            if (airDate == onlineEpisode[DBOnlineEpisode.cFirstAired]) return 0;
                            return int.MaxValue;
                        }

                        if (localEpisode[DBEpisode.cEpisodeIndex] >= 0 && (int)localEpisode[DBEpisode.cSeasonIndex] == (int)onlineEpisode[DBOnlineEpisode.cSeasonIndex] &&
                                ((int)localEpisode[DBEpisode.cEpisodeIndex] == (int)onlineEpisode[DBOnlineEpisode.cEpisodeIndex] ||
                                iEpIndex2 == (int)onlineEpisode[DBOnlineEpisode.cEpisodeIndex]))
                        {
                            return 0;
                        }
                        else return int.MaxValue;

                    case "DVD":
                        // use cDVDEpisodeNumber and cDVDSeasonNumber for DVD matching
                        // if the episode field is empty or 0 then fallback to air date fields
                        if ( string.IsNullOrEmpty(onlineEpisode[DBOnlineEpisode.cDVDEpisodeNumber]) || onlineEpisode[DBOnlineEpisode.cDVDEpisodeNumber] == 0 )
                        {
                            return matchOnlineToLocalEpisode( series, localEpisode, onlineEpisode, "Aired" );
                        }

                        System.Globalization.NumberFormatInfo provider = new System.Globalization.NumberFormatInfo();
                        provider.NumberDecimalSeparator = ".";
                                                
                        float onlineSeasonTemp;
                        int onlineSeason = -1;
                        if (float.TryParse(onlineEpisode[DBOnlineEpisode.cDVDSeasonNumber], System.Globalization.NumberStyles.AllowDecimalPoint, provider, out onlineSeasonTemp))
                            onlineSeason = (int)onlineSeasonTemp;

                        int localSeason = 0;
                        int localEp = 0;
                        int localEp2 = 0;
                        if (!string.IsNullOrEmpty(localEpisode[DBEpisode.cOriginalComposite]))
                        {                            
                            Helper.GetEpisodeIndexesFromComposite(localEpisode[DBEpisode.cOriginalComposite2], out localSeason, out localEp2);
                            Helper.GetEpisodeIndexesFromComposite(localEpisode[DBEpisode.cOriginalComposite], out localSeason, out localEp);
                        }
                        else
                        {
                            localSeason = (int)localEpisode[DBEpisode.cSeasonIndex];
                            localEp = (int)localEpisode[DBEpisode.cEpisodeIndex];
                            localEp2 = (int)localEpisode[DBEpisode.cEpisodeIndex2];
                        }

                        float onlineEp = -1;
                        if (onlineSeason != -1 && float.TryParse(onlineEpisode[DBOnlineEpisode.cDVDEpisodeNumber], System.Globalization.NumberStyles.AllowDecimalPoint, provider, out onlineEp))
                        {
                            string localstring;
                            double localcomp;
                            localstring = (localEp.ToString() + "." + localEp2.ToString());
                            localcomp = Convert.ToDouble(localstring, provider);
                            if (localSeason == onlineSeason && 
                               (localcomp == onlineEp || localEp == (int)onlineEp))
                            {
                                return 0;
                            }
                            else
                            {
                                return int.MaxValue;
                            }
                        }
                        break;

                    case "Absolute":
                        System.Globalization.NumberFormatInfo provided = new System.Globalization.NumberFormatInfo();
                        float onlineabs = -1;
                        float onlineabsTemp;
                        if (float.TryParse(onlineEpisode[DBOnlineEpisode.cAbsoluteNumber], System.Globalization.NumberStyles.AllowDecimalPoint, provided, out onlineabsTemp))
                            onlineabs = onlineabsTemp;

                        if (onlineabs != -1)
                        {
                            double localabs = -1;
                            if ((int)localEpisode[DBEpisode.cSeasonIndex] == 0)
                            {
                                /*Now we have to figure out whether we are at ep 100 or more*/
                                localabs = Convert.ToDouble(localEpisode[DBEpisode.cSeasonIndex].ToString() + localEpisode[DBEpisode.cEpisodeIndex].ToString());
                            }
                            else if ((int)localEpisode[DBEpisode.cSeasonIndex] >= 1 && (int)localEpisode[DBEpisode.cEpisodeIndex] < 10)
                            {
                                /* Any episode X0[0-9] should be combined in this manner */
                                localabs = Convert.ToDouble(localEpisode[DBEpisode.cSeasonIndex].ToString() + "0" + localEpisode[DBEpisode.cEpisodeIndex].ToString());
                            }
                            else if ((int)localEpisode[DBEpisode.cSeasonIndex] >= 1 && (int)localEpisode[DBEpisode.cEpisodeIndex] >= 10)
                            {
                                /* All other episodes should fall into this category */
                                localabs = Convert.ToDouble(localEpisode[DBEpisode.cSeasonIndex].ToString() + localEpisode[DBEpisode.cEpisodeIndex].ToString());
                            }

                            float.TryParse(onlineEpisode[DBOnlineEpisode.cAbsoluteNumber], System.Globalization.NumberStyles.AllowDecimalPoint, provided, out onlineabs);
                            if (localabs == onlineabs)
                            {
                                return 0;
                            }
                            else
                            {
                                return int.MaxValue;
                            }
                        }
                        break;
                    case "Title":
                        int fuzzyness = 3;
                        string localTitle = localEpisode[DBEpisode.cRawEpisodeName];
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
            MPTVSeriesLog.Write(BigLogMessage("Gathering Local Information"));
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

            // Signal external event listeners that new local files have been added
            mNewLocalFiles = parsedFiles.Count > 0;

            int nSeason = 0;
            List<DBSeries> relatedSeries = new List<DBSeries>();
            List<DBSeason> relatedSeasons = new List<DBSeason>();

            int nIndex = 0;
            foreach (parseResult progress in parsedFiles)
            {
                if (Worker.CancellationPending)
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
                        DataUpdated = true;
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

                    // we may need to update the composite id later when matching to online episode                    
                    if (bNewFile)
                        episode[DBEpisode.cCompositeUpdated] = 0;

                    episode.Commit();

                }

                if(++nIndex % 25 == 0)
                    Worker.ReportProgress(0, new ParsingProgress(ParsingAction.LocalScan, progress.match_filename, nIndex, parsedFiles.Count));
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
            var episodes = DBEpisode.Get(new SQLCondition(new DBEpisode(), DBOnlineEpisode.cWatched, false, SQLConditionType.Equal));

            foreach (DBSeason season in relatedSeasons)
            {
                var HasUnwatched = episodes.Any(e => e[DBSeason.cSeriesID] == season[DBSeason.cSeriesID] && e[DBEpisode.cSeasonIndex] == season[DBSeason.cIndex]);
                if (season[DBSeason.cUnwatchedItems] != HasUnwatched || !season[DBSeason.cHidden] != false)
                {
                    season[DBSeason.cUnwatchedItems] = HasUnwatched;
                    season[DBSeason.cHidden] = 0;
                    season.Commit();
                } // else nothing changed, so dont bother
            }
            
            foreach (DBSeries series in relatedSeries)
            {
                var HasUnwatched = episodes.Any(e => e[DBSeries.cID] == series[DBSeries.cID]);
                if (series[DBSeason.cUnwatchedItems] != HasUnwatched || !series[DBSeries.cHidden] != false)
                {
                    series[DBOnlineSeries.cUnwatchedItems] = HasUnwatched;
                    series[DBSeries.cHidden] = 0;
                    series.Commit();
                } // else nothing changed, so dont bother
            }
            
            Worker.ReportProgress(0, new ParsingProgress(ParsingAction.LocalScan, parsedFiles.Count));
        }
        #endregion

        #region HelperFunctions
        static List<string> GenerateIDListOfString<T>(List<T> aEntities, string aFieldName) where T : DBTable
        {
            // generate a comma separated list of all the ids
            var lSeriesIds = new List<string>(aEntities.Count);
            if (aEntities.Count > 0)
            {
                foreach (DBTable entity in aEntities)
                {
                    lSeriesIds.Add(entity[aFieldName].ToString());
                }
            }
            return lSeriesIds;
        }
        #endregion

        #region MessageFormatters
        static string BigLogMessage(string msg)
        {
            return string.Format("***************     {0}     ***************", msg);
        }

        static string PrettyStars(int length)
        {
            StringBuilder b = new StringBuilder(length);
            for (int i = 0; i < length; i++) b.Append('*');
            return b.ToString();
        }
        #endregion
    }
}
