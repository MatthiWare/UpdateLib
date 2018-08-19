using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using MatthiWare.UpdateLib.Abstractions;
using MatthiWare.UpdateLib.Common;

namespace MatthiWare.UpdateLib.Core.Internal.CommandLine
{
    internal class StringToUpdateVersionArgumentResolver : ICommandLineArgumentResolver<UpdateVersion>
    {
        public bool CanResolve(ref string[] data, ref int index)
            => UpdateVersion.CanParse(data[index]);

        public UpdateVersion Resolve(ref string[] data, ref int index)
            => new UpdateVersion(data[index]);

        object ICommandLineArgumentResolver.Resolve(ref string[] data, ref int index)
            => Resolve(ref data, ref index);
    }
}
