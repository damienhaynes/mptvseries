namespace WindowPlugins.GUITVSeries
{
    partial class About
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblBuild = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkForum = new System.Windows.Forms.LinkLabel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblEpisodes = new System.Windows.Forms.Label();
            this.lblSeasons = new System.Windows.Forms.Label();
            this.lblSeries = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.locationBrowser5 = new WindowPlugins.GUITVSeries.LocationBrowser();
            this.locationBrowser4 = new WindowPlugins.GUITVSeries.LocationBrowser();
            this.locationBrowser3 = new WindowPlugins.GUITVSeries.LocationBrowser();
            this.locationBrowser2 = new WindowPlugins.GUITVSeries.LocationBrowser();
            this.locationBrowser1 = new WindowPlugins.GUITVSeries.LocationBrowser();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lblBuild);
            this.groupBox1.Controls.Add(this.lblVersion);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(23, 150);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(353, 100);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "About MP-TVSeries";
            // 
            // lblBuild
            // 
            this.lblBuild.AutoSize = true;
            this.lblBuild.Location = new System.Drawing.Point(9, 75);
            this.lblBuild.Name = "lblBuild";
            this.lblBuild.Size = new System.Drawing.Size(30, 13);
            this.lblBuild.TabIndex = 2;
            this.lblBuild.Text = "Built:";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(9, 59);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(48, 13);
            this.lblVersion.TabIndex = 1;
            this.lblVersion.Text = "Version: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(238, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "MP-TVSeries is a plugin for Mediaportal aimed at \r\norganizing TV-Series.";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.linkLabel3);
            this.groupBox2.Controls.Add(this.linkLabel2);
            this.groupBox2.Controls.Add(this.linkLabel1);
            this.groupBox2.Controls.Add(this.linkForum);
            this.groupBox2.Location = new System.Drawing.Point(23, 254);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(353, 84);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Help";
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Location = new System.Drawing.Point(9, 65);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(68, 13);
            this.linkLabel3.TabIndex = 3;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "Wiki/Manual";
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel3_LinkClicked);
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(9, 49);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(125, 13);
            this.linkLabel2.TabIndex = 2;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "SourceForge Project Site";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(9, 32);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(78, 13);
            this.linkLabel1.TabIndex = 1;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "TheTVDB.com";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // linkForum
            // 
            this.linkForum.AutoSize = true;
            this.linkForum.Location = new System.Drawing.Point(9, 16);
            this.linkForum.Name = "linkForum";
            this.linkForum.Size = new System.Drawing.Size(36, 13);
            this.linkForum.TabIndex = 0;
            this.linkForum.TabStop = true;
            this.linkForum.Text = "Forum";
            this.linkForum.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkForum_LinkClicked);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.lblEpisodes);
            this.groupBox3.Controls.Add(this.lblSeasons);
            this.groupBox3.Controls.Add(this.lblSeries);
            this.groupBox3.Location = new System.Drawing.Point(23, 344);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(353, 72);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Local Statistics";
            // 
            // lblEpisodes
            // 
            this.lblEpisodes.AutoSize = true;
            this.lblEpisodes.Location = new System.Drawing.Point(9, 52);
            this.lblEpisodes.Name = "lblEpisodes";
            this.lblEpisodes.Size = new System.Drawing.Size(53, 13);
            this.lblEpisodes.TabIndex = 2;
            this.lblEpisodes.Text = "Episodes:";
            // 
            // lblSeasons
            // 
            this.lblSeasons.AutoSize = true;
            this.lblSeasons.Location = new System.Drawing.Point(9, 36);
            this.lblSeasons.Name = "lblSeasons";
            this.lblSeasons.Size = new System.Drawing.Size(51, 13);
            this.lblSeasons.TabIndex = 1;
            this.lblSeasons.Text = "Seasons:";
            // 
            // lblSeries
            // 
            this.lblSeries.AutoSize = true;
            this.lblSeries.Location = new System.Drawing.Point(9, 20);
            this.lblSeries.Name = "lblSeries";
            this.lblSeries.Size = new System.Drawing.Size(39, 13);
            this.lblSeries.TabIndex = 0;
            this.lblSeries.Text = "Series:";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.locationBrowser5);
            this.groupBox4.Controls.Add(this.locationBrowser4);
            this.groupBox4.Controls.Add(this.locationBrowser3);
            this.groupBox4.Controls.Add(this.locationBrowser2);
            this.groupBox4.Controls.Add(this.locationBrowser1);
            this.groupBox4.Location = new System.Drawing.Point(23, 422);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(353, 141);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Local Paths:";
            // 
            // locationBrowser5
            // 
            this.locationBrowser5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.locationBrowser5.BackColor = System.Drawing.Color.Transparent;
            this.locationBrowser5.Location = new System.Drawing.Point(6, 107);
            this.locationBrowser5.Name = "locationBrowser5";
            this.locationBrowser5.Size = new System.Drawing.Size(341, 29);
            this.locationBrowser5.TabIndex = 4;
            // 
            // locationBrowser4
            // 
            this.locationBrowser4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.locationBrowser4.BackColor = System.Drawing.Color.Transparent;
            this.locationBrowser4.Location = new System.Drawing.Point(6, 86);
            this.locationBrowser4.Name = "locationBrowser4";
            this.locationBrowser4.Size = new System.Drawing.Size(341, 29);
            this.locationBrowser4.TabIndex = 3;
            // 
            // locationBrowser3
            // 
            this.locationBrowser3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.locationBrowser3.BackColor = System.Drawing.Color.Transparent;
            this.locationBrowser3.Location = new System.Drawing.Point(6, 63);
            this.locationBrowser3.Name = "locationBrowser3";
            this.locationBrowser3.Size = new System.Drawing.Size(341, 29);
            this.locationBrowser3.TabIndex = 2;
            // 
            // locationBrowser2
            // 
            this.locationBrowser2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.locationBrowser2.BackColor = System.Drawing.Color.Transparent;
            this.locationBrowser2.Location = new System.Drawing.Point(6, 40);
            this.locationBrowser2.Name = "locationBrowser2";
            this.locationBrowser2.Size = new System.Drawing.Size(341, 29);
            this.locationBrowser2.TabIndex = 1;
            // 
            // locationBrowser1
            // 
            this.locationBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.locationBrowser1.BackColor = System.Drawing.Color.Transparent;
            this.locationBrowser1.Location = new System.Drawing.Point(6, 19);
            this.locationBrowser1.Name = "locationBrowser1";
            this.locationBrowser1.Size = new System.Drawing.Size(341, 29);
            this.locationBrowser1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.Image = global::WindowPlugins.GUITVSeries.Properties.Resources.tv_series_logo_noreflection;
            this.pictureBox1.InitialImage = global::WindowPlugins.GUITVSeries.Properties.Resources.tv_series_logo_noreflection;
            this.pictureBox1.Location = new System.Drawing.Point(23, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(353, 141);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.UseWaitCursor = true;
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "About";
            this.Size = new System.Drawing.Size(404, 572);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblBuild;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblEpisodes;
        private System.Windows.Forms.Label lblSeasons;
        private System.Windows.Forms.Label lblSeries;
        private System.Windows.Forms.GroupBox groupBox4;
        private LocationBrowser locationBrowser5;
        private LocationBrowser locationBrowser4;
        private LocationBrowser locationBrowser3;
        private LocationBrowser locationBrowser2;
        private LocationBrowser locationBrowser1;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel linkForum;
    }
}
