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
                listBoxViewTemplate.Items.Add(template.name);
            }

            // Select First Item, to populate description
            listBoxViewTemplate.SelectedIndex = 0;
        }

        private void GetPredefinedTemplates() {
            // Contining Series
            ViewTemplate template = new ViewTemplate();
            template.name = Translation.ViewContinuing;
            template.description = "Filters series list with shows that are still Continuing.";
            template.configuration = @"series<;><Series.Status>;=;Continuing<;><;><nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
            templates.Add(template);

            // Ended Series
            template = new ViewTemplate();
            template.name = Translation.ViewEnded;
            template.description = "Filters series list with shows that have Ended.";
            template.configuration = @"series<;><Series.Status>;=;Ended<;><;><nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
            templates.Add(template);

            // HD Episodes
            template = new ViewTemplate();
			template.name = Translation.ViewHighDefinition;
            template.description = "Filters all episodes that contain a Video Height Greater than or equal to 720 pixels.";         
            template.configuration = @"series<;><Episode.videoHeight>;>=;720<;><;><nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
            templates.Add(template);

            // SD Episodes
            template = new ViewTemplate();
			template.name = Translation.ViewStandardDefinition;
            template.description = "Filters all episodes that contain a Video Height Less than 720 pixels.";
            template.configuration = @"series<;><Episode.videoHeight>;<;720<;><;><nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
            templates.Add(template);

            // Top 10 (Online)
            template = new ViewTemplate();
            template.name = Translation.ViewTop10Online;
            template.description = "Filters theTVDB Top 10 highest ratings series, in descending order.";
			template.configuration = @"series<;><Series.Rating>;!=;<;><Series.Rating>;desc<;>10<nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
            templates.Add(template);

            // Top 10 (User)
            template = new ViewTemplate();
			template.name = Translation.ViewTop10User;
            template.description = "Filters your personal Top 10 highest rated series, in descending order.";
			template.configuration = @"series<;><Series.myRating>;!=;<;><Series.myRating>;desc<;>10<nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
            templates.Add(template);

			// Top 25 (Online)
			template = new ViewTemplate();
			template.name = Translation.ViewTop25Online;
			template.description = "Filters theTVDB Top 25 highest ratings series, in descending order.";
			template.configuration = @"series<;><Series.Rating>;!=;<;><Series.Rating>;desc<;>25<nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
			templates.Add(template);

			// Top 25 (User)
			template = new ViewTemplate();
			template.name = Translation.ViewTop25User;
			template.description = "Filters your personal Top 25 highest rated series, in descending order.";
			template.configuration = @"series<;><Series.myRating>;!=;<;><Series.myRating>;desc<;>25<nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
			templates.Add(template); 

            // Subtitles Episodes
            template = new ViewTemplate();
			template.name = Translation.ViewSubtitles;
            template.description = "Filters series that contain episodes with one or more subtitles.";
            template.configuration = @"series<;><Episode.TextCount>;>=;1<;><;><nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
            templates.Add(template);

            // Multi-Audio Episodes
            template = new ViewTemplate();
			template.name = Translation.ViewMultiAudio;
            template.description = "Filters series that contain episodes with more than one Audio Track.";
            template.configuration = @"series<;><Episode.AudioTracks>;>;1<;><;><nextStep>season<;><;><Season.seasonIndex>;asc<;><nextStep>episode<;><;><Episode.EpisodeIndex>;asc<;>";
            templates.Add(template);
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
            public string configuration;
            public string description;
        }

    }
}
