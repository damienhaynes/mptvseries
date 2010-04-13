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
using SQLite.NET;
using WindowPlugins.GUITVSeries.DataBase;

namespace WindowPlugins.GUITVSeries.DataClass
{
    public class DBNewzbin : DBTable
    {
        public const String cTableName = "news";
        public const int cDBVersion = 4;

		#region Local DB Fields
		//declare fieldsnames as constants here, and then add them to TableFields
		public const String cID = "ID"; 
        public const String cSearchUrl = "searchUrl";
        public const String cSearchRegexReport = "searchRegexMain";
        public const String cSearchRegexName = "searchRegexName";
        public const String cSearchRegexID = "searchRegexID";
        public const String cSearchRegexSize = "searchRegexSize";
        public const String cSearchRegexPostDate = "searchRegexPost";
        public const String cSearchRegexReportDate = "searchRegexReport";
        public const String cSearchRegexFormat = "searchRegexFormat";
        public const String cSearchRegexLanguage = "searchRegexLanguage";
        public const String cSearchRegexGroup = "searchRegexGroup";
        public const String cSearchRegexIsolateArticleName = "searchRegexIsolateArticleName";
        public const String cSearchRegexParseArticleName = "searchRegexParseArticleName";
        public const String cLogin = "login";
        public const String cPassword = "password";
        public const String cCookieList = "cookielist";

		// all mandatory fields. Place the primary key first - it's just good manners
		public static DBFieldDefList TableFields = new DBFieldDefList{
            {cID,                           new DBFieldDef{ FieldName = cID,								Type = DBFieldType.String,      Primary = true}},
            {cSearchUrl,                    new DBFieldDef{ FieldName = cSearchUrl,						Type = DBFieldType.String}},
            {cSearchRegexReport,            new DBFieldDef{ FieldName = cSearchRegexReport,				Type = DBFieldType.String}},
            {cSearchRegexName,              new DBFieldDef{ FieldName = cSearchRegexName,				Type = DBFieldType.String}},
            {cSearchRegexID,                new DBFieldDef{ FieldName = cSearchRegexID,					Type = DBFieldType.String}},
            {cSearchRegexSize,              new DBFieldDef{ FieldName = cSearchRegexSize,				Type = DBFieldType.String}},
            {cSearchRegexPostDate,          new DBFieldDef{ FieldName = cSearchRegexPostDate,			Type = DBFieldType.String}},
            {cSearchRegexReportDate,        new DBFieldDef{ FieldName = cSearchRegexReportDate,			Type = DBFieldType.String}},
            {cSearchRegexFormat,			new DBFieldDef{ FieldName = cSearchRegexFormat,				Type = DBFieldType.String}},
            {cSearchRegexLanguage,			new DBFieldDef{ FieldName = cSearchRegexLanguage,			Type = DBFieldType.String}},
            {cSearchRegexGroup,				new DBFieldDef{ FieldName = cSearchRegexGroup,				Type = DBFieldType.String}},
            {cSearchRegexIsolateArticleName,new DBFieldDef{ FieldName = cSearchRegexIsolateArticleName,	Type = DBFieldType.String}},
            {cSearchRegexParseArticleName,  new DBFieldDef{ FieldName = cSearchRegexParseArticleName,	Type = DBFieldType.String}},
            {cLogin,						new DBFieldDef{ FieldName = cLogin,							Type = DBFieldType.String}},
            {cPassword,						new DBFieldDef{ FieldName = cPassword,						Type = DBFieldType.String}},
            {cCookieList,					new DBFieldDef{ FieldName = cCookieList,						Type = DBFieldType.String}},
        };
		#endregion

		public override string ToString()
        {
            return this[cID];
        }

