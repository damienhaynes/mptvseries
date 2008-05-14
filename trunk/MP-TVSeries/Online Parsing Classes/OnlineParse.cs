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
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;

namespace WindowPlugins.GUITVSeries
{

    enum ParsingAction
    {
        Full,
        List_Add,
        List_Remove,
        LocalScanNoExactMatch,
    }

    class CParsingParameters
    {
        public ParsingAction m_action = ParsingAction.Full;
        public bool m_bLocalScan = true;
        public bool m_bUpdateScan = true;
        public List<PathPair> m_files = null;

        public CParsingParameters(bool bScanNew, bool bUpdateExisting)
        {
            m_bLocalScan = bScanNew;
            m_bUpdateScan = bUpdateExisting;
        }

        public CParsingParameters(ParsingAction action, List<PathPair> files)
        {
            m_action = action;
            m_files = files;
            m_bLocalScan = true;
            m_bUpdateScan = false;
        }
    };

    class OnlineParsing
    {
        public BackgroundWorker worker = new BackgroundWorker();
        Feedback.Interface m_feedback = null;

        bool m_bDataUpdated = false;
        bool m_bFullSeriesRetrieval = false;
        bool m_bReparseNeeded = false;
        bool m_bNoExactMatch = false;
        CParsingParameters m_params = null;

        public delegate void OnlineParsingProgressHandler(int nProgress);
        public delegate void OnlineParsingCompletedHandler(bool bDataUpdated);

        /// <summary>
        /// This will be triggered once all the SeriesAndEpisodeInfo has been parsed completely.
        /// </summary>
        public event OnlineParsingProgressHandler OnlineParsingProgress;
        public event OnlineParsingCompletedHandler OnlineParsingCompleted;

        public OnlineParsing(Feedback.Interface feedback)
        {
            m_feedback = feedback;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (OnlineParsingCompleted != null) // only if any subscribers exist
            {
                this.OnlineParsingCompleted.Invoke(m_bDataUpdated);
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (OnlineParsingProgress != null) // only if any subscribers exist
                OnlineParsingProgress.Invoke(e.ProgressPercentage);
        }

        public bool IsWorking
        {
            get { return worker.IsBusy; }
        }

        public bool Start(CParsingParameters param)
        {
            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync(param);
                return true;
            }
            return false;
        }

        public bool LocalScan
        {
            get { if (m_params != null) return m_params.m_bLocalScan; else return false; }
        }

        public bool UpdateScan
        {
            get { if (m_params != null) return m_params.m_bUpdateScan; else return false; }
        }

        public void Cancel()
        {
            worker.CancelAsync();
            //            m_bAbort = true;
        }

        public void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            m_params = e.Argument as CParsingParameters;
            m_bFullSeriesRetrieval = DBOption.GetOptions(DBOption.cFullSeriesRetrieval);
            m_bNoExactMatch = false;
            worker.ReportProgress(0);
            string initialMsg = string.Empty; ;
            switch (m_params.m_action)
            {
                case ParsingAction.List_Remove:
                    initialMsg = "*******************    Remove Run Starting     ***************************";
                    MPTVSeriesLog.Write(prettyStars(initialMsg.Length));
                    MPTVSeriesLog.Write(initialMsg);
                    MPTVSeriesLog.Write(prettyStars(initialMsg.Length));
                    // should we remove deleted files?
                    if (!DBOption.GetOptions(DBOption.cDontClearMissingLocalFiles))
                    {
                        List<DBOnlineSeries> relatedSeries = new List<DBOnlineSeries>();
                        List<DBSeason> relatedSeasons = new List<DBSeason>();

                        foreach (PathPair pair in m_params.m_files)
                        {
                            if(!LocalParse.isOnRemovable(pair.m_sFull_FileName))
                            {
                                DBEpisode episode = new DBEpisode(pair.m_sFull_FileName);
                                
                                // already in?
                                bool bSeasonFound = false;
                                foreach (DBSeason season in relatedSeasons)
                                    if (season[DBSeason.cSeriesID] == episode[DBEpisode.cSeriesID] && season[DBSeason.cIndex] == episode[DBEpisode.cSeasonIndex])
                                    {
                                        bSeasonFound = true;
                                        break;
                                    }
                                if (!bSeasonFound)
                                    relatedSeasons.Add(new DBSeason(episode[DBEpisode.cSeriesID], episode[DBEpisode.cSeasonIndex]));

                                bool bSeriesFound = false;
                                foreach (DBOnlineSeries series in relatedSeries)
                                    if (series[DBOnlineSeries.cID] == episode[DBEpisode.cSeriesID])
                                    {
                                        bSeriesFound = true;
                                        break;
                                    }
                                if (!bSeriesFound)
                                    relatedSeries.Add(new DBOnlineSeries(episode[DBEpisode.cSeriesID]));

                                SQLCondition condition = new SQLCondition();
                                condition.Add(new DBEpisode(), DBEpisode.cFilename, pair.m_sFull_FileName, SQLConditionType.Equal);
                                DBEpisode.Clear(condition);
                            }
                        }

                        // now go over the touched seasons & series
                        foreach (DBSeason season in relatedSeasons)
                        {
                            if (DBEpisode.Get(season[DBSeason.cSeriesID], season[DBSeason.cIndex], false).Count > 0)
                            {
                                season[DBSeason.cHasLocalFilesTemp] = true;
                                season[DBSeason.cHasEpisodes] = true;
                            }
                            else
                                season[DBSeason.cHasLocalFilesTemp] = false;

                            DBEpisode episode = DBEpisode.GetFirstUnwatched(season[DBSeason.cSeriesID], season[DBSeason.cIndex]);
                            if (episode != null)
                                season[DBSeason.cUnwatchedItems] = true;
                            else
                                season[DBSeason.cUnwatchedItems] = false;

                            season.Commit();
                        }

                        foreach (DBOnlineSeries series in relatedSeries)
                        {
                            if (DBEpisode.Get((int)series[DBOnlineSeries.cID], false).Count > 0)
                                series[DBOnlineSeries.cHasLocalFilesTemp] = true;
                            else
                                series[DBOnlineSeries.cHasLocalFilesTemp] = false;

                            DBEpisode episode = DBEpisode.GetFirstUnwatched(series[DBSeries.cID]);
                            if (episode != null)
                                series[DBOnlineSeries.cUnwatchedItems] = true;
                            else
                                series[DBOnlineSeries.cUnwatchedItems] = false;

                            series.Commit();
                        }

                        // and copy the HasLocalFileTemp value into the real one
                        DBSeries.GlobalSet(DBOnlineSeries.cHasLocalFiles, DBOnlineSeries.cHasLocalFilesTemp);
                        DBSeason.GlobalSet(DBSeason.cHasLocalFiles, DBSeason.cHasLocalFilesTemp);
                        // and we are done, the backgroundworker is going to notify so
                        MPTVSeriesLog.Write("***************************************************************************");
                        MPTVSeriesLog.Write("*******************          Completed          ***************************");
                        MPTVSeriesLog.Write("***************************************************************************");
                    }
                    return;

                case ParsingAction.List_Add:
                    initialMsg = "*******************       Add Run Starting     ***************************";
                    MPTVSeriesLog.Write(prettyStars(initialMsg.Length));
                    MPTVSeriesLog.Write(initialMsg);
                    MPTVSeriesLog.Write(prettyStars(initialMsg.Length));
                    ParseLocal(m_params.m_files);
                    break;

                case ParsingAction.LocalScanNoExactMatch:
                    initialMsg = String.Format("******************* NoExactMatch Run Starting (LocalScan: {0} -  UpdateScan: {1})   ***************************", m_params.m_bLocalScan, m_params.m_bUpdateScan);
                    MPTVSeriesLog.Write(prettyStars(initialMsg.Length));
                    MPTVSeriesLog.Write(initialMsg);
                    MPTVSeriesLog.Write(prettyStars(initialMsg.Length));
                    m_bNoExactMatch = true;
                    goto case ParsingAction.Full;

                case ParsingAction.Full:
                    initialMsg = String.Format("******************* Full Run Starting (LocalScan: {0} -  UpdateScan: {1})   ***************************", m_params.m_bLocalScan, m_params.m_bUpdateScan);
                    MPTVSeriesLog.Write(prettyStars(initialMsg.Length));
                    MPTVSeriesLog.Write(initialMsg);
                    MPTVSeriesLog.Write(prettyStars(initialMsg.Length));
                    if (m_params.m_bLocalScan)
                    {
                        // mark all files in the db as not processed (to figure out which ones we'll have to remove after the import)
                        DBEpisode.GlobalSet(DBEpisode.cImportProcessed, 2);
                        // also clear all season & series for local files
                        DBSeries.GlobalSet(DBOnlineSeries.cHasLocalFilesTemp, false);
                        DBSeason.GlobalSet(DBSeason.cHasLocalFilesTemp, false);

                        List<String> listFolders = new List<string>();
                        DBImportPath[] importPathes = DBImportPath.GetAll();
                        if (importPathes != null)
                        {
                            foreach (DBImportPath importPath in importPathes)
                            {
                                if (importPath[DBImportPath.cEnabled] != 0)
                                {
                                    listFolders.Add(importPath[DBImportPath.cPath]);
                                }
                            }
                        }

                        ParseLocal(Filelister.GetFiles(listFolders));
                        
                        // now, remove all episodes still processed = 0, the weren't find in the scan
                        if (!DBOption.GetOptions(DBOption.cDontClearMissingLocalFiles))
                        {
                            SQLCondition condition = new SQLCondition();
                            condition.Add(new DBEpisode(), DBEpisode.cImportProcessed, 2, SQLConditionType.Equal);
                            condition.Add(new DBEpisode(), DBEpisode.cIsOnRemovable, false, SQLConditionType.Equal);
                            DBEpisode.Clear(condition);
                        }
                        // and copy the HasLocalFileTemp value into the real one
                        DBSeries.GlobalSet(DBOnlineSeries.cHasLocalFiles, DBOnlineSeries.cHasLocalFilesTemp);
                        DBSeason.GlobalSet(DBSeason.cHasLocalFiles, DBSeason.cHasLocalFilesTemp);
                    } 
                    break;
            }

