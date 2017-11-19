/*  Copyright (C) 2016 - MatthiWare (Matthias Beerens)
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
using System.IO;
using MatthiWare.UpdateLib.Utils;

namespace MatthiWare.UpdateLib.Compression.VCDiff
{
    internal class AddressCache
    {
        const byte SelfMode = 0;
        const byte HereMode = 1;

        int m_nearSize, m_sameSize, m_nextNearSlot;
        int[] m_near, m_same;

        Stream m_addressStream;

        internal AddressCache(int nearSize, int sameSize)
        {
            m_nearSize = nearSize;
            m_sameSize = sameSize;
            m_near = new int[nearSize];
            m_same = new int[sameSize * 256];
        }

        public void Reset(byte[] addresses)
        {
            m_nextNearSlot = 0;
            Array.Clear(m_near, 0, m_near.Length);
            Array.Clear(m_same, 0, m_same.Length);

            m_addressStream = new MemoryStream(addresses, false);
        }

        internal int DecodeAddress(int here, byte mode)
        {
            int ret;

            if (mode == SelfMode)
                ret = m_addressStream.ReadBigEndian7BitEncodedInt();
            else if (mode == HereMode)
                ret = here - m_addressStream.ReadBigEndian7BitEncodedInt();
            else if (mode - 2 < m_nearSize) // near cache
                ret = m_near[mode - 2] + m_addressStream.ReadBigEndian7BitEncodedInt();
            else // same cache
                ret = m_same[((mode - (2 + m_nearSize)) * 256) + m_addressStream.CheckedReadByte()];

            Update(ret);

            return ret;
        }

        private void Update(int address)
        {
            if (m_nearSize > 0)
            {
                m_near[m_nextNearSlot] = address;
                m_nextNearSlot = (m_nextNearSlot + 1) % m_nearSize;
            }

            if (m_sameSize > 0)
                m_same[address % (m_sameSize * 256)] = address;
        }

    }
}
