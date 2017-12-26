/*  Copyright
 *  
 *  UpdateLib - .Net auto update library <https://github.com/MatthiWare/UpdateLib>
 *  
/*  Copyright
 *  
 *  UpdateLib - .Net auto update library <https://github.com/MatthiWare/UpdateLib>
 *  
 *  File: DownloadTask.cs v0.5
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

using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Common.Exceptions;
using MatthiWare.UpdateLib.Security;
using MatthiWare.UpdateLib.Utils;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace MatthiWare.UpdateLib.Tasks
{
    public class DownloadTask : UpdatableTask
    {
        private WebClient webClient;
        private ManualResetEvent wait;

        public FileEntry Entry { get; private set; }

        public DownloadTask(FileEntry entry)
        {
            Entry = entry;
            webClient = new WebClient();
            webClient.DownloadProgressChanged += (o, e) => { OnTaskProgressChanged(e.ProgressPercentage, 110); };
            webClient.DownloadFileCompleted += (o, e) => { wait.Set(); };
        }

        protected override void DoWork()
        {
            if (IsCancelled)
                return;

            wait = new ManualResetEvent(false);

            string localFile = Updater.Instance.Converter.Convert(Entry.DestinationLocation);
            string remoteFile = string.Concat(Updater.Instance.RemoteBasePath, Entry.SourceLocation);
            
            Updater.Instance.Logger.Debug(nameof(DownloadTask), nameof(DoWork), $"LocalFile => {localFile}");
            Updater.Instance.Logger.Debug(nameof(DownloadTask), nameof(DoWork), $"RemoteFile => {remoteFile}");

            FileInfo fi = new FileInfo(localFile);
            string cacheFile = $"{IOUtils.CachePath}\\{fi.Name}";

            if (File.Exists(cacheFile))
                File.Delete(cacheFile);

            if (fi.Exists)
                fi.MoveTo(cacheFile);

            if (!fi.Directory.Exists)
                fi.Directory.Create();

            var uri = new Uri(remoteFile);
            webClient.DownloadFileAsync(uri, localFile);

            wait.WaitOne();
            wait.Close();
            wait = null;

            var hash = HashUtil.GetHash(localFile);

            if (hash.Length != Entry.Hash.Length || hash != Entry.Hash)
                throw new InvalidHashException($"Calculated hash doesn't match provided hash for file: {localFile}");

            Updater.Instance.GetCache().AddOrUpdateEntry(localFile, hash);

            OnTaskProgressChanged(100);
        }

        public override void Rollback()
        {
            OnTaskProgressChanged(0);

            webClient.CancelAsync();

            FileInfo localFile = new FileInfo(Updater.Instance.Converter.Convert(Entry.DestinationLocation));
            FileInfo cacheFile = new FileInfo($"{IOUtils.CachePath}\\{localFile.Name}");

            if (!cacheFile.Exists)
            {
                OnTaskProgressChanged(100);
                return;
            }

            if (localFile.Exists)
                localFile.Delete();

            cacheFile.MoveTo(localFile.FullName);

            OnTaskProgressChanged(50);

            Updater.Instance.GetCache().AddOrUpdateEntry(localFile.FullName);

            Updater.Instance.Logger.Warn(nameof(DownloadTask), nameof(Cancel), $"Rolled back -> {Entry.Name}");

            OnTaskProgressChanged(100);
        }
    }
}
