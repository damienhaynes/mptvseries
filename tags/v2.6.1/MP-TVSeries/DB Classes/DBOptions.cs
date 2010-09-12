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
using SQLite.NET;
using MediaPortal.Database;

namespace WindowPlugins.GUITVSeries
{
    public class DBOption
    {
        public static bool bTableUpdateDone = false;

        public const String cConfig_LogCollapsed = "Config_LogShown";

        public const String cDBSeriesVersion = "DBSeriesVersion";
        public const String cDBOnlineSeriesVersion = "DBOnlineSeriesVersion";
        public const String cDBSeriesLastLocalID = "DBSeriesLasLocalID";
        public const String cDBSeasonVersion = "DBSeasonVersion";
        public const String cDBEpisodesVersion = "DBEpisodesVersion";
        public const String cDBExpressionsVersion = "DBExpressionsVersion";
        public const String cDBNewzbinVersion = "DBNewzbinVersion";
        public const String cDBTorrentVersion = "dbTorrentVersion";
        public const String cDBViewsVersion = "DBViewsVersion";
        public const String cDBReplacementsVersion = "DBReplacementsVersion";
        public const String cDBUserSelectionsVersion = "DBUserSelectionsVersion";
        public const String cDBIgnoredDownloadedFilesVersion = "DBIgnoredDownloadedFilesVersion";

        public const String cShowHiddenItems = "ShowHiddenItems";
        public const String cOnlineParseEnabled = "OnlineParseEnabled";
        public const String cFullSeriesRetrieval = "FullSeriesRetrieval";
        public const String cAutoChooseSeries = "AutoChooseSeries";
        public const String cAutoChooseOrder = "AutoChooseOrder";

        public const String cImport_FolderWatch = "doFolderWatch";
        public const String cImport_ScanRemoteShare = "scanRemoteShare";
        public const String cImport_ScanOnStartup = "ScanOnStartup";
        public const String cImport_ScanWhileFullscreenVideo = "AutoScanLocalFilesFSV";
        public const String cImport_ScanRemoteShareLapse = "AutoScanLocalFilesLapse";
        public const String cImport_AutoUpdateOnlineData = "AutoUpdateOnlineData";
        public const String cImport_AutoUpdateOnlineDataLapse = "AutoUpdateOnlineDataLapse";
        public const String cImport_DontClearMissingLocalFiles = "DontClearMissingLocalFiles";        
        public const String cImport_OnlineUpdateScanLastTime = "UpdateScanLastTime";

        public const String cPlay_SubtitleDownloadOnPlay = "SubtitleDownloadOnPlay";

        public const String cView_PluginName = "View_PluginName";
        public const String cView_Episode_OnlyShowLocalFiles = "View_Episode_OnlyShowLocalFiles";
        public const String cView_Episode_HideUnwatchedSummary = "View_Episode_HideUnwatchedSummary";
        public const String cView_Episode_HideUnwatchedThumbnail = "View_Episode_HideUnwatchedThumbnail";
        public const String cViewAutoHeight = "ViewAutoHeight";

        public const String cView_Series_ListFormat = "View_Series_ListFormat";
        public const String cView_Series_Col1 = "View_Series_Col1";
        public const String cView_Series_Col2 = "View_Series_Col2";
        public const String cView_Series_Col3 = "View_Series_Col3";
        public const String cView_Series_Title = "View_Series_Title";
        public const String cView_Series_Subtitle = "View_Series_Secondary";
        public const String cView_Series_Main = "View_Series_Main";

        public const String cSeries_UseSortName = "Series_UseSortName";

        public const String cView_Season_ListFormat = "View_Season_ListFormat";
        public const String cView_Season_Col1 = "View_Season_Col1";
        public const String cView_Season_Col2 = "View_Season_Col2";
        public const String cView_Season_Col3 = "View_Season_Col3";
        public const String cView_Season_Title = "View_Season_Title";
        public const String cView_Season_Subtitle = "View_Season_Secondary";
        public const String cView_Season_Main = "View_Season_Main";

        public const String cView_Episode_Col1 = "View_Episode_Col1";
        public const String cView_Episode_Col2 = "View_Episode_Col2";
        public const String cView_Episode_Col3 = "View_Episode_Col3";
        public const String cView_Episode_Title = "View_Episode_Title";
        public const String cView_Episode_Subtitle = "View_Episode_Secondary";
        public const String cView_Episode_Main = "View_Episode_Main";

