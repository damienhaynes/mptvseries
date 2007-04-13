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

        public VideoHandler()
        {
            playlistPlayer = PlayListPlayer.SingletonPlayer;

            g_Player.PlayBackStopped += new MediaPortal.Player.g_Player.StoppedHandler(OnPlayBackStopped);
            g_Player.PlayBackEnded += new MediaPortal.Player.g_Player.EndedHandler(OnPlayBackEnded);
            g_Player.PlayBackStarted += new MediaPortal.Player.g_Player.StartedHandler(OnPlayBackStarted);
        }

        public bool ResumeOrPlay(DBEpisode episode)
        {
            try
            {
                MPTVSeriesLog.Write("Attempting to play: ", episode[DBEpisode.cFilename].ToString(), MPTVSeriesLog.LogLevel.Normal);
                // don't have this file !
                if (episode[DBEpisode.cFilename] == String.Empty)
                    return false;

                m_currentEpisode = episode;
                IMDBMovie movieDetails = new IMDBMovie();
                int timeMovieStopped = 0;

                int idFile = VideoDatabase.GetFileId(m_currentEpisode[DBEpisode.cFilename]);
                int idMovie = VideoDatabase.GetMovieId(m_currentEpisode[DBEpisode.cFilename]);

                if ((idMovie >= 0) && (idFile >= 0))
                {
                    VideoDatabase.GetMovieInfo(m_currentEpisode[DBEpisode.cFilename], ref movieDetails);
                    FillMovieDetails(ref movieDetails);

                    byte[] resumeData = null;
                    VideoDatabase.SetMovieInfo(m_currentEpisode[DBEpisode.cFilename], ref movieDetails);

                    timeMovieStopped = VideoDatabase.GetMovieStopTimeAndResumeData(idFile, out resumeData);


                    if (timeMovieStopped > 0)
                    {
                        GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                        if (null == dlgYesNo) 
                            return false;
                        dlgYesNo.SetHeading(GUILocalizeStrings.Get(900)); //resume movie?
                        dlgYesNo.SetLine(1, movieDetails.Title);
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
                            VideoDatabase.DeleteMovieStopTime(idFile);
                        }
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

        void FillMovieDetails(ref IMDBMovie details)
        {
            DBOnlineSeries series = new DBOnlineSeries(m_currentEpisode[DBEpisode.cSeriesID]);
            details.Title = series[DBOnlineSeries.cPrettyName] + " " + m_currentEpisode[DBOnlineEpisode.cSeasonIndex] + "x" + m_currentEpisode[DBOnlineEpisode.cEpisodeIndex] + ": " + m_currentEpisode[DBOnlineEpisode.cEpisodeName];
            details.Plot = m_currentEpisode[DBOnlineEpisode.cEpisodeSummary];
            //details.SingleGenre = series[DBOnlineSeries.cGenre].ToString().Replace("|", " ");
            details.WritingCredits = "TVSeries"; // so it is identifyable what mp-tvseries stored
        }

        void AddFileToDatabase()
        {
            try
            {

                if (!Utils.IsVideo(m_currentEpisode[DBEpisode.cFilename])) return;
                if (PlayListFactory.IsPlayList(m_currentEpisode[DBEpisode.cFilename])) return;

                if (!VideoDatabase.HasMovieInfo(m_currentEpisode[DBEpisode.cFilename]))
                {
                    int iidMovie = VideoDatabase.AddMovieFile(m_currentEpisode[DBEpisode.cFilename]);
                    IMDBMovie details = new IMDBMovie();
                    VideoDatabase.GetMovieInfoById(iidMovie, ref details);
                    FillMovieDetails(ref details);
                    VideoDatabase.SetMovieInfo(m_currentEpisode[DBEpisode.cFilename], ref details);
                }
            }
            catch (Exception e)
            {
                MPTVSeriesLog.Write("TVSeriesPlugin.VideoHandler.AddFileToDatabase()\r\n" + e.ToString());
            }
        }

        private void OnPlayBackStopped(MediaPortal.Player.g_Player.MediaType type, int timeMovieStopped, string filename)
        {
            MPTVSeriesLog.Write("Playback stopped for: ", filename, MPTVSeriesLog.LogLevel.Normal);
            try
            {
                if (type != g_Player.MediaType.Video) return;

                if (m_currentEpisode != null && m_currentEpisode[DBEpisode.cFilename] == filename)
                {
                    // Handle all movie files from idMovie
                    System.Collections.ArrayList movies = new System.Collections.ArrayList();

                    int iidMovie = VideoDatabase.GetMovieId(filename);

                    VideoDatabase.GetFiles(iidMovie, ref movies);
                    // temporary data, I don't want any series to show in the movie list!
                    VideoDatabase.DeleteMovieInfoById(iidMovie);
                    // yes, it's stupid, I have to add the moviefile after deleting the movieInfo otherwise the resume time isn't saved 
                    iidMovie = VideoDatabase.AddMovieFile(m_currentEpisode[DBEpisode.cFilename]);

                    if (movies.Count <= 0) return;

                    for (int i = 0; i < movies.Count; i++)
                    {
                        string strFilePath = (string)movies[i];
                        int idFile = VideoDatabase.GetFileId(strFilePath);
                        if (idFile < 0) break;

                        if ((filename == strFilePath) && (timeMovieStopped > 0))
                        {
                            byte[] resumeData = null;
                            g_Player.Player.GetResumeState(out resumeData);
                            MPTVSeriesLog.Write("TVSeriesPlugin::OnPlayBackStopped idFile=" + idFile + " timeMovieStopped=" +timeMovieStopped +"resumeData=" + resumeData);
                            VideoDatabase.SetMovieStopTimeAndResumeData(idFile, timeMovieStopped, resumeData);
                            MPTVSeriesLog.Write("TVSeriesPlugin::OnPlayBackStopped store resume time");
                        }
                        else
                        {
                            VideoDatabase.DeleteMovieStopTime(idFile);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MPTVSeriesLog.Write("TVSeriesPlugin.VideoHandler.OnPlayBackStopped()\r\n" + e.ToString());
            }
        }

        private void OnPlayBackEnded(MediaPortal.Player.g_Player.MediaType type, string filename)
        {
            MPTVSeriesLog.Write("Playback ended for: ", filename, MPTVSeriesLog.LogLevel.Normal);
            try
            {
                if (type != g_Player.MediaType.Video) return;

                if (m_currentEpisode != null && m_currentEpisode[DBEpisode.cFilename] == filename)
                {
                    // Handle all movie files from idMovie
                    System.Collections.ArrayList movies = new System.Collections.ArrayList();
                    int iidMovie = VideoDatabase.GetMovieId(filename);
                    // temporary data, I don't want any series to show in the movie list!
                    VideoDatabase.DeleteMovieInfoById(iidMovie);
                    iidMovie = VideoDatabase.AddMovieFile(m_currentEpisode[DBEpisode.cFilename]);
                    if (iidMovie >= 0)
                    {
                        VideoDatabase.GetFiles(iidMovie, ref movies);
                        for (int i = 0; i < movies.Count; i++)
                        {
                            string strFilePath = (string)movies[i];
                            int idFile = VideoDatabase.GetFileId(strFilePath);
                            if (idFile < 0) break;
                            VideoDatabase.DeleteMovieStopTime(idFile);
                        }

                        IMDBMovie details = new IMDBMovie();
                        VideoDatabase.GetMovieInfoById(iidMovie, ref details);
                        details.Watched++;
                        VideoDatabase.SetWatched(details);
                    }
                }
            }
            catch (Exception e)
            {
                MPTVSeriesLog.Write("TVSeriesPlugin.VideoHandler.OnPlayBackEnded()\r\n" + e.ToString());
            }
        }

        private void OnPlayBackStarted(MediaPortal.Player.g_Player.MediaType type, string filename)
        {
            MPTVSeriesLog.Write("Playback started for: ", filename, MPTVSeriesLog.LogLevel.Normal);
            try
            {
                if (type != g_Player.MediaType.Video) return;
                if (m_currentEpisode != null && filename == m_currentEpisode[DBEpisode.cFilename])
                {
                    AddFileToDatabase();

                    int idFile = VideoDatabase.GetFileId(filename);
                    if (idFile != -1)
                    {
                        int movieDuration = (int)g_Player.Duration;
                        VideoDatabase.SetMovieDuration(idFile, movieDuration);
                    }
                }
            }
            catch (Exception e)
            {
                MPTVSeriesLog.Write("TVSeriesPlugin.VideoHandler.OnPlayBackStarted()\r\n" + e.ToString());
            }
        }
    }
}