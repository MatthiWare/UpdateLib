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
using System.Linq;
using System.Text;

namespace UpdateLib.Tests.Files
{
    [TestFixture]
    public class HashCacheFileTest
    {

        private string m_path, m_tempFile;

        [SetUp]
        public void Before()
        {
            var path = Path.GetTempPath();

            m_path = $@"{path}\\Cache_{Guid.NewGuid().ToString()}\{HashCacheFile.FILE_NAME}";

            m_tempFile = Path.GetTempPath() + Guid.NewGuid().ToString() + "hash_cache_file_test.tmp";
            using (StreamWriter sw = new StreamWriter(File.Open(m_tempFile, FileMode.OpenOrCreate, FileAccess.Write), Encoding.Default))
            {
                sw.WriteLine("This is a test");
            }
        }

        [Test]
        public void TestLoadAndSaving()
        {
            HashCacheFile file = new HashCacheFile();

            file.Items.Add(new HashCacheEntry(m_tempFile));

            file.Save(m_path);

            HashCacheFile loadedFile = HashCacheFile.Load(m_path);

            CheckEntries(file.Items.FirstOrDefault(), loadedFile.Items.FirstOrDefault());
        }

        [Test]
        public void TestAddOrUpdateEntry()
        {
            HashCacheFile file = new HashCacheFile();

            file.AddOrUpdateEntry(m_tempFile);

            var hash = new HashCacheEntry(m_tempFile);

            CheckEntries(file.Items.FirstOrDefault(), hash);

            EditFile();

            file.AddOrUpdateEntry(m_tempFile);

            Assert.AreEqual(file.Items.FirstOrDefault().FilePath.Trim(), hash.FilePath.Trim());
            Assert.AreNotEqual(file.Items.FirstOrDefault().Hash.Trim(), hash.Hash.Trim());
            Assert.AreNotEqual(file.Items.FirstOrDefault().Ticks, hash.Ticks);
        }


        private void CheckEntries(HashCacheEntry expected, HashCacheEntry actual)
        {
            Assert.AreEqual(expected.FilePath.Trim(), actual.FilePath.Trim());
            Assert.AreEqual(expected.Hash.Trim(), actual.Hash.Trim());
            Assert.AreEqual(expected.Ticks, actual.Ticks);
        }

        private void EditFile()
        {
            using (StreamWriter sw = new StreamWriter(File.Open(m_tempFile, FileMode.OpenOrCreate, FileAccess.Write), Encoding.Default))
            {
                sw.WriteLine("edited");

                sw.Flush();
            }
        }

        [TearDown]
        public void CleanUp()
        {
            if (File.Exists(m_path))
                File.Delete(m_path);

            if (File.Exists(m_tempFile))
                File.Delete(m_tempFile);
        }

    }
}
