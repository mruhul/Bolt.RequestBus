using System;
using Bolt.RequestBus;

namespace Sample.CreateBook
{
    public class BookCreatedEventHandler : IEventHandler<BookCreatedEvent>
    {
        public BookCreatedEventHandler()
        {
            
        }

        public void Handle(BookCreatedEvent eEvent)
        {
            throw new NotImplementedException();
        }
    }
}
