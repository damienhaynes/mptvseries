using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    class GetEpisodes
    {
        public BackgroundWorker m_Worker = new BackgroundWorker();
        private int m_nSeriesID;
        private int m_nSeasonIndex = -1;
        private int m_nEpisodeIndex = -1;
        private long m_nGetEpisodesTimeStamp = 0;

        public GetEpisodes(int nSeriesID, long nGetEpisodesTimeStamp)
        {
            m_nSeriesID = nSeriesID;
            m_nGetEpisodesTimeStamp = nGetEpisodesTimeStamp;
            m_Worker.WorkerReportsProgress = true;
            m_Worker.WorkerSupportsCancellation = true;
            m_Worker.DoWork += new DoWorkEventHandler(worker_DoWork);
        }

        public GetEpisodes(int nSeriesID, int nSeasonIndex, int nEpisodeIndex)
        {
            m_nSeriesID = nSeriesID;
            m_nSeasonIndex = nSeasonIndex;
            m_nEpisodeIndex = nEpisodeIndex;
            m_Worker.WorkerReportsProgress = true;
            m_Worker.WorkerSupportsCancellation = true;
            m_Worker.DoWork += new DoWorkEventHandler(worker_DoWork);

        }

        public void DoParse()
        {
            m_Worker.RunWorkerAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            XmlNodeList nodeList = null;
            if (m_nEpisodeIndex != -1 && m_nSeasonIndex != -1)
                nodeList = ZsoriParser.GetEpisodes(m_nSeriesID, m_nSeasonIndex, m_nEpisodeIndex);
            else
                nodeList = ZsoriParser.GetEpisodes(m_nSeriesID, m_nGetEpisodesTimeStamp);

            if (nodeList != null)
            {
                GetEpisodesResults results = new GetEpisodesResults();

                foreach (XmlNode itemNode in nodeList)
                {
                    // first return item SHOULD ALWAYS be the sync time (hope so at least!)
                    if (itemNode.ChildNodes[0].Name == "SyncTime")
                    {
                        results.m_nServerTimeStamp = Convert.ToInt64(itemNode.ChildNodes[0].InnerText);
                    }
                    else
                    {
                        DBOnlineEpisode episode = new DBOnlineEpisode();
                        foreach (XmlNode propertyNode in itemNode.ChildNodes)
                        {
                            if (DBOnlineEpisode.s_OnlineToFieldMap.ContainsKey(propertyNode.Name))
                                episode[DBOnlineEpisode.s_OnlineToFieldMap[propertyNode.Name]] = propertyNode.InnerText;
                            else
                            {
                                // we don't know that field, add it to the series table
                                episode.AddColumn(propertyNode.Name, new DBField(DBField.cTypeString));
                                episode[propertyNode.Name] = propertyNode.InnerText;
                            }
                        }
                        results.listEpisodes.Add(episode);
                    }
                }

                e.Result = results;
            }
        }
    }

    class GetEpisodesResults
    {
        public long m_nServerTimeStamp = 0;
        public List<DBOnlineEpisode> listEpisodes = new List<DBOnlineEpisode>();
    };
}
