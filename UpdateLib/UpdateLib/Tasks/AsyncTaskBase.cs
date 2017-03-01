using System;
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

#if DEBUG
        public Stopwatch m_sw = new Stopwatch();
#endif

        private readonly Queue<WaitHandle> waitQueue = new Queue<WaitHandle>();
        private WaitHandle mainWait;
        private readonly object sync = new object();

        private Thread _workerThread;

        private bool cancelled = false;

        /// <summary>
        /// Raises when this <see cref="AsyncTaskBase"/> is completed. 
        /// </summary>
        public event EventHandler<AsyncCompletedEventArgs> TaskCompleted;
        /// <summary>
        /// Raises when the <see cref="AsyncTaskBase"/> progress changed. 
        /// </summary>
        public event EventHandler<ProgressChangedEventArgs> TaskProgressChanged;

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

        /// <summary>
        /// Starts the task
        /// </summary>
        public void Start()
        {
            Action worker = new Action(() =>
            {
                try
                {
                    DoWork();
                }
                catch (Exception e)
                {
                    throw e;
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

                Exception error = null;
                try
                {
                    worker.EndInvoke(r);

#if DEBUG
                    Console.WriteLine($"[{GetType().Name}]: Completed in {m_sw.ElapsedMilliseconds}ms");
#endif
                }
                catch (Exception e)
                {
                    error = e;
                    Console.WriteLine($"[{e.GetBaseException().GetType().Name}][{GetType().Name}]: {e.Message}{e.StackTrace}");
                }

                OnTaskCompleted(error, IsCancelled);

            }), null).AsyncWaitHandle;
        }

        /// <summary>
        /// The worker method.
        /// </summary>
        protected abstract void DoWork();

        /// <summary>
        /// Cancels the current <see cref="AsyncTaskBase"/>
        /// Check <see cref="IsCancelled"/> in the worker code to see if the <see cref="AsyncTaskBase"/> got cancelled.  
        /// </summary>
        public void Cancel()
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
            if (mainWait == null)
                return;

            mainWait.WaitOne();
            mainWait.Close();
            mainWait = null;
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
        /// The result <see cref="T"/> 
        /// </summary>
        public virtual T Result { get; set; }
    }
}
