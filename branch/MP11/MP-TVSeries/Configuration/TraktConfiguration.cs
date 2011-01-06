using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WindowPlugins.GUITVSeries.Trakt;

namespace WindowPlugins.GUITVSeries.Configuration
{
    public partial class TraktConfiguration : UserControl
    {
        public TraktConfiguration()
        {
            InitializeComponent();
            LoadFromDB();
        }

        private void LoadFromDB()
        {
            textBoxUsername.Text = DBOption.GetOptions(DBOption.cTraktUsername);
            textBoxPassword.Text = DBOption.GetOptions(DBOption.cTraktPassword);
            textBoxAPIKey.Text = DBOption.GetOptions(DBOption.cTraktAPIKey);
        }

        private void textBoxUsername_TextChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cTraktUsername, textBoxUsername.Text);
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cTraktPassword, textBoxPassword.Text);
        }

        private void linkLabelSignUp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {            
            System.Diagnostics.Process.Start(@"http://trakt.tv");
        }

        private void textBoxAPIKey_TextChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cTraktAPIKey, textBoxAPIKey.Text);
        }

        private void buttonTestAPI_Click(object sender, EventArgs e)
        {
            string username = DBOption.GetOptions(DBOption.cTraktUsername);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(DBOption.GetOptions(DBOption.cTraktAPIKey))) return;

            MPTVSeriesLog.Write("Trakt: Getting Shows for user '{0}'", username);
            IEnumerable<TraktLibraryShows> showsForUser = TraktAPI.GetSeriesForUser(DBOption.GetOptions(DBOption.cTraktUsername));
            MPTVSeriesLog.Write("Show Count: {0}", showsForUser.Count().ToString());

            MPTVSeriesLog.Write("Trakt: Getting Show overview for Series ID: '79488'");
            TraktSeriesOverview seriesOverview = TraktAPI.GetSeriesOverview("79488");
            if (seriesOverview.status != "error") MPTVSeriesLog.Write("Successfully got data for {0}", seriesOverview.Title);

            MPTVSeriesLog.Write("Trakt: Getting User Profile for user '{0}'", username);
            TraktUserProfile userProfile = TraktAPI.GetUserProfile(username);            
            if(userProfile != null && !string.IsNullOrEmpty(userProfile.Protected)) MPTVSeriesLog.Write("Successfully got data for {0}", userProfile.FullName);

            MPTVSeriesLog.Write("Trakt: Getting watched History for user '{0}'", username);
            IEnumerable<TraktWatchedHistory> watchedHistory = TraktAPI.GetUserWatchedHistory(username);
            MPTVSeriesLog.Write("Watched History Count: {0}", watchedHistory.Count().ToString());
        }
    }
}
