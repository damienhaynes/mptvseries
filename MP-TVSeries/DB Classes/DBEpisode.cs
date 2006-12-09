using System;
using System.Collections.Generic;
using System.Text;
using SQLite.NET;
using MediaPortal.Database;

namespace WindowPlugins.GUITVSeries
{
    public class DBOnlineEpisode : DBTable
    {
        public const String cTableName = "online_episodes";

        public const String cCompositeID = "CompositeID";         // composite string used for primary index, based on series, season & episode
        public const String cSeriesID = "SeriesID";
        public const String cID = "EpisodeID";                    // onlineDB episodeID
        public const String cSeasonIndex = "SeasonIndex";         // season index
        public const String cEpisodeIndex = "EpisodeIndex";       // episode index
        public const String cEpisodeName = "EpisodeName";         // episode name
        public const String cWatched = "Watched";          // tag to know if episode has been watched already (overrides the local file's tag)
        public const String cEpisodeSummary = "Summary";
        public const String cFirstAired = "FirstAired";
        public const String cOnlineDataImported = "OnlineDataImported";
        public const String cGuestStars = "GuestStars";
        public const String cDirector = "Director";
        public const String cWriter = "Writer";
        public const String cHidden = "Hidden";
        public const String cLastUpdated = "lastupdated";

        public static Dictionary<String, String> s_OnlineToFieldMap = new Dictionary<String, String>();
        public static Dictionary<string, DBField> s_fields = new Dictionary<string,DBField>();

        static DBOnlineEpisode()
        {
            s_OnlineToFieldMap.Add("SeasonNumber", cSeasonIndex);
            s_OnlineToFieldMap.Add("EpisodeNumber", cEpisodeIndex);
            s_OnlineToFieldMap.Add("EpisodeName", cEpisodeName);
            s_OnlineToFieldMap.Add("id", cID);
            s_OnlineToFieldMap.Add("Overview", cEpisodeSummary);
            s_OnlineToFieldMap.Add("FirstAired", cFirstAired);

            // make sure the table is created on first run
            DBOnlineEpisode dummy = new DBOnlineEpisode();
        }

        public DBOnlineEpisode()
            : base(cTableName)
        {
            InitColumns();
            InitValues();
        }

        public DBOnlineEpisode(DBValue nSeriesID, DBValue nSeasonIndex, DBValue nEpisodeIndex)
            : base(cTableName)
        {
            InitColumns();
            if (!ReadPrimary(nSeriesID + "_" + nSeasonIndex + "x" + nEpisodeIndex))
                InitValues();
            this[cSeriesID] = nSeriesID;
            this[cSeasonIndex] = nSeasonIndex;
            this[cEpisodeIndex] = nEpisodeIndex;
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
            base.AddColumn(cCompositeID, new DBField(DBField.cTypeString, true));
            base.AddColumn(cID, new DBField(DBField.cTypeInt));
            base.AddColumn(cSeriesID, new DBField(DBField.cTypeInt));
            base.AddColumn(cEpisodeIndex, new DBField(DBField.cTypeInt));
            base.AddColumn(cSeasonIndex, new DBField(DBField.cTypeInt));
            base.AddColumn(cEpisodeName, new DBField(DBField.cTypeString));

            base.AddColumn(cWatched, new DBField(DBField.cTypeInt));
            base.AddColumn(cEpisodeSummary, new DBField(DBField.cTypeString));
            base.AddColumn(cFirstAired, new DBField(DBField.cTypeString));
            base.AddColumn(cOnlineDataImported, new DBField(DBField.cTypeInt));
            base.AddColumn(cGuestStars, new DBField(DBField.cTypeString));
            base.AddColumn(cDirector, new DBField(DBField.cTypeString));
            base.AddColumn(cWriter, new DBField(DBField.cTypeString));
            base.AddColumn(cHidden, new DBField(DBField.cTypeInt));
            base.AddColumn(cLastUpdated, new DBField(DBField.cTypeString));

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
            Clear(new DBOnlineEpisode(), conditions);
        }
    };

    public class DBEpisode : DBTable
    {
        public const String cTableName = "local_episodes";
        public const String cOutName = "Episode";

