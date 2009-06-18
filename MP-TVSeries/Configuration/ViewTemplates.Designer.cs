namespace WindowPlugins.GUITVSeries.Configuration {
    partial class ViewTemplates {
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
            this.listBoxViewTemplate = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxViewDescription = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxViewTemplate
            // 
            this.listBoxViewTemplate.FormattingEnabled = true;
            this.listBoxViewTemplate.Location = new System.Drawing.Point(6, 19);
            this.listBoxViewTemplate.Name = "listBoxViewTemplate";
            this.listBoxViewTemplate.ScrollAlwaysVisible = true;
            this.listBoxViewTemplate.Size = new System.Drawing.Size(248, 121);
            this.listBoxViewTemplate.TabIndex = 0;
            this.listBoxViewTemplate.SelectedIndexChanged += new System.EventHandler(this.listBoxViewTemplate_SelectedIndexChanged);
            this.listBoxViewTemplate.DoubleClick += new System.EventHandler(this.listBoxViewTemplate_DoubleClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxViewDescription);
            this.groupBox1.Controls.Add(this.listBoxViewTemplate);
            this.groupBox1.Location = new System.Drawing.Point(12, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 221);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select a Template";
            // 
            // textBoxViewDescription
            // 
            this.textBoxViewDescription.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxViewDescription.Location = new System.Drawing.Point(6, 146);
            this.textBoxViewDescription.Multiline = true;
            this.textBoxViewDescription.Name = "textBoxViewDescription";
            this.textBoxViewDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxViewDescription.Size = new System.Drawing.Size(248, 64);
            this.textBoxViewDescription.TabIndex = 4;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(197, 229);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(116, 229);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // ViewTemplates
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(284, 258);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewTemplates";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "View Templates";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxViewTemplate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TextBox textBoxViewDescription;
    }
}