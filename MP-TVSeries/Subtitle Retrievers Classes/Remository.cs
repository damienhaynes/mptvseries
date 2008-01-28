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
  class Remository
  {
    public BackgroundWorker worker = null;

    String m_sBaseUrl = String.Empty;
    int m_iMainIdx = 0;
    String m_sUserName = String.Empty;
    String m_sPassword = String.Empty;
    DBEpisode m_dbEpisode = null;
    bool m_bSubtitleRetrieved = false;
    Feedback.Interface m_feedback = null;

    public delegate void SubtitleRetrievalCompletedHandler(bool bFound);
    /// <summary>
    /// This will be triggered once all the SeriesAndEpisodeInfo has been parsed completely.
    /// </summary>
    public event SubtitleRetrievalCompletedHandler SubtitleRetrievalCompleted;


    #region Constructors

    public Remository(Feedback.Interface feedback)
    {
      init(feedback);
    }
    private void init(Feedback.Interface feedback)
    {
      m_sBaseUrl = DBOption.GetOptions(DBOption.cSubs_Remository_BaseURL);
      m_iMainIdx = DBOption.GetOptions(DBOption.cSubs_Remository_MainIdx);
      m_sUserName = DBOption.GetOptions(DBOption.cSubs_Remository_UserName);
      m_sPassword = DBOption.GetOptions(DBOption.cSubs_Remository_Password);
      m_feedback = feedback;

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
        RemositoryEpisode episode = new RemositoryEpisode(series[DBOnlineSeries.cOriginalName], m_dbEpisode[DBEpisode.cFilename], m_dbEpisode[DBEpisode.cSeasonIndex], m_dbEpisode[DBEpisode.cEpisodeIndex]);
       
        String sLocalSeriesName = episode.m_sSeriesName;

        String loginUri = m_sBaseUrl + "index.php";

        String reqString = "option=com_smf&action=login2&user=" + m_sUserName + "&passwrd=" + m_sPassword;

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
        client.Headers.Add(headerCookies);

        Stream data = client.OpenRead(loginUri + "?option=com_remository&itemid=" + m_iMainIdx);
        StreamReader reader = new StreamReader(data);
        String sPage = reader.ReadToEnd().Replace('\0', ' ');

        String RegExp = "<td><h3>.*?href=\"([^\"]*?id=(\\d*)\">([^<]*))</a>";
        String RegExpEpisode = "<dd><img.*?href=\"([^\\\"]*?id=(\\d*)\">([^<]*))</a>";
        Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
        MatchCollection matches = Engine.Matches(sPage);
        int iSeriesIdx = 0;
        int iSeasonIdx = 0;
        int iEpisodeIdx = 0;

        List<SeriesMatchResult> sortedMatchList = new List<SeriesMatchResult>();
        List<SeriesMatchResult> exactMatches = new List<SeriesMatchResult>();
        Feedback.CItem selectedSeries = null;

        foreach (Match match in matches)
        {
          SeriesMatchResult result = new SeriesMatchResult(match.Groups[3].Value.ToLower(), match.Groups[2].Value);
          result.ComputeDistance(episode);
          sortedMatchList.Add(result);
        }

        sortedMatchList.Sort();

        
        if (sortedMatchList.Count > 0)
        {
          MPTVSeriesLog.Write(String.Format("Found {0} series/season entries in the page", sortedMatchList.Count));
          foreach (SeriesMatchResult result in sortedMatchList)
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
            iSeriesIdx = Convert.ToInt32(exactMatches[0].sIdx);
          }
          else
          {
            List<Feedback.CItem> Choices = new List<Feedback.CItem>();

            foreach (SeriesMatchResult match in exactMatches)
            {
              Choices.Add(new Feedback.CItem(match.sSubFullName, String.Empty, match.sIdx));
            }

            Feedback.CDescriptor seriesSelector = new Feedback.CDescriptor();
            seriesSelector.m_sTitle = "Choose correct series";
            seriesSelector.m_sItemToMatchLabel = "Local series:";
            seriesSelector.m_sItemToMatch = episode.m_sSeriesName;
            seriesSelector.m_sListLabel = "Available series:";
            seriesSelector.m_List = Choices;
            seriesSelector.m_sbtnIgnoreLabel = String.Empty;

            if (m_feedback.ChooseFromSelection(seriesSelector, out selectedSeries) == Feedback.ReturnCode.OK)
            {
              iSeriesIdx = Convert.ToInt32(selectedSeries.m_Tag as String);
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
            foreach (SeriesMatchResult match in sortedMatchList)
            {
              Choices.Add(new Feedback.CItem(match.sSubFullName.Trim(), String.Empty, match.sIdx));
            }

            Feedback.CDescriptor descriptor = new Feedback.CDescriptor();
            descriptor.m_sTitle = "Choose correct series";
            descriptor.m_sItemToMatchLabel = "Local series:";
            descriptor.m_sItemToMatch = episode.m_sSeriesName;
            descriptor.m_sListLabel = "Available series:";
            descriptor.m_List = Choices;
            descriptor.m_sbtnIgnoreLabel = String.Empty;

            Feedback.CItem Selected = null;
            if (m_feedback.ChooseFromSelection(descriptor, out Selected) == Feedback.ReturnCode.OK)
            {
              iSeriesIdx = Convert.ToInt32(Selected.m_Tag as String);
            }
          }
        }

        if (iSeriesIdx == 0)
        {
          MPTVSeriesLog.Write("NO Series avalilable: (Name = " + episode.m_sSeriesName + ")");
        }
        else
        {
          MPTVSeriesLog.Write("Series found: (idx = " + iSeriesIdx + ")");

          data = client.OpenRead(loginUri + "?option=com_remository&itemid=" + m_iMainIdx + "&func=select&id=" + iSeriesIdx);
          reader = new StreamReader(data);
          sPage = reader.ReadToEnd().Replace('\0', ' ');
          matches = Engine.Matches(sPage);
          
          foreach (Match match in matches)
          {
            if (match.Groups[3].Value.Trim() == "Stagione " + episode.m_nSeasonIndex)
            {
              iSeasonIdx = Convert.ToInt32(match.Groups[2].Value);
              break;
            }
          }

          if (iSeasonIdx == 0 && matches.Count > 0 )
          {            
            List<Feedback.CItem> Choices = new List<Feedback.CItem>();
            foreach (Match match in matches)
            {
              Choices.Add(new Feedback.CItem(match.Groups[3].Value.Trim(), String.Empty, match.Groups[2].Value));
            }

            Feedback.CDescriptor descriptor = new Feedback.CDescriptor();
            descriptor.m_sTitle = "Choose correct Season";
            descriptor.m_sItemToMatchLabel = "Local Season index:";
            descriptor.m_sItemToMatch = episode.m_nSeasonIndex + "";
            descriptor.m_sListLabel = "Available seasons list:";
            descriptor.m_List = Choices;
            descriptor.m_sbtnIgnoreLabel = String.Empty;

            Feedback.CItem Selected = null;
            if (m_feedback.ChooseFromSelection(descriptor, out Selected) == Feedback.ReturnCode.OK)
            {
              iSeasonIdx = Convert.ToInt32(Selected.m_Tag as String);
            }
          }

          if (iSeasonIdx == 0) {
            MPTVSeriesLog.Write("NO Season avalilable: (Season = " + episode.m_nSeasonIndex + ")");
          }
          else
          {
            MPTVSeriesLog.Write("Season found: (idx = " + iSeasonIdx + ")");
            data = client.OpenRead(loginUri + "?option=com_remository&itemid=" + m_iMainIdx + "&func=select&id=" + iSeasonIdx);
            reader = new StreamReader(data);
            sPage = reader.ReadToEnd().Replace('\0', ' ');

            //Find other file version
            Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
            matches = Engine.Matches(sPage);

            //Find default file version
            Engine = new Regex(RegExpEpisode, RegexOptions.IgnoreCase);
            MatchCollection fileMatches = Engine.Matches(sPage);
            Feedback.CItem selectedVersion = null;
            String sSelectedVersionTag = null;

            if (matches.Count > 0)
            {
              List<Feedback.CItem> Choices  = new List<Feedback.CItem>();
              
              Feedback.CDescriptor versionSelector = new Feedback.CDescriptor();
              versionSelector.m_sTitle = "Select desired subtitle version";
              versionSelector.m_sItemToMatchLabel = "Episdode:";
              versionSelector.m_sItemToMatch = episode.m_sSeriesName + " " + episode.m_nSeasonIndex + "x" + episode.m_nEpisodeIndex;
              versionSelector.m_sListLabel = "Version:";
              versionSelector.m_List = Choices;
              versionSelector.m_sbtnIgnoreLabel = String.Empty;

              //add default version if available
              if (fileMatches.Count >0)
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
              data = client.OpenRead(loginUri + "?option=com_remository&itemid=" + m_iMainIdx + "&func=select&id=" + selectedVersion.m_Tag as String);
              reader = new StreamReader(data);
              sPage = reader.ReadToEnd().Replace('\0', ' ');
              Engine = new Regex(RegExpEpisode, RegexOptions.IgnoreCase);
              fileMatches = Engine.Matches(sPage);
            }

            foreach (Match match in fileMatches)
            {
              String ep = episode.m_sSeriesName + " " + episode.m_nSeasonIndex + "x" + String.Format("{0:00}", episode.m_nEpisodeIndex);
              if (match.Groups[3].Value.Trim().ToLower() == ep.ToLower())
              {
                iEpisodeIdx = Convert.ToInt32(match.Groups[2].Value);
                break;
              }
            }

            if (iEpisodeIdx == 0 && fileMatches.Count > 0)
            {
              List<Feedback.CItem> Choices = new List<Feedback.CItem>();
              foreach (Match match in fileMatches)
              {
                Choices.Add(new Feedback.CItem(match.Groups[3].Value.Trim(), String.Empty, match.Groups[2].Value));
              }

              Feedback.CDescriptor descriptor = new Feedback.CDescriptor();
              descriptor.m_sTitle = "Choose correct Episode";
              descriptor.m_sItemToMatchLabel = "Local Episiode index:";
              descriptor.m_sItemToMatch = episode.m_sSeriesName + " " + episode.m_nSeasonIndex + "x" + episode.m_nEpisodeIndex;
              descriptor.m_sListLabel = "Available Episode list:";
              descriptor.m_List = Choices;
              descriptor.m_sbtnIgnoreLabel = String.Empty;

              Feedback.CItem Selected = null;
              if (m_feedback.ChooseFromSelection(descriptor, out Selected) == Feedback.ReturnCode.OK)
              {
                iEpisodeIdx = Convert.ToInt32(Selected.m_Tag as String);
              }
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

              RegExp = "<h2><a href=\"([^\\\"]*fname=([^\\\"]*))";
              Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
              matches = Engine.Matches(sPage);

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
                    Feedback.CDescriptor descriptor = new Feedback.CDescriptor();
                    descriptor.m_sTitle = "Select matching subtitle file";
                    descriptor.m_sItemToMatchLabel = "Episdode:";
                    descriptor.m_sItemToMatch = episode.m_sSeriesName + " " + episode.m_nSeasonIndex + "x" + episode.m_nEpisodeIndex;
                    descriptor.m_sListLabel = "Matching subtitles:";
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
                      MPTVSeriesLog.Write("File " + targetFile + " found: deleting");
                      System.IO.File.Delete(targetFile);
                    }
                    System.IO.File.Move(selectedFile, targetFile);
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

  class RemositoryEpisode: SubtitleEpisode
  {
    public RemositoryEpisode(String sSeriesName, String sFileName, int nSeasonIndex, int nEpisodeIndex)
    : base(sSeriesName,sFileName,nSeasonIndex,nEpisodeIndex)
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

  public class SeriesMatchResult : IComparable<SeriesMatchResult>
  {
    public String sSubFullName = String.Empty;
    public String sIdx = String.Empty;

    // for sorting
    public int nDistance = 0xFFFF;

    public int CompareTo(SeriesMatchResult other)
    {
      return nDistance.CompareTo(other.nDistance);
    }

    public SeriesMatchResult(String sName, String sIndex)
    {
      sSubFullName = sName.ToLower();
      sIdx = sIndex;
    }

    public void ComputeDistance(SubtitleEpisode episode)
    {
      nDistance = MediaPortal.Util.Levenshtein.Match(sSubFullName, episode.m_sSeriesName.ToLower());
    }
  };
}

