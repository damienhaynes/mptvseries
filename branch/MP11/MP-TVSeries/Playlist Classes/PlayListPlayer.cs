#region Copyright (C) 2005-2008 Team MediaPortal

/* 
 *	Copyright (C) 2005-2008 Team MediaPortal
 *	http://www.team-mediaportal.com
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *   
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *   
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA. 
 *  http://www.gnu.org/copyleft/gpl.html
 *
 */

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using MediaPortal.GUI.Library;
using MediaPortal.GUI.Video;
using MediaPortal.Util;
using MediaPortal.Player;
using MediaPortal.Playlists;
using MediaPortal.Profile;
using MediaPortal.Configuration;
using Trakt;
using Trakt.Show;

namespace WindowPlugins.GUITVSeries
{
    public class PlayListPlayer
    {
        #region g_Player

        public interface IPlayer
        {
            bool Playing { get; }
            void Release();
            bool Play(string strFile);                        
            void Stop();
            void SeekAsolutePercentage(int iPercentage);
            double Duration { get; }
            double CurrentPosition { get; }
            void SeekAbsolute(double dTime);
            bool HasVideo { get; }
            bool ShowFullScreenWindow();
        }

        private class FakePlayer : IPlayer
        {
            public bool Playing
            {
                get { return MediaPortal.Player.g_Player.Playing; }
            }

            public void Release()
            {
                MediaPortal.Player.g_Player.Release();
            }

            bool IPlayer.Play(string strFile)
            {
                return MediaPortal.Player.g_Player.Play(strFile);
            }
         
            public void Stop()
            {
                MediaPortal.Player.g_Player.Stop();
            }

            public void SeekAsolutePercentage(int iPercentage)
            {
                MediaPortal.Player.g_Player.SeekAsolutePercentage(iPercentage);
            }

            public double Duration
            {
                get { return MediaPortal.Player.g_Player.Duration; }
            }

            public double CurrentPosition
            {
                get { return MediaPortal.Player.g_Player.CurrentPosition; }
            }

            public void SeekAbsolute(double dTime)
            {
                MediaPortal.Player.g_Player.SeekAbsolute(dTime);
            }

            public bool HasVideo
            {
                get { return MediaPortal.Player.g_Player.HasVideo; }
            }

            public bool ShowFullScreenWindow()
            {
                return MediaPortal.Player.g_Player.ShowFullScreenWindow();
            }
        }

        public IPlayer g_Player = new FakePlayer();
        
        #endregion

        int _entriesNotFound = 0;
        int _currentItem = -1;
        PlayListType _currentPlayList = PlayListType.PLAYLIST_NONE;
        PlayList _tvseriesPlayList = new PlayList();        
        PlayList _emptyPlayList = new PlayList();    
        bool _repeatPlayList = DBOption.GetOptions(DBOption.cRepeatPlaylist);
        bool _playlistAutoPlay = DBOption.GetOptions(DBOption.cPlaylistAutoPlay);
		bool _playlistAutoShuffle = DBOption.GetOptions(DBOption.cPlaylistAutoShuffle);
        string _currentPlaylistName = string.Empty;		
		private bool listenToExternalPlayerEvents = false;
        private Timer m_TraktTimer = null;
        private TimerCallback m_timerDelegate = null;
        BackgroundWorker TraktScrobbleUpdater = new BackgroundWorker();
        private bool TraktMarkedFirstAsWatched = false;

        public PlayListPlayer()
        {
            Init();
        }

        private static PlayListPlayer singletonPlayer = new PlayListPlayer();

        public static PlayListPlayer SingletonPlayer
        {
            get
            {
                return singletonPlayer;
            }
        }

        public void Init()
        {
            GUIWindowManager.Receivers += new SendMessageHandler(this.OnMessage);

			// external player handlers
			Utils.OnStartExternal += new Utils.UtilEventHandler(onStartExternal);
			Utils.OnStopExternal += new Utils.UtilEventHandler(onStopExternal);

            // trakt scrobble background thread
            TraktScrobbleUpdater.WorkerSupportsCancellation = true;
            TraktScrobbleUpdater.DoWork += new DoWorkEventHandler(TraktScrobble_DoWork);
        }

