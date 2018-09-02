using System;
using System.Collections.Generic;
using System.Text;

namespace MatthiWare.UpdateLib.Utils
{
    internal static class Guard
    {
        public static T NotNull<T>(this T value, string propertyName) where T : class
           => value ?? throw new ArgumentNullException(propertyName);

        public static string NotNullOrEmpty(this string value, string propertyName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(propertyName);

            return value;
        }
    }
}
