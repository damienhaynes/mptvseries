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
        static List<String> sExpressions = new List<String>();
        static List<Regex> regularExpressions = new List<Regex>();
        //static List<DBReplacements> replacements = new List<DBReplacements>();
        static Dictionary<string, string> replacements = new Dictionary<string, string>();

        public Dictionary<string, string> Matches
        {
            get { return m_Matches;}
        }

        public String RegexpMatched
        {
            get { return m_RegexpMatched; }
        }

        /// <summary>
        /// Loads and compile Parsing Expressions and also String Replacements
        /// </summary>
        /// <returns></returns>
        public static bool reLoadExpressions()
        {
            // build a list of all the regular expressions to apply
            bool error = false;
            try
            {
                MPTVSeriesLog.Write("Compiling Regex...");
                sExpressions.Clear();
                regularExpressions.Clear();
                replacements.Clear();
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
                        sExpression = sExpression.Replace("<firstaired>", "<" + DBOnlineEpisode.cFirstAired + ">");

                        sExpressions.Add(sExpression);
                        // we precompile the expressions here which is faster in the end
                        regularExpressions.Add(new Regex(sExpression, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled));
                    }
                }
                MPTVSeriesLog.Write("Compiled Regex sucessfuly, " + sExpressions.Count.ToString() + " Expressions found");
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Error loading Parsing Expressions: " + ex.Message);
                error = true;
            }
                // now go for the replacements
            try
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

                    replacements.Add(searchString, replaceString);
                }
                return error;
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Error loading String Replacements: " + ex.Message);
                return false;
            }
        }

        public FilenameParser(string filename)
        {
            try
            {
                ////////////////////////////////////////////////////////////////////////////////////////////
                // Parsing filename for all recognized naming formats to extract episode information
                ////////////////////////////////////////////////////////////////////////////////////////////
                m_Filename = filename;
                if (sExpressions.Count == 0) reLoadExpressions();

                int index = 0;
                foreach(Regex regularExpression in regularExpressions)
                {
                    string _Pattern;
                    string _Source;
                    _Source = m_Filename;

                    Match matchResults;
                    matchResults = regularExpression.Match(_Source);

                    if (matchResults.Success)
                    {
                        for (int i = 1; i < matchResults.Groups.Count; i++)
                        {
                            string GroupName = regularExpression.GroupNameFromNumber(i);
                            string GroupValue = matchResults.Groups[i].Value;

                            if (GroupValue != "" && GroupName != "unknown")
                            {
                                foreach (KeyValuePair<string, string> replacement in replacements)
                                    GroupValue = GroupValue.Replace(replacement.Key, replacement.Value);

                                GroupValue = GroupValue.Trim();
                                m_Matches.Add(GroupName, GroupValue);
                            }
                        }
                        // stop on the first successful match
                        m_RegexpMatched = sExpressions[index];
                        return;
                    }
                  index++;
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
