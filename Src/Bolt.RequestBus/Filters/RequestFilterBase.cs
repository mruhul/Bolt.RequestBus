namespace Bolt.RequestBus.Filters
{
    public abstract class RequestFilterBase<TRequest> 
        : IRequestFilter<TRequest> where TRequest : IRequest
    {
        public virtual void OnInit(TRequest request)
        {
        }

        public virtual void OnValidated(TRequest request)
        {
        }
    }

    public abstract class RequestFilterBase<TRequest, TValue>
        : IRequestFilter<TRequest, TValue> where TRequest : IRequest
    {
        public virtual void OnInit(TRequest request)
        {
        }

        public virtual void OnValidated(TRequest request)
        {
        }

        public virtual void OnCompleted(TRequest request, TValue value)
        {
        }
    }
}
