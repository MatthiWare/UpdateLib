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

using MatthiWare.UpdateLib.Threading;
using NUnit.Framework;
using System.Threading;

namespace UpdateLib.Tests.Threading
{
    [TestFixture]
    public class AtomicIntegerTest
    {


        [Test, Parallelizable]
        public void AtomicIntegerShouldGetCorrectValueFromConstructor()
        {
            AtomicInteger myInt = new AtomicInteger(50);
            Assert.AreEqual(50, myInt.Value);
        }

        [Test, Parallelizable]
        public void AtomicIntegerShouldIncrementCorrectly()
        {
            AtomicInteger myInt = new AtomicInteger();
            Assert.AreEqual(1, myInt.Increment());
        }

        [Test, Parallelizable]
        public void AtomicIntegerShouldDecrementCorrectly()
        {
            AtomicInteger myInt = new AtomicInteger();
            Assert.AreEqual(-1, myInt.Decrement());
        }

        [Test, Parallelizable]
        public void AtomicIntegerShouldHaveCorrectValueAfterSetting()
        {
            AtomicInteger myInt = new AtomicInteger(50);
            Assert.AreEqual(50, myInt.Value);
            myInt.Value = 30;
            Assert.AreEqual(30, myInt.Value);
        }

        [Test, Parallelizable]
        public void MultithreadedTest()
        {
            AtomicInteger myInt = new AtomicInteger(50);

            ThreadStart IncrementAction = new ThreadStart(() => myInt.Increment());

            ThreadStart DecrementAction = new ThreadStart(() => myInt.Decrement());

            Thread[] threads = new Thread[10];

            for (int i = 0; i < 10; i++)
            {
                threads[i] = new Thread((i % 2 == 0) ? IncrementAction : DecrementAction);
                threads[i].Start();
            }

            for (int i = 0; i < 10; i++)
            {
                threads[i].Join();
            }

            Assert.AreEqual(50, myInt.Value);
        }

        [Test, Parallelizable]
        public void ToStringShouldDisplayValue()
        {
            AtomicInteger myInt = new AtomicInteger(30);
            Assert.AreEqual(myInt.Value.ToString(), myInt.ToString());
        }
    }
}
