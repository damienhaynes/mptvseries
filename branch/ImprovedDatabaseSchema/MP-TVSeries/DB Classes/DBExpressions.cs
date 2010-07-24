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
using SQLite.NET;
using WindowPlugins.GUITVSeries.DataBase;

namespace WindowPlugins.GUITVSeries.DataClass
{
    public class DBExpression : DBTable
    {
        public const String cTableName = "expressions";

		#region Local DB Fields
		//declare fieldsnames as constants here, and then add them to TableFields
		public const String cIndex = "ID";
        public const String cEnabled = "enabled";
        public const String cType = "type";
        public const String cExpression = "expression";

		// all mandatory fields. Place the primary key first - it's just good manners
		public static readonly DBFieldDefList TableFields = new DBFieldDefList {
                    {cIndex,       new DBFieldDef{ FieldName = cIndex,      TableName = cTableName, Type = DBFieldType.Int,     Primary = true }},
                    {cEnabled,     new DBFieldDef{ FieldName = cEnabled,    TableName = cTableName, Type = DBFieldType.Int }},
                    {cType,        new DBFieldDef{ FieldName = cType,       TableName = cTableName, Type = DBFieldType.String }},
                    {cExpression,  new DBFieldDef{ FieldName = cExpression, TableName = cTableName, Type = DBFieldType.String }}
        };
		#endregion

		public enum ExpressionType
        {
            Simple,
            RegExp
        } 

        public DBExpression()
			: base(cTableName, TableFields)
        {
        }

        public DBExpression(long ID)
			: base(cTableName, TableFields)
        {
            ReadPrimary(ID.ToString());
        }

        static DBExpression()
        {
        	DatabaseUpgrade();
        }

		#region deprecated database upgrade method - use MaintainDatabaseTable instead
		/// <summary>
		/// deprecated database upgrade method - use MaintainDatabaseTable instead
		/// TODO: delete this
		/// </summary>
		private static void DatabaseUpgrade()
    	{
    		DBExpression[] expressions = DBExpression.GetAll();
    		foreach (DBExpression e in expressions)
    		{
    			if(e[DBExpression.cExpression] == @"(?<series>[^\\\[]*) - \[(?<season>[0-9]{1,2})x(?<episode>[0-9\w]+)\](( |)(-( |)|))(?<title>(?![^\]*?sample)[^$]*?)\.(?<ext>[^.]*)")
    			{
    				// fix typo
    				e[DBExpression.cExpression] = @"(?<series>[^\\\[]*) - \[(?<season>[0-9]{1,2})x(?<episode>[0-9\W]+)\](( |)(-( |)|))(?<title>(?![^\\]*?sample)[^$]*?)\.(?<ext>[^.]*)";
    				e.Commit();
    				break;
    			}
    		}
		}
		#endregion

		internal static void MaintainDatabaseTable(Version lastVersion)
		{
			try {
				//test for table existance
				if (!DatabaseHelper.TableExists(cTableName)) {
					DatabaseHelper.CreateTable(cTableName, TableFields.Values);
					AddDefaults();
				}
			} catch (Exception) {
				MPTVSeriesLog.Write("Error Maintaining the " + cTableName + " Table");
			}
		}
		
