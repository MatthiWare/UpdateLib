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
        void Debug(string tag, string msg);
        void Info(string tag, string msg);
        void Warn(string tag, string msg);
        void Error(string tag, string msg);
        void Error(string tag, Exception e);
    }
}
