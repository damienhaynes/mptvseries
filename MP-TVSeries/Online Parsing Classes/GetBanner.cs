using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml;
using System.Net;
using System.IO;

namespace WindowPlugins.GUITVSeries
{
    class GetBanner
    {
        public BackgroundWorker m_Worker = new BackgroundWorker();
        private int m_nSeriesID;
        private long m_nUpdateBannersTimeStamp = 0;

        public GetBanner(int nSeriesID, long nUpdateBannersTimeStamp)
        {
            m_nSeriesID = nSeriesID;
            m_nUpdateBannersTimeStamp = nUpdateBannersTimeStamp;
            m_Worker.WorkerReportsProgress = true;
            m_Worker.WorkerSupportsCancellation = true;
            m_Worker.DoWork += new DoWorkEventHandler(worker_DoWork);
        }

        public void DoParse()
        {
            m_Worker.RunWorkerAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            XmlNodeList nodeList = ZsoriParser.GetBanners(m_nSeriesID, m_nUpdateBannersTimeStamp);
            if (nodeList != null)
            {
                GetBannersResults results = new GetBannersResults();

                foreach (XmlNode itemNode in nodeList)
                {
                    // first return item SHOULD ALWAYS be the sync time (hope so at least!)
                    if (itemNode.ChildNodes[0].Name == "SyncTime")
                    {
                        results.m_nServerTimeStamp = Convert.ToInt64(itemNode.ChildNodes[0].InnerText);
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
                                results.bannerSeriesList.Add(bannerSeries);
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
                                results.bannerSeasonList.Add(bannerSeason);
                                break;
                        }
                    }
                }

                String sBannersBasePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\banners\";

                // now that we have all the paths, download all the files
                foreach (BannerSeries bannerSeries in results.bannerSeriesList)
                {
                    bannerSeries.sBannerFileName =  bannerSeries.sSeriesName.Replace(' ', '_') + @"\" + bannerSeries.sOnlineBannerPath.Replace('/', '_');
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
                            DBTVSeries.Log("Banner download failed (" + bannerSeries.sOnlineBannerPath + ")");
                        }
                    }
                }

                foreach (BannerSeason bannerSeason in results.bannerSeasonList)
                {
                    bannerSeason.sBannerFileName = bannerSeason.sSeriesName.Replace(' ', '_') + @"\" + bannerSeason.sOnlineBannerPath.Replace('/', '_');
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
                            DBTVSeries.Log("Banner download failed (" + bannerSeason.sOnlineBannerPath + ")");
                        }
                    }
                } 
                e.Result = results;
            }
        }
    }

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

    class GetBannersResults
    {
        public long m_nServerTimeStamp = 0;
        public List<BannerSeries> bannerSeriesList = new List<BannerSeries>();
        public List<BannerSeason> bannerSeasonList = new List<BannerSeason>();
    };
}
