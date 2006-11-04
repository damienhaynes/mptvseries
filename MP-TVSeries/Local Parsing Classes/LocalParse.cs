using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries
{
    public class LocalParse
    {
        public BackgroundWorker worker = new BackgroundWorker();

        public LocalParse()
        {
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            String[] files = Filelister.GetFiles();
            List<parseResult> results = new List<parseResult>();
            parseResult progressReporter;
            int nFailed = 0;
            int nCount = 0;
            foreach (String file in files)
            {
                FilenameParser parser = new FilenameParser(file);
                ListViewItem item = new ListViewItem(file);
                item.UseItemStyleForSubItems = true;
                
                progressReporter = new parseResult();

                foreach (KeyValuePair<string, string> match in parser.Matches)
                {
                    ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem(item, match.Value);
                    subItem.Name = match.Key;
                    item.SubItems.Add(subItem);
                }

                // make sure we have all the necessary data for a full match
                if (!parser.Matches.ContainsKey(DBEpisode.cSeasonIndex) ||
                    !parser.Matches.ContainsKey(DBEpisode.cEpisodeIndex) ||
                    !parser.Matches.ContainsKey(DBSeries.cParsedName))
                {
                    progressReporter.success = false;
                    progressReporter.exception = "Parsing failed for " + file;
                    item.ForeColor = System.Drawing.Color.White;
                    item.BackColor = System.Drawing.Color.Tomato;
                    
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

                        progressReporter.failedSeason = true;
                        progressReporter.exception = "Season not numerical";
                    }
                    try { Convert.ToInt32(parser.Matches[DBEpisode.cEpisodeIndex]); }
                    catch (System.FormatException)
                    {
                        item.UseItemStyleForSubItems = false;
                        item.SubItems[DBEpisode.cEpisodeIndex].ForeColor = System.Drawing.Color.White;
                        item.SubItems[DBEpisode.cEpisodeIndex].BackColor = System.Drawing.Color.Tomato;
                        nFailed++;

                        progressReporter.failedEpisode = true;
                        progressReporter.exception += "Episode not numerical";
                    }
                }

                progressReporter.filename = file;
                progressReporter.item = item;
                progressReporter.parser = parser;
                results.Add(progressReporter);

                if (nCount++ % 50 == 0)
                {
                    worker.ReportProgress(Convert.ToInt32(100.0 / files.Length * nCount), results);
                    results = new List<parseResult>();
                }
                
                //nCount++;
            }
            e.Result = results;
        }

        public void DoParse()
        {
            worker.RunWorkerAsync();
        }
    }

    public class parseResult
    {
        public bool success = true;
        public bool failedSeason = false;
        public bool failedEpisode = false;
        public ListViewItem item;
        public string exception;
        public FilenameParser parser;
        public string filename;
    }
}
