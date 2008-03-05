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
        public List<seriesBannersMap> seriesBanners = new List<seriesBannersMap>();

        public long OnlineTimeStamp
        { get { return timestamp; } }

        public List<DBValue> UpdatedSeries
        { get { return series; } }

        public List<DBValue> UpdatedEpisodes
        { get { return episodes; } }

        public GetUpdates(OnlineAPI.UpdateType type, double minTimestampToConsider)
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
                    string id = string.Empty;
                    BannerSeries bs = new BannerSeries();
                    BannerSeason bse = new BannerSeason();
                    long timeStamp = 0;
                    if(entryType != "Banner")
                    {
                        foreach (XmlNode propertyNode in node.ChildNodes)
                        {
                            switch (propertyNode.Name)
                            {
                                case "id":
                                    id = propertyNode.InnerText;
                                    break;
                                case "time":
                                    long.TryParse(propertyNode.InnerText, out timeStamp);                                   
                                    break;
                            }
                        }
                        if (timeStamp >= minTimestampToConsider && !Helper.String.IsNullOrEmpty(id))
                        {
                            switch (entryType)
                            {
                                case "Series":
                                    series.Add(id);
                                    break;
                                case "Episode":
                                    episodes.Add(id);
                                    break;
                            }
                            if (maxTimeStamp < timeStamp) maxTimeStamp = timeStamp; // should be changed to timestamp returned from server and not maxTimestamp in updates
                        }
                    }
                    else
                    {
                        bool isSeason = false;
                        bool isWideseason = false;
                        string seriesID = string.Empty;
                        foreach (XmlNode propertyNode in node.ChildNodes)
                        {
                            switch (propertyNode.Name)
                            {
                                case "SeasonNum":
                                    bse.sSeason = propertyNode.InnerText; //series also returns this for some reason
                                    break;
                                case "Series":
                                    seriesID = propertyNode.InnerText;
                                    break;
                                case "format":
                                    switch (propertyNode.InnerText)
                                    {
                                        case "graphical":
                                            bs.bGraphical = true;
                                            break;
                                        case "text":
                                            bs.bGraphical = false;
                                            break;
                                        case "blank":
                                            bs.bGraphical = false;
                                            break;
                                        case "standard":
                                            isWideseason = false;
                                            break;
                                        case "wide":
                                            isWideseason = true;
                                            break;
                                    }
                                    break;
                                case "language":
                                    bs.sBannerLang = propertyNode.InnerText;
                                    bse.sBannerLang = propertyNode.InnerText;
                                    break;
                                case "path":
                                    bs.sOnlineBannerPath = propertyNode.InnerText;
                                    bse.sOnlineBannerPath = propertyNode.InnerText;
                                    break;
                                case "type":
                                    switch (propertyNode.InnerText)
                                    {
                                        case "season":
                                            isSeason = true;
                                            break;
                                        case "series":
                                            isSeason = false;
                                            break;
                                    }
                                    break;
                            }
                        }
                        seriesBannersMap dummy = new seriesBannersMap(seriesID);
                        if (!seriesBanners.Contains(dummy)) seriesBanners.Add(dummy);
                        if (isSeason)
                        {
                            if (!isWideseason) seriesBanners[seriesBanners.IndexOf(dummy)].seasonBanners.Add(bse);
                        }
                        else seriesBanners[seriesBanners.IndexOf(dummy)].seriesBanners.Add(bs);      
                    }
                }
            }
            this.timestamp = maxTimeStamp;
        }
    }
}
