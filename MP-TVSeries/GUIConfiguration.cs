using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using MediaPortal.Util;
using System.Windows.Forms;
using WindowPlugins.GUITVSeries;
#if DEBUG
using System.Diagnostics;
#endif

namespace WindowPlugins.GUITVSeries
{
    public partial class ConfigurationForm : Form
    {
        private List<Panel> m_paneList = new List<Panel>();
        private TreeNode nodeEdited = null;
        private OnlineParsing m_parser = null;
        private DateTime m_timingStart = new DateTime();

        private DBSeries m_SeriesReference = new DBSeries();
        private DBSeason m_SeasonReference = new DBSeason();
        private DBEpisode m_EpisodeReference = new DBEpisode(true);

        public ConfigurationForm()
        {
#if DEBUG
//    Debugger.Launch();
#endif


            InitializeComponent();
            MPTVSeriesLog.AddNotifier(ref listBox_Log);
            
            MPTVSeriesLog.Write("**** Plugin started in configuration mode ***");
            this.comboBox1.SelectedIndex = 0;
            InitSettingsTreeAndPanes();
            LoadImportPathes();
            LoadExpressions();
            LoadReplacements();
            LoadTree();
        }

        #region Init
        private void InitSettingsTreeAndPanes()
        {
            m_paneList.Add(panel_ImportPathes);
            m_paneList.Add(panel_Expressions);
            m_paneList.Add(panel_StringReplacements);
            m_paneList.Add(panel_ParsingTest);
            m_paneList.Add(panel_OnlineData);

            foreach (Panel pane in m_paneList)
            {
                pane.Dock = DockStyle.Fill;
                pane.Visible = false;
                TreeNode node = new TreeNode(pane.Tag.ToString());
                node.Name = pane.Name;
                treeView_Settings.Nodes.Add(node);
            }

            treeView_Settings.SelectedNode = treeView_Settings.Nodes[0];

            checkBox_OnlineSearch.Checked = DBOption.GetOptions(DBOption.cOnlineParseEnabled);
            checkBox_FullSeriesRetrieval.Checked = DBOption.GetOptions(DBOption.cFullSeriesRetrieval);
            checkBox_AutoChooseSeries.Checked = DBOption.GetOptions(DBOption.cAutoChooseSeries);
            checkBox_LocalDataOverride.Checked = DBOption.GetOptions(DBOption.cLocalDataOverride);
            checkBox_Episode_OnlyShowLocalFiles.Checked = DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles);
            checkBox_Episode_HideUnwatchedSummary.Checked = DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedSummary);

            checkBox_AutoScanLocal.Checked = DBOption.GetOptions(DBOption.cAutoScanLocalFiles);
            numericUpDown_AutoScanLocal.Enabled = checkBox_AutoScanLocal.Checked;
            int nValue = DBOption.GetOptions(DBOption.cAutoScanLocalFilesLapse);
            numericUpDown_AutoScanLocal.Minimum = 1;
            numericUpDown_AutoScanLocal.Maximum = 180;
            numericUpDown_AutoScanLocal.Value = nValue;
            checkBox_AutoOnlineDataRefresh.Checked = DBOption.GetOptions(DBOption.cAutoUpdateOnlineData);
            numericUpDown_AutoOnlineDataRefresh.Enabled = checkBox_AutoOnlineDataRefresh.Checked;
            nValue = DBOption.GetOptions(DBOption.cAutoUpdateOnlineDataLapse);
            numericUpDown_AutoOnlineDataRefresh.Minimum = 1;
            numericUpDown_AutoOnlineDataRefresh.Maximum = 24;
            numericUpDown_AutoOnlineDataRefresh.Value = nValue;

            richTextBox_seriesFormat_Col1.Tag = new FieldTag(DBOption.cView_Series_Col1, FieldTag.Level.Series);
            FieldValidate(ref richTextBox_seriesFormat_Col1);

            richTextBox_seriesFormat_Col2.Tag = new FieldTag(DBOption.cView_Series_Col2, FieldTag.Level.Series);
            FieldValidate(ref richTextBox_seriesFormat_Col2);

            richTextBox_seriesFormat_Col3.Tag = new FieldTag(DBOption.cView_Series_Col3, FieldTag.Level.Series);
            FieldValidate(ref richTextBox_seriesFormat_Col3);

            richTextBox_seriesFormat_Title.Tag = new FieldTag(DBOption.cView_Series_Title, FieldTag.Level.Series);
            FieldValidate(ref richTextBox_seriesFormat_Title);

            richTextBox_seriesFormat_Subtitle.Tag = new FieldTag(DBOption.cView_Series_Subtitle, FieldTag.Level.Series);
            FieldValidate(ref richTextBox_seriesFormat_Subtitle);

            richTextBox_seriesFormat_Main.Tag = new FieldTag(DBOption.cView_Series_Main, FieldTag.Level.Series);
            FieldValidate(ref richTextBox_seriesFormat_Main);

            richTextBox_episodeFormat_Col1.Tag = new FieldTag(DBOption.cView_Episode_Col1, FieldTag.Level.Episode);
            FieldValidate(ref richTextBox_episodeFormat_Col1);

            richTextBox_episodeFormat_Col2.Tag = new FieldTag(DBOption.cView_Episode_Col2, FieldTag.Level.Episode);
            FieldValidate(ref richTextBox_episodeFormat_Col2);

            richTextBox_episodeFormat_Col3.Tag = new FieldTag(DBOption.cView_Episode_Col3, FieldTag.Level.Episode);
            FieldValidate(ref richTextBox_episodeFormat_Col3);

            richTextBox_episodeFormat_Title.Tag = new FieldTag(DBOption.cView_Episode_Title, FieldTag.Level.Episode);
            FieldValidate(ref richTextBox_episodeFormat_Title);

            richTextBox_episodeFormat_Subtitle.Tag = new FieldTag(DBOption.cView_Episode_Subtitle, FieldTag.Level.Episode);
            FieldValidate(ref richTextBox_episodeFormat_Subtitle);

