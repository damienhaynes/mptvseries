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
    partial class ChooseFromSelectionDialog
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
            this.textbox_ToMatch = new System.Windows.Forms.TextBox();
            this.label_ToMatch = new System.Windows.Forms.Label();
            this.label_Choices = new System.Windows.Forms.Label();
            this.listbox_Choices = new System.Windows.Forms.ListBox();
            this.button_OK = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.button_Ignore = new System.Windows.Forms.Button();
            this.textbox_Description = new System.Windows.Forms.TextBox();
            this.radOption1 = new System.Windows.Forms.RadioButton();
            this.radOption2 = new System.Windows.Forms.RadioButton();
            this.radOption4 = new System.Windows.Forms.RadioButton();
            this.radOption3 = new System.Windows.Forms.RadioButton();
            this.radOption6 = new System.Windows.Forms.RadioButton();
            this.radOption5 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // textbox_ToMatch
            // 
            this.textbox_ToMatch.Location = new System.Drawing.Point(15, 25);
            this.textbox_ToMatch.Name = "textbox_ToMatch";
            this.textbox_ToMatch.Size = new System.Drawing.Size(406, 20);
            this.textbox_ToMatch.TabIndex = 1;
            this.textbox_ToMatch.TextChanged += new System.EventHandler(this.textbox_ToMatch_TextChanged);
            // 
            // label_ToMatch
            // 
            this.label_ToMatch.AutoSize = true;
            this.label_ToMatch.Location = new System.Drawing.Point(12, 9);
            this.label_ToMatch.Name = "label_ToMatch";
            this.label_ToMatch.Size = new System.Drawing.Size(63, 13);
            this.label_ToMatch.TabIndex = 0;
            this.label_ToMatch.Text = "Looking for:";
            // 
            // label_Choices
            // 
            this.label_Choices.AutoSize = true;
            this.label_Choices.Location = new System.Drawing.Point(12, 47);
            this.label_Choices.Name = "label_Choices";
            this.label_Choices.Size = new System.Drawing.Size(161, 13);
            this.label_Choices.TabIndex = 2;
            this.label_Choices.Text = "Please Select the matching item:";
            // 
            // listbox_Choices
            // 
            this.listbox_Choices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.listbox_Choices.FormattingEnabled = true;
            this.listbox_Choices.Location = new System.Drawing.Point(15, 65);
            this.listbox_Choices.Name = "listbox_Choices";
            this.listbox_Choices.Size = new System.Drawing.Size(406, 95);
            this.listbox_Choices.TabIndex = 3;
            this.listbox_Choices.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listbox_Choices_MouseDoubleClick);
            this.listbox_Choices.SelectedIndexChanged += new System.EventHandler(this.listbox_Choices_SelectedIndexChanged);
            // 
            // button_OK
            // 
            this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_OK.AutoSize = true;
            this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_OK.Location = new System.Drawing.Point(15, 259);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(75, 23);
            this.button_OK.TabIndex = 5;
            this.button_OK.Text = "OK";
            this.button_OK.UseVisualStyleBackColor = true;
            // 
            // button_Cancel
            // 
            this.button_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button_Cancel.AutoSize = true;
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new System.Drawing.Point(174, 259);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel.TabIndex = 6;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            // 
            // button_Ignore
            // 
            this.button_Ignore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Ignore.AutoSize = true;
            this.button_Ignore.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.button_Ignore.Location = new System.Drawing.Point(346, 259);
            this.button_Ignore.Name = "button_Ignore";
            this.button_Ignore.Size = new System.Drawing.Size(75, 23);
            this.button_Ignore.TabIndex = 7;
            this.button_Ignore.Text = "Ignore";
            this.button_Ignore.UseVisualStyleBackColor = true;
            // 
            // textbox_Description
            // 
            this.textbox_Description.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textbox_Description.Location = new System.Drawing.Point(15, 166);
            this.textbox_Description.Multiline = true;
            this.textbox_Description.Name = "textbox_Description";
            this.textbox_Description.ReadOnly = true;
            this.textbox_Description.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textbox_Description.Size = new System.Drawing.Size(406, 86);
            this.textbox_Description.TabIndex = 4;
            // 
            // radOption1
            // 
            this.radOption1.AutoSize = true;
            this.radOption1.Location = new System.Drawing.Point(37, 61);
            this.radOption1.Name = "radOption1";
            this.radOption1.Size = new System.Drawing.Size(77, 17);
            this.radOption1.TabIndex = 8;
            this.radOption1.TabStop = true;
            this.radOption1.Text = "radOption1";
            this.radOption1.UseVisualStyleBackColor = true;
            this.radOption1.CheckedChanged += new System.EventHandler(this.radOption1_CheckedChanged);
            // 
            // radOption2
            // 
            this.radOption2.AutoSize = true;
            this.radOption2.Location = new System.Drawing.Point(37, 78);
            this.radOption2.Name = "radOption2";
            this.radOption2.Size = new System.Drawing.Size(77, 17);
            this.radOption2.TabIndex = 9;
            this.radOption2.TabStop = true;
            this.radOption2.Text = "radOption2";
            this.radOption2.UseVisualStyleBackColor = true;
            this.radOption2.CheckedChanged += new System.EventHandler(this.radOption2_CheckedChanged);
            // 
            // radOption4
            // 
            this.radOption4.AutoSize = true;
            this.radOption4.Location = new System.Drawing.Point(37, 112);
            this.radOption4.Name = "radOption4";
            this.radOption4.Size = new System.Drawing.Size(77, 17);
            this.radOption4.TabIndex = 11;
            this.radOption4.TabStop = true;
            this.radOption4.Text = "radOption4";
            this.radOption4.UseVisualStyleBackColor = true;
            this.radOption4.CheckedChanged += new System.EventHandler(this.radOption4_CheckedChanged);
            // 
            // radOption3
            // 
            this.radOption3.AutoSize = true;
            this.radOption3.Location = new System.Drawing.Point(37, 95);
            this.radOption3.Name = "radOption3";
            this.radOption3.Size = new System.Drawing.Size(77, 17);
            this.radOption3.TabIndex = 10;
            this.radOption3.TabStop = true;
            this.radOption3.Text = "radOption3";
            this.radOption3.UseVisualStyleBackColor = true;
            this.radOption3.CheckedChanged += new System.EventHandler(this.radOption3_CheckedChanged);
            // 
            // radOption6
            // 
            this.radOption6.AutoSize = true;
            this.radOption6.Location = new System.Drawing.Point(37, 145);
            this.radOption6.Name = "radOption6";
            this.radOption6.Size = new System.Drawing.Size(77, 17);
            this.radOption6.TabIndex = 13;
            this.radOption6.TabStop = true;
            this.radOption6.Text = "radOption6";
            this.radOption6.UseVisualStyleBackColor = true;
            this.radOption6.CheckedChanged += new System.EventHandler(this.radOption6_CheckedChanged);
            // 
            // radOption5
            // 
            this.radOption5.AutoSize = true;
            this.radOption5.Location = new System.Drawing.Point(37, 128);
            this.radOption5.Name = "radOption5";
            this.radOption5.Size = new System.Drawing.Size(77, 17);
            this.radOption5.TabIndex = 12;
            this.radOption5.TabStop = true;
            this.radOption5.Text = "radOption5";
            this.radOption5.UseVisualStyleBackColor = true;
            this.radOption5.CheckedChanged += new System.EventHandler(this.radOption5_CheckedChanged);
            // 
            // ChooseFromSelectionDialog
            // 
            this.AcceptButton = this.button_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_Cancel;
            this.ClientSize = new System.Drawing.Size(435, 293);
            this.Controls.Add(this.radOption6);
            this.Controls.Add(this.radOption5);
            this.Controls.Add(this.radOption4);
            this.Controls.Add(this.radOption3);
            this.Controls.Add(this.radOption2);
            this.Controls.Add(this.radOption1);
            this.Controls.Add(this.button_Ignore);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.textbox_ToMatch);
            this.Controls.Add(this.label_ToMatch);
            this.Controls.Add(this.label_Choices);
            this.Controls.Add(this.listbox_Choices);
            this.Controls.Add(this.textbox_Description);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ChooseFromSelectionDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ChooseFromSelection";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textbox_ToMatch;
        private System.Windows.Forms.Label label_ToMatch;
        private System.Windows.Forms.Label label_Choices;
        private System.Windows.Forms.ListBox listbox_Choices;
        private System.Windows.Forms.Button button_OK;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Button button_Ignore;
        private System.Windows.Forms.TextBox textbox_Description;
        private System.Windows.Forms.RadioButton radOption1;
        private System.Windows.Forms.RadioButton radOption2;
        private System.Windows.Forms.RadioButton radOption4;
        private System.Windows.Forms.RadioButton radOption3;
        private System.Windows.Forms.RadioButton radOption6;
        private System.Windows.Forms.RadioButton radOption5;
    }
}