using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    class GetSeries
    {
        public BackgroundWorker m_Worker = new BackgroundWorker();
        private String m_SeriesName;

        public GetSeries(String sSeriesName)
        {
            m_SeriesName = sSeriesName;
            m_Worker.WorkerReportsProgress = true;
            m_Worker.WorkerSupportsCancellation = true;
            m_Worker.DoWork += new DoWorkEventHandler(worker_DoWork);
        }

        public void DoParse()
        {
            m_Worker.RunWorkerAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            XmlNodeList nodeList = ZsoriParser.FindSeries(m_SeriesName);
            if (nodeList != null)
            {
                GetSeriesResults results = new GetSeriesResults();

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
                    results.listSeries.Add(series);
                }

                e.Result = results;
            }
        }
    }

    class GetSeriesResults
    {
        public List<DBSeries> listSeries = new List<DBSeries>();
    };
}
