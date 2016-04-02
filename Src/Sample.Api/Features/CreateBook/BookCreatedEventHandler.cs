using Bolt.RequestBus;
using Sample.Api.Features.Shared;
using Sample.Api.Infrastructure.PersistentStores;

namespace Sample.Api.Features.CreateBook
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
