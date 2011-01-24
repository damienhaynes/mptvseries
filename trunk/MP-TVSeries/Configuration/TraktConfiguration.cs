using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Trakt;
using Trakt.Show;
using Trakt.User;

namespace WindowPlugins.GUITVSeries.Configuration
{
    public partial class TraktConfiguration : UserControl
    {
        private const string cButtonManualSync = "Manual Synchronize";
        private const string cButtonCancelSync = "Cancel";
        private const string cButtonFinishSync = "Finished";

        private BackgroundWorker bgTraktSync = null;

        public TraktConfiguration()
        {
            InitializeComponent();
            LoadFromDB();

            // register text change event after loading password field as we dont want to re-hash
            this.textBoxPassword.TextChanged += new System.EventHandler(this.textBoxPassword_TextChanged);
        }

        public void EnableSyncButtonState(bool enable)
        {            
            this.buttonManualSync.Enabled = enable;
        }

        public void CancelSynchronization()
        {
            if (bgTraktSync != null && bgTraktSync.IsBusy)
                bgTraktSync.CancelAsync();
        }

        private void LoadFromDB()
        {
            textBoxUsername.Text = DBOption.GetOptions(DBOption.cTraktUsername);
            textBoxPassword.Text = DBOption.GetOptions(DBOption.cTraktPassword);

            TraktAPI.UserAgent = Settings.UserAgent;
            TraktAPI.Username = DBOption.GetOptions(DBOption.cTraktUsername);
            TraktAPI.Password = DBOption.GetOptions(DBOption.cTraktPassword);
        }