        public const String cUTorrentPath = "uTorrentPath";
        public const String cUTorrentDownloadPath = "uTorrentDownloadPath";
        public const String cTorrentSearch = "TorrentSearch_Current";

        public const String cNewsLeecherPath = "NewsLeecherPath";
        public const String cNewsLeecherDownloadPath = "NewsLeecherDownloadPath";

        public const String cDownloadMonitor_RenameFiles = "DownloadMonitor_RenameFiles";

        public const String cRandomBanner = "randomBanner";
        public const String cLanguage = "lang";

        public const String cUpdateBannersTimeStamp = "UpdateBannersTimeStamp"; // not used anymore

        public const String cUsesNewPathFormat = "usesNewPathFormat";

        public const String cOnlineLanguage = "onlineLanguage";

        public const String cShowDeleteMenu = "ShowDeleteMenu";

        public const String cMainMirror = "mainMirror";
        public const String cGetBlankBanners = "getBlankBanners";
        public const String cGetEpisodeSnapshots = "getEpisodeSnapshots";

        public const String cWatchedAfter = "watchedAfter";

        public const String cAltImgLoading = "altImageLoading";

        public const String cNewAPIUpgradeDone = "newAPIUpgradeDone";
        public const String cUpdateTimeStamp = "UpdateTimeStamp"; // new one and only timestamp
        public const String cOnlineUserID = "onlineUserID";
        public const String cAskToRate = "askToRate";
        public const String cswitchViewsFast = "switchViewsFast";

        public const String cAppendFirstLogoToList = "appendFirstLogoToList";
        public const String cGraphicalGroupView = "graphicalGroupView";

        public const String cQualitySeriesBanners = "QualitySeriesBanners";
        public const String cQualitySeasonBanners = "QualitySeasonBanners";
        public const String cQualityEpisodeImages = "QualityEpisodeImages";
        public const String cQualitySeriesPosters = "QualityPosterImages";

        public const String cGetSeriesPosters = "getSeriesPosters";
        public const String cShowSeriesFanart = "showSeriesFanart";

        public const String cFanartRandom = "FanartRandom";        

        public const String m_sMainMirror = "http://thetvdb.com";

        public const String cOnlineFavourites = "UseOnlineFavourites";
        public const String cPlaylistPath = "PlayListPath";
        public const String cRepeatPlaylist = "RepeatPlaylist";
        public const String cPlaylistAutoPlay = "PlaylistAutoPlay";
		public const String cPlaylistAutoShuffle = "PlaylistAutoShuffle";
        public const String cPlaylistUnwatchedOnly = "PlaylistUnwatchedOnly";

        public const String cAutoDownloadMissingArtwork = "AutoDownloadMissingArtwork";
        public const String cAutoUpdateEpisodeRatings = "AutoUpdateEpisodeRatings";
        public const String cAutoUpdateAllFanart = "AutoUpdateAllFanart";

        public const String cAutoDownloadFanart = "AutoDownloadFanart";
        public const String cAutoDownloadFanartCount = "AutoDownloadFanartCount";
        public const String cAutoDownloadFanartResolution = "AutoDownloadFanartResolution";
        public const String cAutoDownloadFanartSeriesNames = "AutoDownloadFanartSeriesNames";

        public const String cFanartThumbnailResolutionFilter = "FanartThumbnailResolutionFilter";
        public const String cFanartCurrentView = "FanartCurrentView";
       
        public const String cUseRegionalDateFormatString = "UseRegionalDateFormatString";

		public const String cDefaultRating = "DefaultRating";
		public const String cRatingDisplayStars = "RatingDisplayStars";

        public const String cSortSpecials = "SortSpecials";
        public const String cBackdropLoadingDelay = "BackdropLoadingDelay";
        public const String cArtworkLoadingDelay = "ArtworkLoadingDelay";
        public const String cRandomFanartInterval = "RandomFanartInterval";
        public const String cParentalControlPinCode = "ParentalControlPinCode";
		public const String cKeyboardStyle = "KeyboardStyle";
		public const String cMarkRatedEpisodeAsWatched = "MarkRatedEpisodeAsWatched";

        public const String cSubtitleDownloaderEnabled = "SubtitleDownloaderEnabled";
        public const String cSubtitleDownloaderLanguages = "SubtitleDownloaderLanguages";
        public const String cSubtitleDownloadersEnabled = "SubtitleDownloadersEnabled";

