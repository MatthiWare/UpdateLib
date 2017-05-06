﻿using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MatthiWare.UpdateLib.Tasks
{
    public class UpdateCacheTask : AsyncTask<HashCacheFile>
    {
        protected override void DoWork()
        {
            // first of lets load the file
            Result = HashCacheFile.Load();

            DirectoryInfo dir = new DirectoryInfo(".");
            IEnumerable<FileInfo> files = dir.GetFiles("*", SearchOption.AllDirectories).Where(f => !f.FullName.Contains(".old.tmp"));


            Logger.Debug(GetType().Name, $"found {files.Count()} files to recheck.");

            if (Result == null) // The file doesn't exist yet
            {
                Logger.Warn(GetType().Name, $"{nameof(HashCacheFile)} doesn't exist. Creating..");

                Result = new HashCacheFile();

                foreach (FileInfo f in files)
                {
                    try
                    {
                        Result.Items.Add(new HashCacheEntry(f.FullName));
                    }
                    catch (Exception ex) // file might no longer exist or is in use
                    {
                        Logger.Error(GetType().Name, ex);
                    }
                }

                Result.Save();

                return;
            }

            foreach (FileInfo f in files)
            {
                HashCacheEntry entry = Result.Items.Find(match => match.FilePath == f.FullName);
                if (entry == null)
                {

                    try
                    {
                        Result.Items.Add(new HashCacheEntry(f.FullName));
                    }
                    catch (Exception ex) // file might no longer exist or is in use
                    {
                        Logger.Error(GetType().Name, ex);
                    }

                    continue;
                }

                // check to see if the file has been modified since last cache check
                entry.Recalculate(f.LastWriteTime.Ticks);
            }

            Result.Save();
        }
    }
}
