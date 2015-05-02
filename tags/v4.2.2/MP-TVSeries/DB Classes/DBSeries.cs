#region GNU license
// MP-TVSeries - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2007
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SQLite.NET;

namespace WindowPlugins.GUITVSeries
{
    //moved to DBOnineSeries.cs
    //public class DBOnlineSeries : DBTable

    public class DBSeries : DBTable
    {
        public delegate void dbSeriesUpdateOccuredDelegate(DBSeries updated);
        public static event dbSeriesUpdateOccuredDelegate dbSeriesUpdateOccured;

        public const String cTableName = "local_series";
        public const String cOutName = "Series";

        #region DB Field Names
        public const String cParsedName = "Parsed_Name";
        public const String cID = "ID";
        public const String cScanIgnore = "ScanIgnore";
        public const String cDuplicateLocalName = "DuplicateLocalName";
        public const String cHidden = "Hidden";
        #endregion

        public const int cDBVersion = 15;

        private DBOnlineSeries m_onlineSeries = null;
		new public static List<string> FieldsRequiringSplit = new List<string>(new string[] { "Genre", "Actors", "Network", "ViewTags" });
        public static Dictionary<String, String> s_FieldToDisplayNameMap = new Dictionary<String, String>();
        static int s_nLastLocalID;

        public List<string> cachedLogoResults = null;

