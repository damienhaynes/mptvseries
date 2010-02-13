namespace WindowPlugins.GUITVSeries.Configuration {
    partial class DeleteDialog {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.radioDeleteFromDiskAndDatabase = new System.Windows.Forms.RadioButton();
            this.radioDeleteFromDatabase = new System.Windows.Forms.RadioButton();
            this.radioDeleteFromDisk = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioDeleteFromDiskAndDatabase
            // 
            this.radioDeleteFromDiskAndDatabase.AutoSize = true;
            this.radioDeleteFromDiskAndDatabase.Location = new System.Drawing.Point(25, 104);
            this.radioDeleteFromDiskAndDatabase.Name = "radioDeleteFromDiskAndDatabase";
            this.radioDeleteFromDiskAndDatabase.Size = new System.Drawing.Size(173, 17);
            this.radioDeleteFromDiskAndDatabase.TabIndex = 4;
            this.radioDeleteFromDiskAndDatabase.TabStop = true;
            this.radioDeleteFromDiskAndDatabase.Text = "Delete from Disk and Database";
            this.radioDeleteFromDiskAndDatabase.UseVisualStyleBackColor = true;
            this.radioDeleteFromDiskAndDatabase.Click += new System.EventHandler(this.radioDeleteFromDiskAndDatabase_Click);
            // 
            // radioDeleteFromDatabase
            // 
            this.radioDeleteFromDatabase.AutoSize = true;
            this.radioDeleteFromDatabase.Location = new System.Drawing.Point(25, 31);
            this.radioDeleteFromDatabase.Name = "radioDeleteFromDatabase";
            this.radioDeleteFromDatabase.Size = new System.Drawing.Size(128, 17);
            this.radioDeleteFromDatabase.TabIndex = 2;
            this.radioDeleteFromDatabase.TabStop = true;
            this.radioDeleteFromDatabase.Text = "Delete from Database";
            this.radioDeleteFromDatabase.UseVisualStyleBackColor = true;
            this.radioDeleteFromDatabase.Click += new System.EventHandler(this.radioDeleteFromDatabase_Click);
            // 
            // radioDeleteFromDisk
            // 
            this.radioDeleteFromDisk.AutoSize = true;
            this.radioDeleteFromDisk.Location = new System.Drawing.Point(25, 68);
            this.radioDeleteFromDisk.Name = "radioDeleteFromDisk";
            this.radioDeleteFromDisk.Size = new System.Drawing.Size(103, 17);
            this.radioDeleteFromDisk.TabIndex = 3;
            this.radioDeleteFromDisk.TabStop = true;
            this.radioDeleteFromDisk.Text = "Delete from Disk";
            this.radioDeleteFromDisk.UseVisualStyleBackColor = true;
            this.radioDeleteFromDisk.Click += new System.EventHandler(this.radioDeleteFromDisk_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioDeleteFromDatabase);
            this.groupBox1.Controls.Add(this.radioDeleteFromDiskAndDatabase);
            this.groupBox1.Controls.Add(this.radioDeleteFromDisk);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(255, 152);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select Choice:";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(192, 171);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(111, 171);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // DeleteDialog
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(279, 203);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeleteDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Delete";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radioDeleteFromDiskAndDatabase;
        private System.Windows.Forms.RadioButton radioDeleteFromDatabase;
        private System.Windows.Forms.RadioButton radioDeleteFromDisk;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
    }
}