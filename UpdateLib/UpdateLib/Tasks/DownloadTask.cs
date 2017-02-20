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
    public class DownloadTask : AsyncTask
    {
        private WebClient webClient;
        public ListViewItem Item { get; private set; }
        public FileEntry Entry { get; private set; }

        private ManualResetEvent wait;

        public DownloadTask(ListViewItem item)
        {
            Item = item;
            Entry = (FileEntry)Item.Tag;

            webClient = new WebClient();
            webClient.DownloadProgressChanged += (o, e)=> { OnTaskProgressChanged(e); };
            webClient.DownloadFileCompleted += (o, e) => { wait.Set(); OnTaskCompleted(e); };
        }

        public override void DoWork()
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
    }
}
