using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;

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
            {
                if (!CheckHasSufficientPermissionsForDirectory(dir))
                {
                    Result = true;
                    return;
                }
            }

            foreach (RegistryFolderEntry dir in File.Registry)
                if (!CheckHasSufficientPermissionForRegistry(dir))
                {
                    Result = true;
                    return;
                }
        }

        private bool CheckHasSufficientPermissionsForDirectory(DirectoryEntry dir)
        {
            string localPath = Updater.Instance.Converter.Replace(dir.DestinationLocation);

            if (!PermissionUtil.DirectoryHasPermission(localPath))
                return false;

            foreach (DirectoryEntry subDir in dir.Directories)
                if (!CheckHasSufficientPermissionsForDirectory(subDir))
                    return false;

            return true;
        }

        private bool CheckHasSufficientPermissionForRegistry(RegistryFolderEntry dir)
        {
            foreach (RegistryKeyEntry key in dir.Keys)
                if (!PermissionUtil.CheckRegPermission(key))
                    return false;

            foreach (RegistryFolderEntry subDir in dir.Folders)
                if (!CheckHasSufficientPermissionForRegistry(dir))
                    return false;

            return true;
        }
    }
}
