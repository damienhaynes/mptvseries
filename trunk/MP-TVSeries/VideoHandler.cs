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
using System.Diagnostics;
using System.Text;
using MediaPortal.Configuration;
using MediaPortal.GUI.Library;
using MediaPortal.GUI.Video;
using MediaPortal.Player;
using MediaPortal.Dialogs;
using MediaPortal.Util;
using MediaPortal.Playlists;
using MediaPortal.Video.Database;
using WindowPlugins.GUITVSeries;


namespace WindowPlugins.GUITVSeries
{
    class VideoHandler
    {
        #region Vars
        static MediaPortal.Playlists.PlayListPlayer playlistPlayer;
        DBEpisode m_currentEpisode;      
        System.ComponentModel.BackgroundWorker w = new System.ComponentModel.BackgroundWorker();
        public delegate void rateRequest(DBEpisode episode);
        public event rateRequest RateRequestOccured;
        private bool m_bIsExternalPlayer = false;
        private bool m_bIsExternalDVDPlayer = false;
        private bool m_bIsImageFile = false;
		private bool listenToExternalPlayerEvents = false;
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
			MediaPortal.Util.Utils.OnStartExternal += new MediaPortal.Util.Utils.UtilEventHandler(onStartExternal);
			MediaPortal.Util.Utils.OnStopExternal += new MediaPortal.Util.Utils.UtilEventHandler(onStopExternal);

            g_Player.PlayBackStopped += new MediaPortal.Player.g_Player.StoppedHandler(OnPlayBackStopped);
            g_Player.PlayBackEnded += new MediaPortal.Player.g_Player.EndedHandler(OnPlayBackEnded);
            g_Player.PlayBackStarted += new MediaPortal.Player.g_Player.StartedHandler(OnPlayBackStarted);
			//g_Player.PlayBackChanged += new MediaPortal.Player.g_Player.StartedHandler(OnPlayBackStarted);
            w.DoWork += new System.ComponentModel.DoWorkEventHandler(w_DoWork);
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

                m_currentEpisode = episode;
                int timeMovieStopped = m_currentEpisode[DBEpisode.cStopTime];

                // Check if file is an Image e.g. ISO
                string filename = m_currentEpisode[DBEpisode.cFilename];
                m_bIsImageFile = Helper.IsImageFile(filename);                               
                
                #region Removable Media Handling
                bool isOnRemovable = false;
                if (episode[DBEpisode.cIsOnRemovable]) isOnRemovable = true;
                else if (episode[DBEpisode.cIsOnRemovable] == string.Empty) // was imported before support for this
                {
                    isOnRemovable = LocalParse.isOnRemovable(episode[DBEpisode.cFilename]);
                    episode[DBEpisode.cIsOnRemovable] = isOnRemovable;
                    if (isOnRemovable) episode[DBEpisode.cVolumeLabel] = LocalParse.getDiskID(episode[DBEpisode.cFilename]);
                    episode.Commit();
                }

                if (isOnRemovable && !System.IO.File.Exists(episode[DBEpisode.cFilename]))
                {
                    // ask the user to input cd/dvd whatever
                    GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                    if (null == dlgOK)
                        return false;
                    dlgOK.SetHeading(Translation.insertDisk);
                    dlgOK.SetLine(1, string.Empty);
                    dlgOK.SetLine(2, m_currentEpisode[DBEpisode.cVolumeLabel].ToString());
                    dlgOK.DoModal(GUIWindowManager.ActiveWindow);

                    if (!System.IO.File.Exists(episode[DBEpisode.cFilename]))
                    {
                        return false; // still not found, return to list
                    }

                }
                #endregion

                #region Ask user to Resume

                // skip this if we are using an External Player                
                bool bExternalPlayer = m_bIsImageFile ? m_bIsExternalDVDPlayer : m_bIsExternalPlayer;
                