        public const String cSubstituteMissingArtwork = "SubstituteMissingArtwork";

        public const String cSkipSeasonViewOnSingleSeason = "SkipSeasonViewOnSingleSeason";

        public const String cInvokeExtBeforePlayback = "InvokeExtBeforePlayback";
        public const String cInvokeExtBeforePlaybackArgs = "InvokeExtBeforePlaybackArgs";
        public const String cInvokeExtBeforePlaybackWaitForExit = "InvokeExtBeforePlaybackWaitForExit";
        public const String cInvokeExtAfterPlayback = "InvokeExtAfterPlayback";
        public const String cInvokeExtAfterPlaybackArgs = "InvokeExtAfterPlaybackArgs";
        public const String cInvokeExtAfterPlaybackWaitForExit = "InvokeExtAfterPlaybackWaitForExit";

        public const String cCountEmptyAndFutureAiredEps = "CountEmptyAndFutureAiredEps";

        public const String cOnPlaySeriesOrSeasonAction = "OnPlaySeriesOrSeasonAction";

        public const String cNewEpisodeThumbType = "NewEpisodeThumbType";

        public const String cLogLevel = "logLevel";

        public const String cSubCentralEnabled = "SubCentralEnabled";

        public const String cSubCentralEnabledForEpisodes = "SubCentralEnabledForEpisodes";

        public const String cUseFullNameInSubDialog = "UseFullNameInSubDialog";

        public const String cConfigSizeHeight = "configSizeHeight";
        public const String cConfigSizeWidth = "configSizeWidth";

        public const String cDisableMediaInfo = "DisableMediaInfo";
        public const String cMediaInfoParseSpeed = "MediaInfoParseSpeed";

        public const String cImportDelay = "ImportDelay";

        private static Dictionary<string, DBValue> optionsCache = new Dictionary<string, DBValue>();

