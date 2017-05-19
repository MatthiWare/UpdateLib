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
            amountToDownload.Value = file.Count;
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

                Logger.Error(GetType().Name, e.Error);
            }

            if (amountToDownload.Decrement() == 0)
            {
                RestartApp();
            }
        }

        private void CancelOtherTasks()
        {
            foreach (DownloadTask task in tasks)
                task.Cancel();
        }

        private void RestartApp()
        {
            Updater instance = Updater.Instance;
            List<string> args = new List<string>(Environment.GetCommandLineArgs());

            if (instance.EnableCmdArguments)
                for (int i = 0; i < args.Count; i++)
                    if (args[i] == instance.StartUpdatingCmdArg || args[i] == instance.UpdateSilentlyCmdArg)
                        args[i] = string.Empty;

            args.Add(instance.WaitForProcessCmdArg);
            args.Add(Process.GetCurrentProcess().Id.ToString());

            string arguments = args.Where(a => !string.IsNullOrEmpty(a)).Distinct().AppendAll(" ");

            string startupPath = Application.StartupPath;

            ProcessStartInfo startInfo = new ProcessStartInfo(startupPath, arguments);
            Process.Start(startInfo);

            Environment.Exit(0);
        }
    }
}
