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

        public TreeViewFolderNode(string folderName, GenFolder folder, string imageKey = FOLDER_KEY)
        {
            Text = folderName;
            ImageKey = imageKey;
            SelectedImageKey = imageKey;
            Folder = folder;
        }

    }
}
