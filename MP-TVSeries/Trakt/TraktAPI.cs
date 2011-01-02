using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace WindowPlugins.GUITVSeries.Trakt
{
    static class TraktAPI
    {
        public enum Status
        {
            Watching,
            Watched
        }

        private static class ApiURIs
        {
            public const string APIPost = @"http://api.trakt.tv/post";
            public const string SendUpdate = @"""type"":""{0}"",""status"":""{1}"",""title"":""{2}"",""year"":""{3}"",""season"":""{4}"",""episode"":""{5}"",""tvdbid"":""{6}"",""progress"":""{7}"",""plugin_version"":""{8}"",""media_center"":""{9}"",""media_center_version"":""{10}"",""media_center_date"":""{11}"",""duration"":""{12}"",""username"":""{13}"",""password"":""{14}""";
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

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return;

            DBSeries series = Helper.getCorrespondingSeries(episode[DBEpisode.cSeriesID]);

            if (series == null) return;

            string type = "TVShow";
            string title = series.ToString();
            string year = DBSeries.GetSeriesYear(series);
            string seasonIdx = episode[DBOnlineEpisode.cSeasonIndex];
            string episodeIdx = episode[DBOnlineEpisode.cEpisodeIndex];
            string tvdbid = series[DBSeries.cID];
            string version = Settings.Version.ToString();
            string mpversion = Settings.MPVersion.ToString();
            string builddate = Settings.MPBuildDate.ToString("yyyy-MM-dd HH:mm:ss");            

            // final check that we have everything we need
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(seasonIdx) || string.IsNullOrEmpty(episodeIdx))
                return;

            string data = string.Format(ApiURIs.SendUpdate, type,
                                                            status.ToString(),
                                                            title,
                                                            year,
                                                            seasonIdx,
                                                            episodeIdx,
                                                            tvdbid,
                                                            progress.ToString(),
                                                            version,
                                                            "mp-tvseries",
                                                            mpversion,
                                                            builddate,
                                                            duration.ToString(),
                                                            username,
                                                            password);
            
            Transmit(data);
        }

        private static void Transmit(string status)
        {
            try
            {
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", Settings.UserAgent);
                MPTVSeriesLog.Write("Trakt: Data: ", status, MPTVSeriesLog.LogLevel.Debug);
                string result = client.UploadString(ApiURIs.APIPost, string.Concat("{", status, "}"));
                MPTVSeriesLog.Write("Trakt: {0}", result);
            }
            catch (WebException e)
            {
                // Notify user something bad happened
                MPTVSeriesLog.Write("Trakt Error: {0}", e.Message);
                TVSeriesPlugin.ShowNotifyDialog(Translation.TraktError, e.Message);
            }
           
            return;
        }

    }        
}
