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


        [Test]
        public void ErrorLogLevelShouldNotLogWhenDebugLog()
        {
            Logger.LogLevel = LoggingLevel.Error;
            Mock<ILogWriter> writer = SetUpWriter(LoggingLevel.Debug);

            Logger.Writers.Add(writer.Object);

            Logger.Debug(nameof(LoggingTests), "This is my log msg");

            writer.Verify(mock => mock.Log(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void DebugLogLevelShouldLogErrorLog()
        {
            Logger.LogLevel = LoggingLevel.Debug;
            Mock<ILogWriter> writer = SetUpWriter(LoggingLevel.Error);

            Logger.Writers.Add(writer.Object);

            Logger.Error(nameof(LoggingTests), "This is my log msg");

            writer.Verify(mock => mock.Log(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ErrorLogLevelShouldNotLogAnyLowerLevel()
        {
            Logger.LogLevel = LoggingLevel.Error;

            Mock<ILogWriter> info = SetUpWriter(LoggingLevel.Info);

            Mock<ILogWriter> warn = SetUpWriter(LoggingLevel.Warn);

            Mock<ILogWriter> debug = SetUpWriter(LoggingLevel.Debug);

            Logger.Writers.Add(info.Object);
            Logger.Writers.Add(warn.Object);
            Logger.Writers.Add(debug.Object);

            Logger.Error("", "");
            Logger.Warn("", "");
            Logger.Info("", "");
            Logger.Debug("", "");

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
            Logger.Writers.Clear();
        }

    }
}
