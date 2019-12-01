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

using WindowPlugins.GUITVSeries.Configuration;

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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            this.columnHeader_Series = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_Title = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_Season = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_Episode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_OriginallyAired = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuStrip_DetailsTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ignoreOnScanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reScanMediaInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.watchedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unWatchedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenuStrip_InsertFields = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolTip_Help = new System.Windows.Forms.ToolTip(this.components);
            this.linkMediaInfoUpdate = new System.Windows.Forms.LinkLabel();
            this.checkBox_ShowHidden = new System.Windows.Forms.CheckBox();
            this.textBox_dblocation = new System.Windows.Forms.TextBox();
            this.lblClearDB = new System.Windows.Forms.LinkLabel();
            this.linkExParsingExpressions = new System.Windows.Forms.LinkLabel();
            this.linkImpParsingExpressions = new System.Windows.Forms.LinkLabel();
            this.buildExpr = new System.Windows.Forms.LinkLabel();
            this.resetExpr = new System.Windows.Forms.LinkLabel();
            this.dataGridView_Replace = new System.Windows.Forms.DataGridView();
            this.button_TestReparse = new System.Windows.Forms.Button();
            this.buttonStartImport = new System.Windows.Forms.Button();
            this.checkboxAutoDownloadFanartSeriesName = new System.Windows.Forms.CheckBox();
            this.checkBox_AutoDownloadMissingArtwork = new System.Windows.Forms.CheckBox();
            this.checkBox_AutoUpdateEpisodeRatings = new System.Windows.Forms.CheckBox();
            this.checkBox_ScanOnStartup = new System.Windows.Forms.CheckBox();
            this.checkBox_AutoOnlineDataRefresh = new System.Windows.Forms.CheckBox();
            this.checkBox_AutoChooseOrder = new System.Windows.Forms.CheckBox();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.checkDownloadEpisodeSnapshots = new System.Windows.Forms.CheckBox();
            this.chkBlankBanners = new System.Windows.Forms.CheckBox();
            this.txtMainMirror = new System.Windows.Forms.TextBox();
            this.linkDelUpdateTime = new System.Windows.Forms.LinkLabel();
            this.comboOnlineLang = new System.Windows.Forms.ComboBox();
            this.checkBox_OverrideComboLang = new System.Windows.Forms.CheckBox();
            this.checkBox_OnlineSearch = new System.Windows.Forms.CheckBox();
            this.checkBox_AutoChooseSeries = new System.Windows.Forms.CheckBox();
            this.checkBox_FullSeriesRetrieval = new System.Windows.Forms.CheckBox();
            this.nudRecentlyAddedDays = new System.Windows.Forms.NumericUpDown();
            this.nudScanRemoteShareFrequency = new System.Windows.Forms.NumericUpDown();
            this.checkBox_scanRemoteShares = new System.Windows.Forms.CheckBox();
            this.checkboxRatingDisplayStars = new System.Windows.Forms.CheckBox();
            this.checkbox_SortSpecials = new System.Windows.Forms.CheckBox();
            this.linkExWatched = new System.Windows.Forms.LinkLabel();
            this.linkImpWatched = new System.Windows.Forms.LinkLabel();
            this.chkAllowDeletes = new System.Windows.Forms.CheckBox();
            this.checkBox_doFolderWatch = new System.Windows.Forms.CheckBox();
            this.checkBox_Series_UseSortName = new System.Windows.Forms.CheckBox();
            this.nudWatchedAfter = new System.Windows.Forms.NumericUpDown();
            this.checkBox_RandBanner = new System.Windows.Forms.CheckBox();
            this.textBox_PluginHomeName = new System.Windows.Forms.TextBox();
            this.nudParentalControlTimeout = new System.Windows.Forms.NumericUpDown();
            this.buttonViewTemplates = new System.Windows.Forms.Button();
            this.btnLogoDown = new System.Windows.Forms.Button();
            this.btnlogoUp = new System.Windows.Forms.Button();
            this.lnkLogoImp = new System.Windows.Forms.LinkLabel();
            this.lstLogos = new System.Windows.Forms.ListBox();
            this.btnLogoTemplate = new System.Windows.Forms.Button();
            this.btnrmvLogo = new System.Windows.Forms.Button();
            this.lnkLogoExport = new System.Windows.Forms.LinkLabel();
            this.btnLogoEdit = new System.Windows.Forms.Button();
            this.addLogo = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.toolTip_InfoHelp = new System.Windows.Forms.ToolTip(this.components);
            this.numericUpDownImportDelay = new System.Windows.Forms.NumericUpDown();
            this.checkBox_Episode_HideUnwatchedSummary = new System.Windows.Forms.CheckBox();
            this.checkBox_Episode_HideUnwatchedThumbnail = new System.Windows.Forms.CheckBox();
            this.checkBox_Episode_OnlyShowLocalFiles = new System.Windows.Forms.CheckBox();
            this.chkUseRegionalDateFormatString = new System.Windows.Forms.CheckBox();
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
            this.dbCheckBoxTraktCommunityRatings = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.dbOptCheckBoxCleanOnlineEpisodes = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.dbOptionCheckBox4 = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.dbOptChkBoxPlayOutOfOrderCheck = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.dbOptionSQLLogging = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.dbOptChkBox_SubCentral_DownloadSubtitlesOnPlay = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.checkBox_CountSpecialEpisodesAsWatched = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.dbOptChkBoxCountEmptyFutureEps = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.dbOptionCheckBoxMarkRatedEpsAsWatched = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.optionAsk2Rate = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.dbOptionParsedNameFromFolder = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.splitMain_Log = new System.Windows.Forms.SplitContainer();
            this.tabControl_Details = new System.Windows.Forms.TabControl();
            this.tabPage_Details = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.treeView_Library = new System.Windows.Forms.TreeView();
            this.lnkIMDbSeries = new System.Windows.Forms.LinkLabel();
            this.lnkTraktSeries = new System.Windows.Forms.LinkLabel();
            this.lnkTVDbSeries = new System.Windows.Forms.LinkLabel();
            this.label23 = new System.Windows.Forms.Label();
            this.lnkImageCache = new System.Windows.Forms.LinkLabel();
            this.lnkOpenAPICacheDir = new System.Windows.Forms.LinkLabel();
            this.pictureBox_SeriesPoster = new System.Windows.Forms.PictureBox();
            this.comboBox_PosterSelection = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pictureBox_Series = new System.Windows.Forms.PictureBox();
            this.comboBox_BannerSelection = new System.Windows.Forms.ComboBox();
            this.panDBLocation = new System.Windows.Forms.Panel();
            this.button_dbbrowse = new System.Windows.Forms.Button();
            this.label28 = new System.Windows.Forms.Label();
            this.tabPage_Import = new System.Windows.Forms.TabPage();
            this.splitContainer_SettingsOutput = new System.Windows.Forms.SplitContainer();
            this.splitContainerImportSettings = new System.Windows.Forms.SplitContainer();
            this.treeView_Settings = new System.Windows.Forms.TreeView();
            this.panel_ImportPathes = new System.Windows.Forms.Panel();
            this.label68 = new System.Windows.Forms.Label();
            this.dataGridView_ImportPathes = new System.Windows.Forms.DataGridView();
            this.listView_ParsingResults = new System.Windows.Forms.ListView();
            this.FileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ParsedSeriesName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SeasonID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.EpisodeID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.EpisodeTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
            this.tabOnlineData = new System.Windows.Forms.TabPage();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.panel_OnlineData = new System.Windows.Forms.Panel();
            this.dbOptCheckBoxRemoveEpZero = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.dbOptCheckBoxDownloadActors = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.nudConsecFailures = new System.Windows.Forms.NumericUpDown();
            this.label20 = new System.Windows.Forms.Label();
            this.buttonArtworkDownloadLimits = new System.Windows.Forms.Button();
            this.dbOptCheckBoxGetTextBanners = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.label18 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lblImportDelaySecs = new System.Windows.Forms.Label();
            this.lblImportDelayCaption = new System.Windows.Forms.Label();
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
            this.dbOptionCheckBox3 = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.lblRecentAddedDays = new System.Windows.Forms.Label();
            this.cbNewEpisodeThumbIndicator = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.laOnPlaySeriesOrSeasonAction = new System.Windows.Forms.Label();
            this.cbOnPlaySeriesOrSeasonAction = new System.Windows.Forms.ComboBox();
            this.dbOptChkBoxScanFullscreenVideo = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.dbOptionCheckBox2 = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.dbOptionCheckBoxSubstituteMissingArtwork = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.label77 = new System.Windows.Forms.Label();
            this.label76 = new System.Windows.Forms.Label();
            this.label75 = new System.Windows.Forms.Label();
            this.numericUpDownArtworkDelay = new System.Windows.Forms.NumericUpDown();
            this.label74 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.numericUpDownBackdropDelay = new System.Windows.Forms.NumericUpDown();
            this.label39 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.tab_view = new System.Windows.Forms.TabPage();
            this.playlistSettings = new WindowPlugins.GUITVSeries.Configuration.PlaylistSettings();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.dtpParentalBefore = new System.Windows.Forms.DateTimePicker();
            this.label22 = new System.Windows.Forms.Label();
            this.dtpParentalAfter = new System.Windows.Forms.DateTimePicker();
            this.label21 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
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
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.aboutScreen = new WindowPlugins.GUITVSeries.About();
            this.listBox_Log = new System.Windows.Forms.ListBox();
            this.dbOptionDisableMediaInfoInConfig = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.contextMenuStrip_DetailsTree.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Replace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRecentlyAddedDays)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScanRemoteShareFrequency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWatchedAfter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudParentalControlTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownImportDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain_Log)).BeginInit();
            this.splitMain_Log.Panel1.SuspendLayout();
            this.splitMain_Log.Panel2.SuspendLayout();
            this.splitMain_Log.SuspendLayout();
            this.tabControl_Details.SuspendLayout();
            this.tabPage_Details.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SeriesPoster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Series)).BeginInit();
            this.panDBLocation.SuspendLayout();
            this.tabPage_Import.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_SettingsOutput)).BeginInit();
            this.splitContainer_SettingsOutput.Panel1.SuspendLayout();
            this.splitContainer_SettingsOutput.Panel2.SuspendLayout();
            this.splitContainer_SettingsOutput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerImportSettings)).BeginInit();
            this.splitContainerImportSettings.Panel1.SuspendLayout();
            this.splitContainerImportSettings.Panel2.SuspendLayout();
            this.splitContainerImportSettings.SuspendLayout();
            this.panel_ImportPathes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ImportPathes)).BeginInit();
            this.panel_Expressions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Expressions)).BeginInit();
            this.panel_StringReplacements.SuspendLayout();
            this.tabOnlineData.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.panel_OnlineData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudConsecFailures)).BeginInit();
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
            this.contextMenuStrip_DetailsTree.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip_DetailsTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideToolStripMenuItem,
            this.ignoreOnScanToolStripMenuItem,
            this.toolStripSeparator4,
            this.deleteToolStripMenuItem,
            this.updateToolStripMenuItem,
            this.reScanMediaInfoToolStripMenuItem,
            this.toolStripSeparator2,
            this.watchedToolStripMenuItem,
            this.unWatchedToolStripMenuItem,
            this.toolStripSeparator1});
            this.contextMenuStrip_DetailsTree.Name = "contextMenuStrip_DetailsTree";
            this.contextMenuStrip_DetailsTree.Size = new System.Drawing.Size(177, 176);
            this.contextMenuStrip_DetailsTree.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_DetailsTree_Opening);
            this.contextMenuStrip_DetailsTree.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip_DetailsTree_ItemClicked);
            // 
            // hideToolStripMenuItem
            // 
            this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
            this.hideToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.hideToolStripMenuItem.Tag = "hide";
            this.hideToolStripMenuItem.Text = "Hide";
            // 
            // ignoreOnScanToolStripMenuItem
            // 
            this.ignoreOnScanToolStripMenuItem.CheckOnClick = true;
            this.ignoreOnScanToolStripMenuItem.Name = "ignoreOnScanToolStripMenuItem";
            this.ignoreOnScanToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.ignoreOnScanToolStripMenuItem.Tag = "scanignore";
            this.ignoreOnScanToolStripMenuItem.Text = "Ignore on Scan";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(173, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.deleteToolStripMenuItem.Tag = "delete";
            this.deleteToolStripMenuItem.Text = "Delete...";
            // 
            // updateToolStripMenuItem
            // 
            this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            this.updateToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.updateToolStripMenuItem.Tag = "update";
            this.updateToolStripMenuItem.Text = "Update";
            // 
            // reScanMediaInfoToolStripMenuItem
            // 
            this.reScanMediaInfoToolStripMenuItem.Name = "reScanMediaInfoToolStripMenuItem";
            this.reScanMediaInfoToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.reScanMediaInfoToolStripMenuItem.Tag = "mediainfo";
            this.reScanMediaInfoToolStripMenuItem.Text = "Re-Scan MediaInfo";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(173, 6);
            // 
            // watchedToolStripMenuItem
            // 
            this.watchedToolStripMenuItem.Name = "watchedToolStripMenuItem";
            this.watchedToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.watchedToolStripMenuItem.Tag = "watched";
            this.watchedToolStripMenuItem.Text = "Set As Watched";
            // 
            // unWatchedToolStripMenuItem
            // 
            this.unWatchedToolStripMenuItem.Name = "unWatchedToolStripMenuItem";
            this.unWatchedToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.unWatchedToolStripMenuItem.Tag = "unwatched";
            this.unWatchedToolStripMenuItem.Text = "Set As Un-Watched";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(173, 6);
            // 
            // contextMenuStrip_InsertFields
            // 
            this.contextMenuStrip_InsertFields.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip_InsertFields.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.contextMenuStrip_InsertFields.Name = "contextMenuStrip_SeriesFields";
            this.contextMenuStrip_InsertFields.Size = new System.Drawing.Size(61, 4);
            this.contextMenuStrip_InsertFields.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_SeriesFields_Opening);
            this.contextMenuStrip_InsertFields.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip_SeriesFields_ItemClicked);
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
            this.imageList1.Images.SetKeyName(11, "trakt.png");
            // 
            // linkMediaInfoUpdate
            // 
            this.linkMediaInfoUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkMediaInfoUpdate.AutoSize = true;
            this.linkMediaInfoUpdate.Location = new System.Drawing.Point(127, 911);
            this.linkMediaInfoUpdate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkMediaInfoUpdate.Name = "linkMediaInfoUpdate";
            this.linkMediaInfoUpdate.Size = new System.Drawing.Size(95, 13);
            this.linkMediaInfoUpdate.TabIndex = 29;
            this.linkMediaInfoUpdate.TabStop = true;
            this.linkMediaInfoUpdate.Text = "Update Media Info";
            this.toolTip_Help.SetToolTip(this.linkMediaInfoUpdate, "Click here to force the plug-in to read the media information for local files aga" +
        "in e.g. resolution, codecs...");
            this.linkMediaInfoUpdate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkMediaInfoUpdate_LinkClicked);
            // 
            // checkBox_ShowHidden
            // 
            this.checkBox_ShowHidden.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_ShowHidden.AutoSize = true;
            this.checkBox_ShowHidden.Location = new System.Drawing.Point(9, 910);
            this.checkBox_ShowHidden.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox_ShowHidden.Name = "checkBox_ShowHidden";
            this.checkBox_ShowHidden.Size = new System.Drawing.Size(90, 17);
            this.checkBox_ShowHidden.TabIndex = 1;
            this.checkBox_ShowHidden.Tag = "z";
            this.checkBox_ShowHidden.Text = "&Show Hidden";
            this.toolTip_Help.SetToolTip(this.checkBox_ShowHidden, "Shows the Series,Seasons and Episodes that have been hidden from view");
            this.checkBox_ShowHidden.UseVisualStyleBackColor = true;
            this.checkBox_ShowHidden.CheckedChanged += new System.EventHandler(this.checkBox_ShowHidden_CheckedChanged);
            // 
            // textBox_dblocation
            // 
            this.textBox_dblocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_dblocation.Location = new System.Drawing.Point(185, 5);
            this.textBox_dblocation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox_dblocation.Name = "textBox_dblocation";
            this.textBox_dblocation.ReadOnly = true;
            this.textBox_dblocation.Size = new System.Drawing.Size(682, 20);
            this.textBox_dblocation.TabIndex = 1;
            this.toolTip_Help.SetToolTip(this.textBox_dblocation, "The Database Path used by MP-TVSeries to store all TV information and settings fo" +
        "r this plugin");
            // 
            // lblClearDB
            // 
            this.lblClearDB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblClearDB.AutoSize = true;
            this.lblClearDB.Location = new System.Drawing.Point(912, 8);
            this.lblClearDB.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblClearDB.Name = "lblClearDB";
            this.lblClearDB.Size = new System.Drawing.Size(36, 13);
            this.lblClearDB.TabIndex = 3;
            this.lblClearDB.TabStop = true;
            this.lblClearDB.Text = "Empty";
            this.toolTip_Help.SetToolTip(this.lblClearDB, "Deletes all Series, Season and Episode information from the local database");
            this.lblClearDB.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblClearDB_LinkClicked);
            // 
            // linkExParsingExpressions
            // 
            this.linkExParsingExpressions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkExParsingExpressions.AutoSize = true;
            this.linkExParsingExpressions.Location = new System.Drawing.Point(849, 178);
            this.linkExParsingExpressions.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkExParsingExpressions.Name = "linkExParsingExpressions";
            this.linkExParsingExpressions.Size = new System.Drawing.Size(37, 13);
            this.linkExParsingExpressions.TabIndex = 8;
            this.linkExParsingExpressions.TabStop = true;
            this.linkExParsingExpressions.Text = "Export";
            this.toolTip_Help.SetToolTip(this.linkExParsingExpressions, "Export Parsing Expressions to a file");
            this.linkExParsingExpressions.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkExParsingExpressions_LinkClicked);
            // 
            // linkImpParsingExpressions
            // 
            this.linkImpParsingExpressions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkImpParsingExpressions.AutoSize = true;
            this.linkImpParsingExpressions.Location = new System.Drawing.Point(849, 155);
            this.linkImpParsingExpressions.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkImpParsingExpressions.Name = "linkImpParsingExpressions";
            this.linkImpParsingExpressions.Size = new System.Drawing.Size(36, 13);
            this.linkImpParsingExpressions.TabIndex = 7;
            this.linkImpParsingExpressions.TabStop = true;
            this.linkImpParsingExpressions.Text = "Import";
            this.toolTip_Help.SetToolTip(this.linkImpParsingExpressions, "Import Parsing Expressions from a file");
            this.linkImpParsingExpressions.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkImpParsingExpressions_LinkClicked);
            // 
            // buildExpr
            // 
            this.buildExpr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buildExpr.AutoSize = true;
            this.buildExpr.Location = new System.Drawing.Point(849, 247);
            this.buildExpr.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
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
            this.resetExpr.Location = new System.Drawing.Point(849, 201);
            this.resetExpr.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
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
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_Replace.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView_Replace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_Replace.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView_Replace.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView_Replace.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Replace.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView_Replace.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Replace.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView_Replace.Location = new System.Drawing.Point(9, 65);
            this.dataGridView_Replace.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dataGridView_Replace.MultiSelect = false;
            this.dataGridView_Replace.Name = "dataGridView_Replace";
            this.dataGridView_Replace.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Replace.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView_Replace.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_Replace.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridView_Replace.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_Replace.RowTemplate.Height = 18;
            this.dataGridView_Replace.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Replace.Size = new System.Drawing.Size(938, 164);
            this.dataGridView_Replace.StandardTab = true;
            this.dataGridView_Replace.TabIndex = 0;
            this.toolTip_Help.SetToolTip(this.dataGridView_Replace, "Strings that are to be replaced before or after parsing data to online database l" +
        "ookup");
            this.dataGridView_Replace.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_Replace_CellEndEdit);
            this.dataGridView_Replace.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView_Replace_DefaultValuesNeeded);
            this.dataGridView_Replace.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView_Replace_UserDeletedRow);
            // 
            // button_TestReparse
            // 
            this.button_TestReparse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_TestReparse.Image = ((System.Drawing.Image)(resources.GetObject("button_TestReparse.Image")));
            this.button_TestReparse.Location = new System.Drawing.Point(881, 236);
            this.button_TestReparse.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button_TestReparse.Name = "button_TestReparse";
            this.button_TestReparse.Size = new System.Drawing.Size(30, 32);
            this.button_TestReparse.TabIndex = 1;
            this.toolTip_Help.SetToolTip(this.button_TestReparse, "Redo a local parsing test");
            this.button_TestReparse.UseVisualStyleBackColor = true;
            this.button_TestReparse.Click += new System.EventHandler(this.button_TestReparse_Click);
            // 
            // buttonStartImport
            // 
            this.buttonStartImport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStartImport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStartImport.Location = new System.Drawing.Point(4, 5);
            this.buttonStartImport.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonStartImport.Name = "buttonStartImport";
            this.buttonStartImport.Size = new System.Drawing.Size(1185, 35);
            this.buttonStartImport.TabIndex = 4;
            this.buttonStartImport.Text = "&Start Import Wizard";
            this.toolTip_Help.SetToolTip(this.buttonStartImport, "Click to Start Importing data from the Online Database, updates data for all seri" +
        "es, seasons and episodes new and old");
            this.buttonStartImport.UseVisualStyleBackColor = true;
            this.buttonStartImport.Click += new System.EventHandler(this.buttonStartImport_Click);
            // 
            // checkboxAutoDownloadFanartSeriesName
            // 
            this.checkboxAutoDownloadFanartSeriesName.AutoSize = true;
            this.checkboxAutoDownloadFanartSeriesName.Location = new System.Drawing.Point(448, 501);
            this.checkboxAutoDownloadFanartSeriesName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkboxAutoDownloadFanartSeriesName.Name = "checkboxAutoDownloadFanartSeriesName";
            this.checkboxAutoDownloadFanartSeriesName.Size = new System.Drawing.Size(265, 17);
            this.checkboxAutoDownloadFanartSeriesName.TabIndex = 42;
            this.checkboxAutoDownloadFanartSeriesName.Text = "Download Fanart containing Series Names (Logos)";
            this.toolTip_Help.SetToolTip(this.checkboxAutoDownloadFanartSeriesName, "Enable to automatically download Fanart that contain series names in artwork, use" +
        "rs can\r\nstill choose Fanart that contain series names from the Fanart Chooser wi" +
        "ndow.");
            this.checkboxAutoDownloadFanartSeriesName.UseVisualStyleBackColor = true;
            this.checkboxAutoDownloadFanartSeriesName.CheckedChanged += new System.EventHandler(this.checkboxAutoDownloadFanartSeriesName_CheckedChanged);
            // 
            // checkBox_AutoDownloadMissingArtwork
            // 
            this.checkBox_AutoDownloadMissingArtwork.AutoSize = true;
            this.checkBox_AutoDownloadMissingArtwork.Location = new System.Drawing.Point(12, 474);
            this.checkBox_AutoDownloadMissingArtwork.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox_AutoDownloadMissingArtwork.Name = "checkBox_AutoDownloadMissingArtwork";
            this.checkBox_AutoDownloadMissingArtwork.Size = new System.Drawing.Size(337, 17);
            this.checkBox_AutoDownloadMissingArtwork.TabIndex = 29;
            this.checkBox_AutoDownloadMissingArtwork.Text = "Download missing artwork for existing series/season on local scan";
            this.toolTip_Help.SetToolTip(this.checkBox_AutoDownloadMissingArtwork, "Only update new artwork when unchecked.  This can help speed up import for large " +
        "collections.");
            this.checkBox_AutoDownloadMissingArtwork.UseVisualStyleBackColor = true;
            this.checkBox_AutoDownloadMissingArtwork.CheckedChanged += new System.EventHandler(this.checkBox_AutoDownloadMissingArtwork_CheckedChanged);
            // 
            // checkBox_AutoUpdateEpisodeRatings
            // 
            this.checkBox_AutoUpdateEpisodeRatings.AutoSize = true;
            this.checkBox_AutoUpdateEpisodeRatings.Location = new System.Drawing.Point(12, 265);
            this.checkBox_AutoUpdateEpisodeRatings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox_AutoUpdateEpisodeRatings.Name = "checkBox_AutoUpdateEpisodeRatings";
            this.checkBox_AutoUpdateEpisodeRatings.Size = new System.Drawing.Size(440, 17);
            this.checkBox_AutoUpdateEpisodeRatings.TabIndex = 20;
            this.checkBox_AutoUpdateEpisodeRatings.Text = "Automatically update user episode ratings from theTVDb.com in addition to series " +
    "ratings";
            this.toolTip_Help.SetToolTip(this.checkBox_AutoUpdateEpisodeRatings, resources.GetString("checkBox_AutoUpdateEpisodeRatings.ToolTip"));
            this.checkBox_AutoUpdateEpisodeRatings.UseVisualStyleBackColor = true;
            this.checkBox_AutoUpdateEpisodeRatings.CheckedChanged += new System.EventHandler(this.checkBox_AutoUpdateEpisodeRatings_CheckedChanged);
            // 
            // checkBox_ScanOnStartup
            // 
            this.checkBox_ScanOnStartup.AutoSize = true;
            this.checkBox_ScanOnStartup.Location = new System.Drawing.Point(12, 107);
            this.checkBox_ScanOnStartup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox_ScanOnStartup.Name = "checkBox_ScanOnStartup";
            this.checkBox_ScanOnStartup.Size = new System.Drawing.Size(268, 17);
            this.checkBox_ScanOnStartup.TabIndex = 9;
            this.checkBox_ScanOnStartup.Text = "E&nable local/online scan when starting MediaPortal";
            this.toolTip_Help.SetToolTip(this.checkBox_ScanOnStartup, resources.GetString("checkBox_ScanOnStartup.ToolTip"));
            this.checkBox_ScanOnStartup.UseVisualStyleBackColor = true;
            this.checkBox_ScanOnStartup.CheckedChanged += new System.EventHandler(this.checkBox_ScanOnStartup_CheckedChanged);
            // 
            // checkBox_AutoOnlineDataRefresh
            // 
            this.checkBox_AutoOnlineDataRefresh.AutoSize = true;
            this.checkBox_AutoOnlineDataRefresh.Location = new System.Drawing.Point(12, 157);
            this.checkBox_AutoOnlineDataRefresh.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox_AutoOnlineDataRefresh.Name = "checkBox_AutoOnlineDataRefresh";
            this.checkBox_AutoOnlineDataRefresh.Size = new System.Drawing.Size(328, 17);
            this.checkBox_AutoOnlineDataRefresh.TabIndex = 13;
            this.checkBox_AutoOnlineDataRefresh.Text = "A&utomatically download updated data from theTVDB.com every:";
            this.toolTip_Help.SetToolTip(this.checkBox_AutoOnlineDataRefresh, "Enable this option to make the plug-in automatically ask for a refresh of the onl" +
        "ine data every x hours");
            this.checkBox_AutoOnlineDataRefresh.UseVisualStyleBackColor = true;
            this.checkBox_AutoOnlineDataRefresh.CheckedChanged += new System.EventHandler(this.checkBox_AutoOnlineDataRefresh_CheckedChanged);
            // 
            // checkBox_AutoChooseOrder
            // 
            this.checkBox_AutoChooseOrder.AutoSize = true;
            this.checkBox_AutoChooseOrder.Location = new System.Drawing.Point(12, 211);
            this.checkBox_AutoChooseOrder.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox_AutoChooseOrder.Name = "checkBox_AutoChooseOrder";
            this.checkBox_AutoChooseOrder.Size = new System.Drawing.Size(331, 17);
            this.checkBox_AutoChooseOrder.TabIndex = 18;
            this.checkBox_AutoChooseOrder.Text = "Automatically choose Aired when multiple &orders are found online";
            this.toolTip_Help.SetToolTip(this.checkBox_AutoChooseOrder, "Enable this option to automatically select Aired order when multiple orders are f" +
        "ound online e.g. DVD or Absolute");
            this.checkBox_AutoChooseOrder.UseVisualStyleBackColor = true;
            this.checkBox_AutoChooseOrder.CheckedChanged += new System.EventHandler(this.checkBox_AutoChooseOrder_CheckedChanged);
            // 
            // txtUserID
            // 
            this.txtUserID.Location = new System.Drawing.Point(127, 53);
            this.txtUserID.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.Size = new System.Drawing.Size(180, 20);
            this.txtUserID.TabIndex = 4;
            this.toolTip_Help.SetToolTip(this.txtUserID, resources.GetString("txtUserID.ToolTip"));
            this.txtUserID.TextChanged += new System.EventHandler(this.txtUserID_TextChanged);
            // 
            // checkDownloadEpisodeSnapshots
            // 
            this.checkDownloadEpisodeSnapshots.AutoSize = true;
            this.checkDownloadEpisodeSnapshots.Location = new System.Drawing.Point(12, 395);
            this.checkDownloadEpisodeSnapshots.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkDownloadEpisodeSnapshots.Name = "checkDownloadEpisodeSnapshots";
            this.checkDownloadEpisodeSnapshots.Size = new System.Drawing.Size(172, 17);
            this.checkDownloadEpisodeSnapshots.TabIndex = 26;
            this.checkDownloadEpisodeSnapshots.Text = "Download Episode &Thumbnails";
            this.toolTip_Help.SetToolTip(this.checkDownloadEpisodeSnapshots, "Enable to download Episode Thumbnails if available.");
            this.checkDownloadEpisodeSnapshots.UseVisualStyleBackColor = true;
            this.checkDownloadEpisodeSnapshots.CheckedChanged += new System.EventHandler(this.checkDownloadEpisodeSnapshots_CheckedChanged);
            // 
            // chkBlankBanners
            // 
            this.chkBlankBanners.AutoSize = true;
            this.chkBlankBanners.Location = new System.Drawing.Point(12, 422);
            this.chkBlankBanners.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkBlankBanners.Name = "chkBlankBanners";
            this.chkBlankBanners.Size = new System.Drawing.Size(389, 17);
            this.chkBlankBanners.TabIndex = 27;
            this.chkBlankBanners.Text = "Download Series WideBanners that contain no graphical or text series names";
            this.toolTip_Help.SetToolTip(this.chkBlankBanners, "Enable to download blank series widebanners as well as localized banners.");
            this.chkBlankBanners.UseVisualStyleBackColor = true;
            this.chkBlankBanners.CheckedChanged += new System.EventHandler(this.chkBlankBanners_CheckedChanged);
            // 
            // txtMainMirror
            // 
            this.txtMainMirror.Location = new System.Drawing.Point(127, 28);
            this.txtMainMirror.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtMainMirror.Name = "txtMainMirror";
            this.txtMainMirror.Size = new System.Drawing.Size(180, 20);
            this.txtMainMirror.TabIndex = 2;
            this.toolTip_Help.SetToolTip(this.txtMainMirror, "Download mirror for online database");
            this.txtMainMirror.TextChanged += new System.EventHandler(this.txtMainMirror_TextChanged);
            // 
            // linkDelUpdateTime
            // 
            this.linkDelUpdateTime.AutoSize = true;
            this.linkDelUpdateTime.Location = new System.Drawing.Point(489, 157);
            this.linkDelUpdateTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkDelUpdateTime.Name = "linkDelUpdateTime";
            this.linkDelUpdateTime.Size = new System.Drawing.Size(123, 13);
            this.linkDelUpdateTime.TabIndex = 16;
            this.linkDelUpdateTime.TabStop = true;
            this.linkDelUpdateTime.Text = "Clear Update Timestamp";
            this.toolTip_Help.SetToolTip(this.linkDelUpdateTime, "Click here to reset Timestamps for the last date-time data for series was updated" +
        " from the online database.\r\nThis will force a re-request of all data on next imp" +
        "ort");
            this.linkDelUpdateTime.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDelUpdateTime_LinkClicked);
            // 
            // comboOnlineLang
            // 
            this.comboOnlineLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboOnlineLang.FormattingEnabled = true;
            this.comboOnlineLang.Location = new System.Drawing.Point(127, 78);
            this.comboOnlineLang.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboOnlineLang.Name = "comboOnlineLang";
            this.comboOnlineLang.Size = new System.Drawing.Size(180, 21);
            this.comboOnlineLang.TabIndex = 6;
            this.toolTip_Help.SetToolTip(this.comboOnlineLang, "Select the language to download TV Series information in, defaults to English");
            this.comboOnlineLang.SelectedIndexChanged += new System.EventHandler(this.comboOnlineLang_SelectedIndexChanged);
            // 
            // checkBox_OverrideComboLang
            // 
            this.checkBox_OverrideComboLang.AutoSize = true;
            this.checkBox_OverrideComboLang.Location = new System.Drawing.Point(322, 80);
            this.checkBox_OverrideComboLang.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox_OverrideComboLang.Name = "checkBox_OverrideComboLang";
            this.checkBox_OverrideComboLang.Size = new System.Drawing.Size(164, 17);
            this.checkBox_OverrideComboLang.TabIndex = 8;
            this.checkBox_OverrideComboLang.Text = "Override Language for Series";
            this.toolTip_Help.SetToolTip(this.checkBox_OverrideComboLang, "Enable this option to change the Meta language for specific series..\rLanguage can" +
        " be manually entered or corrected in Details tab");
            this.checkBox_OverrideComboLang.UseVisualStyleBackColor = true;
            this.checkBox_OverrideComboLang.CheckedChanged += new System.EventHandler(this.checkBox_OverrideComboLang_CheckedChanged);
            // 
            // checkBox_OnlineSearch
            // 
            this.checkBox_OnlineSearch.AutoSize = true;
            this.checkBox_OnlineSearch.Location = new System.Drawing.Point(12, 7);
            this.checkBox_OnlineSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox_OnlineSearch.Name = "checkBox_OnlineSearch";
            this.checkBox_OnlineSearch.Size = new System.Drawing.Size(320, 17);
            this.checkBox_OnlineSearch.TabIndex = 0;
            this.checkBox_OnlineSearch.Text = "&Download data from the TV Online Database ( theTVDB.com )";
            this.toolTip_Help.SetToolTip(this.checkBox_OnlineSearch, "Enable this option to download data for TV Series including Banners and Episode t" +
        "humbnails.\r\nData can be manually entered or corrected in Details tab");
            this.checkBox_OnlineSearch.UseVisualStyleBackColor = true;
            this.checkBox_OnlineSearch.CheckedChanged += new System.EventHandler(this.checkBox_OnlineSearch_CheckedChanged);
            // 
            // checkBox_AutoChooseSeries
            // 
            this.checkBox_AutoChooseSeries.AutoSize = true;
            this.checkBox_AutoChooseSeries.Location = new System.Drawing.Point(12, 184);
            this.checkBox_AutoChooseSeries.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox_AutoChooseSeries.Name = "checkBox_AutoChooseSeries";
            this.checkBox_AutoChooseSeries.Size = new System.Drawing.Size(334, 17);
            this.checkBox_AutoChooseSeries.TabIndex = 17;
            this.checkBox_AutoChooseSeries.Text = "Automatically &choose Series when an exact match is found online";
            this.toolTip_Help.SetToolTip(this.checkBox_AutoChooseSeries, resources.GetString("checkBox_AutoChooseSeries.ToolTip"));
            this.checkBox_AutoChooseSeries.UseVisualStyleBackColor = true;
            this.checkBox_AutoChooseSeries.CheckedChanged += new System.EventHandler(this.checkBox_AutoChooseSeries_CheckedChanged);
            // 
            // checkBox_FullSeriesRetrieval
            // 
            this.checkBox_FullSeriesRetrieval.AutoSize = true;
            this.checkBox_FullSeriesRetrieval.Location = new System.Drawing.Point(12, 292);
            this.checkBox_FullSeriesRetrieval.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox_FullSeriesRetrieval.Name = "checkBox_FullSeriesRetrieval";
            this.checkBox_FullSeriesRetrieval.Size = new System.Drawing.Size(263, 17);
            this.checkBox_FullSeriesRetrieval.TabIndex = 21;
            this.checkBox_FullSeriesRetrieval.Text = "Download &Episode information for the whole series";
            this.toolTip_Help.SetToolTip(this.checkBox_FullSeriesRetrieval, "Enable this option to download data for all seasons and episodes available online" +
        " (takes longer).\r\nDisable to download data only for episodes local to this compu" +
        "ter");
            this.checkBox_FullSeriesRetrieval.UseVisualStyleBackColor = true;
            this.checkBox_FullSeriesRetrieval.CheckedChanged += new System.EventHandler(this.checkBox_FullSeriesRetrieval_CheckedChanged);
            // 
            // nudRecentlyAddedDays
            // 
            this.nudRecentlyAddedDays.Location = new System.Drawing.Point(277, 437);
            this.nudRecentlyAddedDays.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.nudRecentlyAddedDays.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRecentlyAddedDays.Name = "nudRecentlyAddedDays";
            this.nudRecentlyAddedDays.Size = new System.Drawing.Size(64, 20);
            this.nudRecentlyAddedDays.TabIndex = 26;
            this.toolTip_Help.SetToolTip(this.nudRecentlyAddedDays, "Select the number of days to look back in database for new episodes when displayi" +
        "ng the *NEW* stamp on series thumbs.");
            this.nudRecentlyAddedDays.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.nudRecentlyAddedDays.ValueChanged += new System.EventHandler(this.nudRecentlyAddedDays_ValueChanged);
            // 
            // nudScanRemoteShareFrequency
            // 
            this.nudScanRemoteShareFrequency.Location = new System.Drawing.Point(205, 312);
            this.nudScanRemoteShareFrequency.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.nudScanRemoteShareFrequency.Size = new System.Drawing.Size(66, 20);
            this.nudScanRemoteShareFrequency.TabIndex = 19;
            this.toolTip_Help.SetToolTip(this.nudScanRemoteShareFrequency, "Enter the percentage of the episode that has been viewed to consider it as watche" +
        "d.\r\nEpisode foreground color will change to indicated that it is watched");
            this.nudScanRemoteShareFrequency.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudScanRemoteShareFrequency.ValueChanged += new System.EventHandler(this.nudScanRemoteShareFrequency_ValueChanged);
            // 
            // checkBox_scanRemoteShares
            // 
            this.checkBox_scanRemoteShares.AutoSize = true;
            this.checkBox_scanRemoteShares.Location = new System.Drawing.Point(40, 315);
            this.checkBox_scanRemoteShares.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox_scanRemoteShares.Name = "checkBox_scanRemoteShares";
            this.checkBox_scanRemoteShares.Size = new System.Drawing.Size(149, 17);
            this.checkBox_scanRemoteShares.TabIndex = 18;
            this.checkBox_scanRemoteShares.Text = "Scan remote shares every";
            this.toolTip_Help.SetToolTip(this.checkBox_scanRemoteShares, "(Recommended) Instead of trying to setup a filesystem  watcher on a remote share " +
        "(hazardous), scan periodically");
            this.checkBox_scanRemoteShares.UseVisualStyleBackColor = true;
            this.checkBox_scanRemoteShares.CheckedChanged += new System.EventHandler(this.checkBox_scanRemoteShares_CheckedChanged);
            // 
            // checkboxRatingDisplayStars
            // 
            this.checkboxRatingDisplayStars.AutoSize = true;
            this.checkboxRatingDisplayStars.Location = new System.Drawing.Point(8, 153);
            this.checkboxRatingDisplayStars.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkboxRatingDisplayStars.Name = "checkboxRatingDisplayStars";
            this.checkboxRatingDisplayStars.Size = new System.Drawing.Size(143, 17);
            this.checkboxRatingDisplayStars.TabIndex = 12;
            this.checkboxRatingDisplayStars.Text = "Show 5 Star Rate Dialog";
            this.toolTip_Help.SetToolTip(this.checkboxRatingDisplayStars, "Enable this option to dislay a 5 Star Rate Dialog instead of 10 Stars, theTVDB.co" +
        "m / trakt.tv accepts 10 Stars\r\nso 5 Stars will be rounded up.");
            this.checkboxRatingDisplayStars.UseVisualStyleBackColor = true;
            this.checkboxRatingDisplayStars.CheckedChanged += new System.EventHandler(this.checkboxRatingDisplayStars_CheckedChanged);
            // 
            // checkbox_SortSpecials
            // 
            this.checkbox_SortSpecials.AutoSize = true;
            this.checkbox_SortSpecials.Location = new System.Drawing.Point(467, 90);
            this.checkbox_SortSpecials.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkbox_SortSpecials.Name = "checkbox_SortSpecials";
            this.checkbox_SortSpecials.Size = new System.Drawing.Size(239, 17);
            this.checkbox_SortSpecials.TabIndex = 31;
            this.checkbox_SortSpecials.Text = "Sort Specials in Episode list (Aired Order only)";
            this.toolTip_Help.SetToolTip(this.checkbox_SortSpecials, "Enable this option to sort specials in the main episode list by their correspondi" +
        "ng air date.\r\nNote: This will only work if the episode list is sorted by \'Aired " +
        "Order\'.");
            this.checkbox_SortSpecials.UseVisualStyleBackColor = true;
            this.checkbox_SortSpecials.CheckedChanged += new System.EventHandler(this.checkbox_SortSpecials_CheckedChanged);
            // 
            // linkExWatched
            // 
            this.linkExWatched.AutoSize = true;
            this.linkExWatched.Location = new System.Drawing.Point(487, 67);
            this.linkExWatched.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkExWatched.Name = "linkExWatched";
            this.linkExWatched.Size = new System.Drawing.Size(121, 13);
            this.linkExWatched.TabIndex = 30;
            this.linkExWatched.TabStop = true;
            this.linkExWatched.Text = "Export Watched Flags...";
            this.toolTip_Help.SetToolTip(this.linkExWatched, "Export the \'Watched\' Status of all episodes to file.\r\n\r\nNote: If you have the Tra" +
        "kt plugin installed you can automatically keep your watched history synced onlin" +
        "e.");
            this.linkExWatched.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkExWatched_LinkClicked);
            // 
            // linkImpWatched
            // 
            this.linkImpWatched.AutoSize = true;
            this.linkImpWatched.Location = new System.Drawing.Point(487, 44);
            this.linkImpWatched.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkImpWatched.Name = "linkImpWatched";
            this.linkImpWatched.Size = new System.Drawing.Size(120, 13);
            this.linkImpWatched.TabIndex = 29;
            this.linkImpWatched.TabStop = true;
            this.linkImpWatched.Text = "Import Watched Flags...";
            this.toolTip_Help.SetToolTip(this.linkImpWatched, "Import the \'Watched\' Status of all episodes from file.\r\n\r\nNote: If you have the T" +
        "rakt plugin installed you can automatically keep your watched history synced onl" +
        "ine.");
            this.linkImpWatched.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkImpWatched_LinkClicked);
            // 
            // chkAllowDeletes
            // 
            this.chkAllowDeletes.AutoSize = true;
            this.chkAllowDeletes.Location = new System.Drawing.Point(467, 252);
            this.chkAllowDeletes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkAllowDeletes.Name = "chkAllowDeletes";
            this.chkAllowDeletes.Size = new System.Drawing.Size(269, 17);
            this.chkAllowDeletes.TabIndex = 37;
            this.chkAllowDeletes.Text = "Allow user to delete &files from the GUI context menu";
            this.toolTip_Help.SetToolTip(this.chkAllowDeletes, "Enable this option to allow users to delete items using the context menu from wit" +
        "h-in Media Portal");
            this.chkAllowDeletes.UseVisualStyleBackColor = true;
            this.chkAllowDeletes.CheckedChanged += new System.EventHandler(this.chkAllowDeletes_CheckedChanged);
            // 
            // checkBox_doFolderWatch
            // 
            this.checkBox_doFolderWatch.AutoSize = true;
            this.checkBox_doFolderWatch.Location = new System.Drawing.Point(8, 288);
            this.checkBox_doFolderWatch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox_doFolderWatch.Name = "checkBox_doFolderWatch";
            this.checkBox_doFolderWatch.Size = new System.Drawing.Size(263, 17);
            this.checkBox_doFolderWatch.TabIndex = 17;
            this.checkBox_doFolderWatch.Text = "Watch my &Import folders for changes automatically";
            this.toolTip_Help.SetToolTip(this.checkBox_doFolderWatch, "Enable this option to allow the plug-in to monitor import folders for new, delete" +
        "d and modified TV Series and update the database accordingly.\r\nThis option only " +
        "works from with-in Media Portal");
            this.checkBox_doFolderWatch.UseVisualStyleBackColor = true;
            this.checkBox_doFolderWatch.CheckedChanged += new System.EventHandler(this.checkBox_doFolderWatch_CheckedChanged);
            // 
            // checkBox_Series_UseSortName
            // 
            this.checkBox_Series_UseSortName.AutoSize = true;
            this.checkBox_Series_UseSortName.Location = new System.Drawing.Point(8, 261);
            this.checkBox_Series_UseSortName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.nudWatchedAfter.Location = new System.Drawing.Point(171, 42);
            this.nudWatchedAfter.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.nudWatchedAfter.Name = "nudWatchedAfter";
            this.nudWatchedAfter.Size = new System.Drawing.Size(66, 20);
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
            // checkBox_RandBanner
            // 
            this.checkBox_RandBanner.AutoSize = true;
            this.checkBox_RandBanner.Location = new System.Drawing.Point(467, 144);
            this.checkBox_RandBanner.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox_RandBanner.Name = "checkBox_RandBanner";
            this.checkBox_RandBanner.Size = new System.Drawing.Size(261, 17);
            this.checkBox_RandBanner.TabIndex = 33;
            this.checkBox_RandBanner.Text = "&Display random Artwork in series and season view";
            this.toolTip_Help.SetToolTip(this.checkBox_RandBanner, "Enable this option to display a random banner when entering series/season view");
            this.checkBox_RandBanner.UseVisualStyleBackColor = true;
            this.checkBox_RandBanner.CheckedChanged += new System.EventHandler(this.checkBox_RandBanner_CheckedChanged);
            // 
            // textBox_PluginHomeName
            // 
            this.textBox_PluginHomeName.Location = new System.Drawing.Point(171, 17);
            this.textBox_PluginHomeName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox_PluginHomeName.Name = "textBox_PluginHomeName";
            this.textBox_PluginHomeName.Size = new System.Drawing.Size(157, 20);
            this.textBox_PluginHomeName.TabIndex = 1;
            this.toolTip_Help.SetToolTip(this.textBox_PluginHomeName, "Enter the name for the plug-in as listed in the Media Portal Home screen");
            this.textBox_PluginHomeName.TextChanged += new System.EventHandler(this.textBox_PluginHomeName_TextChanged);
            // 
            // nudParentalControlTimeout
            // 
            this.nudParentalControlTimeout.Location = new System.Drawing.Point(559, 282);
            this.nudParentalControlTimeout.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.nudParentalControlTimeout.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.nudParentalControlTimeout.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudParentalControlTimeout.Name = "nudParentalControlTimeout";
            this.nudParentalControlTimeout.Size = new System.Drawing.Size(104, 20);
            this.nudParentalControlTimeout.TabIndex = 11;
            this.toolTip_Help.SetToolTip(this.nudParentalControlTimeout, "Set number of minutes for Parental Control Lock to re-enable.\r\nNext time you ente" +
        "r the view, Pin Code will be requested.");
            this.nudParentalControlTimeout.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudParentalControlTimeout.ValueChanged += new System.EventHandler(this.nudParentalControlTimeout_ValueChanged);
            // 
            // buttonViewTemplates
            // 
            this.buttonViewTemplates.Location = new System.Drawing.Point(8, 310);
            this.buttonViewTemplates.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonViewTemplates.Name = "buttonViewTemplates";
            this.buttonViewTemplates.Size = new System.Drawing.Size(112, 21);
            this.buttonViewTemplates.TabIndex = 9;
            this.buttonViewTemplates.Text = "&Templates...";
            this.toolTip_Help.SetToolTip(this.buttonViewTemplates, "Click to select a pre-defined View Filter from a list.");
            this.buttonViewTemplates.UseVisualStyleBackColor = true;
            this.buttonViewTemplates.Click += new System.EventHandler(this.buttonViewTemplates_Click);
            // 
            // btnLogoDown
            // 
            this.btnLogoDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogoDown.Image = global::WindowPlugins.GUITVSeries.Properties.Resources.arrow_down;
            this.btnLogoDown.Location = new System.Drawing.Point(1126, 97);
            this.btnLogoDown.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnLogoDown.Name = "btnLogoDown";
            this.btnLogoDown.Size = new System.Drawing.Size(35, 31);
            this.btnLogoDown.TabIndex = 2;
            this.toolTip_Help.SetToolTip(this.btnLogoDown, "Moves the logo position to the right when displayed in Media Portal");
            this.btnLogoDown.UseVisualStyleBackColor = true;
            this.btnLogoDown.Click += new System.EventHandler(this.btnLogoDown_Click);
            // 
            // btnlogoUp
            // 
            this.btnlogoUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnlogoUp.Image = global::WindowPlugins.GUITVSeries.Properties.Resources.arrow_up;
            this.btnlogoUp.Location = new System.Drawing.Point(1126, 56);
            this.btnlogoUp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnlogoUp.Name = "btnlogoUp";
            this.btnlogoUp.Size = new System.Drawing.Size(35, 31);
            this.btnlogoUp.TabIndex = 1;
            this.toolTip_Help.SetToolTip(this.btnlogoUp, "Moves the logo position to the left when displayed in Media Portal");
            this.btnlogoUp.UseVisualStyleBackColor = true;
            this.btnlogoUp.Click += new System.EventHandler(this.btnlogoUp_Click);
            // 
            // lnkLogoImp
            // 
            this.lnkLogoImp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkLogoImp.Location = new System.Drawing.Point(123, 839);
            this.lnkLogoImp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lnkLogoImp.Name = "lnkLogoImp";
            this.lnkLogoImp.Size = new System.Drawing.Size(76, 20);
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
            this.lstLogos.Location = new System.Drawing.Point(10, 29);
            this.lstLogos.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lstLogos.Name = "lstLogos";
            this.lstLogos.ScrollAlwaysVisible = true;
            this.lstLogos.Size = new System.Drawing.Size(1108, 797);
            this.lstLogos.TabIndex = 0;
            this.toolTip_Help.SetToolTip(this.lstLogos, "Contains the list of Logo rules defined to display in Media Portal");
            // 
            // btnLogoTemplate
            // 
            this.btnLogoTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLogoTemplate.Location = new System.Drawing.Point(10, 836);
            this.btnLogoTemplate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnLogoTemplate.Name = "btnLogoTemplate";
            this.btnLogoTemplate.Size = new System.Drawing.Size(105, 26);
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
            this.btnrmvLogo.Location = new System.Drawing.Point(674, 833);
            this.btnrmvLogo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnrmvLogo.Name = "btnrmvLogo";
            this.btnrmvLogo.Size = new System.Drawing.Size(105, 26);
            this.btnrmvLogo.TabIndex = 7;
            this.btnrmvLogo.Text = "&Delete";
            this.toolTip_Help.SetToolTip(this.btnrmvLogo, "Click to remove the selected logo rule in list");
            this.btnrmvLogo.UseVisualStyleBackColor = true;
            this.btnrmvLogo.Click += new System.EventHandler(this.btnrmvLogo_Click);
            // 
            // lnkLogoExport
            // 
            this.lnkLogoExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkLogoExport.Location = new System.Drawing.Point(197, 839);
            this.lnkLogoExport.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lnkLogoExport.Name = "lnkLogoExport";
            this.lnkLogoExport.Size = new System.Drawing.Size(76, 20);
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
            this.btnLogoEdit.Location = new System.Drawing.Point(900, 833);
            this.btnLogoEdit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnLogoEdit.Name = "btnLogoEdit";
            this.btnLogoEdit.Size = new System.Drawing.Size(105, 26);
            this.btnLogoEdit.TabIndex = 8;
            this.btnLogoEdit.Text = "&Edit...";
            this.toolTip_Help.SetToolTip(this.btnLogoEdit, "Click to edit the selected logo rule in list");
            this.btnLogoEdit.UseVisualStyleBackColor = true;
            this.btnLogoEdit.Click += new System.EventHandler(this.btnLogoEdit_Click);
            // 
            // addLogo
            // 
            this.addLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addLogo.Location = new System.Drawing.Point(1013, 833);
            this.addLogo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.addLogo.Name = "addLogo";
            this.addLogo.Size = new System.Drawing.Size(105, 26);
            this.addLogo.TabIndex = 9;
            this.addLogo.Text = "&Add...";
            this.toolTip_Help.SetToolTip(this.addLogo, resources.GetString("addLogo.ToolTip"));
            this.addLogo.UseVisualStyleBackColor = true;
            this.addLogo.Click += new System.EventHandler(this.addLogo_Click);
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button1.Image = global::WindowPlugins.GUITVSeries.Properties.Resources.arrow_up_small;
            this.button1.Location = new System.Drawing.Point(0, 977);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(1203, 22);
            this.button1.TabIndex = 0;
            this.toolTip_Help.SetToolTip(this.button1, "Click here to show/hide the log window, useful for diagnosing any errors or watch" +
        "ing progress of Import");
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // toolTip_InfoHelp
            // 
            this.toolTip_InfoHelp.AutoPopDelay = 10000;
            this.toolTip_InfoHelp.InitialDelay = 500;
            this.toolTip_InfoHelp.IsBalloon = true;
            this.toolTip_InfoHelp.ReshowDelay = 100;
            this.toolTip_InfoHelp.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip_InfoHelp.ToolTipTitle = "Help";
            // 
            // numericUpDownImportDelay
            // 
            this.numericUpDownImportDelay.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownImportDelay.Location = new System.Drawing.Point(172, 127);
            this.numericUpDownImportDelay.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownImportDelay.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.numericUpDownImportDelay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownImportDelay.Name = "numericUpDownImportDelay";
            this.numericUpDownImportDelay.Size = new System.Drawing.Size(75, 20);
            this.numericUpDownImportDelay.TabIndex = 11;
            this.toolTip_InfoHelp.SetToolTip(this.numericUpDownImportDelay, "Set the delay before the Importer starts in GUI, this can help reduce load of CPU" +
        " on startup.");
            this.numericUpDownImportDelay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownImportDelay.ValueChanged += new System.EventHandler(this.numericUpDownImportDelay_ValueChanged);
            // 
            // checkBox_Episode_HideUnwatchedSummary
            // 
            this.checkBox_Episode_HideUnwatchedSummary.AutoSize = true;
            this.checkBox_Episode_HideUnwatchedSummary.Location = new System.Drawing.Point(8, 180);
            this.checkBox_Episode_HideUnwatchedSummary.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox_Episode_HideUnwatchedSummary.Name = "checkBox_Episode_HideUnwatchedSummary";
            this.checkBox_Episode_HideUnwatchedSummary.Size = new System.Drawing.Size(335, 17);
            this.checkBox_Episode_HideUnwatchedSummary.TabIndex = 13;
            this.checkBox_Episode_HideUnwatchedSummary.Text = "&Hide episode overview/summary on episodes that are unwatched";
            this.toolTip_InfoHelp.SetToolTip(this.checkBox_Episode_HideUnwatchedSummary, resources.GetString("checkBox_Episode_HideUnwatchedSummary.ToolTip"));
            this.checkBox_Episode_HideUnwatchedSummary.UseVisualStyleBackColor = true;
            this.checkBox_Episode_HideUnwatchedSummary.CheckedChanged += new System.EventHandler(this.checkBox_Episode_HideUnwatchedSummary_CheckedChanged);
            // 
            // checkBox_Episode_HideUnwatchedThumbnail
            // 
            this.checkBox_Episode_HideUnwatchedThumbnail.AutoSize = true;
            this.checkBox_Episode_HideUnwatchedThumbnail.Location = new System.Drawing.Point(8, 207);
            this.checkBox_Episode_HideUnwatchedThumbnail.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox_Episode_HideUnwatchedThumbnail.Name = "checkBox_Episode_HideUnwatchedThumbnail";
            this.checkBox_Episode_HideUnwatchedThumbnail.Size = new System.Drawing.Size(291, 17);
            this.checkBox_Episode_HideUnwatchedThumbnail.TabIndex = 14;
            this.checkBox_Episode_HideUnwatchedThumbnail.Text = "Hide episode &thumbnail on episodes that are unwatched";
            this.toolTip_InfoHelp.SetToolTip(this.checkBox_Episode_HideUnwatchedThumbnail, resources.GetString("checkBox_Episode_HideUnwatchedThumbnail.ToolTip"));
            this.checkBox_Episode_HideUnwatchedThumbnail.UseVisualStyleBackColor = true;
            this.checkBox_Episode_HideUnwatchedThumbnail.CheckedChanged += new System.EventHandler(this.checkBox_Episode_HideUnwatchedThumbnail_CheckedChanged);
            // 
            // checkBox_Episode_OnlyShowLocalFiles
            // 
            this.checkBox_Episode_OnlyShowLocalFiles.AutoSize = true;
            this.checkBox_Episode_OnlyShowLocalFiles.Location = new System.Drawing.Point(467, 171);
            this.checkBox_Episode_OnlyShowLocalFiles.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox_Episode_OnlyShowLocalFiles.Name = "checkBox_Episode_OnlyShowLocalFiles";
            this.checkBox_Episode_OnlyShowLocalFiles.Size = new System.Drawing.Size(256, 17);
            this.checkBox_Episode_OnlyShowLocalFiles.TabIndex = 34;
            this.checkBox_Episode_OnlyShowLocalFiles.Text = "&Show all episodes in local database when in GUI";
            this.toolTip_InfoHelp.SetToolTip(this.checkBox_Episode_OnlyShowLocalFiles, resources.GetString("checkBox_Episode_OnlyShowLocalFiles.ToolTip"));
            this.checkBox_Episode_OnlyShowLocalFiles.UseVisualStyleBackColor = true;
            this.checkBox_Episode_OnlyShowLocalFiles.CheckedChanged += new System.EventHandler(this.checkBox_Episode_OnlyShowLocalFiles_CheckedChanged);
            // 
            // chkUseRegionalDateFormatString
            // 
            this.chkUseRegionalDateFormatString.AutoSize = true;
            this.chkUseRegionalDateFormatString.Location = new System.Drawing.Point(8, 234);
            this.chkUseRegionalDateFormatString.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkUseRegionalDateFormatString.Name = "chkUseRegionalDateFormatString";
            this.chkUseRegionalDateFormatString.Size = new System.Drawing.Size(211, 17);
            this.chkUseRegionalDateFormatString.TabIndex = 15;
            this.chkUseRegionalDateFormatString.Text = "&Use Regional Settings to Display Dates";
            this.toolTip_InfoHelp.SetToolTip(this.chkUseRegionalDateFormatString, "Enable this option to convert all date formats yyyy-MM-dd to format defined in th" +
        "e Windows Control Panel\r\nunder Regional and Language options.");
            this.chkUseRegionalDateFormatString.UseVisualStyleBackColor = true;
            this.chkUseRegionalDateFormatString.CheckedChanged += new System.EventHandler(this.chkUseRegionalDateFormatString_CheckedChanged);
            // 
            // richTextBox_seasonFormat_Col3
            // 
            this.richTextBox_seasonFormat_Col3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_seasonFormat_Col3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seasonFormat_Col3.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seasonFormat_Col3.Location = new System.Drawing.Point(765, 23);
            this.richTextBox_seasonFormat_Col3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBox_seasonFormat_Col3.Multiline = false;
            this.richTextBox_seasonFormat_Col3.Name = "richTextBox_seasonFormat_Col3";
            this.richTextBox_seasonFormat_Col3.Size = new System.Drawing.Size(382, 24);
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
            this.richTextBox_seasonFormat_Main.Location = new System.Drawing.Point(110, 101);
            this.richTextBox_seasonFormat_Main.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBox_seasonFormat_Main.Multiline = false;
            this.richTextBox_seasonFormat_Main.Name = "richTextBox_seasonFormat_Main";
            this.richTextBox_seasonFormat_Main.Size = new System.Drawing.Size(1037, 24);
            this.richTextBox_seasonFormat_Main.TabIndex = 10;
            this.richTextBox_seasonFormat_Main.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_seasonFormat_Main, resources.GetString("richTextBox_seasonFormat_Main.ToolTip"));
            // 
            // richTextBox_seasonFormat_Col1
            // 
            this.richTextBox_seasonFormat_Col1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seasonFormat_Col1.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seasonFormat_Col1.Location = new System.Drawing.Point(110, 23);
            this.richTextBox_seasonFormat_Col1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBox_seasonFormat_Col1.Multiline = false;
            this.richTextBox_seasonFormat_Col1.Name = "richTextBox_seasonFormat_Col1";
            this.richTextBox_seasonFormat_Col1.Size = new System.Drawing.Size(268, 24);
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
            this.richTextBox_seasonFormat_Subtitle.Location = new System.Drawing.Point(110, 75);
            this.richTextBox_seasonFormat_Subtitle.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBox_seasonFormat_Subtitle.Multiline = false;
            this.richTextBox_seasonFormat_Subtitle.Name = "richTextBox_seasonFormat_Subtitle";
            this.richTextBox_seasonFormat_Subtitle.Size = new System.Drawing.Size(1037, 24);
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
            this.richTextBox_seasonFormat_Col2.Location = new System.Drawing.Point(378, 23);
            this.richTextBox_seasonFormat_Col2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBox_seasonFormat_Col2.Multiline = false;
            this.richTextBox_seasonFormat_Col2.Name = "richTextBox_seasonFormat_Col2";
            this.richTextBox_seasonFormat_Col2.Size = new System.Drawing.Size(389, 24);
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
            this.richTextBox_seasonFormat_Title.Location = new System.Drawing.Point(110, 49);
            this.richTextBox_seasonFormat_Title.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBox_seasonFormat_Title.Multiline = false;
            this.richTextBox_seasonFormat_Title.Name = "richTextBox_seasonFormat_Title";
            this.richTextBox_seasonFormat_Title.Size = new System.Drawing.Size(1037, 24);
            this.richTextBox_seasonFormat_Title.TabIndex = 6;
            this.richTextBox_seasonFormat_Title.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_seasonFormat_Title, resources.GetString("richTextBox_seasonFormat_Title.ToolTip"));
            // 
            // richTextBox_episodeFormat_Col3
            // 
            this.richTextBox_episodeFormat_Col3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_episodeFormat_Col3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_episodeFormat_Col3.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_episodeFormat_Col3.Location = new System.Drawing.Point(765, 21);
            this.richTextBox_episodeFormat_Col3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBox_episodeFormat_Col3.Multiline = false;
            this.richTextBox_episodeFormat_Col3.Name = "richTextBox_episodeFormat_Col3";
            this.richTextBox_episodeFormat_Col3.Size = new System.Drawing.Size(382, 24);
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
            this.richTextBox_episodeFormat_Col2.Location = new System.Drawing.Point(376, 21);
            this.richTextBox_episodeFormat_Col2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBox_episodeFormat_Col2.Multiline = false;
            this.richTextBox_episodeFormat_Col2.Name = "richTextBox_episodeFormat_Col2";
            this.richTextBox_episodeFormat_Col2.Size = new System.Drawing.Size(391, 24);
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
            this.richTextBox_episodeFormat_Main.Location = new System.Drawing.Point(110, 99);
            this.richTextBox_episodeFormat_Main.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBox_episodeFormat_Main.Multiline = false;
            this.richTextBox_episodeFormat_Main.Name = "richTextBox_episodeFormat_Main";
            this.richTextBox_episodeFormat_Main.Size = new System.Drawing.Size(1037, 24);
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
            this.richTextBox_episodeFormat_Subtitle.Location = new System.Drawing.Point(110, 73);
            this.richTextBox_episodeFormat_Subtitle.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBox_episodeFormat_Subtitle.Multiline = false;
            this.richTextBox_episodeFormat_Subtitle.Name = "richTextBox_episodeFormat_Subtitle";
            this.richTextBox_episodeFormat_Subtitle.Size = new System.Drawing.Size(1037, 24);
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
            this.richTextBox_episodeFormat_Title.Location = new System.Drawing.Point(110, 47);
            this.richTextBox_episodeFormat_Title.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBox_episodeFormat_Title.Multiline = false;
            this.richTextBox_episodeFormat_Title.Name = "richTextBox_episodeFormat_Title";
            this.richTextBox_episodeFormat_Title.Size = new System.Drawing.Size(1037, 24);
            this.richTextBox_episodeFormat_Title.TabIndex = 5;
            this.richTextBox_episodeFormat_Title.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_episodeFormat_Title, resources.GetString("richTextBox_episodeFormat_Title.ToolTip"));
            // 
            // richTextBox_episodeFormat_Col1
            // 
            this.richTextBox_episodeFormat_Col1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_episodeFormat_Col1.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_episodeFormat_Col1.Location = new System.Drawing.Point(110, 21);
            this.richTextBox_episodeFormat_Col1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBox_episodeFormat_Col1.Multiline = false;
            this.richTextBox_episodeFormat_Col1.Name = "richTextBox_episodeFormat_Col1";
            this.richTextBox_episodeFormat_Col1.Size = new System.Drawing.Size(268, 24);
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
            this.richTextBox_seriesFormat_Col3.Location = new System.Drawing.Point(767, 18);
            this.richTextBox_seriesFormat_Col3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBox_seriesFormat_Col3.Multiline = false;
            this.richTextBox_seriesFormat_Col3.Name = "richTextBox_seriesFormat_Col3";
            this.richTextBox_seriesFormat_Col3.Size = new System.Drawing.Size(380, 24);
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
            this.richTextBox_seriesFormat_Main.Location = new System.Drawing.Point(112, 96);
            this.richTextBox_seriesFormat_Main.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBox_seriesFormat_Main.Multiline = false;
            this.richTextBox_seriesFormat_Main.Name = "richTextBox_seriesFormat_Main";
            this.richTextBox_seriesFormat_Main.Size = new System.Drawing.Size(1035, 24);
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
            this.richTextBox_seriesFormat_Subtitle.Location = new System.Drawing.Point(112, 70);
            this.richTextBox_seriesFormat_Subtitle.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBox_seriesFormat_Subtitle.Multiline = false;
            this.richTextBox_seriesFormat_Subtitle.Name = "richTextBox_seriesFormat_Subtitle";
            this.richTextBox_seriesFormat_Subtitle.Size = new System.Drawing.Size(1035, 24);
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
            this.richTextBox_seriesFormat_Col2.Location = new System.Drawing.Point(380, 18);
            this.richTextBox_seriesFormat_Col2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBox_seriesFormat_Col2.Multiline = false;
            this.richTextBox_seriesFormat_Col2.Name = "richTextBox_seriesFormat_Col2";
            this.richTextBox_seriesFormat_Col2.Size = new System.Drawing.Size(389, 24);
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
            this.richTextBox_seriesFormat_Title.Location = new System.Drawing.Point(112, 44);
            this.richTextBox_seriesFormat_Title.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBox_seriesFormat_Title.Multiline = false;
            this.richTextBox_seriesFormat_Title.Name = "richTextBox_seriesFormat_Title";
            this.richTextBox_seriesFormat_Title.Size = new System.Drawing.Size(1035, 24);
            this.richTextBox_seriesFormat_Title.TabIndex = 6;
            this.richTextBox_seriesFormat_Title.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_seriesFormat_Title, resources.GetString("richTextBox_seriesFormat_Title.ToolTip"));
            // 
            // richTextBox_seriesFormat_Col1
            // 
            this.richTextBox_seriesFormat_Col1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seriesFormat_Col1.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seriesFormat_Col1.Location = new System.Drawing.Point(112, 18);
            this.richTextBox_seriesFormat_Col1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBox_seriesFormat_Col1.Multiline = false;
            this.richTextBox_seriesFormat_Col1.Name = "richTextBox_seriesFormat_Col1";
            this.richTextBox_seriesFormat_Col1.Size = new System.Drawing.Size(268, 24);
            this.richTextBox_seriesFormat_Col1.TabIndex = 2;
            this.richTextBox_seriesFormat_Col1.Text = "";
            this.toolTip_InfoHelp.SetToolTip(this.richTextBox_seriesFormat_Col1, "Enter in the field(s) to be displayed in column one of the series view.\r\nYou can " +
        "right click on this textbox to bring up a menu of available fields from the data" +
        "base.\r\n\r\nDefault: Empty");
            // 
            // dbCheckBoxTraktCommunityRatings
            // 
            this.dbCheckBoxTraktCommunityRatings.AutoSize = true;
            this.dbCheckBoxTraktCommunityRatings.Location = new System.Drawing.Point(12, 238);
            this.dbCheckBoxTraktCommunityRatings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dbCheckBoxTraktCommunityRatings.Name = "dbCheckBoxTraktCommunityRatings";
            this.dbCheckBoxTraktCommunityRatings.Option = "TraktCommunityRatings";
            this.dbCheckBoxTraktCommunityRatings.Size = new System.Drawing.Size(320, 17);
            this.dbCheckBoxTraktCommunityRatings.TabIndex = 19;
            this.dbCheckBoxTraktCommunityRatings.Text = "Automatically update community ratings and votes from trakt.tv";
            this.dbCheckBoxTraktCommunityRatings.ToolTip = "";
            this.toolTip_InfoHelp.SetToolTip(this.dbCheckBoxTraktCommunityRatings, resources.GetString("dbCheckBoxTraktCommunityRatings.ToolTip"));
            this.dbCheckBoxTraktCommunityRatings.UseVisualStyleBackColor = true;
            // 
            // dbOptCheckBoxCleanOnlineEpisodes
            // 
            this.dbOptCheckBoxCleanOnlineEpisodes.AutoSize = true;
            this.dbOptCheckBoxCleanOnlineEpisodes.Checked = true;
            this.dbOptCheckBoxCleanOnlineEpisodes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dbOptCheckBoxCleanOnlineEpisodes.Location = new System.Drawing.Point(12, 319);
            this.dbOptCheckBoxCleanOnlineEpisodes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dbOptCheckBoxCleanOnlineEpisodes.Name = "dbOptCheckBoxCleanOnlineEpisodes";
            this.dbOptCheckBoxCleanOnlineEpisodes.Option = "CleanOnlineEpisodes";
            this.dbOptCheckBoxCleanOnlineEpisodes.Size = new System.Drawing.Size(368, 17);
            this.dbOptCheckBoxCleanOnlineEpisodes.TabIndex = 22;
            this.dbOptCheckBoxCleanOnlineEpisodes.Text = "Remove online episode references from database when not found online";
            this.dbOptCheckBoxCleanOnlineEpisodes.ToolTip = "";
            this.toolTip_InfoHelp.SetToolTip(this.dbOptCheckBoxCleanOnlineEpisodes, "This option will help keep your database clean when enabled.\r\n\r\nThis option is on" +
        "ly applicable if all episode information is downloaded.");
            this.dbOptCheckBoxCleanOnlineEpisodes.UseVisualStyleBackColor = true;
            this.dbOptCheckBoxCleanOnlineEpisodes.CheckedChanged += new System.EventHandler(this.dbOptCheckBoxCleanOnlineEpisodes_CheckedChanged);
            // 
            // dbOptionCheckBox4
            // 
            this.dbOptionCheckBox4.AutoSize = true;
            this.dbOptionCheckBox4.Location = new System.Drawing.Point(12, 540);
            this.dbOptionCheckBox4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dbOptionCheckBox4.Name = "dbOptionCheckBox4";
            this.dbOptionCheckBox4.Option = "checkArtwork";
            this.dbOptionCheckBox4.Size = new System.Drawing.Size(244, 17);
            this.dbOptionCheckBox4.TabIndex = 31;
            this.dbOptionCheckBox4.Text = "Check artwork downloaded exists and is valid.";
            this.dbOptionCheckBox4.ToolTip = "";
            this.toolTip_InfoHelp.SetToolTip(this.dbOptionCheckBox4, "Enable this option for an additional check of all series posters and banners.\r\n\r\n" +
        "If artwork is not found on disk or is corrupt, then entry from database is clear" +
        "ed.");
            this.dbOptionCheckBox4.UseVisualStyleBackColor = true;
            // 
            // dbOptChkBoxPlayOutOfOrderCheck
            // 
            this.dbOptChkBoxPlayOutOfOrderCheck.AutoSize = true;
            this.dbOptChkBoxPlayOutOfOrderCheck.Checked = true;
            this.dbOptChkBoxPlayOutOfOrderCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dbOptChkBoxPlayOutOfOrderCheck.Location = new System.Drawing.Point(467, 333);
            this.dbOptChkBoxPlayOutOfOrderCheck.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dbOptChkBoxPlayOutOfOrderCheck.Name = "dbOptChkBoxPlayOutOfOrderCheck";
            this.dbOptChkBoxPlayOutOfOrderCheck.Option = "CheckPlayOutOfOrder";
            this.dbOptChkBoxPlayOutOfOrderCheck.Size = new System.Drawing.Size(255, 17);
            this.dbOptChkBoxPlayOutOfOrderCheck.TabIndex = 40;
            this.dbOptChkBoxPlayOutOfOrderCheck.Text = "Check on playback of episode is not out of order";
            this.dbOptChkBoxPlayOutOfOrderCheck.ToolTip = "";
            this.toolTip_InfoHelp.SetToolTip(this.dbOptChkBoxPlayOutOfOrderCheck, resources.GetString("dbOptChkBoxPlayOutOfOrderCheck.ToolTip"));
            this.dbOptChkBoxPlayOutOfOrderCheck.UseVisualStyleBackColor = true;
            // 
            // dbOptionSQLLogging
            // 
            this.dbOptionSQLLogging.AutoSize = true;
            this.dbOptionSQLLogging.Location = new System.Drawing.Point(467, 19);
            this.dbOptionSQLLogging.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dbOptionSQLLogging.Name = "dbOptionSQLLogging";
            this.dbOptionSQLLogging.Option = "SQLLoggingEnabled";
            this.dbOptionSQLLogging.Size = new System.Drawing.Size(278, 17);
            this.dbOptionSQLLogging.TabIndex = 28;
            this.dbOptionSQLLogging.Text = "Enable Verbose SQL Logging (Impacts Performance!)";
            this.dbOptionSQLLogging.ToolTip = "";
            this.toolTip_InfoHelp.SetToolTip(this.dbOptionSQLLogging, "Enable verbose SQL Logging, WARNING!!! This will impact performance if enabled.");
            this.dbOptionSQLLogging.UseVisualStyleBackColor = true;
            this.dbOptionSQLLogging.CheckStateChanged += new System.EventHandler(this.dbOptionSQLLogging_CheckStateChanged);
            // 
            // dbOptChkBox_SubCentral_DownloadSubtitlesOnPlay
            // 
            this.dbOptChkBox_SubCentral_DownloadSubtitlesOnPlay.AutoSize = true;
            this.dbOptChkBox_SubCentral_DownloadSubtitlesOnPlay.Location = new System.Drawing.Point(467, 360);
            this.dbOptChkBox_SubCentral_DownloadSubtitlesOnPlay.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dbOptChkBox_SubCentral_DownloadSubtitlesOnPlay.Name = "dbOptChkBox_SubCentral_DownloadSubtitlesOnPlay";
            this.dbOptChkBox_SubCentral_DownloadSubtitlesOnPlay.Option = "SubCentral_SubtitleDownloadOnPlay";
            this.dbOptChkBox_SubCentral_DownloadSubtitlesOnPlay.Size = new System.Drawing.Size(281, 17);
            this.dbOptChkBox_SubCentral_DownloadSubtitlesOnPlay.TabIndex = 41;
            this.dbOptChkBox_SubCentral_DownloadSubtitlesOnPlay.Text = "Offer to download subtitles before playing (SubCentral)";
            this.dbOptChkBox_SubCentral_DownloadSubtitlesOnPlay.ToolTip = "";
            this.toolTip_InfoHelp.SetToolTip(this.dbOptChkBox_SubCentral_DownloadSubtitlesOnPlay, "Check this option to enable subtitle download before playing the episode that doe" +
        "sn\'t have any.\r\nNote: applicable only when using SubCentral.");
            this.dbOptChkBox_SubCentral_DownloadSubtitlesOnPlay.UseVisualStyleBackColor = true;
            // 
            // checkBox_CountSpecialEpisodesAsWatched
            // 
            this.checkBox_CountSpecialEpisodesAsWatched.AutoSize = true;
            this.checkBox_CountSpecialEpisodesAsWatched.Location = new System.Drawing.Point(467, 387);
            this.checkBox_CountSpecialEpisodesAsWatched.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox_CountSpecialEpisodesAsWatched.Name = "checkBox_CountSpecialEpisodesAsWatched";
            this.checkBox_CountSpecialEpisodesAsWatched.Option = "CountSpecialEpisodesAsWatched";
            this.checkBox_CountSpecialEpisodesAsWatched.Size = new System.Drawing.Size(267, 17);
            this.checkBox_CountSpecialEpisodesAsWatched.TabIndex = 42;
            this.checkBox_CountSpecialEpisodesAsWatched.Text = "Count the Special (Season 0) episodes as watched";
            this.checkBox_CountSpecialEpisodesAsWatched.ToolTip = "";
            this.toolTip_InfoHelp.SetToolTip(this.checkBox_CountSpecialEpisodesAsWatched, resources.GetString("checkBox_CountSpecialEpisodesAsWatched.ToolTip"));
            this.checkBox_CountSpecialEpisodesAsWatched.UseVisualStyleBackColor = true;
            // 
            // dbOptChkBoxCountEmptyFutureEps
            // 
            this.dbOptChkBoxCountEmptyFutureEps.AutoSize = true;
            this.dbOptChkBoxCountEmptyFutureEps.Checked = true;
            this.dbOptChkBoxCountEmptyFutureEps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dbOptChkBoxCountEmptyFutureEps.Location = new System.Drawing.Point(467, 306);
            this.dbOptChkBoxCountEmptyFutureEps.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dbOptChkBoxCountEmptyFutureEps.Name = "dbOptChkBoxCountEmptyFutureEps";
            this.dbOptChkBoxCountEmptyFutureEps.Option = "CountEmptyAndFutureAiredEps";
            this.dbOptChkBoxCountEmptyFutureEps.Size = new System.Drawing.Size(289, 17);
            this.dbOptChkBoxCountEmptyFutureEps.TabIndex = 39;
            this.dbOptChkBoxCountEmptyFutureEps.Text = "Count episodes that have no AirDate or Air in the Future";
            this.dbOptChkBoxCountEmptyFutureEps.ToolTip = "";
            this.toolTip_InfoHelp.SetToolTip(this.dbOptChkBoxCountEmptyFutureEps, "Check this option if you want to calculate episode counts were episodes do not ha" +
        "ve an airdate or airs at a future date\r\nThis will only count episodes that you c" +
        "an see when browsing GUI.");
            this.dbOptChkBoxCountEmptyFutureEps.UseVisualStyleBackColor = true;
            // 
            // dbOptionCheckBoxMarkRatedEpsAsWatched
            // 
            this.dbOptionCheckBoxMarkRatedEpsAsWatched.AutoSize = true;
            this.dbOptionCheckBoxMarkRatedEpsAsWatched.Location = new System.Drawing.Point(467, 198);
            this.dbOptionCheckBoxMarkRatedEpsAsWatched.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dbOptionCheckBoxMarkRatedEpsAsWatched.Name = "dbOptionCheckBoxMarkRatedEpsAsWatched";
            this.dbOptionCheckBoxMarkRatedEpsAsWatched.Option = "MarkRatedEpisodeAsWatched";
            this.dbOptionCheckBoxMarkRatedEpsAsWatched.Size = new System.Drawing.Size(305, 17);
            this.dbOptionCheckBoxMarkRatedEpsAsWatched.TabIndex = 35;
            this.dbOptionCheckBoxMarkRatedEpsAsWatched.Text = "Mark Episodes as Watched if rated online at theTVDB.com";
            this.dbOptionCheckBoxMarkRatedEpsAsWatched.ToolTip = "";
            this.toolTip_InfoHelp.SetToolTip(this.dbOptionCheckBoxMarkRatedEpsAsWatched, resources.GetString("dbOptionCheckBoxMarkRatedEpsAsWatched.ToolTip"));
            this.dbOptionCheckBoxMarkRatedEpsAsWatched.UseVisualStyleBackColor = true;
            // 
            // optionAsk2Rate
            // 
            this.optionAsk2Rate.AutoSize = true;
            this.optionAsk2Rate.Location = new System.Drawing.Point(8, 126);
            this.optionAsk2Rate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.optionAsk2Rate.Name = "optionAsk2Rate";
            this.optionAsk2Rate.Option = "askToRate";
            this.optionAsk2Rate.Size = new System.Drawing.Size(234, 17);
            this.optionAsk2Rate.TabIndex = 11;
            this.optionAsk2Rate.Text = "&Popup Rate Dialog after episode is watched";
            this.optionAsk2Rate.ToolTip = "";
            this.toolTip_InfoHelp.SetToolTip(this.optionAsk2Rate, resources.GetString("optionAsk2Rate.ToolTip"));
            this.optionAsk2Rate.UseVisualStyleBackColor = true;
            this.optionAsk2Rate.CheckedChanged += new System.EventHandler(this.optionAsk2Rate_CheckedChanged);
            // 
            // dbOptionParsedNameFromFolder
            // 
            this.dbOptionParsedNameFromFolder.AutoSize = true;
            this.dbOptionParsedNameFromFolder.Location = new System.Drawing.Point(5, 155);
            this.dbOptionParsedNameFromFolder.Name = "dbOptionParsedNameFromFolder";
            this.dbOptionParsedNameFromFolder.Option = "ParsedNameFromFolder";
            this.dbOptionParsedNameFromFolder.Size = new System.Drawing.Size(228, 17);
            this.dbOptionParsedNameFromFolder.TabIndex = 152;
            this.dbOptionParsedNameFromFolder.Text = "Set the series name basis the folder name?";
            this.dbOptionParsedNameFromFolder.ToolTip = "";
            this.toolTip_InfoHelp.SetToolTip(this.dbOptionParsedNameFromFolder, resources.GetString("dbOptionParsedNameFromFolder.ToolTip"));
            this.dbOptionParsedNameFromFolder.UseVisualStyleBackColor = true;
            // 
            // splitMain_Log
            // 
            this.splitMain_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain_Log.IsSplitterFixed = true;
            this.splitMain_Log.Location = new System.Drawing.Point(0, 0);
            this.splitMain_Log.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.splitMain_Log.Panel2.Padding = new System.Windows.Forms.Padding(15);
            this.splitMain_Log.Panel2Collapsed = true;
            this.splitMain_Log.Size = new System.Drawing.Size(1203, 999);
            this.splitMain_Log.SplitterDistance = 382;
            this.splitMain_Log.SplitterWidth = 2;
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
            this.tabControl_Details.Controls.Add(this.tabAbout);
            this.tabControl_Details.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_Details.ImageList = this.imageList1;
            this.tabControl_Details.Location = new System.Drawing.Point(0, 0);
            this.tabControl_Details.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl_Details.Name = "tabControl_Details";
            this.tabControl_Details.SelectedIndex = 0;
            this.tabControl_Details.Size = new System.Drawing.Size(1203, 977);
            this.tabControl_Details.TabIndex = 0;
            // 
            // tabPage_Details
            // 
            this.tabPage_Details.Controls.Add(this.splitContainer2);
            this.tabPage_Details.ImageIndex = 1;
            this.tabPage_Details.Location = new System.Drawing.Point(4, 31);
            this.tabPage_Details.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage_Details.Name = "tabPage_Details";
            this.tabPage_Details.Size = new System.Drawing.Size(1195, 942);
            this.tabPage_Details.TabIndex = 2;
            this.tabPage_Details.Text = "Details";
            this.tabPage_Details.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.linkMediaInfoUpdate);
            this.splitContainer2.Panel1.Controls.Add(this.treeView_Library);
            this.splitContainer2.Panel1.Controls.Add(this.checkBox_ShowHidden);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lnkIMDbSeries);
            this.splitContainer2.Panel2.Controls.Add(this.lnkTraktSeries);
            this.splitContainer2.Panel2.Controls.Add(this.lnkTVDbSeries);
            this.splitContainer2.Panel2.Controls.Add(this.label23);
            this.splitContainer2.Panel2.Controls.Add(this.lnkImageCache);
            this.splitContainer2.Panel2.Controls.Add(this.lnkOpenAPICacheDir);
            this.splitContainer2.Panel2.Controls.Add(this.pictureBox_SeriesPoster);
            this.splitContainer2.Panel2.Controls.Add(this.comboBox_PosterSelection);
            this.splitContainer2.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer2.Panel2.Controls.Add(this.pictureBox_Series);
            this.splitContainer2.Panel2.Controls.Add(this.comboBox_BannerSelection);
            this.splitContainer2.Panel2.Controls.Add(this.panDBLocation);
            this.splitContainer2.Size = new System.Drawing.Size(1195, 942);
            this.splitContainer2.SplitterDistance = 227;
            this.splitContainer2.SplitterWidth = 6;
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
            this.treeView_Library.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.treeView_Library.MinimumSize = new System.Drawing.Size(148, 152);
            this.treeView_Library.Name = "treeView_Library";
            this.treeView_Library.Size = new System.Drawing.Size(225, 900);
            this.treeView_Library.TabIndex = 0;
            this.treeView_Library.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeView_Library_AfterExpand);
            this.treeView_Library.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_Library_AfterSelect);
            this.treeView_Library.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_Library_NodeMouseClick);
            this.treeView_Library.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView_Library_KeyDown);
            this.treeView_Library.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView_Library_MouseDown);
            // 
            // lnkIMDbSeries
            // 
            this.lnkIMDbSeries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkIMDbSeries.AutoSize = true;
            this.lnkIMDbSeries.Location = new System.Drawing.Point(753, 914);
            this.lnkIMDbSeries.Name = "lnkIMDbSeries";
            this.lnkIMDbSeries.Size = new System.Drawing.Size(56, 13);
            this.lnkIMDbSeries.TabIndex = 155;
            this.lnkIMDbSeries.TabStop = true;
            this.lnkIMDbSeries.Text = "IMDb.com";
            this.lnkIMDbSeries.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lnkIMDbSeries.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkIMDbSeries_LinkClicked);
            // 
            // lnkTraktSeries
            // 
            this.lnkTraktSeries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkTraktSeries.AutoSize = true;
            this.lnkTraktSeries.Location = new System.Drawing.Point(814, 914);
            this.lnkTraktSeries.Name = "lnkTraktSeries";
            this.lnkTraktSeries.Size = new System.Drawing.Size(40, 13);
            this.lnkTraktSeries.TabIndex = 154;
            this.lnkTraktSeries.TabStop = true;
            this.lnkTraktSeries.Text = "trakt.tv";
            this.lnkTraktSeries.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lnkTraktSeries.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkTraktSeries_LinkClicked);
            // 
            // lnkTVDbSeries
            // 
            this.lnkTVDbSeries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkTVDbSeries.AutoSize = true;
            this.lnkTVDbSeries.Location = new System.Drawing.Point(859, 914);
            this.lnkTVDbSeries.Name = "lnkTVDbSeries";
            this.lnkTVDbSeries.Size = new System.Drawing.Size(73, 13);
            this.lnkTVDbSeries.TabIndex = 153;
            this.lnkTVDbSeries.TabStop = true;
            this.lnkTVDbSeries.Text = "theTVDb.com";
            this.lnkTVDbSeries.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lnkTVDbSeries.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkTVDbSeries_LinkClicked);
            // 
            // label23
            // 
            this.label23.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(3, 911);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(168, 13);
            this.label23.TabIndex = 152;
            this.label23.Text = "Open resource for selected series:";
            // 
            // lnkImageCache
            // 
            this.lnkImageCache.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkImageCache.AutoSize = true;
            this.lnkImageCache.Location = new System.Drawing.Point(287, 911);
            this.lnkImageCache.Name = "lnkImageCache";
            this.lnkImageCache.Size = new System.Drawing.Size(75, 13);
            this.lnkImageCache.TabIndex = 151;
            this.lnkImageCache.TabStop = true;
            this.lnkImageCache.Text = "Artwork Folder";
            this.lnkImageCache.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkImageCache_LinkClicked);
            // 
            // lnkOpenAPICacheDir
            // 
            this.lnkOpenAPICacheDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkOpenAPICacheDir.AutoSize = true;
            this.lnkOpenAPICacheDir.Location = new System.Drawing.Point(182, 911);
            this.lnkOpenAPICacheDir.Name = "lnkOpenAPICacheDir";
            this.lnkOpenAPICacheDir.Size = new System.Drawing.Size(90, 13);
            this.lnkOpenAPICacheDir.TabIndex = 150;
            this.lnkOpenAPICacheDir.TabStop = true;
            this.lnkOpenAPICacheDir.Text = "API Cache Folder";
            this.lnkOpenAPICacheDir.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkOpenAPICacheDir_LinkClicked);
            // 
            // pictureBox_SeriesPoster
            // 
            this.pictureBox_SeriesPoster.BackColor = System.Drawing.Color.White;
            this.pictureBox_SeriesPoster.ErrorImage = null;
            this.pictureBox_SeriesPoster.InitialImage = null;
            this.pictureBox_SeriesPoster.Location = new System.Drawing.Point(0, 72);
            this.pictureBox_SeriesPoster.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pictureBox_SeriesPoster.Name = "pictureBox_SeriesPoster";
            this.pictureBox_SeriesPoster.Size = new System.Drawing.Size(130, 197);
            this.pictureBox_SeriesPoster.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_SeriesPoster.TabIndex = 149;
            this.pictureBox_SeriesPoster.TabStop = false;
            // 
            // comboBox_PosterSelection
            // 
            this.comboBox_PosterSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_PosterSelection.FormattingEnabled = true;
            this.comboBox_PosterSelection.Location = new System.Drawing.Point(0, 45);
            this.comboBox_PosterSelection.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboBox_PosterSelection.Name = "comboBox_PosterSelection";
            this.comboBox_PosterSelection.Size = new System.Drawing.Size(128, 21);
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
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridView1.Location = new System.Drawing.Point(0, 271);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.dataGridView1.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView1.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.Size = new System.Drawing.Size(933, 629);
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
            // pictureBox_Series
            // 
            this.pictureBox_Series.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox_Series.BackColor = System.Drawing.Color.White;
            this.pictureBox_Series.ErrorImage = null;
            this.pictureBox_Series.InitialImage = null;
            this.pictureBox_Series.Location = new System.Drawing.Point(132, 72);
            this.pictureBox_Series.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pictureBox_Series.Name = "pictureBox_Series";
            this.pictureBox_Series.Size = new System.Drawing.Size(801, 197);
            this.pictureBox_Series.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox_Series.TabIndex = 147;
            this.pictureBox_Series.TabStop = false;
            // 
            // comboBox_BannerSelection
            // 
            this.comboBox_BannerSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_BannerSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_BannerSelection.FormattingEnabled = true;
            this.comboBox_BannerSelection.Location = new System.Drawing.Point(132, 45);
            this.comboBox_BannerSelection.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboBox_BannerSelection.Name = "comboBox_BannerSelection";
            this.comboBox_BannerSelection.Size = new System.Drawing.Size(801, 21);
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
            this.panDBLocation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panDBLocation.Name = "panDBLocation";
            this.panDBLocation.Size = new System.Drawing.Size(962, 35);
            this.panDBLocation.TabIndex = 0;
            // 
            // button_dbbrowse
            // 
            this.button_dbbrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_dbbrowse.Location = new System.Drawing.Point(875, 5);
            this.button_dbbrowse.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button_dbbrowse.Name = "button_dbbrowse";
            this.button_dbbrowse.Size = new System.Drawing.Size(29, 20);
            this.button_dbbrowse.TabIndex = 2;
            this.button_dbbrowse.Text = "...";
            this.button_dbbrowse.UseVisualStyleBackColor = true;
            this.button_dbbrowse.Click += new System.EventHandler(this.button_dbbrowse_Click);
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(4, 8);
            this.label28.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
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
            this.tabPage_Import.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage_Import.Name = "tabPage_Import";
            this.tabPage_Import.Size = new System.Drawing.Size(1195, 942);
            this.tabPage_Import.TabIndex = 4;
            this.tabPage_Import.Text = "Import";
            this.tabPage_Import.UseVisualStyleBackColor = true;
            // 
            // splitContainer_SettingsOutput
            // 
            this.splitContainer_SettingsOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_SettingsOutput.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_SettingsOutput.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer_SettingsOutput.Name = "splitContainer_SettingsOutput";
            this.splitContainer_SettingsOutput.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_SettingsOutput.Panel1
            // 
            this.splitContainer_SettingsOutput.Panel1.Controls.Add(this.splitContainerImportSettings);
            // 
            // splitContainer_SettingsOutput.Panel2
            // 
            this.splitContainer_SettingsOutput.Panel2.Controls.Add(this.buttonStartImport);
            this.splitContainer_SettingsOutput.Panel2MinSize = 30;
            this.splitContainer_SettingsOutput.Size = new System.Drawing.Size(1195, 942);
            this.splitContainer_SettingsOutput.SplitterDistance = 869;
            this.splitContainer_SettingsOutput.SplitterWidth = 6;
            this.splitContainer_SettingsOutput.TabIndex = 157;
            this.splitContainer_SettingsOutput.TabStop = false;
            // 
            // splitContainerImportSettings
            // 
            this.splitContainerImportSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerImportSettings.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerImportSettings.Location = new System.Drawing.Point(0, 0);
            this.splitContainerImportSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainerImportSettings.Name = "splitContainerImportSettings";
            // 
            // splitContainerImportSettings.Panel1
            // 
            this.splitContainerImportSettings.Panel1.Controls.Add(this.treeView_Settings);
            // 
            // splitContainerImportSettings.Panel2
            // 
            this.splitContainerImportSettings.Panel2.Controls.Add(this.panel_ImportPathes);
            this.splitContainerImportSettings.Panel2.Controls.Add(this.panel_Expressions);
            this.splitContainerImportSettings.Panel2.Controls.Add(this.panel_StringReplacements);
            this.splitContainerImportSettings.Size = new System.Drawing.Size(1195, 869);
            this.splitContainerImportSettings.SplitterDistance = 151;
            this.splitContainerImportSettings.SplitterWidth = 6;
            this.splitContainerImportSettings.TabIndex = 156;
            // 
            // treeView_Settings
            // 
            this.treeView_Settings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_Settings.Location = new System.Drawing.Point(0, 0);
            this.treeView_Settings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.treeView_Settings.Name = "treeView_Settings";
            this.treeView_Settings.Size = new System.Drawing.Size(151, 869);
            this.treeView_Settings.TabIndex = 0;
            this.treeView_Settings.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_Settings_AfterSelect);
            // 
            // panel_ImportPathes
            // 
            this.panel_ImportPathes.Controls.Add(this.dbOptionParsedNameFromFolder);
            this.panel_ImportPathes.Controls.Add(this.label68);
            this.panel_ImportPathes.Controls.Add(this.button_TestReparse);
            this.panel_ImportPathes.Controls.Add(this.dataGridView_ImportPathes);
            this.panel_ImportPathes.Controls.Add(this.listView_ParsingResults);
            this.panel_ImportPathes.Location = new System.Drawing.Point(85, 9);
            this.panel_ImportPathes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel_ImportPathes.Name = "panel_ImportPathes";
            this.panel_ImportPathes.Size = new System.Drawing.Size(931, 578);
            this.panel_ImportPathes.TabIndex = 154;
            this.panel_ImportPathes.Tag = "Import Paths";
            // 
            // label68
            // 
            this.label68.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label68.Location = new System.Drawing.Point(2, 180);
            this.label68.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label68.Name = "label68";
            this.label68.Size = new System.Drawing.Size(871, 34);
            this.label68.TabIndex = 151;
            this.label68.Text = resources.GetString("label68.Text");
            // 
            // dataGridView_ImportPathes
            // 
            this.dataGridView_ImportPathes.AllowUserToResizeColumns = false;
            this.dataGridView_ImportPathes.AllowUserToResizeRows = false;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_ImportPathes.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle9;
            this.dataGridView_ImportPathes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_ImportPathes.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView_ImportPathes.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView_ImportPathes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_ImportPathes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.dataGridView_ImportPathes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_ImportPathes.DefaultCellStyle = dataGridViewCellStyle11;
            this.dataGridView_ImportPathes.Location = new System.Drawing.Point(4, 6);
            this.dataGridView_ImportPathes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dataGridView_ImportPathes.MultiSelect = false;
            this.dataGridView_ImportPathes.Name = "dataGridView_ImportPathes";
            this.dataGridView_ImportPathes.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_ImportPathes.RowHeadersDefaultCellStyle = dataGridViewCellStyle12;
            this.dataGridView_ImportPathes.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_ImportPathes.RowsDefaultCellStyle = dataGridViewCellStyle13;
            this.dataGridView_ImportPathes.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_ImportPathes.RowTemplate.Height = 18;
            this.dataGridView_ImportPathes.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_ImportPathes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView_ImportPathes.Size = new System.Drawing.Size(868, 144);
            this.dataGridView_ImportPathes.StandardTab = true;
            this.dataGridView_ImportPathes.TabIndex = 150;
            this.dataGridView_ImportPathes.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_ImportPathes_CellContentClick);
            this.dataGridView_ImportPathes.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_ImportPathes_CellValueChanged);
            this.dataGridView_ImportPathes.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataGridView_ImportPathes_RowsRemoved);
            this.dataGridView_ImportPathes.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView_ImportPathes_UserDeletedRow);
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
            this.listView_ParsingResults.Location = new System.Drawing.Point(4, 219);
            this.listView_ParsingResults.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listView_ParsingResults.MultiSelect = false;
            this.listView_ParsingResults.Name = "listView_ParsingResults";
            this.listView_ParsingResults.Size = new System.Drawing.Size(869, 354);
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
            this.panel_Expressions.Location = new System.Drawing.Point(9, 101);
            this.panel_Expressions.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel_Expressions.Name = "panel_Expressions";
            this.panel_Expressions.Size = new System.Drawing.Size(901, 371);
            this.panel_Expressions.TabIndex = 155;
            this.panel_Expressions.Tag = "Parsing Expressions";
            // 
            // linkExpressionHelp
            // 
            this.linkExpressionHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkExpressionHelp.AutoSize = true;
            this.linkExpressionHelp.Location = new System.Drawing.Point(849, 224);
            this.linkExpressionHelp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
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
            this.label70.Location = new System.Drawing.Point(8, 6);
            this.label70.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label70.Name = "label70";
            this.label70.Size = new System.Drawing.Size(856, 52);
            this.label70.TabIndex = 5;
            this.label70.Text = resources.GetString("label70.Text");
            // 
            // dataGridView_Expressions
            // 
            this.dataGridView_Expressions.AllowUserToResizeColumns = false;
            this.dataGridView_Expressions.AllowUserToResizeRows = false;
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_Expressions.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle14;
            this.dataGridView_Expressions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_Expressions.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView_Expressions.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView_Expressions.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Expressions.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle15;
            this.dataGridView_Expressions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle16.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle16.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle16.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle16.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle16.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Expressions.DefaultCellStyle = dataGridViewCellStyle16;
            this.dataGridView_Expressions.Location = new System.Drawing.Point(11, 63);
            this.dataGridView_Expressions.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dataGridView_Expressions.MultiSelect = false;
            this.dataGridView_Expressions.Name = "dataGridView_Expressions";
            this.dataGridView_Expressions.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle17.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle17.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle17.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle17.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Expressions.RowHeadersDefaultCellStyle = dataGridViewCellStyle17;
            this.dataGridView_Expressions.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_Expressions.RowsDefaultCellStyle = dataGridViewCellStyle18;
            this.dataGridView_Expressions.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_Expressions.RowTemplate.Height = 18;
            this.dataGridView_Expressions.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Expressions.Size = new System.Drawing.Size(830, 303);
            this.dataGridView_Expressions.StandardTab = true;
            this.dataGridView_Expressions.TabIndex = 0;
            this.dataGridView_Expressions.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_Expressions_CellEndEdit);
            this.dataGridView_Expressions.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView_Expressions_UserDeletedRow);
            // 
            // button_MoveExpUp
            // 
            this.button_MoveExpUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_MoveExpUp.Image = ((System.Drawing.Image)(resources.GetObject("button_MoveExpUp.Image")));
            this.button_MoveExpUp.Location = new System.Drawing.Point(852, 67);
            this.button_MoveExpUp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button_MoveExpUp.Name = "button_MoveExpUp";
            this.button_MoveExpUp.Size = new System.Drawing.Size(32, 27);
            this.button_MoveExpUp.TabIndex = 1;
            this.button_MoveExpUp.UseVisualStyleBackColor = true;
            this.button_MoveExpUp.Click += new System.EventHandler(this.button_MoveExpUp_Click);
            // 
            // button_MoveExpDown
            // 
            this.button_MoveExpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_MoveExpDown.Image = ((System.Drawing.Image)(resources.GetObject("button_MoveExpDown.Image")));
            this.button_MoveExpDown.Location = new System.Drawing.Point(852, 104);
            this.button_MoveExpDown.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button_MoveExpDown.Name = "button_MoveExpDown";
            this.button_MoveExpDown.Size = new System.Drawing.Size(32, 27);
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
            this.panel_StringReplacements.Location = new System.Drawing.Point(9, 602);
            this.panel_StringReplacements.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel_StringReplacements.Name = "panel_StringReplacements";
            this.panel_StringReplacements.Size = new System.Drawing.Size(1007, 234);
            this.panel_StringReplacements.TabIndex = 155;
            this.panel_StringReplacements.Tag = "String Replacements";
            // 
            // linkLabelResetStringReplacements
            // 
            this.linkLabelResetStringReplacements.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelResetStringReplacements.AutoSize = true;
            this.linkLabelResetStringReplacements.Location = new System.Drawing.Point(954, 114);
            this.linkLabelResetStringReplacements.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
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
            this.linkLabelImportStringReplacements.Location = new System.Drawing.Point(954, 76);
            this.linkLabelImportStringReplacements.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
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
            this.linkLabelExportStringReplacements.Location = new System.Drawing.Point(954, 95);
            this.linkLabelExportStringReplacements.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
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
            this.label69.Location = new System.Drawing.Point(6, 8);
            this.label69.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label69.Name = "label69";
            this.label69.Size = new System.Drawing.Size(961, 52);
            this.label69.TabIndex = 1;
            this.label69.Text = resources.GetString("label69.Text");
            // 
            // tabOnlineData
            // 
            this.tabOnlineData.Controls.Add(this.groupBox12);
            this.tabOnlineData.ImageIndex = 8;
            this.tabOnlineData.Location = new System.Drawing.Point(4, 31);
            this.tabOnlineData.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabOnlineData.Name = "tabOnlineData";
            this.tabOnlineData.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabOnlineData.Size = new System.Drawing.Size(1195, 942);
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
            this.groupBox12.Location = new System.Drawing.Point(4, 5);
            this.groupBox12.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox12.Size = new System.Drawing.Size(1181, 914);
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
            this.panel_OnlineData.Controls.Add(this.dbCheckBoxTraktCommunityRatings);
            this.panel_OnlineData.Controls.Add(this.dbOptCheckBoxRemoveEpZero);
            this.panel_OnlineData.Controls.Add(this.dbOptCheckBoxCleanOnlineEpisodes);
            this.panel_OnlineData.Controls.Add(this.dbOptCheckBoxDownloadActors);
            this.panel_OnlineData.Controls.Add(this.dbOptionCheckBox4);
            this.panel_OnlineData.Controls.Add(this.label11);
            this.panel_OnlineData.Controls.Add(this.nudConsecFailures);
            this.panel_OnlineData.Controls.Add(this.label20);
            this.panel_OnlineData.Controls.Add(this.buttonArtworkDownloadLimits);
            this.panel_OnlineData.Controls.Add(this.dbOptCheckBoxGetTextBanners);
            this.panel_OnlineData.Controls.Add(this.label18);
            this.panel_OnlineData.Controls.Add(this.groupBox5);
            this.panel_OnlineData.Controls.Add(this.lblImportDelaySecs);
            this.panel_OnlineData.Controls.Add(this.numericUpDownImportDelay);
            this.panel_OnlineData.Controls.Add(this.lblImportDelayCaption);
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
            this.panel_OnlineData.Controls.Add(this.checkBox_OverrideComboLang);
            this.panel_OnlineData.Controls.Add(this.checkBox_OnlineSearch);
            this.panel_OnlineData.Controls.Add(this.checkBox_AutoChooseSeries);
            this.panel_OnlineData.Controls.Add(this.checkBox_FullSeriesRetrieval);
            this.panel_OnlineData.Location = new System.Drawing.Point(10, 23);
            this.panel_OnlineData.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel_OnlineData.Name = "panel_OnlineData";
            this.panel_OnlineData.Size = new System.Drawing.Size(1163, 876);
            this.panel_OnlineData.TabIndex = 0;
            this.panel_OnlineData.Tag = "Online Data";
            // 
            // dbOptCheckBoxRemoveEpZero
            // 
            this.dbOptCheckBoxRemoveEpZero.AutoSize = true;
            this.dbOptCheckBoxRemoveEpZero.Location = new System.Drawing.Point(39, 346);
            this.dbOptCheckBoxRemoveEpZero.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dbOptCheckBoxRemoveEpZero.Name = "dbOptCheckBoxRemoveEpZero";
            this.dbOptCheckBoxRemoveEpZero.Option = "";
            this.dbOptCheckBoxRemoveEpZero.Size = new System.Drawing.Size(236, 17);
            this.dbOptCheckBoxRemoveEpZero.TabIndex = 23;
            this.dbOptCheckBoxRemoveEpZero.Text = "Remove Episode \'0\' even when exists online";
            this.dbOptCheckBoxRemoveEpZero.ToolTip = "";
            this.dbOptCheckBoxRemoveEpZero.UseVisualStyleBackColor = true;
            // 
            // dbOptCheckBoxDownloadActors
            // 
            this.dbOptCheckBoxDownloadActors.AutoSize = true;
            this.dbOptCheckBoxDownloadActors.Checked = true;
            this.dbOptCheckBoxDownloadActors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dbOptCheckBoxDownloadActors.Location = new System.Drawing.Point(448, 528);
            this.dbOptCheckBoxDownloadActors.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dbOptCheckBoxDownloadActors.Name = "dbOptCheckBoxDownloadActors";
            this.dbOptCheckBoxDownloadActors.Option = "AutoDownloadActors";
            this.dbOptCheckBoxDownloadActors.Size = new System.Drawing.Size(271, 17);
            this.dbOptCheckBoxDownloadActors.TabIndex = 43;
            this.dbOptCheckBoxDownloadActors.Text = "Automatically download actor thumbnails and details";
            this.dbOptCheckBoxDownloadActors.ToolTip = "";
            this.dbOptCheckBoxDownloadActors.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(213, 570);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(43, 13);
            this.label11.TabIndex = 34;
            this.label11.Text = "failures.";
            // 
            // nudConsecFailures
            // 
            this.nudConsecFailures.Location = new System.Drawing.Point(143, 568);
            this.nudConsecFailures.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.nudConsecFailures.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudConsecFailures.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudConsecFailures.Name = "nudConsecFailures";
            this.nudConsecFailures.Size = new System.Drawing.Size(57, 20);
            this.nudConsecFailures.TabIndex = 33;
            this.nudConsecFailures.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudConsecFailures.ValueChanged += new System.EventHandler(this.nudConsecFailures_ValueChanged);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(9, 570);
            this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(121, 13);
            this.label20.TabIndex = 32;
            this.label20.Text = "Cancel downloads after ";
            // 
            // buttonArtworkDownloadLimits
            // 
            this.buttonArtworkDownloadLimits.Location = new System.Drawing.Point(39, 500);
            this.buttonArtworkDownloadLimits.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonArtworkDownloadLimits.Name = "buttonArtworkDownloadLimits";
            this.buttonArtworkDownloadLimits.Size = new System.Drawing.Size(225, 30);
            this.buttonArtworkDownloadLimits.TabIndex = 30;
            this.buttonArtworkDownloadLimits.Text = "Configure Artwork Download Limits...";
            this.buttonArtworkDownloadLimits.UseVisualStyleBackColor = true;
            this.buttonArtworkDownloadLimits.Click += new System.EventHandler(this.buttonArtworkDownloadLimits_Click);
            // 
            // dbOptCheckBoxGetTextBanners
            // 
            this.dbOptCheckBoxGetTextBanners.AutoSize = true;
            this.dbOptCheckBoxGetTextBanners.Location = new System.Drawing.Point(12, 449);
            this.dbOptCheckBoxGetTextBanners.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dbOptCheckBoxGetTextBanners.Name = "dbOptCheckBoxGetTextBanners";
            this.dbOptCheckBoxGetTextBanners.Option = "GetTextBanners";
            this.dbOptCheckBoxGetTextBanners.Size = new System.Drawing.Size(282, 17);
            this.dbOptCheckBoxGetTextBanners.TabIndex = 28;
            this.dbOptCheckBoxGetTextBanners.Text = "Download Series WideBanners containing \'Text\' name";
            this.dbOptCheckBoxGetTextBanners.ToolTip = "Enable to download series widebanners that do not contain a graphical series name" +
    ".";
            this.dbOptCheckBoxGetTextBanners.UseVisualStyleBackColor = true;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(9, 370);
            this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(50, 13);
            this.label18.TabIndex = 24;
            this.label18.Text = "Artwork";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox5
            // 
            this.groupBox5.Location = new System.Drawing.Point(67, 376);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox5.Size = new System.Drawing.Size(650, 3);
            this.groupBox5.TabIndex = 25;
            this.groupBox5.TabStop = false;
            // 
            // lblImportDelaySecs
            // 
            this.lblImportDelaySecs.AutoSize = true;
            this.lblImportDelaySecs.Location = new System.Drawing.Point(253, 130);
            this.lblImportDelaySecs.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblImportDelaySecs.Name = "lblImportDelaySecs";
            this.lblImportDelaySecs.Size = new System.Drawing.Size(29, 13);
            this.lblImportDelaySecs.TabIndex = 12;
            this.lblImportDelaySecs.Text = "secs";
            // 
            // lblImportDelayCaption
            // 
            this.lblImportDelayCaption.AutoSize = true;
            this.lblImportDelayCaption.Location = new System.Drawing.Point(45, 129);
            this.lblImportDelayCaption.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblImportDelayCaption.Name = "lblImportDelayCaption";
            this.lblImportDelayCaption.Size = new System.Drawing.Size(110, 13);
            this.lblImportDelayCaption.TabIndex = 10;
            this.lblImportDelayCaption.Text = "Delay Initial Import by:";
            // 
            // checkBox_AutoUpdateAllFanart
            // 
            this.checkBox_AutoUpdateAllFanart.AutoSize = true;
            this.checkBox_AutoUpdateAllFanart.Location = new System.Drawing.Point(448, 474);
            this.checkBox_AutoUpdateAllFanart.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox_AutoUpdateAllFanart.Name = "checkBox_AutoUpdateAllFanart";
            this.checkBox_AutoUpdateAllFanart.Size = new System.Drawing.Size(192, 17);
            this.checkBox_AutoUpdateAllFanart.TabIndex = 41;
            this.checkBox_AutoUpdateAllFanart.Text = "Update all existing Fanart on Import";
            this.checkBox_AutoUpdateAllFanart.UseVisualStyleBackColor = true;
            this.checkBox_AutoUpdateAllFanart.CheckedChanged += new System.EventHandler(this.checkBox_AutoUpdateAllFanart_CheckedChanged);
            // 
            // label73
            // 
            this.label73.Location = new System.Drawing.Point(629, 446);
            this.label73.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label73.Name = "label73";
            this.label73.Size = new System.Drawing.Size(76, 23);
            this.label73.TabIndex = 40;
            this.label73.Text = "fanarts";
            this.label73.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // spinMaxFanarts
            // 
            this.spinMaxFanarts.Location = new System.Drawing.Point(567, 449);
            this.spinMaxFanarts.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.spinMaxFanarts.Size = new System.Drawing.Size(54, 20);
            this.spinMaxFanarts.TabIndex = 39;
            this.spinMaxFanarts.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.spinMaxFanarts.ValueChanged += new System.EventHandler(this.spinMaxFanarts_ValueChanged);
            // 
            // label72
            // 
            this.label72.Location = new System.Drawing.Point(468, 446);
            this.label72.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label72.Name = "label72";
            this.label72.Size = new System.Drawing.Size(70, 21);
            this.label72.TabIndex = 38;
            this.label72.Text = "Retrieve:";
            this.label72.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label71
            // 
            this.label71.Location = new System.Drawing.Point(468, 417);
            this.label71.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label71.Name = "label71";
            this.label71.Size = new System.Drawing.Size(77, 22);
            this.label71.TabIndex = 36;
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
            this.cboFanartResolution.Location = new System.Drawing.Point(567, 418);
            this.cboFanartResolution.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cboFanartResolution.Name = "cboFanartResolution";
            this.cboFanartResolution.Size = new System.Drawing.Size(103, 21);
            this.cboFanartResolution.TabIndex = 37;
            this.cboFanartResolution.SelectedIndexChanged += new System.EventHandler(this.cboFanartResolution_SelectedIndexChanged);
            // 
            // chkAutoDownloadFanart
            // 
            this.chkAutoDownloadFanart.AutoSize = true;
            this.chkAutoDownloadFanart.Location = new System.Drawing.Point(448, 395);
            this.chkAutoDownloadFanart.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkAutoDownloadFanart.Name = "chkAutoDownloadFanart";
            this.chkAutoDownloadFanart.Size = new System.Drawing.Size(202, 17);
            this.chkAutoDownloadFanart.TabIndex = 35;
            this.chkAutoDownloadFanart.Text = "Automatically download Series &Fanart";
            this.chkAutoDownloadFanart.UseVisualStyleBackColor = true;
            this.chkAutoDownloadFanart.CheckedChanged += new System.EventHandler(this.chkAutoDownloadFanart_CheckedChanged);
            // 
            // linkAccountID
            // 
            this.linkAccountID.AutoSize = true;
            this.linkAccountID.Location = new System.Drawing.Point(319, 56);
            this.linkAccountID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkAccountID.Name = "linkAccountID";
            this.linkAccountID.Size = new System.Drawing.Size(90, 13);
            this.linkAccountID.TabIndex = 7;
            this.linkAccountID.TabStop = true;
            this.linkAccountID.Text = "Account Identifier";
            this.linkAccountID.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkAccountID_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(442, 158);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "&hour(s)";
            // 
            // numericUpDown_AutoOnlineDataRefresh
            // 
            this.numericUpDown_AutoOnlineDataRefresh.Location = new System.Drawing.Point(356, 154);
            this.numericUpDown_AutoOnlineDataRefresh.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDown_AutoOnlineDataRefresh.Maximum = new decimal(new int[] {
            168,
            0,
            0,
            0});
            this.numericUpDown_AutoOnlineDataRefresh.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_AutoOnlineDataRefresh.Name = "numericUpDown_AutoOnlineDataRefresh";
            this.numericUpDown_AutoOnlineDataRefresh.Size = new System.Drawing.Size(75, 20);
            this.numericUpDown_AutoOnlineDataRefresh.TabIndex = 14;
            this.numericUpDown_AutoOnlineDataRefresh.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDown_AutoOnlineDataRefresh.ValueChanged += new System.EventHandler(this.numericUpDown_AutoOnlineDataRefresh_ValueChanged);
            // 
            // label54
            // 
            this.label54.Location = new System.Drawing.Point(45, 53);
            this.label54.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(78, 20);
            this.label54.TabIndex = 3;
            this.label54.Text = "&Account ID:";
            this.label54.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label29
            // 
            this.label29.Location = new System.Drawing.Point(45, 27);
            this.label29.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(78, 20);
            this.label29.TabIndex = 1;
            this.label29.Text = "&Main Mirror:";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label26
            // 
            this.label26.Location = new System.Drawing.Point(45, 77);
            this.label26.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(78, 20);
            this.label26.TabIndex = 5;
            this.label26.Text = "&Language:";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabPage_MP_DisplayControl
            // 
            this.tabPage_MP_DisplayControl.AutoScroll = true;
            this.tabPage_MP_DisplayControl.AutoScrollMinSize = new System.Drawing.Size(0, 478);
            this.tabPage_MP_DisplayControl.Controls.Add(this.groupBox1);
            this.tabPage_MP_DisplayControl.ImageIndex = 9;
            this.tabPage_MP_DisplayControl.Location = new System.Drawing.Point(4, 31);
            this.tabPage_MP_DisplayControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage_MP_DisplayControl.Name = "tabPage_MP_DisplayControl";
            this.tabPage_MP_DisplayControl.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage_MP_DisplayControl.Size = new System.Drawing.Size(1195, 942);
            this.tabPage_MP_DisplayControl.TabIndex = 5;
            this.tabPage_MP_DisplayControl.Text = "General";
            this.tabPage_MP_DisplayControl.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.dbOptionDisableMediaInfoInConfig);
            this.groupBox1.Controls.Add(this.dbOptChkBoxPlayOutOfOrderCheck);
            this.groupBox1.Controls.Add(this.dbOptionSQLLogging);
            this.groupBox1.Controls.Add(this.dbOptChkBox_SubCentral_DownloadSubtitlesOnPlay);
            this.groupBox1.Controls.Add(this.checkBox_CountSpecialEpisodesAsWatched);
            this.groupBox1.Controls.Add(this.dbOptionCheckBox3);
            this.groupBox1.Controls.Add(this.lblRecentAddedDays);
            this.groupBox1.Controls.Add(this.nudRecentlyAddedDays);
            this.groupBox1.Controls.Add(this.cbNewEpisodeThumbIndicator);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.laOnPlaySeriesOrSeasonAction);
            this.groupBox1.Controls.Add(this.cbOnPlaySeriesOrSeasonAction);
            this.groupBox1.Controls.Add(this.dbOptChkBoxCountEmptyFutureEps);
            this.groupBox1.Controls.Add(this.dbOptChkBoxScanFullscreenVideo);
            this.groupBox1.Controls.Add(this.dbOptionCheckBox2);
            this.groupBox1.Controls.Add(this.dbOptionCheckBoxSubstituteMissingArtwork);
            this.groupBox1.Controls.Add(this.nudScanRemoteShareFrequency);
            this.groupBox1.Controls.Add(this.checkBox_scanRemoteShares);
            this.groupBox1.Controls.Add(this.dbOptionCheckBoxMarkRatedEpsAsWatched);
            this.groupBox1.Controls.Add(this.checkboxRatingDisplayStars);
            this.groupBox1.Controls.Add(this.label77);
            this.groupBox1.Controls.Add(this.label76);
            this.groupBox1.Controls.Add(this.label75);
            this.groupBox1.Controls.Add(this.numericUpDownArtworkDelay);
            this.groupBox1.Controls.Add(this.label74);
            this.groupBox1.Controls.Add(this.label27);
            this.groupBox1.Controls.Add(this.numericUpDownBackdropDelay);
            this.groupBox1.Controls.Add(this.checkbox_SortSpecials);
            this.groupBox1.Controls.Add(this.linkExWatched);
            this.groupBox1.Controls.Add(this.linkImpWatched);
            this.groupBox1.Controls.Add(this.chkAllowDeletes);
            this.groupBox1.Controls.Add(this.checkBox_doFolderWatch);
            this.groupBox1.Controls.Add(this.checkBox_Episode_HideUnwatchedSummary);
            this.groupBox1.Controls.Add(this.checkBox_Episode_HideUnwatchedThumbnail);
            this.groupBox1.Controls.Add(this.checkBox_Episode_OnlyShowLocalFiles);
            this.groupBox1.Controls.Add(this.checkBox_Series_UseSortName);
            this.groupBox1.Controls.Add(this.optionAsk2Rate);
            this.groupBox1.Controls.Add(this.chkUseRegionalDateFormatString);
            this.groupBox1.Controls.Add(this.label39);
            this.groupBox1.Controls.Add(this.label38);
            this.groupBox1.Controls.Add(this.nudWatchedAfter);
            this.groupBox1.Controls.Add(this.checkBox_RandBanner);
            this.groupBox1.Controls.Add(this.textBox_PluginHomeName);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Location = new System.Drawing.Point(9, 9);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(1173, 911);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General Settings";
            // 
            // dbOptionCheckBox3
            // 
            this.dbOptionCheckBox3.AutoSize = true;
            this.dbOptionCheckBox3.Checked = true;
            this.dbOptionCheckBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dbOptionCheckBox3.Location = new System.Drawing.Point(467, 117);
            this.dbOptionCheckBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dbOptionCheckBox3.Name = "dbOptionCheckBox3";
            this.dbOptionCheckBox3.Option = "SortSpecialSeasonLast";
            this.dbOptionCheckBox3.Size = new System.Drawing.Size(188, 17);
            this.dbOptionCheckBox3.TabIndex = 32;
            this.dbOptionCheckBox3.Text = "Sort Specials at end of season list.";
            this.dbOptionCheckBox3.ToolTip = "Enable this option to sort specials at end of season list rather than first item." +
    "";
            this.dbOptionCheckBox3.UseVisualStyleBackColor = true;
            // 
            // lblRecentAddedDays
            // 
            this.lblRecentAddedDays.AutoSize = true;
            this.lblRecentAddedDays.Location = new System.Drawing.Point(349, 441);
            this.lblRecentAddedDays.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRecentAddedDays.Name = "lblRecentAddedDays";
            this.lblRecentAddedDays.Size = new System.Drawing.Size(31, 13);
            this.lblRecentAddedDays.TabIndex = 27;
            this.lblRecentAddedDays.Text = "Days";
            // 
            // cbNewEpisodeThumbIndicator
            // 
            this.cbNewEpisodeThumbIndicator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbNewEpisodeThumbIndicator.FormattingEnabled = true;
            this.cbNewEpisodeThumbIndicator.Items.AddRange(new object[] {
            "None",
            "Unwatched Episodes",
            "Recently Added Episodes",
            "Recently Added Unwatched Episodes"});
            this.cbNewEpisodeThumbIndicator.Location = new System.Drawing.Point(8, 437);
            this.cbNewEpisodeThumbIndicator.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbNewEpisodeThumbIndicator.Name = "cbNewEpisodeThumbIndicator";
            this.cbNewEpisodeThumbIndicator.Size = new System.Drawing.Size(263, 21);
            this.cbNewEpisodeThumbIndicator.TabIndex = 25;
            this.cbNewEpisodeThumbIndicator.SelectedIndexChanged += new System.EventHandler(this.cbNewEpisodeThumbIndicator_SelectedIndexChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(8, 420);
            this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(117, 13);
            this.label17.TabIndex = 24;
            this.label17.Text = "New Episode Indicator:";
            // 
            // laOnPlaySeriesOrSeasonAction
            // 
            this.laOnPlaySeriesOrSeasonAction.AutoSize = true;
            this.laOnPlaySeriesOrSeasonAction.Location = new System.Drawing.Point(8, 371);
            this.laOnPlaySeriesOrSeasonAction.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.laOnPlaySeriesOrSeasonAction.Name = "laOnPlaySeriesOrSeasonAction";
            this.laOnPlaySeriesOrSeasonAction.Size = new System.Drawing.Size(197, 13);
            this.laOnPlaySeriesOrSeasonAction.TabIndex = 22;
            this.laOnPlaySeriesOrSeasonAction.Text = "On play action on series or season view:";
            this.laOnPlaySeriesOrSeasonAction.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbOnPlaySeriesOrSeasonAction
            // 
            this.cbOnPlaySeriesOrSeasonAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOnPlaySeriesOrSeasonAction.FormattingEnabled = true;
            this.cbOnPlaySeriesOrSeasonAction.Items.AddRange(new object[] {
            "Do nothing",
            "Play random episode",
            "Play first unwatched episode",
            "Play first unwatched episode otherwise random",
            "Play random unwatched episode",
            "Play latest episode",
            "Always ask"});
            this.cbOnPlaySeriesOrSeasonAction.Location = new System.Drawing.Point(8, 389);
            this.cbOnPlaySeriesOrSeasonAction.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbOnPlaySeriesOrSeasonAction.Name = "cbOnPlaySeriesOrSeasonAction";
            this.cbOnPlaySeriesOrSeasonAction.Size = new System.Drawing.Size(263, 21);
            this.cbOnPlaySeriesOrSeasonAction.TabIndex = 23;
            this.cbOnPlaySeriesOrSeasonAction.SelectedIndexChanged += new System.EventHandler(this.cbOnPlaySeriesOrSeasonAction_SelectedIndexChanged);
            // 
            // dbOptChkBoxScanFullscreenVideo
            // 
            this.dbOptChkBoxScanFullscreenVideo.AutoSize = true;
            this.dbOptChkBoxScanFullscreenVideo.Location = new System.Drawing.Point(40, 342);
            this.dbOptChkBoxScanFullscreenVideo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dbOptChkBoxScanFullscreenVideo.Name = "dbOptChkBoxScanFullscreenVideo";
            this.dbOptChkBoxScanFullscreenVideo.Option = "AutoScanLocalFilesFSV";
            this.dbOptChkBoxScanFullscreenVideo.Size = new System.Drawing.Size(266, 17);
            this.dbOptChkBoxScanFullscreenVideo.TabIndex = 21;
            this.dbOptChkBoxScanFullscreenVideo.Text = "Scan remote shares while fullscreen video is active";
            this.dbOptChkBoxScanFullscreenVideo.ToolTip = "";
            this.dbOptChkBoxScanFullscreenVideo.UseVisualStyleBackColor = true;
            // 
            // dbOptionCheckBox2
            // 
            this.dbOptionCheckBox2.AutoSize = true;
            this.dbOptionCheckBox2.Checked = true;
            this.dbOptionCheckBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dbOptionCheckBox2.Location = new System.Drawing.Point(467, 225);
            this.dbOptionCheckBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dbOptionCheckBox2.Name = "dbOptionCheckBox2";
            this.dbOptionCheckBox2.Option = "SkipSeasonViewOnSingleSeason";
            this.dbOptionCheckBox2.Size = new System.Drawing.Size(234, 17);
            this.dbOptionCheckBox2.TabIndex = 36;
            this.dbOptionCheckBox2.Text = "Skip season view if there is only one season";
            this.dbOptionCheckBox2.ToolTip = "";
            this.dbOptionCheckBox2.UseVisualStyleBackColor = true;
            // 
            // dbOptionCheckBoxSubstituteMissingArtwork
            // 
            this.dbOptionCheckBoxSubstituteMissingArtwork.AutoSize = true;
            this.dbOptionCheckBoxSubstituteMissingArtwork.Location = new System.Drawing.Point(467, 279);
            this.dbOptionCheckBoxSubstituteMissingArtwork.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dbOptionCheckBoxSubstituteMissingArtwork.Name = "dbOptionCheckBoxSubstituteMissingArtwork";
            this.dbOptionCheckBoxSubstituteMissingArtwork.Option = "SubstituteMissingArtwork";
            this.dbOptionCheckBoxSubstituteMissingArtwork.Size = new System.Drawing.Size(280, 17);
            this.dbOptionCheckBoxSubstituteMissingArtwork.TabIndex = 38;
            this.dbOptionCheckBoxSubstituteMissingArtwork.Text = "Substitute Missing Season Posters with Series Posters";
            this.dbOptionCheckBoxSubstituteMissingArtwork.ToolTip = "";
            this.dbOptionCheckBoxSubstituteMissingArtwork.UseVisualStyleBackColor = true;
            // 
            // label77
            // 
            this.label77.AutoSize = true;
            this.label77.Location = new System.Drawing.Point(279, 314);
            this.label77.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label77.Name = "label77";
            this.label77.Size = new System.Drawing.Size(43, 13);
            this.label77.TabIndex = 20;
            this.label77.Text = "minutes";
            // 
            // label76
            // 
            this.label76.AutoSize = true;
            this.label76.Location = new System.Drawing.Point(245, 94);
            this.label76.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label76.Name = "label76";
            this.label76.Size = new System.Drawing.Size(20, 13);
            this.label76.TabIndex = 10;
            this.label76.Text = "ms";
            // 
            // label75
            // 
            this.label75.AutoSize = true;
            this.label75.Location = new System.Drawing.Point(245, 69);
            this.label75.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label75.Name = "label75";
            this.label75.Size = new System.Drawing.Size(20, 13);
            this.label75.TabIndex = 7;
            this.label75.Text = "ms";
            // 
            // numericUpDownArtworkDelay
            // 
            this.numericUpDownArtworkDelay.Location = new System.Drawing.Point(171, 92);
            this.numericUpDownArtworkDelay.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownArtworkDelay.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericUpDownArtworkDelay.Name = "numericUpDownArtworkDelay";
            this.numericUpDownArtworkDelay.Size = new System.Drawing.Size(66, 20);
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
            this.label74.Location = new System.Drawing.Point(8, 101);
            this.label74.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label74.Name = "label74";
            this.label74.Size = new System.Drawing.Size(117, 13);
            this.label74.TabIndex = 8;
            this.label74.Text = "Artwork Loading Delay:";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(8, 75);
            this.label27.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(127, 13);
            this.label27.TabIndex = 5;
            this.label27.Text = "Backdrop Loading Delay:";
            // 
            // numericUpDownBackdropDelay
            // 
            this.numericUpDownBackdropDelay.Location = new System.Drawing.Point(171, 67);
            this.numericUpDownBackdropDelay.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownBackdropDelay.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericUpDownBackdropDelay.Name = "numericUpDownBackdropDelay";
            this.numericUpDownBackdropDelay.Size = new System.Drawing.Size(66, 20);
            this.numericUpDownBackdropDelay.TabIndex = 6;
            this.numericUpDownBackdropDelay.Value = new decimal(new int[] {
            250,
            0,
            0,
            0});
            this.numericUpDownBackdropDelay.ValueChanged += new System.EventHandler(this.numericUpDownBackdropDelay_ValueChanged);
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(245, 44);
            this.label39.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(15, 13);
            this.label39.TabIndex = 4;
            this.label39.Text = "%";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(8, 49);
            this.label38.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(141, 13);
            this.label38.TabIndex = 2;
            this.label38.Text = "An episode is &watched after:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(8, 20);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
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
            this.tab_view.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tab_view.Name = "tab_view";
            this.tab_view.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tab_view.Size = new System.Drawing.Size(1195, 942);
            this.tab_view.TabIndex = 7;
            this.tab_view.Text = "Views/Filters";
            this.tab_view.UseVisualStyleBackColor = true;
            // 
            // playlistSettings
            // 
            this.playlistSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.playlistSettings.AutoScroll = true;
            this.playlistSettings.Location = new System.Drawing.Point(0, 392);
            this.playlistSettings.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.playlistSettings.Name = "playlistSettings";
            this.playlistSettings.Size = new System.Drawing.Size(1180, 541);
            this.playlistSettings.TabIndex = 1;
            // 
            // groupBox8
            // 
            this.groupBox8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox8.Controls.Add(this.dtpParentalBefore);
            this.groupBox8.Controls.Add(this.label22);
            this.groupBox8.Controls.Add(this.dtpParentalAfter);
            this.groupBox8.Controls.Add(this.label21);
            this.groupBox8.Controls.Add(this.nudParentalControlTimeout);
            this.groupBox8.Controls.Add(this.label19);
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
            this.groupBox8.Location = new System.Drawing.Point(4, 9);
            this.groupBox8.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox8.Size = new System.Drawing.Size(1174, 379);
            this.groupBox8.TabIndex = 0;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Customize Views";
            // 
            // dtpParentalBefore
            // 
            this.dtpParentalBefore.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpParentalBefore.Location = new System.Drawing.Point(559, 334);
            this.dtpParentalBefore.Name = "dtpParentalBefore";
            this.dtpParentalBefore.ShowUpDown = true;
            this.dtpParentalBefore.Size = new System.Drawing.Size(104, 20);
            this.dtpParentalBefore.TabIndex = 15;
            this.dtpParentalBefore.Value = new System.DateTime(2018, 2, 25, 5, 0, 0, 0);
            this.dtpParentalBefore.ValueChanged += new System.EventHandler(this.dtpParentalBefore_ValueChanged);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(470, 334);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(61, 13);
            this.label22.TabIndex = 14;
            this.label22.Text = "and before:";
            // 
            // dtpParentalAfter
            // 
            this.dtpParentalAfter.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpParentalAfter.Location = new System.Drawing.Point(559, 308);
            this.dtpParentalAfter.Name = "dtpParentalAfter";
            this.dtpParentalAfter.ShowUpDown = true;
            this.dtpParentalAfter.Size = new System.Drawing.Size(104, 20);
            this.dtpParentalAfter.TabIndex = 13;
            this.dtpParentalAfter.Value = new System.DateTime(2018, 2, 25, 21, 0, 0, 0);
            this.dtpParentalAfter.ValueChanged += new System.EventHandler(this.dtpParentalAfter_ValueChanged);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(386, 310);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(145, 13);
            this.label21.TabIndex = 12;
            this.label21.Text = "Disable parental control after:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(356, 287);
            this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(175, 13);
            this.label19.TabIndex = 10;
            this.label19.Text = "Parental Control Auto-Lock Interval:";
            // 
            // buttonPinCode
            // 
            this.buttonPinCode.Location = new System.Drawing.Point(378, 248);
            this.buttonPinCode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonPinCode.Name = "buttonPinCode";
            this.buttonPinCode.Size = new System.Drawing.Size(100, 22);
            this.buttonPinCode.TabIndex = 8;
            this.buttonPinCode.Text = "Pin &Code...";
            this.buttonPinCode.UseVisualStyleBackColor = true;
            this.buttonPinCode.Click += new System.EventHandler(this.buttonPinCode_Click);
            // 
            // buttonEditView
            // 
            this.buttonEditView.Enabled = false;
            this.buttonEditView.Location = new System.Drawing.Point(559, 94);
            this.buttonEditView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonEditView.Name = "buttonEditView";
            this.buttonEditView.Size = new System.Drawing.Size(74, 21);
            this.buttonEditView.TabIndex = 7;
            this.buttonEditView.Text = "&Edit";
            this.buttonEditView.UseVisualStyleBackColor = true;
            this.buttonEditView.Click += new System.EventHandler(this.buttonEditView_Click);
            // 
            // checkBoxParentalControl
            // 
            this.checkBoxParentalControl.AutoSize = true;
            this.checkBoxParentalControl.Location = new System.Drawing.Point(360, 221);
            this.checkBoxParentalControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.label25.Location = new System.Drawing.Point(357, 45);
            this.label25.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(52, 13);
            this.label25.TabIndex = 3;
            this.label25.Text = "Hierarchy";
            // 
            // checkCurViewEnabled
            // 
            this.checkCurViewEnabled.AutoSize = true;
            this.checkCurViewEnabled.Location = new System.Drawing.Point(360, 194);
            this.checkCurViewEnabled.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.lnkResetView.Location = new System.Drawing.Point(166, 315);
            this.lnkResetView.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lnkResetView.Name = "lnkResetView";
            this.lnkResetView.Size = new System.Drawing.Size(89, 13);
            this.lnkResetView.TabIndex = 3;
            this.lnkResetView.TabStop = true;
            this.lnkResetView.Text = "Reset to Defaults";
            this.lnkResetView.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkResetView_LinkClicked);
            // 
            // btnRemoveView
            // 
            this.btnRemoveView.Location = new System.Drawing.Point(559, 126);
            this.btnRemoveView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnRemoveView.Name = "btnRemoveView";
            this.btnRemoveView.Size = new System.Drawing.Size(74, 21);
            this.btnRemoveView.TabIndex = 4;
            this.btnRemoveView.Text = "&Remove";
            this.btnRemoveView.UseVisualStyleBackColor = true;
            this.btnRemoveView.Click += new System.EventHandler(this.btnRemoveView_Click);
            // 
            // view_selStepsList
            // 
            this.view_selStepsList.Enabled = false;
            this.view_selStepsList.FormattingEnabled = true;
            this.view_selStepsList.Location = new System.Drawing.Point(359, 63);
            this.view_selStepsList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.view_selStepsList.Name = "view_selStepsList";
            this.view_selStepsList.Size = new System.Drawing.Size(184, 121);
            this.view_selStepsList.TabIndex = 4;
            // 
            // btnAddView
            // 
            this.btnAddView.Location = new System.Drawing.Point(559, 63);
            this.btnAddView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnAddView.Name = "btnAddView";
            this.btnAddView.Size = new System.Drawing.Size(74, 21);
            this.btnAddView.TabIndex = 5;
            this.btnAddView.Text = "&Add";
            this.btnAddView.UseVisualStyleBackColor = true;
            this.btnAddView.Click += new System.EventHandler(this.btnAddView_Click);
            // 
            // _availViews
            // 
            this._availViews.FormattingEnabled = true;
            this._availViews.Location = new System.Drawing.Point(8, 23);
            this._availViews.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._availViews.Name = "_availViews";
            this._availViews.Size = new System.Drawing.Size(247, 277);
            this._availViews.TabIndex = 0;
            this._availViews.SelectedIndexChanged += new System.EventHandler(this._availViews_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(295, 23);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // view_selectedName
            // 
            this.view_selectedName.Location = new System.Drawing.Point(359, 20);
            this.view_selectedName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.view_selectedName.Name = "view_selectedName";
            this.view_selectedName.Size = new System.Drawing.Size(184, 20);
            this.view_selectedName.TabIndex = 1;
            this.view_selectedName.TextChanged += new System.EventHandler(this.view_selectedName_TextChanged);
            // 
            // tabLogoRules
            // 
            this.tabLogoRules.Controls.Add(this.label65);
            this.tabLogoRules.Controls.Add(this.groupBox7);
            this.tabLogoRules.ImageIndex = 5;
            this.tabLogoRules.Location = new System.Drawing.Point(4, 31);
            this.tabLogoRules.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabLogoRules.Name = "tabLogoRules";
            this.tabLogoRules.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabLogoRules.Size = new System.Drawing.Size(1195, 942);
            this.tabLogoRules.TabIndex = 9;
            this.tabLogoRules.Text = "Logos";
            this.tabLogoRules.UseVisualStyleBackColor = true;
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Location = new System.Drawing.Point(16, 11);
            this.label65.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
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
            this.groupBox7.Location = new System.Drawing.Point(17, 42);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox7.Size = new System.Drawing.Size(1169, 890);
            this.groupBox7.TabIndex = 5;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Logo Configuration";
            // 
            // btnLogoDeleteAll
            // 
            this.btnLogoDeleteAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogoDeleteAll.Location = new System.Drawing.Point(787, 833);
            this.btnLogoDeleteAll.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnLogoDeleteAll.Name = "btnLogoDeleteAll";
            this.btnLogoDeleteAll.Size = new System.Drawing.Size(105, 26);
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
            this.tabFormattingRules.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabFormattingRules.Name = "tabFormattingRules";
            this.tabFormattingRules.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabFormattingRules.Size = new System.Drawing.Size(1195, 942);
            this.tabFormattingRules.TabIndex = 10;
            this.tabFormattingRules.Text = "Formatting";
            this.tabFormattingRules.UseVisualStyleBackColor = true;
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(16, 11);
            this.label64.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
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
            this.formattingConfiguration1.Location = new System.Drawing.Point(16, 45);
            this.formattingConfiguration1.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.formattingConfiguration1.Name = "formattingConfiguration1";
            this.formattingConfiguration1.Size = new System.Drawing.Size(1168, 586);
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
            this.tabLayoutSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabLayoutSettings.Name = "tabLayoutSettings";
            this.tabLayoutSettings.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabLayoutSettings.Size = new System.Drawing.Size(1195, 942);
            this.tabLayoutSettings.TabIndex = 11;
            this.tabLayoutSettings.Text = "Layout";
            this.tabLayoutSettings.UseVisualStyleBackColor = true;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(16, 11);
            this.label37.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(781, 13);
            this.label37.TabIndex = 10;
            this.label37.Text = "Listing Format Settings will be overridden by skin if defined, to override set th" +
    "e Import attribute to false in your TVSeries.SkinSettings.xml located in your sk" +
    "in directory.\r\n";
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
            this.groupBox3.Location = new System.Drawing.Point(19, 183);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Size = new System.Drawing.Size(1155, 140);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Season View";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(9, 44);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(93, 20);
            this.label12.TabIndex = 5;
            this.label12.Text = "Title Format:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(6, 18);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(96, 20);
            this.label13.TabIndex = 0;
            this.label13.Text = "Listing Format:";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(6, 70);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(96, 20);
            this.label14.TabIndex = 7;
            this.label14.Text = "Subtitle Format:";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(6, 96);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(96, 20);
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
            this.groupBox4.Location = new System.Drawing.Point(19, 333);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox4.Size = new System.Drawing.Size(1155, 138);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Episode View";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(0, 21);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(104, 20);
            this.label7.TabIndex = 0;
            this.label7.Text = "Listing Format:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(3, 47);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(101, 20);
            this.label8.TabIndex = 4;
            this.label8.Text = "Title Format:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(6, 73);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(98, 20);
            this.label9.TabIndex = 6;
            this.label9.Text = "Subtitle Format:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(9, 99);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(95, 20);
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
            this.groupBox2.Location = new System.Drawing.Point(19, 29);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(1155, 144);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Series View";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 18);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "Listing Format:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 94);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 20);
            this.label6.TabIndex = 9;
            this.label6.Text = "Main Format:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 68);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 20);
            this.label5.TabIndex = 7;
            this.label5.Text = "Subtitle Format:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 42);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 20);
            this.label4.TabIndex = 5;
            this.label4.Text = "Title Format:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabAbout
            // 
            this.tabAbout.AutoScroll = true;
            this.tabAbout.Controls.Add(this.aboutScreen);
            this.tabAbout.ImageIndex = 4;
            this.tabAbout.Location = new System.Drawing.Point(4, 31);
            this.tabAbout.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabAbout.Size = new System.Drawing.Size(1195, 942);
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
            this.aboutScreen.Location = new System.Drawing.Point(3, 5);
            this.aboutScreen.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.aboutScreen.Name = "aboutScreen";
            this.aboutScreen.Size = new System.Drawing.Size(1187, 919);
            this.aboutScreen.TabIndex = 0;
            // 
            // listBox_Log
            // 
            this.listBox_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox_Log.FormattingEnabled = true;
            this.listBox_Log.HorizontalScrollbar = true;
            this.listBox_Log.Location = new System.Drawing.Point(15, 15);
            this.listBox_Log.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listBox_Log.Name = "listBox_Log";
            this.listBox_Log.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listBox_Log.Size = new System.Drawing.Size(120, 16);
            this.listBox_Log.TabIndex = 5;
            // 
            // dbOptionDisableMediaInfoInConfig
            // 
            this.dbOptionDisableMediaInfoInConfig.AutoSize = true;
            this.dbOptionDisableMediaInfoInConfig.Location = new System.Drawing.Point(467, 413);
            this.dbOptionDisableMediaInfoInConfig.Name = "dbOptionDisableMediaInfoInConfig";
            this.dbOptionDisableMediaInfoInConfig.Option = "DisableMediaInfoInConfigImports";
            this.dbOptionDisableMediaInfoInConfig.Size = new System.Drawing.Size(292, 17);
            this.dbOptionDisableMediaInfoInConfig.TabIndex = 43;
            this.dbOptionDisableMediaInfoInConfig.Text = "Disable MediaInfo scan when importing via configuration";
            this.dbOptionDisableMediaInfoInConfig.ToolTip = "";
            this.dbOptionDisableMediaInfoInConfig.UseVisualStyleBackColor = true;
            // 
            // ConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1203, 999);
            this.Controls.Add(this.splitMain_Log);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(1219, 1038);
            this.Name = "ConfigurationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MP-TV Series Configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigurationForm_FormClosing);
            this.contextMenuStrip_DetailsTree.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Replace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRecentlyAddedDays)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScanRemoteShareFrequency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWatchedAfter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudParentalControlTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownImportDelay)).EndInit();
            this.splitMain_Log.Panel1.ResumeLayout(false);
            this.splitMain_Log.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain_Log)).EndInit();
            this.splitMain_Log.ResumeLayout(false);
            this.tabControl_Details.ResumeLayout(false);
            this.tabPage_Details.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SeriesPoster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Series)).EndInit();
            this.panDBLocation.ResumeLayout(false);
            this.panDBLocation.PerformLayout();
            this.tabPage_Import.ResumeLayout(false);
            this.splitContainer_SettingsOutput.Panel1.ResumeLayout(false);
            this.splitContainer_SettingsOutput.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_SettingsOutput)).EndInit();
            this.splitContainer_SettingsOutput.ResumeLayout(false);
            this.splitContainerImportSettings.Panel1.ResumeLayout(false);
            this.splitContainerImportSettings.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerImportSettings)).EndInit();
            this.splitContainerImportSettings.ResumeLayout(false);
            this.panel_ImportPathes.ResumeLayout(false);
            this.panel_ImportPathes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ImportPathes)).EndInit();
            this.panel_Expressions.ResumeLayout(false);
            this.panel_Expressions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Expressions)).EndInit();
            this.panel_StringReplacements.ResumeLayout(false);
            this.panel_StringReplacements.PerformLayout();
            this.tabOnlineData.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            this.panel_OnlineData.ResumeLayout(false);
            this.panel_OnlineData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudConsecFailures)).EndInit();
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
            this.tabAbout.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColumnHeader columnHeader_Series;
        private System.Windows.Forms.ColumnHeader columnHeader_Title;
        private System.Windows.Forms.ColumnHeader columnHeader_Season;
        private System.Windows.Forms.ColumnHeader columnHeader_Episode;
        private System.Windows.Forms.ColumnHeader columnHeader_OriginallyAired;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ListBox listBox_Log;
        private System.Windows.Forms.ToolTip toolTip_Help;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_InsertFields;
        private System.Windows.Forms.SplitContainer splitMain_Log;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_DetailsTree;
        private System.Windows.Forms.ToolStripMenuItem hideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem watchedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unWatchedToolStripMenuItem;
        //private System.Windows.Forms.ToolStripMenuItem resetUserSelectionsToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip_InfoHelp;
        private System.Windows.Forms.ImageList imageList1;
        //private System.Windows.Forms.RadioButton radioButton_episode_AllEpisodes;
        //private System.Windows.Forms.Label label_textboxEmuleWSType;
        //private System.Windows.Forms.ComboBox comboBox_emuleWSType;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem reScanMediaInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ignoreOnScanToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.TabControl tabControl_Details;
        private System.Windows.Forms.TabPage tabPage_Details;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.LinkLabel linkMediaInfoUpdate;
        private System.Windows.Forms.TreeView treeView_Library;
        private System.Windows.Forms.CheckBox checkBox_ShowHidden;
        private System.Windows.Forms.PictureBox pictureBox_SeriesPoster;
        private System.Windows.Forms.ComboBox comboBox_PosterSelection;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.PictureBox pictureBox_Series;
        private System.Windows.Forms.ComboBox comboBox_BannerSelection;
        private System.Windows.Forms.Panel panDBLocation;
        private System.Windows.Forms.TextBox textBox_dblocation;
        private System.Windows.Forms.LinkLabel lblClearDB;
        private System.Windows.Forms.Button button_dbbrowse;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.TabPage tabPage_Import;
        private System.Windows.Forms.SplitContainer splitContainer_SettingsOutput;
        private System.Windows.Forms.SplitContainer splitContainerImportSettings;
        private System.Windows.Forms.TreeView treeView_Settings;
        private System.Windows.Forms.Panel panel_Expressions;
        private System.Windows.Forms.LinkLabel linkExParsingExpressions;
        private System.Windows.Forms.LinkLabel linkImpParsingExpressions;
        private System.Windows.Forms.LinkLabel linkExpressionHelp;
        private System.Windows.Forms.Label label70;
        private System.Windows.Forms.LinkLabel buildExpr;
        private System.Windows.Forms.LinkLabel resetExpr;
        private System.Windows.Forms.DataGridView dataGridView_Expressions;
        private System.Windows.Forms.Button button_MoveExpUp;
        private System.Windows.Forms.Button button_MoveExpDown;
        private System.Windows.Forms.Panel panel_StringReplacements;
        private System.Windows.Forms.LinkLabel linkLabelResetStringReplacements;
        private System.Windows.Forms.LinkLabel linkLabelImportStringReplacements;
        private System.Windows.Forms.LinkLabel linkLabelExportStringReplacements;
        private System.Windows.Forms.Label label69;
        private System.Windows.Forms.DataGridView dataGridView_Replace;
        private System.Windows.Forms.Panel panel_ImportPathes;
        private System.Windows.Forms.Label label68;
        private System.Windows.Forms.Button button_TestReparse;
        private System.Windows.Forms.DataGridView dataGridView_ImportPathes;
        private System.Windows.Forms.ListView listView_ParsingResults;
        private System.Windows.Forms.ColumnHeader FileName;
        private System.Windows.Forms.ColumnHeader ParsedSeriesName;
        private System.Windows.Forms.ColumnHeader SeasonID;
        private System.Windows.Forms.ColumnHeader EpisodeID;
        private System.Windows.Forms.ColumnHeader EpisodeTitle;
        private System.Windows.Forms.Button buttonStartImport;
        private System.Windows.Forms.TabPage tabOnlineData;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.Panel panel_OnlineData;
        private System.Windows.Forms.Button buttonArtworkDownloadLimits;
        private Configuration.DBOptionCheckBox dbOptCheckBoxGetTextBanners;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label lblImportDelaySecs;
        private System.Windows.Forms.NumericUpDown numericUpDownImportDelay;
        private System.Windows.Forms.Label lblImportDelayCaption;
        private System.Windows.Forms.CheckBox checkboxAutoDownloadFanartSeriesName;
        private System.Windows.Forms.CheckBox checkBox_AutoDownloadMissingArtwork;
        private System.Windows.Forms.CheckBox checkBox_AutoUpdateAllFanart;
        private System.Windows.Forms.CheckBox checkBox_AutoUpdateEpisodeRatings;
        private System.Windows.Forms.CheckBox checkBox_ScanOnStartup;
        private System.Windows.Forms.Label label73;
        private System.Windows.Forms.NumericUpDown spinMaxFanarts;
        private System.Windows.Forms.Label label72;
        private System.Windows.Forms.Label label71;
        private System.Windows.Forms.ComboBox cboFanartResolution;
        private System.Windows.Forms.CheckBox chkAutoDownloadFanart;
        private System.Windows.Forms.LinkLabel linkAccountID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown_AutoOnlineDataRefresh;
        private System.Windows.Forms.CheckBox checkBox_AutoOnlineDataRefresh;
        private System.Windows.Forms.CheckBox checkBox_AutoChooseOrder;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.Label label54;
        private System.Windows.Forms.CheckBox checkDownloadEpisodeSnapshots;
        private System.Windows.Forms.CheckBox chkBlankBanners;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox txtMainMirror;
        private System.Windows.Forms.LinkLabel linkDelUpdateTime;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.ComboBox comboOnlineLang;
        private System.Windows.Forms.CheckBox checkBox_OverrideComboLang;
        private System.Windows.Forms.CheckBox checkBox_OnlineSearch;
        private System.Windows.Forms.CheckBox checkBox_AutoChooseSeries;
        private System.Windows.Forms.CheckBox checkBox_FullSeriesRetrieval;
        private System.Windows.Forms.TabPage tabPage_MP_DisplayControl;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblRecentAddedDays;
        private System.Windows.Forms.NumericUpDown nudRecentlyAddedDays;
        private System.Windows.Forms.ComboBox cbNewEpisodeThumbIndicator;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label laOnPlaySeriesOrSeasonAction;
        private System.Windows.Forms.ComboBox cbOnPlaySeriesOrSeasonAction;
        private Configuration.DBOptionCheckBox dbOptChkBoxCountEmptyFutureEps;
        private Configuration.DBOptionCheckBox dbOptChkBoxScanFullscreenVideo;
        private Configuration.DBOptionCheckBox dbOptionCheckBox2;
        private Configuration.DBOptionCheckBox dbOptionCheckBoxSubstituteMissingArtwork;
        private System.Windows.Forms.NumericUpDown nudScanRemoteShareFrequency;
        private System.Windows.Forms.CheckBox checkBox_scanRemoteShares;
        private Configuration.DBOptionCheckBox dbOptionCheckBoxMarkRatedEpsAsWatched;
        private System.Windows.Forms.CheckBox checkboxRatingDisplayStars;
        private System.Windows.Forms.Label label77;
        private System.Windows.Forms.Label label76;
        private System.Windows.Forms.Label label75;
        private System.Windows.Forms.NumericUpDown numericUpDownArtworkDelay;
        private System.Windows.Forms.Label label74;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.NumericUpDown numericUpDownBackdropDelay;
        private System.Windows.Forms.CheckBox checkbox_SortSpecials;
        private System.Windows.Forms.LinkLabel linkExWatched;
        private System.Windows.Forms.LinkLabel linkImpWatched;
        private System.Windows.Forms.CheckBox chkAllowDeletes;
        private System.Windows.Forms.CheckBox checkBox_doFolderWatch;
        private System.Windows.Forms.CheckBox checkBox_Episode_HideUnwatchedSummary;
        private System.Windows.Forms.CheckBox checkBox_Episode_HideUnwatchedThumbnail;
        private System.Windows.Forms.CheckBox checkBox_Episode_OnlyShowLocalFiles;
        private System.Windows.Forms.CheckBox checkBox_Series_UseSortName;
        private Configuration.DBOptionCheckBox optionAsk2Rate;
        private System.Windows.Forms.CheckBox chkUseRegionalDateFormatString;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.NumericUpDown nudWatchedAfter;
        private System.Windows.Forms.CheckBox checkBox_RandBanner;
        private System.Windows.Forms.TextBox textBox_PluginHomeName;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TabPage tab_view;
        private Configuration.PlaylistSettings playlistSettings;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Button buttonViewTemplates;
        private System.Windows.Forms.Button buttonPinCode;
        private System.Windows.Forms.Button buttonEditView;
        private System.Windows.Forms.CheckBox checkBoxParentalControl;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.CheckBox checkCurViewEnabled;
        private System.Windows.Forms.LinkLabel lnkResetView;
        private System.Windows.Forms.Button btnRemoveView;
        private System.Windows.Forms.ListBox view_selStepsList;
        private System.Windows.Forms.Button btnAddView;
        private System.Windows.Forms.ListBox _availViews;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox view_selectedName;
        private System.Windows.Forms.TabPage tabLogoRules;
        private System.Windows.Forms.Label label65;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button btnLogoDeleteAll;
        private System.Windows.Forms.Button btnLogoDown;
        private System.Windows.Forms.Button btnlogoUp;
        private System.Windows.Forms.LinkLabel lnkLogoImp;
        private System.Windows.Forms.ListBox lstLogos;
        private System.Windows.Forms.Button btnLogoTemplate;
        private System.Windows.Forms.Button btnrmvLogo;
        private System.Windows.Forms.LinkLabel lnkLogoExport;
        private System.Windows.Forms.Button btnLogoEdit;
        private System.Windows.Forms.Button addLogo;
        private System.Windows.Forms.TabPage tabFormattingRules;
        private System.Windows.Forms.Label label64;
        private Configuration.FormattingConfiguration formattingConfiguration1;
        private System.Windows.Forms.TabPage tabLayoutSettings;
        private System.Windows.Forms.Label label37;
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
        private System.Windows.Forms.TabPage tabAbout;
        private About aboutScreen;
        private Configuration.DBOptionCheckBox dbOptionCheckBox3;
        private System.Windows.Forms.NumericUpDown nudParentalControlTimeout;
        private System.Windows.Forms.Label label19;
        private WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox dbOptChkBox_SubCentral_DownloadSubtitlesOnPlay;
        private Configuration.DBOptionCheckBox dbOptionSQLLogging;
        private Configuration.DBOptionCheckBox dbOptChkBoxPlayOutOfOrderCheck;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown nudConsecFailures;
        private System.Windows.Forms.Label label20;
        private Configuration.DBOptionCheckBox dbOptionCheckBox4;
        private Configuration.DBOptionCheckBox dbOptCheckBoxDownloadActors;
        private Configuration.DBOptionCheckBox dbOptCheckBoxCleanOnlineEpisodes;
        private Configuration.DBOptionCheckBox dbOptCheckBoxRemoveEpZero;
        private Configuration.DBOptionCheckBox checkBox_CountSpecialEpisodesAsWatched;
        private Configuration.DBOptionCheckBox dbCheckBoxTraktCommunityRatings;
        private System.Windows.Forms.DateTimePicker dtpParentalBefore;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.DateTimePicker dtpParentalAfter;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.LinkLabel lnkImageCache;
        private System.Windows.Forms.LinkLabel lnkOpenAPICacheDir;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.LinkLabel lnkTVDbSeries;
        private System.Windows.Forms.LinkLabel lnkIMDbSeries;
        private System.Windows.Forms.LinkLabel lnkTraktSeries;
        private DBOptionCheckBox dbOptionParsedNameFromFolder;
        private DBOptionCheckBox dbOptionDisableMediaInfoInConfig;
    }
}