        private const string cCreateTableQuery = "CREATE TABLE options (option_id integer primary key, property text, value text)";

        
        static DBOption()
        {
            try
            {
                SQLiteResultSet results = DBTVSeries.Execute("SELECT name FROM sqlite_master WHERE name='options' and type='table' UNION ALL SELECT name FROM sqlite_temp_master WHERE type='table' ORDER BY name");
                if (results == null || results.Rows.Count == 0)
                {
                    // no table, create it
                    DBTVSeries.Execute(cCreateTableQuery);
                }

                if (GetOptions(cConfig_LogCollapsed) == null)
                    SetOptions(cConfig_LogCollapsed, false);

                if (GetOptions(cDBSeriesLastLocalID) == null)
                    SetOptions(cDBSeriesLastLocalID, -1);

                // update default values if not there already
                if (GetOptions(cShowHiddenItems) == null)
                    SetOptions(cShowHiddenItems, false);

                if (GetOptions(cOnlineParseEnabled) == null)
                    SetOptions(cOnlineParseEnabled, true);

                if (GetOptions(cFullSeriesRetrieval) == null)
                    SetOptions(cFullSeriesRetrieval, false);

                if (GetOptions(cGetEpisodeSnapshots) == null)
                    SetOptions(cGetEpisodeSnapshots, true);

                if (GetOptions(cAutoChooseSeries) == null)
                    SetOptions(cAutoChooseSeries, true);
                
                if (GetOptions(cAutoChooseOrder) == null)
                    SetOptions(cAutoChooseOrder, true);

                if (GetOptions(cView_Episode_OnlyShowLocalFiles) == null)
                    SetOptions(cView_Episode_OnlyShowLocalFiles, true);

                if (GetOptions(cView_Episode_HideUnwatchedSummary) == null)
                    SetOptions(cView_Episode_HideUnwatchedSummary, false);

                if (GetOptions(cView_Episode_HideUnwatchedThumbnail) == null)
                    SetOptions(cView_Episode_HideUnwatchedThumbnail, false);
                
                if (GetOptions(cImport_FolderWatch) == null)
                    SetOptions(cImport_FolderWatch, true);

                if (GetOptions(cImport_ScanRemoteShare) == null)
                    SetOptions(cImport_ScanRemoteShare, true);

                if (GetOptions(cImport_ScanRemoteShareLapse) == null)
                    SetOptions(cImport_ScanRemoteShareLapse, 5);

                if (GetOptions(cImport_AutoUpdateOnlineData) == null)
                    SetOptions(cImport_AutoUpdateOnlineData, true);

                if (GetOptions(cImport_AutoUpdateOnlineDataLapse) == null)
                    SetOptions(cImport_AutoUpdateOnlineDataLapse, 12);

                if (GetOptions(cImport_OnlineUpdateScanLastTime) == null)
                    SetOptions(cImport_OnlineUpdateScanLastTime, 0);

                if (GetOptions(cImport_DontClearMissingLocalFiles) == null)
                    SetOptions(cImport_DontClearMissingLocalFiles, 0);

                if (GetOptions(cPlay_SubtitleDownloadOnPlay) == null)
                    SetOptions(cPlay_SubtitleDownloadOnPlay, false);

                if (GetOptions(cView_PluginName) == null)
                    SetOptions(cView_PluginName, "My TV Series");

                if (GetOptions(cViewAutoHeight) == null)
                    SetOptions(cViewAutoHeight, true);

                if (GetOptions(cSubtitleDownloaderEnabled) == null)
                    SetOptions(cSubtitleDownloaderEnabled, false);

                if (GetOptions(cSubtitleDownloaderLanguages) == null)
                    SetOptions(cSubtitleDownloaderLanguages, "");

                if (GetOptions(cSubtitleDownloadersEnabled) == null)
                    SetOptions(cSubtitleDownloadersEnabled, "");

                if (GetOptions(cView_Series_ListFormat) == null)
                    SetOptions(cView_Series_ListFormat, "WideBanners"); // Old Classic look by default
                else
                {
                    // Upgrade old cView_Series_ListFormat 
                    if (GetOptions(cView_Series_ListFormat) == 0)
                    {
                        if (GetOptions(cGetSeriesPosters) == null || GetOptions(cGetSeriesPosters) == 0)
                            SetOptions(cView_Series_ListFormat, "ListBanners");
                        else
                            SetOptions(cView_Series_ListFormat, "ListPosters");
                    }
                    else if (GetOptions(cView_Series_ListFormat) == 1)
                    {
                        if (GetOptions(cGetSeriesPosters) == null || GetOptions(cGetSeriesPosters) == 1)
                            SetOptions(cView_Series_ListFormat, "WideBanners");
                        else
                            SetOptions(cView_Series_ListFormat, "Filmstrip");
                    }
                }

                if (GetOptions(cView_Series_Col1) == null)
                    SetOptions(cView_Series_Col1, "");

                if (GetOptions(cView_Series_Col2) == null)
                    SetOptions(cView_Series_Col2, "<" + DBSeries.cOutName + "." + DBOnlineSeries.cPrettyName + ">");

                if (GetOptions(cView_Series_Col3) == null)
                    SetOptions(cView_Series_Col3, "<" + DBSeries.cOutName + "." + DBOnlineSeries.cAirsDay + ">");

                if (GetOptions(cView_Series_Title) == null)
                    SetOptions(cView_Series_Title, "<" + DBSeries.cOutName + "." + DBOnlineSeries.cPrettyName + ">");

                if (GetOptions(cView_Series_Subtitle) == null)
                    SetOptions(cView_Series_Subtitle, "<" + DBSeries.cOutName + "." + DBOnlineSeries.cGenre + ">");

                if (GetOptions(cView_Series_Main) == null)
                    SetOptions(cView_Series_Main, "<" + DBSeries.cOutName + "." + DBOnlineSeries.cSummary + ">");

                if (GetOptions(cSeries_UseSortName) == null)
                    SetOptions(cSeries_UseSortName, 0); // default sort is by pretty name

                if (GetOptions(cView_Season_ListFormat) == null)
                    SetOptions(cView_Season_ListFormat, 0); // ListView by default

                if (GetOptions(cView_Season_Col1) == null)
                    SetOptions(cView_Season_Col1, "");

                if (GetOptions(cView_Season_Col2) == null)
                    SetOptions(cView_Season_Col2, "Season <" + DBSeason.cOutName + "." + DBSeason.cIndex + ">");

                if (GetOptions(cView_Season_Col3) == null)
                    SetOptions(cView_Season_Col3, "");

                if (GetOptions(cView_Season_Title) == null)
                    SetOptions(cView_Season_Title, "<" + DBSeries.cOutName + "." + DBOnlineSeries.cPrettyName + "> Season <" + DBSeason.cOutName + "." + DBSeason.cIndex + ">");

                if (GetOptions(cView_Season_Subtitle) == null)
                    SetOptions(cView_Season_Subtitle, "<" + DBSeries.cOutName + "." + DBOnlineSeries.cGenre + ">");

                if (GetOptions(cView_Season_Main) == null)
                    SetOptions(cView_Season_Main, "<" + DBSeries.cOutName + "." + DBOnlineSeries.cSummary + ">"); 
                
                if (GetOptions(cView_Episode_Col1) == null)
                    SetOptions(cView_Episode_Col1, "");

                if (GetOptions(cView_Episode_Col2) == null)
                    SetOptions(cView_Episode_Col2, "<" + DBEpisode.cOutName + "." + DBEpisode.cEpisodeIndex + ">: <" + DBEpisode.cOutName + "." + DBEpisode.cEpisodeName + ">");

                if (GetOptions(cView_Episode_Col3) == null)
                    SetOptions(cView_Episode_Col3, "<" + DBEpisode.cOutName + "." + DBOnlineEpisode.cFirstAired + ">");

                if (GetOptions(cView_Episode_Title) == null)
                    SetOptions(cView_Episode_Title, "<" + DBEpisode.cOutName + "." + DBEpisode.cSeasonIndex + ">x<" + DBEpisode.cOutName + "." + DBEpisode.cEpisodeIndex + ">: <" + DBEpisode.cOutName + "." + DBEpisode.cEpisodeName + ">");

                if (GetOptions(cView_Episode_Subtitle) == null)
                    SetOptions(cView_Episode_Subtitle, "<" + DBSeries.cOutName + "." + DBOnlineSeries.cGenre + ">");

                if (GetOptions(cView_Episode_Main) == null)
                    SetOptions(cView_Episode_Main, "<" + DBEpisode.cOutName + "." + DBOnlineEpisode.cEpisodeSummary + ">");

                if (GetOptions(cTorrentSearch) == null)
                    SetOptions(cTorrentSearch, String.Empty);

                if (GetOptions(cUTorrentPath) == null)
                    SetOptions(cUTorrentPath, String.Empty);

                if (GetOptions(cNewsLeecherPath) == null)
                    SetOptions(cNewsLeecherPath, String.Empty);

                if (GetOptions(cRandomBanner) == null)
                    SetOptions(cRandomBanner, 0);

                if (GetOptions(cWatchedAfter) == null)
                    SetOptions(cWatchedAfter, 95); //-- 95% by default

                if (GetOptions(cDownloadMonitor_RenameFiles) == null)
                    SetOptions(cDownloadMonitor_RenameFiles, 0); //do not rename by default

                // this is the default main mirrors
                if (GetOptions(DBOption.cMainMirror) == null || GetOptions(DBOption.cMainMirror) == "http://thetvdb.com/interfaces")
                    DBOption.SetOptions(DBOption.cMainMirror, m_sMainMirror);

                if (GetOptions(cNewAPIUpgradeDone) == null)
                    SetOptions(cNewAPIUpgradeDone, 0);

                int oldLangOptionSet;
                if (GetOptions(cOnlineLanguage) == null || int.TryParse(GetOptions(cOnlineLanguage), out oldLangOptionSet))
                    SetOptions(cOnlineLanguage, "en"); // old api used index for onlinelang, new one two letters

                if (GetOptions(cAppendFirstLogoToList) == null)
                    SetOptions(cAppendFirstLogoToList, 0); //default no (most skins don't seem to use this)

                if (GetOptions(cGraphicalGroupView) == null)
                    SetOptions(cGraphicalGroupView, 0); //default yes (should work on all skins)

                if (GetOptions(cQualitySeriesBanners) == null)
                    SetOptions(cQualitySeriesBanners, 75);

                if (GetOptions(cQualitySeriesPosters) == null)
                    SetOptions(cQualitySeriesPosters, 50);

                if (GetOptions(cQualitySeasonBanners) == null)
                    SetOptions(cQualitySeasonBanners, 75);

                if (GetOptions(cQualityEpisodeImages) == null)
                    SetOptions(cQualityEpisodeImages, 100);

                if (GetOptions(cShowSeriesFanart) == null)
                    SetOptions(cShowSeriesFanart, true);

                if (GetOptions(cFanartRandom) == null)
                    SetOptions(cFanartRandom, true);

                if (GetOptions(cOnlineFavourites) == null)
                    SetOptions(cOnlineFavourites, false);

                string playListFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                playListFolder += @"\My Playlists";
                if (GetOptions(cPlaylistPath) == null)
                    SetOptions(cPlaylistPath, playListFolder);

                if (GetOptions(cRepeatPlaylist) == null)
                    SetOptions(cRepeatPlaylist, false);

                if (GetOptions(cPlaylistAutoPlay) == null)
                    SetOptions(cPlaylistAutoPlay, true);

				if (GetOptions(cPlaylistAutoShuffle) == null)
					SetOptions(cPlaylistAutoShuffle, false);

                if (GetOptions(cImport_ScanOnStartup) == null)
                    SetOptions(cImport_ScanOnStartup, true);

                if (GetOptions(cAutoDownloadMissingArtwork) == null)
                    SetOptions(cAutoDownloadMissingArtwork, true);

                if (GetOptions(cAutoUpdateEpisodeRatings) == null)
                    SetOptions(cAutoUpdateEpisodeRatings, true);

                if (GetOptions(cAutoUpdateAllFanart) == null)
                    SetOptions(cAutoUpdateAllFanart, true);

                if (GetOptions(cAutoDownloadFanart) == null)
                    SetOptions(cAutoDownloadFanart, true);

                if (GetOptions(cAutoDownloadFanartCount) == null)
                    SetOptions(cAutoDownloadFanartCount, 3);

                if (GetOptions(cAutoDownloadFanartResolution) == null)
                    SetOptions(cAutoDownloadFanartResolution, 0); //0=Both,1=1280x720,2=1920x1080

                if (GetOptions(cFanartThumbnailResolutionFilter) == null)
                    SetOptions(cFanartThumbnailResolutionFilter, 0);

                if (GetOptions(cFanartCurrentView) == null)
                    SetOptions(cFanartCurrentView, 2); // Large Icons

                if (GetOptions(cUseRegionalDateFormatString) == null)
                    SetOptions(cUseRegionalDateFormatString, 0);

				if (GetOptions(cDefaultRating) == null)
					SetOptions(cDefaultRating, 7); // Scale 1 - 10

				if (GetOptions(cRatingDisplayStars) == null)
					SetOptions(cRatingDisplayStars, 10); // 5 or 10 Stars

                if (GetOptions(cSortSpecials) == null)
                    SetOptions(cSortSpecials, 1);

                if (GetOptions(cBackdropLoadingDelay) == null)
                    SetOptions(cBackdropLoadingDelay, 250); //milliseconds

                if (GetOptions(cArtworkLoadingDelay) == null)
                    SetOptions(cArtworkLoadingDelay, 250); //milliseconds

                if (GetOptions(cRandomFanartInterval) == null)
                    SetOptions(cRandomFanartInterval, 30000); //milliseconds

                if (GetOptions(cAutoDownloadFanartSeriesNames) == null)
                    SetOptions(cAutoDownloadFanartSeriesNames, 0);

                if (GetOptions(cParentalControlPinCode) == null)
                    SetOptions(cParentalControlPinCode, string.Empty);

				if (GetOptions(cKeyboardStyle) == null)
					SetOptions(cKeyboardStyle, 0); // NORMAL KEYBOARD

				if (GetOptions(cMarkRatedEpisodeAsWatched) == null)
					SetOptions(cMarkRatedEpisodeAsWatched, 0);

                if (GetOptions(cSubstituteMissingArtwork) == null)
                    SetOptions(cSubstituteMissingArtwork, 0);

                if (GetOptions(cAskToRate)==null)
                    SetOptions(cAskToRate, 1);

                if (GetOptions(cSkipSeasonViewOnSingleSeason) == null)
                    SetOptions(cSkipSeasonViewOnSingleSeason, 1);

                if (GetOptions(cImport_ScanWhileFullscreenVideo) == null)
                    SetOptions(cImport_ScanWhileFullscreenVideo, 0);

                if (GetOptions(cInvokeExtBeforePlayback) == null)
                    SetOptions(cInvokeExtBeforePlayback, string.Empty);

                if (GetOptions(cInvokeExtBeforePlaybackArgs) == null)
                    SetOptions(cInvokeExtBeforePlaybackArgs, "\"<Episode.EpisodeFilename>\"");

                if (GetOptions(cInvokeExtBeforePlaybackWaitForExit) == null)
                    SetOptions(cInvokeExtBeforePlaybackWaitForExit, 0);

                if (GetOptions(cInvokeExtAfterPlayback) == null)
                    SetOptions(cInvokeExtAfterPlayback, string.Empty);

                if (GetOptions(cInvokeExtAfterPlaybackArgs) == null)
                    SetOptions(cInvokeExtAfterPlaybackArgs, "\"<Episode.EpisodeFilename>\"");

                if (GetOptions(cInvokeExtAfterPlaybackWaitForExit) == null)
                    SetOptions(cInvokeExtAfterPlaybackWaitForExit, 0);

                if (GetOptions(cCountEmptyAndFutureAiredEps) == null)
                    SetOptions(cCountEmptyAndFutureAiredEps, 1);

                if (GetOptions(cOnPlaySeriesOrSeasonAction) == null)
                    SetOptions(cOnPlaySeriesOrSeasonAction, 2); // set first unwatched as default

                if (GetOptions(cLogLevel) == null)
                    SetOptions(cLogLevel, 0);

                if (GetOptions(cNewEpisodeThumbType) == null)
                    SetOptions(cNewEpisodeThumbType, 2); // Recently Added Episodes

                if (GetOptions(cSubCentralEnabled) == null)
                    SetOptions(cSubCentralEnabled, true);

                if (GetOptions(cSubCentralEnabledForEpisodes) == null)
                    SetOptions(cSubCentralEnabledForEpisodes, true);

                if (GetOptions(cUseFullNameInSubDialog) == null)
                    SetOptions(cUseFullNameInSubDialog, false);

                if (GetOptions(cPlaylistUnwatchedOnly) == null)
                    SetOptions(cPlaylistUnwatchedOnly, false);

                if (GetOptions(cDisableMediaInfo) == null)
                    SetOptions(cDisableMediaInfo, false);

                if (GetOptions(cMediaInfoParseSpeed) == null)
                    SetOptions(cMediaInfoParseSpeed, "0.3"); // Default is 0.5 but we dont need that for TVSeries.

                if (GetOptions(cImportDelay) == null)
                    SetOptions(cImportDelay, 30);

            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("DBOption.UpdateTable failed (" + ex.Message + ").");
            }
        }

