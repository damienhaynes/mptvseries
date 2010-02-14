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
        internal ImportProgessPage(ConfigurationForm owner, OnlineParsing parser)
        {
            InitializeComponent();

            this.owner = owner;
            parser.OnlineParsingCompleted += new OnlineParsing.OnlineParsingCompletedHandler(b =>
                {
                    this.btnFinish.Enabled = true; 
                    this.btnCancel.Enabled = false;
                });
            parser.OnlineParsingProgress += new OnlineParsing.OnlineParsingProgressHandler(ReceiveUpdates);

            this.progressLabel1.Label.Text = "Local Filename Processing";
            this.progressLabel1.Status = ProgressLabelStatus.Finished;
            this.progressLabel1.Progress.Text = "Processed in previous Step";
            this.progressLabel10.Label.Text = "Reading MediaInfo of local files";


            this.progressLabel2.Label.Text = "Identifying Series";
            this.progressLabel3.Label.Text = "Identifying Episodes";

            this.progressLabel4.Label.Text = "Retrieving Series MetaData";
            this.progressLabel5.Label.Text = "Updating Episode MetaData";


            this.progressLabel6.Label.Text = "Retrieving Series and Season Artwork";
            this.progressLabel8.Label.Text = "Retrieving Episode Thumbnails";
            this.progressLabel9.Label.Text = "Retrieving Fanart";            
        }

        void setProgressLabel(ProgressLabel lbl, ParsingProgress progress)
        {
            string type = string.Empty;
            switch (progress.CurrentAction)
            {
                case ParsingAction.MediaInfo:
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
                    if (progress.Details is DBEpisode)
                    {
                        this.labelDetails.Text = progress.Details.ToString();
                        bool DontShowSummary = DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedSummary) && !progress.Details[DBOnlineEpisode.cWatched];
                        this.textDetails.Text = DontShowSummary ? "Spoilers!" : progress.Details[DBOnlineEpisode.cEpisodeSummary].ToString();

                        bool DontShowTN = DBOption.GetOptions(DBOption.cView_Episode_HideUnwatchedThumbnail) && !progress.Details[DBOnlineEpisode.cWatched];
                        this.pictureDetails.Image = null;
                        if (!DontShowTN)
                        {
                            tryShowDetailsPicture(progress.Picture);
                        }
                    }
                    else if (progress.Details is DBSeries)
                    {
                        this.labelDetails.Text = progress.Details.ToString();
                        this.textDetails.Text = progress.Details[DBOnlineSeries.cSummary];
                        this.pictureDetails.Image = null;
                        tryShowDetailsPicture(progress.Picture);
                    }
                }
                else
                {
                    labelDetails.Text = string.Empty;
                    textDetails.Text = string.Empty;
                    pictureDetails.Image = null;
                }
            }                
        }

        void tryShowDetailsPicture(string path)
        {
            if (path != null && System.IO.File.Exists(path))
            {
                try
                {
                    this.pictureDetails.Image = ImageAllocator.LoadImageFastFromFile(path);
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
            }
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            if (ImportFinished != null)
                ImportFinished(this, new EventArgs());
        }
    }
}
