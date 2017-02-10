using MatthiWare.UpdateLib.Security;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateLib.Tests.Security
{
    [TestFixture]
    public class HashUtilTest
    {
        private string temp_file;

        [SetUp]
        public void Before()
        {
            temp_file = Path.GetTempPath() + Guid.NewGuid().ToString() + "hash_util_test.tmp";
            using (StreamWriter sw = new StreamWriter(File.Open(temp_file, FileMode.OpenOrCreate, FileAccess.Write),Encoding.UTF8))
            {
                sw.WriteLine("This is a test");
            }
        }

        [Test]
        public void GetHashFromNullShouldThrowNullArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => HashUtil.GetHash(null));
        }

        [Test]
        public void GetHashFromNotExistingFileShouldThrowFileNotFoundException()
        {
            if (File.Exists(temp_file))
                File.Delete(temp_file);

            Assert.Throws<FileNotFoundException>(() => HashUtil.GetHash(temp_file));
        }

        [Test]
        public void GetHashShouldReturnTheCorrectHash()
        {
            string hash = HashUtil.GetHash(temp_file);
            Assert.AreEqual("41CA5D124F0D02EB9A7AED0332D0965C", hash);
        }

        [TearDown]
        public void CleanUp()
        {
            if (File.Exists(temp_file))
                File.Delete(temp_file);
        }

    }
}
