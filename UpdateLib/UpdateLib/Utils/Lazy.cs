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

using System;

namespace MatthiWare.UpdateLib.Utils
{
    /// <summary>
    /// Threadsafe lazy initialization
    /// </summary>
    /// <typeparam name="T">The return type</typeparam>
    public class Lazy<T>
    {
        private readonly Func<T> m_initFunction;

        private readonly object sync = new object();
        private bool m_initialized = false;

        private T m_storedValue = default(T);

        /// <summary>
        /// Gets the value and initializes once.
        /// </summary>
        public T Value
        {
            get
            {
                if (!m_initialized)
                    lock (sync)
                        if (!m_initialized)
                        {
                            m_storedValue = m_initFunction();
                            m_initialized = true;
                        }

                return m_storedValue;
            }
            set
            {
                lock (sync)
                {
                    m_initialized = true;
                    m_storedValue = value;
                }
            }
        }

        /// <summary>
        /// Resets the lazy function
        /// </summary>
        public void Reset()
        {
            lock (sync)
                m_initialized = false;
        }

        /// <summary>
        /// Makes a new instance of an lazy initializer
        /// </summary>
        /// <param name="initFunction">The lazy initialization function</param>
        public Lazy(Func<T> initFunction)
        {
            m_initFunction = initFunction;
        }


    }
}
