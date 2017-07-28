using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Logging;
using MatthiWare.UpdateLib.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;

namespace MatthiWare.UpdateLib.Security
{
    public static class PermissionUtil
    {

        private const string uacRegistryKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
        private const string uacRegistryValue = "EnableLUA";

        private static uint STANDARD_RIGHTS_READ = 0x00020000;
        private static uint TOKEN_QUERY = 0x0008;
        private static uint TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool GetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, IntPtr TokenInformation, uint TokenInformationLength, out uint ReturnLength);

        public enum TOKEN_INFORMATION_CLASS
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin,
            TokenElevationType,
            TokenLinkedToken,
            TokenElevation,
            TokenHasRestrictions,
            TokenAccessInformation,
            TokenVirtualizationAllowed,
            TokenVirtualizationEnabled,
            TokenIntegrityLevel,
            TokenUIAccess,
            TokenMandatoryPolicy,
            TokenLogonSid,
            MaxTokenInfoClass
        }

        public enum TOKEN_ELEVATION_TYPE
        {
            TokenElevationTypeDefault = 1,
            TokenElevationTypeFull,
            TokenElevationTypeLimited
        }

        public static bool IsUacEnabled
        {
            get
            {
                return m_lazyIsUacEnabled.Value;
            }
        }

        public static bool IsProcessElevated
        {
            get
            {
                return m_lazyIsElevated.Value;
            }
        }

        private static Lazy<bool> m_lazyIsUacEnabled = new Lazy<bool>(() =>
        {
            RegistryKey uacKey = Registry.LocalMachine.OpenSubKey(uacRegistryKey, false);
            bool result = uacKey.GetValue(uacRegistryValue).Equals(1);
            return result;
        });

        private static Lazy<bool> m_lazyIsElevated = new Lazy<bool>(() =>
        {
            if (IsUacEnabled)
            {
                IntPtr tokenHandle;
                if (!OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_READ, out tokenHandle))
                {
                    Updater.Instance.Logger.Warn(nameof(PermissionUtil), nameof(IsProcessElevated), "Could not get process token.  Win32 Error Code: " + Marshal.GetLastWin32Error());
                    return false;
                }

                TOKEN_ELEVATION_TYPE elevationResult = TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault;

                int elevationResultSize = Marshal.SizeOf((int)elevationResult);
                uint returnedSize = 0;
                IntPtr elevationTypePtr = Marshal.AllocHGlobal(elevationResultSize);

                bool success = GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenElevationType, elevationTypePtr, (uint)elevationResultSize, out returnedSize);
                if (success)
                {
                    elevationResult = (TOKEN_ELEVATION_TYPE)Marshal.ReadInt32(elevationTypePtr);
                    bool isProcessAdmin = elevationResult == TOKEN_ELEVATION_TYPE.TokenElevationTypeFull;
                    return isProcessAdmin;
                }
                else
                {
                    Updater.Instance.Logger.Warn(nameof(PermissionUtil), nameof(IsProcessElevated), "Unable to determine the current elevation.");
                    return false;
                }
            }
            else
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                bool result = principal.IsInRole(WindowsBuiltInRole.Administrator);
                return result;
            }
        });


        public static bool DirectoryHasPermission(string dir, FileSystemRights accessRights = FileSystemRights.Modify)
        {
            if (string.IsNullOrEmpty(dir))
                return false;

            try
            {
                AuthorizationRuleCollection rules = Directory.GetAccessControl(dir).GetAccessRules(true, true, typeof(SecurityIdentifier));
                WindowsIdentity identity = WindowsIdentity.GetCurrent();

                foreach (FileSystemAccessRule rule in rules)
                    if (identity.Groups.Contains(rule.IdentityReference) || rule.IdentityReference == identity.User)
                        if ((accessRights & rule.FileSystemRights) == accessRights && rule.AccessControlType == AccessControlType.Allow)
                            return true;
            }
            catch (Exception e)
            {
                Updater.Instance.Logger.Warn(nameof(PermissionUtil), nameof(DirectoryHasPermission), $"Current user has no access rights to: '{dir}'{Environment.NewLine}{e.ToString()}");
            }

            return false;
        }
        
        public static bool CheckRegPermission(RegistryKeyEntry key)
        {
            try
            {
                RegistryHelper.InternalOpenSubKey(key.Parent.DestinationLocation, key.Name);
                return true;
            }
            catch (Exception ex)
            {
                Updater.Instance.Logger.Error(nameof(PermissionUtil), nameof(CheckRegPermission), ex);
                return false;
            }
        }
    }
}
