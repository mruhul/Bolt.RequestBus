using System;
using Bolt.RequestBus;

namespace Sample.Api.Features.DeleteBook
{
    public class DeleteBookRequest : IRequest
    {
        public Guid Id { get; set; }
    }
}