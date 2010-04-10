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
using System.Linq;
using WindowPlugins.GUITVSeries.DataBase;

namespace WindowPlugins.GUITVSeries
{
    class GetSeries
    {
        /// <summary>
        /// The maximum string Levenshtein Distance for an auto-match.
        /// This could be tweaked.
        /// </summary>
        public static int FuzzyMatching_MaxLSDistance = 2;

        private List<DBOnlineSeries> listSeries = new List<DBOnlineSeries>();
        private string nameToMatch = null;

        public List<DBOnlineSeries> Results
        {
            get 
            {
                return listSeries; 
            }
        }

        public DBOnlineSeries PerfectMatch { get; protected set; }

        DBOnlineSeries RankSearchResults(string name, IList<DBOnlineSeries> candidates, out List<DBOnlineSeries> orderedCandidates)
        {
            string cleanedName = name.ToLowerInvariant().Trim().CleanStringOfSpecialChars();
            
            // calculate distances
            var bestMatch = (from candidate in candidates
                             select new
                             {
                                 LSDistance = MediaPortal.Util.Levenshtein.Match(cleanedName, candidate[DBOnlineSeries.cPrettyName].ToString().ToLowerInvariant().CleanStringOfSpecialChars()),
                                 Series = candidate
                             });

            // make them unique
            // note: this is different from onlineparse, should probably pick one implementation (read: this one!)           
            // old implementation disabled as its done here now
            var uniqueResults = from candidate in bestMatch
                                group candidate by (int)candidate.Series[DBOnlineSeries.cID];

            // now order the series by their minLSDistance (each ID can have several series and thus names)
            // we dont care which one won, we just want the minimum it scored
            // we also pick out the series in the users lang, and the englis lang
            var weightedUniqueResults = from ur in uniqueResults
                                        select new
                                        {
                                            MinLSDistance = ur.Min(r => r.LSDistance),
                                            SeriesScored = ur.OrderBy(r => r.LSDistance).FirstOrDefault(),
                                            SeriesUserLang = ur.FirstOrDefault(r => r.Series["language"] == Online_Parsing_Classes.OnlineAPI.SelLanguageAsString),
                                            SeriesEng = ur.FirstOrDefault(r => r.Series["language"] == "en"),
                                        };

            // now decide which one to display
            // 1) userlang 2) english 3) whichever scored our best result, this has to exist
            var weightedDisplayResults = from dr in weightedUniqueResults
                                         orderby dr.MinLSDistance
                                         select new
                                         {
                                             LSDistance = dr.MinLSDistance,
                                             Series = (dr.SeriesUserLang != null && dr.SeriesUserLang.Series != null) ? dr.SeriesUserLang.Series
                                                        : (dr.SeriesEng != null && dr.SeriesEng.Series != null) ? dr.SeriesEng.Series
                                                        : dr.SeriesScored.Series,
                                         };

            // give the ordered results back (for displaying)
            orderedCandidates = weightedDisplayResults.Select(r => r.Series).ToList();

            // get the best one thats under a certain distance, which is a bit more fuzzy than the perfect requirement before            
            if (Settings.isConfig || DBOption.GetOptions(DBOption.cAutoChooseSeries) == 1)
            {
                var best = weightedDisplayResults.FirstOrDefault(m => m.LSDistance <= FuzzyMatching_MaxLSDistance);
                if (best != null)
                    return best.Series;
            }
            return null;
        }

        public GetSeries(String sSeriesName)
        {
            XmlNode node = Online_Parsing_Classes.OnlineAPI.GetSeries(sSeriesName);

            nameToMatch = sSeriesName;
            if (node != null)
            {
                foreach (XmlNode itemNode in node.ChildNodes)
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
                            series.AddColumn(propertyNode.Name, new DBField(DBFieldValueType.String));
                            series[propertyNode.Name] = propertyNode.InnerText;
                        }
                    }
                    listSeries.Add(series);                    
                }

                PerfectMatch = RankSearchResults(nameToMatch, listSeries, out listSeries);
            }
        }

    }
}
