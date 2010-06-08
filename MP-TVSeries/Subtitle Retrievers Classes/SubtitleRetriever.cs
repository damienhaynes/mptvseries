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
using System.ComponentModel;
using System.IO;
using System.Threading;
using SubtitleDownloader.Core;
using WindowPlugins.GUITVSeries.Feedback;

namespace WindowPlugins.GUITVSeries.Subtitles
{
    public class SubtitleRetriever
    {
        public delegate void SubtitleRetrievalCompletedHandler(bool bFound, string message);

        public event SubtitleRetrievalCompletedHandler SubtitleRetrievalCompleted;

        private readonly BackgroundWorker worker;
        
        private readonly IFeedback feedback;

        private bool _subtitleRetrieved = false;

        private string _errorMessage;

        protected readonly ISubtitleDownloader Downloader;

        protected DBEpisode Episode;

        protected DBOnlineSeries Series;
        
        protected int SeasonIndex;

        protected int EpisodeIndex;

        protected string SeriesName;

        protected string EpisodeFileName;

        protected string EpisodeFileNameWithoutExtension;

        public SubtitleRetriever(IFeedback feedback, ISubtitleDownloader downloader)
        {
            this.Downloader = downloader;
            this.feedback = feedback;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += WorkerDoWork;
            worker.RunWorkerCompleted += WorkerRunWorkerCompleted;
        }

        public void GetSubs(DBEpisode episode)
        {
            MPTVSeriesLog.Write("Searching for subtitles...");

            Episode = episode;

            Series = new DBOnlineSeries(this.Episode[DBEpisode.cSeriesID]);
            DBUserSelection seriesSelection = new DBUserSelection(SelectionLevel.series, SelectionType.subtitles, Series[DBSeries.cID]);

            SeriesName = Series[DBOnlineSeries.cOriginalName];
            if (seriesSelection.Enabled) SeriesName = seriesSelection[DBUserSelection.cUserKey];

            EpisodeIndex = Episode[DBEpisode.cEpisodeIndex];
            SeasonIndex = Episode[DBEpisode.cSeasonIndex];

            EpisodeFileName = Episode[DBEpisode.cFilename];
            EpisodeFileNameWithoutExtension = Path.GetFileNameWithoutExtension(EpisodeFileName);

            worker.RunWorkerAsync();
        }

        public List<Subtitle> PerformSearch(string[] languageCodes)
        {
            EpisodeSearchQuery query = new EpisodeSearchQuery(SeriesName, SeasonIndex, EpisodeIndex);
            query.LanguageCodes = languageCodes;

            return Downloader.SearchSubtitles(query);   
        }

        void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (SubtitleRetrievalCompleted != null) // only if any subscribers exist
            {
                SubtitleRetrievalCompleted.Invoke(_subtitleRetrieved, _errorMessage);
            }
        }

        void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            List<Subtitle> subtitles = PerformSearch(GetConfiguredLanguages());

            MPTVSeriesLog.Write("Searching for subtitles completed. Found " + subtitles.Count + " subtitle(s).");

