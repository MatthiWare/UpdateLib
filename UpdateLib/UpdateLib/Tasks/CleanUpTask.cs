using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MatthiWare.UpdateLib.Tasks
{
    public class CleanUpTask : AsyncTaskBase
    {

        private Queue<WaitHandle> whQueue;
        private readonly object sync = new object();

        public string PathToClean { get; set; }
        public string SearchPattern { get; set; }
        public bool IncludeSubDirectories { get; set; }

        public CleanUpTask(string pathToCleanUp, string searchPattern = "*.old.tmp", bool includeSubDirs = true)
        {
            whQueue = new Queue<WaitHandle>();

            PathToClean = pathToCleanUp;
            SearchPattern = searchPattern;
            IncludeSubDirectories = includeSubDirs;
        }
        public override void DoWork()
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
                    Console.WriteLine("[ERROR]: Unable to delete file {0} -> {1}.", file.FullName, e.Message);
                }
            }
        }
    }
}
