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
        public TraktConfiguration()
        {
            InitializeComponent();
            LoadFromDB();

            // register text change event after loading password field as we dont want to re-hash
            this.textBoxPassword.TextChanged += new System.EventHandler(this.textBoxPassword_TextChanged);
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
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            // Hash Password
            DBOption.SetOptions(DBOption.cTraktPassword, textBoxPassword.Text.ToSHA1Hash());
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
        
        #region testing
        private void buttonTestAPI_Click(object sender, EventArgs e)
        {
            string username = TraktAPI.Username;

            if (string.IsNullOrEmpty(username)) return;

            MPTVSeriesLog.Write("Trakt: Getting Shows for user '{0}'", username);
            IEnumerable<TraktLibraryShows> showsForUser = TraktAPI.GetSeriesForUser(username);
            MPTVSeriesLog.Write("Show Count: {0}", showsForUser.Count().ToString());

            MPTVSeriesLog.Write("Trakt: Getting Show overview for Series ID: '79488'");
            TraktSeriesOverview seriesOverview = TraktAPI.GetSeriesOverview("79488");
            if (seriesOverview.Status != "failure") MPTVSeriesLog.Write("Successfully got data for {0}", seriesOverview.Title);

            MPTVSeriesLog.Write("Trakt: Getting User Profile for user '{0}'", username);
            TraktUserProfile userProfile = TraktAPI.GetUserProfile(username);            
            if(userProfile != null && !string.IsNullOrEmpty(userProfile.Protected)) MPTVSeriesLog.Write("Successfully got data for {0}", userProfile.FullName);

            MPTVSeriesLog.Write("Trakt: Getting watched History for user '{0}'", username);
            IEnumerable<TraktWatchedEpisodeHistory> watchedHistory = TraktAPI.GetUserWatchedHistory(username);
            MPTVSeriesLog.Write("Watched History Count: {0}", watchedHistory.Count().ToString());
        }
        #endregion
    }
}
