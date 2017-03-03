using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace MatthiWare.UpdateLib.Files
{
    /// <summary>
    /// This will convert specified path variables to their actual path
    /// </summary>
    public class PathVariableConverter
    {
        private Dictionary<string, string> variables;

        public PathVariableConverter()
        { 
            variables = new Dictionary<string, string>();

            variables.Add("appdir", new DirectoryInfo(".").FullName);
            variables.Add("appdata", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            variables.Add("temp", Path.GetTempPath());
            variables.Add("otherdir", "");
        }

        /// <summary>
        /// Gets the full path of a key
        /// </summary>
        /// <param name="key">The variable name</param>
        /// <returns>The full path of the variable as a <see cref="string"/> </returns>
        public string this[string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException("key");

                if (!variables.ContainsKey(key.ToLower()))
                    return null;

                return variables[key.ToLower()];
            }
        }

        public void Add(string key, string val)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            if (string.IsNullOrEmpty(val))
                throw new ArgumentNullException("val");

            if (variables.ContainsKey(key.ToLower()))
                throw new ArgumentException("Duplicate key entry", "key");

            variables.Add(key.ToLower(), val);
        }

        /// <summary>
        /// Gets wether or not the converter contains a certain key
        /// </summary>
        /// <param name="key">The variable name</param>
        /// <returns>True if the converter contains the key and False if not</returns>
        public bool Contains(string key)
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
                throw new ArgumentNullException("key");

            if (!variables.ContainsKey(key.ToLower()))
                return false;

            return variables.Remove(key.ToLower());
        }

        public string Replace(string input)
        {
            string[] tokens = input.Split('%');
            StringBuilder sb = new StringBuilder();

            foreach (string i in tokens)
                sb.Append(Contains(i) ? this[i] : i);

            return sb.ToString();
        }

    }
}
