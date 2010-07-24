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
using System.IO;
using WindowPlugins.GUITVSeries.DataBase;

namespace WindowPlugins.GUITVSeries.DataClass
{
    public class DBSeason : DBTable, ICacheable<DBSeason>
    {
		private static void overRide(DBSeason old, DBSeason newObject)
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
        
        #region DB Field Names
		/// <summary>
		/// local name, unique (it's the primary key) which is a composite of the series name & the season index
		/// </summary>
        public const String cID = "ID";
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

        // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
		public static readonly DBFieldDefList TableFields = new DBFieldDefList {
            {cID,						new DBFieldDef{FieldName = cID,					TableName = cTableName,	Type = DBFieldType.String, 
																							Primary = true,			PrettyName = "Composite Season ID"}},
            {cSeriesID,					new DBFieldDef{FieldName = cSeriesID,			TableName = cTableName,	Type = DBFieldType.Int, PrettyName = "Series ID"}},
            {cIndex,					new DBFieldDef{FieldName = cIndex,				TableName = cTableName,	Type = DBFieldType.Int, PrettyName = "Season Index"}},
            {cBannerFileNames,			new DBFieldDef{FieldName = cBannerFileNames,	TableName = cTableName,	Type = DBFieldType.String}},
            {cCurrentBannerFileName,	new DBFieldDef{FieldName = cCurrentBannerFileName,TableName = cTableName,	Type = DBFieldType.String}},
            {cHasLocalFiles,			new DBFieldDef{FieldName = cHasLocalFiles,		TableName = cTableName,	Type = DBFieldType.Int}},
            {cHasLocalFilesTemp,		new DBFieldDef{FieldName = cHasLocalFilesTemp,	TableName = cTableName,	Type = DBFieldType.Int}},
            {cHasEpisodes,				new DBFieldDef{FieldName = cHasEpisodes,		TableName = cTableName,	Type = DBFieldType.Int}},
            {cHasEpisodesTemp,			new DBFieldDef{FieldName = cHasEpisodesTemp,	TableName = cTableName,	Type = DBFieldType.Int}},
            {cHidden,					new DBFieldDef{FieldName = cHidden,				TableName = cTableName,	Type = DBFieldType.Int}},
            {cForomSubtitleRoot,		new DBFieldDef{FieldName = cForomSubtitleRoot,	TableName = cTableName,	Type = DBFieldType.String}},
            {cUnwatchedItems,			new DBFieldDef{FieldName = cUnwatchedItems,		TableName = cTableName,	Type = DBFieldType.Int}},
            {cEpisodeCount,				new DBFieldDef{FieldName = cEpisodeCount,		TableName = cTableName,	Type = DBFieldType.Int, PrettyName = "Episodes"}},
            {cEpisodesUnWatched,		new DBFieldDef{FieldName = cEpisodesUnWatched,	TableName = cTableName,	Type = DBFieldType.Int, PrettyName = "Episodes UnWatched"}}
    	                                                                       	
		};
        #endregion

        public delegate void dbSeasonUpdateOccuredDelegate(DBSeason updated);
        public static event dbSeasonUpdateOccuredDelegate dbSeasonUpdateOccured;

        public List<string> cachedLogoResults = null;

        static DBSeason()
        {
        	DatabaseUpgrade();
        }

		#region deprecated database upgrade method - use MaintainDatabaseTable instead
		private const int cDBVersion = 5;
		/// <summary>
		/// deprecated database upgrade method - use MaintainDatabaseTable instead
		/// </summary>
    	private static void DatabaseUpgrade()
    	{
    		const int nCurrentDBVersion = cDBVersion;
    		int nUpgradeDBVersion = DBOption.GetOptions(DBOption.cDBSeasonVersion);
    		if (nUpgradeDBVersion == nCurrentDBVersion) {
    			return;
    		}
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
    						const string sqlQuery = "DROP TABLE season";
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
		#endregion

		internal static void MaintainDatabaseTable(Version lastVersion)
		{
			try {
				//test for table existance
				if (!DatabaseHelper.TableExists(cTableName)) {
					DatabaseHelper.CreateTable(cTableName, TableFields.Values);
				}
			} catch (Exception) {
				MPTVSeriesLog.Write("Error Maintaining the " + cTableName + " Table");
			}
		}

		public DBSeason()
			: base(cTableName, TableFields)
        {
        }

        public DBSeason(int nSeriesID, int nSeasonIndex)
			: base(cTableName, TableFields)
        {
            String sSeasonID = nSeriesID + "_s" + nSeasonIndex;
            if (!ReadPrimary(sSeasonID))
            {
                // set the parent series so that banners will be refreshed from scratched
                DBOnlineSeries series = new DBOnlineSeries(nSeriesID);
                series[DBOnlineSeries.cBannersDownloaded] = 0;
                series.Commit();
            }
            this[cSeriesID] = nSeriesID;
            this[cIndex] = nSeasonIndex;
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
                if (DBOption.GetOptions(DBOption.cRandomBanner) == true) return GUITVSeries.Banner.getRandomBanner(BannerList);
                if (String.IsNullOrEmpty(this[cCurrentBannerFileName]))
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
                if (String.IsNullOrEmpty(sList))
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
                    if (String.IsNullOrEmpty(sIn))
                        sIn += value[i];
                    else
                        sIn += "," + value[i];
                }
                this[cBannerFileNames] = sIn;

            }
        }

