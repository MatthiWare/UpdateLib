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

using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Threading;
using MatthiWare.UpdateLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MatthiWare.UpdateLib.Tasks
{
    public class DownloadManager
    {
        private AtomicInteger m_amountToDownload = new AtomicInteger();
        private UpdateInfo m_updateInfo;

        private List<UpdatableTask> m_tasks = new List<UpdatableTask>();
        private bool hasRegUpdate, hasErrors = false;

        public event EventHandler Completed;

        public DownloadManager(UpdateInfo updateInfo)
        {
            m_amountToDownload.Value = updateInfo.FileCount;
            this.m_updateInfo = updateInfo;
        }

        private void Reset()
        {
            m_tasks.Clear();
            hasRegUpdate = false;
            hasErrors = false;
        }

        public void Update()
        {
            Reset();

            AddUpdates();
            StartUpdate();
        }

        private void StartUpdate()
        {
            IEnumerable<DownloadTask> _tasks = m_tasks.Select(task => task as DownloadTask).NotNull();

            _tasks.ForEach(task => task.Start());

            if (hasRegUpdate && _tasks.Count() == 0)
                StartRegUpdate();
        }

        private void StartRegUpdate()
        {
            m_tasks.Select(task => task as UpdateRegistryTask).NotNull().FirstOrDefault()?.Start();
        }

        private void AddUpdates()
        {
            foreach (FileEntry file in m_updateInfo.Folders
                .SelectMany(dir => dir.GetItems())
                .Select(e => e as FileEntry)
                .Distinct()
                .NotNull())
            {
                DownloadTask task = new DownloadTask(file);
                task.TaskCompleted += Task_TaskCompleted;

                m_tasks.Add(task);
            }

            IEnumerable<RegistryKeyEntry> keys = m_updateInfo.Registry
                .SelectMany(dir => dir.GetItems())
                .Select(e => e as RegistryKeyEntry)
                .Distinct()
                .NotNull();

            if (keys.Count() == 0)
                return;

            hasRegUpdate = true;
            m_amountToDownload.Increment();

            UpdateRegistryTask regTask = new UpdateRegistryTask(keys);
            regTask.TaskCompleted += Task_TaskCompleted;

            m_tasks.Add(regTask);
        }

        private void Task_TaskCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                hasErrors = true;

                CancelOtherTasks();
            }

            int left = m_amountToDownload.Decrement();

            if (hasErrors)
                return;

            if (hasRegUpdate && left == 1)
                StartRegUpdate();
            else if (left == 0)
                Completed?.Invoke(this, EventArgs.Empty);

        }

        private void CancelOtherTasks()
        {
            foreach (UpdatableTask task in m_tasks)
                if (!task.IsCancelled)
                    task.Cancel();
        }

    }
}
