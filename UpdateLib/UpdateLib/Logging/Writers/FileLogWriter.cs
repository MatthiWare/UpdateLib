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

using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Tasks;
using MatthiWare.UpdateLib.Threading;
using MatthiWare.UpdateLib.Utils;
using System;
using System.IO;
using System.Reflection;

namespace MatthiWare.UpdateLib.Logging.Writers
{
    public class FileLogWriter : ILogWriter
    {
        public LoggingLevel LoggingLevel { get { return LoggingLevel.Debug; } }

        private Lazy<FileInfo> m_logFile = new Lazy<FileInfo>(GetLogFile);

        private ConcurrentQueue<string> m_logQueue = new ConcurrentQueue<string>();
        private AsyncTask m_logTask;

        public FileLogWriter()
        {
            m_logTask = AsyncTaskFactory.From(new Action(Log));
        }

        private static FileInfo GetLogFile()
        {
            FileInfo m_logFile = new FileInfo($@"{IOUtils.LogPath}\log_{DateTime.Now.ToString("yyyyMMdd")}.log");

            if (!m_logFile.Directory.Exists)
                m_logFile.Directory.Create();

            return m_logFile;
        }

        public void Log(string text)
        {
            m_logQueue.Enqueue(text);

            if (!m_logTask.IsRunning)
                m_logTask.Start();
        }

        private void Log()
        {
            string text;
            using (StreamWriter writer = new StreamWriter(m_logFile.Value.Open(FileMode.OpenOrCreate, FileAccess.Write)))
                while (m_logQueue.TryDequeue(out text))
                {
                    writer.BaseStream.Seek(0, SeekOrigin.End);
                    writer.WriteLine(text);
                }

        }
    }
}

