using MatthiWare.UpdateLib.Files;
using System;

namespace MatthiWare.UpdateLib.Tasks
{
    public class UpdateFileProcessorTask : AsyncTask
    {
        private UpdateFile file;

        public UpdateFileProcessorTask(UpdateFile file)
        {
            this.file = file;
        }


        private void PostProcessDirectory(DirectoryEntry dir)
        {
            foreach (EntryBase file in dir.Items)
                file.Parent = dir;

            int left = dir.Directories.Count;
            foreach (DirectoryEntry subDir in dir.Directories)
            {
                subDir.Parent = dir;

                if (--left == 0)
                    PostProcessDirectory(subDir);
                else
                    Enqueue(new Action<DirectoryEntry>(PostProcessDirectory), subDir);
            }
        }

        protected override void DoWork()
        {
            foreach (DirectoryEntry dir in file.Folders)
                PostProcessDirectory(dir);

            foreach (DirectoryEntry dir in file.Registry)
                PostProcessDirectory(dir);

            AwaitWorkers();
        }
    }
}
