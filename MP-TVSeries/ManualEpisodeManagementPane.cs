#region GNU license
// MP-TVSeries - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2007
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using WindowPlugins.GUITVSeries.Local_Parsing_Classes;

namespace WindowPlugins.GUITVSeries {
    public partial class ManualEpisodeManagementPane : UserControl {
        ConfigurationForm owner;

        public ManualEpisodeManagementPane() {
            InitializeComponent();
        }

        public void SetOwner(ConfigurationForm owner) {
            this.owner = owner;
        }

        // launches an asynchronous refresh of the list of files on the dialog
        public void refreshFileList() {
            refreshProgressBar.Visible = true;
            refreshFileListButton.Enabled = false;
            refreshProgressBar.Value = 0;
            progressTimer.Start();

            fileListView.Items.Clear();

            LocalParse fileParser = new LocalParse();
            fileParser.LocalParseCompleted += new LocalParse.LocalParseCompletedHandler(refreshFileListComplete);
            fileParser.AsyncFullParse();
        }

        // listens for when a local parse has been completed and the results are
        // ready to be loaded onto the screen
        private void refreshFileListComplete(List<parseResult> results) {
            refreshProgressBar.Visible = false;
            refreshFileListButton.Enabled = true;
            progressTimer.Stop();

            populateFileList(results);
        }


        // populates the filelist on screen from the results of a local parse run
        private void populateFileList(List<parseResult> results) {
            List<DBEpisode> loadedEpisodes = DBEpisode.Get(new SQLCondition());
            List<string> loadedEpisodeFilenames = new List<string>();

            foreach (DBEpisode currEp in loadedEpisodes) {
                loadedEpisodeFilenames.Add(currEp[DBEpisode.cFilename]);    
            }

            foreach (parseResult currResult in results) {
                if (!loadedEpisodeFilenames.Contains(currResult.full_filename)) {
                    FileInfo file = new FileInfo(currResult.full_filename);
                    
                    // filename
                    ListViewItem newItem = new ListViewItem(file.Name);
                    newItem.Tag = file;

                    // path begining with import path directory, we want to remove the filename from 
                    // this info, it would be redundant
                    int indexOfFileName = currResult.match_filename.IndexOf(file.Name);
                    string partialPath = currResult.match_filename.Substring(0, indexOfFileName);
                    ListViewItem.ListViewSubItem pathSubItem = new ListViewItem.ListViewSubItem(newItem, partialPath);
                    newItem.SubItems.Add(pathSubItem);

                    fileListView.Items.Add(newItem);
                }
            }
        }

        // launches the manual add dialog with the currently selected file loaded
        private void addEpisode() {
            bool needReload = false;

            foreach (ListViewItem selectedItem in fileListView.SelectedItems) {
                FileInfo selectedFile = (FileInfo)selectedItem.Tag;
                ManualParseDialog parseDialog = new ManualParseDialog(selectedFile.FullName);
                DialogResult result = parseDialog.ShowDialog();
                if (result == DialogResult.OK) {
                    needReload = true;
                    fileListView.Items.Remove(selectedItem);
                }
            }

            if (needReload)
                owner.LoadTree();
        }
        
        // launches the manual add dialog with no file loaded. primarily for 
        // adding episodes not in the import paths
        private void addUnlistedEpisode() {
            ManualParseDialog parseDialog = new ManualParseDialog();
            DialogResult result = parseDialog.ShowDialog();
            if (result == DialogResult.OK)
                owner.LoadTree();
        }

        // plays the selected episode with the default media player
        private void playEpisode() {
            foreach (ListViewItem selectedItem in fileListView.SelectedItems) {
                FileInfo selectedFile = (FileInfo)selectedItem.Tag;
                ProcessStartInfo processInfo = new ProcessStartInfo(selectedFile.FullName);
                Process.Start(processInfo);
            }
        }

        // timer listener for updating the progress bar so the user knows
        // that when we are doing a local parse, something is actually happening
        // shoudl probably be changed to use the main progress bar instead of
        // out custom one.
        private void progressTimer_Tick(object sender, EventArgs e) {
            int newValue = refreshProgressBar.Value + 10;

            if (newValue > refreshProgressBar.Maximum)
                newValue = refreshProgressBar.Minimum;

            refreshProgressBar.Value = newValue;
        }

        // launches the context menu on a right click
        private void fileListView_mouseClick(object sender, MouseEventArgs mouseEvent) {
            if (mouseEvent.Button == MouseButtons.Right && sender == fileListView)
                fileListContextMenu.Show((Control)sender, mouseEvent.Location);
        }

        private void refreshFileListButton_Click(object sender, EventArgs e) {
            refreshFileList();
        }

        private void addUnlistedButton_Click(object sender, EventArgs e) {
            addUnlistedEpisode();
        }
        
        private void manuallyAddButton_Click(object sender, EventArgs e) {
            addEpisode();
        }

        private void playEpisodeButton_Click(object sender, EventArgs e) {
            playEpisode();
        }

        private void addEpisodeMI_Click(object sender, EventArgs e) {
            addEpisode();
        }

        private void playEpisodeMI_Click(object sender, EventArgs e) {
            playEpisode();
        }

        private void fileListView_DoubleClick(object sender, EventArgs e) {
            addEpisode();
        }

    }
}
