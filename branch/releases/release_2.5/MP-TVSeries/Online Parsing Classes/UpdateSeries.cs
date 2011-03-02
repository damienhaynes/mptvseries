#region GNU license
// MP-TVSeries - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2007
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#endregion


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
        private List<DBOnlineSeries> listSeries = new List<DBOnlineSeries>();
        private List<int> listIncorrectIDs = new List<int>();

        public long ServerTimeStamp
        {
            get { return m_nServerTimeStamp; }
        }

        public List<DBOnlineSeries> Results
        {
            get { return listSeries; }
        }

        public List<int> BadIds
        {
            get { return listIncorrectIDs; }
        }

        public UpdateSeries(List<String> sSeriesIDs)
        {
            foreach(string id in sSeriesIDs)
                Work(id);
        }
        
        public UpdateSeries(String sSeriesIDs)
        {
            Work(sSeriesIDs);
        }
        
        public UpdateSeries(List<String> sSeriesIDs, String languageID)
        {
            foreach (String id in sSeriesIDs)
                Work(id, languageID);
        }
        
        public UpdateSeries(String sSeriesIDs, String languageID)
        {
            Work(sSeriesIDs, languageID);
        }
        
        void Work(String sSeriesID)
        {
            Work(sSeriesID, "");
        }
        
        void Work(String sSeriesID, String languageID)
        {
            if (sSeriesID.Length > 0)
            {
                int result;
                if (int.TryParse(sSeriesID,out result))
                    MPTVSeriesLog.Write(string.Format("Retrieving updated Metadata for series {0}",Helper.getCorrespondingSeries(result)));
                
                XmlNode node = null;
                if (String.IsNullOrEmpty(languageID))
                {
                    node = Online_Parsing_Classes.OnlineAPI.UpdateSeries(sSeriesID);
                }
                else
                {
                    node = Online_Parsing_Classes.OnlineAPI.UpdateSeries(sSeriesID, languageID);
                }

                if (node != null)
                {
                    foreach (XmlNode itemNode in node.ChildNodes)
                    {
                        bool hasDVDOrdering = false;
                        bool hasAbsoluteOrdering = false;
                        DBOnlineSeries series = new DBOnlineSeries();
                        foreach (XmlNode seriesNode in itemNode)
                        {
                            // first return item SHOULD ALWAYS be the series
                            if (seriesNode.Name.Equals("Series", StringComparison.InvariantCultureIgnoreCase))
                            {                                                                
                                foreach (XmlNode propertyNode in seriesNode.ChildNodes)
                                {
                                    if (propertyNode.Name == "Language") // work around inconsistancy (language = Language)
                                    {
                                        series["language"] = propertyNode.InnerText;
                                    }
                                    else if (DBOnlineSeries.s_OnlineToFieldMap.ContainsKey(propertyNode.Name))
                                        series[DBOnlineSeries.s_OnlineToFieldMap[propertyNode.Name]] = propertyNode.InnerText;
                                    else
                                    {
                                        // we don't know that field, add it to the series table
                                        series.AddColumn(propertyNode.Name, new DBField(DBField.cTypeString));
                                        series[propertyNode.Name] = propertyNode.InnerText;
                                    }
                                }
                                if (series != null) listSeries.Add(series);
                            }
                            else if(!hasDVDOrdering || !hasAbsoluteOrdering || seriesNode.Name.Equals("Episode", StringComparison.InvariantCultureIgnoreCase))
                            {                                
                                foreach (XmlNode propertyNode in seriesNode.ChildNodes)
                                {
                                    switch (propertyNode.Name)
                                    {
                                        case "DVD_episodenumber":
                                        case "DVD_season":
                                            if(!String.IsNullOrEmpty(propertyNode.InnerText)) hasDVDOrdering = true;
                                            break;
                                        case "absolute_number":
                                            if (!String.IsNullOrEmpty(propertyNode.InnerText)) hasAbsoluteOrdering = true;
                                            break;
                                    }
                                }
                            }
                        }
                        if ((hasAbsoluteOrdering || hasDVDOrdering))
                        {
                            string ordering = series[DBOnlineSeries.cEpisodeOrders] == string.Empty ? "Aired|" : (string)series[DBOnlineSeries.cEpisodeOrders];
                            if (hasAbsoluteOrdering) ordering += "Absolute|";
                            if (hasDVDOrdering) ordering += "DVD";
                            series[DBOnlineSeries.cEpisodeOrders] = ordering;
                        }
                    }
                }
            }
        }
    }
}

