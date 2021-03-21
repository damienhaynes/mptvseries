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
using System.IO;
using System.Threading;
using System.Net;
using WindowPlugins.GUITVSeries.TmdbAPI;

namespace WindowPlugins.GUITVSeries
{
    public class MPTVSeriesLog // can't call it Log because MP's own Log is called that
    {
        public enum LogLevel
        {
            Normal,
            Debug,
            DebugSQL
        }

        #region Vars
        static String m_filename = Settings.GetPath(Settings.Path.log);
        static StreamWriter m_LogStream;
        static System.Windows.Forms.ListBox m_ListLog;
        static MediaPortal.Dialogs.GUIDialogProgress m_DlgProgress;
        delegate void Log_WriteCallback(string input);
        #endregion

        #region Public Fields
        public static bool pauseAutoWriteDB = false;
        #endregion

        #region Properties
        static LogLevel selectedLogLevel{ get; set; }
        #endregion
        
        #region Constructor
        static MPTVSeriesLog()
        {
            if (System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToLower() == "devenv")
                return;
                            
            InitLogLevel();

            // let's rename the old one to .bak just like MP does
            try
            {
                if (File.Exists(m_filename))
                {
                    string bgFile = Settings.GetPath(Settings.Path.logBackup);
                    if (File.Exists(bgFile)) File.Delete(bgFile);
                    File.Move(m_filename, Settings.GetPath(Settings.Path.logBackup));
                }
            }
            catch (Exception e)
            {
                Write("Problem backing up Log file: " + e.Message);
            }
            try
            {
                m_LogStream = File.CreateText(m_filename);
                m_LogStream.Close();
                m_LogStream.Dispose();
            }
            catch (Exception)
            {
                // oopps, can't create file
            }
            
            // log data to and from themoviedb.org
            TmdbAPI.TmdbAPI.OnDataSend += new TmdbAPI.TmdbAPI.OnDataSendDelegate( API_OnDataSend );
            TmdbAPI.TmdbAPI.OnDataError += new TmdbAPI.TmdbAPI.OnDataErrorDelegate( API_OnDataError );
            TmdbAPI.TmdbAPI.OnDataReceived += new TmdbAPI.TmdbAPI.OnDataReceivedDelegate( API_OnDataReceived );

            // log data to and from themoviedb.org
            FanartTvAPI.FanartTvAPI.OnDataSend += new FanartTvAPI.FanartTvAPI.OnDataSendDelegate(API_OnDataSend);
            FanartTvAPI.FanartTvAPI.OnDataError += new FanartTvAPI.FanartTvAPI.OnDataErrorDelegate(API_OnDataError);
            FanartTvAPI.FanartTvAPI.OnDataReceived += new FanartTvAPI.FanartTvAPI.OnDataReceivedDelegate(API_OnDataReceived);

            pauseAutoWriteDB = true;
            
        }
        #endregion

        #region Private Methods
        private static void API_OnDataSend( string aAddress, string aData )
        {
            if ( !string.IsNullOrEmpty( aData ) )
            {
                Write( string.Format("Address: {0}, Post: {1}", aAddress, aData), LogLevel.Debug );
            }
            else
            {
                Write( string.Format("Address: {0}", aAddress), LogLevel.Debug );
            }
        }

        private static void API_OnDataReceived( string aResponse, HttpWebResponse aWebResponse )
        {
            string lHeaders = string.Empty;
            foreach ( string key in aWebResponse.Headers.AllKeys )
            {
                lHeaders += string.Format( "{0}: {1}, ", key, aWebResponse.Headers[key] );
            }

            Write( string.Format("Response: {0}, Headers: {{{1}}}", aResponse ?? "null", lHeaders.TrimEnd( new char[] { ',', ' ' } ) ), LogLevel.Debug );
        }

        private static void API_OnDataError( string aError )
        {
            Write( aError );
        }
        #endregion

        #region Other Public Methods
        public static void InitLogLevel()
        {
            using (var xmlreader = new MediaPortal.Profile.MPSettings())
            {
                int logLevel = xmlreader.GetValueAsInt("general", "loglevel", 2);
                if (DBOption.GetOptions(DBOption.cSQLLoggingEnabled))
                    selectedLogLevel = MPTVSeriesLog.LogLevel.DebugSQL;
                else if (logLevel == 3)
                    selectedLogLevel = MPTVSeriesLog.LogLevel.Debug;
                else
                    selectedLogLevel = MPTVSeriesLog.LogLevel.Normal;
            }
        }
        #endregion

