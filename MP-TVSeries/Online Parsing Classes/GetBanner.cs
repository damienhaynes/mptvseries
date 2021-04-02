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
using System.IO;
using System.Linq;
using WindowPlugins.GUITVSeries.FanartTvAPI.DataStructures;
using WindowPlugins.GUITVSeries.Online_Parsing_Classes;
using WindowPlugins.GUITVSeries.TmdbAPI.DataStructures;

namespace WindowPlugins.GUITVSeries
{
    public delegate void NewArtWorkDownloadDoneHandler(string aArtworkFile);

    enum EArtworkStyles
    {
        graphical,
        text,
        blank,
        season,
        seasonwide,
        unknown
    }

    abstract class Artwork<T> : IComparable<T> where T : Artwork<T>
    {
        public string SeriesID = string.Empty;
        public string SeriesName = string.Empty;
        public string OnlinePath = string.Empty;
        public string OnlineThumbPath = string.Empty;
        public string FileName = string.Empty;
        public string Language = string.Empty;
        public double Rating = 0.0;
        public int RatingCount = 0;

        public int CompareTo(T aOther)
        {
            // Sort by:
            // 1. Local Language
            // 2. Score (TMDb factors in number of votes)

            double lThisArtwork;
            double lOtherArtwork; 
            if(DBOption.GetOptions(DBOption.cOverrideLanguage))
            {
                lThisArtwork = this.Language == OnlineAPI.GetLanguageOverride(this.SeriesID) ? 100.0 : 0.0;
                lOtherArtwork = aOther.Language == OnlineAPI.GetLanguageOverride(aOther.SeriesID) ? 100.0 : 0.0;
            }
            else
            { 
                lThisArtwork = this.Language == OnlineAPI.SelectedLanguage ? 100.0 : 0.0;
                lOtherArtwork = aOther.Language == OnlineAPI.SelectedLanguage ? 100.0 : 0.0;
            }
            
            lThisArtwork += this.Rating;
            lOtherArtwork += aOther.Rating;

            return lOtherArtwork.CompareTo(lThisArtwork);
        }
    }

    class WideBannerSeries : Artwork<WideBannerSeries>
    {
        public EArtworkStyles Style = EArtworkStyles.graphical;
    };

    class PosterSeries : Artwork<PosterSeries> {}   

    class PosterSeason : Artwork<PosterSeason>
    {
        public string SeasonIndex = string.Empty;
        public EArtworkStyles Style = EArtworkStyles.season;
    };

    class SeriesBannersMap : IEquatable<SeriesBannersMap>
    {
        public string SeriesID = string.Empty;
        public List<WideBannerSeries> SeriesWideBanners = new List<WideBannerSeries>();
        public List<PosterSeries> SeriesPosters = new List<PosterSeries>();
        public List<PosterSeason> SeasonPosters = new List<PosterSeason>();

        public SeriesBannersMap() {}

        public SeriesBannersMap(string aSeriesID)
        {
            this.SeriesID = aSeriesID;
        }
        
        #region IEquatable<seriesBannersMap> Members

        bool IEquatable<SeriesBannersMap>.Equals(SeriesBannersMap aOther)
        {
            return SeriesID.Equals(aOther.SeriesID);
        }

        #endregion
    }

    class GetBanner
    {
        public event NewArtWorkDownloadDoneHandler BannerDownloadDone;
        public List<SeriesBannersMap> SeriesBannersMap = new List<SeriesBannersMap>();

        public GetBanner(int aSeriesID)
        {
            DoWork(aSeriesID);
        }

        private List<WideBannerSeries> GetWideSeriesBanners(List<FanartTvImage> aImages, int aSeriesID, string aSeriesName)
        {
            var lWideBanners = new List<WideBannerSeries>();

            if (aImages == null) return lWideBanners;

            foreach(FanartTvImage image in aImages)
            {
                string lOnlineFilePath = image.Url.Replace("https://assets.fanart.tv/", "");

                var lWideBanner = new WideBannerSeries
                {
                    Language = image.Language,
                    OnlinePath = lOnlineFilePath,
                    OnlineThumbPath = lOnlineFilePath.Replace("fanart/", "preview/"),
                    Rating = image.Likes,
                    RatingCount = image.Likes,
                    SeriesName = aSeriesName,
                    SeriesID = aSeriesID.ToString(),
                    Style = EArtworkStyles.graphical
                };

                lWideBanners.Add(lWideBanner);
            }

            return lWideBanners;
        }

