using System;
using System.Web.Http;
using Bolt.RequestBus;
using Sample.Api.Features.GetBooks;
using Sample.Api.Features.Shared.Extensions;
using Sample.Api.Infrastructure.Extensions;

namespace Sample.Api.Features.CreateBook
{
    [RoutePrefix("v1/books")]
    public class CreateBooksController : ApiController
    {
        private readonly IRequestBus bus;

        public CreateBooksController(IRequestBus bus)
        {
            this.bus = bus;
        }
        
        [HttpPost]
        [Route]
        public IHttpActionResult Post([FromBody] CreateBookRequest request)
        {
            var response = bus.Send<CreateBookRequest, Guid>(request);

            return this.ResponseResult(response, id => Url.BookById(id));
        }
    }
}