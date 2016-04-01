using Bolt.RequestBus;
using Sample.Infrastructure;
using Sample.Infrastructure.PersistentStores;

namespace Sample.Features.Shared
{
    public class EventSourceHandler<TEvent> : IEventHandler<TEvent> where TEvent : IEvent
    {
        private readonly IPersistentStore store;
        public EventSourceHandler(IPersistentStore store)
        {
            this.store = store;
        }

        public void Handle(TEvent eEvent)
        {
            store.Write(Constants.PersistanceStoreNames.EventSource, new { Type = eEvent.GetType().GetFriendlyName(), Event =  eEvent});
        }
    }
}