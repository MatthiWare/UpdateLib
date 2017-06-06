using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Logging
{
    public interface ILogger
    {
        LoggingLevel LogLevel { get; set; }
        ICollection<ILogWriter> Writers { get; }
        void Log(string tag, string msg, LoggingLevel level);
        void Debug(string className, string methodName, string msg);
        void Info(string className, string methodName, string msg);
        void Warn(string className, string methodName, string msg);
        void Error(string className, string methodName, string msg);
        void Error(string className, string methodName, Exception e);
    }
}
