using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;

namespace WindowPlugins.GUITVSeries.Online_Parsing_Classes
{
    static class OnlineAPI
    {
        private static class apiURIs
        {
            public const string Mirrors = "mirrors";
            public const string Languages = "languages";
            public const string GetSeries = @"GetSeries.php?seriesname={0}&language={1}";
            public const string FullSeriesUpdate = @"series/{0}/all/{1}";
            public const string Updates = "updates/updates_{0}";
            public const string SubmitRating = "User_Rating.php?accountid={0}&itemtype={1}&itemid={2}&rating={3}";
            public const string GetRatingsForUser = @"GetRatingsForUser.php?apikey={0}&accountid={1}&seriesid={2}";
            public const string ConfigureFavourites = @"User_Favorites.php?accountid={0}&type={1}&seriesid={2}";
            public const string GetAllFavourites = @"User_Favorites.php?accountid={0}";
        }

        public enum Format
        {
            Xml,
            Zip,
            NoExtension
        }

        public enum UpdateType
        {
            day = 0,
            week = 1,
            month = 2,
            all = 3,
        }

        public enum RatingType
        {
            series = 0,
            episode = 1,
        }

        static Dictionary<int, WebClient> webClientList = new Dictionary<int, WebClient>();
        static int nDownloadGUIDGenerator = 1;

        #region Language
        static string selLang = string.Empty;

        public static string SelLanguageAsString
        {
            get
            {
                if ( selLang.Length == 0 )
                {
                    string lang = DBOption.GetOptions( DBOption.cOnlineLanguage );
                    if ( !String.IsNullOrEmpty( lang ) ) selLang = lang;
                    else selLang = "en"; // use english
                }
                return selLang;
            }
            set { selLang = value; }
        }

        public static string GetLanguageOverride( String sSeriesID )
        {
            string sqlCon = string.Empty;
            SQLCondition cond = new SQLCondition();

            sqlCon = "id = " + sSeriesID;
            cond.AddCustom( sqlCon );

            // Get the language that the user has selected for the serie
            List<DBValue> seriesLanguage = DBOnlineSeries.GetSingleField( DBOnlineSeries.cLanguage, cond, new DBOnlineSeries() );

            if ( ( seriesLanguage.Count > 0 ) && ( seriesLanguage[0] != "" ) )
            {
                return seriesLanguage[0];
            }
            else
            {
                //If there is no language prefered for the series, the fallback is SelLanguageAsString (en).
                return SelLanguageAsString;
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
                return SelLanguageAsString;
            }
        }

        #endregion

        static public XmlNode GetMirrors( String sServer )
        {
            return Generic( sServer + apiURIs.Mirrors, false, Format.Xml );
        }

        static public XmlNode GetLanguages()
        {
            return Generic( apiURIs.Languages, Format.Xml );
        }

        static public XmlNode GetSeries( String sSeriesName, string aLanguageCode = "en" )
        {
            string lEncodedSeriesName;
            try
            {
                lEncodedSeriesName = Uri.EscapeDataString(sSeriesName);
            }
            catch (UriFormatException ex)
            {
                MPTVSeriesLog.Write($"Failed to get encoded series name of '{sSeriesName}'. Error={ex.Message}");
                lEncodedSeriesName = HttpUtility.UrlEncode(sSeriesName);
            }
            
            return Generic( string.Format( apiURIs.GetSeries, lEncodedSeriesName, aLanguageCode ), true, false, Format.NoExtension );
        }

        static public XmlNode GetUserRatings( String sSeriesID, String sAccountID )
        {
            string url = String.Format( apiURIs.GetRatingsForUser, DBOnlineMirror.cApiKey, sAccountID, sSeriesID );
            return Generic( url, true, false, Format.NoExtension );
        }

        static public XmlNode GetUserFavourites( String sAccountID )
        {
            string url = String.Format( apiURIs.GetAllFavourites, sAccountID );
            return Generic( url, true, false, Format.NoExtension );
        }

        static public XmlNode ConfigureFavourites( bool bAdd, String sAccountID, String sSeriesID )
        {
            if ( sAccountID.Length == 0 )
            {
                MPTVSeriesLog.Write( "Cannot submit online favourite, make sure you have your Account identifier is set!" );
                return null;
            }

            if ( bAdd )
            {
                MPTVSeriesLog.Write( string.Format( "Adding favourite series \"{0}\" to online database (theTVDB.com)", Helper.getCorrespondingSeries( int.Parse( sSeriesID ) ) ) );
            }
            else
            {
                MPTVSeriesLog.Write( string.Format( "Removing favourite series \"{0}\" from online database (theTVDB.com)", Helper.getCorrespondingSeries( int.Parse( sSeriesID ) ) ) );
            }

            string url = String.Format( apiURIs.ConfigureFavourites, sAccountID, ( bAdd ? "add" : "remove" ), sSeriesID );
            return Generic( url, true, false, Format.NoExtension );
        }

