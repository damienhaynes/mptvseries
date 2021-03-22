﻿using MediaPortal.GUI.Library;
using MediaPortal.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml;
using WindowPlugins.GUITVSeries.FanartTvAPI.DataStructures;
using WindowPlugins.GUITVSeries.Online_Parsing_Classes;
using WindowPlugins.GUITVSeries.TmdbAPI;
using WindowPlugins.GUITVSeries.TmdbAPI.DataStructures;
using WindowPlugins.GUITVSeries.TmdbAPI.Extensions;
using Action = MediaPortal.GUI.Library.Action;
using Cornerstone.MP;

namespace WindowPlugins.GUITVSeries.GUI
{
    public enum ArtworkType
    {
        SeriesFanart,
        SeriesPoster,
        SeriesBanner,
        SeasonPoster,
        EpisodeThumb
    }

    public enum ArtworkDataProvider
    {
        TVDb, /*Default*/
        TMDb,
        FanartTV
    }

    /// <summary>
    /// Defines the configuration when loading the window
    /// Can be used as a string loading parameter by serialising to JSON
    /// </summary>
    public class ArtworkLoadingParameters
    {
        public int SeriesId { get; set; }

        public int SeasonIndex { get; set; }

        public int EpisodeIndex { get; set; }

        public ArtworkType Type { get; set; }

        public ArtworkDataProvider Provider { get; set; }

        public DBSeries Series
        {
            get
            {
                if ( series == null )
                    series = Helper.getCorrespondingSeries( SeriesId );
                
                return series;
            }
        }
        private DBSeries series;

        public DBSeason Season
        {
            get
            {
                if (season == null)
                    season = Helper.getCorrespondingSeason( SeriesId, SeasonIndex );

                return season;
            }
        }
        private DBSeason season;

        public DBEpisode Episode
        {
            get
            {
                if (episode == null)
                    episode = DBEpisode.Get(SeriesId, SeasonIndex, EpisodeIndex);

                return episode;
            }
        }
        private DBEpisode episode;
    }

    public class GUIArtworkChooser : GUIWindow
    {
        #region Skin Controls

        [SkinControlAttribute( 50 )]
        protected GUIFacadeControl Facade = null;

        [SkinControlAttribute( 2 )]
        protected GUIButtonControl ButtonLayouts = null;

        [SkinControlAttribute( 3 )]
        protected GUIButtonControl ButtonOnlineProviders = null;

        #endregion

        #region Enums

        public enum ContextMenuItem
        {
            Layout,
            Filter,
            Delete,
            ChangeSource
        }

        public enum Layout
        {
            List = 0,
            SmallIcons = 1,
            LargeIcons = 2,
            Filmstrip = 3
        }

        #endregion

        #region Constructor

        public GUIArtworkChooser()
        {
            AsyncArtworkImage = new AsyncImageResource
            {
                Property = "#TVSeries.Artwork.AsyncFilename",
                Delay = DBOption.GetOptions(DBOption.cArtworkLoadingDelay)
            };
        }

        #endregion

        #region Private Properties

        private AsyncImageResource AsyncArtworkImage = null;

        private bool StopDownload { get; set; }

        private Layout CurrentLayout { get; set; }

        private ArtworkLoadingParameters ArtworkParams { get; set; }

        private int DefaultArtIndex { get; set; }
        
        #endregion

        #region Public Properties

        #endregion

        #region Base Overrides

        public override int GetID
        {
            get { return 9817; }
        }

        public override bool Init()
        {
            return Load( GUIGraphicsContext.Skin + @"\TVSeries.ArtworkChooser.xml" );
        }

        protected override void OnPageLoad()
        {
            MPTVSeriesLog.Write( "Entering Artwork Chooser window" );

            // set window name
            GUIPropertyManager.SetProperty( "#currentmodule", Translation.Artwork );

            // clear any properties from previous series
            ClearProperties();

            // set default layout
            int.TryParse(DBOption.GetOptions(DBOption.cArtworkChooserLayout), out int defaultLayout);
            CurrentLayout = ( Layout )defaultLayout;

            // update button label
            if ( ButtonLayouts != null )
            {
                GUIControl.SetControlLabel( GetID, ButtonLayouts.GetID, GetLayoutTranslation( CurrentLayout ) );
            }

            // Deserialise loading parameter from JSON (ArtworkParameters)
            if (!LoadParameters())
            {
                MPTVSeriesLog.Write( "Unable to load Artwork Chooser, loading parameters not provided. Reverting to main TV-Series window" );
                GUIWindowManager.ActivateWindow( 9811 );
                return;
            }

            // update source label
            if ( ButtonOnlineProviders != null )
            {
                GUIControl.SetControlLabel( GetID, ButtonOnlineProviders.GetID, string.Format( Translation.Provider, ArtworkParams.Provider.ToString() ) );
            }

            // get the thumbnails to load for user selection
            DownloadArtworkThumbs();
        }

        protected override void OnPageDestroy( int newWindowId )
        {
            // stop any background tasks
            StopDownload = true;
            GUIConnector.Instance.StopBackgroundTask();

            // save current layout
            DBOption.SetOptions( DBOption.cArtworkChooserLayout, ( int )CurrentLayout );

            MPTVSeriesLog.Write( "Exiting Artwork Chooser window" );
        }

        protected override void OnClicked( int controlId, GUIControl control, Action.ActionType actionType )
        {
            // wait for any background action to finish
            if ( GUIConnector.Instance.IsBusy ) return;

            switch ( controlId )
            {
                // Layout Button
                case 2:
                    ShowLayoutsMenu();
                    break;
                // Data Provider Button
                case 3:
                    ShowDataProvidersMenu();
                    break;
                // Facade
                case 50:
                    switch (ArtworkParams.Type)
                    {
                        case ArtworkType.SeriesPoster:
                            OnSeriesPosterClicked();
                            break;
                        case ArtworkType.SeasonPoster:
                            OnSeasonPosterClicked();
                            break;
                        case ArtworkType.SeriesBanner:
                            OnSeriesWideBannerClicked();
                            break;
                        case ArtworkType.EpisodeThumb:
                            OnEpisodeThumbnailClicked();
                            break;
                        default:
                            OnFanartClicked();
                            break;
                    }
                    break;
            }

            base.OnClicked( controlId, control, actionType );
        }

        public override void OnAction( Action action )
        {
            switch ( action.wID )
            {
                case Action.ActionType.ACTION_PREVIOUS_MENU:
                    break;
            }
            base.OnAction( action );
        }

        protected override void OnShowContextMenu()
        {
            GUIListItem lSelectedItem = Facade.SelectedListItem;
            if ( lSelectedItem == null ) return;

            var lSelectedArtworkItem = lSelectedItem as GUIArtworkListItem;

            var lDlg = ( IDialogbox )GUIWindowManager.GetWindow( ( int )GUIWindow.Window.WINDOW_DIALOG_MENU );
            lDlg.Reset();
            lDlg.SetHeading( Translation.ArtworkChooser );

            // create items for context menu
            var lListItem = new GUIListItem( Translation.ChangeOnlineProvider + " ..." );
            lDlg.Add( lListItem );
            lListItem.ItemId = ( int )ContextMenuItem.ChangeSource;

            if ( lSelectedArtworkItem != null )
            {
                lListItem = new GUIListItem( Translation.ChangeLayout + " ..." );
                lDlg.Add( lListItem );
                lListItem.ItemId = ( int )ContextMenuItem.Layout;

                // allow user to delete a locally downloaded artwork            
                var lArtwork = lSelectedArtworkItem.Item as Artwork;
                if ( lArtwork.IsLocal && !lArtwork.IsDefault )
                {
                    lListItem = new GUIListItem( Translation.ArtworkDelete );
                    lDlg.Add( lListItem );
                    lListItem.ItemId = ( int )ContextMenuItem.Delete;
                }
            }

            // show context menu
            lDlg.DoModal( GUIWindowManager.ActiveWindow );
            if ( lDlg.SelectedId < 0 ) return;

            // do what was requested
            switch ( lDlg.SelectedId )
            {
                case ( ( int )ContextMenuItem.Layout ):
                    ShowLayoutsMenu();
                    break;

                case ( ( int )ContextMenuItem.ChangeSource ):
                    ShowDataProvidersMenu();
                    break;

                case ( ( int )ContextMenuItem.Delete ):
                    DeleteArtwork( lSelectedArtworkItem );
                    break;
            }

            base.OnShowContextMenu();
        }
        #endregion

        #region Public Methods

        public static bool DeleteFile(string aFilename)
        {
            try
            {
                MPTVSeriesLog.Write($"Deleting local artwork '{aFilename}' from disk");
                File.Delete(aFilename);
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Failed to delete local artwork from disk. Reason=" + ex.Message);
                return false;
            }

            return true;
        }

        #endregion

        #region Private Methods
        private void DeleteArtwork( GUIArtworkListItem aSelectedGUIItem )
        {
            // NB: we do not need to worry about deleting 'Default' artwork as 
            // context menu item is not visible when default

            var lArtwork = aSelectedGUIItem.Item as Artwork;

            switch ( ArtworkParams.Type )
            {
                case ArtworkType.SeriesFanart:
                    // step 1: delete artwork from disk
                    if ( DeleteFile( lArtwork.LocalPath ) )
                    {
                        // step 2: remove local reference from database
                        lArtwork.Fanart[DBFanart.cLocalPath] = string.Empty;
                        lArtwork.Fanart.Commit();

                        // step 3: clear the fanart cache
                        DBFanart.ClearSeriesFromCache(lArtwork.Series[DBOnlineSeries.cID]);

                        // step 4: mark as remote
                        lArtwork.IsLocal = false;
                        aSelectedGUIItem.Label2 = Translation.FanArtOnline;

                        // step 5: rotate current background art if we're using random fanart
                        // if using random fanart ignore rotated art in favour of chosen one
                        if ( DBOption.GetOptions( DBOption.cFanartRandom ) && GUIPropertyManager.GetProperty( "#TVSeries.Current.Fanart" ) == lArtwork.LocalPath )
                        {
                            var lTvsWindow = GUIWindowManager.GetWindow( 9811 ) as TVSeriesPlugin;
                            TVSeriesPlugin.LoadFanart( lTvsWindow );
                        }
                    }
                    break;

                case ArtworkType.SeriesPoster:
                    // step 1: delete artwork from disk
                    if ( DeleteFile( lArtwork.LocalPath ) )
                    {
                        // step 2: remove from available artworks, pipe seperated relative paths
                        string lPath = "posters/" + Path.GetFileName( lArtwork.OnlinePath );
                        string lRelativePath = Helper.CleanLocalPath( lArtwork.Series.ToString() ) + @"\-lang" + lArtwork.Language + "-" + lPath;

                        var lArtworks = lArtwork.Series[DBOnlineSeries.cPosterFileNames].ToString().Split( '|' ).ToList();
                        lArtworks.RemoveAll( a => a.Contains( lRelativePath ) );

                        lArtwork.Series[DBOnlineSeries.cPosterFileNames] = string.Join( "|", lArtworks );
                        lArtwork.Series.Commit();

                        // step 3: mark as remote
                        lArtwork.IsLocal = false;
                        aSelectedGUIItem.Label2 = Translation.FanArtOnline;
                    }
                    break;

                case ArtworkType.SeriesBanner:
                    // step 1: delete artwork from disk
                    if ( DeleteFile( lArtwork.LocalPath ) )
                    {
                        // step 2: remove from available artworks, pipe seperated relative paths
                        string lPath = "graphical/" + Path.GetFileName( lArtwork.OnlinePath );
                        string lRelativePath = Helper.CleanLocalPath( lArtwork.Series.ToString() ) + @"\-lang" + lArtwork.Language + "-" + lPath;

                        var lArtworks = lArtwork.Series[DBOnlineSeries.cPosterFileNames].ToString().Split( '|' ).ToList();
                        lArtworks.RemoveAll( a => a.Contains( lRelativePath ) );

                        lArtwork.Series[DBOnlineSeries.cBannerFileNames] = string.Join( "|", lArtworks );
                        lArtwork.Series.Commit();

                        // step 3: mark as remote
                        lArtwork.IsLocal = false;
                        aSelectedGUIItem.Label2 = Translation.FanArtOnline;
                    }
                    break;

                case ArtworkType.SeasonPoster:
                    // step 1: delete artwork from disk
                    if ( DeleteFile( lArtwork.LocalPath ) )
                    {
                        // step 2: remove from available artworks, pipe seperated relative paths
                        string lPath = "seasons/" + Path.GetFileName( lArtwork.OnlinePath );
                        string lRelativePath = Helper.CleanLocalPath( lArtwork.Series.ToString() ) + @"\-lang" + lArtwork.Language + "-" + lPath;

                        var lArtworks = lArtwork.Season[DBSeason.cBannerFileNames].ToString().Split( '|' ).ToList();
                        lArtworks.RemoveAll( a => a.Contains( lRelativePath ) );

                        lArtwork.Season[DBSeason.cBannerFileNames] = string.Join( "|", lArtworks );
                        lArtwork.Season.Commit();

                        // step 3: mark as remote
                        lArtwork.IsLocal = false;
                        aSelectedGUIItem.Label2 = Translation.FanArtOnline;
                    }
                    break;

                case ArtworkType.EpisodeThumb:
                    // step 1: delete artwork from disk
                    if (DeleteFile(lArtwork.LocalPath))
                    {
                        // step 2: mark as remote
                        lArtwork.IsLocal = false;
                        aSelectedGUIItem.Label2 = Translation.FanArtOnline;
                    }
                    break;
            }
        }

