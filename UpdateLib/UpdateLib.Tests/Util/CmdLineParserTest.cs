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
        OptionsWrapper<UpdateLibOptions> optionsFactory = new OptionsWrapper<UpdateLibOptions>(
                new UpdateLibOptions
                {
                    CommandLineArgumentPrefix = "--"
                });

        CmdLineParser cmd;

        [SetUp]
        public void Setup()
        {
            cmd = new CmdLineParser(optionsFactory);
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

            cmd.AddParameter("silent", ParamMandatoryType.Required);
            cmd.AddParameter("wait", ParamMandatoryType.Required, ParamValueType.Required, new IntArgumentResolver());
            cmd.AddParameter("update", ParamMandatoryType.Required);
            cmd.AddParameter("text", ParamMandatoryType.Required, ParamValueType.Required, new StringArgumentResolver(optionsFactory));
            cmd.AddParameter("ints", ParamMandatoryType.Required, ParamValueType.Required, new MultipleIntArgumentResolver());

            cmd.Parse(args);

            Assert.IsTrue(cmd.Get("silent")?.IsFound ?? false);
            Assert.IsTrue(cmd.Get("wait")?.IsFound ?? false);
            Assert.AreEqual(9999, cmd.Get<int>("wait")?.Value ?? -1);
            Assert.IsTrue(cmd.Get("update")?.IsFound ?? false);
            Assert.IsTrue(cmd.Get("text")?.IsFound ?? false);
            Assert.AreEqual("this is my text message", cmd.Get<string>("text")?.Value);
            Assert.IsTrue(cmd.Get("ints")?.IsFound ?? false);
            Assert.AreEqual(ints, cmd.Get<int[]>("ints")?.Value);
        }

        [Test]
        public void OptionalArgumentIsNotMandatory()
        {
            string[] args = {
                @"C:\Dev\TestApp.exe",
                "--silent"
            };

            cmd.AddParameter("wait");

            cmd.Parse(args);

            Assert.IsFalse(cmd.Get("wait").IsFound);
        }

        [Test]
        public void OptionalValueTypeTest()
        {
            string[] args = {
                @"C:\Dev\TestApp.exe",
                "--wait",
                "--otherParam"
            };

            cmd.AddParameter("wait", ParamMandatoryType.Required, ParamValueType.Optional, new IntArgumentResolver());
            cmd.AddParameter("otherParam", ParamMandatoryType.Required, ParamValueType.Optional, new StringArgumentResolver(optionsFactory));

            cmd.Parse(args);

            Assert.IsTrue(cmd.Get("wait").IsFound);
            Assert.AreEqual(default(int), cmd.Get<int>("wait").Value);

            Assert.IsTrue(cmd.Get("otherParam").IsFound);
            Assert.AreEqual(default(string), cmd.Get<string>("otherParam").Value);
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

            cmd.AddParameter("test", ParamMandatoryType.Required, ParamValueType.Optional, new StringArgumentResolver(optionsFactory));
            cmd.AddParameter("test2", ParamMandatoryType.Required, ParamValueType.Optional, new IntArgumentResolver());

            cmd.Parse(args);

            Assert.IsTrue(cmd.Get("test").IsFound);
            Assert.IsTrue(cmd.Get("test2").IsFound);
            Assert.AreEqual("random", cmd.Get<string>("test").Value);
            Assert.AreEqual(10, cmd.Get<int>("test2").Value);
        }

        [Test]
        public void TestDoubleParse()
        {
            string[] args = {
                @"C:\Dev\TestApp.exe",
                "--test1",
                "10",
                "--test2"
            };

            cmd.AddParameter("test1", ParamMandatoryType.Required, ParamValueType.Required, new IntArgumentResolver());
            cmd.AddParameter("test2", ParamMandatoryType.Optional, ParamValueType.None);

            cmd.Parse(args);

            Assert.IsTrue(cmd.Get("test1").IsFound);
            Assert.AreEqual(10, cmd.Get<int>("test1").Value);
            Assert.IsTrue(cmd.Get("test2").IsFound);

            args[2] = "11";

            Array.Resize(ref args, 3);

            cmd.Parse(args);

            Assert.IsTrue(cmd.Get("test1").IsFound);
            Assert.AreEqual(11, cmd.Get<int>("test1").Value);
            Assert.IsFalse(cmd.Get("test2").IsFound);
        }

        [Test]
        public void AddingFaultyParameterThrowsException()
        {
            Assert.Catch<ArgumentNullException>(() => cmd.AddParameter(null));
            Assert.Catch<ArgumentNullException>(() => cmd.AddParameter(""));
            Assert.Catch<ArgumentException>(() => cmd.AddParameter("test 123"));
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
