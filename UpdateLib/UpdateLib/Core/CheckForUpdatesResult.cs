using MatthiWare.UpdateLib.Common;

namespace MatthiWare.UpdateLib.Core
{
    public class CheckForUpdatesResult
    {
        public UpdateVersion Version { get { return UpdateInfo.Version; } }
        public bool UpdateAvailable => UpdateInfo != null;
        public UpdateInfo UpdateInfo { get; internal set; }
        public bool AdminRightsNeeded { get; internal set; }
    }
}