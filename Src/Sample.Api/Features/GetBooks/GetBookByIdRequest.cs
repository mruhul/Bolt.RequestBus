using System;
using Bolt.RequestBus;

namespace Sample.Api.Features.GetBooks
{
    public class GetBookByIdRequest : IRequest
    {
        public Guid Id { get; set; }
    }

    public class GetAllBooksRequest : IRequest
    {
    }
}
