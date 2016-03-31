using System;
using Bolt.Logger;
using Bolt.RequestBus.Handlers;

namespace Sample.CreateBook
{
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
}