        static DBNewzbin()
        {
            const int nCurrentDBVersion = cDBVersion;
            int nUpgradeDBVersion = DBOption.GetOptions(DBOption.cDBNewzbinVersion);
			if (nUpgradeDBVersion == nCurrentDBVersion) {
				return;
			}
			List<DBNewzbin> NewzsSearchList = DBNewzbin.Get();
			while (nUpgradeDBVersion != nCurrentDBVersion)
                // take care of the upgrade in the table
                switch (nUpgradeDBVersion)
                {
                    default:
                        {
                            // from scratch or from earlier version; logic of parsing changed, it's done in multiple regexp now. So add those new strings 
                            try
                            {
                                DBNewzbin item = null;
                                if (NewzsSearchList.Count != 0)
                                    item = NewzsSearchList[0];
                                else
                                    item = new DBNewzbin("0");

                                item[DBNewzbin.cSearchRegexReport] = @"<td colspan=""3"" class=""title"">(?<post>.*?)</tr>\s*</tbody>";
                                item[DBNewzbin.cSearchRegexName] = "<a href=\"/browse/post.*?>([^<]*)";
                                item[DBNewzbin.cSearchRegexID] = "<a href=\"/browse/post/(.*?)/\">";
                                item[DBNewzbin.cSearchRegexSize] = "class=\"fileSize\">.*?<span>([^<]*)";
                                item[DBNewzbin.cSearchRegexPostDate] = @"class=""(?<param>age[^""]*)"">([^<]*)(?=.*\k<param>)";
                                item[DBNewzbin.cSearchRegexReportDate] = @"class=""(?<param>age[^""]*)"">([^<]*)(?!.*\k<param>)";
                                item[DBNewzbin.cSearchRegexFormat] = "ps_rb_video_format[^>]*>([^<]*)";
                                item[DBNewzbin.cSearchRegexLanguage] = "ps_rb_language[^>]*>([^<]*)";
                                item[DBNewzbin.cSearchRegexGroup] = "<a href=\"/browse/group[^\"]*\" title=[^>]*>([^<]*)";
                                item[DBNewzbin.cSearchRegexIsolateArticleName] = @"</span>\s*([^<]*\.(?:r00|part0?1\.rar|\.0*1)[^<]*)";
                                item[DBNewzbin.cSearchRegexParseArticleName] = @"(?:&quot;|\[)?(.*?)(?:&quot;|\])?(?:\.r\d\d|\.part0?1\.rar|\.0*1| - | -=- |\]-\[| \(|\) |&quot;)";
                                item.Commit();
                                nUpgradeDBVersion = nCurrentDBVersion;
                            }
                            catch { }
                        }
                        break;

                    case 2:
                        {
                            DBNewzbin item = null;
                            if (NewzsSearchList.Count != 0)
                            {
                                item = NewzsSearchList[0];
                                item[DBNewzbin.cSearchRegexPostDate] = @"class=""(?<param>age[^""]*)"">([^<]*)(?=.*\k<param>)";
                                item[DBNewzbin.cSearchRegexReportDate] = @"class=""(?<param>age[^""]*)"">([^<]*)(?!.*\k<param>)";
                                item.Commit();
                                nUpgradeDBVersion++;
                            }
                            else
                                nUpgradeDBVersion = 0;
                            
                        }
                        break;

                    case 3:
                        {
                            DBNewzbin item = null;
                            if (NewzsSearchList.Count != 0)
                            {
                                item = NewzsSearchList[0];
                                item[DBNewzbin.cSearchRegexIsolateArticleName] = @"</span>\s*([^<]*\.(?:r00|part0?1\.rar|\.0*1)[^<]*)";
                                item[DBNewzbin.cSearchRegexParseArticleName] = @"(?:&quot;|\[)?(.*?)(?:&quot;|\])?(?:\.r\d\d|\.part0?1\.rar|\.0*1| - | -=- |\]-\[| \(|\) |&quot;)";
                                item.Commit();
                                nUpgradeDBVersion++;
                            }
                            else
                                nUpgradeDBVersion = 0;
                        }
                        break;
                }
            DBOption.SetOptions(DBOption.cDBNewzbinVersion, nCurrentDBVersion);
        }

		internal static void MaintainDatabaseTable(Version lastVersion)
		{
			try {
				//test for table existance
				if (!DatabaseHelper.TableExists(cTableName)) {
					DatabaseHelper.CreateTable(cTableName, TableFields.Values);
					return;
				}
			} catch (Exception) {
				MPTVSeriesLog.Write("Unable to Correctly Maintain the " + cTableName + " Table");
			}
		}
		
		public DBNewzbin()
            : base(cTableName)
        {
        }

        public DBNewzbin(String sName)
            : base(cTableName)
        {
            ReadPrimary(sName);
        }

        protected override void InitColumns()
        {
        	AddColumns(TableFields.Values);
        }

        public static void Clear(SQLCondition conditions)
        {
            Clear(new DBNewzbin(), conditions);
        }

        public static void GlobalSet(String sKey, DBValue Value)
        {
            GlobalSet(sKey, Value, new SQLCondition());
        }

        public static void GlobalSet(String sKey, DBValue Value, SQLCondition condition)
        {
            GlobalSet(new DBNewzbin(), sKey, Value, condition);
        }

        public static void GlobalSet(String sKey1, String sKey2)
        {
            GlobalSet(sKey1, sKey2, new SQLCondition());
        }

        public static void GlobalSet(String sKey1, String sKey2, SQLCondition condition)
        {
            GlobalSet(new DBNewzbin(), sKey1, sKey2, condition);
        }

        public static List<DBNewzbin> Get()
        {
            // create table if it doesn't exist already
            SQLCondition condition = new SQLCondition();
            String sqlQuery = "select * from " + cTableName + condition + " order by " + cID;
            SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
            List<DBNewzbin> outList = new List<DBNewzbin>();
            if (results.Rows.Count > 0)
            {
                for (int index = 0; index < results.Rows.Count; index++)
                {
                    DBNewzbin outItem = new DBNewzbin();
                    outItem.Read(ref results, index);
                    outList.Add(outItem);
                }
            }
            return outList;
        }
    }
}