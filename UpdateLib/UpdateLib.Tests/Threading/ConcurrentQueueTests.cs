using MatthiWare.UpdateLib.Threading;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateLib.Tests.Threading
{
    [TestFixture]
    public class ConcurrentQueueTests
    {
        private ConcurrentQueue<object> m_queue;

        [SetUp]
        public void Setup()
        {
            m_queue = new ConcurrentQueue<object>();
        }

        [Test, Parallelizable]
        public void CountShouldBeZero()
        {
            Assert.AreEqual(0, m_queue.Count);
        }

        [Test, Parallelizable]
        public void AddingItemsShouldIncrementCount()
        {
            m_queue.Enqueue(new object());
            Assert.AreEqual(1, m_queue.Count);
            m_queue.Enqueue(new object());
            Assert.AreEqual(2, m_queue.Count);
            m_queue.Enqueue(new object());
            Assert.AreEqual(3, m_queue.Count);
        }

        [Test, Parallelizable]
        public void DequeueShouldDecrementCount()
        {
            object data = null;

            m_queue.Enqueue(new object());
            Assert.AreEqual(1, m_queue.Count);
            m_queue.Enqueue(new object());
            Assert.AreEqual(2, m_queue.Count);
            m_queue.Enqueue(new object());
            Assert.AreEqual(3, m_queue.Count);

            Assert.IsTrue(m_queue.TryDequeue(out data));
            Assert.AreEqual(2, m_queue.Count);
            Assert.IsTrue(m_queue.TryDequeue(out data));
            Assert.AreEqual(1, m_queue.Count);
            Assert.IsTrue(m_queue.TryDequeue(out data));
            Assert.AreEqual(0, m_queue.Count);
        }

        [Test, Parallelizable]
        public void ClearShouldEmptyTheQueue()
        {
            m_queue.Enqueue(new object());
            Assert.AreEqual(1, m_queue.Count);
            m_queue.Enqueue(new object());
            Assert.AreEqual(2, m_queue.Count);
            m_queue.Enqueue(new object());
            Assert.AreEqual(3, m_queue.Count);

            m_queue.Clear();

            Assert.AreEqual(0, m_queue.Count);
        }

        [TearDown]
        public void CleanUp()
        {
            m_queue.Clear();
            m_queue = null;
        }

    }
}