        static public XmlNode UpdateSeries( String sSeriesID )
        { return UpdateSeries( sSeriesID, true ); }

        static public XmlNode UpdateSeries( String sSeriesID, String languageID, bool aOverride = false )
        { return UpdateSeries( sSeriesID, languageID, true, aOverride ); }

        static private XmlNode UpdateSeries( String sSeriesID, bool first )
        {
            int series = Int32.Parse( sSeriesID );

            if ( DBOption.GetOptions( DBOption.cOverrideLanguage ) )
            {
                string SelLang = GetLanguageOverride( sSeriesID );
                return getFromCache( series, SelLang + ".xml", SelLang );
            }
            else
            {
                return getFromCache( series, SelLanguageAsString + ".xml" );
            }
        }

        static private XmlNode UpdateSeries( String sSeriesID, String languageID, bool first, bool aOverride = false )
        {
            // we may need to get english original name even when series language is overridden
            if ( DBOption.GetOptions( DBOption.cOverrideLanguage ) && !aOverride )
            {
                languageID = GetLanguageOverride( sSeriesID );
                int series = Int32.Parse( sSeriesID );
                return getFromCache( series, languageID + ".xml", languageID );
            }
            else
            {
                // if we have an overridden language then we do not care
                int series = Int32.Parse( sSeriesID );
                return getFromCache( series, languageID + ".xml", languageID );
            }
        }

        static public bool SubmitRating( RatingType type, string itemId, int rating )
        {
            string account = DBOption.GetOptions( DBOption.cOnlineUserID );
            if ( String.IsNullOrEmpty( account ) )
            {
                string[] lines = new string[] { Translation.TVDB_INFO_ACCOUNTID_1, Translation.TVDB_INFO_ACCOUNTID_2 };
                //TVSeriesPlugin.ShowDialogOk(Translation.TVDB_INFO_TITLE, lines); //trakt.tv also listens to this, also can store ratings locally.
                MPTVSeriesLog.Write( "Cannot submit rating to thetvdb.com, this requires your Account Identifier to be set." );
                return false;
            }

            if ( itemId == "0" || rating < 0 || rating > 10 )
            {
                MPTVSeriesLog.Write( "Cannot submit rating, invalid values...this is most likely a programming error" );
                return false;
            }

            if ( !DBOnlineMirror.IsMirrorsAvailable )
            {
                // Server maybe available now.
                DBOnlineMirror.Init();
                if ( !DBOnlineMirror.IsMirrorsAvailable )
                {
                    GUIDialogOK dlgOK = ( GUIDialogOK )GUIWindowManager.GetWindow( ( int )GUIWindow.Window.WINDOW_DIALOG_OK );
                    dlgOK.SetHeading( Translation.TVDB_ERROR_TITLE );
                    if ( !TVSeriesPlugin.IsNetworkAvailable )
                    {
                        string[] lines = new string[] { Translation.NETWORK_ERROR_UNAVAILABLE_1, Translation.NETWORK_ERROR_UNAVAILABLE_2 };
                        TVSeriesPlugin.ShowDialogOk( Translation.TVDB_ERROR_TITLE, lines );
                    }
                    else
                    {
                        string[] lines = new string[] { Translation.TVDB_ERROR_UNAVAILABLE_1, Translation.TVDB_ERROR_UNAVAILABLE_2 };
                        TVSeriesPlugin.ShowDialogOk( Translation.TVDB_ERROR_TITLE, lines );
                    }

                    MPTVSeriesLog.Write( "Cannot submit rating, the online database is unavailable" );
                    return false;
                }
            }
            // ok we're good
            MPTVSeriesLog.Write( string.Format( "Submitting Rating of {2} for {0} {1}", type.ToString(), itemId, rating ), MPTVSeriesLog.LogLevel.Debug );
            Generic( string.Format( apiURIs.SubmitRating, account, type.ToString(), itemId, rating ), true, false, Format.NoExtension );
            return true;
        }

        static public XmlNode Updates( UpdateType type, Format format = Format.Zip )
        {
            string typeName = Enum.GetName( typeof( UpdateType ), type );
            return Generic( string.Format( apiURIs.Updates, typeName ), true, true, format, "updates_" + typeName, -1 );
        }

