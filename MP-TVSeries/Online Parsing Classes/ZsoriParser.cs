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
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    class ZsoriParser
    {
        static public XmlNodeList GetMirrors(String sServer)
        {
            return Generic(sServer + "/GetMirrors.php");
        }

        static public XmlNodeList GetSeries(String sSeriesName)
        {
            return Generic(DBOnlineMirror.Interface + "/GetSeries.php?seriesname=" + sSeriesName.Replace(' ', '+'));
        }

        static public XmlNodeList GetEpisodes(int nSeriesID, long nGetEpisodesTimeStamp, string order)
        {
            return Generic(DBOnlineMirror.Interface + "/GetEpisodes.php?seriesid=" + nSeriesID + "&lasttime=" + nGetEpisodesTimeStamp + "&order=" + order);
        }

        static public XmlNodeList GetEpisodes(int nSeriesID, DateTime firstAired, string order)
        {
            return Generic(DBOnlineMirror.Interface + "/GetEpisodes.php?seriesid=" + nSeriesID + "&firstaired=" + firstAired.Date.ToString("yyyy-MM-dd") + "&order=" + order);
        }

        static public XmlNodeList GetEpisodes(int nSeriesID, int nSeasonIndex, int nEpisodeIndex, string order)
        {
            return Generic(DBOnlineMirror.Interface + "/GetEpisodes.php?seriesid=" + nSeriesID + "&season=" + nSeasonIndex + "&episode=" + nEpisodeIndex + "&order=" + order);
        }

        static public XmlNodeList UpdateSeries(String sSeriesIDs, long nUpdateSeriesTimeStamp)
        {
            return Generic(DBOnlineMirror.Interface + "/SeriesUpdates.php?lasttime=" + nUpdateSeriesTimeStamp + "&idlist=" + sSeriesIDs);
        }

        static public XmlNodeList UpdateEpisodes(String sEpisodesIDs, long nUpdateEpisodesTimeStamp)
        {
            return Generic(DBOnlineMirror.Interface + "/EpisodeUpdates.php?lasttime=" + nUpdateEpisodesTimeStamp + "&idlist=" + sEpisodesIDs);
        }

        static public XmlNodeList GetBanners(int nSeriesID, long nUpdateBannersTimeStamp)
        {
            return Generic(DBOnlineMirror.Interface + "/GetBanners.php?seriesid=" + nSeriesID + "&lasttime=" + nUpdateBannersTimeStamp);
        }

        static public XmlNodeList GetAllBanners(string idList, long nUpdateBannersTimeStamp)
        {
            return Generic(DBOnlineMirror.Interface + "/GetBanners.php?idlist=" + idList + "&lasttime=" + nUpdateBannersTimeStamp);
        }

        static private XmlNodeList Generic(String sUrl)
        {
            MPTVSeriesLog.Write("Retrieving Data from: ", sUrl, MPTVSeriesLog.LogLevel.Debug);
            WebClient client = new WebClient();
            Stream data = null;
            try
            {
                data = client.OpenRead(sUrl);
            }
            catch (Exception e)
            {
                // can't connect, timeout, etc
                MPTVSeriesLog.Write("Can't connect to " + sUrl + " : " + e.Message);
            }
            if (data != null)
            {
                StreamReader reader = new StreamReader(data, Encoding.Default, true);
                String sXmlData = reader.ReadToEnd().Replace('\0', ' ');
                data.Close();
                reader.Close();
                MPTVSeriesLog.Write("*************************************", MPTVSeriesLog.LogLevel.Debug);
                MPTVSeriesLog.Write(sXmlData, MPTVSeriesLog.LogLevel.Debug, false);
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
            }
            return null;
        }
    }
}
