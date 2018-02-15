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
            using (var enumerator = collection.GetEnumerator())
                while (enumerator.MoveNext())
                    yield return enumerator.Current.Replace(oldStr, newStr);
        }

        [DebuggerStepThrough]
        public static T MaxOrDefault<T, K>(this IEnumerable<T> collection, Func<T, K> resolve) where K : IComparable<K>
        {
            using (var enumerator = collection.GetEnumerator())
            {
                T max = default(T);

                while (enumerator.MoveNext())
                {
                    T other = enumerator.Current;

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
        }

        public static string GetDescription(this Type type)
            => type.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().FirstOrDefault().Description ?? "invalid description";


        [DebuggerStepThrough]
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            using (var enumerator = collection.GetEnumerator())
                while (enumerator.MoveNext())
                    action(enumerator.Current);
        }

        [DebuggerStepThrough]
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> collection)
        {
            using (var enumerator = collection.GetEnumerator())
                while (enumerator.MoveNext())
                    if (enumerator.Current != null)
                        yield return enumerator.Current;

        }

        [DebuggerStepThrough]
        public static IEnumerable<string> NotEmpty(this IEnumerable<string> collection)
        {
            using (var enumerator = collection.GetEnumerator())
                while (enumerator.MoveNext())
                    if (!string.IsNullOrEmpty(enumerator.Current))
                        yield return enumerator.Current;
        }

        /// <summary>
        /// Skips n amount of the last elements
        /// </summary>
        /// <typeparam name="T">Any</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="count">The count of last elements to skip</param>
        /// <returns><see cref="IEnumerable{T}"/> without the last <paramref name="count"/> elements.</returns>
        [DebuggerStepThrough]
        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int count)
        {
            if (count == 0)
            {
                foreach (T item in source)
                    yield return item;

                yield break;
            }

            int i = 0;
            var buffer = new Queue<T>(count + 1);

            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (i == count)
                    {
                        yield return buffer.Dequeue();
                        i--;
                    }

                    buffer.Enqueue(enumerator.Current);
                    i++;
                }
            }
        }
    }
}
