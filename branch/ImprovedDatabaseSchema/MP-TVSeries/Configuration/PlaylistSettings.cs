using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WindowPlugins.GUITVSeries.DataClass;

namespace WindowPlugins.GUITVSeries.Configuration {
	public partial class PlaylistSettings : UserControl {

		private string PlaylistPath { get; set; }			
		
		public PlaylistSettings() {
			InitializeComponent();
			LoadFromDB();
		}

		private void LoadFromDB() {
			// Playlist Folder
			PlaylistPath = DBOption.GetOptions(DBOption.cPlaylistPath);
			textBoxPlaylistFolder.Text = PlaylistPath;
		}		

		private void textBoxPlaylistFolder_TextChanged(object sender, EventArgs e) {			
			DBOption.SetOptions(DBOption.cPlaylistPath, textBoxPlaylistFolder.Text);
		}

		private void textBoxBrowse_Click(object sender, EventArgs e) {
			FolderBrowserDialog folderDialog = new FolderBrowserDialog();
			folderDialog.SelectedPath = PlaylistPath;
			folderDialog.Description = "Select or create a New Path save/load tvseries playlists:";
			DialogResult result = folderDialog.ShowDialog();
			if (result == DialogResult.OK)
				textBoxPlaylistFolder.Text = folderDialog.SelectedPath;            
		}

	}
}
