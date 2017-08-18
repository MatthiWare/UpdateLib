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

using System;
using System.Collections.Generic;
using System.Linq;

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
