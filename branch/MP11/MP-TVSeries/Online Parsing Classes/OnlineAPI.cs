﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using System.Web;
using MediaPortal.GUI.Library;
using MediaPortal.Dialogs;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using WindowPlugins.GUITVSeries.FollwitTv;

namespace WindowPlugins.GUITVSeries.Online_Parsing_Classes
{
  static class OnlineAPI
  {
    private static class apiURIs
    {
        public const string Mirrors = "mirrors";
        public const string Languages = "languages";
        public const string GetSeries = @"GetSeries.php?seriesname={0}&language=all";
        public const string FullSeriesUpdate = @"series/{0}/all/{1}";
        public const string Updates = "updates/updates_{0}";
        public const string SubmitRating = "User_Rating.php?accountid={0}&itemtype={1}&itemid={2}&rating={3}";
        public const string GetRatingsForUser = @"GetRatingsForUser.php?apikey={0}&accountid={1}&seriesid={2}";
        public const string ConfigureFavourites = @"User_Favorites.php?accountid={0}&type={1}&seriesid={2}";
        public const string GetAllFavourites = @"User_Favorites.php?accountid={0}";
    }

    private enum Format
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

    # region Language
    static string selLang = string.Empty;

    public static string SelLanguageAsString
    {
      get
      {
        if (selLang.Length == 0)
        {
          string lang = DBOption.GetOptions(DBOption.cOnlineLanguage);
          if (!String.IsNullOrEmpty(lang)) selLang = lang;
          else selLang = "en"; // use english
        }
        return selLang;
      }
      set { selLang = value; }
    }
    #endregion

    static public XmlNode GetMirrors(String sServer)
    {
      return Generic(sServer + apiURIs.Mirrors, false, Format.Xml);
    }

    static public XmlNode GetLanguages()
    {
      return Generic(apiURIs.Languages, Format.Xml);
    }

    static public XmlNode GetSeries(String sSeriesName)
    {
        return Generic(string.Format(apiURIs.GetSeries, HttpUtility.UrlEncode(sSeriesName)), true, false, Format.NoExtension);
    }

    static public XmlNode GetUserRatings(String sSeriesID, String sAccountID)
    {        
        string url = String.Format(apiURIs.GetRatingsForUser, DBOnlineMirror.cApiKey, sAccountID, sSeriesID);
        return Generic(url, true, false, Format.NoExtension);
    }

    static public XmlNode GetUserFavourites(String sAccountID)
    {
        string url = String.Format(apiURIs.GetAllFavourites, sAccountID);
        return Generic(url, true, false, Format.NoExtension);
    }
    
    static public XmlNode ConfigureFavourites(bool bAdd, String sAccountID, String sSeriesID)
    {
        if (sAccountID.Length == 0) {
            MPTVSeriesLog.Write("Cannot submit online favourite, make sure you have your Account identifier is set!");
            return null;
        }

        if (bAdd) {
            MPTVSeriesLog.Write(string.Format("Adding favourite series \"{0}\" to online database (theTVDB.com)", Helper.getCorrespondingSeries(int.Parse(sSeriesID))));
        } 
        else {
            MPTVSeriesLog.Write(string.Format("Removing favourite series \"{0}\" from online database (theTVDB.com)", Helper.getCorrespondingSeries(int.Parse(sSeriesID))));
        }

        string url = String.Format(apiURIs.ConfigureFavourites, sAccountID, (bAdd?"add":"remove"),sSeriesID);
        return Generic(url, true, false, Format.NoExtension);
    }

    static public XmlNode UpdateSeries(String sSeriesID)
    { return UpdateSeries(sSeriesID, true); }

    static public XmlNode UpdateSeries(String sSeriesID, String languageID)
    { return UpdateSeries(sSeriesID, languageID, true); }

    static private XmlNode UpdateSeries(String sSeriesID, bool first)
    {
      int series = Int32.Parse(sSeriesID);
      return getFromCache(series, SelLanguageAsString + ".xml");
    }

    static private XmlNode UpdateSeries(String sSeriesID, String languageID, bool first)
    {
        int series = Int32.Parse(sSeriesID);
        return getFromCache(series, languageID + ".xml", languageID);
    }

