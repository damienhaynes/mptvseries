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
using System.Runtime.InteropServices;
using SQLite.NET;
using MediaPortal.Database;
using System.Text.RegularExpressions;

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
        
        new public static List<string> FieldsRequiringSplit = new List<string>(new string[] { "Writer", "Director", "GuestStars" });

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
            
            DBTVSeries.CreateDBIndices("create index if not exists seriesIDOnlineEp on online_episodes(SeriesID ASC)","online_episodes",true);
 
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
            base.AddColumn(cCompositeID, new DBField(DBField.cTypeString, true));
            base.AddColumn(cID, new DBField(DBField.cTypeInt));
            base.AddColumn(cSeriesID, new DBField(DBField.cTypeInt));
            base.AddColumn(cEpisodeIndex, new DBField(DBField.cTypeInt));
            base.AddColumn(cSeasonIndex, new DBField(DBField.cTypeInt));
            base.AddColumn(cEpisodeName, new DBField(DBField.cTypeString));

            base.AddColumn(cWatched, new DBField(DBField.cTypeInt));
            base.AddColumn(cEpisodeSummary, new DBField(DBField.cTypeString));
            base.AddColumn(cFirstAired, new DBField(DBField.cTypeString));
            base.AddColumn(cOnlineDataImported, new DBField(DBField.cTypeInt));
            base.AddColumn(cGuestStars, new DBField(DBField.cTypeString));
            base.AddColumn(cDirector, new DBField(DBField.cTypeString));
            base.AddColumn(cWriter, new DBField(DBField.cTypeString));
            base.AddColumn(cHidden, new DBField(DBField.cTypeInt));
            base.AddColumn(cLastUpdated, new DBField(DBField.cTypeString));
            base.AddColumn(cDownloadPending, new DBField(DBField.cTypeInt));
            base.AddColumn(cDownloadExpectedNames, new DBField(DBField.cTypeString));
            base.AddColumn(cEpisodeThumbnailUrl, new DBField(DBField.cTypeString));
            base.AddColumn(cEpisodeThumbnailFilename, new DBField(DBField.cTypeString));

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

    public class DBEpisode : DBTable, ICacheable<DBEpisode>
    {
        public static void overRide(DBEpisode old, DBEpisode newObject)
        {
            old = newObject;
        }

        public DBEpisode fullItem
        {
            get { return this; }
            set { overRide(this, value); }
        }

        public const String cTableName = "local_episodes";
        public const String cOutName = "Episode";
        public const int cDBVersion = 6;

        #region Local DB Fields
        public const String cFilename = "EpisodeFilename";
        public const String cCompositeID = DBOnlineEpisode.cCompositeID;           // composite string used for link key to online episode data
        public const String cSeriesID = DBOnlineEpisode.cSeriesID;
        public const String cSeasonIndex = DBOnlineEpisode.cSeasonIndex;
        public const String cEpisodeIndex = DBOnlineEpisode.cEpisodeIndex;

        // ids for the second episode if it's a double (and please don't ever do triple episodes)
        public const String cCompositeID2 = DBOnlineEpisode.cCompositeID + "2";
        public const String cEpisodeIndex2 = DBOnlineEpisode.cEpisodeIndex + "2";
        
        public const String cEpisodeName = "LocalEpisodeName";
        public const String cImportProcessed = "LocalImportProcessed";
        public const String cAvailableSubtitles = "AvailableSubtitles";

        public const String cAudioCodec = "AudioCodec";
        public const String cAudioBitrate = "AudioBitrate";
        public const String cAudioChannels = "AudioChannels";
        public const String cAudioTracks = "AudioTracks";
        public const String cTextCount = "TextCount";
        public const String cVideoWidth = "videoWidth";
        public const String cVideoHeight = "videoHeight";
        public const String cVideoBitRate = "VideoBitrate";
        public const String cVideoFrameRate = "VideoFrameRate";
        public const String cVideoAspectRatio = "VideoAspectRatio";
        public const String cVideoCodec = "VideoCodec";
        
        public const String cStopTime = "StopTime";

        public const String cFileSizeBytes = "FileSizeB";
        public const String cFileSize = "FileSize";

        public const String cLocalPlaytime = "localPlaytime";
        public const String cPrettyPlaytime = "PrettyLocalPlaytime";
        public const String cFilenameWOPath = "EpisodeFilenameWithoutPath";
        public const String cExtension = "ext";

        public const String cIsOnRemovable = "Removable";
        public const String cVolumeLabel = "VolumeLabel";

        public const String cFileDateCreated = "FileDateCreated";
        public const String cFileDateAdded = "FileDateAdded";
        
        public const String cIsAvailable = "IsAvailable";
        #endregion

        private DBOnlineEpisode m_onlineEpisode = null;

        public static Dictionary<String, String> s_FieldToDisplayNameMap = new Dictionary<String, String>();
        public static Dictionary<string, DBField> s_fields = new Dictionary<string, DBField>();

        public delegate void dbEpisodeUpdateOccuredDelegate(DBEpisode updated);
        public static event dbEpisodeUpdateOccuredDelegate dbEpisodeUpdateOccured;

        public static List<string> subTitleExtensions = new List<string>();

        public List<string> cachedLogoResults = null;
        public string cachedFirstLogo = null;

        public const int MAX_MEDIAINFO_RETRIES = 5;

        private static bool m_bUpdateEpisodeCount = false; // used to ensure StdConds are used while in Config mode

        static DBEpisode()
        {
            // make sure the table is created on first run
            DBEpisode dummy = new DBEpisode();

            ///////////////////////////////////////////////////
            #region Pretty Names displayed in Configuration Details Tab
            s_FieldToDisplayNameMap.Add(cFilename, "Local FileName");
            s_FieldToDisplayNameMap.Add(cCompositeID, "Composite Episode ID");
            s_FieldToDisplayNameMap.Add(cSeriesID, "Series ID");
            s_FieldToDisplayNameMap.Add(cSeasonIndex, "Season Index");
            s_FieldToDisplayNameMap.Add(cEpisodeIndex, "Episode Index");
            s_FieldToDisplayNameMap.Add(cEpisodeName, "Episode Name");
            s_FieldToDisplayNameMap.Add(cFileDateAdded, "Date Added");
            s_FieldToDisplayNameMap.Add(cFileDateCreated, "Date Created");
            s_FieldToDisplayNameMap.Add(cAvailableSubtitles, "Subtitles Available");
            s_FieldToDisplayNameMap.Add(cVideoWidth, "Video Width");
            s_FieldToDisplayNameMap.Add(cVideoHeight, "Video Height");
            s_FieldToDisplayNameMap.Add(cVideoAspectRatio, "Video Aspect Ratio");
            s_FieldToDisplayNameMap.Add(cVideoBitRate, "Video Bit Rate");
            s_FieldToDisplayNameMap.Add(cVideoCodec, "Video Codec");
            s_FieldToDisplayNameMap.Add(cVideoFrameRate, "Video Frame Rate");
            s_FieldToDisplayNameMap.Add(cAudioBitrate, "Audio Bitrate");
            s_FieldToDisplayNameMap.Add(cVolumeLabel, "Volume Label");
            s_FieldToDisplayNameMap.Add(cAudioChannels, "Audio Channels");
            s_FieldToDisplayNameMap.Add(cAudioCodec, "Audio Codec");
            s_FieldToDisplayNameMap.Add(cAudioTracks, "Audio Tracks");
            s_FieldToDisplayNameMap.Add(cTextCount, "Subtitle Count");
            #endregion
            ///////////////////////////////////////////////////

            int nCurrentDBVersion = cDBVersion;
            int nUpgradeDBVersion = DBOption.GetOptions(DBOption.cDBEpisodesVersion);

            ///////////////////////////////////////////////////
            #region Database Upgrade
            while (nUpgradeDBVersion != nCurrentDBVersion)
                // take care of the upgrade in the table
                switch (nUpgradeDBVersion)
                {
                    case 1:
                        // upgrade to version 2; clear the series table (we use 2 other tables now)
                        try
                        {
                            String sqlQuery = "DROP TABLE " + cTableName;
                            DBTVSeries.Execute(sqlQuery);
                            sqlQuery = "DROP TABLE " + DBOnlineEpisode.cTableName;
                            DBTVSeries.Execute(sqlQuery);
                            nUpgradeDBVersion++;
                        }
                        catch { }
                        break;

                    case 2:
                        DBOnlineEpisode.GlobalSet(new DBOnlineEpisode(), DBOnlineEpisode.cHidden, 0, new SQLCondition());
                        nUpgradeDBVersion++;
                        break;

                    case 3:
                        DBEpisode.GlobalSet(new DBEpisode(), DBEpisode.cEpisodeIndex2, 0, new SQLCondition());
                        nUpgradeDBVersion++;
                        break;

                    case 4:
                        DBOnlineEpisode.GlobalSet(new DBOnlineEpisode(), DBOnlineEpisode.cDownloadPending, 0, new SQLCondition());
                        nUpgradeDBVersion++;
                        break;

                    case 5:
                        DBOnlineEpisode.GlobalSet(new DBOnlineEpisode(), DBOnlineEpisode.cEpisodeThumbnailUrl, (DBValue)"init", new SQLCondition());
                        DBOnlineEpisode.GlobalSet(new DBOnlineEpisode(), DBOnlineEpisode.cEpisodeThumbnailFilename, (DBValue)"", new SQLCondition());
                        nUpgradeDBVersion++;
                        break;

                    default:
                        nUpgradeDBVersion = nCurrentDBVersion;
                        break;
                }
            DBOption.SetOptions(DBOption.cDBEpisodesVersion, nCurrentDBVersion);
            #endregion
            ///////////////////////////////////////////////////
        }

        public static String PrettyFieldName(String sFieldName)
        {
            if (DBOnlineEpisode.s_FieldToDisplayNameMap.ContainsKey(sFieldName))
                return DBOnlineEpisode.s_FieldToDisplayNameMap[sFieldName];
            if (s_FieldToDisplayNameMap.ContainsKey(sFieldName))
                return s_FieldToDisplayNameMap[sFieldName];
            else
                return sFieldName;
        }

        public DBEpisode() 
            : base(cTableName)
        {
            InitColumns();
            InitValues();
    
            DBTVSeries.CreateDBIndices("create index if not exists epComp1 ON local_episodes(CompositeID ASC)","local_episodes",false);
            DBTVSeries.CreateDBIndices("create index if not exists epComp2 ON local_episodes(CompositeID2 ASC)","local_episodes", true);

        }

        public DBEpisode(bool bCreateEmptyOnline)
            : base(cTableName)
        {
            InitColumns();
            InitValues();
            if (bCreateEmptyOnline)
                m_onlineEpisode = new DBOnlineEpisode();
        }

        public DBEpisode(DBOnlineEpisode onlineEpisode, String filename)
            : base(cTableName)
        {
            InitColumns();
            if (!ReadPrimary(filename))
                InitValues();
            //if (System.IO.File.Exists(filename) && !HasMediaInfo && !Helper.IsImageFile(filename))
            //    ReadMediaInfo();

            //composite id will bw set automatically from setting these three
            this[DBEpisode.cSeriesID] = onlineEpisode[DBOnlineEpisode.cSeriesID];
            this[DBEpisode.cSeasonIndex] = onlineEpisode[DBOnlineEpisode.cSeasonIndex];
            this[DBEpisode.cEpisodeIndex] = onlineEpisode[DBOnlineEpisode.cEpisodeIndex];
            m_onlineEpisode = onlineEpisode;
        }

        public DBEpisode(String filename, bool bSkipMediaInfo)
            : base(cTableName)
        {
            InitColumns();
            if (!ReadPrimary(filename))
                InitValues();
            
            //if (System.IO.File.Exists(filename) && !HasMediaInfo && !bSkipMediaInfo && !Helper.IsImageFile(filename))
            //    ReadMediaInfo();

            if (this[cSeriesID].ToString().Length > 0 && this[cSeasonIndex] != -1 && this[cEpisodeIndex] != -1)
            {
                m_onlineEpisode = new DBOnlineEpisode(this[cSeriesID], this[cSeasonIndex], this[cEpisodeIndex]);
                this[cCompositeID] = m_onlineEpisode[DBOnlineEpisode.cCompositeID];
            }
        }

        public void ChangeSeriesID(int nSeriesID)
        {
            // use base[] as this[] as logic for DBOnlineEpisode prevents localepisode seriesID being set 
            // and interfers with double episode logic

            // TODO: update local_episodes set seriesID =  74205 where seriesID = -1
            DBOnlineEpisode newOnlineEpisode = new DBOnlineEpisode();
            string composite = nSeriesID + "_" + base[cSeasonIndex] + "x" + base[cEpisodeIndex];
            if (!base[DBEpisode.cCompositeID].ToString().Contains("x"))
                composite = nSeriesID + "_" + base[DBOnlineEpisode.cFirstAired];
            if (!newOnlineEpisode.ReadPrimary(composite))
            {
                newOnlineEpisode[cSeriesID] = nSeriesID;
                newOnlineEpisode[cSeasonIndex] = base[cSeasonIndex];
                newOnlineEpisode[cEpisodeIndex] = base[cEpisodeIndex];

                foreach (String fieldName in m_onlineEpisode.FieldNames)
                {
                    switch (fieldName)
                    {
                        case DBOnlineEpisode.cCompositeID:
                        case DBOnlineEpisode.cSeriesID:
                            break;

                        default:
                            newOnlineEpisode[fieldName] = m_onlineEpisode[fieldName];
                            break;
                    }
                }
            }
            base[cCompositeID] = newOnlineEpisode[DBOnlineEpisode.cCompositeID];
            base[cSeriesID] = nSeriesID;

            if (base[DBEpisode.cCompositeID2].ToString().Length > 0) {
                DBOnlineEpisode oldDouble = new DBOnlineEpisode();
                bool oldExist = oldDouble.ReadPrimary(base[DBEpisode.cCompositeID2]);
                base[DBEpisode.cCompositeID2] = nSeriesID + "_" + base[DBEpisode.cSeasonIndex] + "x" + base[DBEpisode.cEpisodeIndex2];
                DBOnlineEpisode newDouble = new DBOnlineEpisode();
                if (!newDouble.ReadPrimary(base[DBEpisode.cCompositeID2])) {
                    if (oldExist) {
                        foreach (string fieldName in oldDouble.FieldNames) {
                            switch (fieldName) {
                                case DBOnlineEpisode.cCompositeID:
                                case DBOnlineEpisode.cSeriesID:
                                    break;

                                default:
                                    newDouble[fieldName] = oldDouble[fieldName];
                                    break;
                            }
                        }
                    }

                    newDouble[cSeriesID] = nSeriesID;
                    newDouble[cSeasonIndex] = base[cSeasonIndex];
                    newDouble[cEpisodeIndex] = base[cEpisodeIndex2];

                    newDouble.Commit();
                }
            }
            m_onlineEpisode = newOnlineEpisode;
            Commit();
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST
            base.AddColumn(cFilename, new DBField(DBField.cTypeString, true));
            base.AddColumn(cCompositeID, new DBField(DBField.cTypeString));
            base.AddColumn(cSeriesID, new DBField(DBField.cTypeInt));
            base.AddColumn(cSeasonIndex, new DBField(DBField.cTypeInt));
            base.AddColumn(cEpisodeIndex, new DBField(DBField.cTypeInt));
            base.AddColumn(cEpisodeName, new DBField(DBField.cTypeString));
            base.AddColumn(cImportProcessed, new DBField(DBField.cTypeInt));
            base.AddColumn(cAvailableSubtitles, new DBField(DBField.cTypeString));

            base.AddColumn(cCompositeID2, new DBField(DBField.cTypeString));
            base.AddColumn(cEpisodeIndex2, new DBField(DBField.cTypeInt));

            base.AddColumn(cVideoWidth, new DBField(DBField.cTypeInt));
            base.AddColumn(cVideoHeight, new DBField(DBField.cTypeInt));

            base.AddColumn(cFileDateAdded, new DBField(DBField.cTypeString));
            base.AddColumn(cFileDateCreated, new DBField(DBField.cTypeString));
            
            base.AddColumn(cIsAvailable, new DBField(DBField.cTypeInt));

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

        public override void InitValues()
        {
            base.InitValues();
            this[cSeasonIndex] = -1;
            this[cEpisodeIndex] = -1;
        }

        public bool HasMediaInfo
        {
            get
            {
                // Check at least one MediaInfo field has been populated                
                if (String.IsNullOrEmpty(this["localPlaytime"]))                                
                    return false;
                else
                {
                    int noAttempts = 0;
                    if (!int.TryParse(this["localPlaytime"], out noAttempts)) return true;
                    
                    // local playtime will be greater than zero if mediainfo has been retrieved
                    if (noAttempts > 0) return true;

                    // attempt to read MediaInfo until maximum attempts have been reached
                    if (noAttempts >= -MAX_MEDIAINFO_RETRIES) return false;

                    return true;
                }

            }
        }

        public bool ReadMediaInfo()
        {
            // Get File Date Added/Created
            GetFileTimeStamps();

            MediaInfoLib.MediaInfo MI = WindowPlugins.GUITVSeries.MediaInfoLib.MediaInfo.GetInstance();
            
            // MediaInfo Object could not be created
            if (null == MI) return false;
            
            // Check if File Exists and is not an Image type e.g. ISO (we can't extract mediainfo from that)
            if (System.IO.File.Exists(this[DBEpisode.cFilename]) && !Helper.IsImageFile(this[DBEpisode.cFilename]))
            {
                try
                {
                    MPTVSeriesLog.Write("Attempting to read Mediainfo for ", this[DBEpisode.cFilename].ToString(), MPTVSeriesLog.LogLevel.DebugSQL);
                    
                    // open file in MediaInfo
                    MI.Open(this[DBEpisode.cFilename]);
                                        
                    // check number of failed attempts at mediainfo extraction                    
                    int noAttempts = 0;
                    int.TryParse(this["localPlaytime"], out noAttempts);
                    noAttempts--;
                    
                    // Get Playtime (runtime)
                    string result = MI.VideoPlaytime;
                    this["localPlaytime"] = result != "-1" ? result : noAttempts.ToString();

                    bool failed = false;
                    if (result != "-1")
                    {
                        this["VideoCodec"] = MI.VideoCodec;
                        this["VideoBitrate"] = MI.VideoBitrate;
                        this["VideoFrameRate"] = MI.VideoFramesPerSecond;
                        this["videoWidth"] = MI.VideoWidth;
                        this["videoHeight"] = MI.VideoHeight;
                        this["VideoAspectRatio"] = MI.VideoAspectRatio;

                        this["AudioCodec"] = MI.AudioCodec;
                        this["AudioBitrate"] = MI.AudioBitrate;
                        this["AudioChannels"] = MI.AudioChannelCount;
                        this["AudioTracks"] = MI.AudioStreamCount;

                        this["TextCount"] = MI.SubtitleCount;
                    }
                    else 
                        failed = true;
                    
                    // MediaInfo cleanup
                    MI.Close();

                    if (failed) {
                        // Get number of retries left to report to user
                        int retries = MAX_MEDIAINFO_RETRIES - (noAttempts * -1);

                        string retriesLeft = retries > 0 ? retries.ToString() : "No";
                        retriesLeft = string.Format("Problem parsing MediaInfo for: {0}, ({1} retries left)", this[DBEpisode.cFilename].ToString(), retriesLeft);

                        MPTVSeriesLog.Write(retriesLeft, MPTVSeriesLog.LogLevel.Normal);
                    }
                    else {
                        if (OnlineParsing.IsMainOnlineParseComplete) {
                            // we can now log output to keep user informed of scan progress
                            MPTVSeriesLog.Write("Succesfully read MediaInfo for ", this[DBEpisode.cFilename].ToString(),MPTVSeriesLog.LogLevel.Normal);
                        }
                        else {                            
                            MPTVSeriesLog.Write("Succesfully read MediaInfo for ", this[DBEpisode.cFilename].ToString(), MPTVSeriesLog.LogLevel.Debug);
                        }
                    }
                    // Commit MediaInfo to database
                    Commit();
                    
                    return true;
                }
                catch (Exception ex)
                {
                    MPTVSeriesLog.Write("Error reading MediaInfo: ", ex.Message, MPTVSeriesLog.LogLevel.Normal);
                }
                
            }
            return false;

        }

        private void GetFileTimeStamps()
        {
            try
            {
                if (System.IO.File.Exists(this[DBEpisode.cFilename]))
                {
                    this[cFileDateAdded] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    this[cFileDateCreated] = System.IO.File.GetCreationTime(this[DBEpisode.cFilename]).ToString("yyyy-MM-dd HH:mm:ss");
                    Commit();
                }
            }
            catch
            {
                MPTVSeriesLog.Write("Error: Unable to extract File Timestamps");
            }
        }

        public bool checkHasSubtitles()
        {
            if (String.IsNullOrEmpty(this[DBEpisode.cFilename])) return false;
            if (subTitleExtensions.Count == 0)
            {
                // load them in first time
                subTitleExtensions.Add(".aqt");
                subTitleExtensions.Add(".asc");
                subTitleExtensions.Add(".ass");
                subTitleExtensions.Add(".dat");
                subTitleExtensions.Add(".dks");
                subTitleExtensions.Add(".js");
                subTitleExtensions.Add(".jss");
                subTitleExtensions.Add(".lrc");
                subTitleExtensions.Add(".mpl");
                subTitleExtensions.Add(".ovr");
                subTitleExtensions.Add(".pan");
                subTitleExtensions.Add(".pjs");
                subTitleExtensions.Add(".psb");
                subTitleExtensions.Add(".rt");
                subTitleExtensions.Add(".rtf");
                subTitleExtensions.Add(".s2k");
                subTitleExtensions.Add(".sbt");
                subTitleExtensions.Add(".scr");
                subTitleExtensions.Add(".smi");
                subTitleExtensions.Add(".son");
                subTitleExtensions.Add(".srt");
                subTitleExtensions.Add(".ssa");
                subTitleExtensions.Add(".sst");
                subTitleExtensions.Add(".ssts");
                subTitleExtensions.Add(".stl");
                subTitleExtensions.Add(".sub");
                subTitleExtensions.Add(".txt");
                subTitleExtensions.Add(".vkt");
                subTitleExtensions.Add(".vsf");
                subTitleExtensions.Add(".zeg");
                
            }

            // Read MediaInfo for embedded subtitles
            if (!String.IsNullOrEmpty(this["TextCount"]))
            {
                if ((int)this["TextCount"] > 0) 
                    return true;
            }

            string filenameNoExt = System.IO.Path.GetFileNameWithoutExtension(this[cFilename]);
            try
            {
                foreach (string file in System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(this[cFilename]), filenameNoExt + "*"))
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(file);
                    if (subTitleExtensions.Contains(fi.Extension.ToLower())) return true;
                }
            }
            catch (Exception)
            {
                // most likley path not available
            }
            return false;
        }

        public DBOnlineEpisode onlineEpisode
        {
            get { return m_onlineEpisode; }
        }

        public override ICollection<String> FieldNames
        {
            get
            {
                List<String> outList = new List<String>();
                // some ordering: I want some fields to come up first
                outList.Add(cSeriesID);
                outList.Add(cSeasonIndex);
                outList.Add(cEpisodeIndex);
                outList.Add(cEpisodeIndex2);
                outList.Add(cCompositeID);
                outList.Add(cCompositeID2);

                foreach (KeyValuePair<string, DBField> pair in m_fields)
                {
                    if (outList.IndexOf(pair.Key) == -1)
                        outList.Add(pair.Key);
                }
                if (m_onlineEpisode != null)
                {
                    foreach (KeyValuePair<string, DBField> pair in m_onlineEpisode.m_fields)
                    {
                        if (outList.IndexOf(pair.Key) == -1)
                            outList.Add(pair.Key);
                    }
                }

                return outList;
            }
        }

        // function override to search on both this & the onlineEpisode
        public override DBValue this[String fieldName]
        {
            get
            {

                switch (fieldName)
                {
                    case cFileSizeBytes:
                        return System.IO.File.Exists(base[DBEpisode.cFilename]) ? new System.IO.FileInfo(base[DBEpisode.cFilename]).Length : 0;
                    case cFileSize:
                        return  StrFormatByteSize(this[cFileSizeBytes]);
                    case cAvailableSubtitles:
                        return (this[cAvailableSubtitles] = checkHasSubtitles());
                    case cPrettyPlaytime:
                        return Helper.MSToMMSS(this["localPlaytime"]);
                    case cFilenameWOPath:
                        return System.IO.Path.GetFileName(this[cFilename]);
                        }
                // online data always takes precedence over the local file data
                if (m_onlineEpisode != null)
                {
                    DBValue retVal = null;
                    switch (fieldName)
                    {
                        case cEpisodeName:
                            retVal = m_onlineEpisode[DBOnlineEpisode.cEpisodeName];
                            if (String.IsNullOrEmpty(retVal))
                                retVal = base[cEpisodeName];
                            return retVal;

                        default:
                            retVal = m_onlineEpisode[fieldName];
                            if (String.IsNullOrEmpty(retVal))
                                retVal = base[fieldName];
                            return retVal;
                    }
                }
                else
                    return base[fieldName];
            }

            set
            {

                if (m_onlineEpisode != null)
                {
                    switch (fieldName)
                    {
                        case cEpisodeName:
                        case cCompositeID:
                        case cEpisodeIndex:
                        case cSeasonIndex:
                        case cCompositeID2:
                        case cIsOnRemovable:
                        case cVolumeLabel:
                            // the only flags we are not rerouting to the onlineEpisode if it exists
                            break;
                        case cEpisodeIndex2:
                            if (!String.IsNullOrEmpty(value) && (!Helper.String.IsNumerical(value) || Int32.Parse(value) != Int32.Parse(base[cEpisodeIndex]) + 1))
                            {
                                MPTVSeriesLog.Write("Info: A file parsed out a secondary episode index, indicating a double episode, however the value was discarded because it was either not numerical or not equal to <episodeIndex> + 1. This is often an indication of too loose restriction in your parsing expressions. - " +
                                    base[cFilename] + " Value for " + cEpisodeIndex2 + " was: " + value.ToString());
                                return;
                            }
                            break;

                        default:
                            if (m_onlineEpisode.m_fields.ContainsKey(fieldName) || fieldName == DBOnlineEpisode.cMyRating)
                            {
                                m_onlineEpisode[fieldName] = value;
                                return;
                            }
                            break;
                    }
                }

                base[fieldName] = value;

                if (m_onlineEpisode == null && base[cSeriesID].ToString().Length > 0 && base[cSeasonIndex] != -1 && base[cEpisodeIndex] != -1)
                {
                    // we have enough data to create an online episode
                    m_onlineEpisode = new DBOnlineEpisode(base[cSeriesID], base[cSeasonIndex], base[cEpisodeIndex]);
                    base[cCompositeID] = m_onlineEpisode[DBOnlineEpisode.cCompositeID];
                    Commit();
                }
                else if (m_onlineEpisode == null && base[cSeriesID].ToString().Length > 0 && base[DBOnlineEpisode.cFirstAired].ToString().Length > 0)
                {
                    // in case of firstaired matching, we temporarily create an composite id based on it, this will later be changed to season/ep again
                    m_onlineEpisode = new DBOnlineEpisode();
                    m_onlineEpisode[DBOnlineEpisode.cCompositeID] = base[cSeriesID] + "_" + base[DBOnlineEpisode.cFirstAired];
                    base[cCompositeID] = m_onlineEpisode[DBOnlineEpisode.cCompositeID];
                    m_onlineEpisode[DBOnlineEpisode.cSeasonIndex] = base[cSeasonIndex];
                    m_onlineEpisode[DBOnlineEpisode.cEpisodeIndex] = base[cEpisodeIndex];
                    m_onlineEpisode[DBOnlineEpisode.cSeriesID] = base[cSeriesID];
                    Commit();
                }
                else if (m_onlineEpisode != null && base[cCompositeID] == base[cSeriesID] + "_" + base[DBOnlineEpisode.cFirstAired] && base[cSeasonIndex] != -1 && base[cEpisodeIndex] != -1)
                {
                    // in case of firstaired matching, this is the place we change the composite id back
                    m_onlineEpisode[DBOnlineEpisode.cCompositeID] = base[cSeriesID] + "_" + base[cSeasonIndex] + "x" + base[cEpisodeIndex];
                    base[cCompositeID] = m_onlineEpisode[DBOnlineEpisode.cCompositeID];
                    m_onlineEpisode.Commit();

                    SQLCondition cleanup = new SQLCondition();
                    cleanup.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, base[cSeriesID], SQLConditionType.Equal);
                    cleanup.Add(new DBOnlineEpisode(), DBOnlineEpisode.cCompositeID, base[cSeriesID] + "_" + base[DBOnlineEpisode.cFirstAired], SQLConditionType.Equal);
                    DBOnlineEpisode.Clear(cleanup);
                }

            }
        }

        public override bool Commit()
        {
            if (m_onlineEpisode != null)
                m_onlineEpisode.Commit();
            if (dbEpisodeUpdateOccured != null)
                dbEpisodeUpdateOccured(this);
            return base.Commit();
        }

        public static DBEpisode GetFirstUnwatched(int seriesID)
        {
            SQLCondition conditions = new SQLCondition();
            conditions.Add(new DBEpisode(), DBEpisode.cSeriesID, new DBValue(seriesID), SQLConditionType.Equal);
            List<DBEpisode> results = GetFirstUnwatched(conditions);
            if (results.Count > 0)
                return results[0];
            else
                return null;
        }

        public static DBEpisode GetFirstUnwatched(int seriesID, int seasonIndex)
        {
            SQLCondition conditions = new SQLCondition();
            conditions.Add(new DBEpisode(), DBEpisode.cSeriesID, new DBValue(seriesID), SQLConditionType.Equal);
            conditions.Add(new DBEpisode(), DBEpisode.cSeasonIndex, new DBValue(seasonIndex), SQLConditionType.Equal);
            List<DBEpisode> results = GetFirstUnwatched(conditions);
            if (results.Count > 0)
                return results[0];
            else
                return null;
        }

        public static List<DBEpisode> GetFirstUnwatched()
        {
            return GetFirstUnwatched(new SQLCondition());
        }

        public static void GetSeasonEpisodeCounts(DBSeason season, out int epsTotal, out int epsUnWatched)
        {
            m_bUpdateEpisodeCount = true;

            SQLCondition cond = new SQLCondition(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, season[DBSeason.cSeriesID], SQLConditionType.Equal);
            cond.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeasonIndex, season[DBSeason.cIndex], SQLConditionType.Equal);
