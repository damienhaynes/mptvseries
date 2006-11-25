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

        public const String cDBSeriesVersion = "DBSeriesVersion";
        public const String cDBSeriesLastLocalID = "DBSeriesLasLocalID";
        public const String cDBSeasonVersion = "DBSeasonVersion";
        public const String cDBEpisodesVersion = "DBEpisodesVersion";

        public const String cOnlineParseEnabled = "OnlineParseEnabled";
        public const String cFullSeriesRetrieval = "FullSeriesRetrieval";
        public const String cAutoChooseSeries = "AutoChooseSeries";
        public const String cLocalDataOverride = "LocalDataOverride";
        public const String cAutoScanLocalFiles = "AutoScanLocalFiles";
        public const String cAutoScanLocalFilesLapse = "AutoScanLocalFilesLapse";
        public const String cAutoUpdateOnlineData = "AutoUpdateOnlineData";
        public const String cAutoUpdateOnlineDataLapse = "AutoUpdateOnlineDataLapse";

        public const String cGetEpisodesTimeStamp = "GetEpisodesTimeStamp";
        public const String cUpdateSeriesTimeStamp = "UpdateSeriesTimeStamp";
        public const String cUpdateBannersTimeStamp = "UpdateBannersTimeStamp";
        public const String cUpdateEpisodesTimeStamp = "UpdateEpisodesTimeStamp";

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

        public const String cSubs_Forom_BaseURL = "Subs_Forom_BaseURL";
        public const String cSubs_Forom_ID = "Subs_Forom_ID";

        static DBOption()
        {
            try
            {
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

                if (GetOptions(cDBSeriesVersion) == null)
                    SetOptions(cDBSeriesVersion, 1);

                if (GetOptions(cDBSeriesLastLocalID) == null)
                    SetOptions(cDBSeriesLastLocalID, -1);

                if (GetOptions(cDBSeasonVersion) == null)
                    SetOptions(cDBSeasonVersion, 1);

                if (GetOptions(cDBEpisodesVersion) == null)
                    SetOptions(cDBEpisodesVersion, 1);

                // update default values if not there already
                if (GetOptions(cOnlineParseEnabled) == null)
                    SetOptions(cOnlineParseEnabled, true);

                if (GetOptions(cFullSeriesRetrieval) == null)
                    SetOptions(cFullSeriesRetrieval, false);

                if (GetOptions(cAutoChooseSeries) == null)
                    SetOptions(cAutoChooseSeries, false);

                if (GetOptions(cLocalDataOverride) == null)
                    SetOptions(cLocalDataOverride, true);

                if (GetOptions(cView_Episode_OnlyShowLocalFiles) == null)
                    SetOptions(cView_Episode_OnlyShowLocalFiles, true);

                if (GetOptions(cView_Episode_HideUnwatchedSummary) == null)
                    SetOptions(cView_Episode_HideUnwatchedSummary, true);

                if (GetOptions(cGetEpisodesTimeStamp) == null)
                    SetOptions(cGetEpisodesTimeStamp, 0);

                if (GetOptions(cUpdateSeriesTimeStamp) == null)
                    SetOptions(cUpdateSeriesTimeStamp, 0);

                if (GetOptions(cUpdateBannersTimeStamp) == null)
                    SetOptions(cUpdateBannersTimeStamp, 0);

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

                if (GetOptions(cView_PluginName) == null)
                    SetOptions(cView_PluginName, "My TV Series");

                if (GetOptions(cViewAutoHeight) == null)
                    SetOptions(cViewAutoHeight, true);

                if (GetOptions(cView_Series_ListFormat) == null)
                    SetOptions(cView_Series_ListFormat, 1); // graphical by default

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

                if (GetOptions(cView_Season_ListFormat) == null)
                    SetOptions(cView_Season_ListFormat, 0); // text by default

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

                if (GetOptions(cSubs_Forom_BaseURL) == null)
                    SetOptions(cSubs_Forom_BaseURL, @"http://www.foroms.net/documents/index.php");
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
                String convertedProperty = property;
                String convertedvalue = value;

                String sqlQuery;
                if (GetOptions(convertedProperty) == null)
                    sqlQuery = "insert into options (option_id, property, value) values(NULL, '" + convertedProperty + "', '" + convertedvalue + "')";
                else
                    sqlQuery = "update options set value = '" + value + "' where property = '" + convertedProperty + "'";
                DBTVSeries.Execute(sqlQuery);
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

                string sqlQuery = "select value from options where property = '" + convertedProperty + "'";
                SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
                if (results.Rows.Count > 0)
                    return DatabaseUtility.Get(results, 0, "value");
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("An Error Occurred (" + ex.Message + ").");
            }
            return null;
        }
    };
}