        public static void Clear(SQLCondition conditions)
        {
            Clear(DBSeason.cTableName, conditions);
        }

        public static void GlobalSet(String sKey, DBValue Value)
        {
            GlobalSet(sKey, Value, new SQLCondition());
        }

        public static void GlobalSet(String sKey, DBValue Value, SQLCondition condition)
        {
            GlobalSet(DBSeason.TableFields, sKey, Value, condition);
        }

        public static void GlobalSet(String sKey1, String sKey2)
        {
            GlobalSet(sKey1, sKey2, new SQLCondition());
        }

        public static void GlobalSet(String sKey1, String sKey2, SQLCondition condition)
        {
            GlobalSet(DBSeason.TableFields, sKey1, sKey2, condition);
        }

        public static SQLCondition stdConditions
        {
            get
            {
                SQLCondition conditions = new SQLCondition();
                //if(!Settings.isConfig && DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles))
                //    conditions.Add(new DBSeason(), cHasLocalFiles, 0, SQLConditionType.NotEqual);


                if(!Settings.isConfig) conditions.Add(DBSeason.TableFields, cHasEpisodes, 1, SQLConditionType.Equal);

                // include hidden?
                if (!Settings.isConfig || !DBOption.GetOptions(DBOption.cShowHiddenItems))
                    conditions.Add(DBSeason.TableFields, DBSeason.cHidden, 0, SQLConditionType.Equal);

                if (!Settings.isConfig && DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles))
                {
                    SQLCondition fullSubCond = new SQLCondition();
					fullSubCond.AddCustom(DBOnlineEpisode.TableFields[DBOnlineEpisode.cSeriesID].Q, DBSeason.TableFields[DBSeason.cSeriesID].Q, SQLConditionType.Equal);
					fullSubCond.AddCustom(DBOnlineEpisode.TableFields[DBOnlineEpisode.cSeasonIndex].Q, DBSeason.TableFields[DBSeason.cIndex].Q, SQLConditionType.Equal);
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
                                 ? " order by " + TableFields[DBSeason.cIndex].Q
                                 : condition.orderString;

            if (includeStdCond)
            {
                condition.AddCustom(stdConditions.ConditionsSQLString);

                if (!Settings.isConfig)
                {
                    SQLCondition fullSubCond;
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
            SQLCondition cond = new SQLCondition(DBSeason.TableFields, cSeriesID, seriesID, SQLConditionType.Equal);
            cond.Add(DBSeason.TableFields, cIndex, index, SQLConditionType.Equal);
            List<DBSeason> res = Get(cond, false);
            if (res.Count > 0)
                return res[0];
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
                otherConditions.Add(DBSeason.TableFields, cSeriesID, nSeriesID, SQLConditionType.Equal);

            return Get(otherConditions);
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

        public static void UpdateEpisodeCounts(DBSeries series, DBSeason season)
        {
            if (series == null || season == null) return;

            int epsTotal = 0;
            int epsUnWatched = 0;

            // Updated Season count
            DBEpisode.GetSeasonEpisodeCounts(season, out epsTotal, out epsUnWatched);
            season[DBSeason.cEpisodeCount] = epsTotal;
            season[DBSeason.cEpisodesUnWatched] = epsUnWatched;
            //UpdateUnWatched - faster than method
            season[DBSeason.cUnwatchedItems] = epsUnWatched != 0;
            season.Commit();

            // Now Update the series count

        	DBEpisode.GetSeriesEpisodeCounts(series[DBSeries.cID], out epsTotal, out epsUnWatched);
            series[DBOnlineSeries.cEpisodeCount] = epsTotal;
            series[DBOnlineSeries.cEpisodesUnWatched] = epsUnWatched;
            //UpdateUnWatched - faster than method
            series[DBOnlineSeries.cUnwatchedItems] = epsUnWatched != 0;
            series.Commit();
        }

        public List<string> deleteSeason(TVSeriesPlugin.DeleteMenuItems type)
        {
            List<string> resultMsg = new List<string>(); 

            // Always delete from Local episode table if deleting from disk or database
            SQLCondition condition = new SQLCondition();
			condition.Add(DBEpisode.TableFields, DBEpisode.cSeriesID, this[DBSeason.cSeriesID], SQLConditionType.Equal);
			condition.Add(DBEpisode.TableFields, DBEpisode.cSeasonIndex, this[DBSeason.cIndex], SQLConditionType.Equal);
            /* TODO will include hidden episodes as hidden attribute is only in onlineepisodes. maybe we should include it in localepisodes also..
             * if hidden episodes are excluded then the if (resultMsg.Count is wrong and should do another select to get proper count
            if (!DBOption.GetOptions(DBOption.cShowHiddenItems))
            {
                //don't include hidden seasons unless the ShowHiddenItems option is set
                condition.Add(new DBEpisode(), idden, 0, SQLConditionType.Equal);
            }
            */
            
            List<DBEpisode> episodes = DBEpisode.Get(condition, false);
            if (episodes != null)
            {
                foreach (DBEpisode episode in episodes)
                {
                    resultMsg.AddRange(episode.deleteEpisode(type));
                }

                // if there are no local episodes, we still need to delete from online table
                if (episodes.Count == 0 && type != TVSeriesPlugin.DeleteMenuItems.disk)
                {
                    condition = new SQLCondition();
                    condition.Add(DBOnlineEpisode.TableFields, DBOnlineEpisode.cSeriesID, this[DBSeason.cSeriesID], SQLConditionType.Equal);
                    condition.Add(DBOnlineEpisode.TableFields, DBOnlineEpisode.cSeasonIndex, this[DBSeason.cIndex], SQLConditionType.Equal);                    
                    DBOnlineEpisode.Clear(condition);
                }
            }

            #region Facade Remote Color
            // if we were successful at deleting all episodes of season from disk, set HasLocalFiles to false
            // note: we only do this if the database entries still exist
            if (resultMsg.Count == 0 && type == TVSeriesPlugin.DeleteMenuItems.disk)
            {
                this[cHasLocalFiles] = false;
                this.Commit();                
            }

            // if we were successful at deleting all episodes of season from disk, 
            // also check if any local episodes exist on disk for series and set HasLocalFiles to false
            if (resultMsg.Count == 0 && type != TVSeriesPlugin.DeleteMenuItems.database)
            {
                // Check Series for Local Files
                SQLCondition episodeConditions = new SQLCondition();
                episodeConditions.Add(DBEpisode.TableFields, DBEpisode.cSeriesID, this[DBSeason.cSeriesID], SQLConditionType.Equal);
                List<DBEpisode> localEpisodes = DBEpisode.Get(episodeConditions);
                if (localEpisodes.Count == 0 && !DBSeries.IsSeriesRemoved)
                {
                    DBSeries series = DBSeries.Get(this[DBSeason.cSeriesID]);                   
                    if (series != null)
                    {
                        series[DBOnlineSeries.cHasLocalFiles] = false;
                        series.Commit();
                    }
                }
            }
            #endregion

            #region Cleanup

            // if there are no error messages and if we need to delete from db
            if (resultMsg.Count == 0 && type != TVSeriesPlugin.DeleteMenuItems.disk)
            {
                condition = new SQLCondition();
                condition.Add(DBSeason.TableFields, DBSeason.cSeriesID, this[DBSeason.cSeriesID], SQLConditionType.Equal);
                condition.Add(DBSeason.TableFields, DBSeason.cIndex, this[DBSeason.cIndex], SQLConditionType.Equal);
                DBSeason.Clear(condition);
            }

            DBSeries.IsSeriesRemoved = false;
            if (type != TVSeriesPlugin.DeleteMenuItems.disk)
            {
                // If local/online episode count is zero then delete the series and all seasons
                condition = new SQLCondition();
                condition.Add(DBOnlineEpisode.TableFields, DBOnlineEpisode.cSeriesID, this[DBSeason.cSeriesID], SQLConditionType.Equal);
                episodes = DBEpisode.Get(condition, false);
                if (episodes.Count == 0)
                {
                    // Delete Seasons
                    condition = new SQLCondition();
                    condition.Add(DBSeason.TableFields, DBSeason.cSeriesID, this[DBSeason.cSeriesID], SQLConditionType.Equal);
                    DBSeason.Clear(condition);

                    // Delete Local Series
                    condition = new SQLCondition();
                    condition.Add(DBSeries.TableFields, DBSeries.cID, this[DBSeason.cSeriesID], SQLConditionType.Equal);
                    DBSeries.Clear(condition);

                    // Delete Online Series
                    condition = new SQLCondition();
                    condition.Add(DBOnlineSeries.TableFields, DBOnlineSeries.cID, this[DBSeason.cSeriesID], SQLConditionType.Equal);
                    DBOnlineSeries.Clear(condition);

                    DBSeries.IsSeriesRemoved = true;
                }
            }
            #endregion
            
            return resultMsg;
        }
        
    }
}