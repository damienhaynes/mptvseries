using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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
        private const String sNodeImportPathes ="import_pathes";
        private const String sNodeExpressions = "import_pathes";
        private const String sNodeTest = "import_pathes";

        private bool m_PropertySaveRequired = false;

        private List<Panel> m_paneList = new List<Panel>();

        public ConfigurationForm()
        {
#if DEBUG
//    Debugger.Launch();
#endif


            InitializeComponent();
            DBTVSeries.AttachLog(ref listBox_Log);
            DBTVSeries.Log("**** Plugin started in configuration mode ***");
            InitSettingsTreeAndPanes();

            LoadImportPathes();
            LoadExpressions();
            LoadTree();

            String sInterface = DBOnlineMirror.Interface;


//             this.m_Database.Open();
//             this.treeView_Library.Select();
//             this.LoadTree();
        }

        private void InitSettingsTreeAndPanes()
        {
            m_paneList.Add(panel_ImportPathes);
            m_paneList.Add(panel_Expressions);
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
        }

        public void StartParsingTest(bool bForceRefresh)
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

            listBox_Results.Items.Clear();
            listBox_Results.Items.Add("Getting all files...");
            listBox_Results.Refresh();


            String[] files = Filelister.GetFiles();
            int nFailed = 0;
            int nCount = 0;
            foreach (String file in files)
            {
                FilenameParser parser = new FilenameParser(file);
                ListViewItem item = new ListViewItem(file);
                item.UseItemStyleForSubItems = true;

                foreach (KeyValuePair<string, string> match in parser.Matches)
                {
                    if (!listView_ParsingResults.Columns.ContainsKey(match.Key))
                    {
                        // add a column for that match
                        ColumnHeader newcolumn = new ColumnHeader();
                        newcolumn.Name = match.Key;
                        newcolumn.Text = match.Key;
                        listView_ParsingResults.Columns.Add(newcolumn);
                    }

                    ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem(item, match.Value);
                    subItem.Name = match.Key;
                    item.SubItems.Add(subItem);
                }

                // make sure we have all the necessary data for a full match
                if (!parser.Matches.ContainsKey(DBEpisode.cSeasonIndex) ||
                    !parser.Matches.ContainsKey(DBEpisode.cEpisodeIndex) ||
                    !parser.Matches.ContainsKey(DBSeries.cParsedName))
                {
                    item.ForeColor = System.Drawing.Color.White;
                    item.BackColor = System.Drawing.Color.Tomato;
                    listBox_Results.Items.Add("Parsing failed for " + file);
                    nFailed++;

                }
                else
                {
                    // make sure episode & season are properly matched (numerical values)
                    try { Convert.ToInt32(parser.Matches[DBEpisode.cSeasonIndex]); }
                    catch (System.FormatException)
                    {
                        item.UseItemStyleForSubItems = false;
                        item.SubItems[DBEpisode.cSeasonIndex].ForeColor = System.Drawing.Color.White;
                        item.SubItems[DBEpisode.cSeasonIndex].BackColor = System.Drawing.Color.Tomato;
                        nFailed++;
                        listBox_Results.Items.Add("Season not numerical for " + file);
                    }
                    try { Convert.ToInt32(parser.Matches[DBEpisode.cEpisodeIndex]); }
                    catch (System.FormatException)
                    {
                        item.UseItemStyleForSubItems = false;
                        item.SubItems[DBEpisode.cEpisodeIndex].ForeColor = System.Drawing.Color.White;
                        item.SubItems[DBEpisode.cEpisodeIndex].BackColor = System.Drawing.Color.Tomato;
                        nFailed++;
                        listBox_Results.Items.Add("Episode not numerical for " + file);
                    }
                }

                listView_ParsingResults.Items.Add(item);
                listView_ParsingResults.EnsureVisible(listView_ParsingResults.Items.Count - 1);
                // only do that once in a while, it's really slow

                if (nCount % 10 == 0)
                    listView_ParsingResults.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                nCount++;
            }

            foreach (ColumnHeader header in listView_ParsingResults.Columns)
            {
                header.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                header.Width += 10;
                if (header.Width < 80)
                    header.Width = 80;
            }

            listBox_Results.Items.Add(nFailed + " failed out of " + files.Length + " parsed files.");
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

        private void dataGridView_ImportPathes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DBImportPath importPath = new DBImportPath();
            importPath[DBImportPath.cIndex] = e.RowIndex.ToString();
            foreach (DataGridViewCell cell in dataGridView_ImportPathes.Rows[e.RowIndex].Cells)
            {
                if (cell.Value == null)
                    return;
                if (cell.ValueType.Name == "Boolean")
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
                    dataGridView_ImportPathes.Rows.Add();

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
                expression[DBExpression.cExpression] = @"<series> - <season>x<episode> - <title>";
                expression.Commit();

                expression[DBExpression.cIndex] = "1";
                expression[DBExpression.cExpression] = @"\<series>\Season <season>\Episode <episode> - <title>";
                expression.Commit();

                expression[DBExpression.cType] = DBExpression.cType_Regexp;
                expression[DBExpression.cIndex] = "2";
                expression[DBExpression.cExpression] = @"(?<series>[^\\\[]*) - \[(?<season>[0-9]{1,2})x(?<episode>[0-9\W]+)\](( |)(-( |)|))(?<title>[^$]*?)";
                expression.Commit();

                expression[DBExpression.cIndex] = "3";
                expression[DBExpression.cExpression] = @"(?<series>[^\\$]*) - season (?<season>[0-9]{1,2}) - (?<title>[^$]*?)";
                expression.Commit();

                expression[DBExpression.cIndex] = "4";
                expression[DBExpression.cExpression] = @"^(?<series>[^\\$]+)\\[^\\$]*?(?:s(?<season>[0-1]?[0-9])e(?<episode>[0-9]{2})|(?<season>(?:[0-1][0-9]|(?<!\d)[0-9]))x?(?<episode>[0-9]{2}))(?!\d)[ \-\.]*(?<title>[^\\]*?)\.(?:[^.]*)$";
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
                        expression[cell.OwningColumn.Name] = (String)cell.Value;
                    expression.Commit();
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
                StartParsingTest(false);
        }


        private void LocalParse()
        {
            // mark all files in the db as not processed (to figure out which ones we'll have to remove after the import)
            DBEpisode.GlobalSet(DBEpisode.cImportProcessed, 2);
            
            // retrieve all local files, and import
            String[] files = Filelister.GetFiles();
            foreach (String file in files)
            {
                FilenameParser parser = new FilenameParser(file);

                // make sure we have all the necessary data for a full match
                if (parser.Matches.ContainsKey(DBEpisode.cSeasonIndex) &&
                    parser.Matches.ContainsKey(DBEpisode.cEpisodeIndex) &&
                    parser.Matches.ContainsKey(DBSeries.cParsedName))
                {
                    // make sure episode & season are properly matched (numerical values)
                    int nSeason = 0;
                    int nEpisode = 0;
                    bool bProperFormat = true;
                    try 
                    {
                        nEpisode = Convert.ToInt32(parser.Matches[DBEpisode.cEpisodeIndex]);
                        nSeason = Convert.ToInt32(parser.Matches[DBEpisode.cSeasonIndex]); 
                    }
                    catch (System.FormatException)
                    {
                        bProperFormat = false;
                    }
                    if (bProperFormat)
                    {
                        // ok, we are sure it's valid now
                        // series first
                        DBSeries series = new DBSeries(parser.Matches[DBSeries.cParsedName]);
                        // not much to do here except commiting the series
                        series.Commit();

                        // season now
                        DBSeason season = new DBSeason(parser.Matches[DBSeries.cParsedName], nSeason);
                        season.Commit();

                        // then episode
                        DBEpisode episode = new DBEpisode(file);
                        bool bNewFile = false;
                        if (episode[DBEpisode.cImportProcessed] != 2)
                        {
                            bNewFile = true;
                        }
                        episode[DBEpisode.cImportProcessed] = 1;

                        foreach (KeyValuePair<string, string> match in parser.Matches)
                        {
                            episode.AddColumn(match.Key, new DBField(DBField.cTypeString));
                            if (bNewFile || episode[match.Key].ToString() != match.Value)
                                episode[match.Key] = match.Value;
                        }
                        episode.Commit();
                    }
                }
            }

            // now, remove all episodes still processed = 0, the weren't find in the scan
            DBEpisode.Clear(DBEpisode.cImportProcessed, 2);

            // and refresh tree
            LoadTree();
        }

        private void OnlineParse()
        {

        }


        private void LoadTree()
        {
            TreeView root = this.treeView_Library;
            root.Nodes.Clear();

            List<DBSeries> seriesList = DBSeries.Get();
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

                List<DBSeason> seasonsList = DBSeason.Get(series[DBSeries.cParsedName].ToString());
                foreach (DBSeason season in seasonsList)
                {
                    TreeNode seasonNode = new TreeNode("Season " + season[DBSeason.cIndex]);
                    seasonNode.Name = DBSeason.cTableName;
                    seasonNode.Tag = (DBSeason)season;
                    seriesNode.Nodes.Add(seasonNode);


                    List<DBEpisode> episodesList = DBEpisode.Get((String)series[DBSeries.cParsedName], (int)season[DBSeason.cIndex], false);

                    foreach (DBEpisode episode in episodesList)
                    {
                        String sEpisodeName = (String)episode[DBEpisode.cEpisodeName];
                        TreeNode episodeNode = new TreeNode(episode[DBEpisode.cSeasonIndex] + "x" + episode[DBEpisode.cEpisodeIndex] + " - " + sEpisodeName);
                        episodeNode.Name = DBEpisode.cTableName;
                        episodeNode.Tag = (DBEpisode)episode;
//                         if (!System.IO.File.Exists(episode))
//                         {
//                             episodeNode.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
//                         }

                        seasonNode.Nodes.Add(episodeNode);
                    }
                }
            }
        }

        private void treeView_Library_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //////////////////////////////////////////////////////////////////////////////
            #region Clears all fields so new data can be entered

            this.m_PropertySaveRequired = false;
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
                    DBEpisode episode = (DBEpisode)node.Tag;
