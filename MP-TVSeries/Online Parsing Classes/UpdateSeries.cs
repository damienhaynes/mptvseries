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
using System.Linq;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    class UpdateSeries
    {
        private List<String> mSeriesIDs = null;
        private long mServerTimeStamp = 0;
        private List<DBOnlineSeries> mSeriesList = new List<DBOnlineSeries>();
        private List<int> mIncorrectIdsList = new List<int>();

        public long ServerTimeStamp
        {
            get { return mServerTimeStamp; }
        }

        public List<DBOnlineSeries> Results
        {
            get
            {
                if (mSeriesList == null)
                    mSeriesList = new List<DBOnlineSeries>(ResultsLazy);
                return mSeriesList;
            }
        }

        /// <summary>
        /// Lazily Evaluates
        /// </summary>
        public IEnumerable<DBOnlineSeries> ResultsLazy
        {
            get
            {
                foreach (string id in mSeriesIDs)
                {
                    var results = Work(id);
                    foreach (var r in results)
                        if(r != null && r[DBOnlineSeries.cID] > 0)
                          yield return r;
                }
            }
        }

        public List<int> BadIds
        {
            get { return mIncorrectIdsList; }
        }

        public UpdateSeries(String sSeriesID)
        {
            mSeriesList = Work(sSeriesID).ToList();
        }

        public UpdateSeries(List<String> sSeriesIDs)
        {
            this.mSeriesIDs = sSeriesIDs;            
        }

        public UpdateSeries(String sSeriesID, String languageID, bool aOverride = false )
        {
            mSeriesList = Work(sSeriesID, languageID, aOverride).ToList();
        }

        private IEnumerable<DBOnlineSeries> Work(String sSeriesID)
        {
            return Work(sSeriesID, "");
        }

        private IEnumerable<DBOnlineSeries> Work(String sSeriesID, String languageID, bool aOverride = false)
        {
            if (sSeriesID.Length > 0)
            {
                int result;
                if (int.TryParse(sSeriesID,out result))
                    MPTVSeriesLog.Write(string.Format("Retrieving updated Metadata for series {0}",Helper.getCorrespondingSeries(result)), MPTVSeriesLog.LogLevel.Debug);

                XmlNode node = null;
                if (String.IsNullOrEmpty(languageID))
                {
                    node = Online_Parsing_Classes.OnlineAPI.UpdateSeries(sSeriesID);
                }
                else
                {
                    node = Online_Parsing_Classes.OnlineAPI.UpdateSeries(sSeriesID, languageID, aOverride );
                }

                if (node != null)
                {
                    foreach (XmlNode itemNode in node.SelectNodes("/Data"))
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
                                if (series != null) mSeriesList.Add(series);
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
                        if(series != null) yield return series;
                    }
                }
            }
        }
    }
}

