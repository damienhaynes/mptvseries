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
using System.Text;
using System.ComponentModel;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    class GetEpisodes
    {
        //private long m_nServerTimeStamp = 0;
        private List<DBOnlineEpisode> listEpisodes = new List<DBOnlineEpisode>();

        //public long ServerTimeStamp
        //{
        //    get { return m_nServerTimeStamp; }
        //}

        public List<DBOnlineEpisode> Results
        {
            get { return listEpisodes; }
        }

        #region old stuff - safe to remove
        /*
        public GetEpisodes(int nSeriesID, long nGetEpisodesTimeStamp)
        {
            Work(nSeriesID, -1, -1, nGetEpisodesTimeStamp, default(DateTime));
        }

        public GetEpisodes(int nSeriesID)
        {
            Work(nSeriesID, -1, -1, 0, default(DateTime));
        }

        public GetEpisodes(int nSeriesID, int nSeasonIndex, int nEpisodeIndex)
        {
            Work(nSeriesID, nSeasonIndex, nEpisodeIndex, 0, default(DateTime));
        }

        public GetEpisodes(int nSeriesID, DateTime firstAired)
        {
            Work(nSeriesID, -1, -1, 0, firstAired);
        }

        public void Work(int nSeriesID, int nSeasonIndex, int nEpisodeIndex, long nGetEpisodesTimeStamp, DateTime firstAired)
        {
            XmlNodeList nodeList = null;
            string choosenOrdering;

            DBSeries localSeries = DBSeries.Get(nSeriesID, false);
            if (localSeries != null)
                choosenOrdering = DBSeries.Get(nSeriesID, false)[DBOnlineSeries.cChoseEpisodeOrder];
            else
                choosenOrdering = "Aired";

            if (nEpisodeIndex != -1 && nSeasonIndex != -1)
                nodeList = ZsoriParser.GetEpisodes(nSeriesID, nSeasonIndex, nEpisodeIndex, choosenOrdering);
            else if (!firstAired.Equals(default(DateTime)))
                nodeList = ZsoriParser.GetEpisodes(nSeriesID, firstAired, choosenOrdering);
            else
                nodeList = ZsoriParser.GetEpisodes(nSeriesID, nGetEpisodesTimeStamp, choosenOrdering);

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
        }*/
#endregion

        public GetEpisodes(string seriesID)
        {
            int s = 0;
            if(Int32.TryParse(seriesID, out s) && s > 0)
                doWork(s);
        }        
        
        public void doWork(int nSeriesID)
        {            
            XmlNodeList nodeList = null;
            //string choosenOrdering;

            //DBSeries localSeries = DBSeries.Get(nSeriesID, false);
            //if (localSeries != null)
            //    choosenOrdering = DBSeries.Get(nSeriesID, false)[DBOnlineSeries.cChoseEpisodeOrder];
            //else
            //    choosenOrdering = "Aired";

            nodeList = Online_Parsing_Classes.OnlineAPI.UpdateEpisodes(nSeriesID);

            if (nodeList != null)
            {
                foreach (XmlNode itemNode in nodeList)
                {
                    foreach (XmlNode episodeNode in itemNode)
                    {
                        if (episodeNode.Name == "Episode")
                        {
                            DBOnlineEpisode episode = new DBOnlineEpisode();
                            foreach (XmlNode propertyNode in episodeNode.ChildNodes)
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
}
