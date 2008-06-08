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
#if inclDownloaders
namespace WindowPlugins.GUITVSeries.Subtitles
{
  class Remository
  {
    public BackgroundWorker worker = null;

    String m_sBaseUrl = String.Empty;
    int m_iMainIdx = 0;
    String m_sUserName = String.Empty;
    String m_sPassword = String.Empty;
    DBEpisode m_dbEpisode = null;
    bool m_bSubtitleRetrieved = false;
    Feedback.IFeedback m_feedback = null;

    private String RegExp = String.Empty;
    private String RegExpEpisode = String.Empty;
    private String RegExpDownload = String.Empty;

    private Regex Engine = null;
    private Regex EpisodeEngine = null;
    private Regex DownloadEngine = null;

    private String loginUri = String.Empty;
    private String reqString = String.Empty;

    public delegate void SubtitleRetrievalCompletedHandler(bool bFound);
    /// <summary>
    /// This will be triggered once all the SeriesAndEpisodeInfo has been parsed completely.
    /// </summary>
    public event SubtitleRetrievalCompletedHandler SubtitleRetrievalCompleted;


    #region Constructors

    public Remository(Feedback.IFeedback feedback)
    {
      init(feedback);
    }
    private void init(Feedback.IFeedback feedback)
    {
      m_sBaseUrl = DBOption.GetOptions(DBOption.cSubs_Remository_BaseURL);
      m_iMainIdx = DBOption.GetOptions(DBOption.cSubs_Remository_MainIdx);
      m_sUserName = DBOption.GetOptions(DBOption.cSubs_Remository_UserName);
      m_sPassword = DBOption.GetOptions(DBOption.cSubs_Remository_Password);
      m_feedback = feedback;

      //series-season regexp
      RegExp = "<td><h3>.*?href=\"([^\"]*?id=(\\d*)\">([^<]*))</a>";
      //episode regexp
      RegExpEpisode = "<dd><img.*?href=\"([^\\\"]*?id=(\\d*)\">([^<]*))</a>";
      //download regexp
      RegExpDownload = "<h2><a href=\"([^\\\"]*fname=([^\\\"]*))";      

      Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
      EpisodeEngine = new Regex(RegExpEpisode, RegexOptions.IgnoreCase);
      DownloadEngine = new Regex(RegExpDownload, RegexOptions.IgnoreCase);

      loginUri = m_sBaseUrl + "index.php";
      reqString = "option=com_smf&action=login2&user=" + m_sUserName + "&passwrd=" + m_sPassword;

      worker = new BackgroundWorker();
      worker.WorkerReportsProgress = true;
      worker.WorkerSupportsCancellation = true;
      worker.DoWork += new DoWorkEventHandler(worker_DoWork);
      worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
    }

  # endregion

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
      System.Threading.Thread.CurrentThread.Priority = ThreadPriority.Lowest;
      WebClient client = new WebClient();
      
