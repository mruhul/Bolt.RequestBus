using System.Threading.Tasks;

namespace Bolt.RequestBus.Filters
{
    public abstract class AsyncRequestFilterBase<TRequest> 
        : IAsyncRequestFilter<TRequest> where TRequest : IRequest
    {
        public virtual Task OnInitAsync(TRequest request)
        {
            return Task.FromResult(0);
        }

        public virtual Task OnValidatedAsync(TRequest request)
        {
            return Task.FromResult(0);
        }
    }

    public abstract class AsyncRequestFilterBase<TRequest, TValue>
        : IAsyncRequestFilter<TRequest,TValue> where TRequest : IRequest
    {
        public virtual Task OnInitAsync(TRequest request)
        {
            return Task.FromResult(0);
        }

        public virtual Task OnValidatedAsync(TRequest request)
        {
            return Task.FromResult(0);
        }

        public virtual Task OnCompletedAsync(TRequest request, TValue value)
        {
            return Task.FromResult(0);
        }
    }
}
