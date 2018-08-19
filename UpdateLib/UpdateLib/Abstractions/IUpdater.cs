using System.Threading;
using System.Threading.Tasks;
using MatthiWare.UpdateLib.Core;

namespace MatthiWare.UpdateLib.Abstractions
{
    public interface IUpdater
    {
        Task InitializeAsync(CancellationToken cancellation = default);

        Task<CheckForUpdatesResult> CheckForUpdatesAsync(CancellationToken cancellation = default);
    }
}
