using System;
using System.Collections.Generic;
using System.Text;
using MediaPortal.Configuration;
using System.IO;

namespace WindowPlugins.GUITVSeries
{
    sealed class Settings
    {
        private static string logPath = string.Empty;
        private static string dbPath = string.Empty;
        private static string bannersPath = string.Empty;
        private static string langPath = string.Empty;
        private static string thumbsPath = string.Empty;

        public enum Path
        {
            log,
            database,
            banners,
            lang,
            thumbs
        };

        public static string GetPath(Path path)
        {
            switch (path)
            {
                case Path.log:
                    return logPath;
                case Path.database:
                    return dbPath;
                case Path.banners:
                    return bannersPath;
                case Path.lang:
                    return langPath;
                case Path.thumbs:
                    return thumbsPath;
            }
            return string.Empty;
        }

        private Settings()
        {
        }

        static Settings()
        {
            logPath = Config.GetFile(Config.Dir.Log, "MP-TVSeries.log");
            dbPath = Config.GetFile(Config.Dir.Database, "TVSeriesDatabase4.db3");
            bannersPath = Config.GetSubFolder(Config.Dir.Thumbs, "MPTVSeriesBanners");
            langPath = Config.GetSubFolder(Config.Dir.Language, "MP-TVSeries");
            thumbsPath = Config.GetFolder(Config.Dir.Thumbs);

        }

        private static void initFolders()
        {
            try
            {
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(dbPath))) Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dbPath));
                if (!Directory.Exists(bannersPath)) Directory.CreateDirectory(bannersPath);
                if (!Directory.Exists(langPath)) Directory.CreateDirectory(langPath);
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Error getting Paths: " + ex.Message);
            }
        }
    }
}
