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
            // we still want our updater to have visual styles in case of update cmd argument switch
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SetupLogging();
            InitializeUpdater();

            Application.Run(new Form1());
        }

        private static void SetupLogging()
        {
            Logger.Writers.Add(new ConsoleLogWriter());
            Logger.Writers.Add(new FileLogWriter());
        }

        private static void InitializeUpdater()
        {
            // Set update url
            Updater.Instance.UpdateURL = "https://raw.githubusercontent.com/MatthiWare/UpdateLib.TestApp.UpdateExample/master/Dev/updatefile.xml";
            //Updater.Instance.UpdateURL = "http://matthiware.dev/UpdateLib/Dev/updatefile.xml";

            Updater.Instance.Initialize();
        }

        
    }
}
