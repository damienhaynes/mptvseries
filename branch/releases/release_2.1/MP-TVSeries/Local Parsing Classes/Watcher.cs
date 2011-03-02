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
using System.Threading;

namespace WindowPlugins.GUITVSeries
{
    enum WatcherItemType
    {
        Added,
        Deleted
    }

    class WatcherItem
    {
        public String m_sFullPathFileName;
        public String m_sParsedFileName;
        public WatcherItemType m_type;

        public WatcherItem(FileSystemWatcher watcher, RenamedEventArgs e, bool bOldName)
        {
            if (bOldName)
            {
                m_sFullPathFileName = e.OldFullPath;
                m_sParsedFileName = m_sFullPathFileName.Substring(watcher.Path.Length).TrimStart('\\');
                m_type = WatcherItemType.Deleted;
                MPTVSeriesLog.Write("File monitor: " + m_sParsedFileName + " " + m_type);
            }
            else
            {
                m_sFullPathFileName = e.FullPath;
                m_sParsedFileName = m_sFullPathFileName.Substring(watcher.Path.Length).TrimStart('\\');
                m_type = WatcherItemType.Added;
                MPTVSeriesLog.Write("File monitor: " + m_sParsedFileName + " " + m_type);
            }
        }

        public WatcherItem(FileSystemWatcher watcher, FileSystemEventArgs e)
        {
            m_sFullPathFileName = e.FullPath;
            m_sParsedFileName = m_sFullPathFileName.Substring(watcher.Path.Length).TrimStart('\\');
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Deleted:
                    m_type = WatcherItemType.Deleted;
                    break;

                default:
                    m_type = WatcherItemType.Added;
                    break;
            }
            MPTVSeriesLog.Write("File monitor: " + m_sParsedFileName + " " + m_type);
        }
    };

    class Watcher
    {
        public BackgroundWorker worker = new BackgroundWorker();
        List<String> m_WatchedFolders;
        List<System.IO.FileSystemWatcher> m_watchersList = new List<System.IO.FileSystemWatcher>();
        List<WatcherItem> m_modifiedFilesList = new List<WatcherItem>();
        bool progressUpdateRequired = false;
        public delegate void WatcherProgressHandler(int nProgress, List<WatcherItem> modifiedFilesList);

        /// <summary>
        /// This will be triggered once all the SeriesAndEpisodeInfo has been parsed completely.
        /// </summary>
        public event WatcherProgressHandler WatcherProgress;

        public Watcher(List<String> WatchedFolders)
        {
            m_WatchedFolders = WatchedFolders;
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.DoWork += new DoWorkEventHandler(workerWatcher_DoWork);
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (WatcherProgress != null) // only if any subscribers exist
                WatcherProgress.Invoke(e.ProgressPercentage, e.UserState as List<WatcherItem>);
        }

        public void StartFolderWatch()
        {
            // start the thread that is going to handle the addition in the db when files change
            worker.RunWorkerAsync();
        }

        void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            // rename: delete the old, add the new
            lock (m_modifiedFilesList)
            {
                String sOldExtention = System.IO.Path.GetExtension(e.OldFullPath);
                if (MediaPortal.Util.Utils.VideoExtensions.IndexOf(sOldExtention) != -1)
                    m_modifiedFilesList.Add(new WatcherItem(sender as FileSystemWatcher, e, true));
                String sNewExtention = System.IO.Path.GetExtension(e.FullPath);
                if (MediaPortal.Util.Utils.VideoExtensions.IndexOf(sNewExtention) != -1)
                    m_modifiedFilesList.Add(new WatcherItem(sender as FileSystemWatcher, e, false));
            }
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            // a file has changed! created, not created, whatever. Just add it to our list. We only process this list once in a while
            lock (m_modifiedFilesList)
            {
                foreach (WatcherItem item in m_modifiedFilesList)
                {
                    if (item.m_sFullPathFileName == e.FullPath)
                        return;
                }

                m_modifiedFilesList.Add(new WatcherItem(sender as FileSystemWatcher, e));
                progressUpdateRequired = true; // signal the worker thread (not using events because we don't want to react immediatly but only every couple of seconds at the most
            }
        }

        void setUpWatches()
        {
            if (m_watchersList.Count > 0) return; // this can only run once
            // ok let's see ... go through all enable import folders, and add a watchfolder on it
            foreach (String sWatchedFolder in m_WatchedFolders)
            {
                if (Directory.Exists(sWatchedFolder))
                {
                    // one watcher for each extension type
                    foreach (String extention in MediaPortal.Util.Utils.VideoExtensions)
                    {
                        FileSystemWatcher watcher = new FileSystemWatcher();
                        watcher.Filter = "*" + extention;
                        watcher.Path = sWatchedFolder;
                        watcher.IncludeSubdirectories = true;
                        watcher.NotifyFilter = NotifyFilters.FileName;
                        // Inker, I don't think lastwrite is such as good idea if you have your download/recording dir monitored
                        // only check for lastwrite .. I believe that's the only thing we're interested in
                        watcher.Changed += new FileSystemEventHandler(watcher_Changed);
                        watcher.Created += new FileSystemEventHandler(watcher_Changed);
                        watcher.Deleted += new FileSystemEventHandler(watcher_Changed);
                        watcher.Renamed += new RenamedEventHandler(watcher_Renamed);
                        watcher.EnableRaisingEvents = true;
                        m_watchersList.Add(watcher);
                    }
                }
            }
        }

        void signalActionRequired()
        {
            try
            {
                List<WatcherItem> outList = new List<WatcherItem>();
                lock (m_modifiedFilesList)
                {
                    // go over the modified files list once in a while & update
                    outList.AddRange(m_modifiedFilesList);
                    m_modifiedFilesList.Clear();
                    if (outList.Count > 0)
                        worker.ReportProgress(0, outList);
                }
                outList = null;
            }
            catch (Exception exp)
            {
                MPTVSeriesLog.Write("Exception happened in workerWatcher_DoWork: " + exp.Message);
            }
        }

        void workerWatcher_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            setUpWatches();
            while (!worker.CancellationPending)
            {
                if (progressUpdateRequired)
                {
                    signalActionRequired();
                }
                // wait
                Thread.Sleep(3000); // every 3 seconds do a quick check if we need to do something
            }
        }
    };

}

