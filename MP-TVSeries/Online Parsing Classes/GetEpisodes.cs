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

        public GetEpisodes(int nSeriesID)
        {
            m_nSeriesID = nSeriesID;
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
            XmlNodeList nodeList = ZsoriParser.GetEpisodes(m_nSeriesID);
            if (nodeList != null)
            {
                GetEpisodesResults results = new GetEpisodesResults();

                foreach (XmlNode itemNode in nodeList)
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

                e.Result = results;
            }
        }
    }

    class GetEpisodesResults
    {
        public List<DBOnlineEpisode> listEpisodes = new List<DBOnlineEpisode>();
    };
}