        public void OnMessage(GUIMessage message)
        {
            switch (message.Message)
            {
                case GUIMessage.MessageType.GUI_MSG_PLAYBACK_STOPPED:
                {
                    PlayListItem item = GetCurrentItem();
                    if (item != null)
                    {
                        // cancel trakt watch timer
                        if (m_TraktTimer != null) m_TraktTimer.Dispose();

                        Reset();
                         _currentPlayList = PlayListType.PLAYLIST_NONE;
                         SetProperties(item, true);
                    }
                    GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_FOCUS, 0, 0, 0, -1, 0, null);
                    GUIGraphicsContext.SendMessage(msg);                    
                }
                break;

                case GUIMessage.MessageType.GUI_MSG_PLAYBACK_ENDED:
                {
                    #region Trakt
                    // submit watched state to trakt API
                    // could be a double episode so set both episodes as watched
                    PlayListItem item = GetCurrentItem();

                    if (item != null)
                    {
                        if (item.Episode != null)
                        {
                            if (item.Episode[DBEpisode.cEpisodeIndex2] > 0)
                            {
                                // only set 2nd episode as watched here
                                SQLCondition condition = new SQLCondition();
                                condition.Add(new DBEpisode(), DBEpisode.cFilename, item.FileName, SQLConditionType.Equal);
                                List<DBEpisode> episodes = DBEpisode.Get(condition, false);
                                TraktScrobbleUpdater.RunWorkerAsync(episodes[1]);
                            }
                            else
                            {
                                TraktScrobbleUpdater.RunWorkerAsync(item.Episode);
                            }
                        }
                    }
                    #endregion

                    SetAsWatched();
                    PlayNext();
                    if (!g_Player.Playing)
                    {
                        g_Player.Release();

                        // Clear focus when playback ended
                        GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_FOCUS, 0, 0, 0, -1, 0, null);
                        GUIGraphicsContext.SendMessage(msg);
                    }
                }
                break;

                case GUIMessage.MessageType.GUI_MSG_PLAY_FILE:
                {
                    MPTVSeriesLog.Write(string.Format("Playlistplayer: Start file ({0})", message.Label));
                    g_Player.Play(message.Label);
                    if (!g_Player.Playing) g_Player.Stop();
                }
                break;
        
                case GUIMessage.MessageType.GUI_MSG_STOP_FILE:
                {
                    MPTVSeriesLog.Write(string.Format("Playlistplayer: Stop file"));
                    g_Player.Stop();
                }
                break;

                case GUIMessage.MessageType.GUI_MSG_SEEK_FILE_PERCENTAGE:
                {
                    MPTVSeriesLog.Write(string.Format("Playlistplayer: SeekPercent ({0}%)", message.Param1));
                    g_Player.SeekAsolutePercentage(message.Param1);
                    MPTVSeriesLog.Write(string.Format("Playlistplayer: SeekPercent ({0}%) done", message.Param1),MPTVSeriesLog.LogLevel.Debug);
                }
                break;
        
                case GUIMessage.MessageType.GUI_MSG_SEEK_FILE_END:
                {
                    double duration = g_Player.Duration;
                    double position = g_Player.CurrentPosition;
                    if (position < duration - 1d)
                    {
                        MPTVSeriesLog.Write(string.Format("Playlistplayer: SeekEnd ({0})", duration));
                        g_Player.SeekAbsolute(duration - 2d);
                        MPTVSeriesLog.Write(string.Format("Playlistplayer: SeekEnd ({0}) done", g_Player.CurrentPosition),MPTVSeriesLog.LogLevel.Debug);
                    }
                }
                break;
        
