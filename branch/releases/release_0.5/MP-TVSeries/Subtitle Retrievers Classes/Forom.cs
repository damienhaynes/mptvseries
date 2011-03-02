using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace WindowPlugins.GUITVSeries
{
    public class Levenshtein
    {
        /// <summary>
        /// Compute Levenshtein distance.
        /// http://www.merriampark.com/ld.htm
        /// </summary>
        /// <returns>Distance between the two strings.
        /// The larger the number, the bigger the difference.
        /// </returns>
        public static int CalcEditDistance(string s, string t)
        {
            int n = s.Length; //length of s
            int m = t.Length; //length of t
            int[,] d = new int[n + 1, m + 1]; // matrix
            int cost; // cost
            // Step 1
            if (n == 0) return m;
            if (m == 0) return n;
            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 0; j <= m; d[0, j] = j++) ;
            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    cost = (t.Substring(j - 1, 1) == s.Substring(i - 1, 1) ? 0 : 1);
                    // Step 6
                    d[i, j] = System.Math.Min(System.Math.Min(d[i - 1, j] + 1, d[i, j - 1] +
                    1),
                    d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
    }

    class Forom
    {
        String m_sBaseUrl = String.Empty;
        String m_sID = String.Empty;

        public Forom()
        {
            m_sBaseUrl = DBOption.GetOptions(DBOption.cSubs_Forom_BaseURL);
            m_sID = DBOption.GetOptions(DBOption.cSubs_Forom_ID);
        }

        public bool GetSubs(DBEpisode episode)
        {
//             try
//             {
//                 episode[DBOnlineEpisode.] = "the simpsons";
//                 episode[DBEpisode.cSeasonIndex] = 16;
//                 
//                 String sLal = String.Empty;
//                 String sLocalSeriesName = episode[DBOnlineEpisode.cSeriesParsedName];
//                 if ((sLocalSeriesName[0] >= '0' && sLocalSeriesName[0] <= '9') || (sLocalSeriesName[0] >= 'a' && sLocalSeriesName[0] <= 'f'))
//                 {
//                     sLal = "1F";
//                 }
//                 else if (sLocalSeriesName[0] >= 'g' && sLocalSeriesName[0] <= 'l')
//                 {
//                     sLal = "GL";
//                 }
//                 else if (sLocalSeriesName[0] >= 'm' && sLocalSeriesName[0] <= 's')
//                 {
//                     sLal = "MS";
//                 }
//                 else if (sLocalSeriesName[0] >= 't' && sLocalSeriesName[0] <= 'z')
//                 {
//                     sLal = "TZ";
//                 }
// 
//                 if (sLal != String.Empty && m_sID != String.Empty)
//                 {
//                     String s1stLevelURL = String.Format(@"{0}?lal={1}&c={2}", m_sBaseUrl, sLal, m_sID);
//                     WebClient client = new WebClient();
//                     Stream data = client.OpenRead(s1stLevelURL);
//                     StreamReader reader = new StreamReader(data);
//                     String sPage = reader.ReadToEnd().Replace('\0', ' ');
//                     data.Close();
//                     reader.Close();
// 
//                     String RegExp = String.Format("<td class=\"menu1\">(?:&nbsp;)*([^<]*?)</td>.*?href=\\\"([^\"]*?indexb[^\"]*?{0}[^\"]*?)\\\"", m_sID);
//                     Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
//                     MatchCollection matches = Engine.Matches(sPage);
//                     List<ForomMatchResult> sortedMatchList = new List<ForomMatchResult>();
//                     foreach (Match match in matches)
//                     {
//                         ForomMatchResult result = new ForomMatchResult(match.Groups[1].Value, match.Groups[2].Value);
//                         // first pass, don't take in account a possible season number in the name
//                         result.ComputeDistance(episode);
//                         sortedMatchList.Add(result);
//                     }
// 
//                     sortedMatchList.Sort();
// 
//                     List<ForomMatchResult> exactMatches = new List<ForomMatchResult>();
//                     if (sortedMatchList.Count > 0)
//                     {
//                         foreach (ForomMatchResult result in sortedMatchList)
//                         {
//                             if (result.nDistance == 0)
//                                 exactMatches.Add(result);
//                         }
//                     }
// 
//                     if (exactMatches.Count > 0)
//                     {
//                         foreach (ForomMatchResult result in exactMatches)
//                         {
//                             if (episode[DBEpisode.cSeasonIndex] >= result.nSeasonMin && episode[DBEpisode.cSeasonIndex] <= result.nSeasonMax)
//                             {
//                                 // we found the right one without doubt. Let's go in !!!
//                                 MPTVSeriesLog.Log_Write(String.Format("{0}: Found {1} (season {2} to {3})", result.nDistance, result.sSubFullName, result.nSeasonMin, result.nSeasonMax));
//                             }
//                         }
//                     }
//                 }
//                 return false;
//             }
// 
//             catch (Exception e)
            {
                return false;
            }
        }
    }

    class ForomMatchResult : IComparable<ForomMatchResult>
    {
        public String sSubFullName = String.Empty;
        public String sSubName = String.Empty;

        public String sSubLink = String.Empty;
        public String sLanguage = String.Empty;
        public String sSubLinkName = String.Empty;
        public int nSeasonMin = 0xFFFF;
        public int nSeasonMax = 0;

        // for sorting
        public int nDistance = 0xFFFF;

        public int CompareTo(ForomMatchResult other)
        {
            return nDistance.CompareTo(other.nDistance);
        }

        public ForomMatchResult(String sName, String sLink)
        {
            sSubFullName = sName.ToLower();
            sSubName = sSubFullName;
            sSubLink = sLink;
            // extract language & name
            String RegExp = "\\?lg=([^&]*?)&type=(.*?)&";
            Regex subEngine = new Regex(RegExp, RegexOptions.IgnoreCase);
            Match subMatch = subEngine.Match(sSubLink);
            if (subMatch.Success)
            {
                sLanguage = subMatch.Groups[1].Value;
                sSubLinkName = subMatch.Groups[2].Value.ToLower();
            }

            if (sSubFullName.Contains("saison"))
            {
                String sSeasonDesc = sSubFullName.Substring(sSubFullName.IndexOf("saison") + 6);
                sSubName = sSubFullName.Substring(0, sSubFullName.IndexOf("saison")).Trim();

                RegExp = @"\d+";
                subEngine = new Regex(RegExp, RegexOptions.IgnoreCase);
                MatchCollection matches = subEngine.Matches(sSeasonDesc);
                foreach (Match match in matches)
                {
                    int nCurrent = Convert.ToInt32(match.Value);
                    nSeasonMin = Math.Min(nCurrent, nSeasonMin);
                    nSeasonMax = Math.Max(nCurrent, nSeasonMax);
                }
            }
            else
            {
                // no season => assume season 1
                nSeasonMin = 1;
                nSeasonMax = 1;
            }
        }

        public void ComputeDistance(DBEpisode episode)
        {
//            nDistance = Levenshtein.CalcEditDistance(sSubName, episode[DBEpisode.cSeriesParsedName]);
        }
    };
}
