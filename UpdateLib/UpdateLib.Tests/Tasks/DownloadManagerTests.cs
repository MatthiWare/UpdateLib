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

using MatthiWare.UpdateLib;
using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Security;
using MatthiWare.UpdateLib.Tasks;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;

namespace UpdateLib.Tests.Tasks
{
    [TestFixture]
    public class DownloadManagerTests
    {

        private string m_path, m_file;

        [SetUp]
        public void Before()
        {
            m_path = string.Concat(Path.GetTempPath(), Guid.NewGuid().ToString(), "\\updater");

            m_file = string.Concat(m_path, "\\testfile.txt").Replace("//", "\\");

            if (!Directory.Exists(m_path))
                Directory.CreateDirectory(m_path);

            using (StreamWriter sw = new StreamWriter(File.Open(m_file, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                sw.Write("test");
            }

            Updater.Instance.ConfigurePathConverter(c => c["appdir"] = $@"{Path.GetTempPath()}dl_test");
            Updater.Instance.ConfigureUpdateUrl(m_path + "\\update.xml");

            Updater.Instance.Initialize();
        }

        [Test]
        public void TestDownloadManager()
        {


            UpdateFile file = new UpdateFile();

            DirectoryEntry dir = new DirectoryEntry("%appdir%");
            FileEntry entry_file = new FileEntry("testfile.txt");
            entry_file.Hash = HashUtil.GetHash(m_file);
            dir.Add(entry_file);

            file.Folders.Add(dir);

            ManualResetEvent wait = new ManualResetEvent(false);
            DownloadManager manager = new DownloadManager(file);
            manager.Completed += (o, e) => wait.Set();
            manager.Update();

            Assert.IsTrue(wait.WaitOne(TimeSpan.FromSeconds(20)), "The async download timed-out after 10 seconds");

            string localFile = Updater.Instance.Converter.Convert("%appdir%/testfile.txt");
            Assert.IsTrue(File.Exists(localFile), "File didn't exist");

            using (StreamReader sr = new StreamReader(File.Open(localFile, FileMode.Open, FileAccess.Read)))
            {
                var data = sr.ReadToEnd();

                Assert.AreEqual("test", data);
            }

        }


    }
}
