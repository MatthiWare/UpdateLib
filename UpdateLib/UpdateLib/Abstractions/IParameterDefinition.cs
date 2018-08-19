using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.UpdateLib.Common;

namespace MatthiWare.UpdateLib.Abstractions
{
    public interface IParameterDefinition
    {
        string Name { get; }
        ParamValueType ValueType { get; }
        ParamMandatoryType MandatoryType { get; }
        dynamic Value { get; }
        int Count { get; set; }
        bool IsFound { get; }
        void Reset();
        void Resolve(ref string[] args, ref int index);
        bool CanResolve(ref string[] args, ref int index);
    }

    public interface IParameterDefinition<T> : IParameterDefinition
    {
        new T Value { get; }
    }
}
