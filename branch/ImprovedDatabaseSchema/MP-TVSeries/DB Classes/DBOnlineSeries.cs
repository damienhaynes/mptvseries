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
using WindowPlugins.GUITVSeries.DataBase;

namespace WindowPlugins.GUITVSeries.DataClass
{
    public class DBOnlineSeries : DBTable {
		public const String cTableName = "online_series";
		
		#region Online DB Fields
        public const String cID = "ID";
        public const String cPrettyName = "Pretty_Name";
        public const String cSortName = "SortName";
        public const String cStatus = "Status";
        public const String cGenre = "Genre";
        public const String cSummary = "Summary";
        public const String cAirsDay = "AirsDay";
        public const String cAirsTime = "AirsTime";
        public const String cActors = "Actors";
        public const String cBannerFileNames = "BannerFileNames";
        public const String cCurrentBannerFileName = "CurrentBannerFileName";
        public const String cPosterFileNames = "PosterFileNames";
        public const String cCurrentPosterFileName = "PosterBannerFileName";
        public const String cBannersDownloaded = "BannersDownloaded";
        public const String cHasLocalFiles = "HasLocalFiles";
        public const String cHasLocalFilesTemp = "HasLocalFiles_Temp";
        /// <summary>
		/// Online data imported flag values while updating:
		/// 0: the series just got an ID, the update series hasn't run on it yet
		/// 1: online data is marked as "old", and needs a refresh
		/// 2: online data up to date.
		/// </summary>
		public const String cOnlineDataImported = "OnlineDataImported";
        public const String cGetEpisodesTimeStamp = "GetEpisodesTimeStamp";
        public const String cUpdateBannersTimeStamp = "UpdateBannersTimeStamp";
        public const String cWatchedFileTimeStamp = "WatchedFileTimeStamp";
        public const String cUnwatchedItems = "UnwatchedItems";
        public const String cEpisodeCount = "EpisodeCount";
        public const String cEpisodesUnWatched = "EpisodesUnWatched";
		public const String cViewTags = "ViewTags";
		public const String cSeriesID = "SeriesID";
		public const String cFirstAired = "FirstAired";
		public const String cIsFavourite = "isFavourite";
		public const String cIsOnlineFavourite = "isOnlineFavourite";
		public const String cEpisodeOrders = "EpisodeOrders";
		public const String cChoseEpisodeOrder = "choosenOrder";
		public const String cOriginalName = "origName";
		public const String cRating = "Rating";
		public const String cMyRating = "myRating";
		public const String cBanner = "banner";
		public const String cLanguage = "language";
		public const String cIMDBID = "IMDB_ID";
		public const String cZap2ITID = "zap2it_id";
		public const String cContentRating = "ContentRating";
		public const String cNetworkID = "NetworkID";
		public const String cAdded = "added";
		public const String cAddedBy = "addedBy";
		public const String cFanart = "fanart";
		public const String cLastUpdated = "lastupdated";
		public const String cPoster = "poster";

		//unsed field constants
		//public const String cTaggedToDownload = "taggedToDownload"; - no longer seems to be used execept in old upgrade code

