using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MatthiWare.UpdateLib.Generator
{
    public class PatchBuilder
    {
        private readonly string previousVersionFolder, currentVersionFolder, tempFolder, outputFolder;

        public PatchBuilder(string prev, string curr, string output)
        {
            previousVersionFolder = prev;
            currentVersionFolder = curr;
            tempFolder = $@"{System.IO.Path.GetTempPath()}UpdateLib.Generator\{Guid.NewGuid().ToString()}\";
            outputFolder = output;
        }

        public async Task<int> CreatePatch(Action<int> progress, CancellationToken cancellation = default)
        {
            progress(0);

            await Task.Delay(2000);

            progress(50);

            Console.WriteLine(previousVersionFolder);
            await Task.Delay(200);
            Console.WriteLine(currentVersionFolder);
            await Task.Delay(200);
            Console.WriteLine(tempFolder);
            await Task.Delay(200);
            Console.WriteLine(outputFolder);

            progress(75);

            await Task.Delay(1000);

            progress(100);

            return await Task.FromResult(0);
        }
    }
}
