namespace WindowPlugins.GUITVSeries
{
    partial class loadingDisplay
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
            this.label1 = new System.Windows.Forms.Label();
            this.series = new System.Windows.Forms.Label();
            this.season = new System.Windows.Forms.Label();
            this.episodes = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.version = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(216, 155);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(163, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please wait, loading your library...";
            this.label1.UseWaitCursor = true;
            // 
            // series
            // 
            this.series.AutoSize = true;
            this.series.BackColor = System.Drawing.Color.Transparent;
            this.series.Location = new System.Drawing.Point(180, 172);
            this.series.Name = "series";
            this.series.Size = new System.Drawing.Size(35, 13);
            this.series.TabIndex = 3;
            this.series.Text = "label3";
            this.series.UseWaitCursor = true;
            // 
            // season
            // 
            this.season.AutoSize = true;
            this.season.BackColor = System.Drawing.Color.Transparent;
            this.season.Location = new System.Drawing.Point(277, 172);
            this.season.Name = "season";
            this.season.Size = new System.Drawing.Size(35, 13);
            this.season.TabIndex = 4;
            this.season.Text = "label3";
            this.season.UseWaitCursor = true;
            // 
            // episodes
            // 
            this.episodes.AutoSize = true;
            this.episodes.BackColor = System.Drawing.Color.Transparent;
            this.episodes.Location = new System.Drawing.Point(378, 172);
            this.episodes.Name = "episodes";
            this.episodes.Size = new System.Drawing.Size(35, 13);
            this.episodes.TabIndex = 5;
            this.episodes.Text = "label3";
            this.episodes.UseWaitCursor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.Image = global::WindowPlugins.GUITVSeries.Properties.Resources.tv_series_logo_noreflection;
            this.pictureBox1.InitialImage = global::WindowPlugins.GUITVSeries.Properties.Resources.tv_series_logo_noreflection;
            this.pictureBox1.Location = new System.Drawing.Point(12, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(560, 148);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.UseWaitCursor = true;
            // 
            // version
            // 
            this.version.AutoSize = true;
            this.version.BackColor = System.Drawing.Color.Transparent;
            this.version.Location = new System.Drawing.Point(277, 131);
            this.version.Name = "version";
            this.version.Size = new System.Drawing.Size(41, 13);
            this.version.TabIndex = 6;
            this.version.Text = "version";
            this.version.UseWaitCursor = true;
            // 
            // loadingDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(584, 194);
            this.Controls.Add(this.version);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.episodes);
            this.Controls.Add(this.season);
            this.Controls.Add(this.series);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "loadingDisplay";
            this.Opacity = 0.9;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Please wait...";
            this.TransparencyKey = System.Drawing.Color.Maroon;
            this.UseWaitCursor = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label series;
        private System.Windows.Forms.Label season;
        private System.Windows.Forms.Label episodes;
        private System.Windows.Forms.Label version;
    }
}