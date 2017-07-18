using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
