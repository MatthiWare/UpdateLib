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

using System;
using MatthiWare.UpdateLib;
using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Utils;
using NUnit.Framework;

namespace UpdateLib.Tests.Util
{
    [TestFixture]
    public class IOUtilsTest
    {

        Updater instance;

        [SetUp]
        public void Setup()
        {
            //instance = Updater.Instance;
        }

        [Test]
        public void TestAppDataPathLocal()
        {
            //instance.ConfigureInstallationMode(InstallationMode.Local);

            //string path = IOUtils.AppDataPath;

            //Assert.IsTrue(path.Contains("Local"), $"Path: '{path}' didn't contain 'Local'");
        }

        [Test]
        public void TestAppDataPathRoaming()
        {
            //instance.ConfigureInstallationMode(InstallationMode.Shared);

            //string path = IOUtils.AppDataPath;

            //Assert.IsTrue(path.Contains("Roaming"), $"Path: '{path}' didn't contain 'Roaming'");
        }

        [Test]
        public void RemoteBasePathShouldThrowArgumentNullException()
        {
            Assert.Catch<ArgumentNullException>(() => IOUtils.GetRemoteBasePath(string.Empty));
            Assert.Catch<ArgumentNullException>(() => IOUtils.GetRemoteBasePath(null));
        }

        [Test]
        public void TestRemotebasePath()
        {
            string input = "http://matthiware.dev/prd/test";

            string output = IOUtils.GetRemoteBasePath(input);

            Assert.AreEqual("http://matthiware.dev/prd/", output);
        }

        [TearDown]
        public void CleanUp()
        {
            instance = null;
        }

    }
}
