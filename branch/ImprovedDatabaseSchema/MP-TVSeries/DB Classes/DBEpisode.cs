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
using System.Text.RegularExpressions;
using System.IO;
using WindowPlugins.GUITVSeries.DataBase;

namespace WindowPlugins.GUITVSeries.DataClass
{
	public class DBEpisode : DBTable, ICacheable<DBEpisode>
	{
		#region ICacheable
		private static void overRide(DBEpisode old, DBEpisode newObject)
		{
		    old = newObject;
		}

		public DBEpisode fullItem
		{
		    get { return this; }
		    set { overRide(this, value); }
		}
		#endregion

		public const String cTableName = "local_episodes";
        public const String cOutName = "Episode";

        #region Local DB Fields
		//declare fieldsnames as constants here, and then add them to TableFields
        public const String cID = "Id";
        public const String cFilename = "EpisodeFilename";
        public const String cCompositeID = DBOnlineEpisode.cCompositeID;           // composite string used for link key to online episode data
        public const String cSeriesID = DBOnlineEpisode.cSeriesID;
        public const String cSeasonIndex = DBOnlineEpisode.cSeasonIndex;
        public const String cEpisodeIndex = DBOnlineEpisode.cEpisodeIndex;
        public const String cEpisodeName = "LocalEpisodeName";
        public const String cImportProcessed = "LocalImportProcessed";
        public const String cAvailableSubtitles = "AvailableSubtitles";

        // ids for the second episode if it's a double (and please don't ever do triple episodes)
        public const String cCompositeID2 = DBOnlineEpisode.cCompositeID + "2";
        public const String cEpisodeIndex2 = DBOnlineEpisode.cEpisodeIndex + "2";

        public const String cVideoWidth = "videoWidth";
        public const String cVideoHeight = "videoHeight";
        public const String cFileDateAdded = "FileDateAdded";
        public const String cFileDateCreated = "FileDateCreated";
        public const String cIsAvailable = "IsAvailable";

        public const String cExtension = "ext";
        public const String cIsOnRemovable = "Removable";
        public const String cLocalPlaytime = "localPlaytime";
        public const String cStopTime = "StopTime";
        public const String cVolumeLabel = "VolumeLabel";

        public const String cVideoBitRate = "VideoBitrate";
        public const String cVideoFrameRate = "VideoFrameRate";
        public const String cVideoAspectRatio = "VideoAspectRatio";
        public const String cVideoCodec = "VideoCodec";
        public const String cAudioCodec = "AudioCodec";
        public const String cAudioBitrate = "AudioBitrate";
        public const String cAudioChannels = "AudioChannels";
        public const String cAudioTracks = "AudioTracks";
        public const String cTextCount = "TextCount";
                
        //virtual fields - not in the database don't add them to tablefields
        public const String cFileSizeBytes = "FileSizeB";
        public const String cFileSize = "FileSize";
        public const String cPrettyPlaytime = "PrettyLocalPlaytime";
        public const String cFilenameWOPath = "EpisodeFilenameWithoutPath";
        
