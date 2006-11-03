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
            this.columnHeader_Series = new System.Windows.Forms.ColumnHeader();
            this.columnHeader_Title = new System.Windows.Forms.ColumnHeader();
            this.columnHeader_Season = new System.Windows.Forms.ColumnHeader();
            this.columnHeader_Episode = new System.Windows.Forms.ColumnHeader();
            this.columnHeader_OriginallyAired = new System.Windows.Forms.ColumnHeader();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabControl_Details = new System.Windows.Forms.TabControl();
            this.tabPage_Import = new System.Windows.Forms.TabPage();
            this.splitContainer_SettingsOutput = new System.Windows.Forms.SplitContainer();
            this.splitContainerImportSettings = new System.Windows.Forms.SplitContainer();
            this.treeView_Settings = new System.Windows.Forms.TreeView();
            this.panel_OnlineData = new System.Windows.Forms.Panel();
            this.checkBox_OnlineSearch = new System.Windows.Forms.CheckBox();
            this.checkBox_LocalDataOverride = new System.Windows.Forms.CheckBox();
            this.checkBox_AutoChooseSeries = new System.Windows.Forms.CheckBox();
            this.checkBox_FullSeriesRetrieval = new System.Windows.Forms.CheckBox();
            this.panel_ParsingTest = new System.Windows.Forms.Panel();
            this.splitContainer_parsingtest = new System.Windows.Forms.SplitContainer();
            this.button_TestReparse = new System.Windows.Forms.Button();
            this.listView_ParsingResults = new System.Windows.Forms.ListView();
            this.listBox_Results = new System.Windows.Forms.ListBox();
            this.panel_ImportPathes = new System.Windows.Forms.Panel();
            this.dataGridView_ImportPathes = new System.Windows.Forms.DataGridView();
            this.panel_Expressions = new System.Windows.Forms.Panel();
            this.dataGridView_Expressions = new System.Windows.Forms.DataGridView();
            this.button_MoveExpUp = new System.Windows.Forms.Button();
            this.button_MoveExpDown = new System.Windows.Forms.Button();
            this.listBox_Log = new System.Windows.Forms.ListBox();
            this.progressBar_Parsing = new System.Windows.Forms.ProgressBar();
            this.button_Start = new System.Windows.Forms.Button();
            this.tabPage_Details = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.treeView_Library = new System.Windows.Forms.TreeView();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.detailsPropertyBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.pictureBox_Series = new System.Windows.Forms.PictureBox();
            this.tabControl_Details.SuspendLayout();
            this.tabPage_Import.SuspendLayout();
            this.splitContainer_SettingsOutput.Panel1.SuspendLayout();
            this.splitContainer_SettingsOutput.Panel2.SuspendLayout();
            this.splitContainer_SettingsOutput.SuspendLayout();
            this.splitContainerImportSettings.Panel1.SuspendLayout();
            this.splitContainerImportSettings.Panel2.SuspendLayout();
            this.splitContainerImportSettings.SuspendLayout();
            this.panel_OnlineData.SuspendLayout();
            this.panel_ParsingTest.SuspendLayout();
            this.splitContainer_parsingtest.Panel1.SuspendLayout();
            this.splitContainer_parsingtest.Panel2.SuspendLayout();
            this.splitContainer_parsingtest.SuspendLayout();
            this.panel_ImportPathes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ImportPathes)).BeginInit();
            this.panel_Expressions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Expressions)).BeginInit();
            this.tabPage_Details.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.detailsPropertyBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Series)).BeginInit();
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
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // tabControl_Details
            // 
            this.tabControl_Details.Controls.Add(this.tabPage_Import);
            this.tabControl_Details.Controls.Add(this.tabPage_Details);
            this.tabControl_Details.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_Details.Location = new System.Drawing.Point(0, 0);
            this.tabControl_Details.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl_Details.Name = "tabControl_Details";
            this.tabControl_Details.SelectedIndex = 0;
            this.tabControl_Details.Size = new System.Drawing.Size(813, 505);
            this.tabControl_Details.TabIndex = 64;
            // 
            // tabPage_Import
            // 
            this.tabPage_Import.AutoScroll = true;
            this.tabPage_Import.BackColor = System.Drawing.Color.Transparent;
            this.tabPage_Import.Controls.Add(this.splitContainer_SettingsOutput);
            this.tabPage_Import.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Import.Name = "tabPage_Import";
            this.tabPage_Import.Size = new System.Drawing.Size(805, 479);
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
            this.splitContainer_SettingsOutput.Panel2.Controls.Add(this.listBox_Log);
            this.splitContainer_SettingsOutput.Panel2.Controls.Add(this.progressBar_Parsing);
            this.splitContainer_SettingsOutput.Panel2.Controls.Add(this.button_Start);
            this.splitContainer_SettingsOutput.Size = new System.Drawing.Size(805, 479);
            this.splitContainer_SettingsOutput.SplitterDistance = 308;
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
            this.splitContainerImportSettings.Panel2.Controls.Add(this.panel_OnlineData);
            this.splitContainerImportSettings.Panel2.Controls.Add(this.panel_ParsingTest);
            this.splitContainerImportSettings.Panel2.Controls.Add(this.panel_ImportPathes);
            this.splitContainerImportSettings.Panel2.Controls.Add(this.panel_Expressions);
            this.splitContainerImportSettings.Size = new System.Drawing.Size(805, 308);
            this.splitContainerImportSettings.SplitterDistance = 113;
            this.splitContainerImportSettings.TabIndex = 156;
            // 
            // treeView_Settings
            // 
            this.treeView_Settings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_Settings.Location = new System.Drawing.Point(0, 0);
            this.treeView_Settings.Name = "treeView_Settings";
            this.treeView_Settings.Size = new System.Drawing.Size(113, 308);
            this.treeView_Settings.TabIndex = 153;
            this.treeView_Settings.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_Settings_AfterSelect);
            // 
            // panel_OnlineData
            // 
            this.panel_OnlineData.Controls.Add(this.checkBox_OnlineSearch);
            this.panel_OnlineData.Controls.Add(this.checkBox_LocalDataOverride);
            this.panel_OnlineData.Controls.Add(this.checkBox_AutoChooseSeries);
            this.panel_OnlineData.Controls.Add(this.checkBox_FullSeriesRetrieval);
            this.panel_OnlineData.Location = new System.Drawing.Point(177, 6);
            this.panel_OnlineData.Name = "panel_OnlineData";
            this.panel_OnlineData.Size = new System.Drawing.Size(503, 114);
            this.panel_OnlineData.TabIndex = 157;
            this.panel_OnlineData.Tag = "Online Data Sync";
            // 
            // checkBox_OnlineSearch
            // 
            this.checkBox_OnlineSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_OnlineSearch.Location = new System.Drawing.Point(4, 7);
            this.checkBox_OnlineSearch.Name = "checkBox_OnlineSearch";
            this.checkBox_OnlineSearch.Size = new System.Drawing.Size(496, 17);
            this.checkBox_OnlineSearch.TabIndex = 5;
            this.checkBox_OnlineSearch.Text = "Enable Online Database data retrieval";
            this.checkBox_OnlineSearch.UseVisualStyleBackColor = true;
            this.checkBox_OnlineSearch.CheckedChanged += new System.EventHandler(this.checkBox_OnlineSearch_CheckedChanged);
            // 
            // checkBox_LocalDataOverride
            // 
            this.checkBox_LocalDataOverride.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_LocalDataOverride.Location = new System.Drawing.Point(4, 78);
            this.checkBox_LocalDataOverride.Name = "checkBox_LocalDataOverride";
            this.checkBox_LocalDataOverride.Size = new System.Drawing.Size(496, 17);
            this.checkBox_LocalDataOverride.TabIndex = 2;
            this.checkBox_LocalDataOverride.Text = "Online data should override data from file parsing (series name, episode name)";
            this.checkBox_LocalDataOverride.UseVisualStyleBackColor = true;
            this.checkBox_LocalDataOverride.CheckedChanged += new System.EventHandler(this.checkBox_LocalDataOverride_CheckedChanged);
            // 
            // checkBox_AutoChooseSeries
            // 
            this.checkBox_AutoChooseSeries.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_AutoChooseSeries.Location = new System.Drawing.Point(4, 54);
            this.checkBox_AutoChooseSeries.Name = "checkBox_AutoChooseSeries";
            this.checkBox_AutoChooseSeries.Size = new System.Drawing.Size(496, 17);
            this.checkBox_AutoChooseSeries.TabIndex = 1;
            this.checkBox_AutoChooseSeries.Text = "Auto-Choose series when multiple entries are returned from the Online Database";
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
            this.checkBox_FullSeriesRetrieval.Size = new System.Drawing.Size(496, 17);
            this.checkBox_FullSeriesRetrieval.TabIndex = 0;
            this.checkBox_FullSeriesRetrieval.Text = "Retrieve episode information for the whole series (instead of only for the availa" +
                "ble parsed episodes)";
            this.checkBox_FullSeriesRetrieval.UseVisualStyleBackColor = true;
            this.checkBox_FullSeriesRetrieval.CheckedChanged += new System.EventHandler(this.checkBox_FullSeriesRetrieval_CheckedChanged);
            // 
            // panel_ParsingTest
            // 
            this.panel_ParsingTest.Controls.Add(this.splitContainer_parsingtest);
            this.panel_ParsingTest.Location = new System.Drawing.Point(2, 167);
            this.panel_ParsingTest.Name = "panel_ParsingTest";
            this.panel_ParsingTest.Size = new System.Drawing.Size(157, 113);
            this.panel_ParsingTest.TabIndex = 156;
            this.panel_ParsingTest.Tag = "Parsing Test";
            // 
            // splitContainer_parsingtest
            // 
            this.splitContainer_parsingtest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_parsingtest.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_parsingtest.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer_parsingtest.Name = "splitContainer_parsingtest";
            this.splitContainer_parsingtest.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_parsingtest.Panel1
            // 
            this.splitContainer_parsingtest.Panel1.Controls.Add(this.button_TestReparse);
            this.splitContainer_parsingtest.Panel1.Controls.Add(this.listView_ParsingResults);
            // 
            // splitContainer_parsingtest.Panel2
            // 
            this.splitContainer_parsingtest.Panel2.Controls.Add(this.listBox_Results);
            this.splitContainer_parsingtest.Size = new System.Drawing.Size(157, 113);
            this.splitContainer_parsingtest.SplitterDistance = 78;
            this.splitContainer_parsingtest.TabIndex = 0;
            // 
            // button_TestReparse
            // 
            this.button_TestReparse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_TestReparse.Image = ((System.Drawing.Image)(resources.GetObject("button_TestReparse.Image")));
            this.button_TestReparse.Location = new System.Drawing.Point(126, 3);
            this.button_TestReparse.Name = "button_TestReparse";
            this.button_TestReparse.Size = new System.Drawing.Size(28, 31);
            this.button_TestReparse.TabIndex = 153;
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
            this.listView_ParsingResults.Size = new System.Drawing.Size(123, 76);
            this.listView_ParsingResults.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView_ParsingResults.TabIndex = 49;
            this.listView_ParsingResults.UseCompatibleStateImageBehavior = false;
            this.listView_ParsingResults.View = System.Windows.Forms.View.Details;
            // 
            // listBox_Results
            // 
            this.listBox_Results.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox_Results.FormattingEnabled = true;
            this.listBox_Results.Location = new System.Drawing.Point(0, 0);
            this.listBox_Results.Name = "listBox_Results";
            this.listBox_Results.Size = new System.Drawing.Size(157, 30);
            this.listBox_Results.TabIndex = 50;
            // 
            // panel_ImportPathes
            // 
            this.panel_ImportPathes.Controls.Add(this.dataGridView_ImportPathes);
            this.panel_ImportPathes.Location = new System.Drawing.Point(2, 86);
            this.panel_ImportPathes.Name = "panel_ImportPathes";
            this.panel_ImportPathes.Size = new System.Drawing.Size(157, 75);
            this.panel_ImportPathes.TabIndex = 154;
            this.panel_ImportPathes.Tag = "Import Pathes";
            // 
            // dataGridView_ImportPathes
            // 
            this.dataGridView_ImportPathes.AllowUserToResizeColumns = false;
            this.dataGridView_ImportPathes.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_ImportPathes.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView_ImportPathes.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView_ImportPathes.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView_ImportPathes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView_ImportPathes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_ImportPathes.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView_ImportPathes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_ImportPathes.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_ImportPathes.Name = "dataGridView_ImportPathes";
            this.dataGridView_ImportPathes.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dataGridView_ImportPathes.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_ImportPathes.RowsDefaultCellStyle = dataGridViewCellStyle3;
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
            this.panel_Expressions.Tag = "Expressions";
            // 
            // dataGridView_Expressions
            // 
            this.dataGridView_Expressions.AllowUserToResizeColumns = false;
            this.dataGridView_Expressions.AllowUserToResizeRows = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_Expressions.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView_Expressions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_Expressions.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView_Expressions.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView_Expressions.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView_Expressions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Expressions.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridView_Expressions.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_Expressions.MultiSelect = false;
            this.dataGridView_Expressions.Name = "dataGridView_Expressions";
            this.dataGridView_Expressions.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dataGridView_Expressions.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView_Expressions.RowsDefaultCellStyle = dataGridViewCellStyle6;
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
            // listBox_Log
            // 
            this.listBox_Log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox_Log.FormattingEnabled = true;
            this.listBox_Log.Location = new System.Drawing.Point(0, 33);
            this.listBox_Log.Name = "listBox_Log";
            this.listBox_Log.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listBox_Log.Size = new System.Drawing.Size(805, 121);
            this.listBox_Log.TabIndex = 5;
            // 
            // progressBar_Parsing
            // 
            this.progressBar_Parsing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar_Parsing.Location = new System.Drawing.Point(80, 7);
            this.progressBar_Parsing.Name = "progressBar_Parsing";
            this.progressBar_Parsing.Size = new System.Drawing.Size(717, 20);
            this.progressBar_Parsing.TabIndex = 3;
            // 
            // button_Start
            // 
            this.button_Start.Location = new System.Drawing.Point(8, 6);
            this.button_Start.Name = "button_Start";
            this.button_Start.Size = new System.Drawing.Size(66, 23);
            this.button_Start.TabIndex = 4;
            this.button_Start.Text = "Start";
            this.button_Start.UseVisualStyleBackColor = true;
            this.button_Start.Click += new System.EventHandler(this.button_Start_Click);
            // 
            // tabPage_Details
            // 
            this.tabPage_Details.Controls.Add(this.splitContainer2);
            this.tabPage_Details.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Details.Name = "tabPage_Details";
            this.tabPage_Details.Size = new System.Drawing.Size(805, 479);
            this.tabPage_Details.TabIndex = 2;
            this.tabPage_Details.Text = "Details";
            this.tabPage_Details.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.treeView_Library);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer2.Panel2.Controls.Add(this.pictureBox_Series);
            this.splitContainer2.Size = new System.Drawing.Size(805, 479);
            this.splitContainer2.SplitterDistance = 233;
            this.splitContainer2.TabIndex = 0;
            // 
            // treeView_Library
            // 
            this.treeView_Library.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_Library.ForeColor = System.Drawing.SystemColors.WindowText;
            this.treeView_Library.FullRowSelect = true;
            this.treeView_Library.HideSelection = false;
            this.treeView_Library.Location = new System.Drawing.Point(0, 0);
            this.treeView_Library.MinimumSize = new System.Drawing.Size(100, 100);
            this.treeView_Library.Name = "treeView_Library";
            this.treeView_Library.Size = new System.Drawing.Size(233, 479);
            this.treeView_Library.TabIndex = 47;
            this.treeView_Library.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_Library_AfterSelect);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.dataGridView1.DataSource = this.detailsPropertyBindingSource;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 140);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.dataGridView1.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridView1.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.Size = new System.Drawing.Size(568, 339);
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
            this.pictureBox_Series.BackColor = System.Drawing.Color.White;
            this.pictureBox_Series.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBox_Series.ErrorImage = null;
            this.pictureBox_Series.InitialImage = null;
            this.pictureBox_Series.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_Series.Name = "pictureBox_Series";
            this.pictureBox_Series.Size = new System.Drawing.Size(568, 140);
            this.pictureBox_Series.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_Series.TabIndex = 147;
            this.pictureBox_Series.TabStop = false;
            // 
            // ConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(813, 505);
            this.Controls.Add(this.tabControl_Details);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "ConfigurationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "My TV Series Configuration v0.40";
            this.tabControl_Details.ResumeLayout(false);
            this.tabPage_Import.ResumeLayout(false);
            this.splitContainer_SettingsOutput.Panel1.ResumeLayout(false);
            this.splitContainer_SettingsOutput.Panel2.ResumeLayout(false);
            this.splitContainer_SettingsOutput.ResumeLayout(false);
            this.splitContainerImportSettings.Panel1.ResumeLayout(false);
            this.splitContainerImportSettings.Panel2.ResumeLayout(false);
            this.splitContainerImportSettings.ResumeLayout(false);
            this.panel_OnlineData.ResumeLayout(false);
            this.panel_ParsingTest.ResumeLayout(false);
            this.splitContainer_parsingtest.Panel1.ResumeLayout(false);
            this.splitContainer_parsingtest.Panel2.ResumeLayout(false);
            this.splitContainer_parsingtest.ResumeLayout(false);
            this.panel_ImportPathes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ImportPathes)).EndInit();
            this.panel_Expressions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Expressions)).EndInit();
            this.tabPage_Details.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.detailsPropertyBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Series)).EndInit();
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
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
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
        private System.Windows.Forms.ListView listView_ParsingResults;
        private System.Windows.Forms.ListBox listBox_Results;
        private System.Windows.Forms.SplitContainer splitContainer_parsingtest;
        private System.Windows.Forms.Panel panel_OnlineData;
        private System.Windows.Forms.CheckBox checkBox_FullSeriesRetrieval;
        private System.Windows.Forms.CheckBox checkBox_AutoChooseSeries;
        private System.Windows.Forms.CheckBox checkBox_LocalDataOverride;
        private System.Windows.Forms.CheckBox checkBox_OnlineSearch;
        private System.Windows.Forms.Button button_Start;
        private System.Windows.Forms.ProgressBar progressBar_Parsing;
        private System.Windows.Forms.SplitContainer splitContainer_SettingsOutput;
        private System.Windows.Forms.ListBox listBox_Log;
        private System.Windows.Forms.Button button_TestReparse;
    }
}