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

        private List<DownloadTask> tasks = new List<DownloadTask>();
        private bool succes = true;

        public DownloadManager(UpdateFile file)
        {
            amountToDownload.Value = file.FileCount;
            this.file = file;
        }

        public void Update()
        {
            foreach (DirectoryEntry dir in file.Folders)
                UpdateFilesFromDirectoryEntry(dir);
        }

        private void UpdateFilesFromDirectoryEntry(DirectoryEntry dir)
        {
            foreach (FileEntry f in dir.Files)
                UpdateEntry(f);

            foreach (DirectoryEntry d in dir.Directories)
                UpdateFilesFromDirectoryEntry(d);
        }

        private void UpdateEntry(FileEntry entry)
        {
            DownloadTask task = new DownloadTask(entry);
            task.TaskCompleted += Task_TaskCompleted;

            tasks.Add(task);

            task.Start();
        }

        private void Task_TaskCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                succes = false;
                CancelOtherTasks();

                Updater.Instance.Logger.Error(nameof(DownloadManager), nameof(Update), e.Error);
            }

            if (amountToDownload.Decrement() == 0)
            {
                Updater.Instance.GetCache().Save();
                Updater.Instance.RestartApp();
            }

        }

        private void CancelOtherTasks()
        {
            foreach (DownloadTask task in tasks)
                if (!task.IsCancelled)
                    task.Cancel();
        }

    }
}
