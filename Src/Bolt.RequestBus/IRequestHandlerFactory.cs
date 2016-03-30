namespace Bolt.RequestBus
{
    public interface IRequestHandlerFactory<in TRequest, TReturn> where TRequest : IRequest
    {
        IRequestHandler<TRequest, TReturn> Create();
    }


    public interface IRequestHandlerFactory<in TRequest> where TRequest : IRequest
    {
        IRequestHandler<TRequest> Create();
    }
}