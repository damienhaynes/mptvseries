using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries
{
    public partial class ucConfigDisplayControl : UserControl
    {
        private DBSeries m_SeriesReference = new DBSeries(true);
        private DBSeason m_SeasonReference = new DBSeason();
        private DBEpisode m_EpisodeReference = new DBEpisode(true);

        public ucConfigDisplayControl()
        {
            InitializeComponent();
        }

        private void FieldValidate(ref RichTextBox textBox)
        {
            FieldTag tag = textBox.Tag as FieldTag;
            if (!tag.m_bInited)
            {
                textBox.Text = DBOption.GetOptions(tag.m_sOptionName);
                tag.m_bInited = true;
            }

            int nCarret = textBox.SelectionStart;
            String s = textBox.Text;
            Color defColor = textBox.ForeColor;

            int nStart = 0;
            while (s.Length != 0)
            {
                int nTagStart = s.IndexOf('<');
                if (nTagStart != -1)
                {
                    String sCurrent = s.Substring(0, nTagStart);
                    s = s.Substring(nTagStart);

                    textBox.SelectionStart = nStart;
                    textBox.SelectionLength = sCurrent.Length;
                    textBox.SelectionColor = defColor;
                    nStart += sCurrent.Length;

                    int nTagEnd = s.IndexOf('>');
                    if (nTagEnd != -1)
                    {
                        sCurrent = s.Substring(0, nTagEnd + 1);
                        s = s.Substring(nTagEnd + 1);

                        bool bValid = false;
                        textBox.SelectionStart = nStart;
                        textBox.SelectionLength = sCurrent.Length;

                        // find out of the tag exists in the table(s)
                        String sTag = sCurrent.Substring(1, sCurrent.Length - 2);
                        if (sTag.IndexOf('.') != -1)
                        {
                            String sTableName = sTag.Substring(0, sTag.IndexOf('.'));
                            String sFieldName = sTag.Substring(sTag.IndexOf('.') + 1);

                            // unwatchedItems isnt in fieldnames since its purely virtual
                            bValid |= ((sFieldName == DBOnlineSeries.cUnwatchedItems && tag.m_Level == FieldTag.Level.Series) ||
                               (sFieldName == DBSeason.cUnwatchedItems && tag.m_Level == FieldTag.Level.Season));
                            // same for filesizes
                            bValid |= ((sFieldName == DBEpisode.cFileSize && tag.m_Level == FieldTag.Level.Episode) ||
                               (sFieldName == DBEpisode.cFileSizeBytes && tag.m_Level == FieldTag.Level.Episode));
                            // and prettyPlaytime
                            bValid |= (sFieldName == DBEpisode.cPrettyPlaytime && tag.m_Level == FieldTag.Level.Episode);

                            switch (tag.m_Level)
                            {
                                case FieldTag.Level.Series:
                                    if (sTableName == DBSeries.cOutName)
                                        bValid |= m_SeriesReference.FieldNames.Contains(sFieldName);
                                    break;

                                case FieldTag.Level.Season:
                                    if (sTableName == DBSeries.cOutName)
                                        bValid |= m_SeriesReference.FieldNames.Contains(sFieldName);
                                    if (sTableName == DBSeason.cOutName)
                                        bValid |= m_SeasonReference.FieldNames.Contains(sFieldName);
                                    break;

                                case FieldTag.Level.Episode:
                                    if (sTableName == DBSeries.cOutName)
                                        bValid |= m_SeriesReference.FieldNames.Contains(sFieldName);
                                    if (sTableName == DBSeason.cOutName)
                                        bValid |= m_SeasonReference.FieldNames.Contains(sFieldName);
                                    if (sTableName == DBEpisode.cOutName)
                                        bValid |= m_EpisodeReference.FieldNames.Contains(sFieldName);
                                    break;
                            }
                        }

                        if (bValid)
                            textBox.SelectionColor = Color.Green;
                        else
                            textBox.SelectionColor = Color.Red;
                        nStart += sCurrent.Length;

                    }
                    else
                    {
                        // no more closing tag, no good, red
                        textBox.SelectionStart = nStart;
                        textBox.SelectionLength = textBox.Text.Length - nStart;
                        textBox.SelectionColor = Color.Tomato;
                        s = String.Empty;
                    }
                }
                else
                {
                    // no more opening tag
                    textBox.SelectionStart = nStart;
                    textBox.SelectionLength = textBox.Text.Length - nStart;
                    textBox.SelectionColor = defColor;
                    s = String.Empty;
                }
            }

            textBox.SelectionLength = 0;
            textBox.SelectionStart = nCarret;

            DBOption.SetOptions(tag.m_sOptionName, textBox.Text);
        }

        private void addLogo_Click(object sender, EventArgs e)
        {
            logoConfigurator.validDelegate del = delegate(ref RichTextBox txtBox) { FieldValidate(ref txtBox); };
            logoConfigurator lc = new logoConfigurator(del);

            if (DialogResult.OK == lc.ShowDialog())
            {
                List<string> entries = new List<string>();
                foreach (string item in lstLogos.Items)
                    entries.Add(item.ToString());
                entries.Add(lc.result);
                localLogos.saveToDB(entries);
                lstLogos.Items.Clear();
                lstLogos.Items.AddRange(localLogos.getFromDB().ToArray());
            }
        }

        private void btnrmvLogo_Click(object sender, EventArgs e)
        {
            if (lstLogos.SelectedIndex == -1) return;
            List<string> entries = new List<string>();
            foreach (string item in lstLogos.Items)
                entries.Add(item.ToString());
            entries.Remove((string)lstLogos.SelectedItem);
            localLogos.saveToDB(entries);
            lstLogos.Items.Clear();
            lstLogos.Items.AddRange(localLogos.getFromDB().ToArray());
        }

        private void btnLogoDown_Click(object sender, EventArgs e)
        {
            if (lstLogos.Items.Count < 2) return;
            if (lstLogos.SelectedIndex == lstLogos.Items.Count - 1 || lstLogos.SelectedIndex == -1) return;

            string selected = (string)lstLogos.SelectedItem;
            lstLogos.Items[lstLogos.SelectedIndex] = lstLogos.Items[lstLogos.SelectedIndex + 1];
            lstLogos.Items[lstLogos.SelectedIndex + 1] = selected;
            int index = lstLogos.SelectedIndex + 1;
            List<string> entries = new List<string>();
            foreach (string item in lstLogos.Items)
                entries.Add(item.ToString());
            localLogos.saveToDB(entries);
            lstLogos.Items.Clear();
            lstLogos.Items.AddRange(localLogos.getFromDB().ToArray());

            lstLogos.SelectedIndex = index;

        }

        private void btnlogoUp_Click(object sender, EventArgs e)
        {
            if (lstLogos.Items.Count < 2) return;
            if (lstLogos.SelectedIndex == 0 || lstLogos.SelectedIndex == -1) return;

            string selected = (string)lstLogos.SelectedItem;
            lstLogos.Items[lstLogos.SelectedIndex] = lstLogos.Items[lstLogos.SelectedIndex - 1];
            lstLogos.Items[lstLogos.SelectedIndex - 1] = selected;
            int index = lstLogos.SelectedIndex - 1;
            List<string> entries = new List<string>();
            foreach (string item in lstLogos.Items)
                entries.Add(item.ToString());
            localLogos.saveToDB(entries);
            lstLogos.Items.Clear();
            lstLogos.Items.AddRange(localLogos.getFromDB().ToArray());

            lstLogos.SelectedIndex = index;
        }

        private void btnLogoEdit_Click(object sender, EventArgs e)
        {
            if (lstLogos.SelectedIndex == -1) return;
            logoConfigurator.validDelegate del = delegate(ref RichTextBox txtBox) { FieldValidate(ref txtBox); };
            logoConfigurator lc = new logoConfigurator(del, (string)lstLogos.SelectedItem);

            if (DialogResult.OK == lc.ShowDialog())
            {
                //lstLogos.SelectedItem = lc.result;

                List<string> entries = new List<string>();
                foreach (string item in lstLogos.Items)
                {
                    if (item == (string)lstLogos.SelectedItem)
                        entries.Add(lc.result);
                    else
                        entries.Add(item.ToString());
                }
                localLogos.saveToDB(entries);
                lstLogos.Items.Clear();
                lstLogos.Items.AddRange(localLogos.getFromDB().ToArray());
            }
        }
    }
}
