using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries.Configuration
{
    public partial class DatabaseConfigurator : Form
    {
        private const string defaultConnectionString = @"Persist Security Info=True;USER ID=sa;Password=mediaportal;Initial Catalog=MpTvSeriesDb4;Data Source=localhost\SQLEXPRESS;Connection Timeout=300";

        public DatabaseConfigurator()
        {
            InitializeComponent();

            textBox_dblocation.Text = Settings.GetPath(Settings.Path.database);
            radio_sqlclient.Checked = Settings.UseSQLClient;

            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            if (string.IsNullOrEmpty(Settings.ConnectionString)) {
                builder.ConnectionString = defaultConnectionString;
            } else {
                builder.ConnectionString = Settings.ConnectionString;
            }

            object o = null;
            if (builder.TryGetValue("USER ID", out o)) {
                textBox_username.Text = o.ToString();
            } else {
                textBox_username.Text = "sa";
            }
            if (builder.TryGetValue("Password", out o)) {
                textBox_password.Text = o.ToString();
            } else {
                textBox_password.Text = "mediaportal";
            }
            if (builder.TryGetValue("Data Source", out o)) {
                textBox_server.Text = o.ToString();
            } else {
                textBox_server.Text = @"localhost\SQLEXPRESS";
            }
            if (builder.TryGetValue("Initial Catalog", out o)) {
                textBox_database.Text = o.ToString();
            } else {
                textBox_database.Text = "MpTvSeriesDb4";
            }
        }

        private void button_dbbrowse_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = Settings.GetPath(Settings.Path.database);
            openFileDialog.Filter = "Executable files (*.db3)|";
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                Settings.SetDBPath(openFileDialog.FileName);
                textBox_dblocation.Text = openFileDialog.FileName;
            }
        }

        private void button_create_Click(object sender, EventArgs e)
        {
            string connectionString = getConnectionString();
            try {
                SQLClientProvider.TestConnection(connectionString);
                if (MessageBox.Show(this, "Overwrite Existing Database?", "OverWrite Database", MessageBoxButtons.YesNo) == DialogResult.No) {
                    return;
                }
            } catch {
                //connection test failed - do nothing
            }

            button_ok.Enabled = false;
            try {
                SQLClientProvider.CreateDatabase(connectionString);
                button_test.Enabled = true;
            } catch (Exception ex) {
                MessageBox.Show(this, string.Format("Error Creating Database: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string getConnectionString()
        {
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            builder.Add("Persist Security Info", true);
            builder.Add("USER ID", textBox_username.Text);
            builder.Add("Password", textBox_password.Text);
            builder.Add("Initial Catalog", textBox_database.Text);
            builder.Add("Data Source", textBox_server.Text);
            builder.Add("Connection Timeout", 300);

            return builder.ConnectionString;
        }

        private void button_test_Click(object sender, EventArgs e)
        {
            try {
                SQLClientProvider.TestConnection(getConnectionString());
                button_ok.Enabled = true;
            } catch (Exception ex) {
                MessageBox.Show(this, string.Format("Error Connecting to Database: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void radio_sqlclient_CheckedChanged(object sender, EventArgs e)
        {
            button_ok.Enabled = !radio_sqlclient.Checked;
            textBox_database.Enabled = radio_sqlclient.Checked;
            textBox_dblocation.Enabled = radio_sqlclient.Checked;
            textBox_password.Enabled = radio_sqlclient.Checked;
            textBox_server.Enabled = radio_sqlclient.Checked;
            textBox_username.Enabled = radio_sqlclient.Checked;

            button_dbbrowse.Enabled = !radio_sqlclient.Checked;
            button_test.Enabled = radio_sqlclient.Checked;
            button_create.Enabled = radio_sqlclient.Checked;
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            if (radio_sqlclient.Checked) {
                Settings.SetConnectionString(getConnectionString());
            } else {
                Settings.DeleteConnectionString();
                Settings.SetDBPath(textBox_dblocation.Text);
            }
            Close();
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            button_ok.Enabled = false;
            button_test.Enabled = true;
            button_create.Enabled = true;
        }
    }
}
