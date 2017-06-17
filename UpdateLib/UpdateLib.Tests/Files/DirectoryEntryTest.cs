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

        [Test]
        public void CheckIfCountReturnsCorrectValue()
        {
            DirectoryEntry entry = MakeDirWithSubDirs(2, 3, 5);
            Assert.AreEqual(2 * 3 * 5, entry.Count);

            entry = MakeDirWithSubDirs(5, 5, 1);
            Assert.AreEqual(5 * 5 * 1, entry.Count);
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
                    {
                        entry.Files.Add(new FileEntry($"file_{f}.txt"));
                    }

                    dir.Directories.Add(entry);
                }

                DirectoryEntry subdir = new DirectoryEntry($"subdir_{d}");
                dir.Directories.Add(subdir);

                dir = subdir;

            }

            return root;
        }
    }
}
