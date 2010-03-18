using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries.Configuration
{
    public partial class ImportProgessPage : UserControl
    {
        public event EventHandler ImportFinished;

        ConfigurationForm owner = null;
        internal ImportProgessPage(ConfigurationForm owner)
        {
            InitializeComponent();

            this.owner = owner;
            
            this.progressLabel1.Label.Text = "Local Filename Processing";
            this.progressLabel10.Label.Text = "Reading MediaInfo of local files";

            this.progressLabel2.Label.Text = "Matching Series";
            this.progressLabel3.Label.Text = "Identifying Episodes";

            this.progressLabel4.Label.Text = "Retrieving Series MetaData";
            this.progressLabel5.Label.Text = "Updating Episode MetaData";


            this.progressLabel6.Label.Text = "Retrieving Series and Season Artwork";
            this.progressLabel8.Label.Text = "Retrieving Episode Thumbnails";
            this.progressLabel9.Label.Text = "Retrieving Fanart";            
        }

        internal void AddParser(OnlineParsing parser)
        {
            parser.OnlineParsingCompleted += new OnlineParsing.OnlineParsingCompletedHandler(b =>
            {
                this.btnFinish.Enabled = true;
                this.btnCancel.Enabled = false;
            });
            parser.OnlineParsingProgress += new OnlineParsing.OnlineParsingProgressHandler(ReceiveUpdates);
        }

        void setProgressLabel(ProgressLabel lbl, ParsingProgress progress)
        {
            string type = string.Empty;
            switch (progress.CurrentAction)
            {
                case ParsingAction.MediaInfo:
                case ParsingAction.LocalScan:
                    type = "files";
                    break;
                case ParsingAction.IdentifyNewSeries:                
                case ParsingAction.UpdateSeries:
                case ParsingAction.UpdateEpisodes:
                case ParsingAction.UpdateBanners:
                case ParsingAction.UpdateFanart:
                case ParsingAction.GetNewBanners:
                case ParsingAction.GetNewFanArt:
                    type = "series";
                    break;
                case ParsingAction.IdentifyNewEpisodes:
                case ParsingAction.UpdateEpisodeThumbNails:
                    type = "episodes";
                    break;
                default:
                    type = "items";
                    break;
            }
            if(progress.CurrentItem == -1) // indicates whole step is done
            {
                lbl.Status = ProgressLabelStatus.Finished;                
                lbl.Progress.Text = string.Format("{0} {1} processed", progress.TotalItems, type);
            }
            else
            {
                lbl.Status = ProgressLabelStatus.InProgress;
                string item = progress.CurrentProgress == null ? string.Empty : ("(" + progress.CurrentProgress + ") ");
                lbl.Progress.Text = string.Format("{1} of {2} {3} {0}", item, progress.CurrentItem, progress.TotalItems, type);
            }
            
        }

        public void ReceiveUpdates(int nprogress, ParsingProgress progress)
        {            
            if (progress != null)
            {
                //MPTVSeriesLog.Write(string.Format("progress received: {0} {1} {2} {3} d: {5} Pic: {4}", progress.CurrentAction, progress.CurrentItem, progress.CurrentProgress, progress.TotalItems, progress.Details, progress.Picture));
                switch (progress.CurrentAction)
                {
                    case ParsingAction.NoExactMatch:
                        break;
                    case ParsingAction.LocalScan:
                        setProgressLabel(this.progressLabel1, progress);
                        break;
                    case ParsingAction.List_Add:
                        break;
                    case ParsingAction.List_Remove:
                        break;
                    case ParsingAction.MediaInfo:
                        setProgressLabel(this.progressLabel10, progress);
                        break;
                    case ParsingAction.IdentifyNewSeries:
                        setProgressLabel(this.progressLabel2, progress);
                        break;
                    case ParsingAction.IdentifyNewEpisodes:
                        setProgressLabel(this.progressLabel3, progress);
                        break;
                    case ParsingAction.GetOnlineUpdates:
                        break;
                    case ParsingAction.UpdateSeries:
                        setProgressLabel(this.progressLabel4, progress);
                        break;
                    case ParsingAction.UpdateEpisodes:
                        setProgressLabel(this.progressLabel5, progress);
                        break;
                    case ParsingAction.UpdateEpisodeCounts:
                        break;
                    case ParsingAction.UpdateUserRatings:
                        break;
                    case ParsingAction.UpdateBanners:
                        setProgressLabel(this.progressLabel6, progress);
                        break;
                    case ParsingAction.UpdateFanart:
                        setProgressLabel(this.progressLabel9, progress);
                        break;
                    case ParsingAction.GetNewBanners:
                        goto case ParsingAction.UpdateBanners;
                    case ParsingAction.GetNewFanArt:
                        break;
                    case ParsingAction.UpdateEpisodeThumbNails:
                        setProgressLabel(this.progressLabel8, progress);
                        break;
                    case ParsingAction.UpdateUserFavourites:
                        break;
                    case ParsingAction.BroadcastRecentlyAdded:
                        break;
                    default:
                        break;
                }

                // display the details
                if (progress.Details is DBTable)
                {
                    labelNoInfo.Visible = false;
                    labelDetailHeader.Visible = true;
                    labelDetailHeader.Text = progress.Details.ToString();
                    textBoxDetails.Visible = true;
                    pictureBoxDetails.Visible = true;
                    if (progress.Details is DBEpisode)
                    {                        
                        this.textBoxDetails.Text = progress.Details.ToString();
                        bool DontShowSummary = DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedSummary) && !progress.Details[DBOnlineEpisode.cWatched];
                        this.textBoxDetails.Text = DontShowSummary ? "Spoilers!" : progress.Details[DBOnlineEpisode.cEpisodeSummary].ToString();

                        bool DontShowTN = DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedThumbnail) && !progress.Details[DBOnlineEpisode.cWatched];
                        this.pictureBoxDetails.Image = null;
                        if (!DontShowTN)
                        {
                            tryShowDetailsPicture(progress.Picture);
                        }
                    }
                    else if (progress.Details is DBSeries)
                    {
                        this.textBoxDetails.Text = progress.Details.ToString();
                        this.textBoxDetails.Text = progress.Details[DBOnlineSeries.cSummary];
                        this.pictureBoxDetails.Image = null;
                        tryShowDetailsPicture(progress.Picture);
                    }
                }
                else
                {
                    labelNoInfo.Visible = true;
                    textBoxDetails.Text = string.Empty;
                    textBoxDetails.Visible = false;
                    pictureBoxDetails.Image = null;
                    pictureBoxDetails.Visible = false;
                }
            }                
        }

        void tryShowDetailsPicture(string path)
        {
            if (path != null && System.IO.File.Exists(path))
            {
                try
                {
                    this.pictureBoxDetails.Image = ImageAllocator.LoadImageFastFromFile(path);
                }
                catch (Exception) { };
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you wish to Cancel the Import?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (this.owner != null)
                    owner.AbortImport();
                if (this.ImportFinished != null)
                    ImportFinished(this, new EventArgs());
            }
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            if (ImportFinished != null)
                ImportFinished(this, new EventArgs());
        }

        public void ShowDetailsPanel(Control c)
        {
            if (this.panel1.Controls.Contains(c))
            {
                c.BringToFront();
                c.Visible = true;
                return;
            }
            this.panel1.Controls.Add(c);
            c.Dock = DockStyle.Fill;
            c.BringToFront();
            c.Visible = true;

            //this.panelProgress.Width = 275;
        }

        public void RemoveDetailsPanel(Control c)
        {
            this.panel1.Controls.Remove(c);
            //this.panelProgress.Width = 400;
        }

        public void AddSleepingDetailsPanel(Control c)
        {
            this.panel1.Controls.Add(c);
            c.Dock = DockStyle.Fill;
            c.BringToFront();
            c.Visible = false;
        }
    }
}
