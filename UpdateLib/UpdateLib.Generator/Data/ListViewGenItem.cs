using MatthiWare.UpdateLib.Generator.Data.FilesPage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Generator.Data
{
    public class ListViewGenItem : ListViewItem
    {
        public IGenItem Item { get; set; }

        public ListViewGenItem(IGenItem item)
            : base(item.GetListViewItems(), item.GetListViewImageKey())
        {
            Item = item;
        }
    }
}
