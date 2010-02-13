using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries.Configuration
{    
    public partial class ImportPanelSeriesID : UserControl
    {
        public event userFinishedEditingDel UserFinishedEditing;

        const string cSearching = "Searching...";
        const string cWait2Search = "Waiting to Search...";

        Color AutoApproved = Color.Green;
        Color ManualApproved = Color.LightGreen;
        Color SkipColor = Color.Yellow;
        Color IgnoreColor = Color.Gray;

        IList<parseResult> givenResults = null;

        delegate void SearchProgressDelegate(string searchString, List<DBOnlineSeries> result, DataGridViewRow row);
        public ImportPanelSeriesID()
        {
            InitializeComponent();
        }

        Dictionary<int, string> lastSearch = new Dictionary<int, string>();

        public void SetResults(IList<parseResult> results)
        {
            givenResults = results;

            // get the parsed series
            var uniqueSeries = from result in results
                                where !string.IsNullOrEmpty(result.parser.Matches[DBSeries.cParsedName])
                                group result by result.parser.Matches[DBSeries.cParsedName];
            
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

        bool isGridPrepared = false;
        void prepareGrid()
        {
            // add the columns, fixed here
            var dgvcS = new DataGridViewTextBoxColumn();
            dgvcS.Name = "Series";
            dgvcS.ReadOnly = true;

            var dgvcO = new DataGridViewComboBoxColumn();
            dgvcO.Name = "Online Series";
            dgvcO.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            var dgvcSearch = new DataGridViewTextBoxColumn();
            dgvcSearch.Name = "Search different Name";
            dgvcSearch.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;

            var dgvcSearchOK = new DataGridViewButtonColumn();
            dgvcSearchOK.Name = "-";
            dgvcSearchOK.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            var dgvcApprove = new DataGridViewComboBoxColumn();
            dgvcApprove.Name = "Status";
            dgvcApprove.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dataGridView1.Columns.Add(dgvcS);
            dataGridView1.Columns.Add(dgvcO);
            dataGridView1.Columns.Add(dgvcSearch);
            dataGridView1.Columns.Add(dgvcSearchOK);
            dataGridView1.Columns.Add(dgvcApprove);

            dataGridView1.CellValueChanged += new DataGridViewCellEventHandler((sender, e) =>
            {
                var row = dataGridView1.Rows[e.RowIndex];
                // was onlinesearchresult changed?
                if (e.ColumnIndex == 1)
                {
                    // we dont do anything else, just approve                    
                    row.Cells[4].Value = UserInputResults.SeriesAction.Approve;
                    row.Tag = null; // for color coding
                }
                else if (e.ColumnIndex == 4)
                {
                    // seriesAction
                    // we color code
                    
                    var reqAction = (UserInputResults.SeriesAction)row.Cells[4].Value;
                    switch (reqAction)
                    {
                        case UserInputResults.SeriesAction.Skip:
                            row.DefaultCellStyle.BackColor = SkipColor;
                            break;
                        case UserInputResults.SeriesAction.IgnoreAlways:
                            row.DefaultCellStyle.BackColor = IgnoreColor;
                            break;
                        case UserInputResults.SeriesAction.Approve:
                            if (row.Cells[4].Tag == null) // manually approved
                                row.DefaultCellStyle.BackColor = ManualApproved;
                            else
                                row.DefaultCellStyle.BackColor = AutoApproved;
                            break;
                    }
                }
            });

            dataGridView1.CellContentClick += new DataGridViewCellEventHandler((sender, e) =>
            {

                // was search clicked?
                if (e.ColumnIndex == 3)
                {
                    // which row was clicked?
                    var row = dataGridView1.Rows[e.RowIndex];
                    // fire off a new search, if user typed something
                    string customSearch = row.Cells[2].Value as string;
                    if (!string.IsNullOrEmpty(customSearch))
                        FireOffSearch(row.Tag as IGrouping<string, parseResult>, row, customSearch);
                }

            });

            isGridPrepared = true;
        }

        void FillGrid(List<IGrouping<string, parseResult>> uniqueNewSeries)
        {
            dataGridView1.SuspendLayout();
            
            if (!isGridPrepared) prepareGrid();

            foreach (var newSeries in uniqueNewSeries)
            {
                var row = new DataGridViewRow();
                var seriesCell = new DataGridViewTextBoxCell();
                seriesCell.Value = newSeries.Key;
                row.Cells.Add(seriesCell);

                var onlineSeriesCell = new DataGridViewComboBoxCell();
                onlineSeriesCell.Items.Add(cWait2Search);
                onlineSeriesCell.Value = cWait2Search;                         
                row.Cells.Add(onlineSeriesCell);

                var searchCell = new DataGridViewTextBoxCell();
                searchCell.Value = string.Empty;
                row.Cells.Add(searchCell);

                var searchOKCell = new DataGridViewButtonCell();
                searchOKCell.Value = "Search";                
                row.Cells.Add(searchOKCell);

                var approveCell = new DataGridViewComboBoxCell();
                approveCell.Items.Add(UserInputResults.SeriesAction.Approve);
                approveCell.Items.Add(UserInputResults.SeriesAction.Skip);
                approveCell.Items.Add(UserInputResults.SeriesAction.IgnoreAlways);
                approveCell.Value = UserInputResults.SeriesAction.Skip;
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

            System.Threading.ThreadPool.QueueUserWorkItem((o) =>
                {
                    // tell user we are now searching
                    SearchProgress(cSearching, null, row);
                    GetSeries gs = new GetSeries(toSearch);
                    // and give the results
                    SearchProgress(toSearch, gs.Results, row);
                });
            
        }

        void SearchProgress(string searchString, List<DBOnlineSeries> result, DataGridViewRow row)
        {
            
            // we need to invoke
            if (this.dataGridView1.InvokeRequired)
            {                
                this.dataGridView1.Invoke(new SearchProgressDelegate(SearchProgress), searchString, result, row);
                return;
            }

            // lets update the combobox
            var cc = new DataGridViewComboBoxCell();
            row.Cells[1] = cc;

            // ok, if we only get a string and the row, display that string
            if (result == null)
            {
                cc.Items.Add(searchString); // not really searchstring, probably statusmsg
                cc.Value = searchString;
                return;
            }

            // else we got the results
            // lets see about ordering, maybe auto approvable
            var perfectMatch = RankSearchResults(searchString, result, out result);
            cc.Tag = result;

            foreach (var r in result)
                cc.Items.Add(getDisplayStringForSeries(r));

            if (cc.Items.Count < 1)
                cc.Items.Add("No Results found");

            cc.Value = cc.Items[0];

            // if we found a perfect match, set approved to true
            if (perfectMatch == null)
                row.Cells[4].Value = UserInputResults.SeriesAction.Skip;
            else
            {
                row.Cells[4].Value = UserInputResults.SeriesAction.Approve;
                row.Tag = "auto"; // for color coding
            }
        }

        string getDisplayStringForSeries(DBOnlineSeries series)
        {
            return string.Format("{0} ({1})", series[DBOnlineSeries.cPrettyName], series[DBOnlineSeries.cID]);
        }

        DBOnlineSeries RankSearchResults(string name, IList<DBOnlineSeries> candidates, out List<DBOnlineSeries> orderedCandidates)
        {          

            // calculate distances
            // note: this should also be done in GetSeries, but it seems to simplistic, I don't trust it :-(
            var bestMatch = (from candidate in candidates                            
                            select new { LSDistance = MediaPortal.Util.Levenshtein.Match(name.ToLowerInvariant(), candidate[DBOnlineSeries.cPrettyName].ToString().ToLowerInvariant()),
                                         Series = candidate });
            
            // make them unique
            // note: this is different from onlineparse, should probably pick one implementation (read: this one!)           
            var uniqueResults = from candidate in bestMatch
                                group candidate by (int)candidate.Series[DBOnlineSeries.cID];
            
            // now order the series by their minLSDistance (each ID can have several series and thus names)
            // we dont care which one won, we just want the minimum it scored
            // we also pick out the series in the users lang, and the englis lang
            var weightedUniqueResults = from ur in uniqueResults
                                        select new 
                                        {  
                                            MinLSDistance  = ur.Min(r => r.LSDistance), 
                                            SeriesScored   = ur.OrderBy( r => r.LSDistance).FirstOrDefault(),
                                            SeriesUserLang = ur.FirstOrDefault( r => r.Series["language"] == Online_Parsing_Classes.OnlineAPI.SelLanguageAsString),
                                            SeriesEng      = ur.FirstOrDefault( r => r.Series["language"] == "en"),                                         
                                        };
            
            // now decide which one to display
            // 1) userlang 2) english 3) whichever scored our best result, this has to exist
            var weightedDisplayResults = from dr in weightedUniqueResults
                                         orderby dr.MinLSDistance
                                         select new
                                         {
                                             LSDistance = dr.MinLSDistance,
                                             Series = (dr.SeriesUserLang != null && dr.SeriesUserLang.Series != null) ? dr.SeriesUserLang.Series
                                                        : (dr.SeriesEng != null && dr.SeriesEng.Series != null) ? dr.SeriesEng.Series
                                                        : dr.SeriesScored.Series,
                                         };

            
            // give the ordered results back for displaying
            orderedCandidates = weightedDisplayResults.Select(r => r.Series).ToList();
            
            // get the best one thats under a distance of 2, which is a bit more fuzzy than the perfect requirement in onlineparse
            // this could be tweaked
            var best = weightedDisplayResults.FirstOrDefault(m => m.LSDistance < 2);
            if (best != null)
                return best.Series;

            return null;
         }

        Dictionary<string, UserInputResultSeriesActionPair> getApprovedResults()
        {
            var pageResult = new Dictionary<string, UserInputResultSeriesActionPair>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string inputSeriesName = row.Cells[0].Value as string;
                var requestedAction = (UserInputResults.SeriesAction)row.Cells[4].Value;
                DBOnlineSeries chosenSeries = null;

                // else we dont even care
                if (requestedAction == UserInputResults.SeriesAction.Approve)
                {
                    var seriesList = row.Cells[1].Tag as List<DBOnlineSeries>;
                    if (seriesList != null)
                    {
                        string toMatch = row.Cells[1].Value as string;
                        chosenSeries = seriesList.SingleOrDefault(s => toMatch == getDisplayStringForSeries(s));
                    }
                }

                pageResult.Add(inputSeriesName, new UserInputResultSeriesActionPair(requestedAction, chosenSeries));         
            }

            return pageResult;
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
    }
}
