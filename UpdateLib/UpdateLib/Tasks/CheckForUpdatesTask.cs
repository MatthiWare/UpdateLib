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
using System.Collections.Generic;
using System.Net;

using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Common.Abstraction;
using MatthiWare.UpdateLib.Common.Exceptions;
using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Utils;

using static MatthiWare.UpdateLib.Tasks.CheckForUpdatesTask;

namespace MatthiWare.UpdateLib.Tasks
{
    public class CheckForUpdatesTask : AsyncTask<CheckForUpdatesResult, CheckForUpdatesTask>
    {
        public IList<string> Urls { get; set; }

        private WebClient m_wc;
        private UpdateVersion m_version;
        private Updater m_updater;

        private const string REPLACE_FILE_NAME = "%file%";

        public CheckForUpdatesTask(IList<string> urls, UpdateVersion currentVersion)
        {
            Urls = urls ?? throw new ArgumentNullException(nameof(urls));
            m_version = currentVersion ?? throw new ArgumentNullException(nameof(currentVersion));
            if (urls.Count == 0) throw new ArgumentException("Empty url list provided", nameof(urls));

            m_wc = new WebClient();
            m_updater = Updater.Instance;
        }

        protected override void DoWork()
        {
            Result = new CheckForUpdatesResult();

            if (!NetworkUtils.HasConnection()) throw new NoInternetException("No internet available");

            if (IsCancelled) return;

            // load the updatefile from disk
            var file = DownloadFile<UpdateCatalogFile>(Urls.Replace(REPLACE_FILE_NAME, UpdateCatalogFile.FILE_NAME), $"{IOUtils.AppDataPath}\\{UpdateCatalogFile.FILE_NAME}");

            if (IsCancelled) return;

            if (file.Catalog == null || file.Catalog.Count == 0) throw new InvalidUpdateServerException();

            Result.UpdateInfo = file.GetLatestUpdateForVersion(m_version);
            Result.ApplicationName = file.ApplicationName;
            Result.DownloadURLs = file.DownloadUrls;

            if (!Result.UpdateAvailable || IsCancelled) return;

            if (Result.DownloadURLs == null || Result.DownloadURLs.Count == 0) throw new InvalidUpdateServerException();

            var meta = DownloadFile<UpdateMetadataFile>(Result.DownloadURLs.Replace(REPLACE_FILE_NAME, Result.UpdateInfo.FileName), $"{IOUtils.TempPath}\\{UpdateMetadataFile.FILE_NAME}");

            if (IsCancelled) return;

            var privilegesCheckTask = new CheckRequiredPrivilegesTask(meta).ConfigureAwait(false).Start();
            Result.AdminRightsNeeded = privilegesCheckTask.AwaitTask().Result;
        }

        private T DownloadFile<T>(IEnumerable<string> urlsToUse, string localFile) where T : FileBase<T>, new()
        {
            var enumerator = urlsToUse.GetEnumerator();

            m_updater.Logger.Info(nameof(CheckForUpdatesTask), nameof(DownloadFile), $"Getting {typeof(T).GetDescription()}");

            do
            {
                if (!enumerator.MoveNext() || string.IsNullOrEmpty(enumerator.Current))
                    throw new UnableToDownloadUpdateException();
            } while (!DownloadLocalFile(enumerator.Current, localFile));

            return FileManager.LoadFile<T>(localFile);
        }

        private bool DownloadLocalFile(string url, string localFile)
        {
            m_updater.Logger.Debug(nameof(CheckForUpdatesTask), nameof(DownloadLocalFile), $"Attempting to download file from {url}");

            try
            {
                m_wc.DownloadFile(url, localFile);

                m_updater.Logger.Info(nameof(CheckForUpdatesTask), nameof(DownloadLocalFile), $"Succesfully downloaded file from {url}");
            }
            catch (Exception e)
            {
                m_updater.Logger.Error(nameof(CheckForUpdatesTask), nameof(DoWork), e);
                return false;
            }

            return true;
        }

        public class CheckForUpdatesResult
        {
            public UpdateVersion Version { get { return UpdateInfo.Version; } }
            public bool UpdateAvailable => UpdateInfo != null;
            public UpdateInfo UpdateInfo { get; set; }
            public string ApplicationName { get; set; }
            public IList<string> DownloadURLs { get; set; }
            public bool AdminRightsNeeded { get; set; }
        }
    }
}
