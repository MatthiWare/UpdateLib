using MatthiWare.UpdateLib.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Tasks
{
    public class DownloadTask : IDisposable
    {
        public event EventHandler<AsyncCompletedEventArgs> TaskCompleted;
        public event EventHandler<DownloadProgressChangedEventArgs> TaskProgressChanged; 

        private WebClient webClient;
        public ListViewItem Item { get; private set; }
        public FileEntry Entry { get; private set; }

        private ManualResetEvent wait;

        public DownloadTask(ListViewItem item)
        {
            Item = item;
            Entry = (FileEntry)Item.Tag;

            webClient = new WebClient();
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
        }

        public void Start()
        {
            wait = new ManualResetEvent(false);

            string localFile = Updater.Instance.Converter.Replace(Entry.DestinationLocation);
            string remoteFile = string.Concat(Updater.Instance.RemoteBasePath, Entry.SourceLocation);

            Console.WriteLine("[INFO]: DownloadTask: LocalFile => {0}", localFile);
            Console.WriteLine("[INFO]: DownloadTask: RemoteFile => {0}", remoteFile);

            if (File.Exists(localFile))
                File.Move(localFile, string.Concat(localFile, ".old.tmp"));

            webClient.DownloadFileAsync(new Uri(remoteFile), localFile);
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            wait.Set();
            TaskCompleted?.Invoke(this, e);
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            TaskProgressChanged?.Invoke(this, e);
        }

        public void AwaitTask()
        {
            if (wait == null)
                throw new InvalidOperationException("The task needs to be started first");

            wait.WaitOne();
            wait.Close();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    webClient.Dispose();

                    if (wait != null)
                        wait.Close();
                }

                disposedValue = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}
