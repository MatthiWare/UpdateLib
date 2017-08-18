/*  UpdateLib - .Net auto update library
 *  Copyright (C) 2016 - MatthiWare (Matthias Beerens)
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published
 *  by the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;
using System.Linq;
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
                return Items.Count + Directories.Sum(d => d.Count);
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
            folder.ParentFolder = this;
            Directories.Add(folder);
            FolderTreeView.Nodes.Add(folder.FolderTreeView);
        }

        public void Remove(IGenItem item)
        {
            Items.Remove(item);
            item.View.Remove();
        }

        public void Remove(GenFolder folder)
        {
            Directories.Remove(folder);
            FolderTreeView.Nodes.Remove(folder.FolderTreeView);
            folder.FolderListView.Remove();
        }

    }
}
