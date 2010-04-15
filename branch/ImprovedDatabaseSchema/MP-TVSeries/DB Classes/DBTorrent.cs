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
    public class DBTorrentSearch : DBTable
    {
        public const String cTableName = "torrent";

		#region Local DB Fields
		//declare fieldsnames as constants here, and then add them to TableFields
		public const String cID = "ID";
        public const String cSearchUrl = "searchUrl";
        public const String cSearchRegex = "searchRegex";
        public const String cDetailsUrl = "detailsUrl";
        public const String cDetailsRegex = "detailsRegex";

		// all mandatory fields. Place the primary key first - it's just good manners
		public static readonly DBFieldDefList TableFields = new DBFieldDefList {
            {cID,			new DBFieldDef{FieldName = cID,			TableName = cTableName,	Type = DBFieldType.String,	Primary = true}},
            {cSearchUrl,	new DBFieldDef{FieldName = cSearchUrl,	TableName = cTableName,	Type = DBFieldType.String}},
            {cSearchRegex,	new DBFieldDef{FieldName = cSearchRegex,TableName = cTableName,	Type = DBFieldType.String}},
            {cDetailsUrl,	new DBFieldDef{FieldName = cDetailsUrl,	TableName = cTableName,	Type = DBFieldType.String}},
            {cDetailsRegex,	new DBFieldDef{FieldName = cDetailsRegex,TableName = cTableName,Type = DBFieldType.String}}
		};
		#endregion

		public override string ToString()
        {
            return this[cID];
        }

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
        }

        public DBTorrentSearch()
			: base(cTableName, TableFields)
        {
        }

        public DBTorrentSearch(String sName)
			: base(cTableName, TableFields)
        {
            ReadPrimary(sName);
        }

        public static void Clear(SQLCondition conditions)
        {
            Clear(DBTorrentSearch.cTableName, conditions);
        }

        public static void GlobalSet(String sKey, DBValue Value)
        {
            GlobalSet(sKey, Value, new SQLCondition());
        }

        public static void GlobalSet(String sKey, DBValue Value, SQLCondition condition)
        {
            GlobalSet(DBTorrentSearch.TableFields, sKey, Value, condition);
        }

        public static void GlobalSet(String sKey1, String sKey2)
        {
            GlobalSet(sKey1, sKey2, new SQLCondition());
        }

        public static void GlobalSet(String sKey1, String sKey2, SQLCondition condition)
        {
			GlobalSet(DBTorrentSearch.TableFields, sKey1, sKey2, condition);
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