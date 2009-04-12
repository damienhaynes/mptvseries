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
    public class DBFormatting : DBTable
    {
        public const String cTableName = "FormattingRules";

        public const String cIndex = "ID";
        public const String cEnabled = "enabled";
        public const String cReplace = "Replace";
        public const String cWith = "With";

        public DBFormatting()
            : base(cTableName)
        {
            InitColumns();
            InitValues();
        }

        public DBFormatting(long ID)
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
            AddColumn(cReplace, new DBField(DBField.cTypeString));
            AddColumn(cWith, new DBField(DBField.cTypeString));
        }

        public static void ClearAll()
        {
            String sqlQuery = "delete from " + cTableName;
            cache = null;
            DBTVSeries.Execute(sqlQuery);
        }

        public static void Clear(int Index)
        {
            DBFormatting dummy = new DBFormatting(Index);
            Clear(dummy, new SQLCondition(dummy, DBFormatting.cIndex, Index, SQLConditionType.Equal));
            cache = null;
        }

        public void Delete()
        {
            Clear(this[cIndex]);
            cache = null;
        }

        public static DBFormatting[] cache = null; // public for config

        public static IEnumerable<DBFormatting> GetAll()
        {
            foreach (DBFormatting dbf in GetAll(true))
                yield return dbf;
        }

        public static IEnumerable<DBFormatting> GetAll(bool includeDisabled)
        {
            if (cache == null || Settings.isConfig)
            {
                try
                {
                    // make sure the table is created - create a dummy object
                    DBFormatting dummy = new DBFormatting();

                    // retrieve all fields in the table
                    String sqlQuery = "select * from " + cTableName;
                    if (!includeDisabled)
                    {
                        sqlQuery += " where " + cEnabled + " = 1";
                    }
                    sqlQuery += " order by " + cIndex;

                    SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
                    if (results.Rows.Count > 0)
                    {
                        cache = new DBFormatting[results.Rows.Count];
                        for (int index = 0; index < results.Rows.Count; index++)
                        {
                            cache[index] = new DBFormatting();
                            cache[index].Read(ref results, index);                            
                        }
                    }
                    MPTVSeriesLog.Write("Found and loaded " + results.Rows.Count + " User Formatting Rules",MPTVSeriesLog.LogLevel.Debug);
                    if (results.Rows.Count == 0) cache = new DBFormatting[0];
                    
                }
                catch (Exception ex)
                {
                    MPTVSeriesLog.Write("Error in DBFormatting.Get (" + ex.Message + ").");
                }
            }
            if(cache != null)
                for (int i = 0; i < cache.Length; i++) yield return cache[i];

        }

        public override string ToString()
        {
            return this[cReplace] + " -> " + this[cWith];
        }
    }
}