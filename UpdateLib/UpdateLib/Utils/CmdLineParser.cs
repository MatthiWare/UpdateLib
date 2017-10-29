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
using MatthiWare.UpdateLib.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Utils
{
    public class CmdLineParser
    {
        private SortedDictionary<string, ParameterDefinition> m_params = new SortedDictionary<string, ParameterDefinition>();

        public string ParameterPrefix { get; set; } = "--";

        public void AddParameter(string paramName, ParamMandatoryType mandatoryType = ParamMandatoryType.Required, ParamValueType valueType = ParamValueType.String, string help = "")
        {
            if (string.IsNullOrEmpty(paramName)) throw new ArgumentNullException(nameof(paramName));
            if (m_params.ContainsKey(paramName)) throw new ArgumentException("Key already exists", nameof(paramName));

            var param = new ParameterDefinition(paramName, mandatoryType, valueType, help);
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
            m_params.ForEach(kvp => kvp.Value.Reset());

            for (int i = 0; i < args.Length; i++)
            {
                string data = args[i];

                var def = m_params.Where(p => p.Key == ParameterPrefix + data).Select(p => p.Value).FirstOrDefault();

                if (def == null)
                    continue;
            }

            CheckAllMandatoryParamsFound();
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