            richTextBox_episodeFormat_Main.Tag = new FieldTag(DBOption.cView_Episode_Main, FieldTag.Level.Episode);
            FieldValidate(ref richTextBox_episodeFormat_Main);

//            contextMenuStrip_SeriesFields.Items.Add("Insert a field value");
//             // Create a new ToolStrip control.
//             ToolStrip ts = new ToolStrip();
// 
//             // Create a ToolStripDropDownButton control and add it
//             // to the ToolStrip control's Items collections.
//             ToolStripDropDownButton fruitToolStripDropDownButton = new ToolStripDropDownButton("Fruit", null, null, "Fruit");
//             ts.Items.Add(fruitToolStripDropDownButton);
// 
//             // Dock the ToolStrip control to the top of the form.
//             ts.Dock = DockStyle.Top;
// 
//             // Assign the ContextMenuStrip control as the 
//             // ToolStripDropDownButton control's DropDown menu.
//             fruitToolStripDropDownButton.DropDown = contextMenuStrip_SeriesFields;
// 
//             // Create a new MenuStrip control and add a ToolStripMenuItem.
//             MenuStrip ms = new MenuStrip();
//             ToolStripMenuItem fruitToolStripMenuItem = new ToolStripMenuItem("Fruit", null, null, "Fruit");
//             ms.Items.Add(fruitToolStripMenuItem);
// 
//             // Dock the MenuStrip control to the top of the form.
//             ms.Dock = DockStyle.Top;
// 
//             // Assign the MenuStrip control as the 
//             // ToolStripMenuItem's DropDown menu.
//             fruitToolStripMenuItem.DropDown = contextMenuStrip_SeriesFields;
// 
//             // Assign the ContextMenuStrip to the form's 
//             // ContextMenuStrip property.
//             this.ContextMenuStrip = contextMenuStrip_SeriesFields;
// 
//             // Add the ToolStrip control to the Controls collection.
//             this.Controls.Add(ts);
// 
//             //Add a button to the form and assign its ContextMenuStrip.
//             Button b = new Button();
//             b.Location = new System.Drawing.Point(60, 60);
//             this.Controls.Add(b);
//             b.ContextMenuStrip = contextMenuStrip_SeriesFields;
// 
//             // Add the MenuStrip control last.
//             // This is important for correct placement in the z-order.
//             this.Controls.Add(ms);
        }

        private void LoadImportPathes()
        {
            if (dataGridView_ImportPathes.Columns.Count == 0)
            {
                DataGridViewCheckBoxColumn columnEnabled = new DataGridViewCheckBoxColumn();
                columnEnabled.Name = DBImportPath.cEnabled;
                columnEnabled.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                dataGridView_ImportPathes.Columns.Add(columnEnabled);

                DataGridViewButtonColumn columnPath = new DataGridViewButtonColumn();
                columnPath.Name = DBImportPath.cPath;
                columnPath.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView_ImportPathes.Columns.Add(columnPath);
            }

            DBImportPath[] importPathes = DBImportPath.GetAll();

            dataGridView_ImportPathes.Rows.Clear();

            if (importPathes != null && importPathes.Length > 0)
            {
                dataGridView_ImportPathes.Rows.Add(importPathes.Length);
                foreach (DBImportPath importPath in importPathes)
                {
                    DataGridViewRow row = dataGridView_ImportPathes.Rows[importPath[DBImportPath.cIndex]];
                    row.Cells[DBImportPath.cEnabled].Value = (Boolean)importPath[DBImportPath.cEnabled];
                    row.Cells[DBImportPath.cPath].Value = (String)importPath[DBImportPath.cPath];
                }
            }
        }

        private void LoadExpressions()
        {
            DBExpression[] expressions = DBExpression.GetAll();
            if (expressions == null || expressions.Length == 0)
            {
                // no expressions in the db => put the default ones
                DBExpression expression = new DBExpression();
                expression[DBExpression.cIndex] = "0";
                expression[DBExpression.cEnabled] = "1";
                expression[DBExpression.cType] = DBExpression.cType_Simple;
                expression[DBExpression.cExpression] = @"<series> - <season>x<episode> - <title>.<ext>";
                expression.Commit();

                expression[DBExpression.cIndex] = "1";
                expression[DBExpression.cExpression] = @"\<series>\Season <season>\Episode <episode> - <title>.<ext>";
                expression.Commit();

                expression[DBExpression.cType] = DBExpression.cType_Regexp;
                expression[DBExpression.cIndex] = "2";
                expression[DBExpression.cExpression] = @"(?<series>[^\\\[]*) - \[(?<season>[0-9]{1,2})x(?<episode>[0-9\W]+)\](( |)(-( |)|))(?<title>[^$]*?)\.(?<ext>[^.]*)";
                expression.Commit();

                expression[DBExpression.cIndex] = "3";
                expression[DBExpression.cExpression] = @"(?<series>[^\\$]*) - season (?<season>[0-9]{1,2}) - (?<title>[^$]*?)\.(?<ext>[^.]*)";
                expression.Commit();

                expression[DBExpression.cIndex] = "4";
                expression[DBExpression.cExpression] = @"^(?<series>[^\\$]+)\\[^\\$]*?(?:s(?<season>[0-1]?[0-9])e(?<episode>[0-9]{2})|(?<season>(?:[0-1][0-9]|(?<!\d)[0-9]))x?(?<episode>[0-9]{2}))(?!\d)[ \-\.]*(?<title>[^\\]*?)\.(?<ext>[^.]*)$";
                expression.Commit();

                expression[DBExpression.cIndex] = "5";
                expression[DBExpression.cExpression] = @"^.*?\\?(?<series>[^\\$]+?)(?:s(?<season>[0-1]?[0-9])e(?<episode>[0-9]{2})|(?<season>(?:[0-1][0-9]|(?<!\d)[0-9]))x?(?<episode>[0-9]{2}))(?!\d)[ \-\.]*(?<title>[^\\]*?)\.(?<ext>[^.]*)$";
                expression.Commit();
                

                // refresh
                expressions = DBExpression.GetAll();
            }

            // load them up in the datagrid

            //             foreach (KeyValuePair<string, DBField> field in expressions[0].m_fields)
            //             {
            //                 if (field.Key != DBExpression.cIndex)
            //                 {
            //                     DataGridViewCheckBoxColumn column = new DataGridBoolColumn();
            //                     column.Name = field.Key;
            //                     column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //                     dataGridView_Expressions.Columns.Add(column);
            //                 }
            //             }

            if (dataGridView_Expressions.Columns.Count == 0)
            {
                DataGridViewCheckBoxColumn columnEnabled = new DataGridViewCheckBoxColumn();
                columnEnabled.Name = DBExpression.cEnabled;
                columnEnabled.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                dataGridView_Expressions.Columns.Add(columnEnabled);

                DataGridViewComboBoxColumn columnType = new DataGridViewComboBoxColumn();
                columnType.Name = DBExpression.cType;
                columnType.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DataGridViewComboBoxCell comboCellTemplate = new DataGridViewComboBoxCell();
                comboCellTemplate.Items.Add(DBExpression.cType_Simple);
                comboCellTemplate.Items.Add(DBExpression.cType_Regexp);
                columnType.CellTemplate = comboCellTemplate;
                dataGridView_Expressions.Columns.Add(columnType);

                DataGridViewTextBoxColumn columnExpression = new DataGridViewTextBoxColumn();
                columnExpression.Name = DBExpression.cExpression;
                columnExpression.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView_Expressions.Columns.Add(columnExpression);
            }
            dataGridView_Expressions.Rows.Clear();
            dataGridView_Expressions.Rows.Add(expressions.Length);

            foreach (DBExpression expression in expressions)
            {
                DataGridViewRow row = dataGridView_Expressions.Rows[expression[DBExpression.cIndex]];
                row.Cells[DBExpression.cEnabled].Value = (Boolean)expression[DBExpression.cEnabled];
                DataGridViewComboBoxCell comboCell = new DataGridViewComboBoxCell();
                comboCell.Items.Add(DBExpression.cType_Simple);
                comboCell.Items.Add(DBExpression.cType_Regexp);
                comboCell.Value = (String)expression[DBExpression.cType];
                row.Cells[DBExpression.cType] = comboCell;
                row.Cells[DBExpression.cExpression].Value = (String)expression[DBExpression.cExpression];
            }
        }

        private void LoadReplacements()
        {
            DBReplacements[] replacements = DBReplacements.GetAll();
            if (replacements == null || replacements.Length == 0)
            {
                // no replacements in the db => put the default ones
                DBReplacements replacement = new DBReplacements();
                replacement[DBReplacements.cIndex] = "0";
                replacement[DBReplacements.cEnabled] = "1";
                replacement[DBReplacements.cToReplace] = ".";
                replacement[DBReplacements.cWith] = @"<space>";
                replacement.Commit();

                replacement[DBReplacements.cIndex] = "1";
                replacement[DBReplacements.cToReplace] = "_";
                replacement[DBReplacements.cWith] = @"<space>";
                replacement.Commit();

                replacement[DBReplacements.cIndex] = "2";
                replacement[DBReplacements.cToReplace] = "-<space>";
                replacement[DBReplacements.cWith] = @"<empty>";
                replacement.Commit();

                // refresh
                replacements = DBReplacements.GetAll();
            }

            // load them up in the datagrid

            //             foreach (KeyValuePair<string, DBField> field in expressions[0].m_fields)
            //             {
            //                 if (field.Key != DBExpression.cIndex)
            //                 {
            //                     DataGridViewCheckBoxColumn column = new DataGridBoolColumn();
            //                     column.Name = field.Key;
            //                     column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //                     dataGridView_Expressions.Columns.Add(column);
            //                 }
            //             }

            if (dataGridView_Replace.Columns.Count == 0)
            {
                DataGridViewCheckBoxColumn columnEnabled = new DataGridViewCheckBoxColumn();
                columnEnabled.Name = DBReplacements.cEnabled;
                columnEnabled.HeaderText = DBReplacements.PrettyFieldName(DBReplacements.cEnabled);
                columnEnabled.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                dataGridView_Replace.Columns.Add(columnEnabled);

                DataGridViewTextBoxColumn columnToReplace = new DataGridViewTextBoxColumn();
                columnToReplace.Name = DBReplacements.cToReplace;
                columnToReplace.HeaderText = DBReplacements.PrettyFieldName(DBReplacements.cToReplace);
                columnToReplace.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView_Replace.Columns.Add(columnToReplace);

                DataGridViewTextBoxColumn columnWith = new DataGridViewTextBoxColumn();
                columnWith.Name = DBReplacements.cWith;
                columnWith.HeaderText = DBReplacements.PrettyFieldName(DBReplacements.cWith);
                columnWith.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView_Replace.Columns.Add(columnWith);
            }
            dataGridView_Replace.Rows.Clear();
            dataGridView_Replace.Rows.Add(replacements.Length);

            foreach (DBReplacements replacement in replacements)
            {
                DataGridViewRow row = dataGridView_Replace.Rows[replacement[DBReplacements.cIndex]];
                row.Cells[DBReplacements.cEnabled].Value = (Boolean)replacement[DBReplacements.cEnabled];
                row.Cells[DBReplacements.cToReplace].Value = (String)replacement[DBReplacements.cToReplace];
                row.Cells[DBReplacements.cWith].Value = (String)replacement[DBReplacements.cWith];
            }
        }

        private void LoadTree()
        {
            TreeView root = this.treeView_Library;
            root.Nodes.Clear();

            List<DBSeries> seriesList = DBSeries.Get(false);
            if (seriesList.Count == 0)
            {
                return;
            }

            foreach (DBSeries series in seriesList)
            {
                TreeNode seriesNode = new TreeNode(series[DBSeries.cPrettyName]);
                seriesNode.Name = DBSeries.cTableName;
                seriesNode.Tag = (DBSeries)series;
                seriesNode.Expand();
                root.Nodes.Add(seriesNode);

                List<DBSeason> seasonsList = DBSeason.Get(series[DBSeries.cParsedName].ToString(), false);
                foreach (DBSeason season in seasonsList)
                {
                    TreeNode seasonNode = new TreeNode("Season " + season[DBSeason.cIndex]);
                    seasonNode.Name = DBSeason.cTableName;
                    seasonNode.Tag = (DBSeason)season;
                    seriesNode.Nodes.Add(seasonNode);
                    // default a season node to disabled, reenable it if an episode node is valid
                    seasonNode.ForeColor = System.Drawing.SystemColors.GrayText;


                    List<DBEpisode> episodesList = DBEpisode.Get((String)series[DBSeries.cParsedName], (int)season[DBSeason.cIndex], false);

                    foreach (DBEpisode episode in episodesList)
                    {
                        String sEpisodeName = (String)episode[DBEpisode.cEpisodeName];
                        TreeNode episodeNode = new TreeNode(episode[DBEpisode.cSeasonIndex] + "x" + episode[DBEpisode.cEpisodeIndex] + " - " + sEpisodeName);
                        episodeNode.Name = DBEpisode.cTableName;
                        episodeNode.Tag = (DBEpisode)episode;
                        if (episode[DBEpisode.cFilename] == "")
                        {
                            episodeNode.ForeColor = System.Drawing.SystemColors.GrayText;
                        }
                        else 
                        {
                            seasonNode.ForeColor = treeView_Library.ForeColor;
                        }

                        seasonNode.Nodes.Add(episodeNode);
                    }
                    if (episodesList.Count == 0)
                    {
                        // no episodes => no season node
                        seriesNode.Nodes.Remove(seasonNode);
                    }
                }
            }
        }
        #endregion

        #region Import Handling
        private void dataGridView_ImportPathes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DBImportPath importPath = new DBImportPath();
            importPath[DBImportPath.cIndex] = e.RowIndex.ToString();
            foreach (DataGridViewCell cell in dataGridView_ImportPathes.Rows[e.RowIndex].Cells)
            {
                if (cell.Value == null)
                    return;
                if (cell.ValueType == typeof(Boolean))
                    importPath[cell.OwningColumn.Name] =(Boolean)cell.Value;
                else
                    importPath[cell.OwningColumn.Name] = (String)cell.Value;
            }
            importPath.Commit();
        }

        private void dataGridView_ImportPathes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView_ImportPathes.Columns[DBImportPath.cPath].Index)
            {
                if (dataGridView_ImportPathes.NewRowIndex == e.RowIndex)
                {
                    dataGridView_ImportPathes.Rows.Add();
                    dataGridView_ImportPathes.Rows[e.RowIndex].Cells[DBImportPath.cEnabled].Value = true;
                }

                if (dataGridView_ImportPathes.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                    folderBrowserDialog1.SelectedPath = dataGridView_ImportPathes.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                DialogResult result = this.folderBrowserDialog1.ShowDialog();
                if (result.ToString() == "Cancel")
                    return;

                dataGridView_ImportPathes.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = folderBrowserDialog1.SelectedPath;
            }
        }

        private void SaveAllImportPathes()
        {
            // need to save back all the rows
            DBImportPath.ClearAll();

            foreach (DataGridViewRow row in dataGridView_ImportPathes.Rows)
            {
                if (row.Index != dataGridView_ImportPathes.NewRowIndex)
                {
                    DBImportPath importPath = new DBImportPath();
                    importPath[DBImportPath.cIndex] = row.Index.ToString();
                    foreach (DataGridViewCell cell in row.Cells)
                        importPath[cell.OwningColumn.Name] = (String)cell.Value;
                    importPath.Commit();
                }
            }
        }

        private void dataGridView_ImportPathes_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            SaveAllImportPathes();
        }
        #endregion

        #region Expressions Handling
        private void dataGridView_Expressions_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DBExpression expression = new DBExpression();
            expression[DBExpression.cIndex] = e.RowIndex.ToString();
            foreach (DataGridViewCell cell in dataGridView_Expressions.Rows[e.RowIndex].Cells)
            {
                if (cell.Value == null)
                    return;
                if (cell.ValueType.Name == "Boolean")
                    expression[cell.OwningColumn.Name] = (Boolean)cell.Value;
                else
                    expression[cell.OwningColumn.Name] = (String)cell.Value;
            }
            expression.Commit();
        }

        private void SaveAllExpressions()
        {
            // need to save back all the rows
            DBExpression.ClearAll();

            foreach (DataGridViewRow row in dataGridView_Expressions.Rows)
            {
                if (row.Index != dataGridView_Expressions.NewRowIndex)
                {
                    DBExpression expression = new DBExpression();
                    expression[DBExpression.cIndex] = row.Index.ToString();
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value == null)
                            return;
                        if (cell.ValueType.Name == "Boolean")
                            expression[cell.OwningColumn.Name] = (Boolean)cell.Value;
                        else
                            expression[cell.OwningColumn.Name] = (String)cell.Value;
                    }
                }
            }
        }

        private void dataGridView_Expressions_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            SaveAllExpressions();
        }

        private void button_MoveExpUp_Click(object sender, EventArgs e)
        {
            int nCurrentRow = dataGridView_Expressions.CurrentCellAddress.Y;
            if (nCurrentRow > 0) 
            {
                DBExpression expressionGoingUp = new DBExpression(nCurrentRow);
                DBExpression expressionGoingDown = new DBExpression(nCurrentRow - 1);
                expressionGoingUp[DBExpression.cIndex] = Convert.ToString(nCurrentRow - 1);
                expressionGoingUp.Commit();
                expressionGoingDown[DBExpression.cIndex] = Convert.ToString(nCurrentRow);
                expressionGoingDown.Commit();
                LoadExpressions();
                dataGridView_Expressions.CurrentCell = dataGridView_Expressions.Rows[nCurrentRow - 1].Cells[dataGridView_Expressions.CurrentCellAddress.X];

            }
        }

        private void button_MoveExpDown_Click(object sender, EventArgs e)
        {
            int nCurrentRow = dataGridView_Expressions.CurrentCellAddress.Y;
            if (nCurrentRow < dataGridView_Expressions.Rows.Count - 2) //don't take in account the new line 
            {
                DBExpression expressionGoingDown = new DBExpression(nCurrentRow);
                DBExpression expressionGoingUp = new DBExpression(nCurrentRow + 1);
                expressionGoingUp[DBExpression.cIndex] = Convert.ToString(nCurrentRow);
                expressionGoingUp.Commit();
                expressionGoingDown[DBExpression.cIndex] = Convert.ToString(nCurrentRow+1);
                expressionGoingDown.Commit();
                LoadExpressions();
                dataGridView_Expressions.CurrentCell = dataGridView_Expressions.Rows[nCurrentRow + 1].Cells[dataGridView_Expressions.CurrentCellAddress.X];
            }
        }
        #endregion

        #region Replacements Handling
        private void dataGridView_Replace_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DBReplacements replacement = new DBReplacements();
            replacement[DBReplacements.cIndex] = e.RowIndex.ToString();
            foreach (DataGridViewCell cell in dataGridView_Replace.Rows[e.RowIndex].Cells)
            {
                if (cell.Value == null)
                    return;
                if (cell.ValueType.Name == "Boolean")
                    replacement[cell.OwningColumn.Name] = (Boolean)cell.Value;
                else
                    replacement[cell.OwningColumn.Name] = (String)cell.Value;
            }
            replacement.Commit();
        }

        private void SaveAllReplacements()
        {
            // need to save back all the rows
            DBReplacements.ClearAll();

            foreach (DataGridViewRow row in dataGridView_Expressions.Rows)
            {
                if (row.Index != dataGridView_Expressions.NewRowIndex)
                {
                    DBReplacements replacement = new DBReplacements();
                    replacement[DBReplacements.cIndex] = row.Index.ToString();
                    foreach (DataGridViewCell cell in row.Cells)
                        replacement[cell.OwningColumn.Name] = (String)cell.Value;
                    replacement.Commit();
                }
            }
        }

        private void dataGridView_Replace_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            SaveAllReplacements();
        }
        #endregion

        #region Test Parsing Handling
        void TestParsing_FillList(List<parseResult> results)
        {
            foreach (parseResult progress in results)
            {
                foreach (KeyValuePair<String, String> MatchPair in progress.parser.Matches)
                {
                    if (!listView_ParsingResults.Columns.ContainsKey(MatchPair.Key))
                    {
                        // add a column for that match
                        ColumnHeader newcolumn = new ColumnHeader();
                        newcolumn.Name = MatchPair.Key;
                        newcolumn.Text = MatchPair.Key;
                        listView_ParsingResults.Columns.Add(newcolumn);
                    }
                }

                ListViewItem item = new ListViewItem(progress.match_filename);
                item.SubItems[0].Name = listView_ParsingResults.Columns[0].Name;



                foreach (ColumnHeader column in listView_ParsingResults.Columns)
                {
                    if (column.Index > 0)
                    {
                        ListViewItem.ListViewSubItem subItem = null;
                        if (progress.parser.Matches.ContainsKey(column.Name))
                            subItem = new ListViewItem.ListViewSubItem(item, progress.parser.Matches[column.Name]);
                        else
                            subItem = new ListViewItem.ListViewSubItem(item, "");
                        subItem.Name = column.Name;
                        item.SubItems.Add(subItem);
                    }
                }

                if (progress.failedSeason)
                {
                    item.UseItemStyleForSubItems = false;
                    item.SubItems[DBEpisode.cSeasonIndex].ForeColor = System.Drawing.Color.White;
                    item.SubItems[DBEpisode.cSeasonIndex].BackColor = System.Drawing.Color.Tomato;
                }

                if (progress.failedEpisode)
                {
                    item.UseItemStyleForSubItems = false;
                    item.SubItems[DBEpisode.cEpisodeIndex].ForeColor = System.Drawing.Color.White;
                    item.SubItems[DBEpisode.cEpisodeIndex].BackColor = System.Drawing.Color.Tomato;
                }

                if (!progress.success && !progress.failedEpisode && !progress.failedSeason)
                {
                    item.ForeColor = System.Drawing.Color.White;
                    item.BackColor = System.Drawing.Color.Tomato;
                }

                if (!progress.success)
                    MPTVSeriesLog.Write("Parsing failed for " + progress.match_filename);
                if (progress.failedSeason || progress.failedEpisode)
                    MPTVSeriesLog.Write(progress.exception + " for " + progress.match_filename);
                listView_ParsingResults.Items.Add(item);
                listView_ParsingResults.EnsureVisible(listView_ParsingResults.Items.Count - 1);
                // only do that once in a while, it's really slow
            }
            listView_ParsingResults.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            foreach (ColumnHeader header in listView_ParsingResults.Columns)
            {
                header.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                header.Width += 10;
                if (header.Width < 80)
                    header.Width = 80;
            }
        }

        void TestParsing_LocalParseCompleted(List<parseResult> results)
        {
            TestParsing_FillList(results);
            MPTVSeriesLog.Write("Parsing test completed");
            this.progressBar_Parsing.Value = 100;
        }

        void TestParsing_LocalParseProgress(int nProgress, List<parseResult> results)
        {
            this.progressBar_Parsing.Value = nProgress;
            TestParsing_FillList(results);
        }

        void TestParsing_Start(bool bForceRefresh)
        {
            if (!bForceRefresh && listView_ParsingResults.Items.Count > 0)
                return;

            listView_ParsingResults.Items.Clear();
            listView_ParsingResults.Columns.Clear();
            // add mandatory columns
            ColumnHeader columnFileName = new ColumnHeader();
            columnFileName.Name = "FileName";
            columnFileName.Text = "FileName";
            listView_ParsingResults.Columns.Add(columnFileName);

            ColumnHeader columnSeriesName = new ColumnHeader();
            columnSeriesName.Name = DBSeries.cParsedName;
            columnSeriesName.Text = "Parsed Series Name";
            listView_ParsingResults.Columns.Add(columnSeriesName);

            ColumnHeader columnSeasonNumber = new ColumnHeader();
            columnSeasonNumber.Name = DBEpisode.cSeasonIndex;
            columnSeasonNumber.Text = "Season ID";
            listView_ParsingResults.Columns.Add(columnSeasonNumber);

            ColumnHeader columnEpisodeNumber = new ColumnHeader();
            columnEpisodeNumber.Name = DBEpisode.cEpisodeIndex;
            columnEpisodeNumber.Text = "Episode ID";
            listView_ParsingResults.Columns.Add(columnEpisodeNumber);

            ColumnHeader columnEpisodeTitle = new ColumnHeader();
            columnEpisodeTitle.Name = DBEpisode.cEpisodeName;
            columnEpisodeTitle.Text = "Episode Title";
            listView_ParsingResults.Columns.Add(columnEpisodeTitle);

            MPTVSeriesLog.Write("Parsing test beginning, getting all files...");

            LocalParse runner = new LocalParse();
            runner.LocalParseProgress += new LocalParse.LocalParseProgressHandler(TestParsing_LocalParseProgress);
            runner.LocalParseCompleted += new LocalParse.LocalParseCompletedHandler(TestParsing_LocalParseCompleted);
            runner.DoParse(true);
        }
        #endregion

        private void Parsing_Start()
        {
            if (m_parser != null)
            {
                m_parser.Cancel();
                button_Start.Enabled = false;
            }
            else
            {
                button_Start.Text = "Abort";
                m_timingStart = DateTime.Now;
                m_parser = new OnlineParsing();
                m_parser.OnlineParsingProgress += new OnlineParsing.OnlineParsingProgressHandler(runner_OnlineParsingProgress);
                m_parser.OnlineParsingCompleted += new OnlineParsing.OnlineParsingCompletedHandler(runner_OnlineParsingCompleted);
                m_parser.Start(true, true);
            }
        }

        void runner_OnlineParsingProgress(int nProgress)
        {
            this.progressBar_Parsing.Value = nProgress;
        }

        void runner_OnlineParsingCompleted(bool bDataUpdated)
        {
            this.progressBar_Parsing.Value = 100;
            TimeSpan span = DateTime.Now - m_timingStart;
            MPTVSeriesLog.Write("Parsing Completed in " + span);
            button_Start.Text = "Start Import";
            button_Start.Enabled = true;

            m_parser.OnlineParsingProgress -= new OnlineParsing.OnlineParsingProgressHandler(runner_OnlineParsingProgress);
            m_parser.OnlineParsingCompleted -= new OnlineParsing.OnlineParsingCompletedHandler(runner_OnlineParsingCompleted);
            m_parser = null;

            // a full configuration scan counts as a scan - set the dates so we don't rescan everything right away in MP
            DBOption.SetOptions(DBOption.cLocalScanLastTime, DateTime.Now.ToString());
            DBOption.SetOptions(DBOption.cUpdateScanLastTime, DateTime.Now.ToString());

            LoadTree();
        }
        
        #region Series treeview handling
        private void treeView_Library_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //////////////////////////////////////////////////////////////////////////////
            #region Clears all fields so new data can be entered

            this.detailsPropertyBindingSource.Clear();
            try
            {
                if (this.pictureBox_Series.Image != null)
                {
                    this.pictureBox_Series.Image.Dispose();
                    this.pictureBox_Series.Image = null;
                }
            }
            catch { }

            #endregion
            //////////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////////
            #region Select appropriate tab base on which node level was clicked

            TreeNode node = e.Node;
            switch (node.Name)
            {
                //////////////////////////////////////////////////////////////////////////////
                #region When Episode Nodes is Clicked

                case DBEpisode.cTableName:
                    {
                        DBEpisode episode = (DBEpisode)node.Tag;
                        // assume an episode is always in a season which is always in a series
                        DBSeries series = (DBSeries)node.Parent.Parent.Tag;

                        String filename = series.Banner;
                        if (filename != String.Empty)
                            this.pictureBox_Series.Image = Image.FromFile(filename);

                        // go over all the fields, (and update only those which haven't been modified by the user - will do that later)
                        foreach (String key in episode.FieldNames)
                        {
                            switch (key)
                            {
                                case DBEpisode.cSeasonIndex:
                                case DBEpisode.cEpisodeIndex:
                                case DBEpisode.cSeriesParsedName:
                                case DBEpisode.cCompositeID:
                                case DBEpisode.cFilename:
                                case DBOnlineEpisode.cID:
                                    AddPropertyBindingSource(DBEpisode.PrettyFieldName(key), key, episode[key], false);
                                    break;

                                case DBEpisode.cEpisodeName:
                                    AddPropertyBindingSource(DBEpisode.PrettyFieldName(key), DBOnlineEpisode.cEpisodeName, episode[key]);
                                    break;

                                case DBOnlineEpisode.cEpisodeName:
                                case DBEpisode.cImportProcessed:
                                case DBOnlineEpisode.cOnlineDataImported:
                                    // hide those, they are handled internally
                                    break;

                                default:
                                    AddPropertyBindingSource(DBEpisode.PrettyFieldName(key), key, episode[key]);
                                    break;

                            }
                        }
                    }
                    break;

                #endregion
                //////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////
                #region When Season Nodes is Clicked

                case DBSeason.cTableName:
                    {
                        DBSeason season = (DBSeason)node.Tag;

                        comboBox_BannerSelection.Items.Clear();
                        // populate banner dropdown
                        foreach (String filename in season.BannerList)
                        {
                            BannerComboItem newItem = new BannerComboItem(Path.GetFileName(filename), filename);
                            comboBox_BannerSelection.Items.Add(newItem);
                        }

                        if (season.Banner != String.Empty)
                        {
                            this.pictureBox_Series.Image = Image.FromFile(season.Banner);
                            foreach (BannerComboItem comboItem in comboBox_BannerSelection.Items)
                                if (comboItem.sFullPath == season.Banner)
                                {
                                    comboBox_BannerSelection.SelectedItem = comboItem;
                                    break;
                                }
                        }

                        // go over all the fields, (and update only those which haven't been modified by the user - will do that later)
                        foreach (String key in season.FieldNames)
                        {
                            switch (key)
                            {
                                case DBSeason.cBannerFileNames:
                                case DBSeason.cCurrentBannerFileName:
                                case DBSeason.cHasLocalFiles:
                                case DBSeason.cHasLocalFilesTemp:
                                    // hide those, they are handled internally
                                    break;

                                default:
                                    AddPropertyBindingSource(DBSeason.PrettyFieldName(key), key, season[key], false);
                                    break;

                            }
                        }
                    }
                    break;
                #endregion

                //////////////////////////////////////////////////////////////////////////////
                #region When Series Nodes is Clicked

                case DBSeries.cTableName:
                    {
                        DBSeries series = (DBSeries)node.Tag;

                        comboBox_BannerSelection.Items.Clear();
                        // populate banner dropdown
                        foreach (String filename in series.BannerList)
                        {
                            BannerComboItem newItem = new BannerComboItem(Path.GetFileName(filename), filename);
                            comboBox_BannerSelection.Items.Add(newItem);
                        }

                        if (series.Banner != String.Empty)
                        {
                            this.pictureBox_Series.Image = Image.FromFile(series.Banner);
                            foreach (BannerComboItem comboItem in comboBox_BannerSelection.Items)
                                if (comboItem.sFullPath == series.Banner)
                                {
                                    comboBox_BannerSelection.SelectedItem = comboItem;
                                    break;
                                }
                        }

                        // go over all the fields, (and update only those which haven't been modified by the user - will do that later)
                        foreach (String key in series.FieldNames)
                        {
                            switch (key)
                            {
                                case DBSeries.cBannerFileNames:
                                case DBSeries.cBannersDownloaded:
                                case DBSeries.cCurrentBannerFileName:
                                case DBSeries.cHasLocalFiles:
                                case DBSeries.cHasLocalFilesTemp:
                                case DBSeries.cOnlineDataImported:
                                    // hide those, they are handled internally
                                    break;

                                case DBSeries.cParsedName:
                                case DBSeries.cID:
                                    AddPropertyBindingSource(DBEpisode.PrettyFieldName(key), key, series[key], false);
                                    break;


                                default:
                                    AddPropertyBindingSource(DBEpisode.PrettyFieldName(key), key, series[key]);
                                    break;

                            }
                        }
                    }
                    break;

                #endregion
                //////////////////////////////////////////////////////////////////////////////

            }
            #endregion
            //////////////////////////////////////////////////////////////////////////////
        }

        private void AddPropertyBindingSource(string FieldPrettyName, string FieldName, string FieldValue)
        {
            AddPropertyBindingSource(FieldPrettyName, FieldName, FieldValue, true, DataGridViewContentAlignment.MiddleLeft);
        }

        private void AddPropertyBindingSource(string FieldPrettyName, string FieldName, string FieldValue, bool CanModify)
        {
            AddPropertyBindingSource(FieldPrettyName, FieldName, FieldValue, CanModify, DataGridViewContentAlignment.MiddleLeft);
        }

        private void AddPropertyBindingSource(string FieldPrettyName, string FieldName, string FieldValue, bool CanModify, DataGridViewContentAlignment TextAlign)
        {
            int id = this.detailsPropertyBindingSource.Add(new DetailsProperty(FieldPrettyName, FieldValue));

            DataGridViewCell cell = this.dataGridView1.Rows[id].Cells[0];
            cell.ReadOnly = true;

            cell = this.dataGridView1.Rows[id].Cells[1];
            cell.Tag = FieldName;
            if (!CanModify)
            {
                cell.ReadOnly = true;
                cell.Style.BackColor = System.Drawing.SystemColors.Control;
            }

            cell.Style.Alignment = TextAlign;
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            nodeEdited = treeView_Library.SelectedNode;
            /*
            if (this.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString() == "Filename")
            {
                openFileDialog1.FileName = this.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                openFileDialog1.ShowDialog();
                if (this.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString() != openFileDialog1.FileName)
                {
                    this.dataGridView1.Rows[e.RowIndex].Cells[1].Value = openFileDialog1.FileName;
                    m_PropertySaveRequired = true;
                }
                e.Cancel = true;
                return;
            }

            if (this.treeView_Library.Nodes.Count > 0)
                m_PropertySaveRequired = true;
            */
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[1];
            switch (nodeEdited.Name)
            {
                case DBSeries.cTableName:
                    DBSeries series = (DBSeries)nodeEdited.Tag;
                    series[(String)cell.Tag] = (String)cell.Value;
                    series.Commit();
                    if (series[DBSeries.cPrettyName] != String.Empty)
                        nodeEdited.Text = series[DBSeries.cPrettyName];
                    break;

                case DBSeason.cTableName:
                    DBSeason season = (DBSeason)nodeEdited.Tag;
                    season[(String)cell.Tag] = (String)cell.Value;
                    season.Commit();
                    break;

                case DBEpisode.cTableName:
                    DBEpisode episode = (DBEpisode)nodeEdited.Tag;
                    episode[(String)cell.Tag] = (String)cell.Value;
                    episode.Commit();
                    if (episode[DBEpisode.cEpisodeName] != String.Empty)
                        nodeEdited.Text = episode[DBEpisode.cSeasonIndex] + "x" + episode[DBEpisode.cEpisodeIndex] + " - " + episode[DBEpisode.cEpisodeName];
                    break;
            }
        }
        #endregion

        #region UI actions

        private void treeView_Settings_AfterSelect(object sender, TreeViewEventArgs e)
        {
            foreach (Panel pane in m_paneList)
            {
                if (pane.Name == e.Node.Name)
                {
                    pane.Visible = true;
                }
                else
                    pane.Visible = false;
            }

            // special behavior for some nodes
            if (e.Node.Name == panel_ParsingTest.Name)
                TestParsing_Start(false);
        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            Parsing_Start();
        }

        private void button_TestReparse_Click(object sender, EventArgs e)
        {
            TestParsing_Start(true);
        }
        #endregion

        private void checkBox_OnlineSearch_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cOnlineParseEnabled, checkBox_OnlineSearch.Checked);
        }

        private void checkBox_FullSeriesRetrieval_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cFullSeriesRetrieval, checkBox_FullSeriesRetrieval.Checked);
        }

        private void checkBox_AutoChooseSeries_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cAutoChooseSeries, checkBox_AutoChooseSeries.Checked);
        }

        private void checkBox_LocalDataOverride_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cLocalDataOverride, checkBox_LocalDataOverride.Checked);
        }

        private void comboBox_BannerSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (treeView_Library.SelectedNode.Name)
            {
                case DBSeries.cTableName:
                    {
                        DBSeries series = (DBSeries)treeView_Library.SelectedNode.Tag;
                        series.Banner = ((BannerComboItem)comboBox_BannerSelection.SelectedItem).sFullPath;
                        this.pictureBox_Series.Image = Image.FromFile(series.Banner);
                        series.Commit();
                    }
                    break;

                case DBSeason.cTableName:
                    {
                        DBSeason season = (DBSeason)treeView_Library.SelectedNode.Tag;
                        season.Banner = ((BannerComboItem)comboBox_BannerSelection.SelectedItem).sFullPath;
                        this.pictureBox_Series.Image = Image.FromFile(season.Banner);
                        season.Commit();
                    }
                    break;

                case DBEpisode.cTableName:
                    {
                        DBSeries series = (DBSeries)treeView_Library.SelectedNode.Parent.Parent.Tag;
                        series.Banner = ((BannerComboItem)comboBox_BannerSelection.SelectedItem).sFullPath;
                        this.pictureBox_Series.Image = Image.FromFile(series.Banner);
                        series.Commit();
                    }
                    break;
            }   
        }

        private void comboBox_BannerSelection_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void checkBox_Episode_MatchingLocalFile_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cView_Episode_OnlyShowLocalFiles, checkBox_Episode_OnlyShowLocalFiles.Checked);
        }

        private void checkBox_Episode_HideUnwatchedSummary_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cView_Episode_HideUnwatchedSummary, checkBox_Episode_HideUnwatchedSummary.Checked);
        }

        private void numericUpDown_AutoScanLocal_ValueChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cAutoScanLocalFilesLapse, (int)numericUpDown_AutoScanLocal.Value);
        }

        private void checkBox_AutoScanLocal_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cAutoScanLocalFiles, checkBox_AutoScanLocal.Checked);
            numericUpDown_AutoScanLocal.Enabled = checkBox_AutoScanLocal.Checked;
        }

        private void checkBox_AutoOnlineDataRefresh_CheckedChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cAutoUpdateOnlineData, checkBox_AutoOnlineDataRefresh.Checked);
            numericUpDown_AutoOnlineDataRefresh.Enabled = checkBox_AutoOnlineDataRefresh.Checked;
        }

        private void numericUpDown_AutoOnlineDataRefresh_ValueChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cAutoUpdateOnlineDataLapse, (int)numericUpDown_AutoOnlineDataRefresh.Value);
        }

        private void treeView_Library_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                TreeNode nodeDeleted = treeView_Library.SelectedNode;
                switch (nodeDeleted.Name)
                {
                    case DBSeries.cTableName:
                        if (MessageBox.Show("Are you sure you want to delete that series and all the underlying seasons and episodes?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            DBSeries series = (DBSeries)nodeDeleted.Tag;
                            SQLCondition condition = new SQLCondition(new DBEpisode());
                            condition.Add(DBEpisode.cSeriesParsedName, series[DBSeries.cParsedName], true);
                            DBEpisode.Clear(condition);
                            condition = new SQLCondition(new DBOnlineEpisode());
                            condition.Add(DBOnlineEpisode.cSeriesParsedName, series[DBSeries.cParsedName], true);
                            DBOnlineEpisode.Clear(condition);

                            condition = new SQLCondition(new DBSeason());
                            condition.Add(DBSeason.cSeriesName, series[DBSeries.cParsedName], true);
                            DBSeason.Clear(condition);

                            condition = new SQLCondition(new DBSeries());
                            condition.Add(DBSeries.cParsedName, series[DBSeries.cParsedName], true);
                            DBSeries.Clear(condition);

                            treeView_Library.Nodes.Remove(nodeDeleted);
                        }
                        break;

                    case DBSeason.cTableName:
                        if (MessageBox.Show("Are you sure you want to delete that season and all the underlying episodes?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            DBSeason season = (DBSeason)nodeDeleted.Tag;

                            SQLCondition condition = new SQLCondition(new DBEpisode());
                            condition.Add(DBEpisode.cSeriesParsedName, season[DBSeason.cSeriesName], true);
                            condition.Add(DBEpisode.cSeasonIndex, season[DBSeason.cIndex], true);
                            DBEpisode.Clear(condition);
                            condition = new SQLCondition(new DBOnlineEpisode());
                            condition.Add(DBOnlineEpisode.cSeriesParsedName, season[DBSeason.cSeriesName], true);
                            condition.Add(DBOnlineEpisode.cSeasonIndex, season[DBSeason.cIndex], true);
                            DBOnlineEpisode.Clear(condition);

                            condition = new SQLCondition(new DBSeason());
                            condition.Add(DBSeason.cID, season[DBSeason.cID], true);
                            DBSeason.Clear(condition);

                            treeView_Library.Nodes.Remove(nodeDeleted);
                        }
                        break;

                    case DBEpisode.cTableName:
                        if (MessageBox.Show("Are you sure you want to delete that episode?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            DBEpisode episode = (DBEpisode)nodeDeleted.Tag;
                            SQLCondition condition = new SQLCondition(new DBEpisode());
                            condition.Add(DBEpisode.cEpisodeName, episode[DBEpisode.cEpisodeName], true);
                            DBEpisode.Clear(condition);
                            condition = new SQLCondition(new DBOnlineEpisode());
                            condition.Add(DBOnlineEpisode.cEpisodeName, episode[DBOnlineEpisode.cEpisodeName], true);
                            DBOnlineEpisode.Clear(condition);
                            treeView_Library.Nodes.Remove(nodeDeleted);
                        }
                        break;
                }
                if (treeView_Library.Nodes.Count == 0)
                {
                    // also clear the data pane
                    this.detailsPropertyBindingSource.Clear();
                    try
                    {
                        if (this.pictureBox_Series.Image != null)
                        {
                            this.pictureBox_Series.Image.Dispose();
                            this.pictureBox_Series.Image = null;
                        }
                    }
                    catch { }
                }
            }
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

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            RichTextBox textBox = sender as RichTextBox;
            FieldValidate(ref textBox);
        }
        
        private void contextMenuStrip_SeriesFields_Opening(object sender, CancelEventArgs e)
        {
            // Acquire references to the owning control and item.
            RichTextBox textBox = contextMenuStrip_InsertFields.SourceControl as RichTextBox;

            // Clear the ContextMenuStrip control's Items collection.
            contextMenuStrip_InsertFields.Items.Clear();
            contextMenuStrip_InsertFields.CanOverflow = true;
            
            contextMenuStrip_InsertFields.Items.Add("Add a field Value:");
            contextMenuStrip_InsertFields.Items[0].Enabled = false;
            // Populate the ContextMenuStrip control with its default items.
            contextMenuStrip_InsertFields.Items.Add("-");
            contextMenuStrip_InsertFields.Items[1].Enabled = false;

            FieldTag tag = textBox.Tag as FieldTag;

            // series' always there
            {
                ToolStripMenuItem subMenuItem = new ToolStripMenuItem(DBSeries.cOutName + " values");
                ContextMenuStrip subMenu = new ContextMenuStrip(this.components);
                subMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
                subMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip_SeriesFields_ItemClicked);
                subMenuItem.DropDown = subMenu;
                List<String> fieldList = m_SeriesReference.FieldNames;
                fieldList.Remove(DBSeries.cHasLocalFiles);
                fieldList.Remove(DBSeries.cHasLocalFilesTemp);
                fieldList.Remove(DBSeries.cBannerFileNames);
                fieldList.Remove(DBSeries.cBannersDownloaded);
                fieldList.Remove(DBSeries.cCurrentBannerFileName);
                fieldList.Remove(DBSeries.cOnlineDataImported);

                foreach (String sField in m_SeriesReference.FieldNames)
                {
                    ToolStripItem item = new ToolStripLabel();
                    item.Name = "<" + DBSeries.cOutName + "." + sField + ">";
                    item.Tag = textBox;
                    String sPretty = DBSeries.PrettyFieldName(sField);
                    if (sPretty == sField)
                        item.Text = item.Name;
                    else
                        item.Text = item.Name + " - (" + sPretty + ")";
                    subMenu.Items.Add(item);
                }
                contextMenuStrip_InsertFields.Items.Add(subMenuItem);
            }

            // season
            if (tag.m_Level == FieldTag.Level.Season || tag.m_Level == FieldTag.Level.Episode)
            {
                ToolStripMenuItem subMenuItem = new ToolStripMenuItem(DBSeason.cOutName + " values");
                ContextMenuStrip subMenu = new ContextMenuStrip(this.components);
                subMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
                subMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip_SeriesFields_ItemClicked);
                subMenuItem.DropDown = subMenu;
                List<String> fieldList = m_SeasonReference.FieldNames;
                fieldList.Remove(DBSeason.cHasLocalFiles);
                fieldList.Remove(DBSeason.cHasLocalFilesTemp);
                fieldList.Remove(DBSeason.cBannerFileNames);
                fieldList.Remove(DBSeason.cCurrentBannerFileName);
                foreach (String sField in m_SeasonReference.FieldNames)
                {
                    ToolStripItem item = new ToolStripLabel();
                    item.Name = "<" + DBSeason.cOutName + "." + sField + ">";
                    item.Tag = textBox;
                    String sPretty = DBSeason.PrettyFieldName(sField);
                    if (sPretty == sField)
                        item.Text = item.Name;
                    else
                        item.Text = item.Name + " - (" + sPretty + ")";
                    subMenu.Items.Add(item);
                }
                contextMenuStrip_InsertFields.Items.Add(subMenuItem);
            }

            // episode
            if (tag.m_Level == FieldTag.Level.Episode)
            {
                ToolStripMenuItem subMenuItem = new ToolStripMenuItem(DBEpisode.cOutName + " values");
                ContextMenuStrip subMenu = new ContextMenuStrip(this.components);
                subMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
                subMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip_SeriesFields_ItemClicked);
                subMenuItem.DropDown = subMenu;
                List<String> fieldList = m_EpisodeReference.FieldNames;
                fieldList.Remove(DBEpisode.cImportProcessed);
                fieldList.Remove(DBOnlineEpisode.cOnlineDataImported);
                foreach (String sField in m_EpisodeReference.FieldNames)
                {
                    ToolStripItem item = new ToolStripLabel();
                    item.Name = "<" + DBEpisode.cOutName + "." + sField + ">";
                    item.Tag = textBox;
                    String sPretty = DBEpisode.PrettyFieldName(sField);
                    if (sPretty == sField)
                        item.Text = item.Name;
                    else
                        item.Text = item.Name + " - (" + sPretty + ")";
                    subMenu.Items.Add(item);
                }
                contextMenuStrip_InsertFields.Items.Add(subMenuItem);
            }

            // Set Cancel to false. 
            // It is optimized to true based on empty entry.
            e.Cancel = false;
        }

        private void contextMenuStrip_SeriesFields_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // Acquire references to the owning control and item.
            RichTextBox textBox = e.ClickedItem.Tag as RichTextBox;
            if (textBox != null)
                textBox.Text = textBox.Text.Insert(textBox.SelectionStart, e.ClickedItem.Name);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex == 0) MPTVSeriesLog.selectedLogLevel = MPTVSeriesLog.LogLevel.Normal;
            else if (this.comboBox1.SelectedIndex == 1) MPTVSeriesLog.selectedLogLevel = MPTVSeriesLog.LogLevel.Debug;
            else MPTVSeriesLog.selectedLogLevel = MPTVSeriesLog.LogLevel.Normal;
        }
    }

    public class BannerComboItem
    {
        public String sName = String.Empty;
        public String sFullPath;

        public BannerComboItem(String sName, String sFullPath)
        {
            this.sName = sName;
            this.sFullPath = sFullPath;
        }

        public override String ToString()
        {
            return sName;
        }

    };

    public class FieldTag
    {
        public String m_sOptionName;
        public Level m_Level;
        public bool m_bInited = false;

        public enum Level
        {
            Series,
            Season,
            Episode
        }

        public FieldTag(String optionName, Level level)
        {
            m_sOptionName = optionName;
            m_Level = level;
        }
    };

    public class DetailsProperty
    {
        String m_Property = String.Empty;
        String m_Value = String.Empty;

        public DetailsProperty(String property, String value)
        {
            this.m_Property = property;
            this.m_Value = value;
        }

        public String Property
        {
            get
            {
                return this.m_Property;
            }
            set
            {
                this.m_Property = value;
            }
        }
        public String Value
        {
            get
            {
                return this.m_Value;
            }
            set
            {
                this.m_Value = value;
            }
        }
    }
}

