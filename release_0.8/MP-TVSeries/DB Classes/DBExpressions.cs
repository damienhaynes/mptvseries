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
        public const int cDBVersion = 2;

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

        static DBExpression()
        {
            // make sure the table is created - create a dummy object
            DBExpression dummy = new DBExpression();
            DBExpression[] expressions = DBExpression.GetAll();

            int nCurrentVersion = cDBVersion;
            while (DBOption.GetOptions(DBOption.cDBExpressionsVersion) != nCurrentVersion)
                // take care of the upgrade in the table
                switch ((int)DBOption.GetOptions(DBOption.cDBExpressionsVersion))
                {
                    case 1:
                        // upgrade to version 2; try to replace the previous regular expressions
                        if (expressions != null && expressions.Length > 0)
                        {
                            bool bExp1Replaced = false;
                            bool bExp2Replaced = false;
                            for (int index = 0; index < expressions.Length; index++)
                            {
                                DBExpression expression = expressions[index];
                                if (expression[DBExpression.cExpression] == @"^(?<series>[^\\$]+)\\[^\\$]*?(?:s(?<season>[0-1]?[0-9])e(?<episode>[0-9]{2})|(?<season>(?:[0-1][0-9]|(?<!\d)[0-9]))x?(?<episode>[0-9]{2}))(?!\d)[ \-\.]*(?<title>[^\\]*?)\.(?<ext>[^.]*)$")
                                {
                                    bExp1Replaced = true;
                                    expression[DBExpression.cExpression] = @"^(?<series>[^\\$]+)\\[^\\$]*?(?:s(?<season>[0-1]?\d)e(?<episode>\d\d)|(?<season>(?:[0-1]\d|(?<!\d)\d))x?(?<episode>\d\d))(?!\d)(?:[ .-]?(?:(?<episode2>\d\d(?!\d))|E(?<episode2>\d\d)|(?:\d\d|(?<!\d)\d)x?(?<episode2>\d{2}(?!\d)))|)[ -.]*(?<title>[^\\]*?(?<HR>HR\.)?[^\\]*?)\.(?<ext>[^.]*)$";
                                    expression.Commit();
                                }
                                else if (expression[DBExpression.cExpression] == @"^.*?\\?(?<series>[^\\$]+?)(?:s(?<season>[0-1]?[0-9])e(?<episode>[0-9]{2})|(?<season>(?:[0-1][0-9]|(?<!\d)[0-9]))x?(?<episode>[0-9]{2}))(?!\d)[ \-\.]*(?<title>[^\\]*?)\.(?<ext>[^.]*)$")
                                {
                                    bExp2Replaced = true;
                                    expression[DBExpression.cExpression] = @"^.*?\\?(?<series>[^\\$]+?)(?:s(?<season>[0-1]?\d)e(?<episode>\d\d)|(?<season>(?:[0-1]\d|(?<!\d)\d))x?(?<episode>\d\d))(?!\d)(?:[ .-]?(?:(?<episode2>\d\d(?!\d))|E(?<episode2>\d\d)|(?:\d\d|(?<!\d)\d)x?(?<episode2>\d{2}(?!\d)))|)[ -.]*(?<title>[^\\]*?(?<HR>HR\.)?[^\\]*?)\.(?<ext>[^.]*)$";
                                    expression.Commit();
                                }
                            }

                            if (!bExp1Replaced)
                            {
                                // well, not found, so the user might have modified it or deleted it. Add the new one at the end
                                DBExpression expression = new DBExpression();
                                expression[DBExpression.cIndex] = expressions.Length;
                                expression[DBExpression.cEnabled] = "1";
                                expression[DBExpression.cType] = DBExpression.cType_Regexp;
                                expression[DBExpression.cExpression] = @"^(?<series>[^\\$]+)\\[^\\$]*?(?:s(?<season>[0-1]?\d)e(?<episode>\d\d)|(?<season>(?:[0-1]\d|(?<!\d)\d))x?(?<episode>\d\d))(?!\d)(?:[ .-]?(?:(?<episode2>\d\d(?!\d))|E(?<episode2>\d\d)|(?:\d\d|(?<!\d)\d)x?(?<episode2>\d{2}(?!\d)))|)[ -.]*(?<title>[^\\]*?(?<HR>HR\.)?[^\\]*?)\.(?<ext>[^.]*)$";
                                expression.Commit();
                            }

                            if (!bExp2Replaced)
                            {
                                DBExpression expression = new DBExpression();
                                expression[DBExpression.cIndex] = expressions.Length + 1;
                                expression[DBExpression.cEnabled] = "1";
                                expression[DBExpression.cType] = DBExpression.cType_Regexp;
                                expression[DBExpression.cExpression] = @"^.*?\\?(?<series>[^\\$]+?)(?:s(?<season>[0-1]?\d)e(?<episode>\d\d)|(?<season>(?:[0-1]\d|(?<!\d)\d))x?(?<episode>\d\d))(?!\d)(?:[ .-]?(?:(?<episode2>\d\d(?!\d))|E(?<episode2>\d\d)|(?:\d\d|(?<!\d)\d)x?(?<episode2>\d{2}(?!\d)))|)[ -.]*(?<title>[^\\]*?(?<HR>HR\.)?[^\\]*?)\.(?<ext>[^.]*)$";
                                expression.Commit();
                            }
                        }
                        DBOption.SetOptions(DBOption.cDBExpressionsVersion, nCurrentVersion);
                        break;

                    default:
                        break;
                }

            expressions = DBExpression.GetAll();
            if (expressions == null || expressions.Length == 0)
            {
                // no expressions in the db => put the default ones
                DBExpression expression = new DBExpression();
                expression[DBExpression.cIndex] = "0";
                expression[DBExpression.cEnabled] = "1";
                expression[DBExpression.cType] = DBExpression.cType_Simple;
                expression[DBExpression.cExpression] = @"<series> - <season>x<episode> - <title>.<ext>";
                expression.Commit();

                expression[DBExpression.cIndex] = "1";
                expression[DBExpression.cExpression] = @"\<series>\Season <season>\Episode <episode> - <title>.<ext>";
                expression.Commit();

                expression[DBExpression.cType] = DBExpression.cType_Regexp;
                expression[DBExpression.cIndex] = "2";
                expression[DBExpression.cExpression] = @"(?<series>[^\\\[]*) - \[(?<season>[0-9]{1,2})x(?<episode>[0-9\W]+)\](( |)(-( |)|))(?<title>[^$]*?)\.(?<ext>[^.]*)";
                expression.Commit();

                expression[DBExpression.cIndex] = "3";
                expression[DBExpression.cExpression] = @"(?<series>[^\\$]*) - season (?<season>[0-9]{1,2}) - (?<title>[^$]*?)\.(?<ext>[^.]*)";
                expression.Commit();

                expression[DBExpression.cIndex] = "4";
                expression[DBExpression.cExpression] = @"^(?<series>[^\\$]+)\\[^\\$]*?(?:s(?<season>[0-1]?\d)e(?<episode>\d\d)|(?<season>(?:[0-1]\d|(?<!\d)\d))x?(?<episode>\d\d))(?!\d)(?:[ .-]?(?:(?<episode2>\d\d(?!\d))|E(?<episode2>\d\d)|(?:\d\d|(?<!\d)\d)x?(?<episode2>\d{2}(?!\d)))|)[ -.]*(?<title>[^\\]*?(?<HR>HR\.)?[^\\]*?)\.(?<ext>[^.]*)$";
                expression.Commit();

                expression[DBExpression.cIndex] = "5";
                expression[DBExpression.cExpression] = @"^.*?\\?(?<series>[^\\$]+?)(?:s(?<season>[0-1]?\d)e(?<episode>\d\d)|(?<season>(?:[0-1]\d|(?<!\d)\d))x?(?<episode>\d\d))(?!\d)(?:[ .-]?(?:(?<episode2>\d\d(?!\d))|E(?<episode2>\d\d)|(?:\d\d|(?<!\d)\d)x?(?<episode2>\d{2}(?!\d)))|)[ -.]*(?<title>[^\\]*?(?<HR>HR\.)?[^\\]*?)\.(?<ext>[^.]*)$";
                expression.Commit();
            }
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
                MPTVSeriesLog.Write("Error in DBExpression.Get (" + ex.Message + ").");
            }
            return null;
        }
    }
}
