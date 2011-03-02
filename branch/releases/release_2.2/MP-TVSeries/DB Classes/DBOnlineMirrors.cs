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
        [Flags]
        private enum TypeMask
        {
            XML = 0x0001,
            Banners = 0x0002,
            Zip = 0x0004
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
        private static bool m_bMirrorsAvaiable = false;

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
                if (mirrorList[i][cTypeMask] == null || !ContainsTypeMask((TypeMask)(int)mirrorList[i][cTypeMask], mask))                              
                    mirrorList.RemoveAt(i--);
            }
        }

        private static bool ContainsTypeMask(TypeMask combined, TypeMask checkagainst)
        {
            return ((combined & checkagainst) == checkagainst);
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
            IsMirrorsAvailable = true;
            string sMirror = DBOption.GetOptions(DBOption.cMainMirror);
            if (!LoadMirrorList(sMirror))
            {
                IsMirrorsAvailable = false;
                // Try again using the Hardcoded mirror
                if (!sMirror.Equals(DBOption.m_sMainMirror))
                {
                    MPTVSeriesLog.Write("Attempting to retrieve Mirrors from default location");
                    if (LoadMirrorList(DBOption.m_sMainMirror))
                    {   
                        DBOption.SetOptions(DBOption.cMainMirror, DBOption.m_sMainMirror);
                        IsMirrorsAvailable = true;
                    }
                }
                if (!IsMirrorsAvailable)
                {
                    MPTVSeriesLog.Write("Warning: No mirrors received, nothing will be downloaded!");
                    return;
                }
            }      
             
            //This is now handled server-side using mod_rewrite and round-robin DNS,
            //so the mirrors file is somewhat deprecated.

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
                s_sCurrentInterface = xmlMirrors[r.Next(xmlMirrors.Count)][cMirrorpath];
            if (zipMirrors.Count > 0)
                s_sCurrentZip = zipMirrors[r.Next(zipMirrors.Count)][cMirrorpath] + "/";
            if (bannerMirrors.Count > 0)
                s_sCurrentBanner = bannerMirrors[r.Next(bannerMirrors.Count)][cMirrorpath] + "/banners/";
        }

        public static bool IsMirrorsAvailable
        {
            get { return m_bMirrorsAvaiable; }
            set { m_bMirrorsAvaiable = value; }
        }
          
        private static bool LoadMirrorList(String sServer)
        {
            try
            {
                XmlNode node = Online_Parsing_Classes.OnlineAPI.GetMirrors(appendAPI(sServer, true));

                if (node == null)
                    return false;

                int count = 0;
                foreach (XmlNode itemNode in node.ChildNodes)
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
                MPTVSeriesLog.Write("Received " + count.ToString() + " mirror site(s) from " + sServer);
                return true;
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write(string.Format("Error: unable to retrieve list of mirrors online: {0}", ex.Message));
                return false;
            }
        }

        static string appendAPI(string path, bool appendKey)
        {
            return string.Format("{0}api/{1}", (path.EndsWith("/") ? path : (path + "/")), (appendKey ? (cApiKey + "/") : string.Empty));
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
