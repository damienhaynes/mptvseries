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
  class SeriesSubs
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

    public SeriesSubs(Feedback.IFeedback feedback)
	{
	  m_sBaseUrl = DBOption.GetOptions(DBOption.cSubs_SeriesSubs_BaseURL);
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
	  MPTVSeriesLog.Write("Starting SeriesSubs Subtitles retrieval");
	  MPTVSeriesLog.Write("**********************************");

	  System.Threading.Thread.CurrentThread.Priority = ThreadPriority.Lowest;
	  try
	  {
		WebClient client = new WebClient();

		DBOnlineSeries series = new DBOnlineSeries(m_dbEpisode[DBEpisode.cSeriesID]);
		DBSeason season = new DBSeason(m_dbEpisode[DBEpisode.cSeriesID], m_dbEpisode[DBEpisode.cSeasonIndex]);
		SeriesSubsSubtitleEpisode episode = new SeriesSubsSubtitleEpisode(series[DBOnlineSeries.cOriginalName], m_dbEpisode[DBEpisode.cFilename], m_dbEpisode[DBEpisode.cSeasonIndex], m_dbEpisode[DBEpisode.cEpisodeIndex]);
		SeriesSubsSeriesMatchResult finalSeriesResult = null;

		String sLocalSeriesName = episode.m_sSeriesName;

		if (sLocalSeriesName.Length > 0 && m_sBaseUrl.Length > 0)
		{
		  String s1stLevelURL = m_sBaseUrl;
		  MPTVSeriesLog.Write("Step 1: looking into " + s1stLevelURL);
		  Stream data = client.OpenRead(s1stLevelURL);
		  StreamReader reader = new StreamReader(data);
		  String sPage = reader.ReadToEnd().Replace('\0', ' ');
		  data.Close();
		  reader.Close();

		  String RegExp = @"<div style=""float: left;"">\s*<[^>]*>\s*<a href=\""([^""]*)\"">([^<]*)";
		  Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
		  MatchCollection matches = Engine.Matches(sPage);
		  List<SeriesSubsSeriesMatchResult> sortedMatchList = new List<SeriesSubsSeriesMatchResult>();
		  foreach (Match match in matches)
		  {
			SeriesSubsSeriesMatchResult result = new SeriesSubsSeriesMatchResult(match.Groups[2].Value, match.Groups[1].Value);
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
			foreach (SeriesSubsSeriesMatchResult match in sortedMatchList)
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
			  finalSeriesResult = Selected.m_Tag as SeriesSubsSeriesMatchResult;
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
		  MPTVSeriesLog.Write("Step 2: looking into " + s2ndLevelURL);
		  Stream data = client.OpenRead(s2ndLevelURL);
		  StreamReader reader = new StreamReader(data);
		  String sPage = reader.ReadToEnd().Replace('\0', ' ');
		  data.Close();
		  reader.Close();

		  String RegExp = @"<td class=""gst_fichier"">\s*<img[^>]*>\s*<a href=\""([^""]*)\"">\s*Saison\s*(\d)*\s*</a>\s*</td>";
		  Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
		  MatchCollection matches = Engine.Matches(sPage);
		  foreach (Match match in matches)
		  {
			if (Convert.ToInt32(match.Groups[2].Value) == episode.m_nSeasonIndex)
			{
			  sSeasonUrl = match.Groups[1].Value;
			  break;
			}
		  }
		}

		if (sSeasonUrl != String.Empty)
		{
		  List<SeriesSubsEpisodeMatchResult> matchList = new List<SeriesSubsEpisodeMatchResult>();
		  String s3rdLevelURL = sSeasonUrl;

		  {
			MPTVSeriesLog.Write("Step 3: looking into " + s3rdLevelURL);
			Stream data = client.OpenRead(s3rdLevelURL);
			StreamReader reader = new StreamReader(data);
			String sPage = reader.ReadToEnd().Replace('\0', ' ');
			data.Close();
			reader.Close();

			String RegExp = @"<td class=""gst_fichier"">\s*<img[^>]*>\s*<a href=\""([^""]*)\"">([^<]*)";
			Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
			MatchCollection matches = Engine.Matches(sPage);
			if (matches.Count == 0)
			{
			  MPTVSeriesLog.Write("Error: no episodes found in the series/season page");
			}
			foreach (Match match in matches)
			{
			  SeriesSubsEpisodeMatchResult result = new SeriesSubsEpisodeMatchResult(match.Groups[2].Value, match.Groups[1].Value);
			  // match season index & episode index
			  if (result.m_nSeasonIndex == episode.m_nSeasonIndex && episode.m_nEpisodeIndex >= result.m_nEpisodeIndexFirst && episode.m_nEpisodeIndex <= result.m_nEpisodeIndexLast)
			  {
				MPTVSeriesLog.Write(String.Format("Found a matching episode ({0})", result.m_sName));
				result.ComputeDistance(episode);
				bool bFound = false;
				foreach (SeriesSubsEpisodeMatchResult matchFind in matchList)
				  if (matchFind.m_sName == result.m_sName)
				  {
					bFound = true;
					break;
				  }
				if (!bFound)
				  matchList.Add(result);
			  }
			}
		  }

		  MPTVSeriesLog.Write(String.Format("{0} matching subtitles Found", matchList.Count));

		  List<SeriesSubsEpisodeMatchResult> sortedMatchList = new List<SeriesSubsEpisodeMatchResult>();

		  // process rars or zips if any
		  foreach (SeriesSubsEpisodeMatchResult result in matchList)
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
					SeriesSubsEpisodeMatchResult extractedFile = new SeriesSubsEpisodeMatchResult(file, "file://" + System.IO.Path.GetTempPath() + file);
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
					  SeriesSubsEpisodeMatchResult extractedFile = new SeriesSubsEpisodeMatchResult(fileName, "file://" + System.IO.Path.GetTempPath() + theEntry.Name);
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
			SeriesSubsEpisodeMatchResult finalEpisodeResult = null;

			if (sortedMatchList.Count == 1)
			  finalEpisodeResult = sortedMatchList[0];
			else
			{
			  // now, sort & take the first one as our best result
			  List<Feedback.CItem> Choices = new List<Feedback.CItem>();
			  foreach (SeriesSubsEpisodeMatchResult result in sortedMatchList)
			  {
				Choices.Add(new Feedback.CItem(result.m_sName, String.Empty, result));
			  }

			  // ask the user
			  Feedback.ChooseFromSelectionDescriptor descriptor = new Feedback.ChooseFromSelectionDescriptor();
			  descriptor.m_sTitle = "Select matching subtitle file";
			  descriptor.m_sItemToMatchLabel = "filename:";
			  descriptor.m_sItemToMatch = Path.GetFileNameWithoutExtension(episode.m_sFileName);
			  descriptor.m_sListLabel = "Matching subtitles:";
			  descriptor.m_List = Choices;
			  descriptor.m_sbtnIgnoreLabel = String.Empty;

			  Feedback.CItem Selected = null;
			  if (m_feedback.ChooseFromSelection(descriptor, out Selected) == Feedback.ReturnCode.OK)
			  {
				finalEpisodeResult = Selected.m_Tag as SeriesSubsEpisodeMatchResult;
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
		  foreach (SeriesSubsEpisodeMatchResult result in matchList)
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
		  MPTVSeriesLog.Write("SeriesSubs Subtitles retrieval ended");
		  MPTVSeriesLog.Write("*******************************");
		}
	  }

	  catch (Exception ex)
	  {
		MPTVSeriesLog.Write("Could not do SeriesSubs Subtitle retrival: " + ex.Message, MPTVSeriesLog.LogLevel.Normal);
	  }
	}
  }

  public class SeriesSubsSubtitleEpisode
  {
	public String m_sSeriesName = String.Empty;
	public String m_sFileName = String.Empty;
	public int m_nSeasonIndex = 0;
	public int m_nEpisodeIndex = 0;

	public SeriesSubsSubtitleEpisode(String sSeriesName, String sFileName, int nSeasonIndex, int nEpisodeIndex)
	{
	  m_sSeriesName = sSeriesName;
	  m_sFileName = sFileName;
	  m_nSeasonIndex = nSeasonIndex;
	  m_nEpisodeIndex = nEpisodeIndex;
	}
  };

  class SeriesSubsEpisodeMatchResult : IComparable<SeriesSubsEpisodeMatchResult>
  {
	public String m_sName = String.Empty;
	public String m_sLink = String.Empty;
	public int m_nSeasonIndex = 0;
	public int m_nEpisodeIndexFirst = 0xFFFF;
	public int m_nEpisodeIndexLast = 0;

	// for sorting
	public int m_nDistance = 0xFFFF;

	public SeriesSubsEpisodeMatchResult(String sName, String sLink)
	{
	  m_sName = sName;
	  m_sLink = sLink;

	  String RegExp = @"[^\\$]*?(?:s(?<season>[0-1]?\d)ep?(?<episode>\d\d)|(?<season>(?:[0-1]\d|(?<!\d)\d))x?(?<episode>\d\d))(?!\d)(?:[ .-@]?(?:s\k<season>e?(?<episode2>\d{2}(?!\d))|\k<season>x?(?<episode2>\d{2}(?!\d))|(?<episode2>\d\d(?!\d))|E(?<episode2>\d\d))|)[ -.]*(?<title>(?!.*sample)[^\\]*?[^\\]*?)\.(?<ext>[^.]*)$";
	  // extract language & name
	  Regex subEngine = new Regex(RegExp, RegexOptions.IgnoreCase);
	  Match subMatch = subEngine.Match(sName);
	  if (subMatch.Success)
	  {
		m_nSeasonIndex = Convert.ToInt32(subMatch.Groups["season"].Value);
		m_nEpisodeIndexFirst = Convert.ToInt32(subMatch.Groups["episode"].Value);
		if (subMatch.Groups["episode2"].Success)
		  m_nEpisodeIndexLast = Convert.ToInt32(subMatch.Groups["episode2"].Value);
		else
		  m_nEpisodeIndexLast = m_nEpisodeIndexFirst;
	  }
	}

	public int CompareTo(SeriesSubsEpisodeMatchResult other)
	{
	  return m_nDistance.CompareTo(other.m_nDistance);
	}

	public void ComputeDistance(SeriesSubsSubtitleEpisode episode)
	{
	  m_nDistance = MediaPortal.Util.Levenshtein.Match(Path.GetFileNameWithoutExtension(m_sName).ToLower(), Path.GetFileNameWithoutExtension(episode.m_sFileName).ToLower());
	}
  };

  class SeriesSubsSeriesMatchResult : IComparable<SeriesSubsSeriesMatchResult>
  {
	public String sName = String.Empty;
	public String sSubLink = String.Empty;

	// for sorting
	public int nDistance = 0xFFFF;

	public int CompareTo(SeriesSubsSeriesMatchResult other)
	{
	  return nDistance.CompareTo(other.nDistance);
	}

	public SeriesSubsSeriesMatchResult(String sName, String sLink)
	{
	  this.sName = sName;
	  this.sSubLink = sLink;
	}

	public void ComputeDistance(SeriesSubsSubtitleEpisode episode)
	{
	  nDistance = MediaPortal.Util.Levenshtein.Match(sName.ToLower(), episode.m_sSeriesName.ToLower());
	}
  };
}

