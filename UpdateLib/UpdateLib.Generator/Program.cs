using MatthiWare.UpdateLib.Logging;
using MatthiWare.UpdateLib.Logging.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UpdateLib.Generator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Logger.Writers.Add(new ConsoleLogWriter());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TestForm());
        }
    }
}
