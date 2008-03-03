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
                    long timeStamp = 0;
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
            }
            this.timestamp = maxTimeStamp;
        }
    }
}
