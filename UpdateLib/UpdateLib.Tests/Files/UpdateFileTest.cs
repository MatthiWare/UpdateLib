using MatthiWare.UpdateLib.Files;
using Moq;
using NUnit.Framework;
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
using System.IO;

namespace UpdateLib.Tests.Files
{
    [TestFixture]
    public class UpdateFileTest
    {
        private string temp_file;
        [SetUp]
        public void Before()
        {
            temp_file = Path.GetTempPath() + Guid.NewGuid().ToString() + "updatefile.xml";
        }

        [Test]
        public void SaveAndLoadUpdateFileShouldBeTheSame()
        {
            UpdateFile file = MakeUpdateFile();
            file.Save(temp_file);

            UpdateFile loadedFile = UpdateFile.Load(temp_file);

            Assert.AreEqual(file.ApplicationName, loadedFile.ApplicationName);
            Assert.AreEqual(file.VersionString, loadedFile.VersionString);
            Assert.AreEqual(file.FileCount, loadedFile.FileCount);
        }

        [Test]
        public void SaveInvalidParamterShouldThrowExceptions()
        {
            Stream nullStream = null;

            UpdateFile file = new UpdateFile();

            Assert.Catch<ArgumentNullException>(() => { file.Save(nullStream); });
            Assert.Catch<ArgumentNullException>(() => { file.Save(string.Empty); });

            Mock<Stream> unwritableStream = new Mock<Stream>();
            unwritableStream.SetupGet(s => s.CanWrite).Returns(false);

            Assert.Catch<ArgumentException>(() => { file.Save(unwritableStream.Object); });
        }

        [Test]
        public void LoadInvalidParameterShouldThrowExceptions()
        {
            Stream nullStream = null;

            Assert.Catch<ArgumentNullException>(() => { UpdateFile.Load(nullStream); });
            Assert.Catch<ArgumentNullException>(() => { UpdateFile.Load(string.Empty); });

            CleanUp();

            Assert.Catch<FileNotFoundException>(() => { UpdateFile.Load(temp_file); });

            Mock<Stream> unreadableStream = new Mock<Stream>();
            unreadableStream.SetupGet(s => s.CanRead).Returns(false);

            Assert.Catch<ArgumentException>(() => { UpdateFile.Load(unreadableStream.Object); });
        }

        private UpdateFile MakeUpdateFile()
        {
            UpdateFile file = new UpdateFile();

            file.ApplicationName = nameof(UpdateFileTest);
            file.VersionString = "9.9.9.9";

            DirectoryEntry appSubFolder = new DirectoryEntry("AppSubFolder");
            DirectoryEntry otherSubFolder = new DirectoryEntry("OtherSubFolder");

            EntryBase appFile = new FileEntry()
            {
                Name = "application.exe",
                Description = "my desc",
                Hash = "AAA"
            };

            EntryBase otherFile = new FileEntry()
            {
                Name = "data.xml",
                Description = "my data file",
                Hash = "BBB"
            };

            appSubFolder.Add(appFile);
            otherSubFolder.Add(otherFile);

            file.Folders.Add(appSubFolder);
            file.Folders.Add(otherSubFolder);

            DirectoryEntry regDir = new DirectoryEntry("HKEY_LOCAL_MACHINE");

            EntryBase regEntry = new RegistryKeyEntry("test", Microsoft.Win32.RegistryValueKind.String, null);

            regDir.Add(regEntry);

            file.Registry.Add(regDir);

            Assert.AreEqual(2, file.FileCount);
            Assert.AreEqual(1, file.RegistryKeyCount);

            return file;
        }

        [TearDown]
        public void CleanUp()
        {
            if (File.Exists(temp_file))
                File.Delete(temp_file);
        }

    }
}
