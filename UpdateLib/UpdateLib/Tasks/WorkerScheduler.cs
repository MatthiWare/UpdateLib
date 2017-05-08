using MatthiWare.UpdateLib.Logging;
using MatthiWare.UpdateLib.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MatthiWare.UpdateLib.Tasks
{
    internal class WorkerScheduler
    {

        #region Singleton
        private static volatile WorkerScheduler instance = null;
        private static readonly object synclock = new object();

        /// <summary>
        /// Gets a thread safe instance of <see cref="WorkerScheduler"/> 
        /// </summary>
        public static WorkerScheduler Instance
        {
            get
            {
                if (instance == null)
                    lock (synclock)
                        if (instance == null)
                            instance = new WorkerScheduler();

                return instance;
            }
        }
        #endregion

        #region Fields

        private readonly long MAX_WORKERS;
        private readonly ConcurrentQueue<AsyncTask> m_taskQueue;
        private readonly AtomicInteger m_currentWorkerCount;
        private readonly AsyncTask m_dispatcherTask;
        private readonly ManualResetEvent m_waitForAvailableWorker;

        #endregion

        private WorkerScheduler()
        {
            MAX_WORKERS = Environment.ProcessorCount;
            m_taskQueue = new ConcurrentQueue<AsyncTask>();
            m_dispatcherTask = AsyncTaskFactory.From(new Action(Dispatcher), null);
            m_waitForAvailableWorker = new ManualResetEvent(true);
            m_currentWorkerCount = new AtomicInteger();
        }

        public void Schedule(AsyncTask task)
        {
            Enqueue(task);

            lock (synclock)
                if (!m_dispatcherTask.IsRunning)
                    m_dispatcherTask.Start();
        }

        private void Dispatcher()
        {
            AsyncTask task = null;
            while (m_taskQueue.TryDequeue(out task))
            {
                if (task.IsCompleted || task.IsCancelled || task.HasErrors)
                    continue;

                SetupTask(task);

                if (m_currentWorkerCount.Value >= MAX_WORKERS)
                    m_waitForAvailableWorker.Reset();

                m_waitForAvailableWorker.WaitOne();

                Logger.Debug(GetType().Name, $"Current worker count: {m_currentWorkerCount.Increment()}");

                task.ConfigureAwait(false).Start();
            }
        }

        private void SetupTask(AsyncTask task)
        {
            task.TaskCompleted += (o, e) =>
            {

                if (m_currentWorkerCount.Decrement() < MAX_WORKERS)
                    m_waitForAvailableWorker.Set();

                Logger.Debug(GetType().Name, $"Current worker count: {m_currentWorkerCount}");
            };
        }

        private void Enqueue(AsyncTask task)
        {
            m_taskQueue.Enqueue(task);
        }

    }
}
