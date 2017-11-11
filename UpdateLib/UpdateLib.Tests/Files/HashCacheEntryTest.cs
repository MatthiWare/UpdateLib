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
using MatthiWare.UpdateLib.Files;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace UpdateLib.Tests.Files
{
    [TestFixture]
    public class HashCacheEntryTest
    {
        private string temp_file;
        [SetUp]
        public void Before()
        {
            temp_file = Path.GetTempPath() + Guid.NewGuid().ToString() + "hash_util_test.tmp";
            using (StreamWriter sw = new StreamWriter(File.Open(temp_file, FileMode.OpenOrCreate, FileAccess.Write), Encoding.Default))
            {
                sw.WriteLine("This is a test");
            }
        }

        [Test]
        public void NullEntryShouldThrowArgumentNullException()
        {
            Assert.Catch<ArgumentNullException>(() => new HashCacheEntry(null));
        }

        [Test]
        public void NotExistingFileShouldThrowFileNotFoundException()
        {
            Assert.Catch<FileNotFoundException>(() => new HashCacheEntry("notexistingfile.dll"));
        }

        [Test]
        public void TestHashing()
        {
            string hash = "07849dc26fcbb2f3bd5f57bdf214bae374575f1bd4e6816482324799417cb379".ToUpper();
            HashCacheEntry entry = new HashCacheEntry(temp_file);
            Assert.AreEqual(hash, entry.Hash);
            long ticks = entry.Ticks;

            EditTempFile();
            
            entry.Recalculate();

            Assert.AreNotEqual(ticks, entry.Ticks);
            Assert.AreNotEqual(hash, entry.Hash);
        }

        private void EditTempFile()
        {
            using (StreamWriter sw = new StreamWriter(File.Open(temp_file, FileMode.Open, FileAccess.Write), Encoding.Default))
            {
                sw.WriteLine("edited file");
            }
        }


        [TearDown]
        public void CleanUp()
        {
            if (File.Exists(temp_file))
                File.Delete(temp_file);
        }
    }
}