        static DBSeries()
        {
            // make sure the table is created on first run (and columns are added before we call SET)
            DBSeries dummy = new DBSeries();

            s_nLastLocalID = DBOption.GetOptions(DBOption.cDBSeriesLastLocalID);

            s_FieldToDisplayNameMap.Add(cParsedName, "Parsed Name");

            int nCurrentDBVersion = cDBVersion;
            int nUpgradeDBVersion = DBOption.GetOptions(DBOption.cDBSeriesVersion);

            while (nUpgradeDBVersion != nCurrentDBVersion)
            {
                SQLCondition condEmpty = new SQLCondition();
                List<DBSeries> AllSeries = Get(condEmpty);

                // take care of the upgrade in the table    
                switch (nUpgradeDBVersion)
                {
                    case 1:
                    case 2:
                        // upgrade to version 3; clear the series table (we use 2 other tables now)
                        try
                        {
                            String sqlQuery = "DROP TABLE series";
                            DBTVSeries.Execute(sqlQuery);
                            nUpgradeDBVersion++;
                        }
                        catch { }
                        break;

                    case 3:
                        // set all new perseries timestamps to 0
                        DBOnlineSeries.GlobalSet(new DBOnlineSeries(), DBOnlineSeries.cGetEpisodesTimeStamp, 0, new SQLCondition());
                        DBOnlineSeries.GlobalSet(new DBOnlineSeries(), DBOnlineSeries.cUpdateBannersTimeStamp, 0, new SQLCondition());
                        nUpgradeDBVersion++;
                        break;

                    case 4:
                        DBSeries.GlobalSet(new DBSeries(), DBSeries.cHidden, 0, new SQLCondition());
                        nUpgradeDBVersion++;
                        break;

                    case 5:
                        // copy all local parsed name into the online series if seriesID = 0
                        SQLCondition conditions = new SQLCondition();
                        conditions.Add(new DBOnlineSeries(), DBOnlineSeries.cID, 0, SQLConditionType.LessThan);
                        // just getting the series should be enough
                        List<DBSeries> seriesList = DBSeries.Get(conditions);
                        nUpgradeDBVersion++;
                        break;

                    case 6:
                        // set all watched flag timestamp to 0 (will be created)
                        DBOnlineSeries.GlobalSet(new DBOnlineSeries(), DBOnlineSeries.cWatchedFileTimeStamp, 0, new SQLCondition());
                        nUpgradeDBVersion++;
                        break;

                    case 7:
                        // all series no tagged for auto download at first
                        DBOnlineSeries.GlobalSet(new DBOnlineSeries(), DBOnlineSeries.cTaggedToDownload, 0, new SQLCondition());
                        nUpgradeDBVersion++;
                        break;

                    case 8:
                        // create the unwatcheditem value by parsin the episodes                
                        foreach (DBSeries series in AllSeries)
                        {
                            DBEpisode episode = DBEpisode.GetFirstUnwatched(series[DBSeries.cID]);
                            if (episode != null)
                                series[DBOnlineSeries.cUnwatchedItems] = true;
                            else
                                series[DBOnlineSeries.cUnwatchedItems] = false;
                            series.Commit();
                        }
                        nUpgradeDBVersion++;
                        break;

                    case 9:
                        // Set number of watched/unwatched episodes                                       
                        foreach (DBSeries series in AllSeries)
                        {                                                                                    
                            int epsTotal = 0;
                            int epsUnWatched = 0;
                            DBEpisode.GetSeriesEpisodeCounts(series[DBSeries.cID], out epsTotal, out epsUnWatched);
                            series[DBOnlineSeries.cEpisodeCount] = epsTotal;
                            series[DBOnlineSeries.cEpisodesUnWatched] = epsUnWatched;
                            series.Commit();
                        }
                        nUpgradeDBVersion++;
                        break;
                    
                    case 10:
                        // Update Sort Name Column
                        foreach (DBSeries series in AllSeries)
                        {
                            series[DBOnlineSeries.cSortName] = Helper.GetSortByName(series[DBOnlineSeries.cPrettyName]);
                            series.Commit();
                        }
                        nUpgradeDBVersion++;
                        break;
                    
                    case 11:
                        // Migrate isFavourite to new Tagged View
                        conditions = new SQLCondition();
                        conditions.Add(new DBOnlineSeries(), DBOnlineSeries.cIsFavourite, "1", SQLConditionType.Equal);
                        seriesList = DBSeries.Get(conditions);

                        MPTVSeriesLog.Write("Migrating Favourite Series");
                        foreach (DBSeries series in seriesList) {
                            // Tagged view are seperated with the pipe "|" character
                            string tagName = "|" + DBView.cTranslateTokenFavourite + "|";                      
                            series[DBOnlineSeries.cViewTags] = Helper.GetSeriesViewTags(series, true, tagName);                             
                            series.Commit();                            
                        }

                        // Migrate isOnlineFavourite to new TaggedView
                        conditions = new SQLCondition();
                        conditions.Add(new DBOnlineSeries(), DBOnlineSeries.cIsOnlineFavourite, "1", SQLConditionType.Equal);
                        seriesList = DBSeries.Get(conditions);

                        MPTVSeriesLog.Write("Migrating Online Favourite Series");
                        foreach (DBSeries series in seriesList) {
                            // Tagged view are seperated with the pipe "|" character
                            string tagName = "|" + DBView.cTranslateTokenOnlineFavourite + "|";
                            series[DBOnlineSeries.cViewTags] = Helper.GetSeriesViewTags(series, true, tagName);
                            series.Commit();                            
                        }

                        nUpgradeDBVersion++;
                        break;
                    case 12:
                        // we now have parsed_series names as titlecased
                        // to avoid users having to re-identify series for new episodes, and to avoid duplicate entries, we upgrade existing series names

                        foreach (var series in AllSeries)
                        {
                            string oldName = series[DBSeries.cParsedName];
                            string newName = oldName.ToTitleCase();
                            MPTVSeriesLog.Write(string.Format("Upgrading Parsed Series Name: {0} to {1}", oldName, newName));
                            series[DBSeries.cParsedName] = newName;
                            series.Commit();
                        }

                        nUpgradeDBVersion++;
                        break;
                    case 13:
                        // original name not working in previous release
                        DBOnlineSeries.GlobalSet(new DBOnlineSeries(), DBOnlineSeries.cOriginalName, (DBValue)string.Empty, new SQLCondition());
                        nUpgradeDBVersion++;
                        break;
                    case 14:
                        // original name not working in previous release
                        DBOnlineSeries.GlobalSet(new DBOnlineSeries(), DBOnlineSeries.cTraktIgnore, 0, new SQLCondition());
                        nUpgradeDBVersion++;
                        break;
                    default:
                        // new DB, nothing special to do
                        nUpgradeDBVersion = nCurrentDBVersion;
                        break;
                }
            }
            DBOption.SetOptions(DBOption.cDBSeriesVersion, nCurrentDBVersion);
        }

