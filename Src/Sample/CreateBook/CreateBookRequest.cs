using System;
using Bolt.Logger;
using Bolt.RequestBus;
using Bolt.RequestBus.Handlers;

namespace Sample.CreateBook
{
    public class CreateBookRequest : Bolt.RequestBus.IRequest
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Author { get; set; }
    }

    public class CreateBookRequestHandler : RequestHandlerBase<CreateBookRequest, Guid>
    {
        private readonly ILogger logger;

        public CreateBookRequestHandler(ILogger logger)
        {
            this.logger = logger;
        }

        protected override Guid Process(CreateBookRequest msg)
        {
            var id = Guid.NewGuid();

            logger.Info($"New book created with id {id} and title {msg.Title}");

            return id;
        }
    }

    public class EventSourceHandler<TEvent> : IEventHandler<TEvent> where TEvent : IEvent
    {
        public void Handle(TEvent eEvent)
        {
            throw new NotImplementedException();
        }
    }
}
