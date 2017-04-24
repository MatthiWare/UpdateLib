using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Generator.Data.FilesPage
{
    public class GenFolder
    {
        public string Name { get; set; }
        public List<GenFile> Files { get; private set; } = new List<GenFile>();
        public List<GenFolder> Directories { get; private set; } = new List<GenFolder>();
        public GenFolder ParentFolder { get; set; }
        public bool IsRoot { get { return ParentFolder == null; } }


        public ListViewItemFolder FolderListView { get; set; }
        public TreeViewFolderNode FolderTreeView { get; set; }


        public GenFolder(string name, GenFolder parent)
        {
            Name = name;
            ParentFolder = parent;
        }

    }
}
