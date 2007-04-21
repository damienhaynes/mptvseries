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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.columnHeader_Series = new System.Windows.Forms.ColumnHeader();
            this.columnHeader_Title = new System.Windows.Forms.ColumnHeader();
            this.columnHeader_Season = new System.Windows.Forms.ColumnHeader();
            this.columnHeader_Episode = new System.Windows.Forms.ColumnHeader();
            this.columnHeader_OriginallyAired = new System.Windows.Forms.ColumnHeader();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.tabControl_Details = new System.Windows.Forms.TabControl();
            this.tabPage_Details = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.checkBox_ShowHidden = new System.Windows.Forms.CheckBox();
            this.treeView_Library = new System.Windows.Forms.TreeView();
            this.contextMenuStrip_DetailsTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getSubtitlesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.torrentThToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comboBox_BannerSelection = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.detailsPropertyBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.pictureBox_Series = new System.Windows.Forms.PictureBox();
            this.tabPage_Import = new System.Windows.Forms.TabPage();
            this.splitContainer_SettingsOutput = new System.Windows.Forms.SplitContainer();
            this.splitContainerImportSettings = new System.Windows.Forms.SplitContainer();
            this.treeView_Settings = new System.Windows.Forms.TreeView();
            this.panel_StringReplacements = new System.Windows.Forms.Panel();
            this.dataGridView_Replace = new System.Windows.Forms.DataGridView();
            this.panel_OnlineData = new System.Windows.Forms.Panel();
            this.cleanBanners = new System.Windows.Forms.LinkLabel();
            this.checkBox_doFolderWatch = new System.Windows.Forms.CheckBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.checkBox_DontClearMissingLocalFiles = new System.Windows.Forms.CheckBox();
            this.checkBox_OnlineSearch = new System.Windows.Forms.CheckBox();
            this.checkBox_LocalDataOverride = new System.Windows.Forms.CheckBox();
            this.checkBox_AutoChooseSeries = new System.Windows.Forms.CheckBox();
            this.checkBox_FullSeriesRetrieval = new System.Windows.Forms.CheckBox();
            this.panel_ParsingTest = new System.Windows.Forms.Panel();
            this.button_TestReparse = new System.Windows.Forms.Button();
            this.listView_ParsingResults = new System.Windows.Forms.ListView();
            this.panel_ImportPathes = new System.Windows.Forms.Panel();
            this.dataGridView_ImportPathes = new System.Windows.Forms.DataGridView();
            this.panel_Expressions = new System.Windows.Forms.Panel();
            this.dataGridView_Expressions = new System.Windows.Forms.DataGridView();
            this.button_MoveExpUp = new System.Windows.Forms.Button();
            this.button_MoveExpDown = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.progressBar_Parsing = new System.Windows.Forms.ProgressBar();
            this.button_Start = new System.Windows.Forms.Button();
            this.tab_view = new System.Windows.Forms.TabPage();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.comboBox7 = new System.Windows.Forms.ComboBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.comboBox8 = new System.Windows.Forms.ComboBox();
            this.richTextBox4 = new System.Windows.Forms.RichTextBox();
            this.comboBox9 = new System.Windows.Forms.ComboBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.comboBox10 = new System.Windows.Forms.ComboBox();
            this.richTextBox5 = new System.Windows.Forms.RichTextBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.comboBox5 = new System.Windows.Forms.ComboBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.comboBox6 = new System.Windows.Forms.ComboBox();
            this.richTextBox3 = new System.Windows.Forms.RichTextBox();
            this._23_link = new System.Windows.Forms.ComboBox();
            this.cond3_cond = new System.Windows.Forms.TextBox();
            this.cond3_type = new System.Windows.Forms.ComboBox();
            this.cond3_what = new System.Windows.Forms.RichTextBox();
            this._12_link = new System.Windows.Forms.ComboBox();
            this.cond2_cond = new System.Windows.Forms.TextBox();
            this.cond2_type = new System.Windows.Forms.ComboBox();
            this.cond2_what = new System.Windows.Forms.RichTextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.cond1_cond = new System.Windows.Forms.TextBox();
            this.cond1_type = new System.Windows.Forms.ComboBox();
            this.cond1_what = new System.Windows.Forms.RichTextBox();
            this.viewStepGroupLbl = new System.Windows.Forms.Label();
            this.viewStepGroupByTextBox = new System.Windows.Forms.TextBox();
            this.viewStepType = new System.Windows.Forms.ComboBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.button7 = new System.Windows.Forms.Button();
            this.label25 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.view_selStepsList = new System.Windows.Forms.ListBox();
            this.view_selectedName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.btnViewUp = new System.Windows.Forms.Button();
            this.btnViewDown = new System.Windows.Forms.Button();
            this.btnRemoveView = new System.Windows.Forms.Button();
            this.btnAddView = new System.Windows.Forms.Button();
            this._availViews = new System.Windows.Forms.ListBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.button3 = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.tabPage_MP_DisplayControl = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.btnLogoEdit = new System.Windows.Forms.Button();
            this.btnLogoDown = new System.Windows.Forms.Button();
            this.btnlogoUp = new System.Windows.Forms.Button();
            this.btnrmvLogo = new System.Windows.Forms.Button();
            this.addLogo = new System.Windows.Forms.Button();
            this.lstLogos = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.comboBox_seasonFormat = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.richTextBox_seasonFormat_Col3 = new System.Windows.Forms.RichTextBox();
            this.contextMenuStrip_InsertFields = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.label12 = new System.Windows.Forms.Label();
            this.richTextBox_seasonFormat_Main = new System.Windows.Forms.RichTextBox();
            this.richTextBox_seasonFormat_Col1 = new System.Windows.Forms.RichTextBox();
            this.richTextBox_seasonFormat_Subtitle = new System.Windows.Forms.RichTextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.richTextBox_seasonFormat_Col2 = new System.Windows.Forms.RichTextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.richTextBox_seasonFormat_Title = new System.Windows.Forms.RichTextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.richTextBox_episodeFormat_Col3 = new System.Windows.Forms.RichTextBox();
            this.checkBox_Episode_HideUnwatchedSummary = new System.Windows.Forms.CheckBox();
            this.richTextBox_episodeFormat_Col2 = new System.Windows.Forms.RichTextBox();
            this.checkBox_Episode_OnlyShowLocalFiles = new System.Windows.Forms.CheckBox();
            this.richTextBox_episodeFormat_Main = new System.Windows.Forms.RichTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.richTextBox_episodeFormat_Subtitle = new System.Windows.Forms.RichTextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.richTextBox_episodeFormat_Title = new System.Windows.Forms.RichTextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.richTextBox_episodeFormat_Col1 = new System.Windows.Forms.RichTextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBox_seriesFormat = new System.Windows.Forms.ComboBox();
            this.richTextBox_seriesFormat_Col3 = new System.Windows.Forms.RichTextBox();
            this.richTextBox_seriesFormat_Main = new System.Windows.Forms.RichTextBox();
            this.richTextBox_seriesFormat_Subtitle = new System.Windows.Forms.RichTextBox();
            this.richTextBox_seriesFormat_Col2 = new System.Windows.Forms.RichTextBox();
            this.richTextBox_seriesFormat_Title = new System.Windows.Forms.RichTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.richTextBox_seriesFormat_Col1 = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label28 = new System.Windows.Forms.Label();
            this.comboLanguage = new System.Windows.Forms.ComboBox();
            this.checkBox_RandBanner = new System.Windows.Forms.CheckBox();
            this.textBox_PluginHomeName = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.checkBox_AutoHeight = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown_AutoOnlineDataRefresh = new System.Windows.Forms.NumericUpDown();
            this.checkBox_AutoOnlineDataRefresh = new System.Windows.Forms.CheckBox();
            this.tabpage_Subtitles = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.textBox_TorrentDetailsRegex = new System.Windows.Forms.TextBox();
            this.textBox_TorrentSearchRegex = new System.Windows.Forms.TextBox();
            this.textBox_TorrentDetailsUrl = new System.Windows.Forms.TextBox();
            this.textBox_TorrentSearchUrl = new System.Windows.Forms.TextBox();
            this.comboBox_TorrentPreset = new System.Windows.Forms.ComboBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.button_uTorrentBrowse = new System.Windows.Forms.Button();
            this.textBox_uTorrentPath = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage_Forom = new System.Windows.Forms.TabPage();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.textBox_foromBaseURL = new System.Windows.Forms.TextBox();
            this.textBox_foromID = new System.Windows.Forms.TextBox();
            this.listBox_Log = new System.Windows.Forms.ListBox();
            this.toolTip_Help = new System.Windows.Forms.ToolTip(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl_Details.SuspendLayout();
            this.tabPage_Details.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.contextMenuStrip_DetailsTree.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.detailsPropertyBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Series)).BeginInit();
            this.tabPage_Import.SuspendLayout();
            this.splitContainer_SettingsOutput.Panel1.SuspendLayout();
            this.splitContainer_SettingsOutput.Panel2.SuspendLayout();
            this.splitContainer_SettingsOutput.SuspendLayout();
            this.splitContainerImportSettings.Panel1.SuspendLayout();
            this.splitContainerImportSettings.Panel2.SuspendLayout();
            this.splitContainerImportSettings.SuspendLayout();
            this.panel_StringReplacements.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Replace)).BeginInit();
            this.panel_OnlineData.SuspendLayout();
            this.panel_ParsingTest.SuspendLayout();
            this.panel_ImportPathes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ImportPathes)).BeginInit();
            this.panel_Expressions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Expressions)).BeginInit();
            this.tab_view.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.tabPage_MP_DisplayControl.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_AutoOnlineDataRefresh)).BeginInit();
            this.tabpage_Subtitles.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage_Forom.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
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
            // tabControl_Details
            // 
            this.tabControl_Details.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl_Details.Controls.Add(this.tabPage_Details);
            this.tabControl_Details.Controls.Add(this.tabPage_Import);
            this.tabControl_Details.Controls.Add(this.tab_view);
            this.tabControl_Details.Controls.Add(this.tabPage_MP_DisplayControl);
            this.tabControl_Details.Controls.Add(this.tabpage_Subtitles);
            this.tabControl_Details.Location = new System.Drawing.Point(0, 0);
            this.tabControl_Details.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl_Details.Name = "tabControl_Details";
            this.tabControl_Details.SelectedIndex = 0;
            this.tabControl_Details.Size = new System.Drawing.Size(692, 769);
            this.tabControl_Details.TabIndex = 64;
            // 
            // tabPage_Details
            // 
            this.tabPage_Details.Controls.Add(this.splitContainer2);
            this.tabPage_Details.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Details.Name = "tabPage_Details";
            this.tabPage_Details.Size = new System.Drawing.Size(684, 743);
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
            this.splitContainer2.Panel1.Controls.Add(this.checkBox_ShowHidden);
            this.splitContainer2.Panel1.Controls.Add(this.treeView_Library);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.comboBox_BannerSelection);
            this.splitContainer2.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer2.Panel2.Controls.Add(this.pictureBox_Series);
            this.splitContainer2.Size = new System.Drawing.Size(684, 743);
            this.splitContainer2.SplitterDistance = 196;
            this.splitContainer2.TabIndex = 0;
            // 
            // checkBox_ShowHidden
            // 
            this.checkBox_ShowHidden.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_ShowHidden.AutoSize = true;
            this.checkBox_ShowHidden.Location = new System.Drawing.Point(9, 721);
            this.checkBox_ShowHidden.Name = "checkBox_ShowHidden";
            this.checkBox_ShowHidden.Size = new System.Drawing.Size(117, 17);
            this.checkBox_ShowHidden.TabIndex = 48;
            this.checkBox_ShowHidden.Text = "Show Hidden items";
            this.checkBox_ShowHidden.UseVisualStyleBackColor = true;
            this.checkBox_ShowHidden.CheckedChanged += new System.EventHandler(this.checkBox_ShowHidden_CheckedChanged);
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
            this.treeView_Library.Size = new System.Drawing.Size(196, 717);
            this.treeView_Library.TabIndex = 47;
            this.treeView_Library.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_Library_AfterSelect);
            this.treeView_Library.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_Library_NodeMouseClick);
            this.treeView_Library.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView_Library_KeyDown);
            // 
            // contextMenuStrip_DetailsTree
            // 
            this.contextMenuStrip_DetailsTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.getSubtitlesToolStripMenuItem,
            this.torrentThToolStripMenuItem});
            this.contextMenuStrip_DetailsTree.Name = "contextMenuStrip_DetailsTree";
            this.contextMenuStrip_DetailsTree.Size = new System.Drawing.Size(141, 92);
            this.contextMenuStrip_DetailsTree.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_DetailsTree_Opening);
            this.contextMenuStrip_DetailsTree.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip_DetailsTree_ItemClicked);
            // 
            // hideToolStripMenuItem
            // 
            this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
            this.hideToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.hideToolStripMenuItem.Tag = "hide";
            this.hideToolStripMenuItem.Text = "Hide";
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.deleteToolStripMenuItem.Tag = "delete";
            this.deleteToolStripMenuItem.Text = "Delete";
            // 
            // getSubtitlesToolStripMenuItem
            // 
            this.getSubtitlesToolStripMenuItem.Name = "getSubtitlesToolStripMenuItem";
            this.getSubtitlesToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.getSubtitlesToolStripMenuItem.Tag = "subtitle";
            this.getSubtitlesToolStripMenuItem.Text = "Get Subtitles";
            // 
            // torrentThToolStripMenuItem
            // 
            this.torrentThToolStripMenuItem.Name = "torrentThToolStripMenuItem";
            this.torrentThToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.torrentThToolStripMenuItem.Tag = "torrent";
            this.torrentThToolStripMenuItem.Text = "Torrent this";
            // 
            // comboBox_BannerSelection
            // 
            this.comboBox_BannerSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_BannerSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_BannerSelection.FormattingEnabled = true;
            this.comboBox_BannerSelection.Location = new System.Drawing.Point(0, 0);
            this.comboBox_BannerSelection.Name = "comboBox_BannerSelection";
            this.comboBox_BannerSelection.Size = new System.Drawing.Size(484, 21);
            this.comboBox_BannerSelection.TabIndex = 149;
            this.comboBox_BannerSelection.SelectedIndexChanged += new System.EventHandler(this.comboBox_BannerSelection_SelectedIndexChanged);
            this.comboBox_BannerSelection.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboBox_BannerSelection_KeyPress);
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
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.dataGridView1.DataSource = this.detailsPropertyBindingSource;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.Location = new System.Drawing.Point(0, 148);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.dataGridView1.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView1.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.Size = new System.Drawing.Size(484, 595);
            this.dataGridView1.StandardTab = true;
            this.dataGridView1.TabIndex = 148;
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
            this.pictureBox_Series.Location = new System.Drawing.Point(0, 21);
            this.pictureBox_Series.Name = "pictureBox_Series";
            this.pictureBox_Series.Size = new System.Drawing.Size(484, 128);
            this.pictureBox_Series.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_Series.TabIndex = 147;
            this.pictureBox_Series.TabStop = false;
            // 
            // tabPage_Import
            // 
            this.tabPage_Import.AutoScroll = true;
            this.tabPage_Import.BackColor = System.Drawing.Color.Transparent;
            this.tabPage_Import.Controls.Add(this.splitContainer_SettingsOutput);
            this.tabPage_Import.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Import.Name = "tabPage_Import";
            this.tabPage_Import.Size = new System.Drawing.Size(684, 743);
            this.tabPage_Import.TabIndex = 4;
            this.tabPage_Import.Text = "Import Settings";
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
            this.splitContainer_SettingsOutput.Panel2.Controls.Add(this.label11);
            this.splitContainer_SettingsOutput.Panel2.Controls.Add(this.comboBox1);
            this.splitContainer_SettingsOutput.Panel2.Controls.Add(this.progressBar_Parsing);
            this.splitContainer_SettingsOutput.Panel2.Controls.Add(this.button_Start);
            this.splitContainer_SettingsOutput.Size = new System.Drawing.Size(684, 743);
            this.splitContainer_SettingsOutput.SplitterDistance = 676;
            this.splitContainer_SettingsOutput.TabIndex = 157;
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
            this.splitContainerImportSettings.Panel2.Controls.Add(this.panel_StringReplacements);
            this.splitContainerImportSettings.Panel2.Controls.Add(this.panel_OnlineData);
            this.splitContainerImportSettings.Panel2.Controls.Add(this.panel_ParsingTest);
            this.splitContainerImportSettings.Panel2.Controls.Add(this.panel_ImportPathes);
            this.splitContainerImportSettings.Panel2.Controls.Add(this.panel_Expressions);
            this.splitContainerImportSettings.Size = new System.Drawing.Size(684, 676);
            this.splitContainerImportSettings.SplitterDistance = 151;
            this.splitContainerImportSettings.TabIndex = 156;
            // 
            // treeView_Settings
            // 
            this.treeView_Settings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_Settings.Location = new System.Drawing.Point(0, 0);
            this.treeView_Settings.Name = "treeView_Settings";
            this.treeView_Settings.Size = new System.Drawing.Size(151, 676);
            this.treeView_Settings.TabIndex = 153;
            this.treeView_Settings.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_Settings_AfterSelect);
            // 
            // panel_StringReplacements
            // 
            this.panel_StringReplacements.Controls.Add(this.dataGridView_Replace);
            this.panel_StringReplacements.Location = new System.Drawing.Point(236, 216);
            this.panel_StringReplacements.Name = "panel_StringReplacements";
            this.panel_StringReplacements.Size = new System.Drawing.Size(157, 75);
            this.panel_StringReplacements.TabIndex = 155;
            this.panel_StringReplacements.Tag = "String Replacements";
            // 
            // dataGridView_Replace
            // 
            this.dataGridView_Replace.AllowUserToResizeColumns = false;
            this.dataGridView_Replace.AllowUserToResizeRows = false;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_Replace.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView_Replace.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView_Replace.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView_Replace.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView_Replace.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Replace.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView_Replace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_Replace.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_Replace.Name = "dataGridView_Replace";
            this.dataGridView_Replace.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dataGridView_Replace.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_Replace.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView_Replace.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_Replace.RowTemplate.Height = 18;
            this.dataGridView_Replace.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Replace.Size = new System.Drawing.Size(157, 75);
            this.dataGridView_Replace.StandardTab = true;
            this.dataGridView_Replace.TabIndex = 150;
            this.dataGridView_Replace.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView_Replace_UserDeletedRow);
            this.dataGridView_Replace.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_Replace_CellEndEdit);
            // 
            // panel_OnlineData
            // 
            this.panel_OnlineData.Controls.Add(this.cleanBanners);
            this.panel_OnlineData.Controls.Add(this.checkBox_doFolderWatch);
            this.panel_OnlineData.Controls.Add(this.linkLabel1);
            this.panel_OnlineData.Controls.Add(this.checkBox_DontClearMissingLocalFiles);
            this.panel_OnlineData.Controls.Add(this.checkBox_OnlineSearch);
            this.panel_OnlineData.Controls.Add(this.checkBox_LocalDataOverride);
            this.panel_OnlineData.Controls.Add(this.checkBox_AutoChooseSeries);
            this.panel_OnlineData.Controls.Add(this.checkBox_FullSeriesRetrieval);
            this.panel_OnlineData.Location = new System.Drawing.Point(177, 6);
            this.panel_OnlineData.Name = "panel_OnlineData";
            this.panel_OnlineData.Size = new System.Drawing.Size(465, 195);
            this.panel_OnlineData.TabIndex = 157;
            this.panel_OnlineData.Tag = "Online Data Sync";
            // 
            // cleanBanners
            // 
            this.cleanBanners.AutoSize = true;
            this.cleanBanners.Location = new System.Drawing.Point(3, 161);
            this.cleanBanners.Name = "cleanBanners";
            this.cleanBanners.Size = new System.Drawing.Size(88, 13);
            this.cleanBanners.TabIndex = 14;
            this.cleanBanners.TabStop = true;
            this.cleanBanners.Text = "Cleanup Banners";
            this.cleanBanners.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.cleanBanners_LinkClicked);
            // 
            // checkBox_doFolderWatch
            // 
            this.checkBox_doFolderWatch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_doFolderWatch.Location = new System.Drawing.Point(4, 123);
            this.checkBox_doFolderWatch.Name = "checkBox_doFolderWatch";
            this.checkBox_doFolderWatch.Size = new System.Drawing.Size(415, 17);
            this.checkBox_doFolderWatch.TabIndex = 13;
            this.checkBox_doFolderWatch.Text = "Watch my important folders for changes automatically";
            this.checkBox_doFolderWatch.UseVisualStyleBackColor = true;
            this.checkBox_doFolderWatch.CheckedChanged += new System.EventHandler(this.checkBox_doFolderWatch_CheckedChanged);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(3, 143);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(132, 13);
            this.linkLabel1.TabIndex = 12;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Force update of MediaInfo";
            this.toolTip_Help.SetToolTip(this.linkLabel1, "Click here to force the plugin to read the mediainfos (such as resolution) now!");
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // checkBox_DontClearMissingLocalFiles
            // 
            this.checkBox_DontClearMissingLocalFiles.AutoSize = true;
            this.checkBox_DontClearMissingLocalFiles.Location = new System.Drawing.Point(4, 77);
            this.checkBox_DontClearMissingLocalFiles.Name = "checkBox_DontClearMissingLocalFiles";
            this.checkBox_DontClearMissingLocalFiles.Size = new System.Drawing.Size(289, 17);
            this.checkBox_DontClearMissingLocalFiles.TabIndex = 6;
            this.checkBox_DontClearMissingLocalFiles.Text = "Don\'t clear local reference for missing local files on scan";
            this.checkBox_DontClearMissingLocalFiles.UseVisualStyleBackColor = true;
            this.checkBox_DontClearMissingLocalFiles.CheckedChanged += new System.EventHandler(this.checkBox_DontClearMissingLocalFiles_CheckedChanged);
            // 
            // checkBox_OnlineSearch
            // 
            this.checkBox_OnlineSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_OnlineSearch.Location = new System.Drawing.Point(4, 7);
            this.checkBox_OnlineSearch.Name = "checkBox_OnlineSearch";
            this.checkBox_OnlineSearch.Size = new System.Drawing.Size(458, 17);
            this.checkBox_OnlineSearch.TabIndex = 5;
            this.checkBox_OnlineSearch.Text = "Enable Online Database data retrieval";
            this.toolTip_Help.SetToolTip(this.checkBox_OnlineSearch, "When not checked, no online data will be pulled out");
            this.checkBox_OnlineSearch.UseVisualStyleBackColor = true;
            this.checkBox_OnlineSearch.CheckedChanged += new System.EventHandler(this.checkBox_OnlineSearch_CheckedChanged);
            // 
            // checkBox_LocalDataOverride
            // 
            this.checkBox_LocalDataOverride.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_LocalDataOverride.Location = new System.Drawing.Point(4, 100);
            this.checkBox_LocalDataOverride.Name = "checkBox_LocalDataOverride";
            this.checkBox_LocalDataOverride.Size = new System.Drawing.Size(415, 17);
            this.checkBox_LocalDataOverride.TabIndex = 2;
            this.checkBox_LocalDataOverride.Text = "Online data should override data from file parsing (series name, episode name)";
            this.checkBox_LocalDataOverride.UseVisualStyleBackColor = true;
            this.checkBox_LocalDataOverride.Visible = false;
            this.checkBox_LocalDataOverride.CheckedChanged += new System.EventHandler(this.checkBox_LocalDataOverride_CheckedChanged);
            // 
            // checkBox_AutoChooseSeries
            // 
            this.checkBox_AutoChooseSeries.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_AutoChooseSeries.Location = new System.Drawing.Point(4, 54);
            this.checkBox_AutoChooseSeries.Name = "checkBox_AutoChooseSeries";
            this.checkBox_AutoChooseSeries.Size = new System.Drawing.Size(458, 17);
            this.checkBox_AutoChooseSeries.TabIndex = 1;
            this.checkBox_AutoChooseSeries.Text = "Auto-Choose series when multiple entries are returned from the Online Database";
            this.toolTip_Help.SetToolTip(this.checkBox_AutoChooseSeries, "When checked, if there is exactly one exact match in the returned list, it will b" +
                    "e used. Otherwise, in any conflicting case the choose series dialog will pop up");
            this.checkBox_AutoChooseSeries.UseVisualStyleBackColor = true;
            this.checkBox_AutoChooseSeries.CheckedChanged += new System.EventHandler(this.checkBox_AutoChooseSeries_CheckedChanged);
            // 
            // checkBox_FullSeriesRetrieval
            // 
            this.checkBox_FullSeriesRetrieval.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_FullSeriesRetrieval.AutoEllipsis = true;
            this.checkBox_FullSeriesRetrieval.Location = new System.Drawing.Point(4, 30);
            this.checkBox_FullSeriesRetrieval.Name = "checkBox_FullSeriesRetrieval";
            this.checkBox_FullSeriesRetrieval.Size = new System.Drawing.Size(458, 17);
            this.checkBox_FullSeriesRetrieval.TabIndex = 0;
            this.checkBox_FullSeriesRetrieval.Text = "Retrieve episode information for the whole series";
            this.toolTip_Help.SetToolTip(this.checkBox_FullSeriesRetrieval, "When unchecked, online data will be pulled out only for the existing episode. Oth" +
                    "erwise all seasons / episodes will be fetched");
            this.checkBox_FullSeriesRetrieval.UseVisualStyleBackColor = true;
            this.checkBox_FullSeriesRetrieval.CheckedChanged += new System.EventHandler(this.checkBox_FullSeriesRetrieval_CheckedChanged);
            // 
            // panel_ParsingTest
            // 
            this.panel_ParsingTest.Controls.Add(this.button_TestReparse);
            this.panel_ParsingTest.Controls.Add(this.listView_ParsingResults);
            this.panel_ParsingTest.Location = new System.Drawing.Point(2, 167);
            this.panel_ParsingTest.Name = "panel_ParsingTest";
            this.panel_ParsingTest.Size = new System.Drawing.Size(157, 113);
            this.panel_ParsingTest.TabIndex = 156;
            this.panel_ParsingTest.Tag = "Parsing Test";
            // 
            // button_TestReparse
            // 
            this.button_TestReparse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_TestReparse.Image = ((System.Drawing.Image)(resources.GetObject("button_TestReparse.Image")));
            this.button_TestReparse.Location = new System.Drawing.Point(126, 3);
            this.button_TestReparse.Name = "button_TestReparse";
            this.button_TestReparse.Size = new System.Drawing.Size(28, 31);
            this.button_TestReparse.TabIndex = 153;
            this.toolTip_Help.SetToolTip(this.button_TestReparse, "Redo a local parsing test");
            this.button_TestReparse.UseVisualStyleBackColor = true;
            this.button_TestReparse.Click += new System.EventHandler(this.button_TestReparse_Click);
            // 
            // listView_ParsingResults
            // 
            this.listView_ParsingResults.AllowColumnReorder = true;
            this.listView_ParsingResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView_ParsingResults.FullRowSelect = true;
            this.listView_ParsingResults.GridLines = true;
            this.listView_ParsingResults.HideSelection = false;
            this.listView_ParsingResults.Location = new System.Drawing.Point(0, 0);
            this.listView_ParsingResults.MultiSelect = false;
            this.listView_ParsingResults.Name = "listView_ParsingResults";
            this.listView_ParsingResults.Size = new System.Drawing.Size(123, 113);
            this.listView_ParsingResults.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView_ParsingResults.TabIndex = 49;
            this.listView_ParsingResults.UseCompatibleStateImageBehavior = false;
            this.listView_ParsingResults.View = System.Windows.Forms.View.Details;
            // 
            // panel_ImportPathes
            // 
            this.panel_ImportPathes.Controls.Add(this.dataGridView_ImportPathes);
            this.panel_ImportPathes.Location = new System.Drawing.Point(2, 86);
            this.panel_ImportPathes.Name = "panel_ImportPathes";
            this.panel_ImportPathes.Size = new System.Drawing.Size(157, 75);
            this.panel_ImportPathes.TabIndex = 154;
            this.panel_ImportPathes.Tag = "Import Paths";
            // 
            // dataGridView_ImportPathes
            // 
            this.dataGridView_ImportPathes.AllowUserToResizeColumns = false;
            this.dataGridView_ImportPathes.AllowUserToResizeRows = false;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_ImportPathes.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridView_ImportPathes.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView_ImportPathes.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView_ImportPathes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView_ImportPathes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_ImportPathes.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView_ImportPathes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_ImportPathes.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_ImportPathes.Name = "dataGridView_ImportPathes";
            this.dataGridView_ImportPathes.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dataGridView_ImportPathes.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_ImportPathes.RowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridView_ImportPathes.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_ImportPathes.RowTemplate.Height = 18;
            this.dataGridView_ImportPathes.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_ImportPathes.Size = new System.Drawing.Size(157, 75);
            this.dataGridView_ImportPathes.StandardTab = true;
            this.dataGridView_ImportPathes.TabIndex = 150;
            this.dataGridView_ImportPathes.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView_ImportPathes_UserDeletedRow);
            this.dataGridView_ImportPathes.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_ImportPathes_CellValueChanged);
            this.dataGridView_ImportPathes.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_ImportPathes_CellContentClick);
            // 
            // panel_Expressions
            // 
            this.panel_Expressions.Controls.Add(this.dataGridView_Expressions);
            this.panel_Expressions.Controls.Add(this.button_MoveExpUp);
            this.panel_Expressions.Controls.Add(this.button_MoveExpDown);
            this.panel_Expressions.Location = new System.Drawing.Point(2, 3);
            this.panel_Expressions.Name = "panel_Expressions";
            this.panel_Expressions.Size = new System.Drawing.Size(157, 77);
            this.panel_Expressions.TabIndex = 155;
            this.panel_Expressions.Tag = "Parsing Expressions";
            // 
            // dataGridView_Expressions
            // 
            this.dataGridView_Expressions.AllowUserToResizeColumns = false;
            this.dataGridView_Expressions.AllowUserToResizeRows = false;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_Expressions.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridView_Expressions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_Expressions.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView_Expressions.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView_Expressions.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView_Expressions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Expressions.DefaultCellStyle = dataGridViewCellStyle9;
            this.dataGridView_Expressions.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_Expressions.MultiSelect = false;
            this.dataGridView_Expressions.Name = "dataGridView_Expressions";
            this.dataGridView_Expressions.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dataGridView_Expressions.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_Expressions.RowsDefaultCellStyle = dataGridViewCellStyle10;
            this.dataGridView_Expressions.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_Expressions.RowTemplate.Height = 18;
            this.dataGridView_Expressions.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Expressions.Size = new System.Drawing.Size(123, 77);
            this.dataGridView_Expressions.StandardTab = true;
            this.dataGridView_Expressions.TabIndex = 149;
            this.dataGridView_Expressions.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView_Expressions_UserDeletedRow);
            this.dataGridView_Expressions.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_Expressions_CellEndEdit);
            // 
            // button_MoveExpUp
            // 
            this.button_MoveExpUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_MoveExpUp.Image = ((System.Drawing.Image)(resources.GetObject("button_MoveExpUp.Image")));
            this.button_MoveExpUp.Location = new System.Drawing.Point(126, 3);
            this.button_MoveExpUp.Name = "button_MoveExpUp";
            this.button_MoveExpUp.Size = new System.Drawing.Size(28, 29);
            this.button_MoveExpUp.TabIndex = 152;
            this.button_MoveExpUp.UseVisualStyleBackColor = true;
            this.button_MoveExpUp.Click += new System.EventHandler(this.button_MoveExpUp_Click);
            // 
            // button_MoveExpDown
            // 
            this.button_MoveExpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_MoveExpDown.Image = ((System.Drawing.Image)(resources.GetObject("button_MoveExpDown.Image")));
            this.button_MoveExpDown.Location = new System.Drawing.Point(126, 38);
            this.button_MoveExpDown.Name = "button_MoveExpDown";
            this.button_MoveExpDown.Size = new System.Drawing.Size(28, 29);
            this.button_MoveExpDown.TabIndex = 152;
            this.button_MoveExpDown.UseVisualStyleBackColor = true;
            this.button_MoveExpDown.Click += new System.EventHandler(this.button_MoveExpDown_Click);
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(519, 11);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(57, 13);
            this.label11.TabIndex = 7;
            this.label11.Text = "Log Level:";
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Normal",
            "Debug (huge!)"});
            this.comboBox1.Location = new System.Drawing.Point(577, 8);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(99, 21);
            this.comboBox1.TabIndex = 6;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // progressBar_Parsing
            // 
            this.progressBar_Parsing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar_Parsing.Location = new System.Drawing.Point(119, 7);
            this.progressBar_Parsing.Name = "progressBar_Parsing";
            this.progressBar_Parsing.Size = new System.Drawing.Size(377, 20);
            this.progressBar_Parsing.TabIndex = 3;
            // 
            // button_Start
            // 
            this.button_Start.Location = new System.Drawing.Point(8, 6);
            this.button_Start.Name = "button_Start";
            this.button_Start.Size = new System.Drawing.Size(105, 23);
            this.button_Start.TabIndex = 4;
            this.button_Start.Text = "Start Import";
            this.button_Start.UseVisualStyleBackColor = true;
            this.button_Start.Click += new System.EventHandler(this.button_Start_Click);
            // 
            // tab_view
            // 
            this.tab_view.Controls.Add(this.groupBox10);
            this.tab_view.Controls.Add(this.groupBox9);
            this.tab_view.Controls.Add(this.groupBox8);
            this.tab_view.Controls.Add(this.listBox1);
            this.tab_view.Controls.Add(this.numericUpDown1);
            this.tab_view.Controls.Add(this.button3);
            this.tab_view.Controls.Add(this.richTextBox1);
            this.tab_view.Location = new System.Drawing.Point(4, 22);
            this.tab_view.Name = "tab_view";
            this.tab_view.Padding = new System.Windows.Forms.Padding(3);
            this.tab_view.Size = new System.Drawing.Size(684, 743);
            this.tab_view.TabIndex = 7;
            this.tab_view.Text = "Views";
            this.tab_view.UseVisualStyleBackColor = true;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.groupBox11);
            this.groupBox10.Controls.Add(this.viewStepGroupLbl);
            this.groupBox10.Controls.Add(this.viewStepGroupByTextBox);
            this.groupBox10.Controls.Add(this.viewStepType);
            this.groupBox10.Location = new System.Drawing.Point(8, 344);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(484, 273);
            this.groupBox10.TabIndex = 6;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "configure Hierarchy Step";
            this.groupBox10.Visible = false;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.comboBox7);
            this.groupBox11.Controls.Add(this.textBox5);
            this.groupBox11.Controls.Add(this.comboBox8);
            this.groupBox11.Controls.Add(this.richTextBox4);
            this.groupBox11.Controls.Add(this.comboBox9);
            this.groupBox11.Controls.Add(this.textBox6);
            this.groupBox11.Controls.Add(this.comboBox10);
            this.groupBox11.Controls.Add(this.richTextBox5);
            this.groupBox11.Controls.Add(this.comboBox3);
            this.groupBox11.Controls.Add(this.textBox3);
            this.groupBox11.Controls.Add(this.comboBox4);
            this.groupBox11.Controls.Add(this.richTextBox2);
            this.groupBox11.Controls.Add(this.comboBox5);
            this.groupBox11.Controls.Add(this.textBox4);
            this.groupBox11.Controls.Add(this.comboBox6);
            this.groupBox11.Controls.Add(this.richTextBox3);
            this.groupBox11.Controls.Add(this._23_link);
            this.groupBox11.Controls.Add(this.cond3_cond);
            this.groupBox11.Controls.Add(this.cond3_type);
            this.groupBox11.Controls.Add(this.cond3_what);
            this.groupBox11.Controls.Add(this._12_link);
            this.groupBox11.Controls.Add(this.cond2_cond);
            this.groupBox11.Controls.Add(this.cond2_type);
            this.groupBox11.Controls.Add(this.cond2_what);
            this.groupBox11.Controls.Add(this.label27);
            this.groupBox11.Controls.Add(this.cond1_cond);
            this.groupBox11.Controls.Add(this.cond1_type);
            this.groupBox11.Controls.Add(this.cond1_what);
            this.groupBox11.Location = new System.Drawing.Point(6, 62);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(455, 205);
            this.groupBox11.TabIndex = 3;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Step Conditions";
            // 
            // comboBox7
            // 
            this.comboBox7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox7.Enabled = false;
            this.comboBox7.FormattingEnabled = true;
            this.comboBox7.Items.AddRange(new object[] {
            "AND",
            "OR"});
            this.comboBox7.Location = new System.Drawing.Point(20, 177);
            this.comboBox7.Name = "comboBox7";
            this.comboBox7.Size = new System.Drawing.Size(57, 21);
            this.comboBox7.TabIndex = 41;
            // 
            // textBox5
            // 
            this.textBox5.Enabled = false;
            this.textBox5.Location = new System.Drawing.Point(312, 175);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(130, 20);
            this.textBox5.TabIndex = 40;
            // 
            // comboBox8
            // 
            this.comboBox8.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox8.Enabled = false;
            this.comboBox8.FormattingEnabled = true;
            this.comboBox8.Items.AddRange(new object[] {
            "=",
            "!=",
            ">",
            ">=",
            "<",
            "<=",
            "contains",
            "!contains"});
            this.comboBox8.Location = new System.Drawing.Point(231, 175);
            this.comboBox8.Name = "comboBox8";
            this.comboBox8.Size = new System.Drawing.Size(75, 21);
            this.comboBox8.TabIndex = 39;
            // 
            // richTextBox4
            // 
            this.richTextBox4.Enabled = false;
            this.richTextBox4.Location = new System.Drawing.Point(83, 177);
            this.richTextBox4.Name = "richTextBox4";
            this.richTextBox4.Size = new System.Drawing.Size(142, 20);
            this.richTextBox4.TabIndex = 38;
            this.richTextBox4.Text = "";
            // 
            // comboBox9
            // 
            this.comboBox9.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox9.Enabled = false;
            this.comboBox9.FormattingEnabled = true;
            this.comboBox9.Items.AddRange(new object[] {
            "AND",
            "OR"});
            this.comboBox9.Location = new System.Drawing.Point(20, 151);
            this.comboBox9.Name = "comboBox9";
            this.comboBox9.Size = new System.Drawing.Size(57, 21);
            this.comboBox9.TabIndex = 37;
            // 
            // textBox6
            // 
            this.textBox6.Enabled = false;
            this.textBox6.Location = new System.Drawing.Point(312, 149);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(130, 20);
            this.textBox6.TabIndex = 36;
            // 
            // comboBox10
            // 
            this.comboBox10.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox10.Enabled = false;
            this.comboBox10.FormattingEnabled = true;
            this.comboBox10.Items.AddRange(new object[] {
            "=",
            "!=",
            ">",
            ">=",
            "<",
            "<=",
            "contains",
            "!contains"});
            this.comboBox10.Location = new System.Drawing.Point(231, 149);
            this.comboBox10.Name = "comboBox10";
            this.comboBox10.Size = new System.Drawing.Size(75, 21);
            this.comboBox10.TabIndex = 35;
            // 
            // richTextBox5
            // 
            this.richTextBox5.Enabled = false;
            this.richTextBox5.Location = new System.Drawing.Point(83, 151);
            this.richTextBox5.Name = "richTextBox5";
            this.richTextBox5.Size = new System.Drawing.Size(142, 20);
            this.richTextBox5.TabIndex = 34;
            this.richTextBox5.Text = "";
            // 
            // comboBox3
            // 
            this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox3.Enabled = false;
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "AND",
            "OR"});
            this.comboBox3.Location = new System.Drawing.Point(20, 125);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(57, 21);
            this.comboBox3.TabIndex = 33;
            // 
            // textBox3
            // 
            this.textBox3.Enabled = false;
            this.textBox3.Location = new System.Drawing.Point(312, 123);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(130, 20);
            this.textBox3.TabIndex = 32;
            // 
            // comboBox4
            // 
            this.comboBox4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox4.Enabled = false;
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Items.AddRange(new object[] {
            "=",
            "!=",
            ">",
            ">=",
            "<",
            "<=",
            "contains",
            "!contains"});
            this.comboBox4.Location = new System.Drawing.Point(231, 123);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(75, 21);
            this.comboBox4.TabIndex = 31;
            // 
            // richTextBox2
            // 
            this.richTextBox2.Enabled = false;
            this.richTextBox2.Location = new System.Drawing.Point(83, 125);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(142, 20);
            this.richTextBox2.TabIndex = 30;
            this.richTextBox2.Text = "";
            // 
            // comboBox5
            // 
            this.comboBox5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox5.Enabled = false;
            this.comboBox5.FormattingEnabled = true;
            this.comboBox5.Items.AddRange(new object[] {
            "AND",
            "OR"});
            this.comboBox5.Location = new System.Drawing.Point(20, 99);
            this.comboBox5.Name = "comboBox5";
            this.comboBox5.Size = new System.Drawing.Size(57, 21);
            this.comboBox5.TabIndex = 29;
            // 
            // textBox4
            // 
            this.textBox4.Enabled = false;
            this.textBox4.Location = new System.Drawing.Point(312, 97);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(130, 20);
            this.textBox4.TabIndex = 28;
            // 
            // comboBox6
            // 
            this.comboBox6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox6.Enabled = false;
            this.comboBox6.FormattingEnabled = true;
            this.comboBox6.Items.AddRange(new object[] {
            "=",
            "!=",
            ">",
            ">=",
            "<",
            "<=",
            "contains",
            "!contains"});
            this.comboBox6.Location = new System.Drawing.Point(231, 97);
            this.comboBox6.Name = "comboBox6";
            this.comboBox6.Size = new System.Drawing.Size(75, 21);
            this.comboBox6.TabIndex = 27;
            // 
            // richTextBox3
            // 
            this.richTextBox3.Enabled = false;
            this.richTextBox3.Location = new System.Drawing.Point(83, 99);
            this.richTextBox3.Name = "richTextBox3";
            this.richTextBox3.Size = new System.Drawing.Size(142, 20);
            this.richTextBox3.TabIndex = 26;
            this.richTextBox3.Text = "";
            // 
            // _23_link
            // 
            this._23_link.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._23_link.Enabled = false;
            this._23_link.FormattingEnabled = true;
            this._23_link.Items.AddRange(new object[] {
            "AND",
            "OR"});
            this._23_link.Location = new System.Drawing.Point(20, 72);
            this._23_link.Name = "_23_link";
            this._23_link.Size = new System.Drawing.Size(57, 21);
            this._23_link.TabIndex = 25;
            // 
            // cond3_cond
            // 
            this.cond3_cond.Enabled = false;
            this.cond3_cond.Location = new System.Drawing.Point(312, 70);
            this.cond3_cond.Name = "cond3_cond";
            this.cond3_cond.Size = new System.Drawing.Size(130, 20);
            this.cond3_cond.TabIndex = 24;
            // 
            // cond3_type
            // 
            this.cond3_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cond3_type.Enabled = false;
            this.cond3_type.FormattingEnabled = true;
            this.cond3_type.Items.AddRange(new object[] {
            "=",
            "!=",
            ">",
            ">=",
            "<",
            "<=",
            "contains",
            "!contains"});
            this.cond3_type.Location = new System.Drawing.Point(231, 70);
            this.cond3_type.Name = "cond3_type";
            this.cond3_type.Size = new System.Drawing.Size(75, 21);
            this.cond3_type.TabIndex = 23;
            // 
            // cond3_what
            // 
            this.cond3_what.Enabled = false;
            this.cond3_what.Location = new System.Drawing.Point(83, 72);
            this.cond3_what.Name = "cond3_what";
            this.cond3_what.Size = new System.Drawing.Size(142, 20);
            this.cond3_what.TabIndex = 22;
            this.cond3_what.Text = "";
            // 
            // _12_link
            // 
            this._12_link.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._12_link.Enabled = false;
            this._12_link.FormattingEnabled = true;
            this._12_link.Items.AddRange(new object[] {
            "AND",
            "OR"});
            this._12_link.Location = new System.Drawing.Point(20, 46);
            this._12_link.Name = "_12_link";
            this._12_link.Size = new System.Drawing.Size(57, 21);
            this._12_link.TabIndex = 21;
            // 
            // cond2_cond
            // 
            this.cond2_cond.Enabled = false;
            this.cond2_cond.Location = new System.Drawing.Point(312, 44);
            this.cond2_cond.Name = "cond2_cond";
            this.cond2_cond.Size = new System.Drawing.Size(130, 20);
            this.cond2_cond.TabIndex = 20;
            // 
            // cond2_type
            // 
            this.cond2_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cond2_type.Enabled = false;
            this.cond2_type.FormattingEnabled = true;
            this.cond2_type.Items.AddRange(new object[] {
            "=",
            "!=",
            ">",
            ">=",
            "<",
            "<=",
            "contains",
            "!contains"});
            this.cond2_type.Location = new System.Drawing.Point(231, 44);
            this.cond2_type.Name = "cond2_type";
            this.cond2_type.Size = new System.Drawing.Size(75, 21);
            this.cond2_type.TabIndex = 19;
            // 
            // cond2_what
            // 
            this.cond2_what.Enabled = false;
            this.cond2_what.Location = new System.Drawing.Point(83, 46);
            this.cond2_what.Name = "cond2_what";
            this.cond2_what.Size = new System.Drawing.Size(142, 20);
            this.cond2_what.TabIndex = 18;
            this.cond2_what.Text = "";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(20, 25);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(30, 13);
            this.label27.TabIndex = 17;
            this.label27.Text = "...if...";
            // 
            // cond1_cond
            // 
            this.cond1_cond.Location = new System.Drawing.Point(312, 17);
            this.cond1_cond.Name = "cond1_cond";
            this.cond1_cond.Size = new System.Drawing.Size(130, 20);
            this.cond1_cond.TabIndex = 16;
            // 
            // cond1_type
            // 
            this.cond1_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cond1_type.FormattingEnabled = true;
            this.cond1_type.Items.AddRange(new object[] {
            "=",
            "!=",
            ">",
            ">=",
            "<",
            "<=",
            "contains",
            "!contains"});
            this.cond1_type.Location = new System.Drawing.Point(231, 17);
            this.cond1_type.Name = "cond1_type";
            this.cond1_type.Size = new System.Drawing.Size(75, 21);
            this.cond1_type.TabIndex = 15;
            // 
            // cond1_what
            // 
            this.cond1_what.Location = new System.Drawing.Point(83, 19);
            this.cond1_what.Name = "cond1_what";
            this.cond1_what.Size = new System.Drawing.Size(142, 20);
            this.cond1_what.TabIndex = 14;
            this.cond1_what.Text = "";
            // 
            // viewStepGroupLbl
            // 
            this.viewStepGroupLbl.AutoSize = true;
            this.viewStepGroupLbl.Location = new System.Drawing.Point(133, 35);
            this.viewStepGroupLbl.Name = "viewStepGroupLbl";
            this.viewStepGroupLbl.Size = new System.Drawing.Size(49, 13);
            this.viewStepGroupLbl.TabIndex = 2;
            this.viewStepGroupLbl.Text = "group By";
            // 
            // viewStepGroupByTextBox
            // 
            this.viewStepGroupByTextBox.Location = new System.Drawing.Point(188, 32);
            this.viewStepGroupByTextBox.Name = "viewStepGroupByTextBox";
            this.viewStepGroupByTextBox.Size = new System.Drawing.Size(108, 20);
            this.viewStepGroupByTextBox.TabIndex = 1;
            // 
            // viewStepType
            // 
            this.viewStepType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.viewStepType.FormattingEnabled = true;
            this.viewStepType.Items.AddRange(new object[] {
            "group",
            "series",
            "season",
            "episode"});
            this.viewStepType.Location = new System.Drawing.Point(6, 32);
            this.viewStepType.Name = "viewStepType";
            this.viewStepType.Size = new System.Drawing.Size(121, 21);
            this.viewStepType.TabIndex = 0;
            this.viewStepType.SelectedIndexChanged += new System.EventHandler(this.viewStepType_SelectedIndexChanged);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.button7);
            this.groupBox9.Controls.Add(this.label25);
            this.groupBox9.Controls.Add(this.button2);
            this.groupBox9.Controls.Add(this.button4);
            this.groupBox9.Controls.Add(this.button5);
            this.groupBox9.Controls.Add(this.button6);
            this.groupBox9.Controls.Add(this.view_selStepsList);
            this.groupBox9.Controls.Add(this.view_selectedName);
            this.groupBox9.Controls.Add(this.label1);
            this.groupBox9.Location = new System.Drawing.Point(252, 23);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(240, 315);
            this.groupBox9.TabIndex = 5;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "define View";
            this.groupBox9.Visible = false;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(90, 55);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(104, 23);
            this.button7.TabIndex = 161;
            this.button7.Text = "Paste all at once";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(6, 68);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(52, 13);
            this.label25.TabIndex = 160;
            this.label25.Text = "Hierarchy";
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.Location = new System.Drawing.Point(200, 143);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(28, 29);
            this.button2.TabIndex = 159;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Image = ((System.Drawing.Image)(resources.GetObject("button4.Image")));
            this.button4.Location = new System.Drawing.Point(200, 178);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(28, 29);
            this.button4.TabIndex = 158;
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(44, 276);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(69, 23);
            this.button5.TabIndex = 157;
            this.button5.Text = "Remove";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(119, 276);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 155;
            this.button6.Text = "Add";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // view_selStepsList
            // 
            this.view_selStepsList.FormattingEnabled = true;
            this.view_selStepsList.Location = new System.Drawing.Point(6, 84);
            this.view_selStepsList.Name = "view_selStepsList";
            this.view_selStepsList.Size = new System.Drawing.Size(188, 186);
            this.view_selStepsList.TabIndex = 156;
            this.view_selStepsList.SelectedIndexChanged += new System.EventHandler(this.view_selStepsList_SelectedIndexChanged);
            // 
            // view_selectedName
            // 
            this.view_selectedName.Location = new System.Drawing.Point(50, 28);
            this.view_selectedName.Name = "view_selectedName";
            this.view_selectedName.Size = new System.Drawing.Size(115, 20);
            this.view_selectedName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.btnViewUp);
            this.groupBox8.Controls.Add(this.btnViewDown);
            this.groupBox8.Controls.Add(this.btnRemoveView);
            this.groupBox8.Controls.Add(this.btnAddView);
            this.groupBox8.Controls.Add(this._availViews);
            this.groupBox8.Location = new System.Drawing.Point(8, 23);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(238, 315);
            this.groupBox8.TabIndex = 4;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "available Views";
            // 
            // btnViewUp
            // 
            this.btnViewUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnViewUp.Enabled = false;
            this.btnViewUp.Image = ((System.Drawing.Image)(resources.GetObject("btnViewUp.Image")));
            this.btnViewUp.Location = new System.Drawing.Point(203, 108);
            this.btnViewUp.Name = "btnViewUp";
            this.btnViewUp.Size = new System.Drawing.Size(28, 29);
            this.btnViewUp.TabIndex = 154;
            this.btnViewUp.UseVisualStyleBackColor = true;
            // 
            // btnViewDown
            // 
            this.btnViewDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnViewDown.Enabled = false;
            this.btnViewDown.Image = ((System.Drawing.Image)(resources.GetObject("btnViewDown.Image")));
            this.btnViewDown.Location = new System.Drawing.Point(203, 143);
            this.btnViewDown.Name = "btnViewDown";
            this.btnViewDown.Size = new System.Drawing.Size(28, 29);
            this.btnViewDown.TabIndex = 153;
            this.btnViewDown.UseVisualStyleBackColor = true;
            // 
            // btnRemoveView
            // 
            this.btnRemoveView.Enabled = false;
            this.btnRemoveView.Location = new System.Drawing.Point(44, 276);
            this.btnRemoveView.Name = "btnRemoveView";
            this.btnRemoveView.Size = new System.Drawing.Size(69, 23);
            this.btnRemoveView.TabIndex = 5;
            this.btnRemoveView.Text = "Remove";
            this.btnRemoveView.UseVisualStyleBackColor = true;
            // 
            // btnAddView
            // 
            this.btnAddView.Enabled = false;
            this.btnAddView.Location = new System.Drawing.Point(119, 276);
            this.btnAddView.Name = "btnAddView";
            this.btnAddView.Size = new System.Drawing.Size(75, 23);
            this.btnAddView.TabIndex = 5;
            this.btnAddView.Text = "Add";
            this.btnAddView.UseVisualStyleBackColor = true;
            // 
            // _availViews
            // 
            this._availViews.FormattingEnabled = true;
            this._availViews.Location = new System.Drawing.Point(6, 19);
            this._availViews.Name = "_availViews";
            this._availViews.Size = new System.Drawing.Size(188, 251);
            this._availViews.TabIndex = 5;
            this._availViews.SelectedIndexChanged += new System.EventHandler(this._availViews_SelectedIndexChanged);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(8, 695);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(426, 30);
            this.listBox1.TabIndex = 3;
            this.listBox1.Visible = false;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(234, 667);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown1.TabIndex = 2;
            this.numericUpDown1.Visible = false;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(360, 664);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 1;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(8, 623);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(426, 35);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            this.richTextBox1.Visible = false;
            // 
            // tabPage_MP_DisplayControl
            // 
            this.tabPage_MP_DisplayControl.AutoScroll = true;
            this.tabPage_MP_DisplayControl.AutoScrollMinSize = new System.Drawing.Size(0, 478);
            this.tabPage_MP_DisplayControl.Controls.Add(this.groupBox7);
            this.tabPage_MP_DisplayControl.Controls.Add(this.groupBox3);
            this.tabPage_MP_DisplayControl.Controls.Add(this.groupBox4);
            this.tabPage_MP_DisplayControl.Controls.Add(this.groupBox2);
            this.tabPage_MP_DisplayControl.Controls.Add(this.groupBox1);
            this.tabPage_MP_DisplayControl.Location = new System.Drawing.Point(4, 22);
            this.tabPage_MP_DisplayControl.Name = "tabPage_MP_DisplayControl";
            this.tabPage_MP_DisplayControl.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_MP_DisplayControl.Size = new System.Drawing.Size(684, 743);
            this.tabPage_MP_DisplayControl.TabIndex = 5;
            this.tabPage_MP_DisplayControl.Text = "MediaPortal Display Control";
            this.tabPage_MP_DisplayControl.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox7.Controls.Add(this.btnLogoEdit);
            this.groupBox7.Controls.Add(this.btnLogoDown);
            this.groupBox7.Controls.Add(this.btnlogoUp);
            this.groupBox7.Controls.Add(this.btnrmvLogo);
            this.groupBox7.Controls.Add(this.addLogo);
            this.groupBox7.Controls.Add(this.lstLogos);
            this.groupBox7.Location = new System.Drawing.Point(4, 478);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(662, 148);
            this.groupBox7.TabIndex = 3;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Logo Configuration";
            // 
            // btnLogoEdit
            // 
            this.btnLogoEdit.Location = new System.Drawing.Point(448, 119);
            this.btnLogoEdit.Name = "btnLogoEdit";
            this.btnLogoEdit.Size = new System.Drawing.Size(90, 23);
            this.btnLogoEdit.TabIndex = 5;
            this.btnLogoEdit.Text = "Edit";
            this.btnLogoEdit.UseVisualStyleBackColor = true;
            this.btnLogoEdit.Click += new System.EventHandler(this.btnLogoEdit_Click);
            // 
            // btnLogoDown
            // 
            this.btnLogoDown.Location = new System.Drawing.Point(7, 117);
            this.btnLogoDown.Name = "btnLogoDown";
            this.btnLogoDown.Size = new System.Drawing.Size(75, 23);
            this.btnLogoDown.TabIndex = 4;
            this.btnLogoDown.Text = "Down";
            this.btnLogoDown.UseVisualStyleBackColor = true;
            this.btnLogoDown.Click += new System.EventHandler(this.btnLogoDown_Click);
            // 
            // btnlogoUp
            // 
            this.btnlogoUp.Location = new System.Drawing.Point(88, 117);
            this.btnlogoUp.Name = "btnlogoUp";
            this.btnlogoUp.Size = new System.Drawing.Size(75, 23);
            this.btnlogoUp.TabIndex = 3;
            this.btnlogoUp.Text = "Up";
            this.btnlogoUp.UseVisualStyleBackColor = true;
            this.btnlogoUp.Click += new System.EventHandler(this.btnlogoUp_Click);
            // 
            // btnrmvLogo
            // 
            this.btnrmvLogo.Location = new System.Drawing.Point(350, 120);
            this.btnrmvLogo.Name = "btnrmvLogo";
            this.btnrmvLogo.Size = new System.Drawing.Size(92, 23);
            this.btnrmvLogo.TabIndex = 2;
            this.btnrmvLogo.Text = "Remove LogoRule";
            this.btnrmvLogo.UseVisualStyleBackColor = true;
            this.btnrmvLogo.Click += new System.EventHandler(this.btnrmvLogo_Click);
            // 
            // addLogo
            // 
            this.addLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addLogo.Location = new System.Drawing.Point(544, 118);
            this.addLogo.Name = "addLogo";
            this.addLogo.Size = new System.Drawing.Size(110, 23);
            this.addLogo.TabIndex = 1;
            this.addLogo.Text = "Add LogoRule";
            this.addLogo.UseVisualStyleBackColor = true;
            this.addLogo.Click += new System.EventHandler(this.addLogo_Click);
            // 
            // lstLogos
            // 
            this.lstLogos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstLogos.FormattingEnabled = true;
            this.lstLogos.Location = new System.Drawing.Point(7, 19);
            this.lstLogos.Name = "lstLogos";
            this.lstLogos.Size = new System.Drawing.Size(646, 95);
            this.lstLogos.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.comboBox_seasonFormat);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.richTextBox_seasonFormat_Col3);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.richTextBox_seasonFormat_Main);
            this.groupBox3.Controls.Add(this.richTextBox_seasonFormat_Col1);
            this.groupBox3.Controls.Add(this.richTextBox_seasonFormat_Subtitle);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.richTextBox_seasonFormat_Col2);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.richTextBox_seasonFormat_Title);
            this.groupBox3.Location = new System.Drawing.Point(4, 231);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(661, 108);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Season View Settings";
            // 
            // comboBox_seasonFormat
            // 
            this.comboBox_seasonFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_seasonFormat.FormattingEnabled = true;
            this.comboBox_seasonFormat.Location = new System.Drawing.Point(119, 15);
            this.comboBox_seasonFormat.Name = "comboBox_seasonFormat";
            this.comboBox_seasonFormat.Size = new System.Drawing.Size(86, 21);
            this.comboBox_seasonFormat.TabIndex = 3;
            this.comboBox_seasonFormat.SelectedIndexChanged += new System.EventHandler(this.comboBox_seasonFormat_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(4, 18);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(114, 13);
            this.label13.TabIndex = 1;
            this.label13.Text = "Season Listing Format:";
            // 
            // richTextBox_seasonFormat_Col3
            // 
            this.richTextBox_seasonFormat_Col3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_seasonFormat_Col3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seasonFormat_Col3.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seasonFormat_Col3.Location = new System.Drawing.Point(519, 15);
            this.richTextBox_seasonFormat_Col3.Multiline = false;
            this.richTextBox_seasonFormat_Col3.Name = "richTextBox_seasonFormat_Col3";
            this.richTextBox_seasonFormat_Col3.Size = new System.Drawing.Size(136, 20);
            this.richTextBox_seasonFormat_Col3.TabIndex = 2;
            this.richTextBox_seasonFormat_Col3.Text = "";
            this.richTextBox_seasonFormat_Col3.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // contextMenuStrip_InsertFields
            // 
            this.contextMenuStrip_InsertFields.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.contextMenuStrip_InsertFields.Name = "contextMenuStrip_SeriesFields";
            this.contextMenuStrip_InsertFields.Size = new System.Drawing.Size(61, 4);
            this.contextMenuStrip_InsertFields.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_SeriesFields_Opening);
            this.contextMenuStrip_InsertFields.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip_SeriesFields_ItemClicked);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(4, 40);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(150, 13);
            this.label12.TabIndex = 1;
            this.label12.Text = "Season Title InfoPane Format:";
            // 
            // richTextBox_seasonFormat_Main
            // 
            this.richTextBox_seasonFormat_Main.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_seasonFormat_Main.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seasonFormat_Main.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seasonFormat_Main.Location = new System.Drawing.Point(217, 80);
            this.richTextBox_seasonFormat_Main.Multiline = false;
            this.richTextBox_seasonFormat_Main.Name = "richTextBox_seasonFormat_Main";
            this.richTextBox_seasonFormat_Main.Size = new System.Drawing.Size(438, 20);
            this.richTextBox_seasonFormat_Main.TabIndex = 2;
            this.richTextBox_seasonFormat_Main.Text = "";
            this.richTextBox_seasonFormat_Main.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // richTextBox_seasonFormat_Col1
            // 
            this.richTextBox_seasonFormat_Col1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seasonFormat_Col1.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seasonFormat_Col1.Location = new System.Drawing.Point(217, 15);
            this.richTextBox_seasonFormat_Col1.Multiline = false;
            this.richTextBox_seasonFormat_Col1.Name = "richTextBox_seasonFormat_Col1";
            this.richTextBox_seasonFormat_Col1.Size = new System.Drawing.Size(151, 20);
            this.richTextBox_seasonFormat_Col1.TabIndex = 2;
            this.richTextBox_seasonFormat_Col1.Text = "";
            this.richTextBox_seasonFormat_Col1.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // richTextBox_seasonFormat_Subtitle
            // 
            this.richTextBox_seasonFormat_Subtitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_seasonFormat_Subtitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seasonFormat_Subtitle.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seasonFormat_Subtitle.Location = new System.Drawing.Point(217, 59);
            this.richTextBox_seasonFormat_Subtitle.Multiline = false;
            this.richTextBox_seasonFormat_Subtitle.Name = "richTextBox_seasonFormat_Subtitle";
            this.richTextBox_seasonFormat_Subtitle.Size = new System.Drawing.Size(438, 20);
            this.richTextBox_seasonFormat_Subtitle.TabIndex = 2;
            this.richTextBox_seasonFormat_Subtitle.Text = "";
            this.richTextBox_seasonFormat_Subtitle.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(4, 61);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(165, 13);
            this.label14.TabIndex = 1;
            this.label14.Text = "Season Subtitle InfoPane Format:";
            // 
            // richTextBox_seasonFormat_Col2
            // 
            this.richTextBox_seasonFormat_Col2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seasonFormat_Col2.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seasonFormat_Col2.Location = new System.Drawing.Point(368, 15);
            this.richTextBox_seasonFormat_Col2.Multiline = false;
            this.richTextBox_seasonFormat_Col2.Name = "richTextBox_seasonFormat_Col2";
            this.richTextBox_seasonFormat_Col2.Size = new System.Drawing.Size(151, 20);
            this.richTextBox_seasonFormat_Col2.TabIndex = 2;
            this.richTextBox_seasonFormat_Col2.Text = "";
            this.richTextBox_seasonFormat_Col2.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(4, 82);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(153, 13);
            this.label15.TabIndex = 1;
            this.label15.Text = "Season Main InfoPane Format:";
            // 
            // richTextBox_seasonFormat_Title
            // 
            this.richTextBox_seasonFormat_Title.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_seasonFormat_Title.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seasonFormat_Title.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seasonFormat_Title.Location = new System.Drawing.Point(217, 38);
            this.richTextBox_seasonFormat_Title.Multiline = false;
            this.richTextBox_seasonFormat_Title.Name = "richTextBox_seasonFormat_Title";
            this.richTextBox_seasonFormat_Title.Size = new System.Drawing.Size(438, 20);
            this.richTextBox_seasonFormat_Title.TabIndex = 2;
            this.richTextBox_seasonFormat_Title.Text = "";
            this.richTextBox_seasonFormat_Title.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.richTextBox_episodeFormat_Col3);
            this.groupBox4.Controls.Add(this.checkBox_Episode_HideUnwatchedSummary);
            this.groupBox4.Controls.Add(this.richTextBox_episodeFormat_Col2);
            this.groupBox4.Controls.Add(this.checkBox_Episode_OnlyShowLocalFiles);
            this.groupBox4.Controls.Add(this.richTextBox_episodeFormat_Main);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.richTextBox_episodeFormat_Subtitle);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.richTextBox_episodeFormat_Title);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.richTextBox_episodeFormat_Col1);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Location = new System.Drawing.Point(3, 345);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(662, 127);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Episode View Settings";
            // 
            // richTextBox_episodeFormat_Col3
            // 
            this.richTextBox_episodeFormat_Col3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_episodeFormat_Col3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_episodeFormat_Col3.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_episodeFormat_Col3.Location = new System.Drawing.Point(490, 14);
            this.richTextBox_episodeFormat_Col3.Multiline = false;
            this.richTextBox_episodeFormat_Col3.Name = "richTextBox_episodeFormat_Col3";
            this.richTextBox_episodeFormat_Col3.Size = new System.Drawing.Size(164, 20);
            this.richTextBox_episodeFormat_Col3.TabIndex = 2;
            this.richTextBox_episodeFormat_Col3.Text = "";
            this.richTextBox_episodeFormat_Col3.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // checkBox_Episode_HideUnwatchedSummary
            // 
            this.checkBox_Episode_HideUnwatchedSummary.AutoSize = true;
            this.checkBox_Episode_HideUnwatchedSummary.Location = new System.Drawing.Point(321, 104);
            this.checkBox_Episode_HideUnwatchedSummary.Name = "checkBox_Episode_HideUnwatchedSummary";
            this.checkBox_Episode_HideUnwatchedSummary.Size = new System.Drawing.Size(318, 17);
            this.checkBox_Episode_HideUnwatchedSummary.TabIndex = 1;
            this.checkBox_Episode_HideUnwatchedSummary.Text = "Hide Episode summary if the episode hasn\'t been watched yet";
            this.toolTip_Help.SetToolTip(this.checkBox_Episode_HideUnwatchedSummary, "Prevents the summary to be displayed for episodes not already marked as watched (" +
                    "prevents spoilers!)");
            this.checkBox_Episode_HideUnwatchedSummary.UseVisualStyleBackColor = true;
            this.checkBox_Episode_HideUnwatchedSummary.CheckedChanged += new System.EventHandler(this.checkBox_Episode_HideUnwatchedSummary_CheckedChanged);
            // 
            // richTextBox_episodeFormat_Col2
            // 
            this.richTextBox_episodeFormat_Col2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_episodeFormat_Col2.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_episodeFormat_Col2.Location = new System.Drawing.Point(320, 14);
            this.richTextBox_episodeFormat_Col2.Multiline = false;
            this.richTextBox_episodeFormat_Col2.Name = "richTextBox_episodeFormat_Col2";
            this.richTextBox_episodeFormat_Col2.Size = new System.Drawing.Size(169, 20);
            this.richTextBox_episodeFormat_Col2.TabIndex = 2;
            this.richTextBox_episodeFormat_Col2.Text = "";
            this.richTextBox_episodeFormat_Col2.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // checkBox_Episode_OnlyShowLocalFiles
            // 
            this.checkBox_Episode_OnlyShowLocalFiles.AutoSize = true;
            this.checkBox_Episode_OnlyShowLocalFiles.Location = new System.Drawing.Point(7, 104);
            this.checkBox_Episode_OnlyShowLocalFiles.Name = "checkBox_Episode_OnlyShowLocalFiles";
            this.checkBox_Episode_OnlyShowLocalFiles.Size = new System.Drawing.Size(238, 17);
            this.checkBox_Episode_OnlyShowLocalFiles.TabIndex = 0;
            this.checkBox_Episode_OnlyShowLocalFiles.Text = "Show only episodes with a matching local file";
            this.toolTip_Help.SetToolTip(this.checkBox_Episode_OnlyShowLocalFiles, "When checked, only episodes / seasons / series with a matching local file will be" +
                    " displayed in MP; otherwise all episodes are listed. That setting can be changed" +
                    " from MP");
            this.checkBox_Episode_OnlyShowLocalFiles.UseVisualStyleBackColor = true;
            this.checkBox_Episode_OnlyShowLocalFiles.CheckedChanged += new System.EventHandler(this.checkBox_Episode_MatchingLocalFile_CheckedChanged);
            // 
            // richTextBox_episodeFormat_Main
            // 
            this.richTextBox_episodeFormat_Main.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_episodeFormat_Main.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_episodeFormat_Main.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_episodeFormat_Main.Location = new System.Drawing.Point(218, 80);
            this.richTextBox_episodeFormat_Main.Multiline = false;
            this.richTextBox_episodeFormat_Main.Name = "richTextBox_episodeFormat_Main";
            this.richTextBox_episodeFormat_Main.Size = new System.Drawing.Size(436, 20);
            this.richTextBox_episodeFormat_Main.TabIndex = 2;
            this.richTextBox_episodeFormat_Main.Text = "";
            this.richTextBox_episodeFormat_Main.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(116, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Episode Listing Format:";
            // 
            // richTextBox_episodeFormat_Subtitle
            // 
            this.richTextBox_episodeFormat_Subtitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_episodeFormat_Subtitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_episodeFormat_Subtitle.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_episodeFormat_Subtitle.Location = new System.Drawing.Point(218, 58);
            this.richTextBox_episodeFormat_Subtitle.Multiline = false;
            this.richTextBox_episodeFormat_Subtitle.Name = "richTextBox_episodeFormat_Subtitle";
            this.richTextBox_episodeFormat_Subtitle.Size = new System.Drawing.Size(436, 20);
            this.richTextBox_episodeFormat_Subtitle.TabIndex = 2;
            this.richTextBox_episodeFormat_Subtitle.Text = "";
            this.richTextBox_episodeFormat_Subtitle.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 38);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(152, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Episode Title InfoPane Format:";
            // 
            // richTextBox_episodeFormat_Title
            // 
            this.richTextBox_episodeFormat_Title.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_episodeFormat_Title.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_episodeFormat_Title.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_episodeFormat_Title.Location = new System.Drawing.Point(218, 36);
            this.richTextBox_episodeFormat_Title.Multiline = false;
            this.richTextBox_episodeFormat_Title.Name = "richTextBox_episodeFormat_Title";
            this.richTextBox_episodeFormat_Title.Size = new System.Drawing.Size(436, 20);
            this.richTextBox_episodeFormat_Title.TabIndex = 2;
            this.richTextBox_episodeFormat_Title.Text = "";
            this.toolTip_Help.SetToolTip(this.richTextBox_episodeFormat_Title, "What\'s displayed in the Title info pane (usually the title!). Use \\n to introduce" +
                    " a line break");
            this.richTextBox_episodeFormat_Title.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 60);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(167, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Episode Subtitle InfoPane Format:";
            // 
            // richTextBox_episodeFormat_Col1
            // 
            this.richTextBox_episodeFormat_Col1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_episodeFormat_Col1.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_episodeFormat_Col1.Location = new System.Drawing.Point(150, 14);
            this.richTextBox_episodeFormat_Col1.Multiline = false;
            this.richTextBox_episodeFormat_Col1.Name = "richTextBox_episodeFormat_Col1";
            this.richTextBox_episodeFormat_Col1.Size = new System.Drawing.Size(169, 20);
            this.richTextBox_episodeFormat_Col1.TabIndex = 2;
            this.richTextBox_episodeFormat_Col1.Text = "";
            this.richTextBox_episodeFormat_Col1.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 82);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(155, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Episode Main InfoPane Format:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.comboBox_seriesFormat);
            this.groupBox2.Controls.Add(this.richTextBox_seriesFormat_Col3);
            this.groupBox2.Controls.Add(this.richTextBox_seriesFormat_Main);
            this.groupBox2.Controls.Add(this.richTextBox_seriesFormat_Subtitle);
            this.groupBox2.Controls.Add(this.richTextBox_seriesFormat_Col2);
            this.groupBox2.Controls.Add(this.richTextBox_seriesFormat_Title);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.richTextBox_seriesFormat_Col1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(3, 118);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(662, 107);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Series View Settings";
            // 
            // comboBox_seriesFormat
            // 
            this.comboBox_seriesFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_seriesFormat.FormattingEnabled = true;
            this.comboBox_seriesFormat.Location = new System.Drawing.Point(119, 15);
            this.comboBox_seriesFormat.Name = "comboBox_seriesFormat";
            this.comboBox_seriesFormat.Size = new System.Drawing.Size(86, 21);
            this.comboBox_seriesFormat.TabIndex = 3;
            this.comboBox_seriesFormat.SelectedIndexChanged += new System.EventHandler(this.comboBox_seriesFormat_SelectedIndexChanged);
            // 
            // richTextBox_seriesFormat_Col3
            // 
            this.richTextBox_seriesFormat_Col3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_seriesFormat_Col3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seriesFormat_Col3.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seriesFormat_Col3.Location = new System.Drawing.Point(519, 15);
            this.richTextBox_seriesFormat_Col3.Multiline = false;
            this.richTextBox_seriesFormat_Col3.Name = "richTextBox_seriesFormat_Col3";
            this.richTextBox_seriesFormat_Col3.Size = new System.Drawing.Size(136, 20);
            this.richTextBox_seriesFormat_Col3.TabIndex = 2;
            this.richTextBox_seriesFormat_Col3.Text = "";
            this.toolTip_Help.SetToolTip(this.richTextBox_seriesFormat_Col3, "Fields to be displayed in column 3; Right-click to insert a DB field");
            this.richTextBox_seriesFormat_Col3.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // richTextBox_seriesFormat_Main
            // 
            this.richTextBox_seriesFormat_Main.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_seriesFormat_Main.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seriesFormat_Main.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seriesFormat_Main.Location = new System.Drawing.Point(217, 80);
            this.richTextBox_seriesFormat_Main.Multiline = false;
            this.richTextBox_seriesFormat_Main.Name = "richTextBox_seriesFormat_Main";
            this.richTextBox_seriesFormat_Main.Size = new System.Drawing.Size(438, 20);
            this.richTextBox_seriesFormat_Main.TabIndex = 2;
            this.richTextBox_seriesFormat_Main.Text = "";
            this.toolTip_Help.SetToolTip(this.richTextBox_seriesFormat_Main, "What\'s displayed in the main info pane (usually the summary). Use \\n to introduce" +
                    " a line break");
            this.richTextBox_seriesFormat_Main.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // richTextBox_seriesFormat_Subtitle
            // 
            this.richTextBox_seriesFormat_Subtitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_seriesFormat_Subtitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seriesFormat_Subtitle.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seriesFormat_Subtitle.Location = new System.Drawing.Point(217, 59);
            this.richTextBox_seriesFormat_Subtitle.Multiline = false;
            this.richTextBox_seriesFormat_Subtitle.Name = "richTextBox_seriesFormat_Subtitle";
            this.richTextBox_seriesFormat_Subtitle.Size = new System.Drawing.Size(438, 20);
            this.richTextBox_seriesFormat_Subtitle.TabIndex = 2;
            this.richTextBox_seriesFormat_Subtitle.Text = "";
            this.toolTip_Help.SetToolTip(this.richTextBox_seriesFormat_Subtitle, "What\'s displayed in the subtitle info pane (usually the genre). Use \\n to introdu" +
                    "ce a line break");
            this.richTextBox_seriesFormat_Subtitle.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // richTextBox_seriesFormat_Col2
            // 
            this.richTextBox_seriesFormat_Col2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seriesFormat_Col2.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seriesFormat_Col2.Location = new System.Drawing.Point(368, 15);
            this.richTextBox_seriesFormat_Col2.Multiline = false;
            this.richTextBox_seriesFormat_Col2.Name = "richTextBox_seriesFormat_Col2";
            this.richTextBox_seriesFormat_Col2.Size = new System.Drawing.Size(151, 20);
            this.richTextBox_seriesFormat_Col2.TabIndex = 2;
            this.richTextBox_seriesFormat_Col2.Text = "";
            this.toolTip_Help.SetToolTip(this.richTextBox_seriesFormat_Col2, "Fields to be displayed in column 2; Right-click to insert a DB field");
            this.richTextBox_seriesFormat_Col2.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // richTextBox_seriesFormat_Title
            // 
            this.richTextBox_seriesFormat_Title.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_seriesFormat_Title.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seriesFormat_Title.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seriesFormat_Title.Location = new System.Drawing.Point(217, 38);
            this.richTextBox_seriesFormat_Title.Multiline = false;
            this.richTextBox_seriesFormat_Title.Name = "richTextBox_seriesFormat_Title";
            this.richTextBox_seriesFormat_Title.Size = new System.Drawing.Size(438, 20);
            this.richTextBox_seriesFormat_Title.TabIndex = 2;
            this.richTextBox_seriesFormat_Title.Text = "";
            this.toolTip_Help.SetToolTip(this.richTextBox_seriesFormat_Title, "What\'s displayed in the Title info pane (usually the title!). Use \\n to introduce" +
                    " a line break");
            this.richTextBox_seriesFormat_Title.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(146, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Series Main InfoPane Format:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(158, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Series Subtitle InfoPane Format:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Series Listing Format:";
            // 
            // richTextBox_seriesFormat_Col1
            // 
            this.richTextBox_seriesFormat_Col1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_seriesFormat_Col1.ContextMenuStrip = this.contextMenuStrip_InsertFields;
            this.richTextBox_seriesFormat_Col1.Location = new System.Drawing.Point(217, 15);
            this.richTextBox_seriesFormat_Col1.Multiline = false;
            this.richTextBox_seriesFormat_Col1.Name = "richTextBox_seriesFormat_Col1";
            this.richTextBox_seriesFormat_Col1.Size = new System.Drawing.Size(151, 20);
            this.richTextBox_seriesFormat_Col1.TabIndex = 2;
            this.richTextBox_seriesFormat_Col1.Text = "";
            this.toolTip_Help.SetToolTip(this.richTextBox_seriesFormat_Col1, "Fields to be displayed in column 1; Right-click to insert a DB field");
            this.richTextBox_seriesFormat_Col1.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(143, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Series Title InfoPane Format:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label28);
            this.groupBox1.Controls.Add(this.comboLanguage);
            this.groupBox1.Controls.Add(this.checkBox_RandBanner);
            this.groupBox1.Controls.Add(this.textBox_PluginHomeName);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.checkBox_AutoHeight);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numericUpDown_AutoOnlineDataRefresh);
            this.groupBox1.Controls.Add(this.checkBox_AutoOnlineDataRefresh);
            this.groupBox1.Location = new System.Drawing.Point(4, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(661, 105);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General Settings";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(382, 82);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(58, 13);
            this.label28.TabIndex = 8;
            this.label28.Text = "Language:";
            // 
            // comboLanguage
            // 
            this.comboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLanguage.FormattingEnabled = true;
            this.comboLanguage.Location = new System.Drawing.Point(448, 79);
            this.comboLanguage.Name = "comboLanguage";
            this.comboLanguage.Size = new System.Drawing.Size(119, 21);
            this.comboLanguage.TabIndex = 7;
            this.comboLanguage.SelectedIndexChanged += new System.EventHandler(this.comboLanguage_SelectedIndexChanged);
            // 
            // checkBox_RandBanner
            // 
            this.checkBox_RandBanner.AutoSize = true;
            this.checkBox_RandBanner.Location = new System.Drawing.Point(5, 59);
            this.checkBox_RandBanner.Name = "checkBox_RandBanner";
            this.checkBox_RandBanner.Size = new System.Drawing.Size(140, 17);
            this.checkBox_RandBanner.TabIndex = 6;
            this.checkBox_RandBanner.Text = "Display Random Banner";
            this.checkBox_RandBanner.UseVisualStyleBackColor = true;
            this.checkBox_RandBanner.CheckedChanged += new System.EventHandler(this.checkBox_RandBanner_CheckedChanged);
            // 
            // textBox_PluginHomeName
            // 
            this.textBox_PluginHomeName.Location = new System.Drawing.Point(149, 79);
            this.textBox_PluginHomeName.Name = "textBox_PluginHomeName";
            this.textBox_PluginHomeName.Size = new System.Drawing.Size(217, 20);
            this.textBox_PluginHomeName.TabIndex = 5;
            this.textBox_PluginHomeName.TextChanged += new System.EventHandler(this.textBox_PluginHomeName_TextChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(2, 82);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(141, 13);
            this.label16.TabIndex = 4;
            this.label16.Text = "Name of the plugin in Home:";
            // 
            // checkBox_AutoHeight
            // 
            this.checkBox_AutoHeight.AutoSize = true;
            this.checkBox_AutoHeight.Enabled = false;
            this.checkBox_AutoHeight.Location = new System.Drawing.Point(5, 39);
            this.checkBox_AutoHeight.Name = "checkBox_AutoHeight";
            this.checkBox_AutoHeight.Size = new System.Drawing.Size(517, 17);
            this.checkBox_AutoHeight.TabIndex = 3;
            this.checkBox_AutoHeight.Text = "Automatically resize the height of the InfoPane text elements to accommodate the " +
                "amount of data shown";
            this.checkBox_AutoHeight.UseVisualStyleBackColor = true;
            this.checkBox_AutoHeight.CheckedChanged += new System.EventHandler(this.checkBox_AutoHeight_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(328, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "hour(s)";
            // 
            // numericUpDown_AutoOnlineDataRefresh
            // 
            this.numericUpDown_AutoOnlineDataRefresh.Location = new System.Drawing.Point(284, 18);
            this.numericUpDown_AutoOnlineDataRefresh.Name = "numericUpDown_AutoOnlineDataRefresh";
            this.numericUpDown_AutoOnlineDataRefresh.Size = new System.Drawing.Size(38, 20);
            this.numericUpDown_AutoOnlineDataRefresh.TabIndex = 1;
            this.numericUpDown_AutoOnlineDataRefresh.ValueChanged += new System.EventHandler(this.numericUpDown_AutoOnlineDataRefresh_ValueChanged);
            // 
            // checkBox_AutoOnlineDataRefresh
            // 
            this.checkBox_AutoOnlineDataRefresh.AutoSize = true;
            this.checkBox_AutoOnlineDataRefresh.Location = new System.Drawing.Point(5, 19);
            this.checkBox_AutoOnlineDataRefresh.Name = "checkBox_AutoOnlineDataRefresh";
            this.checkBox_AutoOnlineDataRefresh.Size = new System.Drawing.Size(277, 17);
            this.checkBox_AutoOnlineDataRefresh.TabIndex = 0;
            this.checkBox_AutoOnlineDataRefresh.Text = "Automatically query the server for updated data every";
            this.toolTip_Help.SetToolTip(this.checkBox_AutoOnlineDataRefresh, "When checked, the plugin will automatically ask for a refresh of the online data " +
                    "every x hours");
            this.checkBox_AutoOnlineDataRefresh.UseVisualStyleBackColor = true;
            this.checkBox_AutoOnlineDataRefresh.CheckedChanged += new System.EventHandler(this.checkBox_AutoOnlineDataRefresh_CheckedChanged);
            // 
            // tabpage_Subtitles
            // 
            this.tabpage_Subtitles.Controls.Add(this.groupBox6);
            this.tabpage_Subtitles.Controls.Add(this.groupBox5);
            this.tabpage_Subtitles.Location = new System.Drawing.Point(4, 22);
            this.tabpage_Subtitles.Name = "tabpage_Subtitles";
            this.tabpage_Subtitles.Padding = new System.Windows.Forms.Padding(3);
            this.tabpage_Subtitles.Size = new System.Drawing.Size(684, 743);
            this.tabpage_Subtitles.TabIndex = 6;
            this.tabpage_Subtitles.Text = "Extras";
            this.tabpage_Subtitles.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.textBox_TorrentDetailsRegex);
            this.groupBox6.Controls.Add(this.textBox_TorrentSearchRegex);
            this.groupBox6.Controls.Add(this.textBox_TorrentDetailsUrl);
            this.groupBox6.Controls.Add(this.textBox_TorrentSearchUrl);
            this.groupBox6.Controls.Add(this.comboBox_TorrentPreset);
            this.groupBox6.Controls.Add(this.label24);
            this.groupBox6.Controls.Add(this.label22);
            this.groupBox6.Controls.Add(this.label23);
            this.groupBox6.Controls.Add(this.label21);
            this.groupBox6.Controls.Add(this.label20);
            this.groupBox6.Controls.Add(this.button_uTorrentBrowse);
            this.groupBox6.Controls.Add(this.textBox_uTorrentPath);
            this.groupBox6.Controls.Add(this.label19);
            this.groupBox6.Location = new System.Drawing.Point(7, 162);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(671, 284);
            this.groupBox6.TabIndex = 4;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Torrent Search";
            // 
            // textBox_TorrentDetailsRegex
            // 
            this.textBox_TorrentDetailsRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_TorrentDetailsRegex.Location = new System.Drawing.Point(111, 188);
            this.textBox_TorrentDetailsRegex.Multiline = true;
            this.textBox_TorrentDetailsRegex.Name = "textBox_TorrentDetailsRegex";
            this.textBox_TorrentDetailsRegex.Size = new System.Drawing.Size(554, 51);
            this.textBox_TorrentDetailsRegex.TabIndex = 6;
            this.toolTip_Help.SetToolTip(this.textBox_TorrentDetailsRegex, resources.GetString("textBox_TorrentDetailsRegex.ToolTip"));
            this.textBox_TorrentDetailsRegex.TextChanged += new System.EventHandler(this.textBox_TorrentDetailsRegex_TextChanged);
            // 
            // textBox_TorrentSearchRegex
            // 
            this.textBox_TorrentSearchRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_TorrentSearchRegex.Location = new System.Drawing.Point(111, 103);
            this.textBox_TorrentSearchRegex.Multiline = true;
            this.textBox_TorrentSearchRegex.Name = "textBox_TorrentSearchRegex";
            this.textBox_TorrentSearchRegex.Size = new System.Drawing.Size(554, 51);
            this.textBox_TorrentSearchRegex.TabIndex = 6;
            this.toolTip_Help.SetToolTip(this.textBox_TorrentSearchRegex, resources.GetString("textBox_TorrentSearchRegex.ToolTip"));
            this.textBox_TorrentSearchRegex.TextChanged += new System.EventHandler(this.textBox_TorrentRegex_TextChanged);
            // 
            // textBox_TorrentDetailsUrl
            // 
            this.textBox_TorrentDetailsUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_TorrentDetailsUrl.Location = new System.Drawing.Point(111, 162);
            this.textBox_TorrentDetailsUrl.Name = "textBox_TorrentDetailsUrl";
            this.textBox_TorrentDetailsUrl.Size = new System.Drawing.Size(554, 20);
            this.textBox_TorrentDetailsUrl.TabIndex = 6;
            this.toolTip_Help.SetToolTip(this.textBox_TorrentDetailsUrl, "Url of the website showing details on a specific torrent.\r\nThis url will replace " +
                    "the $id$ string with the <id> extracted from the search results");
            this.textBox_TorrentDetailsUrl.TextChanged += new System.EventHandler(this.textBox_TorrentDetailsUrl_TextChanged);
            // 
            // textBox_TorrentSearchUrl
            // 
            this.textBox_TorrentSearchUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_TorrentSearchUrl.Location = new System.Drawing.Point(111, 77);
            this.textBox_TorrentSearchUrl.Name = "textBox_TorrentSearchUrl";
            this.textBox_TorrentSearchUrl.Size = new System.Drawing.Size(554, 20);
            this.textBox_TorrentSearchUrl.TabIndex = 6;
            this.toolTip_Help.SetToolTip(this.textBox_TorrentSearchUrl, "Url of the website listing the search results.\r\nThe search string will replace th" +
                    "e $search$ string in the specified url.");
            this.textBox_TorrentSearchUrl.TextChanged += new System.EventHandler(this.textBox_TorrentUrl_TextChanged);
            // 
            // comboBox_TorrentPreset
            // 
            this.comboBox_TorrentPreset.FormattingEnabled = true;
            this.comboBox_TorrentPreset.Location = new System.Drawing.Point(111, 49);
            this.comboBox_TorrentPreset.Name = "comboBox_TorrentPreset";
            this.comboBox_TorrentPreset.Size = new System.Drawing.Size(337, 21);
            this.comboBox_TorrentPreset.Sorted = true;
            this.comboBox_TorrentPreset.TabIndex = 5;
            this.comboBox_TorrentPreset.SelectedIndexChanged += new System.EventHandler(this.comboBox_TorrentPreset_SelectedIndexChanged);
            this.comboBox_TorrentPreset.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBox_TorrentPreset_KeyDown);
            // 
            // label24
            // 
            this.label24.Location = new System.Drawing.Point(9, 191);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(100, 24);
            this.label24.TabIndex = 4;
            this.label24.Text = "Details Regex:";
            this.label24.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label22
            // 
            this.label22.Location = new System.Drawing.Point(9, 106);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(100, 24);
            this.label22.TabIndex = 4;
            this.label22.Text = "Search Regex:";
            this.label22.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label23
            // 
            this.label23.Location = new System.Drawing.Point(6, 165);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(100, 21);
            this.label23.TabIndex = 4;
            this.label23.Text = "Details Url:";
            this.label23.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label21
            // 
            this.label21.Location = new System.Drawing.Point(9, 80);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(100, 21);
            this.label21.TabIndex = 4;
            this.label21.Text = "Search Url:";
            this.label21.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(11, 52);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(100, 21);
            this.label20.TabIndex = 4;
            this.label20.Text = "Websearch preset:";
            this.label20.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // button_uTorrentBrowse
            // 
            this.button_uTorrentBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_uTorrentBrowse.Location = new System.Drawing.Point(639, 15);
            this.button_uTorrentBrowse.Name = "button_uTorrentBrowse";
            this.button_uTorrentBrowse.Size = new System.Drawing.Size(26, 23);
            this.button_uTorrentBrowse.TabIndex = 3;
            this.button_uTorrentBrowse.Text = "...";
            this.button_uTorrentBrowse.UseVisualStyleBackColor = true;
            this.button_uTorrentBrowse.Click += new System.EventHandler(this.button_uTorrentBrowse_Click);
            // 
            // textBox_uTorrentPath
            // 
            this.textBox_uTorrentPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_uTorrentPath.Location = new System.Drawing.Point(111, 17);
            this.textBox_uTorrentPath.Name = "textBox_uTorrentPath";
            this.textBox_uTorrentPath.Size = new System.Drawing.Size(522, 20);
            this.textBox_uTorrentPath.TabIndex = 1;
            this.toolTip_Help.SetToolTip(this.textBox_uTorrentPath, "Enter path to your uTorrent exe");
            this.textBox_uTorrentPath.TextChanged += new System.EventHandler(this.textBox_uTorrentPath_TextChanged);
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(11, 20);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(98, 18);
            this.label19.TabIndex = 0;
            this.label19.Text = "uTorrent Path:";
            this.label19.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.tabControl1);
            this.groupBox5.Location = new System.Drawing.Point(6, 6);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(672, 149);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Subtitles";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage_Forom);
            this.tabControl1.Location = new System.Drawing.Point(6, 19);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(660, 124);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage_Forom
            // 
            this.tabPage_Forom.Controls.Add(this.label17);
            this.tabPage_Forom.Controls.Add(this.label18);
            this.tabPage_Forom.Controls.Add(this.textBox_foromBaseURL);
            this.tabPage_Forom.Controls.Add(this.textBox_foromID);
            this.tabPage_Forom.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Forom.Name = "tabPage_Forom";
            this.tabPage_Forom.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Forom.Size = new System.Drawing.Size(652, 98);
            this.tabPage_Forom.TabIndex = 0;
            this.tabPage_Forom.Text = "Forom";
            this.tabPage_Forom.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 9);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(90, 13);
            this.label17.TabIndex = 1;
            this.label17.Text = "Forom base URL:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 36);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(50, 13);
            this.label18.TabIndex = 1;
            this.label18.Text = "Forom id:";
            // 
            // textBox_foromBaseURL
            // 
            this.textBox_foromBaseURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_foromBaseURL.Location = new System.Drawing.Point(102, 6);
            this.textBox_foromBaseURL.Name = "textBox_foromBaseURL";
            this.textBox_foromBaseURL.Size = new System.Drawing.Size(544, 20);
            this.textBox_foromBaseURL.TabIndex = 0;
            this.toolTip_Help.SetToolTip(this.textBox_foromBaseURL, "Base URL for the Forom website. Normally you don\'t need to change that; if you do" +
                    ", you\'re on your own :)");
            this.textBox_foromBaseURL.TextChanged += new System.EventHandler(this.textBox_foromBaseURL_TextChanged);
            // 
            // textBox_foromID
            // 
            this.textBox_foromID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_foromID.Location = new System.Drawing.Point(102, 33);
            this.textBox_foromID.Name = "textBox_foromID";
            this.textBox_foromID.Size = new System.Drawing.Size(544, 20);
            this.textBox_foromID.TabIndex = 0;
            this.toolTip_Help.SetToolTip(this.textBox_foromID, resources.GetString("textBox_foromID.ToolTip"));
            this.textBox_foromID.TextChanged += new System.EventHandler(this.textBox_foromID_TextChanged);
            // 
            // listBox_Log
            // 
            this.listBox_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox_Log.FormattingEnabled = true;
            this.listBox_Log.HorizontalScrollbar = true;
            this.listBox_Log.Location = new System.Drawing.Point(0, 0);
            this.listBox_Log.Name = "listBox_Log";
            this.listBox_Log.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listBox_Log.Size = new System.Drawing.Size(150, 43);
            this.listBox_Log.TabIndex = 5;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.button1);
            this.splitContainer1.Panel1.Controls.Add(this.tabControl_Details);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listBox_Log);
            this.splitContainer1.Panel2Collapsed = true;
            this.splitContainer1.Size = new System.Drawing.Size(692, 779);
            this.splitContainer1.SplitterDistance = 382;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 65;
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button1.Location = new System.Drawing.Point(0, 769);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(692, 10);
            this.button1.TabIndex = 65;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 779);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(700, 450);
            this.Name = "ConfigurationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MP-TV Series Configuration v";
            this.tabControl_Details.ResumeLayout(false);
            this.tabPage_Details.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.contextMenuStrip_DetailsTree.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.detailsPropertyBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Series)).EndInit();
            this.tabPage_Import.ResumeLayout(false);
            this.splitContainer_SettingsOutput.Panel1.ResumeLayout(false);
            this.splitContainer_SettingsOutput.Panel2.ResumeLayout(false);
            this.splitContainer_SettingsOutput.Panel2.PerformLayout();
            this.splitContainer_SettingsOutput.ResumeLayout(false);
            this.splitContainerImportSettings.Panel1.ResumeLayout(false);
            this.splitContainerImportSettings.Panel2.ResumeLayout(false);
            this.splitContainerImportSettings.ResumeLayout(false);
            this.panel_StringReplacements.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Replace)).EndInit();
            this.panel_OnlineData.ResumeLayout(false);
            this.panel_OnlineData.PerformLayout();
            this.panel_ParsingTest.ResumeLayout(false);
            this.panel_ImportPathes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ImportPathes)).EndInit();
            this.panel_Expressions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Expressions)).EndInit();
            this.tab_view.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.tabPage_MP_DisplayControl.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_AutoOnlineDataRefresh)).EndInit();
            this.tabpage_Subtitles.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage_Forom.ResumeLayout(false);
            this.tabPage_Forom.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColumnHeader columnHeader_Series;
        private System.Windows.Forms.ColumnHeader columnHeader_Title;
        private System.Windows.Forms.ColumnHeader columnHeader_Season;
        private System.Windows.Forms.ColumnHeader columnHeader_Episode;
        private System.Windows.Forms.ColumnHeader columnHeader_OriginallyAired;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
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
        private System.Windows.Forms.Panel panel_ParsingTest;
        private System.Windows.Forms.Panel panel_OnlineData;
        private System.Windows.Forms.CheckBox checkBox_FullSeriesRetrieval;
        private System.Windows.Forms.CheckBox checkBox_AutoChooseSeries;
        private System.Windows.Forms.CheckBox checkBox_LocalDataOverride;
        private System.Windows.Forms.CheckBox checkBox_OnlineSearch;
        private System.Windows.Forms.Button button_Start;
        private System.Windows.Forms.ProgressBar progressBar_Parsing;
        private System.Windows.Forms.SplitContainer splitContainer_SettingsOutput;
        private System.Windows.Forms.ListBox listBox_Log;
        private System.Windows.Forms.ComboBox comboBox_BannerSelection;
        private System.Windows.Forms.TabPage tabPage_MP_DisplayControl;
        private System.Windows.Forms.CheckBox checkBox_Episode_OnlyShowLocalFiles;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox_Episode_HideUnwatchedSummary;
        private System.Windows.Forms.CheckBox checkBox_AutoOnlineDataRefresh;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown_AutoOnlineDataRefresh;
        private System.Windows.Forms.ToolTip toolTip_Help;
        private System.Windows.Forms.Panel panel_StringReplacements;
        private System.Windows.Forms.DataGridView dataGridView_Replace;
        private System.Windows.Forms.Button button_TestReparse;
        private System.Windows.Forms.ListView listView_ParsingResults;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox richTextBox_seriesFormat_Col1;
        private System.Windows.Forms.RichTextBox richTextBox_seriesFormat_Col3;
        private System.Windows.Forms.RichTextBox richTextBox_seriesFormat_Col2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_InsertFields;
        private System.Windows.Forms.RichTextBox richTextBox_seriesFormat_Title;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox richTextBox_seriesFormat_Main;
        private System.Windows.Forms.RichTextBox richTextBox_seriesFormat_Subtitle;
        private System.Windows.Forms.Label label6;
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
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox checkBox_AutoHeight;
        private System.Windows.Forms.ComboBox comboBox_seriesFormat;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox comboBox_seasonFormat;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.RichTextBox richTextBox_seasonFormat_Col3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.RichTextBox richTextBox_seasonFormat_Main;
        private System.Windows.Forms.RichTextBox richTextBox_seasonFormat_Col1;
        private System.Windows.Forms.RichTextBox richTextBox_seasonFormat_Subtitle;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.RichTextBox richTextBox_seasonFormat_Col2;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.RichTextBox richTextBox_seasonFormat_Title;
        private System.Windows.Forms.TextBox textBox_PluginHomeName;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TabPage tabpage_Subtitles;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBox_DontClearMissingLocalFiles;
        private System.Windows.Forms.CheckBox checkBox_ShowHidden;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_DetailsTree;
        private System.Windows.Forms.ToolStripMenuItem hideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getSubtitlesToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage_Forom;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox textBox_foromBaseURL;
        private System.Windows.Forms.TextBox textBox_foromID;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Button button_uTorrentBrowse;
        private System.Windows.Forms.TextBox textBox_uTorrentPath;
        private System.Windows.Forms.TextBox textBox_TorrentSearchRegex;
        private System.Windows.Forms.TextBox textBox_TorrentSearchUrl;
        private System.Windows.Forms.ComboBox comboBox_TorrentPreset;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.ToolStripMenuItem torrentThToolStripMenuItem;
        private System.Windows.Forms.TextBox textBox_TorrentDetailsUrl;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox textBox_TorrentDetailsRegex;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.CheckBox checkBox_RandBanner;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.ListBox lstLogos;
        private System.Windows.Forms.Button addLogo;
        private System.Windows.Forms.Button btnrmvLogo;
        private System.Windows.Forms.Button btnLogoEdit;
        private System.Windows.Forms.Button btnLogoDown;
        private System.Windows.Forms.Button btnlogoUp;
        private System.Windows.Forms.CheckBox checkBox_doFolderWatch;
        private System.Windows.Forms.TabPage tab_view;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Button btnAddView;
        private System.Windows.Forms.ListBox _availViews;
        private System.Windows.Forms.Button btnViewUp;
        private System.Windows.Forms.Button btnViewDown;
        private System.Windows.Forms.Button btnRemoveView;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.TextBox view_selectedName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.ListBox view_selStepsList;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.Label viewStepGroupLbl;
        private System.Windows.Forms.TextBox viewStepGroupByTextBox;
        private System.Windows.Forms.ComboBox viewStepType;
        private System.Windows.Forms.ComboBox comboBox7;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.ComboBox comboBox8;
        private System.Windows.Forms.RichTextBox richTextBox4;
        private System.Windows.Forms.ComboBox comboBox9;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.ComboBox comboBox10;
        private System.Windows.Forms.RichTextBox richTextBox5;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.ComboBox comboBox4;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.ComboBox comboBox5;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.ComboBox comboBox6;
        private System.Windows.Forms.RichTextBox richTextBox3;
        private System.Windows.Forms.ComboBox _23_link;
        private System.Windows.Forms.TextBox cond3_cond;
        private System.Windows.Forms.ComboBox cond3_type;
        private System.Windows.Forms.RichTextBox cond3_what;
        private System.Windows.Forms.ComboBox _12_link;
        private System.Windows.Forms.TextBox cond2_cond;
        private System.Windows.Forms.ComboBox cond2_type;
        private System.Windows.Forms.RichTextBox cond2_what;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox cond1_cond;
        private System.Windows.Forms.ComboBox cond1_type;
        private System.Windows.Forms.RichTextBox cond1_what;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.ComboBox comboLanguage;
        private System.Windows.Forms.LinkLabel cleanBanners;
    }
}