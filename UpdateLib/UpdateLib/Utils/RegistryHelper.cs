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

            return OpenSubKey(GetRootKey(pathToKeyRoot), pathToKey);
        }

        private static RegistryKey GetRootKey(string pathToKeyRoot)
        {
            if (pathToKeyRoot.StartsWith("HKEY_LOCAL_MACHINE"))
                return Registry.LocalMachine;
            else if (pathToKeyRoot.StartsWith("HKEY_CURRENT_USER"))
                return Registry.CurrentUser;
            else if (pathToKeyRoot.StartsWith("HKEY_CLASSES_ROOT"))
                return Registry.ClassesRoot;
            else if (pathToKeyRoot.StartsWith("HKEY_USERS"))
                return Registry.Users;
            else if (pathToKeyRoot.StartsWith("HKEY_CURRENT_CONFIG"))
                return Registry.CurrentConfig;
            else if (pathToKeyRoot.StartsWith("HKEY_PERFORMANCE_DATA"))
                return Registry.PerformanceData;
            else if (pathToKeyRoot.StartsWith("HKEY_DYN_DATA"))
                return Registry.DynData;
            else
                return null;
        }

        private static RegistryKey OpenSubKey(RegistryKey key, string path)
        {
            RegistryKey reg = key?.OpenSubKey(path, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);

            if (reg != null)
                return reg;

            key.CreateSubKey(path, RegistryKeyPermissionCheck.ReadWriteSubTree);

            return OpenSubKey(key, path);
        }

        internal static void InternalOpenSubKey(string root, string keyName)
        {
            RegistryKey key = GetRootKey(root);

            foreach (string item in root.Split(char.Parse(@"\")).Skip(1).Where(item => !string.IsNullOrEmpty(item)))
            {
                RegistryKey tmp = key?.OpenSubKey(item, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
                key?.Close();

                key = tmp;
            }

            key?.Close();
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

                return value != null;
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

            if (key?.GetValueNames().Contains(logicalKey.Name) ?? false)
                rollback.type = key.GetValueKind(logicalKey.Name);

            key.SetValue(logicalKey.Name, logicalKey.Value, logicalKey.Type);
        }

        /// <summary>
        /// We do nothing with this
        /// </summary>
        private class NullObject { }
    }
}
