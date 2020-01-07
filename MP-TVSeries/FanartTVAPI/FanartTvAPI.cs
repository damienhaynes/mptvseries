using System.IO;
using System.Net;
using WindowPlugins.GUITVSeries.FanartTvAPI.DataStructures;
using WindowPlugins.GUITVSeries.FanartTvAPI.Extensions;

namespace WindowPlugins.GUITVSeries.TmdbAPI
{
    public static class FanartTvAPI
    {
        private const string ApiUrl = "http://webservice.fanart.tv/v3/movies/{0}";

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
            string lResponse = GetFromFanartTv(string.Format(ApiUrl, aId));
            return lResponse.FromJSON<FanartTvImages>();
        }
        #endregion

        static string GetFromFanartTv(string address)
        {
            OnDataSend?.Invoke(address, null);

            var headerCollection = new WebHeaderCollection();

            var request = WebRequest.Create(address) as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "GET";
            request.ContentLength = 0;
            request.Timeout = 120000;
            request.ContentType = "application/json";
            request.UserAgent = UserAgent;

            // add required headers for authorisation
            request.Headers.Add("api-key", ApiKey);

            if (!string.IsNullOrEmpty(ClientKey))
            {
                request.Headers.Add("client_key", ClientKey);
            }

            string strResponse = null;

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response == null) return null;

                Stream stream = response.GetResponseStream();

                StreamReader reader = new StreamReader(stream);
                strResponse = reader.ReadToEnd();

                headerCollection = response.Headers;

                OnDataReceived?.Invoke(strResponse, response);

                stream.Close();
                reader.Close();
                response.Close();
            }
            catch (WebException wex)
            {
                string errorMessage = wex.Message;
                if (wex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = wex.Response as HttpWebResponse;

                    string headers = string.Empty;
                    foreach (string key in response.Headers.AllKeys)
                    {
                        headers += string.Format("{0}: {1}, ", key, response.Headers[key]);
                    }
                    errorMessage = string.Format("Protocol Error, Code = '{0}', Description = '{1}', Url = '{2}', Headers = '{3}'", (int)response.StatusCode, response.StatusDescription, address, headers.TrimEnd(new char[] { ',', ' ' }));
                }

                OnDataError?.Invoke(errorMessage);

                strResponse = null;
            }
            catch (IOException ioe)
            {
                string errorMessage = string.Format("Request failed due to an IO error, Description = '{0}', Url = '{1}', Method = 'GET'", ioe.Message, address);

                OnDataError?.Invoke(ioe.Message);

                strResponse = null;
            }

            return strResponse;
        }
    }
}
