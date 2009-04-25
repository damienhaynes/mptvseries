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
//using SQLite.NET;
using System.Data;
//using MediaPortal.Database;

namespace WindowPlugins.GUITVSeries
{
    public class DBView : DBTable
    {
        public const String cTableName = "Views";
        public const int cDBVersion = 3;

        public const String cIndex = "ID";
        public const String cEnabled = "enabled";
        public const String cSort = "sort";
        public const String cTransToken = "TransToken";
        public const String cPrettyName = "PrettyName";
        public const String cViewConfig = "viewConfig";

        public DBView()
            : base(cTableName)
        {
            InitColumns();
            InitValues();
        }

        public DBView(long ID)
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
            AddColumn(cSort, new DBField(DBField.cTypeInt));
            AddColumn(cTransToken, new DBField(DBField.cTypeString, 50));
            AddColumn(cPrettyName, new DBField(DBField.cTypeString, 50));
            AddColumn(cViewConfig, new DBField(DBField.cTypeString, 1024));   
        }

        public static void ClearAll()
        {
            String sqlQuery = "delete from " + cTableName;
            DBTVSeries.Execute(sqlQuery);
        }

        public static DBView[] getAll(bool includeDisabled)
        {
            try
            {
                // make sure the table is created - create a dummy object
                DBView dummy = new DBView();

                // retrieve all fields in the table
                String sqlQuery = "select * from " + cTableName +
                    (includeDisabled ? string.Empty : " where " + cEnabled + " = 1")
                     + " order by " + cSort;
                DataTable results = DBTVSeries.Execute(sqlQuery);
                if (results.Rows.Count > 0)
                {
                    DBView[] views = new DBView[results.Rows.Count];
                    for (int index = 0; index < results.Rows.Count; index++)
                    {
                        views[index] = new DBView();
                        views[index].Read(results, index);
                    }
                    return views;
                }
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Error in DBView.Get (" + ex.Message + ").");
            }
            return new DBView[0];
        }

        static DBView()
        {
            fillDefaults();
        }

        public static void fillDefaults()
        {
            DBView dummy = new DBView();

            DBView[] views = DBView.getAll(true);
            if (views == null || views.Length == 0)
            {
                // no views in the db => put the default ones
                DBView view = new DBView();
                view[cIndex] = "0";
                view[cEnabled] = "1";
                view[cSort] = "1";
                view[cTransToken] = "All";
                view[cPrettyName] = "";
                view[cViewConfig] = @"series<;><;><;>" +
                                    "<nextStep>season<;><;><Season.seasonIndex>;asc<;>" +
                                    "<nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                view.Commit();

                view = new DBView();
                view[cIndex] = "1";
                view[cEnabled] = "1";
                view[cSort] = "3";
                view[cTransToken] = "Favourites";
                view[cPrettyName] = "";
                view[cViewConfig] = @"series<;><Series.isFavourite>;=;1<;><;>" +
                                    "<nextStep>season<;><;><Season.seasonIndex>;asc<;>" +
                                    "<nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                view.Commit();

                view = new DBView();
                view[cIndex] = "2";
                view[cEnabled] = "1";
                view[cSort] = "2";
                view[cTransToken] = "Unwatched";
                view[cPrettyName] = "";
                view[cViewConfig] = @"series<;><Episode.Watched>;=;0<;><;>" +
                                    "<nextStep>season<;>;;<;><Season.seasonIndex>;asc<;>" +
                                    "<nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                view.Commit();

                view = new DBView();
                view[cIndex] = "3";
                view[cEnabled] = "1";
                view[cSort] = "4";
                view[cTransToken] = "Channels";
                view[cPrettyName] = "";
                view[cViewConfig] = @"group:<Series.Network><;><;><;>" +
                                    "<nextStep>series<;><;><;>" +
                                    "<nextStep>season<;><;><Season.seasonIndex>;asc<;>" +
                                    "<nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                view.Commit();

                view = new DBView();
                view[cIndex] = "4";
                view[cEnabled] = "1";
                view[cSort] = "5";
                view[cTransToken] = "Genres";
                view[cPrettyName] = "";
                view[cViewConfig] = @"group:<Series.Genre><;><;><;>" +
                                    "<nextStep>series<;><;><;>" +
                                    "<nextStep>season<;><;><Season.seasonIndex>;asc<;>" +
                                    "<nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                view.Commit();

                view = new DBView();
                view[cIndex] = "5";
                view[cEnabled] = "1";
                view[cSort] = "6";
                view[cTransToken] = "Latest";
                view[cPrettyName] = "";
                view[cViewConfig] = @"episode<;><Episode.FirstAired>;<=;<today><cond><Episode.FirstAired>;>=;<today-30><;><Episode.FirstAired>;desc<;>";
                view.Commit();

                view = new DBView();
                view[cIndex] = "6";
                view[cEnabled] = "1";
                view[cSort] = "7";
                view[cTransToken] = "RecentlyAdded";
                view[cPrettyName] = "";
                view[cViewConfig] = @"episode<;><Episode.FileDateCreated>;>=;<today-7><;><Episode.FileDateCreated>;desc<;>";
                view.Commit();

            }

            int nCurrentDBVersion = cDBVersion;
            int nUpgradeDBVersion = DBOption.GetOptions(DBOption.cDBViewsVersion);

            while (nUpgradeDBVersion != nCurrentDBVersion)
            {
                // take care of the upgrade in the table
                switch (nUpgradeDBVersion)
                {
                    case 1:
                        //Upgrade to version 2; 'Latest' view doesn't show anything from the future, and shows only from the last 30 days
                        DBView view = new DBView(5);
                        view[cViewConfig] = @"episode<;><Episode.FirstAired>;<=;<today><cond><Episode.FirstAired>;>=;<today-30><;><Episode.FirstAired>;desc<;>";
                        view.Commit();
                        nUpgradeDBVersion++;
                        break;

                    case 2:
                        // Upgrade to version 3, new view 'Recently Added'
                        view = new DBView();
                        view[cIndex] = "6";
                        view[cEnabled] = "1";
                        view[cSort] = "7";
                        view[cTransToken] = "RecentlyAdded";
                        view[cPrettyName] = "";
                        view[cViewConfig] = @"episode<;><Episode.FileDateCreated>;>=;<today-7><;><Episode.FileDateCreated>;desc<;>";
                        view.Commit();
                        nUpgradeDBVersion++;
                        break;
   
                    default:
                        nUpgradeDBVersion = nCurrentDBVersion;
                        break;
                }
            }
            DBOption.SetOptions(DBOption.cDBViewsVersion, nCurrentDBVersion);
        }
    }
}
