using MatthiWare.UpdateLib.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MatthiWare.UpdateLib
{
    public class Updater
    {
        private String m_url;
        private IUpdateFile m_updateFile;
        private String m_temp;
        private Assembly m_asm;

        public Updater(String url)
        {
            m_url = url;
            m_asm = Assembly.GetEntryAssembly();
            m_temp = String.Concat(Path.GetTempPath(), m_asm.FullName, "/Updater/");
        }



        public async Task<bool> CheckForUpdates()
        {
            
            return true;
        }

        private async Task<IUpdateFile> GetUpdateFileFromUrl(String url)
        {
            WebClient client = new WebClient();
            return null;
        }

        private Version GetCurrentVersion()
        {
            return new Version(m_asm.ImageRuntimeVersion);
        }

        public void StartUpdate()
        {

        }

    }
}
