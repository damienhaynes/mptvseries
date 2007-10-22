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
using MediaPortal.Playlists;
using WindowPlugins.GUITVSeries;
using System.Threading;
using System.Collections.Generic;
using WindowPlugins.GUITVSeries.Subtitles;
using WindowPlugins.GUITVSeries.Feedback;
using Download = WindowPlugins.GUITVSeries.Download;
using Torrent = WindowPlugins.GUITVSeries.Torrent;
using Newzbin = WindowPlugins.GUITVSeries.Newzbin;

namespace MediaPortal.GUI.Video
{
    public class TVSeriesPlugin : GUIWindow, ISetupForm, Interface
    {
        public TVSeriesPlugin()
        {
            m_stepSelections.Add(new string[] { null });
            // disable that dynamic skin adjustment....skinners should have the power to position the elements whereever with the plugin inerveining
            if (DBOption.GetOptions(DBOption.cViewAutoHeight)) DBOption.SetOptions(DBOption.cViewAutoHeight, false);
            if (DBOption.GetOptions(DBOption.cMainMirror) == null) DBOption.SetOptions(DBOption.cMainMirror, "http://www.thetvdb.com/interfaces"); // this is the default main mirrors
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
        // should return true to this method, otherwise if it should not be on home
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

        private Listlevel listLevel = Listlevel.Series;
        private DBSeries m_SelectedSeries;
        private DBSeason m_SelectedSeason;
        private DBEpisode m_SelectedEpisode;
        private VideoHandler m_VideoHandler;
        List<logicalView> m_allViews = new List<logicalView>();
        private logicalView m_CurrLView = null;
        private int m_CurrViewStep = 0;
        private List<string[]> m_stepSelections = new List<string[]>();
        private string[] m_stepSelection = null;
        private bool skipSeasonIfOne_DirectionDown = true;
        private string[] m_back_up_select_this = null;
        private bool foromWorking = false;
        private bool torrentWorking = false;
        private string foromID = DBOption.GetOptions(DBOption.cSubs_Forom_ID);

        private TimerCallback m_timerDelegate = null;
        private System.Threading.Timer m_scanTimer = null;
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
        private int logosHeight = 100;
        private int logosWidth = 250;

        #region Skin Variables

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
            Translation.Init();
            MPTVSeriesLog.Write("**** Plugin started in MediaPortal ***");
            String xmlSkin = GUIGraphicsContext.Skin + @"\TVSeries.xml";
            MPTVSeriesLog.Write("Loading XML Skin: " + xmlSkin);

            m_VideoHandler = new VideoHandler();

            m_parserUpdater = new OnlineParsing(this);
            m_parserUpdater.OnlineParsingCompleted += new OnlineParsing.OnlineParsingCompletedHandler(parserUpdater_OnlineParsingCompleted);

            if (DBOption.GetOptions("doFolderWatch"))
            {
                setUpFolderWatches();

                // always do a local scan when starting up the app - later on the watcher will monitor changes
                m_parserUpdaterQueue.Add(new CParsingParameters(true, false));
                Microsoft.Win32.SystemEvents.PowerModeChanged += new Microsoft.Win32.PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
            }
            else
            {
                // else the user has selected to always manually do local scans
                setProcessAnimationStatus(false);
            }

            // init display format strings
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
            return Load(xmlSkin);
        }

