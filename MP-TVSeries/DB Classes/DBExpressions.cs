using System;
using System.Collections.Generic;
using System.Text;
using SQLite.NET;
using MediaPortal.Database;

namespace WindowPlugins.GUITVSeries
{
    public class DBExpression : DBTable
    {
        public const String cTableName = "expressions";

        public const String cIndex = "ID";
        public const String cEnabled = "enabled";
        public const String cType = "type";
        public const String cExpression = "expression";

        public const String cType_Simple = "simple";
        public const String cType_Regexp = "regexp";

        public DBExpression()
            : base(cTableName)
        {
            InitColumns();
            InitValues();
        }

        public DBExpression(long ID)
            : base(cTableName)
        {
            InitColumns();
            if (!ReadPrimary(ID.ToString()))
                InitValues();
        }

        private void InitValues()
        {
            this[cEnabled] = 0;
            this[cType] = String.Empty;
            this[cExpression] = String.Empty;
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
            AddColumn(cIndex, new DBField(DBField.cTypeInt, true));
            AddColumn(cEnabled, new DBField(DBField.cTypeInt));
            AddColumn(cType, new DBField(DBField.cTypeString));
            AddColumn(cExpression, new DBField(DBField.cTypeString));
        }

        public static void ClearAll()
        {
            String sqlQuery = "delete from "+ cTableName;
            DBTVSeries.Execute(sqlQuery);
        }

        public static DBExpression[] GetAll()
        {
            try
            {
                // make sure the table is created - create a dummy object
                DBExpression dummy = new DBExpression();

                // retrieve all fields in the table
                String sqlQuery = "select * from " + cTableName + " order by " + cIndex;
                SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
                if (results.Rows.Count > 0)
                {
                    DBExpression[] expressions = new DBExpression[results.Rows.Count];
                    for (int index = 0; index < results.Rows.Count; index++)
                    {
                        expressions[index] = new DBExpression();
                        expressions[index].Read(ref results, index);
                    }
                    return expressions;
                }
            }
            catch (Exception ex)
            {
                DBTVSeries.Log("Error in DBExpression.Get (" + ex.Message + ").");
            }
            return null;
        }
    }
}
