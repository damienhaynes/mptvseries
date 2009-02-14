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

        public const String cShowHiddenItems = "ShowHiddenItems";
        public const String cOnlineParseEnabled = "OnlineParseEnabled";
        public const String cFullSeriesRetrieval = "FullSeriesRetrieval";
        public const String cAutoChooseSeries = "AutoChooseSeries";
        public const String cAutoChooseOrder = "AutoChooseOrder";        
        public const String cAutoScanLocalFiles = "AutoScanLocalFiles";
        public const String cAutoScanLocalFilesLapse = "AutoScanLocalFilesLapse";
        public const String cAutoUpdateOnlineData = "AutoUpdateOnlineData";
        public const String cAutoUpdateOnlineDataLapse = "AutoUpdateOnlineDataLapse";
        public const String cDontClearMissingLocalFiles = "DontClearMissingLocalFiles";        

        public const String cUpdateSeriesTimeStamp = "UpdateSeriesTimeStamp"; // not used anymore
        public const String cUpdateEpisodesTimeStamp = "UpdateEpisodesTimeStamp"; // not used anymore

        public const String cLocalScanLastTime = "LocalScanLastTime";
        public const String cUpdateScanLastTime = "UpdateScanLastTime";

        public const String cView_PluginName = "View_PluginName";
        public const String cView_Episode_OnlyShowLocalFiles = "View_Episode_OnlyShowLocalFiles";
        public const String cView_Episode_HideUnwatchedSummary = "View_Episode_HideUnwatchedSummary";
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

        public const String cSubs_Forom_Enable = "Subs_Forom_Enable";
        public const String cSubs_Forom_BaseURL = "Subs_Forom_BaseURL";
        public const String cSubs_Forom_ID = "Subs_Forom_ID";

	    public const String cSubs_SeriesSubs_Enable = "Subs_Series_Enable";
	    public const String cSubs_SeriesSubs_BaseURL = "Subs_SeriesSubs_BaseURL";

        public const String cSubs_Remository_Enable = "Subs_Remository_Enable";
        public const String cSubs_Remository_BaseURL = "Subs_Remository_BaseURL";
        public const String cSubs_Remository_MainIdx = "Subs_Remository_MainIdx";
        public const String cSubs_Remository_UserName = "Subs_Remository_UserName";
        public const String cSubs_Remository_Password = "Subs_Remository_Password";
        public const String cSubs_Remository_RegexSeriesSeasons = "Subs_Remository_RegEx_Series_Seasons";
        public const String cSubs_Remository_RegexEpisode = "Subs_Remository_RegEx_Episode";
        public const String cSubs_Remository_RegexDownload = "Subs_Remository_Download";

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

        public const String cDeleteFile = "deleteFile";

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

        private static Dictionary<string, DBValue> optionsCache = new Dictionary<string, DBValue>();

        static DBOption()
        {
            try
            {
                SQLiteResultSet results;
                results = DBTVSeries.Execute("SELECT name FROM sqlite_master WHERE name='options' and type='table' UNION ALL SELECT name FROM sqlite_temp_master WHERE type='table' ORDER BY name");
                if (results == null || results.Rows.Count == 0)
                {
                    // no table, create it
                    String sQuery = "CREATE TABLE options (option_id integer primary key, property text, value text);\n";
                    DBTVSeries.Execute(sQuery);
                }

                if (GetOptions(cConfig_LogCollapsed) == null)
                    SetOptions(cConfig_LogCollapsed, true);

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
                    SetOptions(cAutoChooseOrder, false);

                if (GetOptions(cView_Episode_OnlyShowLocalFiles) == null)
                    SetOptions(cView_Episode_OnlyShowLocalFiles, true);

                if (GetOptions(cView_Episode_HideUnwatchedSummary) == null)
                    SetOptions(cView_Episode_HideUnwatchedSummary, false);// Unless we can also hide episode thumb as well there is no point making this default
                                                                          // I think 90% of users would prefer to see overview always, you need to read it to be spoiled
                if (GetOptions(cUpdateSeriesTimeStamp) == null)
                    SetOptions(cUpdateSeriesTimeStamp, 0);

                if (GetOptions(cUpdateEpisodesTimeStamp) == null)
                    SetOptions(cUpdateEpisodesTimeStamp, 0);

                if (GetOptions(cAutoScanLocalFiles) == null)
                    SetOptions(cAutoScanLocalFiles, true);

                if (GetOptions(cAutoScanLocalFilesLapse) == null)
                    SetOptions(cAutoScanLocalFilesLapse, 5);

                if (GetOptions(cAutoUpdateOnlineData) == null)
                    SetOptions(cAutoUpdateOnlineData, true);

                if (GetOptions(cAutoUpdateOnlineDataLapse) == null)
                    SetOptions(cAutoUpdateOnlineDataLapse, 12);

                if (GetOptions(cLocalScanLastTime) == null)
                    SetOptions(cLocalScanLastTime, 0);

                if (GetOptions(cUpdateScanLastTime) == null)
                    SetOptions(cUpdateScanLastTime, 0);

                if (GetOptions(cDontClearMissingLocalFiles) == null)
                    SetOptions(cDontClearMissingLocalFiles, 0);

                if (GetOptions(cView_PluginName) == null)
                    SetOptions(cView_PluginName, "My TV Series");

                if (GetOptions(cViewAutoHeight) == null)
                    SetOptions(cViewAutoHeight, true);

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

		        if (GetOptions(cSubs_Forom_Enable) == null)
		            SetOptions(cSubs_Forom_Enable, false);

		        if (GetOptions(cSubs_SeriesSubs_Enable) == null)
		            SetOptions(cSubs_SeriesSubs_Enable, false);

		        if (GetOptions(cSubs_Remository_Enable) == null)
		            SetOptions(cSubs_Remository_Enable, false);

                if (GetOptions(cSubs_Forom_BaseURL) == null)
                    SetOptions(cSubs_Forom_BaseURL, @"http://www.foroms.net/documents");

		        if (GetOptions(cSubs_SeriesSubs_BaseURL) == null)
		            SetOptions(cSubs_SeriesSubs_BaseURL, @"http://www.seriessub.com/sous-titres/");

                if (GetOptions(cSubs_Remository_BaseURL) == null)
                    SetOptions(cSubs_Remository_BaseURL, @"http://www.italiansubs.net/");
                
                if (GetOptions(cSubs_Remository_MainIdx) == null)
                    SetOptions(cSubs_Remository_MainIdx, "27");

                if (GetOptions(cTorrentSearch) == null)
                    SetOptions(cTorrentSearch, String.Empty);

                if (GetOptions(cUTorrentPath) == null)
                    SetOptions(cUTorrentPath, String.Empty);

                if (GetOptions(cNewsLeecherPath) == null)
                    SetOptions(cNewsLeecherPath, String.Empty);

                if (GetOptions(cRandomBanner) == null)
                    SetOptions(cRandomBanner, 0);
                if (GetOptions("doFolderWatch") == null)
                    SetOptions("doFolderWatch", true); 

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
                    SQLiteResultSet results;
                    results = DBTVSeries.Execute("SELECT name FROM sqlite_master WHERE name='options' and type='table' UNION ALL SELECT name FROM sqlite_temp_master WHERE type='table' ORDER BY name");
                    if (results != null && results.Rows.Count > 0)
                    {
                        // table is already there, perfect
                    }
                    else
                    {
                        // no table, create it
                        String sQuery = "CREATE TABLE options (option_id integer primary key, property text, value text);\n";
                        DBTVSeries.Execute(sQuery);
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
//                UpdateTable();
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
//                UpdateTable();
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
    };
}
