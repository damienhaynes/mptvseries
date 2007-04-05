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
    class UpdateEpisodes
    {
        private long m_nServerTimeStamp = 0;
        private List<DBOnlineEpisode> listEpisodes = new List<DBOnlineEpisode>();
        private List<int> listIncorrectIDs = new List<int>();

        public long ServerTimeStamp
        {
            get { return m_nServerTimeStamp; }
        }

        public List<DBOnlineEpisode> Results
        {
            get { return listEpisodes; }
        }

        public List<int> BadIds
        {
            get { return listIncorrectIDs; }
        }


        public UpdateEpisodes(String sEpisodesIDs, long nUpdateEpisodesTimeStamp)
        {
            if (sEpisodesIDs != String.Empty)
            {
                XmlNodeList nodeList = null;
                nodeList = ZsoriParser.UpdateEpisodes(sEpisodesIDs, nUpdateEpisodesTimeStamp);

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
                                if (propertyNode.Name == "IncorrectID")
                                {
                                    // alert! drop this series, the ID doesn't match anything anymore for some reason
                                    listIncorrectIDs.Add(episode[DBOnlineEpisode.cID]);
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
                                listEpisodes.Add(episode);
                        }
                    }
                }
            }
        }
    }
}
