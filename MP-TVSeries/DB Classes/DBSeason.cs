using System;
using System.Collections.Generic;
using System.Text;
using SQLite.NET;
using MediaPortal.Database;

namespace WindowPlugins.GUITVSeries
{
    public class DBSeason : DBTable
    {
        public const String cTableName = "season";

        public const String cID = "ID"; // local name, unique (it's the primary key) which is a composite of the series name & the season index
        public const String cSeriesName = DBSeries.cParsedName;
        public const String cIndex = "SeasonIndex";
        public const String cBannerFileName = "BannerFileName";

        public static Dictionary<String, String> s_FieldToDisplayNameMap = new Dictionary<String, String>();

        static DBSeason()
        {
            s_FieldToDisplayNameMap.Add(cID, "Composite Season ID");
            s_FieldToDisplayNameMap.Add(cSeriesName, "Series Parsed Name");
            s_FieldToDisplayNameMap.Add(cIndex, "Season Index");
            s_FieldToDisplayNameMap.Add(cBannerFileName, "Banner FileName");
        }

        public static String PrettyFieldName(String sFieldName)
        {
            if (s_FieldToDisplayNameMap.ContainsKey(sFieldName))
                return s_FieldToDisplayNameMap[sFieldName];
            else
                return sFieldName;
        }

        public DBSeason()
            : base(cTableName)
        {
            InitColumns();
            InitValues();
            // all available fields
        }

        public DBSeason(String SeriesName, int nSeasonIndex)
            : base(cTableName)
        {
            InitColumns();
            String sSeasonID = SeriesName + "_s" + nSeasonIndex;
            if (!ReadPrimary(sSeasonID))
            {
                InitValues();
            }
            this[cSeriesName] = SeriesName;
            this[cIndex] = nSeasonIndex;
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
            AddColumn(cID, new DBField(DBField.cTypeString, true));
            AddColumn(cSeriesName, new DBField(DBField.cTypeString));
            AddColumn(cIndex, new DBField(DBField.cTypeInt));
            AddColumn(cBannerFileName, new DBField(DBField.cTypeString));
        }

        public static List<DBSeason> Get(String sSeriesName)
        {
            // create table if it doesn't exist already
            SQLCondition condition = new SQLCondition(new DBSeason());
            condition.Add(cSeriesName, sSeriesName, true);
            String sqlQuery = "select * from " + cTableName + " where " + condition + " order by " + cIndex;
            SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
            List<DBSeason> outList = new List<DBSeason>();
            if (results.Rows.Count > 0)
            {
                for (int index = 0; index < results.Rows.Count; index++)
                {
                    DBSeason season = new DBSeason();
                    season.Read(ref results, index);
                    outList.Add(season);
                }
            }
            return outList;
        }
    }
}
