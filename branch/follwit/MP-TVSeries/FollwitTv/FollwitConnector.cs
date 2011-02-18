using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Follwit.API;

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

        private static void _follwitAPI_RequestEvent(string RequestText) {
            throw new NotImplementedException();
        }

        private static void _follwitAPI_ResponseEvent(string ResponseText) {
            throw new NotImplementedException();
        }

    }
}
