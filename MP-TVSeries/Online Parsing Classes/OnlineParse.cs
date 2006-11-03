using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WindowPlugins.GUITVSeries
{
    class OnlineParse
    {
        List<DBSeries> m_SeriesList = new List<DBSeries>();
        int m_nCurrentSeriesIndex = 0;
        public delegate void GetSeriesEpisodesCompletedHandler();

        /// <summary>
        /// This will be triggered once all the SeriesAndEpisodeInfo has been parsed completely.
        /// </summary>
        public event GetSeriesEpisodesCompletedHandler GetSeriesEpisodesCompleted;

        public void Start()
        {
            
            // asynchronous execution: we start with GetSeries, and the execution path goes on through the various worker events
            MatchSeries_Next(true);
        }


        private void MatchSeries_Next(bool bStart)
        {

            if (bStart)
            {
                DBTVSeries.Log("*********  Starting GetSeries  *********");
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

            if (m_nCurrentSeriesIndex >= m_SeriesList.Count)
            {
                // we did all our series. Go to the next step, the episode retrieval
                MatchEpisodes_Next(true);
            }
            else
            {
                GetSeries seriesParser = new GetSeries(m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName]);
                seriesParser.m_Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnlineParsing_GetSeriesCompleted);
                seriesParser.DoParse();
            }
        }

        void OnlineParsing_GetSeriesCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GetSeriesResults results = (GetSeriesResults)e.Result;

            if (results.listSeries.Count > 0)
            {
                DBTVSeries.Log("Found " + m_SeriesList.Count + " matching names for " + m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName]);
                DBSeries UserChosenSeries = results.listSeries[0];

                SelectSeries userSelection;
                if (results.listSeries.Count > 1)
                {
                    // User has three choices:
                    // 1) Pick a series from the list
                    // 2) Simply skip
                    // 3) Skip and never ask for this series again
                    userSelection =  = new SelectSeries();
                    userSelection.addSeriesToSelection(results.listSeries);
                    userSelection.ShowDialog();
                    UserChosenSeries = userSelection.userChoice;
                }

                if (UserChosenSeries != null) // make sure selection was not cancelled
                {
                    // set the ID on the current series with the one from the chosen one
                    if (userSelection != null && userSelection.neverAskAgain) // user selected to never ask for this again
                        m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cID] = -1; // ID -1 means it will be skipped in the future
                    else
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

            if (m_nCurrentSeriesIndex >= m_SeriesList.Count)
            {
                // we did all our series. Go to the next step, the series data update
                UpdateSeries();
            }
            else
            {
                DBTVSeries.Log("Looking for the episodes of " + m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName]);
                GetEpisodes episodesParser = new GetEpisodes(m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cID]);
                episodesParser.m_Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnlineParsing_GetEpisodesCompleted);
                episodesParser.DoParse();
            }
        }


        void OnlineParsing_GetEpisodesCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GetEpisodesResults results = (GetEpisodesResults)e.Result;

            if (results.listEpisodes.Count > 0)
            {
                DBTVSeries.Log("Found " + results.listEpisodes.Count + " episodes for " + m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName]);
                if (DBOption.GetOptions(DBOption.cFullSeriesRetrieval) == true)
                {
                    // add all online episodes in the local db
                    foreach (DBOnlineEpisode onlineEpisode in results.listEpisodes)
                    {
                        // season if not there yet
                        DBSeason season = new DBSeason(m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName], onlineEpisode[DBOnlineEpisode.cSeasonIndex]);
                        season.Commit();

                        DBOnlineEpisode newOnlineEpisode = new DBOnlineEpisode(m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName], onlineEpisode[DBOnlineEpisode.cSeasonIndex], onlineEpisode[DBOnlineEpisode.cEpisodeIndex]);
                        newOnlineEpisode[DBOnlineEpisode.cID] = onlineEpisode[DBOnlineEpisode.cID];
                        newOnlineEpisode[DBOnlineEpisode.cEpisodeName] = onlineEpisode[DBOnlineEpisode.cEpisodeName];
                        newOnlineEpisode[DBOnlineEpisode.cSeriesID] = m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cID];
                        newOnlineEpisode.Commit();
                    }
                }
                else
                {
                    // look for the episodes for that series, and compare / update the values
                    List<DBEpisode> episodeList = DBEpisode.Get(m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cParsedName], true);

                    foreach (DBOnlineEpisode onlineEpisode in results.listEpisodes)
                    {
                        foreach (DBEpisode localEpisode in episodeList)
                        {
                            if ((int)localEpisode[DBEpisode.cSeasonIndex] == (int)onlineEpisode[DBOnlineEpisode.cSeasonIndex] &&
                                (int)localEpisode[DBEpisode.cEpisodeIndex] == (int)onlineEpisode[DBOnlineEpisode.cEpisodeIndex])
                            {
                                DBTVSeries.Log(localEpisode[DBOnlineEpisode.cID] + " identified");
                                // update data
                                localEpisode[DBOnlineEpisode.cID] = onlineEpisode[DBOnlineEpisode.cID];
                                localEpisode[DBOnlineEpisode.cEpisodeName] = onlineEpisode[DBOnlineEpisode.cEpisodeName];
                                localEpisode[DBOnlineEpisode.cSeriesID] = m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cID];
                                localEpisode.Commit();
                                // remove the localEpisode from the list (we found it)
                                episodeList.Remove(localEpisode);
                                break;
                            }
                        }
                    }
                }
            }

            // process next series
            MatchEpisodes_Next(false);
        }

        void UpdateSeries()
        {
            if (GetSeriesEpisodesCompleted != null) // only if any subscribers exist
            {
                this.GetSeriesEpisodesCompleted.Invoke();
            }

        }

    }
}
