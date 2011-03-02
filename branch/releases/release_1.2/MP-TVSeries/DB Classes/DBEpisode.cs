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

namespace WindowPlugins.GUITVSeries
{
    public class DBOnlineEpisode : DBTable
    {
        public const String cTableName = "online_episodes";

        public const String cCompositeID = "CompositeID";         // composite string used for primary index, based on series, season & episode
        public const String cSeriesID = "SeriesID";
        public const String cID = "EpisodeID";                    // onlineDB episodeID
        public const String cSeasonIndex = "SeasonIndex";         // season index
        public const String cEpisodeIndex = "EpisodeIndex";       // episode index
        public const String cEpisodeName = "EpisodeName";         // episode name
        public const String cWatched = "Watched";          // tag to know if episode has been watched already (overrides the local file's tag)
        public const String cEpisodeSummary = "Summary";
        public const String cFirstAired = "FirstAired";
        public const String cOnlineDataImported = "OnlineDataImported";
        public const String cGuestStars = "GuestStars";
        public const String cDirector = "Director";
        public const String cWriter = "Writer";
        public const String cHidden = "Hidden";
        public const String cLastUpdated = "lastupdated";
        public const String cDownloadPending = "DownloadPending";
        public const String cEpisodeImageFilename = "filename";

        public const String cAirsBeforeSeason = "airsbefore_season";
        public const String cAirsBeforeEpisode = "airsbefore_episode";

        public static Dictionary<String, String> s_OnlineToFieldMap = new Dictionary<String, String>();
        public static Dictionary<string, DBField> s_fields = new Dictionary<string,DBField>();

        static DBOnlineEpisode()
        {
            s_OnlineToFieldMap.Add("SeasonNumber", cSeasonIndex);
            s_OnlineToFieldMap.Add("EpisodeNumber", cEpisodeIndex);
            s_OnlineToFieldMap.Add("EpisodeName", cEpisodeName);
            s_OnlineToFieldMap.Add("id", cID);
            s_OnlineToFieldMap.Add("Overview", cEpisodeSummary);
            s_OnlineToFieldMap.Add("FirstAired", cFirstAired);

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
                if (this[cEpisodeImageFilename].ToString().Length > 0)
				{
                    return Helper.PathCombine(Settings.GetPath(Settings.Path.banners), this[cEpisodeImageFilename]);
				} else return string.Empty;
			}
		}

