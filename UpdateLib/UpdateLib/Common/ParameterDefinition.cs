﻿using MatthiWare.UpdateLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Common
{
    public class ParameterDefinition
    {
        public string ParameterName { get; private set; }
        public ParamMandatoryType MandatoryType { get; private set; }
        public ParamValueType ValueType { get; private set; }
        public string HelpMessage { get; private set; }

        public object Value { get; internal set; }

        public int Count { get; internal set; }

        public bool IsFound { get { return Count > 0; } } 

        public ParameterDefinition(string paramName, ParamMandatoryType mandatoryType, ParamValueType valueType, string help)
        {
            ParameterName = paramName;
            MandatoryType = mandatoryType;
            ValueType = valueType;
            HelpMessage = help;
        }

        internal void Reset()
        {
            Value = null;
            Count = 0;
        }
    }
}
