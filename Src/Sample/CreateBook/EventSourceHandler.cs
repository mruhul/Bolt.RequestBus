using System;
using Bolt.RequestBus;

namespace Sample.CreateBook
{
    public class EventSourceHandler<TEvent> : IEventHandler<TEvent> where TEvent : IEvent
    {
        public EventSourceHandler()
        {
            
        }

        public void Handle(TEvent eEvent)
        {
            throw new NotImplementedException();
        }
    }
}