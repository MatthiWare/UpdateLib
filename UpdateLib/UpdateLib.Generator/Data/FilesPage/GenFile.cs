using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Generator.Data.FilesPage
{
    public class GenFile
    {
        private FileInfo m_file;

        public string Name { get { return m_file.Name; } }
        public string RealPath { get { return m_file.FullName; } }
        public string Extension { get { return m_file.Extension; } }
        public string Size { get { return ConvertBytesToSizeString(m_file.Length); } }

        public GenFolder ParentFolder { get; set; }

        public ListViewItemFile FileListView { get; set; }

        public GenFile(FileInfo file)
        {
            m_file = file;

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