            // now on with online parsing
            if (DBOption.GetOptions(DBOption.cOnlineParseEnabled) == 1)
            {
                int counter = 0;
                m_bReparseNeeded = true;
                while (m_bReparseNeeded && counter < 4) // limit the max number of loops
                {
                    m_bReparseNeeded = false;

                    // 1) Identify any new/unidentified Series
                    GetSeries();

                    // 2) Download metadata for them
                    UpdateSeries(true, null); // todo: ask orderoption

                    // 3) identifies new episodes
                    GetEpisodes();                                                         
                    
                    counter++;

                }

                // if we do updates too, let's get the current timestamp
                if (m_params.m_bUpdateScan)
                {
                    // let's get the time we last updated from the local options
                    long lastUpdateTimeStamp = DBOption.GetOptions(DBOption.cUpdateTimeStamp);
                    double curTimeStamp = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                    double sinceLastUpdate = curTimeStamp - lastUpdateTimeStamp;
                    
                    Online_Parsing_Classes.OnlineAPI.UpdateType uType = WindowPlugins.GUITVSeries.Online_Parsing_Classes.OnlineAPI.UpdateType.all;
                    if (sinceLastUpdate < 3600 * 24)
                        uType = WindowPlugins.GUITVSeries.Online_Parsing_Classes.OnlineAPI.UpdateType.day;
                    else if (sinceLastUpdate < 3600 * 24 * 7)
                        uType = WindowPlugins.GUITVSeries.Online_Parsing_Classes.OnlineAPI.UpdateType.week;
                    else if (sinceLastUpdate < 3600 * 24 * 30)
                        uType = WindowPlugins.GUITVSeries.Online_Parsing_Classes.OnlineAPI.UpdateType.month;
                    //uType = WindowPlugins.GUITVSeries.Online_Parsing_Classes.OnlineAPI.UpdateType.month;
                    Online_Parsing_Classes.GetUpdates GU = new WindowPlugins.GUITVSeries.Online_Parsing_Classes.GetUpdates(uType);

                    // update series
                    UpdateSeries(false, GU.UpdatedSeries);

                    // update episodes
                    UpdateEpisodes(GU.UpdatedEpisodes);

                    // update banners
                    UpdateBanners(false, GU.UpdatedSeries);

                    // lets save the updateTimestamp
                    if (GU.OnlineTimeStamp > 0)
                        DBOption.SetOptions(DBOption.cUpdateTimeStamp, GU.OnlineTimeStamp);
                }
                UpdateBanners(true, null);// update new series for banners                             

                UpdateEpisodeThumbNails();
                Online_Parsing_Classes.OnlineAPI.ClearBuffer();
            }
            // and we are done, the backgroundworker is going to notify so
            MPTVSeriesLog.Write("***************************************************************************");
            MPTVSeriesLog.Write("*******************          Completed          ***************************");
            MPTVSeriesLog.Write("***************************************************************************");
        }

        /// <summary>
        /// sets:
        ///  - DBEpisode.cImportProcessed to 1
        ///  - DBSeason.cHasLocalFilesTemp to true
        ///  - DBSeries.cHasLocalFilesTemp to true
        /// </summary>
        /// <param name="filenames"></param>
        static void UpdateStatus(List<string> filenames)
        {
            if (filenames.Count == 0) return;
            SQLCondition cond = new SQLCondition();
            
            // for huge libraries a stringBuilder is much better than adding every filename seperatly to the condition (and for small ones it hardly matters)
            // sqllite expression tree can only be 1000 deep though, to be safe we do 500 at once and no more
            List<SQLCondition> importProcessedConds = new List<SQLCondition>();
            try
            {
                StringBuilder condBuilder = new StringBuilder();
                string field = DBEpisode.Q(DBEpisode.cFilename);
                int count = 0;
                foreach (string file in filenames)
                {
                    if (condBuilder.Length > 0)
                        condBuilder.Append(" or ");
                    condBuilder.Append(field).Append(" = '").Append(file.Replace("'", "''")).Append('\'');
                    if (count++ >= 500)
                    {
                        cond.AddCustom(condBuilder.ToString());
                        importProcessedConds.Add(cond);
                        cond = new SQLCondition();
                        condBuilder.Remove(0, condBuilder.Length);
                        count = 0;
                    }
                }
                if (count > 0)
                {
                    cond.AddCustom(condBuilder.ToString());
                    importProcessedConds.Add(cond);
                }
            }
            catch ( Exception ex)
            {
                MPTVSeriesLog.Write(ex.Message);
                return;
            }

            foreach(SQLCondition condition in importProcessedConds)
                DBEpisode.GlobalSet(DBEpisode.cImportProcessed, 1, condition);

            SQLCondition condSeason = new SQLCondition();
            condSeason.AddCustom(" exists( select " + DBEpisode.Q(DBEpisode.cFilename) + " from " + DBEpisode.cTableName
                            + " where " + DBEpisode.cSeriesID + " = " + DBSeason.Q(DBSeason.cSeriesID) + " and "
                            + DBEpisode.cSeasonIndex + " = " + DBSeason.Q(DBSeason.cIndex) + " and " + DBEpisode.Q(DBEpisode.cImportProcessed) + " = 1 "   + ")");
            DBSeason.GlobalSet(DBSeason.cHasLocalFilesTemp, true, condSeason);

            SQLCondition condSeries = new SQLCondition();
            condSeries.AddCustom(" exists( select " + DBEpisode.Q(DBEpisode.cFilename) + " from " + DBEpisode.cTableName
                            + " where " + DBEpisode.cSeriesID + " = " + DBOnlineSeries.Q(DBOnlineSeries.cID) +
                            " and " + DBEpisode.Q(DBEpisode.cImportProcessed) + " = 1 " + ")");
            DBSeries.GlobalSet(DBOnlineSeries.cHasLocalFilesTemp, true, condSeries);
        }

