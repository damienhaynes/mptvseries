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
        private String m_ListLevel = cListLevelSeries;
        private DBSeries m_SelectedSeries;
        private DBSeason m_SelectedSeason;
        private DBEpisode m_SelectedEpisode;
        private VideoHandler m_VideoHandler;

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

            m_watcherUpdater = new Watcher(this);
            m_watcherUpdater.WatcherProgress += new Watcher.WatcherProgressHandler(watcherUpdater_WatcherProgress);
            m_watcherUpdater.StartFolderWatch();

            // always do a local scan when starting up the app - later on the watcher will monitor changes
            m_parserUpdaterQueue.Add(new CParsingParameters(true, false));

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
            m_scanTimer = new System.Threading.Timer(timerDelegate, null, 1000, 1000);
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
                                                        sOut += " * Hidden to prevent spoilers *";
                                                    break;

                                                case DBOnlineEpisode.cWatched:
                                                    sOut += source[sFieldName] == 0 ? "No" : "Yes";
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
                        sOut += "#Error";
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
                    gph.DrawString("Season " + season[DBSeason.cIndex], font, new SolidBrush(Color.FromArgb(200, Color.White)), 5, (sizeImage.Height - font.GetHeight()) / 2);
                    gph.Dispose();
                    GUITextureManager.LoadFromMemory(image, "[" + season[DBSeason.cID] + "]", 0, sizeImage.Width, sizeImage.Height);
                }
                return "[" + season[DBSeason.cID] + "]";
            }
        }

        void LoadFacade()
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
                switch (this.m_ListLevel)
                {
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

                            SQLCondition condition = new SQLCondition();
                            condition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);
                            condition.Add(new DBSeries(), DBSeries.cHidden, 0, SQLConditionType.Equal);
                            if (DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles))
                                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cHasLocalFiles, 0, SQLConditionType.NotEqual);

                            List<DBSeries> seriesList = DBSeries.Get(condition);

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
                                        if (series[DBOnlineSeries.cHasLocalFiles] != 0 && selectedIndex == -1)
                                            selectedIndex = count;
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
                                this.m_Facade.SelectedListItemIndex = selectedIndex;
                        }
                        break;
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
                                m_Facade.AlbumListView.IconOffsetY = m_nInitialIconYOffset * (sizeImage.Height - 2* m_Facade.AlbumListView.IconOffsetY) / m_nInitialItemHeight;
                                sizeImage.Height -=  2* m_Facade.AlbumListView.IconOffsetY;
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

                            foreach (DBSeason season in DBSeason.Get(m_SelectedSeries[DBSeries.cID], DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles), true, false))
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
                        }
                        break;

                    case cListLevelEpisodes:
                        {
                            m_Season_Image.Visible = true;
                            int selectedIndex = -1;
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

                            foreach (DBEpisode episode in DBEpisode.Get(m_SelectedSeries[DBSeries.cID], m_SelectedSeason[DBSeason.cIndex], DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles), DBOption.GetOptions(DBOption.cShowHiddenItems)))
                            {
                                try
                                {
                                    bEmpty = false;
                                    GUIListItem item = new GUIListItem(FormatField(m_sFormatEpisodeCol2, episode));
                                    item.Label2 = FormatField(m_sFormatEpisodeCol3, episode);
                                    item.Label3 = FormatField(m_sFormatEpisodeCol1, episode);
                                    item.IsRemote = episode[DBEpisode.cFilename] != "";
                                    item.IsDownloading = episode[DBOnlineEpisode.cWatched] == 0;
                                    item.TVTag = episode;

                                    if (this.m_SelectedEpisode != null)
                                    {
                                        if (episode[DBEpisode.cEpisodeIndex] == this.m_SelectedEpisode[DBEpisode.cEpisodeIndex])
                                            selectedIndex = count;
                                    }
                                    else
                                    {
                                        // select the first that has a file and is not watched
                                        if (episode[DBEpisode.cFilename] != "" && episode[DBOnlineEpisode.cWatched] == 0 && selectedIndex == -1)
                                            selectedIndex = count;
                                    }

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
                        break;
                }
                if (bEmpty)
                {
                    GUIListItem item = new GUIListItem("No items!");
                    item.IsRemote = true;
                    this.m_Facade.Add(item);
                }
            }
        }

        protected override void OnPageLoad()
        {
            this.LoadFacade();
            if (m_parserUpdaterWorking)
            {
                if (m_ImportAnimation != null)
                    m_ImportAnimation.AllocResources();
            }
            else
            {
                if (m_ImportAnimation != null)
                    m_ImportAnimation.FreeResources();
            }

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

                switch (this.m_ListLevel)
                {
                    case cListLevelEpisodes:
                        {
                            DBEpisode episode = (DBEpisode)currentitem.TVTag;
                            pItem = new GUIListItem("Toggle watched flag");
                            dlg.Add(pItem);
                            pItem.ItemId = 1;

                            pItem = new GUIListItem("Retrieve Subtitle");
                            dlg.Add(pItem);
                            pItem.ItemId = 2;

                            pItem = new GUIListItem("Load via Torrent");
                            dlg.Add(pItem);
                            pItem.ItemId = 3;

                            pItem = new GUIListItem("-------------------------------");
                            dlg.Add(pItem);
                        }
                        break;
                }

                pItem = new GUIListItem("Hide");
                dlg.Add(pItem);
                pItem.ItemId = 100 + 1;

                pItem = new GUIListItem("Delete");
                dlg.Add(pItem);
                pItem.ItemId = 100 + 2;

                pItem = new GUIListItem("-------------------------------");
                dlg.Add(pItem);

                pItem = new GUIListItem("Force Local Scan" + (m_parserUpdaterWorking ? " (In Progress)" : ""));
                dlg.Add(pItem);
                pItem.ItemId = 100 + 3;

                pItem = new GUIListItem("Force Online Refresh" + (m_parserUpdaterWorking ? " (In Progress)" : ""));
                dlg.Add(pItem);
                pItem.ItemId = 100 + 4;

                pItem = new GUIListItem("-------------------------------");
                dlg.Add(pItem);

                pItem = new GUIListItem("Only show episodes with a local file (" + (DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles) ? "on" : "off") + ")");
                dlg.Add(pItem);
                pItem.ItemId = 100 + 5;

                pItem = new GUIListItem("Hide the episode's summary on unwatched episodes (" + (DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedSummary)? "on" : "off") + ")");
                dlg.Add(pItem);
                pItem.ItemId = 100 + 6;

                pItem = new GUIListItem("-------------------------------");
                dlg.Add(pItem);

                pItem = new GUIListItem("Play Random Episode");
                dlg.Add(pItem);
                pItem.ItemId = 100 + 7;

                pItem = new GUIListItem("Play Random First Unwatched Episode");
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
                                        if (m_ImportAnimation != null)
                                            m_ImportAnimation.AllocResources();
                                        Forom forom = new Forom(this);
                                        forom.SubtitleRetrievalCompleted += new WindowPlugins.GUITVSeries.Subtitles.Forom.SubtitleRetrievalCompletedHandler(forom_SubtitleRetrievalCompleted);
                                        forom.GetSubs(episode);
                                    }
                                    break;

                                case 3:
                                    {
                                        DBEpisode episode = (DBEpisode)currentitem.TVTag;
                                        TorrentLoad torrentLoad = new TorrentLoad(this);
                                        torrentLoad.TorrentLoadCompleted += new WindowPlugins.GUITVSeries.Torrent.TorrentLoad.TorrentLoadCompletedHandler(torrentLoad_TorrentLoadCompleted);
                                        if (torrentLoad.Search(episode))
                                            if (m_ImportAnimation != null)
                                                m_ImportAnimation.AllocResources();
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

                    case 100 + 2:
                        {
                            // delete
                            String sMsg = String.Empty;
                            switch (this.m_ListLevel)
                            {
                                case cListLevelSeries:
                                    sMsg = "Delete that series?";
                                    break;

                                case cListLevelSeasons:
                                    sMsg = "Delete that season?";
                                    break;
                                
                                case cListLevelEpisodes:
                                    sMsg = "Delete that episode?";
                                    break;
                            }
                            GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                            if (null == dlgYesNo) return;
                            dlgYesNo.SetHeading("Confirm"); //resume movie?
                            dlgYesNo.SetLine(1, sMsg);
                            dlgYesNo.SetDefaultToYes(false);
                            dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
                            if (dlgYesNo.IsConfirmed)
                            {
                                switch (this.m_ListLevel)
                                {
                                    case cListLevelSeries:
                                        {
                                            DBSeries series = (DBSeries)currentitem.TVTag;
                                            SQLCondition condition = new SQLCondition();
                                            condition.Add(new DBEpisode(), DBEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
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
                                            condition.Add(new DBEpisode(), DBEpisode.cEpisodeName, episode[DBEpisode.cEpisodeName], SQLConditionType.Equal);
                                            DBEpisode.Clear(condition);
                                            condition = new SQLCondition();
                                            condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cEpisodeName, episode[DBOnlineEpisode.cEpisodeName], SQLConditionType.Equal);
                                            DBOnlineEpisode.Clear(condition);
                                        }
                                        break;
                                }
                            }
                        }
                        break;

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
            if (m_ImportAnimation != null)
                m_ImportAnimation.FreeResources();
            if (bFound)
            {
                LoadFacade();
            }
            else
            {
                GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                dlgOK.SetHeading("Completed");
                dlgOK.SetLine(1, "No subtitles found");
                dlgOK.DoModal(GUIWindowManager.ActiveWindow);
            }
        }

        void torrentLoad_TorrentLoadCompleted(bool bOK)
        {
            if (m_ImportAnimation != null)
                m_ImportAnimation.FreeResources();
        }


        public override void OnAction(Action action)
        {
            switch (action.wID)
            {
                case Action.ActionType.ACTION_PARENT_DIR:
                case Action.ActionType.ACTION_HOME:
                case Action.ActionType.ACTION_PREVIOUS_MENU:
                    // back one level
                    switch (this.m_ListLevel)
                    {
                        case cListLevelSeries:
                            GUIWindowManager.ShowPreviousWindow();
                            break;
                        case cListLevelSeasons:
                            this.m_ListLevel = cListLevelSeries;
                            this.m_SelectedSeason = null;
                            this.LoadFacade();
                            break;
                        case cListLevelEpisodes:
                            this.m_ListLevel = cListLevelSeasons;
                            this.m_SelectedEpisode = null;
                            this.LoadFacade();
                            break;
                    }
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

                switch (this.m_ListLevel)
                {
                    case cListLevelSeries:
                        this.m_ListLevel = cListLevelSeasons;
                        this.m_SelectedSeries = (DBSeries)this.m_Facade.SelectedListItem.TVTag;
                        this.LoadFacade();
                        this.m_Facade.Focus = true;
                        break;
                    case cListLevelSeasons:
                        this.m_ListLevel = cListLevelEpisodes;
                        this.m_SelectedSeason = (DBSeason)this.m_Facade.SelectedListItem.TVTag;
                        this.LoadFacade();
                        this.m_Facade.Focus = true;
                        break;
                    case cListLevelEpisodes:
                        this.m_SelectedEpisode = (DBEpisode)this.m_Facade.SelectedListItem.TVTag;
                       
                        if (m_VideoHandler.ResumeOrPlay(m_SelectedEpisode))
                        {
                            SQLCondition condition = new SQLCondition();
                            condition.Add(new DBEpisode(), DBEpisode.cFilename, this.m_SelectedEpisode[DBEpisode.cFilename], SQLConditionType.Equal);
                            List<DBEpisode> episodes = DBEpisode.Get(condition, false);
                            foreach (DBEpisode episode in episodes)
                            {
                                episode[DBOnlineEpisode.cWatched] = 1;
                                episode.Commit();
                            }
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
                        // only load the wait cursor if we are in the plugin
                        if (m_ImportAnimation != null)
                            m_ImportAnimation.AllocResources();

                        m_parserUpdaterWorking = true;
                        m_parserUpdater.Start(m_parserUpdaterQueue[0]);
                        m_parserUpdaterQueue.RemoveAt(0);
                    }
                }
            }
            base.Process();
        }

        void parserUpdater_OnlineParsingCompleted(bool bDataUpdated)
        {
            if (m_ImportAnimation != null)
                m_ImportAnimation.FreeResources();

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

        private void Series_OnItemSelected(GUIListItem item)
        {
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
            if (item == null)
                return;

            DBSeason season = (DBSeason)item.TVTag;

            if (m_Season_Image != null)
            {
                try
                {
                    m_Season_Image.SetFileName(GetSeasonBanner(season));
                    m_Season_Image.KeepAspectRatio = true;
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
                pItem = new GUIListItem("**** skip / Never ask again ****");
                dlg.Add(pItem);
                pItem.ItemId = 0;
            }

            if (descriptor.m_List.Count == 0)
            {
                pItem = new GUIListItem("no results found!");
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
    }
}