      try
      {
        MPTVSeriesLog.Write("**********************************");
        MPTVSeriesLog.Write("Starting REMOSITORY Subtitles retrieval");
        MPTVSeriesLog.Write("**********************************");

        DBOnlineSeries series = new DBOnlineSeries(m_dbEpisode[DBEpisode.cSeriesID]);
        DBSeason season = new DBSeason(m_dbEpisode[DBEpisode.cSeriesID], m_dbEpisode[DBEpisode.cSeasonIndex]);
        RemositorySubtitleEpisode episode = new RemositorySubtitleEpisode(series[DBOnlineSeries.cOriginalName], m_dbEpisode[DBEpisode.cFilename], m_dbEpisode[DBEpisode.cSeasonIndex], m_dbEpisode[DBEpisode.cEpisodeIndex]);
       
        //login handling: get session cookie and add to WebClient headers in order to user autenticated session
        String sLocalSeriesName = episode.m_sSeriesName;

        CookieContainer cc = new CookieContainer();
        HttpWebRequest loginRequest = (HttpWebRequest)WebRequest.Create(loginUri + "?" + reqString);

        loginRequest.Proxy = null;
        loginRequest.CookieContainer = cc;
        loginRequest.Method = "GET";

        HttpWebResponse loginResponse = (HttpWebResponse)loginRequest.GetResponse();
        loginResponse.Close();

        WebHeaderCollection headerCookies = new WebHeaderCollection();
        String cookieHeader = "";
        foreach (Cookie c in loginResponse.Cookies)
        {
          cookieHeader += c.Name + "=" + c.Value + "; ";
        }
        MPTVSeriesLog.Write("Adding Cookie " + cookieHeader);
        headerCookies.Add("Cookie",cookieHeader);
        //add login cookie to webClient
        client.Headers.Add(headerCookies);


        //STEP 1: check series name
        Stream data = client.OpenRead(loginUri + "?option=com_remository&itemid=" + m_iMainIdx);
        StreamReader reader = new StreamReader(data);
        String sPage = reader.ReadToEnd().Replace('\0', ' ');
  
        int iSeriesIdx = 0;
        int iSeasonIdx = 0;
        int iEpisodeIdx = 0;

        iSeriesIdx = matchSeries(sPage,client,episode);

        if (iSeriesIdx == 0)
        {
          MPTVSeriesLog.Write("NO Series avalilable: (Name = " + episode.m_sSeriesName + ")");
        }
        else
        {
          MPTVSeriesLog.Write("Series found: (idx = " + iSeriesIdx + ")");

          //step 2: find season if available
          data = client.OpenRead(loginUri + "?option=com_remository&itemid=" + m_iMainIdx + "&func=select&id=" + iSeriesIdx);
          reader = new StreamReader(data);
          sPage = reader.ReadToEnd().Replace('\0', ' ');
         
          iSeasonIdx = matchSeason(sPage,client,episode);

          if (iSeasonIdx == 0)
          {
            iEpisodeIdx = matchEpisode(sPage,client,episode);
          }

          if (iSeasonIdx == 0 && iEpisodeIdx == 0 ) {
            MPTVSeriesLog.Write("NO Season avalilable: (Season = " + episode.m_nSeasonIndex + ")");
          }
          else
          {
            if (iEpisodeIdx == 0)
            {
              MPTVSeriesLog.Write("Season found: (idx = " + iSeasonIdx + ")");
              data = client.OpenRead(loginUri + "?option=com_remository&itemid=" + m_iMainIdx + "&func=select&id=" + iSeasonIdx);
              reader = new StreamReader(data);
              sPage = reader.ReadToEnd().Replace('\0', ' ');

              iEpisodeIdx = matchEpisode(sPage,client,episode);
            }
            
            if (iEpisodeIdx == 0)
            {
              MPTVSeriesLog.Write("NO Episode avalilable: (Episode = " + episode.m_nEpisodeIndex + ")");
            }
            else
            {
              MPTVSeriesLog.Write("Episode found: (idx = " + iEpisodeIdx + ")");
              data = client.OpenRead(loginUri + "?option=com_remository&itemid=" + m_iMainIdx + "&func=fileinfo&id=" + iEpisodeIdx);
              reader = new StreamReader(data);
              sPage = reader.ReadToEnd().Replace('\0', ' ');

              MatchCollection matches = DownloadEngine.Matches(sPage);

              foreach (Match match in matches)
              {
              String dir = Path.GetDirectoryName(episode.m_sFileName);
                String movieFileName = Path.GetFileName(episode.m_sFileName);
                String archiveFile = dir + Path.DirectorySeparatorChar + match.Groups[2].Value;

                String url = match.Groups[1].Value.Replace("&amp;", "&");
                MPTVSeriesLog.Write("Download file : " + match.Groups[2]);
                if (System.IO.File.Exists(archiveFile))
                {
                  MPTVSeriesLog.Write("File " + archiveFile + " found: deleting");
                  System.IO.File.Delete(archiveFile);
                }
                client.DownloadFile(url, archiveFile);

                List<Feedback.CItem> Choices = episode.extract(match.Groups[2].Value);
                String selectedFile = String.Empty;
                if (Choices.Count == 1)
                {
                  selectedFile = Choices[0].m_Tag as String;
                }
                else
                {
                  if (Choices.Count > 0)
                  {
                    Feedback.ChooseFromSelectionDescriptor descriptor = new Feedback.ChooseFromSelectionDescriptor();
                    descriptor.m_sTitle = Translation.CFS_Select_Matching_Subitle_File;
                    descriptor.m_sItemToMatchLabel = Translation.CFS_Subtitle_Episode;
                    descriptor.m_sItemToMatch = episode.m_sSeriesName + " " + episode.m_nSeasonIndex + "x" + episode.m_nEpisodeIndex;
                    descriptor.m_sListLabel = Translation.CFS_Matching_Subtitles;
                    descriptor.m_List = Choices;
                    descriptor.m_sbtnIgnoreLabel = String.Empty;

                    Feedback.CItem Selected = null;
                    if (m_feedback.ChooseFromSelection(descriptor, out Selected) == Feedback.ReturnCode.OK)
                    {
                      selectedFile = Selected.m_Tag as String;
                    }
                  }
                  else
                  {
                    MPTVSeriesLog.Write("No files found!");
                  }
                }

                foreach (Feedback.CItem choice in Choices)
                {
                  if (choice.m_Tag as String == selectedFile)
                  {
                    String targetFile = dir + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(movieFileName) + Path.GetExtension(selectedFile);
                    if (System.IO.File.Exists(targetFile))
                    {
                      Feedback.ChooseFromYesNoDescriptor descriptor = new Feedback.ChooseFromYesNoDescriptor();
                      descriptor.m_sTitle = Translation.CYN_Subtitle_File_Replace;
                      descriptor.m_sLabel = Translation.CYN_Old_Subtitle_Replace;
                      descriptor.m_dialogButtons = Feedback.DialogButtons.YesNo;
                      descriptor.m_dialogDefaultButton = Feedback.ReturnCode.Yes;
                      if (m_feedback.YesNoOkDialog(descriptor) == Feedback.ReturnCode.Yes)
                      {
                        MPTVSeriesLog.Write("File " + targetFile + " found: deleted and replaced ");
                        System.IO.File.Delete(targetFile);
                        System.IO.File.Move(selectedFile, targetFile);
                      }
                      else
                      {
                        MPTVSeriesLog.Write("File " + targetFile + " found: NOT deleted");
                        System.IO.File.Delete(selectedFile);
                      }
                    }
                    else
                    {
                      System.IO.File.Move(selectedFile, targetFile);
                    }
                   
                    MPTVSeriesLog.Write("Selected : " + Path.GetFileName(choice.m_Tag as String));
                    m_bSubtitleRetrieved = true;
                  }
                  else
                  {
                    System.IO.File.Delete(choice.m_Tag as String);                    
                  }
                  //check if dir is empty
                  if (isDirectoryEmpty(Path.GetDirectoryName(choice.m_Tag as String)))
                  {
                    System.IO.Directory.Delete(Path.GetDirectoryName(choice.m_Tag as String));
                  }
                }
                System.IO.File.Delete(archiveFile);


              }
            }
          }
        }

        MPTVSeriesLog.Write("*******************************");
        MPTVSeriesLog.Write("REMOSITORY Subtitles retrieval ended");
        MPTVSeriesLog.Write("*******************************");
      }
      catch (Exception ex)
      {
        MPTVSeriesLog.Write("Could not do Remository Subtitle retrival: " + ex.Message, MPTVSeriesLog.LogLevel.Normal);
        MPTVSeriesLog.Write("Could not do Remository Subtitle retrival: " + ex.StackTrace, MPTVSeriesLog.LogLevel.Normal);
        if (ex.InnerException != null)
        {
          MPTVSeriesLog.Write("Inner Exception : " + ex.InnerException.Message, MPTVSeriesLog.LogLevel.Normal);
          MPTVSeriesLog.Write("Inner Exception : " + ex.InnerException.StackTrace, MPTVSeriesLog.LogLevel.Normal);

        }
      }
      client.Dispose();
    }


