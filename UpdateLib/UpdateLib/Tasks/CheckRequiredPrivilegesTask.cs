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
            {
                Result = true;
                return;
            }

            foreach (DirectoryEntry dir in File.Folders)
            {
                if (!CheckHasSufficientPermissionsForDirectory(dir))
                {
                    Result = true;
                    return;
                }
            }


            // TODO add more checks for registry for example
        }

        private bool CheckHasSufficientPermissionsForDirectory(DirectoryEntry dir)
        {
            string localPath = Updater.Instance.Converter.Replace(dir.DestinationLocation);

            if (!PermissionUtil.DirectoryHasPermission(localPath, (FileSystemRights.Modify)))
                return false;

            foreach (DirectoryEntry subDir in dir.Directories)
            {
                if (!CheckHasSufficientPermissionsForDirectory(subDir))
                    return false;
            }

            return true;
        }
    }
}
