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

    public class DBTorrentSearch : DBTable
    {
        public const String cTableName = "torrent";
        public const int cDBVersion = 2;

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

            int nCurrentDBTorrentVersion = cDBVersion;
            while (DBOption.GetOptions(DBOption.cDBTorrentVersion) != nCurrentDBTorrentVersion)
                // take care of the upgrade in the table
                switch ((int)DBOption.GetOptions(DBOption.cDBTorrentVersion))
                {
                    default:
                        {
                            // 1 or nothing: assume it's starting from scratch or it's an older version
                            // put the default ones
                            DBTorrentSearch item = new DBTorrentSearch("TorrentPortal");
                            item[DBTorrentSearch.cSearchUrl] = "http://www.torrentportal.com/torrents-search.php?search=$search$&sort=seeders&d=desc&type=and&sizel=&sizeh=&cat=3&hidedead=on&exclude=";
                            item[DBTorrentSearch.cSearchRegex] = @"<tr><td class=""alt\d n c""><a href=""(?<link>/download/(?<id>\d*)[^""]*)"">.*?(?:<td.*?/td>){2}<td.*?<a href=""/detail[^>]*>(?:<[^>]*>)*(?<name>[^<]*).*?/td>(?:<td.*?/td>)<td[^>]*>(?:<[^>]*>)*(?<size>[^<]*).*?/td><td[^>]*>(?:<[^>]*>)*(?<seeds>\d*).*?/td><td[^>]*>(?:<[^>]*>)*(?<leechers>\d*).*?/td>";
                            item[DBTorrentSearch.cDetailsUrl] = "http://www.torrentportal.com/details/$id$";
                            item[DBTorrentSearch.cDetailsRegex] = "Number of files.*?<td>[^\\d]*(?<filecount>\\d*)";
                            item.Commit();

                            item = new DBTorrentSearch("Mininova");
                            item[DBTorrentSearch.cSearchUrl] = "http://www.mininova.org/search/$search$/seeds";
                            item[DBTorrentSearch.cSearchRegex] = @"<tr[^>]*><td>(?<date>[^<]*?)</td><td><a[^>]*>TV Shows</a></td><td>.*?<a[^>]*href=""(?<link>/get/(?<id>\d*))""[^>]*>.*?<a[^>]*href=""/tor/\d*"">(?<name>[^<]*).*?right"">(?<size>[^<]*)</td><td align=""right""><span[^>]*>(?<seeds>\d*)</span></td><td align=""right""><span[^>]*>(?<leechers>\d*)</span></td>";
                            item[DBTorrentSearch.cDetailsUrl] = "http://www.mininova.org/det/$id$";
                            item[DBTorrentSearch.cDetailsRegex] = @"Total size:</strong>.*?in (?<filecount>\d*)";
                            item.Commit();
                            DBOption.SetOptions(DBOption.cDBTorrentVersion, nCurrentDBTorrentVersion);
                        }
                        break;
                }
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