        private void OnEpisodeThumbnailClicked()
        {
            var lSelectedItem = Facade.SelectedListItem as GUIArtworkListItem;
            if (lSelectedItem == null) return;

            var lArtwork = lSelectedItem.Item as Artwork;
            if (lArtwork == null) return;

            // if the item is currently downloading do nothing (maybe prompt to cancel later)
            if (lSelectedItem.IsDownloading) return;

            // if the item is default, then nothing to do
            if (lArtwork.IsDefault) return;

            // if the item is local and not default, make it the default
            if (lArtwork.IsLocal && !lArtwork.IsDefault)
            {
                // remove existing art as default
                var lOldDefault = GUIFacadeControl.GetListItem(GetID, Facade.GetID, DefaultArtIndex) as GUIArtworkListItem;
                lOldDefault.Label2 = Translation.FanArtLocal;
                lOldDefault.IsPlayed = false;
                (lOldDefault.Item as Artwork).IsDefault = false;

                // set the source of image in episode table
                lArtwork.Episode[DBOnlineEpisode.cTMDbEpisodeThumbnailUrl] = lArtwork.Provider == ArtworkDataProvider.TMDb ? lArtwork.OnlinePath : string.Empty;
                lArtwork.Episode[DBOnlineEpisode.cEpisodeThumbnailSource] = (int)lArtwork.Provider;
                lArtwork.Episode.Commit();

                // update new art to default and commit
                lArtwork.IsDefault = true;
                lArtwork.Episode[DBOnlineEpisode.cEpisodeThumbnailFilename] = lArtwork.Episode.BuildEpisodeThumbFilename();
                lArtwork.Episode.Commit();
                DefaultArtIndex = Facade.SelectedListItemIndex;

                // update facade
                lSelectedItem.Label2 = Translation.ArtworkSelected;
                lSelectedItem.IsPlayed = true;

                MPTVSeriesLog.Write($"Marking selected episode thumbnail '{lArtwork.LocalPath}' as selected");

                // update the main GUI property so affect is immediate on exit
                GUIPropertyManager.SetProperty("#TVSeries.EpisodeImage", lArtwork.LocalPath);
            }
            else if (!lArtwork.IsLocal)
            {
                MPTVSeriesLog.Write($"Selected episode thumbnail '{lArtwork.OnlinePath}' does not exist, downloading now");

                // the art it not local and we want to download it
                // start download in background and let user continue selecting art to download
                lArtwork.DownloadItemIndex = Facade.SelectedListItemIndex;
                StartDownload(lArtwork);
            }

            OnSelected(lSelectedItem, Facade);
        }

        private void OnSeriesPosterClicked()
        {
            var lSelectedItem = Facade.SelectedListItem as GUIArtworkListItem;
            if ( lSelectedItem == null ) return;

            var lArtwork = lSelectedItem.Item as Artwork;
            if ( lArtwork == null ) return;

            // if the item is currently downloading do nothing (maybe prompt to cancel later)
            if ( lSelectedItem.IsDownloading ) return;

            // if the item is default, then nothing to do
            if ( lArtwork.IsDefault ) return;
            
            // if the item is local and not default, make it the default
            if ( lArtwork.IsLocal && !lArtwork.IsDefault )
            {
                // remove existing art as default
                var lOldDefault = GUIFacadeControl.GetListItem( GetID, Facade.GetID, DefaultArtIndex ) as GUIArtworkListItem;
                lOldDefault.Label2 = Translation.FanArtLocal;
                lOldDefault.IsPlayed = false;
                ( lOldDefault.Item as Artwork ).IsDefault = false;

                // update new art to default and commit
                string lPath = "posters/" + Path.GetFileName( lArtwork.OnlinePath );
                string lRelativePath = Helper.CleanLocalPath( lArtwork.Series.ToString() ) + @"\-lang" + lArtwork.Language + "-" + lPath;

                lArtwork.IsDefault = true;
                lArtwork.Series[DBOnlineSeries.cCurrentPosterFileName] = lRelativePath;
                lArtwork.Series.Commit();
                DefaultArtIndex = Facade.SelectedListItemIndex;

                // update facade
                lSelectedItem.Label2 = Translation.ArtworkSelected;
                lSelectedItem.IsPlayed = true;

                MPTVSeriesLog.Write( $"Marking selected series poster '{lArtwork.LocalPath}' as selected" );

                // update the main GUI property so affect is immediate on exit
                GUIPropertyManager.SetProperty( "#TVSeries.SeriesPoster", lArtwork.LocalPath );
            }
            else if ( !lArtwork.IsLocal )
            {
                MPTVSeriesLog.Write( $"Selected series poster '{lArtwork.OnlinePath}' does not exist, downloading now" );

                // the art it not local and we want to download it
                // start download in background and let user continue selecting art to download
                lArtwork.DownloadItemIndex = Facade.SelectedListItemIndex;
                StartDownload( lArtwork );
            }

            OnSelected( lSelectedItem, Facade);
        }

        private void OnSeasonPosterClicked()
        {
            var lSelectedItem = Facade.SelectedListItem as GUIArtworkListItem;
            if ( lSelectedItem == null ) return;

            var lArtwork = lSelectedItem.Item as Artwork;
            if ( lArtwork == null ) return;

            // if the item is currently downloading do nothing (maybe prompt to cancel later)
            if ( lSelectedItem.IsDownloading ) return;

            // if the item is default, then nothing to do
            if ( lArtwork.IsDefault ) return;

            // if the item is local and not default, make it the default
            if ( lArtwork.IsLocal && !lArtwork.IsDefault )
            {
                // remove existing art as default
                var lOldDefault = GUIFacadeControl.GetListItem( GetID, Facade.GetID, DefaultArtIndex ) as GUIArtworkListItem;
                lOldDefault.Label2 = Translation.FanArtLocal;
                lOldDefault.IsPlayed = false;
                ( lOldDefault.Item as Artwork ).IsDefault = false;

                // update new art to default and commit
                string lPath = "seasons/" + Path.GetFileName( lArtwork.OnlinePath );
                string lRelativePath = Helper.CleanLocalPath( lArtwork.Series.ToString() ) + @"\-lang" + lArtwork.Language + "-" + lPath;

                lArtwork.IsDefault = true;
                lArtwork.Season[DBSeason.cCurrentBannerFileName] = lRelativePath;
                lArtwork.Season.Commit();
                DefaultArtIndex = Facade.SelectedListItemIndex;

                // update facade
                lSelectedItem.Label2 = Translation.ArtworkSelected;
                lSelectedItem.IsPlayed = true;

                MPTVSeriesLog.Write( $"Marking selected season poster '{lArtwork.LocalPath}' as selected" );

                // update the main GUI property so affect is immediate on exit
                GUIPropertyManager.SetProperty( "#TVSeries.SeasonPoster", lArtwork.LocalPath );
            }
            else if ( !lArtwork.IsLocal )
            {
                MPTVSeriesLog.Write( $"Selected season poster '{lArtwork.OnlinePath}' does not exist, downloading now" );

                // the art it not local and we want to download it
                // start download in background and let user continue selecting art to download
                lArtwork.DownloadItemIndex = Facade.SelectedListItemIndex;
                StartDownload( lArtwork );
            }

            OnSelected( lSelectedItem, Facade);
        }

        private void OnSeriesWideBannerClicked()
        {
            var lSelectedItem = Facade.SelectedListItem as GUIArtworkListItem;
            if ( lSelectedItem == null ) return;

            var lArtwork = lSelectedItem.Item as Artwork;
            if ( lArtwork == null ) return;

            // if the item is currently downloading do nothing (maybe prompt to cancel later)
            if ( lSelectedItem.IsDownloading ) return;

            // if the item is default, then nothing to do
            if ( lArtwork.IsDefault ) return;

            // if the item is local and not default, make it the default
            if ( lArtwork.IsLocal && !lArtwork.IsDefault )
            {
                // remove existing art as default
                var lOldDefault = GUIFacadeControl.GetListItem( GetID, Facade.GetID, DefaultArtIndex ) as GUIArtworkListItem;
                lOldDefault.Label2 = Translation.FanArtLocal;
                lOldDefault.IsPlayed = false;
                ( lOldDefault.Item as Artwork ).IsDefault = false;

                // update new art to default and commit
                string lPath = "graphical/" + Path.GetFileName( lArtwork.OnlinePath );
                string lRelativePath = Helper.CleanLocalPath( lArtwork.Series.ToString() ) + @"\-lang" + lArtwork.Language + "-" + lPath;

                lArtwork.IsDefault = true;
                lArtwork.Series[DBOnlineSeries.cCurrentBannerFileName] = lRelativePath;
                lArtwork.Series.Commit();
                DefaultArtIndex = Facade.SelectedListItemIndex;

                // update facade
                lSelectedItem.Label2 = Translation.ArtworkSelected;
                lSelectedItem.IsPlayed = true;

                MPTVSeriesLog.Write( $"Marking selected series widebanner '{lArtwork.LocalPath}' as selected" );

                // update the main GUI property so affect is immediate on exit
                GUIPropertyManager.SetProperty( "#TVSeries.SeriesBanner", lArtwork.LocalPath );
            }
            else if ( !lArtwork.IsLocal )
            {
                MPTVSeriesLog.Write( $"Selected series widebanner '{lArtwork.OnlinePath}' does not exist, downloading now" );

                // the art it not local and we want to download it
                // start download in background and let user continue selecting art to download
                lArtwork.DownloadItemIndex = Facade.SelectedListItemIndex;
                StartDownload( lArtwork );
            }

            OnSelected( lSelectedItem, Facade);
        }

