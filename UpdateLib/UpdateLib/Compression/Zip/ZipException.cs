using System;

namespace MatthiWare.UpdateLib.Compression.Zip
{

    [Serializable]
    public class ZipException : Exception
    {
        public ZipException() { }
        public ZipException(string message) : base(message) { }
        public ZipException(string message, Exception inner) : base(message, inner) { }
        protected ZipException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
