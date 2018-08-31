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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Utils
{
    public static class ExtensionMethods
    {
        [DebuggerStepThrough]
        public static string AppendAll<T>(this IEnumerable<T> collection, string seperator)
        {
            using (var enumerator = collection.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return string.Empty;

                var builder = new StringBuilder().Append(enumerator.Current);

                while (enumerator.MoveNext())
                    builder.Append(seperator).Append(enumerator.Current);

                return builder.ToString();
            }
        }

        public static IEnumerable<string> Replace(this IEnumerable<string> collection, string oldStr, string newStr)
        {
            foreach (var str in collection)
                yield return str.Replace(oldStr, newStr);
        }

        [DebuggerStepThrough]
        public static T MaxOrDefault<T, K>(this IEnumerable<T> collection, Func<T, K> resolve) where K : IComparable<K>
        {
            T max = default(T);

            foreach (T other in collection)
            {
                if (max == null)
                {
                    max = other;
                    continue;
                }

                if (resolve(other).CompareTo(resolve(max)) > 0)
                    max = other;
            }

            return max;
        }

        public static string GetDescription(this Type type)
            => type.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().FirstOrDefault().Description ?? "No description available";


        [DebuggerStepThrough]
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection)
                action(item);
        }

        [DebuggerStepThrough]
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> collection)
        {
            foreach (T item in collection)
                if (item != null)
                    yield return item;

        }

        [DebuggerStepThrough]
        public static IEnumerable<string> NotEmpty(this IEnumerable<string> collection)
        {
            foreach (var item in collection)
                if (!string.IsNullOrEmpty(item))
                    yield return item;
        }
    }
}
