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
using WindowPlugins.GUITVSeries.DataBase;

namespace WindowPlugins.GUITVSeries.DataClass
{
    public class DBOnlineEpisode : DBTable
    {
        public const String cTableName = "online_episodes";

        #region Online DB Fields
		//declare fieldsnames as constants here, and then add them to TableFields
		public const String cID = "Id";
		public const String cCompositeID = "CompositeID";         // composite string based on series, season & episode
        public const String cSeriesID = "SeriesID";
        public const String cOnlineID = "OnlineID";              // onlineDB episodeID
        public const String cSeasonIndex = "SeasonIndex";         // season index
        public const String cEpisodeIndex = "EpisodeIndex";       // episode index
        public const String cEpisodeName = "EpisodeName";         // episode name
        public const String cWatched = "Watched";                 // tag to know if episode has been watched already (overrides the local file's tag)
        public const String cEpisodeSummary = "Summary";
        public const String cFirstAired = "FirstAired";
        public const String cOnlineDataImported = "OnlineDataImported";
        public const String cGuestStars = "GuestStars";
        public const String cDirector = "Director";
        public const String cWriter = "Writer";
        public const String cHidden = "Hidden";
        public const String cLastUpdated = "lastupdated";
        public const String cDownloadPending = "DownloadPending";
        public const String cDownloadExpectedNames = "DownloadExpectedName";
        public const String cEpisodeThumbnailUrl = "ThumbUrl";
        public const String cEpisodeThumbnailFilename = "thumbFilename";
		public const String cAirsBeforeSeason = "airsbefore_season";
		public const String cAirsBeforeEpisode = "airsbefore_episode";
		public const String cAirsAfterSeason = "airsafter_season";
		public const String cRating = "Rating";
		public const String cMyRating = "myRating";
		public const String cCombinedEpisodeNumber = "Combined_episodenumber";
		public const String cCombinedSeason = "Combined_season";
		public const String cDVDChapter = "DVD_chapter";
		public const String cDVDDiscID = "DVD_discid";
		public const String cDVDEpisodeNumber = "DVD_episodenumber";
		public const String cDVDSeasonNumber = "DVD_season";
		public const String cEpisodeImageFlag = "EpImgFlag";
		public const String cIMDBID = "IMDB_ID";
		public const String cLanguage = "Language";
		public const String cProductionCode = "ProductionCode";
		public const String cAbsoluteNumber = "absolute_number";
		public const String cSeasonID = "seasonid";

