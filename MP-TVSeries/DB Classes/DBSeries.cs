using System;
using System.Collections.Generic;
using System.Text;
using SQLite.NET;
using MediaPortal.Database;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    public class DBSeries : DBTable
    {
        public const String cTableName = "series";

        public const String cParsedName = "Parsed_Name";
        public const String cID = "ID";
        public const String cPrettyName = "Pretty_Name";
        public const String cStatus = "Status";
        public const String cGenre = "Genre";
        public const String cSummary = "Summary";
        public const String cBannerFileName = "BannerFileName";

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

            s_OnlineToFieldMap.Add("id", cID);
            s_OnlineToFieldMap.Add("SeriesName", cPrettyName);
            s_OnlineToFieldMap.Add("Status", cStatus);
            s_OnlineToFieldMap.Add("Genre", cGenre);
            s_OnlineToFieldMap.Add("Overview", cSummary);

            // make sure the table is created on first run
            DBSeries dummy = new DBSeries();
        }

        static String PrettyFieldName(String sFieldName)
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
            AddColumn(cBannerFileName, new DBField(DBField.cTypeString));
            AddColumn(cSummary, new DBField(DBField.cTypeString));
        }

        public String Label(String fieldName)
        {
            if (s_FieldToDisplayNameMap.ContainsKey(fieldName))
                return s_FieldToDisplayNameMap[fieldName];
            else
                return fieldName;
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

        public static List<DBSeries> Get()
        {
            String sqlQuery = "select * from " + cTableName + " order by " + cParsedName;
            return Get(sqlQuery);
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
