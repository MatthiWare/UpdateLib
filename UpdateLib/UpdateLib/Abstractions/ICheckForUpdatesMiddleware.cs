using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MatthiWare.UpdateLib.Abstractions
{
    public interface ICheckForUpdatesMiddleware
    {
        void BeforeCheckForUpdates(CancellationToken cancellation);

        void OnCheckForUpdates(CancellationToken cancellation);

        void PostCheckForUpdates(CancellationToken cancellation);
    }
}
