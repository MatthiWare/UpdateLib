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
   public class FileEntryTest
    {
        [Test]
        public void ShouldGiveCorrectSourceAndDestination()
        {
            DirectoryEntry root = new DirectoryEntry("%root%");
            DirectoryEntry subFolder = new DirectoryEntry("sub");
            FileEntry file = new FileEntry("myfile.txt");

            root.Directories.Add(subFolder);
            subFolder.Parent = root;

            subFolder.Files.Add(file);
            file.Parent = subFolder;

            string outputSource = "sub/myfile.txt";
            string outputDest = "%root%\\sub\\myfile.txt";

            Assert.AreEqual(outputSource, file.SourceLocation);
            Assert.AreEqual(outputDest, file.DestinationLocation);
        }
    }
}
