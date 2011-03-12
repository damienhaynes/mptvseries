using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Follwit.API;
using Follwit.API.Data;
using System.Threading;
using WindowPlugins.GUITVSeries.Configuration;

namespace WindowPlugins.GUITVSeries.FollwitTv {
    public class FollwitConnector {
        internal static object follwitApiLock = new Object();
        internal static Thread updateThread = null;

        public static bool Enabled {
            get { return DBOption.GetOptions(DBOption.cFollwitEnabled); }
            internal set { DBOption.SetOptions(DBOption.cFollwitEnabled, value); }
        }

        public static FollwitApi FollwitApi {
            get {
                if (Enabled && _follwitAPI == null) {
                    try {
                        DateTime start = DateTime.Now;
                        lock (follwitApiLock) _follwitAPI = FollwitApi.Login(Username, HashedPassword, ApiUrl);
                        
                        if (_follwitAPI == null) {
                            MPTVSeriesLog.Write("[follw.it] Failed to log in: Invalid Username or Password!");
                        }

                        if (_follwitAPI != null) {
                            _follwitAPI.RequestEvent += new Follwit.API.FollwitApi.FitAPIRequestDelegate(_follwitAPI_RequestEvent);
                            _follwitAPI.ResponseEvent += new Follwit.API.FollwitApi.FitAPIResponseDelegate(_follwitAPI_ResponseEvent);

                            MPTVSeriesLog.Write("[follw.it] Logged in as: {0} ({1})", _follwitAPI.User.Name, DateTime.Now - start);
                        }
                    }
                    catch (Exception ex) {
                        MPTVSeriesLog.Write("[follwit] Failed to log in: " + ex.Message);
                    }
                }

                return _follwitAPI;
            }
        } private static FollwitApi _follwitAPI = null;

        public static string ApiUrl {
            get { return BaseUrl + "api/2/"; }
        }

        public static string BaseUrl {
            get { return DBOption.GetOptions(DBOption.cFollwitBaseUrl); }
            internal set { 
                DBOption.SetOptions(DBOption.cFollwitBaseUrl, value);
                _follwitAPI = null;
            }
        }

        public static string Username {
            get { return DBOption.GetOptions(DBOption.cFollwitUsername); }
            internal set { 
                DBOption.SetOptions(DBOption.cFollwitUsername, value);
                _follwitAPI = null;
            }
        }

        internal static string Password {
            set { 
                DBOption.SetOptions(DBOption.cFollwitHashedPassword, value);
                _follwitAPI = null;
            }
        }

        public static string HashedPassword {
            get { return DBOption.GetOptions(DBOption.cFollwitHashedPassword); }
            internal set { DBOption.SetOptions(DBOption.cFollwitHashedPassword, value); }
        }

        public static DateTime LastUpdated {
            get { return new DateTime(DBOption.GetOptions(DBOption.cFollwitLastUpdated)); }
            internal set { DBOption.SetOptions(DBOption.cFollwitLastUpdated, value.Ticks); }
        }

        public static TimeSpan UpdateFrequency {
            get { return new TimeSpan(0, DBOption.GetOptions(DBOption.cFollwitUpdateFrequency), 0); }
            internal set { DBOption.SetOptions(DBOption.cFollwitHashedPassword, value.TotalMinutes); }
        }

        static FollwitConnector() {
            FollwitConnector.InitUpdateThread();
        }

        public static void SyncNewEpisodes() {
            SyncNewEpisodes(null);
        }

        public static void SyncNewEpisodes(ProgressDialog.ProgressDelegate progress) {
            // grab list of all local episodes
            SQLCondition condition = new SQLCondition();
            condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cFollwitId, 0, SQLConditionType.Equal);
            List<DBEpisode> episodes = DBEpisode.Get(condition, false);

            SyncEpisodes(episodes, progress);
        }

        public static void SyncAllEpisodes() {
            SyncAllEpisodes(null);
        }

        public static void SyncAllEpisodes(ProgressDialog.ProgressDelegate progress) {
            // grab list of all local episodes
            SQLCondition condition = new SQLCondition();
            condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, 0, SQLConditionType.GreaterThan);
            List<DBEpisode> episodes = DBEpisode.Get(condition, false);

