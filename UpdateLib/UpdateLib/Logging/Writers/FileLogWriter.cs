using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MatthiWare.UpdateLib.Logging.Writers
{
    public class FileLogWriter : ILogWriter
    {
        public const string LOG_FOLDER_NAME = "Log";

        public LoggingLevel LoggingLevel { get { return LoggingLevel.Debug; } }

        private FileInfo logFile;

        private readonly object sync = new object();

        public FileLogWriter()
        {
            string path = GetPathPrefix();
            string productName = GetProductName();
            string name = Assembly.GetEntryAssembly().GetName().Name;
            logFile = new FileInfo($@"{path}\{name}\{productName}\{LOG_FOLDER_NAME}\log_{DateTime.Now.ToString("yyyyMMdd")}.log");

            if (!logFile.Directory.Exists)
                logFile.Directory.Create();
        }

        private string GetProductName()
        {
            AssemblyProductAttribute attr = Attribute.GetCustomAttribute(Assembly.GetAssembly(typeof(FileLogWriter)), typeof(AssemblyProductAttribute)) as AssemblyProductAttribute;
            return attr?.Product ?? "";
        }

        private string GetPathPrefix()
        {
            switch (Updater.Instance.InstallationMode)
            {
                case InstallationMode.Local:
                    return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                default:
                    return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }
        }

        public void Log(string text)
        {
            Action<string> logAction = new Action<string>(LogAsync);
            logAction.BeginInvoke(text, new AsyncCallback(r => logAction.EndInvoke(r)), null);
        }

        private void LogAsync(string text)
        {
            lock (sync)
            {
                using (StreamWriter writer = new StreamWriter(logFile.Open(FileMode.OpenOrCreate, FileAccess.Write)))
                {
                    writer.BaseStream.Seek(0, SeekOrigin.End);
                    writer.WriteLine(text);
                }
            }
        }
    }
}
