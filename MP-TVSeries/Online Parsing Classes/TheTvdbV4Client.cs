using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using WindowPlugins.GUITVSeries.Online_Parsing_Classes.DTO;

namespace WindowPlugins.GUITVSeries.Online_Parsing_Classes
{
  public static class TheTvdbV4Client
  {
    public delegate void OnDataSendDelegate( string url, string postData );
    public delegate void OnDataReceivedDelegate( string response, HttpWebResponse webResponse );
    public delegate void OnDataErrorDelegate( string error );

    public static event OnDataSendDelegate OnDataSend;
    public static event OnDataReceivedDelegate OnDataReceived;
    public static event OnDataErrorDelegate OnDataError;

    private const string BaseUrl = "https://api4.thetvdb.com/v4/";

    /// <summary>
    /// Perform login with apiKey (from parameter or embedded resource). Returns bearer token or null on failure.
    /// </summary>
    public static string Login( string apiKey = null, int timeoutMs = 30_000 )
    {
      // if no apiKey provided, try to load from embedded resource file 'APIKey.resx'
      if ( string.IsNullOrEmpty( apiKey ) )
      {
        try
        {
          apiKey = Credentials.APIKey;
        }
        catch ( Exception )
        {
          apiKey = null;
        }
      }

      if ( string.IsNullOrEmpty( apiKey ) || apiKey.Equals( "SECRET_PLACEHOLDER_TVDB_API_KEY" ) )
      {
        OnDataError?.Invoke( "No API key available for tvdb v4 login. Check Credentials.cs and add your registered API key" );
        return null;
      }

      string url = BaseUrl + "login";

      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

      try
      {
        string postBody = $"{{\"apikey\":\"{EscapeJson( apiKey )}\"}}";
        string respText = SendRequest( url, "POST", null, postBody, timeoutMs );

        var serializer = new DataContractJsonSerializer( typeof( TvdbLoginResponse ) );
        using ( var ms = new MemoryStream( Encoding.UTF8.GetBytes( respText ) ) )
        {
          var loginResp = (TvdbLoginResponse)serializer.ReadObject( ms );
          string token = loginResp?.Data?.Token;
          if (!string.IsNullOrEmpty(token))
          {
            try
            {
              // token is valid for 1 month so store expiry too
              DBOption.SetOptions(DBOption.cTVDBv4Token, token);
              DBOption.SetOptions(DBOption.cTVDBv4TokenExpiry, DateTime.UtcNow.AddDays(30).ToString("o"));
            }
            catch (Exception ex)
            {
              OnDataError?.Invoke("Failed to persist token: " + ex.Message);
            }
          }
          return token;
        }
      }
      catch ( WebException wex )
      {
        string msg = wex.Message;
        if ( wex.Response is HttpWebResponse hr )
        {
          msg = $"HTTP {(int)hr.StatusCode} - {hr.StatusDescription}";
        }
        OnDataError?.Invoke( $"Login failed: {msg}" );
        return null;
      }
      catch ( Exception ex )
      {
        OnDataError?.Invoke( "Login failed: " + ex.Message );
        return null;
      }
    }

    /// <summary>
    /// Search series by name. Returns list of series or null on error.
    /// </summary>
    public static List<TvdbSeries> SearchSeries( string bearerToken, string seriesName, int limit = 10, int timeoutMs = 60_000 )
    {
      if ( string.IsNullOrEmpty( bearerToken ) || string.IsNullOrEmpty( seriesName ) )
        return null;

      string encoded = Uri.EscapeDataString( seriesName );
      string url = BaseUrl + $"search?query={encoded}&type=series&limit={limit}";

      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

      try
      {
        var headers = new WebHeaderCollection
        {
          { HttpRequestHeader.Authorization, "Bearer " + bearerToken }
        };
        string respText = SendRequest( url, "GET", headers, null, timeoutMs );

        var settings = new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };
        var serializer = new DataContractJsonSerializer( typeof( TvdbSearchResponse ), settings );
        using ( var ms = new MemoryStream( Encoding.UTF8.GetBytes( respText ) ) )
        {
          var searchResp = (TvdbSearchResponse)serializer.ReadObject( ms );
          // temporarily log raw response for debugging
          try
          {
            MPTVSeriesLog.Write($"TVDB v4 search URL: {url}", MPTVSeriesLog.LogLevel.Debug);
            MPTVSeriesLog.Write($"TVDB v4 bearer present: {(string.IsNullOrEmpty(bearerToken) ? "no" : "yes (hidden)")}", MPTVSeriesLog.LogLevel.Debug);
            MPTVSeriesLog.Write($"TVDB v4 raw response: {respText}", MPTVSeriesLog.LogLevel.Debug);
          }
          catch { }
          return searchResp?.Data;
        }
      }
      catch ( WebException wex )
      {
        string msg = wex.Message;
        if ( wex.Response is HttpWebResponse hr )
        {
          msg = $"HTTP {(int)hr.StatusCode} - {hr.StatusDescription}";
        }
        OnDataError?.Invoke( $"Search failed: {msg}" );
        return null;
      }
      catch ( Exception ex )
      {
        OnDataError?.Invoke( "Search failed: " + ex.Message );
        return null;
      }
    }

    static string EscapeJson( string s )
    {
      if ( s == null )
        return string.Empty;
      return s.Replace( "\\", "\\\\" ).Replace( "\"", "\\\"" ).Replace( "\n", "\\n" ).Replace( "\r", "\\r" );
    }

    static string SendRequest( string url, string method, WebHeaderCollection headers, string postData, int timeoutMs )
    {
      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

      OnDataSend?.Invoke( url, postData );

      var request = (HttpWebRequest)WebRequest.Create( url );
      request.Method = method;
      request.UserAgent = Settings.UserAgent;
      request.Accept = "application/json";
      request.Timeout = timeoutMs;

      if ( headers != null )
      {
        request.Headers.Add( headers );
      }

      if ( !string.IsNullOrEmpty( postData ) )
      {
        request.ContentType = "application/json";
        byte[] bytes = Encoding.UTF8.GetBytes( postData );
        request.ContentLength = bytes.Length;
        using ( var reqStream = request.GetRequestStream() )
        {
          reqStream.Write( bytes, 0, bytes.Length );
        }
      }

      using ( var response = (HttpWebResponse)request.GetResponse() )
      {
        if ( response == null ) return null;

        string respText;
        using ( var sr = new StreamReader( response.GetResponseStream() ) )
          respText = sr.ReadToEnd();

        OnDataReceived?.Invoke( respText, response );
        return respText;
      }
    }
  }
}
