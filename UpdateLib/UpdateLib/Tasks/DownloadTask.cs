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

using System;
using System.IO;
using System.Net;
using System.Threading;

namespace MatthiWare.UpdateLib.Tasks
{
    public class DownloadTask : AsyncTask<object, DownloadTask>
    {
        private WebClient webClient;
        private ManualResetEvent wait;
        private Updater m_updater;

        public string Url { get; set; }
        public string Local { get; set; }

        public DownloadTask(string url, string local)
        {
            if (string.IsNullOrEmpty(local)) throw new ArgumentNullException(nameof(local));
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));

            Url = url;
            Local = local;

            webClient = new WebClient();
            webClient.DownloadProgressChanged += (o, e) => { OnTaskProgressChanged(e.ProgressPercentage); };
            webClient.DownloadFileCompleted += (o, e) => { wait.Set(); };

            m_updater = Updater.Instance;
        }

        protected override void DoWork()
        {
            if (IsCancelled)
                return;

            wait = new ManualResetEvent(false);

            m_updater.Logger.Debug(nameof(DownloadTask), nameof(DoWork), $"LocalFile => {Local}");
            m_updater.Logger.Debug(nameof(DownloadTask), nameof(DoWork), $"RemoteFile => {Url}");

            var fi = new FileInfo(Local);

            if (!fi.Directory.Exists)
                fi.Directory.Create();

            var uri = new Uri(Url);
            webClient.DownloadFileAsync(uri, Local);

            wait.WaitOne();
            wait.Close();
            wait = null;

            OnTaskProgressChanged(100);
        }
    }
}
