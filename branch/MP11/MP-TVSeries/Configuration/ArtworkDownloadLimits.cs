using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries.Configuration
{
    public partial class ArtworkDownloadLimits : Form
    {
        public ArtworkDownloadLimits()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            // init values
            numericUpDownSeriesWideBaners.Value = DBOption.GetOptions(DBOption.cArtworkLimitSeriesWideBanners);
            numericUpDownSeriesPosters.Value = DBOption.GetOptions(DBOption.cArtworkLimitSeriesPosters);
            numericUpDownSeasonPosters.Value = DBOption.GetOptions(DBOption.cArtworkLimitSeasonPosters);

            base.OnLoad(e);
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            // apply changes
            DBOption.SetOptions(DBOption.cArtworkLimitSeriesWideBanners, (int)numericUpDownSeriesWideBaners.Value);
            DBOption.SetOptions(DBOption.cArtworkLimitSeriesPosters, (int)numericUpDownSeriesPosters.Value);
            DBOption.SetOptions(DBOption.cArtworkLimitSeasonPosters, (int)numericUpDownSeasonPosters.Value);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
