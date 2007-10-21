namespace WindowPlugins.GUITVSeries.Local_Parsing_Classes {
    partial class ManualParseDialog {
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
            this.fileLabel = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.fileTextBox = new System.Windows.Forms.TextBox();
            this.selectFileButton = new System.Windows.Forms.Button();
            this.seriesLabel = new System.Windows.Forms.Label();
            this.seriesComboBox = new System.Windows.Forms.ComboBox();
            this.seasonLabel = new System.Windows.Forms.Label();
            this.episodeLabel = new System.Windows.Forms.Label();
            this.episodeComboBox = new System.Windows.Forms.ComboBox();
            this.seasonComboBox = new System.Windows.Forms.ComboBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // fileLabel
            // 
            this.fileLabel.AutoSize = true;
            this.fileLabel.Location = new System.Drawing.Point(13, 13);
            this.fileLabel.Name = "fileLabel";
            this.fileLabel.Size = new System.Drawing.Size(49, 13);
            this.fileLabel.TabIndex = 0;
            this.fileLabel.Text = "Filename";
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "avi";
            this.openFileDialog.FileName = "openFileDialog";
            this.openFileDialog.Filter = "Video Files|*.avi";
            this.openFileDialog.Title = "Video File to Add";
            // 
            // fileTextBox
            // 
            this.fileTextBox.Location = new System.Drawing.Point(16, 30);
            this.fileTextBox.Name = "fileTextBox";
            this.fileTextBox.ReadOnly = true;
            this.fileTextBox.Size = new System.Drawing.Size(239, 20);
            this.fileTextBox.TabIndex = 1;
            // 
            // selectFileButton
            // 
            this.selectFileButton.Location = new System.Drawing.Point(261, 30);
            this.selectFileButton.Name = "selectFileButton";
            this.selectFileButton.Size = new System.Drawing.Size(85, 23);
            this.selectFileButton.TabIndex = 2;
            this.selectFileButton.Text = "Browse...";
            this.selectFileButton.UseVisualStyleBackColor = true;
            this.selectFileButton.Click += new System.EventHandler(this.selectFileButton_click);
            // 
            // seriesLabel
            // 
            this.seriesLabel.AutoSize = true;
            this.seriesLabel.Location = new System.Drawing.Point(13, 71);
            this.seriesLabel.Name = "seriesLabel";
            this.seriesLabel.Size = new System.Drawing.Size(36, 13);
            this.seriesLabel.TabIndex = 3;
            this.seriesLabel.Text = "Series";
            // 
            // seriesComboBox
            // 
            this.seriesComboBox.FormattingEnabled = true;
            this.seriesComboBox.Location = new System.Drawing.Point(16, 86);
            this.seriesComboBox.Name = "seriesComboBox";
            this.seriesComboBox.Size = new System.Drawing.Size(274, 21);
            this.seriesComboBox.Sorted = true;
            this.seriesComboBox.TabIndex = 4;
            this.seriesComboBox.SelectedIndexChanged += new System.EventHandler(this.seriesComboBox_SelectedIndexChanged);
            this.seriesComboBox.LostFocus += new System.EventHandler(this.seriesComboBox_LostFocus);
            this.seriesComboBox.GotFocus += new System.EventHandler(this.seriesComboBox_GotFocus);
            this.seriesComboBox.TextChanged += new System.EventHandler(this.seriesComboBox_TextChanged);
            // 
            // seasonLabel
            // 
            this.seasonLabel.AutoSize = true;
            this.seasonLabel.Location = new System.Drawing.Point(293, 71);
            this.seasonLabel.Name = "seasonLabel";
            this.seasonLabel.Size = new System.Drawing.Size(53, 13);
            this.seasonLabel.TabIndex = 7;
            this.seasonLabel.Text = "Season #";
            // 
            // episodeLabel
            // 
            this.episodeLabel.AutoSize = true;
            this.episodeLabel.Location = new System.Drawing.Point(13, 110);
            this.episodeLabel.Name = "episodeLabel";
            this.episodeLabel.Size = new System.Drawing.Size(45, 13);
            this.episodeLabel.TabIndex = 9;
            this.episodeLabel.Text = "Episode";
            // 
            // episodeComboBox
            // 
            this.episodeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.episodeComboBox.Enabled = false;
            this.episodeComboBox.FormattingEnabled = true;
            this.episodeComboBox.Location = new System.Drawing.Point(16, 127);
            this.episodeComboBox.Name = "episodeComboBox";
            this.episodeComboBox.Size = new System.Drawing.Size(330, 190);
            this.episodeComboBox.TabIndex = 10;
            this.episodeComboBox.SelectedIndexChanged += new System.EventHandler(this.episodeComboBox_SelectedIndexChanged);
            this.episodeComboBox.TextChanged += new System.EventHandler(this.episodeComboBox_TextChanged);
            this.episodeComboBox.DropDownClosed += new System.EventHandler(this.episodeComboBox_DropDownClosed);
            // 
            // seasonComboBox
            // 
            this.seasonComboBox.Enabled = false;
            this.seasonComboBox.FormattingEnabled = true;
            this.seasonComboBox.Location = new System.Drawing.Point(296, 86);
            this.seasonComboBox.Name = "seasonComboBox";
            this.seasonComboBox.Size = new System.Drawing.Size(50, 21);
            this.seasonComboBox.TabIndex = 11;
            this.seasonComboBox.SelectedIndexChanged += new System.EventHandler(this.seasonComboBox_SelectedIndexChanged);
            // 
            // okButton
            // 
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(181, 322);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(84, 23);
            this.okButton.TabIndex = 12;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(271, 322);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 13;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ManualParseDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 357);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.seasonComboBox);
            this.Controls.Add(this.episodeComboBox);
            this.Controls.Add(this.episodeLabel);
            this.Controls.Add(this.seasonLabel);
            this.Controls.Add(this.seriesComboBox);
            this.Controls.Add(this.seriesLabel);
            this.Controls.Add(this.selectFileButton);
            this.Controls.Add(this.fileTextBox);
            this.Controls.Add(this.fileLabel);
            this.Name = "ManualParseDialog";
            this.Text = "Manual Episode Entry";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label fileLabel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TextBox fileTextBox;
        private System.Windows.Forms.Button selectFileButton;
        private System.Windows.Forms.Label seriesLabel;
        private System.Windows.Forms.ComboBox seriesComboBox;
        private System.Windows.Forms.Label seasonLabel;
        private System.Windows.Forms.Label episodeLabel;
        private System.Windows.Forms.ComboBox episodeComboBox;
        private System.Windows.Forms.ComboBox seasonComboBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}