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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowPlugins.GUITVSeries.Local_Parsing_Classes {
    public partial class ManualParseDialog : Form {
        private FileInfo videoFile;
        private static Hashtable episodeCache;
        
        private static List<DBOnlineEpisode> allEpisodes;
        private static List<DBOnlineSeries> allSeries;

        private bool checkingForSeries = false;

        private const string SERIES_HELP_MESSAGE = "For an unlisted series, type series name here and press <ENTER>";

        // launches the dialog with no default information specified
        public ManualParseDialog() : this (string.Empty) {
        }

        static ManualParseDialog() {
            episodeCache = new Hashtable();
        }

        // launches the ManualParseDialog with the specified filename loaded in the file textfield        
        public ManualParseDialog(string filename) {
            InitializeComponent();

            if (filename != string.Empty)
                selectFileButton.Enabled = false;

            videoFile = null;
 
            setFile(filename);

            allEpisodes = new List<DBOnlineEpisode>();
            allSeries = new List<DBOnlineSeries>();

            // populate the series dropdown listed with series that currently exist in the DB
            List<DBOnlineSeries> seriesList = DBOnlineSeries.getAllSeries();
            foreach (DBOnlineSeries currSeries in seriesList) {
                this.seriesComboBox.Items.Add(currSeries);
            }
            
            // set the default help info for the series combo box
            seriesComboBox.Text = SERIES_HELP_MESSAGE;
            seriesComboBox.ForeColor = Color.Gray;
        }

        // checks the validity of the provided file, if ok, store it's info and update screen
        private void setFile(string filename) {
            // check if we have anythign passed in
            if (filename != string.Empty) {
                videoFile = new FileInfo(filename);

                // file exists, update screen and quit
                if (videoFile.Exists) {
                    fileTextBox.Text = videoFile.Name;
                    return;
                }
            }

            // we couldn't create a valid file object pointing to an existing file
            videoFile = null;
            fileTextBox.Text = "";
            return;
        }

        // retrieves a list of all episodes info for a given series. if we already pulled
        // it once, it will use to cacheing to prevent multiple pulls of the same data
        private List<DBOnlineEpisode> getEpisodes(DBOnlineSeries series) {
            if (series == null)
                return null;
            
            // try to load from cache. i.e. check if we already loaded before, and if
            // so don't connect to the online DB again
            List<DBOnlineEpisode> episodeList;
            episodeList = (List<DBOnlineEpisode>) episodeCache[series];

            // if we dont have the episode list, pull it down
            if (episodeList == null) {
                // try to grab the season id, and exit if we fail. 
                int selectedSeriesID;
                bool haveValidSeriesID = int.TryParse(series[DBOnlineSeries.cID], out selectedSeriesID);
                if (!haveValidSeriesID) {
                    episodeComboBox.Enabled = false;
                    return new List<DBOnlineEpisode>();
                }

                // retrieve and store the rsults
                GetEpisodes episodeGrabber = new GetEpisodes(selectedSeriesID.ToString());//new GetEpisodes(selectedSeriesID);
                episodeList = episodeGrabber.Results;
                episodeCache.Add(series, episodeList);
            }

            return episodeList;
        }

        // commits the episode to the database (as well as the season and series if neccisary)
        private bool commitEpisode()
        {
            DBOnlineEpisode onlineEp = (DBOnlineEpisode)episodeComboBox.SelectedItem;
            DBOnlineSeries onlineSeries = (DBOnlineSeries)seriesComboBox.SelectedItem;

            if (onlineEp == null || onlineSeries == null)
                return false;

            int seriesID = (int)onlineSeries[DBOnlineSeries.cID];
            int seasonNum = (int)onlineEp[DBOnlineEpisode.cSeasonIndex];
            int episodeNum = (int)onlineEp[DBOnlineEpisode.cEpisodeIndex];

            // construct and add the episode (i am not sure how much of this is required...
            // would be nice if a DBOnlineEpisode object could just create a DBEpisode object...
            DBEpisode episode = new DBEpisode(onlineEp, this.videoFile.FullName);
            episode[DBEpisode.cImportProcessed] = 1;            
            episode[DBEpisode.cIsAvailable] = 1;

            episode[DBOnlineEpisode.cID] = 0; //Force it to update on next scan
            if (episode[DBOnlineEpisode.cEpisodeName].ToString().Length == 0)
                episode[DBOnlineEpisode.cEpisodeName] = onlineEp[DBOnlineEpisode.cEpisodeName];
            episode.Commit();

            // make sure the season is in the DB
            DBSeason season = new DBSeason(seriesID, seasonNum);
            season[DBSeason.cHasLocalFilesTemp] = true;
            season[DBSeason.cHasEpisodes] = true;
            if (onlineEp[DBOnlineEpisode.cWatched] == 0)
                season[DBSeason.cUnwatchedItems] = 1;
            season.Commit();

            // build the local series
            DBSeries localSeries = DBSeries.Get(onlineSeries[DBOnlineSeries.cID]);
            if (localSeries == null) {
                localSeries = new DBSeries((string)onlineSeries[DBOnlineSeries.cPrettyName]);
                localSeries[DBSeries.cID] = onlineSeries[DBOnlineSeries.cID];
            }
            localSeries.Commit();
            onlineSeries[DBOnlineSeries.cHasLocalFiles] = 1;
            if (onlineEp[DBOnlineEpisode.cWatched] == 0)
                onlineSeries[DBOnlineSeries.cUnwatchedItems] = 1;
            onlineSeries.Commit();

            // update detailed online data for new stuff
            OnlineParsing onlineParser = new OnlineParsing(ConfigurationForm.GetInstance());
            //onlineParser.UpdateSeries(true);
            //onlineParser.UpdateBanners(true);
            //onlineParser.UpdateEpisodes(true);

            return true;
        }
 
        // attempts to populate the season combo box based on the selection in the series combo
        private void populateSeasonList() {
           // grab series and episode info
            DBOnlineSeries selectedSeries = (DBOnlineSeries)seriesComboBox.SelectedItem;
            List<DBOnlineEpisode> episodeList = this.getEpisodes(selectedSeries);

            // clear the currently selected season
            seasonComboBox.Items.Clear();
            seasonComboBox.Text = "";
            seasonComboBox.SelectedItem = null;

            // if no series is found or we have no episodes, nothing to populate, quit
            if (selectedSeries == null || episodeList.Count == 0) {
                seasonComboBox.Enabled = false;
                return;
            }
            
            // loop through the episode list and add the seasons to the combo box.
            seasonComboBox.Items.Clear();
            foreach (DBOnlineEpisode currEpisode in episodeList) {
                int seasonNum = (int) currEpisode[DBOnlineEpisode.cSeasonIndex];
                if (!seasonComboBox.Items.Contains(seasonNum))
                    seasonComboBox.Items.Add(seasonNum);
            }
            
            // if we had values, enable the box
            if (seasonComboBox.Items.Count > 0)
                seasonComboBox.Enabled = true;
            else
                seasonComboBox.Enabled = false;
        }

        // attempts to populate the episode combo box based on the values in the series
        // combo and the season combo box. if either has an invalid value, just exit.
        private void populateEpisodeList() {
            int selectedSeason;

            allEpisodes.Clear();
            episodeComboBox.Items.Clear();

            // grab the season and series selected
            DBOnlineSeries selectedSeries = (DBOnlineSeries) seriesComboBox.SelectedItem;
            bool filterBySeason = int.TryParse(seasonComboBox.Text, out selectedSeason);

            // if we dont have a valid series, exit
            if (selectedSeries == null) {
                episodeComboBox.Text = "";
                episodeComboBox.SelectedItem = null;
                episodeComboBox.Enabled = false;
                return;
            }

            // update the entries in the GUI controls
            episodeComboBox.Enabled = true;
            episodeComboBox.SelectedItem = null;
            episodeComboBox.Text = "";

            // grab the episodes for the given series and season from the online DB
            List<DBOnlineEpisode> episodeList = this.getEpisodes(selectedSeries);
            foreach (DBOnlineEpisode currEpisode in episodeList) {
                if (!filterBySeason || (int)currEpisode[DBOnlineEpisode.cSeasonIndex] == selectedSeason) {
                    episodeComboBox.Items.Add(currEpisode);
                    allEpisodes.Add(currEpisode);
                }
            }
        }

        // checks the existing value of the series combobox and updates the help message 
        // accordingly. intended to be triggered by focus changes for the combo box
        private void updateSeriesHelpMessage() {
            // when the series combo gets focus, clear the help message as needed
            if (seriesComboBox.Text == SERIES_HELP_MESSAGE) {
                seriesComboBox.Text = "";
                seriesComboBox.SelectedItem = null;
                seriesComboBox.ForeColor = Color.Black;
                return;
            }

            // if nothing is in the seriesbox, fill in the help info
            if (seriesComboBox.Text == "") {
                seriesComboBox.Text = SERIES_HELP_MESSAGE;
                seriesComboBox.SelectedItem = null;
                seriesComboBox.ForeColor = Color.Gray;
                return;
            }
        }

        // checks if the value in the series combo box is a custom entry. if so, looks
        // up the series and loads it into memory
        private void checkForNewSeries() {
            if (seriesComboBox.Text == SERIES_HELP_MESSAGE)
                return;

            if (checkingForSeries)
                return;
            
            checkingForSeries = true;

            DBOnlineSeries selectedSeries = (DBOnlineSeries)seriesComboBox.SelectedItem;
            if (seriesComboBox.Text.Length != 0 && seriesComboBox.SelectedItem == null) {
                DBOnlineSeries newSeries = OnlineParsing.SearchForSeries(seriesComboBox.Text, false, (Feedback.IFeedback)ConfigurationForm.GetInstance());

                // if a series was able to be parsed from the custom text, 
                // set it as the selected series
                if (newSeries != null) {
                    // check if we already have this series in the list
                    bool alreadyExists = false;
                    foreach (DBOnlineSeries currSeries in seriesComboBox.Items) {
                        if (currSeries[DBOnlineSeries.cID] == newSeries[DBOnlineSeries.cID]) {
                            seriesComboBox.SelectedItem = currSeries;
                            alreadyExists = true;
                            break;
                        }
                    }
                    
                    // if we didnt find it, add it
                    if (!alreadyExists) {
                        seriesComboBox.Items.Add(newSeries);
                        seriesComboBox.SelectedItem = newSeries;
                    }
                }
            }

            checkingForSeries = false;
        }

        private void filterEpisodeList() {
            int    cursorPos = episodeComboBox.SelectionStart;
            string typedText = episodeComboBox.Text.ToLower();

            episodeComboBox.BeginUpdate();
            episodeComboBox.Items.Remove("");

            // go through and only add episodes that match the filtering of the text typed
            foreach (DBOnlineEpisode currEp in allEpisodes) {
                string epName = currEp[DBOnlineEpisode.cEpisodeName];
                if (epName.ToLower().Contains(typedText) && !episodeComboBox.Items.Contains(currEp))
                    episodeComboBox.Items.Add(currEp);
                if (!epName.ToLower().Contains(typedText) && episodeComboBox.Items.Contains(currEp))
                    episodeComboBox.Items.Remove(currEp);
            }

            episodeComboBox.EndUpdate();
            episodeComboBox.SelectionStart = cursorPos;
            Cursor.Show();
        }

        private void clearEpisodeFilter() {
            episodeComboBox.BeginUpdate();

            foreach (DBOnlineEpisode currEp in allEpisodes) 
                if (!episodeComboBox.Items.Contains(currEp))
                    episodeComboBox.Items.Add(currEp);
            
            episodeComboBox.EndUpdate();
        }

        // when an episode is selected, validate the data in the combo boxes and if
        // everything looks ok, enable the OK button
        private void updateOKButton() {
            DBOnlineEpisode onlineEp = (DBOnlineEpisode)episodeComboBox.SelectedItem;
            DBOnlineSeries onlineSeries = (DBOnlineSeries)seriesComboBox.SelectedItem;

            if (onlineEp == null || onlineSeries == null || videoFile == null)
                okButton.Enabled = false;
            else
                okButton.Enabled = true;
        }

        // action performed when the "Browse..." button is clicked. Launches file select dialog.
        private void selectFileButton_click(object sender, EventArgs e) {
            // allow all extensions set in MP
            string filter = "Video Files|";
            foreach (string ext in MediaPortal.Util.Utils.VideoExtensions)
                filter += "*" + ext + ";";
            filter.Remove(filter.Length - 2, 1); // last ;
            openFileDialog.Filter = filter;
            // if we already have a file loaded, init the dialog with that file selected
            if (videoFile != null)
            {
                openFileDialog.FileName = videoFile.Name;
                openFileDialog.InitialDirectory = videoFile.DirectoryName;
            }
            else openFileDialog.FileName = string.Empty;

            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK) {
                setFile(openFileDialog.FileName);
                updateOKButton();
            }
        }    

        private void seriesComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            populateSeasonList();
            populateEpisodeList();
            updateOKButton();       
        }

        private void seriesComboBox_TextChanged(object sender, EventArgs e) {
            populateSeasonList();
            populateEpisodeList();
            updateOKButton();
        }


        private void seriesComboBox_LostFocus(object sender, EventArgs e) {
            updateSeriesHelpMessage();
            //checkForNewSeries();
        }

        private void seriesComboBox_GotFocus(object sender, EventArgs e) {
            updateSeriesHelpMessage();
        }

        private void seasonComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            populateEpisodeList();
            updateOKButton();
        }

        private void episodeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            updateOKButton();
        }

        private void episodeComboBox_TextChanged(object sender, EventArgs e) {
            if (episodeComboBox.SelectedItem == null ||
                episodeComboBox.SelectedItem.ToString() != episodeComboBox.Text) {

                filterEpisodeList();
            }
                
        }

        private void episodeComboBox_DropDownClosed(object sender, EventArgs e) {
            MPTVSeriesLog.Write("count: " + episodeComboBox.Items.Count);
            if (episodeComboBox.Items.Count > 0 && episodeComboBox.SelectedItem != null)
                clearEpisodeFilter();
        }
        
        private void okButton_Click(object sender, EventArgs e) {
            commitEpisode();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void seriesComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r') //ENTER PRESSED
            {
                updateSeriesHelpMessage();
                checkForNewSeries();
            }
        }

    }
}