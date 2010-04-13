using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WindowPlugins.GUITVSeries.DataClass;

namespace WindowPlugins.GUITVSeries.Configuration
{
    public partial class FormattingConfiguration : UserControl
    {
        DBFormatting selected = null;
        DBSeries series = null;
        DBSeason season = null;
        DBEpisode ep = null;

        public FormattingConfiguration()
        {
            InitializeComponent();
            LoadFromDB();
        }

        public DBSeries Series
        {
            set { series = value; }
        }

        public DBSeason Season
        {
            set { season = value; }
        }

        public DBEpisode Episode
        {
            set { ep = value; }
        }

        public void LoadFromDB()
        {
            selected = null;
            addToList(DBFormatting.GetAll());
            enableControls(false);
        }

        void addToList(IEnumerable<DBFormatting> rules)
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
                MessageBox.Show("You need to create at least one Formatting Rule first!");
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
                            DBFormatting re = new DBFormatting(i);
                            re[DBFormatting.cEnabled] = s[DBFormatting.cEnabled];
                            re[DBFormatting.cReplace] = s[DBFormatting.cReplace];
                            re[DBFormatting.cWith] = s[DBFormatting.cWith];
                            re.Commit();
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

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.IO.StreamWriter w = null;
            try
            {
                saveFileDialog1.AddExtension = true;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    w = new System.IO.StreamWriter(saveFileDialog1.FileName);
                    foreach (object o in list.Items)
                    {
                        DBFormatting dbf = o as DBFormatting;
                        w.WriteLine(string.Format("<Enabled>{0}<Format>{1}<FormatAs>{2}", dbf[DBFormatting.cEnabled], dbf[DBFormatting.cReplace], dbf[DBFormatting.cWith]));
                    }
                    w.Flush();
                    MPTVSeriesLog.Write(list.Items.Count.ToString() + " Formatting Rules Exported",MPTVSeriesLog.LogLevel.Normal);
                }
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Error in trying to Export User Formatting Rules: " + ex.Message);
            }
            finally
            {
                if (w != null) w.Close();
            }
        }

        private void lnkImport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.IO.StreamReader w = null;
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    w = new System.IO.StreamReader(openFileDialog1.FileName);
                    List<DBFormatting> imports = new List<DBFormatting>();
                    string line;
                    string[] splits = new string[] { "<Enabled>", "<Format>", "<FormatAs>" };
                    while((line = w.ReadLine()) != null)
                    {
                        string[] properties = line.Split(splits, StringSplitOptions.RemoveEmptyEntries);
                        if (properties.Length == 3)
                        {
                            DBFormatting dbf = new DBFormatting();
                            dbf[DBFormatting.cEnabled] = properties[0];
                            dbf[DBFormatting.cReplace] = properties[1];
                            dbf[DBFormatting.cWith] = properties[2];
                            imports.Add(dbf);
                            // now for add each one to the list
                            saveToDBAndReload(dbf, true);
                        }
                        else
                        {
                            MessageBox.Show("Unable to Import: " + line);
                        }
                    }

                    MPTVSeriesLog.Write(imports.Count.ToString() + " Formatting Rules Imported",MPTVSeriesLog.LogLevel.Normal);
                }
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Error in trying to Export User Formatting Rules: " + ex.Message);
            }
            finally
            {
                if (w != null) w.Close();
            }
        }

        private void textReplace_TextChanged(object sender, EventArgs e)
        {
            Evaluate();
        }

        private void txtWith_TextChanged(object sender, EventArgs e)
        {
            Evaluate();
        }

        private void Evaluate()
        {        
            this.textBox1.Text = "Make sure you select an item in the Details Tree";

            if (txtWith.Text == string.Empty || textReplace.Text == string.Empty)
                this.textBox1.Text = "The result of your Formatting Rule will appear here...";
            else
            {
                DBFormatting current = this.fromInput();
                DBFormatting.cache = new DBFormatting[] { current };

                if (txtWith.Text.IndexOf("<Episode", 0) >= 0)
                {
                    if (ep == null) return;
                    this.textBox1.Text = FieldGetter.resolveDynString(this.textReplace.Text, ep, true, true);
                }
                else if (txtWith.Text.IndexOf("<Season", 0) >= 0)
                {
                    if (season == null) return;
                    this.textBox1.Text = FieldGetter.resolveDynString(this.textReplace.Text, season, true, true);
                }
                else
                {
                    if (txtWith.Text.IndexOf("<Series", 0) >= 0)
                    {
                        if (series == null) return;
                        this.textBox1.Text = FieldGetter.resolveDynString(this.textReplace.Text, series, true, true);
                    }
                }

                DBFormatting.cache = null;                
            }
        }

        private void btnFRDeleteAll_Click(object sender, EventArgs e)
        {
            if (list.Items.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to delete all Formatting Rules?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;
                
                DBFormatting.ClearAll();
                list.Items.Clear();
                enableControls(false);
            }
        }
    }
}
