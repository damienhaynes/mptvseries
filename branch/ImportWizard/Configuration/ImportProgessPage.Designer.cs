namespace WindowPlugins.GUITVSeries.Configuration
{
    partial class ImportProgessPage
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
            this.label1 = new System.Windows.Forms.Label();
            this.label_wait_parse = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnFinish = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressLabel1 = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.progressLabel10 = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.progressLabel2 = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.progressLabel4 = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.progressLabel3 = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.progressLabel5 = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.progressLabel8 = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.progressLabel9 = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.progressLabel6 = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.labelDetails = new System.Windows.Forms.Label();
            this.pictureDetails = new System.Windows.Forms.PictureBox();
            this.textDetails = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(22, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(203, 16);
            this.label1.TabIndex = 12;
            this.label1.Text = "Step 2 of 2: Online Data Retrieval";
            // 
            // label_wait_parse
            // 
            this.label_wait_parse.AutoSize = true;
            this.label_wait_parse.Location = new System.Drawing.Point(37, 35);
            this.label_wait_parse.Name = "label_wait_parse";
            this.label_wait_parse.Size = new System.Drawing.Size(622, 26);
            this.label_wait_parse.TabIndex = 13;
            this.label_wait_parse.Text = "Please wait while your Series and Episodes are being identified. You may be asked" +
                " for clearification during the identification stages.\r\n\r\n";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(626, 355);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnFinish
            // 
            this.btnFinish.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFinish.Enabled = false;
            this.btnFinish.Location = new System.Drawing.Point(707, 355);
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Size = new System.Drawing.Size(75, 23);
            this.btnFinish.TabIndex = 15;
            this.btnFinish.Text = "Finish";
            this.btnFinish.UseVisualStyleBackColor = true;
            this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressLabel1);
            this.groupBox1.Controls.Add(this.progressLabel10);
            this.groupBox1.Location = new System.Drawing.Point(40, 64);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(428, 112);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Local Files";
            // 
            // progressLabel1
            // 
            this.progressLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel1.Location = new System.Drawing.Point(6, 19);
            this.progressLabel1.Name = "progressLabel1";
            this.progressLabel1.Size = new System.Drawing.Size(416, 39);
            this.progressLabel1.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.progressLabel1.TabIndex = 16;
            // 
            // progressLabel10
            // 
            this.progressLabel10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel10.Location = new System.Drawing.Point(6, 64);
            this.progressLabel10.Name = "progressLabel10";
            this.progressLabel10.Size = new System.Drawing.Size(416, 39);
            this.progressLabel10.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.progressLabel10.TabIndex = 22;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.progressLabel2);
            this.groupBox2.Controls.Add(this.progressLabel4);
            this.groupBox2.Controls.Add(this.progressLabel3);
            this.groupBox2.Controls.Add(this.progressLabel5);
            this.groupBox2.Location = new System.Drawing.Point(40, 182);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(428, 203);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Metadata";
            // 
            // progressLabel2
            // 
            this.progressLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel2.Location = new System.Drawing.Point(6, 19);
            this.progressLabel2.Name = "progressLabel2";
            this.progressLabel2.Size = new System.Drawing.Size(416, 39);
            this.progressLabel2.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.progressLabel2.TabIndex = 17;
            // 
            // progressLabel4
            // 
            this.progressLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel4.Location = new System.Drawing.Point(6, 109);
            this.progressLabel4.Name = "progressLabel4";
            this.progressLabel4.Size = new System.Drawing.Size(416, 39);
            this.progressLabel4.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.progressLabel4.TabIndex = 18;
            // 
            // progressLabel3
            // 
            this.progressLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel3.Location = new System.Drawing.Point(6, 64);
            this.progressLabel3.Name = "progressLabel3";
            this.progressLabel3.Size = new System.Drawing.Size(416, 39);
            this.progressLabel3.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.progressLabel3.TabIndex = 19;
            // 
            // progressLabel5
            // 
            this.progressLabel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel5.Location = new System.Drawing.Point(6, 154);
            this.progressLabel5.Name = "progressLabel5";
            this.progressLabel5.Size = new System.Drawing.Size(416, 39);
            this.progressLabel5.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.progressLabel5.TabIndex = 21;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.progressLabel8);
            this.groupBox3.Controls.Add(this.progressLabel9);
            this.groupBox3.Controls.Add(this.progressLabel6);
            this.groupBox3.Location = new System.Drawing.Point(474, 182);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(314, 157);
            this.groupBox3.TabIndex = 28;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Artwork";
            // 
            // progressLabel8
            // 
            this.progressLabel8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel8.Location = new System.Drawing.Point(6, 109);
            this.progressLabel8.Name = "progressLabel8";
            this.progressLabel8.Size = new System.Drawing.Size(302, 39);
            this.progressLabel8.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.progressLabel8.TabIndex = 24;
            // 
            // progressLabel9
            // 
            this.progressLabel9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel9.Location = new System.Drawing.Point(6, 64);
            this.progressLabel9.Name = "progressLabel9";
            this.progressLabel9.Size = new System.Drawing.Size(302, 39);
            this.progressLabel9.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.progressLabel9.TabIndex = 23;
            // 
            // progressLabel6
            // 
            this.progressLabel6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel6.Location = new System.Drawing.Point(6, 19);
            this.progressLabel6.Name = "progressLabel6";
            this.progressLabel6.Size = new System.Drawing.Size(302, 39);
            this.progressLabel6.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.progressLabel6.TabIndex = 20;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.labelDetails);
            this.groupBox4.Controls.Add(this.pictureDetails);
            this.groupBox4.Controls.Add(this.textDetails);
            this.groupBox4.Location = new System.Drawing.Point(474, 64);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(314, 112);
            this.groupBox4.TabIndex = 29;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Details of current Item";
            // 
            // labelDetails
            // 
            this.labelDetails.AutoSize = true;
            this.labelDetails.Location = new System.Drawing.Point(73, 18);
            this.labelDetails.Name = "labelDetails";
            this.labelDetails.Size = new System.Drawing.Size(64, 13);
            this.labelDetails.TabIndex = 2;
            this.labelDetails.Text = "Current Item";
            // 
            // pictureDetails
            // 
            this.pictureDetails.Location = new System.Drawing.Point(6, 19);
            this.pictureDetails.Name = "pictureDetails";
            this.pictureDetails.Size = new System.Drawing.Size(61, 84);
            this.pictureDetails.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureDetails.TabIndex = 1;
            this.pictureDetails.TabStop = false;
            // 
            // textDetails
            // 
            this.textDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textDetails.Location = new System.Drawing.Point(73, 35);
            this.textDetails.Multiline = true;
            this.textDetails.Name = "textDetails";
            this.textDetails.Size = new System.Drawing.Size(235, 68);
            this.textDetails.TabIndex = 0;
            // 
            // ImportProgessPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnFinish);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label_wait_parse);
            this.Controls.Add(this.label1);
            this.Name = "ImportProgessPage";
            this.Size = new System.Drawing.Size(791, 397);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDetails)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_wait_parse;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnFinish;
        private ProgressLabel progressLabel1;
        private ProgressLabel progressLabel2;
        private ProgressLabel progressLabel3;
        private ProgressLabel progressLabel4;
        private ProgressLabel progressLabel5;
        private ProgressLabel progressLabel6;
        private ProgressLabel progressLabel8;
        private ProgressLabel progressLabel9;
        private ProgressLabel progressLabel10;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.PictureBox pictureDetails;
        private System.Windows.Forms.TextBox textDetails;
        private System.Windows.Forms.Label labelDetails;
    }
}
