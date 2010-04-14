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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using SQLite.NET;
using WindowPlugins.GUITVSeries.DataClass;

namespace WindowPlugins.GUITVSeries.DataBase
{
    /// <summary>
    /// An alias for a Collection of DBTables
    /// </summary>
    public class DBTableList : Dictionary<string, DBFieldList>{}

    /// <summary>
    /// table class - used as a base for table objects (series, episodes, etc)
    /// holds a field hash table, includes an update mechanism to keep the DB tables in sync
    /// </summary>
    public abstract class DBTable
    {
        public const string cUserEditPostFix = @"__USEREDIT__";

        public string TableName { get; private set; }

        public DBField PrimaryKey
        {
            get
            {
                return m_fields.PrimaryKey;
            }
        }

		private bool m_CommitNeeded = false;

        /// <summary>
        /// list of the tables field names mapped to fields
        /// </summary>
        public DBFieldList m_fields = new DBFieldList();
        
        /// <summary>
        /// a list of all the dbtables in the database
        /// </summary>
        private static readonly DBTableList m_tableList = new DBTableList();

        #region events
        public delegate void dbUpdateOccuredDelegate(string table);
        public static event dbUpdateOccuredDelegate dbUpdateOccured;
        #endregion

        static DBTable()
        {
            //test the version number
            Version lastVersion = new Version(DBOption.GetOptions(DBOption.cDBLastVersion));
            Version currentVersion = Settings.Version;
        	if (lastVersion == currentVersion) {
        		return;
        	}
			MPTVSeriesLog.Write("Performing database upgrade - please be patient");

        	DBEpisode.MaintainDatabaseTable(lastVersion);
        	DBExpression.MaintainDatabaseTable(lastVersion);
        	DBFanart.MaintainDatabaseTable(lastVersion);
        	DBFormatting.MaintainDatabaseTable(lastVersion);
        	DBIgnoredDownloadedFiles.MaintainDatabaseTable(lastVersion);
        	DBNewzbin.MaintainDatabaseTable(lastVersion);
        	DBOnlineEpisode.MaintainDatabaseTable(lastVersion);

        	DBOnlineSeries.MaintainDatabaseTable(lastVersion);
        	DBImportPath.MaintainDatabaseTable(lastVersion);
        	DBReplacements.MaintainDatabaseTable(lastVersion);
        	DBSeason.MaintainDatabaseTable(lastVersion);
        	DBSeries.MaintainDatabaseTable(lastVersion);
        	DBTorrentSearch.MaintainDatabaseTable(lastVersion);
        	DBUserSelection.MaintainDatabaseTable(lastVersion);
        	DBView.MaintainDatabaseTable(lastVersion);

			MPTVSeriesLog.Write("Cleaning up database");
        	DBTVSeries.Execute("Vacuum");
			MPTVSeriesLog.Write("Database upgrade Finished");

			DBOption.SetOptions(DBOption.cDBLastVersion, currentVersion.ToString());
        }
        
