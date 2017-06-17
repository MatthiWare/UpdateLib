using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Utils
{
    public static class RegistryHelper
    {

        public static RegistryKey GetEntryKey(string registryRootKey)
        {
            if (string.IsNullOrEmpty(registryRootKey)) throw new ArgumentNullException(nameof(registryRootKey));

            switch (registryRootKey)
            {
                case "HKEY_LOCAL_MACHINE":
                    return Registry.LocalMachine;

                case "HKEY_CURRENT_USER":
                    return Registry.CurrentUser;

                case "HKEY_CLASSES_ROOT":
                    return Registry.ClassesRoot;

                case "HKEY_USERS":
                    return Registry.Users;

                case "HKEY_CURRENT_CONFIG":
                    return Registry.CurrentConfig;

                case "HKEY_PERFORMANCE_DATA":
                    return Registry.PerformanceData;

                case "HKEY_DYN_DATA":
                    return Registry.DynData;

                default:
                    return null;
            }
        }

    }
}
