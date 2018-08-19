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
using MatthiWare.UpdateLib.Tasks;
using NUnit.Framework;
using System;
using System.Linq;

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

            u.InitializeAsync();

            Assert.IsTrue(u.IsInitialized);

            AsyncTask task = u.CleanUpTask.AwaitTask();
            AsyncTask task2 = u.UpdateCacheTask.AwaitTask();

            Assert.IsTrue(task.IsRunning || task.IsCompleted);
            Assert.IsTrue(task2.IsRunning || task2.IsCompleted);
        }
    }
}
