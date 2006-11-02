using System;
using System.Collections.Generic;
using System.Text;
using SQLite.NET;
using MediaPortal.Database;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace WindowPlugins.GUITVSeries
{
    public class DBValue
    {
        private String value;

        public override String ToString()
        {
            return value;
        }

        public DBValue(String value)
        {
            this.value = value;
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
            return value.value;
        }

        static public implicit operator Boolean(DBValue value)
        {
            if (value.value != "0" && value.value != "")
                return true;
            else
                return false;
        }


        static public implicit operator int(DBValue value)
        {
            try { return Convert.ToInt32(value.value); }
            catch (System.FormatException) { return 0; }
        }

        static public implicit operator long(DBValue value)
        {
            try { return Convert.ToInt64(value.value); }
            catch (System.FormatException) { return 0; }
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
    };

    // field class - used to hold information
    public class DBField
    {
        public const String cTypeInt = "integer";
        public const String cTypeString = "text";

        // private access
        private String m_type;
        private bool m_primaryKey;
        private DBValue m_value;

        public DBField(String type)
        {
            m_type = type;
            m_primaryKey = false;
        }
        public DBField(String type, bool primaryKey)
        {
            m_type = type;
            m_primaryKey = primaryKey;
        }

        public bool Primary
        {
            get { return this.m_primaryKey; }
            set { this.m_primaryKey = value; }
        }

        public String Type
        {
            get { return this.m_type; }
            set { this.m_type = value; }
        }

        public DBValue Value
        {
            // save DB friendly string (doubling singlequotes
            get { return this.m_value; }
            set { this.m_value = value; }
        }
    };


    // table class - used as a base for table objects (series, episodes, etc)
    // holds a field hash table, includes an update mechanism to keep the DB tables in sync
    public class DBTable
    {
        public string m_tableName;
        public Dictionary<string, DBField> m_fields;
        public bool m_CommitNeeded = false;

        public DBTable(string tableName)
        {
            m_tableName = tableName;
            m_fields = new Dictionary<string, DBField>();
            // load up fields from the table
            SQLiteResultSet results;
            results = DBTVSeries.Execute("SELECT sql FROM sqlite_master WHERE name='" + m_tableName + "'");
            if (results != null && results.Rows.Count > 0) 
            {
                // we have the table definition, parse it for names/types
                String sCreateTable = results.Rows[0].fields[0];
                String RegExp = @"CREATE TABLE .*?\((.*?)\)";
                Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
                Match tablematch = Engine.Match(sCreateTable);
                if (tablematch.Success)
                {
                    String sParameters = tablematch.Groups[1].Value;
                    // we have the list of parameters, parse them
                    RegExp = @"([^\s,]+) ([^\s,]+)( primary key)?";
                    Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
                    MatchCollection matches = Engine.Matches(sParameters);
                    foreach (Match parammatch in matches) 
                    {
                        String sName = parammatch.Groups[1].Value;
                        String sType = parammatch.Groups[2].Value;
                        bool bPrimary = false;
                        if (parammatch.Groups[3].Success)
                            bPrimary = true;
                        if (!m_fields.ContainsKey(sName)) {
                            m_fields.Add(sName, new DBField(sType, bPrimary));
                        }
                    }
                }
                else 
                {
                    DBTVSeries.Log("parsing of CREATE TABLE failed!!!");
                }


            }
            else {
                // no tables, assume it's going to be created later (using AddColumn)
            }
        }

        public bool AddColumn(String sName, DBField field)
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
                    SQLiteResultSet results;
                    results = DBTVSeries.Execute("SELECT name FROM sqlite_master WHERE name='" + m_tableName + "'");
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
                        String sQuery = "CREATE TABLE " + m_tableName + " (" + sName + " " + field.Type + (field.Primary ? " primary key)" : ")");
                        DBTVSeries.Execute(sQuery);
                    }
                    m_fields.Add(sName, field);
                    return true;
                }
                catch (Exception ex)
                {
                    DBTVSeries.Log(m_tableName + " table.AddColumn failed (" + ex.Message + ").");
                    return false;
                }
            }
            return false;
        }

        public virtual DBValue this[String fieldName]
        {
            get 
            {
                if (m_fields.ContainsKey(fieldName))
                    return m_fields[fieldName].Value;
                else
                    return null;
            }
            set
            {
                try
                {
                    if (m_fields.ContainsKey(fieldName))
                    {
                        if (m_fields[fieldName].Type == DBField.cTypeInt)
                            m_fields[fieldName].Value = (long)value;
                        else
                            m_fields[fieldName].Value = value;
                        m_CommitNeeded = true;
                    }
                }
                catch (SystemException)
                {
                    DBTVSeries.Log("Cast exception when trying to assign " + value + " to field " + fieldName + " in table " + m_tableName);
                }
            }
        }

        public static String Q(String sField)
        {
            return sField;
        }

        public bool Read(ref SQLiteResultSet records, int index)
        {
            if (records.Rows.Count > 0)
            {
                foreach (KeyValuePair<string, DBField> field in m_fields)
                {
                    if (records.ColumnIndices.ContainsKey(field.Key))
                        field.Value.Value = DatabaseUtility.Get(records, index, field.Key);
                    else
                        field.Value.Value = DatabaseUtility.Get(records, index, m_tableName + "." + field.Key);
                }
                return true;
            }
            return false;
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
                SQLCondition condition = new SQLCondition(this);
                condition.Add(PrimaryKey(), m_fields[PrimaryKey()].Value, true);
                String sqlQuery = "select * from " + m_tableName + " where " + condition;
                SQLiteResultSet records = DBTVSeries.Execute(sqlQuery);
                return Read(ref records, 0);
            }
            catch (Exception ex)
            {
                DBTVSeries.Log("An Error Occurred (" + ex.Message + ").");
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

                if (PrimaryField.Value.Value == null || PrimaryField.Value.Value == String.Empty)
                    return false;

                String sWhere = " where ";
                switch (PrimaryField.Value.Type)
                {
                    case DBField.cTypeInt:
                        sWhere += PrimaryField.Key + " = " + PrimaryField.Value.Value;
                        break;

                    case DBField.cTypeString:
                        sWhere += PrimaryField.Key + " = '" + ((String)PrimaryField.Value.Value).Replace("'", "''") + "'";
                        break;
                }

                // use the primary key field
                String sqlQuery = "select " + PrimaryField.Key + " from " + m_tableName + sWhere;
                SQLiteResultSet records = DBTVSeries.Execute(sqlQuery);
                if (records.Rows.Count > 0)
                {
                    // already exists, update
                    sqlQuery = "update " + m_tableName + " set ";
                    foreach (KeyValuePair<string, DBField> fieldPair in m_fields)
                    {
                        if (!fieldPair.Value.Primary)
                        {
                            switch (fieldPair.Value.Type)
                            {
                                case DBField.cTypeInt:
                                    sqlQuery += fieldPair.Key + " = " + fieldPair.Value.Value + ",";
                                    break;

                                case DBField.cTypeString:
                                    sqlQuery += fieldPair.Key + " = '" + ((String)(fieldPair.Value.Value)).Replace("'", "''") + "',";
                                    break;
                            }
                            
                        }
                    }

                    sqlQuery = sqlQuery.Substring(0, sqlQuery.Length - 1) + sWhere;
                    DBTVSeries.Execute(sqlQuery);
                }
                else
                {
                    // add new record
                    String sParamNames = String.Empty;
                    String sParamValues = String.Empty;
                    foreach (KeyValuePair<string, DBField> fieldPair in m_fields)
                    {
                        sParamNames += fieldPair.Key + ",";
                        switch (fieldPair.Value.Type)
                        {
                            case DBField.cTypeInt:
                                sParamValues += fieldPair.Value.Value + ",";
                                break;

                            case DBField.cTypeString:
                                sParamValues += "'" + ((String)(fieldPair.Value.Value)).Replace("'", "''") + "',";
                                break;
                        }
                    }
                    sParamNames = sParamNames.Substring(0, sParamNames.Length - 1);
                    sParamValues = sParamValues.Substring(0, sParamValues.Length - 1);
                    sqlQuery = "insert into " + m_tableName + " (" + sParamNames + ") values(" + sParamValues + ")";
                    DBTVSeries.Execute(sqlQuery);
                }

                return true;
            }
            catch (Exception ex)
            {
                DBTVSeries.Log("An Error Occurred (" + ex.Message + ").");
                return false;
            }
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
            if (m_sFrom != String.Empty)
                m_sFrom += ", ";
            m_sFrom += table.m_tableName;
        }

        public void AddWhat(DBTable table)
        {
            foreach (KeyValuePair<string, DBField> field in table.m_fields)
            {
                if (m_sWhat == String.Empty)
                    m_sWhat += table.m_tableName + "." + field.Key;
                else
                    m_sWhat += ", " + table.m_tableName + "." + field.Key;
            }

        }

        public void Add(String sField)
        {
            if (m_sWhat != String.Empty)
                m_sWhat += ", ";
            m_sWhat += sField;
        }

        public static implicit operator String(SQLWhat what)
        {
            return what.m_sWhat + " from " + what.m_sFrom;
        }
    };

    public class SQLCondition
    {
        private DBTable m_table = null;
        private String m_sConditions = String.Empty;
        
        public SQLCondition(DBTable table)
        {
            m_table = table;
        }

        public void Add(String sField, DBValue value, bool bEqual)
        {
            if (m_sConditions != String.Empty)
                m_sConditions += " and ";

            // filter with available fields only
            if (m_table.m_fields.ContainsKey(sField))
            {
                switch (m_table.m_fields[sField].Type)
                {
                    case DBField.cTypeInt:
                        m_sConditions += m_table.m_tableName+ "." + sField + (bEqual?" = ":" != ") + value;
                        break;

                    case DBField.cTypeString:
                        m_sConditions += m_table.m_tableName + "." + sField + (bEqual ? " = '" : " != '") + ((String)value).Replace("'", "''") + "'";
                        break;
                }
            }
        }

        public static implicit operator String(SQLCondition conditions)
        {
            return conditions.m_sConditions;
        }
    };

    // DB class - static, one instance only. 
    // holds the SQLLite object and the log object.
    public class DBTVSeries
    {
        #region private & init stuff
        private static SQLiteClient m_db = null;
        private static LogWriter m_log = new LogWriter();

        private static void InitDB()
        {
            String databaseFile;

            databaseFile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
#if TEST
            databaseFile += @"\TVSeriesDatabase4.db3";
#else
            databaseFile = databaseFile.Remove(databaseFile.LastIndexOf('\\')); // Get out of Windows folder
            databaseFile = databaseFile.Remove(databaseFile.LastIndexOf('\\')); // Get out of plugin folder
            databaseFile += @"\Database\TVSeriesDatabase4.db3";
#endif

            try
            {
                m_db = new SQLiteClient(databaseFile);

                m_db.Execute("PRAGMA cache_size=2000;\n");
                m_db.Execute("PRAGMA synchronous='OFF';\n");
                m_db.Execute("PRAGMA count_changes=1;\n");
                m_db.Execute("PRAGMA full_column_names=0;\n");
                m_db.Execute("PRAGMA short_column_names=0;\n");

                m_log.Write("Sucessfully opened database '" + databaseFile + "'.");
            }
            catch (Exception ex)
            {
                m_log.Write("Failed to open database '" + databaseFile + "' (" + ex.Message + ").");
            }
        }
        #endregion

        public static void AttachLog(ref System.Windows.Forms.ListBox listBox)
        {
            m_log.AddNotifier(ref listBox);
        }
        public static void Log(String sLog)
        {
            m_log.Write(sLog);
        }

        public static SQLiteResultSet Execute(String sCommand)
        {
            if (m_db == null)
            {
                InitDB();
            }

            try
            {
                return m_db.Execute(sCommand);
            }
            catch (Exception ex)
            {
                m_log.Write("Commit failed on this command: <" + sCommand + "> (" + ex.Message + ").");
                return new SQLiteResultSet();
            }
        }
    };
}
