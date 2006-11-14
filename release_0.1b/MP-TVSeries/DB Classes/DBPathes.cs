using System;
using System.Collections.Generic;
using System.Text;
using SQLite.NET;
using MediaPortal.Database;

namespace WindowPlugins.GUITVSeries
{
    public class DBImportPath : DBTable
    {
        public const String cTableName = "ImportPathes";

        public const String cIndex = "ID";
        public const String cEnabled = "enabled";
        public const String cPath = "Path";

        public DBImportPath()
            : base(cTableName)
        {
            InitColumns();
            InitValues();
        }

        public DBImportPath(long ID)
            : base(cTableName)
        {
            InitColumns();
            if (!ReadPrimary(ID.ToString()))
                InitValues();
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
            AddColumn(cIndex, new DBField(DBField.cTypeInt, true));
            AddColumn(cEnabled, new DBField(DBField.cTypeInt));
            AddColumn(cPath, new DBField(DBField.cTypeString));
        }
        public static void ClearAll()
        {
            String sqlQuery = "delete from " + cTableName;
            DBTVSeries.Execute(sqlQuery);
        }

        public static DBImportPath[] GetAll()
        {
            try
            {
                // make sure the table is created - create a dummy object
                DBImportPath dummy = new DBImportPath();

                // retrieve all fields in the table
                String sqlQuery = "select * from " + cTableName + " order by " + cIndex;
                SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
                if (results.Rows.Count > 0)
                {
                    DBImportPath[] importPathes = new DBImportPath[results.Rows.Count];
                    for (int index = 0; index < results.Rows.Count; index++)
                    {
                        importPathes[index] = new DBImportPath();
                        importPathes[index].Read(ref results, index);
                    }
                    return importPathes;
                }
            }
            catch (Exception ex)
            {
                DBTVSeries.Log("Error in DBImportPath.Get (" + ex.Message + ").");
            }
            return null;
        }
    }
}