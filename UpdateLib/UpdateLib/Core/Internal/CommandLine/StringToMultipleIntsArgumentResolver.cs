using System.Collections.Generic;
using System.Linq;
using MatthiWare.UpdateLib.Abstractions;

namespace MatthiWare.UpdateLib.Core.Internal.CommandLine
{
    public class StringToMultipleIntsArgumentResolver : ICommandLineArgumentResolver<int[]>
    {
        private IList<int> ints = new List<int>();

        public bool CanResolve(ref string[] data, ref int index)
        {
            while (index < data.Length && int.TryParse(data[index], out int outValue))
            {
                ints.Add(outValue);
                index++;
            }

            return ints.Count >= 2; // at least 2 elements
        }


        public int[] Resolve(ref string[] data, ref int index)
            => ints.ToArray();

        object ICommandLineArgumentResolver.Resolve(ref string[] data, ref int index)
            => Resolve(ref data, ref index);
    }
}
