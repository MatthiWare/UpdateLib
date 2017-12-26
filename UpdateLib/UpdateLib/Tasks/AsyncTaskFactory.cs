/*  Copyright
 *  
 *  UpdateLib - .Net auto update library <https://github.com/MatthiWare/UpdateLib>
 *  
 *  File: AsyncTaskFactory.cs v0.5
 *  
 *  Author: Matthias Beerens
 *  
 *  Copyright (C) 2016 - MatthiWare
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
 *  along with this program.  If not, see <https://github.com/MatthiWare/UpdateLib/blob/master/LICENSE>.
 */

using System;

namespace MatthiWare.UpdateLib.Tasks
{
    /// <summary>
    /// Factory methods for creating and starting a new task
    /// </summary>
    public class AsyncTaskFactory
    {
        /// <summary>
        /// Starts a new task
        /// </summary>
        /// <param name="action">The action delegate</param>
        /// <param name="args">The parameters to pass to the action</param>
        /// <returns>The <see cref="AsyncTask"/> object</returns>
        public static AsyncTask StartNew(Delegate action, params object[] args)
            => new AnonymousTask(action, args).Start();

        /// <summary>
        ///  Starts a new task
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="action">The action delegate</param>
        /// <param name="args">The parameters to pass to the action</param>
        /// <returns>The <see cref="AsyncTask"/> object with <see cref="T"/> result property</returns>
        public static AsyncTask<T> StartNew<T>(Delegate action, params object[] args)
            => new AnonymousTask<T>(action, args).Start();

        /// <summary>
        /// Creates a new task
        /// </summary>
        /// <param name="action">The action delegate</param>
        /// <param name="args">The parameters to pass to the action</param>
        /// <returns>The <see cref="AsyncTask"/> object</returns>
        public static AsyncTask From(Delegate action, params object[] args)
            => new AnonymousTask(action, args);

        /// <summary>
        /// Creates a new task
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="action">The action delegate</param>
        /// <param name="args">The parameters to pass to the action</param>
        /// <returns>The <see cref="AsyncTask"/> object with <see cref="T"/> result property</returns>
        public static AsyncTask<T> From<T>(Delegate action, params object[] args)
            => new AnonymousTask<T>(action, args);

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
                => action.DynamicInvoke(args);
        }

        private class AnonymousTask<TResult> : AsyncTask<TResult, AsyncTask<TResult>>
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
                Result = (TResult)action.DynamicInvoke(args);
            }
        }

    }
}
