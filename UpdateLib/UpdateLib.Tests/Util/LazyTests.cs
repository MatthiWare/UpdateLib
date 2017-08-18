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

using NUnit.Framework;

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