        // all mandatory fields. Place the primary key first - it's just good manners
		public static readonly DBFieldDefList TableFields = new DBFieldDefList {
            {cID,                 new DBFieldDef{ FieldName = cID,                TableName = cTableName, Type = DBFieldType.Int,     Primary = true,     AutoIncrement = true }},
            {cFilename,           new DBFieldDef{ FieldName = cFilename,          TableName = cTableName, Type = DBFieldType.String,  PrettyName = "Local FileName",		Indexed = true}},
            {cCompositeID,        new DBFieldDef{ FieldName = cCompositeID,       TableName = cTableName, Type = DBFieldType.String,  PrettyName = "Composite Episode ID", Indexed = true}},
            {cSeriesID,           new DBFieldDef{ FieldName = cSeriesID,          TableName = cTableName, Type = DBFieldType.Int,     PrettyName = "Series ID"}},
            {cSeasonIndex,        new DBFieldDef{ FieldName = cSeasonIndex,       TableName = cTableName, Type = DBFieldType.Int,     PrettyName = "Season Index",		Default = -1}},
            {cEpisodeIndex,       new DBFieldDef{ FieldName = cEpisodeIndex,      TableName = cTableName, Type = DBFieldType.Int,     PrettyName = "Episode Index",	Default = -1}},
            {cEpisodeName,        new DBFieldDef{ FieldName = cEpisodeName,       TableName = cTableName, Type = DBFieldType.String,  PrettyName = "Episode Name"}},
            {cImportProcessed,    new DBFieldDef{ FieldName = cImportProcessed,   TableName = cTableName, Type = DBFieldType.Int}},
            {cAvailableSubtitles, new DBFieldDef{ FieldName = cAvailableSubtitles,TableName = cTableName, Type = DBFieldType.String,  PrettyName = "Subtitles Available"}},
            {cCompositeID2,       new DBFieldDef{ FieldName = cCompositeID2,      TableName = cTableName, Type = DBFieldType.String,  Indexed = true}},
            {cEpisodeIndex2,      new DBFieldDef{ FieldName = cEpisodeIndex2,     TableName = cTableName, Type = DBFieldType.Int}},
            {cVideoWidth,         new DBFieldDef{ FieldName = cVideoWidth,        TableName = cTableName, Type = DBFieldType.Int,     PrettyName = "Video Width"}},
            {cVideoHeight,        new DBFieldDef{ FieldName = cVideoHeight,       TableName = cTableName, Type = DBFieldType.Int,     PrettyName = "Video Height"}},
            {cFileDateAdded,      new DBFieldDef{ FieldName = cFileDateAdded,     TableName = cTableName, Type = DBFieldType.String,  PrettyName = "Date Added"}},
            {cFileDateCreated,    new DBFieldDef{ FieldName = cFileDateCreated,   TableName = cTableName, Type = DBFieldType.String,  PrettyName = "Date Created"}},
            {cIsAvailable,        new DBFieldDef{ FieldName = cIsAvailable,       TableName = cTableName, Type = DBFieldType.Int}},
            {cExtension,          new DBFieldDef{ FieldName = cExtension,         TableName = cTableName, Type = DBFieldType.String}},
            {cIsOnRemovable,      new DBFieldDef{ FieldName = cIsOnRemovable,     TableName = cTableName, Type = DBFieldType.Int}},
            {cVolumeLabel,        new DBFieldDef{ FieldName = cVolumeLabel,       TableName = cTableName, Type = DBFieldType.String,  PrettyName = "Volume Label"}},
            {cLocalPlaytime,      new DBFieldDef{ FieldName = cLocalPlaytime,     TableName = cTableName, Type = DBFieldType.Int}},
            {cStopTime,           new DBFieldDef{ FieldName = cStopTime,          TableName = cTableName, Type = DBFieldType.Int}},

            {cVideoCodec,         new DBFieldDef{ FieldName = cVideoCodec,        TableName = cTableName, Type = DBFieldType.String,  PrettyName = "Video Codec"}},
            {cVideoBitRate,       new DBFieldDef{ FieldName = cVideoBitRate,      TableName = cTableName, Type = DBFieldType.String,  PrettyName = "Video Bit Rate"}},
            {cVideoFrameRate,     new DBFieldDef{ FieldName = cVideoFrameRate,    TableName = cTableName, Type = DBFieldType.String,  PrettyName = "Video Frame Rate"}},
            {cVideoAspectRatio,   new DBFieldDef{ FieldName = cVideoAspectRatio,  TableName = cTableName, Type = DBFieldType.String,  PrettyName = "Video Aspect Ratio"}},
            {cAudioCodec,         new DBFieldDef{ FieldName = cAudioCodec,        TableName = cTableName, Type = DBFieldType.String,  PrettyName = "Audio Codec"}},
            {cAudioBitrate,       new DBFieldDef{ FieldName = cAudioBitrate,      TableName = cTableName, Type = DBFieldType.String,  PrettyName = "Audio Bitrate"}},
            {cAudioChannels,      new DBFieldDef{ FieldName = cAudioChannels,     TableName = cTableName, Type = DBFieldType.String,  PrettyName = "Audio Channels"}},
            {cAudioTracks,        new DBFieldDef{ FieldName = cAudioTracks,       TableName = cTableName, Type = DBFieldType.String,  PrettyName = "Audio Tracks"}},
            {cTextCount,          new DBFieldDef{ FieldName = cTextCount,         TableName = cTableName, Type = DBFieldType.String,  PrettyName = "Subtitle Count"}},
        };
        #endregion

        private DBOnlineEpisode m_onlineEpisode = null;

        public delegate void dbEpisodeUpdateOccuredDelegate(DBEpisode updated);
        public static event dbEpisodeUpdateOccuredDelegate dbEpisodeUpdateOccured;

