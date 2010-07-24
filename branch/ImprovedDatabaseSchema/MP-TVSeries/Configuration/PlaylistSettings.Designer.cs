namespace WindowPlugins.GUITVSeries.Configuration {
	partial class PlaylistSettings {
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxBrowse = new System.Windows.Forms.Button();
            this.textBoxPlaylistFolder = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dbOptionCheckBox1 = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.dbOptCheckBoxAutoShuffle = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.dbOptCheckBoxAutoPlayList = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.dbOptCheckBoxRepeatPlaylist = new WindowPlugins.GUITVSeries.Configuration.DBOptionCheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.dbOptionCheckBox1);
            this.groupBox1.Controls.Add(this.dbOptCheckBoxAutoShuffle);
            this.groupBox1.Controls.Add(this.dbOptCheckBoxAutoPlayList);
            this.groupBox1.Controls.Add(this.dbOptCheckBoxRepeatPlaylist);
            this.groupBox1.Controls.Add(this.textBoxBrowse);
            this.groupBox1.Controls.Add(this.textBoxPlaylistFolder);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(4, 0);
            this.groupBox1.MinimumSize = new System.Drawing.Size(537, 192);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(537, 192);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Playlist View";
            // 
            // textBoxBrowse
            // 
            this.textBoxBrowse.Location = new System.Drawing.Point(398, 28);
            this.textBoxBrowse.Name = "textBoxBrowse";
            this.textBoxBrowse.Size = new System.Drawing.Size(75, 23);
            this.textBoxBrowse.TabIndex = 2;
            this.textBoxBrowse.Text = "&Browse...";
            this.textBoxBrowse.UseVisualStyleBackColor = true;
            this.textBoxBrowse.Click += new System.EventHandler(this.textBoxBrowse_Click);
            // 
            // textBoxPlaylistFolder
            // 
            this.textBoxPlaylistFolder.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.textBoxPlaylistFolder.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.textBoxPlaylistFolder.Location = new System.Drawing.Point(109, 30);
            this.textBoxPlaylistFolder.Name = "textBoxPlaylistFolder";
            this.textBoxPlaylistFolder.Size = new System.Drawing.Size(283, 20);
            this.textBoxPlaylistFolder.TabIndex = 1;
            this.textBoxPlaylistFolder.TextChanged += new System.EventHandler(this.textBoxPlaylistFolder_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Playlist &Folder:";
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(109, 145);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(364, 3);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            // 
            // dbOptionCheckBox1
            // 
            this.dbOptionCheckBox1.AutoSize = true;
            this.dbOptionCheckBox1.Location = new System.Drawing.Point(109, 161);
            this.dbOptionCheckBox1.Name = "dbOptionCheckBox1";
            this.dbOptionCheckBox1.Option = "PlaylistUnwatchedOnly";
            this.dbOptionCheckBox1.Size = new System.Drawing.Size(344, 17);
            this.dbOptionCheckBox1.TabIndex = 6;
            this.dbOptionCheckBox1.Text = "Only Add Un&watched Items to playlist view on Series/Season views";
            this.dbOptionCheckBox1.ToolTip = "";
            this.dbOptionCheckBox1.UseVisualStyleBackColor = true;
            // 
            // dbOptCheckBoxAutoShuffle
            // 
            this.dbOptCheckBoxAutoShuffle.AutoSize = true;
            this.dbOptCheckBoxAutoShuffle.Location = new System.Drawing.Point(109, 114);
            this.dbOptCheckBoxAutoShuffle.Name = "dbOptCheckBoxAutoShuffle";
            this.dbOptCheckBoxAutoShuffle.Option = "PlaylistAutoShuffle";
            this.dbOptCheckBoxAutoShuffle.Size = new System.Drawing.Size(180, 17);
            this.dbOptCheckBoxAutoShuffle.TabIndex = 5;
            this.dbOptCheckBoxAutoShuffle.Text = "Auto &shuffle playlist when loaded";
            this.dbOptCheckBoxAutoShuffle.ToolTip = "";
            this.dbOptCheckBoxAutoShuffle.UseVisualStyleBackColor = true;
            // 
            // dbOptCheckBoxAutoPlayList
            // 
            this.dbOptCheckBoxAutoPlayList.AutoSize = true;
            this.dbOptCheckBoxAutoPlayList.Checked = true;
            this.dbOptCheckBoxAutoPlayList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dbOptCheckBoxAutoPlayList.Location = new System.Drawing.Point(109, 90);
            this.dbOptCheckBoxAutoPlayList.Name = "dbOptCheckBoxAutoPlayList";
            this.dbOptCheckBoxAutoPlayList.Option = "PlaylistAutoPlay";
            this.dbOptCheckBoxAutoPlayList.Size = new System.Drawing.Size(207, 17);
            this.dbOptCheckBoxAutoPlayList.TabIndex = 4;
            this.dbOptCheckBoxAutoPlayList.Text = "A&uto play playlist when loaded from file";
            this.dbOptCheckBoxAutoPlayList.ToolTip = "";
            this.dbOptCheckBoxAutoPlayList.UseVisualStyleBackColor = true;
            // 
            // dbOptCheckBoxRepeatPlaylist
            // 
            this.dbOptCheckBoxRepeatPlaylist.AutoSize = true;
            this.dbOptCheckBoxRepeatPlaylist.Location = new System.Drawing.Point(109, 66);
            this.dbOptCheckBoxRepeatPlaylist.Name = "dbOptCheckBoxRepeatPlaylist";
            this.dbOptCheckBoxRepeatPlaylist.Option = "RepeatPlaylist";
            this.dbOptCheckBoxRepeatPlaylist.Size = new System.Drawing.Size(180, 17);
            this.dbOptCheckBoxRepeatPlaylist.TabIndex = 3;
            this.dbOptCheckBoxRepeatPlaylist.Text = "Repeat/&Loop episodes in playlist";
            this.dbOptCheckBoxRepeatPlaylist.ToolTip = "";
            this.dbOptCheckBoxRepeatPlaylist.UseVisualStyleBackColor = true;
            // 
            // PlaylistSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "PlaylistSettings";
            this.Size = new System.Drawing.Size(544, 202);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button textBoxBrowse;
		private System.Windows.Forms.TextBox textBoxPlaylistFolder;
		private System.Windows.Forms.Label label1;
		private DBOptionCheckBox dbOptCheckBoxRepeatPlaylist;
		private DBOptionCheckBox dbOptCheckBoxAutoPlayList;
		private DBOptionCheckBox dbOptCheckBoxAutoShuffle;
        private System.Windows.Forms.GroupBox groupBox2;
        private DBOptionCheckBox dbOptionCheckBox1;
	}
}
