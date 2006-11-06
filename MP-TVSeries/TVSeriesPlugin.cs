using System;
using System.Windows.Forms;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortal.Dialogs;
using MediaPortal.Util;
using MediaPortal.Playlists;
using WindowPlugins.GUITVSeries;

namespace MediaPortal.GUI.Video
{
    public class TVSeriesPlugin : GUIWindow, ISetupForm
    {
        public TVSeriesPlugin()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        #region ISetupForm Members

        // Returns the name of the plugin which is shown in the plugin menu
        public string PluginName()
        {
            return "MP-TV Series";
        }

        // Returns the description of the plugin is shown in the plugin menu
        public string Description()
        {
            return "Plugin used to manage and play television series";
        }

        // Returns the author of the plugin which is shown in the plugin menu
        public string Author()
        {
            return "Zeflash, based on the work of WeeToddDid (Luc Theriault)";
        }

        // show the setup dialog
        public void ShowPlugin()
        {
            ConfigurationForm dialog = new ConfigurationForm();
            dialog.ShowDialog();
        }

        // Indicates whether plugin can be enabled/disabled
        public bool CanEnable()
        {
            return true;
        }

        // get ID of windowplugin belonging to this setup
        public int GetWindowId()
        {
            return 9811;
        }

        // Indicates if plugin is enabled by default;
        public bool DefaultEnabled()
        {
            return true;
        }

        // indicates if a plugin has its own setup screen
        public bool HasSetup()
        {
            return true;
        }

        /// <summary>
        /// If the plugin should have its own button on the main menu of Media Portal then it
        /// should return true to this method, otherwise if it should not be on home
        /// it should return false
        /// </summary>
        /// <param name="strButtonText">text the button should have</param>
        /// <param name="strButtonImage">image for the button, or empty for default</param>
        /// <param name="strButtonImageFocus">image for the button, or empty for default</param>
        /// <param name="strPictureImage">subpicture for the button or empty for none</param>
        /// <returns>true  : plugin needs its own button on home
        ///          false : plugin does not need its own button on home</returns>
        public bool GetHome(out string strButtonText, out string strButtonImage, out string strButtonImageFocus, out string strPictureImage)
        {
            strButtonText = "My TV Series";
            strButtonImage = String.Empty;
            strButtonImageFocus = String.Empty;
            strPictureImage = String.Empty;
            return true;
        }

        #endregion

        private String m_ListLevel = "Series";
        private DBSeries m_SelectedSeries;
        private DBSeason m_SelectedSeason;
        private DBEpisode m_SelectedEpisode;
        public LogWriter m_Logs = new LogWriter();
        private VideoHandler m_VideoHandler;

        #region Skin Variables
        [SkinControlAttribute(2)]
        protected GUIButtonControl m_Button_Back = null;

        [SkinControlAttribute(3)]
        protected GUIButtonControl m_Button_View = null;

        [SkinControlAttribute(50)]
        protected GUIFacadeControl m_Facade = null;

        [SkinControlAttribute(30)]
        protected GUIImage m_Image = null;

        [SkinControlAttribute(31)]
        protected GUITextScrollUpControl m_Description = null;

        [SkinControlAttribute(32)]
        protected GUITextControl m_Series_Name = null;

        [SkinControlAttribute(33)]
        protected GUITextControl m_Genre = null;

        [SkinControlAttribute(34)]
        protected GUITextControl m_Series_Network = null;

        [SkinControlAttribute(35)]
        protected GUITextControl m_Series_Duration = null;

        [SkinControlAttribute(36)]
        protected GUITextControl m_Series_Status = null;

        [SkinControlAttribute(37)]
        protected GUITextControl m_Series_Premiered = null;

        [SkinControlAttribute(40)]
        protected GUITextControl m_Title = null;

        [SkinControlAttribute(41)]
        protected GUITextControl m_Airs = null;

        [SkinControlAttribute(42)]
        protected GUITextControl m_Episode_SeasonNumber = null;

