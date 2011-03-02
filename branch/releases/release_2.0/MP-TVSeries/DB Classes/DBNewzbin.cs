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
using System.IO;
using MediaPortal.Database;

namespace WindowPlugins.GUITVSeries
{
        public class DBNewzbin : DBTable
    {
        public const String cTableName = "news";
        public const int cDBVersion = 4;

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

        public override string ToString()
        {
            return base[cID];
        }

        static DBNewzbin()
        {
            DBNewzbin dummy = new DBNewzbin();

            List<DBNewzbin> NewzsSearchList = DBNewzbin.Get();
            int nCurrentDBVersion = cDBVersion;
            int nUpgradeDBVersion = DBOption.GetOptions(DBOption.cDBNewzbinVersion);
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

        public DBNewzbin()
            : base(cTableName)
        {
            InitColumns();
            InitValues();
            // all available fields
        }

        public DBNewzbin(String sName)
            : base(cTableName)
        {
            InitColumns();
            if (!ReadPrimary(sName))
            {
                InitValues();
            }
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
            AddColumn(cID, new DBField(DBField.cTypeString, true));
            AddColumn(cSearchUrl, new DBField(DBField.cTypeString));
            AddColumn(cSearchRegexReport, new DBField(DBField.cTypeString));
            AddColumn(cSearchRegexName, new DBField(DBField.cTypeString));
            AddColumn(cSearchRegexID, new DBField(DBField.cTypeString));
            AddColumn(cSearchRegexSize, new DBField(DBField.cTypeString));
            AddColumn(cSearchRegexPostDate, new DBField(DBField.cTypeString));
            AddColumn(cSearchRegexReportDate, new DBField(DBField.cTypeString));
            AddColumn(cSearchRegexFormat, new DBField(DBField.cTypeString));
            AddColumn(cSearchRegexLanguage, new DBField(DBField.cTypeString));
            AddColumn(cSearchRegexGroup, new DBField(DBField.cTypeString));
            AddColumn(cSearchRegexIsolateArticleName, new DBField(DBField.cTypeString));
            AddColumn(cSearchRegexParseArticleName, new DBField(DBField.cTypeString));
            AddColumn(cLogin, new DBField(DBField.cTypeString));
            AddColumn(cPassword, new DBField(DBField.cTypeString));
            AddColumn(cCookieList, new DBField(DBField.cTypeString));
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
