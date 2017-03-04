using MatthiWare.UpdateLib.Files;
using NUnit.Framework;
using System;
using System.IO;

namespace UpdateLib.Tests.Files
{
    [TestFixture]
    public class HashCacheEntryTest
    {
        [Test]
        public void NullEntryShouldThrowArgumentNullException()
        {
            Assert.Catch<ArgumentNullException>(() => new HashCacheEntry(null));
        }

        [Test]
        public void NotExistingFileShouldThrowFileNotFoundException()
        {
            Assert.Catch<FileNotFoundException>(() => new HashCacheEntry("notexistingfile.dll"));
        }
    }
}
