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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using MediaPortal.Dialogs;
using MediaPortal.Util;
using MediaPortal.Configuration;
using System.Windows.Forms;
using SQLite.NET;
using WindowPlugins.GUITVSeries;
using WindowPlugins.GUITVSeries.Feedback;
using WindowPlugins.GUITVSeries.Local_Parsing_Classes;
using WindowPlugins.GUITVSeries.Configuration;
using System.Xml;
using SubtitleDownloader.Core;
using WindowPlugins.GUITVSeries.Subtitles;
using MediaPortal.GUI.Library;

#if DEBUG
using System.Diagnostics;
#endif

// TODO: replace all checkboxes that are used to save options with a dboptioncheckbox!!!

namespace WindowPlugins.GUITVSeries
{
    public partial class ConfigurationForm : Form, Feedback.IFeedback
    {
        private List<Control> m_paneListSettings = new List<Control>();
        private List<Panel> m_paneListExtra = new List<Panel>();
        private TreeNode nodeEdited = null;
        private OnlineParsing m_parser = null;
        private DateTime m_timingStart = new DateTime();

        private DBSeries m_SeriesReference = new DBSeries(true);
        private DBSeason m_SeasonReference = new DBSeason();
        private DBEpisode m_EpisodeReference = new DBEpisode(true);

        private DBTorrentSearch m_currentTorrentSearch = null;
        private DBNewzbin m_currentNewsSearch = null;
        List<logicalView> availViews = new List<logicalView>();
        logicalView selectedView = null;
        loadingDisplay load = null;
        List<Language> onlineLanguages = new List<Language>();
        bool initLoading = true;

        private Control m_localControlForInvoke;
        private static ConfigurationForm instance = null;

        private bool subtitleDownloaderWorking = false;

        private List<CheckBox> subtitleDownloader_LanguageCheckBoxes = new List<CheckBox>();
        private List<CheckBox> subtitleDownloader_DownloaderCheckBoxes = new List<CheckBox>();

        public static ConfigurationForm GetInstance()
        {
            return instance;
        }

        public ConfigurationForm()
        {
            m_localControlForInvoke = new Control();
            m_localControlForInvoke.CreateControl();
#if DEBUG
            //    Debugger.Launch();
#endif
            InitializeComponent();
            MPTVSeriesLog.AddNotifier(ref listBox_Log);

            MPTVSeriesLog.Write("**** Plugin started in configuration mode ***");
            
            // set height/width
            int height = DBOption.GetOptions("configSizeHeight");
            int width = DBOption.GetOptions("configSizeWidth");
            if (height > this.MinimumSize.Height && width > this.MinimumSize.Width)
            {
                System.Drawing.Size s = new Size(width, height);
                this.Size = s;
            }
            this.Resize += new EventHandler(ConfigurationForm_Resize);
            Download.Monitor.Start(this);
            panel_manualEpisodeManagement.SetOwner(this);

            load = new loadingDisplay();
            InitSettingsTreeAndPanes();
            InitExtraTreeAndPanes();
            
            LoadImportPathes();
            LoadExpressions();
            LoadReplacements();
            
            initLoading = false;
            LoadTree();

            // Only Advanced Users / Skin Designers need to see these.
            // Tabs are visible if import="false" TVSeries.SkinSettings.xml
            if (SkinSettings.ImportFormatting) tabControl_Details.TabPages.Remove(tabFormattingRules);
            if (SkinSettings.ImportLogos) tabControl_Details.TabPages.Remove(tabLogoRules);
            
            if (load != null) load.Close();
            instance = this;

            this.aboutScreen.setUpMPInfo(Settings.Version.ToString(), Settings.BuildDate);
            this.aboutScreen.setUpPaths();
        }

        void ConfigurationForm_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }

        #region Init
        private void InitSettingsTreeAndPanes()
        {   
            string skinSettings = null;
			string MediaPortalConfig = Path.Combine(Config.GetFolder(Config.Dir.Config), "MediaPortal.xml");
     
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(MediaPortalConfig);
                // Get current skin file
                XmlNodeList nodeList = doc.DocumentElement.SelectNodes("/profile/section");
                foreach (XmlNode node in nodeList)
                {
                    if (node.Attributes.Item(0).Value == "skin")
                    {
                        XmlNodeList skinNodelist = node.ChildNodes;
                        foreach (XmlNode skinNode in skinNodelist)
                        {
                            if (skinNode.Attributes.Item(0).Value == "name")
                            {
                                skinSettings = Path.Combine(Config.GetFolder(Config.Dir.Skin), skinNode.InnerText + "\\TVSeries.SkinSettings.xml");
                                break;
                            }
                        }
                        break;
                    }
                }
                // Load Skin Settings if they exist
                SkinSettings.Load(skinSettings);
                // Reload formatting rules
                if (!SkinSettings.ImportFormatting) {
                    formattingConfiguration1.LoadFromDB();
                }
            }
            catch { }

            textBox_dblocation.Text = Settings.GetPath(Settings.Path.database);

            //this.comboLogLevel.SelectedIndex = 0;
            this.splitContainer2.Panel1.SizeChanged += new EventHandler(Panel1_SizeChanged);
            m_paneListSettings.Add(panel_ImportPathes);
            m_paneListSettings.Add(panel_StringReplacements);            
            m_paneListSettings.Add(panel_Expressions);            
            m_paneListSettings.Add(panel_manualEpisodeManagement);

            foreach (Control pane in m_paneListSettings)
            {
                pane.Dock = DockStyle.Fill;
                pane.Visible = false;
                TreeNode node = new TreeNode(pane.Tag.ToString());
                node.Name = pane.Name;
                treeView_Settings.Nodes.Add(node);
            }

