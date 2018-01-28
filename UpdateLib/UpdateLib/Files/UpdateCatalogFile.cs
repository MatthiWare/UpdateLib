/*  Copyright
 *  
 *  UpdateLib - .Net auto update library <https://github.com/MatthiWare/UpdateLib>
 *  
 *  File: ServerFile.cs v0.5
 *  
 *  Author: Matthias Beerens
 *  
 *  Copyright (C) 2017 - MatthiWare
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
using System.Collections.Generic;
using System.Linq;

using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Common.Abstraction;
using MatthiWare.UpdateLib.Utils;

namespace MatthiWare.UpdateLib.Files
{
    [Serializable]
    public class UpdateCatalogFile : FileBase<UpdateCatalogFile>
    {
        public List<CatalogEntry> Catalog { get; private set; } = new List<CatalogEntry>();

        /// <summary>
        /// Download Url's 
        /// </summary>
        public List<string> DownloadUrls { get; private set; } = new List<string>();

        /// <summary>
        /// Tries to get the best update for the current version. 
        /// </summary>
        /// <param name="currentVersion"></param>
        /// <param name="entry"></param>
        /// <returns>True if an update available, false if none available.</returns>
        public bool TryGetLatestUpdateForVersion(UpdateVersion currentVersion, out CatalogEntry entry)
        {
            if (currentVersion == null) throw new ArgumentNullException(nameof(currentVersion));

            entry = Catalog.OrderBy(c => c).Where(c => currentVersion < c.Version && ((c.IsPatch && c.BasedOnVersion == currentVersion) || !c.IsPatch)).FirstOrDefault();

            return entry != null;
        }

        public override UpdateCatalogFile Load() => Load($"{IOUtils.AppDataPath}\\catalogus.xml");

        public override void Save() => throw new NotImplementedException();
    }
}
