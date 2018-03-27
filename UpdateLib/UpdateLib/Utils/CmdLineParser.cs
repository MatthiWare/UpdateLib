/*  Copyright (C) 2016 - MatthiWare (Matthias Beerens)
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
using System.Linq;

using MatthiWare.UpdateLib.Common;

namespace MatthiWare.UpdateLib.Utils
{
    public class CmdLineParser
    {
        private SortedDictionary<string, ParameterDefinition> m_params = new SortedDictionary<string, ParameterDefinition>();

        public string ParameterPrefix { get; set; } = "--";

        public void AddParameter(string paramName, ParamMandatoryType mandatoryType = ParamMandatoryType.Optional, ParamValueType valueType = ParamValueType.None)
        {
            if (string.IsNullOrEmpty(paramName)) throw new ArgumentNullException(nameof(paramName));
            if (paramName.Contains(' ')) throw new ArgumentException("Parameter cannot contain spaces", nameof(paramName));
            if (m_params.ContainsKey(paramName)) throw new ArgumentException("Key already exists", nameof(paramName));

            var param = new ParameterDefinition(paramName, mandatoryType, valueType);
            m_params.Add(paramName, param);
        }

        public ParameterDefinition this[string paramName]
        {
            get
            {
                return m_params.ContainsKey(paramName) ? m_params[paramName] : null;
            }
        }

        public void Parse() => Parse(Environment.GetCommandLineArgs());


        public void Parse(string[] args)
        {
            if (string.IsNullOrEmpty(ParameterPrefix)) throw new ArgumentNullException(nameof(ParameterPrefix));

            m_params.ForEach(kvp => kvp.Value.Reset());

            for (int i = 0; i < args.Length; i++)
            {
                string data = args[i];

                var def = m_params.Where(p => ParameterPrefix + p.Key == data).Select(p => p.Value).FirstOrDefault();

                if (def == null)
                    continue;

                def.Count++;

                if (def.ValueType == ParamValueType.None)
                    continue;

                FindParameterValue(def, args, ref i);
            }

            CheckAllMandatoryParamsFound();
        }

        private void FindParameterValue(ParameterDefinition param, string[] args, ref int index)
        {
            string data = args[++index];
            bool succes = true;

            if (param.ValueType == ParamValueType.Int || param.ValueType == ParamValueType.OptionalInt)
            {
                succes = int.TryParse(data, out int value);

                if (succes)
                    param.Value = value;
            }
            else if (param.ValueType == ParamValueType.Bool || param.ValueType == ParamValueType.OptionalBool)
            {
                succes = bool.TryParse(data, out bool value);

                if (succes)
                    param.Value = value;
            }
            else if (param.ValueType == ParamValueType.String || param.ValueType == ParamValueType.OptionalString)
            {
                succes = !data.StartsWith(ParameterPrefix);

                if (succes)
                    param.Value = data;
            }
            else if (param.ValueType == ParamValueType.MultipleInts)
            {
                var values = new List<int>();

                while (index < args.Length && int.TryParse(args[index], out int outValue))
                {
                    values.Add(outValue);
                    index++;
                }

                succes = values.Count >= 2;

                if (succes)
                    param.Value = values.ToArray();
            }

            if (!succes)
            {
                --index;

                if (param.ValueType != ParamValueType.OptionalBool &&
                    param.ValueType != ParamValueType.OptionalInt &&
                    param.ValueType != ParamValueType.OptionalString)
                    param.Count = 0;
            }

        }

        private void CheckAllMandatoryParamsFound()
        {
            m_params
                .Where(kvp => kvp.Value.MandatoryType == ParamMandatoryType.Required)
                .Select(kvp => kvp.Value)
                .ForEach(param =>
                {
                    if (!param.IsFound)
                        throw new KeyNotFoundException($"Mandatory parameter '{param.ParameterName}' is missing");
                });
        }
    }


}
