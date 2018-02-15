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

using MatthiWare.UpdateLib.Utils;

using NUnit.Framework;

namespace UpdateLib.Tests.Util
{
    [TestFixture]
    public class LazyTests
    {

        [Test]
        public void TestLazyInitializesCorreclty()
        {
            var myObject = new Lazy<string>(() => "test");

            Assert.AreEqual("test", myObject);
        }

        [Test]
        public void TestLaszySet()
        {
            var myObj = new Lazy<string>(() => "test");
            myObj.Value = "new";

            Assert.AreEqual("new", myObj);
        }

        [Test]
        public void TestLazyReset()
        {
            SwitchObject switcher = new SwitchObject();

            var myLazy = new Lazy<string>(switcher.Get);

            Assert.AreEqual(switcher.Get(), myLazy);

            switcher.Toggle();

            Assert.AreNotEqual(switcher.Get(), myLazy);

            myLazy.Reset();

            Assert.AreEqual(switcher.Get(), myLazy);
        }

        private class SwitchObject
        {
            private bool m_state = false;

            public void Toggle()
            {
                m_state = !m_state;
            }

            public string Get()
                => m_state ? "true" : "false";
        }
    }
}
