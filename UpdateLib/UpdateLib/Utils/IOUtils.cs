/*  UpdateLib - .Net auto update library
 *  Copyright (C) 2016 - MatthiWare (Matthias Beerens)
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published
 *  by the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
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
