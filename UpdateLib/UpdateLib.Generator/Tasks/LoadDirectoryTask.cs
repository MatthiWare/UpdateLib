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

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using MatthiWare.UpdateLib.Tasks;
using MatthiWare.UpdateLib.UI;

namespace MatthiWare.UpdateLib.Generator.Tasks
{
    public class LoadDirectoryTask : AsyncTask
    {
        public ListView ItemsListView { get; set; }
        public ImageList IconList { get; set; }
        public DirectoryInfo DirectoryPath { get; set; }

        public LoadDirectoryTask(ListView lv, ImageList iconCache, DirectoryInfo dirPath)
        {
            ItemsListView = lv ?? throw new ArgumentNullException(nameof(lv));
            IconList = iconCache ?? throw new ArgumentNullException(nameof(iconCache));
            DirectoryPath = dirPath ?? throw new ArgumentNullException(nameof(dirPath));

            if (!DirectoryPath.Exists) throw new DirectoryNotFoundException($"The directory '{dirPath.FullName}' was not found.");
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
