using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bolt.RequestBus;

namespace Sample.CreateBook
{
    public class BookCreatedEvent : IEvent
    {
        public Guid Id { get; set; }
    }
}
