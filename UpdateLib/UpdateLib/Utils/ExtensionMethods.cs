using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Diagnostics;

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
