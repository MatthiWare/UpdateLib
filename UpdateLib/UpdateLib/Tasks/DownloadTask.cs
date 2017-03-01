﻿using MatthiWare.UpdateLib.Files;
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
    public class DownloadTask : AsyncTaskBase
    {
        private WebClient webClient;
        private ManualResetEvent wait;

        public ListViewItem Item { get; private set; }
        public FileEntry Entry { get; private set; }
        
        public DownloadTask(ListViewItem item)
        {
            Item = item;
            Entry = (FileEntry)Item.Tag;

            webClient = new WebClient();
            webClient.DownloadProgressChanged += (o, e)=> { OnTaskProgressChanged(e); };
            webClient.DownloadFileCompleted += (o, e) => { wait.Set(); };
        }

        protected override void DoWork()
        {
            wait = new ManualResetEvent(false);

            string localFile = Updater.Instance.Converter.Replace(Entry.DestinationLocation);
            string remoteFile = string.Concat(Updater.Instance.RemoteBasePath, Entry.SourceLocation);

            Console.WriteLine($"[{GetType().Name}]: LocalFile => {localFile}");
            Console.WriteLine($"[{GetType().Name}]: RemoteFile => {remoteFile}");

            if (File.Exists(localFile))
                File.Move(localFile, string.Concat(localFile, ".old.tmp"));

            webClient.DownloadFileAsync(new Uri(remoteFile), localFile);

            wait.WaitOne();
            wait.Close();
            wait = null;
        }
    }
}
