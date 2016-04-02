using System.Linq;
using Bolt.RequestBus.Handlers;
using Sample.Api.Features.Shared;
using Sample.Api.Infrastructure.PersistentStores;

namespace Sample.Api.Features.GetBooks
{
    public class GetBooksByIdRequestHandler : RequestHandlerBase<GetBookByIdRequest, BookDto>
    {
        private readonly IBookRepository repository;

        public GetBooksByIdRequestHandler(IBookRepository repository)
        {
            this.repository = repository;
        }

        protected override BookDto Process(GetBookByIdRequest msg)
        {
            var result = repository.GetById(msg.Id);

            return result != null 
                ? new BookDto
                {
                    Id = result.Id,
                    Title = result.Title,
                    Price = result.Price,
                    Author = result.Author
                } 
                : null;
        }
    }
}