using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib
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
}