    private int matchSeries(String sPage,WebClient client, RemositorySubtitleEpisode episode)
    {
      MatchCollection matches = Engine.Matches(sPage);
      matches = Engine.Matches(sPage);
      int retValue = 0;

      List<RemositorySeriesMatchResult> sortedMatchList = new List<RemositorySeriesMatchResult>();
      List<RemositorySeriesMatchResult> exactMatches = new List<RemositorySeriesMatchResult>();
      Feedback.CItem selectedSeries = null;

      //load matches in sortedMatchList
      foreach (Match match in matches)
      {
        RemositorySeriesMatchResult result = new RemositorySeriesMatchResult(match.Groups[3].Value.ToLower(), match.Groups[2].Value);
        result.ComputeDistance(episode);
        sortedMatchList.Add(result);
      }

      sortedMatchList.Sort();


      if (sortedMatchList.Count > 0)
      {
        MPTVSeriesLog.Write(String.Format("Found {0} series/season entries in the page", sortedMatchList.Count));
        //check if there are exact matches
        foreach (RemositorySeriesMatchResult result in sortedMatchList)
        {
          if (result.nDistance == 1)
            exactMatches.Add(result);
        }
      }

      if (exactMatches.Count > 0)
      {
        MPTVSeriesLog.Write(String.Format("Found {0} exact matches in the page", exactMatches.Count));
        if (exactMatches.Count == 1)
        {
          retValue = Convert.ToInt32(exactMatches[0].sIdx);
        }
        else
        {
          List<Feedback.CItem> Choices = new List<Feedback.CItem>();

          foreach (RemositorySeriesMatchResult match in exactMatches)
          {
            Choices.Add(new Feedback.CItem(match.sSubFullName, String.Empty, match.sIdx));
          }

          Feedback.ChooseFromSelectionDescriptor seriesSelector = new Feedback.ChooseFromSelectionDescriptor();
          seriesSelector.m_sTitle = Translation.CFS_Choose_Correct_Series;
          seriesSelector.m_sItemToMatchLabel = Translation.CFS_Local_Series;
          seriesSelector.m_sItemToMatch = episode.m_sSeriesName;
          seriesSelector.m_sListLabel = Translation.CFS_Available_Series;
          seriesSelector.m_List = Choices;
          seriesSelector.m_sbtnIgnoreLabel = String.Empty;

          if (m_feedback.ChooseFromSelection(seriesSelector, out selectedSeries) == Feedback.ReturnCode.OK)
          {
            retValue = Convert.ToInt32(selectedSeries.m_Tag as String);
          }
        }
      }
      else
      {
        if (sortedMatchList.Count > 0)
        {
          MPTVSeriesLog.Write("Choosing the series/season from a list");
          // show the user the list and ask for the right one
          List<Feedback.CItem> Choices = new List<Feedback.CItem>();
          foreach (RemositorySeriesMatchResult match in sortedMatchList)
          {
            Choices.Add(new Feedback.CItem(match.sSubFullName.Trim(), String.Empty, match.sIdx));
          }

          Feedback.ChooseFromSelectionDescriptor descriptor = new Feedback.ChooseFromSelectionDescriptor();
          descriptor.m_sTitle = Translation.CFS_Choose_Correct_Series;
          descriptor.m_sItemToMatchLabel = Translation.CFS_Local_Series;
          descriptor.m_sItemToMatch = episode.m_sSeriesName;
          descriptor.m_sListLabel = Translation.CFS_Available_Series;
          descriptor.m_List = Choices;
          descriptor.m_sbtnIgnoreLabel = String.Empty;

          Feedback.CItem Selected = null;
          if (m_feedback.ChooseFromSelection(descriptor, out Selected) == Feedback.ReturnCode.OK)
          {
            retValue = Convert.ToInt32(Selected.m_Tag as String);
          }
        }
      }
      return retValue;
    }

