using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries.Configuration
{
    public partial class FormattingConfiguration : UserControl
    {
        DBFormatting selected = null;
        public FormattingConfiguration()
        {
            InitializeComponent();
            LoadFromDB();
        }

        public void LoadFromDB()
        {
            selected = null;
            addToList(DBFormatting.GetAll());
            enableControls(false);
        }

        void addToList(DBFormatting[] rules)
        {
            list.Items.Clear();
            if (rules != null)
            {
                foreach (DBFormatting db in rules)
                {
                    list.Items.Add(db);
                }
            }
        }

        void enableControls(bool enable)
        {
            this.checkEnabled.Enabled = enable;
            this.textReplace.Enabled = enable;
            this.txtWith.Enabled = enable;

            if(!enable)
            {
                this.checkEnabled.Checked = false;
                this.txtWith.Text = string.Empty;
                this.textReplace.Text = string.Empty;
            }
        }

        private void list_SelectedIndexChanged(object sender, EventArgs e)
        {
            selected = list.SelectedItem as DBFormatting;
            if (selected == null)
            {
                enableControls(false);
                return;
            }
            this.checkEnabled.Checked = selected[DBFormatting.cEnabled];
            this.textReplace.Text = selected[DBFormatting.cReplace];
            this.txtWith.Text = selected[DBFormatting.cWith];
            enableControls(true);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            saveChanges();
        }

        DBFormatting fromInput()
        {
            DBFormatting item = new DBFormatting();
            item[DBFormatting.cEnabled] = this.checkEnabled.Checked;
            item[DBFormatting.cReplace] = this.textReplace.Text;
            item[DBFormatting.cWith] = this.txtWith.Text;

            return item;
        }

        private void saveChanges()
        {
            if(!isValid(fromInput()))
            {
                MessageBox.Show("You need to fill out at least the Search Rule");
                return;
            }
            if (selected == null)
            {
                // new entry
                saveToDBAndReload(fromInput(), true);
                return;
            }
            // existing entry changed
            DBFormatting changed = fromInput();
            
            changed[DBFormatting.cIndex] = selected[DBFormatting.cIndex];
            saveToDBAndReload(changed, false);
            selected = null;
        }

        bool saveToDBAndReload(DBFormatting rule, bool newItem)
        {
            if (isValid(rule))
            {
                if (newItem)
                {
                    DBFormatting.ClearAll();
                    for (int i = 0; i < list.Items.Count; i++)
                    {
                        DBFormatting s = list.Items[i] as DBFormatting;
                        if (s != null)
                        {
                            s[DBFormatting.cIndex] = i;
                            s.Commit();
                        }
                    }
                    rule[DBFormatting.cIndex] = list.Items.Count;
                }

                rule.Commit();
                LoadFromDB();
                return true;
            }
            else return false;
        }

        bool isValid(DBFormatting rule)
        {
            return rule[DBFormatting.cReplace].ToString().Trim() != string.Empty;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selected == null) MessageBox.Show("You need to select a rule first!");
            else
            {
                selected.Delete();
                LoadFromDB();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            selected = null;
            enableControls(false);
            enableControls(true);
        }
    }
}
