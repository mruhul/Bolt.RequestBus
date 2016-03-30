namespace Bolt.RequestBus.Events
{
    public class RequestCompleted<TRequest> : IEvent
        where TRequest : IRequest
    {
        public RequestCompleted(TRequest msg)
        {
            Request = msg;
        }

        public TRequest Request { get; }
    }

    public class RequestCompleted<TRequest, TResult> : IEvent
        where TRequest : IRequest
    {
        public RequestCompleted(TRequest msg, TResult result)
        {
            Request = msg;
            Result = result;
        }

        public TRequest Request { get; }
        public TResult Result { get; }
    }
}