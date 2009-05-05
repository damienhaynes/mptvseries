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
//using SQLite.NET;
using System.Data;
using System.Data.Common;
//using MediaPortal.Database;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.IO;

namespace WindowPlugins.GUITVSeries
{
    public class DBValue
    {
        static System.Globalization.NumberFormatInfo provider = new System.Globalization.NumberFormatInfo();
        static DBValue()
        {
            provider.NumberDecimalSeparator = ".";
        }
        private String value = String.Empty;

        public override String ToString()
        {
            return value;
        }

        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            return this == obj as DBValue;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            return base.GetHashCode();
        }

        public DBValue(String value)
        {
            if (value != null)
                this.value = value;
            else
                this.value = String.Empty;
        }
        public DBValue(Boolean value)
        {
            // booleans are 0/1
            this.value = Convert.ToInt16(value).ToString();
        }

        public DBValue(int value)
        {
            this.value = value.ToString();
        }
        public DBValue(long value)
        {
            this.value = value.ToString();
        }

        static public implicit operator String(DBValue value)
        {
            if (value == null)
                return "";

            return value.value;
        }

        static public implicit operator Boolean(DBValue value)
        {
            if (value == null)
                return false;

            if (value.value.Length > 0 && value.value != "0")
                return true;
            else
                return false;
        }

        static public implicit operator int(DBValue value)
        {
            if (null == value)
                return 0;
            try {
                return Convert.ToInt32(value.value);
            } catch (System.FormatException) {
                return 0;
            }
        }

        static public implicit operator long(DBValue value)
        {
            if (null == value)
                return 0;
            try {
                return Convert.ToInt64(value.value);
            } catch (System.FormatException) {
                return 0;
            }
        }

        /// <summary>
        /// NumberDecimalSeperator needs to be "."
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static public implicit operator double(DBValue value)
        {
            if (null == value)
                return 0;
            try {
                return Convert.ToDouble(value.value, provider);
            } catch (System.FormatException) {
                return 0;
            }
        }

        static public implicit operator DBValue(String value)
        {
            return new DBValue(value);
        }

        static public implicit operator DBValue(Boolean value)
        {
            return new DBValue(value);
        }

        static public implicit operator DBValue(int value)
        {
            return new DBValue(value);
        }

        static public implicit operator DBValue(long value)
        {
            return new DBValue(value);
        }

        static public implicit operator DBValue(double value)
        {
            return new DBValue(value.ToString(provider));
        }

        static public bool operator ==(DBValue first, DBValue second)
        {
            if ((object)first == null || (object)second == null) {
                if ((object)first == null && (object)second == null)
                    return true;
                else
                    return false;
            }
            return first.value == second.value;
        }

