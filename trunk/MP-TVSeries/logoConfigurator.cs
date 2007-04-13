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


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries
{
    public partial class logoConfigurator : Form
    {
        public delegate void validDelegate(ref RichTextBox txtBox);
        validDelegate validateTxtBox;
        string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        public logoConfigurator(validDelegate validDel)
        {
            init(validDel);
        }

        public logoConfigurator(validDelegate validDel, string entryToEdit)
        {
            init(validDel);
            parseForEdit(entryToEdit);
        }

        public void init(validDelegate validDel)
        {
            InitializeComponent();
            validateTxtBox = validDel;
            FieldTag forWhats = new FieldTag("lastField", FieldTag.Level.Episode);
            forWhats.m_bInited = true; // avoid autofilling
            this.cond1_what.Tag = forWhats;
            this.cond2_what.Tag = forWhats;
            this.cond3_what.Tag = forWhats;

            this.textBox1.Tag = forWhats;

            this.cond1_type.SelectedIndex = 0;
            this.cond2_type.SelectedIndex = 0;
            this.cond3_type.SelectedIndex = 0;

            this._12_link.SelectedIndex = 0;
            this._23_link.SelectedIndex = 0;
        }


        public string result = string.Empty;
        private void logoConfigurator_Load(object sender, EventArgs e)
        {

        }

        private void parseForEdit(string entry)
        {
            try
            {
                string[] split = System.Text.RegularExpressions.Regex.Split(entry, localLogos.condSplit);
                if (split[0].Contains("<") || System.IO.Path.IsPathRooted(split[0]))
                    this.textBox1.Text = split[0];
                else this.textBox1.Text = appPath + "\\" + split[0];

                this.cond1_what.Text = split[1];
                this.cond1_type.SelectedItem = split[2];
                this.cond1_cond.Text = split[3];
                this._12_link.SelectedItem = split[4];

                this.cond2_what.Text = split[5];
                this.cond2_type.SelectedItem = split[6];
                this.cond2_cond.Text = split[7];
                this._23_link.SelectedItem = split[8];

                this.cond3_what.Text = split[9];
                this.cond3_type.SelectedItem = split[10];
                this.cond3_cond.Text = split[11];

                if (textBox1.Text.Contains(">") && textBox1.Text.Contains("<"))
                    textBox1.ReadOnly = false;
                // dyn filename quick check
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was a problem parsing the Rule: " + ex.Message);
            }
        }

        private void browse_Click(object sender, EventArgs e)
        {
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG";
            openFileDialog1.InitialDirectory = appPath;
            if (DialogResult.OK == openFileDialog1.ShowDialog())
                this.textBox1.Text = openFileDialog1.FileName;
            this.textBox1.ReadOnly = true;
        }

        private void save_Click(object sender, EventArgs e)
        {
            this.result = getResult(true);
            if (result == null)
            {
                MessageBox.Show("There was a problem saving the LogoRule.\nDoes the file exist?");
            }
            else
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        string getResult(bool checkFileExists)
        {
            if (!this.cond2_what.Enabled || (cond2_what.Text.Trim() == string.Empty && cond2_cond.Text.Trim() == string.Empty))
            {
                _12_link.SelectedIndex = 0; // force back to AND
                cond2_what.Text = "";
                cond2_cond.Text = "";
                cond2_type.SelectedIndex = 0;
            }
            if (!this.cond3_what.Enabled || (cond3_what.Text.Trim() == string.Empty && cond3_cond.Text.Trim() == string.Empty))
            {
                _23_link.SelectedIndex = 0; // force back to AND
                cond3_what.Text = "";
                cond3_cond.Text = "";
                cond3_type.SelectedIndex = 0;
            }
            string file = this.textBox1.Text;

            if (file.Contains(appPath)) file = file.Replace(appPath, "").Remove(0,1); // first \ also away

            if (!checkFileExists || (System.IO.File.Exists(this.textBox1.Text) || (textBox1.Text.Contains(">") && textBox1.Text.Contains("<"))))
            {
                return file + localLogos.condSplit
                            + this.cond1_what.Text + localLogos.condSplit
                            + this.cond1_type.SelectedItem.ToString() + localLogos.condSplit
                            + this.cond1_cond.Text + localLogos.condSplit
                            + this._12_link.SelectedItem.ToString() + localLogos.condSplit
                            + this.cond2_what.Text + localLogos.condSplit
                            + this.cond2_type.SelectedItem.ToString() + localLogos.condSplit
                            + this.cond2_cond.Text + localLogos.condSplit
                            + this._23_link.SelectedItem.ToString() + localLogos.condSplit
                            + this.cond3_what.Text + localLogos.condSplit
                            + this.cond3_type.SelectedItem.ToString() + localLogos.condSplit
                            + this.cond3_cond.Text + localLogos.condSplit;
            }
            else return null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cond1_what_TextChanged(object sender, EventArgs e)
        {
            validateTxtBox(ref this.cond1_what);
            enabled2();
        }

        private void cond2_what_TextChanged(object sender, EventArgs e)
        {
            validateTxtBox(ref this.cond2_what);
            enabled3();
        }

        private void cond3_what_TextChanged(object sender, EventArgs e)
        {
            validateTxtBox(ref this.cond3_what);
        }

        private void enabled2()
        {
            bool enable = true;
            if (this.cond1_what.Text != string.Empty && this.cond1_cond.Text != string.Empty)
                enable = true;
            else
                enable = false;
            this._12_link.Enabled = enable;
            this.cond2_cond.Enabled = enable;
            this.cond2_type.Enabled = enable;
            this.cond2_what.Enabled = enable;
        }

        private void enabled3()
        {
            bool enable = true;
            if (this.cond2_what.Text != string.Empty && this.cond2_cond.Text != string.Empty)
                enable = true;
            else
                enable = false;
            this._23_link.Enabled = enable;
            this.cond3_cond.Enabled = enable;
            this.cond3_type.Enabled = enable;
            this.cond3_what.Enabled = enable;
        }

        private void cond1_cond_TextChanged(object sender, EventArgs e)
        {
            enabled2();
        }

        private void cond2_cond_TextChanged(object sender, EventArgs e)
        {
            enabled3();
        }

        private void btnDynFilename_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            if (DialogResult.OK == folderBrowserDialog1.ShowDialog())
            {
                this.textBox1.Text = folderBrowserDialog1.SelectedPath + @"\";
                this.textBox1.ReadOnly = false;

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (this.textBox1.Enabled)
            {
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string pasted = Clipboard.GetText();
            try
            {
                parseForEdit(pasted);
            }
            catch (Exception)
            {
                MessageBox.Show("There was a problem pasting the logo rule!");
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {

            string copied = getResult(false);
            if (copied == null)
            {
                MessageBox.Show("There was a problem copying the logo rule!"); 
            }
            else
            {
                Clipboard.SetText(copied);
                MessageBox.Show("Logo Rule Configuration copied to Clipboard!"); 
            }
        }
    }
}
