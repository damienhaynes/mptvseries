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
        private List<DBOnlineEpisode> listEpisodes = new List<DBOnlineEpisode>();

        public List<DBOnlineEpisode> Results
        {
            get { return listEpisodes; }
        }

        public GetEpisodes(string seriesID)
        {
            int s = 0;
            if(Int32.TryParse(seriesID, out s) && s > 0)
                doWork(s);
        }        
        
        public void doWork(int nSeriesID)
        {            
            XmlNodeList nodeList = null;
    
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
