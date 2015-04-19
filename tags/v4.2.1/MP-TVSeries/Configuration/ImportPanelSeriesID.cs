using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using WindowPlugins.GUITVSeries.Properties;

namespace WindowPlugins.GUITVSeries.Configuration
{    
    public partial class ImportPanelSeriesID : UserControl
    {
        public event UserFinishedEditingDelegate UserFinishedEditing;

        public delegate void SeriesGridPopulatedDelegate();
        public event SeriesGridPopulatedDelegate SeriesGridPopulated;

        const string cSearching = "Searching...";
        const string cWait2Search = "Waiting to Search...";

        const string colImage     = "Status";
        const string colSeries    = "Series";
        const string colOSeries   = "OSeries";
        //const string colOSeriesOrder = "Order"; // only get this during alter stages, or we get the full series record here already when user makes selection?
        //const string colOSeriesD  = "DSeries";
        const string colSearchTXT = "SearchTXT";
        const string colSearchBTN = "SearchBTN";
        const string colAction    = "Action";

        const string searchTip = "<Custom Search>";


        Color Approved = Color.LightGreen;
        Color SkipColor = Color.LightYellow;
        Color IgnoreColor = Color.LightGray;

        IList<parseResult> givenResults = null;

        Dictionary<UserInputResults.SeriesAction, string> displayedActions = new Dictionary<UserInputResults.SeriesAction, string>();

        int queuedSearches = 0;
        int activeSearches = 0;

        delegate void SearchProgressDelegate(string searchString, GetSeries searchResult, DataGridViewRow row);
        public ImportPanelSeriesID()
        {
            InitializeComponent();            
        }

        Dictionary<int, string> lastSearch = new Dictionary<int, string>();

        public void Init(IList<parseResult> results)
        {
            if (results.Count == 0)
            {
                // nothing to do, skip this step
                UserFinishedEditing(new UserInputResults(givenResults, getApprovedResults()), UserFinishedRequestedAction.Next);
            }

            ImportWizard.OnWizardNavigate += new ImportWizard.WizardNavigateDelegate(ImportWizard_OnWizardNavigate);
            SetResults(results);
        }

        private void SetResults(IList<parseResult> results)
        {
            givenResults = results;

            
            // get the parsed series as grouped by nicely Title Cased strings
            var uniqueSeries = from result in results
                                where !string.IsNullOrEmpty(result.parser.Matches[DBSeries.cParsedName])
                                group result by (string)result.parser.Matches[DBSeries.cParsedName];
            
            // now filter by those we don't already have identified in the db
            // get them from the db
            var alreadyID = DBSeries.GetSingleField(DBSeries.cParsedName, new SQLCondition(new DBSeries(), DBSeries.cID, 0, SQLConditionType.GreaterThan), new DBSeries());

            // and filter
            uniqueSeries = from uniqueS in uniqueSeries
                           where !alreadyID.Contains(alreadyID.Contains(uniqueS.Key))
                           select uniqueS;

            // now we have all new series that need identifying
            // fill the grid
            FillGrid(uniqueSeries.ToList());           
        }

        public void ClearResults()
        {
            givenResults = null;
            displayedActions = new Dictionary<UserInputResults.SeriesAction, string>();
            dataGridViewIdentifySeries.Rows.Clear();
            dataGridViewIdentifySeries.Columns.Clear();
            isGridPrepared = false;
        }

        private int ColIndexOf(string columnName)
        {
            return dataGridViewIdentifySeries.Columns[columnName].Index;
        }

