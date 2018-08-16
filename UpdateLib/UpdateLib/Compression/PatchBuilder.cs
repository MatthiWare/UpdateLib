using MatthiWare.UpdateLib.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Compression
{
    public class PatchBuilder
    {

        public event ProgressChangedHandler ProgressChanged;

        public void Generate()
        {

        }

        protected void OnProgressChanged(bool completed, double progress)
            => ProgressChanged?.Invoke(completed, progress);

    }
}
