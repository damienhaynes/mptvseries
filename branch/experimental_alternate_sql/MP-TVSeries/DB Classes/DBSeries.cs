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
using System.Text;
//using SQLite.NET;
using System.Data;
//using MediaPortal.Database;
using System.IO;
using System.Xml;

namespace WindowPlugins.GUITVSeries
{
    public class DBOnlineSeries : DBTable
    {
        public const String cTableName = "online_series";

        public const String cID = "ID";
        public const String cPrettyName = "Pretty_Name";
        public const String cSortName = "SortName";
        public const String cStatus = "Status";
        public const String cGenre = "Genre";
        public const String cSummary = "Summary";
        public const String cAirsDay = "AirsDay";
        public const String cAirsTime = "AirsTime";
        public const String cActors = "Actors";
        public const String cBannerFileNames = "BannerFileNames";
        public const String cCurrentBannerFileName = "CurrentBannerFileName";
        public const String cPosterFileNames = "PosterFileNames";
        public const String cCurrentPosterFileName = "PosterBannerFileName";
        public const String cBannersDownloaded = "BannersDownloaded";
        public const String cHasLocalFiles = "HasLocalFiles";
        public const String cHasLocalFilesTemp = "HasLocalFiles_Temp";
        public const String cOnlineDataImported = "OnlineDataImported";
        // Online data imported flag values while updating:
        // 0: the series just got an ID, the update series hasn't run on it yet
        // 1: online data is marked as "old", and needs a refresh
        // 2: online data up to date.
        public const String cGetEpisodesTimeStamp = "GetEpisodesTimeStamp";
        public const String cUpdateBannersTimeStamp = "UpdateBannersTimeStamp";
        public const String cWatchedFileTimeStamp = "WatchedFileTimeStamp";

        public const String cIsFavourite = "isFavourite";
        public const String cIsOnlineFavourite = "isOnlineFavourite";
        public const String cUnwatchedItems = "UnwatchedItems";

        public const String cEpisodeOrders = "EpisodeOrders";
        public const String cChoseEpisodeOrder = "choosenOrder";

        public const String cOriginalName = "origName";

        public const String cTaggedToDownload = "taggedToDownload";

        public const String cRating = "Rating";
        public const String cMyRating = "myRating";

        public const String cEpisodeCount = "EpisodeCount";
        public const String cEpisodesUnWatched = "EpisodesUnWatched";

        public const int cDBVersion = 2;

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
            s_FieldToDisplayNameMap.Add(cPosterFileNames, "Poster FileName List");
            s_FieldToDisplayNameMap.Add(cCurrentPosterFileName, "Current Poster FileName");
            s_FieldToDisplayNameMap.Add(cAirsDay, "Week Day Aired");
            s_FieldToDisplayNameMap.Add(cAirsTime, "Hour Aired");
            s_FieldToDisplayNameMap.Add(cSortName, "Sort (Original) Name");
           
            s_OnlineToFieldMap.Add("id", cID);            
            s_OnlineToFieldMap.Add("SeriesName", cPrettyName);
            s_OnlineToFieldMap.Add("Status", cStatus);
            s_OnlineToFieldMap.Add("Genre", cGenre);
            s_OnlineToFieldMap.Add("Overview", cSummary);
            s_OnlineToFieldMap.Add("Airs_DayOfWeek", cAirsDay);
            s_OnlineToFieldMap.Add("Airs_Time", cAirsTime);
            s_OnlineToFieldMap.Add("SortName", cSortName);            

            // make sure the table is created on first run
            DBOnlineSeries dummy = new DBOnlineSeries();

            int nCurrentDBVersion = cDBVersion;
            int nUpgradeDBVersion = DBOption.GetOptions(DBOption.cDBOnlineSeriesVersion);