    static public bool SubmitRating(RatingType type, string itemId, int rating)
    {
      string account = DBOption.GetOptions(DBOption.cOnlineUserID);
      if (String.IsNullOrEmpty(account))
      {
        // if there is no tvdb account identifier, but we are using follwit. dont display the
        // "please fill in ID" error.
        if (FollwitConnector.Enabled) {
          return false;
        }
            string[] lines = new string[] { Translation.TVDB_INFO_ACCOUNTID_1, Translation.TVDB_INFO_ACCOUNTID_2 };
            TVSeriesPlugin.ShowDialogOk(Translation.TVDB_INFO_TITLE, lines);
        MPTVSeriesLog.Write("Cannot submit rating, make sure you have your Account identifier set!");
        return false;
      }

      if (itemId == "0" || rating < 0 || rating > 10)
      {
        MPTVSeriesLog.Write("Cannot submit rating, invalid values.....this is most likely a programming error");
        return false;
      }

      if (!DBOnlineMirror.IsMirrorsAvailable)
      {
        // Server maybe available now.
        DBOnlineMirror.Init();
        if (!DBOnlineMirror.IsMirrorsAvailable)
        {
          GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
          dlgOK.SetHeading(Translation.TVDB_ERROR_TITLE);
                if (!TVSeriesPlugin.IsNetworkAvailable)
                {
                    string[] lines = new string[] { Translation.NETWORK_ERROR_UNAVAILABLE_1, Translation.NETWORK_ERROR_UNAVAILABLE_2 };
                    TVSeriesPlugin.ShowDialogOk(Translation.TVDB_ERROR_TITLE, lines);
          }
                else
                {
                    string[] lines = new string[] { Translation.TVDB_ERROR_UNAVAILABLE_1, Translation.TVDB_ERROR_UNAVAILABLE_2 };
                    TVSeriesPlugin.ShowDialogOk(Translation.TVDB_ERROR_TITLE, lines);
          }
                
          MPTVSeriesLog.Write("Cannot submit rating, the online database is unavailable");
          return false;
        }
      }
      // ok we're good
      MPTVSeriesLog.Write(string.Format("Submitting Rating of {2} for {0} {1}", type.ToString(), itemId, rating), MPTVSeriesLog.LogLevel.Debug);
      Generic(string.Format(apiURIs.SubmitRating, account, type.ToString(), itemId, rating), true, false, Format.NoExtension);
      return true;
    }

    static public XmlNode Updates(UpdateType type)
    {
      string typeName = Enum.GetName(typeof(UpdateType), type);
      return Generic(string.Format(apiURIs.Updates, typeName), true, true, Format.Zip, "updates_" + typeName, -1);
    }

    static public XmlNode UpdateEpisodes(int seriesID)
    { return UpdateEpisodes(seriesID, true); }

    static XmlNode UpdateEpisodes(int seriesID, bool first)
    {
      return getFromCache(seriesID, SelLanguageAsString + ".xml");
    }
    
    static public XmlNode getBannerList(int seriesID)
    {
      return getFromCache(seriesID, "banners.xml");
    }

    static public XmlNode GetActorsList(int seriesID)
    {
        return getFromCache(seriesID, "actors.xml");
    }

