using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    class GetEpisodes
    {
        private long m_nServerTimeStamp = 0;
        private List<DBOnlineEpisode> listEpisodes = new List<DBOnlineEpisode>();

        public long ServerTimeStamp
        {
            get { return m_nServerTimeStamp; }
        }

        public List<DBOnlineEpisode> Results
        {
            get { return listEpisodes; }
        }

        public GetEpisodes(int nSeriesID, long nGetEpisodesTimeStamp)
        {
            Work(nSeriesID, -1, -1, nGetEpisodesTimeStamp);
        }

        public GetEpisodes(int nSeriesID, int nSeasonIndex, int nEpisodeIndex)
        {
            Work(nSeriesID, nSeasonIndex, nEpisodeIndex, 0);
        }

        private void Work(int nSeriesID, int nSeasonIndex, int nEpisodeIndex, long nGetEpisodesTimeStamp)
        {
            XmlNodeList nodeList = null;
            if (nEpisodeIndex != -1 && nSeasonIndex != -1)
                nodeList = ZsoriParser.GetEpisodes(nSeriesID, nSeasonIndex, nEpisodeIndex);
            else
                nodeList = ZsoriParser.GetEpisodes(nSeriesID, nGetEpisodesTimeStamp);

            if (nodeList != null)
            {
                foreach (XmlNode itemNode in nodeList)
                {
                    // first return item SHOULD ALWAYS be the sync time (hope so at least!)
                    if (itemNode.ChildNodes[0].Name == "SyncTime")
                    {
                        m_nServerTimeStamp = Convert.ToInt64(itemNode.ChildNodes[0].InnerText);
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
                        listEpisodes.Add(episode);
                    }
                }
            }
        }
    }
}
