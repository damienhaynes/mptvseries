using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WindowPlugins.GUITVSeries
{
    class OnlineParse
    {
        List<DBSeries> m_SeriesList = new List<DBSeries>();
        int m_nCurrentSeriesIndex = -1;
        public delegate void GetSeriesEpisodesCompletedHandler(object sender);

        /// <summary>
        /// This will be triggered once all the SeriesAndEpisodeInfo has been parsed completely.
        /// </summary>
        public event GetSeriesEpisodesCompletedHandler GetSeriesEpisodesCompleted;

        public void Start()
        {
            // asynchronous execution: we start with GetSeries, and the execution path goes on through the various worker events
            MatchSeries_Next();
        }

        private void MatchSeries_Next()
        {
            if (m_nCurrentSeriesIndex == -1)
            {
                SQLCondition condition = new SQLCondition(new DBSeries());
                // all series that don't have an onlineID ( = 0)
                condition.Add(DBSeries.cID, 0, true);
                m_SeriesList = DBSeries.Get(condition);
                m_nCurrentSeriesIndex = 0;
            }
            else 
            {
                m_nCurrentSeriesIndex++;
            }
            if (m_nCurrentSeriesIndex >= m_SeriesList.Count)
            {
                // we did all our series. Go to the next step, the episode retrieval
                GetSeriesEpisodes();
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
                DBSeries UserChosenSeries = results.listSeries[0];
                if (results.listSeries.Count > 1)
                {
                    SelectSeries userSelection = new SelectSeries();
                    userSelection.addSeriesToSelection(results.listSeries);
                    userSelection.ShowDialog();
                    UserChosenSeries = userSelection.userChoice;
                }

                if (UserChosenSeries != null) // make sure selection was not cancelled
                {
                    // set the ID on the current series with the one from the chosen one
                    m_SeriesList[m_nCurrentSeriesIndex][DBSeries.cID] = UserChosenSeries[DBSeries.cID];
                    m_SeriesList[m_nCurrentSeriesIndex].Commit();
                }
            }

            // process next entry
             MatchSeries_Next();
        }

        void GetSeriesEpisodes()
        {
            // stop there for now;
            // TODO: need an event system to notify the GUIConfiguration that this process has ended


            // get all series with 
            SQLCondition condition = new SQLCondition(new DBSeries());
            // all series that have an onlineID ( != 0)
            condition.Add(DBSeries.cParsedName, 0, false);
            m_SeriesList = DBSeries.Get(condition);
            m_nCurrentSeriesIndex = 0;

            // blabla do your work and then
            
            this.GetSeriesEpisodesCompleted.Invoke(this);
        }
    }
}
