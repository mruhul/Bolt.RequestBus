using System.Collections.Generic;
using Bolt.RequestBus;
using Sample.Infrastructure.PersistentStores;

namespace Sample.EventSource
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
            store.Write("EventSource", eEvent);
        }
    }
}