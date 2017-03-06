using System.Diagnostics;

namespace MatthiWare.UpdateLib.Logging.Writers
{
    public class ConsoleLogWriter : ILogWriter
    {
        public LoggingLevel LoggingLevel { get { return LoggingLevel.Debug; } }

        public void Log(string text)
        {
            Debug.WriteLine(text);
        }
    }
}
