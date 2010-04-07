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
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;

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
        //private static SQLiteClient m_db = null;
        private static DBProvider m_DBProvider = null;

        private static int m_nLogLevel = 0; // normal log = 0; debug log = 1;

        private static bool m_bIndexOnlineEpisodes;
        private static bool m_bIndexLocalEpisodes;
        private static bool m_bIndexLocalSeries;

        private static DbConnection GetConnection()
        {
            if (m_DBProvider == null) {
                InitDB();
            }

            DbProviderFactory factory = null;

            //because were making a dll plugin we can't add the dataprovider to the app.config, and can't be sure that
            //the data provider has been added to the machine.config - so hard code it for now
            if (m_DBProvider.sProviderName == "System.Data.SQLite") {
                factory = System.Data.SQLite.SQLiteFactory.Instance;
            } else if (m_DBProvider.sProviderName == "MySql.Data.MySqlClient") {
                factory = MySql.Data.MySqlClient.MySqlClientFactory.Instance;
            } else {
                factory = DbProviderFactories.GetFactory(m_DBProvider.sProviderName);
            }

            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = m_DBProvider.sConnectionString;

            return connection;
        }

        private static void InitDB()
        {
            if (System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToLower() == "devenv")
                return;

            switch (Settings.SQLClient) {
                case Settings.SqlClients.sqlClient:
                    m_DBProvider = new SQLClientProvider(Settings.ConnectionString);
                    break;
                case Settings.SqlClients.mySql:
                    m_DBProvider = new MySqlProvider(Settings.ConnectionString);
                    break;
                default:
                    m_DBProvider = new SQLiteProvider(Settings.GetPath(Settings.Path.database));
                    break;
            }

            try {
                m_DBProvider.InitDB();

                MPTVSeriesLog.Write("Successfully opened database '" + m_DBProvider.sConnectionString + "'");
            } catch (Exception ex) {
                MPTVSeriesLog.Write("Failed to open database '" + m_DBProvider.sConnectionString + "' (" + ex.Message + ")");
            }
        }
        #endregion
        public static void SetGlobalLogLevel(int nLevel)
        {
            m_nLogLevel = nLevel;
        }

        public static void CreateDBIndices(string sCommand, string sTable, string sIndex, bool bSetFlag)
        {
            try {
                switch (sTable) {
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
                if (!IndexExists(sIndex, sTable)) {
                    Execute(sCommand);
                }
            } catch (Exception e) {
                MPTVSeriesLog.Write("Warning, failed to create Index: " + e.Message);
            }
        }

        public static void Close()
        {
            String databaseFile = string.Empty;
            databaseFile = Settings.GetPath(Settings.Path.database);

            try {
                //m_db.Close();
                //m_db.Dispose();
                //m_db = null; ;
                MPTVSeriesLog.Write("Successfully closed Database: " + databaseFile);
            } catch (Exception) {
                MPTVSeriesLog.Write("Failed closing Database: " + databaseFile);
            }
        }

        /// <summary>
        /// Get a set of values from the database
        /// </summary>
        /// <param name="sCommand"></param>
        /// <returns></returns>
        public static DataTable Execute(String sCommand)
        {
            sCommand = m_DBProvider.Clean(sCommand);

            DataTable result = new DataTable();
            using (DbConnection connection = GetConnection()) {
                try {
                    MPTVSeriesLog.Write("Executing SQL: ", sCommand, MPTVSeriesLog.LogLevel.DebugSQL);
                    connection.Open();
                    using (DbCommand command = connection.CreateCommand()) {
                        command.CommandText = sCommand;
                        command.CommandTimeout = 120;
                        using (DbDataReader reader = command.ExecuteReader()) {
                            //setup the data types for the columns, to avoid the incorrect columns being loaded
                            for (int i = 0; i < reader.FieldCount; i++) {
                                if (reader.GetFieldType(i) == typeof(int)) {
                                    result.Columns.Add(reader.GetName(i), typeof(int));
                                } else {
                                    result.Columns.Add(reader.GetName(i), typeof(string));
                                }
                            }
                            result.Load(reader);
                        }
                        MPTVSeriesLog.Write("Success, returned Rows: ", result.Rows.Count, MPTVSeriesLog.LogLevel.DebugSQL);
                    }
                    return result;
                } catch (Exception ex) {
                    MPTVSeriesLog.Write("Commit failed on this command: <" + sCommand + "> (" + ex.Message + ").");
                    return new DataTable();
                } finally {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Gets a single value from the Database
        /// </summary>
        /// <param name="sCommand"></param>
        /// <returns></returns>
        public static object ExecuteScalar(String sCommand)
        {
            sCommand = m_DBProvider.Clean(sCommand);

            using (DbConnection connection = GetConnection()) {

                try {
                    MPTVSeriesLog.Write("Executing SQL: ", sCommand, MPTVSeriesLog.LogLevel.DebugSQL);
                    connection.Open();
                    Object result = null;
                    using (DbCommand command = connection.CreateCommand()) {
                        command.CommandText = sCommand;
                        command.CommandTimeout = 120;
                        result = command.ExecuteScalar();
                    }
                    MPTVSeriesLog.Write("Success, returned Value: ", result == null ? "null" : result.ToString(), MPTVSeriesLog.LogLevel.DebugSQL);
                    return result;
                } catch (Exception ex) {
                    MPTVSeriesLog.Write("Commit failed on this command: <" + sCommand + "> (" + ex.Message + ").");
                    return null;
                } finally {
                    connection.Close();
                }
            }
        }

        public static bool TableExists(String sName)
        {
            using (DbConnection connection = GetConnection()) {

                try {
                    MPTVSeriesLog.Write("Quering SQL Schema: ", string.Format("TableExists: {0}", sName), MPTVSeriesLog.LogLevel.DebugSQL);
                    connection.Open();

                    DataTable tables = connection.GetSchema("Tables");
                    foreach (DataRow row in tables.Rows) {
                        if (sName.Equals(row["TABLE_NAME"] as string, StringComparison.CurrentCultureIgnoreCase)) {
                            MPTVSeriesLog.Write("Success, Table Found: ", "true", MPTVSeriesLog.LogLevel.DebugSQL);
                            return true;
                        }
                    }

                    MPTVSeriesLog.Write("Success, Table Not Found: ", "false", MPTVSeriesLog.LogLevel.DebugSQL);
                    return false;
                } catch (Exception ex) {
                    MPTVSeriesLog.Write("Query failed looking for this table: <" + sName + "> (" + ex.Message + ").");
                    return false;
                } finally {
                    connection.Close();
                }
            }
        }

        public static bool IndexExists(String sName, String sTableName)
        {
            using (DbConnection connection = GetConnection()) {

                try {
                    MPTVSeriesLog.Write("Quering SQL Schema: ", string.Format("IndexExists: {0}", sName), MPTVSeriesLog.LogLevel.DebugSQL);
                    connection.Open();

                    DataTable tables = connection.GetSchema("Indexes", new string[] { null, null, sTableName, null });
                    foreach (DataRow row in tables.Rows) {
                        if (sName.Equals(row["index_name"] as string, StringComparison.CurrentCultureIgnoreCase)) {
                            MPTVSeriesLog.Write("Success, INdex Found: ", "true", MPTVSeriesLog.LogLevel.DebugSQL);
                            return true;
                        }
                    }

                    MPTVSeriesLog.Write("Success, Index Not Found: ", "false", MPTVSeriesLog.LogLevel.DebugSQL);
                    return false;
                } catch (Exception ex) {
                    MPTVSeriesLog.Write("Query failed looking for this index: <" + sName + "> (" + ex.Message + ").");
                    return false;
                } finally {
                    connection.Close();
                }
            }
        }

        public static DataTable GetTableColumns(String sName)
        {
            using (DbConnection connection = GetConnection()) {
                try {
                    MPTVSeriesLog.Write("Quering SQL Schema: ", string.Format("GetTableColumns: {0}", sName), MPTVSeriesLog.LogLevel.DebugSQL);
                    connection.Open();

                    DataTable result = new DataTable();
                    using (DbCommand command = connection.CreateCommand()) {
                        command.CommandText = string.Format("select * from {0}", sName);
                        using (DbDataReader reader = command.ExecuteReader(CommandBehavior.KeyInfo)) {
                            result.Load(reader);
                        }
                    }
                    if (result != null) {
                        MPTVSeriesLog.Write("Success, found x columns: ", result.Rows.Count, MPTVSeriesLog.LogLevel.DebugSQL);
                    } else {
                        MPTVSeriesLog.Write("Success, but table not found: ", 0, MPTVSeriesLog.LogLevel.DebugSQL);
                    }
                    return result;
                } catch (Exception ex) {
                    MPTVSeriesLog.Write("Query failed looking for this table:  <" + sName + "> (" + ex.Message + ").");
                    return null;
                } finally {
                    connection.Close();
                }
            }
        }

        public static void AddColumn(string tableName, string fieldName, DBField field)
        {
            if (m_DBProvider == null) {
                InitDB();
            }
            if (TableExists(tableName)) {
                m_DBProvider.AddColumn(tableName, fieldName, field);
            } else {
                m_DBProvider.CreateTable(tableName, fieldName, field);
            }
        }

        public static bool bUseLimit
        {
            get
            {
                if (m_DBProvider == null) {
                    InitDB();
                }
                return m_DBProvider.bUseLimit;
            }
        }

        public static string sGetLastIdCommand
        {
            get
            {
                if (m_DBProvider == null) {
                    InitDB();
                }
                return m_DBProvider.sGetLastIdCommand;
            }
        }

        public static char cIdentifierStart
        {
            get
            {
                if (m_DBProvider == null) {
                    InitDB();
                }
                return m_DBProvider.cIdentifierStart;
            }
        }

        public static char cIdentifierFinish
        {
            get
            {
                if (m_DBProvider == null) {
                    InitDB();
                }
                return m_DBProvider.cIdentifierFinish;
            }
        }

        #region public properties

        public static bool IsDatabaseOnNetworkPath
        {
            get
            {
                String databaseFile = string.Empty;
                databaseFile = Settings.GetPath(Settings.Path.database);

                bool isRemotePath = PathIsNetworkPath(databaseFile);
                return isRemotePath;
            }
        }

        public static string CurrentDatabaseDescription
        {
            get
            {
                if (m_DBProvider == null) {
                    InitDB();
                }
                return m_DBProvider.Description;
            }
        }

        #endregion

        public static void DatabaseToXML()
        {
            DataTable tables;
            using (DbConnection connection = GetConnection()) {
                connection.Open();
                tables = connection.GetSchema("Tables");
            }

            DataSet data = new DataSet("MPTVSeries");

            foreach (DataRow row in tables.Rows) {
                string tablename = row["TABLE_NAME"].ToString();
                DataTable datatable = DBTVSeries.Execute("select * from " + tablename);
                data.Tables.Add(datatable);
            }

            data.WriteXml("mptvseries.xml");
        }

        public static void XmlToDatabase()
        {
            DataSet data = new DataSet("MPTVSeries");
            data.ReadXml("mptvseries.xml");

            using (DbConnection connection = GetConnection()) {
                connection.Open();

                foreach (DataTable table in data.Tables) {
                    string name = '`' + table.TableName + '`';
                    string columnnames = "(";
                    string parameternames = "(";
                    foreach (DataColumn column in table.Columns) {
                        if (column.ColumnName != "option_id") {
                            columnnames += '`' + column.ColumnName + '`' + ",";
                            parameternames += "@" + column.ColumnName + ",";
                        }
                    }
                    columnnames = columnnames.Remove(columnnames.Length - 1);
                    parameternames = parameternames.Remove(parameternames.Length - 1);
                    columnnames += ")";
                    parameternames += ")";
                    foreach (DataRow row in table.Rows) {
                        try {
                            using (DbCommand command = connection.CreateCommand()) {
                                command.CommandText = "insert into " + name + columnnames + " values " + parameternames;
                                foreach (DataColumn column in table.Columns) {
                                    if (column.ColumnName != "option_id") {
                                        DbParameter parameter = command.CreateParameter();
                                        parameter.ParameterName = "@" + column.ColumnName;
                                        parameter.Value = row[column];
                                        command.Parameters.Add(parameter);
                                    }
                                }
                                command.ExecuteNonQuery();
                            }
                        } catch (Exception e) {
                            int i = 0;
                        }
                    }
                }
            }
        }
    };
}