            if (subtitles.Count > 0)
            {
                List<CItem> choices = GetSubtitleChoices(subtitles);

                CItem selected = ChooseSubtitleDialog(SeriesName, SeasonIndex, EpisodeIndex, Episode[DBEpisode.cFilenameWOPathAndExtension], choices);

                if (selected != null)
                {
                    Subtitle subtitle = (Subtitle) selected.m_Tag;
                    DownloadAndSaveSubtitle(subtitle);
                }
            }
        }

        private void DownloadAndSaveSubtitle(Subtitle subtitle)
        {
            MPTVSeriesLog.Write("Downloading subtitle...");

            try
            {
                List<FileInfo> originalFiles = Downloader.SaveSubtitle(subtitle);

                if (!File.Exists(EpisodeFileName))
                {
                    _errorMessage = "Unable to download subtitle: Not a local episode!";
                    MPTVSeriesLog.Write(_errorMessage);
                    return;
                }
                if (originalFiles.Count != 1)
                {
                    _errorMessage = "Download returned " + originalFiles.Count + " subtitle files. Count mismatch, should be one!";
                    MPTVSeriesLog.Write(_errorMessage);
                    return;
                }

                FileInfo originalFile = originalFiles[0];

                string destinationFile =
                    SubtitleDownloader.Util.FileUtils.GetFileNameForSubtitle(
                        originalFile.FullName, subtitle.LanguageCode, EpisodeFileName);

                if (File.Exists(destinationFile))
                {
                    bool deleteExisting = DeleteExistingFileDialog();

                    if (deleteExisting)
                    {
                        File.Delete(destinationFile);
                        File.Move(originalFile.FullName, destinationFile);
                        _subtitleRetrieved = true;
                    }
                }
                else
                {
                    File.Move(originalFile.FullName, destinationFile);
                    _subtitleRetrieved = true;
                }
                MPTVSeriesLog.Write("Downloading subtitle completed.");
            }
            catch (Exception e)
            {
                _errorMessage = "Downloading subtitle file failed: " + e.Message;
                MPTVSeriesLog.Write(_errorMessage);
                throw;
            }
        }

        private CItem ChooseSubtitleDialog(string seriesName, int seasonIndex, int episodeIndex, string fileName, List<CItem> choices)
        {
            ChooseFromSelectionDescriptor descriptor = new ChooseFromSelectionDescriptor();
            if (DBOption.GetOptions(DBOption.cUseFullNameInSubDialog))
            {
                if (Settings.isConfig)
                {
                    descriptor.m_sTitle = Translation.CFS_Choose_Correct_Episode;
                    descriptor.m_sItemToMatchLabel = Translation.CFS_Local_Episode_Index;
                    descriptor.m_sItemToMatch = fileName;
                }
                else
                {
                    descriptor.m_sTitle = fileName;
                    descriptor.m_sItemToMatchLabel = string.Empty;
                    descriptor.m_sItemToMatch = string.Empty;
                }
            }
            else 
            {
                descriptor.m_sTitle = Translation.CFS_Choose_Correct_Episode;
                descriptor.m_sItemToMatchLabel = Translation.CFS_Local_Episode_Index;
                descriptor.m_sItemToMatch = seriesName + " " + seasonIndex + "x" + episodeIndex;
            }
            descriptor.m_sListLabel = Translation.CFS_Available_Episode_List;
            descriptor.m_List = choices;
            descriptor.m_sbtnIgnoreLabel = String.Empty;

            CItem selected;

            if (feedback.ChooseFromSelection(descriptor, out selected) == ReturnCode.OK)
            {
               return selected;
            }
            return null;
        }

        private List<CItem> GetSubtitleChoices(List<Subtitle> subtitles)
        {
            List<CItem> choices = new List<CItem>();
                
            foreach (Subtitle subtitle in subtitles)
            {
                choices.Add(new CItem("[" + subtitle.LanguageCode + "] " + subtitle.FileName, String.Empty, subtitle));
            }
            return choices;
        }

        private bool DeleteExistingFileDialog()
        {
            ChooseFromYesNoDescriptor descriptor = new ChooseFromYesNoDescriptor();
            descriptor.m_sTitle = Translation.CYN_Subtitle_File_Replace;
            descriptor.m_sLabel = Translation.CYN_Old_Subtitle_Replace;
            descriptor.m_dialogButtons = DialogButtons.YesNo;
            descriptor.m_dialogDefaultButton = ReturnCode.Yes;
            if (feedback.YesNoOkDialog(descriptor) == ReturnCode.Yes)
            {
                return true;
            }
            return false;
        }

        private string[] GetConfiguredLanguages()
        {
            List<String> result = new List<string>();

            String languages = DBOption.GetOptions(DBOption.cSubtitleDownloaderLanguages);

            string[] splitted = languages.Split('|');

            foreach (var s in splitted)
            {
                if (s.Length == 3)
                    result.Add(s);
            }

            return result.ToArray();
        } 
    }
}

