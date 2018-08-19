using System;
using System.Collections.Generic;
using System.Text;

namespace MatthiWare.UpdateLib.Abstractions
{
    public interface ICommandLineArgumentResolver
    {
        bool CanResolve(ref string[] data, ref int index);

        object Resolve(ref string[] data, ref int index);
    }

    public interface ICommandLineArgumentResolver<TOutput> : ICommandLineArgumentResolver
    {
        new TOutput Resolve(ref string[] data, ref int index);
    }
}
