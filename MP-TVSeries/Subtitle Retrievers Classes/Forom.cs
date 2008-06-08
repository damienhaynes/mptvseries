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

#if inclDownloaders
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.ComponentModel;
using ICSharpCode.SharpZipLib.Zip;

namespace WindowPlugins.GUITVSeries.Subtitles
{
    class Forom
    {
        public BackgroundWorker worker = null;

        String m_sBaseUrl = String.Empty;
        String m_sID = String.Empty;
        DBEpisode m_dbEpisode = null;
        bool m_bSubtitleRetrieved = false;
        Feedback.IFeedback m_feedback = null;

        public delegate void SubtitleRetrievalCompletedHandler(bool bFound);
        /// <summary>
        /// This will be triggered once all the SeriesAndEpisodeInfo has been parsed completely.
        /// </summary>
        public event SubtitleRetrievalCompletedHandler SubtitleRetrievalCompleted;

        public Forom(Feedback.IFeedback feedback)
        {
            m_sBaseUrl = DBOption.GetOptions(DBOption.cSubs_Forom_BaseURL);
            m_sID = DBOption.GetOptions(DBOption.cSubs_Forom_ID);
            m_feedback = feedback;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (SubtitleRetrievalCompleted != null) // only if any subscribers exist
            {
                this.SubtitleRetrievalCompleted.Invoke(m_bSubtitleRetrieved);
            }
        }

