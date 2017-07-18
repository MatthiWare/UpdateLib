using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Tasks;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;

namespace MatthiWare.UpdateLib.Utils
{
    public static class RegistryHelper
    {

        public static RegistryKey GetOrMakeKey(RegistryKeyEntry key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            string pathToKeyRoot = key.Parent.DestinationLocation;

            return GetOrMakeKey(pathToKeyRoot);
        }

        public static RegistryKey GetOrMakeKey(string pathToKeyRoot)
        {
            if (string.IsNullOrEmpty(pathToKeyRoot)) throw new ArgumentNullException(nameof(pathToKeyRoot));

            string pathToKey = pathToKeyRoot.Split(char.Parse(@"\")).Skip(1).AppendAll(@"\");

            if (pathToKeyRoot.StartsWith("HKEY_LOCAL_MACHINE"))
                return OpenSubKey(Registry.LocalMachine, pathToKey);
            else if (pathToKeyRoot.StartsWith("HKEY_CURRENT_USER"))
                return OpenSubKey(Registry.CurrentUser, pathToKey);
            else if (pathToKeyRoot.StartsWith("HKEY_CLASSES_ROOT"))
                return OpenSubKey(Registry.ClassesRoot, pathToKey);
            else if (pathToKeyRoot.StartsWith("HKEY_USERS"))
                return OpenSubKey(Registry.Users, pathToKey);
            else if (pathToKeyRoot.StartsWith("HKEY_CURRENT_CONFIG"))
                return OpenSubKey(Registry.CurrentConfig, pathToKey);
            else if (pathToKeyRoot.StartsWith("HKEY_PERFORMANCE_DATA"))
                return OpenSubKey(Registry.PerformanceData, pathToKey);
            else if (pathToKeyRoot.StartsWith("HKEY_DYN_DATA"))
                return OpenSubKey(Registry.DynData, pathToKey);
            else
                return null;
        }

        private static RegistryKey OpenSubKey(RegistryKey key, string path)
        {
            return key.OpenSubKey(path, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
        }


        public static bool IsSame(RegistryKeyEntry key)
        {
            if (key == null)
                return false;

            string path = key.Parent.DestinationLocation;

            try
            {
                object value = Registry.GetValue(path, key.Name, new NullObject());

                return key.Value.Equals(value);

            }
            catch (Exception e)
            {
                Updater.Instance.Logger.Error(nameof(RegistryHelper), nameof(IsSame), e);
            }

            return false;
        }

        public static bool Exists(RegistryKeyEntry key, out object value)
        {
            value = null;

            if (key == null)
                return false;

            string path = key.Parent.DestinationLocation;

            try
            {
                value = Registry.GetValue(path, key.Name, null);

                return true;
            }
            catch (Exception e)
            {
                Updater.Instance.Logger.Error(nameof(RegistryHelper), nameof(Exists), e);
            }

            return false;
        }

        public static void Update(RegistryKeyEntry logicalKey, UpdateRegistryTask.RollbackData rollback)
        {
            if (logicalKey == null) throw new ArgumentNullException(nameof(logicalKey));

            RegistryKey key = GetOrMakeKey(logicalKey);

            rollback.type = key.GetValueKind(logicalKey.Name);

            key.SetValue(logicalKey.Name, logicalKey.Value, logicalKey.Type);
        }

        /// <summary>
        /// We do nothing with this
        /// </summary>
        private class NullObject { }
    }
}
