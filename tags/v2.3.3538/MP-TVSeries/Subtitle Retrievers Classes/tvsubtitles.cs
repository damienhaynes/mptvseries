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
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.ComponentModel;
using ICSharpCode.SharpZipLib.Zip;

namespace WindowPlugins.GUITVSeries.Subtitles
{
  class TvSubtitles
  {
    public BackgroundWorker worker = null;

    String m_sBaseUrl = String.Empty;
    DBEpisode m_dbEpisode = null;
    bool m_bSubtitleRetrieved = false;
    Feedback.IFeedback m_feedback = null;

    public delegate void SubtitleRetrievalCompletedHandler(bool bFound);
    /// <summary>
    /// This will be triggered once all the SeriesAndEpisodeInfo has been parsed completely.
    /// </summary>
    public event SubtitleRetrievalCompletedHandler SubtitleRetrievalCompleted;

    public TvSubtitles(Feedback.IFeedback feedback)
    {
      m_sBaseUrl = "http://www.tvsubtitles.net";
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
      MPTVSeriesLog.Write("Starting TVSubtitles Subtitles retrieval");
      MPTVSeriesLog.Write("**********************************");

      System.Threading.Thread.CurrentThread.Priority = ThreadPriority.Lowest;
      try
      {
        WebClient client = new WebClient();

        DBOnlineSeries series = new DBOnlineSeries(m_dbEpisode[DBEpisode.cSeriesID]);
        DBSeason season = new DBSeason(m_dbEpisode[DBEpisode.cSeriesID], m_dbEpisode[DBEpisode.cSeasonIndex]);
        TVSubtitlesSubtitleEpisode episode = new TVSubtitlesSubtitleEpisode(series[DBOnlineSeries.cOriginalName], m_dbEpisode[DBEpisode.cFilename], m_dbEpisode[DBEpisode.cSeasonIndex], m_dbEpisode[DBEpisode.cEpisodeIndex]);
        TVSubtitlesSeriesMatchResult finalSeriesResult = null;

        String sLocalSeriesName = episode.m_sSeriesName;
        MatchCollection matches;

        if (sLocalSeriesName.Length > 0 && m_sBaseUrl.Length > 0)
        {
          String s1stLevelURL = m_sBaseUrl + "/tvshows.html";
          MPTVSeriesLog.Write("Step 1: looking into list of shows in: " + s1stLevelURL);
          Stream data = client.OpenRead(s1stLevelURL);
          StreamReader reader = new StreamReader(data);
          String sPage = reader.ReadToEnd().Replace('\0', ' ');
          data.Close();
          reader.Close();

          String RegExp = @"<td align=left[^>]*>\s*<a href=""([^""]*)""><b>([^<]*)";
          Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
          matches = Engine.Matches(sPage);
          List<TVSubtitlesSeriesMatchResult> sortedMatchList = new List<TVSubtitlesSeriesMatchResult>();
          foreach (Match match in matches)
          {
            TVSubtitlesSeriesMatchResult result = new TVSubtitlesSeriesMatchResult(match.Groups[2].Value, match.Groups[1].Value);
            result.ComputeDistance(episode);
            sortedMatchList.Add(result);
          }

          sortedMatchList.Sort();

          if (sortedMatchList[0].nDistance == 0)
            finalSeriesResult = sortedMatchList[0];
          else
          {
            MPTVSeriesLog.Write("Choosing the series from a list");
            // show the user the list and ask for the right one
            List<Feedback.CItem> Choices = new List<Feedback.CItem>();
            foreach (TVSubtitlesSeriesMatchResult match in sortedMatchList)
            {
              Choices.Add(new Feedback.CItem(match.sName, String.Empty, match));
            }
            Feedback.ChooseFromSelectionDescriptor descriptor = new Feedback.ChooseFromSelectionDescriptor();
            descriptor.m_sTitle = "Choose correct series item";
            descriptor.m_sItemToMatchLabel = "Local series:";
            descriptor.m_sItemToMatch = episode.m_sSeriesName;
            descriptor.m_sListLabel = "Available series list:";
            descriptor.m_List = Choices;
            descriptor.m_sbtnIgnoreLabel = String.Empty;

            Feedback.CItem Selected = null;
            if (m_feedback.ChooseFromSelection(descriptor, out Selected) == Feedback.ReturnCode.OK)
            {
              finalSeriesResult = Selected.m_Tag as TVSubtitlesSeriesMatchResult;
            }
          }
        }
        else
        {
          String error = String.Format("Error, bad base URL ({0})", m_sBaseUrl);
          MPTVSeriesLog.Write(error);
          throw new Exception(error);
        }

        // now, look for the season
        String sSeasonUrl = String.Empty;
        if (finalSeriesResult != null)
        {
          String s2ndLevelURL = finalSeriesResult.sSubLink;
          String RegExp = @"(.*tvshow-\d*-)(\d*).html";
          Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
          Match match = Engine.Match(s2ndLevelURL);
          if (match.Success)
          {
            sSeasonUrl = m_sBaseUrl + "/" + match.Groups[1].Value + episode.m_nSeasonIndex + ".html";
          }
        }

        String sEpisodeUrl = String.Empty;
        if (sSeasonUrl != String.Empty)
        {
          String s3rdLevelURL = sSeasonUrl;

          MPTVSeriesLog.Write("Step 2: looking into list of episodes in: " + sSeasonUrl);
          Stream data = client.OpenRead(s3rdLevelURL);
          StreamReader reader = new StreamReader(data);
          String sPage = reader.ReadToEnd().Replace('\0', ' ');
          data.Close();
          reader.Close();

          String RegExp = @"<tr[^>]*>\s*<td>(\d*)x(\d*)</td>\s*<td[^>]*>\s*<a href=""([^""]*)""><b>([^<]*)";
          Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
          matches = Engine.Matches(sPage);
          if (matches.Count == 0)
          {
            MPTVSeriesLog.Write("Error: no episodes found in the series/season page");
          }
          foreach (Match match in matches)
          {
            int nEpisodeIndex = Convert.ToInt32(match.Groups[2].Value);
            if (nEpisodeIndex == episode.m_nEpisodeIndex)
            {
              sEpisodeUrl = m_sBaseUrl + "/" + match.Groups[3].Value;
              break;
            }
          }
        }

        if (sEpisodeUrl == String.Empty)
        {
          MPTVSeriesLog.Write("Choosing the episode from a list");
          // show the user the list and ask for the right one
          List<Feedback.CItem> Choices = new List<Feedback.CItem>();
          foreach (Match match in matches)
          {
            String sName = match.Groups[1].Value + "x" + match.Groups[2].Value + " - " + match.Groups[4].Value;
            Choices.Add(new Feedback.CItem(sName, String.Empty, match));
          }
          Feedback.ChooseFromSelectionDescriptor descriptor = new Feedback.ChooseFromSelectionDescriptor();
          descriptor.m_sTitle = "Choose correct episode item";
          descriptor.m_sItemToMatchLabel = "Local Episode:";
          descriptor.m_sItemToMatch = episode.m_nSeasonIndex + "x" + episode.m_nEpisodeIndex + " - " + episode.m_sFileName;
          descriptor.m_sListLabel = "Available series list:";
          descriptor.m_List = Choices;
          descriptor.m_sbtnIgnoreLabel = String.Empty;

          Feedback.CItem Selected = null;
          if (m_feedback.ChooseFromSelection(descriptor, out Selected) == Feedback.ReturnCode.OK)
          {
            Match selected = Selected.m_Tag as Match;
            sEpisodeUrl = selected.Groups[3].Value;
          }
        }

        if (sEpisodeUrl != String.Empty)
        {
          String s3rdLevelURL = sEpisodeUrl;

          MPTVSeriesLog.Write("Step 3: looking into the list of subtitles in: " + s3rdLevelURL);
          Stream data = client.OpenRead(s3rdLevelURL);
          StreamReader reader = new StreamReader(data);
          String sPage = reader.ReadToEnd().Replace('\0', ' ');
          data.Close();
          reader.Close();

          String sFilters = DBOption.GetOptions(DBOption.cSubs_TVSubtitles_LanguageFilterList);
          String RegExp = @"<a href=""([^""]*)""[^>]*>(?:\s*?<(?!/a)[^>]*>[^<]*)*?<img src="".*?/(" + sFilters + @")[^>/]*>([^<]*)";
          Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
          matches = Engine.Matches(sPage);
          if (matches.Count == 0)
          {
            MPTVSeriesLog.Write("Error: no subtitles found in the episode page");
          }

          if (matches.Count != 0)
          {
            // now, sort & take the first one as our best result
            List<Feedback.CItem> Choices = new List<Feedback.CItem>();
            foreach (Match match in matches)
            {
              String sName = match.Groups[3].Value.Replace("subtitles", "");
              Choices.Add(new Feedback.CItem(sName, String.Empty, match));
            }

            // ask the user
            Feedback.ChooseFromSelectionDescriptor descriptor = new Feedback.ChooseFromSelectionDescriptor();
            descriptor.m_sTitle = "Select matching subtitle";
            descriptor.m_sItemToMatchLabel = "filename:";
            descriptor.m_sItemToMatch = Path.GetFileNameWithoutExtension(episode.m_sFileName);
            descriptor.m_sListLabel = "Matching subtitles:";
            descriptor.m_List = Choices;
            descriptor.m_sbtnIgnoreLabel = String.Empty;

            Feedback.CItem Selected = null;
            if (m_feedback.ChooseFromSelection(descriptor, out Selected) == Feedback.ReturnCode.OK)
            {
              Match selected = Selected.m_Tag as Match;
              String sDownloadUrl = m_sBaseUrl + selected.Groups[1].Value;			  
              sDownloadUrl = sDownloadUrl.Replace("/subtitle", "/download");

			  client.Headers.Add("referer", m_sBaseUrl);
              // we need to download those somewhere, unpack, remove the entry from the list & replace it with the files in the archive
              client.DownloadFile(sDownloadUrl, System.IO.Path.GetTempPath() + "temp_subs.zip");

              using (ZipInputStream s = new ZipInputStream(File.OpenRead(System.IO.Path.GetTempPath() + "temp_subs.zip")))
              {
                ZipEntry theEntry;
                if ((theEntry = s.GetNextEntry()) != null)
                {
                  string fileName = Path.GetFileName(theEntry.Name);

                  if (fileName.Length > 0 && fileName[0] != '.')
                  {
                    // get the path of our video
                    // we have it!!! download, store in the right place & rename accordingly
                    RegExp = @"(.*)\..*";
                    Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
                    Match matchNameNoExt = Engine.Match(episode.m_sFileName);

                    RegExp = @".*\.(.*)";
                    Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
                    Match matchExt = Engine.Match(fileName);
                    if (matchExt.Success && matchNameNoExt.Success)
                    {
                      using (FileStream streamWriter = File.Create(matchNameNoExt.Groups[1].Value + "." + matchExt.Groups[1].Value))
                      {

                        int size = 2048;
                        byte[] bdata = new byte[2048];
                        while (true)
                        {
                          size = s.Read(bdata, 0, bdata.Length);
                          if (size > 0)
                          {
                            streamWriter.Write(bdata, 0, size);
                          }
                          else
                          {
                            break;
                          }
                        }
                      }
                      m_bSubtitleRetrieved = true;
                      m_dbEpisode[DBEpisode.cAvailableSubtitles] = 1;
                      m_dbEpisode.Commit();
                    }
                  }
                }
              }

              if (!m_bSubtitleRetrieved)
              {
                Unrar unrar = new Unrar();
                unrar.ArchiveName = System.IO.Path.GetTempPath() + "temp_subs.zip";
                List<String> fileList = unrar.FileNameList;
                MPTVSeriesLog.Write(String.Format("Decompressing archive {0} : {1} files", "temp_subs.zip", fileList.Count));

                if (fileList.Count == 0)
                {
                  // weird
                  MPTVSeriesLog.Write("Error: no files in sub archive - must be corrupted");
                }
                else if (fileList.Count == 1)
                {
                  // normal case
                  String file = fileList[0];
                  // get the path of our video
                  // we have it!!! download, store in the right place & rename accordingly
                  RegExp = @"(.*)\..*";
                  Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
                  Match matchNameNoExt = Engine.Match(episode.m_sFileName);

                  RegExp = @".*\.(.*)";
                  Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
                  Match matchExt = Engine.Match(file);
                  if (matchExt.Success && matchNameNoExt.Success)
                  {
                    if (unrar.Extract(file, matchNameNoExt.Groups[1].Value + "." + matchExt.Groups[1].Value))
                    {
                      m_bSubtitleRetrieved = true;
                      m_dbEpisode[DBEpisode.cAvailableSubtitles] = 1;
                      m_dbEpisode.Commit();
                    }
                    else
                      MPTVSeriesLog.Write("Error: Decompression of archive failed");
                  }
                  else
                    MPTVSeriesLog.Write("Error: no extension for subtitle");
                }
                else
                {
                  // more than one sub in the rar? can't be as far as I know
                  MPTVSeriesLog.Write("Error: More than one file in sub archive!");
                }
              }
            }
          }

          MPTVSeriesLog.Write("*******************************");
          MPTVSeriesLog.Write("TVSubtitles Subtitles retrieval ended");
          MPTVSeriesLog.Write("*******************************");
        }
      }

      catch (Exception ex)
      {
        MPTVSeriesLog.Write("Could not do TVSubtitles Subtitle retrieval: " + ex.Message, MPTVSeriesLog.LogLevel.Normal);
      }
    }
  }

