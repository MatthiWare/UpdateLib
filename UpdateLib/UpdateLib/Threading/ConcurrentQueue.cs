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

using System.Collections.Generic;

namespace MatthiWare.UpdateLib.Threading
{
    public class ConcurrentQueue<T>
    {
        private readonly Queue<T> m_queue;
        private readonly object sync = new object();

        public ConcurrentQueue()
        {
            m_queue = new Queue<T>();
        }

        public void Enqueue(T item)
        {
            lock (sync)
                m_queue.Enqueue(item);
        }

        public int Count
        {
            get
            {
                lock (sync)
                    return m_queue.Count;
            }
        }

        public bool TryDequeue(out T data)
        {
            data = default(T);

            lock (sync)
            {
                if (m_queue.Count <= 0)
                    return false;

                data = m_queue.Dequeue();
                return true;
            }
        }

        public void Clear()
        {
            lock (sync)
                m_queue.Clear();
        }
    }
}
