using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    class UpdateSeries
    {
        public BackgroundWorker m_Worker = new BackgroundWorker();
        private String m_sSeriesIDs;
        private long m_nUpdateSeriesTimeStamp;

        public UpdateSeries(String sSeriesIDs, long nUpdateSeriesTimeStamp)
        {
            m_sSeriesIDs = sSeriesIDs;
            m_nUpdateSeriesTimeStamp = nUpdateSeriesTimeStamp;
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
            if (m_sSeriesIDs != String.Empty)
            {
                XmlNodeList nodeList = null;
                nodeList = ZsoriParser.UpdateSeries(m_sSeriesIDs, m_nUpdateSeriesTimeStamp);

                if (nodeList != null)
                {
                    UpdateSeriesResults results = new UpdateSeriesResults();

                    foreach (XmlNode itemNode in nodeList)
                    {
                        // first return item SHOULD ALWAYS be the sync time (hope so at least!)
                        if (itemNode.ChildNodes[0].Name == "SyncTime")
                        {
                            results.m_nServerTimeStamp = Convert.ToInt64(itemNode.ChildNodes[0].InnerText);
                        }
                        else
                        {
                            DBSeries series = new DBSeries();
                            foreach (XmlNode propertyNode in itemNode.ChildNodes)
                            {
                                if (propertyNode.Name == "IncorrectID")
                                {
                                    // alert! drop this series, the ID doesn't match anything anymore for some reason
                                    results.listIncorrectIDs.Add(series[DBSeries.cID]);
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
                                results.listSeries.Add(series);
                        }
                    }

                    e.Result = results;
                }
            }
        }
    }

    class UpdateSeriesResults
    {
        public long m_nServerTimeStamp = 0;
        public List<DBSeries> listSeries = new List<DBSeries>();
        public List<int> listIncorrectIDs = new List<int>();
    };
}

