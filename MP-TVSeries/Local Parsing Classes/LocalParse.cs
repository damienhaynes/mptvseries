#region GNU license
// MP-TVSeries - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2007
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#endregion


using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries
{
    public class LocalParse
    {

        private BackgroundWorker worker = null;

        public delegate void LocalParseProgressHandler(int nProgress, List<parseResult> results);
        public delegate void LocalParseCompletedHandler(List<parseResult> results);
        public event LocalParseProgressHandler LocalParseProgress;
        public event LocalParseCompletedHandler LocalParseCompleted;

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<PathPair> files = Filelister.GetFiles();
            e.Result = Parse(files);
        }

        public static List<parseResult> Parse(List<PathPair> files)
        {
            List<parseResult> results = new List<parseResult>();
            parseResult progressReporter;
            int nFailed = 0;
            foreach (PathPair file in files)
            {
                FilenameParser parser = new FilenameParser(file.sMatch_FileName);
                ListViewItem item = new ListViewItem(file.sMatch_FileName);
                item.UseItemStyleForSubItems = true;
                
                progressReporter = new parseResult();
                
                // make sure we have all the necessary data for a full match
                if (!parser.Matches.ContainsKey(DBEpisode.cSeasonIndex) ||
                    !parser.Matches.ContainsKey(DBEpisode.cEpisodeIndex))
                {
                    if (parser.Matches.ContainsKey(DBSeries.cParsedName) &&
                        parser.Matches.ContainsKey(DBOnlineEpisode.cFirstAired))
                    {
                        try{ System.DateTime.Parse(parser.Matches[DBOnlineEpisode.cFirstAired]);}
                        catch (System.FormatException)
                        {
                            nFailed++;
                            progressReporter.failedAirDate = true;
                            progressReporter.success = false;
                            progressReporter.exception = "Airdate not valid";
                        }
                    }
                    else
                    {
                        progressReporter.success = false;
                        progressReporter.exception = "Parsing failed for " + file;

                        nFailed++;
                    }

                }
                else
                {
                    // make sure episode & season are properly matched (numerical values)
                    try { Convert.ToInt32(parser.Matches[DBEpisode.cSeasonIndex]); }
                    catch (System.FormatException)
                    {
                        nFailed++;
                        progressReporter.failedSeason = true;
                        progressReporter.success = false;
                        progressReporter.exception += "Season not numerical ";
                    }
                    try { Convert.ToInt32(parser.Matches[DBEpisode.cEpisodeIndex]); }
                    catch (System.FormatException)
                    {
                        nFailed++;
                        progressReporter.failedEpisode = true;
                        progressReporter.success = false;
                        progressReporter.exception += "Episode not numerical ";
                    }
                }

                progressReporter.match_filename = file.sMatch_FileName;
                progressReporter.full_filename = file.sFull_FileName;
                progressReporter.parser = parser;
                results.Add(progressReporter);
            }
            return results;
        }

        public void AsyncFullParse()
        {
            MPTVSeriesLog.Write("Starting Local Parsing operation - Async: yes", MPTVSeriesLog.LogLevel.Debug);
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerAsync();
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MPTVSeriesLog.Write("Finished Parsing operation - Async: True", MPTVSeriesLog.LogLevel.Debug);
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
        public bool failedAirDate = false;
        public string exception;
        public FilenameParser parser;
        public string match_filename;
        public string full_filename;
    }
}
