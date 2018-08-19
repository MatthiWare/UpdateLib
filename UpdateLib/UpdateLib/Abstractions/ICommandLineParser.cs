using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.UpdateLib.Common;

namespace MatthiWare.UpdateLib.Abstractions
{
    public interface ICommandLineParser
    {
        void AddParameter<T>(string paramName, ParamMandatoryType mandatoryType = ParamMandatoryType.Optional, ParamValueType valueType = ParamValueType.None);

        void AddParameter<T>(string paramName, ParamMandatoryType mandatoryType, ParamValueType valueType, ICommandLineArgumentResolver<T> resolver);

        void Parse();

        IParameterDefinition<T> Get<T>(string name);

        IParameterDefinition Get(string name);

    }
}
