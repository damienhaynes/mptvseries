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

namespace WindowPlugins.GUITVSeries
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Interface needed to be Implemented by a class in order to be cachable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICacheable<T>
    {
        T fullItem
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Is used to keep objects in memory to avoid havint to do some SQL queries
    /// particularly when hierarchical information would need to be loaded additionally
    /// after the main query (logos/flat views, etc)
    /// </summary>
    public class cache
    {
        #region Main Object Declarations
        /// <summary>
        /// The cache object, containing all series/seasons/episodes that are in cache!
        /// </summary>
        static hierarchyCache<int, // top level
                    hierarchyCache<DBSeries, // series
                        hierarchyCache<DBSeason, // seasons
                            DBEpisode, DBEpisode>, // episodes
                    DBSeason>, 
                DBSeries> _cache =
            new hierarchyCache<int, hierarchyCache<DBSeries, hierarchyCache<DBSeason, DBEpisode, DBEpisode>, DBSeason>, DBSeries>();


        static hierarchyCache<DBSeries, hierarchyCache<DBSeason, DBEpisode, DBEpisode>, DBSeason> singleSeriesRef;
        static hierarchyCache<DBSeason, DBEpisode, DBEpisode> singleSeasonRef;
        #endregion

        #region Setup Variables
        public const int maxNoObjects = 750; // if this number is reached the entire cache is dumped and started over
        public static int currNoObjects; // (typically this will be in the 100-200 range at the most)
        public static int Reads;
        public static int Writes;
        public static int Mods;
        #endregion

        #region Constructor
        static cache()
        {
            if (!Settings.isConfig)
            {
                // for globalset and clear we dumb all objects in question
                DBTable.dbUpdateOccured += new DBTable.dbUpdateOccuredDelegate(DBTable_dbUpdateOccured);
                // for commits on single object we can update the specific object in question
                // (if commited ones are not yet in cache we don't add them though)
                DBSeries.dbSeriesUpdateOccured += new DBSeries.dbSeriesUpdateOccuredDelegate(DBSeries_dbSeriesUpdateOccured);
                DBSeason.dbSeasonUpdateOccured += new DBSeason.dbSeasonUpdateOccuredDelegate(DBSeason_dbSeasonUpdateOccured);
                DBEpisode.dbEpisodeUpdateOccured += new DBEpisode.dbEpisodeUpdateOccuredDelegate(DBEpisode_dbEpisodeUpdateOccured);

                MPTVSeriesLog.Write("Cache Initialized",MPTVSeriesLog.LogLevel.Debug);
            }
        }
        #endregion

        #region Update Event Handlers
        static void DBEpisode_dbEpisodeUpdateOccured(DBEpisode updated)
        {
            //MPTVSeriesLog.Write("Cache: Episode Commit occured: ", updated.ToString(), MPTVSeriesLog.LogLevel.Debug);
            if (getEpisode(updated[DBEpisode.cSeriesID], updated[DBEpisode.cSeasonIndex], updated[DBEpisode.cEpisodeIndex]) != null)
                addChangeEpisode(updated);
        }

        static void DBSeason_dbSeasonUpdateOccured(DBSeason updated)
        {
            //MPTVSeriesLog.Write("Cache: Season Commit occured: ", updated.ToString(), MPTVSeriesLog.LogLevel.Debug);
            if (getSeason(updated[DBSeason.cSeriesID], updated[DBSeason.cIndex]) != null)
                addChangeSeason(updated);
        }

        static void DBSeries_dbSeriesUpdateOccured(DBSeries updated)
        {
            //MPTVSeriesLog.Write("Cache: Series Commit occured: ",  updated.ToString(), MPTVSeriesLog.LogLevel.Debug);
            if (_cache.getItemOfSubItem(updated[DBSeries.cID]) != null) 
                addChangeSeries(updated);
        }

        static void DBTable_dbUpdateOccured(string table)
        {
            if (_cache == null)
                return;

            MPTVSeriesLog.Write("Cache: DB Write operation: ", table, MPTVSeriesLog.LogLevel.Debug);
            switch (table)
            {
                case DBSeries.cTableName:
                case DBOnlineSeries.cTableName:
                    _cache.dummySubItems(null);
                    break;
                case DBEpisode.cTableName:
                case DBOnlineEpisode.cTableName:
                    foreach (KeyValuePair<int, hierarchyCache<DBSeries, hierarchyCache<DBSeason, DBEpisode, DBEpisode>, DBSeason>> sub in _cache.subItems)
                    {
                        foreach (KeyValuePair<int, hierarchyCache<DBSeason, DBEpisode, DBEpisode>> sub2 in sub.Value.subItems)
                            sub2.Value.dummySubItems(null);
                    }
                    break;
                case DBSeason.cTableName:
                    foreach (KeyValuePair<int, hierarchyCache<DBSeries, hierarchyCache<DBSeason, DBEpisode, DBEpisode>, DBSeason>> sub in _cache.subItems)
                    {
                        sub.Value.dummySubItems(null);
                    }
                    break;
            }
        }
        #endregion

        #region Retrieval Methods
        public static DBEpisode getEpisode(int SeriesID, int SeasonIndex, int EpisodeIndex)
        {
            singleSeriesRef = _cache.getSubItem(SeriesID);
            DBEpisode e = null;
            if (singleSeriesRef != null)
            {
                singleSeasonRef = singleSeriesRef.getSubItem(SeasonIndex);
                e = singleSeasonRef == null ? null : singleSeasonRef.getItemOfSubItem(EpisodeIndex);
            }
            //MPTVSeriesLog.Write("Cache: Requested Episode: " + SeriesID.ToString() + " S" + SeasonIndex.ToString() + " E" + EpisodeIndex.ToString() + (e == null ? " - Failed" : " - Sucess"), MPTVSeriesLog.LogLevel.Debug);
            return e;

        }

        public static DBSeries getSeries(int SeriesID)
        {
           DBSeries s = _cache.getItemOfSubItem(SeriesID);
           //MPTVSeriesLog.Write("Cache: Requested Series: " + SeriesID.ToString() + (s == null ? " - Failed" : " - Sucess"), MPTVSeriesLog.LogLevel.Debug);
           return s;
        }

        public static DBSeason getSeason(int SeriesID, int SeasonIndex)
        {
            singleSeriesRef = _cache.getSubItem(SeriesID);
            DBSeason s = singleSeriesRef == null ? null : singleSeriesRef.getItemOfSubItem(SeasonIndex);
            //MPTVSeriesLog.Write("Cache: Requested Season: " + SeriesID.ToString() + " S" + SeasonIndex.ToString() + (s == null ? " - Failed" : " - Sucess", MPTVSeriesLog.LogLevel.Debug));
            return s;
        }
        #endregion

        #region Add Methods
        public static void addChangeEpisode(DBEpisode episode)
        {
            if (episode == null) return;
            _cache.AddDummy(episode[DBSeason.cSeriesID]);
            singleSeriesRef = _cache.getSubItem(episode[DBSeason.cSeriesID]);
            if (singleSeriesRef == null) return;
            singleSeriesRef.AddDummy(episode[DBSeason.cIndex]);
            // use addRaw for episode!!
            //MPTVSeriesLog.Write("Cache: Adding/Changing Episode: " + episode[DBEpisode.cCompositeID], MPTVSeriesLog.LogLevel.Debug);
            var hc = singleSeriesRef.getSubItem(episode[DBSeason.cIndex]);
            if (hc != null) hc.AddRaw(episode[DBEpisode.cEpisodeIndex], episode);
        }

        public static void addChangeSeries(DBSeries series)
        {
            if (series == null) return;
            //MPTVSeriesLog.Write("Cache: Adding/Changing Series: " + series[DBSeries.cID], MPTVSeriesLog.LogLevel.Debug);
            _cache.Add(series[DBSeries.cID], series);
        }

        public static void addChangeSeason(DBSeason season)
        {
            if (season == null) return;
            // does the series exist already, if not create a dummy
            _cache.AddDummy(season[DBSeason.cSeriesID]);
            //MPTVSeriesLog.Write("Cache: Adding/Changing Season: " + season[DBSeason.cSeriesID] + " S" + season[DBSeason.cIndex], MPTVSeriesLog.LogLevel.Debug);
            var hc = _cache.getSubItem(season[DBSeason.cSeriesID]);
            if (hc != null) hc.Add(season[DBSeason.cIndex], season);
        }
        #endregion

        #region Impl. Methods
        static void sizeChanged()
        {
            if (currNoObjects > maxNoObjects)
            {
                dump();
                // empty cache (yes completely, I know its not ideal)
                MPTVSeriesLog.Write("Cache: Dumped....was getting too big (" + maxNoObjects.ToString() + " objects)");
            }
        }

        public static void dump()
        {
            _cache.dump();
        }
        #endregion

        #region HierarchyCache Implementation
        class hierarchyCache<T, S, M> : ICacheable<T> where S : ICacheable<M>, new()
        {
            System.Threading.ReaderWriterLockSlim _lock = new System.Threading.ReaderWriterLockSlim();
            T _Item = default(T);
            public Dictionary<int, S> subItems = new Dictionary<int, S>();

            public void dump()
            {
                currNoObjects -= subItems.Count;
                subItems.Clear();
                subItems = new Dictionary<int, S>();
            }

            public void dummySubItems(M dummy)
            {
                try
                {
                    _lock.EnterReadLock();
                    foreach (KeyValuePair<int, S> sub in subItems)
                    {
                        currNoObjects--;
                        sub.Value.fullItem = dummy;
                    }
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }

            public T fullItem
            {
                get { return _Item; }
                set { _Item = value; }
            }

            public S getSubItem(int index)
            {
                S result = default(S);
                try
                {
                    _lock.EnterReadLock();
                    if (subItems.TryGetValue(index, out result))
                        return result;
                } 
                finally {
                    _lock.ExitReadLock();
                }
                return default(S);
            }

            public M getItemOfSubItem(int index)
            {
                S rawSubItem = getSubItem(index);
                return rawSubItem == null ? default(M) : rawSubItem.fullItem;
            }

            public bool ContainsKey(int key)
            {
                return subItems.ContainsKey(key);
            }

            public void AddRaw(int key, S subItem)
            {
                if (subItem == null)
                    return;
                try
                {
                    _lock.EnterUpgradeableReadLock();
                    if (!subItems.ContainsKey(key))
                    {
                        try
                        {
                            _lock.EnterWriteLock();
                            subItems.Add(key, subItem);
                        }
                        finally
                        {
                            _lock.ExitWriteLock();
                        }
                        currNoObjects++;
                        cache.sizeChanged();
                    }
                    else
                    {
                        subItems[key] = subItem;
                    }
                }
                finally
                {
                    _lock.ExitUpgradeableReadLock();
                }
            }

            public void Add(int key, M subItem)
            {
                if (subItem == null)
                    return;

                try
                {
                    _lock.EnterUpgradeableReadLock();
                    if (!subItems.ContainsKey(key))
                    {
                        S rawsubItem = new S();
                        rawsubItem.fullItem = subItem;
                        try
                        {
                            _lock.EnterWriteLock();
                            subItems.Add(key, rawsubItem);
                        }
                        finally
                        {
                            _lock.ExitWriteLock();
                        }
                        if (subItem != null)
                        {
                            currNoObjects++;
                            cache.sizeChanged();
                        }
                    }
                    else
                    {
                        subItems[key].fullItem = subItem;
                    }
                }
                finally
                {
                    _lock.ExitUpgradeableReadLock();
                }
            }

            public void AddDummy(int key)
            {
                try
                {
                    _lock.EnterUpgradeableReadLock();
                    if (!subItems.ContainsKey(key))
                    {
                        try
                        {
                            _lock.EnterWriteLock();
                            subItems.Add(key, new S());
                        }
                        finally
                        {
                            _lock.ExitWriteLock();
                        }
                    }
                }
                finally
                {
                    _lock.ExitUpgradeableReadLock();
                }
            }

            public override string ToString()
            {
                return fullItem.ToString() + " Subitems: " + subItems.Count.ToString();
            }
        }
        #endregion
    }
}
