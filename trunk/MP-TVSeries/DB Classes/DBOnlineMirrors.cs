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
    class DBOnlineMirror : DBTable
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

        private static String s_sCurrentInterface = String.Empty;
        private static String s_sCurrentBanner = String.Empty;
        private static String s_sCurrentZip = String.Empty;
        private static Dictionary<String, String> s_OnlineToFieldMap = new Dictionary<String, String>();

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

            // make sure the table is created on first run
            DBOnlineMirror dummy = new DBOnlineMirror();

            cApiKey = new System.Resources.ResourceManager("WindowPlugins.GUITVSeries.Online_Parsing_Classes.APIKey", typeof(DBOnlineMirror).Assembly).GetString("APIKey");
        }

        private DBOnlineMirror()
            : base(cTableName)
        {
            InitColumns();
            InitValues();
        }

        private DBOnlineMirror(int nID)
            : base(cTableName)
        {
            InitColumns();
            if (!ReadPrimary(nID))
            {
                InitValues();
            }
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
            AddColumn(cID, new DBField(DBField.cTypeInt, true));
            //AddColumn(cBanners, new DBField(DBField.cTypeString));
            //AddColumn(cInterface, new DBField(DBField.cTypeString));
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
            if (Settings.newAPI && !DBOption.GetOptions(DBOption.cNewAPIUpgradeDone)) // upgrade
            {
                String sqlDel = "drop table " + cTableName;
            }

            if (Helper.String.IsNullOrEmpty(cApiKey))
            {
                MPTVSeriesLog.Write("No APIKey...if you compile yourself you need to register an APIKey at theTVDB.com and add it to the resourceFile, nothing will be downloaded!");
                return;
            }

            // TODO: improve mirrorhandling
            List<DBOnlineMirror> mirrorList = Get();
            bool startEmpty = false;
            if (mirrorList.Count == 0)
            {
                // no mirrors yet - refresh using "seed"
                LoadMirrorList(DBOption.GetOptions(DBOption.cMainMirror));
                mirrorList = Get();
                startEmpty = true;
            }

            if (Settings.newAPI)
            {
                List<DBOnlineMirror> xmlMirrors = new List<DBOnlineMirror>(mirrorList);
                List<DBOnlineMirror> zipMirrors = new List<DBOnlineMirror>(mirrorList);
                List<DBOnlineMirror> bannerMirrors = new List<DBOnlineMirror>(mirrorList);

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
            else
            {
                // choose first interface as what's returned by the server is already randomized
                for (int index = 0; index < mirrorList.Count; index++)
                {
                    // use the mirror to check for valid output
                    if (LoadMirrorList(mirrorList[index][cInterface]))
                    {
                        // valid data, let's use that one
                        s_sCurrentInterface = mirrorList[index][cInterface];
                        s_sCurrentBanner = mirrorList[index][cBanners];
                        return;
                    }
                    else
                    {
                        // its no good, delete it from our cache
                        SQLCondition cond = new SQLCondition();
                        cond.Add(new DBOnlineMirror(), DBOnlineMirror.cInterface, mirrorList[index][cInterface], SQLConditionType.Equal);
                        DBOnlineMirror.Clear(new DBOnlineMirror(), cond);
                    }
                }
                if (startEmpty)
                {
                    // ok, we requeried the manual main mirror, and none of its reported mirrors were any good
                    // last option is to use it directly (the main mirror itself must be good or this code would not be reached, and it must not have reported itself as a mirror)

                    s_sCurrentInterface = DBOption.GetOptions(DBOption.cMainMirror);
                    s_sCurrentBanner = "none"; // no banner mirror though! not string.empty to prevent re-init everytime
                    MPTVSeriesLog.Write("Could not connect to any mirrors, using Main Mirror direclty as backup! (no banners will be downloaded!)");
                }
                else
                {
                    // oops, if we are here no mirror in our list was good, try main mirror (perhaps it was changed and thus isnt in this list yet)
                    // note: this only queries the manually entered main servers and asks it for a list of mirrors, this list may or may not contain this main mirror
                    if (LoadMirrorList(DBOption.GetOptions(DBOption.cMainMirror)))
                    {
                        // and test again (we delete all from the db so recursion should be save)
                        Init();
                    }
                    else
                    {
                        MPTVSeriesLog.Write("Could not connect to any mirrors, please check your internet connection!");
                    }
                }
            }
        }

        private static bool LoadMirrorList(String sServer)
        {
            XmlNodeList nodeList;
            if (Settings.newAPI) nodeList = Online_Parsing_Classes.OnlineAPI.GetMirrors(appendAPI(sServer, true));
            else nodeList = ZsoriParser.GetMirrors(sServer);
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
                        // we don't know that field, add it to the series table
                        mirror.AddColumn(propertyNode.Name, new DBField(DBField.cTypeString));
                        mirror[propertyNode.Name] = propertyNode.InnerText;
                    }
                }
                count++;
                mirror.Commit();
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
                if (Helper.String.IsNullOrEmpty(s_sCurrentInterface))
                {
                    Init();
                }

                if (Settings.newAPI) return appendAPI(s_sCurrentInterface, true);
                else return s_sCurrentInterface;

            }
        }

        public static String InterfaceWithoutKey
        {
            get 
            {
                if (Helper.String.IsNullOrEmpty(s_sCurrentInterface))
                {
                    Init();
                }

                if (Settings.newAPI) return appendAPI(s_sCurrentInterface, false);
                else return s_sCurrentInterface;

            }
        }

        public static String Banners
        {
            get
            {
                if (Helper.String.IsNullOrEmpty(s_sCurrentBanner))
                {
                    Init();
                }

                return s_sCurrentBanner;
            }
        }

        public static String ZipInterface
        {
            get
            {
                if (Helper.String.IsNullOrEmpty(s_sCurrentZip))
                {
                    Init();
                }

                return appendAPI(s_sCurrentZip, true);
            }
        }

        private static List<DBOnlineMirror> Get()
        {
            String sqlQuery = "select * from " + cTableName + " order by " + cID;
            SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
            List<DBOnlineMirror> outList = new List<DBOnlineMirror>();
            if (results.Rows.Count > 0)
            {
                for (int index = 0; index < results.Rows.Count; index++)
                {
                    DBOnlineMirror mirror = new DBOnlineMirror();
                    mirror.Read(ref results, index);
                    outList.Add(mirror);
                }
            }
            return outList;
        }
    }
}