            while (nUpgradeDBVersion != nCurrentDBVersion)
            {                
                List<DBOnlineSeries> AllSeries = getAllSeries();

                // take care of the upgrade in the table    
                switch (nUpgradeDBVersion)
                {
                    case 1:                        
                        nUpgradeDBVersion++;
                        break;

                    default:                        
                        if (AllSeries.Count > 0)
                        {
                            foreach (DBOnlineSeries series in AllSeries)
                            {
                                // Migrate old cBannerFileNames and cCurrentBannerFileName to cPosterFileNames and cCurrentPosterFileName
                                // if were using posters previously
                                if (series[DBOnlineSeries.cCurrentBannerFileName].ToString().Contains("-posters"))
                                {
                                    series[DBOnlineSeries.cCurrentPosterFileName] = series[DBOnlineSeries.cCurrentBannerFileName];
                                    series[DBOnlineSeries.cPosterFileNames] = series[DBOnlineSeries.cBannerFileNames];
                                    // clear old ones
                                    series[DBOnlineSeries.cCurrentBannerFileName] = string.Empty;
                                    series[DBOnlineSeries.cBannerFileNames] = string.Empty;
                                    series.Commit();
                                }                                                                
                            }
                        }
                        // new DB, nothing special to do
                        nUpgradeDBVersion = nCurrentDBVersion;
                        break;
                }
            }
            DBOption.SetOptions(DBOption.cDBOnlineSeriesVersion, nCurrentDBVersion);

        }

        // returns a list of all series with information stored in the database. 
        public static List<DBOnlineSeries> getAllSeries() {
            List<DBValue> seriesIDs = DBOnlineSeries.GetSingleField(DBOnlineSeries.cID, new SQLCondition(), new DBOnlineSeries());
            List<DBOnlineSeries> rtn = new List<DBOnlineSeries>();

            foreach (DBValue currSeriesID in seriesIDs) {
                rtn.Add(new DBOnlineSeries(currSeriesID));
            }

            return rtn;
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
            base.AddColumn(cPrettyName, new DBField(DBField.cTypeString, 100));
            base.AddColumn(cSortName, new DBField(DBField.cTypeString, 100));
            base.AddColumn(cStatus, new DBField(DBField.cTypeString, 20));
            base.AddColumn(cGenre, new DBField(DBField.cTypeString, 100));
            base.AddColumn(cBannerFileNames, new DBField(DBField.cTypeString, DBField.cMaxLength));
            base.AddColumn(cCurrentBannerFileName, new DBField(DBField.cTypeString, 200));
            base.AddColumn(cPosterFileNames, new DBField(DBField.cTypeString, DBField.cMaxLength));
            base.AddColumn(cCurrentPosterFileName, new DBField(DBField.cTypeString, 200));
            base.AddColumn(cSummary, new DBField(DBField.cTypeString, DBField.cMaxLength));
            base.AddColumn(cOnlineDataImported, new DBField(DBField.cTypeInt));
            base.AddColumn(cAirsDay, new DBField(DBField.cTypeString, 20));
            base.AddColumn(cAirsTime, new DBField(DBField.cTypeString, 20));
            base.AddColumn(cActors, new DBField(DBField.cTypeString, DBField.cMaxLength));
            base.AddColumn(cBannersDownloaded, new DBField(DBField.cTypeInt));
            base.AddColumn(cHasLocalFiles, new DBField(DBField.cTypeInt));
            base.AddColumn(cHasLocalFilesTemp, new DBField(DBField.cTypeInt));
            base.AddColumn(cGetEpisodesTimeStamp, new DBField(DBField.cTypeInt));
            base.AddColumn(cUpdateBannersTimeStamp, new DBField(DBField.cTypeInt));
            base.AddColumn(cIsFavourite, new DBField(DBField.cTypeString, 5));
            base.AddColumn(cWatchedFileTimeStamp, new DBField(DBField.cTypeInt));
            base.AddColumn(cUnwatchedItems, new DBField(DBField.cTypeInt));
            base.AddColumn(cEpisodeCount, new DBField(DBField.cTypeInt));
            base.AddColumn(cEpisodesUnWatched, new DBField(DBField.cTypeInt));

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

        public override DBValue this[String fieldName]
        {
            get
            {
                switch (fieldName)
                {
                    // forom subtitle retrieval always needs original (english) series title
                    // if the user choose a different language for the import, we don't have this as the prettyname
                    case DBOnlineSeries.cOriginalName:
                        string origLanguage = "7"; // 7 = english (original)
                        if (DBOption.GetOptions(DBOption.cOnlineLanguage) == origLanguage)
                            return base[DBOnlineSeries.cPrettyName];
                        else
                        {
                            if (base[DBOnlineSeries.cOriginalName].ToString().Length > 0)
                                return base[DBOnlineSeries.cOriginalName];
                            else
                            {
                                // we need to get it
                                MPTVSeriesLog.Write("Retrieving original Series Name...");
                                UpdateSeries origParser = null; //= new UpdateSeries(base[DBOnlineSeries.cID], 0, origLanguage); // doesn't work anymore
                                if (origParser != null && origParser.Results.Count == 1)
                                {
                                    base[DBOnlineSeries.cOriginalName] = origParser.Results[0][DBOnlineSeries.cPrettyName];
                                    Commit(); // save for next time
                                    MPTVSeriesLog.Write("Original Series Name retrieved");
                                    return origParser.Results[0][DBOnlineSeries.cPrettyName];
                                }
                                else
                                {
                                    MPTVSeriesLog.Write("Original Series Name could not be retrieved");
                                    // something wrong
                                    return base[DBOnlineSeries.cPrettyName];
                                }
                            }
                        }
                    default:
                        return base[fieldName];
                }
            }
            set
            {
                base[fieldName] = value;                           
            }
        }

        /// <summary>
        /// Returns PrettyName
        /// </summary>
        /// <returns>PrettyName</returns>
        public override string ToString()
        {
            return this[cPrettyName];
        }
    };

