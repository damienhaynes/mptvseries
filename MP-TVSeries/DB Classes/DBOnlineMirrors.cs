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
        public const String cTableName = "OnlineMirrors";

        public const String cID = "ID";
        public const String cInterface = "Interface";
        public const String cBanners = "Banners";

        private static String s_sCurrentInterface = String.Empty;
        private static String s_sCurrentBanner = String.Empty;
        private static Dictionary<String, String> s_OnlineToFieldMap = new Dictionary<String, String>();

        static DBOnlineMirror()
        {
            s_OnlineToFieldMap.Add("id", cID);
            s_OnlineToFieldMap.Add("interface", cInterface);
            s_OnlineToFieldMap.Add("banners", cBanners);

            // make sure the table is created on first run
            DBOnlineMirror dummy = new DBOnlineMirror();
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
            AddColumn(cBanners, new DBField(DBField.cTypeString));
            AddColumn(cInterface, new DBField(DBField.cTypeString));
        }

        public static void Init()
        {
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

        private static bool LoadMirrorList(String sServer)
        {
            XmlNodeList nodeList = ZsoriParser.GetMirrors(sServer);
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

        public static String Interface
        {
            get 
            {
                if (Helper.String.IsNullOrEmpty(s_sCurrentInterface))
                {
                    Init();
                }

                return s_sCurrentInterface;

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
