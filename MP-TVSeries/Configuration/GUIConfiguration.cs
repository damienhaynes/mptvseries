#region GNU license
// MP-TVSeries - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2019
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

using MediaPortal.Configuration;
using SQLite.NET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Threading.Tasks;
using WindowPlugins.GUITVSeries.Configuration;
using WindowPlugins.GUITVSeries.Feedback;
using WindowPlugins.GUITVSeries.TmdbAPI.Extensions;
using WindowPlugins.GUITVSeries.TmdbAPI;

// TODO: replace all checkboxes that are used to save options with a dboptioncheckbox

namespace WindowPlugins.GUITVSeries
{
    public partial class ConfigurationForm : Form, IFeedback
    {
        private List<Control> mPaneListSettings = new List<Control>();
        private List<Panel> mPaneListExtra = new List<Panel>();
        private TreeNode mNodeEdited = null;
        private OnlineParsing mParser = null;
        private DateTime mTimingStart = new DateTime();

        private DBSeries mSeriesReference = new DBSeries(true);
        private DBSeason mSeasonReference = new DBSeason();
        private DBEpisode mEpisodeReference = new DBEpisode(true);
       
        List<logicalView> mAvailableViews = new List<logicalView>();
        logicalView mSelectedView = null;
        loadingDisplay mLoadDisplay = null;
        List<Language> mOnlineLanguages = new List<Language>();
        bool mInitLoading = true;

        private Control mLocalControlForInvoke;
        private static ConfigurationForm mInstance = null;

        public static ConfigurationForm GetInstance()
        {
            return mInstance;
        }

        public ConfigurationForm()
        {
            mLocalControlForInvoke = new Control();
            mLocalControlForInvoke.CreateControl();

            InitializeComponent();
            
            MPTVSeriesLog.AddNotifier(ref listBox_Log);

            MPTVSeriesLog.Write("Plugin started in configuration mode");

            Translation.Init();

            // set height/width
            int height = DBOption.GetOptions(DBOption.cConfigSizeHeight);
            int width = DBOption.GetOptions(DBOption.cConfigSizeWidth);
            if (height > this.MinimumSize.Height && width > this.MinimumSize.Width)
            {
                Size s = new Size(width, height);
                this.Size = s;
            }
            this.Resize += new EventHandler(ConfigurationForm_Resize);            

            mLoadDisplay = new loadingDisplay();

            OnlineParsing.OnlineParsingCompleted += new OnlineParsing.OnlineParsingCompletedHandler(OnlineParsing_OnCompleted);
            
            InitSettingsTreeAndPanes();
            InitExtraTreeAndPanes();
            
            LoadImportPathes();
            LoadExpressions();
            LoadReplacements();
            
            mInitLoading = false;
            LoadTree();

            // Push Translated Strings to skin
            MPTVSeriesLog.Write( "Setting translated strings: ", MPTVSeriesLog.LogLevel.Debug );
            string propertyName = string.Empty;
            string propertyValue = string.Empty;
            foreach ( string name in Translation.Strings.Keys )
            {
                propertyName = "#TVSeries.Translation." + name + ".Label";
                propertyValue = Translation.Strings[name];
                MPTVSeriesLog.Write( propertyName + " = " + propertyValue, MPTVSeriesLog.LogLevel.Debug );
            }

            DBOption.LogOptions();

            #region TMDb Provider
            var lTmdbTask = Task.Factory.StartNew( () =>
            {
                TmdbAPI.TmdbAPI.UserAgent = Settings.UserAgent;

                // refresh configuration
                var lTmdbConfig = TmdbAPI.TmdbAPI.GetConfiguration();
                DBOption.SetOptions( DBOption.cTmdbConfiguration, lTmdbConfig.ToJSON() );
            } );
            #endregion

            #region Fanart.tv Provider
            // https://fanart.tv/get-an-api-key/
            FanartTvAPI.FanartTvAPI.Init(Settings.UserAgent, "4fe5df4e1a4069867074458c7f9c795e", DBOption.GetOptions(DBOption.cFanartTvClientKey));
            #endregion

            // Only Advanced Users / Skin Designers need to see these.
            // Tabs are visible if import="false" TVSeries.SkinSettings.xml
            if ( SkinSettings.ImportFormatting) tabControl_Details.TabPages.Remove(tabFormattingRules);
            if (SkinSettings.ImportLogos) tabControl_Details.TabPages.Remove(tabLogoRules);
            
            if (mLoadDisplay != null) mLoadDisplay.Close();
            mInstance = this;

            this.aboutScreen.setUpMPInfo(Settings.Version.ToString(), Settings.BuildDate);
            this.aboutScreen.setUpPaths();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
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
            //this.cbOnPlaySeriesOrSeasonAction.SelectedIndex = 2;

            this.splitContainer2.Panel1.SizeChanged += new EventHandler(Panel1_SizeChanged);
            mPaneListSettings.Add(panel_ImportPathes);
            mPaneListSettings.Add(panel_StringReplacements);            
            mPaneListSettings.Add(panel_Expressions);

            foreach (Control pane in mPaneListSettings)
            {
                pane.Dock = DockStyle.Fill;
                pane.Visible = false;
                TreeNode node = new TreeNode(pane.Tag.ToString());
                node.Name = pane.Name;
                treeView_Settings.Nodes.Add(node);
            }

            splitMain_Log.Panel2Collapsed = DBOption.GetOptions(DBOption.cConfigLogCollapsed);
            LogWindowChanged();
            treeView_Settings.SelectedNode = treeView_Settings.Nodes[0];
            nudWatchedAfter.Value = DBOption.GetOptions(DBOption.cWatchedAfter);
            textBox_PluginHomeName.Text = DBOption.GetOptions(DBOption.cPluginName);
            checkBox_FullSeriesRetrieval.Checked = DBOption.GetOptions(DBOption.cFullSeriesRetrieval);
            dbOptCheckBoxCleanOnlineEpisodes.Enabled = checkBox_FullSeriesRetrieval.Checked;
            dbOptCheckBoxRemoveEpZero.Enabled = checkBox_FullSeriesRetrieval.Checked;
            checkBox_AutoChooseSeries.Checked = DBOption.GetOptions(DBOption.cAutoChooseSeries);
            checkBox_AutoChooseOrder.Checked = DBOption.GetOptions(DBOption.cAutoChooseOrder);            
            checkBox_Episode_OnlyShowLocalFiles.Checked = !DBOption.GetOptions(DBOption.cOnlyShowLocalFiles);
            checkBox_Episode_HideUnwatchedSummary.Checked = DBOption.GetOptions(DBOption.cHideUnwatchedSummary);
            checkBox_Episode_HideUnwatchedThumbnail.Checked = DBOption.GetOptions(DBOption.cHideUnwatchedThumbnail);
            checkBox_doFolderWatch.Checked = DBOption.GetOptions(DBOption.cImportFolderWatch);
            checkBox_scanRemoteShares.Checked = DBOption.GetOptions(DBOption.cImportScanRemoteShare);
            nudScanRemoteShareFrequency.Value = DBOption.GetOptions(DBOption.cImportScanRemoteShareLapse);

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
            this.chkAllowDeletes.Checked = (bool)DBOption.GetOptions(DBOption.cShowDeleteMenu);
            this.chkUseRegionalDateFormatString.Checked = (bool)DBOption.GetOptions(DBOption.cAltImgLoading);
            checkDownloadEpisodeSnapshots.Checked = DBOption.GetOptions(DBOption.cGetEpisodeSnapshots);
            checkBox_ShowHidden.Checked = DBOption.GetOptions(DBOption.cShowHiddenItems);            
            checkbox_SortSpecials.Checked = DBOption.GetOptions(DBOption.cSortSpecials);
            checkBox_ScanOnStartup.Checked = DBOption.GetOptions(DBOption.cImportScanOnStartup);
            if (!checkBox_ScanOnStartup.Checked)
            {
                lblImportDelayCaption.Enabled = false;
                lblImportDelaySecs.Enabled = false;
                numericUpDownImportDelay.Enabled = false;
            }
            numericUpDownImportDelay.Value = DBOption.GetOptions(DBOption.cImportDelay);

            checkBox_AutoDownloadMissingArtwork.Checked = DBOption.GetOptions(DBOption.cAutoDownloadMissingArtwork);
            checkBox_AutoUpdateAllFanart.Checked = DBOption.GetOptions(DBOption.cAutoUpdateAllFanart);
			numericUpDownArtworkDelay.Value = DBOption.GetOptions(DBOption.cArtworkLoadingDelay);
			numericUpDownBackdropDelay.Value = DBOption.GetOptions(DBOption.cBackdropLoadingDelay);
            nudConsecFailures.Value = DBOption.GetOptions(DBOption.cMaxConsecutiveDownloadErrors);

			if (DBOption.GetOptions(DBOption.cRatingDisplayStars) == 5)
				checkboxRatingDisplayStars.Checked = true;

            int nValue = DBOption.GetOptions(DBOption.cImportAutoUpdateOnlineDataLapse);            
            numericUpDown_AutoOnlineDataRefresh.Value = nValue;
            checkBox_AutoOnlineDataRefresh.Checked = DBOption.GetOptions(DBOption.cImportAutoUpdateOnlineData);
            numericUpDown_AutoOnlineDataRefresh.Enabled = checkBox_AutoOnlineDataRefresh.Checked;

            chkAutoDownloadFanart.Checked = DBOption.GetOptions(DBOption.cAutoDownloadFanart);
            cboFanartResolution.SelectedIndex = DBOption.GetOptions(DBOption.cAutoDownloadFanartResolution);
            spinMaxFanarts.Value = DBOption.GetOptions(DBOption.cAutoDownloadFanartCount);
            checkboxAutoDownloadFanartSeriesName.Checked = DBOption.GetOptions(DBOption.cAutoDownloadFanartSeriesNames);

            txtFanartTVClientKey.Text = DBOption.GetOptions( DBOption.cFanartTvClientKey );

            checkBox_Series_UseSortName.Checked = DBOption.GetOptions(DBOption.cUseSortName);                        
            
            richTextBox_seriesFormat_Col1.Tag = new FieldTag(DBOption.cViewSeriesColOne, FieldTag.Level.Series);
            richTextBox_seriesFormat_Col1.Enabled = !SkinSettings.ImportViews;
            FieldValidate(ref richTextBox_seriesFormat_Col1);

            richTextBox_seriesFormat_Col2.Tag = new FieldTag(DBOption.cViewSeriesColTwo, FieldTag.Level.Series);
            richTextBox_seriesFormat_Col2.Enabled = !SkinSettings.ImportViews;
            FieldValidate(ref richTextBox_seriesFormat_Col2);

            richTextBox_seriesFormat_Col3.Tag = new FieldTag(DBOption.cViewSeriesColThree, FieldTag.Level.Series);
            richTextBox_seriesFormat_Col3.Enabled = !SkinSettings.ImportViews;
            FieldValidate(ref richTextBox_seriesFormat_Col3);

            richTextBox_seriesFormat_Title.Tag = new FieldTag(DBOption.cViewSeriesTitle, FieldTag.Level.Series);
            FieldValidate(ref richTextBox_seriesFormat_Title);

            richTextBox_seriesFormat_Subtitle.Tag = new FieldTag(DBOption.cViewSeriesSecondTitle, FieldTag.Level.Series);
            FieldValidate(ref richTextBox_seriesFormat_Subtitle);

            richTextBox_seriesFormat_Main.Tag = new FieldTag(DBOption.cViewSeriesMain, FieldTag.Level.Series);
            FieldValidate(ref richTextBox_seriesFormat_Main);          

            richTextBox_seasonFormat_Col1.Tag = new FieldTag(DBOption.cViewSeasonColOne, FieldTag.Level.Season);
            richTextBox_seasonFormat_Col1.Enabled = !SkinSettings.ImportViews;
            FieldValidate(ref richTextBox_seasonFormat_Col1);

            richTextBox_seasonFormat_Col2.Tag = new FieldTag(DBOption.cViewSeasonColTwo, FieldTag.Level.Season);
            richTextBox_seasonFormat_Col2.Enabled = !SkinSettings.ImportViews;
            FieldValidate(ref richTextBox_seasonFormat_Col2);

            richTextBox_seasonFormat_Col3.Tag = new FieldTag(DBOption.cViewSeasonColThree, FieldTag.Level.Season);
            richTextBox_seasonFormat_Col3.Enabled = !SkinSettings.ImportViews;
            FieldValidate(ref richTextBox_seasonFormat_Col3);

            richTextBox_seasonFormat_Title.Tag = new FieldTag(DBOption.cViewSeasonTitle, FieldTag.Level.Season);
            FieldValidate(ref richTextBox_seasonFormat_Title);

            richTextBox_seasonFormat_Subtitle.Tag = new FieldTag(DBOption.cViewSeasonSecondTitle, FieldTag.Level.Season);
            FieldValidate(ref richTextBox_seasonFormat_Subtitle);

            richTextBox_seasonFormat_Main.Tag = new FieldTag(DBOption.cViewSeasonMain, FieldTag.Level.Season);
            FieldValidate(ref richTextBox_seasonFormat_Main);

            richTextBox_episodeFormat_Col1.Tag = new FieldTag(DBOption.cViewEpisodeColOne, FieldTag.Level.Episode);
            richTextBox_episodeFormat_Col1.Enabled = !SkinSettings.ImportViews;
            FieldValidate(ref richTextBox_episodeFormat_Col1);

            richTextBox_episodeFormat_Col2.Tag = new FieldTag(DBOption.cViewEpisodeColTwo, FieldTag.Level.Episode);
            richTextBox_episodeFormat_Col2.Enabled = !SkinSettings.ImportViews;
            FieldValidate(ref richTextBox_episodeFormat_Col2);

            richTextBox_episodeFormat_Col3.Tag = new FieldTag(DBOption.cViewEpisodeColThree, FieldTag.Level.Episode);
            richTextBox_episodeFormat_Col3.Enabled = !SkinSettings.ImportViews;
            FieldValidate(ref richTextBox_episodeFormat_Col3);

            richTextBox_episodeFormat_Title.Tag = new FieldTag(DBOption.cViewEpisodeTitle, FieldTag.Level.Episode);
            FieldValidate(ref richTextBox_episodeFormat_Title);

            richTextBox_episodeFormat_Subtitle.Tag = new FieldTag(DBOption.cViewEpisodeSecondTitle, FieldTag.Level.Episode);
            FieldValidate(ref richTextBox_episodeFormat_Subtitle);

            richTextBox_episodeFormat_Main.Tag = new FieldTag(DBOption.cViewEpisodeMain, FieldTag.Level.Episode);
            FieldValidate(ref richTextBox_episodeFormat_Main);              

            chkUseRegionalDateFormatString.Checked = DBOption.GetOptions(DBOption.cUseRegionalDateFormatString);

            //if (!(Helper.IsSubCentralAvailableAndEnabled && DBOption.GetOptions(DBOption.cSubCentralEnabled)))
            //    dbOptChkBox_SubCentral_DownloadSubtitlesOnPlay.Visible = false;

            dbOptCheckBoxRemoveEpZero.Enabled = DBOption.GetOptions(DBOption.cCleanOnlineEpisodes);

            checkBox_OverrideComboLang.Checked = DBOption.GetOptions(DBOption.cOverrideLanguage);

            dtpParentalAfter.Text = DBOption.GetOptions(DBOption.cParentalControlDisableAfter);
            dtpParentalBefore.Text = DBOption.GetOptions(DBOption.cParentalControlDisableBefore);

            tabControl_Details.SelectTab(1);
        }

