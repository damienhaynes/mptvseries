#region GNU license
// MP-TVSeries - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2021
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
using System.Linq;
using System.Xml;
using WindowPlugins.GUITVSeries.TmdbAPI;
using WindowPlugins.GUITVSeries.TmdbAPI.DataStructures;

namespace WindowPlugins.GUITVSeries
{
    class UpdateSeries
    {
        private readonly List<String> mSeriesIDs = null;
        private readonly long mServerTimeStamp = 0;
        private List<DBOnlineSeries> mSeriesList = new List<DBOnlineSeries>();
        private readonly List<int> mIncorrectIdsList = new List<int>();

        public long ServerTimeStamp
        {
            get { return mServerTimeStamp; }
        }

        public List<DBOnlineSeries> Results
        {
            get
            {
                if (mSeriesList == null)
                    mSeriesList = new List<DBOnlineSeries>(ResultsLazy);
                return mSeriesList;
            }
        }

        /// <summary>
        /// Lazily Evaluates
        /// </summary>
        public IEnumerable<DBOnlineSeries> ResultsLazy
        {
            get
            {
                foreach (string id in mSeriesIDs)
                {
                    var results = Work(id);
                    foreach (var r in results)
                    {
                        if (r != null && r[DBOnlineSeries.cID] > 0)
                            yield return r;
                    }
                }
            }
        }

        public List<int> BadIds
        {
            get { return mIncorrectIdsList; }
        }

        public UpdateSeries(String aSeriesID)
        {
            mSeriesList = Work(aSeriesID).ToList();
        }

        public UpdateSeries(List<String> aSeriesIDs)
        {
            this.mSeriesIDs = aSeriesIDs;            
        }

        public UpdateSeries(String aSeriesID, String aLanguageID, bool aOverride = false )
        {
            mSeriesList = Work(aSeriesID, aLanguageID, aOverride).ToList();
        }

        private IEnumerable<DBOnlineSeries> Work(String sSeriesID)
        {
            return Work(sSeriesID, "");
        }

