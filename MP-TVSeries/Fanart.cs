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
using System.Drawing;
using System.Drawing.Imaging;

namespace WindowPlugins.GUITVSeries
{
    public class Fanart : IDisposable
    {
        #region Config Constants
        
        const string seriesFanArtFilenameFormat = "*{0}*.*";
        const string seasonFanArtFilenameFormat = "*{0}S{1}*.*";
        const string lightIdentifier = "_light_";
        
        #endregion

        #region Static Vars

        static Dictionary<int, Fanart> fanartsCache = new Dictionary<int, Fanart>();
        static Random fanartRandom = new Random();
        static Object lockObject = new Object();

        #endregion

        #region Vars

        int mSeriesId = -1;
        int mSeasonIndex = -1;

        bool? mIsLight = null;
        bool mSeasonMode = false;

        List<string> mFanarts = null;

        string mRandomPick = null;        

        DBFanart mDbChosenFanart = null;

        #endregion

        #region Properties
        
        public int SeriesID 
        { 
            get { return mSeriesId; } 
        }

        public int SeasonIndex 
        { 
            get { return mSeasonIndex; }
        }

        public bool Found 
        { 
            get
            { 
                return mFanarts != null && mFanarts.Count > 0; 
            } 
        }
        
        public bool SeasonMode 
        { 
            get { return mSeasonMode; } 
        }

        public bool HasColorInfo
        {
            get 
            {
                return mDbChosenFanart != null && mDbChosenFanart.HasColorInfo;
            }
        }

        public Color[] Colors
        {
            get
            {
                if (HasColorInfo)
                {
                    System.Drawing.Color[] colors = new System.Drawing.Color[3];
                    for (int i = 0; i < 3;)
                        colors[i] = mDbChosenFanart.GetColor(++i);
                    return colors;
                }
                else return null;
            }
        }

        public static string RGBColorToHex(Color color)
        {
            // without alpha
            return String.Format("{0:x}", color.R) +
                   String.Format("{0:x}", color.G) +
                   String.Format("{0:x}", color.B);
        }

        #endregion

        #region Private Constructors

        Fanart(int seriesID)
        {
            mSeriesId = seriesID;
            mSeasonMode = false;
            getFanart();
        }

        Fanart(int seriesID, int seasonIndex)
        {
            mSeriesId = seriesID;
            mSeasonIndex = seasonIndex;
            mSeasonMode = true;
            getFanart();
        }

        #endregion

        #region Static Methods
       
        public static Fanart getFanart(int seriesID)
        {
            // possibly multiple plugins accessing fanart at the same time
            lock (lockObject)
            {
                Fanart f = null;
                if (fanartsCache.ContainsKey(seriesID))
                {
                    f = fanartsCache[seriesID];
                    f.ForceNewPick();
                }
                else
                {
                    // this will get simple drop-ins (old method)
                    f = new Fanart(seriesID); 
                    fanartsCache.Add(seriesID, f);
                }
                return f;
            }
        }

        public static bool RefreshFanart(int seriesID)
        {
            Fanart f = null;
            if (fanartsCache.ContainsKey(seriesID))
            {
                f = fanartsCache[seriesID];
                f.getFanart();
                return true;
            }
            else
                return false;
        }

        public static Fanart getFanart(int seriesID, int seasonIndex)
        {
            // no cache for now for series
            return new Fanart(seriesID, seasonIndex);
        }

