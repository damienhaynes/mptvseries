using System;
using System.Collections.Generic;
using System.Text;
using MediaPortal.Configuration;
using System.IO;
using Microsoft.Win32;

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

        public static bool SetPath(Path path, string databaseFile)
        {
            switch (path)
            {
                case Path.database:
                    RegistryKey rk = Registry.CurrentUser.CreateSubKey("Software\\MPTVSeries");
                    rk.SetValue("DBFile", databaseFile);
                    return true;

                case Path.log:
                    return false;
                case Path.banners:
                    return false;
                case Path.lang:
                    return false;
                case Path.thumbs:
                    return false;

                default:
                    return false;
            }

        }

        private Settings()
        {
        }

        static Settings()
        {

            // AB: can override DB path, stored in the registry
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("Software\\MPTVSeries");
            if (rk == null)
                rk = Registry.CurrentUser.CreateSubKey("Software\\MPTVSeries");

            dbPath = Config.GetFile(Config.Dir.Database, "TVSeriesDatabase4.db3");
            Directory.CreateDirectory(Config.GetFolder(Config.Dir.Database));

            if (rk != null)
            {
                // input fields
                Object value;
                value = rk.GetValue("DBFile");
                if (value != null)
                    dbPath = value.ToString();
            }

            // now, update the other paths based on the database one
            String baseDir = dbPath.Remove(dbPath.LastIndexOf('\\')); // remove DB name
            baseDir = baseDir.Remove(baseDir.LastIndexOf('\\')); // Get out of database folder
            logPath = System.IO.Path.Combine(baseDir, @"log\MP-TVSeries.log");
            bannersPath = System.IO.Path.Combine(baseDir, @"thumbs\MPTVSeriesBanners");
            langPath = System.IO.Path.Combine(baseDir, @"language\MP-TVSeries");
            thumbsPath = System.IO.Path.Combine(baseDir, @"thumbs");
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
