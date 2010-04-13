using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SQLite.NET;

namespace WindowPlugins.GUITVSeries.DataBase
{
    internal static class DatabaseHelper
    {
        internal static bool TableExists(string tableName)
        {
            SQLiteResultSet results = DBTVSeries.Execute("SELECT name FROM sqlite_master WHERE name='" + tableName + "'");
            return (results != null && results.Rows.Count > 0);
        }

        internal static void AddColumn(string tableName, DBFieldDef newField)
        {
            DBTVSeries.Execute("ALTER TABLE " + tableName + " ADD " + newField.ColumnDefinition);
        }

        internal static void CreateTable(string tableName, IEnumerable<DBFieldDef> fields)
        {
            //we already know we have at least one
            string columnDefs = string.Empty;
            bool first = true;
            foreach (DBFieldDef field in fields) {
                columnDefs += field.ColumnDefinition;
                if (first) {
                    first = false;
                } else {
                    columnDefs += ", ";
                }
            }

            String sQuery = "CREATE TABLE " + tableName + " (" + columnDefs + ")";
            DBTVSeries.Execute(sQuery);
        }

		internal static void CreateIndexes(string tableName, IEnumerable<DBFieldDef> fields)
        {
			IEnumerable<string> indexes = from fieldDef in fields
								where fieldDef.Indexed && !fieldDef.Primary
								select fieldDef.FieldName;

			foreach (string index in indexes) {
				DBTVSeries.Execute(string.Format("create index if not exists {0}__{1} ON {0}({1} ASC)", tableName, index));
			}
        }

		internal static void DeleteAllIndexes(string tableName)
		{
			//select all the indexs for the table where sql is not null, so that we don't select the primary keys
			SQLiteResultSet results = DBTVSeries.Execute(string.Format("SELECT name FROM sqlite_master where type = 'index' and tbl_name = '{0} and sql is not null", tableName));
			foreach (SQLiteResultSet.Row row in results.Rows) {
				DBTVSeries.Execute("drop index if exists " + row.fields[0]);
			}
		}
		
		internal static bool CreateAutoIDKey(string tableName)
        {
            SQLiteResultSet results = DBTVSeries.Execute("SELECT sql FROM sqlite_master WHERE name='" + tableName + "'");
            if (results == null || results.Rows.Count == 0) {
                //table dosen't exist - will be created later
                return true;
            }

            // we have the table definition
            Regex Engine = new Regex(@"CREATE TABLE .*?\((.*?)\)", RegexOptions.IgnoreCase);
            Match tablematch = Engine.Match(results.Rows[0].fields[0]);
            if (!tablematch.Success) {
                MPTVSeriesLog.Write("ID Key creation in " + tableName + " TABLE has failed!");
                return false;
            }
            String sColumns = tablematch.Groups[1].Value;

            if (Regex.IsMatch(sColumns, @"[(,\s]Id", RegexOptions.IgnoreCase)) {
                MPTVSeriesLog.Write("ID column already exists in " + tableName + "!");
                return false;
            }

            //find the old primary key
            Engine = new Regex(@"([^\s]+)\s+([^\s]+)\s+primary key", RegexOptions.IgnoreCase);
            Match oldKeyMatch = Engine.Match(sColumns);
            String oldKey = null;
            if (oldKeyMatch.Success) {
                oldKey = oldKeyMatch.Groups[1].Value;
            }

            //remove the current primary key definition
            sColumns = Regex.Replace(sColumns, " primary key", String.Empty, RegexOptions.IgnoreCase);

            string newTableName = tableName + "_new";
            //create the new table
            String createQuery = String.Format("CREATE TABLE {0} (Id INTEGER PRIMARY KEY, {1})", newTableName, sColumns);
            DBTVSeries.Execute(createQuery);

            //now we need a string only containing all the column names
            Engine = new Regex(@"([^\s]+)\s+([^,]+)?,", RegexOptions.IgnoreCase);
            MatchCollection matches = Engine.Matches(sColumns + ',');
            if (matches.Count == 0) {
                MPTVSeriesLog.Write("ID Key creation in " + tableName + " TABLE has failed!");
                return false;
            }

            sColumns = String.Empty;
            foreach (Match match in matches) {
                sColumns = sColumns + match.Groups[1] + ", ";
            }
            sColumns = sColumns.Substring(0, sColumns.Length - 2); // remove trailing comma

            //copy values from the old table to the new
            String insertSql = String.Format("insert into {0} ({1}) select {1} from {2}", newTableName, sColumns, tableName);
            DBTVSeries.Execute(insertSql);

            //delete the old table
            DBTVSeries.Execute("Drop Table " + tableName);

            //rename the current table
            DBTVSeries.Execute(String.Format("alter table {0} rename to {1}", newTableName, tableName));
            
            return true;
        }
    }
}
