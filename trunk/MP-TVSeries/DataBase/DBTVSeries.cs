#region GNU license
// MP-TVSeries - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2015
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
using System.IO;
using System.Runtime.InteropServices;
using SQLite.NET;
using MediaPortal.Configuration;

namespace WindowPlugins.GUITVSeries
{
    /// <summary>
    /// DB class - static, one instance only. 
    /// holds the SQLite object and the log object.
    /// </summary>
    public class DBTVSeries
    {
        #region static imports

        [DllImport("shlwapi.dll")]
        private static extern bool PathIsNetworkPath(string Path);

        #endregion

        #region privates
        private static SQLiteClient m_db = null;        

        private static bool m_bIndexOnlineEpisodes;
        private static bool m_bIndexLocalEpisodes;
        private static bool m_bIndexLocalSeries;
        #endregion

        #region init
        private static void InitDB()
        {
            if (System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToLower() == "devenv")
                return;

            string databaseFile = Settings.GetPath(Settings.Path.database);

            bool databaseExists = true;
            if (!File.Exists(databaseFile))
                databaseExists = false;

            try
            {
                m_db = new SQLiteClient(databaseFile);

                // check database integrity
                if (databaseExists)
                {
                    if (CheckIntegrity())
                    {
                        // backup volume if all is good
                        Backup();
                    }
                    else
                    {
                        // restore last known good volume if corrupt
                        Restore();
                    }
                }
                
                ExecutePragmas();

                MPTVSeriesLog.Write("Successfully opened database. Filename = '{0}'", databaseFile);

                return;
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Failed to open database. Filename = '{0}', Reason = '{1}'", databaseFile, ex.Message);

                // restore last known good volume
                Restore();
                ExecutePragmas();
            }
        }

        private static void ExecutePragmas()
        {
            // Each page uses about 1.5K of memory
            m_db.Execute("PRAGMA cache_size=5000;");
            m_db.Execute("PRAGMA synchronous='OFF';");
            m_db.Execute("PRAGMA count_changes=1;");
            m_db.Execute("PRAGMA full_column_names=0;");
            m_db.Execute("PRAGMA short_column_names=0;");
            m_db.Execute("PRAGMA temp_store = MEMORY;");
        }

        private static void Backup()
        {
            MPTVSeriesLog.Write("Backing up database");
            
            string backupDirectory = Config.GetSubFolder(Config.Dir.Database, "MP-TVSeries_Backup");
            string sourceFile = Settings.GetPath(Settings.Path.database);
            string destinationFile = Path.Combine(backupDirectory, "TVSeriesDatabase4.db3");

            try
            {
                if (!Directory.Exists(backupDirectory))
                {
                    Directory.CreateDirectory(backupDirectory);
                }

                File.Copy(sourceFile, destinationFile, true);
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Failed to backup database. Source File = '{0}', Destination File = '{1}', Reason = '{2}'", sourceFile, destinationFile, ex.Message);
            }
        }

        private static void Restore()
        {
            // back the corrupt database in case user wants to fix themselves
            string backupDirectory = Config.GetSubFolder(Config.Dir.Database, "MP-TVSeries_Backup");
            string sourceFile = Settings.GetPath(Settings.Path.database);
            string destinationFile = Path.Combine(backupDirectory, string.Format("TVSeriesDatabase4-Corrupt-{0}.db3", DateTime.Now.ToString("yyyyMMddHHmmss")));
            
            MPTVSeriesLog.Write("Backing up corrupt database. Filename = '{0}'", destinationFile);

            try
            {
                File.Copy(sourceFile, destinationFile, true);
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Failed to backup corrupt database. Source File = '{0}', Destination File = '{1}', Reason = '{2}'", sourceFile, destinationFile, ex.Message);
            }

            MPTVSeriesLog.Write("Restoring last known good database");

            sourceFile = Path.Combine(backupDirectory, "TVSeriesDatabase4.db3");
            destinationFile = Settings.GetPath(Settings.Path.database);
            
            try
            {
                File.Copy(sourceFile, destinationFile, true);
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Failed to restore database. Source File = '{0}', Destination File = '{1}', Reason = '{2}'", sourceFile, destinationFile, ex.Message);
            }
        }
        #endregion

        #region public methods
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
                m_db = null;
                MPTVSeriesLog.Write("Successfully closed database. Filename = '{0}'", databaseFile);
            }
            catch (Exception)
            {
                MPTVSeriesLog.Write("Failed to close database. Filename = '{0}'", databaseFile);
            }
        }

        public static SQLiteResultSet Execute(String query)
        {
            if (m_db == null)
            {
                InitDB();
            }

            SQLite.NET.SQLiteResultSet result;
            try
            {
                MPTVSeriesLog.Write(string.Format("Executing SQL query. Query = '{0}'", query), MPTVSeriesLog.LogLevel.DebugSQL);
                result = m_db.Execute(query);
                MPTVSeriesLog.Write(string.Format("Successfully executed SQL query. Row Count = '{0}'", result.Rows.Count), MPTVSeriesLog.LogLevel.DebugSQL);
                return result;
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("SQL Execution Failed. Reason = '{0}', Query = '{1}'", ex.Message, query);
                return new SQLiteResultSet();
            }
        }

        public static bool CheckIntegrity()
        {
            string query = "PRAGMA integrity_check;";
            MPTVSeriesLog.Write("Executing SQL integrity check");

            SQLiteResultSet results = Execute(query);
            if (results != null)
            {
                if (results.Rows.Count == 1)
                {
                    SQLiteResultSet.Row arr = results.Rows[0];
                    if (arr.fields.Count == 1)
                    {
                        if (arr.fields[0] == "ok")
                        {
                            MPTVSeriesLog.Write("Database integrity check succeeded");
                            return true;
                        }
                    }
                }
            }
            MPTVSeriesLog.Write("Integrity check failed, database is corrupt. Filename = '{0}'", m_db.DatabaseName);
            return false;
        }
        #endregion

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