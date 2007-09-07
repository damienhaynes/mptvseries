using System;
using System.Collections.Generic;
using System.Text;

namespace WindowPlugins.GUITVSeries
{
    class Fanart
    {
        static Dictionary<int, Fanart> fanartsCache = new Dictionary<int, Fanart>();
        static System.Drawing.Size requiredSize = new System.Drawing.Size();

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

        public static Fanart getFanart(int seriesID, int seasonIndex)
        {
            // no cache for now for series
            return new Fanart(seriesID, seasonIndex);
        }

        static Random fanartRandom = new Random();
        const string seriesFanArtFilenameFormat = "*{0}*.png";
        const string seasonFanArtFilenameFormat = "*{0}S{1}*.png";
        const string lightIdentifier = "_light_";

        int _seriesID = -1;
        int _seasonIndex = -1;
        bool? _isLight = null;
        bool _seasonMode = false;
        List<string> _fanArts = null;
        string _randomPick = null;
        string _textureName = null;

        public int SeriesID { get { return _seriesID; } }
        public int SeasonIndex { get { return _seasonIndex; } }
        public bool Found { get { return _fanArts != null && _fanArts.Count > 0; } }
        public bool SeasonMode { get { return _seasonMode; } }

        private Fanart(int seriesID)
        {
            _seriesID = seriesID;
            _seasonMode = false;
            getFanart();
        }

        private Fanart(int seriesID, int seasonIndex)
        {
            _seriesID = seriesID;
            _seasonIndex = seasonIndex;
            _seasonMode = true;
            getFanart();
        }

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
                FlushTexture();
                _textureName = ImageAllocator.buildMemoryImageFromFile(RandomFanart, requiredSize);
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

    }
}
