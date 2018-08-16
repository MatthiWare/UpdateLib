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
using System.IO;
using System.Text;

using MatthiWare.UpdateLib.Common;

namespace MatthiWare.UpdateLib.Utils
{
    public static class IOUtils
    {
        private static Lazy<string> m_getAppDataPath = new Lazy<string>(() => "");
        private static Lazy<string> m_getCachePath = new Lazy<string>(() => $"");
        private static Lazy<string> m_getLogPath = new Lazy<string>(() => $"");
        private static Lazy<string> m_getTempPath = new Lazy<string>(() => $"");


        public static string AppDataPath => m_getAppDataPath.Value;
        public static string CachePath => m_getCachePath.Value;
        public static string LogPath => m_getLogPath.Value;
        public static string TempPath => m_getTempPath.Value;

        internal static string GetRemoteBasePath(string url)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));

            const char slash = '/';
            const char backslash = '\\';

            StringBuilder builder = new StringBuilder();

            foreach (var s in url.Split(slash, backslash).SkipLast(1))
            {
                builder.Append(s);
                builder.Append(slash);
            }

            return builder.ToString();
        }

        private static string GetPathPrefix()
        {
            switch (Updater.Instance.InstallationMode)
            {
                case InstallationMode.Local:
                    return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                case InstallationMode.Shared:
                default:
                    return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }
        }

        internal static byte[] CheckedReadBytes(this Stream stream, int size)
        {
            byte[] ret = new byte[size];
            int index = 0;

            while (index < size)
            {
                int read = stream.Read(ret, index, size - index);

                if (read == 0)
                    throw new EndOfStreamException();

                index += read;
            }

            return ret;
        }

        internal static void CheckedReadBytes(this Stream stream, byte[] buffer, int offset, int length)
        {
            int index = offset;
            int size = offset + length;

            while (index < size)
            {
                int read = stream.Read(buffer, index, size);

                if (read == 0)
                    throw new EndOfStreamException();

                index += read;
            }
        }

        internal static byte CheckedReadByte(this Stream stream)
        {
            int ret = stream.ReadByte();

            if (ret == -1)
                throw new IOException("Unable to read byte from stream");

            return (byte)ret;
        }

        internal static int ReadBigEndian7BitEncodedInt(this Stream stream)
        {
            int ret = 0;

            for (int i = 0; i < 5; i++)
            {
                int b = stream.ReadByte();
                if (b == -1)
                    throw new EndOfStreamException();

                ret = (ret << 7) | (b & 0x7f);
                if ((b & 0x80) == 0)
                    return ret;
            }

            throw new IOException("Invalid 7-bit encoded integer in stream");
        }

        internal static void Copy(Stream source, Stream dest, byte[] buffer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (dest == null) throw new ArgumentNullException(nameof(dest));
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));

            bool copying = true;

            while (copying)
            {
                int bytesRead = source.Read(buffer, 0, buffer.Length);
                copying = bytesRead > 0;

                if (copying)
                    dest.Write(buffer, 0, bytesRead);
                else
                    dest.Flush();
            }
        }
    }
}