    private int matchSeason(String sPage, WebClient client, RemositorySubtitleEpisode episode)
    {
      int retValue = 0;
      MatchCollection matches = Engine.Matches(sPage);

      foreach (Match match in matches)
      {
        if (match.Groups[3].Value.Trim() == "Stagione " + episode.m_nSeasonIndex)
        {
          retValue = Convert.ToInt32(match.Groups[2].Value);
          break;
        }
      }

      if (retValue == 0 && matches.Count > 0)
      {
        List<Feedback.CItem> Choices = new List<Feedback.CItem>();
        foreach (Match match in matches)
        {
          Choices.Add(new Feedback.CItem(match.Groups[3].Value.Trim(), String.Empty, match.Groups[2].Value));
        }

        Feedback.ChooseFromSelectionDescriptor descriptor = new Feedback.ChooseFromSelectionDescriptor();
        descriptor.m_sTitle = Translation.CFS_Choose_Correct_Season;
        descriptor.m_sItemToMatchLabel = Translation.CFS_Local_Season_Index;
        descriptor.m_sItemToMatch = episode.m_nSeasonIndex + "";
        descriptor.m_sListLabel = Translation.CFS_Available_Seasons;
        descriptor.m_List = Choices;
        descriptor.m_sbtnIgnoreLabel = String.Empty;

        Feedback.CItem Selected = null;
        if (m_feedback.ChooseFromSelection(descriptor, out Selected) == Feedback.ReturnCode.OK)
        {
          retValue = Convert.ToInt32(Selected.m_Tag as String);
        }
      }
      return retValue;
    }

