using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Trakt;
using Trakt.Show;
using Trakt.Rate;

namespace WindowPlugins.GUITVSeries
{
    public static class TraktHandler
    {
        public static bool SyncInProgress = false;

        /// <summary>
        /// Create scrobble data that can be used to send to Trakt API
        /// </summary>
        public static TraktEpisodeScrobble CreateScrobbleData(DBEpisode episode)
        {
            string username = TraktAPI.Username;
            string password = TraktAPI.Password;

            // check if trakt is enabled
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            DBSeries series = Helper.getCorrespondingSeries(episode[DBEpisode.cSeriesID]);
            if (series == null || series[DBOnlineSeries.cTraktIgnore]) return null;

            // create scrobble data
            TraktEpisodeScrobble scrobbleData = new TraktEpisodeScrobble
            {
                Title = series[DBOnlineSeries.cOriginalName],
                Year = series.Year,
                Season = episode[DBOnlineEpisode.cSeasonIndex],
                Episode = episode[DBOnlineEpisode.cEpisodeIndex],
                SeriesID = series[DBSeries.cID],
                PluginVersion = Settings.Version.ToString(),
                MediaCenter = "mp-tvseries",
                MediaCenterVersion = Settings.MPVersion.ToString(),
                MediaCenterBuildDate = Settings.MPBuildDate.ToString("yyyy-MM-dd HH:mm:ss"),
                UserName = username,
                Password = password
            };

            return scrobbleData;
        }
       
        private static TraktSync GetTraktSyncObject(DBSeries series, List<DBEpisode> episodes)
        {
            // set series properties for episodes
            TraktSync traktSync = new TraktSync
            {
                Password = TraktAPI.Password,
                UserName = TraktAPI.Username,
                SeriesID = series[DBSeries.cID],
                IMDBID = series[DBOnlineSeries.cIMDBID],
                Year = series.Year,
                Title = series[DBOnlineSeries.cOriginalName]
            };

            // get list of episodes for series
            List<TraktSync.Episode> epList = new List<TraktSync.Episode>();

            foreach (DBEpisode ep in episodes.Where(e => e[DBEpisode.cSeriesID] == series[DBSeries.cID]))
            {
                TraktSync.Episode episode = new TraktSync.Episode();
                episode.SeasonIndex = ep[DBOnlineEpisode.cSeasonIndex];
                episode.EpisodeIndex = ep[DBOnlineEpisode.cEpisodeIndex];
                epList.Add(episode);
            }

            traktSync.EpisodeList = epList;
            return traktSync;
        }

        public static List<DBEpisode> GetEpisodesToSync(TraktSyncModes mode)
        {
            // Get episodes for every series
            return GetEpisodesToSync(null, mode);
        }

        public static List<DBEpisode> GetEpisodesToSync(DBSeries series, TraktSyncModes mode)
        {
            List<DBEpisode> episodes = new List<DBEpisode>();

            SQLCondition conditions = new SQLCondition();

            if (series == null)
            {
                // Get episodes for every series
                conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, 0, SQLConditionType.GreaterThan);
            }
            else
            {
                // Get episodes for a single series
                conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, series[DBSeries.cID], SQLConditionType.Equal);
            }

