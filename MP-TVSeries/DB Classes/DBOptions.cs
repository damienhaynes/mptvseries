#region GNU license
// MP-TVSeries - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2013
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

using MediaPortal.Database;
using SQLite.NET;
using System;
using System.Collections.Generic;

namespace WindowPlugins.GUITVSeries
{
    enum NewEpisodeIndicatorType
    {
        none,
        unwatched,
        recentlyadded,
        recentlyaddedunwatched
    }

    public class DBOption
    {
        #region Database Fields
        public const string cArtworkChooserLayout = "ArtworkChooserLayout";
        public const string cConfigLogCollapsed = "Config_LogShown";
        public const string cDBSeriesVersion = "DBSeriesVersion";
        public const string cDBOnlineSeriesVersion = "DBOnlineSeriesVersion";
        public const string cDBSeriesLastLocalID = "DBSeriesLasLocalID";
        public const string cDBSeasonVersion = "DBSeasonVersion";
        public const string cDBEpisodesVersion = "DBEpisodesVersion";
        public const string cDBExpressionsVersion = "DBExpressionsVersion";        
        public const string cDBViewsVersion = "DBViewsVersion";
        public const string cDBReplacementsVersion = "DBReplacementsVersion";
        public const string cShowHiddenItems = "ShowHiddenItems";
        public const string cOnlineParseEnabled = "OnlineParseEnabled";
        public const string cFullSeriesRetrieval = "FullSeriesRetrieval";
        public const string cAutoChooseSeries = "AutoChooseSeries";
        public const string cAutoChooseOrder = "AutoChooseOrder";
        public const string cImportFolderWatch = "doFolderWatch";
        public const string cImportScanRemoteShare = "scanRemoteShare";
        public const string cImportScanOnStartup = "ScanOnStartup";
        public const string cImportScanWhileFullscreenVideo = "AutoScanLocalFilesFSV";
        public const string cImportScanRemoteShareLapse = "AutoScanLocalFilesLapse";
        public const string cImportAutoUpdateOnlineData = "AutoUpdateOnlineData";
        public const string cImportAutoUpdateOnlineDataLapse = "AutoUpdateOnlineDataLapse";
        public const string cImportDontClearMissingLocalFiles = "DontClearMissingLocalFiles";        
        public const string cImportOnlineUpdateScanLastTime = "UpdateScanLastTime";
        public const string cPluginName = "View_PluginName";
        public const string cOnlyShowLocalFiles = "View_Episode_OnlyShowLocalFiles";
        public const string cHideUnwatchedSummary = "View_Episode_HideUnwatchedSummary";
        public const string cHideUnwatchedThumbnail = "View_Episode_HideUnwatchedThumbnail";
        public const string cUseSortName = "Series_UseSortName";
        public const string cViewAutoHeight = "ViewAutoHeight";
        public const string cViewSeriesListFormat = "View_Series_ListFormat";
        public const string cViewSeriesColOne = "View_Series_Col1";
        public const string cViewSeriesColTwo = "View_Series_Col2";
        public const string cViewSeriesColThree = "View_Series_Col3";
        public const string cViewSeriesTitle = "View_Series_Title";
        public const string cViewSeriesSecondTitle = "View_Series_Secondary";
        public const string cViewSeriesMain = "View_Series_Main";
        public const string cViewSeasonListFormat = "View_Season_ListFormat";
        public const string cViewSeasonColOne = "View_Season_Col1";
        public const string cViewSeasonColTwo = "View_Season_Col2";
        public const string cViewSeasonColThree = "View_Season_Col3";
        public const string cViewSeasonTitle = "View_Season_Title";
        public const string cViewSeasonSecondTitle = "View_Season_Secondary";
        public const string cViewSeasonMain = "View_Season_Main";
        public const string cViewEpisodeColOne = "View_Episode_Col1";
        public const string cViewEpisodeColTwo = "View_Episode_Col2";
        public const string cViewEpisodeColThree = "View_Episode_Col3";
        public const string cViewEpisodeTitle = "View_Episode_Title";
        public const string cViewEpisodeSecondTitle = "View_Episode_Secondary";
        public const string cViewEpisodeMain = "View_Episode_Main";
        public const string cRandomBanner = "randomBanner";
        public const string cOnlineLanguage = "onlineLanguage";
        public const string cShowDeleteMenu = "ShowDeleteMenu";
        public const string cMainMirrorHardCoded = "https://thetvdb.com";
        public const string cMainMirror = "mainMirror";
        public const string cGetBlankBanners = "getBlankBanners";
        public const string cGetTextBanners = "getTextBanners";
        public const string cGetEpisodeSnapshots = "getEpisodeSnapshots";
        public const string cCheckArtwork = "checkArtwork";
        public const string cWatchedAfter = "watchedAfter";
        public const string cAltImgLoading = "altImageLoading";
        public const string cNewAPIUpgradeDone = "newAPIUpgradeDone";
        public const string cUpdateTimeStamp = "UpdateTimeStamp"; // new one and only timestamp
        public const string cOnlineUserID = "onlineUserID";
        public const string cAskToRate = "askToRate";
        public const string cAppendFirstLogoToList = "appendFirstLogoToList";
        public const string cGraphicalGroupView = "graphicalGroupView";
        public const string cQualitySeriesBanners = "QualitySeriesBanners";
        public const string cQualitySeasonBanners = "QualitySeasonBanners";
        public const string cQualitySeasonCoverflow = "QualitySeasonCoverflow";
        public const string cQualityEpisodeImages = "QualityEpisodeImages";
        public const string cQualitySeriesPosters = "QualityPosterImages";
        public const string cQualitySeriesCoverflow = "QualitySeriesCoverflow";
        public const string cGetSeriesPosters = "getSeriesPosters";
        public const string cFanartRandom = "FanartRandom";
        public const string cOnlineFavourites = "UseOnlineFavourites";
        public const string cPlaylistPath = "PlayListPath";
        public const string cRepeatPlaylist = "RepeatPlaylist";
        public const string cPlaylistAutoPlay = "PlaylistAutoPlay";
		public const string cPlaylistAutoShuffle = "PlaylistAutoShuffle";
        public const string cPlaylistUnwatchedOnly = "PlaylistUnwatchedOnly";
        public const string cAutoDownloadMissingArtwork = "AutoDownloadMissingArtwork";
        public const string cAutoUpdateEpisodeRatings = "AutoUpdateEpisodeRatings";
        public const string cAutoUpdateAllFanart = "AutoUpdateAllFanart";
        public const string cAutoDownloadFanart = "AutoDownloadFanart";
        public const string cAutoDownloadFanartCount = "AutoDownloadFanartCount";
        public const string cAutoDownloadFanartResolution = "AutoDownloadFanartResolution";
        public const string cAutoDownloadFanartSeriesNames = "AutoDownloadFanartSeriesNames";
        public const string cAutoDownloadActors = "AutoDownloadActors";
        public const string cFanartThumbnailResolutionFilter = "FanartThumbnailResolutionFilter";
        public const string cFanartCurrentView = "FanartCurrentView";
        public const string cUseRegionalDateFormatString = "UseRegionalDateFormatString";
		public const string cDefaultRating = "DefaultRating";
		public const string cRatingDisplayStars = "RatingDisplayStars";
        public const string cSortSpecials = "SortSpecials";
        public const string cBackdropLoadingDelay = "BackdropLoadingDelay";
        public const string cArtworkLoadingDelay = "ArtworkLoadingDelay";
        public const string cRandomFanartInterval = "RandomFanartInterval";
        public const string cParentalControlPinCode = "ParentalControlPinCode";		
		public const string cMarkRatedEpisodeAsWatched = "MarkRatedEpisodeAsWatched";
        public const string cSubstituteMissingArtwork = "SubstituteMissingArtwork";
        public const string cSkipSeasonViewOnSingleSeason = "SkipSeasonViewOnSingleSeason";
        public const string cInvokeExtBeforePlayback = "InvokeExtBeforePlayback";
        public const string cInvokeExtBeforePlaybackArgs = "InvokeExtBeforePlaybackArgs";
        public const string cInvokeExtBeforePlaybackWaitForExit = "InvokeExtBeforePlaybackWaitForExit";
        public const string cInvokeExtAfterPlayback = "InvokeExtAfterPlayback";
        public const string cInvokeExtAfterPlaybackArgs = "InvokeExtAfterPlaybackArgs";
        public const string cInvokeExtAfterPlaybackWaitForExit = "InvokeExtAfterPlaybackWaitForExit";
        public const string cCountEmptyAndFutureAiredEps = "CountEmptyAndFutureAiredEps";
        public const string cOnPlaySeriesOrSeasonAction = "OnPlaySeriesOrSeasonAction";
        public const string cNewEpisodeThumbType = "NewEpisodeThumbType";
        public const string cNewEpisodeRecentDays = "NewEpisodeRecentDays";
        public const string cSubCentralEnabled = "SubCentral_Enabled";
        public const string cSubCentralEnabledForEpisodes = "SubCentral_EnabledForEpisodes";
        public const string cSubCentralSubtitleDownloadOnPlay = "SubCentral_SubtitleDownloadOnPlay";
        public const string cConfigSizeHeight = "configSizeHeight";
        public const string cConfigSizeWidth = "configSizeWidth";
        public const string cDisableMediaInfo = "DisableMediaInfo";
        public const string cDisableMediaInfoInConfigImports = "DisableMediaInfoInConfigImports";
        public const string cMediaInfoParseSpeed = "MediaInfoParseSpeed";
        public const string cImportDelay = "ImportDelay";        
        public const string cDelayImportPathMonitoringValue = "DelayImportPathMonitoringValue";
        public const string cSetHiddenSeriesAsScanIgnore = "SetHiddenSeriesAsScanIgnore";
        public const string cArtworkLimitSeriesWideBanners = "ArtworkLimitSeriesWideBanners";
        public const string cArtworkLimitSeriesPosters = "ArtworkLimitSeriesPosters";
        public const string cArtworkLimitSeasonPosters = "ArtworkLimitSeasonPosters";
        public const string cMaxConsecutiveDownloadErrors = "MaxConsecutiveDownloadErrors";
        public const string cSortSpecialSeasonLast = "SortSpecialSeasonLast";
        public const string cActorLayout = "ActorLayout";
        public const string cAutoGenerateEpisodeTitles = "AutoGenerateEpisodeTitles";
        public const string cParentalControlResetInterval = "ParentalControlResetInterval";
        public const string cParentalControlDisableAfter = "ParentalControlDisableAfter";
        public const string cParentalControlDisableBefore = "ParentalControlDisableBefore";
        public const string cSQLLoggingEnabled = "SQLLoggingEnabled";
        public const string cCheckPlayOutOfOrder = "CheckPlayOutOfOrder";
        public const string cFilterUnwatched = "FilterUnwatched";
        public const string cCleanOnlineEpisodes = "CleanOnlineEpisodes";
        public const string cCleanOnlineEpisodeZero = "CleanOnlineEpisodeZero";
        public const string cOverrideLanguage = "OverrideLanguage";
        public const string cCountSpecialEpisodesAsWatched = "CountSpecialEpisodesAsWatched";
        public const string cTraktCommunityRatings = "TraktCommunityRatings";
        public const string cTraktLastDateUpdated = "TraktLastDateUpdated";
        public const string cParsedNameFromFolder = "ParsedNameFromFolder";
        public const string cCheckShowOnlyEpisodesRequiringManualSelection = "CheckShowOnlyEpisodesRequiringManualSelection";
        public const string cTmdbConfiguration = "TmdbConfiguration";
        public const string cFanartTvClientKey = "FanartTvClientKey";
        public const string cOverrideSearchLanguageToEnglish = "OverrideSearchLanguageToEnglish";
        #endregion

