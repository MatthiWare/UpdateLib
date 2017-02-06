using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace MatthiWare.UpdateLib.Files
{
    /// <summary>
    /// This will convert specified path variables to their actual path
    /// </summary>
    public static class PathVariableConverter
    {

        private static Dictionary<string, string> variables = new Dictionary<string, string>();

        static PathVariableConverter()
        {
            variables.Add("appdir", Assembly.GetEntryAssembly().Location);
            variables.Add("appdata", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            variables.Add("temp", Path.GetTempPath());
            variables.Add("otherdir", "");
        }

        /// <summary>
        /// Gets the full path of a key
        /// </summary>
        /// <param name="key">The variable name</param>
        /// <returns>The full path of the variable as a <see cref="string"/> </returns>
        public static string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            if (!variables.ContainsKey(key.ToLower()))
                return null;

            return variables[key.ToLower()];
        }

        /// <summary>
        /// Gets wether or not the converter contains a certain key
        /// </summary>
        /// <param name="key">The variable name</param>
        /// <returns>True if the converter contains the key and False if not</returns>
        public static bool Contains(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            return variables.ContainsKey(key.ToLower());
        }

        /// <summary>
        /// Removes a specified key from the converter
        /// </summary>
        /// <param name="key">The variable name</param>
        /// <returns>True if the converter removed the key and False if not</returns>
        public static bool Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            if (!variables.ContainsKey(key.ToLower()))
                return false;

            return variables.Remove(key.ToLower());
        }

        public static string Replace(string input)
        {
            string[] tokens = input.Split('%');
            StringBuilder sb = new StringBuilder();

            foreach (string i in tokens)
                sb.Append(Contains(i) ? Get(i) : i);

            return sb.ToString();
        }

    }
}
