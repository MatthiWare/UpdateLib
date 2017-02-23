using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        /// <summary>
        /// Raises when this <see cref="AsyncTaskBase"/> is completed. 
        /// </summary>
        public event EventHandler<AsyncCompletedEventArgs> TaskCompleted;
        /// <summary>
        /// Raises when the <see cref="AsyncTaskBase"/> progress changed. 
        /// </summary>
        public event EventHandler<ProgressChangedEventArgs> TaskProgressChanged;

        /// <summary>
        /// Starts the task
        /// </summary>
        public void Start()
        {
            Action a = new Action(() =>
            {
                Exception taskException = null;

#if DEBUG
                m_sw.Reset();
                m_sw.Start();
#endif

                try
                {
                    DoWork();
                    AwaitWorker();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"[ERROR][AsyncTask]: {e.Message}\n{e.StackTrace}");
                    taskException = e;
                }

                OnTaskCompleted(taskException);

#if DEBUG
                m_sw.Stop();
                Console.WriteLine("[{0}]: Completed in {1}ms", GetType().FullName, m_sw.ElapsedMilliseconds);
#endif
            });

            mainWait = a.BeginInvoke(new AsyncCallback(r => a.EndInvoke(r)), null).AsyncWaitHandle;
        }

        /// <summary>
        /// The worker method.
        /// </summary>
        public abstract void DoWork();

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
        /// Blocks the calling thread untill the task is done
        /// </summary>
        public void AwaitTask()
        {
            mainWait.WaitOne();
            mainWait.Close();
        }

        private void AwaitWorker()
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
    public abstract class AsyncTask<T> : AsyncTaskBase
    {
        /// <summary>
        /// The result <see cref="T"/> 
        /// </summary>
        public T Result { get; set; }
    }
}
