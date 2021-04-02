using System;
using System.IO;
using WindowPlugins.GUITVSeries.FanartTvAPI.DataStructures;
using WindowPlugins.GUITVSeries.FanartTvAPI.Extensions;

namespace WindowPlugins.GUITVSeries.FanartTvAPI
{
    public static class FanartTvCache
    {
        //public static bool SaveSeriesToCache(TmdbShowDetail aSeries, string aLanguage = "en")
        //{
        //    if (aSeries == null) return false;

        //    MPTVSeriesLog.Write($"Saving {aSeries.Name} to file cache", MPTVSeriesLog.LogLevel.Debug);

        //    string lFilename = Path.Combine(Settings.GetPath(Settings.Path.config), string.Format(@"Cache\Tmdb\{0}\{1}\series.json", aSeries.Id, aLanguage));

        //    try
        //    {
        //        if (!Directory.Exists(Path.GetDirectoryName(lFilename)))
        //            Directory.CreateDirectory(Path.GetDirectoryName(lFilename));

        //        // serialise the object to json and save
        //        File.WriteAllText(lFilename, aSeries.ToJSON());
        //    }
        //    catch (Exception ex)
        //    {
        //        MPTVSeriesLog.Write($"Failed to save '{lFilename}' to cache: {ex.Message}");
        //        return false;
        //    }

        //    return true;
        //}

        //public static TmdbShowDetail LoadSeriesFromCache(int aSeriesID, string aLanguage = "en")
        //{
        //    MPTVSeriesLog.Write($"Loading series '{aSeriesID}' from file cache", MPTVSeriesLog.LogLevel.Debug);

        //    string lFilename = Path.Combine(Settings.GetPath(Settings.Path.config), string.Format(@"Cache\Tmdb\{0}\{1}\series.json", aSeriesID, aLanguage));

        //    if (!File.Exists(lFilename)) return null;

        //    string lSeries = File.ReadAllText(lFilename);

        //    return lSeries.FromJSON<TmdbShowDetail>();
        //}
    }
}
