using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Trakt.Show;
using Trakt.User;
using Trakt.Movie;
using WindowPlugins.GUITVSeries;

namespace Trakt
{
    static class TraktAPI
    {
        public enum Status
        {
            watching,
            scrobble
        }

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
        /// Returns the series list for a user
        /// </summary>
        /// <param name="user">username of person to get series library</param>
        public static IEnumerable<TraktLibraryShows> GetSeriesForUser(string user)
        {
            string seriesForUser = Transmit(string.Format(TraktURIs.UserLibraryShows, user), string.Empty);
            return seriesForUser.FromJSONArray<TraktLibraryShows>();
        }

        /// <summary>
        /// Returns the movie list for a user
        /// </summary>
        /// <param name="user">username of person to get movie library</param>
        public static IEnumerable<TraktLibraryMovies> GetMoviesForUser(string user)
        {
            string moviesForUser = Transmit(string.Format(TraktURIs.UserLibraryMovies, user), string.Empty);
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
            string userProfile = Transmit(string.Format(TraktURIs.UserProfile, user), string.Empty);
            return userProfile.FromJSON<TraktUserProfile>();
        }

        /// <summary>
        /// Returns a list of watched items for a user
        /// only friends or non-private users will return any data
        /// Maximum of 100 items will be returned from API
        /// </summary>
        /// <param name="user">username of person to retrieve watched items</param>
        public static IEnumerable<TraktWatchedEpisodeHistory> GetUserWatchedHistory(string user)
        {
            string userWatchedHistory = Transmit(string.Format(TraktURIs.UserWatchedEpisodes, user), string.Empty);

            // get list of objects from json array
            return userWatchedHistory.FromJSONArray<TraktWatchedEpisodeHistory>();
        }

        /// <summary>
        /// Send Post to trakt.tv api during episode watching or after episode watched
        /// </summary>
        /// <param name="scrobbleData">Episode object being scrobbled</param>
        /// <param name="status">Watching or Watched</param>
        public static TraktResponse ScrobbleShowState(TraktEpisodeScrobble scrobbleData, Status status)
        {
            // check that we have everything we need
            // server can accept title/year if imdb id is not supplied
            if (string.IsNullOrEmpty(scrobbleData.Title) || string.IsNullOrEmpty(scrobbleData.Season) || string.IsNullOrEmpty(scrobbleData.Episode))
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
        public static TraktResponse ScrobbleMovieState(TraktMovieScrobble scrobbleData, Status status)
        {
            // check that we have everything we need
            // server can accept title if series id is not supplied
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
