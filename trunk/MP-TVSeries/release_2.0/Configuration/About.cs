using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries
{
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
        }

        public void setUpMPInfo(string version, DateTime buildDate)
        {
            this.lblVersion.Text += " " + version;
            this.lblBuild.Text += " " + buildDate + " GMT";            
        }

        public void setUpLocalInfo(int series, int seasons, int episodes)
        {
            this.lblSeries.Text += " " + series.ToString();
            this.lblSeasons.Text += " " + seasons.ToString();
            this.lblEpisodes.Text += " " + episodes.ToString();
        }

        public void setUpPaths()
        {
            locationBrowser1.setUpFile("Database:", Settings.GetPath(Settings.Path.database));
            locationBrowser2.setUpFile("Log:", Settings.GetPath(Settings.Path.log));
            locationBrowser3.setUpFolder("Banners:", Settings.GetPath(Settings.Path.banners));
            locationBrowser4.setUpFolder("Fanart:", Settings.GetPath(Settings.Path.fanart));
            locationBrowser5.setUpFolder("Languages:", Settings.GetPath(Settings.Path.lang));
        }

        private void linkForum_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://forum.team-mediaportal.com/my-tvseries-162/");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://www.thetvdb.com");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://sourceforge.net/projects/mp-tvseries/");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://wiki.team-mediaportal.com/Extensions-Plugins/MP-TVSeries");
        }

    }
}
