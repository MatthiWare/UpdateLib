using MatthiWare.UpdateLib;
using MatthiWare.UpdateLib.Logging;
using MatthiWare.UpdateLib.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TestApp.Testing
{
    public class DummyTask : AsyncTask
    {
        protected override void DoWork()
        {
            for (int i = 0; i < 10; i++)
            {
                Enqueue(new Action<int>(ChildWorkStuff), i);
            }
        }

        Random rnd = new Random(DateTime.Now.Millisecond);

        private void ChildWorkStuff(int id)
        {
            int waitTime = rnd.Next(1000, 5000);

            Thread.Sleep(waitTime);

            Updater.Instance.Logger.Debug(nameof(ChildWorkStuff), string.Empty, $"Task[{id.ToString("X2")}] Completed");
        }


    }
}
