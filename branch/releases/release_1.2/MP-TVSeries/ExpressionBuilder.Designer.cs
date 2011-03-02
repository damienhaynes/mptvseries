namespace WindowPlugins.GUITVSeries
{
    partial class ExpressionBuilder
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
            this.ddTokens = new System.Windows.Forms.ComboBox();
            this.panOptions = new System.Windows.Forms.Panel();
            this.btnReplace = new System.Windows.Forms.Button();
            this.btnInsert = new System.Windows.Forms.Button();
            this.panConstraint = new System.Windows.Forms.Panel();
            this.txtConstraint = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.txtExpression = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panOptions.SuspendLayout();
            this.panConstraint.SuspendLayout();
            this.SuspendLayout();
            // 
            // ddTokens
            // 
            this.ddTokens.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddTokens.Items.AddRange(new object[] {
            "Series",
            "Season",
            "Episode",
            "Episode 2",
            "Title"});
            this.ddTokens.Location = new System.Drawing.Point(8, 3);
            this.ddTokens.MaxDropDownItems = 9;
            this.ddTokens.Name = "ddTokens";
            this.ddTokens.Size = new System.Drawing.Size(121, 21);
            this.ddTokens.TabIndex = 1;
            this.ddTokens.SelectedIndexChanged += new System.EventHandler(this.ddTokens_SelectedIndexChanged);
            // 
            // panOptions
            // 
            this.panOptions.Controls.Add(this.btnReplace);
            this.panOptions.Controls.Add(this.btnInsert);
            this.panOptions.Controls.Add(this.panConstraint);
            this.panOptions.Location = new System.Drawing.Point(135, 1);
            this.panOptions.Name = "panOptions";
            this.panOptions.Size = new System.Drawing.Size(275, 24);
            this.panOptions.TabIndex = 2;
            // 
            // btnReplace
            // 
            this.btnReplace.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnReplace.Location = new System.Drawing.Point(216, 0);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(56, 24);
            this.btnReplace.TabIndex = 6;
            this.btnReplace.Text = "Replace";
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // btnInsert
            // 
            this.btnInsert.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnInsert.Location = new System.Drawing.Point(160, 0);
            this.btnInsert.Name = "btnInsert";
            this.btnInsert.Size = new System.Drawing.Size(56, 24);
            this.btnInsert.TabIndex = 3;
            this.btnInsert.Text = "Insert";
            this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
            // 
            // panConstraint
            // 
            this.panConstraint.Controls.Add(this.txtConstraint);
            this.panConstraint.Controls.Add(this.lblDescription);
            this.panConstraint.Dock = System.Windows.Forms.DockStyle.Left;
            this.panConstraint.Location = new System.Drawing.Point(0, 0);
            this.panConstraint.Name = "panConstraint";
            this.panConstraint.Size = new System.Drawing.Size(160, 24);
            this.panConstraint.TabIndex = 5;
            // 
            // txtConstraint
            // 
            this.txtConstraint.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtConstraint.Location = new System.Drawing.Point(88, 0);
            this.txtConstraint.Name = "txtConstraint";
            this.txtConstraint.Size = new System.Drawing.Size(64, 20);
            this.txtConstraint.TabIndex = 1;
            // 
            // lblDescription
            // 
            this.lblDescription.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblDescription.Location = new System.Drawing.Point(0, 0);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(88, 24);
            this.lblDescription.TabIndex = 0;
            this.lblDescription.Text = "Label1";
            this.lblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(413, 1);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(90, 23);
            this.btnImport.TabIndex = 5;
            this.btnImport.Text = "Import Format";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(347, 56);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtExpression
            // 
            this.txtExpression.HideSelection = false;
            this.txtExpression.Location = new System.Drawing.Point(8, 30);
            this.txtExpression.Name = "txtExpression";
            this.txtExpression.Size = new System.Drawing.Size(495, 20);
            this.txtExpression.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(428, 56);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ExpressionBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 85);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.panOptions);
            this.Controls.Add(this.ddTokens);
            this.Controls.Add(this.txtExpression);
            this.Name = "ExpressionBuilder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Simple Expression Builder";
            this.panOptions.ResumeLayout(false);
            this.panConstraint.ResumeLayout(false);
            this.panConstraint.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ddTokens;
        private System.Windows.Forms.Panel panOptions;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtConstraint;
        private System.Windows.Forms.Button btnInsert;
        private System.Windows.Forms.Panel panConstraint;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtExpression;
        private System.Windows.Forms.Button btnCancel;
    }
}