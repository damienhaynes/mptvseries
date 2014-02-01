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
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Threading;
using MediaPortal.Configuration;
using MediaPortal.GUI.Library;
using MediaPortal.GUI.Video;
using MediaPortal.Player;
using MediaPortal.Dialogs;
using MediaPortal.Util;
using MediaPortal.Playlists;
using MediaPortal.Video.Database;
using WindowPlugins.GUITVSeries;
using WindowPlugins.GUITVSeries.FollwitTv;

namespace WindowPlugins.GUITVSeries
{
    public class VideoHandler
    {
        #region Vars
        static MediaPortal.Playlists.PlayListPlayer playlistPlayer;
        public static DBEpisode m_currentEpisode;
        DBEpisode m_previousEpisode;
        BackgroundWorker PlayPropertyUpdater = new BackgroundWorker();
        public delegate void rateRequest(DBEpisode episode);        
        public event rateRequest RateRequestOccured;        
        private bool m_bIsExternalPlayer = false;
        private bool m_bIsExternalDVDPlayer = false;
        private bool m_bIsImageFile = false;
		private bool listenToExternalPlayerEvents = false;        
        #endregion

        #region Static Events
        /// <summary>
        /// Event gets triggered when an Episode has finished being watched and considered watched
        /// </summary>
        /// <param name="episode">Episode object just watched</param>
        public delegate void EpisodeWatchedDelegate(DBEpisode episode);
        
        /// <summary>
        /// Event gets triggered when an episode has been started
        /// </summary>
        /// <param name="episode">Episode object being watched</param>
        public delegate void EpisodeStartedDelegate(DBEpisode episode);

        /// <summary>
        /// Event gets triggered when an episode has stopped but not considered watched
        /// </summary>
        /// <param name="episode">Episode object just watched</param>
        public delegate void EpisodeStoppedDelegate(DBEpisode episode);

        public static event EpisodeWatchedDelegate EpisodeWatched;
        public static event EpisodeStartedDelegate EpisodeStarted;
        public static event EpisodeStoppedDelegate EpisodeStopped;
        #endregion

        #region Constructor
        public VideoHandler()
        {
            playlistPlayer = MediaPortal.Playlists.PlayListPlayer.SingletonPlayer;

            // Check if External Player is being used
            MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml"));
            m_bIsExternalPlayer = !xmlreader.GetValueAsBool("movieplayer", "internal", true);
            m_bIsExternalDVDPlayer = !xmlreader.GetValueAsBool("dvdplayer", "internal", true);
            
			// external player handlers
			Utils.OnStartExternal += new Utils.UtilEventHandler(onStartExternal);
			Utils.OnStopExternal += new Utils.UtilEventHandler(onStopExternal);

            g_Player.PlayBackStopped += new MediaPortal.Player.g_Player.StoppedHandler(OnPlayBackStopped);
            g_Player.PlayBackEnded += new MediaPortal.Player.g_Player.EndedHandler(OnPlayBackEnded);
            g_Player.PlayBackStarted += new MediaPortal.Player.g_Player.StartedHandler(OnPlayBackStarted);
            g_Player.PlayBackChanged += new g_Player.ChangedHandler(OnPlaybackChanged);
            PlayPropertyUpdater.WorkerSupportsCancellation = true;
            PlayPropertyUpdater.DoWork += new DoWorkEventHandler(SetPlayProperties_DoWork);
        }

        #endregion

