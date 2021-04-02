using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TraktAPI.Extensions;
using TraktPlugin.TmdbAPI.DataStructures;

namespace WindowPlugins.GUITVSeries.Online_Parsing_Classes
{
    static class OnlineAPI
    {
        private static readonly Dictionary<int, WebClient> mWebClients = new Dictionary<int, WebClient>();
        private static int mDownloadGUIDGenerator = 1;

        #region Language
        static string mSelectedLanguage = string.Empty;

        public static string SelectedLanguage
        {
            get
            {
                if ( mSelectedLanguage.Length == 0 )
                {
                    string lLanguage = DBOption.GetOptions( DBOption.cOnlineLanguage );
                    if ( !String.IsNullOrEmpty( lLanguage ) )
                        mSelectedLanguage = lLanguage;
                    else 
                        mSelectedLanguage = "en"; // use english
                }
                return mSelectedLanguage;
            }
            set
            { 
                mSelectedLanguage = value;
            }
        }

        public static string GetLanguageOverride( string aSeriesId )
        {
            string lSqlCondition = string.Empty;
            var lCondition = new SQLCondition();

            lSqlCondition = "id = " + aSeriesId;
            lCondition.AddCustom( lSqlCondition );

            // Get the language that the user has selected for the serie
            List<DBValue> seriesLanguage = DBOnlineSeries.GetSingleField( DBOnlineSeries.cLanguage, lCondition, new DBOnlineSeries() );

            if ( ( seriesLanguage.Count > 0 ) && ( seriesLanguage[0] != "" ) )
            {
                return seriesLanguage[0];
            }
            else
            {
                //If there is no language prefered for the series, the fallback is SelLanguageAsString (en).
                return SelectedLanguage;
            }
        }

        public static string GetSeriesLanguage( int aSeriesID )
        {
            if ( DBOption.GetOptions( DBOption.cOverrideLanguage ) )
            {
                return GetLanguageOverride( aSeriesID.ToString() );
            }
            else
            {
                return SelectedLanguage;
            }
        }

        #endregion

        #region TMDb Helpers

        public static string GetTMDbBasePath()
        {
            TmdbConfiguration lTmdbConfig = DBOption.GetOptions(DBOption.cTmdbConfiguration).ToString().FromJSON<TmdbConfiguration>();
            
            return lTmdbConfig?.Images?.SecureBaseUrl ?? "https://image.tmdb.org/t/p/";
        }

        #endregion

        public static bool CheckFileDownload(int aDownloadGuid)
        {
            return mWebClients.ContainsKey(aDownloadGuid);
        }

        public static bool CancelFileDownload(int aDownloadGuid)
        {
            if (mWebClients.ContainsKey(aDownloadGuid))
            {
                WebClient lClient = mWebClients[aDownloadGuid];
                lClient.CancelAsync();
                return true;
            }
            return false;
        }

        public static string DownloadBanner(string aOnlineFilename, Settings.Path aLocalPath, string aLocalFilename)
        {
            var lWebClient = new WebClient();

            // Widebanners come from fanart.tv, adjust online path to suit
            string lBasePath = aLocalFilename.Contains("graphical/") ? "https://assets.fanart.tv/" : GetTMDbBasePath();
            string lFullLocalPath = Helper.PathCombine(Settings.GetPath(aLocalPath), aLocalFilename);
            string lFullURL = lBasePath + aOnlineFilename;

            lWebClient.Headers.Add("user-agent", Settings.UserAgent);

            // .NET 4.0: Use TLS v1.2. Many download sources no longer support the older and now insecure TLS v1.0/1.1 and SSL v3.
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)0xc00;

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(lFullLocalPath));
                if ( !File.Exists( lFullLocalPath ) // only if the file doesn't exist
                    || ImageAllocator.LoadImageFastFromFile(lFullLocalPath ) == null) // or the file is damaged
                {
                    MPTVSeriesLog.Write("Downloading new Image from: " + lFullURL, MPTVSeriesLog.LogLevel.Debug);
                    lWebClient.DownloadFile(lFullURL, lFullLocalPath);
                    return lFullLocalPath;
                }
                return string.Empty;
            }
            catch (WebException ex)
            {
                MPTVSeriesLog.Write($"Banner download failed from '{lFullURL}' to '{lFullLocalPath.Replace("/", @"\")}'. Reason='{ex.Message}'");
                return null;
            }
        }

        public static int StartFileDownload( string aFullURL, Settings.Path aLocalPath, string aLocalFilename )
        {
            var lWebClient = new WebClient();

            // .NET 4.0: Use TLS v1.2. Many download sources no longer support the older and now insecure TLS v1.0/1.1 and SSL v3.
            ServicePointManager.SecurityProtocol = ( SecurityProtocolType )0xc00;

            int lDownloadGuid = mDownloadGUIDGenerator;
            mDownloadGUIDGenerator++;

            string lFullLocalPath = Helper.PathCombine( Settings.GetPath( aLocalPath ), aLocalFilename );
            lWebClient.Headers.Add( "user-agent", Settings.UserAgent );

            try
            {
                Directory.CreateDirectory( Path.GetDirectoryName( lFullLocalPath ) );
                if ( 
                       !File.Exists( lFullLocalPath ) // only if the file doesn't exist
                    || ImageAllocator.LoadImageFastFromFile( lFullLocalPath ) == null ) // or the file is damaged
                {
                    lWebClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler( WebClient_DownloadFileCompleted );
                    lWebClient.DownloadFileAsync( new Uri( aFullURL ), lFullLocalPath, lDownloadGuid );
                    mWebClients.Add( lDownloadGuid, lWebClient );
                    return lDownloadGuid;
                }
                return -1;
            }
            catch ( WebException )
            {
                MPTVSeriesLog.Write( "File download failed (" + aFullURL + ") to " + lFullLocalPath.Replace( "/", @"\" ) );
                return -1;
            }
        }

        private static void WebClient_DownloadFileCompleted( object sender, System.ComponentModel.AsyncCompletedEventArgs e )
        {
            mWebClients.Remove( ( int )e.UserState );
        }

    }
}
