using System;
using System.ComponentModel;

namespace MatthiWare.UpdateLib.Tasks
{
    public class CheckForUpdatesCompletedEventArgs : AsyncCompletedEventArgs
    {
        public string LatestVersion { get; set; }
        public bool UpdateAvailable { get; set; }

        public CheckForUpdatesCompletedEventArgs(CheckForUpdatesTask.CheckForUpdatesResult result, Exception error, bool cancelled, object userState) 
            : base(error, cancelled, userState)
        {
            LatestVersion = result.Version;
            UpdateAvailable = result.UpdateAvailable;
        }

        public CheckForUpdatesCompletedEventArgs(CheckForUpdatesTask.CheckForUpdatesResult result, AsyncCompletedEventArgs e)
              : this(result, e.Error, e.Cancelled, e.UserState)
        {

        }
    }
}
