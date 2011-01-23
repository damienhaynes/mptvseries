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

namespace WindowPlugins.GUITVSeries
{
    public class DBOnlineSeries : DBTable {
        #region Online DB Fields
        public const String cTableName = "online_series";

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
        public const String cOnlineDataImported = "OnlineDataImported";
        // Online data imported flag values while updating:
        // 0: the series just got an ID, the update series hasn't run on it yet
        // 1: online data is marked as "old", and needs a refresh
        // 2: online data up to date.
        public const String cGetEpisodesTimeStamp = "GetEpisodesTimeStamp";
        public const String cUpdateBannersTimeStamp = "UpdateBannersTimeStamp";
        public const String cWatchedFileTimeStamp = "WatchedFileTimeStamp";

        public const String cIsFavourite = "isFavourite";
        public const String cIsOnlineFavourite = "isOnlineFavourite";
        public const String cUnwatchedItems = "UnwatchedItems";

        public const String cEpisodeOrders = "EpisodeOrders";
        public const String cChosenEpisodeOrder = "choosenOrder";
        public const String cEpisodeSortOrder = "EpisodeSortOrder";

        public const String cOriginalName = "origName";

        public const String cTaggedToDownload = "taggedToDownload";

        public const String cRating = "Rating";
        public const String cMyRating = "myRating";
        public const String cRatingCount = "RatingCount";

        public const String cEpisodeCount = "EpisodeCount";
        public const String cEpisodesUnWatched = "EpisodesUnWatched";

        public const String cFirstAired = "FirstAired";

        public const String cViewTags = "ViewTags";

        public const String cSeriesID = "SeriesID";
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

        public const String cHasNewEpisodes = "HasNewEpisodes";

        public const String cTraktIgnore = "TraktIgnore";

        #endregion

        public const int cDBVersion = 3;

        public static Dictionary<String, String> s_FieldToDisplayNameMap = new Dictionary<String, String>();
        public static Dictionary<String, String> s_OnlineToFieldMap = new Dictionary<String, String>();
        public static Dictionary<string, DBField> s_fields = new Dictionary<string, DBField>();

        static DBOnlineSeries() {
            ///////////////////////////////////////////////////
            #region Pretty Names displayed in Configuration Details Tab
            s_FieldToDisplayNameMap.Add(cID, "Online Series ID");
            s_FieldToDisplayNameMap.Add(cPrettyName, "Title");
            s_FieldToDisplayNameMap.Add(cStatus, "Show Status");
            s_FieldToDisplayNameMap.Add(cGenre, "Genre");
            s_FieldToDisplayNameMap.Add(cSummary, "Show Overview");            
            s_FieldToDisplayNameMap.Add(cAirsDay, "Aired Day");
            s_FieldToDisplayNameMap.Add(cAirsTime, "Aired Time");
            s_FieldToDisplayNameMap.Add(cSortName, "Sort By");
            s_FieldToDisplayNameMap.Add(cLanguage, "Language");
            s_FieldToDisplayNameMap.Add(cIMDBID, "IMDB ID");
            s_FieldToDisplayNameMap.Add(cEpisodeOrders, "Episode Orders");
            s_FieldToDisplayNameMap.Add(cChosenEpisodeOrder, "Episode Order");
            s_FieldToDisplayNameMap.Add(cContentRating, "Content Rating");
            s_FieldToDisplayNameMap.Add(cMyRating, "My Rating");
            s_FieldToDisplayNameMap.Add(cFirstAired, "First Aired");
            s_FieldToDisplayNameMap.Add(cEpisodeCount, "Episodes");
            s_FieldToDisplayNameMap.Add(cEpisodesUnWatched, "Episodes UnWatched");            
            #endregion
            ///////////////////////////////////////////////////

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

            // make sure the table is created on first run
            DBOnlineSeries dummy = new DBOnlineSeries();           

        }

        // returns a list of all series with information stored in the database. 
        public static List<DBOnlineSeries> getAllSeries() {
            List<DBValue> seriesIDs = DBOnlineSeries.GetSingleField(DBOnlineSeries.cID, new SQLCondition(), new DBOnlineSeries());
            List<DBOnlineSeries> rtn = new List<DBOnlineSeries>();

            foreach (DBValue currSeriesID in seriesIDs) {
                rtn.Add(new DBOnlineSeries(currSeriesID));
            }

            return rtn;
        }


        public DBOnlineSeries()
            : base(cTableName)
        {
            InitColumns();
            InitValues();
        }

