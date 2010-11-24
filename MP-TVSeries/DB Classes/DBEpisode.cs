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
using System.IO;

namespace WindowPlugins.GUITVSeries
{
    //moved to DBOnineEpisodes.cs
    //public class DBOnlineEpisode : DBTable
    public enum MostRecentType
    {
        Watched,
        Added,
        Created
    }

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
        public const int cDBVersion = 7;

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
        public const String cCompositeUpdated = "CompositeUpdated";
        public const String cOriginalComposite = "OriginalComposite";
        public const String cOriginalComposite2 = "OriginalComposite2";
        public const String cAvailableSubtitles = "AvailableSubtitles";

        public const String cAudioCodec = "AudioCodec";        
        public const String cAudioFormat = "AudioFormat";
        public const String cAudioFormatProfile = "AudioFormatProfile";
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
        public const String cVideoFormat = "VideoFormat";
        public const String cVideoFormatProfile = "VideoFormatProfile";
        
        public const String cStopTime = "StopTime";
        public const String cDateWatched = "DateWatched";

        public const String cFileSizeBytes = "FileSizeB";
        public const String cFileSize = "FileSize";

        public const String cLocalPlaytime = "localPlaytime";
        public const String cPrettyPlaytime = "PrettyLocalPlaytime";
        public const String cFilenameWOPath = "EpisodeFilenameWithoutPath";
        public const String cFilenameWOPathAndExtension = "EpisodeFilenameWithoutPathAndExtension";
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
            s_FieldToDisplayNameMap.Add(cVideoFormat, "Video Format");
            s_FieldToDisplayNameMap.Add(cVideoFormatProfile, "Video Format Profile");
            s_FieldToDisplayNameMap.Add(cVideoFrameRate, "Video Frame Rate");
            s_FieldToDisplayNameMap.Add(cAudioBitrate, "Audio Bit Rate");
            s_FieldToDisplayNameMap.Add(cVolumeLabel, "Volume Label");
            s_FieldToDisplayNameMap.Add(cAudioChannels, "Audio Channels");
            s_FieldToDisplayNameMap.Add(cAudioCodec, "Audio Codec");            
            s_FieldToDisplayNameMap.Add(cAudioFormat, "Audio Format");
            s_FieldToDisplayNameMap.Add(cAudioFormatProfile, "Audio Format Profile");
            s_FieldToDisplayNameMap.Add(cAudioTracks, "Audio Tracks");
            s_FieldToDisplayNameMap.Add(cTextCount, "Subtitle Count");
            s_FieldToDisplayNameMap.Add(cLocalPlaytime, "Runtime");
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
                    
                    case 6:
                        // Ensure that Volume Label is not empty
                        SQLCondition conditions = new SQLCondition();
                        conditions.Add(new DBEpisode(), DBEpisode.cFilename, string.Empty, SQLConditionType.NotEqual);
                        List<DBEpisode> episodes = DBEpisode.Get(conditions);
                        
                        // For some reason I couldnt get all eps missing a volume label when I explictly added condition
                        // use this method instead to get them by removing all eps that already have a volume label
                        episodes.RemoveAll(ep => ep[cVolumeLabel].ToString().Trim() != string.Empty);

                        foreach (DBEpisode episode in episodes)
                        {
                            string filename = episode[cFilename].ToString();
                            if (!string.IsNullOrEmpty(filename))
                            {
                                string volumeLabel = DeviceManager.GetVolumeLabel(filename);
                                if (string.IsNullOrEmpty(volumeLabel))
                                {
                                    // Get Import Folder as volume label
                                    volumeLabel = LocalParse.getImportPath(episode[cFilename]);
                                }
                                episode[cVolumeLabel] = volumeLabel;
                                episode.Commit();
                            }
                        }
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

        public void ChangeIndexes(int seasonIndex, int episodeIndex)
        {
            ChangeIndexes(seasonIndex, episodeIndex, false);
        }