        void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            if (e.Mode == Microsoft.Win32.PowerModes.Resume)
            {
                // event is only registered if watch folder option is ticked, so no need to check again here
                // we have to reregister the folder watches
                setUpFolderWatches();

                // lets do a full folder scan since we might have network shares which could have been updated
                m_parserUpdaterQueue.Add(new CParsingParameters(true, false));
            }
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
                    m_parserUpdaterQueue.Add(new CParsingParameters(ParsingAction.List_Add, filesAdded));
                }
            }

            if (filesRemoved.Count > 0)
            {
                // queue it
                lock (m_parserUpdaterQueue)
                {
                    m_parserUpdaterQueue.Add(new CParsingParameters(ParsingAction.List_Remove, filesRemoved));
                }
            }
        }

        void setFacadeMode(GUIFacadeControl.ViewMode mode)
        {
            if (this.dummyThumbnailGraphicalMode == null || mode == GUIFacadeControl.ViewMode.List)
            {
                this.m_Facade.View = mode;
                if (this.dummyThumbnailGraphicalMode != null)
                    dummyThumbnailGraphicalMode.Visible = false;
            }
            else 
            {
                if (mode == GUIFacadeControl.ViewMode.AlbumView)
                {
                    MPTVSeriesLog.Write("FacadeMode: Switching to LargeIcons");
                    this.m_Facade.View = GUIFacadeControl.ViewMode.LargeIcons;
                }
                this.dummyThumbnailGraphicalMode.Visible = mode == GUIFacadeControl.ViewMode.AlbumView; // so you can trigger animations
            }
            if(dummyFacadeListMode != null)
                this.dummyFacadeListMode.Visible = this.m_Facade.View == GUIFacadeControl.ViewMode.List;            
        }
        /*
        bool facadeLoaded = false;
        void LoadFacade()
        {
            System.ComponentModel.BackgroundWorker bg = new System.ComponentModel.BackgroundWorker();
            bg.DoWork += new System.ComponentModel.DoWorkEventHandler(bgLoadFacade);
            bg.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(bgFacadeDone);

            bg.RunWorkerAsync();
            //GUIWindowManager.Process();
            //while (!facadeLoaded) ;
        }

        void bgFacadeDone(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            facadeLoaded = true;
        }

        void bgLoadFacade(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            facadeLoaded = false; // reset
            using (WaitCursor c = new WaitCursor())
                lock (this) { bgLoadFacade(); }
        }

        void bgLoadFacade()*/
        void LoadFacade()
        {
            MPTVSeriesLog.Write("Begin LoadFacade");
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
                if(this.m_Facade.ThumbnailView != null)
                    this.m_Facade.ThumbnailView.Clear();
                bool bEmpty = true;
                MPTVSeriesLog.Write("LoadFacade: ListLevel: ", listLevel.ToString(), MPTVSeriesLog.LogLevel.Normal);
                switch (this.listLevel)
                {
                    #region Group
                    case Listlevel.Group:
                        {
                            ImageAllocator.FlushAll();
                            // these are groups of certain categories, eg. Genres
                            // always list mode
                            setFacadeMode(GUIFacadeControl.ViewMode.List);   
                            int selectedIndex = -1;
                             // view handling
                            List<string> items = m_CurrLView.getGroupItems(m_CurrViewStep, m_stepSelection);

                            for (int index = 0; index < items.Count; index++)
                            {
                                bEmpty = false;
                                GUIListItem item = new GUIListItem(items[index]);
                                if (item.Label.Length == 0) item.Label = Translation.Unknown;
                                item.TVTag = items[index];
                                item.IsRemote = true;
                                item.IsDownloading = true;

                                // also display fist logo in list directly
                                item.IconImage = item.IconImageBig = localLogos.getLogos(m_CurrLView.groupedInfo(m_CurrViewStep), item.Label, 0, 0);

                                this.m_Facade.Add(item);

                                if (m_back_up_select_this != null && selectedIndex == -1 && item.Label == m_back_up_select_this[0])
                                    selectedIndex = index;
                            }
                            if (selectedIndex > -1)
                                this.m_Facade.SelectedListItemIndex = selectedIndex;
                        }
                        break;
                    #endregion
                    #region Series
                    case Listlevel.Series:
                        {
                            if (DBOption.GetOptions(DBOption.cRandomBanner)) ImageAllocator.FlushAll();
                            else
                            {
                                // flush episodes & seasons
                                ImageAllocator.FlushOthers(false);
                                ImageAllocator.FlushSeasons();
                            }

                            int nSeriesDisplayMode = DBOption.GetOptions(DBOption.cView_Series_ListFormat);
                            int selectedIndex = -1;
                            int count = 0;

                            if (nSeriesDisplayMode == 1)
                            {
                                // graphical
                                setFacadeMode(GUIFacadeControl.ViewMode.AlbumView);
                                // assume 758 x 140 for all banners
                                Size sizeImage = new Size();

                                m_Facade.AlbumListView.IconOffsetX = m_nInitialIconXOffset;
                                sizeImage.Width = m_Facade.AlbumListView.Width - 2 * m_Facade.AlbumListView.IconOffsetX;
                                sizeImage.Height = sizeImage.Width * 140 / 758;
                                m_Facade.AlbumListView.IconOffsetY = m_nInitialIconYOffset * sizeImage.Height / m_nInitialItemHeight;
                                m_Facade.AlbumListView.ItemHeight = sizeImage.Height + 2 * m_Facade.AlbumListView.IconOffsetY;
                                m_Facade.AlbumListView.SetImageDimensions(sizeImage.Width, sizeImage.Height);
                                m_Facade.AlbumListView.AllocResources();
                            }
                            else
                            {
                                // text as usual
                                setFacadeMode(GUIFacadeControl.ViewMode.List);
                            }
                            // view handling
                            List<DBSeries> seriesList = m_CurrLView.getSeriesItems(m_CurrViewStep, m_stepSelection);
                            MPTVSeriesLog.Write("LoadFacade: BeginDisplayLoopSeries: ", seriesList.Count.ToString(), MPTVSeriesLog.LogLevel.Normal);
                            foreach (DBSeries series in seriesList)
                            {
                                try
                                {
                                    bEmpty = false;
                                    GUIListItem item = null;
                                    if (nSeriesDisplayMode == 1)
                                    {
                                        item = new GUIListItem();
                                        string img = ImageAllocator.GetSeriesBanner(series);

                                        if (Helper.String.IsNullOrEmpty(img))
                                            item.Label = FieldGetter.resolveDynString(m_sFormatSeriesCol2, series);
                                        else item.IconImage = item.IconImageBig = img;
                                    }
                                    else
                                    {
                                        item = new GUIListItem(FieldGetter.resolveDynString(m_sFormatSeriesCol2, series));
                                        item.Label2 = FieldGetter.resolveDynString(m_sFormatSeriesCol3, series);
                                        item.Label3 = FieldGetter.resolveDynString(m_sFormatSeriesCol1, series);
                                    }
                                    item.TVTag = series;
                                    item.IsRemote = series[DBOnlineSeries.cHasLocalFiles] != 0;
                                    item.IsDownloading = true;

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
                                            item.Selected = true;
                                        }
                                    }
                                    if (m_back_up_select_this != null && series != null && selectedIndex == -1 && series[DBSeries.cID] == m_back_up_select_this[0])
                                    {
                                        selectedIndex = count;
                                        item.Selected = true;
                                    }

                                    this.m_Facade.Add(item);
                                }
                                catch (Exception ex)
                                {
                                    MPTVSeriesLog.Write("The 'LoadFacade' function has generated an error displaying series list item: " + ex.Message);
                                }
                                count++;
                            }
                            if (selectedIndex != -1)
                            {
                                this.m_Facade.SelectedListItemIndex = selectedIndex;

                                //System.Reflection.FieldInfo fi =
                                //this.m_Facade.ThumbnailView.GetType().GetField("iItem", System.Reflection.BindingFlags.NonPublic);
                                //fi.SetValue(this.m_Facade.ThumbnailView, selectedIndex);
                            }
                        }
                        break;
                    #endregion
                    #region Season
                    case Listlevel.Season:
                        {
                            int selectedIndex = -1;
                            int count = 0;
                            int nSeasonDisplayMode = DBOption.GetOptions(DBOption.cView_Season_ListFormat);
                            if (nSeasonDisplayMode == 1)
                            {
                                setFacadeMode(GUIFacadeControl.ViewMode.AlbumView);
                                // assume 400 x 578 for all season images
                                Size sizeImage = new Size();
                                // reverse, 1 season picture by page
                                // integrate the size difference in the offset so the image stays centered & doesn't go "out" of the selection box
                                sizeImage.Height = m_Facade.AlbumListView.Height - m_Facade.AlbumListView.Space - m_Facade.AlbumListView.SpinHeight - 6; // taken from how itemsperpage is calculated in GUIListControl
                                m_Facade.AlbumListView.IconOffsetY = m_nInitialIconYOffset * (sizeImage.Height - 2 * m_Facade.AlbumListView.IconOffsetY) / m_nInitialItemHeight;
                                sizeImage.Height -= 2 * m_Facade.AlbumListView.IconOffsetY;
                                sizeImage.Width = sizeImage.Height * 400 / 578;
                                m_Facade.AlbumListView.ItemHeight = sizeImage.Height + 2 * m_Facade.AlbumListView.IconOffsetY;
                                m_Facade.AlbumListView.SetImageDimensions(sizeImage.Width, sizeImage.Height);
                                m_Facade.AlbumListView.IconOffsetX = (m_Facade.AlbumListView.Width - sizeImage.Width) / 2;
                                m_Facade.AlbumListView.AllocResources();
                            }
                            else
                            {
                                setFacadeMode(GUIFacadeControl.ViewMode.List);
                            }

                            // view handling
                            List<DBSeason> seasons = m_CurrLView.getSeasonItems(m_CurrViewStep, m_stepSelection);
                            MPTVSeriesLog.Write("LoadFacade: BeginDisplayLoopSeason: ", seasons.Count.ToString(), MPTVSeriesLog.LogLevel.Normal);
                            bool canBeSkipped = seasons.Count == 1;
                            foreach (DBSeason season in seasons)
                            {
                                try
                                {
                                    bEmpty = false;
                                    GUIListItem item = null;
                                    if (!canBeSkipped)
                                    {
                                        if (nSeasonDisplayMode == 1)
                                        {
                                            item = new GUIListItem();
                                            item.IconImage = item.IconImageBig = ImageAllocator.GetSeasonBanner(season, true);
                                        }
                                        else
                                        {
                                            item = new GUIListItem(FieldGetter.resolveDynString(m_sFormatSeasonCol2, season));
                                            if (!m_CurrLView.stepHasSeriesBeforeIt(m_CurrViewStep))
                                                // somehow the seriesname should be displayed too I guess, but this is more important in the episodes view

                                                item.Label2 = FieldGetter.resolveDynString(m_sFormatSeasonCol3, season);
                                            item.Label3 = FieldGetter.resolveDynString(m_sFormatSeasonCol1, season);
                                            item.IconImage = ImageAllocator.GetSeasonBanner(season, false);
                                        }
                                        item.IsRemote = season[DBSeason.cHasLocalFiles] != 0;
                                        item.IsDownloading = true;
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
                                            if (season[DBOnlineSeries.cHasLocalFiles] != 0 && selectedIndex == -1)
                                                selectedIndex = count;
                                        }
                                        if (m_back_up_select_this != null && season != null && selectedIndex == -1 && season[DBSeason.cSeriesID] == m_back_up_select_this[0] && season[DBSeason.cIndex] == m_back_up_select_this[1])
                                            selectedIndex = count;
                                    }
                                    else
                                    {
                                        // since onseasonselected won't be triggered automatically, we have to force it
                                        Season_OnItemSelected(item);
                                    }
                                    this.m_Facade.Add(item);
                                }
                                catch (Exception ex)
                                {
                                    MPTVSeriesLog.Write("The 'LoadFacade' function has generated an error displaying season list item: " + ex.Message);
                                }
                                count++;
                            }

                            if (selectedIndex != -1)
                                this.m_Facade.SelectedListItemIndex = selectedIndex;
                            // if there is only one season to display, skip directly to the episodes list
                            if (skipSeasonIfOne_DirectionDown && seasons.Count == 1)
                            {
                                MPTVSeriesLog.Write("Skipping season display");
                                OnClicked(m_Facade.GetID, m_Facade, Action.ActionType.ACTION_SELECT_ITEM);
                            }
                            else if (seasons.Count == 1)
                            {
                                // we're back from the ep list, go up one hierarchy more (depending on view, most likly series)
                                MPTVSeriesLog.Write("Skipping season display");
                                OnAction(new Action(Action.ActionType.ACTION_PREVIOUS_MENU, 0, 0));
                            }
                        }
                        break;
                    #endregion
                    #region Episode
                    case Listlevel.Episode:
                        {
                            int selectedIndex = -1;
                            int count = 0;
                            bool bFindNext = false;
                            setFacadeMode(GUIFacadeControl.ViewMode.List);
                            List<DBEpisode> episodesToDisplay = m_CurrLView.getEpisodeItems(m_CurrViewStep, m_stepSelection);
                            MPTVSeriesLog.Write("LoadFacade: BeginDisplayLoopEp: ", episodesToDisplay.Count.ToString(), MPTVSeriesLog.LogLevel.Normal);
                            perfana.logMeasure(MPTVSeriesLog.LogLevel.Normal);
                            GUIListItem item = null;
                            foreach (DBEpisode episode in episodesToDisplay)
                            {
                                try
                                {
                                    bEmpty = false;
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
                                    item.IsRemote = episode[DBEpisode.cFilename].ToString().Length > 0;
                                    item.IsDownloading = episode[DBOnlineEpisode.cWatched] == 0;
                                    item.TVTag = episode;

                                    if (this.m_SelectedEpisode != null)
                                    {
                                       if (episode[DBEpisode.cCompositeID] == this.m_SelectedEpisode[DBEpisode.cCompositeID]) 
                                       {

                                           if (!this.m_SelectedEpisode[DBOnlineEpisode.cWatched])
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

                                        // first returned logo should also show up here in list view directly
                                    item.IconImage = localLogos.getFirstEpLogo(episode);
                                    this.m_Facade.Add(item);
                                }
                                catch (Exception ex)
                                {
                                    MPTVSeriesLog.Write("The 'LoadFacade' function has generated an error displaying episode list item: " + ex.Message);
                                }
                                count++;
                            }
                            this.m_Facade.Focus = true;
                            if (selectedIndex != -1)
                            {
                                this.m_Facade.SelectedListItemIndex = selectedIndex;
                            }

                        }
                        MPTVSeriesLog.Write("LoadFacade: Finish");
                        break;
                    #endregion
                }
                if (bEmpty)
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
                GUIControl.FocusControl(m_Facade.GetID, m_Facade.ListView.GetID, GUIControl.Direction.Left);
            }

            catch (Exception e)
            {
                MPTVSeriesLog.Write("The 'LoadFacade' function has generated an error: " + e.Message);
            }
            perfana.logMeasure(MPTVSeriesLog.LogLevel.Normal);
        }

        protected override void OnPageLoad()
        {
            if (m_Facade == null) // wrong skin file
            {
                GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                dlg.Reset();
                dlg.SetHeading(Translation.wrongSkin);
                dlg.DoModal(GetID);
                GUIWindowManager.ShowPreviousWindow();
                return;
            }
            ImageAllocator.SetFontName(m_Facade.AlbumListView == null ? m_Facade.ListView.FontName : m_Facade.AlbumListView.FontName);

            // for some reason on non initial loads (such as coming back from fullscreen video or after having exited to home and coming back)
            // the labels don't display, unless we somehow call them like so
            // (no, allocResources doesnt work!)
            clearGUIProperty(guiProperty.Subtitle);
            clearGUIProperty(guiProperty.Title);
            clearGUIProperty(guiProperty.Description);

            clearGUIProperty(guiProperty.CurrentView);
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
                    catch (Exception){}
                }
                else MPTVSeriesLog.Write("Error, cannot display items because: No Views have been found!");
            }
            else setViewLabels();
            loadFanart(null); // init dummy labels
            LoadFacade();
            m_Facade.Focus = true;
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
            base.OnPageDestroy(new_windowId);
        }

        enum guiProperty
        {
            Title,
            Subtitle,
            Description,
            CurrentView,
            NextView,
            LastView,
            SeriesBanner,
            SeasonBanner,
            EpisodeImage,
            Logos,
        }

        void setGUIProperty(guiProperty which, string value)
        {
            setGUIProperty(which.ToString(), value);
        }

        void setGUIProperty(string which, string value)
        {
            MediaPortal.GUI.Library.GUIPropertyManager.SetProperty("#TVSeries." + which, value);
        }

        void clearGUIProperty(guiProperty which)
        {
            setGUIProperty(which, string.Empty);
        }

        void clearGUIProperty(string which)
        {
            setGUIProperty(which, string.Empty);
        }

        enum eContextItems
        {
            toggleWatched,
            cycleSeriesBanner,
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
            actionRecheckMI
        }

        enum eContextMenus
        {
            download = 100,
            action,
            options,
        }

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

                        if (this.listLevel == Listlevel.Episode)
                        {
                            pItem = new GUIListItem(Translation.Toggle_watched_flag);
                            dlg.Add(pItem);
                            pItem.ItemId = (int)eContextItems.toggleWatched;
                        }
                        else if (this.listLevel != Listlevel.Group)
                        {
                            pItem = new GUIListItem(Translation.Mark_all_as_watched);
                            dlg.Add(pItem);
                            pItem.ItemId = (int)eContextItems.actionMarkAllWatched;

                            pItem = new GUIListItem(Translation.Mark_all_as_unwatched);
                            dlg.Add(pItem);
                            pItem.ItemId = (int)eContextItems.actionMarkAllUnwatched;
                        }

                        if (this.listLevel == Listlevel.Series)
                        {
                            pItem = new GUIListItem(Translation.Cycle_Banner);
                            dlg.Add(pItem);
                            pItem.ItemId = (int)eContextItems.cycleSeriesBanner;

                            pItem = new GUIListItem(Translation.Force_Online_Match);
                            dlg.Add(pItem);
                            pItem.ItemId = (int)eContextItems.forceSeriesQuery;
                        }

                        if (listLevel != Listlevel.Group)
                        {
                            // Fav. handling
                            DBSeries currentSeries;
                            if (listLevel == Listlevel.Series)
                                currentSeries = (DBSeries)currentitem.TVTag;
                            else currentSeries = m_SelectedSeries;

                            pItem = new GUIListItem(currentSeries[DBOnlineSeries.cIsFavourite] == 1 ? Translation.Remove_series_from_Favourites : Translation.Add_series_to_Favourites);
                            dlg.Add(pItem);
                            pItem.ItemId = (int)eContextItems.actionToggleFavorite;
                        }

                        if (this.listLevel == Listlevel.Episode)
                        {
                            pItem = new GUIListItem(Translation.Download + " >>");
                            dlg.Add(pItem);
                            pItem.ItemId = (int)eContextMenus.download;
                        }
                    }
                    else dlg.SetHeading(m_CurrLView.Name);

                    if (listLevel != Listlevel.Group)
                    {
                        pItem = new GUIListItem(Translation.Actions + " >>");
                        dlg.Add(pItem);
                        pItem.ItemId = (int)eContextMenus.action;
                    }

                    pItem = new GUIListItem(Translation.Options + " >>");
                    dlg.Add(pItem);
                    pItem.ItemId = (int)eContextMenus.options;

                    dlg.DoModal(GUIWindowManager.ActiveWindow);
                    switch (dlg.SelectedId)
                    {
                        case (int)eContextMenus.download:
                            {
                                dlg.Reset();
                                dlg.SetHeading(Translation.Download);
                                if (foromID.Length > 0)
                                {
                                    pItem = new GUIListItem(Translation.Retrieve_Subtitle);
                                    dlg.Add(pItem);
                                    pItem.ItemId = (int)eContextItems.downloadSubtitle;
                                }

                                pItem = new GUIListItem(Translation.Load_via_NewsLeecher);
                                dlg.Add(pItem);
                                pItem.ItemId = (int)eContextItems.downloadviaNewz;

                                pItem = new GUIListItem(Translation.Load_via_Torrent);
                                dlg.Add(pItem);
                                pItem.ItemId = (int)eContextItems.downloadviaTorrent;

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

                                    pItem = new GUIListItem(Translation.Delete);
                                    dlg.Add(pItem);
                                    pItem.ItemId = (int)eContextItems.actionDelete;

                                    pItem = new GUIListItem(Translation.updateMI);
                                    dlg.Add(pItem);
                                    pItem.ItemId = (int)eContextItems.actionRecheckMI;
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
                                dlg.SetHeading(Translation.Actions);

                                pItem = new GUIListItem(Translation.Only_show_episodes_with_a_local_file + " (" + (DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles) ? Translation.on : Translation.off) + ")");
                                dlg.Add(pItem);
                                pItem.ItemId = (int)eContextItems.optionsOnlyShowLocal;

                                pItem = new GUIListItem(Translation.Hide_summary_on_unwatched + " (" + (DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedSummary) ? "on" : "off") + ")");
                                dlg.Add(pItem);
                                pItem.ItemId = (int)eContextItems.optionsPreventSpoilers;

                                dlg.DoModal(GUIWindowManager.ActiveWindow);
                                if (dlg.SelectedId != -1)
                                    bExitMenu = true;
                            }
                            break;

                        default:
                            bExitMenu = true;
                            break;
                    }
                }
                while (!bExitMenu);

                if (dlg.SelectedId == -1) return;

                switch (dlg.SelectedId)
                {
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
                                    }
                                }
                                else
                                {
                                    selectedEpisode[DBOnlineEpisode.cWatched] = selectedEpisode[DBOnlineEpisode.cWatched] == 0;
                                    selectedEpisode.Commit();
                                }
                                LoadFacade();
                            }
                        }
                        break;

                    case (int)eContextItems.cycleSeriesBanner:
                        {
                            int nCurrent = selectedSeries.BannerList.IndexOf(selectedSeries.Banner);
                            nCurrent++;
                            if (nCurrent >= selectedSeries.BannerList.Count)
                                nCurrent = 0;

                            selectedSeries.Banner = selectedSeries.BannerList[nCurrent];
                            selectedSeries.Commit();
                            LoadFacade();
                        }
                        break;
                    case (int)eContextItems.actionRecheckMI:
                        switch (listLevel)
                        {
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
                            m_parserUpdaterQueue.Add(new CParsingParameters(ParsingAction.LocalScanNoExactMatch, null));
                        }
                        break;

                    case (int)eContextItems.downloadSubtitle:
                        {
                            if (selectedEpisode != null)
                            {
                                DBEpisode episode = (DBEpisode)currentitem.TVTag;
                                setProcessAnimationStatus(true);
                                foromWorking = true;
                                Forom forom = new Forom(this);
                                forom.SubtitleRetrievalCompleted += new WindowPlugins.GUITVSeries.Subtitles.Forom.SubtitleRetrievalCompletedHandler(forom_SubtitleRetrievalCompleted);
                                forom.GetSubs(selectedEpisode);
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

                    case (int)eContextItems.actionHide:
                        {
                            // hide - we can only hide things for now, no unhide
                            switch (this.listLevel)
                            {
                                case Listlevel.Series:
                                    selectedSeries[DBSeries.cHidden] = true;
                                    selectedSeries.Commit();
                                    break;

                                case Listlevel.Season:
                                    selectedSeason[DBSeason.cHidden] = true;
                                    selectedSeason.Commit();
                                    break;

                                case Listlevel.Episode:
                                    selectedEpisode[DBOnlineEpisode.cHidden] = true;
                                    selectedEpisode.Commit();
                                    break;
                            }
                            LoadFacade();
                        }
                        break;

                    case (int)eContextItems.actionMarkAllWatched:
                        // all watched
                        if (this.listLevel == Listlevel.Series && m_SelectedSeries != null)
                        {
                            DBTVSeries.Execute("update online_episodes set watched = 1 where " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID) + " = " + m_SelectedSeries[DBSeries.cID]);
                            cache.dump();
                        }
                        else if (this.listLevel == Listlevel.Season && m_SelectedSeason != null)
                        {
                            DBTVSeries.Execute("update online_episodes set watched = 1 where " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID) + " = " + m_SelectedSeason[DBSeason.cSeriesID] +
                                                " and " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeasonIndex) + " = " + m_SelectedSeason[DBSeason.cIndex]);
                            cache.dump();
                        }
                        LoadFacade(); // refresh
                        break;
                    case (int)eContextItems.actionMarkAllUnwatched:
                        // all unwatched
                        if (this.listLevel == Listlevel.Series && m_SelectedSeries != null)
                        {
                            DBTVSeries.Execute("update online_episodes set watched = 0 where " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID) + " = " + m_SelectedSeries[DBSeries.cID]);
                            cache.dump();
                        }
                        else if (this.listLevel == Listlevel.Season && m_SelectedSeason != null)
                        {
                            DBTVSeries.Execute("update online_episodes set watched = 0 where " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID) + " = " + m_SelectedSeason[DBSeason.cSeriesID] +
                                                " and " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeasonIndex) + " = " + m_SelectedSeason[DBSeason.cIndex]);
                            cache.dump();
                        }
                        LoadFacade(); // refresh
                        break;

                    case (int)eContextItems.actionDelete:
                        {
                            // delete
                            String sMsg = String.Empty;
                            switch (this.listLevel)
                            {
                                case Listlevel.Series:
                                    sMsg = Translation.Delete_that_series;
                                    break;

                                case Listlevel.Season:
                                    sMsg = Translation.Delete_that_season;
                                    break;

                                case Listlevel.Episode:
                                    sMsg = Translation.Delete_that_episode;
                                    break;
                            }
                            GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                            if (null == dlgYesNo) return;
                            dlgYesNo.SetHeading(Translation.Confirm);
                            dlgYesNo.SetLine(1, sMsg);
                            dlgYesNo.SetDefaultToYes(false);
                            dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
                            if (dlgYesNo.IsConfirmed)
                            {
                                List<DBEpisode> epsDeletion = new List<DBEpisode>();
                                switch (this.listLevel)
                                {
                                    case Listlevel.Series:
                                        {
                                            SQLCondition condition = new SQLCondition();
                                            condition.Add(new DBEpisode(), DBEpisode.cSeriesID, selectedSeries[DBSeries.cID], SQLConditionType.Equal);
                                            if (DBOption.GetOptions(DBOption.cDeleteFile)) epsDeletion.AddRange(DBEpisode.Get(condition, false));
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
                                        }
                                        break;

                                    case Listlevel.Season:
                                        {
                                            SQLCondition condition = new SQLCondition();
                                            condition.Add(new DBEpisode(), DBEpisode.cSeriesID, selectedSeason[DBSeason.cSeriesID], SQLConditionType.Equal);
                                            condition.Add(new DBEpisode(), DBEpisode.cSeasonIndex, selectedSeason[DBSeason.cIndex], SQLConditionType.Equal);
                                            if (DBOption.GetOptions(DBOption.cDeleteFile)) epsDeletion.AddRange(DBEpisode.Get(condition, false));
                                            DBEpisode.Clear(condition);
                                            condition = new SQLCondition();
                                            condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, selectedSeason[DBSeason.cSeriesID], SQLConditionType.Equal);
                                            condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeasonIndex, selectedSeason[DBSeason.cIndex], SQLConditionType.Equal);
                                            DBOnlineEpisode.Clear(condition);

                                            condition = new SQLCondition();
                                            condition.Add(new DBSeason(), DBSeason.cID, selectedSeason[DBSeason.cID], SQLConditionType.Equal);
                                            DBSeason.Clear(condition);
                                        }
                                        break;

                                    case Listlevel.Episode:
                                        {
                                            SQLCondition condition = new SQLCondition();
                                            condition.Add(new DBEpisode(), DBEpisode.cFilename, selectedEpisode[DBEpisode.cFilename], SQLConditionType.Equal);
                                            if (DBOption.GetOptions(DBOption.cDeleteFile)) epsDeletion.AddRange(DBEpisode.Get(condition, false));
                                            condition = new SQLCondition();
                                            condition.Add(new DBEpisode(), DBEpisode.cFilename, selectedEpisode[DBEpisode.cFilename], SQLConditionType.Equal);
                                            DBEpisode.Clear(condition);
                                            condition = new SQLCondition();
                                            condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cID, selectedEpisode[DBOnlineEpisode.cID], SQLConditionType.Equal);
                                            DBOnlineEpisode.Clear(condition);
                                        }
                                        break;
                                }
                                if (epsDeletion.Count > 0)// && DBOption.GetOptions(DBOption.cDeleteFile))
                                {
                                    // delete the actual files!!

                                    //dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                                    //if (null == dlgYesNo) return;
                                    //dlgYesNo.SetHeading(Translation.Confirm);
                                    //dlgYesNo.SetLine(1, String.Format(Translation.delPhyiscalWarning, epsDeletion.Count));
                                    //dlgYesNo.SetDefaultToYes(false);
                                    //dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
                                    List<string> files = Helper.getFieldNameListFromList<DBEpisode>(DBEpisode.cFilename, epsDeletion);
                                    if (dlgYesNo.IsConfirmed)
                                    {
                                        foreach (string file in files)
                                        {
                                            System.IO.File.Delete(file);
                                        }
                                    }
                                }
                                LoadFacade();
                            }
                        }
                        break;

                    case (int)eContextItems.actionToggleFavorite:
                        {
                            // toggle Favourites
                            m_SelectedSeries.toggleFavourite();
                            // if we are in fav view we have to reload to get the series away
                            LoadFacade();
                            break;
                        }

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

                    case (int)eContextItems.actionPlayRandom:
                        playRandomEp();
                        break;

                    case (int)eContextItems.optionsOnlyShowLocal:
                        DBOption.SetOptions(DBOption.cView_Episode_OnlyShowLocalFiles, !DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles));
                        LoadFacade();
                        break;

                    case (int)eContextItems.optionsPreventSpoilers:
                        DBOption.SetOptions(DBOption.cView_Episode_HideUnwatchedSummary, !DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedSummary));
                        LoadFacade();
                        break;
                }
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("The 'OnShowContextMenu' function has generated an error: " + ex.Message);
            }

        }

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

        void forom_SubtitleRetrievalCompleted(bool bFound)
        {
            setProcessAnimationStatus(false);
            foromWorking = false;
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
        void setViewLabels()
        {
            try
            {
                setGUIProperty(guiProperty.CurrentView, m_CurrLView.prettyName);
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
            MPTVSeriesLog.Write("Switching logical view to " + view.Name);
            m_CurrLView = view;

            // set the skin labels
            setViewLabels();

            if (fanartSet) loadFanart(null);

            m_CurrViewStep = 0; // we always start out at step 0
            m_stepSelection = null;
            m_stepSelections = new List<string[]>();
            m_stepSelections.Add(new string[] { null });
            setNewListLevelOfCurrView(0);

            DBOption.SetOptions("lastView", view.Name); // to remember next time the plugin is entered
        }

        void setNewListLevelOfCurrView(int step)
        {
            switch (m_CurrLView.gettypeOfStep(step))
            {
                case logicalViewStep.type.group:
                    listLevel = Listlevel.Group;
                    break;
                case logicalViewStep.type.series:
                    listLevel = Listlevel.Series;
                    break;
                case logicalViewStep.type.season:
                    listLevel = Listlevel.Season;
                    break;
                case logicalViewStep.type.episode:
                    listLevel = Listlevel.Episode;
                    break;
            }
            MPTVSeriesLog.Write("new listlevel: " + listLevel.ToString());
        }

        int thumbnail_last_selected = 0;
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
                        m_CurrViewStep--;
                        setNewListLevelOfCurrView(m_CurrViewStep);
                        m_back_up_select_this = m_stepSelection;
                        m_stepSelection = m_stepSelections[m_CurrViewStep];
                        skipSeasonIfOne_DirectionDown = false; // otherwise the user cant get back out
                        LoadFacade();
                        if (this.listLevel == Listlevel.Series) loadFanart(null);
                        else if (this.listLevel == Listlevel.Season) loadFanart(m_SelectedSeries);
                        skipSeasonIfOne_DirectionDown = true;
                    }
                    break;
                case Action.ActionType.ACTION_MOVE_LEFT:
                    if (this.m_Facade.View == GUIFacadeControl.ViewMode.LargeIcons)
                    {
                        thumbnail_last_selected = m_Facade.SelectedListItemIndex;
                        base.OnAction(action);
                        if (thumbnail_last_selected == m_Facade.SelectedListItemIndex)
                        {
                            switchView(-1);
                            LoadFacade();
                        }
                        thumbnail_last_selected = m_Facade.SelectedListItemIndex;
                    }
                    else
                    {
                        switchView(-1);
                        LoadFacade();
                    }
                    
                    break;
                case Action.ActionType.ACTION_MOVE_RIGHT:
                    if (this.m_Facade.View == GUIFacadeControl.ViewMode.LargeIcons)
                    {
                        thumbnail_last_selected = m_Facade.SelectedListItemIndex;
                        base.OnAction(action);
                        if (thumbnail_last_selected == m_Facade.SelectedListItemIndex)
                        {
                            switchView(1);
                            LoadFacade();
                            //m_Facade.sele
                            OnAction(new Action(Action.ActionType.ACTION_MOVE_DOWN, 0, 0));
                            OnAction(new Action(Action.ActionType.ACTION_MOVE_DOWN, 0, 0));
                            //GUIControl.FocusItemControl(this.GetID, m_Facade.GetID, m_Facade.SelectedListItemIndex);
                        }
                        thumbnail_last_selected = m_Facade.SelectedListItemIndex;
                    }
                    else
                    {
                        switchView(1);
                        LoadFacade();
                    }
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
            try
            {
                if (FanartBackground == null) fanartSet = false;
                else
                {
                    FanartBackground.Visible = true; // always visible, no more triggered animations (sorry)
                    if (item == null)
                    {
                        MPTVSeriesLog.Write("Fanart: resetting to normal", MPTVSeriesLog.LogLevel.Normal);
                        Fanart.FlushTextures();
                        currSeriesFanart = null;
                        //FanartBackground.Visible = false;
                        FanartBackground.SetFileName(string.Empty);
                        if (this.dummyIsFanartLoaded != null)
                            this.dummyIsFanartLoaded.Visible = false;
                        if (this.dummyIsLightFanartLoaded != null)
                            this.dummyIsLightFanartLoaded.Visible = false;
                        if (this.dummyIsDarkFanartLoaded != null)
                            this.dummyIsDarkFanartLoaded.Visible = false;
                        fanartSet = false;
                    }
                    else
                    {
                        Fanart f = currSeriesFanart;
                        DBSeries s = item as DBSeries;
                        if (s != null)
                        {
                            if (f == null || f.SeriesID != s[DBSeries.cID])
                            {
                                f = Fanart.getFanart(s[DBSeries.cID]);
                                f.ForceNewPick(); // k, lets get more random then
                                if (f != null)
                                {
                                    f.FlushTexture();
                                }
                            }
                            // else we came back from season, we want the same fanart again

                           currSeriesFanart = f;
                        }
                        else
                        {
                            DBSeason se = item as DBSeason;
                            if (se != null)
                                f = Fanart.getFanart(se[DBSeason.cSeriesID], se[DBSeason.cIndex]);
                        }
                        if (f != null && f.Found)
                        {
                            MPTVSeriesLog.Write("Fanart: found, loading: ", f.RandomFanart, MPTVSeriesLog.LogLevel.Normal);
                            FanartBackground.SetFileName(f.RandomFanartAsTexture);
                            //FanartBackground.Visible = true;
                            if (this.dummyIsLightFanartLoaded != null)
                                this.dummyIsLightFanartLoaded.Visible = f.RandomPickIsLight;
                            if (this.dummyIsDarkFanartLoaded != null)
                                this.dummyIsDarkFanartLoaded.Visible = !f.RandomPickIsLight;
                            fanartSet = true;

                        }
                        else if(f != null && !f.SeasonMode) loadFanart(null);

                    }
                }
                if (this.dummyIsFanartLoaded != null)
                    this.dummyIsFanartLoaded.Visible = fanartSet;
                return fanartSet;
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Fanart: Problem encountered: " + ex.Message, MPTVSeriesLog.LogLevel.Normal);
                return false;
            }
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType)
        {
            if (actionType != Action.ActionType.ACTION_SELECT_ITEM) return; // some other events raised onClicked too for some reason?
            if (control == this.m_Facade)
            {
                if (this.m_Facade.SelectedListItem.TVTag == null)
                    return;
                m_back_up_select_this = null;
                switch (this.listLevel)
                {
                    case Listlevel.Group:
                        this.m_CurrViewStep++;
                        setNewListLevelOfCurrView(m_CurrViewStep);
                        m_stepSelection = new string[] { this.m_Facade.SelectedListItem.Label };
                        m_stepSelections.Add(m_stepSelection);
                        MPTVSeriesLog.Write("Selected: ", this.m_Facade.SelectedListItem.Label, MPTVSeriesLog.LogLevel.Normal);
                        LoadFacade();
                        this.m_Facade.Focus = true;
                        break;
                    case Listlevel.Series:
                        this.m_CurrViewStep++;
                        setNewListLevelOfCurrView(m_CurrViewStep);
                        this.m_SelectedSeries = (DBSeries)this.m_Facade.SelectedListItem.TVTag;
                        m_stepSelection = new string[] { m_SelectedSeries[DBSeries.cID].ToString() };
                        m_stepSelections.Add(m_stepSelection);
                        MPTVSeriesLog.Write("Selected: ", m_stepSelection[0], MPTVSeriesLog.LogLevel.Normal);
                        MPTVSeriesLog.Write("Fanart: Series selected");
                        this.loadFanart(m_SelectedSeries);
                        this.LoadFacade();
                        this.m_Facade.Focus = true;
                        
                        break;
                    case Listlevel.Season:
                        this.m_CurrViewStep++;
                        setNewListLevelOfCurrView(m_CurrViewStep);
                        this.m_SelectedSeason = (DBSeason)this.m_Facade.SelectedListItem.TVTag;
                        m_stepSelection = new string[] { m_SelectedSeason[DBSeason.cSeriesID].ToString(), m_SelectedSeason[DBSeason.cIndex].ToString() };
                        m_stepSelections.Add(m_stepSelection);
                        MPTVSeriesLog.Write("Selected: ", m_stepSelection[0] + " - " + m_stepSelection[1], MPTVSeriesLog.LogLevel.Normal);
                        this.LoadFacade();
                        MPTVSeriesLog.Write("Fanart: Season selected");
                        this.loadFanart(m_SelectedSeason);
                        this.m_Facade.Focus = true;
                        break;
                    case Listlevel.Episode:
                        this.m_SelectedEpisode = (DBEpisode)this.m_Facade.SelectedListItem.TVTag;
                        MPTVSeriesLog.Write("Selected: ", this.m_SelectedEpisode[DBEpisode.cCompositeID].ToString(), MPTVSeriesLog.LogLevel.Normal);
                        /*if (*/
                        m_VideoHandler.ResumeOrPlay(m_SelectedEpisode);//)
                        //{
                            // AB: I put back this code as it was before, as if I watch one local episode I think it's safe to consider all local episodes watched

                            //-- Jon: isWatched check now happens on stopping (VideoHandler.OnPlayBackStopped)
                            /* 
                             * SQLCondition condition = new SQLCondition();
                             * condition.Add(new DBEpisode(), DBEpisode.cFilename, this.m_SelectedEpisode[DBEpisode.cFilename], SQLConditionType.Equal);
                             * List<DBEpisode> episodes = DBEpisode.Get(condition, false);
                             * foreach (DBEpisode episode in episodes)
                             * {
                             *     episode[DBOnlineEpisode.cWatched] = 1;
                             *     episode.Commit();
                             * }
                             * this.LoadFacade();
                             * this.OnPageLoad();
                             */
                        //}
                        break;
                }
            }
            base.OnClicked(controlId, control, actionType);
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
                    else if (!foromWorking && !torrentWorking)
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
                LoadFacade();
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

            // let's try to give the user a bit more information
            string groupedBy = m_CurrLView.groupedInfo(m_CurrViewStep);
            if (groupedBy.Contains("<Ser"))
            {
                int count = 0;
                string seriesNames = string.Empty;
                SQLCondition cond = new SQLCondition();
                cond.AddOrderItem(DBOnlineSeries.Q(DBOnlineSeries.cPrettyName), SQLCondition.orderType.Ascending);
                cond.SetLimit(20);
                if (m_CurrLView.steps[m_CurrViewStep].groupedBy.attempSplit && this.m_Facade.SelectedListItem.Label.ToString() != Translation.Unknown)
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
            clearGUIProperty(guiProperty.SeriesBanner);
            clearGUIProperty(guiProperty.SeasonBanner);

            clearFieldsForskin("Episode");
            clearFieldsForskin("Season");
            clearFieldsForskin("Series");

        }

        private void Series_OnItemSelected(GUIListItem item)
        {
            m_SelectedSeason = null;
            m_SelectedEpisode = null;
            if (item == null || item.TVTag == null || !(item.TVTag is DBSeries))
                return;
            DBSeries series = (DBSeries)item.TVTag;
            item.Selected = true;
            m_SelectedSeries = series;
            clearGUIProperty(guiProperty.EpisodeImage);
            clearGUIProperty(guiProperty.SeasonBanner);

            setGUIProperty(guiProperty.Title, FieldGetter.resolveDynString(m_sFormatSeriesTitle, series));
            setGUIProperty(guiProperty.Subtitle, FieldGetter.resolveDynString(m_sFormatSeriesSubtitle, series));
            setGUIProperty(guiProperty.Description, FieldGetter.resolveDynString(m_sFormatSeriesMain, series));

            setGUIProperty(guiProperty.SeriesBanner, ImageAllocator.GetSeriesBanner(series));
            setGUIProperty(guiProperty.Logos, localLogos.getLogos(ref series, logosHeight, logosWidth));

            pushFieldsToSkin(m_SelectedSeries, "Series");
            pushFieldsToSkin(m_SelectedSeries.onlineSeries, "Series");

            clearFieldsForskin("Episode");
            clearFieldsForskin("Season");

        }

        private void Season_OnItemSelected(GUIListItem item)
        {
            m_SelectedEpisode = null;
            if (item == null || item.TVTag == null)
                return;
            DBSeason season = (DBSeason)item.TVTag;
            m_SelectedSeason = season;

            setGUIProperty(guiProperty.Title, FieldGetter.resolveDynString(m_sFormatSeasonTitle, season));
            setGUIProperty(guiProperty.Subtitle, FieldGetter.resolveDynString(m_sFormatSeasonSubtitle, season));
            setGUIProperty(guiProperty.Description, FieldGetter.resolveDynString(m_sFormatSeasonMain, season));

            setGUIProperty(guiProperty.SeasonBanner, ImageAllocator.GetSeasonBanner(season, false));
            
            setGUIProperty(guiProperty.Logos, localLogos.getLogos(ref season, logosHeight, logosWidth));

            clearGUIProperty(guiProperty.EpisodeImage);

            if (!m_CurrLView.stepHasSeriesBeforeIt(m_CurrViewStep))
            {
                // it is the case
                m_SelectedSeries = Helper.getCorrespondingSeries(season[DBSeason.cSeriesID]);
                if (m_SelectedSeries != null)
                    setGUIProperty(guiProperty.SeriesBanner, ImageAllocator.GetSeriesBanner(m_SelectedSeries));
                else clearGUIProperty(guiProperty.SeriesBanner);
            }

            pushFieldsToSkin(m_SelectedSeason, "Season");
            clearFieldsForskin("Episode");
        }
        private void Episode_OnItemSelected(GUIListItem item)
        {
            if (item == null || item.TVTag == null)
                return;
            DBEpisode episode = (DBEpisode)item.TVTag;
            this.m_SelectedEpisode = episode;
            setGUIProperty(guiProperty.Logos, localLogos.getLogos(ref episode, logosHeight, logosWidth));

            if (!localLogos.appendEpImage)
                setGUIProperty(guiProperty.EpisodeImage, episode.Image);
            else clearGUIProperty(guiProperty.EpisodeImage);

            setGUIProperty(guiProperty.Title, FieldGetter.resolveDynString(m_sFormatEpisodeTitle, episode));
            setGUIProperty(guiProperty.Subtitle, FieldGetter.resolveDynString(m_sFormatEpisodeSubtitle, episode));
            setGUIProperty(guiProperty.Description, FieldGetter.resolveDynString(m_sFormatEpisodeMain, episode));

            // with groups in episode view its possible the user never selected a series/season (flat view)
            // thus its desirable to display the series_banner and season banner on hover)
            if (!m_CurrLView.stepHasSeriesBeforeIt(m_CurrViewStep))
            {
                // it is the case
                m_SelectedSeason = Helper.getCorrespondingSeason(episode[DBEpisode.cSeriesID], episode[DBEpisode.cSeasonIndex]);
                m_SelectedSeries = Helper.getCorrespondingSeries(episode[DBEpisode.cSeriesID]);

                if (m_SelectedSeries != null)
                    setGUIProperty(guiProperty.SeriesBanner, ImageAllocator.GetSeriesBanner(m_SelectedSeries));
                else clearGUIProperty(guiProperty.SeriesBanner);
                if (m_SelectedSeason != null)
                    setGUIProperty(guiProperty.SeasonBanner, ImageAllocator.GetSeasonBanner(m_SelectedSeason, false));
                else clearGUIProperty(guiProperty.SeasonBanner);
            }
            pushFieldsToSkin(m_SelectedEpisode, "Episode");
            pushFieldsToSkin(m_SelectedEpisode.onlineEpisode, "Episode");
        }

        public ReturnCode ChooseFromSelection(CDescriptor descriptor, out CItem selected)
        {
            try
            {
                while (this.m_Facade == null) Thread.Sleep(2000); // don't show anything if we're not inside the plugin window

                GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                selected = null;
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
                        selected = new CItem(descriptor.m_sItemToMatch, String.Empty, DlgSelected.m_Tag);
                        return ReturnCode.OK;
                    }
                    else return ReturnCode.Cancel;
                }
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("The ChooseFromSelection Method has generated an error: " + ex.Message);
                selected = null;
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

            MPTVSeriesLog.Write("Randomly Selected: ", selectedEpisode[DBEpisode.cCompositeID].ToString(), MPTVSeriesLog.LogLevel.Normal);

            // removed the if statement here to mimic functionality when an episode is selected
            // via the regular UI, since watched flag is now set after viewing (is this right?)
            m_VideoHandler.ResumeOrPlay(selectedEpisode);
        }

        private void setProcessAnimationStatus(bool enable)
        {
            //MPTVSeriesLog.Write("Set Animation: ", enable.ToString(), MPTVSeriesLog.LogLevel.Normal);
            try
            {
                if (m_ImportAnimation != null)
                {
                    if (enable)
                        m_ImportAnimation.AllocResources();
                    else
                        m_ImportAnimation.FreeResources();
                    m_ImportAnimation.Visible = enable;
                    //MPTVSeriesLog.Write("Set Animation: ", "Done", MPTVSeriesLog.LogLevel.Normal);
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

        Dictionary<string, List<string>> _allFieldsForSkin = new Dictionary<string, List<string>>();
        private void pushFieldsToSkin(DBTable item, string pre)
        {
            if(item == null) return;
            foreach (KeyValuePair<string, DBField> kv in item.m_fields)
            {
                List<string> l = null;
                if (_allFieldsForSkin.ContainsKey(pre)) l = _allFieldsForSkin[pre];
                else
                {
                    l = new List<string>();
                    _allFieldsForSkin.Add(pre, l);
                }
                if(l!= null && !l.Contains(kv.Key)) l.Add(kv.Key);

                setGUIProperty(pre + "." + kv.Key, kv.Value.Value);
            }
        }
        private void clearFieldsForskin(string pre)
        {
            if (_allFieldsForSkin.ContainsKey(pre))
            {
                List<string> fields = _allFieldsForSkin[pre];
                foreach (string field in fields)
                    clearGUIProperty(field);
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
}