        static public XmlNode UpdateEpisodes( int seriesID )
        { return UpdateEpisodes( seriesID, true ); }

        static XmlNode UpdateEpisodes( int seriesID, bool first )
        {
            if ( DBOption.GetOptions( DBOption.cOverrideLanguage ) )
            {
                string SelLang = GetLanguageOverride( seriesID.ToString() );
                return getFromCache( seriesID, SelLang + ".xml" );
            }
            else
            {
                return getFromCache( seriesID, SelLanguageAsString + ".xml" );
            }
        }
        
        static public XmlNode GetBannerList( int aSeriesID, string aLanguageId = null )
        {
            if ( aLanguageId == null )
            {
                // get current language or series overridden language
                return getFromCache( aSeriesID, "banners.xml" );
            }
            else
            {
                string lFilename = Path.Combine( Settings.GetPath( Settings.Path.config ), string.Format( @"Cache\{0}\{1}\{2}", aSeriesID, aLanguageId, "banners.xml" ) );
                XmlNode lNode = Helper.LoadXmlCache( lFilename );
                if ( lNode != null ) return lNode;

                // download and save to cache
                lNode = Generic( string.Format( apiURIs.FullSeriesUpdate, aSeriesID, aLanguageId ),
                                 true, true, Format.Zip, aLanguageId, aSeriesID );

                if ( lNode == null ) return null;

                // our cache is not set, get from cache
                return GetBannerList(aSeriesID, aLanguageId);
            }
        }

        static public XmlNode GetActorsList( int seriesID )
        {
            return getFromCache( seriesID, "actors.xml" );
        }

        static public string DownloadBanner( string onlineFilename, Settings.Path localPath, string localFilename )
        {
            WebClient webClient = new WebClient();
            string fullLocalPath = Helper.PathCombine( Settings.GetPath( localPath ), localFilename );
            string fullURL = ( DBOnlineMirror.Banners.EndsWith( "/" ) ? DBOnlineMirror.Banners : ( DBOnlineMirror.Banners + "/" ) ) + onlineFilename;
            webClient.Headers.Add( "user-agent", Settings.UserAgent );

            // .NET 4.0: Use TLS v1.2. Many download sources no longer support the older and now insecure TLS v1.0/1.1 and SSL v3.
            ServicePointManager.SecurityProtocol = ( SecurityProtocolType )0xc00;

            try
            {
                Directory.CreateDirectory( Path.GetDirectoryName( fullLocalPath ) );
                if ( !File.Exists( fullLocalPath ) // only if the file doesn't exist
                    || ImageAllocator.LoadImageFastFromFile( fullLocalPath ) == null ) // or the file is damaged
                {
                    MPTVSeriesLog.Write( "Downloading new Image from: " + fullURL, MPTVSeriesLog.LogLevel.Debug );
                    webClient.DownloadFile( fullURL, fullLocalPath );
                    return fullLocalPath;
                }
                return string.Empty;
            }
            catch ( WebException ex )
            {
                MPTVSeriesLog.Write( $"Banner download failed from '{fullURL}' to '{fullLocalPath.Replace("/", @"\")}'. Reason='{ex.Message}'" );
                return null;
            }
        }

