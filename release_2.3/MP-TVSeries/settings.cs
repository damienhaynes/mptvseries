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
        public const bool newAPI = true;

        public enum Path
        {
            log,
            logBackup,
            database,
            banners,
            lang,
            thumbs,
            app,
            fanart,
            skin
        };

        #region Path Vars
        static string logPath = string.Empty;
        static string backupLogPath = string.Empty;
        static string dbPath = string.Empty;
        static string bannersPath = string.Empty;
        static string langPath = string.Empty;
        static string thumbsPath = string.Empty;
        static string apppath = string.Empty;
        static string fanArtPath = string.Empty;
        static string skinPath = string.Empty;
        #endregion

        #region Vars
        static Assembly _entryAssembly = null;
        static bool _isConfig;
        static Version _version = null;
        static DateTime _buildDate;
        static string _userAgent = null;
        #endregion

        #region Constructors
        private Settings() {}

        static Settings()
        {
            try
            {
                apppath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

                _entryAssembly = Assembly.GetEntryAssembly();
                _isConfig = !System.IO.Path.GetFileNameWithoutExtension(EntryAssembly.Location).Equals("mediaportal", StringComparison.InvariantCultureIgnoreCase);
                _version = Assembly.GetCallingAssembly().GetName().Version;
                _buildDate = getLinkerTimeStamp(Assembly.GetAssembly(typeof(Settings)).Location);
                _userAgent = string.Format("MPTVSeries{0}/{1}", isConfig ? "Config" : string.Empty, Version);
            }
            catch (Exception) { }

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
            backupLogPath = Config.GetFile(Config.Dir.Log, "MP-TVSeries.bak");
            bannersPath = Config.GetSubFolder(Config.Dir.Thumbs, "MPTVSeriesBanners");
            langPath = Config.GetSubFolder(Config.Dir.Language, "MP-TVSeries");
            thumbsPath = Config.GetFolder(Config.Dir.Thumbs);
            skinPath = Config.GetFolder(Config.Dir.Skin);
            fanArtPath = thumbsPath + @"\Fan Art";
            initFolders();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the Assembly that loaded the Plugin (usually Mediaportal.exe or Configuration.exe)
        /// </summary>
        public static Assembly EntryAssembly
        { get { return _entryAssembly; } }

        /// <summary>
        /// Gets a bool indicating wether or not the plugin has been loaded inside the configuration (checks EntryAssembly)
        /// </summary>
        public static bool isConfig
        { get { return _isConfig; } }

        /// <summary>
        /// Gets a the Version of the Plugin
        /// </summary>
        public static Version Version
        { get { return _version; } }

        public static DateTime BuildDate
        { get { return _buildDate; } }

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
                case Path.logBackup:
                    return backupLogPath;
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
                case Path.skin:
                    return skinPath;
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

        private static DateTime getLinkerTimeStamp(string filePath)
        {
            const int PeHeaderOffset = 60;
            const int LinkerTimestampOffset = 8;

            byte[] b = new byte[2047];
            using (System.IO.Stream s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                s.Read(b, 0, 2047);
            }

            int secondsSince1970 = BitConverter.ToInt32(b, BitConverter.ToInt32(b, PeHeaderOffset) + LinkerTimestampOffset);

            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(secondsSince1970);
        }

        #endregion
    }
}