        #region Public Methods
        public bool ResumeOrPlay(DBEpisode episode)
        {
            try
            {
                MPTVSeriesLog.Write("Attempting to play: ", episode[DBEpisode.cFilename].ToString(), MPTVSeriesLog.LogLevel.Debug);
                // don't have this file !
                if (episode[DBEpisode.cFilename].ToString().Length == 0)
                    return false;

                // check that we are not playing an episode out of episode if unwatched
                // ignore specials as they can be pretty out of wack!
                #region PlayBack Order

                // check sort order so our check is accurate
                var series = Helper.getCorrespondingSeries(episode[DBOnlineEpisode.cSeriesID]);
                bool dvdSortOrder = series[DBOnlineSeries.cEpisodeSortOrder] == "DVD";

                string seasonField = dvdSortOrder ? DBOnlineEpisode.cCombinedSeason : DBOnlineEpisode.cSeasonIndex;
                string episodeField = dvdSortOrder ? DBOnlineEpisode.cCombinedEpisodeNumber : DBOnlineEpisode.cEpisodeIndex;

                if (DBOption.GetOptions(DBOption.cCheckPlayOutOfOrder) && !episode[DBOnlineEpisode.cWatched] && episode[seasonField] > 0 && episode[episodeField] > 1)
                {
                    // first get the next unwatched episode from previously played
                    // we are only interested in current season (could be multi-user watching multiple seasons)
                    // API for GetNextUnwatched is not desirable as that works from Date Watched, we only care about watched here
                    var conditions = new SQLCondition();
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, episode[DBOnlineSeries.cSeriesID], SQLConditionType.Equal);
                    conditions.Add(new DBOnlineEpisode(), seasonField, episode[seasonField], SQLConditionType.Equal);
                    conditions.Add(new DBOnlineEpisode(), episodeField, episode[episodeField], SQLConditionType.LessThan);
                    conditions.Add(new DBOnlineEpisode(), episodeField, 0, SQLConditionType.GreaterThan);
                    var episodes = DBEpisode.Get(conditions, false);

                    if (episodes != null && episodes.Count > 0)
                    {
                        // set logical playback order based on sort order
                        episodes.Sort();

                        // if the previous found episode is not watched then we are playing out of order
                        // if we have a gap in episode collection then assume it has not been watched (this check is needed when user does not download all episode info)
                        if (!episodes.Last()[DBOnlineEpisode.cWatched] || (episode[episodeField] - episodes.Last()[episodeField]) > 1)
                        {
                            GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                            dlgYesNo.SetHeading(Translation.Warning);
                            dlgYesNo.SetLine(1, Translation.PlaybackOutOfOrderLine1);
                            dlgYesNo.SetLine(2, string.Format("{0} - {1}x{2}", series.ToString(), episode[seasonField], episode[episodeField] - 1));
                            dlgYesNo.SetLine(3, Translation.PlaybackOutOfOrderLine2);
                            dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
                            if (!dlgYesNo.IsConfirmed) return false;
                        }
                    }
                }
                #endregion

                m_previousEpisode = m_currentEpisode;
                m_currentEpisode = episode;
                int timeMovieStopped = m_currentEpisode[DBEpisode.cStopTime];

                // asynchronously send now watching info to follwit
                FollwitConnector.NowWatching(episode, true);

                // Check if file is an Image e.g. ISO
                string filename = m_currentEpisode[DBEpisode.cFilename];
                m_bIsImageFile = Helper.IsImageFile(filename);

                #region Invoke Before Playback
                // see if we have an invokeOption set up
                string invoke = (string)DBOption.GetOptions(DBOption.cInvokeExtBeforePlayback);                
                if (!string.IsNullOrEmpty(invoke))
                {
                    string invokeArgs = (string)DBOption.GetOptions(DBOption.cInvokeExtBeforePlaybackArgs);
                    try
                    {
                        // replace any placeholders in the arguments for the script if any have been supplied.
                        if (!string.IsNullOrEmpty(invokeArgs))
                        {
                            invokeArgs = FieldGetter.resolveDynString(invokeArgs, m_currentEpisode, true);
                        }
                        invoke = FieldGetter.resolveDynString(invoke, m_currentEpisode, true);
                        
                        // use ProcessStartInfo instead of Process.Start(string) as latter produces a "cannot find file"
                        // error if you pass in command line arguments.
                        // also allows us to run the script hidden, preventing, for example, a command prompt popping up.
                        ProcessStartInfo psi = new ProcessStartInfo(invoke, invokeArgs);
                        psi.WindowStyle = ProcessWindowStyle.Hidden;
                        Process proc = System.Diagnostics.Process.Start(psi);
                        MPTVSeriesLog.Write(string.Format("Sucessfully Invoked BeforeFilePlay Command: '{0}' '{1}'",  invoke, invokeArgs));

                        // if not present in database this evaluates to false. If present and not a valid bool then
                        // it evaluates to true
                        bool waitForExit = (bool)DBOption.GetOptions(DBOption.cInvokeExtBeforePlaybackWaitForExit);
                        
                        // if true this thread will wait for the external user script to complete before continuing.
                        if (waitForExit)
                        {
                            proc.WaitForExit();
                        }
                    }
                    catch (Exception e)
                    {
                        MPTVSeriesLog.Write(string.Format("Unable to Invoke BeforeFilePlay Command: '{0}' '{1}'",  invoke, invokeArgs));
                        MPTVSeriesLog.Write(e.Message);
                    }
                }
                #endregion

                #region Removable Media Handling
                if (!File.Exists(m_currentEpisode[DBEpisode.cFilename]))
                {
                    string episodeVolumeLabel = m_currentEpisode[DBEpisode.cVolumeLabel].ToString();

                    if (string.IsNullOrEmpty(episodeVolumeLabel))
                        episodeVolumeLabel = LocalParse.getImportPath(m_currentEpisode[DBEpisode.cFilename]);

                    // ask the user to input cd/dvd, usb disk or confirm network drive is connected
                    GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                    if (null == dlgOK)
                        return false;
                    dlgOK.SetHeading(Translation.insertDisk);
                    dlgOK.SetLine(1, Translation.InsertDiskMessage1);
                    dlgOK.SetLine(2, Translation.InsertDiskMessage2);
                    dlgOK.SetLine(3, Translation.InsertDiskMessage3);
                    dlgOK.SetLine(4, string.Format(Translation.InsertDiskMessage4, episodeVolumeLabel));                    
                    dlgOK.DoModal(GUIWindowManager.ActiveWindow);

                    if (!File.Exists(m_currentEpisode[DBEpisode.cFilename]))
                    {
                        return false; // still not found, return to list
                    }

                }
                #endregion

                #region Ask user to Resume

                // skip this if we are using an External Player
                bool bExternalPlayer = m_bIsImageFile ? m_bIsExternalDVDPlayer : m_bIsExternalPlayer;

                if (timeMovieStopped > 0 && !bExternalPlayer)
                {
                    MPTVSeriesLog.Write("Asking user to resume episode from: " + Utils.SecondsToHMSString(timeMovieStopped));
                    GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);

                    if (null != dlgYesNo)
                    {
                        dlgYesNo.SetHeading(Translation.ResumeEpisode);
                        dlgYesNo.SetLine(1, m_currentEpisode.onlineEpisode.CompleteTitle);
                        dlgYesNo.SetLine(2, GUILocalizeStrings.Get(936) + " " + Utils.SecondsToHMSString(timeMovieStopped));
                        dlgYesNo.SetDefaultToYes(true);
                        dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
                        // reset resume data in DB
                        if (!dlgYesNo.IsConfirmed)
                        {
                            timeMovieStopped = 0;
                            m_currentEpisode[DBEpisode.cStopTime] = timeMovieStopped;
                            m_currentEpisode.Commit();
                            MPTVSeriesLog.Write("User selected to start episode from beginning", MPTVSeriesLog.LogLevel.Debug);
                        }
                        else
                        {
                            MPTVSeriesLog.Write("User selected to resume episode", MPTVSeriesLog.LogLevel.Debug);

                            // dont scrobble first of double episode if resuming past halfway
                            double duration = m_currentEpisode[DBEpisode.cLocalPlaytime];
                        }
                    }
                }

