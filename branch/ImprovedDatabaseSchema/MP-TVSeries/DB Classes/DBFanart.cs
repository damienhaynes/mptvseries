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
using SQLite.NET;
using WindowPlugins.GUITVSeries.DataBase;

namespace WindowPlugins.GUITVSeries.DataClass
{
    public class DBFanart : DBTable
    {
        public const String cTableName = "Fanart";

		#region Local DB Fields
		//declare fieldsnames as constants here, and then add them to TableFields
		public const String cIndex = "id"; // comes from online
        public const String cSeriesID = "seriesID";
        public const String cChosen = "Chosen";
        public const String cLocalPath = "LocalPath";
        public const String cBannerPath = "BannerPath"; // online
        public const String cThumbnailPath = "ThumbnailPath"; // online
        public const String cColors = "Colors"; // online
        public const String cResolution = "BannerType2"; // online
        public const String cDisabled = "Disabled";
        public const String cSeriesName = "SeriesName"; // online

		// all mandatory fields. Place the primary key first - it's just good manners
		public static readonly DBFieldDefList TableFields = new DBFieldDefList {
                    {cIndex,        new DBFieldDef{ FieldName = cIndex,         TableName = cTableName, Type = DBFieldType.Int,     Primary = true }},
                    {cSeriesID,     new DBFieldDef{ FieldName = cSeriesID,      TableName = cTableName, Type = DBFieldType.Int}},
                    {cChosen,       new DBFieldDef{ FieldName = cChosen,        TableName = cTableName, Type = DBFieldType.String}},
                    {cLocalPath,    new DBFieldDef{ FieldName = cLocalPath,     TableName = cTableName, Type = DBFieldType.String}},
                    {cBannerPath,   new DBFieldDef{ FieldName = cBannerPath,    TableName = cTableName, Type = DBFieldType.String}},
                    {cThumbnailPath,new DBFieldDef{ FieldName = cThumbnailPath, TableName = cTableName, Type = DBFieldType.String}},
                    {cColors,       new DBFieldDef{ FieldName = cColors,        TableName = cTableName, Type = DBFieldType.String}},
                    {cResolution,   new DBFieldDef{ FieldName = cResolution,    TableName = cTableName, Type = DBFieldType.String}},
                    {cDisabled,     new DBFieldDef{ FieldName = cDisabled,      TableName = cTableName, Type = DBFieldType.String}},
                    {cSeriesName,   new DBFieldDef{ FieldName = cSeriesName,    TableName = cTableName, Type = DBFieldType.String}}
        };
		#endregion

		enum FanartResolution
        {
            BOTH,
            HD,
            FULLHD
        }

        public DBFanart()
			: base(cTableName, TableFields)
        {
        }

        public DBFanart(long ID)
			: base(cTableName, TableFields)
        {
            ReadPrimary(ID.ToString());
        }

		internal static void MaintainDatabaseTable(Version lastVersion)
		{
			try {
				//test for table existance
				if (!DatabaseHelper.TableExists(cTableName)) {
					DatabaseHelper.CreateTable(cTableName, TableFields.Values);
				}
			} catch (Exception) {
				MPTVSeriesLog.Write("Error Maintaining the " + cTableName + " Table");
			}
		}
		
        public static void ClearAll()
        {      
            cache.Clear();         
        }

        public static void Clear(int Index)
        {
            Clear(DBFanart.cTableName, new SQLCondition(DBFanart.TableFields, DBFanart.cIndex, Index, SQLConditionType.Equal));
            cache.Remove(Index);
        }

        public static void ClearDB(int seriesID) {
            Clear(DBFanart.cTableName, new SQLCondition(DBFanart.TableFields, DBFanart.cSeriesID, seriesID, SQLConditionType.Equal));
            ClearAll();
        }

        public void Delete()
        {
            // first let's delete the physical file
            if (this.isAvailableLocally)
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

        public static List<DBFanart> GetAll(int SeriesID, bool availableOnly)
        {           
            lock (cache)
            {
                if (cache == null || !cache.ContainsKey(SeriesID))
                {
                    try
                    {
                        // make sure the table is created - create a dummy object
                        DBFanart dummy = new DBFanart();

                        // retrieve all fields in the table
                        String sqlQuery = "select * from " + cTableName;
                        if (SeriesID > 0)
                        {
                            sqlQuery += " where " + cSeriesID + " = " + SeriesID.ToString();
                            if (availableOnly)
                                sqlQuery += " and " + cLocalPath + " != ''";
                        }
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
                        MPTVSeriesLog.Write("Found " + results.Rows.Count + " Fanart from Database", MPTVSeriesLog.LogLevel.Debug);

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
          
        } readonly List<DBFanart> _FanartsToDownload = new List<DBFanart>();

        /// <summary>
        /// Checks if a Series Fanart contains a Series Name
        /// </summary>
        public bool HasSeriesName {
            get
            {
            	if (this[cSeriesName] == "true")
                    return true;
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
                GlobalSet(DBFanart.TableFields, cChosen, false, new SQLCondition(DBFanart.TableFields, cSeriesID, this[cSeriesID], SQLConditionType.Equal));
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

        public bool isAvailableLocally
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
    }
}