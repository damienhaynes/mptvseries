using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;

namespace WindowPlugins.GUITVSeries
{
    class OnlineParsing
    {
        LocalParse m_localParser = null;
        GetSeries m_GetSeriesParser = null;
        GetEpisodes m_GetEpisodesParser = null;
        UpdateSeries m_UpdateSeriesParser = null;
        GetBanner m_GetBannerParser = null;
        UpdateEpisodes m_updateEpisodesParser = null;

        bool m_bAbort = false;
        bool m_bFullSeriesRetrieval = false;
        List<DBSeries> m_SeriesList = new List<DBSeries>();
        int m_nCurrentSeriesIndex = 0;
        List<DBEpisode> m_EpisodeList = new List<DBEpisode>();
        int m_nCurrentEpisodeIndex = 0;

        bool m_bSeries_UpdateNew = false;
        bool m_bEpisodes_UpdateNew = false;
        bool m_bSeriesBanners_UpdateNew = false;

        Dictionary<int, DBSeries> m_IDToSeriesMap = new Dictionary<int,DBSeries>();
        Dictionary<int, DBEpisode> m_IDToEpisodesMap = new Dictionary<int, DBEpisode>();

        bool m_bReparseNeeded = false;

        // used for progress only
        int m_nTotalUpdateEpisode = 0;

        public delegate void OnlineParsingProgressHandler(int nProgress);
        public delegate void OnlineParsingCompletedHandler();

        /// <summary>
        /// This will be triggered once all the SeriesAndEpisodeInfo has been parsed completely.
        /// </summary>
        public event OnlineParsingProgressHandler OnlineParsingProgress;
        public event OnlineParsingCompletedHandler OnlineParsingCompleted;

        public void Cancel()
        {
            m_bAbort = true;
        }

        public void Start()
        {
            m_bFullSeriesRetrieval = DBOption.GetOptions(DBOption.cFullSeriesRetrieval);

            // mark all files in the db as not processed (to figure out which ones we'll have to remove after the import)
            DBEpisode.GlobalSet(DBEpisode.cImportProcessed, 2);
            m_localParser = new LocalParse();
            m_localParser.worker.ProgressChanged += new ProgressChangedEventHandler(LocalParsing_LocalParseProgress);
            m_localParser.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LocalParsing_LocalParseCompleted);
            m_localParser.DoParse();
        }

        void LocalParsing_LocalParseProgress(object sender, ProgressChangedEventArgs e)
        {
            if (m_bAbort)
            {
                if (m_localParser != null)
                {
                    m_localParser.worker.CancelAsync();
                    m_localParser = null;
                    Aborted();
                }
                return;
            }

            List<parseResult> results = (List<parseResult>)e.UserState;
            LocalParsing_ProcessResults(results);
            if (DBOption.GetOptions(DBOption.cOnlineParseEnabled) == 1)
                if (OnlineParsingProgress != null) // only if any subscribers exist
                    OnlineParsingProgress.Invoke(e.ProgressPercentage / 10);
            else
                if (OnlineParsingProgress != null) // only if any subscribers exist
                    OnlineParsingProgress.Invoke(e.ProgressPercentage);
        }

        void LocalParsing_LocalParseCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<parseResult> results = (List<parseResult>)e.Result;
            if (OnlineParsingProgress != null) // only if any subscribers exist
                OnlineParsingProgress.Invoke(10);
            LocalParsing_ProcessResults(results);

            // now, remove all episodes still processed = 0, the weren't find in the scan
            DBEpisode.Clear(DBEpisode.cImportProcessed, 2);

