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
    public class DBSeason : DBTable, ICacheable<DBSeason>
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
        public const int cDBVersion = 5;
        
        #region DB Field Names
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

        public const String cEpisodeCount = "EpisodeCount";
        public const String cEpisodesUnWatched = "EpisodesUnWatched";
        #endregion

        public static Dictionary<String, String> s_FieldToDisplayNameMap = new Dictionary<String, String>();

        public delegate void dbSeasonUpdateOccuredDelegate(DBSeason updated);
        public static event dbSeasonUpdateOccuredDelegate dbSeasonUpdateOccured;

        public List<string> cachedLogoResults = null;

        static DBSeason()
        {
            DBSeason dummy = new DBSeason();

            ////////////////////////////////////////////////////////////////////////////////
            #region Pretty Names displayed in Configuration Details Tab
            s_FieldToDisplayNameMap.Add(cID, "Composite Season ID");
            s_FieldToDisplayNameMap.Add(cSeriesID, "Series ID");
            s_FieldToDisplayNameMap.Add(cIndex, "Season Index");
            s_FieldToDisplayNameMap.Add(cEpisodeCount, "Episodes");
            s_FieldToDisplayNameMap.Add(cEpisodesUnWatched, "Episodes UnWatched");
            #endregion
            ////////////////////////////////////////////////////////////////////////////////

            int nCurrentDBVersion = cDBVersion;
            int nUpgradeDBVersion = DBOption.GetOptions(DBOption.cDBSeasonVersion);
            while (nUpgradeDBVersion != nCurrentDBVersion)
            {
                SQLCondition condEmpty = new SQLCondition();
                List<DBSeason> AllSeasons = Get(condEmpty, true);                
                // take care of the upgrade in the table
                switch (nUpgradeDBVersion)
                {
                    case 1:
                        // upgrade to version 2; clear the season table (series table format changed)
                        try
                        {
                            String sqlQuery = "DROP TABLE season";
                            DBTVSeries.Execute(sqlQuery);
                            nUpgradeDBVersion++;
                        }
                        catch { }
                        break;

                    case 2:
                        DBSeason.GlobalSet(DBSeason.cHidden, 0, new SQLCondition());
                        DBSeries.GlobalSet(DBOnlineSeries.cGetEpisodesTimeStamp, 0, new SQLCondition());
                        nUpgradeDBVersion++;
                        break;

                    case 3:
                        // create the unwatcheditem value by parsin the episodes                        
                        foreach (DBSeason season in AllSeasons)
                        {
                            DBEpisode episode = DBEpisode.GetFirstUnwatched(season[DBSeason.cSeriesID], season[DBSeason.cIndex]);
                            if (episode != null)
                                season[DBSeason.cUnwatchedItems] = true;
                            else
                                season[DBSeason.cUnwatchedItems] = false;
                            season.Commit();
                        }
                        nUpgradeDBVersion++;
                        break;

                    case 4:
                        // Set number of watched/unwatched episodes                                       
                        foreach (DBSeason season in AllSeasons)
                        {                           
                            int epsTotal = 0;
                            int epsUnWatched = 0;
                            DBEpisode.GetSeasonEpisodeCounts(season, out epsTotal, out epsUnWatched);
                            season[DBSeason.cEpisodeCount] = epsTotal;
                            season[DBSeason.cEpisodesUnWatched] = epsUnWatched;
                            season.Commit();
                        }
                        nUpgradeDBVersion++;
                        break;

                    default:
                        nUpgradeDBVersion = nCurrentDBVersion;
                        break;
                }
            }
            DBOption.SetOptions(DBOption.cDBSeasonVersion, nCurrentDBVersion);
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
            AddColumn(cUnwatchedItems, new DBField(DBField.cTypeInt));
            AddColumn(cEpisodeCount, new DBField(DBField.cTypeInt));
            AddColumn(cEpisodesUnWatched, new DBField(DBField.cTypeInt));
        }

        public void ChangeSeriesID(int nSeriesID)
        {
            DBSeason newSeason = new DBSeason();
            String sSeasonID = nSeriesID + "_s" + this[cIndex];
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
                            newSeason[fieldName] = this[fieldName];
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
                if (Helper.String.IsNullOrEmpty(this[cCurrentBannerFileName]))
                    return String.Empty;
                string filename;
                if (this[cCurrentBannerFileName].ToString().IndexOf(Directory.GetDirectoryRoot(this[cCurrentBannerFileName])) == -1)
                    filename = Helper.PathCombine(Settings.GetPath(Settings.Path.banners), this[cCurrentBannerFileName]);
                else
                    filename = this[cCurrentBannerFileName];
                if (System.IO.File.Exists(filename)) return filename;
                return string.Empty;
            }
            set
            {
                value = value.Replace(Settings.GetPath(Settings.Path.banners), "");
                this[cCurrentBannerFileName] = value;
            }
        }
        public override DBValue this[String fieldName]
        {
            get
            {
                switch (fieldName)
                {  
                    default: return base[fieldName];
                }
            }
            set
            {
                switch (fieldName)
                {
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
                String sList = this[cBannerFileNames];
                if (Helper.String.IsNullOrEmpty(sList))
                    return outList;

                String[] split = sList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);                
                foreach (String filename in split)
                {                    
                    outList.Add(Helper.PathCombine(Settings.GetPath(Settings.Path.banners), filename));
                }
                return outList;
            }
            set
            {
                String sIn = String.Empty;                
                for(int i=0; i<value.Count; i++)
                {
                    value[i] = value[i].Replace(Settings.GetPath(Settings.Path.banners), "");
                    if (Helper.String.IsNullOrEmpty(sIn))
                        sIn += value[i];
                    else
                        sIn += "," + value[i];
                }
                this[cBannerFileNames] = sIn;

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
                //if(!Settings.isConfig && DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles))
                //    conditions.Add(new DBSeason(), cHasLocalFiles, 0, SQLConditionType.NotEqual);


                if(!Settings.isConfig) conditions.Add(new DBSeason(), cHasEpisodes, 1, SQLConditionType.Equal);

                // include hidden?
                if (!Settings.isConfig || !DBOption.GetOptions(DBOption.cShowHiddenItems))
                    conditions.Add(new DBSeason(), DBSeason.cHidden, 0, SQLConditionType.Equal);

                if (!Settings.isConfig && DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles))
                {
                    SQLCondition fullSubCond = new SQLCondition();
                    fullSubCond.AddCustom(DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID), DBSeason.Q(DBSeason.cSeriesID), SQLConditionType.Equal);
                    fullSubCond.AddCustom(DBOnlineEpisode.Q(DBOnlineEpisode.cSeasonIndex), DBSeason.Q(DBSeason.cIndex), SQLConditionType.Equal);
                    conditions.AddCustom(" exists( " + DBEpisode.stdGetSQL(fullSubCond, false) + " )");
                }


                return conditions;
            }
        }

        public static string stdGetSQL(SQLCondition condition, bool selectFull)
        {
            return stdGetSQL(condition, selectFull, true);
        }
        public static string stdGetSQL(SQLCondition condition, bool selectFull, bool includeStdCond)
        {
            string orderBy = !condition.customOrderStringIsSet
                  ? " order by " + Q(cIndex)
                  : condition.orderString;

            if (includeStdCond)
            {
                condition.AddCustom(stdConditions.ConditionsSQLString);

                if (!Settings.isConfig)
                {
                    SQLCondition fullSubCond = new SQLCondition();
                    //fullSubCond.AddCustom(DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID), DBSeason.Q(DBSeason.cSeriesID), SQLConditionType.Equal);
                    //fullSubCond.AddCustom(DBOnlineEpisode.Q(DBOnlineEpisode.cSeasonIndex), DBSeason.Q(DBSeason.cIndex), SQLConditionType.Equal);
                    //condition.AddCustom(" season.seasonindex in ( " + DBEpisode.stdGetSQL(fullSubCond, false, true, DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID)) + " )");
                    string join = null;
                    if (DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles))
                    {
                        fullSubCond = DBEpisode.stdConditions;
                        condition.AddCustom(fullSubCond.ConditionsSQLString.Replace("where", "and"));
                        join = " left join local_episodes on season.seriesid = local_episodes.seriesid " +
                            " and season.seasonindex = local_episodes.seasonindex left join online_episodes on local_episodes.compositeid = online_episodes.compositeid ";

                    }
                    else
                    {
                        join = " left join online_episodes on season.seriesid = online_episodes.seriesid " +
                            " and season.seasonindex = online_episodes.seasonindex";
                    }
                    return "select " + new SQLWhat(new DBSeason()) + 
                        join +
                        condition  + " group by season.id " + orderBy + condition.limitString;
                }
            }

            if(selectFull)
               return "select " + new SQLWhat(new DBSeason()) + condition + orderBy + condition.limitString;
            else
               return "select " + DBSeason.cID + " from " + DBSeason.cTableName + " " + condition + orderBy + condition.limitString;
        }

        public static List<DBSeason> Get(int nSeriesID)
        {
            return Get(nSeriesID, new SQLCondition());
        }

        /// <summary>
        /// does not use StdCond
        /// </summary>
        /// <param name="seriesID"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static DBSeason getRaw(int seriesID, int index)
        {
            SQLCondition cond = new SQLCondition(new DBSeason(), cSeriesID, seriesID, SQLConditionType.Equal);
            cond.Add(new DBSeason(), cIndex, index, SQLConditionType.Equal);
            List<DBSeason> res = Get(cond, false);
            if (res.Count > 0)
                return res[0];
            else
                return null;
        }
        public static List<DBSeason> Get(SQLCondition condition)
        {
            return Get(condition, true);
        }
        public static List<DBSeason> Get(SQLCondition condition, bool includeStdCond)
        {
            string sqlQuery = stdGetSQL(condition, true, includeStdCond);
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

        public static void UpdateUnWatched(DBEpisode episode)
        {
            DBSeason season = new DBSeason(episode[DBEpisode.cSeriesID], episode[DBEpisode.cSeasonIndex]);
            DBEpisode FirstUnwatchedEpisode = DBEpisode.GetFirstUnwatched(season[DBSeason.cSeriesID], season[DBSeason.cIndex]);
            if (FirstUnwatchedEpisode != null)
                season[DBSeason.cUnwatchedItems] = true;
            else
                season[DBSeason.cUnwatchedItems] = false;
            season.Commit();
        }

        public static void UpdatedEpisodeCounts(DBSeries series, DBSeason season)
        {
            int epsTotal = 0;
            int epsUnWatched = 0;

            // Updated Season count
            DBEpisode.GetSeasonEpisodeCounts(season, out epsTotal, out epsUnWatched);
            season[DBSeason.cEpisodeCount] = epsTotal;
            season[DBSeason.cEpisodesUnWatched] = epsUnWatched;
            //UpdateUnWatched - faster than method
            if (epsUnWatched == 0) season[DBSeason.cUnwatchedItems] = false;
            else season[DBSeason.cUnwatchedItems] = true;
            season.Commit();

            // Now Update the series count
            epsTotal = 0;
            epsUnWatched = 0;

            DBEpisode.GetSeriesEpisodeCounts(series[DBSeries.cID], out epsTotal, out epsUnWatched);
            series[DBOnlineSeries.cEpisodeCount] = epsTotal;
            series[DBOnlineSeries.cEpisodesUnWatched] = epsUnWatched;
            //UpdateUnWatched - faster than method
            if (epsUnWatched == 0) series[DBOnlineSeries.cUnwatchedItems] = false;
            else series[DBOnlineSeries.cUnwatchedItems] = true;
            series.Commit();
        }
        
    }
}
