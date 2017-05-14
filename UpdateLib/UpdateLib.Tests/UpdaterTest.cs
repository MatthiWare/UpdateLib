using MatthiWare.UpdateLib;
using MatthiWare.UpdateLib.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateLib.Tests
{
    [TestFixture]
    public class UpdaterTest
    {
        [Test]
        public void EnsureMultithreadedAccessWorks()
        {
            int samples = 10;

            AsyncTask<Updater>[] tasks = new AsyncTask<Updater>[samples];
            Updater[] updaters = new Updater[samples];

            Func<Updater> getUpdaterAction = new Func<Updater>(() => Updater.Instance);

            for (int i = 0; i < samples; i++)
                tasks[i] = AsyncTaskFactory.StartNew<Updater>(getUpdaterAction, null);

            AsyncTask.WaitAll(tasks);

            for (int i = 0; i < samples; i++)
                updaters[i] = tasks[i].Result;

            int amountOfDistinctUpdaters = updaters.Distinct().Count();

            Assert.AreEqual(1, amountOfDistinctUpdaters);
        }

        [Test]
        public void TestInitializationActuallyInitializes()
        {
            Updater u = Updater.Instance;

            u.Initialize();

            Assert.IsTrue(u.IsInitialized);

            AsyncTask task = u.CleanUpTask.AwaitTask();
            AsyncTask task2 = u.UpdateCacheTask.AwaitTask();

            Assert.IsTrue(task.IsRunning || task.IsCompleted);
            Assert.IsTrue(task2.IsRunning || task2.IsCompleted);
        }
    }
}
