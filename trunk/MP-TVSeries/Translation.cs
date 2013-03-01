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
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using MediaPortal.Configuration;
using MediaPortal.GUI.Library;

namespace WindowPlugins.GUITVSeries
{
    static class Translation
    {        
        /// <summary>
        /// These will be loaded with the language files content
        /// if the selected lang file is not found, it will first try to load en-US.xml as a backup
        /// if that also fails it will use the hardcoded strings as a last resort.
        /// </summary>
        #region Translatable Fields
        public static string Series = "Series";
        public static string Series_Plural = "Series";
        public static string Season = "Season";
        public static string Seasons = "Seasons";
        public static string Episode = "Episode";
        public static string Episodes = "Episodes";
        public static string Toggle_watched_flag = "Toggle Watched";
        public static string Retrieve_Subtitle = "Retrieve Subtitle";
        public static string Mark_all_as_watched = "Mark all as Watched";
        public static string Mark_all_as_unwatched = "Mark all as Unwatched";
        public static string Hide = "Hide";
        public static string UnHide = "UnHide";
        public static string Delete = "Delete";
        public static string SortBy = "Sort By";
        public static string DVDOrder = "DVD Order";
        public static string AiredOrder = "Aired Order";
        public static string AbsoluteOrder = "Absolute Order";
        public static string Title = "Title";
        public static string ChangeOnlineMatchOrder = "Change Online Matching Method";
        public static string Remove_series_from_Favourites = "Remove from Favourites";
        public static string Add_series_to_Favourites = "Add to Favourites";
        public static string Force_Local_Scan = "Force Local Scan ";
        public static string Force_Online_Refresh = "Force Online Refresh ";
        public static string In_Progress_with_Barracks = "(In Progress)";
        public static string ShowAllEpisodes = "Show all Episodes";
        public static string ShowHiddenItems = "Show Hidden Items";
        public static string Hide_summary_on_unwatched = "Hide Spoilers";
        public static string Hide_thumbnail_on_unwatched = "Hide Spoiler Thumbnails";
        public static string on = "on";
        public static string off = "off";
        public static string Play_Random_Episode = "Play Random Episode";
        public static string Play_Random_First_Unwatched_Episode = "Play Random First Unwatched Episode";
        public static string Delete_that_series = "Delete this series?";
        public static string Delete_that_season = "Delete this season?";
        public static string Delete_that_episode = "Delete this episode?";
        public static string Confirm = "Confirm";
        public static string Completed = "Completed";        
        public static string _Hidden_to_prevent_spoilers_ = " * Hidden to prevent spoilers *";
        public static string Yes = "Yes";
        public static string No = "No";
        public static string Error = "#Error";
        public static string No_items = "No items!";
        public static string Unknown = "Unknown";
        public static string wrongSkin = "Wrong Skin file";
        public static string special = "Special";
        public static string specials = "Specials";
        public static string delPhyiscalWarning = "You are about to permanently delete {0} physical file(s).\nWould you like to proceed?";
        public static string Force_Online_Match = "Force New Online Lookup For Series";        
        public static string Download = "Download";
        public static string Actions = "Actions";
        public static string Options = "Options";
        public static string updateMI = "Update Media Info";
        public static string insertDisk = "Please insert Disk";
        public static string InsertDiskMessage1 = "The media for the episode you have selected";
        public static string InsertDiskMessage2 = "is currently not available.";
        public static string InsertDiskMessage3 = "Please insert or connect media";
        public static string InsertDiskMessage4 = "labeled: {0}";
        public static string OK = "OK";
        public static string Cancel = "Cancel";
        public static string Ignore = "Ignore";
        public static string RateSeries = "Rate this Series";
        public static string RateEpisode = "Rate this Episode";
        public static string ResetRating = "Reset Rating";
        public static string AskToRate = "Ask me to rate unrated episodes";
        public static string DontAskToRate = "Don't ask me to rate again";
        public static string RatingStar = "Star";
        public static string RatingStars = "Stars";                
        public static string ChangeLayout = "Change Layout";
        public static string UseOnlineFavourites = "Use Online Favourites";
        public static string AddToPlaylist = "Add to Playlist";
		public static string NoPlaylistsFound = "No Playlists found in:";
        public static string SortByStrings = "the|a|an";
        public static string DeleteFromDisk = "Delete from Disk";       
        public static string DeleteFromDatabase = "Delete from Database";     
        public static string DeleteFromFileDatabase = "Delete from Disk and Database";
        public static string DeleteSubtitles = "Delete Subtitles";
        public static string RateDialogLabel = "Select Your Rating for {0}";
        public static string CycleSeriesBanner = "Cycle Series Banner";
        public static string CycleSeriesPoster = "Cycle Series Poster";
        public static string CycleSeriesThumb = "Cycle Series Panel";
        public static string CycleSeasonBanner = "Cycle Season Poster";
		public static string Update = "Update";
		public static string DeleteThumbnailsHeading = "Delete Thumbnails";
		public static string DeleteThumbnailsLine1 = "Would you also like to Delete and Re-Download";
		public static string DeleteThumbnailsLine2 = "all episode thumbnails?";
        public static string EpisodeFilenameEmpty = "Episode Filename is Empty in Database";
        public static string UnableToDeleteSubtitleFile = "Unable to Delete Subtitle File: {0}";
        public static string PathNotAvailable = "Path is not available: {0}";
        public static string UnableToDeleteSubtitles = "Unable to Delete Subtitles";
        public static string ErrorClear = "Error";
        public static string UnableToDelete = "Unable to Delete";
        public static string UnableToDeleteFile = "Unable to Delete File: {0}";

