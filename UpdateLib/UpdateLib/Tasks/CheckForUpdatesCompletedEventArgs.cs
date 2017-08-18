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

using System;
using System.ComponentModel;

namespace MatthiWare.UpdateLib.Tasks
{
    public class CheckForUpdatesCompletedEventArgs : AsyncCompletedEventArgs
    {
        public string LatestVersion { get; set; }
        public bool UpdateAvailable { get; set; }

        public CheckForUpdatesCompletedEventArgs(CheckForUpdatesTask.CheckForUpdatesResult result, Exception error, bool cancelled, object userState) 
            : base(error, cancelled, userState)
        {
            LatestVersion = result.Version;
            UpdateAvailable = result.UpdateAvailable;
        }

        public CheckForUpdatesCompletedEventArgs(CheckForUpdatesTask.CheckForUpdatesResult result, AsyncCompletedEventArgs e)
              : this(result, e.Error, e.Cancelled, e.UserState)
        {

        }
    }
}
