using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries
{
    public partial class SelectSeries : Form
    {
        List<DBSeries> _series;

        public SelectSeries(String sLocalSeriesName)
        {
            InitializeComponent();
            label_LocalSeriesName.Text = sLocalSeriesName;
        }

        public DBSeries userChoice
        {
            get 
            {
                return _series[this.listbox_Series.SelectedIndex]; 
            }
        }

        public void addSeriesToSelection(List<DBSeries> series)
        {
            this.listbox_Series.Items.Clear();
            _series = series;
            foreach (DBSeries item in _series)
            {
                this.listbox_Series.Items.Add(item[DBSeries.cPrettyName]); 
            }
        }

        private void listItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.textbox_Summary.Text = "First Aired: " + _series[this.listbox_Series.SelectedIndex]["FirstAired"] + "\r\nOverview:\r\n" + _series[this.listbox_Series.SelectedIndex][DBSeries.cSummary]; 
        }
    }
}