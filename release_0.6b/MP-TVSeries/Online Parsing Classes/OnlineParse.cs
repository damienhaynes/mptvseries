using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Text;

namespace WindowPlugins.GUITVSeries
{
    class OnlineParsing
    {
        public BackgroundWorker worker = new BackgroundWorker();

        bool m_bDataUpdated = false;
        bool m_bLocalScan = true;
        bool m_bUpdateScan = true;
        bool m_bFullSeriesRetrieval = false;
        bool m_bReparseNeeded = false;

        public delegate void OnlineParsingProgressHandler(int nProgress);
        public delegate void OnlineParsingCompletedHandler(bool bDataUpdated);

        /// <summary>
        /// This will be triggered once all the SeriesAndEpisodeInfo has been parsed completely.
        /// </summary>
        public event OnlineParsingProgressHandler OnlineParsingProgress;
        public event OnlineParsingCompletedHandler OnlineParsingCompleted;

        public OnlineParsing()
        {
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

        public void Start(bool bScanNew, bool bUpdateExisting)
        {
            m_bLocalScan = bScanNew;
            m_bUpdateScan = bUpdateExisting;
            worker.RunWorkerAsync();
        }

        public bool LocalScan
        {
            get { return m_bLocalScan; }
        }

        public bool UpdateScan
        {
            get { return m_bUpdateScan; }
        }

        public void Cancel()
        {
            worker.CancelAsync();
            //            m_bAbort = true;
        }

        public void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
            MPTVSeriesLog.Write("***************************************************************************");
            MPTVSeriesLog.Write("*******************       Parsing Starting      ***************************");
            MPTVSeriesLog.Write("***************************************************************************");
            m_bFullSeriesRetrieval = DBOption.GetOptions(DBOption.cFullSeriesRetrieval);
            worker.ReportProgress(0);

            if (m_bLocalScan)
            {
                LocalParse();
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
                    if (m_bUpdateScan)
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
                    if (m_bUpdateScan)
                    {
                        // refresh existing banners
                        UpdateBanners(false);
                    }

                    // now update the episodes
                    UpdateEpisodes(true);

                    if (m_bUpdateScan)
                    {
                        // now refresh existing episodes
                        UpdateEpisodes(false);
                    }
                }
            }
            // and we are done, the backgroundworker is going to notify so
            MPTVSeriesLog.Write("***************************************************************************");
            MPTVSeriesLog.Write("*******************       Parsing Completed     ***************************");
            MPTVSeriesLog.Write("***************************************************************************");
        }

        void LocalParse()
        {
            // mark all files in the db as not processed (to figure out which ones we'll have to remove after the import)
            DBEpisode.GlobalSet(DBEpisode.cImportProcessed, 2);
            // also clear all season & series for local files
            DBSeries.GlobalSet(DBOnlineSeries.cHasLocalFilesTemp, false);
            DBSeason.GlobalSet(DBSeason.cHasLocalFilesTemp, false);
            LocalParse localParser = new LocalParse();
            localParser.DoParse(false);

            foreach (parseResult progress in localParser.Results)
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
                    season[DBSeason.cHasLocalFilesTemp] = 1;
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

        void GetSeries()
        {
            MPTVSeriesLog.Write("*********  GetSeries - unknown series  *********");

            SQLCondition condition = null;
            if (m_bUpdateScan)
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

                    if (GetSeriesParser.Results.Count > 0)
                    {
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

                        DBOnlineSeries UserChosenSeries = GetSeriesParser.Results[0];

                        SelectSeries userSelection = null;
                        if (ExactMatchCount != 1 || (SubStringCount != 1 && DBOption.GetOptions(DBOption.cAutoChooseSeries) == 0))
                        {
                            // 
                            // User has three choices:
                            // 1) Pick a series from the list
                            // 2) Simply skip
                            // 3) Skip and never ask for this series again
                            userSelection = new SelectSeries(sSeriesNameToSearch, true);
                            userSelection.addSeriesToSelection(GetSeriesParser.Results);
                            DialogResult result = userSelection.ShowDialog();
                            switch (result)
                            {
                                case DialogResult.Cancel:
                                    UserChosenSeries = null;
                                    bDone = true;
                                    break;

                                case DialogResult.Ignore:
                                    UserChosenSeries = null;
                                    series[DBSeries.cScanIgnore] = 1; // means it will be skipped in the future
                                    series[DBSeries.cHidden] = true;
                                    series.Commit();
                                    bDone = true;
                                    break;

                                case DialogResult.OK:
                                    if (sSeriesNameToSearch != userSelection.SeriesName)
                                    {
                                        sSeriesNameToSearch = userSelection.SeriesName;
                                        UserChosenSeries = null;
                                    }
                                    else
                                    {
                                        UserChosenSeries = userSelection.userChoice;
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
                    else
                    {
                        MPTVSeriesLog.Write("No matching names found for " + sSeriesNameToSearch);
                        // ask for an alternative name
                        SelectSeries userSelection = new SelectSeries(sSeriesNameToSearch, false);
                        userSelection.addSeriesToSelection(GetSeriesParser.Results);
                        DialogResult result = userSelection.ShowDialog();
                        switch (result)
                        {
                            case DialogResult.Cancel:
                                bDone = true;
                                break;

                            case DialogResult.Ignore:
                                series[DBSeries.cScanIgnore] = 1; // means it will be skipped in the future
                                series[DBSeries.cHidden] = true;
                                series.Commit();
                                bDone = true;
                                break;

                            case DialogResult.OK:
                                sSeriesNameToSearch = userSelection.SeriesName;
                                break;
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
            if (m_bUpdateScan)
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

                if (m_bFullSeriesRetrieval && m_bUpdateScan)
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
                // call update with batches of 100 ids max - otherwise the server fails to generate a big enough xml chunk
                while (/*nCount < 100 && */episodeList.Count > 0)
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
                                case DBEpisode.cWatched:
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