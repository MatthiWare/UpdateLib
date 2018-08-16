/*  Copyright
 *  
 *  UpdateLib - .Net auto update library <https://github.com/MatthiWare/UpdateLib>
 *  
 *  File: UpdateCacheTask.cs v0.5
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
using System.IO;
using System.Linq;

namespace MatthiWare.UpdateLib.Tasks
{
    public class UpdateCacheTask : AsyncTask<HashCacheFile, UpdateCacheTask>
    {
        private Dictionary<HashCacheEntry, bool> existingEntries = null;
        private IEnumerable<System.IO.FileInfo> files = null;

        protected override void DoWork()
        {
            existingEntries = new Dictionary<HashCacheEntry, bool>();

            try
            {
                // first of lets load the file, (file might be corrupt..)
                Result = FileManager.LoadFile<HashCacheFile>();

                Result.Items.ForEach(e => existingEntries.Add(e, false));
            }
            catch (Exception e)
            {
                Updater.Instance.Logger.Error(nameof(UpdateCacheTask), nameof(DoWork), e);
                Result = null;
            }

            var dir = new DirectoryInfo(Updater.Instance.Converter.Convert("%appdir%"));
            files = dir.GetFiles("*", SearchOption.AllDirectories).Where(f => !f.FullName.Contains(".old.tmp"));

            Updater.Instance.Logger.Debug(nameof(UpdateCacheTask), nameof(DoWork), $"found {files.Count()} files to recheck.");

            if (Result == null) // The file doesn't exist yet
            {
                Result = CreateNewHashCacheFile();

                return;
            }

            CheckFiles();

            existingEntries.Where(e => e.Value == false).ForEach(e => Result.Items.Remove(e.Key));

            Result.Save();
        }

        private void CheckFiles()
        {
            foreach (System.IO.FileInfo f in files)
            {
                HashCacheEntry entry = base.Result.Items.Find(match => match.FilePath == f.FullName);
                if (entry == null)
                {
                    try
                    {
                        base.Result.Items.Add(new HashCacheEntry(f.FullName));
                    }
                    catch (Exception ex) // file might no longer exist or is in use
                    {
                        Updater.Instance.Logger.Error(nameof(UpdateCacheTask), nameof(DoWork), ex);
                    }

                    continue;
                }

                existingEntries[entry] = true;

                // check to see if the file has been modified since last cache check
                entry.Recalculate();
            }
        }

        private HashCacheFile CreateNewHashCacheFile()
        {
            Updater.Instance.Logger.Warn(nameof(UpdateCacheTask), nameof(DoWork), $"{nameof(HashCacheFile)} doesn't exist. Creating..");

            var result = new HashCacheFile();

            foreach (System.IO.FileInfo f in files)
            {
                try
                {
                    result.Items.Add(new HashCacheEntry(f.FullName));
                }
                catch (Exception ex) // file might no longer exist or is in use
                {
                    Updater.Instance.Logger.Error(nameof(UpdateCacheTask), nameof(DoWork), ex);
                }
            }

            result.Save();

            return result;
        }
    }
}
