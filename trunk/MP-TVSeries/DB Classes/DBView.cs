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
    public class DBView : DBTable
    {
        public const String cTableName = "Views";
        public const int cDBVersion = 4;

        public const String cIndex = "ID";
        public const String cEnabled = "enabled";
        public const String cSort = "sort";
        public const String cTransToken = "TransToken";
        public const String cPrettyName = "PrettyName";
        public const String cViewConfig = "viewConfig";
        public const String cTaggedView = "TaggedView";

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
            AddColumn(cTransToken, new DBField(DBField.cTypeString));
            AddColumn(cPrettyName, new DBField(DBField.cTypeString));
            AddColumn(cViewConfig, new DBField(DBField.cTypeString));
            AddColumn(cTaggedView, new DBField(DBField.cTypeInt));
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
                SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
                if (results.Rows.Count > 0)
                {
                    DBView[] views = new DBView[results.Rows.Count];
                    for (int index = 0; index < results.Rows.Count; index++)
                    {
                        views[index] = new DBView();
                        views[index].Read(ref results, index);
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

        public static DBView[] getTaggedViews() {
            try {
                // Make sure the table is created - create a dummy object
                DBView dummy = new DBView();

                // Only get Tagged Views                
                String sqlQuery = "select * from " + cTableName + " where " + cTaggedView + " = 1";

                SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
                if (results.Rows.Count > 0) {
                    DBView[] views = new DBView[results.Rows.Count];
                    for (int index = 0; index < results.Rows.Count; index++) {
                        views[index] = new DBView();
                        views[index].Read(ref results, index);
                    }
                    return views;
                }
            }
            catch (Exception ex) {
                MPTVSeriesLog.Write("Error in retrieving Tagged Views (" + ex.Message + ").");
            }
            return new DBView[0];
        }

        static DBView()
        {
            fillDefaults();
        }

        public static void AddView(int index, string name, string config, bool tagview) {
            DBView view = new DBView();
            view[cIndex] = index;
            view[cEnabled] = "1";
            view[cSort] = index + 1;
            view[cTransToken] = name;
            view[cPrettyName] = name;
            view[cViewConfig] = config;
            view[cTaggedView] = tagview;
            view.Commit();
            return;
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
                view[cTaggedView] = "0";
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
                view[cTaggedView] = "0";
                view.Commit();

                view = new DBView();
                view[cIndex] = "2";
                view[cEnabled] = "1";
                view[cSort] = "2";
                view[cTransToken] = "Unwatched";
                view[cPrettyName] = "";
                view[cViewConfig] = @"series<;><Episode.Watched>;=;0<;><;>" +
                                    "<nextStep>season<;><;><Season.seasonIndex>;asc<;>" +
                                    "<nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                view[cTaggedView] = "0";
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
                view[cTaggedView] = "0";
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
                view[cTaggedView] = "0";
                view.Commit();

				view = new DBView();
				view[cIndex] = "5";
				view[cEnabled] = "1";
				view[cSort] = "6";
				view[cTransToken] = "ContentRating";
				view[cPrettyName] = "";
				view[cViewConfig] = @"group:<Series.ContentRating><;><;><;>" +
									"<nextStep>series<;><;><;>" +
									"<nextStep>season<;><;><Season.seasonIndex>;asc<;>" +
									"<nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
				view[cTaggedView] = "0";
				view.Commit();

				view = new DBView();
				view[cIndex] = "6";
				view[cEnabled] = "1";
				view[cSort] = "7";
				view[cTransToken] = "ViewTags";
				view[cPrettyName] = "";
				view[cViewConfig] = @"group:<Series.ViewTags><;><;><;>" +
									"<nextStep>series<;><;><;>" +
									"<nextStep>season<;><;><Season.seasonIndex>;asc<;>" +
									"<nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
				view[cTaggedView] = "0";
				view.Commit();

                view = new DBView();
                view[cIndex] = "7";
                view[cEnabled] = "1";
                view[cSort] = "8";
                view[cTransToken] = "Latest";
                view[cPrettyName] = "";
                view[cViewConfig] = @"episode<;><Episode.FirstAired>;<=;<today><cond><Episode.FirstAired>;>=;<today-30><;><Episode.FirstAired>;desc<;>";
                view[cTaggedView] = "0";
                view.Commit();

                view = new DBView();
                view[cIndex] = "8";
                view[cEnabled] = "1";
                view[cSort] = "9";
                view[cTransToken] = "RecentlyAdded";
                view[cPrettyName] = "";
                view[cViewConfig] = @"episode<;><Episode.FileDateCreated>;>=;<today-7><;><Episode.FileDateCreated>;desc<;>";
                view[cTaggedView] = "0";
                view.Commit();

            }

            int nCurrentDBVersion = cDBVersion;
            int nUpgradeDBVersion = DBOption.GetOptions(DBOption.cDBViewsVersion);

            while (nUpgradeDBVersion != nCurrentDBVersion)
            {
                // WARNING: as of version 4, we can now remove and add views. 
                // Be particularly carefull if adding/Removing/Updating

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

					case 3:
						// Upgrade to version 4, new view 'Content Rating'
						view = new DBView();
						view[cIndex] = "7";
						view[cEnabled] = "1";
						view[cSort] = "8";
						view[cTransToken] = "ContentRating";
						view[cPrettyName] = "";
						view[cViewConfig] = @"group:<Series.ContentRating><;><;><;>" +
											"<nextStep>series<;><;><;>" +
											"<nextStep>season<;><;><Season.seasonIndex>;asc<;>" +
											"<nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
						view.Commit();
						
						// New view 'View Tags'
						view = new DBView();
						view[cIndex] = "8";
						view[cEnabled] = "1";
						view[cSort] = "9";
						view[cTransToken] = "ViewTags";
						view[cPrettyName] = "";
						view[cViewConfig] = @"group:<Series.ViewTags><;><;><;>" +
											"<nextStep>series<;><;><;>" +
											"<nextStep>season<;><;><Season.seasonIndex>;asc<;>" +
											"<nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";

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
