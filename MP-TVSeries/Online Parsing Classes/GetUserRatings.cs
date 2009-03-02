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
    class GetUserRatings
    {               
        # region properties
        
        public Dictionary<string, string> EpisodeRating
        {
            get
            {
                return _episoderatings; 
            }
            set
            {
                _episoderatings = value;
            }
        } private Dictionary<string, string> _episoderatings = new Dictionary<string, string>();

        public string SeriesRating
        {
            get 
            {
                return _seriesRating; 
            }
            set
            {
                _seriesRating = value;
            }
        } private string _seriesRating;

        # endregion properties

        public GetUserRatings(string sSeriesID, string sAccountID)
        {
            doWork(sSeriesID, sAccountID);
        }        
        
        public void doWork(String sSeriesID, string sAccountID)
        {            
            XmlNodeList nodeList = null;
            nodeList = Online_Parsing_Classes.OnlineAPI.GetUserRatings(sSeriesID, sAccountID).ChildNodes;

            if (nodeList != null)
            {
                foreach (XmlNode itemNode in nodeList)
                {
                    if (itemNode.Name == "Episode")
                    {    
                        string id = string.Empty;
                        string rating = string.Empty;

                        foreach (XmlNode episodeNode in itemNode)
                        {
                            if (episodeNode.Name == "id")
                                id = episodeNode.InnerText;
                            if (episodeNode.Name == "UserRating")
                                rating = episodeNode.InnerText;
                        }
                        EpisodeRating.Add(id, rating);
                    }
                    else if (itemNode.Name == "Series")
                    {
                        foreach (XmlNode seriesNode in itemNode)
                        {
                            if (seriesNode.Name == "UserRating")                          
                                SeriesRating = seriesNode.InnerText;                                                    
                        }         
                    }
                }
            }
        }
    }
}
