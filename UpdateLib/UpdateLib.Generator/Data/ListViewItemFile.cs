using MatthiWare.UpdateLib.Generator.Data.FilesPage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Generator.Data
{
    public class ListViewItemFile : ListViewItem
    {
        public GenFile File { get; set; }

        private ListViewItemFile(string[] items, string imageKey)
            : base(items, imageKey)
        { }

        public ListViewItemFile(FileInfo file)
            : this(new string[] { file.Name, file.LastWriteTime.ToString(), "File", ConvertBytesToSizeString(file.Length)}, file.Extension)
        {
        }

        private static string ConvertBytesToSizeString(long size)
        {
            size = Math.Max(0, size);

            double kb = Math.Ceiling(size / 1024.0);

            return $"{kb.ToString("N0")} kB";
        }
    }
}