  public class TVSubtitlesSubtitleEpisode
  {
    public String m_sSeriesName = String.Empty;
    public String m_sFileName = String.Empty;
    public int m_nSeasonIndex = 0;
    public int m_nEpisodeIndex = 0;

    public TVSubtitlesSubtitleEpisode(String sSeriesName, String sFileName, int nSeasonIndex, int nEpisodeIndex)
    {
      m_sSeriesName = sSeriesName;
      m_sFileName = sFileName;
      m_nSeasonIndex = nSeasonIndex;
      m_nEpisodeIndex = nEpisodeIndex;
    }
  };


  class TVSubtitlesSeriesMatchResult : IComparable<TVSubtitlesSeriesMatchResult>
  {
    public String sName = String.Empty;
    public String sSubLink = String.Empty;

    // for sorting
    public int nDistance = 0xFFFF;

    public int CompareTo(TVSubtitlesSeriesMatchResult other)
    {
      return nDistance.CompareTo(other.nDistance);
    }

    public TVSubtitlesSeriesMatchResult(String sName, String sLink)
    {
      this.sName = sName;
      this.sSubLink = sLink;
    }

    public void ComputeDistance(TVSubtitlesSubtitleEpisode episode)
    {
      nDistance = MediaPortal.Util.Levenshtein.Match(sName.ToLower(), episode.m_sSeriesName.ToLower());
    }
  };
}

