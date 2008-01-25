#region GNU license
// MP-TVSeries - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2007
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#endregion

using System;
using System.IO;
using System.Net;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    sealed class ZsoriParser
    {
        ZsoriParser() { }

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
                    else selLang = "7"; // use english
                }
                return selLang;
            }
            set { selLang = value; }
        }
        #endregion

        #region Public Methods
        static public XmlNodeList GetMirrors(String sServer)
        {
            return Generic(sServer + "/GetMirrors.php", false);
        }

        static public XmlNodeList GetLanguages()
        {
            return Generic("/GetLanguages.php");
        }

        static public XmlNodeList GetSeries(String sSeriesName)
        {
            return Generic(string.Format(@"/GetSeries.php?seriesname={0}&language={1}", 
                                           sSeriesName.Replace(' ', '+'),
                                           SelLanguageAsString));
        }

        static public XmlNodeList GetEpisodes(int nSeriesID, long nGetEpisodesTimeStamp, string order)
        {
            return Generic(string.Format(@"/GetEpisodes.php?seriesid={0}&lasttime={1}&order={2}&language={3}",
                                            nSeriesID, nGetEpisodesTimeStamp, order, SelLanguageAsString));
        }

        static public XmlNodeList GetEpisodes(int nSeriesID, DateTime firstAired, string order)
        {
            return Generic(string.Format(@"/GetEpisodes.php?seriesid={0}&firstaired={1}&order={2}&language={3}",
                                           nSeriesID, firstAired.Date.ToString("yyyy-MM-dd"),
                                           order, SelLanguageAsString));
        }

        static public XmlNodeList GetEpisodes(int nSeriesID, int nSeasonIndex, int nEpisodeIndex, string order)
        {
            return Generic(string.Format(@"/GetEpisodes.php?seriesid={0}&season={1}&episode={2}&order={3}&language={4}",
                                           nSeriesID, nSeasonIndex, nEpisodeIndex, order, SelLanguageAsString));
        }

        static public XmlNodeList UpdateSeries(String sSeriesIDs, string forcedLang, long nUpdateSeriesTimeStamp)
        {
            return Generic(string.Format(@"/SeriesUpdates.php?lasttime={0}&idlist={1}&language={2}",
                                           nUpdateSeriesTimeStamp, sSeriesIDs, 
                                           (forcedLang == null ? SelLanguageAsString : forcedLang)));
        }

        static public XmlNodeList UpdateEpisodes(String sEpisodesIDs, long nUpdateEpisodesTimeStamp)
        {
            return Generic(string.Format(@"/EpisodeUpdates.php?lasttime={0}&idlist={1}&language={2}",
                                           nUpdateEpisodesTimeStamp, sEpisodesIDs, SelLanguageAsString));
        }

        static public XmlNodeList GetBanners(int nSeriesID, long nUpdateBannersTimeStamp, string forceLang)
        {
            return Generic(string.Format(@"/GetBanners.php?seriesid={0}&lasttime={1}&language={2}",
                                           nSeriesID, nUpdateBannersTimeStamp,
                                           (forceLang == null ? SelLanguageAsString : forceLang)));
        }
        #endregion

        #region Generic Private Implementation
        static XmlNodeList Generic(String sUrl)
        { return Generic(sUrl, true); }

        static XmlNodeList Generic(String sUrl, bool appendBaseUrl)
        {
            if (appendBaseUrl) sUrl = DBOnlineMirror.Interface + sUrl; 
            MPTVSeriesLog.Write("Retrieving Data from: ", sUrl, MPTVSeriesLog.LogLevel.Debug);
            if (sUrl == null || sUrl.Length < 1 || sUrl[0] == '/')
            {
                // this happens if no active mirror is set
                return null;
            }

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream data = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(sUrl);
                // Note: some network proxies require the useragent string to be set or they will deny the http request
                // this is true for instance for EVERY thailand internet connection (also needs to be set for banners/episodethumbs and any other http request we send)
                request.UserAgent = Settings.UserAgent;
                request.Timeout = 20000;
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception e)
            {
                // can't connect, timeout, etc
                MPTVSeriesLog.Write("Can't connect to " + sUrl + " : " + e.Message);
            }

            if (response != null)
            {
                // Get the stream associated with the response.
                data = response.GetResponseStream();
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
                    // skip xml node
                    XmlNode root = doc.FirstChild.NextSibling;
                    if (root.Name == "Items")
                    {
                        return root.ChildNodes;
                    }
                }
                catch (XmlException e)
                {
                    // bummer
                    MPTVSeriesLog.Write("Xml parsing of " + sUrl + " failed (line " + e.LineNumber + " - " + e.Message + ")");
                }
                response.Close();
            }


            return null;
        }

        #endregion
    }
}
