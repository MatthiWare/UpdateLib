using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MatthiWare.UpdateLib.Tasks
{
    public class CleanUpTask
    {

        private Queue<WaitHandle> whQueue;
        private readonly object sync = new object();
        private Queue<FileInfo> cleanupQueue;

        public string PathToClean { get; set; }
        public string SearchPattern { get; set; }
        public bool IncludeSubDirectories { get; set; }

        public CleanUpTask(string pathToCleanUp, string searchPattern = "*.old.tmp", bool includeSubDirs = true)
        {
            cleanupQueue = new Queue<FileInfo>();
            whQueue = new Queue<WaitHandle>();

            PathToClean = pathToCleanUp;
            SearchPattern = searchPattern;
            IncludeSubDirectories = includeSubDirs;
        }

        public void Start()
        {
            DirectoryInfo dir = new DirectoryInfo(PathToClean);
            Console.WriteLine(dir.FullName);

            dir.GetFiles(SearchPattern, IncludeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            Action caller = new Action(Worker);
            caller.BeginInvoke(new AsyncCallback(r => caller.EndInvoke(r)), null);
        }

        private void Worker()
        {

        }

        public void AwaitTask()
        {
            while (whQueue.Count > 0)
            {
                WaitHandle wh = null;
                lock (sync)
                    wh = whQueue.Dequeue();

                wh.WaitOne();
                wh.Close();
            }
        }
    }
}
