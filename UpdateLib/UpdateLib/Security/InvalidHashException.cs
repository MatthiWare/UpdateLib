using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MatthiWare.UpdateLib.Security
{
    [Serializable]
    public class InvalidHashException : Exception
    {
        public InvalidHashException() { }

        public InvalidHashException(string message) :
            base(message)
        { }

        public InvalidHashException(string message, Exception inner) :
            base(message, inner)
        { }

        protected InvalidHashException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        { }
    }
}
