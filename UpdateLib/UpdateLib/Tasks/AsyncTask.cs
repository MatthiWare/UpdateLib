using MatthiWare.UpdateLib.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace MatthiWare.UpdateLib.Tasks
{
    /// <summary>
    /// Base class for all Tasks that need to be run Async
    /// </summary>
    public abstract class AsyncTask
    {

        #region private fields

        private Exception m_lastException = null;

#if DEBUG
        public Stopwatch m_sw = new Stopwatch();
#endif

        private readonly Queue<WaitHandle> waitQueue = new Queue<WaitHandle>();
        private WaitHandle mainWait;
        private readonly object sync = new object();

        private bool m_running = false;
        private bool m_cancelled = false;

        #endregion

        #region events

        /// <summary>
        /// Raises when this <see cref="AsyncTask"/> is completed. 
        /// </summary>
        public event EventHandler<AsyncCompletedEventArgs> TaskCompleted;
        /// <summary>
        /// Raises when the <see cref="AsyncTask"/> progress changed. 
        /// </summary>
        public event EventHandler<ProgressChangedEventArgs> TaskProgressChanged;

        #endregion

        #region properties

        public Exception LastException
        {
            get
            {
                lock (sync)
                    return m_lastException;
            }
            protected set
            {
                lock (sync)
                    m_lastException = value;
            }
        }

        /// <summary>
        /// Gets if there have been any errors in the task.
        /// </summary>
        public bool HasErrors
        {
            get
            {
                lock (sync)
                    return m_lastException != null;
            }
        }

        /// <summary>
        /// Gets if the current <see cref="AsyncTask"/> is cancelled. 
        /// </summary>
        public bool IsCancelled
        {
            get
            {
                lock (sync)
                    return m_cancelled;
            }
            private set
            {
                lock (sync)
                    m_cancelled = value;
            }
        }


        public bool IsRunning
        {
            get
            {
                lock (sync)
                    return m_running;
            }
            private set
            {
                lock (sync)
                    m_running = value;
            }
        }

        #endregion

        #region static methods

        /// <summary>
        /// Blocks the calling threads and calls <see cref="AsyncTask.AwaitTask"/> on each <see cref="AsyncTask"/> in <paramref name="tasks"/>.
        /// </summary>
        /// <param name="tasks">The tasks to await.</param>
        public static void WaitAll(IEnumerable<AsyncTask> tasks)
        {
            foreach (AsyncTask task in tasks)
            {
                task.AwaitTask();
            }
        }

        #endregion

        /// <summary>
        /// Resets the task back to its initial state
        /// </summary>
        private void Reset()
        {
            IsCancelled = false;
            IsRunning = false;
            m_lastException = null;

            mainWait = null;
            waitQueue.Clear();
        }

        /// <summary>
        /// Starts the task
        /// </summary>
        /// <returns>Returns the current Task.</returns>
        public AsyncTask Start()
        {
            if (IsRunning)
                return this;

            Reset();

            Action worker = new Action(() =>
            {
                try
                {
                    IsRunning = true;
                    DoWork();
                }
                catch (Exception ex)
                {
                    LastException = ex;

                    Logger.Error(GetType().Name, ex);
                }
                finally
                {
                    AwaitWorkers();
                    IsRunning = false;
                }
            });

#if DEBUG
            m_sw.Reset();
            m_sw.Start();
#endif

            mainWait = worker.BeginInvoke(new AsyncCallback((IAsyncResult r) =>
            {
#if DEBUG
                m_sw.Stop();
#endif
                worker.EndInvoke(r);
#if DEBUG
                Logger.Debug(GetType().Name, $"Completed in {m_sw.ElapsedMilliseconds}ms");
#endif
                OnTaskCompleted(m_lastException, IsCancelled);

            }), null).AsyncWaitHandle;

            return this;
        }

        /// <summary>
        /// The worker method.
        /// </summary>
        protected abstract void DoWork();

        /// <summary>
        /// Cancels the current <see cref="AsyncTask"/>
        /// Check <see cref="IsCancelled"/> in the worker code to see if the <see cref="AsyncTask"/> got cancelled.  
        /// </summary>
        public virtual void Cancel()
        {
            IsCancelled = true;
        }

        /// <summary>
        /// Adds a new inner task
        /// </summary>
        /// <param name="action"></param>
        /// <param name="args">Optional arguments for the action</param>
        protected void Enqueue(Delegate action, params object[] args)
        {
            Action subTaskAction = new Action(() =>
            {
                try
                {
                    action.DynamicInvoke(args);
                }
                catch (Exception ex)
                {
                    LastException = ex?.InnerException ?? ex;

                    Logger.Error(GetType().Name, LastException);
                }
            });

            // Don't allow to start another task when the parent task has been cancelled or contains errors.
            if (HasErrors || IsCancelled)
                return;

            IAsyncResult result = subTaskAction.BeginInvoke(new AsyncCallback(r => { subTaskAction.EndInvoke(r); }), null);
            lock (sync)
                waitQueue.Enqueue(result.AsyncWaitHandle);
        }

        /// <summary>
        /// Blocks the calling thread until the complete task is done.
        /// DO NOT call this in the worker method use <see cref="AwaitWorkers"/> method instead. 
        /// </summary>
        public void AwaitTask()
        {
            if (mainWait != null)
            {
                mainWait.WaitOne();
                mainWait.Close();
                mainWait = null;
            }
        }

        /// <summary>
        /// Blocks the calling thread until all the workers are done.
        /// </summary>
        protected void AwaitWorkers()
        {
            while (waitQueue.Count > 0)
            {
                WaitHandle wh = null;
                lock (sync)
                    wh = waitQueue.Dequeue();

                wh.WaitOne();
                wh.Close();
            }
        }

        /// <summary>
        /// Raises the <see cref="TaskProgressChanged"/> event.  
        /// </summary>
        /// <param name="done">The amount of work that is done.</param>
        /// <param name="total">The total amount of work.</param>
        protected virtual void OnTaskProgressChanged(int done, int total)
        {
            double progress = ((double)done / total) * 100;
            TaskProgressChanged?.Invoke(this, new ProgressChangedEventArgs((int)progress, null));
        }

        /// <summary>
        /// Raises the <see cref="TaskProgressChanged"/> event.  
        /// </summary>
        /// <param name="percent">The percentage of work that is done.</param>
        protected virtual void OnTaskProgressChanged(int percent)
        {
            TaskProgressChanged?.Invoke(this, new ProgressChangedEventArgs(percent, null));
        }

        /// <summary>
        /// Raises the <see cref="TaskProgressChanged"/> event.  
        /// </summary>
        /// <param name="e">The <see cref="ProgressChangedEventArgs"/> event.</param>
        protected virtual void OnTaskProgressChanged(ProgressChangedEventArgs e)
        {
            TaskProgressChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="TaskCompleted"/> event. 
        /// </summary>
        /// <param name="e">If an <see cref="Exception"/> occured pass the <see cref="Exception"/> object.</param>
        /// <param name="cancelled">Indicates whether the <see cref="AsyncTask"/> got cancelled.</param>
        protected virtual void OnTaskCompleted(Exception e, bool cancelled = false)
        {
            TaskCompleted?.Invoke(this, new AsyncCompletedEventArgs(e, cancelled, null));
        }

        /// <summary>
        /// Raises the <see cref="TaskCompleted"/> event. 
        /// </summary>
        /// <param name="e">The <see cref="AsyncCompletedEventArgs"/> event.</param>
        protected virtual void OnTaskCompleted(AsyncCompletedEventArgs e)
        {
            TaskCompleted?.Invoke(this, e);
        }
    }

    /// <summary>
    /// Base class for all Tasks that need to be run Async
    /// </summary>
    /// <typeparam name="T">The type of the Result object</typeparam>
    public abstract class AsyncTask<T> : AsyncTask
    {
        /// <summary>
        /// Gets or sets the result <see cref="T"/> 
        /// </summary>
        public virtual T Result { get; protected set; }

        /// <summary>
        /// Starts the task
        /// </summary>
        /// <returns>Returns the current Task.</returns>
        public new AsyncTask<T> Start()
        {
            base.Start();
            return this;
        }

        /// <summary>
        /// Blocks the calling thread until the complete task is done.
        /// DO NOT call this in the worker method use <see cref="AsyncTask.AwaitWorkers"/> method instead. 
        /// </summary>
        /// <returns><see cref="Result"/></returns>
        public new T AwaitTask()
        {
            base.AwaitTask();
            return Result;
        }
    }
}
