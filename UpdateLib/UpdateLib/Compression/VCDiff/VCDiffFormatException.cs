using System;

namespace MatthiWare.UpdateLib.Compression.VCDiff
{
    [Serializable]
    public class VCDiffFormatException : Exception
    {
        public VCDiffFormatException() { }
        public VCDiffFormatException(string message) : base(message) { }
        public VCDiffFormatException(string message, Exception inner) : base(message, inner) { }
        protected VCDiffFormatException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
