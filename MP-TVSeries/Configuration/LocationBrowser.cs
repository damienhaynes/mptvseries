using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries
{
    public partial class LocationBrowser : UserControl
    {
        string folderToBrowse = string.Empty;
        public LocationBrowser()
        {
            InitializeComponent();
        }

        public void setUpFile(string Description, string File)
        {
            this.lblDescr.Text = Description;
            this.txtLocation.Text = File;
            folderToBrowse = System.IO.Path.GetDirectoryName(File);
        }

        public void setUpFolder(string Description, string Folder)
        {
            this.lblDescr.Text = Description;
            this.txtLocation.Text = Folder;
            folderToBrowse = Folder;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (System.IO.Directory.Exists(folderToBrowse))
                System.Diagnostics.Process.Start(folderToBrowse);
            else MessageBox.Show("The directory/file does not exist.");
        }
    }
}
