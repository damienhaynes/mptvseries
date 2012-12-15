using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using SQLite.NET;
using MediaPortal.Database;
using WindowPlugins.GUITVSeries.GUI;

namespace WindowPlugins.GUITVSeries
{
    class DBActor : DBTable, INotifyPropertyChanged
    {
        #region Variables
        public const String cTableName = "Actors";

        private static Dictionary<int, List<DBActor>> _cache = new Dictionary<int, List<DBActor>>();
        #endregion

        # region Online Fields
        public const String cIndex = "id";
        public const String cImage = "Image";
        public const String cName = "Name";
        public const String cRole = "Role";
        public const String cSortOrder = "SortOrder";
        #endregion

        # region Local Fields
        public const String cSeriesID = "SeriesId";
        #endregion

        #region Constructors
        public DBActor() : base(cTableName)
        {
            InitColumns();
            InitValues();
        }

        public DBActor(long ID) : base(cTableName)
        {
            InitColumns();
            if (!ReadPrimary(ID.ToString()))
                InitValues();
        }
        #endregion

        #region Private Methods
        private void InitColumns()
        {
            AddColumn(cIndex, new DBField(DBField.cTypeInt, true));
            AddColumn(cSeriesID, new DBField(DBField.cTypeInt));
            AddColumn(cImage, new DBField(DBField.cTypeString));
            AddColumn(cName, new DBField(DBField.cTypeString));
            AddColumn(cRole, new DBField(DBField.cTypeString));
            AddColumn(cSortOrder, new DBField(DBField.cTypeInt));
            
        }
        #endregion

        #region Public Methods
        public static void ClearAll()
        {
            _cache.Clear();
        }

        public static void Clear(int Index)
        {
            DBActor actor = new DBActor(Index);
            Clear(actor, new SQLCondition(actor, cIndex, Index, SQLConditionType.Equal));
            _cache.Remove(Index);
        }

        public static void ClearDB(int seriesID)
        {
            DBActor actors = new DBActor(seriesID);
            Clear(actors, new SQLCondition(actors, cSeriesID, seriesID, SQLConditionType.Equal));
            
            // clear the cache so that we dont accidently get invalid entries
            ClearAll();
        }

        public override bool Commit()
        {
            lock (_cache)
            {
                if (_cache.ContainsKey(this[DBActor.cSeriesID]))
                    _cache.Remove(this[DBActor.cSeriesID]);
            }
            return base.Commit();
        }

        public static List<DBActor> GetAll(int seriesID)
        {
            lock (_cache)
            {
                if (_cache == null || !_cache.ContainsKey(seriesID))
                {
                    try
                    {
                        // make sure the table is created
                        DBActor actor = new DBActor();

                        // retrieve all fields in the table
                        String sqlQuery = "select * from " + cTableName;
                        if (seriesID > 0)
                        {
                            sqlQuery += " where " + cSeriesID + " = " + seriesID.ToString();                            
                        }
                        sqlQuery += " order by " + cIndex;

                        SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
                        if (results.Rows.Count > 0)
                        {
                            List<DBActor> actors = new List<DBActor>(results.Rows.Count);

                            for (int index = 0; index < results.Rows.Count; index++)
                            {
                                actors.Add(new DBActor());
                                actors[index].Read(ref results, index);
                            }

                            if (_cache == null) 
                                _cache = new Dictionary<int, List<DBActor>>();
                            
                            _cache.Add(seriesID, actors);
                        }
                        MPTVSeriesLog.Write("Found " + results.Rows.Count + " Actors from Database", MPTVSeriesLog.LogLevel.Debug);

                    }
                    catch (Exception ex)
                    {
                        MPTVSeriesLog.Write("Error in DBActors.GetAll (" + ex.Message + ")");
                    }
                }

                List<DBActor> seriesActors = null;
                if (_cache != null && _cache.TryGetValue(seriesID, out seriesActors))
                    return seriesActors;
                
                return new List<DBActor>();
            }
        }        

        public override string ToString()
        {
            return Name + " as " + Role;
        }
        
        #endregion

        #region Public Properties
        public int SeriesId 
        {
            get { return this[cSeriesID]; } 
        }

        public string Name 
        { 
            get { return this[cName]; }
        }

        public string Role 
        { 
            get { return this[cRole]; }
        }

        public string Image
        {
            get
            {
                string seriesName = Helper.getCorrespondingSeries(this[cSeriesID]).ToString();
                string seriesPath = Helper.PathCombine(Settings.GetPath(Settings.Path.banners), Helper.cleanLocalPath(seriesName));

                return Helper.PathCombine(seriesPath, this[cImage]);
            }            
        }

        public string ImageRemotePath
        {
            get
            {
                if (string.IsNullOrEmpty(this[cImage])) 
                    return string.Empty;
                return Helper.PathCombine("http://thetvdb.com/banners/", this[cImage]);
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public string ThumbnailImage { get; set; }

        /// <summary>
        /// Notify ThumbnailImage property change during async image downloading
        /// Sends messages to facade to update image
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        internal static List<int> GetSeriesWithActors()
        {
            List<int> seriesids = new List<int>();

            string sqlQuery = "SELECT DISTINCT seriesID FROM Actors";
            SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);

            if (results.Rows.Count > 0)
            {
                for (int index = 0; index < results.Rows.Count; index++)
                {
                    int result = 0;
                    if (int.TryParse(results.Rows[index].fields[0], out result))
                        seriesids.Add(result);
                }
            }
            return seriesids;
        }
    }
}
