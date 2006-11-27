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
            List<DBOnlineMirror> mirrorList = Get();
            if (mirrorList.Count == 0)
            {
                // no mirrors yet - refresh using "seed"
                LoadMirrorList("http://tvdb.zsori.com/interfaces");
                mirrorList = Get();
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
                    break;
                }
            }
        }

        private static bool LoadMirrorList(String sServer)
        {
            XmlNodeList nodeList = ZsoriParser.GetMirrors(sServer);
            if (nodeList == null)
                return false;

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

                mirror.Commit();
            }
            return true;
        }

        public static String Interface
        {
            get 
            {
                if (s_sCurrentInterface == String.Empty)
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
                if (s_sCurrentBanner == String.Empty)
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