        private IEnumerable<DBOnlineSeries> Work(String aSeriesID, String aLanguageID, bool aOverride = false)
        {
            if (aSeriesID.Length > 0)
            {                
                if (int.TryParse(aSeriesID, out int lSeriesId))
                {
                    MPTVSeriesLog.Write(string.Format("Retrieving updated Metadata for series {0}", Helper.GetCorrespondingSeries(lSeriesId)), MPTVSeriesLog.LogLevel.Debug);
                }

                TmdbShowDetail lShowDetail = null;
                if (String.IsNullOrEmpty(aLanguageID))
                {
                    lShowDetail = TmdbAPI.TmdbAPI.GetShowDetail(lSeriesId);
                }
                else
                {
                    lShowDetail = TmdbAPI.TmdbAPI.GetShowDetail(lSeriesId, aLanguageID);
                }

                if (lShowDetail != null)
                {
                    var lSeries = new DBOnlineSeries();

                    lSeries[DBOnlineSeries.cID] = lShowDetail.Id;
                    lSeries[DBOnlineSeries.cTmdbId] = lShowDetail.Id;
                    
                    lSeries[DBOnlineSeries.cActors] = string.Join("|", lShowDetail.Credits?.Cast?.Select(c => c.Name));
                    //lSeries[DBOnlineSeries.cAirsDay] = string.Empty;
                    //lSeries[DBOnlineSeries.cAirsTime] = string.Empty;
                    //lSeries[DBOnlineSeries.cAliasNames] = string.Empty; // could append 'alternative_titles'
                    lSeries[DBOnlineSeries.cContentRating] = lShowDetail.ContentRatings.Results.FirstOrDefault(r => r.Code == "US")?.Rating; // Allow user to choose which country
                    lSeries[DBOnlineSeries.cCountry] = lShowDetail.OriginCountries.FirstOrDefault(); // create a split field
                    lSeries[DBOnlineSeries.cEpisodeOrders] = "Aired|"; // will need need review of API
                    //lSeries[DBOnlineSeries.cEpisodeSortOrder] = string.Empty;
                    lSeries[DBOnlineSeries.cFanart] = lShowDetail.BackdropPath; // useless
                    lSeries[DBOnlineSeries.cFirstAired] = lShowDetail.FirstAirDate;
                    lSeries[DBOnlineSeries.cGenre] = string.Join("|", lShowDetail.Genres?.Select(g => g.Name));
                    lSeries[DBOnlineSeries.cIMDBID] = lShowDetail.ExternalIds?.ImdbId;
                    //lSeries[DBOnlineSeries.cIsOnlineFavourite] = string.Empty; // could append 'account_states' endpoint
                    lSeries[DBOnlineSeries.cLanguage] = lShowDetail.Languages.FirstOrDefault(); // create a split field
                    lSeries[DBOnlineSeries.cLastEpisodeAirDate] = lShowDetail.LastEpisodeToAir?.AirDate;
                    lSeries[DBOnlineSeries.cLastUpdatedDetail] = string.Empty;
                    lSeries[DBOnlineSeries.cNetwork] = lShowDetail.Networks.FirstOrDefault()?.Name; // create a split field 
                    lSeries[DBOnlineSeries.cNetworkID] = lShowDetail.Networks.FirstOrDefault()?.Id; // could be removed or create new table
                    lSeries[DBOnlineSeries.cOriginalName] = lShowDetail.OriginalName;
                    lSeries[DBOnlineSeries.cPoster] = lShowDetail.PosterPath; // useless
                    lSeries[DBOnlineSeries.cPrettyName] = lShowDetail.Name;
                    lSeries[DBOnlineSeries.cRating] = lShowDetail.Score;
                    lSeries[DBOnlineSeries.cRatingCount] = lShowDetail.Votes;
                    lSeries[DBOnlineSeries.cSeriesID] = lShowDetail.Id; // redundant
                    lSeries[DBOnlineSeries.cStatus] = lShowDetail.Status; // "Returning Series" == "Continuing"
                    lSeries[DBOnlineSeries.cSummary] = lShowDetail.Overview;
                    lSeries[DBOnlineSeries.cRuntime] = lShowDetail.EpisodeRuntimes.FirstOrDefault(); // could create a split field
                    lSeries[DBOnlineSeries.cCreators] = string.Join("|", lShowDetail.Creators?.Select(c => c.Name));
                    lSeries[DBOnlineSeries.cHomepage] = lShowDetail.Homepage;
                    lSeries[DBOnlineSeries.cProductionCompanies] = string.Join("|", lShowDetail.ProductionCompanies?.Select(c => c.Name));
                    lSeries[DBOnlineSeries.cProductionCountries] = string.Join("|", lShowDetail.ProductionCountries?.Select(c => c.Name));
                    lSeries[DBOnlineSeries.cTagline] = lShowDetail.Tagline;
                    lSeries[DBOnlineSeries.cType] = lShowDetail.Type;
                    lSeries[DBOnlineSeries.cSpokenLanguages] = string.Join("|", lShowDetail.SpokenLanaguages?.Select(l => l.EnglishName));
                    lSeries[DBOnlineSeries.cOnlineSeasonCount] = lShowDetail.SeasonCount;
                    lSeries[DBOnlineSeries.cOnlineEpisodeCount] = lShowDetail.EpisodeCount;
                    lSeries[DBOnlineSeries.cTvdbId] = lShowDetail.ExternalIds?.TvdbId;
                    lSeries[DBOnlineSeries.cOnlineSeasonsAvailable] = string.Join(",", lShowDetail.Seasons?.Select(s => s.SeasonNumber)); // for query of season details so we can get every episode for series

                    mSeriesList.Add(lSeries);
                    yield return lSeries;
                }
            }
        }
    }
}

