using MatthiWare.UpdateLib.Tasks;
using MatthiWare.UpdateLib.UI;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Generator.Tasks
{
    public class LoadDirectoryTask : AsyncTask
    {
        public ListView ItemsListView { get; set; }
        public ImageList IconList { get; set; }
        public DirectoryInfo DirectoryPath { get; set; }

        public LoadDirectoryTask(ListView lv, ImageList iconCache, DirectoryInfo dirPath)
        {
            if (lv == null) throw new ArgumentNullException(nameof(lv));
            if (iconCache == null) throw new ArgumentNullException(nameof(iconCache));
            if (dirPath == null) throw new ArgumentNullException(nameof(dirPath));
            if (!dirPath.Exists) throw new DirectoryNotFoundException($"The directory '{dirPath.FullName}' was not found.");

            ItemsListView = lv;
            IconList = iconCache;
            DirectoryPath = dirPath;
        }

        protected override void DoWork()
        {
            BeginUpdate();

            Clear();

            foreach (DirectoryInfo subDir in DirectoryPath.GetDirectories())
            {
                ListViewItem item = new ListViewItem(new string[] { subDir.Name, subDir.LastWriteTimeUtc.ToString(), subDir.FullName });
                item.Tag = subDir;
                item.ImageKey = "folder";

                AddItem(item);
            }

            foreach (FileInfo file in DirectoryPath.GetFiles())
            {
                ListViewItem item = new ListViewItem(new string[] { file.Name, file.LastWriteTimeUtc.ToString(), file.FullName });
                item.Tag = file;

                if (!IconList.Images.ContainsKey(file.Extension))
                    IconList.Images.Add(file.Extension, Icon.ExtractAssociatedIcon(file.FullName));

                item.ImageKey = file.Extension;

                AddItem(item);
            }

            SetColumnAutoSize(0);
            SetColumnAutoSize(1);
            SetColumnAutoSize(2);

            EndUpdate();
            
        }

        private void SetColumnAutoSize(int clmn)
        {
            ItemsListView.InvokeOnUI(() => ItemsListView.Columns[clmn].Width = -1);
        }

        private void EndUpdate()
        {
            ItemsListView.InvokeOnUI(() => ItemsListView.EndUpdate());
        }

        private void BeginUpdate()
        {
            ItemsListView.InvokeOnUI(() => ItemsListView.BeginUpdate());
        }

        private void Clear()
        {
            ItemsListView.InvokeOnUI(() => ItemsListView.Items.Clear());
        }

        private void AddItem(ListViewItem item)
        {
            ItemsListView.InvokeOnUI(() => ItemsListView.Items.Add(item));
        }
    }
}
