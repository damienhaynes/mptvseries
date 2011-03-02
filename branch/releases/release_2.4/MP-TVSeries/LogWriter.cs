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

namespace WindowPlugins.GUITVSeries
{
    public class MPTVSeriesLog // can't call it Log because MP's own Log is called that
    {
        const bool OmmitKey = true;
        public enum LogLevel
        {
            Normal,
            Debug,
            DebugSQL
        }

        #region Vars
        static private LogLevel _selectedLogLevel = LogLevel.Debug;
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
        public static LogLevel selectedLogLevel
	    {
		    get { return _selectedLogLevel;}
		    set {
                    if (value != _selectedLogLevel)
                    {
                        Write("Switched LogLevel to: " + value.ToString());
                        _selectedLogLevel = value;
                        if (!pauseAutoWriteDB)
                            DBOption.SetOptions("logLevel", (int)value);
                    }
                }
            }
        #endregion

        #region Constructor
        static MPTVSeriesLog()
        {
            if (System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToLower() == "devenv")
                return;
 
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

            int level = 0;
            int.TryParse(DBOption.GetOptions("logLevel"), out level);
            selectedLogLevel = (LogLevel)level;
            pauseAutoWriteDB = true;
            
            string[] ver = Settings.Version.ToString().Split(new char[] { '.' });            
            Write(string.Format("MP-TVSeries Version: v{0}.{1}.{2}", ver.GetValue(0), ver.GetValue(1), ver.GetValue(2)));
            Write("MP-TVSeries Build Date: " + Settings.BuildDate);
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

        /// <summary>
        /// Use this for Std. Log entries, only show up in LogLevel.Normal
        /// </summary>
        /// <param name="entry"></param>
        static public void Write(String entry, LogLevel level)
        {
            Write(entry, level, true);
        }
        #endregion

        #region Implementation
        static void Write(string entry, LogLevel level, bool singleLine)
        {
            if ((int)level <= (int)selectedLogLevel)
            {
                if (m_LogStream != null)
                {
                    lock (m_LogStream)
                    {
                        try
                        {
                            if (File.Exists(m_filename))
                                m_LogStream = File.AppendText(m_filename);
                            else
                                m_LogStream = File.CreateText(m_filename);
                            
                            if (OmmitKey && !Helper.String.IsNullOrEmpty(DBOnlineMirror.cApiKey) && entry.Contains(DBOnlineMirror.cApiKey))
                                entry = entry.Replace(DBOnlineMirror.cApiKey, "<apikey>");

                            String sPrefix = String.Format("{0:D8} - {1} - ", Thread.CurrentThread.ManagedThreadId, DateTime.Now);
                            
                            if (singleLine)
                                m_LogStream.WriteLine(sPrefix + entry);
                            else
                                m_LogStream.Write(sPrefix + "\n" + entry);

                            m_LogStream.Flush();
                            m_LogStream.Close();
                            m_LogStream.Dispose();

                        }
                        catch (Exception ex)
                        {
                            // well we can't write...maybe no file access or something....and we can't even log the error
                            Log_Write(entry);
                            Log_Write(ex.Message);                           
                            //m_LogStream.Close();
                            //m_LogStream.Dispose(); //Crash
                        }
                    }
                }
                // Dont log debug msgs to Configuration Logger
                if (level != LogLevel.Debug)
                    Log_Write(entry);
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
                    m_ListLog.Items.Add(DateTime.Now + " - " + entry);
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
