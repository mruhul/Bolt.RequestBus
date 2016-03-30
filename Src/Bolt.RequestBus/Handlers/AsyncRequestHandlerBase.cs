using System.Threading.Tasks;

namespace Bolt.RequestBus.Handlers
{
    public abstract class AsyncRequestHandlerBase<TRequest, TResult> : IAsyncRequestHandler<TRequest, TResult> 
        where TRequest : IRequest
    {
        public async Task<IResponse<TResult>> HandleAsync(TRequest msg)
        {
            var result = await ProcessAsync(msg);

            return Response.Succeed(result);
        }

        protected abstract Task<TResult> ProcessAsync(TRequest msg);
    }

    public abstract class AsyncRequestHandlerBase<TRequest> : IAsyncRequestHandler<TRequest>
        where TRequest : IRequest
    {
        public async Task<IResponse> HandleAsync(TRequest msg)
        {
            await ProcessAsync(msg);

            return Response.Succeed();
        }

        protected abstract Task ProcessAsync(TRequest msg);
    }
}