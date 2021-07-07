﻿/*  Copyright
 *  
 *  UpdateLib - .Net auto update library <https://github.com/MatthiWare/UpdateLib>
 *  
 *  File: FileManager.cs v0.5
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

using System.IO;
using MatthiWare.UpdateLib.Common.Abstraction;

namespace MatthiWare.UpdateLib.Common
{
    public static class FileManager
    {
        public static T LoadFile<T>() where T : FileBase<T>, new()
            => new T().Load();

        public static T LoadFile<T>(string path) where T : FileBase<T>, new()
            => new T().Load(path);

        public static T LoadFile<T>(Stream stream) where T : FileBase<T>, new()
            => new T().Load(stream);
    }
}
