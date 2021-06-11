using System;

namespace net.atos.daf.ct2.exceptionhandling.entity
{
    public class DAFException : Exception
    {
        public DAFException()
        { }

        public DAFException(string message)
            : base(message)
        { }

        public DAFException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
