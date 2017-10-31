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
using System.IO;
using System.Text;

namespace MatthiWare.UpdateLib.Files
{
    /// <summary>
    /// This will convert specified path variables to their actual path
    /// </summary>
    public class PathVariableConverter
    {
        private Dictionary<string, string> variables;

        private readonly object sync = new object();

        public PathVariableConverter()
        {
            variables = new Dictionary<string, string>();

            variables.Add("appdir", new DirectoryInfo(".").FullName);
            variables.Add("appdata", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            variables.Add("temp", Path.GetTempPath());
            variables.Add("otherdir", string.Empty);
        }

        /// <summary>
        /// Gets or sets the full path of a key
        /// </summary>
        /// <param name="key">The variable name</param>
        /// <returns>The full path of the variable as a <see cref="string"/> </returns>
        public string this[string key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Add(key, value);
            }
        }

        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            lock (sync)
            {
                if (!ContainsInternal(key))
                    return null;

                return variables[key.ToLower()];
            }
        }

        public void Add(string key, string val)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (string.IsNullOrEmpty(val))
                throw new ArgumentNullException(nameof(val));

            lock (sync)
            {
                if (ContainsInternal(key))
                    variables[key.ToLower()] = val;
                else
                    variables.Add(key.ToLower(), val);
            }
        }

        /// <summary>
        /// Gets wether or not the converter contains a certain key
        /// </summary>
        /// <param name="key">The variable name</param>
        /// <returns>True if the converter contains the key and False if not</returns>
        public bool Contains(string key)
        {
            lock (sync)
                return ContainsInternal(key);
        }

        private bool ContainsInternal(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            return variables.ContainsKey(key.ToLower());
        }

        /// <summary>
        /// Removes a specified key from the converter
        /// </summary>
        /// <param name="key">The variable name</param>
        /// <returns>True if the converter removed the key and False if not</returns>
        public bool Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (!ContainsInternal(key))
                return false;

            lock (sync)
                return variables.Remove(key.ToLower());
        }

        public string Convert(string input)
        {
            string[] tokens = input.Split('%');
            StringBuilder sb = new StringBuilder();

            foreach (string i in tokens)
                sb.Append(Contains(i) ? this[i] : i);

            return sb.ToString();
        }

    }
}
