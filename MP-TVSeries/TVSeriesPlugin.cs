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
using WindowPlugins.GUITVSeries.Torrent;

namespace MediaPortal.GUI.Video
{
    public class TVSeriesPlugin : GUIWindow, ISetupForm, Interface
    {
        public TVSeriesPlugin()
        {
            m_stepSelections.Add(new string[] { null });
            // disable that dynamic skin adjustment....skinners should have the power to position the elements whereever with the plugin inerveining
            if (DBOption.GetOptions(DBOption.cViewAutoHeight)) DBOption.SetOptions(DBOption.cViewAutoHeight, false);
            try
            {
                if(!DBOption.GetOptions(DBOption.cUsesNewPathFormat))
                {
                    PathMigration.migrateDB(); // needs to be first
                    PathMigration.migrateBanners();
                }
                
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("A general error occured migrating to the new locations: " + ex.Message);
            }

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
            strButtonText = DBOption.GetOptions(DBOption.cView_PluginName);
            strButtonImage = String.Empty;
            strButtonImageFocus = String.Empty;
            strPictureImage = "hover_my tv series.png";
            return true;
        }

        #endregion

        private const String cListLevelSeries = "Series";
        private const String cListLevelSeasons = "Seasons";
        private const String cListLevelEpisodes = "Episodes";
        private const String cListLevelGroup = "Group";
        private String m_ListLevel = cListLevelSeries;
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

        private TimerCallback timerDelegate = null;
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
        
#region Skin Variables
        [SkinControlAttribute(2)]
        protected GUIButtonControl m_Button_Back = null;

        [SkinControlAttribute(3)]
        protected GUIButtonControl m_Button_View = null;

        [SkinControlAttribute(50)]
        protected GUIFacadeControl m_Facade = null;

        [SkinControlAttribute(51)]
        protected GUIAnimation m_ImportAnimation = null;

        [SkinControlAttribute(30)]
        protected GUIImage m_Image = null;

        [SkinControlAttribute(31)]
        protected GUITextScrollUpControl m_Description = null;

        [SkinControlAttribute(32)]
        protected GUITextScrollUpControl m_Series_Name = null;

        [SkinControlAttribute(33)]
        protected GUITextScrollUpControl m_Genre = null;

        [SkinControlAttribute(34)]
        protected GUITextControl m_Series_Network = null;

        [SkinControlAttribute(35)]
        protected GUITextControl m_Series_Duration = null;

        [SkinControlAttribute(36)]
        protected GUITextControl m_Series_Status = null;

        [SkinControlAttribute(37)]
        protected GUITextControl m_Series_Premiered = null;

        [SkinControlAttribute(40)]
        protected GUITextScrollUpControl m_Title = null;

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

        [SkinControlAttribute(46)]
        protected GUIImage m_Season_Image = null;

        [SkinControlAttribute(66)]
        protected GUIImage m_Logos_Image = null;

        [SkinControlAttribute(67)]
        protected GUIImage m_Episode_Image = null;

        [SkinControlAttribute(77)]
        protected GUILabelControl view_curr = null;

        [SkinControlAttribute(78)]
        protected GUILabelControl view_prev = null;

        [SkinControlAttribute(79)]
        protected GUILabelControl view_next = null;

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
            MPTVSeriesLog.Write("**** Plugin started in MediaPortal ***");
            String xmlSkin = GUIGraphicsContext.Skin + @"\TVSeries.xml";
            MPTVSeriesLog.Write("Loading XML Skin: " + xmlSkin);

            m_VideoHandler = new VideoHandler();

            m_parserUpdater = new OnlineParsing(this);
            m_parserUpdater.OnlineParsingCompleted += new OnlineParsing.OnlineParsingCompletedHandler(parserUpdater_OnlineParsingCompleted);

            if (DBOption.GetOptions("doFolderWatch"))
            {
                m_watcherUpdater = new Watcher(this);
                m_watcherUpdater.WatcherProgress += new Watcher.WatcherProgressHandler(watcherUpdater_WatcherProgress);
                m_watcherUpdater.StartFolderWatch();

                // always do a local scan when starting up the app - later on the watcher will monitor changes
                m_parserUpdaterQueue.Add(new CParsingParameters(true, false));
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
            catch {}

            if (DBOption.GetOptions(DBOption.cAutoUpdateOnlineData))
                m_nUpdateScanLapse = DBOption.GetOptions(DBOption.cAutoUpdateOnlineDataLapse);

            // timer check every seconds
            timerDelegate = new TimerCallback(Clock);
            m_scanTimer = new System.Threading.Timer(timerDelegate, null, 1500, 1500);
            return Load(xmlSkin);
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

        String FormatField(String sFormat, DBTable table)
        {
            String sOut = String.Empty;

            while (sFormat.Length != 0)
            {
                int nTagStart = sFormat.IndexOf('<');
                if (nTagStart != -1)
                {
                    sOut += sFormat.Substring(0, nTagStart);
                    sFormat = sFormat.Substring(nTagStart);
                    
                    int nTagEnd = sFormat.IndexOf('>');
                    if (nTagEnd != -1)
                    {
                        String sTag = sFormat.Substring(1, nTagEnd - 1);
                        sFormat = sFormat.Substring(nTagEnd + 1);

                        if (sTag.IndexOf('.') != -1)
                        {
                            String sTableName = sTag.Substring(0, sTag.IndexOf('.'));
                            String sFieldName = sTag.Substring(sTag.IndexOf('.') + 1);

                            switch (sTableName)
                            {
                                case DBSeries.cOutName:
                                    {
                                        DBSeries source = table as DBSeries;
                                        if (source == null)
                                            source = m_SelectedSeries;
                                        if (source != null)
                                        {
                                            switch (sFieldName)
                                            {
                                                case DBOnlineSeries.cActors:
                                                case DBOnlineSeries.cGenre:
                                                    sOut += ((String)source[sFieldName]).Trim('|').Replace("|", ", ");
                                                    break;

                                                default:
                                                    sOut += source[sFieldName];
                                                    break;
                                            }
                                        }
                                    }
                                    break;

                                case DBSeason.cOutName:
                                    {
                                        DBSeason source = table as DBSeason;
                                        if (source == null)
                                            source = m_SelectedSeason;
                                        if (source != null)
                                        {
                                            sOut += source[sFieldName];
                                        }
                                    }
                                    break;

                                case DBEpisode.cOutName:
                                    {
                                        DBEpisode source = table as DBEpisode;
                                        if (source == null)
                                            source = m_SelectedEpisode;
                                        if (source != null)
                                        {
                                            switch (sFieldName)
                                            {
                                                case DBOnlineEpisode.cEpisodeSummary:
                                                    if (DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedSummary) != true || table[DBOnlineEpisode.cWatched] != 0)
                                                        sOut += source[sFieldName];
                                                    else
                                                        sOut += Translation._Hidden_to_prevent_spoilers_;
                                                    break;

                                                case DBOnlineEpisode.cWatched:
                                                    sOut += source[sFieldName] == 0 ? Translation.No : Translation.Yes;
                                                    break;

                                                case DBOnlineEpisode.cGuestStars:
                                                case DBOnlineEpisode.cDirector:
                                                case DBOnlineEpisode.cWriter:
                                                    sOut += ((String)source[sFieldName]).Trim('|').Replace("|", ", ");
                                                    break;

                                                default:
                                                    sOut += source[sFieldName];
                                                    break;
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        sOut += Translation.Error;
                        sFormat = String.Empty;
                    }
                }
                else
                {
                    // no more opening tag
                    sOut += sFormat;
                    sFormat = String.Empty;
                }
            }

            String sCR = "" + (char)10 + (char)13;
            sOut = sOut.Replace("\\n", sCR);
            return sOut;
        }

        String GetSeriesBanner(DBSeries series)
        {
            String filename = series.Banner;
            if (filename != String.Empty)
            {
                return filename;
            }
            else
            {
                // no image, use text, create our own
                if (GUITextureManager.LoadFromMemory(null, "[series_" + series[DBSeries.cID] + "]", 0, 0, 0) == 0)
                {
                    Size sizeImage = new Size(758, 140);
                    Bitmap image = new Bitmap(sizeImage.Width, sizeImage.Height);
                    Graphics gph = Graphics.FromImage(image);
                    gph.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.White)), new Rectangle(0, 0, sizeImage.Width, sizeImage.Height));
                    GUIFont fontList = GUIFontManager.GetFont(m_Facade.AlbumListView.FontName);
                    Font font = new Font(fontList.FontName, 36);
                    gph.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    gph.DrawString(series[DBOnlineSeries.cPrettyName], font, new SolidBrush(Color.FromArgb(200, Color.White)), 5, (sizeImage.Height - font.GetHeight()) / 2);
                    gph.Dispose();
                    GUITextureManager.LoadFromMemory(image, "[series_" + series[DBSeries.cID] + "]", 0, sizeImage.Width, sizeImage.Height);
                }
                return "[series_" + series[DBSeries.cID] + "]";
            }
        }

        String GetSeasonBanner(DBSeason season)
        {
            String filename = season.Banner;
            if (filename != String.Empty)
            {
                return filename;
            }
            else
            {
                if (GUITextureManager.LoadFromMemory(null, "[" + season[DBSeason.cID] + "]", 0, 0, 0) == 0)
                {
                    // no image, use text, create our own
                    Size sizeImage = new Size(400, 578);
                    Bitmap image = new Bitmap(sizeImage.Width, sizeImage.Height);
                    Graphics gph = Graphics.FromImage(image);
                    gph.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.White)), new Rectangle(0, 0, sizeImage.Width, sizeImage.Height));
                    GUIFont fontList = GUIFontManager.GetFont(m_Facade.AlbumListView.FontName);
                    Font font = new Font(fontList.FontName, 48);
                    gph.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    gph.DrawString(Translation.Season + season[DBSeason.cIndex], font, new SolidBrush(Color.FromArgb(200, Color.White)), 5, (sizeImage.Height - font.GetHeight()) / 2);
                    gph.Dispose();
                    GUITextureManager.LoadFromMemory(image, "[" + season[DBSeason.cID] + "]", 0, sizeImage.Width, sizeImage.Height);
                }
                return "[" + season[DBSeason.cID] + "]";
            }
        }

