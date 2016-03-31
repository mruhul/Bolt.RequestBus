using System;
using Bolt.RequestBus;

namespace Sample.CreateBook
{
    public class EventSourceHandler<TEvent> : IEventHandler<TEvent> where TEvent : IEvent
    {
        private readonly IEventSourceRepo repo;

        public EventSourceHandler(IEventSourceRepo repo)
        {
            this.repo = repo;
        }

        public void Handle(TEvent eEvent)
        {
            repo.Save(eEvent);
        }
    }
}