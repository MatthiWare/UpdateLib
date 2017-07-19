using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Logging;
using MatthiWare.UpdateLib.Security;
using System;
using System.IO;
using System.Net;
using System.Security;
using System.Threading;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Tasks
{
    public class DownloadTask : UpdatableTask
    {
        private WebClient webClient;
        private ManualResetEvent wait;

        public FileEntry Entry { get; private set; }

        public DownloadTask(ListViewItem item, FileEntry entry)
            : this(entry)
        {
            Item = item;
        }

        public DownloadTask(FileEntry entry)
        {
            Entry = entry;
            webClient = new WebClient();
            webClient.DownloadProgressChanged += (o, e) => { OnTaskProgressChanged(e); };
            webClient.DownloadFileCompleted += (o, e) => { wait.Set(); };
        }

        protected override void DoWork()
        {
            if (IsCancelled)
                return;

            wait = new ManualResetEvent(false);

            string localFile = Updater.Instance.Converter.Replace(Entry.DestinationLocation);
            string remoteFile = string.Concat(Updater.Instance.RemoteBasePath, Entry.SourceLocation);

            Updater.Instance.Logger.Debug(nameof(DownloadTask), nameof(DoWork), $"LocalFile => {localFile}");
            Updater.Instance.Logger.Debug(nameof(DownloadTask), nameof(DoWork), $"RemoteFile => {remoteFile}");

            if (File.Exists(localFile))
                File.Move(localFile, $"{localFile}.old.tmp");

            webClient.DownloadFileAsync(new Uri(remoteFile), localFile);

            wait.WaitOne();
            wait.Close();
            wait = null;

            string hash = HashUtil.GetHash(localFile);

            if (hash.Length != Entry.Hash.Length || hash != Entry.Hash)
                throw new InvalidHashException($"Calculated hash doesn't match provided hash for file: {localFile}");

            Updater.Instance.GetCache().AddOrUpdateEntry(localFile, hash);
        }

        public override void Rollback()
        {
            OnTaskProgressChanged(0);

            webClient.CancelAsync();

            string localFile = Updater.Instance.Converter.Replace(Entry.DestinationLocation);
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
