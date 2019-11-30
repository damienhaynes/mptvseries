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
    class GetSeries
    {
        /// <summary>
        /// The maximum string Levenshtein Distance for an auto-match.
        /// </summary>
        public static int FuzzyMatching_MaxLSDistance = 2;

        public List<DBOnlineSeries> Results
        {
            get 
            {
                return mListSeries; 
            }
        }
        private List<DBOnlineSeries> mListSeries = new List<DBOnlineSeries>();

        public DBOnlineSeries PerfectMatch { get; protected set; }

        DBOnlineSeries RankSearchResults(string name, IList<DBOnlineSeries> candidates, out List<DBOnlineSeries> orderedCandidates)
        {
            // remove any non alphabet characters
            string cleanedName = name.ToLowerInvariant().Trim().RemoveSpecialCharacters();
            
            // calculate distances basis the series names
            var bestMatch = (from candidate in candidates
                             select new
                             {
                                 LSDistance = MediaPortal.Util.Levenshtein.Match(cleanedName, candidate[DBOnlineSeries.cPrettyName].ToString().ToLowerInvariant().RemoveSpecialCharacters()),
                                 Series = candidate
                             });

            // calculate distances basis series alias names
            bestMatch = bestMatch.Union(from candidate in candidates.Where(c => !string.IsNullOrEmpty(c[DBOnlineSeries.cAliasNames]))
                                        from alias in candidate[DBOnlineSeries.cAliasNames].ToString().Split('|')
                                        select new
                                        {
                                            LSDistance = MediaPortal.Util.Levenshtein.Match(cleanedName, alias.ToLowerInvariant().RemoveSpecialCharacters()),
                                            Series = candidate
                                        });

            // make results unique
            var uniqueResults = from candidate in bestMatch
                                group candidate by (int)candidate.Series[DBOnlineSeries.cID];

            // now order the series by their minLSDistance (each ID can have several series and thus names)
            // we dont care which one won, we just want the minimum it scored
            // we also pick out the series in the users language, and the English language
            var weightedUniqueResults = from ur in uniqueResults
                                        select new
                                        {
                                            MinLSDistance = ur.Min(r => r.LSDistance),
                                            SeriesScored = ur.OrderBy(r => r.LSDistance).FirstOrDefault(),
                                            SeriesUserLang = ur.FirstOrDefault(r => r.Series["language"] == r.Series[DBOnlineSeries.cLanguage]),
                                            SeriesEng = ur.FirstOrDefault(r => r.Series["language"] == "en"),
                                        };

            // now decide which one to display
            // 1) users language 2) English 3) whichever scored our best result, this has to exist
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

            // get the best one that's under a certain distance, which is a bit more fuzzy than the perfect requirement before
            if (Settings.isConfig || DBOption.GetOptions(DBOption.cAutoChooseSeries) == 1)
            {
                var best = weightedDisplayResults.FirstOrDefault(m => m.LSDistance <= FuzzyMatching_MaxLSDistance);
                if (best != null)
                    return best.Series;
            }
            return null;
        }

        public GetSeries(String aSeriesName)
        {
            string lUserLanguage = DBOption.GetOptions( DBOption.cOnlineLanguage );

            // search for series basis the user's language
            XmlNode lNode = Online_Parsing_Classes.OnlineAPI.GetSeries( aSeriesName, lUserLanguage );
            if ( lNode == null ) return;

            // if we have no results from the user's language try English
            if ( lNode.ChildNodes.Count == 0 && lUserLanguage != "en" )
            {
                lNode = Online_Parsing_Classes.OnlineAPI.GetSeries( aSeriesName, "en" );
                if ( lNode == null ) return;
            }

            foreach ( XmlNode itemNode in lNode.ChildNodes)
            {
                var lSeries = new DBOnlineSeries();
                    
                foreach (XmlNode propertyNode in itemNode.ChildNodes)
                {
                    if ( propertyNode.Name == "seriesid" ) // work around SeriesID inconsistancy
                    {
                        lSeries[DBOnlineSeries.cID] = propertyNode.InnerText;
                    }
                    else if ( DBOnlineSeries.s_OnlineToFieldMap.ContainsKey( propertyNode.Name ) )
                    {
                        lSeries[DBOnlineSeries.s_OnlineToFieldMap[propertyNode.Name]] = propertyNode.InnerText;
                    }
                    else
                    {
                        // we don't know that field, add it to the series table
                        lSeries.AddColumn( propertyNode.Name, new DBField( DBField.cTypeString ) );
                        lSeries[propertyNode.Name] = propertyNode.InnerText;
                    }
                }
                mListSeries.Add(lSeries);
            }

            PerfectMatch = RankSearchResults( aSeriesName, mListSeries, out mListSeries );            
        }
    }
}
