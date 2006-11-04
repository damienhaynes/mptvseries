namespace WindowPlugins.GUITVSeries
{
    partial class SelectSeries
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
            this.listbox_Series = new System.Windows.Forms.ListBox();
            this.textbox_Summary = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCnl = new System.Windows.Forms.Button();
            this.btnnever = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label_LocalSeriesName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listbox_Series
            // 
            this.listbox_Series.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.listbox_Series.FormattingEnabled = true;
            this.listbox_Series.Location = new System.Drawing.Point(12, 64);
            this.listbox_Series.Name = "listbox_Series";
            this.listbox_Series.Size = new System.Drawing.Size(406, 95);
            this.listbox_Series.TabIndex = 0;
            this.listbox_Series.SelectedIndexChanged += new System.EventHandler(this.listItems_SelectedIndexChanged);
            // 
            // textbox_Summary
            // 
            this.textbox_Summary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textbox_Summary.Location = new System.Drawing.Point(12, 165);
            this.textbox_Summary.Multiline = true;
            this.textbox_Summary.Name = "textbox_Summary";
            this.textbox_Summary.ReadOnly = true;
            this.textbox_Summary.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textbox_Summary.Size = new System.Drawing.Size(406, 86);
            this.textbox_Summary.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(251, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Please choose the correct series from the list below:";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(343, 257);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCnl
            // 
            this.btnCnl.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCnl.Location = new System.Drawing.Point(247, 257);
            this.btnCnl.Name = "btnCnl";
            this.btnCnl.Size = new System.Drawing.Size(90, 23);
            this.btnCnl.TabIndex = 4;
            this.btnCnl.Text = "Skip this time";
            this.btnCnl.UseVisualStyleBackColor = true;
            this.btnCnl.Click += new System.EventHandler(this.btnCnl_Click);
            // 
            // btnnever
            // 
            this.btnnever.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.btnnever.Location = new System.Drawing.Point(119, 257);
            this.btnnever.Name = "btnnever";
            this.btnnever.Size = new System.Drawing.Size(122, 23);
            this.btnnever.TabIndex = 5;
            this.btnnever.Text = "Skip/Never ask again";
            this.btnnever.UseVisualStyleBackColor = true;
            this.btnnever.Click += new System.EventHandler(this.btnnever_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Local Series Name:";
            // 
            // label_LocalSeriesName
            // 
            this.label_LocalSeriesName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label_LocalSeriesName.Location = new System.Drawing.Point(12, 28);
            this.label_LocalSeriesName.Name = "label_LocalSeriesName";
            this.label_LocalSeriesName.Size = new System.Drawing.Size(406, 18);
            this.label_LocalSeriesName.TabIndex = 7;
            this.label_LocalSeriesName.Text = "label3";
            this.label_LocalSeriesName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SelectSeries
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCnl;
            this.ClientSize = new System.Drawing.Size(430, 286);
            this.Controls.Add(this.label_LocalSeriesName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnnever);
            this.Controls.Add(this.btnCnl);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textbox_Summary);
            this.Controls.Add(this.listbox_Series);
            this.Name = "SelectSeries";
            this.Text = "Unable to automatically determine correct series";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listbox_Series;
        private System.Windows.Forms.TextBox textbox_Summary;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCnl;
        private System.Windows.Forms.Button btnnever;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label_LocalSeriesName;
    }
}