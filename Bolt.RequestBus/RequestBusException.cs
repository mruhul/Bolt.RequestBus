using System;

namespace Bolt.RequestBus
{
    public class RequestBusException : Exception
    {
        public RequestBusException(string message)
            : base(message)
        {
        }

        public RequestBusException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