                if (timeMovieStopped > 0 && !bExternalPlayer) {                                       
                    MPTVSeriesLog.Write("Asking user to resume episode from: " + Utils.SecondsToHMSString(timeMovieStopped));
                    GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);

                    if (null != dlgYesNo) {
                        dlgYesNo.SetHeading(GUILocalizeStrings.Get(900)); //resume movie?
                        dlgYesNo.SetLine(1, m_currentEpisode.onlineEpisode.CompleteTitle);
                        dlgYesNo.SetLine(2, GUILocalizeStrings.Get(936) + " " + Utils.SecondsToHMSString(timeMovieStopped));
                        dlgYesNo.SetDefaultToYes(true);
                        dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
                        // reset resume data in DB
                        if (!dlgYesNo.IsConfirmed) {
                            timeMovieStopped = 0;
                            m_currentEpisode[DBEpisode.cStopTime] = timeMovieStopped;
                            m_currentEpisode.Commit();
                            MPTVSeriesLog.Write("User selected to start episode from beginning", MPTVSeriesLog.LogLevel.Debug);
                        }
                        else {
                            MPTVSeriesLog.Write("User selected to resume episode", MPTVSeriesLog.LogLevel.Debug);
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
        /// <param name="e"></param>
        void w_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            bool clear = (bool)e.Argument;
            if (!clear)
                System.Threading.Thread.Sleep(2000);
            
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
            DBSeries series = null;
            if(!clear) series = Helper.getCorrespondingSeries(m_currentEpisode[DBEpisode.cSeriesID]);

            if (m_currentEpisode == null) return;

			// Show Plot in OSD or Hide Spoilers
            if (!DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedSummary) || m_currentEpisode[DBOnlineEpisode.cWatched])
                MediaPortal.GUI.Library.GUIPropertyManager.SetProperty("#Play.Current.Plot", clear ? "" : (string)m_currentEpisode[DBOnlineEpisode.cEpisodeSummary]);                
            else
                MediaPortal.GUI.Library.GUIPropertyManager.SetProperty("#Play.Current.Plot", clear ? "" : Translation._Hidden_to_prevent_spoilers_);

			// Show Episode Thumbnail or Series Poster if Hide Spoilers is enabled
			if (!DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedThumbnail) || m_currentEpisode[DBOnlineEpisode.cWatched])                
                MediaPortal.GUI.Library.GUIPropertyManager.SetProperty("#Play.Current.Thumb", clear ? "" : ImageAllocator.ExtractFullName(localLogos.getFirstEpLogo(m_currentEpisode)));
			else
				MediaPortal.GUI.Library.GUIPropertyManager.SetProperty("#Play.Current.Thumb", clear ? "" : series.Poster);
			
            MediaPortal.GUI.Library.GUIPropertyManager.SetProperty("#Play.Current.Title", clear ? "" : m_currentEpisode.onlineEpisode.CompleteTitle);            
            MediaPortal.GUI.Library.GUIPropertyManager.SetProperty("#Play.Current.Year", clear ? "" : (string)m_currentEpisode[DBOnlineEpisode.cFirstAired]);                        
            MediaPortal.GUI.Library.GUIPropertyManager.SetProperty("#Play.Current.Genre", clear ? "" : series[DBOnlineSeries.cGenre].ToString().Trim('|').Replace("|", ", "));
        }