        void Panel1_SizeChanged(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void InitExtraTreeAndPanes()
        {
            if (!SkinSettings.ImportLogos) {
                lstLogos.Items.AddRange(localLogos.getFromDB().ToArray());
            }

            // Get Online Languages and fill language combobox
            // do this in background thread so doesn't lockup during load
            LoadOnlineLanguages();
            LoadViews();

			// Select First Item in list
			if (_availViews.Items.Count > 0)
				_availViews.SelectedIndex = 0;

            this.nudParentalControlTimeout.Value = DBOption.GetOptions(DBOption.cParentalControlResetInterval);

            MPTVSeriesLog.pauseAutoWriteDB = false;

            this.cbOnPlaySeriesOrSeasonAction.SelectedIndex = (int)DBOption.GetOptions(DBOption.cOnPlaySeriesOrSeasonAction);
            
            this.cbNewEpisodeThumbIndicator.SelectedIndex = (int)DBOption.GetOptions(DBOption.cNewEpisodeThumbType);
            this.nudRecentlyAddedDays.Value = DBOption.GetOptions(DBOption.cNewEpisodeRecentDays);
			
            // only enable days options if using a 'true' new episode type
            if ((int)DBOption.GetOptions(DBOption.cNewEpisodeThumbType) == (int)NewEpisodeIndicatorType.none ||
                (int)DBOption.GetOptions(DBOption.cNewEpisodeThumbType) == (int)NewEpisodeIndicatorType.unwatched)
            {
                this.nudRecentlyAddedDays.Enabled = false;
                this.lblRecentAddedDays.Enabled = false;
            }

        }

        #region Online Languages
        private void LoadOnlineLanguages()
        {
            BackgroundWorker onlineLangDownloader = new BackgroundWorker();
            onlineLangDownloader.DoWork += new DoWorkEventHandler(GetOnlineLanaguages);
            onlineLangDownloader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GetOnlineLanaguages_Completed);
            onlineLangDownloader.RunWorkerAsync();
        }

        private void GetOnlineLanaguages_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            // we successfully got langages and can allow user to select
            if (mOnlineLanguages.Count != 0)
            {
                MPTVSeriesLog.Write("Successfully retrieved {0} languages from online", mOnlineLanguages.Count);

                if (!DBOption.GetOptions(DBOption.cOverrideLanguage))
                {
                    comboOnlineLang.Enabled = true;
                }
            }
        }

        private void GetOnlineLanaguages(object sender, DoWorkEventArgs e)
        {            
            MPTVSeriesLog.Write("Retrieving a list of languages from online");
            
            comboOnlineLang.Enabled = false;

            // get the online languages from the interface
            if (mOnlineLanguages.Count == 0)
            {
                mOnlineLanguages.AddRange(new GetLanguages().Languages);
            }

            // Not necessary to read into the combobox if not used!
            if (!DBOption.GetOptions(DBOption.cOverrideLanguage))
            {
                string selectedLanguage = DBOption.GetOptions(DBOption.cOnlineLanguage);
                foreach (Language lang in mOnlineLanguages)
                {
                    comboOnlineLang.Items.Add( lang );
                    if (lang.Abbreviation == selectedLanguage) comboOnlineLang.SelectedItem = lang;
                }
            }
        }
        #endregion

        private void LoadViews()
        {
            mAvailableViews.Clear();
            mAvailableViews = logicalView.getAll(true); //include disabled
            _availViews.Items.Clear();
            foreach (logicalView view in mAvailableViews)
                _availViews.Items.Add(view.Name);
        }

        private void LoadImportPathes()
        {
            if (dataGridView_ImportPathes.Columns.Count == 0)
            {
                DataGridViewCheckBoxColumn columnEnabled = new DataGridViewCheckBoxColumn();
                columnEnabled.Name = "enabled";
                columnEnabled.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                dataGridView_ImportPathes.Columns.Add(columnEnabled);

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

                DataGridViewCheckBoxColumn columnRegex = new DataGridViewCheckBoxColumn();
                columnRegex.Name = DBReplacements.cIsRegex;
                columnRegex.HeaderText = DBReplacements.PrettyFieldName(DBReplacements.cIsRegex);
                columnRegex.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                dataGridView_Replace.Columns.Add(columnRegex);

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
                row.Cells[DBReplacements.cIsRegex].Value = (Boolean)replacement[DBReplacements.cIsRegex];
            }
        }

        public void LoadTree()
        {
            if (mInitLoading) return;
            if (null == mLoadDisplay) mLoadDisplay = new loadingDisplay();
           
            // clear current selection
            treeView_Library.Nodes.Clear();
            pictureBox_Series.Image = null;
            pictureBox_SeriesPoster.Image = null;
            comboBox_BannerSelection.Items.Clear();            
            comboBox_PosterSelection.Items.Clear();
            dataGridView1.Rows.Clear();

            SQLCondition condition = new SQLCondition();
            List<DBSeries> seriesList = DBSeries.Get(condition);
            mLoadDisplay.updateStats(seriesList.Count, 0, 0);
            List<DBSeason> allSeasons = DBSeason.Get(new SQLCondition(), false);
            mLoadDisplay.updateStats(seriesList.Count, allSeasons.Count, 0);            
            SQLiteResultSet results = DBTVSeries.Execute("select count(*) from online_episodes");

            mLoadDisplay.updateStats(seriesList.Count, allSeasons.Count, int.Parse(results.GetRow(0).fields[0]));
            aboutScreen.setUpLocalInfo(seriesList.Count, allSeasons.Count, int.Parse(results.GetRow(0).fields[0]));

            if (seriesList.Count == 0)
            {
                mLoadDisplay.Close();
                mLoadDisplay = null;
                return;
            }

            // sort specials at end of season list if needed
            allSeasons.Sort();

            int index = 0;
            foreach (DBSeries series in seriesList)
            {
                var seasons = allSeasons.Where(s => s[DBSeason.cSeriesID] == series[DBSeries.cID]).ToList();
                CreateSeriesNode(series, seasons, index++);
            }
            this.ResumeLayout();
            mLoadDisplay.Close();
            mLoadDisplay = null;

            // select the first node
            if (this.treeView_Library.Nodes.Count > 0)
            {
                this.treeView_Library.SelectedNode = this.treeView_Library.Nodes[0];
                this.treeView_Library.Select();
            }
        }

        private void CreateSeriesNode(DBSeries series, int index)
        {
            CreateSeriesNode(series, null, index);
        }

        private void CreateSeriesNode(DBSeries series, List<DBSeason> seasons, int index)
        {
            if (series == null) return;

            string sName = (DBOption.GetOptions(DBOption.cUseSortName) ? series[DBOnlineSeries.cSortName] : series[DBOnlineSeries.cPrettyName]);
            TreeNode seriesNode = new TreeNode(System.Web.HttpUtility.HtmlDecode(sName));
            seriesNode.Name = DBSeries.cTableName;
            seriesNode.Tag = (DBSeries)series;
            this.treeView_Library.Nodes.Insert(index, seriesNode);
            Font fontDefault = treeView_Library.Font;

            // set color for non-local files
            if (series[DBOnlineSeries.cEpisodeCount] == 0)
            {
                seriesNode.ForeColor = System.Drawing.SystemColors.GrayText;
            }
            else
            {
                // set color for watched items
                if (series[DBOnlineSeries.cUnwatchedItems] == 0)
                    seriesNode.ForeColor = System.Drawing.Color.DarkBlue;
            }

            // set FontStyle
            if (series[DBSeries.cHidden])
                seriesNode.NodeFont = new Font(fontDefault.Name, fontDefault.Size, FontStyle.Italic);

            int seriesID = series[DBSeries.cID];

            if (seasons == null)
            {
                seasons = DBSeason.Get(new SQLCondition(new DBSeason(), DBSeason.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal), false);
                if (seasons == null) return;
                
                seasons.Sort();
            }

            foreach (DBSeason season in seasons)
            {
                CreateSeasonTree(seriesNode, season);
            }
        }

