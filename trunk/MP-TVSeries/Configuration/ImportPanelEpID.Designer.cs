namespace WindowPlugins.GUITVSeries.Configuration
{
    partial class ImportPanelEpID
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
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label_wait_parse = new System.Windows.Forms.Label();
            this.checkBoxFilter = new System.Windows.Forms.CheckBox();
            this.listBoxSeries = new System.Windows.Forms.ListBox();
            this.listBoxLocal = new System.Windows.Forms.ListBox();
            this.listBoxOnline = new System.Windows.Forms.ListBox();
            this.textBoxDetails = new System.Windows.Forms.TextBox();
            this.textDetailsLocal = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboMatchOptions = new System.Windows.Forms.ComboBox();
            this.buttonMatchAgain = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(690, 395);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(771, 395);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Next";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(22, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(200, 16);
            this.label1.TabIndex = 13;
            this.label1.Text = "Step 3 of 4: Identifiying Episodes";
            // 
            // label_wait_parse
            // 
            this.label_wait_parse.AutoSize = true;
            this.label_wait_parse.Location = new System.Drawing.Point(37, 35);
            this.label_wait_parse.Name = "label_wait_parse";
            this.label_wait_parse.Size = new System.Drawing.Size(273, 13);
            this.label_wait_parse.TabIndex = 12;
            this.label_wait_parse.Text = "Review, change and approve identified Episodes below.";
            // 
            // checkBoxFilter
            // 
            this.checkBoxFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxFilter.AutoSize = true;
            this.checkBoxFilter.Location = new System.Drawing.Point(598, 37);
            this.checkBoxFilter.Name = "checkBoxFilter";
            this.checkBoxFilter.Size = new System.Drawing.Size(248, 17);
            this.checkBoxFilter.TabIndex = 15;
            this.checkBoxFilter.Text = "Show only Episodes requiring manual Selection";
            this.checkBoxFilter.UseVisualStyleBackColor = true;
            this.checkBoxFilter.CheckedChanged += new System.EventHandler(this.checkBoxFilter_CheckedChanged);
            // 
            // listBoxSeries
            // 
            this.listBoxSeries.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxSeries.FormattingEnabled = true;
            this.listBoxSeries.Location = new System.Drawing.Point(25, 73);
            this.listBoxSeries.Name = "listBoxSeries";
            this.listBoxSeries.Size = new System.Drawing.Size(197, 316);
            this.listBoxSeries.TabIndex = 17;
            this.listBoxSeries.SelectedIndexChanged += new System.EventHandler(this.listBoxSeries_SelectedIndexChanged);
            // 
            // listBoxLocal
            // 
            this.listBoxLocal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxLocal.FormattingEnabled = true;
            this.listBoxLocal.Location = new System.Drawing.Point(228, 73);
            this.listBoxLocal.Name = "listBoxLocal";
            this.listBoxLocal.Size = new System.Drawing.Size(250, 316);
            this.listBoxLocal.TabIndex = 18;
            this.listBoxLocal.SelectedIndexChanged += new System.EventHandler(this.listBoxLocal_SelectedIndexChanged);
            // 
            // listBoxOnline
            // 
            this.listBoxOnline.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxOnline.FormattingEnabled = true;
            this.listBoxOnline.Location = new System.Drawing.Point(484, 73);
            this.listBoxOnline.Name = "listBoxOnline";
            this.listBoxOnline.Size = new System.Drawing.Size(144, 316);
            this.listBoxOnline.TabIndex = 19;
            this.listBoxOnline.SelectedIndexChanged += new System.EventHandler(this.listBoxOnline_SelectedIndexChanged);
            // 
            // textBoxDetails
            // 
            this.textBoxDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDetails.Location = new System.Drawing.Point(634, 160);
            this.textBoxDetails.Multiline = true;
            this.textBoxDetails.Name = "textBoxDetails";
            this.textBoxDetails.Size = new System.Drawing.Size(212, 229);
            this.textBoxDetails.TabIndex = 20;
            // 
            // textDetailsLocal
            // 
            this.textDetailsLocal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textDetailsLocal.Location = new System.Drawing.Point(634, 78);
            this.textDetailsLocal.Multiline = true;
            this.textDetailsLocal.Name = "textDetailsLocal";
            this.textDetailsLocal.Size = new System.Drawing.Size(212, 51);
            this.textDetailsLocal.TabIndex = 21;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(634, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "FileName:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(634, 139);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "Online Episode:";
            // 
            // comboMatchOptions
            // 
            this.comboMatchOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboMatchOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMatchOptions.FormattingEnabled = true;
            this.comboMatchOptions.Location = new System.Drawing.Point(369, 31);
            this.comboMatchOptions.Name = "comboMatchOptions";
            this.comboMatchOptions.Size = new System.Drawing.Size(121, 21);
            this.comboMatchOptions.TabIndex = 24;
            // 
            // buttonMatchAgain
            // 
            this.buttonMatchAgain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMatchAgain.Location = new System.Drawing.Point(496, 30);
            this.buttonMatchAgain.Name = "buttonMatchAgain";
            this.buttonMatchAgain.Size = new System.Drawing.Size(75, 23);
            this.buttonMatchAgain.TabIndex = 25;
            this.buttonMatchAgain.Text = "Match";
            this.buttonMatchAgain.UseVisualStyleBackColor = true;
            this.buttonMatchAgain.Click += new System.EventHandler(this.buttonMatchAgain_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(366, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(280, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "Auto Match the episodes again, using this ordering option:";
            // 
            // ImportPanelEpID
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonMatchAgain);
            this.Controls.Add(this.comboMatchOptions);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textDetailsLocal);
            this.Controls.Add(this.textBoxDetails);
            this.Controls.Add(this.listBoxOnline);
            this.Controls.Add(this.listBoxLocal);
            this.Controls.Add(this.listBoxSeries);
            this.Controls.Add(this.checkBoxFilter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label_wait_parse);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "ImportPanelEpID";
            this.Size = new System.Drawing.Size(861, 424);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_wait_parse;
        private System.Windows.Forms.CheckBox checkBoxFilter;
        private System.Windows.Forms.ListBox listBoxSeries;
        private System.Windows.Forms.ListBox listBoxLocal;
        private System.Windows.Forms.ListBox listBoxOnline;
        private System.Windows.Forms.TextBox textBoxDetails;
        private System.Windows.Forms.TextBox textDetailsLocal;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboMatchOptions;
        private System.Windows.Forms.Button buttonMatchAgain;
        private System.Windows.Forms.Label label4;
    }
}
