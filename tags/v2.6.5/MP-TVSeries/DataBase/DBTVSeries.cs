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
using System.Runtime.InteropServices;
using SQLite.NET;

namespace WindowPlugins.GUITVSeries
{
    /// <summary>
    /// DB class - static, one instance only. 
    /// holds the SQLLite object and the log object.
    /// </summary>
    public class DBTVSeries
    {

        #region static imports

        [DllImport("shlwapi.dll")]
        private static extern bool PathIsNetworkPath(string Path);

        #endregion

        #region private & init stuff        
        private static SQLiteClient m_db = null;
        private static int m_nLogLevel = 0; // normal log = 0; debug log = 1;

        private static bool m_bIndexOnlineEpisodes;
        private static bool m_bIndexLocalEpisodes;
        private static bool m_bIndexLocalSeries;

        private static void InitDB()
        {
            if (System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToLower() == "devenv")
                return;

            string databaseFile = Settings.GetPath(Settings.Path.database);

            try
            {
                m_db = new SQLiteClient(databaseFile);

                m_db.Execute("PRAGMA cache_size=5000;");        // Each page uses about 1.5K of memory
                m_db.Execute("PRAGMA synchronous='OFF';");
                m_db.Execute("PRAGMA count_changes=1;");
                m_db.Execute("PRAGMA full_column_names=0;");
                m_db.Execute("PRAGMA short_column_names=0;");
                m_db.Execute("PRAGMA temp_store = MEMORY;");

                // Indicies are now created when the tables exists
                //m_db.Execute("create index if not exists epComp1 ON local_episodes(CompositeID ASC)");
                //m_db.Execute("create index if not exists epComp2 ON local_episodes(CompositeID2 ASC)");
                //m_db.Execute("create index if not exists seriesIDLocal on local_series(ID ASC)");
                //m_db.Execute("create index if not exists seriesIDOnlineEp on online_episodes(SeriesID ASC)");
                
                MPTVSeriesLog.Write("Successfully opened database '" + databaseFile + "'");
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Failed to open database '" + databaseFile + "' (" + ex.Message + ")");
            }
        }
        #endregion
        public static void SetGlobalLogLevel(int nLevel)
        {
            m_nLogLevel = nLevel;
        }

        public static void CreateDBIndices(string sCommand, string sTable, bool bSetFlag)
        {            
            try
            {
                switch (sTable)
                {
                    case "online_episodes":
                        if (m_bIndexOnlineEpisodes)
                            return;
                        m_bIndexOnlineEpisodes = bSetFlag;
                        break;
                    case "local_episodes":
                        if (m_bIndexLocalEpisodes)
                            return;
                        m_bIndexLocalEpisodes = bSetFlag;
                        break;
                    case "local_series":
                        if (m_bIndexLocalSeries)
                            return;
                        m_bIndexLocalSeries = bSetFlag;
                        break;
                    default:
                        return;                        
                }                                               
                m_db.Execute(sCommand);
            }
            catch (Exception e)
            {
                MPTVSeriesLog.Write("Warning, failed to create Index: " + e.Message);
            }            
        }

        public static void Close ()
        {
            string databaseFile = Settings.GetPath(Settings.Path.database);

            try
            {            
                m_db.Close();
                m_db.Dispose();
                m_db = null; ;
                MPTVSeriesLog.Write("Successfully closed Database: " + databaseFile);
            }
            catch (Exception)
            {
                MPTVSeriesLog.Write("Failed closing Database: " + databaseFile);
            }
        }

        public static SQLiteResultSet Execute(String sCommand)
        {
            if (m_db == null)
            {
                InitDB();
            }
            SQLite.NET.SQLiteResultSet result;
            try
            {
                MPTVSeriesLog.Write("Executing SQL: ", sCommand, MPTVSeriesLog.LogLevel.DebugSQL);
                result = m_db.Execute(sCommand);
                MPTVSeriesLog.Write("Success, returned Rows: ", result.Rows.Count, MPTVSeriesLog.LogLevel.DebugSQL);
                return result;
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Commit failed on this command: <" + sCommand + "> (" + ex.Message + ").");
                return new SQLiteResultSet();
            }
        }

        #region public properties

        public static bool IsDatabaseOnNetworkPath
        {
            get
            {
                string databaseFile = Settings.GetPath(Settings.Path.database);
                return PathIsNetworkPath(databaseFile);
            }
        }

        #endregion
    };
}