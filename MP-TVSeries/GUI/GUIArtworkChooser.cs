using MediaPortal.GUI.Library;
using MediaPortal.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using WindowPlugins.GUITVSeries.Extensions;
using WindowPlugins.GUITVSeries.Online_Parsing_Classes;
using Action = MediaPortal.GUI.Library.Action;

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
    }

    public class GUIArtworkChooser : GUIWindow
    {
        #region Skin Controls

        [SkinControlAttribute( 50 )]
        protected GUIFacadeControl mFacadePosters = null;

        [SkinControlAttribute( 51 )]
        protected GUIFacadeControl mFacadeWidebanners = null;

        [SkinControlAttribute( 52 )]
        protected GUIFacadeControl mFacadeThumbnails = null;

        [SkinControlAttribute( 2 )]
        protected GUIButtonControl ButtonLayouts = null;

        [SkinControlAttribute( 3 )]
        protected GUIButtonControl ButtonResolutionFilter = null;

        [SkinControlAttribute( 4 )]
        protected GUIButtonControl ButtonLanguageFilter = null;

        [SkinControlAttribute( 5 )]
        protected GUIButtonControl ButtonSortBy = null;

        [SkinControlAttribute( 6 )]
        protected GUIButtonControl ButtonRefresh = null;

        #endregion

        #region Enums

        public enum ContextMenuItem
        {
            Layout,
            Filter
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

        public GUIArtworkChooser() { }

        #endregion

        #region Private Properties

        private bool StopDownload { get; set; }

        private Layout CurrentLayout { get; set; }

        private ArtworkLoadingParameters ArtworkParams { get; set; }

        private GUIFacadeControl Facade
        {
            get
            {
                GUIFacadeControl lFacade = null;

                switch ( ArtworkParams.Type )
                {
                    case ArtworkType.SeriesPoster:
                    case ArtworkType.SeasonPoster:
                        lFacade = mFacadePosters;
                        break;

                    case ArtworkType.SeriesFanart:
                    case ArtworkType.EpisodeThumb:
                        lFacade = mFacadeThumbnails;
                        break;

                    case ArtworkType.SeriesBanner:
                        lFacade = mFacadeWidebanners;
                        break;
                }
                return lFacade;
            }
        }

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
            // set window name
            GUIPropertyManager.SetProperty( "#currentmodule", Translation.Artwork );

            // clear any properties from previous series
            ClearProperties();

            // set default layout
            int defaultLayout = 0;
            int.TryParse( DBOption.GetOptions( DBOption.cArtworkChooserLayout ), out defaultLayout );
            CurrentLayout = ( Layout )defaultLayout;

            // update button label
            GUIControl.SetControlLabel( GetID, ButtonLayouts.GetID, GetLayoutTranslation( CurrentLayout ) );

            // Deserialise loading parameter from JSON (ArtworkParameters)
            LoadParameters();

            // set facade visibility
            SetFacadeVisibility();

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
        }

        protected override void OnClicked( int controlId, GUIControl control, Action.ActionType actionType )
        {
            // wait for any background action to finish
            if ( GUIConnector.Instance.IsBusy ) return;

            if ( control == ButtonLayouts )
            {
                ShowLayoutsMenu();
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
            GUIListItem selectedItem = Facade.SelectedListItem;
            if ( selectedItem == null ) return;

            var dlg = ( IDialogbox )GUIWindowManager.GetWindow( ( int )GUIWindow.Window.WINDOW_DIALOG_MENU );
            dlg.Reset();
            dlg.SetHeading( Translation.Actors );

            GUIListItem listItem = null;

            listItem = new GUIListItem( Translation.ChangeLayout + " ..." );
            dlg.Add( listItem );
            listItem.ItemId = ( int )ContextMenuItem.Layout;

            // Show Context Menu
            dlg.DoModal( GUIWindowManager.ActiveWindow );
            if ( dlg.SelectedId < 0 ) return;

            switch ( dlg.SelectedId )
            {
                case ( ( int )ContextMenuItem.Layout ):
                    ShowLayoutsMenu();
                    break;
            }

            base.OnShowContextMenu();
        }
        #endregion

        #region Private Methods

        private void SetFacadeVisibility()
        {
            switch ( ArtworkParams.Type )
            {
                case ArtworkType.SeriesPoster:
                case ArtworkType.SeasonPoster:
                    mFacadeThumbnails.Visible = false;
                    mFacadeWidebanners.Visible = false;
                    break;

                case ArtworkType.SeriesFanart:
                case ArtworkType.EpisodeThumb:
                    mFacadePosters.Visible = false;
                    mFacadeWidebanners.Visible = false;
                    break;

                case ArtworkType.SeriesBanner:
                    mFacadePosters.Visible = false;
                    mFacadeThumbnails.Visible = false;
                    break;
            }
        }

        private bool LoadParameters()
        {
            // _loadingParameter can be set by a plugin developer or a skin designer
            if ( string.IsNullOrEmpty( _loadParameter ) )
                return false;

            ArtworkParams = _loadParameter.FromJSON<ArtworkLoadingParameters>();
            if ( ArtworkParams == null ) return false;

            SetProperty( "SeriesID", ArtworkParams.SeriesId.ToString() );
            SetProperty( "SeriesName", ArtworkParams.Series.ToString() );
            SetProperty( "Type", ArtworkParams.Type.ToString() );
            if ( ArtworkParams.Type == ArtworkType.SeasonPoster )
            {
                SetProperty( "SeasonIndex", ArtworkParams.SeasonIndex.ToString() );
            }

            return true;
        }

        private void DownloadArtworkThumbs()
        {
            GUIConnector.Instance.ExecuteInBackgroundAndCallback( () =>
            {
                return GetArtworkThumbs();
            },
            delegate ( bool success, object result )
            {
                if ( success )
                {
                    var lArtwork = result as List<TvdbArt>;
                    LoadFacade( lArtwork );
                }
            }, Translation.GettingArtwork, true );
        }

        private void GetFanart(XmlNode aNode, DBFanart aDefaultFanart, ref List<TvdbArt> aArtwork)
        {
            foreach ( XmlNode banner in aNode.SelectNodes( "/Banners/Banner[BannerType='fanart']" ) )
            {
                var lFanart = new TvdbArt();

                lFanart.Id = uint.Parse( banner.SelectSingleNode( "id" ).InnerText );
                lFanart.Language = banner.SelectSingleNode( "Language" ).InnerText;
                lFanart.OnlinePath = banner.SelectSingleNode( "BannerPath" ).InnerText;
                lFanart.OnlineThumbPath = banner.SelectSingleNode( "ThumbnailPath" ).InnerText;
                lFanart.Resolution = banner.SelectSingleNode( "BannerType2" ).InnerText;
                lFanart.HasLogo = Convert.ToBoolean( banner.SelectSingleNode( "SeriesName" ).InnerText );

                if ( !string.IsNullOrEmpty( banner.SelectSingleNode( "Rating" ).InnerText ) )
                {
                    double rating = double.Parse( banner.SelectSingleNode( "Rating" ).InnerText, NumberStyles.Any, NumberFormatInfo.InvariantInfo );
                    lFanart.Rating = Math.Round( rating, 1, MidpointRounding.AwayFromZero );
                }

                if ( !string.IsNullOrEmpty( banner.SelectSingleNode( "RatingCount" ).InnerText ) )
                    lFanart.Votes = uint.Parse( banner.SelectSingleNode( "RatingCount" ).InnerText );

                // create the local filename path for the online thumbnail image e.g. _cache\fanart\original\<seriesId>-*.jpg
                string lLocalThumbPath = Fanart.GetLocalThumbPath( lFanart.OnlineThumbPath, ArtworkParams.SeriesId.ToString() );
                lFanart.LocalThumbPath = Helper.PathCombine( Settings.GetPath( Settings.Path.fanart ), lLocalThumbPath.Replace( "/", @"\" ) );

                // create the local filename path for the online image e.g. fanart\original\<seriesId>-*.jpg
                string lLocalPath = Fanart.GetLocalPath( lFanart.OnlinePath, ArtworkParams.SeriesId.ToString() );
                lFanart.LocalPath = Helper.PathCombine( Settings.GetPath( Settings.Path.fanart ), lLocalPath.Replace( "/", @"\" ) );

                // if the fullsize artwork is already downloaded, then set it
                if ( File.Exists( lFanart.LocalPath ) ) lFanart.IsLocal = true;

                // if the artwork is default/selected, then set it
                if ( aDefaultFanart != null )
                {
                    if ( aDefaultFanart[DBFanart.cLocalPath] == lLocalPath.Replace( "/", @"\" ) )
                    {
                        lFanart.IsDefault = true;
                    }
                }

                // create full dowload url's
                string lBaseUrl = DBOnlineMirror.Banners.EndsWith( "/" ) ? DBOnlineMirror.Banners : DBOnlineMirror.Banners + "/";
                lFanart.ThumbnailUrl = lBaseUrl + lFanart.OnlineThumbPath;
                lFanart.Url = lBaseUrl + lFanart.OnlinePath;

                if (!aArtwork.Contains(lFanart))
                    aArtwork.Add( lFanart );
            }
        }

        private void GetSeriesPosters(XmlNode aNode, ref List<TvdbArt> aArtwork)
        {
            foreach ( XmlNode banner in aNode.SelectNodes( "/Banners/Banner[BannerType='poster']" ) )
            {
                var lPoster = new TvdbArt();

                lPoster.Language = banner.SelectSingleNode( "Language" ).InnerText;
                lPoster.OnlinePath = banner.SelectSingleNode( "BannerPath" ).InnerText;
                lPoster.OnlineThumbPath = banner.SelectSingleNode( "ThumbnailPath" ).InnerText;
                lPoster.Resolution = banner.SelectSingleNode( "BannerType2" ).InnerText;

                if ( !string.IsNullOrEmpty( banner.SelectSingleNode( "Rating" ).InnerText ) )
                {
                    double rating = double.Parse( banner.SelectSingleNode( "Rating" ).InnerText, NumberStyles.Any, NumberFormatInfo.InvariantInfo );
                    lPoster.Rating = Math.Round( rating, 1, MidpointRounding.AwayFromZero );
                }

                if ( !string.IsNullOrEmpty( banner.SelectSingleNode( "RatingCount" ).InnerText ) )
                    lPoster.Votes = uint.Parse( banner.SelectSingleNode( "RatingCount" ).InnerText );

                // create the local filename path for the online thumbnail image e.g. 13 Reasons Why\Thumbnails\-langen-posters\5aecac0f66076_t.jpg
                string lThumbPath = "posters/" + Path.GetFileName( lPoster.OnlineThumbPath );
                string lRelativePath = Helper.cleanLocalPath( ArtworkParams.Series.ToString() ) + @"\Thumbnails\-lang" + lPoster.Language + "-" + lThumbPath;
                lPoster.LocalThumbPath = Helper.PathCombine( Settings.GetPath( Settings.Path.banners ), lRelativePath );

                // create the local filename path for the online image
                string lPath = "posters/" + Path.GetFileName( lPoster.OnlinePath );
                lRelativePath = Helper.cleanLocalPath( ArtworkParams.Series.ToString() ) + @"\-lang" + lPoster.Language + "-" + lPath;
                lPoster.LocalPath = Helper.PathCombine( Settings.GetPath( Settings.Path.banners ), lRelativePath );

                // if the fullsize artwork is already downloaded, then set it
                if ( File.Exists( lPoster.LocalPath ) ) lPoster.IsLocal = true;

                // if the artwork is default/selected, then set it
                // remove any inconsistency with slashes, it should still be unique
                if ( lRelativePath.Replace( "\\", "" ).Replace( "/", "" ) == ArtworkParams.Series[DBOnlineSeries.cCurrentPosterFileName].ToString().Replace( "\\", "" ).Replace( "/", "" ) )
                    lPoster.IsDefault = true;

                // create full dowload url's
                string lBaseUrl = DBOnlineMirror.Banners.EndsWith( "/" ) ? DBOnlineMirror.Banners : DBOnlineMirror.Banners + "/";
                lPoster.ThumbnailUrl = lBaseUrl + lPoster.OnlineThumbPath;
                lPoster.Url = lBaseUrl + lPoster.OnlinePath;

                if (!aArtwork.Contains(lPoster))
                    aArtwork.Add( lPoster );
            }
        }

        private void GetSeriesWideBanners(XmlNode aNode, ref List<TvdbArt> aArtwork)
        {
            foreach ( XmlNode banner in aNode.SelectNodes( "/Banners/Banner[BannerType='series']" ) )
            {
                var lWideBanner = new TvdbArt();

                lWideBanner.Language = banner.SelectSingleNode( "Language" ).InnerText;
                lWideBanner.OnlinePath = banner.SelectSingleNode( "BannerPath" ).InnerText;
                lWideBanner.OnlineThumbPath = banner.SelectSingleNode( "ThumbnailPath" ).InnerText;
                lWideBanner.Resolution = banner.SelectSingleNode( "BannerType2" ).InnerText;

                if ( !string.IsNullOrEmpty( banner.SelectSingleNode( "Rating" ).InnerText ) )
                {
                    double rating = double.Parse( banner.SelectSingleNode( "Rating" ).InnerText, NumberStyles.Any, NumberFormatInfo.InvariantInfo );
                    lWideBanner.Rating = Math.Round( rating, 1, MidpointRounding.AwayFromZero );
                }

                if ( !string.IsNullOrEmpty( banner.SelectSingleNode( "RatingCount" ).InnerText ) )
                    lWideBanner.Votes = uint.Parse( banner.SelectSingleNode( "RatingCount" ).InnerText );

                // create the local filename path for the online thumbnail image e.g. 13 Reasons Why\Thumbnails\-langen-graphical\5aecac0f66076_t.jpg
                string lThumbPath = "graphical/" + Path.GetFileName( lWideBanner.OnlineThumbPath );
                string lRelativePath = Helper.cleanLocalPath( ArtworkParams.Series.ToString() ) + @"\Thumbnails\-lang" + lWideBanner.Language + "-" + lThumbPath;
                lWideBanner.LocalThumbPath = Helper.PathCombine( Settings.GetPath( Settings.Path.banners ), lRelativePath );

                // create the local filename path for the online image
                string lPath = "graphical/" + Path.GetFileName( lWideBanner.OnlinePath );
                lRelativePath = Helper.cleanLocalPath( ArtworkParams.Series.ToString() ) + @"\-lang" + lWideBanner.Language + "-" + lPath;
                lWideBanner.LocalPath = Helper.PathCombine( Settings.GetPath( Settings.Path.banners ), lRelativePath );

                // if the fullsize artwork is already downloaded, then set it
                if ( File.Exists( lWideBanner.LocalPath ) ) lWideBanner.IsLocal = true;

                // if the artwork is default/selected, then set it
                // remove any inconsistency with slashes, it should still be unique
                if ( lRelativePath.Replace( "\\", "" ).Replace( "/", "" ) == ArtworkParams.Series[DBOnlineSeries.cCurrentBannerFileName].ToString().Replace( "\\", "" ).Replace( "/", "" ) )
                    lWideBanner.IsDefault = true;

                // create full dowload url's
                string lBaseUrl = DBOnlineMirror.Banners.EndsWith( "/" ) ? DBOnlineMirror.Banners : DBOnlineMirror.Banners + "/";
                lWideBanner.ThumbnailUrl = lBaseUrl + lWideBanner.OnlineThumbPath;
                lWideBanner.Url = lBaseUrl + lWideBanner.OnlinePath;

                if ( !aArtwork.Contains( lWideBanner ) )
                    aArtwork.Add( lWideBanner );
            }
        }

        private void GetSeasonPosters(XmlNode aNode, ref List<TvdbArt> aArtwork)
        {
            foreach ( XmlNode banner in aNode.SelectNodes( "/Banners/Banner[BannerType='season']" ) )
            {
                // only interested in artwork for the selected season
                if ( banner.SelectSingleNode( "Season" ).InnerText != ArtworkParams.SeasonIndex.ToString() )
                    continue;

                var lSeasonPoster = new TvdbArt();

                lSeasonPoster.Language = banner.SelectSingleNode( "Language" ).InnerText;
                lSeasonPoster.OnlinePath = banner.SelectSingleNode( "BannerPath" ).InnerText;
                lSeasonPoster.OnlineThumbPath = banner.SelectSingleNode( "ThumbnailPath" ).InnerText;
                lSeasonPoster.Resolution = banner.SelectSingleNode( "BannerType2" ).InnerText;

                if ( !string.IsNullOrEmpty( banner.SelectSingleNode( "Rating" ).InnerText ) )
                {
                    double rating = double.Parse( banner.SelectSingleNode( "Rating" ).InnerText, NumberStyles.Any, NumberFormatInfo.InvariantInfo );
                    lSeasonPoster.Rating = Math.Round( rating, 1, MidpointRounding.AwayFromZero );
                }

                if ( !string.IsNullOrEmpty( banner.SelectSingleNode( "RatingCount" ).InnerText ) )
                    lSeasonPoster.Votes = uint.Parse( banner.SelectSingleNode( "RatingCount" ).InnerText );

                // create the local filename path for the online thumbnail image e.g. 13 Reasons Why\Thumbnails\-langen-seasons\5aecac0f66076_t.jpg
                string lThumbPath = "seasons/" + Path.GetFileName( lSeasonPoster.OnlineThumbPath );
                string lRelativePath = Helper.cleanLocalPath( ArtworkParams.Series.ToString() ) + @"\Thumbnails\-lang" + lSeasonPoster.Language + "-" + lThumbPath;
                lSeasonPoster.LocalThumbPath = Helper.PathCombine( Settings.GetPath( Settings.Path.banners ), lRelativePath );

                // create the local filename path for the online image
                string lPath = "seasons/" + Path.GetFileName( lSeasonPoster.OnlinePath );
                lRelativePath = Helper.cleanLocalPath( ArtworkParams.Series.ToString() ) + @"\-lang" + lSeasonPoster.Language + "-" + lPath;
                lSeasonPoster.LocalPath = Helper.PathCombine( Settings.GetPath( Settings.Path.banners ), lRelativePath );

                // if the fullsize artwork is already downloaded, then set it
                if ( File.Exists( lSeasonPoster.LocalPath ) ) lSeasonPoster.IsLocal = true;

                // if the artwork is default/selected, then set it
                // remove any inconsistency with slashes, it should still be unique
                if ( lRelativePath.Replace( "\\", "" ).Replace( "/", "" ) == ArtworkParams.Season[DBSeason.cCurrentBannerFileName].ToString().Replace( "\\", "" ).Replace( "/", "" ) )
                    lSeasonPoster.IsDefault = true;

                // create full dowload url's
                string lBaseUrl = DBOnlineMirror.Banners.EndsWith( "/" ) ? DBOnlineMirror.Banners : DBOnlineMirror.Banners + "/";
                lSeasonPoster.ThumbnailUrl = lBaseUrl + lSeasonPoster.OnlineThumbPath;
                lSeasonPoster.Url = lBaseUrl + lSeasonPoster.OnlinePath;

                if ( !aArtwork.Contains( lSeasonPoster ) )
                    aArtwork.Add( lSeasonPoster );
            }
        }

        private List<TvdbArt> GetArtworkThumbs()
        {
            var lArtwork = new List<TvdbArt>();

            switch ( ArtworkParams.Type )
            {
                #region Series Fanart
                case ArtworkType.SeriesFanart:
                    XmlNode lBanners = OnlineAPI.GetBannerList( ArtworkParams.SeriesId );
                    if ( lBanners == null ) return null;
                    
                    // get fanart from database table
                    var lDBFanart = DBFanart.GetAll( ArtworkParams.SeriesId, false );

                    // get the default fanart
                    var lDefaultFanart = lDBFanart.FirstOrDefault( f => f[DBFanart.cChosen] == 1 );

                    GetFanart( lBanners, lDefaultFanart, ref lArtwork );

                    // get english fanart too
                    if ( OnlineAPI.GetSeriesLanguage( ArtworkParams.SeriesId ) != "en" )
                    {
                        lBanners = OnlineAPI.GetBannerList( ArtworkParams.SeriesId, "en" );
                        if ( lBanners == null ) return null;

                        GetFanart( lBanners, lDefaultFanart, ref lArtwork );
                    }

                    lArtwork.Sort( new GUIListItemSorter( SortingFields.Score, SortingDirections.Descending ) );
                    return lArtwork;
                #endregion

                #region Series Posters
                case ArtworkType.SeriesPoster:
                    lBanners = OnlineAPI.GetBannerList( ArtworkParams.SeriesId );
                    if ( lBanners == null ) return null;

                    GetSeriesPosters( lBanners, ref lArtwork );

                    // get english artwork too
                    if ( OnlineAPI.GetSeriesLanguage( ArtworkParams.SeriesId ) != "en" )
                    {
                        lBanners = OnlineAPI.GetBannerList( ArtworkParams.SeriesId, "en" );
                        if ( lBanners == null ) return null;

                        GetSeriesPosters( lBanners, ref lArtwork );
                    }

                    lArtwork.Sort( new GUIListItemSorter( SortingFields.Score, SortingDirections.Descending ) );
                    return lArtwork;
                #endregion

                #region Series Widebanner
                case ArtworkType.SeriesBanner:
                    lBanners = OnlineAPI.GetBannerList( ArtworkParams.SeriesId );
                    if ( lBanners == null ) return null;

                    GetSeriesWideBanners( lBanners, ref lArtwork );

                    // get english artwork too
                    if ( OnlineAPI.GetSeriesLanguage( ArtworkParams.SeriesId ) != "en" )
                    {
                        lBanners = OnlineAPI.GetBannerList( ArtworkParams.SeriesId, "en" );
                        if ( lBanners == null ) return null;

                        GetSeriesWideBanners( lBanners, ref lArtwork );
                    }

                    lArtwork.Sort( new GUIListItemSorter( SortingFields.Score, SortingDirections.Descending ) );
                    return lArtwork;
                #endregion

                #region Season Posters
                case ArtworkType.SeasonPoster:
                    lBanners = OnlineAPI.GetBannerList( ArtworkParams.SeriesId );
                    if ( lBanners == null ) return null;

                    GetSeasonPosters( lBanners, ref lArtwork );

                    // get english artwork too
                    if ( OnlineAPI.GetSeriesLanguage( ArtworkParams.SeriesId ) != "en" )
                    {
                        lBanners = OnlineAPI.GetBannerList( ArtworkParams.SeriesId, "en" );
                        if ( lBanners == null ) return null;

                        GetSeasonPosters( lBanners, ref lArtwork );
                    }

                    lArtwork.Sort( new GUIListItemSorter( SortingFields.Score, SortingDirections.Descending ) );
                    return lArtwork;
                #endregion

                case ArtworkType.EpisodeThumb:
                    return lArtwork;

                default:
                    return null;
            }
        }

        private void LoadFacade( List<TvdbArt> aArtwork )
        {
            // clear facade
            GUIControl.ClearControl( GetID, Facade.GetID );

            // notify user if no thumbs to display and backout of window
            if ( aArtwork == null || aArtwork.Count == 0 )
            {
                TVSeriesPlugin.ShowNotifyDialog( Translation.Artwork, string.Format( Translation.NoArtwork, ArtworkParams.Type ) );
                GUIWindowManager.ShowPreviousWindow();
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
                Utils.SetDefaultIcons( lArtworkItem );

                Facade.Add( lArtworkItem );

                // if default, get index
                if ( lItem.IsDefault )
                    lSelectedIndex = Facade.Count - 1;
            }

            // Set the selected item based on current
            Facade.SelectedListItemIndex = lSelectedIndex;

            // Set Facade Layout
            Facade.CurrentLayout = ( GUIFacadeControl.Layout )CurrentLayout;

            GUIControl.FocusControl( GetID, Facade.GetID );

            // Download artwork thumbs async and set to facade
            GetImages( aArtwork );
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

        private string GetLabelTwo(TvdbArt aArtwork)
        {
            // if it's the default then its local too
            if ( aArtwork.IsDefault )
                return Translation.ArtworkSelected;

            // otherwise we have it already or it's online
            return aArtwork.IsLocal? Translation.FanArtLocal: Translation.FanArtOnline;
        }

        private void SetProperty( string property, string value )
        {
            string propertyValue = string.IsNullOrEmpty( value ) ? "N/A" : value;
            string propertyKey = string.Concat( "#TVSeries.Artwork.", property );
            GUIPropertyManager.SetProperty( propertyKey, propertyValue );
        }

        private void ClearProperties()
        {
            // Global
            SetProperty( "SeriesID", " " );
            SetProperty( "SeriesName", " " );
            SetProperty( "SeasonIndex", " " );
            SetProperty( "Type", " " );

            // Selected
            SetProperty( "Filename", " " );
            SetProperty( "Language", " " );
            SetProperty( "OnlinePath", " " );
            SetProperty( "OnlineThumbPath", " " );
            SetProperty( "Rating", " " );
            SetProperty( "RatingCount", " " );            
            SetProperty( "IsDefault", " " );
            SetProperty( "IsLocal", " " );
            SetProperty( "SelectedItem", " " );
        }

        private void OnSelected( GUIListItem item, GUIControl parent )
        {
            var lArtwork = ( item as GUIArtworkListItem ).Item as TvdbArt;
            
            SetProperty( "Filename", lArtwork.LocalThumbPath.Replace( "/", @"\" ) ); // publish fullsize if available ?
            SetProperty( "Language", lArtwork.Language );
            SetProperty( "OnlinePath", lArtwork.OnlinePath );
            SetProperty( "OnlineThumbPath", lArtwork.OnlineThumbPath );
            SetProperty( "Rating", lArtwork.Rating.ToString() );
            SetProperty( "RatingCount", lArtwork.Votes.ToString() );
            SetProperty( "IsDefault", lArtwork.IsDefault.ToString() );
            SetProperty( "IsLocal", lArtwork.IsLocal.ToString() );

            SetProperty( "SelectedItem", $"{lArtwork.Rating} ({lArtwork.Votes} {Translation.Votes}) | {GetLabelTwo(lArtwork)}");
        }

        private void GetImages( List<TvdbArt> aArtwork )
        {
            StopDownload = false;

            // split the downloads in 5+ groups and do multithreaded downloading
            int groupSize = ( int )Math.Max( 1, Math.Floor( ( double )aArtwork.Count / 5 ) );
            int groups = ( int )Math.Ceiling( ( double )aArtwork.Count / groupSize );

            for ( int i = 0; i < groups; i++ )
            {
                var groupList = new List<TvdbArt>();
                for ( int j = groupSize * i; j < groupSize * i + ( groupSize * ( i + 1 ) > aArtwork.Count ? aArtwork.Count - groupSize * i : groupSize ); j++ )
                {
                    groupList.Add( aArtwork[j] );
                }

                new System.Threading.Thread( delegate ( object aObject )
                {
                    var lItems = ( List<TvdbArt> )aObject;
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
                GUIControl.SetControlLabel( GetID, ButtonLayouts.GetID, GetLayoutTranslation( CurrentLayout ) );
            }
        }

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
                var notifier = value as INotifyPropertyChanged;
                if ( notifier != null )
                {
                    notifier.PropertyChanged += ( aSource, aEventArgs ) =>
                    {
                        if ( aSource is TvdbArt && aEventArgs.PropertyName == "LocalThumbPath" )
                            SetImageToGui( ( aSource as TvdbArt ).LocalThumbPath );
                    };
                }
            }
        }
        protected object _Item;

        protected void SetImageToGui( string aImageFilePath )
        {
            if ( string.IsNullOrEmpty( aImageFilePath ) ) return;

            string lTexture = GetTextureFromFile( aImageFilePath );

            if ( GUITextureManager.LoadFromMemory( ImageFast.FromFile( aImageFilePath ), lTexture, 0, 0, 0 ) > 0 )
            {
                ThumbnailImage = lTexture;
                IconImage = lTexture;
                IconImageBig = lTexture;
            }

            // if selected and GUIArtworkChooser is the current window force an update of thumbnail
            var lArtworkWindow = GUIWindowManager.GetWindow( GUIWindowManager.ActiveWindow ) as GUIArtworkChooser;
            if ( lArtworkWindow != null )
            {
                //GUIListItem lSelectedItem = GUIControl.GetSelectedListItem( 9817, 50 );
                //if ( lSelectedItem == this )
                //{
                //    GUIWindowManager.SendThreadMessage( new GUIMessage( GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GUIWindowManager.ActiveWindow, 0, 50, ItemId, 0, null ) );
                //}
            }
        }

        private string GetTextureFromFile( string aFilename )
        {
            return "[TVSeries:" + aFilename.GetHashCode() + "]";
        }

        #endregion
    }

    public class TvdbArt : INotifyPropertyChanged
    {
        public uint Id { get; set; }

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

            TvdbArt p = obj as TvdbArt;
            if ( ( Object )p == null )
                return false;

            return ( OnlinePath == p.OnlinePath );
        }

        public bool Equals( TvdbArt a )
        {
            if ( ( object )a == null )
                return false;

            return ( OnlinePath == a.OnlinePath );
        }

        public static bool operator ==( TvdbArt a, TvdbArt b )
        {
            if ( Object.ReferenceEquals( a, b ) )
                return true;

            if ( ( object )a == null || ( object )b == null )
                return false;

            return a.OnlinePath == b.OnlinePath;
        }

        public static bool operator !=( TvdbArt a, TvdbArt b )
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

    public class GUIListItemSorter : IComparer<TvdbArt>
    {
        private SortingFields mSortField;
        private SortingDirections mSortDirection;

        public GUIListItemSorter( SortingFields aSortField, SortingDirections aSortDirection )
        {
            mSortField = aSortField;
            mSortDirection = aSortDirection;
        }

        public int Compare( TvdbArt aArtworkA, TvdbArt aArtworkY )
        {
            try
            {
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
                        lResult = 0;
                        lResult = aArtworkA.Votes.CompareTo( aArtworkY.Votes );
                        break;

                    // default to the title field
                    default:
                        lResult = 0;
                        break;
                }
                
                if ( mSortDirection == SortingDirections.Descending )
                    lResult = -lResult;

                return lResult;
            }
            catch
            {
                return 0;
            }
        }
    }
}

