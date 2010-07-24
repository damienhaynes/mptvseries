using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.Globalization;

namespace WindowPlugins.GUITVSeries.Configuration
{    
    public partial class ImportPanelParsing : UserControl
    {        
        public event UserFinishedEditingDelegate UserFinishedEditing;
        public delegate void ParsingGridPopulatedDelegate();
        public event ParsingGridPopulatedDelegate ParsingGridPopulated;

        public ImportPanelParsing()
        {
            InitializeComponent();            
        }

        public List<PathPair> allFoundFiles = new List<PathPair>();

        List<columns> uniqueCols = null;
        List<string> userCols = new List<string>();
        List<string> staticCols = new List<string>();
        List<parseResult> origResults = null;
        void FillGrid(IList<parseResult> parsingResults)
        {
            dataGridViewReview.SuspendLayout();

            this.dataGridViewReview.Rows.Clear();
            this.dataGridViewReview.Columns.Clear();
            // how many cells?            
            // we have filename + status as a given
            // + all unique regex-group-matches, lets find them

            dataGridViewReview.Columns.Add("Enabled", "-");
            dataGridViewReview.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            
            dataGridViewReview.Columns.Add("Filename", "Filename");
            dataGridViewReview.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewReview.Columns[1].ReadOnly = true;

            // this is a bit inefficient in the way it joins the the usercols with the autocols
            // but it doesn't really matter
            uniqueCols = (from result in parsingResults
                          select result.parser.Matches.Keys).SelectMany(
                            k =>
                            {
                                var complete = k.ToList();
                                complete.AddRange(userCols);
                                return complete;
                            }).Distinct().Select( c => new columns(c)).ToList();           


            // now lets add the columns, but lets get a certain order
            foreach (var col in uniqueCols)
            {
                switch (col.Name)
                {
                    case DBSeries.cParsedName:
                        col.Importance = 99;
                        col.Pretty = "Series";
                        break;
                    case DBEpisode.cSeasonIndex:
                        col.Importance = 98;
                        col.Pretty = "Season Index";
                        break;
                    case DBEpisode.cEpisodeIndex:
                        col.Importance = 97;
                        col.Pretty = "Episode";
                        break;
                    case DBEpisode.cEpisodeIndex2:
                        col.Importance = 96;
                        col.Pretty = "Episode2";
                        break;
                    default:
                        break;
                }
            }
            uniqueCols = uniqueCols.OrderByDescending(c => c.Importance).ToList();

            foreach (var group in uniqueCols)
            {
                dataGridViewReview.Columns.Add(group.Name, group.Pretty);
            }

            // note: we order by parsed_name (series), which also sorts those with !success at the top
            foreach (var result in parsingResults.OrderBy(r => r.parser.Matches.SingleOrDefault(kv => kv.Key == DBSeries.cParsedName).Value
                                                             + r.parser.Matches.SingleOrDefault(kv => kv.Key == DBEpisode.cSeasonIndex).Value
                                                             + r.parser.Matches.SingleOrDefault(kv => kv.Key == DBEpisode.cEpisodeIndex).Value))
            {
                DataGridViewRow r = new DataGridViewRow();

                if (result.success)
                    r.DefaultCellStyle.BackColor = Color.LightGreen;
                else
                    r.DefaultCellStyle.BackColor = Color.LemonChiffon;

                // status
                var scell = new DataGridViewCheckBoxCell(false);
                scell.Value = result.success;  
                scell.ContextMenuStrip = contextMenuStripChangeCell;
                r.Cells.Add(scell);

                // filename
                var fcell = new DataGridViewTextBoxCell();
                fcell.Value = result.match_filename;
                fcell.ContextMenuStrip = contextMenuStripChangeCell;
                r.Cells.Add(fcell);                

                foreach (var col in uniqueCols)
                {
                    var vcell = new DataGridViewTextBoxCell();
                    string val = string.Empty;
                    result.parser.Matches.TryGetValue(col.Name, out val);
                    vcell.Value = val;
                    vcell.Tag = val; // this is for reference of the original value
                    r.Cells.Add(vcell);

                    vcell.ContextMenuStrip = contextMenuStripChangeCell;
                }

                r.Tag = result;                
                this.dataGridViewReview.Rows.Add(r);
            }
            this.dataGridViewReview.ResumeLayout();

            // when we support custom fields in importer this can always be true
            if (dataGridViewReview.ColumnCount < 9 && dataGridViewReview.Rows.Count > 0) lnkAdd.Visible = true;

            updateCount();

            pictureBoxWork.Visible = false;
        }

