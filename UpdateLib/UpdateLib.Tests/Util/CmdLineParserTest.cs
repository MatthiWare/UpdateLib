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
                "--update",
                "--text",
                "this is my text message",
                "--ints",
                "5",
                "10",
                "15",
                "20"
            };

            int[] ints = { 5, 10, 15, 20 };

            cmd.AddParameter("silent", ParamMandatoryType.Required, ParamValueType.None);
            cmd.AddParameter("wait", ParamMandatoryType.Required, ParamValueType.Int);
            cmd.AddParameter("update", ParamMandatoryType.Required, ParamValueType.None);
            cmd.AddParameter("text", ParamMandatoryType.Required, ParamValueType.String);
            cmd.AddParameter("ints", ParamMandatoryType.Required, ParamValueType.MultipleInts);

            cmd.Parse(args);

            Assert.IsTrue(cmd["silent"]?.IsFound);
            Assert.IsTrue(cmd["wait"]?.IsFound);
            Assert.AreEqual(9999, cmd["wait"]?.Value);
            Assert.IsTrue(cmd["update"]?.IsFound);
            Assert.IsTrue(cmd["text"]?.IsFound);
            Assert.AreEqual("this is my text message", cmd["text"]?.Value);
            Assert.IsTrue(cmd["ints"]?.IsFound);
            Assert.AreEqual(ints, cmd["ints"]?.Value);
        }

        [Test]
        public void OptionalArgumentIsNotMandatory()
        {
            string[] args = {
                @"C:\Dev\TestApp.exe",
                "--silent"
            };

            int[] ints = { 5, 10, 15, 20 };

            cmd.AddParameter("silent", ParamMandatoryType.Required, ParamValueType.None);
            cmd.AddParameter("wait", ParamMandatoryType.Optional, ParamValueType.Int);

            cmd.Parse(args);

            Assert.IsTrue(cmd["silent"]?.IsFound);
            Assert.IsFalse(cmd["wait"]?.IsFound);
        }

        [Test]
        public void TestOptionalParamValues()
        {
            string[] args = {
                @"C:\Dev\TestApp.exe",
                "--test",
                "random",
                "--test2",
                "10"
            };

            cmd.AddParameter("test", ParamMandatoryType.Required, ParamValueType.OptionalInt);
            cmd.AddParameter("test2", ParamMandatoryType.Required, ParamValueType.OptionalInt);

            cmd.Parse(args);

            Assert.IsTrue(cmd["test"].IsFound);
            Assert.IsTrue(cmd["test2"].IsFound);
            Assert.AreEqual(null, cmd["test"].Value);
            Assert.AreEqual(10, cmd["test2"].Value);
        }

        [Test]
        public void AddingFaultyParameterThrowsException()
        {
            Assert.Catch<ArgumentNullException>(() => cmd.AddParameter(null));
        }

        [Test]
        public void AddDuplicateParameterThrowsException()
        {
            cmd.AddParameter("silent", ParamMandatoryType.Required, ParamValueType.None);
            Assert.Catch<ArgumentException>(() => cmd.AddParameter("silent", ParamMandatoryType.Required, ParamValueType.None));
        }

        [TearDown]
        public void CleanUp()
        {
            cmd = null;
        }

    }
}
