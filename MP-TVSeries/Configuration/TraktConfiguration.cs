using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
        }

        private void textBoxUsername_TextChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cTraktUsername, textBoxUsername.Text);
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            DBOption.SetOptions(DBOption.cTraktPassword, textBoxPassword.Text);
        }
    }
}
