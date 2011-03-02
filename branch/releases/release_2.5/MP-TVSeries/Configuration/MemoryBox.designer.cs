namespace WindowPlugins.GUITVSeries
{
    partial class MemoryBox
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MemoryBox));
			this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
			this.buttonYes = new System.Windows.Forms.Button();
			this.buttonYestoAll = new System.Windows.Forms.Button();
			this.buttonNo = new System.Windows.Forms.Button();
			this.buttonNotoAll = new System.Windows.Forms.Button();
			this.labelBody = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBoxIcon
			// 
			this.pictureBoxIcon.Location = new System.Drawing.Point(12, 12);
			this.pictureBoxIcon.Name = "pictureBoxIcon";
			this.pictureBoxIcon.Size = new System.Drawing.Size(100, 100);
			this.pictureBoxIcon.TabIndex = 0;
			this.pictureBoxIcon.TabStop = false;
			// 
			// buttonYes
			// 
			this.buttonYes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonYes.Location = new System.Drawing.Point(104, 194);
			this.buttonYes.Name = "buttonYes";
			this.buttonYes.Size = new System.Drawing.Size(75, 23);
			this.buttonYes.TabIndex = 1;
			this.buttonYes.Text = "Yes";
			this.buttonYes.UseVisualStyleBackColor = true;
			this.buttonYes.Click += new System.EventHandler(this.buttonYes_Click);
			// 
			// buttonYestoAll
			// 
			this.buttonYestoAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonYestoAll.Location = new System.Drawing.Point(185, 194);
			this.buttonYestoAll.Name = "buttonYestoAll";
			this.buttonYestoAll.Size = new System.Drawing.Size(75, 23);
			this.buttonYestoAll.TabIndex = 2;
			this.buttonYestoAll.Text = "Yes to All";
			this.buttonYestoAll.UseVisualStyleBackColor = true;
			this.buttonYestoAll.Click += new System.EventHandler(this.buttonYestoAll_Click);
			// 
			// buttonNo
			// 
			this.buttonNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonNo.Location = new System.Drawing.Point(266, 194);
			this.buttonNo.Name = "buttonNo";
			this.buttonNo.Size = new System.Drawing.Size(75, 23);
			this.buttonNo.TabIndex = 3;
			this.buttonNo.Text = "No";
			this.buttonNo.UseVisualStyleBackColor = true;
			this.buttonNo.Click += new System.EventHandler(this.buttonNo_Click);
			// 
			// buttonNotoAll
			// 
			this.buttonNotoAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonNotoAll.Location = new System.Drawing.Point(347, 194);
			this.buttonNotoAll.Name = "buttonNotoAll";
			this.buttonNotoAll.Size = new System.Drawing.Size(75, 23);
			this.buttonNotoAll.TabIndex = 4;
			this.buttonNotoAll.Text = "No to All";
			this.buttonNotoAll.UseVisualStyleBackColor = true;
			this.buttonNotoAll.Click += new System.EventHandler(this.buttonNotoAll_Click);
			// 
			// labelBody
			// 
			this.labelBody.AutoSize = true;
			this.labelBody.Location = new System.Drawing.Point(118, 12);
			this.labelBody.Name = "labelBody";
			this.labelBody.Size = new System.Drawing.Size(272, 169);
			this.labelBody.TabIndex = 6;
			this.labelBody.Text = resources.GetString("labelBody.Text");
			// 
			// MemoryBox
			// 
			this.AcceptButton = this.buttonYes;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(434, 229);
			this.Controls.Add(this.labelBody);
			this.Controls.Add(this.buttonNotoAll);
			this.Controls.Add(this.buttonNo);
			this.Controls.Add(this.buttonYestoAll);
			this.Controls.Add(this.buttonYes);
			this.Controls.Add(this.pictureBoxIcon);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MemoryBox";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MemoryBox";
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.Button buttonYes;
        private System.Windows.Forms.Button buttonYestoAll;
        private System.Windows.Forms.Button buttonNo;
		private System.Windows.Forms.Button buttonNotoAll;
        private System.Windows.Forms.Label labelBody;
    }
}