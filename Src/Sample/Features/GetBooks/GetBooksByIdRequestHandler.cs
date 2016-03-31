using System.Linq;
using Bolt.RequestBus.Handlers;
using Sample.Features.Shared;
using Sample.Infrastructure.PersistentStores;

namespace Sample.Features.GetBooks
{
    public class GetBooksByIdRequestHandler : RequestHandlerBase<GetBookByIdRequest, BookDto>
    {
        private readonly IPersistentStore store;

        public GetBooksByIdRequestHandler(IPersistentStore store)
        {
            this.store = store;
        }

        protected override BookDto Process(GetBookByIdRequest msg)
        {
            return store.Read<BookRecord>()
                .Where(x => x.Id == msg.Id)
                .Select(x => new BookDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Price = x.Price,
                    Author = x.Author
                })
                .FirstOrDefault(record => record.Id == msg.Id);
        }
    }
}