        void ParseLocal(List<PathPair> files)
        {
            MPTVSeriesLog.Write(bigLogMessage("Gathering Local Information"));
            List<parseResult> parsedFiles = LocalParse.Parse(files, false);
            
            // don't process those already in DB
            List<string> dbEps = new List<string>();
            SQLite.NET.SQLiteResultSet results = DBTVSeries.Execute("select episodefilename from local_episodes");
            if (results.Rows.Count > 0)
            {
                for (int index = 0; index < results.Rows.Count; index++)
                {
                    dbEps.Add(results.Rows[index].fields[0]);
                }
            }
            List<string> updateStatusEps = new List<string>();
            for (int i = 0; i < parsedFiles.Count; i++)
            {
                if (dbEps.Contains(parsedFiles[i].full_filename))
                {
                    updateStatusEps.Add(parsedFiles[i].full_filename);
                    parsedFiles.RemoveAt(i);
                    i--;
                }
            }

            UpdateStatus(updateStatusEps);
            MPTVSeriesLog.Write(parsedFiles.Count.ToString() + " local files found that sucessfully parsed and are not already in Database, now adding/upading Database");
            int nSeason = 0;
            List<DBSeries> relatedSeries = new List<DBSeries>();
            List<DBSeason> relatedSeasons = new List<DBSeason>();

            foreach (parseResult progress in parsedFiles)
            {
                if (worker.CancellationPending)
                    return;
                if (progress.success)
                {
                    DBSeries series = null;
                    if (progress.parser.Matches.ContainsKey(DBOnlineEpisode.cFirstAired))
                    {
                        // series first
                        series = new DBSeries(progress.parser.Matches[DBSeries.cParsedName].ToLower());
                        series[DBOnlineSeries.cHasLocalFilesTemp] = 1;
                        // not much to do here except commiting the series
                        series.Commit();
                    }
                    else
                    {
                        nSeason = Convert.ToInt32(progress.parser.Matches[DBEpisode.cSeasonIndex]);

                        // ok, we are sure it's valid now
                        // series first
                        series = new DBSeries(progress.parser.Matches[DBSeries.cParsedName].ToLower());
                        series[DBOnlineSeries.cHasLocalFilesTemp] = 1;
                        // not much to do here except commiting the series
                        series.Commit();

                        // season now
                        DBSeason season = new DBSeason(series[DBSeries.cID], nSeason);
                        season[DBSeason.cHasLocalFilesTemp] = true;
                        season[DBSeason.cHasEpisodes] = true;
                        season.Commit();

                        // already in?
                        bool bSeasonFound = false;
                        foreach (DBSeason seasonLoop in relatedSeasons)
                            if (seasonLoop[DBSeason.cSeriesID] == series[DBOnlineSeries.cID] && seasonLoop[DBSeason.cIndex] == nSeason)
                            {
                                bSeasonFound = true;
                                break;
                            }
                        if (!bSeasonFound)
                            relatedSeasons.Add(season);

                        bool bSeriesFound = false;
                        foreach (DBSeries seriesLoop in relatedSeries)
                            if (seriesLoop[DBOnlineSeries.cID] == series[DBOnlineSeries.cID])
                            {
                                bSeriesFound = true;
                                break;
                            }
                        if (!bSeriesFound)
                            relatedSeries.Add(series);

                    }
                    
                    // then episode
                    DBEpisode episode = new DBEpisode(progress.full_filename);
                    bool bNewFile = false;
                    if (episode[DBEpisode.cImportProcessed] != 2)
                    {
                        m_bDataUpdated = true;
                        bNewFile = true;
                    }

                    episode[DBEpisode.cImportProcessed] = 1;
                    episode[DBEpisode.cSeriesID] = series[DBSeries.cID];
                    if (progress.parser.Matches.ContainsKey(DBEpisode.cEpisodeIndex2))
                    {
                        episode[DBEpisode.cEpisodeIndex2] = progress.parser.Matches[DBEpisode.cEpisodeIndex2];
                        episode[DBEpisode.cCompositeID2] = episode[DBEpisode.cSeriesID] + "_" + nSeason + "x" + episode[DBEpisode.cEpisodeIndex2];
                    }

                    episode[DBEpisode.cAvailableSubtitles] = episode.checkHasSubtitles();
                    
                    foreach (KeyValuePair<string, string> match in progress.parser.Matches)
                    {
                        if (match.Key != DBSeries.cParsedName)
                        {
                            episode.AddColumn(match.Key, new DBField(DBField.cTypeString));
                            if (bNewFile || (episode[match.Key] != null && episode[match.Key] != match.Value))
                                episode[match.Key] = match.Value;
                        }
                    }
                    episode.Commit();
                }
            }

            // now go over the touched seasons & series
            foreach (DBSeason season in relatedSeasons)
            {
                DBEpisode episode = DBEpisode.GetFirstUnwatched(season[DBSeason.cSeriesID], season[DBSeason.cIndex]);
                if (episode != null)
                    season[DBSeason.cUnwatchedItems] = true;
                else
                    season[DBSeason.cUnwatchedItems] = false;

                season.Commit();
            }

            foreach (DBSeries series in relatedSeries)
            {
                DBEpisode episode = DBEpisode.GetFirstUnwatched(series[DBSeries.cID]);
                if (episode != null)
                    series[DBOnlineSeries.cUnwatchedItems] = true;
                else
                    series[DBOnlineSeries.cUnwatchedItems] = false;

                series.Commit();
            }
        }

