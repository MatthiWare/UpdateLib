using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Tasks
{
    public class AsyncTaskFactory
    {

        public static AsyncTaskBase StartNew(Action action)
        {
            ParameterLessTask task = new ParameterLessTask(action);
            return task.Start();
        }

        public static AsyncTaskBase StartNew<T>(Action<T> action, T obj)
        {
            GenericTask1<T> task = new GenericTask1<T>(action, obj);
            return task.Start();
        }

        public static AsyncTaskBase StartNew<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 input1, T2 input2, T3 input3, T4 input4)
        {
            GenericTask4<T1, T2, T3, T4> task = new GenericTask4<T1, T2, T3, T4>(action, input1, input2, input3, input4);
            return task.Start();
        }

        private class GenericTask4<T1, T2, T3, T4> : AsyncTaskBase
        {
            private Action<T1, T2, T3, T4> action;
            private T1 input1;
            private T2 input2;
            private T3 input3;
            private T4 input4;

            public GenericTask4(Action<T1, T2, T3, T4> action, T1 input1, T2 input2, T3 input3, T4 input4)
            {
                this.action = action;
                this.input1 = input1;
                this.input2 = input2;
                this.input3 = input3;
                this.input4 = input4;
            }

            protected override void DoWork()
            {
                action.Invoke(input1, input2, input3, input4);
            }
        }

        private class GenericTask1<T> : AsyncTaskBase
        {
            private Action<T> action;
            private T param;

            public GenericTask1(Action<T> action, T param)
            {
                this.action = action;
                this.param = param;
            }

            protected override void DoWork()
            {
                action.Invoke(param);
            }
        }

        private class ParameterLessTask : AsyncTaskBase
        {
            private Action a;

            public ParameterLessTask(Action a)
            {
                this.a = a;
            }

            protected override void DoWork()
            {
                a.Invoke();
            }
        }

    }
}
