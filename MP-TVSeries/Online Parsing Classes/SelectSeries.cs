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

        public SelectSeries()
        {
            InitializeComponent();
        }

        public DBSeries userChoice
        {
            get 
            {
                if (cancelled)
                    return null;
                else
                    return _series[this.listItems.SelectedIndex]; 
            }
        }

        public void addSeriesToSelection(List<DBSeries> series)
        {
            this.listItems.Items.Clear();
            _series = series;
            foreach (DBSeries item in _series)
            {
                this.listItems.Items.Add(item[DBSeries.cPrettyName]); 
            }
        }

        private void listItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.textBox1.Text = _series[this.listItems.SelectedIndex][DBSeries.cPrettyName];
        }

        private void btnCnl_Click(object sender, EventArgs e)
        {
            cancelled = true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}