    private int matchEpisode(String sPage,WebClient client, RemositorySubtitleEpisode episode)
    {
      int retValue = 0;
      //Find other file version
      MatchCollection matches = Engine.Matches(sPage);

      //Find default file version
      MatchCollection fileMatches = EpisodeEngine.Matches(sPage);
      Feedback.CItem selectedVersion = null;
      String sSelectedVersionTag = null;

      if (matches.Count > 0)
      {
        List<Feedback.CItem> Choices = new List<Feedback.CItem>();

        Feedback.ChooseFromSelectionDescriptor versionSelector = new Feedback.ChooseFromSelectionDescriptor();
        versionSelector.m_sTitle = Translation.CFS_Select_Correct_Subtitle_Version;
        versionSelector.m_sItemToMatchLabel = Translation.CFS_Subtitle_Episode;
        versionSelector.m_sItemToMatch = episode.m_sSeriesName + " " + episode.m_nSeasonIndex + "x" + episode.m_nEpisodeIndex;
        versionSelector.m_sListLabel = Translation.CFS_Select_Version;
        versionSelector.m_List = Choices;
        versionSelector.m_sbtnIgnoreLabel = String.Empty;

        //add default version if available
        if (fileMatches.Count > 0)
        {
          Choices.Add(new Feedback.CItem("Default Version", String.Empty, "default_version"));
        }

        //add other versions
        foreach (Match match in matches)
        {
          Choices.Add(new Feedback.CItem(match.Groups[3].Value.Trim(), String.Empty, match.Groups[2].Value));
        }


        if (m_feedback.ChooseFromSelection(versionSelector, out selectedVersion) == Feedback.ReturnCode.OK)
        {
          sSelectedVersionTag = selectedVersion.m_Tag as String;
        }
      }
      else
      {
        sSelectedVersionTag = "default_version";
      }

      MPTVSeriesLog.Write("Episode Version selected: (Episode = " + sSelectedVersionTag + ")");
      if (sSelectedVersionTag != "default_version")
      {
        //load custom file version page
        Stream data = client.OpenRead(loginUri + "?option=com_remository&itemid=" + m_iMainIdx + "&func=select&id=" + selectedVersion.m_Tag as String);
        StreamReader reader = new StreamReader(data);
        sPage = reader.ReadToEnd().Replace('\0', ' ');
        fileMatches = EpisodeEngine.Matches(sPage);
      }

      foreach (Match match in fileMatches)
      {
        String ep = episode.m_sSeriesName + " " + episode.m_nSeasonIndex + "x" + String.Format("{0:00}", episode.m_nEpisodeIndex);
        if (match.Groups[3].Value.Trim().ToLower() == ep.ToLower())
        {
          retValue = Convert.ToInt32(match.Groups[2].Value);
          break;
        }
      }

      if (retValue == 0 && fileMatches.Count > 0)
      {
        List<Feedback.CItem> Choices = new List<Feedback.CItem>();
        foreach (Match match in fileMatches)
        {
          Choices.Add(new Feedback.CItem(match.Groups[3].Value.Trim(), String.Empty, match.Groups[2].Value));
        }

        Feedback.ChooseFromSelectionDescriptor descriptor = new Feedback.ChooseFromSelectionDescriptor();
        descriptor.m_sTitle = Translation.CFS_Choose_Correct_Episode;
        descriptor.m_sItemToMatchLabel = Translation.CFS_Local_Episode_Index;
        descriptor.m_sItemToMatch = episode.m_sSeriesName + " " + episode.m_nSeasonIndex + "x" + episode.m_nEpisodeIndex;
        descriptor.m_sListLabel = Translation.CFS_Available_Episode_List;
        descriptor.m_List = Choices;
        descriptor.m_sbtnIgnoreLabel = String.Empty;

        Feedback.CItem Selected = null;
        if (m_feedback.ChooseFromSelection(descriptor, out Selected) == Feedback.ReturnCode.OK)
        {
          retValue = Convert.ToInt32(Selected.m_Tag as String);
        }
      }
      return retValue;
    }

