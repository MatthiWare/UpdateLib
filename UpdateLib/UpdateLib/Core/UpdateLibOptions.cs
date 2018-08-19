using System;
using System.Collections.Generic;
using System.Text;

namespace MatthiWare.UpdateLib.Core
{
    public sealed class UpdateLibOptions
    {
        public string UpdateSilentArgumentName { get; set; }
        public string UpdateArgumentName { get; set; }
        public string WaitArgumentName { get; set; }
        public string RollbackArgumentName { get; set; }
        public string UpdateUrl { get; set; }
        public string ParameterPrefix { get; set; }
        public bool AllowUnsafeUrl { get; set; }
    }
}
