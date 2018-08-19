using MatthiWare.UpdateLib.Abstractions;

namespace MatthiWare.UpdateLib.Common
{
    public class ParameterDefinition<T> : IParameterDefinition<T>
    {
        public string Name { get; private set; }
        public ParamMandatoryType MandatoryType { get; private set; }

        public ParamValueType ValueType { get; private set; }

        private ICommandLineArgumentResolver<T> m_valueResolver;

        public T Value { get; set; }

        public int Count { get; set; }

        public bool IsFound => Count > 0;

        object IParameterDefinition.Value => Value;

        internal ParameterDefinition(string paramName, ParamMandatoryType mandatoryType, ParamValueType valueType)
        {
            Name = paramName;
            MandatoryType = mandatoryType;
            ValueType = valueType;
        }

        internal ParameterDefinition(string paramName, ParamMandatoryType mandatoryType, ParamValueType valueType, ICommandLineArgumentResolver<T> valueResolver)
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

        public bool CanResolve(ref string[] args, ref int index) => m_valueResolver.CanResolve(ref args, ref index);

        public void Reset()
        {
            Value = default;
            Count = 0;
        }
    }
}
