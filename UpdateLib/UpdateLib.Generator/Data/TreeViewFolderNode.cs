using MatthiWare.UpdateLib.Generator.Data.FilesPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Generator.Data
{
    public class TreeViewFolderNode : TreeNode
    {
        internal const string FOLDER_KEY = "folderimagekey";

        public GenFolder Folder { get; set; }

        public TreeViewFolderNode(string folderName, GenFolder folder)
        {
            Text = folderName;
            ImageKey = FOLDER_KEY;
            SelectedImageKey = FOLDER_KEY;
            Folder = folder;
        }

    }
}
