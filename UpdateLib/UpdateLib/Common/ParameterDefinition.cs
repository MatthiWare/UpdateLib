namespace MatthiWare.UpdateLib.Common
{
    public class ParameterDefinition
    {
        public string ParameterName { get; private set; }
        public ParamMandatoryType MandatoryType { get; private set; }
        public ParamValueType ValueType { get; private set; }
        public object Value { get; internal set; }

        public int Count { get; internal set; }

        public bool IsFound => Count > 0;

        internal ParameterDefinition(string paramName, ParamMandatoryType mandatoryType = ParamMandatoryType.Optional, ParamValueType valueType = ParamValueType.None)
        {
            ParameterName = paramName;
            MandatoryType = mandatoryType;
            ValueType = valueType;
        }

        internal void Reset()
        {
            Value = null;
            Count = 0;
        }
    }
}
