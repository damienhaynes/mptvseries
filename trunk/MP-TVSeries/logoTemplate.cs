using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries
{
    public partial class logoTemplate : Form
    {
        public string result = string.Empty;
        List<Template> templates = new List<Template>();
        Template sel = null;
        public logoTemplate()
        {
            InitializeComponent();
            load();
        }

        public void load()
        {
            folderBrowserDialog1.SelectedPath = Settings.GetPath(Settings.Path.banners);

            openFileDialog1.CheckFileExists = true;
            openFileDialog1.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG";
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.InitialDirectory = Settings.GetPath(Settings.Path.banners);

            // hard coded templates
            getHardCodedTemplates();

            foreach (Template temp in templates)
                this.listBox1.Items.Add(temp.name);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            result = string.Empty;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (sel != null)
            {
                result = sel.template;
                if (sel.template.Contains("<folder>"))
                {
                    folderBrowserDialog1.ShowDialog();
                    if (folderBrowserDialog1.SelectedPath.Length == 0)
                    {
                        MessageBox.Show("Please provide an existing path");
                    }
                    else
                    {
                        result = result.Replace(@"<folder>", folderBrowserDialog1.SelectedPath);
                        this.Close();
                    }
                }
                else
                {
                    openFileDialog1.ShowDialog();
                    if (openFileDialog1.FileName.Length == 0)
                    {
                        MessageBox.Show("Please provide an existing image file");
                    }
                    else
                    {
                        result = result.Replace(@"<file>", openFileDialog1.FileName);
                        this.Close();
                    }
                }
            }
            else MessageBox.Show("Please select a template");
        }

        public void getHardCodedTemplates()
        {
            Template t = new Template();
            t.name = "Network (Channel) Logo";
            t.descr = "Display a Network (Channel) Logo."
                       + Environment.NewLine + 
                       " Expected Filename: dynamic (example below)"
                       + Environment.NewLine + "abc.png";
            t.template = @"<folder>\<Series.Network>.png;-;;-;=;-;;-;AND;-;;-;=;-;;-;AND;-;;-;=;-;;-;";
            templates.Add(t);

            t = new Template();
            t.name = "HD 720p";
            t.descr = "Display a Logo for files with a dimension of 1280x720 (HD 720p)"
                       + Environment.NewLine +
                       "Expected Filename: static";

            t.template = @"<file>;-;<Episode.videoWidth>;-;=;-;1280;-;AND;-;<Episode.videoHeight>;-;=;-;720;-;AND;-;;-;=;-;;-;";
            templates.Add(t);

            t = new Template();
            t.name = "HD 1080(i/p)";
            t.descr = "Display a Logo for files with a dimension of 1920x1080 (HD 1080i/p)"
                       + Environment.NewLine +
                       "Expected Filename: static";

            t.template = @"<file>;-;<Episode.videoWidth>;-;=;-;1920;-;AND;-;<Episode.videoHeight>;-;=;-;1080;-;AND;-;;-;=;-;;-;";
            templates.Add(t);

            t = new Template();
            t.name = "HRHD 540p";
            t.descr = "Display a Logo for files with a dimension of approximately 960x540 (HRHD)"
                      + Environment.NewLine +
                       "Matching: Width 940 - 990, Height > 520"
                       + Environment.NewLine +
                       "Expected Filename: static";

            t.template = @"<file>;-;<Episode.videoWidth>;-;>;-;940;-;AND;-;<Episode.videoHeight>;-;>;-;520;-;AND;-;<Episode.videoWidth>;-;<;-;990;-;";
            templates.Add(t);

            t = new Template();
            t.name = "Surround Sound";
            t.descr = "Display a surround sound logo for files with more than (or equal to) 5 Audio channels (surround)."
                       + Environment.NewLine +
                       "Expected Filename: static";

            t.template = @"<file>;-;<Episode.AudioChannels>;-;>=;-;5;-;AND;-;;-;=;-;;-;AND;-;;-;=;-;;-;";
            templates.Add(t);

            t = new Template();
            t.name = "Favourites";
            t.descr = "Display a logo for Series you have marked as Favourites."
                       + Environment.NewLine +
                       "Expected Filename: static";

            t.template = @"<file>;-;<Series.isFavourite>;-;=;-;1;-;AND;-;;-;=;-;;-;AND;-;;-;=;-;;-;";
            templates.Add(t);

            t = new Template();
            t.name = "HDTV in Filename";
            t.descr = "Display a logo if the filename has HDTV in it."
                       + Environment.NewLine +
                       "Expected Filename: static";

            t.template = @"<file>;-;<Episode.EpisodeFilename>;-;contains;-;HDTV;-;AND;-;;-;=;-;;-;AND;-;;-;=;-;;-;";
            templates.Add(t);

            t = new Template();
            t.name = "Video Codec";
            t.descr = "Display a logo depending on the Video Codec for a given file."
                       + Environment.NewLine +
                       "Note that some codecs are parsed not in their \"friendly\" name (it uses the FOURCC value). Check the Episode Details in the Configuration to find out what a codec parses as."
                        +Environment.NewLine +
                        "Examples (friendly name vs parsed as:"
                        +Environment.NewLine +
                        "DivX: divx or DIV3 or DX50"
                        +Environment.NewLine +
                        "XVID: xvid"
                        +Environment.NewLine +
                        "H264: V_MPEG4_ISO_AVC"
                        + Environment.NewLine +
                        " Expected Filename: dynamic (example below)"
                       + Environment.NewLine + "V_MPEG4_ISO_AVC.png";

            t.template = @"<folder>\<Episode.VideoCodec>.png;-;;-;=;-;;-;AND;-;;-;=;-;;-;AND;-;;-;=;-;;-;";
            templates.Add(t);

            t = new Template();
            t.name = "Audio Codec";
            t.descr = "Display a logo depending on the Audio Codec for a given file."
                       + Environment.NewLine +
                       "Note that some codecs are parsed not in their \"friendly\" name (it uses the FOURCC value). Check the Episode Details in the Configuration to find out what a codec parses as."
            +Environment.NewLine +
            "Examples (friendly name vs parsed as:"
            +Environment.NewLine +
            "MP3: MPEG-1A L3"
            +Environment.NewLine +
            "AC3: A_AC3 or AC3"
            +Environment.NewLine +
            "H264: V_MPEG4_ISO_AVC"
            +Environment.NewLine +
            " Expected Filename: dynamic (example below)"
           + Environment.NewLine + "V_MPEG4_ISO_AVC.png";

            t.template = @"<folder>\<Episode.VideoCodec>.png;-;;-;=;-;;-;AND;-;;-;=;-;;-;AND;-;;-;=;-;;-;";
            templates.Add(t);

            t = new Template();
            t.name = "16:9";
            t.descr = "Display a logo for files in 16:9 (Widescreen) Aspect Ratio. Values from 1.7 - 1.85 are accepted."
                       + Environment.NewLine +
                       "Expected Filename: static";

            t.template = @"<file>;-;<Episode.VideoAspectRatio>;-;>;-;1.7;-;AND;-;<Episode.VideoAspectRatio>;-;<;-;1.85;-;AND;-;;-;=;-;;-;";
            templates.Add(t);

            t = new Template();
            t.name = "4:3";
            t.descr = "Display a logo for files in 4:3 (Fullscreen) Aspect Ratio. Values from 1.3 - 1.5 are accepted."
                       + Environment.NewLine +
                       "Expected Filename: static";

            t.template = @"<file>;-;<Episode.VideoAspectRatio>;-;>;-;1.3;-;AND;-;<Episode.VideoAspectRatio>;-;<;-;1.5;-;AND;-;;-;=;-;;-;";
            templates.Add(t);

            t = new Template();
            t.name = "Genre Logo";
            t.descr = "Display a Logo depending on the genre of a series."
                       + Environment.NewLine +
                       "Note that values such as Comedy/Animation will be handled as two and both individual logos displayed"
                       + Environment.NewLine +
                       " Expected Filename: dynamic (example below)"
                       + Environment.NewLine + "comedy.png";
            t.template = @"<folder>\<Series.Genre>.png;-;;-;=;-;;-;AND;-;;-;=;-;;-;AND;-;;-;=;-;;-;";
            templates.Add(t);

            t = new Template();
            t.name = "Subtitles Logo";
            t.descr = "Display a Logo if an Episode has subtitles alongside it."
                       + Environment.NewLine +
                       "Expected Filename: static";

            t.template = @"<file>;-;<Episode.AvailableSubtitles>;-;=;-;1;-;AND;-;;-;=;-;;-;AND;-;;-;=;-;;-;";
            templates.Add(t);
            

            t = new Template();
            t.name = "10 min Episode";
            t.descr = "Display a logo if an episode is between 5:00 and 14:59 in lenght."
                       + Environment.NewLine +
                       "Expected Filename: static";

            t.template = @"<file>;-;<Episode.localPlaytime>;-;>=;-;300000;-;AND;-;<Episode.localPlaytime>;-;<;-;900000;-;AND;-;;-;=;-;;-;";
            templates.Add(t);

            t = new Template();
            t.name = "20 min Episode";
            t.descr = "Display a logo if an episode is between 15:00 and 24:59 in lenght."
                       + Environment.NewLine +
                       "Expected Filename: static";

            t.template = @"<file>;-;<Episode.localPlaytime>;-;>=;-;900000;-;AND;-;<Episode.localPlaytime>;-;<;-;1500000;-;AND;-;;-;=;-;;-;";
            templates.Add(t);

            t = new Template();
            t.name = "30 min Episode";
            t.descr = "Display a logo if an episode is between 25:00 and 34:59 in lenght."
                       + Environment.NewLine +
                       "Expected Filename: static";

            t.template = @"<file>;-;<Episode.localPlaytime>;-;>=;-;1500000;-;AND;-;<Episode.localPlaytime>;-;<;-;2100000;-;AND;-;;-;=;-;;-;";
            templates.Add(t);

            t = new Template();
            t.name = "40 min Episode";
            t.descr = "Display a logo if an episode is between 35:00 and 44:59 in lenght."
                       + Environment.NewLine +
                       "Expected Filename: static";

            t.template = @"<file>;-;<Episode.localPlaytime>;-;>=;-;2100000;-;AND;-;<Episode.localPlaytime>;-;<;-;2700000;-;AND;-;;-;=;-;;-;";
            templates.Add(t);

            t = new Template();
            t.name = "50 min Episode";
            t.descr = "Display a logo if an episode is between 45:00 and 54:59 in lenght."
                       + Environment.NewLine +
                       "Expected Filename: static";

            t.template = @"<file>;-;<Episode.localPlaytime>;-;>=;-;2700000;-;AND;-;<Episode.localPlaytime>;-;<;-;3300000;-;AND;-;;-;=;-;;-;";
            templates.Add(t);

            t = new Template();
            t.name = "60 min Episode";
            t.descr = "Display a logo if an episode is between 55:00 and 1:04:59 in lenght."
                       + Environment.NewLine +
                       "Expected Filename: static";

            t.template = @"<file>;-;<Episode.localPlaytime>;-;>=;-;3300000;-;AND;-;<Episode.localPlaytime>;-;<;-;3900000;-;AND;-;;-;=;-;;-;";
            templates.Add(t);

            t = new Template();
            t.name = "80 min Episode (2x40)";
            t.descr = "Display a logo if an episode is between 1:10:00 and 1:19:59 in lenght."
                       + Environment.NewLine +
                       "Expected Filename: static";

            t.template = @"<file>;-;<Episode.localPlaytime>;-;>=;-;4200000;-;AND;-;<Episode.localPlaytime>;-;<;-;5400000;-;AND;-;;-;=;-;;-;";
            templates.Add(t);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            sel = templates[listBox1.SelectedIndex];
            this.textBox1.Text = sel.descr + Environment.NewLine;
            if (sel.template.Contains("<folder>"))
                textBox1.Text += Environment.NewLine + "You will be asked to provide a folder where the logos (image files) matching the description (will) reside.";
            else
                textBox1.Text += Environment.NewLine + "You will be asked to point to an image file to be displayed.";
        }
    }

    public class Template
    {
        public string name;
        public string template;
        public string descr;
    }
}