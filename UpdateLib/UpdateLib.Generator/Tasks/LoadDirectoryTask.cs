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
            if (ItemsListView.InvokeRequired)
            {
                ItemsListView.Invoke(new Action<int>(SetColumnAutoSize), clmn);
                return;
            }

            ItemsListView.Columns[clmn].Width = -1;
        }

        private void EndUpdate()
        {
            if (ItemsListView.InvokeRequired)
            {
                ItemsListView.Invoke(new Action(EndUpdate));
                return;
            }

            ItemsListView.EndUpdate();
        }

        private void BeginUpdate()
        {
            if (ItemsListView.InvokeRequired)
            {
                ItemsListView.Invoke(new Action(BeginUpdate));
                return;
            }

            ItemsListView.BeginUpdate();
        }

        private void Clear()
        {
            if (ItemsListView.InvokeRequired)
            {
                ItemsListView.Invoke(new Action(Clear));
                return;
            }

            ItemsListView.Items.Clear();
        }

        private void AddItem(ListViewItem item)
        {
            if (ItemsListView.InvokeRequired)
            {
                ItemsListView.Invoke(new Action<ListViewItem>(AddItem), item);
                return;
            }

            ItemsListView.Items.Add(item);
        }
    }
}
