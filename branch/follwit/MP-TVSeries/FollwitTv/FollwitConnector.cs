using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Follwit.API;
using Follwit.API.Data;
using CookComputing.XmlRpc;

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
                        lock (follwitApiLock) _follwitAPI = FollwitApi.Login(Username, HashedPassword, ApiUrl);
                        
                        if (_follwitAPI == null) {
                            MPTVSeriesLog.Write("Failed to log in to follw.it: Invalid Username or Password!", MPTVSeriesLog.LogLevel.Normal);
                        }

                        if (_follwitAPI != null) {
                            _follwitAPI.RequestEvent += new Follwit.API.FollwitApi.FitAPIRequestDelegate(_follwitAPI_RequestEvent);
                            _follwitAPI.ResponseEvent += new Follwit.API.FollwitApi.FitAPIResponseDelegate(_follwitAPI_ResponseEvent);

                            MPTVSeriesLog.Write("Logged in to follw.it as {0}.", _follwitAPI.User.Name);
                        }
                    }
                    catch (Exception ex) {
                        MPTVSeriesLog.Write("Failed to log in to follw.it: " + ex.Message);
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
            MPTVSeriesLog.Write("Begining follw.it full synchronization.");
            DateTime start = DateTime.Now;

            // grab list of all local episodes
            SQLCondition condition = new SQLCondition();
            condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, 0, SQLConditionType.GreaterThan);
            List<DBEpisode> episodes = DBEpisode.Get(condition, false);

            // basic data structures used for our processing loop
            Dictionary<string, DBEpisode> episodeLookup = new Dictionary<string, DBEpisode>();
            List<FitEpisode> epsToSend = new List<FitEpisode>();
            List<XmlRpcStruct> totalOutput = new List<XmlRpcStruct>();

            // send episodes to server in small groups at a time.
            foreach (DBEpisode currEpisode in episodes) {
                FitEpisode fitEpisode = GetFitEpisode(currEpisode);
                episodeLookup[fitEpisode.SourceId] = currEpisode;
                epsToSend.Add(fitEpisode);

                if (epsToSend.Count > 30) {
                    object[] output = (object[]) FollwitConnector.FollwitApi.BulkAction(epsToSend);
                    foreach (object currRecord in output) totalOutput.Add((XmlRpcStruct)currRecord);
                    epsToSend.Clear();
                }
            }

            // send remaining group of episodes
            object[] output2 = (object[])FollwitConnector.FollwitApi.BulkAction(epsToSend);
            foreach (object currRecord in output2) totalOutput.Add((XmlRpcStruct)currRecord);

            // locally store returned data (currently only follwit id)
            foreach (XmlRpcStruct currRecord in totalOutput) {
                DBEpisode ep = episodeLookup[(string)currRecord["SourceId"]];
                ep[DBOnlineEpisode.cFollwitId] = (string)currRecord["EpisodeId"];
                ep.Commit();
            }

            MPTVSeriesLog.Write("Finished follw.it full synchronization. (" + (DateTime.Now - start).ToString() + ")");
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
            MPTVSeriesLog.Write(RequestText, MPTVSeriesLog.LogLevel.Debug);
        }

        private static void _follwitAPI_ResponseEvent(string ResponseText) {
            MPTVSeriesLog.Write(ResponseText, MPTVSeriesLog.LogLevel.Debug);
        }

    }
}
