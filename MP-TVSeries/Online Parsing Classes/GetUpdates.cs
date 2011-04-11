using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace WindowPlugins.GUITVSeries.Online_Parsing_Classes
{
    class GetUpdates
    {
        long timestamp;
        Dictionary<DBValue, long> series = null;
        Dictionary<DBValue, long> episodes = null;
        Dictionary<DBValue, long> banners = null;
        Dictionary<DBValue, long> fanart = null;

        public long OnlineTimeStamp
        { get { return timestamp; } }

        public Dictionary<DBValue, long> UpdatedSeries
        { get { return series; } }

        public Dictionary<DBValue, long> UpdatedEpisodes
        { get { return episodes; } }

        public Dictionary<DBValue, long> UpdatedBanners
        { get { return banners; } }

        public Dictionary<DBValue, long> UpdatedFanart
        { get { return fanart; } }

        public GetUpdates(OnlineAPI.UpdateType type)
        {
            if (type != OnlineAPI.UpdateType.all)
                MPTVSeriesLog.Write(string.Format("Downloading updates from the last {0}", type.ToString()));
            else
                MPTVSeriesLog.Write("Downloading all updates");

            XmlNode updates = OnlineAPI.Updates(type);
            if (updates == null)
                return;
       
            long.TryParse(updates.Attributes["time"].Value, out this.timestamp);

            // get all the series ids
            series = new Dictionary<DBValue, long>();
            foreach (XmlNode node in updates.SelectNodes("/Data/Series"))
            {
                long time;
                long.TryParse(node.SelectSingleNode("time").InnerText, out time);
                this.series.Add(node.SelectSingleNode("id").InnerText, time);
            }

            // get all the episode ids
            episodes = new Dictionary<DBValue, long>();
            foreach (XmlNode node in updates.SelectNodes("/Data/Episode"))
            {
                long time;
                long.TryParse(node.SelectSingleNode("time").InnerText, out time);
                this.episodes.Add(node.SelectSingleNode("id").InnerText, time);
            }
            
            // get all the season banners
            banners = new Dictionary<DBValue, long>();
            string id = string.Empty;
            long value;

            foreach (XmlNode node in updates.SelectNodes("/Data/Banner[type='season']"))
            {
                long time;
                long.TryParse(node.SelectSingleNode("time").InnerText, out time);
                id = node.SelectSingleNode("Series").InnerText;
                if (!this.banners.TryGetValue(id, out value))
                    this.banners.Add(id, time);
            }

            //get all the series banners
            foreach (XmlNode node in updates.SelectNodes("/Data/Banner[type='series']"))
            {
                long time;
                long.TryParse(node.SelectSingleNode("time").InnerText, out time);
                id = node.SelectSingleNode("Series").InnerText;
                if (!this.banners.TryGetValue(id, out value))
                    this.banners.Add(id, time);
            }

            //get all the poster banners
            foreach (XmlNode node in updates.SelectNodes("/Data/Banner[type='poster']"))
            {
                long time;
                long.TryParse(node.SelectSingleNode("time").InnerText, out time);
                id = node.SelectSingleNode("Series").InnerText;
                if (!this.banners.TryGetValue(id, out value))
                    this.banners.Add(id, time);
            }

            //get all the fanart banners
            fanart = new Dictionary<DBValue, long>();
            id = string.Empty;
            foreach (XmlNode node in updates.SelectNodes("/Data/Banner[type='fanart']"))
            {
                long time;
                long.TryParse(node.SelectSingleNode("time").InnerText, out time);
                id = node.SelectSingleNode("Series").InnerText;
                if (!this.fanart.TryGetValue(id, out value))
                    this.fanart.Add(id, time);
            }

        }
    }
}