//                     String filename = this.m_Database.GetSeries(episode.SeriesIndex).GetImageBanner();
//                     if (filename != String.Empty)
//                         this.pictureBox_Series.Image = Image.FromFile(filename);

                    AddPropertyBindingSource("Season Number", DBEpisode.cSeasonIndex, episode[DBEpisode.cSeasonIndex], false);
                    AddPropertyBindingSource("Episode Number", DBEpisode.cEpisodeIndex, episode[DBEpisode.cEpisodeIndex]);
                    AddPropertyBindingSource("Episode Title", DBEpisode.cEpisodeName, episode[DBEpisode.cEpisodeName]);
                    break;
                #endregion
                //////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////
                #region When Season Nodes is Clicked

                case DBSeason.cTableName:
                    DBSeason season = (DBSeason)node.Tag;
                    String filename = season[DBSeason.cBannerFileName];
                    if (filename != String.Empty)
                        this.pictureBox_Series.Image = Image.FromFile(filename);

                    AddPropertyBindingSource("Season Number", DBSeason.cIndex, "Season " + season[DBSeason.cIndex], false);
                    break;
                #endregion

                //////////////////////////////////////////////////////////////////////////////
                #region When Series Nodes is Clicked

                case DBSeries.cTableName:
                    {
                        DBSeries series = (DBSeries)node.Tag;
                        filename = series[DBSeries.cBannerFileName];

                        if (filename != String.Empty)
                            this.pictureBox_Series.Image = Image.FromFile(filename);

                        String genres = series[DBSeries.cGenre];

                        AddPropertyBindingSource("Series Parsed Name", DBSeries.cParsedName, series[DBSeries.cParsedName], false);
                        AddPropertyBindingSource("Series Pretty Name", DBSeries.cPrettyName, series[DBSeries.cPrettyName]);
                        AddPropertyBindingSource("Genre", DBSeries.cGenre, series[DBSeries.cGenre]);
//                         AddPropertyBindingSource("Network", series[DBSeries.cNet]);
//                         AddPropertyBindingSource("Duration", series.Duration);
//                         AddPropertyBindingSource("Status", series.Status);
//                         AddPropertyBindingSource("Premiered", series.Premiered);
//                         AddPropertyBindingSource("Airs", series.Airs);
//                         AddPropertyBindingSource("Series Description", series.Description);
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


        private void button_Start_Click(object sender, EventArgs e)
        {
            LocalParse();
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            TreeNode node = treeView_Library.SelectedNode;
            DataGridViewCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[1];
            switch (node.Name)
            {
                case DBSeries.cTableName:
                    DBSeries series = (DBSeries)node.Tag;
                    series[(String)cell.Tag] = (String)cell.Value;
                    series.Commit();
                    if (series[DBSeries.cPrettyName] != String.Empty)
                        node.Text = series[DBSeries.cPrettyName];
                    break;

                case DBSeason.cTableName:
                    DBSeason season = (DBSeason)node.Tag;
                    season[(String)cell.Tag] = (String)cell.Value;
                    season.Commit();
                    break;

                case DBEpisode.cTableName:
                    DBEpisode episode = (DBEpisode)node.Tag;
                    episode[(String)cell.Tag] = (String)cell.Value;
                    episode.Commit();
                    if (episode[DBEpisode.cEpisodeName] != String.Empty)
                        node.Text = episode[DBEpisode.cSeasonIndex] + "x" + episode[DBEpisode.cEpisodeIndex] + " - " + episode[DBEpisode.cEpisodeName];
                    break;
            }
        }

        private void button_TestReparse_Click(object sender, EventArgs e)
        {
            StartParsingTest(true);
        }








        ////////////////////////////////////////////////////////////////////////////////////////////
    }

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