        private void textBoxUsername_TextChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cTraktUsername, textBoxUsername.Text);
            TraktAPI.Username = DBOption.GetOptions(DBOption.cTraktUsername);
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            // Hash Password
            DBOption.SetOptions(DBOption.cTraktPassword, textBoxPassword.Text.ToSHA1Hash());
            TraktAPI.Password = DBOption.GetOptions(DBOption.cTraktPassword);
        }

        private void textBoxPassword_Enter(object sender, EventArgs e)
        {
            // clear password field so can be re-entered easily, when re-entering config
            // it wont look like original because its hashed, so it's less confusing if cleared
            textBoxPassword.Text = string.Empty;
        }

        private void linkLabelSignUp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://trakt.tv");
        }

        private void buttonSeriesIgnore_Click(object sender, EventArgs e)
        {
            SeriesSelect SeriesSelectDlg = new SeriesSelect();

            // Get list of series in view
            SQLCondition conditions = new SQLCondition();
            conditions.Add(new DBOnlineSeries(), DBOnlineSeries.cTraktIgnore, 1, SQLConditionType.Equal);            
            SeriesSelectDlg.CheckedItems = DBSeries.Get(conditions);

            // Get list of series not in view
            conditions = new SQLCondition();
            conditions.Add(new DBOnlineSeries(), DBOnlineSeries.cTraktIgnore, 1, SQLConditionType.NotEqual);            
            SeriesSelectDlg.UnCheckedItems = DBSeries.Get(conditions);

            // Show series list dialog
            DialogResult result = SeriesSelectDlg.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                foreach (DBSeries series in SeriesSelectDlg.CheckedItems)
                {
                    // ignore these series
                    series[DBOnlineSeries.cTraktIgnore] = 1;
                    series.Commit();
                }

                foreach (DBSeries series in SeriesSelectDlg.UnCheckedItems)
                {
                    // unignore these series
                    series[DBOnlineSeries.cTraktIgnore] = 0;
                    series.Commit();
                }
            }
        }

        private void linkReSync_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // clear all flags for traktSeen and traktLibrary so they will re-sent on next synchronize
            DBOnlineEpisode.GlobalSet(new DBOnlineEpisode(), DBOnlineEpisode.cTraktLibrary, 0, new SQLCondition());
            DBOnlineEpisode.GlobalSet(new DBOnlineEpisode(), DBOnlineEpisode.cTraktSeen, 0, new SQLCondition());
            MPTVSeriesLog.Write("trakt flags reset in database, library will be re-sent on next synchronize");
        }

        private void buttonManualSync_Click(object sender, EventArgs e)
        {
            // if we click on the button again, we cancel
            if (bgTraktSync != null && bgTraktSync.IsBusy)
            {
                bgTraktSync.CancelAsync();
                return;
            }

            // if we have previously finished, then just reset
            if (buttonManualSync.Text == cButtonFinishSync)
            {
                buttonManualSync.Text = cButtonManualSync;
                progressTraktSync.Value = 0;
                return;
            }

            bgTraktSync = new BackgroundWorker();
            bgTraktSync.DoWork += new DoWorkEventHandler(bgTraktSync_DoWork);
            bgTraktSync.ProgressChanged +=new ProgressChangedEventHandler(bgTraktSync_ProgressChanged);
            bgTraktSync.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(bgTraktSync_RunWorkerCompleted);
            bgTraktSync.WorkerReportsProgress = true;
            bgTraktSync.WorkerSupportsCancellation = true;

            // synchronize
            bgTraktSync.RunWorkerAsync();
        }

        #region Trakt Synchronize

        private void bgTraktSync_DoWork(object sender, DoWorkEventArgs e)
        {
            MPTVSeriesLog.Write("Trakt: Synchronize Start");

            buttonManualSync.Text = cButtonCancelSync;
            ConfigurationForm.GetInstance().EnableImportButtonState(false);

            List<DBSeries> seriesList = DBSeries.Get(new SQLCondition());
       
            int progress = 0;
            
            foreach (DBSeries series in seriesList)
            {
                if (bgTraktSync.CancellationPending) return;

                if (series[DBSeries.cID] <= 0) continue;

                List<DBEpisode> episodesUnSeen = TraktHandler.GetEpisodesToSync(series, TraktSyncModes.unseen);
                List<DBEpisode> episodesLibrary = TraktHandler.GetEpisodesToSync(series, TraktSyncModes.library);
                List<DBEpisode> episodesSeen = TraktHandler.GetEpisodesToSync(series, TraktSyncModes.seen);

                // remove any seen episodes from library episode list as 'seen' counts as being part of the library
                // dont want to hit the server unnecessarily
                episodesLibrary.RemoveAll(eps => episodesSeen.Contains(eps));

                // sync UnSeen
                TraktHandler.SynchronizeLibrary(episodesUnSeen, TraktSyncModes.unseen);

                // sync library
                TraktHandler.SynchronizeLibrary(episodesLibrary, TraktSyncModes.library);

                // sync Seen
                TraktHandler.SynchronizeLibrary(episodesSeen, TraktSyncModes.seen);

                int percentage = Convert.ToInt32((double)(100 * progress++) / seriesList.Count());
                bgTraktSync.ReportProgress(percentage);
            }

            MPTVSeriesLog.Write("Trakt: Synchronize Complete");
        }

        private void bgTraktSync_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressTraktSync.Value = e.ProgressPercentage;
        }

        private void bgTraktSync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressTraktSync.Value = 100;
            buttonManualSync.Text = cButtonFinishSync;
            ConfigurationForm.GetInstance().EnableImportButtonState(true);
        }

        #endregion

        #region testing
        private void buttonTestAPI_Click(object sender, EventArgs e)
        {
            string username = TraktAPI.Username;

            if (string.IsNullOrEmpty(username)) return;

            //IEnumerable<TraktUserCalendar> calendar = TraktAPI.GetCalendarForUser(username);
            //MPTVSeriesLog.Write("Calendar Count: {0}", calendar.Count().ToString());
            
            //foreach (var day in calendar)
            //{
            //    MPTVSeriesLog.Write(day.Date);
            //    foreach (var ep in day.Episodes)
            //    {
            //        MPTVSeriesLog.Write(ep.Show.Title + " - " + ep.Episode.SeasonIndex + "x" + ep.Episode.EpisodeIndex + " - " + ep.Episode.Title);
            //    }

            //}

            //TraktSync traktSync = new TraktSync();

            //List<DBEpisode> episodes = DBEpisode.Get(79488);

            //DBSeries series = Helper.getCorrespondingSeries(79488);

            //traktSync.SeriesID = series[DBSeries.cID];
            //traktSync.IMDBID = series[DBOnlineSeries.cIMDBID];
            //traktSync.Password = TraktAPI.Password;
            //traktSync.UserName = TraktAPI.Username;
            //traktSync.Year = DBSeries.GetSeriesYear(series);
            //traktSync.Title = series[DBOnlineSeries.cOriginalName];

            //List<TraktSync.Episode> epList = new List<TraktSync.Episode>();

            //foreach(DBEpisode ep in episodes)
            //{
            //    TraktSync.Episode episode = new TraktSync.Episode();
            //    episode.SeasonIndex = ep[DBOnlineEpisode.cSeasonIndex];
            //    episode.EpisodeIndex = ep[DBOnlineEpisode.cEpisodeIndex];
            //    epList.Add(episode);
            //}

            //traktSync.EpisodeList = epList;

            //TraktResponse response = TraktAPI.SyncEpisodeLibrary(traktSync, TraktAPI.SyncModes.library);
            //if (response.Message != null) MPTVSeriesLog.Write("Trakt Response: " + response.Message);
            //if (response.Error != null) MPTVSeriesLog.Write("Trakt Error: " + response.Error);

            //System.Threading.Thread.Sleep(2000);

            //response = TraktAPI.SyncEpisodeLibrary(traktSync, TraktAPI.SyncModes.unlibrary);
            //if (response.Message != null) MPTVSeriesLog.Write("Trakt Response: " + response.Message);
            //if (response.Error != null) MPTVSeriesLog.Write("Trakt Error: " + response.Error);

            //System.Threading.Thread.Sleep(2000);

            //response = TraktAPI.SyncEpisodeLibrary(traktSync, TraktAPI.SyncModes.seen);
            //if (response.Message != null) MPTVSeriesLog.Write("Trakt Response: " + response.Message);
            //if (response.Error != null) MPTVSeriesLog.Write("Trakt Error: " + response.Error);

            //System.Threading.Thread.Sleep(2000);

            //response = TraktAPI.SyncEpisodeLibrary(traktSync, TraktAPI.SyncModes.unseen);
            //if (response.Message != null) MPTVSeriesLog.Write("Trakt Response: " + response.Message);
            //if (response.Error != null) MPTVSeriesLog.Write("Trakt Error: " + response.Error);

            //MPTVSeriesLog.Write("Trakt: Getting Shows for user '{0}'", username);
            //IEnumerable<TraktLibraryShows> showsForUser = TraktAPI.GetSeriesForUser(username);
            //MPTVSeriesLog.Write("Show Count: {0}", showsForUser.Count().ToString());

            //MPTVSeriesLog.Write("Trakt: Getting Show overview for Series ID: '79488'");
            //TraktSeriesOverview seriesOverview = TraktAPI.GetSeriesOverview("79488");
            //if (seriesOverview.Status != "failure") MPTVSeriesLog.Write("Successfully got data for {0}", seriesOverview.Title);

            //MPTVSeriesLog.Write("Trakt: Getting User Profile for user '{0}'", username);
            //TraktUserProfile userProfile = TraktAPI.GetUserProfile(username);
            //if (userProfile != null && !string.IsNullOrEmpty(userProfile.Protected)) MPTVSeriesLog.Write("Successfully got data for {0}", userProfile.FullName);

            //MPTVSeriesLog.Write("Trakt: Getting User Friends for user '{0}'", username);
            //IEnumerable<TraktUserProfile> userFriends = TraktAPI.GetUserFriends(username);
            //MPTVSeriesLog.Write("Friend Count: {0}", userFriends.Count().ToString());

            //MPTVSeriesLog.Write("Trakt: Getting watched History for user '{0}'", username);
            //IEnumerable<TraktWatchedEpisodeHistory> watchedHistory = TraktAPI.GetUserWatchedHistory(username);
            //MPTVSeriesLog.Write("Watched History Count: {0}", watchedHistory.Count().ToString());
        }
        #endregion

    }
}