        // Views
        public static string Genres = "Genres";
        public static string All = "All";
        public static string Latest = "Latest Aired";
        public static string Channels = "Channels";
        public static string Unwatched = "Unwatched";
        public static string Favourites = "Favourites";
        public static string OnlineFavourites = "Online Favourites";
        public static string RecentlyAdded = "Recently Added";
		public static string ContentRating = "Content Rating";
        public static string ChangeView = "Change View";
		public static string ViewTags = "View Tags";
        public static string ChangeViewFast = "Immediately Change Views";
		public static string AddViewTag = "Add Series to View";
		public static string RemoveViewTag = "Remove Series from View";
		public static string NewViewTag = "Create New View";        
        public static string ViewTagExistsMessage = "The view \"{0}\" already exists";
        public static string PinCode = "Pin Code";
        public static string PinCodeIncorrectLine1 = "The entered pin code is incorrect";
        public static string PinCodeIncorrectLine2 = "Press OK to try again";
        public static string ParentalControlLocked = "Lock Views";
        public static string ViewIsLocked  = "View Locked";
		public static string PinCodeDlgLabel1 = "The view \"{0}\" is locked";
		public static string PinCodeDlgLabel2 = "Enter in 4 Digit Pin Code to proceed:";
		public static string PinCodeMessageIncorrect = "Pin Incorrect";
		public static string Continuing = "Continuing Series";
		public static string Ended = "Ended Series";
		public static string Top10User = "Top 10 (User)";
		public static string Top10Online = "Top 10 (Online)";
		public static string Top25User = "Top 25 (User)";
		public static string Top25Online = "Top 25 (Online)";
		public static string HighDefinition = "High Definition";
		public static string StandardDefinition = "Standard Defintion";
		public static string Subtitles = "Subtitles";
		public static string MultiAudio = "Multiple Audio Tracks";
        public static string Actors = "Actors";
        public static string RemovableMedia = "Removable Media";
        public static string LocalMedia = "Local Media";
        public static string AdultSeries = "Adult Series";
        public static string KidsSeries = "Kids Series";
        public static string AvailableMedia = "Available Media";

        // Fanart
        public static string FanArt = "Fanart";
        public static string FanListAll = "List all Fanart for this Series";
        public static string FanArtGetAndUse = "Download Fanart and Use";
        public static string FanArtGet = "Download Fanart";
        public static string FanArtUse = "Set Fanart as Default";
        public static string FanArtDelete = "Delete Fanart";
        public static string FanDownloadingStatus = "Downloading {0} Fanart...";        
        public static string FanArtLocal = "Local";
        public static string FanArtOnline = "Remote";
        public static string FanArtOnlineLoading = "Loading {0} of {1} Fanart...";
        public static string FanArtRandom = "Display random Fanart";
        public static string FanArtNoneFound = "No Fanart could be found";        
        public static string FanArtFilter = "Filter";
        public static string FanArtFilterAll = "All";
        public static string FanartDisableLabel = "Disabled";
        public static string FanartMenuEnable = "Enable Fanart";
        public static string FanartMenuDisable = "Disable Fanart";
        public static string FanartRandomInterval = "Fanart Timer Interval";
        public static string ClearFanartCache = "Clear Fanart Cache";
        
