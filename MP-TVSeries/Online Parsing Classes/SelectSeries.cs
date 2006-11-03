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
        bool cancelled = false;
        bool neveragain = false;

        public SelectSeries()
        {
            InitializeComponent();
        }

        public DBSeries userChoice
        {
            get 
            {
                if (cancelled || neverAskAgain)
                    return null;
                else
                    return _series[this.listbox_Series.SelectedIndex]; 
            }
        }
        public bool neverAskAgain
        {
            get
            {
                return neveragain;
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

        private void btnCnl_Click(object sender, EventArgs e)
        {
            cancelled = true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnnever_Click(object sender, EventArgs e)
        {
            neveragain = true;
        }
    }
}