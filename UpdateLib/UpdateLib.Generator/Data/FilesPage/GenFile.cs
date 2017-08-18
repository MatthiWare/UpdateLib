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

namespace MatthiWare.UpdateLib.Generator.Data.FilesPage
{
    public class GenFile : IGenItem
    {
        public event EventHandler Changed;

        private FileInfo m_fileInfo;
        public FileInfo FileInfo
        {
            get { return m_fileInfo; }
            set { m_fileInfo = value; Changed?.Invoke(this, EventArgs.Empty); }
        }

        public string Name { get { return FileInfo?.Name ?? string.Empty; } set { Changed?.Invoke(this, EventArgs.Empty); } }
        public string RealPath { get { return FileInfo?.FullName ?? string.Empty; } }
        public string Extension { get { return FileInfo?.Extension ?? string.Empty; } }
        public string Size { get { return ConvertBytesToSizeString(FileInfo?.Length ?? 0); } }

        public GenFolder Parent { get; set; }

        public ListViewGenItem View { get; set; }

        public GenFile(FileInfo file)
        {
            FileInfo = file;

            View = new ListViewGenItem(this);
        }

        private static string ConvertBytesToSizeString(long size)
        {
            size = Math.Max(0, size);

            double kb = Math.Ceiling(size / 1024.0);

            return $"{kb.ToString("N0")} kB";
        }

        public string[] GetListViewItems()
        {
            return new string[] { Name, "File", FileInfo.LastWriteTime.ToString(), Size };
        }
        public string GetListViewImageKey()
        {
            return Extension;
        }
    }
}
