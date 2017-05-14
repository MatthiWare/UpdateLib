using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
