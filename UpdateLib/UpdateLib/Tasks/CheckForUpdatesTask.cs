/*  Copyright
 *  
 *  UpdateLib - .Net auto update library <https://github.com/MatthiWare/UpdateLib>
 *  
 *  File: CheckForUpdatesTask.cs v0.5
 *  
 *  Author: Matthias Beerens
 *  
 *  Copyright (C) 2016 - MatthiWare
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
 *  along with this program.  If not, see <https://github.com/MatthiWare/UpdateLib/blob/master/LICENSE>.
 */

using System;
using System.Net;
using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Utils;
using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Common.Exceptions;
using static MatthiWare.UpdateLib.Tasks.CheckForUpdatesTask;

namespace MatthiWare.UpdateLib.Tasks
{
    public class CheckForUpdatesTask : AsyncTask<CheckForUpdatesResult, CheckForUpdatesTask>
    {
        public Uri Uri { get; set; }

        private WebClient m_wc;

        public CheckForUpdatesTask(Uri uri)
        {
            Uri = uri;
            m_wc = new WebClient();
        }

        protected override void DoWork()
        {
            Result = new CheckForUpdatesResult();

            Updater updater = Updater.Instance;

            if (!NetworkUtils.HasConnection()) throw new NoInternetException("No internet available");

            // Getting the file name from the url
            string localFile = $@"{IOUtils.AppDataPath}\Update.xml";

            if (IsCancelled) return;

            updater.Logger.Debug(nameof(CheckForUpdatesTask), nameof(DoWork), $"Attempting to download update file from {Uri}");
            m_wc.DownloadFile(Uri, localFile);

            // load the updatefile from disk
            Result.UpdateInfo = FileManager.LoadFile<UpdateFile>(localFile).GetLatestUpdate() ?? throw new NoVersionSpecifiedException();

            var privilegesCheckTask = CheckPrivileges(Result.UpdateInfo);

            // lets wait for the Cache update to complete and get the task
            var cache = updater.GetCache();

            // Wait for the clean up to complete
            updater.CleanUpTask.AwaitTask();

            if (IsCancelled)
                return;

            /* 
             * Start a task to get all the files that need to be updated
             * Returns if there is anything to update
             */
            var updatedFilesTask = CheckForUpdatedFiles(Result.UpdateInfo, cache);

            Result.AdminRightsNeeded = privilegesCheckTask.AwaitTask().Result;
            Result.UpdateAvailable = updatedFilesTask.AwaitTask().Result;
        }

        private CheckForUpdatedItemsTask CheckForUpdatedFiles(UpdateInfo updateInfo, HashCacheFile cache)
            => new CheckForUpdatedItemsTask(updateInfo, cache).ConfigureAwait(false).Start();

        private CheckRequiredPrivilegesTask CheckPrivileges(UpdateInfo updateInfo)
            => new CheckRequiredPrivilegesTask(updateInfo).ConfigureAwait(false).Start();

        public class CheckForUpdatesResult
        {
            public UpdateVersion Version { get { return UpdateInfo.Version; } }
            public bool UpdateAvailable { get; set; } = false;
            public UpdateInfo UpdateInfo { get; set; }
            public bool AdminRightsNeeded { get; set; } = false;
        }
    }
}
