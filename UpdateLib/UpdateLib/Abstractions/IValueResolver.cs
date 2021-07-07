using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.UpdateLib.Common;

namespace MatthiWare.UpdateLib.Abstractions
{
    public interface IValueResolver<T>
    {
        T Resolve();

    }
}