//            string query = stdGetSQL(cond, false, true, "count(*) as epCount, sum(" + DBOnlineEpisode.cWatched + ") as watched");
            string query = stdGetSQL(cond, false, true, "online_episodes.CompositeID, Watched, FirstAired"); 
            SQLiteResultSet results = DBTVSeries.Execute(query);
            epsTotal = 0; // total episode means total *AIRED* episodes - I'm sure no-one's interested in counting unaired episodes
            int parseResult = 0;
            int epsWatched = 0;
            //we either get two rows (one for normal episodes, one for double episodes), or we get no rows so we add them
            for (int i = 0; i < results.Rows.Count; i++) {
                if (int.TryParse(results.Rows[i].fields[1], out parseResult)) {
                    epsWatched+= parseResult;
                }

                Regex r = new Regex(@"(\d{4})-(\d{2})-(\d{2})");
                Match match = r.Match(results.Rows[i].fields[2]);
                DateTime firstAired;
                if (match.Success) {
                    firstAired = new DateTime(Convert.ToInt32(match.Groups[1].Value),Convert.ToInt32(match.Groups[2].Value),Convert.ToInt32(match.Groups[3].Value));
                    if (firstAired < DateTime.Today)
                        epsTotal++;
                }
//                 if (int.TryParse(results.Rows[i].fields[0], out parseResult)) {
//                     epsTotal += parseResult;
//                 }
//                 if (int.TryParse(results.Rows[i].fields[1], out parseResult)) {
//                     epsWatched += parseResult;
//                 }
            }
            epsUnWatched = epsTotal - epsWatched;
            // this happens if for some reasoon an episode is marked as watched, but firstaired is in the future 
            // - or no firstaired provided
            if (epsUnWatched < 0)
                epsUnWatched = 0;
        }

        public static void GetSeriesEpisodeCounts(int series, out int epsTotal, out int epsUnWatched)
        {
            m_bUpdateEpisodeCount = true;

            SQLCondition cond = new SQLCondition(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series, SQLConditionType.Equal);
//            string query = stdGetSQL(cond, false, true, "count(*) as epCount, sum(" + DBOnlineEpisode.cWatched + ") as watched");
            string query = stdGetSQL(cond, false, true, "online_episodes.CompositeID, Watched, FirstAired");
            SQLiteResultSet results = DBTVSeries.Execute(query);
            epsTotal = 0;
            int parseResult = 0;
            int epsWatched = 0;
            //we either get two rows (one for normal episodes, one for double episodes), or we get no rows so we add them
            for (int i = 0; i < results.Rows.Count; i++) {
                if (int.TryParse(results.Rows[i].fields[1], out parseResult))
                {
                    epsWatched += parseResult;
                }

                Regex r = new Regex(@"(\d{4})-(\d{2})-(\d{2})");
                Match match = r.Match(results.Rows[i].fields[2]);
                DateTime firstAired;
                if (match.Success)
                {
                    firstAired = new DateTime(Convert.ToInt32(match.Groups[1].Value), Convert.ToInt32(match.Groups[2].Value), Convert.ToInt32(match.Groups[3].Value));
                    if (firstAired < DateTime.Today)
                        epsTotal++;
                }

                
//                 if (int.TryParse(results.Rows[i].fields[0], out parseResult)) {
//                     epsTotal += parseResult;
//                 }
//                 if (int.TryParse(results.Rows[i].fields[1], out parseResult)) {
//                     epsWatched += parseResult;
//                 }
            }
            epsUnWatched = epsTotal - epsWatched;
        }

        static List<DBEpisode> GetFirstUnwatched(SQLCondition conditions)
        {
            SQLWhat what = new SQLWhat(new DBEpisode());
            conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cWatched, new DBValue(false), SQLConditionType.Equal);

            string sqlQuery = "select " + what + " where compositeid in ( select min(local_episodes.compositeid) from local_episodes inner join online_episodes on local_episodes.compositeid = online_episodes.compositeid " + conditions 
                + @" and online_episodes.hidden = 0 "
                + @"and exists (select id from local_series where id = local_episodes.seriesid and hidden = 0) " 
                + @"and exists (select id from season where seriesid = local_episodes.seriesid and seasonindex = local_episodes.seasonindex and hidden = 0) "
                + @"group by local_episodes.seriesID );";
            SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
            List<DBEpisode> outList = new List<DBEpisode>();
            if (results.Rows.Count > 0)
            {
                for (int index = 0; index < results.Rows.Count; index++)
                {
                    DBEpisode episode = new DBEpisode();
                    episode.Read(ref results, index);
                    episode.m_onlineEpisode = new DBOnlineEpisode();
                    episode.m_onlineEpisode.Read(results.Rows[index], results.ColumnIndices);
                    outList.Add(episode);
                }
            }
            return outList;
        }

        public static SQLCondition stdConditions
        {
            get
            {
                SQLCondition conditions = new SQLCondition();
                if ((!Settings.isConfig || m_bUpdateEpisodeCount) && DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles))
                    conditions.Add(new DBEpisode(), DBEpisode.cFilename, string.Empty, SQLConditionType.NotEqual);

                // include hidden?
                if ((!Settings.isConfig || m_bUpdateEpisodeCount) || !DBOption.GetOptions(DBOption.cShowHiddenItems))
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cHidden, 0, SQLConditionType.Equal);

                m_bUpdateEpisodeCount = false;
                return conditions;
            }
        }

        public static string stdGetSQL(SQLCondition conditions, bool selectFull)
        {
            return stdGetSQL(conditions, selectFull, true);
        }

        public static string stdGetSQL(SQLCondition conditions, bool selectFull, bool inclStdCond)
        {
            return stdGetSQL(conditions, selectFull, inclStdCond, DBOnlineEpisode.cTableName + "." + DBOnlineEpisode.cEpisodeIndex, false);
        }

        public static string stdGetSQL(SQLCondition conditions, bool selectFull, bool inclStdCond, bool reverseJoin)
        {
            return stdGetSQL(conditions, selectFull, inclStdCond, DBOnlineEpisode.cTableName + "." + DBOnlineEpisode.cEpisodeIndex, reverseJoin);
        }

        public static string stdGetSQL(SQLCondition conditions, bool selectFull, bool inclStdCond, string fieldToSelectIfNotFull)
        {
            return stdGetSQL(conditions, selectFull, inclStdCond, fieldToSelectIfNotFull, false);
        }

        public static string stdGetSQL(SQLCondition conditions, bool selectFull, bool inclStdCond, string fieldToSelectIfNotFull, bool reverseJoin)
        {
            String sqlQuery = string.Empty;
            String sqlWhat = string.Empty;
            if (inclStdCond) {
                conditions.AddCustom(stdConditions.ConditionsSQLString);
            }

            SQLCondition conditionsFirst = conditions.Copy();
            SQLCondition conditionsSecond = conditions.Copy();
            // need to extract the series condition from the original conditions, to retrieve the series this is based upon
            String sWhere = conditions;
            String RegExp = DBOnlineEpisode.Q(cSeriesID) + @" = (-?\d+)";
            Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
            Match matchSeriesID = Engine.Match(conditions);
            RegExp = DBOnlineEpisode.Q(cSeasonIndex) + @" = (-?\d+)";
            Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
            Match matchSeasonIndex = Engine.Match(conditions);

            SQLCondition subQueryConditions = new SQLCondition();
            if (matchSeriesID.Success)
                subQueryConditions.Add(new DBEpisode(), cSeriesID, matchSeriesID.Groups[1].Value, SQLConditionType.Equal);
            if (matchSeasonIndex.Success)
                subQueryConditions.Add(new DBEpisode(), cSeasonIndex, matchSeasonIndex.Groups[1].Value, SQLConditionType.Equal);
            subQueryConditions.Add(new DBEpisode(), cCompositeID2, "", SQLConditionType.NotEqual);

            String sqlSubQuery = "select distinct " + DBEpisode.Q(cCompositeID2) + " from " + DBEpisode.cTableName + subQueryConditions;
            conditionsFirst.AddCustom(sqlSubQuery, DBOnlineEpisode.Q(cCompositeID), SQLConditionType.NotIn);
            conditionsSecond.Add(new DBEpisode(), cCompositeID2, "", SQLConditionType.NotEqual);

            DBTable first = null;
            DBTable second = null;
            if (reverseJoin) {
                //reverse the order of these so that its possible to select DBEpisodes without DBOnlineEpisodes
                // - SQLite dosen't fully support right joins so we have to reverse the table order
                first = new DBEpisode();
                second = new DBOnlineEpisode();
            } else {
                first = new DBOnlineEpisode();
                second = new DBEpisode();
            }

            string orderBy = string.Empty;
            if(selectFull)
            {
                orderBy = !conditions.customOrderStringIsSet
                      ? string.Empty
                      : conditions.orderString;

                if (String.IsNullOrEmpty(orderBy))
                    orderBy = " order by " + DBOnlineEpisode.Q(cEpisodeIndex);

                SQLWhat what = new SQLWhat(first);
                what.AddWhat(second);
                // one query gets both first & second episode
                sqlWhat = "select " + what;
            }
            else
            {
                sqlWhat = "select " + fieldToSelectIfNotFull + " from " + first.m_tableName;
            }
            // oh, oh, the or join condition is slower than hell
            // its orders of magnitude faster to make two queries instead and do a UNION
            // union currently has a problem with orders in sqlite (bug in 3.4) -> temp workaround = use explicite alias on order field (works only if a single order col)
            // http://www.sqlite.org/cvstrac/tktview?tn=2561,6
            if (!String.IsNullOrEmpty(orderBy))
            {
                string ordercol = orderBy.Replace(" order by ", "").Replace(" asc ", "").Replace(" desc ", "");
                string ordecolsplit = ordercol;
                
                if (ordercol.Contains("."))
                {
                    ordecolsplit = ordecolsplit.Split(new char[] { '.' })[1];
                }
                sqlWhat = sqlWhat.Replace(ordercol, ordercol + " as " + ordercol.Replace(".", "") + " ");
                orderBy = " order by " + ordercol.Replace(".", "") + (orderBy.Contains(" desc ") ? " desc " : " asc ");
            }

            sqlQuery = sqlWhat + " left join " + second.m_tableName + " on (" + DBEpisode.Q(cCompositeID) + "=" + DBOnlineEpisode.Q(cCompositeID)
                + ") " + conditionsFirst
                + " union ";
            sqlQuery += sqlWhat + " left join " + second.m_tableName + " on (" + DBEpisode.Q(cCompositeID2) + "=" + DBOnlineEpisode.Q(cCompositeID)
                + ") " + conditionsSecond + orderBy + conditions.limitString;
            
            return sqlQuery;
        }

        public static List<DBEpisode> Get(int nSeriesID)
        {
            return Get(nSeriesID, true);
        }
        public static List<DBEpisode> Get(int nSeriesID, bool inclStdCond)
        {
            SQLCondition conditions = new SQLCondition();
            conditions.Add(new DBOnlineEpisode(), cSeriesID, nSeriesID, SQLConditionType.Equal);

            return Get(conditions, inclStdCond);
        }

        public static List<DBEpisode> Get(int nSeriesID, int nSeasonIndex)
        {
            return Get(nSeriesID, nSeasonIndex, true);
        }
        public static List<DBEpisode> Get(int nSeriesID, int nSeasonIndex, bool includeStdCond)
        {
            SQLCondition conditions = new SQLCondition();
            conditions.Add(new DBOnlineEpisode(), cSeriesID, nSeriesID, SQLConditionType.Equal);
            conditions.Add(new DBOnlineEpisode(), cSeasonIndex, nSeasonIndex, SQLConditionType.Equal);

            return Get(conditions, includeStdCond);
        }

        public static List<DBEpisode> Get(SQLCondition conditions)
        {
            return Get(conditions, true);
        }

        public static List<DBEpisode> Get(SQLCondition conditions, bool includeStdCond)
        {
            return Get(stdGetSQL(conditions, true, includeStdCond, false));
        }

        public static List<DBEpisode> Get(SQLCondition conditions, bool includeStdCond, bool reverseJoin)
        {
            return Get(stdGetSQL(conditions, true, includeStdCond, reverseJoin));
        }

        public static List<DBEpisode> Get(string query)
        {
            SQLiteResultSet results = DBTVSeries.Execute(query);
            List<DBEpisode> outList = new List<DBEpisode>();
            if (results.Rows.Count > 0)
            {
                for (int index = 0; index < results.Rows.Count; index++)
                {
                    DBEpisode episode = new DBEpisode();
                    episode.Read(results.Rows[index], results.ColumnIndices);
                    episode.m_onlineEpisode = new DBOnlineEpisode();
                    episode.m_onlineEpisode.Read(results.Rows[index], results.ColumnIndices);
                    outList.Add(episode);
                }
            }
            return outList;
        }

        /// <summary>
        /// If the episode contains an OnlineEpisode returns it's ToString() result
        /// </summary>
        /// <returns>"series.toString() - 1x01" or OnlineEpisode.ToString() result</returns>
        public override string ToString()
        {
            if (this.m_onlineEpisode != null) return m_onlineEpisode.ToString();
            DBSeries s = Helper.getCorrespondingSeries(this[DBEpisode.cSeriesID]);
            return string.Format("{0} - {1}x{2}", (s == null ? "(" + this[DBEpisode.cSeriesID] + ")" : s.ToString()), this[DBEpisode.cSeasonIndex], this[DBEpisode.cEpisodeIndex]);
        }

        public static void Clear(SQLCondition conditions)
        {
            Clear(new DBEpisode(), conditions);
        }

        public static void GlobalSet(String sKey, DBValue Value)
        {
            GlobalSet(sKey, Value, new SQLCondition());
        }

        public static void GlobalSet(String sKey, DBValue Value, SQLCondition condition)
        {
            GlobalSet(new DBOnlineEpisode(), sKey, Value, condition);
            GlobalSet(new DBEpisode(), sKey, Value, condition);
        }

        public static new String Q(String sField)
        {
            return cTableName + "." + sField;
        }

        public string Image
		{
			get
			{
                if (m_onlineEpisode == null) return string.Empty;
                return m_onlineEpisode.Image;
			}
        }

        #region PrettyFilesize
        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        static extern long StrFormatByteSize(long fileSize,
        [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer, int bufferSize);

        static string StrFormatByteSize(long fileSize)
        {
            StringBuilder sbBuffer = new StringBuilder(20);
            StrFormatByteSize(fileSize, sbBuffer, 20);
            return sbBuffer.ToString();
        }
        #endregion
    }
}
