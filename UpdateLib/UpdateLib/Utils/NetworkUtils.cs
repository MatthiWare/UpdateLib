using MatthiWare.UpdateLib.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Utils
{
    public static class NetworkUtils
    {


        public static bool HasConnection()
        {
            int desc;
            return NativeMethods.InternetGetConnectedState(out desc, 0);
        }

    }
}
