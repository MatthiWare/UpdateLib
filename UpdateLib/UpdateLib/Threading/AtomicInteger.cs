using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
