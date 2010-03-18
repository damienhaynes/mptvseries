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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panelTitle = new System.Windows.Forms.Panel();
            this.panelProgress = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.progressLabel1 = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.progressLabel10 = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.progressLabel2 = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.progressLabel4 = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.progressLabel3 = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.progressLabel5 = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.progressLabel8 = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.progressLabel9 = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.progressLabel6 = new WindowPlugins.GUITVSeries.Configuration.ProgressLabel();
            this.groupBoxDetails = new System.Windows.Forms.GroupBox();
            this.labelNoInfo = new System.Windows.Forms.Label();
            this.pictureBoxDetails = new System.Windows.Forms.PictureBox();
            this.textBoxDetails = new System.Windows.Forms.TextBox();
            this.labelDetailHeader = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panelTitle.SuspendLayout();
            this.panelProgress.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBoxDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDetails)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(82, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(221, 16);
            this.label1.TabIndex = 12;
            this.label1.Text = "Importing your Series and Episodes";
            // 
            // label_wait_parse
            // 
            this.label_wait_parse.AutoSize = true;
            this.label_wait_parse.Location = new System.Drawing.Point(97, 39);
            this.label_wait_parse.Name = "label_wait_parse";
            this.label_wait_parse.Size = new System.Drawing.Size(153, 13);
            this.label_wait_parse.TabIndex = 13;
            this.label_wait_parse.Text = "Welcome to the Import Wizard!";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(344, 464);
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
            this.btnFinish.Location = new System.Drawing.Point(425, 464);
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Size = new System.Drawing.Size(75, 23);
            this.btnFinish.TabIndex = 15;
            this.btnFinish.Text = "Finish";
            this.btnFinish.UseVisualStyleBackColor = true;
            this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.progressLabel1);
            this.groupBox1.Controls.Add(this.progressLabel10);
            this.groupBox1.Location = new System.Drawing.Point(3, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(269, 90);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Local Files";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.progressLabel2);
            this.groupBox2.Controls.Add(this.progressLabel4);
            this.groupBox2.Controls.Add(this.progressLabel3);
            this.groupBox2.Controls.Add(this.progressLabel5);
            this.groupBox2.Location = new System.Drawing.Point(2, 101);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(270, 161);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Metadata";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.progressLabel8);
            this.groupBox3.Controls.Add(this.progressLabel9);
            this.groupBox3.Controls.Add(this.progressLabel6);
            this.groupBox3.Location = new System.Drawing.Point(3, 268);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(269, 123);
            this.groupBox3.TabIndex = 28;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Artwork";
            // 
            // panelTitle
            // 
            this.panelTitle.Controls.Add(this.label1);
            this.panelTitle.Controls.Add(this.label_wait_parse);
            this.panelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitle.Location = new System.Drawing.Point(0, 0);
            this.panelTitle.Name = "panelTitle";
            this.panelTitle.Size = new System.Drawing.Size(791, 68);
            this.panelTitle.TabIndex = 29;
            // 
            // panelProgress
            // 
            this.panelProgress.AutoScroll = true;
            this.panelProgress.Controls.Add(this.groupBox1);
            this.panelProgress.Controls.Add(this.groupBox2);
            this.panelProgress.Controls.Add(this.groupBox3);
            this.panelProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelProgress.Location = new System.Drawing.Point(0, 0);
            this.panelProgress.Name = "panelProgress";
            this.panelProgress.Size = new System.Drawing.Size(275, 498);
            this.panelProgress.TabIndex = 30;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBoxDetails);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnFinish);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(512, 498);
            this.panel1.TabIndex = 31;
            // 
            // progressLabel1
            // 
            this.progressLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel1.Location = new System.Drawing.Point(-3, 16);
            this.progressLabel1.Name = "progressLabel1";
            this.progressLabel1.Size = new System.Drawing.Size(257, 39);
            this.progressLabel1.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.progressLabel1.TabIndex = 16;
            // 
            // progressLabel10
            // 
            this.progressLabel10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel10.Location = new System.Drawing.Point(-3, 49);
            this.progressLabel10.Name = "progressLabel10";
            this.progressLabel10.Size = new System.Drawing.Size(257, 39);
            this.progressLabel10.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.progressLabel10.TabIndex = 22;
            // 
            // progressLabel2
            // 
            this.progressLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel2.Location = new System.Drawing.Point(0, 19);
            this.progressLabel2.Name = "progressLabel2";
            this.progressLabel2.Size = new System.Drawing.Size(258, 39);
            this.progressLabel2.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.progressLabel2.TabIndex = 17;
            // 
            // progressLabel4
            // 
            this.progressLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel4.Location = new System.Drawing.Point(0, 51);
            this.progressLabel4.Name = "progressLabel4";
            this.progressLabel4.Size = new System.Drawing.Size(258, 39);
            this.progressLabel4.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.progressLabel4.TabIndex = 18;
            // 
            // progressLabel3
            // 
            this.progressLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel3.Location = new System.Drawing.Point(0, 84);
            this.progressLabel3.Name = "progressLabel3";
            this.progressLabel3.Size = new System.Drawing.Size(258, 39);
            this.progressLabel3.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.progressLabel3.TabIndex = 19;
            // 
            // progressLabel5
            // 
            this.progressLabel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel5.Location = new System.Drawing.Point(0, 116);
            this.progressLabel5.Name = "progressLabel5";
            this.progressLabel5.Size = new System.Drawing.Size(258, 39);
            this.progressLabel5.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.progressLabel5.TabIndex = 21;
            // 
            // progressLabel8
            // 
            this.progressLabel8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel8.Location = new System.Drawing.Point(-3, 85);
            this.progressLabel8.Name = "progressLabel8";
            this.progressLabel8.Size = new System.Drawing.Size(257, 39);
            this.progressLabel8.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.progressLabel8.TabIndex = 24;
            // 
            // progressLabel9
            // 
            this.progressLabel9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel9.Location = new System.Drawing.Point(-3, 51);
            this.progressLabel9.Name = "progressLabel9";
            this.progressLabel9.Size = new System.Drawing.Size(257, 39);
            this.progressLabel9.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.progressLabel9.TabIndex = 23;
            // 
            // progressLabel6
            // 
            this.progressLabel6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel6.Location = new System.Drawing.Point(-3, 19);
            this.progressLabel6.Name = "progressLabel6";
            this.progressLabel6.Size = new System.Drawing.Size(257, 39);
            this.progressLabel6.Status = WindowPlugins.GUITVSeries.Configuration.ProgressLabelStatus.Waiting;
            this.progressLabel6.TabIndex = 20;
            // 
            // groupBoxDetails
            // 
            this.groupBoxDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxDetails.Controls.Add(this.labelDetailHeader);
            this.groupBoxDetails.Controls.Add(this.textBoxDetails);
            this.groupBoxDetails.Controls.Add(this.pictureBoxDetails);
            this.groupBoxDetails.Controls.Add(this.labelNoInfo);
            this.groupBoxDetails.Location = new System.Drawing.Point(3, 22);
            this.groupBoxDetails.Name = "groupBoxDetails";
            this.groupBoxDetails.Size = new System.Drawing.Size(497, 436);
            this.groupBoxDetails.TabIndex = 16;
            this.groupBoxDetails.TabStop = false;
            this.groupBoxDetails.Text = "Currently Processing";
            // 
            // labelNoInfo
            // 
            this.labelNoInfo.AutoSize = true;
            this.labelNoInfo.Location = new System.Drawing.Point(31, 26);
            this.labelNoInfo.Name = "labelNoInfo";
            this.labelNoInfo.Size = new System.Drawing.Size(158, 13);
            this.labelNoInfo.TabIndex = 0;
            this.labelNoInfo.Text = "No Further Information Available";
            // 
            // pictureBoxDetails
            // 
            this.pictureBoxDetails.Location = new System.Drawing.Point(6, 67);
            this.pictureBoxDetails.Name = "pictureBoxDetails";
            this.pictureBoxDetails.Size = new System.Drawing.Size(120, 102);
            this.pictureBoxDetails.TabIndex = 1;
            this.pictureBoxDetails.TabStop = false;
            // 
            // textBoxDetails
            // 
            this.textBoxDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDetails.Location = new System.Drawing.Point(132, 67);
            this.textBoxDetails.Multiline = true;
            this.textBoxDetails.Name = "textBoxDetails";
            this.textBoxDetails.ReadOnly = true;
            this.textBoxDetails.Size = new System.Drawing.Size(365, 214);
            this.textBoxDetails.TabIndex = 2;
            // 
            // labelDetailHeader
            // 
            this.labelDetailHeader.AutoSize = true;
            this.labelDetailHeader.Location = new System.Drawing.Point(129, 46);
            this.labelDetailHeader.Name = "labelDetailHeader";
            this.labelDetailHeader.Size = new System.Drawing.Size(59, 13);
            this.labelDetailHeader.TabIndex = 3;
            this.labelDetailHeader.Text = "Information";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 68);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panelProgress);
            this.splitContainer1.Panel1MinSize = 275;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(791, 498);
            this.splitContainer1.SplitterDistance = 275;
            this.splitContainer1.TabIndex = 32;
            // 
            // ImportProgessPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panelTitle);
            this.Name = "ImportProgessPage";
            this.Size = new System.Drawing.Size(791, 566);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.panelTitle.ResumeLayout(false);
            this.panelTitle.PerformLayout();
            this.panelProgress.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBoxDetails.ResumeLayout(false);
            this.groupBoxDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDetails)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

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
        private System.Windows.Forms.Panel panelTitle;
        private System.Windows.Forms.Panel panelProgress;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBoxDetails;
        private System.Windows.Forms.PictureBox pictureBoxDetails;
        private System.Windows.Forms.Label labelNoInfo;
        private System.Windows.Forms.TextBox textBoxDetails;
        private System.Windows.Forms.Label labelDetailHeader;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
