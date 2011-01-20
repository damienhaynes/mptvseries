namespace WindowPlugins.GUITVSeries.Configuration
{
    partial class TraktConfiguration
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
            this.label8 = new System.Windows.Forms.Label();
            this.buttonManualSync = new System.Windows.Forms.Button();
            this.progressTraktSync = new System.Windows.Forms.ProgressBar();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.linkReSync = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonSeriesIgnore = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonTestAPI = new System.Windows.Forms.Button();
            this.linkLabelSignUp = new System.Windows.Forms.LinkLabel();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.groupBox5);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.buttonManualSync);
            this.groupBox1.Controls.Add(this.progressTraktSync);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.linkReSync);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.buttonSeriesIgnore);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.buttonTestAPI);
            this.groupBox1.Controls.Add(this.linkLabelSignUp);
            this.groupBox1.Controls.Add(this.textBoxPassword);
            this.groupBox1.Controls.Add(this.textBoxUsername);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(619, 577);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "trakt Config";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(100, 301);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(436, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "You can choose to synchronize your collection now or let tvseries handle this aut" +
                "omatically:";
            // 
            // buttonManualSync
            // 
            this.buttonManualSync.Location = new System.Drawing.Point(407, 349);
            this.buttonManualSync.Name = "buttonManualSync";
            this.buttonManualSync.Size = new System.Drawing.Size(149, 23);
            this.buttonManualSync.TabIndex = 19;
            this.buttonManualSync.Text = "Manual Synchronize";
            this.buttonManualSync.UseVisualStyleBackColor = true;
            this.buttonManualSync.Click += new System.EventHandler(this.buttonManualSync_Click);
            // 
            // progressTraktSync
            // 
            this.progressTraktSync.Location = new System.Drawing.Point(100, 320);
            this.progressTraktSync.Name = "progressTraktSync";
            this.progressTraktSync.Size = new System.Drawing.Size(456, 23);
            this.progressTraktSync.TabIndex = 18;
            // 
            // groupBox4
            // 
            this.groupBox4.Location = new System.Drawing.Point(101, 287);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(469, 5);
            this.groupBox4.TabIndex = 17;
            this.groupBox4.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(18, 284);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(76, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Synchronize";
            // 
            // linkReSync
            // 
            this.linkReSync.AutoSize = true;
            this.linkReSync.Location = new System.Drawing.Point(98, 258);
            this.linkReSync.Name = "linkReSync";
            this.linkReSync.Size = new System.Drawing.Size(227, 13);
            this.linkReSync.TabIndex = 15;
            this.linkReSync.TabStop = true;
            this.linkReSync.Text = "Yes, re-send my collection on next synchronize";
            this.linkReSync.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkReSync_LinkClicked);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(98, 237);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(402, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Do you want to re-send your library and watched states to trakt on next synchroni" +
                "ze:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(18, 220);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Maintenance";
            // 
            // groupBox3
            // 
            this.groupBox3.Location = new System.Drawing.Point(100, 224);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(469, 5);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(21, 20);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(535, 38);
            this.textBox1.TabIndex = 11;
            this.textBox1.Text = "trakt helps keep a record of what TV shows you are watching. Based on your favori" +
                "tes, trakt recommends additional shows you\'ll enjoy! Watch a TV show in MediaPor" +
                "tal...and trakt builds up your profile!";
            // 
            // buttonSeriesIgnore
            // 
            this.buttonSeriesIgnore.Location = new System.Drawing.Point(101, 193);
            this.buttonSeriesIgnore.Name = "buttonSeriesIgnore";
            this.buttonSeriesIgnore.Size = new System.Drawing.Size(164, 23);
            this.buttonSeriesIgnore.TabIndex = 9;
            this.buttonSeriesIgnore.Text = "&Series to Ignore...";
            this.buttonSeriesIgnore.UseVisualStyleBackColor = true;
            this.buttonSeriesIgnore.Click += new System.EventHandler(this.buttonSeriesIgnore_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(98, 176);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(431, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Select the series you would like to ignore from being synchronized and scrobbled " +
                "on trakt: ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(18, 161);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Privacy";
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(100, 164);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(469, 5);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            // 
            // buttonTestAPI
            // 
            this.buttonTestAPI.Location = new System.Drawing.Point(469, 548);
            this.buttonTestAPI.Name = "buttonTestAPI";
            this.buttonTestAPI.Size = new System.Drawing.Size(100, 23);
            this.buttonTestAPI.TabIndex = 10;
            this.buttonTestAPI.Text = "Test API";
            this.buttonTestAPI.UseVisualStyleBackColor = true;
            this.buttonTestAPI.Visible = false;
            this.buttonTestAPI.Click += new System.EventHandler(this.buttonTestAPI_Click);
            // 
            // linkLabelSignUp
            // 
            this.linkLabelSignUp.AutoSize = true;
            this.linkLabelSignUp.Location = new System.Drawing.Point(362, 140);
            this.linkLabelSignUp.Name = "linkLabelSignUp";
            this.linkLabelSignUp.Size = new System.Drawing.Size(49, 13);
            this.linkLabelSignUp.TabIndex = 5;
            this.linkLabelSignUp.TabStop = true;
            this.linkLabelSignUp.Text = "Signup...";
            this.linkLabelSignUp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelSignUp_LinkClicked);
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(188, 113);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(223, 20);
            this.textBoxPassword.TabIndex = 4;
            this.textBoxPassword.UseSystemPasswordChar = true;
            this.textBoxPassword.Enter += new System.EventHandler(this.textBoxPassword_Enter);
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Location = new System.Drawing.Point(188, 87);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(223, 20);
            this.textBoxUsername.TabIndex = 2;
            this.textBoxUsername.TextChanged += new System.EventHandler(this.textBoxUsername_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(104, 116);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "&Password:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(104, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "&Username:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(18, 70);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Login";
            // 
            // groupBox5
            // 
            this.groupBox5.Location = new System.Drawing.Point(100, 73);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(469, 5);
            this.groupBox5.TabIndex = 22;
            this.groupBox5.TabStop = false;
            // 
            // TraktConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "TraktConfiguration";
            this.Size = new System.Drawing.Size(626, 584);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLabelSignUp;
        private System.Windows.Forms.Button buttonTestAPI;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonSeriesIgnore;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.LinkLabel linkReSync;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonManualSync;
        private System.Windows.Forms.ProgressBar progressTraktSync;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox5;
    }
}
