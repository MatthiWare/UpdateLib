using MatthiWare.UpdateLib.Threading;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
