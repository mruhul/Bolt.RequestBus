using System;
using Bolt.RequestBus;

namespace Sample.Features.CreateBook
{
    public class BookCreatedEvent : IEvent
    {
        public Guid Id { get; set; }
    }
}