        public string CompleteTitle
        {
            get
            {
                return Helper.getCorrespondingSeries(this[DBOnlineEpisode.cSeriesID])[DBOnlineSeries.cPrettyName] + " " + this[DBOnlineEpisode.cSeasonIndex] + "x" + this[DBOnlineEpisode.cEpisodeIndex] + ": " + this[DBOnlineEpisode.cEpisodeName];
            }
        }
        public override string ToString()
        {
            return this[DBOnlineEpisode.cCompositeID];
        }
    };

    public class DBEpisode : DBTable, cache.ICacheable<DBEpisode>
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
        public const int cDBVersion = 5;

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

        public const String cVideoWidth = "videoWidth";
        public const String cVideoHeight = "videoHeight";
        public const String cStopTime = "StopTime";

        public const String cFileSizeBytes = "FileSizeB";
        public const String cFileSize = "FileSize";

        public const String cPrettyPlaytime = "PrettyLocalPlaytime";

        private DBOnlineEpisode m_onlineEpisode = null;

        public static Dictionary<String, String> s_FieldToDisplayNameMap = new Dictionary<String, String>();
        public static Dictionary<string, DBField> s_fields = new Dictionary<string, DBField>();

        public delegate void dbEpisodeUpdateOccuredDelegate(DBEpisode updated);
        public static event dbEpisodeUpdateOccuredDelegate dbEpisodeUpdateOccured;

        public static List<string> subTitleExtensions = new List<string>();

        public List<string> cachedLogoResults = null;
        public string cachedFirstLogo = null;

        static MediaInfoLib.MediaInfo MI;
        const int maxMIAttempts = 6;

        static DBEpisode()
        {
            // make sure the table is created on first run
            DBEpisode dummy = new DBEpisode();

            s_FieldToDisplayNameMap.Add(cFilename, "Local FileName");
            s_FieldToDisplayNameMap.Add(cCompositeID, "Composite Episode ID");
            s_FieldToDisplayNameMap.Add(cSeriesID, "Series ID");
            s_FieldToDisplayNameMap.Add(cSeasonIndex, "Season Index");
            s_FieldToDisplayNameMap.Add(cEpisodeIndex, "Episode Index");
            s_FieldToDisplayNameMap.Add(cEpisodeName, "Episode Name");
            s_FieldToDisplayNameMap.Add(DBOnlineEpisode.cID, "Episode ID");
            s_FieldToDisplayNameMap.Add(DBOnlineEpisode.cEpisodeSummary, "Overview");

            int nCurrentDBEpisodeVersion = cDBVersion;
            while (DBOption.GetOptions(DBOption.cDBEpisodesVersion) != nCurrentDBEpisodeVersion)
                // take care of the upgrade in the table
                switch ((int)DBOption.GetOptions(DBOption.cDBEpisodesVersion))
                {
                    case 1:
                        // upgrade to version 2; clear the series table (we use 2 other tables now)
                        try
                        {
                            String sqlQuery = "DROP TABLE " + cTableName;
                            DBTVSeries.Execute(sqlQuery);
                            sqlQuery = "DROP TABLE " + DBOnlineEpisode.cTableName;
                            DBTVSeries.Execute(sqlQuery);
                            DBOption.SetOptions(DBOption.cDBEpisodesVersion, nCurrentDBEpisodeVersion);
                        }
                        catch { }
                        break;

                    case 2:
                        DBOnlineEpisode.GlobalSet(new DBOnlineEpisode(), DBOnlineEpisode.cHidden, 0, new SQLCondition());
                        DBOption.SetOptions(DBOption.cDBEpisodesVersion, nCurrentDBEpisodeVersion);
                        break;

                    case 3:
                        DBEpisode.GlobalSet(new DBEpisode(), DBEpisode.cEpisodeIndex2, 0, new SQLCondition());
                        DBOption.SetOptions(DBOption.cDBEpisodesVersion, nCurrentDBEpisodeVersion);
                        break;

                    case 4:
                        DBEpisode.GlobalSet(new DBOnlineEpisode(), DBOnlineEpisode.cDownloadPending, 0, new SQLCondition());
                        DBOption.SetOptions(DBOption.cDBEpisodesVersion, nCurrentDBEpisodeVersion);
                        break;

                    default:
                        break;
                }
            // create the dll interop for getting MediaInfo
            try
            {
               MI = new MediaInfoLib.MediaInfo();
            }
            catch(Exception ex)
            {
                // if it fails, most likely the dll is not in the correct folder
                MPTVSeriesLog.Write("Failed to create MediaInfo Object: ", ex.Message, MPTVSeriesLog.LogLevel.Normal);
            }
        }

        public static String PrettyFieldName(String sFieldName)
        {
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
        }

        public DBEpisode(bool bCreateEmptyOnline)
            : base(cTableName)
        {
            InitColumns();
            InitValues();
            if (bCreateEmptyOnline)
                m_onlineEpisode = new DBOnlineEpisode();
        }

        public DBEpisode(String filename)
            : base(cTableName)
        {
            InitColumns();
            if (!ReadPrimary(filename))
                InitValues();
            if (System.IO.File.Exists(filename) && !mediaInfoIsSet) readMediaInfoOfLocal();
            if (base[cSeriesID].ToString().Length > 0 && base[cSeasonIndex] != -1 && base[cEpisodeIndex] != -1)
            {
                m_onlineEpisode = new DBOnlineEpisode(base[cSeriesID], base[cSeasonIndex], base[cEpisodeIndex]);
                base[cCompositeID] = m_onlineEpisode[DBOnlineEpisode.cCompositeID];
            }
        }

        public void ChangeSeriesID(int nSeriesID)
        {
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
            if (base[DBEpisode.cCompositeID2].ToString().Length > 0)
                base[DBEpisode.cCompositeID2] = nSeriesID + "_" + base[DBEpisode.cSeasonIndex] + "x" + base[DBEpisode.cEpisodeIndex2];
            m_onlineEpisode = newOnlineEpisode;
            Commit();
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
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
            base[cSeasonIndex] = -1;
            base[cEpisodeIndex] = -1;
        }

        public bool mediaInfoIsSet
        {
            get
            {
                if (this["localPlaytime"].ToString().Length == 0 // ||
                //    this["VideoCodec"] == "" ||
                //    this["VideoBitrate"] == "" ||
                //    this["VideoFrameRate"] == "" ||
                //    this["videoWidth"] == "" ||
                //    this["videoHeight"] == "" ||
                //    this["VideoAspectRatio"] == "" ||
                //    this["AudioCodec"] == "" ||
                //    this["AudioChannels"] == "" ||
                //    this["AudioBitrate"] == ""
                    ) return false;
                else
                {
                    int noAttempts = 0;
                    if (!int.TryParse(this["localPlaytime"], out noAttempts)) return true;
                    if (noAttempts >= 0 - maxMIAttempts && noAttempts < 0) return false; // we attempt to readout maxMIAttempts times
                    return true;
                }

            }
        }

        public bool readMediaInfoOfLocal()
        {
            if (null == MI) return false; // MediaInfo Object could not be created

            if(System.IO.File.Exists(this[DBEpisode.cFilename]))
            {
                
                try
                {
                    MPTVSeriesLog.Write("Attempting to read Mediainfo for ", this[DBEpisode.cFilename].ToString(), MPTVSeriesLog.LogLevel.Normal);
                    MI.Open(this[DBEpisode.cFilename]);
                    string result = string.Empty;
                    int noAttempts = 0;
                    int.TryParse(this["localPlaytime"], out noAttempts);
                    noAttempts--;
                    bool failed = false;
                    this["localPlaytime"] = (result = MI.getPlaytime()).Length > 0 ? result : noAttempts.ToString();
                    if (result.Length > 0)
                    {
                        this["VideoCodec"] = (result = MI.getVidCodec()).Length > 0 ? result : "-1";
                        this["VideoBitrate"] = (result = MI.getVidBitrate()).Length > 0 ? result : "-1";
                        this["VideoFrameRate"] = (result = MI.getFPS()).Length > 0 ? result : "-1";
                        this["videoWidth"] = (result = MI.getWidth()).Length > 0 ? result : "-1"; // lower case for compat. with older version
                        this["videoHeight"] = (result = MI.getHeight()).Length > 0 ? result : "-1";
                        this["VideoAspectRatio"] = (result = MI.getAR()).Length > 0 ? result : "-1";

                        this["AudioCodec"] = (result = MI.getAudioCodec()).Length > 0 ? result : "-1";
                        this["AudioBitrate"] = (result = MI.getAudioBitrate()).Length > 0 ? result : "-1";
                        this["AudioChannels"] = (result = MI.getNoChannels()).Length > 0 ? result : "-1";
                    }
                    else failed = true;
                    MI.Close();

                    if (failed)
                    {
                        MPTVSeriesLog.Write("Problem parsing ", this[DBEpisode.cFilename].ToString(), MPTVSeriesLog.LogLevel.Normal);
                        int retry = maxMIAttempts - (noAttempts * -1);
                        if(retry > 0)
                            MPTVSeriesLog.Write("This file will be retried: ", retry.ToString() + " times", MPTVSeriesLog.LogLevel.Normal);
                        else
                            MPTVSeriesLog.Write("This file will NOT be retried, you can however force a manual readout.");

                    }
                    else MPTVSeriesLog.Write("Succesfully read Mediainfo for ", this[DBEpisode.cFilename].ToString(), MPTVSeriesLog.LogLevel.Normal);

                    Commit();
                    
                    return true;
                }
                catch (Exception ex)
                {
                    MPTVSeriesLog.Write("Error reading Mediainfo ", ex.Message, MPTVSeriesLog.LogLevel.Normal);
                }
                
            }
            return false;

        }

        public bool checkHasSubtitles()
        {
            if (Helper.String.IsNullOrEmpty(this[DBEpisode.cFilename])) return false;
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
            bool exists = false;
            string filenameNoExt = System.IO.Path.GetFileNameWithoutExtension(this[cFilename]);
            filenameNoExt = System.IO.Path.GetDirectoryName(this[cFilename]) + "\\" + filenameNoExt;
            for (int i = 0; i < subTitleExtensions.Count; i++)
            {
                if (System.IO.File.Exists(filenameNoExt + subTitleExtensions[i]))
                {
                    exists = true;
                    break;
                }
            }
            return exists;
        }

        public DBOnlineEpisode onlineEpisode
        {
            get { return m_onlineEpisode; }
        }

        public override List<String> FieldNames
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
                        return new System.IO.FileInfo(base[DBEpisode.cFilename]).Length;
                    case cFileSize:
                        return StrFormatByteSize(new System.IO.FileInfo(base[DBEpisode.cFilename]).Length);
                    case cAvailableSubtitles:
                        return (this[cAvailableSubtitles] = checkHasSubtitles());
                    case cPrettyPlaytime:
                        return Helper.MSToMMSS(this["localPlaytime"]);
                }
                // online data always takes precedence over the local file data
                if (m_onlineEpisode != null)
                {
                    DBValue retVal = null;
                    switch (fieldName)
                    {
                        case cEpisodeName:
                            retVal = m_onlineEpisode[DBOnlineEpisode.cEpisodeName];
                            if (Helper.String.IsNullOrEmpty(retVal))
                                retVal = base[cEpisodeName];
                            return retVal;

                        default:
                            retVal = m_onlineEpisode[fieldName];
                            if (Helper.String.IsNullOrEmpty(retVal))
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
                        case cEpisodeIndex2:
                        case cCompositeID2:
                            // the only flags we are not rerouting to the onlineEpisode if it exists
                            break;


                        default:
                            if (m_onlineEpisode.m_fields.ContainsKey(fieldName))
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
                    episode.m_onlineEpisode.Read(ref results, index);
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
                if (!Settings.isConfig && DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles))
                    conditions.Add(new DBEpisode(), DBEpisode.cFilename, string.Empty, SQLConditionType.NotEqual);

                // include hidden?
                if (!Settings.isConfig || !DBOption.GetOptions(DBOption.cShowHiddenItems))
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cHidden, 0, SQLConditionType.Equal);

                return conditions;
            }
        }

        public static string stdGetSQL(SQLCondition conditions, bool selectFull)
        {
            return stdGetSQL(conditions, selectFull, true);
        }

        public static string stdGetSQL(SQLCondition conditions, bool selectFull, bool inclStdCond)
        {
            String sqlQuery = String.Empty;
            conditions.AddCustom(stdConditions.ConditionsSQLString);

            string orderBy = string.Empty;
            if(selectFull)
            {
                orderBy = !conditions.customOrderStringIsSet
                      ? string.Empty
                      : conditions.orderString;

                if (Helper.String.IsNullOrEmpty(orderBy))
                    orderBy = " order by " + DBOnlineEpisode.Q(cEpisodeIndex);

                SQLWhat what = new SQLWhat(new DBOnlineEpisode());
                what.AddWhat(new DBEpisode());
                // one query gets both first & second episode
                sqlQuery = "select " + what;
            }
            else
            {
                sqlQuery = "select " + DBOnlineEpisode.cTableName + "." + DBOnlineEpisode.cEpisodeIndex + " from " + DBOnlineEpisode.cTableName;
            }
            return sqlQuery + " left join " + cTableName + " on (" + DBEpisode.Q(cCompositeID) + "==" + DBOnlineEpisode.Q(cCompositeID) + " or " + DBEpisode.Q(cCompositeID2) + "==" + DBOnlineEpisode.Q(cCompositeID) + ") " + conditions + orderBy + conditions.limitString;
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
            return Get(stdGetSQL(conditions, true, includeStdCond));
        }

        public static List<DBEpisode> Get(string sqlQuery)
        {
            //MPTVSeriesLog.Write(sqlQuery);
            SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
            List<DBEpisode> outList = new List<DBEpisode>();
            if (results.Rows.Count > 0)
            {
                for (int index = 0; index < results.Rows.Count; index++)
                {
                    DBEpisode episode = new DBEpisode();
                    episode.Read(ref results, index);
                    episode.m_onlineEpisode = new DBOnlineEpisode();
                    episode.m_onlineEpisode.Read(ref results, index);
                    outList.Add(episode);
                }
            }
            return outList;
        }

        public override string ToString()
        {
            return this[DBEpisode.cCompositeID];
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
