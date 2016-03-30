using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IRequestBus
    {
        IResponse<T> Send<TRequest, T>(TRequest request) where TRequest : IRequest;
        IResponse Send<TRequest>(TRequest request) where TRequest : IRequest;
        Task<IResponse<T>> SendAsync<TRequest, T>(TRequest request) where TRequest : IRequest;
        Task<IResponse> SendAsync<TRequest>(TRequest request) where TRequest : IRequest;
        
        void Publish<TEvent>(TEvent evnt) where TEvent : IEvent;
        Task PublishAsync<TEvent>(TEvent evnt) where TEvent : IEvent;
    }
}
