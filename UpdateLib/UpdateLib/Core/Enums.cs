﻿namespace MatthiWare.UpdateLib.Common
{
    /// <summary>
    /// Indicates the how the underlaying application is installed. 
    /// </summary>
    public enum InstallationMode
    {
        /// <summary>
        /// Shared installation we will use the roaming folder
        /// </summary>
        Shared = 0,

        /// <summary>
        /// Single user installation we will use the local folder
        /// </summary>
        Local = 1
    }

    public enum ParamValueType
    {
        None,
        Optional,
        Required
    }

    public enum ParamMandatoryType
    {
        Optional,
        Required
    }

    public enum VersionLabel : byte
    {
        Alpha = 0,
        Beta = 1,
        RC = 2,
        None = 3
    }

    internal enum InstructionType : byte
    {
        NoOp = 0,
        Add = 1,
        Run = 2,
        Copy = 3
    }
}