        void updateCount()
        {
            int total = this.dataGridViewReview.Rows.Count;
            int dis = (from DataGridViewRow row in this.dataGridViewReview.Rows
                       where row.Visible
                       select row).Count();
            this.lblCount.Text = string.Format("{0} Files found ({1} displayed)", total, dis);
        }

        public void Init()
        {
            lnkAdd.Visible = false;

            ImportWizard.OnWizardNavigate += new ImportWizard.WizardNavigateDelegate(ImportWizard_OnWizardNavigate);

            DoLocalParsing();
        }

        private void DoLocalParsing()
        {            
            LocalParse runner = new LocalParse();
            runner.LocalParseCompleted += new LocalParse.LocalParseCompletedHandler(//runner_LocalParseCompleted);
                results => 
                {
                    allFoundFiles = results.Select(r => r.PathPair).ToList();
                    OnlineParsing.RemoveFilesInDB(ref results);
                    this.labelWaitParse.Text = "Local File Parsing is done, displaying Results...";
                    MPTVSeriesLog.Write(this.labelWaitParse.Text);
                    origResults = results.ToList<parseResult>();                    
                    FillGrid(results);
                    this.labelWaitParse.Text = "Please make changes to the Results below, and/or add files. Click Next to continue.";
                    // fire off event so user can click Next in wizard
                    if (ParsingGridPopulated != null)
                        ParsingGridPopulated();
                });
            runner.AsyncFullParse();
        }

        private void ImportWizard_OnWizardNavigate(UserFinishedRequestedAction reqAction)
        {
            if (reqAction == UserFinishedRequestedAction.Next)
            {
                var results = IdentifyChanges(false);
                // TODO: make possible to only have series filled out!
                // we require at least the series to be filled for all enabled ones
                var invalids = results.Count(pr => !pr.parser.Matches.ContainsKey(DBSeries.cParsedName) || string.IsNullOrEmpty(pr.parser.Matches[DBSeries.cParsedName]));
                invalids += results.Count(pr => !pr.parser.Matches.ContainsKey(DBEpisode.cEpisodeIndex) || string.IsNullOrEmpty(pr.parser.Matches[DBEpisode.cEpisodeIndex]));
                invalids += results.Count(pr => !pr.parser.Matches.ContainsKey(DBEpisode.cSeasonIndex) || string.IsNullOrEmpty(pr.parser.Matches[DBEpisode.cSeasonIndex]));
                if (invalids == 0)
                {
                    if (UserFinishedEditing != null)
                        UserFinishedEditing(new UserInputResults(results, null), reqAction);
                }
                else
                {
                    MessageBox.Show("All Enabled results need at least the Series/Season/Episode IDs Filled out!", "Unable to continue", MessageBoxButtons.OK);
                    return;
                }
            }
            else if (reqAction == UserFinishedRequestedAction.Cancel)
            {
                if (UserFinishedEditing != null)
                    UserFinishedEditing(null, reqAction);
            }

            // we no longer need to listen to navigate event
            ImportWizard.OnWizardNavigate -= new ImportWizard.WizardNavigateDelegate(ImportWizard_OnWizardNavigate);
        }

