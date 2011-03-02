using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

namespace WindowPlugins.GUITVSeries
{
    public class MPTVSeriesLog // can't call it Log because MP's own Log is called that
    {
        public enum LogLevel
        {
            Normal,
            Debug
        }

        static private LogLevel _selectedLogLevel = LogLevel.Normal;
	    public static LogLevel selectedLogLevel
	    {
		    get { return _selectedLogLevel;}
		    set {
                    if (value != _selectedLogLevel)
                    {
                        Write("Switched LogLevel to: " + value.ToString());
                        _selectedLogLevel = value;
                    }
                }
	    }

        static private String m_filename;
        static private StreamWriter m_LogStream;
        static private System.Windows.Forms.ListBox m_ListLog;
        static private MediaPortal.Dialogs.GUIDialogProgress m_DlgProgress;
        private delegate void Log_WriteCallback(string input);

        static public void AddNotifier(ref System.Windows.Forms.ListBox notifier)
        {
            m_ListLog = notifier;
        }
        static public void AddNotifier(ref MediaPortal.Dialogs.GUIDialogProgress notifier)
        {
            m_DlgProgress = notifier;
        }

        static MPTVSeriesLog()
        {
            String logfile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

#if TEST
            logfile += @"\MP-TVSeries.log";
#else
            logfile = logfile.Remove(logfile.LastIndexOf('\\')); // Get out of Windows folder
            logfile = logfile.Remove(logfile.LastIndexOf('\\')); // Get out of plugin folder
            Directory.CreateDirectory(logfile + @"\Log");
            logfile += @"\Log\MP-TVSeries.log";
#endif
            m_filename = logfile;
            m_LogStream = File.CreateText(m_filename);
            m_LogStream.Close();
            m_LogStream.Dispose();
        }

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

        static public void Write(string entry, LogLevel level, bool singleLine)
        {
            if ((int)level <= (int)selectedLogLevel)
            {
                lock (typeof(MPTVSeriesLog))
                {
                    if (File.Exists(m_filename))
                        m_LogStream = File.AppendText(m_filename);
                    else
                        m_LogStream = File.CreateText(m_filename);
                    if(singleLine) m_LogStream.WriteLine(DateTime.Now + " - " + entry);
                    else m_LogStream.Write(DateTime.Now + " - \n" + entry);
                    m_LogStream.Flush();

                    m_LogStream.Close();
                    m_LogStream.Dispose();
                }
                if (level != LogLevel.Debug) Log_Write(entry);
            }
        }

        static public void Log_Write(String entry)
        {
            if (m_ListLog != null)
            {
                if (m_ListLog.InvokeRequired)
                {
                    Log_WriteCallback d = new Log_WriteCallback(Write);
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

            //if (this.m_DlgProgress != null)
            //{
            //    int lineSize = 50;
            //    if (entry.Length >= lineSize)
            //    {
            //        int split = lineSize;
            //        for (int index = lineSize - 1; index >= 0; index--)
            //        {
            //            if (entry[index] == ' ')
            //            {
            //                split = index;
            //                break;
            //            }
            //        }
            //        this.m_DlgProgress.SetLine(1, entry.Substring(0, split - 1));
            //        this.m_DlgProgress.SetLine(2, entry.Substring(split + 1));
            //    }
            //    else
            //    {
            //        this.m_DlgProgress.SetLine(1, entry);
            //        this.m_DlgProgress.SetLine(2, "");
            //    }
            //    this.m_DlgProgress.Progress();
            //}
        }
    }
}
