using MatthiWare.UpdateLib.Logging;
using MatthiWare.UpdateLib.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Linq;

namespace MatthiWare.UpdateLib.Tasks
{
    /// <summary>
    /// Base class for all Tasks that need to be run Async
    /// </summary>
    public abstract class AsyncTask
    {

        #region private fields

        private Exception m_lastException = null;

        private bool m_useSyncContext = true;
        private SynchronizationContext m_syncContext;

        public bool IsChildTask { get; internal set; } = false;

#if DEBUG
        public Stopwatch m_sw = new Stopwatch();
#endif

        private readonly ConcurrentQueue<AsyncTask> m_childTasks = new ConcurrentQueue<AsyncTask>();
        private ManualResetEvent m_waitHandle = new ManualResetEvent(true);
        private readonly object sync = new object();

        private bool m_running = false;
        private bool m_cancelled = false;
        private bool m_completed = false;

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

        public bool IsCompleted
        {
            get
            {
                lock (sync)
                    return m_completed;
            }
            set
            {
                lock (sync)
                    m_completed = value;
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
                task.AwaitTask();
        }

        #endregion

        #region FluentAPI

        /// <summary>
        /// Enable if we should switch back to the synchronization context to continue our Task completed. 
        /// </summary>
        /// <remarks>default is true.</remarks>
        /// <param name="useSyncContext">Indicate if we should use  the synchronization context</param>
        /// <returns>The task object for fluent API.</returns>
        public AsyncTask ConfigureAwait(bool useSyncContext)
        {
            m_useSyncContext = useSyncContext;
            return this;
        }

        #endregion

        /// <summary>
        /// Resets the task back to its initial state
        /// </summary>
        private void Reset()
        {
            IsCancelled = false;
            IsRunning = false;
            LastException = null;
            IsCompleted = false;

            m_waitHandle.Reset();
            m_childTasks.Clear();

#if DEBUG
            m_sw.Reset();
#endif
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

            m_syncContext = SynchronizationContext.Current;

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

                    Updater.Instance.Logger.Error(GetType().Name, null, ex);
                }
                finally
                {
                    AwaitWorkers();
                    IsRunning = false;
                    IsCompleted = true;
                }
            });

#if DEBUG
            m_sw.Start();
#endif

            worker.BeginInvoke(new AsyncCallback((IAsyncResult r) =>
            {
#if DEBUG
                m_sw.Stop();
                Updater.Instance.Logger.Debug(GetType().Name, nameof(Start), $"Completed in {m_sw.ElapsedMilliseconds}ms");
#endif
                worker.EndInvoke(r);

                OnTaskCompleted(m_lastException, IsCancelled);

                m_waitHandle.Set();

            }), null); ;

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
            // Don't allow to start another task when the parent task has been cancelled or contains errors.
            if (HasErrors || IsCancelled)
                return;

            AsyncTask task = AsyncTaskFactory.From(action, args);
            task.IsChildTask = true;
            task.TaskCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    LastException = e.Error?.InnerException ?? e.Error;

                    Updater.Instance.Logger.Error(GetType().Name, null, LastException);
                }
            };

            m_childTasks.Enqueue(task);

            WorkerScheduler.Instance.Schedule(task);
        }

        /// <summary>
        /// Blocks the calling thread until the complete task is done.
        /// DO NOT call this in the worker method use <see cref="AwaitWorkers"/> method instead. 
        /// </summary>
        public AsyncTask AwaitTask()
        {
            if (IsChildTask && !IsCompleted && !IsRunning)
                Reset();

            if (m_waitHandle != null)
            {
                m_waitHandle.WaitOne();
                m_waitHandle.Close();
                m_waitHandle = null;
            }

            return this;
        }

        private int x = 0;

        /// <summary>
        /// Blocks the calling thread until all the workers are done.
        /// </summary>
        protected void AwaitWorkers()
        {
            AsyncTask task = null;
            while (m_childTasks.TryDequeue(out task))
                task.AwaitTask();
        }

        /// <summary>
        /// Raises the <see cref="TaskProgressChanged"/> event.  
        /// </summary>
        /// <param name="done">The amount of work that is done.</param>
        /// <param name="total">The total amount of work.</param>
        protected virtual void OnTaskProgressChanged(int done, int total)
        {
            double percent = ((double)done / total) * 100;
            OnTaskProgressChanged((int)percent);
        }

        /// <summary>   /// Raises the <see cref="TaskProgressChanged"/> event.  
        /// </summary>
        /// <param name="percent">The percentage of work that is done.</param>
        protected virtual void OnTaskProgressChanged(int percent)
        {
            OnTaskProgressChanged(new ProgressChangedEventArgs(percent, null));
        }

        private int m_lastProgressUpdate = -1;

        /// <summary>
        /// Raises the <see cref="TaskProgressChanged"/> event.  
        /// </summary>
        /// <param name="e">The <see cref="ProgressChangedEventArgs"/> event.</param>
        protected virtual void OnTaskProgressChanged(ProgressChangedEventArgs e)
        {
            // filter out redundant calls
            if (m_lastProgressUpdate == e.ProgressPercentage)
                return;

            m_lastProgressUpdate = e.ProgressPercentage;

            if (!m_useSyncContext || m_syncContext == null)
                TaskProgressChanged?.Invoke(this, e);
            else
                m_syncContext.Post(new SendOrPostCallback((o) => TaskProgressChanged?.Invoke(this, e)), null);

        }

        /// <summary>
        /// Raises the <see cref="TaskCompleted"/> event. 
        /// </summary>
        /// <param name="e">If an <see cref="Exception"/> occured pass the <see cref="Exception"/> object.</param>
        /// <param name="cancelled">Indicates whether the <see cref="AsyncTask"/> got cancelled.</param>
        protected virtual void OnTaskCompleted(Exception e, bool cancelled = false)
        {
            OnTaskCompleted(new AsyncCompletedEventArgs(e, cancelled, null));
        }

        /// <summary>
        /// Raises the <see cref="TaskCompleted"/> event. 
        /// </summary>
        /// <param name="e">The <see cref="AsyncCompletedEventArgs"/> event.</param>
        protected virtual void OnTaskCompleted(AsyncCompletedEventArgs e)
        {
            if (!m_useSyncContext || m_syncContext == null)
                TaskCompleted?.Invoke(this, e);
            else
                m_syncContext.Post(new SendOrPostCallback((o) => TaskCompleted?.Invoke(this, e)), null);
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

        #region FluentAPI

        /// <summary>
        /// Enable if we should switch back to the synchronization context to continue our Task completed. 
        /// </summary>
        /// <remarks>default is true.</remarks>
        /// <param name="useSyncContext">Indicate if we should use  the synchronization context</param>
        /// <returns>The task object for fluent API.</returns>
        public new AsyncTask<T> ConfigureAwait(bool useSyncContext)
        {
            return (AsyncTask<T>)base.ConfigureAwait(useSyncContext);
        }

        /// <summary>
        /// Starts the task
        /// </summary>
        /// <returns>Returns the current Task.</returns>
        public new AsyncTask<T> Start()
        {
            return (AsyncTask<T>)base.Start();

        }

        /// <summary>
        /// Blocks the calling thread until the complete task is done.
        /// DO NOT call this in the worker method use <see cref="AsyncTask.AwaitWorkers"/> method instead. 
        /// </summary>
        /// <returns><see cref="Result"/></returns>
        public new AsyncTask<T> AwaitTask()
        {
            return (AsyncTask<T>)base.AwaitTask();

        }

        #endregion


    }
}
