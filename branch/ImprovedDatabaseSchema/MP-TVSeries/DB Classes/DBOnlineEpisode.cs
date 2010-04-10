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

namespace WindowPlugins.GUITVSeries
{
    public class DBOnlineEpisode : DBTable
    {
        public const String cTableName = "online_episodes";

        #region Online DB Fields
        public const String cCompositeID = "CompositeID";         // composite string used for primary index, based on series, season & episode
        public const String cSeriesID = "SeriesID";
        public const String cID = "EpisodeID";                    // onlineDB episodeID
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
        #endregion

        public static Dictionary<String, String> s_OnlineToFieldMap = new Dictionary<String, String>();
        public static Dictionary<String, String> s_FieldToDisplayNameMap = new Dictionary<String, String>();
        public static Dictionary<string, DBField> s_fields = new Dictionary<string,DBField>();
        
        public static List<string> FieldsRequiringSplit = new List<string>(new string[] { "Writer", "Director", "GuestStars" });

        static DBOnlineEpisode()
        {
            //////////////////////////////////////////////////
            #region Pretty Names displayed in Configuration Details Tab
            s_FieldToDisplayNameMap.Add(cID, "Episode ID");
            s_FieldToDisplayNameMap.Add(cEpisodeSummary, "Overview");
            s_FieldToDisplayNameMap.Add(cMyRating, "My Rating");
            s_FieldToDisplayNameMap.Add(cFirstAired, "Air Date");
            s_FieldToDisplayNameMap.Add(cGuestStars, "Guest Stars");
            s_FieldToDisplayNameMap.Add(cDVDEpisodeNumber, "DVD Episode Number");
            s_FieldToDisplayNameMap.Add(cDVDSeasonNumber, "DVD Season Number");
            s_FieldToDisplayNameMap.Add(cAirsAfterSeason, "Airs After Season");
            s_FieldToDisplayNameMap.Add(cAirsBeforeSeason, "Airs Before Season");
            s_FieldToDisplayNameMap.Add(cAirsBeforeEpisode, "Airs Before Episode");
            #endregion
            //////////////////////////////////////////////////

            //////////////////////////////////////////////////
            #region Local DB field mapping to Online DB
            s_OnlineToFieldMap.Add("SeasonNumber", cSeasonIndex);
            s_OnlineToFieldMap.Add("EpisodeNumber", cEpisodeIndex);
            s_OnlineToFieldMap.Add("EpisodeName", cEpisodeName);
            s_OnlineToFieldMap.Add("id", cID);
            s_OnlineToFieldMap.Add("Overview", cEpisodeSummary);
            s_OnlineToFieldMap.Add("FirstAired", cFirstAired);
            s_OnlineToFieldMap.Add("filename", cEpisodeThumbnailUrl);
            #endregion
            //////////////////////////////////////////////////

            // lately it also returns seriesID (which we already had before - but unfortunatly not in all lower case, prevents an error msg, thats all
            s_OnlineToFieldMap.Add("seriesid", cSeriesID);

            // make sure the table is created on first run
            DBOnlineEpisode dummy = new DBOnlineEpisode();
        }

        public DBOnlineEpisode()
            : base(cTableName)
        {
            InitColumns();
            InitValues();
        }

        public DBOnlineEpisode(DBValue nSeriesID, DBValue nSeasonIndex, DBValue nEpisodeIndex)
            : base(cTableName)
        {
            InitColumns();
            if (!ReadPrimary(nSeriesID + "_" + nSeasonIndex + "x" + nEpisodeIndex))
                InitValues();
            this[cSeriesID] = nSeriesID;
            this[cSeasonIndex] = nSeasonIndex;
            this[cEpisodeIndex] = nEpisodeIndex;
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
            base.AddColumn(cCompositeID, new DBField(DBFieldValueType.String, true));
            base.AddColumn(cID, new DBField(DBFieldValueType.Int));
            base.AddColumn(cSeriesID, new DBField(DBFieldValueType.Int));
            base.AddColumn(cEpisodeIndex, new DBField(DBFieldValueType.Int));
            base.AddColumn(cSeasonIndex, new DBField(DBFieldValueType.Int));
            base.AddColumn(cEpisodeName, new DBField(DBFieldValueType.String));

            base.AddColumn(cWatched, new DBField(DBFieldValueType.Int));
            base.AddColumn(cEpisodeSummary, new DBField(DBFieldValueType.String));
            base.AddColumn(cFirstAired, new DBField(DBFieldValueType.String));
            base.AddColumn(cOnlineDataImported, new DBField(DBFieldValueType.Int));
            base.AddColumn(cGuestStars, new DBField(DBFieldValueType.String));
            base.AddColumn(cDirector, new DBField(DBFieldValueType.String));
            base.AddColumn(cWriter, new DBField(DBFieldValueType.String));
            base.AddColumn(cHidden, new DBField(DBFieldValueType.Int));
            base.AddColumn(cLastUpdated, new DBField(DBFieldValueType.String));
            base.AddColumn(cDownloadPending, new DBField(DBFieldValueType.Int));
            base.AddColumn(cDownloadExpectedNames, new DBField(DBFieldValueType.String));
            base.AddColumn(cEpisodeThumbnailUrl, new DBField(DBFieldValueType.String));
            base.AddColumn(cEpisodeThumbnailFilename, new DBField(DBFieldValueType.String));

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

        public static String Q(String sField)
        {
            return cTableName + "." + sField;
        }

        public static void Clear(SQLCondition conditions)
        {
            Clear(new DBOnlineEpisode(), conditions);
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

        public override DBValue  this[string fieldName]
        {
            get
            {
                return base[fieldName];
            }
            set
            {
                base[fieldName] = value;
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