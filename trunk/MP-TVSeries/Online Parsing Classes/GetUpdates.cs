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
        List<DBValue> banners = null;

        public long OnlineTimeStamp
        { get { return timestamp; } }

        public List<DBValue> UpdatedSeries
        { get { return series; } }

        public List<DBValue> UpdatedEpisodes
        { get { return episodes; } }

        public List<DBValue> UpdatedBanners
        { get { return banners; } }

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

            //get all the series ids
            series = new List<DBValue>();
            foreach (XmlNode node in updates.SelectNodes("/Data/Series/id")) {
                this.series.Add(node.Value);
            }

            //get all the episode ids
            episodes = new List<DBValue>();
            foreach (XmlNode node in updates.SelectNodes("/Data/Episode/id")) {
                this.episodes.Add(node.Value);
            }
            
            //get all the season banners
            banners = new List<DBValue>();
            string id = string.Empty;

            foreach (XmlNode node in updates.SelectNodes("/Data/Banner[type='season']")) {
                id = node.SelectSingleNode("Series").InnerText;
                if (!this.banners.Contains(id))
                    this.banners.Add(id);
            }

            //get all the series banners
            foreach (XmlNode node in updates.SelectNodes("/Data/Banner[type='series']")) {
                id = node.SelectSingleNode("Series").InnerText;
                if (!this.banners.Contains(id))
                    this.banners.Add(id);
            }

            //get all the poster banners
            foreach (XmlNode node in updates.SelectNodes("/Data/Banner[type='poster']")) {
                id = node.SelectSingleNode("Series").InnerText;
                if (!this.banners.Contains(id))
                    this.banners.Add(id);
            }

        }
    }
}
