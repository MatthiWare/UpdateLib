using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Logging
{
    public static class Logger
    {
        public static LoggingLevel LogLevel { get; set; } = LoggingLevel.Debug;

        public static List<ILogWriter> Writers { get; } = new List<ILogWriter>();

        public static void Log(string tag, string msg, LoggingLevel level)
        {
            if (level < LogLevel) return;

            Writers
                .Where(w => w.LoggingLevel >= LogLevel && level >= w.LoggingLevel)
                .ToList()
                .ForEach(w => w.Log($"[{DateTime.Now.ToString()}][{level.ToString()}][{tag}]\t{msg}"));
        }

        public static void Debug(string tag, string msg)
        {
            Log(tag, msg, LoggingLevel.Debug);
        }

        public static void Info(string tag, string msg)
        {
            Log(tag, msg, LoggingLevel.Info);
        }

        public static void Warn(string tag, string msg)
        {
            Log(tag, msg, LoggingLevel.Warn);
        }

        public static void Error(string tag, string msg)
        {
            Log(tag, msg, LoggingLevel.Error);
        }

        public static void Error(string tag, Exception e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(e.GetBaseException().GetType().Name);
            sb.AppendLine(e.Message);
            sb.AppendLine(e.StackTrace);

            Exception inner = e;
            while ((inner = inner.InnerException) != null)
            {
                sb.AppendLine(inner.Message);
                sb.AppendLine(inner.StackTrace);
            }

            Error(tag, sb.ToString());
        }
    }
}
