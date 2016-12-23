using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Files
{
    public interface IUpdateFile
    {
        Version Version { get; }

        void AddFile(IFile file);

        void Generate();

        IUpdateFile LoadFile(String path);
    }
}