                #endregion

                Play(timeMovieStopped);
                return true;
            }
            catch (Exception e)
            {
                MPTVSeriesLog.Write("TVSeriesPlugin.VideoHandler.ResumeOrPlay()\r\n" + e.ToString());
                return false;
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the movie metadata on the playback screen (for when the user clicks info). 
        /// The delay is neccesary because Player tries to use metadata from the MyVideos database.
        /// We want to update this after that happens so the correct info is there.
        /// Clears properties if (EventArgs.Argument == true)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Clear State</param>
        void SetPlayProperties_DoWork(object sender, DoWorkEventArgs e)
        {
            bool clear = (bool)e.Argument;
            if (!clear)
                Thread.Sleep(2000);

            if (PlayPropertyUpdater.CancellationPending) return;
            
            SetGUIProperties((bool)e.Argument);
        }

        /// <summary>
        /// Sets the following Properties:
        /// "#Play.Current.Title"
        /// "#Play.Current.Plot"
        /// "#Play.Current.Thumb"
        /// "#Play.Current.Year"
        /// </summary>
        /// <param name="clear">Clears the properties instead of filling them if True</param>
        void SetGUIProperties(bool clear)
        {
            if (m_currentEpisode == null) return;

            DBSeries series = null;
            if (!clear) series = Helper.getCorrespondingSeries(m_currentEpisode[DBEpisode.cSeriesID]);
            DBSeason season = null;
            if (!clear) season = Helper.getCorrespondingSeason(m_currentEpisode[DBEpisode.cSeriesID], m_currentEpisode[DBEpisode.cSeasonIndex]);

			// Show Plot in OSD or Hide Spoilers (note: FieldGetter takes care of that)         
            GUIPropertyManager.SetProperty("#Play.Current.Plot", clear ? " " : FieldGetter.resolveDynString(TVSeriesPlugin.m_sFormatEpisodeMain, m_currentEpisode));

			// Show Episode Thumbnail or Series Poster if Hide Spoilers is enabled
            string osdImage = string.Empty;            
            if (!clear)
            {
                foreach (KeyValuePair<string, string> kvp in SkinSettings.VideoOSDImages)
                {
                    switch (kvp.Key) 
                    {
                        case "episode":
                            osdImage = ImageAllocator.GetEpisodeImage(m_currentEpisode);
                            break;
                        case "season":
                            osdImage = season.Banner;
                            break;
                        case "series":
                            osdImage = series.Poster;
                            break;
                        case "custom":
                            string value = replaceDynamicFields(kvp.Value);
                            string file = Helper.getCleanAbsolutePath(value);
                            if (System.IO.File.Exists(file))
                                osdImage = file;
                            break;
                    }

                    osdImage = osdImage.Trim();
                    if (string.IsNullOrEmpty(osdImage)) continue;
                    else break;
                }
            }
            GUIPropertyManager.SetProperty("#Play.Current.Thumb", clear ? " " : osdImage);

            // double check, i don't want play images to be cleared on ended or stopped...
            if (PlayPropertyUpdater.CancellationPending) return;

            foreach (KeyValuePair<string, string> kvp in SkinSettings.VideoPlayImages)
            {
                if (!clear)
                {
                    string value = replaceDynamicFields(kvp.Value);
                    string file = Helper.getCleanAbsolutePath(value);
                    if (System.IO.File.Exists(file))
                    {
                        MPTVSeriesLog.Write(string.Format("Setting play image {0} for property {1}", file, kvp.Key), MPTVSeriesLog.LogLevel.Debug);
                        GUIPropertyManager.SetProperty(kvp.Key, clear ? " " : file);
                    }
                }
                else
                {
                    MPTVSeriesLog.Write(string.Format("Clearing play image for property {0}", kvp.Key), MPTVSeriesLog.LogLevel.Debug);
                    GUIPropertyManager.SetProperty(kvp.Key, " ");
                }
            }
			
            GUIPropertyManager.SetProperty("#Play.Current.Title", clear ? " " : m_currentEpisode.onlineEpisode.CompleteTitle);
            GUIPropertyManager.SetProperty("#Play.Current.Year", clear ? " " : FieldGetter.resolveDynString("<" + DBEpisode.cOutName + "." + DBOnlineEpisode.cFirstAired + ">", m_currentEpisode, false));
            GUIPropertyManager.SetProperty("#Play.Current.Genre", clear ? " " : FieldGetter.resolveDynString(TVSeriesPlugin.m_sFormatEpisodeSubtitle, m_currentEpisode));
        }

        string replaceDynamicFields(string value)
        {
            string result = value;

            Regex matchRegEx = new Regex(@"\<[a-zA-Z\.]+\>");
            foreach (Match m in matchRegEx.Matches(value))
            {
                string resolvedValue = FieldGetter.resolveDynString(m.Value, m_currentEpisode, false);
                result = result.Replace(m.Value, resolvedValue);
            }

            return result;
        }

        void MarkEpisodeAsWatched(DBEpisode episode)
        {
            // Could be a double episode, so mark both as watched
            SQLCondition condition = new SQLCondition();
            condition.Add(new DBEpisode(), DBEpisode.cFilename, episode[DBEpisode.cFilename], SQLConditionType.Equal);
            List<DBEpisode> episodes = DBEpisode.Get(condition, false);
            foreach (DBEpisode ep in episodes)
            {
                string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                ep[DBOnlineEpisode.cWatched] = 1;
                ep[DBOnlineEpisode.cPlayCount] = ep[DBOnlineEpisode.cPlayCount] + 1;
                ep[DBEpisode.cDateWatched] = date;
                ep[DBOnlineEpisode.cLastWatchedDate] = date;
                if (string.IsNullOrEmpty(ep[DBOnlineEpisode.cFirstWatchedDate]))
                {
                    ep[DBOnlineEpisode.cFirstWatchedDate] = date;
                }
                ep.Commit(); 
            }
            // Update Episode Counts
            DBSeries series = Helper.getCorrespondingSeries(m_currentEpisode[DBEpisode.cSeriesID]);
            DBSeason season = Helper.getCorrespondingSeason(episode[DBEpisode.cSeriesID], episode[DBEpisode.cSeasonIndex]);
            DBSeason.UpdateEpisodeCounts(series, season);
        }

        /// <summary>
        /// Initiates Playback of m_currentEpisode[DBEpisode.cFilename] and calls Fullscreen Window
        /// </summary>
        /// <param name="timeMovieStopped">Resumepoint of Movie, 0 or negative for Start from Beginning</param>
        /// 
        bool Play(int timeMovieStopped)
        {
            bool result = false;
            try
            {
                // sometimes it takes up to 30+ secs to go to fullscreen even though the video is already playing
                // lets force fullscreen here
                // note: MP might still be unresponsive during this time, but at least we are in fullscreen and can see video should this happen
                // I haven't actually found out why it happens, but I strongly believe it has something to do with the video database and the player doing something in the background
                // (why does it do anything with the video database.....i just want it to play a file and do NOTHING else!)                
                GUIGraphicsContext.IsFullScreenVideo = true;
                GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_FULLSCREEN_VIDEO);

                // If the file is an image file, it should be mounted before playing
                string filename = m_currentEpisode[DBEpisode.cFilename];
                if (m_bIsImageFile) { 
                    if (!GUIVideoFiles.MountImageFile(GUIWindowManager.ActiveWindow, filename)) {                        
                        return false;
                    }
                }
                
                // Start Listening to any External Player Events
                listenToExternalPlayerEvents = true;

                #region Publish Play properties for InfoService plugin
                string seriesName = Helper.getCorrespondingSeries(m_currentEpisode[DBEpisode.cSeriesID]).ToString();
                string seasonID = m_currentEpisode[DBEpisode.cSeasonIndex];
                string episodeID = m_currentEpisode[DBEpisode.cEpisodeIndex];
                string episodeName = m_currentEpisode[DBEpisode.cEpisodeName];
                GUIPropertyManager.SetProperty("#TVSeries.Extended.Title", string.Format("{0}/{1}/{2}/{3}", seriesName, seasonID, episodeID, episodeName));
                MPTVSeriesLog.Write(string.Format("#TVSeries.Extended.Title: {0}/{1}/{2}/{3}", seriesName, seasonID, episodeID, episodeName));
                #endregion

                // tell any listeners that we are starting playback
                if (EpisodeStarted != null)
                    EpisodeStarted(m_currentEpisode);

                // Play File
                result = g_Player.Play(filename, g_Player.MediaType.Video);
                
                // Stope Listening to any External Player Events
				listenToExternalPlayerEvents = false;

                // tell player where to resume                
				if (g_Player.Playing && timeMovieStopped > 0) {
					MPTVSeriesLog.Write("Setting seek position at: " + timeMovieStopped, MPTVSeriesLog.LogLevel.Debug);
					GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_SEEK_POSITION, 0, 0, 0, 0, 0, null);
					msg.Param1 = (int)timeMovieStopped;
					GUIGraphicsContext.SendMessage(msg);
				}			

            }
            catch (Exception e)
            {
                MPTVSeriesLog.Write("TVSeriesPlugin.VideoHandler.Play()\r\n" + e.ToString());
                result = false;
            }
            return result;
        }
        #endregion

        #region Playback Event Handlers
        void OnPlayBackStopped(g_Player.MediaType type, int timeMovieStopped, string filename)
        {
            if (PlayBackOpIsOfConcern(type, filename))
            {
                if (PlayPropertyUpdater.IsBusy) PlayPropertyUpdater.CancelAsync();
                LogPlayBackOp("stopped", filename);
                try
                {
                    #region Set Resume Point or Watched
                    double watchedAfter = DBOption.GetOptions(DBOption.cWatchedAfter);
                    if ((timeMovieStopped / playlistPlayer.g_Player.Duration) > watchedAfter / 100)
                    {
                        m_currentEpisode[DBEpisode.cStopTime] = 0;
                        m_currentEpisode.Commit();
                        PlaybackOperationEnded(true);
                    }
                    else
                    {
                        m_currentEpisode[DBEpisode.cStopTime] = timeMovieStopped;
                        m_currentEpisode.Commit();
                        PlaybackOperationEnded(false);                        
                    }
                    #endregion

                    m_currentEpisode = null;
                    m_previousEpisode = null;
                }
                catch (Exception e)
                {
                    MPTVSeriesLog.Write("TVSeriesPlugin.VideoHandler.OnPlayBackStopped()\r\n" + e.ToString());
                }
            }
        }

        void OnPlayBackEnded(g_Player.MediaType type, string filename)
        {
            if (PlayBackOpIsOfConcern(type, filename))
            {
                if (PlayPropertyUpdater.IsBusy) PlayPropertyUpdater.CancelAsync();
                LogPlayBackOp("ended", filename);
                try
                {
                    m_currentEpisode[DBEpisode.cStopTime] = 0;
                    m_currentEpisode.Commit();
                    PlaybackOperationEnded(true);

                    m_currentEpisode = null;
                    m_previousEpisode = null;
                }
                catch (Exception e)
                {
                    MPTVSeriesLog.Write("TVSeriesPlugin.VideoHandler.OnPlayBackEnded()\r\n" + e.ToString());
                }
            }
        }

        void OnPlaybackChanged(g_Player.MediaType type, int timeMovieStopped, string filename)
        {
            if (PlayBackOpWasOfConcern(g_Player.IsVideo? g_Player.MediaType.Video : g_Player.MediaType.Unknown, g_Player.CurrentFile))
            {
                if (PlayPropertyUpdater.IsBusy) PlayPropertyUpdater.CancelAsync();
                LogPlayBackOp("changed", g_Player.CurrentFile);
                try 
                {
                    #region Set Resume Point or Watched
                    double watchedAfter = DBOption.GetOptions(DBOption.cWatchedAfter);
                    if (!m_previousEpisode[DBOnlineEpisode.cWatched]
                        && (timeMovieStopped / playlistPlayer.g_Player.Duration) > watchedAfter / 100) 
                    {
                        m_previousEpisode[DBEpisode.cStopTime] = 0;
                        m_previousEpisode.Commit();
                        MPTVSeriesLog.Write("This episode counts as watched");
                        MarkEpisodeAsWatched(m_previousEpisode);
                        SetGUIProperties(true);
                    }
                    else
                    {
                        m_previousEpisode[DBEpisode.cStopTime] = timeMovieStopped;
                        m_previousEpisode.Commit();
                        SetGUIProperties(true);
                    }
                    #endregion

                    m_previousEpisode = null;
                }
                catch (Exception e)
                {
                    MPTVSeriesLog.Write("TVSeriesPlugin.VideoHandler.OnPlaybackChanged()\r\n" + e.ToString());
                }
            }
        }

        void OnPlayBackStarted(g_Player.MediaType type, string filename)
        {
            if (PlayBackOpIsOfConcern(type, filename))
            {
                LogPlayBackOp("started", filename);
                // really stupid, you have to wait until the player itself sets the properties (a few seconds) and after that set them
                PlayPropertyUpdater.RunWorkerAsync(false);
            }
        }
        #endregion

		#region External Player Event Handlers
		private void onStartExternal(Process proc, bool waitForExit) {
			// If we were listening for external player events
			if (listenToExternalPlayerEvents) {
				MPTVSeriesLog.Write("Playback Started in External Player:" + m_currentEpisode.ToString());				
			}
		}

		private void onStopExternal(Process proc, bool waitForExit) {
			if (!listenToExternalPlayerEvents)
				return;

			MPTVSeriesLog.Write("Playback Stopped in External Player:" + m_currentEpisode.ToString());
		
			// Exit fullscreen Video so we can see main facade again			
			if (GUIGraphicsContext.IsFullScreenVideo) {
				GUIGraphicsContext.IsFullScreenVideo = false;
			}
			// Mark Episode as watched regardless and prompt for rating
			bool markAsWatched = (DBOption.GetOptions(DBOption.cWatchedAfter) > 0 && m_currentEpisode[DBOnlineEpisode.cWatched] == 0);
			PlaybackOperationEnded(markAsWatched);	
		}
		#endregion

        #region Helpers
        bool PlayBackOpIsOfConcern(g_Player.MediaType type, string filename)
        {
            if (string.IsNullOrEmpty(filename)) return false;

            return (m_currentEpisode != null && 
                    type == g_Player.MediaType.Video && 
                    m_currentEpisode[DBEpisode.cFilename] == filename);
        }

        bool PlayBackOpWasOfConcern(g_Player.MediaType type, string filename) {
            if (string.IsNullOrEmpty(filename)) return false;

            return (m_previousEpisode != null &&
                    type == g_Player.MediaType.Video &&
                    m_previousEpisode[DBEpisode.cFilename] == filename);
        }

        void PlaybackOperationEnded(bool countAsWatched)
        {
            // if needed, asynchronously notify follw.it we arnt watching the show anymore
            FollwitConnector.NowWatching(m_currentEpisode, false);
            if (countAsWatched) FollwitConnector.Watch(m_currentEpisode, true, true);

            // notify listeners
            if (!countAsWatched && EpisodeStopped != null) EpisodeStopped(m_currentEpisode);

            if (countAsWatched || m_currentEpisode[DBOnlineEpisode.cWatched])
            {
                MPTVSeriesLog.Write("This episode counts as watched");
                if (countAsWatched)
                {
                    MarkEpisodeAsWatched(m_currentEpisode);
                    // notify listeners
                    if (EpisodeWatched != null) EpisodeWatched(m_currentEpisode);
                }
                // if the ep wasn't rated before, and the option to ask is set, bring up the ratings menu
                if ((String.IsNullOrEmpty(m_currentEpisode[DBOnlineEpisode.cMyRating]) || m_currentEpisode[DBOnlineEpisode.cMyRating] == 0) && DBOption.GetOptions(DBOption.cAskToRate))
                {
                    MPTVSeriesLog.Write("Episode not rated yet");
                    if(RateRequestOccured != null)
                        RateRequestOccured.Invoke(m_currentEpisode);
                } 
                else 
                    MPTVSeriesLog.Write("Episode has already been rated or option not set");
            }
            SetGUIProperties(true); // clear GUI Properties

            #region Invoke After Playback
            string invoke = (string)DBOption.GetOptions(DBOption.cInvokeExtAfterPlayback);
            if (countAsWatched && !string.IsNullOrEmpty(invoke))
            {
                string invokeArgs = (string)DBOption.GetOptions(DBOption.cInvokeExtAfterPlaybackArgs);
                try
                {                    
                    // replace any placeholders in the arguments for the script if any have been supplied.
                    if (!string.IsNullOrEmpty(invokeArgs))
                    {
                        invokeArgs = FieldGetter.resolveDynString(invokeArgs, m_currentEpisode, true);
                    }
                    invoke = FieldGetter.resolveDynString(invoke, m_currentEpisode, true);

                    // use ProcessStartInfo instead of Process.Start(string) as latter produces a "cannot find file"
                    // error if you pass in command line arguments.
                    // also allows us to run the script hidden, preventing, for example, a command prompt popping up.
                    ProcessStartInfo psi = new ProcessStartInfo(invoke, invokeArgs);
                    psi.WindowStyle = ProcessWindowStyle.Hidden;
                    Process proc = System.Diagnostics.Process.Start(psi);
                    MPTVSeriesLog.Write(string.Format("Sucessfully Invoked AfterFilePlay Command: '{0}' '{1}'", invoke, invokeArgs));

                    // if not present in database this evaluates to false. If present and not a valid bool then
                    // it evaluates to true
                    bool waitForExit = (bool)DBOption.GetOptions(DBOption.cInvokeExtAfterPlaybackWaitForExit);

                    // if true this thread will wait for the external user script to complete before continuing.
                    if (waitForExit)
                    {
                        proc.WaitForExit();
                    }
                }
                catch (Exception e)
                {
                    MPTVSeriesLog.Write(string.Format("Unable to Invoke ExtAfterPlayback Command: '{0}' '{1}'", invoke, invokeArgs));
                    MPTVSeriesLog.Write(e.Message);
                }
            }
            #endregion
        }

        void LogPlayBackOp(string OperationType, string filename)
        {
            MPTVSeriesLog.Write(string.Format("Playback {0} for: {1}", OperationType, filename), MPTVSeriesLog.LogLevel.Normal);
        }        

        #endregion
    }
}
