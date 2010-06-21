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
            this.components = new System.ComponentModel.Container();
            this.labelStep = new System.Windows.Forms.Label();
            this.labelWaitParse = new System.Windows.Forms.Label();
            this.checkBoxFilter = new System.Windows.Forms.CheckBox();
            this.listBoxSeries = new System.Windows.Forms.ListBox();
            this.listBoxLocal = new System.Windows.Forms.ListBox();
            this.listBoxOnline = new System.Windows.Forms.ListBox();
            this.comboMatchOptions = new System.Windows.Forms.ComboBox();
            this.buttonMatchAgain = new System.Windows.Forms.Button();
            this.labelMatchOrder = new System.Windows.Forms.Label();
            this.labelImportWizardTitle = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.toolTipLocalEpisode = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // labelStep
            // 
            this.labelStep.AutoSize = true;
            this.labelStep.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStep.Location = new System.Drawing.Point(3, 25);
            this.labelStep.Name = "labelStep";
            this.labelStep.Size = new System.Drawing.Size(200, 16);
            this.labelStep.TabIndex = 13;
            this.labelStep.Text = "Step 3 of 4: Identifiying Episodes";
            // 
            // labelWaitParse
            // 
            this.labelWaitParse.AutoSize = true;
            this.labelWaitParse.Location = new System.Drawing.Point(18, 49);
            this.labelWaitParse.Name = "labelWaitParse";
            this.labelWaitParse.Size = new System.Drawing.Size(273, 13);
            this.labelWaitParse.TabIndex = 12;
            this.labelWaitParse.Text = "Review, change and approve identified Episodes below.";
            // 
            // checkBoxFilter
            // 
            this.checkBoxFilter.AutoSize = true;
            this.checkBoxFilter.Location = new System.Drawing.Point(6, 472);
            this.checkBoxFilter.Name = "checkBoxFilter";
            this.checkBoxFilter.Size = new System.Drawing.Size(246, 17);
            this.checkBoxFilter.TabIndex = 15;
            this.checkBoxFilter.Text = "&Show only Episodes requiring manual selection";
            this.checkBoxFilter.UseVisualStyleBackColor = true;
            this.checkBoxFilter.CheckedChanged += new System.EventHandler(this.checkBoxFilter_CheckedChanged);
            // 
            // listBoxSeries
            // 
            this.listBoxSeries.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxSeries.FormattingEnabled = true;
            this.listBoxSeries.Location = new System.Drawing.Point(6, 111);
            this.listBoxSeries.Name = "listBoxSeries";
            this.listBoxSeries.Size = new System.Drawing.Size(197, 355);
            this.listBoxSeries.TabIndex = 17;
            this.listBoxSeries.SelectedIndexChanged += new System.EventHandler(this.listBoxSeries_SelectedIndexChanged);
            // 
            // listBoxLocal
            // 
            this.listBoxLocal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxLocal.FormattingEnabled = true;
            this.listBoxLocal.Location = new System.Drawing.Point(209, 111);
            this.listBoxLocal.Name = "listBoxLocal";
            this.listBoxLocal.Size = new System.Drawing.Size(271, 355);
            this.listBoxLocal.TabIndex = 18;
            this.listBoxLocal.SelectedIndexChanged += new System.EventHandler(this.listBoxLocal_SelectedIndexChanged);
            this.listBoxLocal.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listBoxLocal_MouseMove);
            // 
            // listBoxOnline
            // 
            this.listBoxOnline.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxOnline.FormattingEnabled = true;
            this.listBoxOnline.Location = new System.Drawing.Point(486, 111);
            this.listBoxOnline.Name = "listBoxOnline";
            this.listBoxOnline.Size = new System.Drawing.Size(270, 355);
            this.listBoxOnline.TabIndex = 19;
            this.listBoxOnline.SelectedIndexChanged += new System.EventHandler(this.listBoxOnline_SelectedIndexChanged);
            // 
            // comboMatchOptions
            // 
            this.comboMatchOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboMatchOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMatchOptions.FormattingEnabled = true;
            this.comboMatchOptions.Location = new System.Drawing.Point(553, 65);
            this.comboMatchOptions.Name = "comboMatchOptions";
            this.comboMatchOptions.Size = new System.Drawing.Size(121, 21);
            this.comboMatchOptions.TabIndex = 24;
            // 
            // buttonMatchAgain
            // 
            this.buttonMatchAgain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMatchAgain.Location = new System.Drawing.Point(680, 64);
            this.buttonMatchAgain.Name = "buttonMatchAgain";
            this.buttonMatchAgain.Size = new System.Drawing.Size(75, 23);
            this.buttonMatchAgain.TabIndex = 25;
            this.buttonMatchAgain.Text = "&Match";
            this.buttonMatchAgain.UseVisualStyleBackColor = true;
            this.buttonMatchAgain.Click += new System.EventHandler(this.buttonMatchAgain_Click);
            // 
            // labelMatchOrder
            // 
            this.labelMatchOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMatchOrder.AutoSize = true;
            this.labelMatchOrder.Location = new System.Drawing.Point(475, 42);
            this.labelMatchOrder.Name = "labelMatchOrder";
            this.labelMatchOrder.Size = new System.Drawing.Size(280, 13);
            this.labelMatchOrder.TabIndex = 26;
            this.labelMatchOrder.Text = "Auto Match the episodes again, using this ordering option:";
            // 
            // labelImportWizardTitle
            // 
            this.labelImportWizardTitle.AutoSize = true;
            this.labelImportWizardTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelImportWizardTitle.Location = new System.Drawing.Point(3, 3);
            this.labelImportWizardTitle.Name = "labelImportWizardTitle";
            this.labelImportWizardTitle.Size = new System.Drawing.Size(234, 16);
            this.labelImportWizardTitle.TabIndex = 27;
            this.labelImportWizardTitle.Text = "Import your Series and Episodes";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 28;
            this.label1.Text = "Series:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(209, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 29;
            this.label2.Text = "Local Episodes:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(486, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 30;
            this.label3.Text = "Online Episodes:";
            // 
            // toolTipLocalEpisode
            // 
            this.toolTipLocalEpisode.IsBalloon = true;
            this.toolTipLocalEpisode.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipLocalEpisode.ToolTipTitle = "Local Episode Information";
            // 
            // ImportPanelEpID
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelImportWizardTitle);
            this.Controls.Add(this.labelMatchOrder);
            this.Controls.Add(this.buttonMatchAgain);
            this.Controls.Add(this.comboMatchOptions);
            this.Controls.Add(this.listBoxOnline);
            this.Controls.Add(this.listBoxLocal);
            this.Controls.Add(this.listBoxSeries);
            this.Controls.Add(this.checkBoxFilter);
            this.Controls.Add(this.labelStep);
            this.Controls.Add(this.labelWaitParse);
            this.Name = "ImportPanelEpID";
            this.Size = new System.Drawing.Size(771, 504);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelStep;
        private System.Windows.Forms.Label labelWaitParse;
        private System.Windows.Forms.CheckBox checkBoxFilter;
        private System.Windows.Forms.ListBox listBoxSeries;
        private System.Windows.Forms.ListBox listBoxLocal;
        private System.Windows.Forms.ListBox listBoxOnline;
        private System.Windows.Forms.ComboBox comboMatchOptions;
        private System.Windows.Forms.Button buttonMatchAgain;
        private System.Windows.Forms.Label labelMatchOrder;
        private System.Windows.Forms.Label labelImportWizardTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolTip toolTipLocalEpisode;
    }
}