        void LoadFacade()
        {
            MPTVSeriesLog.Write("Begin LoadFacade");
            try
            {
                if (this.m_Facade != null)
                {
                    m_Button_View.Visible = false;
                    m_Button_Back.Visible = false;

                    if (m_nInitialIconXOffset == 0)
                        m_nInitialIconXOffset = m_Facade.AlbumListView.IconOffsetX;
                    if (m_nInitialIconYOffset == 0)
                        m_nInitialIconYOffset = m_Facade.AlbumListView.IconOffsetY;
                    if (m_nInitialItemHeight == 0)
                        m_nInitialItemHeight = m_Facade.AlbumListView.ItemHeight;

                    this.m_Facade.ListView.Clear();
                    this.m_Facade.AlbumListView.Clear();
                    bool bEmpty = true;
                    MPTVSeriesLog.Write("LoadFacade: ListLevel: ", m_ListLevel, MPTVSeriesLog.LogLevel.Normal);
                    switch (this.m_ListLevel)
                    {
                        #region Group
                        case cListLevelGroup:
                            {
                                // these are groups of certain categories, eg. Genres

                                // always list mode
                                this.m_Facade.View = GUIFacadeControl.ViewMode.List;
                                int selectedIndex = -1;
                                // view handling
                                List<string> items = m_CurrLView.getGroupItems(m_CurrViewStep, m_stepSelection);

                                for (int index = 0; index < items.Count; index++)
                                {
                                    bEmpty = false;
                                    GUIListItem item = new GUIListItem(items[index]);
                                    if (item.Label == string.Empty) item.Label = Translation.Unknown;
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
                        case cListLevelSeries:
                            {
                                int nSeriesDisplayMode = DBOption.GetOptions(DBOption.cView_Series_ListFormat);
                                int selectedIndex = -1;
                                int count = 0;

                                if (nSeriesDisplayMode == 1)
                                {
                                    // graphical
                                    this.m_Facade.View = GUIFacadeControl.ViewMode.AlbumView;
                                    // assume 758 x 140 for all banners
                                    Size sizeImage = new Size();

                                    m_Facade.AlbumListView.IconOffsetX = m_nInitialIconXOffset;
                                    sizeImage.Width = m_Facade.AlbumListView.Width - 2 * m_Facade.AlbumListView.IconOffsetX;
                                    sizeImage.Height = sizeImage.Width * 140 / 758;
                                    m_Facade.AlbumListView.IconOffsetY = m_nInitialIconYOffset * sizeImage.Height / m_nInitialItemHeight;
                                    m_Facade.AlbumListView.ItemHeight = sizeImage.Height + 2 * m_Facade.AlbumListView.IconOffsetY;
                                    m_Facade.AlbumListView.SetImageDimensions(sizeImage.Width, sizeImage.Height);
                                    m_Facade.AlbumListView.AllocResources();
                                    m_Image.Visible = false;
                                }
                                else
                                {
                                    // text as usual
                                    this.m_Facade.View = GUIFacadeControl.ViewMode.List;
                                    m_Image.Visible = true;
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
                                            item.IconImage = item.IconImageBig = GetSeriesBanner(series);
                                        }
                                        else
                                        {
                                            item = new GUIListItem(FormatField(m_sFormatSeriesCol2, series));
                                            item.Label2 = FormatField(m_sFormatSeriesCol3, series);
                                            item.Label3 = FormatField(m_sFormatSeriesCol1, series);
                                        }
                                        item.TVTag = series;
                                        item.IsRemote = series[DBOnlineSeries.cHasLocalFiles] != 0;
                                        item.IsDownloading = true;

                                        if (this.m_SelectedSeries != null)
                                        {
                                            if (series[DBSeries.cID] == this.m_SelectedSeries[DBSeries.cID])
                                                selectedIndex = count;
                                        }
                                        else
                                        {
                                            // select the first that has a file
                                            if (selectedIndex == -1 && series[DBOnlineSeries.cHasLocalFiles] != 0)
                                                selectedIndex = count;
                                        }
                                        if (m_back_up_select_this != null && series != null && selectedIndex == -1 && series[DBSeries.cID] == m_back_up_select_this[0])
                                            selectedIndex = count;

                                        this.m_Facade.Add(item);
                                    }
                                    catch (Exception ex)
                                    {
                                        MPTVSeriesLog.Write("The 'LoadFacade' function has generated an error displaying series list item: " + ex.Message);
                                    }
                                    count++;
                                }
                                if (selectedIndex != -1)
                                    this.m_Facade.SelectedListItemIndex = selectedIndex;
                            }
                            break;
#endregion
                        #region Season
                        case cListLevelSeasons:
                            {
                                m_Image.Visible = true;
                                int selectedIndex = -1;
                                int count = 0;
                                int nSeasonDisplayMode = DBOption.GetOptions(DBOption.cView_Season_ListFormat);
                                if (nSeasonDisplayMode == 1)
                                {
                                    this.m_Facade.View = GUIFacadeControl.ViewMode.AlbumView;
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
                                    m_Season_Image.Visible = false;
                                }
                                else
                                {
                                    this.m_Facade.View = GUIFacadeControl.ViewMode.List;
                                    m_Season_Image.Visible = true;
                                }

                                if (m_SelectedSeries != null && this.m_Image != null)
                                {
                                    try
                                    {
                                        this.m_Image.SetFileName(GetSeriesBanner(m_SelectedSeries));
                                        this.m_Image.KeepAspectRatio = true;
                                    }
                                    catch { }
                                }

                                // view handling
                                List<DBSeason> seasons = m_CurrLView.getSeasonItems(m_CurrViewStep, m_stepSelection);
                                MPTVSeriesLog.Write("LoadFacade: BeginDisplayLoopSeason: ", seasons.Count.ToString(), MPTVSeriesLog.LogLevel.Normal);
                                foreach (DBSeason season in seasons)
                                {
                                    try
                                    {
                                        bEmpty = false;
                                        GUIListItem item = null;
                                        if (nSeasonDisplayMode == 1)
                                        {
                                            item = new GUIListItem();
                                            item.IconImage = item.IconImageBig = GetSeasonBanner(season);
                                        }
                                        else
                                        {
                                            item = new GUIListItem(FormatField(m_sFormatSeasonCol2, season));
                                            if (!m_CurrLView.stepHasSeriesBeforeIt(m_CurrViewStep))
                                                // somehow the seriesname should be displayed too I guess, but this is more important in the episodes view
                                                item.Label2 = FormatField(m_sFormatSeasonCol3, season);
                                            item.Label3 = FormatField(m_sFormatSeasonCol1, season);
                                        }
                                        item.IsRemote = season[DBSeason.cHasLocalFiles] != 0;
                                        item.IsDownloading = true;
                                        item.TVTag = season;

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
                        case cListLevelEpisodes:
                            {
                                m_Season_Image.Visible = true;
                                int selectedIndex = -1;
                                //m_SelectedEpisode = null;
                                int count = 0;
                                this.m_Facade.View = GUIFacadeControl.ViewMode.List;

                                if (m_SelectedSeries != null && this.m_Image != null)
                                {
                                    try
                                    {
                                        this.m_Image.SetFileName(GetSeriesBanner(m_SelectedSeries));
                                        this.m_Image.KeepAspectRatio = true;
                                    }
                                    catch { }
                                }
                                if (m_SelectedSeason != null && this.m_Season_Image != null)
                                {
                                    try
                                    {
                                        this.m_Season_Image.SetFileName(GetSeasonBanner(m_SelectedSeason));
                                        this.m_Season_Image.KeepAspectRatio = true;
                                    }
                                    catch { }

                                }

                                // view handling
                                List<DBEpisode> episodesToDisplay = m_CurrLView.getEpisodeItems(m_CurrViewStep, m_stepSelection);
                                MPTVSeriesLog.Write("LoadFacade: BeginDisplayLoopEp: ", episodesToDisplay.Count.ToString(), MPTVSeriesLog.LogLevel.Normal);

                                Dictionary<int, string> cachedSeriesNames = null;
                                if (!m_CurrLView.stepHasSeriesBeforeIt(m_CurrViewStep))
                                {
                                    // we cache seriesnames so we don't have to query for every item
                                    // only nessesary for views that don't have series before them
                                    cachedSeriesNames = new Dictionary<int, string>();
                                }

                                foreach (DBEpisode episode in episodesToDisplay)
                                {
                                    try
                                    {
                                        bEmpty = false;
                                        GUIListItem item = new GUIListItem();

                                        // its possible the user never selected a series/season (flat view)
                                        // thus its desirable to display series and season index also)
                                        if (!m_CurrLView.stepHasSeriesBeforeIt(m_CurrViewStep))
                                        {
                                            // it is the case
                                            if (cachedSeriesNames.ContainsKey(episode[DBEpisode.cSeriesID]))
                                            {
                                                item.Label = cachedSeriesNames[episode[DBEpisode.cSeriesID]] + " - " + FormatField(m_sFormatEpisodeCol2, episode);
                                            }
                                            else
                                            {
                                                DBOnlineSeries emptySeries = new DBOnlineSeries();
                                                List<DBValue> epsSeries = DBOnlineSeries.GetSingleField(DBOnlineSeries.cPrettyName,
                                                                                                        new SQLCondition(emptySeries, DBOnlineSeries.cID, episode[DBEpisode.cSeriesID], SQLConditionType.Equal),
                                                                                                        emptySeries);
                                                if (epsSeries.Count > 0)
                                                {
                                                    item.Label = epsSeries[0] + " - " + FormatField(m_sFormatEpisodeCol2, episode);
                                                    cachedSeriesNames.Add(episode[DBEpisode.cSeriesID], epsSeries[0]);
                                                }
                                                else
                                                {
                                                    item.Label = FormatField(m_sFormatEpisodeCol2, episode);
                                                    cachedSeriesNames.Add(episode[DBEpisode.cSeriesID], string.Empty);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // we came from series on top, only display index/title
                                            item.Label = FormatField(m_sFormatEpisodeCol2, episode);
                                        }

                                        item.Label2 = FormatField(m_sFormatEpisodeCol3, episode);
                                        item.Label3 = FormatField(m_sFormatEpisodeCol1, episode);
                                        item.IsRemote = episode[DBEpisode.cFilename] != "";
                                        item.IsDownloading = episode[DBOnlineEpisode.cWatched] == 0;
                                        item.TVTag = episode;

                                        if (this.m_SelectedEpisode != null)
                                        {
                                            if (episode[DBEpisode.cCompositeID] == this.m_SelectedEpisode[DBEpisode.cCompositeID])
                                                selectedIndex = count;
                                        }
                                        else
                                        {
                                            // select the first that has a file and is not watched
                                            if (episode[DBEpisode.cFilename] != "" && episode[DBOnlineEpisode.cWatched] == 0 && selectedIndex == -1)
                                                selectedIndex = count;
                                        }

                                        // first returned logo should also show up here in list view directly
                                        item.IconImage = item.IconImageBig = localLogos.getFirstEpLogo(episode);

                                        this.m_Facade.Add(item);
                                    }
                                    catch (Exception ex)
                                    {
                                        MPTVSeriesLog.Write("The 'LoadFacade' function has generated an error displaying episode list item: " + ex.Message);
                                    }
                                    count++;
                                }
                                this.m_Button_Back.Focus = false;
                                this.m_Facade.Focus = true;
                                if (selectedIndex != -1)
                                    this.m_Facade.SelectedListItemIndex = selectedIndex;
                            }
                            MPTVSeriesLog.Write("LoadFacade: Finish");
                            break;
                        #endregion
                    }
                    if (bEmpty)
                    {
                        if (m_CurrViewStep == 0)
                        {
                            this.m_Facade.View = GUIFacadeControl.ViewMode.List;
                            GUIListItem item = new GUIListItem(Translation.No_items);
                            item.IsRemote = true;
                            this.m_Facade.Add(item);

                            m_Description.Label = string.Empty;
                            m_Title.Label = string.Empty;
                            m_Title.Visible = true;
                            m_Genre.Label = string.Empty;

                            m_Image.SetFileName("");
                            m_Season_Image.SetFileName("");
                            this.m_Logos_Image.SetFileName("");

                        }
                        else
                        {
                            // probably something was removed
                            MPTVSeriesLog.Write("Nothing to display, going out");
                            OnAction(new Action(Action.ActionType.ACTION_PREVIOUS_MENU, 0, 0));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MPTVSeriesLog.Write("The 'LoadFacade' function has generated an error: " + e.Message);
            }

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
            if (m_CurrLView != null)
            {
                LoadFacade();
                this.m_Facade.Focus = true;
                setProcessAnimationStatus(m_parserUpdaterWorking);
                return; //otherwise after fullscreen we loose the current view positions
            }
            localLogos.appendEpImage = m_Episode_Image == null ? true : false;
            // get views
            //m_allViews = logicalView.getAllFromDB();
            m_allViews = logicalView.getStaticViews(false); // hardcoded until configuration is set up!
            if (m_allViews.Count > 0)
            {
                try
                {
                    switchView((string)DBOption.GetOptions("lastView"));
                    this.LoadFacade();
                }
                catch (Exception)
                {
                  
                }
            }
            else
            {
                MPTVSeriesLog.Write("Error, cannot display items because: No Views have been found!");
            }
            this.m_Facade.Focus = true;
            m_Title.Height = m_Title.ItemHeight;
            m_Genre.Height = m_Genre.ItemHeight;
            setProcessAnimationStatus(m_parserUpdaterWorking);

        }

        protected override void OnPageDestroy(int new_windowId)
        {
            base.OnPageDestroy(new_windowId);
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
                GUIListItem pItem = null;

                if (m_ListLevel != cListLevelGroup)
                {

                    switch (this.m_ListLevel)
                    {
                        case cListLevelEpisodes:
                            {
                                pItem = new GUIListItem(Translation.Toggle_watched_flag);
                                dlg.Add(pItem);
                                pItem.ItemId = 1;

                                if (foromID != string.Empty)
                                {
                                    pItem = new GUIListItem(Translation.Retrieve_Subtitle);
                                    dlg.Add(pItem);
                                    pItem.ItemId = 2;
                                }

                                pItem = new GUIListItem(Translation.Load_via_Torrent);
                                dlg.Add(pItem);
                                pItem.ItemId = 3;

                                pItem = new GUIListItem("-------------------------------");
                                dlg.Add(pItem);
                                break;
                            }
                        default:
                            pItem = new GUIListItem(Translation.Mark_all_as_watched);
                            dlg.Add(pItem);
                            pItem.ItemId = 11;

                            pItem = new GUIListItem(Translation.Mark_all_as_unwatched);
                            dlg.Add(pItem);
                            pItem.ItemId = 12;
                            break;
                    }

                    pItem = new GUIListItem(Translation.Hide);
                    dlg.Add(pItem);
                    pItem.ItemId = 100 + 1;

                    pItem = new GUIListItem(Translation.Delete);
                    dlg.Add(pItem);
                    pItem.ItemId = 100 + 2;

                    // Fav. handling
                    DBSeries currentSeries;
                    if (m_ListLevel == cListLevelSeries)
                        currentSeries = (DBSeries)currentitem.TVTag;
                    else currentSeries = m_SelectedSeries;

                    pItem = new GUIListItem(currentSeries[DBOnlineSeries.cIsFavourite] == 1 ? Translation.Remove_series_from_Favourites : Translation.Add_series_to_Favourites);
                    dlg.Add(pItem);
                    pItem.ItemId = 100 + 20; //yes its lazy, I know

                    pItem = new GUIListItem("-------------------------------");
                    dlg.Add(pItem);
                }
                pItem = new GUIListItem(Translation.Force_Local_Scan + (m_parserUpdaterWorking ? Translation.In_Progress_with_Barracks : ""));
                dlg.Add(pItem);
                pItem.ItemId = 100 + 3;

                pItem = new GUIListItem(Translation.Force_Online_Refresh + (m_parserUpdaterWorking ? Translation.In_Progress_with_Barracks : ""));
                dlg.Add(pItem);
                pItem.ItemId = 100 + 4;

                pItem = new GUIListItem("-------------------------------");
                dlg.Add(pItem);

                pItem = new GUIListItem(Translation.Only_show_episodes_with_a_local_file + " (" + (DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles) ? Translation.on : Translation.off) + ")");
                dlg.Add(pItem);
                pItem.ItemId = 100 + 5;

                pItem = new GUIListItem(Translation.Hide_summary_on_unwatched +  " (" + (DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedSummary)? "on" : "off") + ")");
                dlg.Add(pItem);
                pItem.ItemId = 100 + 6;

                pItem = new GUIListItem("-------------------------------");
                dlg.Add(pItem);

                pItem = new GUIListItem(Translation.Play_Random_Episode);
                dlg.Add(pItem);
                pItem.ItemId = 100 + 7;

                pItem = new GUIListItem(Translation.Play_Random_First_Unwatched_Episode);
                dlg.Add(pItem);
                pItem.ItemId = 100 + 8;


                dlg.DoModal(GetID);
                if (dlg.SelectedId == -1) return;

                // specific context settings
                switch (this.m_ListLevel)
                {
                    case cListLevelEpisodes:
                        {
                            switch (dlg.SelectedId)
                            {
                                case 1:
                                    {
                                        // toggle watched
                                        DBEpisode currentEpisode = (DBEpisode)currentitem.TVTag;
                                        if (currentEpisode[DBEpisode.cFilename] != String.Empty)
                                        {
                                            SQLCondition condition = new SQLCondition();
                                            condition.Add(new DBEpisode(), DBEpisode.cFilename, currentEpisode[DBEpisode.cFilename], SQLConditionType.Equal);
                                            List<DBEpisode> episodes = DBEpisode.Get(condition, false);
                                            foreach (DBEpisode episode in episodes)
                                            {
                                                episode[DBOnlineEpisode.cWatched] = currentEpisode[DBOnlineEpisode.cWatched] == 0;
                                                episode.Commit();
                                            }
                                        }
                                        else
                                        {
                                            currentEpisode[DBOnlineEpisode.cWatched] = currentEpisode[DBOnlineEpisode.cWatched] == 0;
                                            currentEpisode.Commit();
                                        }
                                        LoadFacade();
                                    }
                                    break;

                                case 2:
                                    {
                                        DBEpisode episode = (DBEpisode)currentitem.TVTag;
                                        setProcessAnimationStatus(true);
                                        foromWorking = true;
                                        Forom forom = new Forom(this);
                                        forom.SubtitleRetrievalCompleted += new WindowPlugins.GUITVSeries.Subtitles.Forom.SubtitleRetrievalCompletedHandler(forom_SubtitleRetrievalCompleted);
                                        forom.GetSubs(episode);
                                    }
                                    break;

                                case 3:
                                    {
                                        DBEpisode episode = (DBEpisode)currentitem.TVTag;
                                        TorrentLoad torrentLoad = new TorrentLoad(this);
                                        torrentWorking = true;
                                        torrentLoad.TorrentLoadCompleted += new WindowPlugins.GUITVSeries.Torrent.TorrentLoad.TorrentLoadCompletedHandler(torrentLoad_TorrentLoadCompleted);
                                        if (torrentLoad.Search(episode))
                                            setProcessAnimationStatus(true);
                                    }
                                    break;
                            }
                        }
                        break;
                }

                // global context settings
                switch (dlg.SelectedId)
                {
                    case 100 + 1:
                        // hide - we can only hide things for now, no unhide
                        switch (this.m_ListLevel)
                        {
                            case cListLevelSeries:
                                DBSeries series = (DBSeries)currentitem.TVTag;
                                series[DBSeries.cHidden] = true;
                                series.Commit();
                                LoadFacade();
                                break;

                            case cListLevelSeasons:
                                DBSeason season = (DBSeason)currentitem.TVTag;
                                season[DBSeason.cHidden] = true;
                                season.Commit();
                                LoadFacade();
                                break;
                            
                            case cListLevelEpisodes:
                                DBEpisode episode = (DBEpisode)currentitem.TVTag;
                                episode[DBOnlineEpisode.cHidden] = true;
                                episode.Commit();
                                LoadFacade();
                                break;
                        }
                        break;

                    case 11:
                        // all watched
                        if (this.m_ListLevel == cListLevelSeries && m_SelectedSeries != null)
                        {
                            DBTVSeries.Execute("update online_episodes set watched = 1 where " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID) + " = " + m_SelectedSeries[DBSeries.cID]);
                        }
                        else if (this.m_ListLevel == cListLevelSeasons && m_SelectedSeason != null)
                        {
                            DBTVSeries.Execute("update online_episodes set watched = 1 where " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID) + " = " + m_SelectedSeason[DBSeason.cSeriesID] +
                                                " and " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeasonIndex) + " = " + m_SelectedSeason[DBSeason.cIndex]);
                        }
                        LoadFacade(); // refresh
                        break;
                    case 12:
                        // all unwatched
                        if (this.m_ListLevel == cListLevelSeries && m_SelectedSeries != null)
                        {
                            DBTVSeries.Execute("update online_episodes set watched = 0 where " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID) + " = " + m_SelectedSeries[DBSeries.cID]);
                        }
                        else if (this.m_ListLevel == cListLevelSeasons && m_SelectedSeason != null)
                        {
                            DBTVSeries.Execute("update online_episodes set watched = 0 where " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID) + " = " + m_SelectedSeason[DBSeason.cSeriesID] +
                                                " and " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeasonIndex) + " = " + m_SelectedSeason[DBSeason.cIndex]);
                        }
                        LoadFacade(); // refresh
                        break;
                    case 100 + 2:
                        {
                            // delete
                            String sMsg = String.Empty;
                            switch (this.m_ListLevel)
                            {
                                case cListLevelSeries:
                                    sMsg = Translation.Delete_that_series;
                                    break;

                                case cListLevelSeasons:
                                    sMsg = Translation.Delete_that_season;
                                    break;
                                
                                case cListLevelEpisodes:
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
                                switch (this.m_ListLevel)
                                {
                                    case cListLevelSeries:
                                        {
                                            DBSeries series = (DBSeries)currentitem.TVTag;
                                            SQLCondition condition = new SQLCondition();
                                            condition.Add(new DBEpisode(), DBEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                                            if (DBOption.GetOptions(DBOption.cDeleteFile)) epsDeletion.AddRange(DBEpisode.Get(condition, false));
                                            DBEpisode.Clear(condition);
                                            condition = new SQLCondition();
                                            condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                                            DBOnlineEpisode.Clear(condition);

                                            condition = new SQLCondition();
                                            condition.Add(new DBSeason(), DBSeason.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                                            DBSeason.Clear(condition);

                                            condition = new SQLCondition();
                                            condition.Add(new DBSeries(), DBSeries.cID, series[DBSeries.cID], SQLConditionType.Equal);
                                            DBSeries.Clear(condition);

                                            condition = new SQLCondition();
                                            condition.Add(new DBOnlineSeries(), DBOnlineSeries.cID, series[DBSeries.cID], SQLConditionType.Equal);
                                            DBOnlineSeries.Clear(condition);
                                            LoadFacade();
                                        }
                                        break;

                                    case cListLevelSeasons:
                                        {
                                            DBSeason season = (DBSeason)currentitem.TVTag;
                                            SQLCondition condition = new SQLCondition();
                                            condition.Add(new DBEpisode(), DBEpisode.cSeriesID, season[DBSeason.cSeriesID], SQLConditionType.Equal);
                                            condition.Add(new DBEpisode(), DBEpisode.cSeasonIndex, season[DBSeason.cIndex], SQLConditionType.Equal);
                                            if (DBOption.GetOptions(DBOption.cDeleteFile)) epsDeletion.AddRange(DBEpisode.Get(condition, false));
                                            DBEpisode.Clear(condition);
                                            condition = new SQLCondition();
                                            condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, season[DBSeason.cSeriesID], SQLConditionType.Equal);
                                            condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeasonIndex, season[DBSeason.cIndex], SQLConditionType.Equal);
                                            DBOnlineEpisode.Clear(condition);

                                            condition = new SQLCondition();
                                            condition.Add(new DBSeason(), DBSeason.cID, season[DBSeason.cID], SQLConditionType.Equal);
                                            DBSeason.Clear(condition);
                                        }
                                        break;
                                    
                                    case cListLevelEpisodes:
                                        {
                                            DBEpisode episode = (DBEpisode)currentitem.TVTag;
                                            SQLCondition condition = new SQLCondition();
                                            condition.Add(new DBEpisode(), DBEpisode.cFilename, episode[DBEpisode.cFilename], SQLConditionType.Equal);
                                            if (DBOption.GetOptions(DBOption.cDeleteFile)) epsDeletion.AddRange(DBEpisode.Get(condition, false));
                                            DBEpisode.Clear(condition);
                                            condition = new SQLCondition();
                                            condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cEpisodeName, episode[DBOnlineEpisode.cEpisodeName], SQLConditionType.Equal);
                                            DBOnlineEpisode.Clear(condition);
                                        }
                                        break;
                                }
                                if (epsDeletion.Count > 0 && DBOption.GetOptions(DBOption.cDeleteFile))
                                {
                                    // delete the actual files!!

                                    dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                                    if (null == dlgYesNo) return;
                                    dlgYesNo.SetHeading(Translation.Confirm);
                                    dlgYesNo.SetLine(1, String.Format(Translation.delPhyiscalWarning, epsDeletion.Count));
                                    dlgYesNo.SetDefaultToYes(false);
                                    dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
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

                    case 100 + 20:
                        {
                            // toggle Favourites
                            m_SelectedSeries.toggleFavourite();
                            // if we are in fav view we have to reload to get the series away
                            LoadFacade();
                            break;
                        }
                    case 100 + 3:
                        // queue scan
                        lock (m_parserUpdaterQueue)
                        {
                            m_parserUpdaterQueue.Add(new CParsingParameters(true, false));
                        }
                        break;

                    case 100 + 4:
                        // queue scan
                        lock (m_parserUpdaterQueue)
                        {
                            m_parserUpdaterQueue.Add(new CParsingParameters(true, true));
                        }
                        break;

                    case 100 + 5:
                        DBOption.SetOptions(DBOption.cView_Episode_OnlyShowLocalFiles, !DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles));
                        LoadFacade();
                        break;

                    case 100 + 6:
                        DBOption.SetOptions(DBOption.cView_Episode_HideUnwatchedSummary, !DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedSummary));
                        LoadFacade();
                        break;
                    case 100 + 7:
                        playRandomEp(false);
                        break;
                    case 100 + 8:
                        playRandomEp(true);
                        break;
                }
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("The 'OnShowContextMenu' function has generated an error: " + ex.Message);
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

        void torrentLoad_TorrentLoadCompleted(bool bOK)
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
            switchView(Helper.getElementFromList<logicalView, string>(viewName, "Name", 0, m_allViews));
        }
        void switchView(logicalView view)
        {
            if (view == null) view = m_allViews[0]; // view was removed or something
            MPTVSeriesLog.Write("Switching logical view to " + view.Name);
            m_CurrLView = view;

            // set the skin labels
            try
            {
                view_curr.Label = view.prettyName;
                if (m_allViews.Count > 1)
                {
                    view_next.Label = Helper.getElementFromList<logicalView, string>(m_CurrLView.Name, "Name", 1, m_allViews).prettyName;
                    view_prev.Label = Helper.getElementFromList<logicalView, string>(m_CurrLView.Name, "Name", -1, m_allViews).prettyName;
                }
                else
                {
                    // if only one (enabled) view supress the display of prev/next
                    view_next.Label = string.Empty;
                    view_prev.Label = string.Empty;
                }
            }
            catch (Exception)
            {
                MPTVSeriesLog.Write("Error displaying view names....check your skin file");
            }

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
                    m_ListLevel = cListLevelGroup;
                    break;
                case logicalViewStep.type.series:
                    m_ListLevel = cListLevelSeries;
                    break;
                case logicalViewStep.type.season:
                    m_ListLevel = cListLevelSeasons;
                    break;
                case logicalViewStep.type.episode:
                    m_ListLevel = cListLevelEpisodes;
                    break;
            }
            MPTVSeriesLog.Write("new listlevel: " + m_ListLevel);
        }

        public override void OnAction(Action action)
        {
            switch (action.wID)
            {
                case Action.ActionType.ACTION_PARENT_DIR:
                case Action.ActionType.ACTION_HOME:
                case Action.ActionType.ACTION_PREVIOUS_MENU:
                    // back one level
                    MPTVSeriesLog.Write("ACTION_PREVIOUS_MENU", MPTVSeriesLog.LogLevel.Debug);
                    if (m_CurrViewStep == 0) GUIWindowManager.ShowPreviousWindow();
                    else
                    {
                        m_stepSelections.RemoveAt(m_CurrViewStep);
                        m_CurrViewStep--;
                        setNewListLevelOfCurrView(m_CurrViewStep);
                        m_back_up_select_this = m_stepSelection;
                        m_stepSelection = m_stepSelections[m_CurrViewStep];
                        skipSeasonIfOne_DirectionDown = false; // otherwise the user cant get back out
                        LoadFacade();
                        skipSeasonIfOne_DirectionDown = true;
                    }
                    break;
                case Action.ActionType.ACTION_MOVE_LEFT:
                        switchView(-1);
                        LoadFacade();
                    break;
                case Action.ActionType.ACTION_MOVE_RIGHT:
                        switchView(1);
                        LoadFacade();
                    break;

                default:
                    base.OnAction(action);
                    break;
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
                switch (this.m_ListLevel)
                {
                    case cListLevelGroup:
                        this.m_CurrViewStep++;
                        setNewListLevelOfCurrView(m_CurrViewStep);
                        m_stepSelection = new string[] { this.m_Facade.SelectedListItem.Label };
                        m_stepSelections.Add(m_stepSelection);
                        MPTVSeriesLog.Write("Selected: ", this.m_Facade.SelectedListItem.Label, MPTVSeriesLog.LogLevel.Normal);
                        LoadFacade();
                        this.m_Facade.Focus = true;
                        break;
                    case cListLevelSeries:
                        this.m_CurrViewStep++;
                        setNewListLevelOfCurrView(m_CurrViewStep);
                        this.m_SelectedSeries = (DBSeries)this.m_Facade.SelectedListItem.TVTag;
                        m_stepSelection = new string[] { m_SelectedSeries[DBSeries.cID].ToString() };
                        m_stepSelections.Add(m_stepSelection);
                        MPTVSeriesLog.Write("Selected: ", m_stepSelection[0], MPTVSeriesLog.LogLevel.Normal);
                        this.LoadFacade();
                        this.m_Facade.Focus = true;
                        break;
                    case cListLevelSeasons:
                        this.m_CurrViewStep++;
                        setNewListLevelOfCurrView(m_CurrViewStep);
                        this.m_SelectedSeason = (DBSeason)this.m_Facade.SelectedListItem.TVTag;
                        m_stepSelection = new string[] { m_SelectedSeason[DBSeason.cSeriesID].ToString(), m_SelectedSeason[DBSeason.cIndex].ToString() };
                        m_stepSelections.Add(m_stepSelection);
                        MPTVSeriesLog.Write("Selected: ", m_stepSelection[0] + " - " + m_stepSelection[1], MPTVSeriesLog.LogLevel.Normal);
                        this.LoadFacade();
                        this.m_Facade.Focus = true;
                        break;
                    case cListLevelEpisodes:
                        this.m_SelectedEpisode = (DBEpisode)this.m_Facade.SelectedListItem.TVTag;
                        MPTVSeriesLog.Write("Selected: ", this.m_SelectedEpisode[DBEpisode.cCompositeID].ToString(), MPTVSeriesLog.LogLevel.Normal);
                        if (m_VideoHandler.ResumeOrPlay(m_SelectedEpisode))
                        {
                            m_SelectedEpisode[DBOnlineEpisode.cWatched] = 1;
                            m_SelectedEpisode.Commit();

                            this.LoadFacade();
                        }
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
                            switch (this.m_ListLevel)
                            {
                                case cListLevelGroup:
                                    Group_OnItemSelected(m_Facade.SelectedListItem);
                                    break;
                                case cListLevelSeries:
                                    Series_OnItemSelected(m_Facade.SelectedListItem);
                                    break;

                                case cListLevelSeasons:
                                    Season_OnItemSelected(m_Facade.SelectedListItem);
                                    break;

                                case cListLevelEpisodes:
                                    Episode_OnItemSelected(m_Facade.SelectedListItem);
                                    break;
                            }
                        }
                    }
                    break;
            }
            return base.OnMessage(message);
        }

        private int CountCRLF(String sIn)
        {
            int nCount = -1;
            int nNext = 0;
            do
            {
                nCount++;
                if (nNext < sIn.Length)
                    nNext = sIn.IndexOf((char)10, nNext + 1);
                else
                    nNext = -1;
            }
            while (nNext != -1);
            return nCount;
        }
        
        private void Group_OnItemSelected(GUIListItem item)
        {
            m_SelectedSeries = null;
            m_SelectedSeason = null;
            m_SelectedEpisode = null;
            if (item == null) return;
            
            m_Genre.Label = string.Empty;

            this.m_Logos_Image.FileName = localLogos.getLogos(m_CurrLView.groupedInfo(m_CurrViewStep), this.m_Facade.SelectedListItem.Label, m_Logos_Image.Height, m_Logos_Image.Width);

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
                    cond.Add(new DBOnlineSeries(), DBOnlineSeries.cHasLocalFiles, true, SQLConditionType.Equal);
                }
                cond.AddCustom("exists ( select id from local_series where id = online_series.id and hidden = 0)");

                foreach (string series in DBOnlineSeries.GetSingleField(DBOnlineSeries.cPrettyName, cond, new DBOnlineSeries()))
                {
                    seriesNames += series + Environment.NewLine;
                    count++;
                }
                
                m_Genre.Label = count.ToString() + " " + (count == 1 ? Translation.Series : Translation.Series_Plural);
                m_Description.Label = seriesNames;
            }
            else m_Description.Label = string.Empty;

            m_Title.Label = item.Label.ToString();

            m_Image.SetFileName("");
            m_Season_Image.SetFileName("");

            if (!localLogos.appendEpImage && m_Episode_Image != null)
            {
                try
                {
                    this.m_Episode_Image.FileName = string.Empty;
                    this.m_Episode_Image.Visible = true;
                }
                catch { }
            }
        }

        private void Series_OnItemSelected(GUIListItem item)
        {
            m_SelectedSeason = null;
            m_SelectedEpisode = null;
            if (item == null)
                return;

            DBSeries series = (DBSeries)item.TVTag;
            m_SelectedSeries = series;
            if (this.m_Image != null)
            {
                try
                {
                    m_Image.SetFileName(GetSeriesBanner(series));
                    m_Image.KeepAspectRatio = true;
                }
                catch { }
            }

            if (m_Season_Image != null)
            {
                try
                {
                    m_Season_Image.SetFileName("");
                }
                catch { }
            }

            if (this.m_Logos_Image != null)
            {
                try
                {
                    this.m_Logos_Image.FileName = localLogos.getLogos(ref series, m_Logos_Image.Height, m_Logos_Image.Width);
                    this.m_Logos_Image.Visible = true;
                }
                catch { }
            }

            if (!localLogos.appendEpImage && m_Episode_Image != null)
            {
                try
                {
                    this.m_Episode_Image.FileName = string.Empty;
                    this.m_Episode_Image.Visible = true;
                }
                catch { }
            }

            if (DBOption.GetOptions(DBOption.cViewAutoHeight))
            {
                int nStartOffset = m_Image.YPosition + m_Image.Height + 5;
                int nBottomLimit = m_Description.YPosition + m_Description.Height;
                if (m_Title != null)
                {
                    m_Title.Label = String.Empty;
                    m_Title.YPosition = nStartOffset;
                    String sTitle = FormatField(m_sFormatSeriesTitle, series);
                    m_Title.Label = sTitle;
                    int nLines = CountCRLF(sTitle) + 1;
                    if (nLines > 4)
                        nLines = 4;
                    m_Title.Height = (m_Title.ItemHeight + m_Title.Space) * (nLines);
                    m_Title.AllocResources();
                    nStartOffset += m_Title.Height + 5;
                }

                if (m_Genre != null)
                {
                    m_Genre.Label = String.Empty;
                    m_Genre.YPosition = nStartOffset;
                    String sLabel = FormatField(m_sFormatSeriesSubtitle, series);
                    m_Genre.Label = sLabel;
                    int nLines = CountCRLF(sLabel) + 1;
                    if (nLines > 4)
                        nLines = 4;
                    m_Genre.Height = (m_Genre.ItemHeight + m_Genre.Space) * (nLines);
                    m_Genre.AllocResources();
                    nStartOffset += m_Genre.Height + 5;
                }

                if (this.m_Description != null)
                {
                    m_Description.Label = String.Empty;
                    m_Description.YPosition = nStartOffset;
                    m_Description.Height = nBottomLimit - nStartOffset;
                    m_Description.Label = FormatField(m_sFormatSeriesMain, series);
                }
            }
            else
            {
                if (m_Title != null)
                    m_Title.Label = FormatField(m_sFormatSeriesTitle, series);
                if (m_Genre != null)
                    m_Genre.Label = FormatField(m_sFormatSeriesSubtitle, series);
                if (m_Description != null)
                    m_Description.Label = FormatField(m_sFormatSeriesMain, series);
            }
        }

        private void Season_OnItemSelected(GUIListItem item)
        {
            m_SelectedEpisode = null;
            if (item == null)
                return;

            DBSeason season = (DBSeason)item.TVTag;
            m_SelectedSeason = season;
            if (m_Season_Image != null)
            {
                try
                {
                    m_Season_Image.SetFileName(GetSeasonBanner(season));
                    m_Season_Image.KeepAspectRatio = true;
                }
                catch { }
            }

            if (this.m_Logos_Image != null)
            {
                try
                {
                    this.m_Logos_Image.FileName = localLogos.getLogos(ref season, m_Logos_Image.Height, m_Logos_Image.Width);
                    this.m_Logos_Image.Visible = true;
                }
                catch { }
            }

            if (!localLogos.appendEpImage && m_Episode_Image != null)
            {
                try
                {
                    this.m_Episode_Image.FileName = string.Empty;
                    this.m_Episode_Image.Visible = true;
                }
                catch { }
            }

            if (DBOption.GetOptions(DBOption.cViewAutoHeight))
            {
                m_Title.Label = String.Empty;
                int nStartOffset = m_Image.YPosition + m_Image.Height + 5;
                int nBottomLimit = m_Description.YPosition + m_Description.Height;
                if (m_Title != null)
                {
                    m_Title.YPosition = nStartOffset;
                    m_Title.Label = FormatField(m_sFormatSeasonTitle, season);
                    nStartOffset += m_Title.Height + 5;
                }

                if (m_Genre != null)
                {
                    m_Genre.Label = String.Empty;
                    m_Genre.YPosition = nStartOffset;
                    String sLabel = FormatField(m_sFormatSeasonSubtitle, season);
                    m_Genre.Label = sLabel;
                    int nLines = CountCRLF(sLabel) + 1;
                    if (nLines > 4)
                        nLines = 4;
                    m_Genre.Height = (m_Genre.ItemHeight + m_Genre.Space) * (nLines);
                    m_Genre.AllocResources();
                    nStartOffset += m_Genre.Height + 5;
                }

                if (this.m_Description != null)
                {
                    m_Description.Label = String.Empty;
                    m_Description.YPosition = nStartOffset;
                    m_Description.Height = nBottomLimit - nStartOffset;
                    m_Description.Label = FormatField(m_sFormatSeasonMain, season);
                }
            }
            else
            {
                if (m_Title != null)
                    m_Title.Label = FormatField(m_sFormatSeasonTitle, season);
                if (m_Genre != null)
                    m_Genre.Label = FormatField(m_sFormatSeasonSubtitle, season);
                if (m_Description != null)
                    m_Description.Label = FormatField(m_sFormatSeasonMain, season);
            }
        }
        private void Episode_OnItemSelected(GUIListItem item)
        {
            if (item == null)
                return;
            DBEpisode episode = (DBEpisode)item.TVTag;
            this.m_SelectedEpisode = episode;
            if (this.m_Logos_Image != null)
            {
                try
                {
                    this.m_Logos_Image.FileName = localLogos.getLogos(ref episode, m_Logos_Image.Height, m_Logos_Image.Width);
                    this.m_Logos_Image.Visible = true;
                }
                catch { }
            }
            if (!localLogos.appendEpImage && m_Episode_Image != null)
            {
                try
                {
                    this.m_Episode_Image.FileName = episode.Image;
                    this.m_Episode_Image.Visible = true;
                }
                catch { }
            }

            // with groups in episode view its possible the user never selected a series/season (flat view)
            // thus its desirable to display the series_banner and season banner on hover)
            if (!m_CurrLView.stepHasSeriesBeforeIt(m_CurrViewStep))
            {
                // it is the case
                SQLCondition cond = new SQLCondition();
                cond.Add(new DBSeries(), DBSeries.cID, episode[DBEpisode.cSeriesID], SQLConditionType.Equal);
                m_SelectedSeries = DBSeries.Get(cond)[0]; // set selected season and series for formatfield below
                m_SelectedSeason = null;
                
                foreach (DBSeason s in DBSeason.Get(episode[DBEpisode.cSeriesID], false, false, true, new SQLCondition(new DBSeason(), DBSeason.cIndex, episode[DBEpisode.cSeasonIndex], SQLConditionType.Equal)))
                {
                    m_SelectedSeason = s;
                    break;
                }
                if (this.m_Image != null && m_SelectedSeries != null)
                {
                    try
                    {
                        m_Image.SetFileName(GetSeriesBanner(m_SelectedSeries));
                        m_Image.KeepAspectRatio = true;
                        m_Image.Visible = true;
                    }
                    catch { }
                }
                if (m_Season_Image != null && m_SelectedSeason != null)
                {
                    try
                    {
                        m_Season_Image.SetFileName(GetSeasonBanner(m_SelectedSeason));
                        m_Season_Image.KeepAspectRatio = true;
                        m_Season_Image.Visible = true;
                    }
                    catch { }
                }
            }

            if (DBOption.GetOptions(DBOption.cViewAutoHeight))
            {
                int nStartOffset = m_Image.YPosition + m_Image.Height + 5;
                int nBottomLimit = m_Description.YPosition + m_Description.Height;
                if (m_Title != null)
                {
                    m_Title.Label = String.Empty;
                    m_Title.YPosition = nStartOffset;
                    m_Title.Label = FormatField(m_sFormatEpisodeTitle, episode);
                    nStartOffset += m_Title.Height + 5;
                }

                if (m_Genre != null)
                {
                    m_Genre.Label = String.Empty;
                    m_Genre.YPosition = nStartOffset;
                    String sLabel = FormatField(m_sFormatEpisodeSubtitle, episode);
                    m_Genre.Label = sLabel;
                    int nLines = CountCRLF(sLabel) + 1;
                    if (nLines > 4)
                        nLines = 4;
                    m_Genre.Height = m_Genre.ItemHeight * (nLines);
                    nStartOffset += m_Genre.Height + 5;
                }

                if (this.m_Description != null)
                {
                    m_Description.Label = String.Empty;
                    m_Description.YPosition = nStartOffset;
                    m_Description.Height = nBottomLimit - nStartOffset;
                    m_Description.Label = FormatField(m_sFormatEpisodeMain, episode);
                }
            }
            else
            {
                if (m_Title != null)
                    m_Title.Label = FormatField(m_sFormatEpisodeTitle, episode);
                if (m_Genre != null)
                    m_Genre.Label = FormatField(m_sFormatEpisodeSubtitle, episode);
                if (m_Description != null)
                    m_Description.Label = FormatField(m_sFormatEpisodeMain, episode);
            }
        }

        public ReturnCode ChooseFromSelection(CDescriptor descriptor, out CItem selected)
        {
            try
            {
                GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                selected = null;
                if (dlg == null)
                    return ReturnCode.Cancel;

                dlg.Reset();
                if (descriptor.m_sItemToMatchLabel == "")
                    dlg.SetHeading(descriptor.m_sTitle);
                else
                    dlg.SetHeading(descriptor.m_sItemToMatchLabel + " " + descriptor.m_sItemToMatch);

                GUIListItem pItem = null;

                if (descriptor.m_sbtnIgnoreLabel != String.Empty)
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

                dlg.DoModal(GetID);
                if (dlg.SelectedId == -1)
                {
                    return ReturnCode.Cancel;
                }
                else
                {
                    if (dlg.SelectedId < 10)
                    {
                        return ReturnCode.Ignore;
                    }
                    else
                    {
                        CItem DlgSelected = descriptor.m_List[dlg.SelectedId - 10];
                        selected = new CItem(descriptor.m_sItemToMatch, String.Empty, DlgSelected.m_Tag);
                        return ReturnCode.OK;
                    }
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

        private void playRandomEp(bool firstUnwatchedOnly)
        {
            List<DBEpisode> validEps = new List<DBEpisode>();
            SQLCondition conditions = new SQLCondition();
            if (!firstUnwatchedOnly)
                conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cWatched, new DBValue(true), SQLConditionType.Equal);
            switch (this.m_ListLevel)
            {
                case cListLevelEpisodes:
                    {
                        // pick one from the current series/season only
                        if (firstUnwatchedOnly)
                        {
                            validEps.Add(DBEpisode.GetFirstUnwatched(Convert.ToInt32((string)m_SelectedSeries[DBSeries.cID]), Convert.ToInt32((string)m_SelectedSeason[DBSeason.cIndex])));
                            break;
                        }
                        else
                            conditions.Add(new DBEpisode(), DBEpisode.cSeasonIndex, new DBValue((string)m_SelectedSeason[DBSeason.cIndex]), SQLConditionType.Equal);
                        goto case cListLevelSeasons; // also add the series condition
                    }
                case cListLevelSeasons:
                    {
                        // pick one from the current series only
                        if (firstUnwatchedOnly)
                            validEps.Add(DBEpisode.GetFirstUnwatched(Convert.ToInt32((string)m_SelectedSeries[DBSeries.cID])));
                        else
                            conditions.Add(new DBEpisode(), DBEpisode.cSeriesID, new DBValue((string)m_SelectedSeries[DBSeries.cID]), SQLConditionType.Equal);
                        break;
                    }
                default:
                    if(firstUnwatchedOnly)
                        validEps = DBEpisode.GetFirstUnwatched();
                    break;
            }
            if(!firstUnwatchedOnly)
                validEps = DBEpisode.Get(conditions, false);

            DBEpisode pickedEp;

            if (validEps != null && validEps.Count > 0)
            {   
                if(validEps.Count > 1)
                    pickedEp = validEps[new Random().Next(0, validEps.Count)];
                else
                    pickedEp = validEps[0];
            }
            else
                return;
            if (m_VideoHandler.ResumeOrPlay(pickedEp))
            {
                SQLCondition condition = new SQLCondition();
                condition.Add(new DBEpisode(), DBEpisode.cFilename, pickedEp[DBEpisode.cFilename], SQLConditionType.Equal);
                List<DBEpisode> episodes = DBEpisode.Get(condition, false);
                foreach (DBEpisode episode in episodes)
                {
                    episode[DBOnlineEpisode.cWatched] = 1;
                    episode.Commit();
                }
                this.LoadFacade();
            }
        }

        private void setProcessAnimationStatus(bool enable)
        {
            //MPTVSeriesLog.Write("Set Animation: ", enable.ToString(), MPTVSeriesLog.LogLevel.Normal);
            if (m_ImportAnimation != null)
            {
                if(enable)
                    m_ImportAnimation.AllocResources();
                else
                    m_ImportAnimation.FreeResources();
                m_ImportAnimation.Visible = enable;
                //MPTVSeriesLog.Write("Set Animation: ", "Done", MPTVSeriesLog.LogLevel.Normal);
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
            if (null != this.m_Facade) localLogos.cleanUP();
        }
    }
}
