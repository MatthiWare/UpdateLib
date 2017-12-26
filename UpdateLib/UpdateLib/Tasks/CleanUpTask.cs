/*  Copyright
 *  
 *  UpdateLib - .Net auto update library <https://github.com/MatthiWare/UpdateLib>
 *  
 *  File: CleanUpTask.cs v0.5
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

using System;
using System.IO;

namespace MatthiWare.UpdateLib.Tasks
{
    public class CleanUpTask : AsyncTask<object, CleanUpTask>
    {
        public string PathToClean { get; set; }
        public string SearchPattern { get; set; }
        public bool IncludeSubDirectories { get; set; }

        public CleanUpTask(string pathToCleanUp, string searchPattern = "*.old.tmp", bool includeSubDirs = true)
        {
            PathToClean = Updater.Instance.Converter.Convert(pathToCleanUp);
            SearchPattern = searchPattern;
            IncludeSubDirectories = includeSubDirs;
        }
        protected override void DoWork()
        {
            DirectoryInfo dir = new DirectoryInfo(PathToClean);
            FileInfo[] files = dir.GetFiles(SearchPattern, IncludeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            foreach (FileInfo file in files)
            {
                try
                {
                    file.Delete();
                }
                catch (Exception e)
                {
                    Updater.Instance.Logger.Error(nameof(CleanUpTask), nameof(DoWork), e);
                }
            }
        }
    }
}
