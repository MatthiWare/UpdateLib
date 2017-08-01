using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
