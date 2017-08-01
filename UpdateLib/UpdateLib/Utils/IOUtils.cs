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

        internal static void ReinitializeAppData()
        {
            m_getAppDataPath.Reset();
        }

        public static string AppDataPath { get { return m_getAppDataPath.Value; } }

        public static string GetRemoteBasePath(string url)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));

            const char slash = '/';
            const char backslash = '\\';

            StringBuilder builder = new StringBuilder();

            foreach (var s in url.Split(slash, backslash).SkipLast(1))
            {
                builder.Append(s);
                builder.Append(slash);

            }

            return builder.ToString();

            //return .AppendAll(splitter.ToString());
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