        [SkinControlAttribute(43)]
        protected GUITextControl m_Episode_EpisodeNumber = null;

        [SkinControlAttribute(44)]
        protected GUITextControl m_Episode_Filename = null;

        [SkinControlAttribute(45)]
        protected GUITextControl m_Episode_Actors = null;
        #endregion

        public override int GetID
        {
            get
            {
                return 9811;
            }
        }

        public override bool Init()
        {
            this.m_Logs.Write("**** Plugin started in MediaPortal ***");
            String xmlSkin = GUIGraphicsContext.Skin + @"\TVSeries.xml";
            this.m_Logs.Write("Loading XML Skin: " + xmlSkin);

            m_VideoHandler = new VideoHandler();

            return Load(xmlSkin);
        }

        void LoadFacade()
        {
            this.m_Facade.Clear();
            switch (this.m_ListLevel)
            {
                case "Series":
                    int selectedIndex = 0;
                    int count = 0;
                    DBSeries.Get();
                    foreach (DBSeries series in DBSeries.Get())
                    {
                        try
                        {
                            GUIListItem item = new GUIListItem(series[DBSeries.cPrettyName]);
//                            item.Label2 = series.Airs;
                            item.TVTag = series;
                            String filename = series.Banner;
                            if (filename != String.Empty)
                                item.IconImage = item.IconImageBig = filename;
                            item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(Series_OnItemSelected);
                            if (this.m_SelectedSeries != null && series[DBSeries.cParsedName] == this.m_SelectedSeries[DBSeries.cParsedName])
                                selectedIndex = count;
                            this.m_Facade.Add(item);
                        }
                        catch (Exception ex)
                        {
                            this.m_Logs.Write("The 'LoadFacade' function has generated an error displaying series list item: " + ex.Message);
                        }
                        count++;
                    }
                    this.m_Facade.SelectedListItemIndex = selectedIndex;
                    this.Series_OnItemSelected(this.m_Facade.SelectedListItem, this.m_Facade);
                    break;
                case "Season":
                    selectedIndex = 0;
                    count = 0;
                        foreach (DBSeason season in DBSeason.Get(m_SelectedSeries[DBSeries.cParsedName]))
                        {
                            try
                            {
                                GUIListItem item = new GUIListItem("Season " + season[DBSeason.cIndex]);
                                String filename = season.Banner;
                                if (filename == String.Empty) filename = this.m_SelectedSeries.Banner;
                                if (filename != String.Empty)
                                {
                                    item.IconImage = filename;
                                    item.IconImageBig = filename;
                                }
                                item.TVTag = season;
                                item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(Season_OnItemSelected);

                                if (this.m_SelectedSeason != null && this.m_SelectedSeason[DBSeason.cIndex] == season[DBSeason.cIndex])
                                    selectedIndex = count;
                                this.m_Facade.Add(item);
                            }
                            catch (Exception ex)
                            {
                                this.m_Logs.Write("The 'LoadFacade' function has generated an error displaying season list item: " + ex.Message);
                            }
                            count++;
                        }                    


//                     string[] special_results = this.m_Database.GetSpecialListing(this.m_SelectedSeries.Index, true);
//                     if (special_results != null)
//                     {
//                         foreach (string special in special_results)
//                         {
//                             try
//                             {
//                                 GUIListItem item = new GUIListItem(special);
//                                 String filename = this.m_SelectedSeries.GetSpecialImage(special);
// 
//                                 if (filename == String.Empty) filename = this.m_SelectedSeries.GetImage();
//                                 if (filename == String.Empty) filename = this.m_SelectedSeries.GetImageBanner();
//                                 if (filename != String.Empty)
//                                 {
//                                     item.IconImage = filename;
//                                     item.IconImageBig = filename;
//                                 }
// 
//                                 item.TVTag = special;
// 
//                                 if (this.m_SelectedSeason != null && this.m_SelectedSeason.GetType() == typeof(string) && special == (string)this.m_SelectedSeason)
//                                     selectedIndex = count;
// 
//                                 item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(Season_OnItemSelected);
// 
//                                 this.m_Facade.Add(item);
//                             }
//                             catch (Exception ex)
//                             {
//                                 this.m_Logs.Write("The 'LoadFacade' function has generated an error displaying season list item: " + ex.Message);
//                             }
//                             count++;
//                         }
//                     }

                    this.m_Facade.SelectedListItemIndex = selectedIndex;
                    this.Season_OnItemSelected(this.m_Facade.SelectedListItem, this.m_Facade);

                    break;
                case "Episode":
                    selectedIndex = 0;
                    count = 0;
                    foreach (DBEpisode episode in DBEpisode.Get(m_SelectedSeries[DBSeries.cParsedName], m_SelectedSeason[DBSeason.cIndex], true))
                    {
                        try
                        {
                            GUIListItem item = new GUIListItem(episode[DBEpisode.cEpisodeIndex] + ": " + episode[DBEpisode.cEpisodeName]);

//                            item.Label2 = episode.Aired;
                            item.Shaded = episode[DBEpisode.cWatched] != 0;
                            item.Label3 = episode[DBEpisode.cWatched] != 0 ? "" : "*";
                            item.TVTag = episode;

                            if (this.m_SelectedEpisode != null && episode[DBEpisode.cEpisodeIndex] == this.m_SelectedEpisode[DBEpisode.cEpisodeIndex])
                                selectedIndex = count;

                            item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(Episode_OnItemSelected);
                            this.m_Facade.Add(item);
                        }
                        catch (Exception ex)
                        {
                            this.m_Logs.Write("The 'LoadFacade' function has generated an error displaying episode list item: " + ex.Message);
                        }
                        count++;
                    }
                    this.m_Button_Back.Focus = false;
                    this.m_Facade.ListView.ShadedColor = 0x50ff0000;
                    this.m_Facade.Focus = true;
                    this.m_Facade.SelectedListItemIndex = selectedIndex;
                    this.Episode_OnItemSelected(this.m_Facade.SelectedListItem, this.m_Facade);
                    break;
            }
        }