        void GetSeries()
        {
            MPTVSeriesLog.Write(bigLogMessage("Identifying Unknown Series Online"));

            SQLCondition condition = null;
            //if (m_params.m_bUpdateScan)
            //{
            //    // mark existing online data as "old", needs a refresh
            //    condition = new SQLCondition();
            //    condition.Add(new DBOnlineSeries(), DBOnlineSeries.cOnlineDataImported, 2, SQLConditionType.Equal);
            //    DBTable.GlobalSet(new DBOnlineSeries(), DBOnlineSeries.cOnlineDataImported, 1, condition);

            //    // mark existing banners as "old", needs a refresh too
            //    condition = new SQLCondition();
            //    condition.Add(new DBOnlineSeries(), DBOnlineSeries.cBannersDownloaded, 2, SQLConditionType.Equal);
            //    DBTable.GlobalSet(new DBOnlineSeries(), DBOnlineSeries.cBannersDownloaded, 1, condition);
            //}

            condition = new SQLCondition();
            // all series that don't have an onlineID ( < 0) and not marked as ignored
            condition.Add(new DBSeries(), DBSeries.cID, 0, SQLConditionType.LessThan);
            condition.Add(new DBSeries(), DBSeries.cScanIgnore, 0, SQLConditionType.Equal);

            int nIndex = 0;
            List<DBSeries> seriesList = DBSeries.Get(condition, false, false);
            if(seriesList.Count > 0)
                MPTVSeriesLog.Write(string.Format("Found {0} Unknown Series, attempting to identify them now...", seriesList.Count));
            else
                MPTVSeriesLog.Write("All Series are already identified.");

            foreach (DBSeries series in seriesList) {
                if (worker.CancellationPending)
                    return;

                worker.ReportProgress(10 + (10 * nIndex / seriesList.Count));
                nIndex++;

                String sSeriesNameToSearch = series[DBSeries.cParsedName];
                DBOnlineSeries UserChosenSeries = SearchForSeries(sSeriesNameToSearch);

                if (UserChosenSeries != null) // make sure selection was not cancelled
                    {
                    // set the ID on the current series with the one from the chosen one
                    // we need to update all depending items - seasons & episodes
                    List<DBSeason> seasons = DBSeason.Get(series[DBSeries.cID]);
                    foreach (DBSeason season in seasons)
                        season.ChangeSeriesID(UserChosenSeries[DBSeries.cID]);

                    SQLCondition setcondition = new SQLCondition();
                    setcondition.Add(new DBSeason(), DBSeason.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                    DBSeason.Clear(setcondition);

                    setcondition = new SQLCondition();
                    setcondition.Add(new DBEpisode(), DBEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                    List<DBEpisode> episodes = DBEpisode.Get(setcondition, false);
                    foreach (DBEpisode episode in episodes)
                        episode.ChangeSeriesID(UserChosenSeries[DBSeries.cID]);

                    setcondition = new SQLCondition();
                    setcondition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                    DBOnlineEpisode.Clear(setcondition);

                    int nSeriesID = series[DBSeries.cID];                    

                    series.ChangeSeriesID(UserChosenSeries[DBSeries.cID]);
                    series.Commit();

                    setcondition = new SQLCondition();
                    setcondition.Add(new DBOnlineSeries(), DBOnlineSeries.cID, nSeriesID, SQLConditionType.Equal);
                    DBOnlineSeries.Clear(setcondition);

                    // only keep one local dbseries marked as non dupe
                    setcondition = new SQLCondition();
                    setcondition.Add(new DBOnlineSeries(), DBOnlineSeries.cID, UserChosenSeries[DBSeries.cID], SQLConditionType.Equal);
                    List<DBSeries> seriesDupeSetList = DBSeries.Get(setcondition);
                    bool bFirst = true;
                    foreach (DBSeries seriesDupeSet in seriesDupeSetList) {
                        if (bFirst) {
                            seriesDupeSet[DBSeries.cDuplicateLocalName] = 0;
                            seriesDupeSet.Commit();
                            bFirst = false;
                        } else {
                            seriesDupeSet[DBSeries.cDuplicateLocalName] = 1;
                            seriesDupeSet.Commit();
                        }
                    }
                }

            }
        }

        public DBOnlineSeries SearchForSeries(string seriesName) {
            string nameToSearch = seriesName;
            
            while (true) {
                // query online db for possible matches
                GetSeries GetSeriesParser = new GetSeries(nameToSearch);

                // try to find an exact match in our results, if found, return
                if (DBOption.GetOptions(DBOption.cAutoChooseSeries) == 1)
                {
                    foreach (DBOnlineSeries onlineSeries in GetSeriesParser.Results)
                    {
                        if (!m_bNoExactMatch && !Helper.String.IsNullOrEmpty(onlineSeries[DBOnlineSeries.cPrettyName]) &&
                           (onlineSeries[DBOnlineSeries.cPrettyName].ToString().Trim().Equals(nameToSearch.Trim().ToLower(), StringComparison.InvariantCultureIgnoreCase)))
                        {
                            MPTVSeriesLog.Write(string.Format("\"{0}\" was automatically matched to \"{1}\" (SeriesID: {2}), there were a total of {3} matches returned from the Online Database", nameToSearch, onlineSeries.ToString(), onlineSeries[DBOnlineSeries.cID], GetSeriesParser.Results.Count));
                            return onlineSeries;
                        }
                    }
                }
                MPTVSeriesLog.Write(string.Format("Found {0} possible matches for \"{1}\"", GetSeriesParser.Results.Count, nameToSearch));

                // User has four choices:
                // 1) Pick a series from the list
                // 2) Simply skip
                // 3) Skip and never ask for this series again
                // 4) Manually Search

                List<Feedback.CItem> Choices = new List<Feedback.CItem>();
                Dictionary<int, DBOnlineSeries> uniqueSeriesIds = new Dictionary<int, DBOnlineSeries>();
                foreach (DBOnlineSeries onlineSeries in GetSeriesParser.Results) // make them unique (each seriesID) - if possible in users lang
                {
                    if (!uniqueSeriesIds.ContainsKey(onlineSeries[DBOnlineSeries.cID]))
                        uniqueSeriesIds.Add(onlineSeries[DBOnlineSeries.cID], onlineSeries);
                    else if (onlineSeries["language"] == Online_Parsing_Classes.OnlineAPI.SelLanguageAsString)
                        uniqueSeriesIds[onlineSeries[DBOnlineSeries.cID]] = onlineSeries;
                }
                foreach (KeyValuePair<int, DBOnlineSeries> onlineSeries in uniqueSeriesIds)
                {
                   Choices.Add(new Feedback.CItem(onlineSeries.Value[DBOnlineSeries.cPrettyName], 
                       "SeriesID: " + onlineSeries.Value[DBOnlineSeries.cID] + Environment.NewLine +
                       onlineSeries.Value[DBOnlineSeries.cSummary], 
                       onlineSeries.Value));
                }

                if (Choices.Count == 0)
                    Choices.Add(new Feedback.CItem("No Match Found, try to enter another name for the show", String.Empty, null));

                Feedback.ChooseFromSelectionDescriptor descriptor = new Feedback.ChooseFromSelectionDescriptor();
                descriptor.m_sTitle = "Unable to find matching series";
                descriptor.m_sItemToMatchLabel = "Local series:";
                descriptor.m_sItemToMatch = nameToSearch;
                descriptor.m_sListLabel = "Choose the correct series from this list:";
                descriptor.m_List = Choices;
                descriptor.m_sbtnCancelLabel = "&Skip";
                descriptor.m_sbtnIgnoreLabel = "Skip &Always";
                
                bool bKeepTrying = true;
                while (bKeepTrying)
                {
                    Feedback.CItem Selected = null;
                    Feedback.ReturnCode result = m_feedback.ChooseFromSelection(descriptor, out Selected);
                    switch (result)
                    {
                        case Feedback.ReturnCode.Cancel:
                            MPTVSeriesLog.Write("User cancelled Series Selection");
                            return null;

                        case Feedback.ReturnCode.Ignore:
                            MPTVSeriesLog.Write("User chose to Ignore \"" + nameToSearch + "\" in the future");
                            nameToSearch = null;
                            DBSeries series = new DBSeries(seriesName);
                            series[DBSeries.cScanIgnore] = 1; // means it will be skipped in the future
                            series[DBSeries.cHidden] = true;
                            series.Commit();
                            return null;

                        case Feedback.ReturnCode.OK:
                            DBOnlineSeries selectedSeries = Selected.m_Tag as DBOnlineSeries;

                            // Show the Virtual Keyboard to manual enter in name to search
                            // For use with-in Media Portal only, config already allow you to re-enter search.
                            /*if (selectedSeries == null && !Settings.isConfig)
                            {                                
                                VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);                                
                                if (null == keyboard)
                                    return null;
                                keyboard.Reset();
                                keyboard.Text = nameToSearch;                          
                                keyboard.DoModal(GUIWindowManager.ActiveWindow);
                                
                                // TODO
                                if (keyboard.IsConfirmed)
                                {
                                    nameToSearch = keyboard.Text;
                                    bKeepTrying = false;
                                }
                                else
                                    return null;
                            }*/

                            if (nameToSearch != Selected.m_sName || selectedSeries == null)
                            {
                                nameToSearch = Selected.m_sName;
                                bKeepTrying = false;
                            }
                            else
                            {
                                MPTVSeriesLog.Write(string.Format("User matched \"{0}\" to \"{1}\" (SeriesID: {2})", nameToSearch, selectedSeries.ToString(), selectedSeries[DBOnlineSeries.cID]));
                                return selectedSeries;
                            }
                            break;

                        case Feedback.ReturnCode.NotReady:
                            {
                                // plugin's not loaded (yet?) so wait and ask again later
                                Thread.Sleep(2000);
                            }
                            break;
                    }
                }
                if (!bKeepTrying) MPTVSeriesLog.Write("User typed a new Search term: \"" + nameToSearch + "\"");
            }            
        }

        public void determineOrderOption(DBSeries series)
        {
            try
            {
                List<string> episodeOrders = new List<string>(series[DBOnlineSeries.cEpisodeOrders].ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
                if (episodeOrders.Count > 1)
                {
                    MPTVSeriesLog.Write(string.Format("\"{0}\" supports {1} different ordering options, asking user...", series.ToString(), episodeOrders.Count));
                    // let the user choose
                    string helpText = "Some series expose several ways in which they are ordered, for instance a DVD-release may differ from the original Air schedule." + Environment.NewLine +
                                      "Note that your file numbering must match the option you choose here." + Environment.NewLine +
                                      "Choose the default \"Aired\" option unless you have a specific reason not to!";

                    List<Feedback.CItem> Choices = new List<Feedback.CItem>();
                    foreach (string orderOption in episodeOrders)
                        Choices.Add(new Feedback.CItem(orderOption, helpText, orderOption));

                    Feedback.ChooseFromSelectionDescriptor descriptor = new Feedback.ChooseFromSelectionDescriptor();
                    descriptor.m_sTitle = "Multiple ordering Options detected";
                    descriptor.m_sItemToMatchLabel = "The following Series supports multiple Order Options:";
                    descriptor.m_sItemToMatch = series[DBOnlineSeries.cPrettyName];
                    descriptor.m_sListLabel = "Please choose the desired Option:";
                    descriptor.m_List = Choices;
                    descriptor.m_useRadioToSelect = true;
                    descriptor.m_allowAlter = false;

                    Feedback.CItem selectedOrdering = null;
                    Feedback.ReturnCode result = m_feedback.ChooseFromSelection(descriptor, out selectedOrdering);
                    if (result == WindowPlugins.GUITVSeries.Feedback.ReturnCode.OK)
                    {
                        series[DBOnlineSeries.cChoseEpisodeOrder] = (string)selectedOrdering.m_Tag;
                    }
                }

            }
            catch (Exception)
            {

            }
            // End support for ordering
        }

        public void UpdateSeries(bool bUpdateNewSeries, List<DBValue> seriesUpdated)
        {
            // now retrieve the info about the series
            SQLCondition condition = new SQLCondition();
            // all series that have an onlineID ( != 0)
            condition.Add(new DBOnlineSeries(), DBOnlineSeries.cID, 0, SQLConditionType.GreaterThan);
            condition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);
            if (bUpdateNewSeries)
            {
                MPTVSeriesLog.Write(bigLogMessage("Retrieving Metadata for new Series"));
                // and that never had data imported from the online DB
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cOnlineDataImported, 0, SQLConditionType.Equal);
            }
            else
            {
                MPTVSeriesLog.Write(bigLogMessage("Updating Metadata for existing Series"));
                // and that already had data imported from the online DB (but not the new ones, that are set to 2)
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cOnlineDataImported, 1, SQLConditionType.Equal);
                //nUpdateSeriesTimeStamp = (long)DBOption.GetOptions(DBOption.cUpdateSeriesTimeStamp);
            }
            List<DBSeries> SeriesList = DBSeries.Get(condition, false, false);

            if (!bUpdateNewSeries && SeriesList.Count > 0)
            {

                // let's check which of these we have any interest in
                for (int i = 0; i < SeriesList.Count; i++)
                    if (!seriesUpdated.Contains(SeriesList[i][DBSeries.cID]))
                    {
                        SeriesList.RemoveAt(i);
                        i--;
                    }
            }

            if (SeriesList.Count > 0)
            {
                MPTVSeriesLog.Write(string.Format("{0} metadata of {1} Series", (bUpdateNewSeries ? "Retrieving" : "Looking for updated"), SeriesList.Count));

                UpdateSeries UpdateSeriesParser = new UpdateSeries(generateIDListOfString(SeriesList, DBSeries.cID));

                if (UpdateSeriesParser.Results.Count == 0)
                    MPTVSeriesLog.Write(string.Format("No {0} found", (bUpdateNewSeries ? "metadata" : "updates")));

                foreach (DBOnlineSeries updatedSeries in UpdateSeriesParser.Results)
                {
                    m_bDataUpdated = true;
                    if (worker.CancellationPending)
                        return;

                    MPTVSeriesLog.Write(string.Format("Metadata {0} for \"{1}\"", (bUpdateNewSeries ? "retrieved" : "updated"), updatedSeries.ToString()));
                    // find the corresponding series in our list
                    foreach (DBSeries localSeries in SeriesList)
                    {
                        if (localSeries[DBSeries.cID] == updatedSeries[DBSeries.cID])
                        {
                            // go over all the fields, (and update only those which haven't been modified by the user - will do that later)
                            foreach (String key in updatedSeries.FieldNames)
                            {
                                switch (key)
                                {
                                    // do not overwrite current series local settings with the one from the online series (baaaad design??)
                                    case DBSeries.cParsedName: // this field shouldn't be required here since updatedSeries is an Onlineseries and not a localseries??
                                    case DBOnlineSeries.cHasLocalFiles:
                                    case DBOnlineSeries.cHasLocalFilesTemp:
                                    case DBOnlineSeries.cIsFavourite:                                    
                                    case DBOnlineSeries.cChoseEpisodeOrder:

                                    case DBOnlineSeries.cBannerFileNames: // banners get handled differently (later on)
                                    case DBOnlineSeries.cCurrentBannerFileName:
                                    case DBOnlineSeries.cMyRating:
                                        break;
                                    case DBOnlineSeries.cEpisodeOrders:
                                        if(bUpdateNewSeries) goto default;
                                        break;
                                    default:
                                        localSeries.AddColumn(key, new DBField(DBField.cTypeString));
                                        localSeries[key] = updatedSeries[key];
                                        break;
                                }
                            }

                            // diff. order options
                            if (bUpdateNewSeries) determineOrderOption(localSeries);                            

                            // data import completed; set to 2 (data up to date)
                            localSeries[DBOnlineSeries.cOnlineDataImported] = 2;

                            if (localSeries[DBOnlineSeries.cHasLocalFilesTemp])
                                localSeries[DBOnlineSeries.cHasLocalFiles] = 1;
                            localSeries.Commit();
                            //                        SeriesList.Remove(localSeries);
                        }
                    }
                }
            }
            else MPTVSeriesLog.Write("Nothing to do...");
        }