        private static void UpdateTable()
        {
            try
            {
                if (!bTableUpdateDone)
                {
                    bTableUpdateDone = true;
                    SQLiteResultSet results = DBTVSeries.Execute("SELECT name FROM sqlite_master WHERE name='options' and type='table' UNION ALL SELECT name FROM sqlite_temp_master WHERE type='table' ORDER BY name");
                    if (results == null || results.Rows.Count == 0)                    
                    {
                        // no table, create it
                        DBTVSeries.Execute(cCreateTableQuery);
                    }
                }
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("DBOption.UpdateTable failed (" + ex.Message + ").");
            }
        }

        public static bool SetOptions(String property, DBValue value)
        {
            try
            {
				// UpdateTable();
                if (!optionsCache.ContainsKey(property) || optionsCache[property] != value)
                {
                    String convertedProperty = property;
                    String convertedvalue = value.ToString().Replace("'", "''");

                    String sqlQuery;
                    if (GetOptions(convertedProperty) == null)
                        sqlQuery = "insert into options (option_id, property, value) values(NULL, '" + convertedProperty + "', '" + convertedvalue + "')";
                    else
                        sqlQuery = "update options set value = '" + convertedvalue + "' where property = '" + convertedProperty + "'";
                    optionsCache[property] = value;
                    DBTVSeries.Execute(sqlQuery);
                } 
                return true;
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("An Error Occurred (" + ex.Message + ").");
                return false;
            }
        }

        public static DBValue GetOptions(String property)
        {
            try
            {
                // UpdateTable();
                String convertedProperty = property;
                DatabaseUtility.RemoveInvalidChars(ref convertedProperty);
                if (optionsCache.ContainsKey(convertedProperty)) return optionsCache[convertedProperty];

                string sqlQuery = "select value from options where property = '" + convertedProperty + "'";
                SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
                if (results.Rows.Count > 0)
                {
                    string res = DatabaseUtility.Get(results, 0, "value");
                    if(!optionsCache.ContainsKey(property))
                        optionsCache.Add(property, res);
                    return res;
                }
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("An Error Occurred (" + ex.Message + ").");
            }
            return null;
        }

        public static void LogOptions() {
            foreach (string key in optionsCache.Keys) {
                MPTVSeriesLog.Write(string.Format("Option {0}: {1}", key, optionsCache[key].ToString()), MPTVSeriesLog.LogLevel.Debug);
            }
        }

    };
}
