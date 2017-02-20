using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace MatthiWare.UpdateLib.Tasks
{
    /// <summary>
    /// Base class for all Tasks that need to be run Async
    /// </summary>
    public abstract class AsyncTask
    {

        private readonly Queue<WaitHandle> waitQueue = new Queue<WaitHandle>();
        private readonly object sync = new object();

        /// <summary>
        /// Raises when this <see cref="AsyncTask"/> is completed. 
        /// </summary>
        public event EventHandler<AsyncCompletedEventArgs> TaskCompleted;
        /// <summary>
        /// Raises when the <see cref="AsyncTask"/> progress changed. 
        /// </summary>
        public event EventHandler<ProgressChangedEventArgs> TaskProgressChanged;

        /// <summary>
        /// Starts the task
        /// </summary>
        public void Start()
        {
            Action a = new Action(() => {
                Exception taskException = null;

                try
                {
                    DoWork();
                    AwaitTask();
                }
                catch (Exception e)
                {
                    taskException = e;
                }

                OnTaskCompleted(taskException);
            });

            a.BeginInvoke(new AsyncCallback(r => a.EndInvoke(r)), null);
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
        /// Raises the <see cref="TaskCompleted"/> event. 
        /// </summary>
        /// <param name="e">If an <see cref="Exception"/> occured pass the <see cref="Exception"/> object.</param>
        /// <param name="cancelled">Indicates whether the <see cref="AsyncTask"/> got cancelled.</param>
        protected virtual void OnTaskCompleted(Exception e, bool cancelled=false)
        {
            TaskCompleted?.Invoke(this, new AsyncCompletedEventArgs(e, cancelled, null));
        }
    }

    /// <summary>
    /// Base class for all Tasks that need to be run Async
    /// </summary>
    /// <typeparam name="T">The type of the Result object</typeparam>
    public abstract class AsyncTask<T> : AsyncTask where T : class
    {
        /// <summary>
        /// The result <see cref="T"/> 
        /// </summary>
        public T Result { get; set; }
    }
}
