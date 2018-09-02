using MatthiWare.UpdateLib.Generator.Abstractions;
using MatthiWare.UpdateLib.Generator.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MatthiWare.UpdateLib.Utils;

namespace MatthiWare.UpdateLib.Generator
{
    public class PatchBuilder
    {
        private readonly string previousVersionFolder, currentVersionFolder, tempFolder, outputFolder;
        private int currProgress, totalprogress;
        private readonly IList<IPatchBuilderTask> tasks;

        public PatchBuilder(string prev, string curr, string output)
        {
            previousVersionFolder = prev;
            currentVersionFolder = curr;
            tempFolder = $@"{System.IO.Path.GetTempPath()}UpdateLib.Generator\{Guid.NewGuid().ToString()}\";
            outputFolder = output;
            tasks = new List<IPatchBuilderTask>();
        }

        public async Task<int> CreatePatch(Action<int> progress, CancellationToken cancellation = default)
        {
            DirectoryInfo dirCurrVersion = new DirectoryInfo(currentVersionFolder);

            var files = dirCurrVersion.GetFiles("*", SearchOption.AllDirectories);

            foreach (var fi in files)
                tasks.Add(new DeltaTask(this, fi.FullName, fi.FullName));

            totalprogress = tasks.Count;

            Action reportTaskCompleted = new Action(() => 
            {
                currProgress += 1;

                progress(currProgress / totalprogress);
            });

            await tasks.ForEachAsync(task => task.Execute().ContinueWith(x => reportTaskCompleted()), Environment.ProcessorCount, cancellation);
            

            return await Task.FromResult(0);
        }
    }
}
