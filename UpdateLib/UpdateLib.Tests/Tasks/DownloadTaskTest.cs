using MatthiWare.UpdateLib;
using MatthiWare.UpdateLib.Files;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateLib.Tests.Tasks
{
    //[TestFixture]
    public class DownloadTaskTest
    {

      //  [OneTimeSetUp]
        public void Before()
        {
            var path = new DirectoryInfo(".").FullName;

            Updater.Instance
                .ConfigureUpdateUrl($@"file:///{path}")
                .ConfigureInstallationMode(InstallationMode.Local)
                .ConfigureUnsafeConnections(true)
                .Initialize();
        }

       // [Test]
        public void TestDownload()
        {
            FileEntry fe = MakeFileEntry();

            var s = fe.DestinationLocation;
        }

        private FileEntry MakeFileEntry()
        {
            DirectoryEntry appDir = new DirectoryEntry("%appdir%");
            FileEntry file = new FileEntry("testFile");


            appDir.Add(file);

            return file;
        }

    }
}
