using MatthiWare.UpdateLib.Abstractions;

namespace MatthiWare.UpdateLib.Core.Internal.CommandLine
{
    public class IntArgumentResolver : ICommandLineArgumentResolver<int>
    {
        private int value;

        public bool CanResolve(ref string[] data, ref int index)
            => int.TryParse(data[index], out value);

        public int Resolve(ref string[] data, ref int index)
            => value;

        object ICommandLineArgumentResolver.Resolve(ref string[] data, ref int index)
            => Resolve(ref data, ref index);
    }
}