        #region Public Config Methods
        static public void AddNotifier(ref System.Windows.Forms.ListBox notifier)
        {
            m_ListLog = notifier;
        }
        static public void AddNotifier(ref MediaPortal.Dialogs.GUIDialogProgress notifier)
        {
            m_DlgProgress = notifier;
        }
        #endregion

        #region Public Write Methods
        /// <summary>
        /// Use this for Std. Log entries, only show up in LogLevel.Normal
        /// </summary>
        /// <param name="entry"></param>
        static public void Write(String entry)
        {
            Write(entry, LogLevel.Normal);
        }

        /// <summary>
        /// Use this for Log entries with many lines (e.g. StackTrace), only show up in LogLevel.Normal
        /// </summary>
        /// <param name="entry"></param>
        static public void WriteMultiLine(String entry, LogLevel level)
        {
            Write(entry, level, false);
        }

        /// <summary>
        /// Use this for Log entries with many lines (e.g. StackTrace)
        /// </summary>
        /// <param name="entry"></param>
        static public void WriteMultiLine(String entry)
        {
            WriteMultiLine(entry, LogLevel.Normal);
        }

        /// <summary>
        /// To avoid having to join values if not needed in lower LogLevels use this
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="param"></param>
        /// <param name="level"></param>
        static public void Write(String entry, int param, LogLevel level)
        {
            // speed things up so for 2 strings they will only get joined if really needed, 
            // otherwise all those debug calls will be slog
            if ((int)level <= (int)selectedLogLevel)
            {
                Write(entry + param.ToString(), level);
            }
        }

        /// <summary>
        /// To avoid having to join values if not needed in lower LogLevels use this
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="param"></param>
        /// <param name="level"></param>
        static public void Write(String entry, String param, LogLevel level)
        {
            // speed things up so for 2 strings they will only get joined if really needed, 
            // otherwise all those debug calls will be slog
            if ((int)level <= (int)selectedLogLevel)
            {
                Write(entry + param, level);
            }
        }

        static public void Write(String format, params object[] arg)
        {
            Write(string.Format(format, arg), LogLevel.Normal);
        }

        /// <summary>
        /// Use this for Std. Log entries, only show up in LogLevel.Normal
        /// </summary>
        /// <param name="entry"></param>
        static public void Write(String entry, LogLevel level)
        {
            Write(entry, level, true);
        }

        static string CreatePrefix(LogLevel level)
        {
            string logLevelString = "INFO";
            if (level == LogLevel.Normal)
                logLevelString = "INFO";
            else if(level == LogLevel.Debug)
                logLevelString = "DEBG";
            else
                logLevelString = "SQL ";

            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + String.Format(" [{0}][{1}]", logLevelString, Thread.CurrentThread.ManagedThreadId.ToString().PadLeft(2, '0')) + ": ";
        }
        #endregion

        #region Implementation
        static void Write(string entry, LogLevel level, bool singleLine)
        {
            if ((int)level <= (int)selectedLogLevel)
            {
                if (m_LogStream != null)
                {
                    #region Hide Personal Info
                    entry = entry.Replace( TmdbURIs.ApiKey, "<apiKey>" );
                    #endregion

                    string sPrefix = CreatePrefix(level);

                    // note: keep the lock block as small as possible
                    // don't call anything from within that could potentially call for a logwrite itself, or we have a deadlock
                    lock (m_LogStream) 
                    {
                        try
                        {
                            if (File.Exists(m_filename))
                                m_LogStream = File.AppendText(m_filename);
                            else
                                m_LogStream = File.CreateText(m_filename);

                            if (singleLine)
                                m_LogStream.WriteLine(sPrefix + entry);
                            else
                                m_LogStream.Write(sPrefix + "\n" + entry);

                            m_LogStream.Flush();
                            m_LogStream.Close();
                            m_LogStream.Dispose();
                        }
                        catch { }
                    }

                    // don't log debug msgs to Configuration Logger
                    if (level < LogLevel.Debug)
                        Log_Write(entry);
                }
            }
        }

        static void Log_Write(String entry)
        {
            if (m_ListLog != null)
            {
                if (m_ListLog.InvokeRequired)
                {
                    Log_WriteCallback d = new Log_WriteCallback(Log_Write);
                    m_ListLog.Invoke(d, new object[] { entry });
                }
                else
                {
                    m_ListLog.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ": " + entry);
                    int nTopIndex = m_ListLog.Items.Count - m_ListLog.Height / m_ListLog.ItemHeight;
                    if (nTopIndex < 0)
                        nTopIndex = 0;
                    m_ListLog.TopIndex = nTopIndex;
                }
            }
        }
        #endregion
    }
}
