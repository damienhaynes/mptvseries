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
    class GetSeries
    {
        private List<DBOnlineSeries> listSeries = new List<DBOnlineSeries>();
        private bool sorted = false;
        private string nameToMatch = null;

        public List<DBOnlineSeries> Results
        {
            get 
            {
                if (!sorted) sortLD();
                return listSeries; 
            }
        }

        private void sortLD()
        {
            // use LD to sort to most likely
            listSeries.Sort(delegate(DBOnlineSeries i1, DBOnlineSeries i2)
            {
                return MediaPortal.Util.Levenshtein.Match(i1[DBOnlineSeries.cPrettyName], nameToMatch)
                      .CompareTo(MediaPortal.Util.Levenshtein.Match(i2[DBOnlineSeries.cPrettyName], nameToMatch));
            });
        }

        public GetSeries(String sSeriesName)
        {
            XmlNodeList nodeList = Online_Parsing_Classes.OnlineAPI.GetSeries(sSeriesName);
            nameToMatch = sSeriesName;
            if (nodeList != null)
            {
                foreach (XmlNode itemNode in nodeList)
                {
                    DBOnlineSeries series = new DBOnlineSeries();
                    
                    foreach (XmlNode propertyNode in itemNode.ChildNodes)
                    {
                        if (propertyNode.Name == "seriesid") // work around SeriesID inconsistancy
                        {
                            series[DBOnlineSeries.cID] = propertyNode.InnerText;
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
                    listSeries.Add(series);
                }
            }
        }

    }
}
