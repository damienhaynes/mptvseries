using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowPlugins.GUITVSeries.Configuration
{
    public partial class ImportPanelProgress : UserControl
    {
        public ImportPanelProgress()
        {
            InitializeComponent();

            this.labelFilenameProcessingProgress.Label.Text = "Local Filename Processing";
            this.labelMediaInfoProgress.Label.Text = "Reading MediaInfo of local files";

            this.labelMatchingSeriesProgress.Label.Text = "Matching Series";
            this.labelIdentifyEpisodesProgress.Label.Text = "Identifying Episodes";

            this.labelRetrievingSeriesMetaDataProgress.Label.Text = "Retrieving Series MetaData";
            this.labelUpdatingEpisodeMetaDataProgress.Label.Text = "Updating Episode MetaData";

            this.labelRetrievingSeriesArtworkProgress.Label.Text = "Retrieving Series and Season Artwork";
            this.labelRetrievingEpisodeThumbsProgress.Label.Text = "Retrieving Episode Thumbnails";
            this.labelRetrievingFanartProgress.Label.Text = "Retrieving Fanart";
            this.labelRetrievingActorThumbProgress.Label.Text = "Retrieving Actor Thumbnails and Details";

            this.labelRetrievingUserRatings.Label.Text = "Retrieving User Ratings";
            this.labelRetrievingCommunityRatings.Label.Text = "Retrieving Trakt Community Ratings";
            this.labelRetrievingFavourites.Label.Text = "Retrieving User Favourites";
            this.labelRetrievingEpisodeCounts.Label.Text = "Retrieving Episode Counts";
            this.labelCleaningEpisodes.Label.Text = "Cleaning Online Episode References";
        }

        internal void Init()
        {
            OnlineParsing.OnlineParsingProgress += new OnlineParsing.OnlineParsingProgressHandler(ReceiveUpdates);
        }

        internal void DeInit()
        {
            OnlineParsing.OnlineParsingProgress -= new OnlineParsing.OnlineParsingProgressHandler(ReceiveUpdates);
        }

        private void SetProgressLabel(ProgressLabel label, ParsingProgress progress)
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
                case ParsingAction.UpdateBanners:
                case ParsingAction.UpdateFanart:
                case ParsingAction.GetNewBanners:
                case ParsingAction.GetNewFanArt:
                case ParsingAction.GetNewActors:
                case ParsingAction.UpdateCommunityRatings:
                case ParsingAction.UpdateUserFavourites:
                case ParsingAction.UpdateUserRatings:
                case ParsingAction.UpdateEpisodeCounts:
                case ParsingAction.CleanupEpisodes:
                    type = "series";
                    break;

                case ParsingAction.IdentifyNewEpisodes:
                case ParsingAction.UpdateEpisodes:                
                case ParsingAction.UpdateEpisodeThumbNails:
                    type = "episodes";
                    break;

                default:
                    type = "items";
                    break;
            }

            if (progress.CurrentItem == -1) // indicates whole step is done
            {
                label.Status = ProgressLabelStatus.Finished;
                label.Progress.Text = string.Format("{0} {1} processed", progress.TotalItems, type);
            }
            else
            {
                label.Status = ProgressLabelStatus.InProgress;
                string item = progress.CurrentProgress == null ? string.Empty : ("(" + progress.CurrentProgress + ") ");
                label.Progress.Text = string.Format("{1} of {2} {3} {0}", item, progress.CurrentItem, progress.TotalItems, type);
            }
        }

        private void ReceiveUpdates(int nprogress, ParsingProgress progress)
        {
            if (progress != null)
            {
                if (progress.CurrentItem != -1) 
                    MPTVSeriesLog.Write(string.Format("progress received: {0} [{1}/{2}] {3}", progress.CurrentAction, progress.CurrentItem, progress.TotalItems, progress.CurrentProgress));
                
                switch (progress.CurrentAction)
                {
                    case ParsingAction.NoExactMatch:
                        break;
                    case ParsingAction.LocalScan:
                        SetProgressLabel(this.labelFilenameProcessingProgress, progress);
                        break;
                    case ParsingAction.List_Add:
                        break;
                    case ParsingAction.List_Remove:
                        break;
                    case ParsingAction.MediaInfo:
                        SetProgressLabel(this.labelMediaInfoProgress, progress);
                        break;
                    case ParsingAction.IdentifyNewSeries:
                        SetProgressLabel(this.labelMatchingSeriesProgress, progress);
                        break;
                    case ParsingAction.IdentifyNewEpisodes:
                        SetProgressLabel(this.labelIdentifyEpisodesProgress, progress);
                        break;
                    case ParsingAction.GetOnlineUpdates:
                        break;
                    case ParsingAction.UpdateSeries:
                        SetProgressLabel(this.labelRetrievingSeriesMetaDataProgress, progress);
                        break;
                    case ParsingAction.UpdateEpisodes:
                        SetProgressLabel(this.labelUpdatingEpisodeMetaDataProgress, progress);
                        break;
                    case ParsingAction.UpdateEpisodeCounts:
                        SetProgressLabel(this.labelRetrievingEpisodeCounts, progress);
                        break;
                    case ParsingAction.UpdateUserRatings:
                        SetProgressLabel(this.labelRetrievingUserRatings, progress);
                        break;
                    case ParsingAction.UpdateCommunityRatings:
                        SetProgressLabel(this.labelRetrievingCommunityRatings, progress);
                        break;
                    case ParsingAction.UpdateBanners:
                        SetProgressLabel(this.labelRetrievingSeriesArtworkProgress, progress);
                        break;
                    case ParsingAction.UpdateFanart:
                        SetProgressLabel(this.labelRetrievingFanartProgress, progress);
                        break;
                    case ParsingAction.GetNewBanners:
                        goto case ParsingAction.UpdateBanners;
                    case ParsingAction.GetNewFanArt:
                        break;
                    case ParsingAction.GetNewActors:
                        SetProgressLabel(this.labelRetrievingActorThumbProgress, progress);
                        break;
                    case ParsingAction.UpdateEpisodeThumbNails:
                        SetProgressLabel(this.labelRetrievingEpisodeThumbsProgress, progress);
                        break;
                    case ParsingAction.UpdateUserFavourites:
                        SetProgressLabel(this.labelRetrievingFavourites, progress);
                        break;
                    case ParsingAction.CleanupEpisodes:
                        SetProgressLabel(this.labelCleaningEpisodes, progress);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
