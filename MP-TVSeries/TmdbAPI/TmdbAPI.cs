using System.IO;
using System.Net;
using System.Threading;
using WindowPlugins.GUITVSeries.TmdbAPI.DataStructures;
using WindowPlugins.GUITVSeries.TmdbAPI.Extensions;
using System.Collections.Generic;

namespace WindowPlugins.GUITVSeries.TmdbAPI
{
    #region Enums
    public enum ExternalSource
    {
        imdb_id,
        tvrage_id,
        tvdb_id
    }
    #endregion

    public static class TmdbAPI
    {
        #region Web Events

        // these events can be used to log data sent / received from tmdb
        public delegate void OnDataSendDelegate(string url, string postData);
        public delegate void OnDataReceivedDelegate(string response, HttpWebResponse webResponse);
        public delegate void OnDataErrorDelegate(string error);

        public static event OnDataSendDelegate OnDataSend;
        public static event OnDataReceivedDelegate OnDataReceived;
        public static event OnDataErrorDelegate OnDataError;

        #endregion

        #region Settings

        // these settings should be set before sending data to tmdb        
        public static string UserAgent { get; set; }

        #endregion

        #region Configuration
        public static TmdbConfiguration GetConfiguration()
        {
            string lResponse = GetFromTmdb(TmdbURIs.ApiConfig);
            return lResponse.FromJSON<TmdbConfiguration>();
        }

        public static IEnumerable<TmdbLanguage> GetLanguages()
        {
            string lResponse = GetFromTmdb(TmdbURIs.ApiGetLanguages );
            return lResponse.FromJSONArray<TmdbLanguage>();
        }

        #endregion

        #region Images

        public static TmdbShowImages GetShowImages( string aId, string aIncludeLanguages = "en,null" )
        {
            string lResponse = GetFromTmdb(string.Format(TmdbURIs.ApiGetShowImages, aId, aIncludeLanguages));
            return lResponse.FromJSON<TmdbShowImages>();
        }

        public static TmdbSeasonImages GetSeasonImages( string aId, int aSeason, string aIncludeLanguages = "en,null" )
        {
            string lResponse = GetFromTmdb(string.Format(TmdbURIs.ApiGetSeasonImages, aId, aSeason, aIncludeLanguages));
            return lResponse.FromJSON<TmdbSeasonImages>();
        }

        public static TmdbEpisodeImages GetEpisodeImages( string aId, int aSeason, int aEpisode, string aIncludeLanguages = "en,null" )
        {
            string lResponse = GetFromTmdb(string.Format(TmdbURIs.ApiGetEpisodeImages, aId, aSeason, aEpisode, aIncludeLanguages));
            return lResponse.FromJSON<TmdbEpisodeImages>();
        }
        #endregion

        #region Search
        public static TmdbFindResult TmdbFind( string aId, ExternalSource aSourceId )
        {
            string lResponse = GetFromTmdb( string.Format( TmdbURIs.ApiFind, aId, aSourceId.ToString() ) );
            return lResponse.FromJSON<TmdbFindResult>();
        }

        public static TmdbSearchResult Search( string aName, string aLanguage = "en", int aPage = 1, bool aIncludeAdult = false )
        {
            string lEncodedSeriesName;
            try
            {
                lEncodedSeriesName = System.Uri.EscapeDataString(aName);
            }
            catch (System.UriFormatException)
            {
                lEncodedSeriesName = System.Web.HttpUtility.UrlEncode(aName);
            }

            string lResponse = GetFromTmdb(string.Format(TmdbURIs.ApiSearchTvShow, aLanguage, aPage, lEncodedSeriesName, aIncludeAdult));
            return lResponse.FromJSON<TmdbSearchResult>();
        }

        #endregion

        #region Details

        public static TmdbShowDetail GetShowDetail(int aSeriesId, string aLanguage = "en")
        {
            // load from cache if it exists
            TmdbShowDetail lShowDetail = TmdbCache.LoadSeriesFromCache(aSeriesId, aLanguage);
            if (lShowDetail == null)
            {
                string lResponse = GetFromTmdb(string.Format(TmdbURIs.ApiTvShowDetail, aSeriesId, aLanguage));
                lShowDetail = lResponse.FromJSON<TmdbShowDetail>();
            }

            // save to cache
            TmdbCache.SaveSeriesToCache(lShowDetail, aLanguage);

            return lShowDetail;
        }

        public static TmdbSeasonDetail GetSeasonDetail(int aSeriesId, int aSeason, string aLanguage = "en")
        {
            // load from cache if it exists
            TmdbSeasonDetail lSeasonDetail = TmdbCache.LoadSeasonFromCache(aSeriesId, aSeason, aLanguage);
            if (lSeasonDetail == null)
            {
                string lResponse = GetFromTmdb(string.Format(TmdbURIs.ApiTvSeasonDetail, aSeriesId, aSeason, aLanguage));
                lSeasonDetail = lResponse.FromJSON<TmdbSeasonDetail>();
            }

            // save to cache
            TmdbCache.SaveSeasonToCache(lSeasonDetail, aSeriesId, aSeason, aLanguage);

            return lSeasonDetail;
        }

        #endregion

        #region Changes

        public static TmdbTvChanges GetTvChanges(string aStartDate, string aPage = "1")
        {   
            string lResponse = GetFromTmdb(string.Format(TmdbURIs.ApiChanges, aStartDate, aPage));
            return lResponse.FromJSON<TmdbTvChanges>();
        }


        #endregion

        static string GetFromTmdb(string address, int delayRequest = 0)
        {
            if (delayRequest > 0)
                Thread.Sleep(1000 + delayRequest);

            OnDataSend?.Invoke(address, null);

            var headerCollection = new WebHeaderCollection();

            var request = WebRequest.Create(address) as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "GET";
            request.ContentLength = 0;
            request.Timeout = 120000;
            request.ContentType = "application/json";
            request.UserAgent = UserAgent;

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

                    // check if we got a 429 error code
                    // https://developers.themoviedb.org/3/getting-started/request-rate-limiting

                    if ((int)response.StatusCode == 429)
                    {
                        int retry = 0;
                        int.TryParse(response.Headers["Retry-After"], out retry);

                        errorMessage = string.Format("Request Rate Limiting is in effect, retrying request in {0} seconds. Url = '{1}'", retry, address);

                        OnDataError?.Invoke(errorMessage);

                        return GetFromTmdb(address, retry * 1000);
                    }
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
