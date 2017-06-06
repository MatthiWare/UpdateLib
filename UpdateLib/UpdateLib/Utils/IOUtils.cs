using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MatthiWare.UpdateLib.Utils
{
    public static class IOUtils
    {
        private static Lazy<string> m_getAppDataPath = new Lazy<string>(GetAppDataPath);
        
        public static string AppDataPath { get { return m_getAppDataPath.Value; } }

        public static string GetRemoteBasePath(string url)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));

            const char splitter = '/';

            string[] tokens = url.Split(splitter);
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < tokens.Length - 1; i++)
            {
                builder.Append(tokens[i]);
                builder.Append(splitter);
            }

            return builder.ToString();
        }

        private static string GetAppDataPath()
        {
            string path = GetPathPrefix();
            string updaterName = Updater.UpdaterName;
            string productName = Updater.ProductName;

            return $@"{path}\{productName}\{updaterName}";
        }

        private static string GetPathPrefix()
        {
            switch (Updater.Instance.InstallationMode)
            {
                case InstallationMode.Local:
                    return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                case InstallationMode.Shared:
                default:
                    return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }

           
        }

    }
}
