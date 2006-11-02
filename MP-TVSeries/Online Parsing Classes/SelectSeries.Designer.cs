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
            this.listItems = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCnl = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listItems
            // 
            this.listItems.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.listItems.FormattingEnabled = true;
            this.listItems.Location = new System.Drawing.Point(12, 38);
            this.listItems.Name = "listItems";
            this.listItems.Size = new System.Drawing.Size(406, 147);
            this.listItems.TabIndex = 0;
            this.listItems.SelectedIndexChanged += new System.EventHandler(this.listItems_SelectedIndexChanged);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 191);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(406, 60);
            this.textBox1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(251, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Please choose the correct series from the list below:";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(343, 257);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCnl
            // 
            this.btnCnl.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCnl.Location = new System.Drawing.Point(262, 257);
            this.btnCnl.Name = "btnCnl";
            this.btnCnl.Size = new System.Drawing.Size(75, 23);
            this.btnCnl.TabIndex = 4;
            this.btnCnl.Text = "Cancel";
            this.btnCnl.UseVisualStyleBackColor = true;
            this.btnCnl.Click += new System.EventHandler(this.btnCnl_Click);
            // 
            // SelectSeries
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCnl;
            this.ClientSize = new System.Drawing.Size(430, 286);
            this.Controls.Add(this.btnCnl);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.listItems);
            this.Name = "SelectSeries";
            this.Text = "Unable to automatically determine correct series";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listItems;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCnl;
    }
}