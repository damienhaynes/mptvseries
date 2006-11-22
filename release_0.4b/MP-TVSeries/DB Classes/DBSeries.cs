using System;
using System.Collections.Generic;
using System.Text;
using SQLite.NET;
using MediaPortal.Database;
using System.IO;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    public class DBSeries : DBTable
    {
        public const String cTableName = "series";
        public const String cOutName = "Series";

        public const String cParsedName = "Parsed_Name";
        public const String cID = "ID";
        public const String cPrettyName = "Pretty_Name";
        public const String cStatus = "Status";
        public const String cGenre = "Genre";
        public const String cSummary = "Summary";
        public const String cAirsDay = "AirsDay";
        public const String cAirsTime = "AirsTime";
        public const String cBannerFileNames = "BannerFileNames";
        public const String cCurrentBannerFileName = "CurrentBannerFileName";
        public const String cHasLocalFiles = "HasLocalFiles";
        public const String cHasLocalFilesTemp = "HasLocalFiles_Temp";
        public const String cOnlineDataImported = "OnlineDataImported";
        public const String cBannersDownloaded = "BannersDownloaded";
        public const String cActors = "Actors";
        // Online data imported flag values while updating:
        // 0: the series just got an ID, the update series hasn't run on it yet
        // 1: online data is marked as "old", and needs a refresh
        // 2: online data up to date.

        public static Dictionary<String, String> s_FieldToDisplayNameMap = new Dictionary<String, String>();
        public static Dictionary<String, String> s_OnlineToFieldMap = new Dictionary<String, String>();

        static DBSeries()
        {
            s_FieldToDisplayNameMap.Add(cParsedName, "Parsed Name");
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
            DBSeries dummy = new DBSeries();

            int nCurrentDBSeriesVersion = 2;
            while (DBOption.GetOptions(DBOption.cDBSeriesVersion) != nCurrentDBSeriesVersion)
            // take care of the upgrade in the table
            switch ((int)DBOption.GetOptions(DBOption.cDBSeriesVersion))
            {
                case 1:
                    // upgrade to version 2; do the necessary upgrades (copy Airs_DayOfWeek into cAirsDay and Airs_Time into cAirsTime)
                    String sqlQuery = "update " + cTableName + " set " + cAirsDay + " = Airs_DayOfWeek, " + cAirsTime + " = Airs_Time";
                    DBTVSeries.Execute(sqlQuery);
                    // do you believe what a mess it is to DROP a column in sqlite ... ssshhh

                    // we have the new column list, start the query
                    String sCreateFieldList = String.Empty;
                    String sFieldList = String.Empty;

                    Dictionary<String, DBField> fields = dummy.m_fields;
                    fields.Remove("Airs_DayOfWeek");
                    fields.Remove("Airs_Time");
                    foreach (KeyValuePair<String, DBField> pair in dummy.m_fields)
                    {
                        if (sCreateFieldList != String.Empty)
                            sCreateFieldList += ",";
                        if (sFieldList != String.Empty)
                            sFieldList += ",";
                        sCreateFieldList += pair.Key + " " + pair.Value.Type + (pair.Value.Primary ? " primary key" : "");
                        sFieldList += pair.Key;
                    }

                    sqlQuery = "CREATE TEMPORARY TABLE t_backup(" + sCreateFieldList + ");";
                    DBTVSeries.Execute(sqlQuery);
                    sqlQuery = "INSERT INTO t_backup SELECT " + sFieldList + " FROM " + cTableName + ";";
                    DBTVSeries.Execute(sqlQuery);
                    sqlQuery = "DROP TABLE " + cTableName + ";";
                    DBTVSeries.Execute(sqlQuery);
                    sqlQuery = "CREATE TABLE " + cTableName + "(" + sCreateFieldList + ");";
                    DBTVSeries.Execute(sqlQuery);
                    sqlQuery = "INSERT INTO " + cTableName + " SELECT " + sFieldList + " FROM t_backup;";
                    DBTVSeries.Execute(sqlQuery);
                    sqlQuery = "DROP TABLE t_backup;";
                    DBTVSeries.Execute(sqlQuery);
                    DBOption.SetOptions(DBOption.cDBSeriesVersion, 2);
                    break;

                default:
                    break;
            }
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

        public DBSeries(String SeriesName)
            : base(cTableName)
        {
            InitColumns();
            if (!ReadPrimary(SeriesName))
            {
                InitValues();
            }
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
            AddColumn(cParsedName, new DBField(DBField.cTypeString, true));
            AddColumn(cID, new DBField(DBField.cTypeInt));
            AddColumn(cPrettyName, new DBField(DBField.cTypeString));
            AddColumn(cStatus, new DBField(DBField.cTypeString));
            AddColumn(cGenre, new DBField(DBField.cTypeString));
            AddColumn(cBannerFileNames, new DBField(DBField.cTypeString));
            AddColumn(cCurrentBannerFileName, new DBField(DBField.cTypeString));
            AddColumn(cSummary, new DBField(DBField.cTypeString));
            AddColumn(cOnlineDataImported, new DBField(DBField.cTypeInt));
            AddColumn(cBannersDownloaded, new DBField(DBField.cTypeInt));
            AddColumn(cHasLocalFiles, new DBField(DBField.cTypeInt));
            AddColumn(cHasLocalFilesTemp, new DBField(DBField.cTypeInt));
            AddColumn(cAirsDay, new DBField(DBField.cTypeString));
            AddColumn(cAirsTime, new DBField(DBField.cTypeString));
            AddColumn(cActors, new DBField(DBField.cTypeString));
        }

        // function override to search on both this & the onlineEpisode
        public override DBValue this[String fieldName]
        {
            get
            {
                switch (fieldName)
                {
                    case cPrettyName:
                        DBValue retVal = base[cPrettyName];
                        if (retVal == null || retVal == String.Empty)
                            retVal = base[cParsedName];
                        return retVal;

                    default:
                        return base[fieldName];
                }
            }

            set
            {
                base[fieldName] = value;
            }
        }

        public String Banner
        {
            get
            {
                if (base[cCurrentBannerFileName] == String.Empty)
                    return String.Empty;

                if (base[cCurrentBannerFileName].ToString().IndexOf(Directory.GetDirectoryRoot(base[cCurrentBannerFileName])) == -1)
                    return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\banners\" + base[cCurrentBannerFileName];
                else
                    return base[cCurrentBannerFileName];
            }
            set
            {
                base[cCurrentBannerFileName] = value;
            }
        }

        public List<String> BannerList
        {
            get
            {
                List<String> outList = new List<string>();
                String sList = base[cBannerFileNames];
                if (sList == String.Empty)
                    return outList;

                String[] split = sList.Split(new char[]{'|'});
                foreach (String filename in split)
                {
                    if (filename.IndexOf(Directory.GetDirectoryRoot(filename)) == -1)
                        outList.Add(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\banners\" + filename);
                    else
                        outList.Add(filename);
                }
                return outList;
            }
            set
            {
                String sIn = String.Empty;
                foreach (String filename in value)
                {
                    if (sIn == String.Empty)
                        sIn += filename;
                    else
                        sIn += "," + filename;
                }
                base[cBannerFileNames] = sIn;

            }
        }

        public static void Clear(SQLCondition conditions)
        {
            Clear(new DBSeries(), conditions);
        }

        public static void GlobalSet(String sKey, DBValue Value)
        {
            GlobalSet(sKey, Value, new SQLCondition(new DBSeries()));
        }

        public static void GlobalSet(String sKey, DBValue Value, SQLCondition condition)
        {
            GlobalSet(new DBSeries(), sKey, Value, condition);
        }

        public static void GlobalSet(String sKey1, String sKey2)
        {
            GlobalSet(sKey1, sKey2, new SQLCondition(new DBSeries()));
        }

        public static void GlobalSet(String sKey1, String sKey2, SQLCondition condition)
        {
            GlobalSet(new DBSeries(), sKey1, sKey2, condition);
        }

        public static List<DBSeries> Get(bool bExistingFilesOnly)
        {
            if (bExistingFilesOnly)
            {
                SQLCondition condition = new SQLCondition(new DBSeries());
                condition.Add(cHasLocalFiles, 0, false);
                return Get("select * from " + cTableName + " where " + condition + " order by " + cParsedName);
            }
            else
            {
                return Get("select * from " + cTableName + " order by " + cParsedName);
            }
        }

        public static List<DBSeries> Get(SQLCondition condition)
        {
            String sqlQuery = "select * from " + cTableName + " where " + condition + " order by " + cParsedName;
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
                    outList.Add(series);
                }
            }
            return outList;
        }
    }
}
