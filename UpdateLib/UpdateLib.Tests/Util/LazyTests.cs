using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatthiWare.UpdateLib.Utils;

namespace UpdateLib.Tests.Util
{
    [TestFixture]
    public class LazyTests
    {

        [Test]
        public void TestLazyInitializesCorreclty()
        {
            MatthiWare.UpdateLib.Utils.Lazy<string> myObject = new MatthiWare.UpdateLib.Utils.Lazy<string>(GetMyInitValue);

            Assert.AreEqual("test", myObject.Value);
        }

        private string GetMyInitValue()
        {
            return "test";
        }

    }
}
