using System;
using System.Collections.Generic;
using System.Text;
using SQLite.NET;
using MediaPortal.Database;
using System.IO;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    public class DBOnlineSeries : DBTable
    {
        public const String cTableName = "online_series";

        public const String cID = "ID";
        public const String cPrettyName = "Pretty_Name";
        public const String cStatus = "Status";
        public const String cGenre = "Genre";
        public const String cSummary = "Summary";
        public const String cAirsDay = "AirsDay";
        public const String cAirsTime = "AirsTime";
        public const String cActors = "Actors";
        public const String cBannerFileNames = "BannerFileNames";
        public const String cCurrentBannerFileName = "CurrentBannerFileName";
        public const String cBannersDownloaded = "BannersDownloaded";
        public const String cHasLocalFiles = "HasLocalFiles";
        public const String cHasLocalFilesTemp = "HasLocalFiles_Temp";
        public const String cOnlineDataImported = "OnlineDataImported";
        // Online data imported flag values while updating:
        // 0: the series just got an ID, the update series hasn't run on it yet
        // 1: online data is marked as "old", and needs a refresh
        // 2: online data up to date.

        public static Dictionary<String, String> s_FieldToDisplayNameMap = new Dictionary<String, String>();
        public static Dictionary<String, String> s_OnlineToFieldMap = new Dictionary<String, String>();
        public static Dictionary<string, DBField> s_fields = new Dictionary<string, DBField>();


        static DBOnlineSeries()
        {
            s_FieldToDisplayNameMap.Add(cID, "Online Series ID");
            s_FieldToDisplayNameMap.Add(cPrettyName, "Pretty Name");
            s_FieldToDisplayNameMap.Add(cStatus, "Show Status");
            s_FieldToDisplayNameMap.Add(cGenre, "Genre");
            s_FieldToDisplayNameMap.Add(cSummary, "Show Overview");
            s_FieldToDisplayNameMap.Add(cBannerFileNames, "Banner FileName List");
            s_FieldToDisplayNameMap.Add(cCurrentBannerFileName, "Current Banner FileName");
            s_FieldToDisplayNameMap.Add(cAirsDay, "Week Day Aired");
            s_FieldToDisplayNameMap.Add(cAirsTime, "Hour Aired");

            s_OnlineToFieldMap.Add("id", cID);
            s_OnlineToFieldMap.Add("SeriesName", cPrettyName);
            s_OnlineToFieldMap.Add("Status", cStatus);
            s_OnlineToFieldMap.Add("Genre", cGenre);
            s_OnlineToFieldMap.Add("Overview", cSummary);
            s_OnlineToFieldMap.Add("Airs_DayOfWeek", cAirsDay);
            s_OnlineToFieldMap.Add("Airs_Time", cAirsTime);

            // make sure the table is created on first run
            DBOnlineSeries dummy = new DBOnlineSeries();
        }

        public DBOnlineSeries()
            : base(cTableName)
        {
            InitColumns();
            InitValues();
        }

        public DBOnlineSeries(int nSeriesID)
            : base(cTableName)
        {
            InitColumns();
            if (!ReadPrimary(nSeriesID))
                InitValues();
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
            base.AddColumn(cID, new DBField(DBField.cTypeInt, true));
            base.AddColumn(cPrettyName, new DBField(DBField.cTypeString));
            base.AddColumn(cStatus, new DBField(DBField.cTypeString));
            base.AddColumn(cGenre, new DBField(DBField.cTypeString));
            base.AddColumn(cBannerFileNames, new DBField(DBField.cTypeString));
            base.AddColumn(cCurrentBannerFileName, new DBField(DBField.cTypeString));
            base.AddColumn(cSummary, new DBField(DBField.cTypeString));
            base.AddColumn(cOnlineDataImported, new DBField(DBField.cTypeInt));
            base.AddColumn(cAirsDay, new DBField(DBField.cTypeString));
            base.AddColumn(cAirsTime, new DBField(DBField.cTypeString));
            base.AddColumn(cActors, new DBField(DBField.cTypeString));
            base.AddColumn(cBannersDownloaded, new DBField(DBField.cTypeInt));
            base.AddColumn(cHasLocalFiles, new DBField(DBField.cTypeInt));
            base.AddColumn(cHasLocalFilesTemp, new DBField(DBField.cTypeInt));

            foreach (KeyValuePair<String, DBField> pair in m_fields)
            {
                if (!s_fields.ContainsKey(pair.Key))
                    s_fields.Add(pair.Key, pair.Value);
            }
        }

        public override bool AddColumn(string sName, DBField field)
        {
            if (!s_fields.ContainsKey(sName))
            {
                s_fields.Add(sName, field);
                return base.AddColumn(sName, field);
            }
            else
            {
                // we globally know about this key already, so don't call the base
                if (!m_fields.ContainsKey(sName))
                    m_fields.Add(sName, field);
                return false;
            }
        }

        public static new String Q(String sField)
        {
            return cTableName + "." + sField;
        }

        public static void Clear(SQLCondition conditions)
        {
            Clear(new DBOnlineSeries(), conditions);
        }
    };

    public class DBSeries : DBTable
    {
        public const String cTableName = "local_series";
        public const String cOutName = "Series";

        public const String cParsedName = "Parsed_Name";
        public const String cID = "ID";
        public const String cScanIgnore = "ScanIgnore";
        public const String cDuplicateLocalName = "DuplicateLocalName";

        private DBOnlineSeries m_onlineSeries = null;

        public static Dictionary<String, String> s_FieldToDisplayNameMap = new Dictionary<String, String>();
        static int s_nLastLocalID;

        static DBSeries()
        {
            s_nLastLocalID = DBOption.GetOptions(DBOption.cDBSeriesLastLocalID);

            s_FieldToDisplayNameMap.Add(cParsedName, "Parsed Name");

            int nCurrentDBSeriesVersion = 3;
            while (DBOption.GetOptions(DBOption.cDBSeriesVersion) != nCurrentDBSeriesVersion)
            // take care of the upgrade in the table
            switch ((int)DBOption.GetOptions(DBOption.cDBSeriesVersion))
            {
                case 1:
                case 2:
                    // upgrade to version 3; clear the series table (we use 2 other tables now)
                    try
                    {
                        String sqlQuery = "DROP TABLE series";
                        DBTVSeries.Execute(sqlQuery);
                        DBOption.SetOptions(DBOption.cDBSeriesVersion, nCurrentDBSeriesVersion);
                    }
                    catch {}
                    break;

                default:
                    break;
            }

            // make sure the table is created on first run
            DBSeries dummy = new DBSeries();
        }

        public static String PrettyFieldName(String sFieldName)
        {
            if (s_FieldToDisplayNameMap.ContainsKey(sFieldName))
                return s_FieldToDisplayNameMap[sFieldName];
            else
                return sFieldName;
        }

        public DBSeries()
            : base(cTableName)
        {
            InitColumns();
            InitValues();
        }

        public DBSeries(bool bCreateEmptyOnline)
            : base(cTableName)
        {
            InitColumns();
            InitValues();
            if (bCreateEmptyOnline)
                m_onlineSeries = new DBOnlineSeries();
        }

        public DBSeries(String SeriesName)
            : base(cTableName)
        {
            InitColumns();
            if (!ReadPrimary(SeriesName))
                InitValues();
            if (base[cID] == 0)
            {
                m_onlineSeries = new DBOnlineSeries(s_nLastLocalID);
                s_nLastLocalID--;
                DBOption.SetOptions(DBOption.cDBSeriesLastLocalID, s_nLastLocalID);
                base[cID] = m_onlineSeries[DBOnlineSeries.cID];
            }
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
            base.AddColumn(cParsedName, new DBField(DBField.cTypeString, true));
            base.AddColumn(cID, new DBField(DBField.cTypeInt));
            base.AddColumn(cScanIgnore, new DBField(DBField.cTypeInt));
            base.AddColumn(cDuplicateLocalName, new DBField(DBField.cTypeInt));
        }

        public static new String Q(String sField)
        {
            return cTableName + "." + sField;
        }

        public override bool AddColumn(string sName, DBField field)
        {
            // can't add columns to 
            if (m_onlineSeries != null)
                return m_onlineSeries.AddColumn(sName, field);
            else
                return false;
        }

        public DBOnlineSeries onlineSeries
        {
            get { return m_onlineSeries; }
        }

        public void ChangeSeriesID(int nSeriesID)
        {
            m_onlineSeries = new DBOnlineSeries(nSeriesID);
            base[cID] = nSeriesID;
        }

        public override List<String> FieldNames
        {
            get
            {
                List<String> outList = new List<String>();
                // some ordering: I want some fields to come up first
                foreach (KeyValuePair<string, DBField> pair in m_fields)
                {
                    if (outList.IndexOf(pair.Key) == -1)
                        outList.Add(pair.Key);
                }
                if (m_onlineSeries != null)
                {
                    foreach (KeyValuePair<string, DBField> pair in m_onlineSeries.m_fields)
                    {
                        if (outList.IndexOf(pair.Key) == -1)
                            outList.Add(pair.Key);
                    }
                }

                return outList;
            }
        }
        // function override to search on both this & the onlineEpisode
        public override DBValue this[String fieldName]
        {
            get
            {
                switch (fieldName)
                {
                    case DBOnlineSeries.cPrettyName:
                        DBValue retVal = null;
                        if (m_onlineSeries != null)
                            retVal = m_onlineSeries[DBOnlineSeries.cPrettyName];

                        if (retVal == null || retVal == String.Empty)
                            retVal = base[cParsedName];
                        return retVal;

                    case cParsedName:
                    case cScanIgnore:
                    case cDuplicateLocalName:
                        return base[fieldName];


                    default:
                        if (m_onlineSeries != null)
                            return m_onlineSeries[fieldName];
                        else
                            return base[fieldName];
                }
            }

            set
            {
                switch (fieldName)
                {
                    case cScanIgnore:
                    case cDuplicateLocalName:
                        base[fieldName] = value;
                        break;

                    case cID:
                        base[fieldName] = value;
                        if (m_onlineSeries != null)
                            m_onlineSeries[fieldName] = value;
                        break;

                    default:
                        if (m_onlineSeries != null)
                            m_onlineSeries[fieldName] = value;
                        break;
                }
            }
        }

        public String Banner
        {
            get
            {
                if (m_onlineSeries != null)
                {
                    if (m_onlineSeries[DBOnlineSeries.cCurrentBannerFileName] == String.Empty)
                        return String.Empty;

                    if (m_onlineSeries[DBOnlineSeries.cCurrentBannerFileName].ToString().IndexOf(Directory.GetDirectoryRoot(m_onlineSeries[DBOnlineSeries.cCurrentBannerFileName])) == -1)
                        return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\banners\" + m_onlineSeries[DBOnlineSeries.cCurrentBannerFileName];
                    else
                        return m_onlineSeries[DBOnlineSeries.cCurrentBannerFileName];
                }
                else
                    return String.Empty;
            }
            set
            {
                if (m_onlineSeries != null)
                    m_onlineSeries[DBOnlineSeries.cCurrentBannerFileName] = value;
            }
        }

        public List<String> BannerList
        {
            get
            {
                List<String> outList = new List<string>();
                if (m_onlineSeries != null)
                {
                    String sList = m_onlineSeries[DBOnlineSeries.cBannerFileNames];
                    if (sList == String.Empty)
                        return outList;

                    String[] split = sList.Split(new char[] { '|' });
                    foreach (String filename in split)
                    {
                        if (filename.IndexOf(Directory.GetDirectoryRoot(filename)) == -1)
                            outList.Add(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\banners\" + filename);
                        else
                            outList.Add(filename);
                    }
                }
                return outList;
            }
            set
            {
                if (m_onlineSeries != null)
                {
                    String sIn = String.Empty;
                    foreach (String filename in value)
                    {
                        if (sIn == String.Empty)
                            sIn += filename;
                        else
                            sIn += "," + filename;
                    }
                    m_onlineSeries[DBOnlineSeries.cBannerFileNames] = sIn;
                }
            }
        }

        public override bool Commit()
        {
            if (m_onlineSeries != null)
                m_onlineSeries.Commit();

            return base.Commit();
        }

        public static void Clear(SQLCondition conditions)
        {
            Clear(new DBSeries(), conditions);
        }

        public static void GlobalSet(String sKey, DBValue Value)
        {
            GlobalSet(sKey, Value, new SQLCondition());
        }

        public static void GlobalSet(String sKey, DBValue Value, SQLCondition condition)
        {
            GlobalSet(new DBSeries(), sKey, Value, condition);
            GlobalSet(new DBOnlineSeries(), sKey, Value, condition);
        }

        public static void GlobalSet(String sKey1, String sKey2)
        {
            GlobalSet(sKey1, sKey2, new SQLCondition());
        }

        public static void GlobalSet(String sKey1, String sKey2, SQLCondition condition)
        {
            GlobalSet(new DBSeries(), sKey1, sKey2, condition);
            GlobalSet(new DBOnlineSeries(), sKey1, sKey2, condition);
        }

        public static List<DBSeries> Get(bool bExistingFilesOnly)
        {
            if (bExistingFilesOnly)
            {
                SQLCondition condition = new SQLCondition();
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cHasLocalFiles, 0, SQLConditionType.NotEqual);
                return Get(condition);
            }
            else
            {
                SQLWhat what = new SQLWhat(new DBOnlineSeries());
                what.AddWhat(new DBSeries());
                String sqlQuery = "select " + what + " left join " + cTableName + " on " + DBSeries.Q(cID) + "==" + DBOnlineSeries.Q(cID) + " order by " + DBSeries.Q(cParsedName);
                return Get(sqlQuery);
            }
        }

        public static List<DBSeries> Get(SQLCondition conditions)
        {
            SQLWhat what = new SQLWhat(new DBOnlineSeries());
            what.AddWhat(new DBSeries());
            String sqlQuery = String.Empty;
            sqlQuery = "select " + what + " left join " + cTableName + " on " + DBSeries.Q(cID) + "==" + DBOnlineSeries.Q(cID) + " where " + conditions + " order by " + DBSeries.Q(cParsedName);
            return Get(sqlQuery);
        }

        private static List<DBSeries> Get(String sqlQuery)
        {
            SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
            List<DBSeries> outList = new List<DBSeries>();
            if (results.Rows.Count > 0)
            {
                for (int index = 0; index < results.Rows.Count; index++)
                {
                    DBSeries series = new DBSeries();
                    series.Read(ref results, index);
                    series.m_onlineSeries = new DBOnlineSeries();
                    series.m_onlineSeries.Read(ref results, index);
                    outList.Add(series);
                }
            }
            return outList;
        }
    }
}
