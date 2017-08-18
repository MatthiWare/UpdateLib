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

using MatthiWare.UpdateLib.Generator.Data.FilesPage;
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
            Item.Changed += Item_Changed;
        }

        private void Item_Changed(object sender, EventArgs e)
        {
            string[] items = Item.GetListViewItems();

            for (int i = 0; i < items.Length; i++)
                SubItems[i].Text = items[i];

            ImageKey = Item.GetListViewImageKey();
        }
    }
}
