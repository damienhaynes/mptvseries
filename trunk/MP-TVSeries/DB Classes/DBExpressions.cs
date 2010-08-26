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
using SQLite.NET;
using MediaPortal.Database;

namespace WindowPlugins.GUITVSeries
{
    public class DBExpression : DBTable
    {
        public const String cTableName = "expressions";
        public const int cDBVersion = 4;

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
            InitValues(-1,"");
        }

        public DBExpression(long ID)
            : base(cTableName)
        {
            InitColumns();
            if (!ReadPrimary(ID.ToString()))
                InitValues(-1,"");
        }

        static DBExpression()
        {
            // make sure the table is created - create a dummy object
            DBExpression dummy = new DBExpression();
            DBExpression[] expressions = DBExpression.GetAll();
                        
            if (expressions == null || expressions.Length == 0)
            {
                // no expressions in the db => put the default ones
                AddDefaults();
            }           
        }

        public static void AddDefaults()
        {
            DBExpression expression = new DBExpression();
            expression[DBExpression.cEnabled] = "1";
            expression[DBExpression.cIndex] = "0";
            expression[DBExpression.cType] = DBExpression.cType_Regexp;
            expression[DBExpression.cExpression] = @"^.*?\\?(?<series>[^\\$]+?)[ _.\-\[]+(?:[s]?(?<season>\d+)[ _.\-\[\]]*[ex](?<episode>\d+)|(?:\#|\-\s)(?<season>\d+)\.(?<episode>\d+))(?:[ex+-]*(?<episode2>\d+)|[ _.+-]*[\#s]?\d+[ex\.](?<episode2>(?!\d+\.\d+)\d+))?[ _.\-\[\]]*(?<title>(?![^\\]*?(?<!the)[ .(-]sample[ .)-]).*?)\.(?<ext>[^.]*)$";
            expression.Commit();

            expression[DBExpression.cIndex] = "1";
            expression[DBExpression.cType] = DBExpression.cType_Regexp;
            expression[DBExpression.cExpression] = @"^.*?\\?(?<series>[^\\$]+?)(?:s(?<season>[0-3]?\d)\s?ep?(?<episode>\d\d)|(?<season>(?:[0-1]\d|(?<!\d)\d))x?(?<episode>\d\d))(?!\d)(?:[ .-]?(?:s\k<season>e?(?<episode2>\d{2}(?!\d))|\k<season>x?(?<episode2>\d{2}(?!\d))|(?<episode2>\d\d(?!\d))|E(?<episode2>\d\d))|)[ -.]*(?<title>(?![^\\]*?sample)[^\\]*?[^\\]*?)\.(?<ext>[^.]*)$";
            expression.Commit();

            expression[DBExpression.cIndex] = "2";
            expression[DBExpression.cType] = DBExpression.cType_Regexp;
            expression[DBExpression.cExpression] = @"^(?<series>[^\\$]+)\\[^\\$]*?(?:s(?<season>[0-1]?\d)ep?(?<episode>\d\d)|(?<season>(?:[0-1]\d|(?<!\d)\d))x?(?<episode>\d\d))(?!\d)(?:[ .-]?(?:s\k<season>e?(?<episode2>\d{2}(?!\d))|\k<season>x?(?<episode2>\d{2}(?!\d))|(?<episode2>\d\d(?!\d))|E(?<episode2>\d\d))|)[ -.]*(?<title>(?!.*sample)[^\\]*?[^\\]*?)\.(?<ext>[^.]*)$";
            expression.Commit();

            expression[DBExpression.cType] = DBExpression.cType_Regexp;
            expression[DBExpression.cIndex] = "3";
            expression[DBExpression.cExpression] = @"(?<series>[^\\\[]*) - \[(?<season>[0-9]{1,2})x(?<episode>[0-9\W]+)\](( |)(-( |)|))(?<title>(?![^\\]*?sample)[^$]*?)\.(?<ext>[^.]*)";
            expression.Commit();

            expression[DBExpression.cIndex] = "4";
            expression[DBExpression.cType] = DBExpression.cType_Regexp;
            expression[DBExpression.cExpression] = @"(?<series>[^\\$]*) - season (?<season>[0-9]{1,2}) - (?<title>(?![^\\]*?sample)[^$]*?)\.(?<ext>[^.]*)";
            expression.Commit();

            expression[DBExpression.cIndex] = "5";
            expression[DBExpression.cType] = DBExpression.cType_Simple;
            expression[DBExpression.cExpression] = @"<series> - <season>x<episode> - <title>.<ext>";
            expression.Commit();

            expression[DBExpression.cIndex] = "6";
            expression[DBExpression.cType] = DBExpression.cType_Simple;
            expression[DBExpression.cExpression] = @"<series>\Season <season>\Episode <episode> - <title>.<ext>";
            expression.Commit();

            expression[DBExpression.cIndex] = "7";
            expression[DBExpression.cType] = DBExpression.cType_Simple;
            expression[DBExpression.cExpression] = @"<series>\<season>x<episode> - <title>.<ext>";
            expression.Commit();
            
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

        public static void Clear(int Index) {
            DBExpression dummy = new DBExpression(Index);
            Clear(dummy, new SQLCondition(dummy, DBExpression.cIndex, Index, SQLConditionType.Equal));            
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
