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
        public static string AppendAll(this IEnumerable<string> collection, string seperator)
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
            {
                while (enumerator.MoveNext())
                {
                    action(enumerator.Current);
                }
            }
        }

        [DebuggerStepThrough]
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> collection)
        {
            using (var enumerator = collection.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current != null)
                        yield return enumerator.Current;
                }
            }
        }
    }
}
