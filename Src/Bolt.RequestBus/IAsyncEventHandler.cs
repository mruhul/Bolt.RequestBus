using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IAsyncEventHandler<in TEvent> where TEvent : IEvent
    {
        Task HandleAsync(TEvent eEvent);
    }
}
