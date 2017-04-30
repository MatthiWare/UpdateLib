﻿using System;
using System.Net;
using MatthiWare.UpdateLib.Files;
using System.Windows.Forms;
using System.Drawing;
using MatthiWare.UpdateLib.UI;
using static MatthiWare.UpdateLib.Tasks.CheckForUpdatesTask;

namespace MatthiWare.UpdateLib.Tasks
{
    public class CheckForUpdatesTask : AsyncTask<Data>
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

            Result = new Data();

            // Getting the file name from the url
            string localFile = GetLocalFileName();

            // downloading the file synchronous
            wcDownloader.DownloadFile(Url, localFile);

            // load the updatefile from disk
            UpdateFile file = UpdateFile.Load(localFile);
            Result.Version = file.VersionString;

            // lets wait for the Cache update to complete and get the task
            HashCacheFile cache = Updater.Instance.GetCache();

            // Wait for the clean up to complete
            Updater.Instance.CleanUpTask.AwaitTask();

            /* 
             * Start a task to get all the files that need to be updated
             * Returns if there is anything to update
             */
            Result.UpdateAvailable = CheckForUpdatedFiles(file, cache).AwaitTask();
            
            if (!Result.UpdateAvailable) // no updates available
                return;

            DialogResult result = MessageDialog.Show(
                    "Update available",
                    $"Version {Result.Version} available",
                    "Update now?\nPress yes to update or no to cancel.",
                    SystemIcons.Question);


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

        public class Data
        {
            public string Version { get; set; } = "";
            public bool UpdateAvailable { get; set; } = false;
        }
    }
}
