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
    public abstract class AsyncTaskBase
    {

        #region private fields

#if DEBUG
        public Stopwatch m_sw = new Stopwatch();
#endif

        private readonly Queue<WaitHandle> waitQueue = new Queue<WaitHandle>();
        private WaitHandle mainWait;
        private readonly object sync = new object();

        private bool cancelled = false;

        #endregion

        #region events

        /// <summary>
        /// Raises when this <see cref="AsyncTaskBase"/> is completed. 
        /// </summary>
        public event EventHandler<AsyncCompletedEventArgs> TaskCompleted;
        /// <summary>
        /// Raises when the <see cref="AsyncTaskBase"/> progress changed. 
        /// </summary>
        public event EventHandler<ProgressChangedEventArgs> TaskProgressChanged;

        #endregion

        #region properties

        /// <summary>
        /// Gets if the current <see cref="AsyncTaskBase"/> is cancelled. 
        /// </summary>
        public bool IsCancelled
        {
            get
            {
                lock (sync)
                    return cancelled;
            }
        }

        #endregion

        #region static methods

        /// <summary>
        /// Blocks the calling threads and calls <see cref="AsyncTaskBase.AwaitTask"/> on each <see cref="AsyncTaskBase"/> in <paramref name="tasks"/>.
        /// </summary>
        /// <param name="tasks">The tasks to await.</param>
        public static void WaitAll(IEnumerable<AsyncTaskBase> tasks)
        {
            foreach (AsyncTaskBase task in tasks)
            {
                task.AwaitTask();
            }
        }

        #endregion

        /// <summary>
        /// Starts the task
        /// </summary>
        /// <returns>Returns the current Task.</returns>
        public AsyncTaskBase Start()
        {
            Exception taskException = null;
            Action worker = new Action(() =>
            {
                try
                {
                    DoWork();
                }
                catch (Exception ex)
                {
                    taskException = ex;
                    Logger.Error(GetType().Name, ex);
                }
                finally
                {
                    AwaitWorkers();
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
                OnTaskCompleted(taskException, IsCancelled);

            }), null).AsyncWaitHandle;

            return this;
        }

        /// <summary>
        /// The worker method.
        /// </summary>
        protected abstract void DoWork();

        /// <summary>
        /// Cancels the current <see cref="AsyncTaskBase"/>
        /// Check <see cref="IsCancelled"/> in the worker code to see if the <see cref="AsyncTaskBase"/> got cancelled.  
        /// </summary>
        public virtual void Cancel()
        {
            lock (sync)
                cancelled = true;
        }

        /// <summary>
        /// Adds a new wait object to the queue
        /// </summary>
        /// <param name="waitHandle">The wait object</param>
        public void Enqueue(WaitHandle waitHandle)
        {
            lock (sync)
                waitQueue.Enqueue(waitHandle);
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
            int progress = (done * 100) / total;
            TaskProgressChanged?.Invoke(this, new ProgressChangedEventArgs(progress, null));
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
        /// <param name="cancelled">Indicates whether the <see cref="AsyncTaskBase"/> got cancelled.</param>
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
    public abstract class AsyncTaskBase<T> : AsyncTaskBase
    {
        /// <summary>
        /// Gets or sets the result <see cref="T"/> 
        /// </summary>
        public virtual T Result { get; protected set; }

        /// <summary>
        /// Starts the task
        /// </summary>
        /// <returns>Returns the current Task.</returns>
        public new AsyncTaskBase<T> Start()
        {
            base.Start();
            return this;
        }

        /// <summary>
        /// Blocks the calling thread until the complete task is done.
        /// DO NOT call this in the worker method use <see cref="AsyncTaskBase.AwaitWorkers"/> method instead. 
        /// </summary>
        /// <returns><see cref="Result"/></returns>
        public new T AwaitTask()
        {
            base.AwaitTask();
            return Result;
        }
    }
}
