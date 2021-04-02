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
using System.IO;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    class GetUserRatings
    {
        public class Ratings
        {
            public string CommunityRating { get; set; }
            public string UserRating { get; set; }

            public Ratings(string userRating, string communityRating)
            {
                UserRating = userRating;
                CommunityRating = communityRating;
            }
        }
    
        #region public properties

        public string SeriesCommunityRating { get; set; }
        public string SeriesUserRating { get; set; }

        public Dictionary<string, Ratings> EpisodeRatings {
            get {
                return _episodeRatings;
            }
            set {
                _episodeRatings = value;
            }
        } private Dictionary<string, Ratings> _episodeRatings = new Dictionary<string, Ratings>();

        public Dictionary<string, string> SeriesUserRatings {
            get {
                return _SeriesUserRatings;
            }
            set {
                _SeriesUserRatings = value;
            }
        } private Dictionary<string, string> _SeriesUserRatings = new Dictionary<string, string>();

        public Dictionary<string, string> SeriesCommunityRatings {
            get {
                return _SeriesCommunityRatings;
            }
            set {
                _SeriesCommunityRatings = value;
            }
        } private Dictionary<string, string> _SeriesCommunityRatings = new Dictionary<string, string>();

        #endregion properties

        #region constructor

        public GetUserRatings(string sSeriesID, string sAccountID)
        {
            DoWork(sSeriesID, sAccountID);
        }

        #endregion

        #region private methods

        void DoWork(String sSeriesID, string sAccountID)
        {
            XmlNode node = null;
            string filename = string.Empty;
                        
            if (sSeriesID != null)
            {
                filename = Path.Combine(Settings.GetPath(Settings.Path.config), string.Format(@"Cache\{0}\ratings.xml", sSeriesID));

                // check if we have user rating in cache
               // node = Helper.LoadXmlCache(filename);
            }

            // download ratings
            if (node == null)
            {
                // TODO: Get TMDB User Ratings

                if (node == null) return;

                // check if there is any user ratings
                if (node.SelectSingleNode("/Data/Error") != null)
                {
                    MPTVSeriesLog.Write("User Ratings: " + node.SelectSingleNode("/Data/Error").InnerText, MPTVSeriesLog.LogLevel.Debug);
                    return;
                }

                // save to file cache
                if (sSeriesID != null)
                {
                    //Helper.SaveXmlCache(filename, node);
                }
            }

            try
            {
                if (sSeriesID != null)
                {
                    // download all series and episode ratings
                    GetSeriesAndEpisodeRatings(node);
                }
                else
                {
                    // only update series ratings, not individual episode ratings
                    // quicker b/c we can pull them all in one query
                    GetSeriesRatings(node);
                }
            }
            catch (Exception e)
            {
                MPTVSeriesLog.Write("Error getting User Ratings: {0}", e.Message);
            }
        }

        void GetSeriesAndEpisodeRatings(XmlNode node)
        {
            // Get Series Rating
            SeriesUserRating = node.SelectSingleNode("/Data/Series/UserRating").InnerText;
            SeriesCommunityRating = node.SelectSingleNode("/Data/Series/CommunityRating").InnerText;

            // Get Episode Ratings
            foreach (XmlNode episodeNode in node.SelectNodes("/Data/Episode"))
            {
                string episodeid = episodeNode.SelectSingleNode("id").InnerText;
                string userRating = episodeNode.SelectSingleNode("UserRating").InnerText;
                string communityRating = episodeNode.SelectSingleNode("CommunityRating").InnerText;
                EpisodeRatings.Add(episodeid, new Ratings(userRating, communityRating));
            }
        }

        void GetSeriesRatings(XmlNode node)
        {
            foreach (XmlNode seriesNode in node.SelectNodes("/Data/Series"))
            {
                string seriesid = seriesNode.SelectSingleNode("seriesid").InnerText;
                string userRating = seriesNode.SelectSingleNode("UserRating").InnerText;
                string communityRating = seriesNode.SelectSingleNode("CommunityRating").InnerText;
                SeriesUserRatings.Add(seriesid, userRating);
                SeriesCommunityRatings.Add(seriesid, communityRating);
            }
        }

        #endregion
    }
}
