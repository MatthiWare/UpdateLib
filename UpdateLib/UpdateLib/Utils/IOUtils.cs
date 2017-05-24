using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Utils
{
    public static class IOUtils
    {

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

        public static string GetAppDataPath()
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
