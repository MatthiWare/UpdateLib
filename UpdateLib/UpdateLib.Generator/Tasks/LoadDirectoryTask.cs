using MatthiWare.UpdateLib.Tasks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UpdateLib.Generator.Tasks
{
    public class LoadDirectoryTask : AsyncTaskBase
    {
        public ListView ItemsListView { get; set; }
        public ImageList IconList { get; set; }
        public string DirectoryPath { get; set; }

        public LoadDirectoryTask(ListView lv, ImageList iconCache, string dirPath)
        {
            if (lv == null) throw new ArgumentNullException(nameof(lv));
            if (iconCache == null) throw new ArgumentNullException(nameof(iconCache));
            if (string.IsNullOrEmpty(dirPath)) throw new ArgumentNullException(nameof(dirPath));
            if (!Directory.Exists(dirPath)) throw new DirectoryNotFoundException($"The directory '{dirPath}' was not found.");

            ItemsListView = lv;
            IconList = iconCache;
            DirectoryPath = dirPath;
        }

        public override void DoWork()
        {
            DirectoryInfo dir = new DirectoryInfo(DirectoryPath);

            ItemsListView.BeginUpdate();

            ItemsListView.Clear();

            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                ListViewItem item = new ListViewItem(subDir.Name);
                item.Tag = subDir;
                item.ImageKey = "folder";

                ItemsListView.Items.Add(item);
            }

            foreach (FileInfo file in dir.GetFiles())
            {
                ListViewItem item = new ListViewItem(file.Name);
                item.Tag = file;

                if (!IconList.Images.ContainsKey(file.Extension))
                    IconList.Images.Add(file.Extension, Icon.ExtractAssociatedIcon(file.FullName));

                item.ImageKey = file.Extension;

                ItemsListView.Items.Add(item);
            }

            ItemsListView.EndUpdate();
        }
    }
}
