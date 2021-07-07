using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MatthiWare.UpdateLib.Core;

namespace MatthiWare.UpdateLib.Abstractions.Internal
{
    public interface IUpdateManager
    {
        Task<CheckForUpdatesResult> CheckForUpdatesAsync(CancellationToken cancellation);

        Task UpdateAsync(CancellationToken cancellation);
    }
}
