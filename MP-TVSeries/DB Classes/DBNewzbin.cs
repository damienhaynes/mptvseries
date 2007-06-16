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
        public const int cDBVersion = 1;

        public const String cID = "ID"; 
        public const String cSearchUrl = "searchUrl";
        public const String cSearchRegex = "searchRegex";
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
            if (NewzsSearchList.Count == 0)
            {
                // put the default ones
                DBNewzbin item = new DBNewzbin("Newzbin");
                item[DBNewzbin.cSearchUrl] = "http://v3.newzbin.com/search/query/?fpn=p&q=$search$&category=8&searchaction=Go&sort=date&order=desc";
//                item[DBNewzbin.cSearchRegex] = "<td rowspan=\"2\" class=\"title\">.*?browse/post/(?<ID>\\d*)/\">(?<name>.*?)</a>.*?\"fileSize\".*?<span>(?<size>.*?)</span>.*?<span[^>]*>(?<post>.*?)</span>.*?<span[^>]*>(?<report>.*?)</span>";
                // logged in version
                item[DBNewzbin.cSearchRegex] = "<td colspan=\"3\" class=\"title\">.*?browse/post/(?<ID>\\d*)/\">(?<name>.*?)</a>.*?<span[^>]*>(?<post>.*?)</span>.*?<span[^>]*>(?<report>.*?)</span>.*?\"fileSize\".*?<span>(?<size>.*?)</span>(?:.*?Video Fmt:\\s*<a[^>]*>(?<format>[^<]*)</a>.*?|.*?)</tr>\\s*</tbody>";
                item.Commit();
            }
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
            AddColumn(cSearchRegex, new DBField(DBField.cTypeString));
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
