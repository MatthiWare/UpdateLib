using MatthiWare.UpdateLib.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatthiWare.UpdateLib.Encoders
{
    public interface IEncoder
    {
        IUpdateFile Decode();
        void Encode(IUpdateFile file);
    }
}
