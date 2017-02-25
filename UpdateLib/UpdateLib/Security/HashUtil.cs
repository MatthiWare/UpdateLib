using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace MatthiWare.UpdateLib.Security
{
    public class HashUtil
    {
        /// <summary>
        /// Gets the hash from file using the given hashing algorithm.
        /// </summary>
        /// <typeparam name="T">The hashing algorithm should be a type of <see cref="HashAlgorithm"/> </typeparam>
        /// <param name="file">The input file to hash</param>
        /// <returns>The hashed string</returns>
        public static string GetHash<T>(string file) where T : HashAlgorithm
        {
            if (string.IsNullOrEmpty(file))
                throw new ArgumentNullException(file);

            if (!File.Exists(file))
                throw new FileNotFoundException("File not found", file);

            using (Stream stream = File.OpenRead(file))
            {
                using (HashAlgorithm algo = HashAlgorithm.Create(typeof(T).FullName))
                {
                    byte[] hash = algo.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", string.Empty);
                }
            }
        }

    }
}
