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
            MatthiWare.UpdateLib.Utils.Lazy<string> myObject = new MatthiWare.UpdateLib.Utils.Lazy<string>(() => "test");

            Assert.AreEqual("test", myObject.Value);
        }

        [Test]
        public void TestLaszySet()
        {
            MatthiWare.UpdateLib.Utils.Lazy<string> myObj = new MatthiWare.UpdateLib.Utils.Lazy<string>(() => "test");
            myObj.Value = "new";

            Assert.AreEqual("new", myObj.Value);
        }

        [Test]
        public void TestLazyReset()
        {
            SwitchObject switcher = new SwitchObject();

            MatthiWare.UpdateLib.Utils.Lazy<string> myLazy = new MatthiWare.UpdateLib.Utils.Lazy<string>(switcher.Get);

            Assert.AreEqual(switcher.Get(), myLazy.Value);

            switcher.Toggle();

            Assert.AreNotEqual(switcher.Get(), myLazy.Value);

            myLazy.Reset();

            Assert.AreEqual(switcher.Get(), myLazy.Value);
        }

        private class SwitchObject
        {
            private bool m_state = false;

            public void Toggle()
            {
                m_state = !m_state;
            }

            public string Get()
            {
                return m_state ? "true" : "false";
            }
        }
    }
}
