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

        public const String cDBVersion = "DBVersion";
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

        public const String cView_Episode_OnlyShowLocalFiles = "View_Episode_OnlyShowLocalFiles";
        public const String cView_Episode_HideUnwatchedSummary = "View_Episode_HideUnwatchedSummary";

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

                if (GetOptions(DBOption.cDBVersion) == "")
                    SetOptions(DBOption.cDBVersion, 1);

                // update default values if not there already
                if (GetOptions(DBOption.cOnlineParseEnabled) == "")
                    SetOptions(DBOption.cOnlineParseEnabled, true);

                if (GetOptions(DBOption.cFullSeriesRetrieval) == "")
                    SetOptions(DBOption.cFullSeriesRetrieval, false);

                if (GetOptions(DBOption.cAutoChooseSeries) == "")
                    SetOptions(DBOption.cAutoChooseSeries, false);

                if (GetOptions(DBOption.cLocalDataOverride) == "")
                    SetOptions(DBOption.cLocalDataOverride, true);

                if (GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles) == "")
                    SetOptions(DBOption.cView_Episode_OnlyShowLocalFiles, true);

                if (GetOptions(DBOption.cView_Episode_HideUnwatchedSummary) == "")
                    SetOptions(DBOption.cView_Episode_HideUnwatchedSummary, true);

                if (GetOptions(DBOption.cGetEpisodesTimeStamp) == "")
                    SetOptions(DBOption.cGetEpisodesTimeStamp, 0);

                if (GetOptions(DBOption.cUpdateSeriesTimeStamp) == "")
                    SetOptions(DBOption.cUpdateSeriesTimeStamp, 0);

                if (GetOptions(DBOption.cUpdateBannersTimeStamp) == "")
                    SetOptions(DBOption.cUpdateBannersTimeStamp, 0);

                if (GetOptions(DBOption.cUpdateEpisodesTimeStamp) == "")
                    SetOptions(DBOption.cUpdateEpisodesTimeStamp, 0);

                if (GetOptions(DBOption.cAutoScanLocalFiles) == "")
                    SetOptions(DBOption.cAutoScanLocalFiles, true);

                if (GetOptions(DBOption.cAutoScanLocalFilesLapse) == "")
                    SetOptions(DBOption.cAutoScanLocalFilesLapse, 5);

                if (GetOptions(DBOption.cAutoUpdateOnlineData) == "")
                    SetOptions(DBOption.cAutoUpdateOnlineData, true);

                if (GetOptions(DBOption.cAutoUpdateOnlineDataLapse) == "")
                    SetOptions(DBOption.cAutoUpdateOnlineDataLapse, 12);

                if (GetOptions(DBOption.cLocalScanLastTime) == "")
                    SetOptions(DBOption.cLocalScanLastTime, 0);

                if (GetOptions(DBOption.cUpdateScanLastTime) == "")
                    SetOptions(DBOption.cUpdateScanLastTime, 0);
            }
            catch (Exception ex)
            {
                DBTVSeries.Log("DBOption.UpdateTable failed (" + ex.Message + ").");
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
                DBTVSeries.Log("DBOption.UpdateTable failed (" + ex.Message + ").");
            }
        }

        public static bool SetOptions(String property, DBValue value)
        {
            try
            {
//                UpdateTable();
                String convertedProperty = property;
                String convertedvalue = value;

                DatabaseUtility.RemoveInvalidChars(ref convertedProperty);
                DatabaseUtility.RemoveInvalidChars(ref convertedvalue);
                String sqlQuery;
                if (GetOptions(convertedProperty) == "")
                    sqlQuery = "insert into options (option_id, property, value) values(NULL, '" + convertedProperty + "', '" + convertedvalue + "')";
                else
                    sqlQuery = "update options set value = '" + value + "' where property = '" + convertedProperty + "'";
                DBTVSeries.Execute(sqlQuery);
                return true;
            }
            catch (Exception ex)
            {
                DBTVSeries.Log("An Error Occurred (" + ex.Message + ").");
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
                DBTVSeries.Log("An Error Occurred (" + ex.Message + ").");
            }
            return new DBValue("");
        }
    };
}
