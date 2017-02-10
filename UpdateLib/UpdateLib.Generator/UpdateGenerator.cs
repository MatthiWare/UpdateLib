using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UpdateLib.Generator
{
    public class UpdateGenerator
    {
        private UpdateFile updateFile;

        public UpdateGenerator()
        {
            updateFile = new UpdateFile();
        }

        public void AddDirectory(DirectoryInfo dir)
        {
            if (dir == null)
                throw new ArgumentNullException("dir", "The directory cannot be null");

            if (!dir.Exists)
                throw new DirectoryNotFoundException(string.Format("The directory '{0}' does not exist.", dir.FullName));

            AddDirRecursive(dir, updateFile.ApplicationDirectory);
        }

        private void AddDirRecursive(DirectoryInfo dir, DirectoryEntry entry)
        {
            foreach (FileInfo fi in dir.GetFiles())
            {
                Console.WriteLine("Adding: {0}", fi.FullName);

                FileEntry newEntry = new FileEntry(fi.Name);
                newEntry.Hash = HashUtil.GetHash(fi.FullName);

                entry.Files.Add(newEntry);
            }

            foreach(DirectoryInfo newDir in dir.GetDirectories())
            {
                DirectoryEntry newEntry = new DirectoryEntry(newDir.Name);
                entry.Directories.Add(newEntry);
                AddDirRecursive(newDir, newEntry);
            }
        }


        public UpdateFile Build()
        {
            return updateFile;
        }

    }
}
