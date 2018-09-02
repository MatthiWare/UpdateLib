using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;

namespace MatthiWare.UpdateLib.Core.Internal
{
    public class UpdateLibOptionsSetup : IConfigureOptions<UpdateLibOptions>
    {
        private const string m_argUpdateSilent = "silent";
        private const string m_argUpdate = "update";
        private const string m_argWait = "wait";
        private const string m_rollback = "rollback";
        private const string m_argumentPrefix = "--";

        public void Configure(UpdateLibOptions options)
        {
            options.CommandLineArgumentPrefix = m_argumentPrefix;
            options.RollbackArgumentName = m_rollback;
            options.UpdateSilentArgumentName = m_argUpdateSilent;
            options.WaitArgumentName = m_argWait;
            options.UpdateArgumentName = m_argUpdate;
        }
    }
}
