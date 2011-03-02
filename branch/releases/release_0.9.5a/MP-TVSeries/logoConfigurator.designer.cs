namespace WindowPlugins.GUITVSeries
{
    partial class logoConfigurator
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.browse = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.cond1_what = new System.Windows.Forms.RichTextBox();
            this.cond1_type = new System.Windows.Forms.ComboBox();
            this.cond1_cond = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cond2_cond = new System.Windows.Forms.TextBox();
            this.cond2_type = new System.Windows.Forms.ComboBox();
            this.cond2_what = new System.Windows.Forms.RichTextBox();
            this._12_link = new System.Windows.Forms.ComboBox();
            this._23_link = new System.Windows.Forms.ComboBox();
            this.cond3_cond = new System.Windows.Forms.TextBox();
            this.cond3_type = new System.Windows.Forms.ComboBox();
            this.cond3_what = new System.Windows.Forms.RichTextBox();
            this.save = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDynFilename = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // browse
            // 
            this.browse.Location = new System.Drawing.Point(223, 23);
            this.browse.Name = "browse";
            this.browse.Size = new System.Drawing.Size(64, 23);
            this.browse.TabIndex = 0;
            this.browse.Text = "browse...";
            this.browse.UseVisualStyleBackColor = true;
            this.browse.Click += new System.EventHandler(this.browse_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(30, 23);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(187, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // cond1_what
            // 
            this.cond1_what.Location = new System.Drawing.Point(75, 53);
            this.cond1_what.Name = "cond1_what";
            this.cond1_what.Size = new System.Drawing.Size(142, 20);
            this.cond1_what.TabIndex = 2;
            this.cond1_what.Text = "";
            this.cond1_what.TextChanged += new System.EventHandler(this.cond1_what_TextChanged);
            // 
            // cond1_type
            // 
            this.cond1_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cond1_type.FormattingEnabled = true;
            this.cond1_type.Items.AddRange(new object[] {
            "=",
            "!=",
            ">",
            ">=",
            "<",
            "<=",
            "contains",
            "!contains"});
            this.cond1_type.Location = new System.Drawing.Point(223, 51);
            this.cond1_type.Name = "cond1_type";
            this.cond1_type.Size = new System.Drawing.Size(75, 21);
            this.cond1_type.TabIndex = 3;
            // 
            // cond1_cond
            // 
            this.cond1_cond.Location = new System.Drawing.Point(304, 51);
            this.cond1_cond.Name = "cond1_cond";
            this.cond1_cond.Size = new System.Drawing.Size(130, 20);
            this.cond1_cond.TabIndex = 4;
            this.cond1_cond.TextChanged += new System.EventHandler(this.cond1_cond_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "...if...";
            // 
            // cond2_cond
            // 
            this.cond2_cond.Enabled = false;
            this.cond2_cond.Location = new System.Drawing.Point(304, 78);
            this.cond2_cond.Name = "cond2_cond";
            this.cond2_cond.Size = new System.Drawing.Size(130, 20);
            this.cond2_cond.TabIndex = 8;
            this.cond2_cond.TextChanged += new System.EventHandler(this.cond2_cond_TextChanged);
            // 
            // cond2_type
            // 
            this.cond2_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cond2_type.Enabled = false;
            this.cond2_type.FormattingEnabled = true;
            this.cond2_type.Items.AddRange(new object[] {
            "=",
            "!=",
            ">",
            ">=",
            "<",
            "<=",
            "contains",
            "!contains"});
            this.cond2_type.Location = new System.Drawing.Point(223, 78);
            this.cond2_type.Name = "cond2_type";
            this.cond2_type.Size = new System.Drawing.Size(75, 21);
            this.cond2_type.TabIndex = 7;
            // 
            // cond2_what
            // 
            this.cond2_what.Enabled = false;
            this.cond2_what.Location = new System.Drawing.Point(75, 80);
            this.cond2_what.Name = "cond2_what";
            this.cond2_what.Size = new System.Drawing.Size(142, 20);
            this.cond2_what.TabIndex = 6;
            this.cond2_what.Text = "";
            this.cond2_what.TextChanged += new System.EventHandler(this.cond2_what_TextChanged);
            // 
            // _12_link
            // 
            this._12_link.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._12_link.Enabled = false;
            this._12_link.FormattingEnabled = true;
            this._12_link.Items.AddRange(new object[] {
            "AND",
            "OR"});
            this._12_link.Location = new System.Drawing.Point(12, 80);
            this._12_link.Name = "_12_link";
            this._12_link.Size = new System.Drawing.Size(57, 21);
            this._12_link.TabIndex = 9;
            // 
            // _23_link
            // 
            this._23_link.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._23_link.Enabled = false;
            this._23_link.FormattingEnabled = true;
            this._23_link.Items.AddRange(new object[] {
            "AND",
            "OR"});
            this._23_link.Location = new System.Drawing.Point(12, 106);
            this._23_link.Name = "_23_link";
            this._23_link.Size = new System.Drawing.Size(57, 21);
            this._23_link.TabIndex = 13;
            // 
            // cond3_cond
            // 
            this.cond3_cond.Enabled = false;
            this.cond3_cond.Location = new System.Drawing.Point(304, 104);
            this.cond3_cond.Name = "cond3_cond";
            this.cond3_cond.Size = new System.Drawing.Size(130, 20);
            this.cond3_cond.TabIndex = 12;
            // 
            // cond3_type
            // 
            this.cond3_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cond3_type.Enabled = false;
            this.cond3_type.FormattingEnabled = true;
            this.cond3_type.Items.AddRange(new object[] {
            "=",
            "!=",
            ">",
            ">=",
            "<",
            "<=",
            "contains",
            "!contains"});
            this.cond3_type.Location = new System.Drawing.Point(223, 104);
            this.cond3_type.Name = "cond3_type";
            this.cond3_type.Size = new System.Drawing.Size(75, 21);
            this.cond3_type.TabIndex = 11;
            // 
            // cond3_what
            // 
            this.cond3_what.Enabled = false;
            this.cond3_what.Location = new System.Drawing.Point(75, 106);
            this.cond3_what.Name = "cond3_what";
            this.cond3_what.Size = new System.Drawing.Size(142, 20);
            this.cond3_what.TabIndex = 10;
            this.cond3_what.Text = "";
            this.cond3_what.TextChanged += new System.EventHandler(this.cond3_what_TextChanged);
            // 
            // save
            // 
            this.save.Location = new System.Drawing.Point(357, 131);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(75, 23);
            this.save.TabIndex = 14;
            this.save.Text = "Save";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(276, 131);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 15;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Display the following Logo";
            // 
            // btnDynFilename
            // 
            this.btnDynFilename.Location = new System.Drawing.Point(304, 23);
            this.btnDynFilename.Name = "btnDynFilename";
            this.btnDynFilename.Size = new System.Drawing.Size(126, 23);
            this.btnDynFilename.TabIndex = 17;
            this.btnDynFilename.Text = "dynamic Filename....";
            this.btnDynFilename.UseVisualStyleBackColor = true;
            this.btnDynFilename.Click += new System.EventHandler(this.btnDynFilename_Click);
            // 
            // logoConfigurator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 161);
            this.Controls.Add(this.btnDynFilename);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.save);
            this.Controls.Add(this._23_link);
            this.Controls.Add(this.cond3_cond);
            this.Controls.Add(this.cond3_type);
            this.Controls.Add(this.cond3_what);
            this.Controls.Add(this._12_link);
            this.Controls.Add(this.cond2_cond);
            this.Controls.Add(this.cond2_type);
            this.Controls.Add(this.cond2_what);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cond1_cond);
            this.Controls.Add(this.cond1_type);
            this.Controls.Add(this.cond1_what);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.browse);
            this.Name = "logoConfigurator";
            this.Text = "Edit conditional logo";
            this.Load += new System.EventHandler(this.logoConfigurator_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button browse;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RichTextBox cond1_what;
        private System.Windows.Forms.ComboBox cond1_type;
        private System.Windows.Forms.TextBox cond1_cond;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox cond2_cond;
        private System.Windows.Forms.ComboBox cond2_type;
        private System.Windows.Forms.RichTextBox cond2_what;
        private System.Windows.Forms.ComboBox _12_link;
        private System.Windows.Forms.ComboBox _23_link;
        private System.Windows.Forms.TextBox cond3_cond;
        private System.Windows.Forms.ComboBox cond3_type;
        private System.Windows.Forms.RichTextBox cond3_what;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDynFilename;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}
