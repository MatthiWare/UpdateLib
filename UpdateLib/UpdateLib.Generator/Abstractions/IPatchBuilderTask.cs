using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatthiWare.UpdateLib.Generator.Abstractions
{
    public interface IPatchBuilderTask
    {
        Task Execute();
    }
}