            // now on with online parsing
            if (DBOption.GetOptions(DBOption.cOnlineParseEnabled) == 1)
            {
                // asynchronous execution: we start with GetSeries, and the execution path goes on through the various worker events
                MatchSeries_Next(true);
            }
            else
            {
                // and we are done
                if (OnlineParsingCompleted != null) // only if any subscribers exist
                {
                    this.OnlineParsingCompleted.Invoke();
                }
            }
        }

        void LocalParsing_ProcessResults(List<parseResult> results)
        {
            foreach (parseResult progress in results)
            {
                if (progress.success)
                {
                    int nEpisode = Convert.ToInt32(progress.parser.Matches[DBEpisode.cEpisodeIndex]);
                    int nSeason = Convert.ToInt32(progress.parser.Matches[DBEpisode.cSeasonIndex]);

                    // ok, we are sure it's valid now
                    // series first
                    DBSeries series = new DBSeries(progress.parser.Matches[DBSeries.cParsedName]);
                    // not much to do here except commiting the series
                    series.Commit();

                    // season now
                    DBSeason season = new DBSeason(progress.parser.Matches[DBSeries.cParsedName], nSeason);
                    season.Commit();

                    // then episode
                    DBEpisode episode = new DBEpisode(progress.full_filename);
                    bool bNewFile = false;
                    if (episode[DBEpisode.cImportProcessed] != 2)
                    {
                        bNewFile = true;
                    }
                    episode[DBEpisode.cImportProcessed] = 1;

                    foreach (KeyValuePair<string, string> match in progress.parser.Matches)
                    {
                        episode.AddColumn(match.Key, new DBField(DBField.cTypeString));
                        if (bNewFile || episode[match.Key].ToString() != match.Value)
                            episode[match.Key] = match.Value;
                    }
                    episode.Commit();
                }
            }
        }

        private void MatchSeries_Next(bool bStart)
        {
            if (bStart)
            {
                DBTVSeries.Log("*********  GetSeries - unknown series  *********");
                SQLCondition condition = new SQLCondition(new DBSeries());
                // all series that don't have an onlineID ( = 0)
                condition.Add(DBSeries.cID, 0, true);
                m_SeriesList = DBSeries.Get(condition);
                m_nCurrentSeriesIndex = 0;
                DBTVSeries.Log("Found " + m_SeriesList.Count + " Series without an online ID, looking for them");
            }
            else 
            {
                m_nCurrentSeriesIndex++;
            }

            if (m_SeriesList.Count > 0)
                if (OnlineParsingProgress != null) // only if any subscribers exist
                    OnlineParsingProgress.Invoke(10 + (10 * m_nCurrentSeriesIndex / m_SeriesList.Count));
            if (m_nCurrentSeriesIndex >= m_SeriesList.Count)
            {
                if (OnlineParsingProgress != null) // only if any subscribers exist
                    OnlineParsingProgress.Invoke(20);
                // we did all our series. Go to the next step, the episode retrieval
                MatchEpisodes_Next(true);
            }
            else
            {
                m_GetSeriesParser = new GetSeries(m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName]);
                m_GetSeriesParser.m_Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnlineParsing_GetSeriesCompleted);
                m_GetSeriesParser.DoParse();
            }
        }

        void OnlineParsing_GetSeriesCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (m_bAbort)
            {
                if (m_GetSeriesParser != null)
                {
                    m_GetSeriesParser.m_Worker.CancelAsync();
                    m_GetSeriesParser = null;
                    Aborted();
                }
                return;
            }

            GetSeriesResults results = (GetSeriesResults)e.Result;

            if (results.listSeries.Count > 0)
            {
                DBTVSeries.Log("Found " + m_SeriesList.Count + " matching names for " + m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName]);
                DBSeries UserChosenSeries = results.listSeries[0];

                SelectSeries userSelection = null;
                if (results.listSeries.Count > 1 && UserChosenSeries[DBSeries.cPrettyName].ToString().ToLowerInvariant() != m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName].ToString().ToLowerInvariant())
                {
                    // 
                    // User has three choices:
                    // 1) Pick a series from the list
                    // 2) Simply skip
                    // 3) Skip and never ask for this series again
                    userSelection = new SelectSeries(m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName]);
                    userSelection.addSeriesToSelection(results.listSeries);
                    DialogResult result = userSelection.ShowDialog();
                    switch (result)
                    {
                        case DialogResult.Cancel:
                            UserChosenSeries = null;
                            break;

                        case DialogResult.Ignore:
                            UserChosenSeries = null;
                            m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cID] = -1; // ID -1 means it will be skipped in the future
                            m_SeriesList[m_nCurrentSeriesIndex].Commit();
                            break;

                        case DialogResult.OK:
                            UserChosenSeries = userSelection.userChoice;
                            break;
                    }
                }

                if (UserChosenSeries != null) // make sure selection was not cancelled
                {
                    // set the ID on the current series with the one from the chosen one
                    m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cID] = UserChosenSeries[DBSeries.cID];
                    m_SeriesList[m_nCurrentSeriesIndex].Commit();
                }
            }
            else 
            {
                DBTVSeries.Log("No matching names found for " + m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName]);
            }

            // process next series
            MatchSeries_Next(false);
        }

        void MatchEpisodes_Next(bool bStart)
        {
            if (bStart)
            {
                DBTVSeries.Log("*********  Starting GetEpisodes  *********");
                SQLCondition condition = new SQLCondition(new DBSeries());
                // all series that have an onlineID ( != 0)
                condition.Add(DBSeries.cID, 0, false);
                condition.Add(DBSeries.cID, -1, false); // for series that were
                m_SeriesList = DBSeries.Get(condition);
                m_nCurrentSeriesIndex = 0;
            }
            else
            {
                m_nCurrentSeriesIndex++;
            }

            if (m_SeriesList.Count > 0)
                if (OnlineParsingProgress != null) // only if any subscribers exist
                    OnlineParsingProgress.Invoke(20 + (20 * m_nCurrentSeriesIndex / m_SeriesList.Count));
            if (m_nCurrentSeriesIndex >= m_SeriesList.Count)
            {
                if (OnlineParsingProgress != null) // only if any subscribers exist
                    OnlineParsingProgress.Invoke(40);
                // we did all our series. Go to the next step, the series data update
                UpdateSeries(true);
            }
            else
            {
                if (m_bFullSeriesRetrieval)
                {
                    DBTVSeries.Log("Looking for all the episodes of " + m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName]);
                    m_GetEpisodesParser = new GetEpisodes(m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cID]);
                    m_GetEpisodesParser.m_Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnlineParsing_GetEpisodesCompleted);
                    m_GetEpisodesParser.DoParse();
                }
                else
                {
                    // if just retrieving info for existing files, for each series we have an ID of, build the list of episodes without ids;
                    // if there are less than 5 episodes in the list, do them individually (saves server bandwidth)))
                    // otherwise retrieve the full list
                    MatchEpisodeLocalOnly_Next(true);
                }
            }
        }

        void MatchEpisodeLocalOnly_Next(bool bStart)
        {
            if (bStart)
            {
                // build the list of unidentified episodes
                SQLCondition conditions = new SQLCondition(new DBOnlineEpisode());
                conditions.Add(DBOnlineEpisode.cSeriesParsedName, m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName], true);
                conditions.Add(DBOnlineEpisode.cID, 0, true);
                m_EpisodeList = DBEpisode.Get(conditions);
                m_nCurrentEpisodeIndex = 0;
            }
            else 
            {
                m_nCurrentEpisodeIndex++;
            }

            if (m_nCurrentEpisodeIndex >= m_EpisodeList.Count)
            {
                // we did all our unidentified episodes, go to the next series in the list
                MatchEpisodes_Next(false);
            }
            else
            {
                if (m_EpisodeList.Count < 5)
                {
                    DBTVSeries.Log("Looking for the single episode " + m_EpisodeList[m_nCurrentEpisodeIndex][DBOnlineEpisode.cCompositeID]);
                    m_GetEpisodesParser = new GetEpisodes(m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cID], m_EpisodeList[m_nCurrentEpisodeIndex][DBEpisode.cSeasonIndex], m_EpisodeList[m_nCurrentEpisodeIndex][DBEpisode.cEpisodeIndex]);
                    m_GetEpisodesParser.m_Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnlineParsing_GetEpisodesLocalOnlyCompleted);
                    m_GetEpisodesParser.DoParse();
                }
                else
                {
                    // no need to do single matches for many episodes, it's more efficient to do it all at once
                    DBTVSeries.Log("Looking for all the episodes of " + m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName]);
                    m_GetEpisodesParser = new GetEpisodes(m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cID]);
                    m_GetEpisodesParser.m_Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnlineParsing_GetEpisodesCompleted);
                    m_GetEpisodesParser.DoParse();
                }
            }
        }

        void OnlineParsing_GetEpisodesLocalOnlyCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (m_bAbort)
            {
                if (m_GetEpisodesParser != null)
                {
                    m_GetEpisodesParser.m_Worker.CancelAsync();
                    m_GetEpisodesParser = null;
                    Aborted();
                }
                return;
            }

            GetEpisodesResults results = (GetEpisodesResults)e.Result;

            if (results.listEpisodes.Count > 0)
            {
                DBOnlineEpisode onlineEpisode = results.listEpisodes[0];
                DBTVSeries.Log("Found episodeID for " + m_EpisodeList[m_nCurrentEpisodeIndex][DBOnlineEpisode.cCompositeID]);
                m_EpisodeList[m_nCurrentEpisodeIndex][DBOnlineEpisode.cID] = onlineEpisode[DBOnlineEpisode.cID];
                if (m_EpisodeList[m_nCurrentEpisodeIndex][DBOnlineEpisode.cEpisodeName] == "")
                    m_EpisodeList[m_nCurrentEpisodeIndex][DBOnlineEpisode.cEpisodeName] = onlineEpisode[DBOnlineEpisode.cEpisodeName];
                m_EpisodeList[m_nCurrentEpisodeIndex].Commit();
            }
            MatchEpisodeLocalOnly_Next(false);            
        }

        void OnlineParsing_GetEpisodesCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (m_bAbort)
            {
                if (m_GetEpisodesParser != null)
                {
                    m_GetEpisodesParser.m_Worker.CancelAsync();
                    m_GetEpisodesParser = null;
                    Aborted();
                }
                return;
            }

            GetEpisodesResults results = (GetEpisodesResults)e.Result;

            if (results.listEpisodes.Count > 0)
            {
                DBTVSeries.Log("Found " + results.listEpisodes.Count + " episodes for " + m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName]);
                if (m_bFullSeriesRetrieval)
                {
                    // add all online episodes in the local db
                    foreach (DBOnlineEpisode onlineEpisode in results.listEpisodes)
                    {
                        // season if not there yet
                        DBSeason season = new DBSeason(m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName], onlineEpisode[DBOnlineEpisode.cSeasonIndex]);
                        season.Commit();

                        DBOnlineEpisode newOnlineEpisode = new DBOnlineEpisode(m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName], onlineEpisode[DBOnlineEpisode.cSeasonIndex], onlineEpisode[DBOnlineEpisode.cEpisodeIndex]);
                        newOnlineEpisode[DBOnlineEpisode.cID] = onlineEpisode[DBOnlineEpisode.cID];
                        if (newOnlineEpisode[DBOnlineEpisode.cEpisodeName] == "")
                            newOnlineEpisode[DBOnlineEpisode.cEpisodeName] = onlineEpisode[DBOnlineEpisode.cEpisodeName];
                        newOnlineEpisode.Commit();
                    }
                }
                else
                {
                    // look for the episodes for that series, and compare / update the values
                    foreach (DBOnlineEpisode onlineEpisode in results.listEpisodes)
                    {
                        foreach (DBEpisode localEpisode in m_EpisodeList)
                        {
                            if ((int)localEpisode[DBEpisode.cSeasonIndex] == (int)onlineEpisode[DBOnlineEpisode.cSeasonIndex] &&
                                (int)localEpisode[DBEpisode.cEpisodeIndex] == (int)onlineEpisode[DBOnlineEpisode.cEpisodeIndex])
                            {
                                DBTVSeries.Log(localEpisode[DBOnlineEpisode.cCompositeID] + " identified");
                                // update data
                                localEpisode[DBOnlineEpisode.cID] = onlineEpisode[DBOnlineEpisode.cID];
                                if (m_EpisodeList[m_nCurrentEpisodeIndex][DBOnlineEpisode.cEpisodeName] == "")
                                    localEpisode[DBOnlineEpisode.cEpisodeName] = onlineEpisode[DBOnlineEpisode.cEpisodeName];
                                localEpisode.Commit();
                                // remove the localEpisode from the local list (we found it, it's updated, it's faster this way)
                                m_EpisodeList.Remove(localEpisode);
                                break;
                            }
                        }
                    }
                }
            }

            // process next series
            MatchEpisodes_Next(false);
        }

        void UpdateSeries(bool bUpdateEmptySeries)
        {
            m_SeriesList.Clear();
            m_EpisodeList.Clear();

            long nUpdateSeriesTimeStamp = 0;
            // now retrieve the info about the series
            SQLCondition condition = new SQLCondition(new DBSeries());
            // all series that have an onlineID ( != 0)
            condition.Add(DBSeries.cID, 0, false);
            condition.Add(DBSeries.cID, -1, false);
            if (bUpdateEmptySeries)
            {
                DBTVSeries.Log("*********  UpdateSeries - retrieve unknown series  *********");
                // and that never had data imported from the online DB
                condition.Add(DBSeries.cOnlineImportProcessed, 0, true);
                // in that case, don't use the lasttime of import
                nUpdateSeriesTimeStamp = 0;
            }
            else
            {
                DBTVSeries.Log("*********  UpdateSeries - refresh series  *********");
                // and that already had data imported from the online DB
                condition.Add(DBSeries.cOnlineImportProcessed, 0, false);
                nUpdateSeriesTimeStamp = (long)DBOption.GetOptions(DBOption.cUpdateSeriesTimeStamp);
            }
            m_bSeries_UpdateNew = bUpdateEmptySeries;

            List<DBSeries> SeriesList = DBSeries.Get(condition);

            m_IDToSeriesMap.Clear();
            // build the map for faster retrieval
            foreach (DBSeries series in SeriesList)
                m_IDToSeriesMap.Add(series[DBSeries.cID], series);

            // generate a comma separated list of all the series ID
            String sSeriesIDs = String.Empty;
            foreach (KeyValuePair<int, DBSeries> pair in m_IDToSeriesMap)
            {
                if (sSeriesIDs == String.Empty)
                    sSeriesIDs += pair.Value[DBSeries.cID];
                else
                    sSeriesIDs += "," + pair.Value[DBSeries.cID];
            }

            // use the last known timestamp from when we updated the series
            DBTVSeries.Log("Updating " + SeriesList.Count + " Series");
            m_UpdateSeriesParser = new UpdateSeries(sSeriesIDs, nUpdateSeriesTimeStamp);
            m_UpdateSeriesParser.m_Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnlineParsing_UpdateSeriesCompleted);
            m_UpdateSeriesParser.DoParse();
        }

        void OnlineParsing_UpdateSeriesCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (m_bAbort)
            {
                if (m_UpdateSeriesParser != null)
                {
                    m_UpdateSeriesParser.m_Worker.CancelAsync();
                    m_UpdateSeriesParser = null;
                    Aborted();
                }
                return;
            }

            UpdateSeriesResults results = (UpdateSeriesResults)e.Result;
            
            if (results != null)
            {
                foreach (DBSeries onlineSeries in results.listSeries)
                {
                    DBTVSeries.Log("Updating data for " + onlineSeries[DBSeries.cPrettyName]);
                    // find the corresponding series in our list
                    DBSeries localSeries = m_IDToSeriesMap[onlineSeries[DBSeries.cID]];

                    if (localSeries != null)
                    {
                        // go over all the fields, (and update only those which haven't been modified by the user - will do that later)
                        foreach (String key in onlineSeries.FieldNames)
                        {
                            if (key != DBSeries.cParsedName)
                            {
                                localSeries.AddColumn(key, new DBField(DBField.cTypeString));
                                localSeries[key] = onlineSeries[key];
                            }
                        }
                        localSeries[DBSeries.cOnlineImportProcessed] = 1;
                        localSeries.Commit();
                    }
                    else
                    {
                        // hopefully the server will NEVER return an ID I didn't asked for!
                    }
                }

                // now process incorrect IDs if any
                foreach (int nIncorrectID in results.listIncorrectIDs)
                {
                    m_bReparseNeeded = true;
                    DBTVSeries.Log("Incorrect SeriesID found! ID=" + nIncorrectID + " for local series '" + m_IDToSeriesMap[nIncorrectID][DBSeries.cParsedName] + "'");
                    // reset the seriesID of this series
                    m_IDToSeriesMap[nIncorrectID][DBSeries.cID] = 0;
                    m_IDToSeriesMap[nIncorrectID][DBSeries.cOnlineImportProcessed] = 0;
                    m_IDToSeriesMap[nIncorrectID].Commit();
                }
            }

            if (m_bSeries_UpdateNew)
            {
                if (OnlineParsingProgress != null) // only if any subscribers exist
                    OnlineParsingProgress.Invoke(45);
                // now do an regular update (refresh, with timestamp) on all the series
                UpdateSeries(false);
            }
            else 
            {
                if (OnlineParsingProgress != null) // only if any subscribers exist
                    OnlineParsingProgress.Invoke(50);
                // save last series timestamp
                if (results != null)
                    DBOption.SetOptions(DBOption.cUpdateSeriesTimeStamp, results.m_nServerTimeStamp);

                UpdateBanners(true, true);
            }
        }

        void UpdateBanners(bool bUpdateNewSeries, bool bStart)
        {
            if (bStart)
            {
                SQLCondition condition = new SQLCondition(new DBSeries());
                // all series that don't have an onlineID ( = 0)
                condition.Add(DBSeries.cID, 0, false);
                condition.Add(DBSeries.cID, -1, false);
                if (bUpdateNewSeries)
                {
                    DBTVSeries.Log("*********  UpdateBanners - retrieve banners for new series  *********");
                    // and that never had data imported from the online DB
                    condition.Add(DBSeries.cOnlineImportProcessed, 2, false);
                }
                else
                {
                    DBTVSeries.Log("*********  UpdateBanners - refresh banners for new series  *********");
                    // and that already had data imported from the online DB
                    condition.Add(DBSeries.cOnlineImportProcessed, 2, true);
                }
                m_bSeriesBanners_UpdateNew = bUpdateNewSeries;
                m_SeriesList = DBSeries.Get(condition);
                m_nCurrentSeriesIndex = 0;
                DBTVSeries.Log("Looking for banners on " + m_SeriesList.Count + " Series");
            }
            else
            {
                m_nCurrentSeriesIndex++;
            }

            if (m_SeriesList.Count > 0)
                if (OnlineParsingProgress != null) // only if any subscribers exist
                    OnlineParsingProgress.Invoke(50 + (bUpdateNewSeries ? 0 : 10) + (10 * m_nCurrentSeriesIndex / m_SeriesList.Count));
            else
                if (OnlineParsingProgress != null) // only if any subscribers exist
                    OnlineParsingProgress.Invoke(50 + (bUpdateNewSeries ? 10 : 20));

            long nUpdateBannersTimeStamp = 0;
            // in that case, don't use the lasttime of import
            if (!bUpdateNewSeries)
                nUpdateBannersTimeStamp = (long)DBOption.GetOptions(DBOption.cUpdateBannersTimeStamp);

            if (m_nCurrentSeriesIndex >= m_SeriesList.Count)
            {
                if (m_bSeriesBanners_UpdateNew)
                {
                    // refresh existing banners
                    UpdateBanners(false, true);
                }
                else
                {
                    // now that the update is done, copy the timestamp from the series update into the banner one
                    // HACK: we need a value returned from the server
                    DBOption.SetOptions(DBOption.cUpdateSeriesTimeStamp, DBOption.GetOptions(DBOption.cUpdateSeriesTimeStamp));

                    // now update the episodes
                    UpdateEpisodes_Next(true, true);
                }
            }
            else
            {
                if (m_bSeriesBanners_UpdateNew)
                    DBTVSeries.Log("Downloading banners for " + m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName]);
                else
                    DBTVSeries.Log("Refreshing banners for " + m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName]);
                m_GetBannerParser = new GetBanner(m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cID], nUpdateBannersTimeStamp);
                m_GetBannerParser.m_Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnlineParsing_UpdateBannersCompleted);
                m_GetBannerParser.DoParse();
            }
        }

        void OnlineParsing_UpdateBannersCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (m_bAbort)
            {
                if (m_GetBannerParser != null)
                {
                    m_GetBannerParser.m_Worker.CancelAsync();
                    m_GetBannerParser = null;
                    Aborted();
                }
                return;
            }

            GetBannersResults results = (GetBannersResults)e.Result;
            String sLastTextBanner = String.Empty;
            String sLastGraphicalBanner = String.Empty;

            foreach (BannerSeries bannerSeries in results.bannerSeriesList)
            {
                if (m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cBannerFileNames].ToString().IndexOf(bannerSeries.sBannerFileName) == -1)
                {
                    if (m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cBannerFileNames] == String.Empty)
                        m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cBannerFileNames] += bannerSeries.sBannerFileName;
                    else
                    {
                        DBTVSeries.Log("New banner found for " + m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName] + " : " + bannerSeries.sOnlineBannerPath);
                        m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cBannerFileNames] += "|" + bannerSeries.sBannerFileName;
                    }
                }
                // prefer graphical
                if (bannerSeries.bGraphical)
                    sLastGraphicalBanner = bannerSeries.sBannerFileName;
                else
                    sLastTextBanner = bannerSeries.sBannerFileName;
            }
            // use the last banner as the current one (if any graphical found)
            // otherwise use the first available
            if (sLastGraphicalBanner != String.Empty)
                m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cCurrentBannerFileName] = sLastGraphicalBanner;
            else
                m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cCurrentBannerFileName] = sLastTextBanner;

            m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cOnlineImportProcessed] = 2;
            m_SeriesList[m_nCurrentSeriesIndex].Commit();

            foreach (BannerSeason bannerSeason in results.bannerSeasonList)
            {
                DBSeason season = new DBSeason(m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName], bannerSeason.nIndex);
                if (season[DBSeason.cBannerFileNames].ToString().IndexOf(bannerSeason.sBannerFileName) == -1)
                {
                    if (season[DBSeason.cBannerFileNames] == String.Empty)
                    {
                        season[DBSeason.cBannerFileNames] += bannerSeason.sBannerFileName;
                    }
                    else
                    {
                        season[DBSeason.cBannerFileNames] += "|" + bannerSeason.sBannerFileName;
                        DBTVSeries.Log("New banner found for " + season[DBSeason.cID] + " : " + bannerSeason.sOnlineBannerPath);
                    }
                }
                // use the last banner as the current one
                season[DBSeason.cCurrentBannerFileName] = bannerSeason.sBannerFileName;
                season.Commit();
            }

            // process next series
            UpdateBanners(m_bSeriesBanners_UpdateNew, false);
        }


        void UpdateEpisodes_Next(bool bUpdateNewEpisodes, bool bStart)
        {
            // now retrieve the info about the series
            if (bStart)
            {
                SQLCondition condition = new SQLCondition(new DBOnlineEpisode());
                // all series that have an onlineID ( != 0)
                condition.Add(DBOnlineEpisode.cID, 0, false);
                condition.Add(DBOnlineEpisode.cID, -1, false);

                if (bUpdateNewEpisodes)
                {
                    DBTVSeries.Log("*********  UpdateEpisodes - retrieve unknown episodes  *********");
                    // and that never had data imported from the online DB
                    condition.Add(DBOnlineEpisode.cOnlineImportProcessed, 0, true);
                }
                else
                {
                    DBTVSeries.Log("*********  UpdateEpisodes - refresh episodes  *********");
                    // and that already had data imported from the online DB
                    condition.Add(DBOnlineEpisode.cOnlineImportProcessed, 0, false);
                }
                m_bEpisodes_UpdateNew = bUpdateNewEpisodes;

                m_EpisodeList = DBEpisode.Get(condition);
                m_nTotalUpdateEpisode = m_EpisodeList.Count;
                m_nCurrentEpisodeIndex = 0;
            }

            if (m_nTotalUpdateEpisode > 0)
                if (OnlineParsingProgress != null) // only if any subscribers exist
                    OnlineParsingProgress.Invoke(70 + (bUpdateNewEpisodes ? 0 : 15) + (15 * m_nCurrentEpisodeIndex / m_nTotalUpdateEpisode));
            else
                if (OnlineParsingProgress != null) // only if any subscribers exist
                    OnlineParsingProgress.Invoke(70 + (bUpdateNewEpisodes ? 15 : 30));

            long nUpdateEpisodesTimeStamp = 0;
            // in that case, don't use the lasttime of import
            if (!bUpdateNewEpisodes)
                nUpdateEpisodesTimeStamp = (long)DBOption.GetOptions(DBOption.cUpdateEpisodesTimeStamp);

            m_IDToEpisodesMap.Clear();
            int nCount = 0;
            while ((nCount < 400 || nUpdateEpisodesTimeStamp != 0) && m_EpisodeList.Count > 0)
            {
                DBEpisode episode = m_EpisodeList[0];
                m_EpisodeList.RemoveAt(0);
                if (!m_IDToEpisodesMap.ContainsKey(episode[DBOnlineEpisode.cID]))
                    m_IDToEpisodesMap.Add(episode[DBOnlineEpisode.cID], episode);

                nCount++;
                m_nCurrentEpisodeIndex++;
            }

            // generate a comma separated list of all the series ID
            String sEpisodeIDs = String.Empty;
            foreach (KeyValuePair<int, DBEpisode> pair in m_IDToEpisodesMap)
            {
                if (sEpisodeIDs == String.Empty)
                    sEpisodeIDs += pair.Value[DBOnlineEpisode.cID];
                else
                    sEpisodeIDs += "," + pair.Value[DBOnlineEpisode.cID];
            }

            // use the last known timestamp from when we updated the series
            DBTVSeries.Log("Updating " + m_IDToEpisodesMap.Count + " Episodes, " + m_EpisodeList.Count + " left");
            m_updateEpisodesParser = new UpdateEpisodes(sEpisodeIDs, nUpdateEpisodesTimeStamp);
            m_updateEpisodesParser.m_Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnlineParsing_UpdateEpisodesCompleted);
            m_updateEpisodesParser.DoParse();
        }

        void OnlineParsing_UpdateEpisodesCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (m_bAbort)
            {
                if (m_updateEpisodesParser != null)
                {
                    m_updateEpisodesParser.m_Worker.CancelAsync();
                    m_updateEpisodesParser = null;
                    Aborted();
                }
                return;
            }

            UpdateEpisodesResults results = (UpdateEpisodesResults)e.Result;

            if (results != null)
            {
                foreach (DBOnlineEpisode onlineEpisode in results.listEpisodes)
                {
                    // find the corresponding series in our list
                    DBEpisode localEpisode = m_IDToEpisodesMap[onlineEpisode[DBOnlineEpisode.cID]];
                    DBTVSeries.Log("Updating data for " + localEpisode[DBEpisode.cCompositeID]);

                    if (localEpisode != null)
                    {
                        // go over all the fields, (and update only those which haven't been modified by the user - will do that later)
                        foreach (String key in onlineEpisode.FieldNames)
                        {
                            switch (key)
                            {
                                case DBOnlineEpisode.cCompositeID:
                                case DBEpisode.cSeriesParsedName:
                                    // do nothing here, it would break the DB links
                                    break;

                                default:
                                    localEpisode.onlineEpisode.AddColumn(key, new DBField(DBField.cTypeString));
                                    localEpisode[key] = onlineEpisode[key];
                                    break;
                            }
                        }
                        localEpisode[DBOnlineEpisode.cOnlineImportProcessed] = 1;
                        localEpisode.Commit();
                    }
                    else
                    {
                        // hopefully the server will NEVER return an ID I didn't asked for!
                    }
                }

                // now process incorrect IDs if any
                foreach (int nIncorrectID in results.listIncorrectIDs)
                {
                    m_bReparseNeeded = true;
                    DBTVSeries.Log("Incorrect EpisodeID found! ID=" + nIncorrectID + " for episode '" + m_IDToEpisodesMap[nIncorrectID][DBOnlineEpisode.cCompositeID] + "'");
                    // reset the seriesID of this series
                    m_IDToEpisodesMap[nIncorrectID][DBOnlineEpisode.cID] = 0;
                    m_IDToEpisodesMap[nIncorrectID][DBOnlineEpisode.cOnlineImportProcessed] = 0;
                    m_IDToEpisodesMap[nIncorrectID].Commit();
                }
            }

            if (m_EpisodeList.Count > 0)
            {
                // next batch
                UpdateEpisodes_Next(m_bEpisodes_UpdateNew, false);
            }
            else 
            {
                if (m_bEpisodes_UpdateNew)
                {
                    // now refresh existing episodes
                    UpdateEpisodes_Next(false, true);
                }
                else
                {
                    // save last episodes timestamp
                    if (results != null)
                        DBOption.SetOptions(DBOption.cUpdateEpisodesTimeStamp, results.m_nServerTimeStamp);

                    if (m_bReparseNeeded)
                    {
                        // we ran into some incorrect IDS, do it all again
                        MatchSeries_Next(true);
                    }
                    else
                    {
                        // and we are done
                        if (OnlineParsingCompleted != null) // only if any subscribers exist
                        {
                            this.OnlineParsingCompleted.Invoke();
                        }
                    }
                }
            }
        }

        private void Aborted()
        {
            this.OnlineParsingCompleted.Invoke();
        }
    }
}