        private static readonly List<string> subTitleExtensions = new List<string> {
        	".aqt", ".asc", ".ass", ".dat", ".dks", ".js",  ".jss", ".lrc", ".mpl", ".ovr", ".pan", ".pjs", ".psb", ".rt",  ".rtf", 
			".s2k", ".sbt", ".scr", ".smi", ".son", ".srt", ".ssa", ".sst", ".ssts",".stl", ".sub", ".txt", ".vkt", ".vsf", ".zeg"
		};

        public List<string> cachedLogoResults = null;
        public string cachedFirstLogo = null;

        public const int MAX_MEDIAINFO_RETRIES = 5;

        private static bool m_bUpdateEpisodeCount = false; // used to ensure StdConds are used while in Config mode

        static DBEpisode()
        {
        	DatabaseUpgrade();
		}

		#region deprecated database upgrade method - use MaintainDatabaseTable instead
		private const int cDBVersion = 6;
		/// <summary>
		/// deprecated database upgrade method - use MaintainDatabaseTable instead
    	/// </summary>
		private static void DatabaseUpgrade()
    	{
    		const int nCurrentDBVersion = cDBVersion;
    		int nUpgradeDBVersion = DBOption.GetOptions(DBOption.cDBEpisodesVersion);

    		if (nUpgradeDBVersion == nCurrentDBVersion) {
    			return;
    		}
    		while (nUpgradeDBVersion != nCurrentDBVersion)
    			// take care of the upgrade in the table
    			switch (nUpgradeDBVersion) {
    				case 1:
    					// upgrade to version 2; clear the series table (we use 2 other tables now)
    					try {
    						String sqlQuery = "DROP TABLE " + cTableName;
    						DBTVSeries.Execute(sqlQuery);
    						sqlQuery = "DROP TABLE " + DBOnlineEpisode.cTableName;
    						DBTVSeries.Execute(sqlQuery);
    						nUpgradeDBVersion++;
    					} catch {
    					}
    					break;

    				case 2:
    					DBOnlineEpisode.GlobalSet(DBOnlineEpisode.TableFields, DBOnlineEpisode.cHidden, 0, new SQLCondition());
    					nUpgradeDBVersion++;
    					break;

    				case 3:
    					DBEpisode.GlobalSet(DBEpisode.TableFields, DBEpisode.cEpisodeIndex2, 0, new SQLCondition());
    					nUpgradeDBVersion++;
    					break;

    				case 4:
						DBOnlineEpisode.GlobalSet(DBOnlineEpisode.TableFields, DBOnlineEpisode.cDownloadPending, 0, new SQLCondition());
    					nUpgradeDBVersion++;
    					break;

    				case 5:
						DBOnlineEpisode.GlobalSet(DBOnlineEpisode.TableFields, DBOnlineEpisode.cEpisodeThumbnailUrl, (DBValue)"init", new SQLCondition());
						DBOnlineEpisode.GlobalSet(DBOnlineEpisode.TableFields, DBOnlineEpisode.cEpisodeThumbnailFilename, (DBValue)"", new SQLCondition());
    					nUpgradeDBVersion++;
    					break;

    				default:
    					nUpgradeDBVersion = nCurrentDBVersion;
    					break;
    			}
    		DBOption.SetOptions(DBOption.cDBEpisodesVersion, nCurrentDBVersion);
		}
		#endregion