        bool isGridPrepared = false;
        private void prepareGrid()
        {
            // add the columns, fixed here
            var dgvcI = new DataGridViewImageColumn();
            dgvcI.Name = colImage;
            dgvcI.HeaderText = string.Empty;
            dgvcI.ReadOnly = true;
            dgvcI.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;

            var dgvcS = new DataGridViewTextBoxColumn();
            dgvcS.Name = colSeries;
            dgvcS.ReadOnly = true;
            dgvcS.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;

            var dgvcO = new DataGridViewComboBoxColumn();
            dgvcO.Name = colOSeries;
            dgvcO.HeaderText = "Matched Online Series";
            dgvcO.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            dgvcO.Width = 240;

            //var dgvcOrder = new DataGridViewComboBoxColumn();
            //dgvcOrder.Name = colOSeriesOrder;
            //dgvcOrder.HeaderText = "Ordering";
            //dgvcOrder.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            //dgvcOrder.Width = 100;

            //var dgvcD = new DataGridViewButtonColumn();
            //dgvcD.Name = colOSeriesD;
            //dgvcD.HeaderText = string.Empty;
            //dgvcD.Width = 16;

            var dgvcSearch = new DataGridViewTextBoxColumn();
            dgvcSearch.Name = colSearchTXT;
            dgvcSearch.HeaderText = "Custom Search Name";
            dgvcSearch.Width = 180;

            var dgvcSearchOK = new DataGridViewButtonColumn();
            dgvcSearchOK.Name = colSearchBTN;
            dgvcSearchOK.HeaderText = string.Empty;
            dgvcSearchOK.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            var dgvcApprove = new DataGridViewComboBoxColumn();
            dgvcApprove.Name = colAction;
            dgvcApprove.Width = 85;

            dataGridViewIdentifySeries.Columns.Add(dgvcI);
            dataGridViewIdentifySeries.Columns.Add(dgvcS);
            dataGridViewIdentifySeries.Columns.Add(dgvcO);
            //dataGridView1.Columns.Add(dgvcOrder);
            //dataGridView1.Columns.Add(dgvcD);
            dataGridViewIdentifySeries.Columns.Add(dgvcSearch);
            dataGridViewIdentifySeries.Columns.Add(dgvcSearchOK);
            dataGridViewIdentifySeries.Columns.Add(dgvcApprove);

            dataGridViewIdentifySeries.EditMode = DataGridViewEditMode.EditOnEnter;

            #region Grid Events
            dataGridViewIdentifySeries.CellBeginEdit += new DataGridViewCellCancelEventHandler((sender, e) =>
            {
                var row = dataGridViewIdentifySeries.Rows[e.RowIndex];
                // did we enter the searchField
                if (e.ColumnIndex == ColIndexOf(colSearchTXT))
                {
                    var cell = row.Cells[e.ColumnIndex];
                    if ((string)cell.Value == searchTip)
                    {
                        // make it easier to do custom search, 
                        // new search name will most likely be like parsed name
                        cell.Value = row.Cells[colSeries].Value;                       
                    }
                }
            });

            dataGridViewIdentifySeries.CellValueChanged += new DataGridViewCellEventHandler((sender, e) =>
            {
                // when we exit back from a dirty state, the event gets triggered
                if (displayedActions.Count == 0) return;

                var row = dataGridViewIdentifySeries.Rows[e.RowIndex];
                var cell = row.Cells[e.ColumnIndex];
                
                // has online search result changed?
                if (e.ColumnIndex == ColIndexOf(colOSeries))
                {
                    var actionCell = row.Cells[colAction] as DataGridViewComboBoxCell;
                    if (cell.Tag is List<DBOnlineSeries>)
                    {
                        // we have valid results in combobox, and one selected
                        // we dont do anything else, just approve                    
                        actionCell.Value = displayedActions[UserInputResults.SeriesAction.Approve];
                    }
                    else
                    {
                        // set to skip
                        actionCell.Value = displayedActions[UserInputResults.SeriesAction.Skip];
                    }

                    // get the selected series to display the plot, ordering options
                    var series = getSeriesFromSelected(row);
                    //var orderCell = row.Cells[ColIndexOf(colOSeriesOrder)] as DataGridViewComboBoxCell;
                    //orderCell.Items.Clear();                    
                    if (series != null)
                    {
                        cell.ToolTipText = AdjustSummaryTooltip(series[DBOnlineSeries.cSummary].ToString());
                        //displayValsInCBCell(orderCell, series[DBOnlineSeries.cEpisodeOrders].ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));                        
                    }
                    else cell.ToolTipText = string.Empty;
                }
                else if (e.ColumnIndex == ColIndexOf(colAction))
                {
                    // seriesAction
                    // we color code and set icon        

                    var reqAction = row.Cells[ColIndexOf(colAction)].Value.ToString();
                    if (reqAction == displayedActions[UserInputResults.SeriesAction.Skip])
                    {
                        row.Cells[colImage].Value = Resources.importpending;
                        row.DefaultCellStyle.BackColor = SkipColor;
                    }
                    else if (reqAction == displayedActions[UserInputResults.SeriesAction.IgnoreAlways])
                    {
                        row.Cells[colImage].Value = Resources.importignored;
                        row.DefaultCellStyle.BackColor = IgnoreColor;
                    }
                    else if (reqAction == displayedActions[UserInputResults.SeriesAction.Approve])
                    {
                        row.Cells[colImage].Value = Resources.importaccept;
                        row.DefaultCellStyle.BackColor = Approved;
                    }
                                      
                }
                //else if (e.ColumnIndex == ColIndexOf(colSearchTXT))
                //{
                //    if ((string)cell.Value == string.Empty && cell.Tag == null)
                //        cell.Value = searchTip;
                //    cell.Tag = null;
                //}
            });

            dataGridViewIdentifySeries.CellContentClick += new DataGridViewCellEventHandler((sender, e) =>
            {
                if (e.RowIndex < 0) return;

                // which row was clicked?
                var row = dataGridViewIdentifySeries.Rows[e.RowIndex];
                
                // was search clicked?
                if (e.ColumnIndex == ColIndexOf(colSearchBTN))
                {                    
                    // fire off a new search, if user typed something
                    string customSearch = row.Cells[ColIndexOf(colSearchTXT)].Value as string;
                    if (!string.IsNullOrEmpty(customSearch) && !customSearch.Equals(searchTip))
                        FireOffSearch(row.Tag as IGrouping<string, parseResult>, row, customSearch);
                }

            });
            #endregion

            displayedActions.Add(UserInputResults.SeriesAction.Approve, "Approve");
            displayedActions.Add(UserInputResults.SeriesAction.Skip, "Skip");
            displayedActions.Add(UserInputResults.SeriesAction.IgnoreAlways, "Always Ignore");

            isGridPrepared = true;
        }

