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
        List<DBOnlineSeries> _series;
        String m_sSeriesName = String.Empty;

        public SelectSeries(String sLocalSeriesName, bool bSelect)
        {
            InitializeComponent();
            textbox_LocalSeriesName.Text = sLocalSeriesName;
            if (!bSelect)
            {
                this.listbox_Series.Items.Add("No Match Found, try to enter another name for the show");
                btnOK.Text = "Search Again";
            }
            else
            {
                btnOK.Text = "OK";
            }
        }

        public DBOnlineSeries userChoice
        {
            get 
            {
                return _series[this.listbox_Series.SelectedIndex]; 
            }
        }

        public String SeriesName
        {
            get { return m_sSeriesName; }
        }

        public void addSeriesToSelection(List<DBOnlineSeries> series)
        {
            this.listbox_Series.Items.Clear();
            _series = series;
            foreach (DBOnlineSeries item in _series)
            {
                this.listbox_Series.Items.Add(item[DBOnlineSeries.cPrettyName]); 
            }
            if(this.listbox_Series.Items.Count > 0)
                this.listbox_Series.SelectedIndex = 0;
        }

        private void listItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listbox_Series.SelectedIndex != -1)
            {
                this.textbox_Summary.Text = "First Aired: " + _series[this.listbox_Series.SelectedIndex]["FirstAired"] + "\r\nOverview:\r\n" + _series[this.listbox_Series.SelectedIndex][DBOnlineSeries.cSummary];
                btnOK.Text = "OK";
            }
        }

        private void textbox_LocalSeriesName_TextChanged(object sender, EventArgs e)
        {
            btnOK.Text = "Search Again";
            listbox_Series.SelectedIndex = -1;
            m_sSeriesName = textbox_LocalSeriesName.Text;
        }
    }
}