        public void GetEpisodes()
        {
            MPTVSeriesLog.Write(bigLogMessage("Get Episodes"));
            SQLCondition condition = null;
            if (m_params.m_bUpdateScan)
            {
                // mark existing online data as "old", needs a refresh
                condition = new SQLCondition();
                condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cOnlineDataImported, 2, SQLConditionType.Equal);
                DBEpisode.GlobalSet(DBOnlineEpisode.cOnlineDataImported, 1, condition);
            }

            condition = new SQLCondition();
            // all series that have an onlineID ( != 0)
            condition.Add(new DBSeries(), DBSeries.cID, 0, SQLConditionType.GreaterThan);
            condition.Add(new DBSeries(), DBSeries.cScanIgnore, 0, SQLConditionType.Equal);
            condition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);

            List<DBSeries> seriesList = DBSeries.Get(condition, false, false);

            if (m_bFullSeriesRetrieval && m_params.m_bUpdateScan)
                MPTVSeriesLog.Write("Mode: Get all Episodes of Series");

            foreach (DBSeries series in seriesList)
            {
                List<DBEpisode> episodesList = null;
                //if (!m_bFullSeriesRetrieval)
                //{
                    // lets get the list of unidentified series
                    SQLCondition conditions = new SQLCondition();
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cID, 0, SQLConditionType.Equal);
                    episodesList = DBEpisode.Get(conditions, false);
                //}
                if (m_bFullSeriesRetrieval || episodesList.Count > 0)
                {
                    GetEpisodes episodesParser = new GetEpisodes((string)series[DBSeries.cID]);
                    if (episodesParser.Results.Count > 0)
                    {
                        MPTVSeriesLog.Write(string.Format("Found {0} Episodes for \"{1}\", matching them up with local Episodes now", episodesParser.Results.Count, series.ToString()));
                        // look for the episodes for that series, and compare / update the values
                        matchOnlineToLocalEpisodes(series, episodesList, episodesParser);
                    }
                    if (m_bFullSeriesRetrieval)
                    {
                        // add all online episodes in the local db
                        System.Globalization.NumberFormatInfo provider = new System.Globalization.NumberFormatInfo();
                        provider.NumberDecimalSeparator = ".";
                        foreach (DBOnlineEpisode onlineEpisode in episodesParser.Results)
                        {
                            // only add episodes that have seaon/ep set in dvd-ordering mode
                            float onlineEp = -1;
                            float.TryParse(onlineEpisode["DVD_episodenumber"], System.Globalization.NumberStyles.AllowDecimalPoint, provider, out onlineEp);
                            float onlineSeason = -1;
                            float.TryParse(onlineEpisode["DVD_season"], System.Globalization.NumberStyles.AllowDecimalPoint, provider, out onlineSeason);
                            if (series[DBOnlineSeries.cChoseEpisodeOrder] == "DVD")
                            {
                                onlineEpisode[DBOnlineEpisode.cSeasonIndex] = onlineSeason;
                                onlineEpisode[DBOnlineEpisode.cEpisodeIndex] = onlineEp;
                            }

                            if (!(series[DBOnlineSeries.cChoseEpisodeOrder] == "DVD" && (onlineEp == -1 || onlineSeason == -1)))
                            {
                                // season if not there yet
                                DBSeason season = new DBSeason(series[DBSeries.cID], onlineEpisode[DBOnlineEpisode.cSeasonIndex]);
                                season[DBSeason.cHasEpisodes] = true;
                                DBSeason existing = DBSeason.getRaw(series[DBSeries.cID], onlineEpisode[DBOnlineEpisode.cSeasonIndex]);
                                if (existing != null)
                                {
                                    season[DBSeason.cHasLocalFiles] = existing[DBSeason.cHasLocalFilesTemp];
                                    season[DBSeason.cHasLocalFilesTemp] = existing[DBSeason.cHasLocalFilesTemp];
                                }
                                season.Commit();

                                DBOnlineEpisode newOnlineEpisode = new DBOnlineEpisode(series[DBSeries.cID], onlineEpisode[DBOnlineEpisode.cSeasonIndex], onlineEpisode[DBOnlineEpisode.cEpisodeIndex]);
                                newOnlineEpisode[DBOnlineEpisode.cID] = onlineEpisode[DBOnlineEpisode.cID];
                                if (newOnlineEpisode[DBOnlineEpisode.cEpisodeName].ToString().Length == 0)
                                    newOnlineEpisode[DBOnlineEpisode.cEpisodeName] = onlineEpisode[DBOnlineEpisode.cEpisodeName];
                                foreach (String key in onlineEpisode.FieldNames)
                                {
                                    switch (key)
                                    {
                                        case DBOnlineEpisode.cCompositeID:
                                        case DBEpisode.cSeriesID:
                                        case DBOnlineEpisode.cWatched:
                                        case DBOnlineEpisode.cHidden:
                                        case DBOnlineEpisode.cDownloadPending:
                                        case DBOnlineEpisode.cDownloadExpectedNames:
                                        case DBOnlineEpisode.cMyRating:
                                            // do nothing here, those information are local only
                                            break;

                                        case DBOnlineEpisode.cSeasonIndex:
                                        case DBOnlineEpisode.cEpisodeIndex:
                                            break; // those must not get overwritten from what they were set to by getEpisodes (because of different order options)

                                        default:
                                            newOnlineEpisode.AddColumn(key, new DBField(DBField.cTypeString));
                                            newOnlineEpisode[key] = onlineEpisode[key];
                                            break;
                                    }
                                }
                                newOnlineEpisode.Commit();
                            }
                        }
                    }
                    else MPTVSeriesLog.Write("None of these episodes could be identified (Possible reasons: Wrong filename/OnlineDatabase does not have these episodes)");
                }
            }
        }

        public void UpdateEpisodes(List<DBValue> episodesUpdated)
        {
            // let's check which series/episodes we have locally
            SQLCondition cond = new SQLCondition(new DBOnlineEpisode(), DBOnlineEpisode.cID, 0, SQLConditionType.GreaterThan);
            cond.AddOrderItem(DBEpisode.Q(DBEpisode.cSeriesID), SQLCondition.orderType.Ascending);

            List<DBEpisode> episodesInDB = DBEpisode.Get(cond);

            if (episodesUpdated == null || episodesUpdated.Count == 0) episodesInDB.Clear();

            // let's check which of these we have any interest in
            for (int i = 0; i < episodesInDB.Count; i++)
                if (!episodesUpdated.Contains(episodesInDB[i][DBOnlineEpisode.cID]))
                    episodesInDB.RemoveAt(i--);

            // let's updated those we are interested in
            // for the remaining ones get the <lang>.xml
            MPTVSeriesLog.Write(episodesInDB.Count.ToString() + " episodes need updating");
            int seriesID = 0;
            for(int i=0; i<episodesInDB.Count; i++)
            {
                if (seriesID != episodesInDB[i][DBEpisode.cSeriesID])
                {
                    seriesID = episodesInDB[i][DBEpisode.cSeriesID];
                    matchOnlineToLocalEpisodes(Helper.getCorrespondingSeries(episodesInDB[i][DBEpisode.cSeriesID]), episodesInDB, new GetEpisodes((string)episodesInDB[i][DBEpisode.cSeriesID]));
                }
            }
        }

        public void UpdateEpisodeThumbNails()
        {
            if (DBOption.GetOptions(DBOption.cGetEpisodeSnapshots) == true)
            {
                MPTVSeriesLog.Write(bigLogMessage("Checking for EpisodeThumbnails..."));
                // get a list of all the episodes with thumbnailUrl
                SQLCondition condition = new SQLCondition();
                condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cEpisodeThumbnailUrl, string.Empty, SQLConditionType.NotEqual);
                condition.AddOrderItem(DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID), SQLCondition.orderType.Ascending);
                List<DBEpisode> episodes = DBEpisode.Get(condition);
                DBSeries tmpSeries = null; 
                foreach (DBEpisode episode in episodes)
                {
                    String sThumbNailFilename = episode[DBOnlineEpisode.cEpisodeThumbnailFilename];
                    string basePath = Settings.GetPath(Settings.Path.banners);
                    string completePath = Helper.PathCombine(basePath, sThumbNailFilename);
                    if (!File.Exists(completePath))
                    {
                        // we need the pretty name to figure out the folder to store to
                        try
                        {
                            if (null == tmpSeries || tmpSeries[DBSeries.cID] != episode[DBEpisode.cSeriesID])
                            {
                                tmpSeries = Helper.getCorrespondingSeries(episode[DBOnlineEpisode.cSeriesID]);
                            }
                            string seriesFolder = tmpSeries[DBOnlineSeries.cPrettyName];
                            foreach (char c in System.IO.Path.GetInvalidFileNameChars()) seriesFolder = seriesFolder.Replace(c, '_');
                            sThumbNailFilename = Helper.PathCombine(seriesFolder, @"Episodes\" + episode[DBOnlineEpisode.cSeasonIndex] + "x" + episode[DBOnlineEpisode.cEpisodeIndex] + ".jpg");
                            completePath = Helper.PathCombine(basePath, sThumbNailFilename);

                            if (!File.Exists(completePath))
                            {
                                MPTVSeriesLog.Write(string.Format("New EpisodeImage found for \"{0}\": {1}", episode.ToString(), episode[DBOnlineEpisode.cEpisodeThumbnailUrl]));
                                System.Net.WebClient webClient = new System.Net.WebClient();
                                webClient.Headers.Add("user-agent", Settings.UserAgent);
                                try
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(completePath));
                                    string url = DBOnlineMirror.Banners + episode[DBOnlineEpisode.cEpisodeThumbnailUrl];
                                    webClient.DownloadFile(url, completePath);
                                }
                                catch (System.Net.WebException)
                                {
                                    MPTVSeriesLog.Write("Banner download failed (" + episode[DBOnlineEpisode.cEpisodeThumbnailUrl] + ")");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MPTVSeriesLog.Write("There was a problem getting the episode image: " + ex.Message);
                        }
                        episode[DBOnlineEpisode.cEpisodeThumbnailFilename] = sThumbNailFilename;
                        episode.Commit();
                    }
                }
            }

        }


        public void UpdateBanners(bool bUpdateNewSeries, List<DBValue> updatedSeries)
        {
            SQLCondition condition = new SQLCondition();
            // all series that have an onlineID ( > 0)
            condition.Add(new DBOnlineSeries(), DBSeries.cID, 0, SQLConditionType.GreaterThan);
            condition.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);
            if (bUpdateNewSeries)
            {
                MPTVSeriesLog.Write(bigLogMessage("Checking for banners for series without any banners"));
                // and that never had data imported from the online DB
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cBannerFileNames, string.Empty, SQLConditionType.Equal);
                condition.nextIsOr = true;
                condition.AddCustom(" exists (select * from season where seriesID = online_series.id and bannerfilenames != '')");
                condition.nextIsOr = false;
            }
            else
            {
                MPTVSeriesLog.Write(bigLogMessage("Checking for new banners"));                
                // and that already had data imported from the online DB
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cBannerFileNames, string.Empty, SQLConditionType.NotEqual);
            }

            List<DBSeries> seriesList = DBSeries.Get(condition, false, false);
            if (!bUpdateNewSeries && seriesList.Count > 0)
            {
                // let's check which of these we have any interest in
                for (int i = 0; i < seriesList.Count; i++)
                    if (!updatedSeries.Contains(seriesList[i][DBSeries.cID]))
                    {
                        seriesList.RemoveAt(i--);
                    }
            }


            int nIndex = 0;
            if (seriesList.Count == 0)
            {
                if (bUpdateNewSeries) MPTVSeriesLog.Write("All Series appear to have banners already");
                else MPTVSeriesLog.Write("Nothing to do");
            }
            else MPTVSeriesLog.Write("Looking for banners on " + seriesList.Count + " Series");

            foreach (DBSeries series in seriesList)
            {
                if (worker.CancellationPending)
                    return;
                nIndex++;
                MPTVSeriesLog.Write((bUpdateNewSeries ? "Downloading" : "Refreshing") + " banners for \"" + series.ToString() + "\"");

                GetBanner bannerParser = null;
                //if (bUpdateNewSeries)
                //{                    
                    bannerParser = new GetBanner((string)series[DBSeries.cID]);
                //}
                String sLastTextBanner = String.Empty;
                String sLastGraphicalBanner = String.Empty;

                //seriesBannersMap seriesBanners = Helper.getElementFromList<seriesBannersMap, string>(series[DBSeries.cID], "seriesID", 0, bUpdateNewSeries ? bannerParser.seriesBanners : preSeriesBanners);
                seriesBannersMap seriesBanners = Helper.getElementFromList<seriesBannersMap, string>(series[DBSeries.cID], "seriesID", 0, bannerParser.seriesBanners);
                if (seriesBanners != null)  // oops!
                {
                    bool hasOwnLang = false;
                    foreach (BannerSeries bannerSeries in seriesBanners.seriesBanners)
                    {
                        if (!series[DBOnlineSeries.cBannerFileNames].ToString().Contains(bannerSeries.sBannerFileName))
                        {
                            m_bDataUpdated = true;
                            MPTVSeriesLog.Write("New banner found for \"" + series.ToString() + "\" : " + bannerSeries.sOnlineBannerPath);
                            if (series[DBOnlineSeries.cBannerFileNames].ToString().Trim().Length == 0)
                            {
                                series[DBOnlineSeries.cBannerFileNames] += bannerSeries.sBannerFileName;
                            }
                            else
                            {
                                series[DBOnlineSeries.cBannerFileNames] += "|" + bannerSeries.sBannerFileName;
                            }
                        }
                        // prefer graphical
                        if (bannerSeries.sBannerLang == Online_Parsing_Classes.OnlineAPI.SelLanguageAsString)
                        {
                            if (bannerSeries.bGraphical)
                                sLastGraphicalBanner = bannerSeries.sBannerFileName;
                            else
                                sLastTextBanner = bannerSeries.sBannerFileName;
                            hasOwnLang = true;
                        }
                        else if(!hasOwnLang)
                        {
                            if (bannerSeries.bGraphical)
                                sLastGraphicalBanner = bannerSeries.sBannerFileName;
                            else
                                sLastTextBanner = bannerSeries.sBannerFileName;
                        }
                    }

                    if (series[DBOnlineSeries.cCurrentBannerFileName].ToString().Trim().Length == 0)
                    {
                        // use the last banner as the current one (if any graphical found)
                        // otherwise use the first available
                        if (sLastGraphicalBanner.Length > 0)
                            series[DBOnlineSeries.cCurrentBannerFileName] = sLastGraphicalBanner;
                        else
                            series[DBOnlineSeries.cCurrentBannerFileName] = sLastTextBanner;
                    }

                    //series[DBOnlineSeries.cBannersDownloaded] = 2;
                    series.Commit();
                    string lastSeasonBanner = string.Empty;
                    hasOwnLang = false;
                    foreach (BannerSeason bannerSeason in seriesBanners.seasonBanners)
                    {
                        DBSeason season = new DBSeason(series[DBSeries.cID], Int32.Parse(bannerSeason.sSeason));
                        if (season[DBSeason.cBannerFileNames].ToString().IndexOf(bannerSeason.sBannerFileName) == -1)
                        {
                            m_bDataUpdated = true;
                            if (season[DBSeason.cBannerFileNames].ToString().Length == 0)
                            {
                                season[DBSeason.cBannerFileNames] += bannerSeason.sBannerFileName;
                            }
                            else
                            {
                                season[DBSeason.cBannerFileNames] += "|" + bannerSeason.sBannerFileName;
                                MPTVSeriesLog.Write("New banner found for \"" + series.ToString() + "\" Season " + season[DBSeason.cIndex] + ": " + bannerSeason.sOnlineBannerPath);
                            }
                        }
     
                        if (bannerSeason.sBannerLang == Online_Parsing_Classes.OnlineAPI.SelLanguageAsString)
                        {
                            lastSeasonBanner = bannerSeason.sBannerFileName;
                            hasOwnLang = true;
                        }
                        else if(!hasOwnLang)
                        {
                            lastSeasonBanner = bannerSeason.sBannerFileName;
                        }
                        // use the last banner as the current one
                        if (season[DBSeason.cCurrentBannerFileName].ToString().Trim().Length == 0)
                            season[DBSeason.cCurrentBannerFileName] = lastSeasonBanner;
                        season.Commit();
                    }
                    
                    //if (!bUpdateNewSeries)
                    //{
                    //    // disable for new way
                    //    //series[DBOnlineSeries.cUpdateBannersTimeStamp] = bannerParser.ServerTimeStamp;
                    //    series.Commit();
                    //}
                    //if (!bUpdateNewSeries) DBOption.SetOptions(DBOption.cUpdateBannersTimeStamp, bannerParser.ServerTimeStamp);
                }
            }
        }

        private static bool matchOnlineToLocalEpisode(DBSeries series, DBEpisode localEpisode, DBOnlineEpisode onlineEpisode)
        {
            switch ((string)series[DBOnlineSeries.cChoseEpisodeOrder])
            {
                case "":
                case "Aired":
                    return ((int)localEpisode[DBEpisode.cSeasonIndex] == (int)onlineEpisode[DBOnlineEpisode.cSeasonIndex] &&
                            ((int)localEpisode[DBEpisode.cEpisodeIndex] == (int)onlineEpisode[DBOnlineEpisode.cEpisodeIndex] ||
                            (int)localEpisode[DBEpisode.cEpisodeIndex2] == (int)onlineEpisode[DBOnlineEpisode.cEpisodeIndex]));
                case "DVD":
                    System.Globalization.NumberFormatInfo provider = new System.Globalization.NumberFormatInfo();
                    provider.NumberDecimalSeparator = ".";
                    int localSeason = (int)localEpisode[DBEpisode.cSeasonIndex];
                    float onlineSeasonTemp;
                    int onlineSeason = -1; 
                    if( float.TryParse(onlineEpisode["DVD_season"], System.Globalization.NumberStyles.AllowDecimalPoint, provider, out onlineSeasonTemp))
                        onlineSeason = (int)onlineSeasonTemp;
                    int localEp = (int)localEpisode[DBEpisode.cEpisodeIndex];
                    int localEp2 = (int)localEpisode[DBEpisode.cEpisodeIndex2];
                    if (Helper.String.IsNullOrEmpty(localEpisode[DBEpisode.cEpisodeIndex2])) localEp2 = -1;
                    float onlineEp = -1;
                    if (onlineSeason != -1 && float.TryParse(onlineEpisode["DVD_episodenumber"], System.Globalization.NumberStyles.AllowDecimalPoint, provider, out onlineEp))
                    {
                        if (!Helper.String.IsNullOrEmpty(onlineEpisode["DVD_season"]) && !Helper.String.IsNullOrEmpty(onlineEpisode["DVD_season"]) &&
                            (localSeason == onlineSeason && (localEp == onlineEp || localEp2 == -1 ? false : localEp2 == onlineEp)))
                        {
                            // overwrite onlineEps season/ep #
                            onlineEpisode[DBOnlineEpisode.cSeasonIndex] = (int)localEpisode[DBEpisode.cSeasonIndex];
                            if (localEp == onlineEp)
                                onlineEpisode[DBEpisode.cEpisodeIndex] = localEp;
                            else
                                onlineEpisode[DBEpisode.cEpisodeIndex] = localEp2;
                            return true;
                        }
                    } break;
                //case "Absolute":
                //    if ( !Helper.String.IsNullOrEmpty(onlineEpisode["absolute_number"]) &&
                //        ( (int)localEpisode[DBEpisode.cSeasonIndex] == 1 &&
                //        ((int)localEpisode[DBEpisode.cEpisodeIndex] == (int)onlineEpisode["absolute_number"] ||
                //        (int)localEpisode[DBEpisode.cEpisodeIndex2] == (int)onlineEpisode["absolute_number"])))
                //    {
                //        // overwrite onlineEps season/ep #
                //        onlineEpisode[DBOnlineEpisode.cSeasonIndex] = (int)localEpisode[DBEpisode.cSeasonIndex];
                //        if ((int)localEpisode[DBEpisode.cEpisodeIndex] == (int)onlineEpisode["absolute_number"])
                //            onlineEpisode[DBEpisode.cEpisodeIndex] = (int)localEpisode[DBEpisode.cEpisodeIndex];
                //        else
                //            onlineEpisode[DBEpisode.cEpisodeIndex] = (int)localEpisode[DBEpisode.cEpisodeIndex2];
                //        return true;
                //    } break;
            }
            return false;
        }

        private static void matchOnlineToLocalEpisodes(DBSeries series, List<DBEpisode> episodesList, GetEpisodes episodesParser)
        {
            if (episodesList == null || episodesList.Count == 0) return;
            foreach (DBEpisode localEpisode in episodesList)
            {
                foreach (DBOnlineEpisode onlineEpisode in episodesParser.Results)
                {                
                    if ((int)localEpisode[DBEpisode.cSeriesID] == (int)onlineEpisode[DBOnlineEpisode.cSeriesID])
                    {
                        if (matchOnlineToLocalEpisode(series, localEpisode, onlineEpisode))
                        {
                            //if (localEpisode[DBEpisode.cSeriesID] == 73545 && localEpisode[DBEpisode.cSeasonIndex] == 0 && localEpisode[DBEpisode.cEpisodeIndex] == 5)
                            //    MPTVSeriesLog.Write(localEpisode.ToString() + " hidden flag: " + localEpisode[DBOnlineEpisode.cHidden].ToString());

                            // season update for online data
                            DBSeason season = new DBSeason(series[DBSeries.cID], onlineEpisode[DBOnlineEpisode.cSeasonIndex]);
                            season[DBSeason.cHasEpisodes] = true;
                            season.Commit();

                            // update data
                            localEpisode[DBOnlineEpisode.cID] = onlineEpisode[DBOnlineEpisode.cID];
                            if (localEpisode[DBOnlineEpisode.cEpisodeName].ToString().Length == 0)
                                localEpisode[DBOnlineEpisode.cEpisodeName] = onlineEpisode[DBOnlineEpisode.cEpisodeName];

                            // go over all the fields, (and update only those which haven't been modified by the user - will do that later)
                            foreach (String key in onlineEpisode.FieldNames)
                            {
                                switch (key)
                                {
                                    case DBOnlineEpisode.cCompositeID:
                                    case DBEpisode.cSeriesID:
                                    case DBOnlineEpisode.cWatched:
                                    case DBOnlineEpisode.cHidden:
                                    case DBOnlineEpisode.cDownloadPending:
                                    case DBOnlineEpisode.cDownloadExpectedNames:
                                    case DBOnlineEpisode.cMyRating:
                                        // do nothing here, those information are local only
                                        break;

                                    case DBOnlineEpisode.cSeasonIndex:
                                    case DBOnlineEpisode.cEpisodeIndex:
                                        break; // those must not get overwritten from what they were set to by getEpisodes (because of different order options)

                                    default:
                                        localEpisode.onlineEpisode.AddColumn(key, new DBField(DBField.cTypeString));
                                        localEpisode[key] = onlineEpisode[key];
                                        break;
                                }
                            }
                            localEpisode[DBOnlineEpisode.cOnlineDataImported] = 1;
                            MPTVSeriesLog.Write("\"" + localEpisode.ToString() + "\" identified");
                            localEpisode.Commit();
                            break;
                        }

                    }
                }
            }
        }
        /*
        static string generateIDList<T>(List<T> entities, string fieldname) where T:DBTable
        {
            // generate a comma separated list of all the ids
            String sSeriesIDs = String.Empty;
            if (entities.Count > 0)
            {
                foreach (DBTable entity in entities)
                {
                    if (sSeriesIDs.Length > 0)
                        sSeriesIDs += ",";
                    sSeriesIDs += entity[fieldname];
                }
            }
            return sSeriesIDs;
        }
        */
        static List<string> generateIDListOfString<T>(List<T> entities, string fieldname) where T : DBTable
        {
            // generate a comma separated list of all the ids
            List<String> sSeriesIDs = new List<String>(entities.Count);
            if (entities.Count > 0)
                foreach (DBTable entity in entities)
                    sSeriesIDs.Add(entity[fieldname].ToString());
            return sSeriesIDs;
        }

        static string bigLogMessage(string msg)
        {
            return string.Format("***************     {0}     ***************", msg);
        }

        static string prettyStars(int lenght)
        {
            StringBuilder b = new StringBuilder(lenght);
            for (int i = 0; i < lenght; i++) b.Append('*');
            return b.ToString();
        }
    }
}
