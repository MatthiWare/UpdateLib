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
