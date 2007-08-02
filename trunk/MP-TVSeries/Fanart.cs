using System;
using System.Collections.Generic;
using System.Text;

namespace WindowPlugins.GUITVSeries
{
    class Fanart
    {
        static Dictionary<int, Fanart> fanartsCache = new Dictionary<int, Fanart>();

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

        static Random fanartRandom = new Random();
        const string seriesFanArtFilenameFormat = "*{0}*.png";
        const string seasonFanArtFilenameFormat = null; // needs to be speced "*{0}S{1}*.png" ??
        const string lightIdentifier = "_light_";

        int _seriesID = -1;
        int _seasonIndex = -1;
        bool? _isLight = null;
        bool _seasonMode = false;
        string[] _fanArts = null;
        string _randomPick = null;

        public int SeriesID { get { return _seriesID; } }
        //public int SeasonIndex { get { return _seasonIndex; } }
        public bool Found { get { return _fanArts != null && _fanArts.Length > 0; } }

        private Fanart(int seriesID)
        {
            _seriesID = seriesID;
            _seasonMode = false;
            getFanart();
        }

        // for future use
        /*
        private Fanart(int seriesID, int seasonIndex)
        {
            _seriesID = seriesID;
            _seasonIndex = seasonIndex;
            _seasonMode = true;
            getFanart();
        } */

        public string RandomFanart
        {
            get 
            {
                if (_randomPick != null) return _randomPick;
                else if (_fanArts == null || _fanArts.Length == 0) _randomPick = string.Empty;
                else if (_fanArts.Length == 1) _randomPick = _fanArts[0];
                else _randomPick = _fanArts[fanartRandom.Next(0, _fanArts.Length)];
                return _randomPick;
            }
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
                _fanArts = System.IO.Directory.GetFiles(Settings.GetPath(Settings.Path.fanart), filter, System.IO.SearchOption.AllDirectories);
                MPTVSeriesLog.Write("Fanart: found ", _fanArts.Length.ToString(), MPTVSeriesLog.LogLevel.Debug);
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("An error occured looking for fanart: " + ex.Message);
            }
        }

    }
}
