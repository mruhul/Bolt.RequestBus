namespace Bolt.RequestBus
{
    public interface IRequestHandler<in TRequest, out TValue> where TRequest : IRequest
    {
        IResponse<TValue> Handle(TRequest msg);
    }
    

    public interface IRequestHandler<in TRequest> where TRequest : IRequest
    {
        IResponse Handle(TRequest msg);
    }
}