namespace WindowPlugins.GUITVSeries.Configuration
{
    partial class ImportPanelParsing
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewReview = new System.Windows.Forms.DataGridView();
            this.labelWaitParse = new System.Windows.Forms.Label();
            this.lnkAdd = new System.Windows.Forms.LinkLabel();
            this.groupBoxAddCol = new System.Windows.Forms.GroupBox();
            this.comboBoxAddColumn = new System.Windows.Forms.ComboBox();
            this.buttonAddColCancel = new System.Windows.Forms.Button();
            this.buttonAddColOK = new System.Windows.Forms.Button();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.contextMenuStripChangeCell = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeActionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lnkAddFiles = new System.Windows.Forms.LinkLabel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.checkFilterMan = new System.Windows.Forms.CheckBox();
            this.lblCount = new System.Windows.Forms.Label();
            this.labelStep = new System.Windows.Forms.Label();
            this.labelImportWizardTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewReview)).BeginInit();
            this.groupBoxAddCol.SuspendLayout();
            this.contextMenuStripChangeCell.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewReview
            // 
            this.dataGridViewReview.AllowUserToAddRows = false;
            this.dataGridViewReview.AllowUserToDeleteRows = false;
            this.dataGridViewReview.AllowUserToOrderColumns = true;
            this.dataGridViewReview.AllowUserToResizeRows = false;
            this.dataGridViewReview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewReview.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewReview.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewReview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewReview.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewReview.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridViewReview.GridColor = System.Drawing.Color.White;
            this.dataGridViewReview.Location = new System.Drawing.Point(21, 74);
            this.dataGridViewReview.MultiSelect = false;
            this.dataGridViewReview.Name = "dataGridViewReview";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewReview.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewReview.RowHeadersVisible = false;
            this.dataGridViewReview.Size = new System.Drawing.Size(717, 279);
            this.dataGridViewReview.TabIndex = 0;
            // 
            // labelWaitParse
            // 
            this.labelWaitParse.AutoSize = true;
            this.labelWaitParse.Location = new System.Drawing.Point(18, 49);
            this.labelWaitParse.Name = "labelWaitParse";
            this.labelWaitParse.Size = new System.Drawing.Size(225, 13);
            this.labelWaitParse.TabIndex = 3;
            this.labelWaitParse.Text = "Please Wait while the files are being Parsed ...";
            // 
            // lnkAdd
            // 
            this.lnkAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkAdd.AutoSize = true;
            this.lnkAdd.Location = new System.Drawing.Point(489, 51);
            this.lnkAdd.Name = "lnkAdd";
            this.lnkAdd.Size = new System.Drawing.Size(64, 13);
            this.lnkAdd.TabIndex = 4;
            this.lnkAdd.TabStop = true;
            this.lnkAdd.Text = "Add Column";
            this.lnkAdd.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkAdd_LinkClicked);
            // 
            // groupBoxAddCol
            // 
            this.groupBoxAddCol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxAddCol.Controls.Add(this.comboBoxAddColumn);
            this.groupBoxAddCol.Controls.Add(this.buttonAddColCancel);
            this.groupBoxAddCol.Controls.Add(this.buttonAddColOK);
            this.groupBoxAddCol.Location = new System.Drawing.Point(443, 74);
            this.groupBoxAddCol.Name = "groupBoxAddCol";
            this.groupBoxAddCol.Size = new System.Drawing.Size(295, 85);
            this.groupBoxAddCol.TabIndex = 6;
            this.groupBoxAddCol.TabStop = false;
            this.groupBoxAddCol.Text = "Add a Column";
            this.groupBoxAddCol.Visible = false;
            // 
            // comboBoxAddColumn
            // 
            this.comboBoxAddColumn.FormattingEnabled = true;
            this.comboBoxAddColumn.Items.AddRange(new object[] {
            "Series Name",
            "Season Index",
            "Episode Index",
            "Episode Index 2",
            "File Extension",
            "Removable",
            "Episode Name"});
            this.comboBoxAddColumn.Location = new System.Drawing.Point(6, 29);
            this.comboBoxAddColumn.Name = "comboBoxAddColumn";
            this.comboBoxAddColumn.Size = new System.Drawing.Size(277, 21);
            this.comboBoxAddColumn.TabIndex = 3;
            // 
            // buttonAddColCancel
            // 
            this.buttonAddColCancel.Location = new System.Drawing.Point(125, 55);
            this.buttonAddColCancel.Name = "buttonAddColCancel";
            this.buttonAddColCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonAddColCancel.TabIndex = 2;
            this.buttonAddColCancel.Text = "Cancel";
            this.buttonAddColCancel.UseVisualStyleBackColor = true;
            this.buttonAddColCancel.Click += new System.EventHandler(this.buttonAddColCancel_Click);
            // 
            // buttonAddColOK
            // 
            this.buttonAddColOK.Location = new System.Drawing.Point(208, 55);
            this.buttonAddColOK.Name = "buttonAddColOK";
            this.buttonAddColOK.Size = new System.Drawing.Size(75, 23);
            this.buttonAddColOK.TabIndex = 1;
            this.buttonAddColOK.Text = "OK";
            this.buttonAddColOK.UseVisualStyleBackColor = true;
            this.buttonAddColOK.Click += new System.EventHandler(this.buttonAddColOK_Click);
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFilter.Location = new System.Drawing.Point(559, 48);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(179, 20);
            this.textBoxFilter.TabIndex = 7;
            this.textBoxFilter.Text = "Filter by..";
            this.textBoxFilter.TextChanged += new System.EventHandler(this.textBoxFilter_TextChanged);
            this.textBoxFilter.Click += new System.EventHandler(this.textBoxFilter_Click);
            // 
            // contextMenuStripChangeCell
            // 
            this.contextMenuStripChangeCell.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeActionToolStripMenuItem});
            this.contextMenuStripChangeCell.Name = "contextMenuStripChangeCell";
            this.contextMenuStripChangeCell.ShowImageMargin = false;
            this.contextMenuStripChangeCell.Size = new System.Drawing.Size(129, 26);
            this.contextMenuStripChangeCell.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStripChangeCell_ItemClicked);
            this.contextMenuStripChangeCell.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripChangeCell_Opening);
            // 
            // changeActionToolStripMenuItem
            // 
            this.changeActionToolStripMenuItem.Name = "changeActionToolStripMenuItem";
            this.changeActionToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.changeActionToolStripMenuItem.Text = "Change Action";
            // 
            // lnkAddFiles
            // 
            this.lnkAddFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkAddFiles.AutoSize = true;
            this.lnkAddFiles.Location = new System.Drawing.Point(245, 360);
            this.lnkAddFiles.Name = "lnkAddFiles";
            this.lnkAddFiles.Size = new System.Drawing.Size(125, 13);
            this.lnkAddFiles.TabIndex = 8;
            this.lnkAddFiles.TabStop = true;
            this.lnkAddFiles.Text = "Manually Add (a) File(s)...";
            this.lnkAddFiles.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkAddFiles_LinkClicked);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // checkFilterMan
            // 
            this.checkFilterMan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkFilterMan.AutoSize = true;
            this.checkFilterMan.Location = new System.Drawing.Point(21, 359);
            this.checkFilterMan.Name = "checkFilterMan";
            this.checkFilterMan.Size = new System.Drawing.Size(183, 17);
            this.checkFilterMan.TabIndex = 9;
            this.checkFilterMan.Text = "Only display manually added Files";
            this.checkFilterMan.UseVisualStyleBackColor = true;
            this.checkFilterMan.CheckedChanged += new System.EventHandler(this.checkFilterMan_CheckedChanged);
            // 
            // lblCount
            // 
            this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCount.Location = new System.Drawing.Point(434, 356);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(304, 13);
            this.lblCount.TabIndex = 10;
            this.lblCount.Text = "0 Files found (0 displayed)";
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelStep
            // 
            this.labelStep.AutoSize = true;
            this.labelStep.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStep.Location = new System.Drawing.Point(3, 25);
            this.labelStep.Name = "labelStep";
            this.labelStep.Size = new System.Drawing.Size(343, 16);
            this.labelStep.TabIndex = 11;
            this.labelStep.Text = "Step 1 of 4: Review and Change the local File Information";
            // 
            // labelImportWizardTitle
            // 
            this.labelImportWizardTitle.AutoSize = true;
            this.labelImportWizardTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelImportWizardTitle.Location = new System.Drawing.Point(3, 3);
            this.labelImportWizardTitle.Name = "labelImportWizardTitle";
            this.labelImportWizardTitle.Size = new System.Drawing.Size(234, 16);
            this.labelImportWizardTitle.TabIndex = 13;
            this.labelImportWizardTitle.Text = "Import your Series and Episodes";
            // 
            // ImportPanelParsing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.labelImportWizardTitle);
            this.Controls.Add(this.groupBoxAddCol);
            this.Controls.Add(this.labelStep);
            this.Controls.Add(this.labelWaitParse);
            this.Controls.Add(this.checkFilterMan);
            this.Controls.Add(this.textBoxFilter);
            this.Controls.Add(this.dataGridViewReview);
            this.Controls.Add(this.lnkAdd);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.lnkAddFiles);
            this.MinimumSize = new System.Drawing.Size(758, 385);
            this.Name = "ImportPanelParsing";
            this.Size = new System.Drawing.Size(758, 385);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewReview)).EndInit();
            this.groupBoxAddCol.ResumeLayout(false);
            this.contextMenuStripChangeCell.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewReview;
        private System.Windows.Forms.Label labelWaitParse;
        private System.Windows.Forms.LinkLabel lnkAdd;
        private System.Windows.Forms.GroupBox groupBoxAddCol;
        private System.Windows.Forms.Button buttonAddColCancel;
        private System.Windows.Forms.Button buttonAddColOK;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripChangeCell;
        private System.Windows.Forms.ToolStripMenuItem changeActionToolStripMenuItem;
        private System.Windows.Forms.LinkLabel lnkAddFiles;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.CheckBox checkFilterMan;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Label labelStep;
        private System.Windows.Forms.Label labelImportWizardTitle;
        private System.Windows.Forms.ComboBox comboBoxAddColumn;
    }
}
