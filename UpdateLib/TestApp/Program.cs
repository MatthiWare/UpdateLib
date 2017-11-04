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

using MatthiWare.UpdateLib;
using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Logging;
using MatthiWare.UpdateLib.Logging.Writers;
using System;
using System.Text;
using System.Windows.Forms;

namespace TestApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Console.WriteLine(Environment.CommandLine);
            foreach (var s in Environment.GetCommandLineArgs())
                Console.WriteLine(s);

           // Environment.Exit(0);

            // we still want our updater to have visual styles in case of update cmd argument switch
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Updater.Instance
                //.ConfigureUpdateUrl("https://raw.githubusercontent.com/MatthiWare/UpdateLib.TestApp.UpdateExample/master/Dev/updatefile.xml")
                .ConfigureUpdateUrl("http://matthiware.dev/UpdateLib/Dev/updatefile.xml")
                .ConfigureLogger((logger) => logger.LogLevel = LoggingLevel.Debug)
                .ConfigureLogger((logger) => logger.Writers.Add(new ConsoleLogWriter()))
                .ConfigureLogger((logger) => logger.Writers.Add(new FileLogWriter()))
                .ConfigureAllowUnsafeConnections(true)
                .ConfigureCacheInvalidation(TimeSpan.FromSeconds(30))
                .ConfigureNeedsRestartBeforeUpdate(true)
                .ConfigureInstallationMode(InstallationMode.Shared)
                .Initialize();

            Application.Run(new Form1());


        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Console.WriteLine(e.Exception.ToString());
            MessageBox.Show(e.Exception.ToString());
        }
    }
}