		// all mandatory fields. Place the primary key first - it's just good manners
		public static DBFieldDefList TableFields = new DBFieldDefList {
            {cID,						new DBFieldDef{ FieldName = cID,					TableName = cTableName, Type = DBFieldType.Int,			
																								Primary = true,     AutoIncrement = true }},
            {cCompositeID,				new DBFieldDef{FieldName = cCompositeID,			TableName = cTableName,	Type = DBFieldType.String}},
            {cOnlineID,					new DBFieldDef{FieldName = cOnlineID,				TableName = cTableName,	Type = DBFieldType.Int,		PrettyName = "Online Episode ID"}},
            {cSeriesID,					new DBFieldDef{FieldName = cSeriesID,				TableName = cTableName,	Type = DBFieldType.Int,		Indexed = true}},
            {cEpisodeIndex,				new DBFieldDef{FieldName = cEpisodeIndex,			TableName = cTableName,	Type = DBFieldType.Int}},
            {cSeasonID,					new DBFieldDef{FieldName = cSeasonID,				TableName = cTableName,	Type = DBFieldType.String}},
            {cSeasonIndex,				new DBFieldDef{FieldName = cSeasonIndex,			TableName = cTableName,	Type = DBFieldType.Int}},
            {cEpisodeName,				new DBFieldDef{FieldName = cEpisodeName,			TableName = cTableName,	Type = DBFieldType.String}},
            {cWatched,					new DBFieldDef{FieldName = cWatched,				TableName = cTableName,	Type = DBFieldType.Int}},
            {cEpisodeSummary,			new DBFieldDef{FieldName = cEpisodeSummary,			TableName = cTableName,	Type = DBFieldType.String,	PrettyName = "Overview"}},
            {cFirstAired,				new DBFieldDef{FieldName = cFirstAired,				TableName = cTableName,	Type = DBFieldType.String,	PrettyName = "Air Date"}},
            {cOnlineDataImported,		new DBFieldDef{FieldName = cOnlineDataImported,		TableName = cTableName,	Type = DBFieldType.Int}},
            {cGuestStars,				new DBFieldDef{FieldName = cGuestStars,				TableName = cTableName,	Type = DBFieldType.String,	PrettyName = "Guest Stars"}},
            {cDirector,					new DBFieldDef{FieldName = cDirector,				TableName = cTableName,	Type = DBFieldType.String}},
            {cWriter,					new DBFieldDef{FieldName = cWriter,					TableName = cTableName,	Type = DBFieldType.String}},
            {cHidden,					new DBFieldDef{FieldName = cHidden,					TableName = cTableName,	Type = DBFieldType.Int}},
            {cLastUpdated,				new DBFieldDef{FieldName = cLastUpdated,			TableName = cTableName,	Type = DBFieldType.String}},
            {cDownloadPending,			new DBFieldDef{FieldName = cDownloadPending,		TableName = cTableName,	Type = DBFieldType.Int}},
            {cDownloadExpectedNames,	new DBFieldDef{FieldName = cDownloadExpectedNames,	TableName = cTableName,	Type = DBFieldType.String}},
            {cEpisodeThumbnailUrl,		new DBFieldDef{FieldName = cEpisodeThumbnailUrl,	TableName = cTableName,	Type = DBFieldType.String}},
            {cEpisodeThumbnailFilename,	new DBFieldDef{FieldName = cEpisodeThumbnailFilename,TableName = cTableName,	Type = DBFieldType.String}},
            {cRating,					new DBFieldDef{FieldName = cRating,					TableName = cTableName,	Type = DBFieldType.String}},
            {cMyRating,					new DBFieldDef{FieldName = cMyRating,				TableName = cTableName,	Type = DBFieldType.String}},
            {cCombinedEpisodeNumber,	new DBFieldDef{FieldName = cCombinedEpisodeNumber,	TableName = cTableName,	Type = DBFieldType.String}},
            {cCombinedSeason,			new DBFieldDef{FieldName = cCombinedSeason,			TableName = cTableName,	Type = DBFieldType.String}},
            {cDVDChapter,				new DBFieldDef{FieldName = cDVDChapter,				TableName = cTableName,	Type = DBFieldType.String}},
            {cDVDDiscID,				new DBFieldDef{FieldName = cDVDDiscID,				TableName = cTableName,	Type = DBFieldType.String}},
            {cDVDEpisodeNumber,			new DBFieldDef{FieldName = cDVDEpisodeNumber,		TableName = cTableName,	Type = DBFieldType.String}},
            {cDVDSeasonNumber,			new DBFieldDef{FieldName = cDVDSeasonNumber,		TableName = cTableName,	Type = DBFieldType.String}},
            {cEpisodeImageFlag,			new DBFieldDef{FieldName = cEpisodeImageFlag,		TableName = cTableName,	Type = DBFieldType.String}},

	        {cIMDBID,					new DBFieldDef{FieldName = cIMDBID,					TableName = cTableName,	Type = DBFieldType.String}},
            {cLanguage,					new DBFieldDef{FieldName = cLanguage,				TableName = cTableName,	Type = DBFieldType.String}},
            {cProductionCode,			new DBFieldDef{FieldName = cProductionCode,			TableName = cTableName,	Type = DBFieldType.String}},
            {cAbsoluteNumber,			new DBFieldDef{FieldName = cAbsoluteNumber,			TableName = cTableName,	Type = DBFieldType.String}},
            {cAirsBeforeSeason,			new DBFieldDef{FieldName = cAirsBeforeSeason,		TableName = cTableName,	Type = DBFieldType.String}},
            {cAirsBeforeEpisode,		new DBFieldDef{FieldName = cAirsBeforeEpisode,		TableName = cTableName,	Type = DBFieldType.String}},
            {cAirsAfterSeason,			new DBFieldDef{FieldName = cAirsAfterSeason,		TableName = cTableName,	Type = DBFieldType.String}}
		};
		#endregion

        public static Dictionary<String, String> s_OnlineToFieldMap = new Dictionary<String, String>();
		public static List<string> FieldsRequiringSplit = new List<string> { cWriter, cDirector, cGuestStars };

