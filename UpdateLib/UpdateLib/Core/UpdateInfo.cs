/*  Copyright
 *  
 *  UpdateLib - .Net auto update library <https://github.com/MatthiWare/UpdateLib>
 *  
 *  File: UpdateInfo.cs v0.5
 *  
 *  Author: Matthias Beerens
 *  
 *  Copyright (C) 2018 - MatthiWare
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
using System.Xml.Serialization;

namespace MatthiWare.UpdateLib.Common
{
    /// <summary>
    /// An entry for the <see cref="Files.UpdateCatalogFile"/>
    /// </summary>
    public class UpdateInfo : IComparable, IComparable<UpdateInfo>
    {
        /// <summary>
        /// Specifies the version this patch is based on. 
        /// This field can be null in case it is not a patch. 
        /// </summary>
        public UpdateVersion BasedOnVersion { get; set; }

        /// <summary>
        /// The version of the update/patch. 
        /// </summary>
        public UpdateVersion Version { get; set; }

        /// <summary>
        /// The file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The meta file name.
        /// </summary>
        public string MetaFileName { get; set; }

        /// <summary>
        /// The calculated hash for the update/patch.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// The calculated hash for the meta file.
        /// </summary>
        public string MetaHash { get; set; }

        /// <summary>
        /// Indicates if this update is a patch.
        /// </summary>
        public bool IsPatch => BasedOnVersion != null;

        public UpdateInfo() { }

        /// <summary>
        /// A new catalog entry
        /// </summary>
        /// <param name="version">The update version</param>
        /// <param name="basedOnVersion">The version this update is based on, can be null if it's not a patch.</param>
        /// <param name="fileName">The file name for the update.</param>
        /// <param name="hash">The calculated hash for the update</param>
        public UpdateInfo(UpdateVersion version, UpdateVersion basedOnVersion, string fileName, string hash)
        {
            Version = version ?? throw new ArgumentNullException(nameof(version));
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            Hash = hash ?? throw new ArgumentNullException(nameof(hash));

            BasedOnVersion = basedOnVersion;

            if (version <= basedOnVersion) throw new ArgumentOutOfRangeException(nameof(basedOnVersion), "The new version cannot be smaller than the version it was based on.");
        }

        public int CompareTo(UpdateInfo other)
        {
            if (other == null) return -1;

            if (Version > other.Version) return -1;

            if (Version == other.Version)
            {
                if (IsPatch && other.IsPatch) return BasedOnVersion.CompareTo(other.BasedOnVersion);

                if (IsPatch && !other.IsPatch) return -1;

                if (!IsPatch && other.IsPatch) return 1;

                return 0;
            }

            return 1;
        }

        public int CompareTo(object obj)
            => CompareTo(obj as UpdateInfo);
    }
}
