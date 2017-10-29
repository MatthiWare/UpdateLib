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
using MatthiWare.UpdateLib.Utils;
using NUnit.Framework;
using System;

namespace UpdateLib.Tests.Util
{
    [TestFixture]
    public class CmdLineParserTest
    {

        CmdLineParser cmd;

        [SetUp]
        public void Setup()
        {
            cmd = new CmdLineParser();
        }

        [Test]
        public void GoodArgsAreAllParsedCorrectly()
        {
            string[] args = {
                @"C:\Dev\TestApp.exe",
                "--silent",
                "--wait",
                "9999",
                "--update"
            };

            cmd.AddParameter("silent", ParamMandatoryType.Required, ParamValueType.Bool);
            cmd.AddParameter("wait", ParamMandatoryType.Required, ParamValueType.Int);
            cmd.AddParameter("update", ParamMandatoryType.Required, ParamValueType.Bool);

            cmd.Parse(args);

            Assert.IsNotNull(cmd["silent"]);
            Assert.IsNotNull(cmd[""]);
        }

        [Test]
        public void AddingFaultyParameterThrowsException()
        {
            Assert.Catch<ArgumentNullException>(() => cmd.AddParameter(null));
        }

        [TearDown]
        public void CleanUp()
        {
            cmd = null;
        }

    }
}