        private void FillGrid(List<IGrouping<string, parseResult>> uniqueNewSeries)
        {
            dataGridViewIdentifySeries.SuspendLayout();
            
            if (!isGridPrepared) prepareGrid();
            
            foreach (var newSeries in uniqueNewSeries)
            {
                var row = new DataGridViewRow();

                var imageCell = new DataGridViewImageCell();
                imageCell.Value = Resources.importupdating;
                row.Cells.Add(imageCell);

                var seriesCell = new DataGridViewTextBoxCell();
                seriesCell.Value = newSeries.Key;
                row.Cells.Add(seriesCell);

                var onlineSeriesCell = new DataGridViewComboBoxCell();
                onlineSeriesCell.Items.Add(cWait2Search);
                onlineSeriesCell.Value = cWait2Search;     
                row.Cells.Add(onlineSeriesCell);

                //var onlineSeriesOrderCell = new DataGridViewComboBoxCell();
                //row.Cells.Add(onlineSeriesOrderCell);

                //var DCell = new DataGridViewButtonCell();
                //DCell.Value = "?";
                //row.Cells.Add(DCell);

                var searchCell = new DataGridViewTextBoxCell();
                searchCell.Value = searchTip;
                searchCell.ToolTipText = "Type a new string to search again. Click search to find results.";
                row.Cells.Add(searchCell);

                var searchOKCell = new DataGridViewButtonCell();
                searchOKCell.Value = "Search";
                row.Cells.Add(searchOKCell);

                var approveCell = new DataGridViewComboBoxCell();
                approveCell.Items.Add(displayedActions[UserInputResults.SeriesAction.Approve]);
                approveCell.Items.Add(displayedActions[UserInputResults.SeriesAction.Skip]);
                approveCell.Items.Add(displayedActions[UserInputResults.SeriesAction.IgnoreAlways]);
                approveCell.Value = displayedActions[UserInputResults.SeriesAction.Skip];
                row.Cells.Add(approveCell);

                row.Tag = newSeries;
                dataGridViewIdentifySeries.Rows.Add(row);

                FireOffSearch(newSeries, row, null);
            }

            dataGridViewIdentifySeries.ResumeLayout();          
        }

