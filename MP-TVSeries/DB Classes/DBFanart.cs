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


using SQLite.NET;
using System;
using System.Collections.Generic;

namespace WindowPlugins.GUITVSeries
{
    public class DBFanart : DBTable, IComparable<DBFanart>
    {
        public const String cTableName = "Fanart";

        public const String cIndex = "id"; // comes from online
        public const String cSeriesID = "seriesID";
        public const String cChosen = "Chosen";
        public const String cLocalPath = "LocalPath";
        public const String cBannerPath = "BannerPath"; // online
        public const String cVignettePath = "VignettePath"; // online
        public const String cThumbnailPath = "ThumbnailPath"; // online
        public const String cColors = "Colors"; // online
        public const String cResolution = "BannerType2"; // online
        public const String cBannerType = "BannerType"; // online
        public const String cLanguage = "Language"; // online
        public const String cDisabled = "Disabled";
        public const String cSeriesName = "SeriesName"; // online
        public const String cRating = "Rating"; // online
        public const String cRatingCount = "RatingCount"; // online
        public const String cDataSource = "DataSource";

        enum FanartResolution
        {
            BOTH,
            HD,
            FULLHD
        }

        public DBFanart()
            : base(cTableName)
        {
            InitColumns();
            InitValues();
        }

        public DBFanart(long ID)
            : base(cTableName)
        {
            InitColumns();
            if (!ReadPrimary(ID.ToString()))
                InitValues();
        }

        private void InitColumns()
        {
            // all mandatory fields. WARNING: INDEX HAS TO BE INCLUDED FIRST
            AddColumn(cIndex, new DBField(DBField.cTypeInt, true));
            AddColumn(cSeriesID, new DBField(DBField.cTypeInt));
            AddColumn(cChosen, new DBField(DBField.cTypeString));
            AddColumn(cLocalPath, new DBField(DBField.cTypeString));
            AddColumn(cBannerPath, new DBField(DBField.cTypeString));
            AddColumn(cThumbnailPath, new DBField(DBField.cTypeString));
            AddColumn(cColors, new DBField(DBField.cTypeString));
            AddColumn(cDisabled, new DBField(DBField.cTypeString));
            AddColumn(cSeriesName, new DBField(DBField.cTypeString));
        }

        public static void ClearAll()
        {
            cache.Clear();
        }

        public static void ClearSeriesFromCache(int aSeriesId)
        {
            if (cache.ContainsKey(aSeriesId))
                cache.Remove( aSeriesId );
        }

        public static void Clear(int Index)
        {
            DBFanart dummy = new DBFanart(Index);
            Clear(dummy, new SQLCondition(dummy, DBFanart.cIndex, Index, SQLConditionType.Equal));
            cache.Remove(Index);
        }

        public static void ClearDB(int seriesID) {
            DBFanart dummy = new DBFanart(seriesID);
            Clear(dummy, new SQLCondition(dummy, DBFanart.cSeriesID, seriesID, SQLConditionType.Equal));
            ClearAll();
        }

        public void Delete()
        {
            // first let's delete the physical file
            if (this.IsAvailableLocally)
            {
                try
                {                    
                    System.IO.File.Delete(FullLocalPath);
                    MPTVSeriesLog.Write("Fanart Deleted: " + FullLocalPath);
                }
                catch (Exception ex)
                {
                    MPTVSeriesLog.Write("Failed to delete file: " + FullLocalPath + " (" + ex.Message + ")");
                }
            }
            Clear(this[cIndex]);         
        }
   
        public override bool Commit()
        {
            lock(cache)
            {
                if (cache.ContainsKey(this[DBFanart.cSeriesID]))
                    cache.Remove(this[DBFanart.cSeriesID]);
            }
            return base.Commit();
        }

        static Dictionary<int, List<DBFanart>> cache = new Dictionary<int,List<DBFanart>>();

