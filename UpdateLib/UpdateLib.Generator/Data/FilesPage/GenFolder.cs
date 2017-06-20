using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Generator.Data.FilesPage
{
    public class GenFolder
    {
        public string Name { get; set; }
        public string PathVariable { get; set; }
        public List<IGenItem> Items { get; private set; } = new List<IGenItem>();
        public List<GenFolder> Directories { get; private set; } = new List<GenFolder>();
        public GenFolder ParentFolder { get; set; }
        public bool IsRoot { get { return ParentFolder == null; } }
        public bool ProtectedFolder { get; set; } = false;

        public ListViewFolder FolderListView { get; set; }
        public TreeViewFolderNode FolderTreeView { get; set; }

        public int Count
        {
            get
            {
                return Directories.Sum(d => d.Count);
            }
        }

        public GenFolder(string name, ContextMenuStrip menu)
        {
            Name = name;

            FolderListView = new ListViewFolder(name, this);
            FolderTreeView = new TreeViewFolderNode(name, this);

            FolderTreeView.ContextMenuStrip = menu;

        }

        public void Add(IGenItem item)
        {
            item.Parent = this;
            Items.Add(item);
        }

        public void Add(GenFolder folder)
        {
            folder.ParentFolder = folder;
            Directories.Add(folder);
            FolderTreeView.Nodes.Add(folder.FolderTreeView);
        }

        public void Remove(IGenItem item)
        {
            Items.Remove(item);
        }

        public void Remove(GenFolder folder)
        {
            Directories.Remove(folder);
            FolderTreeView.Nodes.Remove(folder.FolderTreeView);
        }

    }
}
