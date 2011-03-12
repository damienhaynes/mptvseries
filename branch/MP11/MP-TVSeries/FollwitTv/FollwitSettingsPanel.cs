using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Follwit.API.UI;
using System.Diagnostics;
using WindowPlugins.GUITVSeries.Configuration;

namespace WindowPlugins.GUITVSeries.FollwitTv {
    public partial class FollwitSettingsPanel : UserControl {

        private bool updatingControls = false;

        public FollwitSettingsPanel() {
            InitializeComponent();
            UpdateControls();
        }

        private void ConnectAccount() {
            LoginForm loginForm = new LoginForm();
            loginForm.ApiUrl = FollwitConnector.ApiUrl;

            DialogResult result = loginForm.ShowDialog();
            if (result == DialogResult.OK) {
                FollwitConnector.Enabled = true;
                FollwitConnector.Username = loginForm.ValidatedUser.Name;
                FollwitConnector.HashedPassword = loginForm.ValidatedUser.HashedPassword;

                Sync();
                UpdateControls();
            }
        }

        private void DisconnectAccount() {
            DialogResult response = MessageBox.Show("Are you sure you want to disconnect this computer\nfrom follw.it?", "follw.it", MessageBoxButtons.YesNo);
            if (response == DialogResult.Yes) {
                FollwitConnector.Enabled = false;
                FollwitConnector.Username = String.Empty;
                FollwitConnector.HashedPassword = String.Empty;
                UpdateControls();
            }
        }

        private void Sync() {
            ProgressDialog popup = new ProgressDialog();
            popup.Text = "Synchronizing Collection...";
            popup.Owner = FindForm();
            FollwitConnector.SyncAllEpisodes(popup.SetProgress);
            popup.ShowDialog();

        }

        private void OpenUserPage() {
            string url = String.Format("{0}u/{1}", FollwitConnector.BaseUrl, FollwitConnector.Username);
            ProcessStartInfo processInfo = new ProcessStartInfo(url);
            Process.Start(processInfo);
        }

        private void OpenMainFollwitPage() {
            ProcessStartInfo processInfo = new ProcessStartInfo(FollwitConnector.BaseUrl);
            Process.Start(processInfo);
        }

        private void SwitchWebsite() {
            if (FollwitConnector.BaseUrl.Equals("http://follw.it/")) {
                DialogResult result = MessageBox.Show("Switch connection to follw.it test server?", "follw.it", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes) FollwitConnector.BaseUrl = "http://devtv.follw.it/";
            } else {
                DialogResult result = MessageBox.Show("Switch connection to follw.it production server?", "follw.it", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes) FollwitConnector.BaseUrl = "http://follw.it/";
            }
        }

        private void UpdateControls() {
            if (updatingControls) return;
            updatingControls = true;

            defineSyncedShowsButton.Enabled = restrictSyncCheckBox.Checked;

            if (!FollwitConnector.Enabled) {
                statusLabel.Text = "Not connected to follw.it!";
                statusLabel.ForeColor = Color.Red;

                userLinkLabel.Visible = false;

                accountButton.Text = "Setup Account";

                publicProfileCheckBox.Checked = false;
                defineSyncedShowsButton.Enabled = false;
                restrictSyncCheckBox.Enabled = false;
                publicProfileCheckBox.Enabled = false;
                syncButton.Enabled = false;
            }
            else {
                statusLabel.Text = "Logged in as:";
                statusLabel.ForeColor = Label.DefaultForeColor;

                userLinkLabel.Visible = true;
                userLinkLabel.Text = FollwitConnector.Username;

                accountButton.Text = "Disconnect Account";

                syncButton.Enabled = true;

                // not implemented. swap when done.
                //try { publicProfileCheckBox.Checked = ??; }
                //catch { }
                restrictSyncCheckBox.Enabled = false;
                publicProfileCheckBox.Enabled = false;
                
            }

            updatingControls = false;
        }


        private void setupDisconnecButton_Click(object sender, EventArgs e) {
            if (FollwitConnector.Enabled)
                DisconnectAccount();
            else
                ConnectAccount();

        }

        private void syncButton_Click(object sender, EventArgs e) {
            Sync();
        }

        private void privateProfileCheckBox_CheckedChanged(object sender, EventArgs e) {

        }

        private void restrictSyncCheckBox_CheckedChanged(object sender, EventArgs e) {

        }

        private void defineSyncedShowsButton_Click(object sender, EventArgs e) {

        }

        private void userLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            OpenUserPage();
        }

        private void logoPanel1_Click(object sender, EventArgs e) {
            if ((ModifierKeys & Keys.Control) == Keys.Control) 
                SwitchWebsite();
            else 
                OpenMainFollwitPage();
        }

        private void logoPanel1_MouseEnter(object sender, EventArgs e) {
            this.Cursor = Cursors.Hand;
        }

        private void logoPanel1_MouseLeave(object sender, EventArgs e) {
            this.Cursor = Cursors.Default;
        }
    }
}
