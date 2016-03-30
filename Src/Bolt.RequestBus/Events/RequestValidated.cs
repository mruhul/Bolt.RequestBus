namespace Bolt.RequestBus.Events
{
    public class RequestValidated<TRequest> : IEvent
    {
        public RequestValidated(TRequest msg)
        {
            Request = msg;
        }

        public TRequest Request { get; set; }
    }
}