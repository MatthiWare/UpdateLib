using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Security;
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
