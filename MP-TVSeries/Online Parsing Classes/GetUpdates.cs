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

        public long OnlineTimeStamp
        { get { return timestamp; } }

        public List<DBValue> UpdatedSeries
        { get { return series; } }

        public List<DBValue> UpdatedEpisodes
        { get { return episodes; } }

        public GetUpdates(OnlineAPI.UpdateType type)
        {
            long lastUpdatetime = DBOption.GetOptions(DBOption.cUpdateTimeStamp);
            XmlNodeList nodelist = OnlineAPI.Updates(lastUpdatetime, type);
            if (type > OnlineAPI.UpdateType.none) series = new List<DBValue>();
            if (type > OnlineAPI.UpdateType.series) episodes = new List<DBValue>(); 
            if (nodelist != null)
            {
                foreach (XmlNode node in nodelist)
                {
                    switch (node.Name)
                    {
                        case "Time":
                            timestamp = long.Parse(node.InnerText);
                            break;
                        case "Series":
                            series.Add(node.InnerText);
                            break;
                        case "Episode":
                            episodes.Add(node.InnerText);
                            break;
                    }
                }
            }
        }
    }
}
