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

using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Files;
using System;

namespace MatthiWare.UpdateLib.Tasks
{
    public class UpdateInfoPostProcessorTask : AsyncTask
    {
        private UpdateInfo m_updateInfo;

        public UpdateInfoPostProcessorTask(UpdateInfo updateInfo)
        {
            m_updateInfo = updateInfo;
        }


        private void PostProcessDirectory(DirectoryEntry dir)
        {
            foreach (EntryBase file in dir.Items)
                file.Parent = dir;

            int left = dir.Directories.Count;
            foreach (DirectoryEntry subDir in dir.Directories)
            {
                subDir.Parent = dir;

                if (--left == 0)
                    PostProcessDirectory(subDir);
                else
                    Enqueue(new Action<DirectoryEntry>(PostProcessDirectory), subDir);
            }
        }

        protected override void DoWork()
        {
            foreach (DirectoryEntry dir in m_updateInfo.Folders)
                PostProcessDirectory(dir);

            foreach (DirectoryEntry dir in m_updateInfo.Registry)
                PostProcessDirectory(dir);

            AwaitWorkers();
        }
    }
}