        /// <summary>
        /// Deletes all cached thumbs for a Series Fanart
        /// Sometimes fanarts get deleted online but we still have the old
        /// thumbs cached. This allows the user to clear the cache and re-download the thumbs
        /// </summary>
        /// <param name="seriesID"></param>
        public static void ClearFanartCache(int seriesID) {                    
            string searchPattern = seriesID.ToString() + "*.jpg";
            string cachePath = Helper.PathCombine(Settings.GetPath(Settings.Path.fanart), @"_cache\fanart\original\");
            if (!System.IO.Directory.Exists(cachePath)) return; //exit if no dir to avoid any exception in GetFiles
            string[] fileList = Directory.GetFiles(cachePath, searchPattern);

            foreach (string file in fileList) {
                MPTVSeriesLog.Write("Deleting Cached Fanart Thumbnail: " + file);
                FileInfo fileInfo = new FileInfo(file);                
                try {
                    fileInfo.Delete();
                }
                catch (Exception ex) {
                    MPTVSeriesLog.Write("Failed to Delete Cached Fanart Thumbnail: " + file + ": " + ex.Message);
                }
            }

            // Clear DB and Clear Cache
            DBFanart.ClearDB(seriesID);
        }

        public static string GetLocalThumbPath( DBFanart aFanart )
        {
            return GetLocalThumbPath( aFanart[DBFanart.cThumbnailPath], aFanart[DBFanart.cSeriesID] );
        }
        public static string GetLocalThumbPath(string aOnlineThumbPath, string aSeriesID)
        {
            // fanart thumbs are stored in: _cache\fanart\original\<seriesId>-*.jpg
            string lThumbPath = "_cache/fanart/original/" + Path.GetFileName( aOnlineThumbPath );
            if ( !lThumbPath.Contains( aSeriesID ) )
            {
                // add the series id to the filename
                string lOldValue = Path.GetFileName( aOnlineThumbPath );
                string lNewValue = aSeriesID + "-" + Path.GetFileName( aOnlineThumbPath );
                lThumbPath = lThumbPath.Replace( lOldValue, lNewValue );
            }
            return lThumbPath.Replace( "/", @"\" );
        }

        public static string GetLocalPath( DBFanart aFanart )
        {
            return GetLocalPath( aFanart[DBFanart.cBannerPath], aFanart[DBFanart.cSeriesID] );
        }
        public static string GetLocalPath( string aThumbPath, string aSeriesID )
        {
            // fanart thumbs are stored in: fanart\original\<seriesId>-*.jpg
            string lPath = "fanart/original/" + Path.GetFileName( aThumbPath );
            if ( !lPath.Contains( aSeriesID ) )
            {
                // add the series id to the filename
                string lOldValue = Path.GetFileName( aThumbPath );
                string lNewValue = aSeriesID + "-" + Path.GetFileName( aThumbPath );
                lPath = lPath.Replace( lOldValue, lNewValue );
            }
            return lPath.Replace( "/", @"\" );
        }

        #endregion

        #region Instance Methods

        public string FanartThumbFilename
        {
            get
            {
                if (!string.IsNullOrEmpty(_ThumbFileName)) return _ThumbFileName;

                List<DBFanart> lFanarts = DBFanart.GetAll(SeriesID, true);

                // check if we have populated the db with fanarts
                if (lFanarts == null || lFanarts.Count == 0) 
                    return string.Empty;

                // get a fallback fanart if preferred one does not exist
                _ThumbFileName = Path.Combine( Settings.GetPath( Settings.Path.fanart ), GetLocalThumbPath(lFanarts[0]) );
                
                // favour chosen fanart if it exists
                lFanarts.RemoveAll(f => f.Chosen != true);

                // should only be left with one if there is one chosen
                if (lFanarts.Count > 0)
                    _ThumbFileName = Path.Combine( Settings.GetPath( Settings.Path.fanart ), GetLocalThumbPath( lFanarts[0] ) );

                // cached thumbs may not be downloaded
                // currently only get retrieved on fanart chooser window open
                if ( File.Exists( _ThumbFileName ) )
                {
                    return _ThumbFileName;
                }
                else
                {
                    // lets create one from the fullsize fanart on disk
                    string lFullSizeFanart = Fanart.getFanart( SeriesID ).FanartFilename;
                    if ( !string.IsNullOrEmpty( lFullSizeFanart ) && File.Exists( lFullSizeFanart ) &&
                        _ThumbFileName.Length > Settings.GetPath( Settings.Path.fanart ).Length ) // only if we have filename
                    {
                        try
                        {
                            //create directory first, to get rid of GDI+ errors
                            Directory.CreateDirectory( Path.GetDirectoryName( _ThumbFileName ) );

                            Image lImage = Image.FromFile( lFullSizeFanart );
                            Bitmap lBitmap = ImageAllocator.Resize( lImage, new Size( 400, 225 ) );

                            lBitmap.Save( _ThumbFileName, ImageFormat.Jpeg );
                            return _ThumbFileName;
                        }
                        catch ( Exception ex )
                        {
                            MPTVSeriesLog.Write( "Failed to create fanart thumbnail '{0}' from full fanart '{1}'. {2}", _ThumbFileName, lFullSizeFanart, ex.ToString() );
                        }
                    }
                }

                // let skin handle it
                return string.Empty;
            }
        } string _ThumbFileName = string.Empty;

        public string FanartFilename
        {
            get 
            {
                // Maybe there has been some new additions we dont know about yet
                if (mFanarts != null && mFanarts.Count == 0)
                {
                    getFanart();
                    if (mFanarts.Count == 0)
                        return string.Empty;
                }
                
                if (DBOption.GetOptions(DBOption.cFanartRandom))
                {                                        
                    if (mRandomPick != null && mRandomPick != String.Empty)
                        return mRandomPick;

                    List<DBFanart> lFanartInDb = null;

                    if (DBFanart.GetAll(SeriesID, true) != null && (lFanartInDb = DBFanart.GetAll(SeriesID, true)) != null && lFanartInDb.Count > 0)
                    {
                        // Choose from db takes precedence (not ideal)                        
                        List<DBFanart> lTempFanart = lFanartInDb;
                        for (int i = (lTempFanart.Count - 1); i >= 0; i--)
                        {
                            // Remove any fanarts in database that are not local or have been disabled
                            if (!lTempFanart[i].IsAvailableLocally || lTempFanart[i].Disabled)
                                lFanartInDb.Remove(lFanartInDb[i]);
                        }
                        // we may no longer have any fanarts in db to choose from as they could be disabled/deleted from disk
                        if (lFanartInDb.Count > 0)
                            mRandomPick = lFanartInDb[fanartRandom.Next(0, lFanartInDb.Count)].FullLocalPath;

                        if (String.IsNullOrEmpty(mRandomPick))
                        {
                            if (mFanarts != null && mFanarts.Count > 0)
                                mRandomPick = mFanarts[fanartRandom.Next(0, mFanarts.Count)];
                            else
                                mRandomPick = string.Empty;
                        }
                    }
                    else
                    {
                        if (mFanarts != null && mFanarts.Count > 0)
                            mRandomPick = mFanarts[fanartRandom.Next(0, mFanarts.Count)];
                        else
                            mRandomPick = string.Empty;
                    }
                    return mRandomPick;
                }
                else
                {                    
                    // see if we have a chosen one in the db
                    List<DBFanart> lFanartInDb = DBFanart.GetAll(SeriesID, true);
                    if (lFanartInDb != null && lFanartInDb.Count > 0)
                    {
                        foreach (DBFanart f in lFanartInDb)
                        {
                            if (f.Chosen && f.IsAvailableLocally && !f.Disabled)
                            {
                                mDbChosenFanart = f;
                                break;
                            }
                        }

                        // we couldn't find any fanart set as chosen in db, we try to choose the first available
                        if (mDbChosenFanart == null || String.IsNullOrEmpty(mDbChosenFanart.FullLocalPath))
                        {
                            foreach (DBFanart f in lFanartInDb)
                            {
                                // Checking if available will also remove from database if not
                                if (f.IsAvailableLocally && !f.Disabled)
                                {
                                    mDbChosenFanart = f;
                                    break;
                                }
                            }
                        }

                        if (mDbChosenFanart != null)
                            return mDbChosenFanart.FullLocalPath;

                        // If still no fanart found in db, choose from available on hard drive
                        if (mDbChosenFanart == null || String.IsNullOrEmpty(mDbChosenFanart.FullLocalPath))
                        {
                            if (mFanarts != null && mFanarts.Count > 0)
                                return mRandomPick = mFanarts[0];
                            else
                                return string.Empty;
                        }
                        else
                            return mDbChosenFanart.FullLocalPath;
                    }
                    else
                    {
                        // No fanart found in db but user doesn't want random, we always return the first
                        if (mFanarts != null && mFanarts.Count > 0)
                            return mRandomPick = mFanarts[0];
                        else 
                            return string.Empty;
                    }
                }
            }
        }

        public bool RandomPickIsLight
        {
            get
            {
                if (mIsLight != null) return mIsLight.Value;
                mIsLight = fanArtIsLight(FanartFilename);
                return mIsLight.Value;
            }
        }

        public void ForceNewPick()
        {
            mIsLight = null;
            mRandomPick = null;
        }

        #endregion

        #region Implementation Instance Methods

        bool fanArtIsLight(string fanartFilename)
        {
            return (fanartFilename.Contains(lightIdentifier));
        }

        /// <summary>
        /// Creates a List of Fanarts in your Thumbs folder
        /// </summary>
        void getFanart()
        {            
            string fanartFolder = Settings.GetPath(Settings.Path.fanart);

            // Check if Fanart folder exists in MediaPortal's Thumbs directory
            if (Directory.Exists(fanartFolder))
            {
                MPTVSeriesLog.Write($"Checking for local fanart on disk for series '{Helper.getCorrespondingSeries(mSeriesId)}'", MPTVSeriesLog.LogLevel.Debug);
                try
                {
                    // Create a Filename filter for Season / Series Fanart
                    string seasonFilter = string.Format(seasonFanArtFilenameFormat, mSeriesId, mSeasonIndex);
                    string seriesFilter = string.Format(seriesFanArtFilenameFormat, mSeriesId);
                    
                    string filter = mSeasonMode ? seasonFilter : seriesFilter;                                  

                    mFanarts = new List<string>();
                    // Store list of all fanart files found in all sub-directories of fanart thumbs folder
                    mFanarts.AddRange(Directory.GetFiles(fanartFolder, filter, SearchOption.AllDirectories));

                    // If no Season Fanart was found, see if any Series fanart exists
                    if (mFanarts.Count == 0 && mSeasonMode)
                    {
                        MPTVSeriesLog.Write("No Season Fanart found on disk, searching for series fanart", MPTVSeriesLog.LogLevel.Debug);
                        mFanarts.AddRange(Directory.GetFiles(fanartFolder, seriesFilter, SearchOption.AllDirectories));
                    }

                    // Remove any files that we don't want e.g. thumbnails in the _cache folder
                    // and Season fanart if we are not in Season Mode
                    if (!mSeasonMode) removeSeasonFromSeries();
                    removeFromFanart("_cache");

                    MPTVSeriesLog.Write("Found " + mFanarts.Count.ToString() + " fanart files on disk", MPTVSeriesLog.LogLevel.Debug);
                }
                catch (Exception ex)
                {
                    MPTVSeriesLog.Write("An error occured looking for fanart. Exception=" + ex.Message);
                }
            }
        }

        void removeSeasonFromSeries()
        {
            string seasonFormat = SeriesID.ToString() + 'S';
            removeFromFanart(seasonFormat);
        }

        void removeFromFanart(string needle)
        {
            if (mFanarts == null) return;
            for (int i = 0; i < mFanarts.Count; i++)
            {
                if (mFanarts[i].Contains(needle))
                {
                    mFanarts.Remove(mFanarts[i]);
                    i--;
                }
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
