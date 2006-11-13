using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries
{
    public class LocalParse
    {

        private bool m_bAsync = false;
        private BackgroundWorker worker = null;
        private List<parseResult> m_results;

        public delegate void LocalParseProgressHandler(int nProgress, List<parseResult> results);
        public delegate void LocalParseCompletedHandler(List<parseResult> results);
        public event LocalParseProgressHandler LocalParseProgress;
        public event LocalParseCompletedHandler LocalParseCompleted;

        public List<parseResult> Results
        {
            get { return m_results; }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<PathPair> files = Filelister.GetFiles();
            List<parseResult> results = new List<parseResult>();
            parseResult progressReporter;
            int nFailed = 0;
            int nCount = 0;
            foreach (PathPair file in files)
            {
                FilenameParser parser = new FilenameParser(file.sMatch_FileName);
                ListViewItem item = new ListViewItem(file.sMatch_FileName);
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
                        progressReporter.success = false;
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
                        progressReporter.success = false;
                        progressReporter.exception += "Episode not numerical";
                    }
                }

                progressReporter.match_filename = file.sMatch_FileName;
                progressReporter.full_filename = file.sFull_FileName;
                progressReporter.item = item;
                progressReporter.parser = parser;
                results.Add(progressReporter);

                if (m_bAsync && nCount++ % 50 == 0)
                {
                    worker.ReportProgress(Convert.ToInt32(100.0 / files.Count * nCount), results);
                    results = new List<parseResult>();
                }
                
                //nCount++;
            }
            e.Result = results;
        }

        public void DoParse(bool bAsync)
        {
            m_bAsync = bAsync;
            if (bAsync)
            {
                worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;
                worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                worker.RunWorkerAsync();
            }
            else
            {
                DoWorkEventArgs e = new DoWorkEventArgs(null);
                worker_DoWork(null, e);
                m_results = (List<parseResult>)e.Result;
            }
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<parseResult> results = (List<parseResult>)e.Result;
            if (LocalParseCompleted != null)
                LocalParseCompleted.Invoke(results);
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            List<parseResult> results = (List<parseResult>)e.UserState;
            if (LocalParseProgress != null)
                LocalParseProgress.Invoke(e.ProgressPercentage, results);
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
        public string match_filename;
        public string full_filename;
    }
}
