using System;
using Bolt.Logger;
using Bolt.RequestBus;
using Bolt.RequestBus.Handlers;
using Sample.Features.Shared;
using Sample.Infrastructure.PersistentStores;

namespace Sample.Features.CreateBook
{
    public class CreateBookRequestHandler : RequestHandlerBase<CreateBookRequest, Guid>
    {
        private readonly ILogger logger;
        private readonly IPersistentStore store;
        private readonly IRequestBus bus;

        public CreateBookRequestHandler(ILogger logger, IPersistentStore store, IRequestBus bus)
        {
            this.logger = logger;
            this.store = store;
            this.bus = bus;
        }

        protected override Guid Process(CreateBookRequest msg)
        {
            var id = Guid.NewGuid();
            
            store.Write(new BookRecord
            {
                Title = msg.Title,
                Price = msg.Price,
                Author = msg.Author,
                Id = id
            });

            logger.Info($"New book created with id {id} and title {msg.Title}");

            bus.Publish(new BookCreatedEvent { Id = id });

            return id;
        }
    }
}