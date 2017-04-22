using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Generator.Data
{
    public class ListViewItemFolder : ListViewItem
    {
        internal const string FOLDER_KEY = "folderimagekey";

        private ListViewItemFolder(string[] items, string imageKey)
            : base(items, imageKey)
        { }

        public ListViewItemFolder(string folderName)
            : this(new string[] { "", folderName, "", "Folder", "" }, FOLDER_KEY)
        {
        }
    }
}
