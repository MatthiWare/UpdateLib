/*  UpdateLib - .Net auto update library
 *  Copyright (C) 2016 - MatthiWare (Matthias Beerens)
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
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using MatthiWare.UpdateLib.Threading;
using System;
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
        private readonly object sync = new object();

        public const long MIN_WORKERS = 8;

        #endregion

        private WorkerScheduler()
        {
            MAX_WORKERS = Math.Max(Environment.ProcessorCount, MIN_WORKERS);
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
                lock (sync)
                {
                    if (task.IsCompleted || task.IsCancelled || task.HasErrors)
                        continue;

                    SetupTask(task);

                    if (m_currentWorkerCount.Value >= MAX_WORKERS)
                        m_waitForAvailableWorker.Reset();

                    m_waitForAvailableWorker.WaitOne();

                    Updater.Instance.Logger.Debug(nameof(WorkerScheduler), nameof(Dispatcher), $"Current worker count: {m_currentWorkerCount.Increment()} | Current queue count: {m_taskQueue.Count}");

                    task.ConfigureAwait(false).Start();
                }
            }
        }

        private void SetupTask(AsyncTask task)
        {
            task.TaskCompleted += (o, e) =>
            {

                if (m_currentWorkerCount.Decrement() < MAX_WORKERS)
                    m_waitForAvailableWorker.Set();

                Updater.Instance.Logger.Debug(nameof(WorkerScheduler), nameof(Dispatcher), $"Current worker count: {m_currentWorkerCount}");
            };
        }

        private void Enqueue(AsyncTask task)
        {
            m_taskQueue.Enqueue(task);
        }

    }
}
