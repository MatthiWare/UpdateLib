namespace MatthiWare.UpdateLib.Common
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
        String,
        OptionalString,
        Int,
        OptionalInt,
        Bool,
        OptionalBool,
        MultipleInts
    }

    public enum ParamMandatoryType
    {
        Optional,
        Required
    }

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

    public enum LoggingLevel
    {
        Debug = 0,
        Info = 1,
        Warn = 2,
        Error = 3
    }

    public enum VersionLabel : byte
    {
        Alpha=0,
        Beta=1,
        RC=2,
        None=3
    }
}