        private IList<parseResult> IdentifyChanges(bool includeDisabled)
        {
            List<parseResult> changes = new List<parseResult>();

            Dictionary<string, parseResult> origParseResults = origResults.ToDictionary(pr => pr.full_filename, pr => pr);

            foreach (DataGridViewRow row in dataGridViewReview.Rows)
            {
                if (includeDisabled || (bool)row.Cells[0].Value == true)
                {
                    // check if every editable col still equals to the original parseresult
                    for (int i = 0; i < uniqueCols.Count; i++)
                    {
                        if (row.Tag is parseResult)
                        {
                            //var origPR = origResults.SingleOrDefault(pr => pr.full_filename == (row.Tag as parseResult).full_filename);
                            
                            parseResult origPR;
                            if (origParseResults.TryGetValue((row.Tag as parseResult).full_filename, out origPR))
                            //if (origPR != null)
                            {
                                origPR.success = (bool)row.Cells[0].Value == true;
                                var colname = uniqueCols[i].Name;
                                string origValue;
                                string newValue = row.Cells[colname].Value as string;

                                if ((!origPR.parser.Matches.TryGetValue(colname, out origValue) && !string.IsNullOrEmpty(newValue)) // manually added
                                    || (newValue != origValue)) // or changed
                                {
                                    MPTVSeriesLog.Write(string.Format("User Change Detected for: \"{0}\" Column: \"{1}\" - From: \"{2}\" to \"{3}\"",
                                        origPR.match_filename, colname, origValue, newValue));
                                    // this one was changed
                                    // update the parsingResult
                                    origPR.parser.Matches[colname] = newValue;
                                }
                            }
                        }
                    }
                    if (row.Tag is parseResult)
                        changes.Add(row.Tag as parseResult);
                }
            }
            return changes;
        }

        private void lnkAdd_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.groupBoxAddCol.Visible = true;
            this.comboBoxAddColumn.Focus();
        }

