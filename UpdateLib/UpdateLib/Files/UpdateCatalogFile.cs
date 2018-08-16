/*  Copyright
 *  
 *  UpdateLib - .Net auto update library <https://github.com/MatthiWare/UpdateLib>
 *  
 *  File: UpdateCatalogFile.cs v0.5
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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Common.Abstraction;
using MatthiWare.UpdateLib.Compression.GZip;

namespace MatthiWare.UpdateLib.Files
{
    [Serializable, Description("Update Catalog File")]
    public class UpdateCatalogFile : FileBase<UpdateCatalogFile>
    {
        public const string FILE_NAME = "catalogus.gz";

        /// <summary>
        /// Gets or sets the name of the application
        /// </summary>
        [XmlAttribute]
        public string ApplicationName { get; set; } = "UpdateLib";

        /// <summary>
        /// Gets the <see cref="UpdateInfo"/> Catalog
        /// </summary>
        public List<UpdateInfo> Catalog { get; private set; } = new List<UpdateInfo>();

        /// <summary>
        /// Download Url's 
        /// </summary>
        public List<string> DownloadUrls { get; private set; } = new List<string>();

        /// <summary>
        /// Gets the best update for the current version. 
        /// </summary>
        /// <param name="currentVersion">The currect application version</param>
        /// <returns><see cref="UpdateInfo"/></returns>
        public UpdateInfo GetLatestUpdateForVersion(UpdateVersion currentVersion)
        {
            if (currentVersion == null) throw new ArgumentNullException(nameof(currentVersion));

            return Catalog.OrderBy(c => c).Where(c => currentVersion < c.Version && ((c.IsPatch && c.BasedOnVersion == currentVersion) || !c.IsPatch)).FirstOrDefault();
        }

        public override UpdateCatalogFile Load() => throw new NotImplementedException();

        public override UpdateCatalogFile Load(Stream stream)
           => base.Load(new GZipInputStream(stream, false));

        public override void Save() => throw new NotImplementedException();

        public override void Save(Stream stream)
            => base.Save(new GZipOutputStream(stream, false));
    }
}
