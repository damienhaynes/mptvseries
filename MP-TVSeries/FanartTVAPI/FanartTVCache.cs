using System;
using System.IO;
using WindowPlugins.GUITVSeries.FanartTvAPI.DataStructures;
using WindowPlugins.GUITVSeries.FanartTvAPI.Extensions;

namespace WindowPlugins.GUITVSeries.FanartTvAPI
{
    public static class FanartTvCache
    {
        const string cImageFilePath = @"Cache\FanartTV\{0}\images.json";
        public static bool SaveSeriesToCache(FanartTvImages aImages, string aSeriesId)
        {
            if (aImages == null) return false;

            MPTVSeriesLog.Write($"Saving fanart.tv images to file cache. TVDb ID={aSeriesId}", MPTVSeriesLog.LogLevel.Debug);

            string lFilename = Path.Combine(Settings.GetPath(Settings.Path.config), string.Format(cImageFilePath, aSeriesId));

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(lFilename)))
                    Directory.CreateDirectory(Path.GetDirectoryName(lFilename));

                // serialise the object to json and save
                File.WriteAllText(lFilename, aImages.ToJSON());
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write($"Failed to save '{lFilename}' to cache: {ex.Message}");
                return false;
            }

            return true;
        }

        public static FanartTvImages LoadSeriesFromCache(string aSeriesId)
        {
            MPTVSeriesLog.Write($"Loading fanart.tv images from file cache. TVDb ID={aSeriesId}", MPTVSeriesLog.LogLevel.Debug);

            string lFilename = Path.Combine(Settings.GetPath(Settings.Path.config), string.Format(cImageFilePath, aSeriesId));

            if (!File.Exists(lFilename)) return null;

            // if the cache is older than 1 day, download again
            if (File.GetLastWriteTime(lFilename) < DateTime.Now.Subtract(new TimeSpan(24, 0, 0)))
                return null;

            string lSeries = File.ReadAllText(lFilename);

            return lSeries.FromJSON<FanartTvImages>();
        }
    }
}
