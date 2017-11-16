using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Compression.GZip
{

    [Serializable]
    public class GZipException : Exception
    {
        public GZipException() { }
        public GZipException(string message) : base(message) { }
        public GZipException(string message, Exception inner) : base(message, inner) { }
        protected GZipException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
