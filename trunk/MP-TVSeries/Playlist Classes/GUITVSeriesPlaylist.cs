#region Copyright (C) 2005-2009 Team MediaPortal

/* 
 *	Copyright (C) 2005-2009 Team MediaPortal
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MediaPortal.Configuration;
using MediaPortal.GUI.Library;
using MediaPortal.GUI.Video;
using MediaPortal.Player;
using MediaPortal.Playlists;
using MediaPortal.Util;
using MediaPortal.Dialogs;
using MediaPortal.Video.Database;
using Action = MediaPortal.GUI.Library.Action;

namespace WindowPlugins.GUITVSeries
{
    public class GUITVSeriesPlayList : GUIWindow
    {
        #region variables

        private DirectoryHistory m_history = new DirectoryHistory();
        private string currentFolder = string.Empty;
        private int currentSelectedItem = -1;
        private int previousControlId = 0;
        private int m_nTempPlayListWindow = 0;
        private string m_strTempPlayListDirectory = string.Empty;
        private VirtualDirectory m_directory = new VirtualDirectory();
        PlayListPlayer playlistPlayer;
        private View currentView = View.PlayList;
        const int windowID = 9813;
        private String m_sFormatEpisodeTitle = String.Empty;
        private String m_sFormatEpisodeSubtitle = String.Empty;
        private String m_sFormatEpisodeMain = String.Empty;

        #endregion

        #region skin variables
        [SkinControl(2)]  protected GUIButtonControl btnViewAs = null;
        [SkinControl(9)]  protected GUIButtonControl btnLoad = null;        
        [SkinControl(20)] protected GUIButtonControl btnShuffle = null;
        [SkinControl(21)] protected GUIButtonControl btnSave = null;
        [SkinControl(22)] protected GUIButtonControl btnClear = null;
        [SkinControl(23)] protected GUIButtonControl btnPlay = null;
        [SkinControl(24)] protected GUIButtonControl btnNext = null;
        [SkinControl(25)] protected GUIButtonControl btnPrevious = null;
        [SkinControl(30)] protected GUIToggleButtonControl btnRepeat = null;
        [SkinControl(40)] protected GUIToggleButtonControl btnAutoPlay = null;
        [SkinControl(50)] protected GUIFacadeControl m_Facade = null;
        #endregion

        public enum View
        {
            List = 0,
            Icons = 1,
            LargeIcons = 2,
            FilmStrip = 3,
            AlbumView = 4,
            PlayList = 5
        }

        enum guiProperty
        {
            Title,
            Subtitle,
            Description,            
            EpisodeImage,
            SeriesBanner,
            SeasonBanner,
            Logos,            
        }

        public GUITVSeriesPlayList()
        {
            GetID = (int) Window.WINDOW_VIDEO_PLAYLIST;
            playlistPlayer = PlayListPlayer.SingletonPlayer;
            m_directory.AddDrives();
            m_directory.SetExtensions(Utils.VideoExtensions);
            m_directory.AddExtension(".tvsplaylist");
        }

        public static int GetWindowID
        { 
            get { return windowID; } 
        }

        public override int GetID
        { 
            get { return windowID; } 
        }

        public int GetWindowId()
        { 
            return windowID; 
        }		

        protected View CurrentView
        {
            get { return currentView; }
            set { currentView = value; }
        }

        #region BaseWindow Members

		/// <summary>
		/// MediaPortal will set #currentmodule with GetModuleName()
		/// </summary>
		/// <returns>Localized Window Name</returns>
		//public override string GetModuleName() {
		//	return GUILocalizeStrings.Get(136);
		//}

        public override bool Init() {
            currentFolder = Directory.GetCurrentDirectory();

            string xmlSkin = GUIGraphicsContext.Skin + @"\TVSeries.Playlist.xml";
            MPTVSeriesLog.Write("Loading main skin window: " + xmlSkin);            

            return Load(xmlSkin);
        }

        public override void OnAction(Action action)
        {
            switch (action.wID)
            {
                case Action.ActionType.ACTION_SHOW_PLAYLIST:
                    GUIWindowManager.ShowPreviousWindow();
                    return;
                case Action.ActionType.ACTION_MOVE_SELECTED_ITEM_UP:
                    MovePlayListItemUp();
                    return;
                case Action.ActionType.ACTION_MOVE_SELECTED_ITEM_DOWN:
                    MovePlayListItemDown();
                    return;
                case Action.ActionType.ACTION_DELETE_SELECTED_ITEM:
                    DeletePlayListItem();
                    return;
                // Handle case where playlist has been stopped and we receive a player action.
                // This allows us to restart the playback proccess...
                case Action.ActionType.ACTION_MUSIC_PLAY:
                case Action.ActionType.ACTION_NEXT_ITEM:
                case Action.ActionType.ACTION_PAUSE:
                case Action.ActionType.ACTION_PREV_ITEM:
                    if (playlistPlayer.CurrentPlaylistType != PlayListType.PLAYLIST_TVSERIES)
                    {
                        playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_TVSERIES;
                        if (g_Player.CurrentFile == "")
                        {
                            PlayList playList = playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_TVSERIES);
                            if (playList != null && playList.Count > 0)
                            {
                                playlistPlayer.Play(0);
                                UpdateButtonStates();
                            }
                        }
                    }
                    break;
            }
            base.OnAction(action);
        }

        protected override void OnPageLoad()
        {
            base.OnPageLoad();
            if (m_Facade != null)
            {
                m_Facade.CurrentLayout = (GUIFacadeControl.Layout)CurrentView;
            }

            MediaPortal.GUI.Library.GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(136));

            // Episode Formatting
            m_sFormatEpisodeTitle = DBOption.GetOptions(DBOption.cView_Episode_Title);
            m_sFormatEpisodeSubtitle = DBOption.GetOptions(DBOption.cView_Episode_Subtitle);
            m_sFormatEpisodeMain = DBOption.GetOptions(DBOption.cView_Episode_Main);
            
            Helper.disableNativeAutoplay();

            // Clear GUI Properties
            ClearGUIProperties();

            LoadDirectory(string.Empty);
            if (g_Player.Playing && playlistPlayer.CurrentPlaylistType == PlayListType.PLAYLIST_TVSERIES)
            {
                int iCurrentItem = playlistPlayer.CurrentItem;
                if (iCurrentItem >= 0 && iCurrentItem <= m_Facade.Count)
                {
                    GUIControl.SelectItemControl(GetID, m_Facade.GetID, iCurrentItem);
                }
            }

            // Prompt to load a Playlist if there is no items in current plalist
            if (m_Facade.Count <= 0 && btnLoad != null)
            {
                GUIControl.FocusControl(GetID, btnLoad.GetID);
            }

            if (m_Facade.Count > 0)
            {
                GUIControl.FocusControl(GetID, m_Facade.GetID);
                SelectCurrentItem();
            }

            playlistPlayer.RepeatPlaylist = DBOption.GetOptions(DBOption.cRepeatPlaylist);
            if (btnRepeat != null)
            {
                btnRepeat.Selected = playlistPlayer.RepeatPlaylist;
            }

            playlistPlayer.PlaylistAutoPlay = DBOption.GetOptions(DBOption.cPlaylistAutoPlay);
            if (btnAutoPlay != null)
            {
                btnAutoPlay.Selected = playlistPlayer.PlaylistAutoPlay;
                btnAutoPlay.Label = Translation.ButtonAutoPlay;
            }
            
        }

        protected override void OnPageDestroy(int newWindowId)
        {
            currentSelectedItem = m_Facade.SelectedListItemIndex;
            DBOption.SetOptions(DBOption.cRepeatPlaylist, playlistPlayer.RepeatPlaylist);
            DBOption.SetOptions(DBOption.cPlaylistAutoPlay, playlistPlayer.PlaylistAutoPlay);
            prevSelectedEpisode = null;
            Helper.enableNativeAutoplay();
            base.OnPageDestroy(newWindowId);
        }

        protected override void OnClicked(int controlId, GUIControl control, Action.ActionType actionType)
        {
            if (control == btnViewAs)
            {
                bool shouldContinue = false;
                do
                {
                    shouldContinue = false;
                    switch (CurrentView)
                    {
                        case View.List:
                            CurrentView = View.PlayList;
                            if (!AllowView(CurrentView) || m_Facade.PlayListLayout == null)
                            {
                                shouldContinue = true;
                            }
                            else
                            {
                                m_Facade.CurrentLayout = GUIFacadeControl.Layout.Playlist;
                            }
                            break;

                        case View.PlayList:
                            CurrentView = View.Icons;
                            if (!AllowView(CurrentView) || m_Facade.ThumbnailLayout == null)
                            {
                                shouldContinue = true;
                            }
                            else
                            {
                                m_Facade.CurrentLayout = GUIFacadeControl.Layout.SmallIcons;
                            }
                            break;

                        case View.Icons:
                            CurrentView = View.LargeIcons;
                            if (!AllowView(CurrentView) || m_Facade.ThumbnailLayout == null)
                            {
                                shouldContinue = true;
                            }
                            else
                            {
                                m_Facade.CurrentLayout = GUIFacadeControl.Layout.LargeIcons;
                            }
                            break;

                        case View.LargeIcons:
                            CurrentView = View.FilmStrip;
                            if (!AllowView(CurrentView) || m_Facade.FilmstripLayout == null)
                            {
                                shouldContinue = true;
                            }
                            else
                            {
                                m_Facade.CurrentLayout = GUIFacadeControl.Layout.Filmstrip;
                            }
                            break;

                        case View.FilmStrip:
                            CurrentView = View.List;
                            if (!AllowView(CurrentView) || m_Facade.ListLayout == null)
                            {
                                shouldContinue = true;
                            }
                            else
                            {
                                m_Facade.CurrentLayout = GUIFacadeControl.Layout.List;
                            }
                            break;
                    }
                } while (shouldContinue);
                SelectCurrentItem();
                GUIControl.FocusControl(GetID, controlId);
            }
            else if (control == btnShuffle)
            {
                OnShufflePlayList();
            }
            else if (control == btnSave)
            {
                OnSavePlayList();
            }
            else if (control == btnClear)
            {
                OnClearPlayList();
            }
            else if (control == btnPlay || control == this.m_Facade)
            {
                if (control == this.m_Facade && actionType != Action.ActionType.ACTION_SELECT_ITEM) return; // some other events raised onClicked too for some reason?

                playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_TVSERIES;
                playlistPlayer.Reset();
                playlistPlayer.Play(m_Facade.SelectedListItemIndex);
                UpdateButtonStates();
            }
            else if (control == btnNext)
            {
                playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_TVSERIES;
                playlistPlayer.PlayNext();
            }
            else if (control == btnPrevious)
            {
                playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_TVSERIES;
                playlistPlayer.PlayPrevious();
            }
            else if ((btnRepeat != null) && (control == btnRepeat))
            {
                playlistPlayer.RepeatPlaylist = btnRepeat.Selected;
            }
            else if (control == btnLoad)
            {
                OnShowSavedPlaylists(DBOption.GetOptions(DBOption.cPlaylistPath));
            }
            else if (control == btnAutoPlay)
            {
                playlistPlayer.PlaylistAutoPlay = btnAutoPlay.Selected;
            }
            base.OnClicked(controlId, control, actionType);
        }

        public override bool OnMessage(GUIMessage message)
        {
            switch (message.Message)
            {
                case GUIMessage.MessageType.GUI_MSG_PLAYBACK_STOPPED:
                    {
                        for (int i = 0; i < m_Facade.Count; ++i)
                        {
                            GUIListItem item = m_Facade[i];
                            if (item != null && item.Selected)
                            {
                                item.Selected = false;
                                break;
                            }
                        }
                        UpdateButtonStates();
                    }
                    break;

                case GUIMessage.MessageType.GUI_MSG_PLAYLIST_CHANGED:
                    {
                        // global playlist changed outside playlist window
                        LoadDirectory(string.Empty);

                        if (previousControlId == m_Facade.GetID && m_Facade.Count <= 0)
                        {
                            previousControlId = btnViewAs.GetID;
                            GUIControl.FocusControl(GetID, previousControlId);
                        }
                        SelectCurrentVideo();
                    }
                    break;
            }
            return base.OnMessage(message);
        }

        #endregion

        protected bool AllowView(View view)
        {
            if (view == View.List)
                return false;

            return true;
        }

        protected void SelectCurrentItem()
        {
            int iItem = m_Facade.SelectedListItemIndex;
            if (iItem > -1)
            {
                GUIControl.SelectItemControl(GetID, m_Facade.GetID, iItem);
            }
            UpdateButtonStates();
        }

        protected void UpdateButtonStates()
        {
            string strLine = string.Empty;
            View view = CurrentView;
            switch (view)
            {
                case View.List:
                    strLine = GUILocalizeStrings.Get(101);
                    break;
                case View.Icons:
                    strLine = GUILocalizeStrings.Get(100);
                    break;
                case View.LargeIcons:
                    strLine = GUILocalizeStrings.Get(417);
                    break;
                case View.FilmStrip:
                    strLine = GUILocalizeStrings.Get(733);
                    break;
                case View.PlayList:
                    strLine = GUILocalizeStrings.Get(101);
                    break;
            }
            if (btnViewAs != null)
                GUIControl.SetControlLabel(GetID, btnViewAs.GetID, strLine);

            if (m_Facade.Count > 0)
            {
                if (btnClear != null) btnClear.Disabled = false;
                if (btnPlay != null) btnPlay.Disabled = false;
                if (btnSave != null) btnSave.Disabled = false;
                if (btnShuffle != null) btnShuffle.Disabled = false;

                if (g_Player.Playing && playlistPlayer.CurrentPlaylistType == PlayListType.PLAYLIST_TVSERIES)
                {
                    if (btnNext != null) btnNext.Disabled = false;
                    if (btnPrevious != null) btnPrevious.Disabled = false;
                }
                else
                {
                    if (btnNext != null) btnNext.Disabled = true;
                    if (btnPrevious != null) btnPrevious.Disabled = true;
                }
            }
            else
            {
                if (btnClear != null) btnClear.Disabled = true;
                if (btnPlay != null) btnPlay.Disabled = true;
                if (btnNext != null) btnNext.Disabled = true;
                if (btnPrevious != null) btnPrevious.Disabled = true;
                if (btnSave != null) btnSave.Disabled = true;
                if (btnShuffle != null) btnShuffle.Disabled = true;
            }
        }

        protected void LoadDirectory(string strNewDirectory)
        {
            if (m_Facade == null)
                return;

            GUIWaitCursor.Show();
            try
            {
                GUIListItem SelectedItem = m_Facade.SelectedListItem;
                if (SelectedItem != null)
                {
                    if (SelectedItem.IsFolder && SelectedItem.Label != "..")
                    {
                        m_history.Set(SelectedItem.Label, currentFolder);
                    }
                }
                currentFolder = strNewDirectory;
                m_Facade.Clear();

                string strObjects = string.Empty;

                ArrayList itemlist = new ArrayList();

                PlayList playlist = playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_TVSERIES);
                /* copy playlist from general playlist*/
                int iCurrentItem = -1;
                if (playlistPlayer.CurrentPlaylistType == PlayListType.PLAYLIST_TVSERIES)
                {
                    iCurrentItem = playlistPlayer.CurrentItem;
                }

                string strFileName;
                for (int i = 0; i < playlist.Count; ++i)
                {
                    PlayListItem item = playlist[i];
                    strFileName = item.FileName;

                    GUIListItem pItem = new GUIListItem(item.EpisodeName);
                    pItem.Path = strFileName;
                    pItem.IsFolder = false;
                    pItem.TVTag = item.Episode;
                    
                    // update images
                    pItem.ThumbnailImage = item.EpisodeThumb;
                    //pItem.IconImageBig = item.EpisodeThumb;
                    //pItem.IconImage = item.EpisodeThumb;                    

                    if (item.IsWatched)
                    {
                        pItem.IsPlayed = true; // facade colours...dont seem to work!
                        pItem.IconImage = GUIGraphicsContext.Skin + @"\Media\tvseries_Watched.png";
                    }
                    else
                    {
                        pItem.IsPlayed = false;
                        pItem.IconImage = GUIGraphicsContext.Skin + @"\Media\tvseries_UnWatched.png";
                    }

                    if (item.Duration > 0)
                    {
                        double nDuration = item.Duration;
                        if (nDuration > 0)
                        {
                            string str = Helper.MSToMMSS(nDuration);
                            pItem.Label2 = str;
                        }
                        else
                        {
                            pItem.Label2 = string.Empty;
                        }
                    }

                    itemlist.Add(pItem);
                    //MediaPortal.Util.Utils.SetDefaultIcons(pItem);
                }

                iCurrentItem = 0;
                strFileName = string.Empty;
                //	Search current playlist item
                if ((m_nTempPlayListWindow == GetID && m_strTempPlayListDirectory.IndexOf(currentFolder) >= 0 && g_Player.Playing)
                    || (GetID == (int)Window.WINDOW_VIDEO_PLAYLIST && playlistPlayer.CurrentPlaylistType == PlayListType.PLAYLIST_TVSERIES
                    && g_Player.Playing))
                {
                    iCurrentItem = playlistPlayer.CurrentItem;
                    if (iCurrentItem >= 0)
                    {
                        playlist = playlistPlayer.GetPlaylist(playlistPlayer.CurrentPlaylistType);
                        if (iCurrentItem < playlist.Count)
                        {
                            PlayListItem item = playlist[iCurrentItem];
                            strFileName = item.FileName;
                        }
                    }
                }

                string strSelectedItem = m_history.Get(currentFolder);
                int iItem = 0;
                foreach (GUIListItem item in itemlist)
                {
                    m_Facade.Add(item);                    
                    item.OnItemSelected += new GUIListItem.ItemSelectedHandler(onFacadeItemSelected);

                    //	synchronize playlist with current directory
                    if (strFileName.Length > 0 && item.Path == strFileName)
                    {
                        item.Selected = true;
                    }
                }
                for (int i = 0; i < m_Facade.Count; ++i)
                {
                    GUIListItem item = m_Facade[i];
                    if (item.Label == strSelectedItem)
                    {
                        GUIControl.SelectItemControl(GetID, m_Facade.GetID, iItem);
                        break;
                    }
                    iItem++;
                }

                //set object count label
                int iTotalItems = itemlist.Count;
                GUIPropertyManager.SetProperty("#itemcount", Translation.Episodes + ": " + iTotalItems.ToString());
                GUIPropertyManager.SetProperty("#TVSeries.Playlist.Count", iTotalItems.ToString());

                if (currentSelectedItem >= 0)
                {
                    GUIControl.SelectItemControl(GetID, m_Facade.GetID, currentSelectedItem);                    
                }
                UpdateButtonStates();
                GUIWaitCursor.Hide();
            }
            catch (Exception ex)
            {
                GUIWaitCursor.Hide();
                MPTVSeriesLog.Write(string.Format("GUITVSeriesPlaylist: An error occured while loading the directory - {0}", ex.Message));
            }
        }

        private void ClearFileItems()
        {
            GUIControl.ClearControl(GetID, m_Facade.GetID);
        }

        private void OnClearPlayList()
        {
            currentSelectedItem = -1;
            ClearFileItems();
            playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_TVSERIES).Clear();
            if (playlistPlayer.CurrentPlaylistType == PlayListType.PLAYLIST_TVSERIES)
            {
                playlistPlayer.Reset();
            }
            LoadDirectory(string.Empty);
            UpdateButtonStates();
            ClearGUIProperties();
            if (btnLoad != null)
                GUIControl.FocusControl(GetID, btnLoad.GetID);
        }

        protected void OnClick(int itemIndex)
        {
            currentSelectedItem = m_Facade.SelectedListItemIndex;
            GUIListItem item = m_Facade.SelectedListItem;
            if (item == null)
            {
                return;
            }
            if (item.IsFolder)
            {
                return;
            }

            playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_TVSERIES;
            playlistPlayer.Reset();
            playlistPlayer.Play(itemIndex);
        }

        // triggered when a selection change was made on the facade
        DBEpisode prevSelectedEpisode = null;
        private void onFacadeItemSelected(GUIListItem item, GUIControl parent)
        {
            // if this is not a message from the facade, exit
            if (parent != m_Facade && parent != m_Facade.FilmstripLayout &&
                parent != m_Facade.ThumbnailLayout && parent != m_Facade.ListLayout && 
                parent != m_Facade.PlayListLayout)
                return;
            
            if (item == null || item.TVTag == null)
                return;

            DBEpisode episode = item.TVTag as DBEpisode;
            if (episode == null || prevSelectedEpisode == episode) 
                return;

            if (item.IsPlayed) episode[DBOnlineEpisode.cWatched] = true;

            // Push properties to skin            
            TVSeriesPlugin.setGUIProperty(guiProperty.Title.ToString(), FieldGetter.resolveDynString(m_sFormatEpisodeTitle, episode));
            TVSeriesPlugin.setGUIProperty(guiProperty.Subtitle.ToString(), FieldGetter.resolveDynString(m_sFormatEpisodeSubtitle, episode));            
            TVSeriesPlugin.setGUIProperty(guiProperty.Description.ToString(), FieldGetter.resolveDynString(m_sFormatEpisodeMain, episode));
            TVSeriesPlugin.setGUIProperty(guiProperty.Logos.ToString(), localLogos.getLogos(ref episode, TVSeriesPlugin.logosHeight, TVSeriesPlugin.logosWidth));
            TVSeriesPlugin.setGUIProperty(guiProperty.EpisodeImage.ToString(), ImageAllocator.GetEpisodeImage(episode));
            GUIPropertyManager.SetProperty("#selectedthumb", ImageAllocator.GetEpisodeImage(episode));           
            
            TVSeriesPlugin.pushFieldsToSkin(episode, "Episode");
            
            // Some strange issues with logos when using mouse and hovering over current item
            // Dont push properties next time if the same episode is selected
            prevSelectedEpisode = episode;

        } 

        private void ClearGUIProperties()
        {
            TVSeriesPlugin.clearGUIProperty(guiProperty.Title.ToString());
            TVSeriesPlugin.clearGUIProperty(guiProperty.Subtitle.ToString());
            TVSeriesPlugin.clearGUIProperty(guiProperty.Description.ToString());
            TVSeriesPlugin.clearGUIProperty(guiProperty.Logos.ToString());
            TVSeriesPlugin.clearGUIProperty(guiProperty.EpisodeImage.ToString());
            TVSeriesPlugin.clearFieldsForskin("Episode");
        }

        protected void OnQueueItem(int itemIndex)
        {
            RemovePlayListItem(itemIndex);
        }

        private void RemovePlayListItem(int itemIndex)
        {
            GUIListItem listItem = m_Facade[itemIndex];
            if (listItem == null)
            {
                return;
            }
            string itemFileName = listItem.Path;

            playlistPlayer.Remove(PlayListType.PLAYLIST_TVSERIES, itemFileName);

            LoadDirectory(currentFolder);
            UpdateButtonStates();
            GUIControl.SelectItemControl(GetID, m_Facade.GetID, itemIndex);
            SelectCurrentVideo();
        }

        private void OnShufflePlayList()
        {
            currentSelectedItem = m_Facade.SelectedListItemIndex;
            ClearFileItems();
            PlayList playlist = playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_TVSERIES);

            if (playlist.Count <= 0)
            {
                return;
            }
            string currentItemFileName = string.Empty;
            if (playlistPlayer.CurrentItem >= 0)
            {
                if (g_Player.Playing && playlistPlayer.CurrentPlaylistType == PlayListType.PLAYLIST_TVSERIES)
                {
                    PlayListItem item = playlist[playlistPlayer.CurrentItem];
                    currentItemFileName = item.FileName;
                }
            }
            playlist.Shuffle();
            if (playlistPlayer.CurrentPlaylistType == PlayListType.PLAYLIST_TVSERIES)
            {
                playlistPlayer.Reset();
            }

            if (currentItemFileName.Length > 0)
            {
                for (int i = 0; i < playlist.Count; i++)
                {
                    PlayListItem playListItem = playlist[i];
                    if (playListItem.FileName == currentItemFileName)
                    {
                        playlistPlayer.CurrentItem = i;
                    }
                }
            }

            LoadDirectory(currentFolder);
        }

        protected void SwitchView()
        {
            if (m_Facade == null)
            {
                return;
            }
            switch (CurrentView)
            {
                case View.List:
                    m_Facade.CurrentLayout = GUIFacadeControl.Layout.List;
                    break;
                case View.Icons:
                    m_Facade.CurrentLayout = GUIFacadeControl.Layout.SmallIcons;
                    break;
                case View.LargeIcons:
                    m_Facade.CurrentLayout = GUIFacadeControl.Layout.LargeIcons;
                    break;
                case View.FilmStrip:
                    m_Facade.CurrentLayout = GUIFacadeControl.Layout.Filmstrip;
                    break;
                case View.PlayList:
                    m_Facade.CurrentLayout = GUIFacadeControl.Layout.Playlist;
                    break;
            }
        }

        protected bool GetKeyboard(ref string strLine)
        {
            try
            {
                VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)Window.WINDOW_VIRTUAL_KEYBOARD);
                if (null == keyboard)
                {
                    return false;
                }
                keyboard.Reset();
                keyboard.Text = strLine;                
                keyboard.DoModal(GetID);
                if (keyboard.IsConfirmed)
                {
                    strLine = keyboard.Text;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write(string.Format("Virtual Keyboard error: {0}, stack: {1}",ex.Message, ex.StackTrace));
                return false;
            }
        }

        private void OnSavePlayList()
        {
            currentSelectedItem = m_Facade.SelectedListItemIndex;
            string playlistFileName = string.Empty;
            if (GetKeyboard(ref playlistFileName))
            {
                string playListPath = string.Empty;              
                playListPath = DBOption.GetOptions(DBOption.cPlaylistPath);
                playListPath = MediaPortal.Util.Utils.RemoveTrailingSlash(playListPath);
				
				// check if Playlist folder exists, create it if not
				if (!Directory.Exists(playListPath)){
					try {
						Directory.CreateDirectory(playListPath);						
					}
					catch (Exception e){
						MPTVSeriesLog.Write("Error: Unable to create Playlist path: " + e.Message);
						return;
					}
				}
				
                string fullPlayListPath = Path.GetFileNameWithoutExtension(playlistFileName);

                fullPlayListPath += ".tvsplaylist";
                if (playListPath.Length != 0)
                {
                  fullPlayListPath = playListPath + @"\" + fullPlayListPath;
                }
                PlayList playlist = new PlayList();
                for (int i = 0; i < m_Facade.Count; ++i)
                {
                    GUIListItem listItem = m_Facade[i];
                    PlayListItem playListItem = new PlayListItem();                                       
                    playListItem.Episode = listItem.TVTag as DBEpisode;
                    playlist.Add(playListItem);
                }
                PlayListIO saver = new PlayListIO();
                saver.Save(playlist, fullPlayListPath);
            }
        }
        
        protected void OnShowSavedPlaylists(string _directory)
        {
            VirtualDirectory _virtualDirectory = new VirtualDirectory();
            _virtualDirectory.AddExtension(".tvsplaylist");            

            List<GUIListItem> itemlist = _virtualDirectory.GetDirectoryExt(_directory);
            if (_directory == DBOption.GetOptions(DBOption.cPlaylistPath))
                itemlist.RemoveAt(0);

			// If no playlists found, show a Message to user and then exit
			if (itemlist.Count == 0) {
				GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
				dlgOK.SetHeading(983);
				dlgOK.SetLine(1, Translation.NoPlaylistsFound);
				dlgOK.SetLine(2, _directory);
				dlgOK.DoModal(GUIWindowManager.ActiveWindow);
				return;
			}

            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null)
                return;
            dlg.Reset();
            dlg.SetHeading(983); // Saved Playlists

            foreach (GUIListItem item in itemlist)
            {
                MediaPortal.Util.Utils.SetDefaultIcons(item);
                dlg.Add(item);
            }

            dlg.DoModal(GetID);

            if (dlg.SelectedLabel == -1)
                return;

            GUIListItem selectItem = itemlist[dlg.SelectedLabel];
            if (selectItem.IsFolder)
            {
                OnShowSavedPlaylists(selectItem.Path);
                return;
            }

            GUIWaitCursor.Show();
            LoadPlayList(selectItem.Path);
            GUIWaitCursor.Hide();
        }

        protected void LoadPlayList(string strPlayList)
        {
            IPlayListIO loader = PlayListFactory.CreateIO(strPlayList);
            if (loader == null)
                return;
            PlayList playlist = new PlayList();

            if (!loader.Load(playlist, strPlayList))
            {
                TellUserSomethingWentWrong();
                return;
            }

            playlistPlayer.CurrentPlaylistName = System.IO.Path.GetFileNameWithoutExtension(strPlayList);
            if (playlist.Count == 1 && playlistPlayer.PlaylistAutoPlay)
            {
                MPTVSeriesLog.Write(string.Format("GUITVSeriesPlaylist: play single playlist item - {0}", playlist[0].FileName));

                // If the file is an image file, it should be mounted before playing
                string filename = playlist[0].FileName;
                if (Helper.IsImageFile(filename)) {
                    if (!GUIVideoFiles.MountImageFile(GUIWindowManager.ActiveWindow, filename)) {
                        return;
                    }
                }

                if (g_Player.Play(filename))
                {
                    if (MediaPortal.Util.Utils.IsVideo(filename))
                    {
                        g_Player.ShowFullScreenWindow();
                    }
                }
                return;
            }

            // clear current playlist
            playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_TVSERIES).Clear();

            // add each item of the playlist to the playlistplayer
            for (int i = 0; i < playlist.Count; ++i)
            {
                PlayListItem playListItem = playlist[i];
                playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_TVSERIES).Add(playListItem);
            }

            // if we got a playlist
            if (playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_TVSERIES).Count > 0)
            {				
                playlist = playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_TVSERIES);

				// autoshuffle on load
				if (playlistPlayer.PlaylistAutoShuffle) {
					playlist.Shuffle();
				}

				// then get 1st item
                PlayListItem item = playlist[0];

                // and start playing it
                if (playlistPlayer.PlaylistAutoPlay)
                {
                    playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_TVSERIES;
                    playlistPlayer.Reset();
                    playlistPlayer.Play(0);
                }

                // and activate the playlist window if its not activated yet
                if (GetID == GUIWindowManager.ActiveWindow)
                {
                    GUIWindowManager.ActivateWindow(GetID);
                }                
            }
        }

        private void TellUserSomethingWentWrong()
        {
            GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            if (dlgOK != null)
            {
                dlgOK.SetHeading(6);
                dlgOK.SetLine(1, 477);
                dlgOK.SetLine(2, string.Empty);
                dlgOK.DoModal(GetID);
            }
        }

        private void SelectCurrentVideo()
        {
            if (g_Player.Playing && playlistPlayer.CurrentPlaylistType == PlayListType.PLAYLIST_TVSERIES)
            {
                int currentItemIndex = playlistPlayer.CurrentItem;
                if (currentItemIndex >= 0 && currentItemIndex <= m_Facade.Count)
                {
                    GUIControl.SelectItemControl(GetID, m_Facade.GetID, currentItemIndex);
                }
            }
        }

        private void MovePlayListItemUp()
        {
            if (playlistPlayer.CurrentPlaylistType == PlayListType.PLAYLIST_NONE)
            {
                playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_TVSERIES;
            }

            if (playlistPlayer.CurrentPlaylistType != PlayListType.PLAYLIST_TVSERIES
                || m_Facade.CurrentLayout != GUIFacadeControl.Layout.Playlist
                || m_Facade.PlayListLayout == null)
            {
                return;
            }

            int iItem = m_Facade.SelectedListItemIndex;

            // Prevent moving backwards past the top item in the list

            PlayList playList = playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_TVSERIES);
            playList.MovePlayListItemUp(iItem);
            int selectedIndex = m_Facade.MoveItemUp(iItem, true);

            if (iItem == playlistPlayer.CurrentItem)
            {
                playlistPlayer.CurrentItem = selectedIndex;
            }

            m_Facade.SelectedListItemIndex = selectedIndex;
            UpdateButtonStates();
        }

        private void MovePlayListItemDown()
        {
            if (playlistPlayer.CurrentPlaylistType == PlayListType.PLAYLIST_NONE)
            {
                playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_TVSERIES;
            }

            if (playlistPlayer.CurrentPlaylistType != PlayListType.PLAYLIST_TVSERIES
                || m_Facade.CurrentLayout != GUIFacadeControl.Layout.Playlist
                || m_Facade.PlayListLayout == null)
            {
                return;
            }

            int iItem = m_Facade.SelectedListItemIndex;
            PlayList playList = playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_TVSERIES);

            // Prevent moving fowards past the last item in the list
            // as this would cause the currently playing item to scroll
            // off of the list view...

            playList.MovePlayListItemDown(iItem);
            int selectedIndex = m_Facade.MoveItemDown(iItem, true);

            if (iItem == playlistPlayer.CurrentItem)
            {
                playlistPlayer.CurrentItem = selectedIndex;
            }

            m_Facade.SelectedListItemIndex = selectedIndex;

            UpdateButtonStates();
        }

        private void DeletePlayListItem()
        {
            if (playlistPlayer.CurrentPlaylistType == PlayListType.PLAYLIST_NONE)
            {
                playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_TVSERIES;
            }

            if (playlistPlayer.CurrentPlaylistType != PlayListType.PLAYLIST_TVSERIES
                || m_Facade.CurrentLayout != GUIFacadeControl.Layout.Playlist
                || m_Facade.PlayListLayout == null)
            {
                return;
            }

            int iItem = m_Facade.SelectedListItemIndex;

            string currentFile = g_Player.CurrentFile;
            GUIListItem item = m_Facade[iItem];         

            RemovePlayListItem(iItem);

            if (m_Facade.Count == 0)
            {
                g_Player.Stop();
                ClearGUIProperties();
                if (btnLoad != null)
                    GUIControl.FocusControl(GetID, btnLoad.GetID); 
            }
            else
            {
                m_Facade.PlayListLayout.SelectedListItemIndex = iItem;
            }

            UpdateButtonStates();
        }
    }
}