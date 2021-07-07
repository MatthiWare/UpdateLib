/*  UpdateLib - .Net auto update library
 *  Copyright (C) 2016 - MatthiWare (Matthias Beerens)
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published
 *  by the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using MatthiWare.UpdateLib.Common;
using NUnit.Framework;
using System;
using System.Linq;

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
