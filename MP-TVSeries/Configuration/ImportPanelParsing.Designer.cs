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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label_wait_parse = new System.Windows.Forms.Label();
            this.lnkAdd = new System.Windows.Forms.LinkLabel();
            this.groupBoxAddCol = new System.Windows.Forms.GroupBox();
            this.buttonAddColCancel = new System.Windows.Forms.Button();
            this.buttonAddColOK = new System.Windows.Forms.Button();
            this.textBoxAddCol = new System.Windows.Forms.TextBox();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.contextMenuStripChangeCell = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeActionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lnkAddFiles = new System.Windows.Forms.LinkLabel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.checkFilterMan = new System.Windows.Forms.CheckBox();
            this.lblCount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBoxAddCol.SuspendLayout();
            this.contextMenuStripChangeCell.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.GridColor = System.Drawing.Color.White;
            this.dataGridView1.Location = new System.Drawing.Point(40, 60);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.Size = new System.Drawing.Size(673, 263);
            this.dataGridView1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(638, 349);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Next";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(557, 349);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label_wait_parse
            // 
            this.label_wait_parse.AutoSize = true;
            this.label_wait_parse.Location = new System.Drawing.Point(37, 35);
            this.label_wait_parse.Name = "label_wait_parse";
            this.label_wait_parse.Size = new System.Drawing.Size(225, 13);
            this.label_wait_parse.TabIndex = 3;
            this.label_wait_parse.Text = "Please Wait while the files are being Parsed ...";
            // 
            // lnkAdd
            // 
            this.lnkAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkAdd.AutoSize = true;
            this.lnkAdd.Location = new System.Drawing.Point(643, 41);
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
            this.groupBoxAddCol.Controls.Add(this.buttonAddColCancel);
            this.groupBoxAddCol.Controls.Add(this.buttonAddColOK);
            this.groupBoxAddCol.Controls.Add(this.textBoxAddCol);
            this.groupBoxAddCol.Location = new System.Drawing.Point(418, 57);
            this.groupBoxAddCol.Name = "groupBoxAddCol";
            this.groupBoxAddCol.Size = new System.Drawing.Size(295, 85);
            this.groupBoxAddCol.TabIndex = 6;
            this.groupBoxAddCol.TabStop = false;
            this.groupBoxAddCol.Text = "Add a Column";
            this.groupBoxAddCol.Visible = false;
            // 
            // buttonAddColCancel
            // 
            this.buttonAddColCancel.Location = new System.Drawing.Point(125, 55);
            this.buttonAddColCancel.Name = "buttonAddColCancel";
            this.buttonAddColCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonAddColCancel.TabIndex = 2;
            this.buttonAddColCancel.Text = "Cancel";
            this.buttonAddColCancel.UseVisualStyleBackColor = true;
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
            // textBoxAddCol
            // 
            this.textBoxAddCol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAddCol.Location = new System.Drawing.Point(6, 29);
            this.textBoxAddCol.Name = "textBoxAddCol";
            this.textBoxAddCol.Size = new System.Drawing.Size(283, 20);
            this.textBoxAddCol.TabIndex = 0;
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFilter.Location = new System.Drawing.Point(453, 38);
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
            this.lnkAddFiles.Location = new System.Drawing.Point(37, 354);
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
            this.checkFilterMan.Location = new System.Drawing.Point(40, 329);
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
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(563, 326);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(129, 13);
            this.lblCount.TabIndex = 10;
            this.lblCount.Text = "0 Files found (0 displayed)";
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(22, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(343, 16);
            this.label1.TabIndex = 11;
            this.label1.Text = "Step 1 of 3: Review and Change the local File Information";
            // 
            // ImportPanelParsing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.checkFilterMan);
            this.Controls.Add(this.lnkAddFiles);
            this.Controls.Add(this.textBoxFilter);
            this.Controls.Add(this.groupBoxAddCol);
            this.Controls.Add(this.lnkAdd);
            this.Controls.Add(this.label_wait_parse);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "ImportPanelParsing";
            this.Size = new System.Drawing.Size(733, 385);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBoxAddCol.ResumeLayout(false);
            this.groupBoxAddCol.PerformLayout();
            this.contextMenuStripChangeCell.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label_wait_parse;
        private System.Windows.Forms.LinkLabel lnkAdd;
        private System.Windows.Forms.GroupBox groupBoxAddCol;
        private System.Windows.Forms.Button buttonAddColCancel;
        private System.Windows.Forms.Button buttonAddColOK;
        private System.Windows.Forms.TextBox textBoxAddCol;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripChangeCell;
        private System.Windows.Forms.ToolStripMenuItem changeActionToolStripMenuItem;
        private System.Windows.Forms.LinkLabel lnkAddFiles;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.CheckBox checkFilterMan;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Label label1;
    }
}