        /// <summary>
        /// Change the local episode composite, season and episode indexes
        /// </summary>
        /// <param name="seasonIndex">Season Index of new episode ID</param>
        /// <param name="episodeIndex">Episode Index of new episode ID</param>
        /// <param name="isSecondPart">Set to true if its the 2nd part of a double episode</param>
        public void ChangeIndexes(int seasonIndex, int episodeIndex, bool isSecondPart)
        {
            string currentComposite = base[cCompositeID];
            string currentComposite2 = base[cCompositeID2];

            // save original composite id's for when we do online updates and re-matching
            if (!isSecondPart && string.IsNullOrEmpty(base[cOriginalComposite]))
                base[cOriginalComposite] = currentComposite;
            if (isSecondPart && string.IsNullOrEmpty(base[cOriginalComposite2]))
                base[cOriginalComposite2] = currentComposite2;

            string composite = base[cSeriesID] + "_" + seasonIndex + "x" + episodeIndex;
            base[cSeasonIndex] = seasonIndex;

            if (!isSecondPart)
            {                                
                base[cEpisodeIndex] = episodeIndex;
                base[cCompositeID] = composite;
            }
            else
            {
                // only update second part of episode
                base[cEpisodeIndex2] = episodeIndex;
                base[cCompositeID2] = composite;
            }

            DBOnlineEpisode newOnlineEpisode = new DBOnlineEpisode();
            bool bNewOnlineEpsisode = false;
            if (!newOnlineEpisode.ReadPrimary(composite))
            {                
                foreach (String fieldName in m_onlineEpisode.FieldNames)
                {
                    switch (fieldName)
                    {
                        default:
                            newOnlineEpisode[fieldName] = m_onlineEpisode[fieldName];
                            break;
                    }
                }

                newOnlineEpisode[cSeriesID] = this[cSeriesID];
                newOnlineEpisode[cSeasonIndex] = base[cSeasonIndex];

                if (!isSecondPart)
                {
                    newOnlineEpisode[cEpisodeIndex] = base[cEpisodeIndex];
                    newOnlineEpisode[cCompositeID] = base[cCompositeID];
                }
                else
                {
                    newOnlineEpisode[cEpisodeIndex] = base[cEpisodeIndex2];
                    newOnlineEpisode[cCompositeID] = base[cCompositeID2];
                }

                bNewOnlineEpsisode = true;
            }
            m_onlineEpisode = newOnlineEpisode;
            Commit();

            // cleanup old reference if we needed to create a new online episode
            // only remove if no online data associated with it
            if (bNewOnlineEpsisode)
            {
                string compositeID = isSecondPart ? currentComposite2 : currentComposite;

                SQLCondition condition = new SQLCondition();
                condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cCompositeID, compositeID, SQLConditionType.Equal);
                condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cID, 0, SQLConditionType.Equal);
                DBOnlineEpisode.Clear(condition);
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
            base.AddColumn(cOriginalComposite, new DBField(DBField.cTypeString));
            base.AddColumn(cCompositeID, new DBField(DBField.cTypeString));
            base.AddColumn(cSeriesID, new DBField(DBField.cTypeInt));
            base.AddColumn(cSeasonIndex, new DBField(DBField.cTypeInt));
            base.AddColumn(cEpisodeIndex, new DBField(DBField.cTypeInt));
            base.AddColumn(cEpisodeName, new DBField(DBField.cTypeString));
            base.AddColumn(cImportProcessed, new DBField(DBField.cTypeInt));
            base.AddColumn(cCompositeUpdated, new DBField(DBField.cTypeInt));
            base.AddColumn(cAvailableSubtitles, new DBField(DBField.cTypeString));
            base.AddColumn(cOriginalComposite2, new DBField(DBField.cTypeString));
            base.AddColumn(cCompositeID2, new DBField(DBField.cTypeString));
            base.AddColumn(cEpisodeIndex2, new DBField(DBField.cTypeInt));
            base.AddColumn(cVideoWidth, new DBField(DBField.cTypeInt));
            base.AddColumn(cVideoHeight, new DBField(DBField.cTypeInt));
            base.AddColumn(cFileDateAdded, new DBField(DBField.cTypeString));
            base.AddColumn(cFileDateCreated, new DBField(DBField.cTypeString));            
            base.AddColumn(cIsAvailable, new DBField(DBField.cTypeInt));
            base.AddColumn(cDateWatched, new DBField(DBField.cTypeString));

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
                if (String.IsNullOrEmpty(this[cLocalPlaytime]))                                
                    return false;
                else
                {
                    int noAttempts = 0;
                    if (!int.TryParse(this[cLocalPlaytime], out noAttempts)) return true;
                    
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
                    int.TryParse(this[cLocalPlaytime], out noAttempts);
                    noAttempts--;
                    
                    // Get Playtime (runtime)
                    string result = MI.VideoPlaytime;
                    this[cLocalPlaytime] = result != "-1" ? result : noAttempts.ToString();

                    bool failed = false;
                    if (result != "-1")
                    {
                        this[cVideoCodec] = MI.VideoCodec;
                        this[cVideoFormat] = MI.VideoCodecFormat;
                        this[cVideoFormatProfile] = MI.VideoFormatProfile;
                        this[cVideoBitRate] = MI.VideoBitrate;
                        this[cVideoFrameRate] = MI.VideoFramesPerSecond;
                        this[cVideoWidth] = MI.VideoWidth;
                        this[cVideoHeight] = MI.VideoHeight;
                        this[cVideoAspectRatio] = MI.VideoAspectRatio;

                        this[cAudioCodec] = MI.AudioCodec;                        
                        this[cAudioFormat] = MI.AudioCodecFormat;
                        this[cAudioFormatProfile] = MI.AudioFormatProfile;
                        this[cAudioBitrate] = MI.AudioBitrate;
                        this[cAudioChannels] = MI.AudioChannelCount;
                        this[cAudioTracks] = MI.AudioStreamCount;

                        this[cTextCount] = MI.SubtitleCount;
                        
                        // check for subtitles in mediainfo                        
                        this[cAvailableSubtitles] = checkHasSubtitles();
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
                        MPTVSeriesLog.Write("Succesfully read MediaInfo for ", this[DBEpisode.cFilename].ToString(), OnlineParsing.IsMainOnlineParseComplete ? MPTVSeriesLog.LogLevel.Normal : MPTVSeriesLog.LogLevel.Debug);                   
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
            return checkHasSubtitles(true);
        }

        public bool checkHasLocalSubtitles()
        {
            return checkHasSubtitles(false);
        }

        public bool checkHasSubtitles(bool useMediaInfo)
        {
            if (String.IsNullOrEmpty(this[DBEpisode.cFilename])) return false;

            int textCount = -1;
            if (useMediaInfo && !String.IsNullOrEmpty(this["TextCount"]))
            {
                textCount = (int)this["TextCount"];
                if (textCount == -1) textCount = 0;
            }

            fillSubTitleExtensions();

            if (textCount > 0)
                return true;

            // Read MediaInfo for embedded subtitles
            /*
            if (useMediaInfo && !String.IsNullOrEmpty(this["TextCount"]))
            {
                if ((int)this["TextCount"] > 0) 
                    return true;
            }
            */
            
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

        bool isWritable(FileInfo fileInfo)
        {
            FileStream stream = null;
            try
            {
                stream = fileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch
            {
                return true;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            return false;
        }

        public bool isWritable()
        {
            string file = this[DBEpisode.cFilename];
            if (string.IsNullOrEmpty(file)) return false;
            FileInfo fi = new FileInfo(file);
            if (fi != null)
            {
                return isWritable(fi);
            }
            return false;
        }

        public List<string> deleteEpisode(TVSeriesPlugin.DeleteMenuItems type)
        {
            List<string> resultMsg = new List<string>(); 

            // Always delete from Local episode table if deleting from disk or database
            SQLCondition condition = new SQLCondition();
            condition.Add(new DBEpisode(), DBEpisode.cFilename, this[DBEpisode.cFilename], SQLConditionType.Equal);

            List<DBEpisode> episodes = DBEpisode.Get(condition, false);
            if (episodes != null)
            {
                foreach (DBEpisode episode in episodes)
                {
                    string file = this[DBEpisode.cFilename];
                    if ((type != TVSeriesPlugin.DeleteMenuItems.database && !episode.isWritable()) || type == TVSeriesPlugin.DeleteMenuItems.database)
                    {
                        DBEpisode.Clear(condition);

                        if (type != TVSeriesPlugin.DeleteMenuItems.database)
                        {
                            try
                            {
                                MPTVSeriesLog.Write(string.Format("Deleting file: {0}", file));
                                System.IO.File.Delete(file);
                            }
                            catch (Exception ex)
                            {
                                // this should succeed all the time because of the locked check..but still..
                                MPTVSeriesLog.Write(string.Format("Failed to delete: {0}, {1}", file, ex.Message));
                            }
                        }

                        if (type != TVSeriesPlugin.DeleteMenuItems.disk)
                        {
                            SQLCondition condition1 = new SQLCondition();
                            condition1.Add(new DBOnlineEpisode(), DBOnlineEpisode.cID, this[DBOnlineEpisode.cID], SQLConditionType.Equal);
                            if (this[DBOnlineEpisode.cID] == 0)
                            {
                                // online episodes with no data have id=0, so we should improve the query
                                condition1.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, this[DBOnlineEpisode.cSeriesID], SQLConditionType.Equal);
                                condition1.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeasonIndex, this[DBOnlineEpisode.cSeasonIndex], SQLConditionType.Equal);
                                condition1.Add(new DBOnlineEpisode(), DBOnlineEpisode.cEpisodeIndex, this[DBOnlineEpisode.cEpisodeIndex], SQLConditionType.Equal);
                            }
                            DBOnlineEpisode.Clear(condition1);
                        }
                    
                    }
                    else
                    {
                        resultMsg.Add(string.Format(Translation.UnableToDeleteFile, file));
                    }
                }

                // if there are no local episodes, we still need to delete from online table
                if (episodes.Count == 0 && type != TVSeriesPlugin.DeleteMenuItems.disk)
                {
                    condition = new SQLCondition();
                    condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cID, this[DBOnlineEpisode.cID], SQLConditionType.Equal);                    
                    if (this[DBOnlineEpisode.cID] == 0)
                    {
                        // online episodes with no data have id=0, so we should improve the query
                        condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, this[DBOnlineEpisode.cSeriesID], SQLConditionType.Equal);
                        condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeasonIndex, this[DBOnlineEpisode.cSeasonIndex], SQLConditionType.Equal);
                        condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cEpisodeIndex, this[DBOnlineEpisode.cEpisodeIndex], SQLConditionType.Equal);
                    }
                    DBOnlineEpisode.Clear(condition);
                }
            }

            #region Facade Remote Color
            
            // if we have removed all local episodes for a season then set HasLocalFiles to false for season
            if (type == TVSeriesPlugin.DeleteMenuItems.disk)
            {
                SQLCondition seasonConditions = new SQLCondition();
                seasonConditions.Add(new DBEpisode(), DBEpisode.cSeriesID, this[DBEpisode.cSeriesID], SQLConditionType.Equal);
                seasonConditions.Add(new DBEpisode(), DBEpisode.cSeasonIndex, this[DBEpisode.cSeasonIndex], SQLConditionType.Equal);
                List<DBEpisode> localEpisodes = DBEpisode.Get(seasonConditions);
                if (localEpisodes.Count == 0)
                {
                    SQLCondition cond = new SQLCondition();
                    cond.Add(new DBSeason(), DBSeason.cSeriesID, this[DBEpisode.cSeriesID], SQLConditionType.Equal);
                    cond.Add(new DBSeason(), DBSeason.cIndex, this[DBEpisode.cSeasonIndex], SQLConditionType.Equal);
                    List<DBSeason> season = DBSeason.Get(cond);                    
                    // should only get one season returned
                    if (season != null && season.Count == 1)
                    {
                        season[0][DBSeason.cHasLocalFiles] = false;
                        season[0].Commit();
                    }
                    
                    // also check if local files exist for series
                    SQLCondition seriesConditions = new SQLCondition();
                    seriesConditions.Add(new DBEpisode(), DBEpisode.cSeriesID, this[DBEpisode.cSeriesID], SQLConditionType.Equal);                    
                    localEpisodes = DBEpisode.Get(seriesConditions);
                    if (localEpisodes.Count == 0)
                    {
                        DBSeries series = DBSeries.Get(this[DBEpisode.cSeriesID]);
                        if (series != null)
                        {
                            series[DBOnlineSeries.cHasLocalFiles] = false;
                            series.Commit();
                        }
                    }
                }
            }
            
            #endregion

            #region Cleanup
            DBSeries.IsSeriesRemoved = false;
            if (type != TVSeriesPlugin.DeleteMenuItems.disk)
            {
                // If local/online episode count is zero then delete the season
                condition = new SQLCondition();
                condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, this[DBOnlineEpisode.cSeriesID], SQLConditionType.Equal);
                condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeasonIndex, this[DBOnlineEpisode.cSeasonIndex], SQLConditionType.Equal);
                episodes = DBEpisode.Get(condition, false);
                if (episodes.Count == 0)
                {
                    condition = new SQLCondition();
                    condition.Add(new DBSeason(), DBSeason.cSeriesID, this[DBOnlineEpisode.cSeriesID], SQLConditionType.Equal);
                    condition.Add(new DBSeason(), DBSeason.cIndex, this[DBOnlineEpisode.cSeasonIndex], SQLConditionType.Equal);
                    DBSeason.Clear(condition);

                    // If episode count is still zero, then delete the series\seasons
                    condition = new SQLCondition();
                    condition.Add(new DBEpisode(), DBEpisode.cSeriesID, this[DBOnlineEpisode.cSeriesID], SQLConditionType.Equal);
                    episodes = DBEpisode.Get(condition, false);
                    if (episodes.Count == 0)
                    {
                        // Delete All Seasons
                        condition = new SQLCondition();
                        condition.Add(new DBSeason(), DBSeason.cSeriesID, this[DBOnlineEpisode.cSeriesID], SQLConditionType.Equal);
                        DBSeason.Clear(condition);

                        // Delete Local Series
                        condition = new SQLCondition();
                        condition.Add(new DBSeries(), DBSeries.cID, this[DBOnlineEpisode.cSeriesID], SQLConditionType.Equal);
                        DBSeries.Clear(condition);

                        // Delete Online Series
                        condition = new SQLCondition();
                        condition.Add(new DBOnlineSeries(), DBOnlineSeries.cID, this[DBOnlineEpisode.cSeriesID], SQLConditionType.Equal);
                        DBOnlineSeries.Clear(condition);

                        DBSeries.IsSeriesRemoved = true;
                    }
                }
            }
            #endregion

            return resultMsg;
        }

        public List<string> deleteLocalSubTitles()
        {
            List<string> resultMsg = new List<string>(); 

            if (String.IsNullOrEmpty(this[DBEpisode.cFilename]))
            {
                resultMsg.Add(Translation.EpisodeFilenameEmpty);
                return resultMsg;
            }
            fillSubTitleExtensions();

            string filenameNoExt = System.IO.Path.GetFileNameWithoutExtension(this[cFilename]);
            string path = string.Empty;
            try
            {
                path = System.IO.Path.GetDirectoryName(this[cFilename]);
                foreach (string file in System.IO.Directory.GetFiles(path, filenameNoExt + "*"))
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(file);
                    if (subTitleExtensions.Contains(fi.Extension.ToLower()))
                    {
                        try
                        {
                            System.IO.File.Delete(file);
                        }
                        catch
                        {
                            resultMsg.Add(string.Format(Translation.UnableToDeleteSubtitleFile, file));
                        }
                    }
                }
            }
            catch (Exception)
            {
                if (!String.IsNullOrEmpty(path))
                    resultMsg.Add(string.Format(Translation.PathNotAvailable, path));
            }
            return resultMsg;
        }

        public void HideEpisode(bool hide)
        {
            MPTVSeriesLog.Write(string.Format("{0} epsiode {1} from view", (hide ? "Hiding" : "UnHiding"), this));
            this[DBOnlineEpisode.cHidden] = hide;            
            this.Commit();

            // check if last episode is hidden and hide season
        }

        private void fillSubTitleExtensions()
        {
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
                        return StrFormatByteSize(this[cFileSizeBytes]);
                    //case cAvailableSubtitles:
                    //    return (this[cAvailableSubtitles] = checkHasSubtitles());
                    case cPrettyPlaytime:
                        return Helper.MSToMMSS(this["localPlaytime"]);
                    case cFilenameWOPath:
                        return System.IO.Path.GetFileName(this[cFilename]);
                    case cFilenameWOPathAndExtension:
                        return System.IO.Path.GetFileNameWithoutExtension(this[cFilename]);
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
                            if (!String.IsNullOrEmpty(value) && (!((string)value).IsNumerical() || Int32.Parse(value) != Int32.Parse(base[cEpisodeIndex]) + 1))
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
            if (!DBOption.GetOptions(DBOption.cShowHiddenItems))
            {
                //don't include hidden episodes unless the ShowHiddenItems option is set
                cond.Add(new DBOnlineEpisode(), DBOnlineEpisode.cHidden, 0, SQLConditionType.Equal);
            }
            string query = stdGetSQL(cond, false, true, "online_episodes.CompositeID, Watched, FirstAired");
            SQLiteResultSet results = DBTVSeries.Execute(query);

            epsTotal = 0;
            int parseResult = 0;
            int epsWatched = 0;

            // we either get two rows (one for normal episodes, one for double episodes), 
            // or we get no rows so we add them
            for (int i = 0; i < results.Rows.Count; i++)
            {
                // increment watched count if episode is watched
                if (int.TryParse(results.Rows[i].fields[1], out parseResult))
                {
                    epsWatched += parseResult;
                }
                
                Regex r = new Regex(@"(\d{4})-(\d{2})-(\d{2})");
                Match match = r.Match(results.Rows[i].fields[2]);
                DateTime firstAired;

                try
                {
                    if (match.Success)
                    {
                        // if episode airdate is in the future conditionally add to episode count
                        firstAired = new DateTime(Convert.ToInt32(match.Groups[1].Value), Convert.ToInt32(match.Groups[2].Value), Convert.ToInt32(match.Groups[3].Value));
                        if (firstAired < DateTime.Today || DBOption.GetOptions(DBOption.cCountEmptyAndFutureAiredEps))
                            epsTotal++;
                    }
                    else if (DBOption.GetOptions(DBOption.cCountEmptyAndFutureAiredEps))
                    {
                        // no airdate field set, this occurs for specials most of the time                   
                        epsTotal++;
                    }
                }
                catch
                {
                    // most likely invalid date in database 
                    epsTotal++;
                }

            }
            epsUnWatched = epsTotal - epsWatched;
            
            // this happens if for some reason an episode is marked as watched, but firstaired is in the future 
            // - or no firstaired provided
            if (epsUnWatched < 0)
                epsUnWatched = 0;
        }

        public static void GetSeriesEpisodeCounts(int series, out int epsTotal, out int epsUnWatched)
        {
            m_bUpdateEpisodeCount = true;

            SQLCondition cond = new SQLCondition(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series, SQLConditionType.Equal);            
            string query = stdGetSQL(cond, false, true, "online_episodes.CompositeID, Watched, FirstAired");
            SQLiteResultSet results = DBTVSeries.Execute(query);

            epsTotal = 0;
            int parseResult = 0;
            int epsWatched = 0;

            // we either get two rows (one for normal episodes, one for double episodes), 
            // or we get no rows so we add them
            for (int i = 0; i < results.Rows.Count; i++)
            {
                // increment watched count if episode is watched
                if (int.TryParse(results.Rows[i].fields[1], out parseResult))
                {
                    epsWatched += parseResult;
                }

                Regex r = new Regex(@"(\d{4})-(\d{2})-(\d{2})");
                Match match = r.Match(results.Rows[i].fields[2]);
                DateTime firstAired;

                try
                {
                    if (match.Success)
                    {
                        // if episode airdate is in the future conditionally add to episode count
                        firstAired = new DateTime(Convert.ToInt32(match.Groups[1].Value), Convert.ToInt32(match.Groups[2].Value), Convert.ToInt32(match.Groups[3].Value));
                        if (firstAired < DateTime.Today || DBOption.GetOptions(DBOption.cCountEmptyAndFutureAiredEps))
                            epsTotal++;
                    }
                    else if (DBOption.GetOptions(DBOption.cCountEmptyAndFutureAiredEps))
                    {
                        // no airdate field set, this occurs for specials most of the time                    
                        epsTotal++;
                    }
                }
                catch
                {
                    // most likely invalid date in database 
                    epsTotal++;
                }
        
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
                if (!DBOption.GetOptions(DBOption.cShowHiddenItems))
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
        
        #region Most Recent Helpers

        /// <summary>
        /// returns the 3 most recent episodes based on criteria
        /// </summary>        
        public static List<DBEpisode> GetMostRecent(MostRecentType type)
        {
            return GetMostRecent(type, 30, 3);
        }

        /// <summary>
        /// returns the most recent episodes based on criteria
        /// </summary>
        /// <param name="type">most recent type</param>
        /// <param name="days">number of days to look back in database</param>
        /// <param name="limit">number of results to return</param>        
        public static List<DBEpisode> GetMostRecent(MostRecentType type, int days, int limit)
        {
            return GetMostRecent(type, days, limit, false);
        }

        /// <summary>
        /// returns the most recent episodes based on criteria
        /// </summary>
        /// <param name="type">most recent type</param>
        /// <param name="days">number of days to look back in database</param>
        /// <param name="limit">number of results to return</param>
        /// <param name="unwatched">only get unwatched episodes (only used with recent added type)</param>
        public static List<DBEpisode> GetMostRecent(MostRecentType type, int days, int limit, bool unwatchedOnly)
        {
            // Create Time Span to lookup most recents
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            dt = dt.Subtract(new TimeSpan(days, 0, 0, 0, 0));
            string date = dt.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");

            // Create Conditions for SQL Query
            SQLCondition condition = new SQLCondition();
            switch (type)
            {
                case MostRecentType.Created:
                    condition.Add(new DBEpisode(), DBEpisode.cFileDateCreated, date, SQLConditionType.GreaterEqualThan);                    
                    if (unwatchedOnly)
                    {
                        condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cWatched, 1, SQLConditionType.NotEqual);
                    }
                    condition.AddOrderItem(DBEpisode.Q(DBEpisode.cFileDateCreated), SQLCondition.orderType.Descending);
                    break;

                case MostRecentType.Added:
                    condition.Add(new DBEpisode(), DBEpisode.cFileDateAdded, date, SQLConditionType.GreaterEqualThan);
                    if (unwatchedOnly)
                    {
                        condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cWatched, 1, SQLConditionType.NotEqual);
                    }
                    condition.AddOrderItem(DBEpisode.Q(cFileDateAdded), SQLCondition.orderType.Descending);
                    break;               

                case MostRecentType.Watched:
                    condition.Add(new DBEpisode(), DBEpisode.cDateWatched, date, SQLConditionType.GreaterEqualThan);
                    condition.AddOrderItem(DBEpisode.Q(DBEpisode.cDateWatched), SQLCondition.orderType.Descending);
                    break;
            }
            
            // Only get from database what we need
            condition.SetLimit(limit);

            return Get(condition, false);
        }        
        #endregion
        public static List<DBEpisode> GetAll()
        {
            return Get(new SQLCondition(), true);
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
