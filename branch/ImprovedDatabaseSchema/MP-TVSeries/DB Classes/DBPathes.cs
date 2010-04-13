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
	public class DBImportPath : DBTable
	{
		public const String cTableName = "ImportPathes";

		#region Local DB Fields
		//declare fieldsnames as constants here, and then add them to TableFields
		public const string cIndex = "ID";
		public const string cEnabled = "enabled";
		public const string cPath = "Path";
		public const string cRemovable = "removable";
		public const string cKeepReference = "keep_references";

		// all mandatory fields. Place the primary key first - it's just good manners
		public static readonly DBFieldDefList TableFields = new DBFieldDefList { 
			{cIndex,			new DBFieldDef { FieldName = cIndex,		Type = DBFieldType.Int,			Primary = true }},
			{cEnabled,			new DBFieldDef { FieldName = cEnabled,		Type = DBFieldType.Int }},
			{cPath,				new DBFieldDef { FieldName = cPath,			Type = DBFieldType.String }},
			{cRemovable,		new DBFieldDef { FieldName = cRemovable,	Type = DBFieldType.Int }},
			{cKeepReference,	new DBFieldDef { FieldName = cKeepReference,Type = DBFieldType.Int }}
		};
		#endregion

		public static bool includesNetworkShares { get; private set; }

        public DBImportPath()
            : base(cTableName)
        {
        }

        public DBImportPath(long ID)
            : base(cTableName)
        {
            ReadPrimary(ID.ToString());
        }

		internal static void MaintainDatabaseTable(Version lastVersion)
		{
			try {
				//test for table existance
				if (!DatabaseHelper.TableExists(cTableName)) {
					DatabaseHelper.CreateTable(cTableName, TableFields.Values);
				}
			} catch (Exception) {
				MPTVSeriesLog.Write("Unable to Correctly Maintain the " + cTableName + " Table");
			}
		}
		
		protected override void InitColumns()
        {
            AddColumns(TableFields.Values);
        }

        public override void InitValues()
        {
            InitValues(-1, "");
        }

        public static void ClearAll()
        {
            DBTVSeries.Execute("delete from " + cTableName);
            includesNetworkShares = false;
        }

        public static DBImportPath[] GetAll()
        {
            try
            {
                // make sure the table is created - create a dummy object
                DBImportPath dummy = new DBImportPath();

                // retrieve all fields in the table
                const string sqlQuery = "select * from " + cTableName + " order by " + cIndex;
                SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
                if (results.Rows.Count > 0)
                {
                    DBImportPath[] importPathes = new DBImportPath[results.Rows.Count];
                    for (int index = 0; index < results.Rows.Count; index++)
                    {
                        importPathes[index] = new DBImportPath();
                        importPathes[index].Read(ref results, index);                        
                    }
                    return importPathes;
                }
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Error in DBImportPath.Get (" + ex.Message + ").");
            }
            return null;
        }
    }
}