        static DBOnlineEpisode()
        {
            //TODO: Add Onlinefield to DBFieldDef
			//////////////////////////////////////////////////
            #region Local DB field mapping to Online DB
            s_OnlineToFieldMap.Add("SeasonNumber", cSeasonIndex);
            s_OnlineToFieldMap.Add("EpisodeNumber", cEpisodeIndex);
            s_OnlineToFieldMap.Add("EpisodeName", cEpisodeName);
            s_OnlineToFieldMap.Add("id", cOnlineID);
            s_OnlineToFieldMap.Add("Overview", cEpisodeSummary);
            s_OnlineToFieldMap.Add("FirstAired", cFirstAired);
            s_OnlineToFieldMap.Add("filename", cEpisodeThumbnailUrl);
            #endregion
            //////////////////////////////////////////////////

            // lately it also returns seriesID (which we already had before - but unfortunatly not in all lower case, prevents an error msg, thats all
            s_OnlineToFieldMap.Add("seriesid", cSeriesID);
        }

		internal static void MaintainDatabaseTable(Version lastVersion)
		{
			try {
				//test for table existance
				if (!DatabaseHelper.TableExists(cTableName)) {
					DatabaseHelper.CreateTable(cTableName, TableFields.Values);
					DatabaseHelper.CreateIndexes(cTableName, TableFields.Values);
					return;
				}
				MPTVSeriesLog.Write("Upgrading " + cTableName + " Table");

				if (lastVersion < new Version("2.6.0.1044")) {
					//needs to be done before DBTable constructor is called
					//to prevent columns being added to the database

					//rename the EpisodeID column to OnlineID so as to avoid any confusion about the new Id column
					DatabaseHelper.RenameDatabaseColumn(cTableName, "EpisodeID", cOnlineID);

					DatabaseHelper.CreateAutoIDKey(cTableName);

					//delete all the current indexes as they don't match the new naming scheme
					// - CreateAutoIDKey should have done this for us
					//DatabaseHelper.DeleteAllIndexes(cTableName);
				}
				MPTVSeriesLog.Write("Upgrade of " + cTableName + " Finished");

				DatabaseHelper.CreateIndexes(cTableName, TableFields.Values);
			} catch (Exception) {
				MPTVSeriesLog.Write("Error Maintaining the " + cTableName + " Table");
			}
		}

        public DBOnlineEpisode()
			: base(cTableName, TableFields)
        {
        }

        public DBOnlineEpisode(DBValue nSeriesID, DBValue nSeasonIndex, DBValue nEpisodeIndex)
			: base(cTableName, TableFields)
        {
            ReadComposite(nSeriesID + "_" + nSeasonIndex + "x" + nEpisodeIndex);
            this[cSeriesID] = nSeriesID;
            this[cSeasonIndex] = nSeasonIndex;
            this[cEpisodeIndex] = nEpisodeIndex;
        }

		public bool ReadComposite(string composite)
		{
			try {
				m_fields[DBOnlineEpisode.cCompositeID].Value = composite;
				SQLCondition condition = new SQLCondition();
				condition.Add(DBOnlineEpisode.TableFields, cCompositeID, m_fields[DBOnlineEpisode.cCompositeID].Value, SQLConditionType.Equal);
				String sqlQuery = "select * from " + TableName + condition;
				SQLiteResultSet records = DBTVSeries.Execute(sqlQuery);
				return Read(ref records, 0);
			} catch (Exception ex) {
				MPTVSeriesLog.Write("An Error Occurred (" + ex.Message + ").");
			}
			return false;
		}
		
		public static void Clear(SQLCondition conditions)
        {
			Clear(DBOnlineEpisode.cTableName, conditions);
        }

        public string Image
        {
            get
            {
                if (this[cEpisodeThumbnailFilename].ToString().Length > 0)
                {
                    return Helper.PathCombine(Settings.GetPath(Settings.Path.banners), this[cEpisodeThumbnailFilename]);
                } else return string.Empty;
            }
        }

        public string CompleteTitle
        {
            get
            {
                return Helper.getCorrespondingSeries(this[DBOnlineEpisode.cSeriesID]).ToString() + " " + this[DBOnlineEpisode.cSeasonIndex] + "x" + this[DBOnlineEpisode.cEpisodeIndex] + ": " + this[DBOnlineEpisode.cEpisodeName];
            }
        }

        /// <summary>
        /// Returns a pretty String representation of this DBOnlineEpisode (Series - 1x01 - Pilot)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            DBSeries s = Helper.getCorrespondingSeries(this[DBOnlineEpisode.cSeriesID]);
            return string.Format("{0} - {1}x{2} - {3}", (s == null ? string.Empty : s[DBOnlineSeries.cPrettyName].ToString()), this[DBOnlineEpisode.cSeasonIndex], this[DBOnlineEpisode.cEpisodeIndex], this[cEpisodeName]);
        }
    };
}