        private static readonly Object thisLock = new Object();
        private static Dictionary<string, DBValue> optionsCache = new Dictionary<string, DBValue>();

        static DBOption()
        {
            CreateTable();

            #region Set Default Options
            if (GetOptions(cConfigLogCollapsed) == null)
                SetOptions(cConfigLogCollapsed, false);

            if (GetOptions(cDBSeriesLastLocalID) == null)
                SetOptions(cDBSeriesLastLocalID, -1);

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

            if (GetOptions(cOnlyShowLocalFiles) == null)
                SetOptions(cOnlyShowLocalFiles, true);

            if (GetOptions(cHideUnwatchedSummary) == null)
                SetOptions(cHideUnwatchedSummary, false);

            if (GetOptions(cHideUnwatchedThumbnail) == null)
                SetOptions(cHideUnwatchedThumbnail, false);
                
            if (GetOptions(cImportFolderWatch) == null)
                SetOptions(cImportFolderWatch, true);

            if (GetOptions(cImportScanRemoteShare) == null)
                SetOptions(cImportScanRemoteShare, true);

            if (GetOptions(cImportScanRemoteShareLapse) == null)
                SetOptions(cImportScanRemoteShareLapse, 5);

            if (GetOptions(cImportAutoUpdateOnlineData) == null)
                SetOptions(cImportAutoUpdateOnlineData, true);

            if (GetOptions(cImportAutoUpdateOnlineDataLapse) == null)
                SetOptions(cImportAutoUpdateOnlineDataLapse, 12);

            if (GetOptions(cImportOnlineUpdateScanLastTime) == null)
                SetOptions(cImportOnlineUpdateScanLastTime, 0);

            if (GetOptions(cImportDontClearMissingLocalFiles) == null)
                SetOptions(cImportDontClearMissingLocalFiles, 0);

            if (GetOptions(cCheckArtwork) == null) 
                SetOptions(cCheckArtwork, 0);
                
            if (GetOptions(cPluginName) == null)
                SetOptions(cPluginName, "My TV Series");

            if (GetOptions(cViewAutoHeight) == null)
                SetOptions(cViewAutoHeight, true);
                
            if (GetOptions(cViewSeriesListFormat) == null)
                SetOptions(cViewSeriesListFormat, "ListPosters");

            if (GetOptions(cViewSeasonListFormat) == null)
                SetOptions(cViewSeasonListFormat, 0);

            if (GetOptions(cViewSeriesColOne) == null)
                SetOptions(cViewSeriesColOne, "");

            if (GetOptions(cViewSeriesColTwo) == null)
                SetOptions(cViewSeriesColTwo, "<" + DBSeries.cOutName + "." + DBOnlineSeries.cPrettyName + ">");

            if (GetOptions(cViewSeriesColThree) == null)
                SetOptions(cViewSeriesColThree, "<" + DBSeries.cOutName + "." + DBOnlineSeries.cAirsDay + ">");

            if (GetOptions(cViewSeriesTitle) == null)
                SetOptions(cViewSeriesTitle, "<" + DBSeries.cOutName + "." + DBOnlineSeries.cPrettyName + ">");

            if (GetOptions(cViewSeriesSecondTitle) == null)
                SetOptions(cViewSeriesSecondTitle, "<" + DBSeries.cOutName + "." + DBOnlineSeries.cGenre + ">");

            if (GetOptions(cViewSeriesMain) == null)
                SetOptions(cViewSeriesMain, "<" + DBSeries.cOutName + "." + DBOnlineSeries.cSummary + ">");

            if (GetOptions(cUseSortName) == null)
                SetOptions(cUseSortName, 0); // default sort is by pretty name

            if (GetOptions(cViewSeasonColOne) == null)
                SetOptions(cViewSeasonColOne, "");

            if (GetOptions(cViewSeasonColTwo) == null)
                SetOptions(cViewSeasonColTwo, "Season <" + DBSeason.cOutName + "." + DBSeason.cIndex + ">");

            if (GetOptions(cViewSeasonColThree) == null)
                SetOptions(cViewSeasonColThree, "");

            if (GetOptions(cViewSeasonTitle) == null)
                SetOptions(cViewSeasonTitle, "<" + DBSeries.cOutName + "." + DBOnlineSeries.cPrettyName + "> Season <" + DBSeason.cOutName + "." + DBSeason.cIndex + ">");

            if (GetOptions(cViewSeasonSecondTitle) == null)
                SetOptions(cViewSeasonSecondTitle, "<" + DBSeries.cOutName + "." + DBOnlineSeries.cGenre + ">");

            if (GetOptions(cViewSeasonMain) == null)
                SetOptions(cViewSeasonMain, "<" + DBSeries.cOutName + "." + DBOnlineSeries.cSummary + ">"); 
                
            if (GetOptions(cViewEpisodeColOne) == null)
                SetOptions(cViewEpisodeColOne, "");

            if (GetOptions(cViewEpisodeColTwo) == null)
                SetOptions(cViewEpisodeColTwo, "<" + DBEpisode.cOutName + "." + DBEpisode.cEpisodeIndex + ">: <" + DBEpisode.cOutName + "." + DBEpisode.cEpisodeName + ">");

            if (GetOptions(cViewEpisodeColThree) == null)
                SetOptions(cViewEpisodeColThree, "<" + DBEpisode.cOutName + "." + DBOnlineEpisode.cFirstAired + ">");

            if (GetOptions(cViewEpisodeTitle) == null)
                SetOptions(cViewEpisodeTitle, "<" + DBEpisode.cOutName + "." + DBEpisode.cSeasonIndex + ">x<" + DBEpisode.cOutName + "." + DBEpisode.cEpisodeIndex + ">: <" + DBEpisode.cOutName + "." + DBEpisode.cEpisodeName + ">");

            if (GetOptions(cViewEpisodeSecondTitle) == null)
                SetOptions(cViewEpisodeSecondTitle, "<" + DBSeries.cOutName + "." + DBOnlineSeries.cGenre + ">");

            if (GetOptions(cViewEpisodeMain) == null)
                SetOptions(cViewEpisodeMain, "<" + DBEpisode.cOutName + "." + DBOnlineEpisode.cEpisodeSummary + ">");

            if (GetOptions(cRandomBanner) == null)
                SetOptions(cRandomBanner, 0);

            if (GetOptions(cWatchedAfter) == null)
                SetOptions(cWatchedAfter, 95);

            // this is the default main mirrors
            if (GetOptions(DBOption.cMainMirror) == null || GetOptions(DBOption.cMainMirror) == "http://thetvdb.com/interfaces")
                DBOption.SetOptions(DBOption.cMainMirror, cMainMirrorHardCoded);

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

            if (GetOptions(cQualitySeriesCoverflow) == null)
                SetOptions(cQualitySeriesCoverflow, 50);

            if (GetOptions(cQualitySeasonBanners) == null)
                SetOptions(cQualitySeasonBanners, 75);

            if (GetOptions(cQualitySeasonCoverflow) == null)
                SetOptions(cQualitySeasonCoverflow, 90);

            if (GetOptions(cQualityEpisodeImages) == null)
                SetOptions(cQualityEpisodeImages, 100);

            if (GetOptions(cFanartRandom) == null)
                SetOptions(cFanartRandom, true);

            if (GetOptions(cOnlineFavourites) == null)
                SetOptions(cOnlineFavourites, false);

            if (GetOptions(cPlaylistPath) == null)
            {
                string playListFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\My Playlists";
                SetOptions(cPlaylistPath, playListFolder);
            }

            if (GetOptions(cRepeatPlaylist) == null)
                SetOptions(cRepeatPlaylist, false);

            if (GetOptions(cPlaylistAutoPlay) == null)
                SetOptions(cPlaylistAutoPlay, true);

			if (GetOptions(cPlaylistAutoShuffle) == null)
				SetOptions(cPlaylistAutoShuffle, false);

            if (GetOptions(cImportScanOnStartup) == null)
                SetOptions(cImportScanOnStartup, true);

            if (GetOptions(cAutoDownloadMissingArtwork) == null)
                SetOptions(cAutoDownloadMissingArtwork, true);

            if (GetOptions(cAutoUpdateEpisodeRatings) == null)
                SetOptions(cAutoUpdateEpisodeRatings, false);

            if (GetOptions(cAutoUpdateAllFanart) == null)
                SetOptions(cAutoUpdateAllFanart, false);

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
                SetOptions(cSortSpecials, 0);

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

			if (GetOptions(cMarkRatedEpisodeAsWatched) == null)
				SetOptions(cMarkRatedEpisodeAsWatched, 0);

            if (GetOptions(cSubstituteMissingArtwork) == null)
                SetOptions(cSubstituteMissingArtwork, 1);

            if (GetOptions(cAskToRate)==null)
                SetOptions(cAskToRate, 0);

            if (GetOptions(cSkipSeasonViewOnSingleSeason) == null)
                SetOptions(cSkipSeasonViewOnSingleSeason, 1);

            if (GetOptions(cImportScanWhileFullscreenVideo) == null)
                SetOptions(cImportScanWhileFullscreenVideo, 0);

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
                
            if (GetOptions(cNewEpisodeThumbType) == null)
                SetOptions(cNewEpisodeThumbType, (int)NewEpisodeIndicatorType.recentlyadded); // Recently Added Episodes

            if (GetOptions(cNewEpisodeRecentDays) == null)
                SetOptions(cNewEpisodeRecentDays, 7);

            if (GetOptions(cSubCentralEnabled) == null)
                SetOptions(cSubCentralEnabled, true);

            if (GetOptions(cSubCentralEnabledForEpisodes) == null)
                SetOptions(cSubCentralEnabledForEpisodes, true);

            if (GetOptions(cSubCentralSubtitleDownloadOnPlay) == null)
                SetOptions(cSubCentralSubtitleDownloadOnPlay, false);

            if (GetOptions(cPlaylistUnwatchedOnly) == null)
                SetOptions(cPlaylistUnwatchedOnly, false);

            if (GetOptions(cDisableMediaInfo) == null)
                SetOptions(cDisableMediaInfo, false);

            if (GetOptions(cMediaInfoParseSpeed) == null)
                SetOptions(cMediaInfoParseSpeed, "0.1"); // Default is 0.5 (scan 50% of file) but we dont need that for TVSeries.

            if (GetOptions(cImportDelay) == null)
                SetOptions(cImportDelay, 30);

            if (GetOptions(cDelayImportPathMonitoringValue) == null)
                SetOptions(cDelayImportPathMonitoringValue, 20);

            if (GetOptions(cSetHiddenSeriesAsScanIgnore) == null)
                SetOptions(cSetHiddenSeriesAsScanIgnore, true);

            if (GetOptions(cGetBlankBanners) == null)
                SetOptions(cGetBlankBanners, false);

            if (GetOptions(cGetTextBanners) == null)
                SetOptions(cGetTextBanners, false);

            if (GetOptions(cArtworkLimitSeriesWideBanners) == null)
                SetOptions(cArtworkLimitSeriesWideBanners, 1); // fanart.tv is slow

            if (GetOptions(cArtworkLimitSeriesPosters) == null)
                SetOptions(cArtworkLimitSeriesPosters, 3);

            if (GetOptions(cArtworkLimitSeasonPosters) == null)
                SetOptions(cArtworkLimitSeasonPosters, 2); // 20 seasons = 40 posters

            if (GetOptions(cMaxConsecutiveDownloadErrors) == null)
                SetOptions(cMaxConsecutiveDownloadErrors, 3);

            if (GetOptions(cSortSpecialSeasonLast) == null)
                SetOptions(cSortSpecialSeasonLast, true);

            if (GetOptions(cActorLayout) == null)
                SetOptions(cActorLayout, 0);

            if ( GetOptions( cArtworkChooserLayout  ) == null )
                SetOptions( cArtworkChooserLayout, 0 );

            if (GetOptions(cAutoGenerateEpisodeTitles) == null)
                SetOptions(cAutoGenerateEpisodeTitles, true);

            if (GetOptions(cParentalControlDisableAfter) == null)
                SetOptions(cParentalControlDisableAfter, new DateTime(2018, 02, 25, 21, 0, 0).ToShortTimeString());

            if (GetOptions(cParentalControlDisableBefore) == null)
                SetOptions(cParentalControlDisableBefore, new DateTime(2018, 02, 26, 3, 0, 0).ToShortTimeString());

            if (GetOptions(cParentalControlResetInterval) == null)
                SetOptions(cParentalControlResetInterval, 60);

            if (GetOptions(cShowDeleteMenu) == null)
                SetOptions(cShowDeleteMenu, true);

            if (GetOptions(cSQLLoggingEnabled) == null)
                SetOptions(cSQLLoggingEnabled, false);

            if (GetOptions(cCheckPlayOutOfOrder) == null)
                SetOptions(cCheckPlayOutOfOrder, true);

            if (GetOptions(cFilterUnwatched) == null)
                SetOptions(cFilterUnwatched, false);

            if (GetOptions(cAutoDownloadActors) == null)
                SetOptions(cAutoDownloadActors, false);

            if (GetOptions(cCleanOnlineEpisodes) == null)
                SetOptions(cCleanOnlineEpisodes, true);

            if (GetOptions(cCleanOnlineEpisodeZero) == null)
                SetOptions(cCleanOnlineEpisodeZero, false);

            if (GetOptions(cOverrideLanguage) == null)
                SetOptions(cOverrideLanguage, false);

            if (GetOptions(cCountSpecialEpisodesAsWatched) == null)
                SetOptions(cCountSpecialEpisodesAsWatched, false);

            if (GetOptions(cTraktCommunityRatings) == null)
                SetOptions(cTraktCommunityRatings, true);

            if (GetOptions(cParsedNameFromFolder) == null)
                SetOptions(cParsedNameFromFolder, false);

            if (GetOptions(cDisableMediaInfoInConfigImports) == null)
                SetOptions(cDisableMediaInfoInConfigImports, false );

            if (GetOptions(cCheckShowOnlyEpisodesRequiringManualSelection) == null)
                SetOptions(cCheckShowOnlyEpisodesRequiringManualSelection, false);

            #endregion
        }

