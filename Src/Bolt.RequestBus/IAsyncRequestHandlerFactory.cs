namespace Bolt.RequestBus
{
    public interface IAsyncRequestHandlerFactory<in TRequest, TReturn> where TRequest : IRequest
    {
        IAsyncRequestHandler<TRequest, TReturn> Create();
    }
    

    public interface IAsyncRequestHandlerFactory<in TRequest> where TRequest : IRequest
    {
        IAsyncRequestHandler<TRequest> Create();
    }
}