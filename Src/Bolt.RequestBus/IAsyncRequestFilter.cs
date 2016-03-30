using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IAsyncRequestFilter<in TRequest> where TRequest : IRequest
    {
        Task OnInitAsync(TRequest request);
        Task OnValidatedAsync(TRequest request);
    }
    
    public interface IAsyncRequestFilter<in TRequest, in TValue> where TRequest : IRequest
    {
        Task OnInitAsync(TRequest request);
        Task OnValidatedAsync(TRequest request);
        Task OnCompletedAsync(TRequest request, TValue value);
    }
}