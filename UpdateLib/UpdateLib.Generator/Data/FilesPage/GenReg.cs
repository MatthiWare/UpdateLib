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

using Microsoft.Win32;
using System;

namespace MatthiWare.UpdateLib.Generator.Data.FilesPage
{
    public class GenReg : IGenItem
    {
        public event EventHandler Changed;

        private RegistryValueKind m_type;
        public RegistryValueKind Type
        {
            get { return m_type; }
            set
            {
                m_type = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        private string m_name;
        public string Name
        {
            get { return m_name; }
            set
            {
                m_name = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        private object m_value;
        public object Value
        {
            get { return m_value; }
            set
            {
                m_value = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public GenFolder Parent { get; set; }
        public ListViewGenItem View { get; set; }

        public GenReg(string name, RegistryValueKind kind = RegistryValueKind.String)
        {
            Name = name;
            Type = kind;

            View = new ListViewGenItem(this);
        }

        public string[] GetListViewItems()
        {
            return new string[] { Name, GetTypeName(), Value?.ToString() ?? string.Empty };
        }

        private string GetTypeName()
        {
            switch (Type)
            {
                case RegistryValueKind.ExpandString:
                    return "REG_EXPANDED_SZ";
                case RegistryValueKind.MultiString:
                    return "REG_MULTI_SZ";
                case RegistryValueKind.Binary:
                    return "REG_BINARY";
                case RegistryValueKind.DWord:
                    return "REG_DWORD";
                case RegistryValueKind.QWord:
                    return "REG_QWORD";
                case RegistryValueKind.String:
                case RegistryValueKind.Unknown:
                default:
                    return "REG_SZ";
            }
        }

        public string GetListViewImageKey()
        {
            switch (Type)
            {
                case RegistryValueKind.String:
                case RegistryValueKind.ExpandString:
                case RegistryValueKind.MultiString:
                    return "REG_SZ";
                case RegistryValueKind.Binary:
                case RegistryValueKind.DWord:
                case RegistryValueKind.QWord:
                case RegistryValueKind.Unknown:
                default:
                    return "REG_BIN";
            }
        }

    }
}
