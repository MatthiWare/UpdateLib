using System;
using System.Reflection;
using MatthiWare.UpdateLib.Abstractions;
using MatthiWare.UpdateLib.Common;

namespace MatthiWare.UpdateLib.Core.Internal
{
    public sealed class DefaultVersionResolver : IValueResolver<UpdateVersion>
    {
        private UpdateVersion m_versionCached;

        public UpdateVersion Resolve() => GetOrResolve();

        private UpdateVersion GetOrResolve()
        {
            if (m_versionCached == null)
            {
                var attribute = Assembly.GetEntryAssembly().GetCustomAttribute<ApplicationVersionAttribute>();
                m_versionCached = attribute?.Version ?? throw new InvalidOperationException("Unable to resolve the application version from the assembly metadata. Consider using a custom resolver.");
            }

            return m_versionCached;
        }
    }
}