        protected DBTable(string tableName, DBFieldDefList requiredFields)
        {
            TableName = tableName;

            // this base constructor was very expensive
            // we now cache the result for all objects : dbtable and only redo it if an alter table occured (this drastically cut down the number of sql statements)
            // this piece of code alone took over 30% of the time on my machine when entering config and newing up all the episodes
            DBFieldList cachedForTable;
            if (m_tableList.TryGetValue(tableName, out cachedForTable)) {// good, cached, this happens 99% of the time
                m_fields = cachedForTable.Copy();
            } else {
                // we have to get it, happens when the first object is created or after an alter table
                cachedForTable = new DBFieldList();
                // load up fields from the database, this allows for fields which are not referneced by code

                //Test if the table exists
                if (DatabaseHelper.TableExists(TableName)) {
					//ensure that all required columns are in the table
					AddColumns(requiredFields.Values);

                    #region read table data from database

                    // we have the table definition, parse it for names/types
                	String sCreateTable = DatabaseHelper.GetTableSQL(TableName);
                    String RegExp = @"CREATE TABLE .*?\((.*?)\)";
                    Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
                    Match tablematch = Engine.Match(sCreateTable);
                    if (tablematch.Success)
                    {
                        String sParameters = tablematch.Groups[1].Value + ','; //trailng comma make the regex below find the last column
                        // we have the list of parameters, parse them
                        // group 1: fieldname, group2: type, group3: test for primary key, group4: any thing else until a comma (not null, Default value etc.)
                        RegExp = @"([^\s]+)\s+([^\s,]+)(\s+primary key)?([^,]+)?,";
                        Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
                        MatchCollection matches = Engine.Matches(sParameters);
                        foreach (Match parammatch in matches)
                        {
                            String sName = parammatch.Groups[1].Value;
                            // could be either "int" or "integer"
                            bool bIntType = parammatch.Groups[2].Value.StartsWith("int", StringComparison.InvariantCultureIgnoreCase);
                            bool bPrimary = parammatch.Groups[3].Success;
                            // In Sqlite an "integer" (but not "int") Primary Key is an alias for the sqlite rowid, and therefore auto increments
                            bool bAutoIncrement = (bPrimary && parammatch.Groups[2].Value.Equals("integer", StringComparison.InvariantCultureIgnoreCase)) ||
                                                  // or a column can be set as autoincrement
                                                  parammatch.Groups[4].Value.ToLowerInvariant().Contains("autoincrement");

                            DBFieldDef fieldDef = new DBFieldDef() { FieldName = sName,
                                                       Type = (bIntType ? DBFieldType.Int : DBFieldType.String),
                                                       Primary = bPrimary, 
                                                       AutoIncrement = bAutoIncrement};
                            DBField field = new DBField(fieldDef);

                            if (!m_fields.ContainsKey(sName))
                            {
                                m_fields.Add(sName, field);
                                if (field.Primary) {
                                    m_fields.PrimaryKey = field;
                                }
                            }
                            cachedForTable.Add(sName, field);
                        }
                        lock (m_tableList)
                            m_tableList.Add(tableName, cachedForTable);

                    }
                    else
                    {
                        MPTVSeriesLog.Write("parsing of CREATE TABLE failed!!!");
                    }
                    #endregion
                } else {
                    //create the table
					DatabaseHelper.CreateTable(tableName, requiredFields.Values);
                }
            }
        }

        public virtual bool AddColumn(DBFieldDef field)
        {
            // verify if we already have that field avail
            if (!m_fields.ContainsKey(field.FieldName)) {
                return AddColumns(new[] { field });
            }
            return false;
        }

        private bool AddColumns(IEnumerable<DBFieldDef> fields)
        {
            // verify if we already have that fields
            List<DBFieldDef> newFields = new List<DBFieldDef>();
            foreach (DBFieldDef field in fields) {
                if (!m_fields.ContainsKey(field.FieldName)) {
                    newFields.Add(field);
                }
            }

            if (newFields.Count > 0) 
            {
                try
                {
                    if (DatabaseHelper.TableExists(TableName))
                    {
                        // table already exists, alter it
                        foreach (DBFieldDef field in newFields) {
                            DatabaseHelper.AddColumn(TableName, field);
                        }
                    }
                    else
                    {
                        DatabaseHelper.CreateTable(TableName, newFields);
                        foreach (DBFieldDef field in newFields) {
                            m_fields.Add(field.FieldName, new DBField(field));
                        }                        
                    }
                    // delete the s_fields cache so newed up objects get the right fields
                    lock (m_tableList)
                        m_tableList.Remove(TableName);
                    return true;
                }
                catch (Exception ex)
                {
                    MPTVSeriesLog.Write(TableName + " table.AddColumn failed (" + ex.Message + ").");
                    return false;
                }
            }
            return false;
        }

        public virtual DBValue this[String fieldName]
        {
            get
            {
                DBField result;
                if (!m_fields.TryGetValue(fieldName, out result)) {
                    return string.Empty;
                }
                return result.Value;

            }
            set
            {
                try
                {
                    DBField result;
                    if (m_fields.TryGetValue(fieldName, out result))
                    {
                        if (result.Value != value)
                        {
                            if (result.Type == DBFieldType.Int) {
                                result.Value = (long)value;
                            } else {
                                result.Value = value;
                            }
                            result.WasChanged = true;
                            m_CommitNeeded = true;
                        }
                    }
                    else
                    {
                        AddColumn(new DBFieldDef() { FieldName = fieldName, Type = DBFieldType.String });
                        this[fieldName] = value;
                    }
                }
                catch (SystemException)
                {
                    MPTVSeriesLog.Write("Cast exception when trying to assign " + value + " to field " + fieldName + " in table " + TableName);
                }
            }
        }