        public static List<int> GetSeriesWithFanart()
        {           
            List<int> seriesids = new List<int>();

            string sqlQuery = "SELECT DISTINCT seriesID FROM Fanart";
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

        public static List<DBFanart> GetAll(int SeriesID, bool availableOnly)
        {
            lock (cache)
            {
                if (SeriesID < 0) return new List<DBFanart>();

                if (cache == null || !cache.ContainsKey(SeriesID))
                {
                    try
                    {
                        // make sure the table is created - create a dummy object
                        DBFanart dummy = new DBFanart();

                        // retrieve all fields in the table
                        String sqlQuery = "select * from " + cTableName;
                        sqlQuery += " where " + cSeriesID + " = " + SeriesID.ToString();
                        if (availableOnly)
                            sqlQuery += " and " + cLocalPath + " != ''";
                        sqlQuery += " order by " + cIndex;

                        SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
                        if (results.Rows.Count > 0)
                        {
                            List<DBFanart> ourFanart = new List<DBFanart>(results.Rows.Count);

                            for (int index = 0; index < results.Rows.Count; index++)
                            {
                                ourFanart.Add(new DBFanart());
                                ourFanart[index].Read(ref results, index);
                            }
                            if (cache == null) cache = new Dictionary<int, List<DBFanart>>();
                            cache.Add(SeriesID, ourFanart);
                        }
                        MPTVSeriesLog.Write("Found " + results.Rows.Count + " fanart from database", MPTVSeriesLog.LogLevel.Debug);

                    }
                    catch (Exception ex)
                    {
                        MPTVSeriesLog.Write("Error in DBFanart.Get (" + ex.Message + ").");
                    }
                }
                List<DBFanart> faForSeries = null;
                if (cache != null && cache.TryGetValue(SeriesID, out faForSeries))
                    return faForSeries;
                return new List<DBFanart>();
            }
        }

        public List<DBFanart> FanartsToDownload(int SeriesID)
        {       
            // Only get a list of fanart that is available for download
            String sqlQuery = "select * from " + cTableName;
            sqlQuery += " where " + cSeriesID + " = " + SeriesID.ToString();
    
            // Get Preferred Resolution
            int res = DBOption.GetOptions(DBOption.cAutoDownloadFanartResolution);
            bool getSeriesNameFanart = DBOption.GetOptions(DBOption.cAutoDownloadFanartSeriesNames);

            if (res == (int)FanartResolution.HD)
                sqlQuery += " and " + cResolution + " = " + "\"1280x720\"";
            if (res == (int)FanartResolution.FULLHD)
                sqlQuery += " and " + cResolution + " = " + "\"1920x1080\"";
            if (!getSeriesNameFanart)
                sqlQuery += " and " + cSeriesName + " != " + "\"true\"";
            
            SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);

            if (results.Rows.Count > 0)
            {                              
                int iFanartCount = 0;
                List<DBFanart> AvailableFanarts = new List<DBFanart>(results.Rows.Count);
                for (int index = 0; index < results.Rows.Count; index++)
                {
                    if (results.GetField(index, (int)results.ColumnIndices[cLocalPath]).Length > 0)
                        iFanartCount++;
                    else
                    {
                        // Add 'Available to Download' fanart to list
                        AvailableFanarts.Add(new DBFanart());
                        AvailableFanarts[AvailableFanarts.Count-1].Read(ref results, index);                       
                    }
                }
                
                // sort by highest rated
                AvailableFanarts.Sort();

                // Only return the fanarts that we want to download
                int AutoDownloadCount = DBOption.GetOptions(DBOption.cAutoDownloadFanartCount);

                for (int i = 0; i < AvailableFanarts.Count; i++)
                {
                    // Dont get more than the user wants
                    if (iFanartCount >= AutoDownloadCount)
                        break;
                    _FanartsToDownload.Add(AvailableFanarts[i]);
                    iFanartCount++;
                }
            }
            return _FanartsToDownload;
          
        } List<DBFanart> _FanartsToDownload = new List<DBFanart>();

        /// <summary>
        /// Checks if a Series Fanart contains a Series Name
        /// </summary>
        public bool HasSeriesName {
            get {
                if (this[cSeriesName] = "true")
                    return true;
                else
                    return false;
            }            
        }

        public bool Chosen
        {
            get
            {
                return this[cChosen];
            }
            set
            {
                GlobalSet(new DBFanart(), cChosen, false, new SQLCondition(new DBFanart(), cSeriesID, this[cSeriesID], SQLConditionType.Equal));
                this[cChosen] = value;
                this.Commit();
            }
        }

        public bool Disabled
        {
            get
            {
                if (this[cDisabled])
                    return true;
                return false;
            }  
            set
            {                
                this[cDisabled] = value;
                this.Commit();
            }
        }

        public bool IsAvailableLocally
        {
            get
            {
                if(String.IsNullOrEmpty(this[DBFanart.cLocalPath])) return false;
                
                // Check if file in path exists, remove it from database if not
                if (System.IO.File.Exists(Settings.GetPath(Settings.Path.fanart) + @"\" + this[DBFanart.cLocalPath])) return true;
                this[DBFanart.cLocalPath] = string.Empty;
                return false;
            }
        }

        public string FullLocalPath
        {
            get
            {
                if (String.IsNullOrEmpty(this[cLocalPath])) return string.Empty;
                return Helper.PathCombine(Settings.GetPath(Settings.Path.fanart), this[cLocalPath]);
            }
        }

        public bool HasColorInfo
        {
            get
            {
                return !String.IsNullOrEmpty(this[cColors]);
            }
        }

        public System.Drawing.Color GetColor(int which)
        {
            if (HasColorInfo && which <= 3 && which > 0)
            {
                string[] split = this[cColors].ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 3) return default(System.Drawing.Color);
                string[] rgbValues = split[--which].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                return System.Drawing.Color.FromArgb(100, Int32.Parse(rgbValues[0]), Int32.Parse(rgbValues[1]), Int32.Parse(rgbValues[2]));
            }
            else return default(System.Drawing.Color);
        }

        public override string ToString()
        {
            return this[cSeriesID] + " -> " + this[cIndex];
        }

        #region Comparison
        public override bool Equals( Object obj )
        {
            if ( obj == null )
                return false;

            DBFanart f = obj as DBFanart;
            if ( ( Object )f == null )
                return false;

            return ( this[DBFanart.cThumbnailPath] == f[DBFanart.cThumbnailPath] );
        }
        
        public override int GetHashCode()
        {
            return this[DBFanart.cThumbnailPath];
        }
        #endregion

        #region IComparable
        public int CompareTo(DBFanart other)
        {
            // Sort by:
            // Number of Votes (AKA. how many times it's been favourited)

            double thisFanart = 0.0;
            double otherFanart = 0.0;
            
            thisFanart += this[cRatingCount];
            otherFanart += other[cRatingCount];

            return otherFanart.CompareTo(thisFanart);
        }
        #endregion
    }
}
