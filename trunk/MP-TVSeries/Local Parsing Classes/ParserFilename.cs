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
using System.Text.RegularExpressions;

namespace WindowPlugins.GUITVSeries
{
    public class FilenameParser
    {
        private string m_Filename = string.Empty;
        private Dictionary<string, string> m_Matches = new Dictionary<string, string>();
        private String m_RegexpMatched = string.Empty;

        public Dictionary<string, string> Matches
        {
            get { return m_Matches;}
        }

        public String RegexpMatched
        {
            get { return m_RegexpMatched; }
        }

        public FilenameParser(string filename)
        {
            try
            {
                ////////////////////////////////////////////////////////////////////////////////////////////
                // Parsing filename for all recognized naming formats to extract episode information
                ////////////////////////////////////////////////////////////////////////////////////////////
                m_Filename = filename;

                // build a list of all the regular expressions to apply
                List<String> sExpressions = new List<String>();
                DBExpression[] expressions = DBExpression.GetAll();
                foreach (DBExpression expression in expressions)
                {
                    if (expression[DBExpression.cEnabled] != 0)
                    {
                        String sExpression = String.Empty;
                        switch ((String)expression[DBExpression.cType])
                        {
                            case DBExpression.cType_Simple:
                                sExpression = ConvertSimpleExpressionToRegEx(expression[DBExpression.cExpression]);
                                break;

                            case DBExpression.cType_Regexp:
                                sExpression = expression[DBExpression.cExpression];
                                break;
                        }
                        sExpression = sExpression.ToLower();
                        // replace series, season and episode by the valid DBEpisode column names
                        sExpression = sExpression.Replace("<series>", "<" + DBSeries.cParsedName + ">");
                        sExpression = sExpression.Replace("<season>", "<" + DBEpisode.cSeasonIndex + ">");
                        sExpression = sExpression.Replace("<episode>", "<" + DBEpisode.cEpisodeIndex + ">");
                        sExpression = sExpression.Replace("<episode2>", "<" + DBEpisode.cEpisodeIndex2 + ">");
                        sExpression = sExpression.Replace("<title>", "<" + DBEpisode.cEpisodeName + ">");

                        sExpressions.Add(sExpression);
                    }
                }

                foreach (String pattern in sExpressions)
                {
                    string _Pattern;
                    string _Source;

                    Dictionary<string, string> episodeMetaData = new Dictionary<string, string>();

                    _Pattern = pattern;
                    _Source = m_Filename;

                    Match matchResults;
                    Regex regularExpression = new Regex(_Pattern, RegexOptions.IgnoreCase  | RegexOptions.ExplicitCapture);

                    matchResults = regularExpression.Match(_Source);

                    if (matchResults.Success)
                    {
                        for (int i = 1; i < matchResults.Groups.Count; i++)
                        {
                            string GroupName = regularExpression.GroupNameFromNumber(i);
                            string GroupValue = matchResults.Groups[i].Value;

                            if (GroupValue != "" && GroupName != "unknown")
                            {
                                foreach (DBReplacements replacement in DBReplacements.GetAll())
                                {
                                    String searchString = replacement[DBReplacements.cToReplace];
                                    searchString = searchString
                                        .ToLower()
                                        .Replace("<space>", " ");

                                    String replaceString = replacement[DBReplacements.cWith];
                                    replaceString = replaceString
                                        .ToLower()
                                        .Replace("<space>", " ")
                                        .Replace("<empty>", "");

                                    GroupValue = GroupValue.Replace(searchString, replaceString);
                                }

                                GroupValue = GroupValue.Trim();
                                m_Matches.Add(GroupName, GroupValue);
                            }
                        }
                        // stop on the first successful match
                        m_RegexpMatched = _Pattern;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("And error occured in the 'FilenameParser' function (" + ex.ToString() + ")");
            }
        }

        private static string ConvertSimpleExpressionToRegEx(string SimpleExpression)
        {
            string field = "";
            string finalRegEx = "";
            int openTagLocation = -1;
            int closeTagLocation = 0;

            SimpleExpression = SimpleExpression.Replace(@"\", @"\\");
            SimpleExpression = SimpleExpression.Replace(".", @"\.");


            while (true)
            {
                openTagLocation = SimpleExpression.IndexOf('<', closeTagLocation);

                if (openTagLocation == -1)
                {
                    if (closeTagLocation > 0)
                        finalRegEx += SimpleExpression.Substring(closeTagLocation + 1);
                    else
                        finalRegEx += SimpleExpression;

                    break;
                }

                if (closeTagLocation == 0)
                    finalRegEx = SimpleExpression.Substring(0, openTagLocation);
                else
                    finalRegEx += SimpleExpression.Substring(closeTagLocation + 1, openTagLocation - closeTagLocation - 1);

                closeTagLocation = SimpleExpression.IndexOf('>', openTagLocation);

                field = SimpleExpression.Substring(openTagLocation + 1, closeTagLocation - openTagLocation - 1);

                if (field != "")
                {
                    // other tags coming? put lazy *, otherwise put a greedy one
                    if (SimpleExpression.IndexOf('<', closeTagLocation) != -1)
                        finalRegEx += String.Format(@"(?<{0}>[^\\]*?)", field);
                    else
                        finalRegEx += String.Format(@"(?<{0}>[^\\]*)", field);
                }
                else
                {
                    // other tags coming? put lazy *, otherwise put a greedy one
                    if (SimpleExpression.IndexOf('<', closeTagLocation) != -1)
                        finalRegEx += @"(?:[^\\]*?)";
                    else
                        finalRegEx += @"(?:[^\\]*)";
                }
            }

            return finalRegEx;
        }
    }
}
