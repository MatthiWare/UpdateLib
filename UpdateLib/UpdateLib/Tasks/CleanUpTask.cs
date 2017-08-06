using MatthiWare.UpdateLib.Logging;
using System;
using System.IO;

namespace MatthiWare.UpdateLib.Tasks
{
    public class CleanUpTask : AsyncTask
    {
        public string PathToClean { get; set; }
        public string SearchPattern { get; set; }
        public bool IncludeSubDirectories { get; set; }

        public CleanUpTask(string pathToCleanUp, string searchPattern = "*.old.tmp", bool includeSubDirs = true)
        {
            PathToClean = Updater.Instance.Converter.Convert(pathToCleanUp);
            SearchPattern = searchPattern;
            IncludeSubDirectories = includeSubDirs;
        }
        protected override void DoWork()
        {
            DirectoryInfo dir = new DirectoryInfo(PathToClean);
            FileInfo[] files = dir.GetFiles(SearchPattern, IncludeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            foreach (FileInfo file in files)
            {
                try
                {
                    file.Delete();
                }
                catch (Exception e)
                {
                    Updater.Instance.Logger.Error(nameof(CleanUpTask), nameof(DoWork), e);
                }
            }
        }
    }
}
