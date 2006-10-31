using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

namespace WindowPlugins.GUITVSeries
{
    public class LogWriter
    {
        private String m_filename;
        private StreamWriter m_LogStream;
        private System.Windows.Forms.ListBox m_ListLog;
        private MediaPortal.Dialogs.GUIDialogProgress m_DlgProgress;
        private delegate int itemAddDel(string msg);

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

        void LogWriter_newMsg(object sender, string entry)
        {
            
            if (this.m_ListLog != null)
                {
                    itemAddDel del = new itemAddDel(m_ListLog.Items.Add);

                    m_ListLog.Invoke(del, entry);
                    int nTopIndex = m_ListLog.Items.Count - m_ListLog.Height / m_ListLog.ItemHeight;
                    if (nTopIndex < 0)
                        nTopIndex = 0;
                    m_ListLog.TopIndex = nTopIndex;
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

        public void Write(String entry)
        {
            lock (typeof(LogWriter))
            {
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

    public class SafeInvokeHelper
    {
        static readonly ModuleBuilder builder;
        static readonly AssemblyBuilder myAsmBuilder;
        static readonly Hashtable methodLookup;

        static SafeInvokeHelper()
        {
            AssemblyName name = new AssemblyName();
            name.Name = "temp";
            myAsmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            builder = myAsmBuilder.DefineDynamicModule("TempModule");
            methodLookup = new Hashtable();
        }

        public static void Invoke(System.Windows.Forms.Control obj, string methodName, params object[] paramValues)
        {
            Delegate del = null;
            string key = obj.GetType().Name + "." + methodName;
            if (methodLookup.Contains(key))
                del = (Delegate)methodLookup[key];
            else
            {
                Type[] paramList = new Type[obj.GetType().GetMethod(methodName).GetParameters().Length];
                int n = 0;
                foreach (ParameterInfo pi in obj.GetType().GetMethod(methodName).GetParameters()) paramList[n++] = pi.ParameterType;
                TypeBuilder typeB = builder.DefineType("Test" + methodName, TypeAttributes.Class | TypeAttributes.AutoLayout | TypeAttributes.Public | TypeAttributes.Sealed, typeof(MulticastDelegate), PackingSize.Unspecified);
                ConstructorBuilder conB = typeB.DefineConstructor(MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, new Type[] { typeof(object), typeof(IntPtr) });
                conB.SetImplementationFlags(MethodImplAttributes.Runtime);
                MethodBuilder mb = typeB.DefineMethod("Invoke", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, typeof(void), paramList);
                mb.SetImplementationFlags(MethodImplAttributes.Runtime);
                Type tp = typeB.CreateType();
                MethodInfo mi = obj.GetType().GetMethod(methodName);
                del = MulticastDelegate.CreateDelegate(tp, obj, methodName);
                methodLookup.Add(key, del);
            }

            obj.Invoke(del, paramValues);
        }
    }
}
