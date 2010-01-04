namespace WindowPlugins.GUITVSeries.Configuration {
	partial class SeriesSelect {
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
            this.checkedListBoxSeries = new System.Windows.Forms.CheckedListBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.labelSeriesSelected = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // checkedListBoxSeries
            // 
            this.checkedListBoxSeries.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBoxSeries.FormattingEnabled = true;
            this.checkedListBoxSeries.Location = new System.Drawing.Point(13, 13);
            this.checkedListBoxSeries.Name = "checkedListBoxSeries";
            this.checkedListBoxSeries.ScrollAlwaysVisible = true;
            this.checkedListBoxSeries.Size = new System.Drawing.Size(245, 334);
            this.checkedListBoxSeries.TabIndex = 0;
            this.checkedListBoxSeries.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxSeries_ItemCheck);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(183, 354);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // labelSeriesSelected
            // 
            this.labelSeriesSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelSeriesSelected.AutoSize = true;
            this.labelSeriesSelected.Location = new System.Drawing.Point(13, 357);
            this.labelSeriesSelected.Name = "labelSeriesSelected";
            this.labelSeriesSelected.Size = new System.Drawing.Size(93, 13);
            this.labelSeriesSelected.TabIndex = 2;
            this.labelSeriesSelected.Text = "0: Series Selected";
            // 
            // SeriesSelect
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(270, 387);
            this.Controls.Add(this.labelSeriesSelected);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.checkedListBoxSeries);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(256, 423);
            this.Name = "SeriesSelect";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add/Remove Series";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckedListBox checkedListBoxSeries;
		private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label labelSeriesSelected;
	}
}