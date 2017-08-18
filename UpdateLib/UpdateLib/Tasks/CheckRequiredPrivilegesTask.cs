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

using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Security;
using System.Linq;
using MatthiWare.UpdateLib.Utils;

namespace MatthiWare.UpdateLib.Tasks
{
    public class CheckRequiredPrivilegesTask : AsyncTask<bool>
    {

        public UpdateFile File { get; set; }

        public CheckRequiredPrivilegesTask(UpdateFile file)
        {
            File = file;
        }

        protected override void DoWork()
        {
            Result = false;

            if (PermissionUtil.IsProcessElevated)
                return;

            foreach (DirectoryEntry dir in File.Folders)
                if (!CheckHasSufficientPermissionsForDirectory(dir))
                {
                    Result = true;
                    return;
                }


            foreach (DirectoryEntry dir in File.Registry)
                if (!CheckHasSufficientPermissionForRegistry(dir))
                {
                    Result = true;
                    return;
                }

            Updater.Instance.Logger.Info(nameof(CheckRequiredPrivilegesTask), nameof(DoWork), $"Elavation required: {Result}");
        }

        private bool CheckHasSufficientPermissionsForDirectory(DirectoryEntry dir)
        {
            string localPath = Updater.Instance.Converter.Convert(dir.DestinationLocation);

            if (!PermissionUtil.DirectoryHasPermission(localPath))
                return false;

            foreach (DirectoryEntry subDir in dir.Directories)
                if (!CheckHasSufficientPermissionsForDirectory(subDir))
                    return false;

            return true;
        }

        private bool CheckHasSufficientPermissionForRegistry(DirectoryEntry dir)
        {
            foreach (RegistryKeyEntry key in dir.GetItems().Select(e => e as RegistryKeyEntry).NotNull())
                if (!PermissionUtil.CheckRegPermission(key))
                    return false;

            return true;
        }
    }
}