        private void buttonAddColOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.comboBoxAddColumn.Text))
                MessageBox.Show("Please enter the name of the column");
            else if (this.uniqueCols.Contains(this.comboBoxAddColumn.Text) || this.userCols.Contains(this.comboBoxAddColumn.Text))
                MessageBox.Show("This Column already exists");
            else
            {
                string colName = string.Empty;
                switch (this.comboBoxAddColumn.Text)
                {
                    case "Series Name":
                        colName = DBSeries.cParsedName;
                        break;
                    case "Season Index":
                        colName = DBSeason.cIndex;
                        break;
                    case "Episode Index":
                        colName = DBEpisode.cEpisodeIndex;
                        break;
                    case "Episode Index 2":
                        colName = DBEpisode.cEpisodeIndex2;
                        break;
                    case "File Extension":
                        colName = DBEpisode.cExtension;
                        break;
                    case "Episode Name":
                        colName = DBEpisode.cEpisodeName;
                        break;
                    default:
                        colName = this.comboBoxAddColumn.Text;
                        break;
                }
                this.userCols.Add(colName);
                this.groupBoxAddCol.Visible = false;
                RefreshGrid();
            }
        }

        private void buttonAddColCancel_Click(object sender, EventArgs e)
        {
            this.groupBoxAddCol.Visible = false;
        }

        parseResult merge(parseResult orig, DataGridViewRow changes)
        {
            foreach (DataGridViewCell cell in changes.Cells)
            {
                var colname = dataGridViewReview.Columns[cell.ColumnIndex].Name;
                if (colname == "Status")
                    orig.success = (bool)cell.Value;
                if (colname != "Filename" &&
                    colname != "Status")
                {
                    if (orig.parser.Matches.ContainsKey(colname))
                        orig.parser.Matches[colname] = cell.Value as string;
                    else if(!string.IsNullOrEmpty(cell.Value as string)) 
                        orig.parser.Matches.Add(colname, cell.Value as string);
                }
            }

            return orig;
        }

        void RefreshGrid()
        {
            var updatedPRs = from DataGridViewRow r in dataGridViewReview.Rows
                             where r.Tag is parseResult
                             select merge(r.Tag as parseResult, r);
            FillGrid(updatedPRs.ToList());
        }

        string filterDefault = "Filter by..";
        //bool clearedByClick = false;
        private void textBoxFilter_TextChanged(object sender, EventArgs e)
        {
            Filter();
            /*
            if (textBoxFilter.Text == string.Empty)
            {
                if (!clearedByClick)
                {
                    resetFilter();
                }
                else clearedByClick = false;
            }
            else if (textBoxFilter.Text != filterDefault)
                Filter(this.textBoxFilter.Text);
            */

        }

        private void textBoxFilter_Click(object sender, EventArgs e)
        {
            if (textBoxFilter.Text == filterDefault)
            {
                //clearedByClick = true;
                textBoxFilter.Text = string.Empty;
            }
        }

        void resetFilter()
        {
            dataGridViewReview.SuspendLayout();
            foreach (DataGridViewRow row in dataGridViewReview.Rows)
                row.Visible = true;
            dataGridViewReview.ResumeLayout();
        }

        void Filter(string needle)
        {
            needle = needle.ToLower();
            Filter(row =>
                {
                    bool matches = false;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value != null && cell.Value.ToString().ToLower().Contains(needle))
                        {
                            matches = true;
                            break;
                        }
                    }
                    return matches;
                });
        }

        bool filteringResultForRow(DataGridViewRow row)
        {
            bool matches = false;
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (textBoxFilter.Text != filterDefault && textBoxFilter.Text != string.Empty)
                {
                    if (cell.Value != null && cell.Value.ToString().ToLower().Contains(textBoxFilter.Text.ToLower()))
                    {
                        matches = true;
                        break;
                    }
                }
                else
                    matches = true;
            }

            bool matchesManual = false;
            if (checkFilterMan.Checked)
            {
                parseResult pr = row.Tag as parseResult;
                matchesManual = pr != null && pr.match_filename == pr.full_filename;
            }
            else
                matchesManual = true;

            return matches && matchesManual;
        }

        void Filter()
        {
            dataGridViewReview.SuspendLayout();

            foreach (DataGridViewRow row in dataGridViewReview.Rows)
            {
                row.Visible = filteringResultForRow(row);
            }
            
            dataGridViewReview.ResumeLayout();
            updateCount();

        }

        void Filter(Func<DataGridViewRow, bool> filter)
        {
            dataGridViewReview.SuspendLayout();
            
            foreach (DataGridViewRow row in dataGridViewReview.Rows)
            {
                row.Visible = filter(row);
            }
            dataGridViewReview.ResumeLayout();
            updateCount();
        }

        void autoChangeAll(int column, string orig, string newval)
        {
            dataGridViewReview.SuspendLayout();

            foreach (DataGridViewRow row in dataGridViewReview.Rows)
            {
                if (row.Visible) // only filtered (currently displayed)
                {
                    if (row.Cells[column].Value as string == orig)
                    {
                        MPTVSeriesLog.Write(row.Cells[column].Value + "->" + newval);
                        row.Cells[column].Value = newval;
                    }
                }
            }

            RefreshGrid();
            dataGridViewReview.ResumeLayout();
        }

        private void contextMenuStripChangeCell_Opening(object sender, CancelEventArgs e)
        {

            DataGridViewCell cell = null;
            if(dataGridViewReview.SelectedCells.Count == 1)
                cell = dataGridViewReview.SelectedCells[0];
            if (cell == null) return;

            contextMenuStripChangeCell.Items.Clear();
            parseResult origpr = dataGridViewReview.Rows[cell.RowIndex].Tag as parseResult;
            contextMenuStripChangeCell.Items.Add("File: " + origpr.full_filename);
            contextMenuStripChangeCell.Items.Add(string.Format("Matched using expression [{0}]:", origpr.parser.RegexpMatchedIndex));
            contextMenuStripChangeCell.Items.Add(origpr.parser.RegexpMatched);

            // for enabled column simply offer to change to all
            if (cell is DataGridViewCheckBoxCell && cell.ColumnIndex == 0)
            {
                bool isChecked = (bool)cell.Value;
                contextMenuStripChangeCell.Items.Add(string.Format("{0} all currently displayed", isChecked ? "Disable" : "Enable"));
                contextMenuStripChangeCell.Items[2].Tag = new object[] { cell, isChecked, !isChecked };
                return;
            }

            var orig = cell.Tag as string;        
            if (orig == null) return;
            
            
            //// lets see if the cell was changed
            string cellValue = cell.Value as string;
            // if no change dont offer to do anything
            
            if(string.IsNullOrEmpty(orig) && string.IsNullOrEmpty(cellValue)) return;
            if(orig == cellValue) return;
            // else offer to do this change automatically for them all
            
            contextMenuStripChangeCell.Items.Add(string.Format("Change all \"{0}\" to \"{1}\" in Column \"{2}\"", orig, cellValue, dataGridViewReview.Columns[cell.ColumnIndex].HeaderText));
            contextMenuStripChangeCell.Items[2].Tag = new object[] { cell, orig, cellValue };
        }

        private void contextMenuStripChangeCell_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            object[] args = e.ClickedItem.Tag as object[];
            if (args != null && args.Length == 3 && args[0] is DataGridViewCell)
            {    
                if (args[1] is string && args[2] is string)
                    autoChangeAll((args[0] as DataGridViewCell).ColumnIndex, args[1] as string, args[2] as string);
                else if (args[1] is bool && args[2] is bool) // for enabled, yikes, copy/paste :-(
                {
                    dataGridViewReview.SuspendLayout();

                    foreach (DataGridViewRow row in dataGridViewReview.Rows)
                    {
                        if (row.Visible) // only filtered (currently displayed)
                        {
                            row.Cells[0].Value = args[2];
                        }
                    }

                    RefreshGrid();
                    dataGridViewReview.ResumeLayout();
                }
            }
        }

        void addFiles(IEnumerable<string> path)
        {
            // parse them
            var parseResult = LocalParse.Parse(path.Select(file => new PathPair(file, file)).ToList());

            // add them to the origlist
            if (origResults == null) origResults = new List<parseResult>();
            foreach (parseResult pr in parseResult)
            {
                if (origResults.Find(match => match.full_filename == pr.full_filename) == null)
                    origResults.Add(pr);
            }

            // now merge them with the changes
            List<parseResult> allResults = (List<parseResult>)IdentifyChanges(true);
            foreach (parseResult pr in parseResult)
            {
                if (allResults.Find(match => match.full_filename == pr.full_filename) == null)
                    allResults.Add(pr);
            }

            // and refresh the grid
            FillGrid(allResults);
            
        }
        string prevPath = string.Empty;
        private void lnkAddFiles_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.InitialDirectory = prevPath;
            openFileDialog1.Multiselect = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileNames != null && openFileDialog1.FileNames.Length > 0)
            {
                addFiles(openFileDialog1.FileNames);
            }
        }

        private void checkFilterMan_CheckedChanged(object sender, EventArgs e)
        {
            Filter();
            /*
            if (checkFilterMan.Checked)
            {
                // we identify manually added as having the same fullfilename as matchfilename
                // should probably be identified differently
                Filter(row =>
                    {
                        parseResult pr = row.Tag as parseResult;
                        return pr != null && pr.match_filename == pr.full_filename;
                    });
            }
            else RefreshGrid();
            */
        }

        void dataGridViewReview_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewReview.CurrentCell.ColumnIndex == 0 && (bool)dataGridViewReview.CurrentCell.Value)
                dataGridViewReview.CurrentRow.DefaultCellStyle.BackColor = Color.LightGreen;
            else
                dataGridViewReview.CurrentRow.DefaultCellStyle.BackColor = Color.LemonChiffon;
    }

        void dataGridViewReview_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridViewReview.CurrentCell.ColumnIndex == 0 && dataGridViewReview.IsCurrentCellDirty)
                dataGridViewReview.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

    }

    internal class columns
    {
        public int Importance;
        public string Name;
        string pretty;

        public string Pretty
        {
            get
            { 
                return string.IsNullOrEmpty(pretty) ? Name : pretty; 
            }
            set 
            { 
                pretty = value; 
            }
        }

        public columns(string name)
        {
            this.Name = name;
        }

        public static implicit operator columns(string colname)
        {
            return new columns(colname);
        }
    }
}
