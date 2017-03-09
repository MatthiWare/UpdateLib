using MatthiWare.UpdateLib.Tasks;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UpdateLib.Tests.Tasks
{
    [TestFixture]
    public class AsyncTaskTest
    {

        [Test]
        public void CancelledTaskIsCancelled()
        {
            TestTask task = new TestTask(1000);
            task.TaskCompleted += (o, e) =>
            {
                Assert.True(e.Cancelled, "The task did not cancel!");
            };
            task.Start();
            task.Cancel();
            task.AwaitTask();

            Assert.True(task.IsCancelled, "Task did not cancel");
        }

        [Test]
        public void FaultyTaskReturnsException()
        {
            ErrorTask task = new ErrorTask();
            task.TaskCompleted += (o, e) =>
            {
                Assert.False(e.Cancelled, "The task got cancelled");
                Assert.NotNull(e.Error, "The error object is null");
                Assert.IsInstanceOf<InvalidOperationException>(e.Error, $"{e.Error} is not an instance of {nameof(InvalidOperationException)}");
            };
            task.Start();
            task.AwaitTask();
        }

        [Test]
        public void TestResultReturnsCorrectObject()
        {
            TestResultTask<bool>(false);
            TestResultTask<int>(19951);
            TestResultTask<Object>(new Object());
        }

        private void TestResultTask<T>(T input)
        {
            ResultTask<T> task = new ResultTask<T>(input);
            task.TaskCompleted += (o, e) =>
            {
                Assert.False(e.Cancelled, "Task got cancelled");
                Assert.IsNull(e.Error, "There was an error");

                Assert.AreEqual(input, task.Result);
            };
            task.Start();
            task.AwaitTask();

            Assert.AreEqual(input, task.Result);
        }

        [Test]
        public void CheckDoubleWait()
        {
            TestTask task = new TestTask(500);
            task.Start();
            task.AwaitTask();
            task.AwaitTask();
        }

        [Test]
        public void TestAwaitWorker()
        {
            WorkerTestTask task = new WorkerTestTask();
            task.TaskCompleted += (o, e) =>
            {
                Assert.IsFalse(e.Cancelled, "The task got cancelled??");
                Assert.IsNull(e.Error, "there was an error");
            };
            task.Start();
            task.AwaitTask();
        }

        private class WorkerTestTask : AsyncTaskBase
        {
            protected override void DoWork()
            {
                bool value = false;

                Action simulation = new Action(() => {
                    Thread.Sleep(1000);
                    value = false;
                });

                //Enqueue(simulation.BeginInvoke(new AsyncCallback(r => simulation.EndInvoke(r)), null).AsyncWaitHandle);

                Assert.IsFalse(value);

                AwaitWorkers();

                Assert.IsTrue(value);
            }
        }

        private class TestTask : AsyncTaskBase
        {
            public int Sleep { get; set; }
            public TestTask(int sleep)
            {
                Sleep = sleep;
            }

            protected override void DoWork()
            {
                Thread.Sleep(Sleep);
            }
        }

        private class ErrorTask : AsyncTaskBase
        {
            protected override void DoWork()
            {
                Action a = new Action(() => { throw new InvalidOperationException(); });

                Enqueue(a.BeginInvoke(new AsyncCallback(r => a.EndInvoke(r)), null).AsyncWaitHandle);

                AwaitWorkers();
            }
        }

        private class ResultTask<T> : AsyncTaskBase<T>
        {
            private T returnObj;

            public ResultTask(T returnObj)
            {
                this.returnObj = returnObj;
            }

            protected override void DoWork()
            {
                Thread.Sleep(100);
                AwaitWorkers();
                Result = returnObj;
            }
        }
    }


}
