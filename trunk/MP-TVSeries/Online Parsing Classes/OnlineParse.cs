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

namespace WindowPlugins.GUITVSeries
{

    enum ParsingAction
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
        UpdateUserFavourites
    }

    class CParsingParameters
    {
        private static List<ParsingAction> FirstLocalScanActions = new List<ParsingAction> { ParsingAction.LocalScan, ParsingAction.MediaInfo,
            ParsingAction.IdentifyNewSeries, ParsingAction.IdentifyNewEpisodes };
        private static List<ParsingAction> UpdateActions = new List<ParsingAction> { ParsingAction.GetOnlineUpdates, ParsingAction.UpdateSeries, 
            ParsingAction.UpdateEpisodes, ParsingAction.UpdateEpisodeCounts, ParsingAction.UpdateUserRatings, ParsingAction.UpdateBanners, ParsingAction.UpdateFanart};
        private static List<ParsingAction> LastLocalScanActions = new List<ParsingAction> { ParsingAction.GetNewBanners, ParsingAction.GetNewFanArt, ParsingAction.UpdateEpisodeThumbNails, 
            ParsingAction.UpdateUserFavourites };

        public List<ParsingAction> m_actions = new List<ParsingAction>();

        public bool m_bLocalScan = true;
        public bool m_bUpdateScan = true;
        public List<PathPair> m_files = null;

        public List<DBValue> m_series = null;
        public List<DBValue> m_episodes = null;

        public CParsingParameters(bool bScanNew, bool bUpdateExisting)
        {
            m_bLocalScan = bScanNew;
            m_bUpdateScan = bUpdateExisting;

            if (m_bLocalScan) 
                m_actions.AddRange(FirstLocalScanActions);
            if (m_bUpdateScan)
                m_actions.AddRange(UpdateActions);
            if (m_bLocalScan)
                m_actions.AddRange(LastLocalScanActions);
        }

        public CParsingParameters(ParsingAction action, List<PathPair> files, bool bScanNew, bool bUpdateExisting)
        {
            m_actions.Add(action);
            m_files = files;

            if (m_bLocalScan)
                m_actions.AddRange(FirstLocalScanActions);
            if (m_bUpdateScan)
                m_actions.AddRange(UpdateActions);
            if (m_bLocalScan)
                m_actions.AddRange(LastLocalScanActions);
        }

        public CParsingParameters(IEnumerable<ParsingAction> actions, List<DBValue> series, List<DBValue> episodes)
        {
            m_actions.AddRange(actions);
            m_episodes = episodes;
            m_series = series;
        }
    };

    class OnlineParsing
    {
        public BackgroundWorker m_worker = new BackgroundWorker();
        IFeedback m_feedback = null;

        bool m_bDataUpdated = false;
        bool m_bFullSeriesRetrieval = false;
        bool m_bNoExactMatch = false;       //if set to true then the user will be always prompted to choose the series
        CParsingParameters m_params = null;

        int RETRY_INTERVAL = 1000;
        int RETRY_MULTIPLIER = 2;
        int MAX_TIMEOUT = 120000;

        public delegate void OnlineParsingProgressHandler(int nProgress);
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
            get
            {
                return m_worker.IsBusy;
            }
        }

        public bool Start(CParsingParameters param)
        {
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

        public void worker_DoWork(object sender, DoWorkEventArgs e)
        {            
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            
            m_params = e.Argument as CParsingParameters;
            m_bFullSeriesRetrieval = DBOption.GetOptions(DBOption.cFullSeriesRetrieval);
            m_bNoExactMatch = false;
            m_worker.ReportProgress(0);

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
            TVSeriesPlugin.IsResumeFromStandby = false;

            BackgroundWorker tMediaInfo = null;
            BackgroundWorker tEpisodeCounts = null;
            BackgroundWorker tUserRatings = null;

            bool online = DBOption.GetOptions(DBOption.cOnlineParseEnabled) == 1 && DBOnlineMirror.IsMirrorsAvailable;
            Online_Parsing_Classes.GetUpdates updates = null;
            foreach (ParsingAction action in m_params.m_actions) {
                
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
                        ParseActionLocalScan(m_params.m_bLocalScan, m_params.m_bUpdateScan);
                        break;

                    case ParsingAction.MediaInfo:
                        //Threaded MediaInfo.dll parsing of new files - goes straight to next task
                        tMediaInfo = new BackgroundWorker();
                        MediaInfoParse(tMediaInfo);
                        break;

                    case ParsingAction.IdentifyNewSeries:
                        if (online) {
                            GetSeries(m_worker, m_bNoExactMatch);
                            UpdateSeries(true, null); // todo: ask orderoption
                        }
                        break;

                    case ParsingAction.IdentifyNewEpisodes:
                        if (online)
                            GetEpisodes(m_params.m_bUpdateScan, m_bFullSeriesRetrieval);
                        break;

                    case ParsingAction.GetOnlineUpdates:
                        if (online)
                            updates = GetOnlineUpdates();
                        break;

                    case ParsingAction.UpdateSeries:
                        if (online) {
                            if (updates != null)
                                UpdateSeries(false, updates.UpdatedSeries);
                            if (m_params.m_series != null)
                                UpdateSeries(false, m_params.m_series);
                        }
                        break;

                    case ParsingAction.UpdateEpisodes:
                        if (online) {
                            if (updates != null)
                                UpdateEpisodes(updates.UpdatedEpisodes);
                            if (m_params.m_episodes != null)
                                UpdateEpisodes(m_params.m_episodes);
                        }
                        break;

                    case ParsingAction.UpdateBanners:
                        if (online && updates != null)
                            UpdateBanners(false, updates.UpdatedBanners);
                        break;

                    case ParsingAction.UpdateFanart:
                        if (online && updates != null && !DBOption.GetOptions(DBOption.cAutoUpdateAllFanart))
                            UpdateFanart(false, updates.UpdatedFanart);
                        break;

                    case ParsingAction.GetNewBanners:
                        if (online)
                            UpdateBanners(true, null);// update new series for banners
                        break;

                    case ParsingAction.GetNewFanArt:
                        if (online)
                            UpdateFanart(true, null);// updates ALL series for fanart - todo: only scan for series with missing fanart since we got new ones from thetvdb.com above...
                                                     //                                       mirror how banners does it.
                        break;

                    case ParsingAction.UpdateEpisodeThumbNails:
                        if (online)
                            UpdateEpisodeThumbNails();
                        break;

                    case ParsingAction.UpdateUserRatings:
                        if (online)
                            tUserRatings = new BackgroundWorker();
                            UpdateUserRatings(tUserRatings);
                        break;

                    case ParsingAction.UpdateUserFavourites:
                        if (online)
                            UpdateUserFavourites();
                        break;

                    case ParsingAction.UpdateEpisodeCounts:
                        //Threaded processing of episode counts - goes straight to next task
                        tEpisodeCounts = new BackgroundWorker();
                        UpdateEpisodeCounts(tEpisodeCounts);
                        break;
                }
            }
            
            //WaitForCompletion()
            //SLEEP UNTIL MEDIAINFO OR ANY OTHER THREADS ARE DONE - avoids user thinking scan is completed
            //if (tMediaInfo == null && tEpisodeCounts == null && tUserRatings == null) continue;
            if (tMediaInfo.IsBusy) MPTVSeriesLog.Write("*******************   MediaInfo Scan Still Going   ************************");
            if (tEpisodeCounts.IsBusy) MPTVSeriesLog.Write("*******************    Episode Count Still Going   ************************");
            if (tUserRatings.IsBusy) MPTVSeriesLog.Write("*******************    User Ratings Still Going    ************************");
            do
            {
                Thread.Sleep(1000);
            }
            while (tMediaInfo.IsBusy || tEpisodeCounts.IsBusy || tUserRatings.IsBusy);

            
            // lets save the updateTimestamp
            if (updates != null && updates.OnlineTimeStamp > 0)
                DBOption.SetOptions(DBOption.cUpdateTimeStamp, updates.OnlineTimeStamp);

            
            Online_Parsing_Classes.OnlineAPI.ClearBuffer();
            // and we are done, the backgroundworker is going to notify so
            MPTVSeriesLog.Write("***************************************************************************");
            MPTVSeriesLog.Write("*******************            Completed           ************************");
            MPTVSeriesLog.Write("***************************************************************************");
        }

        #region ParseActions
        private void ParseActionRemove(List<PathPair> files)
        {
            string initialMsg = "*******************        Remove Run Starting      ***********************";
            MPTVSeriesLog.Write(prettyStars(initialMsg.Length));
            MPTVSeriesLog.Write(initialMsg);
            MPTVSeriesLog.Write(prettyStars(initialMsg.Length));
            // should we remove deleted files?
            if (!DBOption.GetOptions(DBOption.cDontClearMissingLocalFiles)) {
                List<DBOnlineSeries> relatedSeries = new List<DBOnlineSeries>();
                List<DBSeason> relatedSeasons = new List<DBSeason>();

                foreach (PathPair pair in files) {
                    if (!LocalParse.isOnRemovable(pair.m_sFull_FileName)) {
                        DBEpisode episode = new DBEpisode(pair.m_sFull_FileName, true);

                        // already in?
                        bool bSeasonFound = false;
                        foreach (DBSeason season in relatedSeasons)
                            if (season[DBSeason.cSeriesID] == episode[DBEpisode.cSeriesID] && season[DBSeason.cIndex] == episode[DBEpisode.cSeasonIndex]) {
                                bSeasonFound = true;
                                break;
                            }
                        if (!bSeasonFound)
                            relatedSeasons.Add(new DBSeason(episode[DBEpisode.cSeriesID], episode[DBEpisode.cSeasonIndex]));

                        bool bSeriesFound = false;
                        foreach (DBOnlineSeries series in relatedSeries)
                            if (series[DBOnlineSeries.cID] == episode[DBEpisode.cSeriesID]) {
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
                foreach (DBSeason season in relatedSeasons) {
                    if (DBEpisode.Get(season[DBSeason.cSeriesID], season[DBSeason.cIndex], false).Count > 0) {
                        season[DBSeason.cHasLocalFilesTemp] = true;
                        season[DBSeason.cHasEpisodes] = true;
                    } else
                        season[DBSeason.cHasLocalFilesTemp] = false;

                    DBEpisode episode = DBEpisode.GetFirstUnwatched(season[DBSeason.cSeriesID], season[DBSeason.cIndex]);
                    if (episode != null)
                        season[DBSeason.cUnwatchedItems] = true;
                    else
                        season[DBSeason.cUnwatchedItems] = false;

                    season.Commit();
                }

                foreach (DBOnlineSeries series in relatedSeries) {
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
            }
        }

        private void ParseActionAdd(List<PathPair> files)
        {
            string initialMsg = "*******************       Add Run Starting     ***************************";
            MPTVSeriesLog.Write(prettyStars(initialMsg.Length));
            MPTVSeriesLog.Write(initialMsg);
            MPTVSeriesLog.Write(prettyStars(initialMsg.Length));
            ParseLocal(files);
        }

        private void ParseActionLocalScan(bool localScan, bool updateScan)
        {
            string initialMsg = String.Format("******************* Full Run Starting (LocalScan: {0} -  UpdateScan: {1})   ***************************", localScan, updateScan);
            if (m_bNoExactMatch) {
                initialMsg = String.Format("******************* NoExactMatch Run Starting (LocalScan: {0} -  UpdateScan: {1})   ***************************", localScan, updateScan);
            }
            MPTVSeriesLog.Write(prettyStars(initialMsg.Length));
            MPTVSeriesLog.Write(initialMsg);
            MPTVSeriesLog.Write(prettyStars(initialMsg.Length));
            if (localScan) {
                // mark all files in the db as not processed (to figure out which ones we'll have to remove after the import)
                DBEpisode.GlobalSet(DBEpisode.cImportProcessed, 2);
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

                ParseLocal(Filelister.GetFiles(listFolders));

                // now, remove all episodes still processed = 0, the weren't find in the scan
                if (!DBOption.GetOptions(DBOption.cDontClearMissingLocalFiles)) {
                    SQLCondition condition = new SQLCondition();
                    condition.Add(new DBEpisode(), DBEpisode.cImportProcessed, 2, SQLConditionType.Equal);
                    condition.Add(new DBEpisode(), DBEpisode.cIsOnRemovable, false, SQLConditionType.Equal);
                    DBEpisode.Clear(condition);
                }
                // and copy the HasLocalFileTemp value into the real one
                DBSeries.GlobalSet(DBOnlineSeries.cHasLocalFiles, DBOnlineSeries.cHasLocalFilesTemp);
                DBSeason.GlobalSet(DBSeason.cHasLocalFiles, DBSeason.cHasLocalFilesTemp);
            }
        }

        private void GetSeries(BackgroundWorker worker, bool bNoExactMatch)
        {
            MPTVSeriesLog.Write(bigLogMessage("Identifying Unknown Series Online"));

            SQLCondition condition = null;

            condition = new SQLCondition();
            // all series that don't have an onlineID ( < 0) and not marked as ignored
            condition.Add(new DBSeries(), DBSeries.cID, 0, SQLConditionType.LessThan);
            condition.Add(new DBSeries(), DBSeries.cScanIgnore, 0, SQLConditionType.Equal);

            int nIndex = 0;
            List<DBSeries> seriesList = DBSeries.Get(condition, false, false);
            if (seriesList.Count > 0)
                MPTVSeriesLog.Write(string.Format("Found {0} unknown Series, attempting to identify them now", seriesList.Count));
            else
                MPTVSeriesLog.Write("All Series are already identified");

            foreach (DBSeries series in seriesList) {
                if (worker.CancellationPending)
                    return;

                worker.ReportProgress(10 + (10 * nIndex / seriesList.Count));
                nIndex++;

                String sSeriesNameToSearch = series[DBSeries.cParsedName];
                DBOnlineSeries UserChosenSeries = SearchForSeries(sSeriesNameToSearch, bNoExactMatch, m_feedback);

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

            MPTVSeriesLog.Write(string.Format("{0} metadata of {1} Series", (bUpdateNewSeries ? "Retrieving" : "Looking for updated"), SeriesList.Count));

            UpdateSeries UpdateSeriesParser = new UpdateSeries(generateIDListOfString(SeriesList, DBSeries.cID));

            if (UpdateSeriesParser.Results.Count == 0)
                MPTVSeriesLog.Write(string.Format("No {0} found", (bUpdateNewSeries ? "metadata" : "updates")));

            foreach (DBOnlineSeries updatedSeries in UpdateSeriesParser.Results) {
                m_bDataUpdated = true;
                if (m_worker.CancellationPending)
                    return;

                MPTVSeriesLog.Write(string.Format("Metadata {0} for \"{1}\"", (bUpdateNewSeries ? "retrieved" : "updated"), updatedSeries.ToString()));
                // find the corresponding series in our list
                foreach (DBSeries localSeries in SeriesList) {
                    if (localSeries[DBSeries.cID] == updatedSeries[DBSeries.cID]) {
                        // go over all the fields, (and update only those which haven't been modified by the user - will do that later)
                        foreach (String key in updatedSeries.FieldNames) {
                            switch (key) {
                                // do not overwrite current series local settings with the one from the online series (baaaad design??)
                                case DBSeries.cParsedName: // this field shouldn't be required here since updatedSeries is an Onlineseries and not a localseries??
                                case DBOnlineSeries.cHasLocalFiles:
                                case DBOnlineSeries.cHasLocalFilesTemp:
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
                                    localSeries.AddColumn(key, new DBField(DBField.cTypeString));
                                    localSeries[key] = updatedSeries[key];
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
            foreach (DBSeries series in seriesList) {
                List<DBEpisode> episodesList = null;

                // lets get the list of unidentified series
                SQLCondition conditions = new SQLCondition();
                conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cID, 0, SQLConditionType.Equal);
                episodesList = DBEpisode.Get(conditions, false);

                epCount += episodesList.Count;

                if (bFullSeriesRetrieval || episodesList.Count > 0) {
                    GetEpisodes episodesParser = new GetEpisodes((string)series[DBSeries.cID]);
                    if (episodesParser.Results.Count > 0) {
                        MPTVSeriesLog.Write(string.Format("Found {0} episodes online for \"{1}\", matching them up with local episodes now", episodesParser.Results.Count.ToString().PadLeft(3, '0'), series.ToString()));
                        // look for the episodes for that series, and compare / update the values
                        matchOnlineToLocalEpisodes(series, episodesList, episodesParser);
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
                                            newOnlineEpisode.AddColumn(key, new DBField(DBField.cTypeString));
                                            newOnlineEpisode[key] = onlineEpisode[key];
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
                MPTVSeriesLog.Write("Nothing to do");
                return;
            }

            // let's updated those we are interested in
            // for the remaining ones get the <lang>.xml
            MPTVSeriesLog.Write(episodesInDB.Count.ToString() + " episodes need updating");
            int seriesID = 0;
            for (int i = 0; i < episodesInDB.Count; i++) {
                if (seriesID != episodesInDB[i][DBEpisode.cSeriesID]) {
                    seriesID = episodesInDB[i][DBEpisode.cSeriesID];

                    // Filter out episodes and parse only the ones in the current series
                    List<DBEpisode> eps = new List<DBEpisode>(episodesInDB.Count);
                    for (int j = 0; j < episodesInDB.Count; j++)
                        if (episodesInDB[j][DBEpisode.cSeriesID] == seriesID)
                            eps.Add(episodesInDB[j]);

                    matchOnlineToLocalEpisodes(Helper.getCorrespondingSeries(seriesID), eps, new GetEpisodes(seriesID.ToString()));
                }
            }
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
            } else
                MPTVSeriesLog.Write("Looking for artwork on " + seriesList.Count + " Series");

            foreach (DBSeries series in seriesList) {
                if (m_worker.CancellationPending)
                    return;
                nIndex++;
                MPTVSeriesLog.Write((bUpdateNewSeries ? "Downloading" : "Refreshing") + " artwork for \"" + series.ToString() + "\"");

                GetBanner bannerParser = new GetBanner((string)series[DBSeries.cID]);

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
                            MPTVSeriesLog.Write("New series banner found for \"" + series.ToString() + "\" : " + bannerSeries.sOnlineBannerPath);
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
                            MPTVSeriesLog.Write("New series poster found for \"" + series.ToString() + "\" : " + posterSeries.sOnlinePosterPath);
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
                                MPTVSeriesLog.Write("New season banner found for \"" + series.ToString() + "\" Season " + season[DBSeason.cIndex] + ": " + bannerSeason.sOnlineBannerPath);
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

            foreach (DBSeries series in seriesList)
            {
                MPTVSeriesLog.Write("Retrieving Fanart for: " + Helper.getCorrespondingSeries(series[DBSeries.cID]));

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

                        MPTVSeriesLog.Write(string.Format("New Fanart found for \"{0}\": {1}", Helper.getCorrespondingSeries(series[DBSeries.cID]), onlineFilename));
                        bool result = Online_Parsing_Classes.OnlineAPI.DownloadBanner(onlineFilename, Settings.Path.fanart, localFilename);

                        if (result) {
                            // Update Fanart DB
                            toDownload[DBFanart.cLocalPath] = localFilename;
                            toDownload.Commit();
                        }
                    }
                } catch (Exception ex) {
                    MPTVSeriesLog.Write("Failed to update Fanart: " + ex.Message);
                }
            }
        }




        public void UpdateUserRatings(BackgroundWorker tUserRatings)
        {
            MPTVSeriesLog.Write(bigLogMessage("Get User Ratings"));
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

            List<DBOnlineSeries> seriesList = (List<DBOnlineSeries>)e.Argument;

            if (!Helper.String.IsNullOrEmpty(sAccountID))
            {
                if (DBOption.GetOptions(DBOption.cAutoUpdateEpisodeRatings)) // i.e. update Series AND Underlying Episodes
                {
                    foreach (DBOnlineSeries series in seriesList)
                    {
                        MPTVSeriesLog.Write("Retrieving user ratings for series: " + Helper.getCorrespondingSeries((int)series[DBOnlineSeries.cID]));
                        GetUserRatings userRatings = new GetUserRatings(series[DBOnlineSeries.cID], sAccountID);

                        // Set Series Rating
                        series[DBOnlineSeries.cMyRating] = userRatings.SeriesRating;
                        series.Commit();

                        SQLCondition condition = new SQLCondition();
                        condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series[DBOnlineSeries.cID], SQLConditionType.Equal);
                        List<DBEpisode> episodes = DBEpisode.Get(condition);

                        foreach (DBEpisode episode in episodes)
                        {
                            if (userRatings.EpisodeRating.ContainsKey(episode[DBOnlineEpisode.cID]))
                                episode[DBOnlineEpisode.cMyRating] = userRatings.EpisodeRating[episode[DBOnlineEpisode.cID]];
                            episode.Commit();
                        }
                    }
                }
                else //update Series only, not Episodes -- workaround for not being able to pull up all series/episode ratings at once from theTVDB.com; saves time.
                {
                    GetUserRatings userRatings = new GetUserRatings(null, sAccountID);
                    foreach (DBOnlineSeries series in seriesList)
                    {
                        if (userRatings.AllSeriesRatings.ContainsKey(series[DBOnlineSeries.cID]))
                        {
                            MPTVSeriesLog.Write("User ratings retrieved for series: " + Helper.getCorrespondingSeries((int)series[DBOnlineSeries.cID]));
                            series[DBOnlineSeries.cMyRating] = userRatings.AllSeriesRatings[series[DBOnlineSeries.cID]];
                            series.Commit();
                        }
                    }
                }
            }
            
        }

        public void UpdateUserFavourites()
        {
            string sAccountID = DBOption.GetOptions(DBOption.cOnlineUserID);

            if (!Helper.String.IsNullOrEmpty(sAccountID)) {
                MPTVSeriesLog.Write(bigLogMessage("Get User Favourites"));

                GetUserFavourites userFavourites = new GetUserFavourites(sAccountID);

                SQLCondition conditions = new SQLCondition();
                List<DBSeries> seriesList = DBSeries.Get(conditions);
                
                foreach (DBSeries series in seriesList) {
                    if (userFavourites.Series.Contains(series[DBOnlineSeries.cID])) {
                        MPTVSeriesLog.Write("Retrieved favourite series: " + Helper.getCorrespondingSeries((int)series[DBOnlineSeries.cID]));
                        series[DBOnlineSeries.cViewTags] = Helper.GetSeriesViewTags(series, true, DBView.cTranslateTokenOnlineFavourite);
                        series.Commit();
                    } else {
                        series[DBOnlineSeries.cViewTags] = Helper.GetSeriesViewTags(series, false, DBView.cTranslateTokenOnlineFavourite);
                        series.Commit();
                    }
                }
            }
        }

        public void UpdateEpisodeThumbNails()
        {
            if (DBOption.GetOptions(DBOption.cGetEpisodeSnapshots) == true) {
                MPTVSeriesLog.Write(bigLogMessage("Checking for Episode Thumbnails"));

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

                            if (Helper.String.IsNullOrEmpty(order) || order == "Aired")
                                orderIdentifier = ".jpg";
                            else
                                orderIdentifier = "_" + tmpSeries[DBOnlineSeries.cChoseEpisodeOrder] + ".jpg";

                            string seriesFolder = Helper.cleanLocalPath(tmpSeries.ToString());
                            
                            sThumbNailFilename = Helper.PathCombine(seriesFolder, @"Episodes\" + episode[DBOnlineEpisode.cSeasonIndex] + "x" + episode[DBOnlineEpisode.cEpisodeIndex] + orderIdentifier);
                            completePath = Helper.PathCombine(basePath, sThumbNailFilename);

                            if (!File.Exists(completePath)) {
                                MPTVSeriesLog.Write(string.Format("New Episode Image found for \"{0}\": {1}", episode.ToString(), episode[DBOnlineEpisode.cEpisodeThumbnailUrl]));
                                System.Net.WebClient webClient = new System.Net.WebClient();
                                webClient.Headers.Add("user-agent", Settings.UserAgent);
                                string url = DBOnlineMirror.Banners + episode[DBOnlineEpisode.cEpisodeThumbnailUrl];
                                try {
                                    Directory.CreateDirectory(Path.GetDirectoryName(completePath));
                                    // Determine if a thumbnail
                                    if (!url.Contains(".jpg")) {
                                        MPTVSeriesLog.Write("Episode Thumbnail location is incorrect: " + url, MPTVSeriesLog.LogLevel.Normal);
                                        episode[DBOnlineEpisode.cEpisodeThumbnailUrl] = "";
                                        episode[DBOnlineEpisode.cEpisodeThumbnailFilename] = "";
                                    } else {
                                        webClient.DownloadFile(url, completePath);
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
            }
        }

        public void UpdateEpisodeCounts(BackgroundWorker tEpisodeCounts)
        {
            // Update Episode counts
            MPTVSeriesLog.Write("*****************  Updating Episode Counts in Database  *******************");
            MPTVSeriesLog.Write("Episode counts running in the background (threaded)!");
            SQLCondition condEmpty = new SQLCondition();
            List<DBSeries> AllSeries = DBSeries.Get(condEmpty);

            //BackgroundWorker resReader = new BackgroundWorker();
            tEpisodeCounts.DoWork += new DoWorkEventHandler(asyncEpisodeCounts);
            tEpisodeCounts.RunWorkerCompleted += new RunWorkerCompletedEventHandler(asyncEpisodeCountsCompleted);
            tEpisodeCounts.RunWorkerAsync(AllSeries);
        }

        void asyncEpisodeCountsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MPTVSeriesLog.Write("*****************  Episode Counts in Database Updated   *******************");
        }

        void asyncEpisodeCounts(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;
            List<DBSeries> AllSeries = (List<DBSeries>)e.Argument;
            foreach (DBSeries series in AllSeries)
                DBSeries.UpdatedEpisodeCounts(series);
            //e.Result = series.Count;
        }

        public void MediaInfoParse(BackgroundWorker tMediaInfo)
        {
            SQLCondition cond = new SQLCondition();
            cond.Add(new DBEpisode(), DBEpisode.cFilename, "", SQLConditionType.NotEqual);
            cond.Add(new DBEpisode(), DBEpisode.cVideoWidth, "0", SQLConditionType.Equal);            
            // Playtime decrements by one every failed attempt, dont attempt future scans if done more than Maximum attempts
            cond.Add(new DBEpisode(), "localPlaytime", (DBEpisode.maxMIAttempts*-1), SQLConditionType.GreaterThan); 
            List<DBEpisode> episodes = new List<DBEpisode>();
            // get all the episodes
            episodes = DBEpisode.Get(cond, false);
            
            if (episodes.Count > 0)
            {
                MPTVSeriesLog.Write("Updating MediaInfo...Running in the background (threaded)!  Only errors will be shown.");
                MPTVSeriesLog.Write("Note: MediaInfo processing may not be completed until after Parsing is finished.  "
                                    + "Please wait until MediaInfo completes before you close the config.");
                
                tMediaInfo.DoWork += new DoWorkEventHandler(asyncReadResolutions);
                tMediaInfo.RunWorkerCompleted += new RunWorkerCompletedEventHandler(asyncReadResolutionsCompleted);
                tMediaInfo.RunWorkerAsync(episodes);

            }
            else 
                MPTVSeriesLog.Write("No Episodes found that need updating");
        }

        void asyncReadResolutionsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MPTVSeriesLog.Write("Update of MediaInfo complete (processed " + e.Result.ToString() + " files)");
        }

        void asyncReadResolutions(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;
            List<DBEpisode> episodes = (List<DBEpisode>)e.Argument;
            foreach (DBEpisode ep in episodes)
                ep.readMediaInfoOfLocal();
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
                if (DBOption.GetOptions(DBOption.cAutoChooseSeries) == 1) {
                    foreach (DBOnlineSeries onlineSeries in GetSeriesParser.Results) {
                        if (!bNoExactMatch && !Helper.String.IsNullOrEmpty(onlineSeries[DBOnlineSeries.cPrettyName]) &&
                           (onlineSeries[DBOnlineSeries.cPrettyName].ToString().Trim().Equals(nameToSearch.Trim().ToLower(), StringComparison.InvariantCultureIgnoreCase))) {
                            MPTVSeriesLog.Write(string.Format("\"{0}\" was automatically matched to \"{1}\" (SeriesID: {2}), there were a total of {3} matches returned from the Online Database", nameToSearch, onlineSeries.ToString(), onlineSeries[DBOnlineSeries.cID], GetSeriesParser.Results.Count));
                            return onlineSeries;
                        }
                    }
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
                List<string> episodeOrders = new List<string>(series[DBOnlineSeries.cEpisodeOrders].ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
                if (episodeOrders.Count > 1 && (DBOption.GetOptions(DBOption.cAutoChooseOrder) == 0)) {
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
                    if (result == ReturnCode.OK) {
                        series[DBOnlineSeries.cChoseEpisodeOrder] = (string)selectedOrdering.m_Tag;
                        MPTVSeriesLog.Write(string.Format("{0} order option chosen for series \"{1}\"", (string)selectedOrdering.m_Tag, series.ToString()), MPTVSeriesLog.LogLevel.Normal);
                    }
                } else {
                    if (series[DBOnlineSeries.cEpisodeOrders] != "")
                        series[DBOnlineSeries.cChoseEpisodeOrder] = "Aired";
                    MPTVSeriesLog.Write(string.Format("Aired order option chosen for series \"{0}\"", series.ToString()), MPTVSeriesLog.LogLevel.Normal);
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
                        if (matchOnlineToLocalEpisode(series, localEpisode, onlineEpisode)) {
                            // season update for online data
                            DBSeason season = new DBSeason(series[DBSeries.cID], onlineEpisode[DBOnlineEpisode.cSeasonIndex]);
                            season[DBSeason.cHasEpisodes] = true;
                            season.Commit();

                            // update data
                            localEpisode[DBOnlineEpisode.cID] = onlineEpisode[DBOnlineEpisode.cID];
                            if (localEpisode[DBOnlineEpisode.cEpisodeName].ToString().Length == 0)
                                localEpisode[DBOnlineEpisode.cEpisodeName] = onlineEpisode[DBOnlineEpisode.cEpisodeName];

                            // go over all the fields, (and update only those which haven't been modified by the user - will do that later)
                            foreach (String key in onlineEpisode.FieldNames) {
                                switch (key) {
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

                                    default:
                                        localEpisode.onlineEpisode.AddColumn(key, new DBField(DBField.cTypeString));
                                        localEpisode[key] = onlineEpisode[key];
                                        break;
                                }
                            }
                            localEpisode[DBOnlineEpisode.cOnlineDataImported] = 1;
                            MPTVSeriesLog.Write("\"" + localEpisode.ToString() + "\" identified");
                            localEpisode.Commit();
                            bMatchFound = true;
                            break;
                        }
                    }
                }
                if (!bMatchFound)
                    MPTVSeriesLog.Write(localEpisode[DBEpisode.cFilename].ToString() + " could not be matched online, check that the online database has this episode.");
            }
        }

        private static bool matchOnlineToLocalEpisode(DBSeries series, DBEpisode localEpisode, DBOnlineEpisode onlineEpisode)
        {
            switch ((string)series[DBOnlineSeries.cChoseEpisodeOrder]) {
                case "":
                case "Aired":
                    int iEpIndex2 = 0;
                    if ((int)localEpisode[DBEpisode.cEpisodeIndex2] == 0) {
                        // Don't want to match local episodes with no EpisodeIndex2 with an online episode index of zero
                        iEpIndex2 = -1;
                    }

                    return ((int)localEpisode[DBEpisode.cSeasonIndex] == (int)onlineEpisode[DBOnlineEpisode.cSeasonIndex] &&
                            ((int)localEpisode[DBEpisode.cEpisodeIndex] == (int)onlineEpisode[DBOnlineEpisode.cEpisodeIndex] ||
                            iEpIndex2 == (int)onlineEpisode[DBOnlineEpisode.cEpisodeIndex]));

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

                    if (Helper.String.IsNullOrEmpty(localEpisode[DBEpisode.cEpisodeIndex2]))
                        localEp2 = 0;
                    float onlineEp = -1;


                    if (onlineSeason != -1 && float.TryParse(onlineEpisode["DVD_episodenumber"], System.Globalization.NumberStyles.AllowDecimalPoint, provider, out onlineEp)) {
                        //MPTVSeriesLog.Write(string.Format("Series {0} , localEp {1} localEp2 {2} onlineEp {3}", onlineSeason, localEp, localEp2, onlineEp));
                        /*if (!Helper.String.IsNullOrEmpty(onlineEpisode["DVD_season"]) && !Helper.String.IsNullOrEmpty(onlineEpisode["DVD_season"]) &&
                            (localSeason == onlineSeason && ((int)localEp == (int)onlineEp || (int)localEp2 == -1 ? false : (int)localEp2 == (int)onlineEp)))
                        */

                        //if(localEp == (int)onlineEp)
                        string localstring;
                        double localcomp;
                        localstring = (localEp.ToString() + "." + localEp2.ToString());
                        localcomp = Convert.ToDouble(localstring, provider);
                        if (!Helper.String.IsNullOrEmpty(onlineEpisode["DVD_season"]) && !Helper.String.IsNullOrEmpty(onlineEpisode["DVD_episodenumber"]) && (localSeason == onlineSeason && (localcomp == onlineEp || localEp == (int)onlineEp))) {
                            /*check that the vital parts exist DVD_season and DVD_episodenumber, then check to see if we have a match either for the full
                             possible online format of X.Y via the use of localcomp and some string combinations, or through the default style of X.0 
                             via integer comparison*/
                            // overwrite onlineEps season/ep #
                            onlineEpisode[DBOnlineEpisode.cSeasonIndex] = (int)localEpisode[DBEpisode.cSeasonIndex];
                            if (localcomp == onlineEp) {
                                MPTVSeriesLog.Write(string.Format("Episode {0} matched to episode {1}", localEp, onlineEp), MPTVSeriesLog.LogLevel.Debug);
                                onlineEpisode[DBEpisode.cEpisodeIndex] = localcomp;
                            } else if (localEp == (int)onlineEp) {
                                onlineEpisode[DBEpisode.cEpisodeIndex] = localEp;
                            }
                            return true;
                        } else {
                            MPTVSeriesLog.Write(string.Format("File does not match current parse Series: {0} Episode: {1} : Online Episode: {2}", localSeason, localcomp, onlineEp), MPTVSeriesLog.LogLevel.Debug);
                            return false;
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
                    if (onlineabs != -1) {
                        double localabs = -1;
                        if ((int)localEpisode[DBEpisode.cSeasonIndex] == 0) {
                            /*Now we have to figure out whether we are at ep 100 or more*/
                            localabs = Convert.ToDouble(localEpisode[DBEpisode.cSeasonIndex].ToString() + localEpisode[DBEpisode.cEpisodeIndex].ToString());
                        } else if ((int)localEpisode[DBEpisode.cSeasonIndex] >= 1 && (int)localEpisode[DBEpisode.cEpisodeIndex] < 10 /*&& Helper.String.IsNullOrEmpty(localEpisode[DBEpisode.cEpisodeIndex2])*/) {/* Any episode X0[0-9] should be combined in this manner */
                            localabs = Convert.ToDouble(localEpisode[DBEpisode.cSeasonIndex].ToString() + "0" + localEpisode[DBEpisode.cEpisodeIndex].ToString());
                        } else if ((int)localEpisode[DBEpisode.cSeasonIndex] >= 1 && (int)localEpisode[DBEpisode.cEpisodeIndex] >= 10 /*&& Helper.String.IsNullOrEmpty(localEpisode[DBEpisode.cEpisodeIndex2])*/) {/* All other episodes should fall into this category */
                            localabs = Convert.ToDouble(localEpisode[DBEpisode.cSeasonIndex].ToString() + localEpisode[DBEpisode.cEpisodeIndex].ToString());
                        }

                        float.TryParse(onlineEpisode["absolute_number"], System.Globalization.NumberStyles.AllowDecimalPoint, provided, out onlineabs);
                        if (localabs == onlineabs) {
                            localEpisode[DBEpisode.cSeasonIndex] = 1;
                            localEpisode[DBEpisode.cEpisodeIndex] = (int)onlineabs;

                            MPTVSeriesLog.Write(string.Format("Matched Absolute Ep {0} to local ep {1}x{2}", onlineabs, series, localEpisode), MPTVSeriesLog.LogLevel.Debug);
                            return true;
                        } else {
                            MPTVSeriesLog.Write(string.Format("Failed to Match local ep {1}x{2} to Absolute ep {0}", onlineabs, series, localEpisode), MPTVSeriesLog.LogLevel.Debug);
                            return false;
                        }
                    }
                    break;
            }
            return false;
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

            foreach(SQLCondition condition in importProcessedConds)
                DBEpisode.GlobalSet(DBEpisode.cImportProcessed, 1, condition);

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

        private void ParseLocal(List<PathPair> files)
        {
            MPTVSeriesLog.Write(bigLogMessage("Gathering Local Information"));
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

            UpdateStatus(updateStatusEps); //this is what takes the most time for initial parsing of episode.
            MPTVSeriesLog.Write("Adding " + parsedFiles.Count.ToString() + " new file(s) to Database");
            int nSeason = 0;
            List<DBSeries> relatedSeries = new List<DBSeries>();
            List<DBSeason> relatedSeasons = new List<DBSeason>();

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

                    //if it's a new file but the DBOnlineEpisode already has the online ID is set, then ensure that the episode is not hidden
                    if (bNewFile && episode[DBOnlineEpisode.cID] > 0) {
                        episode[DBOnlineEpisode.cHidden] = 0;
                    }

                    episode.Commit();
                }
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
