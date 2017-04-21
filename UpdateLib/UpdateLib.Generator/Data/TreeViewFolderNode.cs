using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Generator.Data
{
    public class TreeViewFolderNode : TreeNode
    {
        private const string FOLDER_KEY = "folderimagekey";


        public TreeViewFolderNode(string folderName)
        {
            Name = folderName;
            ImageKey = FOLDER_KEY;
        }

    }
}
