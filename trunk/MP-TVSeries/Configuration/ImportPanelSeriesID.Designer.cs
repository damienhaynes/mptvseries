namespace WindowPlugins.GUITVSeries.Configuration
{
    partial class ImportPanelSeriesID
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
            this.dataGridViewIdentifySeries = new System.Windows.Forms.DataGridView();
            this.labelStep = new System.Windows.Forms.Label();
            this.labelWaitParse = new System.Windows.Forms.Label();
            this.groupBoxDetails = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.linkLabelCloseDetails = new System.Windows.Forms.LinkLabel();
            this.checkBoxFilter = new System.Windows.Forms.CheckBox();
            this.labelSearchStats = new System.Windows.Forms.Label();
            this.labelImportWizardTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewIdentifySeries)).BeginInit();
            this.groupBoxDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewIdentifySeries
            // 
            this.dataGridViewIdentifySeries.AllowUserToAddRows = false;
            this.dataGridViewIdentifySeries.AllowUserToDeleteRows = false;
            this.dataGridViewIdentifySeries.AllowUserToResizeRows = false;
            this.dataGridViewIdentifySeries.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewIdentifySeries.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewIdentifySeries.CausesValidation = false;
            this.dataGridViewIdentifySeries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewIdentifySeries.Location = new System.Drawing.Point(21, 72);
            this.dataGridViewIdentifySeries.Name = "dataGridViewIdentifySeries";
            this.dataGridViewIdentifySeries.Size = new System.Drawing.Size(531, 308);
            this.dataGridViewIdentifySeries.TabIndex = 0;
            // 
            // labelStep
            // 
            this.labelStep.AutoSize = true;
            this.labelStep.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStep.Location = new System.Drawing.Point(3, 25);
            this.labelStep.Name = "labelStep";
            this.labelStep.Size = new System.Drawing.Size(181, 16);
            this.labelStep.TabIndex = 13;
            this.labelStep.Text = "Step 2 of 4: Identifiying Series";
            // 
            // labelWaitParse
            // 
            this.labelWaitParse.AutoSize = true;
            this.labelWaitParse.Location = new System.Drawing.Point(18, 49);
            this.labelWaitParse.Name = "labelWaitParse";
            this.labelWaitParse.Size = new System.Drawing.Size(259, 13);
            this.labelWaitParse.TabIndex = 12;
            this.labelWaitParse.Text = "Review, change and approve identified Series below.";
            // 
            // groupBoxDetails
            // 
            this.groupBoxDetails.Controls.Add(this.textBox1);
            this.groupBoxDetails.Controls.Add(this.linkLabelCloseDetails);
            this.groupBoxDetails.Location = new System.Drawing.Point(92, 138);
            this.groupBoxDetails.Name = "groupBoxDetails";
            this.groupBoxDetails.Size = new System.Drawing.Size(417, 99);
            this.groupBoxDetails.TabIndex = 14;
            this.groupBoxDetails.TabStop = false;
            this.groupBoxDetails.Text = "Plot";
            this.groupBoxDetails.Visible = false;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BackColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(6, 19);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(405, 61);
            this.textBox1.TabIndex = 1;
            // 
            // linkLabelCloseDetails
            // 
            this.linkLabelCloseDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelCloseDetails.AutoSize = true;
            this.linkLabelCloseDetails.Location = new System.Drawing.Point(378, 83);
            this.linkLabelCloseDetails.Name = "linkLabelCloseDetails";
            this.linkLabelCloseDetails.Size = new System.Drawing.Size(33, 13);
            this.linkLabelCloseDetails.TabIndex = 0;
            this.linkLabelCloseDetails.TabStop = true;
            this.linkLabelCloseDetails.Text = "Close";
            this.linkLabelCloseDetails.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelCloseDetails_LinkClicked);
            // 
            // checkBoxFilter
            // 
            this.checkBoxFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxFilter.AutoSize = true;
            this.checkBoxFilter.Location = new System.Drawing.Point(318, 49);
            this.checkBoxFilter.Name = "checkBoxFilter";
            this.checkBoxFilter.Size = new System.Drawing.Size(234, 17);
            this.checkBoxFilter.TabIndex = 15;
            this.checkBoxFilter.Text = "Show only Series requiring manual Selection";
            this.checkBoxFilter.UseVisualStyleBackColor = true;
            this.checkBoxFilter.CheckedChanged += new System.EventHandler(this.checkBoxFilter_CheckedChanged);
            // 
            // labelSearchStats
            // 
            this.labelSearchStats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelSearchStats.AutoSize = true;
            this.labelSearchStats.Location = new System.Drawing.Point(18, 391);
            this.labelSearchStats.Name = "labelSearchStats";
            this.labelSearchStats.Size = new System.Drawing.Size(70, 13);
            this.labelSearchStats.TabIndex = 16;
            this.labelSearchStats.Text = "Searching for";
            // 
            // labelImportWizardTitle
            // 
            this.labelImportWizardTitle.AutoSize = true;
            this.labelImportWizardTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelImportWizardTitle.Location = new System.Drawing.Point(3, 3);
            this.labelImportWizardTitle.Name = "labelImportWizardTitle";
            this.labelImportWizardTitle.Size = new System.Drawing.Size(234, 16);
            this.labelImportWizardTitle.TabIndex = 17;
            this.labelImportWizardTitle.Text = "Import your Series and Episodes";
            // 
            // ImportPanelSeriesID
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.labelImportWizardTitle);
            this.Controls.Add(this.labelSearchStats);
            this.Controls.Add(this.checkBoxFilter);
            this.Controls.Add(this.groupBoxDetails);
            this.Controls.Add(this.labelStep);
            this.Controls.Add(this.labelWaitParse);
            this.Controls.Add(this.dataGridViewIdentifySeries);
            this.MinimumSize = new System.Drawing.Size(567, 424);
            this.Name = "ImportPanelSeriesID";
            this.Size = new System.Drawing.Size(567, 424);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewIdentifySeries)).EndInit();
            this.groupBoxDetails.ResumeLayout(false);
            this.groupBoxDetails.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewIdentifySeries;
        private System.Windows.Forms.Label labelStep;
        private System.Windows.Forms.Label labelWaitParse;
        private System.Windows.Forms.GroupBox groupBoxDetails;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.LinkLabel linkLabelCloseDetails;
        private System.Windows.Forms.CheckBox checkBoxFilter;
        private System.Windows.Forms.Label labelSearchStats;
        private System.Windows.Forms.Label labelImportWizardTitle;
    }
}
