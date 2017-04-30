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
            SetupLogging();
            InitializeUpdater();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static void InitializeUpdater()
        {
            Updater.Instance.Initialize();
            Updater.Instance.UpdateURL = "https://raw.githubusercontent.com/MatthiWare/UpdateLib.TestApp.UpdateExample/master/Dev/updatefile.xml";
            //Updater.Instance.UpdateURL = "http://matthiware.dev/UpdateLib/Dev/updatefile.xml";
        }

        private static void SetupLogging()
        {
            Logger.Writers.Add(new ConsoleLogWriter());
            Logger.Writers.Add(new FileLogWriter());
        }
    }
}
