using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries.Configuration {
    public partial class DeleteDialog : Form {

        public enum DeleteType {
            disk,
            database,
            diskdatabase
        }

        public DeleteType DeleteMode { get; set; }

        public DeleteDialog() {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e) {
            DeleteMode = DeleteType.database;
            radioDeleteFromDatabase.Checked = true;
            base.OnLoad(e);
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void radioDeleteFromDatabase_Click(object sender, EventArgs e) {
            DeleteMode = DeleteType.database;
        }

        private void radioDeleteFromDisk_Click(object sender, EventArgs e) {
            DeleteMode = DeleteType.disk;
        }

        private void radioDeleteFromDiskAndDatabase_Click(object sender, EventArgs e) {
            DeleteMode = DeleteType.diskdatabase;
        }

    }
}
