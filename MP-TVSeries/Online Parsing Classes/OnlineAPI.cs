using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace WindowPlugins.GUITVSeries.Online_Parsing_Classes
{
    class OnlineAPI
    {
        private static class apiURIs
        {
            public const string Mirrors = "mirrors";
            public const string Languages = "languages";
            public const string GetSeries = @"GetSeries.php?seriesname={0}&language={1}";
            public const string FullSeriesUpdate = @"series/{0}/all/{1}";
            public const string Updates = "Updates.php?type={0}&time={1}";
            public const string SubmitRating = "User_Rating.php?accountid={0}&itemtype={1}&itemid={2}&rating={3}";
        }

        private enum Format
        {
            Xml,
            Zip,
            NoExtension
        }

        public enum UpdateType
        {            
            none = 0,
            series = 1,
            episode = 2,
            all = 3,
        }

        static Dictionary<int, Dictionary<string, XmlDocument>> zipCache = new Dictionary<int, Dictionary<string, XmlDocument>>();

        # region Language
        static string selLang = string.Empty;

        public static string SelLanguageAsString
        {
            get
            {
                if (selLang.Length == 0)
                {
                    int lang = DBOption.GetOptions(DBOption.cOnlineLanguage);
                    if (lang != 0) selLang = lang.ToString();
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
                                           sSeriesName.Replace(' ', '+'),
                                           SelLanguageAsString), true, false, Format.NoExtension);
        }

        static public XmlNodeList UpdateSeries(String sSeriesID)
        { return UpdateSeries(sSeriesID, true); }

        static private XmlNodeList UpdateSeries(String sSeriesID, bool first)
        {
            int series = Int32.Parse(sSeriesID);
            return getFromCache(series, SelLanguageAsString + ".xml");           
        }

        static public bool SubmitRating(UpdateType type, string itemId, int rating)
        {
            string account = DBOption.GetOptions(DBOption.cOnlineUserID);
            if (Helper.String.IsNullOrEmpty(account))
            {
                MPTVSeriesLog.Write("Cannot submit rating, make sure you have your Account identifier set!");
                return false;
            }
            if (itemId == "0" || rating < 0 || rating > 10 || type == UpdateType.all || type == UpdateType.none)
            {
                MPTVSeriesLog.Write("Cannot submit rating, invalid values.....this is most likely a programming error");
                return false;
            }
            // ok we're good
            MPTVSeriesLog.Write(string.Format("Submitting Rating of {2} for {0} {1}", type.ToString(), itemId, rating));
            Generic(string.Format(apiURIs.SubmitRating, account, type.ToString(), itemId, rating), true, false, Format.NoExtension);            
            return true;
        }

        /// <summary>
        /// Warning: Limited to the 1000 most recently updated entries
        /// </summary>
        /// <param name="lastTime"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        static public XmlNodeList Updates(long lastTime, UpdateType type)
        {
            return Generic(string.Format(apiURIs.Updates, Enum.GetName(typeof(UpdateType), type), lastTime), true, false, Format.NoExtension);
        }

        static public XmlNodeList UpdateEpisodes(int seriesID)
        { return UpdateEpisodes(seriesID, true);  }
        
        static XmlNodeList UpdateEpisodes(int seriesID, bool first)
        {
            return getFromCache(seriesID, SelLanguageAsString + ".xml");
        }

        static public XmlNodeList getBannerList(int seriesID)
        {
            return getFromCache(seriesID, "banners.xml");
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
                        if(x.ContainsKey(entryNameToGetIfZip))
                        {
                            XmlNode root = x[entryNameToGetIfZip].FirstChild.NextSibling;
                            //x.Remove(entryNameToGetIfZip);

                            if (zipCache.ContainsKey(seriesIDIfZip)) zipCache.Remove(seriesIDIfZip);
                            if(x.Keys.Count > 0) zipCache.Add(seriesIDIfZip, x);

                            return root.ChildNodes;
                        }
                        else MPTVSeriesLog.Write("Decompression returned null");
                    }
                    
                }
                else
                {
                    StreamReader reader = new StreamReader(data, System.Text.Encoding.Default, true);
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
            MPTVSeriesLog.Write("Retrieving Data from: ", sUrl, MPTVSeriesLog.LogLevel.Normal);
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
            MPTVSeriesLog.Write("Decompressing Stream...");
            int bytes = 2048;
            byte[] data = new byte[2048];
            Dictionary<string, XmlDocument> docsInZip = new Dictionary<string, XmlDocument>();            
            ZipInputStream zis = new ZipInputStream(s);
            ZipEntry currEntry = null;
            StringBuilder b = new StringBuilder();

            while ((currEntry = zis.GetNextEntry()) != null)
            {
                MPTVSeriesLog.Write("Decompressing Entry: " + currEntry.Name);
                XmlDocument d = new XmlDocument();
                while ((bytes = zis.Read(data, 0, data.Length)) > 0)
                    b.Append(new ASCIIEncoding().GetString(data, 0, bytes));
                MPTVSeriesLog.Write("Decompression done, now loading as XML...");    
                d.LoadXml(b.ToString());
                b = new StringBuilder();
                MPTVSeriesLog.Write("Loaded as valid XML");
                docsInZip.Add(currEntry.Name, d);
            }
            return docsInZip;
        }

        #endregion
    }
}
