using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using WindowPlugins.GUITVSeries.TmdbAPI.DataStructures;
using WindowPlugins.GUITVSeries.TmdbAPI.Extensions;

namespace WindowPlugins.GUITVSeries.TmdbAPI
{
    public static class TmdbCache
    {
        public static bool SaveSeriesToCache(TmdbShowDetail aSeries, string aLanguage = "en")
        {
            if (aSeries == null) return false;

            MPTVSeriesLog.Write($"Saving {aSeries.Name} to file cache", MPTVSeriesLog.LogLevel.Debug);

            string lFilename = Path.Combine(Settings.GetPath(Settings.Path.config), string.Format(@"Cache\Tmdb\{0}\{1}\series.json", aSeries.Id, aLanguage));

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(lFilename)))
                    Directory.CreateDirectory(Path.GetDirectoryName(lFilename));

                // serialise the object to json and save
                File.WriteAllText(lFilename, aSeries.ToJSON());
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write($"Failed to save '{lFilename}' to cache: {ex.Message}");
                return false;
            }

            return true;
        }

        public static bool SaveSeasonToCache(TmdbSeasonDetail aSeason, int aSeriesId, int aSeasonNumber, string aLanguage = "en")
        {
            if (aSeason == null) return false;

            MPTVSeriesLog.Write($"Saving {aSeason.Id} to file cache", MPTVSeriesLog.LogLevel.Debug);

            string lFilename = Path.Combine(Settings.GetPath(Settings.Path.config), string.Format(@"Cache\Tmdb\{0}\{1}\season_{2}.json", aSeriesId, aLanguage, aSeasonNumber));

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(lFilename)))
                    Directory.CreateDirectory(Path.GetDirectoryName(lFilename));

                // serialise the object to json and save
                File.WriteAllText(lFilename, aSeason.ToJSON());
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write($"Failed to save '{lFilename}' to cache: {ex.Message}");
                return false;
            }

            return true;
        }

        public static TmdbShowDetail LoadSeriesFromCache(int aSeriesID, string aLanguage = "en")
        {
            MPTVSeriesLog.Write($"Loading series '{aSeriesID}' from file cache", MPTVSeriesLog.LogLevel.Debug);

            string lFilename = Path.Combine(Settings.GetPath(Settings.Path.config), string.Format(@"Cache\Tmdb\{0}\{1}\series.json", aSeriesID, aLanguage));

            if (!File.Exists(lFilename)) return null;

            string lSeries = File.ReadAllText(lFilename);

            return lSeries.FromJSON<TmdbShowDetail>();
        }

        public static TmdbSeasonDetail LoadSeasonFromCache(int aSeriesID, int aSeason, string aLanguage = "en")
        {
            MPTVSeriesLog.Write($"Loading series '{aSeriesID}' season '{aSeason}' from file cache", MPTVSeriesLog.LogLevel.Debug);

            string lFilename = Path.Combine(Settings.GetPath(Settings.Path.config), string.Format(@"Cache\Tmdb\{0}\{1}\season_{2}.json", aSeriesID, aLanguage, aSeason));

            if (!File.Exists(lFilename)) return null;

            string lSeason = File.ReadAllText(lFilename);

            return lSeason.FromJSON<TmdbSeasonDetail>();
        }

    }
}