    public class DBSeries : DBTable
    {
        public delegate void dbSeriesUpdateOccuredDelegate(DBSeries updated);
        public static event dbSeriesUpdateOccuredDelegate dbSeriesUpdateOccured;

        public const String cTableName = "local_series";
        public const String cOutName = "Series";
        public const int cDBVersion = 11;

        public const String cParsedName = "Parsed_Name";
        public const String cID = "ID";
        public const String cScanIgnore = "ScanIgnore";
        public const String cDuplicateLocalName = "DuplicateLocalName";
        public const String cHidden = "Hidden";

        private DBOnlineSeries m_onlineSeries = null;
        new public static List<string> FieldsRequiringSplit = new List<string>(new string[] { "Genre", "Actors", "Network" });
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

            DBTVSeries.CreateDBIndices("create index seriesIDLocal on local_series(ID ASC)", m_tableName, "seriesIDLocal", false);
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
                if (Helper.String.IsNullOrEmpty(m_onlineSeries[DBOnlineSeries.cPrettyName]))
                {
                    m_onlineSeries[DBOnlineSeries.cPrettyName] = base[cParsedName];
                    m_onlineSeries[DBOnlineSeries.cSortName] = base[cParsedName];
                    m_onlineSeries.Commit();
                }
            }
            else
            {
                m_onlineSeries = new DBOnlineSeries(base[cID]);
            }
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST ( I suck at SQL )
            base.AddColumn(cParsedName, new DBField(DBField.cTypeString, true, 200));
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
            newOnlineSeries[DBOnlineSeries.cChoseEpisodeOrder] = m_onlineSeries[DBOnlineSeries.cChoseEpisodeOrder];
            m_onlineSeries = newOnlineSeries;
            base[cID] = nSeriesID;
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