		internal static void MaintainDatabaseTable(Version lastVersion)
        {
            try {
                //test for table existance
                if (!DatabaseHelper.TableExists(cTableName)) {
					DatabaseHelper.CreateTable(cTableName, TableFields.Values);
					DatabaseHelper.CreateIndexes(cTableName, TableFields.Values);
					return;
                }
				MPTVSeriesLog.Write("Upgrading Database Table " + cTableName);

                if (lastVersion < new Version("2.6.0.1044")) {
					//needs to be done before DBTable constructor is called
					//to prevent columns being added to the database

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

        public DBEpisode()
			: base(cTableName, TableFields)
        {
        }

        public DBEpisode(bool bCreateEmptyOnline)
			: base(cTableName, TableFields)
        {
            if (bCreateEmptyOnline)
                m_onlineEpisode = new DBOnlineEpisode();
        }

        public DBEpisode(DBOnlineEpisode onlineEpisode, String filename)
			: base(cTableName, TableFields)
        {
			ReadFilename(filename);
            
            //if (System.IO.File.Exists(filename) && !HasMediaInfo && !Helper.IsImageFile(filename))
            //    ReadMediaInfo();

            //composite id will bw set automatically from setting these three
            this[DBEpisode.cSeriesID] = onlineEpisode[DBOnlineEpisode.cSeriesID];
            this[DBEpisode.cSeasonIndex] = onlineEpisode[DBOnlineEpisode.cSeasonIndex];
            this[DBEpisode.cEpisodeIndex] = onlineEpisode[DBOnlineEpisode.cEpisodeIndex];
            m_onlineEpisode = onlineEpisode;
        }

        public DBEpisode(String filename, bool bSkipMediaInfo)
			: base(cTableName, TableFields)
        {
			ReadFilename(filename);
           
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
            string composite = base[cSeriesID] + "_" + seasonIndex + "x" + episodeIndex;
            base[cSeasonIndex] = seasonIndex;
            base[cEpisodeIndex] = episodeIndex;
            base[cCompositeID] = composite;

            DBOnlineEpisode newOnlineEpisode = new DBOnlineEpisode();
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
                newOnlineEpisode[cEpisodeIndex] = base[cEpisodeIndex];
                newOnlineEpisode[cCompositeID] = base[cCompositeID];
            }
            m_onlineEpisode = newOnlineEpisode;
            Commit();
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
                        this[cVideoCodec] = MI.VideoCodec;
                        this[cVideoBitRate] = MI.VideoBitrate;
                        this[cVideoFrameRate] = MI.VideoFramesPerSecond;
                        this[cVideoWidth] = MI.VideoWidth;
                        this[cVideoHeight] = MI.VideoHeight;
                        this[cVideoAspectRatio] = MI.VideoAspectRatio;

                        this[cAudioCodec] = MI.AudioCodec;
                        this[cAudioBitrate] = MI.AudioBitrate;
                        this[cAudioChannels] = MI.AudioChannelCount;
                        this[cAudioTracks] = MI.AudioStreamCount;

                        this[cTextCount] = MI.SubtitleCount;
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
            return checkHasSubtitles(true);
        }

        public bool checkHasLocalSubtitles()
        {
            return checkHasSubtitles(false);
        }

        public bool checkHasSubtitles(bool useMediaInfo)
        {
            if (String.IsNullOrEmpty(this[DBEpisode.cFilename])) return false;

            // Read MediaInfo for embedded subtitles
            if (useMediaInfo && !String.IsNullOrEmpty(this["TextCount"]))
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

    	static bool isWritable(FileInfo fileInfo)
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
            return isWritable(fi);
        }

        public List<string> deleteEpisode(TVSeriesPlugin.DeleteMenuItems type)
        {
            List<string> resultMsg = new List<string>(); 

            // Always delete from Local episode table if deleting from disk or database
            SQLCondition condition = new SQLCondition();
            condition.Add(DBEpisode.TableFields, DBEpisode.cID, this[DBEpisode.cID], SQLConditionType.Equal);

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
                            condition1.Add(DBOnlineEpisode.TableFields, DBOnlineEpisode.cOnlineID, this[DBOnlineEpisode.cOnlineID], SQLConditionType.Equal);
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
					condition.Add(DBOnlineEpisode.TableFields, DBOnlineEpisode.cOnlineID, this[DBOnlineEpisode.cOnlineID], SQLConditionType.Equal);
                    DBOnlineEpisode.Clear(condition);
                }
            }

            #region Cleanup
            if (type != TVSeriesPlugin.DeleteMenuItems.disk)
            {
                // If episode count is zero then delete the season
                condition = new SQLCondition();
				condition.Add(DBOnlineEpisode.TableFields, DBOnlineEpisode.cSeriesID, this[DBOnlineEpisode.cSeriesID], SQLConditionType.Equal);
				condition.Add(DBOnlineEpisode.TableFields, DBOnlineEpisode.cSeasonIndex, this[DBOnlineEpisode.cSeasonIndex], SQLConditionType.Equal);
                episodes = DBEpisode.Get(condition, false);
                if (episodes.Count == 0)
                {
                    condition = new SQLCondition();
                    condition.Add(DBSeason.TableFields, DBSeason.cSeriesID, this[DBOnlineEpisode.cSeriesID], SQLConditionType.Equal);
					condition.Add(DBSeason.TableFields, DBSeason.cIndex, this[DBOnlineEpisode.cSeasonIndex], SQLConditionType.Equal);
                    DBSeason.Clear(condition);

                    // If episode count is still zero, then delete the series\seasons
                    condition = new SQLCondition();
                    condition.Add(DBEpisode.TableFields, DBEpisode.cSeriesID, this[DBOnlineEpisode.cSeriesID], SQLConditionType.Equal);
                    episodes = DBEpisode.Get(condition, false);
                    if (episodes.Count == 0)
                    {
                        // Delete All Seasons
                        condition = new SQLCondition();
						condition.Add(DBSeason.TableFields, DBSeason.cSeriesID, this[DBOnlineEpisode.cSeriesID], SQLConditionType.Equal);
                        DBSeason.Clear(condition);

                        // Delete Local Series
                        condition = new SQLCondition();
                        condition.Add(DBSeries.TableFields, DBSeries.cID, this[DBOnlineEpisode.cSeriesID], SQLConditionType.Equal);
                        DBSeries.Clear(condition);

                        // Delete Online Series
                        condition = new SQLCondition();
                        condition.Add(DBOnlineSeries.TableFields, DBOnlineSeries.cID, this[DBOnlineEpisode.cSeriesID], SQLConditionType.Equal);
                        DBOnlineSeries.Clear(condition);
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
                outList.Add(cID);
                outList.Add(cSeriesID);
                outList.Add(cSeasonIndex);
                outList.Add(cEpisodeIndex);
                outList.Add(cEpisodeIndex2);
                outList.Add(cCompositeID);
                outList.Add(cCompositeID2);

                foreach (DBField field in m_fields.Values)
                {
                    if (outList.IndexOf(field.FieldName) == -1)
                        outList.Add(field.FieldName);
                }
                if (m_onlineEpisode != null)
                {
                    foreach (DBField field in m_onlineEpisode.m_fields.Values)
                    {
                        if (outList.IndexOf(field.FieldName) == -1)
                            outList.Add(field.FieldName);
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
            	if (m_onlineEpisode == null) {
            		return base[fieldName];
            	}
            	DBValue retVal = null;
            	switch (fieldName) {
            		case cEpisodeName:
            			retVal = m_onlineEpisode[DBOnlineEpisode.cEpisodeName];
            			if (String.IsNullOrEmpty(retVal)) {
            				retVal = base[cEpisodeName];
            			}
            			return retVal;

            		default:
            			retVal = m_onlineEpisode[fieldName];
            			if (String.IsNullOrEmpty(retVal)) {
            				retVal = base[fieldName];
            			}
            			return retVal;
            	}
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
                    cleanup.Add(DBOnlineEpisode.TableFields, DBOnlineEpisode.cSeriesID, base[cSeriesID], SQLConditionType.Equal);
					cleanup.Add(DBOnlineEpisode.TableFields, DBOnlineEpisode.cCompositeID, base[cSeriesID] + "_" + base[DBOnlineEpisode.cFirstAired], SQLConditionType.Equal);
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
            conditions.Add(DBEpisode.TableFields, DBEpisode.cSeriesID, new DBValue(seriesID), SQLConditionType.Equal);
            List<DBEpisode> results = GetFirstUnwatched(conditions);
            if (results.Count > 0)
                return results[0];
        	return null;
        }

        public static DBEpisode GetFirstUnwatched(int seriesID, int seasonIndex)
        {
            SQLCondition conditions = new SQLCondition();
			conditions.Add(DBEpisode.TableFields, DBEpisode.cSeriesID, new DBValue(seriesID), SQLConditionType.Equal);
			conditions.Add(DBEpisode.TableFields, DBEpisode.cSeasonIndex, new DBValue(seasonIndex), SQLConditionType.Equal);
            List<DBEpisode> results = GetFirstUnwatched(conditions);
            if (results.Count > 0)
                return results[0];
        	return null;
        }

        public static List<DBEpisode> GetFirstUnwatched()
        {
            return GetFirstUnwatched(new SQLCondition());
        }

        public static void GetSeasonEpisodeCounts(DBSeason season, out int epsTotal, out int epsUnWatched)
        {
            m_bUpdateEpisodeCount = true;

            SQLCondition cond = new SQLCondition(DBOnlineEpisode.TableFields, DBOnlineEpisode.cSeriesID, season[DBSeason.cSeriesID], SQLConditionType.Equal);
			cond.Add(DBOnlineEpisode.TableFields, DBOnlineEpisode.cSeasonIndex, season[DBSeason.cIndex], SQLConditionType.Equal);           
            string query = stdGetSQL(cond, false, true, "online_episodes.CompositeID, Watched, FirstAired");
            SQLiteResultSet results = DBTVSeries.Execute(query);

            epsTotal = 0;
            int epsWatched = 0;

            // we either get two rows (one for normal episodes, one for double episodes), 
            // or we get no rows so we add them
            for (int i = 0; i < results.Rows.Count; i++)
            {
				int parseResult = 0;
				// increment watched count if episode is watched
                if (int.TryParse(results.Rows[i].fields[1], out parseResult))
                {
                    epsWatched += parseResult;
                }
                
                Regex r = new Regex(@"(\d{4})-(\d{2})-(\d{2})");
                Match match = r.Match(results.Rows[i].fields[2]);

                try
                {
                    if (match.Success)
                    {
                        // if episode airdate is in the future conditionally add to episode count
						DateTime firstAired = new DateTime(Convert.ToInt32(match.Groups[1].Value), Convert.ToInt32(match.Groups[2].Value), Convert.ToInt32(match.Groups[3].Value));
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

			SQLCondition cond = new SQLCondition(DBOnlineEpisode.TableFields, DBOnlineEpisode.cSeriesID, series, SQLConditionType.Equal);            
            string query = stdGetSQL(cond, false, true, "online_episodes.CompositeID, Watched, FirstAired");
            SQLiteResultSet results = DBTVSeries.Execute(query);

            epsTotal = 0;
            int epsWatched = 0;

            // we either get two rows (one for normal episodes, one for double episodes), 
            // or we get no rows so we add them
            for (int i = 0; i < results.Rows.Count; i++)
            {
				int parseResult = 0;
				// increment watched count if episode is watched
                if (int.TryParse(results.Rows[i].fields[1], out parseResult))
                {
                    epsWatched += parseResult;
                }

                Regex r = new Regex(@"(\d{4})-(\d{2})-(\d{2})");
                Match match = r.Match(results.Rows[i].fields[2]);

                try
                {
                    if (match.Success)
                    {
                        // if episode airdate is in the future conditionally add to episode count
						DateTime firstAired = new DateTime(Convert.ToInt32(match.Groups[1].Value), Convert.ToInt32(match.Groups[2].Value), Convert.ToInt32(match.Groups[3].Value));
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
            conditions.Add(DBOnlineEpisode.TableFields, DBOnlineEpisode.cWatched, new DBValue(false), SQLConditionType.Equal);

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
                    conditions.Add(DBEpisode.TableFields, DBEpisode.cFilename, string.Empty, SQLConditionType.NotEqual);

                // include hidden?
                if ((!Settings.isConfig || m_bUpdateEpisodeCount) || !DBOption.GetOptions(DBOption.cShowHiddenItems))
                    conditions.Add(DBOnlineEpisode.TableFields, DBOnlineEpisode.cHidden, 0, SQLConditionType.Equal);

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
            return stdGetSQL(conditions, selectFull, inclStdCond, DBOnlineEpisode.TableFields[DBOnlineEpisode.cEpisodeIndex].Q, false);
        }

        public static string stdGetSQL(SQLCondition conditions, bool selectFull, bool inclStdCond, bool reverseJoin)
        {
			return stdGetSQL(conditions, selectFull, inclStdCond, DBOnlineEpisode.TableFields[DBOnlineEpisode.cEpisodeIndex].Q, reverseJoin);
        }

        public static string stdGetSQL(SQLCondition conditions, bool selectFull, bool inclStdCond, string fieldToSelectIfNotFull)
        {
            return stdGetSQL(conditions, selectFull, inclStdCond, fieldToSelectIfNotFull, false);
        }

        public static string stdGetSQL(SQLCondition conditions, bool selectFull, bool inclStdCond, string fieldToSelectIfNotFull, bool reverseJoin)
        {
        	String sqlWhat;
            if (inclStdCond) {
                conditions.AddCustom(stdConditions.ConditionsSQLString);
            }

            SQLCondition conditionsFirst = conditions.Copy();
            SQLCondition conditionsSecond = conditions.Copy();
            // need to extract the series condition from the original conditions, to retrieve the series this is based upon
            String sWhere = conditions;
            String RegExp = DBOnlineEpisode.TableFields[cSeriesID].Q + @" = (-?\d+)";
            Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
            Match matchSeriesID = Engine.Match(conditions);
            RegExp = DBOnlineEpisode.TableFields[cSeasonIndex].Q + @" = (-?\d+)";
            Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
            Match matchSeasonIndex = Engine.Match(conditions);

            SQLCondition subQueryConditions = new SQLCondition();
            if (matchSeriesID.Success)
				subQueryConditions.Add(DBEpisode.TableFields, cSeriesID, matchSeriesID.Groups[1].Value, SQLConditionType.Equal);
            if (matchSeasonIndex.Success)
				subQueryConditions.Add(DBEpisode.TableFields, cSeasonIndex, matchSeasonIndex.Groups[1].Value, SQLConditionType.Equal);
			subQueryConditions.Add(DBEpisode.TableFields, cCompositeID2, "", SQLConditionType.NotEqual);

			String sqlSubQuery = "select distinct " + DBEpisode.TableFields[cCompositeID2].Q + " from " + DBEpisode.cTableName + subQueryConditions;
            conditionsFirst.AddCustom(sqlSubQuery, DBOnlineEpisode.TableFields[cCompositeID].Q, SQLConditionType.NotIn);
			conditionsSecond.Add(DBEpisode.TableFields, cCompositeID2, "", SQLConditionType.NotEqual);

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
					orderBy = " order by " + DBOnlineEpisode.TableFields[DBOnlineEpisode.cEpisodeIndex].Q;

                SQLWhat what = new SQLWhat(first);
                what.AddWhat(second);
                // one query gets both first & second episode
                sqlWhat = "select " + what;
            }
            else
            {
                sqlWhat = "select " + fieldToSelectIfNotFull + " from " + first.TableName;
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

			return sqlWhat + " left join " + second.TableName + " on (" + DBEpisode.TableFields[cCompositeID].Q + "=" + DBOnlineEpisode.TableFields[DBOnlineEpisode.cCompositeID].Q
                           + ") " + conditionsFirst
                           + " union "
						   + sqlWhat + " left join " + second.TableName + " on (" + DBEpisode.TableFields[cCompositeID2].Q + "=" + DBOnlineEpisode.TableFields[DBOnlineEpisode.cCompositeID].Q
						   + ") " + conditionsSecond + orderBy + conditions.limitString;
        }

        public static List<DBEpisode> Get(int nSeriesID)
        {
            return Get(nSeriesID, true);
        }

        public static List<DBEpisode> Get(int nSeriesID, bool inclStdCond)
        {
            SQLCondition conditions = new SQLCondition();
            conditions.Add(DBOnlineEpisode.TableFields, cSeriesID, nSeriesID, SQLConditionType.Equal);

            return Get(conditions, inclStdCond);
        }

        public static List<DBEpisode> Get(int nSeriesID, int nSeasonIndex)
        {
            return Get(nSeriesID, nSeasonIndex, true);
        }

        public static List<DBEpisode> Get(int nSeriesID, int nSeasonIndex, bool includeStdCond)
        {
            SQLCondition conditions = new SQLCondition();
            conditions.Add(DBOnlineEpisode.TableFields, cSeriesID, nSeriesID, SQLConditionType.Equal);
            conditions.Add(DBOnlineEpisode.TableFields, cSeasonIndex, nSeasonIndex, SQLConditionType.Equal);

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

		public bool ReadFilename(string filename)
		{
			try {
				m_fields[DBEpisode.cFilename].Value = filename;
				SQLCondition condition = new SQLCondition();
				condition.Add(DBEpisode.TableFields, cFilename, m_fields[DBEpisode.cFilename].Value, SQLConditionType.Equal);
				String sqlQuery = "select * from " + TableName + condition;
				SQLiteResultSet records = DBTVSeries.Execute(sqlQuery);
				return Read(ref records, 0);
			} catch (Exception ex) {
				MPTVSeriesLog.Write("An Error Occurred (" + ex.Message + ").");
			}
			return false;
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
            Clear(DBEpisode.cTableName, conditions);
        }

        public static void GlobalSet(String sKey, DBValue Value)
        {
            GlobalSet(sKey, Value, new SQLCondition());
        }

        public static void GlobalSet(String sKey, DBValue Value, SQLCondition condition)
        {
            GlobalSet(DBOnlineEpisode.TableFields, sKey, Value, condition);
            GlobalSet(DBEpisode.TableFields, sKey, Value, condition);
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