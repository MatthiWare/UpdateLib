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

using MatthiWare.UpdateLib.Tasks;
using NUnit.Framework;
using System;
using System.IO;

namespace UpdateLib.Tests.Tasks
{
    [TestFixture]
    public class CleanUpTaskTest
    {

        private string m_folder, m_file;

        [SetUp]
        public void Before()
        {
            m_folder = $"{Path.GetTempPath()}test_{Guid.NewGuid().ToString()}";
            m_file = $"{m_folder}\\test.old.tmp";

            if (!Directory.Exists(m_folder))
                Directory.CreateDirectory(m_folder);

            if (!File.Exists(m_file))
                File.Open(m_file, FileMode.OpenOrCreate).Dispose();
        }

        [Test]
        public void TestCleanUp()
        {
            Assert.IsTrue(File.Exists(m_file));

            CleanUpTask task = new CleanUpTask(m_folder);
            task.ConfigureAwait(false);
            task.Start().AwaitTask();

            Assert.IsFalse(File.Exists(m_file));
        }

        [TearDown]
        public void CleanUp()
        {
            if (Directory.Exists(m_folder))
                Directory.Delete(m_folder);
        }

    }
}
