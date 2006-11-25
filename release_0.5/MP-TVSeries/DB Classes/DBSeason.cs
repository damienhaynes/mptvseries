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
        public const String cOutName = "Season";

        public const String cID = "ID"; // local name, unique (it's the primary key) which is a composite of the series name & the season index
        public const String cSeriesID = "SeriesID";
        public const String cIndex = "SeasonIndex";
        public const String cBannerFileNames = "BannerFileNames";
        public const String cCurrentBannerFileName = "CurrentBannerFileName";
        public const String cHasLocalFiles = "HasLocalFiles";
        public const String cHasLocalFilesTemp = "HasLocalFilesTemp";

        public static Dictionary<String, String> s_FieldToDisplayNameMap = new Dictionary<String, String>();

        static DBSeason()
        {
            s_FieldToDisplayNameMap.Add(cID, "Composite Season ID");
            s_FieldToDisplayNameMap.Add(cSeriesID, "Series ID");
            s_FieldToDisplayNameMap.Add(cIndex, "Season Index");
            s_FieldToDisplayNameMap.Add(cBannerFileNames, "Banner FileName List");
            s_FieldToDisplayNameMap.Add(cCurrentBannerFileName, "Current Banner FileName");

            int nCurrentDBSeasonVersion = 2;
            while (DBOption.GetOptions(DBOption.cDBSeasonVersion) != nCurrentDBSeasonVersion)
                // take care of the upgrade in the table
                switch ((int)DBOption.GetOptions(DBOption.cDBSeasonVersion))
                {
                    case 1:
                        // upgrade to version 2; clear the season table (series table format changed)
                        try
                        {
                            String sqlQuery = "DROP TABLE season";
                            DBTVSeries.Execute(sqlQuery);
                            DBOption.SetOptions(DBOption.cDBSeasonVersion, nCurrentDBSeasonVersion);
                        }
                        catch {}
                        break;

                    default:
                        break;
                }
            DBSeason dummy = new DBSeason();
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

        public DBSeason(int nSeriesID, int nSeasonIndex)
            : base(cTableName)
        {
            InitColumns();
            String sSeasonID = nSeriesID + "_s" + nSeasonIndex;
            if (!ReadPrimary(sSeasonID))
            {
                InitValues();
                // set the parent series so that banners will be refreshed from scratched
                DBOnlineSeries series = new DBOnlineSeries(nSeriesID);
                series[DBOnlineSeries.cBannersDownloaded] = 0;
                series.Commit();
            }
            this[cSeriesID] = nSeriesID;
            this[cIndex] = nSeasonIndex;
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
            AddColumn(cID, new DBField(DBField.cTypeString, true));
            AddColumn(cSeriesID, new DBField(DBField.cTypeInt));
            AddColumn(cIndex, new DBField(DBField.cTypeInt));
            AddColumn(cBannerFileNames, new DBField(DBField.cTypeString));
            AddColumn(cCurrentBannerFileName, new DBField(DBField.cTypeString));
            AddColumn(cHasLocalFiles, new DBField(DBField.cTypeInt));
            AddColumn(cHasLocalFilesTemp, new DBField(DBField.cTypeInt));
        }

        public void ChangeSeriesID(int nSeriesID)
        {
            DBSeason newSeason = new DBSeason();
            String sSeasonID = nSeriesID + "_s" + base[cIndex];
            if (!newSeason.ReadPrimary(sSeasonID))
            {
                foreach (String fieldName in FieldNames)
                {
                    switch (fieldName)
                    {
                        case cSeriesID:
                        case cID:
                            break;

                        default:
                            newSeason[fieldName] = base[fieldName];
                            break;
                    }
                }
                newSeason[cID] = sSeasonID;
                newSeason[cSeriesID] = nSeriesID;
                newSeason.Commit();
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

        public static void Clear(SQLCondition conditions)
        {
            Clear(new DBSeason(), conditions);
        }

        public static void GlobalSet(String sKey, DBValue Value)
        {
            GlobalSet(sKey, Value, new SQLCondition());
        }

        public static void GlobalSet(String sKey, DBValue Value, SQLCondition condition)
        {
            GlobalSet(new DBSeason(), sKey, Value, condition);
        }

        public static void GlobalSet(String sKey1, String sKey2)
        {
            GlobalSet(sKey1, sKey2, new SQLCondition());
        }

        public static void GlobalSet(String sKey1, String sKey2, SQLCondition condition)
        {
            GlobalSet(new DBSeason(), sKey1, sKey2, condition);
        }

        public static List<DBSeason> Get(int nSeriesID, Boolean bExistingFilesOnly)
        {
            // create table if it doesn't exist already
            SQLCondition condition = new SQLCondition();
            condition.Add(new DBSeason(), cSeriesID, nSeriesID, SQLConditionType.Equal);
            if (bExistingFilesOnly)
                condition.Add(new DBSeason(), cHasLocalFiles, 0, SQLConditionType.Equal);
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