        public static String PrettyFieldName(String sFieldName)
        {
            if (DBOnlineSeries.s_FieldToDisplayNameMap.ContainsKey(sFieldName))
                return DBOnlineSeries.s_FieldToDisplayNameMap[sFieldName];
            else if (s_FieldToDisplayNameMap.ContainsKey(sFieldName))
                return s_FieldToDisplayNameMap[sFieldName];
            else
                return sFieldName;
        }

        public DBSeries()
            : base(cTableName)
        {
            InitColumns();
            InitValues();
               
            DBTVSeries.CreateDBIndices("create index if not exists seriesIDLocal on local_series(ID ASC)","local_series",true);
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
            if (this[cID] == 0)
            {
                m_onlineSeries = new DBOnlineSeries(s_nLastLocalID);
                s_nLastLocalID--;
                DBOption.SetOptions(DBOption.cDBSeriesLastLocalID, s_nLastLocalID);
                this[cID] = m_onlineSeries[DBOnlineSeries.cID];
                if (String.IsNullOrEmpty(m_onlineSeries[DBOnlineSeries.cPrettyName]))
                {
                    m_onlineSeries[DBOnlineSeries.cPrettyName] = this[cParsedName];
                    m_onlineSeries[DBOnlineSeries.cSortName] = this[cParsedName];
                    m_onlineSeries.Commit();
                }
            }
            else
            {
                m_onlineSeries = new DBOnlineSeries(this[cID]);
            }
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
            base.AddColumn(cParsedName, new DBField(DBField.cTypeString, true));
            base.AddColumn(cID, new DBField(DBField.cTypeInt));
            base.AddColumn(cScanIgnore, new DBField(DBField.cTypeInt));
            base.AddColumn(cDuplicateLocalName, new DBField(DBField.cTypeInt));
            base.AddColumn(cHidden, new DBField(DBField.cTypeInt));
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
            DBOnlineSeries newOnlineSeries = new DBOnlineSeries(nSeriesID);
            if (m_onlineSeries[DBOnlineSeries.cHasLocalFilesTemp])
                newOnlineSeries[DBOnlineSeries.cHasLocalFilesTemp] = 1;
            if (m_onlineSeries[DBOnlineSeries.cHasLocalFiles])
                newOnlineSeries[DBOnlineSeries.cHasLocalFiles] = 1;
            newOnlineSeries[DBOnlineSeries.cEpisodeOrders] = m_onlineSeries[DBOnlineSeries.cEpisodeOrders];
            newOnlineSeries[DBOnlineSeries.cChosenEpisodeOrder] = m_onlineSeries[DBOnlineSeries.cChosenEpisodeOrder];
            m_onlineSeries = newOnlineSeries;
            this[cID] = nSeriesID;
        }

