using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Tasks
{
    public class AsyncTaskFactory
    {

        public static AsyncTask StartNew(Delegate action, params object[] args)
        {
            AnonymousTask task = new AnonymousTask(action, args);
            return task.Start();
        }

        public static AsyncTask<T> StartNew<T>(Delegate action, params object[] args)
        {
            AnonymousTask<T> task = new AnonymousTask<T>(action, args);
            return task.Start();
        }

        public static AsyncTask From(Delegate action, params object[] args)
        {
            return new AnonymousTask(action, args);
        }

        public static AsyncTask<T> From<T>(Delegate action, params object[] args)
        {
            return new AnonymousTask<T>(action, args);
        }

        private class AnonymousTask<T> : AsyncTask<T>
        {
            private Delegate action;
            private object[] args;

            public AnonymousTask(Delegate action, params object[] args)
            {
                this.action = action;
                this.args = args;
            }

            protected override void DoWork()
            {
                Result = (T)action.DynamicInvoke(args);
            }
        }

        private class AnonymousTask : AsyncTask
        {
            private Delegate action;
            private object[] args;

            public AnonymousTask(Delegate action, params object[] args)
            {
                this.action = action;
                this.args = args;
            }

            protected override void DoWork()
            {
                action.DynamicInvoke(args);
            }
        }

    }
}
