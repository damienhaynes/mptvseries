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
        readonly Dictionary<string, DBField> mfields = new Dictionary<string, DBField>();

        private static String mCurrentInterface = String.Empty;
        private static String mCurrentBanner = String.Empty;
        private static String mCurrentZip = String.Empty;
        private static readonly Dictionary<String, String> mOnlineToFieldMap = new Dictionary<String, String>();
        private static readonly List<DBOnlineMirror> mMemoryMirrors = new List<DBOnlineMirror>();

        /// <summary>
        /// if you compile yourself get you're own key or it will have to be disabled!
        /// </summary>
        public static string cApiKey = string.Empty;
        private static bool mMirrorsAvailable = false;

        static DBOnlineMirror()
        {
            mOnlineToFieldMap.Add("id", cID);
            mOnlineToFieldMap.Add("interface", cInterface);
            mOnlineToFieldMap.Add("banners", cBanners);
            mOnlineToFieldMap.Add("typemask", cTypeMask);

            cApiKey = new System.Resources.ResourceManager("WindowPlugins.GUITVSeries.Online_Parsing_Classes.APIKey", typeof(DBOnlineMirror).Assembly).GetString("Key");
        }

        private DBOnlineMirror()
        { }

        public virtual DBValue this[String fieldName]
        {
            get
            {
                if (mfields.ContainsKey(fieldName))
                    return mfields[fieldName].Value;
                else
                    return String.Empty;
            }
            set
            {
                try
                {
                    if (mfields.ContainsKey(fieldName))
                    {
                        if (mfields[fieldName].Type == DBField.cTypeInt)
                            mfields[fieldName].Value = (long)value;
                        else
                            mfields[fieldName].Value = value;
                    }
                    else
                    {
                        mfields.Add(fieldName, new DBField(DBField.cTypeString));
                        this[fieldName] = value;
                    }
                }
                catch (SystemException)
                { }
            }
        }

        private static void CheckMirrorCapable(List<DBOnlineMirror> mirrorList, TypeMask mask)
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

            if (String.IsNullOrEmpty(cApiKey))
            {
                MPTVSeriesLog.Write("No APIKey...if you compile yourself you need to register an APIKey at theTVDB.com and add it to the resourceFile, nothing will be downloaded!");
                return;
            }

            // no mirrors yet - refresh using "seed"
            IsMirrorsAvailable = true;
            string sMirror = DBOption.GetOptions(DBOption.cMainMirror).ToString().Replace("http://", "https://");
            if (!LoadMirrorList(sMirror))
            {
                IsMirrorsAvailable = false;
                // Try again using the Hardcoded mirror
                if (!sMirror.Equals(DBOption.cMainMirrorHardCoded))
                {
                    MPTVSeriesLog.Write("Attempting to retrieve Mirrors from default location");
                    if (LoadMirrorList(DBOption.cMainMirrorHardCoded))
                    {   
                        DBOption.SetOptions(DBOption.cMainMirror, DBOption.cMainMirrorHardCoded);
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

            var xmlMirrors = new List<DBOnlineMirror>(mMemoryMirrors);
            var zipMirrors = new List<DBOnlineMirror>(mMemoryMirrors);
            var bannerMirrors = new List<DBOnlineMirror>(mMemoryMirrors);

            // seperate them by which one can do what
            CheckMirrorCapable(xmlMirrors, TypeMask.XML);
            CheckMirrorCapable(zipMirrors, TypeMask.Zip);
            CheckMirrorCapable(bannerMirrors, TypeMask.Banners);

            // select a random one for each of them
            var r = new Random();
            if(xmlMirrors.Count > 0)
                mCurrentInterface = xmlMirrors[r.Next(xmlMirrors.Count)][cMirrorpath].ToString().Replace("http://", "https://");
            if (zipMirrors.Count > 0)
                mCurrentZip = zipMirrors[r.Next(zipMirrors.Count)][cMirrorpath].ToString().Replace("http://", "https://") + "/";
            if (bannerMirrors.Count > 0)
                mCurrentBanner = bannerMirrors[r.Next(bannerMirrors.Count)][cMirrorpath].ToString().Replace("http://", "https://") + "/banners/";
        }

        public static bool IsMirrorsAvailable
        {
            get { return mMirrorsAvailable; }
            set { mMirrorsAvailable = value; }
        }
          
        private static bool LoadMirrorList(String sServer)
        {
            try
            {
                XmlNode node = Online_Parsing_Classes.OnlineAPI.GetMirrors(AppendAPI(sServer, true));

                if (node == null)
                    return false;

                int count = 0;
                foreach (XmlNode itemNode in node.ChildNodes)
                {
                    // create a new OnlineMirror object
                    var mirror = new DBOnlineMirror();

                    foreach (XmlNode propertyNode in itemNode.ChildNodes)
                    {
                        if (mOnlineToFieldMap.ContainsKey(propertyNode.Name))
                            mirror[mOnlineToFieldMap[propertyNode.Name]] = propertyNode.InnerText;
                        else
                        {
                            mirror[propertyNode.Name] = propertyNode.InnerText;
                        }
                    }
                    count++;
                    mMemoryMirrors.Add(mirror);
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

        static string AppendAPI(string path, bool appendKey)
        {
            return string.Format("{0}api/{1}", (path.EndsWith("/") ? path : (path + "/")), (appendKey ? (cApiKey + "/") : string.Empty));
        }


        public static String Interface
        {
            get
            {
                InitIfNullOrEmpty(mCurrentInterface);
                return AppendAPI(mCurrentInterface, true);
            }
        }

        public static String InterfaceWithoutKey
        {
            get 
            {
                InitIfNullOrEmpty(mCurrentInterface);
                return AppendAPI(mCurrentInterface, false);
            }
        }

        public static String Banners
        {
            get
            {
                InitIfNullOrEmpty(mCurrentBanner);
                return mCurrentBanner;
            }
        }

        public static String ZipInterface
        {
            get
            {
                InitIfNullOrEmpty(mCurrentZip);
                return AppendAPI(mCurrentZip, true);
            }
        }

        static void InitIfNullOrEmpty(string value)
        {
            if (String.IsNullOrEmpty(value)) Init();
        }
    }
}
