using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace WindowPlugins.GUITVSeries.Torrent
{
    class TorrentLoad
    {
        DBTorrentSearch m_torrentSearch = null;
        DBEpisode m_dbEpisode = null;
        public BackgroundWorker worker = null;
        Feedback.Interface m_feedback = null;
        bool m_bSuccess = false;

        public delegate void TorrentLoadCompletedHandler(bool bOK);
        /// <summary>
        /// This will be triggered once all the SeriesAndEpisodeInfo has been parsed completely.
        /// </summary>
        public event TorrentLoadCompletedHandler TorrentLoadCompleted;

        public TorrentLoad(Feedback.Interface feedback)
        {
            m_feedback = feedback;
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (TorrentLoadCompleted != null) // only if any subscribers exist
            {
                this.TorrentLoadCompleted.Invoke(m_bSuccess);
            }
        }

        public bool Search(DBEpisode dbEpisode)
        {
            List<Feedback.CItem> Choices = new List<Feedback.CItem>();
            foreach (DBTorrentSearch torrentSearch in DBTorrentSearch.Get())
                Choices.Add(new Feedback.CItem(torrentSearch[DBTorrentSearch.cID], String.Empty, torrentSearch));

            Feedback.CDescriptor descriptor = new Feedback.CDescriptor();
            descriptor.m_sTitle = "Choose search site:";
            descriptor.m_sItemToMatchLabel = "";
            descriptor.m_sItemToMatch = "";
            descriptor.m_sListLabel = "Lit of search sites:";
            descriptor.m_List = Choices;
            descriptor.m_sbtnIgnoreLabel = String.Empty;

            Feedback.CItem Selected = null;
            if (m_feedback.ChooseFromSelection(descriptor, out Selected) == Feedback.ReturnCode.OK)
            {
                m_torrentSearch = Selected.m_Tag as DBTorrentSearch;
                if (m_torrentSearch[DBTorrentSearch.cSearchUrl] != String.Empty && System.IO.File.Exists(DBOption.GetOptions(DBOption.cUTorrentPath)))
                {
                    m_dbEpisode = dbEpisode;
                    worker.RunWorkerAsync();
                    return true;
                }
            }
            return false;
        }

        public void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // come up with a valid series name (remove some things basically)
            MPTVSeriesLog.Log_Write("**********************************");
            MPTVSeriesLog.Log_Write("Starting Torrentsearch process");
            MPTVSeriesLog.Log_Write("**********************************");

            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
            try
            {
                DBOnlineSeries series = new DBOnlineSeries(m_dbEpisode[DBEpisode.cSeriesID]);
                String sSeries = series[DBOnlineSeries.cPrettyName];
                // remove anything between parenthesis
                String RegExp = "(\\([^)]*\\))";
                Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
                sSeries = Engine.Replace(sSeries, "").Trim();

                List<TorrentResult> sortedMatchList = new List<TorrentResult>();
                int nCount = 0;
                while (nCount < 3)
                {
                    String sSearch = String.Empty;
                    switch (nCount)
                    {
                        case 0:
                            sSearch = String.Format("{0} S{1:D2}E{2:D2}", sSeries, (int)m_dbEpisode[DBEpisode.cSeasonIndex], (int)m_dbEpisode[DBEpisode.cEpisodeIndex]);
                            break;

                        case 1:
                            sSearch = String.Format("{0} {1}x{2:D2}", sSeries, (int)m_dbEpisode[DBEpisode.cSeasonIndex], (int)m_dbEpisode[DBEpisode.cEpisodeIndex]);
                            break;

                        case 2:
                            sSearch = String.Format("{0} {1}{2:D2}", sSeries, (int)m_dbEpisode[DBEpisode.cSeasonIndex], (int)m_dbEpisode[DBEpisode.cEpisodeIndex]);
                            break;
                    }
                    nCount++;
                    sSearch = sSearch.Replace(' ', '+');
                    RegExp = "\\$search\\$";
                    Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
                    String sUrl = Engine.Replace(m_torrentSearch[DBTorrentSearch.cSearchUrl], sSearch);

                    WebClient client = new WebClient();
                    Stream data = client.OpenRead(sUrl);
                    StreamReader reader = new StreamReader(data);
                    String sPage = reader.ReadToEnd();
                    data.Close();
                    reader.Close();

                    RegExp = m_torrentSearch[DBTorrentSearch.cSearchRegex];
                    Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
                    MatchCollection matches = Engine.Matches(sPage);
                    foreach (Match match in matches)
                    {
                        RegExp = "\\$id\\$";
                        Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
                        sUrl = Engine.Replace(m_torrentSearch[DBTorrentSearch.cDetailsUrl], match.Groups["id"].Value);

                        // go to detail page, and look for the number of files
                        client = new WebClient();
                        data = client.OpenRead(sUrl);
                        reader = new StreamReader(data);
                        sPage = reader.ReadToEnd();
                        data.Close();
                        reader.Close();

                        // and extract the number of files
                        RegExp = m_torrentSearch[DBTorrentSearch.cDetailsRegex];
                        Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
                        Match matchDetails = Engine.Match(sPage);
                        if (matchDetails.Success)
                        {
                            if (Convert.ToInt32(matchDetails.Groups[1].Value) <= 2) // consider a possible nfo file along with the avi?
                                sortedMatchList.Add(new TorrentResult(match));
                        }
                    }
                }

                sortedMatchList.Sort();

                // show the user the list and ask for the right one
                List<Feedback.CItem> Choices = new List<Feedback.CItem>();
                foreach (TorrentResult match in sortedMatchList)
                {
                    Choices.Add(new Feedback.CItem(match.m_sName + " (" + match.m_nSeeds + " / " + match.m_nLeechers + ")", String.Empty, match));
                }
                Feedback.CDescriptor descriptor = new Feedback.CDescriptor();
                descriptor.m_sTitle = "Found torrents:";
                descriptor.m_sItemToMatchLabel = "Was looking for episode:";
                descriptor.m_sItemToMatch = String.Format("{0} {1}x{2:D2}", sSeries, m_dbEpisode[DBEpisode.cSeasonIndex], m_dbEpisode[DBEpisode.cEpisodeIndex]);
                descriptor.m_sListLabel = "Found torrents:";
                descriptor.m_List = Choices;
                descriptor.m_sbtnIgnoreLabel = String.Empty;

                Feedback.CItem Selected = null;
                if (m_feedback.ChooseFromSelection(descriptor, out Selected) == Feedback.ReturnCode.OK)
                {
                    TorrentResult result = Selected.m_Tag as TorrentResult;
                    // download the torrent somewhere
                    String sRootServer = m_torrentSearch[DBTorrentSearch.cSearchUrl];
                    RegExp = "http://[^/]*";
                    Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
                    sRootServer = Engine.Match(sRootServer).Value;
                    WebClient client = new WebClient();
                    client.DownloadFile(sRootServer + result.m_sLink, System.IO.Path.GetTempPath() + "MPTVSeries.torrent");

                    // ok, we have the torrent link, we just need ... a folder
                    List<DBEpisode> SeriesEpisodes = DBEpisode.Get(m_dbEpisode[DBEpisode.cSeriesID], true);
                    if (SeriesEpisodes.Count > 0)
                    {
                        String sDirectory = System.IO.Path.GetDirectoryName(SeriesEpisodes[0][DBEpisode.cFilename]);

                        System.Diagnostics.Process.Start(DBOption.GetOptions(DBOption.cUTorrentPath), "/directory \"" + sDirectory + "\" \"" + System.IO.Path.GetTempPath() + "MPTVSeries.torrent\" /MINIMIZED");
                        m_bSuccess = true;
                    }
                    else
                    {
                        // ah, no existing series yet... doh. Create one based on the first import folder
                        DBImportPath[] paths = DBImportPath.GetAll();
                        if (paths.Length > 0)
                        {
                            String sDirectory = paths[0][DBImportPath.cPath] + "\\" + series[DBOnlineSeries.cPrettyName];
                            System.IO.Directory.CreateDirectory(sDirectory);
                            System.Diagnostics.Process.Start(DBOption.GetOptions(DBOption.cUTorrentPath), "/directory \"" + sDirectory + "\" \"" + System.IO.Path.GetTempPath() + "MPTVSeries.torrent\" /MINIMIZED");
                            m_bSuccess = true;
                        }
                    }
                }
            }
            catch 
            {

            }
        }
    }

    class TorrentResult : IComparable<TorrentResult>
    {
        public String m_sName;
        public String m_sLink;
        public int m_nSeeds;
        public int m_nLeechers;

        public int CompareTo(TorrentResult other)
        {
            return other.m_nSeeds.CompareTo(m_nSeeds);
        }

        public TorrentResult(Match match)
        {
            m_sName = match.Groups["name"].Value;
            m_sLink = match.Groups["link"].Value;
            m_nSeeds = Convert.ToInt32(match.Groups["seeds"].Value);
            m_nLeechers = Convert.ToInt32(match.Groups["leechers"].Value);
        }
    };
}
