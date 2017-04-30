using MatthiWare.UpdateLib.Files;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Assert.AreEqual(file.Count, loadedFile.Count);
        }

        [Test]
        public void SaveInvalidParamterShouldThrowExceptions()
        {
            Stream nullStream = null;

            UpdateFile file = new UpdateFile();

            Assert.Catch<ArgumentNullException>(() => { file.Save(nullStream); });
            Assert.Catch<ArgumentNullException>(() => { file.Save(""); });

            Mock<Stream> unwritableStream = new Mock<Stream>();
            unwritableStream.SetupGet(s => s.CanWrite).Returns(false);

            Assert.Catch<ArgumentException>(() => { file.Save(unwritableStream.Object); });
        }

        [Test]
        public void LoadInvalidParameterShouldThrowExceptions()
        {
            Stream nullStream = null;

            Assert.Catch<ArgumentNullException>(() => { UpdateFile.Load(nullStream); });
            Assert.Catch<ArgumentNullException>(() => { UpdateFile.Load(""); });

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

            FileEntry appFile = new FileEntry()
            {
                Name = "application.exe",
                Description = "my desc",
                Hash = "AAA",
                Parent = appSubFolder
            };

            FileEntry otherFile = new FileEntry()
            {
                Name = "data.xml",
                Description = "my data file",
                Hash = "BBB",
                Parent = otherSubFolder
            };

            appSubFolder.Files.Add(appFile);
            otherSubFolder.Files.Add(otherFile);

            file.Folders.Add(appSubFolder);
            file.Folders.Add(otherSubFolder);

            Assert.AreEqual(2, file.Count);

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