        private List<PosterSeries> GetPosterSeries(List<TmdbImage> aImages, int aSeriesID, string aSeriesName)
        {
            var lPosters = new List<PosterSeries>();

            foreach(TmdbImage image in aImages)
            {
                var lSeriesPoster = new PosterSeries
                {
                    Language = image.LanguageCode,
                    OnlinePath = "original" + image.FilePath,
                    OnlineThumbPath = "w342" + image.FilePath,
                    Rating = image.Score,
                    RatingCount = image.Votes,
                    SeriesName = aSeriesName,
                    SeriesID = aSeriesID.ToString(),
                };

                lPosters.Add(lSeriesPoster);
            }

            return lPosters;
        }

        private List<PosterSeason> GetPosterSeason(List<TmdbImage> aImages, int aSeriesID, string aSeriesName, int aSeasonNumber)
        {
            var lPosters = new List<PosterSeason>();

            foreach (TmdbImage image in aImages)
            {
                var lSeasonPoster = new PosterSeason
                {
                    Language = image.LanguageCode,
                    OnlinePath = "original" + image.FilePath,
                    OnlineThumbPath = "w342" + image.FilePath,
                    Rating = image.Score,
                    RatingCount = image.Votes,
                    SeriesName = aSeriesName,
                    SeriesID = aSeriesID.ToString(),
                    SeasonIndex = aSeasonNumber.ToString(),
                    Style = EArtworkStyles.season
                };

                lPosters.Add(lSeasonPoster);
            }

            return lPosters;
        }

        private void DoWork(int aSeriesID)
        {
            string lLanguage = OnlineAPI.GetSeriesLanguage(aSeriesID);

            TmdbShowDetail lShowDetail = TmdbAPI.TmdbCache.LoadSeriesFromCache(aSeriesID, lLanguage);

            if (lShowDetail == null) return;

            DBSeries lSeries = Helper.getCorrespondingSeries(aSeriesID);
            if (lSeries == null) return;

            string lSeriesName = lSeries.ToString();

            var lWidebannerSeriesList = new List<WideBannerSeries>();
            var lPosterSeriesList = new List<PosterSeries>();
            var lPosterSeasonList = new List<PosterSeason>();
            var lMap = new SeriesBannersMap
            {
                SeriesID = aSeriesID.ToString()
            };

            #region Series WideBanners
            // Widebanners comes from fanart.tv, lookup by thetvdb.com ID
            // the thetvdb.com may not exist for a series from themoviedb.org
            string lTvDbId = lSeries[DBOnlineSeries.cTvdbId];
            if (!string.IsNullOrEmpty(lTvDbId))
            {
                FanartTvImages lFanartTvImages = FanartTvAPI.FanartTvAPI.GetShowImages(lTvDbId);

                lWidebannerSeriesList = GetWideSeriesBanners(lFanartTvImages?.TvBanners, aSeriesID, lSeriesName);

                // sort by highest rated/language
                lWidebannerSeriesList.Sort();

                // Respect User Limits, exception: if higher rated image or localised image is uploaded online
                int limit = DBOption.GetOptions(DBOption.cArtworkLimitSeriesWideBanners);
                if (limit < lWidebannerSeriesList.Count)
                    lWidebannerSeriesList.RemoveRange(limit, lWidebannerSeriesList.Count - limit);
            }
            lMap.SeriesWideBanners = lWidebannerSeriesList;
            #endregion

            #region Series Posters
            lPosterSeriesList = GetPosterSeries(lShowDetail.Images?.Posters, aSeriesID, lSeriesName );

            // sort by highest rated/language
            lPosterSeriesList.Sort();

            int lImageLimit = DBOption.GetOptions(DBOption.cArtworkLimitSeriesPosters);
            if (lImageLimit < lPosterSeriesList.Count)
                lPosterSeriesList.RemoveRange(lImageLimit, lPosterSeriesList.Count - lImageLimit);
            
            lMap.SeriesPosters = lPosterSeriesList;
            #endregion

            #region Season Posters
            foreach(TmdbSeasonBase season in lShowDetail.Seasons)
            {
                TmdbSeasonDetail lSeasonDetail = TmdbAPI.TmdbCache.LoadSeasonFromCache(aSeriesID, season.SeasonNumber, lLanguage);
                if (lSeasonDetail == null)
                {
                    MPTVSeriesLog.Write($"Failed to get info for {aSeriesID} season {season.SeasonNumber}.");
                    continue;
                }
                
                lPosterSeasonList = GetPosterSeason(lSeasonDetail.Images?.Posters, aSeriesID, lSeriesName, season.SeasonNumber);

                // if the poster count is zero try to get from english language
                //var lSeasons = new List<string>();
                //if ( lPosterSeasonList.Count == 0 && OnlineAPI.GetSeriesLanguage( int.Parse( aSeriesID ) ) != "en" )
                //{
                //    if ( lEnglishBanners == null )
                //    {
                //        lEnglishBanners = OnlineAPI.GetBannerList( Int32.Parse( aSeriesID ), "en" );
                //        if ( lEnglishBanners == null ) return;
                //    }

                //    lPosterSeasonList = GetPosterSeason( lEnglishBanners, aSeriesID, lSeriesName, ref lSeasons );
                //}

                lPosterSeasonList.Sort();

                // we dont support season widebanners
                lPosterSeasonList.RemoveAll(p => p.Style == EArtworkStyles.seasonwide);

                lImageLimit = DBOption.GetOptions(DBOption.cArtworkLimitSeasonPosters);
                if (lImageLimit < lPosterSeasonList.Count)
                    lPosterSeasonList.RemoveRange(lImageLimit, lPosterSeasonList.Count - lImageLimit);

                lMap.SeasonPosters.AddRange(lPosterSeasonList);
            }
            #endregion

            // check if series already in map
            if (SeriesBannersMap.Contains(lMap))
            {
                SeriesBannersMap lSeriesMap = SeriesBannersMap[SeriesBannersMap.IndexOf(lMap)];
                lSeriesMap.SeasonPosters.AddRange(lMap.SeasonPosters);
                lSeriesMap.SeriesWideBanners.AddRange(lMap.SeriesWideBanners);
                lSeriesMap.SeriesPosters.AddRange(lMap.SeriesPosters);
            }
            else
            {
                SeriesBannersMap.Add(lMap);
            }
        }

