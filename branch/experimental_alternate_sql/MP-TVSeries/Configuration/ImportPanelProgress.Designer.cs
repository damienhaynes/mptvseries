namespace WindowPlugins.GUITVSeries.Configuration
{
    partial class ImportPanelProgress
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelProgress = new System.Windows.Forms.Panel();
            this.groupBoxLocal = new System.Windows.Forms.GroupBox();
            this.groupBoxMetaData = new System.Windows.Forms.GroupBox();
            this.groupBoxArtwork = new System.Windows.Forms.GroupBox();
            this.labelFilenameProcessingProgress = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.labelMediaInfoProgress = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.labelMatchingSeriesProgress = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.labelRetrievingSeriesMetaDataProgress = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.labelIdentifyEpisodesProgress = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.labelUpdatingEpisodeMetaDataProgress = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.labelRetrievingEpisodeThumbsProgress = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.labelRetrievingFanartProgress = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.labelRetrievingSeriesArtworkProgress = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.panelProgress.SuspendLayout();
            this.groupBoxLocal.SuspendLayout();
            this.groupBoxMetaData.SuspendLayout();
            this.groupBoxArtwork.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelProgress
            // 
            this.panelProgress.AutoScroll = true;
            this.panelProgress.BackColor = System.Drawing.SystemColors.Window;
            this.panelProgress.Controls.Add(this.groupBoxLocal);
            this.panelProgress.Controls.Add(this.groupBoxMetaData);
            this.panelProgress.Controls.Add(this.groupBoxArtwork);
            this.panelProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelProgress.Location = new System.Drawing.Point(0, 0);
            this.panelProgress.Name = "panelProgress";
            this.panelProgress.Size = new System.Drawing.Size(545, 406);
            this.panelProgress.TabIndex = 31;
            // 
            // groupBoxLocal
            // 
            this.groupBoxLocal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxLocal.BackColor = System.Drawing.SystemColors.Window;
            this.groupBoxLocal.Controls.Add(this.labelFilenameProcessingProgress);
            this.groupBoxLocal.Controls.Add(this.labelMediaInfoProgress);
            this.groupBoxLocal.Location = new System.Drawing.Point(3, 6);
            this.groupBoxLocal.Name = "groupBoxLocal";
            this.groupBoxLocal.Size = new System.Drawing.Size(539, 90);
            this.groupBoxLocal.TabIndex = 26;
            this.groupBoxLocal.TabStop = false;
            this.groupBoxLocal.Text = "Local Files";
            // 
            // groupBoxMetaData
            // 
            this.groupBoxMetaData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxMetaData.BackColor = System.Drawing.SystemColors.Window;
            this.groupBoxMetaData.Controls.Add(this.labelMatchingSeriesProgress);
            this.groupBoxMetaData.Controls.Add(this.labelRetrievingSeriesMetaDataProgress);
            this.groupBoxMetaData.Controls.Add(this.labelIdentifyEpisodesProgress);
            this.groupBoxMetaData.Controls.Add(this.labelUpdatingEpisodeMetaDataProgress);
            this.groupBoxMetaData.Location = new System.Drawing.Point(2, 101);
            this.groupBoxMetaData.Name = "groupBoxMetaData";
            this.groupBoxMetaData.Size = new System.Drawing.Size(540, 161);
            this.groupBoxMetaData.TabIndex = 27;
            this.groupBoxMetaData.TabStop = false;
            this.groupBoxMetaData.Text = "Metadata";
            // 
            // groupBoxArtwork
            // 
            this.groupBoxArtwork.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxArtwork.BackColor = System.Drawing.SystemColors.Window;
            this.groupBoxArtwork.Controls.Add(this.labelRetrievingEpisodeThumbsProgress);
            this.groupBoxArtwork.Controls.Add(this.labelRetrievingFanartProgress);
            this.groupBoxArtwork.Controls.Add(this.labelRetrievingSeriesArtworkProgress);
            this.groupBoxArtwork.Location = new System.Drawing.Point(3, 268);
            this.groupBoxArtwork.Name = "groupBoxArtwork";
            this.groupBoxArtwork.Size = new System.Drawing.Size(538, 133);
            this.groupBoxArtwork.TabIndex = 28;
            this.groupBoxArtwork.TabStop = false;
            this.groupBoxArtwork.Text = "Artwork";
            // 
            // labelFilenameProcessingProgress
            // 
            this.labelFilenameProcessingProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFilenameProcessingProgress.Location = new System.Drawing.Point(6, 16);
            this.labelFilenameProcessingProgress.Name = "labelFilenameProcessingProgress";
            this.labelFilenameProcessingProgress.Size = new System.Drawing.Size(518, 39);
            this.labelFilenameProcessingProgress.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.labelFilenameProcessingProgress.TabIndex = 16;
            // 
            // labelMediaInfoProgress
            // 
            this.labelMediaInfoProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMediaInfoProgress.Location = new System.Drawing.Point(6, 49);
            this.labelMediaInfoProgress.Name = "labelMediaInfoProgress";
            this.labelMediaInfoProgress.Size = new System.Drawing.Size(518, 39);
            this.labelMediaInfoProgress.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.labelMediaInfoProgress.TabIndex = 22;
            // 
            // labelMatchingSeriesProgress
            // 
            this.labelMatchingSeriesProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMatchingSeriesProgress.Location = new System.Drawing.Point(6, 19);
            this.labelMatchingSeriesProgress.Name = "labelMatchingSeriesProgress";
            this.labelMatchingSeriesProgress.Size = new System.Drawing.Size(518, 39);
            this.labelMatchingSeriesProgress.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.labelMatchingSeriesProgress.TabIndex = 17;
            // 
            // labelRetrievingSeriesMetaDataProgress
            // 
            this.labelRetrievingSeriesMetaDataProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRetrievingSeriesMetaDataProgress.Location = new System.Drawing.Point(6, 51);
            this.labelRetrievingSeriesMetaDataProgress.Name = "labelRetrievingSeriesMetaDataProgress";
            this.labelRetrievingSeriesMetaDataProgress.Size = new System.Drawing.Size(518, 39);
            this.labelRetrievingSeriesMetaDataProgress.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.labelRetrievingSeriesMetaDataProgress.TabIndex = 18;
            // 
            // labelIdentifyEpisodesProgress
            // 
            this.labelIdentifyEpisodesProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelIdentifyEpisodesProgress.Location = new System.Drawing.Point(6, 84);
            this.labelIdentifyEpisodesProgress.Name = "labelIdentifyEpisodesProgress";
            this.labelIdentifyEpisodesProgress.Size = new System.Drawing.Size(518, 39);
            this.labelIdentifyEpisodesProgress.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.labelIdentifyEpisodesProgress.TabIndex = 19;
            // 
            // labelUpdatingEpisodeMetaDataProgress
            // 
            this.labelUpdatingEpisodeMetaDataProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUpdatingEpisodeMetaDataProgress.Location = new System.Drawing.Point(6, 116);
            this.labelUpdatingEpisodeMetaDataProgress.Name = "labelUpdatingEpisodeMetaDataProgress";
            this.labelUpdatingEpisodeMetaDataProgress.Size = new System.Drawing.Size(518, 39);
            this.labelUpdatingEpisodeMetaDataProgress.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.labelUpdatingEpisodeMetaDataProgress.TabIndex = 21;
            // 
            // labelRetrievingEpisodeThumbsProgress
            // 
            this.labelRetrievingEpisodeThumbsProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRetrievingEpisodeThumbsProgress.Location = new System.Drawing.Point(6, 85);
            this.labelRetrievingEpisodeThumbsProgress.Name = "labelRetrievingEpisodeThumbsProgress";
            this.labelRetrievingEpisodeThumbsProgress.Size = new System.Drawing.Size(517, 39);
            this.labelRetrievingEpisodeThumbsProgress.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.labelRetrievingEpisodeThumbsProgress.TabIndex = 24;
            // 
            // labelRetrievingFanartProgress
            // 
            this.labelRetrievingFanartProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRetrievingFanartProgress.Location = new System.Drawing.Point(6, 51);
            this.labelRetrievingFanartProgress.Name = "labelRetrievingFanartProgress";
            this.labelRetrievingFanartProgress.Size = new System.Drawing.Size(517, 39);
            this.labelRetrievingFanartProgress.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.labelRetrievingFanartProgress.TabIndex = 23;
            // 
            // labelRetrievingSeriesArtworkProgress
            // 
            this.labelRetrievingSeriesArtworkProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRetrievingSeriesArtworkProgress.Location = new System.Drawing.Point(6, 19);
            this.labelRetrievingSeriesArtworkProgress.Name = "labelRetrievingSeriesArtworkProgress";
            this.labelRetrievingSeriesArtworkProgress.Size = new System.Drawing.Size(517, 39);
            this.labelRetrievingSeriesArtworkProgress.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.labelRetrievingSeriesArtworkProgress.TabIndex = 20;
            // 
            // ImportPanelProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelProgress);
            this.Name = "ImportPanelProgress";
            this.Size = new System.Drawing.Size(545, 406);
            this.panelProgress.ResumeLayout(false);
            this.groupBoxLocal.ResumeLayout(false);
            this.groupBoxMetaData.ResumeLayout(false);
            this.groupBoxArtwork.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelProgress;
        private System.Windows.Forms.GroupBox groupBoxLocal;
        private ProgressLabel labelFilenameProcessingProgress;
        private ProgressLabel labelMediaInfoProgress;
        private System.Windows.Forms.GroupBox groupBoxMetaData;
        private ProgressLabel labelMatchingSeriesProgress;
        private ProgressLabel labelRetrievingSeriesMetaDataProgress;
        private ProgressLabel labelIdentifyEpisodesProgress;
        private ProgressLabel labelUpdatingEpisodeMetaDataProgress;
        private System.Windows.Forms.GroupBox groupBoxArtwork;
        private ProgressLabel labelRetrievingEpisodeThumbsProgress;
        private ProgressLabel labelRetrievingFanartProgress;
        private ProgressLabel labelRetrievingSeriesArtworkProgress;
    }
}
