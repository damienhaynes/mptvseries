using System.IO;
using System.Net;
using WindowPlugins.GUITVSeries.FanartTvAPI.DataStructures;
using WindowPlugins.GUITVSeries.FanartTvAPI.Extensions;

namespace WindowPlugins.GUITVSeries.FanartTvAPI
{
    public static class FanartTvAPI
    {
        private const string ApiUrl = "http://webservice.fanart.tv/v3/tv/{0}";

        #region Web Events

        // these events can be used to log data sent / received from fanart.tv
        public delegate void OnDataSendDelegate(string url, string postData);
        public delegate void OnDataReceivedDelegate(string response, HttpWebResponse webResponse);
        public delegate void OnDataErrorDelegate(string error);

        public static event OnDataSendDelegate OnDataSend;
        public static event OnDataReceivedDelegate OnDataReceived;
        public static event OnDataErrorDelegate OnDataError;

        #endregion

        #region Settings

        // these settings should be set before sending data to fanart.tv        
        static string UserAgent { get; set; }

        /// <summary>
        /// Also known as a personal API key, the client key provides developers a way to help support
        /// us by getting their end users to sign up for their own personal API key, this should be used in addition
        /// to the project key, for more details read https://fanart.tv/2015/01/personal-api-keys/
        /// </summary>
        static string ClientKey { get; set; }

        static string ApiKey { get; set; }

        public static void Init(string aUserAgent, string aApiKey, string aClientKey = null)
        {
            ApiKey = aApiKey;
            ClientKey = aClientKey;
            UserAgent = aUserAgent;
        }

        #endregion

        #region Images
        public static FanartTvImages GetShowImages(string aId)
        {
            FanartTvImages lImages = FanartTvCache.LoadSeriesFromCache(aId);

            if (lImages == null)
            {
                string lResponse = GetFromFanartTv(string.Format(ApiUrl, aId));
                
                lImages = lResponse.FromJSON<FanartTvImages>();
                FanartTvCache.SaveSeriesToCache(lImages, aId);
            }

            return lImages;
        }
        #endregion

        static string GetFromFanartTv(string aAddress)
        {
            OnDataSend?.Invoke(aAddress, null);

            var lHeaderCollection = new WebHeaderCollection();

            var lRequest = WebRequest.Create(aAddress) as HttpWebRequest;

            lRequest.KeepAlive = true;
            lRequest.Method = "GET";
            lRequest.ContentLength = 0;
            lRequest.Timeout = 120000;
            lRequest.ContentType = "application/json";
            lRequest.UserAgent = UserAgent;

            // add required headers for authorisation
            lRequest.Headers.Add("api-key", ApiKey);

            if (!string.IsNullOrEmpty(ClientKey))
            {
                lRequest.Headers.Add("client_key", ClientKey);
            }

            string lResponseStr = null;

            try
            {
                var lResponse = (HttpWebResponse)lRequest.GetResponse();
                if (lResponse == null) return null;

                Stream lStream = lResponse.GetResponseStream();

                var lReader = new StreamReader(lStream);
                lResponseStr = lReader.ReadToEnd();

                lHeaderCollection = lResponse.Headers;

                OnDataReceived?.Invoke(lResponseStr, lResponse);

                lStream.Close();
                lReader.Close();
                lResponse.Close();
            }
            catch (WebException wex)
            {
                string lErrorMessage = wex.Message;
                if (wex.Status == WebExceptionStatus.ProtocolError)
                {
                    var lResponse = wex.Response as HttpWebResponse;

                    string lHeaders = string.Empty;
                    foreach (string key in lResponse.Headers.AllKeys)
                    {
                        lHeaders += string.Format("{0}: {1}, ", key, lResponse.Headers[key]);
                    }
                    lErrorMessage = string.Format("Protocol Error, Code = '{0}', Description = '{1}', Url = '{2}', Headers = '{3}'", (int)lResponse.StatusCode, lResponse.StatusDescription, aAddress, lHeaders.TrimEnd(new char[] { ',', ' ' }));
                }

                OnDataError?.Invoke(lErrorMessage);

                lResponseStr = null;
            }
            catch (IOException ioe)
            {
                string lErrorMessage = string.Format("Request failed due to an IO error, Description = '{0}', Url = '{1}', Method = 'GET'", ioe.Message, aAddress);

                OnDataError?.Invoke(ioe.Message);

                lResponseStr = null;
            }

            return lResponseStr;
        }
    }
}
