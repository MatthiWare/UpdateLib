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

using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;

using MatthiWare.UpdateLib.Utils;

namespace MatthiWare.UpdateLib.Compression.Zip
{
    #region ZipFile Class
    /// <summary>
    /// This class represents a Zip archive.  You can ask for the contained
    /// entries, or get an input stream for a file entry.  The entry is
    /// automatically decompressed.
    /// 
    /// You can also update the archive adding or deleting entries.
    /// 
    /// This class is thread safe for input:  You can open input streams for arbitrary
    /// entries in different threads.
    /// <br/>
    /// <br/>Author of the original java version : Jochen Hoenicke
    /// </summary>
    /// <example>
    /// <code>
    /// using System;
    /// using System.Text;
    /// using System.Collections;
    /// using System.IO;
    /// 
    /// using ICSharpCode.SharpZipLib.Zip;
    /// 
    /// class MainClass
    /// {
    /// 	static public void Main(string[] args)
    /// 	{
    /// 		using (ZipFile zFile = new ZipFile(args[0])) {
    /// 			Console.WriteLine("Listing of : " + zFile.Name);
    /// 			Console.WriteLine("");
    /// 			Console.WriteLine("Raw Size    Size      Date     Time     Name");
    /// 			Console.WriteLine("--------  --------  --------  ------  ---------");
    /// 			foreach (ZipEntry e in zFile) {
    /// 				if ( e.IsFile ) {
    /// 					DateTime d = e.DateTime;
    /// 					Console.WriteLine("{0, -10}{1, -10}{2}  {3}   {4}", e.Size, e.CompressedSize,
    /// 						d.ToString("dd-MM-yy"), d.ToString("HH:mm"),
    /// 						e.Name);
    /// 				}
    /// 			}
    /// 		}
    /// 	}
    /// }
    /// </code>
    /// </example>
    public class ZipFile : IEnumerable, IDisposable
    {
        #region Constructors

        /// <summary>
        /// Opens a Zip file reading the given <see cref="FileStream"/>.
        /// </summary>
        /// <param name="file">The <see cref="FileStream"/> to read archive data from.</param>
        /// <exception cref="ArgumentNullException">The supplied argument is null.</exception>
        /// <exception cref="IOException">
        /// An i/o error occurs.
        /// </exception>
        /// <exception cref="ZipException">
        /// The file doesn't contain a valid zip archive.
        /// </exception>
        public ZipFile(FileStream file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            if (!file.CanSeek)
                throw new ArgumentException("Stream is not seekable", nameof(file));

            baseStream_ = file;
            name_ = file.Name;
            isStreamOwner = true;

            try
            {
                ReadEntries();
            }
            catch
            {
                DisposeInternal(true);
                throw;
            }
        }

        #endregion

        #region Destructors and Closing
        /// <summary>
        /// Finalize this instance.
        /// </summary>
        ~ZipFile()
        {
            Dispose(false);
        }

