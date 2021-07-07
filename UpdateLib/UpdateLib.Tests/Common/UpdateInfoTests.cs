using System;

using MatthiWare.UpdateLib.Common;

using NUnit.Framework;

namespace UpdateLib.Tests.Common
{
    [TestFixture]
    public class UpdateInfoTests
    {

        [Test]
        public void ConstructInvalidCatalogEntryThrowsCorrectExceotion()
        {
            Assert.Catch<ArgumentNullException>(() => new UpdateInfo(null, null, null, null));
            Assert.Catch<ArgumentNullException>(() => new UpdateInfo("1.0.0", "0.5.0", null, null));
            Assert.Catch<ArgumentNullException>(() => new UpdateInfo("1.0.0", "0.5.0", "test", null));

            Assert.Catch<ArgumentException>(() => new UpdateInfo("1.0.0", "1.5.0", "test", null));
        }

        [Test]
        public void TestIsPatchProperty()
        {
            var catalogEntry = new UpdateInfo("1.0", null, "test", "test");

            Assert.IsFalse(catalogEntry.IsPatch);

            catalogEntry = new UpdateInfo("1.0", "0.9", "test", "test");

            Assert.IsTrue(catalogEntry.IsPatch);
        }
    }
}
