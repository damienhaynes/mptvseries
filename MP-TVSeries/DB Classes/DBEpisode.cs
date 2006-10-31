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
        public const String cSeriesParsedName = DBSeries.cParsedName;
        public const String cID = "EpisodeID";                    // onlineDB episodeID
        public const String cSeriesID = "SeriesID";               // onlineDB seriesID
        public const String cSeasonIndex = "SeasonIndex";         // season index
        public const String cEpisodeIndex = "EpisodeIndex";       // episode index
        public const String cEpisodeName = "EpisodeName";         // episode name
        public const String cWatched = "Watched";          // tag to know if episode has been watched already (overrides the local file's tag)
        public const String cEpisodeSummary = "Summary";

        public DBOnlineEpisode()
            : base(cTableName)
        {
            InitColumns();
            InitValues();
        }

        public DBOnlineEpisode(DBValue sSeriesName, DBValue nSeasonIndex, DBValue nEpisodeIndex)
            : base(cTableName)
        {
            InitColumns();
            if (!ReadPrimary(sSeriesName + "_" + nSeasonIndex + "x" + nEpisodeIndex))
                InitValues();
            this[cSeriesParsedName] = sSeriesName;
            this[cSeasonIndex] = nSeasonIndex;
            this[cEpisodeIndex] = nEpisodeIndex;
        }

        private void InitValues()
        {
            base[cSeriesParsedName] = String.Empty;
            this[cSeasonIndex] = 0;
            this[cEpisodeIndex] = 0;
            this[cEpisodeName] = String.Empty;

            this[cID] = 0;
            this[cSeriesID] = 0;
            this[cEpisodeName] = String.Empty;
            this[cWatched] = 0;
            this[cEpisodeSummary] = String.Empty;
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
            AddColumn(cCompositeID, new DBField(DBField.cTypeString, true));
            AddColumn(cID, new DBField(DBField.cTypeInt));
            AddColumn(cSeriesParsedName, new DBField(DBField.cTypeString));
            AddColumn(cEpisodeIndex, new DBField(DBField.cTypeInt));
            AddColumn(cSeasonIndex, new DBField(DBField.cTypeInt));
            AddColumn(cEpisodeName, new DBField(DBField.cTypeString));

            AddColumn(cSeriesID, new DBField(DBField.cTypeInt));
            AddColumn(cWatched, new DBField(DBField.cTypeInt));
            AddColumn(cEpisodeSummary, new DBField(DBField.cTypeString));
        }

        public static new String Q(String sField)
        {
            return cTableName + "." + sField;
        }
    };

    public class DBEpisode : DBTable
    {
        public const String cTableName = "local_episodes";

        public const String cFilename = "EpisodeFilename";
        public const String cCompositeID = DBOnlineEpisode.cCompositeID;           // composite string used for link key to online episode data
        public const String cSeriesParsedName = DBSeries.cParsedName;
        public const String cSeasonIndex = DBOnlineEpisode.cSeasonIndex;
        public const String cEpisodeIndex = DBOnlineEpisode.cEpisodeIndex;
        public const String cEpisodeName =DBOnlineEpisode.cEpisodeName;
        public const String cWatched = DBOnlineEpisode.cWatched;
        public const String cImportProcessed = "ImportProcessed";

        private DBOnlineEpisode m_onlineEpisode = null;

        public DBEpisode() 
            : base(cTableName)
        {
            InitColumns();
            InitValues();
        }

        public DBEpisode(String filename)
            : base(cTableName)
        {
            InitColumns();
            if (!ReadPrimary(filename))
                InitValues();
            if (base[cSeriesParsedName] != String.Empty && base[cSeasonIndex] != 0 && base[cEpisodeIndex] != 0)
            {
                m_onlineEpisode = new DBOnlineEpisode(base[cSeriesParsedName], base[cSeasonIndex], base[cEpisodeIndex]);
                base[cCompositeID] = m_onlineEpisode[DBOnlineEpisode.cCompositeID];
            }
        }

        private void InitValues()
        {
            base[cCompositeID] = String.Empty;
            base[cSeriesParsedName] = String.Empty;
            base[cSeasonIndex] = 0;
            base[cEpisodeIndex] = 0;
            base[cEpisodeName] = String.Empty;
            base[cImportProcessed] = 0;
            base[cWatched] = 0;
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
            AddColumn(cFilename, new DBField(DBField.cTypeString, true));
            AddColumn(cCompositeID, new DBField(DBField.cTypeString));
            AddColumn(cSeriesParsedName, new DBField(DBField.cTypeString));
            AddColumn(cSeasonIndex, new DBField(DBField.cTypeInt));
            AddColumn(cEpisodeIndex, new DBField(DBField.cTypeInt));
            AddColumn(cEpisodeName, new DBField(DBField.cTypeString));
            AddColumn(cImportProcessed, new DBField(DBField.cTypeInt));
            AddColumn(cWatched, new DBField(DBField.cTypeInt));
        }

        // function override to search on both this & the onlineEpisode
        public override DBValue this[String fieldName]
        {
            get
            {
                DBValue retVal = null;

                // online data always takes precedence over the local file data
                if (m_onlineEpisode != null)
                {
                    retVal = m_onlineEpisode[fieldName];
                }

                if (retVal == null || retVal == String.Empty)
                    retVal = base[fieldName];
                return retVal;
            }

            set
            {
                if (m_onlineEpisode != null)
                {
                    switch (fieldName)
                    {
                        case cImportProcessed:
                            // the only flag we are not rerouting to the onlineEpisode if it exists
                            break;

                        default:
                            m_onlineEpisode[fieldName] = value;
                            return;
                    }
                }

                base[fieldName] = value;

                if (m_onlineEpisode == null && base[cSeriesParsedName] != String.Empty && base[cSeasonIndex] != 0 && base[cEpisodeIndex] != 0)
                {
                    // we have enough data to create an online episode
                    m_onlineEpisode = new DBOnlineEpisode(base[cSeriesParsedName], base[cSeasonIndex], base[cEpisodeIndex]);
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

        public static List<DBEpisode> Get(String sSeriesName, Boolean bExistingFilesOnly)
        {
            String sqlQuery = String.Empty;
            if (bExistingFilesOnly)
            {
                SQLWhat what = new SQLWhat(new DBEpisode());
                what.Add(new DBOnlineEpisode());
                SQLCondition conditions = new SQLCondition(new DBEpisode());
                conditions.Add(cSeriesParsedName, sSeriesName);
                sqlQuery = "select " + what + " where " + conditions + " and " + cTableName + "." + cCompositeID + "==" + DBOnlineEpisode.cTableName + "." + DBOnlineEpisode.cCompositeID + " order by " + Q(cSeasonIndex) + "," + Q(cEpisodeIndex);
            }
            else
            {
                SQLWhat what = new SQLWhat(new DBOnlineEpisode());
                what.AddWhat(new DBEpisode());
                SQLCondition conditions = new SQLCondition(new DBOnlineEpisode());
                conditions.Add(cSeriesParsedName, sSeriesName);
                // stupid trick for how MP handles multiple columns with the same name (it uses the last one, it should use the first one IMO)
                sqlQuery = "select " + what + " left join " + cTableName + " on " + cTableName + "." + cCompositeID + "==" + DBOnlineEpisode.Q(DBOnlineEpisode.cCompositeID) + " where " + conditions + " order by " + DBOnlineEpisode.Q(cSeasonIndex) + "," + DBOnlineEpisode.Q(cEpisodeIndex);
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


        public static List<DBEpisode> Get(String sSeriesName, int nSeasonIndex, Boolean bExistingFilesOnly)
        {
            String sqlQuery = String.Empty;
            if (bExistingFilesOnly)
            {
                SQLWhat what = new SQLWhat(new DBEpisode());
                what.Add(new DBOnlineEpisode());
                SQLCondition conditions = new SQLCondition(new DBEpisode());
                conditions.Add(cSeriesParsedName, sSeriesName);
                conditions.Add(cSeasonIndex, nSeasonIndex);
                sqlQuery = "select " + what + " where " + conditions + " and " + cTableName + "." + cCompositeID + "==" + DBOnlineEpisode.cTableName + "." + DBOnlineEpisode.cCompositeID + " order by " + Q(cEpisodeIndex);
            }
            else
            {
                SQLWhat what = new SQLWhat(new DBOnlineEpisode());
                what.AddWhat(new DBEpisode());
                SQLCondition conditions = new SQLCondition(new DBOnlineEpisode());
                conditions.Add(cSeriesParsedName, sSeriesName);
                conditions.Add(cSeasonIndex, nSeasonIndex);
                // stupid trick for how MP handles multiple columns with the same name (it uses the last one, it should use the first one IMO)
                sqlQuery = "select " + what + " left join " + cTableName + " on " + cTableName + "." + cCompositeID + "==" + DBOnlineEpisode.Q(DBOnlineEpisode.cCompositeID) + " where " + conditions + " order by " + DBOnlineEpisode.Q(cEpisodeIndex);
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

        public static void Clear(List<DBEpisode> episodes)
        {
            String sqlQuery = "delete from " + cTableName;
            if (episodes != null && episodes.Count > 0)
            {
                sqlQuery += " where " + cFilename + " in (";
                foreach (DBEpisode episode in episodes)
                {
                    if (episode[cFilename] != String.Empty)
                        sqlQuery += "'" + episode[cFilename] + "',";
                }
                sqlQuery = sqlQuery.Substring(sqlQuery.Length - 1);
                sqlQuery += ")";
            }
            SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
        }

        public static void Clear(String sKey, object value)
        {
            DBEpisode dummy = new DBEpisode();
            if (dummy.m_fields.ContainsKey(sKey))
            {
                String sqlQuery = "delete from " + cTableName + " where " + sKey + "=";
                switch (dummy.m_fields[sKey].Type)
                {
                    case DBField.cTypeInt:
                        sqlQuery += value.ToString();
                        break;

                    case DBField.cTypeString:
                        sqlQuery += "'" + value.ToString() + "'";
                        break;
                }
                SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
            }
        }

        public static void GlobalSet(String sKey, object Value)
        {
            DBEpisode dummy = new DBEpisode();
            if (dummy.m_fields.ContainsKey(sKey))
            {
                String sqlQuery = "update " + cTableName + " SET " + sKey + "=";
                switch (dummy.m_fields[sKey].Type)
                {
                    case DBField.cTypeInt:
                        sqlQuery += Value.ToString();
                        break;

                    case DBField.cTypeString:
                        sqlQuery += "'" + Value.ToString() + "'";
                        break;
                }
                SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
            }
        }

        public static new String Q(String sField)
        {
            return cTableName + "." + sField;
        }

    }
}