		public static void AddDefaults()
        {
            DBExpression expression = new DBExpression();
            expression[DBExpression.cEnabled] = "1";

            expression[DBExpression.cIndex] = "0";
            expression[DBExpression.cType] = DBExpression.ExpressionType.RegExp.ToString();
            expression[DBExpression.cExpression] = @"^.*?\\?(?<series>[^\\$]+?)[ .-]+(?:[s]?(?<season>\d+)[ .-]?[ex](?<episode>\d+)|(?:\#|\-\s)(?<season>\d+)\.(?<episode>\d+))(?:[ex+-]*(?<episode2>\d+))?[ .-]*(?<title>(?![^\\]*?sample[ .-])[^$]*?)\.(?<ext>[^.]*)$";
            expression.Commit();

            expression[DBExpression.cIndex] = "1";
            expression[DBExpression.cType] = DBExpression.ExpressionType.RegExp.ToString();
            expression[DBExpression.cExpression] = @"^.*?\\?(?<series>[^\\$]+?)(?:s(?<season>[0-3]?\d)\s?ep?(?<episode>\d\d)|(?<season>(?:[0-1]\d|(?<!\d)\d))x?(?<episode>\d\d))(?!\d)(?:[ .-]?(?:s\k<season>e?(?<episode2>\d{2}(?!\d))|\k<season>x?(?<episode2>\d{2}(?!\d))|(?<episode2>\d\d(?!\d))|E(?<episode2>\d\d))|)[ -.]*(?<title>(?![^\\]*?sample)[^\\]*?[^\\]*?)\.(?<ext>[^.]*)$";
            expression.Commit();

            expression[DBExpression.cIndex] = "2";
            expression[DBExpression.cType] = DBExpression.ExpressionType.RegExp.ToString();
            expression[DBExpression.cExpression] = @"^(?<series>[^\\$]+)\\[^\\$]*?(?:s(?<season>[0-1]?\d)ep?(?<episode>\d\d)|(?<season>(?:[0-1]\d|(?<!\d)\d))x?(?<episode>\d\d))(?!\d)(?:[ .-]?(?:s\k<season>e?(?<episode2>\d{2}(?!\d))|\k<season>x?(?<episode2>\d{2}(?!\d))|(?<episode2>\d\d(?!\d))|E(?<episode2>\d\d))|)[ -.]*(?<title>(?!.*sample)[^\\]*?[^\\]*?)\.(?<ext>[^.]*)$";
            expression.Commit();

            expression[DBExpression.cType] = DBExpression.ExpressionType.RegExp.ToString();
            expression[DBExpression.cIndex] = "3";
            expression[DBExpression.cExpression] = @"(?<series>[^\\\[]*) - \[(?<season>[0-9]{1,2})x(?<episode>[0-9\W]+)\](( |)(-( |)|))(?<title>(?![^\\]*?sample)[^$]*?)\.(?<ext>[^.]*)";
            expression.Commit();

            expression[DBExpression.cIndex] = "4";
            expression[DBExpression.cType] = DBExpression.ExpressionType.RegExp.ToString();
            expression[DBExpression.cExpression] = @"(?<series>[^\\$]*) - season (?<season>[0-9]{1,2}) - (?<title>(?![^\\]*?sample)[^$]*?)\.(?<ext>[^.]*)";
            expression.Commit();

            expression[DBExpression.cIndex] = "5";
            expression[DBExpression.cType] = DBExpression.ExpressionType.Simple.ToString();
            expression[DBExpression.cExpression] = @"<series> - <season>x<episode> - <title>.<ext>";
            expression.Commit();

            expression[DBExpression.cIndex] = "6";
            expression[DBExpression.cType] = DBExpression.ExpressionType.Simple.ToString();
            expression[DBExpression.cExpression] = @"<series>\Season <season>\Episode <episode> - <title>.<ext>";
            expression.Commit();

            expression[DBExpression.cIndex] = "7";
            expression[DBExpression.cType] = DBExpression.ExpressionType.Simple.ToString();
            expression[DBExpression.cExpression] = @"<series>\<season>x<episode> - <title>.<ext>";
            expression.Commit();
            
        }

        public static void ClearAll()
        {
        	const string sqlQuery = "delete from "+ cTableName;
        	DBTVSeries.Execute(sqlQuery);
        }

    	public static void Clear(int Index) {
			Clear(DBExpression.cTableName, new SQLCondition(DBExpression.TableFields, DBExpression.cIndex, Index, SQLConditionType.Equal));            
        }

        public static DBExpression[] GetAll()
        {
            try
            {
                // retrieve all fields in the table
                const string sqlQuery = "select * from " + cTableName + " order by " + cIndex;
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