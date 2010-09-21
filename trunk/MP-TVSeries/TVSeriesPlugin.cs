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
using System.IO;
using System.ComponentModel;
using MediaPortal.Configuration;
using MediaPortal.GUI.Library;
using MediaPortal.GUI.Video;
using MediaPortal.Player;
using MediaPortal.Dialogs;
using MediaPortal.Util;
using Action = MediaPortal.GUI.Library.Action;
using WindowPlugins.GUITVSeries;
using System.Threading;
using System.Collections.Generic;
using WindowPlugins.GUITVSeries.Feedback;
using Download = WindowPlugins.GUITVSeries.Download;
using Torrent = WindowPlugins.GUITVSeries.Torrent;
using Newzbin = WindowPlugins.GUITVSeries.Newzbin;
using aclib.Performance;
using Cornerstone.MP;
using System.Xml;
using WindowPlugins.GUITVSeries.Subtitles;
using SubtitleDownloader.Core;

namespace WindowPlugins.GUITVSeries
{
    //[PluginIcons("WindowPlugins.GUITVSeries.Resources.Images.icon_normal.png", "WindowPlugins.GUITVSeries.Resources.Images.icon_faded.png")]
    public class TVSeriesPlugin : GUIWindow, ISetupForm, IFeedback {

        #region Constructor
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
            seasonbanner.Property = "#TVSeries.SeasonPoster";
            seasonbanner.Delay = artworkDelay;
            
            MPTVSeriesLog.Write(string.Format("MP-TVSeries Version: v{0}", Settings.Version.ToString()));
            MPTVSeriesLog.Write("MP-TVSeries Build Date: " + Settings.BuildDate);
        }
        #endregion

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

		#region Varibles
		private ImageSwapper backdrop;
        private AsyncImageResource seriesbanner = null;
        private AsyncImageResource seriesposter = null;
        private AsyncImageResource seasonbanner = null;

        private Listlevel listLevel = Listlevel.Series;        
		private DBSeries m_SelectedSeries;
        private DBSeason m_SelectedSeason;
        public static DBEpisode m_SelectedEpisode;
		
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
        private bool subtitleDownloaderWorking = false;
        private List<string> availableSubtitleDownloaderNames = null;
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
        public static DateTime m_LastUpdateScan = DateTime.MinValue;

        private int m_nInitialIconXOffset = 0;
        private int m_nInitialIconYOffset = 0;
        private int m_nInitialItemHeight = 0;

        public static String m_sFormatSeriesCol1 = String.Empty;
        public static String m_sFormatSeriesCol2 = String.Empty;
        public static String m_sFormatSeriesCol3 = String.Empty;
        public static String m_sFormatSeriesTitle = String.Empty;
        public static String m_sFormatSeriesSubtitle = String.Empty;
        public static String m_sFormatSeriesMain = String.Empty;

        public static String m_sFormatSeasonCol1 = String.Empty;
        public static String m_sFormatSeasonCol2 = String.Empty;
        public static String m_sFormatSeasonCol3 = String.Empty;
        public static String m_sFormatSeasonTitle = String.Empty;
        public static String m_sFormatSeasonSubtitle = String.Empty;
        public static String m_sFormatSeasonMain = String.Empty;

        public static String m_sFormatEpisodeCol1 = String.Empty;
        public static String m_sFormatEpisodeCol2 = String.Empty;
        public static String m_sFormatEpisodeCol3 = String.Empty;
        public static String m_sFormatEpisodeTitle = String.Empty;
        public static String m_sFormatEpisodeSubtitle = String.Empty;
        public static String m_sFormatEpisodeMain = String.Empty;

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
        private bool m_bPluginLoaded = false;
        private bool m_bShowLastActiveModule = false;
        private int m_iLastActiveModule = 0;
        
		#endregion

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

		#region Enums
		enum eContextItems {
			toggleWatched,
			cycleSeriesBanner,
			cycleSeriesPoster,
			cycleSeasonPoster,
			forceSeriesQuery,
			downloadSubtitle,
			downloadviaTorrent,
			downloadviaNewz,
			actionMarkAllWatched,
			actionMarkAllUnwatched,
			actionToggleFavorite,
			actionHide,
			actionDelete,
			actionUpdate,
			actionLocalScan,
			actionFullRefresh,
            actionChangeOnlineEpisodeMatchOrder,
            actionEpisodeSortBy,
			actionPlayRandom,
            actionLockViews,
            actionResetIgnoredDownloadedFiles,
			optionsOnlyShowLocal,
            optionsShowHiddenItems,
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

		enum eContextMenus {
			download = 100,
			action,
			options,
			rate,
			switchView,
			switchLayout,
			addToView,
			removeFromView
		}

		public enum DeleteMenuItems {
			disk,
			database,
			diskdatabase,
            subtitles,
			cancel
		}

        public enum EpisodeSortByMenuItems
        {
            Aired,
            DVD,
            Absolute,   // used for matching only
            Title       // used for matching only
        }

        enum Listlevel
        {
            Episode,
            Season,
            Series,
            Group
		}

		enum SkipSeasonCodes {
			none,
			SkipSeasonDown,
			SkipSeasonUp
		}

		enum guiProperty {
			Title,
			Subtitle,
			Description,
			CurrentView,
			SimpleCurrentView,
			NextView,
			LastView,
			SeriesBanner,
			SeriesPoster,
			SeasonPoster,
			EpisodeImage,
			Logos,
			SeriesCount,
			GroupCount,
            FilteredEpisodeCount,
            WatchedCount,
            UnWatchedCount,
            LastOnlineUpdate
		}

        public enum OnPlaySeriesOrSeasonAction {
            DoNothing,
            Random,
            FirstUnwatched,
            RandomUnwatched,
            Latest,
            AlwaysAsk
        }
		#endregion

		#region Base Overrides
		public override int GetID
        {
            get
            {
                return 9811;
            }
        }

		/// <summary>
		/// MediaPortal will set #currentmodule with GetModuleName()
		/// </summary>
		/// <returns>Localized Window Name</returns>
		//public override string GetModuleName() {
		//	return pluginName;
		//}

        public override bool Init()
        {
            m_localControlForInvoke = new Control();
            m_localControlForInvoke.CreateControl();

            MPTVSeriesLog.Write("**** Plugin started in MediaPortal ****");

            // Get Logging Level
            MPTVSeriesLog.selectedLogLevel = (MPTVSeriesLog.LogLevel)(int)DBOption.GetOptions("logLevel");
            DBOption.LogOptions();

            #region Translations
            Translation.Init();

            // Push Translated Strings to skin
            MPTVSeriesLog.Write("Setting translated strings: ", MPTVSeriesLog.LogLevel.Debug);
            string propertyName = string.Empty;
            string propertyValue = string.Empty;
            foreach (string name in Translation.Strings.Keys)
            {
                propertyName = "#TVSeries.Translation." + name + ".Label";
                propertyValue = Translation.Strings[name];
                MPTVSeriesLog.Write(propertyName + " = " + propertyValue, MPTVSeriesLog.LogLevel.Debug);
                GUIPropertyManager.SetProperty(propertyName, propertyValue);
            }
            #endregion

            #region Misc
            Download.Monitor.Start(this);
            
            m_VideoHandler = new VideoHandler();                        
            m_VideoHandler.RateRequestOccured += new VideoHandler.rateRequest(m_VideoHandler_RateRequestOccured);

            // Setup Random Fanart Timer
            m_FanartTimer = new System.Threading.Timer(new TimerCallback(FanartTimerEvent), null, Timeout.Infinite, Timeout.Infinite);
            m_bFanartTimerDisabled = true;

            // Lock for Parental Control
            logicalView.IsLocked = true;

            // Check if MediaPortal will Show TVSeries Plugin when restarting
            // We need to do this because we may need to show a modal dialog e.g. PinCode and we can't do this if MediaPortal window is not yet ready            
            using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml"))) {
                m_bShowLastActiveModule = xmlreader.GetValueAsBool("general", "showlastactivemodule", false);
                m_iLastActiveModule = xmlreader.GetValueAsInt("general", "lastactivemodule", -1);
            }
            
            // subtitle downloader background tasks
            BackgroundWorker subsChecker = new BackgroundWorker();
            subsChecker.DoWork += new DoWorkEventHandler(GetSubtitleDownloaderNames);
            subsChecker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GetSubtitleDownloaderNames_Completed);
            subsChecker.RunWorkerAsync();
            #endregion

            #region Initialize Importer
            // timer check every second
            m_timerDelegate = new TimerCallback(Clock);
            int importDelay = DBOption.GetOptions(DBOption.cImportDelay);
            MPTVSeriesLog.Write("Starting initial import scan in: {0} secs", importDelay);
            m_scanTimer = new System.Threading.Timer(m_timerDelegate, null, importDelay*1000, 1000);

            m_parserUpdater = new OnlineParsing(this);
            m_parserUpdater.OnlineParsingProgress += new OnlineParsing.OnlineParsingProgressHandler(parserUpdater_OnlineParsingProgress);
            m_parserUpdater.OnlineParsingCompleted += new OnlineParsing.OnlineParsingCompletedHandler(parserUpdater_OnlineParsingCompleted);

            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += NetworkAvailabilityChanged;
            Microsoft.Win32.SystemEvents.PowerModeChanged += new Microsoft.Win32.PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
           
            try
            {
                m_LastUpdateScan = DateTime.Parse(DBOption.GetOptions(DBOption.cImport_OnlineUpdateScanLastTime));
            }
            catch { }

            if (DBOption.GetOptions(DBOption.cImport_AutoUpdateOnlineData))
                m_nUpdateScanLapse = DBOption.GetOptions(DBOption.cImport_AutoUpdateOnlineDataLapse);

            if (DBOption.GetOptions(DBOption.cImport_FolderWatch))
            {
                DeviceManager.StartMonitor();
                setUpFolderWatches();
            }

            // do a local scan when starting up the app if enabled - later on the watcher will monitor changes            
            if (DBOption.GetOptions(DBOption.cImport_ScanOnStartup))
            {
                // if online updates are required then this will be picked up 
                // in clock timer where last update time is compared
                m_parserUpdaterQueue.Add(new CParsingParameters(true, false));
            }
            else
            {
                // else the user has selected to always manually do local scans
                setProcessAnimationStatus(false);
            }
            #endregion

            #region Skin Settings / Load
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

            // Load all Skin Fields being used
            string[] skinFiles = Directory.GetFiles(GUIGraphicsContext.Skin, "TVSeries*.xml");
            foreach (string skinFile in skinFiles)
            {
                MPTVSeriesLog.Write("Loading Skin Properties in: " + skinFile);
                SkinSettings.GetSkinProperties(skinFile);
            }
            SkinSettings.LogSkinProperties();