        public override ICollection<String> FieldNames
        {
            get
            {
                List<String> outList = new List<String>();                
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

        // function override to search on both this & the onlineSeries
        public override DBValue this[String fieldName]
        {
            get
            {
                switch (fieldName)
                {                        
                    case DBOnlineSeries.cPrettyName:
                    case DBOnlineSeries.cSortName:
                        DBValue retVal = null;
                        if (m_onlineSeries != null)
                            retVal = m_onlineSeries[fieldName];

                        if (String.IsNullOrEmpty(retVal))
                            retVal = base[cParsedName];
                        return retVal;

                    case cParsedName:
                    case cScanIgnore:
                    case cDuplicateLocalName:
                    case cHidden:
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
                    case cHidden:
                        base[fieldName] = value;
                        break;

                    case cID:
                        base[fieldName] = value;
                        if (m_onlineSeries != null)
                            m_onlineSeries[fieldName] = value;
                        break;

                    case DBOnlineSeries.cSortName:
                        // Online Field is no longer populated, create it manually                        
                        m_onlineSeries[DBOnlineSeries.cSortName] = Helper.GetSortByName(m_onlineSeries[DBOnlineSeries.cPrettyName]);
                        break;

                    case DBOnlineSeries.cPrettyName:                    
                        if (m_onlineSeries != null)
                        {
                            // Set sort name again just incase Pretty Name wasn't populated
                            m_onlineSeries[DBOnlineSeries.cSortName] = Helper.GetSortByName(value); 
                            m_onlineSeries[fieldName] = value;
                        }
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
                    if (DBOption.GetOptions(DBOption.cRandomBanner) == true) return ImageAllocator.GetRandomBanner(BannerList);
                    if (String.IsNullOrEmpty(m_onlineSeries[DBOnlineSeries.cCurrentBannerFileName]))
                        return String.Empty;
                    
                    if (m_onlineSeries[DBOnlineSeries.cCurrentBannerFileName].ToString().IndexOf(Directory.GetDirectoryRoot(m_onlineSeries[DBOnlineSeries.cCurrentBannerFileName])) == -1)
                        return Helper.PathCombine(Settings.GetPath(Settings.Path.banners), m_onlineSeries[DBOnlineSeries.cCurrentBannerFileName]);
                    else
                        return m_onlineSeries[DBOnlineSeries.cCurrentBannerFileName];
                }
                else
                    return String.Empty;
            }
            set
            {
                if (m_onlineSeries != null)
                {
                    value = value.Replace(Settings.GetPath(Settings.Path.banners), "");
                    m_onlineSeries[DBOnlineSeries.cCurrentBannerFileName] = value;
                }
            }
        }

        public String Poster
        {
            get
            {
                if (m_onlineSeries != null)
                {
                    if (DBOption.GetOptions(DBOption.cRandomBanner) == true) return ImageAllocator.GetRandomBanner(PosterList);
                    if (String.IsNullOrEmpty(m_onlineSeries[DBOnlineSeries.cCurrentPosterFileName]))
                        return String.Empty;
                    
                    if (m_onlineSeries[DBOnlineSeries.cCurrentPosterFileName].ToString().IndexOf(Directory.GetDirectoryRoot(m_onlineSeries[DBOnlineSeries.cCurrentPosterFileName])) == -1)
                        return Helper.PathCombine(Settings.GetPath(Settings.Path.banners), m_onlineSeries[DBOnlineSeries.cCurrentPosterFileName]);
                    else
                        return m_onlineSeries[DBOnlineSeries.cCurrentPosterFileName];
                }
                else
                    return String.Empty;
            }
            set
            {
                if (m_onlineSeries != null)
                {
                    value = value.Replace(Settings.GetPath(Settings.Path.banners), "");
                    m_onlineSeries[DBOnlineSeries.cCurrentPosterFileName] = value;
                }
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

                    // Add custom artwork by user
                    string customArtwork = Helper.PathCombine(Settings.GetPath(Settings.Path.banners), Helper.cleanLocalPath(m_onlineSeries.ToString()) + @"\widebanner\custom.jpg");
                    if (File.Exists(customArtwork))
                        outList.Add(customArtwork);

                    if (String.IsNullOrEmpty(sList))
                        return outList;

                    String[] split = sList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (String filename in split)
                    {
                        outList.Add(Helper.PathCombine(Settings.GetPath(Settings.Path.banners), filename));
                    }
                }
                return outList;
            }
            set
            {
                if (m_onlineSeries != null)
                {
                    var imageList = new List<string>();

                    for (int i = 0; i < value.Count; i++)
                    {
                        imageList.Add(value[i].Replace(Settings.GetPath(Settings.Path.banners), string.Empty));
                    }
                    m_onlineSeries[DBOnlineSeries.cBannerFileNames] = string.Join("|", imageList.ToArray());
                }
            }
        }
        
        public List<String> PosterList
        {
            get
            {
                List<String> outList = new List<string>();
                if (m_onlineSeries != null)
                {
                    String sList = m_onlineSeries[DBOnlineSeries.cPosterFileNames];

                    // Add custom artwork by user
                    string customArtwork = Helper.PathCombine(Settings.GetPath(Settings.Path.banners), Helper.cleanLocalPath(m_onlineSeries.ToString()) + @"\posters\custom.jpg");
                    if (File.Exists(customArtwork))
                        outList.Add(customArtwork);

                    if (String.IsNullOrEmpty(sList))
                        return outList;

                    String[] split = sList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (String filename in split)
                    {
                        outList.Add(Helper.PathCombine(Settings.GetPath(Settings.Path.banners), filename));
                    }
                }
                return outList;
            }
            set
            {
                if (m_onlineSeries != null)
                {
                    var imageList = new List<string>();

                    for (int i = 0; i < value.Count; i++)
                    {
                        imageList.Add(value[i].Replace(Settings.GetPath(Settings.Path.banners), string.Empty));
                    }
                    m_onlineSeries[DBOnlineSeries.cPosterFileNames] = string.Join("|", imageList.ToArray()); ;
                }
            }
        }

        /// <summary>
        /// Used to determine if an update of the series counts is needed after Delete operations
        /// </summary>
        public static bool IsSeriesRemoved { get; set; }

        /// <summary>
        /// Get the Year component from Series First Aired date
        /// </summary>
        public string Year
        {
            get
            {
                if (string.IsNullOrEmpty(this[DBOnlineSeries.cFirstAired]))
                    return string.Empty;

                return this[DBOnlineSeries.cFirstAired].ToString().Split('-')[0];
            }
        }

        public bool IsAiredOrder
        {
            get { return this[DBOnlineSeries.cEpisodeSortOrder] != "DVD"; }
        }

        public override bool Commit()
        {
            if (m_onlineSeries != null)
                m_onlineSeries.Commit();

            if (dbSeriesUpdateOccured != null)
                dbSeriesUpdateOccured(this);
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

        public static SQLCondition stdConditions
        {
            get
            {
                SQLCondition conditions = new SQLCondition();
                // local dups (parsed names)
                conditions.Add(new DBSeries(), DBSeries.cDuplicateLocalName, 0, SQLConditionType.Equal);

                // include hidden?
                if (!DBOption.GetOptions(DBOption.cShowHiddenItems))
                    conditions.Add(new DBSeries(), DBSeries.cHidden, 0, SQLConditionType.Equal);

                if (!Settings.isConfig && DBOption.GetOptions(DBOption.cOnlyShowLocalFiles) && !conditions.ConditionsSQLString.Contains(DBEpisode.cTableName))
                {
                    SQLCondition fullSubCond = new SQLCondition();
                    //fullSubCond.AddCustom(DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID), DBOnlineSeries.Q(DBOnlineSeries.cID), SQLConditionType.Equal);
                    conditions.AddCustom(" online_series.id in( " + DBEpisode.stdGetSQL(fullSubCond, false, true, "distinct " + DBOnlineEpisode.Q(DBOnlineEpisode.cSeriesID)) + " )");
                }
                return conditions;
            }
        }

        public static List<DBSeries> Get(SQLCondition conditions)
        {
            return Get(conditions, false, true);
        }

        public static List<DBSeries> Get(SQLCondition conditions, bool onlyWithUnwatchedEpisodes, bool includeStdCond)
        {
            if (onlyWithUnwatchedEpisodes)
            {
                conditions.AddCustom(@"(
	                        select count(*) from online_episodes
                            where 
	                        seriesID = local_series.ID
                            and watched = '0'
                            ) > 0");
            }

            String sqlQuery = stdGetSQL(conditions, true, includeStdCond);
            return Get(sqlQuery);
        }
        public static string stdGetSQL(SQLCondition conditions, bool selectFull)
        {
            return stdGetSQL(conditions, selectFull, true);
        }
        public static string stdGetSQL(SQLCondition conditions, bool selectFull, bool includeStdCond)
        {
            string field;
            if (selectFull)
            {
                SQLWhat what = new SQLWhat(new DBOnlineSeries());
                what.AddWhat(new DBSeries());
                field = what;
            }
            else field = DBOnlineSeries.Q(DBOnlineSeries.cID) + " from " + DBOnlineSeries.cTableName;

            if (includeStdCond)
            {
                conditions.AddCustom(stdConditions.ConditionsSQLString);
            }

            string conds = conditions;
            string orderBy = string.Empty;
            if (selectFull)
            {
                bool bUseSortName = DBOption.GetOptions(DBOption.cUseSortName);
                orderBy = conditions.customOrderStringIsSet
                      ? conditions.orderString
                      : " order by " + (bUseSortName?"upper(" + DBOnlineSeries.Q(DBOnlineSeries.cSortName) + "),":"") + "upper(" + DBOnlineSeries.Q(DBOnlineSeries.cPrettyName) + ")";
            }
            return "select " + field + " left join " + cTableName + " on " + DBSeries.Q(cID) + "==" + DBOnlineSeries.Q(cID)
                             + conds
                             + orderBy
                             + conditions.limitString;

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
                    if (series[cID] < 0 && series.m_onlineSeries[DBOnlineSeries.cPrettyName].ToString().Length == 0)
                    {
                        series.m_onlineSeries[DBOnlineSeries.cPrettyName] = series[cParsedName];
                        series.m_onlineSeries.Commit();
                    }
                }
            }
            return outList;
        }

