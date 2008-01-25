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
using MediaPortal.Configuration;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace WindowPlugins.GUITVSeries
{
    sealed class Settings
    {
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

        #region Path Vars
        static string logPath = string.Empty;
        static string dbPath = string.Empty;
        static string bannersPath = string.Empty;
        static string langPath = string.Empty;
        static string thumbsPath = string.Empty;
        static string apppath = string.Empty;
        static string fanArtPath = string.Empty;
        #endregion

        #region Vars
        static string _executingAssembly = Assembly.GetEntryAssembly().Location;
        static bool _isConfig = System.IO.Path.GetFileNameWithoutExtension(ExecutingAssembly).Equals("configuration", StringComparison.InvariantCultureIgnoreCase);        
        static string _version = Assembly.GetCallingAssembly().GetName().Version.ToString();        
        static string _userAgent = string.Format("MP TVSeries Plugin {0} {1}", isConfig ? "Configuration Utility" : string.Empty, Version);
        #endregion

        #region Constructors
        private Settings() {}

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
        #endregion

        #region Properties
        public static string ExecutingAssembly
        { get { return _executingAssembly; } }

        public static bool isConfig
        { get { return _isConfig; } }

        public static string Version
        { get { return _version; } }

        public static string UserAgent
        { get { return _userAgent; } }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the requested Path
        /// </summary>
        /// <param name="path">The Settings.Path to get</param>
        /// <returns>The fully qualified Path as a String</returns>
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
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Sets an alternative Database Storage Location.\nThe new Location will be saved in the Registry.
        /// </summary>
        /// <param name="databaseFile">Location of the Database</param>
        public static void SetDBPath(string databaseFile)
        {
            RegistryKey rk = Registry.CurrentUser.CreateSubKey("Software\\MPTVSeries");
            rk.SetValue("DBFile", databaseFile);
        }
        #endregion

        # region Helpers
        private static void initFolders()
        {
            try
            {
                createDirIfNotExists(dbPath);
                createDirIfNotExists(bannersPath);
                createDirIfNotExists(langPath);

            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Error initiating Paths: " + ex.Message);
            }
        }

        private static void createDirIfNotExists(string dir)
        {
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(dir))) 
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dir));
        }
        #endregion
    }
}
