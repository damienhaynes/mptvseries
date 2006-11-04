using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    class UpdateEpisodes
    {
        public BackgroundWorker m_Worker = new BackgroundWorker();
        private String m_sEpisodesIDs;
        private long m_nUpdateEpisodesTimeStamp;

        public UpdateEpisodes(String sEpisodesIDs, long nUpdateEpisodesTimeStamp)
        {
            m_sEpisodesIDs = sEpisodesIDs;
            m_nUpdateEpisodesTimeStamp = nUpdateEpisodesTimeStamp;
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
            nodeList = ZsoriParser.UpdateEpisodes(m_sEpisodesIDs, m_nUpdateEpisodesTimeStamp);

            if (nodeList != null)
            {
                UpdateEpisodesResults results = new UpdateEpisodesResults();
                
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
                            if (propertyNode.Name == "IncorrectID")
                            {
                                // alert! drop this series, the ID doesn't match anything anymore for some reason
                                results.listIncorrectIDs.Add(episode[DBOnlineEpisode.cID]);
                                episode = null;
                                break;
                            }
                            else
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
                        }
                        if (episode != null)
                            results.listEpisodes.Add(episode);
                    }
                }

                e.Result = results;
            }
        }
    }

    class UpdateEpisodesResults
    {
        public long m_nServerTimeStamp = 0;
        public List<DBOnlineEpisode> listEpisodes = new List<DBOnlineEpisode>();
        public List<int> listIncorrectIDs = new List<int>();
    };
}
