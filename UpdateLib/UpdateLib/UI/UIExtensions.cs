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
        public static void InvokeOnUI<T>(this T control, Action<T> action) where T : ISynchronizeInvoke
        {
            if (control.InvokeRequired)
            {
                
                control.Invoke(new Action(() => action(control)), null);
            }
            else
            {
                action(control);
            }
            
        }

    }
}
