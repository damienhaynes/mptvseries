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
                UpdateTable();
                String convertedProperty = property;
                String convertedvalue = value;

                DatabaseUtility.RemoveInvalidChars(ref convertedProperty);
                DatabaseUtility.RemoveInvalidChars(ref convertedvalue);
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
                DBTVSeries.Log("An Error Occurred (" + ex.Message + ").");
                return false;
            }
        }

        public static DBValue GetOptions(String property)
        {
            try
            {
                UpdateTable();
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
            return null;
        }
    };
}
