using System;
using System.Net;
using MatthiWare.UpdateLib.Files;
using System.Windows.Forms;
using System.Drawing;
using MatthiWare.UpdateLib.UI;
using static MatthiWare.UpdateLib.Tasks.CheckForUpdatesTask;
using MatthiWare.UpdateLib.Logging;
using System.IO;

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
            if (string.IsNullOrEmpty(Url)) throw new WebException("Invalid Url", WebExceptionStatus.NameResolutionFailure);

            Result = new Data();

            Updater updater = Updater.Instance;

            // Getting the file name from the url
            string localFile = GetLocalFileName();

            if (IsUpdateFileInvalid(localFile))
                wcDownloader.DownloadFile(Url, localFile);

            // load the updatefile from disk
            Result.UpdateFile = UpdateFile.Load(localFile);
            Result.Version = Result.UpdateFile.VersionString;

            // lets wait for the Cache update to complete and get the task
            HashCacheFile cache = updater.GetCache();

            // Wait for the clean up to complete
            updater.CleanUpTask.AwaitTask();

            /* 
             * Start a task to get all the files that need to be updated
             * Returns if there is anything to update
             */
            Result.UpdateAvailable = CheckForUpdatedFiles(Result.UpdateFile, cache).AwaitTask().Result;
        }

        private bool IsUpdateFileInvalid(string localFile)
        {
            if (!File.Exists(localFile))
                return true;

            DateTime time = File.GetLastWriteTime(localFile);

            if (time.Add(Updater.Instance.CacheInvalidationTime) < DateTime.Now)
                return true;

            return false;
        }

        private CheckForUpdatedFilesTask CheckForUpdatedFiles(UpdateFile file, HashCacheFile cache)
        {
            CheckForUpdatedFilesTask task = new CheckForUpdatedFilesTask(file, cache, Updater.Instance.Converter);
            task.ConfigureAwait(false).Start();
            return task;
        }

        private string GetLocalFileName()
        {
            const char slash = '/';
            string[] tokens = Url.Split(slash);
            return string.Concat(".", slash, tokens[tokens.Length - 1]);
        }

        public class Data
        {
            public string Version { get; set; } = string.Empty;
            public bool UpdateAvailable { get; set; } = false;
            public UpdateFile UpdateFile { get; set; }
        }
    }
}
