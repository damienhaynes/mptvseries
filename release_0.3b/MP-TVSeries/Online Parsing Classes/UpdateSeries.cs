using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    class UpdateSeries
    {
        private long m_nServerTimeStamp = 0;
        private List<DBSeries> listSeries = new List<DBSeries>();
        private List<int> listIncorrectIDs = new List<int>();

        public long ServerTimeStamp
        {
            get { return m_nServerTimeStamp; }
        }

        public List<DBSeries> Results
        {
            get { return listSeries; }
        }

        public List<int> BadIds
        {
            get { return listIncorrectIDs; }
        }


        public UpdateSeries(String sSeriesIDs, long nUpdateSeriesTimeStamp)
        {
            if (sSeriesIDs != String.Empty)
            {
                XmlNodeList nodeList = null;
                nodeList = ZsoriParser.UpdateSeries(sSeriesIDs, nUpdateSeriesTimeStamp);

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
                            DBSeries series = new DBSeries();
                            foreach (XmlNode propertyNode in itemNode.ChildNodes)
                            {
                                if (propertyNode.Name == "IncorrectID")
                                {
                                    // alert! drop this series, the ID doesn't match anything anymore for some reason
                                    listIncorrectIDs.Add(series[DBSeries.cID]);
                                    series = null;
                                    break;
                                }
                                else
                                {
                                    if (DBSeries.s_OnlineToFieldMap.ContainsKey(propertyNode.Name))
                                        series[DBSeries.s_OnlineToFieldMap[propertyNode.Name]] = propertyNode.InnerText;
                                    else
                                    {
                                        // we don't know that field, add it to the series table
                                        series.AddColumn(propertyNode.Name, new DBField(DBField.cTypeString));
                                        series[propertyNode.Name] = propertyNode.InnerText;
                                    }
                                }
                            }
                            if (series != null)
                                listSeries.Add(series);
                        }
                    }
                }
            }
        }
    }
}

