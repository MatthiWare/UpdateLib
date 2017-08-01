using MatthiWare.UpdateLib.Tasks;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace UpdateLib.Tests.Tasks
{
    [TestFixture]
    public class AsyncTaskTest
    {

        [Test, Parallelizable]
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

        [Test, Parallelizable]
        public void FaultyTaskReturnsException()
        {
            ErrorTask task = new ErrorTask();
            //ManualResetEvent wait = new ManualResetEvent(false);

            task.TaskCompleted += (o, e) =>
            {
                Assert.False(e.Cancelled, "The task got cancelled");
                Assert.NotNull(e.Error, "The error object is null");
                Assert.IsInstanceOf<AsyncTaskTestException>(e.Error, $"{e.Error} is not an instance of {nameof(AsyncTaskTestException)}");
                //  wait.Set();
            };
            task.Start();
            task.AwaitTask();
            //wait.WaitOne();
        }

        [Test, Parallelizable]
        public void TestMethod()
        {
            object o = new object();
            ResultTask<object> task = new ResultTask<object>(o);
            task.Start();
            Assert.AreEqual(o, task.AwaitTask().Result);
        }

        [Test, Parallelizable]
        public void TestResultReturnsCorrectObject()
        {
            TestResultTask<bool>(false);
            TestResultTask<int>(19951);
            TestResultTask<object>(new object());
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

            Assert.AreEqual(input, task.AwaitTask().Result);
        }

        [Test, Parallelizable]
        public void CheckDoubleWait()
        {
            TestTask task = new TestTask(100);
            task.Start();
            task.AwaitTask();
            task.AwaitTask();
        }

        [Test, Parallelizable]
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

        private class WorkerTestTask : AsyncTask
        {
            protected override void DoWork()
            {
                bool value = false;

                Action simulation = new Action(() =>
                {
                    Thread.Sleep(1000);
                    value = true;
                });

                Assert.IsFalse(value);

                Enqueue(simulation);

                AwaitWorkers();

                Assert.IsTrue(value);
            }
        }

        private class TestTask : AsyncTask
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

        private class ErrorTask : AsyncTask
        {
            protected override void DoWork()
            {
                Action a = new Action(() => { Thread.Sleep(1000); throw new AsyncTaskTestException(); });

                Enqueue(a);

                //throw new InvalidOperationException();

                AwaitWorkers();
            }
        }

        private class ResultTask<T> : AsyncTask<T>
        {
            private T returnObj;

            public ResultTask(T returnObj)
            {
                this.returnObj = returnObj;
            }

            protected override void DoWork()
            {
                Action<int> call = new Action<int>((x) =>
                {
                    Thread.Sleep(x);
                    Result = returnObj;
                });

                Enqueue(call, 50);
            }
        }

        private class AsyncTaskTestException : Exception
        {
            public AsyncTaskTestException() : base("Test Exception")
            {
            }

            public AsyncTaskTestException(string message) : base(message)
            {
            }

            public AsyncTaskTestException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected AsyncTaskTestException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }


}
