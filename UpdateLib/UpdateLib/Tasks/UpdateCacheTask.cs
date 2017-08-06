using MatthiWare.UpdateLib.Files;
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
            try
            {
                // first of lets load the file, (file might be corrupt..)
                Result = HashCacheFile.Load();
            }
            catch (Exception e)
            {
                Updater.Instance.Logger.Error(nameof(UpdateCacheTask), nameof(DoWork), e);
                Result = null;
            }

            DirectoryInfo dir = new DirectoryInfo(Updater.Instance.Converter.Convert("%appdir%"));
            IEnumerable<FileInfo> files = dir.GetFiles("*", SearchOption.AllDirectories).Where(f => !f.FullName.Contains(".old.tmp"));

            Updater.Instance.Logger.Debug(nameof(UpdateCacheTask), nameof(DoWork), $"found {files.Count()} files to recheck.");

            if (Result == null) // The file doesn't exist yet
            {
                Updater.Instance.Logger.Warn(nameof(UpdateCacheTask), nameof(DoWork), $"{nameof(HashCacheFile)} doesn't exist. Creating..");

                Result = new HashCacheFile();

                foreach (FileInfo f in files)
                {
                    try
                    {
                        Result.Items.Add(new HashCacheEntry(f.FullName));
                    }
                    catch (Exception ex) // file might no longer exist or is in use
                    {
                        Updater.Instance.Logger.Error(nameof(UpdateCacheTask), nameof(DoWork), ex);
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
                        Updater.Instance.Logger.Error(nameof(UpdateCacheTask), nameof(DoWork), ex);
                    }

                    continue;
                }

                // check to see if the file has been modified since last cache check
                entry.Recalculate();
            }

            Result.Save();
        }
    }
}
