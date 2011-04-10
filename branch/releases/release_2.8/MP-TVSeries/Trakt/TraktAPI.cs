﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Trakt.Show;
using Trakt.User;
using Trakt.Movie;
using Trakt.Rate;
using WindowPlugins.GUITVSeries;

namespace Trakt
{
    public enum TraktScrobbleStates
    {
        watching,
        scrobble
    }

    public enum TraktSyncModes
    {
        library,
        seen,
        unlibrary,
        unseen
    }

    public enum TraktRateType
    {
        episode,
        show,
        movie
    }

    public enum TraktRateValue
    {
        love,
        hate
    }

    static class TraktAPI
    {
        private const string cAPIKey = "5daf4d0b339a90d7473c6f1ed7f609c4e69f92b4";

        /// <summary>
        /// Trakt Username
        /// </summary>
        public static string Username { get; set; }

        /// <summary>
        /// Trakt Password with SHA1 Hash
        /// </summary>
        public static string Password { get; set; }

        /// <summary>
        /// UserAgent header used to Post to Trakt API
        /// </summary>
        public static string UserAgent { get; set; }

        /// <summary>
        /// Returns list of episodes in Users Calendar
        /// </summary>
        /// <param name="user">username of person to get Calendar</param>
        public static IEnumerable<TraktUserCalendar> GetCalendarForUser(string user)
        {   
            // 7-Days from Today
            // All Dates should be in PST (GMT-8)
            DateTime dateNow = DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));
            return GetCalendarForUser(user, dateNow.ToString("yyyyMMdd"), "7");
        }

        /// <summary>
        /// Returns list of episodes in Users Calendar
        /// </summary>
        /// <param name="user">username of person to get Calendar</param>
        /// <param name="startDate">Start Date of calendar in form yyyyMMdd (GMT-8hrs)</param>
        /// <param name="days">Number of days to return in calendar</param>
        public static IEnumerable<TraktUserCalendar> GetCalendarForUser(string user, string startDate, string days)
        {
            string userCalendar = Transmit(string.Format(TraktURIs.UserCalendarShows, user, startDate, days), GetUserAuthentication());
            return userCalendar.FromJSONArray<TraktUserCalendar>();
        }

        /// <summary>
        /// Returns the series list for a user
        /// </summary>
        /// <param name="user">username of person to get series library</param>
        public static IEnumerable<TraktLibraryShows> GetSeriesForUser(string user)
        {
            string seriesForUser = Transmit(string.Format(TraktURIs.UserLibraryShows, user), GetUserAuthentication());
            return seriesForUser.FromJSONArray<TraktLibraryShows>();
        }

        /// <summary>
        /// Returns the movie list for a user
        /// </summary>
        /// <param name="user">username of person to get movie library</param>
        public static IEnumerable<TraktLibraryMovies> GetMoviesForUser(string user)
        {
            string moviesForUser = Transmit(string.Format(TraktURIs.UserLibraryMovies, user), GetUserAuthentication());
            return moviesForUser.FromJSONArray<TraktLibraryMovies>();
        }

        /// <summary>
        /// Returns the series overview including ratings, top watchers, and most watched episodes      
        /// </summary>
        /// <param name="seriesID">tvdb series id of series to lookup</param>
        public static TraktSeriesOverview GetSeriesOverview(string seriesID)
        {
            string seriesOverview = Transmit(string.Format(TraktURIs.SeriesOverview, seriesID), string.Empty);
            return seriesOverview.FromJSON<TraktSeriesOverview>();
        }

        /// <summary>
        /// Returns a users online profile
        /// </summary>
        /// <param name="user">username of person to retrieve profile</param>
        public static TraktUserProfile GetUserProfile(string user)
        {
            string userProfile = Transmit(string.Format(TraktURIs.UserProfile, user), GetUserAuthentication());
            return userProfile.FromJSON<TraktUserProfile>();
        }

        /// <summary>
        /// Returns a list of Friends and their user profiles
        /// </summary>
        /// <param name="user">username of person to retrieve friends list</param>
        public static IEnumerable<TraktUserProfile> GetUserFriends(string user)
        {
            string userFriends = Transmit(string.Format(TraktURIs.UserFriends, user), GetUserAuthentication());
            return userFriends.FromJSONArray<TraktUserProfile>();
        }

        /// <summary>
        /// Returns a list of watched items for a user
        /// only friends or non-private users will return any data
        /// Maximum of 100 items will be returned from API
        /// </summary>
        /// <param name="user">username of person to retrieve watched items</param>
        public static IEnumerable<TraktWatchedEpisodeHistory> GetUserWatchedHistory(string user)
        {
            string userWatchedHistory = Transmit(string.Format(TraktURIs.UserMostRecentWatchedEpisodes, user), GetUserAuthentication());

            // get list of objects from json array
            return userWatchedHistory.FromJSONArray<TraktWatchedEpisodeHistory>();
        }

        /// <summary>
        /// Returns a list of all watched/seen items on trakt.tv        
        /// </summary>
        /// <param name="user">username of person to retrieve watched items</param>
        public static IEnumerable<TraktWatchedShows> GetUserWatched(string user)
        {
            string userWatched = Transmit(string.Format(TraktURIs.UserWatchedEpisodes, user), GetUserAuthentication());

            // get list of objects from json array
            return userWatched.FromJSONArray<TraktWatchedShows>();
        }

        /// <summary>
        /// Send Post to trakt.tv api during episode watching or after episode watched
        /// </summary>
        /// <param name="scrobbleData">Episode object being scrobbled</param>
        /// <param name="status">Watching or Watched</param>
        public static TraktResponse ScrobbleShowState(TraktEpisodeScrobble scrobbleData, TraktScrobbleStates status)
        {
            // check that we have everything we need
            if (string.IsNullOrEmpty(scrobbleData.SeriesID) || string.IsNullOrEmpty(scrobbleData.Season) || string.IsNullOrEmpty(scrobbleData.Episode))
            {
                TraktResponse error = new TraktResponse
                {
                    Error = Translation.TraktNotEnoughInfo,
                    Status = "failure"
                };
                return error;
            }
            
            // serialize Scrobble object to JSON and send to server
            string response = Transmit(string.Format(TraktURIs.ScrobbleShow, status.ToString()), scrobbleData.ToJSON());

            // return success or failure
            return response.FromJSON<TraktResponse>();
        }

        /// <summary>
        /// Send Post to trakt.tv api during movie watching or after movie watched
        /// </summary>
        /// <param name="scrobbleData">Movie object being scrobbled</param>
        /// <param name="status">Watching or Watched</param>
        public static TraktResponse ScrobbleMovieState(TraktMovieScrobble scrobbleData, TraktScrobbleStates status)
        {
            // check that we have everything we need
            // server can accept title if movie id is not supplied
            if (string.IsNullOrEmpty(scrobbleData.Title) || string.IsNullOrEmpty(scrobbleData.Year))
            {
                TraktResponse error = new TraktResponse
                {
                    Error = Translation.TraktNotEnoughInfo,
                    Status = "failure"
                };
                return error;
            }

            // serialize Scrobble object to JSON and send to server
            string response = Transmit(string.Format(TraktURIs.ScrobbleMovie, status.ToString()), scrobbleData.ToJSON());

            // return success or failure
            return response.FromJSON<TraktResponse>();
        }

        /// <summary>
        /// Sync episode library with Trakt
        /// </summary>
        /// <param name="syncData">Series and List of episodes</param>
        /// <param name="mode">Sync mode operation</param>
        public static TraktResponse SyncEpisodeLibrary(TraktSync syncData, TraktSyncModes mode)
        {
            // check that we have everything we need
            // server can accept title/year if imdb id is not supplied
            if (string.IsNullOrEmpty(syncData.SeriesID))
            {
                TraktResponse error = new TraktResponse
                {
                    Error = Translation.TraktNotEnoughInfo,
                    Status = "failure"
                };
                return error;
            }

            // serialize Scrobble object to JSON and send to server
            string response = Transmit(string.Format(TraktURIs.SyncEpisodeLibrary, mode.ToString()), syncData.ToJSON());

            // return success or failure
            return response.FromJSON<TraktResponse>();
        }

        public static TraktRateResponse RateEpisode(TraktRateEpisode episode)
        {
            string response = Transmit(string.Format(TraktURIs.RateItem, TraktRateType.episode.ToString()), episode.ToJSON());
            return response.FromJSON<TraktRateResponse>();
        }

        public static TraktRateResponse RateSeries(TraktRateSeries series)
        {
            string response = Transmit(string.Format(TraktURIs.RateItem, TraktRateType.show.ToString()), series.ToJSON());
            return response.FromJSON<TraktRateResponse>();
        }

        private static string GetUserAuthentication()
        {
            return new TraktAuthentication { Username = TraktAPI.Username, Password = TraktAPI.Password }.ToJSON();
        }

        /// <summary>
        /// Uploads string to address using the Post Method
        /// </summary>
        /// <param name="address">address of resource</param>
        /// <param name="data">json string to post, can be left empty</param>
        /// <param name="notify">notify in GUI if any errors</param>
        /// <returns>response as json string</returns>
        private static string Transmit(string address, string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                MPTVSeriesLog.Write("Trakt Post: ", data, MPTVSeriesLog.LogLevel.Normal);
            }

            // Update API key in placeholder
            address = address.Replace("<apiKey>", cAPIKey);

            try
            {
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", UserAgent);
                return client.UploadString(address, data);
            }
            catch (WebException e)
            {
                // something bad happened e.g. invalid login
                TraktResponse error = new TraktResponse
                {
                    Status = "failure",
                    Error = e.Message
                };
                return error.ToJSON();
            }
        }

    }
}
