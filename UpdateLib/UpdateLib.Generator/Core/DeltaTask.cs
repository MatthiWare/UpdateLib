using MatthiWare.UpdateLib.Generator.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatthiWare.UpdateLib.Generator.Core
{
    public class DeltaTask : IPatchBuilderTask
    {
        private readonly string CurrentFilePath;
        private readonly string OldFilepath;
        private readonly PatchBuilder patchBuilder;

        public DeltaTask(PatchBuilder builder, string curr, string old)
        {
            patchBuilder = builder;
            CurrentFilePath = curr;
            OldFilepath = old;
        }

        public async Task Execute()
        {
            await Task.Delay(50);
            return Task.FromResult(0);
        }
    }
}
