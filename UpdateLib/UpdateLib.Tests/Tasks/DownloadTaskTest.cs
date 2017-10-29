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

using MatthiWare.UpdateLib;
using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Files;
using System.IO;

namespace UpdateLib.Tests.Tasks
{
    //[TestFixture]
    public class DownloadTaskTest
    {

      //  [OneTimeSetUp]
        public void Before()
        {
            var path = new DirectoryInfo(".").FullName;

            Updater.Instance
                .ConfigureUpdateUrl($@"file:///{path}")
                .ConfigureInstallationMode(InstallationMode.Local)
                .ConfigureUnsafeConnections(true)
                .Initialize();
        }

       // [Test]
        public void TestDownload()
        {
            FileEntry fe = MakeFileEntry();

            var s = fe.DestinationLocation;
        }

        private FileEntry MakeFileEntry()
        {
            DirectoryEntry appDir = new DirectoryEntry("%appdir%");
            FileEntry file = new FileEntry("testFile");


            appDir.Add(file);

            return file;
        }

    }
}
