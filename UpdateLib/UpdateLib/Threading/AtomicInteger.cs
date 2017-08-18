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

namespace MatthiWare.UpdateLib.Threading
{
    public class AtomicInteger
    {
        private int m_value;
        private readonly object sync = new object();

        public int Value
        {
            get
            {
                lock (sync)
                    return m_value;
            }
            set
            {
                lock (sync)
                    m_value = value;
            }
        }

        public AtomicInteger()
            : this(0)
        { }

        public AtomicInteger(int startingValue)
        {
            m_value = startingValue;
        }

        public int Increment()
        {
            lock (sync)
                return ++m_value;
        }

        public int Decrement()
        {
            lock (sync)
                return --m_value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
