using System;
using System.ComponentModel;

namespace MatthiWare.UpdateLib.Tasks
{
    public class CheckForUpdatesCompletedEventArgs : AsyncCompletedEventArgs
    {
        public string LatestVersion { get; set; }
        public bool UpdateAvailable { get; set; }

        public CheckForUpdatesCompletedEventArgs(string version, bool update, Exception error, bool cancelled, object userState) 
            : base(error, cancelled, userState)
        {
            LatestVersion = version;
            UpdateAvailable = update;
        }

        public CheckForUpdatesCompletedEventArgs(string version, bool update, AsyncCompletedEventArgs e)
              : this(version, update, e.Error, e.Cancelled, e.UserState)
        {

        }
    }
}
