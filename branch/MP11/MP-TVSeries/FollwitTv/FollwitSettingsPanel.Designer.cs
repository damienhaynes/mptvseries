using Follwit.API.UI.Panels;
namespace WindowPlugins.GUITVSeries.FollwitTv {
    partial class FollwitSettingsPanel {
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
            this.label1 = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.userLinkLabel = new System.Windows.Forms.LinkLabel();
            this.accountButton = new System.Windows.Forms.Button();
            this.syncButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.publicProfileCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.restrictSyncCheckBox = new System.Windows.Forms.CheckBox();
            this.defineSyncedShowsButton = new System.Windows.Forms.Button();
            this.logoPanel1 = new Follwit.API.UI.Panels.LogoPanel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Location = new System.Drawing.Point(3, 66);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(485, 3);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Account:";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.statusLabel.Location = new System.Drawing.Point(0, 0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(71, 13);
            this.statusLabel.TabIndex = 3;
            this.statusLabel.Text = "Logged in as:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.userLinkLabel);
            this.panel1.Controls.Add(this.statusLabel);
            this.panel1.Location = new System.Drawing.Point(133, 75);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 15);
            this.panel1.TabIndex = 4;
            // 
            // userLinkLabel
            // 
            this.userLinkLabel.AutoSize = true;
            this.userLinkLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.userLinkLabel.Location = new System.Drawing.Point(71, 0);
            this.userLinkLabel.Name = "userLinkLabel";
            this.userLinkLabel.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.userLinkLabel.Size = new System.Drawing.Size(45, 13);
            this.userLinkLabel.TabIndex = 4;
            this.userLinkLabel.TabStop = true;
            this.userLinkLabel.Text = "kramer";
            this.userLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.userLinkLabel_LinkClicked);
            // 
            // accountButton
            // 
            this.accountButton.Location = new System.Drawing.Point(134, 96);
            this.accountButton.Name = "accountButton";
            this.accountButton.Size = new System.Drawing.Size(114, 23);
            this.accountButton.TabIndex = 5;
            this.accountButton.Text = "Setup Account";
            this.accountButton.UseVisualStyleBackColor = true;
            this.accountButton.Click += new System.EventHandler(this.setupDisconnecButton_Click);
            // 
            // syncButton
            // 
            this.syncButton.Location = new System.Drawing.Point(254, 96);
            this.syncButton.Name = "syncButton";
            this.syncButton.Size = new System.Drawing.Size(110, 23);
            this.syncButton.TabIndex = 6;
            this.syncButton.Text = "Synchronize Now";
            this.syncButton.UseVisualStyleBackColor = true;
            this.syncButton.Click += new System.EventHandler(this.syncButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Location = new System.Drawing.Point(3, 125);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(485, 3);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(8, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Privacy:";
            // 
            // publicProfileCheckBox
            // 
            this.publicProfileCheckBox.AutoSize = true;
            this.publicProfileCheckBox.Enabled = false;
            this.publicProfileCheckBox.Location = new System.Drawing.Point(136, 134);
            this.publicProfileCheckBox.Name = "publicProfileCheckBox";
            this.publicProfileCheckBox.Size = new System.Drawing.Size(208, 17);
            this.publicProfileCheckBox.TabIndex = 8;
            this.publicProfileCheckBox.Text = "Allow others to view my profile online.";
            this.publicProfileCheckBox.UseVisualStyleBackColor = true;
            this.publicProfileCheckBox.CheckedChanged += new System.EventHandler(this.privateProfileCheckBox_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Location = new System.Drawing.Point(131, 158);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(357, 3);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Visible = false;
            // 
            // restrictSyncCheckBox
            // 
            this.restrictSyncCheckBox.AutoSize = true;
            this.restrictSyncCheckBox.Enabled = false;
            this.restrictSyncCheckBox.Location = new System.Drawing.Point(136, 167);
            this.restrictSyncCheckBox.Name = "restrictSyncCheckBox";
            this.restrictSyncCheckBox.Size = new System.Drawing.Size(355, 17);
            this.restrictSyncCheckBox.TabIndex = 9;
            this.restrictSyncCheckBox.Text = "Restrict which TV shows are synchronized to follw.it. (Coming Soon!)";
            this.restrictSyncCheckBox.UseVisualStyleBackColor = true;
            this.restrictSyncCheckBox.Visible = false;
            this.restrictSyncCheckBox.CheckedChanged += new System.EventHandler(this.restrictSyncCheckBox_CheckedChanged);
            // 
            // defineSyncedShowsButton
            // 
            this.defineSyncedShowsButton.Enabled = false;
            this.defineSyncedShowsButton.Location = new System.Drawing.Point(136, 191);
            this.defineSyncedShowsButton.Name = "defineSyncedShowsButton";
            this.defineSyncedShowsButton.Size = new System.Drawing.Size(162, 23);
            this.defineSyncedShowsButton.TabIndex = 10;
            this.defineSyncedShowsButton.Text = "Define Synchronized Shows";
            this.defineSyncedShowsButton.UseVisualStyleBackColor = true;
            this.defineSyncedShowsButton.Visible = false;
            this.defineSyncedShowsButton.Click += new System.EventHandler(this.defineSyncedShowsButton_Click);
            // 
            // logoPanel1
            // 
            this.logoPanel1.Location = new System.Drawing.Point(3, 3);
            this.logoPanel1.Name = "logoPanel1";
            this.logoPanel1.Size = new System.Drawing.Size(205, 57);
            this.logoPanel1.TabIndex = 0;
            this.logoPanel1.Click += new System.EventHandler(this.logoPanel1_Click);
            this.logoPanel1.MouseEnter += new System.EventHandler(this.logoPanel1_MouseEnter);
            this.logoPanel1.MouseLeave += new System.EventHandler(this.logoPanel1_MouseLeave);
            // 
            // FollwitSettingsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.defineSyncedShowsButton);
            this.Controls.Add(this.restrictSyncCheckBox);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.publicProfileCheckBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.syncButton);
            this.Controls.Add(this.accountButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.logoPanel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FollwitSettingsPanel";
            this.Size = new System.Drawing.Size(491, 224);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private global::Follwit.API.UI.Panels.LogoPanel logoPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel userLinkLabel;
        private System.Windows.Forms.Button accountButton;
        private System.Windows.Forms.Button syncButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox publicProfileCheckBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox restrictSyncCheckBox;
        private System.Windows.Forms.Button defineSyncedShowsButton;
    }
}