        public static DBSeries Get(int seriesID)
        {
            return Get(seriesID, true);
        }

        public static DBSeries Get(int seriesID, bool includeStdCond)
        {
            SQLCondition cond = new SQLCondition();
            cond.Add(new DBOnlineSeries(), DBOnlineSeries.cID, seriesID, SQLConditionType.Equal);
            foreach (DBSeries series in Get(cond, false, includeStdCond))
                return series;
            return null;
        }

        /// <summary>
        /// If Series contains an OnlineSeries, returns its ToString() instead
        /// </summary>
        /// <returns>ParsedName or OnlineSeries.ToString() result</returns>
        public override string ToString()
        {
            if (m_onlineSeries != null)
            {
                string pretty = m_onlineSeries.ToString();
                if (!String.IsNullOrEmpty(pretty)) return pretty;
            }
            return this[DBSeries.cParsedName];
        }

        public static void UpdateUnWatched(DBEpisode episode)
        {
            DBOnlineSeries series = new DBOnlineSeries(episode[DBEpisode.cSeriesID]);
            DBEpisode FirstUnwatchedEpisode = DBEpisode.GetFirstUnwatched(series[DBSeries.cID]);
            if (FirstUnwatchedEpisode != null)
                series[DBOnlineSeries.cUnwatchedItems] = true;
            else
                series[DBOnlineSeries.cUnwatchedItems] = false;
            series.Commit();
        }

