using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.UI
{
    public static class UIExtensions
    {
        public static void InvokeOnUI<T>(this T control, Action action) where T : ISynchronizeInvoke
        {
            if (control != null && control.InvokeRequired)
                control.Invoke(action, null);
            else
                action();

        }

        public static TResult InvokeOnUI<T, TResult>(this T control, Func<TResult> action) where T : ISynchronizeInvoke
        {
            if (control != null && control.InvokeRequired)
                return (TResult)control.Invoke(action, null);
            else
                return action();
        }

    }
}
