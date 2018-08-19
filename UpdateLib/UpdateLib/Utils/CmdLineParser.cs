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
using MatthiWare.UpdateLib.Abstractions;
using MatthiWare.UpdateLib.Common;

namespace MatthiWare.UpdateLib.Utils
{
    public class CmdLineParser : ICommandLineParser
    {
        private SortedDictionary<string, IParameterDefinition> m_params = new SortedDictionary<string, IParameterDefinition>();

        public string ParameterPrefix { get; set; } = "--";

        public void AddParameter<T>(string paramName, ParamMandatoryType mandatoryType = ParamMandatoryType.Optional, ParamValueType valueType = ParamValueType.None)
            => AddParameter<T>(paramName, mandatoryType, valueType, default);

        public void AddParameter<T>(string paramName, ParamMandatoryType mandatoryType, ParamValueType valueType, ICommandLineArgumentResolver<T> resolver)
        {
            if (string.IsNullOrEmpty(paramName)) throw new ArgumentNullException(nameof(paramName));
            if (paramName.Contains(' ')) throw new ArgumentException("Parameter cannot contain spaces", nameof(paramName));
            if (m_params.ContainsKey(paramName)) throw new ArgumentException("Key already exists", nameof(paramName));

            var param = new ParameterDefinition<T>(paramName, mandatoryType, valueType, resolver);
            m_params.Add(paramName, param);
        }

        public IParameterDefinition<T> Get<T>(string paramName)
            => m_params.ContainsKey(paramName) ? m_params[paramName] as IParameterDefinition<T> : default;

        public IParameterDefinition Get(string paramName)
            => m_params.ContainsKey(paramName) ? m_params[paramName] : default;

        public void Parse() => Parse(Environment.GetCommandLineArgs());


        private void Parse(string[] args)
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

                FindParameterValue(def, ref args, ref i);
            }

            CheckAllMandatoryParamsFound();
        }

        private void FindParameterValue(IParameterDefinition param, ref string[] args, ref int index)
        {
            ++index;
            bool succes = param.CanResolve(ref args, ref index);

            if (succes)
                param.Resolve(ref args, ref index);

            //else if (param.ValueType == ParamValueType.Bool || param.ValueType == ParamValueType.OptionalBool)
            //{
            //    succes = bool.TryParse(data, out bool value);

            //    if (succes)
            //        param.Value = value;
            //}
            //else if (param.ValueType == ParamValueType.String || param.ValueType == ParamValueType.OptionalString)
            //{
            //    succes = !data.StartsWith(ParameterPrefix);

            //    if (succes)
            //        param.Value = data;
            //}
            //else if (param.ValueType == ParamValueType.MultipleInts)
            //{
            //    var values = new List<int>();

            //    while (index < args.Length && int.TryParse(args[index], out int outValue))
            //    {
            //        values.Add(outValue);
            //        index++;
            //    }

            //    succes = values.Count >= 2;

            //    if (succes)
            //        param.Value = values.ToArray();
            //}

            if (!succes)
                --index;

        }

        private void CheckAllMandatoryParamsFound()
        {
            IList<Exception> exceptions = new List<Exception>();

            m_params
                .Select(kvp => kvp.Value)
                .Where(param => param.MandatoryType == ParamMandatoryType.Required && !param.IsFound)
                .ForEach(param => exceptions.Add(new KeyNotFoundException($"Mandatory parameter '{param.Name}' is missing")));

            if (exceptions.Any())
                throw new AggregateException("One or more mandatory parameters are missing", exceptions);

        }
    }


}