        public DBOnlineSeries(int nSeriesID)
            : base(cTableName)
        {
            InitColumns();
            if (!ReadPrimary(nSeriesID))
                InitValues();
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
            base.AddColumn(cID, new DBField(DBField.cTypeInt, true));
            base.AddColumn(cPrettyName, new DBField(DBField.cTypeString));
            base.AddColumn(cSortName, new DBField(DBField.cTypeString));
            base.AddColumn(cOriginalName, new DBField(DBField.cTypeString));
            base.AddColumn(cStatus, new DBField(DBField.cTypeString));
            base.AddColumn(cGenre, new DBField(DBField.cTypeString));
            base.AddColumn(cBannerFileNames, new DBField(DBField.cTypeString));
            base.AddColumn(cCurrentBannerFileName, new DBField(DBField.cTypeString));
            base.AddColumn(cPosterFileNames, new DBField(DBField.cTypeString));
            base.AddColumn(cCurrentPosterFileName, new DBField(DBField.cTypeString));
            base.AddColumn(cSummary, new DBField(DBField.cTypeString));
            base.AddColumn(cOnlineDataImported, new DBField(DBField.cTypeInt));
            base.AddColumn(cAirsDay, new DBField(DBField.cTypeString));
            base.AddColumn(cAirsTime, new DBField(DBField.cTypeString));
            base.AddColumn(cActors, new DBField(DBField.cTypeString));
            base.AddColumn(cEpisodeOrders, new DBField(DBField.cType.String));
            base.AddColumn(cChosenEpisodeOrder, new DBField(DBField.cType.String));
            base.AddColumn(cEpisodeSortOrder, new DBField(DBField.cType.String));
            base.AddColumn(cBannersDownloaded, new DBField(DBField.cTypeInt));
            base.AddColumn(cHasLocalFiles, new DBField(DBField.cTypeInt));
            base.AddColumn(cHasLocalFilesTemp, new DBField(DBField.cTypeInt));
            base.AddColumn(cGetEpisodesTimeStamp, new DBField(DBField.cTypeInt));
            base.AddColumn(cUpdateBannersTimeStamp, new DBField(DBField.cTypeInt));           
            base.AddColumn(cWatchedFileTimeStamp, new DBField(DBField.cTypeInt));
            base.AddColumn(cUnwatchedItems, new DBField(DBField.cTypeInt));
            base.AddColumn(cEpisodeCount, new DBField(DBField.cTypeInt));
            base.AddColumn(cEpisodesUnWatched, new DBField(DBField.cTypeInt));
            base.AddColumn(cViewTags, new DBField(DBField.cTypeString));
            base.AddColumn(cTraktIgnore, new DBField(DBField.cTypeInt));

            foreach (KeyValuePair<String, DBField> pair in m_fields)
            {
                if (!s_fields.ContainsKey(pair.Key))
                    s_fields.Add(pair.Key, pair.Value);
            }
        }

        public override bool AddColumn(string sName, DBField field)
        {
            if (!s_fields.ContainsKey(sName))
            {
                s_fields.Add(sName, field);
                return base.AddColumn(sName, field);
            }
            else
            {
                // we globally know about this key already, so don't call the base
                if (!m_fields.ContainsKey(sName))
                    m_fields.Add(sName, field);
                return false;
            }
        }

        public static new String Q(String sField)
        {
            return cTableName + "." + sField;
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
                        string origLanguage = "en"; // English (original)
                        DBValue currentTitle = base[DBOnlineSeries.cPrettyName];

                        if (DBOption.GetOptions(DBOption.cOnlineLanguage) == origLanguage)
                            return base[DBOnlineSeries.cPrettyName];
                        else
                        {
                            if (base[DBOnlineSeries.cOriginalName].ToString().Length > 0)
                                return base[DBOnlineSeries.cOriginalName];
                            else
                            {
                                // we need to get it
                                MPTVSeriesLog.Write("Retrieving Original Series Name for '{0}'", currentTitle);
                                UpdateSeries origParser = new UpdateSeries(base[DBOnlineSeries.cID], origLanguage);
                                if (origParser != null && origParser.Results.Count == 1)
                                {
                                    DBValue origTitle = origParser.Results[0][DBOnlineSeries.cPrettyName];

                                    // there may not be an english title, so localized title is the original name
                                    origTitle = string.IsNullOrEmpty(origTitle) ? base[DBOnlineSeries.cPrettyName] : origTitle;

                                    // save for next time
                                    base[DBOnlineSeries.cOriginalName] = origTitle;
                                    Commit();

                                    MPTVSeriesLog.Write("Original Series Name retrieved: '{0}'", origTitle);
                                    return origTitle;
                                }
                                else
                                {
                                    // something wrong
                                    MPTVSeriesLog.Write("Original Series Name could not be retrieved");
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