        protected override void OnPageLoad()
        {
            this.LoadFacade();
            this.m_Button_Back.Focus = false;
            this.m_Facade.Focus = true;
        }

        protected override void OnShowContextMenu()
        {
            try
            {
                GUIListItem currentitem = this.m_Facade.SelectedListItem;
                if (currentitem == null) return;

                GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlg == null) return;
                dlg.Reset();
                dlg.SetHeading(924); // menu
                GUIListItem pItem = new GUIListItem("Download Coverart");
                dlg.Add(pItem);
                pItem = new GUIListItem("Refresh Information");
                dlg.Add(pItem);
                pItem = new GUIListItem("Play All");
                dlg.Add(pItem);
                pItem = new GUIListItem("Import new Videos");
                dlg.Add(pItem);
                dlg.DoModal(GetID);
                if (dlg.SelectedId == -1) return;

                GUIDialogOK pDlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                pDlgOK.SetHeading("Feature not Implemented");

                switch (dlg.SelectedId)
                {
                    case 1:
                        pDlgOK.SetLine(1, "This feature will allow you to download");
                        pDlgOK.SetLine(2, "images from Amazon and Google.");
                        pDlgOK.DoModal(GUIWindowManager.ActiveWindow);

                        break;

                    case 2:
                        pDlgOK.SetLine(1, "This feature will allow you to update");
                        pDlgOK.SetLine(2, "the information from tv.com.");
                        pDlgOK.DoModal(GUIWindowManager.ActiveWindow);
                        break;

                    case 3:
                        #region Play All
                        PlayListPlayer playlistPlayer = new PlayListPlayer();
                        playlistPlayer.Reset();
                        playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_VIDEO_TEMP;
                        PlayList playlist = playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_VIDEO_TEMP);
                        playlist.Clear();
                        if (this.m_ListLevel == "Series")
                        {
                            DBSeries series = (DBSeries)currentitem.TVTag;
                            foreach (DBEpisode episode in DBEpisode.Get(series[DBSeries.cParsedName], true))
                            {
                                PlayListItem itemNew = new PlayListItem();
                                itemNew.FileName = episode[DBEpisode.cFilename];
                                itemNew.Description = episode[DBOnlineEpisode.cEpisodeSummary];  // not working
                                itemNew.Type = Playlists.PlayListItem.PlayListItemType.Video;
                                itemNew.MusicTag = episode;
                                playlist.Add(itemNew);
                            }
                        }
                        else if (this.m_ListLevel == "Season")
                        {
                            DBSeason season = (DBSeason)currentitem.TVTag;
                            foreach (DBEpisode episode in DBEpisode.Get(this.m_SelectedSeries[DBSeries.cParsedName], season[DBSeason.cIndex], true))
                            {
                                PlayListItem itemNew = new PlayListItem();
                                itemNew.FileName = episode[DBEpisode.cFilename];
                                itemNew.Description = episode[DBOnlineEpisode.cEpisodeSummary];  // not working
                                itemNew.Type = Playlists.PlayListItem.PlayListItemType.Video;
                                itemNew.MusicTag = episode;
                                playlist.Add(itemNew);
                            }
                        }
                        else if (this.m_ListLevel == "Episode")
                        {
                            DBEpisode episode = (DBEpisode)currentitem.TVTag;
                            PlayListItem itemNew = new PlayListItem();
                            itemNew.FileName = episode[DBEpisode.cFilename];
                            GUIPropertyManager.SetProperty("#plot", episode[DBOnlineEpisode.cEpisodeSummary]);
                            itemNew.Description = episode[DBOnlineEpisode.cEpisodeSummary];  // not working
                            itemNew.Type = Playlists.PlayListItem.PlayListItemType.Video;
                            itemNew.MusicTag = episode;
                            playlist.Add(itemNew);
                        }

                        playlistPlayer.PlayNext();

                        DBEpisode currentPlayingEpisode = (DBEpisode)playlistPlayer.GetCurrentItem().MusicTag;
                        DBSeries currentPlayingSeries = new DBSeries(currentPlayingEpisode[DBEpisode.cSeriesParsedName]);
                        
                        this.m_Logs.Write("Playing Movie: " + currentPlayingSeries[DBSeries.cPrettyName] + " - " + currentPlayingEpisode[DBEpisode.cSeasonIndex] + "x" + currentPlayingEpisode[DBEpisode.cEpisodeIndex] + " - " + currentPlayingEpisode[DBEpisode.cEpisodeName]);
                        
                        GUIPropertyManager.SetProperty("#title", currentPlayingSeries[DBSeries.cPrettyName] + " - " + currentPlayingEpisode[DBEpisode.cSeasonIndex] + "x" + currentPlayingEpisode[DBEpisode.cEpisodeIndex] + " - " + currentPlayingEpisode[DBEpisode.cEpisodeName]);
                        GUIPropertyManager.SetProperty("#genre", currentPlayingSeries[DBSeries.cGenre]);
                        GUIPropertyManager.SetProperty("#file", currentPlayingEpisode[DBEpisode.cFilename]);
//                        GUIPropertyManager.SetProperty("#plot", currentPlayingEpisode.Description);

                        //GUIPropertyManager.SetProperty("#year", currentPlayingEpisode);
                        //GUIPropertyManager.SetProperty("#director", currentPlayingEpisode);
                        //GUIPropertyManager.SetProperty("#cast", currentPlayingEpisode);	
                        //GUIPropertyManager.SetProperty("#dvdlabel", currentPlayingEpisode);
                        //GUIPropertyManager.SetProperty("#imdbnumber", currentPlayingEpisode);
                        GUIPropertyManager.SetProperty("#plotoutline", currentPlayingEpisode[DBOnlineEpisode.cEpisodeSummary]);
                        //GUIPropertyManager.SetProperty("#rating", currentPlayingEpisode);	
                        //GUIPropertyManager.SetProperty("#tagline", currentPlayingEpisode);
                        //GUIPropertyManager.SetProperty("#votes", currentPlayingEpisode);	
                        //GUIPropertyManager.SetProperty("#credits", currentPlayingEpisode);
                        //GUIPropertyManager.SetProperty("#mpaarating", currentPlayingEpisode);
                        //GUIPropertyManager.SetProperty("#runtime", currentPlayingEpisode);   
                        //GUIPropertyManager.SetProperty("#iswatched", currentPlayingEpisode); 

//                         GUIPropertyManager.SetProperty("#Play.Current.Thumb", currentPlayingSeries.GetImage());
                        GUIPropertyManager.SetProperty("#Play.Current.File", currentPlayingEpisode[DBEpisode.cFilename]);
                        GUIPropertyManager.SetProperty("#Play.Current.Title", currentPlayingSeries[DBSeries.cPrettyName] + " - " + currentPlayingEpisode[DBEpisode.cSeasonIndex] + "x" + currentPlayingEpisode[DBEpisode.cEpisodeIndex] + " - " + currentPlayingEpisode[DBEpisode.cEpisodeName]);
                        GUIPropertyManager.SetProperty("#Play.Current.Genre", currentPlayingSeries[DBSeries.cGenre]);
                        //GUIPropertyManager.SetProperty("#Play.Current.Comment", currentPlayingEpisode);
                        //GUIPropertyManager.SetProperty("#Play.Current.Artist", currentPlayingEpisode);
                        //GUIPropertyManager.SetProperty("#Play.Current.Director", currentPlayingEpisode);
                        //GUIPropertyManager.SetProperty("#Play.Current.Album", currentPlayingEpisode);
                        //GUIPropertyManager.SetProperty("#Play.Current.Track", currentPlayingEpisode);
                        //GUIPropertyManager.SetProperty("#Play.Current.Year", currentPlayingEpisode);
//                        GUIPropertyManager.SetProperty("#Play.Current.Duration", currentPlayingSeries.Duration);
                        GUIPropertyManager.SetProperty("#Play.Current.Plot", currentPlayingEpisode[DBOnlineEpisode.cEpisodeSummary]);
                        //GUIPropertyManager.SetProperty("#Play.Current.PlotOutline", currentPlayingEpisode.Description);
                        //GUIPropertyManager.SetProperty("#Play.Current.Channel", currentPlayingEpisode);
                        //GUIPropertyManager.SetProperty("#Play.Current.Cast", currentPlayingEpisode);
                        //GUIPropertyManager.SetProperty("#Play.Current.DVDLabel", currentPlayingEpisode);
                        //GUIPropertyManager.SetProperty("#Play.Current.IMDBNumber", currentPlayingEpisode);
                        //GUIPropertyManager.SetProperty("#Play.Current.Rating", currentPlayingEpisode);
                        //GUIPropertyManager.SetProperty("#Play.Current.TagLine", currentPlayingEpisode.Description);
                        //GUIPropertyManager.SetProperty("#Play.Current.Votes", currentPlayingEpisode);
                        //GUIPropertyManager.SetProperty("#Play.Current.Credits", currentPlayingEpisode);
                        //GUIPropertyManager.SetProperty("#Play.Current.Runtime", currentPlayingEpisode);
                        //GUIPropertyManager.SetProperty("#Play.Current.MPAARating", currentPlayingEpisode);
                        //GUIPropertyManager.SetProperty("#Play.Current.IsWatched", currentPlayingEpisode);

                        #endregion
                        break;
                    case 4:
                        GUIDialogProgress pDlgProgress = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);
                        pDlgProgress.SetHeading("Importing Videos");
                        pDlgProgress.SetLine(1, "Scanning folder for video files...");
                        pDlgProgress.StartModal(GUIWindowManager.ActiveWindow);
                        pDlgProgress.ShowProgressBar(true);
                        pDlgProgress.Progress();
// 
//                         ImportVideo import = new ImportVideo(this.m_Database);
//                         pDlgProgress.SetLine(2, "Found " + import.GetFiles.Length.ToString() + " video files.");
//                         import.SetGUIProperties(ref pDlgProgress);
//                         pDlgProgress.SetLine(1, "Importing new videos...");
//                         pDlgProgress.SetLine(2, "");
//                         pDlgProgress.Progress();
//                         System.Threading.Thread importThread = new System.Threading.Thread(new System.Threading.ThreadStart(import.Start));
//                         importThread.Start();
                        this.LoadFacade();
                        break;
                }
            }
            catch (Exception ex)
            {
                this.m_Logs.Write("The 'OnShowContextMenu' function has generated an error: " + ex.Message);
            }

        }

        public override void OnAction(Action action)
        {
            if (action.wID == Action.ActionType.ACTION_CANCEL_IMPORT)
                MessageBox.Show("Test");
            if (action.wID == Action.ActionType.ACTION_PARENT_DIR)
            {
                // simulate a back
                OnClicked(this.m_Button_Back.GetID, this.m_Button_Back, action.wID);
                return;
            }
            base.OnAction(action);
        }
        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType)
        {
            if (control == this.m_Button_Back)
            {
                switch (this.m_ListLevel)
                {
                    case "Series":
                        GUIWindowManager.ShowPreviousWindow();
                        break;
                    case "Season":
                        this.m_ListLevel = "Series";
                        this.m_SelectedSeason = null;
                        break;
                    case "Episode":
                        this.m_ListLevel = "Season";
                        this.m_SelectedEpisode = null;
                        break;
                }
                this.LoadFacade();
            }
            else if (control == this.m_Button_View)
            {
                switch (this.m_Facade.View)
                {
                    case GUIFacadeControl.ViewMode.SmallIcons:
                        this.m_Facade.View = GUIFacadeControl.ViewMode.List;
                        GUIControl.SetControlLabel(GetID, controlId, GUILocalizeStrings.Get(101));
                        break;
                    case GUIFacadeControl.ViewMode.List:
                        this.m_Facade.View = GUIFacadeControl.ViewMode.LargeIcons;
                        GUIControl.SetControlLabel(GetID, controlId, GUILocalizeStrings.Get(417));
                        break;
                    case GUIFacadeControl.ViewMode.LargeIcons:
                        this.m_Facade.View = GUIFacadeControl.ViewMode.SmallIcons;
                        GUIControl.SetControlLabel(GetID, controlId, GUILocalizeStrings.Get(100));
                        break;
                }
                this.LoadFacade();
            }
            else if (control == this.m_Facade)
            {
                switch (this.m_ListLevel)
                {
                    case "Series":
                        this.m_ListLevel = "Season";
                        this.m_SelectedSeries = (DBSeries)this.m_Facade.SelectedListItem.TVTag;
                        this.LoadFacade();
                        this.m_Facade.Focus = true;
                        break;
                    case "Season":
                        this.m_ListLevel = "Episode";
                        this.m_SelectedSeason = (DBSeason)this.m_Facade.SelectedListItem.TVTag;
                        this.LoadFacade();
                        this.m_Facade.Focus = true;
                        break;
                    case "Episode":
                        this.m_SelectedEpisode = (DBEpisode)this.m_Facade.SelectedListItem.TVTag;
                        this.m_SelectedEpisode[DBEpisode.cWatched] = 1;
                        this.m_SelectedEpisode.Commit();
                        this.LoadFacade();

                        m_VideoHandler.ResumeOrPlay(this.m_SelectedEpisode[DBEpisode.cFilename]);
                        /*
                        GUIDialogOK pDlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                        pDlgOK.SetHeading("Could not launch video in player");
                        if (g_Player.Play(this.m_SelectedEpisode.Filename))
                        {
                            if (Utils.IsVideo(this.m_SelectedEpisode.Filename) && g_Player.HasVideo)
                            {
                                GUIGraphicsContext.IsFullScreenVideo = true;
                                GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_FULLSCREEN_VIDEO);
                                GUIPropertyManager.SetProperty("#title", this.m_SelectedSeries.Name + " - " + this.m_SelectedEpisode.SeasonNumber + "x" + this.m_SelectedEpisode.EpisodeNumber + " - " + this.m_SelectedEpisode.Title);
                                GUIPropertyManager.SetProperty("#genre", this.m_SelectedSeries.GetGenreString);
                                GUIPropertyManager.SetProperty("#file", this.m_SelectedEpisode.Filename);
                                GUIPropertyManager.SetProperty("#plot", this.m_SelectedEpisode.Description);
                            }
                            else
                            {
                                pDlgOK.SetLine(1, "File format not recognized.  Ensure that the");
                                pDlgOK.SetLine(2, "proper codecs are installed on this system.");
                                pDlgOK.DoModal(GUIWindowManager.ActiveWindow);
                            }
                        }
                        else
                        {
                            if (!System.IO.File.Exists(this.m_SelectedEpisode.Filename))
                            {
                                pDlgOK.SetLine(1, "Could not locate the file in the location");
                                pDlgOK.SetLine(2, "specified in the plugin's configuration.");
                            }
                            else
                            {
                                pDlgOK.SetLine(1, "File format not recognized.  Ensure that the");
                                pDlgOK.SetLine(2, "proper codecs are installed on this system.");
                            }
                            pDlgOK.DoModal(GUIWindowManager.ActiveWindow);
                        }
                        */

                        break;
                }
            }
            base.OnClicked(controlId, control, actionType);
        }

        private void Series_OnItemSelected(GUIListItem item, GUIControl parent)
        {
            DBSeries series = (DBSeries)item.TVTag;
            try
            {
                String filename = series.Banner;
                if (filename != null)
                {
                    this.m_Image.SetFileName(filename);
                    this.m_Image.AllocResources();
                    this.m_Image.KeepAspectRatio = true;
                }
            }
            catch { }
            try
            {
                this.m_Title.Label = series[DBSeries.cPrettyName];
            }
            catch { }
            try
            {
                this.m_Genre.Label = series[DBSeries.cGenre];
            }
            catch { }
            try
            {
                this.m_Description.Label = (String)series[DBSeries.cSummary] + (char)10 + (char)13;
            }
            catch { }
        }
        private void Season_OnItemSelected(GUIListItem item, GUIControl parent)
        {
            DBSeason season = (DBSeason)item.TVTag;
            try
            {
                this.m_Title.Label = this.m_SelectedSeries[DBSeries.cPrettyName] + " Season " + season[DBSeason.cIndex];
            }
            catch { }
            try
            {
                this.m_Genre.Label = this.m_SelectedSeries[DBSeries.cGenre];
            }
            catch { }
            try
            {
                this.m_Description.Label = (String)this.m_SelectedSeries[DBSeries.cSummary] + (char)10 + (char)13;
            }
            catch { }
        }
        private void Episode_OnItemSelected(GUIListItem item, GUIControl parent)
        {
            DBEpisode episode = (DBEpisode)item.TVTag;
            try
            {
                this.m_Title.Label = this.m_SelectedSeason[DBSeason.cIndex] + ": " + episode[DBEpisode.cEpisodeName];
            }
            catch { }
            try
            {
                this.m_Genre.Label = this.m_SelectedSeries[DBSeries.cGenre];
            }
            catch { }
            try
            {
                this.m_Description.Label = (String)episode[DBOnlineEpisode.cEpisodeSummary] + (char)10 + (char)13;
            }
            catch { }

        }

    }
}