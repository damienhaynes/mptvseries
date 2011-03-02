using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    class GetSeries
    {
        private List<DBOnlineSeries> listSeries = new List<DBOnlineSeries>();

        public List<DBOnlineSeries> Results
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
                    DBOnlineSeries series = new DBOnlineSeries();
                    foreach (XmlNode propertyNode in itemNode.ChildNodes)
                    {
                        if (DBOnlineSeries.s_OnlineToFieldMap.ContainsKey(propertyNode.Name))
                            series[DBOnlineSeries.s_OnlineToFieldMap[propertyNode.Name]] = propertyNode.InnerText;
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