            splitMain_Log.Panel2Collapsed = DBOption.GetOptions(DBOption.cConfig_LogCollapsed);
            log_window_changed();
            treeView_Settings.SelectedNode = treeView_Settings.Nodes[0];
            nudWatchedAfter.Value = DBOption.GetOptions(DBOption.cWatchedAfter);
            textBox_PluginHomeName.Text = DBOption.GetOptions(DBOption.cView_PluginName);
            checkBox_OnlineSearch.Checked = DBOption.GetOptions(DBOption.cOnlineParseEnabled);
            checkBox_FullSeriesRetrieval.Checked = DBOption.GetOptions(DBOption.cFullSeriesRetrieval);
            checkBox_AutoChooseSeries.Checked = DBOption.GetOptions(DBOption.cAutoChooseSeries);
            checkBox_AutoChooseOrder.Checked = DBOption.GetOptions(DBOption.cAutoChooseOrder);            
            checkBox_Episode_OnlyShowLocalFiles.Checked = DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles);
            checkBox_Episode_HideUnwatchedSummary.Checked = DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedSummary);
            checkBox_Episode_HideUnwatchedThumbnail.Checked = DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedThumbnail);
            checkBox_doFolderWatch.Checked = DBOption.GetOptions(DBOption.cImport_FolderWatch);
            checkBox_scanRemoteShares.Checked = DBOption.GetOptions(DBOption.cImport_ScanRemoteShare);
            nudScanRemoteShareFrequency.Value = DBOption.GetOptions(DBOption.cImport_ScanRemoteShareLapse);
            checkBox_SubDownloadOnPlay.Checked = DBOption.GetOptions(DBOption.cPlay_SubtitleDownloadOnPlay);

            if (checkBox_doFolderWatch.Checked) {
                checkBox_scanRemoteShares.Enabled = true;
                if (checkBox_scanRemoteShares.Checked) {
                    nudScanRemoteShareFrequency.Enabled = true;
                    dbOptChkBoxScanFullscreenVideo.Enabled = true;
                }
                else {
                    nudScanRemoteShareFrequency.Enabled = false;
                    dbOptChkBoxScanFullscreenVideo.Enabled = false;
                }
            }
            else {
                checkBox_scanRemoteShares.Enabled = false;
                nudScanRemoteShareFrequency.Enabled = false;
                dbOptChkBoxScanFullscreenVideo.Enabled = false;
            }

            checkBox_RandBanner.Checked = DBOption.GetOptions(DBOption.cRandomBanner);
            textBox_NewsDownloadPath.Text = DBOption.GetOptions(DBOption.cNewsLeecherDownloadPath);
            this.chkAllowDeletes.Checked = (bool)DBOption.GetOptions(DBOption.cShowDeleteMenu);
            this.chkUseRegionalDateFormatString.Checked = (bool)DBOption.GetOptions(DBOption.cAltImgLoading);
            txtUserID.Text = DBOption.GetOptions(DBOption.cOnlineUserID);
            chkBlankBanners.Checked = DBOption.GetOptions(DBOption.cGetBlankBanners);
            checkDownloadEpisodeSnapshots.Checked = DBOption.GetOptions(DBOption.cGetEpisodeSnapshots);
            checkBox_ShowHidden.Checked = DBOption.GetOptions(DBOption.cShowHiddenItems);            
            checkbox_SortSpecials.Checked = DBOption.GetOptions(DBOption.cSortSpecials);
            checkBox_ScanOnStartup.Checked = DBOption.GetOptions(DBOption.cImport_ScanOnStartup);
            checkBox_AutoDownloadMissingArtwork.Checked = DBOption.GetOptions(DBOption.cAutoDownloadMissingArtwork);
            checkBox_AutoUpdateEpisodeRatings.Checked = DBOption.GetOptions(DBOption.cAutoUpdateEpisodeRatings);
            checkBox_AutoUpdateAllFanart.Checked = DBOption.GetOptions(DBOption.cAutoUpdateAllFanart);
			numericUpDownArtworkDelay.Value = DBOption.GetOptions(DBOption.cArtworkLoadingDelay);
			numericUpDownBackdropDelay.Value = DBOption.GetOptions(DBOption.cBackdropLoadingDelay);

			if (DBOption.GetOptions(DBOption.cRatingDisplayStars) == 5)
				checkboxRatingDisplayStars.Checked = true;

            int nValue = DBOption.GetOptions(DBOption.cImport_AutoUpdateOnlineDataLapse);
            numericUpDown_AutoOnlineDataRefresh.Minimum = 1;
            numericUpDown_AutoOnlineDataRefresh.Maximum = 24;
            numericUpDown_AutoOnlineDataRefresh.Value = nValue;
            checkBox_AutoOnlineDataRefresh.Checked = DBOption.GetOptions(DBOption.cImport_AutoUpdateOnlineData);
            numericUpDown_AutoOnlineDataRefresh.Enabled = checkBox_AutoOnlineDataRefresh.Checked;

            chkAutoDownloadFanart.Checked = DBOption.GetOptions(DBOption.cAutoDownloadFanart);
            cboFanartResolution.SelectedIndex = DBOption.GetOptions(DBOption.cAutoDownloadFanartResolution);
            spinMaxFanarts.Value = DBOption.GetOptions(DBOption.cAutoDownloadFanartCount);
            checkboxAutoDownloadFanartSeriesName.Checked = DBOption.GetOptions(DBOption.cAutoDownloadFanartSeriesNames);

            checkBox_Series_UseSortName.Checked = DBOption.GetOptions(DBOption.cSeries_UseSortName);            
            
            chkShowSeriesFanart.Checked = DBOption.GetOptions(DBOption.cShowSeriesFanart);
            
            richTextBox_seriesFormat_Col1.Tag = new FieldTag(DBOption.cView_Series_Col1, FieldTag.Level.Series);
            richTextBox_seriesFormat_Col1.Enabled = !SkinSettings.ImportViews;
            FieldValidate(ref richTextBox_seriesFormat_Col1);

            richTextBox_seriesFormat_Col2.Tag = new FieldTag(DBOption.cView_Series_Col2, FieldTag.Level.Series);
            richTextBox_seriesFormat_Col2.Enabled = !SkinSettings.ImportViews;
            FieldValidate(ref richTextBox_seriesFormat_Col2);

            richTextBox_seriesFormat_Col3.Tag = new FieldTag(DBOption.cView_Series_Col3, FieldTag.Level.Series);
            richTextBox_seriesFormat_Col3.Enabled = !SkinSettings.ImportViews;
            FieldValidate(ref richTextBox_seriesFormat_Col3);

            richTextBox_seriesFormat_Title.Tag = new FieldTag(DBOption.cView_Series_Title, FieldTag.Level.Series);
            FieldValidate(ref richTextBox_seriesFormat_Title);

            richTextBox_seriesFormat_Subtitle.Tag = new FieldTag(DBOption.cView_Series_Subtitle, FieldTag.Level.Series);
            FieldValidate(ref richTextBox_seriesFormat_Subtitle);

            richTextBox_seriesFormat_Main.Tag = new FieldTag(DBOption.cView_Series_Main, FieldTag.Level.Series);
            FieldValidate(ref richTextBox_seriesFormat_Main);          

            richTextBox_seasonFormat_Col1.Tag = new FieldTag(DBOption.cView_Season_Col1, FieldTag.Level.Season);
            richTextBox_seasonFormat_Col1.Enabled = !SkinSettings.ImportViews;
            FieldValidate(ref richTextBox_seasonFormat_Col1);

            richTextBox_seasonFormat_Col2.Tag = new FieldTag(DBOption.cView_Season_Col2, FieldTag.Level.Season);
            richTextBox_seasonFormat_Col2.Enabled = !SkinSettings.ImportViews;
            FieldValidate(ref richTextBox_seasonFormat_Col2);

            richTextBox_seasonFormat_Col3.Tag = new FieldTag(DBOption.cView_Season_Col3, FieldTag.Level.Season);
            richTextBox_seasonFormat_Col3.Enabled = !SkinSettings.ImportViews;
            FieldValidate(ref richTextBox_seasonFormat_Col3);

            richTextBox_seasonFormat_Title.Tag = new FieldTag(DBOption.cView_Season_Title, FieldTag.Level.Season);
            FieldValidate(ref richTextBox_seasonFormat_Title);

            richTextBox_seasonFormat_Subtitle.Tag = new FieldTag(DBOption.cView_Season_Subtitle, FieldTag.Level.Season);
            FieldValidate(ref richTextBox_seasonFormat_Subtitle);

            richTextBox_seasonFormat_Main.Tag = new FieldTag(DBOption.cView_Season_Main, FieldTag.Level.Season);
            FieldValidate(ref richTextBox_seasonFormat_Main);

            richTextBox_episodeFormat_Col1.Tag = new FieldTag(DBOption.cView_Episode_Col1, FieldTag.Level.Episode);
            richTextBox_episodeFormat_Col1.Enabled = !SkinSettings.ImportViews;
            FieldValidate(ref richTextBox_episodeFormat_Col1);

            richTextBox_episodeFormat_Col2.Tag = new FieldTag(DBOption.cView_Episode_Col2, FieldTag.Level.Episode);
            richTextBox_episodeFormat_Col2.Enabled = !SkinSettings.ImportViews;
            FieldValidate(ref richTextBox_episodeFormat_Col2);

            richTextBox_episodeFormat_Col3.Tag = new FieldTag(DBOption.cView_Episode_Col3, FieldTag.Level.Episode);
            richTextBox_episodeFormat_Col3.Enabled = !SkinSettings.ImportViews;
            FieldValidate(ref richTextBox_episodeFormat_Col3);

            richTextBox_episodeFormat_Title.Tag = new FieldTag(DBOption.cView_Episode_Title, FieldTag.Level.Episode);
            FieldValidate(ref richTextBox_episodeFormat_Title);

            richTextBox_episodeFormat_Subtitle.Tag = new FieldTag(DBOption.cView_Episode_Subtitle, FieldTag.Level.Episode);
            FieldValidate(ref richTextBox_episodeFormat_Subtitle);

            richTextBox_episodeFormat_Main.Tag = new FieldTag(DBOption.cView_Episode_Main, FieldTag.Level.Episode);
            FieldValidate(ref richTextBox_episodeFormat_Main);              

            chkUseRegionalDateFormatString.Checked = DBOption.GetOptions(DBOption.cUseRegionalDateFormatString);

            tabControl_Details.SelectTab(1);
        }

        void Panel1_SizeChanged(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void InitExtraTreeAndPanes()
        {
            TreeNode nodeRoot = null;
            TreeNode nodeChild = null;
            m_paneListExtra.Add(panel_subtitleroot);

            nodeRoot = new TreeNode(panel_subtitleroot.Tag.ToString());
            nodeRoot.Name = panel_subtitleroot.Name;
            treeView_Extra.Nodes.Add(nodeRoot);
            m_paneListExtra.Add(panel_torrentroot);
            m_paneListExtra.Add(panel_torrentsearch);

            nodeRoot = new TreeNode(panel_torrentroot.Tag.ToString());
            nodeRoot.Name = panel_torrentroot.Name;
            treeView_Extra.Nodes.Add(nodeRoot);
            nodeChild = new TreeNode(panel_torrentsearch.Tag.ToString());
            nodeChild.Name = panel_torrentsearch.Name;
            nodeRoot.Nodes.Add(nodeChild);

            m_paneListExtra.Add(panel_newsroot);
            m_paneListExtra.Add(panel_newssearch);

            nodeRoot = new TreeNode(panel_newsroot.Tag.ToString());
            nodeRoot.Name = panel_newsroot.Name;
            treeView_Extra.Nodes.Add(nodeRoot);
            nodeChild = new TreeNode(panel_newssearch.Tag.ToString());
            nodeChild.Name = panel_newssearch.Name;
            nodeRoot.Nodes.Add(nodeChild);
            foreach (Control pane in m_paneListExtra)
            {
                pane.Dock = DockStyle.Fill;
                pane.Visible = false;
            }

            LoadTorrentSearches();

            if (!SkinSettings.ImportLogos) {
                lstLogos.Items.AddRange(localLogos.getFromDB().ToArray());
            }

            comboLanguage.Items.AddRange(Translation.getSupportedLangs().ToArray());
            if (comboLanguage.Items.Count == 0) comboLanguage.Enabled = false;
            else
            {
                string sel = DBOption.GetOptions(DBOption.cLanguage);
                for (int i = 0; i < comboLanguage.Items.Count; i++)
                {
                    if ((string)comboLanguage.Items[i] == sel)
                    {
                        comboLanguage.SelectedIndex = i;
                        break;
                    }
                }
            }
            comboOnlineLang_DropDown(comboOnlineLang, new EventArgs());

            LoadViews();
			// Select First Item in list
			if (_availViews.Items.Count > 0)
				_availViews.SelectedIndex = 0;

            txtMainMirror.Text = DBOption.GetOptions(DBOption.cMainMirror);

            MPTVSeriesLog.pauseAutoWriteDB = false;
            MPTVSeriesLog.selectedLogLevel = (MPTVSeriesLog.LogLevel)(int)DBOption.GetOptions("logLevel");
            this.comboLogLevel.SelectedIndex = (int)MPTVSeriesLog.selectedLogLevel;
            
			LoadNewsSearches();

            if (DBOption.GetOptions(DBOption.cSubtitleDownloaderEnabled))
            {
                subtitleDownloader_enabled.Checked = true;
            }

            int endsY = DrawSubtitleDownloaderCheckBoxes();
            DrawSubtitleLanguageCheckBoxes(endsY);
        }

        private void LoadViews()
        {
            availViews.Clear();
            availViews = logicalView.getAll(true); //include disabled
            _availViews.Items.Clear();
            foreach (logicalView view in availViews)
                _availViews.Items.Add(view.Name);

            //if (DBOption.GetOptions(DBOption.cOnlineFavourites))
            //    chkOnlineFavourites.Checked = true;
        }

        private void LoadTorrentSearches()
        {
            textBox_uTorrentPath.Text = DBOption.GetOptions(DBOption.cUTorrentPath);

            List<DBTorrentSearch> torrentSearchList = DBTorrentSearch.Get();
            foreach (DBTorrentSearch item in torrentSearchList)
            {
                comboBox_TorrentPreset.Items.Add(item);
                if (item[DBTorrentSearch.cID] == DBOption.GetOptions(DBOption.cTorrentSearch))
                    m_currentTorrentSearch = item;
            }
            if (m_currentTorrentSearch != null)
                comboBox_TorrentPreset.SelectedItem = m_currentTorrentSearch;
        }

        private void LoadNewsSearches()
        {
            textBox_newsleecher.Text = DBOption.GetOptions(DBOption.cNewsLeecherPath);
            try
            {
                List<DBNewzbin> newsSearches = DBNewzbin.Get();
                if (newsSearches.Count == 0)
                    return;

                m_currentNewsSearch = newsSearches[0];

                textBox_NewsSearchUrl.Text = m_currentNewsSearch[DBNewzbin.cSearchUrl];
                textBox_NewzbinLogin.Text = m_currentNewsSearch[DBNewzbin.cLogin];
                textbox_NewzbinPassword.Text = m_currentNewsSearch[DBNewzbin.cPassword];
                textBox_NewsSearchReportRegex.Text = m_currentNewsSearch[DBNewzbin.cSearchRegexReport];
                textBox_NewsSearchNameRegex.Text = m_currentNewsSearch[DBNewzbin.cSearchRegexName];
                textBox_NewsSearchIDRegex.Text = m_currentNewsSearch[DBNewzbin.cSearchRegexID];
                textBox_NewsSearchSizeRegex.Text = m_currentNewsSearch[DBNewzbin.cSearchRegexSize];
                textBox_NewsSearchPostDateRegex.Text = m_currentNewsSearch[DBNewzbin.cSearchRegexPostDate];
                textBox_NewsSearchReportDateRegex.Text = m_currentNewsSearch[DBNewzbin.cSearchRegexReportDate];
                textBox_NewsSearchFormatRegex.Text = m_currentNewsSearch[DBNewzbin.cSearchRegexFormat];
                textBox_NewsSearchGroupRegex.Text = m_currentNewsSearch[DBNewzbin.cSearchRegexGroup];
                textBox_NewsSearchLanguageRegex.Text = m_currentNewsSearch[DBNewzbin.cSearchRegexLanguage];
                textBox_NewsSearchIsolateArticleRegex.Text = m_currentNewsSearch[DBNewzbin.cSearchRegexIsolateArticleName];
                textBox_NewsSearchParseArticleRegex.Text = m_currentNewsSearch[DBNewzbin.cSearchRegexParseArticleName];
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Error: unable to retrieve NewsSearch settings from database: " + ex.Message);
            }

        }

        private void LoadImportPathes()
        {
            if (dataGridView_ImportPathes.Columns.Count == 0)
            {
                DataGridViewCheckBoxColumn columnEnabled = new DataGridViewCheckBoxColumn();
                columnEnabled.Name = "enabled";
                columnEnabled.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                dataGridView_ImportPathes.Columns.Add(columnEnabled);

                DataGridViewCheckBoxColumn columnRemovable = new DataGridViewCheckBoxColumn();
                columnRemovable.Name = "removable";
                columnRemovable.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                columnRemovable.ToolTipText = @"Enable this option to treat this path as removable e.g. CD\DVD-ROM, USB Drive.";
                dataGridView_ImportPathes.Columns.Add(columnRemovable);
               
                DataGridViewCheckBoxColumn columnKeepReference = new DataGridViewCheckBoxColumn();
                columnKeepReference.Name = "keep_references";
                columnKeepReference.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                columnKeepReference.ToolTipText = "Enable this option to keep reference to files in database that are not available for this import path.";
                dataGridView_ImportPathes.Columns.Add(columnKeepReference);

                DataGridViewButtonColumn columnPath = new DataGridViewButtonColumn();
                columnPath.Name = DBImportPath.cPath;
                columnPath.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView_ImportPathes.Columns.Add(columnPath);
            }

            DBImportPath[] importPathes = DBImportPath.GetAll();

            dataGridView_ImportPathes.Rows.Clear();
            
            if (importPathes != null && importPathes.Length > 0)
            {
                dataGridView_ImportPathes.Rows.Add(importPathes.Length);
                foreach (DBImportPath importPath in importPathes)
                {
                    DataGridViewRow row = dataGridView_ImportPathes.Rows[importPath[DBImportPath.cIndex]];
                    row.Cells[DBImportPath.cEnabled].Value = (Boolean)importPath[DBImportPath.cEnabled];
                    row.Cells[DBImportPath.cRemovable].Value = (Boolean)importPath[DBImportPath.cRemovable];
                    
                    if (row.Cells[DBImportPath.cRemovable].Value.ToString().ToUpper() == "TRUE")
                    {
                        row.Cells[DBImportPath.cKeepReference].Value = false;
                        row.Cells[DBImportPath.cKeepReference].ReadOnly = true;
                    }
                    else
                    {
                        row.Cells[DBImportPath.cKeepReference].Value = (Boolean)importPath[DBImportPath.cKeepReference];
                        row.Cells[DBImportPath.cKeepReference].ReadOnly = false;
                    }
                    row.Cells[DBImportPath.cPath].Value = (String)importPath[DBImportPath.cPath];
                }
            }              
        }

        private void LoadExpressions()
        {
            DBExpression[] expressions = DBExpression.GetAll();
            
            
            if (dataGridView_Expressions.Columns.Count == 0)
            {
                DataGridViewCheckBoxColumn columnEnabled = new DataGridViewCheckBoxColumn();
                columnEnabled.Name = DBExpression.cEnabled;
                columnEnabled.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                dataGridView_Expressions.Columns.Add(columnEnabled);

                DataGridViewComboBoxColumn columnType = new DataGridViewComboBoxColumn();
                columnType.Name = DBExpression.cType;
                columnType.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DataGridViewComboBoxCell comboCellTemplate = new DataGridViewComboBoxCell();
                comboCellTemplate.Items.Add(DBExpression.cType_Simple);
                comboCellTemplate.Items.Add(DBExpression.cType_Regexp);
                columnType.CellTemplate = comboCellTemplate;
                dataGridView_Expressions.Columns.Add(columnType);

                DataGridViewTextBoxColumn columnExpression = new DataGridViewTextBoxColumn();
                columnExpression.Name = DBExpression.cExpression;
                columnExpression.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                columnExpression.SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView_Expressions.Columns.Add(columnExpression);
            }
            
            // check if there were no valid expression returned
            // this shouldnt happen as the constructor should add defaults if null
            if (expressions == null) {
                DBExpression.AddDefaults();
                expressions = DBExpression.GetAll();                
                if (expressions == null) return;
            }
            
            dataGridView_Expressions.Rows.Clear();            
            dataGridView_Expressions.Rows.Add(expressions.Length);

            // load each expression into the grid
            foreach (DBExpression expression in expressions)
            {
                DataGridViewRow row = dataGridView_Expressions.Rows[expression[DBExpression.cIndex]];
                row.Cells[DBExpression.cEnabled].Value = (Boolean)expression[DBExpression.cEnabled];
                DataGridViewComboBoxCell comboCell = new DataGridViewComboBoxCell();
                comboCell.Items.Add(DBExpression.cType_Simple);
                comboCell.Items.Add(DBExpression.cType_Regexp);
                comboCell.Value = (String)expression[DBExpression.cType];
                row.Cells[DBExpression.cType] = comboCell;
                row.Cells[DBExpression.cExpression].Value = (String)expression[DBExpression.cExpression];
            }
        }

        private void LoadReplacements()
        {
            DBReplacements[] replacements = DBReplacements.GetAll();

            // load them up in the datagrid

            if (dataGridView_Replace.Columns.Count == 0)
            {
                DataGridViewCheckBoxColumn columnEnabled = new DataGridViewCheckBoxColumn();
                columnEnabled.Name = DBReplacements.cEnabled;
                columnEnabled.HeaderText = DBReplacements.PrettyFieldName(DBReplacements.cEnabled);
                columnEnabled.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                dataGridView_Replace.Columns.Add(columnEnabled);

                DataGridViewCheckBoxColumn columnTagEnabled = new DataGridViewCheckBoxColumn();
                columnTagEnabled.Name = DBReplacements.cTagEnabled;
                columnTagEnabled.HeaderText = DBReplacements.PrettyFieldName(DBReplacements.cTagEnabled);
                columnTagEnabled.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                
                dataGridView_Replace.Columns.Add(columnTagEnabled);

                DataGridViewCheckBoxColumn columnBefore = new DataGridViewCheckBoxColumn();
                columnBefore.Name = DBReplacements.cBefore;
                columnBefore.HeaderText = DBReplacements.PrettyFieldName(DBReplacements.cBefore);
                columnBefore.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                dataGridView_Replace.Columns.Add(columnBefore);

                DataGridViewTextBoxColumn columnToReplace = new DataGridViewTextBoxColumn();
                columnToReplace.Name = DBReplacements.cToReplace;
                columnToReplace.HeaderText = DBReplacements.PrettyFieldName(DBReplacements.cToReplace);
                columnToReplace.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                columnToReplace.SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView_Replace.Columns.Add(columnToReplace);

                DataGridViewTextBoxColumn columnWith = new DataGridViewTextBoxColumn();
                columnWith.Name = DBReplacements.cWith;
                columnWith.HeaderText = DBReplacements.PrettyFieldName(DBReplacements.cWith);
                columnWith.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                columnWith.SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView_Replace.Columns.Add(columnWith);
            }

            if (replacements == null) {
                DBReplacements.AddDefaults();
                replacements = DBReplacements.GetAll();
                if (replacements == null) return;                
            }

            dataGridView_Replace.Rows.Clear();
            dataGridView_Replace.Rows.Add(replacements.Length);

            foreach (DBReplacements replacement in replacements)
            {
                DataGridViewRow row = dataGridView_Replace.Rows[replacement[DBReplacements.cIndex]];
                row.Cells[DBReplacements.cEnabled].Value = (Boolean)replacement[DBReplacements.cEnabled];
                row.Cells[DBReplacements.cTagEnabled].Value = (Boolean)replacement[DBReplacements.cTagEnabled];
                row.Cells[DBReplacements.cBefore].Value = (Boolean)replacement[DBReplacements.cBefore];
                row.Cells[DBReplacements.cToReplace].Value = (String)replacement[DBReplacements.cToReplace];
                row.Cells[DBReplacements.cWith].Value = (String)replacement[DBReplacements.cWith];
            }
        }

        public void LoadTree()
        {
            if (initLoading) return;
            if (null == load) load = new loadingDisplay();
           
            TreeView root = this.treeView_Library;
            root.Nodes.Clear();
            SQLCondition condition = new SQLCondition();
            List<DBSeries> seriesList = DBSeries.Get(condition);
            load.updateStats(seriesList.Count, 0, 0);
            List<DBSeason> altSeasonList = DBSeason.Get(new SQLCondition(), false);
            load.updateStats(seriesList.Count, altSeasonList.Count, 0);            
            SQLiteResultSet results = DBTVSeries.Execute("select count(*) from online_episodes");

            load.updateStats(seriesList.Count, altSeasonList.Count, int.Parse(results.GetRow(0).fields[0]));
            aboutScreen.setUpLocalInfo(seriesList.Count, altSeasonList.Count, int.Parse(results.GetRow(0).fields[0]));

            if (seriesList.Count == 0)
            {
                load.Close();
                load = null;
                return;
            }

            foreach (DBSeries series in seriesList)
            {
                string sName = (DBOption.GetOptions(DBOption.cSeries_UseSortName) ? series[DBOnlineSeries.cSortName] : series[DBOnlineSeries.cPrettyName]);
                TreeNode seriesNode = new TreeNode(sName);
                seriesNode.Name = DBSeries.cTableName;
                seriesNode.Tag = (DBSeries)series;
                root.Nodes.Add(seriesNode);
                Font fontDefault = treeView_Library.Font;

                // set color for non-local files
                if (series[DBOnlineSeries.cEpisodeCount] == 0) {
                    seriesNode.ForeColor = System.Drawing.SystemColors.GrayText;
                } 
                else {
                    // set color for watched items
                    if (series[DBOnlineSeries.cUnwatchedItems] == 0)
                        seriesNode.ForeColor = System.Drawing.Color.DarkBlue;
                }
                
                // set FontStyle
                if (series[DBSeries.cHidden])
                    seriesNode.NodeFont = new Font(fontDefault.Name, fontDefault.Size, FontStyle.Italic);

                int seriesID = series[DBSeries.cID];            
                foreach (DBSeason season in altSeasonList)
                {
                    if (season[DBSeason.cSeriesID] == seriesID)
                    {
                        TreeNode seasonNode = null;
                        if (season[DBSeason.cIndex] == 0)
                            seasonNode = new TreeNode(Translation.specials);
                        else
                            seasonNode = new TreeNode(Translation.Season + " " + season[DBSeason.cIndex]);
                        
                        seasonNode.Name = DBSeason.cTableName;
                        seasonNode.Tag = (DBSeason)season;
                        seriesNode.Nodes.Add(seasonNode);
                        
                        // set no local files color
                        if (season[DBSeason.cEpisodeCount] == 0) {
                            seasonNode.ForeColor = System.Drawing.SystemColors.GrayText;
                        }
                        else {
                            // set color for watched season
                            if (season[DBSeason.cUnwatchedItems] == 0)
                                seasonNode.ForeColor = System.Drawing.Color.DarkBlue;
                        }

                        // set FontStyle
                        if (season[DBSeason.cHidden])
                            seasonNode.NodeFont = new Font(fontDefault.Name, fontDefault.Size, FontStyle.Italic);
                    }
                }
            }
            this.ResumeLayout();
            load.Close();
            load = null;
        }

        public void SetTreeFonts()
        {
            TreeView root = treeView_Library;
            Font fontDefault = treeView_Library.Font;
            
            if (root.Nodes.Count > 0)
            {
                //Series fonts
                for (int i = 0; i < root.Nodes.Count; i++)
                {
                    DBSeries series = (DBSeries)root.Nodes[i].Tag;
                    if (!series[DBOnlineSeries.cUnwatchedItems] && series[DBSeries.cHidden])
                        root.Nodes[i].NodeFont = new Font(fontDefault.Name, fontDefault.Size, FontStyle.Bold | FontStyle.Italic);
                    else if (!series[DBOnlineSeries.cUnwatchedItems])
                        root.Nodes[i].NodeFont = new Font(fontDefault.Name, fontDefault.Size, FontStyle.Bold);
                    else if (series[DBSeries.cHidden])
                        root.Nodes[i].NodeFont = new Font(fontDefault.Name, fontDefault.Size, FontStyle.Italic);
                    else
                        root.Nodes[i].NodeFont = new Font(fontDefault.Name, fontDefault.Size, FontStyle.Regular);

                    //Season fonts
                    if (root.Nodes[i].Nodes.Count > 0)
                    {
                        for (int j = 0; j < root.Nodes[i].Nodes.Count; j++)
                        {
                            DBSeason season = (DBSeason)root.Nodes[i].Nodes[j].Tag;
                            if (season[DBSeason.cEpisodesUnWatched] == 0 && season[DBSeason.cHidden])
                                root.Nodes[i].Nodes[j].NodeFont = new Font(fontDefault.Name, fontDefault.Size, FontStyle.Bold | FontStyle.Italic);
                            else if (season[DBSeason.cEpisodesUnWatched] == 0)
                                root.Nodes[i].Nodes[j].NodeFont = new Font(fontDefault.Name, fontDefault.Size, FontStyle.Bold);
                            else if (season[DBSeason.cHidden])
                                root.Nodes[i].Nodes[j].NodeFont = new Font(fontDefault.Name, fontDefault.Size, FontStyle.Italic);
                            else
                                root.Nodes[i].Nodes[j].NodeFont = new Font(fontDefault.Name, fontDefault.Size, FontStyle.Regular);

                            //Episode fonts
                            if (root.Nodes[i].Nodes[j].Nodes.Count > 0)
                            {
                                for (int k = 0; k < root.Nodes[i].Nodes[j].Nodes.Count; k++)
                                {
                                    DBEpisode episode = (DBEpisode)root.Nodes[i].Nodes[j].Nodes[k].Tag;
                                    if (episode[DBOnlineEpisode.cWatched] && episode[DBOnlineEpisode.cHidden])
                                        root.Nodes[i].Nodes[j].Nodes[k].NodeFont = new Font(fontDefault.Name, fontDefault.Size, FontStyle.Bold | FontStyle.Italic);
                                    else if (episode[DBOnlineEpisode.cWatched])
                                        root.Nodes[i].Nodes[j].Nodes[k].NodeFont = new Font(fontDefault.Name, fontDefault.Size, FontStyle.Bold);
                                    else if (episode[DBOnlineEpisode.cHidden])
                                        root.Nodes[i].Nodes[j].Nodes[k].NodeFont = new Font(fontDefault.Name, fontDefault.Size, FontStyle.Italic);
                                    else
                                        root.Nodes[i].Nodes[j].Nodes[k].NodeFont = new Font(fontDefault.Name, fontDefault.Size, FontStyle.Regular);
                                }
                            }
                        }
                    }
                }
            }
        }

        private int DrawSubtitleDownloaderCheckBoxes()
        {
            int counter = 1;
            int downloaderCheckboxY = 60;
            int downloaderCheckboxX = 12;

            List<String> downloaders = SubtitleDownloaderFactory.GetSubtitleDownloaderNames();
            downloaders.Sort();

            foreach (var downloader in downloaders)
            {
                var downloaderCheckbox = new System.Windows.Forms.CheckBox();
                downloaderCheckbox.AutoSize = true;
                downloaderCheckbox.Location = new System.Drawing.Point(downloaderCheckboxX, downloaderCheckboxY);
                downloaderCheckbox.Name = "subtitleDownloader_" + downloader;
                downloaderCheckbox.Text = downloader;
                downloaderCheckbox.UseVisualStyleBackColor = true;
                downloaderCheckbox.Tag = downloader;

                string checkedDownloaders =
                    DBOption.GetOptions(DBOption.cSubtitleDownloadersEnabled);

                if (checkedDownloaders.Contains((String)downloaderCheckbox.Tag))
                {
                    downloaderCheckbox.Checked = true;
                }

                downloaderCheckbox.CheckedChanged += downloaderCheckBox_CheckedChanged;

                this.subtitleDownloader_DownloaderCheckBoxes.Add(downloaderCheckbox);
                this.panel_subtitleroot.Controls.Add(downloaderCheckbox);
                downloaderCheckboxX += 130;

                if (counter % 4 == 0)
                {
                    downloaderCheckboxY += 30;
                    downloaderCheckboxX = 12;
                }
                counter++;
            }

            return downloaderCheckboxY;
        }

        private void DrawSubtitleLanguageCheckBoxes(int startY)
        {
            // Draw subtitle language checkboxes dynamically for SubtitleDownloader settings

            int counter = 1;
            int languageCheckboxY = startY + 60;
            int languageCheckboxX = 12;

            List<String> languages = Languages.GetLanguageNames();
            languages.Sort();

            foreach (var languageName in languages)
            {
                var languageCheckbox = new System.Windows.Forms.CheckBox();
                languageCheckbox.AutoSize = true;
                languageCheckbox.Location = new System.Drawing.Point(languageCheckboxX, languageCheckboxY);
                languageCheckbox.Name = "subtitleDownloader_" + languageName;
                languageCheckbox.Text = languageName;
                languageCheckbox.UseVisualStyleBackColor = true;
                languageCheckbox.Tag = Languages.GetLanguageCode(languageName);

                string checkedLanguages = 
                    DBOption.GetOptions(DBOption.cSubtitleDownloaderLanguages);

                if (checkedLanguages.Contains((String) languageCheckbox.Tag))
                {
                    languageCheckbox.Checked = true;
                }

                languageCheckbox.CheckedChanged += languageCheckBox_CheckedChanged;

                this.subtitleDownloader_LanguageCheckBoxes.Add(languageCheckbox);
                this.panel_subtitleroot.Controls.Add(languageCheckbox);
                languageCheckboxX += 125;

                if (counter % 4 == 0)
                {
                    languageCheckboxY += 30;
                    languageCheckboxX = 12;
                }
                counter++;
            }
        }

        private void languageCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            string selectedLanguages = "";

            foreach (var languageCheckbox in subtitleDownloader_LanguageCheckBoxes)
            {
                if (languageCheckbox.Checked)
                {
                    selectedLanguages += "|" + languageCheckbox.Tag;
                }
            }
            DBOption.SetOptions(DBOption.cSubtitleDownloaderLanguages, selectedLanguages);
        }

        private void downloaderCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            string selectedDownloaders = "";

            foreach (var downloaderCheckbox in subtitleDownloader_DownloaderCheckBoxes)
            {
                if (downloaderCheckbox.Checked)
                {
                    selectedDownloaders += "|" + downloaderCheckbox.Tag;
                }
            }
            DBOption.SetOptions(DBOption.cSubtitleDownloadersEnabled, selectedDownloaders);
        }

        #endregion
        
        #region Import Handling
        private void dataGridView_ImportPathes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DBImportPath importPath = new DBImportPath();
            importPath[DBImportPath.cIndex] = e.RowIndex.ToString();
            foreach (DataGridViewCell cell in dataGridView_ImportPathes.Rows[e.RowIndex].Cells)
            {   
                if (cell.Value != null)
                {
                    if (cell.ValueType == typeof(Boolean))
                        importPath[cell.OwningColumn.Name] = (Boolean)cell.Value;
                    else
                        importPath[cell.OwningColumn.Name] = (String)cell.Value;
                }
            }
            importPath.Commit();
        }
        
        private void dataGridView_ImportPathes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Note: Clicking on checkboxes does not trigger the _CellValueChanged event
                         
            if (e.RowIndex < 0)
                return;
            
            DataGridViewCell cell = dataGridView_ImportPathes.Rows[e.RowIndex].Cells[e.ColumnIndex];
            
            // Allow user to delete the row when disabling the 'enabled' checkbox
            if (e.ColumnIndex == dataGridView_ImportPathes.Columns[DBImportPath.cEnabled].Index)
            {
                // check if cell belongs to newly added row
                if (cell.Value != null)
                {
                    string sEnabled = cell.Value.ToString();
                    if (sEnabled == "True")
                    {
                        // Set value so that when checkbox is re-enabled prompt to delete will not show
                        dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cEnabled].Value = false;
                        // Prompt to delete the selected 'Import Path'
                        if (MessageBox.Show("Do you want to remove this Import Path?", "Import Paths", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                            dataGridView_ImportPathes.Rows.RemoveAt(e.RowIndex);                        
                    }
                    else 
                        dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cEnabled].Value = true;
                }
                else
                {
                    // Set default values of cells for new rows
                    // we do this so if user keeps adding empty rows, it wont throw an exception when removed
                    dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cEnabled].Value = true;
                    dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cRemovable].Value = false;                    
                    dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cKeepReference].Value = false;
                    dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cPath].Value = "";
                }
            }

            if (e.ColumnIndex == dataGridView_ImportPathes.Columns[DBImportPath.cRemovable].Index)
            {
                if (cell.Value != null)
                {
                    string sEnabled = cell.Value.ToString();
                    if (sEnabled == "True")
                    {
                        dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cRemovable].Value = false;                        
                        dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cKeepReference].ReadOnly = false;
                    }
                    else
                    {
                        dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cRemovable].Value = true;                        
                        dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cKeepReference].Value = false;
                        dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cKeepReference].ReadOnly = true;
                    }
                }
                else
                {
                    dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cEnabled].Value = true;
                    dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cRemovable].Value = true;                    
                    dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cKeepReference].Value = false;
                    dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cPath].Value = "";
                }
            }
            
            if (e.ColumnIndex == dataGridView_ImportPathes.Columns[DBImportPath.cKeepReference].Index)
            {
                if (!cell.ReadOnly)
                {
                    if (cell.Value != null)
                    {
                        string sEnabled = cell.Value.ToString();
                        if (sEnabled == "True")
                            dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cKeepReference].Value = false;
                        else
                            dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cKeepReference].Value = true;

                        if (dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cRemovable].Value.ToString().ToUpper() == "TRUE")
                        {
                            DataGridViewCheckBoxCell cbCell = (DataGridViewCheckBoxCell)dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cKeepReference];
                            dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cKeepReference].Value = false;

                        }
                    }
                    else
                    {
                        dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cEnabled].Value = true;
                        dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cRemovable].Value = true;
                        dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cKeepReference].Value = false;
                        dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cPath].Value = "";
                    }
                }
            }
            
            if (e.ColumnIndex == dataGridView_ImportPathes.Columns[DBImportPath.cPath].Index)
            {
                // Determine if user clicked on the last row, (manually add new row)
                // When user click on a checkbox column, it automatically creates new rows
                bool bNewRow = false;
                if (dataGridView_ImportPathes.NewRowIndex == e.RowIndex)
                {
                    // Add new row
                    dataGridView_ImportPathes.Rows.Add();
                    // set default values for cells
                    dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cEnabled].Value = true;
                    dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cRemovable].Value = false;
                    dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cKeepReference].Value = false;
                    bNewRow = true;
                }
                
                AddImportPathPopup importPathPopup = new AddImportPathPopup();

                // If Path is defined, set path to default in folder browser dialog
                if (dataGridView_ImportPathes.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                    importPathPopup.SelectedPath = dataGridView_ImportPathes.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                
                // Open Folder Browser Dialog 
                importPathPopup.Owner = this;
                DialogResult result = importPathPopup.ShowDialog();
                if (result == DialogResult.Cancel) {
                    // Delete this row if user didnt select a path
                    if (bNewRow)
                        dataGridView_ImportPathes.Rows.RemoveAt(e.RowIndex);
                    return;
                }
                if (result == DialogResult.OK) {
                    if (!Directory.Exists(importPathPopup.SelectedPath)) {
                        MessageBox.Show("Import path entered does not exist or is invalid.", "Import Path", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (bNewRow)
                            dataGridView_ImportPathes.Rows.RemoveAt(e.RowIndex);
                        return;
                    }
                }

                // Set Path value in cell
                dataGridView_ImportPathes.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = importPathPopup.SelectedPath;

                // Update Parsing Test
                TestParsing_Start(true);
            }

        }

        private void SaveAllImportPathes()
        {
            // need to save back all the rows
            DBImportPath.ClearAll();

            foreach (DataGridViewRow row in dataGridView_ImportPathes.Rows)
            {
                if (row.Index != dataGridView_ImportPathes.NewRowIndex)
                {
                    DBImportPath importPath = new DBImportPath();
                    importPath[DBImportPath.cIndex] = row.Index.ToString();
                    foreach (DataGridViewCell cell in row.Cells)
                        if (cell.ValueType.Name == "Boolean")
                            importPath[cell.OwningColumn.Name] = (Boolean)cell.Value;
                        else
                            importPath[cell.OwningColumn.Name] = (String)cell.Value;
                    importPath.Commit();
                }
            }
        }

        private void dataGridView_ImportPathes_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {        
            SaveAllImportPathes();
            // Update Parsing Test
            TestParsing_Start(true);
        }

        private void dataGridView_ImportPathes_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            SaveAllImportPathes();
        }
        #endregion

        #region Expressions Handling
        private void dataGridView_Expressions_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DBExpression expression = new DBExpression();
            expression[DBExpression.cIndex] = e.RowIndex.ToString();
            foreach (DataGridViewCell cell in dataGridView_Expressions.Rows[e.RowIndex].Cells)
            {
                if (cell.Value == null)
                    return;
                if (cell.ValueType.Name == "Boolean")
                    expression[cell.OwningColumn.Name] = (Boolean)cell.Value;
                else
                    expression[cell.OwningColumn.Name] = (String)cell.Value;
            }
            expression.Commit();
        }

        private void SaveAllExpressions()
        {
            // need to save back all the rows
            DBExpression.ClearAll();

            foreach (DataGridViewRow row in dataGridView_Expressions.Rows)
            {
                if (row.Index != dataGridView_Expressions.NewRowIndex)
                {
                    DBExpression expression = new DBExpression();
                    expression[DBExpression.cIndex] = row.Index.ToString();
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value == null)
                            return;
                        if (cell.ValueType.Name == "Boolean")
                            expression[cell.OwningColumn.Name] = (Boolean)cell.Value;
                        else
                            expression[cell.OwningColumn.Name] = (String)cell.Value;
                    }
                    expression.Commit();
                }
            }
        }

        private void dataGridView_Expressions_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            SaveAllExpressions();
        }

        private void button_MoveExpUp_Click(object sender, EventArgs e)
        {               
            int nCurrentRow = dataGridView_Expressions.CurrentCellAddress.Y;
            int nCurrentCol = dataGridView_Expressions.CurrentCellAddress.X;

            if (nCurrentRow > 0)
            {                           
                DBExpression expressionGoingUp = new DBExpression(nCurrentRow);
                DBExpression expressionGoingDown = new DBExpression(nCurrentRow - 1);
                
                DBExpression.Clear(nCurrentRow - 1);
                DBExpression.Clear(nCurrentRow);
                
                expressionGoingUp[DBExpression.cIndex] = Convert.ToString(nCurrentRow - 1);                                
                expressionGoingUp.Commit();

                expressionGoingDown[DBExpression.cIndex] = Convert.ToString(nCurrentRow);
                expressionGoingDown.Commit();

                LoadExpressions();

                dataGridView_Expressions.CurrentCell = dataGridView_Expressions.Rows[nCurrentRow - 1].Cells[nCurrentCol];
            }
        }

        private void button_MoveExpDown_Click(object sender, EventArgs e)
        {
            int nCurrentRow = dataGridView_Expressions.CurrentCellAddress.Y;
            int nCurrentCol = dataGridView_Expressions.CurrentCellAddress.X;

            // don't take in account the new line
            if (nCurrentRow < dataGridView_Expressions.Rows.Count - 2) 
            {
                DBExpression expressionGoingDown = new DBExpression(nCurrentRow);
                DBExpression expressionGoingUp = new DBExpression(nCurrentRow + 1);
                
                DBExpression.Clear(nCurrentRow);
                DBExpression.Clear(nCurrentRow + 1);

                expressionGoingUp[DBExpression.cIndex] = Convert.ToString(nCurrentRow);
                expressionGoingUp.Commit();

                expressionGoingDown[DBExpression.cIndex] = Convert.ToString(nCurrentRow + 1);
                expressionGoingDown.Commit();
                
                LoadExpressions();

                dataGridView_Expressions.CurrentCell = dataGridView_Expressions.Rows[nCurrentRow + 1].Cells[nCurrentCol];
            }
        }
        #endregion

        #region Replacements Handling
        private void dataGridView_Replace_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DBReplacements replacement = new DBReplacements();
            replacement[DBReplacements.cIndex] = e.RowIndex.ToString();
            foreach (DataGridViewCell cell in dataGridView_Replace.Rows[e.RowIndex].Cells)
            {
                if (cell.Value == null) {
                    return;
                }

                if (cell.ValueType.Name == "Boolean") {
                    replacement[cell.OwningColumn.Name] = (Boolean)cell.Value;
                } else {
                    replacement[cell.OwningColumn.Name] = (String)cell.Value;
                }
            }
            replacement.Commit();
        }

        void dataGridView_Replace_DefaultValuesNeeded(object sender, System.Windows.Forms.DataGridViewRowEventArgs e)
        {
            foreach (DataGridViewCell cell in e.Row.Cells) {
                //give all the check boxes a default value of false
                if (cell.ValueType.Name == "Boolean") {
                    cell.Value = false;
                }
            }
        }

        private void SaveAllReplacements()
        {
            // need to save back all the rows
            DBReplacements.ClearAll();

            foreach (DataGridViewRow row in dataGridView_Replace.Rows)
            {
                if (row.Index != dataGridView_Replace.NewRowIndex)
                {
                    DBReplacements replacement = new DBReplacements();
                    replacement[DBReplacements.cIndex] = row.Index.ToString();
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value == null)
                            return;
                        if (cell.ValueType.Name == "Boolean")
                            replacement[cell.OwningColumn.Name] = (Boolean)cell.Value;
                        else
                            replacement[cell.OwningColumn.Name] = (String)cell.Value;
                    }              
                    replacement.Commit();
                }
            }
        }

        private void dataGridView_Replace_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            SaveAllReplacements();
        }
        #endregion

        #region Test Parsing Handling
        void TestParsing_FillList(List<parseResult> results)
        {
            listView_ParsingResults.SuspendLayout();
            //IComparer sorter = listView_ParsingResults.ListViewItemSorter;
            listView_ParsingResults.ListViewItemSorter = null;
            foreach (parseResult progress in results)
            {
                foreach (KeyValuePair<String, String> MatchPair in progress.parser.Matches)
                {
                    if (!listView_ParsingResults.Columns.ContainsKey(MatchPair.Key))
                    {
                        // add a column for that match
                        ColumnHeader newcolumn = new ColumnHeader();
                        newcolumn.Name = MatchPair.Key;
                        newcolumn.Text = MatchPair.Key;
                        listView_ParsingResults.Columns.Add(newcolumn);
                    }
                }

                ListViewItem item = new ListViewItem(progress.match_filename);
                item.Tag = progress;
                item.SubItems[0].Name = listView_ParsingResults.Columns[0].Name;

                foreach (ColumnHeader column in listView_ParsingResults.Columns)
                {
                    if (column.Index > 0)
                    {
                        ListViewItem.ListViewSubItem subItem = null;
                        if (progress.parser.Matches.ContainsKey(column.Name))
                            subItem = new ListViewItem.ListViewSubItem(item, progress.parser.Matches[column.Name]);
                        else
                            subItem = new ListViewItem.ListViewSubItem(item, "");
                        subItem.Name = column.Name;
                        item.SubItems.Add(subItem);
                    }
                }

                // add in the full filename as a subitem of the list item. this is not used for the
                // actual list but is needed by the manual parser that is launched from a list listener
                ListViewItem.ListViewSubItem fullFileName = new ListViewItem.ListViewSubItem(item, progress.full_filename);
                fullFileName.Name = "FullFileName";
                item.SubItems.Add(fullFileName);

                if (progress.failedSeason)
                {
                    item.UseItemStyleForSubItems = false;
                    item.SubItems[DBEpisode.cSeasonIndex].ForeColor = System.Drawing.Color.White;
                    item.SubItems[DBEpisode.cSeasonIndex].BackColor = System.Drawing.Color.Tomato;
                }

                if (progress.failedEpisode)
                {
                    item.UseItemStyleForSubItems = false;
                    item.SubItems[DBEpisode.cEpisodeIndex].ForeColor = System.Drawing.Color.White;
                    item.SubItems[DBEpisode.cEpisodeIndex].BackColor = System.Drawing.Color.Tomato;
                }

                if (!progress.success && !progress.failedEpisode && !progress.failedSeason)
                {
                    item.ForeColor = System.Drawing.Color.White;
                    item.BackColor = System.Drawing.Color.Tomato;
                }

                if (!progress.success)
                    MPTVSeriesLog.Write("Parsing failed for " + progress.match_filename);
                if (progress.failedSeason || progress.failedEpisode)
                    MPTVSeriesLog.Write(progress.exception + " for " + progress.match_filename);
                listView_ParsingResults.Items.Add(item);
            }
            listView_ParsingResults.ListViewItemSorter = parseResult.Comparer;
            listView_ParsingResults.Sort();
            listView_ParsingResults.ResumeLayout();
            listView_ParsingResults.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            foreach (ColumnHeader header in listView_ParsingResults.Columns)
            {
                header.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                header.Width += 10;
                if (header.Width < 80)
                    header.Width = 80;
                if (header.Width > 200)
                    header.Width = 200;
            }

        }

        void TestParsing_LocalParseCompleted(List<parseResult> results)
        {
            MPTVSeriesLog.Write("Parsing test completed");
            TestParsing_FillList(results);
            this.progressBar_Parsing.Value = 100;
        }

        void TestParsing_LocalParseProgress(int nProgress, List<parseResult> results)
        {
            this.progressBar_Parsing.Value = nProgress;
            TestParsing_FillList(results);
        }

        void TestParsing_Start(bool bForceRefresh)
        {
            if (!bForceRefresh && listView_ParsingResults.Items.Count > 0 || initLoading)
                return;

            // refresh regex and replacements
            FilenameParser.reLoadExpressions();

            listView_ParsingResults.Items.Clear();
            listView_ParsingResults.Columns.Clear();
            // add mandatory columns
            ColumnHeader columnFileName = new ColumnHeader();
            columnFileName.Name = DBEpisode.cFilename;
            columnFileName.Text = "FileName";
            listView_ParsingResults.Columns.Add(columnFileName);

            ColumnHeader columnSeriesName = new ColumnHeader();
            columnSeriesName.Name = DBSeries.cParsedName;
            columnSeriesName.Text = "Parsed Series Name";
            listView_ParsingResults.Columns.Add(columnSeriesName);

            ColumnHeader columnSeasonNumber = new ColumnHeader();
            columnSeasonNumber.Name = DBEpisode.cSeasonIndex;
            columnSeasonNumber.Text = "Season ID";
            listView_ParsingResults.Columns.Add(columnSeasonNumber);

            ColumnHeader columnEpisodeNumber = new ColumnHeader();
            columnEpisodeNumber.Name = DBEpisode.cEpisodeIndex;
            columnEpisodeNumber.Text = "Episode ID";
            listView_ParsingResults.Columns.Add(columnEpisodeNumber);

            ColumnHeader columnEpisodeTitle = new ColumnHeader();
            columnEpisodeTitle.Name = DBEpisode.cEpisodeName;
            columnEpisodeTitle.Text = "Episode Title";
            listView_ParsingResults.Columns.Add(columnEpisodeTitle);

            MPTVSeriesLog.Write("Starting Parsing test, getting all files");

            LocalParse runner = new LocalParse();
            runner.LocalParseProgress += new LocalParse.LocalParseProgressHandler(TestParsing_LocalParseProgress);
            runner.LocalParseCompleted += new LocalParse.LocalParseCompletedHandler(TestParsing_LocalParseCompleted);
            runner.AsyncFullParse();
        }

        // lanches the manual parse dialog
        private void manuallyAddEpisodeMI_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem currItem in listView_ParsingResults.SelectedItems)
            {
                string filename = currItem.SubItems["FullFileName"].Text;

                ManualParseDialog parseDialog = new ManualParseDialog(filename);
                DialogResult result = parseDialog.ShowDialog(this);

                // refresh the tree socanges are visible
                if (result == DialogResult.OK)
                    this.LoadTree();
            }

        }

        #endregion

        private void Parsing_Start()
        {
            Parsing_Start(new CParsingParameters(true, true));
        }

        private void Parsing_Start(CParsingParameters parsingParams)
        {
            if (m_parser == null)
            {
                // refresh regex and replacements
                FilenameParser.reLoadExpressions();

                button_Start.Text = "Abort";
                m_timingStart = DateTime.Now;
                m_parser = new OnlineParsing(this);
                m_parser.OnlineParsingProgress += new OnlineParsing.OnlineParsingProgressHandler(runner_OnlineParsingProgress);
                m_parser.OnlineParsingCompleted += new OnlineParsing.OnlineParsingCompletedHandler(runner_OnlineParsingCompleted);
                m_parser.Start(parsingParams);
            }
        }

        void runner_OnlineParsingProgress(int nProgress)
        {
            this.progressBar_Parsing.Value = nProgress;
        }

        void runner_OnlineParsingCompleted(bool bDataUpdated)
        {
            this.progressBar_Parsing.Value = 100;
            TimeSpan span = DateTime.Now - m_timingStart;
            MPTVSeriesLog.Write("Parsing Completed in " + span);
            button_Start.Text = "Start Import";
            button_Start.Enabled = true;
            m_parser = null;
            DBOption.SetOptions(DBOption.cImport_OnlineUpdateScanLastTime, DateTime.Now.ToString());

            LoadTree();
        }

        #region Series treeview handling
        private void treeView_Library_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.dataGridView1.SuspendLayout();
            foreach (Control c in dataGridView1.Controls)
                c.SuspendLayout();
            //////////////////////////////////////////////////////////////////////////////
            #region Clears all fields so new data can be entered

            this.detailsPropertyBindingSource.Clear();
            try
            {
                if (this.pictureBox_Series.Image != null)
                {
                    this.pictureBox_Series.Image.Dispose();
                    this.pictureBox_Series.Image = null;
                }
                if (this.pictureBox_SeriesPoster.Image != null)
                {
                    this.pictureBox_SeriesPoster.Image.Dispose();
                    this.pictureBox_SeriesPoster.Image = null;
                }

            }
            catch { }

            #endregion
            //////////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////////
            #region Select appropriate tab base on which node level was clicked

            TreeNode node = e.Node;

            switch (node.Name)
            {
                //////////////////////////////////////////////////////////////////////////////
                #region When Episode Nodes is Clicked

                case DBEpisode.cTableName:
                    {
                        DBEpisode episode = (DBEpisode)node.Tag;
                       
                        comboBox_BannerSelection.Items.Clear();
                        comboBox_PosterSelection.Items.Clear();

                        // if we have logos add them to the list
                        string logos = localLogos.getLogos(ref episode, 200, 500, true);
                        if (logos.Length > 0)
                        {
                            BannerComboItem newItem = new BannerComboItem("EpisodeImage/Logos", logos);
                            comboBox_BannerSelection.Items.Add(newItem);
                            comboBox_BannerSelection.SelectedIndex = 0; // force the display
                        }
                        else
                            comboBox_BannerSelection.Enabled = false;

                        comboBox_PosterSelection.Enabled = false;

                        // go over all the fields, (and update only those which haven't been modified by the user - will do that later)
                        foreach (String key in episode.FieldNames)
                        {
                            switch (key)
                            {                                                                
                                case DBEpisode.cFilename:                                
                                    // Read Only Fields
                                    AddPropertyBindingSource(DBEpisode.PrettyFieldName(key), key, episode[key], false);
                                    break;

                                case DBEpisode.cEpisodeName:
                                    AddPropertyBindingSource(DBEpisode.PrettyFieldName(key), DBOnlineEpisode.cEpisodeName, episode[key]);
                                    break;

                                case DBOnlineEpisode.cEpisodeName:
                                case DBEpisode.cImportProcessed:
                                case DBOnlineEpisode.cOnlineDataImported:
                                    // hide these fields, they are handled internally
                                    break;
                                
                                case DBEpisode.cSeasonIndex:
                                case DBEpisode.cEpisodeIndex:
                                case DBEpisode.cEpisodeIndex2:
                                case DBEpisode.cSeriesID:
                                case DBEpisode.cCompositeID:
                                case DBEpisode.cCompositeID2:
                                case DBEpisode.cStopTime:
                                case DBEpisode.cExtension:
                                case DBEpisode.cLocalPlaytime:
                                case DBEpisode.cIsOnRemovable:
                                case DBOnlineEpisode.cHidden:
                                case DBOnlineEpisode.cID:
                                case DBOnlineEpisode.cLastUpdated:
                                case DBOnlineEpisode.cDownloadExpectedNames:
                                case DBOnlineEpisode.cDownloadPending:
                                case DBOnlineEpisode.cWatched:                                
                                case DBOnlineEpisode.cEpisodeThumbnailFilename:
                                case DBOnlineEpisode.cEpisodeThumbnailUrl:
                                case DBOnlineEpisode.cCombinedEpisodeNumber:
                                case DBOnlineEpisode.cCombinedSeason:
                                case DBOnlineEpisode.cEpisodeImageFlag:
                                case DBOnlineEpisode.cIMDBID:
                                case DBOnlineEpisode.cLanguage:
                                case DBOnlineEpisode.cProductionCode:
                                case DBOnlineEpisode.cSeasonID:
                                case DBOnlineEpisode.cDVDChapter:
                                case DBOnlineEpisode.cDVDDiscID:
                                case DBOnlineEpisode.cAbsoluteNumber:
                                    // hide these fields as we are not so interested in, 
                                    // possibly add a toggle option to display all fields later
                                    break;
                                
                                case DBEpisode.cVolumeLabel:
                                    if (String.IsNullOrEmpty(episode[key]))                                        
                                        AddPropertyBindingSource(DBEpisode.PrettyFieldName(key), key, episode[key]);
                                    break;

                                case DBOnlineEpisode.cAirsAfterSeason:
                                case DBOnlineEpisode.cAirsBeforeEpisode:
                                case DBOnlineEpisode.cAirsBeforeSeason:
                                    if (!String.IsNullOrEmpty(episode[key]))
                                        AddPropertyBindingSource(DBEpisode.PrettyFieldName(key), key, episode[key]);
                                    break;

                                case DBEpisode.cAvailableSubtitles:
                                case DBEpisode.cVideoWidth:
                                case DBEpisode.cVideoHeight:
                                case DBEpisode.cVideoFrameRate:
                                case DBEpisode.cVideoCodec:
                                case DBEpisode.cVideoBitRate:
                                case DBEpisode.cVideoAspectRatio:
                                case DBEpisode.cAudioTracks:
                                case DBEpisode.cAudioCodec:
                                case DBEpisode.cAudioChannels:
                                case DBEpisode.cAudioBitrate:
                                case DBEpisode.cFileDateAdded:
                                case DBEpisode.cFileDateCreated:
                                    if (!String.IsNullOrEmpty(episode[key]))
                                        AddPropertyBindingSource(DBEpisode.PrettyFieldName(key), key, episode[key]);
                                    break;

                                case DBEpisode.cTextCount:
                                    if (!String.IsNullOrEmpty(episode[key]) && episode[key] != "-1")
                                        AddPropertyBindingSource(DBEpisode.PrettyFieldName(key), key, episode[key]);
                                    break;

                                default:
                                    AddPropertyBindingSource(DBEpisode.PrettyFieldName(key), key, episode[key]);
                                    break;

                            }
                        }
                        // let configs now what was selected (for samples)
                        if (!SkinSettings.ImportFormatting) {
                            this.formattingConfiguration1.Series = Helper.getCorrespondingSeries(episode[DBEpisode.cSeriesID]);
                            this.formattingConfiguration1.Season = Helper.getCorrespondingSeason(episode[DBEpisode.cSeriesID], episode[DBEpisode.cSeasonIndex]);
                            this.formattingConfiguration1.Episode = episode;
                        }
                    }                    
                    break;

                #endregion
                //////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////
                #region When Season Nodes is Clicked

                case DBSeason.cTableName:
                    {
                        DBSeason season = (DBSeason)node.Tag;

                        comboBox_BannerSelection.Items.Clear();
                        comboBox_PosterSelection.Items.Clear();

                        // populate banner dropdown
                        foreach (String filename in season.BannerList)
                        {
                            BannerComboItem newItem = new BannerComboItem(Path.GetFileName(filename), filename);
                            comboBox_BannerSelection.Items.Add(newItem);
                        }
                        // if we have logos add them to the list
                        string logos = localLogos.getLogos(ref season, 200, 500);
                        if (logos.Length > 0)
                        {
                            BannerComboItem newItem = new BannerComboItem("Logos", logos);
                            comboBox_BannerSelection.Items.Add(newItem);
                        }

                        comboBox_BannerSelection.Enabled = true;
                        comboBox_PosterSelection.Enabled = false;

                        if (season.Banner.Length > 0)
                        {
                            try
                            {
                                this.pictureBox_Series.Image = ImageAllocator.LoadImageFastFromFile(season.Banner); //Image.FromFile(season.Banner);
                            }
                            catch (Exception)
                            {
                            }
                            foreach (BannerComboItem comboItem in comboBox_BannerSelection.Items)
                                if (comboItem.sFullPath == season.Banner)
                                {
                                    comboBox_BannerSelection.SelectedItem = comboItem;
                                    break;
                                }
                        }

                        // go over all the fields, (and update only those which haven't been modified by the user - will do that later)
                        foreach (String key in season.FieldNames)
                        {
                            switch (key)
                            {
                                case DBSeason.cBannerFileNames:
                                case DBSeason.cCurrentBannerFileName:
                                case DBSeason.cHasLocalFiles:
                                case DBSeason.cHasLocalFilesTemp:
                                    // hide those, they are handled internally
                                    break;

                                case DBSeason.cForomSubtitleRoot:                                                                
                                case DBSeason.cUnwatchedItems:
                                case DBSeason.cHasEpisodes:
                                case DBSeason.cHasEpisodesTemp:
                                    // hide these fields as we are not so interested in, 
                                    // possibly add a toggle option to display all fields later
                                    break;

                                default:
                                    AddPropertyBindingSource(DBSeason.PrettyFieldName(key), key, season[key], false);
                                    break;

                            }
                        }
                        // let configs now what was selected (for samples)
                        if (!SkinSettings.ImportFormatting) {
                            this.formattingConfiguration1.Series = Helper.getCorrespondingSeries(season[DBSeason.cSeriesID]);
                            this.formattingConfiguration1.Season = season;
                            this.formattingConfiguration1.Episode = null;
                        }
                    }
                    break;
                #endregion
                //////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////
                #region When Series Nodes is Clicked

                case DBSeries.cTableName:
                    {                       
                        DBSeries series = (DBSeries)node.Tag;                        

                        comboBox_BannerSelection.Items.Clear();
                        comboBox_PosterSelection.Items.Clear();

                        // populate banner dropdown with available banners
                        foreach (String filename in series.BannerList)
                        {
                            BannerComboItem newItem = new BannerComboItem(Path.GetFileName(filename), filename);
                            comboBox_BannerSelection.Items.Add(newItem);
                        }
                        comboBox_BannerSelection.Enabled = true;

                        // populate poster dropdown with available posters
                        foreach (String filename in series.PosterList)
                        {
                            BannerComboItem newItem = new BannerComboItem(Path.GetFileName(filename), filename);
                            comboBox_PosterSelection.Items.Add(newItem);
                        }
                        comboBox_PosterSelection.Enabled = true;

                        if (series.Banner.Length > 0)
                        {
                            try
                            {
                                this.pictureBox_Series.Image = ImageAllocator.LoadImageFastFromFile(series.Banner);                                
                            }
                            catch (System.Exception)
                            {

                            }
                            foreach (BannerComboItem comboItem in comboBox_BannerSelection.Items)
                            {
                                if (comboItem.sFullPath == series.Banner)
                                {
                                    comboBox_BannerSelection.SelectedItem = comboItem;
                                    break;
                                }
                            }
                        }
                        if (series.Poster.Length > 0)
                        {
                            try
                            {
                                this.pictureBox_SeriesPoster.Image = ImageAllocator.LoadImageFastFromFile(series.Banner);
                            }
                            catch (System.Exception)
                            {

                            }
                            foreach (BannerComboItem comboItem in comboBox_PosterSelection.Items)
                            {
                                if (comboItem.sFullPath == series.Poster)
                                {
                                    comboBox_PosterSelection.SelectedItem = comboItem;
                                    break;
                                }
                            }
                        }

                        // if we have logos add them to the list
                        string logos = localLogos.getLogos(ref series, 200, 500);
                        if (logos.Length > 0)
                        {
                            BannerComboItem newItem = new BannerComboItem("Logos", logos);
                            comboBox_BannerSelection.Items.Add(newItem);
                        }

                        // go over all the database fields and add to Grid View
                        foreach (String key in series.FieldNames)
                        {
                            switch (key)
                            {
                                case DBOnlineSeries.cBannerFileNames:
                                case DBOnlineSeries.cPosterFileNames:
                                case DBOnlineSeries.cBannersDownloaded:
                                case DBOnlineSeries.cCurrentBannerFileName:
                                case DBOnlineSeries.cCurrentPosterFileName:
                                case DBOnlineSeries.cHasLocalFiles:
                                case DBOnlineSeries.cHasLocalFilesTemp:
                                case DBOnlineSeries.cOnlineDataImported:
                                case DBSeries.cDuplicateLocalName:                                
                                    // hide these fields, they are handled internally
                                    break;
                                
                                case DBOnlineSeries.cGetEpisodesTimeStamp:
                                case DBOnlineSeries.cUpdateBannersTimeStamp:
                                case DBOnlineSeries.cWatchedFileTimeStamp:
                                case DBOnlineSeries.cUnwatchedItems:
                                case DBOnlineSeries.cZap2ITID:
                                case DBOnlineSeries.cIMDBID:
                                case DBOnlineSeries.cNetworkID:
                                case DBOnlineSeries.cAdded:
                                case DBOnlineSeries.cAddedBy:
                                case DBOnlineSeries.cFanart:
                                case DBOnlineSeries.cLastUpdated:
                                case DBOnlineSeries.cPoster:
                                case DBOnlineSeries.cViewTags:
                                case DBOnlineSeries.cBanner:
                                case DBOnlineSeries.cEpisodeOrders:                                
                                case DBOnlineSeries.cLanguage:
                                case DBOnlineSeries.cSeriesID:
                                case DBOnlineSeries.cOriginalName:
                                case DBSeries.cHidden:                                
                                     // hide these fields as we are not so interested in, 
                                     // possibly add a toggle option to display all fields later
                                     break;

                                case DBSeries.cParsedName:
                                case DBSeries.cID:
                                case DBOnlineSeries.cEpisodeCount:
                                case DBOnlineSeries.cEpisodesUnWatched:
                                    // fields that can not be modified - read only
                                    AddPropertyBindingSource(DBSeries.PrettyFieldName(key), key, series[key], false);
                                    break;

                                case DBOnlineSeries.cChoseEpisodeOrder:
                                    if (!String.IsNullOrEmpty(series[key]))
                                        AddPropertyBindingSource(DBSeries.PrettyFieldName(key), key, series[key]);
                                    break;

                                default:
                                    AddPropertyBindingSource(DBSeries.PrettyFieldName(key), key, series[key]);
                                    break;

                            }
                        }
                        // let formatting rules know what was selected (for samples)    
                        if (!SkinSettings.ImportFormatting) {
                            this.formattingConfiguration1.Series = series;
                            this.formattingConfiguration1.Season = null;
                            this.formattingConfiguration1.Episode = null;
                        }
                    }
                    break;

                #endregion
                //////////////////////////////////////////////////////////////////////////////

            }
            #endregion
            //////////////////////////////////////////////////////////////////////////////
            this.dataGridView1.ResumeLayout();
            foreach (Control c in dataGridView1.Controls)
                c.ResumeLayout();
        }

        private void treeView_Library_AfterExpand(object sender, TreeViewEventArgs e) {
            TreeNode node = e.Node;
            if (node.Level != 0) return;

            Font defaultFont = treeView_Library.Font;
            DBSeries series = (DBSeries)node.Tag;
            int seriesID = series[DBSeries.cID];

            //////////////////////////////////////////////////////////////////////////////
            #region Load Episodes into season tree child nodes of expanding series
            foreach (TreeNode childNode in node.Nodes) {
                // Check if we have already loaded episodes into season nodes
                if (childNode.Nodes.Count == 0) {
                    DBSeason season = (DBSeason)childNode.Tag;
                    int seasonIndex = season[DBSeason.cIndex];

                    List<DBEpisode> episodes = DBEpisode.Get(seriesID, seasonIndex);

                    foreach (DBEpisode episode in episodes) {
                        String episodeName = (String)episode[DBEpisode.cEpisodeName];
                        TreeNode episodeNode = new TreeNode(episode[DBEpisode.cSeasonIndex] + "x" + episode[DBEpisode.cEpisodeIndex] + " - " + episodeName);
                        episodeNode.Name = DBEpisode.cTableName;
                        episodeNode.Tag = (DBEpisode)episode;

                        // set color for non-local file
                        if (episode[DBEpisode.cFilename].ToString().Length == 0) {
                            episodeNode.ForeColor = System.Drawing.SystemColors.GrayText;
                        }
                        else {
                            // set color for watched episode
                            if (episode[DBOnlineEpisode.cWatched] == 1)
                                episodeNode.ForeColor = System.Drawing.Color.DarkBlue;
                        }

                        // set FontStyle for hidden episodes
                        if (episode[DBOnlineEpisode.cHidden])
                            episodeNode.NodeFont = new Font(defaultFont.Name, defaultFont.Size, FontStyle.Italic);

                        childNode.Nodes.Add(episodeNode);
                    }
                }
            }

            #endregion
            //////////////////////////////////////////////////////////////////////////////
        }

        private void AddPropertyBindingSource(string FieldPrettyName, string FieldName, string FieldValue)
        {
            AddPropertyBindingSource(FieldPrettyName, FieldName, FieldValue, true, DataGridViewContentAlignment.MiddleLeft);
        }

        private void AddPropertyBindingSource(string FieldPrettyName, string FieldName, string FieldValue, bool CanModify)
        {
            AddPropertyBindingSource(FieldPrettyName, FieldName, FieldValue, CanModify, DataGridViewContentAlignment.MiddleLeft);
        }

        private void AddPropertyBindingSource(string FieldPrettyName, string FieldName, string FieldValue, bool CanModify, DataGridViewContentAlignment TextAlign)
        {
            // Add new Row
            int id = this.detailsPropertyBindingSource.Add(new DetailsProperty(FieldPrettyName, FieldValue));

            // First Column (Name)
            DataGridViewCell cell = this.dataGridView1.Rows[id].Cells[0];
            cell.Style.BackColor = System.Drawing.SystemColors.Control;
            cell.ReadOnly = true;

            // Second Column (Value)
            cell = this.dataGridView1.Rows[id].Cells[1];
            cell.Tag = FieldName;
            if (!CanModify)
            {
                cell.ReadOnly = true;
                cell.Style.BackColor = System.Drawing.SystemColors.Control;
            }

            cell.Style.Alignment = TextAlign;
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            nodeEdited = treeView_Library.SelectedNode;
            /*
            if (this.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString() == "Filename")
            {
                openFileDialog1.FileName = this.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                openFileDialog1.ShowDialog();
                if (this.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString() != openFileDialog1.FileName)
                {
                    this.dataGridView1.Rows[e.RowIndex].Cells[1].Value = openFileDialog1.FileName;
                    m_PropertySaveRequired = true;
                }
                e.Cancel = true;
                return;
            }

            if (this.treeView_Library.Nodes.Count > 0)
                m_PropertySaveRequired = true;
            */
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[1];
            if (nodeEdited != null)
            {
                switch (nodeEdited.Name)
                {
                    case DBSeries.cTableName:
                        DBSeries series = (DBSeries)nodeEdited.Tag;
                        series[(String)cell.Tag] = (String)cell.Value;
                        series.Commit();
                        if (series[DBOnlineSeries.cPrettyName].ToString().Length > 0)
                            nodeEdited.Text = series[DBOnlineSeries.cPrettyName];
                        break;

                    case DBSeason.cTableName:
                        DBSeason season = (DBSeason)nodeEdited.Tag;
                        season[(String)cell.Tag] = (String)cell.Value;
                        season.Commit();
                        break;

                    case DBEpisode.cTableName:
                        DBEpisode episode = (DBEpisode)nodeEdited.Tag;
                        episode[(String)cell.Tag] = (String)cell.Value;
                        episode.Commit();
                        if (episode[DBEpisode.cEpisodeName].ToString().Length > 0)
                            nodeEdited.Text = episode[DBEpisode.cSeasonIndex] + "x" + episode[DBEpisode.cEpisodeIndex] + " - " + episode[DBEpisode.cEpisodeName];
                        break;
                }
            }
        }
        #endregion

        #region UI actions

        private void treeView_Settings_AfterSelect(object sender, TreeViewEventArgs e)
        {
            foreach (Control pane in m_paneListSettings)
            {
                if (pane.Name == e.Node.Name)
                {
                    pane.Visible = true;
                }
                else
                    pane.Visible = false;
            }

            // special behavior for some nodes         
            if (e.Node.Name == this.panel_manualEpisodeManagement.Name)
                panel_manualEpisodeManagement.refreshFileList();
        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            if (m_parser != null) {
                m_parser.Cancel();
                button_Start.Enabled = false;
            } else
                Parsing_Start();
        }

        private void button_TestReparse_Click(object sender, EventArgs e)
        {
            TestParsing_Start(true);
        }
        #endregion

        private void checkBox_OnlineSearch_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cOnlineParseEnabled, checkBox_OnlineSearch.Checked);
        }

        private void checkBox_FullSeriesRetrieval_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cFullSeriesRetrieval, checkBox_FullSeriesRetrieval.Checked);
        }

        private void checkBox_AutoChooseSeries_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cAutoChooseSeries, checkBox_AutoChooseSeries.Checked);
        }

        private void comboBox_BannerSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (treeView_Library.SelectedNode.Name)
            {
                case DBSeries.cTableName:
                    {
                        DBSeries series = (DBSeries)treeView_Library.SelectedNode.Tag;
                        if (((BannerComboItem)comboBox_BannerSelection.SelectedItem).sName != "Logos")
                            series.Banner = ((BannerComboItem)comboBox_BannerSelection.SelectedItem).sFullPath;
                        try
                        {
                            this.pictureBox_Series.Image = ImageAllocator.LoadImageFastFromFile(ImageAllocator.ExtractFullName(((BannerComboItem)comboBox_BannerSelection.SelectedItem).sFullPath)); // Image.FromFile(ImageAllocator.ExtractFullName(((BannerComboItem)comboBox_BannerSelection.SelectedItem).sFullPath));
                        }
                        catch (Exception)
                        {
                        }
                        series.Commit();
                    }
                    break;

                case DBSeason.cTableName:
                    {
                        DBSeason season = (DBSeason)treeView_Library.SelectedNode.Tag;
                        if (((BannerComboItem)comboBox_BannerSelection.SelectedItem).sName != "Logos")
                            season.Banner = ((BannerComboItem)comboBox_BannerSelection.SelectedItem).sFullPath;
                        try
                        {
                            this.pictureBox_Series.Image = ImageAllocator.LoadImageFastFromFile(ImageAllocator.ExtractFullName(((BannerComboItem)comboBox_BannerSelection.SelectedItem).sFullPath)); // Image.FromFile(ImageAllocator.ExtractFullName(((BannerComboItem)comboBox_BannerSelection.SelectedItem).sFullPath));
                        }
                        catch (Exception)
                        {
                        }
                        season.Commit();
                    }
                    break;

                case DBEpisode.cTableName:
                    {
                        // that was actually a BAD bug ever since logos/ep images, surprisingly nobody found it?
                        // we can't change anythign here, there is exactly 1 ep images/Logos item

                        //DBSeries series = (DBSeries)treeView_Library.SelectedNode.Parent.Parent.Tag;
                        //if (((BannerComboItem)comboBox_BannerSelection.SelectedItem).sName != "Logos")
                        //    series.Banner = ((BannerComboItem)comboBox_BannerSelection.SelectedItem).sFullPath;
                        try
                        {
                            this.pictureBox_Series.Image = ImageAllocator.LoadImageFastFromFile(ImageAllocator.ExtractFullName(((BannerComboItem)comboBox_BannerSelection.SelectedItem).sFullPath)); // Image.FromFile(ImageAllocator.ExtractFullName(((BannerComboItem)comboBox_BannerSelection.SelectedItem).sFullPath));
                        }
                        catch (Exception)
                        {
                        }
                        //series.Commit();
                    }
                    break;
            }

        }

        private void checkBox_Episode_MatchingLocalFile_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cView_Episode_OnlyShowLocalFiles, checkBox_Episode_OnlyShowLocalFiles.Checked);
        }

        private void checkBox_Episode_HideUnwatchedSummary_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cView_Episode_HideUnwatchedSummary, checkBox_Episode_HideUnwatchedSummary.Checked);
        }

        private void checkBox_AutoOnlineDataRefresh_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cImport_AutoUpdateOnlineData, checkBox_AutoOnlineDataRefresh.Checked);
            numericUpDown_AutoOnlineDataRefresh.Enabled = checkBox_AutoOnlineDataRefresh.Checked;
        }

        private void numericUpDown_AutoOnlineDataRefresh_ValueChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cImport_AutoUpdateOnlineDataLapse, (int)numericUpDown_AutoOnlineDataRefresh.Value);
        }

        private void HideNode(TreeNode nodeHidden)
        {
            if (nodeHidden != null)
            {
                bool bHidden = false;
                switch (nodeHidden.Name)
                {
                    case DBSeries.cTableName:
                        DBSeries series = (DBSeries)nodeHidden.Tag;
                        series[DBSeries.cHidden] = !series[DBSeries.cHidden];
                        bHidden = series[DBSeries.cHidden];
                        series.Commit();
                        break;

                    case DBSeason.cTableName:
                        DBSeason season = (DBSeason)nodeHidden.Tag;
                        season[DBSeason.cHidden] = !season[DBSeason.cHidden];
                        bHidden = season[DBSeason.cHidden];
                        season.Commit();
                        break;

                    case DBEpisode.cTableName:
                        DBEpisode episode = (DBEpisode)nodeHidden.Tag;
                        episode[DBOnlineEpisode.cHidden] = !episode[DBOnlineEpisode.cHidden];
                        bHidden = episode[DBOnlineEpisode.cHidden];
                        episode.Commit();
                        break;
                }

                if (DBOption.GetOptions(DBOption.cShowHiddenItems))
                {
                    // change the font
                    if (bHidden)
                    {
                        Font fontDefault = treeView_Library.Font;
                        nodeHidden.NodeFont = new Font(fontDefault.Name, fontDefault.Size, FontStyle.Italic);
                    }
                    else
                    {
                        nodeHidden.NodeFont = treeView_Library.Font;
                    }
                }
                else
                {
                    // just remove the node
                    treeView_Library.Nodes.Remove(nodeHidden);
                }
            }
        }

        private void UpdateNode(TreeNode nodeUpdated)
        {
            if (nodeUpdated != null) {
                List<DBValue> epIdsUpdates = new List<DBValue>();
                List<DBValue> seriesIDsUpdates = new List<DBValue>();

                switch (nodeUpdated.Name) {
                    case DBSeries.cTableName: {
                            DBSeries series = nodeUpdated.Tag as DBSeries;
                            seriesIDsUpdates.Add(series[DBSeries.cID]);
                            SQLCondition cond = new SQLCondition(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                            epIdsUpdates.AddRange(DBEpisode.GetSingleField(DBOnlineEpisode.cID, cond, new DBOnlineEpisode()));
                        }
                        break;

                    case DBSeason.cTableName: {
                            DBSeason season = nodeUpdated.Tag as DBSeason;
                            SQLCondition cond = new SQLCondition(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, season[DBSeason.cSeriesID], SQLConditionType.Equal);
                            cond.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeasonIndex, season[DBSeason.cIndex], SQLConditionType.Equal);
                            epIdsUpdates.AddRange(DBEpisode.GetSingleField(DBOnlineEpisode.cID, cond, new DBOnlineEpisode()));
                        }
                        break;

                    case DBEpisode.cTableName: {
                            DBEpisode episode = nodeUpdated.Tag as DBEpisode;
                            epIdsUpdates.Add(episode[DBOnlineEpisode.cID]);
                        }
                        break;
                }
                if (epIdsUpdates.Count > 0) {
                    Parsing_Start((new CParsingParameters(new List<ParsingAction> { ParsingAction.UpdateSeries, ParsingAction.UpdateEpisodes, ParsingAction.UpdateEpisodeThumbNails }, seriesIDsUpdates, epIdsUpdates)));
                }
            }
        }

        private void RescanMediaInfoNode(TreeNode nodeMediaInfo)
        {
            if (nodeMediaInfo == null) return;

            List<DBEpisode> episodes = new List<DBEpisode>();

            switch (nodeMediaInfo.Name)
            {
                case DBEpisode.cTableName:
                    DBEpisode episode = nodeMediaInfo.Tag as DBEpisode;
                    episodes.Add(episode);
                    break;

                case DBSeason.cTableName:
                    DBSeason season = nodeMediaInfo.Tag as DBSeason;
                    episodes = DBEpisode.Get(season[DBSeason.cSeriesID], season[DBSeason.cIndex], false);                    
                    break;

                case DBSeries.cTableName:
                    DBSeries series = nodeMediaInfo.Tag as DBSeries;
                    episodes = DBEpisode.Get((int)series[DBSeries.cID], false);                    
                    break;
            }
            UpdateMediaInfoASync(episodes);
        }

        private void DeleteNode(TreeNode nodeDeleted)
        {
            if (nodeDeleted != null) {
                List<string> resultMsg = null;
                string msgDlgCaption = string.Empty;

                bool hasSubtitles = false;
                if (nodeDeleted.Name == DBEpisode.cTableName)
                {
                    DBEpisode episode = (DBEpisode)nodeDeleted.Tag;
                    if (episode != null && episode.checkHasLocalSubtitles())
                        hasSubtitles = true;
                }

                DeleteDialog deleteDialog = new DeleteDialog(hasSubtitles);
                DialogResult result = deleteDialog.ShowDialog(this);

                // nothing to do exit
                if (result != DialogResult.OK) return;

                #region Delete Subtitles
                if (deleteDialog.DeleteMode == TVSeriesPlugin.DeleteMenuItems.subtitles)
                {
                    msgDlgCaption = Translation.UnableToDeleteSubtitles;
                    switch (nodeDeleted.Name)
                    {
                        case DBEpisode.cTableName:
                            DBEpisode episode = (DBEpisode)nodeDeleted.Tag;
                            if (episode == null) return;
                            resultMsg = episode.deleteLocalSubTitles();
                            break;
                    }
                    try
                    {
                        TreeNode selectedTN = treeView_Library.SelectedNode;
                        treeView_Library.SelectedNode = null;
                        treeView_Library.SelectedNode = selectedTN;
                    }
                    catch
                    {
                    }                   
                }
                #endregion

                #region Delete From Disk, Database or Both
                if (deleteDialog.DeleteMode != TVSeriesPlugin.DeleteMenuItems.subtitles)
                {
                    msgDlgCaption = Translation.UnableToDelete;
                    switch (nodeDeleted.Name)
                    {
                        #region Delete Series
                        case DBSeries.cTableName:
                            if (result == DialogResult.OK)
                            {
                                DBSeries series = (DBSeries)nodeDeleted.Tag;
                                resultMsg = series.deleteSeries(deleteDialog.DeleteMode);
                            }
                            break;
                        #endregion

                        #region Delete Season
                        case DBSeason.cTableName:
                            if (result == DialogResult.OK)
                            {
                                DBSeason season = (DBSeason)nodeDeleted.Tag;
                                resultMsg = season.deleteSeason(deleteDialog.DeleteMode);
                            }
                            break;
                        #endregion

                        #region Delete Episode
                        case DBEpisode.cTableName:
                            if (result == DialogResult.OK)
                            {
                                DBEpisode episode = (DBEpisode)nodeDeleted.Tag;
                                resultMsg = episode.deleteEpisode(deleteDialog.DeleteMode);
                            }
                            break;
                        #endregion
                    }

                    // Delete tree node
                    if (resultMsg.Count == 0 && deleteDialog.DeleteMode != TVSeriesPlugin.DeleteMenuItems.disk)
                        treeView_Library.Nodes.Remove(nodeDeleted);

                    if (treeView_Library.Nodes.Count == 0)
                    {
                        // also clear the data pane
                        this.detailsPropertyBindingSource.Clear();
                        try
                        {
                            if (this.pictureBox_Series.Image != null)
                            {
                                this.pictureBox_Series.Image.Dispose();
                                this.pictureBox_Series.Image = null;
                            }
                        }
                        catch { }
                    }
                }
                #endregion

                // Show errors, if any
                if (resultMsg != null && resultMsg.Count > 0)
                {
                    MessageBox.Show(string.Join("\n", resultMsg.ToArray()), msgDlgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
        }

        private void ToggleWatchedNode(TreeNode nodeWatched, int watched)
        {
            if (nodeWatched != null)
            {
                Font fontDefault = treeView_Library.Font;

                switch (nodeWatched.Name)
                {
                    case DBSeries.cTableName:
                        DBSeries series = (DBSeries)nodeWatched.Tag;
                        DBTVSeries.Execute("update online_episodes set watched = " + watched + " where " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID) + " = " + series[DBSeries.cID]);
                        //DBTVSeries.Execute("update season set " + DBSeason.cUnwatchedItems + " = 0 and " + DBSeason.cEpisodesUnWatched + " = 0 where " + DBSeason.Q(DBSeason.cSeriesID) + " = " + series[DBSeries.cID]);
                        //DBTVSeries.Execute("update online_series set " + DBOnlineSeries.cUnwatchedItems + " = 0 and " + DBOnlineSeries.cEpisodesUnWatched + " = 0 where " + DBOnlineSeries.Q(DBOnlineSeries.cID) + " = " + series[DBSeries.cID]);
                        //series[DBOnlineSeries.cUnwatchedItems] = false;
                        //series.Commit();
                        // Updated Episode Counts
                        DBSeries.UpdateEpisodeCounts(series);

                        if (nodeWatched.Nodes.Count > 0)
                        {
                            for (int i = 0; i < nodeWatched.Nodes.Count; i++)
                            {
                                //Child Season fonts:
                                DBSeason s = (DBSeason)nodeWatched.Nodes[i].Tag;
                                if (watched == 1)
                                    nodeWatched.Nodes[i].ForeColor = System.Drawing.Color.DarkBlue;
                                else nodeWatched.Nodes[i].ForeColor = treeView_Library.ForeColor;

                                //Child Episode fonts:
                                if (nodeWatched.Nodes[i].Nodes.Count > 0)
                                {
                                    for (int j = 0; j < nodeWatched.Nodes[i].Nodes.Count; j++)
                                    {
                                        DBEpisode ep = (DBEpisode)nodeWatched.Nodes[i].Nodes[j].Tag;
                                        if (watched == 1)
                                            nodeWatched.Nodes[i].Nodes[j].ForeColor = System.Drawing.Color.DarkBlue;
                                        else nodeWatched.Nodes[i].Nodes[j].ForeColor = treeView_Library.ForeColor;
                                    }
                                }
                            }
                        }

                        cache.dump();

                        break;

                    case DBSeason.cTableName:
                        DBSeason season = (DBSeason)nodeWatched.Tag;
                        DBTVSeries.Execute("update online_episodes set watched = " + watched + " where " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID) + " = " + season[DBSeason.cSeriesID] +
                                            " and " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeasonIndex) + " = " + season[DBSeason.cIndex]);
                        //season[DBSeason.cUnwatchedItems] = false;
                        //season.Commit();
                        DBSeries series2 = DBSeries.Get(season[DBSeason.cSeriesID]);
                        DBSeason.UpdateEpisodeCounts(series2, season);

                        //Parent Series color:
                        if (series2[DBOnlineSeries.cUnwatchedItems] == 0)
                            nodeWatched.Parent.ForeColor = System.Drawing.Color.DarkBlue;
                        else nodeWatched.Parent.ForeColor = treeView_Library.ForeColor;
                        
                        //Child Episodes color:
                        if (nodeWatched.Nodes.Count > 0)
                        {
                            for (int i = 0; i < nodeWatched.Nodes.Count; i++)
                            {
                                DBEpisode ep = (DBEpisode)nodeWatched.Nodes[i].Tag;
                                if (watched == 1)
                                    nodeWatched.Nodes[i].ForeColor = System.Drawing.Color.DarkBlue;
                                else nodeWatched.Nodes[i].ForeColor = treeView_Library.ForeColor;
                            }
                        }

                        cache.dump();
                        
                        break;

                    case DBEpisode.cTableName:
                        DBEpisode episode = (DBEpisode)nodeWatched.Tag;
                        DBTVSeries.Execute("update online_episodes set watched = " + watched + " where " + DBOnlineEpisode.Q(DBOnlineEpisode.cCompositeID) + " = \"" + episode[DBEpisode.cCompositeID] + "\"");/* +
                                            " and " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeasonIndex) + " = " + episode[DBEpisode.cSeasonIndex] +
                                            " and " + DBOnlineEpisode.Q(DBOnlineEpisode.cEpisodeIndex) + " = " + episode[DBEpisode.cEpisodeIndex]);*/
                        //episode.Commit();
                        DBSeries series3 = DBSeries.Get(episode[DBEpisode.cSeriesID]);
                        DBSeason season3 = Helper.getCorrespondingSeason(episode[DBEpisode.cSeriesID], episode[DBEpisode.cSeasonIndex]);
                        DBSeason.UpdateEpisodeCounts(series3, season3);

                        //Parent Series color
                        if (series3[DBOnlineSeries.cUnwatchedItems] == 0)
                            nodeWatched.Parent.ForeColor = System.Drawing.Color.DarkBlue;
                        else nodeWatched.Parent.ForeColor = treeView_Library.ForeColor;

                        //Parent Season color
                        if (season3[DBSeason.cUnwatchedItems] == 0)
                            nodeWatched.Parent.Parent.ForeColor = System.Drawing.Color.DarkBlue;
                        else nodeWatched.Parent.Parent.ForeColor = treeView_Library.ForeColor;

                        cache.dump();

                        break;
                }
                //reload tree? - need to just refresh the pane on the right with new database information since we changed it!
                //this.dataGridView1.Refresh();

                //set color of nodeWatched
                if (watched == 1)
                    nodeWatched.ForeColor = System.Drawing.Color.DarkBlue;
                else nodeWatched.ForeColor = treeView_Library.ForeColor;

                //Refresh treeView
                this.treeView_Library.ResumeLayout();
            }
        }
        
        private void treeView_Library_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteNode(treeView_Library.SelectedNode);
            }
        }

        private void FieldValidate(ref RichTextBox textBox)
        {
            FieldTag tag = textBox.Tag as FieldTag;
            if (!tag.m_bInited)
            {
                textBox.Text = DBOption.GetOptions(tag.m_sOptionName);
                tag.m_bInited = true;
            }

            int nCarret = textBox.SelectionStart;
            String s = textBox.Text;
            Color defColor = textBox.ForeColor;

            int nStart = 0;
            while (s.Length != 0)
            {
                int nTagStart = s.IndexOf('<');
                if (nTagStart != -1)
                {
                    String sCurrent = s.Substring(0, nTagStart);
                    s = s.Substring(nTagStart);

                    textBox.SelectionStart = nStart;
                    textBox.SelectionLength = sCurrent.Length;
                    textBox.SelectionColor = defColor;
                    nStart += sCurrent.Length;

                    int nTagEnd = s.IndexOf('>');
                    if (nTagEnd != -1)
                    {
                        sCurrent = s.Substring(0, nTagEnd + 1);
                        s = s.Substring(nTagEnd + 1);

                        bool bValid = false;
                        textBox.SelectionStart = nStart;
                        textBox.SelectionLength = sCurrent.Length;

                        // find out of the tag exists in the table(s)
                        String sTag = sCurrent.Substring(1, sCurrent.Length - 2);
                        if (sTag.IndexOf('.') != -1)
                        {
                            String sTableName = sTag.Substring(0, sTag.IndexOf('.'));
                            String sFieldName = sTag.Substring(sTag.IndexOf('.') + 1);

                            // unwatchedItems isnt in fieldnames since its purely virtual
                            bValid |= ((sFieldName == DBOnlineSeries.cUnwatchedItems && tag.m_Level == FieldTag.Level.Series) ||
                               (sFieldName == DBSeason.cUnwatchedItems && tag.m_Level == FieldTag.Level.Season));
                            // same for filesizes
                            bValid |= ((sFieldName == DBEpisode.cFileSize && tag.m_Level == FieldTag.Level.Episode) ||
                               (sFieldName == DBEpisode.cFileSizeBytes && tag.m_Level == FieldTag.Level.Episode));
                            // and prettyPlaytime
                            bValid |= (sFieldName == DBEpisode.cPrettyPlaytime && tag.m_Level == FieldTag.Level.Episode);
                            // and cFilenameWOPath
                            bValid |= (sFieldName == DBEpisode.cFilenameWOPath && tag.m_Level == FieldTag.Level.Episode);

                            switch (tag.m_Level)
                            {
                                case FieldTag.Level.Series:
                                    if (sTableName == DBSeries.cOutName)
                                        bValid |= m_SeriesReference.FieldNames.Contains(sFieldName);
                                    break;

                                case FieldTag.Level.Season:
                                    if (sTableName == DBSeries.cOutName)
                                        bValid |= m_SeriesReference.FieldNames.Contains(sFieldName);
                                    if (sTableName == DBSeason.cOutName)
                                        bValid |= m_SeasonReference.FieldNames.Contains(sFieldName);
                                    break;

                                case FieldTag.Level.Episode:
                                    if (sTableName == DBSeries.cOutName)
                                        bValid |= m_SeriesReference.FieldNames.Contains(sFieldName);
                                    if (sTableName == DBSeason.cOutName)
                                        bValid |= m_SeasonReference.FieldNames.Contains(sFieldName);
                                    if (sTableName == DBEpisode.cOutName)
                                        bValid |= m_EpisodeReference.FieldNames.Contains(sFieldName);
                                    break;
                            }
                        }

                        if (bValid)
                            textBox.SelectionColor = Color.Green;
                        else
                            textBox.SelectionColor = Color.Red;
                        nStart += sCurrent.Length;

                    }
                    else
                    {
                        // no more closing tag, no good, red
                        textBox.SelectionStart = nStart;
                        textBox.SelectionLength = textBox.Text.Length - nStart;
                        textBox.SelectionColor = Color.Tomato;
                        s = String.Empty;
                    }
                }
                else
                {
                    // no more opening tag
                    textBox.SelectionStart = nStart;
                    textBox.SelectionLength = textBox.Text.Length - nStart;
                    textBox.SelectionColor = defColor;
                    s = String.Empty;
                }
            }

            textBox.SelectionLength = 0;
            textBox.SelectionStart = nCarret;

            DBOption.SetOptions(tag.m_sOptionName, textBox.Text);
        }

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            RichTextBox textBox = sender as RichTextBox;
            FieldValidate(ref textBox);
        }

        private void contextMenuStrip_SeriesFields_Opening(object sender, CancelEventArgs e)
        {
            // Acquire references to the owning control and item.
            RichTextBox textBox = contextMenuStrip_InsertFields.SourceControl as RichTextBox;

            // Clear the ContextMenuStrip control's Items collection.
            contextMenuStrip_InsertFields.Items.Clear();
            contextMenuStrip_InsertFields.CanOverflow = true;

            contextMenuStrip_InsertFields.Items.Add("Add a field Value:");
            contextMenuStrip_InsertFields.Items[0].Enabled = false;
            // Populate the ContextMenuStrip control with its default items.
            contextMenuStrip_InsertFields.Items.Add("-");
            contextMenuStrip_InsertFields.Items[1].Enabled = false;

            FieldTag tag = textBox.Tag as FieldTag;

            // series' always there
            {
                ToolStripMenuItem subMenuItem = new ToolStripMenuItem(DBSeries.cOutName + " values");
                ContextMenuStrip subMenu = new ContextMenuStrip(this.components);
                subMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
                subMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip_SeriesFields_ItemClicked);
                subMenuItem.DropDown = subMenu;
                List<String> fieldList = (List<String>) m_SeriesReference.FieldNames;
                fieldList.Remove(DBOnlineSeries.cHasLocalFiles);
                fieldList.Remove(DBOnlineSeries.cHasLocalFilesTemp);
                fieldList.Remove(DBOnlineSeries.cBannerFileNames);
                fieldList.Remove(DBOnlineSeries.cPosterFileNames);
                fieldList.Remove(DBOnlineSeries.cBannersDownloaded);
                fieldList.Remove(DBOnlineSeries.cCurrentBannerFileName);
                fieldList.Remove(DBOnlineSeries.cCurrentPosterFileName);
                fieldList.Remove(DBOnlineSeries.cOnlineDataImported);
                fieldList.Remove(DBOnlineSeries.cGetEpisodesTimeStamp);
                fieldList.Remove(DBOnlineSeries.cUpdateBannersTimeStamp);
                fieldList.Remove(DBSeries.cScanIgnore);
                fieldList.Remove(DBSeries.cHidden);
                fieldList.Remove(DBSeries.cDuplicateLocalName);


                foreach (String sField in fieldList)
                {
                    ToolStripItem item = new ToolStripLabel();
                    item.Name = "<" + DBSeries.cOutName + "." + sField + ">";
                    item.Tag = textBox;
                    String sPretty = DBSeries.PrettyFieldName(sField);
                    if (sPretty == sField)
                        item.Text = item.Name;
                    else
                        item.Text = item.Name + " - (" + sPretty + ")";
                    subMenu.Items.Add(item);
                }
                contextMenuStrip_InsertFields.Items.Add(subMenuItem);
            }

            // season
            if (tag.m_Level == FieldTag.Level.Season || tag.m_Level == FieldTag.Level.Episode)
            {
                ToolStripMenuItem subMenuItem = new ToolStripMenuItem(DBSeason.cOutName + " values");
                ContextMenuStrip subMenu = new ContextMenuStrip(this.components);
                subMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
                subMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip_SeriesFields_ItemClicked);
                subMenuItem.DropDown = subMenu;
                List<String> fieldList = (List<String>)m_SeasonReference.FieldNames;
                fieldList.Remove(DBSeason.cHasLocalFiles);
                fieldList.Remove(DBSeason.cHasLocalFilesTemp);
                fieldList.Remove(DBSeason.cHasEpisodes);
                fieldList.Remove(DBSeason.cHasEpisodesTemp);
                fieldList.Remove(DBSeason.cBannerFileNames);
                fieldList.Remove(DBSeason.cCurrentBannerFileName);
                fieldList.Remove(DBSeason.cHidden);
                foreach (String sField in fieldList)
                {
                    ToolStripItem item = new ToolStripLabel();
                    item.Name = "<" + DBSeason.cOutName + "." + sField + ">";
                    item.Tag = textBox;
                    String sPretty = DBSeason.PrettyFieldName(sField);
                    if (sPretty == sField)
                        item.Text = item.Name;
                    else
                        item.Text = item.Name + " - (" + sPretty + ")";
                    subMenu.Items.Add(item);
                }
                contextMenuStrip_InsertFields.Items.Add(subMenuItem);
            }

            // episode
            if (tag.m_Level == FieldTag.Level.Episode)
            {
                ToolStripMenuItem subMenuItem = new ToolStripMenuItem(DBEpisode.cOutName + " values");
                ContextMenuStrip subMenu = new ContextMenuStrip(this.components);
                subMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
                subMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip_SeriesFields_ItemClicked);
                subMenuItem.DropDown = subMenu;
                List<String> fieldList = (List<String>)m_EpisodeReference.FieldNames;
                fieldList.Remove(DBEpisode.cImportProcessed);
                fieldList.Remove(DBOnlineEpisode.cOnlineDataImported);
                fieldList.Remove(DBOnlineEpisode.cHidden);
                fieldList.Remove(DBOnlineEpisode.cLastUpdated);

                foreach (String sField in fieldList)
                {
                    ToolStripItem item = new ToolStripLabel();
                    item.Name = "<" + DBEpisode.cOutName + "." + sField + ">";
                    item.Tag = textBox;
                    String sPretty = DBEpisode.PrettyFieldName(sField);
                    if (sPretty == sField)
                        item.Text = item.Name;
                    else
                        item.Text = item.Name + " - (" + sPretty + ")";
                    subMenu.Items.Add(item);
                }
                contextMenuStrip_InsertFields.Items.Add(subMenuItem);
            }

            // Set Cancel to false. 
            // It is optimized to true based on empty entry.
            e.Cancel = false;
        }

        private void contextMenuStrip_SeriesFields_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // Acquire references to the owning control and item.
            RichTextBox textBox = e.ClickedItem.Tag as RichTextBox;
            if (textBox != null)
            {
                int nCarret = textBox.SelectionStart;
                textBox.Text = textBox.Text.Insert(textBox.SelectionStart, e.ClickedItem.Name);
                textBox.SelectionLength = 0;
                textBox.SelectionStart = nCarret;
            }
        }

        private void comboLogLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboLogLevel.SelectedIndex == 0) MPTVSeriesLog.selectedLogLevel = MPTVSeriesLog.LogLevel.Normal;
            else if (this.comboLogLevel.SelectedIndex == 1) MPTVSeriesLog.selectedLogLevel = MPTVSeriesLog.LogLevel.Debug;
            else if (this.comboLogLevel.SelectedIndex == 2) MPTVSeriesLog.selectedLogLevel = MPTVSeriesLog.LogLevel.DebugSQL;
            else MPTVSeriesLog.selectedLogLevel = MPTVSeriesLog.LogLevel.Normal;
        }     

        private void textBox_PluginHomeName_TextChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cView_PluginName, textBox_PluginHomeName.Text);
        }

        private void log_window_changed()
        {
            this.splitMain_Log.SplitterDistance = this.Size.Height / 3 * 2;
            DBOption.SetOptions(DBOption.cConfig_LogCollapsed, splitMain_Log.Panel2Collapsed);

            if (splitMain_Log.Panel2Collapsed)
            {
                button1.Image = global::WindowPlugins.GUITVSeries.Properties.Resources.arrow_up_small;
                this.toolTip_Help.SetToolTip(this.button1, "Click to show log");
            }
            else
            {
                button1.Image = global::WindowPlugins.GUITVSeries.Properties.Resources.arrow_down_small;
                this.toolTip_Help.SetToolTip(this.button1, "Click to hide log");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            splitMain_Log.Panel2Collapsed = !splitMain_Log.Panel2Collapsed;
            DBOption.SetOptions(DBOption.cConfig_LogCollapsed, splitMain_Log.Panel2Collapsed);
            log_window_changed();
        }

        private void checkBox_ShowHidden_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cShowHiddenItems, checkBox_ShowHidden.Checked);
            LoadTree();
        }

        private void contextMenuStrip_DetailsTree_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            TreeNode clickedNode = contextMenuStrip_DetailsTree.Tag as TreeNode;

            if (e.ClickedItem.Tag == null)
                return;
            
            switch (e.ClickedItem.Tag.ToString())
            {
                case "hide":
                    HideNode(clickedNode);
                    break;

                case "delete":
                    DeleteNode(clickedNode);
                    break;

                case "update":
                    UpdateNode(clickedNode);
                    break;
                
                case "mediainfo":
                    RescanMediaInfoNode(clickedNode);
                    break;

                case "watched":
                    ToggleWatchedNode(clickedNode, 1);
                    break;

                case "unwatched":
                    ToggleWatchedNode(clickedNode, 0);
                    break;

                case "subtitle":
                    GetSubtitles(clickedNode);
                    break;

                case "torrent":
                    TorrentFile(clickedNode);
                    break;

                case "newzbin":
                    NewzFile(clickedNode);
                    break;

                case "resetUserSelections":
                    ResetUserSelectionsToolStripMenuItem(clickedNode);
                    break;

            }
        }

        public void TorrentFile(TreeNode node)
        {
            if (node == null)
                return;

            switch (node.Name)
            {
                case DBSeries.cTableName:
                    DBSeries series = (DBSeries)node.Tag;
                    break;

                case DBSeason.cTableName:
                    DBSeason season = (DBSeason)node.Tag;
                    break;

                case DBEpisode.cTableName:
                    DBEpisode episode = (DBEpisode)node.Tag;
                    Torrent.Load Load = new Torrent.Load(this);
                    Load.LoadCompleted += new WindowPlugins.GUITVSeries.Torrent.Load.LoadCompletedHandler(TorrentLoad_LoadCompleted);
                    Load.Search(episode);
                    break;
            }
        }

        public void NewzFile(TreeNode node)
        {
            if (node == null)
                return;

            switch (node.Name)
            {
                case DBSeries.cTableName:
                    DBSeries series = (DBSeries)node.Tag;
                    break;

                case DBSeason.cTableName:
                    DBSeason season = (DBSeason)node.Tag;
                    break;

                case DBEpisode.cTableName:
                    DBEpisode episode = (DBEpisode)node.Tag;
                    Newzbin.Load Load = new Newzbin.Load(this);
                    Load.LoadCompleted += new WindowPlugins.GUITVSeries.Newzbin.Load.LoadCompletedHandler(NewzbinLoad_LoadCompleted);
                    Load.Search(episode);
                    break;
            }
        }

        void TorrentLoad_LoadCompleted(bool bOK)
        {

        }

        void NewzbinLoad_LoadCompleted(bool bOK, String msgOut)
        {

        }

        private delegate ReturnCode ChooseFromSelectionDelegate(ChooseFromSelectionDescriptor descriptor);
        private Feedback.CItem m_selected;

        public Feedback.ReturnCode ChooseFromSelection(Feedback.ChooseFromSelectionDescriptor descriptor, out Feedback.CItem selected)
        {
            Feedback.ReturnCode returnCode;
            if (m_localControlForInvoke.InvokeRequired)
            {
                returnCode = (Feedback.ReturnCode)m_localControlForInvoke.Invoke(new ChooseFromSelectionDelegate(ChooseFromSelectionSync), new Object[] { descriptor });
            }
            else
                returnCode = ChooseFromSelectionSync(descriptor);
            selected = m_selected;
            return returnCode;
        }

        public Feedback.ReturnCode ChooseFromSelectionSync(Feedback.ChooseFromSelectionDescriptor descriptor)
        {
            ChooseFromSelectionDialog userSelection = new ChooseFromSelectionDialog(descriptor);
            DialogResult result = userSelection.ShowDialog();
            m_selected = userSelection.SelectedItem;
            switch (result)
            {
                case DialogResult.OK:
                    return Feedback.ReturnCode.OK;

                case DialogResult.Ignore:
                    return Feedback.ReturnCode.Ignore;

                case DialogResult.Cancel:
                default:
                    return Feedback.ReturnCode.Cancel;
            }
        }

        private delegate ReturnCode YesNoOkDialogDelegate(ChooseFromYesNoDescriptor descriptor);
        public Feedback.ReturnCode YesNoOkDialog(Feedback.ChooseFromYesNoDescriptor descriptor)
        {
            Feedback.ReturnCode returnCode;
            if (m_localControlForInvoke.InvokeRequired)
            {
                returnCode = (Feedback.ReturnCode)m_localControlForInvoke.Invoke(new YesNoOkDialogDelegate(YesNoOkDialogSync), new Object[] { descriptor });
            }
            else
            {
                returnCode = YesNoOkDialogSync(descriptor);
            }
            return returnCode;
        }

        public Feedback.ReturnCode YesNoOkDialogSync(Feedback.ChooseFromYesNoDescriptor descriptor)
        {
            MessageBoxButtons button = MessageBoxButtons.OK;
            MessageBoxIcon icon = MessageBoxIcon.Information;
            MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1;
            ReturnCode retValue = ReturnCode.OK;

            switch (descriptor.m_dialogButtons)
            {
                case DialogButtons.OK:
                    button = MessageBoxButtons.OK;
                    icon = MessageBoxIcon.Information;
                    break;
                case DialogButtons.YesNo:
                    button = MessageBoxButtons.YesNo;
                    icon = MessageBoxIcon.Question;
                    break;
                case DialogButtons.YesNoCancel:
                    button = MessageBoxButtons.YesNoCancel;
                    icon = MessageBoxIcon.Question;
                    break;
            }

            switch (descriptor.m_dialogDefaultButton)
            {
                case ReturnCode.OK:
                    defaultButton = MessageBoxDefaultButton.Button1;
                    break;
                case ReturnCode.Yes:
                    defaultButton = MessageBoxDefaultButton.Button1;
                    break;
                case ReturnCode.No:
                    defaultButton = MessageBoxDefaultButton.Button2;
                    break;
                default:
                    defaultButton = MessageBoxDefaultButton.Button2;
                    break;
            }

            DialogResult result = MessageBox.Show(descriptor.m_sLabel, descriptor.m_sTitle, button, icon, defaultButton);

            switch (result)
            {
                case DialogResult.OK:
                    retValue = ReturnCode.OK;
                    break;
                case DialogResult.Yes:
                    retValue = ReturnCode.Yes;
                    break;
                case DialogResult.No:
                    retValue = ReturnCode.No;
                    break;
                default:
                    retValue = ReturnCode.No;
                    break;
            }

            return retValue;
        }

        private delegate ReturnCode GetStringFromUserDelegate(GetStringFromUserDescriptor descriptor);
        public ReturnCode GetStringFromUser(GetStringFromUserDescriptor descriptor, out string input)
        {
            input = string.Empty;
            return ReturnCode.Ignore;
        }

        private void ResetUserSelectionsToolStripMenuItem(TreeNode node)
        {
            if (node == null)
                return;

            switch (node.Name)
            {
                case DBSeries.cTableName:
                    DBSeries series = (DBSeries)node.Tag;
                    DBUserSelection.Clear(series);
                    break;

                case DBSeason.cTableName:
                    DBSeason season = (DBSeason)node.Tag;
                    DBUserSelection.Clear(season);
                    break;

                case DBEpisode.cTableName:
                    break;
            }
        }

        private void GetSubtitles(TreeNode node)
        {
            if (node == null)
                return;

            switch (node.Name)
            {
                case DBSeries.cTableName:
                    DBSeries series = (DBSeries)node.Tag;
                    break;

                case DBSeason.cTableName:
                    DBSeason season = (DBSeason)node.Tag;
                    break;

                case DBEpisode.cTableName:
                    DBEpisode episode = (DBEpisode)node.Tag;
                    ShowSubtitleMenu(episode);
                    break;
            }
        }

        protected void ShowSubtitleMenu(DBEpisode episode)
        {
            List<CItem> Choices = new List<CItem>();
            string enabledDownloaders = DBOption.GetOptions(DBOption.cSubtitleDownloadersEnabled);

            // Get names of the SubtitleDownloader implementations for menu
            foreach (var name in SubtitleDownloaderFactory.GetSubtitleDownloaderNames())
            {
                if (enabledDownloaders.Contains(name))
                {
                    Choices.Add(new CItem(name, name, name));
                }
            }

            if (Choices.Count == 0)
                return;

            CItem selected = null;

            ChooseFromSelectionDescriptor descriptor = new ChooseFromSelectionDescriptor();
            descriptor.m_sTitle = Translation.GetSubtitlesFrom;
            descriptor.m_sItemToMatchLabel = "";
            descriptor.m_sItemToMatch = "";
            descriptor.m_sListLabel = Translation.EnabledSubtitleSites;
            descriptor.m_List = Choices;
            descriptor.m_sbtnIgnoreLabel = String.Empty;

            ReturnCode returnCode = ChooseFromSelection(descriptor, out selected);

            if (selected != null && returnCode.Equals(ReturnCode.OK))
            {
                ISubtitleDownloader downloader = SubtitleDownloaderFactory.GetSubtitleDownloader(selected.m_Tag.ToString());
                SubtitleRetriever retriever = new SubtitleRetriever(this, downloader);

                if (!subtitleDownloaderWorking)
                {
                    retriever.SubtitleRetrievalCompleted += downloader_SubtitleRetrievalCompleted;
                    subtitleDownloaderWorking = true;
                    retriever.GetSubs(episode);
                }
            }
        }

        void downloader_SubtitleRetrievalCompleted(bool subtitleRetrieved, string errorMessage)
        {
            subtitleDownloaderWorking = false;
        }

        ToolStripMenuItem subMenuItem = null;
        ContextMenuStrip subMenu = null;
        private void contextMenuStrip_DetailsTree_Opening(object sender, CancelEventArgs e)
        {
            TreeNode node = contextMenuStrip_DetailsTree.Tag as TreeNode;
            if (node == null)
                return;

            bool bHidden = false;

            //NOTE: use names to access the menu items in case the order get altered in the future
            switch (node.Name) {
                case DBSeries.cTableName:
                    DBSeries series = (DBSeries)node.Tag;
                    bHidden = series[DBSeries.cHidden];
                    contextMenuStrip_DetailsTree.Items["getSubtitlesToolStripMenuItem"].Enabled = false;
                    contextMenuStrip_DetailsTree.Items["torrentThToolStripMenuItem"].Enabled = false;
                    contextMenuStrip_DetailsTree.Items["newzbinThisToolStripMenuItem"].Enabled = false;

					// Create AddToView ContextMenu Item and Submenu
                    // No need to create a Remove Item as we can use the checked state
                    if (subMenuItem == null) {
                        subMenuItem = new ToolStripMenuItem("Add Series to View");
                        subMenuItem.Name = "addSeriesToView";
                        subMenu = new ContextMenuStrip(this.components);
                        subMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
                        subMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip_AddToView_ItemClicked);
                        subMenuItem.DropDown = subMenu;
                        subMenu.ShowCheckMargin = true;
                        subMenu.ShowImageMargin = false;                        
                    }
                    subMenuItem.Enabled = true;
                    
                    // Populate View Sub-Menu
					DBView[] views = DBView.getTaggedViews();                    
                    string viewTags = series[DBOnlineSeries.cViewTags];

                    subMenu.Items.Clear();
					foreach (DBView view in views) {                        
                        ToolStripMenuItem item = new ToolStripMenuItem();						
                        item.Name = view[DBView.cTransToken];
                        item.Text = view[DBView.cTransToken];
						item.Tag = view;
                        // Check View if already a member                                                
                        string viewTag = "|" + view[DBView.cTransToken] + "|";
                        if (viewTags.Contains(viewTag))
                            item.Checked = true;

						subMenu.Items.Add(item);						
					}                    
					contextMenuStrip_DetailsTree.Items.Add(subMenuItem);
                                        
                    break;

                case DBSeason.cTableName:
                    DBSeason season = (DBSeason)node.Tag;
                    bHidden = season[DBSeason.cHidden];
                    contextMenuStrip_DetailsTree.Items["getSubtitlesToolStripMenuItem"].Enabled = false;
                    contextMenuStrip_DetailsTree.Items["torrentThToolStripMenuItem"].Enabled = false;
                    contextMenuStrip_DetailsTree.Items["newzbinThisToolStripMenuItem"].Enabled = false;
                    
                    if (contextMenuStrip_DetailsTree.Items.ContainsKey("addSeriesToView"))
                        contextMenuStrip_DetailsTree.Items["addSeriesToView"].Enabled = false;
                    break;

                case DBEpisode.cTableName:
                    DBEpisode episode = (DBEpisode)node.Tag;
                    bHidden = episode[DBOnlineEpisode.cHidden];
                    contextMenuStrip_DetailsTree.Items["getSubtitlesToolStripMenuItem"].Enabled = DBOption.GetOptions(DBOption.cSubtitleDownloadersEnabled);                  
                    contextMenuStrip_DetailsTree.Items["torrentThToolStripMenuItem"].Enabled = true;
                    contextMenuStrip_DetailsTree.Items["newzbinThisToolStripMenuItem"].Enabled = true;
                    
                    if (contextMenuStrip_DetailsTree.Items.ContainsKey("addSeriesToView"))
                        contextMenuStrip_DetailsTree.Items["addSeriesToView"].Enabled = false;
                    
                        break;
            }
            // Hide Downloaders not frequently used by users
            if (String.IsNullOrEmpty(DBOption.GetOptions(DBOption.cUTorrentDownloadPath))) {
                contextMenuStrip_DetailsTree.Items["torrentThToolStripMenuItem"].Visible = false;
            }
            if (String.IsNullOrEmpty(DBOption.GetOptions(DBOption.cNewsLeecherDownloadPath))) {
                contextMenuStrip_DetailsTree.Items["newzbinThisToolStripMenuItem"].Visible = false;
            }

            if (bHidden)
                contextMenuStrip_DetailsTree.Items["hideToolStripMenuItem"].Text = "UnHide";
            else
                contextMenuStrip_DetailsTree.Items["hideToolStripMenuItem"].Text = "Hide";
        }
        
		private void contextMenuStrip_AddToView_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            TreeNode node = contextMenuStrip_DetailsTree.Tag as TreeNode;           
            ToolStripMenuItem item = (ToolStripMenuItem)e.ClickedItem;

            // Get Selected View
            DBView view = (DBView)e.ClickedItem.Tag;
            string selectedView = view[DBView.cTransToken];
            string newTags = string.Empty;

            bool add = !item.Checked;

            // Get Current View Tags for series
            DBSeries series = (DBSeries)node.Tag;
            
            // Add Series to view if unchecked
            // Remove Series from view if checked            
            if (!add) {
                MPTVSeriesLog.Write(string.Format("Removing series \"{0}\" from \"{1}\"", series.ToString(), selectedView));
                newTags = Helper.GetSeriesViewTags(series, false, selectedView);          
            }
            else {
                MPTVSeriesLog.Write(string.Format("Adding series \"{0}\" to \"{1}\"", series.ToString(), selectedView));
                newTags = Helper.GetSeriesViewTags(series, true, selectedView);
            }       
 
            // Commit changes to database
            series[DBOnlineSeries.cViewTags] = newTags;
            series.Commit();

            // Add to online database
            if (selectedView == DBView.cTranslateTokenOnlineFavourite) {
                Online_Parsing_Classes.OnlineAPI.ConfigureFavourites(add, DBOption.GetOptions(DBOption.cOnlineUserID), series[DBOnlineSeries.cID]);
            }
                      
		}

        private void treeView_Library_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenuStrip_DetailsTree.Tag = e.Node;
        }

        # region Torrent
        private void textBox_uTorrentPath_TextChanged(object sender, EventArgs e)
        {
            String sPath = textBox_uTorrentPath.Text;
            if (System.IO.File.Exists(sPath))
                textBox_uTorrentPath.BackColor = System.Drawing.SystemColors.ControlLightLight;
            else
                textBox_uTorrentPath.BackColor = System.Drawing.Color.Tomato;
            DBOption.SetOptions(DBOption.cUTorrentPath, sPath);
        }

        private void button_uTorrentBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = DBOption.GetOptions(DBOption.cUTorrentPath);
            openFileDialog.Filter = "Executable files (*.exe)|";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                DBOption.SetOptions(DBOption.cUTorrentPath, openFileDialog.FileName);
                textBox_uTorrentPath.Text = openFileDialog.FileName;
            }
        }

        private void comboBox_TorrentPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_currentTorrentSearch = comboBox_TorrentPreset.SelectedItem as DBTorrentSearch;
            if (m_currentTorrentSearch != null)
            {
                textBox_TorrentSearchUrl.Text = m_currentTorrentSearch[DBTorrentSearch.cSearchUrl];
                textBox_TorrentSearchRegex.Text = m_currentTorrentSearch[DBTorrentSearch.cSearchRegex];
                textBox_TorrentDetailsUrl.Text = m_currentTorrentSearch[DBTorrentSearch.cDetailsUrl];
                textBox_TorrentDetailsRegex.Text = m_currentTorrentSearch[DBTorrentSearch.cDetailsRegex];
                DBOption.SetOptions(DBOption.cTorrentSearch, m_currentTorrentSearch[DBTorrentSearch.cID]);
            }
            else
            {
                textBox_TorrentSearchUrl.Text = String.Empty;
                textBox_TorrentSearchRegex.Text = String.Empty;
                textBox_TorrentDetailsUrl.Text = String.Empty;
                textBox_TorrentDetailsRegex.Text = String.Empty;
            }
        }

        private void comboBox_TorrentPreset_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    {
                        // check if this string exists in the list
                        object selObj = null;
                        foreach (object obj in comboBox_TorrentPreset.Items)
                            if (obj.ToString() == comboBox_TorrentPreset.Text)
                            {
                                selObj = obj;
                                break;
                            }

                        if (selObj == null)
                        {
                            // create a new item
                            DBTorrentSearch newSearch = new DBTorrentSearch(comboBox_TorrentPreset.Text);
                            newSearch.Commit();
                            comboBox_TorrentPreset.Items.Add(newSearch);
                        }

                        // select item
                        comboBox_TorrentPreset.SelectedItem = selObj;
                    }
                    break;

                case Keys.Delete:
                    // delete the selection
                    if (comboBox_TorrentPreset.DroppedDown)
                    {
                        // delete the selected item
                        if (MessageBox.Show("Really delete " + comboBox_TorrentPreset.SelectedItem + " ?", "Confirm") == DialogResult.OK)
                        {
                            SQLCondition condition = new SQLCondition();
                            condition.Add(new DBTorrentSearch(), DBTorrentSearch.cID, comboBox_TorrentPreset.SelectedItem.ToString(), SQLConditionType.Equal);
                            DBTorrentSearch.Clear(condition);
                            comboBox_TorrentPreset.Items.Remove(comboBox_TorrentPreset.SelectedItem);

                            if (comboBox_TorrentPreset.Items.Count > 0)
                                comboBox_TorrentPreset.SelectedIndex = 0;
                        }
                    }
                    break;
            }

        }

        private void textBox_TorrentUrl_TextChanged(object sender, EventArgs e)
        {
            m_currentTorrentSearch[DBTorrentSearch.cSearchUrl] = textBox_TorrentSearchUrl.Text;
            m_currentTorrentSearch.Commit();
        }

        private void textBox_TorrentRegex_TextChanged(object sender, EventArgs e)
        {
            m_currentTorrentSearch[DBTorrentSearch.cSearchRegex] = textBox_TorrentSearchRegex.Text;
            m_currentTorrentSearch.Commit();
        }

        private void textBox_TorrentDetailsUrl_TextChanged(object sender, EventArgs e)
        {
            m_currentTorrentSearch[DBTorrentSearch.cDetailsUrl] = textBox_TorrentDetailsUrl.Text;
            m_currentTorrentSearch.Commit();
        }

        private void textBox_TorrentDetailsRegex_TextChanged(object sender, EventArgs e)
        {
            m_currentTorrentSearch[DBTorrentSearch.cDetailsRegex] = textBox_TorrentDetailsRegex.Text;
            m_currentTorrentSearch.Commit();
        }
        # endregion

        private void checkBox_RandBanner_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cRandomBanner, checkBox_RandBanner.Checked);
        }

        private void linkMediaInfoUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DialogResult result = MessageBox.Show("Force update of Media Info for all files?\n\nSelect No to update new files only.", "Update Media Info", MessageBoxButtons.YesNoCancel);

            if (result == DialogResult.Cancel)
            {
                return;
            }

            SQLCondition cond = new SQLCondition();
            cond.Add(new DBEpisode(), DBEpisode.cFilename, "", SQLConditionType.NotEqual);
            List<DBEpisode> episodes = new List<DBEpisode>();
            // get all the episodes
            episodes = DBEpisode.Get(cond, false);

            if (result == DialogResult.No)
            {
                List<DBEpisode> todoeps = new List<DBEpisode>();
                // only get the episodes that dont have their resolutions read out already
                for (int i = 0; i < episodes.Count; i++)
                    if (!episodes[i].HasMediaInfo)
                        todoeps.Add(episodes[i]);
                episodes = todoeps;
            }

            UpdateMediaInfoASync(episodes);

        }

        private void UpdateMediaInfoASync(List<DBEpisode> episodes)
        {
            if (episodes.Count > 0)
            {
                MPTVSeriesLog.Write("Updating MediaInfo....(Please be patient!)");
                BackgroundWorker resReader = new BackgroundWorker();
                resReader.DoWork += new DoWorkEventHandler(asyncReadResolutions);
                resReader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(asyncReadResolutionsCompleted);
                resReader.RunWorkerAsync(episodes);
            }
            else 
                MPTVSeriesLog.Write("No Episodes found that need updating");
        }

        void asyncReadResolutionsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MPTVSeriesLog.Write("Force update of MediaInfo complete (processed " + e.Result.ToString() + " files)");
            LoadTree();
        }

        void asyncReadResolutions(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;
            List<DBEpisode> episodes = (List<DBEpisode>)e.Argument;
            foreach (DBEpisode ep in episodes)
                ep.ReadMediaInfo();
            e.Result = episodes.Count;
        }

        private void treeView_Extra_AfterSelect(object sender, TreeViewEventArgs e)
        {
            foreach (Panel pane in m_paneListExtra)
            {
                if (pane.Name == e.Node.Name)
                {
                    pane.Visible = true;
                }
                else
                    pane.Visible = false;
            }

        }

        private void button_dbbrowse_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = Settings.GetPath(Settings.Path.database);
            openFileDialog.Filter = "Executable files (*.db3)|";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Settings.SetDBPath(openFileDialog.FileName);
                textBox_dblocation.Text = openFileDialog.FileName;
            }
        }
        # region Newsbin
        private void textBox_NewsSearchUrl_TextChanged(object sender, EventArgs e)
        {
            m_currentNewsSearch[DBNewzbin.cSearchUrl] = textBox_NewsSearchUrl.Text;
            m_currentNewsSearch.Commit();
        }

        private void textBox_NewsSearchRegex_TextChanged(object sender, EventArgs e)
        {
            m_currentNewsSearch[DBNewzbin.cSearchRegexReport] = textBox_NewsSearchReportRegex.Text;
            m_currentNewsSearch.Commit();
        }

        private void textBox_NewzbinLogin_TextChanged(object sender, EventArgs e)
        {
            m_currentNewsSearch[DBNewzbin.cLogin] = textBox_NewzbinLogin.Text;
            m_currentNewsSearch.Commit();
        }

        private void textbox_NewzbinPassword_TextChanged(object sender, EventArgs e)
        {
            m_currentNewsSearch[DBNewzbin.cPassword] = textbox_NewzbinPassword.Text;
            m_currentNewsSearch.Commit();
        }

        private void textBox_NewsSearchIDRegex_TextChanged(object sender, EventArgs e)
        {
            m_currentNewsSearch[DBNewzbin.cSearchRegexID] = textBox_NewsSearchIDRegex.Text;
            m_currentNewsSearch.Commit();
        }

        private void textBox_NewsSearchNameRegex_TextChanged(object sender, EventArgs e)
        {
            m_currentNewsSearch[DBNewzbin.cSearchRegexName] = textBox_NewsSearchNameRegex.Text;
            m_currentNewsSearch.Commit();
        }

        private void textBox_NewsSearchSizeRegex_TextChanged(object sender, EventArgs e)
        {
            m_currentNewsSearch[DBNewzbin.cSearchRegexSize] = textBox_NewsSearchSizeRegex.Text;
            m_currentNewsSearch.Commit();
        }

        private void textBox_NewsSearchPostDateRegex_TextChanged(object sender, EventArgs e)
        {
            m_currentNewsSearch[DBNewzbin.cSearchRegexPostDate] = textBox_NewsSearchPostDateRegex.Text;
            m_currentNewsSearch.Commit();
        }

        private void textBox_NewsSearchReportDateRegex_TextChanged(object sender, EventArgs e)
        {
            m_currentNewsSearch[DBNewzbin.cSearchRegexReportDate] = textBox_NewsSearchReportDateRegex.Text;
            m_currentNewsSearch.Commit();
        }

        private void textBox_NewsSearchFormatRegex_TextChanged(object sender, EventArgs e)
        {
            m_currentNewsSearch[DBNewzbin.cSearchRegexFormat] = textBox_NewsSearchFormatRegex.Text;
            m_currentNewsSearch.Commit();
        }

        private void textBox_NewsSearchGroupRegex_TextChanged(object sender, EventArgs e)
        {
            m_currentNewsSearch[DBNewzbin.cSearchRegexGroup] = textBox_NewsSearchGroupRegex.Text;
            m_currentNewsSearch.Commit();
        }

        private void textBox_NewsSearchLanguageRegex_TextChanged(object sender, EventArgs e)
        {
            m_currentNewsSearch[DBNewzbin.cSearchRegexLanguage] = textBox_NewsSearchLanguageRegex.Text;
            m_currentNewsSearch.Commit();
        }

        private void textBox_NewsSearchIsolateArticleRegex_TextChanged(object sender, EventArgs e)
        {
            m_currentNewsSearch[DBNewzbin.cSearchRegexIsolateArticleName] = textBox_NewsSearchIsolateArticleRegex.Text;
            m_currentNewsSearch.Commit();
        }

        private void textBox_NewsSearchParseArticleRegex_TextChanged(object sender, EventArgs e)
        {
            m_currentNewsSearch[DBNewzbin.cSearchRegexParseArticleName] = textBox_NewsSearchParseArticleRegex.Text;
            m_currentNewsSearch.Commit();
        }

        private void button_newsleecherbrowse_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = DBOption.GetOptions(DBOption.cNewsLeecherPath);
            openFileDialog.Filter = "Executable files (*.exe)|";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                DBOption.SetOptions(DBOption.cNewsLeecherPath, openFileDialog.FileName);
                textBox_newsleecher.Text = openFileDialog.FileName;
            }
        }

        private void textBox_NewsDownloadPath_TextChanged(object sender, EventArgs e)
        {
            String sPath = textBox_NewsDownloadPath.Text;
            DBOption.SetOptions(DBOption.cNewsLeecherDownloadPath, sPath);

        }

        private void button_NewsDownloadPathBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = DBOption.GetOptions(DBOption.cNewsLeecherDownloadPath);
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                DBOption.SetOptions(DBOption.cNewsLeecherDownloadPath, folderBrowserDialog.SelectedPath);
                textBox_NewsDownloadPath.Text = folderBrowserDialog.SelectedPath;
            }
        }
        # endregion      

        private void addLogo_Click(object sender, EventArgs e)
        {
            logoConfigurator.validDelegate del = delegate(ref RichTextBox txtBox) { FieldValidate(ref txtBox); };
            logoConfigurator lc = new logoConfigurator(del, contextMenuStrip_InsertFields);

            if (DialogResult.OK == lc.ShowDialog())
            {
                List<string> entries = new List<string>();
                foreach (string item in lstLogos.Items)
                    entries.Add(item.ToString());
                entries.Add(lc.result);
                localLogos.saveToDB(entries);
                lstLogos.Items.Clear();
                lstLogos.Items.AddRange(localLogos.getFromDB().ToArray());
            }
        }

        private void btnrmvLogo_Click(object sender, EventArgs e)
        {
            if (lstLogos.SelectedIndex == -1) return;
            List<string> entries = new List<string>();
            foreach (string item in lstLogos.Items)
                entries.Add(item.ToString());
            entries.Remove((string)lstLogos.SelectedItem);
            localLogos.saveToDB(entries);
            lstLogos.Items.Clear();
            lstLogos.Items.AddRange(localLogos.getFromDB().ToArray());
        }

        private void btnLogoDown_Click(object sender, EventArgs e)
        {
            if (lstLogos.Items.Count < 2) return;
            if (lstLogos.SelectedIndex == lstLogos.Items.Count - 1 || lstLogos.SelectedIndex == -1) return;

            string selected = (string)lstLogos.SelectedItem;
            lstLogos.Items[lstLogos.SelectedIndex] = lstLogos.Items[lstLogos.SelectedIndex + 1];
            lstLogos.Items[lstLogos.SelectedIndex + 1] = selected;
            int index = lstLogos.SelectedIndex + 1;
            List<string> entries = new List<string>();
            foreach (string item in lstLogos.Items)
                entries.Add(item.ToString());
            localLogos.saveToDB(entries);
            lstLogos.Items.Clear();
            lstLogos.Items.AddRange(localLogos.getFromDB().ToArray());

            lstLogos.SelectedIndex = index;

        }

        private void btnlogoUp_Click(object sender, EventArgs e)
        {
            if (lstLogos.Items.Count < 2) return;
            if (lstLogos.SelectedIndex == 0 || lstLogos.SelectedIndex == -1) return;

            string selected = (string)lstLogos.SelectedItem;
            lstLogos.Items[lstLogos.SelectedIndex] = lstLogos.Items[lstLogos.SelectedIndex - 1];
            lstLogos.Items[lstLogos.SelectedIndex - 1] = selected;
            int index = lstLogos.SelectedIndex - 1;
            List<string> entries = new List<string>();
            foreach (string item in lstLogos.Items)
                entries.Add(item.ToString());
            localLogos.saveToDB(entries);
            lstLogos.Items.Clear();
            lstLogos.Items.AddRange(localLogos.getFromDB().ToArray());

            lstLogos.SelectedIndex = index;
        }

        private void btnLogoEdit_Click(object sender, EventArgs e)
        {
            if (lstLogos.SelectedIndex == -1) return;
            logoConfigurator.validDelegate del = delegate(ref RichTextBox txtBox) { FieldValidate(ref txtBox); };
            logoConfigurator lc = new logoConfigurator(del, contextMenuStrip_InsertFields, (string)lstLogos.SelectedItem);

            if (DialogResult.OK == lc.ShowDialog())
            {
                //lstLogos.SelectedItem = lc.result;

                List<string> entries = new List<string>();
                foreach (string item in lstLogos.Items)
                {
                    if (item == (string)lstLogos.SelectedItem)
                        entries.Add(lc.result);
                    else
                        entries.Add(item.ToString());
                }
                localLogos.saveToDB(entries);
                lstLogos.Items.Clear();
                lstLogos.Items.AddRange(localLogos.getFromDB().ToArray());
            }
        }

        private void checkBox_doFolderWatch_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cImport_FolderWatch, checkBox_doFolderWatch.Checked);
            if (checkBox_doFolderWatch.Checked) {
                checkBox_scanRemoteShares.Enabled = true;
                if (checkBox_scanRemoteShares.Checked) {
                    nudScanRemoteShareFrequency.Enabled = true;
                    dbOptChkBoxScanFullscreenVideo.Enabled = true;
                }
                else {
                    nudScanRemoteShareFrequency.Enabled = false;
                    dbOptChkBoxScanFullscreenVideo.Enabled = false;
                }
            }
            else {
                checkBox_scanRemoteShares.Enabled = false;
                nudScanRemoteShareFrequency.Enabled = false;
                dbOptChkBoxScanFullscreenVideo.Enabled = false;
            }
        }

        private void checkBox_scanRemoteShares_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cImport_ScanRemoteShare, checkBox_scanRemoteShares.Checked);
            if (checkBox_scanRemoteShares.Checked)
                nudScanRemoteShareFrequency.Enabled = true;
            else
                nudScanRemoteShareFrequency.Enabled = false;
        }

        //List<logicalView> testViews = new List<logicalView>();
        //string[] viewArgument = null;
        //logicalViewStep.type currType = logicalViewStep.type.group;
        //bool isinit = false;
        /*private void button3_Click(object sender, EventArgs e)
        {
            if (!isinit)
                this.listBox1.DoubleClick += new EventHandler(listBox1_DoubleClick);
            isinit = true;
            if (viewArgument == null) this.numericUpDown1.Value = 0;
            //testViews = logicalView.getAllFromString(this.richTextBox1.Text.Trim(), true);
            logicalViewStep.type curType = testViews[0].gettypeOfStep((int)this.numericUpDown1.Value);
            this.listBox1.Items.Clear();
            currType = curType;
            switch (curType)
            {
                case logicalViewStep.type.group:
                    foreach (string group in testViews[0].getGroupItems((int)this.numericUpDown1.Value, viewArgument))
                    {
                        this.listBox1.Items.Add(group);
                    }
                    break;
                case logicalViewStep.type.series:
                    foreach (DBSeries series in testViews[0].getSeriesItems((int)this.numericUpDown1.Value, viewArgument))
                    {
                        this.listBox1.Items.Add(series[DBOnlineSeries.cPrettyName] + " <-> " + series[DBOnlineSeries.cID]);
                    }
                    break;
                case logicalViewStep.type.season:
                    foreach (DBSeason season in testViews[0].getSeasonItems((int)this.numericUpDown1.Value, viewArgument))
                    {
                        this.listBox1.Items.Add(season[DBSeason.cIndex]);
                    }
                    break;
            }

            viewArgument = null;
        }

        void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (currType != logicalViewStep.type.episode)
            {
                this.numericUpDown1.Value++;
                switch (currType)
                {
                    case logicalViewStep.type.group:
                        viewArgument = new string[] { (string)listBox1.SelectedItem };
                        break;
                    case logicalViewStep.type.series:
                        viewArgument = new string[] { ((string)listBox1.SelectedItem).Split(new string[] { " <-> " }, StringSplitOptions.None)[1].Trim() };
                        break;
                }
                button3_Click(new object(), new EventArgs());
            }
        }*/

        private void comboLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cLanguage, (string)comboLanguage.SelectedItem);
            Translation.loadTranslations("en(us)"); // flush previously selected, perhaps untranslated in new language, strings (otherwise we get a mix)
            Translation.Init();
            LoadViews();
        }

        private void _availViews_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!pauseViewConfigSave)
            {
                logicalView.s_cachePrettyName = false;
                pauseViewConfigSave = true;
                view_selectedName.Text = string.Empty;
                view_selStepsList.Items.Clear();

                selectedView = Helper.getElementFromList<logicalView, string>((string)_availViews.SelectedItem, "Name", 0, availViews);
                view_selectedName.Text = selectedView.prettyName;
                checkCurViewEnabled.Checked = selectedView.m_Enabled;
                checkBoxParentalControl.Checked = selectedView.ParentalControl;             

                foreach (string step in Helper.getPropertyListFromList<logicalViewStep, String>("Name", selectedView.m_steps))
                    view_selStepsList.Items.Add(step);
                
                pauseViewConfigSave = false;

                // Enable 'Edit' button for Simple Views
                // TODO: allow editing of advanced views when conditional GUI is complete
                if (selectedView.IsTaggedView) {
                    buttonEditView.Enabled = true;
                } else {
                    buttonEditView.Enabled = false;
                }
            }
        }

        ~ConfigurationForm()
        {
            // so that locallogos can clean up its stuff
            localLogos.cleanUP();

            // save the config size
            DBOption.SetOptions("configSizeHeight", this.Size.Height);
            DBOption.SetOptions("configSizeWidth", this.Size.Width);
        }

        private void comboOnlineLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sel = "en";
            foreach (Language lang in onlineLanguages)
            {
                if (lang.language == (string)comboOnlineLang.SelectedItem)
                {
                    sel = lang.abbreviation;
                    break;
                }
            }

            if (sel != string.Empty && sel != DBOption.GetOptions(DBOption.cOnlineLanguage))
            {
                DBOption.SetOptions(DBOption.cOnlineLanguage, sel);
                DBOption.SetOptions(DBOption.cUpdateTimeStamp, 0);
                Online_Parsing_Classes.OnlineAPI.SelLanguageAsString = string.Empty; // to overcome caching
                MessageBox.Show("You need to do a manual import everytime the language is changed or your old items will not be updated!\nNew Language: " + (string)comboOnlineLang.SelectedItem, "Language changed", MessageBoxButtons.OK);
            }
        }

        private void linkDelUpdateTime_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DBOption.SetOptions(DBOption.cUpdateBannersTimeStamp, 0);
            DBOption.SetOptions(DBOption.cUpdateTimeStamp, 0);
            
            MPTVSeriesLog.Write("Last updated Timestamps cleared");
        }

        private void chkAllowDeletes_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cShowDeleteMenu, this.chkAllowDeletes.Checked);
        }

        bool pauseViewConfigSave = false;

        private void viewChanged()
        {
            if (!pauseViewConfigSave)
            {
                selectedView = Helper.getElementFromList<logicalView, string>((string)_availViews.SelectedItem, "Name", 0, availViews);
                if (selectedView != null)
                {
                    selectedView.prettyName = view_selectedName.Text;
                    selectedView.m_Enabled = checkCurViewEnabled.Checked;
                    selectedView.ParentalControl = checkBoxParentalControl.Checked;
                    selectedView.saveToDB();
                    LoadViews();
                    for (int i = 0; i < availViews.Count; i++) {
                        if (availViews[i].m_uniqueID == selectedView.m_uniqueID) {
                            pauseViewConfigSave = true;
                            _availViews.SelectedIndex = i;
                            pauseViewConfigSave = false;
                            break;
                        }
                    }
                }
            }
        }

        private void view_selectedName_TextChanged(object sender, EventArgs e)
        {
            viewChanged();
        }

        private void checkCurViewEnabled_CheckedChanged(object sender, EventArgs e)
        {
            viewChanged();
        }

        private void txtMainMirror_TextChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cMainMirror, txtMainMirror.Text);
        }

        private void linkExWatched_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaveFileDialog fd = new SaveFileDialog();
            fd.Filter = "Exported Watched Flags (*.watched)|*.watched";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                StreamWriter w = new StreamWriter(fd.FileName);
                SQLCondition cond = new SQLCondition();
                cond.Add(new DBOnlineEpisode(), DBOnlineEpisode.cWatched, true, SQLConditionType.Equal);
                foreach (DBValue val in DBOnlineEpisode.GetSingleField(DBOnlineEpisode.cCompositeID, cond, new DBOnlineEpisode()))
                {
                    try
                    {
                        w.WriteLine((string)val);
                    }
                    catch(IOException exception)
                    {
                        MPTVSeriesLog.Write("Watched info NOT exported!  Error: " + exception.ToString());
                        return;
                    }
                }
                w.Close();
                MPTVSeriesLog.Write("Watched info succesfully exported!");
            }
        }

        private void linkImpWatched_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Exported Watched Flags (*.watched)|*.watched";
            if (fd.ShowDialog() == DialogResult.OK && System.IO.File.Exists(fd.FileName))
            {
                StreamReader r = new StreamReader(fd.FileName);
                SQLCondition cond = new SQLCondition();

                string line = string.Empty;
                // set unwatched for all
                DBOnlineEpisode.GlobalSet(new DBOnlineEpisode(), DBOnlineEpisode.cWatched, false, new SQLCondition());
                // now set watched for all in file
                while ((line = r.ReadLine()) != null)
                {
                    cond = new SQLCondition();
                    cond.Add(new DBOnlineEpisode(), DBOnlineEpisode.cCompositeID, line, SQLConditionType.Equal);
                    DBOnlineEpisode.GlobalSet(new DBOnlineEpisode(), DBOnlineEpisode.cWatched, true, cond);
                }
                r.Close();
                MPTVSeriesLog.Write("Watched info succesfully imported!");
                LoadTree(); // reload tree so the changes are visible
            }
        }

        private void linkExParsingExpressions_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DBExpression[] expressions = DBExpression.GetAll();
            if (expressions == null || expressions.Length == 0) {
                MessageBox.Show("No vaild expressions to export!");
                return;
            }

            SaveFileDialog fd = new SaveFileDialog();
            fd.Filter = "Exported Parsing Expressions (*.expr)|*.expr";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                StreamWriter w = new StreamWriter(fd.FileName);                
                foreach (DBExpression expression in expressions)
                {
                    String val = "";
                    //val += expression[DBExpression.cIndex];
                    //val += ";";
                    val += (int)expression[DBExpression.cEnabled];
                    val += ";";
                    val += (String)expression[DBExpression.cType];
                    val += ";";
                    val += (String)expression[DBExpression.cExpression];
                    
                    try
                    {
                        w.WriteLine((string)val);
                    }
                    catch (IOException exception)
                    {
                        MPTVSeriesLog.Write("Parsing Expressions NOT exported!  Error: " + exception.ToString());
                        return;
                    }
                }
                w.Close();
                MPTVSeriesLog.Write("Parsing Expressions succesfully exported!");
            }
        }

        private void linkImpParsingExpressions_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Exported Parsing Expressions (*.expr)|*.expr";
            if (fd.ShowDialog() == DialogResult.OK && System.IO.File.Exists(fd.FileName))
            {
                StreamReader r = new StreamReader(fd.FileName);
                DBExpression expr;

                //Dialog box to make sure they want to clear out current expressions to import new ones.
                if (DialogResult.Yes ==
                    MessageBox.Show("You are about to delete all current parsing expressions," + Environment.NewLine +
                        "and replace them with the imported file." + Environment.NewLine + Environment.NewLine +
                        "Any current Expressions will be lost.  Would you like to proceed?", "Import Expressions", MessageBoxButtons.YesNo))
                {
                    dataGridView_Expressions.Rows.Clear();
                    DBExpression.ClearAll();
                    MPTVSeriesLog.Write("Expressions cleared");
                }

                string line = string.Empty;
                string[] parts;
                int index = 0;

                // now set watched for all in file
                while ((line = r.ReadLine()) != null)
                {
                    char[] c = {';'};
                    parts = line.Split(c, 3);
                    if (parts.Length != 3) continue;

                    expr = new DBExpression();
                    //if (Convert.ToInt32(parts[0]) >= 0) expr[DBExpression.cIndex] = parts[0]; else continue;
                    expr[DBExpression.cIndex] = index;
                    if (Convert.ToInt32(parts[0]) == 0 || Convert.ToInt32(parts[0]) == 1) expr[DBExpression.cEnabled] = parts[0]; else continue;
                    if (parts[1] == DBExpression.cType_Regexp || parts[1] == DBExpression.cType_Simple) expr[DBExpression.cType] = parts[1]; else continue;
                    expr[DBExpression.cExpression] = parts[2];
                   
                    if (expr.Commit()) index++;
                }

                r.Close();
                MPTVSeriesLog.Write("Parsing Expressions succesfully imported!");
                
                LoadExpressions();                
            }
        }

        private void btnLogoTemplate_Click(object sender, EventArgs e)
        {
            logoTemplate t = new logoTemplate();
            t.ShowDialog();
            if (t.result.Length > 0)
            {
                List<string> entries = new List<string>();
                foreach (string item in lstLogos.Items)
                    entries.Add(item.ToString());
                entries.Add(t.result);
                localLogos.saveToDB(entries);
                lstLogos.Items.Clear();
                lstLogos.Items.AddRange(localLogos.getFromDB().ToArray());
            }
        } 

        private void chkBlankBanners_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cGetBlankBanners, chkBlankBanners.Checked);
        }

        private void lblClearDB_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MessageBox.Show("You are about to delete all Series, Seasons and Episodes from your database!" + Environment.NewLine + "Continue?", Translation.Confirm, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // we delete everything
                DBTVSeries.Execute("delete from online_episodes");
                DBTVSeries.Execute("delete from local_episodes");
                DBTVSeries.Execute("delete from season");
                DBTVSeries.Execute("delete from local_series");
                DBTVSeries.Execute("delete from online_series");

                MPTVSeriesLog.Write("Database series, seasons and episodes deleted");
                LoadTree();
            }
        }

        private void lnkLogoExport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaveFileDialog fd = new SaveFileDialog();
            fd.Filter = "Exported Logo Rules (*.logoRules)|*.logoRules";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                StreamWriter w = new StreamWriter(fd.FileName);
                foreach (string logoRule in localLogos.getFromDB())
                    w.WriteLine(logoRule);
                w.Close();
                MPTVSeriesLog.Write(String.Format("{0} Logos succesfully exported",lstLogos.Items.Count));
            }
        }

        private void lnkLogoImp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Exported Logo Rules (*.logoRules)|*.logoRules";
            if (fd.ShowDialog() == DialogResult.OK && System.IO.File.Exists(fd.FileName))
            {
                StreamReader r = new StreamReader(fd.FileName);

                string line = string.Empty;

                List<string> entries = new List<string>();
                foreach (string item in lstLogos.Items)
                    entries.Add(item.ToString());
                while ((line = r.ReadLine()) != null)
                {
                    entries.Add(line);
                }
                localLogos.saveToDB(entries);
                lstLogos.Items.Clear();
                lstLogos.Items.AddRange(localLogos.getFromDB().ToArray());
                r.Close();

                MPTVSeriesLog.Write(string.Format("{0} Logos sucessfully imported",lstLogos.Items.Count));
            }
        }

        private void nudWatchedAfter_ValueChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cWatchedAfter, (int)nudWatchedAfter.Value);
        }

        private void resetExpr_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (DialogResult.Yes ==
                        MessageBox.Show("You are about to delete all parsing expressions, and replace" + Environment.NewLine +
                            "them with the plugin's defaults." + Environment.NewLine + Environment.NewLine +
                            "Any custom Expressions will be lost, would you like to proceed?", "Reset Expressions", MessageBoxButtons.YesNo))
            {
                dataGridView_Expressions.Rows.Clear();

                DBExpression.ClearAll();
                DBExpression.AddDefaults();

                LoadExpressions();
                MPTVSeriesLog.Write("Expressions reset to defaults");

            }
        }

        private void buildExpr_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //ExpressionBuilder expBldForm = new ExpressionBuilder();
            //expBldForm.ShowDialog();
            //-- ToDo: add result to datagridview
        }

        private void checkDownloadEpisodeSnapshots_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cGetEpisodeSnapshots, checkDownloadEpisodeSnapshots.Checked);
        }

        private void checkBox_Series_UseSortName_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cSeries_UseSortName, checkBox_Series_UseSortName.Checked);
            LoadTree();
        }

        private void comboOnlineLang_DropDown(object sender, EventArgs e)
        {
            if (onlineLanguages.Count != 0) return;
            // get the online languages from the interface
            MPTVSeriesLog.Write("Retrieving a list of languages from online");
            onlineLanguages.AddRange(new GetLanguages().languages);
            string selectedLanguage = DBOption.GetOptions(DBOption.cOnlineLanguage);
            foreach (Language lang in onlineLanguages)
            {
                comboOnlineLang.Items.Add(lang.language);
                if (lang.id.ToString() == selectedLanguage) comboOnlineLang.SelectedItem = lang.language;
                if (lang.abbreviation == selectedLanguage) comboOnlineLang.SelectedItem = lang.language;
            }
        }

        private void lnkResetView_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DBView.ClearAll();
            DBView.fillDefaults();
            LoadViews();
        }

        private void txtUserID_TextChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cOnlineUserID, txtUserID.Text);
        }        

        private void btnLogoDeleteAll_Click(object sender, EventArgs e)
        {
            if (lstLogos.Items.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to delete all Logo Rules?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;

                List<string> entries = new List<string>();
                localLogos.saveToDB(entries);
                lstLogos.Items.Clear();
            }
        }

        private void checkBox_AutoChooseOrder_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cAutoChooseOrder, checkBox_AutoChooseOrder.Checked);
        }

        private void chkShowSeriesFanart_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cShowSeriesFanart, chkShowSeriesFanart.Checked);
        }

        private void comboBox_PosterSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            DBSeries series = (DBSeries)treeView_Library.SelectedNode.Tag;
     
            series.Poster = ((BannerComboItem)comboBox_PosterSelection.SelectedItem).sFullPath;
            try
            {
                this.pictureBox_SeriesPoster.Image = ImageAllocator.LoadImageFastFromFile(ImageAllocator.ExtractFullName(((BannerComboItem)comboBox_PosterSelection.SelectedItem).sFullPath)); // Image.FromFile(ImageAllocator.ExtractFullName(((BannerComboItem)comboBox_BannerSelection.SelectedItem).sFullPath));
            }
            catch (Exception)
            {
            }
            series.Commit();
        }
    
        private void checkBox_Episode_OnlyShowLocalFiles_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cView_Episode_OnlyShowLocalFiles, checkBox_Episode_OnlyShowLocalFiles.Checked);
        }

        private void optionAsk2Rate_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cAskToRate, optionAsk2Rate.Checked);
        }

        private void linkAccountID_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://thetvdb.com/?tab=userinfo");
        }

        private void linkExpressionHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://forum.team-mediaportal.com/my-tvseries-162/expressions-rules-requests-21978/");
        }

        /*private void chkOnlineFavourites_CheckedChanged(object sender, EventArgs e)
        {
            DBView view = new DBView(1);
            if (!chkOnlineFavourites.Checked)
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
            DBOption.SetOptions(DBOption.cOnlineFavourites, chkOnlineFavourites.Checked);
        }*/

        private void chkAutoDownloadFanart_CheckedChanged(object sender, EventArgs e)
        {
            // Enable/Disable Fanart controls
            cboFanartResolution.Enabled = chkAutoDownloadFanart.Checked;
            spinMaxFanarts.Enabled = chkAutoDownloadFanart.Checked;

            DBOption.SetOptions(DBOption.cAutoDownloadFanart, chkAutoDownloadFanart.Checked);
        }

        private void cboFanartResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cAutoDownloadFanartResolution, cboFanartResolution.SelectedIndex);
        }

        private void spinMaxFanarts_ValueChanged(object sender, EventArgs e)
        {
            decimal val = spinMaxFanarts.Value;
            DBOption.SetOptions(DBOption.cAutoDownloadFanartCount, val.ToString());
        }

        private void chkUseRegionalDateFormatString_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cUseRegionalDateFormatString, chkUseRegionalDateFormatString.Checked);
        }

        private void checkBox_ScanOnStartup_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cImport_ScanOnStartup, checkBox_ScanOnStartup.Checked);
        }

        private void checkBox_AutoDownloadMissingArtwork_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cAutoDownloadMissingArtwork, checkBox_AutoDownloadMissingArtwork.Checked);
        }

        private void checkBox_AutoUpdateEpisodeRatings_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cAutoUpdateEpisodeRatings, checkBox_AutoUpdateEpisodeRatings.Checked);
        }

        private void checkBox_AutoUpdateAllFanart_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cAutoUpdateAllFanart, checkBox_AutoUpdateAllFanart.Checked);
        }

        private void checkBox_Episode_HideUnwatchedThumbnail_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cView_Episode_HideUnwatchedThumbnail, checkBox_Episode_HideUnwatchedThumbnail.Checked);
        }

        private void btnRemoveView_Click(object sender, EventArgs e) {
            if (availViews.Count == 0)
                return;
            
            // Get Selected View from list
            selectedView = Helper.getElementFromList<logicalView, string>((string)_availViews.SelectedItem, "Name", 0, availViews);
            
            // Confirm Delete
            string message = string.Format("Are you sure you want to delete view \"{0}\"?",selectedView.prettyName);
            DialogResult result = MessageBox.Show(message, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			if (result == DialogResult.No)
				return;

            // Get All current Views
            DBView[] views = DBView.getAll(true);

            // Remove all Rows from Database
            DBView.ClearAll();

            int index = 0;

            // Add Rows back excluding deleted one
            foreach (DBView view in views) {
                if (view[DBView.cIndex] != int.Parse(selectedView.m_uniqueID)) {
                    DBView newView = new DBView();

                    newView[DBView.cIndex] = index;
                    newView[DBView.cEnabled] = view[DBView.cEnabled];
                    newView[DBView.cSort] = view[DBView.cSort];
                    newView[DBView.cTransToken] = view[DBView.cTransToken];
                    newView[DBView.cPrettyName] = view[DBView.cPrettyName];
                    newView[DBView.cViewConfig] = view[DBView.cViewConfig];
                    newView[DBView.cTaggedView] = view[DBView.cTaggedView];
					newView[DBView.cParentalControl] = view[DBView.cParentalControl];
                    newView.Commit();
                    index++;
                }
            }
            
            // Reload List and available Views
            LoadViews();
            
			// Select First Item in list
			if (_availViews.Items.Count > 0)
				_availViews.SelectedIndex = 0;
        }

        private void btnAddView_Click(object sender, EventArgs e) {
			ViewsConfiguration viewConfigDialog = new ViewsConfiguration();

			viewConfigDialog.ViewName = string.Empty;
			viewConfigDialog.Type = ViewsConfiguration.ViewType.SIMPLE;

			DialogResult result = viewConfigDialog.ShowDialog(this);
			if (result == DialogResult.OK) {
				// Add New view
				if (viewConfigDialog.Type == ViewsConfiguration.ViewType.SIMPLE) {
					// Get List of current views
					List<logicalView> views = logicalView.getAll(true);

					// Ensure index is unique...assumes index is updated when deleting views
					int index = views.Count;
					string name = viewConfigDialog.ViewName;
                    
                    // Check if view already exists
                    foreach (logicalView view in views) {
                        if (view.Name == name) {
                            MessageBox.Show(string.Format("The view \"{0}\" already exists, you must enter in a unique name.",name),"Views",MessageBoxButtons.OK,MessageBoxIcon.Information);
                            return;
                        }
                    }

                    string config = DBView.GetTaggedViewConfigString(name);
                    
	                // Add new 'Simple' view                    
					DBView.AddView(index, name, name, config, true);

                    // Add / Remove series from view                    
                    if (viewConfigDialog.SeriesToAdd != null && viewConfigDialog.SeriesToRemove != null) {                                                                        
                        // Add series to view
                        foreach (DBSeries series in viewConfigDialog.SeriesToAdd) {
                            MPTVSeriesLog.Write(string.Format("Adding series \"{0}\" to \"{1}\"", series.ToString(), name));
                            series[DBOnlineSeries.cViewTags] = Helper.GetSeriesViewTags(series, true, name);                            
                            series.Commit();
                            
                            // Add from online database
                            if (name == DBView.cTranslateTokenOnlineFavourite) {
                                Online_Parsing_Classes.OnlineAPI.ConfigureFavourites(true, DBOption.GetOptions(DBOption.cOnlineUserID), series[DBOnlineSeries.cID]);
                            }
                        }

                        // Remove series from view
                        foreach (DBSeries series in viewConfigDialog.SeriesToRemove) {
                            MPTVSeriesLog.Write(string.Format("Removing series \"{0}\" from \"{1}\"", series.ToString(), name));                            
                            series[DBOnlineSeries.cViewTags] = Helper.GetSeriesViewTags(series,false,name);
                            series.Commit();

                            // Remove from online database
                            if (name == DBView.cTranslateTokenOnlineFavourite) {
                                Online_Parsing_Classes.OnlineAPI.ConfigureFavourites(false, DBOption.GetOptions(DBOption.cOnlineUserID), series[DBOnlineSeries.cID]);
                            }
                        }
                    }					
				}

                // Reload List and available Views
                LoadViews();
			}
        }

        private void buttonEditView_Click(object sender, EventArgs e) {
            ViewsConfiguration viewConfigDialog = new ViewsConfiguration();

            viewConfigDialog.ViewName = _availViews.SelectedItem.ToString();
            viewConfigDialog.Type = ViewsConfiguration.ViewType.SIMPLE;
            viewConfigDialog.Edit = true;

            DialogResult result = viewConfigDialog.ShowDialog(this);
            if (result == DialogResult.OK) {
                // Add New view
                if (viewConfigDialog.Type == ViewsConfiguration.ViewType.SIMPLE) {
                    string name = viewConfigDialog.ViewName;

                    // Add / Remove series from view                    
                    if (viewConfigDialog.SeriesToAdd != null && viewConfigDialog.SeriesToRemove != null) {
                        // Add series to view
                        foreach (DBSeries series in viewConfigDialog.SeriesToAdd) {
                            MPTVSeriesLog.Write(string.Format("Adding series \"{0}\" to \"{1}\"", series.ToString(), name));
                            series[DBOnlineSeries.cViewTags] = Helper.GetSeriesViewTags(series, true, name);
                            series.Commit();

                            // Add from online database
                            if (name == DBView.cTranslateTokenOnlineFavourite) {
                                Online_Parsing_Classes.OnlineAPI.ConfigureFavourites(true, DBOption.GetOptions(DBOption.cOnlineUserID), series[DBOnlineSeries.cID]);
                            }
                        }

                        // Remove series from view
                        foreach (DBSeries series in viewConfigDialog.SeriesToRemove) {
                            MPTVSeriesLog.Write(string.Format("Removing series \"{0}\" from \"{1}\"", series.ToString(), name));
                            series[DBOnlineSeries.cViewTags] = Helper.GetSeriesViewTags(series, false, name);
                            series.Commit();

                            // Remove from online database
                            if (name == DBView.cTranslateTokenOnlineFavourite) {
                                Online_Parsing_Classes.OnlineAPI.ConfigureFavourites(false, DBOption.GetOptions(DBOption.cOnlineUserID), series[DBOnlineSeries.cID]);
                            }
                        }
                    }
                }
            }
        }

        private void checkbox_SortSpecials_CheckedChanged(object sender, EventArgs e) {
            DBOption.SetOptions(DBOption.cSortSpecials, checkbox_SortSpecials.Checked);
        }

        private void numericUpDownBackdropDelay_ValueChanged(object sender, EventArgs e) {
            DBOption.SetOptions(DBOption.cBackdropLoadingDelay, (int)numericUpDownBackdropDelay.Value);
        }

        private void numericUpDownArtworkDelay_ValueChanged(object sender, EventArgs e) {
            DBOption.SetOptions(DBOption.cArtworkLoadingDelay, (int)numericUpDownArtworkDelay.Value);
        }

		private void checkboxRatingDisplayStars_CheckedChanged(object sender, EventArgs e) {
			if (checkboxRatingDisplayStars.Checked)
				DBOption.SetOptions(DBOption.cRatingDisplayStars, 5);
			else
				DBOption.SetOptions(DBOption.cRatingDisplayStars, 10);
		}

        private void checkboxAutoDownloadFanartSeriesName_CheckedChanged(object sender, EventArgs e) {
            DBOption.SetOptions(DBOption.cAutoDownloadFanartSeriesNames, checkboxAutoDownloadFanartSeriesName.Checked);
        }

        private void checkBoxParentalControl_CheckedChanged(object sender, EventArgs e) {
            viewChanged();
        }

        private void buttonPinCode_Click(object sender, EventArgs e) {
            PinCode pinCodeDlg = new PinCode();
            pinCodeDlg.Pin = DBOption.GetOptions(DBOption.cParentalControlPinCode);
            DialogResult result = pinCodeDlg.ShowDialog(this);
            
            // Save Pin Code to Options Table
            if  (result == DialogResult.OK)
                DBOption.SetOptions(DBOption.cParentalControlPinCode, pinCodeDlg.Pin);
        }

        private void buttonViewTemplates_Click(object sender, EventArgs e) {
            ViewTemplates dialog = new ViewTemplates();
            DialogResult result = dialog.ShowDialog(this);

            if (result == DialogResult.OK) {
                int index = logicalView.getAll(true).Count;

                string name = dialog.SelectedItem.name;
                string config = dialog.SelectedItem.configuration;
                bool tagview = dialog.SelectedItem.tagview;

                // Add Template to Views
                DBView.AddView(index, name, config, tagview);

                // Reload List and available Views
                LoadViews();
            }
        }

        private void linkLabelExportStringReplacements_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            DBReplacements[] replacements = DBReplacements.GetAll();
            if (replacements == null || replacements.Length == 0) {
                MessageBox.Show("No valid string replacements to export!");
                return;
            }
            
            SaveFileDialog fd = new SaveFileDialog();
            fd.Filter = "Exported String Replacements (*.strrep)|*.strrep";
            if (fd.ShowDialog() == DialogResult.OK) {
                StreamWriter w = new StreamWriter(fd.FileName);                

                foreach (DBReplacements replacement in replacements) {
                    String val = "";
                    val += (int)replacement[DBReplacements.cEnabled];
                    val += ";";
                    val += (int)replacement[DBReplacements.cBefore];
                    val += ";";
                    val += (int)replacement[DBReplacements.cTagEnabled];
                    val += ";";
                    val += (String)replacement[DBReplacements.cToReplace];
                    val += ";";
                    val += (String)replacement[DBReplacements.cWith];                    
                    
                    try {
                        w.WriteLine((string)val);
                    }
                    catch (IOException exception) {
                        MPTVSeriesLog.Write("String Replacements NOT exported!  Error: " + exception.ToString());
                        return;
                    }
                }
                w.Close();
                MPTVSeriesLog.Write("String Replacements succesfully exported!");
            }
        }

        private void linkLabelImportStringReplacements_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Exported String Replacements (*.strrep)|*.strrep";
            if (fd.ShowDialog() == DialogResult.OK && System.IO.File.Exists(fd.FileName)) {
                StreamReader r = new StreamReader(fd.FileName);
                DBReplacements replacement;

                //Dialog box to make sure they want to clear out current replacements to import new ones.
                if (DialogResult.Yes ==
                    MessageBox.Show("You are about to delete all current string replacements," + Environment.NewLine +
                        "and replace them with the imported file." + Environment.NewLine + Environment.NewLine +
                        "Any current Replacements will be lost.  Would you like to proceed?", "Import Replacements", MessageBoxButtons.YesNo)) {
                    dataGridView_Replace.Rows.Clear();
                    DBReplacements.ClearAll();
                    MPTVSeriesLog.Write("Replacements cleared");
                }

                string line = string.Empty;
                string[] parts;
                int index = 0;

                // now set watched for all in file
                while ((line = r.ReadLine()) != null) {
                    char[] c = { ';' };
                    parts = line.Split(c, 5);
                    if (parts.Length != 5) continue;

                    replacement = new DBReplacements();                   
                    replacement[DBReplacements.cIndex] = index;
                    
                    if (Convert.ToInt32(parts[0]) == 0 || Convert.ToInt32(parts[0]) == 1) replacement[DBReplacements.cEnabled] = parts[0]; else continue;
                    if (Convert.ToInt32(parts[1]) == 0 || Convert.ToInt32(parts[1]) == 1) replacement[DBReplacements.cBefore] = parts[1]; else continue;
                    if (Convert.ToInt32(parts[2]) == 0 || Convert.ToInt32(parts[2]) == 1) replacement[DBReplacements.cTagEnabled] = parts[2]; else continue;

                    replacement[DBReplacements.cToReplace] = parts[3];
                    replacement[DBReplacements.cWith] = parts[4];

                    if (replacement.Commit()) index++;
                }

                r.Close();
                MPTVSeriesLog.Write("String Replacements succesfully imported!");

                LoadReplacements();                
            }
        }

        private void linkLabelResetStringReplacements_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (DialogResult.Yes ==
                        MessageBox.Show("You are about to delete all string replacements, and replace" + Environment.NewLine +
                                        "them with the plugin's defaults." + Environment.NewLine + Environment.NewLine +
                                        "Any custom Replacements will be lost, would you like to proceed?", "Reset Replacements", MessageBoxButtons.YesNo)) {
                dataGridView_Replace.Rows.Clear();

                DBReplacements.ClearAll();
                DBReplacements.AddDefaults();

                LoadReplacements();
                MPTVSeriesLog.Write("Replacements reset to defaults");
            }           
        }

        private void nudScanRemoteShareFrequency_ValueChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cImport_ScanRemoteShareLapse, (int)nudScanRemoteShareFrequency.Value);
        }

        private void checkBox_SubDownloadOnPlay_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cPlay_SubtitleDownloadOnPlay, checkBox_SubDownloadOnPlay.Checked);
        }

        private void subtitleDownloader_enabled_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cSubtitleDownloaderEnabled, subtitleDownloader_enabled.Checked);
        }

        // Set focus on selected item when using Mouse Right Click
        private void treeView_Library_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                treeView_Library.SelectedNode = treeView_Library.GetNodeAt(e.X, e.Y);
            } 
        }
    }
    
    public class BannerComboItem
    {
        public String sName = String.Empty;
        public String sFullPath;

        public BannerComboItem(String sName, String sFullPath)
        {
            this.sName = sName;
            this.sFullPath = sFullPath;
        }

        public override String ToString()
        {
            return sName;
        }

    };

    public class FieldTag
    {
        public String m_sOptionName;
        public Level m_Level;
        public bool m_bInited = false;

        public enum Level
        {
            Series,
            Season,
            Episode
        }

        public FieldTag(String optionName, Level level)
        {
            m_sOptionName = optionName;
            m_Level = level;
        }
    };

    public class DetailsProperty
    {
        String m_Property = String.Empty;
        String m_Value = String.Empty;

        public DetailsProperty(String property, String value)
        {
            this.m_Property = property;
            this.m_Value = value;
        }

        public String Property
        {
            get
            {
                return this.m_Property;
            }
            set
            {
                this.m_Property = value;
            }
        }
        public String Value
        {
            get
            {
                return this.m_Value;
            }
            set
            {
                this.m_Value = value;
            }
        }

    }
}