        private void FireOffSearch(IGrouping<string, parseResult> newSeries, DataGridViewRow row, string customString)
        {
            string toSearch = string.IsNullOrEmpty(customString) ? newSeries.Key : customString;
            if(lastSearch.ContainsKey(row.Index))
                lastSearch[row.Index] = toSearch;
            else lastSearch.Add(row.Index, toSearch);
                        
            System.Threading.Interlocked.Increment(ref queuedSearches);
            setSearchStatus();

            int iThreadCount = Environment.ProcessorCount;
            if (iThreadCount < 8) iThreadCount = 8;
            System.Threading.ThreadPool.SetMaxThreads(iThreadCount, iThreadCount);            

            System.Threading.ThreadPool.QueueUserWorkItem((o) =>
                {
                    // tell user we are now searching
                    SearchProgress(cSearching, null, row);
                    GetSeries gs = new GetSeries(toSearch);
                    // and give the results
                    SearchProgress(toSearch, gs, row);
                });
            
        }

        private void SearchProgress(string searchString, GetSeries searchResult, DataGridViewRow row)
        {            
            // we need to invoke
            if (this.dataGridViewIdentifySeries.InvokeRequired)
            {                
                this.dataGridViewIdentifySeries.Invoke(new SearchProgressDelegate(SearchProgress), searchString, searchResult, row);
                return;
            }
            
            // lets update the combobox
            var comboCell = new DataGridViewComboBoxCell();
            row.Cells[ColIndexOf(colOSeries)] = comboCell;

            // ok, if we only get a string and the row, display that string
            if (searchResult == null)
            {
                displayValsInCBCell(comboCell, searchString);

                // this also means that a queued search went into active status
                System.Threading.Interlocked.Decrement(ref queuedSearches);
                System.Threading.Interlocked.Increment(ref activeSearches);
                setSearchStatus();
                return;
            }

            // else we got the results                        
            comboCell.Tag = searchResult.Results;

            // which also means an active search has finished
            System.Threading.Interlocked.Decrement(ref activeSearches);
            setSearchStatus();

            displayValsInCBCell(comboCell, searchResult.Results.Select(r => getDisplayStringForSeries(r)).ToArray());

            if (comboCell.Items.Count < 1)
                displayValsInCBCell(comboCell, "No Results found");

            var actionCell = row.Cells[ColIndexOf(colAction)] as DataGridViewComboBoxCell;

            // overwrite from the cellchanged event which set it to approved
            if (searchResult.PerfectMatch == null)
                actionCell.Value = displayedActions[UserInputResults.SeriesAction.Skip];

        }

        private void displayValsInCBCell(DataGridViewComboBoxCell cell, params string[] values)
        {
            cell.Items.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                cell.Items.Add(values[i]);
                if (i == 0)
                    cell.Value = values[i];
            }
        }

        private string getDisplayStringForSeries(DBOnlineSeries series)
        {
            return string.Format("{0} ({1})", series[DBOnlineSeries.cPrettyName], series[DBOnlineSeries.cID]);
        }
        
        private Dictionary<string, UserInputResultSeriesActionPair> getApprovedResults()
        {
            var pageResult = new Dictionary<string, UserInputResultSeriesActionPair>();
            foreach (DataGridViewRow row in dataGridViewIdentifySeries.Rows)
            {
                string inputSeriesName = row.Cells[ColIndexOf(colSeries)].Value as string;
                var requestedAction = getActionFromRow(row);
                DBOnlineSeries chosenSeries = null;

                // else we dont even care
                if (requestedAction == UserInputResults.SeriesAction.Approve)
                {
                    chosenSeries = getSeriesFromSelected(row);
                }

                MPTVSeriesLog.Write(string.Format("Series \"{0}\" {1} {2}", inputSeriesName, requestedAction.ToString(), chosenSeries == null ? string.Empty : this.getDisplayStringForSeries(chosenSeries))); 

                pageResult.Add(inputSeriesName, new UserInputResultSeriesActionPair(requestedAction, chosenSeries));         
            }

            return pageResult;
        }

