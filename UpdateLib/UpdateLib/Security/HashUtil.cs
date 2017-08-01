﻿using System;
using System.Security.Cryptography;
using System.IO;

namespace MatthiWare.UpdateLib.Security
{
    public static class HashUtil
    {
        /// <summary>
        /// Get the hash from a file using the default hashing algorithm.
        /// default algorithm is <see cref="SHA256"/>. 
        /// </summary>
        /// <param name="file">The input file to hash</param>
        /// <returns>The hashed string</returns>
        public static string GetHash(string file)
        {
            return GetHash<SHA256>(file);
        }

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
