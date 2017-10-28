/*  UpdateLib - .Net auto update library
 *  Copyright (C) 2016 - MatthiWare (Matthias Beerens)
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published
 *  by the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Utils
{
    public class CmdLineParser
    {
        private SortedDictionary<string, ParamDef> m_wantedParameters = new SortedDictionary<string, ParamDef>();
        private StringDictionary m_foundParameters = new StringDictionary();

        public string ParameterPrefix { get; set; } = "--";

        public void AddParameter(string paramName, ParamMandatoryType mandatoryType = ParamMandatoryType.Required, ParamValueType valueType = ParamValueType.String, string help = "")
        {
            if (string.IsNullOrEmpty(paramName)) throw new ArgumentNullException(nameof(paramName));

            var param = new ParamDef(paramName, mandatoryType, valueType, help);
            m_wantedParameters.Add(paramName, param);
        }

        public string this[string paramName]
        {
            get
            {
                return m_foundParameters.ContainsKey(paramName) ? m_foundParameters[paramName] : null;
            }
        }

        public void Parse()
        {
            Parse(Environment.GetCommandLineArgs());

            CheckAllMandatoryParamsFound();
        }

        private void Parse(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string data = args[i];

                var def = m_wantedParameters.Where(p => p.Key == data).Select(p => p.Value).FirstOrDefault();

                if (def == null)
                    continue;

                
            }
        }

        private void CheckAllMandatoryParamsFound()
        {
            m_wantedParameters
                .Where(param => param.Value.MandatoryType == ParamMandatoryType.Required)
                .ForEach(param =>
                {
                    if (!m_foundParameters.ContainsKey(param.Key))
                        throw new KeyNotFoundException($"Mandatory parameter '{param.Key}' is missing");
                });
        }

        private class ParamDef
        {
            public string ParameterName { get; set; }
            public ParamMandatoryType MandatoryType { get; set; }
            public ParamValueType ValueType { get; set; }
            public string HelpMessage { get; set; }

            public ParamDef(string paramName, ParamMandatoryType mandatoryType, ParamValueType valueType, string help)
            {
                ParameterName = paramName;
                MandatoryType = mandatoryType;
                ValueType = valueType;
                HelpMessage = help;
            }
        }
    }

    public enum ParamValueType
    {
        String,
        OptionalString,
        Int,
        OptionalInt,
        Bool,
        OptionalBool,
        MultipleInts
    }

    public enum ParamMandatoryType
    {
        Optional,
        Required
    }
}
