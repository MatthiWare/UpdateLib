using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Files;

using NUnit.Framework;

namespace UpdateLib.Tests.Files
{
    [TestFixture]
    public class UpdateCatalogFileTests
    {

        [Test]
        public void TestTryGetLatestVersionGood()
        {
            var file = GenerateCatalogFile();

            Assert.IsTrue(file.TryGetLatestUpdateForVersion("0.1", out CatalogEntry entry));

            Assert.AreEqual(entry.FileName, "version_1.0-full");
        }

        [Test]
        public void TestTryGetLatestVersionGood2()
        {
            var file = GenerateCatalogFile();

            Assert.IsTrue(file.TryGetLatestUpdateForVersion("0.4", out CatalogEntry entry));

            Assert.AreEqual(entry.FileName, "version_0.4-1.0");
        }

        [Test]
        public void TestTryGetLatestVersionEmptryFileReturnsFalse()
        {
            var file = GenerateCatalogFile(true);

            Assert.IsFalse(file.TryGetLatestUpdateForVersion("0.1", out _));
        }


        private UpdateCatalogFile GenerateCatalogFile(bool empty = false)
        {
            UpdateCatalogFile file = new UpdateCatalogFile();

            if (!empty)
            {
                file.Catalog.Add(new CatalogEntry("1.0", "0.4", "version_0.4-1.0", "0x0"));
                file.Catalog.Add(new CatalogEntry("0.7", "0.1", "version_0.1-0.7", "0x0"));
                file.Catalog.Add(new CatalogEntry("0.8", "0.7", "version_0.7-0.8", "0x0"));
                file.Catalog.Add(new CatalogEntry("0.9", "0.8", "version_0.8-0.9", "0x0"));
                file.Catalog.Add(new CatalogEntry("1.0", null, "version_1.0-full", "0x0"));
                file.Catalog.Add(new CatalogEntry("1.0", "0.5", "version_0.5-1.0", "0x0"));

            }

            return file;
        }
    }
}