        public void DownloadBanners(string aOnlineLanguage)
        {
            int lConsecutiveDownloadErrors = 0;
            if (!int.TryParse(DBOption.GetOptions(DBOption.cMaxConsecutiveDownloadErrors).ToString(), out int lMaxConsecutiveDownloadErrors))
            {
                lMaxConsecutiveDownloadErrors = 3;
            }
            
            // now that we have all the paths, download all the files
            foreach (SeriesBannersMap map in SeriesBannersMap)
            {
                #region Series Wide Banners

                var lSeriesWideBanners = new List<WideBannerSeries>(map.SeriesWideBanners);
                var lSeriesWideBannersToKeep = new List<WideBannerSeries>();

                // if localized and english banners exist then only get them
                // if none exist, then get what's left over
                if (lSeriesWideBanners.Exists(b => b.Language == aOnlineLanguage || b.Language == "en" || string.IsNullOrEmpty(b.Language)))
                {
                    lSeriesWideBanners.RemoveAll(b => b.Language != aOnlineLanguage && b.Language != "en" && !string.IsNullOrEmpty(b.Language));
                }

                foreach (WideBannerSeries seriesWideBanner in lSeriesWideBanners)
                {
                    if (lConsecutiveDownloadErrors >= lMaxConsecutiveDownloadErrors)
                    {
                        MPTVSeriesLog.Write("Too many consecutive download errors. Aborting.");
                        return;
                    }

                    // mark the filename with the language
                    string lPath = "graphical/" + Path.GetFileName(seriesWideBanner.OnlinePath);
                    seriesWideBanner.FileName = Helper.CleanLocalPath(seriesWideBanner.SeriesName) + @"\-lang" + seriesWideBanner.Language + "-" + lPath;
                        
                    string lFile = OnlineAPI.DownloadBanner(seriesWideBanner.OnlinePath, Settings.Path.banners, seriesWideBanner.FileName);
                    if (lFile != null && BannerDownloadDone != null)
                    {
                        BannerDownloadDone(lFile);
                        lSeriesWideBannersToKeep.Add(seriesWideBanner);
                        lConsecutiveDownloadErrors = 0;
                    } 
                    else
                    {
                        lConsecutiveDownloadErrors++;
                    }
                }

                map.SeriesWideBanners = lSeriesWideBannersToKeep;

                #endregion

                #region Series Posters

                var lSeriesPosters = new List<PosterSeries>(map.SeriesPosters);
                var lSeriesPostersToKeep = new List<PosterSeries>();

                // if localized and english banners exist then only get them
                // if none exist, then get what's left over
                if (lSeriesPosters.Exists(p => p.Language == aOnlineLanguage || p.Language == "en" || string.IsNullOrEmpty(p.Language)))
                {
                    lSeriesPosters.RemoveAll(p => p.Language != aOnlineLanguage && p.Language != "en" && !string.IsNullOrEmpty(p.Language));
                }

                foreach (PosterSeries seriesPoster in lSeriesPosters)
                {
                    if (lConsecutiveDownloadErrors >= lMaxConsecutiveDownloadErrors)
                    {
                        MPTVSeriesLog.Write("Too many consecutive download errors. Aborting.");
                        return;
                    }

                    // mark the filename with the language
                    string lPath = "posters/" + Path.GetFileName( seriesPoster.OnlinePath );
                    seriesPoster.FileName = Helper.CleanLocalPath(seriesPoster.SeriesName) + @"\-lang" + seriesPoster.Language + "-" + lPath;

                    string lFile = OnlineAPI.DownloadBanner(seriesPoster.OnlinePath, Settings.Path.banners, seriesPoster.FileName);
                    if (lFile != null && BannerDownloadDone != null)
                    {
                        BannerDownloadDone(lFile);
                        lSeriesPostersToKeep.Add(seriesPoster);
                        lConsecutiveDownloadErrors = 0;
                    }
                    else
                    {
                        lConsecutiveDownloadErrors++;
                    }
                }

                map.SeriesPosters = lSeriesPostersToKeep;

                #endregion

                #region Season Posters

                List<DBSeason> lLocalSeasons = DBSeason.Get(new SQLCondition(new DBSeason(), DBSeason.cSeriesID, map.SeriesID, SQLConditionType.Equal), false);

                var lSeasonPosters = new List<PosterSeason>(map.SeasonPosters);
                var lSeasonPostersToKeep = new List<PosterSeason>();

                // if localized and english banners exist then only get them
                // if none exist, then get what's left over
                if (lSeasonPosters.Exists(p => p.Language == aOnlineLanguage || p.Language == "en" || string.IsNullOrEmpty(p.Language)))
                {
                    lSeasonPosters.RemoveAll(p => p.Language != aOnlineLanguage && p.Language != "en" && !string.IsNullOrEmpty(p.Language));
                }

                foreach (PosterSeason seasonPoster in lSeasonPosters)
                {
                    if (lConsecutiveDownloadErrors >= lMaxConsecutiveDownloadErrors)
                    {
                        MPTVSeriesLog.Write("Too many consecutive download errors. Aborting.");
                        return;
                    }

                    // only download season banners if we have online season in database
                    if (!lLocalSeasons.Any(s => s[DBSeason.cIndex] == seasonPoster.SeasonIndex)) continue;

                    string lPath = "seasons/" + Path.GetFileName( seasonPoster.OnlinePath );
                    seasonPoster.FileName = Helper.CleanLocalPath(seasonPoster.SeriesName) + @"\-lang" + seasonPoster.Language + "-" + lPath;
                        
                    string lFile = OnlineAPI.DownloadBanner(seasonPoster.OnlinePath, Settings.Path.banners, seasonPoster.FileName);
                    if (lFile != null && BannerDownloadDone != null)
                    {
                        BannerDownloadDone(lFile);
                        lSeasonPostersToKeep.Add(seasonPoster);
                        lConsecutiveDownloadErrors = 0;
                    }
                    else
                    {
                        lConsecutiveDownloadErrors++;
                    }
                }

                map.SeasonPosters = lSeasonPostersToKeep;

                #endregion
            }
        }
    }
}