        private static void CreateTable()
        {
            try
            {
                SQLiteResultSet results = DBTVSeries.Execute("SELECT name FROM sqlite_master WHERE name='options' and type='table' UNION ALL SELECT name FROM sqlite_temp_master WHERE type='table' ORDER BY name");
                if (results == null || results.Rows.Count == 0)
                {
                    // no table, create it
                    DBTVSeries.Execute("CREATE TABLE options (option_id integer primary key, property text, value text)");
                }
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("DBOption.CreateTable failed (" + ex.Message + ").");
            }
        }

        public static bool SetOptions(string property, DBValue value)
        {
            try
            {
                lock (thisLock)
                {
                    if (!optionsCache.ContainsKey(property) || optionsCache[property] != value)
                    {
                        // ensure our sql query will be using a valid string
                        string sqlQuery;
                        string convertedProperty = property;
                        string convertedValue = value.ToString().Replace("'", "''");

                        if (GetOptions(property) == null)
                            sqlQuery = "INSERT INTO options (option_id, property, value) VALUES(NULL, '" + convertedProperty + "', '" + convertedValue + "')";
                        else
                            sqlQuery = "UPDATE options SET value = '" + convertedValue + "' WHERE property = '" + convertedProperty + "'";

                        optionsCache[property] = value;
                        DBTVSeries.Execute(sqlQuery);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("An error occurred (" + ex.Message + ").");
                return false;
            }
        }

        public static DBValue GetOptions(string property)
        {
            try
            {
                lock (thisLock)
                {
                    DBValue retValue;
                    if (optionsCache.TryGetValue(property, out retValue))
                        return retValue;

                    // ensure our sql query will be using a valid string
                    string convertedProperty = property;
                    DatabaseUtility.RemoveInvalidChars(ref convertedProperty);

                    string sqlQuery = "SELECT value FROM options WHERE property = '" + convertedProperty + "'";
                    SQLiteResultSet sqlResults = DBTVSeries.Execute(sqlQuery);

                    if (sqlResults.Rows.Count > 0)
                    {
                        string dbValue = DatabaseUtility.Get(sqlResults, 0, "value");

                        if (!optionsCache.ContainsKey(property))
                            optionsCache.Add(property, dbValue);

                        return dbValue;
                    }
                }
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("An error occurred (" + ex.Message + ").");
            }
            return null;
        }

        internal static void LogOptions()
        {
            lock (thisLock)
            {
                foreach (string key in optionsCache.Keys)
                {
                    // dont log private options
                    if (!key.Equals(DBOption.cOnlineUserID))
                    {
                        MPTVSeriesLog.Write(string.Format("Option {0}: {1}", key, optionsCache[key].ToString()), MPTVSeriesLog.LogLevel.Debug);
                    }
                }
            }
        }
    };
}
