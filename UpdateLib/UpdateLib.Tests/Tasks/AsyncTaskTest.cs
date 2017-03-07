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
                throw new InvalidOperationException("custom error");
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
