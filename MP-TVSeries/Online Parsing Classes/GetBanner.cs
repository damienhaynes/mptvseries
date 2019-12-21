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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
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
        public string OnlineThumbPath = string.Empty;
        public string FileName = string.Empty;
        public string Language = string.Empty;
        public double Rating = 0.0;
        public int RatingCount = 0;

        public int CompareTo(T other)
        {
            // Sort by:
            // 1. Local Language
            // 2. Number of Votes (Ratings no longer exist, and is based on a favourate/like system)

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
            
            thisArtwork += this.RatingCount;
            otherArtwork += other.RatingCount;

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

    class SeriesBannersMap : IEquatable<SeriesBannersMap>
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

        public GetBanner( int seriesID )
        {
            doWork( seriesID.ToString() );
        }

        public GetBanner(string seriesID)
        {
            doWork(seriesID);
        }

        private List<WideBannerSeries> GetWideSeriesBanners(XmlNode aNode, string aSeriesID, string aSeriesName )
        {
            var lWideBanners = new List<WideBannerSeries>();

            foreach ( XmlNode banner in aNode.SelectNodes( "/Banners/Banner[BannerType='series']" ) )
            {
                var lSeriesWideBanner = new WideBannerSeries();

                lSeriesWideBanner.Language = banner.SelectSingleNode( "Language" ).InnerText;
                lSeriesWideBanner.OnlinePath = banner.SelectSingleNode( "BannerPath" ).InnerText;
                lSeriesWideBanner.OnlineThumbPath = banner.SelectSingleNode( "ThumbnailPath" ).InnerText;
                lSeriesWideBanner.SeriesName = aSeriesName;

                try
                {
                    lSeriesWideBanner.Style = ( ArtworkStyles )Enum.Parse( typeof( ArtworkStyles ), banner.SelectSingleNode( "BannerType2" ).InnerText, true );
                }
                catch
                {
                    // maybe a new style introduced
                    lSeriesWideBanner.Style = ArtworkStyles.unknown;
                }

                if ( !string.IsNullOrEmpty( banner.SelectSingleNode( "Rating" ).InnerText ) )
                {
                    double rating = double.Parse( banner.SelectSingleNode( "Rating" ).InnerText, NumberStyles.Any, NumberFormatInfo.InvariantInfo );
                    lSeriesWideBanner.Rating = Math.Round( rating, 1, MidpointRounding.AwayFromZero );
                }

                if ( !string.IsNullOrEmpty( banner.SelectSingleNode( "RatingCount" ).InnerText ) )
                    lSeriesWideBanner.RatingCount = int.Parse( banner.SelectSingleNode( "RatingCount" ).InnerText );

                lSeriesWideBanner.SeriesID = aSeriesID;
                lWideBanners.Add( lSeriesWideBanner );
            }

            return lWideBanners;
        }

        private List<PosterSeries> GetPosterSeries( XmlNode aNode, string aSeriesID, string aSeriesName )
        {
            var lPosters = new List<PosterSeries>();

            foreach ( XmlNode banner in aNode.SelectNodes( "/Banners/Banner[BannerType='poster']" ) )
            {
                var lSeriesPoster = new PosterSeries();

                lSeriesPoster.Language = banner.SelectSingleNode( "Language" ).InnerText;
                lSeriesPoster.OnlinePath = banner.SelectSingleNode( "BannerPath" ).InnerText;
                lSeriesPoster.OnlineThumbPath = banner.SelectSingleNode( "ThumbnailPath" ).InnerText;
                lSeriesPoster.SeriesName = aSeriesName;

                if ( !string.IsNullOrEmpty( banner.SelectSingleNode( "Rating" ).InnerText ) )
                {
                    double rating = double.Parse( banner.SelectSingleNode( "Rating" ).InnerText, NumberStyles.Any, NumberFormatInfo.InvariantInfo );
                    lSeriesPoster.Rating = Math.Round( rating, 1, MidpointRounding.AwayFromZero );
                }

                if ( !string.IsNullOrEmpty( banner.SelectSingleNode( "RatingCount" ).InnerText ) )
                    lSeriesPoster.RatingCount = int.Parse( banner.SelectSingleNode( "RatingCount" ).InnerText );

                lSeriesPoster.SeriesID = aSeriesID;
                lPosters.Add( lSeriesPoster );
            }

            return lPosters;
        }

        private List<PosterSeason> GetPosterSeason( XmlNode aNode, string aSeriesID, string aSeriesName, ref List<string> aSeasons )
        {
            var lPosters = new List<PosterSeason>();

            foreach ( XmlNode banner in aNode.SelectNodes( "/Banners/Banner[BannerType='season']" ) )
            {
                var lSeasonPoster = new PosterSeason();

                lSeasonPoster.Language = banner.SelectSingleNode( "Language" ).InnerText;
                lSeasonPoster.OnlinePath = banner.SelectSingleNode( "BannerPath" ).InnerText;
                lSeasonPoster.OnlineThumbPath = banner.SelectSingleNode( "ThumbnailPath" ).InnerText;
                lSeasonPoster.SeasonIndex = banner.SelectSingleNode( "Season" ).InnerText;
                lSeasonPoster.SeriesName = aSeriesName;

                try
                {
                    lSeasonPoster.Style = ( ArtworkStyles )Enum.Parse( typeof( ArtworkStyles ), banner.SelectSingleNode( "BannerType2" ).InnerText, true );
                }
                catch
                {
                    // maybe a new style introduced
                    lSeasonPoster.Style = ArtworkStyles.unknown;
                }

                if ( !string.IsNullOrEmpty( banner.SelectSingleNode( "Rating" ).InnerText ) )
                {
                    double rating = double.Parse( banner.SelectSingleNode( "Rating" ).InnerText, NumberStyles.Any, NumberFormatInfo.InvariantInfo );
                    lSeasonPoster.Rating = Math.Round( rating, 1, MidpointRounding.AwayFromZero );
                }

                if ( !string.IsNullOrEmpty( banner.SelectSingleNode( "RatingCount" ).InnerText ) )
                    lSeasonPoster.RatingCount = int.Parse( banner.SelectSingleNode( "RatingCount" ).InnerText );

                if ( !aSeasons.Contains( lSeasonPoster.SeasonIndex ) )
                    aSeasons.Add( lSeasonPoster.SeasonIndex );

                lSeasonPoster.SeriesID = aSeriesID;
                lPosters.Add( lSeasonPoster );
            }

            return lPosters;
        }

        private void doWork(string aSeriesID)
        {
            XmlNode lEnglishBanners = null;
            XmlNode lBanners = OnlineAPI.GetBannerList(Int32.Parse(aSeriesID));
            if (lBanners == null) return;

            DBSeries lSeries = Helper.getCorrespondingSeries( Int32.Parse( aSeriesID ) );
            if (lSeries == null) return;

            string lSeriesName = lSeries.ToString();
            List<WideBannerSeries> lWidebannerSeriesList = new List<WideBannerSeries>();
            List<PosterSeries> lPosterSeriesList = new List<PosterSeries>();
            List<PosterSeason> lPosterSeasonList = new List<PosterSeason>();
            SeriesBannersMap lMap = new SeriesBannersMap();
            lMap.SeriesID = aSeriesID;

            #region Series WideBanners
            lWidebannerSeriesList = GetWideSeriesBanners( lBanners, aSeriesID, lSeriesName );

            // if the banner count is zero, try to get from english language
            if ( lWidebannerSeriesList.Count == 0 && OnlineAPI.GetSeriesLanguage(int.Parse(aSeriesID)) != "en")
            {
                lEnglishBanners = OnlineAPI.GetBannerList( Int32.Parse( aSeriesID ), "en" );
                if ( lEnglishBanners == null ) return;

                lWidebannerSeriesList = GetWideSeriesBanners( lEnglishBanners, aSeriesID, lSeriesName );
            }

            // sort by highest rated
            lWidebannerSeriesList.Sort();

            // remove banners of no interest
            if (!DBOption.GetOptions(DBOption.cGetTextBanners))
            {
                lWidebannerSeriesList.RemoveAll(b => b.Style == ArtworkStyles.text);
            }
            if (!DBOption.GetOptions(DBOption.cGetBlankBanners))
            {
                lWidebannerSeriesList.RemoveAll(b => b.Style == ArtworkStyles.blank);
            }

            // Respect User Limits, exception: if higher rated image or localised image is uploaded online
            int limit = DBOption.GetOptions(DBOption.cArtworkLimitSeriesWideBanners);
            if (limit < lWidebannerSeriesList.Count)
                lWidebannerSeriesList.RemoveRange(limit, lWidebannerSeriesList.Count - limit);
            
            lMap.SeriesWideBanners = lWidebannerSeriesList;
            #endregion

            #region Series Posters
            lPosterSeriesList = GetPosterSeries( lBanners, aSeriesID, lSeriesName );

            // if the poster count is zero, try to get from english language
            if ( lPosterSeriesList.Count == 0 && OnlineAPI.GetSeriesLanguage( int.Parse( aSeriesID ) ) != "en" )
            {
                if ( lEnglishBanners == null )
                {
                    lEnglishBanners = OnlineAPI.GetBannerList( Int32.Parse( aSeriesID ), "en" );
                    if ( lEnglishBanners == null ) return;
                }

                lPosterSeriesList = GetPosterSeries( lEnglishBanners, aSeriesID, lSeriesName );
            }

            lPosterSeriesList.Sort();

            limit = DBOption.GetOptions(DBOption.cArtworkLimitSeriesPosters);
            if (limit < lPosterSeriesList.Count)
                lPosterSeriesList.RemoveRange(limit, lPosterSeriesList.Count - limit);
            
            lMap.SeriesPosters = lPosterSeriesList;
            #endregion

            #region Season Posters
            List<string> lSeasons = new List<string>();
            lPosterSeasonList = GetPosterSeason( lBanners, aSeriesID, lSeriesName, ref lSeasons );

            // if the poster count is zero try to get from english language
            if ( lPosterSeasonList.Count == 0 && OnlineAPI.GetSeriesLanguage( int.Parse( aSeriesID ) ) != "en" )
            {
                if ( lEnglishBanners == null )
                {
                    lEnglishBanners = OnlineAPI.GetBannerList( Int32.Parse( aSeriesID ), "en" );
                    if ( lEnglishBanners == null ) return;
                }

                lPosterSeasonList = GetPosterSeason( lEnglishBanners, aSeriesID, lSeriesName, ref lSeasons );
            }

            lPosterSeasonList.Sort();
            
            // we dont support season widebanners
            lPosterSeasonList.RemoveAll(p => p.Style == ArtworkStyles.seasonwide);            

            limit = DBOption.GetOptions(DBOption.cArtworkLimitSeasonPosters);
            List<PosterSeason> posterSeasonListTemp = new List<PosterSeason>(lPosterSeasonList);

            foreach (string season in lSeasons)
            {
                int count = 0;
                foreach (PosterSeason pSeason in posterSeasonListTemp)
                {
                    if (season == pSeason.SeasonIndex)
                    {
                        count++;
                        if (limit < count) lPosterSeasonList.Remove(pSeason);
                    }
                }
            }

            lMap.SeasonPosters = lPosterSeasonList;
            #endregion

            // series already in?
            if (SeriesBannersMap.Contains(lMap))
            {
                SeriesBannersMap seriesMap = SeriesBannersMap[SeriesBannersMap.IndexOf(lMap)];
                seriesMap.SeasonPosters.AddRange(lMap.SeasonPosters);
                seriesMap.SeriesWideBanners.AddRange(lMap.SeriesWideBanners);
                seriesMap.SeriesPosters.AddRange(lMap.SeriesPosters);
            }
            else
            {
                SeriesBannersMap.Add(lMap);
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
                    string lPath = "graphical/" + Path.GetFileName( seriesWideBanner.OnlinePath );
                    seriesWideBanner.FileName = Helper.cleanLocalPath(seriesWideBanner.SeriesName) + @"\-lang" + seriesWideBanner.Language + "-" + lPath;
                        
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
                    string lPath = "posters/" + Path.GetFileName( seriesPoster.OnlinePath );
                    seriesPoster.FileName = Helper.cleanLocalPath(seriesPoster.SeriesName) + @"\-lang" + seriesPoster.Language + "-" + lPath;
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

                    string lPath = "seasons/" + Path.GetFileName( seasonPoster.OnlinePath );
                    seasonPoster.FileName = Helper.cleanLocalPath(seasonPoster.SeriesName) + @"\-lang" + seasonPoster.Language + "-" + lPath;
                        
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
        List<DBFanart> mFanart = new List<DBFanart>();
        public List<DBFanart> Fanart { get { return mFanart; } }
        
        private List<DBFanart> getFanart (XmlNode aNode, int aSeriesID )
        {
            List<DBFanart> lFanarts = new List<DBFanart>();

            foreach ( XmlNode fanartNode in aNode.SelectNodes( "/Banners/Banner[BannerType='fanart']" ) )
            {
                DBFanart dbf = new DBFanart();
                foreach ( XmlNode propertyNode in fanartNode.ChildNodes )
                {
                    try
                    {
                        dbf[propertyNode.Name] = propertyNode.InnerText;
                    }
                    catch ( Exception ex )
                    {
                        MPTVSeriesLog.Write( "Error adding Fanart Property to DBEntry: " + propertyNode.Name + " - " + ex.Message );
                    }
                }

                // Sync local files with database
                string localPath = GUITVSeries.Fanart.GetLocalPath( dbf );
                string fanart = Helper.PathCombine( Settings.GetPath( Settings.Path.fanart ), localPath );
                if ( File.Exists( fanart ) )
                {
                    dbf[DBFanart.cLocalPath] = localPath;
                }
                else
                {
                    dbf[DBFanart.cLocalPath] = string.Empty;
                }

                dbf[DBFanart.cSeriesID] = aSeriesID;
                lFanarts.Add( dbf );
            }

            return lFanarts;
        }

        public GetFanart(int SeriesID)
        {
            XmlNode node = OnlineAPI.GetBannerList(SeriesID);
            if (node == null) return;

            mFanart = getFanart( node, SeriesID );
            
            // also try to get from english banners.xml
            if ( OnlineAPI.GetSeriesLanguage( SeriesID ) != "en" )
            {
                node = OnlineAPI.GetBannerList( SeriesID, "en" );
                if ( node == null ) return;

                List<DBFanart> lEnglishFanarts = getFanart( node, SeriesID );

                foreach ( var fanart in lEnglishFanarts)
                {
                    // ensure no duplicates
                    if ( !mFanart.Contains(fanart))
                        mFanart.Add( fanart );
                }
            }          
        }
    }    
}