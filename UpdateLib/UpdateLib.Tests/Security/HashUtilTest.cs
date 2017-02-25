using MatthiWare.UpdateLib.Security;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
            using (StreamWriter sw = new StreamWriter(File.Open(temp_file, FileMode.OpenOrCreate, FileAccess.Write),Encoding.Default))
            {
                sw.WriteLine("This is a test");
            }
        }

        [Test]
        public void GetHashFromNullShouldThrowNullArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => HashUtil.GetHash<SHA256>(null));
        }

        [Test]
        public void GetHashFromNotExistingFileShouldThrowFileNotFoundException()
        {
            if (File.Exists(temp_file))
                File.Delete(temp_file);

            Assert.Throws<FileNotFoundException>(() => HashUtil.GetHash<SHA256>(temp_file));
        }

        [Test]
        public void GetHashShouldReturnTheCorrectHash()
        {
            string hash = HashUtil.GetHash<SHA256>(temp_file);
            Assert.AreEqual("07849dc26fcbb2f3bd5f57bdf214bae374575f1bd4e6816482324799417cb379".ToUpper(), hash);
        }

        [TearDown]
        public void CleanUp()
        {
            if (File.Exists(temp_file))
                File.Delete(temp_file);
        }

    }
}
