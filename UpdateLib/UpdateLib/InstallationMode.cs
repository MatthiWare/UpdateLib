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

namespace MatthiWare.UpdateLib
{
    /// <summary>
    /// Indicates the how the underlaying application is installed. 
    /// </summary>
    public enum InstallationMode
    {
        /// <summary>
        /// Shared installation we will use the roaming folder
        /// </summary>
        Shared = 0,

        /// <summary>
        /// Single user installation we will use the local folder
        /// </summary>
        Local = 1
    }
}