        static public bool operator !=(DBValue first, DBValue second)
        {
            if ((object)first == null || (object)second == null) {
                if ((object)first == null && (object)second == null)
                    return false;
                else
                    return true;
            }
            return first.value != second.value;
        }

    };

    // field class - used to hold information
    public class DBField
    {
        public enum cType
        {
            Int,
            String
        }

        // the following are remainders to easily change the type (because it was a string! comparision before)
        public const cType cTypeInt = cType.Int;
        public const cType cTypeString = cType.String;

        // private access
        private cType m_type;
        private bool m_primaryKey;
        private DBValue m_value;

        //the maximum lenght for string fields (only use in making database columns)
        // - 1024 is a reasonable default - primary keys can't be any larger
        private int m_maxLength = 1024;

        //use this lenght only when you really need the absolute max field length
        public const int cMaxLength = -1;

        private bool wasChanged = false;

        public DBField(cType type)
        {
            m_type = type;
            m_primaryKey = false;
        }
        public DBField(cType type, bool primaryKey)
        {
            m_type = type;
            m_primaryKey = primaryKey;
        }
        public DBField(cType type, bool primaryKey, int maxLength)
        {
            m_type = type;
            m_primaryKey = primaryKey;
            m_maxLength = maxLength;
        }
        public DBField(cType type, int maxLength)
        {
            m_type = type;
            m_maxLength = maxLength;
        }

        public DBField(DBFieldType dbFieldT)
        {
            m_type = dbFieldT.Type;
            m_primaryKey = dbFieldT.Primary;
            m_maxLength = dbFieldT.MaxLength;
        }

        public bool Primary
        {
            get
            {
                return this.m_primaryKey;
            }
            set
            {
                this.m_primaryKey = value;
            }
        }

        public cType Type
        {
            get
            {
                return this.m_type;
            }
            set
            {
                this.m_type = value;
            }
        }

        public DBValue Value
        {
            // save DB friendly string (doubling singlequotes
            get
            {
                return this.m_value;
            }
            set
            {
                this.m_value = value;
            }
        }

        public int MaxLength
        {
            get 
            { 
                return this.m_maxLength;
            }
            set
            {
                this.m_maxLength = value;
            }
        }

        public bool WasChanged
        {
            get
            {
                return wasChanged;
            }
            set
            {
                wasChanged = value;
            }
        }
    };

    public struct DBFieldType
    {
        public DBField.cType Type;
        public bool Primary;
        public int MaxLength;
    }

    // table class - used as a base for table objects (series, episodes, etc)
    // holds a field hash table, includes an update mechanism to keep the DB tables in sync
    public class DBTable
    {
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

            } else {// we have to get it, happens when the first object is created or after an alter table
                cachedForTable = new Dictionary<string, DBFieldType>();
                // load up fields from the table

                DataTable Columns = DBTVSeries.GetTableColumns(m_tableName);
                if (Columns != null) {
                    foreach (DataColumn Column in Columns.Columns) {
                        String sName = Column.ColumnName;
                        String sType = Column.DataType.Name;
                        bool bPrimary = new List<DataColumn>(Columns.PrimaryKey).Contains(Column);

                        DBFieldType cachedInfo = new DBFieldType();
                        cachedInfo.Primary = bPrimary;
                        cachedInfo.Type = sType == "Int32" ? DBField.cTypeInt : DBField.cType.String;
                        cachedInfo.MaxLength = Column.MaxLength;
                        
                        if (!m_fields.ContainsKey(sName)) {
                            m_fields.Add(sName, new DBField(cachedInfo));
                        }

                        cachedForTable.Add(sName, cachedInfo);
                    }

                    lock (fields) {
                        fields.Add(tableName, cachedForTable);
                    }
                } else {
                    // no tables, assume it's going to be created later (using AddColumn)
                }
            }
        }

        public virtual bool AddColumn(String sName, DBField field)
        {
            // verify if we already have that field avail
            if (!m_fields.ContainsKey(sName)) {
                if (m_fields.Count == 0 && !field.Primary) {
                    throw new Exception("First field added needs to be the index");
                }

                try {
                    DBTVSeries.AddColumn(m_tableName, sName, field);

                    // delete the s_fields cache so newed up objects get the right fields
                    lock (fields)
                        fields.Remove(m_tableName);
                    m_fields.Add(sName, field);
                    return true;
                } catch (Exception ex) {
                    MPTVSeriesLog.Write(m_tableName + " table.AddColumn failed (" + ex.Message + ").");
                    return false;
                }
            }
            return false;
        }

        public virtual void InitValues()
        {
            foreach (KeyValuePair<string, DBField> fieldPair in m_fields) {
                if (!fieldPair.Value.Primary || fieldPair.Value.Value == null) {
                    switch (fieldPair.Value.Type) {
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
            foreach (KeyValuePair<string, DBField> fieldPair in m_fields) {
                if (!fieldPair.Value.Primary || fieldPair.Value.Value == null) {
                    switch (fieldPair.Value.Type) {
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
                if (!m_fields.TryGetValue(fieldName, out result))
                    return string.Empty;
                return result.Value;

            }
            set
            {
                try {
                    DBField result;
                    if (m_fields.TryGetValue(fieldName, out result)) {
                        if (result.Value != value) {
                            if (result.Type == DBField.cTypeInt)
                                result.Value = (long)value;
                            else
                                result.Value = value;
                            result.WasChanged = true;
                            m_CommitNeeded = true;
                        }
                    } else {
                        AddColumn(fieldName, new DBField(DBField.cTypeString));
                        this[fieldName] = value;
                    }
                } catch (SystemException) {
                    MPTVSeriesLog.Write("Cast exception when trying to assign " + value + " to field " + fieldName + " in table " + m_tableName);
                }
            }
        }

        public virtual ICollection<String> FieldNames
        {
            get
            {
                List<String> outList = new List<String>();
                foreach (KeyValuePair<string, DBField> pair in m_fields) {
                    outList.Add(pair.Key);
                }
                return outList;
            }
        }

        public static String Q(String sField)
        {
            return sField;
        }

        public bool Read(DataTable records, int index)
        {
            if (records.Rows.Count > 0 || records.Rows.Count < index) {
                DataRow row = records.Rows[index];
                return Read(row);
            }
            return false;
        }

        public bool Read(DataRow row)
        {
            if (row == null || row.ItemArray.Length == 0)
                return false;
            DataColumnCollection columns = row.Table.Columns;
            string fullName = string.Empty;
            foreach (KeyValuePair<string, DBField> field in m_fields) {
                if (columns.Contains(field.Key)) {
                    field.Value.Value = row[field.Key].ToString();
                    continue;
                }
                fullName = m_tableName + "_" + field.Key;
                if (columns.Contains(fullName)) {
                    field.Value.Value = row[fullName].ToString();
                    continue;
                }
                fullName = m_tableName + "." + field.Key;
                if (columns.Contains(fullName)) {
                    field.Value.Value = row[fullName].ToString();
                    continue;
                }
                field.Value.Value = string.Empty;
            }
            m_CommitNeeded = false;
            return true;
        }

        public String PrimaryKey()
        {
            foreach (KeyValuePair<string, DBField> field in m_fields)
                if (field.Value.Primary == true) {
                    return field.Key;
                }

            return null;
        }

        public bool ReadPrimary(DBValue Value)
        {
            try {
                m_fields[PrimaryKey()].Value = Value;
                SQLCondition condition = new SQLCondition();
                condition.Add(this, PrimaryKey(), m_fields[PrimaryKey()].Value, SQLConditionType.Equal);
                String sqlQuery = "select * from " + m_tableName + condition;
                DataTable records = DBTVSeries.Execute(sqlQuery);
                return Read(records, 0);
            } catch (Exception ex) {
                MPTVSeriesLog.Write("An Error Occurred (" + ex.Message + ").");
            }
            return false;
        }

        public virtual bool Commit()
        {
            try {
                if (!m_CommitNeeded)
                    return false;

                KeyValuePair<string, DBField> PrimaryField = new KeyValuePair<string, DBField>();

                foreach (KeyValuePair<string, DBField> field in m_fields)
                    if (field.Value.Primary == true) {
                        PrimaryField = field;
                        break;
                    }

                if (Helper.String.IsNullOrEmpty(PrimaryField.Value.Value))
                    return false;

                String sWhere = " where ";
                switch (PrimaryField.Value.Type) {
                    case DBField.cTypeInt:
                        sWhere += PrimaryField.Key + " = " + PrimaryField.Value.Value;
                        break;

                    case DBField.cTypeString:
                        sWhere += PrimaryField.Key + " = '" + ((String)PrimaryField.Value.Value).Replace("'", "''") + "'";
                        break;
                }

                // use the primary key field
                String sqlQuery = "select " + PrimaryField.Key + " from " + m_tableName + sWhere;
                DataTable records = DBTVSeries.Execute(sqlQuery);
                StringBuilder builder = new StringBuilder();
                if (records.Rows.Count > 0) {
                    // already exists, update
                    builder.Append("update ").Append(m_tableName).Append(" set ");
                    int fieldsNeedingUpdating = 0;
                    foreach (KeyValuePair<string, DBField> fieldPair in m_fields) {
                        if (!fieldPair.Value.Primary && fieldPair.Value.WasChanged) {
                            //use of [] around names allows for keywords to be used as column names (ie. With)
                            builder.Append("[").Append(fieldPair.Key).Append("] = ");
                            switch (fieldPair.Value.Type) {
                                case DBField.cTypeInt:
                                    if (Helper.String.IsNullOrEmpty(fieldPair.Value.Value))
                                        builder.Append("'',");
                                    else
                                        builder.Append((string)fieldPair.Value.Value).Append(',');
                                    break;

                                case DBField.cTypeString:
                                    builder.Append(" '").Append(((String)(fieldPair.Value.Value)).Replace("'", "''")).Append("',");
                                    break;
                            }
                            fieldsNeedingUpdating++;
                            fieldPair.Value.WasChanged = false;
                        }
                    }
                    if (fieldsNeedingUpdating > 0) {
                        sqlQuery = builder.ToString().Substring(0, builder.Length - 1) + sWhere;
                        DBTVSeries.Execute(sqlQuery);
                    }
                } else {
                    // add new record
                    String sParamValues = String.Empty;
                    StringBuilder paramNames = new StringBuilder();
                    bool first = true;
                    foreach (KeyValuePair<string, DBField> fieldPair in m_fields) {
                        if (!first) {
                            paramNames.Append(',');
                            builder.Append(',');
                        } else
                            first = false;
                        //use of [] around names allows for keywords to be used as column names (ie. With)
                        paramNames.Append("[").Append(fieldPair.Key).Append("]");
                        switch (fieldPair.Value.Type) {
                            case DBField.cTypeInt:
                                if (Helper.String.IsNullOrEmpty(fieldPair.Value.Value))
                                    builder.Append("''");
                                else
                                    builder.Append((string)fieldPair.Value.Value);
                                break;

                            case DBField.cTypeString:
                                builder.Append(" '").Append(((String)(fieldPair.Value.Value)).Replace("'", "''")).Append("'");
                                break;
                        }

                    }
                    sParamValues = builder.ToString();
                    builder.Remove(0, builder.Length);
                    builder.Append("insert into ").Append(m_tableName).Append(" (").Append(paramNames).Append(") values(").Append(sParamValues).Append(")");
                    sqlQuery = builder.ToString();

                    DBTVSeries.Execute(sqlQuery);
                }
                return true;
            } catch (Exception ex) {
                MPTVSeriesLog.Write("An Error Occurred (" + ex.Message + ").");
                return false;
            }
        }

        public static void GlobalSet(DBTable obj, String sKey, DBValue Value, SQLCondition conditions)
        {
            if (obj.m_fields.ContainsKey(sKey)) {
                String sqlQuery = "update " + obj.m_tableName + " SET " + sKey + "=";
                switch (obj.m_fields[sKey].Type) {
                    case DBField.cTypeInt:
                        sqlQuery += Value;
                        break;

                    case DBField.cTypeString:
                        sqlQuery += "'" + Value + "'";
                        break;
                }

                sqlQuery += conditions;
                DBTVSeries.Execute(sqlQuery);
                if (dbUpdateOccured != null)
                    dbUpdateOccured(obj.m_tableName);
            }
        }

        public static void GlobalSet(DBTable obj, String sKey1, String sKey2, SQLCondition conditions)
        {
            if (obj.m_fields.ContainsKey(sKey1) && obj.m_fields.ContainsKey(sKey2)) {
                String sqlQuery = "update " + obj.m_tableName + " SET " + sKey1 + " = " + sKey2 + conditions;
                DBTVSeries.Execute(sqlQuery);
                if (dbUpdateOccured != null)
                    dbUpdateOccured(obj.m_tableName);
            }
        }

        public static void Clear(DBTable obj, SQLCondition conditions)
        {
            String sqlQuery = "delete from " + obj.m_tableName + conditions;
            DBTVSeries.Execute(sqlQuery);
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
            if (BannerList == null || BannerList.Count == 0)
                return string.Empty;
            if (BannerList.Count == 1)
                randImage = BannerList[0];

            if (randImage == null) {
                List<string> langG = new List<string>();
                List<string> lang = new List<string>();
                List<string> engG = new List<string>();
                List<string> eng = new List<string>();
                for (int i = 0; i < BannerList.Count; i++) {
                    if (File.Exists(BannerList[i])) {
                        if (BannerList[i].Contains(graphicalBannerRecognizerSubstring)) {
                            if (BannerList[i].Contains(langIdentifier))
                                langG.Add(BannerList[i]);
                            else
                                engG.Add(BannerList[i]);
                        } else {
                            if (BannerList[i].Contains(langIdentifier))
                                lang.Add(BannerList[i]);
                            else
                                eng.Add(BannerList[i]);
                        }
                    }
                }

                try {
                    if (langG.Count > 0)
                        randImage = langG[new Random().Next(0, langG.Count)];
                    else if (lang.Count > 0)
                        randImage = lang[new Random().Next(0, lang.Count)];
                    else if (engG.Count > 0)
                        randImage = engG[new Random().Next(0, engG.Count)];
                    else if (eng.Count > 0)
                        randImage = eng[new Random().Next(0, eng.Count)];
                    else
                        return string.Empty;
                } catch {
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
            try {
                foreach (DataRow result in DBTVSeries.Execute(sql).Rows) {
                    results.Add(result[0].ToString());
                }
            } catch (Exception ex) {
                MPTVSeriesLog.Write("GetSingleField SQL method generated an error: " + ex.Message);
            }
            return results;
        }

        static char[] splits = new char[] { '|', '\\', '/', ',' };

        /// <summary>
        /// For Genre etc. this method will split by each character in "splits"
        /// Should be used only for fields descriped in FieldsRequiringSplit
        /// </summary>
        /// <param name="fieldvalue"></param>
        /// <returns></returns>
        public static string[] splitField(string fieldvalue)
        {
            return fieldvalue.Split(splits, StringSplitOptions.RemoveEmptyEntries);
        }

    };

    public class SQLWhat
    {
        private String m_sWhat = String.Empty;
        private String m_sFrom = String.Empty;
        public SQLWhat()
        {

        }

        public SQLWhat(DBTable table)
        {
            Add(table);
        }

        public void Add(DBTable table)
        {
            AddWhat(table);
            if (m_sFrom.Length > 0)
                m_sFrom += ", ";
            m_sFrom += table.m_tableName;
        }

        public void AddWhat(DBTable table)
        {
            foreach (KeyValuePair<string, DBField> field in table.m_fields) {
                if (Helper.String.IsNullOrEmpty(m_sWhat))
                    m_sWhat += table.m_tableName + "." + field.Key + " as " + table.m_tableName + "_" + field.Key;
                else
                    m_sWhat += ", " + table.m_tableName + "." + field.Key + " as " + table.m_tableName + "_" + field.Key;
            }

        }

        public void Add(String sField)
        {
            if (m_sWhat.Length > 0)
                m_sWhat += ", ";
            m_sWhat += sField;
        }

        public static implicit operator String(SQLWhat what)
        {
            return what.m_sWhat + " from " + what.m_sFrom;
        }
    };

    public enum SQLConditionType
    {
        Equal,
        NotEqual,
        LessThan,
        LessEqualThan,
        GreaterThan,
        GreaterEqualThan,
        Like,
        In,
        NotIn,
    };

    public class SQLCondition
    {
        private String m_sConditions = String.Empty;
        private String m_sLimit = String.Empty;
        private String m_sOrderstring = String.Empty;
        bool _beginGroup = false;
        public void beginGroup()
        {
            _beginGroup = true;
        }

        public void endGroup()
        {
            m_sConditions += " ) ";
        }

        public bool limitIsSet = false;
        public bool customOrderStringIsSet = false;

        public bool nextIsOr = false;

        // I need this for subqueries
        /// <summary>
        /// Warning: do not set "where", also returns without "where"
        /// </summary>
        public string ConditionsSQLString
        {
            set
            {
                m_sConditions = value;
            }
            get
            {
                return m_sConditions;
            }
        }

        public enum orderType
        {
            Ascending,
            Descending
        };

        public SQLCondition()
        {
        }

        public SQLCondition(DBTable table, String sField, DBValue value, SQLConditionType type)
        {
            Add(table, sField, value, type);
        }

        public void AddSubQuery(string field, DBTable table, SQLCondition innerConditions, DBValue value, SQLConditionType type)
        {
            string sValue = value;
            if (type == SQLConditionType.Like)
                sValue = "'%" + ((String)value).Replace("'", "''") + "%'";
            else
                sValue = ((String)value).Replace("'", "''");

            AddCustom("( select " + field + " from " + table.m_tableName + innerConditions + innerConditions.orderString + innerConditions.limitString + " ) ", sValue, type);
        }

        public void Add(DBTable table, String sField, DBValue value, SQLConditionType type)
        {
            if (table.m_fields.ContainsKey(sField)) {
                String sValue = String.Empty;
                switch (table.m_fields[sField].Type) {
                    case DBField.cTypeInt:
                        sValue = value;
                        break;

                    case DBField.cTypeString:
                        if (type == SQLConditionType.Like)
                            sValue = "'%" + ((String)value).Replace("'", "''") + "%'";
                        else
                            sValue = "'" + ((String)value).Replace("'", "''") + "'";
                        break;
                }
                AddCustom(table.m_tableName + "." + sField, sValue, type);
            }
        }

        public void SetLimit(int limit)
        {
            if (DBTVSeries.bUseLimit) {
                m_sLimit = " limit " + limit.ToString();
                limitIsSet = true;
            }
        }

        public string orderString
        {
            get
            {
                return m_sOrderstring;
            }
        }

        public string limitString
        {
            get
            {
                return m_sLimit;
            }
        }

        public void AddOrderItem(string qualifiedFieldname, orderType type)
        {
            if (m_sOrderstring.Length == 0)
                m_sOrderstring = " order by ";
            else
                m_sOrderstring += " , ";
            m_sOrderstring += qualifiedFieldname;
            m_sOrderstring += type == orderType.Ascending ? " asc " : " desc ";
            customOrderStringIsSet = true;
        }

        public void AddCustom(string what, string value, SQLConditionType type, bool EncloseIfString)
        {
            if (EncloseIfString) {
                if (value.Length > 2 && value[0] != '\'' && !Helper.String.IsNumerical(value))
                    value = "'" + value + "'";
            }
            AddCustom(what, value, type);
        }

        public void AddCustom(string what, string value, SQLConditionType type)
        {

            String sType = String.Empty;
            switch (type) {
                case SQLConditionType.Equal:
                    sType = " = ";
                    break;

                case SQLConditionType.NotEqual:
                    sType = " != ";
                    break;
                case SQLConditionType.LessThan:
                    sType = " < ";
                    break;
                case SQLConditionType.LessEqualThan:
                    sType = " <= ";
                    break;
                case SQLConditionType.GreaterThan:
                    sType = " > ";
                    break;
                case SQLConditionType.GreaterEqualThan:
                    sType = " >= ";
                    break;
                case SQLConditionType.Like:
                    sType = " like ";
                    break;
                case SQLConditionType.In:
                    sType = " in ";
                    break;

                case SQLConditionType.NotIn:
                    sType = " not in ";
                    break;
            }
            if (SQLConditionType.In == type || SQLConditionType.NotIn == type) // reverse
                AddCustom(value + sType + "(" + what + ")");
            else
                AddCustom(what + sType + value);
        }

        public void AddCustom(string SQLString)
        {
            if (m_sConditions.Length > 0 && SQLString.Length > 0) {
                if (nextIsOr)
                    m_sConditions += " or ";
                else
                    m_sConditions += " and ";
            }
            if (_beginGroup) {
                m_sConditions += " ( ";
                _beginGroup = false;
            }
            m_sConditions += SQLString;
        }

        public static implicit operator String(SQLCondition conditions)
        {
            return conditions.m_sConditions.Length > 0 ? " where " + conditions.m_sConditions : conditions.m_sConditions;
        }

        public SQLCondition Copy()
        {
            SQLCondition copy = new SQLCondition();
            copy.customOrderStringIsSet = customOrderStringIsSet;
            copy.limitIsSet = limitIsSet;

            copy.m_sConditions = m_sConditions;
            copy.m_sLimit = m_sLimit;
            copy.m_sOrderstring = m_sOrderstring;
            return copy;
        }

        public override string ToString()
        {
            return this;
        }
    };

    #region DBProviders
    public abstract class DBProvider
    {
        protected String m_sConnectionString = string.Empty;

        public abstract String sProviderName
        {
            get;
        }

        public abstract bool bUseLimit
        {
            get;
        }

        public String sConnectionString
        {
            get
            {
                return m_sConnectionString;
            }
        }

        public abstract void InitDB();

        public abstract void AddColumn(string tableName, string fieldName, DBField field);

        public abstract void CreateTable(string tableName, string fieldName, DBField field);
    }

    public class SQLiteProvider : DBProvider
    {
        public SQLiteProvider(string databaseFile)
        {
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            builder.Add("Data Source", databaseFile);

            m_sConnectionString = builder.ConnectionString;
        }

        override public String sProviderName
        {
            get
            {
                return "System.Data.SQLite";
            }
        }

        public override bool bUseLimit
        {
            get
            {
                return true;
            }
        }

        public override void InitDB()
        {
            DbProviderFactory factory = System.Data.SQLite.SQLiteFactory.Instance;
            using (DbConnection connection = factory.CreateConnection()) {
                connection.ConnectionString = sConnectionString;
                try {
                    connection.Open();
                    using (DbCommand command = connection.CreateCommand()) {
                        command.CommandText = "PRAGMA cache_size=5000;";        // Each page uses about 1.5K of memory
                        command.ExecuteNonQuery();
                    }

                    using (DbCommand command = connection.CreateCommand()) {
                        command.CommandText = "PRAGMA synchronous='OFF';";
                        command.ExecuteNonQuery();
                    }

                    using (DbCommand command = connection.CreateCommand()) {
                        command.CommandText = "PRAGMA count_changes=1;";
                        command.ExecuteNonQuery();
                    }

                    using (DbCommand command = connection.CreateCommand()) {
                        command.CommandText = "PRAGMA full_column_names=0;";
                        command.ExecuteNonQuery();
                    }

                    using (DbCommand command = connection.CreateCommand()) {
                        command.CommandText = "PRAGMA short_column_names=0;";
                        command.ExecuteNonQuery();
                    }

                    using (DbCommand command = connection.CreateCommand()) {
                        command.CommandText = "PRAGMA temp_store = MEMORY;";
                        command.ExecuteNonQuery();
                    }
                } finally {
                    connection.Close();
                }
            }
        }

        public override void AddColumn(string tableName, string fieldName, DBField field)
        {
            string sQuery = "ALTER TABLE " + tableName + " ADD " + fieldName + " " + field.Type;
            DbProviderFactory factory = System.Data.SQLite.SQLiteFactory.Instance;

            using (DbConnection connection = factory.CreateConnection()) {
                connection.ConnectionString = sConnectionString;
                try {
                    connection.Open();
                    using (DbCommand command = connection.CreateCommand()) {
                        command.CommandText = sQuery;
                        command.ExecuteNonQuery();
                    }
                } finally {
                    connection.Close();
                }
            }
        }
    
        public override void  CreateTable(string tableName, string fieldName, DBField field)
        {
            String sQuery = "CREATE TABLE " + tableName + " (" + fieldName + " " + field.Type + (field.Primary ? " primary key)" : ")");
            DbProviderFactory factory = System.Data.SQLite.SQLiteFactory.Instance;

            using (DbConnection connection = factory.CreateConnection()) {
                connection.ConnectionString = sConnectionString;
                try {
                    connection.Open();
                    using (DbCommand command = connection.CreateCommand()) {
                        command.CommandText = sQuery;
                        command.ExecuteNonQuery();
                    }
                } finally {
                    connection.Close();
                }
            }
        }
}

    public class SQLClientProvider : DBProvider
    {
        public SQLClientProvider(string connectionString)
        {
            //DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            //builder.Add("Persist Security Info", true);
            //builder.Add("USER ID", "sa");                       //username
            //builder.Add("Password", "mediaportal");             //password
            //builder.Add("Initial Catalog", "MpTvSeriesDb4");    //database name
            //builder.Add("Data Source", "localhost\SQLEXPRESS");    //computer name
            //builder.Add("Connection Timeout", 300);

            m_sConnectionString = connectionString;
        }

        public override string sProviderName
        {
            get
            {
                return "System.Data.SqlClient";
            }
        }

        public override bool bUseLimit
        {
            get
            {
                return false;
            }
        }

        public static void TestConnection(string ConnectionString)
        {
            DbProviderFactory factory = System.Data.SqlClient.SqlClientFactory.Instance;
            using (DbConnection connection = factory.CreateConnection()) {
                //try and open the database
                try {
                    connection.ConnectionString = ConnectionString;
                    connection.Open();
                } finally {
                    connection.Close();
                }
            }
        }

        public static void CreateDatabase(string ConnectionString)
        {
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            builder.ConnectionString = ConnectionString;

            string database = builder["Initial Catalog"].ToString();

            //were going to make the database, so remove the database from the connectionstring
            builder.Remove("Initial Catalog");

            System.Reflection.Assembly assm = System.Reflection.Assembly.GetExecutingAssembly();
            Stream stream = assm.GetManifestResourceStream("WindowPlugins.GUITVSeries.DB_Classes.create_sqlserver_database.sql");

            string createScript = string.Empty;
            using (StreamReader reader = new StreamReader(stream)) {
                createScript = reader.ReadToEnd();
            }

            createScript = createScript.Replace("%MpTvSeriesDb4%", database);
            createScript = createScript.Replace("GO\r\n", "!");
            createScript = createScript.Replace("\r\n", " ");
            createScript = createScript.Replace("\t", " ");
            string[] Commands = createScript.Split('!');

            DbProviderFactory factory = System.Data.SqlClient.SqlClientFactory.Instance;
            using (DbConnection connection = factory.CreateConnection()) {
                try {
                    connection.ConnectionString = builder.ConnectionString;
                    connection.Open();

                    foreach (string commandText in Commands) {
                        string Sql = commandText.Trim();
                        if (!string.IsNullOrEmpty(Sql) && !Sql.StartsWith("--") && !Sql.StartsWith("/*")) {
                            using (DbCommand command = connection.CreateCommand()) {
                                command.CommandText = commandText;
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                } finally {
                    connection.Close();
                }
            }
        }

        public override void InitDB()
        {
            TestConnection(sConnectionString);
        }

         public override void AddColumn(string tableName, string fieldName, DBField field)
         {
             string type = string.Empty;
             if (field.Type == DBField.cType.String) {
                 if (field.MaxLength <= DBField.cMaxLength) {
                     type = "varchar(max)";
                 } else {
                     type = string.Format("varchar({0})", field.MaxLength);
                 }
             } else {
                 type = "int";
             }

             string sQuery = "ALTER TABLE [" + tableName + "] ADD [" + fieldName + "] " + type;
             DbProviderFactory factory = System.Data.SqlClient.SqlClientFactory.Instance;

             using (DbConnection connection = factory.CreateConnection()) {
                 connection.ConnectionString = sConnectionString;
                 try {
                     connection.Open();
                     using (DbCommand command = connection.CreateCommand()) {
                         command.CommandText = sQuery;
                         command.ExecuteNonQuery();
                     }
                 } finally {
                     connection.Close();
                 }
             }
         }

         public override void CreateTable(string tableName, string fieldName, DBField field)
         {
             string type = string.Empty;
             if (field.Type == DBField.cType.String) {
                 if (field.MaxLength <= DBField.cMaxLength) {
                     type = "varchar(max)";
                 } else {
                     type = string.Format("varchar({0})", field.MaxLength);
                 }
             } else {
                 type = "int";
             }

             String sQuery = "CREATE TABLE [" + tableName + "] ([" + fieldName + "] " + type + (field.Primary ? " primary key)" : ")");
             DbProviderFactory factory = System.Data.SqlClient.SqlClientFactory.Instance;

             using (DbConnection connection = factory.CreateConnection()) {
                 connection.ConnectionString = sConnectionString;
                 try {
                     connection.Open();
                     using (DbCommand command = connection.CreateCommand()) {
                         command.CommandText = sQuery;
                         command.ExecuteNonQuery();
                     }
                 } finally {
                     connection.Close();
                 }
             }
         }
    }
    #endregion

    // DB class - static, one instance only. 
    // holds the SQLLite object and the log object.
    public class DBTVSeries
    {
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

            if (Settings.UseSQLClient) {
                m_DBProvider = new SQLClientProvider(Settings.ConnectionString);
            } else {
                m_DBProvider = new SQLiteProvider(Settings.GetPath(Settings.Path.database));
            }

            try
            {
                m_DBProvider.InitDB();

                MPTVSeriesLog.Write("Successfully opened database '" + m_DBProvider.sConnectionString + "'");
            }
            catch (Exception ex)
            {
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

        /// <summary>
        /// Get a set of values from the database
        /// </summary>
        /// <param name="sCommand"></param>
        /// <returns></returns>
        public static DataTable Execute(String sCommand)
        {
            DataTable result = new DataTable();
            using (DbConnection connection = GetConnection()) {
                try {
                    MPTVSeriesLog.Write("Executing SQL: ", sCommand, MPTVSeriesLog.LogLevel.DebugSQL);
                    connection.Open();
                    using (DbCommand command = connection.CreateCommand()) {
                        command.CommandText = sCommand;
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
           using (DbConnection connection = GetConnection()) {

                try {
                    MPTVSeriesLog.Write("Executing SQL: ", sCommand, MPTVSeriesLog.LogLevel.DebugSQL);
                    connection.Open();
                    Object result = null;
                    using (DbCommand command = connection.CreateCommand()) {
                        command.CommandText = sCommand;
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
    };
}