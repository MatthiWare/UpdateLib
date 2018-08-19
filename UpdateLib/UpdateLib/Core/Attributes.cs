using System;

namespace MatthiWare.UpdateLib.Common
{
    /// <summary>
    /// The application version that the <see cref="Updater"/> uses to know what the current version is. 
    /// This attribute should be incremented with each new release, patch, ...
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
    public sealed class ApplicationVersionAttribute : Attribute
    {
        public UpdateVersion Version { get; private set; }

        public ApplicationVersionAttribute(string version)
        {
            Version = version;
        }
    }
}
