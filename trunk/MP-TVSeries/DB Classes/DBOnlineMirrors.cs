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
using SQLite.NET;
using MediaPortal.Database;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    class DBOnlineMirror
    {
        private enum TypeMask
        {
            XML = 1,
            Banners = 2,
            Zip = 4
        }

        public const String cTableName = "OnlineMirrors";

        public const String cID = "ID";
        public const String cInterface = "Interface";
        public const String cBanners = "Banners";
        public const String cMirrorpath = "mirrorpath";
        public const String cTypeMask = "Typemask";

        Dictionary<string, DBField> m_fields = new Dictionary<string, DBField>();

        private static String s_sCurrentInterface = String.Empty;
        private static String s_sCurrentBanner = String.Empty;
        private static String s_sCurrentZip = String.Empty;
        private static Dictionary<String, String> s_OnlineToFieldMap = new Dictionary<String, String>();
        private static List<DBOnlineMirror> memoryMirrors = new List<DBOnlineMirror>();

        /// <summary>
        /// if you compile yourself get you're own key or it will have to be disabled!
        /// </summary>
        public static string cApiKey = string.Empty;

        static DBOnlineMirror()
        {
            s_OnlineToFieldMap.Add("id", cID);
            s_OnlineToFieldMap.Add("interface", cInterface);
            s_OnlineToFieldMap.Add("banners", cBanners);
            s_OnlineToFieldMap.Add("typemask", cTypeMask);

            cApiKey = new System.Resources.ResourceManager("WindowPlugins.GUITVSeries.Online_Parsing_Classes.APIKey", typeof(DBOnlineMirror).Assembly).GetString("Key");
        }

        private DBOnlineMirror()
        { }

        public virtual DBValue this[String fieldName]
        {
            get
            {
                if (m_fields.ContainsKey(fieldName))
                    return m_fields[fieldName].Value;
                else
                    return String.Empty;
            }
            set
            {
                try
                {
                    if (m_fields.ContainsKey(fieldName))
                    {
                        if (m_fields[fieldName].Type == DBField.cTypeInt)
                            m_fields[fieldName].Value = (long)value;
                        else
                            m_fields[fieldName].Value = value;
                    }
                    else
                    {
                        m_fields.Add(fieldName, new DBField(DBField.cTypeString));
                        this[fieldName] = value;
                    }
                }
                catch (SystemException)
                { }
            }
        }

        private static void checkMirrorCapable(List<DBOnlineMirror> mirrorList, TypeMask mask)
        {
            for (int i = 0; i < mirrorList.Count; i++)
            {
                if (mirrorList[i][cTypeMask] == null || (int)mirrorList[i][cTypeMask] < (int)mask)
                    mirrorList.RemoveAt(i--);
            }
        }

        public static void Init()
        {
            if (!DBOption.GetOptions(DBOption.cNewAPIUpgradeDone)) // upgrade
            {
                String sqlDel = "drop table if exists " + cTableName;
                DBTVSeries.Execute(sqlDel);
            }

            if (Helper.String.IsNullOrEmpty(cApiKey))
            {
                MPTVSeriesLog.Write("No APIKey...if you compile yourself you need to register an APIKey at theTVDB.com and add it to the resourceFile, nothing will be downloaded!");
                return;
            }

            // no mirrors yet - refresh using "seed"
            if(!LoadMirrorList(DBOption.GetOptions(DBOption.cMainMirror)))
                MPTVSeriesLog.Write("Warning: No mirrors received, nothing will be downloaded!");

            List<DBOnlineMirror> xmlMirrors = new List<DBOnlineMirror>(memoryMirrors);
            List<DBOnlineMirror> zipMirrors = new List<DBOnlineMirror>(memoryMirrors);
            List<DBOnlineMirror> bannerMirrors = new List<DBOnlineMirror>(memoryMirrors);

            // seperate them by which one can do what
            checkMirrorCapable(xmlMirrors, TypeMask.XML);
            checkMirrorCapable(zipMirrors, TypeMask.Zip);
            checkMirrorCapable(bannerMirrors, TypeMask.Banners);

            // select a random one for each of them
            Random r = new Random();
            if(xmlMirrors.Count > 0)
                s_sCurrentInterface = xmlMirrors[r.Next(0, xmlMirrors.Count - 1)][cMirrorpath];
            if (zipMirrors.Count > 0)
                s_sCurrentZip = zipMirrors[r.Next(0, zipMirrors.Count - 1)][cMirrorpath] + "/";
            if (bannerMirrors.Count > 0)
                s_sCurrentBanner = bannerMirrors[r.Next(0, bannerMirrors.Count - 1)][cMirrorpath] + "/banners/";
        }

        private static bool LoadMirrorList(String sServer)
        {
            XmlNodeList nodeList = Online_Parsing_Classes.OnlineAPI.GetMirrors(appendAPI(sServer, true));
            if (nodeList == null)
                return false;
            int count = 0;
            foreach (XmlNode itemNode in nodeList)
            {
                // create a new OnlineMirror object
                DBOnlineMirror mirror = new DBOnlineMirror();

                foreach (XmlNode propertyNode in itemNode.ChildNodes)
                {
                    if (s_OnlineToFieldMap.ContainsKey(propertyNode.Name))
                        mirror[s_OnlineToFieldMap[propertyNode.Name]] = propertyNode.InnerText;
                    else
                    {
                        mirror[propertyNode.Name] = propertyNode.InnerText;
                    }
                }
                count++;
                memoryMirrors.Add(mirror);
            }
            MPTVSeriesLog.Write("Received " + count.ToString() + " mirrors from " + sServer);
            return true;
        }

        static string appendAPI(string path, bool appendKey)
        {
            return string.Format("{0}/api/{1}", path, appendKey ? (cApiKey + "/") : string.Empty);
        }


        public static String Interface
        {
            get
            {
                initIfNullOrEmpty(s_sCurrentInterface);
                return appendAPI(s_sCurrentInterface, true);
            }
        }

        public static String InterfaceWithoutKey
        {
            get 
            {
                initIfNullOrEmpty(s_sCurrentInterface);
                return appendAPI(s_sCurrentInterface, false);
            }
        }

        public static String Banners
        {
            get
            {
                initIfNullOrEmpty(s_sCurrentBanner);
                return s_sCurrentBanner;
            }
        }

        public static String ZipInterface
        {
            get
            {
                initIfNullOrEmpty(s_sCurrentZip);
                return appendAPI(s_sCurrentZip, true);
            }
        }

        static void initIfNullOrEmpty(string value)
        {
            if (Helper.String.IsNullOrEmpty(value)) Init();
        }
    }
}
