/*  Copyright
 *  
 *  UpdateLib - .Net auto update library <https://github.com/MatthiWare/UpdateLib>
 *  
 *  File: UpdateFile.cs v0.5
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
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using MatthiWare.UpdateLib.Utils;
using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Common.Abstraction;

namespace MatthiWare.UpdateLib.Files
{
    /// <summary>
    /// The UpdateFile 
    /// </summary>
    [Serializable]
    public class UpdateFile : FileBase<UpdateFile>
    {
        /// <summary>
        /// Gets or sets the name of the application
        /// </summary>
        [XmlAttribute]
        public string ApplicationName { get; set; } = "UpdateLib";

        public List<Uri> DownloadURIs { get; } = new List<Uri>();

        public List<UpdateInfo> Updates { get; } = new List<UpdateInfo>();

        public UpdateInfo GetLatestUpdate()
            => Updates.MaxOrDefault(u => u.Version);

        public override void Save()
            => throw new NotImplementedException();

        public override UpdateFile Load()
            => throw new NotImplementedException();
    }
}