        public virtual ICollection<String> FieldNames
        {
            get
            {
                return new List<String>(m_fields.Keys);
            }
        }

        public bool Read(ref SQLiteResultSet records, int index)
        {
            if (records.Rows.Count > 0 || records.Rows.Count < index) {
                SQLiteResultSet.Row row = records.Rows[index];
                return Read(row, records.ColumnIndices);
            }
            return false;
        }

        public bool Read(SQLiteResultSet.Row row, System.Collections.Hashtable ColumnIndices)
        {
            if (row == null || row.fields.Count == 0) {
                return false;
            }
            foreach (DBField field in m_fields.Values)
            {
                object o = null;
                if (((o = ColumnIndices[field.FieldName]) != null)
                    || ((o = ColumnIndices[TableName + "." + field.FieldName]) != null)
                    || ((o = ColumnIndices[TableName + field.FieldName]) != null)) // because of order bug in sqlite
                {
                    field.Value = row.fields[(int)o] ?? string.Empty;
                }
                else
                    // we have a column in mfields that is not in the database result (or null), as such it is to be empty
                    field.Value = string.Empty;
            }
            m_CommitNeeded = false;
            return true;
        }

        public bool ReadPrimary(DBValue Value)
        {
            try
            {
                PrimaryKey.Value = Value;
                SQLCondition condition = new SQLCondition();
                condition.Add(this, PrimaryKey.FieldName, PrimaryKey.Value, SQLConditionType.Equal);
                String sqlQuery = "select * from " + TableName + condition;
                SQLiteResultSet records = DBTVSeries.Execute(sqlQuery);
                return Read(ref records, 0);
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("An Error Occurred (" + ex.Message + ").");
            }
            return false;
        }

        public virtual bool Commit()
        {
            try
            {
                if (!m_CommitNeeded)
                    return false;

                bool update = false;

                if (String.IsNullOrEmpty(PrimaryKey.Value) && !PrimaryKey.AutoIncrement)
                    return false;

                String sqlQuery;
                StringBuilder builder = new StringBuilder();
                String sWhere = " where ";

                if (!String.IsNullOrEmpty(PrimaryKey.Value))
                {

                    switch (PrimaryKey.Type)
                    {
                        case DBFieldType.Int:
                            sWhere += PrimaryKey.FieldName + " = " + PrimaryKey.Value;
                            break;

                        case DBFieldType.String:
                            sWhere += PrimaryKey.FieldName + " = '" + PrimaryKey.SQLSafeValue + "'";
                            break;
                    }

                    // use the primary key field
                    sqlQuery = "select " + PrimaryKey.FieldName + " from " + TableName + sWhere;
                    SQLiteResultSet records = DBTVSeries.Execute(sqlQuery);
                    if (records.Rows.Count > 0)
                    {
                        update = true;
                    }
                }

                if (update)
                {
                    // already exists, update
                    builder.Append("update ").Append(TableName).Append(" set ");
                    int fieldsNeedingUpdating = 0;
                    foreach (DBField field in m_fields.Values)
                    {
                        if (!field.Primary && field.WasChanged)
                        {
                            builder.Append(field.FieldName).Append(" = ");
                            switch (field.Type)
                            {
                                case DBFieldType.Int:
                                    if (String.IsNullOrEmpty(field.Value))
                                        builder.Append("'',");
                                    else
                                        builder.Append((string)field.Value).Append(',');
                                    break;

                                case DBFieldType.String:
                                    builder.Append("'").Append(field.SQLSafeValue).Append("',");
                                    break;
                            }
                            fieldsNeedingUpdating++;
                            field.WasChanged = false;
                        }
                    }
                    if (fieldsNeedingUpdating > 0)
                    {
                        sqlQuery = builder.ToString().Substring(0, builder.Length - 1) + sWhere;
                        DBTVSeries.Execute(sqlQuery);
                    }
                }
                else
                {
                    // add new record
                    StringBuilder paramNames = new StringBuilder();
                    bool first = true;
                    foreach (DBField field in m_fields.Values)
                    {
                        if (!first)
                        {
                            paramNames.Append(',');
                            builder.Append(',');
                        }
                        else
                        {
                            if (field.AutoIncrement)
                            {
                                //skip the autoincrementing field as we want this to be generated
                                continue;
                            }
                            first = false;
                        }
                        paramNames.Append(field.FieldName);
                        switch (field.Type)
                        {
                            case DBFieldType.Int:
                                if (String.IsNullOrEmpty(field.Value))
                                    builder.Append("''");
                                else
                                    builder.Append((string)field.Value);
                                break;

                            case DBFieldType.String:
                                builder.Append("'").Append(field.SQLSafeValue).Append("'");
                                break;
                        }

                    }
                    String sParamValues = builder.ToString();
                    builder.Remove(0, builder.Length);
                    builder.Append("insert into ").Append(TableName).Append(" (").Append(paramNames).Append(") values(").Append(sParamValues).Append(")");
                    sqlQuery = builder.ToString();

                    DBTVSeries.Execute(sqlQuery);

                    if (PrimaryKey.AutoIncrement)
                    {
                        //we've just done an insert to an auto crementing field, so fetch the value
                        SQLiteResultSet results = DBTVSeries.Execute("SELECT last_insert_rowid() AS ID");
                        PrimaryKey.Value = int.Parse(results.Rows[0].fields[0]);
                    }
                }

                m_CommitNeeded = false;
                return true;
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("An Error Occurred (" + ex.Message + ").");
                return false;
            }
        }

