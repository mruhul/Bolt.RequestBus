using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Bolt.RequestBus;
using Sample.Features.CreateBook;
using Sample.Features.GetBooks;

namespace Sample.Api.Controllers
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
        [Route("{id}")]
        public IHttpActionResult Get([FromUri]GetBookByIdRequest request)
        {
            var response = bus.Send<GetBookByIdRequest, BookDto>(request);

            return this.ResponseResult(response);
        }

        [HttpPost]
        [Route("{id}")]
        public IHttpActionResult Post([FromBody] CreateBookRequest request)
        {
            var response = bus.Send<CreateBookRequest, Guid>(request);

            return this.ResponseResult(response);
        }
    }
}