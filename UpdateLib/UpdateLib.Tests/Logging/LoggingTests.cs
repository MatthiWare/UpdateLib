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

using MatthiWare.UpdateLib.Logging;
using NUnit.Framework;
using Moq;
using MatthiWare.UpdateLib.Common;

namespace UpdateLib.Tests.Logging
{
    [TestFixture]
    public class LoggingTests
    {
        private Logger logger;

        [SetUp]
        public void Setup()
        {
            logger = new Logger();
        }

        [Test]
        public void ErrorLogLevelShouldNotLogWhenDebugLog()
        {
            logger.LogLevel = LoggingLevel.Error;
            Mock<ILogWriter> writer = SetUpWriter(LoggingLevel.Debug);

            logger.Writers.Add(writer.Object);

            logger.Debug(nameof(LoggingTests), nameof(ErrorLogLevelShouldNotLogWhenDebugLog), "This is my log msg");

            writer.Verify(mock => mock.Log(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void DebugLogLevelShouldLogErrorLog()
        {
            logger.LogLevel = LoggingLevel.Debug;
            Mock<ILogWriter> writer = SetUpWriter(LoggingLevel.Error);

            logger.Writers.Add(writer.Object);

            logger.Error(nameof(LoggingTests), nameof(DebugLogLevelShouldLogErrorLog), "This is my log msg");

            writer.Verify(mock => mock.Log(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ErrorLogLevelShouldNotLogAnyLowerLevel()
        {
            logger.LogLevel = LoggingLevel.Error;

            Mock<ILogWriter> info = SetUpWriter(LoggingLevel.Info);

            Mock<ILogWriter> warn = SetUpWriter(LoggingLevel.Warn);

            Mock<ILogWriter> debug = SetUpWriter(LoggingLevel.Debug);

            logger.Writers.Add(info.Object);
            logger.Writers.Add(warn.Object);
            logger.Writers.Add(debug.Object);

            logger.Error(string.Empty, string.Empty, string.Empty);
            logger.Warn(string.Empty, string.Empty, string.Empty);
            logger.Info(string.Empty, string.Empty, string.Empty);
            logger.Debug(string.Empty, string.Empty, string.Empty);

            info.Verify(mock => mock.Log(It.IsAny<string>()), Times.Never);
            warn.Verify(mock => mock.Log(It.IsAny<string>()), Times.Never);
            debug.Verify(mock => mock.Log(It.IsAny<string>()), Times.Never);
        }

        private Mock<ILogWriter> SetUpWriter(LoggingLevel level)
        {
            Mock<ILogWriter> writer = new Mock<ILogWriter>();
            writer.SetupGet(w => w.LoggingLevel).Returns(level);

            return writer;
        }

        [TearDown]
        public void CleanUp()
        {
            logger.Writers.Clear();
        }

    }
}
