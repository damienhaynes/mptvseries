using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace WindowPlugins.GUITVSeries.Configuration
{    
    public partial class ImportPanelSeriesID : UserControl
    {
        public event userFinishedEditingDel UserFinishedEditing;

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
        Color SkipColor = Color.Yellow;
        Color IgnoreColor = Color.Gray;

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

        public void SetResults(IList<parseResult> results)
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

        int ColIndexOf(string columnName)
        {
            return dataGridView1.Columns[columnName].Index;
        }

        bool isGridPrepared = false;
        void prepareGrid()
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
            dgvcSearch.HeaderText = "Search different Name";
            dgvcSearch.Width = 180;

            var dgvcSearchOK = new DataGridViewButtonColumn();
            dgvcSearchOK.Name = colSearchBTN;
            dgvcSearchOK.HeaderText = string.Empty;
            dgvcSearchOK.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            var dgvcApprove = new DataGridViewComboBoxColumn();
            dgvcApprove.Name = colAction;
            dgvcApprove.Width = 85;

            dataGridView1.Columns.Add(dgvcI);
            dataGridView1.Columns.Add(dgvcS);
            dataGridView1.Columns.Add(dgvcO);
            //dataGridView1.Columns.Add(dgvcOrder);
            //dataGridView1.Columns.Add(dgvcD);
            dataGridView1.Columns.Add(dgvcSearch);
            dataGridView1.Columns.Add(dgvcSearchOK);
            dataGridView1.Columns.Add(dgvcApprove);

            dataGridView1.CellBeginEdit += new DataGridViewCellCancelEventHandler((sender, e) =>
            {
                var row = dataGridView1.Rows[e.RowIndex];
                // did we enter the searchField
                if (e.ColumnIndex == ColIndexOf(colSearchTXT))
                {
                    var cell = row.Cells[e.ColumnIndex];
                    if ((string)cell.Value == searchTip)
                    {
                        cell.Value = string.Empty;
                        //cell.Tag = new object(); // prevent auto change back
                    }
                }
            });

            dataGridView1.CellValueChanged += new DataGridViewCellEventHandler((sender, e) =>
            {
                var row = dataGridView1.Rows[e.RowIndex];
                var cell = row.Cells[e.ColumnIndex];
                // was onlinesearchresult changed?

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
                        cell.ToolTipText = series[DBOnlineSeries.cSummary];
                        //displayValsInCBCell(orderCell, series[DBOnlineSeries.cEpisodeOrders].ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));                        
                    }
                    else cell.ToolTipText = string.Empty;
                }
                else if (e.ColumnIndex == ColIndexOf(colAction))
                {
                    // seriesAction
                    // we color code

                    var reqAction = row.Cells[ColIndexOf(colAction)].Value.ToString();
                    if (reqAction == displayedActions[UserInputResults.SeriesAction.Skip])
                            row.DefaultCellStyle.BackColor = SkipColor;
                    else if (reqAction == displayedActions[UserInputResults.SeriesAction.IgnoreAlways])
                            row.DefaultCellStyle.BackColor = IgnoreColor;
                    else if (reqAction == displayedActions[UserInputResults.SeriesAction.Approve])
                            row.DefaultCellStyle.BackColor = Approved;
                                      
                }
                //else if (e.ColumnIndex == ColIndexOf(colSearchTXT))
                //{
                //    if ((string)cell.Value == string.Empty && cell.Tag == null)
                //        cell.Value = searchTip;
                //    cell.Tag = null;
                //}
            });

            dataGridView1.CellContentClick += new DataGridViewCellEventHandler((sender, e) =>
            {
                // which row was clicked?
                var row = dataGridView1.Rows[e.RowIndex];
                
                // was search clicked?
                if (e.ColumnIndex == ColIndexOf(colSearchBTN))
                {                    
                    // fire off a new search, if user typed something
                    string customSearch = row.Cells[ColIndexOf(colSearchTXT)].Value as string;
                    if (!string.IsNullOrEmpty(customSearch))
                        FireOffSearch(row.Tag as IGrouping<string, parseResult>, row, customSearch);
                }

            });

            displayedActions.Add(UserInputResults.SeriesAction.Approve, "Approved");
            displayedActions.Add(UserInputResults.SeriesAction.Skip, "Skip");
            displayedActions.Add(UserInputResults.SeriesAction.IgnoreAlways, "Always Ignore");

            isGridPrepared = true;
        }

        void FillGrid(List<IGrouping<string, parseResult>> uniqueNewSeries)
        {
            dataGridView1.SuspendLayout();
            
            if (!isGridPrepared) prepareGrid();
            
            foreach (var newSeries in uniqueNewSeries)
            {
                var row = new DataGridViewRow();

                var imageCell = new DataGridViewImageCell();
                //imageCell.Value = ;
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
                dataGridView1.Rows.Add(row);

                FireOffSearch(newSeries, row, null);
            }

            dataGridView1.ResumeLayout();
        }

        void FireOffSearch(IGrouping<string, parseResult> newSeries, DataGridViewRow row, string customString)
        {
            string toSearch = string.IsNullOrEmpty(customString) ? newSeries.Key : customString;
            if(lastSearch.ContainsKey(row.Index))
                lastSearch[row.Index] = toSearch;
            else lastSearch.Add(row.Index, toSearch);
            
            System.Threading.Interlocked.Increment(ref queuedSearches);
            setSearchStatus();

            System.Threading.ThreadPool.QueueUserWorkItem((o) =>
                {
                    // tell user we are now searching
                    SearchProgress(cSearching, null, row);
                    GetSeries gs = new GetSeries(toSearch);
                    // and give the results
                    SearchProgress(toSearch, gs, row);
                });
            
        }

        void SearchProgress(string searchString, GetSeries searchResult, DataGridViewRow row)
        {            
            // we need to invoke
            if (this.dataGridView1.InvokeRequired)
            {                
                this.dataGridView1.Invoke(new SearchProgressDelegate(SearchProgress), searchString, searchResult, row);
                return;
            }

            // lets update the combobox
            var cc = new DataGridViewComboBoxCell();
            row.Cells[ColIndexOf(colOSeries)] = cc;

            // ok, if we only get a string and the row, display that string
            if (searchResult == null)
            {
                displayValsInCBCell(cc, searchString);

                // this also means that an queued search went into active status
                System.Threading.Interlocked.Decrement(ref queuedSearches);
                System.Threading.Interlocked.Increment(ref activeSearches);
                setSearchStatus();
                return;
            }

            // else we got the results                        
            cc.Tag = searchResult.Results;

            // which also means an active search has finished
            System.Threading.Interlocked.Decrement(ref activeSearches);
            setSearchStatus();

            displayValsInCBCell(cc, searchResult.Results.Select(r => getDisplayStringForSeries(r)).ToArray());

            if (cc.Items.Count < 1)
                displayValsInCBCell(cc, "No Results found");

            var actionCell = row.Cells[ColIndexOf(colAction)] as DataGridViewComboBoxCell;

            // overwrite from the cellchanged event which set it to approve
            if (searchResult.PerfectMatch == null)
                actionCell.Value = displayedActions[UserInputResults.SeriesAction.Skip];

        }

        void displayValsInCBCell(DataGridViewComboBoxCell cell, params string[] values)
        {
            cell.Items.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                cell.Items.Add(values[i]);
                if (i == 0)
                    cell.Value = values[i];
            }
        }

        string getDisplayStringForSeries(DBOnlineSeries series)
        {
            return string.Format("{0} ({1})", series[DBOnlineSeries.cPrettyName], series[DBOnlineSeries.cID]);
        }
        
        Dictionary<string, UserInputResultSeriesActionPair> getApprovedResults()
        {
            var pageResult = new Dictionary<string, UserInputResultSeriesActionPair>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
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

        DBOnlineSeries getSeriesFromSelected(DataGridViewRow row)
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


        void setSearchStatus()
        {
            this.labelSearchStats.Text = string.Format("Searching in progress for {0} series ({1} queued)", activeSearches, queuedSearches);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (UserFinishedEditing != null)
                UserFinishedEditing(null, UserFinishedRequestedAction.Cancel);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (UserFinishedEditing != null)
                UserFinishedEditing(new UserInputResults(givenResults, getApprovedResults()), UserFinishedRequestedAction.Next);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (UserFinishedEditing != null)
                UserFinishedEditing(null, UserFinishedRequestedAction.Prev);
        }

        private void linkLabelCloseDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.groupBoxDetails.Visible = false;
        }

        private void checkBoxFilter_CheckedChanged(object sender, EventArgs e)
        {
            this.dataGridView1.SuspendLayout();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (checkBoxFilter.Checked && getActionFromRow(row) == UserInputResults.SeriesAction.Approve)
                {
                    row.Visible = false;
                }
                else row.Visible = true;
            }
            this.dataGridView1.ResumeLayout();
        }

        UserInputResults.SeriesAction getActionFromRow(DataGridViewRow row)
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
