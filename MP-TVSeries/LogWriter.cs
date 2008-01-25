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

namespace WindowPlugins.GUITVSeries
{
    public class MPTVSeriesLog // can't call it Log because MP's own Log is called that
    {
        public enum LogLevel
        {
            Normal,
            Debug
        }

        #region Vars
        static private LogLevel _selectedLogLevel = LogLevel.Normal;
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
            #if TEST
            m_filename =  System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            m_filename += @"\MP-TVSeries.log";
            #endif

            m_LogStream = File.CreateText(m_filename);
            m_LogStream.Close();
            m_LogStream.Dispose();

            int level = 0;
            int.TryParse(DBOption.GetOptions("logLevel"), out level);
            selectedLogLevel = (LogLevel)level;
            pauseAutoWriteDB = true;
            Write("MPTVSeries Build: " + Settings.Version);
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
                lock (m_LogStream)
                {
                    try
                    {
                        if (File.Exists(m_filename))
                            m_LogStream = File.AppendText(m_filename);
                        else
                            m_LogStream = File.CreateText(m_filename);
                        if (singleLine) m_LogStream.WriteLine(DateTime.Now + " - " + entry);
                        else m_LogStream.Write(DateTime.Now + " - \n" + entry);
                        m_LogStream.Flush();

                        m_LogStream.Close();
                        m_LogStream.Dispose();
                        if (level != LogLevel.Debug) Log_Write(entry);
                    }
                    catch (Exception ex)
                    {
                        // well we can't write...maybe no file acces or something....and we can't even log the error
                        Log_Write(entry);
                        Log_Write(ex.Message);
                    }
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