    private bool isDirectoryEmpty(string path)
    {
      string[] subDirs = Directory.GetDirectories(path);
      if (0 == subDirs.Length)
      {
        string[] files = Directory.GetFiles(path);
        return (0 == files.Length);
      }
      return false;
    }

  }

  public class RemositorySubtitleEpisode
  {
      public String m_sSeriesName = String.Empty;
      public String m_sFileName = String.Empty;
      public int m_nSeasonIndex = 0;
      public int m_nEpisodeIndex = 0;
      
      public RemositorySubtitleEpisode(String sSeriesName, String sFileName, int nSeasonIndex, int nEpisodeIndex)
    {
      m_sSeriesName = sSeriesName.ToLower();
      m_sFileName = sFileName;
      m_nSeasonIndex = nSeasonIndex;
      m_nEpisodeIndex = nEpisodeIndex;
    }
    public List<Feedback.CItem> extract(String subtitleFile)
    {
      String dir = Path.GetDirectoryName(m_sFileName);
      String subtitleFileName = Path.GetFileName(subtitleFile);
      String subtitleFileExtension = Path.GetExtension(subtitleFile);
      String archiveFile = dir + Path.DirectorySeparatorChar + subtitleFile;

      //load files in archive
      List<Feedback.CItem> Choices = new List<Feedback.CItem>();
      
      // RAR HAndling
      if (subtitleFileExtension == ".rar")
      {
        Unrar unrar = new Unrar();
        unrar.ArchiveName = archiveFile;
        unrar.ExtractAll(dir);
        List<String> fileList = unrar.FileNameList;
        foreach (String file in fileList)
        {
          Choices.Add(new Feedback.CItem(file, String.Empty, dir + Path.DirectorySeparatorChar + file));
        }
        unrar = null;
      }
      // ZIP HAndling - Start
      else if (subtitleFileExtension == ".zip")
      {

        using (ZipInputStream s = new ZipInputStream(File.OpenRead(archiveFile)))
        {
          ZipEntry theEntry;
          while ((theEntry = s.GetNextEntry()) != null)
          {
            if (theEntry.IsFile)
            {
              using (FileStream streamWriter = File.Create(dir + Path.DirectorySeparatorChar + theEntry.Name))
              {
                String filename = Path.GetFileName(theEntry.Name);
                if (filename.Length > 0)
                {
                  int size = 2048;
                  byte[] fileData = new byte[2048];
                  while (true)
                  {
                    size = s.Read(fileData, 0, fileData.Length);
                    if (size > 0)
                    {
                      streamWriter.Write(fileData, 0, size);
                    }
                    else
                    {
                      break;
                    }
                  }
                  Choices.Add(new Feedback.CItem(filename, String.Empty, dir + Path.DirectorySeparatorChar + theEntry.Name));
                }
              }
            }
            else
            {
              System.IO.Directory.CreateDirectory(dir + Path.DirectorySeparatorChar + Path.GetDirectoryName(theEntry.Name));
            }
          }
        }
      } // ZIP HAndling   - END
      else
        throw new Exception("Extension not supported " + subtitleFile);
      return Choices;
    }
  }

  public class RemositorySeriesMatchResult : IComparable<RemositorySeriesMatchResult>
  {
    public String sSubFullName = String.Empty;
    public String sIdx = String.Empty;

    // for sorting
    public int nDistance = 0xFFFF;

    public int CompareTo(RemositorySeriesMatchResult other)
    {
      return nDistance.CompareTo(other.nDistance);
    }

    public RemositorySeriesMatchResult(String sName, String sIndex)
    {
      sSubFullName = sName.ToLower();
      sIdx = sIndex;
    }

    public void ComputeDistance(RemositorySubtitleEpisode episode)
    {
      nDistance = MediaPortal.Util.Levenshtein.Match(sSubFullName, episode.m_sSeriesName.ToLower());
    }
  };
}
#endif

