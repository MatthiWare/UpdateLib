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
    public class ListViewFolder : ListViewItem
    {
        internal const string FOLDER_KEY = "folderimagekey";

        public GenFolder Folder { get; set; }

        private ListViewFolder(string[] items, string imageKey)
            : base(items, imageKey)
        { }

        public ListViewFolder(string folderName, GenFolder folder)
            : this(new string[] { folderName, "Folder", string.Empty, string.Empty }, FOLDER_KEY)
        {
            Folder = folder;
        }
    }
}