        public static string FanartIntervalFiveSeconds = "5 Seconds";
        public static string FanartIntervalTenSeconds = "10 Seconds";
        public static string FanartIntervalFifteenSeconds = "15 Seconds";
        public static string FanartIntervalThirtySeconds = "30 Seconds";
        public static string FanartIntervalFortyFiveSeconds = "45 Seconds";
        public static string FanartIntervalSixtySeconds = "60 Seconds";

        // Layouts
        public static string LayoutList = "List";
        public static string LayoutSmallIcons = "Small Icons";
        public static string LayoutWideBanners = "Wide Banners";
        public static string LayoutListBanners = "List Banners";
        public static string LayoutListPosters = "List Posters";
        public static string LayoutListThumbs = "List Panels";
        public static string LayoutFilmstrip = "Filmstrip";
        public static string LayoutCoverflow = "Coverflow";

        // Skin Controls
        public static string ButtonAutoPlay = "Auto Play";
        public static string ButtonSwitchView = "Switch View";
        public static string ButtonRunImport = "Run Import";
        public static string ButtonChangeLayout = "Change Layout";        
        public static string ButtonOptions = "Options";
        public static string ButtonRandomFanart = "Random Fanart";        
        public static string LabelResolution = "Resolution:";
        public static string LabelChosen = "Default:";
        public static string LabelDisabled = "Disabled:";

        // ChooseFromSelectionDescriptor
        public static string CFS_Choose_Item = "Choose item";
        public static string CFS_Select_Matching_Item = "Select matching item:";
        public static string CFS_Search_Again = "Search again";                
        public static string CFS_Choose_Correct_Series = "Choose Correct Series";
        public static string CFS_Local_Series  = "Local Series";
        public static string CFS_Available_Series = "Available Series";        
        public static string CFS_Select_Version = "Version:";
        public static string CFS_Choose_Correct_Episode = "Choose Correct Episode";
        public static string CFS_Local_Episode_Index = "Local Episiode index:";
        public static string CFS_Available_Episode_List = "Available Episode list:";      
        public static string CFS_Looking_For = "Looking for:";        
        public static string CFS_Skip = "Skip";
        public static string CFS_Skip_Never_Ask_Again = "Skip / Never Ask Again";
        public static string CFS_No_Match_Manual_Search = "No Match Found, Manual Search...";
        public static string CFS_No_Results_Found = "No Results Found!";
         
        // TVDB Errors/Messages
        public static string TVDB_ERROR_TITLE = "Online TV Database Error";
        public static string TVDB_INFO_TITLE = "Online TV Database";
        public static string TVDB_INFO_ACCOUNTID_1 = "Account Identifier is not set";
        public static string TVDB_INFO_ACCOUNTID_2 = "Enter your online account ID in Configuration";
        public static string TVDB_ERROR_UNAVAILABLE_1 = "TheTVDB.com is currently unavailable";
        public static string TVDB_ERROR_UNAVAILABLE_2 = "Please try again later";
        public static string NETWORK_ERROR_UNAVAILABLE_1 = "Network connection is unavailable";
        public static string NETWORK_ERROR_UNAVAILABLE_2 = "Check your connection and try again";

		// Rate Movie Descriptions - 5 Stars
		public static string RateFiveStarOne = "Terrible";
		public static string RateFiveStarTwo = "Mediocre";
		public static string RateFiveStarThree = "Good";
		public static string RateFiveStarFour = "Superb";
		public static string RateFiveStarFive = "Perfect";

