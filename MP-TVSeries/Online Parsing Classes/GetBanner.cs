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
using System.Globalization;
using System.Xml;
using System.Net;
using System.IO;
using System.Linq;
using WindowPlugins.GUITVSeries.Online_Parsing_Classes;

namespace WindowPlugins.GUITVSeries
{
    public delegate void NewArtWorkDownloadDoneHandler(string artworkFile);

    enum ArtworkStyles
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
        public string FileName = string.Empty;
        public string Language = string.Empty;
        public double Rating = 0.0;
        public int RatingCount = 0;

        public int CompareTo(T other)
        {
            // Sort by:
            // 1. Local Language 
            // 2. Highest Rated
            // 3. Number of Votes

            double thisArtwork;
            double otherArtwork; 
            if(DBOption.GetOptions(DBOption.cOverrideLanguage))
            {
                thisArtwork = this.Language == OnlineAPI.GetLanguageOverride(this.SeriesID) ? 100.0 : 0.0;
                otherArtwork = other.Language == OnlineAPI.GetLanguageOverride(other.SeriesID) ? 100.0 : 0.0;
            }
            else
            { 
                thisArtwork = this.Language == OnlineAPI.SelLanguageAsString ? 100.0 : 0.0;
                otherArtwork = other.Language == OnlineAPI.SelLanguageAsString ? 100.0 : 0.0;
            }

            if (this.Rating == other.Rating)
            {
                thisArtwork += this.RatingCount;
                otherArtwork += other.RatingCount;
            }

            thisArtwork += this.Rating;
            otherArtwork += other.Rating;

            return otherArtwork.CompareTo(thisArtwork);
        }
    }

    class WideBannerSeries : Artwork<WideBannerSeries>
    {
        public ArtworkStyles Style = ArtworkStyles.graphical;
    };

    class PosterSeries : Artwork<PosterSeries> {}   

    class PosterSeason : Artwork<PosterSeason>
    {
        public String SeasonIndex = string.Empty;
        public ArtworkStyles Style = ArtworkStyles.season;
    };

    class SeriesBannersMap : System.IEquatable<SeriesBannersMap>
    {
        public string SeriesID = string.Empty;
        public List<WideBannerSeries> SeriesWideBanners = new List<WideBannerSeries>();
        public List<PosterSeries> SeriesPosters = new List<PosterSeries>();
        public List<PosterSeason> SeasonPosters = new List<PosterSeason>();

        public SeriesBannersMap() {}

        public SeriesBannersMap(string seriesID)
        {
            this.SeriesID = seriesID;
        }

        #region IEquatable<seriesBannersMap> Members

        bool IEquatable<SeriesBannersMap>.Equals(SeriesBannersMap other)
        {
            return SeriesID.Equals(other.SeriesID);
        }

        #endregion
    }

    class GetBanner
    {
        public event NewArtWorkDownloadDoneHandler BannerDownloadDone;
        public List<SeriesBannersMap> SeriesBannersMap = new List<SeriesBannersMap>();
    
        public GetBanner(string seriesID)
        {
            doWork(seriesID);
        }

        private void doWork(string seriesID)
        {
            XmlNode banners = OnlineAPI.getBannerList(Int32.Parse(seriesID));
            if (banners == null) return;

            DBSeries lSeries = Helper.getCorrespondingSeries( Int32.Parse( seriesID ) );
            if (lSeries == null) return;

            string SeriesName = lSeries.ToString();
            List<WideBannerSeries> widebannerSeriesList = new List<WideBannerSeries>();
            List<PosterSeries> posterSeriesList = new List<PosterSeries>();
            List<PosterSeason> posterSeasonList = new List<PosterSeason>();
            SeriesBannersMap map = new SeriesBannersMap();
            map.SeriesID = seriesID;

            #region Series WideBanners
            foreach (XmlNode banner in banners.SelectNodes("/Banners/Banner[BannerType='series']"))
            {
                WideBannerSeries seriesWideBanners = new WideBannerSeries();
                
                seriesWideBanners.Language = banner.SelectSingleNode("Language").InnerText;
                seriesWideBanners.OnlinePath = banner.SelectSingleNode("BannerPath").InnerText;
                seriesWideBanners.SeriesName = SeriesName;

                try
                {
                    seriesWideBanners.Style = (ArtworkStyles)Enum.Parse(typeof(ArtworkStyles), banner.SelectSingleNode("BannerType2").InnerText, true);
                }
                catch
                {
                    // maybe a new style introduced
                    seriesWideBanners.Style = ArtworkStyles.unknown;
                }

                if (!string.IsNullOrEmpty(banner.SelectSingleNode("Rating").InnerText))
                {
                    double rating = double.Parse(banner.SelectSingleNode("Rating").InnerText, NumberStyles.Any, NumberFormatInfo.InvariantInfo);
                    seriesWideBanners.Rating = Math.Round(rating, 1, MidpointRounding.AwayFromZero);
                }

                if (!string.IsNullOrEmpty(banner.SelectSingleNode("RatingCount").InnerText))
                    seriesWideBanners.RatingCount = int.Parse(banner.SelectSingleNode("RatingCount").InnerText);

                seriesWideBanners.SeriesID = seriesID;
                widebannerSeriesList.Add(seriesWideBanners);
            }
            // sort by highest rated
            widebannerSeriesList.Sort();

            // if the banner count is zero, try to get from the series record
            if ( widebannerSeriesList.Count == 0 && !string.IsNullOrEmpty(lSeries[DBOnlineSeries.cBanner]))
            {
                var seriesWideBanner = new WideBannerSeries();

                seriesWideBanner.Language = "en";
                seriesWideBanner.OnlinePath = lSeries[DBOnlineSeries.cBanner];
                seriesWideBanner.SeriesName = SeriesName;
                seriesWideBanner.Style = ArtworkStyles.graphical;
                widebannerSeriesList.Add( seriesWideBanner );
            }

            // remove banners of no interest
            if (!DBOption.GetOptions(DBOption.cGetTextBanners))
            {
                widebannerSeriesList.RemoveAll(b => b.Style == ArtworkStyles.text);
            }
            if (!DBOption.GetOptions(DBOption.cGetBlankBanners))
            {
                widebannerSeriesList.RemoveAll(b => b.Style == ArtworkStyles.blank);
            }

            // Respect User Limits, exception: if higher rated image or localised image is uploaded online
            int limit = DBOption.GetOptions(DBOption.cArtworkLimitSeriesWideBanners);
            if (limit < widebannerSeriesList.Count)
                widebannerSeriesList.RemoveRange(limit, widebannerSeriesList.Count - limit);
            
            map.SeriesWideBanners = widebannerSeriesList;
            #endregion

            #region Series Posters
            foreach (XmlNode banner in banners.SelectNodes("/Banners/Banner[BannerType='poster']"))
            {
                PosterSeries seriesPoster = new PosterSeries();

                seriesPoster.Language = banner.SelectSingleNode("Language").InnerText;
                seriesPoster.OnlinePath = banner.SelectSingleNode("BannerPath").InnerText;
                seriesPoster.SeriesName = SeriesName;

                if (!string.IsNullOrEmpty(banner.SelectSingleNode("Rating").InnerText))
                {
                    double rating = double.Parse(banner.SelectSingleNode("Rating").InnerText, NumberStyles.Any, NumberFormatInfo.InvariantInfo);
                    seriesPoster.Rating = Math.Round(rating, 1, MidpointRounding.AwayFromZero);
                }

                if (!string.IsNullOrEmpty(banner.SelectSingleNode("RatingCount").InnerText))
                    seriesPoster.RatingCount = int.Parse(banner.SelectSingleNode("RatingCount").InnerText);

                seriesPoster.SeriesID = seriesID;
                posterSeriesList.Add(seriesPoster);
            }

            posterSeriesList.Sort();

            // if the poster count is zero, try to get from the series record
            if ( posterSeasonList.Count == 0 && !string.IsNullOrEmpty( lSeries[DBOnlineSeries.cPoster] ) )
            {
                var seriesPoster = new PosterSeries();

                seriesPoster.Language = "en";
                seriesPoster.OnlinePath = lSeries[DBOnlineSeries.cPoster];
                seriesPoster.SeriesName = SeriesName;
                posterSeriesList.Add( seriesPoster );
            }

            limit = DBOption.GetOptions(DBOption.cArtworkLimitSeriesPosters);
            if (limit < posterSeriesList.Count)
                posterSeriesList.RemoveRange(limit, posterSeriesList.Count - limit);
            
            map.SeriesPosters = posterSeriesList;
            #endregion

            #region Season Posters
            List<string> seasons = new List<string>();
            foreach (XmlNode banner in banners.SelectNodes("/Banners/Banner[BannerType='season']"))
            {
                PosterSeason seasonPoster = new PosterSeason();

                seasonPoster.Language = banner.SelectSingleNode("Language").InnerText;
                seasonPoster.OnlinePath = banner.SelectSingleNode("BannerPath").InnerText;
                seasonPoster.SeasonIndex = banner.SelectSingleNode("Season").InnerText;
                seasonPoster.SeriesName = SeriesName;

                try
                {
                    seasonPoster.Style = (ArtworkStyles)Enum.Parse(typeof(ArtworkStyles), banner.SelectSingleNode("BannerType2").InnerText, true);
                }
                catch
                {
                    // maybe a new style introduced
                    seasonPoster.Style = ArtworkStyles.unknown;
                }

                if (!string.IsNullOrEmpty(banner.SelectSingleNode("Rating").InnerText))
                {
                    double rating = double.Parse(banner.SelectSingleNode("Rating").InnerText, NumberStyles.Any, NumberFormatInfo.InvariantInfo);
                    seasonPoster.Rating = Math.Round(rating, 1, MidpointRounding.AwayFromZero);
                }

                if (!string.IsNullOrEmpty(banner.SelectSingleNode("RatingCount").InnerText))
                    seasonPoster.RatingCount = int.Parse(banner.SelectSingleNode("RatingCount").InnerText);

                if (!seasons.Contains(seasonPoster.SeasonIndex))
                    seasons.Add(seasonPoster.SeasonIndex);

                seasonPoster.SeriesID = seriesID;
                posterSeasonList.Add(seasonPoster);
            }

            posterSeasonList.Sort();
            
            // we dont support season widebanners
            posterSeasonList.RemoveAll(p => p.Style == ArtworkStyles.seasonwide);            

            limit = DBOption.GetOptions(DBOption.cArtworkLimitSeasonPosters);
            List<PosterSeason> posterSeasonListTemp = new List<PosterSeason>(posterSeasonList);

            foreach (string season in seasons)
            {
                int count = 0;
                foreach (PosterSeason pSeason in posterSeasonListTemp)
                {
                    if (season == pSeason.SeasonIndex)
                    {
                        count++;
                        if (limit < count) posterSeasonList.Remove(pSeason);
                    }
                }
            }

            map.SeasonPosters = posterSeasonList;
            #endregion

            // series already in?
            if (SeriesBannersMap.Contains(map))
            {
                SeriesBannersMap seriesMap = SeriesBannersMap[SeriesBannersMap.IndexOf(map)];
                seriesMap.SeasonPosters.AddRange(map.SeasonPosters);
                seriesMap.SeriesWideBanners.AddRange(map.SeriesWideBanners);
                seriesMap.SeriesPosters.AddRange(map.SeriesPosters);
            }
            else
            {
                SeriesBannersMap.Add(map);
            }
        }

        public void DownloadBanners(string onlineLanguage)
        {
            int maxConsecutiveDownloadErrors;
            var consecutiveDownloadErrors = 0;
            if (!int.TryParse(DBOption.GetOptions(DBOption.cMaxConsecutiveDownloadErrors).ToString(), out maxConsecutiveDownloadErrors))
            {
                maxConsecutiveDownloadErrors = 3;
            }
            
            // now that we have all the paths, download all the files
            foreach (SeriesBannersMap map in SeriesBannersMap)
            {
                #region Series Wide Banners

                var seriesWideBanners = new List<WideBannerSeries>(map.SeriesWideBanners);
                var seriesWideBannersToKeep = new List<WideBannerSeries>();

                // if localized and english banners exist then only get them
                // if none exist, then get what's left over
                if (seriesWideBanners.Exists(b => b.Language == onlineLanguage || b.Language == "en" || b.Language == string.Empty))
                {
                    seriesWideBanners.RemoveAll(b => b.Language != onlineLanguage && b.Language != "en" && b.Language != string.Empty);
                }

                foreach (var seriesWideBanner in seriesWideBanners)
                {
                    if (consecutiveDownloadErrors >= maxConsecutiveDownloadErrors)
                    {
                        MPTVSeriesLog.Write("Too many consecutive download errors. Aborting.");
                        return;
                    }

                    // mark the filename with the language
                    seriesWideBanner.FileName = Helper.cleanLocalPath(seriesWideBanner.SeriesName) + @"\-lang" + seriesWideBanner.Language + "-" + seriesWideBanner.OnlinePath;
                        
                    string file = OnlineAPI.DownloadBanner(seriesWideBanner.OnlinePath, Settings.Path.banners, seriesWideBanner.FileName);
                    if (BannerDownloadDone != null)
                    {
                        BannerDownloadDone(file);
                        seriesWideBannersToKeep.Add(seriesWideBanner);
                        consecutiveDownloadErrors = 0;
                    } 
                    else
                    {
                        consecutiveDownloadErrors++;
                    }
                }

                map.SeriesWideBanners = seriesWideBannersToKeep;

                #endregion

                #region Series Posters

                var seriesPosters = new List<PosterSeries>(map.SeriesPosters);
                var seriesPostersToKeep = new List<PosterSeries>();

                // if localized and english banners exist then only get them
                // if none exist, then get what's left over
                if (seriesPosters.Exists(p => p.Language == onlineLanguage || p.Language == "en" || p.Language == string.Empty))
                {
                    seriesPosters.RemoveAll(p => p.Language != onlineLanguage && p.Language != "en" && p.Language != string.Empty);
                }

                foreach (var seriesPoster in seriesPosters)
                {
                    if (consecutiveDownloadErrors >= maxConsecutiveDownloadErrors)
                    {
                        MPTVSeriesLog.Write("Too many consecutive download errors. Aborting.");
                        return;
                    }

                    // mark the filename with the language
                    seriesPoster.FileName = Helper.cleanLocalPath(seriesPoster.SeriesName) + @"\-lang" + seriesPoster.Language + "-" + seriesPoster.OnlinePath;                        
                    string file = OnlineAPI.DownloadBanner(seriesPoster.OnlinePath, Settings.Path.banners, seriesPoster.FileName);
                    if (BannerDownloadDone != null)
                    {
                        BannerDownloadDone(file);
                        seriesPostersToKeep.Add(seriesPoster);
                        consecutiveDownloadErrors = 0;
                    }
                    else
                    {
                        consecutiveDownloadErrors++;
                    }
                }

                map.SeriesPosters = seriesPostersToKeep;

                #endregion

                #region Season Posters

                List<DBSeason> localSeasons = DBSeason.Get(new SQLCondition(new DBSeason(), DBSeason.cSeriesID, map.SeriesID, SQLConditionType.Equal), false);

                var seasonPosters = new List<PosterSeason>(map.SeasonPosters);
                var seasonPostersToKeep = new List<PosterSeason>();

                // if localized and english banners exist then only get them
                // if none exist, then get what's left over
                if (seasonPosters.Exists(p => p.Language == onlineLanguage || p.Language == "en" || p.Language == string.Empty))
                {
                    seasonPosters.RemoveAll(p => p.Language != onlineLanguage && p.Language != "en" && p.Language != string.Empty);
                }

                foreach (var seasonPoster in seasonPosters)
                {
                    if (consecutiveDownloadErrors >= maxConsecutiveDownloadErrors)
                    {
                        MPTVSeriesLog.Write("Too many consecutive download errors. Aborting.");
                        return;
                    }

                    // only download season banners if we have online season in database
                    if (!localSeasons.Any(s => s[DBSeason.cIndex] == seasonPoster.SeasonIndex)) continue;

                    seasonPoster.FileName = Helper.cleanLocalPath(seasonPoster.SeriesName) + @"\-lang" + seasonPoster.Language + "-" + seasonPoster.OnlinePath;
                        
                    string file = OnlineAPI.DownloadBanner(seasonPoster.OnlinePath, Settings.Path.banners, seasonPoster.FileName);
                    if (BannerDownloadDone != null)
                    {
                        BannerDownloadDone(file);
                        seasonPostersToKeep.Add(seasonPoster);
                        consecutiveDownloadErrors = 0;
                    }
                    else
                    {
                        consecutiveDownloadErrors++;
                    }
                }

                map.SeasonPosters = seasonPostersToKeep;

                #endregion
            }
        }
    }

    class GetFanart
    {
        List<DBFanart> _fanart = new List<DBFanart>();
        public List<DBFanart> Fanart { get { return _fanart; } }
        
        public GetFanart(int SeriesID)
        {
            XmlNode node = OnlineAPI.getBannerList(SeriesID);
            if (node == null) return;

            foreach (XmlNode fanartNode in node.SelectNodes("/Banners/Banner[BannerType='fanart']"))
            {
                DBFanart dbf = new DBFanart();
                foreach (XmlNode propertyNode in fanartNode.ChildNodes)
                {
                    try
                    {
                        dbf[propertyNode.Name] = propertyNode.InnerText;
                    }
                    catch (Exception ex)
                    { 
                        MPTVSeriesLog.Write("Error adding Fanart Property to DBEntry: " + propertyNode.Name + " - " + ex.Message);
                    }
                }

                // Sync local files with database
                string localPath = dbf[DBFanart.cBannerPath];
                localPath = localPath.Replace("/", @"\");
                string fanart = Helper.PathCombine(Settings.GetPath(Settings.Path.fanart), localPath);
                if (File.Exists(fanart))
                    dbf[DBFanart.cLocalPath] = localPath;
                else
                    dbf[DBFanart.cLocalPath] = string.Empty;

                dbf[DBFanart.cSeriesID] = SeriesID;
                _fanart.Add(dbf);                
            }
        }
    }    
}