        public const String cFilename = "EpisodeFilename";
        public const String cCompositeID = DBOnlineEpisode.cCompositeID;           // composite string used for link key to online episode data
        public const String cSeriesID = DBOnlineEpisode.cSeriesID;
        public const String cSeasonIndex = DBOnlineEpisode.cSeasonIndex;
        public const String cEpisodeIndex = DBOnlineEpisode.cEpisodeIndex;
        public const String cEpisodeName = "LocalEpisodeName";
        public const String cWatched = DBOnlineEpisode.cWatched;
        public const String cImportProcessed = "LocalImportProcessed";
        public const String cAvailableSubtitles = "AvailableSubtitles";

        private DBOnlineEpisode m_onlineEpisode = null;

        public static Dictionary<String, String> s_FieldToDisplayNameMap = new Dictionary<String, String>();
        public static Dictionary<String, String> s_OnlineToFieldMap = new Dictionary<String, String>();
        public static Dictionary<string, DBField> s_fields = new Dictionary<string, DBField>();

        static DBEpisode()
        {
            // make sure the table is created on first run
            DBEpisode dummy = new DBEpisode();

            s_FieldToDisplayNameMap.Add(cFilename, "Local FileName");
            s_FieldToDisplayNameMap.Add(cCompositeID, "Composite Episode ID");
            s_FieldToDisplayNameMap.Add(cSeriesID, "Series ID");
            s_FieldToDisplayNameMap.Add(cSeasonIndex, "Season Index");
            s_FieldToDisplayNameMap.Add(cEpisodeIndex, "Episode Index");
            s_FieldToDisplayNameMap.Add(cEpisodeName, "Episode Name");
            s_FieldToDisplayNameMap.Add(cWatched, "Watched");
            s_FieldToDisplayNameMap.Add(DBOnlineEpisode.cID, "Episode ID");
            s_FieldToDisplayNameMap.Add(DBOnlineEpisode.cEpisodeSummary, "Overview");

            s_OnlineToFieldMap.Add("SeasonNumber", cSeasonIndex);
            s_OnlineToFieldMap.Add("EpisodeNumber", cEpisodeIndex);
            s_OnlineToFieldMap.Add("EpisodeName", cEpisodeName);
            s_OnlineToFieldMap.Add("id", DBOnlineEpisode.cID);
            s_OnlineToFieldMap.Add("Overview", DBOnlineEpisode.cEpisodeSummary);

            int nCurrentDBEpisodeVersion = 3;
            while (DBOption.GetOptions(DBOption.cDBEpisodesVersion) != nCurrentDBEpisodeVersion)
                // take care of the upgrade in the table
                switch ((int)DBOption.GetOptions(DBOption.cDBEpisodesVersion))
                {
                    case 1:
                        // upgrade to version 2; clear the series table (we use 2 other tables now)
                        try
                        {
                            String sqlQuery = "DROP TABLE " + cTableName;
                            DBTVSeries.Execute(sqlQuery);
                            sqlQuery = "DROP TABLE " + DBOnlineEpisode.cTableName;
                            DBTVSeries.Execute(sqlQuery);
                            DBOption.SetOptions(DBOption.cDBEpisodesVersion, nCurrentDBEpisodeVersion);
                        }
                        catch { }
                        break;

                    case 2:
                        DBOnlineEpisode.GlobalSet(new DBOnlineEpisode(), DBOnlineEpisode.cHidden, 0, new SQLCondition());
                        DBOption.SetOptions(DBOption.cDBEpisodesVersion, nCurrentDBEpisodeVersion);
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

        public DBEpisode() 
            : base(cTableName)
        {
            InitColumns();
            InitValues();
        }

        public DBEpisode(bool bCreateEmptyOnline)
            : base(cTableName)
        {
            InitColumns();
            InitValues();
            if (bCreateEmptyOnline)
                m_onlineEpisode = new DBOnlineEpisode();
        }

        public DBEpisode(String filename)
            : base(cTableName)
        {
            InitColumns();
            if (!ReadPrimary(filename))
                InitValues();
            if (base[cSeriesID] != String.Empty && base[cSeasonIndex] != -1 && base[cEpisodeIndex] != -1)
            {
                m_onlineEpisode = new DBOnlineEpisode(base[cSeriesID], base[cSeasonIndex], base[cEpisodeIndex]);
                base[cCompositeID] = m_onlineEpisode[DBOnlineEpisode.cCompositeID];
            }
        }

        public void ChangeSeriesID(int nSeriesID)
        {
            DBOnlineEpisode newOnlineEpisode = new DBOnlineEpisode();
            if (!newOnlineEpisode.ReadPrimary(nSeriesID + "_" + base[cSeasonIndex] + "x" + base[cEpisodeIndex]))
            {
                newOnlineEpisode[cSeriesID] = nSeriesID;
                newOnlineEpisode[cSeasonIndex] = base[cSeasonIndex];
                newOnlineEpisode[cEpisodeIndex] = base[cEpisodeIndex];

                foreach (String fieldName in m_onlineEpisode.FieldNames)
                {
                    switch (fieldName)
                    {
                        case DBOnlineEpisode.cCompositeID:
                        case DBOnlineEpisode.cSeriesID:
                            break;

                        default:
                            newOnlineEpisode[fieldName] = m_onlineEpisode[fieldName];
                            break;
                    }
                }
            }
            base[cCompositeID] = newOnlineEpisode[DBOnlineEpisode.cCompositeID];
            base[cSeriesID] = nSeriesID;
            m_onlineEpisode = newOnlineEpisode;
            Commit();
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
            base.AddColumn(cFilename, new DBField(DBField.cTypeString, true));
            base.AddColumn(cCompositeID, new DBField(DBField.cTypeString));
            base.AddColumn(cSeriesID, new DBField(DBField.cTypeInt));
            base.AddColumn(cSeasonIndex, new DBField(DBField.cTypeInt));
            base.AddColumn(cEpisodeIndex, new DBField(DBField.cTypeInt));
            base.AddColumn(cEpisodeName, new DBField(DBField.cTypeString));
            base.AddColumn(cImportProcessed, new DBField(DBField.cTypeInt));
            base.AddColumn(cWatched, new DBField(DBField.cTypeInt));
            base.AddColumn(cAvailableSubtitles, new DBField(DBField.cTypeString));

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

        public override void InitValues()
        {
            base.InitValues();
            base[cSeasonIndex] = -1;
            base[cEpisodeIndex] = -1;
        }

        public DBOnlineEpisode onlineEpisode
        {
            get { return m_onlineEpisode; }
        }

        public override List<String> FieldNames
        {
            get
            {
                List<String> outList = new List<String>();
                // some ordering: I want some fields to come up first
                outList.Add(cSeriesID);
                outList.Add(cSeasonIndex);
                outList.Add(cEpisodeIndex);
                outList.Add(cCompositeID);

                foreach (KeyValuePair<string, DBField> pair in m_fields)
                {
                    if (outList.IndexOf(pair.Key) == -1)
                        outList.Add(pair.Key);
                }
                if (m_onlineEpisode != null)
                {
                    foreach (KeyValuePair<string, DBField> pair in m_onlineEpisode.m_fields)
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
                // online data always takes precedence over the local file data
                if (m_onlineEpisode != null)
                {
                    DBValue retVal = null;
                    switch (fieldName)
                    {
                        case cEpisodeName:
                            retVal = m_onlineEpisode[DBOnlineEpisode.cEpisodeName];
                            if (retVal == null || retVal == String.Empty)
                                retVal = base[cEpisodeName];
                            return retVal;

                        default:
                            retVal = m_onlineEpisode[fieldName];
                            if (retVal == null || retVal == String.Empty)
                                retVal = base[fieldName];
                            return retVal;
                    }
                }
                else
                    return base[fieldName];
            }

            set
            {
                if (m_onlineEpisode != null)
                {
                    switch (fieldName)
                    {
                        case cImportProcessed:
                        case cEpisodeName:
                            // the only flags we are not rerouting to the onlineEpisode if it exists
                            break;


                        default:
                            m_onlineEpisode[fieldName] = value;
                            return;
                    }
                }

                base[fieldName] = value;

                if (m_onlineEpisode == null && base[cSeriesID] != String.Empty && base[cSeasonIndex] != -1 && base[cEpisodeIndex] != -1)
                {
                    // we have enough data to create an online episode
                    m_onlineEpisode = new DBOnlineEpisode(base[cSeriesID], base[cSeasonIndex], base[cEpisodeIndex]);
                    base[cCompositeID] = m_onlineEpisode[DBOnlineEpisode.cCompositeID];
                    Commit();
                }
            }
        }

        public override bool Commit()
        {
            if (m_onlineEpisode != null)
                m_onlineEpisode.Commit();
            
            return base.Commit();
        }

        public static List<DBEpisode> Get(int nSeriesID, Boolean bExistingFilesOnly)
        {
            SQLCondition conditions = null;
            if (bExistingFilesOnly)
            {
                conditions = new SQLCondition();
                conditions.Add(new DBEpisode(), cSeriesID, nSeriesID, SQLConditionType.Equal);
                return Get(conditions, false);
            }
            else
            {
                conditions = new SQLCondition();
                conditions.Add(new DBOnlineEpisode(), cSeriesID, nSeriesID, SQLConditionType.Equal);
                return Get(conditions, true);
            }
        }

        public static List<DBEpisode> Get(int nSeriesID, int nSeasonIndex, Boolean bExistingFilesOnly, Boolean bIncludeHidden)
        {
            SQLCondition conditions = null;
            if (bExistingFilesOnly)
            {
                conditions = new SQLCondition();
                conditions.Add(new DBEpisode(), cSeriesID, nSeriesID, SQLConditionType.Equal);
                conditions.Add(new DBEpisode(), cSeasonIndex, nSeasonIndex, SQLConditionType.Equal);
                if (!bIncludeHidden)
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cHidden, 0, SQLConditionType.Equal);

                return Get(conditions, false);
            }
            else
            {
                conditions = new SQLCondition();
                conditions.Add(new DBOnlineEpisode(), cSeriesID, nSeriesID, SQLConditionType.Equal);
                conditions.Add(new DBOnlineEpisode(), cSeasonIndex, nSeasonIndex, SQLConditionType.Equal);
                if (!bIncludeHidden)
                    conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cHidden, 0, SQLConditionType.Equal);
                return Get(conditions, true);
            }
        }

        public static List<DBEpisode> Get(SQLCondition conditions, bool bOnline)
        {
            String sqlQuery = String.Empty;
            if (bOnline)
            {
                SQLWhat what = new SQLWhat(new DBOnlineEpisode());
                what.AddWhat(new DBEpisode());
//                what.Add(DBSeries.Q(DBSeries.cID));
                // provide only qualitied fields, stupid trick for how MP'SQL handles multiple columns with the same name (it uses the last one, it should use the first one IMO)

                sqlQuery = "select " + what + " left join " + cTableName + " on " + DBEpisode.Q(cCompositeID) + "==" + DBOnlineEpisode.Q(cCompositeID) + conditions + " order by " + DBOnlineEpisode.Q(cEpisodeIndex);
            }
            else
            {
                SQLWhat what = new SQLWhat(new DBEpisode());
                what.Add(new DBOnlineEpisode());
                sqlQuery = "select " + what + conditions + " and " + DBEpisode.Q(cCompositeID) + "==" + DBOnlineEpisode.Q(cCompositeID) + " order by " + Q(cEpisodeIndex);

            }

            SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
            List<DBEpisode> outList = new List<DBEpisode>();
            if (results.Rows.Count > 0)
            {
                for (int index = 0; index < results.Rows.Count; index++)
                {
                    DBEpisode episode = new DBEpisode();
                    episode.Read(ref results, index);
                    episode.m_onlineEpisode = new DBOnlineEpisode();
                    episode.m_onlineEpisode.Read(ref results, index);
                    outList.Add(episode);
                }
            }
            return outList;
        }

        public static void Clear(SQLCondition conditions)
        {
            Clear(new DBEpisode(), conditions);
        }

        public static void GlobalSet(String sKey, DBValue Value)
        {
            GlobalSet(sKey, Value, new SQLCondition());
        }

        public static void GlobalSet(String sKey, DBValue Value, SQLCondition condition)
        {
            GlobalSet(new DBOnlineEpisode(), sKey, Value, condition);
            GlobalSet(new DBEpisode(), sKey, Value, condition);
        }

        public static new String Q(String sField)
        {
            return cTableName + "." + sField;
        }

    }
}