            SyncEpisodes(episodes, progress);
        }

        public static void SyncEpisodes(List<DBEpisode> episodes) {
            SyncEpisodes(episodes, null);
        }

        public static void SyncEpisodes(List<DBEpisode> episodes, ProgressDialog.ProgressDelegate progress) {
            if (!Enabled || episodes == null) return;

            Thread thread = new Thread(new ThreadStart(delegate {
                try {
                    MPTVSeriesLog.Write("[follw.it] Beginning synchronization of {0} episodes.", episodes.Count);
                    DateTime start = DateTime.Now;

                    // basic data structures used for our processing loop
                    Dictionary<string, DBEpisode> episodeLookup = new Dictionary<string, DBEpisode>();
                    List<FitEpisode> epsToSend = new List<FitEpisode>();
                    List<FitEpisode> totalOutput = new List<FitEpisode>();
                   
                    int sent = 0;
                    int total = episodes.Count;
                    bool canceled = false;

                    // send episodes to server in small groups at a time.
                    foreach (DBEpisode currEpisode in episodes) {
                        if (!currEpisode.IsAvailableLocally) {
                            total--;
                            continue;
                        }

                        // build follwit episode object. clear watched flag if unwatched
                        // to preserve a possible positive watched status on the server
                        FitEpisode fitEpisode = GetFitEpisode(currEpisode);
                        episodeLookup[fitEpisode.SourceId] = currEpisode;
                        if (fitEpisode.Watched == false) fitEpisode.Watched = null;

                        epsToSend.Add(fitEpisode);

                        if (epsToSend.Count > 30) {
                            totalOutput.AddRange(FollwitConnector.FollwitApi.BulkAction(epsToSend));
                            sent += epsToSend.Count;
                            epsToSend.Clear();

                            // send progress update to any listeners
                            try { if (progress != null) canceled = progress(ProgressDialog.Status.RUNNING, (sent * 100) / total); }
                            catch (Exception) { }

                            // if the listener sent a cancel message, we are done
                            if (canceled) break;
                        }
                    }

                    // send remaining group of episodes
                    totalOutput.AddRange(FollwitConnector.FollwitApi.BulkAction(epsToSend));                    
                    sent += epsToSend.Count;

                    // locally store returned data (currently only follwit id)
                    foreach (FitEpisode fitEp in totalOutput) {
                        DBEpisode ep = episodeLookup[fitEp.SourceId];
                        ep[DBOnlineEpisode.cFollwitId] = fitEp.FollwitId;
                        ep[DBOnlineEpisode.cWatched] = fitEp.Watched;
                        if (fitEp.Rating != 0) ep[DBOnlineEpisode.cMyRating] = fitEp.Rating * 2;
                        ep.Commit();
                    }

                    // send final progress update to listeners
                    if (canceled) {
                        try { if (progress != null) progress(ProgressDialog.Status.CANCELED, (sent * 100) / total); }
                        catch (Exception) { }

                        MPTVSeriesLog.Write("[follw.it] Synchronized {0}/{1} episodes. Canceled by user. ({2}).",
                                            sent, total, DateTime.Now - start);
                    }
                    else {
                        try { if (progress != null) progress(ProgressDialog.Status.DONE, (sent * 100) / total); }
                        catch (Exception) { }

                        MPTVSeriesLog.Write("[follw.it] Synchronized {0} episodes. ({1})", sent, DateTime.Now - start);

                    }
                }
                catch (Exception e) {
                    try { if (progress != null) progress(ProgressDialog.Status.CANCELED, 0); }
                    catch (Exception) { }
                    MPTVSeriesLog.Write("[follw.it] Failed episode synchronization: {0}", e.Message);
                }
            }));

            thread.IsBackground = true;
            thread.Name = "follw.it syncer";
            thread.Start();
        }

        public static void InitUpdateThread() {
            if (updateThread != null) {
                updateThread.Abort();
                updateThread = null;
            }

            updateThread = new Thread(new ThreadStart(delegate () {
                while (true) {
                    try {
                        DateTime start = DateTime.Now;
                        MPTVSeriesLog.Write("[follw.it] Beginning update synchronization.", MPTVSeriesLog.LogLevel.Debug);

                        // grab a list of items to update from the server and process them
                        DateTime serverTime;
                        int actionsTaken = 0;
                        List<TaskListItem> tasks = FollwitApi.GetUserTaskList(LastUpdated, out serverTime);
                        foreach (TaskListItem task in tasks) {
                            DBEpisode episode = null;
                            DBSeries series = null;
                            
                            // try to find a cooresponding episode
                            if (task.EpisodeId != 0) {
                                SQLCondition condition = new SQLCondition();
                                condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cFollwitId, task.EpisodeId, SQLConditionType.Equal);
                                List<DBEpisode> episodes = DBEpisode.Get(condition, false);
                                
                                if (episodes.Count > 0) {
                                    episode = episodes[0];
                                    series = DBSeries.Get(episode[DBEpisode.cSeriesID]);
                                }
                            }

                            // try to find a series if we dont have one yet
                            if (task.Task == TaskItemType.NewSeriesRating) {
                                if (task.TvdbSeriesId != 0 && series == null)
                                    series = DBSeries.Get(task.TvdbSeriesId);
                            }

                            // update local data with retrieved info
                            switch (task.Task) {
                                case TaskItemType.NewEpisodeRating:
                                    if (episode == null) continue;

                                    // if we already have a rating and its within one unit of what we are recieving
                                    // ignore it.
                                    if (episode[DBOnlineEpisode.cMyRating] != "" &&
                                        Math.Abs((decimal)(episode[DBOnlineEpisode.cMyRating] - (task.Rating * 2))) <= 1)
                                        continue;

                                    episode[DBOnlineEpisode.cMyRating] = task.Rating * 2;

                                    episode.Commit();
                                    actionsTaken++;
                                    MPTVSeriesLog.Write("[follw.it] Retrieved rating of {3} for '{0} S{1}E{2}'.",
                                        series == null ? "???" : (string) series[DBOnlineSeries.cPrettyName],
                                        episode[DBOnlineEpisode.cCombinedSeason],
                                        episode[DBOnlineEpisode.cEpisodeIndex],
                                        episode[DBOnlineEpisode.cMyRating]);
                                    break;
                                case TaskItemType.NewEpisodeWatchedStatus:
                                    if (episode == null) continue;
                                    episode[DBOnlineEpisode.cWatched] = task.Watched;

                                    episode.Commit();
                                    actionsTaken++;
                                    MPTVSeriesLog.Write("[follw.it] Retrieved watched status of {3} for '{0:00} S{1}E{2}'.",
                                        series == null ? "???" : (string) series[DBOnlineSeries.cPrettyName],
                                        episode[DBOnlineEpisode.cCombinedSeason],
                                        episode[DBOnlineEpisode.cEpisodeIndex],
                                        episode[DBOnlineEpisode.cWatched] == 1 ? "true" : "false");
                                    break;
                                case TaskItemType.NewSeriesRating:
                                    if (series == null) continue;

                                    series[DBOnlineSeries.cMyRating] = task.Rating * 2;

                                    series.Commit();
                                    actionsTaken++;
                                    MPTVSeriesLog.Write("[follw.it] Retrieved rating of {1} for '{0}'.",
                                        series[DBOnlineSeries.cPrettyName],
                                        series[DBOnlineSeries.cMyRating]);
                                    break;
                            }
                        }

                        // log final results
                        MPTVSeriesLog.LogLevel logLevel = actionsTaken == 0 ? MPTVSeriesLog.LogLevel.Debug : MPTVSeriesLog.LogLevel.Normal;
                        MPTVSeriesLog.Write(string.Format("[follw.it] Finished update synchronization. Acted on {0}/{1} events. ({2})",
                                            actionsTaken, tasks.Count, DateTime.Now - start), logLevel);

                        LastUpdated = serverTime;

                    }
                    catch (Exception e) {
                        MPTVSeriesLog.Write("[follw.it] Failed update synchronization: {0}", e.StackTrace);
                    }

                    // sleep in 5 second intervals to allow the thread to be aborted as needed.
                    DateTime lastChecked = DateTime.Now;
                    while (DateTime.Now - lastChecked < UpdateFrequency) {
                        Thread.Sleep(5000);
                    }
                }

            }));

            updateThread.IsBackground = true;
            updateThread.Name = "follw.it persistent update thread";
            updateThread.Start();
        }

        public static void Watch(DBEpisode episode, bool watched, bool showInStream) {
            if (!Enabled || episode == null || !episode.IsAvailableLocally) return;
            DBSeries series = DBSeries.Get(episode[DBEpisode.cSeriesID]);
          
            Thread thread = new Thread(new ThreadStart(delegate {
                try {
                    // start timer and send request
                    DateTime start = DateTime.Now;
                    FollwitApi.WatchedTVEpisode("follwit", episode[DBOnlineEpisode.cFollwitId], watched, showInStream);
                    
                    // log our success
                    MPTVSeriesLog.Write("[follw.it] Set '{0} S{1}E{2}' watched status to {3}. ({4})",
                                        series[DBOnlineSeries.cPrettyName],
                                        episode[DBOnlineEpisode.cCombinedSeason],
                                        episode[DBOnlineEpisode.cEpisodeIndex],
                                        watched.ToString().ToLower(),
                                        DateTime.Now - start);
                }
                catch (Exception e) {
                    // ah crap.
                    MPTVSeriesLog.Write("[follw.it] Failed setting watched status for '{0} S{1}E{2}': {3}",
                                        series[DBOnlineSeries.cPrettyName],
                                        episode[DBOnlineEpisode.cCombinedSeason],
                                        episode[DBOnlineEpisode.cEpisodeIndex],
                                        e.Message);
                }
            }));

            thread.IsBackground = true;
            thread.Name = "follw.it watched updater";
            thread.Start();
        }

        public static void Watch(List<DBEpisode> episodes, bool watched) {
            if (!Enabled || episodes == null || episodes.Count == 0) return;
          
            Thread thread = new Thread(new ThreadStart(delegate {
                try {
                    DateTime start = DateTime.Now;

                    // build list of epsiodes to send 
                    List<FitEpisode> fitEpisodes = new List<FitEpisode>();
                    foreach (DBEpisode currEp in episodes) {
                        if (!currEp.IsAvailableLocally) continue;

                        FitEpisode fitEp = GetFitEpisode(currEp);
                        fitEp.Rating = null;
                        fitEp.Watched = watched;
                        fitEpisodes.Add(fitEp);
                    }

                    // send request and log results
                    FollwitApi.BulkAction(fitEpisodes);
                    MPTVSeriesLog.Write("[follw.it] Set watched status for {0} episodes to {1}. ({2})", 
                        episodes.Count,
                        watched.ToString().ToLower(),
                        DateTime.Now - start);                    
                }
                catch (Exception e) {
                    MPTVSeriesLog.Write("[follw.it] Failed setting watched for {0} episodes: {1}", episodes.Count, e.Message);                    
                }
            }));

            thread.IsBackground = true;
            thread.Name = "follw.it watched bulk updater";
            thread.Start();
        }

        public static void NowWatching(DBEpisode episode, bool watching) {
            if (!Enabled || episode == null || !episode.IsAvailableLocally) return;
            DBSeries series = DBSeries.Get(episode[DBEpisode.cSeriesID]);

            Thread thread = new Thread(new ThreadStart(delegate {
                try {
                    episode = WaitForSyncedEpisode(episode);
                    if (episode == null) return;

                    // start timer and send request
                    DateTime start = DateTime.Now;
                    if (watching) FollwitApi.WatchingTVEpisode("follwit", episode[DBOnlineEpisode.cFollwitId]);
                    else FollwitApi.StopWatchingTVEpisode("follwit", episode[DBOnlineEpisode.cFollwitId]);

                    // log our success
                    MPTVSeriesLog.Write("[follw.it] Sent {0} watching status for '{1} S{2}E{3}'. ({4})",
                                        watching ? "now" : "stopped",
                                        series[DBOnlineSeries.cPrettyName],
                                        episode[DBOnlineEpisode.cCombinedSeason],
                                        episode[DBOnlineEpisode.cEpisodeIndex],
                                        DateTime.Now - start);
                }
                catch (Exception e) {
                    // ah crap.
                    MPTVSeriesLog.Write("[follw.it] Failed sending now watching status for '{0} S{1}E{2}': {3}",
                                        series[DBOnlineSeries.cPrettyName],
                                        episode[DBOnlineEpisode.cCombinedSeason],
                                        episode[DBOnlineEpisode.cEpisodeIndex],
                                        e.Message);
                }
            }));

            thread.IsBackground = true;
            thread.Name = "follw.it now watching updater";
            thread.Start();
        }

        public static void Rate(DBEpisode episode, int rating) {
            if (!Enabled || episode == null || !episode.IsAvailableLocally) return;
            DBSeries series = DBSeries.Get(episode[DBEpisode.cSeriesID]);

            Thread thread = new Thread(new ThreadStart(delegate {
                try {
                    // start timer and send request
                    DateTime start = DateTime.Now;
                    int fivePointRating = (int)Math.Round(rating / 2.0, MidpointRounding.AwayFromZero);
                    FollwitApi.RateTVEpisode("follwit", episode[DBOnlineEpisode.cFollwitId], fivePointRating);

                    // log our success
                    MPTVSeriesLog.Write("[follw.it] Rated '{0} S{1}E{2}' a {3}/5. ({4})",
                                        series[DBOnlineSeries.cPrettyName],
                                        episode[DBOnlineEpisode.cCombinedSeason],
                                        episode[DBOnlineEpisode.cEpisodeIndex],
                                        fivePointRating,
                                        DateTime.Now - start);
                }
                catch (Exception e) {
                    // ah crap.
                    MPTVSeriesLog.Write("[follw.it] Failed rating '{0} S{1}E{2}': {3}",
                                        series[DBOnlineSeries.cPrettyName],
                                        episode[DBOnlineEpisode.cCombinedSeason],
                                        episode[DBOnlineEpisode.cEpisodeIndex],
                                        e.Message);
                }
            }));

            thread.IsBackground = true;
            thread.Name = "follw.it rating updater";
            thread.Start();
        }

        public static void Rate(DBSeries series, int rating) {
            if (!Enabled || series == null) return;

            Thread thread = new Thread(new ThreadStart(delegate {
                try {
                    // start timer and send request
                    DateTime start = DateTime.Now;
                    int fivePointRating = (int)Math.Round(rating / 2.0, MidpointRounding.AwayFromZero);
                    FollwitApi.RateTVSeries("tvdb", series[DBOnlineSeries.cID], fivePointRating);

                    // log our success
                    MPTVSeriesLog.Write("[follw.it] Rated '{0}' a {1}/5. ({2})",
                                        series[DBOnlineSeries.cPrettyName],
                                        fivePointRating,
                                        DateTime.Now - start);
                }
                catch (Exception e) {
                    // ah crap.
                    MPTVSeriesLog.Write("[follw.it] Failed rating '{0}': {1}",
                                        series[DBOnlineSeries.cPrettyName],
                                        e.Message);
                }
            }));

            thread.IsBackground = true;
            thread.Name = "follw.it series rating updater";
            thread.Start();
        }

        protected static DBEpisode WaitForSyncedEpisode(DBEpisode episode) {
            if (episode == null) return null;
            if (episode[DBOnlineEpisode.cFollwitId] != 0) return episode;

            TimeSpan retryDelay = new TimeSpan(0, 0, 30);
            int limit = 8;
            int epId = episode[DBOnlineEpisode.cID];

            int tries = 0;
            while (tries < limit) {
                // reload the episode from the db to check for changes
                SQLCondition condition = new SQLCondition();
                condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cID, epId, SQLConditionType.Equal);
                condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cFollwitId, 0, SQLConditionType.NotEqual);
                List<DBEpisode> episodes = DBEpisode.Get(condition, false);
                
                if (episodes.Count > 0) return episodes[0];
                
                Thread.Sleep(retryDelay);
                tries++;
            }

            return null;
        }

        public static FitEpisode GetFitEpisode(DBEpisode mptvEpisode) {
            if (mptvEpisode == null)
                return null;
            
            FitEpisode fitEp = new FitEpisode();
            fitEp.SourceName = "tvdb";
            fitEp.SourceId = mptvEpisode.onlineEpisode[DBOnlineEpisode.cID];
            fitEp.InCollection = true;

            if (!String.IsNullOrEmpty(mptvEpisode[DBOnlineEpisode.cMyRating])) {
                int value;
                int.TryParse(mptvEpisode[DBOnlineEpisode.cMyRating], out value);
                fitEp.Rating = (int)Math.Round(value / 2.0, MidpointRounding.AwayFromZero);
            }

            fitEp.Watched = mptvEpisode[DBOnlineEpisode.cWatched] == 1;

            return fitEp;
        }

        private static void _follwitAPI_RequestEvent(string RequestText) {
            MPTVSeriesLog.Write(RequestText, MPTVSeriesLog.LogLevel.DebugSQL);
        }

        private static void _follwitAPI_ResponseEvent(string ResponseText) {
            MPTVSeriesLog.Write(ResponseText, MPTVSeriesLog.LogLevel.DebugSQL);
        }

    }
}
