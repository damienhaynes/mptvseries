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
    public class DBImportPath : DBTable
    {
        public const String cTableName = "ImportPathes";

        public const String cIndex = "ID";
        public const String cEnabled = "enabled";
        public const String cPath = "Path";
        public const String cRemovable = "removable";

        private static bool _includesNetworkShares;
        public static bool includesNetworkShares
        {
            get { return _includesNetworkShares; }
        }

        public DBImportPath()
            : base(cTableName)
        {
            InitColumns();
            InitValues();
        }

        public DBImportPath(long ID)
            : base(cTableName)
        {
            InitColumns();
            if (!ReadPrimary(ID.ToString()))
                InitValues();
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
            AddColumn(cIndex, new DBField(DBField.cTypeInt, true));
            AddColumn(cEnabled, new DBField(DBField.cTypeInt));
            AddColumn(cPath, new DBField(DBField.cTypeString));
            AddColumn(cPath, new DBField(DBField.cTypeInt));
        }

        public static void ClearAll()
        {
            String sqlQuery = "delete from " + cTableName;
            DBTVSeries.Execute(sqlQuery);
            _includesNetworkShares = false;
        }

        public static DBImportPath[] GetAll()
        {
            try
            {
                // make sure the table is created - create a dummy object
                DBImportPath dummy = new DBImportPath();

                // retrieve all fields in the table
                String sqlQuery = "select * from " + cTableName + " order by " + cIndex;
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