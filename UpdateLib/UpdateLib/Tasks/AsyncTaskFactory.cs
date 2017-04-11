using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Tasks
{
    public class AsyncTaskFactory
    {

        public static AsyncTaskBase StartNew(Delegate action, params object[] args)
        {
            GenericTask task = new GenericTask(action, args);
            return task.Start();
        }

        public static AsyncTaskBase<T> StartNew<T>(Delegate action, params object[] args)
        {
            GenericFuncTask<T> task = new GenericFuncTask<T>(action, args);
            return task.Start();
        }

        public static AsyncTaskBase From(Delegate action, params object[] args)
        {
            return new GenericTask(action, args);
        }

        public static AsyncTaskBase<T> From<T>(Delegate action, params object[] args)
        {
            return new GenericFuncTask<T>(action, args);
        }

        private class GenericFuncTask<T> : AsyncTaskBase<T>
        {
            private Delegate action;
            private object[] args;

            public GenericFuncTask(Delegate action, params object[] args)
            {
                this.action = action;
                this.args = args;
            }

            protected override void DoWork()
            {
                Result = (T)action.DynamicInvoke(args);
            }
        }

        private class GenericTask : AsyncTaskBase
        {
            private Delegate action;
            private object[] args;

            public GenericTask(Delegate action, params object[] args)
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
