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
    public class DBReplacements : DBTable
    {
        public const String cTableName = "replacements";

        public const String cIndex = "ID";
        public const String cEnabled = "enabled";
        public const String cToReplace = "toreplace";
        public const String cWith = "with";
        public const String cBefore = "before";

        public static Dictionary<String, String> s_FieldToDisplayNameMap = new Dictionary<String, String>();

        static DBReplacements()
        {
            s_FieldToDisplayNameMap.Add(cEnabled, "Enabled");
            s_FieldToDisplayNameMap.Add(cBefore, "Run before matching");
            s_FieldToDisplayNameMap.Add(cToReplace, "Replace this..");
            s_FieldToDisplayNameMap.Add(cWith, "With this");

            DBReplacements dummy = new DBReplacements();

            DBReplacements[] replacements = DBReplacements.GetAll();
            if (replacements == null || replacements.Length == 0)
            {
                // no replacements in the db => put the default ones
                DBReplacements replacement = new DBReplacements();
                replacement[DBReplacements.cIndex] = "0";
                replacement[DBReplacements.cEnabled] = "1";
                replacement[DBReplacements.cBefore] = "0";
                replacement[DBReplacements.cToReplace] = ".";
                replacement[DBReplacements.cWith] = @"<space>";
                replacement.Commit();

                replacement[DBReplacements.cIndex] = "1";
                replacement[DBReplacements.cBefore] = "0";
                replacement[DBReplacements.cToReplace] = "_";
                replacement[DBReplacements.cWith] = @"<space>";
                replacement.Commit();

                replacement[DBReplacements.cIndex] = "2";
                replacement[DBReplacements.cBefore] = "0";
                replacement[DBReplacements.cToReplace] = "-<space>";
                replacement[DBReplacements.cWith] = @"<empty>";
                replacement.Commit();
                // to avoid being parsed as second episode 20/80
                replacement[DBReplacements.cIndex] = "3";
                replacement[DBReplacements.cBefore] = "1";
                replacement[DBReplacements.cToReplace] = "720p";
                replacement[DBReplacements.cWith] = @"<empty>";
                replacement.Commit();

                replacement[DBReplacements.cIndex] = "4";
                replacement[DBReplacements.cBefore] = "1";
                replacement[DBReplacements.cToReplace] = "1080i";
                replacement[DBReplacements.cWith] = @"<empty>";
                replacement.Commit();

                replacement[DBReplacements.cIndex] = "4";
                replacement[DBReplacements.cBefore] = "1";
                replacement[DBReplacements.cToReplace] = "1080p";
                replacement[DBReplacements.cWith] = @"<empty>";
                replacement.Commit();
            }
        }

        public static String PrettyFieldName(String sFieldName)
        {
            if (s_FieldToDisplayNameMap.ContainsKey(sFieldName))
                return s_FieldToDisplayNameMap[sFieldName];
            else
                return sFieldName;
        }

        public DBReplacements()
            : base(cTableName)
        {
            InitColumns();
            InitValues();
        }

        public DBReplacements(long ID)
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
            AddColumn(cBefore, new DBField(DBField.cTypeInt));
            AddColumn(cToReplace, new DBField(DBField.cTypeString));
            AddColumn(cWith, new DBField(DBField.cTypeString));
        }

        public static void ClearAll()
        {
            String sqlQuery = "delete from "+ cTableName;
            DBTVSeries.Execute(sqlQuery);
        }

        public static DBReplacements[] GetAll()
        {
            try
            {
                // make sure the table is created - create a dummy object

                // retrieve all fields in the table
                String sqlQuery = "select * from " + cTableName + " order by " + cIndex;
                SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
                if (results.Rows.Count > 0)
                {
                    DBReplacements[] outlist = new DBReplacements[results.Rows.Count];
                    for (int index = 0; index < results.Rows.Count; index++)
                    {
                        outlist[index] = new DBReplacements();
                        outlist[index].Read(ref results, index);
                    }
                    return outlist;
                }
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Error in DBReplacements.Get (" + ex.Message + ").");
            }
            return null;
        }
    }
}
