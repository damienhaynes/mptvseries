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

        static public XmlNodeList GetEpisodes(int nSeriesID)
        {
            return Generic(DBOnlineMirror.Interface + "/GetEpisodes.php?seriesid=" + nSeriesID);
        }

        static private XmlNodeList Generic(String sUrl)
        {
            WebClient client = new WebClient();
            Stream data = client.OpenRead(sUrl);
            StreamReader reader = new StreamReader(data);
            String sXmlData = reader.ReadToEnd();
            data.Close();
            reader.Close();

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
                DBTVSeries.Log("Xml parsing of " + sUrl + " failed (line " + e.LineNumber + " - " + e.Message + ")");
            }
            return null;
        }

    }
}
