using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Logging;
using MatthiWare.UpdateLib.Security;
using MatthiWare.UpdateLib.Threading;
using MatthiWare.UpdateLib.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Tasks
{
    public class DownloadManager
    {
        private AtomicInteger amountToDownload = new AtomicInteger();
        private UpdateFile file;

        private List<UpdatableTask> tasks = new List<UpdatableTask>();
        private bool hasRegUpdate = false;

        public DownloadManager(UpdateFile file)
        {
            amountToDownload.Value = file.FileCount;
            this.file = file;
        }

        private void Reset()
        {
            tasks.Clear();
            hasRegUpdate = false;
        }

        public void Update()
        {
            Reset();

            AddUpdates();
            StartUpdate();
        }

        private void StartUpdate()
        {
            IEnumerable<DownloadTask> _tasks = tasks.Select(task => task as DownloadTask).Where(task => task != null);

            _tasks.ForEach(task => task.Start());

            if (hasRegUpdate && _tasks.Count() == 0)
                StartRegUpdate();
        }

        private void StartRegUpdate()
        {
            tasks.Select(task => task as UpdateRegistryTask).Where(task => task != null).FirstOrDefault()?.Start();
        }

        private void AddUpdates()
        {
            foreach (FileEntry file in file.Folders
                .SelectMany(dir => dir.GetItems())
                .Select(e => e as FileEntry)
                .Distinct()
                .Where(e => e != null))
            {
                DownloadTask task = new DownloadTask(file);
                task.TaskCompleted += Task_TaskCompleted;

                tasks.Add(task);
            }

            IEnumerable<RegistryKeyEntry> keys = file.Registry
                .SelectMany(dir => dir.GetItems())
                .Select(e => e as RegistryKeyEntry)
                .Distinct()
                .Where(e => e != null);

            if (keys.Count() == 0)
                return;

            hasRegUpdate = true;
            amountToDownload.Increment();

            tasks.Add(new UpdateRegistryTask(keys));
        }

        private void Task_TaskCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                CancelOtherTasks();

                Updater.Instance.Logger.Error(nameof(DownloadManager), nameof(Update), e.Error);
            }

            int left = amountToDownload.Decrement();

            if (hasRegUpdate && left == 1)
                StartRegUpdate();
            else if (left == 0)
            {
                Updater.Instance.GetCache().Save();
                Updater.Instance.RestartApp();
            }

        }

        private void CancelOtherTasks()
        {
            foreach (UpdatableTask task in tasks)
                if (!task.IsCancelled)
                    task.Cancel();
        }

    }
}
