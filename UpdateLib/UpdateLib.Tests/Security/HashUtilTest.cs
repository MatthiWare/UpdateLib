/*  UpdateLib - .Net auto update library
 *  Copyright (C) 2016 - MatthiWare (Matthias Beerens)
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published
 *  by the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using MatthiWare.UpdateLib.Security;
using NUnit.Framework;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

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
            Assert.Throws<ArgumentNullException>(() => HashUtil.GetHash(null));
        }

        [Test]
        public void GetHashFromNotExistingFileShouldThrowFileNotFoundException()
        {
            CleanUp();

            Assert.Throws<FileNotFoundException>(() => HashUtil.GetHash(temp_file));
        }

        [Test]
        public void GetHashShouldReturnTheCorrectHash()
        {
            Assert.AreEqual("07849dc26fcbb2f3bd5f57bdf214bae374575f1bd4e6816482324799417cb379".ToUpper(), HashUtil.GetHash<SHA256>(temp_file));
            Assert.AreEqual("ecf6475cfcffd6eca2d2d9669e1f6c93e622e6b7".ToUpper(), HashUtil.GetHash<SHA1>(temp_file));
            Assert.AreEqual("ccb935c230474801da1fb876ec9e08b0536598aef7ed6f3eb6db17b85a8701a47d3abeb084687d50eb6ce7136b93f47d489c3df321330338b62edcdd439a2fa0".ToUpper(), HashUtil.GetHash<SHA512>(temp_file));
            Assert.AreEqual("c36fbf21a1e2cb18e73bd3707555df4c".ToUpper(), HashUtil.GetHash<MD5>(temp_file));
        }

        [TearDown]
        public void CleanUp()
        {
            if (File.Exists(temp_file))
                File.Delete(temp_file);
        }

    }
}