        static public int StartFileDownload( string fullURL, Settings.Path localPath, string localFilename )
        {
            WebClient webClient = new WebClient();

            // .NET 4.0: Use TLS v1.2. Many download sources no longer support the older and now insecure TLS v1.0/1.1 and SSL v3.
            ServicePointManager.SecurityProtocol = ( SecurityProtocolType )0xc00;

            int nDownloadGUID = nDownloadGUIDGenerator;
            nDownloadGUIDGenerator++;

            string fullLocalPath = Helper.PathCombine( Settings.GetPath( localPath ), localFilename );
            webClient.Headers.Add( "user-agent", Settings.UserAgent );
            try
            {
                Directory.CreateDirectory( Path.GetDirectoryName( fullLocalPath ) );
                if ( !File.Exists( fullLocalPath ) // only if the file doesn't exist
                    || ImageAllocator.LoadImageFastFromFile( fullLocalPath ) == null ) // or the file is damaged
                {
                    webClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler( webClient_DownloadFileCompleted );
                    webClient.DownloadFileAsync( new Uri( fullURL ), fullLocalPath, nDownloadGUID );
                    webClientList.Add( nDownloadGUID, webClient );
                    return nDownloadGUID;
                }
                return -1;
            }
            catch ( WebException )
            {
                MPTVSeriesLog.Write( "File download failed (" + fullURL + ") to " + fullLocalPath.Replace( "/", @"\" ) );
                return -1;
            }
        }

        static void webClient_DownloadFileCompleted( object sender, System.ComponentModel.AsyncCompletedEventArgs e )
        {
            webClientList.Remove( ( int )e.UserState );
        }

        static public bool CheckFileDownload( int nDownloadGUID )
        {
            return webClientList.ContainsKey( nDownloadGUID );
        }

        static public bool CancelFileDownload( int nDownloadGUID )
        {
            if ( webClientList.ContainsKey( nDownloadGUID ) )
            {
                WebClient client = webClientList[nDownloadGUID];
                client.CancelAsync();
                return true;
            }
            return false;
        }

        static XmlNode getFromCache( int aSeriesID, string aXMLFilename )
        {
            return getFromCache( aSeriesID, true, aXMLFilename, GetSeriesLanguage(aSeriesID) );
        }

        static XmlNode getFromCache( int aSeriesID, string aXMLFilename, string aLanguageId )
        {
            return getFromCache( aSeriesID, true, aXMLFilename, aLanguageId );
        }

        static XmlNode getFromCache( int aSeriesID, bool aRefresh, string aXMLFilename, string aLanguageId )
        {
            // cache filename
            string filename = Path.Combine( Settings.GetPath( Settings.Path.config ), string.Format( @"Cache\{0}\{1}\{2}", aSeriesID, aLanguageId, aXMLFilename ) );

            // check if in cache
            XmlNode node = Helper.LoadXmlCache( filename );
            if ( node != null ) return node;

            // xml is not cached, retrieve online
            if ( aRefresh )
            {
                Generic( string.Format( apiURIs.FullSeriesUpdate, aSeriesID, aLanguageId ),
                        true, true, Format.Zip, aLanguageId, aSeriesID );

                // its now cached, so load it
                return getFromCache( aSeriesID, false, aXMLFilename, aLanguageId );
            }
            return null;
        }

        #region Generic Private Implementation
        static XmlNode Generic( String sUrl, Format format )
        { return Generic( sUrl, true, format ); }

        static XmlNode Generic( String sUrl, bool appendBaseUrl, Format format )
        { return Generic( sUrl, appendBaseUrl, true, format ); }

        static XmlNode Generic( String sUrl, bool appendBaseUrl, bool appendAPIKey, Format format )
        { return Generic( sUrl, appendBaseUrl, appendAPIKey, format, null, 0 ); }

        static XmlNode Generic( String sUrl, bool appendBaseUrl, bool appendAPIKey, Format format, string aLanguageId, int seriesIDIfZip )
        {
            if ( format == Format.Zip )
            {
                if ( appendBaseUrl ) sUrl = DBOnlineMirror.ZipInterface + sUrl;
            }
            else
            {
                if ( appendBaseUrl && appendAPIKey ) sUrl = DBOnlineMirror.Interface + sUrl;
                else if ( appendBaseUrl ) sUrl = DBOnlineMirror.InterfaceWithoutKey + sUrl;
            }
            switch ( format )
            {
                case Format.Xml:
                    sUrl += ".xml";
                    break;
                case Format.Zip:
                    sUrl += ".zip";
                    break;
            }
            Stream lStreamData = RetrieveData( sUrl );

            if ( lStreamData != null )
            {
                if ( format == Format.Zip )
                {
                    if ( !String.IsNullOrEmpty( aLanguageId ) && seriesIDIfZip != 0 )
                    {
                        // save the zip file downloaded to disk
                        //string lFilename = Path.Combine(Settings.GetPath(Settings.Path.config), string.Format(@"Cache\{0}\{1}.zip", seriesIDIfZip, aLanguageId));
                        //Helper.SaveZipCache(lFilename, lStreamData);

                        Dictionary<string, XmlDocument> lZip = DecompressZipToXmls( lStreamData );
                        string lFilenameInZip = aLanguageId + ".xml";
                        XmlNode lRoot;

                        // save all xmls in zip to cache
                        foreach ( string key in lZip.Keys )
                        {
                            string lFilename = Path.Combine( Settings.GetPath( Settings.Path.config ), string.Format( @"Cache\{0}\{1}\{2}", seriesIDIfZip, aLanguageId, key ) );
                            Helper.SaveXmlCache( lFilename, lZip[key].FirstChild.NextSibling ?? lZip[key].FirstChild );
                        }

                        // get what we are looking for            
                        if (lZip.ContainsKey(lFilenameInZip))
                        {
                            lRoot = lZip[lFilenameInZip].FirstChild.NextSibling;
                            return lRoot;
                        }
                        else
                        {
                            MPTVSeriesLog.Write("Decompression returned null or not the requested entry");
                        }
                    }
                }
                else
                {
                    var reader = new StreamReader( lStreamData );
                    String sXmlData = string.Empty;
                    try
                    {
                        sXmlData = reader.ReadToEnd().Replace( '\0', ' ' );
                    }
                    catch ( Exception e )
                    {
                        MPTVSeriesLog.Write( "Error reading stream: {0}", e.Message );
                    }

                    lStreamData.Close();
                    reader.Close();
                    if ( !string.IsNullOrEmpty( sXmlData ) )
                    {
                        MPTVSeriesLog.Write( "*************************************", MPTVSeriesLog.LogLevel.Debug );
                        MPTVSeriesLog.Write( sXmlData, MPTVSeriesLog.LogLevel.Debug );
                        MPTVSeriesLog.Write( "*************************************", MPTVSeriesLog.LogLevel.Debug );
                        try
                        {
                            var doc = new XmlDocument();
                            doc.LoadXml( sXmlData );
                            XmlNode root = doc.FirstChild.NextSibling;
                            return root;
                        }
                        catch ( XmlException e )
                        {
                            // Most likely bad xml formatting, tvdb does not use CDATA structures so good luck.
                            MPTVSeriesLog.Write( "Xml parsing of " + sUrl + " failed (line " + e.LineNumber + " - " + e.Message + ")" );
                        }
                    }
                }
            }
            return null;
        }

        static Stream RetrieveData( String aUrl )
        {
            MPTVSeriesLog.Write( $"Retrieving Data from '{aUrl}'", MPTVSeriesLog.LogLevel.Debug );
            if ( aUrl == null || aUrl.Length < 1 || aUrl[0] == '/' )
            {
                // this happens if no active mirror is set
                return null;
            }

            HttpWebRequest lRequest;
            HttpWebResponse lResponse;

            try
            {
                // .NET 4.0: Use TLS v1.2. Many download sources no longer support the older and now insecure TLS v1.0/1.1 and SSL v3.
                ServicePointManager.SecurityProtocol = ( SecurityProtocolType )0xc00;

                // force secure http
                lRequest = ( HttpWebRequest )WebRequest.Create( aUrl );
                // Note: some network proxies require the useragent string to be set or they will deny the http request
                // this is true for instance for EVERY thailand internet connection (also needs to be set for banners/episodethumbs and any other http request we send)
                lRequest.UserAgent = Settings.UserAgent;
                lRequest.Timeout = 60000;

                lResponse = ( HttpWebResponse )lRequest.GetResponse();

                if ( lResponse != null ) // Get the stream associated with the response.
                {
                    return lResponse.GetResponseStream();
                }   
            }
            catch ( Exception e )
            {
                MPTVSeriesLog.Write( $"Unable to connect to '{aUrl}'. Error = '{e.Message}'" );
            }
            finally
            {
                //if (response != null) response.Close(); // screws up the decompression
            }

            return null;
        }

        static Dictionary<string, XmlDocument> DecompressZipToXmls( Stream aStream )
        {
            MPTVSeriesLog.Write( "Decompressing Stream...", MPTVSeriesLog.LogLevel.Debug );

            var lDocsInZip = new Dictionary<string, XmlDocument>();

            ZipConstants.DefaultCodePage = 850;
            var lZipStream = new ZipInputStream( aStream );
            ZipEntry currEntry = lZipStream.GetNextEntry();

            while ( currEntry != null )
            {
                MPTVSeriesLog.Write( "Decompressing Entry: ", currEntry.Name, MPTVSeriesLog.LogLevel.Debug );
                byte[] buffer = new byte[4096];

                var doc = new XmlDocument();
                var stream = new MemoryStream();
                try
                {
                    StreamUtils.Copy( lZipStream, stream, buffer );
                    stream.Position = 0;
                    MPTVSeriesLog.Write( "Decompression done, now loading as XML...", MPTVSeriesLog.LogLevel.Debug );
                    doc.Load( stream );
                    MPTVSeriesLog.Write( "Loaded as valid XML", MPTVSeriesLog.LogLevel.Debug );
                    // check if .zip in filename and remove for backwards compatibility
                    lDocsInZip.Add( currEntry.Name.Replace( ".zip", string.Empty ), doc );
                }
                catch ( XmlException e )
                {
                    MPTVSeriesLog.Write( "Failed to load XML: " + e.Message.ToString() );
                }

                stream.Close();
                currEntry = lZipStream.GetNextEntry();
            }
            return lDocsInZip;
        }

        #endregion
    }
}
