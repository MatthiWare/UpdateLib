using MatthiWare.UpdateLib;
using MatthiWare.UpdateLib.Logging;
using MatthiWare.UpdateLib.Logging.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
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
            try
            {

                // we still want our updater to have visual styles in case of update cmd argument switch
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Updater.Instance
                    //.ConfigureUpdateUrl("https://raw.githubusercontent.com/MatthiWare/UpdateLib.TestApp.UpdateExample/master/Dev/updatefile.xml")
                    .ConfigureUpdateUrl("http://matthiware.dev/UpdateLib/Dev/updatefile.xml")
                    .ConfigureLogger((logger) => logger.LogLevel = LoggingLevel.Debug)
                    .ConfigureLogger((logger) => logger.Writers.Add(new ConsoleLogWriter()))
                    .ConfigureLogger((logger) => logger.Writers.Add(new FileLogWriter()))
                    .ConfigureUnsafeConnections(true)
                    .ConfigureCacheInvalidation(TimeSpan.FromSeconds(30))
                    .ConfigureUpdateNeedsAdmin(false)
                    .ConfigureInstallationMode(InstallationMode.Shared)
                    .Initialize();

                Application.Run(new Form1());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                MessageBox.Show(e.ToString());
            }
            
        }
    }
}