    static public string DownloadBanner(string onlineFilename, Settings.Path localPath, string localFilename)
    {
        WebClient webClient = new WebClient();
        string fullLocalPath = Helper.PathCombine(Settings.GetPath(localPath), localFilename);
        string fullURL = (DBOnlineMirror.Banners.EndsWith("/") ? DBOnlineMirror.Banners : (DBOnlineMirror.Banners + "/")) + onlineFilename;
        webClient.Headers.Add("user-agent", Settings.UserAgent);
        //webClient.Headers.Add("referer", "http://thetvdb.com/");
        try
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullLocalPath));
            if (!System.IO.File.Exists(fullLocalPath) // only if the file doesn't exist
                || ImageAllocator.LoadImageFastFromFile(fullLocalPath) == null) // or the file is damaged
            {
                MPTVSeriesLog.Write("Downloading new Image from: " + fullURL,MPTVSeriesLog.LogLevel.Debug);
                webClient.DownloadFile(fullURL, fullLocalPath);
                return fullLocalPath;
            }
            return string.Empty;
        }
        catch (WebException)
        {
            MPTVSeriesLog.Write("Banner download failed (" + fullURL + ") to " + fullLocalPath);           
            return string.Empty;
        }
    }

    static public int StartFileDownload(string fullURL, Settings.Path localPath, string localFilename)
    {
      WebClient webClient = new WebClient();
      int nDownloadGUID = nDownloadGUIDGenerator;
      nDownloadGUIDGenerator++;

      string fullLocalPath = Helper.PathCombine(Settings.GetPath(localPath), localFilename);
      webClient.Headers.Add("user-agent", Settings.UserAgent);
      try
      {
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullLocalPath));
        if (!System.IO.File.Exists(fullLocalPath) // only if the file doesn't exist
            || ImageAllocator.LoadImageFastFromFile(fullLocalPath) == null) // or the file is damaged
        {
          webClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(webClient_DownloadFileCompleted);
          webClient.DownloadFileAsync(new Uri(fullURL), fullLocalPath, nDownloadGUID);
          webClientList.Add(nDownloadGUID, webClient);
          return nDownloadGUID;
        }
        return -1;
      }
      catch (WebException)
      {
        MPTVSeriesLog.Write("File download failed (" + fullURL + ") to " + fullLocalPath);
        return -1;
      }
    }

    static void webClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
      webClientList.Remove((int)e.UserState);
    }

    static public bool CheckFileDownload(int nDownloadGUID)
    {
      return webClientList.ContainsKey(nDownloadGUID);
    }

    static public bool CancelFileDownload(int nDownloadGUID)
    {
      if (webClientList.ContainsKey(nDownloadGUID))
      {
        WebClient client = webClientList[nDownloadGUID];
        client.CancelAsync();
        return true;
      }
      return false;
    }

    static XmlNode getFromCache(int seriesID, string elemName)
    {
        return getFromCache(seriesID, true, elemName, SelLanguageAsString);
    }

    static XmlNode getFromCache(int seriesID, string elemName, string languageID)
    {
        return getFromCache(seriesID, true, elemName, languageID);
    }

    static XmlNode getFromCache(int seriesID, bool first, string elemName, string languageID)
    {
        // cache filename
        string filename = Path.Combine(Settings.GetPath(Settings.Path.config), string.Format(@"Cache\{0}\{1}", seriesID, elemName));

        // check if in cache
        XmlNode node = Helper.LoadXmlCache(filename);
        if (node != null) return node;

        // xml is not cached, retrieve online
        if (first)
        {
            Generic(string.Format(apiURIs.FullSeriesUpdate, seriesID, languageID),
                    true, true, Format.Zip, languageID, seriesID);

            // its now cached, so load it
            return getFromCache(seriesID, false, elemName, languageID);
        }
        return null;
    }

    #region Generic Private Implementation
    static XmlNode Generic(String sUrl, Format format)
    { return Generic(sUrl, true, format); }

    static XmlNode Generic(String sUrl, bool appendBaseUrl, Format format)
    { return Generic(sUrl, appendBaseUrl, true, format); }

    static XmlNode Generic(String sUrl, bool appendBaseUrl, bool appendAPIKey, Format format)
    { return Generic(sUrl, appendBaseUrl, appendAPIKey, format, null, 0); }

    static XmlNode Generic(String sUrl, bool appendBaseUrl, bool appendAPIKey, Format format, string entryNameToGetIfZip, int seriesIDIfZip)
    {
      if (format == Format.Zip)
      {
        if (appendBaseUrl) sUrl = DBOnlineMirror.ZipInterface + sUrl;
      }
      else
      {
        if (appendBaseUrl && appendAPIKey) sUrl = DBOnlineMirror.Interface + sUrl;
        else if (appendBaseUrl) sUrl = DBOnlineMirror.InterfaceWithoutKey + sUrl;
      }
      switch (format)
      {
        case Format.Xml: sUrl += ".xml";
          break;
        case Format.Zip: sUrl += ".zip";
          break;
      }

      Stream data = RetrieveData(sUrl);

      if (data != null)
      {
        if (format == Format.Zip)
        {
          if (!String.IsNullOrEmpty(entryNameToGetIfZip) && seriesIDIfZip != 0)
          {
            Dictionary<string, XmlDocument> x = DecompressZipToXmls(data);
            entryNameToGetIfZip += ".xml";
            XmlNode root = null;

            // save all xmls in zip to cache
            foreach(var key in x.Keys)
            {
                string filename = Path.Combine(Settings.GetPath(Settings.Path.config), string.Format(@"Cache\{0}\{1}", seriesIDIfZip, key));
                Helper.SaveXmlCache(filename, x[key].FirstChild.NextSibling);
            }

            // get what we are looking for            
            if (x.ContainsKey(entryNameToGetIfZip))
            {
                root = x[entryNameToGetIfZip].FirstChild.NextSibling;
                return root;
            }
            else MPTVSeriesLog.Write("Decompression returned null or not the requested entry");
          }

        }
        else
        {
            StreamReader reader = new StreamReader(data);
            String sXmlData = string.Empty;
            try
            {
                sXmlData = reader.ReadToEnd().Replace('\0', ' ');
            }
            catch (Exception e)
            {                
                MPTVSeriesLog.Write("Error reading stream: {0}", e.Message);
            }
            data.Close();
            reader.Close();
            if (!string.IsNullOrEmpty(sXmlData))
            {
                MPTVSeriesLog.Write("*************************************", MPTVSeriesLog.LogLevel.Debug);
                MPTVSeriesLog.Write(sXmlData, MPTVSeriesLog.LogLevel.Debug);
                MPTVSeriesLog.Write("*************************************", MPTVSeriesLog.LogLevel.Debug);
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(sXmlData);
                    XmlNode root = doc.FirstChild.NextSibling;
                    return root;
                }
                catch (XmlException e)
                {
                    // Most likely bad xml formatting, tvdb does not use CDATA structures so good luck.
                    MPTVSeriesLog.Write("Xml parsing of " + sUrl + " failed (line " + e.LineNumber + " - " + e.Message + ")");
                }
            }
        }
      }
      return null;
    }

    static Stream RetrieveData(String sUrl)
    {
      MPTVSeriesLog.Write("Retrieving Data from: ", sUrl, MPTVSeriesLog.LogLevel.Debug);
      if (sUrl == null || sUrl.Length < 1 || sUrl[0] == '/')
      {
        // this happens if no active mirror is set
        return null;
      }
      HttpWebRequest request = null;
      HttpWebResponse response = null;
      try
      {
        request = (HttpWebRequest)WebRequest.Create(sUrl);
        // Note: some network proxies require the useragent string to be set or they will deny the http request
        // this is true for instance for EVERY thailand internet connection (also needs to be set for banners/episodethumbs and any other http request we send)
        request.UserAgent = Settings.UserAgent;
        request.Timeout = 20000;        
        response = (HttpWebResponse)request.GetResponse();

        if (response != null) // Get the stream associated with the response.
          return response.GetResponseStream();

      }
      catch (Exception e)
      {
        // can't connect, timeout, etc
        MPTVSeriesLog.Write("Can't connect to " + sUrl + " : " + e.Message);
      }
      finally
      {
        //if (response != null) response.Close(); // screws up the decompression
      }

      return null;
    }
    
    static Dictionary<string, XmlDocument> DecompressZipToXmls(Stream s)
    {
        MPTVSeriesLog.Write("Decompressing Stream...", MPTVSeriesLog.LogLevel.Debug);

        Dictionary<string, XmlDocument> docsInZip = new Dictionary<string, XmlDocument>();
        
        ZipInputStream zis = new ZipInputStream(s);
        ZipEntry currEntry = zis.GetNextEntry();

        while (currEntry != null)
        {
            MPTVSeriesLog.Write("Decompressing Entry: ", currEntry.Name, MPTVSeriesLog.LogLevel.Debug);
            
            byte[] buffer = new byte[4096];

            XmlDocument doc = new XmlDocument();
            MemoryStream stream = new MemoryStream();

            try
            {
                StreamUtils.Copy(zis, stream, buffer);
                stream.Position = 0;

                MPTVSeriesLog.Write("Decompression done, now loading as XML...", MPTVSeriesLog.LogLevel.Debug);
                doc.Load(stream);

                MPTVSeriesLog.Write("Loaded as valid XML", MPTVSeriesLog.LogLevel.Debug);
                docsInZip.Add(currEntry.Name, doc);
            }
            catch (XmlException e)
            {
                MPTVSeriesLog.Write("Failed to load XML: " + e.Message.ToString());
            }

            stream.Close();
            currEntry = zis.GetNextEntry();
        }
        return docsInZip;
    }

    #endregion
  }
}
