using System;
using System.Collections.Generic;
using System.Text;
using SQLite.NET;
using MediaPortal.Database;

namespace WindowPlugins.GUITVSeries
{
    public class DBSeries : DBTable
    {
        public const String cTableName = "series";

        public const String cParsedName = "Parsed_Name";
        public const String cID = "ID";
        public const String cPrettyName = "Pretty_Name";
        public const String cStatus = "Status";
        public const String cFirstAired = "First_Aired";
        public const String cGenre = "Genre";
        public const String cBannerFileName = "BannerFileName";
        public const String cSummary = "Summary";

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

        private void InitValues()
        {
            this[cID] = 0;
            this[cPrettyName] = String.Empty;
            this[cStatus] = String.Empty;
            this[cFirstAired] = String.Empty;
            this[cGenre] = String.Empty;
            this[cBannerFileName] = String.Empty;
            this[cSummary] = String.Empty;
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
            AddColumn(cParsedName, new DBField(DBField.cTypeString, true));
            AddColumn(cID, new DBField(DBField.cTypeInt));
            AddColumn(cPrettyName, new DBField(DBField.cTypeString));
            AddColumn(cStatus, new DBField(DBField.cTypeString));
            AddColumn(cFirstAired, new DBField(DBField.cTypeString));
            AddColumn(cGenre, new DBField(DBField.cTypeString));
            AddColumn(cBannerFileName, new DBField(DBField.cTypeString));
            AddColumn(cSummary, new DBField(DBField.cTypeString));
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

        public static List<DBSeries> Get(String sqlQuery)
        {
            DBSeries dummy = new DBSeries();
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
