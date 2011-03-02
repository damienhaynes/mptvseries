using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    class GetSeries
    {
        private List<DBSeries> listSeries = new List<DBSeries>();

        public List<DBSeries> Results
        {
            get { return listSeries; }
        }

        public GetSeries(String sSeriesName)
        {
            XmlNodeList nodeList = ZsoriParser.GetSeries(sSeriesName);
            if (nodeList != null)
            {
                foreach (XmlNode itemNode in nodeList)
                {
                    DBSeries series = new DBSeries();
                    foreach (XmlNode propertyNode in itemNode.ChildNodes)
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
                    listSeries.Add(series);
                }
            }
        }

    }
}
