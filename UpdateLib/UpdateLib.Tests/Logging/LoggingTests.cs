using MatthiWare.UpdateLib.Logging;
using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateLib.Tests.Logging
{
    [TestFixture]
    public class LoggingTests
    {


        [Test]
        public void ErrorLogLevelShouldNotLogWhenDebugLog()
        {
            Logger.LogLevel = LoggingLevel.Error;
            Mock<ILogWriter> writer = new Mock<ILogWriter>();

            writer.SetupGet<LoggingLevel>(w => w.LoggingLevel).Returns(LoggingLevel.Debug);

            Logger.Writers.Add(writer.Object);

            Logger.Debug(nameof(LoggingTests), "This is my log msg");

            writer.Verify(mock => mock.Log(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void DebugLogLevelShouldLogErrorLog()
        {
            Logger.LogLevel = LoggingLevel.Debug;
            Mock<ILogWriter> writer = new Mock<ILogWriter>();

            writer.SetupGet<LoggingLevel>(w => w.LoggingLevel).Returns(LoggingLevel.Error);

            Logger.Writers.Add(writer.Object);

            Logger.Error(nameof(LoggingTests), "This is my log msg");

            writer.Verify(mock => mock.Log(It.IsAny<string>()), Times.Once);
        }

        [TearDown]
        public void CleanUp()
        {
            Logger.Writers.Clear();
        }

    }
}
