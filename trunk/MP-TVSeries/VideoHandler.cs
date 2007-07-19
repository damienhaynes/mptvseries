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
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortal.Dialogs;
using MediaPortal.Util;
using MediaPortal.Playlists;
using MediaPortal.Video.Database;
using WindowPlugins.GUITVSeries;

using MediaPortal.GUI.Video;

namespace WindowPlugins.GUITVSeries
{
    class VideoHandler
    {
        static PlayListPlayer playlistPlayer;
        DBEpisode m_currentEpisode;
        System.ComponentModel.BackgroundWorker w = new System.ComponentModel.BackgroundWorker();

        public VideoHandler()
        {
            playlistPlayer = PlayListPlayer.SingletonPlayer;

            g_Player.PlayBackStopped += new MediaPortal.Player.g_Player.StoppedHandler(OnPlayBackStopped);
            g_Player.PlayBackEnded += new MediaPortal.Player.g_Player.EndedHandler(OnPlayBackEnded);
            g_Player.PlayBackStarted += new MediaPortal.Player.g_Player.StartedHandler(OnPlayBackStarted);
            w.DoWork += new System.ComponentModel.DoWorkEventHandler(w_DoWork);
        }

        void w_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            SetGUIProperties((bool)e.Argument);
        }


        void SetGUIProperties(bool clear)
        {
            #region availProperties
            //#Play.Current.Thumb
            //#Play.Current.File
            //#Play.Current.Title
            //#Play.Current.Genre
            //#Play.Current.Comment
            //#Play.Current.Artist
            //#Play.Current.Director
            //#Play.Current.Album
            //#Play.Current.Track
            //#Play.Current.Year
            //#Play.Current.Duration
            //#Play.Current.Plot
            //#Play.Current.PlotOutline
            //#Play.Current.Channel
            //#Play.Current.Cast
            //#Play.Current.DVDLabel
            //#Play.Current.IMDBNumber
            //#Play.Current.Rating
            //#Play.Current.TagLine
            //#Play.Current.Votes
            //#Play.Current.Credits
            //#Play.Current.Runtime
            //#Play.Current.MPAARating
            //#Play.Current.IsWatched
            #endregion

            MPTVSeriesLog.Write("SetGUIProperties: " + clear.ToString());
            MPTVSeriesLog.Write(MediaPortal.GUI.Library.GUIPropertyManager.GetProperty("#Play.Current.Title"));
            DBSeries series = null;
            if(!clear) series = Helper.getCorrespondingSeries(m_currentEpisode[DBEpisode.cSeriesID]);

            MediaPortal.GUI.Library.GUIPropertyManager.SetProperty("#Play.Current.Title", clear ? "" : m_currentEpisode.onlineEpisode.CompleteTitle);
            MediaPortal.GUI.Library.GUIPropertyManager.SetProperty("#Play.Current.Plot", clear ? "" : (string)m_currentEpisode[DBOnlineEpisode.cEpisodeSummary]);
            MediaPortal.GUI.Library.GUIPropertyManager.SetProperty("#Play.Current.Thumb", clear ? "" : localLogos.getFirstEpLogo(m_currentEpisode));
            MediaPortal.GUI.Library.GUIPropertyManager.SetProperty("#Play.Current.Year", clear ? "" : (string)m_currentEpisode[DBOnlineEpisode.cFirstAired]);
            MPTVSeriesLog.Write(MediaPortal.GUI.Library.GUIPropertyManager.GetProperty("#Play.Current.Title"));
        }

        public bool ResumeOrPlay(DBEpisode episode)
        {
            try
            {
                MPTVSeriesLog.Write("Attempting to play: ", episode[DBEpisode.cFilename].ToString(), MPTVSeriesLog.LogLevel.Normal);
                // don't have this file !
                if (episode[DBEpisode.cFilename].ToString().Length == 0)
                    return false;

                m_currentEpisode = episode;

                byte[] resumeData = null; // I don't even need resumeData?
                int timeMovieStopped = m_currentEpisode[DBEpisode.cStopTime];

                if (timeMovieStopped > 0)
                {
                    GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                    if (null == dlgYesNo) 
                        return false;
                    dlgYesNo.SetHeading(GUILocalizeStrings.Get(900)); //resume movie?
                    dlgYesNo.SetLine(1, m_currentEpisode.onlineEpisode.CompleteTitle);
                    dlgYesNo.SetLine(2, GUILocalizeStrings.Get(936) + " " + Utils.SecondsToHMSString(timeMovieStopped));
                    dlgYesNo.SetDefaultToYes(true);
                    dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
                    if (dlgYesNo.IsConfirmed)
                    {
                        Play(timeMovieStopped, resumeData);
                        return true;
                    }
                    else
                    {
                        m_currentEpisode[DBEpisode.cStopTime] = 0;
                        m_currentEpisode.Commit();
                    }
                }

                Play(-1, null);
                return true;
            }
            catch (Exception e)
            {
                MPTVSeriesLog.Write("TVSeriesPlugin.VideoHandler.ResumeOrPlay()\r\n" + e.ToString());
                return false;
            }
        }

