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
using System.Net;
using System.IO;

namespace WindowPlugins.GUITVSeries
{
    class BannerSeries
    {
        public bool bGraphical = false;
        public bool bHighestRated = false;        
        public String sSeriesName = String.Empty;
        public String sOnlineBannerPath = String.Empty;
        public String sBannerFileName = String.Empty;
        public String sBannerLang = String.Empty;
    };

    class PosterSeries
    {        
        public bool bHighestRated = false;        
        public String sSeriesName = String.Empty;
        public String sOnlinePosterPath = String.Empty;
        public String sPosterFileName = String.Empty;
        public String sPosterLang = String.Empty;
    };

    class BannerSeason
    {
        public int nIndex = 0;
        public bool bIsNeeded = false;
        public bool bHighestRated = false;
        public String sSeriesName = String.Empty;
        public String sOnlineBannerPath = String.Empty;
        public String sBannerFileName = String.Empty;
        public String sBannerLang = String.Empty;
        public String sSeason = string.Empty;
    };

    class seriesBannersMap : System.IEquatable<seriesBannersMap>    
    {
        public string seriesID = string.Empty;
        public List<BannerSeries> seriesBanners = new List<BannerSeries>();
        public List<PosterSeries> seriesPosters = new List<PosterSeries>();
        public List<BannerSeason> seasonBanners = new List<BannerSeason>();

        public seriesBannersMap()
        {
        }

        public seriesBannersMap(string seriesID)
        {
            this.seriesID = seriesID;
        }

        #region IEquatable<seriesBannersMap> Members

        bool IEquatable<seriesBannersMap>.Equals(seriesBannersMap other)
        {
            return seriesID.Equals(other.seriesID);
        }

        #endregion
    }

    class GetBanner
    {
        public List<seriesBannersMap> seriesBannersMap = new List<seriesBannersMap>();

        static String sBannersBasePath = Settings.GetPath(Settings.Path.banners) + @"\";
        String localizedSeriesName = string.Empty;

        public GetBanner(string seriesID)
        {
            doWork(seriesID);
        }

