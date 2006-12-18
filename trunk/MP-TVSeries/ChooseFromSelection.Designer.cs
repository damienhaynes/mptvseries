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
            this.SuspendLayout();
            // 
            // textbox_ToMatch
            // 
            this.textbox_ToMatch.Location = new System.Drawing.Point(15, 25);
            this.textbox_ToMatch.Name = "textbox_ToMatch";
            this.textbox_ToMatch.Size = new System.Drawing.Size(406, 20);
            this.textbox_ToMatch.TabIndex = 0;
            this.textbox_ToMatch.TextChanged += new System.EventHandler(this.textbox_ToMatch_TextChanged);
            // 
            // label_ToMatch
            // 
            this.label_ToMatch.AutoSize = true;
            this.label_ToMatch.Location = new System.Drawing.Point(12, 9);
            this.label_ToMatch.Name = "label_ToMatch";
            this.label_ToMatch.Size = new System.Drawing.Size(63, 13);
            this.label_ToMatch.TabIndex = 11;
            this.label_ToMatch.Text = "Looking for:";
            // 
            // label_Choices
            // 
            this.label_Choices.AutoSize = true;
            this.label_Choices.Location = new System.Drawing.Point(12, 47);
            this.label_Choices.Name = "label_Choices";
            this.label_Choices.Size = new System.Drawing.Size(161, 13);
            this.label_Choices.TabIndex = 10;
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
            this.listbox_Choices.TabIndex = 1;
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
            this.button_OK.TabIndex = 2;
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
            this.button_Cancel.TabIndex = 3;
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
            this.button_Ignore.TabIndex = 4;
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
            this.textbox_Description.TabIndex = 15;
            // 
            // ChooseFromSelectionDialog
            // 
            this.AcceptButton = this.button_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_Cancel;
            this.ClientSize = new System.Drawing.Size(435, 293);
            this.Controls.Add(this.textbox_Description);
            this.Controls.Add(this.button_Ignore);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.textbox_ToMatch);
            this.Controls.Add(this.label_ToMatch);
            this.Controls.Add(this.label_Choices);
            this.Controls.Add(this.listbox_Choices);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ChooseFromSelectionDialog";
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
    }
}