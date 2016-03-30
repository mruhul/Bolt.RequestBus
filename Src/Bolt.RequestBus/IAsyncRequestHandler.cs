using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IAsyncRequestHandler<in TRequest, TResult> where TRequest : IRequest
    {
        Task<IResponse<TResult>> HandleAsync(TRequest msg);
    }


    public interface IAsyncRequestHandler<in TRequest> where TRequest : IRequest
    {
        Task<IResponse> HandleAsync(TRequest msg);
    }
}