		// Rate Movie Descriptions - 10 Stars
		public static string RateTenStarOne = "Abysmal";
		public static string RateTenStarTwo = "Terrible";
		public static string RateTenStarThree = "Bad";
		public static string RateTenStarFour = "Poor";
		public static string RateTenStarFive = "Mediocre";
		public static string RateTenStarSix = "Fair";
		public static string RateTenStarSeven = "Good";
		public static string RateTenStarEight = "Great";
		public static string RateTenStarNine = "Superb";
		public static string RateTenStarTen = "Perfect";

		// Dialog Names
		public static string RateDialog = "Rate Dialog";
		public static string PinCodeDialog = "Pin Code Dialog";

        // Additional Skin Fields        
        public static string Aired = "Aired";
        public static string AiredStatusContinuing = "Continuing";
        public static string AiredStatusEnded = "Ended";
        public static string Airs = "Airs";
        public static string AirsDay = "Airs Day";
        public static string AirsTime = "Airs Time";
        public static string Cast = "Cast";
        public static string Certification = "Certification";
        public static string Director = "Director";
        public static string Directors = "Directors";
        public static string FileSize = "File Size";
        public static string FirstAired = "First Aired";
        public static string Genre = "Genre";
        public static string Group = "Group";
        public static string Groups = "Groups";
        public static string GuestStar = "Guest Star";
        public static string GuestStars = "Guest Stars";
        public static string LastOnlineUpdate = "Last Online Update";
        public static string MediaInfo = "Media Info";
        public static string Minutes = "Minutes";
        public static string MyRating = "My Rating";
        public static string Network = "Network";
        public static string Playlist = "Playlist";
        public static string Runtime = "Runtime";
        public static string Rating = "Rating";
        public static string Rated = "Rated";
        public static string Votes = "votes";
        public static string SeriesDetails = "Series Details";
        public static string SeriesStatus = "Series Status";
        public static string Starring = "Starring";
        public static string Watched = "Watched";
        public static string Writer = "Writer";
        public static string Writers = "Writers";
        public static string PlaySomething = "Play...";
        public static string PlayError = "Play error";
        public static string RandomEpisode = "random episode";
        public static string FirstUnwatchedEpisode = "first unwatched episode";
        public static string RandomUnwatchedEpisode = "random unwatched episode";
        public static string LatestEpisode = "latest episode";
        public static string UnableToFindAny = "Unable to find any {0}";
        public static string UnableToFindAnyEpisode = "Unable to find any episodes";
               
        public static string PlayNow = "Play Now";

        public static string ResumeEpisode = "Resume episode from last time?";

        public static string SummaryNotAvailable = "No summary is currently available.";

        public static string Timeout = "Timeout";
        public static string NoActors = "There are no Actors for this series.";
        public static string GettingActors = "Getting Actors from Online";

        public static string DownloadAllEpisodeInfo = "Download All Episode Info";
        
        public static string Calendar = "Calendar";
        public static string Recommendations = "Recommendations";
        public static string Trending = "Trending";
        public static string WatchList = "Watch List";
        public static string Shouts = "Shouts";
        public static string MainMenu = "Main Menu";
        public static string AddToWatchList = "Add {0} to Watchlist";
        public static string TraktNotLoggedIn = "You can not do this operation without being\nlogged in. Would you like to Signup or Login\nto trakt.tv now?";
        public static string RelatedSeries = "Related Series";

        public static string SearchTorrent = "Torrent Search";
        public static string SearchNZB = "NZB Search";
        public static string SearchActorInMovies = "Search for Actor in Movies";

        public static string AddToCustomList = "Add {0} to Custom List...";

        public static string Warning = "Warning";
        public static string PlaybackOutOfOrderLine1 = "You have not yet watched/collected";
        public static string PlaybackOutOfOrderLine2 = "Are you sure you want to continue?";
        public static string Filters = "Filters";
        public static string UnwatchedEpisodes = "UnWatched Episodes";
        public static string AllEpisodes = "All Episodes";
        #endregion

        #region Private variables

        private static Dictionary<string, string> translations;
        private static Regex translateExpr = new Regex(@"\$\{([^\}]+)\}");
        private static string path = string.Empty;

        #endregion

        #region Constructor