            switch (mode)
            {
                case TraktSyncModes.library:
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cTraktLibrary, 0, SQLConditionType.Equal);
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cHidden, 0, SQLConditionType.Equal);
                    conditions.Add(new DBEpisode(), DBEpisode.cFilename, string.Empty, SQLConditionType.NotEqual);
                    episodes = DBEpisode.Get(conditions, false);
                    break;

                case TraktSyncModes.seen:
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cHidden, 0, SQLConditionType.Equal);
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cWatched, 1, SQLConditionType.Equal);
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cTraktSeen, 0, SQLConditionType.Equal);
                    episodes = DBEpisode.Get(conditions, false);
                    break;

                case TraktSyncModes.unseen:
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cHidden, 0, SQLConditionType.Equal);
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cTraktSeen, 2, SQLConditionType.Equal);
                    episodes = DBEpisode.Get(conditions, false);
                    break;
            }
            
            return episodes;
        }

        public static void SynchronizeLibrary(List<DBEpisode> episodes, TraktSyncModes mode)
        {
            if (episodes.Count == 0) return;

            // get unique series ids
            var uniqueSeriesIDs = (from seriesIDs in episodes
                                   select seriesIDs[DBEpisode.cSeriesID].ToString()).Distinct().ToList();

            // go over each series, can only send one series at a time
            foreach (string seriesID in uniqueSeriesIDs)
            {
                DBSeries series = Helper.getCorrespondingSeries(int.Parse(seriesID));
                if (series == null || series[DBOnlineSeries.cTraktIgnore]) continue;

                MPTVSeriesLog.Write("Trakt: Synchronizing '{0}' episodes for series '{1}'.", mode.ToString(), series.ToString());

                TraktSync traktSync = GetTraktSyncObject(series, episodes);                

                // upload to trakt
                TraktResponse response = TraktAPI.SyncEpisodeLibrary(traktSync, mode);
                if (response == null)
                {
                    MPTVSeriesLog.Write("Trakt Error: Response from server was unexpected.");
                    continue;
                }

                // check for any error and log result
                CheckTraktErrorAndNotify(response, false);

                if (response.Status == "success")
                {
                    SQLCondition conditions = new SQLCondition();

                    // flag episodes and commit to database
                    switch (mode)
                    {
                        case TraktSyncModes.seen:
                            conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, seriesID, SQLConditionType.Equal);
                            conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cWatched, 1, SQLConditionType.Equal);
                            conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cHidden, 0, SQLConditionType.Equal);
                            conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cTraktSeen, 0, SQLConditionType.Equal);

                            // we always flag traktLibrary field as the 'traktSeen' field counts as part of library
                            DBOnlineEpisode.GlobalSet(new DBOnlineEpisode(), DBOnlineEpisode.cTraktLibrary, 1, conditions);
                            DBOnlineEpisode.GlobalSet(new DBOnlineEpisode(), DBOnlineEpisode.cTraktSeen, 1, conditions);
                            break;

                        case TraktSyncModes.library:
                            // we can't do a global set as our conditions are from two different tables
                            // where filename is not empty and traktLibrary = 0
                            foreach (DBEpisode ep in episodes.Where(e => e[DBEpisode.cSeriesID] == seriesID))
                            {
                                ep[DBOnlineEpisode.cTraktLibrary] = 1;
                                ep.Commit();
                            }
                            break;

                        case TraktSyncModes.unseen:
                            conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, seriesID, SQLConditionType.Equal);
                            conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cHidden, 0, SQLConditionType.Equal);
                            conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cTraktSeen, 2, SQLConditionType.Equal);
                            DBOnlineEpisode.GlobalSet(new DBOnlineEpisode(), DBOnlineEpisode.cTraktSeen, 0, conditions);
                            break;

                        case TraktSyncModes.unlibrary:
                            break;
                    }
                }

                // wait a short period before uploading another series
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// Gets the 100 Most Recently Watched on Trakt and 
        /// syncs the watched state locally
        /// </summary>
        public static void SyncTraktWatchedState()
        {
            if (string.IsNullOrEmpty(TraktAPI.Username) || string.IsNullOrEmpty(TraktAPI.Password))
                return;

            // Get all local unwatched episodes
            SQLCondition conditions = new SQLCondition(new DBOnlineEpisode(), DBOnlineEpisode.cWatched, false, SQLConditionType.Equal);
            List<DBEpisode> episodes = DBEpisode.Get(conditions, false);

            MPTVSeriesLog.Write("Getting Most Recently Watched from Trakt:");
            
            foreach (var watchedItem in TraktAPI.GetUserWatchedHistory(TraktAPI.Username))
            {         
                string seriesID = watchedItem.Show.SeriesID;
                string seasonIdx = watchedItem.Episode.SeasonIndex;
                string episodeIdx = watchedItem.Episode.EpisodeIndex;

                foreach(DBEpisode episode in episodes.Where(e => e[DBEpisode.cSeriesID] == seriesID &&
                                                                 e[DBEpisode.cSeasonIndex] == seasonIdx &&
                                                                 e[DBEpisode.cEpisodeIndex] == episodeIdx)) {
                    // commit watched state
                    episode[DBOnlineEpisode.cWatched] = true;
                    episode.Commit();
                }
            }
            MPTVSeriesLog.Write("Finished Syncing Watched state from Trakt");

        }

        public static void RateEpisode(DBEpisode episode)
        {
            if (string.IsNullOrEmpty(TraktAPI.Username) || string.IsNullOrEmpty(TraktAPI.Password))
                return;

            new Thread(delegate()
                {
                    DBSeries series = Helper.getCorrespondingSeries(episode[DBOnlineEpisode.cSeriesID]);
                    
                    TraktRateValue loveorhate = episode[DBOnlineEpisode.cMyRating] >= 7.0 ? TraktRateValue.love : TraktRateValue.hate;

                    TraktRateEpisode episodeData = new TraktRateEpisode()
                    {
                        Episode = episode[DBOnlineEpisode.cEpisodeIndex],
                        Rating = loveorhate.ToString(),
                        Season = episode[DBOnlineEpisode.cSeasonIndex],
                        SeriesID = episode[DBOnlineEpisode.cSeriesID],
                        Year = series.Year,
                        Title = series[DBOnlineSeries.cOriginalName],
                        UserName = TraktAPI.Username,
                        Password = TraktAPI.Password
                    };

                    TraktRateResponse response = TraktAPI.RateEpisode(episodeData);
                    
                    // check for any error and notify
                    CheckTraktErrorAndNotify(response, false);
                })
                {
                    IsBackground = true,
                    Name = "Trakt Rate Episode"
                }.Start();
        }

        public static void RateSeries(DBSeries series)
        {
            if (string.IsNullOrEmpty(TraktAPI.Username) || string.IsNullOrEmpty(TraktAPI.Password))
                return;

            new Thread(delegate()
            {
                TraktRateValue loveorhate = series[DBOnlineSeries.cMyRating] >= 7.0 ? TraktRateValue.love : TraktRateValue.hate;

                TraktRateSeries seriesData = new TraktRateSeries()
                {
                    Rating = loveorhate.ToString(),
                    SeriesID = series[DBOnlineSeries.cID],
                    Year = series.Year,
                    Title = series[DBOnlineSeries.cOriginalName],
                    UserName = TraktAPI.Username,
                    Password = TraktAPI.Password,
                };

                TraktRateResponse response = TraktAPI.RateSeries(seriesData);

                // check for any error and notify
                CheckTraktErrorAndNotify(response, false);
            })
            {
                IsBackground = true,
                Name = "Trakt Rate Series"
            }.Start();
        }

        /// <summary>
        /// Notify user in GUI if an error state was returned from Trakt API
        /// </summary>
        public static void CheckTraktErrorAndNotify<T>(T response, bool notify)
        {
            var r = response as TraktResponse;

            if (r == null || r.Status == null) return;

            // check response error status
            if (r.Status != "success")
            {
                MPTVSeriesLog.Write("Trakt Error: {0}", r.Error);
                if (notify) TVSeriesPlugin.ShowNotifyDialog(Translation.TraktError, r.Error);
            }
            else
            {
                // success
                MPTVSeriesLog.Write("Trakt Response: {0}", r.Message);
            }
        }

    }
}
