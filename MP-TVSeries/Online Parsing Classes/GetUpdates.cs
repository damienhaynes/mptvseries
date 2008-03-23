using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace WindowPlugins.GUITVSeries.Online_Parsing_Classes
{
    class GetUpdates
    {
        long timestamp;
        List<DBValue> series = null;
        List<DBValue> episodes = null;
        //public List<seriesBannersMap> seriesBanners = new List<seriesBannersMap>();

        public long OnlineTimeStamp
        { get { return timestamp; } }

        public List<DBValue> UpdatedSeries
        { get { return series; } }

        public List<DBValue> UpdatedEpisodes
        { get { return episodes; } }

        public GetUpdates(OnlineAPI.UpdateType type)
        {
            XmlNodeList nodelist = OnlineAPI.Updates(type);
            series = new List<DBValue>();
            episodes = new List<DBValue>();
            long maxTimeStamp = -1;
            if (nodelist != null)
            {
                foreach (XmlNode node in nodelist)
                {
                    string entryType = node.Name;
                    //string id = string.Empty;
                    //BannerSeries bs = new BannerSeries();
                    //BannerSeason bse = new BannerSeason();
                    long timeStamp = 0;

                    switch (entryType)
                    {
                        case "Episode":
                            this.episodes.Add(node.InnerText);
                            break;
                        case "Series":
                            this.series.Add(node.InnerText);
                            break;
                        case "Time":
                            long.TryParse(node.InnerText, out this.timestamp);
                            break;
                    }
                }
            }
        }
    }
}
