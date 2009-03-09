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
            XmlNode updates = OnlineAPI.Updates(type);
            if (updates == null)
                return;

            long.TryParse(updates.Attributes["time"].Value, out this.timestamp);
            
            XmlNodeList nodelist = updates.ChildNodes;
            
            series = new List<DBValue>();
            episodes = new List<DBValue>();
            if (nodelist != null)
            {
                foreach (XmlNode node in nodelist)
                {
                    string entryType = node.Name;
                    string id = string.Empty;
                    for (int i = 0; i < node.ChildNodes.Count; i++)
                    {
                        if (node.ChildNodes[i].Name.Equals("id", StringComparison.InvariantCultureIgnoreCase))
                        {
                            id = node.ChildNodes[i].InnerText;
                            break;
                        }
                    }

                    if (id.Length > 0)
                    {
                        switch (entryType)
                        {
                            case "Episode":
                                this.episodes.Add(id);
                                break;
                            case "Series":
                                this.series.Add(id);
                                break;
                        }
                    }
                }
            }
        }
    }
}
