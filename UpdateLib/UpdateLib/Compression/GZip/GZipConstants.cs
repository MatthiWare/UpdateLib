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

/* Copyright © 2000-2016 SharpZipLib Contributors
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this
 * software and associated documentation files (the "Software"), to deal in the Software
 * without restriction, including without limitation the rights to use, copy, modify, merge,
 * publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
 * to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
 * FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 */


namespace MatthiWare.UpdateLib.Compression.GZip
{
    /// <summary>
    /// This class contains constants used for gzip.
    /// </summary>
    public static class GZipConstants
    {
        /// <summary>
        /// Magic number found at start of GZIP header
        /// </summary>
        public const int GZIP_MAGIC = 0x1F8B;

        /*  The flag byte is divided into individual bits as follows:
			
			bit 0   FTEXT
			bit 1   FHCRC
			bit 2   FEXTRA
			bit 3   FNAME
			bit 4   FCOMMENT
			bit 5   reserved
			bit 6   reserved
			bit 7   reserved
		 */

        /// <summary>
        /// Flag bit mask for text
        /// </summary>
        public const int FTEXT = 0x1;

        /// <summary>
        /// Flag bitmask for Crc
        /// </summary>
        public const int FHCRC = 0x2;

        /// <summary>
        /// Flag bit mask for extra
        /// </summary>
        public const int FEXTRA = 0x4;

        /// <summary>
        /// flag bitmask for name
        /// </summary>
        public const int FNAME = 0x8;

        /// <summary>
        /// flag bit mask indicating comment is present
        /// </summary>
        public const int FCOMMENT = 0x10;
    }
}
