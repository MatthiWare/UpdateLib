using System;

namespace MatthiWare.UpdateLib.Common.Exceptions
{

    [Serializable]
    public class UnableToDownloadUpdateException : Exception
    {
        public UnableToDownloadUpdateException() { }
        public UnableToDownloadUpdateException(string message) : base(message) { }
        public UnableToDownloadUpdateException(string message, Exception inner) : base(message, inner) { }
        protected UnableToDownloadUpdateException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
