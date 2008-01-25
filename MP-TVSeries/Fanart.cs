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
        const string seriesFanArtFilenameFormat = "*{0}*.png";
        const string seasonFanArtFilenameFormat = "*{0}S{1}*.png";
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
        #endregion

        #region Properties
        public int SeriesID { get { return _seriesID; } }
        public int SeasonIndex { get { return _seasonIndex; } }
        public bool Found { get { return _fanArts != null && _fanArts.Count > 0; } }
        public bool SeasonMode { get { return _seasonMode; } }
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
                f = new Fanart(seriesID);
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
        public string RandomFanart
        {
            get 
            {
                if (_randomPick != null) return _randomPick;
                else if (_fanArts == null || _fanArts.Count == 0) _randomPick = string.Empty;
                else if (_fanArts.Count == 1) _randomPick = _fanArts[0];
                else _randomPick = _fanArts[fanartRandom.Next(0, _fanArts.Count)];
                return _randomPick;
            }
        }

        public string RandomFanartAsTexture
        {
            get 
            {
                string _textureName_temp = ImageAllocator.buildMemoryImageFromFile(RandomFanart, requiredSize);
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
                _isLight = fanArtIsLight(RandomFanart);
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
            MPTVSeriesLog.Write("Fanart: checking for ", _seriesID, MPTVSeriesLog.LogLevel.Debug);
            try
            {
                string filter = _seasonMode
                              ? string.Format(seasonFanArtFilenameFormat, _seriesID, _seasonIndex)
                              : string.Format(seriesFanArtFilenameFormat, _seriesID);
                _fanArts = new List<string>();
                _fanArts.AddRange(System.IO.Directory.GetFiles(Settings.GetPath(Settings.Path.fanart), filter, System.IO.SearchOption.AllDirectories));
                if (!_seasonMode) removeSeasonFromSeries();
                MPTVSeriesLog.Write("Fanart: found ", _fanArts.Count.ToString(), MPTVSeriesLog.LogLevel.Debug);
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("An error occured looking for fanart: " + ex.Message);
            }
        }

        void removeSeasonFromSeries()
        {
            if(_fanArts == null) return;
            string seasonFormat = SeriesID.ToString() + 'S';
            for (int i = 0; i < _fanArts.Count; i++)
            {
                if (_fanArts[i].Contains(seasonFormat))
                {
                    _fanArts.Remove(_fanArts[i]);
                    i--;
                }
            }
        }
        #endregion
    }
}
