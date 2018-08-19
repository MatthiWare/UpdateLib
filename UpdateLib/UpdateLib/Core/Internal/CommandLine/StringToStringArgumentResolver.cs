using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.UpdateLib.Abstractions;
using Microsoft.Extensions.Options;

namespace MatthiWare.UpdateLib.Core.Internal.CommandLine
{
    public class StringToStringArgumentResolver : ICommandLineArgumentResolver<string>
    {
        private readonly UpdateLibOptions options;

        public StringToStringArgumentResolver(IOptions<UpdateLibOptions> options)
        {
            this.options = options.Value;
        }

        public bool CanResolve(ref string[] data, ref int index)
            => !data[index].StartsWith(options.ParameterPrefix); // if it is not a parameter


        public string Resolve(ref string[] data, ref int index)
            => data[index];

        object ICommandLineArgumentResolver.Resolve(ref string[] data, ref int index)
            => Resolve(ref data, ref index);
    }
}
