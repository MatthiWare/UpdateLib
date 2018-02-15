/*  Copyright
 *  
 *  UpdateLib - .Net auto update library <https://github.com/MatthiWare/UpdateLib>
 *  
 *  File: CheckForUpdatedItemsTask.cs v0.5
 *  
 *  Author: Matthias Beerens
 *  
 *  Copyright (C) 2016 - MatthiWare
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
 *  along with this program.  If not, see <https://github.com/MatthiWare/UpdateLib/blob/master/LICENSE>.
 */

using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MatthiWare.UpdateLib.Tasks
{
    public class CheckForUpdatedItemsTask : AsyncTask<bool, CheckForUpdatedItemsTask>
    {
        private Files.UpdateInfo m_updateFile;
        private HashCacheFile m_cacheFile;

        public CheckForUpdatedItemsTask(Files.UpdateInfo update, HashCacheFile cache)
        {
            m_updateFile = update ?? throw new ArgumentNullException(nameof(update));
            m_cacheFile = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        protected override void DoWork()
        {
            foreach (DirectoryEntry dir in m_updateFile.Folders)
                Enqueue(new Action<DirectoryEntry>(CheckFiles), dir);

            foreach (DirectoryEntry dir in m_updateFile.Registry)
                Enqueue(new Action<DirectoryEntry>(CheckRegister), dir);

            AwaitWorkers();

            Result = m_updateFile.FileCount > 0 || m_updateFile.RegistryKeyCount > 0;
        }

        private void CheckFiles(DirectoryEntry dir)
        {
            dir.Items.RemoveAll(fe =>
            {
                string convertedPath = Updater.Instance.Converter.Convert(fe.DestinationLocation);
                HashCacheEntry cacheEntry = m_cacheFile.Items.Find(hash => hash.FilePath.Equals(convertedPath));

                if (cacheEntry == null)
                    return false;

                return (fe as FileEntry)?.Hash.Equals(cacheEntry.Hash) ?? true;
            });

            IEnumerable<DirectoryEntry> dirsToCheck = dir.Directories.Where(d => d.Count > 0);
            int left = dirsToCheck.Count();

            foreach (DirectoryEntry subDir in dir.Directories)
            {
                if (--left == 0)
                    CheckFiles(subDir);
                else
                    Enqueue(new Action<DirectoryEntry>(CheckFiles), subDir);
            }
        }

        private void CheckRegister(DirectoryEntry dir)
        {
            dir.Items.RemoveAll(entry =>
            {
                RegistryKeyEntry key = entry as RegistryKeyEntry;

                if (key == null)
                    return true;

                return RegistryHelper.IsSame(key);
            });

            IEnumerable<DirectoryEntry> dirsToCheck = dir.Directories.Where(d => d.Count > 0);
            int left = dirsToCheck.Count();

            foreach (DirectoryEntry subDir in dir.Directories)
            {
                if (--left == 0)
                    CheckRegister(subDir);
                else
                    Enqueue(new Action<DirectoryEntry>(CheckRegister), dir);
            }
        }
    }
}
