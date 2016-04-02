using System;
using Bolt.RequestBus;
using Sample.Api.Infrastructure;

namespace Sample.Api.Features.CreateBook
{
    public class BookCreatedEvent : IEvent, IIgnoreEventSource
    {
        public Guid Id { get; set; }
    }
}
