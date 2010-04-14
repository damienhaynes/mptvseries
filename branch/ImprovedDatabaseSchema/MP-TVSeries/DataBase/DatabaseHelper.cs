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
        internal static string GetTableSQL(string tableName)
        {
			SQLiteResultSet results = DBTVSeries.Execute("SELECT name FROM sqlite_master WHERE name='" + tableName + "'");
			if (results == null || results.Rows.Count == 0)
				return string.Empty;
        	return results.Rows[0].fields[0];
		}

		internal static bool TableExists(string tableName)
        {
			return (string.IsNullOrEmpty(GetTableSQL(tableName)));
        }

        internal static void AddColumn(string tableName, DBFieldDef newField)
        {
            DBTVSeries.Execute("ALTER TABLE " + tableName + " ADD " + newField.ColumnDefinition);
        }

        internal static void CreateTable(string tableName, IEnumerable<DBFieldDef> fields)
        {
			MPTVSeriesLog.Write("Creating Database Table " + tableName);
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
			MPTVSeriesLog.Write("Creating indexes from Database Table " + tableName);
			IEnumerable<string> indexes = from fieldDef in fields
								where fieldDef.Indexed && !fieldDef.Primary
								select fieldDef.FieldName;

			foreach (string index in indexes) {
				DBTVSeries.Execute(string.Format("create index if not exists {0}__{1} ON {0}({1} ASC)", tableName, index));
			}
        }

		internal static void DeleteAllIndexes(string tableName)
		{
			MPTVSeriesLog.Write("Deleting indexes from Database Table " + tableName);
			
			//select all the indexs for the table where sql is not null, so that we don't select the primary keys
			SQLiteResultSet results = DBTVSeries.Execute(string.Format("SELECT name FROM sqlite_master where type = 'index' and tbl_name = '{0} and sql is not null", tableName));
			foreach (SQLiteResultSet.Row row in results.Rows) {
				DBTVSeries.Execute("drop index if exists " + row.fields[0]);
			}
		}
		
		internal static bool CreateAutoIDKey(string tableName)
        {
			string tableSql = GetTableSQL(tableName);
			if (string.IsNullOrEmpty(tableSql)) {
				//table dosen't exist - will be created later
				return true;
			}

			string sColumns = GetColumnsSql(tableSql);
			if (string.IsNullOrEmpty(sColumns))
				return false;

			if (Regex.IsMatch(sColumns, @"[(,\s]Id", RegexOptions.IgnoreCase)) {
                MPTVSeriesLog.Write("ID column already exists in " + tableName + "!");
                return false;
            }

            //remove the current primary key definition
            sColumns = Regex.Replace(sColumns, " primary key", String.Empty, RegexOptions.IgnoreCase);

            string newTableName = tableName + "_new";
            //create the new table
            String createQuery = String.Format("CREATE TABLE {0} (Id INTEGER PRIMARY KEY, {1})", newTableName, sColumns);
            DBTVSeries.Execute(createQuery);

			sColumns = ExtractColumnNames(sColumns);
			if (string.IsNullOrEmpty(sColumns))
				return false;

			//copy values from the old table to the new
            String insertSql = String.Format("insert into {0} ({1}) select {1} from {2}", newTableName, sColumns, tableName);
            DBTVSeries.Execute(insertSql);

            //delete the old table
            DBTVSeries.Execute("Drop Table " + tableName);

            //rename the current table
            DBTVSeries.Execute(String.Format("alter table {0} rename to {1}", newTableName, tableName));
            
            return true;
        }

    	private static string ExtractColumnNames(string sColumns)
    	{
    		//now we need a string only containing all the column names
    		Regex Engine = new Regex(@"([^\s]+)\s+([^,]+)?,", RegexOptions.IgnoreCase);
    		MatchCollection matches = Engine.Matches(sColumns + ',');
    		if (matches.Count == 0) {
    			return null;
    		}

    		sColumns = String.Empty;
    		foreach (Match match in matches) {
    			sColumns = sColumns + match.Groups[1] + ", ";
    		}
    		return sColumns.Substring(0, sColumns.Length - 2); // remove trailing comma
    	}

    	private static string GetColumnsSql(string tableSql)
    	{
    		// we have the table definition
    		Regex Engine = new Regex(@"CREATE TABLE .*?\((.*?)\)", RegexOptions.IgnoreCase);
    		Match tablematch = Engine.Match(tableSql);
    		if (!tablematch.Success) {
    			return null;
    		}
    		return tablematch.Groups[1].Value;
    	}

    	internal static void RenameDatabaseColumn(string tableName, string oldName, string newName)
		{
			string tableSql = GetTableSQL(tableName);

    		string newTableSql = Regex.Replace(tableSql, @"([\(,\s])" + oldName + @"(\s)", "$1" + newName + "$2", RegexOptions.IgnoreCase);

    		string columnsSql = GetColumnsSql(tableSql);

			string newColumnSql = GetColumnsSql(newTableSql);

    		string newTableName = tableName + "_new";

    		DBTVSeries.Execute(string.Format("CREATE TABLE {0} (Id INTEGER PRIMARY KEY, {1})", newTableName, newColumnSql));

    		columnsSql = ExtractColumnNames(columnsSql);
    		newColumnSql = ExtractColumnNames(newColumnSql);

			//copy values from the old table to the new
			String insertSql = String.Format("insert into {0} ({1}) select {2} from {3}", newTableName, newColumnSql, columnsSql, tableName);
			DBTVSeries.Execute(insertSql);

			//delete the old table
			DBTVSeries.Execute("Drop Table " + tableName);

			//rename the current table
			DBTVSeries.Execute(String.Format("alter table {0} rename to {1}", newTableName, tableName));
		}
    }
}
