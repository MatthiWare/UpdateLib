using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Generator.Data.FilesPage
{
    public class GenFile : IGenItem
    {
        public FileInfo FileInfo { get; set; }

        public string Name { get { return FileInfo?.Name ?? string.Empty; } set { } }
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
            return new string[] { Name, FileInfo.LastWriteTime.ToString(), "File", Size };
        }
        public string GetListViewImageKey()
        {
            return Extension;
        }
    }
}
