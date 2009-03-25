namespace WindowPlugins.GUITVSeries.Configuration
{
    partial class DatabaseConfigurator
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
            if (disposing && (components != null)) {
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
            this.textBox_dblocation = new System.Windows.Forms.TextBox();
            this.button_dbbrowse = new System.Windows.Forms.Button();
            this.label28 = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.textBox_username = new System.Windows.Forms.TextBox();
            this.textBox_password = new System.Windows.Forms.TextBox();
            this.textBox_server = new System.Windows.Forms.TextBox();
            this.textBox_database = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button_create = new System.Windows.Forms.Button();
            this.button_test = new System.Windows.Forms.Button();
            this.button_ok = new System.Windows.Forms.Button();
            this.radio_sqlite = new System.Windows.Forms.RadioButton();
            this.radio_sqlclient = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox_dblocation
            // 
            this.textBox_dblocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_dblocation.Location = new System.Drawing.Point(120, 15);
            this.textBox_dblocation.Name = "textBox_dblocation";
            this.textBox_dblocation.ReadOnly = true;
            this.textBox_dblocation.Size = new System.Drawing.Size(280, 20);
            this.textBox_dblocation.TabIndex = 2;
            // 
            // button_dbbrowse
            // 
            this.button_dbbrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_dbbrowse.Location = new System.Drawing.Point(406, 13);
            this.button_dbbrowse.Name = "button_dbbrowse";
            this.button_dbbrowse.Size = new System.Drawing.Size(26, 23);
            this.button_dbbrowse.TabIndex = 3;
            this.button_dbbrowse.Text = "...";
            this.button_dbbrowse.UseVisualStyleBackColor = true;
            this.button_dbbrowse.Click += new System.EventHandler(this.button_dbbrowse_Click);
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(18, 12);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(96, 13);
            this.label28.TabIndex = 0;
            this.label28.Text = "Database location:";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // textBox_username
            // 
            this.textBox_username.Enabled = false;
            this.textBox_username.Location = new System.Drawing.Point(124, 15);
            this.textBox_username.Name = "textBox_username";
            this.textBox_username.Size = new System.Drawing.Size(310, 20);
            this.textBox_username.TabIndex = 1;
            this.textBox_username.Text = "sa";
            // 
            // textBox_password
            // 
            this.textBox_password.Enabled = false;
            this.textBox_password.Location = new System.Drawing.Point(124, 55);
            this.textBox_password.Name = "textBox_password";
            this.textBox_password.Size = new System.Drawing.Size(310, 20);
            this.textBox_password.TabIndex = 3;
            this.textBox_password.Text = "mediaportal";
            // 
            // textBox_server
            // 
            this.textBox_server.Enabled = false;
            this.textBox_server.Location = new System.Drawing.Point(124, 91);
            this.textBox_server.Name = "textBox_server";
            this.textBox_server.Size = new System.Drawing.Size(310, 20);
            this.textBox_server.TabIndex = 5;
            this.textBox_server.Text = "(localhost)";
            // 
            // textBox_database
            // 
            this.textBox_database.Enabled = false;
            this.textBox_database.Location = new System.Drawing.Point(124, 129);
            this.textBox_database.Name = "textBox_database";
            this.textBox_database.Size = new System.Drawing.Size(312, 20);
            this.textBox_database.TabIndex = 7;
            this.textBox_database.Text = "MpTvSeriesDb";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(55, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "User Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(62, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Database Server:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 132);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Database Name:";
            // 
            // button_create
            // 
            this.button_create.Enabled = false;
            this.button_create.Location = new System.Drawing.Point(280, 155);
            this.button_create.Name = "button_create";
            this.button_create.Size = new System.Drawing.Size(75, 23);
            this.button_create.TabIndex = 8;
            this.button_create.Text = "Create";
            this.button_create.UseVisualStyleBackColor = true;
            this.button_create.Click += new System.EventHandler(this.button_create_Click);
            // 
            // button_test
            // 
            this.button_test.Enabled = false;
            this.button_test.Location = new System.Drawing.Point(361, 155);
            this.button_test.Name = "button_test";
            this.button_test.Size = new System.Drawing.Size(75, 23);
            this.button_test.TabIndex = 9;
            this.button_test.Text = "Test";
            this.button_test.UseVisualStyleBackColor = true;
            this.button_test.Click += new System.EventHandler(this.button_test_Click);
            // 
            // button_ok
            // 
            this.button_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ok.Location = new System.Drawing.Point(374, 289);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(75, 23);
            this.button_ok.TabIndex = 6;
            this.button_ok.Text = "Close";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // radio_sqlite
            // 
            this.radio_sqlite.AutoSize = true;
            this.radio_sqlite.Checked = true;
            this.radio_sqlite.Location = new System.Drawing.Point(17, 12);
            this.radio_sqlite.Name = "radio_sqlite";
            this.radio_sqlite.Size = new System.Drawing.Size(79, 17);
            this.radio_sqlite.TabIndex = 0;
            this.radio_sqlite.TabStop = true;
            this.radio_sqlite.Text = "Use SQLite";
            this.radio_sqlite.UseVisualStyleBackColor = true;
            // 
            // radio_sqlclient
            // 
            this.radio_sqlclient.AutoSize = true;
            this.radio_sqlclient.Location = new System.Drawing.Point(17, 80);
            this.radio_sqlclient.Name = "radio_sqlclient";
            this.radio_sqlclient.Size = new System.Drawing.Size(86, 17);
            this.radio_sqlclient.TabIndex = 2;
            this.radio_sqlclient.Text = "Use Sql2005";
            this.radio_sqlclient.UseVisualStyleBackColor = true;
            this.radio_sqlclient.CheckedChanged += new System.EventHandler(this.radio_sqlclient_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBox_dblocation);
            this.groupBox1.Controls.Add(this.button_dbbrowse);
            this.groupBox1.Controls.Add(this.label28);
            this.groupBox1.Location = new System.Drawing.Point(17, 30);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(440, 44);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "(restart needed)";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textBox_username);
            this.groupBox2.Controls.Add(this.textBox_password);
            this.groupBox2.Controls.Add(this.button_test);
            this.groupBox2.Controls.Add(this.button_create);
            this.groupBox2.Controls.Add(this.textBox_server);
            this.groupBox2.Controls.Add(this.textBox_database);
            this.groupBox2.Location = new System.Drawing.Point(13, 93);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(444, 190);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            // 
            // DatabaseConfigurator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(465, 314);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.radio_sqlclient);
            this.Controls.Add(this.radio_sqlite);
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "DatabaseConfigurator";
            this.Text = "Database Configuratoration";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_dblocation;
        private System.Windows.Forms.Button button_dbbrowse;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TextBox textBox_username;
        private System.Windows.Forms.TextBox textBox_password;
        private System.Windows.Forms.TextBox textBox_server;
        private System.Windows.Forms.TextBox textBox_database;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button_create;
        private System.Windows.Forms.Button button_test;
        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.RadioButton radio_sqlite;
        private System.Windows.Forms.RadioButton radio_sqlclient;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
    }
}