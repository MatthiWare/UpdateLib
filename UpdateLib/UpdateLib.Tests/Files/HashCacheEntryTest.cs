using MatthiWare.UpdateLib.Files;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace UpdateLib.Tests.Files
{
    [TestFixture]
    public class HashCacheEntryTest
    {
        private string temp_file;
        [SetUp]
        public void Before()
        {
            temp_file = Path.GetTempPath() + Guid.NewGuid().ToString() + "hash_util_test.tmp";
            using (StreamWriter sw = new StreamWriter(File.Open(temp_file, FileMode.OpenOrCreate, FileAccess.Write), Encoding.Default))
            {
                sw.WriteLine("This is a test");
            }
        }

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

        [Test]
        public void TestHashing()
        {
            string hash = "07849dc26fcbb2f3bd5f57bdf214bae374575f1bd4e6816482324799417cb379".ToUpper();
            HashCacheEntry entry = new HashCacheEntry(temp_file);
            Assert.AreEqual(hash, entry.Hash);
            long ticks = entry.Ticks;

            EditTempFile();
            
            entry.Recalculate(File.GetLastWriteTime(temp_file).Ticks);

            Assert.AreNotEqual(ticks, entry.Ticks);
            Assert.AreNotEqual(hash, entry.Hash);
        }

        private void EditTempFile()
        {
            using (StreamWriter sw = new StreamWriter(File.Open(temp_file, FileMode.Open, FileAccess.Write), Encoding.Default))
            {
                sw.WriteLine("edited file");
            }
        }


        [TearDown]
        public void CleanUp()
        {
            if (File.Exists(temp_file))
                File.Delete(temp_file);
        }
    }
}
