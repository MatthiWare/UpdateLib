using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MatthiWare.UpdateLib.Abstractions;
using MatthiWare.UpdateLib.Abstractions.Internal;
using MatthiWare.UpdateLib.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MatthiWare.UpdateLib.Core.Internal
{
    internal class UpdateManager : IUpdateManager
    {
        private readonly IValueResolver<UpdateVersion> versionResolver;
        private readonly UpdateLibOptions options;
        private readonly ILogger logger;

        public UpdateManager(IValueResolver<UpdateVersion> versionResolver, IOptions<UpdateLibOptions> options, ILogger<UpdateManager> logger)
        {
            this.versionResolver = versionResolver;
            this.options = options.Value;
            this.logger = logger;
        }

        public Task<CheckForUpdatesResult> CheckForUpdatesAsync(CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
}
