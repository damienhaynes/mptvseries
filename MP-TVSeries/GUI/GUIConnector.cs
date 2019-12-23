using MediaPortal.GUI.Library;
using System;
using System.Threading;

namespace WindowPlugins.GUITVSeries.GUI
{
    internal class GUIConnector
    {
        # region Singleton
        protected GUIConnector()
        {
            mTimeoutTimer.Elapsed += TaskWatcherTimerElapsed;
        }
        protected static GUIConnector mInstance = null;
        internal static GUIConnector Instance
        {
            get
            {
                if (mInstance == null) mInstance = new GUIConnector();
                return mInstance;
            }
        }
        #endregion

        internal bool IsBusy { get; private set; }

        Action<bool, object> mCurrentResultHandler = null;
        object mCurrentResult = null;
        bool? mCurrentTaskSuccess = null;
        Exception mCurrentError = null;
        string mCurrentTaskDescription = null;
        Thread mBackgroundThread = null;
        bool mAbortedByUser = false;
        System.Timers.Timer mTimeoutTimer = new System.Timers.Timer(15000) { AutoReset = false };

        public void StopBackgroundTask()
        {
            StopBackgroundTask(true);
        }

        void StopBackgroundTask(bool aByUserRequest)
        {
            if (IsBusy && mCurrentTaskSuccess == null && mBackgroundThread != null && mBackgroundThread.IsAlive)
            {
                MPTVSeriesLog.Write("Aborting background thread: {0}", mCurrentTaskDescription);
                mBackgroundThread.Abort();
                mAbortedByUser = aByUserRequest;
                return;
            }
        }

        void TaskWatcherTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            StopBackgroundTask(false);
        }

        /// <summary>
        /// This method should be used to call methods in GUI that might take a few seconds.
        /// The Wait Cursor will be shown on while executing the task and the resultHandler will be called on the MP Main thread.
        /// </summary>
        /// <param name="aTask">method to invoke on a background thread</param>
        /// <param name="aResultHandler">method to invoke on the GUI Thread with the result of the task</param>
        /// <param name="aTaskDescription">description of the task to be invoked - will be shown in the error message if execution fails or times out</param>
        /// <param name="aTimeout">true: use the timeout, or false: wait forever</param>
        /// <returns>true, if the task could be successfully started in the background</returns>
        internal bool ExecuteInBackgroundAndCallback(Func<object> aTask, Action<bool, object> aResultHandler, string aTaskDescription, bool aTimeout)
        {
            // make sure only one background task can be executed at a time
            if (!IsBusy && Monitor.TryEnter(this))
            {
                try
                {
                    IsBusy = true;
                    mAbortedByUser = false;
                    mCurrentResultHandler = aResultHandler;
                    mCurrentTaskDescription = aTaskDescription;
                    mCurrentResult = null;
                    mCurrentError = null;

                    // while this is null the task has not finished (or later on timeout), true indicates successfull completion and false error
                    mCurrentTaskSuccess = null;

                    // initialise and show the wait cursor in MediaPortal
                    GUIWaitCursor.Init();
                    GUIWaitCursor.Show();

                    mBackgroundThread = new Thread(delegate()
                    {
                        try
                        {
                            mCurrentResult = aTask.Invoke();
                            mCurrentTaskSuccess = true;
                        }
                        catch (ThreadAbortException)
                        {
                            if (!mAbortedByUser) MPTVSeriesLog.Write("Timeout waiting for results.");
                            Thread.ResetAbort();
                        }
                        catch (Exception threadException)
                        {
                            mCurrentError = threadException;
                            MPTVSeriesLog.Write(threadException.ToString());
                            mCurrentTaskSuccess = false;
                        }
                        mTimeoutTimer.Stop();

                        // hide the wait cursor
                        GUIWaitCursor.Hide();

                        // execute the ResultHandler on the Main Thread
                        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, o) => { ExecuteTaskResultHandler(); return 0; }, 0, 0, null);
                    })
                    { 
                        Name = "MP-TVSeries", 
                        IsBackground = true
                    };

                    // disable timeout when debugging
                    if (aTimeout && !System.Diagnostics.Debugger.IsAttached) mTimeoutTimer.Start();

                    // start backgroun task
                    mBackgroundThread.Start();

                    // successfully started the background task
                    return true;
                }
                catch (Exception ex)
                {
                    MPTVSeriesLog.Write(ex.Message);
                    IsBusy = false;
                    mCurrentResultHandler = null;

                    // hide the wait cursor
                    GUIWaitCursor.Hide();

                    // could not start the background task
                    return false; 
                }
            }
            else
            {
                MPTVSeriesLog.Write("Another thread tried to execute a task in background.");
                return false;
            }
        }

        void ExecuteTaskResultHandler()
        {
            if (!IsBusy) return;

            // show an error message if task was not completed successfully
            if (mCurrentTaskSuccess != true)
            {
                if (mCurrentError != null)
                {
                    string[] lines = new string[] { string.Format("{0} {1}", Translation.Error, mCurrentTaskDescription), mCurrentError.Message };
                    TVSeriesPlugin.ShowDialogOk(TVSeriesPlugin.pluginName, lines);                    
                }
                else
                {
                    if (!mAbortedByUser)
                    {
                        if (mCurrentTaskSuccess.HasValue)
                            TVSeriesPlugin.ShowNotifyDialog(TVSeriesPlugin.pluginName, string.Format("{0} {1}", Translation.Error, mCurrentTaskDescription));
                        else
                            TVSeriesPlugin.ShowNotifyDialog(TVSeriesPlugin.pluginName, string.Format("{0} {1}", Translation.Timeout, mCurrentTaskDescription));
                    }
                }
            }

            // store info needed to invoke the result handler
            bool lStoredTaskSuccess = mCurrentTaskSuccess == true;
            var lStoredHandler = mCurrentResultHandler;
            object lStoredResultObject = mCurrentResult;

            // clear all fields and allow execution of another background task 
            // before actually executing the result handler -> this way a result handler can also invoke another background task)
            mCurrentResultHandler = null;
            mCurrentResult = null;
            mCurrentTaskSuccess = null;
            mCurrentError = null;
            mBackgroundThread = null;
            mAbortedByUser = false;
            IsBusy = false;
            mTimeoutTimer.Stop();
            Monitor.Exit(this);

            // execute the result handler
            if (lStoredHandler != null) lStoredHandler.Invoke(lStoredTaskSuccess, lStoredResultObject);
        }
    }
}