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

        public void Debug(string className, string methodName, string msg)
        {
            Log($"{className}::{methodName}", msg, LoggingLevel.Debug);
        }

        public void Info(string className, string methodName, string msg)
        {
            Log($"{className}::{methodName}", msg, LoggingLevel.Info);
        }

        public void Warn(string className, string methodName, string msg)
        {
            Log($"{className}::{methodName}", msg, LoggingLevel.Warn);
        }

        public void Error(string className, string methodName, string msg)
        {
            Log($"{className}::{methodName}", msg, LoggingLevel.Error);
        }

        public void Error(string className, string methodName, Exception e)
        {
            Error(className, string.IsNullOrEmpty(methodName) ? e.TargetSite.Name : methodName, e.ToString());
        }
    }
}
