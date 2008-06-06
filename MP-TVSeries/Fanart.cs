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
    class Fanart
    {
        #region config Constants
        const string seriesFanArtFilenameFormat = "*{0}*.*";
        const string seasonFanArtFilenameFormat = "*{0}S{1}*.*";
        const string lightIdentifier = "_light_";
        #endregion

        #region Static Vars
        static Dictionary<int, Fanart> fanartsCache = new Dictionary<int, Fanart>();
        static System.Drawing.Size requiredSize = new System.Drawing.Size();
        static Random fanartRandom = new Random();
        #endregion

        #region Vars
        int _seriesID = -1;
        int _seasonIndex = -1;
        bool? _isLight = null;
        bool _seasonMode = false;
        List<string> _fanArts = null;
        string _randomPick = null;
        string _textureName = null;
        DBFanart _dbchosenfanart = null;
        #endregion

        #region Properties
        public int SeriesID { get { return _seriesID; } }
        public int SeasonIndex { get { return _seasonIndex; } }
        public bool Found { get { return _fanArts != null && _fanArts.Count > 0; } }
        public bool SeasonMode { get { return _seasonMode; } }
        public bool HasColorInfo
        {
            get 
            {
                return _dbchosenfanart != null && _dbchosenfanart.HasColorInfo;
            }
        }
        public System.Drawing.Color[] Colors
        {
            get
            {
                if (HasColorInfo)
                {
                    System.Drawing.Color[] colors = new System.Drawing.Color[3];
                    for (int i = 0; i < 3;)
                        colors[i] = _dbchosenfanart.GetColor(++i);
                    return colors;
                }
                else return null;
            }
        }

        public static string RGBColorToHex(System.Drawing.Color color)
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
            _seriesID = seriesID;
            _seasonMode = false;
            getFanart();
        }

        Fanart(int seriesID, int seasonIndex)
        {
            _seriesID = seriesID;
            _seasonIndex = seasonIndex;
            _seasonMode = true;
            getFanart();
        }
        #endregion

        #region Static Methods
        public static Fanart getFanart(int seriesID)
        {
            Fanart f = null;
            if (fanartsCache.ContainsKey(seriesID))
            {
                f = fanartsCache[seriesID];
                f.ForceNewPick();
            }
            else
            {
                f = new Fanart(seriesID); // this will get simple drop-ins (old method)
                fanartsCache.Add(seriesID, f);
            }
            return f;
        }

        public static void FlushTextures()
        {
            foreach (KeyValuePair<int, Fanart> p in fanartsCache)
                p.Value.FlushTexture();
        }

        public static Fanart getFanart(int seriesID, int seasonIndex)
        {
            // no cache for now for series
            return new Fanart(seriesID, seasonIndex);
        }
        #endregion

        #region Instance Methods
        public string FanartFilename
        {
            get 
            {
                if (DBOption.GetOptions(DBOption.cFanartRandom))
                {
                    List<DBFanart> _faInDB = null;
                    if (_randomPick != null) return _randomPick;
                    else if (_fanArts == null || _fanArts.Count == 0) _randomPick = string.Empty;
                    else if (DBFanart.GetAll(SeriesID, true) != null && (_faInDB = DBFanart.GetAll(SeriesID, true)) != null && _faInDB.Count > 0) 
                    {

                        _randomPick = _faInDB[fanartRandom.Next(0, _faInDB.Count)].FullLocalPath; // from db take precedence (not ideal)
                    }
                    else _randomPick = _fanArts[fanartRandom.Next(0, _fanArts.Count)];
                    return _randomPick;
                }
                else
                {
                    // see if we have a chosen one in the db
                    List<DBFanart> _faInDB = DBFanart.GetAll(SeriesID, true);
                    if (_faInDB != null && _faInDB.Count > 0)
                    {
                        foreach (DBFanart f in _faInDB)
                            if (f.Chosen)
                            {
                                _dbchosenfanart = f;
                                break;
                            }
                        if (_dbchosenfanart == null) // we have some in db but none chosen, we choose the first
                            _dbchosenfanart = _faInDB[0];
                        return _dbchosenfanart.FullLocalPath;
                    }
                    else
                    {
                        // none in db but user doesn't want random, we always return the first
                        if (_fanArts != null && _fanArts.Count > 0)
                            return _randomPick = _fanArts[0];
                        else return string.Empty;
                    }
                }
            }
        }

        public string FanartAsTexture
        {
            get 
            {
                string _textureName_temp = ImageAllocator.GetOtherImage(FanartFilename, requiredSize, true);
                if (_textureName != _textureName_temp)
                {
                    FlushTexture(); // flush the old one
                    _textureName = _textureName_temp;
                }
                return _textureName;
            }
        }

        public void FlushTexture()
        {
            if (_textureName != null) ImageAllocator.Flush(_textureName);
        }

        public bool RandomPickIsLight
        {
            get
            {
                if (_isLight != null) return _isLight.Value;
                _isLight = fanArtIsLight(FanartFilename);
                return _isLight.Value;
            }
        }

        public void ForceNewPick()
        {
            _isLight = null;
            _randomPick = null;
        }
        #endregion

        #region Implementation Instance Methods
        bool fanArtIsLight(string fanartFilename)
        {
            return (fanartFilename.Contains(lightIdentifier));
        }

        void getFanart()
        {
            if(System.IO.Directory.Exists(Settings.GetPath(Settings.Path.fanart)))
            {
                MPTVSeriesLog.Write("Fanart: checking for ", _seriesID, MPTVSeriesLog.LogLevel.Debug);
                try
                {
                    string filter = _seasonMode
                                  ? string.Format(seasonFanArtFilenameFormat, _seriesID, _seasonIndex)
                                  : string.Format(seriesFanArtFilenameFormat, _seriesID);
                    _fanArts = new List<string>();
                    _fanArts.AddRange(System.IO.Directory.GetFiles(Settings.GetPath(Settings.Path.fanart), filter, System.IO.SearchOption.AllDirectories));
                    if (!_seasonMode) removeSeasonFromSeries();
                    removeFromFanart("_cache"); // thumbnails online
                    MPTVSeriesLog.Write("Fanart: found ", _fanArts.Count.ToString(), MPTVSeriesLog.LogLevel.Debug);
                }
                catch (Exception ex)
                {
                    MPTVSeriesLog.Write("An error occured looking for fanart: " + ex.Message);
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
            if (_fanArts == null) return;
            for (int i = 0; i < _fanArts.Count; i++)
            {
                if (_fanArts[i].Contains(needle))
                {
                    _fanArts.Remove(_fanArts[i]);
                    i--;
                }
            }
        }
        #endregion
    }
}
