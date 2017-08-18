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

using System;

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
