using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace MatthiWare.UpdateLib.Security
{
    public class HashUtil
    {

        /// <summary>
        /// Gets the MD5 hash from a given file
        /// </summary>
        /// <param name="file">The file to hash</param>
        /// <returns></returns>
        public static string GetHash(string file)
        {
            if (String.IsNullOrEmpty(file))
                throw new ArgumentNullException(file);

            if (!File.Exists(file))
                throw new FileNotFoundException("File not found", file);

            using (Stream stream = File.OpenRead(file))
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", string.Empty);
                }
            }
        }

    }
}
