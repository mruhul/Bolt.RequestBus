using System;
using Bolt.RequestBus;

namespace Sample.Features.GetBooks
{
    public class GetBookByIdRequest : IRequest
    {
        public Guid Id { get; set; }
    }
}
