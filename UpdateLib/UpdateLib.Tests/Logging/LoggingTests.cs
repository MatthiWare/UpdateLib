using MatthiWare.UpdateLib.Logging;
using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatthiWare.UpdateLib.Logging.Writers;

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
