using System;
using System.Collections.Generic;
using System.Text;
using SQLite.NET;
using System.IO;
using MediaPortal.Database;

namespace WindowPlugins.GUITVSeries
{
    public class DBSeason : DBTable
    {
        public const String cTableName = "season";

        public const String cID = "ID"; // local name, unique (it's the primary key) which is a composite of the series name & the season index
        public const String cSeriesName = DBSeries.cParsedName;
        public const String cIndex = "SeasonIndex";
        public const String cBannerFileNames = "BannerFileNames";
        public const String cCurrentBannerFileName = "CurrentBannerFileName";
        public const String cHasLocalFiles = "HasLocalFiles";

        public static Dictionary<String, String> s_FieldToDisplayNameMap = new Dictionary<String, String>();

        static DBSeason()
        {
            s_FieldToDisplayNameMap.Add(cID, "Composite Season ID");
            s_FieldToDisplayNameMap.Add(cSeriesName, "Series Parsed Name");
            s_FieldToDisplayNameMap.Add(cIndex, "Season Index");
            s_FieldToDisplayNameMap.Add(cBannerFileNames, "Banner FileName List");
            s_FieldToDisplayNameMap.Add(cCurrentBannerFileName, "Current Banner FileName");
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
            AddColumn(cBannerFileNames, new DBField(DBField.cTypeString));
            AddColumn(cCurrentBannerFileName, new DBField(DBField.cTypeString));
            AddColumn(cHasLocalFiles, new DBField(DBField.cTypeInt));
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

                String[] split = sList.Split(new char[] { '|' });
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

        public static void GlobalSet(String sKey, DBValue Value)
        {
            GlobalSet(sKey, Value, new SQLCondition(new DBSeason()));
        }

        public static void GlobalSet(String sKey, DBValue Value, SQLCondition condition)
        {
            GlobalSet(new DBSeason(), sKey, Value, condition);
        }

        public static List<DBSeason> Get(String sSeriesName, Boolean bExistingFilesOnly)
        {
            // create table if it doesn't exist already
            SQLCondition condition = new SQLCondition(new DBSeason());
            condition.Add(cSeriesName, sSeriesName, true);
            if (bExistingFilesOnly)
                condition.Add(cHasLocalFiles, 0, false);
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