        private void OnFanartClicked()
        {
            var lSelectedItem = Facade.SelectedListItem as GUIArtworkListItem;
            if ( lSelectedItem == null ) return;

            var lArtwork = lSelectedItem.Item as Artwork;
            if ( lArtwork == null ) return;

            // if the item is currently downloading do nothing (maybe prompt to cancel later)
            if ( lSelectedItem.IsDownloading ) return;

            // if the item is default, then nothing to do
            if ( lArtwork.IsDefault ) return;

            // we should have it but just in case
            if ( lArtwork.Fanart == null && lArtwork.Id != 0 )
            {
                var lFanart = new DBFanart( lArtwork.Id );
                lFanart[DBFanart.cBannerType] = "fanart";
                lFanart[DBFanart.cSeriesID] = ArtworkParams.SeriesId;
                lFanart[DBFanart.cBannerPath] = lArtwork.OnlinePath;
                lFanart[DBFanart.cThumbnailPath] = lArtwork.OnlineThumbPath;
                lFanart[DBFanart.cLanguage] = lArtwork.Language;
                lFanart[DBFanart.cRating] = lArtwork.Rating;
                lFanart[DBFanart.cRatingCount] = lArtwork.Votes;
                lFanart[DBFanart.cResolution] = lArtwork.Resolution;
                lFanart.Commit();

                MPTVSeriesLog.Write( "Selected fanart does not exist in database, creating entry" );

                lArtwork.Fanart = lFanart;
            }

            // if the item is local and not default, make it the default
            if ( lArtwork.IsLocal && !lArtwork.IsDefault )
            {
                // remove existing art as default
                var lOldDefault = GUIFacadeControl.GetListItem( GetID, Facade.GetID, DefaultArtIndex ) as GUIArtworkListItem;
                lOldDefault.Label2 = Translation.FanArtLocal;
                lOldDefault.IsPlayed = false;
                ( lOldDefault.Item as Artwork ).IsDefault = false;
                lArtwork.Fanart.Chosen = false;

                // update new art to default and commit
                lArtwork.IsDefault = true;
                lArtwork.Fanart.Chosen = true;
                DefaultArtIndex = Facade.SelectedListItemIndex;

                // update facade
                lSelectedItem.Label2 = Translation.ArtworkSelected;
                lSelectedItem.IsPlayed = true;

                MPTVSeriesLog.Write( $"Marking selected series fanart '{lArtwork.LocalPath}' as selected" );

                // update the current background
                var lTvsWindow = GUIWindowManager.GetWindow( 9811 ) as TVSeriesPlugin;
                TVSeriesPlugin.LoadFanart( lTvsWindow );

                // if using random fanart ignore rotated art in favour of chosen one
                if ( DBOption.GetOptions( DBOption.cFanartRandom ) )
                {
                    GUIPropertyManager.SetProperty( "#TVSeries.Current.Fanart", lArtwork.LocalPath );
                }
            }
            else if ( !lArtwork.IsLocal )
            {
                MPTVSeriesLog.Write( $"Selected series fanart '{lArtwork.OnlinePath}' does not exist, downloading now" );

                // the art is not local and we want to download it
                // start download in background and let user continue selecting art to download
                lArtwork.DownloadItemIndex = Facade.SelectedListItemIndex;
                StartDownload( lArtwork );
            }
            
            OnSelected( lSelectedItem, Facade);
        }

        private bool LoadParameters()
        {
            // _loadingParameter can be set by a plugin developer or a skin designer
            if ( string.IsNullOrEmpty( _loadParameter ) )
                return false;

            MPTVSeriesLog.Write( $"Deserialising loading parameter '{_loadParameter}'" );

            ArtworkParams = _loadParameter.FromJSON<ArtworkLoadingParameters>();
            if ( ArtworkParams == null ) return false;

            SetProperty( "SeriesID", ArtworkParams.SeriesId.ToString(), true );
            SetProperty( "SeriesName", ArtworkParams.Series.ToString(), true );
            SetProperty( "Type", ArtworkParams.Type.ToString(), true );
            SetProperty( "LocalisedType", GetArtworkTypeName(ArtworkParams.Type), true );
            if ( ArtworkParams.Type == ArtworkType.SeasonPoster || ArtworkParams.Type == ArtworkType.EpisodeThumb)
            {
                SetProperty( "SeasonIndex", ArtworkParams.SeasonIndex.ToString(), true );
            }
            if (ArtworkParams.Type == ArtworkType.EpisodeThumb)
            {
                SetProperty("EpisodeIndex", ArtworkParams.EpisodeIndex.ToString(), true);
            }
            SetProperty( "DataProvider", ArtworkParams.Provider.ToString(), true );
            return true;
        }

        private void DownloadArtworkThumbs()
        {
            GUIConnector.Instance.ExecuteInBackgroundAndCallback( () =>
            {
                switch (ArtworkParams.Provider)
                {
                    case ArtworkDataProvider.FanartTV:
                        return GetArtworkThumbsFromFanartTv();
                    default:
                        return GetArtworkThumbsFromTMDb();
                }
            },
            delegate ( bool success, object result )
            {
                if ( success )
                {
                    var lArtwork = result as List<Artwork>;
                    LoadFacade( lArtwork );
                }
            }, Translation.GettingArtwork, true );
        }

