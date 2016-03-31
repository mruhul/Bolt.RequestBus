using System.Linq;
using System.Runtime.InteropServices;
using Bolt.RequestBus;
using Bolt.RequestBus.Handlers;
using Sample.Infrastructure.PersistentStores;
using Sample.Shared;

namespace Sample.GetBooks
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