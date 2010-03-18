using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WindowPlugins.GUITVSeries.Configuration.CollapsableDataGrid;

namespace WindowPlugins.GUITVSeries.Configuration
{
    public partial class ImportPanel : UserControl
    {
        public ImportPanel()
        {
            InitializeComponent();
            this.dataGrid.Visible = false;
        }

        public void Start()
        {
            LocalParse lp = new LocalParse();
            lp.LocalParseCompleted += new LocalParse.LocalParseCompletedHandler(lp_LocalParseCompleted);
            lp.LocalParseProgress += new LocalParse.LocalParseProgressHandler(lp_LocalParseProgress);
            lp.AsyncFullParse();
        }

        void lp_LocalParseProgress(int nProgress, List<parseResult> results)
        {
            this.lblWaitStatus.Text = string.Format("{0} files found so far...", results.Count);
        }

        void lp_LocalParseCompleted(List<parseResult> results)
        {
            this.lblWait.Text = "Please review parsed File Info below and make changes if nessecary";
            this.lblWaitStatus.Text = "Click next when finished";
            LoadResultsToGrid(results);
            this.dataGrid.Visible = true;
        }

        void LoadResultsToGrid(List<parseResult> results)
        {
            var headerStyle = new DataGridViewCellStyle();
            headerStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            headerStyle.BackColor = Color.AliceBlue;

            // group by series
            var bySeries = from result in results
                           group result by result.parser.Matches.FirstOrDefault(t => t.Key == "Parsed_Name").Value;
            bySeries = bySeries.OrderBy(bs => bs.Key);               
            
            foreach (var series in bySeries)
            {
                var row = new CollapsableDataGridViewRow(this.dataGrid,
                          new CollapsableDataGridViewRowCell(typeof(DataGridViewTextBoxCell), "Parsed_Name", series.Key));
                row.HeaderStyle = headerStyle;
                foreach (var item in series)
                {
                    var fileRowVals = 
                        from val in item.parser.Matches
                            select new CollapsableDataGridViewRowCell(
                                          typeof(DataGridViewTextBoxCell), 
                                          val.Key,val.Value);
                    var fileRow = new CollapsableDataGrid.CollapsableDataGridViewRow(this.dataGrid, fileRowVals.ToArray());
                    
                    // for unparsable items make sure std. headers are there
                    if (fileRowVals.Count(v => v.ColumnName == "Parsed_Name") < 1)
                        fileRow.AddCell(new CollapsableDataGridViewRowCell(typeof(DataGridViewTextBoxCell), "Parsed_Name", string.Empty));
                    if (fileRowVals.Count(v => v.ColumnName == "Parsed_Name") < 1)
                        fileRow.AddCell(new CollapsableDataGridViewRowCell(typeof(DataGridViewTextBoxCell), "Parsed_Name", string.Empty));
                    if (fileRowVals.Count(v => v.ColumnName == "Parsed_Name") < 1)
                        fileRow.AddCell(new CollapsableDataGridViewRowCell(typeof(DataGridViewTextBoxCell), "Parsed_Name", string.Empty));

                    // also add arbitrary other vals
                    fileRow.AddCell(new CollapsableDataGridViewRowCell(typeof(DataGridViewTextBoxCell), "Filename", item.full_filename));
                    
                    
                    // now add all the extra info
                    fileRow.CollapsedRows.Add(new CollapsableDataGridViewRow(this.dataGrid,
                                              new CollapsableDataGridViewRowCell(typeof(DataGridViewTextBoxCell),
                                                                                 "Expression Matched:",
                                                                                 item.parser.RegexpMatched)));

                    row.CollapsedRows.Add(fileRow);
                }

                if (this.dataGrid.ColumnCount < row.Cells.Count)
                    this.dataGrid.ColumnCount = row.Cells.Count;
                this.dataGrid.Rows.Add(row);     
            }        
        }
    }
}