        private DBOnlineSeries getSeriesFromSelected(DataGridViewRow row)
        {
            DBOnlineSeries chosenSeries = null;
            var cell = row.Cells[ColIndexOf(colOSeries)] as DataGridViewComboBoxCell;
            var seriesList = cell.Tag as List<DBOnlineSeries>;
            if (seriesList != null)
            {
                string toMatch = cell.Value as string;
                chosenSeries = seriesList.FirstOrDefault(s => toMatch == getDisplayStringForSeries(s));
            }
            return chosenSeries;
        }

        private void setSearchStatus()
        {
            this.labelSearchStats.Text = string.Format("Searching in progress for {0} series ({1} queued)", activeSearches, queuedSearches);

            // send notification that grid is filled
            if (activeSearches == 0 && queuedSearches == 0)
            {
                if (SeriesGridPopulated != null)
                    SeriesGridPopulated();
            }
        }

        private void ImportWizard_OnWizardNavigate(UserFinishedRequestedAction reqAction)
        {
            if (UserFinishedEditing == null) return;

            if (reqAction == UserFinishedRequestedAction.Next)
            {
                // check if cells are in a dirty status
                // TODO: if so, force focus on 1st cell of active row to register changes
                if (dataGridViewIdentifySeries.IsCurrentRowDirty)
                {
                    string currentCol = dataGridViewIdentifySeries.CurrentCell.OwningColumn.Name;

                    // only interested in the dropdowncomboboxes
                    if (currentCol == colOSeries || currentCol == colAction)
                    {
                        string message = "The series '{0}' is still in edit mode. Finish changes, then click Next again.";
                        DialogResult result = MessageBox.Show(string.Format(message, dataGridViewIdentifySeries.CurrentRow.Cells[colSeries].Value), "Unfinished Changes", MessageBoxButtons.RetryCancel, MessageBoxIcon.Information);
                        if (result == DialogResult.Retry) return;
                    }
                    
                }
                UserFinishedEditing(new UserInputResults(givenResults, getApprovedResults()), reqAction);
            }
            else
                UserFinishedEditing(null, reqAction);

            // we no longer need to listen to navigate event
            ImportWizard.OnWizardNavigate -= new ImportWizard.WizardNavigateDelegate(ImportWizard_OnWizardNavigate);
        }

        private string AdjustSummaryTooltip(string summary)
        {
            string newSummary = string.Empty;
            string[] words = summary.Split(' ');

            // Build a string for the tooltip that doesnt span whole width of screen
            int wordCount = 0;
            foreach (string word in words)
            {
                wordCount++;
                newSummary += word + " ";
                if (wordCount > 10)
                {
                    // insert a newline
                    newSummary += "\r\n";
                    wordCount = 0;
                }
            }
            return newSummary;
        }

        private void linkLabelCloseDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.groupBoxDetails.Visible = false;
        }

        private void checkBoxFilter_CheckedChanged(object sender, EventArgs e)
        {
            this.dataGridViewIdentifySeries.SuspendLayout();
            foreach (DataGridViewRow row in dataGridViewIdentifySeries.Rows)
            {
                if (checkBoxFilter.Checked && getActionFromRow(row) == UserInputResults.SeriesAction.Approve)
                {
                    row.Visible = false;
                }
                else row.Visible = true;
            }
            this.dataGridViewIdentifySeries.ResumeLayout();
        }

        private UserInputResults.SeriesAction getActionFromRow(DataGridViewRow row)
        {
            string val = ((string)row.Cells[ColIndexOf(colAction)].Value);
            if (val == displayedActions[UserInputResults.SeriesAction.Approve])
                return UserInputResults.SeriesAction.Approve;
            if (val == displayedActions[UserInputResults.SeriesAction.Skip])
                return UserInputResults.SeriesAction.Skip;
            return UserInputResults.SeriesAction.IgnoreAlways;
        }
    }
}