            String xmlSkin = GUIGraphicsContext.Skin + @"\TVSeries.xml";
            MPTVSeriesLog.Write("Loading main skin window: " + xmlSkin);
            return Load(xmlSkin);
            #endregion
		}

        public override void DeInit()
        {
            base.DeInit();

            DeviceManager.StopMonitor();
        }

        protected override void OnPageLoad() {
            MPTVSeriesLog.Write("OnPageLoad() started.", MPTVSeriesLog.LogLevel.Debug);
            if (m_Facade == null) 
            {
				// Most likely the skin does not exist
				GUIDialogOK dlg = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
				dlg.Reset();
				dlg.SetHeading(Translation.wrongSkin);
				dlg.DoModal(GetID);
				GUIWindowManager.ShowPreviousWindow();
				return;
			}

            MediaPortal.GUI.Library.GUIPropertyManager.SetProperty("#currentmodule", pluginName);

            ImageAllocator.SetFontName(m_Facade.AlbumListView == null ? m_Facade.ListView.FontName : m_Facade.AlbumListView.FontName);

            #region Clear GUI Properties
            // Clear GUI Properties when first entering the plugin
            // This will avoid ugly property names being seen before 
            // its corresponding value is assigned
            if (!m_bPluginLoaded) {
                clearGUIProperty(guiProperty.Subtitle);
                clearGUIProperty(guiProperty.Title);
                clearGUIProperty(guiProperty.Description);

                clearGUIProperty(guiProperty.CurrentView);
                clearGUIProperty(guiProperty.SimpleCurrentView);
                clearGUIProperty(guiProperty.NextView);
                clearGUIProperty(guiProperty.LastView);
                clearGUIProperty(guiProperty.SeriesCount);
                clearGUIProperty(guiProperty.GroupCount);
                clearGUIProperty(guiProperty.FilteredEpisodeCount);
                clearGUIProperty(guiProperty.WatchedCount);
                clearGUIProperty(guiProperty.UnWatchedCount);

                clearFieldsForskin("Series");
                clearFieldsForskin("Season");
                clearFieldsForskin("Episode");
            }
            #endregion

            localLogos.appendEpImage = m_Episode_Image == null ? true : false;

			bool viewSwitched = false;

            // Initialize View, also check if current view is locked after exiting and re-entering plugin
			if (m_CurrLView == null || (m_CurrLView.ParentalControl && logicalView.IsLocked)) {				
				// Get available Views
				m_allViews = logicalView.getAll(false);
				if (m_allViews.Count > 0) {
					try {
						viewSwitched = switchView((string)DBOption.GetOptions("lastView"));
					}
					catch {
						viewSwitched = false;
						MPTVSeriesLog.Write("Error when switching view");
					}
				}
				else {
					viewSwitched = false;
					MPTVSeriesLog.Write("Error, cannot display items because no Views have been found!");
				}
			}
			else {
				viewSwitched = true;
				setViewLabels();
			}            

			// If unable to load view, exit
			if (!viewSwitched) {
				GUIWindowManager.ShowPreviousWindow();
				return;
			}

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

			if (m_Logos_Image != null) {
				logosHeight = m_Logos_Image.Height;
				logosWidth = m_Logos_Image.Width;
			}

            m_bPluginLoaded = true;
            
            Helper.disableNativeAutoplay();

            // Ask to Rate Episode, onPageLoad is triggered after returning from player
            if (ask2Rate != null) {
                showRatingsDialog(ask2Rate, true);
                ask2Rate = null;
                // Refresh the facade if we want to see the submitted rating
                if (this.listLevel == Listlevel.Episode) {
                    LoadFacade();
                }
            }
            
            // Push last update time to skin
            setGUIProperty(guiProperty.LastOnlineUpdate, DBOption.GetOptions(DBOption.cImport_OnlineUpdateScanLastTime));

            MPTVSeriesLog.Write("OnPageLoad() completed.", MPTVSeriesLog.LogLevel.Debug);

		}	

		protected override void OnPageDestroy(int new_windowId) {
            MPTVSeriesLog.Write("OnPageDestroy() started.", MPTVSeriesLog.LogLevel.Debug);
            
            // Disable Random Fanart Timer
			m_FanartTimer.Change(Timeout.Infinite, Timeout.Infinite);
			m_bFanartTimerDisabled = true;
                        
            Helper.enableNativeAutoplay();

			base.OnPageDestroy(new_windowId);
        
            MPTVSeriesLog.Write("OnPageDestroy() completed.", MPTVSeriesLog.LogLevel.Debug);
        }

		#region Main Context Menu
		protected override void OnShowContextMenu() {
			try {
				GUIListItem currentitem = this.m_Facade.SelectedListItem;
				if (currentitem == null) return;
				
				IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
				if (dlg == null) return;

				DBEpisode selectedEpisode = null;
				DBSeason selectedSeason = null;
				DBSeries selectedSeries = null;

				bool emptyList = currentitem.Label == Translation.No_items;
				if (!emptyList) {
					switch (this.listLevel) {
						case Listlevel.Episode: {
								selectedEpisode = (DBEpisode)currentitem.TVTag;
								selectedSeason = Helper.getCorrespondingSeason(selectedEpisode[DBEpisode.cSeriesID], selectedEpisode[DBEpisode.cSeasonIndex]);
								selectedSeries = Helper.getCorrespondingSeries(selectedEpisode[DBEpisode.cSeriesID]);
							}
							break;

						case Listlevel.Season: {
								selectedSeason = (DBSeason)currentitem.TVTag;
								selectedSeries = Helper.getCorrespondingSeries(selectedSeason[DBSeason.cSeriesID]);
							}
							break;

						case Listlevel.Series: {
								selectedSeries = (DBSeries)currentitem.TVTag;
							}
							break;
					}
				}
				bool bExitMenu = false;
				do {
					dlg.Reset();
					GUIListItem pItem = null;

                    bool subtitleDownloaderEnabled = SubtitleDownloaderEnabledAndHasSites() || (DBOption.GetOptions(DBOption.cSubCentralEnabled) && Helper.IsSubCentralAvailableAndEnabled);
					bool newsEnable = System.IO.File.Exists(DBOption.GetOptions(DBOption.cNewsLeecherPath));
					bool torrentsEnable = System.IO.File.Exists(DBOption.GetOptions(DBOption.cUTorrentPath));

					if (!emptyList) {
						switch (this.listLevel) {
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
						if (this.listLevel == Listlevel.Episode) {
							pItem = new GUIListItem(Translation.Toggle_watched_flag);
							dlg.Add(pItem);
							pItem.ItemId = (int)eContextItems.toggleWatched;

							if (!String.IsNullOrEmpty(DBOption.GetOptions(DBOption.cOnlineUserID))) {
								pItem = new GUIListItem(Translation.RateEpisode + " ...");
								dlg.Add(pItem);
								pItem.ItemId = (int)eContextMenus.rate;								
							}
						}
						else if (this.listLevel != Listlevel.Group) {
							if (!String.IsNullOrEmpty(DBOption.GetOptions(DBOption.cOnlineUserID))) {
                                pItem = new GUIListItem(Translation.RateSeries + " ...");
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
						if (this.listLevel != Listlevel.Group) {
							pItem = new GUIListItem(Translation.AddToPlaylist);
							dlg.Add(pItem);
							pItem.ItemId = (int)eContextItems.addToPlaylist;
						}

						if (this.listLevel != Listlevel.Group) {
							if (m_SelectedSeries != null && FanartBackground != null && // only if skins supports it
								m_SelectedSeries[DBOnlineSeries.cID] > 0) {
								pItem = new GUIListItem(Translation.FanArt + " ...");
								dlg.Add(pItem);
								pItem.ItemId = (int)eContextItems.showFanartChooser;
							}
						}

						if (this.listLevel == Listlevel.Series) {
							if (selectedSeries.PosterList.Count > 1) {
								pItem = new GUIListItem(Translation.CycleSeriesPoster);
								dlg.Add(pItem);
								pItem.ItemId = (int)eContextItems.cycleSeriesPoster;
							}

							if (selectedSeries.BannerList.Count > 1) {
								pItem = new GUIListItem(Translation.CycleSeriesBanner);
								dlg.Add(pItem);
								pItem.ItemId = (int)eContextItems.cycleSeriesBanner;
							}

							pItem = new GUIListItem(Translation.Force_Online_Match);
							dlg.Add(pItem);
							pItem.ItemId = (int)eContextItems.forceSeriesQuery;
						}

						// Season View may not be available so show cycle season banner at episode level as well
						if (this.listLevel == Listlevel.Season || this.listLevel == Listlevel.Episode) {
							if (selectedSeason.BannerList.Count > 1) {
								pItem = new GUIListItem(Translation.CycleSeasonBanner);
								dlg.Add(pItem);
								pItem.ItemId = (int)eContextItems.cycleSeasonPoster;
							}
						}
						
						// Can always add to existing or new view
						if (listLevel == Listlevel.Series) {
							pItem = new GUIListItem(Translation.AddViewTag + " ...");
							dlg.Add(pItem);
							pItem.ItemId = (int)eContextMenus.addToView;
						}
						// Dont show if not a member of any view
						if (listLevel == Listlevel.Series) {
							if (!String.IsNullOrEmpty(selectedSeries[DBOnlineSeries.cViewTags])) {
								pItem = new GUIListItem(Translation.RemoveViewTag + " ...");
								dlg.Add(pItem);
								pItem.ItemId = (int)eContextMenus.removeFromView;
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

					if (SkinSettings.GetLayoutCount(this.listLevel.ToString()) > 1) {
						pItem = new GUIListItem(Translation.ChangeLayout + " ...");
						dlg.Add(pItem);
						pItem.ItemId = (int)eContextMenus.switchLayout;
					}

					if (listLevel != Listlevel.Group) {
						pItem = new GUIListItem(Translation.Actions + " ...");
						dlg.Add(pItem);
						pItem.ItemId = (int)eContextMenus.action;
					}

					pItem = new GUIListItem(Translation.Options + " ...");
					dlg.Add(pItem);
					pItem.ItemId = (int)eContextMenus.options;
					#endregion

                    #region Download menu - keep at the bottom for fast access (menu + up => there)
                    if (!emptyList) {
                        if (subtitleDownloaderEnabled || newsEnable || torrentsEnable)
                        {
                            if (listLevel != Listlevel.Group)
                            {
                                if (this.listLevel == Listlevel.Episode)
                                {
                                    pItem = new GUIListItem(Translation.Download + " ...");
                                    dlg.Add(pItem);
                                    pItem.ItemId = (int)eContextMenus.download;
                                }
                            }
                        }
                    }
                    #endregion

                    dlg.DoModal(GUIWindowManager.ActiveWindow);
					#region Selected Menu Item Actions (Sub-Menus)
					switch (dlg.SelectedId) {
						case (int)eContextMenus.download: {
								dlg.Reset();
								dlg.SetHeading(Translation.Download);

								if (subtitleDownloaderEnabled) {
									pItem = new GUIListItem(Translation.Retrieve_Subtitle);
									dlg.Add(pItem);
									pItem.ItemId = (int)eContextItems.downloadSubtitle;
								}

								if (newsEnable) {
									pItem = new GUIListItem(Translation.Load_via_NewsLeecher);
									dlg.Add(pItem);
									pItem.ItemId = (int)eContextItems.downloadviaNewz;
								}

								if (torrentsEnable) {
									pItem = new GUIListItem(Translation.Load_via_Torrent);
									dlg.Add(pItem);
									pItem.ItemId = (int)eContextItems.downloadviaTorrent;
								}

								dlg.DoModal(GUIWindowManager.ActiveWindow);
								if (dlg.SelectedId != -1)
									bExitMenu = true;
							}
							break;

						case (int)eContextMenus.action: {
								dlg.Reset();
								dlg.SetHeading(Translation.Actions);
								if (listLevel != Listlevel.Group) {
									if (DBOption.GetOptions(DBOption.cShowDeleteMenu)) {
										pItem = new GUIListItem(Translation.Delete + " ...");
										dlg.Add(pItem);
										pItem.ItemId = (int)eContextItems.actionDelete;
									}

									if (!m_parserUpdaterWorking) {
										pItem = new GUIListItem(Translation.Update);
										dlg.Add(pItem);
										pItem.ItemId = (int)eContextItems.actionUpdate;
									}

                                    // add hidden menu
                                    // check if item is already hidden
									pItem = new GUIListItem();
                                    switch (listLevel)
                                    {
                                        case Listlevel.Series:
                                            pItem.Label = selectedSeries[DBSeries.cHidden] ? Translation.UnHide : Translation.Hide;
                                            break;
                                        case Listlevel.Season:
                                            pItem.Label = selectedSeason[DBSeries.cHidden] ? Translation.UnHide : Translation.Hide;
                                            break;
                                        case Listlevel.Episode:
                                            pItem.Label = selectedEpisode[DBSeries.cHidden] ? Translation.UnHide : Translation.Hide;
                                            break;
                                    }
									dlg.Add(pItem);
									pItem.ItemId = (int)eContextItems.actionHide;

									pItem = new GUIListItem(Translation.updateMI);
									dlg.Add(pItem);
									pItem.ItemId = (int)eContextItems.actionRecheckMI;

									pItem = new GUIListItem(Translation.ResetUserSelections);
									dlg.Add(pItem);
									pItem.ItemId = (int)eContextItems.resetUserSelections;
								}

                                // Online to Local Episode Matching order
                                if (this.listLevel != Listlevel.Group)
                                {
                                    // get current online episode to local episode matching order
                                    string currMatchOrder = selectedSeries[DBOnlineSeries.cChosenEpisodeOrder].ToString();
                                    if (string.IsNullOrEmpty(currMatchOrder)) currMatchOrder = "Aired";

                                    pItem = new GUIListItem(Translation.ChangeOnlineMatchOrder);
                                    dlg.Add(pItem);
                                    pItem.ItemId = (int)eContextItems.actionChangeOnlineEpisodeMatchOrder;
                                }

                                // Episode Sort By
                                if (this.listLevel == Listlevel.Episode || this.listLevel == Listlevel.Season)
                                {
                                    // get current episode sort order (DVD or Aired)
                                    string currSortBy = selectedSeries[DBOnlineSeries.cEpisodeSortOrder].ToString();
                                    if (string.IsNullOrEmpty(currSortBy)) currSortBy = "Aired";

                                    pItem = new GUIListItem(string.Format("{0}: {1}", Translation.SortBy, Translation.Get(currSortBy + "Order")));
                                    dlg.Add(pItem);
                                    pItem.ItemId = (int)eContextItems.actionEpisodeSortBy;
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

                                if (!String.IsNullOrEmpty(DBOption.GetOptions(DBOption.cParentalControlPinCode))) {
                                    pItem = new GUIListItem(Translation.ParentalControlLocked);
                                    dlg.Add(pItem);
                                    pItem.ItemId = (int)eContextItems.actionLockViews;
                                }

                                if (newsEnable || torrentsEnable) // is this used by subtitles?
                                {
                                    pItem = new GUIListItem(Translation.ResetIgnoredDownloadedFiles);
                                    dlg.Add(pItem);
                                    pItem.ItemId = (int)eContextItems.actionResetIgnoredDownloadedFiles;
                                }

								dlg.DoModal(GUIWindowManager.ActiveWindow);
								if (dlg.SelectedId != -1)
									bExitMenu = true;
							}
							break;

						case (int)eContextMenus.options: {
								dlg.Reset();
								ShowOptionsMenu();
								return;
							}

						case (int)eContextMenus.switchView: {
								dlg.Reset();
								if (showViewSwitchDialog())
									return;
							}
							break;

						case (int)eContextMenus.switchLayout: {
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

						case (int)eContextMenus.rate: {
								switch (listLevel) {
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
					case (int)eContextItems.toggleWatched: {
							// toggle watched
							if (selectedEpisode != null) {
								if (selectedEpisode[DBEpisode.cFilename].ToString().Length > 0) {
									SQLCondition condition = new SQLCondition();
									condition.Add(new DBEpisode(), DBEpisode.cFilename, selectedEpisode[DBEpisode.cFilename], SQLConditionType.Equal);
									List<DBEpisode> episodes = DBEpisode.Get(condition, false);
									foreach (DBEpisode episode in episodes) {
										episode[DBOnlineEpisode.cWatched] = selectedEpisode[DBOnlineEpisode.cWatched] == 0;
										episode.Commit();
										//DBSeason.UpdateUnWatched(episode);
										//DBSeries.UpdateUnWatched(episode);
									}
								}
								else {
									selectedEpisode[DBOnlineEpisode.cWatched] = selectedEpisode[DBOnlineEpisode.cWatched] == 0;
									selectedEpisode.Commit();
									//DBSeason.UpdateUnWatched(selectedEpisode);
									//DBSeries.UpdateUnWatched(selectedEpisode);
								}
								// Update Episode Counts
								DBSeason.UpdateEpisodeCounts(m_SelectedSeries, m_SelectedSeason);
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
							DBSeries.UpdateEpisodeCounts(m_SelectedSeries);
							cache.dump();
						}
						else if (this.listLevel == Listlevel.Season && m_SelectedSeason != null) {
							DBTVSeries.Execute("update online_episodes set watched = 1 where " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID) + " = " + m_SelectedSeason[DBSeason.cSeriesID] +
												" and " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeasonIndex) + " = " + m_SelectedSeason[DBSeason.cIndex]);
							m_SelectedSeason[DBSeason.cUnwatchedItems] = false;
							m_SelectedSeason.Commit();
							DBSeason.UpdateEpisodeCounts(m_SelectedSeries, m_SelectedSeason);
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
							DBSeries.UpdateEpisodeCounts(m_SelectedSeries);
							cache.dump();
						}
						else if (this.listLevel == Listlevel.Season && m_SelectedSeason != null) {
							DBTVSeries.Execute("update online_episodes set watched = 0 where " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID) + " = " + m_SelectedSeason[DBSeason.cSeriesID] +
												" and " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeasonIndex) + " = " + m_SelectedSeason[DBSeason.cIndex]);
							m_SelectedSeason[DBSeason.cUnwatchedItems] = true;
							m_SelectedSeason.Commit();
							DBSeason.UpdateEpisodeCounts(m_SelectedSeries, m_SelectedSeason);
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
					case (int)eContextItems.cycleSeriesBanner: {
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

					case (int)eContextItems.cycleSeriesPoster: {
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

					case (int)eContextItems.cycleSeasonPoster: {
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
					case (int)eContextItems.forceSeriesQuery: {
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
					case (int)eContextItems.downloadSubtitle: {
							if (selectedEpisode != null) {
								DBEpisode episode = (DBEpisode)currentitem.TVTag;
                                ShowSubtitleMenu(episode);
							}
						}
						break;

					case (int)eContextItems.downloadviaTorrent: {
							if (selectedEpisode != null) {
								Torrent.Load torrentLoad = new Torrent.Load(this);
								torrentWorking = true;
								torrentLoad.LoadCompleted += new Torrent.Load.LoadCompletedHandler(Load_TorrentLoadCompleted);
								if (torrentLoad.Search(selectedEpisode))
									setProcessAnimationStatus(true);
							}
						}
						break;

					case (int)eContextItems.downloadviaNewz: {
							if (selectedEpisode != null) {
								Newzbin.Load load = new WindowPlugins.GUITVSeries.Newzbin.Load(this);
								load.LoadCompleted += new WindowPlugins.GUITVSeries.Newzbin.Load.LoadCompletedHandler(load_LoadNewzBinCompleted);

								if (load.Search(selectedEpisode))
									setProcessAnimationStatus(true);
							}
						}
						break;
					#endregion

					#region Favourites
					/*case (int)eContextItems.actionToggleFavorite: {
							// Toggle Favourites
							m_SelectedSeries.toggleFavourite();

							// If we are in favourite view we need to reload to remove the series
							LoadFacade();
							break;
						}*/
					#endregion

					#region Actions
					
                    #region Hide
					case (int)eContextItems.actionHide:                         
						switch (this.listLevel) 
                        {
							case Listlevel.Series:
                                selectedSeries.HideSeries(!selectedSeries[DBSeries.cHidden]);
								break;

							case Listlevel.Season:
                                selectedSeason.HideSeason(!selectedSeason[DBSeason.cHidden]);
								DBSeries.UpdateEpisodeCounts(m_SelectedSeries);
								break;

							case Listlevel.Episode:
                                selectedEpisode.HideEpisode(!selectedEpisode[DBOnlineEpisode.cHidden]);
								DBSeason.UpdateEpisodeCounts(m_SelectedSeries, m_SelectedSeason);
								break;
						}
						LoadFacade();						
						break;
					#endregion

					#region Delete
					case (int)eContextItems.actionDelete: {
							dlg.Reset();
							ShowDeleteMenu(selectedSeries, selectedSeason, selectedEpisode);
						}
						break;
					#endregion

					#region Update Series/Episode Information
					case (int)eContextItems.actionUpdate: {
							dlg.Reset();
							UpdateEpisodes(selectedSeries, m_SelectedSeason, m_SelectedEpisode);
						}
						break;
					#endregion

					#region MediaInfo
					case (int)eContextItems.actionRecheckMI:
						switch (listLevel) {
							case Listlevel.Episode:
								m_SelectedEpisode.ReadMediaInfo();
								// reload here so logos update
								LoadFacade();
								break;
							case Listlevel.Season:
								foreach (DBEpisode ep in DBEpisode.Get(m_SelectedSeason[DBSeason.cSeriesID], m_SelectedSeason[DBSeason.cIndex], false))
									ep.ReadMediaInfo();
								break;
							case Listlevel.Series:
								foreach (DBEpisode ep in DBEpisode.Get((int)m_SelectedSeries[DBSeries.cID], false))
									ep.ReadMediaInfo();
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
						lock (m_parserUpdaterQueue) {
							m_parserUpdaterQueue.Add(new CParsingParameters(true, false));
						}
						break;

					case (int)eContextItems.actionFullRefresh:
						// queue scan
						lock (m_parserUpdaterQueue) {
							m_parserUpdaterQueue.Add(new CParsingParameters(false, true));
						}
						break;
					#endregion

					#region Play
					case (int)eContextItems.actionPlayRandom:
						playRandomEp();
						break;
					#endregion

                    #region Episode Sort By
                    case (int)eContextItems.actionEpisodeSortBy:
                        ShowEpisodeSortByMenu(selectedSeries, false);
                        break;
                    #endregion

                    #region Local to Online Episode Match Order
                    case (int)eContextItems.actionChangeOnlineEpisodeMatchOrder:
                        ShowEpisodeSortByMenu(selectedSeries, true);
                        break;
                    #endregion

                    #region Ignore Downloaded Files
                    case (int)eContextItems.actionResetIgnoredDownloadedFiles:
                        DBIgnoredDownloadedFiles.ClearAll();
                        break;
                    #endregion

                    #region Lock Views
                    case (int)eContextItems.actionLockViews:
                        logicalView.IsLocked = true;
                        break;
                    #endregion

                    #endregion

                }
				#endregion
			}
			catch (Exception ex) {
				MPTVSeriesLog.Write("The 'OnShowContextMenu' function has generated an error: " + ex.Message + ", StackTrace : " + ex.StackTrace);
			}

		}
		#endregion

		public override void OnAction(Action action) {
            DBEpisode selectedEpisode = null;
            DBSeason selectedSeason = null;
            DBSeries selectedSeries = null;

            switch (action.wID) {
				case Action.ActionType.ACTION_PARENT_DIR:				
					ImageAllocator.FlushAll();
					GUIWindowManager.ShowPreviousWindow();
					break;

				case Action.ActionType.ACTION_PREVIOUS_MENU:
					// back one level
					MPTVSeriesLog.Write("ACTION_PREVIOUS_MENU", MPTVSeriesLog.LogLevel.Debug);
					if (m_CurrViewStep == 0) {
                        goto case Action.ActionType.ACTION_PARENT_DIR;
					}
					else {
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

						selectedSeries = null;
						selectedSeason = null;
						selectedEpisode = null;

						switch (this.listLevel) {
							case Listlevel.Series:
								selectedSeries = this.m_Facade.SelectedListItem.TVTag as DBSeries;
								break;
							case Listlevel.Season:
								selectedSeason = this.m_Facade.SelectedListItem.TVTag as DBSeason;
                                selectedSeries = Helper.getCorrespondingSeries(selectedSeason[DBSeason.cSeriesID]);
								break;
							case Listlevel.Episode:
								selectedEpisode = this.m_Facade.SelectedListItem.TVTag as DBEpisode;
                                selectedSeason = Helper.getCorrespondingSeason(selectedEpisode[DBEpisode.cSeriesID], selectedEpisode[DBEpisode.cSeasonIndex]);
                                selectedSeries = Helper.getCorrespondingSeries(selectedEpisode[DBEpisode.cSeriesID]);
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

                case Action.ActionType.ACTION_PLAY:
                case Action.ActionType.ACTION_MUSIC_PLAY:
                    selectedSeries = null;  
                    selectedSeason = null;
                    selectedEpisode = null;
                    string selectedGroup = null;

                    switch (this.listLevel) {
                        case Listlevel.Group:
                            selectedGroup = this.m_Facade.SelectedListItem.TVTag as string;
                            break;
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

                    OnPlaySeriesOrSeasonAction onPlayAction = (OnPlaySeriesOrSeasonAction)(int)DBOption.GetOptions(DBOption.cOnPlaySeriesOrSeasonAction);

                    m_SelectedEpisode = null;
                    if (selectedEpisode != null)
                    {
                        if (selectedEpisode[DBEpisode.cIsAvailable])
                            m_SelectedEpisode = selectedEpisode;
                        else
                            m_SelectedEpisode = null;
                    }
                    else if (selectedGroup != null || selectedSeason != null || selectedSeries != null) {

                        if (onPlayAction == OnPlaySeriesOrSeasonAction.AlwaysAsk) {
                            List<GUIListItem> items = new List<GUIListItem>();
                            items.Add(new GUIListItem(Helper.UppercaseFirst(Translation.RandomEpisode)));
                            items.Add(new GUIListItem(Helper.UppercaseFirst(Translation.FirstUnwatchedEpisode)));
                            items.Add(new GUIListItem(Helper.UppercaseFirst(Translation.RandomUnwatchedEpisode)));
                            items.Add(new GUIListItem(Helper.UppercaseFirst(Translation.LatestEpisode)));

                            onPlayAction = (OnPlaySeriesOrSeasonAction)(ShowMenuDialog(Translation.PlaySomething, items) + 1);
                        }
                        
                        if (onPlayAction != OnPlaySeriesOrSeasonAction.DoNothing) {
                            List<DBEpisode> episodeList = null;
                            if (selectedSeason != null) {
                                episodeList = m_CurrLView.getAllEpisodesForStep(m_CurrViewStep + 1, new string[] { selectedSeason[DBSeason.cSeriesID].ToString(), selectedSeason[DBSeason.cIndex].ToString() });
                            }
                            else if (selectedSeries != null) {
                                episodeList = m_CurrLView.getAllEpisodesForStep(m_CurrViewStep + 1, new string[] { selectedSeries[DBSeries.cID].ToString() });
                            }
                            else if (selectedGroup != null) {
                                episodeList = m_CurrLView.getAllEpisodesForStep(m_CurrViewStep + 1, new string[] { selectedGroup });
                            }

                            switch (onPlayAction) {
                                case OnPlaySeriesOrSeasonAction.Random:
                                    episodeList = FilterEpisodeList(episodeList, true, false);
                                    m_SelectedEpisode = GetRandomEpisode(episodeList);
                                    break;
                                case OnPlaySeriesOrSeasonAction.FirstUnwatched:
                                    episodeList = FilterEpisodeList(episodeList, true, true);
                                    m_SelectedEpisode = GetFirstOrLastEpisode(episodeList, true);
                                    break;
                                case OnPlaySeriesOrSeasonAction.RandomUnwatched:
                                    episodeList = FilterEpisodeList(episodeList, true, true);
                                    m_SelectedEpisode = GetRandomEpisode(episodeList);
                                    break;
                                case OnPlaySeriesOrSeasonAction.Latest:
                                    episodeList = FilterEpisodeList(episodeList, true, false);
                                    m_SelectedEpisode = GetFirstOrLastEpisode(episodeList, false);
                                    break;
                            }
                        }
                    }

                    if (m_SelectedEpisode == null) {
                        switch (onPlayAction) {
                            case OnPlaySeriesOrSeasonAction.Random:
                                ShowNotifyDialog(Translation.PlayError, string.Format(Translation.UnableToFindAny, Translation.RandomEpisode));
                                break;
                            case OnPlaySeriesOrSeasonAction.FirstUnwatched:
                                ShowNotifyDialog(Translation.PlayError, string.Format(Translation.UnableToFindAny, Translation.FirstUnwatchedEpisode));
                                break;
                            case OnPlaySeriesOrSeasonAction.RandomUnwatched:
                                ShowNotifyDialog(Translation.PlayError, string.Format(Translation.UnableToFindAny, Translation.RandomUnwatchedEpisode));
                                break;
                            case OnPlaySeriesOrSeasonAction.Latest:
                                ShowNotifyDialog(Translation.PlayError, string.Format(Translation.UnableToFindAny, Translation.LatestEpisode));
                                break;
                        }
                        goto default;
                    }
                    MPTVSeriesLog.Write("Selected Episode OnAction Play: ", m_SelectedEpisode[DBEpisode.cCompositeID].ToString(), MPTVSeriesLog.LogLevel.Debug);
                    CommonPlayEpisodeAction(false);

                    break;

				default:
					base.OnAction(action);
					break;
			}
		}

		public override bool OnMessage(GUIMessage message) {
			switch (message.Message) {
				case GUIMessage.MessageType.GUI_MSG_ITEM_FOCUS_CHANGED:
					int iControl = message.SenderControlId;
					if (iControl == (int)m_Facade.GetID) {
						switch (this.listLevel) {
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
					return true;

				case GUIMessage.MessageType.GUI_MSG_PLAYBACK_ENDED:
				case GUIMessage.MessageType.GUI_MSG_PLAYBACK_STOPPED:
					//-- Need to reload the GUI to display changes 
					//-- if episode is classified as watched
					LoadFacade();					
					return true;

				default:
					return base.OnMessage(message);
			}
		}

        private void GetSubtitleDownloaderNames_Completed(object sender, RunWorkerCompletedEventArgs e)
        {            
            if (availableSubtitleDownloaderNames != null)
            {
                MPTVSeriesLog.Write(string.Format("Completed getting list of Subtitle Downloader Sites, {0} sites available", availableSubtitleDownloaderNames.Count));
                MPTVSeriesLog.Write("Subtitle sites available:", MPTVSeriesLog.LogLevel.Debug);
                foreach (var name in availableSubtitleDownloaderNames)
                {
                    MPTVSeriesLog.Write(name, MPTVSeriesLog.LogLevel.Debug);   
                }            
            }
        }

        private void GetSubtitleDownloaderNames(object sender, DoWorkEventArgs e)
        {            
            bool isSubtitleDownloaderEnabled = DBOption.GetOptions(DBOption.cSubtitleDownloaderEnabled);
            if (isSubtitleDownloaderEnabled)
            {
                MPTVSeriesLog.Write("Retrieving List of Subtitle Downloader Sites");
                availableSubtitleDownloaderNames = SubtitleDownloaderFactory.GetSubtitleDownloaderNames();
            }
        }

        protected List<CItem> GetEnabledSubtitleDownloaderProviders()
        {
            List<CItem> providers = new List<CItem>();
            string enabledDownloaders = DBOption.GetOptions(DBOption.cSubtitleDownloadersEnabled);
            
            if (availableSubtitleDownloaderNames != null)
            {
                foreach (var name in availableSubtitleDownloaderNames)
                {
                    if (enabledDownloaders.Contains(name))
                    {
                        providers.Add(new CItem(name, name, name));
                    }
                }
            }
            return providers;
        }

        protected bool SubtitleDownloaderEnabledAndHasSites()
        {
            bool isSubtitleDownloaderEnabled = DBOption.GetOptions(DBOption.cSubtitleDownloaderEnabled);
            if (isSubtitleDownloaderEnabled)
            {
                List<CItem> providers = GetEnabledSubtitleDownloaderProviders();
                if (providers.Count > 0)
                    return true;                
            }
            return false;
        }

        protected void ShowSubtitleMenu(DBEpisode episode)
        {
            ShowSubtitleMenu(episode, false);
        }

        protected void ShowSubtitleMenu(DBEpisode episode, bool fromPlay)
        {
            if (Helper.IsSubCentralAvailableAndEnabled && DBOption.GetOptions(DBOption.cSubCentralEnabled) && !fromPlay)
            {
                /*
                DBSeries series = Helper.getCorrespondingSeries(episode[DBEpisode.cSeriesID]);
                string episodeTitle = episode[DBEpisode.cEpisodeName];
                string seasonIdx = episode[DBEpisode.cSeasonIndex];
                string episodeIdx = episode[DBEpisode.cEpisodeIndex];
                string seriesTitle = series[DBOnlineSeries.cOriginalName];
                string thumb = ImageAllocator.GetSeriesPosterAsFilename(series);
                string fanart = Fanart.getFanart(episode[DBEpisode.cSeriesID]).FanartFilename;
                string episodeFileName = episode[DBEpisode.cFilename];

                List<string> episodeDetails = new List<string> { "TVSeries", "", seriesTitle, "", seasonIdx, episodeIdx, thumb, fanart, episodeFileName };

                GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_USER, 84623, 9811, 0, 0, 0, episodeDetails);
                GUIGraphicsContext.SendMessage(msg);
                //GUIWindowManager.Process();
                */
                GUIWindowManager.ActivateWindow(84623);
                return;
            }

            if (!SubtitleDownloaderEnabledAndHasSites()) return;

            List<CItem> Choices = GetEnabledSubtitleDownloaderProviders();

            if (Choices.Count == 0)
            {
                if (fromPlay)
                    m_VideoHandler.ResumeOrPlay(episode);
                return;
            }

            if (fromPlay && Choices.Count > 0)
            {
                Choices.Insert(0, new CItem(Translation.PlayNow, Translation.NoSubtitleDownload, (object)"playNow"));
            }

            CItem selected = null;

            ChooseFromSelectionDescriptor descriptor = new ChooseFromSelectionDescriptor();
            descriptor.m_sTitle = Translation.GetSubtitlesFrom;
            descriptor.m_sItemToMatchLabel = "";
            descriptor.m_sItemToMatch = "";
            descriptor.m_sListLabel = Translation.EnabledSubtitleSites;
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

            if (selected != null)
            {
                if (selected.m_Tag == (object)"playNow")
                {
                    m_VideoHandler.ResumeOrPlay(episode);
                }
                else 
                {
                    ISubtitleDownloader downloader = SubtitleDownloaderFactory.GetSubtitleDownloader(selected.m_Tag.ToString());
                    SubtitleRetriever retriever = new SubtitleRetriever(this, downloader);

                    if (!subtitleDownloaderWorking)
                    {
                        setProcessAnimationStatus(true);
                        retriever.SubtitleRetrievalCompleted += downloader_SubtitleRetrievalCompleted;
                        subtitleDownloaderWorking = true;
                        retriever.GetSubs(episode);
                    }
                }
            }
        }

        protected void ShowDownloadMenu(DBEpisode selectedEpisode)
        {
            IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null) return;

            int nSelection = -1;
            bool bExitMenu = false;
            do
            {
                dlg.Reset();
                GUIListItem pItem = null;

                bool newsEnable = System.IO.File.Exists(DBOption.GetOptions(DBOption.cNewsLeecherPath));
                bool torrentsEnable = System.IO.File.Exists(DBOption.GetOptions(DBOption.cUTorrentPath));

                #region Selected Menu Item Actions (Sub-Menus)

                if (newsEnable && !torrentsEnable)
                {
                    nSelection = (int)eContextItems.downloadviaNewz;
                    bExitMenu = true;
                }
                else if (!newsEnable && torrentsEnable)
                {
                    nSelection = (int)eContextItems.downloadviaTorrent;
                    bExitMenu = true;
                }
                else if (newsEnable && torrentsEnable)
                {
                    dlg.SetHeading(Translation.Download);

                    pItem = new GUIListItem(Translation.Load_via_NewsLeecher);
                    dlg.Add(pItem);
                    pItem.ItemId = (int)eContextItems.downloadviaNewz;

                    pItem = new GUIListItem(Translation.Load_via_Torrent);
                    dlg.Add(pItem);
                    pItem.ItemId = (int)eContextItems.downloadviaTorrent;

                    dlg.DoModal(GUIWindowManager.ActiveWindow);
                    nSelection = dlg.SelectedId;
                    if (nSelection != -1)
                        bExitMenu = true;
                }
                else
                    bExitMenu = true;

                #endregion
            }
            while (!bExitMenu);

            if (nSelection == -1) return;

            #region Selected Menu Item Actions
            switch (nSelection)
            {
                #region Downloaders
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
            }
            #endregion
        }

		protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType) {
			if (control == this.viewMenuButton) {
				showViewSwitchDialog();
				viewMenuButton.Focus = false;				
				return;
			}

			if (control == this.LayoutMenuButton) {
				ShowLayoutMenu();
				LayoutMenuButton.Focus = false;
				return;
			}

			if (control == this.OptionsMenuButton) {
				ShowOptionsMenu();
				OptionsMenuButton.Focus = false;
				m_Facade.Focus = true;
				return;
			}

			if (control == this.ImportButton) {
				lock (m_parserUpdaterQueue) {
					m_parserUpdaterQueue.Add(new CParsingParameters(true, true));
				}
				ImportButton.Focus = false;
				m_Facade.Focus = true;
				return;
			}

			if (control == this.LoadPlaylistButton) {
				OnShowSavedPlaylists(DBOption.GetOptions(DBOption.cPlaylistPath));
				LoadPlaylistButton.Focus = false;
				return;
			}

			if (actionType != Action.ActionType.ACTION_SELECT_ITEM) return; // some other events raised onClicked too for some reason?
			if (control == this.m_Facade) {
				if (this.m_Facade.SelectedListItem == null || this.m_Facade.SelectedListItem.TVTag == null)
					return;

				m_back_up_select_this = null;
				switch (this.listLevel) {
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
						m_SelectedEpisode = this.m_Facade.SelectedListItem.TVTag as DBEpisode;
						if (m_SelectedEpisode == null) return;
						MPTVSeriesLog.Write("Selected: ", m_SelectedEpisode[DBEpisode.cCompositeID].ToString(), MPTVSeriesLog.LogLevel.Debug);

                        CommonPlayEpisodeAction(true);
						break;
				}
			}
			base.OnClicked(controlId, control, actionType);
		}
        #endregion

		void m_VideoHandler_RateRequestOccured(DBEpisode episode)
        {
            ask2Rate = episode;
        }

        void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e) {
            if (e.Mode == Microsoft.Win32.PowerModes.Resume) {
                MPTVSeriesLog.Write("MP-TVSeries is resuming from standby");
                IsResumeFromStandby = true;

                // re-initialise the importer timer                
                int importDelay = DBOption.GetOptions(DBOption.cImportDelay);
                MPTVSeriesLog.Write("Starting initial import scan in: {0} secs", importDelay);
                m_scanTimer = new System.Threading.Timer(m_timerDelegate, null, importDelay*1000, 1000);

                // Force Lock on views after resume from standby
                logicalView.IsLocked = true;

                if (DBOption.GetOptions(DBOption.cImport_FolderWatch))
                {
                    DeviceManager.StartMonitor();
                    setUpFolderWatches();
                }

                // Prompt for PinCode if last view before standby had Parental Controls enabled
                // If the window is not active, we handle on page load
                if (GUIWindowManager.ActiveWindow == GetID) {
                    if (m_CurrLView != null) {
                        bool viewSwitched = false;
                        if (m_CurrLView.ParentalControl) {
                            viewSwitched = switchView((string)DBOption.GetOptions("lastView"));
                            // Exit if no view changed, otherwise reload the facade
                            if (!viewSwitched)
                                GUIWindowManager.ShowPreviousWindow();
                            else
                                LoadFacade();
                        }
                    }
                }

                // Treat Resuming from Standby as a startup action, scan on startup if enabled
                if (DBOption.GetOptions(DBOption.cImport_ScanOnStartup)) {
                    m_parserUpdaterQueue.Add(new CParsingParameters(true, false));
                }
            }
            else if (e.Mode == Microsoft.Win32.PowerModes.Suspend) {
                MPTVSeriesLog.Write("MP-TVSeries is entering standby");

                if (DBOption.GetOptions(DBOption.cImport_FolderWatch))
                {
                    DeviceManager.StopMonitor();
                    stopFolderWatches();
                }

                // stop the import timer
                m_scanTimer.Dispose();

                // Only disconnect from the database if file exists on the network.
                if (DBTVSeries.IsDatabaseOnNetworkPath) {
                    DBTVSeries.Close();
                }
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

        private void watcherUpdater_WatcherProgress(int nProgress, List<WatcherItem> modifiedFilesList)
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
                    m_parserUpdaterQueue.Add(new CParsingParameters(ParsingAction.List_Add, filesAdded, false, false));
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

		#region Facade Loading
		// this is expensive to do if changing mode......450 ms ???
        private void setFacadeMode(GUIFacadeControl.ViewMode mode)
        {
            if (this.m_Facade == null)
                return;

            if (mode == GUIFacadeControl.ViewMode.List)
            {
                PerfWatcher.GetNamedWatch("FacadeMode - switch to List").Start();
                this.m_Facade.View = mode;                
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
            }            
        }
        
        System.ComponentModel.BackgroundWorker bg = null;        

        private void LoadFacade()
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
        private void prepareLoadFacade()
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

                switch (this.listLevel)
                {
                    case Listlevel.Series:						
						// Check if series fanart is enabled                       
                        if (!DBOption.GetOptions(DBOption.cShowSeriesFanart)) {
                            DisableFanart();
                        }                        
                        break;     
            
                    case Listlevel.Season:
                        clearFieldsForskin("Season");
                        clearFieldsForskin("Episode");
                        break;

                    case Listlevel.Group:
                        clearGUIProperty(guiProperty.Description);                        
                        seriesposter.Filename = string.Empty;
                        seriesbanner.Filename = string.Empty;
                        seasonbanner.Filename = string.Empty;
                        clearFieldsForskin("Series");                        
                        break;

                }      
                setNewListLevelOfCurrView(m_CurrViewStep);

            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Error preparing Facade... " + ex.Message);
            }
        }

        SkipSeasonCodes SkipSeasonCode = SkipSeasonCodes.none;
        List<GUIListItem> itemsForDelayedImgLoading = null;
        private void bg_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
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
                            /*if (itemsForDelayedImgLoading != null) {
                                string image = arg.Argument as string;
                                foreach (var item in itemsForDelayedImgLoading) {
                                    DBSeries seriesOfListItem = item.TVTag as DBSeries;
                                    if (seriesOfListItem != null && seriesOfListItem[DBSeries.cID] == arg.IndexArgument) {
                                        //MPTVSeriesLog.Write(string.Format("series:{0}, image:{1}", Helper.getCorrespondingSeries(seriesOfListItem[DBSeries.cID]).ToString(), image));
                                        item.IconImageBig = image;
                                        break;
                                    }
                                }
                            }*/
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

        private void bgFacadeDone(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
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

                    #region Clear GUI Properties
                    clearGUIProperty(guiProperty.Title);
                    clearGUIProperty(guiProperty.Subtitle);
                    clearGUIProperty(guiProperty.Description);
                    
                    clearGUIProperty(guiProperty.SeriesBanner);
                    clearGUIProperty(guiProperty.SeriesPoster);
                    clearGUIProperty(guiProperty.SeasonPoster);
                    clearGUIProperty(guiProperty.EpisodeImage);
                    clearGUIProperty(guiProperty.Logos);                    

                    clearFieldsForskin("Series");
                    clearFieldsForskin("Season");
                    clearFieldsForskin("Episode");
                    #endregion

                }
                else
                {
                    // probably something was removed
                    MPTVSeriesLog.Write("Nothing to display, going out");
                    OnAction(new Action(Action.ActionType.ACTION_PREVIOUS_MENU, 0, 0));
                }
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

        private void bgLoadFacade(object sender, System.ComponentModel.DoWorkEventArgs e)
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

        private void bgLoadFacade()
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
							
							// Get list of series for current view
                            seriesList = m_CurrLView.getSeriesItems(m_CurrViewStep, m_stepSelection);
                            
                            // Sort Series List if Title has been user edited
                            string titleField = DBOption.GetOptions(DBOption.cSeries_UseSortName) ? DBOnlineSeries.cSortName : DBOnlineSeries.cPrettyName;
                            seriesList.Sort(new Comparison<DBSeries>((x, y) => 
                            {
                                string seriesX = string.Empty;
                                string seriesY = string.Empty;

                                if (string.IsNullOrEmpty(seriesX = x[titleField + DBTable.cUserEditPostFix]))
                                    seriesX = x[titleField];

                                if (string.IsNullOrEmpty(seriesY = y[titleField + DBTable.cUserEditPostFix]))
                                    seriesY = y[titleField];

                                return string.Compare(seriesX, seriesY);
                            }));

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

                                        LoadWatchedFlag(item, bWatched, bAvailable, false);
                                    }
                                    item.TVTag = series;
                                    
                                    #region List Colors
                                    item.IsRemote = false;
                                    item.IsPlayed = false;

                                    // Set IsRemote property to true, if there are no episodes local on disk for season
                                    if (!series[DBOnlineSeries.cHasLocalFiles]) {
                                        item.IsRemote = true;
                                    }
                                    // Set IsPlayed property to true, if all episodes in season have been watched
                                    else if (int.Parse(series[DBOnlineSeries.cEpisodesUnWatched]) == 0) {
                                        item.IsPlayed = true;
                                    }
                                    // Set Selected property to true, if all episodes are hidden
                                    if (series[DBSeries.cHidden] && DBOption.GetOptions(DBOption.cShowHiddenItems))
                                    {
                                        // remote/played property trumps selected
                                        item.IsRemote = false;
                                        item.IsPlayed = false;
                                        item.Selected = true;
                                    }
                                    #endregion

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

                            bool canBeSkipped = (seasons.Count == 1) && DBOption.GetOptions(DBOption.cSkipSeasonViewOnSingleSeason);
                            if (!canBeSkipped)
								MPTVSeriesLog.Write(string.Format("Displaying {0} seasons from {1}", seasons.Count.ToString(), m_SelectedSeries), MPTVSeriesLog.LogLevel.Debug);

                            bool graphicalFacade = DBOption.GetOptions(DBOption.cView_Season_ListFormat);
                            ReportFacadeLoadingProgress(BackGroundLoadingArgumentType.SetFacadeMode, 0, (graphicalFacade ? GUIFacadeControl.ViewMode.AlbumView : GUIFacadeControl.ViewMode.List));

                            foreach (DBSeason season in seasons)
                            {
                                try
                                {                                    
                                    item = null;
                                    if (!canBeSkipped)
                                    {
                                        if (graphicalFacade) 
                                        {
                                            item = new GUIListItem();
                                            string filename = ImageAllocator.GetSeasonBanner(season, false);

                                            if (filename.Length == 0) {
                                                // Load Series Poster instead
                                                if (DBOption.GetOptions(DBOption.cSubstituteMissingArtwork) && m_SelectedSeries != null) {
                                                    filename = ImageAllocator.GetSeriesPosterAsFilename(m_SelectedSeries);
                                                }
                                                else {
                                                    // Add Season Label to Poster Thumb
                                                    filename = ImageAllocator.GetSeasonBanner(season, true);
                                                }
                                            }
                                            item.IconImage = item.IconImageBig = filename;
                                        }
                                        else
                                        {
                                            item = new GUIListItem(FieldGetter.resolveDynString(m_sFormatSeasonCol2, season));
                                            item.Label2 = FieldGetter.resolveDynString(m_sFormatSeasonCol3, season);
                                            item.Label3 = FieldGetter.resolveDynString(m_sFormatSeasonCol1, season);

                                            bool bWatched = (int.Parse(season[DBOnlineSeries.cEpisodesUnWatched]) == 0);
                                            bool bAvailable = season[DBSeason.cHasLocalFiles];

                                            if (!LoadWatchedFlag(item, bWatched, bAvailable, false))
                                            {
                                                if (DBOption.GetOptions(DBOption.cAppendFirstLogoToList))
                                                {
                                                    // if skins want to display the logo in the textual list, users need to set the option (expensive)
                                                    item.IconImage = ImageAllocator.GetSeasonBanner(season, false);
                                                }
                                            }
                                        }
                                        #region List Colors
                                        item.IsRemote = false;
                                        item.IsPlayed = false;
                                        
                                        // Set IsRemote property to true, if there are no episodes local on disk for season
                                        if (!season[DBSeason.cHasLocalFiles]) {
                                            item.IsRemote = true;
                                        }
                                        // Set IsPlayed property to true, if all episodes in season have been watched
                                        else if (int.Parse(season[DBSeason.cEpisodesUnWatched]) == 0) {
                                            item.IsPlayed = true;
                                        }
                                        // Set Selected property to true, if all episodes are hidden
                                        if (season[DBSeason.cHidden] && DBOption.GetOptions(DBOption.cShowHiddenItems))
                                        {
                                            item.IsRemote = false;
                                            item.IsPlayed = false;
                                            item.Selected = true;
                                        }
                                        #endregion
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
                                        selectedIndex = 0;

                                        // If we are skipping Season view, going from Series -> Episodes then
                                        // we also need to load season artwork
                                        if (skipSeasonIfOne_DirectionDown && seasons.Count == 1) {
                                            // Delayed Image Loading of Season Banners
                                            string filename = ImageAllocator.GetSeasonBannerAsFilename(season);
                                            if (filename.Length == 0) {
                                                // Load Series Poster instead
                                                if (DBOption.GetOptions(DBOption.cSubstituteMissingArtwork) && m_SelectedSeries != null) {
                                                    filename = ImageAllocator.GetSeriesPosterAsFilename(m_SelectedSeries);
                                                }
                                            }
                                            seasonbanner.Filename = filename;
                                        }
                                        else if (!skipSeasonIfOne_DirectionDown && seasons.Count == 1) {
                                            seasonbanner.Filename = string.Empty;
                                            clearGUIProperty(guiProperty.Logos);
                                            clearGUIProperty(guiProperty.EpisodeImage);
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
                                    MPTVSeriesLog.Write("The 'LoadFacade' function has generated an error displaying season list item: " + ex.Message);
                                }
                                count++;
                            }

                            // if there is only one season to display, skip directly to the episodes list
                            if (skipSeasonIfOne_DirectionDown && seasons.Count == 1 && canBeSkipped)
                            {
                                MPTVSeriesLog.Write("Skipping season display (Series->Episode)", MPTVSeriesLog.LogLevel.Debug);
                                ReportFacadeLoadingProgress(BackGroundLoadingArgumentType.SkipSeasonDown, 0, null);                                
                            }
                            else if (seasons.Count == 1 && canBeSkipped)
                            {
                                // we're back from the ep list, go up one hierarchy more (depending on view, most likely series)
                                MPTVSeriesLog.Write("Skipping season display (Episode->Series)", MPTVSeriesLog.LogLevel.Debug);
                                ReportFacadeLoadingProgress(BackGroundLoadingArgumentType.SkipSeasonUp, 0, null);                                
                            }
                        }
                        break;
                    #endregion
                    #region Episode
                    case Listlevel.Episode:
                        {
                            bool bFindNext = false;
                            ReportFacadeLoadingProgress(BackGroundLoadingArgumentType.SetFacadeMode, 0, GUIFacadeControl.ViewMode.List);
                            
							// Get a list of Episodes to display for current view							
							List<DBEpisode> episodesToDisplay = m_CurrLView.getEpisodeItems(m_CurrViewStep, m_stepSelection);							

                            // Update Filtered Episode Count Property, this acurately displays the number of items on the facade
                            // #TVSeries.Series.EpisodeCount is not desirable in some views e.g. Recently Added or views that filter by episode fields
                            setGUIProperty(guiProperty.FilteredEpisodeCount, episodesToDisplay.Count.ToString());

                            int watchedCount = 0;
                            int unwatchedCount = episodesToDisplay.Count;

                            MPTVSeriesLog.Write(string.Format("Displaying {0} episodes from {1}", episodesToDisplay.Count.ToString(), m_SelectedSeries), MPTVSeriesLog.LogLevel.Debug);
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

                                    #region List Colors
                                    item.IsRemote = false;
                                    item.IsPlayed = false;

                                    // Set IsRemote property to true, if the episode is not local on disk                                    
                                    if (episode[DBEpisode.cFilename].ToString().Length == 0 || episode[DBEpisode.cIsAvailable] != 1)
                                    {
                                        item.IsRemote = true;
                                    }
                                    // Set IsPlayed property to true, if the episode has been watched
                                    else if (episode[DBOnlineEpisode.cWatched]) {
                                        item.IsPlayed = true;
                                    }
                                    // Set Selected property to true, if all episodes are hidden
                                    if (episode[DBOnlineEpisode.cHidden] && DBOption.GetOptions(DBOption.cShowHiddenItems))
                                    {
                                        item.IsRemote = false;
                                        item.IsPlayed = false;
                                        item.Selected = true;
                                    }

                                    #endregion

                                    if (item.IsPlayed) {
                                        watchedCount++;
                                        unwatchedCount--;
                                    }

                                    item.TVTag = episode;

                                    if (m_SelectedEpisode != null)
                                    {
                                        if (episode[DBEpisode.cCompositeID] == m_SelectedEpisode[DBEpisode.cCompositeID])
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
                                    bool bDownloading = episode[DBOnlineEpisode.cDownloadPending];

                                    if (!LoadWatchedFlag(item, bWatched, bAvailable, bDownloading))
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
                            // Push Watched/Unwatched Count for Current Episode View
                            setGUIProperty(guiProperty.WatchedCount, watchedCount.ToString());
                            setGUIProperty(guiProperty.UnWatchedCount, unwatchedCount.ToString());
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
                                KeyValuePair<int, DBSeries> keySeriesValue = new KeyValuePair<int, DBSeries>(currIndex, series);
                                ThreadPool.QueueUserWorkItem(delegate(object state) {
                                    string img = string.Empty;
                                    KeyValuePair<int, DBSeries> stateSeries = (KeyValuePair<int, DBSeries>)state;

                                    // Load Series Banners if WideBanners otherwise load Posters for Filmstrip
                                    if (DBOption.GetOptions(DBOption.cView_Series_ListFormat) == "Filmstrip")
                                        img = ImageAllocator.GetSeriesPoster(stateSeries.Value);
                                    else
                                        img = ImageAllocator.GetSeriesBanner(stateSeries.Value);                                    
                                    //ReportFacadeLoadingProgress(BackGroundLoadingArgumentType.DelayedImgLoading, stateSeries.Value[DBSeries.cID], img);
                                    ReportFacadeLoadingProgress(BackGroundLoadingArgumentType.DelayedImgLoading, stateSeries.Key, img);
                                    Interlocked.Increment(ref done);
                                }, keySeriesValue);
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

		private void ReportFacadeLoadingProgress(BackGroundLoadingArgumentType type, int indexArgument, object state) {
			if (!bg.CancellationPending) {
				BackgroundFacadeLoadingArgument Arg = new BackgroundFacadeLoadingArgument();
				Arg.Type = type;
				Arg.IndexArgument = indexArgument;
				Arg.Argument = state;

				bg.ReportProgress(0, Arg);
			}
		}

		#endregion

		private bool LoadWatchedFlag(GUIListItem item, bool bWatched, bool bAvailable, bool bDownloading)
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

            // being downloaded (not yet local, but coming!)
            string sWatchedDLFilename = GUIGraphicsContext.Skin + @"\Media\tvseries_WatchedDL.png";
            string sUnWatchedDLFilename = GUIGraphicsContext.Skin + @"\Media\tvseries_UnWatchedDL.png";

            // return if images dont exists
            if (!(System.IO.File.Exists(sWatchedFilename) &&
                  System.IO.File.Exists(sUnWatchedFilename) &&
                  System.IO.File.Exists(sWatchedNAFilename) &&
                  System.IO.File.Exists(sUnWatchedNAFilename)))
                return false;

            if (bWatched)
            {
                if (bDownloading)
                {
                    item.IconImage = sWatchedDLFilename;
                }
                else 
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
                if (bDownloading)
                {
                    item.IconImage = sUnWatchedDLFilename;
                }
                else
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

        string getGUIProperty(guiProperty name)
        {
            return getGUIProperty(name.ToString());
        }

        public static string getGUIProperty(string name)
        {
            return GUIPropertyManager.GetProperty("#TVSeries." + name);
        }

        void setGUIProperty(guiProperty name, string value)
        {
            setGUIProperty(name.ToString(), value);
        }

        public static void setGUIProperty(string name, string value)
        {
            string property = "#TVSeries." + name;
            //MPTVSeriesLog.Write("[{0}]: {1}", property, value);
            GUIPropertyManager.SetProperty(property, value);
        }

        void clearGUIProperty(guiProperty name)
        {
            clearGUIProperty(name.ToString());
        }

        public static void clearGUIProperty(string name)
        {
            setGUIProperty(name, " "); // String.Empty doesn't work on non-initialized fields, as a result they would display as ugly #TVSeries.bla.bla
        }

		private void UpdateEpisodes(DBSeries series, DBSeason season, DBEpisode episode) {			
			List<DBValue> epIDsUpdates = new List<DBValue>();
			List<DBValue> seriesIDsUpdates = new List<DBValue>();

			SQLCondition conditions = null;
			string searchPattern = string.Empty;			 			 

			// Get selected Series and/or list of Episode(s) to update
			switch (this.listLevel) {
				case Listlevel.Series:					
					seriesIDsUpdates.Add(series[DBSeries.cID]);
					conditions = new SQLCondition(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
					epIDsUpdates.AddRange(DBEpisode.GetSingleField(DBOnlineEpisode.cID, conditions, new DBOnlineEpisode()));
					searchPattern = "*.jpg";
					break;

				case Listlevel.Season:
					conditions = new SQLCondition(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, season[DBSeason.cSeriesID], SQLConditionType.Equal);
					conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeasonIndex, season[DBSeason.cIndex], SQLConditionType.Equal);
					epIDsUpdates.AddRange(DBEpisode.GetSingleField(DBOnlineEpisode.cID, conditions, new DBOnlineEpisode()));
					searchPattern = season[DBSeason.cIndex] + "x*.jpg";
					break;

				case Listlevel.Episode:
					epIDsUpdates.Add(episode[DBOnlineEpisode.cID]);
					conditions = new SQLCondition(new DBOnlineEpisode(), DBOnlineEpisode.cID, episode[DBOnlineEpisode.cID], SQLConditionType.Equal);
					searchPattern = episode[DBOnlineEpisode.cSeasonIndex] + "x" + episode[DBOnlineEpisode.cEpisodeIndex] + ".jpg";
					break;
			}

			// Delete Physical Thumbnails
			// Dont prompt if just doing a single episode update
			bool deleteThumbs = true;
			if (this.listLevel != Listlevel.Episode) {
				GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
				if (dlgYesNo != null) {
					dlgYesNo.Reset();
					dlgYesNo.SetHeading(Translation.DeleteThumbnailsHeading);
					dlgYesNo.SetLine(1, Translation.DeleteThumbnailsLine1);
					dlgYesNo.SetLine(2, Translation.DeleteThumbnailsLine2);
					dlgYesNo.SetDefaultToYes(false);
					dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
					if (!dlgYesNo.IsConfirmed) deleteThumbs = false;
				}
			}

			if (deleteThumbs) {
				string thumbnailPath = Helper.PathCombine(Settings.GetPath(Settings.Path.banners), Helper.cleanLocalPath(series.ToString()) + @"\Episodes");
				
				// Search and delete matching files that actually exist
				string[] fileList = Directory.GetFiles(thumbnailPath, searchPattern);

				foreach (string file in fileList) {
					MPTVSeriesLog.Write("Deleting Episode Thumbnail: " + file);
					FileInfo fileInfo = new FileInfo(file);
					try {
						fileInfo.Delete();
					}
					catch (Exception ex) {
						MPTVSeriesLog.Write("Failed to Delete Episode Thumbnail: " + file + ": " + ex.Message);
					}
				}

				// Remove local thumbnail reference from db so that it thumbnails will be downloaded
				DBEpisode.GlobalSet(new DBOnlineEpisode(), DBOnlineEpisode.cEpisodeThumbnailFilename, (DBValue)"", conditions);
			}

			// Execute Online Parsing Actions
			if (epIDsUpdates.Count > 0) {

                lock (m_parserUpdaterQueue) {
                    List<ParsingAction> parsingActions = new List<ParsingAction>();
                    // Conditional parsing actions                    
                    if (this.listLevel == Listlevel.Series) parsingActions.Add(ParsingAction.UpdateSeries);
                    parsingActions.Add(ParsingAction.UpdateEpisodes);                    
                    if (deleteThumbs) parsingActions.Add(ParsingAction.UpdateEpisodeThumbNails);
                    parsingActions.Add(ParsingAction.UpdateEpisodeCounts);

                    m_parserUpdaterQueue.Add(new CParsingParameters(parsingActions, seriesIDsUpdates, epIDsUpdates));
                }

			}
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
					else
						value = ratingDlg.Rating.ToString();
				} 
				else return;
			} 
			else {
				IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
				dlg.Reset();
				dlg.SetHeading(dlgHeading + ": " + item.ToString());

                // List Rating 1 Star to 10 Stars
                // Re-use existing menu labels to represent star value
				pItem = new GUIListItem(Translation.RatingStar);
				dlg.Add(pItem);
				pItem.ItemId = 1;

				for (int i = 2; i < 11; i++) {
					pItem = new GUIListItem(Translation.RatingStars);
					dlg.Add(pItem);
					pItem.ItemId = i;
				}

                // Reset Rating (Rate = 0)
                pItem = new GUIListItem(Translation.ResetRating);
                dlg.Add(pItem);
                pItem.ItemId = 11;
                
				if (auto) {
					pItem = new GUIListItem(Translation.DontAskToRate);
					dlg.Add(pItem);
					pItem.ItemId = 12;
				}

				dlg.DoModal(GUIWindowManager.ActiveWindow);

				if (dlg.SelectedId == -1 || dlg.SelectedId > 12) return; // cancelled
                if (dlg.SelectedLabelText == Translation.DontAskToRate && auto) {
					DBOption.SetOptions(DBOption.cAskToRate, false);
					return;
				}
                // Get Rating Value
				value = dlg.SelectedId.ToString();
                // Reset rating
                if (dlg.SelectedLabelText == Translation.ResetRating)
                    value = "0";
			}
			
			string type = (level == Listlevel.Episode ? DBOnlineEpisode.cMyRating : DBOnlineSeries.cMyRating);
			string id = item[level == Listlevel.Episode ? DBOnlineEpisode.cID : DBOnlineSeries.cID];
			
			// Submit rating online database if current rating is different
			if (!String.IsNullOrEmpty(value) && value != item[type])
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

		#region Layout Menu
		private void ShowLayoutMenu() {
			string currentLayout = GetCurrentLayout();

			IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
			dlg.Reset();
			dlg.SetHeading(Translation.ChangeLayout);

			int counter = 0;
			string item = string.Empty;

			// Get Available Layouts for Skin
			foreach (string layout in SkinSettings.SupportedLayouts) {
				switch (this.listLevel) {
					case Listlevel.Group:
						if (layout.StartsWith("Group"))
							item = layout.Substring(5);						
						break;

					case Listlevel.Series:
						if (layout.StartsWith("Series"))
							item = layout.Substring(6);						
						break;

					case Listlevel.Season:
						if (layout.StartsWith("Season"))
							item = layout.Substring(6);						
						break;

					case Listlevel.Episode:
						if (layout.StartsWith("Episode"))
							item = layout.Substring(7);						
						break;
				}

				if (item.Length > 0) {
					string menuItem = GetTranslatedLayout(item);
					if (menuItem.Length > 0) {
						GUIListItem pItem = new GUIListItem(menuItem);
						dlg.Add(pItem);
						pItem.ItemId = counter++;
					}
					item = string.Empty;
				}
			}

			if (counter > 0)
				dlg.DoModal(GUIWindowManager.ActiveWindow);

			if (dlg.SelectedId >= 0) {
				string selectedLayout = GetLogicalLayout(dlg.SelectedLabelText);
				if (!selectedLayout.Equals(currentLayout)) {
					switchLayout(selectedLayout);
					LoadFacade();
				}
			}
		}

		private string GetCurrentLayout() {
			switch (this.listLevel) {
				case Listlevel.Group:
					if (DBOption.GetOptions(DBOption.cGraphicalGroupView))
						return "SmallIcons";
					else
						return "List";

				case Listlevel.Series:
					return DBOption.GetOptions(DBOption.cView_Series_ListFormat);

				case Listlevel.Season:
					if (DBOption.GetOptions(DBOption.cView_Season_ListFormat))
						return "Filmstrip";
					else
						return "List";

				default:
					return "List";
			}
		}

		private string GetLogicalLayout(string layout) {
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

		private string GetTranslatedLayout(string layout) {
			string translatedLayout = string.Empty;

			switch (layout) {
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

		private void switchLayout(string layout) {
			switch (this.listLevel) {
				case Listlevel.Group:
					if (layout == "List")
						DBOption.SetOptions(DBOption.cGraphicalGroupView, "0");
					else
						DBOption.SetOptions(DBOption.cGraphicalGroupView, "1");
					return;

				case Listlevel.Series:
					DBOption.SetOptions(DBOption.cView_Series_ListFormat, layout);
					return;
				
				case Listlevel.Season:
					if (layout == "List")
						DBOption.SetOptions(DBOption.cView_Season_ListFormat, "0");
					else
						DBOption.SetOptions(DBOption.cView_Season_ListFormat, "1");
					return;					

				default:
					return;
			}
		}
		#endregion

		#region Views Menu
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
                bool viewSwitched = false;
                viewSwitched = switchView(m_allViews[dlg.SelectedId]);
                if (viewSwitched) {
                    LoadFacade();
                    return true;
                }
                else 
                    return false;
            }
            return false;
		}
		#endregion

		#region Options Menu
		internal void ShowOptionsMenu()
        {
            IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlg == null) return;

            dlg.Reset();
            dlg.SetHeading(Translation.Options);

            GUIListItem pItem = new GUIListItem(Translation.Only_show_episodes_with_a_local_file + " (" + (DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles) ? Translation.on : Translation.off) + ")");
            dlg.Add(pItem);
            pItem.ItemId = (int)eContextItems.optionsOnlyShowLocal;

            pItem = new GUIListItem(Translation.ShowHiddenItems + " (" + (DBOption.GetOptions(DBOption.cShowHiddenItems) ? Translation.on : Translation.off) + ")");
            dlg.Add(pItem);
            pItem.ItemId = (int)eContextItems.optionsShowHiddenItems;

            pItem = new GUIListItem(Translation.Hide_summary_on_unwatched + " (" + (DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedSummary) ? Translation.on : Translation.off) + ")");
            dlg.Add(pItem);
            pItem.ItemId = (int)eContextItems.optionsPreventSpoilers;

            pItem = new GUIListItem(Translation.Hide_thumbnail_on_unwatched + " (" + (DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedThumbnail) ? Translation.on : Translation.off) + ")");
            dlg.Add(pItem);
            pItem.ItemId = (int)eContextItems.optionsPreventSpoilerThumbnail;

            if (!String.IsNullOrEmpty(DBOption.GetOptions(DBOption.cOnlineUserID)))
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
                List<DBSeries> AllSeries = null;
                SQLCondition condEmpty = null;

                switch (dlg.SelectedId)
                {
                    case (int)eContextItems.optionsOnlyShowLocal:
                        DBOption.SetOptions(DBOption.cView_Episode_OnlyShowLocalFiles, !DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles));
                        // Update Episode counts...this is going to be expensive                        
                        condEmpty = new SQLCondition();
                        AllSeries = DBSeries.Get(condEmpty);
                        foreach (DBSeries series in AllSeries)
                            DBSeries.UpdateEpisodeCounts(series);
                        LoadFacade();
                        break;

                    case (int)eContextItems.optionsShowHiddenItems:
                        DBOption.SetOptions(DBOption.cShowHiddenItems, !DBOption.GetOptions(DBOption.cShowHiddenItems));
                        condEmpty = new SQLCondition();
                        AllSeries = DBSeries.Get(condEmpty);
                        foreach (DBSeries series in AllSeries)
                            DBSeries.UpdateEpisodeCounts(series);
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
                    /*case (int)eContextItems.optionsUseOnlineFavourites:
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
                     */
                }
            }
		}
		#endregion

		#region View Tags Menu
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
                // List available tag views                       
                foreach (DBView view in views) {
                    string viewname = view[DBView.cTransToken];
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
                GetStringFromUserDescriptor Keyboard = new GetStringFromUserDescriptor();
				Keyboard.Text = string.Empty;
                Keyboard.ShiftEnabled = true;
				Keyboard.KeyboardStyle = (GetStringFromUserDescriptor.KeyboardStyles)(int)DBOption.GetOptions(DBOption.cKeyboardStyle);
                bool viewExists = true;

                while (viewExists) {
                    if (this.GetStringFromUser(Keyboard, out selectedItem) == ReturnCode.OK) {
                        // Create View if it doesnt exist
                        viewExists = false;
                        foreach (DBView view in views) {
                            if (selectedItem.Equals(view[DBView.cTransToken], StringComparison.CurrentCultureIgnoreCase)) {
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
                string config = DBView.GetTaggedViewConfigString(selectedItem);

                MPTVSeriesLog.Write(string.Format("Creating New Tagged View: {0}", selectedItem));
                DBView.AddView(index, selectedItem, selectedItem, config, true);
            }
            #endregion

            string newTags = string.Empty;

            // Get Selected Item if not a new Tag
            if (dlg.SelectedId != (int)eContextItems.viewAddToNewView)
                selectedItem = dlg.SelectedLabelText;
            
            // Add / Remove Tag from series
            if (add) {
                MPTVSeriesLog.Write(string.Format("Adding new Tag \"{0}\" to series: {1}", selectedItem, series.ToString()));
                newTags = Helper.GetSeriesViewTags(series, true, selectedItem);
            }
            else {
                MPTVSeriesLog.Write(string.Format("Removing Tag \"{0}\" from series: {1}", selectedItem, series.ToString()));
                newTags = Helper.GetSeriesViewTags(series, false, selectedItem);                          
            }
            
            // Update Main View List
            m_allViews = logicalView.getAll(false);

            // Commit changes to database
            series[DBOnlineSeries.cViewTags] = newTags;
            series.Commit();

            // Special case to handle online favourites
            // We need to add/remove from online database
            if (selectedItem == DBView.cTranslateTokenOnlineFavourite) {
                string account = DBOption.GetOptions(DBOption.cOnlineUserID);
                if (String.IsNullOrEmpty(account)) {
                    GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                    dlgOK.SetHeading(Translation.OnlineFavourites);
                    dlgOK.SetLine(1, Translation.TVDB_INFO_ACCOUNTID_1);
                    dlgOK.SetLine(2, Translation.TVDB_INFO_ACCOUNTID_2);
                    dlgOK.DoModal(GUIWindowManager.ActiveWindow);
                    MPTVSeriesLog.Write("Cannot submit online favourite, make sure you have your Account identifier set!");                    
                }
                else {
                    Online_Parsing_Classes.OnlineAPI.ConfigureFavourites(add, account, series[DBOnlineSeries.cID]);
                }
            }

            // Load Facade to reflect changes
            // We only need to reload when removing from a View that is active
            if (!add) {
                if (m_CurrLView.Name.Equals(selectedItem, StringComparison.CurrentCultureIgnoreCase) || 
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

        #region Episode Sort By Menu
        private void ShowEpisodeSortByMenu(DBSeries series, bool ChangeMatchingOrder)
        {
            IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null)
                return;

            dlg.Reset();
            if (ChangeMatchingOrder) 
                dlg.SetHeading(Translation.ChangeOnlineMatchOrder);
            else 
                dlg.SetHeading(Translation.SortBy);

            #region Add Menu items
            GUIListItem pItem = null;

            string currMatchOrder = series[DBOnlineSeries.cChosenEpisodeOrder].ToString();
            if (string.IsNullOrEmpty(currMatchOrder)) currMatchOrder = EpisodeSortByMenuItems.Aired.ToString();

            string currSortOrder = series[DBOnlineSeries.cEpisodeSortOrder].ToString();
            if (string.IsNullOrEmpty(currSortOrder)) currSortOrder = EpisodeSortByMenuItems.Aired.ToString();

            string selection = ChangeMatchingOrder ? currMatchOrder : currSortOrder;

            // For now we will just add sort options for the Aired and DVD Order            
            pItem = new GUIListItem(Translation.AiredOrder);
            dlg.Add(pItem);
            pItem.ItemId = (int)EpisodeSortByMenuItems.Aired;
            if (selection == EpisodeSortByMenuItems.Aired.ToString()) pItem.Selected = true;

            if (series[DBOnlineSeries.cEpisodeOrders].ToString().Contains(EpisodeSortByMenuItems.DVD.ToString()))
            {
                pItem = new GUIListItem(Translation.DVDOrder);
                dlg.Add(pItem);
                pItem.ItemId = (int)EpisodeSortByMenuItems.DVD;
                if (selection == EpisodeSortByMenuItems.DVD.ToString()) pItem.Selected = true;
            }

            // TODO: sort by absolute order and just show a single season in GUI...later
            if (series[DBOnlineSeries.cEpisodeOrders].ToString().Contains(EpisodeSortByMenuItems.Absolute.ToString()) && ChangeMatchingOrder)
            {
                pItem = new GUIListItem(Translation.AbsoluteOrder);
                dlg.Add(pItem);
                pItem.ItemId = (int)EpisodeSortByMenuItems.Absolute;
                if (selection == EpisodeSortByMenuItems.Absolute.ToString()) pItem.Selected = true;
            }

            if (ChangeMatchingOrder)
            {
                pItem = new GUIListItem(Translation.Title);
                dlg.Add(pItem);
                pItem.ItemId = (int)EpisodeSortByMenuItems.Title;
                if (selection == EpisodeSortByMenuItems.Title.ToString()) pItem.Selected = true;
            }
            #endregion

            #region Show Menu
            dlg.DoModal(GUIWindowManager.ActiveWindow);
            if (dlg.SelectedId < 0)
                return;

            // if we change matching order then we should also change
            // corresponding sort order
            switch (dlg.SelectedId)
            {
                case (int)EpisodeSortByMenuItems.Aired:
                    if (ChangeMatchingOrder)
                    {
                        series[DBOnlineSeries.cChosenEpisodeOrder] = EpisodeSortByMenuItems.Aired.ToString();
                        ChangeEpisodeMatchingOrder(series, EpisodeSortByMenuItems.Aired.ToString());
                    }
                    series[DBOnlineSeries.cEpisodeSortOrder] = EpisodeSortByMenuItems.Aired.ToString();
                    break;
                case (int)EpisodeSortByMenuItems.DVD:
                    if (ChangeMatchingOrder)
                    {
                        series[DBOnlineSeries.cChosenEpisodeOrder] = EpisodeSortByMenuItems.DVD.ToString();
                        ChangeEpisodeMatchingOrder(series, EpisodeSortByMenuItems.DVD.ToString());
                    }
                    series[DBOnlineSeries.cEpisodeSortOrder] = EpisodeSortByMenuItems.DVD.ToString();
                    break;
                case (int)EpisodeSortByMenuItems.Absolute:
                    if (ChangeMatchingOrder)
                    {
                        series[DBOnlineSeries.cChosenEpisodeOrder] = EpisodeSortByMenuItems.Absolute.ToString();
                        ChangeEpisodeMatchingOrder(series, EpisodeSortByMenuItems.Absolute.ToString());
                    }
                    break;
                case (int)EpisodeSortByMenuItems.Title:
                    series[DBOnlineSeries.cChosenEpisodeOrder] = EpisodeSortByMenuItems.Title.ToString();
                    ChangeEpisodeMatchingOrder(series, EpisodeSortByMenuItems.Title.ToString());
                    break;
            }
            #endregion

            // commit selection
            series.Commit();

            // Re-load the facade to re-sort episodes
            LoadFacade();
        }

        private void ChangeEpisodeMatchingOrder(DBSeries series, string order)
        {
            MPTVSeriesLog.Write("Changing Episode Match Order for {0}, to {1}", series.ToString(), order);

            // get list of local episodes to re-match
            SQLCondition conditions = new SQLCondition();
            conditions.Add(new DBEpisode(), DBEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
            List<DBEpisode> localEpisodes = DBEpisode.Get(conditions);
            OnlineParsing.matchOnlineToLocalEpisodes(series, localEpisodes, new GetEpisodes(series[DBSeries.cID]), order);
            return;
        }
        #endregion

        #region Delete Menu
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

            if (this.listLevel == Listlevel.Episode && episode != null && episode.checkHasLocalSubtitles())
            {
                pItem = new GUIListItem(Translation.DeleteSubtitles);
                dlg.Add(pItem);
                pItem.ItemId = (int)DeleteMenuItems.subtitles;
            }

            pItem = new GUIListItem(Translation.Cancel);            
            dlg.Add(pItem);
            pItem.ItemId = (int)DeleteMenuItems.cancel;

            // Show Menu
            dlg.DoModal(GUIWindowManager.ActiveWindow);
            if (dlg.SelectedId < 0 || dlg.SelectedId == (int)DeleteMenuItems.cancel) 
				return;

            List<string> resultMsg = null;
            string msgDlgCaption = string.Empty;

            #region Delete Subtitles
            if (dlg.SelectedId == (int)DeleteMenuItems.subtitles)
            {
                msgDlgCaption = Translation.UnableToDeleteSubtitles;
                switch (this.listLevel)
                {
                    case Listlevel.Episode:
                        if (episode == null) return;
                        resultMsg = episode.deleteLocalSubTitles();
                        break;
                }                
            }
            #endregion            

            #region Delete From Disk, Database or Both
            if (dlg.SelectedId != (int)DeleteMenuItems.subtitles)
            {
                msgDlgCaption = Translation.UnableToDelete;
                switch (this.listLevel)
                {
                    #region Delete Series
                    case Listlevel.Series:
                        resultMsg = series.deleteSeries((DeleteMenuItems)dlg.SelectedId);
                        break;
                    #endregion

                    #region Delete Season
                    case Listlevel.Season:
                        resultMsg = season.deleteSeason((DeleteMenuItems)dlg.SelectedId);
                        break;
                    #endregion

                    #region Delete Episode
                    case Listlevel.Episode:
                        resultMsg = episode.deleteEpisode((DeleteMenuItems)dlg.SelectedId);
                        break;
                    #endregion
                }
                // only update the counts if the database entry for the series still exists
                if (!DBSeries.IsSeriesRemoved) DBSeries.UpdateEpisodeCounts(series);                
            }
            #endregion

            // Re-load the facade to accurately reflect actions taked above
            LoadFacade();

            // Show errors, if any
            if (resultMsg != null && resultMsg.Count > 0)
            {
                GUIDialogText errorDialog = (GUIDialogText)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_TEXT);
                errorDialog.SetHeading(msgDlgCaption);
                errorDialog.SetText(string.Join("\n", resultMsg.ToArray()));
                errorDialog.DoModal(GUIWindowManager.ActiveWindow);
            }
        }
        #endregion

		#region Download Event Handllers
		void load_LoadNewzBinCompleted(bool bOK, String msgOut)
        {
            if (m_ImportAnimation != null)
                m_ImportAnimation.Dispose();

            if (!bOK)
            {
                GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                dlgOK.SetHeading(Translation.ErrorClear);
                dlgOK.SetLine(2, msgOut);
                dlgOK.DoModal(GUIWindowManager.ActiveWindow);
            }
            else
                if (m_Facade != null) LoadFacade();
        }

        void downloader_SubtitleRetrievalCompleted(DBEpisode episode, bool subtitleRetrieved, string errorMessage)
        {
            setProcessAnimationStatus(false);
            subtitleDownloaderWorking = false;
            GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);

            if (subtitleRetrieved)
            {
                if (episode != null)
                {
                    // refresh mediainfo to display subtitle logo
                    episode.ReadMediaInfo();
                }
                LoadFacade();
                //dlgOK.SetHeading(Translation.Completed);
                //dlgOK.SetLine(1, Translation.Subtitles_download_complete);
                //dlgOK.DoModal(GUIWindowManager.ActiveWindow);
            }
            else if (errorMessage != null)
            {
                GUIDialogText errorDialog = (GUIDialogText)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_TEXT);
                errorDialog.SetHeading(Translation.UnableToRetrieveSubtitles);
                errorDialog.SetText(errorMessage);
                errorDialog.DoModal(GUIWindowManager.ActiveWindow);
            }
            else
            {
                dlgOK.SetHeading(Translation.Completed);
                dlgOK.SetLine(1, Translation.NoSubtitlesFoundOrRetrieved);
                dlgOK.DoModal(GUIWindowManager.ActiveWindow);
            }
        }

        void Load_TorrentLoadCompleted(bool bOK)
        {
            setProcessAnimationStatus(false);
            torrentWorking = false;
            if (m_Facade != null) LoadFacade();
        }
		#endregion

		List<string> sviews = new List<string>();

        private void switchView(int offset) 
        {
			// Switch to previous view
            switchView(Helper.getElementFromList<logicalView, string>(m_CurrLView.Name, "Name", offset, m_allViews));
        }

		private bool switchView(string viewName)
        {
			// If view does not exist, default to 'ALL' internal view
			// If 'ALL' view does not exist use first view
            if (String.IsNullOrEmpty(viewName))
				viewName = Translation.All;

           return switchView(Helper.getElementFromList<logicalView, string>(viewName, "Name", 0, m_allViews));
        }

        void setCurPositionLabel()
        {
            string prettyCurrPosition = m_CurrLView.prettyName;
            foreach (string subPos in m_stepSelectionPretty)
                if (!String.IsNullOrEmpty(subPos))
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

        private bool switchView(logicalView view)
        {
            // Handle if view has been removed
            if (view == null) view = m_allViews[0]; 
            
			// Check if View has Parental Control enabled
			if (!CheckParentalControls(view)) {
                // We can't show a dialog on top when there is no main window
                if (!m_bPluginLoaded && m_bShowLastActiveModule && (m_iLastActiveModule == GetID)) {
                    MPTVSeriesLog.Write("Unable to Show PinCode Dialog, MediaPortal not ready, returning to Home screen");
                    m_bPluginLoaded = true;
                    return false;
                }

				// Prompt to choose UnProtected View
				return showViewSwitchDialog();				
			}
			
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
            return true;
        }

		private bool CheckParentalControls(logicalView view) {
			bool pinInCorrect = true;

			if (view.ParentalControl && logicalView.IsLocked) {
                // We can't show a dialog on top when there is no main window
                if (!m_bPluginLoaded && m_bShowLastActiveModule && (m_iLastActiveModule == GetID)) {
                    return false;
                }

                // Update Ugly Current View Property if not yet set
                if (TVSeriesPlugin.getGUIProperty(guiProperty.CurrentView.ToString()).Length == 0) {
                    setGUIProperty(guiProperty.CurrentView, Translation.ViewIsLocked);
                    setGUIProperty(guiProperty.SimpleCurrentView, Translation.ViewIsLocked);
				}

                MPTVSeriesLog.Write(string.Format("View: {0} is locked, prompting for PinCode", view.prettyName));

				// Check if Graphical PinCode dialog exists
                
				if (System.IO.File.Exists(GUIGraphicsContext.Skin + @"\TVSeries.PinCodeDialog.xml")) {
					try {
						GUIPinCode pinCodeDlg = (GUIPinCode)GUIWindowManager.GetWindow(GUIPinCode.ID);
						pinCodeDlg.Reset();

						// Initialize Dialog
						pinCodeDlg.MasterCode = DBOption.GetOptions(DBOption.cParentalControlPinCode);
						pinCodeDlg.EnteredPinCode = string.Empty;
						pinCodeDlg.SetHeading(Translation.PinCode);
						pinCodeDlg.SetLine(1, string.Format(Translation.PinCodeDlgLabel1, view.prettyName));
						pinCodeDlg.SetLine(2, Translation.PinCodeDlgLabel2);
						pinCodeDlg.Message = Translation.PinCodeMessageIncorrect;
						pinCodeDlg.DoModal(pinCodeDlg.GetID); //should this be (GUIWindowManager.ActiveWindow)?
						if (!pinCodeDlg.IsCorrect) {
							// Prompt to choose UnProtected View
                            MPTVSeriesLog.Write("PinCode entered was incorrect, showing Views Menu");							
							return false;
						}
						else
							logicalView.IsLocked = false;
					}
					catch (Exception ex) {
						MPTVSeriesLog.Write(string.Format("An Error occurred in the PinCode Dialog: {0}",ex.Message));
						return false;
					}
				}
				else {
					// Use Virtual Keyboard if skin doesnt exist
					while (pinInCorrect) {
						GetStringFromUserDescriptor Keyboard = new GetStringFromUserDescriptor();
						Keyboard.Text = string.Empty;
						Keyboard.IsPassword = true;
						// Force Keyboard Style to SMS Style as we dont want people to see what code is entered
						Keyboard.KeyboardStyle = GetStringFromUserDescriptor.KeyboardStyles.SMS;
						string enteredPinCode = string.Empty;
						string pinMasterCode = DBOption.GetOptions(DBOption.cParentalControlPinCode);
						if (pinMasterCode.Length == 0) break;

						if (this.GetStringFromUser(Keyboard, out enteredPinCode) == ReturnCode.OK) {
							// Convert SMS Input to Numbers							
							string smsCode = string.Empty;
							if (enteredPinCode.Length == 4) {								
								for (int i = 0; i < 4; i++) {
									smsCode += Helper.ConvertSMSInputToPinCode(enteredPinCode[i].ToString());
								}
							}
							// Check if PinCode is correct
							if (smsCode != pinMasterCode) {
								ShowPinCodeIncorrectMessage();
								pinInCorrect = true;
							}
							else {
								// Cease to prompt for PinCode for remainder of session
								logicalView.IsLocked = false;
								pinInCorrect = false;
							}
						}
						else {							
							return false;
						}
					}
				}
            }
			return true;
		}

        private void ShowPinCodeIncorrectMessage() {
            GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            dlgOK.SetHeading(Translation.PinCode);
            dlgOK.SetLine(1, Translation.PinCodeIncorrectLine1);
            dlgOK.SetLine(2, Translation.PinCodeIncorrectLine2);
            dlgOK.DoModal(GUIWindowManager.ActiveWindow);
        }

        void setNewListLevelOfCurrView(int step)
        {
            if (dummyIsSeriesPosters != null) 
            {
                dummyIsSeriesPosters.Visible = (DBOption.GetOptions(DBOption.cView_Series_ListFormat) == "Filmstrip" 
                                             || DBOption.GetOptions(DBOption.cView_Series_ListFormat) == "ListPosters" );
                dummyIsSeriesPosters.UpdateVisibility();
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

        bool fanartSet = false;
        Fanart currSeriesFanart = null;

        private bool loadFanart(DBTable item)
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

        private void DisableFanart()
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
                    {                        
                        MPTVSeriesLog.Write("Online Update Scan needed, last scan run @ {0}", m_LastUpdateScan);
                        m_LastUpdateScan = DateTime.Now;
                        bUpdateScanNeeded = true;
                    }
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
                    
                    else if (!torrentWorking && !subtitleDownloaderWorking)
                        setProcessAnimationStatus(false);
                }
            }
            base.Process();
        }

        void parserUpdater_OnlineParsingCompleted(bool bDataUpdated)
        {            
            setProcessAnimationStatus(false);

            if (m_parserUpdater.UpdateScan)
            {                
                DBOption.SetOptions(DBOption.cImport_OnlineUpdateScanLastTime, m_LastUpdateScan.ToString());
                setGUIProperty(guiProperty.LastOnlineUpdate, m_LastUpdateScan.ToString());
            }
            m_parserUpdaterWorking = false;
            if (bDataUpdated)
            {
               if (m_Facade != null) LoadFacade();
            }
        }

        void parserUpdater_OnlineParsingProgress(int nProgress, ParsingProgress progress)
        {
            if (progress != null && progress.CurrentItem > 0)
                MPTVSeriesLog.Write("progress received: {0} [{1}/{2}] {3}", progress.CurrentAction, progress.CurrentItem, progress.TotalItems, progress.CurrentProgress);

            // update the facade when progress has reached 30 (arbitrary point where local media has been scanned)
            if (nProgress == 30 && m_Facade != null) LoadFacade();
        }

		#region Facade Item Selected
		// triggered when a selection change was made on the facade
		private void onFacadeItemSelected(GUIListItem item, GUIControl parent) {
			// if this is not a message from the facade, exit
			if (parent != m_Facade && parent != m_Facade.FilmstripView &&
				parent != m_Facade.ThumbnailView && parent != m_Facade.ListView)
				return;

			switch (this.listLevel) {
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

		#region Group Item Selected
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

                bool requiresSplit = false; // use sql 'like' for split fields
                
                // selected group label
                string selectedItem = this.m_Facade.SelectedListItem.Label.ToString();
                
                // unknown really is "" so get all with null values here
                if (selectedItem == Translation.Unknown)
                {                    
                    selectedItem = string.Empty;
                }
                else
                    if (m_CurrLView.m_steps[m_CurrViewStep].groupedBy.attempSplit) requiresSplit = true;

                string tableName = "online_series";
                string tableField = tableName + "." + groupedBy.Substring(groupedBy.IndexOf('.') + 1).Replace(">", "");
                string userEditField = tableField + DBTable.cUserEditPostFix;
                string value = requiresSplit ? "like " + "'%" + selectedItem + "%'" : "= " + "'" + selectedItem + "'";

                // check if the useredit column exists
                string sql = "select " + userEditField + " from " + tableName;
                SQLite.NET.SQLiteResultSet results = DBTVSeries.Execute(sql);                

                if (results.Rows.Count > 0)
                {
                    sql = "(case when (" + userEditField + " is null or " + userEditField + " = " + "'" + "'" + ") " +
                             "then " + tableField + " else " + userEditField + " " +
                             "end) " + value;
                }
                else
                {
                    sql = tableField + " " + value;
                }

                cond.AddCustom(sql);

                if (DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles))
                {
                    // not generic
                    SQLCondition fullSubCond = new SQLCondition();
                    fullSubCond.AddCustom(DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID), DBOnlineSeries.Q(DBOnlineSeries.cID), SQLConditionType.Equal);
                    cond.AddCustom(" exists( " + DBEpisode.stdGetSQL(fullSubCond, false) + " )");
                }
                if (!DBOption.GetOptions(DBOption.cShowHiddenItems))
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
		#endregion

		#region Series Item Selected
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
		#endregion

		#region Season Item Selected
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
            string filename = ImageAllocator.GetSeasonBannerAsFilename(season);
            if (filename.Length == 0) {
                // Load Series Poster instead
                if (DBOption.GetOptions(DBOption.cSubstituteMissingArtwork) && m_SelectedSeries != null) {
                    filename = ImageAllocator.GetSeriesPosterAsFilename(m_SelectedSeries);
                }
            }
            seasonbanner.Filename = filename;            

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
		#endregion

		#region Episode Item Selected
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
           
            m_SelectedEpisode = episode;
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
                    string filename = ImageAllocator.GetSeasonBannerAsFilename(m_SelectedSeason);
                    if (filename.Length == 0) {
                        // Load Series Poster instead
                        if (DBOption.GetOptions(DBOption.cSubstituteMissingArtwork) && m_SelectedSeries != null) {
                            filename = ImageAllocator.GetSeriesPosterAsFilename(m_SelectedSeries);
                        }
                    }
                    seasonbanner.Filename = filename;

                    pushFieldsToSkin(m_SelectedSeason, "Season");
                }
                else
                    clearGUIProperty(guiProperty.SeasonPoster);

                m_bUpdateBanner = false;
            }
            pushFieldsToSkin(m_SelectedEpisode, "Episode");

            // Load Fanart for Selected Series, might be in Episode Only View e.g. Recently Added, Latest		
            if (m_SelectedSeries == null) return;

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
		#endregion
		#endregion

		#region Feedback
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
				VirtualKeyboard keyboard;
				if (descriptor.KeyboardStyle == GetStringFromUserDescriptor.KeyboardStyles.SMS) {
					keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)Window.WINDOW_VIRTUAL_SMS_KEYBOARD);
				}
				else {
					keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)Window.WINDOW_VIRTUAL_KEYBOARD);
				}
                if (null == keyboard)
                    return ReturnCode.Cancel;

                keyboard.Reset();
                keyboard.Text = descriptor.Text;
                keyboard._shiftTurnedOn = descriptor.ShiftEnabled;
				keyboard.Password = descriptor.IsPassword;				
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
		#endregion

		private void playRandomEp()
        {
            List<DBEpisode> episodeList = m_CurrLView.getAllEpisodesForStep(m_CurrViewStep, m_stepSelection);

            episodeList = FilterEpisodeList(episodeList, true, false);

            m_SelectedEpisode = GetRandomEpisode(episodeList);

            if (m_SelectedEpisode == null) {
                ShowNotifyDialog(Translation.PlayError, string.Format(Translation.UnableToFindAny, Translation.RandomEpisode));
            }
            else {
                MPTVSeriesLog.Write("Selected Random Episode: ", m_SelectedEpisode[DBEpisode.cCompositeID].ToString(), MPTVSeriesLog.LogLevel.Debug);

                // removed the if statement here to mimic functionality when an episode is selected
                // via the regular UI, since watched flag is now set after viewing (is this right?)
                //m_VideoHandler.ResumeOrPlay(selectedEpisode);
                CommonPlayEpisodeAction(false);
            }
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
                        m_ImportAnimation.Dispose();
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
                // Go through all enabled import folders, and add a watchfolder on it
                foreach (DBImportPath importPath in importPaths)
                {
                    if (importPath[DBImportPath.cEnabled] != 0)
                        importFolders.Add(importPath[DBImportPath.cPath]);
                }
            }
                
            m_watcherUpdater = new Watcher(importFolders, DBOption.GetOptions(DBOption.cImport_ScanRemoteShareLapse));
            m_watcherUpdater.WatcherProgress += new Watcher.WatcherProgressHandler(watcherUpdater_WatcherProgress);
            m_watcherUpdater.StartFolderWatch();
        }

        private void stopFolderWatches()
        {
            m_watcherUpdater.StopFolderWatch();
            m_watcherUpdater.WatcherProgress -= new Watcher.WatcherProgressHandler(watcherUpdater_WatcherProgress);
            m_watcherUpdater = null;
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
                    if (!TVSeriesPlugin.IsNetworkAvailable) {
                        dlgOK.SetLine(1, Translation.NETWORK_ERROR_UNAVAILABLE_1);
                        dlgOK.SetLine(2, Translation.NETWORK_ERROR_UNAVAILABLE_2);
                    }
                    else {
                        dlgOK.SetLine(1, Translation.TVDB_ERROR_UNAVAILABLE_1);
                        dlgOK.SetLine(2, Translation.TVDB_ERROR_UNAVAILABLE_2);
                    }
                    
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

		#region Playlist Actions
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
                if (DBOption.GetOptions(DBOption.cPlaylistUnwatchedOnly))
                    condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cWatched, false, SQLConditionType.Equal);
            }
            else if (this.listLevel == Listlevel.Season && m_SelectedSeason != null)
            {
                condition.Add(new DBEpisode(), DBEpisode.cSeriesID, m_SelectedSeries[DBSeries.cID], SQLConditionType.Equal);
                condition.Add(new DBEpisode(), DBEpisode.cSeasonIndex, m_SelectedSeason[DBSeason.cIndex], SQLConditionType.Equal);
                if (DBOption.GetOptions(DBOption.cPlaylistUnwatchedOnly))
                    condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cWatched, false, SQLConditionType.Equal);
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
            // Set TVSeries Playlist Extension
            VirtualDirectory _virtualDirectory = new VirtualDirectory();
            _virtualDirectory.AddExtension(".tvsplaylist");

            // Get All Playlists found in Directory
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

            // Create Playist Menu Dialog
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null)
                return;
            dlg.Reset();
            dlg.SetHeading(983); // Saved Playlists
            
            // Add all playlists found to Menu for selection
            foreach (GUIListItem item in itemlist)
            {
                MediaPortal.Util.Utils.SetDefaultIcons(item);
                dlg.Add(item);
            }

            // Show Plaulist Menu Dialog
            dlg.DoModal(GetID);

            // Nothing was selected e.g. BACK
			if (dlg.SelectedLabel == -1)
				return;			

            GUIListItem selectItem = itemlist[dlg.SelectedLabel];
            
            // If Item selected was a Folder, re-curse to show contents
            if (selectItem.IsFolder)
            {
                OnShowSavedPlaylists(selectItem.Path);
                return;
            }
            
            // Load the Selected Playlist
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
                // If the file is an image file, it should be mounted before playing
                string filename = playlist[0].FileName;
                if (Helper.IsImageFile(filename)) {
                    if (!GUIVideoFiles.MountImageFile(GUIWindowManager.ActiveWindow, filename)) {
                        return;
                    }
                }

                MPTVSeriesLog.Write(string.Format("GUITVSeriesPlaylist: play single playlist item - {0}", playlist[0].FileName));
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
                playlist = _playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_TVSERIES);

				// autoshuffle on load
				if (_playlistPlayer.PlaylistAutoShuffle) {
					playlist.Shuffle();
				}

				// then get 1st item
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
		#endregion

        private void CommonPlayEpisodeAction(bool useDownloaders) {
            if (useDownloaders && m_SelectedEpisode[DBEpisode.cFilename].ToString().Length == 0) {
                // we don't have this file - yet. If downloaders are available, show the download pages
                ShowDownloadMenu(m_SelectedEpisode);
            }
            else if (!m_SelectedEpisode[DBEpisode.cAvailableSubtitles] && DBOption.GetOptions(DBOption.cPlay_SubtitleDownloadOnPlay) && SubtitleDownloaderEnabledAndHasSites()) {
                ShowSubtitleMenu(m_SelectedEpisode, true);
            }
            else
                m_VideoHandler.ResumeOrPlay(m_SelectedEpisode);
        }

        private DBEpisode GetFirstOrLastEpisode(List<DBEpisode> episodeList, bool first) {
            DBEpisode result = null;

            if (episodeList == null || episodeList.Count == 0) return result;

            // lame grouping by series / season / episode - doh, results are already sorted that way!
            //episodeList.Sort(new Comparison<DBEpisode>((x, y) => {
            //    return 4 * string.Compare(x[DBOnlineEpisode.cSeriesID].ToString(), y[DBOnlineEpisode.cSeriesID].ToString()) +
            //           2 * string.Compare(x[DBOnlineEpisode.cSeasonIndex].ToString(), y[DBOnlineEpisode.cSeasonIndex].ToString()) +
            //           1 * string.Compare(x[DBOnlineEpisode.cEpisodeIndex].ToString(), y[DBOnlineEpisode.cEpisodeIndex].ToString())
            //           ;
            //}));

            List<DBEpisode> episodeListNew = new List<DBEpisode>();

            // take first or last episode from each series..
            for (int i = 0; i < episodeList.Count; i++) {
                if (first) {
                    if (i > 0) {
                        if (episodeList[i - 1][DBOnlineEpisode.cSeriesID].ToString() != episodeList[i][DBOnlineEpisode.cSeriesID]) {
                            // it's the first one in series
                            episodeListNew.Add(episodeList[i]);
                        }
                    }
                    else {
                        // add first one
                        episodeListNew.Add(episodeList[i]);
                    }
                }
                else {
                    if ((i + 1) < episodeList.Count) {
                        if (episodeList[i + 1][DBOnlineEpisode.cSeriesID].ToString() != episodeList[i][DBOnlineEpisode.cSeriesID]) {
                            // it's the last one in series
                            episodeListNew.Add(episodeList[i]);
                        }
                    }
                    else {
                        // add the last one
                        episodeListNew.Add(episodeList[i]);
                    }
                }
            }

            if (episodeListNew.Count == 1) return episodeListNew[0];

            // there are multiple series selected, sort by series name? aired date? date added?
            // i decided to sort them by aired date and then by series name
            // only sort when searching for latest...makes sense because first unwatched will then reflect the view data.. and latest does not have to do that
            if (!first) {
                episodeListNew.Sort(new Comparison<DBEpisode>((x, y) => {
                    DBSeries seriesX = Helper.getCorrespondingSeries(x[DBOnlineEpisode.cSeriesID]);
                    DBSeries seriesY = Helper.getCorrespondingSeries(y[DBOnlineEpisode.cSeriesID]);
                    string seriesXSortName = seriesX != null ? seriesX[DBOnlineSeries.cSortName].ToString() : string.Empty;
                    string seriesYSortName = seriesY != null ? seriesY[DBOnlineSeries.cSortName].ToString() : string.Empty;
                    return 2 * string.Compare(x[DBOnlineEpisode.cFirstAired].ToString(), y[DBOnlineEpisode.cFirstAired].ToString()) +
                        //2 * string.Compare(x[DBEpisode.cFileDateAdded].ToString(), y[DBEpisode.cFileDateAdded].ToString()) +
                           1 * string.Compare(seriesXSortName, seriesYSortName)
                           ;
                }));
            }

            if (first)
                result = episodeListNew[0];
            else
                result = episodeListNew[episodeListNew.Count - 1];

            return result;
        }

        private DBEpisode GetRandomEpisode(List<DBEpisode> episodeList) {
            DBEpisode result = null;

            if (episodeList == null || episodeList.Count == 0) return result;

            result = episodeList[new Random().Next(0, episodeList.Count - 1)];

            return result;
        }

        private List<DBEpisode> FilterEpisodeList(List<DBEpisode> episodeList, bool filterUnavailable, bool filterWatched) {
            List<DBEpisode> result = new List<DBEpisode>();

            if (episodeList == null || episodeList.Count == 0) return result;

            foreach (DBEpisode episode in episodeList) {
                if (filterUnavailable && (episode[DBEpisode.cFilename].ToString().Length == 0 || episode[DBEpisode.cIsAvailable] != 1)) {
                    continue;
                }

                if (filterWatched && episode[DBOnlineEpisode.cWatched]) {
                    continue;
                }

                result.Add(episode);
            }

            return result;
        }

        /// <summary>
        /// Displays a menu dialog from list of items
        /// </summary>
        /// <returns>Selected item index, -1 if exited</returns>
        public static int ShowMenuDialog(string heading, List<GUIListItem> items) {
            return ShowMenuDialog(heading, items, -1);
        }

        private delegate int ShowMenuDialogDelegate(string heading, List<GUIListItem> items);

        /// <summary>
        /// Displays a menu dialog from list of items
        /// </summary>
        /// <returns>Selected item index, -1 if exited</returns>
        public static int ShowMenuDialog(string heading, List<GUIListItem> items, int selectedItemIndex) {
            if (GUIGraphicsContext.form.InvokeRequired) {
                ShowMenuDialogDelegate d = ShowMenuDialog;
                return (int)GUIGraphicsContext.form.Invoke(d, heading, items);
            }

            GUIDialogMenu dlgMenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlgMenu == null) return -1;

            dlgMenu.Reset();

            dlgMenu.SetHeading(heading);

            foreach (GUIListItem item in items) {
                dlgMenu.Add(item);
            }

            if (selectedItemIndex >= 0)
                dlgMenu.SelectedLabel = selectedItemIndex;

            dlgMenu.DoModal(GUIWindowManager.ActiveWindow);

            if (dlgMenu.SelectedLabel < 0) {
                return -1;
            }

            return dlgMenu.SelectedLabel;
        }

        /// <summary>
        /// Displays a notification dialog.
        /// </summary>
        public static void ShowNotifyDialog(string heading, string text) {
            ShowNotifyDialog(heading, text, string.Empty);
        }

        private delegate void ShowNotifyDialogDelegate(string heading, string text, string image);

        /// <summary>
        /// Displays a notification dialog.
        /// </summary>
        public static void ShowNotifyDialog(string heading, string text, string image) {
            if (GUIGraphicsContext.form.InvokeRequired) {
                ShowNotifyDialogDelegate d = ShowNotifyDialog;
                GUIGraphicsContext.form.Invoke(d, heading, text, image);
                return;
            }

            GUIDialogNotify pDlgNotify = (GUIDialogNotify)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
            if (pDlgNotify == null) return;

            pDlgNotify.SetHeading(heading);

            pDlgNotify.SetImage(image);

            pDlgNotify.SetText(text);

            pDlgNotify.DoModal(GUIWindowManager.ActiveWindow);
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
                this.m_Logos_Image.Dispose();
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



