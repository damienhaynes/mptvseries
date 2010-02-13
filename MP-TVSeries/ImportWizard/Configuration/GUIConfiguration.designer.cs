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


namespace WindowPlugins.GUITVSeries
{
    partial class ConfigurationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle25 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle26 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle27 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle28 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle29 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle30 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle31 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle32 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle33 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle34 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle35 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle36 = new System.Windows.Forms.DataGridViewCellStyle();
            this.columnHeader_Series = new System.Windows.Forms.ColumnHeader();
            this.columnHeader_Title = new System.Windows.Forms.ColumnHeader();
            this.columnHeader_Season = new System.Windows.Forms.ColumnHeader();
            this.columnHeader_Episode = new System.Windows.Forms.ColumnHeader();
            this.columnHeader_OriginallyAired = new System.Windows.Forms.ColumnHeader();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuStrip_DetailsTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.watchedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unWatchedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.getSubtitlesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.torrentThToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newzbinThisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ResetUserSelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenuStrip_InsertFields = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolTip_Help = new System.Windows.Forms.ToolTip(this.components);
            this.checkBox_ShowHidden = new System.Windows.Forms.CheckBox();
            this.textBox_dblocation = new System.Windows.Forms.TextBox();
            this.lblClearDB = new System.Windows.Forms.LinkLabel();
            this.buildExpr = new System.Windows.Forms.LinkLabel();
            this.resetExpr = new System.Windows.Forms.LinkLabel();
            this.dataGridView_Replace = new System.Windows.Forms.DataGridView();
            this.button_TestReparse = new System.Windows.Forms.Button();
            this.button_Start = new System.Windows.Forms.Button();
            this.linkMediaInfoUpdate = new System.Windows.Forms.LinkLabel();
            this.checkBox_AutoOnlineDataRefresh = new System.Windows.Forms.CheckBox();
            this.checkBox_AutoChooseOrder = new System.Windows.Forms.CheckBox();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.checkDownloadEpisodeSnapshots = new System.Windows.Forms.CheckBox();
            this.chkBlankBanners = new System.Windows.Forms.CheckBox();
            this.txtMainMirror = new System.Windows.Forms.TextBox();
            this.linkDelUpdateTime = new System.Windows.Forms.LinkLabel();
            this.comboOnlineLang = new System.Windows.Forms.ComboBox();
            this.checkBox_OnlineSearch = new System.Windows.Forms.CheckBox();
            this.checkBox_AutoChooseSeries = new System.Windows.Forms.CheckBox();
            this.checkBox_FullSeriesRetrieval = new System.Windows.Forms.CheckBox();
            this.linkExWatched = new System.Windows.Forms.LinkLabel();
            this.linkImpWatched = new System.Windows.Forms.LinkLabel();
            this.chkAllowDeletes = new System.Windows.Forms.CheckBox();
            this.checkBox_doFolderWatch = new System.Windows.Forms.CheckBox();
            this.checkBox_DontClearMissingLocalFiles = new System.Windows.Forms.CheckBox();
            this.checkBox_Series_UseSortName = new System.Windows.Forms.CheckBox();
            this.nudWatchedAfter = new System.Windows.Forms.NumericUpDown();
            this.comboLanguage = new System.Windows.Forms.ComboBox();
            this.checkBox_RandBanner = new System.Windows.Forms.CheckBox();
            this.textBox_PluginHomeName = new System.Windows.Forms.TextBox();
            this.btnLogoDown = new System.Windows.Forms.Button();
            this.btnlogoUp = new System.Windows.Forms.Button();
            this.lnkLogoImp = new System.Windows.Forms.LinkLabel();
            this.lstLogos = new System.Windows.Forms.ListBox();
            this.btnLogoTemplate = new System.Windows.Forms.Button();
            this.btnrmvLogo = new System.Windows.Forms.Button();
            this.lnkLogoExport = new System.Windows.Forms.LinkLabel();
            this.btnLogoEdit = new System.Windows.Forms.Button();
            this.addLogo = new System.Windows.Forms.Button();
            this.textBox_NewsSearchReportRegex = new System.Windows.Forms.TextBox();
            this.textBox_uTorrentPath = new System.Windows.Forms.TextBox();
            this.textBox_TorrentDetailsRegex = new System.Windows.Forms.TextBox();
            this.textBox_TorrentSearchRegex = new System.Windows.Forms.TextBox();
            this.textBox_TorrentDetailsUrl = new System.Windows.Forms.TextBox();
            this.textBox_TorrentSearchUrl = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox_ScanOnStartup = new System.Windows.Forms.CheckBox();
            this.checkBox_AutoDownloadMissingArtwork = new System.Windows.Forms.CheckBox();
            this.checkBox_AutoUpdateEpisodeRatings = new System.Windows.Forms.CheckBox();
            this.linkImpParsingExpressions = new System.Windows.Forms.LinkLabel();
            this.linkExParsingExpressions = new System.Windows.Forms.LinkLabel();
            this.checkbox_SortSpecials = new System.Windows.Forms.CheckBox();
            this.checkboxRatingDisplayStars = new System.Windows.Forms.CheckBox();
            this.checkboxAutoDownloadFanartSeriesName = new System.Windows.Forms.CheckBox();
            this.buttonViewTemplates = new System.Windows.Forms.Button();
            this.checkBox_scanRemoteShares = new System.Windows.Forms.CheckBox();
            this.nudScanRemoteShareFrequency = new System.Windows.Forms.NumericUpDown();
            this.checkBox_SubDownloadOnPlay = new System.Windows.Forms.CheckBox();
            this.chkUseRegionalDateFormatString = new System.Windows.Forms.CheckBox();
            this.toolTip_InfoHelp = new System.Windows.Forms.ToolTip(this.components);
            this.checkBox_Episode_HideUnwatchedSummary = new System.Windows.Forms.CheckBox();
            this.checkBox_Episode_OnlyShowLocalFiles = new System.Windows.Forms.CheckBox();
            this.richTextBox_seasonFormat_Col3 = new System.Windows.Forms.RichTextBox();
            this.richTextBox_seasonFormat_Main = new System.Windows.Forms.RichTextBox();
            this.richTextBox_seasonFormat_Col1 = new System.Windows.Forms.RichTextBox();
            this.richTextBox_seasonFormat_Subtitle = new System.Windows.Forms.RichTextBox();
            this.richTextBox_seasonFormat_Col2 = new System.Windows.Forms.RichTextBox();
            this.richTextBox_seasonFormat_Title = new System.Windows.Forms.RichTextBox();
            this.richTextBox_episodeFormat_Col3 = new System.Windows.Forms.RichTextBox();
            this.richTextBox_episodeFormat_Col2 = new System.Windows.Forms.RichTextBox();
            this.richTextBox_episodeFormat_Main = new System.Windows.Forms.RichTextBox();
            this.richTextBox_episodeFormat_Subtitle = new System.Windows.Forms.RichTextBox();
            this.richTextBox_episodeFormat_Title = new System.Windows.Forms.RichTextBox();
            this.richTextBox_episodeFormat_Col1 = new System.Windows.Forms.RichTextBox();
            this.richTextBox_seriesFormat_Col3 = new System.Windows.Forms.RichTextBox();
            this.richTextBox_seriesFormat_Main = new System.Windows.Forms.RichTextBox();
            this.richTextBox_seriesFormat_Subtitle = new System.Windows.Forms.RichTextBox();
            this.richTextBox_seriesFormat_Col2 = new System.Windows.Forms.RichTextBox();
            this.richTextBox_seriesFormat_Title = new System.Windows.Forms.RichTextBox();
            this.richTextBox_seriesFormat_Col1 = new System.Windows.Forms.RichTextBox();
            this.checkBox_Episode_HideUnwatchedThumbnail = new System.Windows.Forms.CheckBox();
            this.optionAsk2Rate = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.dbOptionCheckBoxMarkRatedEpsAsWatched = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.splitMain_Log = new System.Windows.Forms.SplitContainer();
            this.tabControl_Details = new System.Windows.Forms.TabControl();
            this.tabPage_Details = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.treeView_Library = new System.Windows.Forms.TreeView();
            this.pictureBox_SeriesPoster = new System.Windows.Forms.PictureBox();
            this.comboBox_PosterSelection = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.detailsPropertyBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.pictureBox_Series = new System.Windows.Forms.PictureBox();
            this.comboBox_BannerSelection = new System.Windows.Forms.ComboBox();
            this.panDBLocation = new System.Windows.Forms.Panel();
            this.button_dbbrowse = new System.Windows.Forms.Button();
            this.label28 = new System.Windows.Forms.Label();
            this.tabPage_Import = new System.Windows.Forms.TabPage();
            this.splitContainer_SettingsOutput = new System.Windows.Forms.SplitContainer();
            this.splitContainerImportSettings = new System.Windows.Forms.SplitContainer();
            this.treeView_Settings = new System.Windows.Forms.TreeView();
            this.panel_Expressions = new System.Windows.Forms.Panel();
            this.linkExpressionHelp = new System.Windows.Forms.LinkLabel();
            this.label70 = new System.Windows.Forms.Label();
            this.dataGridView_Expressions = new System.Windows.Forms.DataGridView();
            this.button_MoveExpUp = new System.Windows.Forms.Button();
            this.button_MoveExpDown = new System.Windows.Forms.Button();
            this.panel_StringReplacements = new System.Windows.Forms.Panel();
            this.linkLabelResetStringReplacements = new System.Windows.Forms.LinkLabel();
            this.linkLabelImportStringReplacements = new System.Windows.Forms.LinkLabel();
            this.linkLabelExportStringReplacements = new System.Windows.Forms.LinkLabel();
            this.label69 = new System.Windows.Forms.Label();
            this.panel_ImportPathes = new System.Windows.Forms.Panel();
            this.label68 = new System.Windows.Forms.Label();
            this.dataGridView_ImportPathes = new System.Windows.Forms.DataGridView();
            this.listView_ParsingResults = new System.Windows.Forms.ListView();
            this.FileName = new System.Windows.Forms.ColumnHeader();
            this.ParsedSeriesName = new System.Windows.Forms.ColumnHeader();
            this.SeasonID = new System.Windows.Forms.ColumnHeader();
            this.EpisodeID = new System.Windows.Forms.ColumnHeader();
            this.EpisodeTitle = new System.Windows.Forms.ColumnHeader();
            this.panel_manualEpisodeManagement = new WindowPlugins.GUITVSeries.ManualEpisodeManagementPane();
            this.progressBar_Parsing = new System.Windows.Forms.ProgressBar();
            this.tabOnlineData = new System.Windows.Forms.TabPage();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.panel_OnlineData = new System.Windows.Forms.Panel();
            this.checkBox_AutoUpdateAllFanart = new System.Windows.Forms.CheckBox();
            this.label73 = new System.Windows.Forms.Label();
            this.spinMaxFanarts = new System.Windows.Forms.NumericUpDown();
            this.label72 = new System.Windows.Forms.Label();
            this.label71 = new System.Windows.Forms.Label();
            this.cboFanartResolution = new System.Windows.Forms.ComboBox();
            this.chkAutoDownloadFanart = new System.Windows.Forms.CheckBox();
            this.linkAccountID = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown_AutoOnlineDataRefresh = new System.Windows.Forms.NumericUpDown();
            this.label54 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.tabPage_MP_DisplayControl = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dbOptChkBoxScanFullscreenVideo = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.dbOptionCheckBox2 = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.dbOptionCheckBoxSubstituteMissingArtwork = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.dbOptionCheckBoxSMSKeyboard = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.label77 = new System.Windows.Forms.Label();
            this.label76 = new System.Windows.Forms.Label();
            this.label75 = new System.Windows.Forms.Label();
            this.numericUpDownArtworkDelay = new System.Windows.Forms.NumericUpDown();
            this.label74 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.numericUpDownBackdropDelay = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.comboLogLevel = new System.Windows.Forms.ComboBox();
            this.chkShowSeriesFanart = new System.Windows.Forms.CheckBox();
            this.label39 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.tab_view = new System.Windows.Forms.TabPage();
            this.playlistSettings = new WindowPlugins.GUITVSeries.Configuration.PlaylistSettings();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.buttonPinCode = new System.Windows.Forms.Button();
            this.buttonEditView = new System.Windows.Forms.Button();
            this.checkBoxParentalControl = new System.Windows.Forms.CheckBox();
            this.label25 = new System.Windows.Forms.Label();
            this.checkCurViewEnabled = new System.Windows.Forms.CheckBox();
            this.lnkResetView = new System.Windows.Forms.LinkLabel();
            this.btnRemoveView = new System.Windows.Forms.Button();
            this.view_selStepsList = new System.Windows.Forms.ListBox();
            this.btnAddView = new System.Windows.Forms.Button();
            this._availViews = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.view_selectedName = new System.Windows.Forms.TextBox();
            this.tabLogoRules = new System.Windows.Forms.TabPage();
            this.label65 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.btnLogoDeleteAll = new System.Windows.Forms.Button();
            this.tabFormattingRules = new System.Windows.Forms.TabPage();
            this.label64 = new System.Windows.Forms.Label();
            this.formattingConfiguration1 = new WindowPlugins.GUITVSeries.Configuration.FormattingConfiguration();
            this.tabLayoutSettings = new System.Windows.Forms.TabPage();
            this.label37 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabpage_Extra = new System.Windows.Forms.TabPage();
            this.splitContainer_treeextra = new System.Windows.Forms.SplitContainer();
            this.treeView_Extra = new System.Windows.Forms.TreeView();
            this.panel_newssearch = new System.Windows.Forms.Panel();
            this.label33 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.textBox_NewsSearchLanguageRegex = new System.Windows.Forms.TextBox();
            this.textBox_NewsSearchGroupRegex = new System.Windows.Forms.TextBox();
            this.label47 = new System.Windows.Forms.Label();
            this.textBox_NewsSearchParseArticleRegex = new System.Windows.Forms.TextBox();
            this.textBox_NewsSearchIsolateArticleRegex = new System.Windows.Forms.TextBox();
            this.label46 = new System.Windows.Forms.Label();
            this.textBox_NewsSearchFormatRegex = new System.Windows.Forms.TextBox();
            this.textBox_NewsSearchReportDateRegex = new System.Windows.Forms.TextBox();
            this.label49 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.textBox_NewsSearchPostDateRegex = new System.Windows.Forms.TextBox();
            this.label44 = new System.Windows.Forms.Label();
            this.textBox_NewsSearchSizeRegex = new System.Windows.Forms.TextBox();
            this.label43 = new System.Windows.Forms.Label();
            this.textBox_NewsSearchNameRegex = new System.Windows.Forms.TextBox();
            this.label42 = new System.Windows.Forms.Label();
            this.textBox_NewsSearchIDRegex = new System.Windows.Forms.TextBox();
            this.label41 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.textbox_NewzbinPassword = new System.Windows.Forms.TextBox();
            this.textBox_NewzbinLogin = new System.Windows.Forms.TextBox();
            this.textBox_NewsSearchUrl = new System.Windows.Forms.TextBox();
            this.panel_newsroot = new System.Windows.Forms.Panel();
            this.button_NewsDownloadPathBrowse = new System.Windows.Forms.Button();
            this.button_newsleecherbrowse = new System.Windows.Forms.Button();
            this.label35 = new System.Windows.Forms.Label();
            this.textBox_NewsDownloadPath = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.textBox_newsleecher = new System.Windows.Forms.TextBox();
            this.panel_torrentroot = new System.Windows.Forms.Panel();
            this.button_uTorrentBrowse = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.panel_subtitleroot = new System.Windows.Forms.Panel();
            this.subtitleDownloader_enabled = new System.Windows.Forms.CheckBox();
            this.panel_torrentsearch = new System.Windows.Forms.Panel();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.comboBox_TorrentPreset = new System.Windows.Forms.ComboBox();
            this.label24 = new System.Windows.Forms.Label();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.aboutScreen = new WindowPlugins.GUITVSeries.About();
            this.listBox_Log = new System.Windows.Forms.ListBox();
            this.contextMenuStrip_DetailsTree.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Replace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWatchedAfter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScanRemoteShareFrequency)).BeginInit();
            this.splitMain_Log.Panel1.SuspendLayout();
            this.splitMain_Log.Panel2.SuspendLayout();
            this.splitMain_Log.SuspendLayout();
            this.tabControl_Details.SuspendLayout();
            this.tabPage_Details.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SeriesPoster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.detailsPropertyBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Series)).BeginInit();
            this.panDBLocation.SuspendLayout();
            this.tabPage_Import.SuspendLayout();
            this.splitContainer_SettingsOutput.Panel1.SuspendLayout();
            this.splitContainer_SettingsOutput.Panel2.SuspendLayout();
            this.splitContainer_SettingsOutput.SuspendLayout();
            this.splitContainerImportSettings.Panel1.SuspendLayout();
            this.splitContainerImportSettings.Panel2.SuspendLayout();
            this.splitContainerImportSettings.SuspendLayout();
            this.panel_Expressions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Expressions)).BeginInit();
            this.panel_StringReplacements.SuspendLayout();
            this.panel_ImportPathes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ImportPathes)).BeginInit();
            this.tabOnlineData.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.panel_OnlineData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinMaxFanarts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_AutoOnlineDataRefresh)).BeginInit();
            this.tabPage_MP_DisplayControl.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownArtworkDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBackdropDelay)).BeginInit();
            this.tab_view.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.tabLogoRules.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tabFormattingRules.SuspendLayout();
            this.tabLayoutSettings.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabpage_Extra.SuspendLayout();
            this.splitContainer_treeextra.Panel1.SuspendLayout();
            this.splitContainer_treeextra.Panel2.SuspendLayout();
            this.splitContainer_treeextra.SuspendLayout();
            this.panel_newssearch.SuspendLayout();
            this.panel_newsroot.SuspendLayout();
            this.panel_torrentroot.SuspendLayout();
            this.panel_subtitleroot.SuspendLayout();
            this.panel_torrentsearch.SuspendLayout();
            this.tabAbout.SuspendLayout();
            this.SuspendLayout();
            // 
            // columnHeader_Series
            // 
            this.columnHeader_Series.Text = "Series";
            this.columnHeader_Series.Width = 100;
            // 
            // columnHeader_Title
            // 
            this.columnHeader_Title.Text = "Episode Title";
            this.columnHeader_Title.Width = 196;
            // 
            // columnHeader_Season
            // 
            this.columnHeader_Season.Text = "Season";
            // 
            // columnHeader_Episode
            // 
            this.columnHeader_Episode.Text = "Episode";
            this.columnHeader_Episode.Width = 62;
            // 
            // columnHeader_OriginallyAired
            // 
            this.columnHeader_OriginallyAired.Text = "Aired";
            this.columnHeader_OriginallyAired.Width = 89;
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // contextMenuStrip_DetailsTree
            // 
            this.contextMenuStrip_DetailsTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.updateToolStripMenuItem,
            this.toolStripSeparator2,
            this.watchedToolStripMenuItem,
            this.unWatchedToolStripMenuItem,
            this.toolStripSeparator1,
            this.getSubtitlesToolStripMenuItem,
            this.torrentThToolStripMenuItem,
            this.newzbinThisToolStripMenuItem,
            this.ResetUserSelToolStripMenuItem,
            this.toolStripSeparator3});
            this.contextMenuStrip_DetailsTree.Name = "contextMenuStrip_DetailsTree";
            this.contextMenuStrip_DetailsTree.Size = new System.Drawing.Size(185, 220);
            this.contextMenuStrip_DetailsTree.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip_DetailsTree_ItemClicked);
            this.contextMenuStrip_DetailsTree.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_DetailsTree_Opening);
            // 
            // hideToolStripMenuItem
            // 
            this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
            this.hideToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.hideToolStripMenuItem.Tag = "hide";
            this.hideToolStripMenuItem.Text = "Hide";
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.deleteToolStripMenuItem.Tag = "delete";
            this.deleteToolStripMenuItem.Text = "Delete";
            // 
            // updateToolStripMenuItem
            // 
            this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            this.updateToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.updateToolStripMenuItem.Tag = "update";
            this.updateToolStripMenuItem.Text = "Update";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(181, 6);
            // 
            // watchedToolStripMenuItem
            // 
            this.watchedToolStripMenuItem.Name = "watchedToolStripMenuItem";
            this.watchedToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.watchedToolStripMenuItem.Tag = "watched";
            this.watchedToolStripMenuItem.Text = "Set As Watched";
            // 
            // unWatchedToolStripMenuItem
            // 
            this.unWatchedToolStripMenuItem.Name = "unWatchedToolStripMenuItem";
            this.unWatchedToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.unWatchedToolStripMenuItem.Tag = "unwatched";
            this.unWatchedToolStripMenuItem.Text = "Set As Un-Watched";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(181, 6);
            // 
            // getSubtitlesToolStripMenuItem
            // 
            this.getSubtitlesToolStripMenuItem.Name = "getSubtitlesToolStripMenuItem";
            this.getSubtitlesToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.getSubtitlesToolStripMenuItem.Tag = "subtitle";
            this.getSubtitlesToolStripMenuItem.Text = "Get Subtitles";
            // 
            // torrentThToolStripMenuItem
            // 
            this.torrentThToolStripMenuItem.Name = "torrentThToolStripMenuItem";
            this.torrentThToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.torrentThToolStripMenuItem.Tag = "torrent";
            this.torrentThToolStripMenuItem.Text = "Torrent this";
            // 
            // newzbinThisToolStripMenuItem
            // 
            this.newzbinThisToolStripMenuItem.Name = "newzbinThisToolStripMenuItem";
            this.newzbinThisToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.newzbinThisToolStripMenuItem.Tag = "newzbin";
            this.newzbinThisToolStripMenuItem.Text = "Newzbin this";
            // 
            // ResetUserSelToolStripMenuItem
            // 
            this.ResetUserSelToolStripMenuItem.Name = "ResetUserSelToolStripMenuItem";
            this.ResetUserSelToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.ResetUserSelToolStripMenuItem.Tag = "resetUserSelections";
            this.ResetUserSelToolStripMenuItem.Text = "Reset User Selections";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(181, 6);
            // 
            // contextMenuStrip_InsertFields
            // 
            this.contextMenuStrip_InsertFields.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.contextMenuStrip_InsertFields.Name = "contextMenuStrip_SeriesFields";
            this.contextMenuStrip_InsertFields.Size = new System.Drawing.Size(61, 4);
            this.contextMenuStrip_InsertFields.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip_SeriesFields_ItemClicked);
            this.contextMenuStrip_InsertFields.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_SeriesFields_Opening);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "info.png");
            this.imageList1.Images.SetKeyName(1, "computer.png");
            this.imageList1.Images.SetKeyName(2, "view.png");
            this.imageList1.Images.SetKeyName(3, "table.png");
            this.imageList1.Images.SetKeyName(4, "information.png");
            this.imageList1.Images.SetKeyName(5, "logos.png");
            this.imageList1.Images.SetKeyName(6, "format.png");
            this.imageList1.Images.SetKeyName(7, "Downloader.png");
            this.imageList1.Images.SetKeyName(8, "Internet.png");
            this.imageList1.Images.SetKeyName(9, "Settings.png");
            this.imageList1.Images.SetKeyName(10, "Netvibes.png");
            // 
            // checkBox_ShowHidden
            // 
            this.checkBox_ShowHidden.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_ShowHidden.AutoSize = true;
            this.checkBox_ShowHidden.Location = new System.Drawing.Point(3, 493);
            this.checkBox_ShowHidden.Name = "checkBox_ShowHidden";
            this.checkBox_ShowHidden.Size = new System.Drawing.Size(118, 17);
            this.checkBox_ShowHidden.TabIndex = 1;
            this.checkBox_ShowHidden.Tag = "z";
            this.checkBox_ShowHidden.Text = "&Show Hidden Items";
            this.toolTip_Help.SetToolTip(this.checkBox_ShowHidden, "Shows the Series,Seasons and Episodes that have been hidden from view");
            this.checkBox_ShowHidden.UseVisualStyleBackColor = true;
            this.checkBox_ShowHidden.CheckedChanged += new System.EventHandler(this.checkBox_ShowHidden_CheckedChanged);
            // 
            // textBox_dblocation
            // 
            this.textBox_dblocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_dblocation.Location = new System.Drawing.Point(198, 2);
            this.textBox_dblocation.Name = "textBox_dblocation";
            this.textBox_dblocation.ReadOnly = true;
            this.textBox_dblocation.Size = new System.Drawing.Size(268, 20);
            this.textBox_dblocation.TabIndex = 1;
            this.toolTip_Help.SetToolTip(this.textBox_dblocation, "The Database Path used by MP-TVSeries to store all TV information and settings fo" +
                    "r this plugin");
            // 
            // lblClearDB
            // 
            this.lblClearDB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblClearDB.AutoSize = true;
            this.lblClearDB.Location = new System.Drawing.Point(504, 5);
            this.lblClearDB.Name = "lblClearDB";
            this.lblClearDB.Size = new System.Drawing.Size(36, 13);
            this.lblClearDB.TabIndex = 3;
            this.lblClearDB.TabStop = true;
            this.lblClearDB.Text = "Empty";
            this.toolTip_Help.SetToolTip(this.lblClearDB, "Deletes all Series, Season and Episode information from the local database");
            this.lblClearDB.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblClearDB_LinkClicked);
            // 
            // buildExpr
            // 
            this.buildExpr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buildExpr.AutoSize = true;
            this.buildExpr.Location = new System.Drawing.Point(126, 219);
            this.buildExpr.Name = "buildExpr";
            this.buildExpr.Size = new System.Drawing.Size(30, 13);
            this.buildExpr.TabIndex = 4;
            this.buildExpr.TabStop = true;
            this.buildExpr.Text = "Build";
            this.toolTip_Help.SetToolTip(this.buildExpr, "A Simple Expression Builder that can be used to help the plug-in understand your " +
                    "TV Shows file structure");
            this.buildExpr.Visible = false;
            this.buildExpr.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.buildExpr_LinkClicked);
            // 
            // resetExpr
            // 
            this.resetExpr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.resetExpr.AutoSize = true;
            this.resetExpr.Location = new System.Drawing.Point(126, 180);
            this.resetExpr.Name = "resetExpr";
            this.resetExpr.Size = new System.Drawing.Size(35, 13);
            this.resetExpr.TabIndex = 3;
            this.resetExpr.TabStop = true;
            this.resetExpr.Text = "Reset";
            this.toolTip_Help.SetToolTip(this.resetExpr, "Removes all custom defined expressions and restores the plug-in defaults");
            this.resetExpr.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.resetExpr_LinkClicked);
            // 
            // dataGridView_Replace
            // 
            this.dataGridView_Replace.AllowUserToResizeColumns = false;
            this.dataGridView_Replace.AllowUserToResizeRows = false;
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_Replace.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle19;
            this.dataGridView_Replace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_Replace.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView_Replace.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView_Replace.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle20.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle20.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle20.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle20.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle20.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Replace.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle20;
            this.dataGridView_Replace.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle21.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle21.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle21.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle21.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle21.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle21.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Replace.DefaultCellStyle = dataGridViewCellStyle21;
            this.dataGridView_Replace.Location = new System.Drawing.Point(0, 75);
            this.dataGridView_Replace.MultiSelect = false;
            this.dataGridView_Replace.Name = "dataGridView_Replace";
            this.dataGridView_Replace.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle22.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle22.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle22.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle22.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle22.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Replace.RowHeadersDefaultCellStyle = dataGridViewCellStyle22;
            this.dataGridView_Replace.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_Replace.RowsDefaultCellStyle = dataGridViewCellStyle23;
            this.dataGridView_Replace.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_Replace.RowTemplate.Height = 18;
            this.dataGridView_Replace.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Replace.Size = new System.Drawing.Size(124, 77);
            this.dataGridView_Replace.StandardTab = true;
            this.dataGridView_Replace.TabIndex = 0;
            this.toolTip_Help.SetToolTip(this.dataGridView_Replace, "Strings that are to be replaced before or after parsing data to online database l" +
                    "ookup");
            this.dataGridView_Replace.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView_Replace_UserDeletedRow);
            this.dataGridView_Replace.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_Replace_CellEndEdit);
            this.dataGridView_Replace.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView_Replace_DefaultValuesNeeded);
            // 
            // button_TestReparse
            // 
            this.button_TestReparse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_TestReparse.Image = ((System.Drawing.Image)(resources.GetObject("button_TestReparse.Image")));
            this.button_TestReparse.Location = new System.Drawing.Point(399, 158);
            this.button_TestReparse.Name = "button_TestReparse";
            this.button_TestReparse.Size = new System.Drawing.Size(28, 31);
            this.button_TestReparse.TabIndex = 1;
            this.toolTip_Help.SetToolTip(this.button_TestReparse, "Redo a local parsing test");
            this.button_TestReparse.UseVisualStyleBackColor = true;
            this.button_TestReparse.Click += new System.EventHandler(this.button_TestReparse_Click);
            // 
            // button_Start
            // 
            this.button_Start.Location = new System.Drawing.Point(21, 5);
            this.button_Start.Name = "button_Start";
            this.button_Start.Size = new System.Drawing.Size(105, 23);
            this.button_Start.TabIndex = 4;
            this.button_Start.Text = "&Start Import";
            this.toolTip_Help.SetToolTip(this.button_Start, "Click to Start Importing data from the Online Database, updates data for all seri" +
                    "es, seasons and episodes new and old");
            this.button_Start.UseVisualStyleBackColor = true;
            this.button_Start.Click += new System.EventHandler(this.button_Start_Click);
            // 
            // linkMediaInfoUpdate
            // 
            this.linkMediaInfoUpdate.AutoSize = true;
            this.linkMediaInfoUpdate.Location = new System.Drawing.Point(165, 454);
            this.linkMediaInfoUpdate.Name = "linkMediaInfoUpdate";
            this.linkMediaInfoUpdate.Size = new System.Drawing.Size(95, 13);
            this.linkMediaInfoUpdate.TabIndex = 28;
            this.linkMediaInfoUpdate.TabStop = true;
            this.linkMediaInfoUpdate.Text = "Update Media Info";
            this.toolTip_Help.SetToolTip(this.linkMediaInfoUpdate, "Click here to force the plug-in to read the media information for local files aga" +
                    "in e.g. resolution, codecs...");
            this.linkMediaInfoUpdate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkMediaInfoUpdate_LinkClicked);
            // 
            // checkBox_AutoOnlineDataRefresh
            // 
            this.checkBox_AutoOnlineDataRefresh.AutoSize = true;
            this.checkBox_AutoOnlineDataRefresh.Location = new System.Drawing.Point(8, 143);
            this.checkBox_AutoOnlineDataRefresh.Name = "checkBox_AutoOnlineDataRefresh";
            this.checkBox_AutoOnlineDataRefresh.Size = new System.Drawing.Size(328, 17);
            this.checkBox_AutoOnlineDataRefresh.TabIndex = 9;
            this.checkBox_AutoOnlineDataRefresh.Text = "A&utomatically download updated data from theTVDB.com every:";
            this.toolTip_Help.SetToolTip(this.checkBox_AutoOnlineDataRefresh, "Enable this option to make the plug-in automatically ask for a refresh of the onl" +
                    "ine data every x hours");
            this.checkBox_AutoOnlineDataRefresh.UseVisualStyleBackColor = true;
            this.checkBox_AutoOnlineDataRefresh.CheckedChanged += new System.EventHandler(this.checkBox_AutoOnlineDataRefresh_CheckedChanged);
            // 
            // checkBox_AutoChooseOrder
            // 
            this.checkBox_AutoChooseOrder.AutoSize = true;
            this.checkBox_AutoChooseOrder.Location = new System.Drawing.Point(8, 188);
            this.checkBox_AutoChooseOrder.Name = "checkBox_AutoChooseOrder";
            this.checkBox_AutoChooseOrder.Size = new System.Drawing.Size(331, 17);
            this.checkBox_AutoChooseOrder.TabIndex = 13;
            this.checkBox_AutoChooseOrder.Text = "Automatically choose Aired when multiple &orders are found online";
            this.toolTip_Help.SetToolTip(this.checkBox_AutoChooseOrder, "Enable this option to automatically select Aired order when multiple orders are f" +
                    "ound online e.g. DVD or Absolute");
            this.checkBox_AutoChooseOrder.UseVisualStyleBackColor = true;
            this.checkBox_AutoChooseOrder.CheckedChanged += new System.EventHandler(this.checkBox_AutoChooseOrder_CheckedChanged);
            // 
            // txtUserID
            // 
            this.txtUserID.Location = new System.Drawing.Point(127, 55);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.Size = new System.Drawing.Size(189, 20);
            this.txtUserID.TabIndex = 4;
            this.toolTip_Help.SetToolTip(this.txtUserID, "Enter you\'re theTVDB Account Identifier here (Note: this is NOT you\'re username)");
            this.txtUserID.TextChanged += new System.EventHandler(this.txtUserID_TextChanged);
            // 
            // checkDownloadEpisodeSnapshots
            // 
            this.checkDownloadEpisodeSnapshots.AutoSize = true;
            this.checkDownloadEpisodeSnapshots.Location = new System.Drawing.Point(8, 234);
            this.checkDownloadEpisodeSnapshots.Name = "checkDownloadEpisodeSnapshots";
            this.checkDownloadEpisodeSnapshots.Size = new System.Drawing.Size(172, 17);
            this.checkDownloadEpisodeSnapshots.TabIndex = 15;
            this.checkDownloadEpisodeSnapshots.Text = "Download Episode &Thumbnails";
            this.toolTip_Help.SetToolTip(this.checkDownloadEpisodeSnapshots, "Enable to download Episode Thumbnails if available.");
            this.checkDownloadEpisodeSnapshots.UseVisualStyleBackColor = true;
            this.checkDownloadEpisodeSnapshots.CheckedChanged += new System.EventHandler(this.checkDownloadEpisodeSnapshots_CheckedChanged);
            // 
            // chkBlankBanners
            // 
            this.chkBlankBanners.AutoSize = true;
            this.chkBlankBanners.Location = new System.Drawing.Point(8, 257);
            this.chkBlankBanners.Name = "chkBlankBanners";
            this.chkBlankBanners.Size = new System.Drawing.Size(237, 17);
            this.chkBlankBanners.TabIndex = 16;
            this.chkBlankBanners.Text = "Download &Blank Series and Season banners";
            this.toolTip_Help.SetToolTip(this.chkBlankBanners, "Enable to download blank series and season banners in addition to localized and E" +
                    "nglish ones");
            this.chkBlankBanners.UseVisualStyleBackColor = true;
            this.chkBlankBanners.CheckedChanged += new System.EventHandler(this.chkBlankBanners_CheckedChanged);
            // 
            // txtMainMirror
            // 
            this.txtMainMirror.Location = new System.Drawing.Point(127, 29);
            this.txtMainMirror.Name = "txtMainMirror";
            this.txtMainMirror.Size = new System.Drawing.Size(189, 20);
            this.txtMainMirror.TabIndex = 2;
            this.toolTip_Help.SetToolTip(this.txtMainMirror, "Download mirror for online database");
            this.txtMainMirror.TextChanged += new System.EventHandler(this.txtMainMirror_TextChanged);
            // 
            // linkDelUpdateTime
            // 
            this.linkDelUpdateTime.AutoSize = true;
            this.linkDelUpdateTime.Location = new System.Drawing.Point(18, 454);
            this.linkDelUpdateTime.Name = "linkDelUpdateTime";
            this.linkDelUpdateTime.Size = new System.Drawing.Size(128, 13);
            this.linkDelUpdateTime.TabIndex = 27;
            this.linkDelUpdateTime.TabStop = true;
            this.linkDelUpdateTime.Text = "Clear Update Timestamps";
            this.toolTip_Help.SetToolTip(this.linkDelUpdateTime, "Click here to reset Timestamps for the last date-time data for series was updated" +
                    " from the online database.\r\nThis will force a re-request of all data on next imp" +
                    "ort");
            this.linkDelUpdateTime.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDelUpdateTime_LinkClicked);
            // 
            // comboOnlineLang
            // 
            this.comboOnlineLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboOnlineLang.FormattingEnabled = true;
            this.comboOnlineLang.Location = new System.Drawing.Point(127, 82);
            this.comboOnlineLang.Name = "comboOnlineLang";
            this.comboOnlineLang.Size = new System.Drawing.Size(121, 21);
            this.comboOnlineLang.TabIndex = 7;
            this.toolTip_Help.SetToolTip(this.comboOnlineLang, "Select the language to download TV Series information in, defaults to English");
            this.comboOnlineLang.SelectedIndexChanged += new System.EventHandler(this.comboOnlineLang_SelectedIndexChanged);
            this.comboOnlineLang.DropDown += new System.EventHandler(this.comboOnlineLang_DropDown);
            // 
            // checkBox_OnlineSearch
            // 
            this.checkBox_OnlineSearch.AutoSize = true;
            this.checkBox_OnlineSearch.Location = new System.Drawing.Point(8, 7);
            this.checkBox_OnlineSearch.Name = "checkBox_OnlineSearch";
            this.checkBox_OnlineSearch.Size = new System.Drawing.Size(314, 17);
            this.checkBox_OnlineSearch.TabIndex = 0;
            this.checkBox_OnlineSearch.Text = "&Download data from the TV Online Database (theTVDB.com)";
            this.toolTip_Help.SetToolTip(this.checkBox_OnlineSearch, "Enable this option to download data for TV Series including Banners and Episode t" +
                    "humbnails.\r\nData can be manually entered or corrected in Details tab");
            this.checkBox_OnlineSearch.UseVisualStyleBackColor = true;
            this.checkBox_OnlineSearch.CheckedChanged += new System.EventHandler(this.checkBox_OnlineSearch_CheckedChanged);
            // 
            // checkBox_AutoChooseSeries
            // 
            this.checkBox_AutoChooseSeries.AutoSize = true;
            this.checkBox_AutoChooseSeries.Location = new System.Drawing.Point(8, 165);
            this.checkBox_AutoChooseSeries.Name = "checkBox_AutoChooseSeries";
            this.checkBox_AutoChooseSeries.Size = new System.Drawing.Size(334, 17);
            this.checkBox_AutoChooseSeries.TabIndex = 12;
            this.checkBox_AutoChooseSeries.Text = "Automatically &choose Series when an exact match is found online";
            this.toolTip_Help.SetToolTip(this.checkBox_AutoChooseSeries, resources.GetString("checkBox_AutoChooseSeries.ToolTip"));
            this.checkBox_AutoChooseSeries.UseVisualStyleBackColor = true;
            this.checkBox_AutoChooseSeries.CheckedChanged += new System.EventHandler(this.checkBox_AutoChooseSeries_CheckedChanged);
            // 
            // checkBox_FullSeriesRetrieval
            // 
            this.checkBox_FullSeriesRetrieval.AutoSize = true;
            this.checkBox_FullSeriesRetrieval.Location = new System.Drawing.Point(8, 211);
            this.checkBox_FullSeriesRetrieval.Name = "checkBox_FullSeriesRetrieval";
            this.checkBox_FullSeriesRetrieval.Size = new System.Drawing.Size(263, 17);
            this.checkBox_FullSeriesRetrieval.TabIndex = 14;
            this.checkBox_FullSeriesRetrieval.Text = "Download &Episode information for the whole series";
            this.toolTip_Help.SetToolTip(this.checkBox_FullSeriesRetrieval, "Enable this option to download data for all seasons and episodes available online" +
                    " (takes longer).\r\nDisable to download data only for episodes local to this compu" +
                    "ter");
            this.checkBox_FullSeriesRetrieval.UseVisualStyleBackColor = true;
            this.checkBox_FullSeriesRetrieval.CheckedChanged += new System.EventHandler(this.checkBox_FullSeriesRetrieval_CheckedChanged);
            // 
            // linkExWatched
            // 
            this.linkExWatched.AutoSize = true;
            this.linkExWatched.Location = new System.Drawing.Point(401, 101);
            this.linkExWatched.Name = "linkExWatched";
            this.linkExWatched.Size = new System.Drawing.Size(121, 13);
            this.linkExWatched.TabIndex = 25;
            this.linkExWatched.TabStop = true;
            this.linkExWatched.Text = "Export Watched Flags...";
            this.toolTip_Help.SetToolTip(this.linkExWatched, "Export the \'Watched\' Status of all episodes to file");
            this.linkExWatched.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkExWatched_LinkClicked);
            // 
            // linkImpWatched
            // 
            this.linkImpWatched.AutoSize = true;
            this.linkImpWatched.Location = new System.Drawing.Point(401, 78);
            this.linkImpWatched.Name = "linkImpWatched";
            this.linkImpWatched.Size = new System.Drawing.Size(120, 13);
            this.linkImpWatched.TabIndex = 24;
            this.linkImpWatched.TabStop = true;
            this.linkImpWatched.Text = "Import Watched Flags...";
            this.toolTip_Help.SetToolTip(this.linkImpWatched, "Import the \'Watched\' Status of all episodes from file");
            this.linkImpWatched.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkImpWatched_LinkClicked);
            // 
            // chkAllowDeletes
            // 
            this.chkAllowDeletes.Location = new System.Drawing.Point(404, 261);
            this.chkAllowDeletes.Name = "chkAllowDeletes";
            this.chkAllowDeletes.Size = new System.Drawing.Size(417, 17);
            this.chkAllowDeletes.TabIndex = 19;
            this.chkAllowDeletes.Text = "Allow user to delete &files from the GUI context menu";
            this.toolTip_Help.SetToolTip(this.chkAllowDeletes, "Enable this option to allow users to delete items using the context menu from wit" +
                    "h-in Media Portal");
            this.chkAllowDeletes.UseVisualStyleBackColor = true;
            this.chkAllowDeletes.CheckedChanged += new System.EventHandler(this.chkAllowDeletes_CheckedChanged);
            // 
            // checkBox_doFolderWatch
            // 
            this.checkBox_doFolderWatch.Location = new System.Drawing.Point(11, 284);
            this.checkBox_doFolderWatch.Name = "checkBox_doFolderWatch";
            this.checkBox_doFolderWatch.Size = new System.Drawing.Size(385, 17);
            this.checkBox_doFolderWatch.TabIndex = 18;
            this.checkBox_doFolderWatch.Text = "Watch my &Import folders for changes automatically";
            this.toolTip_Help.SetToolTip(this.checkBox_doFolderWatch, "Enable this option to allow the plug-in to monitor import folders for new, delete" +
                    "d and modified TV Series and update the database accordingly.\r\nThis option only " +
                    "works from with-in Media Portal");
            this.checkBox_doFolderWatch.UseVisualStyleBackColor = true;
            this.checkBox_doFolderWatch.CheckedChanged += new System.EventHandler(this.checkBox_doFolderWatch_CheckedChanged);
            // 
            // checkBox_DontClearMissingLocalFiles
            // 
            this.checkBox_DontClearMissingLocalFiles.Location = new System.Drawing.Point(11, 261);
            this.checkBox_DontClearMissingLocalFiles.Name = "checkBox_DontClearMissingLocalFiles";
            this.checkBox_DontClearMissingLocalFiles.Size = new System.Drawing.Size(417, 17);
            this.checkBox_DontClearMissingLocalFiles.TabIndex = 17;
            this.checkBox_DontClearMissingLocalFiles.Text = "&Keep reference to files that are no longer available on computer";
            this.toolTip_Help.SetToolTip(this.checkBox_DontClearMissingLocalFiles, "Enable this option to keep a reference to files that are no longer available duri" +
                    "ng scan e.g. series stored on an offline fileserver");
            this.checkBox_DontClearMissingLocalFiles.UseVisualStyleBackColor = true;
            this.checkBox_DontClearMissingLocalFiles.CheckedChanged += new System.EventHandler(this.checkBox_DontClearMissingLocalFiles_CheckedChanged);
            // 
            // checkBox_Series_UseSortName
            // 
            this.checkBox_Series_UseSortName.AutoSize = true;
            this.checkBox_Series_UseSortName.Location = new System.Drawing.Point(11, 238);
            this.checkBox_Series_UseSortName.Name = "checkBox_Series_UseSortName";
            this.checkBox_Series_UseSortName.Size = new System.Drawing.Size(304, 17);
            this.checkBox_Series_UseSortName.TabIndex = 16;
            this.checkBox_Series_UseSortName.Text = "So&rt Series using the Sort Name instead of the Pretty Name";
            this.toolTip_Help.SetToolTip(this.checkBox_Series_UseSortName, "Enable to sort Series names using the Sort (Original) Name found in the Details t" +
                    "ab");
            this.checkBox_Series_UseSortName.UseVisualStyleBackColor = true;
            this.checkBox_Series_UseSortName.CheckedChanged += new System.EventHandler(this.checkBox_Series_UseSortName_CheckedChanged);
            // 
            // nudWatchedAfter
            // 
            this.nudWatchedAfter.Location = new System.Drawing.Point(159, 46);
            this.nudWatchedAfter.Name = "nudWatchedAfter";
            this.nudWatchedAfter.Size = new System.Drawing.Size(44, 20);
            this.nudWatchedAfter.TabIndex = 3;
            this.toolTip_Help.SetToolTip(this.nudWatchedAfter, "Enter the percentage of the episode that has been viewed to consider it as watche" +
                    "d.\r\nEpisode foreground color will change to indicated that it is watched");
            this.nudWatchedAfter.Value = new decimal(new int[] {
            95,
            0,
            0,
            0});
            this.nudWatchedAfter.ValueChanged += new System.EventHandler(this.nudWatchedAfter_ValueChanged);
            // 
            // comboLanguage
            // 
            this.comboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLanguage.FormattingEnabled = true;
            this.comboLanguage.Location = new System.Drawing.Point(402, 19);
            this.comboLanguage.Name = "comboLanguage";
            this.comboLanguage.Size = new System.Drawing.Size(119, 21);
            this.comboLanguage.TabIndex = 21;
            this.toolTip_Help.SetToolTip(this.comboLanguage, "Select the language that the plugin user interface will be displayed in Media Por" +
                    "tal.\r\nThis does not control the language of downloaded data from the online data" +
                    "base");
            this.comboLanguage.SelectedIndexChanged += new System.EventHandler(this.comboLanguage_SelectedIndexChanged);
            // 
            // checkBox_RandBanner
            // 
            this.checkBox_RandBanner.AutoSize = true;
            this.checkBox_RandBanner.Location = new System.Drawing.Point(404, 146);
            this.checkBox_RandBanner.Name = "checkBox_RandBanner";
            this.checkBox_RandBanner.Size = new System.Drawing.Size(261, 17);
            this.checkBox_RandBanner.TabIndex = 27;
            this.checkBox_RandBanner.Text = "&Display random Artwork in series and season view";
            this.toolTip_Help.SetToolTip(this.checkBox_RandBanner, "Enable this option to display a random banner when entering series/season view");
            this.checkBox_RandBanner.UseVisualStyleBackColor = true;
            this.checkBox_RandBanner.CheckedChanged += new System.EventHandler(this.checkBox_RandBanner_CheckedChanged);
            // 
            // textBox_PluginHomeName
            // 
            this.textBox_PluginHomeName.Location = new System.Drawing.Point(159, 20);
            this.textBox_PluginHomeName.Name = "textBox_PluginHomeName";
            this.textBox_PluginHomeName.Size = new System.Drawing.Size(164, 20);
            this.textBox_PluginHomeName.TabIndex = 1;
            this.toolTip_Help.SetToolTip(this.textBox_PluginHomeName, "Enter the name for the plug-in as listed in the Media Portal Home screen");
            this.textBox_PluginHomeName.TextChanged += new System.EventHandler(this.textBox_PluginHomeName_TextChanged);
            // 
            // btnLogoDown
            // 
            this.btnLogoDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogoDown.Image = global::WindowPlugins.GUITVSeries.Properties.Resources.arrow_down;
            this.btnLogoDown.Location = new System.Drawing.Point(710, 106);
            this.btnLogoDown.Name = "btnLogoDown";
            this.btnLogoDown.Size = new System.Drawing.Size(32, 29);
            this.btnLogoDown.TabIndex = 2;
            this.toolTip_Help.SetToolTip(this.btnLogoDown, "Moves the logo position to the right when displayed in Media Portal");
            this.btnLogoDown.UseVisualStyleBackColor = true;
            this.btnLogoDown.Click += new System.EventHandler(this.btnLogoDown_Click);
            // 
            // btnlogoUp
            // 
            this.btnlogoUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnlogoUp.Image = global::WindowPlugins.GUITVSeries.Properties.Resources.arrow_up;
            this.btnlogoUp.Location = new System.Drawing.Point(709, 71);
            this.btnlogoUp.Name = "btnlogoUp";
            this.btnlogoUp.Size = new System.Drawing.Size(32, 29);
            this.btnlogoUp.TabIndex = 1;
            this.toolTip_Help.SetToolTip(this.btnlogoUp, "Moves the logo position to the left when displayed in Media Portal");
            this.btnlogoUp.UseVisualStyleBackColor = true;
            this.btnlogoUp.Click += new System.EventHandler(this.btnlogoUp_Click);
            // 
            // lnkLogoImp
            // 
            this.lnkLogoImp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkLogoImp.Location = new System.Drawing.Point(92, 439);
            this.lnkLogoImp.Name = "lnkLogoImp";
            this.lnkLogoImp.Size = new System.Drawing.Size(51, 13);
            this.lnkLogoImp.TabIndex = 4;
            this.lnkLogoImp.TabStop = true;
            this.lnkLogoImp.Text = "Import...";
            this.lnkLogoImp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip_Help.SetToolTip(this.lnkLogoImp, "Click to import logo rules from file");
            this.lnkLogoImp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkLogoImp_LinkClicked);
            // 
            // lstLogos
            // 
            this.lstLogos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstLogos.FormattingEnabled = true;
            this.lstLogos.HorizontalScrollbar = true;
            this.lstLogos.Location = new System.Drawing.Point(7, 19);
            this.lstLogos.Name = "lstLogos";
            this.lstLogos.ScrollAlwaysVisible = true;
            this.lstLogos.Size = new System.Drawing.Size(693, 407);
            this.lstLogos.TabIndex = 0;
            this.toolTip_Help.SetToolTip(this.lstLogos, "Contains the list of Logo rules defined to display in Media Portal");
            // 
            // btnLogoTemplate
            // 
            this.btnLogoTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLogoTemplate.Location = new System.Drawing.Point(7, 435);
            this.btnLogoTemplate.Name = "btnLogoTemplate";
            this.btnLogoTemplate.Size = new System.Drawing.Size(75, 23);
            this.btnLogoTemplate.TabIndex = 3;
            this.btnLogoTemplate.Text = "&Templates...";
            this.toolTip_Help.SetToolTip(this.btnLogoTemplate, "Click button to select from a pre-defined list of logo templates.\r\nA Logo package" +
                    " must be installed to display logos succesfully in Media Portal");
            this.btnLogoTemplate.UseVisualStyleBackColor = true;
            this.btnLogoTemplate.Click += new System.EventHandler(this.btnLogoTemplate_Click);
            // 
            // btnrmvLogo
            // 
            this.btnrmvLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnrmvLogo.Location = new System.Drawing.Point(377, 434);
            this.btnrmvLogo.Name = "btnrmvLogo";
            this.btnrmvLogo.Size = new System.Drawing.Size(75, 23);
            this.btnrmvLogo.TabIndex = 7;
            this.btnrmvLogo.Text = "&Delete";
            this.toolTip_Help.SetToolTip(this.btnrmvLogo, "Click to remove the selected logo rule in list");
            this.btnrmvLogo.UseVisualStyleBackColor = true;
            this.btnrmvLogo.Click += new System.EventHandler(this.btnrmvLogo_Click);
            // 
            // lnkLogoExport
            // 
            this.lnkLogoExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkLogoExport.Location = new System.Drawing.Point(145, 439);
            this.lnkLogoExport.Name = "lnkLogoExport";
            this.lnkLogoExport.Size = new System.Drawing.Size(51, 13);
            this.lnkLogoExport.TabIndex = 5;
            this.lnkLogoExport.TabStop = true;
            this.lnkLogoExport.Text = "Export...";
            this.lnkLogoExport.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip_Help.SetToolTip(this.lnkLogoExport, "Click to export logo rules to file");
            this.lnkLogoExport.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkLogoExport_LinkClicked);
            // 
            // btnLogoEdit
            // 
            this.btnLogoEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogoEdit.Location = new System.Drawing.Point(544, 434);
            this.btnLogoEdit.Name = "btnLogoEdit";
            this.btnLogoEdit.Size = new System.Drawing.Size(75, 23);
            this.btnLogoEdit.TabIndex = 8;
            this.btnLogoEdit.Text = "&Edit...";
            this.toolTip_Help.SetToolTip(this.btnLogoEdit, "Click to edit the selected logo rule in list");
            this.btnLogoEdit.UseVisualStyleBackColor = true;
            this.btnLogoEdit.Click += new System.EventHandler(this.btnLogoEdit_Click);
            // 
            // addLogo
            // 
            this.addLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addLogo.Location = new System.Drawing.Point(625, 434);
            this.addLogo.Name = "addLogo";
            this.addLogo.Size = new System.Drawing.Size(75, 23);
            this.addLogo.TabIndex = 9;
            this.addLogo.Text = "&Add...";
            this.toolTip_Help.SetToolTip(this.addLogo, resources.GetString("addLogo.ToolTip"));
            this.addLogo.UseVisualStyleBackColor = true;
            this.addLogo.Click += new System.EventHandler(this.addLogo_Click);
            // 
            // textBox_NewsSearchReportRegex
            // 
            this.textBox_NewsSearchReportRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_NewsSearchReportRegex.Location = new System.Drawing.Point(147, 91);
            this.textBox_NewsSearchReportRegex.Name = "textBox_NewsSearchReportRegex";
            this.textBox_NewsSearchReportRegex.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_NewsSearchReportRegex.Size = new System.Drawing.Size(322, 20);
            this.textBox_NewsSearchReportRegex.TabIndex = 7;
            this.toolTip_Help.SetToolTip(this.textBox_NewsSearchReportRegex, "ID is the only important tag used to download");
            this.textBox_NewsSearchReportRegex.TextChanged += new System.EventHandler(this.textBox_NewsSearchRegex_TextChanged);
            // 
            // textBox_uTorrentPath
            // 
            this.textBox_uTorrentPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_uTorrentPath.Location = new System.Drawing.Point(147, 11);
            this.textBox_uTorrentPath.Name = "textBox_uTorrentPath";
            this.textBox_uTorrentPath.Size = new System.Drawing.Size(37, 20);
            this.textBox_uTorrentPath.TabIndex = 1;
            this.toolTip_Help.SetToolTip(this.textBox_uTorrentPath, "Enter path to your uTorrent executable");
            this.textBox_uTorrentPath.TextChanged += new System.EventHandler(this.textBox_uTorrentPath_TextChanged);
            // 
            // textBox_TorrentDetailsRegex
            // 
            this.textBox_TorrentDetailsRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_TorrentDetailsRegex.Location = new System.Drawing.Point(147, 146);
            this.textBox_TorrentDetailsRegex.Multiline = true;
            this.textBox_TorrentDetailsRegex.Name = "textBox_TorrentDetailsRegex";
            this.textBox_TorrentDetailsRegex.Size = new System.Drawing.Size(68, 51);
            this.textBox_TorrentDetailsRegex.TabIndex = 9;
            this.toolTip_Help.SetToolTip(this.textBox_TorrentDetailsRegex, resources.GetString("textBox_TorrentDetailsRegex.ToolTip"));
            this.textBox_TorrentDetailsRegex.TextChanged += new System.EventHandler(this.textBox_TorrentDetailsRegex_TextChanged);
            // 
            // textBox_TorrentSearchRegex
            // 
            this.textBox_TorrentSearchRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_TorrentSearchRegex.Location = new System.Drawing.Point(147, 61);
            this.textBox_TorrentSearchRegex.Multiline = true;
            this.textBox_TorrentSearchRegex.Name = "textBox_TorrentSearchRegex";
            this.textBox_TorrentSearchRegex.Size = new System.Drawing.Size(68, 51);
            this.textBox_TorrentSearchRegex.TabIndex = 5;
            this.toolTip_Help.SetToolTip(this.textBox_TorrentSearchRegex, resources.GetString("textBox_TorrentSearchRegex.ToolTip"));
            this.textBox_TorrentSearchRegex.TextChanged += new System.EventHandler(this.textBox_TorrentRegex_TextChanged);
            // 
            // textBox_TorrentDetailsUrl
            // 
            this.textBox_TorrentDetailsUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_TorrentDetailsUrl.Location = new System.Drawing.Point(147, 120);
            this.textBox_TorrentDetailsUrl.Name = "textBox_TorrentDetailsUrl";
            this.textBox_TorrentDetailsUrl.Size = new System.Drawing.Size(68, 20);
            this.textBox_TorrentDetailsUrl.TabIndex = 7;
            this.toolTip_Help.SetToolTip(this.textBox_TorrentDetailsUrl, "Url of the website showing details of a specific torrent.\r\nThis url will replace " +
                    "the $id$ string with the <id> extracted from the search results");
            this.textBox_TorrentDetailsUrl.TextChanged += new System.EventHandler(this.textBox_TorrentDetailsUrl_TextChanged);
            // 
            // textBox_TorrentSearchUrl
            // 
            this.textBox_TorrentSearchUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_TorrentSearchUrl.Location = new System.Drawing.Point(147, 35);
            this.textBox_TorrentSearchUrl.Name = "textBox_TorrentSearchUrl";
            this.textBox_TorrentSearchUrl.Size = new System.Drawing.Size(68, 20);
            this.textBox_TorrentSearchUrl.TabIndex = 3;
            this.toolTip_Help.SetToolTip(this.textBox_TorrentSearchUrl, "Url of the website listing the search results.\r\nThe search string will replace th" +
                    "e $search$ string in the specified url.");
            this.textBox_TorrentSearchUrl.TextChanged += new System.EventHandler(this.textBox_TorrentUrl_TextChanged);
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button1.Image = global::WindowPlugins.GUITVSeries.Properties.Resources.arrow_up_small;
            this.button1.Location = new System.Drawing.Point(0, 548);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(774, 14);
            this.button1.TabIndex = 0;
            this.toolTip_Help.SetToolTip(this.button1, "Click here to show/hide the log window, useful for diagnosing any errors or watch" +
                    "ing progress of Import");
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBox_ScanOnStartup
            // 
            this.checkBox_ScanOnStartup.AutoSize = true;
            this.checkBox_ScanOnStartup.Location = new System.Drawing.Point(8, 120);
            this.checkBox_ScanOnStartup.Name = "checkBox_ScanOnStartup";
            this.checkBox_ScanOnStartup.Size = new System.Drawing.Size(268, 17);
            this.checkBox_ScanOnStartup.TabIndex = 8;
            this.checkBox_ScanOnStartup.Text = "E&nable local/online scan when starting MediaPortal";
            this.toolTip_Help.SetToolTip(this.checkBox_ScanOnStartup, resources.GetString("checkBox_ScanOnStartup.ToolTip"));
            this.checkBox_ScanOnStartup.UseVisualStyleBackColor = true;
            this.checkBox_ScanOnStartup.CheckedChanged += new System.EventHandler(this.checkBox_ScanOnStartup_CheckedChanged);
            // 
            // checkBox_AutoDownloadMissingArtwork
            // 
            this.checkBox_AutoDownloadMissingArtwork.AutoSize = true;
            this.checkBox_AutoDownloadMissingArtwork.Location = new System.Drawing.Point(8, 303);
            this.checkBox_AutoDownloadMissingArtwork.Name = "checkBox_AutoDownloadMissingArtwork";
            this.checkBox_AutoDownloadMissingArtwork.Size = new System.Drawing.Size(361, 17);
            this.checkBox_AutoDownloadMissingArtwork.TabIndex = 18;
            this.checkBox_AutoDownloadMissingArtwork.Text = "Automatically download missing artwork for existing series on local scan";
            this.toolTip_Help.SetToolTip(this.checkBox_AutoDownloadMissingArtwork, "Only update new artwork when unchecked.  Saves up to 3 minutes per scan.");
            this.checkBox_AutoDownloadMissingArtwork.UseVisualStyleBackColor = true;
            this.checkBox_AutoDownloadMissingArtwork.CheckedChanged += new System.EventHandler(this.checkBox_AutoDownloadMissingArtwork_CheckedChanged);
            // 
            // checkBox_AutoUpdateEpisodeRatings
            // 
            this.checkBox_AutoUpdateEpisodeRatings.AutoSize = true;
            this.checkBox_AutoUpdateEpisodeRatings.Location = new System.Drawing.Point(8, 280);
            this.checkBox_AutoUpdateEpisodeRatings.Name = "checkBox_AutoUpdateEpisodeRatings";
            this.checkBox_AutoUpdateEpisodeRatings.Size = new System.Drawing.Size(348, 17);
            this.checkBox_AutoUpdateEpisodeRatings.TabIndex = 17;
            this.checkBox_AutoUpdateEpisodeRatings.Text = "Automatically update user episode ratings in addition to series ratings";
            this.toolTip_Help.SetToolTip(this.checkBox_AutoUpdateEpisodeRatings, "Only update user series ratings when unchecked.  Saves up to 3 minutes per scan.");
            this.checkBox_AutoUpdateEpisodeRatings.UseVisualStyleBackColor = true;
            this.checkBox_AutoUpdateEpisodeRatings.CheckedChanged += new System.EventHandler(this.checkBox_AutoUpdateEpisodeRatings_CheckedChanged);
            // 
            // linkImpParsingExpressions
            // 
            this.linkImpParsingExpressions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkImpParsingExpressions.AutoSize = true;
            this.linkImpParsingExpressions.Location = new System.Drawing.Point(126, 139);
            this.linkImpParsingExpressions.Name = "linkImpParsingExpressions";
            this.linkImpParsingExpressions.Size = new System.Drawing.Size(36, 13);
            this.linkImpParsingExpressions.TabIndex = 7;
            this.linkImpParsingExpressions.TabStop = true;
            this.linkImpParsingExpressions.Text = "Import";
            this.toolTip_Help.SetToolTip(this.linkImpParsingExpressions, "Import Parsing Expressions from a file");
            this.linkImpParsingExpressions.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkImpParsingExpressions_LinkClicked);
            // 
            // linkExParsingExpressions
            // 
            this.linkExParsingExpressions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkExParsingExpressions.AutoSize = true;
            this.linkExParsingExpressions.Location = new System.Drawing.Point(126, 159);
            this.linkExParsingExpressions.Name = "linkExParsingExpressions";
            this.linkExParsingExpressions.Size = new System.Drawing.Size(37, 13);
            this.linkExParsingExpressions.TabIndex = 8;
            this.linkExParsingExpressions.TabStop = true;
            this.linkExParsingExpressions.Text = "Export";
            this.toolTip_Help.SetToolTip(this.linkExParsingExpressions, "Export Parsing Expressions to a file");
            this.linkExParsingExpressions.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkExParsingExpressions_LinkClicked);
            // 
            // checkbox_SortSpecials
            // 
            this.checkbox_SortSpecials.AutoSize = true;
            this.checkbox_SortSpecials.Location = new System.Drawing.Point(404, 123);
            this.checkbox_SortSpecials.Name = "checkbox_SortSpecials";
            this.checkbox_SortSpecials.Size = new System.Drawing.Size(155, 17);
            this.checkbox_SortSpecials.TabIndex = 26;
            this.checkbox_SortSpecials.Text = "Sort Specials in Episode list";
            this.toolTip_Help.SetToolTip(this.checkbox_SortSpecials, "Enable this option to sort specials in the main episode list by their correspondi" +
                    "ng air date");
            this.checkbox_SortSpecials.UseVisualStyleBackColor = true;
            this.checkbox_SortSpecials.CheckedChanged += new System.EventHandler(this.checkbox_SortSpecials_CheckedChanged);
            // 
            // checkboxRatingDisplayStars
            // 
            this.checkboxRatingDisplayStars.AutoSize = true;
            this.checkboxRatingDisplayStars.Location = new System.Drawing.Point(11, 146);
            this.checkboxRatingDisplayStars.Name = "checkboxRatingDisplayStars";
            this.checkboxRatingDisplayStars.Size = new System.Drawing.Size(143, 17);
            this.checkboxRatingDisplayStars.TabIndex = 12;
            this.checkboxRatingDisplayStars.Text = "Show 5 Star Rate Dialog";
            this.toolTip_Help.SetToolTip(this.checkboxRatingDisplayStars, "Enable this option to dislay a 5 Star Rate Dialog instead of 10 Stars, theTVDB.co" +
                    "m accepts 10 Stars\r\nso 5 Stars will be rounded up");
            this.checkboxRatingDisplayStars.UseVisualStyleBackColor = true;
            this.checkboxRatingDisplayStars.CheckedChanged += new System.EventHandler(this.checkboxRatingDisplayStars_CheckedChanged);
            // 
            // checkboxAutoDownloadFanartSeriesName
            // 
            this.checkboxAutoDownloadFanartSeriesName.AutoSize = true;
            this.checkboxAutoDownloadFanartSeriesName.Location = new System.Drawing.Point(33, 425);
            this.checkboxAutoDownloadFanartSeriesName.Name = "checkboxAutoDownloadFanartSeriesName";
            this.checkboxAutoDownloadFanartSeriesName.Size = new System.Drawing.Size(227, 17);
            this.checkboxAutoDownloadFanartSeriesName.TabIndex = 26;
            this.checkboxAutoDownloadFanartSeriesName.Text = "Download Fanart containing Series Names";
            this.toolTip_Help.SetToolTip(this.checkboxAutoDownloadFanartSeriesName, "Enable to automatically downlod Fanart that contain series names in artwork, user" +
                    "s can\r\nstill choose Fanart that contain series names from the Fanart Chooser win" +
                    "dow.");
            this.checkboxAutoDownloadFanartSeriesName.UseVisualStyleBackColor = true;
            this.checkboxAutoDownloadFanartSeriesName.CheckedChanged += new System.EventHandler(this.checkboxAutoDownloadFanartSeriesName_CheckedChanged);
            // 
            // buttonViewTemplates
            // 
            this.buttonViewTemplates.Location = new System.Drawing.Point(26, 292);
            this.buttonViewTemplates.Name = "buttonViewTemplates";
            this.buttonViewTemplates.Size = new System.Drawing.Size(75, 23);
            this.buttonViewTemplates.TabIndex = 9;
            this.buttonViewTemplates.Text = "&Templates...";
            this.toolTip_Help.SetToolTip(this.buttonViewTemplates, "Click to select a pre-defined View Filter from a list.");
            this.buttonViewTemplates.UseVisualStyleBackColor = true;
            this.buttonViewTemplates.Click += new System.EventHandler(this.buttonViewTemplates_Click);
            // 
            // checkBox_scanRemoteShares
            // 
            this.checkBox_scanRemoteShares.Location = new System.Drawing.Point(32, 307);
            this.checkBox_scanRemoteShares.Name = "checkBox_scanRemoteShares";
            this.checkBox_scanRemoteShares.Size = new System.Drawing.Size(171, 18);
            this.checkBox_scanRemoteShares.TabIndex = 32;
            this.checkBox_scanRemoteShares.Text = "Scan remote shares every";
            this.toolTip_Help.SetToolTip(this.checkBox_scanRemoteShares, "(Recommended) Instead of trying to setup a filesystem  watcher on a remote share " +
                    "(hazardous), scan periodically");
            this.checkBox_scanRemoteShares.UseVisualStyleBackColor = true;
            this.checkBox_scanRemoteShares.CheckedChanged += new System.EventHandler(this.checkBox_scanRemoteShares_CheckedChanged);
            // 
            // nudScanRemoteShareFrequency
            // 
            this.nudScanRemoteShareFrequency.Location = new System.Drawing.Point(185, 307);
            this.nudScanRemoteShareFrequency.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.nudScanRemoteShareFrequency.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudScanRemoteShareFrequency.Name = "nudScanRemoteShareFrequency";
            this.nudScanRemoteShareFrequency.Size = new System.Drawing.Size(44, 20);
            this.nudScanRemoteShareFrequency.TabIndex = 33;
            this.toolTip_Help.SetToolTip(this.nudScanRemoteShareFrequency, "Enter the percentage of the episode that has been viewed to consider it as watche" +
                    "d.\r\nEpisode foreground color will change to indicated that it is watched");
            this.nudScanRemoteShareFrequency.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudScanRemoteShareFrequency.ValueChanged += new System.EventHandler(this.nudScanRemoteShareFrequency_ValueChanged);
            // 
            // checkBox_SubDownloadOnPlay
            // 
            this.checkBox_SubDownloadOnPlay.Location = new System.Drawing.Point(404, 284);
            this.checkBox_SubDownloadOnPlay.Name = "checkBox_SubDownloadOnPlay";
            this.checkBox_SubDownloadOnPlay.Size = new System.Drawing.Size(417, 17);
            this.checkBox_SubDownloadOnPlay.TabIndex = 34;
            this.checkBox_SubDownloadOnPlay.Text = "Suggest subtitles download on play action if not present";
            this.toolTip_Help.SetToolTip(this.checkBox_SubDownloadOnPlay, "Enable this option to allow users to delete items using the context menu from wit" +
                    "h-in Media Portal");
            this.checkBox_SubDownloadOnPlay.UseVisualStyleBackColor = true;
            this.checkBox_SubDownloadOnPlay.CheckedChanged += new System.EventHandler(this.checkBox_SubDownloadOnPlay_CheckedChanged);
            // 
            // chkUseRegionalDateFormatString
            // 
            this.chkUseRegionalDateFormatString.AutoSize = true;
            this.chkUseRegionalDateFormatString.Location = new System.Drawing.Point(11, 215);
            this.chkUseRegionalDateFormatString.Name = "chkUseRegionalDateFormatString";
            this.chkUseRegionalDateFormatString.Size = new System.Drawing.Size(211, 17);
            this.chkUseRegionalDateFormatString.TabIndex = 15;
            this.chkUseRegionalDateFormatString.Text = "&Use Regional Settings to Display Dates";
            this.toolTip_InfoHelp.SetToolTip(this.chkUseRegionalDateFormatString, "Enable this option to convert all date formats yyyy-MM-dd to format defined in th" +
                    "e Windows Control Panel\r\nunder Regional and Language options.");
            this.chkUseRegionalDateFormatString.UseVisualStyleBackColor = true;
            this.chkUseRegionalDateFormatString.CheckedChanged += new System.EventHandler(this.chkUseRegionalDateFormatString_CheckedChanged);
            // 
            // toolTip_InfoHelp
            // 
            this.toolTip_InfoHelp.AutoPopDelay = 10000;
            this.toolTip_InfoHelp.InitialDelay = 500;
            this.toolTip_InfoHelp.IsBalloon = true;
            this.toolTip_InfoHelp.ReshowDelay = 100;
            this.toolTip_InfoHelp.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip_InfoHelp.ToolTipTitle = "View Format Help";
            // 
            // checkBox_Episode_HideUnwatchedSummary
            // 
            this.checkBox_Episode_HideUnwatchedSummary.AutoSize = true;
            this.checkBox_Episode_HideUnwatchedSummary.Location = new System.Drawing.Point(11, 169);
            this.checkBox_Episode_HideUnwatchedSummary.Name = "checkBox_Episode_HideUnwatchedSummary";
            this.checkBox_Episode_HideUnwatchedSummary.Size = new System.Drawing.Size(335, 17);
            this.checkBox_Episode_HideUnwatchedSummary.TabIndex = 13;
            this.checkBox_Episode_HideUnwatchedSummary.Text = "&Hide episode overview/summary on episodes that are unwatched";
            this.toolTip_InfoHelp.SetToolTip(this.checkBox_Episode_HideUnwatchedSummary, resources.GetString("checkBox_Episode_HideUnwatchedSummary.ToolTip"));
            this.checkBox_Episode_HideUnwatchedSummary.UseVisualStyleBackColor = true;
            this.checkBox_Episode_HideUnwatchedSummary.CheckedChanged += new System.EventHandler(this.checkBox_Episode_HideUnwatchedSummary_CheckedChanged);
            // 
            // checkBox_Episode_OnlyShowLocalFiles
            // 
            this.checkBox_Episode_OnlyShowLocalFiles.AutoSize = true;
            this.checkBox_Episode_OnlyShowLocalFiles.Location = new System.Drawing.Point(404, 169);
            this.checkBox_Episode_OnlyShowLocalFiles.Name = "checkBox_Episode_OnlyShowLocalFiles";
            this.checkBox_Episode_OnlyShowLocalFiles.Size = new System.Drawing.Size(266, 17);
            this.checkBox_Episode_OnlyShowLocalFiles.TabIndex = 28;
            this.checkBox_Episode_OnlyShowLocalFiles.Text = "&Only show episodes that are available on computer";
            this.toolTip_InfoHelp.SetToolTip(this.checkBox_Episode_OnlyShowLocalFiles, resources.GetString("checkBox_Episode_OnlyShowLocalFiles.ToolTip"));
            this.checkBox_Episode_OnlyShowLocalFiles.UseVisualStyleBackColor = true;
            this.checkBox_Episode_OnlyShowLocalFiles.CheckedChanged += new System.EventHandler(this.checkBox_Episode_OnlyShowLocalFiles_CheckedChanged);
            // 
            // richTextBox_seasonFormat_Col3
            // 
            this.richTextBox_seasonFormat_Col3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_seasonFormat_Col3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seasonFormat_Col3.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seasonFormat_Col3.Location = new System.Drawing.Point(509, 23);
            this.richTextBox_seasonFormat_Col3.Multiline = false;
            this.richTextBox_seasonFormat_Col3.Name = "richTextBox_seasonFormat_Col3";
            this.richTextBox_seasonFormat_Col3.Size = new System.Drawing.Size(229, 20);
            this.richTextBox_seasonFormat_Col3.TabIndex = 4;
            this.richTextBox_seasonFormat_Col3.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_seasonFormat_Col3, "Enter in the field(s) to be displayed in column three of the season view.\r\nYou ca" +
                    "n right click on this textbox to bring up a menu of available fields from the da" +
                    "tabase.\r\n\r\nDefault: Empty");
            // 
            // richTextBox_seasonFormat_Main
            // 
            this.richTextBox_seasonFormat_Main.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_seasonFormat_Main.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seasonFormat_Main.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seasonFormat_Main.Location = new System.Drawing.Point(97, 87);
            this.richTextBox_seasonFormat_Main.Multiline = false;
            this.richTextBox_seasonFormat_Main.Name = "richTextBox_seasonFormat_Main";
            this.richTextBox_seasonFormat_Main.Size = new System.Drawing.Size(641, 20);
            this.richTextBox_seasonFormat_Main.TabIndex = 10;
            this.richTextBox_seasonFormat_Main.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_seasonFormat_Main, resources.GetString("richTextBox_seasonFormat_Main.ToolTip"));
            // 
            // richTextBox_seasonFormat_Col1
            // 
            this.richTextBox_seasonFormat_Col1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seasonFormat_Col1.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seasonFormat_Col1.Location = new System.Drawing.Point(97, 23);
            this.richTextBox_seasonFormat_Col1.Multiline = false;
            this.richTextBox_seasonFormat_Col1.Name = "richTextBox_seasonFormat_Col1";
            this.richTextBox_seasonFormat_Col1.Size = new System.Drawing.Size(180, 20);
            this.richTextBox_seasonFormat_Col1.TabIndex = 2;
            this.richTextBox_seasonFormat_Col1.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_seasonFormat_Col1, "Enter in the field(s) to be displayed in column one of the season view.\r\nYou can " +
                    "right click on this textbox to bring up a menu of available fields from the data" +
                    "base.\r\n\r\nDefault: Empty");
            // 
            // richTextBox_seasonFormat_Subtitle
            // 
            this.richTextBox_seasonFormat_Subtitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_seasonFormat_Subtitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seasonFormat_Subtitle.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seasonFormat_Subtitle.Location = new System.Drawing.Point(97, 66);
            this.richTextBox_seasonFormat_Subtitle.Multiline = false;
            this.richTextBox_seasonFormat_Subtitle.Name = "richTextBox_seasonFormat_Subtitle";
            this.richTextBox_seasonFormat_Subtitle.Size = new System.Drawing.Size(641, 20);
            this.richTextBox_seasonFormat_Subtitle.TabIndex = 8;
            this.richTextBox_seasonFormat_Subtitle.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_seasonFormat_Subtitle, resources.GetString("richTextBox_seasonFormat_Subtitle.ToolTip"));
            // 
            // richTextBox_seasonFormat_Col2
            // 
            this.richTextBox_seasonFormat_Col2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_seasonFormat_Col2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seasonFormat_Col2.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seasonFormat_Col2.Location = new System.Drawing.Point(276, 23);
            this.richTextBox_seasonFormat_Col2.Multiline = false;
            this.richTextBox_seasonFormat_Col2.Name = "richTextBox_seasonFormat_Col2";
            this.richTextBox_seasonFormat_Col2.Size = new System.Drawing.Size(289, 20);
            this.richTextBox_seasonFormat_Col2.TabIndex = 3;
            this.richTextBox_seasonFormat_Col2.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_seasonFormat_Col2, resources.GetString("richTextBox_seasonFormat_Col2.ToolTip"));
            // 
            // richTextBox_seasonFormat_Title
            // 
            this.richTextBox_seasonFormat_Title.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_seasonFormat_Title.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seasonFormat_Title.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seasonFormat_Title.Location = new System.Drawing.Point(97, 45);
            this.richTextBox_seasonFormat_Title.Multiline = false;
            this.richTextBox_seasonFormat_Title.Name = "richTextBox_seasonFormat_Title";
            this.richTextBox_seasonFormat_Title.Size = new System.Drawing.Size(641, 20);
            this.richTextBox_seasonFormat_Title.TabIndex = 6;
            this.richTextBox_seasonFormat_Title.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_seasonFormat_Title, resources.GetString("richTextBox_seasonFormat_Title.ToolTip"));
            // 
            // richTextBox_episodeFormat_Col3
            // 
            this.richTextBox_episodeFormat_Col3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_episodeFormat_Col3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_episodeFormat_Col3.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_episodeFormat_Col3.Location = new System.Drawing.Point(509, 22);
            this.richTextBox_episodeFormat_Col3.Multiline = false;
            this.richTextBox_episodeFormat_Col3.Name = "richTextBox_episodeFormat_Col3";
            this.richTextBox_episodeFormat_Col3.Size = new System.Drawing.Size(229, 20);
            this.richTextBox_episodeFormat_Col3.TabIndex = 3;
            this.richTextBox_episodeFormat_Col3.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_episodeFormat_Col3, resources.GetString("richTextBox_episodeFormat_Col3.ToolTip"));
            // 
            // richTextBox_episodeFormat_Col2
            // 
            this.richTextBox_episodeFormat_Col2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_episodeFormat_Col2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_episodeFormat_Col2.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_episodeFormat_Col2.Location = new System.Drawing.Point(275, 22);
            this.richTextBox_episodeFormat_Col2.Multiline = false;
            this.richTextBox_episodeFormat_Col2.Name = "richTextBox_episodeFormat_Col2";
            this.richTextBox_episodeFormat_Col2.Size = new System.Drawing.Size(289, 20);
            this.richTextBox_episodeFormat_Col2.TabIndex = 2;
            this.richTextBox_episodeFormat_Col2.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_episodeFormat_Col2, resources.GetString("richTextBox_episodeFormat_Col2.ToolTip"));
            // 
            // richTextBox_episodeFormat_Main
            // 
            this.richTextBox_episodeFormat_Main.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_episodeFormat_Main.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_episodeFormat_Main.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_episodeFormat_Main.Location = new System.Drawing.Point(97, 88);
            this.richTextBox_episodeFormat_Main.Multiline = false;
            this.richTextBox_episodeFormat_Main.Name = "richTextBox_episodeFormat_Main";
            this.richTextBox_episodeFormat_Main.Size = new System.Drawing.Size(641, 20);
            this.richTextBox_episodeFormat_Main.TabIndex = 9;
            this.richTextBox_episodeFormat_Main.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_episodeFormat_Main, resources.GetString("richTextBox_episodeFormat_Main.ToolTip"));
            // 
            // richTextBox_episodeFormat_Subtitle
            // 
            this.richTextBox_episodeFormat_Subtitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_episodeFormat_Subtitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_episodeFormat_Subtitle.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_episodeFormat_Subtitle.Location = new System.Drawing.Point(97, 66);
            this.richTextBox_episodeFormat_Subtitle.Multiline = false;
            this.richTextBox_episodeFormat_Subtitle.Name = "richTextBox_episodeFormat_Subtitle";
            this.richTextBox_episodeFormat_Subtitle.Size = new System.Drawing.Size(641, 20);
            this.richTextBox_episodeFormat_Subtitle.TabIndex = 7;
            this.richTextBox_episodeFormat_Subtitle.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_episodeFormat_Subtitle, resources.GetString("richTextBox_episodeFormat_Subtitle.ToolTip"));
            // 
            // richTextBox_episodeFormat_Title
            // 
            this.richTextBox_episodeFormat_Title.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_episodeFormat_Title.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_episodeFormat_Title.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_episodeFormat_Title.Location = new System.Drawing.Point(97, 44);
            this.richTextBox_episodeFormat_Title.Multiline = false;
            this.richTextBox_episodeFormat_Title.Name = "richTextBox_episodeFormat_Title";
            this.richTextBox_episodeFormat_Title.Size = new System.Drawing.Size(641, 20);
            this.richTextBox_episodeFormat_Title.TabIndex = 5;
            this.richTextBox_episodeFormat_Title.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_episodeFormat_Title, resources.GetString("richTextBox_episodeFormat_Title.ToolTip"));
            // 
            // richTextBox_episodeFormat_Col1
            // 
            this.richTextBox_episodeFormat_Col1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_episodeFormat_Col1.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_episodeFormat_Col1.Location = new System.Drawing.Point(97, 22);
            this.richTextBox_episodeFormat_Col1.Multiline = false;
            this.richTextBox_episodeFormat_Col1.Name = "richTextBox_episodeFormat_Col1";
            this.richTextBox_episodeFormat_Col1.Size = new System.Drawing.Size(180, 20);
            this.richTextBox_episodeFormat_Col1.TabIndex = 1;
            this.richTextBox_episodeFormat_Col1.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_episodeFormat_Col1, "Enter in the field(s) to be displayed in column one of the episode view.\r\nYou can" +
                    " right click on this textbox to bring up a menu of available fields from the dat" +
                    "abase.\r\n\r\nDefault: Empty");
            // 
            // richTextBox_seriesFormat_Col3
            // 
            this.richTextBox_seriesFormat_Col3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_seriesFormat_Col3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seriesFormat_Col3.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seriesFormat_Col3.Location = new System.Drawing.Point(509, 25);
            this.richTextBox_seriesFormat_Col3.Multiline = false;
            this.richTextBox_seriesFormat_Col3.Name = "richTextBox_seriesFormat_Col3";
            this.richTextBox_seriesFormat_Col3.Size = new System.Drawing.Size(229, 20);
            this.richTextBox_seriesFormat_Col3.TabIndex = 4;
            this.richTextBox_seriesFormat_Col3.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_seriesFormat_Col3, resources.GetString("richTextBox_seriesFormat_Col3.ToolTip"));
            // 
            // richTextBox_seriesFormat_Main
            // 
            this.richTextBox_seriesFormat_Main.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_seriesFormat_Main.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seriesFormat_Main.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seriesFormat_Main.Location = new System.Drawing.Point(97, 89);
            this.richTextBox_seriesFormat_Main.Multiline = false;
            this.richTextBox_seriesFormat_Main.Name = "richTextBox_seriesFormat_Main";
            this.richTextBox_seriesFormat_Main.Size = new System.Drawing.Size(641, 20);
            this.richTextBox_seriesFormat_Main.TabIndex = 10;
            this.richTextBox_seriesFormat_Main.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_seriesFormat_Main, resources.GetString("richTextBox_seriesFormat_Main.ToolTip"));
            // 
            // richTextBox_seriesFormat_Subtitle
            // 
            this.richTextBox_seriesFormat_Subtitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_seriesFormat_Subtitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seriesFormat_Subtitle.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seriesFormat_Subtitle.Location = new System.Drawing.Point(97, 68);
            this.richTextBox_seriesFormat_Subtitle.Multiline = false;
            this.richTextBox_seriesFormat_Subtitle.Name = "richTextBox_seriesFormat_Subtitle";
            this.richTextBox_seriesFormat_Subtitle.Size = new System.Drawing.Size(641, 20);
            this.richTextBox_seriesFormat_Subtitle.TabIndex = 8;
            this.richTextBox_seriesFormat_Subtitle.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_seriesFormat_Subtitle, resources.GetString("richTextBox_seriesFormat_Subtitle.ToolTip"));
            // 
            // richTextBox_seriesFormat_Col2
            // 
            this.richTextBox_seriesFormat_Col2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_seriesFormat_Col2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seriesFormat_Col2.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seriesFormat_Col2.Location = new System.Drawing.Point(276, 25);
            this.richTextBox_seriesFormat_Col2.Multiline = false;
            this.richTextBox_seriesFormat_Col2.Name = "richTextBox_seriesFormat_Col2";
            this.richTextBox_seriesFormat_Col2.Size = new System.Drawing.Size(289, 20);
            this.richTextBox_seriesFormat_Col2.TabIndex = 3;
            this.richTextBox_seriesFormat_Col2.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_seriesFormat_Col2, resources.GetString("richTextBox_seriesFormat_Col2.ToolTip"));
            // 
            // richTextBox_seriesFormat_Title
            // 
            this.richTextBox_seriesFormat_Title.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_seriesFormat_Title.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seriesFormat_Title.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seriesFormat_Title.Location = new System.Drawing.Point(97, 47);
            this.richTextBox_seriesFormat_Title.Multiline = false;
            this.richTextBox_seriesFormat_Title.Name = "richTextBox_seriesFormat_Title";
            this.richTextBox_seriesFormat_Title.Size = new System.Drawing.Size(641, 20);
            this.richTextBox_seriesFormat_Title.TabIndex = 6;
            this.richTextBox_seriesFormat_Title.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_seriesFormat_Title, resources.GetString("richTextBox_seriesFormat_Title.ToolTip"));
            // 
            // richTextBox_seriesFormat_Col1
            // 
            this.richTextBox_seriesFormat_Col1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seriesFormat_Col1.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seriesFormat_Col1.Location = new System.Drawing.Point(97, 25);
            this.richTextBox_seriesFormat_Col1.Multiline = false;
            this.richTextBox_seriesFormat_Col1.Name = "richTextBox_seriesFormat_Col1";
            this.richTextBox_seriesFormat_Col1.Size = new System.Drawing.Size(180, 20);
            this.richTextBox_seriesFormat_Col1.TabIndex = 2;
            this.richTextBox_seriesFormat_Col1.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_seriesFormat_Col1, "Enter in the field(s) to be displayed in column one of the series view.\r\nYou can " +
                    "right click on this textbox to bring up a menu of available fields from the data" +
                    "base.\r\n\r\nDefault: Empty");
            // 
            // checkBox_Episode_HideUnwatchedThumbnail
            // 
            this.checkBox_Episode_HideUnwatchedThumbnail.AutoSize = true;
            this.checkBox_Episode_HideUnwatchedThumbnail.Location = new System.Drawing.Point(11, 192);
            this.checkBox_Episode_HideUnwatchedThumbnail.Name = "checkBox_Episode_HideUnwatchedThumbnail";
            this.checkBox_Episode_HideUnwatchedThumbnail.Size = new System.Drawing.Size(291, 17);
            this.checkBox_Episode_HideUnwatchedThumbnail.TabIndex = 14;
            this.checkBox_Episode_HideUnwatchedThumbnail.Text = "Hide episode &thumbnail on episodes that are unwatched";
            this.toolTip_InfoHelp.SetToolTip(this.checkBox_Episode_HideUnwatchedThumbnail, resources.GetString("checkBox_Episode_HideUnwatchedThumbnail.ToolTip"));
            this.checkBox_Episode_HideUnwatchedThumbnail.UseVisualStyleBackColor = true;
            this.checkBox_Episode_HideUnwatchedThumbnail.CheckedChanged += new System.EventHandler(this.checkBox_Episode_HideUnwatchedThumbnail_CheckedChanged);
            // 
            // optionAsk2Rate
            // 
            this.optionAsk2Rate.AutoSize = true;
            this.optionAsk2Rate.Checked = true;
            this.optionAsk2Rate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.optionAsk2Rate.Location = new System.Drawing.Point(11, 123);
            this.optionAsk2Rate.Name = "optionAsk2Rate";
            this.optionAsk2Rate.Option = "askToRate";
            this.optionAsk2Rate.Size = new System.Drawing.Size(234, 17);
            this.optionAsk2Rate.TabIndex = 11;
            this.optionAsk2Rate.Text = "&Popup Rate Dialog after episode is watched";
            this.optionAsk2Rate.ToolTip = "";
            this.toolTip_InfoHelp.SetToolTip(this.optionAsk2Rate, "Tick this to have the plugin automatically pop-up a rating\'s windows after you ha" +
                    "ve watched an episodes which hasn\'t been rated yet.");
            this.optionAsk2Rate.UseVisualStyleBackColor = true;
            this.optionAsk2Rate.CheckedChanged += new System.EventHandler(this.optionAsk2Rate_CheckedChanged);
            // 
            // dbOptionCheckBoxMarkRatedEpsAsWatched
            // 
            this.dbOptionCheckBoxMarkRatedEpsAsWatched.AutoSize = true;
            this.dbOptionCheckBoxMarkRatedEpsAsWatched.Location = new System.Drawing.Point(404, 215);
            this.dbOptionCheckBoxMarkRatedEpsAsWatched.Name = "dbOptionCheckBoxMarkRatedEpsAsWatched";
            this.dbOptionCheckBoxMarkRatedEpsAsWatched.Option = "MarkRatedEpisodeAsWatched";
            this.dbOptionCheckBoxMarkRatedEpsAsWatched.Size = new System.Drawing.Size(223, 17);
            this.dbOptionCheckBoxMarkRatedEpsAsWatched.TabIndex = 31;
            this.dbOptionCheckBoxMarkRatedEpsAsWatched.Text = "Mark Episodes as Watched if rated online";
            this.dbOptionCheckBoxMarkRatedEpsAsWatched.ToolTip = "";
            this.toolTip_InfoHelp.SetToolTip(this.dbOptionCheckBoxMarkRatedEpsAsWatched, "If enabled and episode has been rated, mark the episode as watched.");
            this.dbOptionCheckBoxMarkRatedEpsAsWatched.UseVisualStyleBackColor = true;
            // 
            // splitMain_Log
            // 
            this.splitMain_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain_Log.IsSplitterFixed = true;
            this.splitMain_Log.Location = new System.Drawing.Point(0, 0);
            this.splitMain_Log.Name = "splitMain_Log";
            this.splitMain_Log.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitMain_Log.Panel1
            // 
            this.splitMain_Log.Panel1.BackColor = System.Drawing.Color.White;
            this.splitMain_Log.Panel1.Controls.Add(this.tabControl_Details);
            this.splitMain_Log.Panel1.Controls.Add(this.button1);
            // 
            // splitMain_Log.Panel2
            // 
            this.splitMain_Log.Panel2.Controls.Add(this.listBox_Log);
            this.splitMain_Log.Panel2.Padding = new System.Windows.Forms.Padding(10);
            this.splitMain_Log.Panel2Collapsed = true;
            this.splitMain_Log.Size = new System.Drawing.Size(774, 562);
            this.splitMain_Log.SplitterDistance = 382;
            this.splitMain_Log.SplitterWidth = 1;
            this.splitMain_Log.TabIndex = 65;
            // 
            // tabControl_Details
            // 
            this.tabControl_Details.Controls.Add(this.tabPage_Details);
            this.tabControl_Details.Controls.Add(this.tabPage_Import);
            this.tabControl_Details.Controls.Add(this.tabOnlineData);
            this.tabControl_Details.Controls.Add(this.tabPage_MP_DisplayControl);
            this.tabControl_Details.Controls.Add(this.tab_view);
            this.tabControl_Details.Controls.Add(this.tabLogoRules);
            this.tabControl_Details.Controls.Add(this.tabFormattingRules);
            this.tabControl_Details.Controls.Add(this.tabLayoutSettings);
            this.tabControl_Details.Controls.Add(this.tabpage_Extra);
            this.tabControl_Details.Controls.Add(this.tabAbout);
            this.tabControl_Details.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_Details.ImageList = this.imageList1;
            this.tabControl_Details.Location = new System.Drawing.Point(0, 0);
            this.tabControl_Details.Name = "tabControl_Details";
            this.tabControl_Details.SelectedIndex = 0;
            this.tabControl_Details.Size = new System.Drawing.Size(774, 548);
            this.tabControl_Details.TabIndex = 0;
            // 
            // tabPage_Details
            // 
            this.tabPage_Details.Controls.Add(this.splitContainer2);
            this.tabPage_Details.ImageIndex = 1;
            this.tabPage_Details.Location = new System.Drawing.Point(4, 31);
            this.tabPage_Details.Name = "tabPage_Details";
            this.tabPage_Details.Size = new System.Drawing.Size(766, 513);
            this.tabPage_Details.TabIndex = 2;
            this.tabPage_Details.Text = "Details";
            this.tabPage_Details.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.treeView_Library);
            this.splitContainer2.Panel1.Controls.Add(this.checkBox_ShowHidden);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pictureBox_SeriesPoster);
            this.splitContainer2.Panel2.Controls.Add(this.comboBox_PosterSelection);
            this.splitContainer2.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer2.Panel2.Controls.Add(this.pictureBox_Series);
            this.splitContainer2.Panel2.Controls.Add(this.comboBox_BannerSelection);
            this.splitContainer2.Panel2.Controls.Add(this.panDBLocation);
            this.splitContainer2.Size = new System.Drawing.Size(766, 513);
            this.splitContainer2.SplitterDistance = 214;
            this.splitContainer2.TabIndex = 0;
            // 
            // treeView_Library
            // 
            this.treeView_Library.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView_Library.ContextMenuStrip = this.contextMenuStrip_DetailsTree;
            this.treeView_Library.ForeColor = System.Drawing.SystemColors.WindowText;
            this.treeView_Library.FullRowSelect = true;
            this.treeView_Library.HideSelection = false;
            this.treeView_Library.Location = new System.Drawing.Point(0, 0);
            this.treeView_Library.MinimumSize = new System.Drawing.Size(100, 100);
            this.treeView_Library.Name = "treeView_Library";
            this.treeView_Library.Size = new System.Drawing.Size(214, 487);
            this.treeView_Library.TabIndex = 0;
            this.treeView_Library.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_Library_AfterSelect);
            this.treeView_Library.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView_Library_MouseDown);
            this.treeView_Library.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_Library_NodeMouseClick);
            this.treeView_Library.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView_Library_KeyDown);
            this.treeView_Library.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeView_Library_AfterExpand);
            // 
            // pictureBox_SeriesPoster
            // 
            this.pictureBox_SeriesPoster.BackColor = System.Drawing.Color.White;
            this.pictureBox_SeriesPoster.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox_SeriesPoster.ErrorImage = null;
            this.pictureBox_SeriesPoster.InitialImage = null;
            this.pictureBox_SeriesPoster.Location = new System.Drawing.Point(0, 47);
            this.pictureBox_SeriesPoster.Name = "pictureBox_SeriesPoster";
            this.pictureBox_SeriesPoster.Size = new System.Drawing.Size(87, 129);
            this.pictureBox_SeriesPoster.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_SeriesPoster.TabIndex = 149;
            this.pictureBox_SeriesPoster.TabStop = false;
            // 
            // comboBox_PosterSelection
            // 
            this.comboBox_PosterSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_PosterSelection.FormattingEnabled = true;
            this.comboBox_PosterSelection.Location = new System.Drawing.Point(0, 25);
            this.comboBox_PosterSelection.Name = "comboBox_PosterSelection";
            this.comboBox_PosterSelection.Size = new System.Drawing.Size(87, 21);
            this.comboBox_PosterSelection.TabIndex = 148;
            this.comboBox_PosterSelection.SelectedIndexChanged += new System.EventHandler(this.comboBox_PosterSelection_SelectedIndexChanged);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle24.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle24.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle24.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle24.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle24.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle24.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle24;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.dataGridView1.DataSource = this.detailsPropertyBindingSource;
            dataGridViewCellStyle25.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle25.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle25.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle25.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle25.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle25.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle25;
            this.dataGridView1.Location = new System.Drawing.Point(0, 176);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle26.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle26.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle26.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle26.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle26.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle26.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle26.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle26;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.dataGridView1.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView1.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.Size = new System.Drawing.Size(548, 334);
            this.dataGridView1.StandardTab = true;
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView1_CellBeginEdit);
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Property";
            this.dataGridViewTextBoxColumn1.HeaderText = "Property";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 150;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 150;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Value";
            this.dataGridViewTextBoxColumn2.HeaderText = "Value";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // detailsPropertyBindingSource
            // 
            this.detailsPropertyBindingSource.DataSource = typeof(WindowPlugins.GUITVSeries.DetailsProperty);
            // 
            // pictureBox_Series
            // 
            this.pictureBox_Series.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox_Series.BackColor = System.Drawing.Color.White;
            this.pictureBox_Series.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox_Series.ErrorImage = null;
            this.pictureBox_Series.InitialImage = null;
            this.pictureBox_Series.Location = new System.Drawing.Point(88, 47);
            this.pictureBox_Series.Name = "pictureBox_Series";
            this.pictureBox_Series.Size = new System.Drawing.Size(459, 129);
            this.pictureBox_Series.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_Series.TabIndex = 147;
            this.pictureBox_Series.TabStop = false;
            // 
            // comboBox_BannerSelection
            // 
            this.comboBox_BannerSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_BannerSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_BannerSelection.FormattingEnabled = true;
            this.comboBox_BannerSelection.Location = new System.Drawing.Point(88, 25);
            this.comboBox_BannerSelection.Name = "comboBox_BannerSelection";
            this.comboBox_BannerSelection.Size = new System.Drawing.Size(460, 21);
            this.comboBox_BannerSelection.TabIndex = 1;
            this.comboBox_BannerSelection.SelectedIndexChanged += new System.EventHandler(this.comboBox_BannerSelection_SelectedIndexChanged);
            // 
            // panDBLocation
            // 
            this.panDBLocation.Controls.Add(this.textBox_dblocation);
            this.panDBLocation.Controls.Add(this.lblClearDB);
            this.panDBLocation.Controls.Add(this.button_dbbrowse);
            this.panDBLocation.Controls.Add(this.label28);
            this.panDBLocation.Dock = System.Windows.Forms.DockStyle.Top;
            this.panDBLocation.Location = new System.Drawing.Point(0, 0);
            this.panDBLocation.Name = "panDBLocation";
            this.panDBLocation.Size = new System.Drawing.Size(548, 25);
            this.panDBLocation.TabIndex = 0;
            // 
            // button_dbbrowse
            // 
            this.button_dbbrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_dbbrowse.Location = new System.Drawing.Point(472, 0);
            this.button_dbbrowse.Name = "button_dbbrowse";
            this.button_dbbrowse.Size = new System.Drawing.Size(26, 23);
            this.button_dbbrowse.TabIndex = 2;
            this.button_dbbrowse.Text = "...";
            this.button_dbbrowse.UseVisualStyleBackColor = true;
            this.button_dbbrowse.Click += new System.EventHandler(this.button_dbbrowse_Click);
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(16, 5);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(173, 13);
            this.label28.TabIndex = 0;
            this.label28.Text = "Database location (restart needed):";
            // 
            // tabPage_Import
            // 
            this.tabPage_Import.AutoScroll = true;
            this.tabPage_Import.BackColor = System.Drawing.Color.Transparent;
            this.tabPage_Import.Controls.Add(this.splitContainer_SettingsOutput);
            this.tabPage_Import.ImageIndex = 10;
            this.tabPage_Import.Location = new System.Drawing.Point(4, 31);
            this.tabPage_Import.Name = "tabPage_Import";
            this.tabPage_Import.Size = new System.Drawing.Size(766, 513);
            this.tabPage_Import.TabIndex = 4;
            this.tabPage_Import.Text = "Import";
            this.tabPage_Import.UseVisualStyleBackColor = true;
            // 
            // splitContainer_SettingsOutput
            // 
            this.splitContainer_SettingsOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_SettingsOutput.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_SettingsOutput.Name = "splitContainer_SettingsOutput";
            this.splitContainer_SettingsOutput.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_SettingsOutput.Panel1
            // 
            this.splitContainer_SettingsOutput.Panel1.Controls.Add(this.splitContainerImportSettings);
            // 
            // splitContainer_SettingsOutput.Panel2
            // 
            this.splitContainer_SettingsOutput.Panel2.Controls.Add(this.progressBar_Parsing);
            this.splitContainer_SettingsOutput.Panel2.Controls.Add(this.button_Start);
            this.splitContainer_SettingsOutput.Panel2MinSize = 30;
            this.splitContainer_SettingsOutput.Size = new System.Drawing.Size(766, 513);
            this.splitContainer_SettingsOutput.SplitterDistance = 457;
            this.splitContainer_SettingsOutput.TabIndex = 157;
            this.splitContainer_SettingsOutput.TabStop = false;
            // 
            // splitContainerImportSettings
            // 
            this.splitContainerImportSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerImportSettings.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerImportSettings.Location = new System.Drawing.Point(0, 0);
            this.splitContainerImportSettings.Name = "splitContainerImportSettings";
            // 
            // splitContainerImportSettings.Panel1
            // 
            this.splitContainerImportSettings.Panel1.Controls.Add(this.treeView_Settings);
            // 
            // splitContainerImportSettings.Panel2
            // 
            this.splitContainerImportSettings.Panel2.Controls.Add(this.panel_Expressions);
            this.splitContainerImportSettings.Panel2.Controls.Add(this.panel_StringReplacements);
            this.splitContainerImportSettings.Panel2.Controls.Add(this.panel_ImportPathes);
            this.splitContainerImportSettings.Panel2.Controls.Add(this.panel_manualEpisodeManagement);
            this.splitContainerImportSettings.Size = new System.Drawing.Size(766, 457);
            this.splitContainerImportSettings.SplitterDistance = 151;
            this.splitContainerImportSettings.TabIndex = 156;
            // 
            // treeView_Settings
            // 
            this.treeView_Settings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_Settings.Location = new System.Drawing.Point(0, 0);
            this.treeView_Settings.Name = "treeView_Settings";
            this.treeView_Settings.Size = new System.Drawing.Size(151, 457);
            this.treeView_Settings.TabIndex = 0;
            this.treeView_Settings.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_Settings_AfterSelect);
            // 
            // panel_Expressions
            // 
            this.panel_Expressions.Controls.Add(this.linkExParsingExpressions);
            this.panel_Expressions.Controls.Add(this.linkImpParsingExpressions);
            this.panel_Expressions.Controls.Add(this.linkExpressionHelp);
            this.panel_Expressions.Controls.Add(this.label70);
            this.panel_Expressions.Controls.Add(this.buildExpr);
            this.panel_Expressions.Controls.Add(this.resetExpr);
            this.panel_Expressions.Controls.Add(this.dataGridView_Expressions);
            this.panel_Expressions.Controls.Add(this.button_MoveExpUp);
            this.panel_Expressions.Controls.Add(this.button_MoveExpDown);
            this.panel_Expressions.Location = new System.Drawing.Point(6, 6);
            this.panel_Expressions.Name = "panel_Expressions";
            this.panel_Expressions.Size = new System.Drawing.Size(164, 241);
            this.panel_Expressions.TabIndex = 155;
            this.panel_Expressions.Tag = "Parsing Expressions";
            // 
            // linkExpressionHelp
            // 
            this.linkExpressionHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkExpressionHelp.AutoSize = true;
            this.linkExpressionHelp.Location = new System.Drawing.Point(126, 200);
            this.linkExpressionHelp.Name = "linkExpressionHelp";
            this.linkExpressionHelp.Size = new System.Drawing.Size(29, 13);
            this.linkExpressionHelp.TabIndex = 6;
            this.linkExpressionHelp.TabStop = true;
            this.linkExpressionHelp.Text = "Help";
            this.linkExpressionHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkExpressionHelp_LinkClicked);
            // 
            // label70
            // 
            this.label70.AutoSize = true;
            this.label70.Location = new System.Drawing.Point(5, 4);
            this.label70.Name = "label70";
            this.label70.Size = new System.Drawing.Size(513, 65);
            this.label70.TabIndex = 5;
            this.label70.Text = resources.GetString("label70.Text");
            // 
            // dataGridView_Expressions
            // 
            this.dataGridView_Expressions.AllowUserToResizeColumns = false;
            this.dataGridView_Expressions.AllowUserToResizeRows = false;
            dataGridViewCellStyle27.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_Expressions.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle27;
            this.dataGridView_Expressions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_Expressions.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView_Expressions.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView_Expressions.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle28.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle28.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle28.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle28.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle28.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle28.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle28.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Expressions.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle28;
            this.dataGridView_Expressions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle29.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle29.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle29.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle29.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle29.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle29.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle29.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Expressions.DefaultCellStyle = dataGridViewCellStyle29;
            this.dataGridView_Expressions.Location = new System.Drawing.Point(1, 72);
            this.dataGridView_Expressions.MultiSelect = false;
            this.dataGridView_Expressions.Name = "dataGridView_Expressions";
            this.dataGridView_Expressions.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle30.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle30.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle30.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle30.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle30.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle30.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle30.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Expressions.RowHeadersDefaultCellStyle = dataGridViewCellStyle30;
            this.dataGridView_Expressions.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridViewCellStyle31.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_Expressions.RowsDefaultCellStyle = dataGridViewCellStyle31;
            this.dataGridView_Expressions.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_Expressions.RowTemplate.Height = 18;
            this.dataGridView_Expressions.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Expressions.Size = new System.Drawing.Size(123, 169);
            this.dataGridView_Expressions.StandardTab = true;
            this.dataGridView_Expressions.TabIndex = 0;
            this.dataGridView_Expressions.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView_Expressions_UserDeletedRow);
            this.dataGridView_Expressions.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_Expressions_CellEndEdit);
            // 
            // button_MoveExpUp
            // 
            this.button_MoveExpUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_MoveExpUp.Image = ((System.Drawing.Image)(resources.GetObject("button_MoveExpUp.Image")));
            this.button_MoveExpUp.Location = new System.Drawing.Point(130, 72);
            this.button_MoveExpUp.Name = "button_MoveExpUp";
            this.button_MoveExpUp.Size = new System.Drawing.Size(28, 29);
            this.button_MoveExpUp.TabIndex = 1;
            this.button_MoveExpUp.UseVisualStyleBackColor = true;
            this.button_MoveExpUp.Click += new System.EventHandler(this.button_MoveExpUp_Click);
            // 
            // button_MoveExpDown
            // 
            this.button_MoveExpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_MoveExpDown.Image = ((System.Drawing.Image)(resources.GetObject("button_MoveExpDown.Image")));
            this.button_MoveExpDown.Location = new System.Drawing.Point(130, 107);
            this.button_MoveExpDown.Name = "button_MoveExpDown";
            this.button_MoveExpDown.Size = new System.Drawing.Size(28, 29);
            this.button_MoveExpDown.TabIndex = 2;
            this.button_MoveExpDown.UseVisualStyleBackColor = true;
            this.button_MoveExpDown.Click += new System.EventHandler(this.button_MoveExpDown_Click);
            // 
            // panel_StringReplacements
            // 
            this.panel_StringReplacements.Controls.Add(this.linkLabelResetStringReplacements);
            this.panel_StringReplacements.Controls.Add(this.linkLabelImportStringReplacements);
            this.panel_StringReplacements.Controls.Add(this.linkLabelExportStringReplacements);
            this.panel_StringReplacements.Controls.Add(this.label69);
            this.panel_StringReplacements.Controls.Add(this.dataGridView_Replace);
            this.panel_StringReplacements.Location = new System.Drawing.Point(6, 167);
            this.panel_StringReplacements.Name = "panel_StringReplacements";
            this.panel_StringReplacements.Size = new System.Drawing.Size(164, 152);
            this.panel_StringReplacements.TabIndex = 155;
            this.panel_StringReplacements.Tag = "String Replacements";
            // 
            // linkLabelResetStringReplacements
            // 
            this.linkLabelResetStringReplacements.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelResetStringReplacements.AutoSize = true;
            this.linkLabelResetStringReplacements.Location = new System.Drawing.Point(126, 128);
            this.linkLabelResetStringReplacements.Name = "linkLabelResetStringReplacements";
            this.linkLabelResetStringReplacements.Size = new System.Drawing.Size(35, 13);
            this.linkLabelResetStringReplacements.TabIndex = 4;
            this.linkLabelResetStringReplacements.TabStop = true;
            this.linkLabelResetStringReplacements.Text = "Reset";
            this.linkLabelResetStringReplacements.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelResetStringReplacements_LinkClicked);
            // 
            // linkLabelImportStringReplacements
            // 
            this.linkLabelImportStringReplacements.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelImportStringReplacements.AutoSize = true;
            this.linkLabelImportStringReplacements.Location = new System.Drawing.Point(126, 83);
            this.linkLabelImportStringReplacements.Name = "linkLabelImportStringReplacements";
            this.linkLabelImportStringReplacements.Size = new System.Drawing.Size(36, 13);
            this.linkLabelImportStringReplacements.TabIndex = 3;
            this.linkLabelImportStringReplacements.TabStop = true;
            this.linkLabelImportStringReplacements.Text = "Import";
            this.linkLabelImportStringReplacements.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelImportStringReplacements_LinkClicked);
            // 
            // linkLabelExportStringReplacements
            // 
            this.linkLabelExportStringReplacements.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelExportStringReplacements.AutoSize = true;
            this.linkLabelExportStringReplacements.Location = new System.Drawing.Point(126, 106);
            this.linkLabelExportStringReplacements.Name = "linkLabelExportStringReplacements";
            this.linkLabelExportStringReplacements.Size = new System.Drawing.Size(37, 13);
            this.linkLabelExportStringReplacements.TabIndex = 2;
            this.linkLabelExportStringReplacements.TabStop = true;
            this.linkLabelExportStringReplacements.Text = "Export";
            this.linkLabelExportStringReplacements.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelExportStringReplacements_LinkClicked);
            // 
            // label69
            // 
            this.label69.AutoSize = true;
            this.label69.Location = new System.Drawing.Point(4, 5);
            this.label69.Name = "label69";
            this.label69.Size = new System.Drawing.Size(545, 65);
            this.label69.TabIndex = 1;
            this.label69.Text = resources.GetString("label69.Text");
            // 
            // panel_ImportPathes
            // 
            this.panel_ImportPathes.Controls.Add(this.label68);
            this.panel_ImportPathes.Controls.Add(this.button_TestReparse);
            this.panel_ImportPathes.Controls.Add(this.dataGridView_ImportPathes);
            this.panel_ImportPathes.Controls.Add(this.listView_ParsingResults);
            this.panel_ImportPathes.Location = new System.Drawing.Point(175, 9);
            this.panel_ImportPathes.Name = "panel_ImportPathes";
            this.panel_ImportPathes.Size = new System.Drawing.Size(433, 376);
            this.panel_ImportPathes.TabIndex = 154;
            this.panel_ImportPathes.Tag = "Import Paths";
            // 
            // label68
            // 
            this.label68.AutoSize = true;
            this.label68.Location = new System.Drawing.Point(1, 117);
            this.label68.Name = "label68";
            this.label68.Size = new System.Drawing.Size(481, 39);
            this.label68.TabIndex = 151;
            this.label68.Text = resources.GetString("label68.Text");
            // 
            // dataGridView_ImportPathes
            // 
            this.dataGridView_ImportPathes.AllowUserToResizeColumns = false;
            this.dataGridView_ImportPathes.AllowUserToResizeRows = false;
            dataGridViewCellStyle32.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_ImportPathes.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle32;
            this.dataGridView_ImportPathes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_ImportPathes.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView_ImportPathes.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView_ImportPathes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle33.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle33.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle33.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle33.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle33.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle33.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle33.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_ImportPathes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle33;
            this.dataGridView_ImportPathes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle34.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle34.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle34.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle34.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle34.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle34.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle34.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_ImportPathes.DefaultCellStyle = dataGridViewCellStyle34;
            this.dataGridView_ImportPathes.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_ImportPathes.MultiSelect = false;
            this.dataGridView_ImportPathes.Name = "dataGridView_ImportPathes";
            this.dataGridView_ImportPathes.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle35.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle35.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle35.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle35.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle35.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle35.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle35.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_ImportPathes.RowHeadersDefaultCellStyle = dataGridViewCellStyle35;
            this.dataGridView_ImportPathes.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridViewCellStyle36.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_ImportPathes.RowsDefaultCellStyle = dataGridViewCellStyle36;
            this.dataGridView_ImportPathes.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_ImportPathes.RowTemplate.Height = 18;
            this.dataGridView_ImportPathes.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_ImportPathes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView_ImportPathes.Size = new System.Drawing.Size(394, 114);
            this.dataGridView_ImportPathes.StandardTab = true;
            this.dataGridView_ImportPathes.TabIndex = 150;
            this.dataGridView_ImportPathes.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_ImportPathes_CellValueChanged);
            this.dataGridView_ImportPathes.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView_ImportPathes_UserDeletedRow);
            this.dataGridView_ImportPathes.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataGridView_ImportPathes_RowsRemoved);
            this.dataGridView_ImportPathes.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_ImportPathes_CellContentClick);
            // 
            // listView_ParsingResults
            // 
            this.listView_ParsingResults.AllowColumnReorder = true;
            this.listView_ParsingResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView_ParsingResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.FileName,
            this.ParsedSeriesName,
            this.SeasonID,
            this.EpisodeID,
            this.EpisodeTitle});
            this.listView_ParsingResults.FullRowSelect = true;
            this.listView_ParsingResults.GridLines = true;
            this.listView_ParsingResults.HideSelection = false;
            this.listView_ParsingResults.Location = new System.Drawing.Point(0, 159);
            this.listView_ParsingResults.MultiSelect = false;
            this.listView_ParsingResults.Name = "listView_ParsingResults";
            this.listView_ParsingResults.Size = new System.Drawing.Size(394, 217);
            this.listView_ParsingResults.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView_ParsingResults.TabIndex = 0;
            this.listView_ParsingResults.UseCompatibleStateImageBehavior = false;
            this.listView_ParsingResults.View = System.Windows.Forms.View.Details;
            // 
            // FileName
            // 
            this.FileName.Text = "FileName";
            // 
            // ParsedSeriesName
            // 
            this.ParsedSeriesName.Text = "Parsed Series Name";
            this.ParsedSeriesName.Width = 113;
            // 
            // SeasonID
            // 
            this.SeasonID.Text = "Season ID";
            this.SeasonID.Width = 79;
            // 
            // EpisodeID
            // 
            this.EpisodeID.Text = "Episode ID";
            this.EpisodeID.Width = 74;
            // 
            // EpisodeTitle
            // 
            this.EpisodeTitle.Text = "Episode Title";
            this.EpisodeTitle.Width = 74;
            // 
            // panel_manualEpisodeManagement
            // 
            this.panel_manualEpisodeManagement.Location = new System.Drawing.Point(6, 311);
            this.panel_manualEpisodeManagement.Name = "panel_manualEpisodeManagement";
            this.panel_manualEpisodeManagement.Size = new System.Drawing.Size(164, 143);
            this.panel_manualEpisodeManagement.TabIndex = 157;
            this.panel_manualEpisodeManagement.Tag = "Manual Episode Importer";
            // 
            // progressBar_Parsing
            // 
            this.progressBar_Parsing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar_Parsing.Location = new System.Drawing.Point(153, 6);
            this.progressBar_Parsing.Name = "progressBar_Parsing";
            this.progressBar_Parsing.Size = new System.Drawing.Size(574, 20);
            this.progressBar_Parsing.TabIndex = 3;
            // 
            // tabOnlineData
            // 
            this.tabOnlineData.Controls.Add(this.groupBox12);
            this.tabOnlineData.ImageIndex = 8;
            this.tabOnlineData.Location = new System.Drawing.Point(4, 31);
            this.tabOnlineData.Name = "tabOnlineData";
            this.tabOnlineData.Padding = new System.Windows.Forms.Padding(3);
            this.tabOnlineData.Size = new System.Drawing.Size(766, 513);
            this.tabOnlineData.TabIndex = 12;
            this.tabOnlineData.Text = "Online Data";
            this.tabOnlineData.UseVisualStyleBackColor = true;
            // 
            // groupBox12
            // 
            this.groupBox12.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox12.Controls.Add(this.panel_OnlineData);
            this.groupBox12.Location = new System.Drawing.Point(3, 3);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(760, 507);
            this.groupBox12.TabIndex = 0;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Online Data Provider";
            // 
            // panel_OnlineData
            // 
            this.panel_OnlineData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_OnlineData.AutoScroll = true;
            this.panel_OnlineData.Controls.Add(this.checkboxAutoDownloadFanartSeriesName);
            this.panel_OnlineData.Controls.Add(this.checkBox_AutoDownloadMissingArtwork);
            this.panel_OnlineData.Controls.Add(this.checkBox_AutoUpdateAllFanart);
            this.panel_OnlineData.Controls.Add(this.checkBox_AutoUpdateEpisodeRatings);
            this.panel_OnlineData.Controls.Add(this.checkBox_ScanOnStartup);
            this.panel_OnlineData.Controls.Add(this.label73);
            this.panel_OnlineData.Controls.Add(this.spinMaxFanarts);
            this.panel_OnlineData.Controls.Add(this.label72);
            this.panel_OnlineData.Controls.Add(this.label71);
            this.panel_OnlineData.Controls.Add(this.cboFanartResolution);
            this.panel_OnlineData.Controls.Add(this.chkAutoDownloadFanart);
            this.panel_OnlineData.Controls.Add(this.linkAccountID);
            this.panel_OnlineData.Controls.Add(this.linkMediaInfoUpdate);
            this.panel_OnlineData.Controls.Add(this.label2);
            this.panel_OnlineData.Controls.Add(this.numericUpDown_AutoOnlineDataRefresh);
            this.panel_OnlineData.Controls.Add(this.checkBox_AutoOnlineDataRefresh);
            this.panel_OnlineData.Controls.Add(this.checkBox_AutoChooseOrder);
            this.panel_OnlineData.Controls.Add(this.txtUserID);
            this.panel_OnlineData.Controls.Add(this.label54);
            this.panel_OnlineData.Controls.Add(this.checkDownloadEpisodeSnapshots);
            this.panel_OnlineData.Controls.Add(this.chkBlankBanners);
            this.panel_OnlineData.Controls.Add(this.label29);
            this.panel_OnlineData.Controls.Add(this.txtMainMirror);
            this.panel_OnlineData.Controls.Add(this.linkDelUpdateTime);
            this.panel_OnlineData.Controls.Add(this.label26);
            this.panel_OnlineData.Controls.Add(this.comboOnlineLang);
            this.panel_OnlineData.Controls.Add(this.checkBox_OnlineSearch);
            this.panel_OnlineData.Controls.Add(this.checkBox_AutoChooseSeries);
            this.panel_OnlineData.Controls.Add(this.checkBox_FullSeriesRetrieval);
            this.panel_OnlineData.Location = new System.Drawing.Point(6, 19);
            this.panel_OnlineData.Name = "panel_OnlineData";
            this.panel_OnlineData.Size = new System.Drawing.Size(748, 482);
            this.panel_OnlineData.TabIndex = 0;
            this.panel_OnlineData.Tag = "Online Data";
            // 
            // checkBox_AutoUpdateAllFanart
            // 
            this.checkBox_AutoUpdateAllFanart.AutoSize = true;
            this.checkBox_AutoUpdateAllFanart.Location = new System.Drawing.Point(33, 401);
            this.checkBox_AutoUpdateAllFanart.Name = "checkBox_AutoUpdateAllFanart";
            this.checkBox_AutoUpdateAllFanart.Size = new System.Drawing.Size(460, 17);
            this.checkBox_AutoUpdateAllFanart.TabIndex = 25;
            this.checkBox_AutoUpdateAllFanart.Text = "Automatically update all Fanart existing and new.  Update only new Fanart when un" +
                "checked";
            this.checkBox_AutoUpdateAllFanart.UseVisualStyleBackColor = true;
            this.checkBox_AutoUpdateAllFanart.CheckedChanged += new System.EventHandler(this.checkBox_AutoUpdateAllFanart_CheckedChanged);
            // 
            // label73
            // 
            this.label73.Location = new System.Drawing.Point(164, 373);
            this.label73.Name = "label73";
            this.label73.Size = new System.Drawing.Size(66, 21);
            this.label73.TabIndex = 24;
            this.label73.Text = "fanarts";
            this.label73.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // spinMaxFanarts
            // 
            this.spinMaxFanarts.Location = new System.Drawing.Point(121, 375);
            this.spinMaxFanarts.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.spinMaxFanarts.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spinMaxFanarts.Name = "spinMaxFanarts";
            this.spinMaxFanarts.Size = new System.Drawing.Size(36, 20);
            this.spinMaxFanarts.TabIndex = 23;
            this.spinMaxFanarts.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.spinMaxFanarts.ValueChanged += new System.EventHandler(this.spinMaxFanarts_ValueChanged);
            // 
            // label72
            // 
            this.label72.Location = new System.Drawing.Point(49, 374);
            this.label72.Name = "label72";
            this.label72.Size = new System.Drawing.Size(66, 21);
            this.label72.TabIndex = 22;
            this.label72.Text = "Retrieve:";
            this.label72.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label71
            // 
            this.label71.Location = new System.Drawing.Point(49, 349);
            this.label71.Name = "label71";
            this.label71.Size = new System.Drawing.Size(66, 21);
            this.label71.TabIndex = 20;
            this.label71.Text = "Resolution:";
            this.label71.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboFanartResolution
            // 
            this.cboFanartResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFanartResolution.FormattingEnabled = true;
            this.cboFanartResolution.Items.AddRange(new object[] {
            "Both",
            "1280x720",
            "1920x1080"});
            this.cboFanartResolution.Location = new System.Drawing.Point(121, 349);
            this.cboFanartResolution.Name = "cboFanartResolution";
            this.cboFanartResolution.Size = new System.Drawing.Size(121, 21);
            this.cboFanartResolution.TabIndex = 21;
            this.cboFanartResolution.SelectedIndexChanged += new System.EventHandler(this.cboFanartResolution_SelectedIndexChanged);
            // 
            // chkAutoDownloadFanart
            // 
            this.chkAutoDownloadFanart.AutoSize = true;
            this.chkAutoDownloadFanart.Location = new System.Drawing.Point(8, 326);
            this.chkAutoDownloadFanart.Name = "chkAutoDownloadFanart";
            this.chkAutoDownloadFanart.Size = new System.Drawing.Size(202, 17);
            this.chkAutoDownloadFanart.TabIndex = 19;
            this.chkAutoDownloadFanart.Text = "Automatically download Series &Fanart";
            this.chkAutoDownloadFanart.UseVisualStyleBackColor = true;
            this.chkAutoDownloadFanart.CheckedChanged += new System.EventHandler(this.chkAutoDownloadFanart_CheckedChanged);
            // 
            // linkAccountID
            // 
            this.linkAccountID.AutoSize = true;
            this.linkAccountID.Location = new System.Drawing.Point(323, 58);
            this.linkAccountID.Name = "linkAccountID";
            this.linkAccountID.Size = new System.Drawing.Size(90, 13);
            this.linkAccountID.TabIndex = 5;
            this.linkAccountID.TabStop = true;
            this.linkAccountID.Text = "Account Identifier";
            this.linkAccountID.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkAccountID_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(382, 143);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "&hour(s)";
            // 
            // numericUpDown_AutoOnlineDataRefresh
            // 
            this.numericUpDown_AutoOnlineDataRefresh.Location = new System.Drawing.Point(340, 139);
            this.numericUpDown_AutoOnlineDataRefresh.Name = "numericUpDown_AutoOnlineDataRefresh";
            this.numericUpDown_AutoOnlineDataRefresh.Size = new System.Drawing.Size(36, 20);
            this.numericUpDown_AutoOnlineDataRefresh.TabIndex = 10;
            this.numericUpDown_AutoOnlineDataRefresh.ValueChanged += new System.EventHandler(this.numericUpDown_AutoOnlineDataRefresh_ValueChanged);
            // 
            // label54
            // 
            this.label54.Location = new System.Drawing.Point(39, 58);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(76, 13);
            this.label54.TabIndex = 3;
            this.label54.Text = "&Account ID:";
            this.label54.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label29
            // 
            this.label29.Location = new System.Drawing.Point(39, 31);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(76, 13);
            this.label29.TabIndex = 1;
            this.label29.Text = "&Main Mirror:";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label26
            // 
            this.label26.Location = new System.Drawing.Point(39, 85);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(76, 13);
            this.label26.TabIndex = 6;
            this.label26.Text = "&Language:";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabPage_MP_DisplayControl
            // 
            this.tabPage_MP_DisplayControl.AutoScroll = true;
            this.tabPage_MP_DisplayControl.AutoScrollMinSize = new System.Drawing.Size(0, 478);
            this.tabPage_MP_DisplayControl.Controls.Add(this.groupBox1);
            this.tabPage_MP_DisplayControl.ImageIndex = 9;
            this.tabPage_MP_DisplayControl.Location = new System.Drawing.Point(4, 31);
            this.tabPage_MP_DisplayControl.Name = "tabPage_MP_DisplayControl";
            this.tabPage_MP_DisplayControl.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_MP_DisplayControl.Size = new System.Drawing.Size(766, 513);
            this.tabPage_MP_DisplayControl.TabIndex = 5;
            this.tabPage_MP_DisplayControl.Text = "General";
            this.tabPage_MP_DisplayControl.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.dbOptChkBoxScanFullscreenVideo);
            this.groupBox1.Controls.Add(this.dbOptionCheckBox2);
            this.groupBox1.Controls.Add(this.dbOptionCheckBoxSubstituteMissingArtwork);
            this.groupBox1.Controls.Add(this.checkBox_SubDownloadOnPlay);
            this.groupBox1.Controls.Add(this.nudScanRemoteShareFrequency);
            this.groupBox1.Controls.Add(this.checkBox_scanRemoteShares);
            this.groupBox1.Controls.Add(this.dbOptionCheckBoxMarkRatedEpsAsWatched);
            this.groupBox1.Controls.Add(this.dbOptionCheckBoxSMSKeyboard);
            this.groupBox1.Controls.Add(this.checkboxRatingDisplayStars);
            this.groupBox1.Controls.Add(this.label77);
            this.groupBox1.Controls.Add(this.label76);
            this.groupBox1.Controls.Add(this.label75);
            this.groupBox1.Controls.Add(this.numericUpDownArtworkDelay);
            this.groupBox1.Controls.Add(this.label74);
            this.groupBox1.Controls.Add(this.label27);
            this.groupBox1.Controls.Add(this.numericUpDownBackdropDelay);
            this.groupBox1.Controls.Add(this.checkbox_SortSpecials);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.comboLogLevel);
            this.groupBox1.Controls.Add(this.linkExWatched);
            this.groupBox1.Controls.Add(this.linkImpWatched);
            this.groupBox1.Controls.Add(this.chkAllowDeletes);
            this.groupBox1.Controls.Add(this.checkBox_doFolderWatch);
            this.groupBox1.Controls.Add(this.checkBox_DontClearMissingLocalFiles);
            this.groupBox1.Controls.Add(this.checkBox_Episode_HideUnwatchedSummary);
            this.groupBox1.Controls.Add(this.checkBox_Episode_HideUnwatchedThumbnail);
            this.groupBox1.Controls.Add(this.checkBox_Episode_OnlyShowLocalFiles);
            this.groupBox1.Controls.Add(this.chkShowSeriesFanart);
            this.groupBox1.Controls.Add(this.checkBox_Series_UseSortName);
            this.groupBox1.Controls.Add(this.optionAsk2Rate);
            this.groupBox1.Controls.Add(this.chkUseRegionalDateFormatString);
            this.groupBox1.Controls.Add(this.label39);
            this.groupBox1.Controls.Add(this.label38);
            this.groupBox1.Controls.Add(this.nudWatchedAfter);
            this.groupBox1.Controls.Add(this.label32);
            this.groupBox1.Controls.Add(this.comboLanguage);
            this.groupBox1.Controls.Add(this.checkBox_RandBanner);
            this.groupBox1.Controls.Add(this.textBox_PluginHomeName);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(754, 504);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General Settings";
            // 
            // dbOptChkBoxScanFullscreenVideo
            // 
            this.dbOptChkBoxScanFullscreenVideo.AutoSize = true;
            this.dbOptChkBoxScanFullscreenVideo.Location = new System.Drawing.Point(32, 332);
            this.dbOptChkBoxScanFullscreenVideo.Name = "dbOptChkBoxScanFullscreenVideo";
            this.dbOptChkBoxScanFullscreenVideo.Option = "AutoScanLocalFilesFSV";
            this.dbOptChkBoxScanFullscreenVideo.Size = new System.Drawing.Size(266, 17);
            this.dbOptChkBoxScanFullscreenVideo.TabIndex = 38;
            this.dbOptChkBoxScanFullscreenVideo.Text = "Scan remote shares while fullscreen video is active";
            this.dbOptChkBoxScanFullscreenVideo.ToolTip = "";
            this.dbOptChkBoxScanFullscreenVideo.UseVisualStyleBackColor = true;
            // 
            // dbOptionCheckBox2
            // 
            this.dbOptionCheckBox2.AutoSize = true;
            this.dbOptionCheckBox2.Checked = true;
            this.dbOptionCheckBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dbOptionCheckBox2.Location = new System.Drawing.Point(404, 238);
            this.dbOptionCheckBox2.Name = "dbOptionCheckBox2";
            this.dbOptionCheckBox2.Option = "SkipSeasonViewOnSingleSeason";
            this.dbOptionCheckBox2.Size = new System.Drawing.Size(234, 17);
            this.dbOptionCheckBox2.TabIndex = 37;
            this.dbOptionCheckBox2.Text = "Skip season view if there is only one season";
            this.dbOptionCheckBox2.ToolTip = "";
            this.dbOptionCheckBox2.UseVisualStyleBackColor = true;
            // 
            // dbOptionCheckBoxSubstituteMissingArtwork
            // 
            this.dbOptionCheckBoxSubstituteMissingArtwork.AutoSize = true;
            this.dbOptionCheckBoxSubstituteMissingArtwork.Location = new System.Drawing.Point(404, 307);
            this.dbOptionCheckBoxSubstituteMissingArtwork.Name = "dbOptionCheckBoxSubstituteMissingArtwork";
            this.dbOptionCheckBoxSubstituteMissingArtwork.Option = "SubstituteMissingArtwork";
            this.dbOptionCheckBoxSubstituteMissingArtwork.Size = new System.Drawing.Size(280, 17);
            this.dbOptionCheckBoxSubstituteMissingArtwork.TabIndex = 35;
            this.dbOptionCheckBoxSubstituteMissingArtwork.Text = "Substitute Missing Season Posters with Series Posters";
            this.dbOptionCheckBoxSubstituteMissingArtwork.ToolTip = "";
            this.dbOptionCheckBoxSubstituteMissingArtwork.UseVisualStyleBackColor = true;
            // 
            // dbOptionCheckBoxSMSKeyboard
            // 
            this.dbOptionCheckBoxSMSKeyboard.AutoSize = true;
            this.dbOptionCheckBoxSMSKeyboard.Location = new System.Drawing.Point(404, 330);
            this.dbOptionCheckBoxSMSKeyboard.Name = "dbOptionCheckBoxSMSKeyboard";
            this.dbOptionCheckBoxSMSKeyboard.Option = "KeyboardStyle";
            this.dbOptionCheckBoxSMSKeyboard.Size = new System.Drawing.Size(305, 17);
            this.dbOptionCheckBoxSMSKeyboard.TabIndex = 30;
            this.dbOptionCheckBoxSMSKeyboard.Text = "Show SMS Style keyboard when requesting input from user";
            this.dbOptionCheckBoxSMSKeyboard.ToolTip = "";
            this.dbOptionCheckBoxSMSKeyboard.UseVisualStyleBackColor = true;
            // 
            // label77
            // 
            this.label77.AutoSize = true;
            this.label77.Location = new System.Drawing.Point(235, 309);
            this.label77.Name = "label77";
            this.label77.Size = new System.Drawing.Size(43, 13);
            this.label77.TabIndex = 10;
            this.label77.Text = "minutes";
            // 
            // label76
            // 
            this.label76.AutoSize = true;
            this.label76.Location = new System.Drawing.Point(209, 99);
            this.label76.Name = "label76";
            this.label76.Size = new System.Drawing.Size(20, 13);
            this.label76.TabIndex = 10;
            this.label76.Text = "ms";
            // 
            // label75
            // 
            this.label75.AutoSize = true;
            this.label75.Location = new System.Drawing.Point(209, 74);
            this.label75.Name = "label75";
            this.label75.Size = new System.Drawing.Size(20, 13);
            this.label75.TabIndex = 7;
            this.label75.Text = "ms";
            // 
            // numericUpDownArtworkDelay
            // 
            this.numericUpDownArtworkDelay.Location = new System.Drawing.Point(159, 96);
            this.numericUpDownArtworkDelay.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericUpDownArtworkDelay.Name = "numericUpDownArtworkDelay";
            this.numericUpDownArtworkDelay.Size = new System.Drawing.Size(44, 20);
            this.numericUpDownArtworkDelay.TabIndex = 9;
            this.numericUpDownArtworkDelay.Value = new decimal(new int[] {
            250,
            0,
            0,
            0});
            this.numericUpDownArtworkDelay.ValueChanged += new System.EventHandler(this.numericUpDownArtworkDelay_ValueChanged);
            // 
            // label74
            // 
            this.label74.AutoSize = true;
            this.label74.Location = new System.Drawing.Point(9, 99);
            this.label74.Name = "label74";
            this.label74.Size = new System.Drawing.Size(117, 13);
            this.label74.TabIndex = 8;
            this.label74.Text = "Artwork Loading Delay:";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(9, 74);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(127, 13);
            this.label27.TabIndex = 5;
            this.label27.Text = "Backdrop Loading Delay:";
            // 
            // numericUpDownBackdropDelay
            // 
            this.numericUpDownBackdropDelay.Location = new System.Drawing.Point(159, 71);
            this.numericUpDownBackdropDelay.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericUpDownBackdropDelay.Name = "numericUpDownBackdropDelay";
            this.numericUpDownBackdropDelay.Size = new System.Drawing.Size(44, 20);
            this.numericUpDownBackdropDelay.TabIndex = 6;
            this.numericUpDownBackdropDelay.Value = new decimal(new int[] {
            250,
            0,
            0,
            0});
            this.numericUpDownBackdropDelay.ValueChanged += new System.EventHandler(this.numericUpDownBackdropDelay_ValueChanged);
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(328, 52);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(68, 13);
            this.label11.TabIndex = 22;
            this.label11.Text = "Lo&g Level:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboLogLevel
            // 
            this.comboLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLogLevel.FormattingEnabled = true;
            this.comboLogLevel.Items.AddRange(new object[] {
            "Normal",
            "Debug",
            "Debug+SQL (huge!)"});
            this.comboLogLevel.Location = new System.Drawing.Point(402, 47);
            this.comboLogLevel.Name = "comboLogLevel";
            this.comboLogLevel.Size = new System.Drawing.Size(119, 21);
            this.comboLogLevel.TabIndex = 23;
            this.comboLogLevel.SelectedIndexChanged += new System.EventHandler(this.comboLogLevel_SelectedIndexChanged);
            // 
            // chkShowSeriesFanart
            // 
            this.chkShowSeriesFanart.AutoSize = true;
            this.chkShowSeriesFanart.Location = new System.Drawing.Point(404, 192);
            this.chkShowSeriesFanart.Name = "chkShowSeriesFanart";
            this.chkShowSeriesFanart.Size = new System.Drawing.Size(154, 17);
            this.chkShowSeriesFanart.TabIndex = 29;
            this.chkShowSeriesFanart.Text = "&Show Fanart in Series view";
            this.chkShowSeriesFanart.UseVisualStyleBackColor = true;
            this.chkShowSeriesFanart.CheckedChanged += new System.EventHandler(this.chkShowSeriesFanart_CheckedChanged);
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(209, 50);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(15, 13);
            this.label39.TabIndex = 4;
            this.label39.Text = "%";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(9, 50);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(141, 13);
            this.label38.TabIndex = 2;
            this.label38.Text = "An episode is &watched after:";
            // 
            // label32
            // 
            this.label32.Location = new System.Drawing.Point(329, 23);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(67, 13);
            this.label32.TabIndex = 20;
            this.label32.Text = "&Language:";
            this.label32.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(9, 23);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(141, 13);
            this.label16.TabIndex = 0;
            this.label16.Text = "&Name of the plugin in Home:";
            // 
            // tab_view
            // 
            this.tab_view.AutoScroll = true;
            this.tab_view.Controls.Add(this.playlistSettings);
            this.tab_view.Controls.Add(this.groupBox8);
            this.tab_view.ImageIndex = 2;
            this.tab_view.Location = new System.Drawing.Point(4, 31);
            this.tab_view.Name = "tab_view";
            this.tab_view.Padding = new System.Windows.Forms.Padding(3);
            this.tab_view.Size = new System.Drawing.Size(766, 513);
            this.tab_view.TabIndex = 7;
            this.tab_view.Text = "Views/Filters";
            this.tab_view.UseVisualStyleBackColor = true;
            // 
            // playlistSettings
            // 
            this.playlistSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.playlistSettings.AutoScroll = true;
            this.playlistSettings.Location = new System.Drawing.Point(-1, 353);
            this.playlistSettings.Name = "playlistSettings";
            this.playlistSettings.Size = new System.Drawing.Size(761, 157);
            this.playlistSettings.TabIndex = 1;
            // 
            // groupBox8
            // 
            this.groupBox8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox8.Controls.Add(this.buttonViewTemplates);
            this.groupBox8.Controls.Add(this.buttonPinCode);
            this.groupBox8.Controls.Add(this.buttonEditView);
            this.groupBox8.Controls.Add(this.checkBoxParentalControl);
            this.groupBox8.Controls.Add(this.label25);
            this.groupBox8.Controls.Add(this.checkCurViewEnabled);
            this.groupBox8.Controls.Add(this.lnkResetView);
            this.groupBox8.Controls.Add(this.btnRemoveView);
            this.groupBox8.Controls.Add(this.view_selStepsList);
            this.groupBox8.Controls.Add(this.btnAddView);
            this.groupBox8.Controls.Add(this._availViews);
            this.groupBox8.Controls.Add(this.label1);
            this.groupBox8.Controls.Add(this.view_selectedName);
            this.groupBox8.Location = new System.Drawing.Point(3, 6);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(755, 341);
            this.groupBox8.TabIndex = 0;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Customize Views";
            // 
            // buttonPinCode
            // 
            this.buttonPinCode.Location = new System.Drawing.Point(260, 227);
            this.buttonPinCode.Name = "buttonPinCode";
            this.buttonPinCode.Size = new System.Drawing.Size(75, 23);
            this.buttonPinCode.TabIndex = 8;
            this.buttonPinCode.Text = "Pin &Code...";
            this.buttonPinCode.UseVisualStyleBackColor = true;
            this.buttonPinCode.Click += new System.EventHandler(this.buttonPinCode_Click);
            // 
            // buttonEditView
            // 
            this.buttonEditView.Enabled = false;
            this.buttonEditView.Location = new System.Drawing.Point(429, 113);
            this.buttonEditView.Name = "buttonEditView";
            this.buttonEditView.Size = new System.Drawing.Size(75, 23);
            this.buttonEditView.TabIndex = 7;
            this.buttonEditView.Text = "&Edit";
            this.buttonEditView.UseVisualStyleBackColor = true;
            this.buttonEditView.Click += new System.EventHandler(this.buttonEditView_Click);
            // 
            // checkBoxParentalControl
            // 
            this.checkBoxParentalControl.AutoSize = true;
            this.checkBoxParentalControl.Location = new System.Drawing.Point(240, 204);
            this.checkBoxParentalControl.Name = "checkBoxParentalControl";
            this.checkBoxParentalControl.Size = new System.Drawing.Size(215, 17);
            this.checkBoxParentalControl.TabIndex = 6;
            this.checkBoxParentalControl.Text = "&Prompt for Pin Code when entering view";
            this.checkBoxParentalControl.UseVisualStyleBackColor = true;
            this.checkBoxParentalControl.CheckedChanged += new System.EventHandler(this.checkBoxParentalControl_CheckedChanged);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(237, 65);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(52, 13);
            this.label25.TabIndex = 3;
            this.label25.Text = "Hierarchy";
            // 
            // checkCurViewEnabled
            // 
            this.checkCurViewEnabled.AutoSize = true;
            this.checkCurViewEnabled.Location = new System.Drawing.Point(240, 180);
            this.checkCurViewEnabled.Name = "checkCurViewEnabled";
            this.checkCurViewEnabled.Size = new System.Drawing.Size(164, 17);
            this.checkCurViewEnabled.TabIndex = 2;
            this.checkCurViewEnabled.Text = "&View is available for selection";
            this.checkCurViewEnabled.UseVisualStyleBackColor = true;
            this.checkCurViewEnabled.CheckedChanged += new System.EventHandler(this.checkCurViewEnabled_CheckedChanged);
            // 
            // lnkResetView
            // 
            this.lnkResetView.AutoSize = true;
            this.lnkResetView.Location = new System.Drawing.Point(126, 297);
            this.lnkResetView.Name = "lnkResetView";
            this.lnkResetView.Size = new System.Drawing.Size(89, 13);
            this.lnkResetView.TabIndex = 3;
            this.lnkResetView.TabStop = true;
            this.lnkResetView.Text = "Reset to Defaults";
            this.lnkResetView.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkResetView_LinkClicked);
            // 
            // btnRemoveView
            // 
            this.btnRemoveView.Location = new System.Drawing.Point(429, 142);
            this.btnRemoveView.Name = "btnRemoveView";
            this.btnRemoveView.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveView.TabIndex = 4;
            this.btnRemoveView.Text = "&Remove";
            this.btnRemoveView.UseVisualStyleBackColor = true;
            this.btnRemoveView.Click += new System.EventHandler(this.btnRemoveView_Click);
            // 
            // view_selStepsList
            // 
            this.view_selStepsList.Enabled = false;
            this.view_selStepsList.FormattingEnabled = true;
            this.view_selStepsList.Location = new System.Drawing.Point(240, 83);
            this.view_selStepsList.Name = "view_selStepsList";
            this.view_selStepsList.Size = new System.Drawing.Size(173, 82);
            this.view_selStepsList.TabIndex = 4;
            // 
            // btnAddView
            // 
            this.btnAddView.Location = new System.Drawing.Point(429, 83);
            this.btnAddView.Name = "btnAddView";
            this.btnAddView.Size = new System.Drawing.Size(75, 23);
            this.btnAddView.TabIndex = 5;
            this.btnAddView.Text = "&Add";
            this.btnAddView.UseVisualStyleBackColor = true;
            this.btnAddView.Click += new System.EventHandler(this.btnAddView_Click);
            // 
            // _availViews
            // 
            this._availViews.FormattingEnabled = true;
            this._availViews.Location = new System.Drawing.Point(26, 35);
            this._availViews.Name = "_availViews";
            this._availViews.Size = new System.Drawing.Size(189, 251);
            this._availViews.TabIndex = 0;
            this._availViews.SelectedIndexChanged += new System.EventHandler(this._availViews_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(237, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // view_selectedName
            // 
            this.view_selectedName.Location = new System.Drawing.Point(289, 32);
            this.view_selectedName.Name = "view_selectedName";
            this.view_selectedName.Size = new System.Drawing.Size(124, 20);
            this.view_selectedName.TabIndex = 1;
            this.view_selectedName.TextChanged += new System.EventHandler(this.view_selectedName_TextChanged);
            // 
            // tabLogoRules
            // 
            this.tabLogoRules.Controls.Add(this.label65);
            this.tabLogoRules.Controls.Add(this.groupBox7);
            this.tabLogoRules.ImageIndex = 5;
            this.tabLogoRules.Location = new System.Drawing.Point(4, 31);
            this.tabLogoRules.Name = "tabLogoRules";
            this.tabLogoRules.Padding = new System.Windows.Forms.Padding(3);
            this.tabLogoRules.Size = new System.Drawing.Size(766, 513);
            this.tabLogoRules.TabIndex = 9;
            this.tabLogoRules.Text = "Logos";
            this.tabLogoRules.UseVisualStyleBackColor = true;
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Location = new System.Drawing.Point(11, 7);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(592, 26);
            this.label65.TabIndex = 11;
            this.label65.Text = "Logo Rules will be overridden by skin if defined, to override set the Import attr" +
                "ibute to false in your TVSeries.SkinSettings.xml \r\nlocated in your skin director" +
                "y.\r\n";
            // 
            // groupBox7
            // 
            this.groupBox7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox7.Controls.Add(this.btnLogoDeleteAll);
            this.groupBox7.Controls.Add(this.btnLogoDown);
            this.groupBox7.Controls.Add(this.btnlogoUp);
            this.groupBox7.Controls.Add(this.lnkLogoImp);
            this.groupBox7.Controls.Add(this.lstLogos);
            this.groupBox7.Controls.Add(this.btnLogoTemplate);
            this.groupBox7.Controls.Add(this.btnrmvLogo);
            this.groupBox7.Controls.Add(this.lnkLogoExport);
            this.groupBox7.Controls.Add(this.btnLogoEdit);
            this.groupBox7.Controls.Add(this.addLogo);
            this.groupBox7.Location = new System.Drawing.Point(8, 43);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(752, 464);
            this.groupBox7.TabIndex = 5;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Logo Configuration";
            // 
            // btnLogoDeleteAll
            // 
            this.btnLogoDeleteAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogoDeleteAll.Location = new System.Drawing.Point(458, 434);
            this.btnLogoDeleteAll.Name = "btnLogoDeleteAll";
            this.btnLogoDeleteAll.Size = new System.Drawing.Size(75, 23);
            this.btnLogoDeleteAll.TabIndex = 6;
            this.btnLogoDeleteAll.Text = "Delete All";
            this.btnLogoDeleteAll.UseVisualStyleBackColor = true;
            this.btnLogoDeleteAll.Click += new System.EventHandler(this.btnLogoDeleteAll_Click);
            // 
            // tabFormattingRules
            // 
            this.tabFormattingRules.Controls.Add(this.label64);
            this.tabFormattingRules.Controls.Add(this.formattingConfiguration1);
            this.tabFormattingRules.ImageIndex = 6;
            this.tabFormattingRules.Location = new System.Drawing.Point(4, 31);
            this.tabFormattingRules.Name = "tabFormattingRules";
            this.tabFormattingRules.Padding = new System.Windows.Forms.Padding(3);
            this.tabFormattingRules.Size = new System.Drawing.Size(766, 513);
            this.tabFormattingRules.TabIndex = 10;
            this.tabFormattingRules.Text = "Formatting";
            this.tabFormattingRules.UseVisualStyleBackColor = true;
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(11, 7);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(617, 26);
            this.label64.TabIndex = 11;
            this.label64.Text = "Formatting Rules will be overridden by skin if defined, to override set the Impor" +
                "t attribute to false in your TVSeries.SkinSettings.xml \r\nlocated in your skin di" +
                "rectory.\r\n";
            // 
            // formattingConfiguration1
            // 
            this.formattingConfiguration1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.formattingConfiguration1.Location = new System.Drawing.Point(8, 44);
            this.formattingConfiguration1.Name = "formattingConfiguration1";
            this.formattingConfiguration1.Size = new System.Drawing.Size(750, 463);
            this.formattingConfiguration1.TabIndex = 6;
            // 
            // tabLayoutSettings
            // 
            this.tabLayoutSettings.AutoScroll = true;
            this.tabLayoutSettings.Controls.Add(this.label37);
            this.tabLayoutSettings.Controls.Add(this.groupBox3);
            this.tabLayoutSettings.Controls.Add(this.groupBox4);
            this.tabLayoutSettings.Controls.Add(this.groupBox2);
            this.tabLayoutSettings.ImageIndex = 3;
            this.tabLayoutSettings.Location = new System.Drawing.Point(4, 31);
            this.tabLayoutSettings.Name = "tabLayoutSettings";
            this.tabLayoutSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabLayoutSettings.Size = new System.Drawing.Size(766, 513);
            this.tabLayoutSettings.TabIndex = 11;
            this.tabLayoutSettings.Text = "Layout";
            this.tabLayoutSettings.UseVisualStyleBackColor = true;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(11, 7);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(644, 26);
            this.label37.TabIndex = 10;
            this.label37.Text = "Listing Format Settings will be overridden by skin if defined, to override set th" +
                "e Import attribute to false in your TVSeries.SkinSettings.xml \r\nlocated in your " +
                "skin directory.\r\n";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.richTextBox_seasonFormat_Col3);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.richTextBox_seasonFormat_Main);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.richTextBox_seasonFormat_Col1);
            this.groupBox3.Controls.Add(this.richTextBox_seasonFormat_Subtitle);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.richTextBox_seasonFormat_Col2);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.richTextBox_seasonFormat_Title);
            this.groupBox3.Location = new System.Drawing.Point(10, 166);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(745, 121);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Season View";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(6, 47);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(85, 13);
            this.label12.TabIndex = 5;
            this.label12.Text = "Title Format:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(6, 25);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(85, 13);
            this.label13.TabIndex = 0;
            this.label13.Text = "Listing Format:";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(6, 68);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(85, 13);
            this.label14.TabIndex = 7;
            this.label14.Text = "Subtitle Format:";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(6, 89);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(85, 13);
            this.label15.TabIndex = 9;
            this.label15.Text = "Main Format:";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.richTextBox_episodeFormat_Col3);
            this.groupBox4.Controls.Add(this.richTextBox_episodeFormat_Col2);
            this.groupBox4.Controls.Add(this.richTextBox_episodeFormat_Main);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.richTextBox_episodeFormat_Subtitle);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.richTextBox_episodeFormat_Title);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.richTextBox_episodeFormat_Col1);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Location = new System.Drawing.Point(10, 293);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(745, 121);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Episode View";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(6, 25);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Listing Format:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(6, 46);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(85, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "Title Format:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(6, 67);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(85, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Subtitle Format:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(6, 90);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(85, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "Main Format:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.richTextBox_seriesFormat_Col3);
            this.groupBox2.Controls.Add(this.richTextBox_seriesFormat_Main);
            this.groupBox2.Controls.Add(this.richTextBox_seriesFormat_Subtitle);
            this.groupBox2.Controls.Add(this.richTextBox_seriesFormat_Col2);
            this.groupBox2.Controls.Add(this.richTextBox_seriesFormat_Title);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.richTextBox_seriesFormat_Col1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(10, 39);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(745, 121);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Series View";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Listing Format:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 92);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Main Format:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 71);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Subtitle Format:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Title Format:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabpage_Extra
            // 
            this.tabpage_Extra.Controls.Add(this.splitContainer_treeextra);
            this.tabpage_Extra.ImageIndex = 7;
            this.tabpage_Extra.Location = new System.Drawing.Point(4, 31);
            this.tabpage_Extra.Name = "tabpage_Extra";
            this.tabpage_Extra.Padding = new System.Windows.Forms.Padding(3);
            this.tabpage_Extra.Size = new System.Drawing.Size(766, 513);
            this.tabpage_Extra.TabIndex = 6;
            this.tabpage_Extra.Text = "Extras";
            this.tabpage_Extra.UseVisualStyleBackColor = true;
            // 
            // splitContainer_treeextra
            // 
            this.splitContainer_treeextra.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_treeextra.Location = new System.Drawing.Point(3, 3);
            this.splitContainer_treeextra.Name = "splitContainer_treeextra";
            // 
            // splitContainer_treeextra.Panel1
            // 
            this.splitContainer_treeextra.Panel1.Controls.Add(this.treeView_Extra);
            // 
            // splitContainer_treeextra.Panel2
            // 
            this.splitContainer_treeextra.Panel2.Controls.Add(this.panel_newssearch);
            this.splitContainer_treeextra.Panel2.Controls.Add(this.panel_newsroot);
            this.splitContainer_treeextra.Panel2.Controls.Add(this.panel_torrentroot);
            this.splitContainer_treeextra.Panel2.Controls.Add(this.panel_subtitleroot);
            this.splitContainer_treeextra.Panel2.Controls.Add(this.panel_torrentsearch);
            this.splitContainer_treeextra.Size = new System.Drawing.Size(760, 507);
            this.splitContainer_treeextra.SplitterDistance = 187;
            this.splitContainer_treeextra.TabIndex = 5;
            // 
            // treeView_Extra
            // 
            this.treeView_Extra.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_Extra.Location = new System.Drawing.Point(0, 0);
            this.treeView_Extra.Name = "treeView_Extra";
            this.treeView_Extra.Size = new System.Drawing.Size(187, 507);
            this.treeView_Extra.TabIndex = 0;
            this.treeView_Extra.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_Extra_AfterSelect);
            // 
            // panel_newssearch
            // 
            this.panel_newssearch.Controls.Add(this.label33);
            this.panel_newssearch.Controls.Add(this.label31);
            this.panel_newssearch.Controls.Add(this.textBox_NewsSearchLanguageRegex);
            this.panel_newssearch.Controls.Add(this.textBox_NewsSearchGroupRegex);
            this.panel_newssearch.Controls.Add(this.label47);
            this.panel_newssearch.Controls.Add(this.textBox_NewsSearchParseArticleRegex);
            this.panel_newssearch.Controls.Add(this.textBox_NewsSearchIsolateArticleRegex);
            this.panel_newssearch.Controls.Add(this.label46);
            this.panel_newssearch.Controls.Add(this.textBox_NewsSearchFormatRegex);
            this.panel_newssearch.Controls.Add(this.textBox_NewsSearchReportDateRegex);
            this.panel_newssearch.Controls.Add(this.label49);
            this.panel_newssearch.Controls.Add(this.label45);
            this.panel_newssearch.Controls.Add(this.label48);
            this.panel_newssearch.Controls.Add(this.textBox_NewsSearchPostDateRegex);
            this.panel_newssearch.Controls.Add(this.label44);
            this.panel_newssearch.Controls.Add(this.textBox_NewsSearchSizeRegex);
            this.panel_newssearch.Controls.Add(this.label43);
            this.panel_newssearch.Controls.Add(this.textBox_NewsSearchNameRegex);
            this.panel_newssearch.Controls.Add(this.label42);
            this.panel_newssearch.Controls.Add(this.textBox_NewsSearchIDRegex);
            this.panel_newssearch.Controls.Add(this.label41);
            this.panel_newssearch.Controls.Add(this.textBox_NewsSearchReportRegex);
            this.panel_newssearch.Controls.Add(this.label40);
            this.panel_newssearch.Controls.Add(this.label36);
            this.panel_newssearch.Controls.Add(this.label34);
            this.panel_newssearch.Controls.Add(this.textbox_NewzbinPassword);
            this.panel_newssearch.Controls.Add(this.textBox_NewzbinLogin);
            this.panel_newssearch.Controls.Add(this.textBox_NewsSearchUrl);
            this.panel_newssearch.Location = new System.Drawing.Point(262, 287);
            this.panel_newssearch.Name = "panel_newssearch";
            this.panel_newssearch.Size = new System.Drawing.Size(480, 410);
            this.panel_newssearch.TabIndex = 2;
            this.panel_newssearch.Tag = "News Search";
            this.panel_newssearch.Visible = false;
            // 
            // label33
            // 
            this.label33.Location = new System.Drawing.Point(4, 37);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(138, 20);
            this.label33.TabIndex = 2;
            this.label33.Text = "newzBin &Password:";
            this.label33.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label31
            // 
            this.label31.Location = new System.Drawing.Point(4, 11);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(138, 20);
            this.label31.TabIndex = 0;
            this.label31.Text = "newzBin &Login:";
            this.label31.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBox_NewsSearchLanguageRegex
            // 
            this.textBox_NewsSearchLanguageRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_NewsSearchLanguageRegex.Location = new System.Drawing.Point(147, 304);
            this.textBox_NewsSearchLanguageRegex.Name = "textBox_NewsSearchLanguageRegex";
            this.textBox_NewsSearchLanguageRegex.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_NewsSearchLanguageRegex.Size = new System.Drawing.Size(322, 20);
            this.textBox_NewsSearchLanguageRegex.TabIndex = 7;
            this.textBox_NewsSearchLanguageRegex.TextChanged += new System.EventHandler(this.textBox_NewsSearchLanguageRegex_TextChanged);
            // 
            // textBox_NewsSearchGroupRegex
            // 
            this.textBox_NewsSearchGroupRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_NewsSearchGroupRegex.Location = new System.Drawing.Point(147, 278);
            this.textBox_NewsSearchGroupRegex.Name = "textBox_NewsSearchGroupRegex";
            this.textBox_NewsSearchGroupRegex.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_NewsSearchGroupRegex.Size = new System.Drawing.Size(322, 20);
            this.textBox_NewsSearchGroupRegex.TabIndex = 7;
            this.textBox_NewsSearchGroupRegex.TextChanged += new System.EventHandler(this.textBox_NewsSearchGroupRegex_TextChanged);
            // 
            // label47
            // 
            this.label47.Location = new System.Drawing.Point(4, 307);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(138, 14);
            this.label47.TabIndex = 6;
            this.label47.Text = "Search L&anguage RegEx:";
            this.label47.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBox_NewsSearchParseArticleRegex
            // 
            this.textBox_NewsSearchParseArticleRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_NewsSearchParseArticleRegex.Location = new System.Drawing.Point(147, 366);
            this.textBox_NewsSearchParseArticleRegex.Name = "textBox_NewsSearchParseArticleRegex";
            this.textBox_NewsSearchParseArticleRegex.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_NewsSearchParseArticleRegex.Size = new System.Drawing.Size(322, 20);
            this.textBox_NewsSearchParseArticleRegex.TabIndex = 7;
            this.textBox_NewsSearchParseArticleRegex.TextChanged += new System.EventHandler(this.textBox_NewsSearchParseArticleRegex_TextChanged);
            // 
            // textBox_NewsSearchIsolateArticleRegex
            // 
            this.textBox_NewsSearchIsolateArticleRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_NewsSearchIsolateArticleRegex.Location = new System.Drawing.Point(147, 337);
            this.textBox_NewsSearchIsolateArticleRegex.Name = "textBox_NewsSearchIsolateArticleRegex";
            this.textBox_NewsSearchIsolateArticleRegex.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_NewsSearchIsolateArticleRegex.Size = new System.Drawing.Size(322, 20);
            this.textBox_NewsSearchIsolateArticleRegex.TabIndex = 7;
            this.textBox_NewsSearchIsolateArticleRegex.TextChanged += new System.EventHandler(this.textBox_NewsSearchIsolateArticleRegex_TextChanged);
            // 
            // label46
            // 
            this.label46.Location = new System.Drawing.Point(4, 281);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(138, 14);
            this.label46.TabIndex = 6;
            this.label46.Text = "Search &Group RegEx:";
            this.label46.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBox_NewsSearchFormatRegex
            // 
            this.textBox_NewsSearchFormatRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_NewsSearchFormatRegex.Location = new System.Drawing.Point(147, 252);
            this.textBox_NewsSearchFormatRegex.Name = "textBox_NewsSearchFormatRegex";
            this.textBox_NewsSearchFormatRegex.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_NewsSearchFormatRegex.Size = new System.Drawing.Size(322, 20);
            this.textBox_NewsSearchFormatRegex.TabIndex = 7;
            this.textBox_NewsSearchFormatRegex.TextChanged += new System.EventHandler(this.textBox_NewsSearchFormatRegex_TextChanged);
            // 
            // textBox_NewsSearchReportDateRegex
            // 
            this.textBox_NewsSearchReportDateRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_NewsSearchReportDateRegex.Location = new System.Drawing.Point(147, 226);
            this.textBox_NewsSearchReportDateRegex.Name = "textBox_NewsSearchReportDateRegex";
            this.textBox_NewsSearchReportDateRegex.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_NewsSearchReportDateRegex.Size = new System.Drawing.Size(322, 20);
            this.textBox_NewsSearchReportDateRegex.TabIndex = 7;
            this.textBox_NewsSearchReportDateRegex.TextChanged += new System.EventHandler(this.textBox_NewsSearchReportDateRegex_TextChanged);
            // 
            // label49
            // 
            this.label49.Location = new System.Drawing.Point(4, 368);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(138, 29);
            this.label49.TabIndex = 6;
            this.label49.Text = "Parse Article Name RegEx:";
            this.label49.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label45
            // 
            this.label45.Location = new System.Drawing.Point(4, 334);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(138, 29);
            this.label45.TabIndex = 6;
            this.label45.Text = "Isolate Article Name RegEx:";
            this.label45.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label48
            // 
            this.label48.Location = new System.Drawing.Point(2, 255);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(140, 14);
            this.label48.TabIndex = 6;
            this.label48.Text = "Search &Format Date RegEx:";
            this.label48.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBox_NewsSearchPostDateRegex
            // 
            this.textBox_NewsSearchPostDateRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_NewsSearchPostDateRegex.Location = new System.Drawing.Point(147, 200);
            this.textBox_NewsSearchPostDateRegex.Name = "textBox_NewsSearchPostDateRegex";
            this.textBox_NewsSearchPostDateRegex.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_NewsSearchPostDateRegex.Size = new System.Drawing.Size(322, 20);
            this.textBox_NewsSearchPostDateRegex.TabIndex = 7;
            this.textBox_NewsSearchPostDateRegex.TextChanged += new System.EventHandler(this.textBox_NewsSearchPostDateRegex_TextChanged);
            // 
            // label44
            // 
            this.label44.Location = new System.Drawing.Point(2, 229);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(140, 14);
            this.label44.TabIndex = 6;
            this.label44.Text = "Search R&eport Date RegEx:";
            this.label44.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBox_NewsSearchSizeRegex
            // 
            this.textBox_NewsSearchSizeRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_NewsSearchSizeRegex.Location = new System.Drawing.Point(147, 174);
            this.textBox_NewsSearchSizeRegex.Name = "textBox_NewsSearchSizeRegex";
            this.textBox_NewsSearchSizeRegex.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_NewsSearchSizeRegex.Size = new System.Drawing.Size(322, 20);
            this.textBox_NewsSearchSizeRegex.TabIndex = 7;
            this.textBox_NewsSearchSizeRegex.TextChanged += new System.EventHandler(this.textBox_NewsSearchSizeRegex_TextChanged);
            // 
            // label43
            // 
            this.label43.Location = new System.Drawing.Point(4, 203);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(138, 14);
            this.label43.TabIndex = 6;
            this.label43.Text = "Search P&ost Date RegEx:";
            this.label43.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBox_NewsSearchNameRegex
            // 
            this.textBox_NewsSearchNameRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_NewsSearchNameRegex.Location = new System.Drawing.Point(147, 148);
            this.textBox_NewsSearchNameRegex.Name = "textBox_NewsSearchNameRegex";
            this.textBox_NewsSearchNameRegex.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_NewsSearchNameRegex.Size = new System.Drawing.Size(322, 20);
            this.textBox_NewsSearchNameRegex.TabIndex = 7;
            this.textBox_NewsSearchNameRegex.TextChanged += new System.EventHandler(this.textBox_NewsSearchNameRegex_TextChanged);
            // 
            // label42
            // 
            this.label42.Location = new System.Drawing.Point(4, 177);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(138, 14);
            this.label42.TabIndex = 6;
            this.label42.Text = "Search &Size RegEx:";
            this.label42.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBox_NewsSearchIDRegex
            // 
            this.textBox_NewsSearchIDRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_NewsSearchIDRegex.Location = new System.Drawing.Point(147, 122);
            this.textBox_NewsSearchIDRegex.Name = "textBox_NewsSearchIDRegex";
            this.textBox_NewsSearchIDRegex.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_NewsSearchIDRegex.Size = new System.Drawing.Size(322, 20);
            this.textBox_NewsSearchIDRegex.TabIndex = 7;
            this.textBox_NewsSearchIDRegex.TextChanged += new System.EventHandler(this.textBox_NewsSearchIDRegex_TextChanged);
            // 
            // label41
            // 
            this.label41.Location = new System.Drawing.Point(4, 151);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(138, 14);
            this.label41.TabIndex = 6;
            this.label41.Text = "Search &Name RegEx:";
            this.label41.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label40
            // 
            this.label40.Location = new System.Drawing.Point(4, 125);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(138, 14);
            this.label40.TabIndex = 6;
            this.label40.Text = "Search &ID RegEx:";
            this.label40.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label36
            // 
            this.label36.Location = new System.Drawing.Point(4, 63);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(138, 20);
            this.label36.TabIndex = 4;
            this.label36.Text = "newzBin Search &URL:";
            this.label36.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label34
            // 
            this.label34.Location = new System.Drawing.Point(4, 94);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(138, 14);
            this.label34.TabIndex = 6;
            this.label34.Text = "Search &Report RegEx:";
            this.label34.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textbox_NewzbinPassword
            // 
            this.textbox_NewzbinPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textbox_NewzbinPassword.Location = new System.Drawing.Point(147, 34);
            this.textbox_NewzbinPassword.Name = "textbox_NewzbinPassword";
            this.textbox_NewzbinPassword.Size = new System.Drawing.Size(322, 20);
            this.textbox_NewzbinPassword.TabIndex = 3;
            this.textbox_NewzbinPassword.UseSystemPasswordChar = true;
            this.textbox_NewzbinPassword.TextChanged += new System.EventHandler(this.textbox_NewzbinPassword_TextChanged);
            // 
            // textBox_NewzbinLogin
            // 
            this.textBox_NewzbinLogin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_NewzbinLogin.Location = new System.Drawing.Point(147, 8);
            this.textBox_NewzbinLogin.Name = "textBox_NewzbinLogin";
            this.textBox_NewzbinLogin.Size = new System.Drawing.Size(322, 20);
            this.textBox_NewzbinLogin.TabIndex = 1;
            this.textBox_NewzbinLogin.TextChanged += new System.EventHandler(this.textBox_NewzbinLogin_TextChanged);
            // 
            // textBox_NewsSearchUrl
            // 
            this.textBox_NewsSearchUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_NewsSearchUrl.Location = new System.Drawing.Point(147, 60);
            this.textBox_NewsSearchUrl.Name = "textBox_NewsSearchUrl";
            this.textBox_NewsSearchUrl.Size = new System.Drawing.Size(322, 20);
            this.textBox_NewsSearchUrl.TabIndex = 5;
            this.textBox_NewsSearchUrl.TextChanged += new System.EventHandler(this.textBox_NewsSearchUrl_TextChanged);
            // 
            // panel_newsroot
            // 
            this.panel_newsroot.Controls.Add(this.button_NewsDownloadPathBrowse);
            this.panel_newsroot.Controls.Add(this.button_newsleecherbrowse);
            this.panel_newsroot.Controls.Add(this.label35);
            this.panel_newsroot.Controls.Add(this.textBox_NewsDownloadPath);
            this.panel_newsroot.Controls.Add(this.label30);
            this.panel_newsroot.Controls.Add(this.textBox_newsleecher);
            this.panel_newsroot.Location = new System.Drawing.Point(338, 222);
            this.panel_newsroot.Name = "panel_newsroot";
            this.panel_newsroot.Size = new System.Drawing.Size(226, 59);
            this.panel_newsroot.TabIndex = 5;
            this.panel_newsroot.Tag = "News";
            this.panel_newsroot.Visible = false;
            // 
            // button_NewsDownloadPathBrowse
            // 
            this.button_NewsDownloadPathBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_NewsDownloadPathBrowse.Location = new System.Drawing.Point(192, 30);
            this.button_NewsDownloadPathBrowse.Name = "button_NewsDownloadPathBrowse";
            this.button_NewsDownloadPathBrowse.Size = new System.Drawing.Size(26, 23);
            this.button_NewsDownloadPathBrowse.TabIndex = 5;
            this.button_NewsDownloadPathBrowse.Text = "...";
            this.button_NewsDownloadPathBrowse.UseVisualStyleBackColor = true;
            this.button_NewsDownloadPathBrowse.Click += new System.EventHandler(this.button_NewsDownloadPathBrowse_Click);
            // 
            // button_newsleecherbrowse
            // 
            this.button_newsleecherbrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_newsleecherbrowse.Location = new System.Drawing.Point(192, 3);
            this.button_newsleecherbrowse.Name = "button_newsleecherbrowse";
            this.button_newsleecherbrowse.Size = new System.Drawing.Size(26, 23);
            this.button_newsleecherbrowse.TabIndex = 2;
            this.button_newsleecherbrowse.Text = "...";
            this.button_newsleecherbrowse.UseVisualStyleBackColor = true;
            this.button_newsleecherbrowse.Click += new System.EventHandler(this.button_newsleecherbrowse_Click);
            // 
            // label35
            // 
            this.label35.Location = new System.Drawing.Point(4, 35);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(138, 18);
            this.label35.TabIndex = 3;
            this.label35.Text = "&Download Path to Monitor:";
            this.label35.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBox_NewsDownloadPath
            // 
            this.textBox_NewsDownloadPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_NewsDownloadPath.Location = new System.Drawing.Point(147, 32);
            this.textBox_NewsDownloadPath.Name = "textBox_NewsDownloadPath";
            this.textBox_NewsDownloadPath.Size = new System.Drawing.Size(37, 20);
            this.textBox_NewsDownloadPath.TabIndex = 4;
            this.textBox_NewsDownloadPath.TextChanged += new System.EventHandler(this.textBox_NewsDownloadPath_TextChanged);
            // 
            // label30
            // 
            this.label30.Location = new System.Drawing.Point(4, 9);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(138, 18);
            this.label30.TabIndex = 0;
            this.label30.Text = "&NewsLeecher Path:";
            this.label30.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBox_newsleecher
            // 
            this.textBox_newsleecher.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_newsleecher.Location = new System.Drawing.Point(147, 6);
            this.textBox_newsleecher.Name = "textBox_newsleecher";
            this.textBox_newsleecher.Size = new System.Drawing.Size(37, 20);
            this.textBox_newsleecher.TabIndex = 1;
            this.textBox_newsleecher.TextChanged += new System.EventHandler(this.textBox_uTorrentPath_TextChanged);
            // 
            // panel_torrentroot
            // 
            this.panel_torrentroot.Controls.Add(this.button_uTorrentBrowse);
            this.panel_torrentroot.Controls.Add(this.label19);
            this.panel_torrentroot.Controls.Add(this.textBox_uTorrentPath);
            this.panel_torrentroot.Location = new System.Drawing.Point(30, 165);
            this.panel_torrentroot.Name = "panel_torrentroot";
            this.panel_torrentroot.Size = new System.Drawing.Size(226, 45);
            this.panel_torrentroot.TabIndex = 4;
            this.panel_torrentroot.Tag = "Torrents";
            // 
            // button_uTorrentBrowse
            // 
            this.button_uTorrentBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_uTorrentBrowse.Location = new System.Drawing.Point(192, 9);
            this.button_uTorrentBrowse.Name = "button_uTorrentBrowse";
            this.button_uTorrentBrowse.Size = new System.Drawing.Size(26, 23);
            this.button_uTorrentBrowse.TabIndex = 2;
            this.button_uTorrentBrowse.Text = "...";
            this.button_uTorrentBrowse.UseVisualStyleBackColor = true;
            this.button_uTorrentBrowse.Click += new System.EventHandler(this.button_uTorrentBrowse_Click);
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(4, 14);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(138, 18);
            this.label19.TabIndex = 0;
            this.label19.Text = "&uTorrent Path:";
            this.label19.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // panel_subtitleroot
            // 
            this.panel_subtitleroot.Controls.Add(this.subtitleDownloader_enabled);
            this.panel_subtitleroot.Location = new System.Drawing.Point(8, 12);
            this.panel_subtitleroot.Name = "panel_subtitleroot";
            this.panel_subtitleroot.Size = new System.Drawing.Size(315, 147);
            this.panel_subtitleroot.TabIndex = 0;
            this.panel_subtitleroot.Tag = "Subtitles";
            // 
            // subtitleDownloader_enabled
            // 
            this.subtitleDownloader_enabled.AutoSize = true;
            this.subtitleDownloader_enabled.Location = new System.Drawing.Point(12, 13);
            this.subtitleDownloader_enabled.Name = "subtitleDownloader_enabled";
            this.subtitleDownloader_enabled.Size = new System.Drawing.Size(154, 17);
            this.subtitleDownloader_enabled.TabIndex = 0;
            this.subtitleDownloader_enabled.Text = "Enable SubtitleDownloader";
            this.subtitleDownloader_enabled.UseVisualStyleBackColor = true;
            this.subtitleDownloader_enabled.CheckedChanged += new System.EventHandler(this.subtitleDownloader_enabled_CheckedChanged);
            // 
            // panel_torrentsearch
            // 
            this.panel_torrentsearch.Controls.Add(this.textBox_TorrentDetailsRegex);
            this.panel_torrentsearch.Controls.Add(this.label20);
            this.panel_torrentsearch.Controls.Add(this.textBox_TorrentSearchRegex);
            this.panel_torrentsearch.Controls.Add(this.label21);
            this.panel_torrentsearch.Controls.Add(this.textBox_TorrentDetailsUrl);
            this.panel_torrentsearch.Controls.Add(this.label23);
            this.panel_torrentsearch.Controls.Add(this.textBox_TorrentSearchUrl);
            this.panel_torrentsearch.Controls.Add(this.label22);
            this.panel_torrentsearch.Controls.Add(this.comboBox_TorrentPreset);
            this.panel_torrentsearch.Controls.Add(this.label24);
            this.panel_torrentsearch.Location = new System.Drawing.Point(338, 12);
            this.panel_torrentsearch.Name = "panel_torrentsearch";
            this.panel_torrentsearch.Size = new System.Drawing.Size(226, 204);
            this.panel_torrentsearch.TabIndex = 3;
            this.panel_torrentsearch.Tag = "Torrent Searches";
            this.panel_torrentsearch.Visible = false;
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(4, 10);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(138, 20);
            this.label20.TabIndex = 0;
            this.label20.Text = "&Web Search Preset:";
            this.label20.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label21
            // 
            this.label21.Location = new System.Drawing.Point(4, 38);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(138, 20);
            this.label21.TabIndex = 2;
            this.label21.Text = "Search &URL:";
            this.label21.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label23
            // 
            this.label23.Location = new System.Drawing.Point(4, 123);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(138, 20);
            this.label23.TabIndex = 6;
            this.label23.Text = "&Details URL:";
            this.label23.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label22
            // 
            this.label22.Location = new System.Drawing.Point(4, 64);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(138, 20);
            this.label22.TabIndex = 4;
            this.label22.Text = "&Search RegEx:";
            this.label22.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // comboBox_TorrentPreset
            // 
            this.comboBox_TorrentPreset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_TorrentPreset.FormattingEnabled = true;
            this.comboBox_TorrentPreset.Location = new System.Drawing.Point(147, 7);
            this.comboBox_TorrentPreset.Name = "comboBox_TorrentPreset";
            this.comboBox_TorrentPreset.Size = new System.Drawing.Size(68, 21);
            this.comboBox_TorrentPreset.Sorted = true;
            this.comboBox_TorrentPreset.TabIndex = 1;
            this.comboBox_TorrentPreset.SelectedIndexChanged += new System.EventHandler(this.comboBox_TorrentPreset_SelectedIndexChanged);
            this.comboBox_TorrentPreset.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBox_TorrentPreset_KeyDown);
            // 
            // label24
            // 
            this.label24.Location = new System.Drawing.Point(4, 149);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(138, 20);
            this.label24.TabIndex = 8;
            this.label24.Text = "Details &RegEx:";
            this.label24.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tabAbout
            // 
            this.tabAbout.AutoScroll = true;
            this.tabAbout.Controls.Add(this.aboutScreen);
            this.tabAbout.ImageIndex = 4;
            this.tabAbout.Location = new System.Drawing.Point(4, 31);
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.Padding = new System.Windows.Forms.Padding(3);
            this.tabAbout.Size = new System.Drawing.Size(766, 513);
            this.tabAbout.TabIndex = 8;
            this.tabAbout.Text = "About";
            this.tabAbout.UseVisualStyleBackColor = true;
            // 
            // aboutScreen
            // 
            this.aboutScreen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.aboutScreen.AutoScroll = true;
            this.aboutScreen.BackColor = System.Drawing.Color.White;
            this.aboutScreen.Location = new System.Drawing.Point(2, 3);
            this.aboutScreen.Name = "aboutScreen";
            this.aboutScreen.Size = new System.Drawing.Size(764, 510);
            this.aboutScreen.TabIndex = 0;
            // 
            // listBox_Log
            // 
            this.listBox_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox_Log.FormattingEnabled = true;
            this.listBox_Log.HorizontalScrollbar = true;
            this.listBox_Log.Location = new System.Drawing.Point(10, 10);
            this.listBox_Log.Name = "listBox_Log";
            this.listBox_Log.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listBox_Log.Size = new System.Drawing.Size(130, 17);
            this.listBox_Log.TabIndex = 5;
            // 
            // ConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 562);
            this.Controls.Add(this.splitMain_Log);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(700, 450);
            this.Name = "ConfigurationForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MP-TV Series Configuration";
            this.contextMenuStrip_DetailsTree.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Replace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWatchedAfter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScanRemoteShareFrequency)).EndInit();
            this.splitMain_Log.Panel1.ResumeLayout(false);
            this.splitMain_Log.Panel2.ResumeLayout(false);
            this.splitMain_Log.ResumeLayout(false);
            this.tabControl_Details.ResumeLayout(false);
            this.tabPage_Details.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SeriesPoster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.detailsPropertyBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Series)).EndInit();
            this.panDBLocation.ResumeLayout(false);
            this.panDBLocation.PerformLayout();
            this.tabPage_Import.ResumeLayout(false);
            this.splitContainer_SettingsOutput.Panel1.ResumeLayout(false);
            this.splitContainer_SettingsOutput.Panel2.ResumeLayout(false);
            this.splitContainer_SettingsOutput.ResumeLayout(false);
            this.splitContainerImportSettings.Panel1.ResumeLayout(false);
            this.splitContainerImportSettings.Panel2.ResumeLayout(false);
            this.splitContainerImportSettings.ResumeLayout(false);
            this.panel_Expressions.ResumeLayout(false);
            this.panel_Expressions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Expressions)).EndInit();
            this.panel_StringReplacements.ResumeLayout(false);
            this.panel_StringReplacements.PerformLayout();
            this.panel_ImportPathes.ResumeLayout(false);
            this.panel_ImportPathes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ImportPathes)).EndInit();
            this.tabOnlineData.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            this.panel_OnlineData.ResumeLayout(false);
            this.panel_OnlineData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinMaxFanarts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_AutoOnlineDataRefresh)).EndInit();
            this.tabPage_MP_DisplayControl.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownArtworkDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBackdropDelay)).EndInit();
            this.tab_view.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.tabLogoRules.ResumeLayout(false);
            this.tabLogoRules.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.tabFormattingRules.ResumeLayout(false);
            this.tabFormattingRules.PerformLayout();
            this.tabLayoutSettings.ResumeLayout(false);
            this.tabLayoutSettings.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tabpage_Extra.ResumeLayout(false);
            this.splitContainer_treeextra.Panel1.ResumeLayout(false);
            this.splitContainer_treeextra.Panel2.ResumeLayout(false);
            this.splitContainer_treeextra.ResumeLayout(false);
            this.panel_newssearch.ResumeLayout(false);
            this.panel_newssearch.PerformLayout();
            this.panel_newsroot.ResumeLayout(false);
            this.panel_newsroot.PerformLayout();
            this.panel_torrentroot.ResumeLayout(false);
            this.panel_torrentroot.PerformLayout();
            this.panel_subtitleroot.ResumeLayout(false);
            this.panel_subtitleroot.PerformLayout();
            this.panel_torrentsearch.ResumeLayout(false);
            this.panel_torrentsearch.PerformLayout();
            this.tabAbout.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColumnHeader columnHeader_Series;
        private System.Windows.Forms.ColumnHeader columnHeader_Title;
        private System.Windows.Forms.ColumnHeader columnHeader_Season;
        private System.Windows.Forms.ColumnHeader columnHeader_Episode;
        private System.Windows.Forms.ColumnHeader columnHeader_OriginallyAired;
        private System.Windows.Forms.BindingSource detailsPropertyBindingSource;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TabControl tabControl_Details;
        private System.Windows.Forms.TabPage tabPage_Details;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TreeView treeView_Library;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.PictureBox pictureBox_Series;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.TabPage tabPage_Import;
        private System.Windows.Forms.DataGridView dataGridView_Expressions;
        private System.Windows.Forms.DataGridView dataGridView_ImportPathes;
        private System.Windows.Forms.Button button_MoveExpDown;
        private System.Windows.Forms.Button button_MoveExpUp;
        private System.Windows.Forms.TreeView treeView_Settings;
        private System.Windows.Forms.Panel panel_ImportPathes;
        private System.Windows.Forms.SplitContainer splitContainerImportSettings;
        private System.Windows.Forms.Panel panel_Expressions;
        private System.Windows.Forms.Button button_Start;
        private System.Windows.Forms.ProgressBar progressBar_Parsing;
        private System.Windows.Forms.SplitContainer splitContainer_SettingsOutput;
        private System.Windows.Forms.ListBox listBox_Log;
        private System.Windows.Forms.ComboBox comboBox_BannerSelection;
        private System.Windows.Forms.TabPage tabPage_MP_DisplayControl;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolTip toolTip_Help;
        private System.Windows.Forms.Panel panel_StringReplacements;
        private System.Windows.Forms.DataGridView dataGridView_Replace;
        private System.Windows.Forms.Button button_TestReparse;
        private System.Windows.Forms.ListView listView_ParsingResults;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_InsertFields;
        private System.Windows.Forms.TextBox textBox_PluginHomeName;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TabPage tabpage_Extra;
        private System.Windows.Forms.SplitContainer splitMain_Log;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBox_ShowHidden;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_DetailsTree;
        private System.Windows.Forms.ToolStripMenuItem hideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem watchedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unWatchedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getSubtitlesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem torrentThToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newzbinThisToolStripMenuItem;
        //private System.Windows.Forms.ToolStripMenuItem resetUserSelectionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ResetUserSelToolStripMenuItem;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Button button_uTorrentBrowse;
        private System.Windows.Forms.TextBox textBox_uTorrentPath;
        private System.Windows.Forms.TextBox textBox_TorrentSearchRegex;
        private System.Windows.Forms.TextBox textBox_TorrentSearchUrl;
        private System.Windows.Forms.ComboBox comboBox_TorrentPreset;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox textBox_TorrentDetailsUrl;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox textBox_TorrentDetailsRegex;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.CheckBox checkBox_RandBanner;
        private System.Windows.Forms.TabPage tab_view;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Button btnAddView;
        private System.Windows.Forms.ListBox _availViews;
        private System.Windows.Forms.Button btnRemoveView;
        private System.Windows.Forms.TextBox view_selectedName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.ListBox view_selStepsList;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.ComboBox comboLanguage;
        private System.Windows.Forms.CheckBox checkCurViewEnabled;
        private System.Windows.Forms.SplitContainer splitContainer_treeextra;
        private System.Windows.Forms.Panel panel_torrentsearch;
        private System.Windows.Forms.Panel panel_newsroot;
        private System.Windows.Forms.Panel panel_torrentroot;
        private System.Windows.Forms.Panel panel_subtitleroot;
        private System.Windows.Forms.Panel panel_newssearch;
        private System.Windows.Forms.Button button_newsleecherbrowse;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.TextBox textBox_newsleecher;
        private System.Windows.Forms.TreeView treeView_Extra;
        private System.Windows.Forms.Button button_dbbrowse;
        private System.Windows.Forms.TextBox textBox_dblocation;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.TextBox textBox_NewsSearchReportRegex;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.TextBox textBox_NewsSearchUrl;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.TextBox textbox_NewzbinPassword;
        private System.Windows.Forms.TextBox textBox_NewzbinLogin;
        private System.Windows.Forms.Button button_NewsDownloadPathBrowse;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.TextBox textBox_NewsDownloadPath;
        private System.Windows.Forms.LinkLabel lblClearDB;
        private System.Windows.Forms.Panel panDBLocation;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.NumericUpDown nudWatchedAfter;
        private System.Windows.Forms.LinkLabel resetExpr;
        private System.Windows.Forms.LinkLabel buildExpr;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.ToolTip toolTip_InfoHelp;
        private System.Windows.Forms.TextBox textBox_NewsSearchLanguageRegex;
        private System.Windows.Forms.TextBox textBox_NewsSearchGroupRegex;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.TextBox textBox_NewsSearchIsolateArticleRegex;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.TextBox textBox_NewsSearchReportDateRegex;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.TextBox textBox_NewsSearchPostDateRegex;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.TextBox textBox_NewsSearchSizeRegex;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.TextBox textBox_NewsSearchNameRegex;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.TextBox textBox_NewsSearchIDRegex;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.TextBox textBox_NewsSearchFormatRegex;
        private System.Windows.Forms.Label label48;
        private System.Windows.Forms.TextBox textBox_NewsSearchParseArticleRegex;
        private System.Windows.Forms.Label label49;
        private ManualEpisodeManagementPane panel_manualEpisodeManagement;
        private System.Windows.Forms.CheckBox chkUseRegionalDateFormatString;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.LinkLabel lnkResetView;
        private System.Windows.Forms.TabPage tabAbout;
        private About aboutScreen;
        private WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox optionAsk2Rate;
        private System.Windows.Forms.ComboBox comboBox_PosterSelection;
        private System.Windows.Forms.PictureBox pictureBox_SeriesPoster;
        private System.Windows.Forms.CheckBox checkBox_Episode_HideUnwatchedSummary;
        private System.Windows.Forms.CheckBox checkBox_Episode_HideUnwatchedThumbnail;
        private System.Windows.Forms.CheckBox checkBox_Episode_OnlyShowLocalFiles;
        private System.Windows.Forms.CheckBox chkShowSeriesFanart;
        private System.Windows.Forms.CheckBox checkBox_Series_UseSortName;
        private System.Windows.Forms.TabPage tabLogoRules;
        private System.Windows.Forms.TabPage tabFormattingRules;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button btnLogoDeleteAll;
        private System.Windows.Forms.LinkLabel lnkLogoImp;
        private System.Windows.Forms.LinkLabel lnkLogoExport;
        private System.Windows.Forms.Button btnLogoTemplate;
        private System.Windows.Forms.Button btnLogoEdit;
        private System.Windows.Forms.Button btnLogoDown;
        private System.Windows.Forms.Button btnlogoUp;
        private System.Windows.Forms.Button btnrmvLogo;
        private System.Windows.Forms.Button addLogo;
        private System.Windows.Forms.ListBox lstLogos;
        private WindowPlugins.GUITVSeries.Configuration.FormattingConfiguration formattingConfiguration1;
        private System.Windows.Forms.CheckBox chkAllowDeletes;
        private System.Windows.Forms.CheckBox checkBox_doFolderWatch;
        private System.Windows.Forms.CheckBox checkBox_DontClearMissingLocalFiles;
        private System.Windows.Forms.LinkLabel linkExWatched;
        private System.Windows.Forms.LinkLabel linkImpWatched;
        private System.Windows.Forms.TabPage tabLayoutSettings;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RichTextBox richTextBox_seasonFormat_Col3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.RichTextBox richTextBox_seasonFormat_Main;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.RichTextBox richTextBox_seasonFormat_Col1;
        private System.Windows.Forms.RichTextBox richTextBox_seasonFormat_Subtitle;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.RichTextBox richTextBox_seasonFormat_Col2;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.RichTextBox richTextBox_seasonFormat_Title;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RichTextBox richTextBox_episodeFormat_Col3;
        private System.Windows.Forms.RichTextBox richTextBox_episodeFormat_Col2;
        private System.Windows.Forms.RichTextBox richTextBox_episodeFormat_Main;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RichTextBox richTextBox_episodeFormat_Subtitle;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RichTextBox richTextBox_episodeFormat_Title;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RichTextBox richTextBox_episodeFormat_Col1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox richTextBox_seriesFormat_Col3;
        private System.Windows.Forms.RichTextBox richTextBox_seriesFormat_Main;
        private System.Windows.Forms.RichTextBox richTextBox_seriesFormat_Subtitle;
        private System.Windows.Forms.RichTextBox richTextBox_seriesFormat_Col2;
        private System.Windows.Forms.RichTextBox richTextBox_seriesFormat_Title;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox richTextBox_seriesFormat_Col1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label65;
        private System.Windows.Forms.Label label64;
        private System.Windows.Forms.TabPage tabOnlineData;
        private System.Windows.Forms.Panel panel_OnlineData;
        private System.Windows.Forms.LinkLabel linkMediaInfoUpdate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown_AutoOnlineDataRefresh;
        private System.Windows.Forms.CheckBox checkBox_AutoOnlineDataRefresh;
        private System.Windows.Forms.CheckBox checkBox_AutoChooseOrder;
        //private System.Windows.Forms.RadioButton radioButton_episode_AllEpisodes;
        //private System.Windows.Forms.Label label_textboxEmuleWSType;
        //private System.Windows.Forms.ComboBox comboBox_emuleWSType;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.Label label54;
        private System.Windows.Forms.CheckBox checkDownloadEpisodeSnapshots;
        private System.Windows.Forms.CheckBox chkBlankBanners;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox txtMainMirror;
        private System.Windows.Forms.LinkLabel linkDelUpdateTime;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.ComboBox comboOnlineLang;
        private System.Windows.Forms.CheckBox checkBox_OnlineSearch;
        private System.Windows.Forms.CheckBox checkBox_AutoChooseSeries;
        private System.Windows.Forms.CheckBox checkBox_FullSeriesRetrieval;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.Label label68;
        private System.Windows.Forms.ColumnHeader FileName;
        private System.Windows.Forms.ColumnHeader ParsedSeriesName;
        private System.Windows.Forms.ColumnHeader SeasonID;
        private System.Windows.Forms.ColumnHeader EpisodeID;
        private System.Windows.Forms.ColumnHeader EpisodeTitle;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox comboLogLevel;
        private System.Windows.Forms.LinkLabel linkAccountID;
        private System.Windows.Forms.Label label69;
        private System.Windows.Forms.Label label70;
		private System.Windows.Forms.LinkLabel linkExpressionHelp;
        private System.Windows.Forms.Label label71;
        private System.Windows.Forms.ComboBox cboFanartResolution;
        private System.Windows.Forms.CheckBox chkAutoDownloadFanart;
        private System.Windows.Forms.Label label73;
        private System.Windows.Forms.NumericUpDown spinMaxFanarts;
        private System.Windows.Forms.Label label72;
        private System.Windows.Forms.CheckBox checkBox_ScanOnStartup;
        private System.Windows.Forms.CheckBox checkBox_AutoUpdateAllFanart;
        private System.Windows.Forms.CheckBox checkBox_AutoUpdateEpisodeRatings;
        private System.Windows.Forms.CheckBox checkBox_AutoDownloadMissingArtwork;
        private System.Windows.Forms.LinkLabel linkExParsingExpressions;
		private System.Windows.Forms.LinkLabel linkImpParsingExpressions;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.CheckBox checkbox_SortSpecials;
        private System.Windows.Forms.NumericUpDown numericUpDownBackdropDelay;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label74;
        private System.Windows.Forms.NumericUpDown numericUpDownArtworkDelay;
        private System.Windows.Forms.Label label76;
		private System.Windows.Forms.Label label75;
		private System.Windows.Forms.CheckBox checkboxRatingDisplayStars;
        private System.Windows.Forms.CheckBox checkboxAutoDownloadFanartSeriesName;
        private System.Windows.Forms.Button buttonEditView;
        private System.Windows.Forms.CheckBox checkBoxParentalControl;
		private System.Windows.Forms.Button buttonPinCode;
		private WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox dbOptionCheckBoxSMSKeyboard;
        private System.Windows.Forms.Button buttonViewTemplates;
		private WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox dbOptionCheckBoxMarkRatedEpsAsWatched;
		private WindowPlugins.GUITVSeries.Configuration.PlaylistSettings playlistSettings;
        private System.Windows.Forms.LinkLabel linkLabelExportStringReplacements;
        private System.Windows.Forms.LinkLabel linkLabelImportStringReplacements;
        private System.Windows.Forms.LinkLabel linkLabelResetStringReplacements;
        private System.Windows.Forms.CheckBox checkBox_scanRemoteShares;
        private System.Windows.Forms.NumericUpDown nudScanRemoteShareFrequency;
        private System.Windows.Forms.Label label77;
        private System.Windows.Forms.CheckBox checkBox_SubDownloadOnPlay;
        private System.Windows.Forms.CheckBox subtitleDownloader_enabled;
        private WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox dbOptionCheckBoxSubstituteMissingArtwork;
        private WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox dbOptionCheckBox2;
        private WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox dbOptChkBoxScanFullscreenVideo;
    }
}
