using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Common
{
    public interface IPatchGenerator
    {
        event ProgressChangedHandler ProgressChanged;
        void Generate();
    }

    public interface IPatcher
    {
        event ProgressChangedHandler ProgressChanged;
        void Patch();
    }
}