                        if (Helper.String.IsNullOrEmpty(retVal))
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
                    if (DBOption.GetOptions(DBOption.cRandomBanner) == true) return getRandomBanner(BannerList);
                    if (Helper.String.IsNullOrEmpty(m_onlineSeries[DBOnlineSeries.cCurrentBannerFileName]))
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
                    if (DBOption.GetOptions(DBOption.cRandomBanner) == true) return getRandomBanner(PosterList);
                    if (Helper.String.IsNullOrEmpty(m_onlineSeries[DBOnlineSeries.cCurrentPosterFileName]))
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
                    if (Helper.String.IsNullOrEmpty(sList))
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
                    String sIn = String.Empty;
                    for (int i = 0; i < value.Count; i++)
                    {
                        value[i] = value[i].Replace(Settings.GetPath(Settings.Path.banners), "");
                        if (Helper.String.IsNullOrEmpty(String.Empty))
                            sIn += value[i];
                        else
                            sIn += "," + value[i];
                    }
                    m_onlineSeries[DBOnlineSeries.cBannerFileNames] = sIn;
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
                    if (Helper.String.IsNullOrEmpty(sList))
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
                    String sIn = String.Empty;
                    for (int i = 0; i < value.Count; i++)
                    {
                        value[i] = value[i].Replace(Settings.GetPath(Settings.Path.banners), "");
                        if (Helper.String.IsNullOrEmpty(String.Empty))
                            sIn += value[i];
                        else
                            sIn += "," + value[i];
                    }
                    m_onlineSeries[DBOnlineSeries.cPosterFileNames] = sIn;
                }
            }
        }

        public override bool Commit()
        {
            if (m_onlineSeries != null)
                m_onlineSeries.Commit();

            if (dbSeriesUpdateOccured != null)
                dbSeriesUpdateOccured(this);
            return base.Commit();
        }

        public void toggleFavourite()
        {
            if (this.m_onlineSeries == null) return; // sorry, can only add online series as Favs. for now
            if (!DBOption.GetOptions(DBOption.cOnlineFavourites))
            {
                this.m_onlineSeries[DBOnlineSeries.cIsFavourite] = !(bool)this.m_onlineSeries[DBOnlineSeries.cIsFavourite];
            }
            else
            {
                this.m_onlineSeries[DBOnlineSeries.cIsOnlineFavourite] = !(bool)this.m_onlineSeries[DBOnlineSeries.cIsOnlineFavourite];
                // Update online (add/remove)
                Online_Parsing_Classes.OnlineAPI.ConfigureFavourites((bool)this.m_onlineSeries[DBOnlineSeries.cIsOnlineFavourite], DBOption.GetOptions(DBOption.cOnlineUserID), this.m_onlineSeries[DBOnlineSeries.cID]);
            }

            this.m_onlineSeries.Commit();

            if (dbSeriesUpdateOccured != null)
                dbSeriesUpdateOccured(this);
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
                if (!Settings.isConfig || !DBOption.GetOptions(DBOption.cShowHiddenItems))
                    conditions.Add(new DBSeries(), DBSeries.cHidden, 0, SQLConditionType.Equal);

                if (!Settings.isConfig && DBOption.GetOptions(DBOption.cView_Episode_OnlyShowLocalFiles) && !conditions.ConditionsSQLString.Contains(DBEpisode.cTableName))
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
                bool bUseSortName = DBOption.GetOptions(DBOption.cSeries_UseSortName);
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
            DataTable results = DBTVSeries.Execute(sqlQuery);
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
                if (!Helper.String.IsNullOrEmpty(pretty)) return pretty;
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

        public static void UpdatedEpisodeCounts(DBSeries series)
        {
            int epsTotal = 0;
            int epsUnWatched = 0;

            DBEpisode.GetSeriesEpisodeCounts(series[DBSeries.cID], out epsTotal, out epsUnWatched);
            series[DBOnlineSeries.cEpisodeCount] = epsTotal;
            series[DBOnlineSeries.cEpisodesUnWatched] = epsUnWatched;
            series.Commit();
    
            // Now Update for each season in series
            List<DBSeason> Seasons = DBSeason.Get(series[DBSeries.cID]);
            foreach (DBSeason season in Seasons)
            {
                epsTotal = 0;
                epsUnWatched = 0;
             
                DBEpisode.GetSeasonEpisodeCounts(season, out epsTotal, out epsUnWatched);
                season[DBSeason.cEpisodeCount] = epsTotal;
                season[DBSeason.cEpisodesUnWatched] = epsUnWatched;
                season.Commit();
            }
        }

    }
}
