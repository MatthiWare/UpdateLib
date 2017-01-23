using MatthiWare.UpdateLib.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Tasks
{
    public class DownloadTask : IDisposable
    {
        public event EventHandler<TaskEventArgs> TaskProgressChanged;
        public event EventHandler<TaskEventArgs> TaskCompleted; 

        private WebClient webClient;
        private ListViewItem file;

        public DownloadTask(ListViewItem item)
        {
            this.file = item;
            webClient = new WebClient();
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            TaskProgressChanged?.Invoke(this, new TaskEventArgs(e.ProgressPercentage));
        }

        public void Execute()
        {
            webClient.DownloadFile("placeholder", file.Name);
            TaskCompleted?.Invoke(this, new TaskEventArgs(100));

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
