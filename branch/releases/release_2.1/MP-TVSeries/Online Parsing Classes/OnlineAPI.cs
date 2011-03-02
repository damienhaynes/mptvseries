﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using MediaPortal.GUI.Library;
using MediaPortal.Dialogs;
using ICSharpCode.SharpZipLib.Zip;

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

    static Dictionary<int, Dictionary<string, XmlDocument>> zipCache = new Dictionary<int, Dictionary<string, XmlDocument>>();
    static Dictionary<int, WebClient> webClientList = new Dictionary<int, WebClient>();
    static int nDownloadGUIDGenerator = 1;

    static public void ClearBuffer()
    {
      zipCache = new Dictionary<int, Dictionary<string, XmlDocument>>();
    }

    # region Language
    static string selLang = string.Empty;

    public static string SelLanguageAsString
    {
      get
      {
        if (selLang.Length == 0)
        {
          string lang = DBOption.GetOptions(DBOption.cOnlineLanguage);
          if (!Helper.String.IsNullOrEmpty(lang)) selLang = lang;
          else selLang = "en"; // use english
        }
        return selLang;
      }
      set { selLang = value; }
    }
    #endregion

    static public XmlNodeList GetMirrors(String sServer)
    {
      return Generic(sServer + apiURIs.Mirrors, false, Format.Xml);
    }

    static public XmlNodeList GetLanguages()
    {
      return Generic(apiURIs.Languages, Format.Xml);
    }

    static public XmlNodeList GetSeries(String sSeriesName)
    {
      return Generic(string.Format(apiURIs.GetSeries,
                                     sSeriesName.Replace(' ', '+')), true, false, Format.NoExtension);
    }

    static public XmlNodeList GetUserRatings(String sSeriesName)
    {
        return Generic(apiURIs.GetRatingsForUser, Format.Xml);
    }

    static public XmlNodeList UpdateSeries(String sSeriesID)
    { return UpdateSeries(sSeriesID, true); }

    static private XmlNodeList UpdateSeries(String sSeriesID, bool first)
    {
      int series = Int32.Parse(sSeriesID);
      return getFromCache(series, SelLanguageAsString + ".xml");
    }

    static public bool SubmitRating(RatingType type, string itemId, int rating)
    {
      string account = DBOption.GetOptions(DBOption.cOnlineUserID);
      if (Helper.String.IsNullOrEmpty(account))
      {
        GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
        dlgOK.SetHeading(Translation.TVDB_INFO_TITLE);
        dlgOK.SetLine(1, Translation.TVDB_INFO_ACCOUNTID_1);
        dlgOK.SetLine(2, Translation.TVDB_INFO_ACCOUNTID_2);
        dlgOK.DoModal(GUIWindowManager.ActiveWindow);
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
            dlgOK.SetLine(1, Translation.NETWORK_ERROR_UNAVAILABLE);
          }
          else
            dlgOK.SetLine(1, Translation.TVDB_ERROR_UNAVAILABLE);

          dlgOK.DoModal(GUIWindowManager.ActiveWindow);
          MPTVSeriesLog.Write("Cannot submit rating, the online database is unavailable");
          return false;
        }
      }
      // ok we're good
      MPTVSeriesLog.Write(string.Format("Submitting Rating of {2} for {0} {1}", type.ToString(), itemId, rating), MPTVSeriesLog.LogLevel.Debug);
      Generic(string.Format(apiURIs.SubmitRating, account, type.ToString(), itemId, rating), true, false, Format.NoExtension);
      return true;
    }

    static public XmlNodeList Updates(UpdateType type)
    {
      string typeName = Enum.GetName(typeof(UpdateType), type);
      return Generic(string.Format(apiURIs.Updates, typeName), true, true, Format.Zip, "updates_" + typeName, -1);
    }

    static public XmlNodeList UpdateEpisodes(int seriesID)
    { return UpdateEpisodes(seriesID, true); }

    static XmlNodeList UpdateEpisodes(int seriesID, bool first)
    {
      return getFromCache(seriesID, SelLanguageAsString + ".xml");
    }

    static public XmlNodeList getBannerList(int seriesID)
    {
      return getFromCache(seriesID, "banners.xml");
    }

    static public bool DownloadBanner(string onlineFilename, Settings.Path localPath, string localFilename)
    {
        WebClient webClient = new WebClient();
        string fullLocalPath = Helper.PathCombine(Settings.GetPath(localPath), localFilename);
        string fullURL = (DBOnlineMirror.Banners.EndsWith("/") ? DBOnlineMirror.Banners : (DBOnlineMirror.Banners + "/")) + onlineFilename;
        webClient.Headers.Add("user-agent", Settings.UserAgent);
        try
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullLocalPath));
            if (!System.IO.File.Exists(fullLocalPath) // only if the file doesn't exist
                || ImageAllocator.LoadImageFastFromFile(fullLocalPath) == null) // or the file is damaged
            {
                webClient.DownloadFile(fullURL, fullLocalPath);
            }
            return true;
        }
        catch (WebException)
        {
            MPTVSeriesLog.Write("Banner download failed (" + fullURL + ") to " + fullLocalPath);
            return false;
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

    static XmlNodeList getFromCache(int seriesID, string elemName)
    {
      return getFromCache(seriesID, true, elemName);
    }
    static XmlNodeList getFromCache(int seriesID, bool first, string elemName)
    {
      if (zipCache.ContainsKey(seriesID))
      {
        // we downloaded the zip before, lets get the record from it
        Dictionary<string, XmlDocument> d = zipCache[seriesID];
        if (d.ContainsKey(elemName))
        {
          return d[elemName].ChildNodes;
        }
      }
      else if (first)
      {
        Generic(string.Format(apiURIs.FullSeriesUpdate,
                                   seriesID,
                                   SelLanguageAsString), true, true, Format.Zip, SelLanguageAsString, seriesID);
        return getFromCache(seriesID, false, elemName);
      }
      return null;
    }

    #region Generic Private Implementation
    static XmlNodeList Generic(String sUrl, Format format)
    { return Generic(sUrl, true, format); }

    static XmlNodeList Generic(String sUrl, bool appendBaseUrl, Format format)
    { return Generic(sUrl, appendBaseUrl, true, format); }

    static XmlNodeList Generic(String sUrl, bool appendBaseUrl, bool appendAPIKey, Format format)
    { return Generic(sUrl, appendBaseUrl, appendAPIKey, format, null, 0); }

    static XmlNodeList Generic(String sUrl, bool appendBaseUrl, bool appendAPIKey, Format format, string entryNameToGetIfZip, int seriesIDIfZip)
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
          if (!Helper.String.IsNullOrEmpty(entryNameToGetIfZip) && seriesIDIfZip != 0)
          {
            Dictionary<string, XmlDocument> x = DecompressZipToXmls(data);
            entryNameToGetIfZip += ".xml";
            if (x.ContainsKey(entryNameToGetIfZip))
            {
              XmlNode root = x[entryNameToGetIfZip].FirstChild.NextSibling;
              //x.Remove(entryNameToGetIfZip);

              if (zipCache.ContainsKey(seriesIDIfZip)) zipCache.Remove(seriesIDIfZip);
              if (x.Keys.Count > 0) zipCache.Add(seriesIDIfZip, x);

              return root.ChildNodes;
            }
            else MPTVSeriesLog.Write("Decompression returned null or not the requested entry");
          }

        }
        else
        {
          StreamReader reader = new StreamReader(data);
          String sXmlData = reader.ReadToEnd().Replace('\0', ' ');
          data.Close();
          reader.Close();
          MPTVSeriesLog.Write("*************************************", MPTVSeriesLog.LogLevel.Debug);
          MPTVSeriesLog.Write(sXmlData, MPTVSeriesLog.LogLevel.Debug);
          MPTVSeriesLog.Write("*************************************", MPTVSeriesLog.LogLevel.Debug);
          try
          {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sXmlData);
            XmlNode root = doc.FirstChild.NextSibling;
            return root.ChildNodes;
          }
          catch (XmlException e)
          {
            // bummer
            MPTVSeriesLog.Write("Xml parsing of " + sUrl + " failed (line " + e.LineNumber + " - " + e.Message + ")");
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

    static UTF8Encoding enc = new UTF8Encoding();
    static Dictionary<string, XmlDocument> DecompressZipToXmls(Stream s)
    {
      MPTVSeriesLog.Write("Decompressing Stream...", MPTVSeriesLog.LogLevel.Debug);
      int bytes = 2048;
      byte[] data = new byte[2048];
      Dictionary<string, XmlDocument> docsInZip = new Dictionary<string, XmlDocument>();
      ZipInputStream zis = new ZipInputStream(s);
      ZipEntry currEntry = null;
      StringBuilder b = new StringBuilder();
      while ((currEntry = zis.GetNextEntry()) != null)
      {
        MPTVSeriesLog.Write("Decompressing Entry: ", currEntry.Name, MPTVSeriesLog.LogLevel.Debug);
        XmlDocument d = new XmlDocument();
        while ((bytes = zis.Read(data, 0, data.Length)) > 0)
          b.Append(enc.GetString(data, 0, bytes));
        MPTVSeriesLog.Write("Decompression done, now loading as XML...", MPTVSeriesLog.LogLevel.Debug);
        try
        {
            d.LoadXml(b.ToString());
            MPTVSeriesLog.Write("Loaded as valid XML", MPTVSeriesLog.LogLevel.Debug);
            docsInZip.Add(currEntry.Name, d);
        }
        catch (XmlException e)
        {
            MPTVSeriesLog.Write("Failed to load XML: " + e.Message.ToString());
        }
        b.Remove(0, b.Length);                
      }
      return docsInZip;
    }

    #endregion
  }
}
