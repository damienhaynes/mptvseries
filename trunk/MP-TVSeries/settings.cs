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
        private static string apppath = string.Empty;
        private static string fanArtPath = string.Empty;

        public static string executingAssembly = System.Reflection.Assembly.GetEntryAssembly().Location;
        public static bool isConfig = System.IO.Path.GetFileNameWithoutExtension(executingAssembly).ToLower() == "configuration";

        public enum Path
        {
            log,
            database,
            banners,
            lang,
            thumbs,
            app,
            fanart
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
                case Path.app:
                    return apppath;
                case Path.fanart:
                    return fanArtPath;
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
            apppath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            // AB: can override DB path, stored in the registry
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("Software\\MPTVSeries");
            if (rk == null)
                rk = Registry.CurrentUser.CreateSubKey("Software\\MPTVSeries");

            dbPath = Config.GetFile(Config.Dir.Database, "TVSeriesDatabase4.db3");

            if (rk != null)
            {
                // input fields
                Object value;
                value = rk.GetValue("DBFile");
                if (value != null)
                    dbPath = value.ToString();
            }

            // we respect overall MP settings which can be optinally defined
            logPath = Config.GetFile(Config.Dir.Log, "MP-TVSeries.log");
            bannersPath = Config.GetSubFolder(Config.Dir.Thumbs, "MPTVSeriesBanners");
            langPath = Config.GetSubFolder(Config.Dir.Language, "MP-TVSeries");
            thumbsPath = Config.GetFolder(Config.Dir.Thumbs);
            fanArtPath = thumbsPath + @"\Fan Art";
            initFolders();
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
                MPTVSeriesLog.Write("Error initiating Paths: " + ex.Message);
            }
        }
    }
}
