using System;
using System.Collections.Generic;
using System.Text;
using SQLite.NET;
using MediaPortal.Database;

namespace WindowPlugins.GUITVSeries
{
    public class DBReplacements : DBTable
    {
        public const String cTableName = "replacements";

        public const String cIndex = "ID";
        public const String cEnabled = "enabled";
        public const String cToReplace = "toreplace";
        public const String cWith = "with";

        public static Dictionary<String, String> s_FieldToDisplayNameMap = new Dictionary<String, String>();

        static DBReplacements()
        {
            s_FieldToDisplayNameMap.Add(cEnabled, "Enabled");
            s_FieldToDisplayNameMap.Add(cToReplace, "Replace this..");
            s_FieldToDisplayNameMap.Add(cWith, "With this");

            DBReplacements dummy = new DBReplacements();
        }

        public static String PrettyFieldName(String sFieldName)
        {
            if (s_FieldToDisplayNameMap.ContainsKey(sFieldName))
                return s_FieldToDisplayNameMap[sFieldName];
            else
                return sFieldName;
        }

        public DBReplacements()
            : base(cTableName)
        {
            InitColumns();
            InitValues();
        }

        public DBReplacements(long ID)
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
            AddColumn(cToReplace, new DBField(DBField.cTypeString));
            AddColumn(cWith, new DBField(DBField.cTypeString));
        }

        public static void ClearAll()
        {
            String sqlQuery = "delete from "+ cTableName;
            DBTVSeries.Execute(sqlQuery);
        }

        public static DBReplacements[] GetAll()
        {
            try
            {
                // make sure the table is created - create a dummy object

                // retrieve all fields in the table
                String sqlQuery = "select * from " + cTableName + " order by " + cIndex;
                SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
                if (results.Rows.Count > 0)
                {
                    DBReplacements[] outlist = new DBReplacements[results.Rows.Count];
                    for (int index = 0; index < results.Rows.Count; index++)
                    {
                        outlist[index] = new DBReplacements();
                        outlist[index].Read(ref results, index);
                    }
                    return outlist;
                }
            }
            catch (Exception ex)
            {
                DBTVSeries.Log("Error in DBReplacements.Get (" + ex.Message + ").");
            }
            return null;
        }
    }
}
