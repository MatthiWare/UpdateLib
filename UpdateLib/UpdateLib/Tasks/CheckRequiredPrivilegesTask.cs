using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
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
            string localPath = Updater.Instance.Converter.Replace(dir.DestinationLocation);

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
