using MatthiWare.UpdateLib.Files;
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
    public class RegistryHelperTest
    {
        [Test]
        public void TestNotExistingRootKey()
        {
            RegistryKeyEntry regKey = new RegistryKeyEntry("unexisting key", Microsoft.Win32.RegistryValueKind.Unknown, null);
            regKey.Parent = new DirectoryEntry("not existing");

            Assert.AreEqual(null, RegistryHelper.GetOrMakeKey(regKey));
        }

        [Test]
        public void GetOrMakeKeyThrowsException()
        {
            RegistryKeyEntry key = null;
            Assert.Throws<ArgumentNullException>(() => RegistryHelper.GetOrMakeKey(key));
            Assert.Throws<ArgumentNullException>(() => RegistryHelper.GetOrMakeKey(string.Empty));
        }

    }
}