        private void CreateSeasonTree(TreeNode seriesNode, DBSeason season)
        {
            Font fontDefault = treeView_Library.Font;

            TreeNode seasonNode = null;
            if (season[DBSeason.cIndex] == 0)
                seasonNode = new TreeNode(Translation.specials);
            else
                seasonNode = new TreeNode(Translation.Season + " " + season[DBSeason.cIndex]);

            seasonNode.Name = DBSeason.cTableName;
            seasonNode.Tag = season;
            seriesNode.Nodes.Add(seasonNode);

            // set no local files color
            if (season[DBSeason.cEpisodeCount] == 0)
            {
                seasonNode.ForeColor = System.Drawing.SystemColors.GrayText;
            }
            else
            {
                // set color for watched season
                if (season[DBSeason.cUnwatchedItems] == 0)
                    seasonNode.ForeColor = System.Drawing.Color.DarkBlue;
            }

            // set FontStyle
            if (season[DBSeason.cHidden])
                seasonNode.NodeFont = new Font(fontDefault.Name, fontDefault.Size, FontStyle.Italic);
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
        #endregion

        #region Import Path Handling

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
                    //dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cRemovable].Value = false;
                    //dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cKeepReference].Value = false;
                    dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cPath].Value = "";
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
                    //dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cRemovable].Value = false;
                    //dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cKeepReference].Value = false;
                    bNewRow = true;
                }

                AddImportPathPopup importPathPopup = new AddImportPathPopup();

                // If Path is defined, set path to default in folder browser dialog
                if (dataGridView_ImportPathes.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                    importPathPopup.SelectedPath = dataGridView_ImportPathes.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                // Open Folder Browser Dialog 
                importPathPopup.Owner = this;
                DialogResult result = importPathPopup.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    // Delete this row if user didnt select a path
                    if (bNewRow)
                        dataGridView_ImportPathes.Rows.RemoveAt(e.RowIndex);
                    return;
                }
                if (result == DialogResult.OK)
                {
                    if (!Directory.Exists(importPathPopup.SelectedPath))
                    {
                        MessageBox.Show("Import path entered does not exist or is invalid.", "Import Path", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (bNewRow)
                            dataGridView_ImportPathes.Rows.RemoveAt(e.RowIndex);
                        return;
                    }
                    else if (DBImportPath.GetAll().Select(p => p[DBImportPath.cPath].ToString().ToLowerInvariant()).Contains(importPathPopup.SelectedPath.ToLowerInvariant()))
                    {
                        MessageBox.Show("Import path already exists.", "Import Path", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (bNewRow)
                            dataGridView_ImportPathes.Rows.RemoveAt(e.RowIndex);
                        return;
                    }
                }

                // Set Path value in cell
                dataGridView_ImportPathes.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = importPathPopup.SelectedPath;

                // Update Parsing Test
                //TestParsing_Start(true);
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
            //TestParsing_Start(true);
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
        private void button_TestReparse_Click(object sender, EventArgs e)
        {
            TestParsing_Start(true);
        }

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
        void TestParsing_LocalParseCompleted(IList<parseResult> results)
        {
            MPTVSeriesLog.Write("Parsing test completed");
            TestParsing_FillList(results.ToList<parseResult>());            
        }

        void TestParsing_LocalParseProgress(int nProgress, List<parseResult> results)
        {            
            TestParsing_FillList(results);
        }

        void TestParsing_Start(bool bForceRefresh)
        {
            if (!bForceRefresh && listView_ParsingResults.Items.Count > 0 || mInitLoading)
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

        #endregion

        #region ImportWizard

        private ImportWizard ParsingWizardHost;
        private ImportPanelParsing ParsingWizardParsingPage;
        private ImportPanelSeriesID ParsingWizardSeriesIDPage;
        private ImportPanelEpID ParsingWizardEpIDPage;
        private ImportPanelProgress ParsingWizardProgress;

        private CParsingParameters ImportWizardParseParams;

        private void buttonStartImport_Click(object sender, EventArgs e)
        {
            EnableImportButtonState(false);

            if (mParser != null)
            {
                AbortImport();
            }
            else
                StartImport();
        }

        public void AbortImport()
        {
            if (mParser != null)
            {
                mParser.Cancel();               
            }

            // remove the progress page
            ImportWizard ipp = null;
            foreach (var control in this.tabPage_Import.Controls)
            {
                if (control is ImportWizard)
                {
                    ipp = control as ImportWizard;
                    break;
                }
            }

            if (ipp != null)
                this.tabPage_Import.Controls.Remove(ipp);
        }

        public void EnableImportButtonState(bool enable)
        {
            this.buttonStartImport.Enabled = enable;
        }

        private void StartImport()
        {
            ImportWizardParseParams = new CParsingParameters(true, true);

            if (mParser == null)
            {
                // refresh regex and replacements
                FilenameParser.reLoadExpressions();

                ParsingWizardHost = new ImportWizard(this);
                ParsingWizardParsingPage = new ImportPanelParsing();
                ParsingWizardSeriesIDPage = new ImportPanelSeriesID();
                ParsingWizardEpIDPage = new ImportPanelEpID();
                ParsingWizardProgress = new ImportPanelProgress();                

                tabPage_Import.Controls.Add(ParsingWizardHost);
                ParsingWizardHost.Dock = DockStyle.Fill;
                ParsingWizardHost.BringToFront();
                ParsingWizardHost.SetButtonState(ImportWizard.WizardButton.Prev, false);
                ParsingWizardHost.SetButtonState(ImportWizard.WizardButton.Next, false);
                ParsingWizardHost.SetButtonState(ImportWizard.WizardButton.Cancel, false);

                ParsingWizardParsingPage.ParsingGridPopulated += new ImportPanelParsing.ParsingGridPopulatedDelegate(ImportWizard_OnParsingGridPopulated);

                // now have it host the the initial parsing page                 
                ParsingWizardHost.ShowDetailsPanel(ParsingWizardParsingPage);
                ParsingWizardHost.AddSleepingDetailsPanel(ParsingWizardEpIDPage);                

                // and fire off work on that page
                ParsingWizardParsingPage.Init();
                ParsingWizardParsingPage.UserFinishedEditing += new UserFinishedEditingDelegate(ImportWizard_OnFinishedLocalParsing);               
            }
        }

        #region Import Wizard Events
        private void ImportWizard_OnFinishedLocalParsing(UserInputResults values, UserFinishedRequestedAction reqAction)
        {
            ParsingWizardParsingPage.ParsingGridPopulated -= new ImportPanelParsing.ParsingGridPopulatedDelegate(ImportWizard_OnParsingGridPopulated);

            ParsingWizardHost.RemoveDetailsPanel(ParsingWizardParsingPage);
            if (reqAction == UserFinishedRequestedAction.Next)
            {
                ParsingWizardHost.SetButtonState(ImportWizard.WizardButton.Prev, false);
                ParsingWizardHost.SetButtonState(ImportWizard.WizardButton.Next, false);

                // show the seriesIdentification Page
                ParsingWizardHost.RemoveDetailsPanel(ParsingWizardParsingPage);
                ParsingWizardHost.ShowDetailsPanel(ParsingWizardSeriesIDPage);
                
                ParsingWizardSeriesIDPage.SeriesGridPopulated += new ImportPanelSeriesID.SeriesGridPopulatedDelegate(ImportWizard_OnSeriesGridPopulated);
                ParsingWizardSeriesIDPage.UserFinishedEditing += new UserFinishedEditingDelegate(ImportWizard_OnFinishedSeriesID);

                ParsingWizardSeriesIDPage.Init(values.ParseResults);                                
            }
            else if (reqAction == UserFinishedRequestedAction.Cancel)
            {
                
            }
        }

        private void ImportWizard_OnFinishedSeriesID(UserInputResults inputResults, UserFinishedRequestedAction requestAction)
        {
            ParsingWizardSeriesIDPage.UserFinishedEditing -= new UserFinishedEditingDelegate(ImportWizard_OnFinishedSeriesID);

            ParsingWizardHost.RemoveDetailsPanel(ParsingWizardSeriesIDPage);

            if (requestAction == UserFinishedRequestedAction.Next)
            {
                mParser = new OnlineParsing(this);

                // and give it to the wizard
                ParsingWizardHost.Init();

                // now show generic progress details (remove seriesIDPage)
                ParsingWizardHost.RemoveDetailsPanel(ParsingWizardSeriesIDPage);                                
                ParsingWizardHost.ShowDetailsPanel(ParsingWizardProgress);
                ParsingWizardProgress.Init();

                ParsingWizardHost.SetButtonState(ImportWizard.WizardButton.Prev, false);
                ParsingWizardHost.SetButtonState(ImportWizard.WizardButton.Next, false);

                ParsingWizardHost.ImportFinished += new EventHandler(ImportWizard_OnFinished);          

                // only now do we set up the parser itself and fire it off
                ImportWizardParseParams.UserInputResult = inputResults;

                // this will be requested by the the parsing engine at the appropriate time
                ImportWizardParseParams.UserEpisodeMatcher = ParsingWizardEpIDPage;
                ImportWizardParseParams.Files = ParsingWizardParsingPage.allFoundFiles; // else they will be marked as removed

                ParsingWizardEpIDPage.UserFinishedEditing += new UserFinishedEditingDelegate(ImportWizard_OnFinishedEditingEpisodes);

                // finally fire it off
                mTimingStart = DateTime.Now;                
                mParser.Start(ImportWizardParseParams);
            }
            else if (requestAction == UserFinishedRequestedAction.Prev)
            {
                // unregister events
                ParsingWizardSeriesIDPage.UserFinishedEditing -= new UserFinishedEditingDelegate(ImportWizard_OnFinishedSeriesID);
                ParsingWizardSeriesIDPage.SeriesGridPopulated -= new ImportPanelSeriesID.SeriesGridPopulatedDelegate(ImportWizard_OnSeriesGridPopulated);

                // remove existing panel and show previous one
                ParsingWizardHost.SetButtonState(ImportWizard.WizardButton.Prev, false);
                ParsingWizardHost.ShowDetailsPanel(ParsingWizardParsingPage);
                ParsingWizardHost.RemoveDetailsPanel(ParsingWizardSeriesIDPage);
                ParsingWizardParsingPage.Init();
                ParsingWizardSeriesIDPage.ClearResults();                                
            }
            else if (requestAction == UserFinishedRequestedAction.Cancel)
            {
                
            }
        }

        private void ImportWizard_OnParsingGridPopulated()
        {
            // user can now go forwards or cancel (back is the same on 1st step)            
            ParsingWizardHost.SetButtonState(ImportWizard.WizardButton.Next, true);
            ParsingWizardHost.SetButtonState(ImportWizard.WizardButton.Cancel, true);
        }

        private void ImportWizard_OnSeriesGridPopulated()
        {
            // user can now go forwards or back
            ParsingWizardHost.SetButtonState(ImportWizard.WizardButton.Prev, true);
            ParsingWizardHost.SetButtonState(ImportWizard.WizardButton.Next, true);
        }

        private void ImportWizard_OnFinishedEditingEpisodes(UserInputResults inputResults, UserFinishedRequestedAction requestAction)
        {
            switch (requestAction)
            {
                case UserFinishedRequestedAction.Cancel:                   
                    ParsingWizardProgress.DeInit();
                    break;

                case UserFinishedRequestedAction.Next:
                    ParsingWizardHost.RemoveDetailsPanel(ParsingWizardEpIDPage);
                    ParsingWizardHost.ShowDetailsPanel(ParsingWizardProgress);
                    ParsingWizardHost.SetButtonState(ImportWizard.WizardButton.Next, false);
                    break;

                case UserFinishedRequestedAction.ShowMe:
                    // it was requested, tell the progress wizard to show it
                    ParsingWizardHost.RemoveDetailsPanel(ParsingWizardProgress);
                    ParsingWizardHost.ShowDetailsPanel(ParsingWizardEpIDPage);
                    ParsingWizardHost.SetButtonState(ImportWizard.WizardButton.Prev, false);
                    ParsingWizardHost.SetButtonState(ImportWizard.WizardButton.Next, true);
                    break;

                default:
                    break;
            }
        }

        private void ImportWizard_OnFinished(object sender, EventArgs e)
        {
            // user clicked finished (can only do this after import wizard is complete)
            ParsingWizardProgress.DeInit();
            this.tabPage_Import.Controls.Remove(ParsingWizardHost);
            EnableImportButtonState(true);
        }
        #endregion

        #endregion

        private void Parsing_Start()
        {
            Parsing_Start(new CParsingParameters(true, true));
        }

        private void Parsing_Start(CParsingParameters parsingParams)
        {
            if (mParser == null)
            {
                // refresh regex and replacements
                FilenameParser.reLoadExpressions();

                mParser = new OnlineParsing(this);
                mParser.Start(parsingParams);
            }
        }

        private void OnlineParsing_OnCompleted(bool newEpisodes)
        {
            TimeSpan span = DateTime.Now - mTimingStart;
            MPTVSeriesLog.Write("Online Parsing Completed in " + span);           
            mParser = null;
            ImportWizardParseParams = null;
            DBOption.SetOptions(DBOption.cImportOnlineUpdateScanLastTime, DateTime.Now.ToString());
            LoadTree();

            if (ParsingWizardHost != null && tabPage_Import.Contains(ParsingWizardHost))
            {                
                ParsingWizardHost.SetButtonVisibility(ImportWizard.WizardButton.Finish, true);
                ParsingWizardHost.SetButtonVisibility(ImportWizard.WizardButton.Cancel, false);
                ParsingWizardHost.SetButtonState(ImportWizard.WizardButton.Next, false);
                ParsingWizardHost.SetButtonState(ImportWizard.WizardButton.Prev, false);
            }

            EnableImportButtonState(true);
        }

        #region Series Details Tab Handling
        private DBSeries mSelectedSeries;
        private DBSeason mSelectedSeason;
        private DBEpisode mSelectedEpisode;
        private SelectedViewStep mSelectedStep = SelectedViewStep.Series;

        private void treeView_Library_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.dataGridView1.SuspendLayout();
            foreach (Control c in dataGridView1.Controls)
                c.SuspendLayout();
            //////////////////////////////////////////////////////////////////////////////
            #region Clears all fields so new data can be entered

            this.dataGridView1.Rows.Clear();
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
                        mSelectedStep = SelectedViewStep.Episode;

                        DBEpisode episode = (DBEpisode)node.Tag;
                        mSelectedEpisode = episode;
                        // Updated selected series if bypassed in tree view
                        if ( mSelectedEpisode[DBOnlineEpisode.cSeriesID] != mSelectedSeries[DBOnlineSeries.cID] )
                        {
                            mSelectedSeries = Helper.getCorrespondingSeries( mSelectedEpisode[DBOnlineEpisode.cSeriesID] );
                        }

                        #region Images
                        comboBox_BannerSelection.Items.Clear();
                        comboBox_PosterSelection.Items.Clear();

                        pictureBox_Series.SizeMode = PictureBoxSizeMode.Zoom;

                        // if we have logos add them to the list
                        string logos = localLogos.getLogos(ref episode, 200, 500, true);
                        if (logos != null && logos.Length > 0)
                        {
                            BannerComboItem newItem = new BannerComboItem("EpisodeImage/Logos", logos);
                            comboBox_BannerSelection.Items.Add(newItem);
                            comboBox_BannerSelection.SelectedIndex = 0; // force the display
                            comboBox_BannerSelection.Enabled = true;
                        }
                        else
                            comboBox_BannerSelection.Enabled = false;

                        comboBox_PosterSelection.Enabled = false;
                        #endregion

                        // go over all the fields, (and update only those which haven't been modified by the user - will do that later)
                        foreach (String key in episode.FieldNames)
                        {
                            switch (key)
                            {                                                                
                                case DBEpisode.cFilename:
                                    // Read Only Fields
                                    if ( !string.IsNullOrEmpty( episode[key] ) )
                                    {
                                        AddPropertyBindingSource( DBEpisode.PrettyFieldName( key ), key, episode[key], false );
                                    }
                                    break;

                                case DBOnlineEpisode.cRating:
                                    if (!String.IsNullOrEmpty(episode[key]))
                                    {
                                        decimal.TryParse(episode[key].ToString(), out decimal val);
                                        string score = val.ToString("#.#");
                                        string votes = episode[DBOnlineEpisode.cRatingCount];
                                        if (!String.IsNullOrEmpty(votes))
                                        {
                                            score = string.Format("{0} ({1} votes)", score, votes);
                                        }
                                        AddPropertyBindingSource(DBEpisode.PrettyFieldName(key), key, score, false);
                                    }
                                    break;

                                case DBEpisode.cEpisodeName:
                                    AddPropertyBindingSource(DBEpisode.PrettyFieldName(key), DBOnlineEpisode.cEpisodeName, episode[key]);
                                    break;

                                case DBOnlineEpisode.cEpisodeName:
                                case DBOnlineEpisode.cOnlineDataImported:
                                case DBEpisode.cImportProcessed: 
                                case DBEpisode.cCompositeUpdated:
                                case DBEpisode.cVideoHeight:
                                    // hide these fields, they are handled internally
                                    break;
                                
                                case DBEpisode.cSeasonIndex:
                                case DBEpisode.cEpisodeIndex2:
                                case DBEpisode.cSeriesID:
                                case DBEpisode.cCompositeID:
                                case DBEpisode.cCompositeID2:
                                case DBEpisode.cOriginalComposite:
                                case DBEpisode.cOriginalComposite2:
                                case DBEpisode.cStopTime:
                                case DBEpisode.cDateWatched:
                                case DBEpisode.cExtension:
                                case DBEpisode.cIsOnRemovable:
                                case DBOnlineEpisode.cHidden:
                                case DBOnlineEpisode.cWatched:
                                case DBOnlineEpisode.cFirstWatchedDate:
                                case DBOnlineEpisode.cLastWatchedDate:
                                case DBOnlineEpisode.cPlayCount:
                                case DBOnlineEpisode.cEpisodeThumbnailFilename:
                                case DBOnlineEpisode.cEpisodeThumbnailUrl:
                                case DBOnlineEpisode.cEpisodeThumbnailSource:
                                case DBOnlineEpisode.cCombinedEpisodeNumber:
                                case DBOnlineEpisode.cCombinedSeason:
                                case DBOnlineEpisode.cDVDSeasonNumber:
                                case DBOnlineEpisode.cEpisodeImageFlag:
                                case DBOnlineEpisode.cIMDBID:
                                case DBOnlineEpisode.cLanguage:
                                case DBOnlineEpisode.cProductionCode:
                                case DBOnlineEpisode.cSeasonID:
                                case DBOnlineEpisode.cDVDChapter:
                                case DBOnlineEpisode.cDVDDiscID:
                                case DBEpisode.cIsAvailable:
                                case DBOnlineEpisode.cRatingCount:
                                case DBOnlineEpisode.cMyRatingAt:
                                case DBOnlineEpisode.cThumbAdded:
                                case DBOnlineEpisode.cThumbHeight:
                                case DBOnlineEpisode.cThumbWidth:
                                case DBOnlineEpisode.cIsMovie:
                                case DBOnlineEpisode.cAirsAfterSeason:
                                case DBOnlineEpisode.cAirsBeforeEpisode:
                                case DBOnlineEpisode.cAirsBeforeSeason:
                                case DBOnlineEpisode.cTMDbEpisodeThumbnailUrl:
                                    // hide these fields as we are not so interested in, 
                                    // possibly add a toggle option to display all fields later
                                    break;

                                // Show DVD number if applicable
                                case DBOnlineEpisode.cDVDEpisodeNumber:
                                    if ( episode[DBOnlineEpisode.cDVDEpisodeNumber] !=0 && mSelectedSeries[DBOnlineSeries.cEpisodeSortOrder] != "DVD" )
                                    {
                                        AddPropertyBindingSource( DBEpisode.PrettyFieldName( key ), key, $"{episode[DBOnlineEpisode.cDVDSeasonNumber]}x{episode[DBOnlineEpisode.cDVDEpisodeNumber]}", false );
                                    }
                                    break;

                                // Show Aired number if applicable
                                case DBOnlineEpisode.cEpisodeIndex:
                                    if ( mSelectedSeries[DBOnlineSeries.cEpisodeSortOrder] == "DVD" )
                                    {
                                        AddPropertyBindingSource( DBEpisode.PrettyFieldName( key ), key, $"{episode[DBOnlineEpisode.cSeasonIndex]}x{episode[DBOnlineEpisode.cEpisodeIndex]}", false );
                                    }
                                    break;

                                // Show Absolute number if applicable
                                case DBOnlineEpisode.cAbsoluteNumber:
                                    if ( episode[DBOnlineEpisode.cAbsoluteNumber] != 0 && mSelectedSeries[DBOnlineSeries.cEpisodeSortOrder] != "Absolute" )
                                    {
                                        AddPropertyBindingSource( DBEpisode.PrettyFieldName( key ), key, episode[key], false );
                                    }
                                    break;

                                case DBOnlineEpisode.cLastUpdated:
                                    UInt64 lResult;
                                    if ( UInt64.TryParse( episode[key], out lResult ) )
                                    {
                                        DateTime lDateTime = new DateTime( 1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc );
                                        lDateTime = lDateTime.AddSeconds( lResult );

                                        AddPropertyBindingSource( DBEpisode.PrettyFieldName( key ), key, lDateTime.ToString(), false );
                                    }
                                    else if ( !string.IsNullOrEmpty( episode[key] ) )
                                        AddPropertyBindingSource( DBEpisode.PrettyFieldName( key ), key, episode[key], false );
                                    break;

                                case DBEpisode.cVolumeLabel:
                                    if (!String.IsNullOrEmpty(episode[key]))
                                        AddPropertyBindingSource(DBEpisode.PrettyFieldName(key), key, episode[key], false);
                                    break;

                                // read-only fields
                                case DBOnlineEpisode.cID:
                                case DBEpisode.cVideoFrameRate:
                                case DBEpisode.cVideoCodec:
                                case DBEpisode.cVideoFormat:
                                case DBEpisode.cVideoFormatProfile:
                                case DBEpisode.cVideoBitRate:
                                case DBEpisode.cVideoAspectRatio:
                                case DBEpisode.cVideoColourPrimaries:
                                case DBEpisode.cVideoFormatCommercial:
                                case DBEpisode.cAudioCodec:
                                case DBEpisode.cAudioFormat:
                                case DBEpisode.cAudioFormatProfile:
                                case DBEpisode.cAudioChannels:
                                case DBEpisode.cAudioBitrate:
                                case DBEpisode.cFileDateAdded:
                                case DBEpisode.cFileDateCreated:
                                    if (!String.IsNullOrEmpty(episode[key]) && episode[key] != "-1")
                                        AddPropertyBindingSource(DBEpisode.PrettyFieldName(key), key, episode[key], false);
                                    break;

                                case DBEpisode.cAudioLanguage:
                                    if ( !String.IsNullOrEmpty( episode[key] ) && episode[key] != "-1" )
                                        AddPropertyBindingSource( DBEpisode.PrettyFieldName( key ), key, GetAudioLanguageDisplayName( episode[key] ), false );
                                    break;
                                
                                case DBEpisode.cVideoWidth:
                                    if ( !String.IsNullOrEmpty( episode[key] ) && episode[key] != "-1" && episode[key] != "0" )
                                        AddPropertyBindingSource( "Resolution", key, $"{episode[DBEpisode.cVideoWidth]}x{episode[DBEpisode.cVideoHeight]}", false );
                                    break;

                                case DBEpisode.cAudioTracks:
                                case DBEpisode.cTextCount:
                                    if ( !String.IsNullOrEmpty( episode[key] ) && episode[key] != "-1" && episode[key] != "1" )
                                        AddPropertyBindingSource( DBEpisode.PrettyFieldName( key ), key, episode[key], false );
                                    break;

                                case DBEpisode.cAvailableSubtitles:
                                    if ( !String.IsNullOrEmpty( episode[key] ) && episode[key] != "-1" )
                                        AddPropertyBindingSource( DBEpisode.PrettyFieldName( key ), key, episode[key] ? "True" : "False", false );
                                    break;

                                case DBEpisode.cLocalPlaytime:
                                    if (!String.IsNullOrEmpty(episode[key]) && episode[key] != "-1")
                                        AddPropertyBindingSource(DBEpisode.PrettyFieldName(key), key, Helper.MSToMMSS(episode[key]), false);
                                    break;

                                default:
                                    AddPropertyBindingSource(DBEpisode.PrettyFieldName(key), key, System.Web.HttpUtility.HtmlDecode(episode[key]));
                                    break;

                            }
                        }
                        // let configs know what was selected (for samples)
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
                        mSelectedStep = SelectedViewStep.Season;

                        DBSeason season = (DBSeason)node.Tag;
                        mSelectedSeason = season;
                        // Updated selected series if bypassed in tree view
                        if (mSelectedSeason[DBSeason.cSeriesID] != mSelectedSeries[DBOnlineSeries.cID])
                        {
                            mSelectedSeries = Helper.getCorrespondingSeries( mSelectedSeason[DBSeason.cSeriesID] );
                        }

                        #region Images
                        comboBox_BannerSelection.Items.Clear();
                        comboBox_PosterSelection.Items.Clear();

                        // populate banner dropdown
                        foreach (String filename in season.BannerList)
                        {
                            BannerComboItem newItem = new BannerComboItem(Path.GetFileName(filename), filename);
                            comboBox_PosterSelection.Items.Add(newItem);
                        }

                        // if we have logos add them to the list
                        string logos = localLogos.getLogos( ref season, 200, 500 );
                        if ( logos.Length > 0 )
                        {
                            BannerComboItem newItem = new BannerComboItem( "Logos", logos );
                            comboBox_PosterSelection.Items.Add( newItem );
                        }

                        if ( comboBox_PosterSelection.Items.Count > 0 )
                            comboBox_PosterSelection.Enabled = true;
                        else
                            comboBox_PosterSelection.Enabled = false;

                        comboBox_BannerSelection.Enabled = false;

                        if (season.Banner.Length > 0)
                        {
                            try
                            {
                                this.pictureBox_SeriesPoster.Image = ImageAllocator.LoadImageFastFromFile( season.Banner ); //Image.FromFile(season.Banner);
                            }
                            catch ( Exception ) { }

                            foreach ( BannerComboItem comboItem in comboBox_PosterSelection.Items )
                            {
                                if ( comboItem.sFullPath == season.Banner )
                                {
                                    comboBox_PosterSelection.SelectedItem = comboItem;
                                    break;
                                }
                            }
                        }
                        #endregion

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
                                case DBSeason.cHidden:
                                case DBSeason.cMyRatingAt:
                                case DBSeason.cRatingCount:
                                case DBSeason.cID:
                                    // hide these fields as we are not so interested in, 
                                    // possibly add a toggle option to display all fields later
                                    break;

                                case DBSeason.cRating:
                                    if ( !String.IsNullOrEmpty( season[key] ) )
                                    {
                                        decimal val = 0;
                                        decimal.TryParse( season[key].ToString(), out val );
                                        string score = val.ToString( "#.#" );
                                        string votes = season[DBSeason.cRatingCount];
                                        if ( !String.IsNullOrEmpty( votes ) )
                                        {
                                            score = string.Format( "{0} ({1} votes)", score, votes );
                                        }
                                        AddPropertyBindingSource( DBSeason.PrettyFieldName( key ), key, score, false );
                                    }
                                    break;

                                case DBSeason.cMyRating:
                                case DBSeason.cTitle:
                                case DBSeason.cSummary:
                                    AddPropertyBindingSource(DBSeason.PrettyFieldName(key), key, season[key], true);
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
                        mSelectedStep = SelectedViewStep.Series;
                                 
                        DBSeries series = (DBSeries)node.Tag;
                        mSelectedSeries = series;

                        #region Images
                        comboBox_BannerSelection.Items.Clear();
                        comboBox_PosterSelection.Items.Clear();

                        pictureBox_Series.SizeMode = PictureBoxSizeMode.Zoom;

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
                                this.pictureBox_Series.Image = ImageAllocator.LoadImageFastFromFile( series.Banner );
                            }
                            catch ( System.Exception ) { }

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
                        #endregion

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
                                case DBOnlineSeries.cTraktIgnore:
                                case DBOnlineSeries.cMyRatingAt:
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
                                case DBOnlineSeries.cLastUpdatedDetail:
                                case DBOnlineSeries.cPoster:
                                case DBOnlineSeries.cViewTags:
                                case DBOnlineSeries.cBanner:
                                case DBOnlineSeries.cEpisodeOrders:
                                case DBOnlineSeries.cSeriesID:
                                case DBOnlineSeries.cHasNewEpisodes:
                                case DBOnlineSeries.cEpisodeSortOrder:
                                case DBSeries.cHidden:
                                case DBSeries.cScanIgnore:
                                case DBOnlineSeries.cRatingCount:
                                case DBOnlineSeries.cTMSWantedOld:
                                case DBOnlineSeries.cTopSeries:
                                case DBOnlineSeries.cTraktID:
                                case DBOnlineSeries.cSlug:
                                case DBSeries.cParsedName:
                                case DBOnlineSeries.cTmdbId:
                                case DBOnlineSeries.cArtworkChooserProvider:
                                    // hide these fields as we are not so interested in,   
                                    // possibly add a toggle option to display all fields later
                                    break;
                                
                                case DBSeries.cID:
                                case DBOnlineSeries.cEpisodeCount:
                                case DBOnlineSeries.cEpisodesUnWatched:
                                case DBOnlineSeries.cChosenEpisodeOrder:
                                case DBOnlineSeries.cAliasNames:
                                    // fields that can not be modified - read only
                                    if ( !string.IsNullOrEmpty( series[key] ) )
                                        AddPropertyBindingSource(DBSeries.PrettyFieldName(key), key, series[key], false);
                                    break;

                                case DBOnlineSeries.cOriginalName:
                                    if ( !string.IsNullOrEmpty( series[key] ) && ( series[key] != series[DBOnlineSeries.cPrettyName] ) )
                                        AddPropertyBindingSource( DBSeries.PrettyFieldName( key ), key, series[key], false );
                                    break;

                                case DBOnlineSeries.cLastUpdated:
                                    UInt64 lResult;
                                    if ( UInt64.TryParse( series[key], out lResult ) )
                                    {
                                        DateTime lDateTime = new DateTime( 1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc );
                                        lDateTime = lDateTime.AddSeconds( lResult );

                                        AddPropertyBindingSource( DBSeries.PrettyFieldName( key ), key, lDateTime.ToString(), false );
                                    }
                                    else if (!string.IsNullOrEmpty( series[key] ))
                                        AddPropertyBindingSource( DBSeries.PrettyFieldName( key ), key, series[key], false );
                                    break;

                                case DBOnlineSeries.cLanguage:
                                    if ( !String.IsNullOrEmpty(series[key]) && DBOption.GetOptions(DBOption.cOverrideLanguage) )
                                        AddPropertyBindingSource(DBSeries.PrettyFieldName(key), key, series[key]);
                                    break;

                                case DBOnlineSeries.cRating:
                                    if (!String.IsNullOrEmpty(series[key]))
                                    {
                                        decimal val = 0;
                                        decimal.TryParse(series[key].ToString(), out val);
                                        string score = val.ToString("#.#");
                                        string votes = series[DBOnlineSeries.cRatingCount];
                                        if (!String.IsNullOrEmpty(votes))
                                        {
                                            score = string.Format("{0} ({1} votes)", score, votes);
                                        }
                                        AddPropertyBindingSource(DBSeries.PrettyFieldName(key), key, score, false);
                                    }
                                    break;

                                case DBOnlineSeries.cLastEpisodeAirDate:
                                    if (!String.IsNullOrEmpty(series[key]))
                                        AddPropertyBindingSource(DBSeries.PrettyFieldName(key), key, series[key]);
                                    break;

                                default:
                                    AddPropertyBindingSource(DBSeries.PrettyFieldName(key), key, System.Web.HttpUtility.HtmlDecode(series[key]));
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
            foreach (TreeNode childNode in node.Nodes)
            {
                // Check if we have already loaded episodes into season nodes
                if (childNode.Nodes.Count == 0)
                {
                    // ensure we use the correct season field for DVD sort order
                    bool lUseDVDOrder = series[DBOnlineSeries.cEpisodeSortOrder] == "DVD";
                    string lSeasonField = DBOnlineEpisode.cSeasonIndex;
                    
                    DBSeason season = (DBSeason)childNode.Tag;
                    int lSeasonIndex = season[DBSeason.cIndex];

                    if ( lUseDVDOrder && lSeasonIndex > 0 )
                        lSeasonField = DBOnlineEpisode.cDVDSeasonNumber;
                    
                    SQLCondition conditions = new SQLCondition();
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                    conditions.beginGroup();
                    conditions.Add(new DBOnlineEpisode(), lSeasonField, lSeasonIndex, SQLConditionType.Equal);
                    if ( lUseDVDOrder && lSeasonIndex > 0 )
                    {
                        // if we fell back to air order then also get these episodes i.e. DVD_episode = 0
                        conditions.beginGroup();
                        conditions.nextIsOr = true;
                        conditions.AddCustom( $"online_episodes.SeasonIndex = '{lSeasonIndex}' and online_episodes.DVD_episodenumber = '0'" );
                        conditions.nextIsOr = false;
                        conditions.endGroup();
                    }
                    conditions.endGroup();
                    List<DBEpisode> episodes = DBEpisode.Get(conditions);
                    
                    // sort by correct order
                    episodes.Sort();

                    foreach (DBEpisode episode in episodes)
                    {
                        String lEpisodeName = (String)episode[DBEpisode.cEpisodeName];
                        TreeNode lEpisodeNode = null;
                        if ( lUseDVDOrder && episode[DBOnlineEpisode.cDVDEpisodeNumber] != 0 )
                        {
                            lEpisodeNode = new TreeNode( episode[DBOnlineEpisode.cDVDSeasonNumber] + "x" + episode[DBOnlineEpisode.cDVDEpisodeNumber] + " - " + lEpisodeName );
                        }
                        else
                        {
                            lEpisodeNode = new TreeNode( episode[DBOnlineEpisode.cSeasonIndex] + "x" + episode[DBOnlineEpisode.cEpisodeIndex] + " - " + lEpisodeName );
                        }
                        lEpisodeNode.Name = DBEpisode.cTableName;
                        lEpisodeNode.Tag = (DBEpisode)episode;

                        // set color for non-local file
                        if (episode[DBEpisode.cFilename].ToString().Length == 0)
                        {
                            lEpisodeNode.ForeColor = SystemColors.GrayText;
                        }
                        else
                        {
                            // set color for watched episode
                            if (episode[DBOnlineEpisode.cWatched] == 1)
                                lEpisodeNode.ForeColor = Color.DarkBlue;
                        }

                        // set FontStyle for hidden episodes
                        if (episode[DBOnlineEpisode.cHidden])
                            lEpisodeNode.NodeFont = new Font(defaultFont.Name, defaultFont.Size, FontStyle.Italic);

                        childNode.Nodes.Add(lEpisodeNode);
                    }
                }
            }

            #endregion
            //////////////////////////////////////////////////////////////////////////////
        }

        private void treeView_Settings_AfterSelect(object sender, TreeViewEventArgs e)
        {
            foreach (Control pane in mPaneListSettings)
            {
                if (pane.Name == e.Node.Name)
                {
                    pane.Visible = true;
                }
                else
                    pane.Visible = false;
            }
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
            int id = -1;
            bool userEdited = false;
            // are we a user_edited item? if so replace the orig entry
            if (FieldName.EndsWith(DBTable.cUserEditPostFix))
            {
                // except if the content is empty, then we just dont display it
                if (string.IsNullOrEmpty(FieldValue)) return;
                string origFieldName = FieldName.Replace(DBTable.cUserEditPostFix, "");
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (dataGridView1.Rows[i].Cells[1].Tag as string == origFieldName)
                    {
                        id = i;
                        userEdited = true;
                        break;
                    }
                }
            }

            if (!userEdited)
            {
                DataGridViewRow dataGridDetailRow = new DataGridViewRow();
                DataGridViewTextBoxCell cFieldName;

                if ((FieldName == DBOnlineSeries.cLanguage) && (DBOption.GetOptions(DBOption.cOverrideLanguage)))
                {
                    dataGridDetailRow = new DataGridViewRow();
                    cFieldName = new DataGridViewTextBoxCell();
                    DataGridViewComboBoxCell cbCell = new DataGridViewComboBoxCell();

                    // First Column (Name)
                    cFieldName.Value = DBOnlineSeries.s_FieldToDisplayNameMap[FieldName];
                    cFieldName.Style.Alignment = TextAlign;
                    cFieldName.Style.BackColor = SystemColors.Control;
                    dataGridDetailRow.Cells.Add(cFieldName);
                    cFieldName.ReadOnly = true;

                    // Second Column (Value)
                    if (mOnlineLanguages.Count == 0)
                    {
                        mOnlineLanguages.AddRange(new GetLanguages().Languages);
                    }

                    // populate languages drop-down and select overridden language for series
                    string selectedLanguage = mOnlineLanguages.Find( x => x.Abbreviation.Contains( FieldValue ) )?.Abbreviation;

                    foreach (var language in mOnlineLanguages)
                    {
                        cbCell.Items.Add( language.ToString() );
                        if ( language.Abbreviation == FieldValue ) cbCell.Value = language.ToString();
                    }

                    cbCell.Tag = FieldName;
                    cbCell.Style.Alignment = TextAlign;

                    dataGridDetailRow.Cells.Add(cbCell);
                    cbCell.ReadOnly = false;

                    // Add the row to the DataGridView
                    dataGridView1.Rows.Add(dataGridDetailRow);
                }
                else
                {
                    cFieldName = new DataGridViewTextBoxCell();
                    DataGridViewTextBoxCell cFieldValue = new DataGridViewTextBoxCell();

                    // First Column (Name)
                    cFieldName.Value = FieldPrettyName;
                    cFieldName.Style.Alignment = TextAlign;
                    cFieldName.Style.BackColor = SystemColors.Control;
                    dataGridDetailRow.Cells.Add(cFieldName);
                    cFieldName.ReadOnly = true;

                    cFieldValue.Value = FieldValue;
                    cFieldValue.Tag = FieldName;

                    dataGridDetailRow.Cells.Add(cFieldValue);

                    if (!CanModify)
                    {
                        cFieldValue.ReadOnly = true;
                        cFieldValue.Style.BackColor = SystemColors.Control;
                    }

                    cFieldValue.Style.Alignment = TextAlign;

                    // Add the rows to the DataGridView
                    dataGridView1.Rows.Add(dataGridDetailRow);
                }
            }
            else
            {
                // user edit, replace the existing value
                dataGridView1.Rows[id].Cells[1].Value = FieldValue;
                dataGridView1.Rows[id].Cells[1].Style.ForeColor = SystemColors.HotTrack;
            }
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            mNodeEdited = treeView_Library.SelectedNode;           
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[1];
            if (mNodeEdited != null)
            {
                string origFieldName = (string)cell.Tag;
                string editFieldName = origFieldName;

                if (!editFieldName.EndsWith(DBTable.cUserEditPostFix))
                    editFieldName += DBTable.cUserEditPostFix;

                if (origFieldName.EndsWith(DBTable.cUserEditPostFix))
                    origFieldName = origFieldName.Replace(DBTable.cUserEditPostFix, string.Empty);

                bool bUserEdit = true;

                // dont store every edit as a user_edit
                switch (origFieldName)
                {
                    case DBSeries.cScanIgnore:
                    case DBOnlineSeries.cLanguage:
                    case DBOnlineSeries.cSummary:
                    case DBOnlineSeries.cMyRating:
                    case DBOnlineSeries.cMyRatingAt:
                    case DBSeason.cTitle:
                        editFieldName = origFieldName;
                        bUserEdit = false;
                        break;
                }

                string newValue = (String)cell.Value;

                switch (mNodeEdited.Name)
                {
                    case DBSeries.cTableName:
                        DBSeries series = (DBSeries)mNodeEdited.Tag;
                        if ( editFieldName == DBOnlineSeries.cLanguage )
                        {
                            Language selectedLang = mOnlineLanguages.Find(x => x.ToString() == newValue);
                            if (selectedLang != null)
                            {
                                series[editFieldName] = selectedLang.Abbreviation;
                            }
                        }
                        else
                        {
                            series[editFieldName] = newValue;
                        }    
                        series.Commit();

                        if (bUserEdit)
                        {
                            if (string.IsNullOrEmpty(newValue))
                            {
                                // restore the old value in the cell so dont need to reload the datagrid
                                cell.Value = series[origFieldName].ToString();
                                cell.Style.ForeColor = System.Drawing.SystemColors.ControlText;
                            }
                            else
                                cell.Style.ForeColor = System.Drawing.SystemColors.HotTrack;
                        }

                        if (series[DBOnlineSeries.cPrettyName].ToString().Length > 0)
                            mNodeEdited.Text = series[DBOnlineSeries.cPrettyName];
                        break;

                    case DBSeason.cTableName:
                        DBSeason season = (DBSeason)mNodeEdited.Tag;
                        season[editFieldName] = newValue;
                        season.Commit();

                        if (bUserEdit)
                        {
                            if (string.IsNullOrEmpty(newValue))
                            {
                                // restore the old value in the cell so dont need to reload the datagrid
                                cell.Value = season[origFieldName].ToString();
                                cell.Style.ForeColor = System.Drawing.SystemColors.ControlText;
                            }
                            else
                                cell.Style.ForeColor = System.Drawing.SystemColors.HotTrack;
                        }

                        break;

                    case DBEpisode.cTableName:
                        DBEpisode episode = (DBEpisode)mNodeEdited.Tag;

                        if (episode.onlineEpisode.FieldNames.Contains(origFieldName))
                        {
                            episode.onlineEpisode[editFieldName] = newValue;
                        }
                        else
                            episode[editFieldName] = newValue;

                        episode.Commit();

                        if (bUserEdit)
                        {
                            if (string.IsNullOrEmpty(newValue))
                            {
                                // restore the old value in the cell so dont need to reload the datagrid
                                cell.Value = episode[origFieldName].ToString();
                                cell.Style.ForeColor = System.Drawing.SystemColors.ControlText;
                            }
                            else
                                cell.Style.ForeColor = System.Drawing.SystemColors.HotTrack;
                        }

                        if (episode[DBEpisode.cEpisodeName].ToString().Length > 0)
                            mNodeEdited.Text = episode[DBEpisode.cSeasonIndex] + "x" + episode[DBEpisode.cEpisodeIndex] + " - " + episode[DBEpisode.cEpisodeName];
                        break;
                }
            }
        }
        #endregion

        private void checkBox_FullSeriesRetrieval_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cFullSeriesRetrieval, checkBox_FullSeriesRetrieval.Checked);

            dbOptCheckBoxCleanOnlineEpisodes.Enabled = checkBox_FullSeriesRetrieval.Checked;
            dbOptCheckBoxRemoveEpZero.Enabled = checkBox_FullSeriesRetrieval.Checked;
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
            DBOption.SetOptions(DBOption.cOnlyShowLocalFiles, checkBox_Episode_OnlyShowLocalFiles.Checked);
        }

        private void checkBox_Episode_HideUnwatchedSummary_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cHideUnwatchedSummary, checkBox_Episode_HideUnwatchedSummary.Checked);
        }

        private void checkBox_AutoOnlineDataRefresh_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cImportAutoUpdateOnlineData, checkBox_AutoOnlineDataRefresh.Checked);
            numericUpDown_AutoOnlineDataRefresh.Enabled = checkBox_AutoOnlineDataRefresh.Checked;
        }

        private void numericUpDown_AutoOnlineDataRefresh_ValueChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cImportAutoUpdateOnlineDataLapse, (int)numericUpDown_AutoOnlineDataRefresh.Value);
        }

        private void ScanIgnore(TreeNode nodeScanIgnore)
        {

            if (nodeScanIgnore != null)
            {
                if (nodeScanIgnore.Name == DBSeries.cTableName)
                {
                    DBSeries series = (DBSeries)nodeScanIgnore.Tag;
                    series[DBSeries.cScanIgnore] = !series[DBSeries.cScanIgnore];
                    series.Commit();                    
                }

            }
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
                        series.HideSeries(!series[DBSeries.cHidden]);
                        bHidden = series[DBSeries.cHidden];                        
                        break;

                    case DBSeason.cTableName:
                        DBSeason season = (DBSeason)nodeHidden.Tag;
                        season.HideSeason(!season[DBSeason.cHidden]);
                        bHidden = season[DBSeason.cHidden];                        
                        break;

                    case DBEpisode.cTableName:
                        DBEpisode episode = (DBEpisode)nodeHidden.Tag;
                        episode.HideEpisode(!episode[DBOnlineEpisode.cHidden]);
                        bHidden = episode[DBOnlineEpisode.cHidden];                        
                        break;
                }

                if (DBOption.GetOptions(DBOption.cShowHiddenItems))
                {
                    // change the font
                    if (bHidden)
                    {
                        Font fontDefault = treeView_Library.Font;
                        nodeHidden.NodeFont = new Font(fontDefault.Name, fontDefault.Size, FontStyle.Italic);
                        
                        // change season and episode nodes as well
                        if (nodeHidden.Name == DBSeries.cTableName)
                        {
                            foreach (TreeNode seasonNode in nodeHidden.Nodes)
                            {
                                seasonNode.NodeFont = new Font(fontDefault.Name, fontDefault.Size, FontStyle.Italic);
                                foreach (TreeNode episodeNode in seasonNode.Nodes)
                                {
                                    episodeNode.NodeFont = new Font(fontDefault.Name, fontDefault.Size, FontStyle.Italic);
                                }
                            }
                        }

                        if (nodeHidden.Name == DBSeason.cTableName)
                        {
                            foreach (TreeNode episodeNode in nodeHidden.Nodes)
                            {
                                episodeNode.NodeFont = new Font(fontDefault.Name, fontDefault.Size, FontStyle.Italic);
                            }
                        }
                    }
                    else
                    {
                        nodeHidden.NodeFont = treeView_Library.Font;
                        // change season and episode nodes as well
                        if (nodeHidden.Name == DBSeries.cTableName)
                        {
                            foreach (TreeNode seasonNode in nodeHidden.Nodes)
                            {
                                seasonNode.NodeFont = treeView_Library.Font;
                                foreach (TreeNode episodeNode in seasonNode.Nodes)
                                {
                                    episodeNode.NodeFont = treeView_Library.Font;
                                }
                            }
                        }

                        if (nodeHidden.Name == DBSeason.cTableName)
                        {
                            foreach (TreeNode episodeNode in nodeHidden.Nodes)
                            {
                                episodeNode.NodeFont = treeView_Library.Font;
                            }
                        }
                    }
                }
                else
                {
                    // just remove the node
                    treeView_Library.Nodes.Remove(nodeHidden);
                }
            }
        }

        private void UpdateNode(TreeNode aNodeUpdated)
        {
            if (aNodeUpdated != null)
            {
                int lSeriesID = 0;
                var lConditions = new SQLCondition();
                var lEpisodeIdUpdates = new List<DBValue>();
                var lSeriesIdUpdates = new List<int>();

                switch (aNodeUpdated.Name)
                {
                    case DBSeries.cTableName:
                        var lSeries = aNodeUpdated.Tag as DBSeries;
                        lSeriesIdUpdates.Add(lSeries[DBSeries.cID]);
                        lConditions = new SQLCondition(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, lSeries[DBSeries.cID], SQLConditionType.Equal);
                        lEpisodeIdUpdates.AddRange(DBEpisode.GetSingleField(DBOnlineEpisode.cID, lConditions, new DBOnlineEpisode()));
                        lSeriesID = lSeries[DBOnlineSeries.cID];
                        break;

                    case DBSeason.cTableName:
                        var lSeason = aNodeUpdated.Tag as DBSeason;
                        lConditions = new SQLCondition(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, lSeason[DBSeason.cSeriesID], SQLConditionType.Equal);
                        lConditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeasonIndex, lSeason[DBSeason.cIndex], SQLConditionType.Equal);
                        lEpisodeIdUpdates.AddRange(DBEpisode.GetSingleField(DBOnlineEpisode.cID, lConditions, new DBOnlineEpisode()));
                        lSeriesID = lSeason[DBSeason.cSeriesID];
                        break;

                    case DBEpisode.cTableName:
                        var lEpisode = aNodeUpdated.Tag as DBEpisode;
                        lEpisodeIdUpdates.Add(lEpisode[DBOnlineEpisode.cID]);
                        lSeriesID = lEpisode[DBOnlineEpisode.cSeriesID];
                        break;
                }

                if (lEpisodeIdUpdates.Count > 0)
                {
                    // Delete API Cache so we make sure we get the latest updates
                    TmdbCache.DeleteSeriesFromCache(lSeriesID);

                    Parsing_Start((new CParsingParameters(new List<ParsingAction> { ParsingAction.UpdateSeries,
                                                                                    ParsingAction.UpdateEpisodes, 
                                                                                    ParsingAction.UpdateEpisodeThumbNails, 
                                                                                    ParsingAction.UpdateCommunityRatings
                                                                                  }, lSeriesIdUpdates, lEpisodeIdUpdates)));
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
                bool hasDuplicateEpisode = false;
                bool hasLocalFiles = false;

                TreeNode seriesNode = null;

                DBEpisode episode = null;
                DBSeason season = null;
                DBSeries series = null;

                if (nodeDeleted.Name == DBSeries.cTableName)
                {
                    series = (DBSeries)nodeDeleted.Tag;
                    seriesNode = nodeDeleted;
                    if (series != null) hasLocalFiles = series[DBOnlineSeries.cHasLocalFiles];
                }
                else if (nodeDeleted.Name == DBSeason.cTableName)
                {
                    season = (DBSeason)nodeDeleted.Tag;
                    series = (DBSeries)nodeDeleted.Parent.Tag;
                    seriesNode = nodeDeleted.Parent;
                    if (season != null) hasLocalFiles = season[DBSeason.cHasLocalFiles];
                }
                else if (nodeDeleted.Name == DBEpisode.cTableName)
                {
                    episode = (DBEpisode)nodeDeleted.Tag;
                    season = (DBSeason)nodeDeleted.Parent.Tag;
                    series = (DBSeries)nodeDeleted.Parent.Parent.Tag;
                    seriesNode = nodeDeleted.Parent.Parent;
                    if (episode != null)
                    {
                        hasSubtitles = episode.CheckHasLocalSubtitles();
                        hasDuplicateEpisode = episode.HasDuplicateEpisode;
                        hasLocalFiles = !string.IsNullOrEmpty(episode[DBEpisode.cFilename]);
                    }
                }

                DeleteDialog deleteDialog = new DeleteDialog(hasSubtitles, hasDuplicateEpisode, hasLocalFiles);
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
                            episode = (DBEpisode)nodeDeleted.Tag;
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
                                resultMsg = series.deleteSeries(deleteDialog.DeleteMode);
                            }
                            break;
                        #endregion

                        #region Delete Season
                        case DBSeason.cTableName:
                            if (result == DialogResult.OK)
                            {
                                resultMsg = season.deleteSeason(deleteDialog.DeleteMode);
                            }
                            break;
                        #endregion

                        #region Delete Episode
                        case DBEpisode.cTableName:
                            if (result == DialogResult.OK)
                            {
                                resultMsg = episode.deleteEpisode(deleteDialog.DeleteMode);
                            }
                            break;
                        #endregion
                    }

                    #region Cleanup Nodes
                    // update nodes so that local episode data is removed
                    if (deleteDialog.DeleteMode == TVSeriesPlugin.DeleteMenuItems.disk)
                    {
                        // get the current series node index
                        int index = this.treeView_Library.Nodes.IndexOf(seriesNode);

                        // delete the series node
                        this.treeView_Library.Nodes.Remove(seriesNode);

                        // create new series node
                        CreateSeriesNode(DBSeries.Get(series[DBSeries.cID], false), null, index);

                        // find previously selected node
                        // it will still exist as we only deleted from disk
                        seriesNode = treeView_Library.Nodes[index];
                        seriesNode.Expand();
                        var selectedNode = seriesNode;

                        if (season != null)
                        {
                            foreach (TreeNode seasonNode in seriesNode.Nodes)
                            {
                                if ((seasonNode.Tag as DBSeason)[DBSeason.cIndex] == season[DBSeason.cIndex])
                                {
                                    seasonNode.Expand();
                                    selectedNode = seasonNode;
                                    
                                    if (episode != null)
                                    {
                                        foreach (TreeNode episodeNode in seasonNode.Nodes)
                                        {
                                            if ((episodeNode.Tag as DBEpisode)[DBEpisode.cEpisodeIndex] == episode[DBEpisode.cEpisodeIndex])
                                            {
                                                selectedNode = episodeNode;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // select the last deleted node
                        this.treeView_Library.SelectedNode = selectedNode;
                        this.treeView_Library.Select();
                    }

                    // always delete tree node if removing from database
                    if (resultMsg.Count == 0 && deleteDialog.DeleteMode != TVSeriesPlugin.DeleteMenuItems.disk)
                    {
                        this.treeView_Library.Nodes.Remove(nodeDeleted);

                        // get new selected node
                        TreeNode selectedNode = this.treeView_Library.SelectedNode;

                        // if we deleted a double episode from the database
                        // then find the 2nd or 1st episode and delete that node as well
                        if (selectedNode != null && selectedNode.Name == DBEpisode.cTableName && episode.IsDoubleEpisode)
                        {
                            string filename = episode[DBEpisode.cFilename];

                            if (episode.IsSecondOfDoubleEpisode)
                            {
                                // check current selected node
                                if ((selectedNode.Tag as DBEpisode)[DBEpisode.cFilename] == filename)
                                {
                                    this.treeView_Library.Nodes.Remove(selectedNode);
                                }
                                // check the previous node
                                else if (selectedNode.PrevNode != null && (selectedNode.PrevNode.Tag as DBEpisode)[DBEpisode.cFilename] == filename)
                                {
                                    this.treeView_Library.Nodes.Remove(selectedNode.PrevNode);
                                }
                            }
                            else
                            {
                                // check current selected node
                                if ((selectedNode.Tag as DBEpisode)[DBEpisode.cFilename] == filename)
                                {
                                    this.treeView_Library.Nodes.Remove(selectedNode);
                                }
                                // check the next node
                                else if (selectedNode.NextNode != null && (selectedNode.NextNode.Tag as DBEpisode)[DBEpisode.cFilename] == filename)
                                {
                                    this.treeView_Library.Nodes.Remove(selectedNode.NextNode);
                                }
                            }

                            // get new selected node
                            selectedNode = this.treeView_Library.SelectedNode;
                        }

                        // see if need to delete season/series tree nodes
                        // i.e. they're no underlying seasons/episodes
                        if (selectedNode != null && selectedNode.Name == DBSeason.cTableName)
                        {
                            // we may have removed the last episode of a season
                            // leaving no episode nodes for the selected season
                            if (selectedNode.GetNodeCount(false) == 0)
                            {
                                // delete selected season node
                                this.treeView_Library.Nodes.Remove(selectedNode);
                                // get new selected node - maybe a series now
                                selectedNode = this.treeView_Library.SelectedNode;
                            }
                        }

                        if (selectedNode != null && selectedNode.Name == DBSeries.cTableName)
                        {
                            if (selectedNode.GetNodeCount(true) == 0)
                            {
                                this.treeView_Library.Nodes.Remove(selectedNode);
                            }
                        }
                    }
                    #endregion

                    #region Clear Selected Images
                    if (this.treeView_Library.Nodes.Count == 0)
                    {
                        // also clear the data pane
                        this.dataGridView1.Rows.Clear();
                        try
                        {
                            if (this.pictureBox_Series.Image != null)
                            {
                                this.pictureBox_Series.Image.Dispose();
                                this.pictureBox_Series.Image = null;
                                this.comboBox_BannerSelection.Items.Clear();
                            }
                            if (this.pictureBox_SeriesPoster != null)
                            {
                                this.pictureBox_SeriesPoster.Image.Dispose();
                                this.pictureBox_SeriesPoster.Image = null;
                                this.comboBox_PosterSelection.Items.Clear();
                            }
                        }
                        catch { }
                    }
                    #endregion
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

                SQLCondition conditions = new SQLCondition();
                List<DBEpisode> episodeList = new List<DBEpisode>();

                DBSeries series = null;
                DBSeason season = null;
                DBEpisode episode = null;

                switch (nodeWatched.Name)
                {
                    case DBSeries.cTableName:
                        series = (DBSeries)nodeWatched.Tag;

                        conditions = new SQLCondition();
                        conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                        conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cFirstAired, DateTime.Now.ToString("yyyy-MM-dd"), SQLConditionType.LessEqualThan);
                        episodeList = DBEpisode.Get(conditions, false);

                        foreach (DBEpisode ep in episodeList)
                        {
                            ep[DBOnlineEpisode.cWatched] = watched;

                            if (watched == 1 && ep[DBOnlineEpisode.cPlayCount] == 0)
                                ep[DBOnlineEpisode.cPlayCount] = 1;
                            if (watched == 1 && string.IsNullOrEmpty(ep[DBOnlineEpisode.cLastWatchedDate]))
                                ep[DBOnlineEpisode.cLastWatchedDate] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            if (watched == 1 && string.IsNullOrEmpty(ep[DBOnlineEpisode.cFirstWatchedDate]))
                                ep[DBOnlineEpisode.cFirstWatchedDate] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            ep.Commit();
                        }

                        // Updated Episode Counts
                        DBSeries.UpdateEpisodeCounts(series);

                        if (nodeWatched.Nodes.Count > 0)
                        {
                            for (int i = 0; i < nodeWatched.Nodes.Count; i++)
                            {
                                // set color of nodeWatched
                                if (watched == 1 && series[DBOnlineSeries.cHasLocalFiles])
                                    nodeWatched.ForeColor = System.Drawing.Color.DarkBlue;
                                else if (watched == 0 && series[DBOnlineSeries.cHasLocalFiles])
                                    nodeWatched.ForeColor = treeView_Library.ForeColor;

                                // Child Season fonts:
                                DBSeason s = (DBSeason)nodeWatched.Nodes[i].Tag;
                                if (watched == 1 && s[DBSeason.cHasLocalFiles])
                                    nodeWatched.Nodes[i].ForeColor = System.Drawing.Color.DarkBlue;
                                else if (watched == 0 && s[DBSeason.cHasLocalFiles])
                                    nodeWatched.Nodes[i].ForeColor = treeView_Library.ForeColor;

                                // Child Episode fonts:
                                if (nodeWatched.Nodes[i].Nodes.Count > 0)
                                {
                                    // only mark the ones we have actually set as watched
                                    int epCount = 0;
                                    if (watched == 1)
                                        epCount = episodeList.Where(e => e[DBOnlineEpisode.cSeasonIndex] == s[DBSeason.cIndex]).Count();
                                    else
                                        epCount = nodeWatched.Nodes[i].Nodes.Count;

                                    for (int j = 0; j < epCount; j++)
                                    {
                                        DBEpisode ep = (DBEpisode)nodeWatched.Nodes[i].Nodes[j].Tag;
                                        if (watched == 1 && !string.IsNullOrEmpty(ep[DBEpisode.cFilename]))
                                            nodeWatched.Nodes[i].Nodes[j].ForeColor = System.Drawing.Color.DarkBlue;
                                        else if (watched == 0 && !string.IsNullOrEmpty(ep[DBEpisode.cFilename]))
                                            nodeWatched.Nodes[i].Nodes[j].ForeColor = treeView_Library.ForeColor;
                                    }
                                }
                            }
                        }

                        cache.dump();
                        break;

                    case DBSeason.cTableName:
                        season = (DBSeason)nodeWatched.Tag;
                        series = DBSeries.Get(season[DBSeason.cSeriesID]);

                        conditions = new SQLCondition();
                        conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                        conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeasonIndex, season[DBSeason.cIndex], SQLConditionType.Equal);
                        conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cFirstAired, DateTime.Now.ToString("yyyy-MM-dd"), SQLConditionType.LessEqualThan);
                        episodeList = DBEpisode.Get(conditions, false);

                        foreach (DBEpisode ep in episodeList)
                        {
                            ep[DBOnlineEpisode.cWatched] = watched;

                            if (watched == 1 && ep[DBOnlineEpisode.cPlayCount] == 0)
                                ep[DBOnlineEpisode.cPlayCount] = 1;
                            if (watched == 1 && string.IsNullOrEmpty(ep[DBOnlineEpisode.cLastWatchedDate]))
                                ep[DBOnlineEpisode.cLastWatchedDate] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            if (watched == 1 && string.IsNullOrEmpty(ep[DBOnlineEpisode.cFirstWatchedDate]))
                                ep[DBOnlineEpisode.cFirstWatchedDate] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            ep.Commit();
                        }

                        // update episode counts
                        DBSeason.UpdateEpisodeCounts(series, season);

                        // set color of nodeWatched
                        if (watched == 1 && season[DBSeason.cHasLocalFiles])
                            nodeWatched.ForeColor = System.Drawing.Color.DarkBlue;
                        else if (watched == 0 && season[DBSeason.cHasLocalFiles])
                            nodeWatched.ForeColor = treeView_Library.ForeColor;

                        // Parent Series color:
                        if (series[DBOnlineSeries.cUnwatchedItems] == 0 && series[DBOnlineSeries.cHasLocalFiles])
                            nodeWatched.Parent.ForeColor = System.Drawing.Color.DarkBlue;
                        else if (series[DBOnlineSeries.cUnwatchedItems] == 1 && series[DBOnlineSeries.cHasLocalFiles])
                            nodeWatched.Parent.ForeColor = treeView_Library.ForeColor;
                        
                        // Child Episodes color:
                        if (episodeList.Count > 0)
                        {
                            // only mark the ones we have actually set as watched
                            int epCount = 0;
                            if (watched == 1)
                                epCount = episodeList.Count();
                            else
                                epCount = nodeWatched.Nodes.Count;

                            for (int i = 0; i < epCount; i++)
                            {
                                DBEpisode ep = (DBEpisode)nodeWatched.Nodes[i].Tag;
                                if (watched == 1 && !string.IsNullOrEmpty(ep[DBEpisode.cFilename]))
                                    nodeWatched.Nodes[i].ForeColor = System.Drawing.Color.DarkBlue;
                                else if (watched == 0 && !string.IsNullOrEmpty(ep[DBEpisode.cFilename]))
                                    nodeWatched.Nodes[i].ForeColor = treeView_Library.ForeColor;
                            }
                        }

                        cache.dump();
                        break;

                    case DBEpisode.cTableName:
                        episode = (DBEpisode)nodeWatched.Tag;

                        episode[DBOnlineEpisode.cWatched] = watched;

                        if (watched == 1 && episode[DBOnlineEpisode.cPlayCount] == 0)
                            episode[DBOnlineEpisode.cPlayCount] = 1;
                        if (watched == 1 && string.IsNullOrEmpty(episode[DBOnlineEpisode.cLastWatchedDate]))
                            episode[DBOnlineEpisode.cLastWatchedDate] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        if (watched == 1 && string.IsNullOrEmpty(episode[DBOnlineEpisode.cFirstWatchedDate]))
                            episode[DBOnlineEpisode.cFirstWatchedDate] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        episode.Commit();

                        series = DBSeries.Get(episode[DBEpisode.cSeriesID]);
                        season = Helper.getCorrespondingSeason(episode[DBEpisode.cSeriesID], episode[DBEpisode.cSeasonIndex]);
                        DBSeason.UpdateEpisodeCounts(series, season);

                        // set color of nodeWatched
                        if (watched == 1 && !string.IsNullOrEmpty(episode[DBEpisode.cFilename]))
                            nodeWatched.ForeColor = System.Drawing.Color.DarkBlue;
                        else if (watched == 0 && !string.IsNullOrEmpty(episode[DBEpisode.cFilename]))
                            nodeWatched.ForeColor = treeView_Library.ForeColor;

                        // Parent Series color
                        if (series[DBOnlineSeries.cUnwatchedItems] == 0 && series[DBOnlineSeries.cHasLocalFiles])
                            nodeWatched.Parent.ForeColor = System.Drawing.Color.DarkBlue;
                        else if (series[DBOnlineSeries.cUnwatchedItems] == 1 && series[DBOnlineSeries.cHasLocalFiles])
                            nodeWatched.Parent.ForeColor = treeView_Library.ForeColor;

                        // Parent Season color
                        if (season[DBSeason.cUnwatchedItems] == 0 && season[DBSeason.cHasLocalFiles])
                            nodeWatched.Parent.Parent.ForeColor = System.Drawing.Color.DarkBlue;
                        else if (season[DBSeason.cUnwatchedItems] == 1 && season[DBSeason.cHasLocalFiles])
                            nodeWatched.Parent.Parent.ForeColor = treeView_Library.ForeColor;

                        cache.dump();
                        break;
                }
               
                // Refresh treeView
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
                                        bValid |= mSeriesReference.FieldNames.Contains(sFieldName);
                                    break;

                                case FieldTag.Level.Season:
                                    if (sTableName == DBSeries.cOutName)
                                        bValid |= mSeriesReference.FieldNames.Contains(sFieldName);
                                    if (sTableName == DBSeason.cOutName)
                                        bValid |= mSeasonReference.FieldNames.Contains(sFieldName);
                                    break;

                                case FieldTag.Level.Episode:
                                    if (sTableName == DBSeries.cOutName)
                                        bValid |= mSeriesReference.FieldNames.Contains(sFieldName);
                                    if (sTableName == DBSeason.cOutName)
                                        bValid |= mSeasonReference.FieldNames.Contains(sFieldName);
                                    if (sTableName == DBEpisode.cOutName)
                                        bValid |= mEpisodeReference.FieldNames.Contains(sFieldName);
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
                List<String> fieldList = (List<String>) mSeriesReference.FieldNames;
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
                List<String> fieldList = (List<String>)mSeasonReference.FieldNames;
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
                List<String> fieldList = (List<String>)mEpisodeReference.FieldNames;
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

        private void textBox_PluginHomeName_TextChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cPluginName, textBox_PluginHomeName.Text);
        }

        private void LogWindowChanged()
        {
            this.splitMain_Log.SplitterDistance = this.Size.Height / 3 * 2;
            DBOption.SetOptions(DBOption.cConfigLogCollapsed, splitMain_Log.Panel2Collapsed);

            if (splitMain_Log.Panel2Collapsed)
            {
                btnShowLog.Image = Properties.Resources.arrow_up_small;
                this.toolTip_Help.SetToolTip(this.btnShowLog, "Click to show log");
            }
            else
            {
                btnShowLog.Image = Properties.Resources.arrow_down_small;
                this.toolTip_Help.SetToolTip(this.btnShowLog, "Click to hide log");
            }
        }

        private void btnShowLog_Click(object sender, EventArgs e)
        {
            splitMain_Log.Panel2Collapsed = !splitMain_Log.Panel2Collapsed;
            DBOption.SetOptions(DBOption.cConfigLogCollapsed, splitMain_Log.Panel2Collapsed);
            LogWindowChanged();
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

                case "scanignore":
                    ScanIgnore(clickedNode);
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

            }
        }
      
        private delegate ReturnCode ChooseFromSelectionDelegate(ChooseFromSelectionDescriptor descriptor);
        private CItem m_selected;

        public ReturnCode ChooseFromSelection(Feedback.ChooseFromSelectionDescriptor descriptor, out Feedback.CItem selected)
        {
            Feedback.ReturnCode returnCode;
            if (mLocalControlForInvoke.InvokeRequired)
            {
                returnCode = (Feedback.ReturnCode)mLocalControlForInvoke.Invoke(new ChooseFromSelectionDelegate(ChooseFromSelectionSync), new Object[] { descriptor });
            }
            else
                returnCode = ChooseFromSelectionSync(descriptor);
            selected = m_selected;
            return returnCode;
        }

        public ReturnCode ChooseFromSelectionSync(Feedback.ChooseFromSelectionDescriptor descriptor)
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

        public ReturnCode YesNoOkDialog(Feedback.ChooseFromYesNoDescriptor descriptor)
        {
            Feedback.ReturnCode returnCode;
            if (mLocalControlForInvoke.InvokeRequired)
            {
                returnCode = (Feedback.ReturnCode)mLocalControlForInvoke.Invoke(new YesNoOkDialogDelegate(YesNoOkDialogSync), new Object[] { descriptor });
            }
            else
            {
                returnCode = YesNoOkDialogSync(descriptor);
            }
            return returnCode;
        }

        public ReturnCode YesNoOkDialogSync(Feedback.ChooseFromYesNoDescriptor descriptor)
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

        ToolStripMenuItem subMenuItem = null;
        ContextMenuStrip subMenu = null;
        private void contextMenuStrip_DetailsTree_Opening( object sender, CancelEventArgs e )
        {
            TreeNode node = contextMenuStrip_DetailsTree.Tag as TreeNode;
            if ( node == null )
                return;

            bool bHidden = false;

            //NOTE: use names to access the menu items in case the order get altered in the future
            switch ( node.Name )
            {
                case DBSeries.cTableName:
                    DBSeries series = ( DBSeries )node.Tag;
                    bHidden = series[DBSeries.cHidden];

                    contextMenuStrip_DetailsTree.Items["ignoreOnScanToolStripMenuItem"].Enabled = true;
                    ToolStripMenuItem ignoreOnScanMenuItem = ( ToolStripMenuItem )contextMenuStrip_DetailsTree.Items["ignoreOnScanToolStripMenuItem"];
                    ignoreOnScanMenuItem.Checked = series[DBSeries.cScanIgnore];

                    // Create AddToView ContextMenu Item and Submenu
                    // No need to create a Remove Item as we can use the checked state
                    if ( subMenuItem == null )
                    {
                        subMenuItem = new ToolStripMenuItem( "Add Series to View" );
                        subMenuItem.Name = "addSeriesToView";
                        subMenu = new ContextMenuStrip( this.components );
                        subMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
                        subMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler( this.contextMenuStrip_AddToView_ItemClicked );
                        subMenuItem.DropDown = subMenu;
                        subMenu.ShowCheckMargin = true;
                        subMenu.ShowImageMargin = false;
                    }
                    subMenuItem.Enabled = true;

                    // Populate View Sub-Menu
                    DBView[] views = DBView.getTaggedViews();
                    string viewTags = series[DBOnlineSeries.cViewTags];

                    subMenu.Items.Clear();
                    foreach ( DBView view in views )
                    {
                        ToolStripMenuItem item = new ToolStripMenuItem();
                        item.Name = view[DBView.cTransToken];
                        item.Text = view[DBView.cTransToken];
                        item.Tag = view;
                        // Check View if already a member                                                
                        string viewTag = "|" + view[DBView.cTransToken] + "|";
                        if ( viewTags.Contains( viewTag ) )
                            item.Checked = true;

                        subMenu.Items.Add( item );
                    }
                    contextMenuStrip_DetailsTree.Items.Add( subMenuItem );

                    break;

                case DBSeason.cTableName:
                    DBSeason season = ( DBSeason )node.Tag;
                    bHidden = season[DBSeason.cHidden];
                    contextMenuStrip_DetailsTree.Items["ignoreOnScanToolStripMenuItem"].Enabled = false;

                    if ( contextMenuStrip_DetailsTree.Items.ContainsKey( "addSeriesToView" ) )
                        contextMenuStrip_DetailsTree.Items["addSeriesToView"].Enabled = false;
                    break;

                case DBEpisode.cTableName:
                    DBEpisode episode = ( DBEpisode )node.Tag;
                    bHidden = episode[DBOnlineEpisode.cHidden];
                    contextMenuStrip_DetailsTree.Items["ignoreOnScanToolStripMenuItem"].Enabled = false;

                    if ( contextMenuStrip_DetailsTree.Items.ContainsKey( "addSeriesToView" ) )
                        contextMenuStrip_DetailsTree.Items["addSeriesToView"].Enabled = false;

                    break;
            }

            if ( bHidden )
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
                // TODO: Add to TMDb Favourites
            }
                      
		}

        private void treeView_Library_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenuStrip_DetailsTree.Tag = e.Node;
        }       

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
                MPTVSeriesLog.Write("Updating MediaInfo...(please be patient!)");
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
            DBOption.SetOptions(DBOption.cImportFolderWatch, checkBox_doFolderWatch.Checked);
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
            DBOption.SetOptions(DBOption.cImportScanRemoteShare, checkBox_scanRemoteShares.Checked);
            if (checkBox_scanRemoteShares.Checked)
                nudScanRemoteShareFrequency.Enabled = true;
            else
                nudScanRemoteShareFrequency.Enabled = false;
        }

        private void _availViews_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!pauseViewConfigSave)
            {
                logicalView.s_cachePrettyName = false;
                pauseViewConfigSave = true;
                view_selectedName.Text = string.Empty;
                view_selStepsList.Items.Clear();

                mSelectedView = Helper.GetElementFromList<logicalView, string>((string)_availViews.SelectedItem, "Name", 0, mAvailableViews);
                view_selectedName.Text = mSelectedView.prettyName;
                checkCurViewEnabled.Checked = mSelectedView.m_Enabled;
                checkBoxParentalControl.Checked = mSelectedView.ParentalControl;             

                foreach (string step in Helper.getPropertyListFromList<logicalViewStep, String>("Name", mSelectedView.m_steps))
                    view_selStepsList.Items.Add(step);
                
                pauseViewConfigSave = false;

                // Enable 'Edit' button for Simple Views
                // TODO: allow editing of advanced views when conditional GUI is complete
                if (mSelectedView.IsTaggedView) {
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
        }

        private void comboOnlineLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ( comboOnlineLang.SelectedIndex < 0 )
                return;

            var lSelectedLanguage = comboOnlineLang.SelectedItem as Language;
            if ( lSelectedLanguage == null ) return;

            if ( lSelectedLanguage.Abbreviation != DBOption.GetOptions(DBOption.cOnlineLanguage))
            {
                DBOption.SetOptions(DBOption.cOnlineLanguage, lSelectedLanguage.Abbreviation );
                DBOption.SetOptions(DBOption.cUpdateTimeStamp, 0);
                Online_Parsing_Classes.OnlineAPI.SelectedLanguage = string.Empty; // to overcome caching
                MPTVSeriesLog.Write("You need to do a manual import everytime the language is changed or your old items will not be updated. New language: " + comboOnlineLang.SelectedItem.ToString());
            }
        }

        private void checkBox_OverrideComboLang_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cOverrideLanguage, checkBox_OverrideComboLang.Checked);
            if (checkBox_OverrideComboLang.Checked)
            {
                // Disable and clear
                comboOnlineLang.Items.Clear();
                comboOnlineLang.Enabled = false;
                DBOption.SetOptions(DBOption.cOnlineLanguage, "en"); //Set the default value.
                Online_Parsing_Classes.OnlineAPI.SelectedLanguage = string.Empty;

                MPTVSeriesLog.Write("Now you can change the language on each Series in Details Tab");
                // Reload the tree for showing the language property
                LoadTree();
            }
            else
            {
                LoadOnlineLanguages();
                // Reload the tree for hiding the language property
                LoadTree();
            }
        }

        private void linkDelUpdateTime_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
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
                mSelectedView = Helper.GetElementFromList<logicalView, string>((string)_availViews.SelectedItem, "Name", 0, mAvailableViews);
                if (mSelectedView != null)
                {
                    mSelectedView.prettyName = view_selectedName.Text;
                    mSelectedView.m_Enabled = checkCurViewEnabled.Checked;
                    mSelectedView.ParentalControl = checkBoxParentalControl.Checked;
                    mSelectedView.saveToDB();
                    LoadViews();
                    for (int i = 0; i < mAvailableViews.Count; i++) {
                        if (mAvailableViews[i].m_uniqueID == mSelectedView.m_uniqueID) {
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

        private void lblClearDB_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MessageBox.Show("You are about to delete all Series, Seasons and Episodes from your database!" + Environment.NewLine + "Continue?", Translation.Confirm, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // we delete everything
                DBTVSeries.Execute("DELETE FROM online_episodes");
                DBTVSeries.Execute("DELETE FROM local_episodes");
                DBTVSeries.Execute("DELETE FROM season");
                DBTVSeries.Execute("DELETE FROM local_series");
                DBTVSeries.Execute("DELETE FROM online_series");

                // delete last update options
                DBTVSeries.Execute("DELETE FROM options WHERE property = 'UpdateScanLastTime'");
                DBTVSeries.Execute("DELETE FROM options WHERE property = 'UpdateTimeStamp'");
                DBTVSeries.Execute("DELETE FROM options WHERE property = 'TraktLastDateUpdated'");

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
            DBOption.SetOptions(DBOption.cUseSortName, checkBox_Series_UseSortName.Checked);
            LoadTree();
        }
       
        private void lnkResetView_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DBView.ClearAll();
            DBView.fillDefaults();
            LoadViews();
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

        private void comboBox_PosterSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ( treeView_Library.SelectedNode.Tag is DBSeries )
            {
                DBSeries series = ( DBSeries )treeView_Library.SelectedNode.Tag;

                series.Poster = ( ( BannerComboItem )comboBox_PosterSelection.SelectedItem ).sFullPath;
                try
                {
                    this.pictureBox_SeriesPoster.Image = ImageAllocator.LoadImageFastFromFile( ImageAllocator.ExtractFullName( ( ( BannerComboItem )comboBox_PosterSelection.SelectedItem ).sFullPath ) ); // Image.FromFile(ImageAllocator.ExtractFullName(((BannerComboItem)comboBox_BannerSelection.SelectedItem).sFullPath));
                }
                catch ( Exception ) { }

                series.Commit();
            }
            else //DBSeason
            {
                DBSeason season = ( DBSeason )treeView_Library.SelectedNode.Tag;

                season.Banner = ( ( BannerComboItem )comboBox_PosterSelection.SelectedItem ).sFullPath;
                try
                {
                    this.pictureBox_SeriesPoster.Image = ImageAllocator.LoadImageFastFromFile( ImageAllocator.ExtractFullName( ( ( BannerComboItem )comboBox_PosterSelection.SelectedItem ).sFullPath ) ); // Image.FromFile(ImageAllocator.ExtractFullName(((BannerComboItem)comboBox_BannerSelection.SelectedItem).sFullPath));
                }
                catch ( Exception ) { }

                season.Commit();
            }
        }
    
        private void checkBox_Episode_OnlyShowLocalFiles_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cOnlyShowLocalFiles, !checkBox_Episode_OnlyShowLocalFiles.Checked);
        }

        private void optionAsk2Rate_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cAskToRate, optionAsk2Rate.Checked);
        }

        private void linkAccountID_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start( @"https://www.thetvdb.com/dashboard/account/editinfo" );
        }

        private void linkExpressionHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start( @"https://forum.team-mediaportal.com/threads/expressions-rules-requests.21978/" );
        }
        
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
            DBOption.SetOptions(DBOption.cImportScanOnStartup, checkBox_ScanOnStartup.Checked);
            lblImportDelaySecs.Enabled = checkBox_ScanOnStartup.Checked;
            lblImportDelayCaption.Enabled = checkBox_ScanOnStartup.Checked;
            numericUpDownImportDelay.Enabled = checkBox_ScanOnStartup.Checked;
        }

        private void checkBox_AutoDownloadMissingArtwork_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cAutoDownloadMissingArtwork, checkBox_AutoDownloadMissingArtwork.Checked);
        }

        private void checkBox_AutoUpdateAllFanart_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cAutoUpdateAllFanart, checkBox_AutoUpdateAllFanart.Checked);
        }

        private void checkBox_Episode_HideUnwatchedThumbnail_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cHideUnwatchedThumbnail, checkBox_Episode_HideUnwatchedThumbnail.Checked);
        }

        private void btnRemoveView_Click(object sender, EventArgs e) {
            if (mAvailableViews.Count == 0)
                return;
            
            // Get Selected View from list
            mSelectedView = Helper.GetElementFromList<logicalView, string>((string)_availViews.SelectedItem, "Name", 0, mAvailableViews);
            
            // Confirm Delete
            string message = string.Format("Are you sure you want to delete view \"{0}\"?",mSelectedView.prettyName);
            DialogResult result = MessageBox.Show(message, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			if (result == DialogResult.No)
				return;

            // if view is a tagged view, remove series attached to view
            if (mSelectedView.IsTaggedView)
            {
                // Get list of series in view
                SQLCondition conditions = new SQLCondition();
                conditions.Add(new DBOnlineSeries(), DBOnlineSeries.cViewTags, mSelectedView.Name, SQLConditionType.Like);
                List<DBSeries> series = DBSeries.Get(conditions);

                foreach (DBSeries s in series)
                {
                    s[DBOnlineSeries.cViewTags] = Helper.GetSeriesViewTags(s, false, mSelectedView.Name);
                    s.Commit();

                    // Remove from online database
                    if (mSelectedView.Name == DBView.cTranslateTokenOnlineFavourite)
                    {
                        // TODO: TMDb Favourites
                    }
                }
            }

            // Get All current Views
            DBView[] views = DBView.getAll(true);

            // Remove all Rows from Database
            DBView.ClearAll();

            int index = 0;

            // Add Rows back excluding deleted one
            foreach (DBView view in views) {
                if (view[DBView.cIndex] != int.Parse(mSelectedView.m_uniqueID)) {
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
                            if (name == DBView.cTranslateTokenOnlineFavourite)
                            {
                                // TODO: TMDb Favourites
                            }
                        }

                        // Remove series from view
                        foreach (DBSeries series in viewConfigDialog.SeriesToRemove) {
                            MPTVSeriesLog.Write(string.Format("Removing series \"{0}\" from \"{1}\"", series.ToString(), name));                            
                            series[DBOnlineSeries.cViewTags] = Helper.GetSeriesViewTags(series,false,name);
                            series.Commit();

                            // Remove from online database
                            if (name == DBView.cTranslateTokenOnlineFavourite)
                            {
                                // TODO: TMDb Favourites
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
                            if (name == DBView.cTranslateTokenOnlineFavourite)
                            {
                                // TODO: TMDb Favourites
                            }
                        }

                        // Remove series from view
                        foreach (DBSeries series in viewConfigDialog.SeriesToRemove) {
                            MPTVSeriesLog.Write(string.Format("Removing series \"{0}\" from \"{1}\"", series.ToString(), name));
                            series[DBOnlineSeries.cViewTags] = Helper.GetSeriesViewTags(series, false, name);
                            series.Commit();

                            // Remove from online database
                            if (name == DBView.cTranslateTokenOnlineFavourite)
                            {
                                // TODO: TMDb Favourites
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

        private void dtpParentalAfter_ValueChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(dtpParentalAfter.Text))
                return;

            DBOption.SetOptions(DBOption.cParentalControlDisableAfter, dtpParentalAfter.Text);
        }

        private void dtpParentalBefore_ValueChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(dtpParentalBefore.Text))
                return;

            DBOption.SetOptions(DBOption.cParentalControlDisableBefore, dtpParentalBefore.Text);
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
                    val += (int)replacement[DBReplacements.cIsRegex];
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

                // Dialog box to make sure they want to clear out current replacements to import new ones.
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

                // read file and import into database
                while ((line = r.ReadLine()) != null) {
                    char[] c = { ';' };
                    parts = line.Split(c, 6);
                    
                    // support older relacements file 
                    if (parts.Length < 5 || parts.Length > 6) continue;

                    replacement = new DBReplacements();                   
                    replacement[DBReplacements.cIndex] = index;
                    
                    if (Convert.ToInt32(parts[0]) == 0 || Convert.ToInt32(parts[0]) == 1) replacement[DBReplacements.cEnabled] = parts[0]; else continue;
                    if (Convert.ToInt32(parts[1]) == 0 || Convert.ToInt32(parts[1]) == 1) replacement[DBReplacements.cBefore] = parts[1]; else continue;
                    if (Convert.ToInt32(parts[2]) == 0 || Convert.ToInt32(parts[2]) == 1) replacement[DBReplacements.cTagEnabled] = parts[2]; else continue;
                    
                    // handle upgrade, first version does not contain IsRegEx part
                    if (parts.Length == 6) {
                        if (Convert.ToInt32(parts[3]) == 0 || Convert.ToInt32(parts[3]) == 1) replacement[DBReplacements.cIsRegex] = parts[3]; else continue;
                        replacement[DBReplacements.cToReplace] = parts[4];
                        replacement[DBReplacements.cWith] = parts[5];
                    }
                    else {
                        replacement[DBReplacements.cIsRegex] = "0";
                        replacement[DBReplacements.cToReplace] = parts[3];
                        replacement[DBReplacements.cWith] = parts[4];
                    }
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
            DBOption.SetOptions(DBOption.cImportScanRemoteShareLapse, (int)nudScanRemoteShareFrequency.Value);
        }        

        // Set focus on selected item when using Mouse Right Click
        private void treeView_Library_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                treeView_Library.SelectedNode = treeView_Library.GetNodeAt(e.X, e.Y);
            } 
        }

        private void cbOnPlaySeriesOrSeasonAction_SelectedIndexChanged(object sender, EventArgs e) {
            // index must match enum OnPlaySeriesOrSeasonAction
            DBOption.SetOptions(DBOption.cOnPlaySeriesOrSeasonAction, cbOnPlaySeriesOrSeasonAction.SelectedIndex);
        }

        private void cbNewEpisodeThumbIndicator_SelectedIndexChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cNewEpisodeThumbType, cbNewEpisodeThumbIndicator.SelectedIndex);

            if (cbNewEpisodeThumbIndicator.SelectedIndex == (int)NewEpisodeIndicatorType.recentlyadded ||
                cbNewEpisodeThumbIndicator.SelectedIndex == (int)NewEpisodeIndicatorType.recentlyaddedunwatched)
            {
                nudRecentlyAddedDays.Enabled = true;
                lblRecentAddedDays.Enabled = true;
            }
            else
            {
                nudRecentlyAddedDays.Enabled = false;
                lblRecentAddedDays.Enabled = false;
            }
        }
     
        private void numericUpDownImportDelay_ValueChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cImportDelay, (int)numericUpDownImportDelay.Value);
        }

        private void nudRecentlyAddedDays_ValueChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cNewEpisodeRecentDays, (int)nudRecentlyAddedDays.Value);
        }

        private void buttonArtworkDownloadLimits_Click(object sender, EventArgs e)
        {
            ArtworkDownloadLimits artworkLimitDlg = new ArtworkDownloadLimits();
            artworkLimitDlg.ShowDialog(this);
        }

        private void nudParentalControlTimeout_ValueChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cParentalControlResetInterval, (int)nudParentalControlTimeout.Value);
        }

        private void ConfigurationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // save the config size
            DBOption.SetOptions(DBOption.cConfigSizeHeight, this.Size.Height);
            DBOption.SetOptions(DBOption.cConfigSizeWidth, this.Size.Width);
        }

        private void dbOptionSQLLogging_CheckStateChanged(object sender, EventArgs e)
        {
            // update logging level so reflects in current config window
            MPTVSeriesLog.InitLogLevel();
        }

        private void nudConsecFailures_ValueChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cMaxConsecutiveDownloadErrors, (int)nudConsecFailures.Value);
        }

        private void dbOptCheckBoxCleanOnlineEpisodes_CheckedChanged(object sender, EventArgs e)
        {
            if (dbOptCheckBoxCleanOnlineEpisodes.Checked)
            {
                dbOptCheckBoxRemoveEpZero.Enabled = true;
            }
            else
            {
                dbOptCheckBoxRemoveEpZero.Enabled = false;
            }
        }

        private void lnkOpenAPICacheDir_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            string lCacheFolder = Settings.GetPath( Settings.Path.config ) + "\\Cache\\TMDB\\";
            lCacheFolder += mSelectedSeries[DBOnlineSeries.cID];

            // Open Directory
            Process.Start( "explorer.exe", lCacheFolder );
        }

        private void lnkImageCache_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            string lArtworkFolder = Settings.GetPath( Settings.Path.banners ) + "\\";
            lArtworkFolder += Helper.CleanLocalPath( mSelectedSeries.ToString() );

            // Open Directory
            Process.Start( "explorer.exe", lArtworkFolder );
        }

        private void lnkTVDbSeries_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            // base series url
            string lUrl = "http://thetvdb.com/series/" + mSelectedSeries.Slug;

            switch ( mSelectedStep )
            {
                case SelectedViewStep.Season:
                    Process.Start( lUrl + "/seasons/official/" + mSelectedSeason[DBSeason.cIndex] );
                    break;

                case SelectedViewStep.Episode:
                    Process.Start( lUrl + "/episodes/" + mSelectedEpisode[DBOnlineEpisode.cID] );
                    break;

                default: // series
                    Process.Start( lUrl );
                    break;
            }
        }

        private void lnkIMDbSeries_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            // series url
            string lSeriedId = mSelectedSeries[DBOnlineSeries.cIMDBID];
            string lUrl = "http://imdb.com/title/" + lSeriedId;

            switch ( mSelectedStep )
            {
                case SelectedViewStep.Season:
                    if ( !string.IsNullOrEmpty( lSeriedId ) )
                        Process.Start( lUrl + "/episodes?season=" + mSelectedSeason[DBSeason.cIndex] );
                    else
                        Process.Start( "http://imdb.com" );
                    break;

                case SelectedViewStep.Episode:
                    string lImdbId = mSelectedEpisode[DBOnlineEpisode.cIMDBID];

                    // if the episode IMDb is empty, take user to series level
                    if ( string.IsNullOrEmpty( lImdbId ) )
                        lImdbId = lSeriedId;

                    Process.Start( "http://imdb.com/title/" + lImdbId );
                    break;

                default: // series
                    if ( !string.IsNullOrEmpty( lSeriedId ) )
                        Process.Start( lUrl );
                    else
                        Process.Start( "http://imdb.com" );
                    break;
            }
        }

        private void lnkTraktSeries_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            // base series url
            string lUrl = "http://trakt.tv/shows/" + mSelectedSeries.Slug;

            switch ( mSelectedStep )
            {
                case SelectedViewStep.Season:
                    Process.Start( lUrl + "/seasons/" + mSelectedSeason[DBSeason.cIndex] );
                    break;

                case SelectedViewStep.Episode:
                    string lSeasonIdx = mSelectedEpisode[DBOnlineEpisode.cSeasonIndex];
                    string lEpisodeIdx = mSelectedEpisode[DBOnlineEpisode.cEpisodeIndex];

                    Process.Start( lUrl + $"/seasons/{lSeasonIdx}/episodes/{lEpisodeIdx}" );
                    break;

                default: // series
                    Process.Start( lUrl );
                    break;
            }
        }
        
        private void txtFanartTVClientKey_TextChanged( object sender, EventArgs e )
        {
            DBOption.SetOptions( DBOption.cFanartTvClientKey, txtFanartTVClientKey.Text );
        }

        private void lnkFanartTvClientKey_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            Process.Start( "https://fanart.tv/2015/01/personal-api-keys/" );
        }

        private void lnkFanartTvSeries_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start($"https://fanart.tv/series/{mSelectedSeries[DBOnlineSeries.cTvdbId]}/");
        }

        private void lnkTMDbSeries_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // don't lock up the GUI in case we need to search online for TMDb ID first
            var lTmdbTask = Task.Factory.StartNew(() =>
            {
                // first check if we have the TMDb ID for the series
                int lTmdbId = mSelectedSeries[DBOnlineSeries.cTmdbId];
                if (lTmdbId <= 0)
                {
                    MPTVSeriesLog.Write($"Searching themoviedb.org for series '{mSelectedSeries[DBOnlineSeries.cPrettyName]}' with TVDb ID '{mSelectedSeries[DBOnlineSeries.cID]}'");

                    // we don't have it, let's search for it and save it for next time
                    // there should only be one result for a tvdb ID.
                    var lResults = TmdbAPI.TmdbAPI.TmdbFind(mSelectedSeries[DBOnlineSeries.cID], ExternalSource.tvdb_id);
                    if (lResults == null || lResults.Shows == null || lResults.Shows.Count == 0)
                    {
                        MPTVSeriesLog.Write($"Failed to find TMDb ID for series '{mSelectedSeries[DBOnlineSeries.cPrettyName]}'");
                        return;
                    }

                    lTmdbId = lResults.Shows.FirstOrDefault().Id;
                    MPTVSeriesLog.Write($"Found TMDb ID '{lTmdbId}' for tv show '{mSelectedSeries[DBOnlineSeries.cPrettyName]}' with TVDb ID '{mSelectedSeries[DBOnlineSeries.cID]}'");

                    // save it for next time
                    mSelectedSeries[DBOnlineSeries.cTmdbId] = lTmdbId;
                    mSelectedSeries.Commit();
                }

                switch (mSelectedStep)
                {
                    case SelectedViewStep.Season:
                        Process.Start($"https://www.themoviedb.org/tv/{lTmdbId}/season/{mSelectedSeason[DBSeason.cIndex]}");
                        break;

                    case SelectedViewStep.Episode:
                        Process.Start($"https://www.themoviedb.org/tv/{lTmdbId}/season/{mSelectedEpisode[DBOnlineEpisode.cSeasonIndex]}/episode/{mSelectedEpisode[DBOnlineEpisode.cEpisodeIndex]}");
                        break;

                    default: // series
                        Process.Start($"https://www.themoviedb.org/tv/{lTmdbId}/");
                        break;
                }
            });
        }

        /// <summary>
        ///  Get the Culture DisplayName from Two Letter ISO
        /// </summary>
        public string GetAudioLanguageDisplayName(String TwoLetterISOLanguage)
        {
            String CultureDisplayName = TwoLetterISOLanguage;
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

            foreach (var culture in cultures)
            {
                if (culture.TwoLetterISOLanguageName == TwoLetterISOLanguage)
                {
                    CultureDisplayName = culture.DisplayName;
                    break;
                }
            }

            return CultureDisplayName;
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

    public enum SelectedViewStep
    {
        Series,
        Season,
        Episode
    }
}