        void MarkEpisodeAsWatched(DBEpisode episode)
        {
            // Could be a double episode, so mark both as watched
            SQLCondition condition = new SQLCondition();
            condition.Add(new DBEpisode(), DBEpisode.cFilename, episode[DBEpisode.cFilename], SQLConditionType.Equal);
            List<DBEpisode> episodes = DBEpisode.Get(condition, false);
            foreach (DBEpisode ep in episodes)
            {
                ep[DBOnlineEpisode.cWatched] = 1;
                ep.Commit();
                //DBSeason.UpdateUnWatched(ep);
                //DBSeries.UpdateUnWatched(ep);
            }
            // Update Episode Counts
            DBSeries series = Helper.getCorrespondingSeries(m_currentEpisode[DBEpisode.cSeriesID]);
            DBSeason season = Helper.getCorrespondingSeason(episode[DBEpisode.cSeriesID], episode[DBEpisode.cSeasonIndex]);
            DBSeason.UpdatedEpisodeCounts(series, season);           
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
        void OnPlayBackStopped(MediaPortal.Player.g_Player.MediaType type, int timeMovieStopped, string filename)
        {
            if (PlayBackOpIsOfConcern(type, filename))
            {
                LogPlayBackOp("stopped", filename);
                try
                {
                    #region Set Resume Point or Watched                    
                    double watchedAfter = DBOption.GetOptions(DBOption.cWatchedAfter);
                    if (!m_currentEpisode[DBOnlineEpisode.cWatched]
                        && (timeMovieStopped / playlistPlayer.g_Player.Duration) > watchedAfter / 100)
                    {
                        PlaybackOperationEnded(true);
                    }
                    else
                    {
                        m_currentEpisode[DBEpisode.cStopTime] = timeMovieStopped;
                        m_currentEpisode.Commit();
                        PlaybackOperationEnded(false);                        
                    }
                    #endregion
                    
                }
                catch (Exception e)
                {
                    MPTVSeriesLog.Write("TVSeriesPlugin.VideoHandler.OnPlayBackStopped()\r\n" + e.ToString());
                }
            }
        }

        void OnPlayBackEnded(MediaPortal.Player.g_Player.MediaType type, string filename)
        {
            if (PlayBackOpIsOfConcern(type, filename))
            {
                LogPlayBackOp("ended", filename);
                try
                {
                    m_currentEpisode[DBEpisode.cStopTime] = 0;
                    PlaybackOperationEnded(true);
                }
                catch (Exception e)
                {
                    MPTVSeriesLog.Write("TVSeriesPlugin.VideoHandler.OnPlayBackEnded()\r\n" + e.ToString());
                }
            }
        }

        void OnPlayBackStarted(MediaPortal.Player.g_Player.MediaType type, string filename)
        {
            if (PlayBackOpIsOfConcern(type, filename))
            {
                LogPlayBackOp("started", filename);
                // really stupid, you have to wait until the player itself sets the properties (a few seconds) and after that set them
                w.RunWorkerAsync(false);
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
        bool PlayBackOpIsOfConcern(MediaPortal.Player.g_Player.MediaType type, string filename)
        {
            return (m_currentEpisode != null && 
                    type == g_Player.MediaType.Video && 
                    m_currentEpisode[DBEpisode.cFilename] == filename);
        }

        void PlaybackOperationEnded(bool countAsWatched)
        {
            if (countAsWatched || m_currentEpisode[DBOnlineEpisode.cWatched])
            {
                MPTVSeriesLog.Write("This episode counts as watched");
                if(countAsWatched) MarkEpisodeAsWatched(m_currentEpisode);
                // if the ep wasn't rated before, and the option to ask is set, bring up the ratings menu
                if ((String.IsNullOrEmpty(m_currentEpisode[DBOnlineEpisode.cMyRating]) || m_currentEpisode[DBOnlineEpisode.cMyRating] == 0) && DBOption.GetOptions(DBOption.cAskToRate))
                {
                    MPTVSeriesLog.Write("Episode not rated yet");
                    if(RateRequestOccured != null)
                        RateRequestOccured.Invoke(m_currentEpisode);
                } else MPTVSeriesLog.Write("Episode has already been rated or option not set");
            }
            SetGUIProperties(true); // clear GUI Properties          
        }

        void LogPlayBackOp(string OperationType, string filename)
        {
            MPTVSeriesLog.Write(string.Format("Playback {0} for: {1}", OperationType, filename), MPTVSeriesLog.LogLevel.Normal);
        }        

        #endregion
    }
}
