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

using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Common.Abstraction;
using MatthiWare.UpdateLib.Files;

using Moq;

using NUnit.Framework;

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
            var file = MakeUpdateFile();
            file.Save(temp_file);

            var updateFile = FileManager.LoadFile<UpdateMetadataFile>(temp_file);

            //Assert.AreEqual(file.ApplicationName, updateFile.ApplicationName);
            Assert.AreEqual(file.Version, updateFile.Version);
            Assert.AreEqual(file.FileCount, updateFile.FileCount);
        }

        [Test]
        public void SaveInvalidParamterShouldThrowExceptions()
        {
            Stream nullStream = null;

            var file = new UpdateMetadataFile();

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

            Assert.Catch<ArgumentNullException>(() => { FileManager.LoadFile<UpdateMetadataFile>(nullStream); });
            Assert.Catch<ArgumentNullException>(() => { FileManager.LoadFile<UpdateMetadataFile>(string.Empty); });

            CleanUp();

            Assert.Catch<FileNotFoundException>(() => { FileManager.LoadFile<UpdateMetadataFile>(temp_file); });

            Mock<Stream> unreadableStream = new Mock<Stream>();
            unreadableStream.SetupGet(s => s.CanRead).Returns(false);

            Assert.Catch<ArgumentException>(() => { FileManager.LoadFile<UpdateMetadataFile>(unreadableStream.Object); });
        }

        private UpdateMetadataFile MakeUpdateFile()
        {
            var file = new UpdateMetadataFile();

            var info = new UpdateMetadataFile { Version = "9.9.9" };

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

            info.Folders.Add(appSubFolder);
            info.Folders.Add(otherSubFolder);

            DirectoryEntry regDir = new DirectoryEntry("HKEY_LOCAL_MACHINE");

            EntryBase regEntry = new RegistryKeyEntry("test", Microsoft.Win32.RegistryValueKind.String, null);

            regDir.Add(regEntry);

            info.Registry.Add(regDir);

            Assert.AreEqual(2, info.FileCount);
            Assert.AreEqual(1, info.RegistryKeyCount);

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
