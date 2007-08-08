#region GNU license
// MP-TVSeries - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2007
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#endregion


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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(logoConfigurator));
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDynFilename = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnPaste = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.toolTipLogos = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipLogos2 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // browse
            // 
            this.browse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.browse.Location = new System.Drawing.Point(245, 27);
            this.browse.Name = "browse";
            this.browse.Size = new System.Drawing.Size(75, 23);
            this.browse.TabIndex = 2;
            this.browse.Text = "&Browse...";
            this.toolTipLogos.SetToolTip(this.browse, "Click to browse for a logo image to be used with this rule.");
            this.browse.UseVisualStyleBackColor = true;
            this.browse.Click += new System.EventHandler(this.browse_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(15, 29);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(224, 20);
            this.textBox1.TabIndex = 1;
            this.toolTipLogos2.SetToolTip(this.textBox1, "This contains the filename of the logo that the rule will load if condition is sa" +
                    "tisfied.\r\nWhen a dynamic rule is applied, enter in the dynamic filename after th" +
                    "e path chosen");
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // cond1_what
            // 
            this.cond1_what.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cond1_what.Location = new System.Drawing.Point(75, 55);
            this.cond1_what.Multiline = false;
            this.cond1_what.Name = "cond1_what";
            this.cond1_what.Size = new System.Drawing.Size(164, 20);
            this.cond1_what.TabIndex = 4;
            this.cond1_what.Text = "";
            this.toolTipLogos2.SetToolTip(this.cond1_what, "Enter in the L.H.S of the expression to be evaluated.\r\nYou can right click on thi" +
                    "s textbox to bring up a menu of available fields from the database");
            this.cond1_what.TextChanged += new System.EventHandler(this.cond1_what_TextChanged);
            // 
            // cond1_type
            // 
            this.cond1_type.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
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
            this.cond1_type.Location = new System.Drawing.Point(245, 55);
            this.cond1_type.Name = "cond1_type";
            this.cond1_type.Size = new System.Drawing.Size(75, 21);
            this.cond1_type.TabIndex = 5;
            this.toolTipLogos2.SetToolTip(this.cond1_type, resources.GetString("cond1_type.ToolTip"));
            // 
            // cond1_cond
            // 
            this.cond1_cond.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cond1_cond.Location = new System.Drawing.Point(326, 55);
            this.cond1_cond.Name = "cond1_cond";
            this.cond1_cond.Size = new System.Drawing.Size(130, 20);
            this.cond1_cond.TabIndex = 6;
            this.toolTipLogos2.SetToolTip(this.cond1_cond, resources.GetString("cond1_cond.ToolTip"));
            this.cond1_cond.TextChanged += new System.EventHandler(this.cond1_cond_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "...if...";
            // 
            // cond2_cond
            // 
            this.cond2_cond.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cond2_cond.Enabled = false;
            this.cond2_cond.Location = new System.Drawing.Point(326, 82);
            this.cond2_cond.Name = "cond2_cond";
            this.cond2_cond.Size = new System.Drawing.Size(130, 20);
            this.cond2_cond.TabIndex = 11;
            this.cond2_cond.TextChanged += new System.EventHandler(this.cond2_cond_TextChanged);
            // 
            // cond2_type
            // 
            this.cond2_type.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
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
            this.cond2_type.Location = new System.Drawing.Point(245, 82);
            this.cond2_type.Name = "cond2_type";
            this.cond2_type.Size = new System.Drawing.Size(75, 21);
            this.cond2_type.TabIndex = 10;
            // 
            // cond2_what
            // 
            this.cond2_what.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cond2_what.Enabled = false;
            this.cond2_what.Location = new System.Drawing.Point(75, 82);
            this.cond2_what.Multiline = false;
            this.cond2_what.Name = "cond2_what";
            this.cond2_what.Size = new System.Drawing.Size(164, 20);
            this.cond2_what.TabIndex = 9;
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
            this._12_link.Location = new System.Drawing.Point(12, 82);
            this._12_link.Name = "_12_link";
            this._12_link.Size = new System.Drawing.Size(57, 21);
            this._12_link.TabIndex = 8;
            this.toolTipLogos2.SetToolTip(this._12_link, resources.GetString("_12_link.ToolTip"));
            // 
            // _23_link
            // 
            this._23_link.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._23_link.Enabled = false;
            this._23_link.FormattingEnabled = true;
            this._23_link.Items.AddRange(new object[] {
            "AND",
            "OR"});
            this._23_link.Location = new System.Drawing.Point(12, 108);
            this._23_link.Name = "_23_link";
            this._23_link.Size = new System.Drawing.Size(57, 21);
            this._23_link.TabIndex = 12;
            // 
            // cond3_cond
            // 
            this.cond3_cond.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cond3_cond.Enabled = false;
            this.cond3_cond.Location = new System.Drawing.Point(326, 108);
            this.cond3_cond.Name = "cond3_cond";
            this.cond3_cond.Size = new System.Drawing.Size(130, 20);
            this.cond3_cond.TabIndex = 15;
            // 
            // cond3_type
            // 
            this.cond3_type.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
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
            this.cond3_type.Location = new System.Drawing.Point(245, 108);
            this.cond3_type.Name = "cond3_type";
            this.cond3_type.Size = new System.Drawing.Size(75, 21);
            this.cond3_type.TabIndex = 14;
            // 
            // cond3_what
            // 
            this.cond3_what.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cond3_what.Enabled = false;
            this.cond3_what.Location = new System.Drawing.Point(75, 108);
            this.cond3_what.Multiline = false;
            this.cond3_what.Name = "cond3_what";
            this.cond3_what.Size = new System.Drawing.Size(164, 20);
            this.cond3_what.TabIndex = 13;
            this.cond3_what.Text = "";
            this.cond3_what.TextChanged += new System.EventHandler(this.cond3_what_TextChanged);
            // 
            // save
            // 
            this.save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.save.Location = new System.Drawing.Point(381, 135);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(75, 23);
            this.save.TabIndex = 19;
            this.save.Text = "&Save";
            this.toolTipLogos.SetToolTip(this.save, "Click to close this dialog and save the logo rule, you can also press the <ENTER>" +
                    " key");
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(300, 135);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "Ca&ncel";
            this.toolTipLogos.SetToolTip(this.btnCancel, "Click to close this dialog without saving logo rule, you can also press the <ESC>" +
                    " key");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Display the following Logo:";
            // 
            // btnDynFilename
            // 
            this.btnDynFilename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDynFilename.Location = new System.Drawing.Point(326, 27);
            this.btnDynFilename.Name = "btnDynFilename";
            this.btnDynFilename.Size = new System.Drawing.Size(130, 23);
            this.btnDynFilename.TabIndex = 3;
            this.btnDynFilename.Text = "&Dynamic Filename....";
            this.toolTipLogos2.SetToolTip(this.btnDynFilename, resources.GetString("btnDynFilename.ToolTip"));
            this.btnDynFilename.UseVisualStyleBackColor = true;
            this.btnDynFilename.Click += new System.EventHandler(this.btnDynFilename_Click);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Enter in a path that contains the dynamic files for this logo rule.";
            // 
            // btnPaste
            // 
            this.btnPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPaste.Location = new System.Drawing.Point(93, 134);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(75, 23);
            this.btnPaste.TabIndex = 17;
            this.btnPaste.Text = "&Paste";
            this.toolTipLogos.SetToolTip(this.btnPaste, "Click to Paste contents of clipboard to create a logo rule");
            this.btnPaste.UseVisualStyleBackColor = true;
            this.btnPaste.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCopy.Location = new System.Drawing.Point(12, 134);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 23);
            this.btnCopy.TabIndex = 16;
            this.btnCopy.Text = "&Copy";
            this.toolTipLogos.SetToolTip(this.btnCopy, "Click to Copy the Logo rule to the clipboard");
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // toolTipLogos
            // 
            this.toolTipLogos.AutoPopDelay = 5000;
            this.toolTipLogos.InitialDelay = 500;
            this.toolTipLogos.ReshowDelay = 100;
            // 
            // toolTipLogos2
            // 
            this.toolTipLogos2.AutoPopDelay = 10000;
            this.toolTipLogos2.InitialDelay = 500;
            this.toolTipLogos2.IsBalloon = true;
            this.toolTipLogos2.ReshowDelay = 100;
            this.toolTipLogos2.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipLogos2.ToolTipTitle = "Logo Configuration Help";
            // 
            // logoConfigurator
            // 
            this.AcceptButton = this.save;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(472, 169);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnPaste);
            this.Controls.Add(this.btnDynFilename);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCancel);
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximumSize = new System.Drawing.Size(1920, 195);
            this.MinimumSize = new System.Drawing.Size(458, 195);
            this.Name = "logoConfigurator";
            this.Text = "Logo Configuration";
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
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDynFilename;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnPaste;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.ToolTip toolTipLogos;
        private System.Windows.Forms.ToolTip toolTipLogos2;
    }
}
