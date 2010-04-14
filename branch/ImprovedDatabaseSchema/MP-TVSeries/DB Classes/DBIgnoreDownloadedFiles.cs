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
    public class DBIgnoredDownloadedFiles : DBTable
    {
        public const String cTableName = "ignored_downloaded_files";

		#region Local DB Fields
		//declare fieldsnames as constants here, and then add them to TableFields
		public const String cFilename = "filename";

		// all mandatory fields. Place the primary key first - it's just good manners
		public static DBFieldDefList TableFields = new DBFieldDefList {
                    {cFilename,    new DBFieldDef{ FieldName = cFilename,    TableName = cTableName, Type = DBFieldType.String,     Primary = true }}
        };
		#endregion

		public override string ToString()
        {
            return this[cFilename];
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
				MPTVSeriesLog.Write("Error Maintaining the " + cTableName + " Table");
			}
		}
		
		public DBIgnoredDownloadedFiles()
			: base(cTableName, TableFields)
        {
        }

        public DBIgnoredDownloadedFiles(String sName)
			: base(cTableName, TableFields)
        {
            ReadPrimary(sName);
        }

        public static void ClearAll()
        {
        	const string sqlQuery = "delete from " + cTableName;
        	DBTVSeries.Execute(sqlQuery);
        }

    	public static List<DBIgnoredDownloadedFiles> Get()
        {
            // create table if it doesn't exist already
            const string sqlQuery = "select * from " + cTableName;
            SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
            List<DBIgnoredDownloadedFiles> outList = new List<DBIgnoredDownloadedFiles>();
            if (results.Rows.Count > 0)
            {
                for (int index = 0; index < results.Rows.Count; index++)
                {
                    DBIgnoredDownloadedFiles outItem = new DBIgnoredDownloadedFiles();
                    outItem.Read(ref results, index);
                    outList.Add(outItem);
                }
            }
            return outList;
        }
    }
}