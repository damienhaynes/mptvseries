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
    public class DBSeason : DBTable, cache.ICacheable<DBSeason>
    {
        public static void overRide(DBSeason old, DBSeason newObject)
        {
            old = newObject;
        }

        public DBSeason fullItem
        {
            get { return this; }
            set { overRide(this, value); }
        }
        public const String cTableName = "season";
        public const String cOutName = "Season";
        public const int cDBVersion = 3;

        public const String cID = "ID"; // local name, unique (it's the primary key) which is a composite of the series name & the season index
        public const String cSeriesID = "SeriesID";
        public const String cIndex = "SeasonIndex";
        public const String cBannerFileNames = "BannerFileNames";
        public const String cCurrentBannerFileName = "CurrentBannerFileName";
        public const String cHasLocalFiles = "HasLocalFiles";
        public const String cHasLocalFilesTemp = "HasLocalFilesTemp";
        public const String cHasEpisodes = "HasOnlineEpisodes";
        public const String cHasEpisodesTemp = "HasOnlineEpisodesTemp";
        public const String cHidden = "Hidden";

        public const String cForomSubtitleRoot = "ForomSubtitleRoot";
        public const String cUnwatchedItems = "UnwatchedItems";

        public static Dictionary<String, String> s_FieldToDisplayNameMap = new Dictionary<String, String>();

        public delegate void dbSeasonUpdateOccuredDelegate(DBSeason updated);
        public static event dbSeasonUpdateOccuredDelegate dbSeasonUpdateOccured;

        public List<string> cachedLogoResults = null;

        static DBSeason()
        {
            DBSeason dummy = new DBSeason();

            s_FieldToDisplayNameMap.Add(cID, "Composite Season ID");
            s_FieldToDisplayNameMap.Add(cSeriesID, "Series ID");
            s_FieldToDisplayNameMap.Add(cIndex, "Season Index");
            s_FieldToDisplayNameMap.Add(cBannerFileNames, "Banner FileName List");
            s_FieldToDisplayNameMap.Add(cCurrentBannerFileName, "Current Banner FileName");

            int nCurrentDBSeasonVersion = cDBVersion;
            while (DBOption.GetOptions(DBOption.cDBSeasonVersion) != nCurrentDBSeasonVersion)
                // take care of the upgrade in the table
                switch ((int)DBOption.GetOptions(DBOption.cDBSeasonVersion))
                {
                    case 1:
                        // upgrade to version 2; clear the season table (series table format changed)
                        try
                        {
                            String sqlQuery = "DROP TABLE season";
                            DBTVSeries.Execute(sqlQuery);
                            DBOption.SetOptions(DBOption.cDBSeasonVersion, nCurrentDBSeasonVersion);
                        }
                        catch {}
                        break;

                    case 2:
                        DBSeason.GlobalSet(DBSeason.cHidden, 0, new SQLCondition());
                        DBSeries.GlobalSet(DBOnlineSeries.cGetEpisodesTimeStamp, 0, new SQLCondition());
                        DBOption.SetOptions(DBOption.cDBSeasonVersion, nCurrentDBSeasonVersion);
                        break;

                    default:
                        break;
                }
        }

        public static String PrettyFieldName(String sFieldName)
        {
            if (s_FieldToDisplayNameMap.ContainsKey(sFieldName))
                return s_FieldToDisplayNameMap[sFieldName];
            else
                return sFieldName;
        }

        public DBSeason()
            : base(cTableName)
        {
            InitColumns();
            InitValues();
            // all available fields
        }

        public DBSeason(int nSeriesID, int nSeasonIndex)
            : base(cTableName)
        {
            InitColumns();
            String sSeasonID = nSeriesID + "_s" + nSeasonIndex;
            if (!ReadPrimary(sSeasonID))
            {
                InitValues();
                // set the parent series so that banners will be refreshed from scratched
                DBOnlineSeries series = new DBOnlineSeries(nSeriesID);
                series[DBOnlineSeries.cBannersDownloaded] = 0;
                series.Commit();
            }
            this[cSeriesID] = nSeriesID;
            this[cIndex] = nSeasonIndex;
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
            AddColumn(cID, new DBField(DBField.cTypeString, true));
            AddColumn(cSeriesID, new DBField(DBField.cTypeInt));
            AddColumn(cIndex, new DBField(DBField.cTypeInt));
            AddColumn(cBannerFileNames, new DBField(DBField.cTypeString));
            AddColumn(cCurrentBannerFileName, new DBField(DBField.cTypeString));
            AddColumn(cHasLocalFiles, new DBField(DBField.cTypeInt));
            AddColumn(cHasLocalFilesTemp, new DBField(DBField.cTypeInt));
            AddColumn(cHasEpisodes, new DBField(DBField.cTypeInt));
            AddColumn(cHasEpisodesTemp, new DBField(DBField.cTypeInt));
            AddColumn(cHidden, new DBField(DBField.cTypeInt));
            AddColumn(cForomSubtitleRoot, new DBField(DBField.cTypeString));
        }

        public void ChangeSeriesID(int nSeriesID)
        {
            DBSeason newSeason = new DBSeason();
            String sSeasonID = nSeriesID + "_s" + base[cIndex];
            if (!newSeason.ReadPrimary(sSeasonID))
            {
                foreach (String fieldName in FieldNames)
                {
                    switch (fieldName)
                    {
                        case cSeriesID:
                        case cID:
                            break;

                        default:
                            newSeason[fieldName] = base[fieldName];
                            break;
                    }
                }
                newSeason[cID] = sSeasonID;
                newSeason[cSeriesID] = nSeriesID;
                newSeason.Commit();
            }
        }

        public String Banner
        {
            get
            {
                if (DBOption.GetOptions(DBOption.cRandomBanner) == true) return getRandomBanner(BannerList);
                if (base[cCurrentBannerFileName] == String.Empty)
                    return String.Empty;

                //if (base[cCurrentBannerFileName].ToString().IndexOf(Directory.GetDirectoryRoot(base[cCurrentBannerFileName])) == -1)
                //    return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\banners\" + base[cCurrentBannerFileName];
                if (base[cCurrentBannerFileName].ToString().IndexOf(Directory.GetDirectoryRoot(base[cCurrentBannerFileName])) == -1)
                    return Helper.PathCombine(Settings.GetPath(Settings.Path.banners), base[cCurrentBannerFileName]);
                else
                    return base[cCurrentBannerFileName];
            }
            set
            {
                value = value.Replace(Settings.GetPath(Settings.Path.banners), "");
                base[cCurrentBannerFileName] = value;
            }
        }
        public override DBValue this[String fieldName]
        {
            get
            {
                switch (fieldName)
                {
                    case DBSeason.cUnwatchedItems:
                        // this one is virtual
                        SQLiteResultSet results = DBTVSeries.Execute("select count(*) from online_episodes where seriesid = " + this[DBSeason.cSeriesID] + " and  seasonIndex = " + this[DBSeason.cIndex] + " and watched = 0");
                        if (results.Rows.Count > 0)
                        {
                            return results.Rows[0].fields[0];
                        }
                        else return 0;   
                    default: return base[fieldName];
                }
            }
            set
            {
                switch (fieldName)
                {
                    case DBSeason.cUnwatchedItems:
                        break;
                    default:
                        base[fieldName] = value;
                        break;
                }
            }
        }
        public List<String> BannerList
        {
            get
            {
                List<String> outList = new List<string>();
                String sList = base[cBannerFileNames];
                if (sList == String.Empty)
                    return outList;

                String[] split = sList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                //string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\banners\";
                foreach (String filename in split)
                {
                    //if (filename.IndexOf(Directory.GetDirectoryRoot(filename)) == -1)
                    //    outList.Add(path + filename);
                    //else
                    //    outList.Add(filename);
                    outList.Add(Helper.PathCombine(Settings.GetPath(Settings.Path.banners), filename));
                }
                return outList;
            }
            set
            {
                String sIn = String.Empty;
                //string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\banners\";
                for(int i=0; i<value.Count; i++)
                {
                    value[i] = value[i].Replace(Settings.GetPath(Settings.Path.banners), "");
                    if (sIn == String.Empty)
                        sIn += value[i];
                    else
                        sIn += "," + value[i];
                }
                base[cBannerFileNames] = sIn;

            }
        }

        public static void Clear(SQLCondition conditions)
        {
            Clear(new DBSeason(), conditions);
        }

        public static void GlobalSet(String sKey, DBValue Value)
        {
            GlobalSet(sKey, Value, new SQLCondition());
        }

        public static void GlobalSet(String sKey, DBValue Value, SQLCondition condition)
        {
            GlobalSet(new DBSeason(), sKey, Value, condition);
        }

        public static void GlobalSet(String sKey1, String sKey2)
        {
            GlobalSet(sKey1, sKey2, new SQLCondition());
        }

        public static void GlobalSet(String sKey1, String sKey2, SQLCondition condition)
        {
            GlobalSet(new DBSeason(), sKey1, sKey2, condition);
        }

        public static SQLCondition stdConditions
        {
            get
            {
                SQLCondition conditions = new SQLCondition();
                if(Settings.isConfig || DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles))
                    conditions.Add(new DBSeason(), cHasLocalFiles, 0, SQLConditionType.NotEqual);


                if(!Settings.isConfig) conditions.Add(new DBSeason(), cHasEpisodes, 1, SQLConditionType.Equal);

                // include hidden?
                if (!Settings.isConfig || !DBOption.GetOptions(DBOption.cShowHiddenItems))
                    conditions.Add(new DBSeason(), DBSeason.cHidden, 0, SQLConditionType.Equal);

                return conditions;
            }
        }

        public static string stdGetSQL(SQLCondition condition, bool selectFull)
        {
            string orderBy = !condition.customOrderStringIsSet
                  ? " order by " + Q(cIndex)
                  : condition.orderString;
            condition.AddCustom(stdConditions.ConditionsSQLString);
            if(selectFull)
               return "select " + new SQLWhat(new DBSeason()) + condition + orderBy + condition.limitString;
            else
               return "select " + DBSeason.cID + " from " + DBSeason.cTableName + " " + condition + orderBy + condition.limitString;
        }

        public static List<DBSeason> Get(int nSeriesID)
        {
            return Get(nSeriesID, new SQLCondition());
        }

        public static List<DBSeason> Get(SQLCondition condition)
        {
            string sqlQuery = stdGetSQL(condition, true);
            //MPTVSeriesLog.Write(sqlQuery);
            SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
            List<DBSeason> outList = new List<DBSeason>();
            if (results.Rows.Count > 0)
            {
                for (int index = 0; index < results.Rows.Count; index++)
                {
                    DBSeason season = new DBSeason();
                    season.Read(ref results, index);
                    outList.Add(season);
                }
            }
            return outList;
        }
        public static List<DBSeason> Get(int nSeriesID, SQLCondition otherConditions)
        {
            // create table if it doesn't exist already
            if(nSeriesID != default(int))
                otherConditions.Add(new DBSeason(), cSeriesID, nSeriesID, SQLConditionType.Equal);

            return Get(otherConditions);
        }
        public static new String Q(String sField)
        {
            return cTableName + "." + sField;
        }

        public override bool Commit()
        {
            if (dbSeasonUpdateOccured != null)
                dbSeasonUpdateOccured(this);
            return base.Commit();
        }
    }
}
