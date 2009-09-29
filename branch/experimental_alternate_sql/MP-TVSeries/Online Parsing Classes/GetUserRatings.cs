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
        /*public class Ratings {
            public string CommunityRating { get; set; }                
            public string UserRating { get; set; }

            public Ratings(string userRating, string communityRating) {
                UserRating = userRating;
                CommunityRating = communityRating;
            }
        }*/
    
        # region properties
        
        public Dictionary<string, string> EpisodeCommunityRatings {
            get {
                return _episodeCommunityRatings;
            }
            set {
                _episodeCommunityRatings = value;
            }
        } private Dictionary<string, string> _episodeCommunityRatings = new Dictionary<string, string>();

        public Dictionary<string, string> EpisodeUserRatings {
            get {
                return _episodeUserRatings;
            }
            set {
                _episodeUserRatings = value;
            }
        } private Dictionary<string, string> _episodeUserRatings = new Dictionary<string, string>();

        /*public Dictionary<string, Ratings> EpisodeRatings {
            get {
                return _episodeRatings;
            }
            set {
                _episodeRatings = value;
            }
        } private Dictionary<string, Ratings> _episodeRatings = new Dictionary<string, Ratings>();*/

        public string SeriesCommunityRating { get; set; }            
        public string SeriesUserRating { get; set; }

        public Dictionary<string, string> AllSeriesUserRatings {
            get {
                return _allSeriesUserRatings;
            }
            set {
                _allSeriesUserRatings = value;
            }
        } private Dictionary<string, string> _allSeriesUserRatings = new Dictionary<string, string>();

        public Dictionary<string, string> AllSeriesCommunityRatings {
            get {
                return _allSeriesCommunityRatings;
            }
            set {
                _allSeriesCommunityRatings = value;
            }
        } private Dictionary<string, string> _allSeriesCommunityRatings = new Dictionary<string, string>();

        # endregion properties

        public GetUserRatings(string sSeriesID, string sAccountID)
        {
            doWork(sSeriesID, sAccountID);
        }        
        
        public void doWork(String sSeriesID, string sAccountID)
        {            
            XmlNode node = Online_Parsing_Classes.OnlineAPI.GetUserRatings(sSeriesID, sAccountID);        

            if (node != null)
            {
                if (sSeriesID != null) {
                    foreach (XmlNode itemNode in node.ChildNodes) {
                        if (itemNode.Name == "Episode") {
                            string id = string.Empty;
                            string userRating = string.Empty;
                            string communityRating = string.Empty;

                            foreach (XmlNode episodeNode in itemNode) {
                                if (episodeNode.Name == "id")
                                    id = episodeNode.InnerText;
                                if (episodeNode.Name == "UserRating")
                                    userRating = episodeNode.InnerText;
                                if (episodeNode.Name == "CommunityRating")
                                    communityRating = episodeNode.InnerText;
                            }
                            EpisodeUserRatings.Add(id, userRating);
                            EpisodeCommunityRatings.Add(id, communityRating);
                            //EpisodeRatings.Add(id, new Ratings(userRating, communityRating));
                        }
                        else if (itemNode.Name == "Series") {
                            foreach (XmlNode seriesNode in itemNode) {
                                if (seriesNode.Name == "UserRating")
                                    SeriesUserRating = seriesNode.InnerText;
                                if (seriesNode.Name == "CommunityRating")
                                    SeriesCommunityRating = seriesNode.InnerText;
                            }
                        }
                    }
                }
                else {
                    //only update series ratings, not individual episode ratings (quicker b/c we can pull them all in one query
                    foreach (XmlNode itemNode in node.ChildNodes) {
                        if (itemNode.Name == "Series") {
                            string id = string.Empty;
                            string userRating = string.Empty;
                            string communityRating = string.Empty;

                            foreach (XmlNode seriesNode in itemNode) {
                                if (seriesNode.Name == "seriesid")
                                    id = seriesNode.InnerText;
                                if (seriesNode.Name == "UserRating")
                                    userRating = seriesNode.InnerText;
                                if (seriesNode.Name == "CommunityRating")
                                    communityRating = seriesNode.InnerText;
                            }
                            AllSeriesUserRatings.Add(id, userRating);
                            AllSeriesCommunityRatings.Add(id, communityRating);
                        }
                    }
                }
            }
        }
    }
}
