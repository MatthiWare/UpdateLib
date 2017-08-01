using MatthiWare.UpdateLib;
using MatthiWare.UpdateLib.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateLib.Tests.Util
{
    [TestFixture]
    public class IOUtilsTest
    {

        Updater instance;

        [SetUp]
        public void Setup()
        {
            instance = Updater.Instance;
        }

        [Test]
        public void TestAppDataPathLocal()
        {
            instance.ConfigureInstallationMode(InstallationMode.Local);

            string path = IOUtils.AppDataPath;

            Assert.IsTrue(path.Contains("Local"), $"Path: '{path}' didn't contain 'Local'");
        }

        [Test]
        public void TestAppDataPathRoaming()
        {
            instance.ConfigureInstallationMode(InstallationMode.Shared);

            string path = IOUtils.AppDataPath;

            Assert.IsTrue(path.Contains("Roaming"), $"Path: '{path}' didn't contain 'Roaming'");
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
