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

namespace WindowPlugins.GUITVSeries
{
    /// <summary>
    /// table class - used as a base for table objects (series, episodes, etc)
    /// holds a field hash table, includes an update mechanism to keep the DB tables in sync
    /// </summary>
    public class DBTable
    {
        public const string cUserEditPostFix = @"__USEREDIT__";
        public string m_tableName;
        public Dictionary<string, DBField> m_fields = new Dictionary<string, DBField>();
        public bool m_CommitNeeded = false;
        public static List<string> FieldsRequiringSplit = new List<string>();
        public List<string> fieldsRequiringSplit = FieldsRequiringSplit;
        public delegate void dbUpdateOccuredDelegate(string table);
        public static event dbUpdateOccuredDelegate dbUpdateOccured;

        protected static Dictionary<string, Dictionary<string, DBFieldType>> fields = new Dictionary<string, Dictionary<string, DBFieldType>>();
        
        public DBTable(string tableName)
        {
            // this base constructor was very expensive
            // we now cache the result for all objects : dbtable and only redo it if an alter table occured (this drastically cut down the number of sql statements)
            // this piece of code alone took over 30% of the time on my machine when entering config and newing up all the episodes

            Dictionary<string, DBFieldType> cachedForTable;
            m_tableName = tableName;
            //m_fields = new Dictionary<string, DBField>();
            if (fields.TryGetValue(tableName, out cachedForTable)) // good, cached, this happens 99% of the time
            {
                foreach (KeyValuePair<string, DBFieldType> entry in cachedForTable)
                    if (!m_fields.ContainsKey(entry.Key))
                        m_fields.Add(entry.Key, new DBField(entry.Value));
                
            }
            else // we have to get it, happens when the first object is created or after an alter table
            {
                cachedForTable = new Dictionary<string, DBFieldType>();
                // load up fields from the table
                SQLiteResultSet results = DBTVSeries.Execute("SELECT sql FROM sqlite_master WHERE name='" + m_tableName + "'");
                if (results != null && results.Rows.Count > 0)
                {
                    // we have the table definition, parse it for names/types
                    String sCreateTable = results.Rows[0].fields[0];
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

                            DBFieldType cachedInfo = new DBFieldType {
                                                                         Primary = bPrimary,
                                                                         Type = (bIntType ? DBField.cTypeInt : DBField.cType.String),
                                                                         AutoIncrement = bAutoIncrement
                                                                     };

                            if (!m_fields.ContainsKey(sName))
                            {
                                m_fields.Add(sName, new DBField(cachedInfo));
                            }

                            cachedForTable.Add(sName, cachedInfo);
                        }
                        lock(fields)
                            fields.Add(tableName, cachedForTable);
                        
                    }
                    else
                    {
                        MPTVSeriesLog.Write("parsing of CREATE TABLE failed!!!");
                    }

                }
                else
                {
                    // no tables, assume it's going to be created later (using AddColumn)
                }
            }
        }

        public virtual bool AddColumn(String sName, DBField field)
        {
            // verify if we already have that field avail
            if (!m_fields.ContainsKey(sName))
            {
                if (m_fields.Count == 0 && !field.Primary) 
                {
                    throw new Exception("First field added needs to be the index");
                }

                try
                {
                    // ok, we don't, add it
                    SQLiteResultSet results = DBTVSeries.Execute("SELECT name FROM sqlite_master WHERE name='" + m_tableName + "'");
                    if (results != null && results.Rows.Count > 0)
                    {
                        // table already exists, alter it
                        String sQuery = "ALTER TABLE " + m_tableName + " ADD " + sName + " " + field.Type;
                        DBTVSeries.Execute(sQuery);
                    }
                    else
                    {
                        // new table, create it
                        // no tables, assume it's going to be created later (using AddColumn)
                        string type = field.Type.ToString();
                        if (field.Primary && field.Type == DBField.cType.Int && field.AutoIncrement) {
                            //for the automatic creation of an auto incremental integer primary key you must use the full "Integer" not just "int"
                            type = "Integer";
                        }

                        String sQuery = "CREATE TABLE " + m_tableName + " (" + sName + " " + type + (field.Primary ? " primary key)" : ")");
                        DBTVSeries.Execute(sQuery);
                    }
                    // delete the s_fields cache so newed up objects get the right fields
                    lock (fields)
                        fields.Remove(m_tableName);
                    m_fields.Add(sName, field);
                    return true;
                }
                catch (Exception ex)
                {
                    MPTVSeriesLog.Write(m_tableName + " table.AddColumn failed (" + ex.Message + ").");
                    return false;
                }
            }
            return false;
        }

        public virtual void InitValues()
        {
            foreach (KeyValuePair<string, DBField> fieldPair in m_fields)
            {
                if (!fieldPair.Value.Primary || fieldPair.Value.Value == null)
                {
                    switch (fieldPair.Value.Type)
                    {
                        case DBField.cTypeInt:                            
                            fieldPair.Value.Value = 0;
                            break;

                        case DBField.cTypeString:
                            fieldPair.Value.Value = "";
                            break;
                    }
                }
            }
            m_CommitNeeded = true;
        }

        public virtual void InitValues(Int32 iValue, String sValue)
        {
            foreach (KeyValuePair<string, DBField> fieldPair in m_fields)
            {
                if (!fieldPair.Value.Primary || fieldPair.Value.Value == null)
                {
                    switch (fieldPair.Value.Type)
                    {
                        case DBField.cTypeInt:
                            fieldPair.Value.Value = iValue;
                            break;

                        case DBField.cTypeString:
                            fieldPair.Value.Value = sValue;
                            break;
                    }
                }
            }
            m_CommitNeeded = true;
        }

        public virtual DBValue this[String fieldName]
        {
            get 
            {
                DBField result;
                if (!m_fields.TryGetValue(fieldName, out result)) return string.Empty;
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
                            if (result.Type == DBField.cTypeInt)
                                result.Value = (long)value;
                            else
                                result.Value = value;
                            result.WasChanged = true;
                            m_CommitNeeded = true;                            
                        }
                    }
                    else
                    {
                        AddColumn(fieldName, new DBField(DBField.cTypeString));
                        this[fieldName] = value;
                    }
                }
                catch (SystemException)
                {
                    MPTVSeriesLog.Write("Cast exception when trying to assign " + value + " to field " + fieldName + " in table " + m_tableName);
                }
            }
        }

        public virtual ICollection<String> FieldNames
        {
            get
            {
                List<String> outList = new List<String>();
                foreach (KeyValuePair<string, DBField> pair in m_fields)
                {
                    outList.Add(pair.Key);
                }
                return outList;
            }
        }

        public static String Q(String sField)
        {
            return sField;
        }

        public bool Read(ref SQLiteResultSet records, int index)
        {            
            if (records.Rows.Count > 0 || records.Rows.Count < index)
            {
                SQLiteResultSet.Row row = records.Rows[index];
                return Read(row, records.ColumnIndices);
            }
            return false;
        }

        public bool Read(SQLiteResultSet.Row row, System.Collections.Hashtable ColumnIndices)
        {
            if (row == null || row.fields.Count == 0) return false;
            foreach (KeyValuePair<string, DBField> field in m_fields)
            {
                object o = null;
                if (((o = ColumnIndices[field.Key]) != null) 
                    || ((o = ColumnIndices[m_tableName + "." + field.Key]) != null) 
                    || ((o = ColumnIndices[m_tableName + field.Key]) != null)) // because of order bug in sqlite
                {
                    int iCol = (int)o;
                    string res = row.fields[iCol];
                    field.Value.Value = res ?? string.Empty;
                }
                else 
                    // we have a column in mfields that is not in the database result (or null), as such it is to be empty
                    field.Value.Value = string.Empty;
            }
            m_CommitNeeded = false;
            return true;
        }

        public String PrimaryKey()
        {
            foreach (KeyValuePair<string, DBField> field in m_fields)
                if (field.Value.Primary == true)
                {
                    return field.Key;
                }

            return null;
        }

        public bool ReadPrimary(DBValue Value)
        {
            try
            {
                m_fields[PrimaryKey()].Value = Value;
                SQLCondition condition = new SQLCondition();
                condition.Add(this, PrimaryKey(), m_fields[PrimaryKey()].Value, SQLConditionType.Equal);
                String sqlQuery = "select * from " + m_tableName + condition;
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

                KeyValuePair<string, DBField> PrimaryField = new KeyValuePair<string, DBField>();

                foreach (KeyValuePair<string, DBField> field in m_fields)
                    if (field.Value.Primary == true)
                    {
                        PrimaryField = field;
                        break;
                    }
                bool update = false;

                if (String.IsNullOrEmpty(PrimaryField.Value.Value) && !PrimaryField.Value.AutoIncrement)
                    return false;

                String sqlQuery;
                StringBuilder builder = new StringBuilder();
                String sWhere = " where ";

                if (!String.IsNullOrEmpty(PrimaryField.Value.Value)) {
                    
                    switch (PrimaryField.Value.Type) {
                        case DBField.cTypeInt:
                            sWhere += PrimaryField.Key + " = " + PrimaryField.Value.Value;
                            break;

                        case DBField.cTypeString:
                            sWhere += PrimaryField.Key + " = '" + ((String)PrimaryField.Value.Value).Replace("'", "''") +
                                      "'";
                            break;
                    }

                    // use the primary key field
                    sqlQuery = "select " + PrimaryField.Key + " from " + m_tableName + sWhere;
                    SQLiteResultSet records = DBTVSeries.Execute(sqlQuery);
                    if (records.Rows.Count > 0) {
                        update = true;
                    }
                }

                if (update)
                {
                    // already exists, update
                    builder.Append("update ").Append(m_tableName).Append(" set ");
                    int fieldsNeedingUpdating = 0;
                    foreach (KeyValuePair<string, DBField> fieldPair in m_fields)
                    {
                        if (!fieldPair.Value.Primary && fieldPair.Value.WasChanged)
                        {
                            builder.Append(fieldPair.Key).Append(" = ");
                            switch (fieldPair.Value.Type)
                            {
                                case DBField.cTypeInt:
                                    if (String.IsNullOrEmpty(fieldPair.Value.Value))
                                        builder.Append("'',");
                                    else
                                        builder.Append((string)fieldPair.Value.Value).Append(',');
                                    break;

                                case DBField.cTypeString:
                                    builder.Append(" '");
                                    builder.Append((fieldPair.Value.Value.ToString()).Replace("'", "''"));
                                    builder.Append("',");
                                    break;
                            }
                            fieldsNeedingUpdating++;
                            fieldPair.Value.WasChanged = false;
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
                    foreach (KeyValuePair<string, DBField> fieldPair in m_fields)
                    {
                        if (!first) {
                            paramNames.Append(',');
                            builder.Append(',');
                        } else {
                            if (fieldPair.Value.AutoIncrement) {
                                //skip the autoincrementing field as we want this to be generated
                                continue;
                            }
                            first = false;
                        }
                        paramNames.Append(fieldPair.Key);
                        switch (fieldPair.Value.Type)
                        {
                            case DBField.cTypeInt:
                                if (String.IsNullOrEmpty(fieldPair.Value.Value))
                                    builder.Append("''");
                                else
                                    builder.Append((string)fieldPair.Value.Value);
                                break;

                            case DBField.cTypeString:
                                builder.Append(" '");
                                builder.Append((fieldPair.Value.Value.ToString()).Replace("'", "''"));
                                builder.Append("',");
                                break;
                        }

                    }
                    String sParamValues = builder.ToString();
                    builder.Remove(0, builder.Length);
                    builder.Append("insert into ").Append(m_tableName).Append(" (").Append(paramNames).Append(") values(").Append(sParamValues).Append(")");
                    sqlQuery = builder.ToString();

                    DBTVSeries.Execute(sqlQuery);

                    if (PrimaryField.Value.AutoIncrement) {
                        //we've just done an insert to an auto crementing field, so fetch the value
                        SQLiteResultSet results = DBTVSeries.Execute("SELECT last_insert_rowid() AS ID");
                        this[PrimaryField.Key] = int.Parse(results.Rows[0].fields[0]);
                        
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
                String sqlQuery = "update " + obj.m_tableName + " SET " + sKey + "=";
                switch (obj.m_fields[sKey].Type)
                {
                    case DBField.cTypeInt:
                        sqlQuery += Value;
                        break;

                    case DBField.cTypeString:
                        sqlQuery += "'" + Value + "'";
                        break;
                }

                sqlQuery += conditions;
                SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
                if (dbUpdateOccured != null)
                    dbUpdateOccured(obj.m_tableName);
            }
        }

        public static void GlobalSet(DBTable obj, String sKey1, String sKey2, SQLCondition conditions)
        {
            if (obj.m_fields.ContainsKey(sKey1) && obj.m_fields.ContainsKey(sKey2))
            {
                String sqlQuery = "update " + obj.m_tableName + " SET " + sKey1 + " = " + sKey2 + conditions;
                SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
                if (dbUpdateOccured != null)
                    dbUpdateOccured(obj.m_tableName);
            }
        }

        public static void Clear(DBTable obj, SQLCondition conditions)
        {
            String sqlQuery = "delete from " + obj.m_tableName + conditions;
            SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
            if (dbUpdateOccured != null)
                dbUpdateOccured(obj.m_tableName);
        }

        protected static string getRandomBanner(List<string> BannerList)
        {
            const string graphicalBannerRecognizerSubstring = "-g";
            string langIdentifier = "-lang" + Online_Parsing_Classes.OnlineAPI.SelLanguageAsString + "-";

            // random banners are prefered in the following order
            // 1) own lang + graphical
            // 2) own lang but not graphical
            // 3) english + graphical (english really is any other language banners that are in db)
            // 4) english but not graphical

            string randImage = null;
            if (BannerList == null || BannerList.Count == 0) return string.Empty;
            if (BannerList.Count == 1) randImage = BannerList[0];

            if (randImage == null)
            {
                List<string> langG = new List<string>();
                List<string> lang = new List<string>();
                List<string> engG = new List<string>();
                List<string> eng = new List<string>();
                for (int i = 0; i < BannerList.Count; i++)
                {
                    if(File.Exists(BannerList[i]))
                    {
                        if(BannerList[i].Contains(graphicalBannerRecognizerSubstring))
                        {
                            if(BannerList[i].Contains(langIdentifier))
                                langG.Add(BannerList[i]);
                            else
                                engG.Add(BannerList[i]);
                        }
                        else
                        {
                            if(BannerList[i].Contains(langIdentifier))
                                lang.Add(BannerList[i]);
                            else
                                eng.Add(BannerList[i]);
                        }
                    }
                }

                try
                {
                    if(langG.Count > 0) randImage = langG[new Random().Next(0, langG.Count)];
                    else if(lang.Count > 0) randImage = lang[new Random().Next(0, lang.Count)];
                    else if(engG.Count > 0) randImage = engG[new Random().Next(0, engG.Count)];
                    else if(eng.Count > 0) randImage = eng[new Random().Next(0, eng.Count)];
                    else return string.Empty;
                }
                catch
                {
                    MPTVSeriesLog.Write("Error getting random Image", MPTVSeriesLog.LogLevel.Normal);
                    return string.Empty;
                }
            }            
            return System.IO.File.Exists(randImage) ? randImage : string.Empty;
        }

        public static List<DBValue> GetSingleField(string field, SQLCondition conds, DBTable obj)
        {

            string sql = "select " + field + " from " + obj.m_tableName + conds + conds.orderString + conds.limitString;
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

        static char[] splits = new char[] { '|', '\\', '/', ',' };

        /// <summary>
        /// For Genre etc. this method will split by each character in "splits"
        /// Should be used only for fields described in FieldsRequiringSplit
        /// </summary>
        /// <param name="fieldvalue"></param>
        /// <returns></returns>
        public static string[] splitField(string fieldvalue)
        {
            return fieldvalue.Split(splits, StringSplitOptions.RemoveEmptyEntries);
        }

    };
}