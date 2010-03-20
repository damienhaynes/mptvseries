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
            switch (Settings.SQLClient) {
                case Settings.SqlClients.sqlClient:
                    radio_sqlclient.Checked = true;
                    break;
                case Settings.SqlClients.mySql:
                    radio_mysql.Checked = true;
                    break;
                default:
                    radio_sqlite.Checked = true;
                    break;

            }

            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            if (!string.IsNullOrEmpty(Settings.ConnectionString)) {
                builder.ConnectionString = Settings.ConnectionString;

                object o = null;
                if (builder.TryGetValue("USER ID", out o)) {
                    textBox_username.Text = o.ToString();
                }
                if (builder.TryGetValue("Password", out o)) {
                    textBox_password.Text = o.ToString();
                }
                if (builder.TryGetValue("Data Source", out o)) {
                    textBox_server.Text = o.ToString();
                }
                if (builder.TryGetValue("Initial Catalog", out o)) {
                    textBox_database.Text = o.ToString();
                }
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

            bool mysql = radio_mysql.Checked;

            try {
                if (mysql) {
                    MySqlProvider.TestConnection(connectionString);
                } else {
                    SQLClientProvider.TestConnection(connectionString);
                }
                if (MessageBox.Show(this, "Overwrite Existing Database?", "OverWrite Database", MessageBoxButtons.YesNo) == DialogResult.No) {
                    return;
                }
            } catch {
                //connection test failed - do nothing
            }

            button_ok.Enabled = false;
            try {
                if (mysql) {
                    MySqlProvider.CreateDatabase(connectionString);
                } else {
                    SQLClientProvider.CreateDatabase(connectionString);
                }
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
                if (radio_mysql.Checked) {
                    MySqlProvider.TestConnection(getConnectionString());
                } else {
                    SQLClientProvider.TestConnection(getConnectionString());
                }
                button_ok.Enabled = true;
            } catch (Exception ex) {
                MessageBox.Show(this, string.Format("Error Connecting to Database: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void radio_sqlclient_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonCheckedChanged(sender, e);

            if (radio_sqlclient.Checked && string.IsNullOrEmpty(Settings.ConnectionString)) {
                textBox_username.Text = "sa";
                textBox_password.Text = "mediaportal";  //default password on a fresh install?
                textBox_server.Text = @"localhost\SQLEXPRESS";
                textBox_database.Text = "MpTvSeriesDb4";
            }

        }

        private void radio_mysql_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonCheckedChanged(sender, e);

            if (radio_mysql.Checked && string.IsNullOrEmpty(Settings.ConnectionString)) {
                textBox_username.Text = "root";
                textBox_password.Text = "MediaPortal";
                textBox_server.Text = "localhost";
                textBox_database.Text = "MpTvSeriesDb4";
            }

        }

        private void radio_sqlite_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonCheckedChanged(sender, e);
        }

        private void radioButtonCheckedChanged(object sender, EventArgs e)
        {
            button_ok.Enabled = radio_sqlite.Checked;
            textBox_database.Enabled = !radio_sqlite.Checked;
            textBox_dblocation.Enabled = !radio_sqlite.Checked;
            textBox_password.Enabled = !radio_sqlite.Checked;
            textBox_server.Enabled = !radio_sqlite.Checked;
            textBox_username.Enabled = !radio_sqlite.Checked;

            button_dbbrowse.Enabled = radio_sqlite.Checked;
            button_test.Enabled = !radio_sqlite.Checked;
            button_create.Enabled = !radio_sqlite.Checked;

        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            if (!radio_sqlite.Checked) {
                Settings.SetConnectionString(getConnectionString());
                if (radio_sqlclient.Checked) {
                    Settings.SetCurrentClient(Settings.SqlClients.sqlClient);
                } else {
                    Settings.SetCurrentClient(Settings.SqlClients.mySql);
                }
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
