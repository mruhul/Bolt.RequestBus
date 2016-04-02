using Bolt.RequestBus;
using Sample.Api.Infrastructure;
using Sample.Api.Infrastructure.Extensions;
using Sample.Api.Infrastructure.PersistentStores;

namespace Sample.Api.Features.Shared
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
            if(eEvent is IIgnoreEventSource) return;

            store.Write(Constants.PersistanceStoreNames.EventSource, new { Type = eEvent.GetType().GetFriendlyName(), Event =  eEvent});
        }
    }
}