        private void doWork(string seriesID)
        {
            XmlNode node = Online_Parsing_Classes.OnlineAPI.getBannerList(Int32.Parse(seriesID));
            List<BannerSeries> m_bannerSeriesList = new List<BannerSeries>();
            List<PosterSeries> m_posterSeriesList = new List<PosterSeries>();
            List<BannerSeason> m_bannerSeasonList = new List<BannerSeason>();
            seriesBannersMap map = new seriesBannersMap();

            if (node != null)
            {                
                foreach (XmlNode topNode in node.ChildNodes)
                {
                    bool bHighestRatedSeriesBannerIsSet = false;
                    bool bHighestRatedSeriesPosterIsSet = false;
                    bool bHighestRatedSeasonIsSet = false;

                    string sCurrentSeason = string.Empty;
                    string[] sPreviousSeasons = { "" };

                    foreach (XmlNode itemNode in topNode.ChildNodes)
                    {                        
                        if(itemNode.Name == "Banner")
                        {
                            BannerSeason seasonBanners = new BannerSeason();
                            BannerSeries seriesBanners = new BannerSeries();
                            PosterSeries seriesPosters = new PosterSeries();

                            bool isSeries = false;
                            bool isPoster = false;
                            bool isSeason = false;
                            foreach (XmlNode propertyNode in itemNode.ChildNodes)
                            {
                                switch (propertyNode.Name)
                                {
                                    case "BannerPath":
                                        seasonBanners.sOnlineBannerPath = propertyNode.InnerText;
                                        seriesBanners.sOnlineBannerPath = propertyNode.InnerText;
                                        seriesPosters.sOnlinePosterPath = propertyNode.InnerText;
                                        break;

                                    case "BannerType":
                                        if (propertyNode.InnerText.Equals("series", StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            isPoster = false;
                                            isSeries = true;
                                        }

                                        if (propertyNode.InnerText.Equals("poster", StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            isPoster = true;
                                            isSeries = true;
                                        }                                    
                                        break;

                                    case "BannerType2":
                                        if (isSeries)
                                        {
                                            if (propertyNode.InnerText.Equals("Graphical", StringComparison.CurrentCultureIgnoreCase))
                                            {
                                                // The first Banner is the Highest Rated Banner                                                
                                                if (!bHighestRatedSeriesBannerIsSet)
                                                {
                                                    bHighestRatedSeriesBannerIsSet = true;
                                                    seriesBanners.bHighestRated = true;
                                                }
                                                seriesBanners.bGraphical = true;
                                            }
                                            if (propertyNode.InnerText.Equals("680x1000", StringComparison.CurrentCultureIgnoreCase))
                                            {
                                                if (!bHighestRatedSeriesPosterIsSet)
                                                {
                                                    bHighestRatedSeriesPosterIsSet = true;
                                                    seriesPosters.bHighestRated = true;
                                                }                                                
                                            }
                                        }
                                        else
                                        {
                                            if (propertyNode.InnerText.Equals("season", StringComparison.CurrentCultureIgnoreCase))                                            
                                                 isSeason = true;
                                        }
                                        break;

                                    case "Language":
                                        seasonBanners.sBannerLang = propertyNode.InnerText;
                                        seriesBanners.sBannerLang = propertyNode.InnerText;
                                        seriesPosters.sPosterLang = propertyNode.InnerText;
                                        break;

                                    case "Season":
                                        sCurrentSeason = propertyNode.InnerText;
                                        seasonBanners.sSeason = sCurrentSeason;
                                        
                                        // Each season can have a highest rated banner
                                        // Search through all previous season banners                                        
                                        bHighestRatedSeasonIsSet = false;
                                        for (int i = 0; i < sPreviousSeasons.Length; i++)
                                        {   
                                            // If season has already been identified as highest rated
                                            // then set flag
                                            if (sPreviousSeasons[i].Equals(sCurrentSeason))
                                            {
                                                bHighestRatedSeasonIsSet = true;
                                                break;
                                            }
                                        }

                                        if (!bHighestRatedSeasonIsSet)
                                        {
                                            bHighestRatedSeasonIsSet = true;
                                            seasonBanners.bHighestRated = true;
                                            Array.Resize(ref sPreviousSeasons, sPreviousSeasons.Length + 1);
                                            sPreviousSeasons[sPreviousSeasons.Length - 1] = sCurrentSeason;                                            
                                        }                                                                                                                        
                                        break;
                                }                                
                            }

                            try
                            {
                                seriesBanners.sSeriesName = Helper.getCorrespondingSeries(Int32.Parse(seriesID)).ToString();
                                seriesPosters.sSeriesName = Helper.getCorrespondingSeries(Int32.Parse(seriesID)).ToString();
                            }
                            catch { return; }
                            seasonBanners.sSeriesName = seriesBanners.sSeriesName;

                            if (isSeries)
                            {
                                if (isPoster)
                                    m_posterSeriesList.Add(seriesPosters);
                                else
                                    m_bannerSeriesList.Add(seriesBanners);
                            }
                            else if (isSeason)
                                m_bannerSeasonList.Add(seasonBanners);
                        }                        
                    }
                }
                map.seasonBanners = m_bannerSeasonList;
                map.seriesBanners = m_bannerSeriesList;
                map.seriesPosters = m_posterSeriesList;

                // series already in?
                if (seriesBannersMap.Contains(map))
                {
                    seriesBannersMap seriesMap = seriesBannersMap[seriesBannersMap.IndexOf(map)];
                    seriesMap.seasonBanners.AddRange(map.seasonBanners);
                    seriesMap.seriesBanners.AddRange(map.seriesBanners);
                    seriesMap.seriesPosters.AddRange(map.seriesPosters);
                }
                else seriesBannersMap.Add(map);
                DownloadBanners(Online_Parsing_Classes.OnlineAPI.SelLanguageAsString);

            }
        }

        private void DownloadBanners(string bannerLang)
        {
            // now that we have all the paths, download all the files
            foreach (seriesBannersMap map in seriesBannersMap)
            {
                foreach (BannerSeries bannerSeries in map.seriesBanners)
                {
                    if (bannerLang == bannerSeries.sBannerLang || "en" == bannerSeries.sBannerLang || "" == bannerSeries.sBannerLang) //also always english ones
                    {
                        // mark the filename with the language                        
                        bannerSeries.sBannerFileName = Helper.cleanLocalPath(bannerSeries.sSeriesName) + @"\-lang" + bannerSeries.sBannerLang + "-" + bannerSeries.sOnlineBannerPath;                        
                        Online_Parsing_Classes.OnlineAPI.DownloadBanner(bannerSeries.sOnlineBannerPath, Settings.Path.banners, bannerSeries.sBannerFileName);
                        //string fullURL = (DBOnlineMirror.Banners.EndsWith("/") ? DBOnlineMirror.Banners : (DBOnlineMirror.Banners + "/")) + bannerSeries.sBannerFileName;                        
                        //int nDownloadGUID = Online_Parsing_Classes.OnlineAPI.StartFileDownload(fullURL, Settings.Path.banners, bannerSeries.sBannerFileName);
                        //while (Online_Parsing_Classes.OnlineAPI.CheckFileDownload(nDownloadGUID)) System.Windows.Forms.Application.DoEvents();
                    }
                }
            
                foreach (PosterSeries posterSeries in map.seriesPosters)
                {
                    if (bannerLang == posterSeries.sPosterLang || "en" == posterSeries.sPosterLang || "" == posterSeries.sPosterLang) //also always english ones
                    {
                        // mark the filename with the language                        
                        posterSeries.sPosterFileName = Helper.cleanLocalPath(posterSeries.sSeriesName) + @"\-lang" + posterSeries.sPosterLang + "-" + posterSeries.sOnlinePosterPath;                        
                        Online_Parsing_Classes.OnlineAPI.DownloadBanner(posterSeries.sOnlinePosterPath, Settings.Path.banners, posterSeries.sPosterFileName);                        
                    }
                }

                foreach (BannerSeason bannerSeason in map.seasonBanners)
                {
                    if (bannerLang == bannerSeason.sBannerLang || "en" == bannerSeason.sBannerLang || "" == bannerSeason.sBannerLang)
                    {
                        bannerSeason.sBannerFileName = Helper.cleanLocalPath(bannerSeason.sSeriesName) + @"\-lang" + bannerSeason.sBannerLang + "-" + bannerSeason.sOnlineBannerPath;
                        Online_Parsing_Classes.OnlineAPI.DownloadBanner(bannerSeason.sOnlineBannerPath, Settings.Path.banners, bannerSeason.sBannerFileName);
                        //string fullURL = (DBOnlineMirror.Banners.EndsWith("/") ? DBOnlineMirror.Banners : (DBOnlineMirror.Banners + "/")) + bannerSeason.sBannerFileName;                        
                        //int nDownloadGUID = Online_Parsing_Classes.OnlineAPI.StartFileDownload(fullURL, Settings.Path.banners, bannerSeason.sBannerFileName);
                        //while (Online_Parsing_Classes.OnlineAPI.CheckFileDownload(nDownloadGUID)) System.Windows.Forms.Application.DoEvents();
                    }
                }
            }
        }
    }

    class GetFanart
    {
        List<DBFanart> _fanart = new List<DBFanart>();
        public List<DBFanart> Fanart { get { return _fanart; } }
        
        public GetFanart(int SeriesID)
        {
            XmlNode node = Online_Parsing_Classes.OnlineAPI.getBannerList(SeriesID);
            if (node != null)
            {
                foreach (XmlNode topNode in node.ChildNodes)
                {
                    foreach (XmlNode itemNode in topNode.ChildNodes)
                    {                        
                        if (itemNode.Name == "Banner")
                        {
                            bool isFanart = false;
                            DBFanart dbf = new DBFanart();
                            foreach (XmlNode propertyNode in itemNode.ChildNodes)
                            {
                                try
                                {
                                    dbf[propertyNode.Name] = propertyNode.InnerText;                                    
                                }
                                catch (Exception ex)
                                { MPTVSeriesLog.Write("Error adding Fanart Property to DBEntry: " + propertyNode.Name + " - " + ex.Message); }

                                if (propertyNode.Name == "BannerType"
                                    && propertyNode.InnerText.Equals("fanart", StringComparison.InvariantCultureIgnoreCase))
                                    isFanart = true;
                            }
                            if (isFanart)
                            {                                                              
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
            }
        }
    }

    
}
