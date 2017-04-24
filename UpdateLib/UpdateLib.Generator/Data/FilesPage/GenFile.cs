using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Generator.Data.FilesPage
{
    public class GenFile
    {
        public FileInfo FileInfo { get; set; }

        public string Name { get { return FileInfo.Name; } }
        public string RealPath { get { return FileInfo.FullName; } }
        public string Extension { get { return FileInfo.Extension; } }
        public string Size { get { return ConvertBytesToSizeString(FileInfo.Length); } }

        public GenFolder ParentFolder { get; set; }

        public ListViewItemFile FileListView { get; set; }

        public GenFile(FileInfo file)
        {
            FileInfo = file;

            FileListView = new ListViewItemFile(file);
        }

        private static string ConvertBytesToSizeString(long size)
        {
            size = Math.Max(0, size);

            double kb = Math.Ceiling(size / 1024.0);

            return $"{kb.ToString("N0")} kB";
        }
    }
}
