using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WindowPlugins.GUITVSeries
{
    public class LogWriter
    {
        private String m_filename;
        private StreamWriter m_LogStream;
        private System.Windows.Forms.ListBox m_ListLog;
        private MediaPortal.Dialogs.GUIDialogProgress m_DlgProgress;

        public void AddNotifier(ref System.Windows.Forms.ListBox notifier)
        {
            this.m_ListLog = notifier;
        }
        public void AddNotifier(ref MediaPortal.Dialogs.GUIDialogProgress notifier)
        {
            this.m_DlgProgress = notifier;
        }

        public LogWriter()
        {
            String logfile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
#if TEST
            logfile += @"\MP-TVSeries.log";
#else
            logfile = logfile.Remove(logfile.LastIndexOf('\\')); // Get out of Windows folder
            logfile = logfile.Remove(logfile.LastIndexOf('\\')); // Get out of plugin folder
            logfile += @"\Log\MP-TVSeries.log";
#endif
            this.m_filename = logfile;
        }

        public void Write(String entry)
        {
            if (this.m_ListLog != null)
            {
                m_ListLog.Items.Add(entry);
                int nTopIndex = m_ListLog.Items.Count - m_ListLog.Height / m_ListLog.ItemHeight;
                if (nTopIndex < 0)
                    nTopIndex = 0;
                m_ListLog.TopIndex = nTopIndex;
            }
            if (this.m_DlgProgress != null)
            {
                int lineSize = 50;
                if (entry.Length >= lineSize)
                {
                    int split = lineSize;
                    for (int index = lineSize - 1; index >= 0; index--)
                    {
                        if (entry[index] == ' ')
                        {
                            split = index;
                            break;
                        }
                    }
                    this.m_DlgProgress.SetLine(1, entry.Substring(0, split - 1));
                    this.m_DlgProgress.SetLine(2, entry.Substring(split + 1));
                }
                else
                {
                    this.m_DlgProgress.SetLine(1, entry);
                    this.m_DlgProgress.SetLine(2, "");
                }
                this.m_DlgProgress.Progress();
            }
            if (File.Exists(this.m_filename))
                this.m_LogStream = File.AppendText(this.m_filename);
            else
                this.m_LogStream = File.CreateText(this.m_filename);

            this.m_LogStream.WriteLine(DateTime.Now + " - " + entry);
            this.m_LogStream.Flush();

            this.m_LogStream.Close();
            this.m_LogStream.Dispose();
        }
    }
}