        /// <summary>
        /// Closes the ZipFile.  If the stream is <see cref="IsStreamOwner">owned</see> then this also closes the underlying input stream.
        /// Once closed, no further instance methods should be called.
        /// </summary>
        /// <exception cref="System.IO.IOException">
        /// An i/o error occurs.
        /// </exception>
        public void Close()
        {
            DisposeInternal(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Properties
        /// <summary>
        /// Get/set a flag indicating if the underlying stream is owned by the ZipFile instance.
        /// If the flag is true then the stream will be closed when <see cref="Close">Close</see> is called.
        /// </summary>
        /// <remarks>
        /// The default value is true in all cases.
        /// </remarks>
        public bool IsStreamOwner
        {
            get { return isStreamOwner; }
            set { isStreamOwner = value; }
        }

        /// <summary>
        /// Get the number of entries contained in this <see cref="ZipFile"/>.
        /// </summary>
        public long Count
        {
            get
            {
                return entries_.Length;
            }
        }

        /// <summary>
        /// Indexer property for ZipEntries
        /// </summary>
        [IndexerName("EntryByIndex")]
        public ZipEntry this[int index]
        {
            get
            {
                return (ZipEntry)entries_[index].Clone();
            }
        }

        #endregion

        #region Input Handling
        /// <summary>
        /// Gets an enumerator for the Zip entries in this Zip file.
        /// </summary>
        /// <returns>Returns an <see cref="IEnumerator"/> for this archive.</returns>
        /// <exception cref="ObjectDisposedException">
        /// The Zip file has been closed.
        /// </exception>
        public IEnumerator GetEnumerator()
        {
            if (isDisposed_)
            {
                throw new ObjectDisposedException("ZipFile");
            }

            return new ZipEntryEnumerator(entries_);
        }

        /// <summary>
        /// Return the index of the entry with a matching name
        /// </summary>
        /// <param name="name">Entry name to find</param>
        /// <param name="ignoreCase">If true the comparison is case insensitive</param>
        /// <returns>The index position of the matching entry or -1 if not found</returns>
        /// <exception cref="ObjectDisposedException">
        /// The Zip file has been closed.
        /// </exception>
        public int FindEntry(string name, bool ignoreCase)
        {
            if (isDisposed_)
            {
                throw new ObjectDisposedException("ZipFile");
            }

            // TODO: This will be slow as the next ice age for huge archives!
            for (int i = 0; i < entries_.Length; i++)
            {
                if (string.Compare(name, entries_[i].Name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Searches for a zip entry in this archive with the given name.
        /// String comparisons are case insensitive
        /// </summary>
        /// <param name="name">
        /// The name to find. May contain directory components separated by slashes ('/').
        /// </param>
        /// <returns>
        /// A clone of the zip entry, or null if no entry with that name exists.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// The Zip file has been closed.
        /// </exception>
        public ZipEntry GetEntry(string name)
        {
            if (isDisposed_)
            {
                throw new ObjectDisposedException("ZipFile");
            }

            int index = FindEntry(name, true);
            return (index >= 0) ? (ZipEntry)entries_[index].Clone() : null;
        }

        #endregion

        #endregion

        #region Disposing

        #region IDisposable Members
        void IDisposable.Dispose()
        {
            Close();
        }
        #endregion

        void DisposeInternal(bool disposing)
        {
            if (!isDisposed_)
            {
                isDisposed_ = true;
                entries_ = new ZipEntry[0];

                if (IsStreamOwner && (baseStream_ != null))
                    lock (baseStream_)
                        baseStream_.Dispose();
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the this instance and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources;
        /// false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            DisposeInternal(disposing);
        }

        #endregion

        #region Internal routines
        #region Reading
        /// <summary>
        /// Read an unsigned short in little endian byte order.
        /// </summary>
        /// <returns>Returns the value read.</returns>
        /// <exception cref="EndOfStreamException">
        /// The stream ends prematurely
        /// </exception>
        ushort ReadLEUshort()
        {
            int data1 = baseStream_.ReadByte();

            if (data1 < 0)
            {
                throw new EndOfStreamException("End of stream");
            }

            int data2 = baseStream_.ReadByte();

            if (data2 < 0)
            {
                throw new EndOfStreamException("End of stream");
            }


            return unchecked((ushort)((ushort)data1 | (ushort)(data2 << 8)));
        }

        /// <summary>
        /// Read a uint in little endian byte order.
        /// </summary>
        /// <returns>Returns the value read.</returns>
        /// <exception cref="IOException">
        /// An i/o error occurs.
        /// </exception>
        /// <exception cref="System.IO.EndOfStreamException">
        /// The file ends prematurely
        /// </exception>
        uint ReadLEUint()
        {
            return (uint)(ReadLEUshort() | (ReadLEUshort() << 16));
        }

        ulong ReadLEUlong()
        {
            return ReadLEUint() | ((ulong)ReadLEUint() << 32);
        }

        #endregion
        // NOTE this returns the offset of the first byte after the signature.
        long LocateBlockWithSignature(int signature, long endLocation, int minimumBlockSize, int maximumVariableData)
        {
            using (ZipHelperStream les = new ZipHelperStream(baseStream_))
            {
                return les.LocateBlockWithSignature(signature, endLocation, minimumBlockSize, maximumVariableData);
            }
        }

        /// <summary>
        /// Search for and read the central directory of a zip file filling the entries array.
        /// </summary>
        /// <exception cref="System.IO.IOException">
        /// An i/o error occurs.
        /// </exception>
        /// <exception cref="ICSharpCode.SharpZipLib.Zip.ZipException">
        /// The central directory is malformed or cannot be found
        /// </exception>
        void ReadEntries()
        {
            // Search for the End Of Central Directory.  When a zip comment is
            // present the directory will start earlier
            // 
            // The search is limited to 64K which is the maximum size of a trailing comment field to aid speed.
            // This should be compatible with both SFX and ZIP files but has only been tested for Zip files
            // If a SFX file has the Zip data attached as a resource and there are other resources occuring later then
            // this could be invalid.
            // Could also speed this up by reading memory in larger blocks.			

            if (baseStream_.CanSeek == false)
                throw new ZipException("ZipFile stream must be seekable");

            long locatedEndOfCentralDir = LocateBlockWithSignature(ZipConstants.EndOfCentralDirectorySignature,
                baseStream_.Length, ZipConstants.EndOfCentralRecordBaseSize, 0xffff);

            if (locatedEndOfCentralDir < 0)
                throw new ZipException("Cannot find central directory");

            // Read end of central directory record
            ushort thisDiskNumber = ReadLEUshort();
            ushort startCentralDirDisk = ReadLEUshort();
            ulong entriesForThisDisk = ReadLEUshort();
            ulong entriesForWholeCentralDir = ReadLEUshort();
            ulong centralDirSize = ReadLEUint();
            long offsetOfCentralDir = ReadLEUint();
            uint commentSize = ReadLEUshort();

            comment_ = (commentSize > 0) ?
                ZipConstants.ConvertToString(baseStream_.CheckedReadBytes((int)commentSize)) :
                string.Empty;

            bool isZip64 = false;

            // Check if zip64 header information is required.
            if ((thisDiskNumber == 0xffff) ||
                (startCentralDirDisk == 0xffff) ||
                (entriesForThisDisk == 0xffff) ||
                (entriesForWholeCentralDir == 0xffff) ||
                (centralDirSize == 0xffffffff) ||
                (offsetOfCentralDir == 0xffffffff))
            {
                isZip64 = true;

                long offset = LocateBlockWithSignature(ZipConstants.Zip64CentralDirLocatorSignature, locatedEndOfCentralDir, 0, 0x1000);
                if (offset < 0)
                    throw new ZipException("Cannot find Zip64 locator");

                // number of the disk with the start of the zip64 end of central directory 4 bytes 
                // relative offset of the zip64 end of central directory record 8 bytes 
                // total number of disks 4 bytes 
                ReadLEUint(); // startDisk64 is not currently used
                ulong offset64 = ReadLEUlong();
                uint totalDisks = ReadLEUint();

                baseStream_.Position = (long)offset64;
                long sig64 = ReadLEUint();

                if (sig64 != ZipConstants.Zip64CentralFileHeaderSignature)
                    throw new ZipException(string.Format("Invalid Zip64 Central directory signature at {0:X}", offset64));

                // NOTE: Record size = SizeOfFixedFields + SizeOfVariableData - 12.
                ulong recordSize = ReadLEUlong();
                int versionMadeBy = ReadLEUshort();
                int versionToExtract = ReadLEUshort();
                uint thisDisk = ReadLEUint();
                uint centralDirDisk = ReadLEUint();
                entriesForThisDisk = ReadLEUlong();
                entriesForWholeCentralDir = ReadLEUlong();
                centralDirSize = ReadLEUlong();
                offsetOfCentralDir = (long)ReadLEUlong();

                // NOTE: zip64 extensible data sector (variable size) is ignored.
            }

            entries_ = new ZipEntry[entriesForThisDisk];

            // SFX/embedded support, find the offset of the first entry vis the start of the stream
            // This applies to Zip files that are appended to the end of an SFX stub.
            // Or are appended as a resource to an executable.
            // Zip files created by some archivers have the offsets altered to reflect the true offsets
            // and so dont require any adjustment here...
            // TODO: Difficulty with Zip64 and SFX offset handling needs resolution - maths?
            if (!isZip64 && (offsetOfCentralDir < locatedEndOfCentralDir - (4 + (long)centralDirSize)))
            {
                offsetOfFirstEntry = locatedEndOfCentralDir - (4 + (long)centralDirSize + offsetOfCentralDir);

                if (offsetOfFirstEntry <= 0)
                    throw new ZipException("Invalid embedded zip archive");
            }

            baseStream_.Seek(offsetOfFirstEntry + offsetOfCentralDir, SeekOrigin.Begin);

            for (ulong i = 0; i < entriesForThisDisk; i++)
            {
                if (ReadLEUint() != ZipConstants.CentralHeaderSignature)
                    throw new ZipException("Wrong Central Directory signature");

                int versionMadeBy = ReadLEUshort();
                int versionToExtract = ReadLEUshort();
                int bitFlags = ReadLEUshort();
                int method = ReadLEUshort();
                uint dostime = ReadLEUint();
                uint crc = ReadLEUint();
                var csize = (long)ReadLEUint();
                var size = (long)ReadLEUint();
                int nameLen = ReadLEUshort();
                int extraLen = ReadLEUshort();
                int commentLen = ReadLEUshort();

                int diskStartNo = ReadLEUshort();  // Not currently used
                int internalAttributes = ReadLEUshort();  // Not currently used

                uint externalAttributes = ReadLEUint();
                long offset = ReadLEUint();

                byte[] buffer = new byte[Math.Max(nameLen, commentLen)];

                baseStream_.CheckedReadBytes(buffer, 0, nameLen);
                string name = ZipConstants.ConvertToStringExt(bitFlags, buffer, nameLen);

                var entry = new ZipEntry(name, versionToExtract, versionMadeBy, (CompressionMethod)method);
                entry.Crc = crc & 0xffffffffL;
                entry.Size = size & 0xffffffffL;
                entry.CompressedSize = csize & 0xffffffffL;
                entry.Flags = bitFlags;
                entry.DosTime = dostime;
                entry.ZipFileIndex = (long)i;
                entry.Offset = offset;
                entry.ExternalFileAttributes = (int)externalAttributes;

                if (extraLen > 0)
                    entry.ExtraData = baseStream_.CheckedReadBytes(extraLen);

                entry.ProcessExtraData(false);

                if (commentLen > 0)
                {
                    baseStream_.CheckedReadBytes(buffer, 0, commentLen);
                    entry.Comment = ZipConstants.ConvertToStringExt(bitFlags, buffer, commentLen);
                }

                entries_[i] = entry;
            }
        }

        #endregion

        #region Instance Fields
        bool isDisposed_;
        string name_;
        string comment_;
        Stream baseStream_;
        bool isStreamOwner;
        long offsetOfFirstEntry;
        ZipEntry[] entries_;
        #endregion

        #region Support Classes

        /// <summary>
        /// An <see cref="IEnumerator">enumerator</see> for <see cref="ZipEntry">Zip entries</see>
        /// </summary>
        class ZipEntryEnumerator : IEnumerator
        {
            #region Constructors
            public ZipEntryEnumerator(ZipEntry[] entries)
            {
                array = entries;
            }

            #endregion
            #region IEnumerator Members
            public object Current
            {
                get
                {
                    return array[index];
                }
            }

            public void Reset()
            {
                index = -1;
            }

            public bool MoveNext()
            {
                return (++index < array.Length);
            }
            #endregion
            #region Instance Fields
            ZipEntry[] array;
            int index = -1;
            #endregion
        }
        #endregion
    }
}
