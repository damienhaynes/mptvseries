using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace WindowPlugins.GUITVSeries.Trakt
{
    static class TraktAPI
    {
        static string apiKey = DBOption.GetOptions(DBOption.cTraktAPIKey);

        public enum Status
        {
            Watching,
            Watched
        }

        /// <summary>
        /// Returns the series list for a user
        /// </summary>
        /// <param name="user">username of person to get series library</param>        
        public static IEnumerable<TraktLibraryShows> GetSeriesForUser(string user)
        {
            string seriesForUser = Transmit(string.Format(TraktURIs.UserLibraryShows, apiKey, user), string.Empty, false);
            return seriesForUser.FromJSONArray<TraktLibraryShows>();
        }

        /// <summary>
        /// Returns the series overview including ratings, top watchers, and most watched episodes      
        /// </summary>
        /// <param name="seriesID">tvdb series id of series to lookup</param>        
        public static TraktSeriesOverview GetSeriesOverview(string seriesID)
        {
            string seriesOverview = Transmit(string.Format(TraktURIs.SeriesOverview, apiKey, seriesID), string.Empty, false);
            return seriesOverview.FromJSON<TraktSeriesOverview>();
        }

        /// <summary>
        /// Returns a users online profile
        /// </summary>
        /// <param name="user">username of person to retrieve profile</param>        
        public static TraktUserProfile GetUserProfile(string user)
        {
            string userProfile = Transmit(string.Format(TraktURIs.UserProfile, apiKey, user), string.Empty, false);
            return userProfile.FromJSON<TraktUserProfile>();
        }

        /// <summary>
        /// Returns a list of watched items for a user
        /// only friends or non-private users will return any data
        /// Maximum of 100 items will be returned from API
        /// </summary>
        /// <param name="user">username of person to retrieve watched items</param>        
        public static IEnumerable<TraktWatchedHistory> GetUserWatchedHistory(string user)
        {
            string userWatchedHistory = Transmit(string.Format(TraktURIs.UserWatched, apiKey, user), string.Empty, false);

            // get list of objects from json array
            return userWatchedHistory.FromJSONArray<TraktWatchedHistory>();
        }

        /// <summary>
        /// Send Post to trakt.tv api during episode watching or after episode watched
        /// </summary>
        /// <param name="episode">Episode object being watched</param>
        /// <param name="progress">Current percentage of video complete</param>
        /// <param name="duration">Length of video in minutes</param>
        /// <param name="status">Watching or Watched</param>
        public static void SendUpdate(DBEpisode episode, int progress, int duration, Status status)
        {
            string username = DBOption.GetOptions(DBOption.cTraktUsername);
            string password = DBOption.GetOptions(DBOption.cTraktPassword);

            // check if trakt is enabled
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return;

            DBSeries series = Helper.getCorrespondingSeries(episode[DBEpisode.cSeriesID]);

            if (series == null) return;

            // create scrobble data
            TraktScrobble scrobbleData = new TraktScrobble
            {
                MediaType = "TVShow",
                Status = status.ToString(),
                Title = series.ToString(),
                Year = DBSeries.GetSeriesYear(series),
                Season = episode[DBOnlineEpisode.cSeasonIndex],
                Episode = episode[DBOnlineEpisode.cEpisodeIndex],
                SeriesID = series[DBSeries.cID],
                Progress = progress.ToString(),
                PluginVersion = Settings.Version.ToString(),
                MediaCenter = "mp-tvseries",
                MediaCenterVersion = Settings.MPVersion.ToString(),
                MediaCenterBuildDate = Settings.MPBuildDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Duration = duration.ToString(),
                UserName = username,
                Password = password
            };

            // final check that we have everything we need
            // server can accept title if series id is not supplied
            if (string.IsNullOrEmpty(scrobbleData.Title) || string.IsNullOrEmpty(scrobbleData.Season) || string.IsNullOrEmpty(scrobbleData.Episode))
                return;
            
            // serialize Scrobble object to JSON and send to server
            Transmit(TraktURIs.APIPost, scrobbleData.ToJSON(), true);
        }

        /// <summary>
        /// Uploads string to address using the Post Method
        /// </summary>
        /// <param name="address">address of resource</param>
        /// <param name="data">json string to post, can be left empty</param>
        /// <param name="notify">notify in GUI if any errors</param>
        /// <returns>response as json string</returns>
        private static string Transmit(string address, string data, bool notify)
        {
            try
            {
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", Settings.UserAgent);
                if (!string.IsNullOrEmpty(data))
                {
                    MPTVSeriesLog.Write("Trakt: Post: ", data, MPTVSeriesLog.LogLevel.Debug);
                }
                string result = client.UploadString(address, data);
                MPTVSeriesLog.Write("Trakt: {0}", result);
                return result;
            }
            catch (WebException e)
            {
                // Notify user something bad happened
                MPTVSeriesLog.Write("Trakt Error: {0}", e.Message);
                if (notify)
                {
                    TVSeriesPlugin.ShowNotifyDialog(Translation.TraktError, e.Message);
                }
                return null;
            }
        }

    }        
}