                case GUIMessage.MessageType.GUI_MSG_SEEK_POSITION:
                {
                    g_Player.SeekAbsolute(message.Param1);
                }
                break;
            }
        }

        public string Get(int iItem)
        {
            if (_currentPlayList == PlayListType.PLAYLIST_NONE) return string.Empty;

            PlayList playlist = GetPlaylist(_currentPlayList);
            if (playlist.Count <= 0) return string.Empty;

            if (iItem >= playlist.Count)
            {
                if (!_repeatPlayList)
                {
                    return string.Empty; ;
                }
                iItem = 0;
            }

            PlayListItem item = playlist[iItem];
            return item.FileName;
        }

        public PlayListItem GetCurrentItem()
        {
            if (_currentItem < 0) return null;

            PlayList playlist = GetPlaylist(_currentPlayList);
            if (playlist == null) return null;

            if (_currentItem < 0 || _currentItem >= playlist.Count)
                _currentItem = 0;

            if (_currentItem >= playlist.Count) return null;

            return playlist[_currentItem];
        }

        public PlayListItem GetNextItem()
        {
            if (_currentPlayList == PlayListType.PLAYLIST_NONE) return null;

            PlayList playlist = GetPlaylist(_currentPlayList);
            if (playlist.Count <= 0) return null;
            int iItem = _currentItem;
            iItem++;

            if (iItem >= playlist.Count)
            {
                if (!_repeatPlayList)
                    return null;

                iItem = 0;
            }

            PlayListItem item = playlist[iItem];
            return item;
        }

        public string GetNext()
        {
            PlayListItem resultingItem = GetNextItem();
            if (resultingItem != null)
                return resultingItem.FileName;
            else
                return string.Empty;
        }

        public void PlayNext()
        {
            if (_currentPlayList == PlayListType.PLAYLIST_NONE) return;

            // cancel trakt watch timer
            if (m_TraktTimer != null) m_TraktTimer.Dispose();

            PlayList playlist = GetPlaylist(_currentPlayList);
            if (playlist.Count <= 0) return;
            int iItem = _currentItem;
            iItem++;

            if (iItem >= playlist.Count)
            {
                if (!_repeatPlayList)
                {
                    _currentPlayList = PlayListType.PLAYLIST_NONE;
                     return;
                }
                iItem = 0;
            }

            if (!Play(iItem))
            {
                if (!g_Player.Playing)
                {
                    PlayNext();
                }
            }
        }

        public void PlayPrevious()
        {            
            if (_currentPlayList == PlayListType.PLAYLIST_NONE)
                return;

            // cancel trakt watch timer
            if (m_TraktTimer != null) m_TraktTimer.Dispose();

            PlayList playlist = GetPlaylist(_currentPlayList);
            if (playlist.Count <= 0) return;
            int iItem = _currentItem;
            iItem--;
            if (iItem < 0)
                iItem = playlist.Count - 1;

            if (!Play(iItem))
            {
                if (!g_Player.Playing)
                {
                    PlayPrevious();
                }
            }
        }

        public void Play(string filename)
        {
            if (_currentPlayList == PlayListType.PLAYLIST_NONE)
                return;

            PlayList playlist = GetPlaylist(_currentPlayList);
            for (int i = 0; i < playlist.Count; ++i)
            {
                PlayListItem item = playlist[i];
                if (item.FileName.Equals(filename))
                {
                    Play(i);
                    return;
                }
            }
        }

        public bool Play(int iItem)
        {
            // if play returns false PlayNext is called but this does not help against selecting an invalid file
            bool skipmissing = false;
            do
            {
                if (_currentPlayList == PlayListType.PLAYLIST_NONE)
                {
                    MPTVSeriesLog.Write("PlaylistPlayer.Play() no playlist selected",MPTVSeriesLog.LogLevel.Debug);
                    return false;
                }
                PlayList playlist = GetPlaylist(_currentPlayList);
                if (playlist.Count <= 0)
                {
                    MPTVSeriesLog.Write("PlaylistPlayer.Play() playlist is empty",MPTVSeriesLog.LogLevel.Debug);
                    return false;
                }
                if (iItem < 0) iItem = 0;
                if (iItem >= playlist.Count)
                {
                    if (skipmissing)
                        return false;
                    else
                    {
                        if (_entriesNotFound < playlist.Count)
                            iItem = playlist.Count - 1;
                        else
                            return false;
                    }
                }
                
                _currentItem = iItem;
                PlayListItem item = playlist[_currentItem];

                GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_FOCUS, 0, 0, 0, _currentItem, 0, null);
                msg.Label = item.FileName;
                GUIGraphicsContext.SendMessage(msg);

                if (playlist.AllPlayed())
                {
                    playlist.ResetStatus();
                }

                bool playResult = false;

                // If the file is an image file, it should be mounted before playing
                string filename = item.FileName;
                if (Helper.IsImageFile(filename)) {
                    if (!GUIVideoFiles.MountImageFile(GUIWindowManager.ActiveWindow, filename)) {
                        return false;
                    }
                }

                // Start Listening to any External Player Events
				listenToExternalPlayerEvents = true;

                #region Publish Play properties for InfoService plugin
                string seriesName = item.SeriesName;
                string seasonID = item.SeasonIndex;
                string episodeID = item.EpisodeIndex;
                string episodeName = item.EpisodeName;
                GUIPropertyManager.SetProperty("#TVSeries.Extended.Title", string.Format("{0}/{1}/{2}/{3}", seriesName, seasonID, episodeID, episodeName));
                MPTVSeriesLog.Write(string.Format("#TVSeries.Extended.Title: {0}/{1}/{2}/{3}", seriesName, seasonID, episodeID, episodeName));
                #endregion

                // Play File
                playResult = g_Player.Play(filename);

                // Stope Listening to any External Player Events
				listenToExternalPlayerEvents = false;

                if (!playResult)
                {
                    //	Count entries in current playlist
                    //	that couldn't be played
                    _entriesNotFound++;
                    MPTVSeriesLog.Write(string.Format("PlaylistPlayer: *** unable to play - {0} - skipping file!", item.FileName));

                    // do not try to play the next file list
                    if (Utils.IsVideo(item.FileName))
                        skipmissing = false;
                    else
                        skipmissing = true;

                    iItem++;
                }
                else
                {
                    item.Played = true;
                    item.IsWatched = true; // for facade watched icons
                    skipmissing = false;
                    if (Utils.IsVideo(item.FileName))
                    {
                        if (g_Player.HasVideo)
                        {
                            g_Player.ShowFullScreenWindow();
                            Thread.Sleep(2000);
                            SetProperties(item, false);
                            // timer for trakt watcher status every 15mins
                            if (m_timerDelegate == null) m_timerDelegate = new TimerCallback(TraktUpdater);
                            m_TraktTimer = new Timer(m_timerDelegate, item, 3000, 900000);

                            TraktMarkedFirstAsWatched = false;
                        }
                    }
                }
            }
            while (skipmissing);
            return g_Player.Playing;
        }

        /// <summary>
        /// Create scrobble data that can be used to send to Trakt API
        /// </summary>
        private TraktEpisodeScrobble CreateScrobbleData(DBEpisode episode)
        {
            string username = TraktAPI.Username;
            string password = TraktAPI.Password;

            // check if trakt is enabled
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            DBSeries series = Helper.getCorrespondingSeries(episode[DBEpisode.cSeriesID]);
            if (series == null) return null;

            // create scrobble data
            TraktEpisodeScrobble scrobbleData = new TraktEpisodeScrobble
            {
                Title = series[DBOnlineSeries.cOriginalName],
                Year = DBSeries.GetSeriesYear(series),
                Season = episode[DBOnlineEpisode.cSeasonIndex],
                Episode = episode.TraktEpisode,
                SeriesID = series[DBSeries.cID],
                PluginVersion = Settings.Version.ToString(),
                MediaCenter = "mp-tvseries",
                MediaCenterVersion = Settings.MPVersion.ToString(),
                MediaCenterBuildDate = Settings.MPBuildDate.ToString("yyyy-MM-dd HH:mm:ss"),
                UserName = username,
                Password = password
            };

            return scrobbleData;
        }

        /// <summary>
        /// Update Trakt status of episode being watched on Timer Interval
        /// </summary>
        private void TraktUpdater(Object stateInfo)
        {
            PlayListItem item = (PlayListItem)stateInfo;

            // duration in minutes
            double duration = item.Duration / 60000;
            double progress = 0.0;

            // get current progress of player (in seconds) to work out percent complete
            if (duration > 0.0)
                progress = ((g_Player.CurrentPosition / 60.0) / duration) * 100.0;

            TraktEpisodeScrobble scrobbleData = null;

            // check if double episode has passed halfway mark and set as watched
            if (item.Episode[DBEpisode.cEpisodeIndex2] > 0 && progress > 50.0)
            {
                SQLCondition condition = new SQLCondition();
                condition.Add(new DBEpisode(), DBEpisode.cFilename, item.FileName, SQLConditionType.Equal);
                List<DBEpisode> episodes = DBEpisode.Get(condition, false);

                if (!TraktMarkedFirstAsWatched)
                {
                    // send scrobble Watched status of first episode
                    TraktScrobbleUpdater.RunWorkerAsync(item.Episode);
                    TraktMarkedFirstAsWatched = true;
                    Thread.Sleep(5000);
                }

                // we are now watching 2nd part of episode
                scrobbleData = CreateScrobbleData(episodes[1]);
            }
            else
            {
                // we are watched single episode or 1st part of double episode
                scrobbleData = CreateScrobbleData(item.Episode);
            }

            if (scrobbleData == null) return;

            // set duration/progress in scrobble data
            scrobbleData.Duration = Convert.ToInt32(duration).ToString();
            scrobbleData.Progress = Convert.ToInt32(progress).ToString();

            // set watching status on trakt
            TraktResponse response = TraktAPI.ScrobbleShowState(scrobbleData, TraktAPI.Status.watching);
            if (response == null) return;
            CheckTraktErrorAndNotify(response);
        }

        /// <summary>
        /// Update trakt status on playback finish
        /// </summary>
        private void TraktScrobble_DoWork(object sender, DoWorkEventArgs e)
        {
            DBEpisode episode = (DBEpisode)e.Argument;
      
            double duration = episode[DBEpisode.cLocalPlaytime] / 60000;

            // get scrobble data to send to api
            TraktEpisodeScrobble scrobbleData = CreateScrobbleData(episode);
            if (scrobbleData == null) return;
            
            // set duration/progress in scrobble data
            scrobbleData.Duration = Convert.ToInt32(duration).ToString();
            scrobbleData.Progress = "100";

            TraktResponse response = TraktAPI.ScrobbleShowState(scrobbleData, TraktAPI.Status.scrobble);
            if (response == null) return;
            CheckTraktErrorAndNotify(response);
        }

        /// <summary>
        /// Notify user in GUI if an error state was returned from Trakt API
        /// </summary>
        private void CheckTraktErrorAndNotify(TraktResponse response)
        {
            if (response.Status == null) return;

            // check response error status
            if (response.Status != "success")
            {
                MPTVSeriesLog.Write("Trakt Error: {0}", response.Error);
                TVSeriesPlugin.ShowNotifyDialog(Translation.TraktError, response.Error);
            }
            else
            {
                // success
                MPTVSeriesLog.Write("Trakt Response: {0}", response.Message);
            }
        }

        /// <summary>        
        /// Updates the movie metadata on the playback screen (for when the user clicks info). 
        /// The delay is neccesary because Player tries to use metadata from the MyVideos database.
        /// We want to update this after that happens so the correct info is there.       
        /// </summary>
        /// <param name="item">Playlist item</param>
        /// <param name="clear">Clears the properties instead of filling them if True</param>
        private void SetProperties(PlayListItem item, bool clear)
        {
            if (item == null) return;

            string title = string.Empty;
            DBSeries series = null;
            DBSeason season = null;

            if (!clear)
            {
                title = string.Format("{0} - {1}x{2} - {3}", item.SeriesName, item.SeasonIndex, item.EpisodeIndex, item.EpisodeName);
                series = Helper.getCorrespondingSeries(item.Episode[DBEpisode.cSeriesID]);
                season = Helper.getCorrespondingSeason(item.Episode[DBEpisode.cSeriesID], int.Parse(item.SeasonIndex));
            }

            // Show Plot in OSD or Hide Spoilers (note: FieldGetter takes care of that)         
            GUIPropertyManager.SetProperty("#Play.Current.Plot", clear ? " " : FieldGetter.resolveDynString(TVSeriesPlugin.m_sFormatEpisodeMain, item.Episode));

            // Show Episode Thumbnail or Series Poster if Hide Spoilers is enabled
            string osdImage = string.Empty;            
            if (!clear)
            {
                foreach (KeyValuePair<string, string> kvp in SkinSettings.VideoOSDImages)
                {
                    switch (kvp.Key)
                    {
                        case "episode":
                            if (!DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedThumbnail) || item.Episode[DBOnlineEpisode.cWatched])
                                osdImage = ImageAllocator.ExtractFullName(localLogos.getFirstEpLogo(item.Episode));
                            break;
                        case "season":
                            osdImage = season.Banner;
                            break;
                        case "series":
                            osdImage = series.Poster;
                            break;
                        case "custom":
                            string value = replaceDynamicFields(kvp.Value, item.Episode);
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

            foreach (KeyValuePair<string, string> kvp in SkinSettings.VideoPlayImages)
            {
                if (!clear)
                {
                    string value = replaceDynamicFields(kvp.Value, item.Episode);
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

            GUIPropertyManager.SetProperty("#Play.Current.Title", clear ? "" : title);
            GUIPropertyManager.SetProperty("#Play.Current.Year", clear ? "" : item.FirstAired);
            GUIPropertyManager.SetProperty("#Play.Current.Genre", clear ? "" : FieldGetter.resolveDynString(TVSeriesPlugin.m_sFormatEpisodeSubtitle, item.Episode));
        }

        private string replaceDynamicFields(string value, DBTable item)
        {
            string result = value;

            Regex matchRegEx = new Regex(@"\<[a-zA-Z\.]+\>");
            foreach (Match m in matchRegEx.Matches(value))
            {
                string resolvedValue = FieldGetter.resolveDynString(m.Value, item, false);
                result = result.Replace(m.Value, resolvedValue);
            }

            return result;
        }

        private void SetAsWatched()
        {
            PlayListItem item = GetCurrentItem();
            if (item == null)
                return;

            item.Watched = true;
        }

        public int CurrentItem
        {
            get { return _currentItem; }
            set
            {     
                if (value >= -1 && value < GetPlaylist(CurrentPlaylistType).Count)
                    _currentItem = value;
            }
        }

        public string CurrentPlaylistName
        {
            get { return _currentPlaylistName; }
            set { _currentPlaylistName = value; }
        }

        public void Remove(PlayListType type, string filename)
        {
            PlayList playlist = GetPlaylist(type);
            int itemRemoved = playlist.Remove(filename);
            if (type != CurrentPlaylistType)
            {
                return;
            }
            if (_currentItem >= itemRemoved) _currentItem--;
        }

        public PlayListType CurrentPlaylistType
        {
            get { return _currentPlayList; }
            set
            {
                if (_currentPlayList != value)
                {
                    _currentPlayList = value;
                    _entriesNotFound = 0;
                }
            }
        }

        public PlayList GetPlaylist(PlayListType nPlayList)
        {
            switch (nPlayList)
            {        
                case PlayListType.PLAYLIST_TVSERIES: return _tvseriesPlayList;                
                default:
                    _emptyPlayList.Clear();
                    return _emptyPlayList;
            }
        }    

        public void Reset()
        {
            _currentItem = -1;
            _entriesNotFound = 0;
        }

        public int EntriesNotFound
        {
            get
            {
                return _entriesNotFound;
            }
        }

        public bool RepeatPlaylist
        {
            get { return _repeatPlayList; }
            set { _repeatPlayList = value; }
        }

        public bool PlaylistAutoPlay
        {
            get { return _playlistAutoPlay; }
            set { _playlistAutoPlay = value; }
        }

		public bool PlaylistAutoShuffle {
			get { return _playlistAutoShuffle; }
			set { _playlistAutoShuffle = value; }
		}

		#region External Player Event Handlers
		private void onStartExternal(Process proc, bool waitForExit) {
			// If we were listening for external player events
			if (listenToExternalPlayerEvents) {
				MPTVSeriesLog.Write("Playback Started in External Player");
			}
		}

		private void onStopExternal(Process proc, bool waitForExit) {
			if (!listenToExternalPlayerEvents)
				return;

			MPTVSeriesLog.Write("Playback Stopped in External Player");
			SetAsWatched();
			PlayNext();
			if (!g_Player.Playing) {
				g_Player.Release();

				// Clear focus when playback ended
				GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_FOCUS, 0, 0, 0, -1, 0, null);
				GUIGraphicsContext.SendMessage(msg);
			}			
		}
		#endregion

    }
}