        public void GetSubs(DBEpisode dbEpisode)
        {
            m_dbEpisode = dbEpisode;
            worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            MPTVSeriesLog.Write("**********************************");
            MPTVSeriesLog.Write("Starting FOROM Subtitles retrieval");
            MPTVSeriesLog.Write("**********************************");

            System.Threading.Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            try
            {
                WebClient client = new WebClient();

                DBOnlineSeries series = new DBOnlineSeries(m_dbEpisode[DBEpisode.cSeriesID]);
                DBSeason season = new DBSeason(m_dbEpisode[DBEpisode.cSeriesID], m_dbEpisode[DBEpisode.cSeasonIndex]);
                ForomSubtitleEpisode episode = new ForomSubtitleEpisode(series[DBOnlineSeries.cOriginalName], m_dbEpisode[DBEpisode.cFilename], m_dbEpisode[DBEpisode.cSeasonIndex], m_dbEpisode[DBEpisode.cEpisodeIndex]);
                ForomSeasonMatchResult finalSeasonResult = null;

                String sLal = String.Empty;
                String sLocalSeriesName = episode.m_sSeriesName;
                if ((sLocalSeriesName[0] >= '0' && sLocalSeriesName[0] <= '9') || (sLocalSeriesName[0] >= 'a' && sLocalSeriesName[0] <= 'f'))
                {
                    sLal = "1F";
                }
                else if (sLocalSeriesName[0] >= 'g' && sLocalSeriesName[0] <= 'l')
                {
                    sLal = "GL";
                }
                else if (sLocalSeriesName[0] >= 'm' && sLocalSeriesName[0] <= 's')
                {
                    sLal = "MS";
                }
                else if (sLocalSeriesName[0] >= 't' && sLocalSeriesName[0] <= 'z')
                {
                    sLal = "TZ";
                }

                if (sLal.Length > 0 && m_sID.Length > 0)
                {
                    String s1stLevelURL = String.Format(@"{0}/index.php?lal={1}&c={2}", m_sBaseUrl, sLal, m_sID);
                    MPTVSeriesLog.Write("Step 1: looking into " + s1stLevelURL);
                    Stream data = client.OpenRead(s1stLevelURL);
                    StreamReader reader = new StreamReader(data);
                    String sPage = reader.ReadToEnd().Replace('\0', ' ');
                    data.Close();
                    reader.Close();

                    String RegExp = String.Format("<td class=\"menu1\">(?:&nbsp;)*([^<]*?)</td>.*?href=\\\"([^\"]*?indexb[^\"]*?{0}[^\"]*?)\\\"", m_sID);
                    Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
                    MatchCollection matches = Engine.Matches(sPage);
                    List<ForomSeasonMatchResult> sortedMatchList = new List<ForomSeasonMatchResult>();
                    foreach (Match match in matches)
                    {
                        ForomSeasonMatchResult result = new ForomSeasonMatchResult(match.Groups[1].Value, match.Groups[2].Value);
                        // first pass, don't take in account a possible season number in the name
                        result.ComputeDistance(episode);
                        sortedMatchList.Add(result);
                    }

                    sortedMatchList.Sort();

                    List<ForomSeasonMatchResult> exactMatches = new List<ForomSeasonMatchResult>();
                    if (sortedMatchList.Count > 0)
                    {
                        MPTVSeriesLog.Write(String.Format("Found {0} series/season entries in the page", sortedMatchList.Count));
                        foreach (ForomSeasonMatchResult result in sortedMatchList)
                        {
                            if (result.nDistance == 0)
                                exactMatches.Add(result);
                        }
                    }

                    bool bOver = false;
                    while (!bOver)
                    {
                        MPTVSeriesLog.Write(String.Format("Found {0} exact matches in the page", exactMatches.Count));
                        if (exactMatches.Count > 0)
                        {
                            foreach (ForomSeasonMatchResult result in exactMatches)
                            {
                                if (episode.m_nSeasonIndex >= result.nSeasonMin && episode.m_nSeasonIndex <= result.nSeasonMax)
                                {
                                    // we found the right one without doubt. Let's go in !!!
                                    MPTVSeriesLog.Write(String.Format("{0}: Found {1} (season {2} to {3})", result.nDistance, result.sSubFullName, result.nSeasonMin, result.nSeasonMax));
                                    finalSeasonResult = result;
                                    bOver = true;
                                }
                            }
                        }
                        else
                        {
                            MPTVSeriesLog.Write("Choosing the series/season from a list");
                            // show the user the list and ask for the right one
                            List<Feedback.CItem> Choices = new List<Feedback.CItem>();
                            foreach (ForomSeasonMatchResult match in sortedMatchList)
                            {
                                if (match.nSeasonMin == match.nSeasonMax)
                                    Choices.Add(new Feedback.CItem(match.sSubName + " season " + match.nSeasonMin, String.Empty, match));
                                else
                                    Choices.Add(new Feedback.CItem(match.sSubName + " seasons between " + match.nSeasonMin + " and " + match.nSeasonMax, String.Empty, match));
                            }
                            Feedback.ChooseFromSelectionDescriptor descriptor = new Feedback.ChooseFromSelectionDescriptor();
                            descriptor.m_sTitle = "Choose correct series / season item";
                            descriptor.m_sItemToMatchLabel = "Local series:";
                            descriptor.m_sItemToMatch = episode.m_sSeriesName + " season " + episode.m_nSeasonIndex;
                            descriptor.m_sListLabel = "Available series / seasons list:";
                            descriptor.m_List = Choices;
                            descriptor.m_sbtnIgnoreLabel = String.Empty;

                            Feedback.CItem Selected = null;
                            if (m_feedback.ChooseFromSelection(descriptor, out Selected) == Feedback.ReturnCode.OK)
                            {
                                finalSeasonResult = Selected.m_Tag as ForomSeasonMatchResult;
                            }
                            bOver = true;
                        }
                    }
                }
                else
                {
                    String error = String.Format("Error, empty parameter (Lal={0} & ID={1})", sLal, m_sID);
                    MPTVSeriesLog.Write(error);
                    throw new Exception(error);
                }

                // now, retrieve the subtitle for this episode (try VF first, then VO if no VF found)
                if (finalSeasonResult != null)
                {
                    List<ForomEpisodeMatchResult> matchList = new List<ForomEpisodeMatchResult>();
                    String sLang = String.Empty; // VF
                    do
                    {
                        String s2ndLevelURL = String.Empty;
                        if (finalSeasonResult.sPHPSessionID.Length > 0)
                            s2ndLevelURL = String.Format(@"{0}/indexb.php?lg={1}&type={2}&c={3}&PHPSESSID={4}", m_sBaseUrl, sLang, finalSeasonResult.sSubLinkName, m_sID, finalSeasonResult.sPHPSessionID);
                        else
                            s2ndLevelURL = String.Format(@"{0}/indexb.php?lg={1}&type={2}&c={3}", m_sBaseUrl, sLang, finalSeasonResult.sSubLinkName, m_sID);

                        MPTVSeriesLog.Write("Step 2: looking into " + s2ndLevelURL);
                        Stream data = client.OpenRead(s2ndLevelURL);
                        StreamReader reader = new StreamReader(data);
                        String sPage = reader.ReadToEnd().Replace('\0', ' ');
                        data.Close();
                        reader.Close();

                        String RegExp = "<tr align=\"left\"[^>]*?><[^>]*?>[^>]*?<a href=\"(?<link>[^\"]*?)\"[^>]*?>(?<name>[^<]*?)</a></td>";
                        Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
                        MatchCollection matches = Engine.Matches(sPage);
                        if (matches.Count == 0)
                        {
                            MPTVSeriesLog.Write("Error: no episodes found in the series/season page");
                        }
                        foreach (Match match in matches)
                        {
                            ForomEpisodeMatchResult result = new ForomEpisodeMatchResult(match.Groups["name"].Value, match.Groups["link"].Value);
                            // match season index & episode index
                            if (result.m_nSeasonIndex == episode.m_nSeasonIndex && result.m_nEpisodeIndex == episode.m_nEpisodeIndex)
                            {
                                MPTVSeriesLog.Write(String.Format("Found a matching episode ({0})", result.m_sName));
                                result.ComputeDistance(episode);
                                bool bFound = false;
                                foreach (ForomEpisodeMatchResult matchFind in matchList)
                                    if (matchFind.m_sName == result.m_sName)
                                    {
                                        bFound = true;
                                        break;
                                    }
                                if (!bFound)
                                    matchList.Add(result);
                            }
                        }

                        if (sLang.Length == 0)
                            sLang = "VO";
                        else
                            break;
                    }
                    while (true);

                    MPTVSeriesLog.Write(String.Format("{0} matching subtitles Found", matchList.Count));

                    List<ForomEpisodeMatchResult> sortedMatchList = new List<ForomEpisodeMatchResult>();

                    // process rars or zips if any
                    foreach (ForomEpisodeMatchResult result in matchList)
                    {
                        String RegExp = @".*\.(.*)";
                        Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
                        Match match = Engine.Match(result.m_sName);
                        if (match.Success)
                        {
                            if (match.Groups[1].Value == "rar")
                            {
                                // we need to download those somewhere, unpack, remove the entry from the list & replace it with the files in the archive
                                client.DownloadFile(result.m_sLink, System.IO.Path.GetTempPath() + result.m_sName);
                                Unrar unrar = new Unrar();
                                unrar.ArchiveName = System.IO.Path.GetTempPath() + result.m_sName;
                                List<String> fileList = unrar.FileNameList;
                                MPTVSeriesLog.Write(String.Format("Decompressing archive {0} : {1} files", result.m_sName, fileList.Count));
                                foreach (String file in fileList)
                                {
                                    if (unrar.Extract(file, System.IO.Path.GetTempPath()))
                                    {
                                        ForomEpisodeMatchResult extractedFile = new ForomEpisodeMatchResult(file, "file://" + System.IO.Path.GetTempPath() + file);
                                        extractedFile.ComputeDistance(episode);
                                        sortedMatchList.Add(extractedFile);
                                    }
                                }
                            }
                            else if (match.Groups[1].Value == "zip")
                            {
                                // we need to download those somewhere, unpack, remove the entry from the list & replace it with the files in the archive
                                client.DownloadFile(result.m_sLink, System.IO.Path.GetTempPath() + result.m_sName);

                                using (ZipInputStream s = new ZipInputStream(File.OpenRead(System.IO.Path.GetTempPath() + result.m_sName)))
                                {
                                    ZipEntry theEntry;
                                    while ((theEntry = s.GetNextEntry()) != null)
                                    {
                                        Console.WriteLine(theEntry.Name);

                                        string directoryName = Path.GetDirectoryName(theEntry.Name);
                                        string fileName = Path.GetFileName(theEntry.Name);

                                        // create directory
                                        if (directoryName.Length > 0)
                                        {
                                            Directory.CreateDirectory(System.IO.Path.GetTempPath() + directoryName);
                                        }

                                        if (fileName.Length > 0 && fileName[0] != '.')
                                        {
                                            using (FileStream streamWriter = File.Create(System.IO.Path.GetTempPath() + theEntry.Name))
                                            {

                                                int size = 2048;
                                                byte[] data = new byte[2048];
                                                while (true)
                                                {
                                                    size = s.Read(data, 0, data.Length);
                                                    if (size > 0)
                                                    {
                                                        streamWriter.Write(data, 0, size);
                                                    }
                                                    else
                                                    {
                                                        break;
                                                    }
                                                }
                                            }
                                            ForomEpisodeMatchResult extractedFile = new ForomEpisodeMatchResult(fileName, "file://" + System.IO.Path.GetTempPath() + theEntry.Name);
                                            extractedFile.ComputeDistance(episode);
                                            sortedMatchList.Add(extractedFile);
                                        }
                                    }
                                }
                            }
                            else
                                sortedMatchList.Add(result);
                        }
                    }

                    sortedMatchList.Sort();

                    if (sortedMatchList.Count != 0)
                    {
                        // we need at least some matches
                        ForomEpisodeMatchResult finalEpisodeResult = null;
                        // now, sort & take the first one as our best result
                        List<Feedback.CItem> Choices = new List<Feedback.CItem>();
                        foreach (ForomEpisodeMatchResult result in sortedMatchList)
                        {
                            // arbitrary value - assume it's the right sub 
                            if (result.m_nDistance < 6)
                            {
                                finalEpisodeResult = result;
                                break;
                            }
                            Choices.Add(new Feedback.CItem(result.m_sName, String.Empty, result));
                        }

                        if (finalEpisodeResult == null)
                        {
                            // ask the user
                            String RegExp = @".*\\(.*)";
                            Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
                            Match match = Engine.Match(episode.m_sFileName);
                            if (match.Success)
                            {
                                // has to be 
                                Feedback.ChooseFromSelectionDescriptor descriptor = new Feedback.ChooseFromSelectionDescriptor();
                                descriptor.m_sTitle = "Select matching subtitle file";
                                descriptor.m_sItemToMatchLabel = "filename:";
                                descriptor.m_sItemToMatch = match.Groups[1].Value;
                                descriptor.m_sListLabel = "Matching subtitles:";
                                descriptor.m_List = Choices;
                                descriptor.m_sbtnIgnoreLabel = String.Empty;

                                Feedback.CItem Selected = null;
                                if (m_feedback.ChooseFromSelection(descriptor, out Selected) == Feedback.ReturnCode.OK)
                                {
                                    finalEpisodeResult = Selected.m_Tag as ForomEpisodeMatchResult;
                                }
                            }
                        }

                        if (finalEpisodeResult != null)
                        {
                            // we have it!!! download, store in the right place & rename accordingly
                            String RegExp = @"(.*)\..*";
                            Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
                            Match matchNameNoExt = Engine.Match(episode.m_sFileName);

                            RegExp = @".*\.(.*)";
                            Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
                            Match matchExt = Engine.Match(finalEpisodeResult.m_sName);
                            if (matchExt.Success && matchNameNoExt.Success)
                            {
                                client.DownloadFile(finalEpisodeResult.m_sLink, matchNameNoExt.Groups[1].Value + "." + matchExt.Groups[1].Value);
                                m_bSubtitleRetrieved = true;
                                m_dbEpisode[DBEpisode.cAvailableSubtitles] = 1;
                                m_dbEpisode.Commit();
                            }
                        }
                    }
                    else
                    {
                        // no match found
                        MPTVSeriesLog.Write(String.Format("No matching episode subtitles found!"));
                    }

                    // cleanup temp files 
                    foreach (ForomEpisodeMatchResult result in matchList)
                    {
                        String RegExp = @".*\.(.*)";
                        Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
                        Match match = Engine.Match(result.m_sName);
                        if (match.Success)
                        {
                            if (match.Groups[1].Value == "rar")
                            {
                                Unrar unrar = new Unrar();
                                unrar.ArchiveName = System.IO.Path.GetTempPath() + result.m_sName;
                                List<String> fileList = unrar.FileNameList;
                                foreach (String file in fileList)
                                    System.IO.File.Delete(System.IO.Path.GetTempPath() + file);

                                System.IO.File.Delete(System.IO.Path.GetTempPath() + result.m_sName);
                            }
                            else if (match.Groups[1].Value == "zip")
                            {
                                // delete files
                                using (ZipInputStream s = new ZipInputStream(File.OpenRead(System.IO.Path.GetTempPath() + result.m_sName)))
                                {
                                    ZipEntry theEntry;
                                    while ((theEntry = s.GetNextEntry()) != null)
                                    {
                                        string fileName = Path.GetFileName(theEntry.Name);
                                        if (fileName.Length > 0)
                                        {
                                            try { System.IO.File.Delete(System.IO.Path.GetTempPath() + theEntry.Name); }
                                            catch { }
                                        }
                                    }
                                }

                                // and folders
                                using (ZipInputStream s = new ZipInputStream(File.OpenRead(System.IO.Path.GetTempPath() + result.m_sName)))
                                {
                                    ZipEntry theEntry;
                                    while ((theEntry = s.GetNextEntry()) != null)
                                    {
                                        string directoryName = Path.GetDirectoryName(theEntry.Name);
                                        string fileName = Path.GetFileName(theEntry.Name);
                                        if (fileName.Length == 0)
                                        {
                                            try { System.IO.Directory.Delete(System.IO.Path.GetTempPath() + directoryName, true); }
                                            catch { }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    MPTVSeriesLog.Write("*******************************");
                    MPTVSeriesLog.Write("FOROM Subtitles retrieval ended");
                    MPTVSeriesLog.Write("*******************************");
                }
            }

            catch(Exception ex)
            {
                MPTVSeriesLog.Write("Could not do Forom Subtitle retrival: " + ex.Message, MPTVSeriesLog.LogLevel.Normal);
            }
        }
    }

    public class ForomSubtitleEpisode
    {
        public String m_sSeriesName = String.Empty;
        public String m_sFileName = String.Empty;
        public int m_nSeasonIndex = 0;
        public int m_nEpisodeIndex = 0;

      public ForomSubtitleEpisode(String sSeriesName, String sFileName, int nSeasonIndex, int nEpisodeIndex)
        {
            m_sSeriesName = sSeriesName.ToLower();
            m_sFileName = sFileName;
            m_nSeasonIndex = nSeasonIndex;
            m_nEpisodeIndex = nEpisodeIndex;
        }
    };

    class ForomEpisodeMatchResult : IComparable<ForomEpisodeMatchResult>
    {
        public String m_sName = String.Empty;
        public String m_sLink = String.Empty;
        public int m_nSeasonIndex = 0;
        public int m_nEpisodeIndex = 0;

        // for sorting
        public int m_nDistance = 0xFFFF;

        public ForomEpisodeMatchResult(String sName, String sLink)
        {
            m_sName = sName;
            m_sLink = sLink;

            String RegExp = @"(?:(?:s|saison )(?<season>[0-1]?[0-9])(?:e| episode )(?<episode>[0-9]{2})|(?<season>(?:[0-1][0-9]|(?<!\d)[0-9]))x?(?<episode>[0-9]{2}))(?!\d)";
            // extract language & name
            Regex subEngine = new Regex(RegExp, RegexOptions.IgnoreCase);
            Match subMatch = subEngine.Match(sName);
            if (subMatch.Success)
            {
                m_nSeasonIndex = Convert.ToInt32(subMatch.Groups[1].Value);
                m_nEpisodeIndex = Convert.ToInt32(subMatch.Groups[2].Value);
            }
        }

        public int CompareTo(ForomEpisodeMatchResult other)
        {
            return m_nDistance.CompareTo(other.m_nDistance);
        }

        public void ComputeDistance(ForomSubtitleEpisode episode)
        {
            m_nDistance = MediaPortal.Util.Levenshtein.Match(m_sName, episode.m_sSeriesName);
        }
    };

    class ForomSeasonMatchResult : IComparable<ForomSeasonMatchResult>
    {
        public String sSubFullName = String.Empty;
        public String sSubName = String.Empty;
        public String sSubLink = String.Empty;
        public String sSubLinkName = String.Empty;
        public String sPHPSessionID = String.Empty;
        public int nSeasonMin = 0xFFFF;
        public int nSeasonMax = 0;

        // for sorting
        public int nDistance = 0xFFFF;

        public int CompareTo(ForomSeasonMatchResult other)
        {
            return nDistance.CompareTo(other.nDistance);
        }

        public ForomSeasonMatchResult(String sName, String sLink)
        {
            sSubFullName = sName.ToLower();
            sSubName = sSubFullName;
            sSubLink = sLink;
            String RegExp = "\\?lg=[^&]*?&type=(.*?)&c=[^&]*(?:&PHPSESSID=(.*)|)";
            // extract language & name
            Regex subEngine = new Regex(RegExp, RegexOptions.IgnoreCase);
            Match subMatch = subEngine.Match(sSubLink);
            if (subMatch.Success)
            {
                sSubLinkName = subMatch.Groups[1].Value.ToLower();
                sPHPSessionID = subMatch.Groups[2].Value;
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

        public void ComputeDistance(ForomSubtitleEpisode episode)
        {
            nDistance = MediaPortal.Util.Levenshtein.Match(sSubName, episode.m_sSeriesName);
        }
    };
}
#endif