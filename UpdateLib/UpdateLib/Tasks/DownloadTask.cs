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

using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Security;
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

            if (fi.Exists)
                fi.MoveTo($"{localFile}.old.tmp");

            if (!fi.Directory.Exists)
                fi.Directory.Create();

            Uri uri = new Uri(remoteFile);
            webClient.DownloadFileAsync(uri, localFile);

            wait.WaitOne();
            wait.Close();
            wait = null;

            string hash = HashUtil.GetHash(localFile);

            if (hash.Length != Entry.Hash.Length || hash != Entry.Hash)
                throw new InvalidHashException($"Calculated hash doesn't match provided hash for file: {localFile}");

            Updater.Instance.GetCache().AddOrUpdateEntry(localFile, hash);

            OnTaskProgressChanged(100);
        }

        public override void Rollback()
        {
            OnTaskProgressChanged(0);

            webClient.CancelAsync();

            string localFile = Updater.Instance.Converter.Convert(Entry.DestinationLocation);
            string localTempFile = $"{localFile}.old.tmp";

            if (!File.Exists(localTempFile))
            {
                OnTaskProgressChanged(100);
                return;
            }

            if (File.Exists(localFile))
                File.Delete(localFile);

            File.Move(localTempFile, localFile);

            OnTaskProgressChanged(50);

            Updater.Instance.GetCache().AddOrUpdateEntry(localFile);

            Updater.Instance.Logger.Warn(nameof(DownloadTask), nameof(Cancel), $"Rolled back -> {Entry.Name}");

            OnTaskProgressChanged(100);
        }
    }
}
