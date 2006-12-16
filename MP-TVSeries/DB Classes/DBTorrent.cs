using System;
using System.Collections.Generic;
using System.Text;
using SQLite.NET;
using System.IO;
using MediaPortal.Database;

namespace WindowPlugins.GUITVSeries
{
    public class DBTorrentSearch : DBTable
    {
        public const String cTableName = "torrent";
        public const int cDBVersion = 1;

        public const String cID = "ID"; 
        public const String cSearchUrl = "searchUrl";
        public const String cSearchRegex = "searchRegex";
        public const String cDetailsUrl = "detailsUrl";
        public const String cDetailsRegex = "detailsRegex";

        public override string ToString()
        {
            return base[cID];
        }

        static DBTorrentSearch()
        {
            DBTorrentSearch dummy = new DBTorrentSearch();

            List<DBTorrentSearch> torrentSearchList = DBTorrentSearch.Get();
            if (torrentSearchList.Count == 0)
            {
                // put the default ones
                DBTorrentSearch item = new DBTorrentSearch("TorrentPortal");
                item[DBTorrentSearch.cSearchUrl] = "http://www.torrentportal.com/torrents-search.php?search=$search$&sort=seeders&d=desc&type=and&sizel=&sizeh=&cat=3&hidedead=on&exclude=";
                item[DBTorrentSearch.cSearchRegex] = "<tr><td class=\"alt\\d n c\"><a href=\"(?<link>/download/(?<id>\\d*)[^\"]*)\">.*?(?:<td.*?/td>){2}<td.*?<a href=\"/detail[^>]*>(?:<[^>]*>)*(?<name>[^<]*).*?/td>(?:<td.*?/td>){2}<td[^>]*>(?:<[^>]*>)*(?<seeds>[^<]*).*?/td><td[^>]*>(?:<[^>]*>)*(?<leechers>[^<]*).*?/td>";
                item[DBTorrentSearch.cDetailsUrl] = "http://www.torrentportal.com/details/$id$";
                item[DBTorrentSearch.cDetailsRegex] = "Number of files.*?<td>[^\\d]*(?<filecount>\\d*)";
                item.Commit();

                item = new DBTorrentSearch("Mininova");
                item[DBTorrentSearch.cSearchUrl] = "http://www.mininova.org/search/$search$/seeds";
                item[DBTorrentSearch.cSearchRegex] = "<td>(?<date>[^<]*?)</td><td><a[^>]*>TV Shows</a></td><td><a[^>]*?href=\"(?<link>/get/(?<id>\\d*))\".*?<a href=[^>]*?>(?<name>[^\"]*?)<.*?\"right\">(?<size>[^>]*?)<.*?class=\".*?\">(?<seeds>\\d*)<.*?class=\".*?\">(?<leechers>\\d*)";
                item[DBTorrentSearch.cDetailsUrl] = "http://www.mininova.org/det/$id$";
                item[DBTorrentSearch.cDetailsRegex] = "Number of files.*?<td>[^\\d]*(?<filecount>\\d*)";
                item.Commit();
            }
//             int nCurrentDBTorrentVersion = cDBVersion;
//             while (DBOption.GetOptions(DBOption.cDBTorrentVersion) != nCurrentDBTorrentVersion)
//                 // take care of the upgrade in the table
//                 switch ((int)DBOption.GetOptions(DBOption.cDBTorrentVersion))
//                 {
//                     case 1:
//                         try
//                         {
//                             DBOption.SetOptions(DBOption.cDBTorrentVersion, nCurrentDBTorrentVersion);
//                         }
//                         catch { }
//                         break;
// 
//                     default:
//                         break;
//                 }
        }

        public DBTorrentSearch()
            : base(cTableName)
        {
            InitColumns();
            InitValues();
            // all available fields
        }

        public DBTorrentSearch(String sName)
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
            AddColumn(cDetailsUrl, new DBField(DBField.cTypeString));
            AddColumn(cDetailsRegex, new DBField(DBField.cTypeString));
        }

        public static void Clear(SQLCondition conditions)
        {
            Clear(new DBTorrentSearch(), conditions);
        }

        public static void GlobalSet(String sKey, DBValue Value)
        {
            GlobalSet(sKey, Value, new SQLCondition());
        }

        public static void GlobalSet(String sKey, DBValue Value, SQLCondition condition)
        {
            GlobalSet(new DBTorrentSearch(), sKey, Value, condition);
        }

        public static void GlobalSet(String sKey1, String sKey2)
        {
            GlobalSet(sKey1, sKey2, new SQLCondition());
        }

        public static void GlobalSet(String sKey1, String sKey2, SQLCondition condition)
        {
            GlobalSet(new DBTorrentSearch(), sKey1, sKey2, condition);
        }

        public static List<DBTorrentSearch> Get()
        {
            // create table if it doesn't exist already
            SQLCondition condition = new SQLCondition();
            String sqlQuery = "select * from " + cTableName + condition + " order by " + cID;
            SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
            List<DBTorrentSearch> outList = new List<DBTorrentSearch>();
            if (results.Rows.Count > 0)
            {
                for (int index = 0; index < results.Rows.Count; index++)
                {
                    DBTorrentSearch outItem = new DBTorrentSearch();
                    outItem.Read(ref results, index);
                    outList.Add(outItem);
                }
            }
            return outList;
        }
    }
}
