using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MatthiWare.UpdateLib.Win32
{
    internal static class NativeMethods
    {

        [DllImport("wininet.dll")]
        internal static extern bool InternetGetConnectedState(out int connDescription, int reservedValue);

    }
}
