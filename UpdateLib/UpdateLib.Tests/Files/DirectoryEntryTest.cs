using MatthiWare.UpdateLib.Files;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateLib.Tests.Files
{
    [TestFixture]
    public class DirectoryEntryTest
    {

        [Test, Parallelizable]
        public void CheckIfCountReturnsCorrectValue()
        {
            DirectoryEntry entry = MakeDirWithSubDirs(2, 3, 5);
            Assert.AreEqual(2 * 3 * 5, entry.Count);
            Assert.AreEqual(2 * 3 * 5, entry.GetItems().Count());

            entry = MakeDirWithSubDirs(5, 5, 1);
            Assert.AreEqual(5 * 5 * 1, entry.Count);
            Assert.AreEqual(5 * 5 * 1, entry.GetItems().Count());
        }

        private DirectoryEntry MakeDirWithSubDirs(int subDirs, int depth, int childsPerSubDir)
        {
            DirectoryEntry root = new DirectoryEntry("test");

            DirectoryEntry dir = root;
            for (int d = 0; d < depth; d++)
            {
                for (int s = 0; s < subDirs; s++)
                {
                    DirectoryEntry entry = new DirectoryEntry($"dir_{s}");

                    for (int f = 0; f < childsPerSubDir; f++)
                        entry.Add(new FileEntry($"file_{f}.txt"));

                    dir.Add(entry);
                }

                DirectoryEntry subdir = new DirectoryEntry($"subdir_{d}");
                dir.Add(subdir);

                dir = subdir;

            }

            return root;
        }

        [Test, Parallelizable]
        public void CheckIfAddRemoveWorks()
        {
            DirectoryEntry entry = new DirectoryEntry("test");
            Assert.AreEqual(0, entry.Count);

            FileEntry f = new FileEntry();

            entry.Add(f);

            Assert.AreEqual(entry, f.Parent);
            Assert.AreEqual(1, entry.Count);

            entry.Remove(f);

            Assert.AreEqual(null, f.Parent);
            Assert.AreEqual(0, entry.Count);

            DirectoryEntry subDir = new DirectoryEntry("sub");
            Assert.AreEqual(0, entry.Directories.Count);

            entry.Add(subDir);

            Assert.AreEqual(entry, subDir.Parent);
            Assert.AreEqual(1, entry.Directories.Count);

            entry.Remove(subDir);

            Assert.AreEqual(null, subDir.Parent);
            Assert.AreEqual(0, entry.Directories.Count);
        }

        [Test, Parallelizable]
        public void AddingOrRemovingNullsShouldThrowExceptions()
        {
            DirectoryEntry entry = new DirectoryEntry("test");

            DirectoryEntry nullDir = null;
            FileEntry nullFile = null;

            Assert.Throws<ArgumentNullException>(() => entry.Add(nullDir));
            Assert.Throws<ArgumentNullException>(() => entry.Add(nullFile));
            Assert.Throws<ArgumentNullException>(() => entry.Remove(nullDir));
            Assert.Throws<ArgumentNullException>(() => entry.Remove(nullFile));
        }
    }
}
