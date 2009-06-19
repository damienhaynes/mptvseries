using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries.Configuration {
    public partial class ViewTemplates : Form {

        List<ViewTemplate> templates = new List<ViewTemplate>();

        public ViewTemplate SelectedItem { get; set; }        

        public ViewTemplates() {
            InitializeComponent();
            LoadTemplates();
        }
        
        private void LoadTemplates() {
            // Get List of Pre-Defined Templates
            GetPredefinedTemplates();

            // Populate list with templates
            foreach(ViewTemplate template in templates) {
                listBoxViewTemplate.Items.Add(template.prettyname);
            }

            // Select First Item, to populate description
            if (listBoxViewTemplate.Items.Count > 0) {
                listBoxViewTemplate.SelectedIndex = 0;
            }
            else {
                buttonOK.Enabled = false;
            }
        }

        private void GetPredefinedTemplates() {
            // Get all Views and only show ones currently not available
            List<logicalView> views = logicalView.getAll(true);
            List<string> viewNames = new List<string>();
            
            foreach(logicalView view in views) {
                viewNames.Add(view.Name);
            }

            // All Series
            if (!viewNames.Contains(DBView.cTranslateTokenAll)) {
                ViewTemplate template = new ViewTemplate();
                template.name = DBView.cTranslateTokenAll;
                template.prettyname = Translation.All;
                template.description = "Shows all series in database.";
                template.configuration = @"series<;><;><;><nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                templates.Add(template);
            }

            // Favourite Series
            if (!viewNames.Contains(DBView.cTranslateTokenFavourite)) {
                ViewTemplate template = new ViewTemplate();
                template.name = DBView.cTranslateTokenFavourite;
                template.prettyname = Translation.Favourites;
                template.tagview = true;
                template.description = "Shows all series tagged as Favourite.";
                template.configuration = @"series<;><Series.ViewTags>;like;%|Favourites|%<;><;><nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                templates.Add(template);
            }

            // Online Favourite Series
            if (!viewNames.Contains(DBView.cTranslateTokenOnlineFavourite)) {
                ViewTemplate template = new ViewTemplate();
                template.name = DBView.cTranslateTokenOnlineFavourite;
                template.prettyname = Translation.OnlineFavourites;
                template.tagview = true;
                template.description = "Shows all series tagged as Favourite online at theTVDB.com. This requires the Account Indentifier to be set.";
                template.configuration = @"series<;><Series.ViewTags>;like;%|OnlineFavourites|%<;><;><nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                templates.Add(template);
            }

            // Unwatched Series
            if (!viewNames.Contains(DBView.cTranslateTokenUnwatched)) {
                ViewTemplate template = new ViewTemplate();
                template.name = DBView.cTranslateTokenUnwatched;
                template.prettyname = Translation.Unwatched;
                template.description = "Shows all series with un-watched episodes.";
                template.configuration = @"series<;><Episode.Watched>;=;0<;><;><nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                templates.Add(template);
            }

            // Channels
            if (!viewNames.Contains(DBView.cTranslateTokenChannels)) {
                ViewTemplate template = new ViewTemplate();
                template.name = DBView.cTranslateTokenChannels;
                template.prettyname = Translation.Channels;
                template.description = "Groups all Series according to Aired TV Channel (Network) e.g. ABC, BBC, The CW, Comedy Channel...";
                template.configuration = @"group:<Series.Network><;><;><;><nextStep>series<;><;><;><nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                templates.Add(template);
            }

            // Genres
            if (!viewNames.Contains(DBView.cTranslateTokenGenres)) {
                ViewTemplate template = new ViewTemplate();
                template.name = DBView.cTranslateTokenGenres;
                template.prettyname = Translation.Genres;
                template.description = "Groups all Series according to Genre (Network), e.g. Action, Drama, Comedy...";
                template.configuration = @"group:<Series.Genre><;><;><;><nextStep>series<;><;><;><nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                templates.Add(template);
            }

            // Content Rating
            if (!viewNames.Contains(DBView.cTranslateTokenContentRating)) {
                ViewTemplate template = new ViewTemplate();
                template.name = DBView.cTranslateTokenContentRating;
                template.prettyname = Translation.ContentRating;
                template.description = "Groups all Series according to Content Rating (Certification) e.g. Rating PG, Rating M...";
                template.configuration = @"group:<Series.ContentRating><;><;><;><nextStep>series<;><;><;><nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                templates.Add(template);
            }

            // View Tags
            if (!viewNames.Contains(DBView.cTranslateTokenViewTags)) {
                ViewTemplate template = new ViewTemplate();
                template.name = DBView.cTranslateTokenViewTags;
                template.prettyname = Translation.ViewTags;
                template.description = "Groups all Series according to View Tags, view tags are views that you can assign series too.";
                template.configuration = @"group:<Series.ViewTags><;><;><;><nextStep>series<;><;><;><nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                templates.Add(template);
            }

            // Actors
            if (!viewNames.Contains(DBView.cTranslateTokenActors)) {
                ViewTemplate template = new ViewTemplate();
                template.name = DBView.cTranslateTokenActors;
                template.prettyname = Translation.Actors;
                template.description = "Groups all Series according to starring Actors.";
                template.configuration = @"group:<Series.Actors><;><;><;><nextStep>series<;><;><;><nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                templates.Add(template);
            }

            // Latest (30 Days Episode View)
            if (!viewNames.Contains(DBView.cTranslateTokenLatest)) {
                ViewTemplate template = new ViewTemplate();
                template.name = DBView.cTranslateTokenLatest;
                template.prettyname = Translation.Latest;
                template.description = "Filters the last 30 Days of episodes which have Aired. This view will skip Series selection and take you directly to the list of episodes. Episodes are listed in descending order.";
                template.configuration = @"episode<;><Episode.FirstAired>;<=;<today><cond><Episode.FirstAired>;>=;<today-30><;><Episode.FirstAired>;desc<;>";
                templates.Add(template);
            }

            // Recently Added (Last 7 Days Episode View)
            if (!viewNames.Contains(DBView.cTranslateTokenRecentlyAdded)) {
                ViewTemplate template = new ViewTemplate();
                template.name = DBView.cTranslateTokenRecentlyAdded;
                template.prettyname = Translation.RecentlyAdded;
                template.description = "Filters the last 7 days of recently added episodes into your library. This view will skip Series selection and take you directly to the list of episodes. Episodes are listed in descending order.";
                template.configuration = @"episode<;><Episode.FileDateCreated>;>=;<today-7><;><Episode.FileDateCreated>;desc<;>";
                templates.Add(template);
            }

            // Contining Series
            if (!viewNames.Contains(DBView.cTranslateTokenContinuing)) {
                ViewTemplate template = new ViewTemplate();
                template.name = DBView.cTranslateTokenContinuing;
                template.prettyname = Translation.Continuing;
                template.description = "Filters series list with shows that are still Continuing.";
                template.configuration = @"series<;><Series.Status>;=;Continuing<;><;><nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                templates.Add(template);
            }

            // Ended Series
            if (!viewNames.Contains(DBView.cTranslateTokenEnded)) {
                ViewTemplate template = new ViewTemplate();
                template.name = DBView.cTranslateTokenEnded;
                template.prettyname = Translation.Ended;
                template.description = "Filters series list with shows that have Ended.";
                template.configuration = @"series<;><Series.Status>;=;Ended<;><;><nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                templates.Add(template);
            }

            // HD Episodes
            if (!viewNames.Contains(DBView.cTranslateTokenHighDefinition)) {
                ViewTemplate template = new ViewTemplate();
                template.name = DBView.cTranslateTokenHighDefinition;
                template.prettyname = Translation.HighDefinition;
                template.description = "Filters all episodes that contain a Video Height Greater than or equal to 720 pixels.";
                template.configuration = @"series<;><Episode.videoHeight>;>=;720<;><;><nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                templates.Add(template);
            }

            // SD Episodes
            if (!viewNames.Contains(DBView.cTranslateTokenStandardDefinition)) {
                ViewTemplate template = new ViewTemplate();
                template.name = DBView.cTranslateTokenStandardDefinition;
                template.prettyname = Translation.StandardDefinition;
                template.description = "Filters all episodes that contain a Video Height Less than 720 pixels.";
                template.configuration = @"series<;><Episode.videoHeight>;<;720<;><;><nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                templates.Add(template);
            }

            // Top 10 (Online)
            if (!viewNames.Contains(DBView.cTranslateTokenTop10Online)) {
                ViewTemplate template = new ViewTemplate();
                template.name = DBView.cTranslateTokenTop10Online;
                template.prettyname = Translation.Top10Online;
                template.description = "Filters theTVDB Top 10 highest ratings series, in descending order.";
                template.configuration = @"series<;><Series.Rating>;!=;<;><Series.Rating>;desc<;>10<nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                templates.Add(template);
            }
            
            // Top 10 (User)
            if (!viewNames.Contains(DBView.cTranslateTokenTop10User)) {
                ViewTemplate template = new ViewTemplate();
                template.name = DBView.cTranslateTokenTop10User;
                template.prettyname = Translation.Top10User;
                template.description = "Filters your personal Top 10 highest rated series, in descending order.";
                template.configuration = @"series<;><Series.myRating>;!=;<;><Series.myRating>;desc<;>10<nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                templates.Add(template);
            }

			// Top 25 (Online)
            if (!viewNames.Contains(DBView.cTranslateTokenTop25Online)) {
                ViewTemplate template = new ViewTemplate();
                template.name = DBView.cTranslateTokenTop25Online;
                template.prettyname = Translation.Top25Online;
                template.description = "Filters theTVDB Top 25 highest ratings series, in descending order.";
                template.configuration = @"series<;><Series.Rating>;!=;<;><Series.Rating>;desc<;>25<nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                templates.Add(template);
            }

			// Top 25 (User)
            if (!viewNames.Contains(DBView.cTranslateTokenTop25User)) {
                ViewTemplate template = new ViewTemplate();
                template.name = DBView.cTranslateTokenTop25User;
                template.prettyname = Translation.Top25User;
                template.description = "Filters your personal Top 25 highest rated series, in descending order.";
                template.configuration = @"series<;><Series.myRating>;!=;<;><Series.myRating>;desc<;>25<nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                templates.Add(template);
            }

            // Subtitles Episodes
            if (!viewNames.Contains(DBView.cTranslateTokenSubtitles)) {
                ViewTemplate template = new ViewTemplate();
                template.name = DBView.cTranslateTokenSubtitles;
                template.prettyname = Translation.Subtitles;
                template.description = "Filters series that contain episodes with one or more subtitles.";
                template.configuration = @"series<;><Episode.TextCount>;>=;1<;><;><nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                templates.Add(template);
            }

            // Multi-Audio Episodes
            if (!viewNames.Contains(DBView.cTranslateTokenMultiAudio)) {
                ViewTemplate template = new ViewTemplate();
                template.name = DBView.cTranslateTokenMultiAudio;
                template.prettyname = Translation.MultiAudio;
                template.description = "Filters series that contain episodes with more than one Audio Track.";
                template.configuration = @"series<;><Episode.AudioTracks>;>;1<;><;><nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
                templates.Add(template);
            }

        }

        private void buttonOK_Click(object sender, EventArgs e) {
            // Set SelectedItem property
            SelectedItem = templates[listBoxViewTemplate.SelectedIndex];            
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void listBoxViewTemplate_SelectedIndexChanged(object sender, EventArgs e) {
            textBoxViewDescription.Text = templates[listBoxViewTemplate.SelectedIndex].description;
        }

		private void listBoxViewTemplate_DoubleClick(object sender, EventArgs e) {
			int selection = listBoxViewTemplate.SelectedIndex;
			if (selection > 0) {
				SelectedItem = templates[selection];
				DialogResult = DialogResult.OK;
				Close();
			}
		}

        public class ViewTemplate {
            public string name;
            public string prettyname;
            public string configuration;
            public string description;
            public bool tagview;
        }

    }
}
