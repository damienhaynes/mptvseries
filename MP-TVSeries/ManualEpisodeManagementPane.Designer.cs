namespace WindowPlugins.GUITVSeries {
    partial class ManualEpisodeManagementPane {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.fileListView = new System.Windows.Forms.ListView();
            this.filenameColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.playEpisodeButton = new System.Windows.Forms.Button();
            this.addUnlistedButton = new System.Windows.Forms.Button();
            this.refreshProgressBar = new System.Windows.Forms.ProgressBar();
            this.manuallyAddButton = new System.Windows.Forms.Button();
            this.refreshFileListButton = new System.Windows.Forms.Button();
            this.genericTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.progressTimer = new System.Windows.Forms.Timer(this.components);
            this.fileListContextMenu = new System.Windows.Forms.ContextMenu();
            this.addEpisodeMI = new System.Windows.Forms.MenuItem();
            this.playEpisodeMI = new System.Windows.Forms.MenuItem();
            this.partialPathColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // fileListView
            // 
            this.fileListView.AllowColumnReorder = true;
            this.fileListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.filenameColumnHeader,
            this.partialPathColumnHeader});
            this.fileListView.FullRowSelect = true;
            this.fileListView.GridLines = true;
            this.fileListView.HideSelection = false;
            this.fileListView.Location = new System.Drawing.Point(3, 33);
            this.fileListView.MultiSelect = false;
            this.fileListView.Name = "fileListView";
            this.fileListView.Size = new System.Drawing.Size(656, 333);
            this.fileListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.fileListView.TabIndex = 1;
            this.fileListView.UseCompatibleStateImageBehavior = false;
            this.fileListView.View = System.Windows.Forms.View.Details;
            this.fileListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.fileListView_mouseClick);
            this.fileListView.DoubleClick += new System.EventHandler(this.fileListView_DoubleClick);
            // 
            // filenameColumnHeader
            // 
            this.filenameColumnHeader.Text = "Filename";
            this.filenameColumnHeader.Width = 532;
            // 
            // buttonPanel
            // 
            this.buttonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPanel.Controls.Add(this.playEpisodeButton);
            this.buttonPanel.Controls.Add(this.addUnlistedButton);
            this.buttonPanel.Controls.Add(this.refreshProgressBar);
            this.buttonPanel.Controls.Add(this.manuallyAddButton);
            this.buttonPanel.Controls.Add(this.refreshFileListButton);
            this.buttonPanel.Location = new System.Drawing.Point(0, 0);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(659, 31);
            this.buttonPanel.TabIndex = 2;
            // 
            // playEpisodeButton
            // 
            this.playEpisodeButton.FlatAppearance.BorderSize = 0;
            this.playEpisodeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.playEpisodeButton.Image = global::WindowPlugins.GUITVSeries.Properties.Resources.control_play_blue;
            this.playEpisodeButton.Location = new System.Drawing.Point(93, 3);
            this.playEpisodeButton.Name = "playEpisodeButton";
            this.playEpisodeButton.Size = new System.Drawing.Size(24, 24);
            this.playEpisodeButton.TabIndex = 4;
            this.genericTooltip.SetToolTip(this.playEpisodeButton, "Play Episode in Default Player");
            this.playEpisodeButton.UseVisualStyleBackColor = true;
            this.playEpisodeButton.Click += new System.EventHandler(this.playEpisodeButton_Click);
            // 
            // addUnlistedButton
            // 
            this.addUnlistedButton.FlatAppearance.BorderSize = 0;
            this.addUnlistedButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addUnlistedButton.Image = global::WindowPlugins.GUITVSeries.Properties.Resources.television_add_unlisted;
            this.addUnlistedButton.Location = new System.Drawing.Point(63, 3);
            this.addUnlistedButton.Name = "addUnlistedButton";
            this.addUnlistedButton.Size = new System.Drawing.Size(24, 24);
            this.addUnlistedButton.TabIndex = 3;
            this.genericTooltip.SetToolTip(this.addUnlistedButton, "Manually Add Unlisted Episode");
            this.addUnlistedButton.UseVisualStyleBackColor = true;
            this.addUnlistedButton.Click += new System.EventHandler(this.addUnlistedButton_Click);
            // 
            // refreshProgressBar
            // 
            this.refreshProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshProgressBar.Location = new System.Drawing.Point(556, 4);
            this.refreshProgressBar.Name = "refreshProgressBar";
            this.refreshProgressBar.Size = new System.Drawing.Size(100, 23);
            this.refreshProgressBar.TabIndex = 2;
            this.refreshProgressBar.Visible = false;
            // 
            // manuallyAddButton
            // 
            this.manuallyAddButton.FlatAppearance.BorderSize = 0;
            this.manuallyAddButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.manuallyAddButton.Image = global::WindowPlugins.GUITVSeries.Properties.Resources.television_add;
            this.manuallyAddButton.Location = new System.Drawing.Point(33, 3);
            this.manuallyAddButton.Name = "manuallyAddButton";
            this.manuallyAddButton.Size = new System.Drawing.Size(24, 24);
            this.manuallyAddButton.TabIndex = 1;
            this.genericTooltip.SetToolTip(this.manuallyAddButton, "Manually Add Selected Episode");
            this.manuallyAddButton.UseVisualStyleBackColor = true;
            this.manuallyAddButton.Click += new System.EventHandler(this.manuallyAddButton_Click);
            // 
            // refreshFileListButton
            // 
            this.refreshFileListButton.FlatAppearance.BorderSize = 0;
            this.refreshFileListButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshFileListButton.Image = global::WindowPlugins.GUITVSeries.Properties.Resources.folder_explore;
            this.refreshFileListButton.Location = new System.Drawing.Point(3, 3);
            this.refreshFileListButton.Name = "refreshFileListButton";
            this.refreshFileListButton.Size = new System.Drawing.Size(24, 24);
            this.refreshFileListButton.TabIndex = 0;
            this.genericTooltip.SetToolTip(this.refreshFileListButton, "Refresh File List");
            this.refreshFileListButton.UseVisualStyleBackColor = true;
            this.refreshFileListButton.Click += new System.EventHandler(this.refreshFileListButton_Click);
            // 
            // progressTimer
            // 
            this.progressTimer.Tick += new System.EventHandler(this.progressTimer_Tick);
            // 
            // fileListContextMenu
            // 
            this.fileListContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.addEpisodeMI,
            this.playEpisodeMI});
            // 
            // addEpisodeMI
            // 
            this.addEpisodeMI.Index = 0;
            this.addEpisodeMI.Text = "Manually Add Episode";
            this.addEpisodeMI.Click += new System.EventHandler(this.addEpisodeMI_Click);
            // 
            // playEpisodeMI
            // 
            this.playEpisodeMI.Index = 1;
            this.playEpisodeMI.Text = "Play Episode in Default Player";
            this.playEpisodeMI.Click += new System.EventHandler(this.playEpisodeMI_Click);
            // 
            // partialPathColumnHeader
            // 
            this.partialPathColumnHeader.Text = "Path";
            this.partialPathColumnHeader.Width = 275;
            // 
            // ManualEpisodeManagementPane
            // 
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.fileListView);
            this.Name = "ManualEpisodeManagementPane";
            this.Size = new System.Drawing.Size(662, 369);
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView fileListView;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button refreshFileListButton;
        private System.Windows.Forms.ToolTip genericTooltip;
        private System.Windows.Forms.Button manuallyAddButton;
        private System.Windows.Forms.ColumnHeader filenameColumnHeader;
        private System.Windows.Forms.ProgressBar refreshProgressBar;
        private System.Windows.Forms.Timer progressTimer;
        private System.Windows.Forms.Button addUnlistedButton;
        private System.Windows.Forms.Button playEpisodeButton;
        private System.Windows.Forms.ContextMenu fileListContextMenu;
        private System.Windows.Forms.MenuItem addEpisodeMI;
        private System.Windows.Forms.MenuItem playEpisodeMI;
        private System.Windows.Forms.ColumnHeader partialPathColumnHeader;
    }
}
