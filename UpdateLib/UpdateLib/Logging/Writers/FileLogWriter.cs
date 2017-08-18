/*  UpdateLib - .Net auto update library
 *  Copyright (C) 2016 - MatthiWare (Matthias Beerens)
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published
 *  by the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using MatthiWare.UpdateLib.Utils;
using System;
using System.IO;
using System.Reflection;

namespace MatthiWare.UpdateLib.Logging.Writers
{
    public class FileLogWriter : ILogWriter
    {
        public const string LOG_FOLDER_NAME = "Log";

        public LoggingLevel LoggingLevel { get { return LoggingLevel.Debug; } }

        private Lazy<FileInfo> m_logFile = new Lazy<FileInfo>(GetLogFile);

        private readonly object sync = new object();

        private static FileInfo GetLogFile()
        {
            string path = IOUtils.AppDataPath;
            string name = Assembly.GetEntryAssembly().GetName().Name;

            FileInfo m_logFile = new FileInfo($@"{path}\{LOG_FOLDER_NAME}\log_{DateTime.Now.ToString("yyyyMMdd")}.log");

            if (!m_logFile.Directory.Exists)
                m_logFile.Directory.Create();

            return m_logFile;
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
                using (StreamWriter writer = new StreamWriter(m_logFile.Value.Open(FileMode.OpenOrCreate, FileAccess.Write)))
                {
                    writer.BaseStream.Seek(0, SeekOrigin.End);
                    writer.WriteLine(text);
                }
            }
        }
    }
}
