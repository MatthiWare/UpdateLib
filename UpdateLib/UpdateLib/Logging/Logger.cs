using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Logging
{
    public class Logger : ILogger
    {
        public LoggingLevel LogLevel { get; set; } = LoggingLevel.Debug;

        public ICollection<ILogWriter> Writers { get; } = new List<ILogWriter>();

        private const string TEMPLATE = "[{0}][{1}][{2}]: {3}";

        public void Log(string tag, string msg, LoggingLevel level)
        {
            if (level < LogLevel) return;

            Writers
                .Where(w => w.LoggingLevel >= LogLevel && level >= w.LoggingLevel)
                .ToList()
                .ForEach(w => w.Log(string.Format(TEMPLATE, DateTime.Now, level, tag, msg))); 
        }

        public void Debug(string tag, string msg)
        {
            Log(tag, msg, LoggingLevel.Debug);
        }

        public void Info(string tag, string msg)
        {
            Log(tag, msg, LoggingLevel.Info);
        }

        public void Warn(string tag, string msg)
        {
            Log(tag, msg, LoggingLevel.Warn);
        }

        public void Error(string tag, string msg)
        {
            Log(tag, msg, LoggingLevel.Error);
        }

        public void Error(string tag, Exception e)
        {
            Error(tag, e.ToString());
        }
    }
}
