using System;
using MatthiWare.UpdateLib.Abstractions;

namespace MatthiWare.UpdateLib.Common
{

    public class ParameterDefinition : IParameterDefinition
    {
        public string Name { get; }

        public ParamValueType ValueType { get; }

        public ParamMandatoryType MandatoryType { get; }

        public int Count { get; set; }

        public bool IsFound => Count > 0;

        public bool CanResolve(ref string[] args, ref int index) => false;

        public ParameterDefinition(string paramName, ParamMandatoryType mandatoryType, ParamValueType valueType)
        {
            Name = paramName;
            MandatoryType = mandatoryType;
            ValueType = valueType;
        }

        public void Reset() => Count = 0;

        public void Resolve(ref string[] args, ref int index) => throw new NotImplementedException();
    }

    public class ParameterDefinition<T> : IParameterDefinition<T>
    {
        public string Name { get; }

        public ParamMandatoryType MandatoryType { get; }

        public ParamValueType ValueType { get; }

        private readonly ICommandLineArgumentResolver<T> m_valueResolver;

        public T Value { get; set; }

        public int Count { get; set; }

        public bool IsFound => Count > 0;

        public ParameterDefinition(string paramName, ParamMandatoryType mandatoryType, ParamValueType valueType, ICommandLineArgumentResolver<T> valueResolver)
        {
            Name = paramName;
            MandatoryType = mandatoryType;
            ValueType = valueType;
            m_valueResolver = valueResolver;
        }

        public void Resolve(ref string[] args, ref int index)
        {
            Value = m_valueResolver.Resolve(ref args, ref index);
            Count++;
        }

        public bool CanResolve(ref string[] args, ref int index)
        {
            if (index < 0 || index >= args.Length) return false;

            return m_valueResolver.CanResolve(ref args, ref index);
        }

        public void Reset()
        {
            Value = default;
            Count = 0;
        }
    }
}
