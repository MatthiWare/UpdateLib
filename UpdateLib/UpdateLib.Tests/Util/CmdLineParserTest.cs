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
using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Core;
using MatthiWare.UpdateLib.Core.Internal.CommandLine;
using MatthiWare.UpdateLib.Utils;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

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
            var optionsFactory = new OptionsWrapper<UpdateLibOptions>(
                new UpdateLibOptions
                {
                    ParameterPrefix = "--"
                });

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

            cmd.AddParameter<string>("silent", ParamMandatoryType.Required);
            cmd.AddParameter("wait", ParamMandatoryType.Required, ParamValueType.Required, new StringToIntArgumentResolver());
            cmd.AddParameter<string>("update", ParamMandatoryType.Required);
            cmd.AddParameter("text", ParamMandatoryType.Required, ParamValueType.Required, new StringToStringArgumentResolver(optionsFactory));
            cmd.AddParameter("ints", ParamMandatoryType.Required, ParamValueType.Required, new StringToMultipleIntsArgumentResolver());

            cmd.Parse(args);

            Assert.IsTrue(cmd.Get("silent")?.IsFound ?? false);
            Assert.IsTrue(cmd.Get("wait")?.IsFound ?? false);
            Assert.AreEqual(9999, cmd.Get("wait")?.Value ?? -1);
            Assert.IsTrue(cmd.Get("update")?.IsFound ?? false);
            Assert.IsTrue(cmd.Get("text")?.IsFound ?? false);
            Assert.AreEqual("this is my text message", cmd.Get("text")?.Value);
            Assert.IsTrue(cmd.Get("ints")?.IsFound ?? false);
            Assert.AreEqual(ints, cmd.Get("ints")?.Value);
        }

        [Test]
        public void OptionalArgumentIsNotMandatory()
        {
            //string[] args = {
            //    @"C:\Dev\TestApp.exe",
            //    "--silent"
            //};

            //int[] ints = { 5, 10, 15, 20 };

            //cmd.AddParameter("silent", ParamMandatoryType.Required, ParamValueType.None);
            //cmd.AddParameter("wait", ParamMandatoryType.Optional, ParamValueType.Int);

            //cmd.Parse(args);

            //Assert.IsTrue(cmd["silent"]?.IsFound);
            //Assert.IsFalse(cmd["wait"]?.IsFound);
        }

        [Test]
        public void TestOptionalParamValues()
        {
            //string[] args = {
            //    @"C:\Dev\TestApp.exe",
            //    "--test",
            //    "random",
            //    "--test2",
            //    "10"
            //};

            //cmd.AddParameter("test", ParamMandatoryType.Required, ParamValueType.OptionalInt);
            //cmd.AddParameter("test2", ParamMandatoryType.Required, ParamValueType.OptionalInt);

            //cmd.Parse(args);

            //Assert.IsTrue(cmd["test"].IsFound);
            //Assert.IsTrue(cmd["test2"].IsFound);
            //Assert.AreEqual(null, cmd["test"].Value);
            //Assert.AreEqual(10, cmd["test2"].Value);
        }

        [Test]
        public void TestDoubleParse()
        {
            //string[] args = {
            //    @"C:\Dev\TestApp.exe",
            //    "--test1",
            //    "10",
            //    "--test2"
            //};

            //cmd.AddParameter("test1", ParamMandatoryType.Required, ParamValueType.Int);
            //cmd.AddParameter("test2", ParamMandatoryType.Optional, ParamValueType.None);

            //cmd.Parse(args);

            //Assert.IsTrue(cmd["test1"].IsFound);
            //Assert.AreEqual(10, cmd["test1"].Value);
            //Assert.IsTrue(cmd["test2"].IsFound);

            //args[2] = "11";

            //Array.Resize(ref args, 3);

            //cmd.Parse(args);

            //Assert.IsTrue(cmd["test1"].IsFound);
            //Assert.AreEqual(11, cmd["test1"].Value);
            //Assert.IsFalse(cmd["test2"].IsFound);
        }

        [Test]
        public void AddingFaultyParameterThrowsException()
        {
            //cmd.ParameterPrefix = string.Empty;
            //Assert.Catch<ArgumentNullException>(() => cmd.AddParameter(null));
            //Assert.Catch<ArgumentException>(() => cmd.AddParameter("test 123"));
            //Assert.Catch<ArgumentNullException>(() => cmd.Parse());
        }

        [Test]
        public void AddDuplicateParameterThrowsException()
        {
            //cmd.AddParameter("silent", ParamMandatoryType.Required, ParamValueType.None);
            //Assert.Catch<ArgumentException>(() => cmd.AddParameter("silent", ParamMandatoryType.Required, ParamValueType.None));
        }

        [TearDown]
        public void CleanUp()
        {
            cmd = null;
        }

    }
}