        private void Play()
        {
        }

        private void Play(int timeMovieStopped, byte[] resumeData)
        {
            try
            {
                playlistPlayer.Reset();
                playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_VIDEO_TEMP;
                PlayList playlist = playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_VIDEO_TEMP);
                playlist.Clear();

                PlayListItem itemNew = new PlayListItem();
                itemNew.FileName = m_currentEpisode[DBEpisode.cFilename];
                itemNew.Type = PlayListItem.PlayListItemType.Video;
                playlist.Add(itemNew);

                playlistPlayer.Play(0);

                if (g_Player.Playing && timeMovieStopped > 0)
                {
                    if (g_Player.IsDVD)
                    {
                        g_Player.Player.SetResumeState(resumeData);
                    }
                    else
                    {
                        GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_SEEK_POSITION, 0, 0, 0, 0, 0, null);
                        msg.Param1 = (int)timeMovieStopped;
                        GUIGraphicsContext.SendMessage(msg);
                    }
                }
            }
            catch (Exception e)
            {
                MPTVSeriesLog.Write("TVSeriesPlugin.VideoHandler.Play()\r\n" + e.ToString());
            }
        }


        private void OnPlayBackStopped(MediaPortal.Player.g_Player.MediaType type, int timeMovieStopped, string filename)
        {
            if (m_currentEpisode != null)
            {
                MPTVSeriesLog.Write("Playback stopped for: ", filename, MPTVSeriesLog.LogLevel.Normal);
                try
                {
                    if (type != g_Player.MediaType.Video) return;

                    if (m_currentEpisode != null && m_currentEpisode[DBEpisode.cFilename] == filename)
                    {
                        w.RunWorkerAsync(true);
                        m_currentEpisode[DBEpisode.cStopTime] = timeMovieStopped;

                        double watchedAfter = DBOption.GetOptions(DBOption.cWatchedAfter);
                        if (!m_currentEpisode[DBOnlineEpisode.cWatched]
                            && (timeMovieStopped / playlistPlayer.g_Player.Duration) > watchedAfter/100) 
                        {
                            m_currentEpisode[DBOnlineEpisode.cWatched] = 1;
                        }
                        m_currentEpisode.Commit();
                        m_currentEpisode = null;
                    }
                }
                catch (Exception e)
                {
                    MPTVSeriesLog.Write("TVSeriesPlugin.VideoHandler.OnPlayBackStopped()\r\n" + e.ToString());
                }
            }
        }

        private void OnPlayBackEnded(MediaPortal.Player.g_Player.MediaType type, string filename)
        {
            if (m_currentEpisode != null)
            {
                MPTVSeriesLog.Write("Playback ended for: ", filename, MPTVSeriesLog.LogLevel.Normal);
                try
                {
                    if (type != g_Player.MediaType.Video) return;

                    if (m_currentEpisode != null && m_currentEpisode[DBEpisode.cFilename] == filename)
                    {
                        w.RunWorkerAsync(true);
                        m_currentEpisode[DBEpisode.cStopTime] = 0;
                        m_currentEpisode[DBOnlineEpisode.cWatched] = 1;
                        m_currentEpisode.Commit();
                        m_currentEpisode = null;
                    }
                }
                catch (Exception e)
                {
                    MPTVSeriesLog.Write("TVSeriesPlugin.VideoHandler.OnPlayBackEnded()\r\n" + e.ToString());
                }
            }
        }

        private void OnPlayBackStarted(MediaPortal.Player.g_Player.MediaType type, string filename)
        {
            if (m_currentEpisode != null)
            {
                MPTVSeriesLog.Write("Playback started for: ", filename, MPTVSeriesLog.LogLevel.Normal);
                try
                {
                    if (type != g_Player.MediaType.Video) return;
                    w.RunWorkerAsync(false); // really stupid, you have to wait until the player itself sets the properties (a few seconds) and after that set them
                }
                catch (Exception e)
                {
                    MPTVSeriesLog.Write("TVSeriesPlugin.VideoHandler.OnPlayBackStarted()\r\n" + e.ToString());
                }
            }
        }
    }
}