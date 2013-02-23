using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries.Configuration {
    public partial class DeleteDialog : Form {

        public TVSeriesPlugin.DeleteMenuItems DeleteMode { get; set; }
        
        public DeleteDialog(bool hasSubtitles = false, bool hasDuplicateEpisodes = false, bool hasLocalFiles = false)
        {
            InitializeComponent();

            DeleteMode = TVSeriesPlugin.DeleteMenuItems.database;
            radioDeleteFromDatabase.Checked = true;

            if (!hasSubtitles)
                radioDeleteSubtitles.Enabled = false;
            
            if (!hasLocalFiles)
            {
                radioDeleteFromDisk.Checked = false;
                radioDeleteFromDisk.Enabled = false;
                radioDeleteFromDiskAndDatabase.Enabled = false;
            }
            
            if (hasDuplicateEpisodes)
            {
                // doesn't make sense to delete online episode record
                // if there is going to be a duplicate local episode left over
                radioDeleteFromDatabase.Enabled = false;
                radioDeleteFromDiskAndDatabase.Enabled = false;

                DeleteMode = TVSeriesPlugin.DeleteMenuItems.disk;
                radioDeleteFromDisk.Checked = true;
            }            
        }

        public DeleteDialog() {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e) {
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
            DeleteMode = TVSeriesPlugin.DeleteMenuItems.database;
        }

        private void radioDeleteFromDisk_Click(object sender, EventArgs e) {
            DeleteMode = TVSeriesPlugin.DeleteMenuItems.disk;
        }

        private void radioDeleteFromDiskAndDatabase_Click(object sender, EventArgs e) {
            DeleteMode = TVSeriesPlugin.DeleteMenuItems.diskdatabase;
        }

        private void radioDeleteSubtitles_Click(object sender, EventArgs e)
        {
            DeleteMode = TVSeriesPlugin.DeleteMenuItems.subtitles;
        }

    }
}