        public static void GlobalSet(DBTable obj, String sKey, DBValue Value, SQLCondition conditions)
        {
            if (obj.m_fields.ContainsKey(sKey))
            {
                String sqlQuery = "update " + obj.TableName + " SET " + sKey + "=";
                switch (obj.m_fields[sKey].Type)
                {
                    case DBFieldType.Int:
                        sqlQuery += Value;
                        break;

                    case DBFieldType.String:
                        sqlQuery += "'" + Value + "'";
                        break;
                }

                sqlQuery += conditions;
                SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
                if (dbUpdateOccured != null)
                    dbUpdateOccured(obj.TableName);
            }
        }

        public static void GlobalSet(DBTable obj, String sKey1, String sKey2, SQLCondition conditions)
        {
            if (obj.m_fields.ContainsKey(sKey1) && obj.m_fields.ContainsKey(sKey2))
            {
                String sqlQuery = "update " + obj.TableName + " SET " + sKey1 + " = " + sKey2 + conditions;
                SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
                if (dbUpdateOccured != null)
                    dbUpdateOccured(obj.TableName);
            }
        }

        public static void Clear(DBTable obj, SQLCondition conditions)
        {
            String sqlQuery = "delete from " + obj.TableName + conditions;
            SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
            if (dbUpdateOccured != null)
                dbUpdateOccured(obj.TableName);
        }

        public static List<DBValue> GetSingleField(string field, SQLCondition conds, DBTable obj)
        {

            string sql = "select " + field + " from " + obj.TableName + conds + conds.orderString + conds.limitString;
            List<DBValue> results = new List<DBValue>();
            try
            {
                foreach (SQLiteResultSet.Row result in DBTVSeries.Execute(sql).Rows)
                {
                    results.Add(result.fields[0]);
                }
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("GetSingleField SQL method generated an error: " + ex.Message);
            }
            return results;
        }

        /// <summary>
        /// For Genre etc. this method will split by each character in "splits"
        /// Should be used only for fields described in FieldsRequiringSplit
        /// </summary>
        /// <param name="fieldvalue"></param>
        /// <returns></returns>
        public static string[] splitField(string fieldvalue)
        {
            char[] splits = new char[] { '|', '\\', '/', ',' };
            return fieldvalue.Split(splits, StringSplitOptions.RemoveEmptyEntries);
        }
    };
}