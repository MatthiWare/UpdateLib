using MatthiWare.UpdateLib.Generator.Data.FilesPage;
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

using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Generator.Data
{
    public class TreeViewFolderNode : TreeNode
    {
        internal const string FOLDER_KEY = "folderimagekey";

        public GenFolder Folder { get; set; }

        public TreeViewFolderNode(string folderName, GenFolder folder, string imageKey = FOLDER_KEY)
        {
            Text = folderName;
            ImageKey = imageKey;
            SelectedImageKey = imageKey;
            Folder = folder;
        }

    }
}