        public static Dictionary<string, List<EpisodeCounter>> GetEpisodesForCount()
        {
            var episodesForCount = new Dictionary<string, List<EpisodeCounter>>();

            string selectFields = "online_episodes.SeriesID, online_episodes.EpisodeIndex, online_episodes.SeasonIndex, online_episodes.Combined_season, online_episodes.Watched";
            string query = string.Empty;
            string whereClause = string.Empty;
            var wheres = new List<string>();

            if (!DBOption.GetOptions(DBOption.cShowHiddenItems))
                wheres.Add("online_episodes.Hidden = 0");

            if (DBOption.GetOptions(DBOption.cOnlyShowLocalFiles))
                wheres.Add("local_episodes.EpisodeFilename != ''");

            if (!DBOption.GetOptions(DBOption.cCountEmptyAndFutureAiredEps))
                wheres.Add(string.Format("online_episodes.FirstAired <= '{0}' AND online_episodes.FirstAired != ''", DateTime.Now.ToString("yyyy-MM-dd")));

            if (wheres.Count > 0)
            {
                whereClause = string.Format("WHERE {0}", string.Join(" AND ", wheres.ToArray()));
            }

            if (DBOption.GetOptions(DBOption.cOnlyShowLocalFiles))
            {
                // if we are only counting episodes that have a file ie. local reference
                // then we need to join the local and online episode tables
                // further more we also need to union two select statements with
                // one returning only the first of a single/double episode and the other
                // returning the second of any double episodes
                
                query = string.Format(@"
                        SELECT {0}
                        FROM online_episodes
                        LEFT JOIN local_episodes
                        ON local_episodes.CompositeID = online_episodes.CompositeID
                        {1}
                        UNION
                        SELECT {0}
                        FROM online_episodes
                        LEFT JOIN local_episodes
                        ON local_episodes.CompositeID2 = online_episodes.CompositeID
                        {1}
                    ", selectFields, whereClause);
            }
            else
            {
                query = string.Format(@"
                        SELECT {0}
                        FROM online_episodes
                        {1}
                    ", selectFields, whereClause);
            }

            SQLiteResultSet results = DBTVSeries.Execute(query);

            foreach(var row in results.Rows)
            {
                var seriesId = row.fields[0];
                var episodeAirIdx = row.fields[1];
                var seasonAirIdx = row.fields[2];
                var seasonDvdIdx = row.fields[3];
                var watched = row.fields[4];

                if (episodesForCount.ContainsKey(seriesId))
                {
                    episodesForCount[seriesId].Add(new EpisodeCounter { EpisodeIdx = episodeAirIdx, SeasonAirIdx = seasonAirIdx, SeasonDvdIdx = seasonDvdIdx, EpisodeWatched = watched });
                }
                else
                {
                    var episodeList = new List<EpisodeCounter>();
                    episodeList.Add(new EpisodeCounter { EpisodeIdx = episodeAirIdx, SeasonAirIdx = seasonAirIdx, SeasonDvdIdx = seasonDvdIdx, EpisodeWatched = watched });
                    episodesForCount.Add(seriesId, episodeList);
                }
            }

            return episodesForCount;
        }

        public static void UpdateEpisodeCounts(DBSeries series, Dictionary<string, List<EpisodeCounter>> episodes)
        {
            if (series == null) return;

            string seriesId = series[DBSeries.cID];
            int seriesEpsTotal = 0;
            int seriesEpsUnWatched = 0;
            bool airedOrder = series.IsAiredOrder;

            // dont worry about filtering season list, we already have a filtered episode list
            // query without std conditions for faster response.
            var conditions = new SQLCondition(new DBSeason(), DBSeason.cSeriesID, seriesId, SQLConditionType.Equal); 
            var seasons = DBSeason.Get(conditions, false);

            // update season counts
            List<EpisodeCounter> eps = new List<EpisodeCounter>();
            if (episodes.TryGetValue(seriesId, out eps))
            {
                foreach(var season in seasons)
                {
                    var seasonEps = eps.Where(e => airedOrder ? e.SeasonAirIdx == season[DBSeason.cIndex] : e.SeasonDvdIdx == season[DBSeason.cIndex]).ToList();
                    
                    // dont commit seasons if are not viewing them
                    // episodes for count is already filtered so can return 0 results
                    if (seasonEps.Count == 0) continue;

                    int count = seasonEps.Count();
                    int unWatchedCount = seasonEps.Where(e => e.EpisodeWatched != "1").Count();

                    season[DBSeason.cEpisodeCount] = count;
                    season[DBSeason.cEpisodesUnWatched] = unWatchedCount;
                    season[DBSeason.cUnwatchedItems] = unWatchedCount > 0;
                    season.Commit();

                    seriesEpsTotal += count;
                    // Count the Special (Season 0 (zero)) episodes as watched!
                    if ((season[DBSeason.cIndex] != 0) || (season[DBSeason.cIndex] == 0 && !DBOption.GetOptions(DBOption.cCountSpecialEpisodesAsWatched)))
                    {
                        seriesEpsUnWatched += unWatchedCount;
                    }
                }

                // update series counts
                series[DBOnlineSeries.cEpisodeCount] = seriesEpsTotal;
                series[DBOnlineSeries.cEpisodesUnWatched] = seriesEpsUnWatched;
                series[DBOnlineSeries.cUnwatchedItems] = seriesEpsUnWatched > 0;
                series.Commit();
            }
        }

        public static void UpdateEpisodeCounts(DBSeries series)
        {
            if (series == null) return;

            int seriesEpsTotal = 0;
            int seriesEpsUnWatched = 0;
            int epsTotal = 0;
            int epsUnWatched = 0;

            // Update for each season in series and add each to total series count
            SQLCondition condition = new SQLCondition();
            if (!DBOption.GetOptions(DBOption.cShowHiddenItems)) {
                //don't include hidden seasons unless the ShowHiddenItems option is set
                condition.Add(new DBSeason(), DBSeason.cHidden, 0, SQLConditionType.Equal);
            }
            
            List<DBSeason> Seasons = DBSeason.Get(series[DBSeries.cID], condition);         
            foreach (DBSeason season in Seasons)
            {
                epsTotal = 0;
                epsUnWatched = 0;
             
                DBEpisode.GetSeasonEpisodeCounts(series, season, out epsTotal, out epsUnWatched);
                season[DBSeason.cEpisodeCount] = epsTotal;
                season[DBSeason.cEpisodesUnWatched] = epsUnWatched;
                season[DBSeason.cUnwatchedItems] = epsUnWatched > 0;
                season.Commit();

                seriesEpsTotal += epsTotal;
                // Count the Special (Season 0 (zero)) episodes as watched!
                if ((season[DBSeason.cIndex] != 0) || (season[DBSeason.cIndex] == 0 && !DBOption.GetOptions(DBOption.cCountSpecialEpisodesAsWatched)))
                {
                    seriesEpsUnWatched += epsUnWatched;
                }

                MPTVSeriesLog.Write(string.Format("Series \"{0} Season {1}\" has {2}/{3} unwatched episodes", series.ToString(), season[DBSeason.cIndex], epsUnWatched, epsTotal), MPTVSeriesLog.LogLevel.Debug);
            }

            MPTVSeriesLog.Write(string.Format("Series \"{0}\" has {1}/{2} unwatched episodes", series.ToString(), seriesEpsUnWatched, seriesEpsTotal), MPTVSeriesLog.LogLevel.Debug);
         
            series[DBOnlineSeries.cEpisodeCount] = seriesEpsTotal;
            series[DBOnlineSeries.cEpisodesUnWatched] = seriesEpsUnWatched;
            series[DBOnlineSeries.cUnwatchedItems] = seriesEpsUnWatched > 0;
            series.Commit();
        }

        public List<string> deleteSeries(TVSeriesPlugin.DeleteMenuItems type)
        {
            List<string> resultMsg = new List<string>();

            // Always delete from Local episode table if deleting from disk or database
            SQLCondition condition = new SQLCondition();
            condition.Add(new DBSeason(), DBSeason.cSeriesID, this[DBSeries.cID], SQLConditionType.Equal);
            /* TODO dunno if to include or exclude hidden items. 
             * if they are excluded then the if (resultMsg.Count is wrong and should do another select to get proper count
            if (!DBOption.GetOptions(DBOption.cShowHiddenItems))
            {
                //don't include hidden seasons unless the ShowHiddenItems option is set
                condition.Add(new DBSeason(), DBSeason.cHidden, 0, SQLConditionType.Equal);
            }
            */

            List<DBSeason> seasons = DBSeason.Get(condition, false);
            if (seasons != null)
            {
                foreach (DBSeason season in seasons)
                {
                    resultMsg.AddRange(season.deleteSeason(type));
                }
            }

            #region Facade Remote Color
            // if we were successful at deleting all episodes of series from disk, set HasLocalFiles to false
            // note: we only do this if the database entries still exist
            if (resultMsg.Count == 0 && type == TVSeriesPlugin.DeleteMenuItems.disk)
            {
                this[DBOnlineSeries.cHasLocalFiles] = false;
                this.Commit();
            }
            #endregion

            #region Cleanup
            // if there are no error messages and if we need to delete from db
            // Delete from online tables and season/series tables
            IsSeriesRemoved = false;
            if (resultMsg.Count == 0 && type != TVSeriesPlugin.DeleteMenuItems.disk)
            {
                condition = new SQLCondition();
                condition.Add(new DBSeries(), DBSeries.cID, this[DBSeries.cID], SQLConditionType.Equal);
                DBSeries.Clear(condition);

                condition = new SQLCondition();
                condition.Add(new DBOnlineSeries(), DBOnlineSeries.cID, this[DBSeries.cID], SQLConditionType.Equal);
                DBOnlineSeries.Clear(condition);

                IsSeriesRemoved = true;
            }
            #endregion

            return resultMsg;
        }

        public void HideSeries(bool hide)
        {
            MPTVSeriesLog.Write(string.Format("{0} series {1} from view", (hide ? "Hiding" : "UnHiding"), Helper.getCorrespondingSeries(this[DBSeries.cID])));
            
            // respect 'Show Local Files Only' setting
            List<DBSeason> seasons = DBSeason.Get(this[DBSeries.cID]);

            if (seasons != null)
            {
                foreach (DBSeason season in seasons)
                {
                    // Hide Seasons
                    season.HideSeason(hide);
                }
            }

            // Hide Series
            this[DBSeries.cHidden] = hide;
            // Set Scan Ignore
            if (DBOption.GetOptions(DBOption.cSetHiddenSeriesAsScanIgnore))
            {
                this[cScanIgnore] = hide;
            }
            this.Commit();
        }

    }

    public class EpisodeCounter
    {
        public string EpisodeIdx { get; set; }
        public string SeasonAirIdx { get; set; }
        public string SeasonDvdIdx { get; set; }
        public string EpisodeWatched { get; set; }
    }
}
