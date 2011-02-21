using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Follwit.API;
using Follwit.API.Data;
using CookComputing.XmlRpc;
using System.Threading;
using WindowPlugins.GUITVSeries.Configuration;

namespace WindowPlugins.GUITVSeries.FollwitTv {
    public class FollwitConnector {
        internal static object follwitApiLock = new Object();

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
            internal set { DBOption.SetOptions(DBOption.cFollwitBaseUrl, value); }
        }

        public static string Username {
            get { return DBOption.GetOptions(DBOption.cFollwitUsername); }
            internal set { DBOption.SetOptions(DBOption.cFollwitUsername, value); }
        }

        internal static string Password {
            set { DBOption.SetOptions(DBOption.cFollwitHashedPassword, value); }
        }

        public static string HashedPassword {
            get { return DBOption.GetOptions(DBOption.cFollwitHashedPassword); }
            internal set { DBOption.SetOptions(DBOption.cFollwitHashedPassword, value); }
        }

        public static void FullSync() {
            FullSync(null);
        }

        public static void FullSync(ProgressDialog.ProgressDelegate progress) {
            if (!Enabled) return;

            Thread thread = new Thread(new ThreadStart(delegate {
                try {
                    MPTVSeriesLog.Write("[follw.it] Beginning full synchronization.");
                    DateTime start = DateTime.Now;

                    // grab list of all local episodes
                    SQLCondition condition = new SQLCondition();
                    condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, 0, SQLConditionType.GreaterThan);
                    List<DBEpisode> episodes = DBEpisode.Get(condition, false);

                    // basic data structures used for our processing loop
                    Dictionary<string, DBEpisode> episodeLookup = new Dictionary<string, DBEpisode>();
                    List<FitEpisode> epsToSend = new List<FitEpisode>();
                    List<XmlRpcStruct> totalOutput = new List<XmlRpcStruct>();
                   
                    int sent = 0;
                    int total = episodes.Count;
                    bool canceled = false;

                    // send episodes to server in small groups at a time.
                    foreach (DBEpisode currEpisode in episodes) {
                        // build follwit episode object. clear watched flag if unwatched
                        // to preserve a possible positive watched status on the server
                        FitEpisode fitEpisode = GetFitEpisode(currEpisode);
                        episodeLookup[fitEpisode.SourceId] = currEpisode;
                        if (fitEpisode.Watched == false) fitEpisode.Watched = null;

                        epsToSend.Add(fitEpisode);

                        if (epsToSend.Count > 30) {
                            object[] output = (object[])FollwitConnector.FollwitApi.BulkAction(epsToSend);
                            foreach (object currRecord in output) totalOutput.Add((XmlRpcStruct)currRecord);
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
                    object[] output2 = (object[])FollwitConnector.FollwitApi.BulkAction(epsToSend);
                    foreach (object currRecord in output2) totalOutput.Add((XmlRpcStruct)currRecord);
                    sent += epsToSend.Count;

                    // locally store returned data (currently only follwit id)
                    foreach (XmlRpcStruct currRecord in totalOutput) {
                        DBEpisode ep = episodeLookup[(string)currRecord["SourceId"]];
                        ep[DBOnlineEpisode.cFollwitId] = (string)currRecord["EpisodeId"];
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

                        MPTVSeriesLog.Write("[follw.it] Finished full synchronization. (" + (DateTime.Now - start).ToString() + ")");

                    }
                }
                catch (Exception e) {
                    // ah crap.
                    MPTVSeriesLog.Write("[follw.it] Failed full synchronization: {0}", e.Message);
                }
            }));

            thread.IsBackground = true;
            thread.Name = "follw.it syncer";
            thread.Start();
        }

        public static void Watch(DBEpisode episode, bool watched, bool showInStream) {
            if (!Enabled || episode == null) return;
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
                                        episode[DBOnlineEpisode.cCombinedEpisodeNumber],
                                        watched.ToString().ToLower(),
                                        DateTime.Now - start);
                }
                catch (Exception e) {
                    // ah crap.
                    MPTVSeriesLog.Write("[follw.it] Failed setting watched status for '{0} S{1}E{2}': {3}",
                                        series[DBOnlineSeries.cPrettyName],
                                        episode[DBOnlineEpisode.cCombinedSeason],
                                        episode[DBOnlineEpisode.cCombinedEpisodeNumber],
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
            if (!Enabled || episode == null) return;
            DBSeries series = DBSeries.Get(episode[DBEpisode.cSeriesID]);

            Thread thread = new Thread(new ThreadStart(delegate {
                try {
                    // start timer and send request
                    DateTime start = DateTime.Now;
                    if (watching) FollwitApi.WatchingTVEpisode("follwit", episode[DBOnlineEpisode.cFollwitId]);
                    else FollwitApi.StopWatchingTVEpisode("follwit", episode[DBOnlineEpisode.cFollwitId]);

                    // log our success
                    MPTVSeriesLog.Write("[follw.it] Sent {0} watching status for '{1} S{2}E{3}'. ({4})",
                                        watching ? "now" : "stopped",
                                        series[DBOnlineSeries.cPrettyName],
                                        episode[DBOnlineEpisode.cCombinedSeason],
                                        episode[DBOnlineEpisode.cCombinedEpisodeNumber],
                                        DateTime.Now - start);
                }
                catch (Exception e) {
                    // ah crap.
                    MPTVSeriesLog.Write("[follw.it] Failed sending now watching status for '{0} S{1}E{2}': {3}",
                                        series[DBOnlineSeries.cPrettyName],
                                        episode[DBOnlineEpisode.cCombinedSeason],
                                        episode[DBOnlineEpisode.cCombinedEpisodeNumber],
                                        e.Message);
                }
            }));

            thread.IsBackground = true;
            thread.Name = "follw.it now watching updater";
            thread.Start();
        }

        public static void Rate(DBEpisode episode, int rating) {
            if (!Enabled || episode == null) return;
            DBSeries series = DBSeries.Get(episode[DBEpisode.cSeriesID]);

            Thread thread = new Thread(new ThreadStart(delegate {
                try {
                    // start timer and send request
                    DateTime start = DateTime.Now;
                    int fivePointRating = (int)Math.Round(rating / 2.0);
                    FollwitApi.RateTVEpisode("follwit", episode[DBOnlineEpisode.cFollwitId], fivePointRating);

                    // log our success
                    MPTVSeriesLog.Write("[follw.it] Rated '{0} S{1}E{2}' a {3}/5. ({4})",
                                        series[DBOnlineSeries.cPrettyName],
                                        episode[DBOnlineEpisode.cCombinedSeason],
                                        episode[DBOnlineEpisode.cCombinedEpisodeNumber],
                                        fivePointRating,
                                        DateTime.Now - start);
                }
                catch (Exception e) {
                    // ah crap.
                    MPTVSeriesLog.Write("[follw.it] Failed rating '{0} S{1}E{2}': {3}",
                                        series[DBOnlineSeries.cPrettyName],
                                        episode[DBOnlineEpisode.cCombinedSeason],
                                        episode[DBOnlineEpisode.cCombinedEpisodeNumber],
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
                    int fivePointRating = (int)Math.Round(rating / 2.0);
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
                fitEp.Rating = (int)Math.Round(value / 2.0);
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
