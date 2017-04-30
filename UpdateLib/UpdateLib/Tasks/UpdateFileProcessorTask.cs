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
            Action<DirectoryEntry> call = new Action<DirectoryEntry>(PostProcessDirectory);

            foreach (FileEntry file in dir.Files)
                file.Parent = dir;

            foreach (DirectoryEntry subDir in dir.Directories)
            {
                subDir.Parent = dir;
                Enqueue(call, subDir);
            }
        }

        protected override void DoWork()
        {
            foreach (DirectoryEntry dir in file.Folders)
                PostProcessDirectory(dir);

            AwaitWorkers();
        }
    }
}
