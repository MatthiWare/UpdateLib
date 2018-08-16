/*  Copyright
 *  
 *  UpdateLib - .Net auto update library <https://github.com/MatthiWare/UpdateLib>
 *  
 *  File: AsyncTask.cs v0.5
 *  
 *  Author: Matthias Beerens
 *  
 *  Copyright (C) 2016 - MatthiWare
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published
 *  by the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <https://github.com/MatthiWare/UpdateLib/blob/master/LICENSE>.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

using MatthiWare.UpdateLib.Common;

namespace MatthiWare.UpdateLib.Tasks
{
    /// <summary>
    /// Base class for all Tasks that need to be run Async
    /// </summary>
    public abstract class AsyncTask
    {

        #region private fields

        protected Exception m_lastException = null;

        private bool m_useSyncContext = true;
        private SynchronizationContext m_syncContext;

        internal bool IsChildTask { get; set; } = false;

#if DEBUG
        public Stopwatch m_sw = new Stopwatch();
#endif

        private readonly Queue<AsyncTask> m_childTasks = new Queue<AsyncTask>();
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
        }



        /// <summary>
        /// Gets if the current <see cref="AsyncTask"/> is Running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                lock (sync)
                    return m_running;
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

        /// <summary>
        /// Starts the task
        /// </summary>
        /// <returns>Returns the current Task.</returns>
        public AsyncTask Start()
        {
            lock (sync)
            {
                if (m_running)
                    return this;

                Reset();

                m_syncContext = SynchronizationContext.Current;

                Action worker = new Action(() =>
                {
                    try
                    {
                        m_running = true;
                        DoWork();
                    }
                    catch (Exception ex)
                    {
                        m_lastException = ex;

                        Updater.Instance.Logger.Error(GetType().Name, null, ex);
                    }
                    finally
                    {
                        AwaitWorkers();

                        m_running = false;
                        m_completed = true;
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
        }

        /// <summary>
        /// Blocks the calling thread until the complete task is done.
        /// DO NOT call this in the worker method use <see cref="AwaitWorkers"/> method instead. 
        /// </summary>
        public AsyncTask AwaitTask()
        {
            lock (sync)
            {
                if (IsChildTask && !m_completed && !m_running)
                    Reset();

                if (m_waitHandle != null)
                {
                    m_waitHandle.WaitOne();
                    m_waitHandle.Close();
                    m_waitHandle = null;
                }

                return this;
            }
        }

        #endregion

        /// <summary>
        /// Resets the task back to its initial state
        /// </summary>
        private void Reset()
        {
            m_cancelled = false;
            m_running = false;
            m_lastException = null;
            m_completed = false;

            m_waitHandle.Reset();
            m_childTasks.Clear();

#if DEBUG
            m_sw.Reset();
#endif
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
            lock (sync)
                m_cancelled = true;
        }

        /// <summary>
        /// Adds a new inner task
        /// </summary>
        /// <param name="action"></param>
        /// <param name="args">Optional arguments for the action</param>
        protected void Enqueue(Delegate action, params object[] args)
        {
            lock (sync)
            {
                // Don't allow to start another child task when the parent task has been cancelled or contains errors.
                if (m_lastException != null || m_cancelled)
                    return;

                var task = AsyncTaskFactory.From(action, args);

                task.IsChildTask = true;
                task.TaskCompleted += (o, e) =>
                {
                    if (e.Error != null)
                    {
                        m_lastException = e.Error?.InnerException ?? e.Error;

                        Updater.Instance.Logger.Error(GetType().Name, null, LastException);
                    }
                };

                m_childTasks.Enqueue(task);

                WorkerScheduler.Instance.Schedule(task);
            }
        }

        /// <summary>
        /// Blocks the calling thread until all the workers are done.
        /// </summary>
        protected void AwaitWorkers()
        {
            lock (sync)
                while (m_childTasks.Count != 0)
                    m_childTasks.Dequeue().AwaitTask();
        }

        /// <summary>
        /// Raises the <see cref="TaskProgressChanged"/> event.  
        /// </summary>
        /// <param name="done">The amount of work that is done.</param>
        /// <param name="total">The total amount of work.</param>
        protected virtual void OnTaskProgressChanged(int done, int total)
            => OnTaskProgressChanged((int)((done / (double)total) * 100));

        /// <summary>   /// Raises the <see cref="TaskProgressChanged"/> event.  
        /// </summary>
        /// <param name="percent">The percentage of work that is done.</param>
        protected virtual void OnTaskProgressChanged(int percent)
            => OnTaskProgressChanged(new ProgressChangedEventArgs(percent, null));

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
            => OnTaskCompleted(new AsyncCompletedEventArgs(e, cancelled, null));

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
    /// <typeparam name="ResultType">The type of the Result object</typeparam>
    public abstract class AsyncTask<ResultType> : AsyncTask
    {
        /// <summary>
        /// Gets or sets the result <see cref="T"/> 
        /// </summary>
        public virtual ResultType Result { get; protected set; } = default(ResultType);
    }

    /// <summary>
    /// Base class for all Tasks that need to be run Async
    /// </summary>
    /// <typeparam name="TaskType">The task type to be returned from the FluentAPI</typeparam>
    /// <typeparam name="ResultType">The type of the Result object</typeparam>
    public abstract class AsyncTask<ResultType, TaskType> : AsyncTask<ResultType> where TaskType : AsyncTask
    {
        #region FluentAPI

        /// <summary>
        /// Enable if we should switch back to the synchronization context to continue our Task completed. 
        /// </summary>
        /// <remarks>default is true.</remarks>
        /// <param name="useSyncContext">Indicate if we should use  the synchronization context</param>
        /// <returns>The task object for fluent API.</returns>
        public new TaskType ConfigureAwait(bool useSyncContext)
        {
            return (TaskType)base.ConfigureAwait(useSyncContext);
        }

        /// <summary>
        /// Starts the task
        /// </summary>
        /// <returns>Returns the current Task.</returns>
        public new TaskType Start()
        {
            return (TaskType)base.Start();

        }

        /// <summary>
        /// Blocks the calling thread until the complete task is done.
        /// DO NOT call this in the worker method use <see cref="AsyncTask.AwaitWorkers"/> method instead. 
        /// </summary>
        /// <returns><see cref="Result"/></returns>
        public new TaskType AwaitTask()
        {
            return (TaskType)base.AwaitTask();

        }

        #endregion
    }


}