		// all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
		public static readonly DBFieldDefList TableFields = new DBFieldDefList {
            {cID,					new DBFieldDef{FieldName = cID,					TableName = cTableName,	Type = DBFieldType.Int,		Primary = true,		
				PrettyName = "Online Series ID"}},
            {cPrettyName,			new DBFieldDef{FieldName = cPrettyName,			TableName = cTableName,	Type = DBFieldType.String,	PrettyName = "Title"}},
            {cSortName,				new DBFieldDef{FieldName = cSortName,			TableName = cTableName,	Type = DBFieldType.String,	PrettyName = "Sort By"}},
            {cOriginalName,			new DBFieldDef{FieldName = cOriginalName,		TableName = cTableName,	Type = DBFieldType.String}},
            {cStatus,				new DBFieldDef{FieldName = cStatus,				TableName = cTableName,	Type = DBFieldType.String, PrettyName = "Show Status"}},
            {cGenre,				new DBFieldDef{FieldName = cGenre,				TableName = cTableName,	Type = DBFieldType.String}},
            {cBannerFileNames,		new DBFieldDef{FieldName = cBannerFileNames,	TableName = cTableName,	Type = DBFieldType.String}},
            {cCurrentBannerFileName,new DBFieldDef{FieldName = cCurrentBannerFileName,TableName = cTableName,	Type = DBFieldType.String}},
            {cPosterFileNames,		new DBFieldDef{FieldName = cPosterFileNames,	TableName = cTableName,	Type = DBFieldType.String}},
            {cCurrentPosterFileName,new DBFieldDef{FieldName = cCurrentPosterFileName,TableName = cTableName,	Type = DBFieldType.String}},
            {cSummary,				new DBFieldDef{FieldName = cSummary,			TableName = cTableName,	Type = DBFieldType.String, PrettyName = "Show Overview"}},
            {cOnlineDataImported,	new DBFieldDef{FieldName = cOnlineDataImported,	TableName = cTableName,	Type = DBFieldType.Int}},
            {cAirsDay,				new DBFieldDef{FieldName = cAirsDay,			TableName = cTableName,	Type = DBFieldType.String, PrettyName = "Aired Day"}},
            {cAirsTime,				new DBFieldDef{FieldName = cAirsTime,			TableName = cTableName,	Type = DBFieldType.String, PrettyName = "Aired Time"}},
            {cActors,				new DBFieldDef{FieldName = cActors,				TableName = cTableName,	Type = DBFieldType.String}},
            {cBannersDownloaded,	new DBFieldDef{FieldName = cBannersDownloaded,	TableName = cTableName,	Type = DBFieldType.Int}},
            {cHasLocalFiles,		new DBFieldDef{FieldName = cHasLocalFiles,		TableName = cTableName,	Type = DBFieldType.Int}},
            {cHasLocalFilesTemp,	new DBFieldDef{FieldName = cHasLocalFilesTemp,	TableName = cTableName,	Type = DBFieldType.Int}},
            {cGetEpisodesTimeStamp,	new DBFieldDef{FieldName = cGetEpisodesTimeStamp,TableName = cTableName,	Type = DBFieldType.Int}},
            {cUpdateBannersTimeStamp,new DBFieldDef{FieldName = cUpdateBannersTimeStamp,TableName = cTableName, Type = DBFieldType.Int}},           
            {cWatchedFileTimeStamp,	new DBFieldDef{FieldName = cWatchedFileTimeStamp,TableName = cTableName,	Type = DBFieldType.Int}},
            {cUnwatchedItems,		new DBFieldDef{FieldName = cUnwatchedItems,		TableName = cTableName,	Type = DBFieldType.Int}},
            {cEpisodeCount,			new DBFieldDef{FieldName = cEpisodeCount,		TableName = cTableName,	Type = DBFieldType.Int, PrettyName = "Episodes"}},
            {cEpisodesUnWatched,	new DBFieldDef{FieldName = cEpisodesUnWatched,	TableName = cTableName,	Type = DBFieldType.Int, PrettyName = "Episodes UnWatched"}},
            {cViewTags,				new DBFieldDef{FieldName = cViewTags,			TableName = cTableName,	Type = DBFieldType.String}},
            {cSeriesID,				new DBFieldDef{FieldName = cSeriesID,			TableName = cTableName,	Type = DBFieldType.String}},
            {cFirstAired,			new DBFieldDef{FieldName = cFirstAired,			TableName = cTableName,	Type = DBFieldType.String, PrettyName = "First Aired"}},
            {cIsFavourite,			new DBFieldDef{FieldName = cIsFavourite,		TableName = cTableName,	Type = DBFieldType.String}},
            {cIsOnlineFavourite,	new DBFieldDef{FieldName = cIsOnlineFavourite,	TableName = cTableName,	Type = DBFieldType.String}},
            {cEpisodeOrders,		new DBFieldDef{FieldName = cEpisodeOrders,		TableName = cTableName,	Type = DBFieldType.String, PrettyName = "Episode Orders"}},
	        {cChoseEpisodeOrder,	new DBFieldDef{FieldName = cChoseEpisodeOrder,	TableName = cTableName,	Type = DBFieldType.String, PrettyName = "Episode Order"}},
            {cRating,				new DBFieldDef{FieldName = cRating,				TableName = cTableName,	Type = DBFieldType.String}},
            {cMyRating,				new DBFieldDef{FieldName = cMyRating,			TableName = cTableName,	Type = DBFieldType.String, PrettyName = "My Rating"}},
            {cBanner,				new DBFieldDef{FieldName = cBanner,				TableName = cTableName,	Type = DBFieldType.String}},
		    {cLanguage,				new DBFieldDef{FieldName = cLanguage,			TableName = cTableName,	Type = DBFieldType.String, PrettyName = "Language"}},
            {cIMDBID,				new DBFieldDef{FieldName = cIMDBID,				TableName = cTableName,	Type = DBFieldType.String, PrettyName = "IMDB ID"}},
            {cZap2ITID,				new DBFieldDef{FieldName = cZap2ITID,			TableName = cTableName,	Type = DBFieldType.String}},
            {cContentRating,		new DBFieldDef{FieldName = cContentRating,		TableName = cTableName,	Type = DBFieldType.String, PrettyName = "Content Rating"}},
            {cNetworkID,			new DBFieldDef{FieldName = cNetworkID,			TableName = cTableName,	Type = DBFieldType.String}},
			{cAdded,				new DBFieldDef{FieldName = cAdded,				TableName = cTableName,	Type = DBFieldType.String}},
            {cAddedBy,				new DBFieldDef{FieldName = cAddedBy,			TableName = cTableName,	Type = DBFieldType.String}},
            {cFanart,				new DBFieldDef{FieldName = cFanart,				TableName = cTableName,	Type = DBFieldType.String}},
            {cLastUpdated,			new DBFieldDef{FieldName = cLastUpdated,		TableName = cTableName,	Type = DBFieldType.String}},
            {cPoster,				new DBFieldDef{FieldName = cPoster,				TableName = cTableName,	Type = DBFieldType.String}}
		};
		#endregion

