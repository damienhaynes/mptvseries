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
using System.Windows.Forms;
using System.Drawing;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortal.Dialogs;
using MediaPortal.Util;
using WindowPlugins.GUITVSeries;
using System.Threading;
using System.Collections.Generic;
using WindowPlugins.GUITVSeries.Feedback;
using Download = WindowPlugins.GUITVSeries.Download;
using Torrent = WindowPlugins.GUITVSeries.Torrent;
using Newzbin = WindowPlugins.GUITVSeries.Newzbin;
using WindowPlugins.GUITVSeries.Subtitles;
using aclib.Performance;
using Cornerstone.MP;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    public class TVSeriesPlugin : GUIWindow, ISetupForm, IFeedback
    {               
        public TVSeriesPlugin()
        {
            m_stepSelections.Add(new string[] { null });
            // disable that dynamic skin adjustment....skinners should have the power to position the elements whereever with the plugin inerveining
            if (DBOption.GetOptions(DBOption.cViewAutoHeight)) DBOption.SetOptions(DBOption.cViewAutoHeight, false);
            
            int artworkDelay = DBOption.GetOptions(DBOption.cArtworkLoadingDelay);
            int backdropDelay = DBOption.GetOptions(DBOption.cBackdropLoadingDelay);

            backdrop = new ImageSwapper();
            backdrop.ImageResource.Delay = backdropDelay;
            backdrop.PropertyOne = "#TVSeries.Fanart.1";
            backdrop.PropertyTwo = "#TVSeries.Fanart.2";

            seriesbanner = new AsyncImageResource();
            seriesbanner.Property = "#TVSeries.SeriesBanner";
            seriesbanner.Delay = artworkDelay;

            seriesposter = new AsyncImageResource();
            seriesposter.Property = "#TVSeries.SeriesPoster";
            seriesposter.Delay = artworkDelay;

            seasonbanner = new AsyncImageResource();
            seasonbanner.Property = "#TVSeries.SeasonBanner";
            seasonbanner.Delay = artworkDelay;	
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
            return "MP-TVSeries Team";
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
            strButtonText = pluginName;
            strButtonImage = String.Empty;
            strButtonImageFocus = String.Empty;
            strPictureImage = "hover_my tv series.png";
            return true;
        }

        #endregion

        private ImageSwapper backdrop;
        private AsyncImageResource seriesbanner = null;
        private AsyncImageResource seriesposter = null;
        private AsyncImageResource seasonbanner = null;

        private Listlevel listLevel = Listlevel.Series;        
		private DBSeries m_SelectedSeries;
        private DBSeason m_SelectedSeason;
        private DBEpisode m_SelectedEpisode;
		
		private DBTable m_FanartItem;
        private VideoHandler m_VideoHandler;
        List<logicalView> m_allViews = new List<logicalView>();
        private logicalView m_CurrLView = null;
        private int m_CurrViewStep = 0;
        private List<string[]> m_stepSelections = new List<string[]>();
        private string[] m_stepSelection = null;
        private List<string> m_stepSelectionPretty = new List<string>();
        private bool skipSeasonIfOne_DirectionDown = true;
        private string[] m_back_up_select_this = null;
        private bool tvsubsWorking = false;
        private bool seriessubWorking = false;
        private bool remositoryWorking = false;
        private bool torrentWorking = false;        
        private bool m_bUpdateBanner = false;
        private TimerCallback m_timerDelegate = null;
        private System.Threading.Timer m_scanTimer = null;
		private System.Threading.Timer m_FanartTimer = null;
        private OnlineParsing m_parserUpdater = null;
        private bool m_parserUpdaterWorking = false;
        private List<CParsingParameters> m_parserUpdaterQueue = new List<CParsingParameters>();        

        private Watcher m_watcherUpdater = null;
        private int m_nUpdateScanLapse = 0;
        private DateTime m_LastLocalScan = DateTime.MinValue;
        private DateTime m_LastUpdateScan = DateTime.MinValue;

        private int m_nInitialIconXOffset = 0;
        private int m_nInitialIconYOffset = 0;
        private int m_nInitialItemHeight = 0;

        private String m_sFormatSeriesCol1 = String.Empty;
        private String m_sFormatSeriesCol2 = String.Empty;
        private String m_sFormatSeriesCol3 = String.Empty;
        private String m_sFormatSeriesTitle = String.Empty;
        private String m_sFormatSeriesSubtitle = String.Empty;
        private String m_sFormatSeriesMain = String.Empty;

        private String m_sFormatSeasonCol1 = String.Empty;
        private String m_sFormatSeasonCol2 = String.Empty;
        private String m_sFormatSeasonCol3 = String.Empty;
        private String m_sFormatSeasonTitle = String.Empty;
        private String m_sFormatSeasonSubtitle = String.Empty;
        private String m_sFormatSeasonMain = String.Empty;

        private String m_sFormatEpisodeCol1 = String.Empty;
        private String m_sFormatEpisodeCol2 = String.Empty;
        private String m_sFormatEpisodeCol3 = String.Empty;
        private String m_sFormatEpisodeTitle = String.Empty;
        private String m_sFormatEpisodeSubtitle = String.Empty;
        private String m_sFormatEpisodeMain = String.Empty;
        private String pluginName = DBOption.GetOptions(DBOption.cView_PluginName);
        public static int logosHeight = 100;
        public static int logosWidth = 250;
        private Control m_localControlForInvoke;
        private DBEpisode ask2Rate = null;
        private static bool m_bResumeFromStandby = false;
        private static bool m_bIsNetworkAvailable = true;
        private static bool m_bQuickSelect = false;
        public static PlayListPlayer _playlistPlayer;
        public static bool m_bOnActionProcessed = false;
        private string m_prevSeriesID = string.Empty;
        private bool m_bFanartTimerDisabled = false;

        #region Skin Variables

        [SkinControlAttribute(2)]
        protected GUIButtonControl viewMenuButton = null;

        [SkinControlAttribute(3)]
        protected GUIButtonControl LayoutMenuButton = null;

        [SkinControlAttribute(4)]
        protected GUIButtonControl OptionsMenuButton = null;

        [SkinControlAttribute(5)]
        protected GUIButtonControl ImportButton = null;

        [SkinControlAttribute(9)]
        protected GUIButtonControl LoadPlaylistButton = null;

        [SkinControlAttribute(50)]
        protected GUIFacadeControl m_Facade = null;

        [SkinControlAttribute(51)]
        protected GUIAnimation m_ImportAnimation = null;

        [SkinControlAttribute(66)]
        protected GUIImage m_Logos_Image = null;

        [SkinControlAttribute(67)]
        protected GUIImage m_Episode_Image = null;

        [SkinControlAttribute(524)]
        protected GUIImage FanartBackground = null;
        
        [SkinControlAttribute(525)]
        protected GUIImage FanartBackground2 = null;

        [SkinControlAttribute(526)]
        protected GUIImage loadingImage = null;

        // let the skins react to what we are displaying
        [SkinControlAttribute(1232)]
        protected GUILabelControl dummyIsFanartLoaded = null;

        [SkinControlAttribute(1233)]
        protected GUILabelControl dummyIsDarkFanartLoaded = null;

        [SkinControlAttribute(1234)]
        protected GUILabelControl dummyIsLightFanartLoaded = null;

        [SkinControlAttribute(1235)]
        protected GUILabelControl dummyFacadeListMode = null;

        [SkinControlAttribute(1236)]
        protected GUILabelControl dummyThumbnailGraphicalMode = null;
        
        [SkinControlAttribute(1237)]
        protected GUILabelControl dummyIsSeries = null;

        [SkinControlAttribute(1238)]
        protected GUILabelControl dummyIsSeasons = null;

        [SkinControlAttribute(1239)]
        protected GUILabelControl dummyIsEpisodes = null;

        [SkinControlAttribute(1240)]
        protected GUILabelControl dummyIsGroups = null;

        [SkinControlAttribute(1241)]
        protected GUILabelControl dummyIsFanartColorAvailable = null;

        [SkinControlAttribute(1242)]
        protected GUILabelControl dummyIsSeriesPosters = null;

        [SkinControlAttribute(1243)]
        protected GUILabelControl dummyIsWatched = null;

        [SkinControlAttribute(1244)]
        protected GUILabelControl dummyIsAvailable = null;

        #endregion

        enum Listlevel
        {
            Episode,
            Season,
            Series,
            Group
        }

        public override int GetID
        {
            get
            {
                return 9811;
            }
        }

        public override bool Init()
        {
            m_localControlForInvoke = new Control();
            m_localControlForInvoke.CreateControl();
            Translation.Init();
            MPTVSeriesLog.Write("**** Plugin started in MediaPortal ***");

            Download.Monitor.Start(this);
            m_VideoHandler = new VideoHandler();
            m_parserUpdater = new OnlineParsing(this);
            m_parserUpdater.OnlineParsingCompleted += new OnlineParsing.OnlineParsingCompletedHandler(parserUpdater_OnlineParsingCompleted);

            if (DBOption.GetOptions("doFolderWatch"))
            {
                System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += NetworkAvailabilityChanged;

                setUpFolderWatches();

                // do a local scan when starting up the app if enabled - later on the watcher will monitor changes
                if (DBOption.GetOptions("ScanOnStartup"))
                {
                    m_parserUpdaterQueue.Add(new CParsingParameters(true, false));
                    Microsoft.Win32.SystemEvents.PowerModeChanged += new Microsoft.Win32.PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
                }
            }
            else
            {
                // else the user has selected to always manually do local scans
                setProcessAnimationStatus(false);
            }
            
            // Import Skin Settings
            string xmlSkinSettings = GUIGraphicsContext.Skin + @"\TVSeries.SkinSettings.xml";
            SkinSettings.Load(xmlSkinSettings);

            #region Display Format Strings
            m_sFormatSeriesCol1 = DBOption.GetOptions(DBOption.cView_Series_Col1);
            m_sFormatSeriesCol2 = DBOption.GetOptions(DBOption.cView_Series_Col2);
            m_sFormatSeriesCol3 = DBOption.GetOptions(DBOption.cView_Series_Col3);
            m_sFormatSeriesTitle = DBOption.GetOptions(DBOption.cView_Series_Title);
            m_sFormatSeriesSubtitle = DBOption.GetOptions(DBOption.cView_Series_Subtitle);
            m_sFormatSeriesMain = DBOption.GetOptions(DBOption.cView_Series_Main);

            m_sFormatSeasonCol1 = DBOption.GetOptions(DBOption.cView_Season_Col1);
            m_sFormatSeasonCol2 = DBOption.GetOptions(DBOption.cView_Season_Col2);
            m_sFormatSeasonCol3 = DBOption.GetOptions(DBOption.cView_Season_Col3);
            m_sFormatSeasonTitle = DBOption.GetOptions(DBOption.cView_Season_Title);
            m_sFormatSeasonSubtitle = DBOption.GetOptions(DBOption.cView_Season_Subtitle);
            m_sFormatSeasonMain = DBOption.GetOptions(DBOption.cView_Season_Main);

            m_sFormatEpisodeCol1 = DBOption.GetOptions(DBOption.cView_Episode_Col1);
            m_sFormatEpisodeCol2 = DBOption.GetOptions(DBOption.cView_Episode_Col2);
            m_sFormatEpisodeCol3 = DBOption.GetOptions(DBOption.cView_Episode_Col3);
            m_sFormatEpisodeTitle = DBOption.GetOptions(DBOption.cView_Episode_Title);
            m_sFormatEpisodeSubtitle = DBOption.GetOptions(DBOption.cView_Episode_Subtitle);
            m_sFormatEpisodeMain = DBOption.GetOptions(DBOption.cView_Episode_Main);
            #endregion

            try
            {
                m_LastUpdateScan = DateTime.Parse(DBOption.GetOptions(DBOption.cUpdateScanLastTime));
            }
            catch { }

            if (DBOption.GetOptions(DBOption.cAutoUpdateOnlineData))
                m_nUpdateScanLapse = DBOption.GetOptions(DBOption.cAutoUpdateOnlineDataLapse);

            // timer check every seconds
            m_timerDelegate = new TimerCallback(Clock);
            m_scanTimer = new System.Threading.Timer(m_timerDelegate, null, 1000, 1000);
            m_VideoHandler.RateRequestOccured += new VideoHandler.rateRequest(m_VideoHandler_RateRequestOccured);

            // Setup Random Fanart Timer
            m_FanartTimer = new System.Threading.Timer(new TimerCallback(FanartTimerEvent), null, Timeout.Infinite, Timeout.Infinite);
            m_bFanartTimerDisabled = true;

            String xmlSkin = GUIGraphicsContext.Skin + @"\TVSeries.xml";
            MPTVSeriesLog.Write("Loading XML Skin: " + xmlSkin);
            SkinSettings.GetSkinProperties(xmlSkin);

            return Load(xmlSkin);
        }

        void m_VideoHandler_RateRequestOccured(DBEpisode episode)
        {
            ask2Rate = episode;
        }

        void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            if (e.Mode == Microsoft.Win32.PowerModes.Resume)
            {
                MPTVSeriesLog.Write("MP-TVSeries is resuming from standby");
                IsResumeFromStandby = true;

                // event is only registered if watch folder option is ticked, so no need to check again here
                // we have to reregister the folder watches
                setUpFolderWatches();

                // lets do a full folder scan since we might have network shares which could have been updated
                if (DBOption.GetOptions("ScanOnStartup"))
                {
                    m_parserUpdaterQueue.Add(new CParsingParameters(true, false));
                }
            }
            else if (e.Mode == Microsoft.Win32.PowerModes.Suspend)
            {
                MPTVSeriesLog.Write("MP-TVSeries is entering standby");
            }
        }

        public static bool IsResumeFromStandby
        {
            get { return m_bResumeFromStandby; }
            set { m_bResumeFromStandby = value; }
        }

        void NetworkAvailabilityChanged(object sender, System.Net.NetworkInformation.NetworkAvailabilityEventArgs e)
        {
            if (e.IsAvailable)
            {
                MPTVSeriesLog.Write("MP-TVSeries is connected to the network");
                IsNetworkAvailable = true;
            }
            else 
            { 
                MPTVSeriesLog.Write("MP-TVSeries is disconnected from the network");
                DBOnlineMirror.IsMirrorsAvailable = false; // Force to recheck later
                IsNetworkAvailable = false;
            }
        }

        public static bool IsNetworkAvailable
        {
            get { return m_bIsNetworkAvailable; }
            set { m_bIsNetworkAvailable = value; }
        }

        void watcherUpdater_WatcherProgress(int nProgress, List<WatcherItem> modifiedFilesList)
        {
            List<PathPair> filesAdded = new List<PathPair>();
            List<PathPair> filesRemoved = new List<PathPair>();

            // go over the modified files list once in a while & update
            foreach (WatcherItem item in modifiedFilesList)
            {
                switch (item.m_type)
                {
                    case WatcherItemType.Added:
                        filesAdded.Add(new PathPair(item.m_sParsedFileName, item.m_sFullPathFileName));
                        break;

                    case WatcherItemType.Deleted:
                        filesRemoved.Add(new PathPair(item.m_sParsedFileName, item.m_sFullPathFileName));
                        break;
                }
            }

            // with out list of files, start the parsing process
            if (filesAdded.Count > 0)
            {
                // queue it
                lock (m_parserUpdaterQueue)
                {
                    m_parserUpdaterQueue.Add(new CParsingParameters(ParsingAction.List_Add, filesAdded, true, false));
                }
            }

            if (filesRemoved.Count > 0)
            {
                // queue it
                lock (m_parserUpdaterQueue)
                {
                    m_parserUpdaterQueue.Add(new CParsingParameters(ParsingAction.List_Remove, filesRemoved, false, false));
                }
            }
        }

        // this is expensive to do if changing mode......450 ms ???
        void setFacadeMode(GUIFacadeControl.ViewMode mode)
        {
            if (this.m_Facade == null)
                return;

            if (mode == GUIFacadeControl.ViewMode.List)
            {
                PerfWatcher.GetNamedWatch("FacadeMode - switch to List").Start();
                this.m_Facade.View = mode;
                if (this.dummyThumbnailGraphicalMode != null) dummyThumbnailGraphicalMode.Visible = false;
                if (this.dummyFacadeListMode != null) this.dummyFacadeListMode.Visible = true;
                PerfWatcher.GetNamedWatch("FacadeMode - switch to List").Stop();
            }
            else
            {
                PerfWatcher.GetNamedWatch("FacadeMode - switch to Album").Start();
                if (mode == GUIFacadeControl.ViewMode.AlbumView)
                {
                    switch (this.listLevel)
                    {
                        case (Listlevel.Series):
                            if (DBOption.GetOptions(DBOption.cView_Series_ListFormat) == "Filmstrip")
                            {
                                MPTVSeriesLog.Write("FacadeMode: Switching to FilmStrip", MPTVSeriesLog.LogLevel.Debug);
                                this.m_Facade.View = GUIFacadeControl.ViewMode.Filmstrip;
                            }
                            if (DBOption.GetOptions(DBOption.cView_Series_ListFormat) == "WideBanners")
                            {
                                MPTVSeriesLog.Write("FacadeMode: Switching to WideThumbs", MPTVSeriesLog.LogLevel.Debug);
                                this.m_Facade.View = GUIFacadeControl.ViewMode.LargeIcons;
                            }
                            break;
                        case (Listlevel.Season):
                            // There is no point having BigIcons for SeasonView, as it would need to re-use the WideBanner sizes
                            // Having multiple facades would get around this issue
                            MPTVSeriesLog.Write("FacadeMode: Switching to Filmstrip", MPTVSeriesLog.LogLevel.Debug);
                            this.m_Facade.View = GUIFacadeControl.ViewMode.Filmstrip;
                            break;
                        case (Listlevel.Group):
                            MPTVSeriesLog.Write("FacadeMode: Switching to Small Thumbs", MPTVSeriesLog.LogLevel.Debug);
                            this.m_Facade.View = GUIFacadeControl.ViewMode.SmallIcons;
                            break;
                    }
                }
                PerfWatcher.GetNamedWatch("FacadeMode - switch to Album").Stop();
                if (this.dummyThumbnailGraphicalMode != null) this.dummyThumbnailGraphicalMode.Visible = true;
                if (this.dummyFacadeListMode != null) this.dummyFacadeListMode.Visible = false;                
            }            
        }
        
        System.ComponentModel.BackgroundWorker bg = null;        

        void LoadFacade()
        {
            if (bg == null)
            {
                bg = new System.ComponentModel.BackgroundWorker();
                bg.DoWork += new System.ComponentModel.DoWorkEventHandler(bgLoadFacade);
                bg.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(bgFacadeDone);
                bg.WorkerReportsProgress = true;
                bg.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(bg_ProgressChanged);
                bg.WorkerSupportsCancellation = true;
            }

            lock (bg)
            {
                if (bg.IsBusy) // we have to wait - complete method will call LoadFacade again
                {
                    if (!bg.CancellationPending)
                        bg.CancelAsync();
                    return;
                }
                aclib.Performance.PerfWatcher.GetNamedWatch("FacadeLoading").Start();
                prepareLoadFacade();
                bg.RunWorkerAsync();
            }
        }

        bool bFacadeEmpty = true;
        void prepareLoadFacade()
        {
            try
            {
                if (m_nInitialIconXOffset == 0)
                    m_nInitialIconXOffset = m_Facade.AlbumListView.IconOffsetX;
                if (m_nInitialIconYOffset == 0)
                    m_nInitialIconYOffset = m_Facade.AlbumListView.IconOffsetY;
                if (m_nInitialItemHeight == 0)
                    m_nInitialItemHeight = m_Facade.AlbumListView.ItemHeight;

                this.m_Facade.ListView.Clear();
                this.m_Facade.AlbumListView.Clear();
                
                if (this.m_Facade.ThumbnailView != null)
                    this.m_Facade.ThumbnailView.Clear();

                if (this.m_Facade.FilmstripView != null)
                    this.m_Facade.FilmstripView.Clear();

                if (m_Facade != null) m_Facade.Focus = true;
                MPTVSeriesLog.Write("LoadFacade: ListLevel: ", listLevel.ToString(), MPTVSeriesLog.LogLevel.Debug);
                setCurPositionLabel();
                // always clear all fields
                switch (this.listLevel)
                {
                    case Listlevel.Series:
                    case Listlevel.Group:
                        clearFieldsForskin("Season");
                        clearFieldsForskin("Series");
                        goto case Listlevel.Episode;
                    case Listlevel.Season:
                        int nSeasonDisplayMode = DBOption.GetOptions(DBOption.cView_Season_ListFormat);
                        if (nSeasonDisplayMode != 0)
                            setFacadeMode(GUIFacadeControl.ViewMode.AlbumView);
                        else
                            setFacadeMode(GUIFacadeControl.ViewMode.List);
                        goto case Listlevel.Episode;
                    case Listlevel.Episode:
                        clearFieldsForskin("Episode");
                        break;

                }
                setNewListLevelOfCurrView(m_CurrViewStep);

            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Error preparing Facade... " + ex.Message);
            }
        }

        enum SkipSeasonCodes
        {
            none,
            SkipSeasonDown,
            SkipSeasonUp
        }
        SkipSeasonCodes SkipSeasonCode = SkipSeasonCodes.none;
        List<GUIListItem> itemsForDelayedImgLoading = null;
        void bg_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            try
            {
                BackgroundFacadeLoadingArgument arg = e.UserState as BackgroundFacadeLoadingArgument;
                MPTVSeriesLog.Write("bg_ProgressChanged for: " + arg.Type.ToString(), MPTVSeriesLog.LogLevel.Debug);

                if (bg.CancellationPending)
                {
                    MPTVSeriesLog.Write("bg_ProgressChanged cancelled", MPTVSeriesLog.LogLevel.Debug);
                    return;
                }

                PerfWatcher.GetNamedWatch("FacadeLoading changed").Start();
                if (arg == null || arg.Type == BackGroundLoadingArgumentType.None) return;

                switch (arg.Type)
                {
                    case BackGroundLoadingArgumentType.FullElement:
                    case BackGroundLoadingArgumentType.ElementForDelayedImgLoading:
                        {
                            PerfWatcher.GetNamedWatch("FacadeLoading addElem").Start();
                            GUIListItem gli = arg.Argument as GUIListItem;
                            if (m_Facade != null && gli != null)
                            {
                                // Messages are not recieved in OnMessage for Filmstrip, instead subscribe to OnItemSelected
                                if (m_Facade.View == GUIFacadeControl.ViewMode.Filmstrip)
                                    gli.OnItemSelected += new GUIListItem.ItemSelectedHandler(onFacadeItemSelected);

                                bFacadeEmpty = false;
                                m_Facade.Add(gli);
                                if (arg.Type == BackGroundLoadingArgumentType.ElementForDelayedImgLoading)
                                {
                                    if (itemsForDelayedImgLoading == null)
                                        itemsForDelayedImgLoading = new List<GUIListItem>();
                                    itemsForDelayedImgLoading.Add(gli);
                                }
                            }
                            PerfWatcher.GetNamedWatch("FacadeLoading addElem").Stop();
                        }
                        break;

                    case BackGroundLoadingArgumentType.DelayedImgLoading:
                        {
                            PerfWatcher.GetNamedWatch("FacadeLoading addDelayedImage").Start();
                            if (itemsForDelayedImgLoading != null && itemsForDelayedImgLoading.Count > arg.IndexArgument)
                            {
                                string image = arg.Argument as string;
                                itemsForDelayedImgLoading[arg.IndexArgument].IconImageBig = image;
                            }
                            PerfWatcher.GetNamedWatch("FacadeLoading addDelayedImage").Stop();
                        }
                        break;

                    case BackGroundLoadingArgumentType.ElementSelection:
                        {
                            // thread told us which element it'd like to select
                            // however the user might have already started moving around
                            // if that is the case, we don't select anything
                            MPTVSeriesLog.Write("Element Selection: " + arg.IndexArgument.ToString(), MPTVSeriesLog.LogLevel.Debug);
                            if (this.m_Facade != null && this.m_Facade.SelectedListItemIndex < 1)
                            {
                                this.m_Facade.Focus = true;
                                this.m_Facade.SelectedListItemIndex = arg.IndexArgument;

                                 // if we are in the filmstrip view also send a message
                                /*if (m_Facade.View == GUIFacadeControl.ViewMode.Filmstrip)
                                {
                                    GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, m_Facade.WindowId, 0, m_Facade.FilmstripView.GetID, arg.IndexArgument, 0, null);
                                    GUIGraphicsContext.SendMessage(msg);
                                    MPTVSeriesLog.Write("Sending a selection postcard to FilmStrip.",MPTVSeriesLog.LogLevel.Debug);
                                }*/

                                // Hack for 'set' SelectedListItemIndex not being implemented in Filmstrip View
                                // Navigate to selected using OnAction instead 
                                if (m_Facade.View == GUIFacadeControl.ViewMode.Filmstrip)
                                {
                                    if (this.listLevel == Listlevel.Series)
                                    {
                                        List<DBSeries> seriesList = m_CurrLView.getSeriesItems(m_CurrViewStep, m_stepSelection);

                                        if (arg.IndexArgument > 0)
                                        {
                                            m_bQuickSelect = true;
                                            for (int i = m_Facade.SelectedListItemIndex; i < seriesList.Count - 1; i++)
                                            {
                                                if (i == arg.IndexArgument)
                                                    break;
                                                // Now push fields to skin
                                                if (i == (arg.IndexArgument - 1))
                                                    m_bQuickSelect = false;

                                                OnAction(new Action(Action.ActionType.ACTION_MOVE_RIGHT, 0, 0));
                                            }
                                            m_bQuickSelect = false;
                                        }
                                        else
                                        {
                                            if (seriesList.Count > 0)
                                            {
                                                GUIListItem selected = new GUIListItem();
                                                selected.TVTag = seriesList[0];
                                                Series_OnItemSelected(selected);
                                            }
                                        }
                                    }
                                    else if (this.listLevel == Listlevel.Season)
                                    {
                                        List<DBSeason> seasonList = m_CurrLView.getSeasonItems(m_CurrViewStep, m_stepSelection);

                                        if (arg.IndexArgument > 0)
                                        {
                                            m_bQuickSelect = true;
                                            for (int i = m_Facade.SelectedListItemIndex; i < seasonList.Count - 1; i++)
                                            {
                                                if (i == arg.IndexArgument)
                                                    break;
                                                // Now push fields to skin
                                                if (i == (arg.IndexArgument - 1))
                                                    m_bQuickSelect = false;

                                                OnAction(new Action(Action.ActionType.ACTION_MOVE_RIGHT, 0, 0));
                                            }
                                            m_bQuickSelect = false;
                                        }
                                        else
                                        {
                                            if (seasonList.Count > 0)
                                            {
                                                GUIListItem selected = new GUIListItem();
                                                selected.TVTag = seasonList[0];
                                                Season_OnItemSelected(selected);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case BackGroundLoadingArgumentType.DelayedImgInit:
                        itemsForDelayedImgLoading = null;
                        break;
                    case BackGroundLoadingArgumentType.SkipSeasonDown:
                        SkipSeasonCode = SkipSeasonCodes.SkipSeasonDown;
                        break;
                    case BackGroundLoadingArgumentType.SkipSeasonUp:
                        SkipSeasonCode = SkipSeasonCodes.SkipSeasonUp;
                        break;
                    case BackGroundLoadingArgumentType.SetFacadeMode:
                        GUIFacadeControl.ViewMode viewMode = (GUIFacadeControl.ViewMode)arg.Argument;
                        setFacadeMode(viewMode);
                        break;
                }
                PerfWatcher.GetNamedWatch("FacadeLoading changed").Stop();
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write(string.Format("Error in bg_ProgressChanged: {0}: {1}", ex.Message, ex.InnerException));
            }
        }

        void bgFacadeDone(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                aclib.Performance.PerfWatcher.GetNamedWatch("FacadeLoading").Stop();
                foreach (aclib.Performance.Watch w in aclib.Performance.PerfWatcher.InstantiatedWatches)
                {
                    MPTVSeriesLog.Write(w.Info, MPTVSeriesLog.LogLevel.Debug);
                    w.Reset();
                }
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Error writing performance statistics to log " + ex.InnerException);
            }

            // ZF - seems to be crashing because of facade being null sometimes, before getting inside the plugin
            if (m_Facade == null)
                return;

            if (e.Cancelled)
            {
                MPTVSeriesLog.Write("in facadedone - detected cancel - performing delayed userclick");
                SkipSeasonCode = SkipSeasonCodes.none;
                skipSeasonIfOne_DirectionDown = true;
                LoadFacade(); // we only cancel if the user clicked something while we were still loading
                // whatever was selected we will enter (this is because m_selected whatever will not get updated
                // even if the user selects somethign else while we wait for cancellation due to it being a different listlevel)                                
                return;
            }
            MPTVSeriesLog.Write("in facadedone",MPTVSeriesLog.LogLevel.Debug);
           
            if (m_Facade == null)
                return;

            m_Facade.Focus = true;
            
            if (bFacadeEmpty)
            {
                if (m_CurrViewStep == 0)
                {
                    setFacadeMode(GUIFacadeControl.ViewMode.List);
                    GUIListItem item = new GUIListItem(Translation.No_items);
                    item.IsRemote = true;
                    this.m_Facade.Add(item);

                    clearGUIProperty(guiProperty.Title);
                    clearGUIProperty(guiProperty.Subtitle);
                    clearGUIProperty(guiProperty.Description);
                    
                    clearGUIProperty(guiProperty.SeriesBanner);
                    clearGUIProperty(guiProperty.SeriesPoster);
                    clearGUIProperty(guiProperty.SeasonBanner);
                    clearGUIProperty(guiProperty.EpisodeImage);
                    clearGUIProperty(guiProperty.Logos);

                }
                else
                {
                    // probably something was removed
                    MPTVSeriesLog.Write("Nothing to display, going out");
                    OnAction(new Action(Action.ActionType.ACTION_PREVIOUS_MENU, 0, 0));
                }
            }
            if (ask2Rate != null)
            {
                showRatingsDialog(ask2Rate, true);
                ask2Rate = null;
                LoadFacade();
            }
            if (skipSeasonIfOne_DirectionDown && SkipSeasonCode == SkipSeasonCodes.SkipSeasonDown)
            {
                OnClicked(m_Facade.GetID, m_Facade, Action.ActionType.ACTION_SELECT_ITEM);
            }
            else if (!skipSeasonIfOne_DirectionDown && SkipSeasonCode == SkipSeasonCodes.SkipSeasonUp)
            {
                OnAction(new Action(Action.ActionType.ACTION_PREVIOUS_MENU, 0, 0));
            }
            SkipSeasonCode = SkipSeasonCodes.none;
            skipSeasonIfOne_DirectionDown = true;
        }

        void bgLoadFacade(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            //facadeLoaded = false; // reset
            PerfWatcher.GetNamedWatch("FacadeLoading BG Thread").Start();
            //using (WaitCursor c = new WaitCursor()) // should we show a waitcursor?
            bgLoadFacade();
            PerfWatcher.GetNamedWatch("FacadeLoading BG Thread").Stop();
            MPTVSeriesLog.Write("bgLoadFacade done", MPTVSeriesLog.LogLevel.Debug);
            if (bg.CancellationPending)
                e.Cancel = true;

        }

        void bgLoadFacade()
        {
            MPTVSeriesLog.Write("Begin LoadFacade", MPTVSeriesLog.LogLevel.Debug);
            try
            {
                GUIListItem item = null;
                int selectedIndex = -1;
                int count = 0;
                bool delayedImageLoading = false;
                List<DBSeries> seriesList = null;
                PerfWatcher.GetNamedWatch("FacadeLoading getting/reporting items").Start();

                switch (this.listLevel)
                {
                    #region Group
                    case Listlevel.Group:
                        {
                            ImageAllocator.FlushAll();
                            // these are groups of certain categories, eg. Genres

                            bool graphical = DBOption.GetOptions(DBOption.cGraphicalGroupView);
                            ReportFacadeLoadingProgress(BackGroundLoadingArgumentType.SetFacadeMode, 0, graphical ? GUIFacadeControl.ViewMode.AlbumView : GUIFacadeControl.ViewMode.List);
                            // view handling
                            List<string> items = m_CurrLView.getGroupItems(m_CurrViewStep, m_stepSelection);

                            setGUIProperty(guiProperty.GroupCount, items.Count.ToString());

                            if (items.Count == 0)
                                bFacadeEmpty = true;

                            for (int index = 0; index < items.Count; index++)
                            {                                
                                item = new GUIListItem(items[index]);
                                if (item.Label.Length == 0) item.Label = Translation.Unknown;
                                item.TVTag = items[index];
                                item.IsRemote = true;
                                item.IsDownloading = true;

                                if (graphical || DBOption.GetOptions(DBOption.cAppendFirstLogoToList))
                                {
                                    // also display fist logo in list directly
                                    item.IconImage = item.IconImageBig = localLogos.getLogos(m_CurrLView.groupedInfo(m_CurrViewStep), item.Label, 0, 0);
                                }
                                else
                                {
                                    //string groupedBy = m_CurrLView.groupedInfo(m_CurrViewStep);
                                    //SQLCondition cond = new SQLCondition();
                                    //if (m_CurrLView.m_steps[m_CurrViewStep].groupedBy.attempSplit && item.Label != Translation.Unknown)
                                    //{
                                    //    cond.Add(new DBOnlineSeries(), groupedBy.Substring(groupedBy.IndexOf('.') + 1).Replace(">", ""), item.Label, SQLConditionType.Like);
                                    //}
                                    //else
                                    //{
                                    //    cond.Add(new DBOnlineSeries(), groupedBy.Substring(groupedBy.IndexOf('.') + 1).Replace(">", ""),
                                    //             (item.Label == Translation.Unknown ? string.Empty : item.Label),
                                    //              SQLConditionType.Equal);
                                    //}
                                    //if (DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles))
                                    //{
                                    //    // not generic
                                    //    SQLCondition fullSubCond = new SQLCondition();
                                    //    fullSubCond.AddCustom(DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID), DBOnlineSeries.Q(DBOnlineSeries.cID), SQLConditionType.Equal);
                                    //    cond.AddCustom(" exists( " + DBEpisode.stdGetSQL(fullSubCond, false) + " )");
                                    //}
                                    //cond.AddCustom("exists ( select id from local_series where id = online_series.id and hidden = 0)");
                                    //List<DBValue> seriesInGroup = DBOnlineSeries.GetSingleField(DBOnlineSeries.cPrettyName, cond, new DBOnlineSeries());
                                    //item.Label3 = string.Format("{0} {1}", seriesInGroup.Count.ToString(), Translation.Series_Plural);

                                }

                                ReportFacadeLoadingProgress(BackGroundLoadingArgumentType.FullElement, index, item);                             

                                if (m_back_up_select_this != null && selectedIndex == -1 && item.Label == m_back_up_select_this[0])
                                    selectedIndex = index;
                            }

                        }
                        break;
                    #endregion
                    #region Series
                    case Listlevel.Series:
                        {
                            string sSeriesDisplayMode = DBOption.GetOptions(DBOption.cView_Series_ListFormat);
                            
                            // We dont need to load thumbnails in the facade if using List View types
                            if (!sSeriesDisplayMode.Contains("List"))
                            {
                                // reinit the itemsList
                                delayedImageLoading = true;
                                ReportFacadeLoadingProgress(BackGroundLoadingArgumentType.DelayedImgInit, 0, null);
                            }

                            if (DBOption.GetOptions(DBOption.cRandomBanner)) ImageAllocator.FlushAll();
                            else
                            {
                                // flush episodes & seasons
                                ImageAllocator.FlushOthers(false);
                                ImageAllocator.FlushSeasons();
                            }                            

                            if (!sSeriesDisplayMode.Contains("List"))
                            {
                                // graphical
                                ReportFacadeLoadingProgress(BackGroundLoadingArgumentType.SetFacadeMode, 0, GUIFacadeControl.ViewMode.AlbumView);
                            }
                            else
                            {
                                // text as usual
                                ReportFacadeLoadingProgress(BackGroundLoadingArgumentType.SetFacadeMode, 0, GUIFacadeControl.ViewMode.List);
                            }

                            if (bg.CancellationPending) return;
                            seriesList = m_CurrLView.getSeriesItems(m_CurrViewStep, m_stepSelection);

                            if (seriesList.Count == 0)
                                bFacadeEmpty = true;

                            // Update Series Count Property
                            setGUIProperty(guiProperty.SeriesCount, seriesList.Count.ToString());

                            MPTVSeriesLog.Write(string.Format("Displaying {0} series", seriesList.Count.ToString()), MPTVSeriesLog.LogLevel.Debug);
                            foreach (DBSeries series in seriesList)
                            {
                                if (bg.CancellationPending) return;
                                try
                                {
                                    item = null;
                                    if (!sSeriesDisplayMode.Contains("List"))
                                    {
                                        // Graphical Mode
                                        item = new GUIListItem();                                                                                           
                                    }
                                    else
                                    {
                                        // Adjust Display name to reflect sort order
                                        if (DBOption.GetOptions(DBOption.cSeries_UseSortName))
                                            m_sFormatSeriesCol2 = m_sFormatSeriesCol2.Replace("Series.Pretty_Name", "Series.SortName");
                                        else
                                            m_sFormatSeriesCol2 = m_sFormatSeriesCol2.Replace("Series.SortName", "Series.Pretty_Name");

                                        item = new GUIListItem(FieldGetter.resolveDynString(m_sFormatSeriesCol2, series));
                                        item.Label2 = FieldGetter.resolveDynString(m_sFormatSeriesCol3, series);
                                        item.Label3 = FieldGetter.resolveDynString(m_sFormatSeriesCol1, series);

                                        bool bWatched = (int.Parse(series[DBOnlineSeries.cEpisodesUnWatched])==0);
                                        bool bAvailable = series[DBOnlineSeries.cHasLocalFiles];

                                        LoadWatchedFlag(item, bWatched, bAvailable);                                        
                                    }
                                    item.TVTag = series;
                                    // Has no local files in series
                                    item.IsRemote = series[DBOnlineSeries.cHasLocalFiles] != 0;
                                    // Has atleast one unwatched episode in series - no IsWatched property
                                    item.IsDownloading = int.Parse(series[DBOnlineSeries.cEpisodesUnWatched]) != 0;

                                    if (this.m_SelectedSeries != null)
                                    {
                                        if (series[DBSeries.cID] == this.m_SelectedSeries[DBSeries.cID])
                                        {
                                            selectedIndex = count;
                                        }
                                    }
                                    else
                                    {
                                        // select the first that has a file
                                        if (selectedIndex == -1 && series[DBOnlineSeries.cHasLocalFiles] != 0)
                                        {
                                            selectedIndex = count;                                        
                                        }
                                    }

                                    if (m_back_up_select_this != null && series != null && selectedIndex == -1 && series[DBSeries.cID] == m_back_up_select_this[0])
                                    {
                                        selectedIndex = count;                                        
                                    }
                                   
                                    if (bg.CancellationPending) return;
                                    else
                                    {
                                        if (!sSeriesDisplayMode.Contains("List"))
                                            ReportFacadeLoadingProgress(BackGroundLoadingArgumentType.ElementForDelayedImgLoading, count, item);
                                        else
                                            ReportFacadeLoadingProgress(BackGroundLoadingArgumentType.FullElement, count, item);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    MPTVSeriesLog.Write("The 'LoadFacade' function has generated an error displaying series list item: " + ex.Message);
                                }
                                count++;
                            }
                        }
                        break;
                    #endregion
                    #region Season
                    case Listlevel.Season:
                        {
                            // view handling                               
                            List<DBSeason> seasons = m_CurrLView.getSeasonItems(m_CurrViewStep, m_stepSelection);

                            bool canBeSkipped = (seasons.Count == 1);
                            if (!canBeSkipped)
								MPTVSeriesLog.Write(string.Format("Displaying {0} seasons from {1}", seasons.Count.ToString(), m_SelectedSeries), MPTVSeriesLog.LogLevel.Normal);

                            foreach (DBSeason season in seasons)
                            {
                                try
                                {
                                    int nSeasonDisplayMode = DBOption.GetOptions(DBOption.cView_Season_ListFormat);
                                    item = null;
                                    if (!canBeSkipped)
                                    {
                                        if (nSeasonDisplayMode != 0)
                                        {
                                            item = new GUIListItem();
                                            item.IconImage = item.IconImageBig = ImageAllocator.GetSeasonBanner(season, true);
                                        }
                                        else
                                        {
                                            item = new GUIListItem(FieldGetter.resolveDynString(m_sFormatSeasonCol2, season));
                                            item.Label2 = FieldGetter.resolveDynString(m_sFormatSeasonCol3, season);
                                            item.Label3 = FieldGetter.resolveDynString(m_sFormatSeasonCol1, season);

                                            bool bWatched = (int.Parse(season[DBOnlineSeries.cEpisodesUnWatched]) == 0);
                                            bool bAvailable = season[DBSeason.cHasLocalFiles];

                                            if (!LoadWatchedFlag(item, bWatched, bAvailable))
                                            {
                                                if (DBOption.GetOptions(DBOption.cAppendFirstLogoToList))
                                                {
                                                    // if skins want to display the logo in the textual list, users need to set the option (expensive)
                                                    item.IconImage = ImageAllocator.GetSeasonBanner(season, false);
                                                }
                                            }                                                                                                                                    
                                        }
                                        // Has no local files in season
                                        item.IsRemote = season[DBSeason.cHasLocalFiles] != 0;
                                        // Has atleast one unwatched episode in season - no IsWatched property
                                        item.IsDownloading = int.Parse(season[DBSeason.cEpisodesUnWatched]) != 0;
                                    }
                                    else item = new GUIListItem();
                                    item.TVTag = season;

                                    if (!canBeSkipped)
                                    {
                                        if (this.m_SelectedSeason != null)
                                        {
                                            if (this.m_SelectedSeason[DBSeason.cIndex] == season[DBSeason.cIndex])
                                                selectedIndex = count;
                                        }
                                        else
                                        {
                                            // select the first that has a file
                                            if (season[DBOnlineSeries.cHasLocalFiles] != 0 && season[DBSeason.cUnwatchedItems] != 0 && selectedIndex == -1)
                                                selectedIndex = count;
                                        }
                                        if (m_back_up_select_this != null && season != null && selectedIndex == -1 && season[DBSeason.cSeriesID] == m_back_up_select_this[0] && season[DBSeason.cIndex] == m_back_up_select_this[1])
                                            selectedIndex = count;
                                    }
                                    else
                                    {
                                        // since onseasonselected won't be triggered automatically, we have to force it
                                        Season_OnItemSelected(item);
                                        selectedIndex = 0;
                                    }

                                    if (bg.CancellationPending) return;
                                    else
                                    {
                                        ReportFacadeLoadingProgress(BackGroundLoadingArgumentType.FullElement, count, item);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    MPTVSeriesLog.Write("The 'LoadFacade' function has generated an error displaying season list item: " + ex.Message);
                                }
                                count++;
                            }

                            // if there is only one season to display, skip directly to the episodes list
                            if (skipSeasonIfOne_DirectionDown && seasons.Count == 1)
                            {
                                MPTVSeriesLog.Write("Skipping season display (down)", MPTVSeriesLog.LogLevel.Debug);
                                ReportFacadeLoadingProgress(BackGroundLoadingArgumentType.SkipSeasonDown, 0, null);
                                //bg.ReportProgress(0, (int)SkipSeasonCodes.SkipSeasonDown);
                            }
                            else if (seasons.Count == 1)
                            {
                                // we're back from the ep list, go up one hierarchy more (depending on view, most likly series)
                                MPTVSeriesLog.Write("Skipping season display (up)", MPTVSeriesLog.LogLevel.Debug);
                                ReportFacadeLoadingProgress(BackGroundLoadingArgumentType.SkipSeasonUp, 0, null);
                                //bg.ReportProgress(0, (int)SkipSeasonCodes.SkipSeasonUp);
                            }
                        }
                        break;
                    #endregion
                    #region Episode
                    case Listlevel.Episode:
                        {
                            bool bFindNext = false;
                            ReportFacadeLoadingProgress(BackGroundLoadingArgumentType.SetFacadeMode, 0, GUIFacadeControl.ViewMode.List);
                            List<DBEpisode> episodesToDisplay = m_CurrLView.getEpisodeItems(m_CurrViewStep, m_stepSelection);
                            MPTVSeriesLog.Write(string.Format("Displaying {0} episodes from {1}", episodesToDisplay.Count.ToString(), m_SelectedSeries), MPTVSeriesLog.LogLevel.Normal);
                            item = null;

							if (episodesToDisplay.Count == 0)							
								bFacadeEmpty = true;							

                            foreach (DBEpisode episode in episodesToDisplay)
                            {
                                try
                                {
                                    //bEmpty = false;
                                    item = new GUIListItem();

                                    // its possible the user never selected a series/season (flat view)
                                    // thus its desirable to display series and season index also)

                                    if (!m_CurrLView.stepHasSeriesBeforeIt(m_CurrViewStep))
                                    {
                                        DBSeries corrSeries = null;
                                        // it is the case
                                        if ((corrSeries = cache.getSeries(episode[DBEpisode.cSeriesID])) == null)
                                        {
                                            corrSeries = DBSeries.Get(episode[DBEpisode.cSeriesID]);
                                            cache.addChangeSeries(corrSeries);
                                        }
                                        if (corrSeries == null)
                                        {
                                            item.Label = FieldGetter.resolveDynString(m_sFormatEpisodeCol2, episode);
                                        }
                                        else
                                        {
                                            item.Label = corrSeries[DBOnlineSeries.cPrettyName] + " - " + FieldGetter.resolveDynString(m_sFormatEpisodeCol2, episode);
                                        }

                                    }
                                    else
                                    {
                                        // we came from series on top, only display index/title
                                        item.Label = FieldGetter.resolveDynString(m_sFormatEpisodeCol2, episode);
                                    }

                                    item.Label2 = FieldGetter.resolveDynString(m_sFormatEpisodeCol3, episode);
                                    item.Label3 = FieldGetter.resolveDynString(m_sFormatEpisodeCol1, episode);
                                    
                                    // File is local
                                    item.IsRemote = episode[DBEpisode.cFilename].ToString().Length > 0;
                                    // Unwatched
                                    item.IsDownloading = episode[DBOnlineEpisode.cWatched] == 0;                                    
                                    
                                    item.TVTag = episode;

                                    if (this.m_SelectedEpisode != null)
                                    {
                                        if (episode[DBEpisode.cCompositeID] == this.m_SelectedEpisode[DBEpisode.cCompositeID])
                                        {
                                            if (!episode[DBOnlineEpisode.cWatched])
                                            {
                                                //-- video has not been watched so keep it selected
                                                selectedIndex = count;
                                            }
                                            else
                                            {
                                                //-- move to the next unwatched video in the list
                                                bFindNext = true;
                                                selectedIndex = count;
                                            }
                                        }
                                        else if (bFindNext && !episode[DBOnlineEpisode.cWatched])
                                        {
                                            selectedIndex = count;
                                            bFindNext = false;
                                        }
                                    }
                                    else
                                    {
                                        // select the first that has a file and is not watched
                                        if (selectedIndex == -1 && episode[DBOnlineEpisode.cWatched] == 0 && episode[DBEpisode.cFilename].ToString().Length > 0)
                                            selectedIndex = count;
                                    }

                                    // show watched flag image if skin supports it
                                    // this should take precedence over least used option for appending logo/ep thumb
                                    bool bWatched = episode[DBOnlineEpisode.cWatched];
                                    bool bAvailable = episode[DBEpisode.cFilename].ToString().Length > 0;

                                    if (!LoadWatchedFlag(item, bWatched, bAvailable))
                                    {
                                        if (DBOption.GetOptions(DBOption.cAppendFirstLogoToList))
                                        {
                                            // first returned logo should also show up here in list view directly
                                            item.IconImage = localLogos.getFirstEpLogo(episode);
                                        }
                                    }

                                    if (bg.CancellationPending) return;
                                    else
                                    {
                                        ReportFacadeLoadingProgress(BackGroundLoadingArgumentType.FullElement, count, item);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MPTVSeriesLog.Write("The 'LoadFacade' function has generated an error displaying episode list item: " + ex.Message);
                                }
                                count++;
                            }

                        }
                        MPTVSeriesLog.Write("LoadFacade: Finish", MPTVSeriesLog.LogLevel.Debug);
                        break;
                    #endregion
                }

                #region Report ItemToAutoSelect
                if (selectedIndex != -1)
                    ReportFacadeLoadingProgress(BackGroundLoadingArgumentType.ElementSelection, selectedIndex, null);
                else ReportFacadeLoadingProgress(BackGroundLoadingArgumentType.ElementSelection, (selectedIndex = 0), null); // select the first by default

                #endregion

                PerfWatcher.GetNamedWatch("FacadeLoading getting/reporting items").Stop();

                #region Delayed Image Loading
                if (delayedImageLoading && seriesList != null)
                {
                    PerfWatcher.GetNamedWatch("FacadeLoading BG Thread - Del. Img Loading").Start();
                    // This is a perfect oportunity to use all cores on the machine
                    // we queue each image up to be loaded, resize and put them into memory in parallel
                    // on my dual core dev. machine this saves about 40%, but it heavily depends on the no. of images
                    // and img sizes the user has selected in config
                    int done = 0;                   // we need to know later when all threads are done
                    ThreadPool.SetMinThreads(8, 8); // seems to default to 2 (avail. cores?)
                    try {
                        // we know which one was selected, lets be smart and try to first load those around it
                        Helper.ProximityForEach(seriesList, selectedIndex, delegate(DBSeries series, int currIndex) {
                            if (!bg.CancellationPending) {
                                // now foreach series, queue up the banner loading in the threadpool
                                ThreadPool.QueueUserWorkItem(delegate(object state) {
                                    string img = string.Empty;
                                    
                                    // Load Series Banners if WideBanners otherwise load Posters for Filmstrip
                                    if (DBOption.GetOptions(DBOption.cView_Series_ListFormat) == "Filmstrip")
                                        img = ImageAllocator.GetSeriesPoster(series);
                                    else
                                        img = ImageAllocator.GetSeriesBanner(series);

                                    ReportFacadeLoadingProgress(BackGroundLoadingArgumentType.DelayedImgLoading, currIndex, img);
                                    Interlocked.Increment(ref done);
                                });
                            }
                            else done++;
                        });

                    }
                    catch (Exception exs) {
                        MPTVSeriesLog.Write("Delayed ImgLoad Exception: " + exs.Message);
                    }

                    // we now need to wait until all are done, because we are already on a different thread
                    // and the workitems themselves call our bg worker's progresschanged method to display the imgs
                    // on the gui's thread, and if we exit to early we cannot do that
                    while (done < seriesList.Count) // let's hope we don't get an exception in a background thread or we will never finish
                        Thread.Sleep(15);           // this no. can use some tweaking
                    
                    PerfWatcher.GetNamedWatch("FacadeLoading BG Thread - Del. Img Loading").Stop();
                }
                #endregion
            }
            catch (Exception e)
            {
                MPTVSeriesLog.Write("The 'LoadFacade' function has generated an error: " + e.Message);
            }
        }

        private bool LoadWatchedFlag(GUIListItem item, bool bWatched, bool bAvailable)
        {          
            // Series & Season List Images
            string sListFilename = string.Empty;
            if (this.listLevel == Listlevel.Series)
                sListFilename = GUIGraphicsContext.Skin + @"\Media\tvseries_SeriesListIcon.png";
            if (this.listLevel == Listlevel.Season)
                sListFilename = GUIGraphicsContext.Skin + @"\Media\tvseries_SeasonListIcon.png";

            if (this.listLevel == Listlevel.Series || this.listLevel == Listlevel.Season)
            {
                if (!(System.IO.File.Exists(sListFilename)))
                    return false;
                item.IconImage = sListFilename;
                return true;
            }

            // Episode List Images

            // Available (Files are Local) Images
            string sWatchedFilename = GUIGraphicsContext.Skin + @"\Media\tvseries_Watched.png";
            string sUnWatchedFilename = GUIGraphicsContext.Skin + @"\Media\tvseries_UnWatched.png";

            // Not Available (Files are not Local) Images
            string sWatchedNAFilename = GUIGraphicsContext.Skin + @"\Media\tvseries_WatchedNA.png";
            string sUnWatchedNAFilename = GUIGraphicsContext.Skin + @"\Media\tvseries_UnWatchedNA.png";

            // return if images dont exists
            if (!(System.IO.File.Exists(sWatchedFilename) &&
                  System.IO.File.Exists(sUnWatchedFilename) &&
                  System.IO.File.Exists(sWatchedNAFilename) &&
                  System.IO.File.Exists(sUnWatchedNAFilename)))
                return false;

            if (bWatched)
            {
                // Load watched flag image                                
                if (!bAvailable)
                {
                    // Load alternative image
                    item.IconImage = sWatchedNAFilename;
                }
                else
                    item.IconImage = sWatchedFilename;
            }
            else
            {
                // Load un-watched flag image                
                if (!bAvailable)
                {
                    // Load alternative image
                    item.IconImage = sUnWatchedNAFilename;
                }
                else
                    item.IconImage = sUnWatchedFilename;
            }
            return true;
        }

        void ReportFacadeLoadingProgress(BackGroundLoadingArgumentType type, int indexArgument, object state)
        {
            if (!bg.CancellationPending)
            {
                BackgroundFacadeLoadingArgument Arg = new BackgroundFacadeLoadingArgument();
                Arg.Type = type;
                Arg.IndexArgument = indexArgument;
                Arg.Argument = state;

                bg.ReportProgress(0, Arg);
            }
        }

        // triggered when a selection change was made on the facade
        private void onFacadeItemSelected(GUIListItem item, GUIControl parent)
        {
            // if this is not a message from the facade, exit
            if (parent != m_Facade && parent != m_Facade.FilmstripView &&
                parent != m_Facade.ThumbnailView && parent != m_Facade.ListView)
                return;

            switch (this.listLevel)
            {
                case Listlevel.Group:
                    Group_OnItemSelected(m_Facade.SelectedListItem);
                    break;
                case Listlevel.Series:
                    Series_OnItemSelected(m_Facade.SelectedListItem);
                    break;

                case Listlevel.Season:
                    Season_OnItemSelected(m_Facade.SelectedListItem);
                    break;

                case Listlevel.Episode:
                    Episode_OnItemSelected(m_Facade.SelectedListItem);
                    break;
            }
        }

        protected override void OnPageLoad()
        {
            if (m_Facade == null) // wrong skin file
            {
                GUIDialogOK dlg = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                dlg.Reset();
                dlg.SetHeading(Translation.wrongSkin);
                dlg.DoModal(GetID);
                GUIWindowManager.ShowPreviousWindow();
                return;
            }
            ImageAllocator.SetFontName(m_Facade.AlbumListView == null ? m_Facade.ListView.FontName : m_Facade.AlbumListView.FontName);

            // For some reason on non initial loads (such as coming back from fullscreen video or after having exited to home and coming back)
            // the labels don't display, unless we somehow call them like so
            // (no, allocResources doesnt work!)
            clearGUIProperty(guiProperty.Subtitle);
            clearGUIProperty(guiProperty.Title);
            clearGUIProperty(guiProperty.Description);

            clearGUIProperty(guiProperty.CurrentView);
            clearGUIProperty(guiProperty.SimpleCurrentView);
            clearGUIProperty(guiProperty.NextView);
            clearGUIProperty(guiProperty.LastView);

            if (m_CurrLView == null)
            {
                localLogos.appendEpImage = m_Episode_Image == null ? true : false;
                // get views
                m_allViews = logicalView.getAll(false); // hardcoded until configuration is set up!
                if (m_allViews.Count > 0)
                {
                    try { switchView((string)DBOption.GetOptions("lastView")); }
                    catch (Exception) { }
                }
                else MPTVSeriesLog.Write("Error, cannot display items because: No Views have been found!");
            }
            else 
                setViewLabels();

            backdrop.GUIImageOne = FanartBackground;
            backdrop.GUIImageTwo = FanartBackground2;
            backdrop.LoadingImage = loadingImage;

            LoadFacade();
            m_Facade.Focus = true;

            // Update Button Labels with translations
            if (viewMenuButton != null)
                viewMenuButton.Label = Translation.ButtonSwitchView;

            if (ImportButton != null)
                ImportButton.Label = Translation.ButtonRunImport;

            if (LayoutMenuButton != null)
                LayoutMenuButton.Label = Translation.ButtonChangeLayout;

            if (OptionsMenuButton != null)
                OptionsMenuButton.Label = Translation.ButtonOptions;

            setProcessAnimationStatus(m_parserUpdaterWorking);
            MediaPortal.GUI.Library.GUIPropertyManager.SetProperty("#currentmodule", pluginName);

            if (m_Logos_Image != null)
            {
                logosHeight = m_Logos_Image.Height;
                logosWidth = m_Logos_Image.Width;
            }
          
        }

        protected override void OnPageDestroy(int new_windowId)
        {
            // Disable Random Fanart Timer
            m_FanartTimer.Change(Timeout.Infinite, Timeout.Infinite);
            m_bFanartTimerDisabled = true;

            base.OnPageDestroy(new_windowId);
        }

        enum guiProperty
        {
            Title,
            Subtitle,
            Description,
            CurrentView,
            SimpleCurrentView,
            NextView,
            LastView,
            SeriesBanner,
            SeriesPoster,
            SeasonBanner,
            EpisodeImage,
            Logos,
            SeriesCount,
            GroupCount
        }

        string getGUIProperty(guiProperty which)
        {
          return getGUIProperty(which.ToString());
        }

        public static string getGUIProperty(string which)
        {
          return MediaPortal.GUI.Library.GUIPropertyManager.GetProperty("#TVSeries." + which);
        }

        void setGUIProperty(guiProperty which, string value)
        {
            setGUIProperty(which.ToString(), value);
        }

        public static void setGUIProperty(string which, string value)
        {
            MediaPortal.GUI.Library.GUIPropertyManager.SetProperty("#TVSeries." + which, value);
        }

        void clearGUIProperty(guiProperty which)
        {
            setGUIProperty(which, string.Empty);
        }

        public static void clearGUIProperty(string which)
        {
            setGUIProperty(which, "-"); // String.Empty doesn't work on non-initialized fields, as a result they would display as ugly #TVSeries.bla.bla
        }

        enum eContextItems
        {
            toggleWatched,
            cycleSeriesBanner,
            cycleSeriesPoster,
            cycleSeasonBanner,
            forceSeriesQuery,
            downloadSubtitle,
            downloadviaTorrent,
            downloadviaNewz,
            actionMarkAllWatched,
            actionMarkAllUnwatched,
            actionToggleFavorite,
            actionHide,
            actionDelete,
            actionLocalScan,
            actionFullRefresh,
            actionPlayRandom,
            optionsOnlyShowLocal,
            optionsPreventSpoilers,
            optionsPreventSpoilerThumbnail,
            optionsAskToRate,
            optionsFastViewSwitch,
            optionsFanartRandom,
            optionsSeriesFanart,
            optionsUseOnlineFavourites,
            actionRecheckMI,
            resetUserSelections,
            showFanartChooser,
            addToPlaylist,			
			viewAddToNewView
        }

        enum eContextMenus
        {
            download = 100,
            action,
            options,
            rate,
            switchView,
            switchLayout,
			addToView,
			removeFromView
        }

        enum DeleteMenuItems {
            disk,
            database,
            diskdatabase,
            cancel
        }

        internal static void showRatingsDialog(DBTable item, bool auto)
        {
            if (item == null) return;
            MPTVSeriesLog.Write("Asking to rate", MPTVSeriesLog.LogLevel.Debug);					

			GUIListItem pItem = null;
			Listlevel level = item is DBEpisode ? Listlevel.Episode : Listlevel.Series;

			string value = "0";
			string dlgHeading = (level == Listlevel.Episode ? Translation.RateEpisode : Translation.RateSeries);

			if (System.IO.File.Exists(GUIGraphicsContext.Skin + @"\TVSeries.RatingDialog.xml")) {
				GUIUserRating ratingDlg = (GUIUserRating)GUIWindowManager.GetWindow(GUIUserRating.ID);
				ratingDlg.Reset();
				ratingDlg.SetHeading((level == Listlevel.Episode ? Translation.RateEpisode : Translation.RateSeries));
				if (level == Listlevel.Series) {
					ratingDlg.SetLine(1, string.Format(Translation.RateDialogLabel, item.ToString()));
				} 
				else {
					ratingDlg.SetLine(1, string.Format(Translation.RateDialogLabel, Translation.Episode));
					ratingDlg.SetLine(2, item.ToString());
				}

				if (DBOption.GetOptions(DBOption.cRatingDisplayStars) == 10) {
					ratingDlg.DisplayStars = GUIUserRating.StarDisplay.TEN_STARS;
					ratingDlg.Rating = DBOption.GetOptions(DBOption.cDefaultRating);
				} 
				else {
					ratingDlg.DisplayStars = GUIUserRating.StarDisplay.FIVE_STARS;
					ratingDlg.Rating = (int)(DBOption.GetOptions(DBOption.cDefaultRating) / 2);
				}
				
				ratingDlg.DoModal(ratingDlg.GetID);
				if (ratingDlg.IsSubmitted) {
					if (ratingDlg.DisplayStars == GUIUserRating.StarDisplay.FIVE_STARS)
						value = (ratingDlg.Rating * 2).ToString();
					value = ratingDlg.Rating.ToString();
				} 
				else return;
			} 
			else {
				IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
				dlg.Reset();
				dlg.SetHeading(dlgHeading + ": " + item.ToString());

				pItem = new GUIListItem(Translation.ResetRating);
				dlg.Add(pItem);
				pItem.ItemId = 0;

				pItem = new GUIListItem("1 " + Translation.RatingStar);
				dlg.Add(pItem);
				pItem.ItemId = 1;

				for (int i = 2; i < 11; i++) {
					pItem = new GUIListItem(i.ToString() + " " + Translation.RatingStars);
					dlg.Add(pItem);
					pItem.ItemId = i;
				}

				if (auto) {
					pItem = new GUIListItem(Translation.DontAskToRate);
					dlg.Add(pItem);
					pItem.ItemId = 11;
				}

				dlg.DoModal(GUIWindowManager.ActiveWindow);

				if (dlg.SelectedId == -1 || dlg.SelectedId > 11) return; // cancelled
				if (dlg.SelectedId == 11 && auto) {
					DBOption.SetOptions(DBOption.cAskToRate, false);
					return;
				}
				value = dlg.SelectedId.ToString();
			}
			
			string type = (level == Listlevel.Episode ? DBOnlineEpisode.cMyRating : DBOnlineSeries.cMyRating);
			string id = item[level == Listlevel.Episode ? DBOnlineEpisode.cID : DBOnlineSeries.cID];
			
			// Submit rating online database if current rating is different
			if (!Helper.String.IsNullOrEmpty(value) && value != item[type])
			{
				int rating = -1;
				if (Int32.TryParse(value, out rating))
				{
					Online_Parsing_Classes.OnlineAPI.SubmitRating(level == Listlevel.Episode ? Online_Parsing_Classes.OnlineAPI.RatingType.episode : Online_Parsing_Classes.OnlineAPI.RatingType.series, id, rating);
				}
			}

			// Apply to local database
			item[type] = value;
			// Set the all user rating if not already set                
			if (item[level == Listlevel.Episode ? DBOnlineEpisode.cRating : DBOnlineSeries.cRating] == "")
				item[level == Listlevel.Episode ? DBOnlineEpisode.cRating : DBOnlineSeries.cRating] = value;

			item.Commit();
			
        }

        internal bool showViewSwitchDialog()
        {
            IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            dlg.Reset();
            dlg.SetHeading(Translation.ChangeView);

            int counter = 0;
            int preSelect = 0;
            foreach (logicalView view in m_allViews)
            {
                GUIListItem pItem = new GUIListItem(view.prettyName);
                if (view.Equals(this.m_CurrLView))
                    preSelect = counter;
                dlg.Add(pItem);
                pItem.ItemId = counter++;
            }

            dlg.DoModal(GUIWindowManager.ActiveWindow);
            if (dlg.SelectedId >= 0 && !m_allViews[dlg.SelectedId].Equals(m_CurrLView))
            {
                switchView(m_allViews[dlg.SelectedId]);
                LoadFacade();
                return true;
            }
            return false;
        }

        internal void ShowOptionsMenu()
        {
            IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlg == null) return;

            dlg.Reset();
            dlg.SetHeading(Translation.Options);

            GUIListItem pItem = new GUIListItem(Translation.Only_show_episodes_with_a_local_file + " (" + (DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles) ? Translation.on : Translation.off) + ")");
            dlg.Add(pItem);
            pItem.ItemId = (int)eContextItems.optionsOnlyShowLocal;

            pItem = new GUIListItem(Translation.Hide_summary_on_unwatched + " (" + (DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedSummary) ? Translation.on : Translation.off) + ")");
            dlg.Add(pItem);
            pItem.ItemId = (int)eContextItems.optionsPreventSpoilers;

            pItem = new GUIListItem(Translation.Hide_thumbnail_on_unwatched + " (" + (DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedThumbnail) ? Translation.on : Translation.off) + ")");
            dlg.Add(pItem);
            pItem.ItemId = (int)eContextItems.optionsPreventSpoilerThumbnail;

            if (!Helper.String.IsNullOrEmpty(DBOption.GetOptions(DBOption.cOnlineUserID)))
            {
                pItem = new GUIListItem(Translation.AskToRate + " (" + (DBOption.GetOptions(DBOption.cAskToRate) ? Translation.on : Translation.off) + ")");
                dlg.Add(pItem);
                pItem.ItemId = (int)eContextItems.optionsAskToRate;
            }

            if (FanartBackground != null)
            {
                pItem = new GUIListItem(Translation.FanArtRandom + " (" + (DBOption.GetOptions(DBOption.cFanartRandom) ? Translation.on : Translation.off) + ")");
                dlg.Add(pItem);
                pItem.ItemId = (int)eContextItems.optionsFanartRandom;

                pItem = new GUIListItem(Translation.ShowSeriesFanart + " (" + (DBOption.GetOptions(DBOption.cShowSeriesFanart) ? Translation.on : Translation.off) + ")");
                dlg.Add(pItem);
                pItem.ItemId = (int)eContextItems.optionsSeriesFanart;
            }

            //pItem = new GUIListItem(Translation.UseOnlineFavourites + " (" + (DBOption.GetOptions(DBOption.cOnlineFavourites) ? Translation.on : Translation.off) + ")");
            //dlg.Add(pItem);
            //pItem.ItemId = (int)eContextItems.optionsUseOnlineFavourites;

            dlg.DoModal(GUIWindowManager.ActiveWindow);
            if (dlg.SelectedId >= 0)
            {
                switch (dlg.SelectedId)
                {
                    case (int)eContextItems.optionsOnlyShowLocal:
                        DBOption.SetOptions(DBOption.cView_Episode_OnlyShowLocalFiles, !DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles));
                        // Update Episode counts...this is going to be expensive                        
                        SQLCondition condEmpty = new SQLCondition();
                        List<DBSeries> AllSeries = DBSeries.Get(condEmpty);
                        foreach (DBSeries series in AllSeries)
                            DBSeries.UpdatedEpisodeCounts(series);
                        LoadFacade();
                        break;

                    case (int)eContextItems.optionsPreventSpoilers:
                        DBOption.SetOptions(DBOption.cView_Episode_HideUnwatchedSummary, !DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedSummary));
                        LoadFacade();
                        break;
                    case (int)eContextItems.optionsPreventSpoilerThumbnail:
                        DBOption.SetOptions(DBOption.cView_Episode_HideUnwatchedThumbnail, !DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedThumbnail));
                        LoadFacade();
                        break;
                    case (int)eContextItems.optionsFastViewSwitch:
                        DBOption.SetOptions(DBOption.cswitchViewsFast, !DBOption.GetOptions(DBOption.cswitchViewsFast));
                        break;
                    case (int)eContextItems.optionsAskToRate:
                        DBOption.SetOptions(DBOption.cAskToRate, !DBOption.GetOptions(DBOption.cAskToRate));
                        break;
                    case (int)eContextItems.optionsFanartRandom:                        
                        DBOption.SetOptions(DBOption.cFanartRandom, !DBOption.GetOptions(DBOption.cFanartRandom));
                        if (!DBOption.GetOptions(DBOption.cFanartRandom)) {
                            // Disable Timer
                            m_FanartTimer.Change(Timeout.Infinite, Timeout.Infinite);
                            m_bFanartTimerDisabled = true;
                        }
                        // Update Fanart Displayed - may need to restore default artwork
                        Series_OnItemSelected(m_Facade.SelectedListItem);

                        break;
                    case (int)eContextItems.optionsSeriesFanart:
                        DBOption.SetOptions(DBOption.cShowSeriesFanart, !DBOption.GetOptions(DBOption.cShowSeriesFanart));
                        if (this.listLevel == Listlevel.Series)
                        {
                            if (DBOption.GetOptions(DBOption.cShowSeriesFanart))
                            {
                                Series_OnItemSelected(m_Facade.SelectedListItem);
                            }
                            else
                                DisableFanart();
                        }
                        break;
                    case (int)eContextItems.optionsUseOnlineFavourites:
                        DBOption.SetOptions(DBOption.cOnlineFavourites, !DBOption.GetOptions(DBOption.cOnlineFavourites));
                        // Update Views                        
                        DBView view = new DBView(1);
                        if (!DBOption.GetOptions(DBOption.cOnlineFavourites))
                        {
                            view[DBView.cViewConfig] = @"series<;><Series.isFavourite>;=;1<;><;>" +
                                                "<nextStep>season<;><;><Season.seasonIndex>;asc<;>" +
                                                "<nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                        }
                        else
                        {
                            view[DBView.cViewConfig] = @"series<;><Series.isOnlineFavourite>;=;1<;><;>" +
                                                "<nextStep>season<;><;><Season.seasonIndex>;asc<;>" +
                                                "<nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                        }
                        view.Commit();
                        LoadFacade();
                        break;
                }
            }
        }

        #region View Tags Context Menu
        private void ShowViewTagsMenu(bool add, DBSeries series) {
            IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null)
                return;
            
            dlg.Reset();
            string sDialogHeading = (add ? Translation.AddViewTag : Translation.RemoveViewTag);
            dlg.SetHeading(sDialogHeading);
     
            GUIListItem pItem = null;
            DBView[] views = DBView.getTaggedViews();

            string currTags = series[DBOnlineSeries.cViewTags];
            String[] splitTags = currTags.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            // Add Menu items
            if (add) {
                // List available tag views, no need for translation
                // these are user created strings                
                foreach (DBView view in views) {
                    string viewname = view[DBView.cPrettyName];
                    // We dont want to add it to the list if its already a member                    
                    if (!currTags.Contains("|" + viewname + "|")) {
                        pItem = new GUIListItem(viewname);
                        dlg.Add(pItem);
                        pItem.ItemId = view[DBView.cIndex];
                    }
                }

                pItem = new GUIListItem(Translation.NewViewTag + " ...");
                dlg.Add(pItem);
                pItem.ItemId = (int)eContextItems.viewAddToNewView;
            }
            else {
                // we list all the tags that current series is a member of
                int iItemIdx = 1000;
                foreach (String tag in splitTags) {
                    pItem = new GUIListItem(tag);
                    dlg.Add(pItem);
                    pItem.ItemId = iItemIdx++;                    
                }                
            }

            // Show Menu
            dlg.DoModal(GUIWindowManager.ActiveWindow);
            if (dlg.SelectedId < 0 || dlg.SelectedId == (int)DeleteMenuItems.cancel)
                return;

            #region Add a New Tagged View
            string selectedItem = string.Empty;
            if (dlg.SelectedId == (int)eContextItems.viewAddToNewView) {
                GetStringFromUserDescriptor GetStringDesc = new GetStringFromUserDescriptor();                                
                GetStringDesc.m_sText = string.Empty;                
                bool viewExists = true;

                while (viewExists) {
                    if (this.GetStringFromUser(GetStringDesc, out selectedItem) == ReturnCode.OK) {
                        // Create View if it doesnt exist
                        viewExists = false;
                        foreach (DBView view in views) {
                            if (selectedItem.Equals(view[DBView.cPrettyName], StringComparison.CurrentCultureIgnoreCase)) {
                                ShowViewExistsMessage(selectedItem);
                                viewExists = true;
                                break;
                            }
                        }
                    }
                    else
                        return;
                }
                if (selectedItem.Length == 0)
                    return;

                // Add New View to database
                // Ensure index is unique...assumes index is updated when deleting views                
                int index = logicalView.getAll(true).Count;
                string config = @"series<;><Series.ViewTags>;like;%|" + selectedItem + "|%<;><;>" +
                                 "<nextStep>season<;><;><Season.seasonIndex>;asc<;>" +
                                 "<nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";

                MPTVSeriesLog.Write(string.Format("Creating New Tagged View: {0}", selectedItem));
                DBView.AddView(index, selectedItem, config, true);
            }
            #endregion

            string newTags = string.Empty;

            // Get Selected Item if not a new Tag
            if (dlg.SelectedId != (int)eContextItems.viewAddToNewView)
                selectedItem = dlg.SelectedLabelText;

            // Add/Remove Tag to/from Series
            bool tagExists = false;                                   

            if (add) {
                MPTVSeriesLog.Write(string.Format("Adding new Tag \"{0}\" to series: {1}", selectedItem, series.ToString()));

                if (Helper.String.IsNullOrEmpty(currTags)) {
                    newTags = "|" + selectedItem + "|";
                }
                else {
                    // Check if the series is already tagged with selected series                    
                    foreach (String tag in splitTags) {
                        if (tag.Equals(selectedItem, StringComparison.CurrentCultureIgnoreCase)) {
                            tagExists = true;
                            newTags = currTags;
                            break;
                        }
                    }
                    // Add tag to series if it doesnt exist
                    if (!tagExists) 
                        newTags = currTags + selectedItem + "|";                    
                }
            }
            else {
                MPTVSeriesLog.Write(string.Format("Removing Tag \"{0}\" from series: {1}", selectedItem, series.ToString()));
     
                foreach (String tag in splitTags) {
                    if (!tag.Equals(selectedItem, StringComparison.CurrentCultureIgnoreCase))
                        newTags += "|" + tag;
                }
                if (!Helper.String.IsNullOrEmpty(newTags)) 
                    newTags += "|";                               
            }
            
            // Update Main View List
            m_allViews = logicalView.getAll(false);

            // Commit changes to database
            series[DBOnlineSeries.cViewTags] = newTags;
            series.Commit();

            // Load Facade to reflect changes
            // We only need to reload when removing from a View that is active
            if (!add) {
                if (m_CurrLView.prettyName.Equals(selectedItem, StringComparison.CurrentCultureIgnoreCase) || 
                    m_CurrLView.gettypeOfStep(0) == logicalViewStep.type.group)
                    LoadFacade();
            }
        }

        private void ShowViewExistsMessage(string view) {
            GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            dlgOK.SetHeading(Translation.ViewTags);
            dlgOK.SetLine(1, string.Format(Translation.ViewTagExistsMessage,view));
            dlgOK.DoModal(GUIWindowManager.ActiveWindow);
        }
        #endregion

        #region Delete Context Menu
        private void ShowDeleteMenu(DBSeries series, DBSeason season, DBEpisode episode) {            
            String sDialogHeading = String.Empty;

            switch (this.listLevel) {
                case Listlevel.Series:
                    sDialogHeading = Translation.Delete_that_series;
                    break;

                case Listlevel.Season:
                    sDialogHeading = Translation.Delete_that_season;
                    break;

                case Listlevel.Episode:
                    sDialogHeading = Translation.Delete_that_episode;
                    break;
            }

            IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null) 
                return;

            dlg.Reset();
            dlg.SetHeading(sDialogHeading);

            // Add Menu items
            GUIListItem pItem = null;

            pItem = new GUIListItem(Translation.DeleteFromDisk);          
            dlg.Add(pItem);
            pItem.ItemId = (int)DeleteMenuItems.disk;

            pItem = new GUIListItem(Translation.DeleteFromDatabase);            
            dlg.Add(pItem);
            pItem.ItemId = (int)DeleteMenuItems.database;
            
            pItem = new GUIListItem(Translation.DeleteFromFileDatabase);            
            dlg.Add(pItem);
            pItem.ItemId = (int)DeleteMenuItems.diskdatabase;

            pItem = new GUIListItem(Translation.Cancel);            
            dlg.Add(pItem);
            pItem.ItemId = (int)DeleteMenuItems.cancel;

            // Show Menu
            dlg.DoModal(GUIWindowManager.ActiveWindow);
            if (dlg.SelectedId < 0 || dlg.SelectedId == (int)DeleteMenuItems.cancel) 
				return;

            List<DBEpisode> epsDeletion = new List<DBEpisode>();
			List<DBEpisode> episodes = new List<DBEpisode>();
            SQLCondition condition = null;

            switch (this.listLevel) {
                case Listlevel.Series:
                    // Always delete from Local episode/series table if deleting from disk or database
                    condition = new SQLCondition();
                    condition.Add(new DBEpisode(), DBEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);                    

                    if (dlg.SelectedId != (int)DeleteMenuItems.database)                           
                        epsDeletion.AddRange(DBEpisode.Get(condition, false));
										
					DBEpisode.Clear(condition);

                    condition = new SQLCondition();
                    condition.Add(new DBSeries(), DBSeries.cID, series[DBSeries.cID], SQLConditionType.Equal);
                    DBSeries.Clear(condition);

                    // Delete from online episode table
                    if (dlg.SelectedId != (int)DeleteMenuItems.disk) {
                        condition = new SQLCondition();
                        condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                        DBOnlineEpisode.Clear(condition);

                        condition = new SQLCondition();
                        condition.Add(new DBSeason(), DBSeason.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                        DBSeason.Clear(condition);

                        condition = new SQLCondition();
                        condition.Add(new DBOnlineSeries(), DBOnlineSeries.cID, series[DBSeries.cID], SQLConditionType.Equal);
                        DBOnlineSeries.Clear(condition);
                    }
                    break;

                case Listlevel.Season:
                    // Always delete from Local episode table if deleting from disk or database
                    condition = new SQLCondition();
                    condition.Add(new DBEpisode(), DBEpisode.cSeriesID, season[DBSeason.cSeriesID], SQLConditionType.Equal);
                    condition.Add(new DBEpisode(), DBEpisode.cSeasonIndex, season[DBSeason.cIndex], SQLConditionType.Equal);
                    
                    if (dlg.SelectedId != (int)DeleteMenuItems.database)                            
                        epsDeletion.AddRange(DBEpisode.Get(condition, false));
					
					DBEpisode.Clear(condition);

                    if (dlg.SelectedId != (int)DeleteMenuItems.disk) {
                        condition = new SQLCondition();
                        condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, season[DBSeason.cSeriesID], SQLConditionType.Equal);
                        condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeasonIndex, season[DBSeason.cIndex], SQLConditionType.Equal);
                        DBOnlineEpisode.Clear(condition);

                        condition = new SQLCondition();
						condition.Add(new DBSeason(), DBSeason.cSeriesID, season[DBSeason.cSeriesID], SQLConditionType.Equal);
						condition.Add(new DBSeason(), DBSeason.cIndex, season[DBSeason.cIndex], SQLConditionType.Equal);						
                        DBSeason.Clear(condition);
					}

					#region Cleanup
					if (dlg.SelectedId != (int)DeleteMenuItems.disk) {
						// If episode count is zero then delete the series and all seasons
						condition = new SQLCondition();
						condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, season[DBSeason.cSeriesID], SQLConditionType.Equal);
						episodes = DBEpisode.Get(condition, false);
						if (episodes.Count == 0) {
							// Delete Seasons
							condition = new SQLCondition();
							condition.Add(new DBSeason(), DBSeason.cSeriesID, season[DBSeason.cSeriesID], SQLConditionType.Equal);
							DBSeason.Clear(condition);

							// Delete Local Series
							condition = new SQLCondition();
							condition.Add(new DBSeries(), DBSeries.cID, season[DBSeason.cSeriesID], SQLConditionType.Equal);
							DBSeries.Clear(condition);

							// Delete Online Series
							condition = new SQLCondition();
							condition.Add(new DBOnlineSeries(), DBOnlineSeries.cID, season[DBSeason.cSeriesID], SQLConditionType.Equal);
							DBOnlineSeries.Clear(condition);
						}
					}
					#endregion
					break;

                case Listlevel.Episode:
                    // Always delete from Local episode table if deleting from disk or database
                    condition = new SQLCondition();
                    condition.Add(new DBEpisode(), DBEpisode.cFilename, episode[DBEpisode.cFilename], SQLConditionType.Equal);                    

                    if (dlg.SelectedId != (int)DeleteMenuItems.database)                            
                        epsDeletion.AddRange(DBEpisode.Get(condition, false));

					DBEpisode.Clear(condition);

					if (dlg.SelectedId != (int)DeleteMenuItems.disk) {
						condition = new SQLCondition();
						condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cID, episode[DBOnlineEpisode.cID], SQLConditionType.Equal);
						DBOnlineEpisode.Clear(condition);
					}

					#region Cleanup
					if (dlg.SelectedId != (int)DeleteMenuItems.disk) {
						// If episode count is zero then delete the season
						condition = new SQLCondition();
						condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, episode[DBOnlineEpisode.cSeriesID], SQLConditionType.Equal);
						condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeasonIndex, episode[DBOnlineEpisode.cSeasonIndex], SQLConditionType.Equal);						
						episodes = DBEpisode.Get(condition, false);
						if (episodes.Count == 0) {
							condition = new SQLCondition();
							condition.Add(new DBSeason(), DBSeason.cSeriesID, episode[DBOnlineEpisode.cSeriesID], SQLConditionType.Equal);							
							condition.Add(new DBSeason(), DBSeason.cIndex, episode[DBOnlineEpisode.cSeasonIndex], SQLConditionType.Equal);
							DBSeason.Clear(condition);

							// If episode count is still zero, then delete the series\seasons
							condition = new SQLCondition();
							condition.Add(new DBEpisode(), DBEpisode.cSeriesID, episode[DBOnlineEpisode.cSeriesID], SQLConditionType.Equal);
							episodes = DBEpisode.Get(condition, false);
							if (episodes.Count == 0) {
								// Delete All Seasons
								condition = new SQLCondition();
								condition.Add(new DBSeason(), DBSeason.cSeriesID, episode[DBOnlineEpisode.cSeriesID], SQLConditionType.Equal);
								DBSeason.Clear(condition);

								// Delete Local Series
								condition = new SQLCondition();
								condition.Add(new DBSeries(), DBSeries.cID, episode[DBOnlineEpisode.cSeriesID], SQLConditionType.Equal);
								DBSeries.Clear(condition);

								// Delete Online Series
								condition = new SQLCondition();
								condition.Add(new DBOnlineSeries(), DBOnlineSeries.cID, episode[DBOnlineEpisode.cSeriesID], SQLConditionType.Equal);
								DBOnlineSeries.Clear(condition);
							}
						}
					}
					#endregion
					break;
            }

            if (epsDeletion.Count > 0) {
                // Delete the actual files
                List<string> files = Helper.getFieldNameListFromList<DBEpisode>(DBEpisode.cFilename, epsDeletion);                    
                foreach (string file in files) {
                    try {
                        MPTVSeriesLog.Write(string.Format("Deleting file: {0}",file));
                        System.IO.File.Delete(file);
                    }
                    catch (Exception ex) {
                        MPTVSeriesLog.Write(string.Format("Failed to delete: {0}, {1}", file, ex.Message));
                    }
                }                   
            }

            // Re-load the facade to accurately reflect actions taked above
            LoadFacade();
        }
        #endregion

        #region Main Context Menu
        protected override void OnShowContextMenu()
        {
            try
            {
                GUIListItem currentitem = this.m_Facade.SelectedListItem;
                if (currentitem == null) return;

                IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlg == null) return;

                DBEpisode selectedEpisode = null;
                DBSeason selectedSeason = null;
                DBSeries selectedSeries = null;

                bool emptyList = currentitem.Label == Translation.No_items;
                if (!emptyList)
                {
                    switch (this.listLevel)
                    {
                        case Listlevel.Episode:
                            {
                                selectedEpisode = (DBEpisode)currentitem.TVTag;
                                selectedSeason = Helper.getCorrespondingSeason(selectedEpisode[DBEpisode.cSeriesID], selectedEpisode[DBEpisode.cSeasonIndex]);
                                selectedSeries = Helper.getCorrespondingSeries(selectedEpisode[DBEpisode.cSeriesID]);
                            }
                            break;

                        case Listlevel.Season:
                            {
                                selectedSeason = (DBSeason)currentitem.TVTag;
                                selectedSeries = Helper.getCorrespondingSeries(selectedSeason[DBSeason.cSeriesID]);
                            }
                            break;

                        case Listlevel.Series:
                            {
                                selectedSeries = (DBSeries)currentitem.TVTag;
                            }
                            break;
                    }
                }
                bool bExitMenu = false;
                do
                {
                    dlg.Reset();
                    GUIListItem pItem = null;

                    bool tvSubtitlesEnable = DBOption.GetOptions(DBOption.cSubs_TVSubtitles_Enable);
                    bool seriesSubEnable = DBOption.GetOptions(DBOption.cSubs_SeriesSubs_Enable);
                    bool remositoryEnable = DBOption.GetOptions(DBOption.cSubs_Remository_Enable);
                    bool newsEnable = System.IO.File.Exists(DBOption.GetOptions(DBOption.cNewsLeecherPath));
                    bool torrentsEnable = System.IO.File.Exists(DBOption.GetOptions(DBOption.cUTorrentPath));

                    if (!emptyList)
                    {
                        switch (this.listLevel)
                        {
                            case Listlevel.Episode:
                                dlg.SetHeading(Translation.Episode + ": " + selectedEpisode[DBEpisode.cEpisodeName]);
                                break;

                            case Listlevel.Season:
                                dlg.SetHeading(Translation.Season + ": " + selectedSeason[DBSeason.cIndex]);
                                break;

                            case Listlevel.Series:
                                dlg.SetHeading(Translation.Series + ": " + selectedSeries[DBOnlineSeries.cPrettyName]);
                                break;
                            default:
                                // group
                                dlg.SetHeading(m_CurrLView.Name);
                                break;
                        }

                        #region Top Level Menu Items - Context Sensitive
                        if (this.listLevel == Listlevel.Episode)
                        {
                            pItem = new GUIListItem(Translation.Toggle_watched_flag);
                            dlg.Add(pItem);
                            pItem.ItemId = (int)eContextItems.toggleWatched;
                            
                            if (!Helper.String.IsNullOrEmpty(DBOption.GetOptions(DBOption.cOnlineUserID)))
                            {
                                pItem = new GUIListItem(Translation.RateEpisode);
                                dlg.Add(pItem);
                                pItem.ItemId = (int)eContextMenus.rate;
                            }
                        }
                        else if (this.listLevel != Listlevel.Group)
                        {
                            if (!Helper.String.IsNullOrEmpty(DBOption.GetOptions(DBOption.cOnlineUserID)))
                            {
                                pItem = new GUIListItem(Translation.RateSeries);
                                dlg.Add(pItem);
                                pItem.ItemId = (int)eContextMenus.rate;
                            }

                            pItem = new GUIListItem(Translation.Mark_all_as_watched);
                            dlg.Add(pItem);
                            pItem.ItemId = (int)eContextItems.actionMarkAllWatched;

                            pItem = new GUIListItem(Translation.Mark_all_as_unwatched);
                            dlg.Add(pItem);
                            pItem.ItemId = (int)eContextItems.actionMarkAllUnwatched;

                        }

                        // Add To Playlist is supported on all views
                        // Group:   Add all episodes for all series in selected group (TODO)
                        // Series:  Add all episodes for selected series
                        // Season:  Add all episodes for selected season
                        // Episode: Add selected episode
                        if (this.listLevel != Listlevel.Group)
                        {
                            pItem = new GUIListItem(Translation.AddToPlaylist);
                            dlg.Add(pItem);
                            pItem.ItemId = (int)eContextItems.addToPlaylist;
                        }

                        if (this.listLevel != Listlevel.Group)
                        {
                            if (m_SelectedSeries != null && FanartBackground != null && // only if skins supports it
                                m_SelectedSeries[DBOnlineSeries.cID] > 0)
                            {
                                pItem = new GUIListItem(Translation.FanArt + " ...");
                                dlg.Add(pItem);
                                pItem.ItemId = (int)eContextItems.showFanartChooser;
                            }
                        }

                        if (this.listLevel == Listlevel.Series)
                        {                      
                            if (selectedSeries.PosterList.Count > 1)
                            {
                                pItem = new GUIListItem(Translation.CycleSeriesPoster);
                                dlg.Add(pItem);
                                pItem.ItemId = (int)eContextItems.cycleSeriesPoster;
                            }
                     
                            if (selectedSeries.BannerList.Count > 1)
                            {
                                pItem = new GUIListItem(Translation.CycleSeriesBanner);
                                dlg.Add(pItem);
                                pItem.ItemId = (int)eContextItems.cycleSeriesBanner;
                            }
                     
                            pItem = new GUIListItem(Translation.Force_Online_Match);
                            dlg.Add(pItem);
                            pItem.ItemId = (int)eContextItems.forceSeriesQuery;                        
                        }

                        // Season View may not be available so show cycle season banner at episode level as well
                        if (this.listLevel == Listlevel.Season || this.listLevel == Listlevel.Episode)
                        {
                            if (selectedSeason.BannerList.Count > 1)
                            {
                                pItem = new GUIListItem(Translation.CycleSeasonBanner);
                                dlg.Add(pItem);
                                pItem.ItemId = (int)eContextItems.cycleSeasonBanner;
                            }
                        }

                        if (listLevel != Listlevel.Group)
                        {							
                            // Fav. handling
                            DBSeries currentSeries;
                            if (listLevel == Listlevel.Series)
                                currentSeries = (DBSeries)currentitem.TVTag;
                            else currentSeries = m_SelectedSeries;

                            if (!DBOption.GetOptions(DBOption.cOnlineFavourites))
                            {
                                pItem = new GUIListItem(currentSeries[DBOnlineSeries.cIsFavourite] == 1 ? Translation.Remove_series_from_Favourites : Translation.Add_series_to_Favourites);
                            }
                            else
                                pItem = new GUIListItem(currentSeries[DBOnlineSeries.cIsOnlineFavourite] == 1 ? Translation.Remove_series_from_Favourites : Translation.Add_series_to_Favourites);

                            dlg.Add(pItem);
                            pItem.ItemId = (int)eContextItems.actionToggleFavorite;
                        }

                        // Can always add to existing or new view
                        if (listLevel == Listlevel.Series) {
                            pItem = new GUIListItem(Translation.AddViewTag + " ...");
                            dlg.Add(pItem);
                            pItem.ItemId = (int)eContextMenus.addToView;
                        }
                        // Dont show if not a member of any view
                        if (listLevel == Listlevel.Series) {
                            if (!Helper.String.IsNullOrEmpty(selectedSeries[DBOnlineSeries.cViewTags])) {
                                pItem = new GUIListItem(Translation.RemoveViewTag + " ...");
                                dlg.Add(pItem);
                                pItem.ItemId = (int)eContextMenus.removeFromView;
                            }
                        }                        

                        if (tvSubtitlesEnable || seriesSubEnable || remositoryEnable || newsEnable || torrentsEnable) {
                            if (listLevel != Listlevel.Group) {
                                if (this.listLevel == Listlevel.Episode) {
                                    pItem = new GUIListItem(Translation.Download + " ...");
                                    dlg.Add(pItem);
                                    pItem.ItemId = (int)eContextMenus.download;
                                }
                            }
                        }

                        #endregion
                    }
                    else 
						dlg.SetHeading(m_CurrLView.Name);

                    #region Top Level Menu Items - Non-Context Sensitive
                    pItem = new GUIListItem(Translation.ChangeView + " ...");
                    dlg.Add(pItem);
                    pItem.ItemId = (int)eContextMenus.switchView;
                    
                    if (SkinSettings.GetLayoutCount(this.listLevel.ToString()) > 1)
                    {
                        pItem = new GUIListItem(Translation.ChangeLayout + " ...");
                        dlg.Add(pItem);
                        pItem.ItemId = (int)eContextMenus.switchLayout;
                    }

                    if (listLevel != Listlevel.Group)
                    {
                        pItem = new GUIListItem(Translation.Actions + " ...");
                        dlg.Add(pItem);
                        pItem.ItemId = (int)eContextMenus.action;
                    }

                    pItem = new GUIListItem(Translation.Options + " ...");
                    dlg.Add(pItem);
                    pItem.ItemId = (int)eContextMenus.options;
                    #endregion
                    
                    dlg.DoModal(GUIWindowManager.ActiveWindow);
                    #region Selected Menu Item Actions (Sub-Menus)
                    switch (dlg.SelectedId) {                        
                        case (int)eContextMenus.download:
                            {
                                dlg.Reset();
                                dlg.SetHeading(Translation.Download);
                                                                
                                if (tvSubtitlesEnable || seriesSubEnable || remositoryEnable)
                                {
                                    pItem = new GUIListItem(Translation.Retrieve_Subtitle);
                                    dlg.Add(pItem);
                                    pItem.ItemId = (int)eContextItems.downloadSubtitle;
                                }

                                if (newsEnable)
                                {
                                    pItem = new GUIListItem(Translation.Load_via_NewsLeecher);
                                    dlg.Add(pItem);
                                    pItem.ItemId = (int)eContextItems.downloadviaNewz;
                                }

                                if (torrentsEnable)
                                {
                                    pItem = new GUIListItem(Translation.Load_via_Torrent);
                                    dlg.Add(pItem);
                                    pItem.ItemId = (int)eContextItems.downloadviaTorrent;
                                }

                                dlg.DoModal(GUIWindowManager.ActiveWindow);
                                if (dlg.SelectedId != -1)
                                    bExitMenu = true;
                            }
                            break;

                        case (int)eContextMenus.action:
                            {
                                dlg.Reset();
                                dlg.SetHeading(Translation.Actions);
                                if (listLevel != Listlevel.Group)
                                {
                                    pItem = new GUIListItem(Translation.Hide);
                                    dlg.Add(pItem);
                                    pItem.ItemId = (int)eContextItems.actionHide;

                                    if (DBOption.GetOptions(DBOption.cShowDeleteMenu)) {
                                        pItem = new GUIListItem(Translation.Delete);
                                        dlg.Add(pItem);
                                        pItem.ItemId = (int)eContextItems.actionDelete;
                                    }

                                    pItem = new GUIListItem(Translation.updateMI);
                                    dlg.Add(pItem);
                                    pItem.ItemId = (int)eContextItems.actionRecheckMI;

                                    pItem = new GUIListItem(Translation.ResetUserSelections);
                                    dlg.Add(pItem);
                                    pItem.ItemId = (int)eContextItems.resetUserSelections;
                                }
                                pItem = new GUIListItem(Translation.Force_Local_Scan + (m_parserUpdaterWorking ? Translation.In_Progress_with_Barracks : ""));
                                dlg.Add(pItem);
                                pItem.ItemId = (int)eContextItems.actionLocalScan;

                                pItem = new GUIListItem(Translation.Force_Online_Refresh + (m_parserUpdaterWorking ? Translation.In_Progress_with_Barracks : ""));
                                dlg.Add(pItem);
                                pItem.ItemId = (int)eContextItems.actionFullRefresh;

                                pItem = new GUIListItem(Translation.Play_Random_Episode);
                                dlg.Add(pItem);
                                pItem.ItemId = (int)eContextItems.actionPlayRandom;

                                dlg.DoModal(GUIWindowManager.ActiveWindow);
                                if (dlg.SelectedId != -1)
                                    bExitMenu = true;
                            }
                            break;

                        case (int)eContextMenus.options:
                            {
                                dlg.Reset();
                                ShowOptionsMenu();
                                return;
                            }                            

                        case (int)eContextMenus.switchView:
                            {
                                dlg.Reset();
                                if (showViewSwitchDialog())
                                    return;
                            }
                            break;

                        case (int)eContextMenus.switchLayout:
                            {
                                dlg.Reset();
                                ShowLayoutMenu();
                                return;
                            }                            
                        
                        case (int)eContextMenus.addToView:
                            dlg.Reset();
                            ShowViewTagsMenu(true, selectedSeries);
                            return;

                        case (int)eContextMenus.removeFromView:
                            dlg.Reset();
                            ShowViewTagsMenu(false, selectedSeries);
                            return;

                        case (int)eContextMenus.rate:
                            {                            
                                switch (listLevel)
                                {
                                    case Listlevel.Episode:
                                        showRatingsDialog(m_SelectedEpisode, false);
                                        MPTVSeriesLog.Write(string.Format("Setting rating of {0} to: {1}/10", m_SelectedEpisode, m_SelectedEpisode[DBOnlineEpisode.cMyRating]));
                                        break;
                                    case Listlevel.Series:
                                    case Listlevel.Season:
                                        showRatingsDialog(m_SelectedSeries, false);
                                        MPTVSeriesLog.Write(string.Format("Setting rating of {0} to: {1}/10", m_SelectedSeries, m_SelectedSeries[DBOnlineSeries.cMyRating]));
                                        break;
                                }
                                LoadFacade();
                                if (dlg.SelectedId != -1)
                                    bExitMenu = true;
                                return;
                            }

                        default:
                            bExitMenu = true;
                            break;
                    }
                    #endregion
                }
                while (!bExitMenu);

                if (dlg.SelectedId == -1) return;

                #region Selected Menu Item Actions
                switch (dlg.SelectedId) {
                    #region Watched/Unwatched
                    case (int)eContextItems.toggleWatched:
                        {
                            // toggle watched
                            if (selectedEpisode != null)
                            {
                                if (selectedEpisode[DBEpisode.cFilename].ToString().Length > 0)
                                {
                                    SQLCondition condition = new SQLCondition();
                                    condition.Add(new DBEpisode(), DBEpisode.cFilename, selectedEpisode[DBEpisode.cFilename], SQLConditionType.Equal);
                                    List<DBEpisode> episodes = DBEpisode.Get(condition, false);
                                    foreach (DBEpisode episode in episodes)
                                    {
                                        episode[DBOnlineEpisode.cWatched] = selectedEpisode[DBOnlineEpisode.cWatched] == 0;
                                        episode.Commit();
                                        //DBSeason.UpdateUnWatched(episode);
                                        //DBSeries.UpdateUnWatched(episode);
                                    }
                                }
                                else
                                {
                                    selectedEpisode[DBOnlineEpisode.cWatched] = selectedEpisode[DBOnlineEpisode.cWatched] == 0;
                                    selectedEpisode.Commit();
                                    //DBSeason.UpdateUnWatched(selectedEpisode);
                                    //DBSeries.UpdateUnWatched(selectedEpisode);
                                }
                                // Update Episode Counts
                                DBSeason.UpdatedEpisodeCounts(m_SelectedSeries,m_SelectedSeason);
                                LoadFacade();
                            }
                        }
                        break;
                                        
                    case (int)eContextItems.actionMarkAllWatched:
                        // all watched
                        if (this.listLevel == Listlevel.Series && m_SelectedSeries != null) {
                            DBTVSeries.Execute("update online_episodes set watched = 1 where " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID) + " = " + m_SelectedSeries[DBSeries.cID]);
                            DBTVSeries.Execute("update season set " + DBSeason.cUnwatchedItems + " = 0 where " + DBSeason.Q(DBSeason.cSeriesID) + " = " + m_SelectedSeries[DBSeries.cID]);
                            m_SelectedSeries[DBOnlineSeries.cUnwatchedItems] = false;
                            m_SelectedSeries.Commit();
                            // Updated Episode Counts
                            DBSeries.UpdatedEpisodeCounts(m_SelectedSeries);
                            cache.dump();
                        }
                        else if (this.listLevel == Listlevel.Season && m_SelectedSeason != null) {
                            DBTVSeries.Execute("update online_episodes set watched = 1 where " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID) + " = " + m_SelectedSeason[DBSeason.cSeriesID] +
                                                " and " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeasonIndex) + " = " + m_SelectedSeason[DBSeason.cIndex]);
                            m_SelectedSeason[DBSeason.cUnwatchedItems] = false;
                            m_SelectedSeason.Commit();
                            DBSeason.UpdatedEpisodeCounts(m_SelectedSeries, m_SelectedSeason);
                            cache.dump();
                        }
                        LoadFacade(); // refresh
                        break;

                    case (int)eContextItems.actionMarkAllUnwatched:
                        // all unwatched
                        if (this.listLevel == Listlevel.Series && m_SelectedSeries != null) {
                            DBTVSeries.Execute("update online_episodes set watched = 0 where " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID) + " = " + m_SelectedSeries[DBSeries.cID]);
                            DBTVSeries.Execute("update season set " + DBSeason.cUnwatchedItems + " = 1 where " + DBSeason.Q(DBSeason.cSeriesID) + " = " + m_SelectedSeries[DBSeries.cID]);
                            m_SelectedSeries[DBOnlineSeries.cUnwatchedItems] = true;
                            m_SelectedSeries.Commit();
                            DBSeries.UpdatedEpisodeCounts(m_SelectedSeries);
                            cache.dump();
                        }
                        else if (this.listLevel == Listlevel.Season && m_SelectedSeason != null) {
                            DBTVSeries.Execute("update online_episodes set watched = 0 where " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID) + " = " + m_SelectedSeason[DBSeason.cSeriesID] +
                                                " and " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeasonIndex) + " = " + m_SelectedSeason[DBSeason.cIndex]);
                            m_SelectedSeason[DBSeason.cUnwatchedItems] = true;
                            m_SelectedSeason.Commit();
                            DBSeason.UpdatedEpisodeCounts(m_SelectedSeries, m_SelectedSeason);
                            cache.dump();
                        }
                        LoadFacade(); // refresh
                        break;                  
                    #endregion

                    #region Playlist
                    case (int)eContextItems.addToPlaylist:
                        AddItemToPlayList();
                        break;
                    #endregion
                    
                    #region Cycle Artwork
                    case (int)eContextItems.cycleSeriesBanner:
                        {
                            int nCurrent = selectedSeries.BannerList.IndexOf(selectedSeries.Banner);
                            nCurrent++;
                            if (nCurrent >= selectedSeries.BannerList.Count)
                                nCurrent = 0;

                            selectedSeries.Banner = selectedSeries.BannerList[nCurrent];
                            selectedSeries.Commit();

                            // No need to re-load the facade for non-graphical layouts
                            if (m_Facade.View == GUIFacadeControl.ViewMode.List)
                                seriesbanner.Filename = ImageAllocator.GetSeriesBannerAsFilename(selectedSeries); 
                            else
                                LoadFacade();
                        }
                        break;

                    case (int)eContextItems.cycleSeriesPoster:
                        {
                            int nCurrent = selectedSeries.PosterList.IndexOf(selectedSeries.Poster);
                            nCurrent++;
                            if (nCurrent >= selectedSeries.PosterList.Count)
                                nCurrent = 0;

                            selectedSeries.Poster = selectedSeries.PosterList[nCurrent];
                            selectedSeries.Commit();

                            // No need to re-load the facade for non-graphical layouts
                            if (m_Facade.View == GUIFacadeControl.ViewMode.List)
                                seriesposter.Filename = ImageAllocator.GetSeriesPosterAsFilename(selectedSeries);
                            else
                                LoadFacade();
                        }
                        break;

                    case (int)eContextItems.cycleSeasonBanner:
                        {
                            int nCurrent = selectedSeason.BannerList.IndexOf(selectedSeason.Banner);
                            nCurrent++;
                            if (nCurrent >= selectedSeason.BannerList.Count)
                                nCurrent = 0;

                            selectedSeason.Banner = selectedSeason.BannerList[nCurrent];
                            selectedSeason.Commit();
                            m_bUpdateBanner = true;

                            // No need to re-load the facade for non-graphical layouts
                            if (m_Facade.View == GUIFacadeControl.ViewMode.List)
                                seasonbanner.Filename = ImageAllocator.GetSeasonBannerAsFilename(selectedSeason);
                            else
                                LoadFacade();
                        }
                        break;
                    #endregion

                    #region Fanart Chooser
                    case (int)eContextItems.showFanartChooser:
                        ShowFanartChooser(m_SelectedSeries[DBOnlineSeries.cID]);
                        break;
                    #endregion                    

                    #region Force Online Series Query
                    case (int)eContextItems.forceSeriesQuery:
                        {
                            // clear the series
                            SQLCondition condition = new SQLCondition();
                            condition.Add(new DBEpisode(), DBEpisode.cSeriesID, selectedSeries[DBSeries.cID], SQLConditionType.Equal);
                            DBEpisode.Clear(condition);
                            condition = new SQLCondition();
                            condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, selectedSeries[DBSeries.cID], SQLConditionType.Equal);
                            DBOnlineEpisode.Clear(condition);

                            condition = new SQLCondition();
                            condition.Add(new DBSeason(), DBSeason.cSeriesID, selectedSeries[DBSeries.cID], SQLConditionType.Equal);
                            DBSeason.Clear(condition);

                            condition = new SQLCondition();
                            condition.Add(new DBSeries(), DBSeries.cID, selectedSeries[DBSeries.cID], SQLConditionType.Equal);
                            DBSeries.Clear(condition);

                            condition = new SQLCondition();
                            condition.Add(new DBOnlineSeries(), DBOnlineSeries.cID, selectedSeries[DBSeries.cID], SQLConditionType.Equal);
                            DBOnlineSeries.Clear(condition);

                            // look for it again
                            m_parserUpdaterQueue.Add(new CParsingParameters(ParsingAction.NoExactMatch, null, true, false));
                        }
                        break;
                    #endregion

                    #region Downloaders
                    case (int)eContextItems.downloadSubtitle:
                        {
                            if (selectedEpisode != null)
                            {
                                DBEpisode episode = (DBEpisode)currentitem.TVTag;
                                setProcessAnimationStatus(true);

                                List<CItem> Choices = new List<CItem>();
                                bool tvSubtitlesEnable = DBOption.GetOptions(DBOption.cSubs_TVSubtitles_Enable);
                                bool seriesSubEnable = DBOption.GetOptions(DBOption.cSubs_SeriesSubs_Enable);
                                bool remositoryEnable = DBOption.GetOptions(DBOption.cSubs_Remository_Enable);

                                if (tvSubtitlesEnable)
                                  Choices.Add(new CItem("TVSubtitles.Net", "TVSubtitles.Net", "TVSubtitles.Net"));
                                if (seriesSubEnable)
                                  Choices.Add(new CItem("Series Subs", "Series Subs", "Series Subs"));
                                if (remositoryEnable)
                                  Choices.Add(new CItem("Remository", "Remository", "Remository"));

                                CItem selected = null;
                                switch (Choices.Count)
                                {
                                  case 0:
                                    // none enable, do nothing
                                    break;

                                  case 1:
                                    // only one enabled, don't bother showing the dialog
                                    selected = Choices[0];
                                    break;

                                  default:
                                    // more than 1 choice, show a feedback dialog
                                    ChooseFromSelectionDescriptor descriptor = new ChooseFromSelectionDescriptor();
                                    descriptor.m_sTitle = "Get subtitles from?";
                                    descriptor.m_sListLabel = "Enabled subtitle sites:";
                                    descriptor.m_List = Choices;
                                    descriptor.m_sbtnIgnoreLabel = String.Empty;

                                    bool bReady = false;
                                    while (!bReady)
                                    {
                                      ReturnCode resultFeedback = ChooseFromSelection(descriptor, out selected);
                                      switch (resultFeedback)
                                      {
                                        case ReturnCode.NotReady:
                                          {
                                            // we'll wait until the plugin is loaded - we don't want to show up unrequested popups outside the tvseries pages
                                            Thread.Sleep(5000);
                                          }
                                          break;

                                        case ReturnCode.OK:
                                          {
                                            bReady = true;
                                          }
                                          break;

                                        default:
                                          {
                                            // exit too if cancelled
                                            bReady = true;
                                          }
                                          break;
                                      }
                                    }
                                    break;
                                }

                                if (selected != null)
                                                {
                                  switch ((String)selected.m_Tag)
                                  {
                                    case "TVSubtitles.Net":
                                      TvSubtitles tvsubs = new TvSubtitles(this);
                                      tvsubs.SubtitleRetrievalCompleted += new WindowPlugins.GUITVSeries.Subtitles.TvSubtitles.SubtitleRetrievalCompletedHandler(tvsubs_SubtitleRetrievalCompleted);
                                      tvsubs.GetSubs(episode);
                                      break;

                                    case "Series Subs":
                                      SeriesSubs seriesSubs = new SeriesSubs(this);
                                      seriesSubs.SubtitleRetrievalCompleted += new WindowPlugins.GUITVSeries.Subtitles.SeriesSubs.SubtitleRetrievalCompletedHandler(seriesSubs_SubtitleRetrievalCompleted);
                                      seriesSubs.GetSubs(episode);
                                      break;

                                    case "Remository":
                                      Remository remository = new Remository(this);
                                      remository.SubtitleRetrievalCompleted += new WindowPlugins.GUITVSeries.Subtitles.Remository.SubtitleRetrievalCompletedHandler(remository_SubtitleRetrievalCompleted);
                                      remository.GetSubs(episode);
                                      break;
                                  }
                                }
                            }
                        }
                        break;

                    case (int)eContextItems.downloadviaTorrent:
                    {
                            if (selectedEpisode != null)
                            {
                                Torrent.Load torrentLoad = new Torrent.Load(this);
                                torrentWorking = true;
                                torrentLoad.LoadCompleted += new Torrent.Load.LoadCompletedHandler(Load_TorrentLoadCompleted);
                                if (torrentLoad.Search(selectedEpisode))
                                    setProcessAnimationStatus(true);
                            }
                        }
                        break;

                    case (int)eContextItems.downloadviaNewz:
                        {
                            if (selectedEpisode != null)
                            {
                                Newzbin.Load load = new WindowPlugins.GUITVSeries.Newzbin.Load(this);
                                load.LoadCompleted += new WindowPlugins.GUITVSeries.Newzbin.Load.LoadCompletedHandler(load_LoadNewzBinCompleted);

                                if (load.Search(selectedEpisode))
                                    setProcessAnimationStatus(true);
                            }
                        }
                        break;
                    #endregion

                    #region Favourites
                    case (int)eContextItems.actionToggleFavorite: {
                            // Toggle Favourites
                            m_SelectedSeries.toggleFavourite();

                            // If we are in favourite view we need to reload to remove the series
                            LoadFacade();
                            break;
                        }
                    #endregion

                    #region Actions
                    #region Hide
                    case (int)eContextItems.actionHide: {
                            // hide - we can only hide things for now, no unhide
                            switch (this.listLevel) {
                                case Listlevel.Series:
                                    selectedSeries[DBSeries.cHidden] = true;
                                    MPTVSeriesLog.Write(string.Format("Hiding episode {0} from view", m_SelectedSeries));
                                    selectedSeries.Commit();
                                    break;

                                case Listlevel.Season:
                                    selectedSeason[DBSeason.cHidden] = true;
                                    selectedSeason.Commit();
                                    DBSeries.UpdatedEpisodeCounts(m_SelectedSeries);
                                    break;

                                case Listlevel.Episode:
                                    selectedEpisode[DBOnlineEpisode.cHidden] = true;
                                    MPTVSeriesLog.Write(string.Format("Hiding series {0} from view", m_SelectedEpisode));
                                    selectedEpisode.Commit();
                                    DBSeason.UpdatedEpisodeCounts(m_SelectedSeries, m_SelectedSeason);
                                    break;
                            }
                            LoadFacade();
                        }
                        break;
                    #endregion

                    #region Delete
                    case (int)eContextItems.actionDelete:
                        {
                            dlg.Reset();
                            ShowDeleteMenu(selectedSeries,selectedSeason,selectedEpisode);                            
                        }
                        break;
                    #endregion

                    #region MediaInfo
                    case (int)eContextItems.actionRecheckMI:
                        switch (listLevel) {
                            case Listlevel.Episode:
                                m_SelectedEpisode.readMediaInfoOfLocal();
                                // reload here so logos update
                                LoadFacade();
                                break;
                            case Listlevel.Season:
                                foreach (DBEpisode ep in DBEpisode.Get(m_SelectedSeason[DBSeason.cSeriesID], m_SelectedSeason[DBSeason.cIndex], false))
                                    ep.readMediaInfoOfLocal();
                                break;
                            case Listlevel.Series:
                                foreach (DBEpisode ep in DBEpisode.Get((int)m_SelectedSeries[DBSeries.cID], false))
                                    ep.readMediaInfoOfLocal();
                                break;
                        }
                        break;
                    #endregion

                    #region User Selections
                    case (int)eContextItems.resetUserSelections:

                        if (listLevel != Listlevel.Group) {
                            if (selectedSeries != null)
                                DBUserSelection.Clear(selectedSeries);

                            if (selectedEpisode != null)
                                DBUserSelection.Clear(selectedEpisode);

                            if (selectedSeason != null)
                                DBUserSelection.Clear(selectedSeason);
                        }
                        break;
                    #endregion

                    #region Import
                    case (int)eContextItems.actionLocalScan:
                        // queue scan
                        lock (m_parserUpdaterQueue)
                        {
                            m_parserUpdaterQueue.Add(new CParsingParameters(true, false));
                        }
                        break;

                    case (int)eContextItems.actionFullRefresh:
                        // queue scan
                        lock (m_parserUpdaterQueue)
                        {
                            m_parserUpdaterQueue.Add(new CParsingParameters(true, true));
                        }
                        break;
                    #endregion

                    #region Play
                    case (int)eContextItems.actionPlayRandom:
                        playRandomEp();
                        break;
                    #endregion
                    #endregion

                }
                #endregion
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("The 'OnShowContextMenu' function has generated an error: " + ex.Message + ", StackTrace : " + ex.StackTrace);
            }

        }
        #endregion

        void load_LoadNewzBinCompleted(bool bOK, String msgOut)
        {
            if (m_ImportAnimation != null)
                m_ImportAnimation.FreeResources();

            if (!bOK)
            {
                GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                dlgOK.SetHeading("Error");
                dlgOK.SetLine(2, msgOut);
                dlgOK.DoModal(GUIWindowManager.ActiveWindow);
            }
        }

        void tvsubs_SubtitleRetrievalCompleted(bool bFound)
        {
            setProcessAnimationStatus(false);
            tvsubsWorking = false;
            if (bFound)
            {
                LoadFacade();
            }
            else
            {
                GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                dlgOK.SetHeading(Translation.Completed);
                dlgOK.SetLine(1, Translation.No_subtitles_found);
                dlgOK.DoModal(GUIWindowManager.ActiveWindow);
            }
        }

        void seriesSubs_SubtitleRetrievalCompleted(bool bFound)
        {
          setProcessAnimationStatus(false);
          seriessubWorking = false;
          if (bFound)
          {
            LoadFacade();
          }
          else
          {
            GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            dlgOK.SetHeading(Translation.Completed);
            dlgOK.SetLine(1, Translation.No_subtitles_found);
            dlgOK.DoModal(GUIWindowManager.ActiveWindow);
          }
        }

        void remository_SubtitleRetrievalCompleted(bool bFound)
        {
            setProcessAnimationStatus(false);
            remositoryWorking = false;
            GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            dlgOK.SetHeading(Translation.Completed);
            if (bFound)
            {
                LoadFacade();
                dlgOK.SetLine(1, Translation.Subtitles_download_complete);
            }
            else
            {
                dlgOK.SetLine(1, Translation.No_subtitles_found);
            }
            dlgOK.DoModal(GUIWindowManager.ActiveWindow);
        }

        void Load_TorrentLoadCompleted(bool bOK)
        {
            setProcessAnimationStatus(false);
            torrentWorking = false;
        }

      List<string> sviews = new List<string>();

        void switchView(int offset) //else previous
        {
            switchView(Helper.getElementFromList<logicalView, string>(m_CurrLView.Name, "Name", offset, m_allViews));
        }

        void switchView(string viewName)
        {
            if (Helper.String.IsNullOrEmpty(viewName)) viewName = Translation.All;
            switchView(Helper.getElementFromList<logicalView, string>(viewName, "Name", 0, m_allViews));
        }
        void setCurPositionLabel()
        {
            string prettyCurrPosition = m_CurrLView.prettyName;
            foreach (string subPos in m_stepSelectionPretty)
                if (!Helper.String.IsNullOrEmpty(subPos))
                    prettyCurrPosition += " -> " + subPos;
            setGUIProperty(guiProperty.CurrentView, prettyCurrPosition);
            setGUIProperty(guiProperty.SimpleCurrentView, m_CurrLView.prettyName);
        }
        void setViewLabels()
        {
            try
            {
                setCurPositionLabel();
                if (m_allViews.Count > 1)
                {
                    setGUIProperty(guiProperty.NextView, Helper.getElementFromList<logicalView, string>(m_CurrLView.Name, "Name", 1, m_allViews).prettyName);
                    setGUIProperty(guiProperty.LastView, Helper.getElementFromList<logicalView, string>(m_CurrLView.Name, "Name", -1, m_allViews).prettyName);
                }
                else
                {
                    // if only one (enabled) view supress the display of prev/next
                    clearGUIProperty(guiProperty.NextView);
                    clearGUIProperty(guiProperty.LastView);
                }
            }
            catch (Exception)
            {
                MPTVSeriesLog.Write("Error displaying view names....check your skin file");
            }
        }
        void switchView(logicalView view)
        {
            if (view == null) view = m_allViews[0]; // view was removed or something
            MPTVSeriesLog.Write("Switching view to " + view.Name);
            m_CurrLView = view;

            if (fanartSet) DisableFanart();

            m_CurrViewStep = 0; // we always start out at step 0
            m_stepSelection = null;
            m_stepSelections = new List<string[]>();
            m_stepSelections.Add(new string[] { null });
            setNewListLevelOfCurrView(0);

            // set the skin labels
            m_stepSelectionPretty.Clear();
            setViewLabels();

            DBOption.SetOptions("lastView", view.Name); // to remember next time the plugin is entered
        }

        void setNewListLevelOfCurrView(int step)
        {
            if (dummyIsSeriesPosters != null) 
            {
                dummyIsSeriesPosters.Visible = (DBOption.GetOptions(DBOption.cView_Series_ListFormat) == "Filmstrip" 
                                             || DBOption.GetOptions(DBOption.cView_Series_ListFormat) == "ListPosters" );
                dummyIsSeriesPosters.UpdateVisibility();
            }

            if (dummyThumbnailGraphicalMode != null)
            {
                if (this.listLevel == Listlevel.Series)
                    dummyThumbnailGraphicalMode.Visible = (DBOption.GetOptions(DBOption.cView_Series_ListFormat) == "Filmstrip"
                                                        || DBOption.GetOptions(DBOption.cView_Series_ListFormat) == "WideBanners");

                if (this.listLevel == Listlevel.Season)
                    dummyThumbnailGraphicalMode.Visible = DBOption.GetOptions(DBOption.cView_Season_ListFormat) != "0";


                if (this.listLevel == Listlevel.Episode)
                    dummyFacadeListMode.Visible = false;

                if (this.listLevel == Listlevel.Group)                                               
                    dummyThumbnailGraphicalMode.Visible = DBOption.GetOptions(DBOption.cGraphicalGroupView) == "1";

                dummyThumbnailGraphicalMode.UpdateVisibility();
            }

            if (dummyFacadeListMode != null)
            {
                if (this.listLevel == Listlevel.Series)
                    dummyFacadeListMode.Visible = (DBOption.GetOptions(DBOption.cView_Series_ListFormat) == "ListPosters"
                                                || DBOption.GetOptions(DBOption.cView_Series_ListFormat) == "ListBanners");
                                            
                if (this.listLevel == Listlevel.Season)
                    dummyFacadeListMode.Visible = DBOption.GetOptions(DBOption.cView_Season_ListFormat) == "0";

                if (this.listLevel == Listlevel.Episode)
                    dummyFacadeListMode.Visible = true;

                if (this.listLevel == Listlevel.Group)
                    dummyFacadeListMode.Visible = DBOption.GetOptions(DBOption.cGraphicalGroupView) == "0";

                dummyFacadeListMode.UpdateVisibility();
            }

            resetListLevelDummies();

            switch (m_CurrLView.gettypeOfStep(step))
            {
                case logicalViewStep.type.group:
                    listLevel = Listlevel.Group;
                    if (dummyIsGroups != null) dummyIsGroups.Visible = true;
                    if (dummyIsGroups != null) dummyIsGroups.UpdateVisibility();
                    break;
                case logicalViewStep.type.series:
                    listLevel = Listlevel.Series;
                    if (dummyIsSeries != null) dummyIsSeries.Visible = true;
                    if (dummyIsSeries != null) dummyIsSeries.UpdateVisibility();
                    break;
                case logicalViewStep.type.season:
                    listLevel = Listlevel.Season;
                    if (dummyIsSeasons != null) dummyIsSeasons.Visible = true;
                    if (dummyIsSeasons != null) dummyIsSeasons.UpdateVisibility();
                    break;
                case logicalViewStep.type.episode:
                    listLevel = Listlevel.Episode;
                    if (dummyIsEpisodes != null) dummyIsEpisodes.Visible = true;
                    if (dummyIsEpisodes != null) dummyIsEpisodes.UpdateVisibility();
                    break;
            }
            MPTVSeriesLog.Write("new listlevel: " + listLevel.ToString(), MPTVSeriesLog.LogLevel.Debug);                        
        }
        
        void resetListLevelDummies()
        {
            if (dummyIsSeries != null) dummyIsSeries.Visible = false;
            if (dummyIsSeasons != null) dummyIsSeasons.Visible = false;
            if (dummyIsEpisodes != null) dummyIsEpisodes.Visible = false;
            if (dummyIsGroups != null) dummyIsGroups.Visible = false;
        }

        public override void OnAction(Action action)
        {
            switch (action.wID)
            {
                case Action.ActionType.ACTION_PARENT_DIR:
                case Action.ActionType.ACTION_HOME:
                    ImageAllocator.FlushAll();
                    GUIWindowManager.ShowPreviousWindow();
                    break;

                case Action.ActionType.ACTION_PREVIOUS_MENU:
                    // back one level
                    MPTVSeriesLog.Write("ACTION_PREVIOUS_MENU", MPTVSeriesLog.LogLevel.Debug);
                    if (m_CurrViewStep == 0)
                    {
                        goto case Action.ActionType.ACTION_HOME;
                    }
                    else
                    {
                        m_stepSelections.RemoveAt(m_CurrViewStep);
                        m_stepSelectionPretty.RemoveAt(m_stepSelectionPretty.Count - 1);
                        m_CurrViewStep--;
                        setNewListLevelOfCurrView(m_CurrViewStep);
                        m_back_up_select_this = m_stepSelection;
                        m_stepSelection = m_stepSelections[m_CurrViewStep];
                        skipSeasonIfOne_DirectionDown = false; // otherwise the user cant get back out
                        LoadFacade();
                    }
                    break;

                case Action.ActionType.ACTION_SHOW_PLAYLIST:
                    ShowPlaylistWindow();                    
                    break;
				
				case Action.ActionType.REMOTE_0:
                    if (DBOption.GetOptions(DBOption.cShowDeleteMenu)) {
                        // MediaPortal Delete Shortcut on Remote/Keyboard					
                        if (this.m_Facade.SelectedListItem == null || this.m_Facade.SelectedListItem.TVTag == null)
                            return;

                        if (this.listLevel == Listlevel.Group)
                            return;

                        DBSeries selectedSeries = null;
                        DBSeason selectedSeason = null;
                        DBEpisode selectedEpisode = null;

                        switch (this.listLevel) {
                            case Listlevel.Series:
                                selectedSeries = this.m_Facade.SelectedListItem.TVTag as DBSeries;
                                break;
                            case Listlevel.Season:
                                selectedSeason = this.m_Facade.SelectedListItem.TVTag as DBSeason;
                                break;
                            case Listlevel.Episode:
                                selectedEpisode = this.m_Facade.SelectedListItem.TVTag as DBEpisode;
                                break;
                        }
                        // Invoke Delete Menu
                        ShowDeleteMenu(selectedSeries, selectedSeason, selectedEpisode);
                        m_bOnActionProcessed = true;
                        return;
                    }
                    break;

                case Action.ActionType.ACTION_KEY_PRESSED:
                    // For some reason this action gets processed twice after deleting an item
                    // using the shortcut above (ZERO on Remote action). This is a workaround
                    // so MediaPortal doesnt throw an exception.
                    if (m_bOnActionProcessed && this.m_Facade.SelectedListItemIndex == -1) {
                        m_bOnActionProcessed = false;
                        return;
                    }
                    base.OnAction(action);
                    break;

                default:
                    base.OnAction(action);
                    break;
            }
        }

        bool fanartSet = false;
        Fanart currSeriesFanart = null;

        bool loadFanart(DBTable item)
        {
            if (FanartBackground == null)
            {
                // Fanart not supported by skin, exit now
                fanartSet = false;
                if (this.dummyIsFanartLoaded != null)
                    this.dummyIsFanartLoaded.Visible = false;

                return false;
            }

            try
            {             
                Fanart fanart = currSeriesFanart;
                DBSeries series = item as DBSeries;

                // Get a Fanart for selected series from Database
                if (series != null)
                {
                    if (fanart == null || fanart.SeriesID != series[DBSeries.cID])
                    {
                        // Get a Fanart object for currently selected series
                        fanart = Fanart.getFanart(series[DBSeries.cID]);
                    }                    
                }
                else
                {
                    // Get Season Fanart if it exists, otherwise return Series fanart
                    DBSeason season = item as DBSeason;
                    if (season != null)
                        fanart = Fanart.getFanart(season[DBSeason.cSeriesID], season[DBSeason.cIndex]);                        
                }
               
                if (fanart == null)
                {
                    // This shouldn't happen
                    MPTVSeriesLog.Write("Fanart is unavailable, disabling", MPTVSeriesLog.LogLevel.Debug);
                    DisableFanart();
                    return false;
                }

                // Reset Fanart Selection for next time
                fanart.ForceNewPick();
                currSeriesFanart = fanart;

                // Activate Backdrop in Image Swapper                
                if (!backdrop.Active) backdrop.Active = true;

                // Assign Fanart filename to Image Loader
                // Will display fanart in backdrop or reset to default background                
                backdrop.Filename = fanart.FanartFilename;
                
                if (fanart.Found)
                    MPTVSeriesLog.Write(string.Format("Fanart found and loaded for series {0}, loading: {1}", Helper.getCorrespondingSeries(fanart.SeriesID).ToString(), backdrop.Filename), MPTVSeriesLog.LogLevel.Debug);
                else
                    MPTVSeriesLog.Write(string.Format("Fanart not found for series {0}", Helper.getCorrespondingSeries(fanart.SeriesID).ToString()));
                
                // I don't think we can support these anymore with dbfanart now
                //if (this.dummyIsLightFanartLoaded != null)
                //    this.dummyIsLightFanartLoaded.Visible = f.RandomPickIsLight;
                //if (this.dummyIsDarkFanartLoaded != null)
                //    this.dummyIsDarkFanartLoaded.Visible = !f.RandomPickIsLight;

                if (fanart.HasColorInfo)
                {
                    System.Drawing.Color[] fanartColors = fanart.Colors;
                    setGUIProperty("FanArt.Colors.LightAccent", Fanart.RGBColorToHex(fanartColors[0]));
                    setGUIProperty("FanArt.Colors.DarkAccent", Fanart.RGBColorToHex(fanartColors[1]));
                    setGUIProperty("FanArt.Colors.Neutral Midtone", Fanart.RGBColorToHex(fanartColors[2]));
                }
                else
                {
                    setGUIProperty("FanArt.Colors.LightAccent", string.Empty);
                    setGUIProperty("FanArt.Colors.DarkAccent", string.Empty);
                    setGUIProperty("FanArt.Colors.Neutral Midtone", string.Empty);
                }

                if (this.dummyIsFanartColorAvailable != null)
                    this.dummyIsFanartColorAvailable.Visible = fanart.HasColorInfo;

                if (this.dummyIsFanartLoaded != null)
                    this.dummyIsFanartLoaded.Visible = true;

                return fanartSet = true;
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Failed to load Fanart: " + ex.Message, MPTVSeriesLog.LogLevel.Normal);
                return fanartSet = false;
            }
        }

		private void FanartTimerEvent(object state) {			
            loadFanart(m_FanartItem);
            m_bFanartTimerDisabled = false;
		}

        void DisableFanart()
        {
            // Disable Random Fanart Timer
            m_FanartTimer.Change(Timeout.Infinite, Timeout.Infinite);
            m_bFanartTimerDisabled = true;

            if (FanartBackground == null)
            {
                // Fanart not supported by skin, exit now
                fanartSet = false;
                if (this.dummyIsFanartLoaded != null)
                    this.dummyIsFanartLoaded.Visible = false;

                return;
            }

            // Disable Fanart                
            MPTVSeriesLog.Write("Fanart: Disabling Fanart Display", MPTVSeriesLog.LogLevel.Debug);

            currSeriesFanart = null;
            fanartSet = false;
            if (backdrop.Active) backdrop.Active = false;
            backdrop.Filename = "";

            // Reset Dummy controls
            if (this.dummyIsFanartLoaded != null)
                this.dummyIsFanartLoaded.Visible = false;
            if (this.dummyIsLightFanartLoaded != null)
                this.dummyIsLightFanartLoaded.Visible = false;
            if (this.dummyIsDarkFanartLoaded != null)
                this.dummyIsDarkFanartLoaded.Visible = false;
            if (this.dummyIsFanartColorAvailable != null)
                this.dummyIsFanartColorAvailable.Visible = false;

            // Reset skin properties
            setGUIProperty("FanArt.Colors.LightAccent", string.Empty);
            setGUIProperty("FanArt.Colors.DarkAccent", string.Empty);
            setGUIProperty("FanArt.Colors.NeutralMidtone", string.Empty);
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType)
        {
            if (control == this.viewMenuButton)
            {
                showViewSwitchDialog();
                viewMenuButton.Focus = false;
                return;
            }

            if (control == this.LayoutMenuButton)
            {
                ShowLayoutMenu();
                LayoutMenuButton.Focus = false;
                return;
            }

            if (control == this.OptionsMenuButton)
            {
                ShowOptionsMenu();
                OptionsMenuButton.Focus = false;
                return;
            }

            if (control == this.ImportButton)
            {
                lock (m_parserUpdaterQueue)
                {
                    m_parserUpdaterQueue.Add(new CParsingParameters(true, true));
                }
                ImportButton.Focus = false;
                return;
            }

            if (control == this.LoadPlaylistButton)
            {
                OnShowSavedPlaylists(DBOption.GetOptions(DBOption.cPlaylistPath));
                LoadPlaylistButton.Focus = false;
                return;
            }

            if (actionType != Action.ActionType.ACTION_SELECT_ITEM) return; // some other events raised onClicked too for some reason?
            if (control == this.m_Facade)
            {
                if (this.m_Facade.SelectedListItem == null || this.m_Facade.SelectedListItem.TVTag == null)
                    return;				

                m_back_up_select_this = null;
                switch (this.listLevel)
                {
                    case Listlevel.Group:
                        this.m_CurrViewStep++;
                        setNewListLevelOfCurrView(m_CurrViewStep);
                        m_stepSelection = new string[] { this.m_Facade.SelectedListItem.Label };
                        m_stepSelections.Add(m_stepSelection);
                        m_stepSelectionPretty.Add(this.m_Facade.SelectedListItem.Label);
                        MPTVSeriesLog.Write("Selected: ", this.m_Facade.SelectedListItem.Label, MPTVSeriesLog.LogLevel.Debug);
                        LoadFacade();
                        this.m_Facade.Focus = true;
                        break;
                    case Listlevel.Series:
                        this.m_SelectedSeries = this.m_Facade.SelectedListItem.TVTag as DBSeries;
                        if (m_SelectedSeries == null) return;

                        this.m_CurrViewStep++;
                        setNewListLevelOfCurrView(m_CurrViewStep);
                        m_stepSelection = new string[] { m_SelectedSeries[DBSeries.cID].ToString() };
                        m_stepSelections.Add(m_stepSelection);
                        m_stepSelectionPretty.Add(this.m_SelectedSeries.ToString());
                        MPTVSeriesLog.Write("Selected: ", m_stepSelection[0], MPTVSeriesLog.LogLevel.Debug);
                        MPTVSeriesLog.Write("Fanart: Series selected", MPTVSeriesLog.LogLevel.Debug);
                        this.LoadFacade();
                        this.m_Facade.Focus = true;
                        break;
                    case Listlevel.Season:
                        this.m_SelectedSeason = this.m_Facade.SelectedListItem.TVTag as DBSeason;
                        if (m_SelectedSeason == null) return;

                        this.m_CurrViewStep++;
                        setNewListLevelOfCurrView(m_CurrViewStep);
                        m_stepSelection = new string[] { m_SelectedSeason[DBSeason.cSeriesID].ToString(), m_SelectedSeason[DBSeason.cIndex].ToString() };
                        m_stepSelections.Add(m_stepSelection);
                        m_stepSelectionPretty.Add(m_SelectedSeason[DBSeason.cIndex] == 0 ? Translation.specials : Translation.Season + " " + m_SelectedSeason[DBSeason.cIndex]);
                        MPTVSeriesLog.Write("Selected: ", m_stepSelection[0] + " - " + m_stepSelection[1], MPTVSeriesLog.LogLevel.Debug);
                        this.LoadFacade();
                        MPTVSeriesLog.Write("Fanart: Season selected", MPTVSeriesLog.LogLevel.Debug);
                        this.m_Facade.Focus = true;
                        break;
                    case Listlevel.Episode:
                        this.m_SelectedEpisode = this.m_Facade.SelectedListItem.TVTag as DBEpisode;
                        if (m_SelectedEpisode == null) return;
                        MPTVSeriesLog.Write("Selected: ", this.m_SelectedEpisode[DBEpisode.cCompositeID].ToString(), MPTVSeriesLog.LogLevel.Debug);
                        m_VideoHandler.ResumeOrPlay(m_SelectedEpisode);
                        break;
                }
            }           
            base.OnClicked(controlId, control, actionType);
        }

        private string GetCurrentLayout()
        {
            switch (this.listLevel)
            {
                case Listlevel.Group:
                {
                    if (DBOption.GetOptions(DBOption.cGraphicalGroupView))
                        return "SmallIcons";
                    else
                        return "List";
                }

                case Listlevel.Series:
                {
                    return DBOption.GetOptions(DBOption.cView_Series_ListFormat);
                }

                case Listlevel.Season:
                {
                    if (DBOption.GetOptions(DBOption.cView_Season_ListFormat))
                        return "Filmstrip";
                    else
                        return "List";
                }

                default:
                    return "List";
            }
        }
                
        private void ShowLayoutMenu()
        {
            string currentLayout = GetCurrentLayout();

            IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            dlg.Reset();
            dlg.SetHeading(Translation.ChangeLayout);

            int counter = 0;            
            string item = string.Empty;

            // Get Available Layouts for Skin
            foreach (string layout in SkinSettings.SupportedLayouts)
            {
                switch (this.listLevel)
                {
                    case Listlevel.Group:
                    {
                        if (layout.StartsWith("Group"))
                            item = layout.Substring(5);
                    }
                    break;

                    case Listlevel.Series:
                    {
                        if (layout.StartsWith("Series"))
                            item = layout.Substring(6);
                    }
                    break;

                    case Listlevel.Season:
                    {
                        if (layout.StartsWith("Season"))
                            item = layout.Substring(6);
                    }
                    break;

                    case Listlevel.Episode:
                    {
                        if (layout.StartsWith("Episode"))
                            item = layout.Substring(7);
                    }
                    break;
                }

                if (item.Length > 0)
                {
                    string menuItem = GetTranslatedLayout(item);
                    if (menuItem.Length > 0)
                    {
                        GUIListItem pItem = new GUIListItem(menuItem);
                        dlg.Add(pItem);
                        pItem.ItemId = counter++;
                    }
                    item = string.Empty;
                }                
            }

            if (counter > 0)            
                dlg.DoModal(GUIWindowManager.ActiveWindow);

            
            if (dlg.SelectedId >= 0)
            {
                string selectedLayout = GetLogicalLayout(dlg.SelectedLabelText);
                if (!selectedLayout.Equals(currentLayout))
                {
                    switchLayout(selectedLayout);
                    LoadFacade();
                }
            }
        }

        private string GetLogicalLayout(string layout)
        {
            if (layout == Translation.LayoutList)
                return "List";

            if (layout == Translation.LayoutListPosters)
                return "ListPosters";

            if (layout == Translation.LayoutListBanners)
                return "ListBanners";

            if (layout == Translation.LayoutFilmstrip)
                return "Filmstrip";

            if (layout == Translation.LayoutSmallIcons)
                return "SmallIcons";

            if (layout == Translation.LayoutWideBanners)
                return "WideBanners";

            return layout;
        }

        private string GetTranslatedLayout(string layout)
        {
            string translatedLayout = string.Empty;

            switch (layout)
            {
                case "List":
                    translatedLayout = Translation.LayoutList;
                    break;
                case "SmallIcons":
                    translatedLayout = Translation.LayoutSmallIcons;
                    break;
                case "ListPosters":
                    translatedLayout = Translation.LayoutListPosters;
                    break;
                case "ListBanners":
                    translatedLayout = Translation.LayoutListBanners;
                    break;
                case "Filmstrip":
                    translatedLayout = Translation.LayoutFilmstrip;
                    break;
                case "WideBanners":
                    translatedLayout = Translation.LayoutWideBanners;
                    break;
            }
            return translatedLayout;

        }

        private void switchLayout(string layout)
        {
            switch (this.listLevel)
            {
                case Listlevel.Group:
                {
                    if (layout == "List")
                        DBOption.SetOptions(DBOption.cGraphicalGroupView,"0");                            
                    else
                        DBOption.SetOptions(DBOption.cGraphicalGroupView, "1");
                    return;
                }

                case Listlevel.Series:
                {
                    DBOption.SetOptions(DBOption.cView_Series_ListFormat,layout);
                    return;
                }

                case Listlevel.Season:
                {
                    if (layout == "List")
                        DBOption.SetOptions(DBOption.cView_Season_ListFormat, "0");
                    else
                        DBOption.SetOptions(DBOption.cView_Season_ListFormat, "1");
                    return;
                }

                default:
                    return;
            }    
        }

        public void Clock(Object stateInfo)
        {
            if (!m_parserUpdaterWorking)
            {
                // need to not be doing something yet (we don't want to accumulate parser objects !)
                bool bUpdateScanNeeded = false;

                if (m_nUpdateScanLapse > 0)
                {
                    TimeSpan tsUpdate = DateTime.Now - m_LastUpdateScan;
                    if ((int)tsUpdate.TotalHours > m_nUpdateScanLapse)
                        bUpdateScanNeeded = true;
                }
                if (bUpdateScanNeeded)
                {
                    // queue scan
                    lock (m_parserUpdaterQueue)
                    {
                        m_parserUpdaterQueue.Add(new CParsingParameters(false, bUpdateScanNeeded));
                    }
                }

                lock (m_parserUpdaterQueue)
                {
                    if (m_parserUpdaterQueue.Count > 0)
                    {
                        setProcessAnimationStatus(true);
                        m_parserUpdaterWorking = true;
                        m_parserUpdater.Start(m_parserUpdaterQueue[0]);
                        m_parserUpdaterQueue.RemoveAt(0);
                    }
                    else if (!tvsubsWorking && !seriessubWorking && !torrentWorking && !remositoryWorking)
                        setProcessAnimationStatus(false);
                }
            }
            base.Process();
        }

        void parserUpdater_OnlineParsingCompleted(bool bDataUpdated)
        {
            MPTVSeriesLog.Write("Online Parsing done");
            setProcessAnimationStatus(false);

            if (m_parserUpdater.UpdateScan)
            {
                m_LastUpdateScan = DateTime.Now;
                DBOption.SetOptions(DBOption.cUpdateScanLastTime, m_LastUpdateScan.ToString());
            }
            m_parserUpdaterWorking = false;
            if (bDataUpdated)
            {
               if (m_Facade != null) LoadFacade();
            }
        }

        public override bool OnMessage(GUIMessage message)
        {            
            switch (message.Message)
            {
                case GUIMessage.MessageType.GUI_MSG_ITEM_FOCUS_CHANGED:
                    {
                        int iControl = message.SenderControlId;
                        if (iControl == (int)m_Facade.GetID)
                        {
                            switch (this.listLevel)
                            {
                                case Listlevel.Group:
                                    Group_OnItemSelected(m_Facade.SelectedListItem);
                                    break;
                                case Listlevel.Series:
                                    Series_OnItemSelected(m_Facade.SelectedListItem);
                                    break;

                                case Listlevel.Season:
                                    Season_OnItemSelected(m_Facade.SelectedListItem);
                                    break;

                                case Listlevel.Episode:
                                    Episode_OnItemSelected(m_Facade.SelectedListItem);
                                    break;
                            }
                        }
                    }
                    return true;

                case GUIMessage.MessageType.GUI_MSG_PLAYBACK_ENDED:
                case GUIMessage.MessageType.GUI_MSG_PLAYBACK_STOPPED:
                    {
                        //-- Need to reload the GUI to display changes 
                        //-- if episode is classified as watched
                        LoadFacade();
                    }
                    return true;				
               
                default:
                    return base.OnMessage(message);
            }

        }

        private void Group_OnItemSelected(GUIListItem item)
        {
            m_SelectedSeries = null;
            m_SelectedSeason = null;
            m_SelectedEpisode = null;
            if (item == null) return;

            setNewListLevelOfCurrView(m_CurrViewStep);

            // let's try to give the user a bit more information
            string groupedBy = m_CurrLView.groupedInfo(m_CurrViewStep);
            if (groupedBy.Contains("<Ser"))
            {
                int count = 0;
                string seriesNames = string.Empty;
                SQLCondition cond = new SQLCondition();
                cond.AddOrderItem(DBOnlineSeries.Q(DBOnlineSeries.cPrettyName), SQLCondition.orderType.Ascending);
                cond.SetLimit(20);
                if (m_CurrLView.m_steps[m_CurrViewStep].groupedBy.attempSplit && this.m_Facade.SelectedListItem.Label.ToString() != Translation.Unknown)
                {
                    cond.Add(new DBOnlineSeries(), groupedBy.Substring(groupedBy.IndexOf('.') + 1).Replace(">", ""), this.m_Facade.SelectedListItem.Label, SQLConditionType.Like);
                }
                else
                {
                    cond.Add(new DBOnlineSeries(), groupedBy.Substring(groupedBy.IndexOf('.') + 1).Replace(">", ""),
                             (this.m_Facade.SelectedListItem.Label.ToString() == Translation.Unknown ? string.Empty : this.m_Facade.SelectedListItem.Label),
                              SQLConditionType.Equal);
                }
                if (DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles))
                {
                    // not generic
                    SQLCondition fullSubCond = new SQLCondition();
                    fullSubCond.AddCustom(DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID), DBOnlineSeries.Q(DBOnlineSeries.cID), SQLConditionType.Equal);
                    cond.AddCustom(" exists( " + DBEpisode.stdGetSQL(fullSubCond, false) + " )");
                }
                cond.AddCustom("exists ( select id from local_series where id = online_series.id and hidden = 0)");

                foreach (string series in DBOnlineSeries.GetSingleField(DBOnlineSeries.cPrettyName, cond, new DBOnlineSeries()))
                {
                    seriesNames += series + Environment.NewLine;
                    count++;
                }

                setGUIProperty(guiProperty.SeriesCount, count.ToString());
                setGUIProperty(guiProperty.Subtitle, count.ToString() + " " + (count == 1 ? Translation.Series : Translation.Series_Plural));
                setGUIProperty(guiProperty.Description, seriesNames);
            }
            else
            {
                clearGUIProperty(guiProperty.Description);
                clearGUIProperty(guiProperty.Subtitle);
            }

            setGUIProperty(guiProperty.Title, item.Label.ToString());

            setGUIProperty(guiProperty.Logos, localLogos.getLogos(m_CurrLView.groupedInfo(m_CurrViewStep), this.m_Facade.SelectedListItem.Label, logosHeight, logosWidth));

            clearGUIProperty(guiProperty.EpisodeImage);
          
            DisableFanart();
        }

        private void Series_OnItemSelected(GUIListItem item)
        {           
            if (m_bQuickSelect) return;

            m_SelectedSeason = null;
            m_SelectedEpisode = null;
            if (item == null || item.TVTag == null || !(item.TVTag is DBSeries))
                return;

            setNewListLevelOfCurrView(m_CurrViewStep);

            DBSeries series = item.TVTag as DBSeries;
            if (series == null) return;
      			
            m_SelectedSeries = series;
                    
            // set watched/unavailable flag
            if (dummyIsWatched != null) dummyIsWatched.Visible = (int.Parse(series[DBOnlineSeries.cEpisodesUnWatched]) == 0);
            if (dummyIsAvailable != null) dummyIsAvailable.Visible = series[DBSeason.cHasLocalFiles];
            
            clearGUIProperty(guiProperty.EpisodeImage);
            seasonbanner.Filename = "";

            setGUIProperty(guiProperty.Title, FieldGetter.resolveDynString(m_sFormatSeriesTitle, series));
            setGUIProperty(guiProperty.Subtitle, FieldGetter.resolveDynString(m_sFormatSeriesSubtitle, series));
            setGUIProperty(guiProperty.Description, FieldGetter.resolveDynString(m_sFormatSeriesMain, series));

            // Delayed Image Loading of Series Banners/Posters            
            seriesbanner.Filename = ImageAllocator.GetSeriesBannerAsFilename(series);
            seriesposter.Filename = ImageAllocator.GetSeriesPosterAsFilename(series);               

            setGUIProperty(guiProperty.Logos, localLogos.getLogos(ref series, logosHeight, logosWidth));

            pushFieldsToSkin(m_SelectedSeries, "Series");

			// Load Fanart
			if (DBOption.GetOptions(DBOption.cShowSeriesFanart)) {
				// Re-initialize timer for random fanart
				m_FanartItem = m_SelectedSeries;
                if (DBOption.GetOptions(DBOption.cFanartRandom)) {
                    // We should update fanart as soon as new series is selected or
                    // if timer was disabled (e.g. fullscreen playback)
                    if (m_SelectedSeries[DBSeries.cID].ToString() != m_prevSeriesID || m_bFanartTimerDisabled)
                        m_FanartTimer.Change(0, DBOption.GetOptions(DBOption.cRandomFanartInterval));
                }
                else
                    loadFanart(m_FanartItem);
			}

            // Remember last series, so we dont re-initialize random fanart timer
            m_prevSeriesID = m_SelectedSeries[DBSeries.cID];
        }

        private void Season_OnItemSelected(GUIListItem item)
        {
            if (m_bQuickSelect) return;

            m_SelectedEpisode = null;
            if (item == null || item.TVTag == null)
                return;

            setNewListLevelOfCurrView(m_CurrViewStep);

            DBSeason season = item.TVTag as DBSeason;
            if (season == null) return;

            m_SelectedSeason = season;

            // set watched/unavailable flag
            if (dummyIsWatched != null) dummyIsWatched.Visible = (int.Parse(season[DBOnlineSeries.cEpisodesUnWatched]) == 0);
            if (dummyIsAvailable != null) dummyIsAvailable.Visible = season[DBSeason.cHasLocalFiles];

            setGUIProperty(guiProperty.Title, FieldGetter.resolveDynString(m_sFormatSeasonTitle, season));
            setGUIProperty(guiProperty.Subtitle, FieldGetter.resolveDynString(m_sFormatSeasonSubtitle, season));
            setGUIProperty(guiProperty.Description, FieldGetter.resolveDynString(m_sFormatSeasonMain, season));

            // Delayed Image Loading of Season Banners            
            seasonbanner.Filename = ImageAllocator.GetSeasonBannerAsFilename(season);
            //setGUIProperty(guiProperty.SeasonBanner, ImageAllocator.GetSeasonBanner(season, false));

            setGUIProperty(guiProperty.Logos, localLogos.getLogos(ref season, logosHeight, logosWidth));

            clearGUIProperty(guiProperty.EpisodeImage);

            if (!m_CurrLView.stepHasSeriesBeforeIt(m_CurrViewStep))
            {
                // it is the case    
                m_SelectedSeries = Helper.getCorrespondingSeries(season[DBSeason.cSeriesID]);
                if (m_SelectedSeries != null) {
                    seriesbanner.Filename = ImageAllocator.GetSeriesBannerAsFilename(m_SelectedSeries);
                    seriesposter.Filename = ImageAllocator.GetSeriesPosterAsFilename(m_SelectedSeries);
                }
                else {
                    clearGUIProperty(guiProperty.SeriesBanner);
                    clearGUIProperty(guiProperty.SeriesPoster);
                }
            }

            pushFieldsToSkin(m_SelectedSeason, "Season");

			// Load Fanart			
			m_FanartItem = m_SelectedSeason;
			if (DBOption.GetOptions(DBOption.cFanartRandom)) {
                // We should update fanart as soon as new series is selected or
                // if timer was disabled (e.g. fullscreen playback)
                if (m_SelectedSeries[DBSeries.cID].ToString() != m_prevSeriesID || m_bFanartTimerDisabled)
                    m_FanartTimer.Change(0, DBOption.GetOptions(DBOption.cRandomFanartInterval));
			}
			else
				loadFanart(m_FanartItem);
            
            // Remember last series, so we dont re-initialize random fanart timer
            m_prevSeriesID = m_SelectedSeries[DBSeries.cID];
        }

        private void Episode_OnItemSelected(GUIListItem item)
        {
            if (item == null || item.TVTag == null)
                return;

            setNewListLevelOfCurrView(m_CurrViewStep);
            
            DBEpisode episode = item.TVTag as DBEpisode;
            if (episode == null) return;

            // set watched/unavailable flag
            if (dummyIsWatched != null) dummyIsWatched.Visible = episode[DBOnlineEpisode.cWatched];
            if (dummyIsAvailable != null) dummyIsAvailable.Visible = episode[DBEpisode.cFilename].ToString().Length > 0;
           
            this.m_SelectedEpisode = episode;
            setGUIProperty(guiProperty.Logos, localLogos.getLogos(ref episode, logosHeight, logosWidth));

            if (!localLogos.appendEpImage && (episode[DBOnlineEpisode.cWatched] || !DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedThumbnail)))
                setGUIProperty(guiProperty.EpisodeImage, ImageAllocator.GetEpisodeImage(m_SelectedEpisode));
            else
                clearGUIProperty(guiProperty.EpisodeImage);

            setGUIProperty(guiProperty.Title, FieldGetter.resolveDynString(m_sFormatEpisodeTitle, episode));
            setGUIProperty(guiProperty.Subtitle, FieldGetter.resolveDynString(m_sFormatEpisodeSubtitle, episode));
            setGUIProperty(guiProperty.Description, FieldGetter.resolveDynString(m_sFormatEpisodeMain, episode));

            // with groups in episode view its possible the user never selected a series/season (flat view)
            // thus its desirable to display the series_banner and season banner on hover)
            if (!m_CurrLView.stepHasSeriesBeforeIt(m_CurrViewStep) || m_bUpdateBanner)
            {
                // it is the case			
                m_SelectedSeason = Helper.getCorrespondingSeason(episode[DBEpisode.cSeriesID], episode[DBEpisode.cSeasonIndex]);
                m_SelectedSeries = Helper.getCorrespondingSeries(episode[DBEpisode.cSeriesID]);

                if (m_SelectedSeries != null) {
                    seriesbanner.Filename = ImageAllocator.GetSeriesBannerAsFilename(m_SelectedSeries);
                    seriesposter.Filename = ImageAllocator.GetSeriesPosterAsFilename(m_SelectedSeries);
                    pushFieldsToSkin(m_SelectedSeries, "Series");
                }
                else {
                    clearGUIProperty(guiProperty.SeriesBanner);
                    clearGUIProperty(guiProperty.SeriesPoster);
                }

                if (m_SelectedSeason != null)
                {                    
                    seasonbanner.Filename = ImageAllocator.GetSeasonBannerAsFilename(m_SelectedSeason);
                    pushFieldsToSkin(m_SelectedSeason, "Season");
                }
                else
                    clearGUIProperty(guiProperty.SeasonBanner);

                m_bUpdateBanner = false;
            }
            pushFieldsToSkin(m_SelectedEpisode, "Episode");

            // Load Fanart for Selected Series, might be in Episode Only View e.g. Recently Added, Latest		
			m_FanartItem = m_SelectedSeries;
			if (DBOption.GetOptions(DBOption.cFanartRandom)) {
                // We should update fanart as soon as new series is selected or
                // if timer was disabled (e.g. fullscreen playback)
                if (m_SelectedSeries[DBSeries.cID].ToString() != m_prevSeriesID || m_bFanartTimerDisabled)
                    m_FanartTimer.Change(0, DBOption.GetOptions(DBOption.cRandomFanartInterval));				
			}
			else
				loadFanart(m_FanartItem);

            // Remember last series, so we dont re-initialize random fanart timer
            m_prevSeriesID = m_SelectedSeries[DBSeries.cID];
        }
        
        private delegate ReturnCode ChooseFromSelectionDelegate(ChooseFromSelectionDescriptor descriptor);
        private CItem m_selected;
        public ReturnCode ChooseFromSelection(ChooseFromSelectionDescriptor descriptor, out CItem selected)
        {
            if (this.m_Facade == null)
            {
                selected = null;
                return ReturnCode.NotReady;
            }

            ReturnCode returnCode;
            if (m_localControlForInvoke.InvokeRequired)
            {
                returnCode = (ReturnCode)m_localControlForInvoke.Invoke(new ChooseFromSelectionDelegate(ChooseFromSelectionSync), new Object[] { descriptor });
            }
            else
                returnCode = ChooseFromSelectionSync(descriptor);
            selected = m_selected;
            return returnCode;
        }

        public ReturnCode ChooseFromSelectionSync(ChooseFromSelectionDescriptor descriptor)
        {
            try
            {
                IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                m_selected = null;
                if (dlg == null)
                    return ReturnCode.Cancel;

                dlg.Reset();
                if (descriptor.m_sItemToMatchLabel.Length == 0)
                    dlg.SetHeading(descriptor.m_sTitle);
                else
                    dlg.SetHeading(descriptor.m_sItemToMatchLabel + " " + descriptor.m_sItemToMatch);

                GUIListItem pItem = null;

                if (descriptor.m_sbtnIgnoreLabel.Length > 0)
                {
                    pItem = new GUIListItem(Translation.skip_Never_ask_again);
                    dlg.Add(pItem);
                    pItem.ItemId = 0;
                }

                if (descriptor.m_List.Count == 0)
                {
                    pItem = new GUIListItem(Translation.no_results_found);
                    dlg.Add(pItem);
                    pItem.ItemId = 1;
                }
                else
                {
                    int nCount = 0;
                    foreach (CItem item in descriptor.m_List)
                    {
                        pItem = new GUIListItem(item.m_sName);
                        dlg.Add(pItem);
                        pItem.ItemId = 10 + nCount;
                        nCount++;
                    }
                }

                dlg.DoModal(GUIWindowManager.ActiveWindow);
                if (dlg.SelectedId == -1)
                {
                    return ReturnCode.Cancel;
                }
                else
                {
                    if (dlg.SelectedId == 0)
                    {
                        return ReturnCode.Ignore;
                    }
                    else if (dlg.SelectedId >= 10)
                    {
                        CItem DlgSelected = descriptor.m_List[dlg.SelectedId - 10];
                        m_selected = new CItem(descriptor.m_sItemToMatch, String.Empty, DlgSelected.m_Tag);
                        return ReturnCode.OK;
                    }
                    else return ReturnCode.Cancel;
                }
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("The ChooseFromSelection Method has generated an error: " + ex.Message);
                m_selected = null;
                return ReturnCode.Cancel;
            }
            finally
            {
                this.m_Facade.Focus = true;
            }
        }

        private delegate ReturnCode YesNoOkDialogDelegate(ChooseFromYesNoDescriptor descriptor);
        public ReturnCode YesNoOkDialog(ChooseFromYesNoDescriptor descriptor)
        {
            ReturnCode returnCode = ReturnCode.OK;
            if (this.m_Facade == null)
            {
                return ReturnCode.NotReady;
            }

            if (m_localControlForInvoke.InvokeRequired)
            {
                returnCode = (ReturnCode)m_localControlForInvoke.Invoke(new YesNoOkDialogDelegate(YesNoOkDialogSync), new Object[] { descriptor });
            }
            else
            {
                returnCode = YesNoOkDialogSync(descriptor);
            }
            return returnCode;
        }

        public ReturnCode YesNoOkDialogSync(ChooseFromYesNoDescriptor descriptor)
        {
            try
            {
                switch (descriptor.m_dialogButtons)
                {
                    case DialogButtons.OK:
                        GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);

                        if (dlgOK == null)
                            return ReturnCode.Cancel;
                        //dlgOK.Reset(); //breaking for users of older MP versions?
                        dlgOK.SetHeading(descriptor.m_sTitle);
                        dlgOK.SetLine(1, descriptor.m_sLabel);
                        dlgOK.DoModal(GUIWindowManager.ActiveWindow);

                        return ReturnCode.OK;

                    case DialogButtons.YesNo:
                    case DialogButtons.YesNoCancel:
                        GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);

                        if (dlgYesNo == null)
                            return ReturnCode.No;
                        dlgYesNo.Reset(); //breaking for users of older MP versions?
                        dlgYesNo.SetHeading(descriptor.m_sTitle);
                        dlgYesNo.SetLine(1, descriptor.m_sLabel);
                        if (descriptor.m_dialogDefaultButton == ReturnCode.Yes)
                            dlgYesNo.SetDefaultToYes(true);
                        else
                            dlgYesNo.SetDefaultToYes(false);
                        dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);

                        if (dlgYesNo.IsConfirmed)
                            return ReturnCode.Yes;
                        else
                            return ReturnCode.No;
                }
                return ReturnCode.No;
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("The YesNoOkDialog Method has generated an error: " + ex.Message);
                return ReturnCode.Cancel;
            }
            finally
            {
                this.m_Facade.Focus = true;
            }
        }

        private delegate ReturnCode GetStringFromUserDelegate(GetStringFromUserDescriptor descriptor);
        private string m_sUserInput;
        public ReturnCode GetStringFromUser(GetStringFromUserDescriptor descriptor, out String input)
        {
            if (this.m_Facade == null)
            {
                input = string.Empty;
                return ReturnCode.NotReady;
            }

            ReturnCode returnCode;
            if (m_localControlForInvoke.InvokeRequired)
            {
                returnCode = (ReturnCode)m_localControlForInvoke.Invoke(new GetStringFromUserDelegate(GetStringFromUserSync), new Object[] { descriptor });
            }
            else
                returnCode = GetStringFromUserSync(descriptor);
            input = m_sUserInput;

            return returnCode;
        }

        public ReturnCode GetStringFromUserSync(GetStringFromUserDescriptor descriptor)
        {
            try
            {
                m_sUserInput = String.Empty;
                VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                if (null == keyboard)
                    return ReturnCode.Cancel;

                keyboard.Reset();
                keyboard.Text = descriptor.m_sText;
                keyboard._shiftTurnedOn = true;
                keyboard.DoModal(GUIWindowManager.ActiveWindow);

                if (keyboard.IsConfirmed)
                {
                    m_sUserInput = keyboard.Text;
                    return ReturnCode.OK;
                }
                else
                    return ReturnCode.Cancel;
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("The GetStringFromUser Method has generated an error: " + ex.Message);
                m_sUserInput = String.Empty;
                return ReturnCode.Cancel;
            }
            finally
            {
                this.m_Facade.Focus = true;
            }
        }

        private void playRandomEp()
        {
            List<DBEpisode> episodeList = m_CurrLView.getAllEpisodesForStep(m_CurrViewStep, m_stepSelection);
            DBEpisode selectedEpisode = episodeList[new Random().Next(0, episodeList.Count)];

            MPTVSeriesLog.Write("Selected Random Episode: ", selectedEpisode[DBEpisode.cCompositeID].ToString(), MPTVSeriesLog.LogLevel.Normal);

            // removed the if statement here to mimic functionality when an episode is selected
            // via the regular UI, since watched flag is now set after viewing (is this right?)
            m_VideoHandler.ResumeOrPlay(selectedEpisode);
        }

        private void setProcessAnimationStatus(bool enable)
        {            
            try
            {
                if (m_ImportAnimation != null)
                {
                    if (enable)
                        m_ImportAnimation.AllocResources();
                    else
                        m_ImportAnimation.FreeResources();
                    m_ImportAnimation.Visible = enable;                  
                }
            }
            catch (Exception)
            {                
            }
        }

        private void setUpFolderWatches()
        {
            List<String> importFolders = new List<String>();
            DBImportPath[] importPaths = DBImportPath.GetAll();
            if (importPaths != null)
            {
                // ok let's see ... go through all enable import folders, and add a watchfolder on it
                foreach (DBImportPath importPath in importPaths)
                {
                    if (importPath[DBImportPath.cEnabled] != 0)
                        importFolders.Add(importPath[DBImportPath.cPath]);
                }
            }

            m_watcherUpdater = new Watcher(importFolders);
            m_watcherUpdater.WatcherProgress += new Watcher.WatcherProgressHandler(watcherUpdater_WatcherProgress);
            m_watcherUpdater.StartFolderWatch();
        }
        
        public static void pushFieldsToSkin(DBTable item, string pre)
        {
            if (item == null) return;
            List<string> fieldsRequestedForPre = null;

            if (SkinSettings.SkinProperties.ContainsKey(pre))
            {
                fieldsRequestedForPre = SkinSettings.SkinProperties[pre];
                for (int i = 0; i < fieldsRequestedForPre.Count; i++)
                {
                    pushFieldToSkin(item, pre, fieldsRequestedForPre[i]);
                }
            }
        }

        private static void pushFieldToSkin(DBTable item, string pre, string field)
        {
            string t = pre + "." + field;
            setGUIProperty(t, FieldGetter.resolveDynString("<" + t + ">", item));
        }

        public static void clearFieldsForskin(string pre)
        {
            if (SkinSettings.SkinProperties.ContainsKey(pre))
            {
                List<string> fields = SkinSettings.SkinProperties[pre];
                for (int i = 0; i < fields.Count; i++)
                    clearGUIProperty(pre + "." + fields[i]);
            }
        }

        FanartChooser fc = null;
        public void ShowFanartChooser(int seriesID)
        {
            if (!DBOnlineMirror.IsMirrorsAvailable)
            {
                // Server maybe available now.
                DBOnlineMirror.Init();
                if (!DBOnlineMirror.IsMirrorsAvailable)
                {
                    GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                    dlgOK.SetHeading(Translation.TVDB_ERROR_TITLE);
                    if (!TVSeriesPlugin.IsNetworkAvailable)
                    {
                        dlgOK.SetLine(1, Translation.NETWORK_ERROR_UNAVAILABLE);
                    }
                    else
                        dlgOK.SetLine(1, Translation.TVDB_ERROR_UNAVAILABLE);
                    
                    dlgOK.DoModal(GUIWindowManager.ActiveWindow);
                    MPTVSeriesLog.Write("Cannot Display Fanart Chooser, the online database is unavailable or network is unavailable.");
                    return;
                }
            }

            MPTVSeriesLog.Write("Switching to Fanart Chooser Window");
            // lets show the other xml

            if (fc == null)
            {
                fc = new FanartChooser();
                GUIWindow fcwindow = (GUIWindow)fc;
                GUIWindowManager.Add(ref fcwindow);
                fc.Init();
            }
            fc.SeriesID = seriesID;
            fc.setPageTitle(Translation.FanArt);
            //if (listLevel == Listlevel.Season || listLevel == Listlevel.Episode)
            //    fc.setPageTitle(GUIPropertyManager.GetProperty("#TVSeries." + guiProperty.CurrentView) + " -> Fanart");
            //else fc.setPageTitle(GUIPropertyManager.GetProperty("#TVSeries." + guiProperty.CurrentView) + " -> " + m_SelectedSeries[DBOnlineSeries.cPrettyName] + " -> Fanart");            
            GUIWindowManager.ActivateWindow(fc.GetID, false);
        }

        GUITVSeriesPlayList TVSeriesPlaylist = null;
        public void ShowPlaylistWindow()
        {         
            MPTVSeriesLog.Write("Switching to Playlist Window");

            if (TVSeriesPlaylist == null)
            {
                TVSeriesPlaylist = new GUITVSeriesPlayList();
                GUIWindow window = (GUIWindow)TVSeriesPlaylist;
                GUIWindowManager.Add(ref window);
                TVSeriesPlaylist.Init();
            }
            GUIWindowManager.ActivateWindow(TVSeriesPlaylist.GetID, false);
        }

        protected void AddItemToPlayList()
        {
            if (_playlistPlayer == null)
            {
                _playlistPlayer = PlayListPlayer.SingletonPlayer;
                _playlistPlayer.PlaylistAutoPlay = DBOption.GetOptions(DBOption.cPlaylistAutoPlay);
                _playlistPlayer.RepeatPlaylist = DBOption.GetOptions(DBOption.cRepeatPlaylist);
            }

            SQLCondition condition = new SQLCondition();
            List<DBEpisode> episodes;

            if (this.listLevel == Listlevel.Group)
            {
                
            }
            else if (this.listLevel == Listlevel.Series && m_SelectedSeries != null)
            {
                condition.Add(new DBEpisode(), DBEpisode.cSeriesID, m_SelectedSeries[DBSeries.cID], SQLConditionType.Equal);
            }
            else if (this.listLevel == Listlevel.Season && m_SelectedSeason != null)
            {
                condition.Add(new DBEpisode(), DBEpisode.cSeriesID, m_SelectedSeries[DBSeries.cID], SQLConditionType.Equal);
                condition.Add(new DBEpisode(), DBEpisode.cSeasonIndex, m_SelectedSeason[DBSeason.cIndex], SQLConditionType.Equal);
            }
            else if (this.listLevel == Listlevel.Episode && m_SelectedEpisode != null)
            {
                condition.Add(new DBEpisode(), DBEpisode.cSeriesID, m_SelectedSeries[DBSeries.cID], SQLConditionType.Equal);
                condition.Add(new DBEpisode(), DBEpisode.cSeasonIndex, m_SelectedSeason[DBSeason.cIndex], SQLConditionType.Equal);
                condition.Add(new DBEpisode(), DBEpisode.cEpisodeIndex, m_SelectedEpisode[DBEpisode.cEpisodeIndex], SQLConditionType.Equal);
            }

            episodes = DBEpisode.Get(condition, false);
            foreach (DBEpisode episode in episodes)
            {
                PlayListItem playlistItem = new PlayListItem(episode);                              
                _playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_TVSERIES).Add(playlistItem);
            }   
   
            // Select next item in list
            int item = m_Facade.SelectedListItemIndex;
            if (item < m_Facade.Count)
                m_Facade.SelectedListItemIndex = item + 1;        

        }

        protected void OnShowSavedPlaylists(string _directory)
        {            
            VirtualDirectory _virtualDirectory = new VirtualDirectory();
            _virtualDirectory.AddExtension(".tvsplaylist");

            List<GUIListItem> itemlist = _virtualDirectory.GetDirectoryExt(_directory);
            if (_directory == DBOption.GetOptions(DBOption.cPlaylistPath))
                itemlist.RemoveAt(0);

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

            if (_playlistPlayer == null)
            {
                _playlistPlayer = PlayListPlayer.SingletonPlayer;
                _playlistPlayer.PlaylistAutoPlay = DBOption.GetOptions(DBOption.cPlaylistAutoPlay);
                _playlistPlayer.RepeatPlaylist = DBOption.GetOptions(DBOption.cRepeatPlaylist);
            }
            _playlistPlayer.CurrentPlaylistName = System.IO.Path.GetFileNameWithoutExtension(strPlayList);

            if (playlist.Count == 1 && _playlistPlayer.PlaylistAutoPlay)
            {
                MPTVSeriesLog.Write(string.Format("GUITVSeriesPlaylist: play single playlist item - {0}", playlist[0].FileName));
                if (g_Player.Play(playlist[0].FileName))
                {
                    if (MediaPortal.Util.Utils.IsVideo(playlist[0].FileName))
                    {
                        g_Player.ShowFullScreenWindow();
                    }
                }
                return;
            }

            // clear current playlist
            _playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_TVSERIES).Clear();

            // add each item of the playlist to the playlistplayer
            for (int i = 0; i < playlist.Count; ++i)
            {
                PlayListItem playListItem = playlist[i];
                _playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_TVSERIES).Add(playListItem);
            }

            // if we got a playlist
            if (_playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_TVSERIES).Count > 0)
            {
                // then get 1st item
                playlist = _playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_TVSERIES);
                PlayListItem item = playlist[0];                

                // and activate the playlist window
                ShowPlaylistWindow();

                // and start playing it
                if (_playlistPlayer.PlaylistAutoPlay)
                {
                    _playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_TVSERIES;
                    _playlistPlayer.Reset();
                    _playlistPlayer.Play(0);
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

        ~TVSeriesPlugin()
        {            
            // so that locallogos can clean up its stuff
            if (null != this.m_Logos_Image)
            {
                this.m_Logos_Image.FreeResources();
                this.m_Logos_Image = null;
            }
            // only when inside MP
            if (!Settings.isConfig) localLogos.cleanUP();
        }
    }

    enum BackGroundLoadingArgumentType
    {
        None,
        FullElement,
        ElementForDelayedImgLoading,
        DelayedImgLoading,
        DelayedImgInit,
        ElementSelection,
        SkipSeasonDown,
        SkipSeasonUp,
        SetFacadeMode
    }

    class BackgroundFacadeLoadingArgument
    {
        public BackGroundLoadingArgumentType Type = BackGroundLoadingArgumentType.None;

        public object Argument = null;
        public int IndexArgument = 0;
    }
}



