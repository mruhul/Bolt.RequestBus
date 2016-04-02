using System;
using System.Web.Http;
using Bolt.RequestBus;
using Sample.Api.Features.GetBooks;
using Sample.Api.Features.Shared.Extensions;
using Sample.Api.Infrastructure.Extensions;

namespace Sample.Api.Features.CreateBook
{
    [RoutePrefix("v1/books")]
    public class BooksController : ApiController
    {
        private readonly IRequestBus bus;

        public BooksController(IRequestBus bus)
        {
            this.bus = bus;
        }

        [HttpGet]
        [Route("{id}", Name = RouteNames.BookById)]
        public IHttpActionResult Get([FromUri]GetBookByIdRequest request)
        {
            var response = bus.Send<GetBookByIdRequest, BookDto>(request);

            return this.ResponseResult(response);
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