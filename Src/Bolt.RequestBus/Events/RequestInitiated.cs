namespace Bolt.RequestBus.Events
{
    public class RequestInitiated<TRequest> : IEvent
        where TRequest : IRequest
    {
        public RequestInitiated(TRequest msg)
        {
            Request = msg;
        }

        public TRequest Request { get; }
    }
}