        static Translation()
        {

        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the translated strings collection in the active language
        /// </summary>
        public static Dictionary<string, string> Strings
        {
            get
            {
                if (translations == null)
                {
                    translations = new Dictionary<string, string>();
                    Type transType = typeof(Translation);
                    FieldInfo[] fields = transType.GetFields(BindingFlags.Public | BindingFlags.Static);
                    foreach (FieldInfo field in fields)
                    {
                        translations.Add(field.Name, field.GetValue(transType).ToString());
                    }
                }
                return translations;
            }
        }

        public static string CurrentLanguage
        {
            get
            {
                string language = string.Empty;
                try
                {
                    language = GUILocalizeStrings.GetCultureName(GUILocalizeStrings.CurrentLanguage());
                }
                catch (Exception)
                {
                    language = CultureInfo.CurrentUICulture.Name;
                }
                return language;
            }
        }
        public static string PreviousLanguage { get; set; }

        #endregion

        #region Public Methods

        public static void Init()
        {
            translations = null;
            MPTVSeriesLog.Write("Using language " + CurrentLanguage);

            path = Config.GetSubFolder(Config.Dir.Language, "MP-TVSeries");

            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            string lang = PreviousLanguage = CurrentLanguage;
            LoadTranslations(lang);

            // publish all available translation strings
            // so skins have access to them
            foreach (string name in Strings.Keys)
            {
                GUIPropertyManager.SetProperty("#TVSeries.Translation." + name + ".Label", Translation.Strings[name]);
            }
        }

        public static int LoadTranslations(string lang)
        {
            XmlDocument doc = new XmlDocument();
            Dictionary<string, string> TranslatedStrings = new Dictionary<string, string>();
            string langPath = string.Empty;

            try
            {
                langPath = Path.Combine(path, lang + ".xml");
                doc.Load(langPath);
            }
            catch (Exception e)
            {
                if (lang == "en")
                    return 0; // otherwise we are in an endless loop!

                if (e.GetType() == typeof(FileNotFoundException))
                    MPTVSeriesLog.Write("Cannot find translation file {0}. Falling back to English", langPath);
                else
                    MPTVSeriesLog.Write("Error in translation xml file: {0}. Falling back to English", lang);

                return LoadTranslations("en");
            }

            foreach (XmlNode stringEntry in doc.DocumentElement.ChildNodes)
            {
                if (stringEntry.NodeType == XmlNodeType.Element)
                {
                    try
                    {
                        TranslatedStrings.Add(stringEntry.Attributes.GetNamedItem("name").Value, stringEntry.InnerText.NormalizeTranslation());
                    }
                    catch (Exception ex)
                    {
                        MPTVSeriesLog.Write("Error in Translation Engine", ex.Message);
                    }
                }
            }

            Type TransType = typeof(Translation);
            FieldInfo[] fieldInfos = TransType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo fi in fieldInfos)
            {
                if (TranslatedStrings != null && TranslatedStrings.ContainsKey(fi.Name))
                    TransType.InvokeMember(fi.Name, BindingFlags.SetField, null, TransType, new object[] { TranslatedStrings[fi.Name] });
                else
                    MPTVSeriesLog.Write("Translation not found for name: {0}. Using hard-coded English default.", fi.Name);
            }
            return TranslatedStrings.Count;
        }

        public static string GetByName(string name)
        {
            if (!Strings.ContainsKey(name))
                return name;

            return Strings[name];
        }

        public static string GetByName(string name, params object[] args)
        {
            return String.Format(GetByName(name), args);
        }

        /// <summary>
        /// Takes an input string and replaces all ${named} variables with the proper translation if available
        /// </summary>
        /// <param name="input">a string containing ${named} variables that represent the translation keys</param>
        /// <returns>translated input string</returns>
        public static string ParseString(string input)
        {
            MatchCollection matches = translateExpr.Matches(input);
            foreach (Match match in matches)
            {
                input = input.Replace(match.Value, GetByName(match.Groups[1].Value));
            }
            return input;
        }

        /// <summary>
        /// Temp workaround to remove unwatched chars from Transifex
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string NormalizeTranslation(this string input)
        {
            input = input.Replace("\\'", "'");
            input = input.Replace("\\\"", "\"");
            return input;
        }

        #endregion

    }
}