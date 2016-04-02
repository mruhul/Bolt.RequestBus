using System.Web.Http;
using Sample.Api.Features.Shared.Extensions;
using Sample.Api.Infrastructure.Extensions;
using Bolt.RequestBus;
using Microsoft.Owin;

namespace Sample.Api.Features.GetBooks
{
    [RoutePrefix("v1/books")]
    public class GetBooksController : ApiController
    {
        private readonly IRequestBus bus;

        public GetBooksController(IRequestBus bus)
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
    }
}