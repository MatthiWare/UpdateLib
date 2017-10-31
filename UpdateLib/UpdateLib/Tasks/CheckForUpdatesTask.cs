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

using System;
using System.Net;
using MatthiWare.UpdateLib.Files;
using System.IO;
using MatthiWare.UpdateLib.Utils;
using static MatthiWare.UpdateLib.Tasks.CheckForUpdatesTask;

namespace MatthiWare.UpdateLib.Tasks
{
    public class CheckForUpdatesTask : AsyncTask<CheckForUpdatesResult>
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

            Result = new CheckForUpdatesResult();

            Updater updater = Updater.Instance;

            if (!NetworkUtils.HasConnection()) throw new WebException("No internet available", WebExceptionStatus.ConnectFailure);

            // Getting the file name from the url
            string localFile = $@"{IOUtils.AppDataPath}\Update.xml";

            if (IsCancelled)
                return;

            if (IsUpdateFileInvalid(localFile))
            {
                updater.Logger.Warn(nameof(CheckForUpdatesTask), nameof(DoWork), "Cached update file validity expired, downloading new one..");
                wcDownloader.DownloadFile(Url, localFile);
            }

            // load the updatefile from disk
            Result.UpdateFile = UpdateFile.Load(localFile);
            Result.Version = Result.UpdateFile.VersionString;

            CheckRequiredPrivilegesTask privilegesCheckTask = CheckPrivileges(Result.UpdateFile);

            // lets wait for the Cache update to complete and get the task
            HashCacheFile cache = updater.GetCache();

            // Wait for the clean up to complete
            updater.CleanUpTask.AwaitTask();

            if (IsCancelled)
                return;

            /* 
             * Start a task to get all the files that need to be updated
             * Returns if there is anything to update
             */
            CheckForUpdatedItemsTask updatedFilesTask = CheckForUpdatedFiles(Result.UpdateFile, cache);

            Result.AdminRightsNeeded = privilegesCheckTask.AwaitTask().Result;
            Result.UpdateAvailable = updatedFilesTask.AwaitTask().Result;
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

        private CheckForUpdatedItemsTask CheckForUpdatedFiles(UpdateFile file, HashCacheFile cache)
        {
            CheckForUpdatedItemsTask task = new CheckForUpdatedItemsTask(file, cache);
            task.ConfigureAwait(false).Start();
            return task;
        }

        private CheckRequiredPrivilegesTask CheckPrivileges(UpdateFile file)
        {
            CheckRequiredPrivilegesTask task = new CheckRequiredPrivilegesTask(file);
            task.ConfigureAwait(false).Start();
            return task;
        }

        public class CheckForUpdatesResult
        {
            public string Version { get; set; } = string.Empty;
            public bool UpdateAvailable { get; set; } = false;
            public UpdateFile UpdateFile { get; set; }
            public bool AdminRightsNeeded { get; set; } = false;
        }
    }
}
