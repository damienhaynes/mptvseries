using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries
{
    public partial class loadingDisplay : Form
    {
        public loadingDisplay()
        {
            InitializeComponent();
            ShowWaiting();
        }

        public new void Close()
        {
            this.Dispose();
            base.Close();
        }

        void ShowWaiting()
        {
            this.series.Text = "0 " + Translation.Series_Plural;
            this.season.Text = "0 " + Translation.Seasons;
            this.episodes.Text = "0 " + Translation.Episodes;
            this.Show();
            this.Refresh();
        }

        public void updateStats(int series, int seasons, int episodes)
        {
            this.series.Text = series.ToString() + " " + Translation.Series_Plural;
            this.season.Text = seasons.ToString() + " " + Translation.Seasons;
            this.episodes.Text = episodes.ToString() + " " + Translation.Episodes;
            this.Refresh();
        }

    }
}