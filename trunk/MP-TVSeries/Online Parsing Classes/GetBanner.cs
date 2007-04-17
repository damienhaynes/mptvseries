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
        public String sSeriesName = String.Empty;
        public String sOnlineBannerPath = String.Empty;
        public String sBannerFileName = String.Empty;
    };

    class BannerSeason
    {
        public int nIndex = 0;
        public bool bIsNeeded = false;
        public String sSeriesName = String.Empty;
        public String sOnlineBannerPath = String.Empty;
        public String sBannerFileName = String.Empty;
    };

    class seriesBannersMap : System.IEquatable<seriesBannersMap>    
    {
        public string seriesID = string.Empty;
        public List<BannerSeries> seriesBanners = new List<BannerSeries>();
        public List<BannerSeason> seasonBanners = new List<BannerSeason>();



        #region IEquatable<seriesBannersMap> Members

        bool IEquatable<seriesBannersMap>.Equals(seriesBannersMap other)
        {
            return seriesID.Equals(other.seriesID);
        }

        #endregion
    }

    class GetBanner
    {
        //private const String cInvalidFileChars = " \":<>?*|/\\";
        private long m_nServerTimeStamp = 0;
        //private List<BannerSeries> m_bannerSeriesList = new List<BannerSeries>();
        //private List<BannerSeason> m_bannerSeasonList = new List<BannerSeason>();
        public List<seriesBannersMap> seriesBanners = new List<seriesBannersMap>();

        static String sBannersBasePath = Settings.GetPath(Settings.Path.banners) + @"\";

        public long ServerTimeStamp
        {
            get { return m_nServerTimeStamp; }
        }

        //public List<BannerSeries> bannerSeriesList
        //{
        //    get { return m_bannerSeriesList; }
        //}

        //public List<BannerSeason> bannerSeasonList
        //{
        //    get { return m_bannerSeasonList; }
        //}


        public GetBanner(int nSeriesID, long nUpdateBannersTimeStamp, List<int> SeasonsToDownload, bool allSeasons)
        {
            work(nSeriesID, nUpdateBannersTimeStamp, SeasonsToDownload, allSeasons);
        }

        /// <summary>
        /// This constructor automatically get's relevant seasons
        /// </summary>
        /// <param name="nSeriesID"></param>
        /// <param name="nUpdateBannersTimeStamp"></param>
        public GetBanner(int nSeriesID, long nUpdateBannersTimeStamp)
        {
            List<int> relevantSeasons = new List<int>();
            foreach(DBSeason season in DBSeason.Get(nSeriesID, false, true, true))
                relevantSeasons.Add(season[DBSeason.cIndex]);
            work(nSeriesID, nUpdateBannersTimeStamp, relevantSeasons, false);
        }

        public GetBanner(string idList, long nUpdateBannersTimeStamp)
        {
            work(ZsoriParser.GetAllBanners(idList, nUpdateBannersTimeStamp), null, true);
        }

        private void work(int nSeriesID, long nUpdateBannersTimeStamp, List<int> SeasonsToDownload, bool allSeasons)
        {
            work(ZsoriParser.GetBanners(nSeriesID, nUpdateBannersTimeStamp), SeasonsToDownload, allSeasons);
        }

        private void work(XmlNodeList nodeList, List<int> SeasonsToDownload, bool allSeasons)
        {
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
                        String sType = String.Empty;
                        //string seriesID = String.Empty;
                        seriesBannersMap map = new seriesBannersMap();
                        List<BannerSeries> m_bannerSeriesList = new List<BannerSeries>();
                        List<BannerSeason> m_bannerSeasonList = new List<BannerSeason>();
                        foreach (XmlNode propertyNode in itemNode.ChildNodes)
                        {
                            if (propertyNode.Name == "Type")
                            {
                                sType = propertyNode.InnerText;
                                if (map.seriesID.Length > 0)
                                    break;
                            }
                            else if (propertyNode.Name == "SeriesID")
                            {
                                map.seriesID = propertyNode.InnerText;
                                if (sType.Length > 0)
                                    break;
                            }
                        }

                        switch (sType)
                        {
                            case "series":
                                BannerSeries bannerSeries = new BannerSeries();
                                foreach (XmlNode propertyNode in itemNode.ChildNodes)
                                {
                                    switch (propertyNode.Name)
                                    {
                                        case "SeriesName":
                                            bannerSeries.sSeriesName = propertyNode.InnerText;
                                            break;

                                        case "BannerType":
                                            switch (propertyNode.InnerText)
                                            {
                                                case "text":
                                                    bannerSeries.bGraphical = false;
                                                    break;

                                                default:
                                                    bannerSeries.bGraphical = true;
                                                    break;
                                            }
                                            break;

                                        case "BannerPath":
                                            {
                                                bannerSeries.sOnlineBannerPath = propertyNode.InnerText;
                                            }
                                            break;
                                    }
                                }
                                m_bannerSeriesList.Add(bannerSeries);
                                break;

                            case "season":
                                if (!allSeasons && SeasonsToDownload.Count == 0) break;
                                BannerSeason bannerSeason = new BannerSeason();
                                foreach (XmlNode propertyNode in itemNode.ChildNodes)
                                {
                                    switch (propertyNode.Name)
                                    {
                                        case "SeriesName":
                                            bannerSeason.sSeriesName = propertyNode.InnerText;
                                            break;

                                        case "Season":
                                            bannerSeason.nIndex = Convert.ToInt32(propertyNode.InnerText);
                                            break;

                                        case "BannerType":
                                            switch (propertyNode.InnerText)
                                            {
                                                case "season":
                                                    // only season type we support
                                                    bannerSeason.bIsNeeded = true;
                                                    break;

                                                default:
                                                    bannerSeason.bIsNeeded = false;
                                                    // ignore unknown types
                                                    break;
                                            }
                                            break;

                                        case "BannerPath":
                                            {
                                                bannerSeason.sOnlineBannerPath = propertyNode.InnerText;
                                            }
                                            break;
                                    }
                                }
                                if (bannerSeason.bIsNeeded && ( allSeasons || ( null!= SeasonsToDownload  && SeasonsToDownload.Contains(bannerSeason.nIndex))))
                                    m_bannerSeasonList.Add(bannerSeason);
                                break;
                        }
                        map.seasonBanners = m_bannerSeasonList;
                        map.seriesBanners = m_bannerSeriesList;
                        // series already in?
                        if(seriesBanners.Contains(map))
                        {
                           seriesBannersMap seriesMap =  seriesBanners[seriesBanners.IndexOf(map)];
                           seriesMap.seasonBanners.AddRange(map.seasonBanners);
                           seriesMap.seriesBanners.AddRange(map.seriesBanners);
                        }
                        else seriesBanners.Add(map);
                    }
                }

                // now that we have all the paths, download all the files
                foreach (seriesBannersMap map in seriesBanners)
                {
                    foreach (BannerSeries bannerSeries in map.seriesBanners)
                    {
                        String sBannerSeriesName = bannerSeries.sSeriesName;
                        String sOnlineBannerPath = bannerSeries.sOnlineBannerPath;
                        foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                        {
                            sBannerSeriesName = sBannerSeriesName.Replace(c, '_');
                            sOnlineBannerPath = sOnlineBannerPath.Replace(c, '_');
                        }
                        bannerSeries.sBannerFileName = sBannerSeriesName + @"\" + sOnlineBannerPath;
                        // check if banner is already there (don't download twice)
                        if (!File.Exists(sBannersBasePath + bannerSeries.sBannerFileName))
                        {
                            WebClient webClient = new WebClient();
                            try
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(sBannersBasePath + bannerSeries.sBannerFileName));
                                webClient.DownloadFile(DBOnlineMirror.Banners + "/" + bannerSeries.sOnlineBannerPath, sBannersBasePath + bannerSeries.sBannerFileName);
                            }
                            catch (WebException)
                            {
                                MPTVSeriesLog.Write("Banner download failed (" + bannerSeries.sOnlineBannerPath + ")");
                            }
                        }
                    }

                    foreach (BannerSeason bannerSeason in map.seasonBanners)
                    {
                        String sBannerSeriesName = bannerSeason.sSeriesName;
                        String sOnlineBannerPath = bannerSeason.sOnlineBannerPath;
                        foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                        {
                            sBannerSeriesName = sBannerSeriesName.Replace(c, '_');
                            sOnlineBannerPath = sOnlineBannerPath.Replace(c, '_');
                        }
                        bannerSeason.sBannerFileName = sBannerSeriesName + @"\" + sOnlineBannerPath;
                        if (!File.Exists(sBannersBasePath + bannerSeason.sBannerFileName))
                        {
                            WebClient webClient = new WebClient();
                            try
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(sBannersBasePath + bannerSeason.sBannerFileName));
                                webClient.DownloadFile(DBOnlineMirror.Banners + "/" + bannerSeason.sOnlineBannerPath, sBannersBasePath + bannerSeason.sBannerFileName);
                            }
                            catch (WebException)
                            {
                                MPTVSeriesLog.Write("Banner download failed (" + bannerSeason.sOnlineBannerPath + ")");
                            }
                        }
                    }
                }
            }
        }
    }
}