        private void GetFanartFromTMDb( List<TmdbImage> aImages, List<DBFanart> aFanarts, ref List<Artwork> aArtwork )
        {
            foreach ( var backdrop in aImages )
            {
                var lFanart = new Artwork();

                lFanart.Id = Math.Abs(backdrop.FilePath.GetHashCode()) * -1; /* make negative to avoid any clashes with thetvdb api*/
                lFanart.Language = backdrop.LanguageCode;
                lFanart.OnlinePath = "original" + backdrop.FilePath; /* make configurable */
                lFanart.OnlineThumbPath = "w780" + backdrop.FilePath; /* make configurable */
                lFanart.Resolution = $"{backdrop.Width}x{backdrop.Height}";
                lFanart.HasLogo = !string.IsNullOrEmpty(backdrop.LanguageCode);
                lFanart.Rating = backdrop.Score;
                lFanart.Votes = (uint)backdrop.Votes;

                // create the local filename path for the online thumbnail image e.g. _cache\fanart\original\<seriesId>-*.jpg
                string lLocalThumbPath = Fanart.GetLocalThumbPath( backdrop.FilePath, ArtworkParams.SeriesId.ToString() );
                lFanart.LocalThumbPath = Helper.PathCombine( Settings.GetPath( Settings.Path.fanart ), lLocalThumbPath.Replace( "/", @"\" ) );

                // create the local filename path for the online image e.g. fanart\original\<seriesId>-*.jpg
                string lLocalPath = Fanart.GetLocalPath( backdrop.FilePath, ArtworkParams.SeriesId.ToString() );
                lFanart.LocalPath = Helper.PathCombine( Settings.GetPath( Settings.Path.fanart ), lLocalPath.Replace( "/", @"\" ) );

                // if the fullsize artwork is already downloaded, then set it
                if ( File.Exists( lFanart.LocalPath ) ) lFanart.IsLocal = true;

                // get reference to fanart in the database if it exists
                var lDBFanart = aFanarts?.FirstOrDefault( f => f[DBFanart.cIndex] == lFanart.Id );

                if ( lDBFanart != null )
                {
                    // if the database has a local path, check if it exists
                    string lDBFanartLocalPath = lDBFanart[DBFanart.cLocalPath];
                    if ( !string.IsNullOrEmpty( lDBFanartLocalPath ) )
                    {
                        // check that the file exists
                        if ( !File.Exists( Helper.PathCombine( Settings.GetPath( Settings.Path.fanart ), lDBFanartLocalPath ) ) )
                        {
                            MPTVSeriesLog.Write( $"Removing local reference '{lDBFanartLocalPath}' from database as it no longer exists" );
                            lDBFanartLocalPath = string.Empty;
                            lDBFanart[DBFanart.cLocalPath] = string.Empty;
                            lDBFanart[DBFanart.cChosen] = false;
                            lDBFanart.Commit();
                        }

                        // check if the fanart is selected/defaulted
                        if ( lDBFanartLocalPath == lLocalPath && lDBFanart.Chosen )
                        {
                            lFanart.IsDefault = true;
                        }
                    }
                }
                else
                {
                    lDBFanart = new DBFanart( lFanart.Id );
                    lDBFanart[DBFanart.cDataSource] = "tmdb";
                    lDBFanart[DBFanart.cBannerType] = "fanart";
                    lDBFanart[DBFanart.cSeriesID] = ArtworkParams.SeriesId;
                    lDBFanart[DBFanart.cTmdbID] = ArtworkParams.SeriesId;
                    lDBFanart[DBFanart.cBannerPath] = lFanart.OnlinePath;
                    lDBFanart[DBFanart.cThumbnailPath] = lFanart.OnlineThumbPath;
                    lDBFanart[DBFanart.cLanguage] = lFanart.Language;
                    lDBFanart[DBFanart.cRating] = lFanart.Rating;
                    lDBFanart[DBFanart.cRatingCount] = lFanart.Votes;
                    lDBFanart[DBFanart.cResolution] = lFanart.Resolution;
                    lDBFanart[DBFanart.cSeriesName] = !string.IsNullOrEmpty( lFanart.Language );
                    // if the fanart is local set the path
                    if ( lFanart.IsLocal )
                    {
                        lDBFanart[DBFanart.cLocalPath] = Fanart.GetLocalPath( lFanart.OnlineThumbPath, ArtworkParams.SeriesId.ToString() );
                    }
                    lDBFanart.Commit();
                }

                // create full dowload url's
                string lBaseUrl = null;
                var lTmdbConfig =  DBOption.GetOptions( DBOption.cTmdbConfiguration ).ToString().FromJSON<TmdbConfiguration>();
                if (lTmdbConfig == null)
                {
                    lBaseUrl = "https://image.tmdb.org/t/p/";
                }
                else
                {
                    lBaseUrl = lTmdbConfig.Images.SecureBaseUrl;
                }

                lFanart.ThumbnailUrl = lBaseUrl + lFanart.OnlineThumbPath;
                lFanart.Url = lBaseUrl + lFanart.OnlinePath;

                lFanart.Type = ArtworkType.SeriesFanart;
                lFanart.Fanart = lDBFanart;
                lFanart.Series = ArtworkParams.Series;

                if ( !aArtwork.Contains( lFanart ) )
                    aArtwork.Add( lFanart );
            }
        }

        private void GetFanartFromFanartTv( List<FanartTvImage> aImages, List<DBFanart> aFanarts, ref List<Artwork> aArtwork )
        {
            if (aImages == null) return;

            foreach (var backdrop in aImages)
            {
                // filter out any languages we're not interesting in
                // we always want English and language neutral <empty>
                if (!string.IsNullOrEmpty(backdrop.Language) && backdrop.Language != "en")
                {
                    string lSeriesLanguage = OnlineAPI.GetSeriesLanguage(ArtworkParams.SeriesId);
                    if (backdrop.Language != lSeriesLanguage)
                    {
                        MPTVSeriesLog.Write($"Filtering out image with language code '{backdrop.Language}', current language set to '{lSeriesLanguage}'");
                        continue;
                    }
                }

                var lFanart = new Artwork();

                string lOnlineFilePath = backdrop.Url.Replace("https://assets.fanart.tv/", "");

                lFanart.Id = backdrop.Id * -1; /* make negative to avoid any clashes with thetvdb api*/
                lFanart.Language = backdrop.Language;
                lFanart.OnlinePath = lOnlineFilePath;
                lFanart.OnlineThumbPath = lOnlineFilePath.Replace("fanart/", "preview/");
                lFanart.HasLogo = false;
                lFanart.Rating = backdrop.Likes;
                lFanart.Votes = (uint)backdrop.Likes;

                // create the local filename path for the online thumbnail image e.g. _cache\fanart\original\<seriesId>-*.jpg
                string lLocalThumbPath = Fanart.GetLocalThumbPath(lOnlineFilePath, ArtworkParams.SeriesId.ToString());
                lFanart.LocalThumbPath = Helper.PathCombine(Settings.GetPath(Settings.Path.fanart), lLocalThumbPath.Replace("/", @"\"));

                // create the local filename path for the online image e.g. fanart\original\<seriesId>-*.jpg
                string lLocalPath = Fanart.GetLocalPath(lOnlineFilePath, ArtworkParams.SeriesId.ToString());
                lFanart.LocalPath = Helper.PathCombine(Settings.GetPath(Settings.Path.fanart), lLocalPath.Replace("/", @"\"));

                // if the fullsize artwork is already downloaded, then set it
                if (File.Exists(lFanart.LocalPath)) lFanart.IsLocal = true;

                // get reference to fanart in the database if it exists
                var lDBFanart = aFanarts?.FirstOrDefault(f => f[DBFanart.cIndex] == lFanart.Id);

                if (lDBFanart != null)
                {
                    // if the database has a local path, check if it exists
                    string lDBFanartLocalPath = lDBFanart[DBFanart.cLocalPath];
                    if (!string.IsNullOrEmpty(lDBFanartLocalPath))
                    {
                        // check that the file exists
                        if (!File.Exists(Helper.PathCombine(Settings.GetPath(Settings.Path.fanart), lDBFanartLocalPath)))
                        {
                            MPTVSeriesLog.Write($"Removing local reference '{lDBFanartLocalPath}' from database as it no longer exists");
                            lDBFanartLocalPath = string.Empty;
                            lDBFanart[DBFanart.cLocalPath] = string.Empty;
                            lDBFanart[DBFanart.cChosen] = false;
                            lDBFanart.Commit();
                        }

                        // check if the fanart is selected/defaulted
                        if (lDBFanartLocalPath == lLocalPath && lDBFanart.Chosen)
                        {
                            lFanart.IsDefault = true;
                        }
                    }
                }
                else
                {
                    lDBFanart = new DBFanart(lFanart.Id);
                    lDBFanart[DBFanart.cDataSource] = "fanart.tv";
                    lDBFanart[DBFanart.cBannerType] = "fanart";
                    lDBFanart[DBFanart.cSeriesID] = ArtworkParams.SeriesId;
                    lDBFanart[DBFanart.cBannerPath] = lFanart.OnlinePath;
                    lDBFanart[DBFanart.cThumbnailPath] = lFanart.OnlineThumbPath;
                    lDBFanart[DBFanart.cLanguage] = lFanart.Language;
                    lDBFanart[DBFanart.cRating] = lFanart.Rating;
                    lDBFanart[DBFanart.cRatingCount] = lFanart.Votes;
                    lDBFanart[DBFanart.cResolution] = lFanart.Resolution;
                    lDBFanart[DBFanart.cSeriesName] = !string.IsNullOrEmpty(lFanart.Language);
                    // if the fanart is local set the path
                    if (lFanart.IsLocal)
                    {
                        lDBFanart[DBFanart.cLocalPath] = Fanart.GetLocalPath(lFanart.OnlineThumbPath, ArtworkParams.SeriesId.ToString());
                    }
                    lDBFanart.Commit();
                }

                lFanart.ThumbnailUrl = backdrop.Url.Replace("fanart/", "preview/");
                lFanart.Url = backdrop.Url;

                lFanart.Type = ArtworkType.SeriesFanart;
                lFanart.Fanart = lDBFanart;
                lFanart.Series = ArtworkParams.Series;

                if (!aArtwork.Contains(lFanart))
                    aArtwork.Add(lFanart);
            }
        }

        private void GetSeriesPostersFromTMDb( List<TmdbImage> aImages, ref List<Artwork> aArtwork )
        {
            foreach ( var poster in aImages )
            {
                var lPoster = new Artwork();

                lPoster.Language = poster.LanguageCode;
                lPoster.OnlinePath = "original" + poster.FilePath; /* make configurable */
                lPoster.OnlineThumbPath = "w342" + poster.FilePath; /* make configurable */
                lPoster.Resolution = $"{poster.Width}x{poster.Height}";
                lPoster.Rating = poster.Score;
                lPoster.Votes = ( uint )poster.Votes;

                // create the local filename path for the online thumbnail image e.g. 13 Reasons Why\Thumbnails\-langen-posters\5aecac0f66076_t.jpg
                string lThumbPath = "posters/" + Path.GetFileName( lPoster.OnlineThumbPath );
                string lRelativeThumbPath = Helper.CleanLocalPath( ArtworkParams.Series.ToString() ) + @"\Thumbnails\-lang" + lPoster.Language + "-" + lThumbPath;
                lPoster.LocalThumbPath = Helper.PathCombine( Settings.GetPath( Settings.Path.banners ), lRelativeThumbPath );

                // create the local filename path for the online image
                string lPath = "posters/" + Path.GetFileName( lPoster.OnlinePath );
                string lRelativePath = Helper.CleanLocalPath( ArtworkParams.Series.ToString() ) + @"\-lang" + lPoster.Language + "-" + lPath;
                lPoster.LocalPath = Helper.PathCombine( Settings.GetPath( Settings.Path.banners ), lRelativePath );

                // if the fullsize artwork is already downloaded, then set it
                if ( File.Exists( lPoster.LocalPath ) ) lPoster.IsLocal = true;

                // check that local posters exist in available list, if not add it
                if ( lPoster.IsLocal )
                {
                    var lAvailablePosters = ArtworkParams.Series[DBOnlineSeries.cPosterFileNames].ToString();
                    if ( !lAvailablePosters.Contains( lRelativePath ) )
                    {
                        MPTVSeriesLog.Write( "Added missing local poster to available posters" );
                        ArtworkParams.Series[DBOnlineSeries.cPosterFileNames] = lAvailablePosters += "|" + lRelativePath;
                        ArtworkParams.Series.Commit();
                    }
                }

                // if the artwork is default/selected, then set it
                // remove any inconsistency with slashes, it should still be unique
                if ( lRelativePath.Replace( "\\", "" ).Replace( "/", "" ) == ArtworkParams.Series[DBOnlineSeries.cCurrentPosterFileName].ToString().Replace( "\\", "" ).Replace( "/", "" ) )
                    lPoster.IsDefault = true;

                // create full dowload url's
                string lBaseUrl = null;
                var lTmdbConfig = DBOption.GetOptions( DBOption.cTmdbConfiguration ).ToString().FromJSON<TmdbConfiguration>();
                if ( lTmdbConfig == null )
                {
                    lBaseUrl = "https://image.tmdb.org/t/p/";
                }
                else
                {
                    lBaseUrl = lTmdbConfig.Images.SecureBaseUrl;
                }

                lPoster.ThumbnailUrl = lBaseUrl + lPoster.OnlineThumbPath;
                lPoster.Url = lBaseUrl + lPoster.OnlinePath;

                lPoster.Type = ArtworkType.SeriesPoster;
                lPoster.Series = ArtworkParams.Series;

                if ( !aArtwork.Contains( lPoster ) )
                    aArtwork.Add( lPoster );
            }
        }

        private void GetSeriesPostersFromFanartTv( List<FanartTvImage> aImages, ref List<Artwork> aArtwork )
        {
            if (aImages == null) return;

            foreach (var poster in aImages)
            {
                // filter out any languages we're not interesting in
                // we always want English and language neutral <empty>
                if (!string.IsNullOrEmpty(poster.Language) && poster.Language != "en")
                {
                    string lSeriesLanguage = OnlineAPI.GetSeriesLanguage(ArtworkParams.SeriesId);
                    if (poster.Language != lSeriesLanguage)
                    {
                        MPTVSeriesLog.Write($"Filtering out image with language code '{poster.Language}', current language set to '{lSeriesLanguage}'");
                        continue;
                    }
                }

                var lPoster = new Artwork();

                string lOnlineFilePath = poster.Url.Replace("https://assets.fanart.tv/", "");

                lPoster.Language = poster.Language;
                lPoster.OnlinePath = lOnlineFilePath;
                lPoster.OnlineThumbPath = lOnlineFilePath.Replace("fanart/", "preview/");
                lPoster.Rating = poster.Likes;
                lPoster.Votes = (uint)poster.Likes;

                // create the local filename path for the online thumbnail image e.g. 13 Reasons Why\Thumbnails\-langen-posters\5aecac0f66076_t.jpg
                string lThumbPath = "posters/" + Path.GetFileName(lPoster.OnlineThumbPath);
                string lRelativeThumbPath = Helper.CleanLocalPath(ArtworkParams.Series.ToString()) + @"\Thumbnails\-lang" + lPoster.Language + "-" + lThumbPath;
                lPoster.LocalThumbPath = Helper.PathCombine(Settings.GetPath(Settings.Path.banners), lRelativeThumbPath);

                // create the local filename path for the online image
                string lPath = "posters/" + Path.GetFileName(lPoster.OnlinePath);
                string lRelativePath = Helper.CleanLocalPath(ArtworkParams.Series.ToString()) + @"\-lang" + lPoster.Language + "-" + lPath;
                lPoster.LocalPath = Helper.PathCombine(Settings.GetPath(Settings.Path.banners), lRelativePath);

                // if the fullsize artwork is already downloaded, then set it
                if (File.Exists(lPoster.LocalPath)) lPoster.IsLocal = true;

                // check that local posters exist in available list, if not add it
                if (lPoster.IsLocal)
                {
                    var lAvailablePosters = ArtworkParams.Series[DBOnlineSeries.cPosterFileNames].ToString();
                    if (!lAvailablePosters.Contains(lRelativePath))
                    {
                        MPTVSeriesLog.Write("Added missing local poster to available posters");
                        ArtworkParams.Series[DBOnlineSeries.cPosterFileNames] = lAvailablePosters += "|" + lRelativePath;
                        ArtworkParams.Series.Commit();
                    }
                }

                // if the artwork is default/selected, then set it
                // remove any inconsistency with slashes, it should still be unique
                if (lRelativePath.Replace("\\", "").Replace("/", "") == ArtworkParams.Series[DBOnlineSeries.cCurrentPosterFileName].ToString().Replace("\\", "").Replace("/", ""))
                    lPoster.IsDefault = true;

                lPoster.ThumbnailUrl = poster.Url.Replace("fanart/", "preview/");
                lPoster.Url = poster.Url;

                lPoster.Type = ArtworkType.SeriesPoster;
                lPoster.Series = ArtworkParams.Series;

                if (!aArtwork.Contains(lPoster))
                    aArtwork.Add(lPoster);
            }
        }

        private void GetSeriesWideBannersFromFanartTv( List<FanartTvImage> aImages, ref List<Artwork> aArtwork )
        {
            if (aImages == null) return;

            foreach (var banner in aImages)
            {
                // filter out any languages we're not interesting in
                // we always want English and language neutral <empty>
                if (!string.IsNullOrEmpty(banner.Language) && banner.Language != "en")
                {
                    string lSeriesLanguage = OnlineAPI.GetSeriesLanguage(ArtworkParams.SeriesId);
                    if (banner.Language != lSeriesLanguage)
                    {
                        MPTVSeriesLog.Write($"Filtering out image with language code '{banner.Language}', current language set to '{lSeriesLanguage}'");
                        continue;
                    }
                }

                var lWideBanner = new Artwork();

                string lOnlineFilePath = banner.Url.Replace("https://assets.fanart.tv/", "");

                lWideBanner.Language = banner.Language;
                lWideBanner.OnlinePath = lOnlineFilePath;
                lWideBanner.OnlineThumbPath = lOnlineFilePath.Replace("fanart/", "preview/");
                lWideBanner.Rating = banner.Likes;
                lWideBanner.Votes = (uint)banner.Likes;

                // create the local filename path for the online thumbnail image e.g. 13 Reasons Why\Thumbnails\-langen-graphical\5aecac0f66076_t.jpg
                string lThumbPath = "graphical/" + Path.GetFileName(lWideBanner.OnlineThumbPath);
                string lRelativeThumbPath = Helper.CleanLocalPath(ArtworkParams.Series.ToString()) + @"\Thumbnails\-lang" + lWideBanner.Language + "-" + lThumbPath;
                lWideBanner.LocalThumbPath = Helper.PathCombine(Settings.GetPath(Settings.Path.banners), lRelativeThumbPath);

                // create the local filename path for the online image
                string lPath = "graphical/" + Path.GetFileName(lWideBanner.OnlinePath);
                string lRelativePath = Helper.CleanLocalPath(ArtworkParams.Series.ToString()) + @"\-lang" + lWideBanner.Language + "-" + lPath;
                lWideBanner.LocalPath = Helper.PathCombine(Settings.GetPath(Settings.Path.banners), lRelativePath);

                // if the fullsize artwork is already downloaded, then set it
                if (File.Exists(lWideBanner.LocalPath)) lWideBanner.IsLocal = true;

                // check that local widebanners exist in available list, if not add it
                if (lWideBanner.IsLocal)
                {
                    var lAvailableBanners = ArtworkParams.Series[DBOnlineSeries.cBannerFileNames].ToString();
                    if (!lAvailableBanners.Contains(lRelativePath))
                    {
                        MPTVSeriesLog.Write("Added missing local widebanner to available banners");
                        ArtworkParams.Series[DBOnlineSeries.cBannerFileNames] = lAvailableBanners += "|" + lRelativePath;
                        ArtworkParams.Series.Commit();
                    }
                }

                // if the artwork is default/selected, then set it
                // remove any inconsistency with slashes, it should still be unique
                if (lRelativePath.Replace("\\", "").Replace("/", "") == ArtworkParams.Series[DBOnlineSeries.cCurrentBannerFileName].ToString().Replace("\\", "").Replace("/", ""))
                    lWideBanner.IsDefault = true;

                lWideBanner.ThumbnailUrl = banner.Url.Replace("fanart/", "preview/");
                lWideBanner.Url = banner.Url;

                lWideBanner.Type = ArtworkType.SeriesPoster;
                lWideBanner.Series = ArtworkParams.Series;

                if (!aArtwork.Contains(lWideBanner))
                    aArtwork.Add(lWideBanner);
            }
        }

        private void GetSeasonPostersFromTMDb( List<TmdbImage> aImages, ref List<Artwork> aArtwork )
        {
            foreach ( var poster in aImages )
            {
                var lSeasonPoster = new Artwork();

                lSeasonPoster.Language = poster.LanguageCode;
                lSeasonPoster.OnlinePath = "original" + poster.FilePath; /* make configurable */
                lSeasonPoster.OnlineThumbPath = "w342" + poster.FilePath; /* make configurable */
                lSeasonPoster.Resolution = $"{poster.Width}x{poster.Height}";
                lSeasonPoster.Rating = poster.Score;
                lSeasonPoster.Votes = ( uint )poster.Votes;

                // create the local filename path for the online thumbnail image e.g. 13 Reasons Why\Thumbnails\-langen-seasons\5aecac0f66076_t.jpg
                string lThumbPath = "seasons/" + Path.GetFileName( lSeasonPoster.OnlineThumbPath );
                string lRelativePath = Helper.CleanLocalPath( ArtworkParams.Series.ToString() ) + @"\Thumbnails\-lang" + lSeasonPoster.Language + "-" + lThumbPath;
                lSeasonPoster.LocalThumbPath = Helper.PathCombine( Settings.GetPath( Settings.Path.banners ), lRelativePath );

                // create the local filename path for the online image
                string lPath = "seasons/" + Path.GetFileName( lSeasonPoster.OnlinePath );
                lRelativePath = Helper.CleanLocalPath( ArtworkParams.Series.ToString() ) + @"\-lang" + lSeasonPoster.Language + "-" + lPath;
                lSeasonPoster.LocalPath = Helper.PathCombine( Settings.GetPath( Settings.Path.banners ), lRelativePath );

                // if the fullsize artwork is already downloaded, then set it
                if ( File.Exists( lSeasonPoster.LocalPath ) ) lSeasonPoster.IsLocal = true;

                // check that local posters exist in available list, if not add it
                if ( lSeasonPoster.IsLocal )
                {
                    var lAvailablePosters = ArtworkParams.Season[DBSeason.cBannerFileNames].ToString();
                    if ( !lAvailablePosters.Contains( lRelativePath ) )
                    {
                        MPTVSeriesLog.Write( "Added missing local season poster to available posters" );
                        ArtworkParams.Season[DBSeason.cBannerFileNames] = lAvailablePosters += "|" + lRelativePath;
                        ArtworkParams.Season.Commit();
                    }
                }

                // if the artwork is default/selected, then set it
                // remove any inconsistency with slashes, it should still be unique
                if ( lRelativePath.Replace( "\\", "" ).Replace( "/", "" ) == ArtworkParams.Series[DBOnlineSeries.cCurrentPosterFileName].ToString().Replace( "\\", "" ).Replace( "/", "" ) )
                    lSeasonPoster.IsDefault = true;

                // create full dowload url's
                string lBaseUrl = null;
                var lTmdbConfig = DBOption.GetOptions( DBOption.cTmdbConfiguration ).ToString().FromJSON<TmdbConfiguration>();
                if ( lTmdbConfig == null )
                {
                    lBaseUrl = "https://image.tmdb.org/t/p/";
                }
                else
                {
                    lBaseUrl = lTmdbConfig.Images.SecureBaseUrl;
                }

                lSeasonPoster.ThumbnailUrl = lBaseUrl + lSeasonPoster.OnlineThumbPath;
                lSeasonPoster.Url = lBaseUrl + lSeasonPoster.OnlinePath;

                lSeasonPoster.Type = ArtworkType.SeasonPoster;
                lSeasonPoster.Series = ArtworkParams.Series;
                lSeasonPoster.Season = ArtworkParams.Season;

                if ( !aArtwork.Contains( lSeasonPoster ) )
                    aArtwork.Add( lSeasonPoster );
            }
        }

        private void GetSeasonPostersFromFanartTv( List<FanartTvSeasonImage> aImages, ref List<Artwork> aArtwork )
        {
            if (aImages == null) return;

            foreach (var poster in aImages)
            {
                // filter out any languages we're not interesting in
                // we always want English and language neutral <empty>
                if (!string.IsNullOrEmpty(poster.Language) && poster.Language != "en")
                {
                    string lSeriesLanguage = OnlineAPI.GetSeriesLanguage(ArtworkParams.SeriesId);
                    if (poster.Language != lSeriesLanguage)
                    {
                        MPTVSeriesLog.Write($"Filtering out image with language code '{poster.Language}', current language set to '{lSeriesLanguage}'");
                        continue;
                    }
                }

                var lSeasonPoster = new Artwork();
                
                string lOnlineFilePath = poster.Url.Replace("https://assets.fanart.tv/", "");

                lSeasonPoster.Language = poster.Language;
                lSeasonPoster.OnlinePath = lOnlineFilePath;
                lSeasonPoster.OnlineThumbPath = lOnlineFilePath.Replace( "fanart/", "preview/" );
                lSeasonPoster.Rating = poster.Likes;
                lSeasonPoster.Votes = (uint)poster.Likes;

                // create the local filename path for the online thumbnail image e.g. 13 Reasons Why\Thumbnails\-langen-seasons\5aecac0f66076_t.jpg
                string lThumbPath = "seasons/" + Path.GetFileName(lSeasonPoster.OnlineThumbPath);
                string lRelativePath = Helper.CleanLocalPath(ArtworkParams.Series.ToString()) + @"\Thumbnails\-lang" + lSeasonPoster.Language + "-" + lThumbPath;
                lSeasonPoster.LocalThumbPath = Helper.PathCombine(Settings.GetPath(Settings.Path.banners), lRelativePath);

                // create the local filename path for the online image
                string lPath = "seasons/" + Path.GetFileName(lSeasonPoster.OnlinePath);
                lRelativePath = Helper.CleanLocalPath(ArtworkParams.Series.ToString()) + @"\-lang" + lSeasonPoster.Language + "-" + lPath;
                lSeasonPoster.LocalPath = Helper.PathCombine(Settings.GetPath(Settings.Path.banners), lRelativePath);

                // if the fullsize artwork is already downloaded, then set it
                if (File.Exists(lSeasonPoster.LocalPath)) lSeasonPoster.IsLocal = true;

                // check that local posters exist in available list, if not add it
                if (lSeasonPoster.IsLocal)
                {
                    var lAvailablePosters = ArtworkParams.Season[DBSeason.cBannerFileNames].ToString();
                    if (!lAvailablePosters.Contains(lRelativePath))
                    {
                        MPTVSeriesLog.Write("Added missing local season poster to available posters");
                        ArtworkParams.Season[DBSeason.cBannerFileNames] = lAvailablePosters += "|" + lRelativePath;
                        ArtworkParams.Season.Commit();
                    }
                }

                // if the artwork is default/selected, then set it
                // remove any inconsistency with slashes, it should still be unique
                if (lRelativePath.Replace("\\", "").Replace("/", "") == ArtworkParams.Series[DBOnlineSeries.cCurrentPosterFileName].ToString().Replace("\\", "").Replace("/", ""))
                    lSeasonPoster.IsDefault = true;

                lSeasonPoster.ThumbnailUrl = poster.Url.Replace( "fanart/", "preview/" );
                lSeasonPoster.Url = poster.Url;

                lSeasonPoster.Type = ArtworkType.SeasonPoster;
                lSeasonPoster.Series = ArtworkParams.Series;
                lSeasonPoster.Season = ArtworkParams.Season;

                if (!aArtwork.Contains(lSeasonPoster))
                    aArtwork.Add(lSeasonPoster);
            }
        }

        private void GetEpisodeThumbnailsFromTMDb( List<TmdbImage> aImages, ref List<Artwork> aArtwork )
        {
            foreach (var episodeImage in aImages)
            {
                var lEpisodeImage = new Artwork
                {
                    Language = episodeImage.LanguageCode,
                    OnlinePath = "original" + episodeImage.FilePath, /* make configurable */
                    OnlineThumbPath = "w300" + episodeImage.FilePath, /* make configurable */
                    Resolution = $"{episodeImage.Width}x{episodeImage.Height}",
                    Rating = episodeImage.Score,
                    Votes = (uint)episodeImage.Votes,
                    Type = ArtworkType.EpisodeThumb,
                    Series = ArtworkParams.Series,
                    Season = ArtworkParams.Season,
                    Episode = ArtworkParams.Episode,
                    Provider = ArtworkDataProvider.TMDb
                };

                string lEpisodeId = $"{ArtworkParams.Episode[DBOnlineEpisode.cSeasonIndex]}x{ArtworkParams.Episode[DBOnlineEpisode.cEpisodeIndex]}";

                // create the local filename path for the online thumbnail image e.g. 13 Reasons Why\Thumbnails\episodes\tmdb\1x1_enSKInSK5OpHuLlAWOy6cB6CBmJ.jpg
                string lThumbPath = $"/Thumbnails/Episodes/tmdb/{lEpisodeId}_" + Path.GetFileName(lEpisodeImage.OnlineThumbPath);
                string lRelativeThumbPath = Helper.CleanLocalPath(ArtworkParams.Series.ToString()) + lThumbPath;
                lEpisodeImage.LocalThumbPath = Helper.PathCombine(Settings.GetPath(Settings.Path.banners), lRelativeThumbPath);

                // create the local filename path for the online image
                string lPath = $"/Episodes/{lEpisodeId}_" + Path.GetFileName(lEpisodeImage.OnlinePath);
                string lRelativePath = Helper.CleanLocalPath(ArtworkParams.Series.ToString()) + lPath;
                lEpisodeImage.LocalPath = Helper.PathCombine(Settings.GetPath(Settings.Path.banners), lRelativePath);

                // if the fullsize artwork is already downloaded, then set it
                if (File.Exists(lEpisodeImage.LocalPath)) lEpisodeImage.IsLocal = true;

                // if the artwork is default/selected, then set it
                if (ArtworkParams.Episode[DBOnlineEpisode.cEpisodeThumbnailFilename] == lRelativePath.Replace("/", @"\"))
                    lEpisodeImage.IsDefault = true;

                // create full dowload url's
                lEpisodeImage.ThumbnailUrl = OnlineAPI.GetTMDbBasePath() + lEpisodeImage.OnlineThumbPath;
                lEpisodeImage.Url = OnlineAPI.GetTMDbBasePath() + lEpisodeImage.OnlinePath;

                if (!aArtwork.Contains(lEpisodeImage))
                    aArtwork.Add(lEpisodeImage);
            }
        }

        private List<Artwork> GetArtworkThumbsFromTMDb()
        {
            var lArtwork = new List<Artwork>();
            
            // first check if we have the TMDb ID for the series
            int lTmdbId = ArtworkParams.Series[DBOnlineSeries.cTmdbId];
            if ( lTmdbId <= 0 )
            {
                // we don't have it, let's search for it and save it for next time
                // there should only be one result for a tvdb ID.
                TmdbFindResult lResults = TmdbAPI.TmdbAPI.TmdbFind(ArtworkParams.Series[DBOnlineSeries.cID], ExternalSource.tvdb_id);
                if ( lResults == null || lResults.Shows == null || lResults.Shows.Count == 0 )
                {
                    // report to facade nothing to do
                    return null;
                }
                
                lTmdbId = lResults.Shows.FirstOrDefault().Id;
                MPTVSeriesLog.Write( $"Found TMDb ID '{lTmdbId}' for tv show '{ArtworkParams.Series[DBOnlineSeries.cPrettyName]}' with TVDb Id '{ArtworkParams.SeriesId}'" );

                // save it for next time
                ArtworkParams.Series[DBOnlineSeries.cTmdbId] = lTmdbId;
                ArtworkParams.Series.Commit();
            }

            string lLanguages = "en,null";
            string lSeriesLanguage = OnlineAPI.GetSeriesLanguage( ArtworkParams.SeriesId );
            if ( lSeriesLanguage != "en" )
            {
                lLanguages = $"{lSeriesLanguage},en,null";
            }

            switch ( ArtworkParams.Type )
            {
                #region Series Fanart
                case ArtworkType.SeriesFanart:
                    var lShowImages = TmdbAPI.TmdbAPI.GetShowImages( lTmdbId.ToString(), lLanguages );

                    // get fanart from database table
                    DBFanart.ClearSeriesFromCache( ArtworkParams.SeriesId );
                    var lDBFanarts = DBFanart.GetAll( ArtworkParams.SeriesId, false );

                    GetFanartFromTMDb( lShowImages?.Backdrops, lDBFanarts, ref lArtwork );

                    lArtwork.Sort( new GUIListItemSorter( SortingFields.Score, SortingDirections.Descending ) );
                    return lArtwork;
                #endregion

                #region Series Posters
                case ArtworkType.SeriesPoster:
                    lShowImages = TmdbAPI.TmdbAPI.GetShowImages( lTmdbId.ToString(), lLanguages );
                    GetSeriesPostersFromTMDb( lShowImages?.Posters, ref lArtwork );
                    lArtwork.Sort( new GUIListItemSorter( SortingFields.Score, SortingDirections.Descending ) );
                    return lArtwork;
                #endregion

                #region Season Posters
                case ArtworkType.SeasonPoster:
                    var lSeasonImages = TmdbAPI.TmdbAPI.GetSeasonImages( lTmdbId.ToString(), ArtworkParams.SeasonIndex, lLanguages );
                    GetSeasonPostersFromTMDb( lSeasonImages?.Posters, ref lArtwork );
                    lArtwork.Sort( new GUIListItemSorter( SortingFields.Score, SortingDirections.Descending ) );
                    return lArtwork;
                #endregion

                case ArtworkType.EpisodeThumb:
                    var lEpisodeImages = TmdbAPI.TmdbAPI.GetEpisodeImages(lTmdbId.ToString(), ArtworkParams.SeasonIndex, ArtworkParams.EpisodeIndex, lLanguages);
                    GetEpisodeThumbnailsFromTMDb(lEpisodeImages?.Stills, ref lArtwork);
                    lArtwork.Sort(new GUIListItemSorter(SortingFields.Score, SortingDirections.Descending));
                    return lArtwork;

                default:
                    return null;
            }
        }

        private List<Artwork> GetArtworkThumbsFromFanartTv()
        {
            var lArtwork = new List<Artwork>();

            switch (ArtworkParams.Type)
            {
                #region Series Fanart
                case ArtworkType.SeriesFanart:
                    var lShowImages = FanartTvAPI.FanartTvAPI.GetShowImages(ArtworkParams.SeriesId.ToString());

                    // get fanart from database table
                    DBFanart.ClearSeriesFromCache(ArtworkParams.SeriesId);
                    var lDBFanarts = DBFanart.GetAll(ArtworkParams.SeriesId, false);

                    GetFanartFromFanartTv(lShowImages?.TvShowBackgrounds, lDBFanarts, ref lArtwork);

                    lArtwork.Sort(new GUIListItemSorter(SortingFields.Votes, SortingDirections.Descending));
                    return lArtwork;
                #endregion

                #region Series Posters
                case ArtworkType.SeriesPoster:
                    lShowImages = FanartTvAPI.FanartTvAPI.GetShowImages(ArtworkParams.SeriesId.ToString());
                    GetSeriesPostersFromFanartTv(lShowImages?.TvPosters, ref lArtwork);
                    lArtwork.Sort(new GUIListItemSorter(SortingFields.Votes, SortingDirections.Descending));
                    return lArtwork;
                #endregion

                #region Series Widebanners
                case ArtworkType.SeriesBanner:
                    lShowImages = FanartTvAPI.FanartTvAPI.GetShowImages(ArtworkParams.SeriesId.ToString());
                    GetSeriesWideBannersFromFanartTv(lShowImages?.TvBanners, ref lArtwork);
                    lArtwork.Sort(new GUIListItemSorter(SortingFields.Votes, SortingDirections.Descending));
                    return lArtwork;
                #endregion

                #region Season Posters
                case ArtworkType.SeasonPoster:
                    lShowImages = FanartTvAPI.FanartTvAPI.GetShowImages(ArtworkParams.SeriesId.ToString());
                    GetSeasonPostersFromFanartTv(lShowImages?.TvSeasonPosters?.Where(i => i.Season == ArtworkParams.SeasonIndex.ToString())?.ToList(), ref lArtwork);
                    lArtwork.Sort(new GUIListItemSorter(SortingFields.Votes, SortingDirections.Descending));
                    return lArtwork;
                #endregion

                case ArtworkType.EpisodeThumb:
                    return null;

                default:
                    return null;
            }
        }

        private void LoadFacade( List<Artwork> aArtwork )
        {
            // clear facade
            GUIControl.ClearControl( GetID, Facade.GetID );

            // notify user if no thumbs to display, let them choose a different data provider
            if ( aArtwork == null || aArtwork.Count == 0 )
            {
                MPTVSeriesLog.Write( $"No '{ArtworkParams.Type}' artwork available for '{ArtworkParams.Series[DBOnlineSeries.cSeriesID]}' from provider '{ArtworkParams.Provider}'" );

                var lNoItem = new GUIListItem( string.Format(Translation.NoArtworkAvailable, GetArtworkTypeName(ArtworkParams.Type), GetDataProviderNameFromEnum( ArtworkParams.Provider) ) );
                lNoItem.IconImage = GetDefaultImage();
                lNoItem.IconImageBig = GetDefaultImage();
                lNoItem.ThumbnailImage = GetDefaultImage();
                lNoItem.OnItemSelected += OnSelected;
                Utils.SetDefaultIcons( lNoItem );
                Facade.Add( lNoItem );

                Facade.CurrentLayout = GUIFacadeControl.Layout.List;
                GUIControl.FocusControl( GetID, Facade.GetID );
                Facade.SelectedListItemIndex = 0;

                GUIPropertyManager.SetProperty( "#itemcount", "0" );
                return;
            }

            // set number of artwork available in skin property
            GUIPropertyManager.SetProperty( "#itemcount", aArtwork.Count.ToString() );

            int lSelectedIndex = 0;

            // Add each artwork thumbnail to the list
            foreach ( var lItem in aArtwork )
            {
                var lArtworkItem = new GUIArtworkListItem( Path.GetFileName( lItem.OnlinePath ) );

                lArtworkItem.Item = lItem;
                lArtworkItem.Label2 = GetLabelTwo( lItem );
                lArtworkItem.IsPlayed = lItem.IsDefault;
                lArtworkItem.IconImage = GetDefaultImage();
                lArtworkItem.IconImageBig = GetDefaultImage();
                lArtworkItem.ThumbnailImage = GetDefaultImage();
                lArtworkItem.OnItemSelected += OnSelected;
                lArtworkItem.HasProgressBar = true;
                Utils.SetDefaultIcons( lArtworkItem );

                Facade.Add( lArtworkItem );

                // if default, get index
                if ( lItem.IsDefault )
                {
                    lSelectedIndex = Facade.Count - 1;
                    MPTVSeriesLog.Write( "Setting default artwork at selected index " + lSelectedIndex, MPTVSeriesLog.LogLevel.Debug );
                }   
            }

            // Set Facade Layout
            Facade.CurrentLayout = ( GUIFacadeControl.Layout )CurrentLayout;
            GUIControl.FocusControl( GetID, Facade.GetID );

            // Set the selected item based on current
            Facade.SelectedListItemIndex = lSelectedIndex;
            DefaultArtIndex = lSelectedIndex;
            
            // Download artwork thumbs async and set to facade
            GetImages( aArtwork );
        }

        private string GetDataProviderNameFromEnum(ArtworkDataProvider aProvider)
        {
            switch ( aProvider )
            {
                case ArtworkDataProvider.TMDb:
                    return "themoviedb.org";
                case ArtworkDataProvider.FanartTV:
                    return "fanart.tv";
                default:
                    return "thetvdb.com";
            }
        }

        private ArtworkDataProvider GetProviderEnumFromString(string aName)
        {
            switch (aName)
            {
                case "themoviedb.org":
                    return ArtworkDataProvider.TMDb;
                case "fanart.tv":
                    return ArtworkDataProvider.FanartTV;
                default:
                    return ArtworkDataProvider.TVDb;
            }
        }

        private string GetArtworkTypeName(ArtworkType aType)
        {
            switch (aType)
            {
                case ArtworkType.SeriesFanart:
                    return Translation.SeriesFanart;
                case ArtworkType.SeriesPoster:
                    return Translation.SeriesPoster;
                case ArtworkType.SeriesBanner:
                    return Translation.SeriesWideBanner;
                case ArtworkType.SeasonPoster:
                    return Translation.SeasonPoster;
                default:
                    return Translation.EpisodeThumb;
            }
        }

        private string GetDefaultImage()
        {
            switch (ArtworkParams.Type)
            {
                case ArtworkType.SeriesFanart:
                case ArtworkType.EpisodeThumb:
                    return "defaultPictureBig.png";
                case ArtworkType.SeriesBanner:
                    return "defaultPictureWideBig.png";
                default:
                    return "defaultVideoBig.png";
            }
        }

        private string GetLabelTwo(Artwork aArtwork)
        {
            // if it's the default then its local too
            if ( aArtwork.IsDefault )
                return Translation.ArtworkSelected;

            if (aArtwork.DownloadProgress > 0 && aArtwork.DownloadProgress < 100)
                return string.Format(Translation.ArtworkDownloading, aArtwork.DownloadProgress);

            // otherwise we have it already or it's online
            return aArtwork.IsLocal? Translation.FanArtLocal: Translation.FanArtOnline;
        }

        private void SetProperty( string aProperty, string aValue, bool aLog = false )
        {
            string propertyValue = string.IsNullOrEmpty( aValue ) ? "N/A" : aValue;
            string propertyKey = string.Concat( "#TVSeries.Artwork.", aProperty );
            if ( aLog )
            {
                MPTVSeriesLog.Write( $"Publishing skin property '{propertyKey}' with value '{propertyValue}'", MPTVSeriesLog.LogLevel.Debug );
            }
            GUIPropertyManager.SetProperty( propertyKey, propertyValue );
        }

        private void ClearProperties()
        {
            // Global
            SetProperty( "SeriesID", " " );
            SetProperty( "SeriesName", " " );
            SetProperty( "SeasonIndex", " " );
            SetProperty( "EpisodeIndex", " " );
            SetProperty( "Type", " " );
            SetProperty( "LocalisedType", " " );
            SetProperty( "DataProvider", " " );

            // Selected
            SetProperty( "Filename", " " );
            SetProperty( "ThumbFilename", " " );
            SetProperty( "Language", " " );
            SetProperty( "OnlinePath", " " );
            SetProperty( "OnlineThumbPath", " " );
            SetProperty( "Resolution", " " );
            SetProperty( "Rating", " " );
            SetProperty( "RatingCount", " " );            
            SetProperty( "IsDefault", " " );
            SetProperty( "IsLocal", " " );
            SetProperty( "SelectedItem", " " );

            AsyncArtworkImage.Filename = string.Empty;
        }

        private void OnSelected( GUIListItem item, GUIControl parent )
        {
            if ( item is GUIArtworkListItem )
            {
                var lArtwork = ( item as GUIArtworkListItem ).Item as Artwork;
                bool lLog = !item.IsDownloading;

                SetProperty( "ThumbFilename", lArtwork.LocalThumbPath.Replace( "/", @"\" ), lLog );
                SetProperty( "Language", lArtwork.Language, lLog );
                SetProperty( "OnlinePath", lArtwork.OnlinePath, lLog );
                SetProperty( "OnlineThumbPath", lArtwork.OnlineThumbPath, lLog );
                SetProperty( "Resolution", lArtwork.Resolution, lLog );
                SetProperty( "Rating", lArtwork.Rating.ToString(), lLog );
                SetProperty( "RatingCount", lArtwork.Votes.ToString(), lLog );
                SetProperty( "IsDefault", lArtwork.IsDefault.ToString(), lLog );
                SetProperty( "IsLocal", lArtwork.IsLocal.ToString(), lLog );

                if (lArtwork.IsLocal)
                {
                    SetProperty("Filename", lArtwork.LocalPath.Replace("/", @"\"), lLog);

                    // delay image loading for big artworks
                    AsyncArtworkImage.Filename = lArtwork.LocalPath.Replace("/", @"\");
                }

                string lSelectedItemProperty;

                // thetvdb.com and fanart.tv only have votes/likes
                // themovidb.org have rating and votes
                if (ArtworkParams.Provider == ArtworkDataProvider.TMDb && lArtwork.Votes != 0)
                {
                    lSelectedItemProperty = $"{lArtwork.Rating.ToString("#,##0.00")} ({lArtwork.Votes} {Translation.Votes}) | {GetLabelTwo(lArtwork)}";
                }
                else
                {
                    lSelectedItemProperty = $"{lArtwork.Votes} {Translation.Votes} | {GetLabelTwo(lArtwork)}";
                }

                // only thetvdb.com and themoviedb.org have resolution
                if (ArtworkParams.Provider != ArtworkDataProvider.FanartTV)
                {
                    lSelectedItemProperty += " | " + lArtwork.Resolution;
                }

                SetProperty("SelectedItem", lSelectedItemProperty, lLog);
            }
            else
            {
                SetProperty( "Filename", " " );
                SetProperty( "Language", " " );
                SetProperty( "OnlinePath", " " );
                SetProperty( "OnlineThumbPath", " " );
                SetProperty( "Resolution", " " );
                SetProperty( "Rating", " " );
                SetProperty( "RatingCount", " " );
                SetProperty( "IsDefault", " " );
                SetProperty( "IsLocal", " " );
                SetProperty( "SelectedItem", " " );
            }
        }

        private void GetImages( List<Artwork> aArtwork )
        {
            StopDownload = false;

            // split the downloads in 5+ groups and do multithreaded downloading
            int groupSize = ( int )Math.Max( 1, Math.Floor( ( double )aArtwork.Count / 5 ) );
            int groups = ( int )Math.Ceiling( ( double )aArtwork.Count / groupSize );

            for ( int i = 0; i < groups; i++ )
            {
                var groupList = new List<Artwork>();
                for ( int j = groupSize * i; j < groupSize * i + ( groupSize * ( i + 1 ) > aArtwork.Count ? aArtwork.Count - groupSize * i : groupSize ); j++ )
                {
                    groupList.Add( aArtwork[j] );
                }

                new Thread( delegate ( object aObject )
                {
                    var lItems = ( List<Artwork> )aObject;
                    foreach ( var item in lItems )
                    {
                        // stop download if we have exited window
                        if ( StopDownload ) break;

                        string remoteThumb = item.ThumbnailUrl;
                        if ( string.IsNullOrEmpty( remoteThumb ) ) continue;

                        string localThumb = item.LocalThumbPath;
                        if ( string.IsNullOrEmpty( localThumb ) ) continue;

                        if ( Helper.DownloadFile( remoteThumb, localThumb ) )
                        {
                            // notify that thumbnail image has been downloaded
                            item.LocalThumbPath = localThumb;
                            item.NotifyPropertyChanged( "LocalThumbPath" );
                        }
                    }
                } )
                {
                    IsBackground = true,
                    Name = "TVSArtwork" + i.ToString()
                }.Start( groupList );
            }
        }

        private string GetLayoutTranslation( Layout aLayout )
        {
            string lLine = string.Empty;
            switch ( aLayout )
            {
                case Layout.List:
                    lLine = GUILocalizeStrings.Get( 101 );
                    break;
                case Layout.SmallIcons:
                    lLine = GUILocalizeStrings.Get( 100 );
                    break;
                case Layout.LargeIcons:
                    lLine = GUILocalizeStrings.Get( 417 );
                    break;
                case Layout.Filmstrip:
                    lLine = GUILocalizeStrings.Get( 733 );
                    break;
            }
            return lLine;
        }

        private void ShowLayoutsMenu()
        {
            var lDialog = ( IDialogbox )GUIWindowManager.GetWindow( ( int )GUIWindow.Window.WINDOW_DIALOG_MENU );
            lDialog.Reset();
            lDialog.SetHeading( GetLayoutTranslation( CurrentLayout ) );

            foreach ( Layout layout in Enum.GetValues( typeof( Layout ) ) )
            {
                string menuItem = GetLayoutTranslation( layout );
                var pItem = new GUIListItem( menuItem );
                if ( layout == CurrentLayout ) pItem.Selected = true;
                lDialog.Add( pItem );
            }

            lDialog.DoModal( GUIWindowManager.ActiveWindow );

            if ( lDialog.SelectedLabel >= 0 )
            {
                CurrentLayout = ( Layout )lDialog.SelectedLabel;
                Facade.CurrentLayout = ( GUIFacadeControl.Layout )CurrentLayout;
                GUIControl.FocusControl( GetID, Facade.GetID );
                if ( ButtonLayouts != null )
                {
                    GUIControl.SetControlLabel( GetID, ButtonLayouts.GetID, GetLayoutTranslation( CurrentLayout ) );
                }
            }
        }

        private void ShowDataProvidersMenu()
        {
            var lDialog = ( IDialogbox )GUIWindowManager.GetWindow( ( int )GUIWindow.Window.WINDOW_DIALOG_MENU );
            lDialog.Reset();
            lDialog.SetHeading( Translation.ChangeOnlineProvider );
            
            var lItem = new GUIListItem( GetDataProviderNameFromEnum(ArtworkDataProvider.TVDb) );
            if ( ArtworkParams.Provider == ArtworkDataProvider.TVDb ) lItem.Selected = true;
            lItem.ItemId = (int)ArtworkDataProvider.TVDb;
            lDialog.Add( lItem );
       
            // themoviedb.org does not support WideBanners
            if (ArtworkParams.Type != ArtworkType.SeriesBanner)
            {
                lItem = new GUIListItem( GetDataProviderNameFromEnum( ArtworkDataProvider.TMDb ) );
                if ( ArtworkParams.Provider == ArtworkDataProvider.TMDb ) lItem.Selected = true;
                lItem.ItemId = (int)ArtworkDataProvider.TMDb;
                lDialog.Add( lItem );
            }

            // fanart.tv does not support episode thumbs
            if (ArtworkParams.Type != ArtworkType.EpisodeThumb)
            {
                lItem = new GUIListItem(GetDataProviderNameFromEnum(ArtworkDataProvider.FanartTV));
                if (ArtworkParams.Provider == ArtworkDataProvider.FanartTV) lItem.Selected = true;
                lItem.ItemId = (int)ArtworkDataProvider.FanartTV;
                lDialog.Add(lItem);
            }

            lDialog.DoModal( GUIWindowManager.ActiveWindow );

            if ( lDialog.SelectedLabel >= 0 )
            {
                ArtworkParams.Provider = GetProviderEnumFromString(lDialog.SelectedLabelText);
                ArtworkParams.Series[DBOnlineSeries.cArtworkChooserProvider] = (int)ArtworkParams.Provider;
                ArtworkParams.Series.Commit();

                SetProperty( "DataProvider", ArtworkParams.Provider.ToString(), true );

                GUIControl.FocusControl( GetID, Facade.GetID );
                if ( ButtonOnlineProviders != null )
                {
                    GUIControl.SetControlLabel( GetID, ButtonOnlineProviders.GetID, string.Format( Translation.Provider, ArtworkParams.Provider.ToString() ) );
                }

                DownloadArtworkThumbs();
            }
        }

        #region Online Art Download
        private void StartDownload( Artwork aArtwork )
        {
            var lDownloadThread = new Thread( (obj) => 
            {
                var lArtwork = obj as Artwork;

                MPTVSeriesLog.Write( $"Starting download of artwork from '{lArtwork.Url}'" );

                // ensure the local directory exists
                string lDirectory = Path.GetDirectoryName(lArtwork.LocalPath);
                if (!Directory.Exists(lDirectory))
                    Directory.CreateDirectory(lDirectory);

                // .NET 4.0: Use TLS v1.2. Many download sources no longer support the older and now insecure TLS v1.0/1.1 and SSL v3.
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)0xc00;

                var lWebClient = new WebClient();
                lWebClient.DownloadProgressChanged += DownloadProgressChanged;
                lWebClient.DownloadFileCompleted += DownloadFileCompleted;
                lWebClient.DownloadFileAsync( new Uri( lArtwork.Url ), lArtwork.LocalPath, lArtwork );
            } );
            lDownloadThread.Start( aArtwork );
        }

        private void DownloadProgressChanged( object sender, DownloadProgressChangedEventArgs e )
        {
            var lArtwork = e.UserState as Artwork;

            // report progress to the facade on selected item
            lArtwork.DownloadProgress = e.ProgressPercentage;
            lArtwork.NotifyPropertyChanged( "DownloadProgress" );
            
            //MPTVSeriesLog.Write( $"Downloading {lArtwork.OnlinePath}, {e.ProgressPercentage}% | {e.BytesReceived} bytes out of {e.TotalBytesToReceive} bytes downloaded", MPTVSeriesLog.LogLevel.Debug );
        }

        private void DownloadFileCompleted( object sender, AsyncCompletedEventArgs e )
        {
            var lArtwork = e.UserState as Artwork;

            if (e.Error == null)
            {
                MPTVSeriesLog.Write($"Completed download of artwork '{lArtwork.LocalPath.Replace("/", @"\")}'");
            }
            else
            {
                lArtwork.DownloadException = e.Error;
                MPTVSeriesLog.Write($"Download of artwork '{lArtwork.LocalPath.Replace("/", @"\")}' failed. Error={lArtwork.DownloadException.ToString()}");
            }

            // report we now have finished download operation
            lArtwork.NotifyPropertyChanged("LocalPath");

        }
        #endregion

        #endregion
    }

    public class GUIArtworkListItem : GUIListItem
    {
        #region Facade Item
        public GUIArtworkListItem( string aLabel ) : base( aLabel ) { }

        public object Item
        {
            get { return _Item; }
            set
            {
                _Item = value;
                if (!(value is INotifyPropertyChanged notifier)) return;

                notifier.PropertyChanged += ( aSource, aEventArgs ) =>
                {
                    var lArtwork = aSource as Artwork;
                    if ( lArtwork == null ) return;

                    switch (aEventArgs.PropertyName)
                    {
                        case "LocalThumbPath":
                            SetImageToGui(lArtwork);
                            break;

                        case "DownloadProgress":
                            this.IsDownloading = true;
                            this.Label2 = string.Format(Translation.ArtworkDownloading, lArtwork.DownloadProgress);
                            this.ProgressBarPercentage = lArtwork.DownloadProgress;

                            // if the current download item is selected, update skin properties
                            UpdateSelectedItemSkinProperties();
                            break;

                        case "LocalPath":
                            this.IsDownloading = false;
                            this.ProgressBarPercentage = 0;

                            // check if artwork download was successful
                            // if an error occurs we will attach an exception object
                            if (lArtwork.DownloadException == null)
                            {
                                this.Label2 = Translation.FanArtLocal;

                                // update database as downloaded
                                SetArtworkAsLocal(lArtwork);
                                // publish big artwork to GUI
                                SetImageToGui(lArtwork);
                            }
                            else
                            {
                                // delete the local artwork if it exists, probably corrupted
                                if (File.Exists(lArtwork.LocalPath))
                                {
                                    GUIArtworkChooser.DeleteFile(lArtwork.LocalPath);
                                }
                                this.Label2 = Translation.DownloadFailed;
                                lArtwork.DownloadException = null;
                            }
                            break;

                        default:
                            break;
                    }
                };
            }
        }
        private object _Item;

        private void UpdateSelectedItemSkinProperties( )
        {
            int lFacadeId = 50;

            if (GUIControl.GetSelectedListItem(GUIWindowManager.ActiveWindow, lFacadeId) is GUIArtworkListItem lSelectedItem && lSelectedItem.Item == this.Item)
            {
                // this should trigger the onSelected event which updates the selected item skin properties
                GUIWindowManager.SendThreadMessage(new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GUIWindowManager.ActiveWindow, 0, lFacadeId, (lSelectedItem.Item as Artwork).DownloadItemIndex, 0, null));
            }
        }

        private void SetArtworkAsLocal(Artwork aArtwork)
        {
            aArtwork.IsLocal = true;

            switch (aArtwork.Type)
            {
                case ArtworkType.SeriesFanart:
                    // update database with local file path
                    aArtwork.Fanart[DBFanart.cLocalPath] = Fanart.GetLocalPath( aArtwork.Fanart );
                    aArtwork.Fanart.Commit();
                    break;

                case ArtworkType.SeriesPoster:
                    // update database with available posters
                    string lPath = "posters/" + System.IO.Path.GetFileName( aArtwork.OnlinePath );
                    string lRelativePath = Helper.CleanLocalPath( aArtwork.Series.ToString() ) + @"\-lang" + aArtwork.Language + "-" + lPath;

                    string lAvailableSeriesPosters = aArtwork.Series[DBOnlineSeries.cPosterFileNames].ToString();
                    aArtwork.Series[DBOnlineSeries.cPosterFileNames] = lAvailableSeriesPosters += "|" + lRelativePath;
                    aArtwork.Series.Commit();
                    break;

                case ArtworkType.SeriesBanner:
                    // update database with available posters
                    lPath = "graphical/" + System.IO.Path.GetFileName( aArtwork.OnlinePath );
                    lRelativePath = Helper.CleanLocalPath( aArtwork.Series.ToString() ) + @"\-lang" + aArtwork.Language + "-" + lPath;

                    string lAvailableSeriesBanners = aArtwork.Series[DBOnlineSeries.cBannerFileNames].ToString();
                    aArtwork.Series[DBOnlineSeries.cBannerFileNames] = lAvailableSeriesBanners += "|" + lRelativePath;
                    aArtwork.Series.Commit();
                    break;

                case ArtworkType.SeasonPoster:
                    // update database with available posters
                    lPath = "seasons/" + System.IO.Path.GetFileName( aArtwork.OnlinePath );
                    lRelativePath = Helper.CleanLocalPath( aArtwork.Series.ToString() ) + @"\-lang" + aArtwork.Language + "-" + lPath;

                    string lAvailableSeasonPosters = aArtwork.Season[DBSeason.cBannerFileNames].ToString();
                    aArtwork.Season[DBSeason.cBannerFileNames] = lAvailableSeasonPosters += "|" + lRelativePath;
                    aArtwork.Season.Commit();
                    break;

                case ArtworkType.EpisodeThumb:
                    // nothing more to do, user can click on it to make default
                    break;

            }
        }

        /// <summary>
        /// Update the facade when art thumbnail is downloaded/available
        /// </summary>
        private void SetImageToGui( Artwork aArtwork )
        {
            if ( string.IsNullOrEmpty( aArtwork.LocalThumbPath ) ) return;

            string lFilename = aArtwork.IsLocal ? aArtwork.LocalPath : aArtwork.LocalThumbPath;
            string lTexture = GetTextureFromFile(lFilename);

            int lMaxHeight = 0;
            int lMaxWidth = 0;

            // if we have the full size artwork then favour this as opposed to low resolution thumbnail
            if (aArtwork.IsLocal)
            {   
                switch (aArtwork.Type)
                {
                    case ArtworkType.EpisodeThumb:
                    case ArtworkType.SeriesFanart:
                        lMaxWidth = 1280;
                        lMaxHeight = 720;
                        break;

                    case ArtworkType.SeasonPoster:
                    case ArtworkType.SeriesPoster:
                        lMaxWidth = 510;
                        lMaxHeight = 750;
                        break;

                    case ArtworkType.SeriesBanner:
                        lMaxWidth = 758;
                        lMaxHeight = 140;
                        break;
                }

                try
                {
                    var lBitMap = new System.Drawing.Bitmap(ImageFast.FromFile(lFilename), lMaxWidth, lMaxHeight);

                    if (GUITextureManager.LoadFromMemory(lBitMap, lTexture, 0, 0, 0) > 0)
                    {
                        ThumbnailImage = lTexture;
                        IconImage = lTexture;
                        IconImageBig = lTexture;
                    }
                }
                catch (Exception ex)
                {
                    MPTVSeriesLog.Write($"Failed to load image '{lFilename}' into facade item. Error = '{ex.Message}'");
                }
            }
            else
            {
                try
                {
                    if (GUITextureManager.LoadFromMemory(ImageFast.FromFile(lFilename), lTexture, 0, 0, 0) > 0)
                    {
                        ThumbnailImage = lTexture;
                        IconImage = lTexture;
                        IconImageBig = lTexture;
                    }
                }
                catch (Exception ex)
                {
                    MPTVSeriesLog.Write($"Failed to load image '{lFilename}' into facade item. Error = '{ex.Message}'");
                }
            }

            // if the selected item is the item with the new image added, then force an update of thumbnail
            if (!(GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindow) is GUIArtworkChooser lArtworkWindow)) return;

            int lFacadeId = 50;
            int lSelectedItemIndex = ( lArtworkWindow.GetControl( lFacadeId ) as GUIFacadeControl ).SelectedListItemIndex;

            GUIListItem lSelectedItem = GUIControl.GetSelectedListItem( GUIWindowManager.ActiveWindow, lFacadeId );
            if ( lSelectedItem == this )
            {
                GUIWindowManager.SendThreadMessage( 
                    new GUIMessage( GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, 
                                    GUIWindowManager.ActiveWindow, 
                                    0, 
                                    lFacadeId,
                                    lSelectedItemIndex, 
                                    0, 
                                    null ) );
            }
        }

        private string GetTextureFromFile( string aFilename )
        {
            return "[TVSeries:" + aFilename + "]";
        }

        #endregion
    }

    public class Artwork : INotifyPropertyChanged
    {
        public int Id { get; set; }

        public string OnlinePath { get; set; }

        public string OnlineThumbPath { get; set; }

        public string Url { get; set; }

        public string ThumbnailUrl { get; set; }

        public string LocalPath { get; set; }

        public string LocalThumbPath { get; set; }

        public string Resolution { get; set; }

        public string Language { get; set; }

        public double Rating { get; set; }

        public uint Votes { get; set; }

        public ArtworkType Type { get; set; }

        public ArtworkDataProvider Provider { get; set; }

        public DBFanart Fanart { get; set; }

        public DBSeries Series { get; set; }

        public DBSeason Season { get; set; }

        public DBEpisode Episode { get; set; }

        /// <summary>
        /// True if the Fanart contains a series logo
        /// Most fanart are clean without series names and logo
        /// </summary>
        public bool HasLogo { get; set; }

        /// <summary>
        /// True if the full size artwork is already downloaded
        /// </summary>
        public bool IsLocal { get; set; }

        /// <summary>
        /// True if the artwork is the default artwork i.e. selected
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// For reporting download progress of a full size image
        /// </summary>
        public int DownloadProgress { get; set; }

        /// <summary>
        /// The index of the GUIListItem artwork being downloaded
        /// </summary>
        public int DownloadItemIndex { get; set; }

        /// <summary>
        /// Error if async download failed
        /// </summary>
        public Exception DownloadException { get; set; }

        #region INotifyPropertyChanged

        /// <summary>
        /// Notify ThumbnailImage property change during async image downloading
        /// Sends messages to facade to update image
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged( string propertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }

        #endregion

        #region Comparison
        public override bool Equals( Object obj )
        {
            if ( obj == null )
                return false;

            Artwork p = obj as Artwork;
            if ( ( Object )p == null )
                return false;

            return ( OnlinePath == p.OnlinePath || LocalPath == p.LocalPath );
        }

        public bool Equals( Artwork a )
        {
            if ( ( object )a == null )
                return false;

            return ( OnlinePath == a.OnlinePath || LocalPath == a.LocalPath );
        }

        public static bool operator ==( Artwork a, Artwork b )
        {
            if ( Object.ReferenceEquals( a, b ) )
                return true;

            if ( ( object )a == null || ( object )b == null )
                return false;

            return a.OnlinePath == b.OnlinePath || a.LocalPath == b.LocalPath;
        }

        public static bool operator !=( Artwork a, Artwork b )
        {
            return !( a == b );
        }

        public override int GetHashCode()
        {
            return OnlinePath.GetHashCode();
        }
        #endregion
    }

    public class SortBy
    {    
        public SortingFields Field { get; set; }

        public SortingDirections Direction { get; set; }
    }

    public enum SortingFields
    {
        Score,
        Votes,
        Resolution
    }

    public enum SortingDirections
    {
        Ascending,
        Descending
    }

    public class GUIListItemSorter : IComparer<Artwork>
    {
        private readonly SortingFields mSortField;
        private readonly SortingDirections mSortDirection;

        public GUIListItemSorter( SortingFields aSortField, SortingDirections aSortDirection )
        {
            mSortField = aSortField;
            mSortDirection = aSortDirection;
        }

        public int Compare( Artwork aArtworkA, Artwork aArtworkY )
        {
            if (aArtworkA == null || aArtworkY == null)
                return 0;

            int lResult;

            switch ( mSortField )
            {
                case SortingFields.Score:
                    lResult = aArtworkA.Rating.CompareTo( aArtworkY.Rating );
                    if ( lResult == 0 )
                    {
                        // if same score compare votes
                        lResult = aArtworkA.Votes.CompareTo( aArtworkY.Votes );
                    }
                    break;

                case SortingFields.Votes:
                    lResult = aArtworkA.Votes.CompareTo( aArtworkY.Votes );
                    break;

                default:
                    lResult = 0;
                    break;
            }
                
            if ( mSortDirection == SortingDirections.Descending )
                lResult = -lResult;

            return lResult;
        }
    }
}

