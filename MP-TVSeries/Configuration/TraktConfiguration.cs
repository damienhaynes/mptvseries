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
    }
}