        public static Dictionary<String, String> s_OnlineToFieldMap = new Dictionary<String, String>();

        static DBOnlineSeries() {
            //////////////////////////////////////////////////
            #region Local DB field mapping to Online DB
            s_OnlineToFieldMap.Add("id", cID);            
            s_OnlineToFieldMap.Add("SeriesName", cPrettyName);
            s_OnlineToFieldMap.Add("Status", cStatus);
            s_OnlineToFieldMap.Add("Genre", cGenre);
            s_OnlineToFieldMap.Add("Overview", cSummary);
            s_OnlineToFieldMap.Add("Airs_DayOfWeek", cAirsDay);
            s_OnlineToFieldMap.Add("Airs_Time", cAirsTime);
            s_OnlineToFieldMap.Add("SortName", cSortName);
            #endregion
            //////////////////////////////////////////////////
        }

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
		
		// returns a list of all series with information stored in the database. 
        public static List<DBOnlineSeries> getAllSeries() {
            List<DBValue> seriesIDs = GetSingleField(DBOnlineSeries.cID, new SQLCondition(), new DBOnlineSeries());
            List<DBOnlineSeries> rtn = new List<DBOnlineSeries>();

            foreach (DBValue currSeriesID in seriesIDs) {
                rtn.Add(new DBOnlineSeries(currSeriesID));
            }

            return rtn;
        }


        public DBOnlineSeries()
			: base(cTableName, TableFields)
        {
        }

        public DBOnlineSeries(int nSeriesID)
			: base(cTableName, TableFields)
        {
            ReadPrimary(nSeriesID);
        }

        public static void Clear(SQLCondition conditions)
        {
            Clear(new DBOnlineSeries(), conditions);
        }

        public override DBValue this[String fieldName]
        {
            get
            {
                switch (fieldName)
                {                    
                        // if the user choose a different language for the import, we don't have this as the prettyname
                    case DBOnlineSeries.cOriginalName:
                        const string origLanguage = "en";
                        if (DBOption.GetOptions(DBOption.cOnlineLanguage) == origLanguage)
                            return base[DBOnlineSeries.cPrettyName];
                        else
                        {
                            if (base[DBOnlineSeries.cOriginalName].ToString().Length > 0)
                                return base[DBOnlineSeries.cOriginalName];
                            else
                            {
                                // we need to get it
                                MPTVSeriesLog.Write("Retrieving original Series Name...");
                                UpdateSeries origParser = new UpdateSeries(base[DBOnlineSeries.cID], origLanguage);
                                if (origParser.Results.Count == 1)
                                {
                                    base[DBOnlineSeries.cOriginalName] = origParser.Results[0][DBOnlineSeries.cPrettyName];
                                    Commit(); // save for next time
                                    MPTVSeriesLog.Write("Original Series Name retrieved");
                                    return origParser.Results[0][DBOnlineSeries.cPrettyName];
                                }
                                else
                                {
                                    MPTVSeriesLog.Write("Original Series Name could not be retrieved");
                                    // something wrong
                                    return base[DBOnlineSeries.cPrettyName];
                                }
                            }
                        }
                    default:
                        return base[fieldName];
                }
            }
            set
            {
                base[fieldName] = value;
            }
        }

        /// <summary>
        /// Returns PrettyName
        /// </summary>
        /// <returns>PrettyName</returns>
        public override string ToString()
        {
            return this[cPrettyName];
        }
    };
}