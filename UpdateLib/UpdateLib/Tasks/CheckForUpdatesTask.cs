using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using MatthiWare.UpdateLib.Files;
using System.Windows.Forms;
using System.Drawing;
using MatthiWare.UpdateLib.UI;

namespace MatthiWare.UpdateLib.Tasks
{
    public class CheckForUpdatesTask : AsyncTaskBase<bool>
    {
        public string Url { get; set; }

        private WebClient wcDownloader;

        public CheckForUpdatesTask(string url)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));

            Url = url;

            wcDownloader = new WebClient();
        }

        protected override void DoWork()
        {
            if (string.IsNullOrEmpty(Url)) throw new WebException("Invalid Url");

            // Getting the file name from the url
            string localFile = GetLocalFileName();

            // downloading the file synchronous
            wcDownloader.DownloadFile(Url, localFile);

            // load the updatefile from disk
            UpdateFile file = UpdateFile.Load(localFile);

            // lets wait for the Cache update to complete and get the task
            HashCacheFile cache = Updater.Instance.GetCache();

            // Wait for the clean up to complete
            Updater.Instance.CleanUpTask.AwaitTask();

            // Start a task to get all the files that need to be updated
            CheckForUpdatedFilesTask checkForUpdateNeeded = CheckForUpdatedFiles(file, cache);
            checkForUpdateNeeded.AwaitTask();

            // setting the output of the task
            Result = checkForUpdateNeeded.Result;

            if (!Result) // no updates available
                return;

            DialogResult result = new MessageDialog(
                    "Update available",
                    $"Version {file.VersionString} available",
                    "Update now?\nPress yes to update or no to cancel.",
                    SystemIcons.Question).ShowDialog();


            if (result != DialogResult.Yes)
            {
                Cancel();
                return;
            }

            UpdaterForm updateForm = new UpdaterForm(file);
            updateForm.ShowDialog();
        }

        private CheckForUpdatedFilesTask CheckForUpdatedFiles(UpdateFile file, HashCacheFile cache)
        {
            CheckForUpdatedFilesTask task = new CheckForUpdatedFilesTask(file, cache, Updater.Instance.Converter);
            task.Start();
            return task;
        }

        private string GetLocalFileName()
        {
            string[] tokens = Url.Split('/');
            return string.Concat("./", tokens[tokens.Length - 1]);
        }
    }
}
