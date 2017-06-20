using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Generator.Data.FilesPage
{
    public interface IGenItem
    {

        string Name { get; set; }
        GenFolder Parent { get; set; }
        ListViewGenItem View { get; set; }

        string[] GetListViewItems();
        string GetListViewImageKey();

    }
}
