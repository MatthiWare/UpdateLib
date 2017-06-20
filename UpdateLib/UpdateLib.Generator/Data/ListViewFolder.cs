using MatthiWare.UpdateLib.Generator.Data.FilesPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Generator.Data
{
    public class ListViewFolder : ListViewItem
    {
        internal const string FOLDER_KEY = "folderimagekey";

        public GenFolder Folder { get; set; }

        private ListViewFolder(string[] items, string imageKey)
            : base(items, imageKey)
        { }

        public ListViewFolder(string folderName, GenFolder folder)
            : this(new string[] { folderName, string.Empty, "Folder", string.Empty }, FOLDER_KEY)
        {
            Folder = folder;
        }
    }
}
