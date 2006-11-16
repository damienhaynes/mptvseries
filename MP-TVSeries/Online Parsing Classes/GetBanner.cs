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
        public bool bGraphical = false;
        public String sSeriesName = String.Empty;
        public String sOnlineBannerPath = String.Empty;
        public String sBannerFileName = String.Empty;
    };

    class GetBanner
    {
        private const String cInvalidFileChars = " \":<>?*|/\\";
        private long m_nServerTimeStamp = 0;
        private List<BannerSeries> m_bannerSeriesList = new List<BannerSeries>();
        private List<BannerSeason> m_bannerSeasonList = new List<BannerSeason>();

        public long ServerTimeStamp
        {
            get { return m_nServerTimeStamp; }
        }

        public List<BannerSeries> bannerSeriesList
        {
            get { return m_bannerSeriesList; }
        }

        public List<BannerSeason> bannerSeasonList
        {
            get { return m_bannerSeasonList; }
        }

        public GetBanner(int nSeriesID, long nUpdateBannersTimeStamp)
        {
            XmlNodeList nodeList = ZsoriParser.GetBanners(nSeriesID, nUpdateBannersTimeStamp);
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
                        foreach (XmlNode propertyNode in itemNode.ChildNodes)
                            if (propertyNode.Name == "Type")
                            {
                                sType = propertyNode.InnerText;
                                break;
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
                                                case "text":
                                                    bannerSeason.bGraphical = false;
                                                    break;

                                                default:
                                                    bannerSeason.bGraphical = true;
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
                                m_bannerSeasonList.Add(bannerSeason);
                                break;
                        }
                    }
                }

                String sBannersBasePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\banners\";

                // now that we have all the paths, download all the files
                foreach (BannerSeries bannerSeries in m_bannerSeriesList)
                {
                    String sBannerSeriesName = bannerSeries.sSeriesName;
                    String sOnlineBannerPath = bannerSeries.sOnlineBannerPath;
                    foreach (char c in cInvalidFileChars)
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

                foreach (BannerSeason bannerSeason in m_bannerSeasonList)
                {
                    String sBannerSeriesName = bannerSeason.sSeriesName;
                    String sOnlineBannerPath = bannerSeason.sOnlineBannerPath;
                    foreach (char c in cInvalidFileChars)
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
