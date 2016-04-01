using System;
using Bolt.RequestBus;
using Sample.Features.Shared;
using Sample.Infrastructure.PersistentStores;

namespace Sample.Features.CreateBook
{
    public class BookCreatedEventHandler : IEventHandler<BookCreatedEvent>
    {
        private readonly IPersistentStore store;

        public BookCreatedEventHandler(IPersistentStore store)
        {
            this.store = store;
        }

        public void Handle(BookCreatedEvent eEvent)
        {
            store.Write(Constants.PersistanceStoreNames.EventSource, eEvent);
        }
    }
}
