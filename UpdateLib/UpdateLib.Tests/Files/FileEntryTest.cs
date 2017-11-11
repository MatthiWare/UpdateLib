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
using NUnit.Framework;

namespace UpdateLib.Tests.Files
{
    [TestFixture]
   public class FileEntryTest
    {
        [Test]
        public void ShouldGiveCorrectSourceAndDestination()
        {
            DirectoryEntry root = new DirectoryEntry("%root%");
            DirectoryEntry subFolder = new DirectoryEntry("sub");
            FileEntry file = new FileEntry("myfile.txt");

            root.Add(subFolder);

            subFolder.Add(file);

            string outputSource = "sub/myfile.txt";
            string outputDest = "%root%\\sub\\myfile.txt";

            Assert.AreEqual(outputSource, file.SourceLocation);
            Assert.AreEqual(outputDest, file.DestinationLocation);
        }
    }
}
