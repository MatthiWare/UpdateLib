/*  Copyright
 *  
 *  UpdateLib - .Net auto update library <https://github.com/MatthiWare/UpdateLib>
 *  
 *  File: DownloadManager.cs v0.5
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
using System.ComponentModel;
using System.IO;
using System.Net;

using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Common.Exceptions;
using MatthiWare.UpdateLib.Security;
using MatthiWare.UpdateLib.Utils;

namespace MatthiWare.UpdateLib.Tasks
{
    public class DownloadManager
    {
        private UpdateInfo m_updateInfo;

        private IList<string> m_urls;

        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        public event EventHandler<AsyncCompletedEventArgs> Completed;

        private int index = 0;

        private Updater updater;

        public DownloadManager(UpdateInfo updateInfo, IList<string> urls)
        {
            m_urls = urls ?? throw new ArgumentNullException(nameof(urls));
            m_updateInfo = updateInfo ?? throw new ArgumentNullException(nameof(updateInfo));

            if (m_urls.Count == 0) throw new ArgumentException("No download sources specified ", nameof(urls));

            updater = Updater.Instance;
        }

        public void Download()
        {
            var urlToUse = GetNextUrl();

            if (string.IsNullOrEmpty(urlToUse))
                throw new WebException("Unable to download patch from specified download sources");

            var local = new FileInfo($"{IOUtils.TempPath}\\{m_updateInfo.FileName}");

            if (!local.Directory.Exists) local.Directory.Create();
            if (local.Exists) local.Delete();

            var task = new DownloadTask(urlToUse, local.FullName);

            task.TaskProgressChanged += (o, e) => OnProgressChanged(e.ProgressPercentage, 110);

            task.TaskCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    updater.Logger.Error(nameof(DownloadManager), nameof(Download), e.Error);
                    updater.Logger.Info(nameof(DownloadManager), nameof(Download), "Attempting to download patch from next url..");

                    task.Url = GetNextUrl();

                    if (string.IsNullOrEmpty(task.Url))
                        OnCompleted(new WebException("Unable to download patch from specified download sources", e.Error));
                    else
                        task.Start();

                    return;
                }

                var hash = HashUtil.GetHash(local.FullName);

                if (m_updateInfo.Hash != hash)
                {
                    OnCompleted(new InvalidHashException($"Hash doesn't match was expecting '{m_updateInfo.Hash}' but got '{hash}'"));
                    return;
                }

                OnProgressChanged(100, 100);
                OnCompleted(null);
            };

            task.Start();
        }

        protected void OnProgressChanged(int done, int total)
            => ProgressChanged?.Invoke(this, new ProgressChangedEventArgs((int)((done / (double)total) * 100), null));

        protected void OnCompleted(Exception e)
            => Completed?.Invoke(this, new AsyncCompletedEventArgs(e, false, null));

        private string GetNextUrl()
            => (index < m_urls.Count) ? m_urls[index++].Replace("%file%", m_updateInfo.FileName) : string.Empty;
    }
}
